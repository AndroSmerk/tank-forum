(function () {
  var STORAGE_KEY = 'tank-chat-collapsed';
  var POLL_INTERVAL = 5000;
  var pollTimer = null;

  function escapeHtml(str) {
    if (!str) return '';
    var div = document.createElement('div');
    div.textContent = str;
    return div.innerHTML;
  }

  function formatChatTime(dateStr) {
    if (!dateStr) return '';
    var d = new Date(dateStr);
    return d.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
  }

  function getRankColor(rankClass) {
    var map = {
      'rank-general': 'var(--rank-general)',
      'rank-colonel': 'var(--rank-colonel)',
      'rank-major': 'var(--rank-major)',
      'rank-lieutenant': 'var(--rank-lieutenant)',
      'rank-sergeant': 'var(--rank-sergeant)',
      'rank-corporal': 'var(--rank-corporal)',
      'rank-private': 'var(--rank-private)'
    };
    return map[rankClass] || 'var(--text-primary)';
  }

  function renderMessages(messages, container) {
    if (!container) return;
    if (!messages || messages.length === 0) {
      container.innerHTML = '<div class="text-sm text-tertiary" style="padding:0.5rem;text-align:center">Пока нет сообщений</div>';
      return;
    }
    container.innerHTML = '';
    messages.forEach(function (m) {
      var div = document.createElement('div');
      div.className = 'chat-msg';
      var color = getRankColor(m.rankClass);
      var avatarHtml = m.avatar
        ? '<img src="' + m.avatar + '" style="width:18px;height:18px;border-radius:50%;vertical-align:middle;margin-right:4px">'
        : '';
      div.innerHTML = '<span class="chat-author" style="color:' + color + '">'
        + avatarHtml + escapeHtml(m.username) + ':</span>'
        + '<span class="chat-text">' + escapeHtml(m.content) + '</span>'
        + '<span class="chat-time">' + formatChatTime(m.createdAt) + '</span>';
      container.appendChild(div);
    });
    container.scrollTop = container.scrollHeight;
  }

  async function loadChatMessages() {
    var container = document.getElementById('chatMessages');
    if (!container) return;
    try {
      var messages = await api.get('/api/chat?limit=50');
      renderMessages(messages, container);
      var countEl = document.getElementById('chatUserCount');
      if (countEl && messages && messages.length > 0) {
        var unique = {};
        messages.forEach(function (m) { unique[m.userId] = true; });
        var count = Object.keys(unique).length;
        countEl.textContent = '· ' + count + ' бойцов';
      } else if (countEl) {
        countEl.textContent = '';
      }
    } catch (_) {}
  }

  async function sendChatMessage(text) {
    try {
      await api.post('/api/chat', { content: text });
      await loadChatMessages();
    } catch (_) {}
  }

  function startPolling() {
    if (pollTimer) clearInterval(pollTimer);
    pollTimer = setInterval(loadChatMessages, POLL_INTERVAL);
  }

  function initChat() {
    var widget = document.getElementById('chatWidget');
    if (!widget) return;

    // Очистка старых данных от предыдущей версии чата
    localStorage.removeItem('tank-chat-messages');

    var header = widget.querySelector('.chat-header');
    var body = widget.querySelector('.chat-body');
    var input = widget.querySelector('.chat-input');
    var sendBtn = widget.querySelector('.chat-send-btn');

    var wasCollapsed = localStorage.getItem(STORAGE_KEY) === 'true';
    if (wasCollapsed) {
      widget.classList.add('collapsed');
      if (body) body.style.display = 'none';
    }

    loadChatMessages();
    startPolling();

    header.addEventListener('click', function () {
      var isCollapsed = widget.classList.toggle('collapsed');
      if (body) body.style.display = isCollapsed ? 'none' : '';
      localStorage.setItem(STORAGE_KEY, isCollapsed);
    });

    function sendMessage() {
      if (!input) return;
      var text = input.value.trim();
      if (!text) return;
      sendBtn.disabled = true;
      sendChatMessage(text).then(function () {
        input.value = '';
        sendBtn.disabled = false;
      }).catch(function () {
        sendBtn.disabled = false;
      });
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
