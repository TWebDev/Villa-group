
Vue.component('search-workflows', require('./searchWorkflows.vue').default);
Vue.component('workflows-management', require('./workflowsManagement.vue').default);
Vue.component('new-step', require('./newStep.vue').default);


export const bus = new Vue();

var workflow = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data: {
        Shared: ePlatStore
        //search: 'searchworkflows',
        //newstep: 'newstep'
        //currentworkflows: 'searchWorkflows'
    }
})