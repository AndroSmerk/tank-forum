# TankiForum — Project Overview

## Описание

Бэкенд форума для сообщества мужчин 30+, увлекающихся танковыми играми (World of Tanks). Проект представляет собой ASP.NET Core Web API на .NET 10 с PostgreSQL, JWT-аутентификацией, Swagger-документацией и расширенным логированием через Serilog.

---

##  Технологии

- **.NET 10** (net10.0)
- **ASP.NET Core** (Web API, Controllers)
- **Entity Framework Core 10** + Npgsql (PostgreSQL)
- **JWT Bearer Authentication**
- **Swagger / Swashbuckle 7.3.1**
- **Serilog** (консоль + rolling file)
- **BCrypt** (хэширование паролей)
- **Rate Limiting** (встроенный, 100 req/min per IP)

---

##  Структура проекта

```
TankiForum/
├── Controllers/           # API эндпоинты (8 контроллеров)
├── Services/              # Бизнес-логика
│   └── Interfaces/        # Интерфейсы сервисов
├── Data/
│   ├── EntityConfigurations/  # Fluent API конфигурации EF Core
│   ├── AppDbContext.cs
│   └── SeedData.cs
├── DTOs/
│   ├── Auth/
│   ├── Categories/
│   ├── Sections/
│   ├── Topics/
│   ├── Posts/
│   ├── Users/
│   ├── Clans/
│   ├── Messages/
│   └── Common/            # PaginatedResponse<T>
├── Models/                # 10 доменных моделей
├── Middleware/             # ExceptionHandlingMiddleware
├── Migrations/            # EF Core миграции
├── AgentesSettings/       # Документация для ИИ-агентов
├── Properties/            # launchSettings.json
├── wwwroot/
│   └── uploads/avatars/   # Загруженные аватары
└── logs/                  # Файлы логов (rolling daily)
```

---

##  Модели данных (10 сущностей)

### Category → Section → Topic → Post (иерархия форума)

| Модель | Поля | Связи |
|--------|------|-------|
| **ForumCategory** | Id, Name, Description | 1:N → ForumSection |
| **ForumSection** | Id, CategoryId, Name, Description | N:1 → Category; 1:N → Topic |
| **Topic** | Id, SectionId, UserId, Title, Tags, CreatedAt, UpdatedAt | N:1 → Section, User; 1:N → Post |
| **Post** | Id, TopicId, UserId, Content, QuotePostId, CreatedAt, UpdatedAt | N:1 → Topic, User; self-ref → QuotePost/QuotedByPosts; 1:N → PostLike |

### Социальные сущности

| Модель | Поля | Связи |
|--------|------|-------|
| **User** | Id, Username, Email, PasswordHash, Avatar, Vocation, Age, City, DisplayName, Rank, RankClass, Quote, FavoriteTank, LastActivityAt, Role, Respects, Achievements, Medals, CreatedAt | 1:N → Topic, Post; N:M → Clan (via UserClan); 1:N → BlockedUser |
| **Clan** | Id, Name, Description, CreatedAt | N:M → User (via UserClan) |
| **UserClan** | UserId, ClanId, JoinedAt | Многое-ко-многим User↔Clan |
| **BlockedUser** | UserId, BlockedUserId, BlockedAt | Черный список пользователя |
| **PostLike** | Id, PostId, UserId, CreatedAt | N:1 → Post, User |
| **Message** | Id, SenderId, ReceiverId, Subject, Content, IsRead, CreatedAt | Личные сообщения |

---

##  База данных (PostgreSQL)

### Таблицы и индексы
- **Users** — уникальные индексы на `Username` и `Email`
- **Clans** — уникальный индекс на `Name`
- **PostLikes** — уникальный составной индекс на `(PostId, UserId)` (один лайк на пользователя на пост)
- Все внешние ключи имеют индексы

### Каскадное удаление
- Удаление категории → каскадно удаляются секции, темы, посты
- Удаление пользователя → каскадно удаляются темы, посты, лайки, членства в кланах
- `QuotePostId` → `DeleteBehavior.SetNull` (при удалении цитируемого поста)
- `BlockedUser` и `Message` → `DeleteBehavior.Restrict` (безопасное удаление)

---

##  API Endpoints

### Auth — `/api/auth`
| Method | Endpoint | Auth | Описание |
|--------|----------|------|----------|
| POST | `/register` | — | Регистрация (username, email, password) |
| POST | `/login` | — | Логин (usernameOrEmail + password) → JWT |
| POST | `/logout` | — | Заглушка (статeless) |

### Users — `/api/users`
| Method | Endpoint | Auth | Описание |
|--------|----------|------|----------|
| GET | `/{id}` | — | Профиль пользователя |
| GET | `/search?q=` | — | Поиск пользователей по username |
| PUT | `/{id}` | ✓ | Обновление профиля + аватар (multipart/form-data) |

### Categories — `/api/categories`
| Method | Endpoint | Auth | Описание |
|--------|----------|------|----------|
| GET | `/` | — | Все категории |
| POST | `/` | Admin | Создать категорию |
| PUT | `/{id}` | Admin | Обновить категорию |
| DELETE | `/{id}` | Admin | Удалить категорию |

### Sections — `/api/sections`
| Method | Endpoint | Auth | Описание |
|--------|----------|------|----------|
| GET | `/?categoryId=` | — | Секции (с фильтром по категории) |
| GET | `/{id}` | — | Секция по ID |
| POST | `/` | Admin | Создать секцию |
| PUT | `/{id}` | Admin | Обновить секцию |
| DELETE | `/{id}` | Admin | Удалить секцию |

