/* ================================================================
   TOAST-УВЕДОМЛЕНИЯ
   ================================================================ */

(function () {
  var container = null;

  function getContainer() {
    if (!container) {
      container = document.createElement('div');
      container.style.cssText = 'position:fixed;top:5rem;right:1rem;z-index:9999;display:flex;flex-direction:column;gap:0.5rem;max-width:380px;width:100%;pointer-events:none';
      document.body.appendChild(container);
    }
    return container;
  }

  function show(message, type, duration) {
    type = type || 'info';
    duration = duration || 4000;

    var c = getContainer();
    var el = document.createElement('div');

    var bgColor = 'var(--olive-800)';
    var borderColor = 'var(--olive-500)';
    var icon = 'ℹ️';

    if (type === 'success') {
      bgColor = 'var(--olive-700)';
      borderColor = 'var(--olive-500)';
      icon = '✅';
    } else if (type === 'error') {
      bgColor = '#3d1e18';
      borderColor = 'var(--fire-500)';
      icon = '❌';
    } else if (type === 'warning') {
      bgColor = '#3d2e11';
      borderColor = 'var(--gold-500)';
      icon = '⚠️';
    }

    el.style.cssText = 'background:' + bgColor + ';color:var(--olive-100);padding:0.75rem 1rem;border-radius:var(--radius-md);font-size:0.85rem;font-weight:500;box-shadow:var(--shadow-lg);border-left:4px solid ' + borderColor + ';display:flex;align-items:center;gap:0.5rem;pointer-events:auto;animation:toastIn 0.3s ease;cursor:pointer';
    el.innerHTML = '<span>' + icon + '</span><span style="flex:1">' + message + '</span>';

    el.addEventListener('click', function () {
      dismiss(el);
    });

    c.appendChild(el);

    if (duration > 0) {
      setTimeout(function () {
        dismiss(el);
      }, duration);
    }
  }

  function dismiss(el) {
    if (!el || !el.parentNode) return;
    el.style.transition = 'opacity 0.3s, transform 0.3s';
    el.style.opacity = '0';
    el.style.transform = 'translateX(100%)';
    setTimeout(function () {
      if (el.parentNode) el.parentNode.removeChild(el);
    }, 300);
  }

  // Стили для анимации (добавляем один раз)
  var styleEl = document.createElement('style');
  styleEl.textContent = '@keyframes toastIn{from{transform:translateX(100%);opacity:0}to{transform:translateX(0);opacity:1}}';
  document.head.appendChild(styleEl);

  window.toast = { show: show, success: function (m, d) { show(m, 'success', d); }, error: function (m, d) { show(m, 'error', d); }, warning: function (m, d) { show(m, 'warning', d); } };
})();
