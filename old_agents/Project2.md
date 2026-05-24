# TankiForum — Backend Analysis

## 1. Общая информация

| Параметр | Значение |
|-----------|---------|
| **Тип** | ASP.NET Core 10 Web API |
| **Язык** | C# (.NET 10, `net10.0`) |
| **ORM** | Entity Framework Core 10 + Npgsql (PostgreSQL) |
| **База данных** | PostgreSQL (`localhost:5432`, database `TankiForum`) |
| **Аутентификация** | JWT Bearer (7 days), BCrypt password hashing |
| **API Documentation** | Swagger / Swashbuckle 7.3.1 |
| **Логирование** | Serilog (console + rolling daily file `logs/forum-.log`) |
| **Rate Limiting** | 100 req/min per IP |
| **CORS** | Полностью открыт (any origin, any method, any header) |
| **Фронтенд** | Статика в `frontend/`, сервируется тем же сервером |
| **Solution** | `TankiForum.slnx` |

### NuGet Packages

- `BCrypt.Net-Next` 4.0.3
- `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.8
- `Microsoft.EntityFrameworkCore` 10.0.8
- `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.1
- `Serilog.AspNetCore` 9.0.0
- `Swashbuckle.AspNetCore` 7.3.1

---

## 2. Структура директорий backend

```
TankiForum\                          (корень проекта)
├── TankiForum.slnx                  (VS solution)
├── Project.md                       (старый документ)
├── frontend\                        (статический фронтенд)
└── TankiForum\                      (основной backend-проект)
    ├── Program.cs                   (точка входа, DI, pipeline)
    ├── TankiForum.csproj            (project файл)
    ├── appsettings.json             (конфигурация)
    ├── appsettings.Development.json
    ├── Properties\
    │   └── launchSettings.json
    ├── Controllers\                 (10 контроллеров)
    │   ├── AuthController.cs
    │   ├── BlockedUsersController.cs
    │   ├── CategoriesController.cs
    │   ├── ClansController.cs
    │   ├── MessagesController.cs
    │   ├── PostsController.cs
    │   ├── SectionsController.cs
    │   ├── StorageController.cs
    │   ├── TopicsController.cs
    │   └── UsersController.cs
    ├── Services\                    (бизнес-логика)
    │   ├── Interfaces\              (9 интерфейсов)
    │   │   ├── IAuthService.cs
    │   │   ├── IBlockedUserService.cs
    │   │   ├── ICategoryService.cs
    │   │   ├── IClanService.cs
    │   │   ├── IMessageService.cs
    │   │   ├── IPostService.cs
    │   │   ├── ISectionService.cs
    │   │   ├── ITopicService.cs
    │   │   └── IUserService.cs
    │   └── *Service.cs             (9 реализаций)
    ├── Models\                      (10 domain entities)
    │   ├── BlockedUser.cs
    │   ├── Clan.cs
    │   ├── ForumCategory.cs
    │   ├── ForumSection.cs
    │   ├── Message.cs
    │   ├── Post.cs
    │   ├── PostLike.cs
    │   ├── Topic.cs
    │   ├── User.cs
    │   └── UserClan.cs
    ├── DTOs\                        (Data Transfer Objects)
    │   ├── Auth\
    │   ├── Categories\
    │   ├── Clans\
    │   ├── Common\
    │   ├── Messages\
    │   ├── Posts\
    │   ├── Sections\
    │   ├── Topics\
    │   └── Users\
    ├── Data\
    │   ├── AppDbContext.cs          (EF Core DbContext)
    │   ├── SeedData.cs              (сидер для development)
    │   └── EntityConfigurations\    (10 Fluent API конфигов)
    ├── Middleware\
    │   └── ExceptionHandlingMiddleware.cs
    ├── Migrations\                  (EF Core миграции)
    │   ├── 20260523145133_InitialCreate.cs
    │   ├── 20260523145133_InitialCreate.Designer.cs
    │   └── AppDbContextModelSnapshot.cs
    └── wwwroot\
        └── uploads\avatars\        (загруженные аватарки)
```

