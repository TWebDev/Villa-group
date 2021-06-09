$(document).ready(function(){
    $('ul.nav-pills li a:first').addClass('active');
    $('.secciones article').hide();
    $('.secciones article:first').show();

    $('ul.nav-pills li a').click(function(){
        $('ul.nav-pills li a').removeClass('active');
        $(this).addClass('active');
        $('.secciones article').hide();

        var activeTab = $(this).attr('href');
        $(activeTab).show();
        return false;
    });

});
$(document).on('click', 'ul li', function(){
  $(this).addClass('active').siblings().removeClass('active')
})
/*EFECTO ZOOM IMAGENES*/
$(document).ready(function(){
    $('.zoom').hover(function() {
        $(this).addClass('transition');
    }, function() {
        $(this).removeClass('transition');
    });
});
/*COLLAPSE FAQS*/
var coll = document.getElementsByClassName("collapsible");
var i;

for (i = 0; i < coll.length; i++) {
  coll[i].addEventListener("click", function() {
    this.classList.toggle("active");
    var content = this.nextElementSibling;
    if (content.style.maxHeight){
      content.style.maxHeight = null;
    } else {
      content.style.maxHeight = content.scrollHeight + "px";
    } 
  });
}
/*Ocultar imagen al hacer click en collapsed*/
$(document).ready(function(){
  
})
/*EFECTO BOTONES*/
$(document).on('click', 'ul li', function(){
  $(this).addClass('active').siblings().removeClass('active')
})

/*OCULTAR IMAGEN AL BAJAR EL MENU*/
function ocultarimagen(){
  document.getElementById('imagen').style.display = 'none';
}