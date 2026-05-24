# PROJECT: TankMen Forum

## Описание проекта

Форум-сообщество для одиноких мужчин 30+, увлекающихся танковыми играми.

Цель проекта:
- создать уютное взрослое комьюнити;
- дать пользователям ощущение принадлежности;
- объединить игроков;
- реализовать атмосферу старых ламповых форумов в современной реализации.

---

# Контекст

```json
{
  "role": "Ты опытный C# DotNet программист",
  "situation": "Ко мне пришёл заказчик с просьбой сделать сайт-форум для одиноких мужчин 30+, играющих в танки",
  "target": "Одинокие мужчины 30+"
}
```

---

# Технологический стек

## Backend
- C#
- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
---

# Архитектурные требования

## Использовать
- Clean Architecture
- SOLID
- Dependency Injection
- CQRS (без фанатизма)
- Feature-based structure
- Repository pattern только если действительно нужен

## Команды

```bash
dotnet restore
dotnet ef database update
dotnet run
```

## Структура проекта

```text
src/
 ├── Controllers/
 ├── DTOs/
 ├── Data/
 ├── Models/
 └── Services/
```

---

# Кодстайл и соглашения

## Обязательные настройки
- Nullable enabled
- ImplicitUsings enabled
- Async/await где необходимо

## Naming
- PascalCase → классы/методы
- camelCase → локальные переменные
- _privateField → приватные поля

## Правила
- Без god classes
- Без fat controllers
- Без бизнес-логики в controllers/pages
- Без SQL в контроллерах
- Без магических строк
- Без хардкода секретов
- Без ViewBag/ViewData

---

# Основные сущности

## User
Описание:
TODO

Поля:
- Id
- Username
- Email
- PasswordHash
- Avatar
- Vocation
- Age
- City
- CreatedThreads
- CreatedPosts
- Respects
- Clans
- Achievements
- Medals
- BlockedUsers
- CreatedAt

Дополнительно:
TODO

---

## ForumCategory
Описание:
TODO

Поля:
- Id
- Name
- Description

---

## ForumSection
Описание:
TODO

Поля:
- Id
- CategoryId
- Name
- Description

---

## Topic
Описание:
TODO

Поля:
- Id
- SectionId
- UserId
- Title
- CreatedAt
- UpdatedAt

---

## Post
Описание:
TODO

Поля:
- Id
- TopicId
- UserId
- Content
- CreatedAt
- UpdatedAt

---

## Clan
Описание:
TODO

Поля:
- Id
- Name
- Description
- CreatedAt

---

# Endpoints

## Auth
```http
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/logout
```

Дополнительно:
TODO

---

## Users
```http
GET    /api/users/{id}
PUT    /api/users/{id}
```

Дополнительно:
TODO

---

## Forum Categories
```http
GET    /api/categories
POST   /api/categories
PUT    /api/categories/{id}
DELETE /api/categories/{id}
```

Дополнительно:
TODO

---

## Sections
```http
GET    /api/sections/{id}
POST   /api/sections
PUT    /api/sections/{id}
DELETE /api/sections/{id}
```

Дополнительно:
TODO

---

## Topics
```http
GET    /api/topics/{id}
POST   /api/topics
PUT    /api/topics/{id}
DELETE /api/topics/{id}
```

Дополнительно:
TODO

---

## Posts
```http
POST   /api/posts
PUT    /api/posts/{id}
DELETE /api/posts/{id}
```

Дополнительно:
TODO

---

## Clans
```http
GET    /api/clans
POST   /api/clans/create
DELETE   /api/clans
POST   /api/clans/join
POST   /api/clans/leave
```

Дополнительно:
TODO

---

# Функциональность

## MVP
- Регистрация
- Авторизация
- Профили
- Форум
- Темы
- Сообщения
- Лайки
- Цитирование
- Личные сообщения
- Кланы

---

## Дополнительные функции
 - Swagger
 - Docs
 - Вывод адреса сервера в консоль

---

# Безопасность

## Обязательно
- JWT/Auth Cookies
- CSRF protection
- Rate limiting
- Password hashing
- Validation
- Authorization policies

## Запрещено
- Хардкод секретов
- Plain-text passwords
- SQL injection vulnerabilities

---

# Производительность

Учитывать:
- Pagination
- Индексы PostgreSQL
- EF Core query optimization
- Caching
- Lazy loading only if justified

---

# Логирование

Использовать:
- Serilog
- Structured logging

Логировать:
- Errors
- Auth events
- Important actions

---

# База данных

## PostgreSQL

Нужно продумать:
- индексы;
- миграции;
- ограничения;
- связи;
- soft delete (если нужен).

---

# Деплой

## Deployment
- systemd service
- PostgreSQL connection config
- HTTPS configuration
- production appsettings
- logging configuration

---

# TODO / ROADMAP

## Архитектура
- [ ] Определить bounded contexts
- [ ] Описать solution structure
- [ ] Описать DI strategy
- [ ] Описать error handling

## Backend
- [ ] Auth
- [ ] Forum
- [ ] Messaging
- [ ] Clans
- [ ] Notifications

## Infra
- [ ] CI/CD
- [ ] Monitoring
- [ ] Backups

---

# Ограничения

## Нельзя
- Огромные контроллеры
- GodService
- jQuery spaghetti
- Случайный JS
- Copy-paste architecture
- Микросервисы без причины
- Overengineering
- Создавать и применять миграции

---

# Definition of Done

Задача считается выполненной если:
- проект компилируется;
- архитектура не деградирует;
- код читаемый;
- решение масштабируемо;
- отсутствуют очевидные anti-patterns.