---

## 3. Архитектура

**Layered Architecture:**
```
Controller → Service (Interface) → Service (Impl) → AppDbContext → PostgreSQL
```

### Dependency Injection (все Scoped)

| Interface | Implementation |
|-----------|---------------|
| `IAuthService` | `AuthService` |
| `IUserService` | `UserService` |
| `ICategoryService` | `CategoryService` |
| `ISectionService` | `SectionService` |
| `ITopicService` | `TopicService` |
| `IPostService` | `PostService` |
| `IClanService` | `ClanService` |
| `IMessageService` | `MessageService` |
| `IBlockedUserService` | `BlockedUserService` |

### Middleware Pipeline

1. `ExceptionHandlingMiddleware`
2. Swagger + SwaggerUI (Development)
3. CORS (все разрешено)
4. Rate Limiter (100 req/min per IP)
5. Authentication (JWT Bearer)
6. Authorization (`"AdminOnly"` policy — `Role = "Admin"`)
7. Static Files (фронтенд + wwwroot)
8. MapControllers

**Repository-less:** сервисы работают напрямую с `AppDbContext`.

---

## 4. Модели БД (10 entities)

### ForumCategory
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `Id` | int | PK, auto-increment |
| `Name` | string | max 100, required |
| `Description` | string? | max 500 |

- **Связи:** 1:N → `ForumSection` (Cascade delete)

### ForumSection
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `Id` | int | PK |
| `CategoryId` | int | FK → ForumCategory |
| `Name` | string | max 100, required |
| `Description` | string? | max 500 |

- **Связи:** N:1 → ForumCategory (Cascade), 1:N → Topic (Cascade)

### Topic
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `Id` | int | PK |
| `SectionId` | int | FK → ForumSection |
| `UserId` | int | FK → User |
| `Title` | string | max 200, required |
| `Tags` | string? | |
| `CreatedAt` | DateTime | |
| `UpdatedAt` | DateTime | |

- **Связи:** N:1 → ForumSection (Cascade), N:1 → User (Cascade), 1:N → Post (Cascade)

### Post
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `Id` | int | PK |
| `TopicId` | int | FK → Topic |
| `UserId` | int | FK → User |
| `Content` | string | required |
| `QuotePostId` | int? | FK → Post (self-ref, SetNull) |
| `CreatedAt` | DateTime | |
| `UpdatedAt` | DateTime | |

- **Связи:** N:1 → Topic (Cascade), N:1 → User (Cascade), N:1 self-ref `QuotePost`, 1:N → PostLike (Cascade)

### PostLike
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `Id` | int | PK |
| `PostId` | int | FK → Post |
| `UserId` | int | FK → User |
| `CreatedAt` | DateTime | |

- **Связи:** N:1 → Post (Cascade), N:1 → User (Cascade)
- **Unique Index:** (PostId, UserId) — один like на юзера на пост

### User
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `Id` | int | PK |
| `Username` | string | max 50, **unique** |
| `Email` | string | max 100, **unique** |
| `PasswordHash` | string | required |
| `Avatar` | string? | max 500 |
| `Vocation` | string? | max 100 |
| `Age` | int? | |
| `City` | string? | max 100 |
| `DisplayName` | string? | |
| `Rank` | string? | |
| `RankClass` | string? | |
| `Quote` | string? | |
| `FavoriteTank` | string? | |
| `LastActivityAt` | DateTime? | |
| `Role` | string | default `"User"`, max 20 |
| `Respects` | int | |
| `Achievements` | string? | max 2000 |
| `Medals` | string? | max 2000 |
| `CreatedAt` | DateTime | |

- **Связи:** 1:N → Topic, 1:N → Post, N:M → Clan (via UserClan), 1:N → BlockedUser
- **Unique Indexes:** Username, Email

### Clan
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `Id` | int | PK |
| `Name` | string | max 100, **unique** |
| `Description` | string? | max 500 |
| `CreatedAt` | DateTime | |

