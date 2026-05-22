(function () {
  var STORAGE_KEY = 'tank-crew';

  var CREW_MEMBERS = [
    { name: 'Tankist_1979', rank: '★★ Лейтенант', rankClass: 'rank-lieutenant', online: true, added: '2 мес' },
    { name: 'Serg_76', rank: '★ Сержант', rankClass: 'rank-sergeant', online: true, added: '1 мес' },
    { name: 'Vova_Na_KV2', rank: 'Ефрейтор', rankClass: 'rank-corporal', online: false, added: '3 нед' },
  ];

  function getCrew() {
    try {
      var raw = localStorage.getItem(STORAGE_KEY);
      return raw ? JSON.parse(raw) : CREW_MEMBERS;
    } catch (e) { return CREW_MEMBERS; }
  }

  function saveCrew(crew) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(crew));
  }

  function addMember(name) {
    var crew = getCrew();
    if (crew.some(function (m) { return m.name === name; })) return false;
    crew.push({ name: name, rank: '', rankClass: '', online: true, added: 'только что' });
    saveCrew(crew);
    return true;
  }

  function removeMember(name) {
    var crew = getCrew();
    crew = crew.filter(function (m) { return m.name !== name; });
    saveCrew(crew);
  }

  function renderCrewList() {
    var crew = getCrew();
    var list = document.getElementById('crewList');
    if (!list) return;
    list.innerHTML = crew.map(function (m) {
      var rankHtml = m.rank ? '<span class="rank-badge" style="font-size:0.55rem;color:var(--'+m.rankClass+');border-color:var(--'+m.rankClass+')">'+m.rank+'</span>' : '';
      return '<div class="online-item">'
        + '<span class="status-dot-sm '+(m.online?'online':'offline')+'"></span>'
        + '<a href="profile.html?user='+encodeURIComponent(m.name)+'" class="name" style="color:var(--text-primary);text-decoration:none">'+m.name+'</a>'
        + rankHtml
        + '<span class="text-xs text-tertiary" style="font-size:0.65rem">'+m.added+'</span>'
        + '<button class="pa-btn" onclick="crewRemove(\''+m.name+'\')" style="color:var(--fire-400);margin-left:auto" title="Убрать из экипажа">✕</button>'
        + '</div>';
    }).join('');
  }

  function renderChipList() {
    var chip = document.getElementById('crewChips');
    if (!chip) return;
    var crew = getCrew();
    var online = crew.filter(function(m){return m.online;}).length;
    chip.innerHTML = '<div style="display:flex;align-items:center;gap:8px;font-size:0.72rem;color:var(--text-tertiary);flex-wrap:wrap">'
      + crew.map(function(m){
        return '<span style="display:inline-flex;align-items:center;gap:4px;background:var(--bg-card-alt);padding:2px 8px;border-radius:10px;border:1px solid var(--border-color)" title="'+m.name+'">'
          + '<span class="status-dot-sm '+(m.online?'online':'offline')+'" style="width:6px;height:6px"></span>'
          + '<span>'+m.name+'</span>'
          + '</span>';
      }).join('')
      + '</div>';
    var countEl = document.getElementById('crewOnlineCount');
    if (countEl) countEl.textContent = '· '+online+' онлайн';
  }

  window.crew = {
    get: getCrew,
    add: addMember,
    remove: removeMember,
    renderList: renderCrewList,
    renderChips: renderChipList,
  };

  window.crewRemove = function(name) {
    crew.remove(name);
    crew.renderList();
    toast.info(name+' удалён из экипажа');
  };

  document.addEventListener('DOMContentLoaded', function () {
    // Подхватываем динамические обновления (после renderTopic и т.д.)
    var observer = new MutationObserver(function () {
      if (document.getElementById('crewChips')) crew.renderChips();
      if (document.getElementById('crewList')) crew.renderList();
    });
    if (document.body) observer.observe(document.body, { childList: true, subtree: true });

    // Первичный рендер
    setTimeout(function () {
      crew.renderChips();
      crew.renderList();
    }, 100);
  });
})();
