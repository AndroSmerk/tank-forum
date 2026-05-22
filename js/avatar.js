(function () {
  var STORAGE_KEY = 'tank-avatar';

  function defaultAvatar() {
    return 'data:image/svg+xml,%3Csvg xmlns=\'http://www.w3.org/2000/svg\' viewBox=\'0 0 100 100\'%3E%3Crect fill=\'%235a5f44\' width=\'100\' height=\'100\'/%3E%3Ccircle fill=\'%23d4d9b0\' cx=\'50\' cy=\'35\' r=\'22\'/%3E%3Ccircle fill=\'%23f2f4e6\' cx=\'50\' cy=\'35\' r=\'18\'/%3E%3Ctext x=\'50\' y=\'42\' text-anchor=\'middle\' font-size=\'20\' fill=\'%233d4030\' font-weight=\'bold\'%3EKOT%3C/text%3E%3C/svg%3E';
  }

  function getAvatarUrl() {
    var saved = localStorage.getItem(STORAGE_KEY);
    return saved || defaultAvatar();
  }

  function uploadAvatar(file, callback) {
    if (!file) return;
    var reader = new FileReader();
    reader.onload = function (e) {
      localStorage.setItem(STORAGE_KEY, e.target.result);
      applyAvatar();
      if (callback) callback(e.target.result);
    };
    reader.readAsDataURL(file);
  }

  function applyAvatar() {
    var url = getAvatarUrl();
    document.querySelectorAll('[data-avatar="me"]').forEach(function (img) {
      img.src = url;
    });
  }

  function setupUploadButton() {
    var btn = document.getElementById('avatarUploadBtn');
    if (!btn) return;

    var input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.style.display = 'none';
    input.id = 'avatarFileInput';
    document.body.appendChild(input);

    btn.addEventListener('click', function () {
      input.click();
    });

    input.addEventListener('change', function () {
      if (input.files && input.files[0]) {
        uploadAvatar(input.files[0], function () {
          toast.success('Аватар обновлён!');
        });
      }
    });
  }

  function watchNewAvatars() {
    var observer = new MutationObserver(function () {
      applyAvatar();
    });
    observer.observe(document.body, { childList: true, subtree: true });
  }

  document.addEventListener('DOMContentLoaded', function () {
    applyAvatar();
    setupUploadButton();
    watchNewAvatars();
  });
})();
