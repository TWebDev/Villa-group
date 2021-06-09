var UserRequests = new Vue({
	el: '#app',
	mixins: [ePlatUtils],
	data: {
		Shared: ePlatStore,
		Requests: [],
		RequestsDataTable: {
			fields: [
                { key: 'Users', label: 'Users', sortable: true },
                { key: 'Attachements', label: 'Docs', sortable: true },
                { key: 'DateSaved', label: 'Saved', sortable: true },
                { key: 'DateDocsChecked', label: 'Verified', sortable: true },
                { key: 'DateApproved', label: 'Approved', sortable: true },
                { key: 'DateDelivered', label: 'Delivered', sortable: true },
                { key: 'NotifyToEmail', sortable: true },
				{ key: 'actions', label: '' }
			],
			sortBy: 'DateSavedDT',
			sortDesc: true,
			perPage: 25,
			currentPage: 1,
			striped: true,
			bordered: true,
			hover: false
		}
	},
	methods: {
		deleteRequest: function (userRequestID) {
			$.confirm({
				title: 'Confirm!',
				content: 'Are you sure that you want to delete this Request?',
				animation: 'zoom',
				closeAnimation: 'scale',
				type: 'orange',
				buttons: {
					confirm: function () {
						//eliminar
						$.ajax({
							url: '/Users/DeleteUserRequest/' + userRequestID,
							cache: false,
							type: 'DELETE',
							success: function (data) {
                                if (data.ResponseType === 1) {
                                    let indexToDelete = 0;
                                    $.each(self.Requests, function (i, request) {
                                        if (request.UserRequestID === userRequestID) {
                                            indexToDelete = i;
                                        }
                                    });
                                    self.Requests.splice(indexToDelete, 1);
								}
						//			//eliminado
						//		} else {
						//			$.alert({
						//				title: 'Error deleting your request. Please try again.',
						//				content: data.ResponseMessage,
						//				animation: 'zoom',
						//				closeAnimation: 'scale',
						//				autoClose: 'ok|3000',
						//				type: 'red'
						//			});
						//		}
							},
							error: function (xhr, status, error) {
								$.alert({
									title: 'Error Deleting',
									content: error,
									animation: 'zoom',
									closeAnimation: 'scale',
									autoClose: 'ok|3000',
									type: 'red'
								});
							}
						});
					},
					cancel: function () {
						
					}
				}
			});
			
		},
		getLists: function () {
			this.Requests = window.Requests;
		}
	},
	mounted: function () {
		//iniciar la sesión
		this.Session().getSessionDetails();

		this.getLists();
	}
});