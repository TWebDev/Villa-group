var UserRequest = new Vue({
    el: '#app',
    mixins: [ePlatUtils],
    data: {
        Shared: ePlatStore,
        UserInfo: {
            UserTemporalID: null,
            FirstName: '',
            LastName: '',
            Email: '',
            JobPosition: '',
            BasedOn: '',
            BasedOnUser: '',
            DirectSupervisor: '',
            DocumentPath: null,
            DocumentName: null
        },
        UserRequest: {
            UserRequestID: null,
            Users: [],
            System: '',
            Destinations: [],
            DestinationsNames: [],
            Terminals: [],
            TerminalsNames: [],
            RequestedBy: '',
            NotifyTo: '',
            DocumentPath: null,
            DocumentName: null
        },
        UsersDataTable: {
            fields: [
                { key: 'FirstName', sortable: false },
                { key: 'LastName', sortable: false },
                { key: 'Email', sortable: false },
                { key: 'JobPosition', sortable: false },
                { key: 'BasedOnUser', label: 'Based on', sortable: false },
                { key: 'DirectSupervisor', sortable: false },
                { key: 'actions', label: '', sortable: false }
            ],
            sortBy: 'index',
            sortDesc: false,
            perPage: 20,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: true
        },
        Uploader: {
            type: 1,
            document: '',
            requestID: null,
            id: null
        },
        showUserInfo: false,
        Systems: [],
        step: 1,
        location: ''
    },
    methods: {
        addUser: function () {
            if (this.UserInfo.FirstName != ''
                && this.UserInfo.LastName != ''
                && this.UserInfo.Email != ''
                && this.UserInfo.JobPosition != ''
                && this.UserInfo.DirectSupervisor != '') {
                this.showUserInfo = false;
                let Users = self.Shared.State.DependentFields.Fields.filter(function (item) {
                    return item.Field == "BasedOn"
                })[0].Values;
                this.UserInfo.BasedOnUser = _.find(Users, function (d) {
                    return d.Value == self.UserInfo.BasedOn
                }).Text;
                this.UserInfo.UserTemporalID = this.Guid().newGuid();
                this.UserRequest.Users.push(_.clone(this.UserInfo));
                this.UserInfo.UserTemporalID = null;
                this.UserInfo.FirstName = '';
                this.UserInfo.LastName = '';
                this.UserInfo.Email = '';
                this.UserInfo.JobPosition = '';
                this.UserInfo.BasedOn = '';
                this.UserInfo.BasedOnUser = '';
                this.UserInfo.DirectSupervisor = '';
            }
        },
        deleteUser: function (index) {
            this.UserRequest.Users.splice(index, 1);
        },
        getLists: function () {
            //dependent fields
            this.UI().loadDependentFields('/Users/GetRequestDependentFields', false, function () {
                //asignar listas
                self.Systems = self.Shared.State.DependentFields.Fields.filter(function (item) {
                    return item.Field == "System"
                })[0].Values;
            });
        },
        saveUserRequest: function () {
            let self = this;
            self.UserRequest.UserRequestID = self.Guid().newGuid();
            this.addUser();
            $.ajax({
                url: '/Users/SaveUserRequest',
                cache: false,
                type: 'POST',
                data: {
                    userRequestID: self.UserRequest.UserRequestID,
                    notifyTo: self.UserRequest.NotifyTo,
                    jsonModel: $.toJSON(self.UserRequest)
                },
                success: function (data) {
                    if (data.ResponseType == 1) {
                        self.UserRequest.UserRequestID = data.UserRequestID;
                        self.step = 2;
                    } else {
                        $.alert({
                            title: 'Error saving your request. Please try again.',
                            content: data.ResponseMessage,
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'red'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    $.alert({
                        title: 'Error Importing',
                        content: error,
                        animation: 'zoom',
                        closeAnimation: 'scale',
                        autoClose: 'ok|3000',
                        type: 'red'
                    });
                }
            });
        },
        printRequest: function () {
            window.print();
            this.step = 3;
        },
        getTodayDate: function (culture) {
            let dateStr = '';
            if (culture == "es-MX") {
                moment.locale('es');
                dateStr = moment().format('DD') + ' de ' + moment(moment(), 'es').format('MMMM') + ' de ' + moment().format('YYYY');
                moment.locale('en');
            } else {
                dateStr = moment().format('MMMM') + ' ' + moment().format('DD') + ', ' + moment().format('YYYY');
            }
            return dateStr;
        },
        goToStep: function (step) {
            this.step = step;
        },
        putInHold: function () {
            let self = this;
            $.ajax({
                url: '/Users/UserRequestChangeStatus',
                cache: false,
                data: {
                    userRequestID: self.UserRequest.UserRequestID,
                    eventID: 5,
                },
                type: 'POST',
                success: function (data) {
                    if (data.ResponseType == 1) {
                        $.alert({
                            title: 'Done',
                            content: 'You can close this tab and come back when you have the contracts ready to upload, using this URL: https://eplat.villagroup.com/users/userrequest/' + self.UserRequest.UserRequestID + ' which was also sent to the requestor\'s email.',
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            type: 'green',
                            columnClass: 'large'
                        });
                    } else {
                        $.alert({
                            title: 'Error putting in hold your request',
                            content: 'Please notify by email to gguerrap@villagroup.com.',
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'red'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    $.alert({
                        title: 'Error putting in hold your request',
                        content: 'Please notify by email to gguerrap@villagroup.com.',
                        animation: 'zoom',
                        closeAnimation: 'scale',
                        autoClose: 'ok|3000',
                        type: 'red'
                    });
                }
            });
        },
        getFollowUpKey: function () {
            let self = this;
            //validar que esté la documentación completa
            let valid = true;
            let msg = '';
            if (self.UserRequest.DocumentPath == null) {
                valid = false;
                msg = 'User Request document is not attached.';
            }
            self.UserRequest.Users.forEach(function (user) {
                if (valid && user.DocumentPath == null) {
                    valid = false;
                    msg = user.FirstName + ' ' + user.LastName + ' Confidenciality Letter is not attached';
                }
            });

            //enviar notificación de nueva solicitud
            if (valid) {
                $.ajax({
                    url: '/Users/NotifyNewUserRequest/' + self.UserRequest.UserRequestID,
                    cache: false,
                    type: 'POST',
                    success: function (data) {
                        if (data.ResponseType == 1) {
                            self.step = 4;
                        } else {
                            $.alert({
                                title: 'Error saving your request',
                                content: 'Please notify by email to gguerrap@villagroup.com.',
                                animation: 'zoom',
                                closeAnimation: 'scale',
                                autoClose: 'ok|3000',
                                type: 'red'
                            });
                        }
                    },
                    error: function (xhr, status, error) {
                        $.alert({
                            title: 'Error saving your request',
                            content: 'Please notify by email to gguerrap@villagroup.com.',
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'red'
                        });
                    }
                });
            } else {
                $.alert({
                    title: 'Error!',
                    content: msg,
                    animation: 'zoom',
                    closeAnimation: 'scale',
                    autoClose: 'ok|3000',
                    type: 'red'
                });
            }            
        },
        openUploadModal: function (type, document, id) {
            $('#exampleModalLabel').html('Upload file for ' + document);
            let userTemporalid = id;
            $('#messages').html('');

            //uploader
            $fub = $('#fine-uploader-basic');
            $messages = $('#messages');
            let self = this;

            let uploader = new qq.FineUploaderBasic({
                button: $fub[0],
                request: {
                    endpoint: '/Users/UploadDocument',
                    params: {
                        type: type,
                        requestID: self.UserRequest.UserRequestID,
                        id: id
                    }
                },
                validation: {
                    allowedExtensions: ['pdf', 'jpg', 'docx'],
                    sizeLimit: 5242880 // 5MB
                },
                callbacks: {
                    onSubmit: function (id, fileName) {
                        $messages.append('<div id="file-' + id + '" class="alert" style="margin: 20px 0 0"></div><div class="progress-' + id + '"><div class="progress-bar" role="progressbar" style="width: 0%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div></div>');
                    },
                    onUpload: function (id, fileName) {
                        $('#file-' + id).addClass('alert-info')
                            .html('<img src="/images/loading.gif" alt="Initializing. Please hold." style="width:16px;"> ' +
                                'Initializing ' +
                                '“' + fileName + '”');
                    },
                    onProgress: function (id, fileName, loaded, total) {
                        if (loaded < total) {
                            progress = Math.round(loaded / total * 100) + '% of ' + Math.round(total / 1024) + ' kB';
                            $('#file-' + id).removeClass('alert-info')
                                .html('<img src="/images/loading.gif" alt="In progress. Please hold." style="width:16px;"> ' +
                                    'Uploading ' +
                                    '“' + fileName + '” ' +
                                    progress);
                            $('#progress-' + id).css('width', progress + '%');
                        } else {
                            $('#file-' + id).addClass('alert-info')
                                .html('<img src="/images/loading.gif" alt="Saving. Please hold." style="width:16px;"> ' +
                                    'Saving ' +
                                    '“' + fileName + '”');

                        }
                    },
                    onComplete: function (id, fileName, responseJSON) {
                        if (responseJSON.success) {
                            $('#file-' + id).removeClass('alert-info')
                                .addClass('alert-success')
                                .html('<i class="icon-ok"></i> ' +
                                    'Successfully saved ' +
                                    '“' + fileName + '”');

                            if (type == 1) {
                                self.UserRequest.DocumentPath = responseJSON.path.path;
                                self.UserRequest.DocumentName = fileName;
                            } else {
                                _.find(self.UserRequest.Users, function (d) {
                                    return d.UserTemporalID == userTemporalid
                                }).DocumentPath = responseJSON.path.path;
                                _.find(self.UserRequest.Users, function (d) {
                                    return d.UserTemporalID == userTemporalid
                                }).DocumentName = fileName;
                            }
                        } else {
                            $('#file-' + id).removeClass('alert-info')
                                .addClass('alert-error')
                                .html('<i class="icon-exclamation-sign"></i> ' +
                                    'Error with ' +
                                    '“' + fileName + '”: ' +
                                    responseJSON.response.Exception.Message);
                        }
                    }
                },
                debug: true
            });
        }
    },
    mounted: function () {
        //obtener listas
        this.getLists();

        //iniciar selectores múltiples
        $('[multiple="multiple"]').multiselect({
            buttonWidth: '100%',
            includeSelectAllOption: true,
        });

        //eventos de multiselect
        $('#Terminals').on('change', function () {
            UserRequest.UserRequest.Terminals = $('#Terminals').val();
            UserRequest.UserRequest.TerminalsNames = '';
            $.each(UserRequest.UserRequest.Terminals, function (i, t) {
                if (UserRequest.UserRequest.TerminalsNames != '') {
                    UserRequest.UserRequest.TerminalsNames += ', ';
                };
                UserRequest.UserRequest.TerminalsNames += $('#Terminals option[value="' + t + '"]').text();
            });
        });

        $('#Destinations').on('change', function () {
            UserRequest.UserRequest.Destinations = $('#Destinations').val();
            UserRequest.UserRequest.DestinationsNames = '';
            $.each(UserRequest.UserRequest.Destinations, function (i, t) {
                if (UserRequest.UserRequest.DestinationsNames != '') {
                    UserRequest.UserRequest.DestinationsNames += ', ';
                };
                UserRequest.UserRequest.DestinationsNames += $('#Destinations option[value="' + t + '"]').text();
            });
        });

        if (window.UserRequestModel !== undefined) {
            this.UserRequest = window.UserRequestModel;
            Vue.nextTick(function () {
                $('#System').trigger('change');
            })
            this.step = 3;
        }
    }
})