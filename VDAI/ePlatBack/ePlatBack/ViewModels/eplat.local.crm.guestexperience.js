var GEM = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data: {
        Shared: ePlatStore,
        SearchFilters: {
            Search_FrontOfficeResortID: 13,
            Search_FromDate: '',
            Search_ToDate: '',
            Search_DateRange: 'today',
            Search_Status: ''
        },
        Cards: {
            Overview: false,
            GuestProfile: false
        },
        Overview: {},
        Arrival: {
            GuestProfile: {
                CustomerInfo: {
                    Hometown: 'State, Country',
                    Phone: '000 000 0000',
                    Email: 'profile@email.com',
                    Picture: '/images/avatar.png'
                },
                Reservations: []
            }
        },
        Arrivals: [],
        FilteredArrivals: [],
        ArrivalsDataTable: {
            fields: [
                { key: 'CheckIn', sortable: true },
                { key: 'CheckOut', sortable: true },
                { key: 'Guest', sortable: true },
                { key: 'RoomType', sortable: true },
                { key: 'RoomNumber', sortable: true },
                { key: 'Guests' },
                { key: 'Agency', sortable: true },

                { key: 'Recurring', sortable: true },
                { key: 'Member', sortable: true },
                { key: 'Survey', sortable: true },
                { key: 'Prearrival', sortable: true },
                { key: 'Complaints', sortable: true }
            ],
            sortBy: 'Guest',
            sortDesc: false,
            perPage: 10,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: true,
            filter: null,
        },
        Preferences: [],
        ArrivalsTableFilters: []
    },
    methods: {
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
        startSearch: function () {
            this.SearchFilters.Search_Status = 'Searching Arrivals';
            this.Arrivals = [];
            this.FilteredArrivals = [];
            this.Cards.GuestProfile = false;
            this.Cards.Overview = false;
        },
        setDateRange: function (range) {
            this.SearchFilters.Search_DateRange = range;
            switch (range) {
                case 'today':
                    this.SearchFilters.Search_FromDate = moment().format('YYYY-MM-DD');
                    this.SearchFilters.Search_ToDate = moment().format('YYYY-MM-DD');
                    break;
                case 'tomorrow':
                    this.SearchFilters.Search_FromDate = moment().add(1, 'days').format('YYYY-MM-DD');
                    this.SearchFilters.Search_ToDate = moment().add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 'week':
                    this.SearchFilters.Search_FromDate = moment().startOf('isoWeek').format('YYYY-MM-DD');
                    this.SearchFilters.Search_ToDate = moment().endOf('isoWeek').format('YYYY-MM-DD');
                    break;
                case 'month':
                    this.SearchFilters.Search_FromDate = moment().format('YYYY-MM-01');
                    this.SearchFilters.Search_ToDate = moment().add(1, 'months').date(1).subtract(1, 'days').format('YYYY-MM-DD');
                    break;
            }
            let self = this;
            Vue.nextTick(function () {
                self.getArrivals();
            });
        },
        setSearchResults: function (data) {
            this.Arrivals = data.Arrivals;
            this.UIData.showSearchCard = false;
            this.ArrivalsTableFilters = [];
            this.applyFilters();
            this.Overview = data.Overview;
            this.Cards.Overview = true;
            if (data.Arrivals.length === 0) {
                this.SearchFilters.Search_Status = 'Search didn\'t get results.';
            }
        },
        getArrivals: function () {
            $('#frmGEMSearch').submit();
        },
        openArrival: function (row, i) {
            this.Arrival = row;
            this.Cards.GuestProfile = true;
            $('#tblArrivals tr').removeClass('selected');
            $('#tblArrivals tr[aria-rowindex="' + (i + 1) + '"]').addClass('selected');
            let selft = this;
            Vue.nextTick(function () {
                self.UI().scrollTo('divGuestProfile');
            });
        },
        getResortPreferences: function () {
            let self = this;
            $.ajax({
                url: '/GuestExperience/GetPreferencesForResort/' + self.SearchFilters.Search_FrontOfficeResortID,
                cache: false,
                type: 'GET',
                success: function (data) {
                    self.Preferences = data;
                    Vue.nextTick(function () {
                        // DOM updated
                        $('#Search_Preferences').multiselect('rebuild');
                    });
                },
                error: function (xhr, status, error) {
                    $.alert({
                        title: 'Error',
                        content: 'Error trying to get the Preferences for current Resort.',
                        animation: 'zoom',
                        closeAnimation: 'scale',
                        autoClose: 'ok|3000',
                        type: 'red'
                    });
                }
            });
        },
        addFilter: function (filter) {
            this.ArrivalsDataTable.filter = null;
            if (filter && filter.indexOf(',') >= 0) {
                if (!this.ArrivalsTableFilters.includes(filter)) {
                    this.ArrivalsTableFilters.push(filter);
                }
            } else if (filter && !this.ArrivalsTableFilters.includes(filter) && filter !== "All Arrivals") {
                this.ArrivalsTableFilters.push(filter);
            }
            if (filter === "All Arrivals") {
                this.ArrivalsTableFilters = [];
            }

            this.applyFilters();
        },
        removeFilter: function (filter) {
            let index = this.ArrivalsTableFilters.indexOf(filter);
            this.ArrivalsTableFilters.splice(index, 1);
            this.applyFilters();
        },
        applyFilters: function () {
            let self = this;
            self.FilteredArrivals = self.Arrivals;
            self.ArrivalsTableFilters.forEach(function (f) {
                if (f === 'Recurring Clients') {
                    self.FilteredArrivals = self.FilteredArrivals.filter(a => a.Recurring === true);
                }
                if (f === 'Members') {
                    self.FilteredArrivals = self.FilteredArrivals.filter(a => a.Member === true);
                }
                if (f === 'Prearrival ResortCom') {
                    self.FilteredArrivals = self.FilteredArrivals.filter(a => a.GuestProfile.PrearrivalInfo.PrearrivalTerminalID === 10);
                }
                if (f === 'Prearrival Tafer') {
                    self.FilteredArrivals = self.FilteredArrivals.filter(a => a.GuestProfile.PrearrivalInfo.PrearrivalTerminalID === 16);
                }
                if (f === 'Had Problems') {
                    self.FilteredArrivals = self.FilteredArrivals.filter(a => a.SurveyProblem === true);
                }
                if (f === 'With Complaints') {
                    self.FilteredArrivals = self.FilteredArrivals.filter(a => a.Complaints === true);
                }
                if (f === 'With Current Tickets') {
                    self.FilteredArrivals = self.FilteredArrivals.filter(a => a.CurrentTickets === true);
                }
                if (f === 'With Surveys') {
                    self.FilteredArrivals = self.FilteredArrivals.filter(a => a.Survey === true);
                }
                if (f === 'With Preferences') {
                    self.FilteredArrivals = self.FilteredArrivals.filter(a => a.Preferences === true);
                }
                if (f.indexOf(',') >= 0) {
                    //preferencia
                    prefArr = f.split(',');
                    let temporalArrivalsList = [];
                    self.FilteredArrivals.forEach(function (arrival) {
                        arrival.GuestProfile.Preferences.forEach(function (preferenceType) {
                            if (preferenceType.PreferenceTypeID == prefArr[0]) {
                                preferenceType.Preferences.forEach(function (preference) {
                                    if (preference.PreferenceID == prefArr[1]) {
                                        temporalArrivalsList.push(arrival);
                                    }
                                });
                            }                            
                        });
                    });
                    self.FilteredArrivals = temporalArrivalsList;
                }
            });
        }
    },
    filters: {
        removeIDs: function (value) {
            if (!value) return '';
            value = value.toString();
            return value.substr(value.lastIndexOf(',') + 1);
        }
    },
    mounted: function () {
        let self = this;
        //iniciar la sesión
        self.Session().getSessionDetails();

        //obtener listas
        self.getResortPreferences();

        //iniciar datepickers
        $('.datetimepicker-input').datetimepicker({
            format: 'YYYY-MM-DD'
        }).on('datetimepicker.hide', function () {
            this.dispatchEvent(new Event('input'));
        });

        self.SearchFilters.Search_FromDate = moment().format('YYYY-MM-DD');
        self.SearchFilters.Search_ToDate = moment().format('YYYY-MM-DD');

        //iniciar selectores múltiples
        $('[multiple="multiple"]').multiselect({
            buttonWidth: '100%',
            includeSelectAllOption: true
        });

        Vue.nextTick(function () {
            self.getArrivals();
        });
    }
});