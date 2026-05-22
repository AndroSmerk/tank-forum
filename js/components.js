/* ================================================================
   ЗАГРУЗЧИК КОМПОНЕНТОВ — хедер, футер, чат
   ================================================================ */

(function () {
  function getActiveClass(page) {
    const current = getCurrentPage();
    return current === page ? 'active' : '';
  }

  function renderHeader() {
    const placeholder = document.getElementById('headerPlaceholder');
    if (!placeholder) return;

    // Проверяем, есть ли уже вставленный хедер
    if (placeholder.querySelector('.header')) return;

    placeholder.innerHTML = `
      <header class="header">
        <a href="feed.html" class="header-logo">
          <span class="logo-icon">⬡</span>
          <div class="logo-text">
            БРОНЯ КРЕПКА
            <small>Танковый форум 30+</small>
          </div>
        </a>

        <button class="burger" id="burgerBtn" aria-label="Меню">
          <span></span><span></span><span></span>
        </button>

        <nav class="header-nav" id="headerNav">
          <a href="feed.html" class="${getActiveClass('feed.html')}">Лента</a>
          <a href="profile.html?self=true" class="${getActiveClass('profile.html')}">Профиль</a>
          <a href="profile.html?crew">Экипаж</a>
          <a href="new-topic.html" class="${getActiveClass('new-topic.html')}">Новая тема</a>
        </nav>

        <div class="header-actions">
          <div class="search-box" style="width:200px">
            <input type="text" id="headerSearch" placeholder="Поиск по танкам, гайдам..." />
          </div>
          <button class="theme-toggle" title="Броня на ночь"></button>
          <a href="profile.html" class="avatar-wrap">
            <img class="avatar avatar-sm" data-avatar="me" src="data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 100 100'%3E%3Crect fill='%235a5f44' width='100' height='100'/%3E%3Ccircle fill='%23d4d9b0' cx='50' cy='35' r='22'/%3E%3Ccircle fill='%23f2f4e6' cx='50' cy='35' r='18'/%3E%3Ctext x='50' y='42' text-anchor='middle' font-size='20' fill='%233d4030' font-weight='bold'%3EKOT%3C/text%3E%3C/svg%3E" alt="Avatar">
            <span class="online-dot"></span>
          </a>
        </div>
      </header>
    `;
  }

  function renderFooter() {
    const placeholder = document.getElementById('footerPlaceholder');
    if (!placeholder) return;
    if (placeholder.querySelector('.footer')) return;

    placeholder.innerHTML = `
      <footer class="footer">
        <span>© 2026 Броня Крепка — Танковый форум для настоящих мужчин</span>
        <span>Сделано с броней · v2.6.1</span>
      </footer>
    `;
  }

  function renderChatWidget() {
    const placeholder = document.getElementById('chatPlaceholder');
    if (!placeholder) return;
    if (placeholder.querySelector('.chat-widget')) return;

    placeholder.innerHTML = `
        <div class="chat-widget" id="chatWidget">
        <div class="chat-header">
          <div>
            💬 Прямая связь с экипажем
            <span class="online-count" id="crewOnlineCount">· загрузка...</span>
          </div>
          <span>▼</span>
        </div>
        <div class="chat-body">
          <div id="crewChips" style="padding:0.4rem 0.75rem;border-bottom:1px solid var(--border-light)"></div>
          <div class="chat-messages">
            <div class="chat-msg">
              <span class="chat-author" style="color:var(--rank-colonel)">Полковник:</span>
              <span class="chat-text">Ребята, кто на вечерний взвод?</span>
              <span class="chat-time">14:32</span>
            </div>
            <div class="chat-msg">
              <span class="chat-author" style="color:var(--rank-lieutenant)">Tankist:</span>
              <span class="chat-text">Я в деле! На чём катаем?</span>
              <span class="chat-time">14:33</span>
            </div>
            <div class="chat-msg">
              <span class="chat-author" style="color:var(--rank-sergeant)">Serg:</span>
              <span class="chat-text">👍🏻 E 50 M или Maus?</span>
              <span class="chat-time">14:34</span>
            </div>
            <div class="chat-msg">
              <span class="chat-author">Vova_Na_KV2:</span>
              <span class="chat-text">🚘 KV-2 only! :D</span>
              <span class="chat-time">14:35</span>
            </div>
          </div>
          <div class="chat-input-area">
            <input class="chat-input" type="text" placeholder="Написать экипажу...">
            <button class="chat-send-btn btn-icon" style="width:32px;height:32px;border:none;background:var(--olive-700);color:var(--olive-200);border-radius:var(--radius-sm)">➤</button>
          </div>
          <div class="chat-stickers">
            <span class="chat-sticker">🦊</span>
            <span class="chat-sticker">🐮</span>
            <span class="chat-sticker">🐻</span>
            <span class="chat-sticker">🐴</span>
            <span class="chat-sticker">💥</span>
            <span class="chat-sticker">🎵</span>
            <span class="chat-sticker">☕</span>
            <span class="chat-sticker">🍻</span>
          </div>
        </div>
      </div>
    `;
  }

  function loadComponents() {
    renderHeader();
    renderFooter();
    renderChatWidget();

    var searchInput = document.getElementById('headerSearch');
    if (searchInput) {
      searchInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter' && getCurrentPage() !== 'feed.html') {
          var q = this.value.trim();
          if (q) window.location.href = 'feed.html?q=' + encodeURIComponent(q);
        }
      });
    }
  }

  document.addEventListener('DOMContentLoaded', loadComponents);
})();
