document.getElementById('year').textContent = new Date().getFullYear();

const toggle = document.getElementById('nav-toggle');
const links  = document.getElementById('nav-links');

toggle.addEventListener('click', function () {
  var isOpen = links.classList.toggle('open');
  toggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
});

links.querySelectorAll('a').forEach(function (a) {
  a.addEventListener('click', function () {
    links.classList.remove('open');
    toggle.setAttribute('aria-expanded', 'false');
  });
});
