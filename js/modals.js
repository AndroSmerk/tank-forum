/* ================================================================
   МОДАЛКИ — открыть / закрыть
   ================================================================ */

(function () {
  function openModal(id) {
    var el = document.getElementById(id);
    if (!el) return;
    el.classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function closeModal(id) {
    var el = document.getElementById(id);
    if (!el) return;
    el.classList.remove('open');
    document.body.style.overflow = '';
  }

  // Закрытие по overlay и Escape
  document.addEventListener('click', function (e) {
    if (e.target.classList.contains('modal-overlay')) {
      closeModal(e.target.id);
    }
  });
  document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
      document.querySelectorAll('.modal-overlay.open').forEach(function (el) {
        closeModal(el.id);
      });
    }
  });

  window.modal = { open: openModal, close: closeModal };
})();
