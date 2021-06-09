var wf = new Vue({
    el: '#app',
    data: {
        Shared: ePlatStore,
        EmailNotifications: {},
        Workflows: [],
        workflowModel: {
            workflowID: null,
            workflow: []
        },
        widthClass:''
    },
    methods: {
        getEmailNotifications: function () {
            let self = this;
            $.ajax({
                url: '/Workflows/GetEmails',
                cache: false,
                type: 'post',
                data: {},
                success: function (data) {
                    self.EmailNotifications = data;
                }
            });
        },
        loadWorkflow: function () {
            let self = this;

        },
        clearStep: function (key) {
            let self = this;
            self.workflowModel.workflow.splice(key, 1);
        },
        addStep: function (evt) {
            console.log($(evt.target).attr('id'));
        }
    },
    computed: {
        
    },
    mounted: function () {
        let self = this;
        self.getEmailNotifications();

        //$('.btn-action').on('click', function () {
        //    var keyValue = {
        //        type: 'action',
        //        subtype: $(this).attr('data-action'),
        //        key: '',
        //        value: '',
        //        delay: { days: '', hours: '' },
        //        yes: '',
        //        no: ''
        //    }
        //    keyValue.key = $(this).text();
        //    self.workflowModel.workflow.push(keyValue);
        //});

        //$('.btn-condition').on('click', function () {
        //    var keyValue = {
        //        type: 'condition',
        //        subtype: $(this).attr('data-action'),
        //        key: '',
        //        value: '',
        //        delay: { days: '', hours: '' },
        //        yes: '',
        //        no: ''
        //    }
        //    keyValue.key = $(this).text();
        //    self.workflowModel.workflow.push(keyValue);
        //});


    }
});