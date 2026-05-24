# TankiForum — Frontend Analysis

## 1. Общая информация

| Параметр | Значение |
|-----------|---------|
| **Архитектура** | Multi-page static HTML/CSS/JS (MPA, не SPA) |
| **Стейт-менеджмент** | localStorage (`tank-*` ключи) |
| **Связь с бекендом** | REST API через `fetch()` с JWT Bearer |
| **Фреймворки** | Отсутствуют — чистый vanilla JS |
| **Сборка** | Отсутствует — чистые статические файлы |

### Структура навигации

```
index.html (auth) → feed.html (лента) → topic.html (тема)
                  → new-topic.html (создать тему) → topic.html
                  → profile.html (профиль)
                  → messages.html (сообщения)
```

**Auth guard:** все страницы кроме `index.html` проверяют `api.isLoggedIn()` при загрузке и редиректят на `index.html` при отсутствии токена.

---

## 2. Структура файлов

```
frontend/
├── .gitignore
├── index.html              (авторизация)
├── feed.html               (лента тем)
├── topic.html              (просмотр темы)
├── new-topic.html          (создание темы)
├── profile.html            (профиль пользователя)
├── messages.html           (личные сообщения)
├── css/
│   ├── base.css            (переменные, сброс, утилиты)
│   ├── theme.css           (тёмная тема "Броня на ночь")
│   ├── components.css      (переиспользуемые компоненты)
│   ├── layout.css          (сетки, карточки, виджеты)
│   ├── auth.css            (стили страницы авторизации)
│   └── responsive.css      (адаптивность)
├── js/
│   ├── utils.js            (утилиты)
│   ├── api.js              (ядро API)
│   ├── theme.js            (тёмная тема)
│   ├── toast.js            (уведомления)
│   ├── avatar.js           (аватарки)
│   ├── modals.js           (модальные окна)
│   ├── components.js       (шапка/подвал/чат)
│   ├── chat.js             (виджет чата)
│   ├── drafts.js           (автосохранение черновиков)
│   └── crew.js             (экипаж/друзья)
└── assets/
    ├── icons/              (пусто, в .gitignore)
    └── images/             (пусто, в .gitignore)
```

---

## 3. HTML-страницы

### 3.1 `index.html` — Авторизация

**Назначение:** Вход, регистрация, восстановление пароля.

**Подключаемые скрипты:** `utils.js`, `api.js`, `theme.js`, `toast.js`, `avatar.js`

**Блоки:**
- **Левая колонка (`.auth-hero`)** — брендинг "БРОНЯ КРЕПКА, ТАНКОВЫЙ ФОРУМ 30+" + 6 фич (система званий, зал славы, тактический полигон, склад нытья, ламповая броня, система друг-враг)
- **Правая колонка (`.auth-card`)** — табы "Вход" / "Регистрация"
- **Форма входа** (`#formLogin`): username/email + пароль + чекбокс "запомнить"
- **Форма восстановления** (`#formRecovery`): email + ссылка
- **Форма регистрации** (`#formRegister`): никнейм, email, пароль, подтверждение, профессия, согласие с правилами

**Поведение:**
- Если уже залогинен → редирект на `feed.html`
- После успеха → сохранение `tank-token`, `tank-user-id`, `tank-username` в localStorage → редирект на `feed.html`

---

### 3.2 `feed.html` — Лента тем

**Назначение:** Главная страница форума со списком тем, сайдбаром, виджетами.

**Скрипты:** `utils.js`, `api.js`, `theme.js`, `toast.js`, `avatar.js`, `modals.js`, `components.js`, `chat.js` + inline

