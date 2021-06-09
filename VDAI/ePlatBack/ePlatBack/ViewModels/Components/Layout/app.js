
import Vue from 'vue';
import Vuex from 'vuex';
Vue.use(Vuex);

//import app from '../Index/index.vue';
import { store } from './stateManagement.js'
import sidebar from './index.vue';
//Vue.component('index', require('./index.vue').default);

var index = new Vue({
    store,
    el: '#content',
   /* data:{
        index:'index'
    },*/
    components: {
        'sidebar': sidebar
    },
    beforeCreate() {// get User information before create elements in html and turn on hub
        let self = this;
        this.$store.dispatch('getUserInfo');
        this.$store.dispatch('getUserNotifications');
        this.$store.dispatch('getUserNotificationsSettings');
        $.connection.hub.start().done(function () {
            console.log('HUB ON');           
            self.$store.dispatch('hubConnected', true);
        });
    }
})