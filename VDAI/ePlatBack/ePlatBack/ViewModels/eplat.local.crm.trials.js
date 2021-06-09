var Trials = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data: {
        Shared: ePlatStore,
        AgentsList: [],
        BookingStatusList: [],
        Trials: [],
        Trial: {
            AssignedTo: '',
            AssignedToUserID: null
        },
        TrialsDataTable: {
            fields: [
                { key: 'ContractNumber', label: 'Contract #', sortable: true },
                { key: 'LastName', label: 'Name', sortable: true },
                { key: 'SalesType', sortable: true },
                { key: 'ContractStatus', sortable: true },
                { key: 'SourceOfSale', sortable: true },
                { key: 'DateSale', label: 'Sales Date', sortable: true },
                { key: 'DateExpiration', label: 'Expiration Date', sortable: true },
                { key: 'Volume', sortable: true },
                { key: 'AssignedTo', sortable: true },
                { key: 'BookingStatus', sortable: true }
            ],
            sortBy: 'DateSale',
            sortDesc: false,
            perPage: 10,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: true
        },
        ImportDataTable: {
            fields: [
                { key: 'ContractNumber', label: 'Contract #', sortable: true },
                { key: 'Reference', label: 'Xref No.', sortable: true },
                { key: 'LastName', label: 'Name', sortable: true },
                { key: 'SalesType', sortable: true },
                { key: 'ContractStatus', sortable: true },
                { key: 'SourceOfSale', sortable: true },
                { key: 'DateInput', label: 'Date Entered', sortable: true },
                { key: 'DateSale', label: 'Sales Date', sortable: true },
                { key: 'Volume', sortable: true },
                { key: 'DateExpiration', label: 'Expiration Date', sortable: true },
                { key: 'AssignedToUserID', sortable: false },
            ],
            sortBy: 'DateSale',
            sortDesc: false,
            perPage: 10,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: true
        },
        showTrialsImport: false,
        showTrialInfo: false,
        importInput: '',
        ImportCollection: []
    },
    computed: {
        DateInputYYYYMMDD: {
            get: function () {
                return moment(this.Trial.DateInput).format("YYYY-MM-DD");
            },
            set: function (newValue) {
                this.Trial.DateInput = newValue;
            }
        },
        DateSaleYYYYMMDD: {
            get: function () {
                return moment(this.Trial.DateSale).format("YYYY-MM-DD");
            },
            set: function (newValue) {
                this.Trial.DateSale = newValue;
            }
        },
        DateExpirationYYYYMMDD: {
            get: function () {
                return moment(this.Trial.DateExpiration).format("YYYY-MM-DD");
            },
            set: function (newValue) {
                this.Trial.DateExpiration = newValue;
            }
        },
        Volume: {
            get: function () {
                return this.Filters().currency(this.Trial.Volume, '');
            },
            set: function (newValue) {
                this.Trial.Volume = this.Filters().currencyToDecimal(newValue);
            }
        },
        Phone1: {
            get: function () {
                return this.Filters().maskedPhone(this.Trial.Phone1);
            },
            set: _.debounce(function (newValue) {
                if (newValue.indexOf("•") < 0) {
                    this.Trial.Phone1 = newValue.replace(' ', '');
                }
            }, 1000)
        },
        Phone2: {
            get: function () {
                return this.Filters().maskedPhone(this.Trial.Phone2);
            },
            set: _.debounce(function (newValue) {
                if (newValue.indexOf("•") < 0) {
                    this.Trial.Phone2 = newValue.replace(' ', '');
                }
            }, 1000)
        },
        Email1: {
            get: function () {
                return this.Filters().maskedEmail(this.Trial.Email1);
            },
            set: _.debounce(function (newValue) {
                this.Trial.Email1 = newValue;
            }, 1000)
        },
        Email2: {
            get: function () {
                return this.Filters().maskedEmail(this.Trial.Email2);
            },
            set: _.debounce(function (newValue) {
                this.Trial.Email2 = newValue;
            }, 1000)
        },
        AssignedToUser: {
            get: function () {
                return this.Trial.AssignedToUserID;
            }
        }
    },
    methods: {
        openTrial: function (row, i) {
            this.Trial = row;
            this.showTrialInfo = true;
            $('#tblTrials tr').removeClass('selected');
            $('#tblTrials tr[aria-rowindex="' + (i + 1) + '"]').addClass('selected');
            this.UI().scrollTo('divTrialInfo');
        },
        setSearchResults: function (data) {
            this.Trials = data;
            this.UI().showSearchCard();
        },
        toggleImportCard: function () {
            this.showTrialsImport = !this.showTrialsImport;
            this.importInput = '';
            $('.import-1').slideDown('fast');
            $('.import-2').slideUp('fast');
        },
        generateImportCollection: function () {
            if (this.importInput.trim() != "") {
                let lines = this.importInput.split('\n');
                let self = this;
                self.ImportCollection = [];
                lines.map(function (item) {
                    let tabs = item.split('\t');
                    if (tabs[0] != "") {
                        self.ImportCollection.push({
                            ContractNumber: tabs[0],
                            Reference: tabs[1],
                            LastName: tabs[3],
                            SalesType: tabs[5],
                            ContractStatus: tabs[6],
                            SourceOfSale: tabs[7],
                            DateInput: moment(tabs[8], 'MM/DD/YY').format('YYYY-MM-DD'),
                            DateSale: moment(tabs[9], 'MM/DD/YY').format('YYYY-MM-DD'),
                            Volume: parseFloat(tabs[10].replace('$', '0').replace(/,/g, '')),
                            DateExpiration: moment(tabs[9], 'MM/DD/YY').add(547, 'days').format('YYYY-MM-DD'),
                            AssignedToUserID: '',
                            AgentsList: self.AgentsList,
                            _cellVariants: {
                                DateExpiration: (moment(tabs[9], 'MM/DD/YY').add(547, 'days').isAfter(moment()) ? '' : 'danger')
                            }
                        });
                    }
                });
                $('.import-1').slideUp('fast');
                $('.import-2').removeClass('d-none').slideDown('fast');
            }
        },
        getLists: function () {
            this.AgentsList = window.AgentsList;
            this.BookingStatusList = window.BookingStatusList;
        },
        proceedToImport: function () {
            let self = this;
            let unassignedLeads = this.ImportCollection.filter(function (item) {
                return item.AssignedToUserID == '';
            });
            if (unassignedLeads.length > 0) {
                //leads pendientes de asignar
                $.confirm({
                    title: 'Please Confirm',
                    content: 'There are ' + unassignedLeads.length + ' unassigned leads. Click "Confirm" and import only the assigned leads or "Cancel" to complete the pending assignations.',
                    buttons: {
                        confirm: function () {
                            //proceed to import
                            self.saveImport(false);
                        },
                        cancel: function () {
                        },
                        importUnassigned: {
                            text: 'Include Unassigned Trials',
                            btnClass: 'btn-default',
                            action: function () {
                                self.saveImport(true);
                            }
                        }
                    }
                });
            } else {
                //guardar asignaciones
                self.saveImport(false);
            }
        },
        saveImport: function (unassigned) {
            let self = this;
            $.ajax({
                url: '/crm/Trials/SaveImport',
                cache: false,
                type: 'POST',
                data: {
                    trials: $.toJSON(self.ImportCollection),
                    unassigned: unassigned
                },
                success: function (data) {
                    if (data.ResponseType == 1) {
                        $.alert({
                            title: 'Trials Succesfully Imported',
                            content: 'Trials were succesfully imported.',
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'green'
                        });
                        self.showTrialsImport = false;
                    } else {
                        $.alert({
                            title: 'Error Importing',
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
        hideTrialInfo: function () {
            this.showTrialInfo = false;
        },
        trialSaved: function (data) {
            if (data.ResponseType == 1) {
                $.alert({
                    title: 'Trial Succesfully Saved',
                    content: 'Changes were succesfully saved.',
                    animation: 'zoom',
                    closeAnimation: 'scale',
                    autoClose: 'ok|3000',
                    type: 'green'
                });
                self.showTrialsImport = false;
            } else {
                $.alert({
                    title: 'Error trying to save',
                    content: data.ResponseMessage,
                    animation: 'zoom',
                    closeAnimation: 'scale',
                    autoClose: 'ok|3000',
                    type: 'red'
                });
            }
        },
        showContactInfo: function (value, title) {
            if (value) {
                let content = '<span class="d-block text-center h4 font-weight-light">' + value + '</span><br /><p><small>You are informed that the information displayed on this page is protected by the federal law of protection of personal data in possession of individuals, by accessing this information constitutes your express acceptance as responsible for the treatment of personal data, we ask you not make misuse of them, since you could be punished for crimes in the matter of misrepresentation of personal data.</small></p>';
                $.alert({
                    title: title,
                    content: content,
                    animation: 'zoom',
                    closeAnimation: 'scale',
                    autoClose: 'ok|20000',
                    type: 'orange'
                });
            }
        },
        saveTrial: function () {
            let self = this;
            $.ajax({
                url: '/crm/Trials/SaveTrial',
                cache: false,
                type: 'POST',
                data: self.Trial,
                success: function (data) {
                    self.trialSaved(data);
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
        updateBookingStatus: function (bookingStatusID, bookingStatus) {
            this.Trial.BookingStatusID = bookingStatusID;
            this.Trial.BookingStatus = bookingStatus;
        }
    },
    watch: {
        AssignedToUser: function (newVal, oldVar) {
            let agent = this.AgentsList.filter(function (agent) {
                return agent.Value == newVal;
            });
            if (agent.length == 1) {
                this.Trial.AssignedTo = agent[0].Text;
            }
        }
    },
    mounted: function () {
        //iniciar la sesión
        this.Session().getSessionDetails();

        //obtener listas
        this.getLists();

        //iniciar selectores múltiples
        $('[multiple="multiple"]').multiselect({
            buttonWidth: '100%',
            includeSelectAllOption: true
        });

        //abrir búsqueda
        this.UI().showSearchCard();
    }
});