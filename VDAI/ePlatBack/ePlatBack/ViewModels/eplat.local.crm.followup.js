var FollowUp = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data: {
        Shared: ePlatStore,
        Lists: {
            LeadSources: [],
            LeadSourceChannels: [],
            LeadStatus: [],
            BookingStatus: [],
            LeadTypes: [],
            InterestLevels: [],
            DiscardReasons: [],
            Agents: [],
            InteractionTypes: [],
            QualificationStatus: [],
            Destinations: [],
            InputMethods: [],
            Countries: [],
            States: []
        },
        Search: {
            SearchLog: {
                Date: moment().format('YYYY-MM-DD'),
                Agents: []
            },
            SearchMails: {
                MailFolder: 'inbox',
                SearchText: ''
            },
            SearchLeads: {
                Search_FromDate: null,
                Search_ToDate: null,
                Search_GuestName: null,
                Search_LeadStatusID: [],
                Search_BookingStatusID: [],
                Search_LeadTypeID: [],
                Search_LeadSourceChannelID: [],
                Search_AssignedToUserID: null
            }
        },
        Cards: {
            Log: {
                Search: true
            },
            Leads: {
                Search: false,
                Summary: true,
                Table: true,
                KanBan: false,
                Profile: false
            }
        },
        Overview: {
            Sales: {},
            Marketing: {},
            Summary: {}
        },
        Leads: [],
        LeadsDataTable: {
            fields: [
                { key: 'Checked', label: '' },
                { key: 'lead', label: 'Lead' },
                { key: 'status', label: 'Status', sortable: true },
                { key: 'activity', label: 'Activity', sortable: true },
                { key: 'quote', label: 'Quote', sortable: true },
                { key: 'CustomerInfo.AssignedToAgent', label: 'Assigned To', sortable: true }
            ],
            sortBy: 'status',
            sortDesc: true,
            perPage: 10,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: true
        },
        Lead: {
            LeadID: null,
            CustomerInfo: {
                FirstName: '',
                LastName: '',
                CountryID: null,
                StateID: null,
                State: '',
                City: '',
                LeadSourceID: null,
                Source: '',
                LeadSourceChannelID: null,
                Channel: '',
                LeadTypeID: null,
                FirstContactType: '',
                InterestedInDestinationID: null,
                InterestedInDestination: '',
                DestinationID: null,
                Destination: '',
                InputMethodID: 1,
                InputMethod: 'Manual',
                BookingStatusID: 19,
                BookingStatus: 'Not Contacted',
                InterestLevelID: null,
                InterestLevel: '',
                DateSaved: '',
                AssignedToUserID: null,
                AssignedToAgent: '',
                Mood: null,
                QualificationStatusID: null,
                Qualification: '',
                LeadStatusID: null,
                LeadStatus: '',
                NQReasonID: null                
            },
            ContactInfo: {
                Phones: [],
                Emails: []
            }
        },
        Phone: {
            PhoneID: null,
            PhoneTypeID: 4,
            PhoneType: 'Unknown',
            Phone: '',
            Ext: '',
            DoNotCall: false,
            Main: false,
            Editing: false
        },
        Email: {
            EmailID: null,
            Email: '',
            Main: false,
            Editing: false
        },
        Logs: [],
        LogsDataTable: {
            fields: [
                { key: 'Date', sortable: true },
                { key: 'InteractionType', label: 'Contact Type', sortable: true },
                { key: 'Details' },
                { key: 'Agent', label: 'Agent', sortable: true },
                { key: 'Customer', sortable: true },
                { key: 'Phone', sortable: true },
                { key: 'Email', sortable: true },
                { key: 'QualificationStatus', label: 'Q/NQ', sortable: true },
                { key: 'InterestedInDestination', label: 'Destination', sortable: true },
                { key: 'Quote', sortable: true },
                { key: 'FirstContact', label: 'FC', sortable: true },
                { key: 'InterestLevel', label: 'Interest', sortable: true },
                { key: 'BookingStatus', label: 'Status', sortable: true },
                { key: 'InteractionComments', label: 'Comments', sortable: true },
                { key: 'Actions', label: '' }
            ],
            sortBy: 'Date',
            sortDesc: true,
            perPage: 10,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: true
        },
        Mails: [],
        Mail: null,
        NewMail: {
            To: [],
            CC: [],
            BCC: [],
            Subject: '',
            Body: '',
            Folder: 'outbox',
            LeadID: null,
            ShowNewMail: false
        }
    },
    computed: {
        DateInputYYYYMMDD: function () {
            if (this.Lead.CustomerInfo.DateSaved !== '') {
                return moment(this.Lead.CustomerInfo.DateSaved).format("YYYY-MM-DD");
            }
        }
    },
    watch: {
        'Lead.CustomerInfo.LeadSourceID': function (newVal, oldVal) {
            let source = this.Lists.LeadSources.filter(function (ls) {
                return ls.Value == newVal;
            });
            if (source.length === 1) {
                this.Lead.CustomerInfo.Source = source[0].Text;
            }
        },
        'Lead.CustomerInfo.LeadSourceChannelID': function (newVal, oldVal) {
            let channel = this.Lists.LeadSourceChannels.filter(function (ch) {
                return ch.Value == newVal;
            });
            if (channel.length === 1) {
                this.Lead.CustomerInfo.Channel = channel[0].Text;
            }
        },
        'Lead.CustomerInfo.LeadTypeID': function (newVal, oldVal) {
            let type = this.Lists.LeadTypes.filter(function (ty) {
                return ty.Value == newVal;
            });
            if (type.length === 1) {
                this.Lead.CustomerInfo.FirstContactType = type[0].Text;
            }
        },
        'Lead.CustomerInfo.AssignedToUserID': function (newVal, oldVal) {
            if (oldVal == '' && newVal != null && newVal != '') {
                this.Lead.CustomerInfo.LeadStatusID = 2;
                this.Lead.CustomerInfo.LeadStatus = 'Assigned';
            }
        },
        'Phone.PhoneTypeID': function (newVal, oldVal) {
            if (newVal != 2) {
                this.Phone.Ext = '';
            }
            let phoneType = this.Lists.PhoneTypes.filter(function (pt) {
                return pt.Value == newVal;
            });
            if (phoneType.length === 1) {
                this.Phone.PhoneType = phoneType[0].Text;
            }
        }
    },
    methods: {
        getLog: function () {
            var self = this;
            $.ajax({
                url: '/FollowUp/GetLog',
                cache: false,
                type: 'GET',
                data: this.Search.SearchLog,
                success: function (data) {
                    self.Logs = data;
                    if (self.Logs.length > 0) {
                        self.Search.SearchLog.Visible = false;
                    }
                    //agregar listas a los formularios de edición
                    if (self.Shared.State.DependentFields.Fields.length > 0) {
                        self.Logs.map(function (item) {
                            item.QualificationStatusList = self.Shared.State.DependentFields.Fields.filter(function (item) {
                                return item.Field == "QualificationStatusID"
                            })[0].Values;
                            item.DestinationsList = self.Shared.State.DependentFields.Fields.filter(function (item) {
                                return item.Field == "InterestedInDestinationID"
                            })[0].Values;
                            item.InterestLevelsList = self.Shared.State.DependentFields.Fields.filter(function (item) {
                                return item.Field == "InterestLevelID"
                            })[0].Values;
                            item.BookingStatusList = self.Shared.State.DependentFields.Fields.filter(function (item) {
                                return item.Field == "BookingStatusID"
                            })[0].Values;
                        });
                    }
                    //generar tooltips para comentarios
                    Vue.nextTick(function () {
                        // DOM updated
                        $('tblLogs [data-toggle="tooltip"]').tooltip();
                    });
                }
            });
        },
        saveLog: function (row) {
            //guardar log
            $.ajax({
                url: '/FollowUp/SaveLog',
                cache: false,
                type: 'POST',
                data: row.item,
                success: function (data) {
                    row.EmailID = data.Log.EmailID;
                    row.LeadID = data.Log.LeadID;
                    row.InteractionID = data.Log.InteractionID;
                    row.item.QualificationStatusID = data.Log.QualificationStatusID;
                    row.item.QualificationStatus = _.find(row.item.QualificationStatusList, function (q) { return q.Value == row.item.QualificationStatusID }).Text;
                    row.item.InterestedInDestinationID = data.Log.InterestedInDestinationID;
                    row.item.InterestedInDestination = _.find(row.item.DestinationsList, function (d) { return d.Value == row.item.InterestedInDestinationID }).Text;
                    row.item.InterestLevelID = data.Log.InterestLevelID;
                    row.item.InterestLevel = _.find(row.item.InterestLevelsList, function (i) { return i.Value == row.item.InterestLevelID }).Text;
                    row.item.BookingStatusID = data.Log.BookingStatusID;
                    row.item.BookingStatus = _.find(row.item.BookingStatusList, function (b) { return b.Value == row.item.BookingStatusID }).Text;

                    //tooltip
                    $('[data-toggle="tooltip"]').tooltip();

                    //copiar datos de customer a otros contactos con el mismo teléfono
                    self.Logs.map(function (item) {
                        if (item.Phone == row.item.Phone) {
                            item.FirstName = row.item.FirstName;
                            item.LastName = row.item.LastName;
                            item.Phone = row.item.Phone;
                            item.Email = row.item.Email;
                            item.QualificationStatus = row.item.QualificationStatus;
                            item.QualificationStatusID = row.item.QualificationStatusID;
                            item.InterestedInDestination = row.item.InterestedInDestination;
                            item.InterestedInDestinationID = row.item.InterestedInDestinationID;
                        }
                    });
                    row.item._showDetails = false;
                }
            });
        },

        sendAndReceive: function () {
            let self = this;
            //revisar si hay sin enviar, para enviar

            //recibir correos de la carpeta seleccionada
            $.ajax({
                url: '/FollowUp/GetMails',
                cache: false,
                type: 'GET',
                data: {
                    folder: self.Search.SearchMails.MailFolder
                },
                success: function (data) {
                    self.Mails = data.Mails;
                }
            });
        },
        openMail: function (id, index) {
            let self = this;
            self.Mail = _.find(self.Mails, function (m) { return m.MailMessageID == id });
            self.Mail.Index = index;
            Vue.nextTick(function () {
                $('[data-toggle="tooltip"]').tooltip();
            });
        },
        searchMails: function () {
            $.ajax({
                url: '/FollowUp/GetMails',
                cache: false,
                type: 'GET',
                data: {
                    folder: self.Search.SearchMails.MailFolder,
                    search: self.Search.SearchMails.SearchText
                },
                success: function (data) {
                    self.Mails = data.Mails;
                }
            });
        },
        deleteMail: function (id, index) {
            let self = this;
            $.ajax({
                url: '/FollowUp/DeleteMail/' + id,
                cache: false,
                type: 'DELETE',
                success: function (data) {
                    self.Mails.splice(index, 1);
                    if (self.Mail.MailMessageID == id) {
                        self.Mail = null;
                    }
                }
            });
        },

        updateBookingStatus: function () {
        },
        newLead: function () {
            let self = this;
            this.Cards.Leads.Profile = true;
            this.Lead.LeadID = null;
            this.Lead.CustomerInfo = {
                FirstName: '',
                LastName: '',
                CountryID: null,
                StateID: null,
                State: '',
                City: '',
                LeadSourceID: null,
                Source: '',
                LeadSourceChannelID: null,
                Channel: '',
                LeadTypeID: null,
                FirstContactType: '',
                InterestedInDestinationID: null,
                InterestedInDestination: '',
                DestinationID: null,
                Destination: '',
                InputMethodID: 1,
                InputMethod: 'Manual',
                BookingStatusID: 19,
                BookingStatus: 'Not Contacted',
                InterestLevelID: null,
                InterestLevel: '',
                DateSaved: '',
                AssignedToUserID: (self.Lists.Agents.length > 0 ? _.find(self.Lists.Agents, 'Selected').Value : null),
                AssignedToAgent: (self.Lists.Agents.length > 0 ? _.find(self.Lists.Agents, 'Selected').Text : ''),
                Mood: null,
                QualificationStatusID: null,
                Qualification: '',
                LeadStatusID: (AssignedToUserID !== null ? 2 : 19),
                LeadStatus: (AssignedToUserID !== null ? 'Assigned' : 'Unassigned'),
                NQReasonID: null,
                Phones: [],
                Emails: []
            };
        },

        addPhoneToLead: function () {
            let phone = this.Phone.Phone;
            if (this.Phone.Main) {
                for (let p = 0; p < this.Lead.ContactInfo.Phones.length; p++) {
                    this.Lead.ContactInfo.Phones[p].Main = false;
                }
            }
            if (this.Lead.ContactInfo.Phones.length === 0) {
                this.Phone.Main = true;
            }
            if (this.Phone.PhoneTypeID == null) {
                this.Phone.PhoneTypeID = 4;
                this.Phone.PhoneType = 'Unknown';
            }
            this.Lead.ContactInfo.Phones.push(_.clone(this.Phone));
            this.clearPhone();
            //buscar coincidencias con otros leads
            $.ajax({
                url: '/FollowUp/PhoneAnalysis',
                cache: false,
                type: 'POST',
                data: {
                    phone: phone
                },
                success: function (data) {
                    if (data.ResponseType === 1) {
                        //revisar si está relacionado a un lead
                        if (data.Analysis.LeadID !== null) {
                            $.confirm({
                                content: 'There is a lead already registered with the phone number: ' + phone + '. Would you like to open that lead profile?',
                                buttons: {
                                    openLead: {
                                        text: 'Open Lead',
                                        keys: ['enter'],
                                        action: function () {
                                            //abrir lead
                                            console.log('open lead: ' + data.LeadID);
                                        }
                                    },
                                    continue: {
                                        text: 'No, thank you',
                                        keys: ['esc'],
                                        action: function () {

                                        }
                                    }
                                }
                            });
                        }
                        if (data.Analysis.CountryID !== null) {
                            this.Lead.CustomerInfo.CountryID = data.Analysis.CountryID;
                        }
                        if (data.Analysis.StateID !== null) {
                            this.Lead.CustomerInfo.StateID = data.Analysis.StateID;
                            this.Lead.CustomerInfo.State = data.Analysis.State;
                        }
                        if (data.Analysis.City !== null) {
                            this.Lead.CustomerInfo.City = data.Analysis.City;
                        }
                    }
                },
                error: function (xhr, status, error) {
                    console.log('Error getting phone analysis');
                }
            });
        },
        clearPhone: function () {
            this.Phone = {
                PhoneID: null,
                PhoneTypeID: 4,
                PhoneType: 'Unknown',
                Phone: '',
                Ext: '',
                DoNotCall: false,
                Main: false,
                Editing: false
            };
        },
        updatePhoneToLead: function (p) {
            let self = this;
            let phoneType = self.Lists.PhoneTypes.filter(function (pt) {
                return pt.Value == self.Lead.ContactInfo.Phones[p].PhoneTypeID;
            });
            if (phoneType.length === 1) {
                self.Lead.ContactInfo.Phones[p].PhoneType = phoneType[0].Text;
            }
            if (self.Lead.ContactInfo.Phones[p].Main) {
                for (let x = 0; x < this.Lead.ContactInfo.Phones.length; x++) {
                    if (x !== p) {
                        self.Lead.ContactInfo.Phones[x].Main = false;
                    }
                }
            }
            if (self.Lead.ContactInfo.Phones[p].PhoneTypeID != 2) {
                self.Lead.ContactInfo.Phones[p].Ext = '';
            }
            self.Lead.ContactInfo.Phones[p].Editing = false;
        },
        editPhone: function (p) {
            ////_.clone(this.Lead.ContactInfo.Phones[p]);
            this.Lead.ContactInfo.Phones[p].Editing = true;
        },
        deletePhone: function (p) {
            let deletingMain = false;
            if (this.Lead.ContactInfo.Phones[p].Main) {
                deletingMain = true;
            }
            this.Lead.ContactInfo.Phones.splice(p, 1);
            if (deletingMain && this.Lead.ContactInfo.Phones.length > 0) {
                this.Lead.ContactInfo.Phones[0].Main = true;
            }
        },

        addEmailToLead: function () {
            if (this.Email.Main) {
                for (let e = 0; e < this.Lead.ContactInfo.Emails.length; e++) {
                    this.Lead.ContactInfo.Emails[e].Main = false;
                }
            }
            if (this.Lead.ContactInfo.Emails.length === 0) {
                this.Email.Main = true;
            }
            this.Lead.ContactInfo.Emails.push(this.Email);
            this.clearEmail();
        },
        clearEmail: function () {
            this.Email = {
                EmailID: null,
                Email: '',
                Main: false,
                Editing: false
            };
        },
        editEmail: function (e) {
            this.Lead.ContactInfo.Emails[e].Editing = true;
        },
        deleteEmail: function (e) {
            let deletingMain = false;
            if (this.Lead.ContactInfo.Emails[e].Main) {
                deletingMain = true;
            }
            this.Lead.ContactInfo.Emails.splice(e, 1);
            if (deletingMain && this.Lead.ContactInfo.Emails.length > 0) {
                this.Lead.ContactInfo.Emails[0].Main = true;
            }
        },
        updateEmailToLead: function (e) {
            let self = this;
            if (self.Lead.ContactInfo.Emails[e].Main) {
                for (let x = 0; x < this.Lead.ContactInfo.Emails.length; x++) {
                    if (x !== e) {
                        self.Lead.ContactInfo.Emails[x].Main = false;
                    }
                }
            }
            self.Lead.ContactInfo.Emails[e].Editing = false;
        },

        saveLead: function () {
            let self = this;

            //validación
            let valid = true;
            let errMsg = '';
            if (self.Lead.FirstName == '' || self.Lead.LastName == '') {
                errMsg = 'You need to specify First Name and Last name in order to save the lead.';
            }
            if (valid && self.Lead.CountryID == null)

                $.ajax({
                    url: '/FollowUp/SaveLead',
                    cache: false,
                    type: 'POST',
                    data: {
                        lead: JSON.stringify(self.Lead)
                    },
                    success: function (data) {
                        if (data.ResponseType == 1) {
                            self.Lead = data.Lead;
                            $.alert({
                                title: 'Lead Succesfully Saved',
                                content: 'Lead information was successfully saved.',
                                animation: 'zoom',
                                closeAnimation: 'scale',
                                autoClose: 'ok|3000',
                                type: 'green'
                            });
                        } else {
                            $.alert({
                                title: 'Error Saving Info',
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
                            title: 'Error Saving Info',
                            content: error,
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'red'
                        });
                    }
                });
        },
        getChannels: function (lsid) {
            return this.Lists.LeadSourceChannels
                .filter(function (ch) {
                    return ch.ParentValue == lsid;
                });
        },
        searchLeads: function () {
            let self = this;
            $.ajax({
                url: '/FollowUp/SearchLeads',
                cache: false,
                type: 'GET',
                data: self.Search.SearchLeads,
                success: function (data) {
                    self.Leads = data.Leads;
                    self.LeadsSummary = data.Summary;
                },
                error: function (xhr, status, error) {
                    $.alert({
                        title: 'Error Searching Leads',
                        content: error,
                        animation: 'zoom',
                        closeAnimation: 'scale',
                        autoClose: 'ok|3000',
                        type: 'red'
                    });
                }
            });
        },
        checkAllLeads: function (e) {
            let isChecked = e.target.checked;
            this.Leads.map(function (l) {
                l.Checked = isChecked;
            });
        }
    },
    mounted: function () {
        let self = this;

        //iniciar la sesión
        this.Session().getSessionDetails();

        //iniciar tooltips
        $('[data-toggle="tooltip"]').tooltip();

        //obtener listas que dependen de la terminal
        $('body').on('selectedTerminalChanged', function () {
            //obtener listas que dependen de la terminal
            self.UI().loadDependentFields('/FollowUp/GetDependentFields', true, function () {
                //asignar listas
                self.Lists.LeadSources = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "LeadSourceID";
                    })[0].Values;

                self.Lists.LeadSourceChannels = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "LeadSourceChannelID";
                    })[0].Values;

                self.Lists.LeadStatus = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "LeadStatusID";
                    })[0].Values;

                self.Lists.BookingStatus = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "BookingStatusID";
                    })[0].Values;

                self.Lists.LeadTypes = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "LeadTypeID";
                    })[0].Values;

                self.Lists.InterestLevels = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "InterestLevelID";
                    })[0].Values;

                self.Lists.DiscardReasons = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "DiscardReasonID";
                    })[0].Values;

                self.Lists.Agents = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "AgentID";
                    })[0].Values;

                self.Lists.InteractionTypes = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "InteractionTypeID";
                    })[0].Values;

                self.Lists.QualificationStatus = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "QualificationStatusID";
                    })[0].Values;

                self.Lists.Destinations = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "InterestedInDestinationID";
                    })[0].Values;

                self.Lists.InputMethods = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "InputMethodID";
                    })[0].Values;

                self.Lists.Countries = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "CountryID";
                    })[0].Values;

                self.Lists.States = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "StateID";
                    })[0].Values;

                self.Lists.PhoneTypes = self.Shared.State.DependentFields.Fields
                    .filter(function (item) {
                        return item.Field == "PhoneTypeID";
                    })[0].Values;

                Vue.nextTick(function () {
                    // DOM updated
                    $('#Log_Agents').multiselect('rebuild');
                    $('#Search_LeadStatusID').multiselect('rebuild');
                    $('#Search_BookingStatusID').multiselect('rebuild');
                    $('#Search_LeadTypeID').multiselect('rebuild');
                    $('#Search_LeadSourceChannelID').multiselect('rebuild');
                });
            });
        });

        //iniciar selectores múltiples
        $('[multiple="multiple"]').multiselect({
            buttonWidth: '100%',
            includeSelectAllOption: true
        });

        //iniciar datepickers
        $('.datetimepicker-input').datetimepicker({
            format: 'YYYY-MM-DD'
        });
        $('.datetimepicker-input').on('datetimepicker.hide', function () {
            this.dispatchEvent(new Event('input'));
        });

        //eventos de tabs
        $('a[role="tab"]').on('click', function () {
            localStorage.Eplat_FollowUp_LastTab = $(this).attr('id');
        });

        $('#pills-log-tab').on('click', function () {
            if (self.Logs.length == 0) {
                $('#btnGetLog').trigger('click');
            }
        });

        $('#pills-email-tab').on('click', function () {
            if (self.Mails.length == 0) {
                $('#btnSendAndReceive').trigger('click');
            }
        });

        //última tab seleccionada
        if (localStorage.Eplat_FollowUp_LastTab != undefined) {
            $('#' + localStorage.Eplat_FollowUp_LastTab).trigger('click');
        }

        //autoheight
        $('#pills-tabContent').height(window.innerHeight - 155);
        $('#emailList').height($('#pills-tabContent').height() - 125);
        $('#emailDetails').height($('#pills-tabContent').height() - 83);
        $(window).resize(function () {
            $('#pills-tabContent').height(window.innerHeight - 155);
            $('#emailList').height($('#pills-tabContent').height() - 125);
            $('#emailDetails').height($('#pills-tabContent').height() - 83);
            $('.ck-editor__editable').height($('#pills-tabContent').height() - 313);
        });

        //ckeditor 5
        ClassicEditor
            .create(document.querySelector('#txtMailEditor'), {
                toolbar: ['heading', '|', 'bold', 'italic', 'link', 'bulletedList', 'numberedList', 'blockQuote']
            })
            .then(function (editor) {
                //console.log(editor);
                $('.ck-editor__editable').height($('#pills-tabContent').height() - 313);
            })
            .catch(function (error) {
                //console.log(error);
            });
    }
})