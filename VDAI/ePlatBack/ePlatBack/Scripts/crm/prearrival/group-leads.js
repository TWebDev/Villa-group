var GroupLead = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data: function () {
        return {
            Shared: ePlatStore,
            LeadGroups: [],
            leads: null,
            GroupName: null,
            GroupID: null
        }
    },
    methods: {
        saveLeadGroup: function () {
            let self = this;
            $.confirm({
                title: (self.GroupID == null ? 'Save' : 'Update')  + ' Group?',
                content: '',
                buttons: {
                    confirm: {
                        btnClass: 'btn-primary',
                        action: function () {
                            function GenericListItem() {
                                Property1 = null;
                                Property2 = null;
                                Property3 = null;
                                Property4 = null;
                                Property5 = null;
                            }
                            var model = new GenericListItem();
                            model.Property1 = self.GroupName;
                            model.Property2 = self.GroupID;
                            model.Property3 = self.leads;
                            model.Property4 = null;
                            model.Property5 = null;
                            $.ajax({
                                url: '/PreArrival/SaveLeadGroup',
                                cache: false,
                                type: 'POST',
                                data: JSON.stringify(model),
                                dataType: 'json',
                                traditional: true,
                                contentType: 'application/json; charset=utf-8',
                                success: function (data) {
                                    $.alert({
                                        title: 'Process Finished',
                                        content: data.ResponseMessage + '<br />' + data.ExceptionMessage,
                                        animation: 'zoom',
                                        closeAnimation: 'scale',
                                        autoClose: 'ok|5000',
                                        type: data.ResponseType == 1 ? 'green' : 'red'
                                    });
                                    self.closeModal();
                                }
                            });
                        }
                    },
                    cancel: function () {

                    }
                }
            });
        },
        closeModal: function () {
            let self = this;
            self.GroupID = null;
            self.GroupName = null;
            parent.$.fancybox.close();
        }
        //acomodar la obtencion de grupos en funcion para llamar en la carga y en la actualizacion
    },
    computed: {
        GroupType: function () {
            return this.GroupID == null ? 'New' : 'Existing';
        }
    },
    filters: {

    },
    watch: {
        
    },
    mounted: function () {
        let self = this;

        $.ajax({
            url: '/PreArrival/GetRecentLeadGroups',
            cache: false,
            type: 'POST',
            data: {},
            success: function (data) {
                $.each(data, function (index, item) {
                    self.LeadGroups[index] = { "value": item.Value, "label": item.Text };
                });

                Vue.nextTick(function () {
                    $('#GroupName').bind('keydown', function (e) {
                        if (e.keyCode === $.ui.keyCode.TAB && $(this).data('ui.autocomplete').menu.active) {
                            e.preventDefault();
                        }
                    }).autocomplete({
                        source: self.LeadGroups,
                        minLength: 0,
                        position: { my: 'left top', at: 'left bottom' },
                        autoFocus: true,
                        select: function (e, ui) {
                            e.preventDefault();
                            $('#GroupID').val(ui.item.value);
                            self.GroupID = ui.item.value;
                            self.GroupName = ui.item.label;
                            $(this).val(ui.item.label);
                        },
                        close: function () {
                            var existing = _.find(self.LeadGroups, function (o) { return o.label == self.GroupName });
                            if (existing == undefined) {
                                self.GroupID = null;
                            }
                        },
                    }).on('focus', function () {
                        $(this).autocomplete('search', '');
                    });
                })

                self.leads = window.location.search.split('=')[1];
            }
        });
    }
})