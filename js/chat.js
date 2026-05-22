(function () {
  var STORAGE_KEY = 'tank-chat-collapsed';
  var MSG_KEY = 'tank-chat-messages';
  var MAX_MSG = 50;

  function loadMessages() {
    var raw = localStorage.getItem(MSG_KEY);
    return raw ? JSON.parse(raw) : [];
  }

  function saveMessages(msgs) {
    if (msgs.length > MAX_MSG) msgs = msgs.slice(msgs.length - MAX_MSG);
    localStorage.setItem(MSG_KEY, JSON.stringify(msgs));
  }

  function renderMessages(messages, container) {
    if (!container) return;
    container.innerHTML = '';
    messages.forEach(function (m) {
      var div = document.createElement('div');
      div.className = 'chat-msg';
      div.innerHTML = '<span class="chat-author" style="color:var(--rank-sergeant)">' + escapeHtml(m.author) + ':</span>'
        + '<span class="chat-text">' + escapeHtml(m.text) + '</span>'
        + '<span class="chat-time">' + escapeHtml(m.time) + '</span>';
      container.appendChild(div);
    });
    container.scrollTop = container.scrollHeight;
  }

  function initChat() {
    var widget = document.getElementById('chatWidget');
    if (!widget) return;

    var header = widget.querySelector('.chat-header');
    var body = widget.querySelector('.chat-body');
    var input = widget.querySelector('.chat-input');
    var sendBtn = widget.querySelector('.chat-send-btn');
    var messages = widget.querySelector('.chat-messages');

    var wasCollapsed = localStorage.getItem(STORAGE_KEY) === 'true';
    if (wasCollapsed) {
      widget.classList.add('collapsed');
      if (body) body.style.display = 'none';
    }

    var savedMessages = loadMessages();
    renderMessages(savedMessages, messages);

    header.addEventListener('click', function () {
      var isCollapsed = widget.classList.toggle('collapsed');
      if (body) body.style.display = isCollapsed ? 'none' : '';
      localStorage.setItem(STORAGE_KEY, isCollapsed);
    });

    function sendMessage() {
      if (!input || !messages) return;
      var text = input.value.trim();
      if (!text) return;

      var time = new Date().toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
      savedMessages.push({ author: 'Вы', text: text, time: time });
      saveMessages(savedMessages);
      renderMessages(savedMessages, messages);
      input.value = '';
    }

    if (sendBtn) {
      sendBtn.addEventListener('click', sendMessage);
    }
    if (input) {
      input.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') sendMessage();
      });
    }

    document.querySelectorAll('.chat-sticker').forEach(function (el) {
      el.addEventListener('click', function () {
        if (input) {
          input.value += el.textContent + ' ';
          input.focus();
        }
      });
    });
  }

  document.addEventListener('DOMContentLoaded', initChat);
})();