- **Связи:** N:M → User (via UserClan)

### UserClan (join table)
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `UserId` | int | FK → User, PK (composite) |
| `ClanId` | int | FK → Clan, PK (composite) |
| `JoinedAt` | DateTime | |

- **Delete:** Cascade для обоих FK

### BlockedUser
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `UserId` | int | FK → User, PK (composite) |
| `BlockedUserId` | int | FK → User, PK (composite) |
| `BlockedAt` | DateTime | |

- **Delete:** Restrict для обоих FK

### Message
| Поле | Тип | Ограничения |
|------|-----|-------------|
| `Id` | int | PK |
| `SenderId` | int | FK → User |
| `ReceiverId` | int | FK → User |
| `Subject` | string | max 200, required |
| `Content` | string | required |
| `IsRead` | bool | |
| `CreatedAt` | DateTime | |

- **Delete:** Restrict для обоих FK

---

## 5. API Endpoints (36 endpoints)

### Auth — `/api/auth` (без авторизации)

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| POST | `/api/auth/register` | — | Регистрация (username, email, password) → `AuthResponse` |
| POST | `/api/auth/login` | — | Логин (usernameOrEmail + password) → `AuthResponse` |
| POST | `/api/auth/logout` | — | Заглушка (stateless) |
| POST | `/api/auth/forgot-password` | — | Запрос сброса пароля |
| POST | `/api/auth/reset-password` | — | Сброс пароля по токену |

### Users — `/api/users`

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| GET | `/api/users/{id}` | — | Профиль пользователя |
| GET | `/api/users/{id}/stats` | — | Статистика (кол-во тем, постов, лайков) |
| GET | `/api/users/search?q=` | — | Поиск по username |
| PUT | `/api/users/{id}` | JWT | Обновление профиля + аватар (multipart/form-data) |
| POST | `/api/users/heartbeat` | JWT | Обновление `LastActivityAt` |
| GET | `/api/users/online` | — | Онлайн пользователи (активны <5 мин) |
| GET | `/api/users/{id}/clans` | — | Список кланов пользователя |

### Categories — `/api/categories`

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| GET | `/api/categories` | — | Все категории |
| POST | `/api/categories` | Admin | Создать категорию |
| PUT | `/api/categories/{id}` | Admin | Обновить категорию |
| DELETE | `/api/categories/{id}` | Admin | Удалить (cascade: → sections → topics → posts) |

### Sections — `/api/sections`

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| GET | `/api/sections?categoryId=` | — | Секции (фильтр по категории) |
| GET | `/api/sections/{id}` | — | Секция по ID |
| POST | `/api/sections` | Admin | Создать секцию |
| PUT | `/api/sections/{id}` | Admin | Обновить секцию |
| DELETE | `/api/sections/{id}` | Admin | Удалить (cascade: → topics → posts) |

### Topics — `/api/topics`

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| GET | `/api/topics?categoryId=&sectionId=&search=&page=&pageSize=` | — | Темы (фильтры, поиск, пагинация) |
| GET | `/api/topics/{id}` | — | Тема с кол-вом постов |
| POST | `/api/topics` | JWT | Создать тему + первый пост |
| PUT | `/api/topics/{id}` | JWT | Обновить заголовок (только автор) |
| DELETE | `/api/topics/{id}` | Admin | Удалить тему (cascade: → posts) |

### Posts — `/api/posts`

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| GET | `/api/posts?topicId=` | — | Посты темы (по CreatedAt) |
| POST | `/api/posts` | JWT | Создать пост (с поддержкой цитирования) |
| PUT | `/api/posts/{id}` | JWT | Редактировать пост (только автор) |
| DELETE | `/api/posts/{id}` | Admin | Удалить пост |
| POST | `/api/posts/{id}/like` | JWT | Тоггл лайка |

