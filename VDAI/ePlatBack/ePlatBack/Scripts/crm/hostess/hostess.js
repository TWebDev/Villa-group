var Hostess = function () {
    var ArrivalsList, Summary, Prearrivals, PowerLine, IsPast, CustomerHistory;
    var tblArrivals, tblBinnacle, tblManifest, tblPrearrivals;
    var xhrArrivals, xhrSummaries;

    var init = function () {
        UI.collapseFieldset('fdsArrivalInfo');

        google.charts.load("current", { packages: ["corechart"] });

        getTodayDate();

        getDependantFields();

        $('.search-date').on('click', function () {
            $('#searchByDate').slideDown('fast');
            $('#searchByName').slideUp('fast');
        });

        $('#txtGuestName').on('keyup', function (e) {
            if (e.keyCode === 13) {
                $('#btnGetArrivalsByName').trigger('click');
            }
        });

        $('.search-name').on('click', function () {
            $('#searchByDate').slideUp('fast');
            $('#searchByName').slideDown('fast');
        });

        $('#btnGetArrivals').on('click', function () {
            Hostess.getArrivalsInfo(false);
        });

        $('#btnGetArrivalsByName').on('click', function () {
            Hostess.getArrivalsByName();
        });

        $('#btnSearchSPIHistory').off('click').on('click', function () {
            Hostess.CustomerHistory = [];
            if ($('#SPIRelatedCustomerID').val() != null) {
                dataObject = {
                    spiCustomerID: $('#SPIRelatedCustomerID').val()
                };
                $.getJSON('/SPI/GetCustomerHistory', dataObject, function (data) {
                    Hostess.CustomerHistory = data;
                    let tbody = '';
                    $.each(Hostess.CustomerHistory.History, function (i, tour) {
                        tbody += '<tr id="th' + i + '"><td>' + tour.TourDate + '</td><td>' + tour.SalesCenter + '</td><td>' + tour.TourContractNumber + '</td><td>' + (tour.Volume != null ? '$' + tour.Volume : '') + '</td></tr>';
                    });
                    $('#tblToursHistory tbody').html(tbody);
                    $('#tblToursHistory tbody tr').off('click').on('click', function () {
                        let index = $(this).attr('id').replace('th', '');
                        $('#TourDate').val(Hostess.CustomerHistory.History[index].TourDate);
                        $('#SalesCenter').val(Hostess.CustomerHistory.History[index].SalesCenter);
                        $('#TourSource').val(Hostess.CustomerHistory.History[index].TourSource);
                        $('#SourceGroup').val(Hostess.CustomerHistory.History[index].SourceGroup);
                        $('#SourceItem').val(Hostess.CustomerHistory.History[index].SourceItem);
                        $('#Qualification').val(Hostess.CustomerHistory.History[index].Qualification);
                        $('#TourContractNumber').val(Hostess.CustomerHistory.History[index].TourContractNumber);
                        $('#Volume').val((Hostess.CustomerHistory.History[index].Volume != null ? '$' + Hostess.CustomerHistory.History[index].Volume : ''));
                        let tbody = '';
                        $.each(Hostess.CustomerHistory.History[index].LegalNames, function (l, legal) {
                            tbody += '<tr><td>' + legal.Name + '</td><td>' + legal.DateOfBirth + '</td><td>' + legal.Age + ' years</td></tr>';
                        });
                        $('#tblLegalNames tbody').html(tbody);
                    });
                    if ($('#tblToursHistory tbody tr').length == 1) {
                        $('#tblToursHistory tbody tr').eq(0).trigger('click');
                    }
                });
            }
        });

        $('#btnCleanDuplicates').off('click').on('click', function () {
            $.ajax({
                url: '/crm/Hostess/CleanDuplicates',
                cache: false,
                data: {
                    date: $('#txtArrivalsDate').val()
                },
                success: function (data) {
                    UI.messageBox(data.ResponseType, data.ResponseMessage);
                }
            });
        });

        $('#tblPreferences input[type="checkbox"]').off('click').on('click', function () {
            let preferences = [];
            $.each($('#tblPreferences input[type="checkbox"]:checked'), function () {
                preferences.push($(this).attr('data-preferenceid'));
            });
            $('#Preferences').val(preferences.join(','));
        });

        $('#hostessTabs').tabs();
        $('#divTabs').tabs();
        $('#guestTabs').tabs();
        $('#btnCloseArrivalInfo').on('click', function () {
            UI.collapseFieldset('fdsArrivalInfo');
        });
        $('#txtArrivalsDate').datepicker({
            constrainInput: true,
            dateFormat: 'yy-mm-dd'
        });
        $('#PresentationDate').datepicker({
            constrainInput: true,
            dateFormat: 'yy-mm-dd'
        }).on('change', function () {
            if ($('#PresentationDate').val() != "") {
                var dataObject = {
                    date: $('#PresentationDate').val(),
                    programID: $('#ProgramID').val()
                }
                $.getJSON('/crm/hostess/GetParties', dataObject, function (data) {
                    $('#SalesRoomPartyID').fillSelect(data);
                    $('#SalesRoomPartyID').trigger('loaded');
                });
            }
        });
        $('#SearchForecast_I_ArrivalDate').datepicker({
            constrainInput: true,
            dateFormat: 'yy-mm-dd'
        });
        $('#SearchForecast_F_ArrivalDate').datepicker({
            constrainInput: true,
            dateFormat: 'yy-mm-dd'
        });
        $('#SearchPenetration_I_ArrivalDate').datepicker({
            constrainInput: true,
            dateFormat: 'yy-mm-dd'
        });
        $('#SearchPenetration_F_ArrivalDate').datepicker({
            constrainInput: true,
            dateFormat: 'yy-mm-dd'
        });
        $('#SearchGlobalPenetration_I_ArrivalDate').datepicker({
            constrainInput: true,
            dateFormat: 'yy-mm-dd'
        });
        $('#SearchGlobalPenetration_F_ArrivalDate').datepicker({
            constrainInput: true,
            dateFormat: 'yy-mm-dd'
        });

        $('#SearchInArrivals_ProgramID').off('change').on('change', formatList);
        $('#SearchInArrivals_ReservationStatus').off('change').on('change', formatList);
        $('#SearchInArrivals_BookingStatusID').off('change').on('change', formatList);
        $('#SearchInBinnacle_ProgramID').off('change').on('change', formatList);

        $('#SearchForecast_I_ArrivalDate').off('change').on('change', function () {
            $('#SearchForecast_F_ArrivalDate').val($('#SearchForecast_I_ArrivalDate').val());
        });
        $('#SearchPenetration_I_ArrivalDate').off('change').on('change', function () {
            $('#SearchPenetration_F_ArrivalDate').val($('#SearchPenetration_I_ArrivalDate').val());
        });

        $('select[multiple="multiple"]').multiselect({
            noneSelectedText: "--All--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();

        Hostess.tblArrivals = $('#tblArrivals').dataTable({
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            },
            "aoColumns": [
                { "mData": "ArrivalDate" },
                { "mData": "Program" },
                { "mData": "ReservationStatus" },
                { "mData": "Nights" },
                { "mData": "Guest" },
                { "mData": "RoomNumber" },
                { "mData": "CountryCode" },
                { "mData": "AgencyCode" },
                { "mData": "MarketCode" },
                { "mData": "ConfirmationNumber" },
                { "mData": "Crs" },
                { "mData": "OPCName" },
                { "mData": "BookingStatus" },
                { "mData": "HostessQualificationStatus" },
                { "mData": "PreCheckIn" },
                { "mData": "Survey" }
            ]
        });
        Hostess.tblBinnacle = $('#tblBinnacle').dataTable({
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            },
            "aoColumns": [
                { "mData": "Program" },
                { "mData": "PresentationDate" },
                { "mData": "RoomNumber" },
                { "mData": "Guest" },
                { "mData": "PresentationPax" },
                { "mData": "Country" },
                { "mData": "HostessQualificationStatus" },
                { "mData": "OPCName" },
                { "mData": "SalesStatus" },
                { "mData": "SalesVolume" }
            ]
        });
        Hostess.tblManifest = $('#tblManifest').dataTable({
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            },
            "aoColumns": [
                { "mData": "PresentationDate" },
                { "mData": "Party" },
                { "mData": "Program" },
                { "mData": "OPCName" },
                { "mData": "LastName" },
                { "mData": "FirstName" },
                { "mData": "RoomNumber" },
                { "mDataProp": "Deposit", "sClass": "format-currency" },
                { "mData": "HostessQualificationStatus" },
                { "mData": "Nationality" },
                { "mData": "PresentationPax" },
                { "mData": "InvitationNumber" },
                { "mData": "VipCardStatus" }
            ],
            "aoRowCallback": [UI.applyFormat('currency', 'tblManifest')]
        });
        Hostess.tblPrearrivals = $('#tblPrearrivals').dataTable({
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            },
            "aoColumns": [
                { "mData": "ArrivalDate" },
                { "mData": "HotelConfirmationNumber" },
                { "mData": "Guest" },
                { "mData": "ClubType" },
                { "mData": "AccountNumber" },
                { "mData": "ContractNumber" },
                { "mData": "CoOwner" },
                { "mData": "PreArrivalOptionsTotal" },
                { "mData": "BookingStatus" },
                { "mData": "HostessQualificationStatus" }
            ],
            "aoRowCallback": [UI.applyFormat('currency', 'tblManifest')]
        });
    }

    var getArrivalsInfo = function (avoidRequestToFront) {
        if (Hostess.xhrArrivals && Hostess.xhrArrivals.readyState != 4) {
            Hostess.xhrArrivals.abort();
        }
        Hostess.xhrArrivals = $.ajax({
            url: '/crm/Hostess/GetArrivals',
            cache: false,
            data: {
                date: $('#txtArrivalsDate').val(),
                avoidRequestToFront: avoidRequestToFront
            },
            success: function (data) {
                Hostess.ArrivalsList = data.Arrivals;
                Hostess.Summary = data.Summary;
                Hostess.IsPast = data.IsPast;
                Hostess.Prearrivals = data.Prearrivals;
                Hostess.PowerLine = data.PowerLine;
                $('#btnCloseArrivalInfo').trigger('click');

                //get available prearrival status
                $('#SearchInPrearrivals_BookingStatusID').html('');
                $.each(Hostess.Prearrivals, function (p, pa) {
                    if ($('#SearchInPrearrivals_BookingStatusID option[value="' + pa.BookingStatusID + '"]').length == 0) {
                        //add option
                        $('#SearchInPrearrivals_BookingStatusID').append('<option value="' + pa.BookingStatusID + '">' + pa.BookingStatus + '</option>');
                    }
                });
                $('#SearchInPrearrivals_BookingStatusID').off('change').on('change', formatList);

                formatList();
            }
        });
    }

    var getArrivalsByName = function () {
        if (Hostess.xhrArrivals && Hostess.xhrArrivals.readyState != 4) {
            Hostess.xhrArrivals.abort();
        }
        let data = {
            date: null,
            name: $('#txtGuestName').val(),
            avoidRequestToFront: true
        }
        Hostess.xhrArrivals = $.post('/crm/Hostess/GetArrivals', data, function (data) {
            Hostess.ArrivalsList = data.Arrivals;
            Hostess.Summary = data.Summary;
            Hostess.IsPast = data.IsPast;
            Hostess.Prearrivals = data.Prearrivals;
            Hostess.PowerLine = data.PowerLine;
            $('#btnCloseArrivalInfo').trigger('click');
            formatList();
        }, 'json');
    }

    var updated = function (data) {
        if (data.ResponseType == 1) {
            UI.messageBox(data.ResponseType, "Arrival Information Successfully Saved!");
            UI.collapseFieldset('fdsArrivalInfo');
            //add data.ArrivalInfo to Hostess.ArrivalsList
            var updatedArrivals = [];
            $.each(Hostess.ArrivalsList, function (a, arr) {
                if (arr.ArrivalID == data.ArrivalInfo.ArrivalID) {
                    updatedArrivals.push(data.ArrivalInfo);
                } else {
                    updatedArrivals.push(arr);
                }
            });
            Hostess.ArrivalsList = updatedArrivals;
            formatList();
            Hostess.getArrivalsInfo(true);
        } else {
            UI.messageBox(data.ResponseType, data.ResponseMessage);
        }
    }

    function getDependantFields() {
        $.ajax({
            url: '/crm/Hostess/GetDependantFields',
            cache: false,
            success: function (data) {
                UI.loadDependantFields(data);
            }
        });
    }

    function formatList() {
        //format Arrivals from Front
        Hostess.tblArrivals.fnClearTable();
        var arrivalsRows = [];
        $.each(Hostess.ArrivalsList, function (a, arrival) {
            if (($('#SearchInArrivals_ProgramID').val() == "0"
                            || (arrival.ProgramID != null && $('#SearchInArrivals_ProgramID').val() == arrival.ProgramID.toString()))
                            &&
                            ($('#SearchInArrivals_ReservationStatus').val() == ""
                            || $('#SearchInArrivals_ReservationStatus').val() == arrival.ReservationStatus)
                            &&
                            ($('#SearchInArrivals_BookingStatusID').val() == "0"
|| ($('#SearchInArrivals_BookingStatusID').val() == "10" && arrival.BookingStatusID == null)
                            || (arrival.BookingStatusID != null && $('#SearchInArrivals_BookingStatusID').val() == arrival.BookingStatusID.toString()))) {
                arrivalsRows.push({
                    "DT_RowId": 'arr-' + arrival.ArrivalID,
                    "ArrivalDate": arrival.ArrivalDate,
                    "Program": arrival.Program,
                    "ReservationStatus": arrival.ReservationStatus,
                    "Nights": arrival.Nights,
                    "Guest": arrival.Guest,
                    "RoomNumber": arrival.RoomNumber,
                    "CountryCode": arrival.CountryCode,
                    "AgencyCode": arrival.AgencyCode,
                    "MarketCode": arrival.MarketCode,
                    "ConfirmationNumber": arrival.ConfirmationNumber,
                    "Crs": arrival.Crs,
                    "OPCName": arrival.OPCName,
                    "BookingStatus": arrival.BookingStatus,
                    "HostessQualificationStatus": arrival.HostessQualificationStatus,
                    "PreCheckIn": arrival.PreCheckIn,
                    "Survey": (arrival.SubmittedDate != null ? 'Yes' : ''),
                    "DT_RowClass": (arrival.ManifestedAsPA == true ? 'pre-booked' : '')
                });
            }
        });
        Hostess.tblArrivals.fnAddData(arrivalsRows);

        //format Binnacle
        Hostess.tblBinnacle.fnClearTable();
        var binnacleRows = [];
        $.each(Hostess.ArrivalsList, function (a, arrival) {
            if ($('#SearchInBinnacle_ProgramID').val() == "0"
                || $('#SearchInBinnacle_ProgramID').val() == arrival.ProgramID.toString()) {
                binnacleRows.push({
                    "DT_RowId": 'bin-' + arrival.ArrivalID,
                    "Program": arrival.Program,
                    "PresentationDate": arrival.PresentationDate,
                    "RoomNumber": arrival.RoomNumber,
                    "Guest": arrival.Guest,
                    "PresentationPax": arrival.PresentationPax,
                    "Country": arrival.Country,
                    "HostessQualificationStatus": arrival.HostessQualificationStatus,
                    "OPCName": arrival.OPCName,
                    "SalesStatus": arrival.SalesStatus,
                    "SalesVolume": arrival.SalesVolume,
                    "DT_RowClass": (arrival.ManifestedAsPA == true ? 'pre-booked' : '')
                });
            }
        });
        Hostess.tblBinnacle.fnAddData(binnacleRows);

        //format Manifest
        Hostess.tblManifest.fnClearTable();
        $('#tblManifestExportable tbody').html('');
        var manifestRows = [];
        $.each(Hostess.ArrivalsList, function (a, arrival) {
            if (arrival.Party != "" && arrival.Program.indexOf("Airport") < 0) {
                manifestRows.push({
                    "DT_RowId": 'man-' + arrival.ArrivalID,
                    "PresentationDate": arrival.PresentationDate,
                    "Party": arrival.Party,
                    "Program": arrival.Program,
                    "OPCName": arrival.OPCName,
                    "LastName": arrival.LastName,
                    "FirstName": arrival.FirstName,
                    "RoomNumber": arrival.RoomNumber,
                    "Deposit": '$' + arrival.Deposit + ' ' + arrival.DepositCurrency,
                    "HostessQualificationStatus": arrival.HostessQualificationStatus,
                    "Nationality": arrival.Nationality,
                    "PresentationPax": arrival.PresentationPax,
                    "InvitationNumber": arrival.InvitationNumber,
                    "VipCardStatus": arrival.VipCardStatus
                });

                let tr = '<tr>'
                tr += '<td>' + arrival.PresentationDate + '</td>';
                tr += '<td>' + arrival.Party + '</td>';
                tr += '<td>' + arrival.Program + '</td>';
                tr += '<td>' + arrival.OPCName + '</td>';
                tr += '<td>' + arrival.LastName + '</td>';
                tr += '<td>' + arrival.FirstName + '</td>';
                tr += '<td>' + arrival.RoomNumber + '</td>';
                tr += '<td>$' + arrival.Deposit + ' ' + arrival.DepositCurrency + '</td>';
                tr += '<td>' + arrival.HostessQualificationStatus + '</td>';
                tr += '<td>' + arrival.Nationality + '</td>';
                tr += '<td>' + arrival.PresentationPax + '</td>';
                tr += '<td>' + arrival.InvitationNumber + '</td>';
                tr += '<td>' + (arrival.VipCardStatus != null ? arrival.VipCardStatus : '') + '</td>';
                tr += '</tr>';
                $('#tblManifestExportable tbody').append(tr);
            }
        });
        Hostess.tblManifest.fnAddData(manifestRows);
        UI.exportToExcel('Manifest');

        //format Prearrivals
        if ($('#tblPrearrivals').length > 0) {
            Hostess.tblPrearrivals.fnClearTable();

            var prearrivalRows = [];
            $.each(Hostess.Prearrivals, function (p, prearrival) {
                if ($('#SearchInPrearrivals_BookingStatusID').val() == prearrival.BookingStatusID.toString()) {
                    prearrivalRows.push({
                        "DT_RowId": 'pre-' + prearrival.ArrivalID,
                        "ArrivalDate": prearrival.ArrivalDateString,
                        "HotelConfirmationNumber": prearrival.HotelConfirmationNumber,
                        "Guest": prearrival.Guest,
                        "ClubType": prearrival.ClubType,
                        "AccountNumber": prearrival.AccountNumber,
                        "ContractNumber": prearrival.ContractNumber,
                        "CoOwner": prearrival.CoOwner,
                        "PreArrivalOptionsTotal": (prearrival.PreArrivalOptionsTotal != null ? "$" + prearrival.PreArrivalOptionsTotal : ""),
                        "BookingStatus": prearrival.BookingStatus,
                        "HostessQualificationStatus": prearrival.HostessQualificationStatus,
                        "DT_RowClass": (prearrival.Located == true ? 'pre-booked' : '')
                    });
                }
            });

            Hostess.tblPrearrivals.fnAddData(prearrivalRows);
        }

        //load Prearrivals in Manifested as Prearrival
        var leadOptions = '<option>No</option>';
        $.each(Hostess.Prearrivals, function (p, prearrival) {
            leadOptions += '<option value="' + prearrival.LeadID + '">' + prearrival.Guest + ' (' + prearrival.HotelConfirmationNumber + ')</option>';
        });
        $('#LeadID').html(leadOptions);

        getSummary();
        getPowerLine();

        bindTablesClicks();
    }

    function getPowerLine() {
        var tbody = '';
        $.each(Hostess.PowerLine, function (t, team) {
            tbody += '<tr>';
            //program
            tbody += '<td rowspan="' + team.Rows + '" class="text-left">';
            tbody += '<span class="right dashboard-total">' + team.Total + '</span>';
            tbody += '<h2 style="display:inline-block; margin-top: 10px;">' + team.Program + '</h2>';
            tbody += '</td>';
            //first opc
            tbody += '<td rowspan="' + team.Promotors[0].Rows + '">';
            tbody += '<h3 style="margin-bottom:0;">' + team.Promotors[0].OPCName + '</h3>';

            tbody += '<div class="table-div">';
            tbody += '<div class="table-row">';
            tbody += '<div class="table-cell">';
            tbody += '<span class="summary-fields">' + team.Promotors[0].Total + '</span><br>';
            tbody += '<span class="summary-note">Assigned Leads</span>';
            tbody += '</div>';
            tbody += '<div class="table-cell">';
            tbody += '<span class="summary-fields">' + team.Promotors[0].BookingPercentage + '%</span><br>';
            tbody += '<span class="summary-note">Penetration Rate</span>';
            tbody += '</div>';
            tbody += '</div>';
            tbody += '</div>';

            tbody += '<table class="table text-center non-editable" style="width:100%;"><tbody><tr>';
            $.each(team.Promotors[0].BSTotals, function (b, bs) {
                tbody += '<td ' + (bs.BookingStatusCode == 'BK' ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
            });
            tbody += '</tr><tr>';
            $.each(team.Promotors[0].BSTotals, function (b, bs) {
                tbody += '<td ' + (bs.BookingStatusCode == 'BK' ? 'class="bs-selected"' : '') + '>' + bs.Amount + '</td>';
            });
            tbody += '</tr><tr>';
            $.each(team.Promotors[0].BSTotals, function (b, bs) {
                tbody += '<td ' + (bs.BookingStatusCode == 'BK' ? 'class="bs-selected"' : '') + '>' + bs.Percentage + '%</td>';
            });
            tbody += '</tr></tbody></table>';
            tbody += '</td>';
            //first qualification
            tbody += '<td rowspan="' + team.Promotors[0].QualificationStatuses[0].Rows + '" class="text-left">';
            tbody += '<span class="right"><strong>' + team.Promotors[0].QualificationStatuses[0].Bookings + ' / ' + team.Promotors[0].QualificationStatuses[0].Total + '</strong>';
            tbody += ' : ' + team.Promotors[0].QualificationStatuses[0].Percentage + '%</span>';
            tbody += '<span class="summary-fields">' + team.Promotors[0].QualificationStatuses[0].Qualification + '</span>';
            tbody += '<table class="table text-center non-editable" style="width:100%;"><tbody><tr>';
            $.each(team.Promotors[0].QualificationStatuses[0].BSTotals, function (b, bs) {
                tbody += '<td>' + bs.BookingStatusCode + '</td>';
            });
            tbody += '</tr><tr>';
            $.each(team.Promotors[0].QualificationStatuses[0].BSTotals, function (b, bs) {
                tbody += '<td>' + bs.Amount + '</td>';
            });
            tbody += '</tr><tr>';
            $.each(team.Promotors[0].QualificationStatuses[0].BSTotals, function (b, bs) {
                tbody += '<td>' + bs.Percentage + '%</td>';
            });
            tbody += '</tr></tbody></table>';
            tbody += '</td>';
            if (team.Promotors[0].QualificationStatuses[0].Guests.length > 0) {
                //first guest
                tbody += '<td colspan="6" class="pwl-guest" id="pwl-' + team.Promotors[0].QualificationStatuses[0].Guests[0].ArrivalID + '">';
                tbody += team.Promotors[0].QualificationStatuses[0].Guests[0].Guest;
                tbody += '</td>';
                tbody += '</tr>';

                //first guest status
                tbody += '<tr>';
                $.each(team.Promotors[0].QualificationStatuses[0].Guests[0].BookingStatus, function (b, bs) {
                    tbody += '<td ' + (bs.Selected == true ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
                });
                tbody += '</tr>';

                //other guests
                $.each(team.Promotors[0].QualificationStatuses[0].Guests, function (g, guest) {
                    if (g > 0) {
                        tbody += '<tr>';
                        tbody += '<td colspan="6" class="pwl-guest" id="pwl-' + guest.ArrivalID + '">';
                        tbody += guest.Guest;
                        tbody += '</td>';
                        tbody += '</tr>';

                        tbody += '<tr>';
                        $.each(guest.BookingStatus, function (b, bs) {
                            tbody += '<td ' + (bs.Selected == true ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
                        });
                        tbody += '</tr>';
                    }
                });
            } else {
                tbody += '<td colspan="6"></td>';
                tbody += '</tr>';
            }

            //other qualifications
            $.each(team.Promotors[0].QualificationStatuses, function (q, qualification) {
                if (q > 0) {
                    tbody += '<tr>';
                    tbody += '<td rowspan="' + qualification.Rows + '" class="text-left">';
                    tbody += '<span class="right"><strong>' + qualification.Bookings + ' / ' + qualification.Total + '</strong>';
                    tbody += ' : ' + qualification.BookingPercentage + '%</span>';
                    tbody += '<span class="summary-fields">' + qualification.Qualification + '</span>';
                    tbody += '<table class="table text-center non-editable" style="width:100%;"><tbody><tr>';
                    $.each(qualification.BSTotals, function (b, bs) {
                        tbody += '<td>' + bs.BookingStatusCode + '</td>';
                    });
                    tbody += '</tr><tr>';
                    $.each(qualification.BSTotals, function (b, bs) {
                        tbody += '<td>' + bs.Amount + '</td>';
                    });
                    tbody += '</tr><tr>';
                    $.each(qualification.BSTotals, function (b, bs) {
                        tbody += '<td>' + bs.Percentage + '%</td>';
                    });
                    tbody += '</tr></tbody></table>';
                    tbody += '</td>';

                    if (qualification.Guests.length > 0) {
                        //first guest
                        tbody += '<td colspan="6" class="pwl-guest" id="pwl-' + qualification.Guests[0].ArrivalID + '">';
                        tbody += qualification.Guests[0].Guest;
                        tbody += '</td>';
                        tbody += '</tr>';

                        //first guest status
                        tbody += '<tr>';
                        $.each(qualification.Guests[0].BookingStatus, function (b, bs) {
                            tbody += '<td ' + (bs.Selected == true ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
                        });
                        tbody += '</tr>';

                        //other guests
                        $.each(qualification.Guests, function (g, guest) {
                            if (g > 0) {
                                tbody += '<tr>';
                                tbody += '<td colspan="6" class="pwl-guest" id="pwl-' + guest.ArrivalID + '">';
                                tbody += guest.Guest;
                                tbody += '</td>';
                                tbody += '</tr>';

                                tbody += '<tr>';
                                $.each(guest.BookingStatus, function (b, bs) {
                                    tbody += '<td ' + (bs.Selected == true ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
                                });
                                tbody += '</tr>';
                            }
                        });
                    } else {
                        tbody += '<td colspan="6"></td>';
                        tbody += '</tr>';
                    }
                }
            });

            //other opcs
            $.each(team.Promotors, function (p, promotor) {
                if (p > 0) {
                    tbody += '<td rowspan="' + promotor.Rows + '">';
                    tbody += '<h3 style="margin-bottom:0;">' + promotor.OPCName + '</h3>';

                    tbody += '<div class="table-div">';
                    tbody += '<div class="table-row">';
                    tbody += '<div class="table-cell">';
                    tbody += '<span class="summary-fields">' + promotor.Total + '</span><br>';
                    tbody += '<span class="summary-note">Assigned Leads</span>';
                    tbody += '</div>';
                    tbody += '<div class="table-cell">';
                    tbody += '<span class="summary-fields">' + promotor.BookingPercentage + '%</span><br>';
                    tbody += '<span class="summary-note">Penetration Rate</span>';
                    tbody += '</div>';
                    tbody += '</div>';
                    tbody += '</div>';

                    tbody += '<table class="table text-center non-editable" style="width:100%;"><tbody><tr>';
                    $.each(promotor.BSTotals, function (b, bs) {
                        tbody += '<td ' + (bs.BookingStatusCode == 'BK' ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
                    });
                    tbody += '</tr><tr>';
                    $.each(promotor.BSTotals, function (b, bs) {
                        tbody += '<td ' + (bs.BookingStatusCode == 'BK' ? 'class="bs-selected"' : '') + '>' + bs.Amount + '</td>';
                    });
                    tbody += '</tr><tr>';
                    $.each(promotor.BSTotals, function (b, bs) {
                        tbody += '<td ' + (bs.BookingStatusCode == 'BK' ? 'class="bs-selected"' : '') + '>' + bs.Percentage + '%</td>';
                    });
                    tbody += '</tr></tbody></table>';
                    tbody += '</td>';
                    //first qualification
                    tbody += '<td rowspan="' + promotor.QualificationStatuses[0].Rows + '" class="text-left">';
                    tbody += '<span class="right"><strong>' + promotor.QualificationStatuses[0].Bookings + ' / ' + promotor.QualificationStatuses[0].Total + '</strong>';
                    tbody += ' : ' + promotor.QualificationStatuses[0].Percentage + '%</span>';
                    tbody += '<span class="summary-fields">' + promotor.QualificationStatuses[0].Qualification + '</span>';
                    tbody += '<table class="table text-center non-editable" style="width:100%;"><tbody><tr>';
                    $.each(promotor.QualificationStatuses[0].BSTotals, function (b, bs) {
                        tbody += '<td>' + bs.BookingStatusCode + '</td>';
                    });
                    tbody += '</tr><tr>';
                    $.each(promotor.QualificationStatuses[0].BSTotals, function (b, bs) {
                        tbody += '<td>' + bs.Amount + '</td>';
                    });
                    tbody += '</tr><tr>';
                    $.each(promotor.QualificationStatuses[0].BSTotals, function (b, bs) {
                        tbody += '<td>' + bs.Percentage + '%</td>';
                    });
                    tbody += '</tr></tbody></table>';
                    tbody += '</td>';
                    if (promotor.QualificationStatuses[0].Guests.length > 0) {
                        //first guest
                        tbody += '<td colspan="6" class="pwl-guest" id="pwl-' + promotor.QualificationStatuses[0].Guests[0].ArrivalID + '">';
                        tbody += promotor.QualificationStatuses[0].Guests[0].Guest;
                        tbody += '</td>';
                        tbody += '</tr>';

                        //first guest status
                        tbody += '<tr>';
                        $.each(promotor.QualificationStatuses[0].Guests[0].BookingStatus, function (b, bs) {
                            tbody += '<td ' + (bs.Selected == true ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
                        });
                        tbody += '</tr>';

                        //other guests
                        $.each(promotor.QualificationStatuses[0].Guests, function (g, guest) {
                            if (g > 0) {
                                tbody += '<tr>';
                                tbody += '<td colspan="6" class="pwl-guest" id="pwl-' + guest.ArrivalID + '">';
                                tbody += guest.Guest;
                                tbody += '</td>';
                                tbody += '</tr>';

                                tbody += '<tr>';
                                $.each(guest.BookingStatus, function (b, bs) {
                                    tbody += '<td ' + (bs.Selected == true ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
                                });
                                tbody += '</tr>';
                            }
                        });
                    } else {
                        tbody += '<td colspan="6"></td>';
                        tbody += '</tr>';
                    }

                    //other qualifications
                    $.each(promotor.QualificationStatuses, function (q, qualification) {
                        if (q > 0) {
                            tbody += '<tr>';
                            tbody += '<td rowspan="' + qualification.Rows + '" class="text-left">';
                            tbody += '<span class="right"><strong>' + qualification.Bookings + ' / ' + qualification.Total + '</strong>';
                            tbody += ' : ' + qualification.Percentage + '%</span>';
                            tbody += '<span class="summary-fields">' + qualification.Qualification + '</span>';
                            tbody += '<table class="table text-center non-editable" style="width:100%;"><tbody><tr>';
                            $.each(qualification.BSTotals, function (b, bs) {
                                tbody += '<td>' + bs.BookingStatusCode + '</td>';
                            });
                            tbody += '</tr><tr>';
                            $.each(qualification.BSTotals, function (b, bs) {
                                tbody += '<td>' + bs.Amount + '</td>';
                            });
                            tbody += '</tr><tr>';
                            $.each(qualification.BSTotals, function (b, bs) {
                                tbody += '<td>' + bs.Percentage + '%</td>';
                            });
                            tbody += '</tr></tbody></table>';
                            tbody += '</td>';

                            if (qualification.Guests.length > 0) {
                                //first guest
                                tbody += '<td colspan="6" class="pwl-guest" id="pwl-' + qualification.Guests[0].ArrivalID + '">';
                                tbody += qualification.Guests[0].Guest;
                                tbody += '</td>';
                                tbody += '</tr>';

                                //first guest status
                                tbody += '<tr>';
                                $.each(qualification.Guests[0].BookingStatus, function (b, bs) {
                                    tbody += '<td ' + (bs.Selected == true ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
                                });
                                tbody += '</tr>';

                                //other guests
                                $.each(qualification.Guests, function (g, guest) {
                                    if (g > 0) {
                                        tbody += '<tr>';
                                        tbody += '<td colspan="6" class="pwl-guest" id="pwl-' + guest.ArrivalID + '">';
                                        tbody += guest.Guest;
                                        tbody += '</td>';
                                        tbody += '</tr>';

                                        tbody += '<tr>';
                                        $.each(guest.BookingStatus, function (b, bs) {
                                            tbody += '<td ' + (bs.Selected == true ? 'class="bs-selected"' : '') + '>' + bs.BookingStatusCode + '</td>';
                                        });
                                        tbody += '</tr>';
                                    }
                                });
                            } else {
                                tbody += '<td colspan="6"></td>';
                                tbody += '</tr>';
                            }
                        }
                    });
                }
            });
        });
        $('#tblPowerLine tbody').html(tbody);

        var tables = '';
        $.each(Hostess.PowerLine, function (t, team) {
            tables += '<span class="right">';
            tables += '<span summary-fields"><strong>' + team.Bookings + '</strong>';
            tables += ' / ' + team.Total + ' : ' + team.BookingPercentage + '%';
            tables += '</span><br /> Penetration</span>';
            tables += '<h3 style="display:inline-block;">' + team.Program + '</h3>';
            tables += '<table id="tblPenetrationRate" class="table non-stripable non-editable text-center" style="width:100%;">';
            tables += '<tbody><tr>';
            $.each(team.QualificationStatuses, function (q, qualification) {
                tables += '<td>';
                tables += '<span class="right"><strong>' + qualification.Bookings + '</strong>';
                tables += ' / ' + qualification.Total + ' : ' + qualification.BookingPercentage + '%';
                tables += '<br />Penetration</span>';
                tables += '<span class="summary-fields">' + qualification.Qualification + '</span>';
                tables += '<table class="table text-center non-editable" style="width:100%;"><tbody><tr>';
                $.each(qualification.BSTotals, function (b, bs) {
                    tables += '<td>' + bs.BookingStatusCode + '</td>';
                });
                tables += '</tr><tr>';
                $.each(qualification.BSTotals, function (b, bs) {
                    tables += '<td>' + bs.Amount + '</td>';
                });
                tables += '</tr><tr>';
                $.each(qualification.BSTotals, function (b, bs) {
                    tables += '<td>' + bs.Percentage + '%</td>';
                });
                tables += '</tr></tbody></table>';
                tables += '</td>';
            });
            tables += '</tr></tbody>';
            tables += '</table><br />';
        });
        $('#divPenetrationRate').html(tables);
    }

    function getSummary() {
        if ($('#searchByDate').is(':visible')) {
            $.each(Hostess.Summary, function (p, program) {
                var id = program.Program.replace(' ', '');
                if (id != '') {
                    if ($('#' + id).length == 0) {
                        $('#divSummary').append($('#divSummaryTemplate').html());
                        $('#divSummary .summary-template:last').attr('id', id);
                    }
                    $('#' + id + ' .title').html(program.Program);
                    $('#' + id + ' .active').html(program.Active);
                    $('#' + id + ' .in').html(program.CheckedIn);
                    $('#' + id + ' .out').html(program.CheckedOut);
                    if (Hostess.IsPast) {
                        $('#' + id + ' .out').show();
                        $('#' + id + ' .out-label').show();
                    } else {
                        $('#' + id + ' .out').parent().remove();
                    }

                    let data = new google.visualization.DataTable();
                    data.addColumn('string', 'Booking Status');
                    data.addColumn('number', 'Amount');
                    $.each(program.BookingStatuses, function (b, status) {
                        data.addRows([[status.BookingStatus, status.Amount]]);
                    });

                    let options = {
                        title: '',
                        pieHole: 0.4,
                        chartArea: { top: 0, height: '80%' },
                        legend: { position: 'bottom' },
                        backgroundColor: '#fcfdfd'
                    };

                    $('#' + id + ' .pie-chart').attr('id', id + '-pie-chart')
                    let chart = new google.visualization.PieChart(document.getElementById(id + '-pie-chart'));
                    chart.draw(data, options);

                } else {
                    //console.log('Active: ' + program.Active);
                    //console.log('Checked In: ' + program.CheckedIn);
                }
            });
            $('#arrivalsDashboard').slideDown('fast');
        } else {
            $('#arrivalsDashboard').slideUp('fast');
        }
    }

    function bindTablesClicks() {
        Hostess.tblArrivals.$('tr').not('theader').off('click').on('click', function (e) {
            if (!$(this).hasClass('selected-row')) {
                Hostess.tblArrivals.$('tr.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                var id = $(this).attr('id').substr($(this).attr('id').indexOf('-') + 1);
                loadArrivalInfo(id);
            }
        });
        Hostess.tblBinnacle.$('tr').not('theader').off('click').on('click', function (e) {
            if (!$(this).hasClass('selected-row')) {
                Hostess.tblBinnacle.$('tr.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                var id = $(this).attr('id').substr($(this).attr('id').indexOf('-') + 1);
                loadArrivalInfo(id);
            }
        });
        Hostess.tblManifest.$('tr').not('theader').off('click').on('click', function (e) {
            if (!$(this).hasClass('selected-row')) {
                Hostess.tblManifest.$('tr.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                var id = $(this).attr('id').substr($(this).attr('id').indexOf('-') + 1);
                loadArrivalInfo(id);
            }
        });
        if ($('#tblPrearrivals').length > 0) {
            Hostess.tblPrearrivals.$('tr').not('theader').off('click').on('click', function (e) {
                if (!$(this).hasClass('selected-row')) {
                    Hostess.tblPrearrivals.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    var id = $(this).attr('id').substr($(this).attr('id').indexOf('-') + 1);
                    loadArrivalInfo(id);
                }
            });
        }        
        $('.pwl-guest').off('click').on('click', function () {
            if (!$(this).hasClass('selected-row')) {
                $('.pwl-guest').removeClass('selected-row');
                $(this).addClass('selected-row');
                var id = $(this).attr('id').substr($(this).attr('id').indexOf('-') + 1);
                loadArrivalInfo(id);
            }
        });
    }

    function clearMemberInfo() {
        $('#SPIRelatedCustomerID').html('');
        $('#tblToursHistory tbody').html('');
        $('#tblLegalNames tbody').html('');
        $('#TourDate').val('');
        $('#SalesCenter').val('');
        $('#TourSource').val('');
        $('#SourceGroup').val('');
        $('#SourceItem').val('');
        $('#Qualification').val('');
        $('#TourContractNumber').val('');
        $('#Volume').val('');
    }

    function loadArrivalInfo(id) {
        if (id != '00000000-0000-0000-0000-000000000000') {
            $('#frmArrivalInfo').clearForm();
            clearMemberInfo();
            $.each(Hostess.ArrivalsList, function (a, arrival) {
                //console.log(arrival);
                if (arrival.ArrivalID == id) {
                    $('#ArrivalID').val(arrival.ArrivalID);
                    if (arrival.Picture != null && arrival.Picture != "") {
                        $('.guest-pic').html('<img style="width:150px;" src="' + arrival.Picture.replace('s64-c', 's300-c') + '" />');
                    } else {
                        $('.guest-pic').html('');
                    }
                    //front office info
                    $('#FrontOfficeReservationID').val(arrival.FrontOfficeReservationID);
                    $('#ArrivalDate').val(arrival.ArrivalDate);
                    $('#Nights').val(arrival.Nights);
                    $('#Guest').val(arrival.Guest);
                    $('.guest-name').text(arrival.FirstName.toString().toLowerCase() + ' ' + arrival.LastName.toString().toLowerCase());
                    $('#FirstName').val(arrival.FirstName);
                    $('#LastName').val(arrival.LastName);
                    $('#Adults').val(arrival.Adults);
                    $('#Children').val(arrival.Children);
                    $('#Infants').val(arrival.Infants);
                    $('#Country').val(arrival.Country);
                    $('#CountryID').val(arrival.CountryID);
                    $('#AgencyName').val(arrival.AgencyName);
                    $('#Source').val(arrival.Source);
                    $('#ReservationStatus').val(arrival.ReservationStatus);
                    $('#RoomType').val(arrival.RoomType);
                    $('#ManifestRoomType').val(arrival.RoomType);
                    $('#ConfirmationNumber').val(arrival.ConfirmationNumber);
                    $('#Crs').val(arrival.Crs);
                    $('#CheckinDateTime').val(arrival.CheckinDateTime);
                    $('#Contract').val(arrival.Contract);
                    $('#PlanType').val(arrival.PlanType);
                    $('#DateLastUpdate').val(arrival.DateLastUpdate);
                    $('#LeadID').val(arrival.LeadID);
                    $('#ExtensionReservation').val(arrival.ExtensionReservation);

                    //guest info
                    $('#Email1').val(arrival.Email1);
                    $('#Email2').val(arrival.Email2);
                    $('#Phone1').val(arrival.Phone1);
                    $('#Phone2').val(arrival.Phone2);

                    $('#CountryID').val(arrival.CountryID).trigger('change');
                    $('#City').val(arrival.City);
                    $('#StateID').val(arrival.StateID);
                    $('#ZipCode').val(arrival.ZipCode);

                    $('#PCIPrearrivalStatus').val(arrival.PCIPrearrivalStatus);
                    $('#PCILastInteractionAgent').val(arrival.PCILastInteractionAgent);
                    $('#PCILastInteractionDate').val(arrival.PCILastInteractionDate);
                    //console.log(arrival.PCIVipCardType);
                    $('#PCIVipCardType').val(arrival.PCIVipCardType);
                    $('#PCIVipCardStatus').val(arrival.PCIVipCardStatus);
                    $('#PCIVipCardStatusAgent').val(arrival.PCIVipCardStatusAgent);
                    if (arrival.PCICompleted == true) {
                        $('#PCICompleted').prop('checked', true);
                    }
                    $('#PCIConciergeComments').val(arrival.PCIConciergeComments);

                    $('#Preferences').val(arrival.Preferences);
                    var arrPreferences = arrival.Preferences.split(',');
                    $('#tblPreferences input[type="checkbox"]').prop('checked', false);
                    $.each(arrPreferences, function (p, pref) {
                        $('#chk_' + pref).prop('checked', true);
                    });

                    //$('#').val(arrival.);

                    //survey info
                    if (arrival.SentDate != null) {
                        $('#SentDate').val(arrival.SentDate);
                        $('#SubmittedDate').val(arrival.SubmittedDate);
                        $('#Rate').val(arrival.Rate);
                        $('#SurveyRoomNumber').val(arrival.SurveyRoomNumber);
                        $('.survey-answers').html('');
                        $.each(arrival.Answers, function (a, answer) {
                            $('.survey-answers').append('<div class="hostess-survey-question ' + (a % 2 > 0 ? 'odd' : 'even') + '"><span class="question">' + answer.Description + '</span><span class="answer">' + answer.Answer + '</span></div>');
                        });
                        $('#divClarabridge').show();
                        $('#tabClarabridge').show();
                    } else {
                        $('#divClarabridge').hide();
                        $('#tabClarabridge').hide();
                    }

                    //member info
                    if (arrival.LeadID != null && arrival.LeadID != '') {
                        //$('#divMemberInfo').show();
                        $('#ClubType').val(arrival.ClubType);
                        $('#AccountNumber').val(arrival.AccountNumber);
                        $('#ContractNumber').val(arrival.ContractNumber);
                        $('#CoOwner').val(arrival.CoOwner);

                        //prearrival info
                        if (arrival.PreArrivalStatus != null) {
                            $('#PreArrivalReservationID').val(arrival.PreArrivalReservationID);
                            $('#PreArrivalStatus').val(arrival.PreArrivalStatus);
                            $('#PreArrivalOptionsTotal').val(arrival.PreArrivalOptionsTotal);
                            $('#PreArrivalFeedBack').val(arrival.PreArrivalFeedBack);
                            //$('.options-conf').attr('href', 'https://www.resortcom.com/ConfirmationRpt/WebPDFCreator.aspx?Enc=False&ConfirmType=1&ActiveOnly=true&Confirm=' + arrival.Crs);
                            //$('.transportation-conf').attr('href', 'https://www.resortcom.com/ConfirmationRpt/WebPDFCreator.aspx?Enc=False&ConfirmType=2&ActiveOnly=true&Confirm=' + arrival.Crs);
                            if (arrival.PrintedLetterOnHand == true) {
                                $('input[name="PrintedLetterOnHand"][value="true"]').prop('checked', true);
                                $('input[name="PrintedLetterOnHand"][value="false"]').prop('checked', false);
                            } else if (arrival.PrintedLetterOnHand == false) {
                                $('input[name="PrintedLetterOnHand"][value="true"]').prop('checked', false);
                                $('input[name="PrintedLetterOnHand"][value="false"]').prop('checked', true);
                            } else {
                                $('input[name="PrintedLetterOnHand"][value="true"]').prop('checked', false);
                                $('input[name="PrintedLetterOnHand"][value="false"]').prop('checked', false);
                            }

                            //interactions
                            if (arrival.Interactions.length > 0) {
                                $('#divInteractionsInfo').html('');
                                var interactionsHTML = '';
                                $.each(arrival.Interactions, function (i, interaction) {
                                    interactionsHTML += '<div class="interaction">';
                                    interactionsHTML += '<div class="interaction-info"><strong>' + parseInt(i + 1).toString() + '</strong><br />' + interaction.Type + '<br />' + interaction.Status + '<br />' + (interaction.User != null ? interaction.User : "") + '<br />' + interaction.Date + '</div>';
                                    interactionsHTML += '<div class="interaction-comments">' + interaction.Comments + '</div>';
                                    interactionsHTML += '</div>';
                                });
                                $('#divInteractionsInfo').html(interactionsHTML);
                            } else {
                                $('#divInteractionsInfo').html('There are not Interactions.');
                            }

                            //flights info
                            if (arrival.FlightsInfo.length > 0) {
                                $('#tblFlights tbody').html('');
                                var flightsHTML = '';
                                $.each(arrival.FlightsInfo, function (f, flight) {
                                    flightsHTML += '<tr>';
                                    flightsHTML += '<td>' + flight.FlightType + '</td>';
                                    flightsHTML += '<td>' + flight.DateTime + '</td>';
                                    flightsHTML += '<td>' + flight.Airline + '</td>';
                                    flightsHTML += '<td>' + flight.FlightNumber + '</td>';
                                    flightsHTML += '<td>' + flight.NumberOfPassengers + '</td>';
                                    flightsHTML += '</tr>';
                                });
                                $('#tblFlights tbody').html(flightsHTML);
                            } else {
                                $('#tblFlights tbody').html('<tr><td colspan="5">There is not Flight Information.</td></tr>');
                            }

                            //letters info
                            $('#tblConfirmationLetters tbody').html('');
                            var lettersHTML = '';
                            $.each(arrival.ConfirmationLetters, function (l, letter) {
                                lettersHTML += '<tr>';
                                //lettersHTML += '<td>' + letter.Key + '</td>';
                                lettersHTML += '<td>' + letter.Value + '</td>';
                                //lettersHTML += '<td><i class="material-icons preview" data-id="' + letter.Key + '" data-reservation-id="' + arrival.PreArrivalReservationID + '">email</i></td>'
                                lettersHTML += '<td><a href="https://eplat.villagroup.com/Notifications/Preview/' + arrival.PreArrivalReservationID + '?log=' + letter.Key + '" class="fake-button options-conf" target="_blank">view</a></td>';
                                //lettersHTML += '<td><a href="http://localhost:45000/Notifications/Preview/' + arrival.PreArrivalReservationID + '?log=' + letter.Key + '" class="fake-button options-conf" target="_blank">view</a></td>';
                                lettersHTML += '</tr>';
                            });
                            lettersHTML += '<tr><td>Options Letter</td><td><a href="https://www.resortcom.com/ConfirmationRpt/WebPDFCreator.aspx?Enc=False&ConfirmType=1&ActiveOnly=true&Confirm=' + arrival.Crs + '" class="fake-button" target="_blank">view</a></td></tr>';
                            lettersHTML += '<tr><td>Transportation Letter</td><td><a href="https://www.resortcom.com/ConfirmationRpt/WebPDFCreator.aspx?Enc=False&ConfirmType=2&ActiveOnly=true&Confirm=' + arrival.Crs + '" class="fake-button" target="_blank">view</a></td></tr>';
                            $('#tblConfirmationLetters tbody').html(lettersHTML);

                            $('#divPreArrivalInfo').show();

                            //$('.preview').unbind('click').on('click', function () {
                            //    var notification = $(this).attr('data-id');
                            //    var transaction = $(this).attr('data-transaction');
                            //    var reservation = $(this).attr('data-reservation-id');
                            //    $.ajax({
                            //        url: '/PreArrival/PreviewEmail',
                            //        cache: false,
                            //        data: { reservationID: reservation, emailNotificationID: notification, transactionID: transaction },
                            //        success: function (data) {
                            //            if (data.Status == null || data.Status == '') {
                            //                UI.renderEmailPreview(data, [reservation, notification, transaction]);
                            //            }
                            //            else {
                            //                UI.messageBox(-1, data.Status, null, null);
                            //            }
                            //        }
                            //    });
                            //});

                        } else {
                            $('#divPreArrivalInfo').hide();
                        }

                    } else {
                        //$('#divMemberInfo').hide();
                        $('#divPreArrivalInfo').hide();
                    }

                    //console.log(arrival);
                    //console.log('programid' + arrival.ProgramID);
                    if (arrival.ProgramID == 3 || arrival.ProgramID == 11) {
                        //limpiar datos de selección anterior

                        //buscar posibles clientes

                        let dataObject = {
                            firstname: $('#FirstName').val(),
                            lastname: $('#LastName').val()
                        }

                        //$.getJSON('/SPI/SearchCustomer', dataObject, function (data) {
                        //    $('#SPIRelatedCustomerID').fillSelect(data);
                        //    if (data.length == 1) {
                        //        //si solo regresa 1, trigger 
                        //        $('#btnSearchSPIHistory').trigger('click');
                        //    }
                        //    if ($('#SPICustomerID').val() != "" && data.length > 1) {
                        //        $('#SPIRelatedCustomerID').val($('#SPICustomerID').val());
                        //        $('#btnSearchSPIHistory').trigger('click');
                        //    }
                        //});

                        $.getJSON('/SPI/SearchCustomerHistory', dataObject, function (data) {
                            //history
                            Hostess.CustomerHistory = data;
                            let tbody = '';
                            $.each(Hostess.CustomerHistory.History, function (i, tour) {
                                tbody += '<tr id="th' + i + '">';
                                tbody += '<td>' + tour.TourDate + '</td>';
                                tbody += '<td>' + tour.SalesCenter + '</td>';
                                tbody += '<td>' + tour.TourContractNumber + '</td>';
                                tbody += '<td>' + (tour.Volume != null ? '$' + tour.Volume : '') + '</td>'
                                tbody += '<td>' + tour.CustomerName + '</td>';
                                tbody += '<td>' + tour.CustomerID + '</td>';
                                tbody += '<td>' + tour.AccountNumber + '</td>';
                                tbody += '<td>' + (tour.LastTour ? 'Yes' : '') + '</td>';
                                tbody += '<td>' + (tour.LastContract ? 'Yes' : '') + '</td>';
                                tbody += '</tr>';
                            });
                            $('#tblToursHistory tbody').html(tbody);
                            $('#tblToursHistory tbody tr').off('click').on('click', function () {
                                let index = $(this).attr('id').replace('th', '');
                                $('#TourDate').val(Hostess.CustomerHistory.History[index].TourDate);
                                $('#SalesCenter').val(Hostess.CustomerHistory.History[index].SalesCenter);
                                $('#TourSource').val(Hostess.CustomerHistory.History[index].TourSource);
                                $('#SourceGroup').val(Hostess.CustomerHistory.History[index].SourceGroup);
                                $('#SourceItem').val(Hostess.CustomerHistory.History[index].SourceItem);
                                $('#Qualification').val(Hostess.CustomerHistory.History[index].Qualification);
                                $('#TourContractNumber').val(Hostess.CustomerHistory.History[index].TourContractNumber);
                                $('#Volume').val((Hostess.CustomerHistory.History[index].Volume != null ? '$' + Hostess.CustomerHistory.History[index].Volume : ''));
                                UI.scrollTo('divTourInfo', null);
                            });
                            if ($('#tblToursHistory tbody tr').length == 1) {
                                $('#tblToursHistory tbody tr').eq(0).trigger('click');
                            }
                            //legal names
                            let lbody = '';
                            $.each(data.LegalNames, function (l, legal) {
                                lbody += '<tr><td>' + legal.Name + '</td><td>' + legal.DateOfBirth + '</td><td>' + legal.Age + ' years</td></tr>';
                            });
                            $('#tblLegalNames tbody').html(lbody);
                        });

                        $('#divMemberInfo').show();
                    } else {
                        $('#divMemberInfo').hide();
                    }

                    //hostess info
                    $('#GuestTypeID').val(arrival.GuestTypeID).trigger('change');
                    $('#ProgramID').val(arrival.ProgramID).trigger('change');
                    $('#HostessQualificationStatusID').val(arrival.HostessQualificationStatusID);
                    $('#Nationality').val(arrival.Nationality);
                    $('#PreCheckIn').val(arrival.PreCheckIn);
                    $('#NQReasonID').val(arrival.NQReasonID);
                    if (arrival.PromotionTeamID != null) {
                        $('#PromotionTeamID').val(arrival.PromotionTeamID).trigger('change');
                    }
                    $('#HostessName').val(arrival.HostessName);
                    $('#HostessInputDateTime').val(arrival.HostessInputDateTime);
                    $('#BookingStatusID').val(arrival.BookingStatusID);
                    $('#Comments').val(arrival.Comments);

                    //revisar si es editable o no
                    if ($('#tabHostessEdit').val() != "true" && $('#HostessInputDateTime').val() != "") {
                        //no editable
                        $('#ProgramID').attr("disabled", "disabled").css('background-color', '#E8EEF4 !important');
                        $('#TravelSourceID').attr("disabled", "disabled").css('background-color', '#E8EEF4 !important');
                        $('#HostessQualificationStatusID').attr("disabled", "disabled").css('background-color', '#E8EEF4 !important');
                        $('#NQReasonID').attr("disabled", "disabled").css('background-color', '#E8EEF4 !important');
                        $('#PromotionTeamID').attr("disabled", "disabled").css('background-color', '#E8EEF4 !important');
                        $('#OPCID').attr("disabled", "disabled").css('background-color', '#E8EEF4 !important');
                        $('#BookingStatusID').attr("disabled", "disabled").css('background-color', '#E8EEF4 !important');
                        $('#VipCardType').attr("disabled", "disabled").css('background-color', '#E8EEF4 !important');
                        $('#VipCardStatus').attr("disabled", "disabled").css('background-color', '#E8EEF4 !important');
                        $('#Comments').attr("readonly", "readonly").css('background-color', '#E8EEF4 !important');
                    } else {
                        $('#ProgramID').prop("disabled", null).css('background-color', '#E1F7FF');
                        $('#TravelSourceID').prop("disabled", null).css('background-color', '#E1F7FF');
                        $('#HostessQualificationStatusID').prop("disabled", null).css('background-color', '#E1F7FF');
                        $('#NQReasonID').prop("disabled", null).css('background-color', '#E1F7FF');
                        $('#PromotionTeamID').prop("disabled", null).css('background-color', '#E1F7FF');
                        $('#OPCID').prop("disabled", null).css('background-color', '#E1F7FF');
                        $('#BookingStatusID').prop("disabled", null).css('background-color', '#E1F7FF');
                        $('#VipCardType').prop("disabled", null).css('background-color', '#E1F7FF');
                        $('#VipCardStatus').prop("disabled", null).css('background-color', '#E1F7FF');
                        $('#Comments').prop("readonly", null).css('background-color', '#E1F7FF');
                    }

                    $('#PresentationDate').val(arrival.PresentationDate).trigger('change');
                    $('#SalesRoomPartyID').off('loaded').on('loaded', function () {
                        $('#SalesRoomPartyID').val(arrival.SalesRoomPartyID);
                    });
                    $('#Deposit').val(arrival.Deposit);
                    $('#DepositCurrencyID').val(arrival.DepositCurrencyID);
                    $('#PickUpTimeHour').val(arrival.PickUpTimeHour);
                    $('#PickUpTimeMinute').val(arrival.PickUpTimeMinute);
                    $('#PickUpTimeMeridian').val(arrival.PickUpTimeMeridian);
                    $('#GuestEmail').val(arrival.GuestEmail);

                    $('#Confirmed').val((arrival.Confirmed != null ? arrival.Confirmed.toString() : null));
                    $('#InvitationNumber').val(arrival.InvitationNumber);
                    $('#ManifestRoomNumber').val(arrival.RoomNumber);
                    $('#PresentationPax').val(arrival.PresentationPax);
                    $('#VipCardType').val(arrival.VipCardType);
                    $('#VipCardStatus').val(arrival.VipCardStatus);
                    $('#VipCardStatusAgent').val(arrival.VipCardStatusAgent);
                    $('#Gifting').val(arrival.Gifting);
                    $('#InvitationGenerated').val(arrival.InvitationGenerated);

                    $('#TravelSourceID').val(arrival.TravelSourceID);
                    $('#OPCID').val(arrival.OPCID);

                    $('#SPICustomerID').val(arrival.SPICustomerID);
                    //$('#').val(arrival.);
                }
            });
            UI.expandFieldset('fdsArrivalInfo');
            UI.scrollTo('fdsArrivalInfo', null);
            $('#guestTabs').tabs('option', 'active', 0);//to force redraw of client info on tabs
        }
    }

    function getTodayDate() {
        var date = new Date();
        $('#txtArrivalsDate').val(date.yyyymmdd());
        Hostess.getArrivalsInfo(false);
    }

    var forecastUpdated = function () {
        UI.exportToExcel("Arrivals Forecast");
    }

    return {
        init: init,
        getArrivalsInfo: getArrivalsInfo,
        getArrivalsByName: getArrivalsByName,
        ArrivalsList: ArrivalsList,
        Summary: Summary,
        IsPast: IsPast,
        Prearrivals: Prearrivals,
        PowerLine: PowerLine,
        CustomerHistory: CustomerHistory,
        tblArrivals: tblArrivals,
        tblBinnacle: tblBinnacle,
        tblManifest: tblManifest,
        tblPrearrivals: tblPrearrivals,
        updated: updated,
        forecastUpdated: forecastUpdated,
        xhrArrivals: xhrArrivals,
        xhrSummaries: xhrSummaries
    }
}();

$(function () {
    Hostess.init();
});