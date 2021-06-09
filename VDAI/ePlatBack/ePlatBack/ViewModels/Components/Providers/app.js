
/*
 * Here will we create a fresh Vue application instance and attach it to
 * the page. Then, you may begin adding components to this application
 * or customize the JavaScript scaffolding to fit your unique needs.
*/
//Irte con un clic a algun elemento de la pagina
var VueScrollTo = require('vue-scrollto');

Vue.use(VueScrollTo)

/* <COMPONENTES A UTILIZAR>*/
import * as VueGoogleMaps from "vue2-google-maps";

Vue.use(VueGoogleMaps, {
    load: {
        key: "AIzaSyAK4f6_68EBekX36U4YLNhvzrK531D9vUw",
        libraries: "places" // necessary for places input
        //AIzaSyAXoJHEOgj8zop9tUl-mOrpneZKlfhHpBM

        //AIzaSyDvCJMyY_0lxPntatflbaxi7kiQzBOXzRE - mia
    }
});

Vue.component('providers', require('./providers.vue').default);
Vue.component('addproviders', require('./addprovider.vue').default);

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
        currentspa: 'providers',
     
    },
    mounted: function () {
        
        /*****METODOS DE UNICIO DE SESION *****/
        let self = this;
        this.Session().getSessionDetails();
        this.UIData.showSearchCard = true;
    }
})