**Блоки:**
- **Шапка** (`#headerPlaceholder`) — инжектится `components.js`
- **Хлебные крошки:** "Форум > Лента тем"
- **Заголовок:** "Лента подразделения" со статами (4283 солдата, 312 онлайн)
- **Кнопка:** "➕ Новая тема" → `new-topic.html`
- **Табы фильтров:** "Все темы" / "💥 Склад нытья"
- **Список тем** (`#topicsContainer`) — динамически рендерятся карточки тем, сгруппированные по категориям
- **Пагинация** (`#pagination`)
- **Сайдбар:**
  - "Зал Боевой Славы" (топ-3 юзера недели)
  - "Кто онлайн" (список + кнопка показать всех)
  - "Свой экипаж" (виджет друзей)
  - "Статистика подразделения" (4 бокса: всего тем/постов/пользователей/онлайн)
  - "Твои достижения" (3 ачивки с прогрессом)
  - "Клубы по интересам" (список кланов + кнопка создать)
- **Модалки:** `#onlineModal` (полный список онлайн), `#createClanModal` (создание клана)
- **Подвал** (`#footerPlaceholder`) — инжектится `components.js`
- **Чат** (`#chatPlaceholder`) — инжектится `components.js`

---

### 3.3 `topic.html` — Просмотр темы

**Назначение:** Просмотр постов в теме, ответ, лайки, автосохранение черновика.

**Скрипты:** `utils.js`, `api.js`, `theme.js`, `toast.js`, `modals.js`, `avatar.js`, `components.js`, `chat.js`, `drafts.js` + inline

**Блоки:**
- Шапка/подвал/чат — через плейсхолдеры
- **`#topicContent`** — полностью динамический контент (рендерится JS)
  - Хлебные крошки, заголовок темы с тегами, автор, посты с аватарами/юзернеймами/рангами
  - Кнопки лайков
  - Форма ответа (с автосохранением через `drafts.js`)
- **Два варианта формы ответа:** обычная (с кнопками прикрепления/стикеров) и "Склад нытья" (с градиентом и "Поддержать бойца")
- **Параметр URL:** `?type=vent` — специальный стиль для жалобной темы

---

### 3.4 `new-topic.html` — Создание темы

**Назначение:** Создание новой темы с выбором категории/секции/тегов.

**Скрипты:** `utils.js`, `api.js`, `theme.js`, `toast.js`, `avatar.js`, `components.js`, `chat.js`, `drafts.js` + inline

**Блоки:**
- Шапка/подвал/чат — через плейсхолдеры
- **Форма** (`#newTopicForm`): заголовок, категория (загружается из API), секция (фильтруется по категории), теги, текст, прикрепление файлов, чекбокс "Склад нытья", кнопки отправить/сохранить/отмена
- Индикатор автосохранения черновика
- Категории и секции подгружаются динамически с `/api/categories`, `/api/sections`
- После успеха → редирект на `topic.html?id=N`

---

### 3.5 `profile.html` — Профиль пользователя (самая большая страница, ~820 строк)

**Назначение:** Просмотр и редактирование профиля, достижения, экипаж, блокировка.

**Скрипты:** `utils.js`, `api.js`, `theme.js`, `toast.js`, `avatar.js`, `modals.js`, `components.js`, `chat.js` + inline

**Блоки:**
- Шапка/подвал/чат — через плейсхолдеры
- **Паспортная карточка:** обложка "ТАНКОВЫЙ ПАСПОРТ", аватар, юзернейм, ранг, медали, цитата, инфо-строки (ранг, профессия, возраст, город, на форуме с, тем, постов, любимый танк, клан, респекты)
- **Кнопки действий:** редактировать профиль, достижения, экипаж, написать сообщение, штрафбат (блок), добавить в экипаж, выйти
- **История наград**
- **Модалки:**
  - `#editProfileModal` — редактирование цитаты, профессии, города, любимого танка, смена аватара
  - `#shtrafbatModal` — блокировка пользователя (username + кнопка "огонь")
  - `#achievementsModal` — список 12 ачивок с прогресс-барами
  - `#crewModal` — управление экипажем (поиск, добавление, удаление)