### Clans — `/api/clans`

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| GET | `/api/clans` | — | Все кланы с кол-вом членов (+ `isMember` если JWT) |
| POST | `/api/clans/create` | JWT | Создать клан (создатель = первый член) |
| POST | `/api/clans/join` | JWT | Вступить в клан |
| POST | `/api/clans/leave` | JWT | Покинуть клан |
| DELETE | `/api/clans?clanId=` | Admin | Удалить клан |

### Messages — `/api/messages` (все JWT)

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| GET | `/api/messages/inbox` | JWT | Входящие |
| GET | `/api/messages/sent` | JWT | Отправленные |
| GET | `/api/messages/{id}` | JWT | Детали (только участник) |
| POST | `/api/messages` | JWT | Отправить сообщение |
| PUT | `/api/messages/{id}/read` | JWT | Отметить как прочитано |

### BlockedUsers — `/api/blockedusers` (все JWT)

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| GET | `/api/blockedusers` | JWT | Список заблокированных |
| POST | `/api/blockedusers/block` | JWT | Заблокировать пользователя |
| POST | `/api/blockedusers/unblock` | JWT | Разблокировать |

### Storage — `/api/storage`

| Method | Route | Auth | Описание |
|--------|-------|------|----------|
| GET | `/api/storage/{name}` | — | Получить файл (с защитой от path traversal) |

---

## 6. Контроллеры

### AuthController
- `Register(RegisterRequest)` → `AuthResponse` (201)
- `Login(LoginRequest)` → `AuthResponse`
- `Logout()` → `{ message }`
- `ForgotPassword(ForgotPasswordRequest)` → `ForgotPasswordResponse`
- `ResetPassword(ResetPasswordRequest)` → `{ message }`

### UsersController
- `GetById(int id)` → `UserDto`
- `GetStats(int id)` → `UserStatsDto`
- `Search(string q)` → `List<UserDto>`
- `Update(int id, UpdateUserRequest, IFormFile? AvatarFile)` → `UserDto` (authorize + сам юзер)
- `Heartbeat()` → 200 OK
- `GetOnline()` → `List<UserDto>`
- `GetUserClans(int id)` → `List<string>`

### CategoriesController
- `GetAll()` → `List<CategoryDto>`
- `Create(CreateCategoryRequest)` → `CategoryDto` (Admin)
- `Update(int id, UpdateCategoryRequest)` → `CategoryDto` (Admin)
- `Delete(int id)` → 204 (Admin)

### SectionsController
- `GetAll(int? categoryId)` → `List<SectionDto>`
- `GetById(int id)` → `SectionDto`
- `Create(CreateSectionRequest)` → `SectionDto` (Admin)
- `Update(int id, UpdateSectionRequest)` → `SectionDto` (Admin)
- `Delete(int id)` → 204 (Admin)

### TopicsController
- `GetAll(...)` → `PaginatedResponse<TopicListDto>`
- `GetById(int id)` → `TopicDto`
- `Create(CreateTopicRequest)` → `TopicDto` (JWT)
- `Update(int id, UpdateTopicRequest)` → `TopicDto` (JWT, автор)
- `Delete(int id)` → 204 (Admin)

### PostsController
- `GetByTopic(int topicId)` → `List<PostDto>`
- `Create(CreatePostRequest)` → `PostDto` (JWT)
- `Update(int id, UpdatePostRequest)` → `PostDto` (JWT, автор)
- `Delete(int id)` → 204 (Admin)
- `Like(int id)` → `{ liked: bool }` (JWT, тоггл)

### ClansController
- `GetAll()` → `List<ClanDto>` (с `IsMember` если JWT)
- `Create(CreateClanRequest)` → `ClanDto` (JWT)
- `Delete(int? clanId)` → 204 (Admin)
- `Join(JoinClanRequest)` → `{ message }` (JWT)
- `Leave(JoinClanRequest)` → `{ message }` (JWT)

