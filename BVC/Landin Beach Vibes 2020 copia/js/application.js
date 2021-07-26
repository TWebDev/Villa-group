$(document).ready(function() {
  // Closes sidebar menu
  $("#menu-close").click(function(e) {
      e.preventDefault();
      $("#sidebar-wrapper").toggleClass("active");
  });
  // Opens the sidebar menu
  $("#menu-toggle").click(function(e) {
      e.preventDefault();
      $("#sidebar-wrapper").toggleClass("active");
  });

  // Scrolls to the selected menu item on the page
  $(function() {
      $('a[href*=#]:not([href=#])').click(function() {
          if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') || location.hostname == this.hostname) {
              var target = $(this.hash);
              target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
              if (target.length) {
                  $('html,body').animate({
                      scrollTop: target.offset().top
                  }, 1000);
                  return false;
              }
          }
      });
  });

  // AJAX Form Submittal
  $(function() {
    var form = $('#contactForm');
    $(form).submit(function(e) {
      // Prevent typical page submittal behavior, and instead, use AJAX
      e.preventDefault();
      var formData = $(form).serialize();

      // Fill in, add, or change to suit your needs
      $.ajax({
        type: 'POST',
        url: $(form).attr('action'),
        data: formData
      })
    });
  });

  //Pop Up Modal
  $('[data-popup-open]').on('click', function(e)  {
      var targeted_popup_class = jQuery(this).attr('data-popup-open');
      $('[data-popup="' + targeted_popup_class + '"]').fadeIn(350);

      e.preventDefault();
  });

  $('[data-popup-close]').on('click', function(e)  {
      var targeted_popup_class = jQuery(this).attr('data-popup-close');
      $('[data-popup="' + targeted_popup_class + '"]').fadeOut(350);
      $("#form").toggleClass("active");
      e.preventDefault();
  });
}); // Document.ready close
