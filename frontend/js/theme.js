/* ================================================================
   ТЕМА — «Броня на ночь»
   ================================================================ */

(function () {
  const STORAGE_KEY = 'tank-theme';

  function applyTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme === 'night' ? 'night' : '');
  }

  function getSavedTheme() {
    return localStorage.getItem(STORAGE_KEY) || 'light';
  }

  function toggleTheme() {
    const current = document.documentElement.getAttribute('data-theme');
    const next = current === 'night' ? 'light' : 'night';
    applyTheme(next);
    localStorage.setItem(STORAGE_KEY, next);
  }

  // Применяем при загрузке
  applyTheme(getSavedTheme());

  // Вешаем обработчики на все тумблеры на странице
  document.addEventListener('click', function (e) {
    const toggle = e.target.closest('.theme-toggle');
    if (toggle) {
      toggleTheme();
    }
  });

  window.tankTheme = { toggleTheme, applyTheme, getSavedTheme };
})();
