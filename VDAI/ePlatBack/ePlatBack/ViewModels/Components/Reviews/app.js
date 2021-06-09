
/*
 * Here will we create a fresh Vue application instance and attach it to
 * the page. Then, you may begin adding components to this application
 * or customize the JavaScript scaffolding to fit your unique needs.
*/
//Irte con un clic a algun elemento de la pagina

Vue.component('reviews', require('./reviews.vue').default);


/* </COMPONENTES A UTILIZAR>*/

//INSTANCIA VUE

var content = new Vue({
    /* <MIXIN DE INICIO DE SESION> */
    mixins: [ePlatUtils],
    el: '#app',

    data: {
        /* <VARIABLE DE INICIO DE SESION> */
        Shared: ePlatStore,
        /* </VARIABLE DE INICIO DE SESION> */
       
        currentreviews:'reviews',
        

    },
    mounted: function () {

        

        /*****METODOS DE UNICIO DE SESION *****/
        let self = this;
        this.Session().getSessionDetails();
        this.UIData.showSearchCard = true;
    }
})
