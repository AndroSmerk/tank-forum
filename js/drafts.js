/* ================================================================
   АВТОСОХРАНЕНИЕ ЧЕРНОВИКОВ — localStorage
   ================================================================ */

(function () {
  const SAVE_DELAY = 800;

  function getKey(name) {
    return 'tank-draft-' + name;
  }

  function saveDraft(name, data) {
    try {
      localStorage.setItem(getKey(name), JSON.stringify(data));
    } catch (e) {
      // localStorage переполнен — игнорируем
    }
  }

  function loadDraft(name) {
    try {
      var raw = localStorage.getItem(getKey(name));
      return raw ? JSON.parse(raw) : null;
    } catch (e) {
      return null;
    }
  }

  function clearDraft(name) {
    localStorage.removeItem(getKey(name));
  }

  // Найти все формы с data-draft
  function initDrafts() {
    var forms = document.querySelectorAll('[data-draft]');
    if (!forms.length) return;

    forms.forEach(function (form) {
      var draftName = form.getAttribute('data-draft');
      var fields = form.querySelectorAll('[data-draft-field]');
      if (!fields.length) return;

      var indicator = form.querySelector('[data-draft-indicator]');
      var lastData = null;

      // Восстановить черновик
      var saved = loadDraft(draftName);
      if (saved) {
        fields.forEach(function (field) {
          var name = field.getAttribute('data-draft-field');
          if (saved[name] !== undefined) {
            if (field.tagName === 'TEXTAREA' || field.tagName === 'INPUT') {
              field.value = saved[name];
            } else if (field.tagName === 'SELECT') {
              field.value = saved[name];
            }
          }
        });
        if (indicator) {
          indicator.textContent = '💾 Восстановлен черновик от ' + formatDate(saved._saved);
          indicator.style.color = 'var(--gold-500)';
        }
      }

      // Функция сохранения
      function collectAndSave() {
        var data = { _saved: new Date().toISOString() };
        fields.forEach(function (field) {
          var name = field.getAttribute('data-draft-field');
          data[name] = field.value || '';
        });

        // Проверка: есть ли хоть что-то заполненное
        var hasContent = fields.some(function (f) {
          return (f.value || '').trim().length > 0;
        });

        if (hasContent) {
          saveDraft(draftName, data);
          if (indicator) {
            indicator.textContent = '💾 Черновик сохранён';
            indicator.style.color = '';
          }
        } else {
          clearDraft(draftName);
          if (indicator) {
            indicator.textContent = '';
          }
        }

        lastData = data;
      }

      // Дебаунс на input/change
      var debouncedSave = debounce(collectAndSave, SAVE_DELAY);

      fields.forEach(function (field) {
        field.addEventListener('input', debouncedSave);
        field.addEventListener('change', debouncedSave);
      });

      // Очистка при успешной отправке
      form.addEventListener('submit', function () {
        clearDraft(draftName);
        if (indicator) {
          indicator.textContent = '✅ Опубликовано!';
          indicator.style.color = 'var(--online-color)';
        }
      });
    });
  }

  document.addEventListener('DOMContentLoaded', initDrafts);

  window.draftsInit = initDrafts;
})();
