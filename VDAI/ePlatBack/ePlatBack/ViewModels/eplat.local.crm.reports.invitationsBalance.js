var balance = new Vue({
    mixins: [ePlatUtils],
    el: "#app",
    data:
        {
            Shared: ePlatStore,
            Invitations:[],
            DataTable: {
            },
            showExportable:false
        },
    computed:
        {
        },
    methods:
        {
            tableResult: function(data)
            {
                let self = this;
                if (data != "" || data != null) {
                    this.UI().exportToExcel('btnExportable', 'tblInvitationsBalance');
                       self.showExportable = true;
                }
                else {
                    $('#tblInvitationsBalance tbody').html("<tr> Empty </tr>");
                }
            },
            exporToExcel:function()
            {
               /* let self = this;
                var buttonID = 'btnExportable';
                var tableID = "tblInvitationsBalance";
                self.UI().exportToExcel(tableID);*/
            }
        },
    mounted: function()
        {
            this.Session().getSessionDetails();
            $('.datetimepicker-input').datetimepicker({
                format: 'YYYY-MM-DD'
            });
            $('.datepicker-input').on('datetimepicker.hide', function (e) {
                this.dispatchEvent(new Event('input'));
            });
            this.UI().showSearchCard();
            this.UI().xhrTargets();
        }
});
