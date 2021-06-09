$(function () {
	REASSIGNMENT.init();
});

var REASSIGNMENT = function () {
	var init = function () {

	}

	var searchResults = function (data) {
		app.ReservationsToReceive = data;
		app.Reservations = data;
	}

	return {
		init: init,
		searchResults: searchResults
	}
}();

var app = new Vue({
	mixins: [ePlatUtils],
	el: '#app',
	data: function () {
		return {
			Shared: ePlatStore,
			ReservationsToReceive: [],
			Search: null
		}
	},
	watch: {
		ReservationsToReceive(nVal) {
			let self = this;
			if (self.Search == false && _.filter(nVal, function (o) { return o.Status == true }).length > 0) {
				$.alert({
					title: 'Reassignation Succeded',
					content: '',
					animation: 'zoom',
					closeAnimation: 'scale',
					autoClose: 'ok|5000',
					type: 'green'
				});
			}
		}
	},
	methods: {
		clearTable() {
			let self = this;
			self.ReservationsToReceive = [];
			self.Reservations = [];
		},
		isJSON(str) {
			try {
				JSON.parse(str);
			} catch (e) { return false; }
			return true;
		},
		formatDate(date) {
			return moment(date).format("YYYY-MM-DD");
		},
		search() {
			let self = this;
			self.clearTable();
			self.Search = true;
			Vue.nextTick(function () {
				$('#frmReservationReassignment').submit();
			})
		},
		save() {
			let self = this;
			self.Search = false;

			Vue.nextTick(function () {
				$('#frmReservationReassignment').submit();
			})
		}
	},
	computed: {
		Reservations: {
			get() {
				return JSON.stringify(this.ReservationsToReceive)
			},
			set(nVal) {

			}
		}
	},
	mounted: function () {
		this.Session().getSessionDetails();
		this.UI().loadDependentFields('/crm/PreArrival/GetDependantResorts', true);

		$('#ArrivalDate').datetimepicker({
			format: 'YYYY-MM-DD'
		});
		$('#ArrivalDate').on('datetimepicker.hide', function () {
			this.dispatchEvent(new Event('input'));
		});

		$('#ArrivalDate').on('blur', function () {
			$(this).datetimepicker('hide');
			this.dispatchEvent(new Event('input'));
		});

		$('#navbarNav,#secondaryMenu').remove();
	}
})