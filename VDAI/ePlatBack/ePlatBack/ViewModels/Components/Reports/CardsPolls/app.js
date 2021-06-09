
/*
 * Here will we create a fresh Vue application instance and attach it to
 * the page. Then, you may begin adding components to this application
 * or customize the JavaScript scaffolding to fit your unique needs.
*/
//Irte con un clic a algun elemento de la pagina
var VueScrollTo = require('vue-scrollto');
import datePicker from 'vue-bootstrap-datetimepicker';
import 'pc-bootstrap4-datetimepicker/build/css/bootstrap-datetimepicker.css';
Vue.use(datePicker);
Vue.use(VueScrollTo)


Vue.component('cardspolls', require('./cardspolls.vue').default);


export const bus = new Vue();

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
        cardspolls: 'cardspolls',
     
    },
    mounted: function () {
        
        /*****METODOS DE UNICIO DE SESION *****/
        let self = this;
        this.Session().getSessionDetails();
        this.UIData.showSearchCard = true;
    }
})
