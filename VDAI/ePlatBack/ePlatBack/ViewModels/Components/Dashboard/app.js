/*Layout 3*/
import Vue from 'vue';
import Vuex from 'vuex';
Vue.use(Vuex);
import { store } from '../Layout/stateManagement.js';
Vue.component('sidebar', require('../Layout/index.vue').default);
Vue.component('terminals', require('../Layout/terminals.vue').default);
/*Layout 3*/

var VueScrollTo = require('vue-scrollto');
Vue.use(VueScrollTo);

Vue.component('dashboard', require('./dashboard.vue').default);

export const bus = new Vue();

var Dashboard = new Vue({
    store: store,
    mixins: [ePlatUtils],
    el: '#app',
    data: {
        dashboardCanvas: 'dashboard'
    },
    mounted: function () {
        
    }
});