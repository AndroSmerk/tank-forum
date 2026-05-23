# Броня Крепка — TankiForum

Танковый форум для возрастной аудитории 30+. ASP.NET Core 10.0 Web API + статический фронтенд на ванильном JavaScript.

---

## Технологический стек

**Бэкенд:**
- .NET 10.0 / ASP.NET Core
- PostgreSQL + Entity Framework Core 10.0
- JWT Bearer аутентификация
- BCrypt (хэширование паролей)
- Serilog (логирование)
- Swagger / Swashbuckle (API-документация)
- Rate Limiting (100 запросов/мин/IP)

**Фронтенд:**
- Ванильный HTML/CSS/JS (без фреймворков)
- Адаптивная вёрстка (mobile, tablet, desktop)
- Тёмная тема "Броня на ночь"
- Чат-виджет с поллингом
- Система автосохранения черновиков

---

## Требования

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/) (14+)
- Git

---

## Быстрый старт

### 1. Клонирование

```bash
git clone <repo-url>
cd TankiForum
```

### 2. Настройка базы данных

Создайте базу данных в PostgreSQL:

```sql
CREATE DATABASE "TankiForum";
```

### 3. Настройка конфигурации

Отредактируйте `TankiForum/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TankiForum;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "TankiForum",
    "Audience": "TankiForum"
  },
  "Urls": "http://localhost:5000"
}
```

**Важно:** `Jwt:Key` должен быть минимум 32 символа.

### 4. Применение миграций

```bash
cd TankiForum
dotnet ef database update
```

Если `dotnet ef` не установлен:

```bash
dotnet tool install --global dotnet-ef
```

### 5. Запуск

```bash
dotnet run --project TankiForum
```

Сервер запустится на `http://localhost:5000`.

При первом запуске в режиме Development база автоматически заполнится тестовыми данными (10 пользователей, 3 категории, 14 тем, 3 клана).

**Тестовые учётные данные:**
- Логин: `Командир` / Пароль: `password` (роль Admin)
- Логин: `Полковник_Медведь` / Пароль: `password`
- Логин: `Tankist_1979` / Пароль: `password`

### 6. Фронтенд

Фронтенд — статические HTML-файлы в папке `frontend/`. Они автоматически раздаются бэкендом в режиме Development.

Откройте в браузере: `http://localhost:5000`

---

## Структура проекта

```
TankiForum/
├── TankiForum.slnx              # Solution-файл
├── TankiForum/                  # ASP.NET Core Web API
│   ├── Program.cs               # Точка входа, pipeline
│   ├── appsettings.json         # Конфигурация
│   ├── Controllers/             # API-контроллеры (11 шт.)
│   ├── Services/                # Бизнес-логика (10 сервисов)
│   ├── Models/                  # Entity-модели (11 сущностей)
│   ├── Data/                    # EF Core DbContext, конфигурации, Seed
│   ├── DTOs/                    # Data Transfer Objects (32 файла)
│   ├── Middleware/              # Exception Handling
│   ├── Migrations/              # EF Core миграции
│   └── wwwroot/                 # Статические файлы (загрузки)
├── frontend/                    # SPA-подобный статический фронтенд
│   ├── index.html               # Вход / регистрация
│   ├── feed.html                # Лента тем
│   ├── topic.html               # Просмотр темы
│   ├── new-topic.html           # Создание темы
│   ├── profile.html             # Профиль пользователя
│   ├── messages.html            # Личные сообщения
│   ├── css/                     # Стили (6 файлов)
│   └── js/                      # Скрипты (9 модулей)
└── .gitignore
```

---

## Конфигурация

### appsettings.json

| Параметр | Описание | По умолчанию |
|----------|----------|-------------|
| `ConnectionStrings:DefaultConnection` | Строка подключения к PostgreSQL | — |
| `Jwt:Key` | Секретный ключ JWT (≥32 символа) | — |
| `Jwt:Issuer` | Издатель токена | TankiForum |
| `Jwt:Audience` | Аудитория токена | TankiForum |
| `Urls` | URL и порт сервера | http://localhost:5000 |
| `Serilog:MinimumLevel` | Уровень логирования | Information |

### Переменные окружения (для продакшена)