### MessagesController (все JWT)
- `GetInbox()` → `List<MessageDto>`
- `GetSent()` → `List<MessageDto>`
- `GetById(int id)` → `MessageDto` (участник)
- `Send(SendMessageRequest)` → `MessageDto`
- `MarkAsRead(int id)` → 204

### BlockedUsersController (все JWT)
- `GetBlocked()` → `List<BlockedUserDto>`
- `Block(BlockUserRequest)` → `{ message }`
- `Unblock(BlockUserRequest)` → `{ message }`

### StorageController
- `GetFile(string name)` → файл с правильным Content-Type (защита от path traversal)

---

## 7. Сервисы (бизнес-логика)

### AuthService
- **RegisterAsync:** проверка дубликатов username/email → BCrypt hash → создание User → JWT token
- **LoginAsync:** поиск по username OR email → BCrypt verify → JWT token
- **ForgotPasswordAsync:** поиск по email → генерация GUID → сохранение как BCrypt-хэш пароля → возврат токена (insecure approach)
- **ResetPasswordAsync:** итерация всех пользователей → BCrypt verify токена → сброс пароля (insecure approach)
- **GenerateJwtToken:** JWT с claims (NameIdentifier, Name, Role), 7-day expiry, HMAC-SHA256

### UserService
- **GetByIdAsync:** поиск по ID → `UserDto`
- **GetStatsAsync:** подсчет тем/постов/лайков пользователя → `UserStatsDto`
- **SearchAsync:** `username.Contains(q)`, max 20
- **UpdateAsync:** обновление полей профиля + опциональная загрузка аватара (jpg/png/gif/webp, max 5MB, GUID filename)
- **HeartbeatAsync:** `LastActivityAt = DateTime.UtcNow`
- **GetOnlineAsync:** `LastActivityAt > now - 5 min`, max 50
- **GetUserClansAsync:** список названий кланов пользователя

### CategoryService
- CRUD: GetAll, GetById, Create, Update, Delete (прямая работа с `ForumCategory`)

### SectionService
- **GetAllAsync:** опциональный фильтр по `categoryId`, включает имя категории, сортировка по CategoryId → Id
- **CreateAsync:** проверка существования категории
- Остальные: GetById, Update, Delete

### TopicService
- **GetAllAsync:** фильтры (categoryId, sectionId, search), пагинация, сортировка по UpdatedAt DESC, включает имя категории
- **GetByIdAsync:** тема + кол-во постов
- **CreateAsync:** создание темы + первого поста в одной транзакции, проверка существования секции
- **UpdateAsync:** только автор, обновляет UpdatedAt
- **DeleteAsync:** каскадное удаление

### PostService
- **GetByTopicIdAsync:** посты по CreatedAt, с информацией о юзере и кол-ве лайков
- **CreateAsync:** проверка существования темы, опционально проверка цитируемого поста, обновляет UpdatedAt темы
- **UpdateAsync:** только автор, обновляет UpdatedAt
- **LikePostAsync:** тоггл — удаляет существующий или добавляет новый, возвращает `{ liked: true/false }`

### ClanService
- **GetAllAsync:** все кланы с кол-вом членов, опционально `IsMember`
- **CreateAsync:** проверка дубликата имени, создание клана + добавление создателя первым членом
- **JoinAsync:** проверка существования клана и отсутствия членства → создание UserClan
- **LeaveAsync:** удаление UserClan
- **DeleteAsync:** каскадное удаление (UserClans)

### MessageService
- **SendAsync:** проверка существования получателя → создание Message
- **GetInboxAsync:** получатель = текущий юзер, сортировка по CreatedAt DESC
- **GetSentAsync:** отправитель = текущий юзер, сортировка по CreatedAt DESC
- **GetByIdAsync:** только если юзер — отправитель ИЛИ получатель
- **MarkAsReadAsync:** только получатель может отметить прочитанным

### BlockedUserService
- **BlockAsync:** запрет на self-block, проверка дубликата, валидация существования цели
- **UnblockAsync:** удаление записи
- **GetBlockedAsync:** список заблокированных с деталями, сортировка по BlockedAt DESC
- **IsBlockedAsync:** проверка, заблокирован ли конкретный пользователь

