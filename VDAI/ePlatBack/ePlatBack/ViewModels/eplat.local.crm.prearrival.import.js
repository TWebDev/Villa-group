$(function () {
    IMPORT.init();
});

var IMPORT = function () {

    var init = function () {

    }

    var searchToImport = function (data) {
        vArrival.Arrivals = data;
        if (vArrival.Arrivals.length > 0 && vArrival.Arrivals[0].Message == 'New RoomTypes Created') {
            vArrival.clearTables();
            $.alert({
                title: 'New Room Types Created',
                content: 'Please repeat your search in order to update the reservations pending to import.',
                animation: 'zoom',
                closeAnimation: 'scale',
                autoClose: 'ok|5000',
                type: 'orange'
            });
        }
    }

    var applyFilters = function () {
        vArrival.filterByLeadSource();
        vArrival.filterByStatus();
        vArrival.filteredArrivals();
    }

    var clearTable = function (item) {
        $('#' + item + ' tbody').empty();
    }

    return {
        init: init,
        searchToImport: searchToImport,
        clearTable: clearTable,
        applyFilters: applyFilters
    }
}();

var vArrival = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data:
        function () {
            return {
                Shared: ePlatStore,
                Arrivals: [],
                Search_I_ImportArrivalDate: '',
                Search_F_ImportArrivalDate: '',
                CSources: null,
                CUsers: null,
                source: null,
                status: null,
                showImported: false,
                user: null,
                assignment: [],
                XHRArrivals: null
            }
        },
    methods: {
        clearTables: function () {
            let self = this;
            self.Arrivals = [];
        },
        importArrivals: function () {
            let self = this;
            $.confirm({
                title: 'Import Arrivals',
                content: 'Arrivals without assignment will be assigned to You',
                buttons: {
                    confirm: {
                        btnClass: 'btn-primary',
                        action: function () {
                            $.ajax({
                                url: '/crm/PreArrival/ImportArrivals',
                                type: 'POST',
                                cache: false,
                                //data: { __data: JSON.stringify(self.Arrivals) },
                                data: { __data: JSON.stringify(self.filteredArrivals()) },
                                success: function (data) {
                                    //self.Arrivals = $.parseJSON(data.ItemID.arrivals);
                                    var _arrivals = $.parseJSON(data.ItemID.arrivals);
                                    _.forEach(_arrivals, function (o) {
                                        var item = _.find(self.Arrivals, function (i) { return o.Index == i.Index });
                                        item.Status = o.Status;
                                        item.Message = o.Message;
                                    });

                                    self.Arrivals[0].LeadSources = self.CSources;
                                    self.Arrivals[0].Users = self.CUsers;
                                    $.alert({
                                        title: 'Import Finished',
                                        content: data.ResponseMessage + '<br />' + data.ExceptionMessage,
                                        animation: 'zoom',
                                        closeAnimation: 'scale',
                                        autoClose: 'ok|5000',
                                        type: data.ResponseType == 1 ? 'green' : 'red'
                                    });
                                }
                            });
                        }
                    },
                    cancel: function () {

                    }
                }
            });
        },
        importRow: function (row, key) {
            let self = this;
            $.confirm({
                title: 'Import Arrival',
                content: 'If arrival is not assigned, It will be assigned to you. Do you want to proceed?',
                buttons: {
                    confirm: {
                        btnClass: 'btn-primary',
                        action: function () {
                            $.ajax({
                                url: '/crm/PreArrival/ImportArrivals',
                                type: 'POST',
                                cache: false,
                                data: { __data: JSON.stringify(row) },
                                success: function (data) {
                                    var arrival = $.parseJSON(data.ItemID.arrivals);
                                    self.Arrivals[key].Status = arrival[0].Status;
                                    $.alert({
                                        title: 'Import Finished',
                                        content: data.ResponseMessage + '<br>' + data.ExceptionMessage,
                                        animation: 'zoom',
                                        closeAnimation: 'scale',
                                        autoClose: 'ok|5000',
                                        type: data.ResponseType == 1 ? 'green' : 'red'
                                    });
                                }
                            });
                        }
                    },
                    cancel: function () {

                    }
                }
            });
        },
        filterByLeadSource: function () {
            let self = this;
            //self.filteredArrivals();
            _.forEach(self.Arrivals, function (o) {
                if (self.source != '') {
                    if (o.LeadSource == self.source) {
                        o.Import = true;
                    }
                    else {
                        o.Import = false;
                    }
                }
                else {
                    o.Import = true;
                }
            });
            return true;
        },
        filterByStatus: function () {
            let self = this;
            _.forEach(self.Arrivals, function (o) {
                if (self.status != null) {
                    if (self.status == true) {
                        if (o.Status == true) {
                            o.Import = true;
                        }
                        else {
                            o.Import = false;
                        }
                    }
                    else {
                        if (o.Status != true) {
                            o.Import = true;
                        }
                        else {
                            o.Import = false;
                        }
                    }
                }
                else {
                    o.Import = true;
                }
            });
            return true;
        },
        filteredArrivals: function () {
            //let self = this;
            //_.forEach(self.Arrivals, function (o) {
            //    if (self.source != '') {
            //        if (o.LeadSource == self.source) {
            //            o.Import = true;
            //        }
            //        else {
            //            o.Import = false;
            //        }
            //    }
            //    else {
            //        o.Import = true;
            //    }
            //});

            //self.filterByLeadSource();
            //self.filterByStatus();
            return _.filter(self.Arrivals, function (o) { return o.Import == true; });
        },
        allSameUser: function () {
            let self = this;
            for (var i = 0; i < self.Arrivals.length; i++) {
                self.Arrivals[i].AssignedToUserID = self.user;
            }
        },
        getNextUser: function (users) {
            var lowest = Number.POSITIVE_INFINITY;
            var tmp;
            var user;
            for (var i = users.length - 1; i >= 0; i--) {
                tmp = users[i].count;
                if (tmp < lowest) {
                    lowest = tmp;
                    user = users[i].user;
                }
            }
            return user;
        },
        orderArrivals: function (byColumn) {
            
            let self = this;
            var a = _.filter(self.Arrivals, function (o) { return o.Import == true; });
            if (localStorage.getItem('ePlat_orderCriteria') == 'asc') {
                localStorage.setItem('ePlat_orderCriteria', 'desc');
                self.Arrivals = _.orderBy(self.Arrivals, [byColumn], ['desc']);
            }
            else {
                localStorage.setItem('ePlat_orderCriteria', 'asc');
                self.Arrivals = _.orderBy(self.Arrivals, [byColumn], ['asc']);
            }
        },
        _orderArrivals: function (byColumn) {
            console.log(byColumn);
            let self = this;
            var a = _.filter(self.Arrivals, function (o) { return o.Import == true; });
            if (a[0].Status == true) {
                //self.Arrivals = _.orderBy(self.Arrivals, ['Status'], ['desc']);
                self.Arrivals = _.orderBy(self.Arrivals, [byColumn], ['desc']);
            }
            else {
                //self.Arrivals = _.orderBy(self.Arrivals, ['Status'], ['asc']);
                self.Arrivals = _.orderBy(self.Arrivals, [byColumn], ['asc']);
            }
        },
        automaticAssignation: function () {
            let self = this;
            var rules = self.Arrivals[0].ListAssignation;//Key:leadSourceID, Value:userID
            var distinctSources = _.uniqBy(rules, 'Key');

            for (var i = 0; i < distinctSources.length; i++) {
                var users = _.filter(rules, function (o) { return o.Key == distinctSources[i].Key; });
                var coincidences = _.filter(self.Arrivals, function (o) { return o.LeadSource == distinctSources[i].Key && o.Status != true; });
                var assignment = [];

                for (var a = 0; a < users.length; a++) {
                    var item = { user: users[a].Value, count: 0 };
                    assignment.push(item);
                }

                for (var a = 0; a < coincidences.length; a++) {
                    var nextUser = self.getNextUser(assignment);
                    _.forEach(_.filter(self.Arrivals, ['idReservacion', coincidences[a].idReservacion]), function (o) {
                        o.AssignedToUserID = nextUser;
                    });
                    _.find(assignment, ['user', nextUser]).count++;
                }
            }
        },
        preselectSource: function (evt, row) {
            let self = this;
            _.forEach(_.filter(self.Arrivals, function (o) { return o.Procedencia == row.Procedencia; }), function (o) {
                o.LeadSource = row.LeadSource;
            });
        }
    },
    computed: {
        NonImportable: function () {
            return this.Arrivals.length > 0 ? _.filter(this.Arrivals, function (o) { return o.LeadSource == null && o.Import == true; }).length : 1;
        },
        ImportedArrivals: function () {
            return this.Arrivals.length > 0 ? _.filter(this.Arrivals, function (o) { return o.Status == true; }).length : 0;
        },
        PendingToImport: function () {
            return this.Arrivals.length > 0 ? _.filter(this.Arrivals, function (o) { return o.Status != true; }).length : 0;
        },
        ListSources: function () {
            this.CSources = this.Arrivals.length > 0 ? this.Arrivals.filter(m => m.LeadSources != null)[0].LeadSources : null;
            return this.Arrivals.length > 0 ? this.Arrivals.filter(m => m.LeadSources != null)[0].LeadSources : null;
        },
        Statuses: function () {
            return [{ Value: null, Text: '--None--' }, { Value: true, Text: "Imported" }, { Value: false, Text: "Not Imported" }];
        },
        Users: function () {
            this.CUsers = this.Arrivals.length > 0 ? this.Arrivals.filter(m => m.Users != null)[0].Users : null;
            return this.Arrivals.length > 0 ? this.Arrivals.filter(m => m.Users != null)[0].Users : null;
        },
        Assigned: function () {
            let self = this;

            var assigned = _.filter(self.Arrivals, function (o) { return o.AssignedToUserID != null && o.LeadSource != null && o.Status != true; });
            var distinct = _.uniqBy(assigned, 'AssignedToUserID');

            var _response = [];

            for (var a = 0; a < distinct.length; a++) {
                var response = {};
                response.User = _.filter(self.Users, function (o) { return o.Value == distinct[a].AssignedToUserID; })[0].Text;
                response.Data = [];
                var _assigned = _.filter(assigned, function (o) { return o.AssignedToUserID == distinct[a].AssignedToUserID; });
                var distinctSources = _.uniqBy(_assigned, 'LeadSource');

                for (var i = 0; i < distinctSources.length; i++) {
                    var count = _.countBy(_assigned, function (o) { return o.LeadSource == distinctSources[i].LeadSource; });
                    var item = {
                        User: _.filter(self.Users, function (o) { return o.Value == distinct[a].AssignedToUserID; })[0].Text,
                        Source: _.filter(self.ListSources, function (o) { return o.Value == distinctSources[i].LeadSource; })[0].Text,
                        Count: count[true]
                    }
                    response.Data.push(item);
                }
                _response.push(response);
            }

            return _response;
        }
    },
    filters: {
        boolToText: function (value) {
            if (value == 0) return 'No'
            return 'Yes'
        }
    },
    watch: {
        Search_I_ImportArrivalDate: function (nVal, oVal) {
            this.Search_F_ImportArrivalDate = (new Date(this.Search_F_ImportArrivalDate) < new Date(nVal) || this.Search_F_ImportArrivalDate == '' ? nVal : this.Search_F_ImportArrivalDate)
        },
        Search_F_ImportArrivalDate: function (nVal, oVal) {
            this.Search_I_ImportArrivalDate = (new Date(this.Search_I_ImportArrivalDate) > new Date(nVal) || this.Search_I_ImportArrivalDate == '' ? nVal : this.Search_I_ImportArrivalDate)
        }
    },
    mounted: function () {
        let self = this;
        this.Session().getSessionDetails();
        $('[multiple="multiple"]').multiselect({
            buttonWidth: '100%',
            includeSelectAllOption: true,
        });

        $('.date-picker').datetimepicker({
            format: 'YYYY-MM-DD'
        }).on('datetimepicker.hide', function () {
            this.dispatchEvent(new Event('input'));
        });

        $('.date-picker').on('blur', function () {
            $(this).datetimepicker('hide');
            this.dispatchEvent(new Event('input'));
        });
    }
})