- **Сайдбар:**
  - Виджет достижений
  - "Боевой путь" — недавняя активность
  - "Настройки" — чекбоксы (уведомления, реплеи, тема) + кнопка смены пароля
- **Параметр URL:** `?user=USERNAME` — просмотр чужого профиля; `?crew` — автооткрытие модалки экипажа

---

### 3.6 `messages.html` — Личные сообщения

**Назначение:** Просмотр и отправка приватных сообщений.

**Скрипты:** `utils.js`, `api.js`, `theme.js`, `toast.js`, `modals.js`, `components.js` + inline

**Блоки:**
- Шапка/подвал/чат — через плейсхолдеры
- **Табы:** "Входящие" / "Отправленные"
- **Список сообщений** (`#msgContainer`) — аватар, автор, тема, превью, время
- **Модалка написания** (`#composeModal`) — to, subject, body + поиск получателя
- **Сайдбар:** информационный виджет
- **Параметр URL:** `?to=USERNAME` — предзаполнение получателя

---

## 4. CSS

### 4.1 `base.css` (134 строки) — База и переменные

**CSS custom properties — дизайн-система:**
- Палитры: `olive` (7 оттенков, зелено-военный), `steel` (5, серо-голубой), `fire` (4, красно-оранжевый), `gold` (4, желто-золотой)
- Фоны: `--bg-body`, `--bg-card`, `--bg-card-alt`, `--bg-input`, `--bg-overlay`
- Текст: `--text-primary/secondary/tertiary/inverse/link/link-hover`
- Границы и тени: `--border-color/light`, `--shadow-sm/md/lg`
- Типографика: `--font-base`, `--font-mono`
- Радиусы: `--radius-sm/md/lg/xl`
- Цвета рангов: от `--rank-private` до `--rank-general`
- Статусы: `--online-color`, `--offline-color`, `--warning-color`, `--error-color`

**CSS Reset + утилиты:** `.text-sm`, `.text-xs`, `.text-tertiary`, `.fw-500/600/700`, `.flex`, `.flex-col`, `.items-center`, `.justify-between`, `.gap-1/2/3/4`, `.mt-2/4`, `.mb-4`, `.w-full`

---

### 4.2 `theme.css` (59 строк) — Тёмная тема

- `[data-theme="night"]` — переопределение всех `--bg-*`, `--text-*`, `--border-*`, `--shadow-*`
- `.theme-toggle` — переключатель 40x22px
- Кастомный скроллбар для WebKit

---

### 4.3 `components.css` (379 строк) — Компоненты

- **Шапка** (`.header`): sticky, olive-900 фон, логотип золотом, навигация
- **Поиск** (`.search-box`)
- **Бургер-меню** (`.burger`): скрыт на десктопе
- **Кнопки:** `.btn`, `.btn-primary` (olive), `.btn-fire` (красная), `.btn-ghost`, `.btn-sm`, `.btn-gold`, `.btn-icon`
- **Формы:** `.form-group`, `.form-checkbox`, `.input`
- **Аватар:** `.avatar`, `.avatar-sm/lg/xl`, `.avatar-wrap`, `.online-dot`
- **Ранги:** `.rank-badge` с `.rank-private/corporal/sergeant/lieutenant/major/colonel/general`
- **Медали:** `.medal` (золото/серебро/огонь), `.medal-strip`
- **Теги:** `.tag`, `.tag-fire/gold/steel`
- **Подвал** (`.footer`)
- **Модалки:** `.modal-overlay`, `.modal` — overlay с fade-in/slide-in
- **Прогресс-бар:** `.progress-bar` / `.progress-bar-fill`

---

### 4.4 `layout.css` (527 строк) — Сетки и контент