### Topics — `/api/topics`
| Method | Endpoint | Auth | Описание |
|--------|----------|------|----------|
| GET | `/?categoryId=&sectionId=&search=&page=&pageSize=` | — | Темы (фильтр, поиск, пагинация) |
| GET | `/{id}` | — | Тема с кол-вом постов |
| POST | `/` | ✓ | Создать тему + первый пост |
| PUT | `/{id}` | ✓ | Обновить заголовок (только автор) |
| DELETE | `/{id}` | Admin | Удалить тему |

### Posts — `/api/posts`
| Method | Endpoint | Auth | Описание |
|--------|----------|------|----------|
| GET | `/?topicId=` | — | Посты темы (с лайками) |
| POST | `/` | ✓ | Создать пост |
| PUT | `/{id}` | ✓ | Редактировать пост (только автор) |
| DELETE | `/{id}` | Admin | Удалить пост |
| POST | `/{id}/like` | ✓ | Лайк/анлайк (toggle) |

### Clans — `/api/clans`
| Method | Endpoint | Auth | Описание |
|--------|----------|------|----------|
| GET | `/` | — | Все кланы с кол-вом участников |
| POST | `/create` | ✓ | Создать клан (создатель — первый член) |
| POST | `/join` | ✓ | Вступить в клан |
| POST | `/leave` | ✓ | Покинуть клан |
| DELETE | `/?clanId=` | Admin | Удалить клан |

### Messages — `/api/messages` (все эндпоинты требуют авторизации)
| Method | Endpoint | Описание |
|--------|----------|----------|
| GET | `/inbox` | Входящие сообщения |
| GET | `/sent` | Отправленные сообщения |
| GET | `/{id}` | Детали сообщения (только участник переписки) |
| POST | `/` | Отправить сообщение |
| PUT | `/{id}/read` | Отметить как прочитанное |

### Storage — `/api/storage`
| Method | Endpoint | Описание |
|--------|----------|----------|
| GET | `/{name}` | Получить файл из uploads (изображения) |

---

##  Архитектура и паттерны

### Layer map
```
Controller → Service (Interface) → Service (Implementation) → AppDbContext (EF Core) → PostgreSQL
```

### DI Registration (Program.cs)
Все сервисы зарегистрированы как `Scoped`:
- `IAuthService → AuthService`
- `IUserService → UserService`
- `ICategoryService → CategoryService`
- `ISectionService → SectionService`
- `ITopicService → TopicService`
- `IPostService → PostService`
- `IClanService → ClanService`
- `IMessageService → MessageService`

### Middleware pipeline order
1. `ExceptionHandlingMiddleware` — глобальный обработчик ошибок
2. `Swagger` + `SwaggerUI` (только Development)
3. `CORS` — разрешены все origins
4. `RateLimiter` — 100 запросов в минуту на IP
5. `Authentication` — JWT Bearer
6. `Authorization` — политика `AdminOnly`
7. `StaticFiles` — фронтенд + `wwwroot`
8. `MapControllers`

### Обработка ошибок
- `UnauthorizedAccessException` → 403 Forbidden
- `InvalidOperationException` → 409 Conflict
- `BadHttpRequestException` → 400 Bad Request
- Все остальные исключения → 500 Internal Server Error
- Ответ в формате `{ title, status, detail }`

### Безопасность
- JWT-токен (7 дней), содержит `NameIdentifier`, `Name`, `Role`
- Пароли хэшируются через BCrypt
- Полиция `AdminOnly` для административных эндпоинтов
- Rate limiting (100 req/min)
- Валидация через Data Annotations
- Проверка на `..` в пути файлов (path traversal protection)

---

##  Seed Data

При первом запуске в Development-режиме БД заполняется:

- **10 пользователей** с русскими никами, рангами, городами, рейтингом уважения
- **3 категории**: "Ламповая броня", "Тактический полигон", "Склад нытья"
- **5 секций**: "Общий чат", "Курилка механика-водителя", "Архив легенд", "Реплеи и гайды", "Поддержка"
- **14 тем** с постами на русском — обсуждения WoT (патчи, фарм, танки, стримы, реплеи, поддержка)
- **3 клана**: "Тяжеловесы", "ПТ-Снайперы", "Арта-изгои"
- Тестовый пароль для всех: `password`

---

##  Конфигурация

### appsettings.json
- **ConnectionStrings.DefaultConnection**: PostgreSQL на localhost:5432
- **Jwt**: Key (≥32 символов), Issuer, Audience
- **Urls**: http://localhost:5000
- **Serilog**: консоль + rolling file в `logs/forum-.log`

### Миграции
1. `20260522211355_InitialCreate` — начальная схема (9 таблиц)
2. `20260523084101_AddUserTopicFieldsAndSeed` — добавлены поля User (DisplayName, Rank, RankClass, Quote, FavoriteTank, LastActivityAt) + Topic.Tags + Seed Data

---

##  Не реализовано / TODO

- ~~Фронтенд~~ ✅ (приложение ищет папку `frontend/` рядом с проектом — найдена, работает)
- ~~Личные сообщения (UI)~~ ✅ — `messages.html`, инбокс/сент/детали/отправка
- ~~Онлайн-статус~~ ✅ — heartbeat + online list
- ~~Статистика пользователя~~ ✅ — `GET /api/users/{id}/stats`
- ~~Штрафбат (Controller)~~ ✅ — BlockedUsersController
- ~~Управление кланами (UI)~~ ✅ — создание/вступление
- ~~Восстановление пароля~~ ✅ — forgot/reset endpoint'ы + фронтенд
- Уведомления (намечены в дорожной карте)
- CI/CD, мониторинг, бэкапы
- Soft delete
- Кэширование
- Экипаж / друзья (требует новой модели)
- Достижения (нужен сервис расчёта)
- Чат-виджет (нужен real-time)
- Зал Боевой Славы
- Модерация контента
- Админ-панель
