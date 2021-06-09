var animation = bodymovin.loadAnimation({
    container: document.getElementById('anim'),
    rederer: 'svg',
    loop: true,
    autoplay: true,
    path: 'data.json'
});
// $("#cerrar").click(function(){
//   $('#ventana-modal').modal("hide");
// });
function cerrar(){
  document.getElementById('ventana-modal').style.display='none';
}
