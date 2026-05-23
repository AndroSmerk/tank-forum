# PlanToDo — TankiForum

## Условные обозначения
- ✅ — реализовано (backend + frontend)
- ⚡ — backend готов, фронтенда нет
- 🌀 — на моках (localStorage / статика), надо переводить на API
- ❌ — нет ни backend, ни frontend

---

## 1. Уже работает через API

| Фича | Статус | Детали |
|------|--------|--------|
| Auth (register/login/logout) | ✅ | JWT Bearer, токен в localStorage |
| Категории | ✅ | GET/POST/PUT/DELETE `/api/categories` |
| Разделы (sections) | ✅ | GET/POST/PUT/DELETE `/api/sections` |
| Темы | ✅ | CRUD + пагинация + поиск `/api/topics` |
| Посты | ✅ | CRUD + лайки `/api/posts` |
| Профиль | ✅ | GET/PUT `/api/users/{id}`, аватар multipart |
| Поиск юзеров | ✅ | GET `/api/users/search?q=` |
| Кланы (список) | ✅ | GET `/api/clans`, memberCount |
| Файлы (storage) | ✅ | GET `/api/storage/{name}` |
| Аватар (сервер) | ✅ | Хранение в `wwwroot/uploads/avatars/` |

---

## 2. Нужно доделать

### 🔴 P1 — Личные сообщения
- **Backend**: ✅ — `MessagesController`, `MessageService`, 5 endpoint'ов
- **Frontend**: ✅ — `messages.html` создан, инбокс/отправленные/детали/ответ, кнопка в профиле
- **Статус**: Готово

### 🔴 P2 — Онлайн-статус
- **Backend**: ✅ — `POST /api/users/heartbeat` + `GET /api/users/online`
- **Frontend**: ✅ — heartbeat на всех страницах, динамический список в `feed.html` sidebar + модалка
- **Статус**: Готово

### 🔴 P3 — Статистика пользователя
- **Backend**: ✅ — `GET /api/users/{id}/stats`
- **Frontend**: ✅ — реальные цифры в `profile.html` (темы, посты, респекты)
- **Статус**: Готово

### 🟡 P4 — Экипаж / друзья
- **Backend**: ❌ — нет таблицы `Friends` или `Crews`
- **Frontend**: 🌀 — `crew.js` на `localStorage`
- **Статус**: Не реализовано (требует новой модели + Controller)

### 🟡 P5 — Достижения
- **Backend**: 🌀 — поля `Achievements`/`Medals` есть, логики нет
- **Frontend**: 🌀 — статика в HTML
- **Статус**: Не реализовано

### 🟡 P6 — Чат-виджет
- **Backend**: ❌ — нет real-time
- **Frontend**: 🌀 — `chat.js` на `localStorage`
- **Статус**: Не реализовано

### 🟡 P7 — Штрафбат
- **Backend**: ✅ — `BlockedUsersController` создан (block/unblock/get)
- **Frontend**: ✅ — кнопка в профиле, вызов API
- **Статус**: Готово

### 🟢 P8 — Зал Боевой Славы
- **Статус**: Не реализовано

### 🟢 P9 — Управление кланами (UI)
- **Backend**: ✅ — CRUD + join/leave
- **Frontend**: ✅ — кнопки создания/вступления в `feed.html`
- **Статус**: Готово

### 🟢 P10 — Восстановление пароля
- **Backend**: ✅ — `POST /api/auth/forgot-password` + `POST /api/auth/reset-password`
- **Frontend**: ✅ — форма подключена к API
- **Статус**: Готово

---

## Сводка по файлам фронтенда

| Файл | Строк | Статус | Комментарий |
|------|-------|--------|-------------|
| `index.html` | ~340 | ✅ | Auth connected |
| `feed.html` | ~436 | ✅ | Topics, categories, clans from API |
| `topic.html` | ~199 | ✅ | Topic, posts, likes, avatars from API |
| `profile.html` | ~783 | ✅ | Profile, avatar from API |
| `new-topic.html` | ~200+ | ✅ | Categories/sections from API |
| `js/api.js` | ~100 | ✅ | Core API module |
| `js/avatar.js` | ~84 | ✅ | Mixed: localStorage + server |
| `js/components.js` | ~140 | ✅ | Header, footer, chat widget (static) |
| `js/chat.js` | ~87 | 🌀 | localStorage chat |
| `js/crew.js` | ~96 | 🌀 | localStorage crew |
| `js/drafts.js` | ~116 | 🌀 | localStorage drafts (ok as-is) |
| `js/modals.js` | ~35 | ✅ | Generic modal open/close |
| `js/theme.js` | — | 🌀 | localStorage theme toggle |
| `js/toast.js` | — | ✅ | Toast notifications |
| `js/utils.js` | ~50 | ✅ | Utility functions |