```bash
# Переопределяют appsettings.json
ConnectionStrings__DefaultConnection="Host=...;Database=TankiForum;..."
Jwt__Key="your-production-key-32-chars-minimum"
Urls="http://0.0.0.0:5000"
ASPNETCORE_ENVIRONMENT="Production"
```

---

## API-документация

В режиме Development Swagger UI доступен по адресу:

```
http://localhost:5000/swagger
```

### Основные эндпоинты

| Метод | Route | Доступ | Описание |
|-------|-------|--------|----------|
| POST | `/api/auth/register` | Публичный | Регистрация |
| POST | `/api/auth/login` | Публичный | Вход |
| GET | `/api/topics` | Публичный | Список тем |
| GET | `/api/topics/{id}` | Публичный | Детали темы |
| GET | `/api/posts?topicId=` | Публичный | Посты темы |
| POST | `/api/posts` | JWT | Создать пост |
| GET | `/api/categories` | Публичный | Категории |
| GET | `/api/users/{id}` | Публичный | Профиль |
| GET | `/api/users/online` | Публичный | Онлайн |
| GET | `/api/clans` | Публичный | Список кланов |
| POST | `/api/chat` | JWT | Отправить в чат |
| GET | `/api/chat?limit=50` | JWT | Сообщения чата |

Полный список эндпоинтов — см. [Project3.md](Project3.md).

---

## Деплой на сервер

### Windows (IIS)

1. Опубликуйте проект:

```bash
dotnet publish TankiForum -c Release -o publish
```

2. Скопируйте содержимое `publish/` на сервер.
3. Настройте IIS сайт с пулом приложений "No Managed Code".
4. Установите переменные окружения через системные настройки или `appsettings.Production.json`.
5. Скопируйте папку `frontend/` рядом с `publish/`.

### Linux (Ubuntu / Nginx + systemd)

1. Опубликуйте проект:

```bash
dotnet publish TankiForum -c Release -o /var/www/tankiforum
```

2. Скопируйте `frontend/` в `/var/www/tankiforum/frontend/`.

3. Создайте `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TankiForum;Username=...;Password=..."
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "TankiForum",
    "Audience": "TankiForum"
  },
  "Urls": "http://localhost:5000"
}
```

4. Создайте systemd-сервис `/etc/systemd/system/tankiforum.service`:

```ini
[Unit]
Description=TankiForum
After=network.target postgresql.service

[Service]
WorkingDirectory=/var/www/tankiforum
ExecStart=/usr/bin/dotnet /var/www/tankiforum/TankiForum.dll
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
User=www-data

[Install]
WantedBy=multi-user.target
```

5. Запустите:

```bash
sudo systemctl enable tankiforum
sudo systemctl start tankiforum
```

6. Настройте Nginx (reverse proxy):

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### Docker (опционально)

Создайте `Dockerfile` в корне проекта:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY TankiForum/TankiForum.csproj TankiForum/
RUN dotnet restore TankiForum/TankiForum.csproj
COPY . .
RUN dotnet publish TankiForum -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
COPY frontend /app/frontend
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "TankiForum.dll"]
```

```bash
docker build -t tankiforum .
docker run -d -p 5000:5000 -e ConnectionStrings__DefaultConnection="..." tankiforum
```

---

## Логирование

Логи пишутся в:
- **Консоль** (stdout)
- **Файлы**: `logs/forum-{yyyyMMdd}.log` (ежедневная ротация)

Уровень логирования настраивается в `appsettings.json` → `Serilog:MinimumLevel`.

---

## Разработка

**Запуск в режиме Development:**
```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project TankiForum
```

При первом запуске SeedData заполнит БД тестовыми данными (проверка: если есть хотя бы один пользователь — пропускается).

**Сброс базы данных:**
```bash
dotnet ef database drop --project TankiForum
dotnet ef database update --project TankiForum
```

---

## Документация

- [Project3.md](TankiForum/AgentsSettings/Project3.md) — полный анализ backend (модели, DTO, контроллеры, сервисы, API-маршруты)
- [Frontend2.md](frontend/AgentsSettings/Frontend2.md) — полный анализ frontend (HTML, CSS, JS, компоненты, API-взаимодействия)

## Лицензия

Проект является частной разработкой. Все права защищены.