- Сетка страницы: `.page-layout` (flex, max-width 1280px), `.main-content`, `.sidebar` (300px)
- Хлебные крошки (`.breadcrumbs`)
- Табы (`.tabs`/`.tab`)
- Категории (`.category-section`/`.category-header`)
- Карточки тем (`.topic-card`) — pinned variant с золотой границей
- Посты (`.post`) — layout: сайдбар (аватар) + тело (текст, действия)
- Рейтинг (`.rating-controls`/`.rating-btn`)
- Виджеты сайдбара (`.sidebar-widget`)
- Онлайн список (`.online-item`)
- Статистика (`.sidebar-stats`)
- Зал славы (`.hof-entry`)
- Ачивки (`.achievement-item`) — с locked состоянием
- Кланы (`.clan-card`/`.clan-emblem`/`.clan-info`)
- Пагинация (`.pagination`/`.page-btn`)
- Чат (`.chat-widget`) — фиксированный,右下, сворачиваемый
- Форма ответа (`.reply-form`)
- "Склад нытья" (`.vent-section`) — градиент, пунктир
- Индикатор автосохранения (`.auto-save-indicator`)

---

### 4.5 `auth.css` (158 строк) — Страница авторизации

- `.auth-page` — центрированный full-viewport, olive-градиент
- `.auth-grid` — flex side-by-side (hero + card)
- `.auth-hero` — левая колонка с брендингом
- `.auth-card` — 420px карточка
- `.usp-list`/`.usp-item` — список фич

---

### 4.6 `responsive.css` (102 строки) — Адаптивность

- **≤767px:** скрыть навигацию, показать бургер, вертикальная верстка, сайдбар на всю ширину
- **768–1023px:** сайдбар 240px, поиск 160px
- **≥1024px:** hover-эффекты на постах
- **≥1400px:** max-width 1400px, сайдбар 340px

---

## 5. JavaScript

### 5.1 `utils.js` (50 строк) — Утилиты

- `formatDate(dateStr)` — конвертация даты в относительный русский текст ("только что", "5 мин назад", "3 дн назад")
- `debounce(fn, delay)` — стандартный debounce
- `escapeHtml(str)` — экранирование HTML через DOM
- `getCurrentPage()` — имя текущего файла из URL
- `isAuthPage()` — проверка, является ли страница index.html
- `pluralize(n, forms)` — русская плюрализация (3 формы)

---

### 5.2 `api.js` (100 строк) — Ядро API (IIFE)

- `API_BASE` — пустая строка (относительные URL)
- `getToken()`, `getUserId()`, `isLoggedIn()` — чтение из localStorage
- `logout()` — очистка всех `tank-*` ключей, редирект на `index.html`
- `request(method, path, body)` — обёртка над `fetch()`:
  - Content-Type: application/json
  - Authorization: Bearer `<token>`
  - 401/403 → авто-логаут
  - Остальные ошибки → тост
- `uploadFile(method, path, formData)` — для multipart/form-data (аватары)
- **Exposed:** `api.get`, `api.post`, `api.put`, `api.del`, `api.upload`, `api.getToken`, `api.getUserId`, `api.isLoggedIn`, `api.logout`

---

### 5.3 `theme.js` (35 строк) — Тёмная тема (IIFE)

- Ключ: `tank-theme`
- `applyTheme(theme)` — устанавливает `data-theme="night"` на `<html>`
- `toggleTheme()` — переключатель
- Инициализация при загрузке
- Делегирование событий на `.theme-toggle`
- **Exposed:** `window.tankTheme`

---

### 5.4 `toast.js` (74 строки) — Уведомления (IIFE)

- Контейнер в правом верхнем углу
- `show(message, type, duration)` — 4 типа: `info`, `success`, `error`, `warning`
- Иконки: ℹ️ ✅ ❌ ⚠️
- Авто-закрытие через 4с или по клику
- **Exposed:** `window.toast.show`, `.success`, `.error`, `.warning`

---

### 5.5 `avatar.js` (84 строки) — Аватарки (IIFE)

