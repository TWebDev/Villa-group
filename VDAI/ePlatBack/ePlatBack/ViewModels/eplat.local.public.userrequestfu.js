var UserRequestFU = new Vue({
    el: '#app',
    mixins: [ePlatUtils],
    data: {
        Shared: ePlatStore,
        UserRequest: {
            UserRequestID: null,
            Users: [],
            System: '',
            Destinations: [],
            DestinationsNames: '',
            Terminals: [],
            TerminalsNames: '',
            RequestedBy: '',
            NotifyTo: '',
            DocumentPath: null,
            DocumentName: null,
            Saved: '',
            Checked: '',
            Approved: '',
            Delivered: ''
        },
        checked: '',
        approved: '',
        delivered: ''
    },
    methods: {
        saveNotes: function () {
            let self = this;
            $.ajax({
                url: '/Users/SaveRequestNote',
                cache: false,
                type: 'POST',
                data: {
                    userRequestID: self.UserRequest.UserRequestID,
                    notes: self.UserRequest.Notes
                },
                success: function (data) {
                    if (data.ResponseType == 1) {
                        $.alert({
                            title: 'Notes successfully saved',
                            content: data.ResponseMessage,
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'green'
                        });
                    } else {
                        $.alert({
                            title: 'Error saving status change',
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
                        title: 'Error Saving',
                        content: error,
                        animation: 'zoom',
                        closeAnimation: 'scale',
                        autoClose: 'ok|3000',
                        type: 'red'
                    });
                }
            });
        },
        triggerEvent: function (eventid) {
            let self = this;
            $.ajax({
                url: '/Users/UserRequestChangeStatus',
                cache: false,
                type: 'POST',
                data: {
                    userRequestID: self.UserRequest.UserRequestID,
                    eventID: eventid,
                },
                success: function (data) {
                    if (data.ResponseType == 1) {
                        switch (eventid) {
                            case 2:
                                self.UserRequest.Checked = moment().format("YYYY-MM-DD HH:mm:ss A");
                                break;
                            case 3:
                                self.UserRequest.Approved = moment().format("YYYY-MM-DD HH:mm:ss A");
                                break;
                            case 4:
                                self.UserRequest.Delivered = moment().format("YYYY-MM-DD HH:mm:ss A");
                                break;
                        }
                        $('#event' + eventid + '-notchecked').addClass('d-none');
                        $('#event' + eventid + '-checked').removeClass('d-none');
                    } else {
                        $.alert({
                            title: 'Error saving status change',
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
                        title: 'Error Saving',
                        content: error,
                        animation: 'zoom',
                        closeAnimation: 'scale',
                        autoClose: 'ok|3000',
                        type: 'red'
                    });
                }
            });
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
    mounted: function() {
        //iniciar la sesión
        if (window.auth) {
            this.Session().getSessionDetails();
        }
        this.UserRequest = window.UserRequest;
    }
})