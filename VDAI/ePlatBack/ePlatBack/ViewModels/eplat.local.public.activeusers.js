var Users = new Vue({
	el: '#app',
	data: {
		Users: [],
		UsersDataTable: {
			fields: [
				{ key: 'System', sortable: true },
				{ key: 'FirstName', sortable: true },
				{ key: 'LastName', sortable: true },
				{ key: 'Username', sortable: true },
				{ key: 'LastActivityDate', label: 'Last Activity', sortable: true },
				{ key: 'DaysToLock', label: 'Allowed Inactive Days', sortable: true },
				{ key: 'Terminals', sortable: true }
			],
			sortBy: 'FirstName',
			sortDesc: false,
			perPage: 20,
			currentPage: 1,
			striped: true,
			bordered: true,
			totalRows: Users.length,
			hover: false
		},
		filter: ''
	},
	methods: {
		onFiltered: function (filteredItems) {
			this.UsersDataTable.totalRows = filteredItems.length;
			this.UsersDataTable.currentPage = 1;
		}
	},
	mounted: function () {
		this.Users = window.Users;
	}
})