- Ключ: `tank-avatar`
- `defaultAvatar()` — SVG data URL с буквой "KOT"
- `uploadAvatar(file, callback)` — FileReader → data URL → localStorage → MutationObserver
- `setServerAvatar(url)` — установка аватара с сервера
- `applyAvatar()` — обновление всех `<img data-avatar="me">`
- `setupUploadButton()` — скрытый `<input type="file">` + `#avatarUploadBtn`
- `watchNewAvatars()` — MutationObserver
- **Exposed:** `window.avatar`

---

### 5.6 `modals.js` (35 строк) — Модальные окна (IIFE)

- `openModal(id)` — добавляет `.open`, блокирует скролл
- `closeModal(id)` — убирает `.open`, разблокирует скролл
- Закрытие по клику на overlay и Escape
- **Exposed:** `window.modal.open`, `.close`

---

### 5.7 `components.js` (141 строка) — Шапка/Подвал/Чат (IIFE)

- `renderHeader()` — инжектит в `#headerPlaceholder`:
  - Логотип "БРОНЯ КРЕПКА"
  - Навигация: Лента, Профиль, Экипаж, Сообщения, Новая тема
  - Поиск, тоггл темы, аватар
  - Active-класс по текущей странице
- `renderFooter()` — копирайт + версия
- `renderChatWidget()` — виджет чата в `#chatPlaceholder`
- `loadComponents()` — запускает все три + поиск по Enter
- **Запуск:** `DOMContentLoaded`

---

### 5.8 `chat.js` (87 строк) — Логика чата (IIFE)

- Ключи: `tank-chat-collapsed`, `tank-chat-messages`
- `loadMessages()` — чтение истории из localStorage (max 50)
- `saveMessages(msgs)` — сохранение
- `initChat()` — сворачивание/разворачивание, отправка по Enter/кнопке, стикеры
- **Запуск:** `DOMContentLoaded`

---

### 5.9 `drafts.js` (116 строк) — Черновики (IIFE)

- Ключ: `tank-draft-{name}`
- `saveDraft(name, data)`, `loadDraft(name)`, `clearDraft(name)` — localStorage
- `initDrafts()` — находит все `[data-draft]` формы:
  - Восстанавливает поля из черновика
  - Автосохранение с debounce 800мс
  - Индикатор статуса
  - Очистка при сабмите
- **Запуск:** `DOMContentLoaded`
- **Exposed:** `window.draftsInit` для динамического контента

---

### 5.10 `crew.js` (96 строк) — Экипаж (IIFE)

- Ключ: `tank-crew`
- Дефолтный экипаж: Tankist_1979, Serg_76, Vova_Na_KV2
- `getCrew()`, `saveCrew()`, `addMember()`, `removeMember()`
- `renderCrewList()` — рендер в `#crewList` с дот-статусами и кнопками удаления
- `renderChipList()` — рендер чипсов в `#crewChips` (в шапке чата)
- `crewRemove(name)` — глобальная функция
- MutationObserver для авто-рендера
- **Exposed:** `window.crew`

---

## 6. API-запросы фронтенда

### Auth
| Метод | Endpoint | Страница | Назначение |
|-------|----------|----------|------------|
| POST | `/api/auth/login` | index.html | Логин |
| POST | `/api/auth/register` | index.html | Регистрация |
| POST | `/api/auth/forgot-password` | index.html | Сброс пароля |

### Topics
| Метод | Endpoint | Страница | Назначение |
|-------|----------|----------|------------|
| GET | `/api/topics?page=N&pageSize=20` | feed.html | Список тем |
| GET | `/api/topics?page=N&pageSize=20&search=QUERY` | feed.html | Поиск тем |
| GET | `/api/topics/{id}` | topic.html | Детали темы |
| POST | `/api/topics` | new-topic.html | Создание темы |

### Posts
| Метод | Endpoint | Страница | Назначение |
|-------|----------|----------|------------|
| GET | `/api/posts?topicId=N` | topic.html | Посты темы |
| POST | `/api/posts` | topic.html | Создание поста |
| POST | `/api/posts/{id}/like` | topic.html | Лайк/анлайк |

