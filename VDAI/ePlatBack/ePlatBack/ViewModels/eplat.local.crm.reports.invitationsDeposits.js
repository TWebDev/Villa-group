var invitationsDeposits = new Vue({
	mixins: [ePlatUtils],
	el: '#app',
	data: {
		Shared: ePlatStore,
        invitations: [],
        showExportable: false
    },
    watch: {
        showExportable: function () {
            this.UI().exportToExcel('btnExportable', 'tbldepositsTable');
        }
    },
	methods:{
		setSearchResult: function (data) {
            let self = this;
            if (data != "" || data != null) {
                this.invitations = data.Result;
                this.showExportable= true;
            } else {
                $('#tbldepositsTable tbody').html("<tr> Empty </tr>");
            }
		}
	},
	mounted: function () {
		let self = this;
		this.Session().getSessionDetails();
        this.UI().showSearchCard();
		$('.datetimepicker-input').datetimepicker({
			format: 'YYYY-MM-DD'
		});
		$('.datepicker-input').on('datetimepicker.hide', function (e) {
			this.dispatchEvent(new Event('input'));
		});
	}
})