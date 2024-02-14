const sidebar = document.querySelector('.sidebar');
const sidebarToggle = document.querySelector('.aside-toggle');
const body = document.querySelector('body');
sidebarToggle.addEventListener('click', function () {
  const visible = sidebar.getAttribute('data-visible') === 'true';
  if (visible) {
    sidebar.setAttribute('data-visible', false);
  } else {
    sidebar.setAttribute('data-visible', true);
  }
});

// Get all sidebar links
var sidebarLinks = document.querySelectorAll('.sidebar a');

// Loop through each link
sidebarLinks.forEach(function (link) {
  // Add click event listener
  link.addEventListener('click', function (event) {
    // Remove active class from all links
    sidebarLinks.forEach(function (link) {
      link.classList.remove('active');
    });

    // Add active class to the clicked link
    this.classList.add('active');
  });

  // Check if the link's href matches the current page URL
  if (link.href === window.location.href) {
    // Add active class to the link
    link.classList.add('active');
  }
});