### Categories & Sections
| Метод | Endpoint | Страница | Назначение |
|-------|----------|----------|------------|
| GET | `/api/categories` | feed.html, new-topic.html | Список категорий |
| GET | `/api/sections` | new-topic.html | Список секций |

### Users
| Метод | Endpoint | Страница | Назначение |
|-------|----------|----------|------------|
| GET | `/api/users/{id}` | profile.html | Профиль |
| GET | `/api/users/{id}/stats` | profile.html | Статистика |
| GET | `/api/users/{id}/clans` | profile.html | Кланы юзера |
| PUT | `/api/users/{id}` | profile.html | Обновление профиля |
| GET | `/api/users/search?q=QUERY` | profile.html, messages.html | Поиск юзеров |
| POST | `/api/users/heartbeat` | Все страницы | Heartbeat (каждые 2 мин) |
| GET | `/api/users/online` | feed.html | Онлайн юзеры |

### Clans
| Метод | Endpoint | Страница | Назначение |
|-------|----------|----------|------------|
| GET | `/api/clans` | feed.html | Список кланов |
| POST | `/api/clans/create` | feed.html | Создать клан |
| POST | `/api/clans/join` | feed.html | Вступить |
| POST | `/api/clans/leave` | feed.html | Покинуть |

### Messages
| Метод | Endpoint | Страница | Назначение |
|-------|----------|----------|------------|
| GET | `/api/messages/inbox` | messages.html | Входящие |
| GET | `/api/messages/sent` | messages.html | Отправленные |
| GET | `/api/messages/{id}` | messages.html | Детали |
| POST | `/api/messages` | messages.html | Отправить |
| PUT | `/api/messages/{id}/read` | messages.html | Прочитано |

### Blocked Users
| Метод | Endpoint | Страница | Назначение |
|-------|----------|----------|------------|
| POST | `/api/blockedusers/block` | profile.html | Заблокировать |

---

## 7. Client-Side Storage (localStorage)

| Ключ | Содержимое | Модуль |
|------|------------|--------|
| `tank-token` | JWT токен | api.js |
| `tank-user-id` | ID пользователя | api.js |
| `tank-username` | Username | api.js |
| `tank-profile` | JSON профиля | profile.html |
| `tank-avatar` | Avatar data URL | avatar.js |
| `tank-theme` | `'light'` / `'night'` | theme.js |
| `tank-settings` | JSON настроек | profile.html |
| `tank-crew` | Массив членов экипажа | crew.js |
| `tank-chat-collapsed` | boolean | chat.js |
| `tank-chat-messages` | История чата | chat.js |
| `tank-draft-{name}` | Черновик формы | drafts.js |

---

## 8. Ключевые паттерны

### Компонентная система
`components.js` инжектит HTML в плейсхолдеры (`#headerPlaceholder`, `#footerPlaceholder`, `#chatPlaceholder`). Все страницы, кроме `index.html`, используют это.

### Автосохранение черновиков
Аттрибуты `[data-draft]`, `[data-draft-field]`, `[data-draft-indicator]`. `drafts.js` сам находит формы. Сохранение в localStorage с debounce 800ms.

### Система аватарок
`<img data-avatar="me">` — MutationObserver авто-применяет аватар. Фолбэк: SVG data URL с первой буквой юзернейма на цветном фоне (детерминировано, хэш строки).

### Модалки
`.modal-overlay` + `.modal`. `modals.js` управляет open/close. Закрытие по overlay и Escape.

### Тост-уведомления
`window.toast.show/success/error/warning`. 4 типа, 4s автозакрытие, slide-in анимация.

### Heartbeat
`POST /api/users/heartbeat` при загрузке страницы и каждые 2 минуты на всех авторизованных страницах.

### Ранговая система
7 уровней: private → corporal → sergeant → lieutenant → major → colonel → general. CSS-классы и переменные для каждого.
