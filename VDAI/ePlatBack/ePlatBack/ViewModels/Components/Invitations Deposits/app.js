
import Vue from 'vue';
import Vuex from 'vuex';
Vue.use(Vuex);

import { store } from '../Layout/stateManagement.js';
Vue.component('sidebar', require('../Layout/index.vue').default);
Vue.component('terminals', require('../Layout/terminals.vue').default);
var VueScrollTo = require('vue-scrollto');
Vue.use(VueScrollTo);
///
Vue.component('invitationReport', require('./deposits.vue').default);

var app = new Vue({
    store: store,
    el: '#app',
    data: {
        invitationReport: 'invitationReport'
    }
})