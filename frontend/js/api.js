(function () {
  var API_BASE = '';

  function getToken() {
    return localStorage.getItem('tank-token');
  }

  function getUserId() {
    return localStorage.getItem('tank-user-id');
  }

  function isLoggedIn() {
    return !!getToken();
  }

  function logout() {
    localStorage.removeItem('tank-token');
    localStorage.removeItem('tank-user-id');
    localStorage.removeItem('tank-username');
    localStorage.removeItem('tank-profile');
    localStorage.removeItem('tank-avatar');
    localStorage.removeItem('tank-role');
    window.location.href = 'index.html';
  }

  async function request(method, path, body) {
    var opts = {
      method: method,
      headers: { 'Content-Type': 'application/json' },
    };
    var token = getToken();
    if (token) opts.headers['Authorization'] = 'Bearer ' + token;
    if (body !== undefined && body !== null) opts.body = JSON.stringify(body);
    try {
      var res = await fetch(API_BASE + path, opts);
      if (res.status === 204) return null;
      if (!res.ok) {
        var errData = null;
        try { errData = await res.json(); } catch (_) {}
        var msg = (errData && errData.detail) || (errData && errData.title) || 'Ошибка сервера';
        if (res.status === 401 || res.status === 403) {
          if (getToken()) {
            toast.error('Сессия истекла. Войдите снова.');
            logout();
          }
          throw new Error(msg);
        }
        toast.error(msg);
        throw new Error(msg);
      }
      if (res.status === 204) return null;
      var ct = res.headers.get('content-type') || '';
      if (ct.indexOf('application/json') !== -1) return await res.json();
      return await res.text();
    } catch (e) {
      if (e.name !== 'Error' || !e.message.match(/Ошибка сервера|Сессия|Conflict/)) {
        var safeMsg = e.message || 'Сетевая ошибка';
        if (safeMsg !== 'Failed to fetch') toast.error(safeMsg);
      }
      throw e;
    }
  }

  async function uploadFile(method, path, formData) {
    var token = getToken();
    var opts = {
      method: method,
      headers: {},
      body: formData,
    };
    if (token) opts.headers['Authorization'] = 'Bearer ' + token;
    try {
      var res = await fetch(API_BASE + path, opts);
      if (!res.ok) {
        var errData = null;
        try { errData = await res.json(); } catch (_) {}
        var msg = (errData && errData.detail) || (errData && errData.title) || 'Ошибка сервера';
        toast.error(msg);
        throw new Error(msg);
      }
      return await res.json();
    } catch (e) {
      if (e.name !== 'Error' || !e.message.match(/Ошибка сервера/)) {
        toast.error(e.message || 'Сетевая ошибка');
      }
      throw e;
    }
  }

  window.api = {
    getToken: getToken,
    getUserId: getUserId,
    isLoggedIn: isLoggedIn,
    logout: logout,
    get: function (path) { return request('GET', path); },
    post: function (path, body) { return request('POST', path, body); },
    put: function (path, body) { return request('PUT', path, body); },
    del: function (path) { return request('DELETE', path); },
    upload: function (method, path, formData) { return uploadFile(method, path, formData); },
  };
})();
