// Toggle sidebar open/close
(function(){
  const toggle = document.getElementById('sidebarToggle');
  const closeBtn = document.getElementById('sidebarClose');

  function openSidebar(){ document.body.classList.add('sidebar-open'); }
  function closeSidebar(){ document.body.classList.remove('sidebar-open'); }

  if (toggle) toggle.addEventListener('click', openSidebar);
  if (closeBtn) closeBtn.addEventListener('click', closeSidebar);

  // Close on navigation click (mobile)
  document.addEventListener('click', function(e){
    const target = e.target;
    if (target.matches('.sidebar-nav a') && window.innerWidth < 980){
      closeSidebar();
    }
  });
})();