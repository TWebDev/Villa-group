var VloManifest = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data: {
        requestDate: moment().format('YYYY-MM-DD'),
        Shared: ePlatStore,
        Manifest: [],
        XHRManifest: null
    },
    methods: {
        getManifest: function () {
            let self = this;
            if (this.XHRManifest && this.XHRManifest.readyState != 4) {
                this.XHRManifest.abort();
            }
            this.XHRManifest = $.ajax({
                url: '/crm/SPI/GetManifestForVLO',
                cache: false,
                type: 'POST',
                data: { date: this.requestDate },
                success: function (data) {
                    self.Manifest = data;
                    Vue.nextTick(function () {
                        $('.collect-date, .activation-date').each(function () {
                            var picker = $(this);
                            var date = picker.val() != '' ? moment(picker.val(), 'YYYY-MM-DD').toDate() : '';
                            picker.datetimepicker({
                                format: 'YYYY-MM-DD',
                                date: date,
                                useCurrent: false
                            });
                            picker.on('blur', function () {
                                this.dispatchEvent(new Event('input'));
                                picker.datetimepicker('hide');
                            });
                        });

                        //$('.email').on('keyup', function () {
                        //    var index = $(this).parents('tr:first').index();
                        //    self.Manifest[index].EmailString = $(this).val();
                        //});

                        $('.vlo').on('change', function () {
                            var index = $(this).parents('tr:first').index();
                            self.Manifest[index].VLO = $(this).children('option:selected').text();
                        });

                        $('[data-toggle="tooltip"]').tooltip();
                    })
                }
            });
        },
        sendAllEmails: function () {
            $.each(this.Manifest.filter(function (e) { return (e.SendStatus != true && e.VLOUserID != null) }), function (index, item) {
                $.ajax({
                    url: '/Notifications/SendLetter',
                    type: 'POST',
                    data: { data: JSON.stringify(item) },
                    success: function (data) {
                        item.SendStatus = data.ResponseType == 1 ? true : false;
                        item.SendStatusString = data.ResponseType == 1 ? data.ResponseMessage : data.ExceptionMessage;
                    }
                });
            });
        },
        resendEmail: function (key) {
            let self = this;

            $.ajax({
                url: '/Notifications/SendLetter',
                type: 'POST',
                data: { data: JSON.stringify(self.Manifest[key]) },
                success: function (data) {
                    self.Manifest[key].SendStatus = data.ResponseType == 1 ? true : false;
                    self.Manifest[key].SendStatusString = data.ResponseType == 1 ? data.ResponseMessage : data.ExceptionMessage;
                    $.alert({
                        title: data.ResponseType == 1 ? 'Email Succesfully Sent' : data.ResponseMessage,
                        content: '',
                        animation: 'zoom',
                        closeAnimation: 'scale',
                        autoClose: 'ok|5000',
                        type: 'green'
                    });
                }
            });
        },
        showEmail: function (key, kkey) {
            if (self.Manifest[key].Email[kkey] != undefined) {
                if (self.Manifest[key].Email[kkey].indexOf('*') != -1) {
                    Vue.set(self.Manifest[key].Email, kkey, self.Manifest[key].EmailString[kkey]);
                }
                else {
                    Vue.set(self.Manifest[key].EmailString, kkey, self.Manifest[key].Email[kkey]);
                    var a = self.Manifest[key].Email[kkey];
                    var b = self.Manifest[key].Email[kkey].lastIndexOf('.');
                    var c = self.Manifest[key].Email[kkey].length;
                    var d = self.Manifest[key].Email[kkey].substr(0, 4);
                    Vue.set(self.Manifest[key].Email, kkey, (d + '********' + a.substr(b, (c - b))));
                }
            }
            //Vue.nextTick(function () {
            //    $('[data-toggle="tooltip"]').tooltip();
            //})
        },
        addEmail: function (key) {
            self.Manifest[key].Email.push('');
            self.Manifest[key].EmailString.push('');
        },
        cancelAddition: function (row, index) {
            self.Manifest[row].Email.splice(index, 1);
            self.Manifest[row].EmailString.splice(index, 1);
        }
    },
    mounted: function () {
        let self = this;

        this.Session().getSessionDetails();
        this.UIData.showSearchCard = true;

        $('.datepicker-input').datetimepicker({
            format: 'YYYY-MM-DD',
            maxDate: new Date()
        });

        $('.datepicker-input').on('blur', function () {
            $(this).datetimepicker('hide');
            this.dispatchEvent(new Event('input'));
            self.getManifest();
        });

        self.getManifest();
    }
})
Vue.component('emails', {
    props: ['email', 'index', 'list'],
    data: function () {
        return {
            Email: '',
            ListEmails: {},
            Index: ''
        }
    },
    template: '<input type="text" v-if="Email.indexOf(\'*\') == -1" class="form-control email d-block" v-model="ListEmails[Index]" />'
                            + '<span class="d-block" v-else>{{ListEmails[Index]}}</span>'
                        + '</div>',
    watch: {
        email: function (newVal, oldVal) {
            let self = this;
            self.Email = newVal;
        },
        list: function (newVal, oldVal) {
            let self = this;
            self.ListEmails = newVal;
        },
        index: function (newVal, oldVal) {
            let self = this;
            self.Index = newVal;
        }
    },
    mounted: function () {
        this.Email = this.email;
        this.ListEmails = this.list;
        this.Index = this.index;
    }
});
Vue.component('email-buttons', {
    props: ['email', 'rowindex', 'index'],
    data: function () {
        return {
            Email:'',
            ParentIndex: '',
            Index: ''
        }
    },
    template: '<span class="d-block">'
                                + '<i class="material-icons" v-if="Email.indexOf(\'*\') != -1" data-toggle="tooltip" title="edit">edit</i>'
                                + '<i class="material-icons" v-if="Email.indexOf(\'*\') == -1 && Email.length > 0" data-toggle="tooltip" title="accept">check</i>'
                                + '<i class="material-icons" v-if="Email.length == 0" v-on:click="cancel(rowindex, index)" data-toggle="tooltip" title="cancel">clear</i>'
                            + '</span>',
    watch: {
        email: function (newVal, oldVal) {
            let self = this;
            self.Email = newVal;
        },
        rowindex: function (newVal, oldVal) {
            let self = this;
            self.ParentIndex = newVal;
        },
        index: function (newVal, oldVal) {
            let self = this;
            self.Index = newVal;
        }
    },
    methods: {
        cancel: function (row, index) {
            let self = this;
            self.$emit('canceladd', row, index);
        }
    },
    mounted: function () {
        this.Email = this.email;
        this.ParentIndex = this.rowindex;
        this.Index = this.index;
    }
});