---

## 8. Middleware

### ExceptionHandlingMiddleware

| Exception | HTTP Status | Title |
|-----------|-------------|-------|
| `UnauthorizedAccessException` | 403 Forbidden | "Forbidden" |
| `InvalidOperationException` | 409 Conflict | "Conflict" |
| `BadHttpRequestException` / `IOException` | 400 Bad Request | "Bad Request" |
| Все остальные | 500 Internal Server Error | "Internal Server Error" |

Response: `{ title, status, detail }` (JSON)

---

## 9. Конфигурация

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TankiForum;Username=postgres;Password=root"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "TankiForum",
    "Audience": "TankiForum"
  },
  "Urls": "http://localhost:5000",
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": { "Microsoft": "Warning", "Microsoft.AspNetCore": "Warning" }
    }
  },
  "Cors": { "AllowedOrigins": ["http://localhost:5000", "http://localhost:3000"] }
}
```

### launchSettings.json
- `http`: `http://localhost:5000`, `ASPNETCORE_ENVIRONMENT=Development`
- `https`: `https://localhost:7226;http://localhost:5000`

---

## 10. Seed Data (Development)

При старте в Development режиме `SeedData.Initialize()` заполняет БД:

- **10 пользователей:** русскоязычные ники танкистов, ранги (General → Private), города, профессии, цитаты, любимые танки, респекты, медали. Пароль у всех: `"password"` (BCrypt). Один (`Командир`) — Admin.
- **3 категории:** "Ламповая броня", "Тактический полигон", "Склад нытья"
- **5 секций:** "Общий чат", "Курилка механика-водителя", "Архив легенд" (cat 1); "Реплеи и гайды" (cat 2); "Поддержка" (cat 3)
- **14 тем с постами** на русском языке (обсуждения WoT: патчи, танки, гайды, геймплей, поддержка, нытье)
- **3 клана:** "Тяжеловесы", "ПТ-Снайперы", "Арта-изгои" (один участник в первом)

---

## 11. Security

- **JWT Authentication:** claims `NameIdentifier` (userId), `Name` (username), `Role`
- **BCrypt password hashing:** все пароли хэшируются перед сохранением
- **AdminOnly policy:** `[Authorize(Policy = "AdminOnly")]` на административных endpoints
- **Rate Limiting:** 100 requests/minute per IP
- **Path traversal protection:** StorageController проверяет `..` в именах файлов
- **Data Annotations:** `[Required]`, `[MinLength]`, `[MaxLength]`, `[EmailAddress]`, `[RegularExpression]`, `[Range]` на всех DTO
- **Проверка владельца:** обновление темы/поста только автором
- **Same-user enforcement:** обновление профиля проверяет, что userId в claim совпадает с запрошенным

---

## 12. Важные детали реализации

- **Stateless logout:** эндпоинт `/logout` — заглушка (JWT stateless)
- **Forgot password:** упрощенный (небезопасный для продакшена) подход — GUID токен хранится как BCrypt-хэш пароля
- **Online status:** heartbeat (`POST /heartbeat`) обновляет `LastActivityAt`, порог онлайна — 5 минут
- **Avatar upload:** только jpg/png/gif/webp, макс 5MB, сохраняется с GUID именем, сервируется через StorageController
- **Like toggle:** один endpoint (`POST /posts/{id}/like`) добавляет и удаляет лайк; возвращает `{ liked: true/false }`
- **Cascade delete:** удаление категории каскадит через секции → темы → посты; удаление пользователя — через темы, посты, лайки, членства в кланах
- **Safe delete:** BlockedUser и Message используют Restrict (предотвращает случайное каскадное удаление)
- **QuotePost self-reference:** SetNull — удаление цитируемого поста оставляет ссылающийся пост целым
- **Frontend serving:** приложение ищет папку `frontend/` относительно проекта, solution root или base directory и сервирует статику
