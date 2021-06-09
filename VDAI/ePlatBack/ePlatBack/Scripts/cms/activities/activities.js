/// <reference path="/Scripts/cms/pictures.js" />
/// <reference path="/Scripts/cms/seoItems.js" />
/// <reference path="/Scripts/cms/prices.js" />
/// <reference path="/Scripts/Common.js" />

$(function () {
    ACTIVITY.init();
    PREVACTIVITY.init();
    $('#PriceInfo_ItemType').val('Activities');
    $('#PictureInfo_ItemType').val('Activities');
    $('#SeoItemInfo_ItemType').val('Activities');
    $('#ActivityInfo_Length').numeric();
    $('#ActivityInfo_MinimumAge').numeric();
    $('#ActivityInfo_MinimumHeight').numeric();
    $('#ActivityInfo_MaximumWeight').numeric();
    UI.applyCKEditor('frmActivityDescription');
    $('#ActivityScheduleInfo_FromDate').datepicker({
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        onClose: function (dateText, inst) {
            if ($('#ActivityScheduleInfo_ToDate').val() != '') {
                if (dateText != '') {
                    var fromDate = $('#ActivityScheduleInfo_FromDate').datepicker('getDate');
                    var toDate = $('#ActivityScheduleInfo_ToDate').datepicker('getDate');
                    if (fromDate > toDate) {
                        $('#ActivityScheduleInfo_ToDate').datepicker('setDate', fromDate);
                    }
                }
                else {
                    $('#ActivityScheduleInfo_ToDate').val(dateText);
                }
            }
            else {
                $('#ActivityScheduleInfo_ToDate').val(dateText);
            }
        },
        onSelect: function (selectedDate) {
            if ($('#ActivityScheduleInfo_ToDate').val() != '') {
                $('#ActivityScheduleInfo_ToDate').datepicker('setDate', $('#ActivityScheduleInfo_ToDate').datepicker('getDate'));
            }
            $('#ActivityScheduleInfo_ToDate').datepicker('option', 'minDate', $('#ActivityScheduleInfo_FromDate').datepicker('getDate'));
        }
    });
    $('#ActivityScheduleInfo_ToDate').datepicker({
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        onClose: function (dateText, inst) {
            if ($('#ActivityScheduleInfo_FromDate').val() != '') {
                var fromDate = $('#ActivityScheduleInfo_FromDate').datepicker('getDate');
                var toDate = $('#ActivityScheduleInfo_ToDate').datepicker('getDate');
                if (fromDate > toDate) {
                    $('#ActivityScheduleInfo_FromDate').datepicker('setDate', toDate);
                }
            }
            else {
                $('#ActivityScheduleInfo_FromDate').val(dateText);
            }
        }
        //onSelect: function (selectedDate) {
        //    if ($('#ActivityScheduleInfo_FromDate').val() != '')
        //        $('#ActivityScheduleInfo_FromDate').datepicker('setDate', $('#ActivityScheduleInfo_FromDate').datepicker('getDate'));
        //    $('#ActivityScheduleInfo_FromDate').datepicker('option', 'maxDate', $('#ActivityScheduleInfo_ToDate').datepicker('getDate'));
        //}
    });
    $('#ActivityScheduleInfo_FromTime').datetimepicker({
        timeFormat: 'HH:mm:ss',
        timeOnly: true,
        stepMinute: 5,
        onClose: function (dateText, inst) {
            if ($('#ActivityScheduleInfo_ToTime').val() == '') {
                $('#ActivityScheduleInfo_ToTime').val(dateText);
            }
            //if ($('#ActivityScheduleInfo_ToTime').val() != '') {
            //    var fromDate = $('#ActivityScheduleInfo_FromTime').datetimepicker('getDate');
            //    var toDate = $('#ActivityScheduleInfo_ToTime').datetimepicker('getDate');
            //    if (fromDate > toDate)
            //        $('#ActivityScheduleInfo_ToTime').datepicker('setDate', fromDate);
            //}
            //else
            //    $('#ActivityScheduleInfo_ToTime').val(dateText);
        },
        onSelect: function (selectedDate) {
            if ($('#ActivityScheduleInfo_ToTime').val() != '')
                $('#ActivityScheduleInfo_ToTime').datetimepicker('setDate', $('#ActivityScheduleInfo_ToTime').datetimepicker('getDate'));
            $('#ActivityScheduleInfo_ToTime').datetimepicker('option', 'minDate', $('#ActivityScheduleInfo_FromTime').datetimepicker('getDate'));
        }
    });
    $('#ActivityScheduleInfo_ToTime').datetimepicker({
        timeFormat: 'HH:mm:ss',
        timeOnly: true,
        stepMinute: 5,
        onClose: function (dateText, inst) {
            if ($('#ActivityScheduleInfo_FromTime').val() == '') {
                $('#ActivityScheduleInfo_FromTime').val(dateText);
            }
            //if ($('#ActivityScheduleInfo_FromTime').val() != '') {
            //    var fromDate = $('#ActivityScheduleInfo_FromTime').datetimepicker('getDate');
            //    var toDate = $('#ActivityScheduleInfo_ToTime').datetimepicker('getDate');
            //    if (fromDate > toDate)
            //        $('#ActivityScheduleInfo_FromTime').datetimepicker('setDate', toDate);
            //}
            //else
            //    $('#ActivityScheduleInfo_FromTime').val(dateText);
        },
        onSelect: function (selectedDate) {
            if ($('#ActivityScheduleInfo_FromTime').val() != '')
                $('#ActivityScheduleInfo_FromTime').datetimepicker('setDate', $('#ActivityScheduleInfo_FromTime').datetimepicker('getDate'));
            $('#ActivityScheduleInfo_FromTime').datetimepicker('option', 'maxDate', $('#ActivityScheduleInfo_ToTime').datetimepicker('getDate'));
        }
    });
    $('#ActivityScheduleInfo_IntervalTime').numeric();
    $('#ActivityMeetingPointInfo_DepartureTime').datetimepicker({
        timeFormat: 'HH:mm:ss',
        timeOnly: true,
        stepMinute: 5,
        onClose: function (data, b, c) {
            //if ($('#ActivityMeetingPointInfo_PlaceString').val() != "") {
            //    var id = data.split(':').join('_') + '+' + $('#ActivityMeetingPointInfo_Place').val();
            //    var builder = '<tr id="' + id + '"><td>' + data + '</td><td>' + $('#ActivityMeetingPointInfo_PlaceString').val() + '<img src="/Content/themes/base/images/cross.png" class="delete-row right"></td></tr>';
            //    if ($('#tblDepartureTimes tbody').find($('#' + id)).length > 0) {
            //        UI.messageBox(-1, "Time already exists", null, null);
            //    }
            //    else {
            //        if (data != '') {
            //            $('#tblDepartureTimes tbody').append(builder);
            //            $('#ActivityMeetingPointInfo_DepartureTimes').attr('value', $('#ActivityMeetingPointInfo_DepartureTimes').val() + id + ',');
            //            //$('#ActivityMeetingPointInfo_DepartureTime').val('');
            //            //$('#ActivityMeetingPointInfo_PlaceString').val('');
            //        }
            //    }
            //    ACTIVITY.deleteRowFunctionality();
            //}
        }
    });

    //$('#btnAddMeetingPointToTable').on('click', function () {
    //    if ($('#ActivityMeetingPointInfo_DepartureTime').val() != '') {
    //        var _id = $('#ActivityMeetingPointInfo_DepartureTime').val().split(':').join('_') + '|' + (!$('input:radio[name="ActivityMeetingPointInfo_AtYourHotel"]')[0].checked ? $('#ActivityMeetingPointInfo_Place').val() : 'null');
    //        if ($('#tblDepartureTimes tbody').find('#' + _id).length > 0) {
    //            UI.messageBox(-1, 'Meeting Point already exist', null, null);
    //        }
    //        else {
    //            var _place = '';
    //            if ($('#ActivityMeetingPointInfo_PlaceString').val() != '' && !$('input:radio[name="ActivityMeetingPointInfo_AtYourHotel"]')[0].checked) {
    //                _place = $('#ActivityMeetingPointInfo_PlaceString').val();
    //            }
    //            else if ($('input:radio[name="ActivityMeetingPointInfo_AtYourHotel"]')[0].checked) {
    //                _place = 'At Your Hotel';
    //            }
    //            if (_place != '') {
    //                var builder = '<tr id="' + _id + '">'
    //                    + '<td>' + $('#ActivityMeetingPointInfo_DepartureTime').val() + '</td>'
    //                    + '<td>' + _place + '<img src="/content/themes/base/images/cross.png" class="delete-row right"></td>'
    //                    + '</tr>';
    //                $('#tblDepartureTimes tbody').append(builder);
    //                $('#ActivityMeetingPointInfo_DepartureTimes').attr('value', $('#ActivityMeetingPointInfo_DepartureTimes').val() + _id + ',');
    //            }
    //            else {
    //                UI.messageBox(-1, 'If meeting point is not at hotel, you must specify a place', null, null);
    //            }
    //        }
    //        ACTIVITY.deleteRowFunctionality();
    //    }
    //});
});

var ACTIVITY = function () {

    var categories;

    var oTable;

    var oAccountingAccountsTable;

    var oAccountsPerActivityTable;

    var oPointsOfSaleTable;

    var oRulesTable;

    var oFutureRulesTable;

    var oStockTransactionsTable;

    var destination;

    //revisar porqué la repetición de peticiones a GetDDLData al cambiar terminales seleccionadas

    var init = function () {
        //disabled only when loaded in activities module
        //$('#SeoItemInfo_TerminalItem').attr('disabled', 'disabled');
        $('#SeoItemInfo_TerminalItem').on('mousedown', function (e) {
            e.preventDefault();
        });
        $('#SeoItemInfo_TerminalItem').attr('data-keep-value', '');

        ACTIVITY.searchResultsTable($('#tblSearchActivitiesResults'));

        $('#ActivityScheduleInfo_CheckAll').on('change', function () {
            var checked = $(this).is(':checked');
            $('#frmActivitySchedule .weekdays-alignment input:checkbox').attr('checked', checked);
        });

        $('#frmActivitySchedule .weekdays-alignment input:checkbox:not(#ActivityScheduleInfo_CheckAll)').on('change', function () {
            if (!$(this).is(':checked')) {
                $('#ActivityScheduleInfo_CheckAll').attr('checked', false);
            }
            else {
                if ($('#frmActivitySchedule .weekdays-alignment input:checkbox:checked:not(#ActivityScheduleInfo_CheckAll)').length == 7) {
                    $('#ActivityScheduleInfo_CheckAll').attr('checked', true);
                }
            }
        });

        $('#ActivityScheduleInfo_FromDate').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#ActivityScheduleInfo_ToDate').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#ActivityScheduleInfo_FromTime').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#ActivityScheduleInfo_ToTime').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#ActivityCategoryInfo_Terminal').on('change', function () {
            $.getJSON('/Activities/GetDDLData', { itemID: $(this).val(), itemType: 'catalogs' }, function (data) {
                $('#ActivityCategoryInfo_Catalog').fillSelect(data);
            });
        });

        $('#ActivityCategoryInfo_Catalog').on('change', function () {
            $.getJSON('/Activities/GetDDLData', { itemID: $(this).val(), itemType: 'categories' }, function (data) {
                $('#ActivityCategoryInfo_Category').fillSelect(data);
            });
        });

        $('#btnSaveActivityCategory').on('click', function () {
            var categories = '';
            var categoryNames = '';
            $('#ActivityCategoryInfo_Categories').empty();
            $('#tblCategoriesSelected tbody tr').each(function (index) {
                categories += $(this).children('td:nth-child(1)').attr('id').substr(10) + ',';
                categoryNames += '<span class="block">' + $(this).children('td:nth-child(1)')[0].textContent + '</span>';
            });
            categories = categories.substr(0, categories.length - 1);
            //categoryNames = categoryNames.substr(0, categoryNames.length - 2);
            ACTIVITY.categories = categoryNames;
            var content = (categories != '') ? categories : '';
            $('#ActivityCategoryInfo_Categories').val(content);
            $('#frmActivityCategory').validate().settings.ignore = '.ignore-validation';//Forces validation of input:hidden
            $('#frmActivityCategory').submit();
        });

        $('#ActivityInfo_OriginalTerminal').on('change', function (e, params) {
            $.getJSON('/Activities/GetDDLData', { itemID: $(this).val(), itemType: 'destinationsByTerminal' }, function (data) {
                $('#ActivityInfo_Destination').fillSelect(data, false);
                var destination = params != undefined ? params.destination : 0;
                ACTIVITY.destination = $('#ActivityInfo_Destination option:selected').val();
                if (destination != 0) {
                    $('#ActivityInfo_Destination option[value="' + destination + '"]').attr('selected', true);
                    $('#ActivityInfo_Destination').trigger('change', params);
                }
            });
            $.getJSON('/Activities/GetDDLData', { itemID: $(this).val(), itemType: 'pointOfSale' }, function (data) {
                $('#ActivityAccountingAccountInfo_PointOfSale').fillSelect(data);
                $('#ActivityAccountingAccountInfo_PointOfSale').multiselect('refresh');
            });
        });

        $('#ActivityInfo_Destination').on('change', function (e, params) {
            $.getJSON('/Activities/GetDDLData', { itemID: $('#ActivityInfo_OriginalTerminal option:selected').val() + '|' + $(this).val(), itemType: 'providers' }, function (data) {
                $('#ActivityInfo_Provider').fillSelect(data);
                var provider = params != undefined ? params.provider : 0;
                var text = $('#ActivityInfo_Destination').val() != '0' ? 'One' : 'Destination';
                $('#ActivityInfo_Provider').prepend('<option value="0" selected="selected">--Select ' + text + '--</option>');
                if (provider != 0) {
                    $('#ActivityInfo_Provider option[value="' + provider + '"]').attr('selected', true);
                    PICTURE.loadPicturesTree(provider);
                    ACTIVITY.updateServicesDependantLists($('#ActivityInfo_OriginalTerminal').val(), provider, $('#ActivityInfo_ActivityID').val());
                }
            });
            $.getJSON('/Activities/GetDDLData', { itemID: $(this).val(), itemType: 'zones' }, function (data) {
                $('#ActivityInfo_Zone').fillSelect(data);
                var zone = params != undefined ? params.zone : 0;
                if (zone != 0) {
                    $('#ActivityInfo_Zone option[value="' + zone + '"]').attr('selected', true);
                }
            });
        });

        $('#btnSaveActivityDescription').unbind('click').on('click', function () {
            var valid = true;
            $('#ulActivityDescriptions li').each(function () {
                if ($(this).text() == $('#ActivityDescriptionInfo_Culture option:selected').text() && !$(this).hasClass('selected-row'))
                    valid = false;
            });
            if (valid) {
                UI.ckeditorUpdateInstances('frmActivityDescription');
                $('#frmActivityDescription').data('validator').settings.ignore = '.ignore-validation';
                $('#frmActivityDescription').submit();
            }
            else {
                UI.messageBox(-1, "Description Language already exists", null, null);
            }
        });

        $('#btnAddCategory').on('click', function () {
            var rows = $('#tblCategoriesSelected > tbody > tr').length;
            var builder = '<tr><td id="tdCategory' + $('#ActivityCategoryInfo_Category option:selected').val() + '">'
                + $('#ActivityCategoryInfo_Catalog option:selected').text() + ' > ' + $('#ActivityCategoryInfo_Category option:selected').text()
                + '</td>'
                + '<td><img class="right" src="/Content/themes/base/images/trash.png"/></td>'
                + '</tr>';
            if ($('#ActivityCategoryInfo_Category option:selected').val() != 0) {
                var counter = 0;
                for (var i = 0; i <= rows; i++) {
                    if (($('#ActivityCategoryInfo_Catalog option:selected').text() + ' > ' + $('#ActivityCategoryInfo_Category option:selected').text()) != $('#tdCategory' + $('#ActivityCategoryInfo_Category option:selected').val()).text())
                        counter++;
                }
                if (counter == (rows + 1)) {
                    $('#tblCategoriesSelected').append(builder);
                    $('#tblCategoriesSelected').show();
                }
                //UI.tablesHoverEffect();
                //UI.tablesStripedEffect();
                ACTIVITY.makeTblCategoriesRowsRemovable();
            }
        });

        $('input:radio[name="ActivityScheduleInfo_IsSpecificTime"]').on('change', function () {
            if ($('input:radio[name="ActivityScheduleInfo_IsSpecificTime"]:checked').val() == 'True') {
                $('#divIntervalTime').hide();
                $('#divToTime').hide();
                $('#ActivityScheduleInfo_Time').hide();
            }
            else {
                $('#divIntervalTime').show();
                $('#divToTime').show();
                $('#ActivityScheduleInfo_Time').show();
            }
        });

        $('input:radio[name="ActivityScheduleInfo_IsPermanent"]').on('change', function () {
            if ($('input:radio[name="ActivityScheduleInfo_IsPermanent"]:checked').val() == 'True') {
                $('#divFromDate').hide();
                $('#divToDate').hide();
            }
            else {
                $('#divFromDate').show();
                $('#divToDate').show();
            }
        });

        $('input:checkbox[name="ActivityMeetingPointInfo_AtYourHotel"]').on('change', function () {
            if ($(this).is(':checked')) {
                $('#ActivityMeetingPointInfo_PlaceString').val('');
            }
        });
        
        $('input:checkbox[name="ActivityInfo_ExcludeForCommission"]').on('change', function () {
            if ($(this).is(':checked')) {
                $('#divJobPositions').show();
                $.getJSON('/Activities/GetDDLData', { itemID: $('#ActivityInfo_OriginalTerminal option:selected').val(), itemType: 'jpPerTerminalCommissions' }, function (data) {
                    $('#ActivityInfo_JobPositions').fillSelect(data);
                    $('#ActivityInfo_JobPositions').multiselect('refresh');
                });
            }
            else {
                $('#divJobPositions').hide();
            }
        });

        $('input:radio[name="PriceTypeRulesInfo_UsesFormula"]').on('change', function () {
            if ($('input:radio[name="PriceTypeRulesInfo_UsesFormula"]:checked').val() == 'True') {
                $('#divFormula').show();
                $('#divNoFormula').hide();
            }
            else {
                $('#divFormula').hide();
                $('#divNoFormula').show();
            }
        });

        $.getJSON('/Activities/GetDDLData', { itemID: 0, itemType: 'units' }, function (data) {
            var array = new Array();
            $.each(data, function (index, item) {
                array[index] = item.Text;
            });
            $('#PriceUnitInfo_Unit').autocomplete({
                source: array,
                minLength: 0,
                position: { my: 'right top', at: 'right bottom' },
                close: function (evt, ui) {
                    $('#PriceUnitInfo_Unit').children('option').each(function () {
                        if ($(this).text() == $('#PriceUnitInfo_Unit').val()) {
                            $(this).attr('selected', true);
                        }
                    });
                }
            });
        });

        $('#Search_Region').on('change', function () {
            UI.updateDependentLists('/Activities/GetDDLData', 'provider', $(this).val(), false, $(document), false);
        });
        //attempt to change itemType to Transportation
        $('input:radio[name="ActivityInfo_TransportationService"]').on('change', function () {
            if ($('input:radio[name="ActivityInfo_TransportationService"]:checked').val() == 'True') {
                $('#PriceInfo_ItemType').val('Transportation');
                $('#divFromZone').show();
                $('#divToZone').show();
                $('#divRoundTrip').show();
            }
            else {
                $('#PriceInfo_ItemType').val('Activities');
                $('#divFromZone').hide();
                $('#divToZone').hide();
                $('#divRoundTrip').hide();
            }
        });

        $('#btnAddMeetingPointToTable').on('click', function () {
            var _id = $('#ActivityMeetingPointInfo_DepartureTime').val().split(':').join('_') + '|' + ($('#ActivityMeetingPointInfo_PlaceString').val() != '' ? $('#ActivityMeetingPointInfo_Place').val() : 'null');
            if ($('#tblDepartureTimes tbody').find('#' + _id).length > 0) {
                UI.messageBox(-1, 'Meeting Point already exist', null, null);
            }
            else {
                var _place = '';
                if ($('#ActivityMeetingPointInfo_PlaceString').val() != '') {
                    _place = $('#ActivityMeetingPointInfo_PlaceString').val();
                }
                else if ($('#ActivityMeetingPointInfo_AtYourHotel').is(':checked')) {
                    _place = 'At Your Hotel';
                }
                if (_place != '') {
                    var builder = '<tr id="' + _id + '">'
                        + '<td>' + $('#ActivityMeetingPointInfo_WeeklySchedule option:selected').text().split(' ')[0] + '</td>'
                        + '<td>' + $('#ActivityMeetingPointInfo_DepartureTime').val() + '</td>'
                        + '<td>' + $('#ActivityMeetingPointInfo_AtYourHotel').is(':checked') + '</td>'
                        + '<td>' + _place + '</td>'
                        + '<td>' + $('#ActivityMeetingPointInfo_Notes').val() + '</td>'
                        + '<td>' + $('#ActivityMeetingPointInfo_IsActive').is(':checked') + '</td>'
                        + '<td><img src="/content/themes/base/images/trash.png" class="delete-row right"></td>'
                        + '</tr>';
                    $('#tblDepartureTimes tbody').append(builder);
                    $('#ActivityMeetingPointInfo_DepartureTimes').attr('value', $('#ActivityMeetingPointInfo_DepartureTimes').val() + _id + ',');
                }
                else {
                    UI.messageBox(-1, 'If meeting point is not at hotel, you must specify a place', null, null);
                }
            }
            ACTIVITY.deleteRowFunctionality();
        });

        $('#btnSaveMeetingPoint').on('click', function () {
            if ($('#ulActivityMeetingPoints').find('.selected-row').length == 0) {
                if ($('#tblDepartureTimes tbody tr').length > 0) {
                    $('#frmMeetingPoint').submit();
                }
                else {
                    $('#btnAddMeetingPointToTable').click();
                    $('#frmMeetingPoint').submit();
                    //UI.messageBox(0, 'There are no pending changes to save', null, null);
                }
            }
            else {
                $('#ActivityMeetingPointInfo_DepartureTimes').val('');
                $('#frmMeetingPoint').submit();
            }
        });
        //rules
        $('input:radio[name="PriceTypeRulesInfo_Base"]').on('change', function () {
            if ($('#PriceTypeRulesInfo_GenericUnit').val() == '') {
                if ($('input:radio[name="PriceTypeRulesInfo_Base"]:checked').val() == 'True') {
                    $('#divBaseDependant').hide();
                }
                else {
                    $('#divBaseDependant').show();
                }
            }
            else {
                $('input:radio[name="PriceTypeRulesInfo_Base"]')[1].checked = true;
                $('#divBaseDependant').show();
            }
        });

        $('#PriceTypeRulesInfo_Terminal').on('mousedown', function (e) {
            e.preventDefault();
        });

        $('#PriceTypeRulesInfo_PriceType').on('change', function (e) {
            if ($('#PriceTypeRulesInfo_PriceType option:selected').text().indexOf('Cost') != -1) {
                $('#PriceTypeRulesInfo_FromDate').datepicker('option', 'minDate', 'null');
            }
            else {
                $('#PriceTypeRulesInfo_FromDate').datepicker('option', 'minDate', 0);
            }
        });

        $('#PriceTypeRulesInfo_GenericUnit').on('change', function (e) {
            if ($(this).val() != '') {
                $('input:radio[name="PriceTypeRulesInfo_Base"]')[1].checked = true;
                $('input:radio[name="PriceTypeRulesInfo_Base"]').trigger('change');
            }
        });

        $('#PriceTypeRulesInfo_Service').multiselect({
            close: function () {
                if ($('#PriceTypeRulesInfo_Service').multiselect('getChecked').length != 0) {
                    $('#PriceTypeRulesInfo_GenericUnit option').each(function () {
                        $(this).show();
                    });
                }
                else {
                    $('#PriceTypeRulesInfo_GenericUnit option:not(:first)').each(function () {
                        $(this).hide();
                    });
                    $('#PriceTypeRulesInfo_GenericUnit option:first').attr('selected', true);
                    $('#PriceTypeRulesInfo_GenericUnit').trigger('change');
                }
            }
        });

        $('#PriceTypeRulesInfo_Service').multiselect('close');

        ACTIVITY.preselectTerminalOnActivityInfo();

        UI.updateListsOnTerminalsChange();

        $('#SearchRules_Date').datetimepicker({
            dateFormat: 'yy-mm-dd',
            timeFormat: 'hh:mm',
            changeMonth: true,
            changeYear: true
        });

        $('#btnSearchRules').on('click', function () {
            ACTIVITY.updateTblPriceTypeRules($('#ActivityInfo_ActivityID').val(), $('#ActivityInfo_OriginalTerminal').val(), $('#SearchRules_Date').val());
        });

        $('#btnNewAccountingAccountInfo').on('click', function () {
            $('#ActivityAccountingAccountInfo_AccountingAccount').parents('.editor-alignment:first').show();
            $('#ActivityAccountingAccountInfo_AccountingAccountString').parents('.editor-alignment:first').hide();
        });

        $('#btnDeleteAccounts').on('click', function () {
            UI.confirmBox('Do you want to proceed', deleteAccountingAccounts, []);
        });
    }

    function deleteAccountingAccounts() {
        var accounts = new Array();
        ACTIVITY.oAccountsPerActivityTable.$('tr').not('theader').each(function () {
            accounts.push($(this).attr('id').split('_')[1]);
        });
        $.ajax({
            url: '/Activities/DeleteActivityAccountingAccounts',
            cache: false,
            type: 'POST',
            data: { accountingAccounts: JSON.stringify(accounts), serviceID: $('#ActivityAccountingAccountInfo_ActivityID').val() },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    $.each(data.ItemID.accountID.split(','), function (index, item) {
                        if ($('#account_' + item).hasClass('selected-row')) {
                            var event = $.Event('keydown');
                            event.keycode = 27;
                            $(document).trigger(event);
                        }
                        ACTIVITY.oAccountsPerActivityTable.fnDeleteRow($('#account_' + item)[0]);
                    });
                    var accounts = '';
                    $.each(data.ItemID.accounts.split('|'), function (index, item) {
                        accounts += '<span class="block">' + item + '</span>';
                    });
                    accounts = accounts != '' ? accounts : '0';
                    $('#trActivity' + $('#ActivityInfo_ActivityID').val()).children('td:nth-child(6)').html(accounts);
                    ACTIVITY.makeTableRowsSelectable();
                    $('#hResults').text('0 Related Accounting Accounts');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, data.InnerException);
            }
        });
    }

    var preselectTerminalOnActivityInfo = function () {
        if (UI.selectedTerminals.split(',').length == 1) {
            $('#ActivityInfo_OriginalTerminal option[value="' + UI.selectedTerminals + '"]').attr('selected', true);
            $('#ActivityInfo_OriginalTerminal').trigger('change');
        }
    }

    var searchResultsTable = function (data) {
        UI.Notifications.workingOn("CMS > Activities");
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchActivitiesResults', tableColumns.length - 1);
        ACTIVITY.oTable = $('#tblSearchActivitiesResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { ACTIVITY.makeTableRowsSelectable(); });
        //});
        //$('#tblSearchActivitiesResults_length').unbind('change').on('change', function () {
        //    ACTIVITY.makeTableRowsSelectable();
        //});
    }

    var accountingAccountsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchAccountingAccountsResults', tableColumns.length - 1);
        ACTIVITY.oAccountingAccountsTable = $('#tblSearchAccountingAccountsResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () {
        //        ACTIVITY.makeTblAccountingAccountsRowsSelectable();
        //    });
        //});
        //$('#tblSearchAccountingAccountsResults_length').unbind('change').on('change', function () {
        //    ACTIVITY.makeAccountingAccountsRowsSelectable();
        //});
    }

    var pointsOfSaleTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchPointsOfSaleResults', tableColumns.length - 1);
        ACTIVITY.oPointsOfSaleTable = $('#tblSearchPointsOfSaleResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () {
        //        ACTIVITY.makeTblPointsOfSaleRowsSelectable();
        //    });
        //});
        //$('#tblSearchPointsOfSaleResults_length').unbind('change').on('change', function () {
        //    ACTIVITY.makeTblPointsOfSaleRowsSelectable();
        //});
    }

    var makeTableRowsSelectable = function () {
        //$('#tblSearchActivitiesResults tbody tr').not('theader').unbind('click').on('click', function (e) {
        ACTIVITY.oTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('disabled-row')) {
                    PICTURE.getItemNames(true);
                    if (!$(this).hasClass('selected-row')) {

                        ACTIVITY.oTable.$('tr.selected-row').removeClass('selected-row primary');
                        $(document).find('.selected-row').each(function () {
                            if ($(this).parents('fieldset:first').hasClass('fds-active')) {
                                var event = $.Event('keydown');
                                event.keyCode = 27;
                                $(document).trigger(event);
                            }
                        });
                        COMMON.expandAndCollapse('#fdsActivitiesInfo', '#fdsActivitiesInfo fieldset');
                        $(this).addClass('selected-row primary');
                        $('#ActivityInfo_ActivityID').val($(this).attr('id').substr(10));
                        $('#ActivityCategoryInfo_ActivityID').val($('#ActivityInfo_ActivityID').val());
                        var id = $('#ActivityInfo_ActivityID').val();
                        $('#fdsActivityCategories').show();
                        $('#fdsActivityDescriptions').show();
                        $('#fdsActivitySchedule').show();
                        $('#fdsActivityMeetingPoints').show();
                        $('#fdsAccountingAccounts').show();
                        $('#fdsPriceTypesRules').show();
                        $('#fdsPrices').show();
                        $('#fdsPricesEditor').show();
                        $('#fdsSeoItems').show();
                        $('#fdsPictures').show();
                        $('#PriceInfo_ItemID').val(id);//prices related
                        $('#PictureInfo_ItemID').val(id);//pictures related
                        $('#SeoItemInfo_ItemID').val(id);//seoItems related
                        SEO.urlText = $(this).children('td:nth-child(2)')[0].textContent.trim().split(',')[0];
                        PICTURE.getGalleryName($('#PictureInfo_ItemType').val(), $('#PictureInfo_ItemID').val());
                        $.ajax({
                            url: '/Activities/GetActivityInfo',
                            cache: false,
                            type: 'POST',
                            data: { activityID: id },
                            //beforeSend: function (xhr) {
                            //    UI.checkForPendingRequests(xhr);
                            //},
                            success: function (data) {
                                //if (data.ActivityInfo_HasSpecialExchangeRate) {
                                //    $('#divPricesCulture').show();
                                //}
                                //else {
                                //    $('#divPricesCulture').hide();
                                //}
                                $('#itemName').text(data.ActivityInfo_Activity);
                                $('#ActivityInfo_Activity').val(data.ActivityInfo_Activity);
                                $('#ActivityInfo_ItemType option[value="' + data.ActivityInfo_ItemType + '"]').attr('selected', true);
                                $('#ActivityInfo_OriginalTerminal option[value="' + data.ActivityInfo_OriginalTerminal + '"]').attr('selected', true);
                                $('#PriceInfo_Terminal').val($('#ActivityInfo_OriginalTerminal option:selected').val());
                                $('#SeoItemInfo_Terminal').val($('#ActivityInfo_OriginalTerminal option:selected').val());
                                $('#SeoItemInfo_TerminalItem option[value="' + $('#SeoItemInfo_Terminal').val() + '"]').attr('selected', true);
                                $('#ActivityInfo_OriginalTerminal').trigger('change', { destination: data.ActivityInfo_Destination, provider: data.ActivityInfo_Provider, zone: data.ActivityInfo_Zone });
                                if ($('input:radio[name=ActivityInfo_ApplyWholeStay]').length > 0) {
                                    if (data.ActivityInfo_ApplyWholeStay) {
                                        $('input:radio[name=ActivityInfo_ApplyWholeStay]')[0].checked = true;
                                    }
                                    else {
                                        $('input:radio[name=ActivityInfo_ApplyWholeStay]')[1].checked = true;
                                    }
                                }
                                $('#ActivityInfo_Length').val(data.ActivityInfo_Length);
                                if (data.ActivityInfo_TransportationService) {
                                    $('input:radio[name=ActivityInfo_TransportationService]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name=ActivityInfo_TransportationService]')[1].checked = true;
                                }
                                $('input:radio[name=ActivityInfo_TransportationService]').trigger('change');
                                if (data.ActivityInfo_OffersRoundTrip) {
                                    $('input:radio[name="ActivityInfo_OffersRoundTrip"]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name="ActivityInfo_OffersRoundTrip"]')[1].checked = true;
                                }
                                $('#ActivityInfo_MinimumAge').val(data.ActivityInfo_MinimumAge);
                                $('#ActivityInfo_MinimumHeight').val(data.ActivityInfo_MinimumHeight);
                                $('#ActivityInfo_MaximumWeight').val(data.ActivityInfo_MaximumWeight);
                                if (data.ActivityInfo_BabiesAllowed) {
                                    $('input:radio[name=ActivityInfo_BabiesAllowed]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name=ActivityInfo_BabiesAllowed]')[1].checked = true;
                                }
                                if (data.ActivityInfo_ChildrenAllowed) {
                                    $('input:radio[name=ActivityInfo_ChildrenAllowed]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name=ActivityInfo_ChildrenAllowed]')[1].checked = true;
                                }
                                if (data.ActivityInfo_AdultsAllowed) {
                                    $('input:radio[name=ActivityInfo_AdultsAllowed]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name=ActivityInfo_AdultsAllowed]')[1].checked = true;
                                }
                                if (data.ActivityInfo_PregnantsAllowed) {
                                    $('input:radio[name=ActivityInfo_PregnantsAllowed]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name=ActivityInfo_PregnantsAllowed]')[1].checked = true;
                                }
                                if (data.ActivityInfo_OldiesAllowed) {
                                    $('input:radio[name=ActivityInfo_OldiesAllowed]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name=ActivityInfo_OldiesAllowed]')[1].checked = true;
                                }
                                $('#ActivityInfo_Video').val(data.ActivityInfo_Video);
                                $('#ActivityInfo_VideoURL').val(data.ActivityInfo_VideoURL);
                                $('input:checkbox[name="ActivityInfo_ExcludeForCommission"]')[0].checked = data.ActivityInfo_ExcludeForCommission;
                                $('input:checkbox[name="ActivityInfo_ExcludeForCommission"]').trigger('change');
                                $('input:checkbox[name="ActivityInfo_AvoidRounding"]')[0].checked = data.ActivityInfo_AvoidRounding;
                                var builderCategories = '';
                                ACTIVITY.categories = '';
                                $('#tblCategoriesSelected tbody').empty();
                                $.each(data.ActivityInfo_ListCategories, function (index, item) {
                                    builderCategories += '<tr><td id="tdCategory' + item.Key + '">' + item.Value + '</td><td>'
                                + '<img src="/Content/themes/base/images/trash.png" class="right"></td></tr>';
                                    ACTIVITY.categories += '<span class="block">' + item.Value + '</span>';
                                    //ACTIVITY.categories += (ACTIVITY.categories != '' ? ', ' : '') + item.Value;
                                });
                                if (builderCategories != '') {
                                    $('#tblCategoriesSelected').show();
                                    $('#tblCategoriesSelected').append(builderCategories);
                                    //UI.tablesStripedEffect();
                                    //UI.tablesHoverEffect();
                                    ACTIVITY.makeTblCategoriesRowsRemovable();
                                }
                                ACTIVITY.updateUlActivityDescriptions(id);
                                ACTIVITY.updateUlActivityAvailabilities(id);
                                ACTIVITY.destination = data.ActivityInfo_Destination;
                                ACTIVITY.updateUlActivityMeetingPoints(id, data.ActivityInfo_OriginalTerminal);
                                ACTIVITY.updateTblActivityAccountingAccounts(id);
                                $('#PriceTypeRulesInfo_SelectedService').val(id);

                                if ($('#fdsPriceTypesRules').length > 0) {
                                    ACTIVITY.updateTblPriceTypeRules(id, data.ActivityInfo_OriginalTerminal, null);
                                }
                                if (data.ActivityInfo_HasSpecialExchangeRate) {
                                    $('#divPricesCulture').show();
                                    $('#PriceInfo_Culture').addClass('special-rate');
                                    PRICE.updateTblPrices(undefined, true);
                                }
                                else {
                                    $('#divPricesCulture').hide();
                                    $('#PriceInfo_Culture').removeClass('special-rate');
                                    PRICE.updateTblPrices(undefined, false);
                                }
                                //PRICE.updateTblPrices();

                                //Load Prices Editor
                                PricesEditor.init(id, data.ActivityInfo_HasSpecialExchangeRate, data.ActivityInfo_OriginalTerminal);

                                SEO.updateTblSeoItems();
                                PICTURE.GetImagesPerItemType($('#PictureInfo_ItemType').val(), $('#PictureInfo_ItemID').val());
                                SEO.updateSeoItemRelatedLists();
                                UI.expandFieldset('fdsActivitiesInfo');
                                UI.scrollTo('fdsActivitiesInfo', null);

                                UI.Notifications.workingOn("CMS > Activities > " + data.ActivityInfo_Activity + " (" + id + ")");
                                if (data.ActivityInfo_ItemType == 4) {
                                    $.getJSON('/Activities/GetStockBalance', { serviceID: data.ActivityInfo_ActivityID }, function (response) {
                                        $('#fdsStockTransactions').show();
                                        $('#SearchStockTransactions_Stock').val(response.stockID);
                                        $('#StockInfo_StockID').val(response.stockID);
                                        $('#StockTransactionInfo_Stock').val(response.stockID);
                                        $('#StockInfo_Service').val(data.ActivityInfo_ActivityID);
                                        $('#StockTransactionInfo_Service').val(data.ActivityInfo_ActivityID);
                                        $('#StockInfo_Quantity').val(response.quantity);
                                        $('#StockInfo_MinimalStock').val(response.stock);
                                        $('#btnSearchStockTransactions').click();
                                    });
                                    if ($('input:radio[name="StockTransactionInfo_Ingress"]')[1] == undefined) {
                                        $('input:radio[name="StockTransactionInfo_Ingress"]')[0].checked = true;
                                    }
                                }
                                else {
                                    $('#fdsStockTransactions').hide();
                                    $('#SearchStockTransactions_Stock').val('');
                                    $('#StockInfo_StockID').val('');
                                    $('#StockTransactionInfo_Stock').val('');
                                    $('#StockInfo_Service').val('');
                                    $('#StockTransactionInfo_Service').val('');
                                    $('#StockInfo_Quantity').val('');
                                    $('#StockInfo_MinimalStock').val('');
                                }
                            }
                        });
                    }
                }
            }
            else {
                if ($(e.target).hasClass('revert')) {
                    UI.confirmBox('Do you confirm you want to proceed?', restoreActivity, [$(e.target).attr('id').substr(4)]);
                }
                else {
                    UI.confirmBox('Do you confirm you want to proceed?', deleteActivity, [$(e.target).attr('id').substr(4)]);
                }
            }
        });
    }

    var updateUlActivityDescriptions = function (activityID) {
        $('#ulActivityDescriptions').empty();
        $('#ActivityDescriptionInfo_ActivityID').val(activityID);
        $.ajax({
            url: '/Activities/GetActivityDescriptions',
            cache: false,
            type: 'POST',
            data: { activityID: activityID },
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="liActivityDescription' + item.ActivityDescriptionInfo_ActivityDescriptionID + '">' + $('#ActivityDescriptionInfo_Culture option[value="' + item.ActivityDescriptionInfo_Culture + '"]').text() + '<img id="delActivityDescription' + item.ActivityDescriptionInfo_ActivityDescriptionID + '" src="/Content/themes/base/images/trash.png" class="right" /></li>';
                    });
                    $('#ulActivityDescriptions').append(builder);
                    UI.ulsHoverEffect('ulActivityDescriptions');
                    ACTIVITY.makeUlActivityDescriptionsRowsSelectable();
                }
                else {
                    builder += '<li id="liNoDescriptions">No Descriptions</li>';
                    $('#ulActivityDescriptions').append(builder);
                }
            }
        });
    }

    var updateUlActivityAvailabilities = function (activityID) {
        $('#ulActivitySchedules').empty();
        $('#ulSchedulesHeader').remove();
        $('#ActivityScheduleInfo_ActivityID').val(activityID);
        $.ajax({
            url: '/Activities/GetActivitySchedules',
            cache: false,
            type: 'POST',
            data: { activityID: activityID },
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var builder = '';
                if (data.length > 0) {
                    var headerBuilder = '<div id="ulSchedulesHeader" class="left-ul-header-alignment"><div class="weekdays-alignment"><span>ID</span></div>'
                    + '<div class="weekdays-alignment"><span>Mon</span></div>'
                    + '<div class="weekdays-alignment"><span>Tue</span></div>'
                    + '<div class="weekdays-alignment"><span>Wed</span></div>'
                    + '<div class="weekdays-alignment"><span>Thu</span></div>'
                    + '<div class="weekdays-alignment"><span>Fri</span></div>'
                    + '<div class="weekdays-alignment"><span>Sat</span></div>'
                    + '<div class="weekdays-alignment"><span>Sun</span></div>'
                    + '<div class="weekdays-alignment" style="padding-left:15px"><span>Time</span></div></div>';
                    $.each(data, function (index, item) {
                        builder += '<li id="liActivitySchedule' + item.ActivityScheduleInfo_ActivityScheduleID + '">'
                        + '<div class="weekdays-alignment"><span>' + item.ActivityScheduleInfo_ActivityScheduleID + '</span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                        if (item.ActivityScheduleInfo_Monday)
                            builder += ' checked="' + item.ActivityScheduleInfo_Monday + '"';
                        builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                        if (item.ActivityScheduleInfo_Tuesday)
                            builder += ' checked="' + item.ActivityScheduleInfo_Tuesday + '"';
                        builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                        if (item.ActivityScheduleInfo_Wednesday)
                            builder += ' checked="' + item.ActivityScheduleInfo_Wednesday + '"';
                        builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                        if (item.ActivityScheduleInfo_Thursday)
                            builder += ' checked="' + item.ActivityScheduleInfo_Thursday + '"';
                        builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                        if (item.ActivityScheduleInfo_Friday)
                            builder += ' checked="' + item.ActivityScheduleInfo_Friday + '"';
                        builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                        if (item.ActivityScheduleInfo_Saturday)
                            builder += ' checked="' + item.ActivityScheduleInfo_Saturday + '"';
                        builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                        if (item.ActivityScheduleInfo_Sunday)
                            builder += ' checked="' + item.ActivityScheduleInfo_Sunday + '"';
                        builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span>' + item.ActivityScheduleInfo_FromTime + '</span></div>';
                        //if time is a range
                        if (item.ActivityScheduleInfo_IsSpecificTime == false) {
                            builder += '<div class="weekdays-alignment"><span> - ' + item.ActivityScheduleInfo_ToTime + '</span></div>';
                        }
                        else {
                            builder += '<div class="weekdays-alignment"><span> </span></div>';
                        }
                        builder += '<img id="delActivitySchedule' + item.ActivityScheduleInfo_ActivityScheduleID + '" src="/Content/themes/base/images/trash.png" class="right" />'
                        + '</li>';
                    });
                    $(headerBuilder).insertBefore($('#ulActivitySchedules'));
                    $('#ulActivitySchedules').append(builder);
                    UI.ulsHoverEffect('ulActivitySchedules');
                    ACTIVITY.makeUlActivitySchedulesRowsSelectable();
                }
                else {
                    builder += '<li id="liNoSchedules">No Schedules</li>';
                    $('#ulActivitySchedules').append(builder);
                }
            }
        });
    }

    var updateUlActivityMeetingPoints = function (activityID, destinationID) {
        $('#ulActivityMeetingPoints').empty();
        $('#ulPointsHeader').remove();
        $('#ActivityMeetingPointInfo_ActivityID').val(activityID);
        $.ajax({
            url: '/Activities/GetActivityMeetingPoints',
            cache: false,
            type: 'POST',
            data: { activityID: activityID },
            success: function (data) {
                var builder = '';
                if (data.length > 0) {
                    var headerPointsBuilder = '<div id="ulPointsHeader" class="left-ul-header-alignment">'
                    + '<div style="display:inline-block;width:70px"><span>Time</span></div>'
                    + '<div style="display:inline-block;width:250px"><span>Place</span></div>'
                    + '<div style="display:inline-block;width:100px"><span>Notes</span></div>'
                    + '<div style="display:inline-block;width:100px"><span>At Your Hotel</span></div>'
                    + '<div style="display:inline-block;width:100px"><span>Is Active</span></div></div>'
                    $.each(data, function (index, item) {
                        builder += '<li id="liMeetingPoint_' + item.ActivityMeetingPointInfo_ActivityMeetingPointID + '">'
                        + '<div style="display:inline-block;width:70px"><span>' + item.ActivityMeetingPointInfo_DepartureTime + '</span></div>'
                        + '<div style="display:inline-block;width:250px"><span>' + item.ActivityMeetingPointInfo_PlaceString + '</span></div>'
                        + '<div style="display:inline-block;width:100px"><span>' + item.ActivityMeetingPointInfo_Notes + '</span></div>'
                        + '<div style="display:inline-block;width:100px"><span><input type="checkbox" disabled="disabled"';
                        if (item.ActivityMeetingPointInfo_AtYourHotel)
                            builder += 'checked="' + item.ActivityMeetingPointInfo_AtYourHotel + '"';
                        builder += '/></span></div>'
                        + '<div style="display:inline-block;width:100px"><span><input type="checkbox" disabled="disabled"';
                        if (item.ActivityMeetingPointInfo_IsActive)
                            builder += 'checked="' + item.ActivityMeetingPointInfo_IsActive + '"';
                        builder += '/></span></div>'
                        + '<img id="delMeetingPoint' + item.ActivityMeetingPointInfo_ActivityMeetingPointID + '" src="/Content/themes/base/images/trash.png" class="right" />'
                        + '</li>';
                    });
                    $(headerPointsBuilder).insertBefore($('#ulActivityMeetingPoints'));
                    $('#ulActivityMeetingPoints').append(builder);
                    UI.ulsHoverEffect('ulActivityMeetingPoints');
                    ACTIVITY.makeUlActivityMeetingPointsRowsSelectable();
                }
                else {
                    builder += '<li id="liNoMeetingPoints">No Meeting Points</li>';
                    $('#ulActivityMeetingPoints').append(builder);
                }
                ACTIVITY.updateDrpsActivityDependents(activityID, destinationID);
            }
        });
        //$('#ulActivityMeetingPoints').empty();
        //$('#ulPointsHeader').remove();
        //$('#ActivityMeetingPointInfo_ActivityID').val(activityID);
        //$.ajax({
        //    url: '/Activities/GetActivityMeetingPoints',
        //    cache: false,
        //    type: 'POST',
        //    data: { activityID: activityID },
        //    //beforeSend: function (xhr) {
        //    //    UI.checkForPendingRequests(xhr);
        //    //},
        //    success: function (data) {
        //        var builder = '';
        //        if (data.length > 0) {
        //            var headerPointsBuilder = '<div id="ulPointsHeader" class="left-ul-header-alignment">'
        //            + '<div style="display:inline-block;width:70px"><span>Time</span></div>'
        //            + '<div style="display:inline-block;width:250px"><span>Place</span></div>'
        //            //+ '<div style="display:inline-block;width:150px"><span>Destination</span></div>'
        //            + '<div style="display:inline-block;width:100px"><span>Notes</span></div>'
        //            + '<div style="display:inline-block;width:100px"><span>At Your Hotel</span></div>'
        //            + '<div style="display:inline-block;width:100px"><span>Is Active</span></div></div>'
        //            $.each(data, function (index, item) {
        //                builder += '<li id="liMeetingPoint' + item.ActivityMeetingPointInfo_ActivityMeetingPointID + '">'
        //                + '<div style="display:inline-block;width:70px"><span>' + item.ActivityMeetingPointInfo_DepartureTime + '</span></div>'
        //                + '<div style="display:inline-block;width:250px"><span>' + item.ActivityMeetingPointInfo_PlaceString + '</span></div>'
        //                //+ '<div style="display:inline-block;width:150px"><span>' + item.ActivityMeetingPointInfo_Destination + '</span></div>'
        //                + '<div style="display:inline-block;width:100px"><span>' + item.ActivityMeetingPointInfo_Notes + '</span></div>'
        //                + '<div style="display:inline-block;width:100px"><span><input type="checkbox" disabled="disabled"';
        //                if (item.ActivityMeetingPointInfo_AtYourHotel)
        //                    builder += 'checked="' + item.ActivityMeetingPointInfo_AtYourHotel + '"';
        //                builder += '/></span></div>'
        //                + '<div style="display:inline-block;width:100px"><span><input type="checkbox" disabled="disabled"';
        //                if (item.ActivityMeetingPointInfo_IsActive)
        //                    builder += 'checked="' + item.ActivityMeetingPointInfo_IsActive + '"';
        //                builder += '/></span></div>'
        //                + '<img id="delMeetingPoint' + item.ActivityMeetingPointInfo_ActivityMeetingPointID + '" src="/Content/themes/base/images/trash.png" class="right" />'
        //                + '</li>';
        //            });
        //            $(headerPointsBuilder).insertBefore($('#ulActivityMeetingPoints'));
        //            $('#ulActivityMeetingPoints').append(builder);
        //            UI.ulsHoverEffect('ulActivityMeetingPoints');
        //            ACTIVITY.makeUlActivityMeetingPointsRowsSelectable();
        //        }
        //        else {
        //            builder += '<li id="liNoMeetingPoints">No Meeting Points</li>';
        //            $('#ulActivityMeetingPoints').append(builder);
        //        }
        //        ACTIVITY.updateDrpsActivityDependents(activityID, destinationID);
        //    }
        //});
    }

    var updateTblActivityAccountingAccounts = function (activityID) {
        $('#ActivityAccountingAccountInfo_ActivityID').val(activityID);
        $.ajax({
            url: '/Activities/GetActivityAccountingAccounts',
            cache: false,
            type: 'POST',
            data: { activityID: activityID },
            success: function (data) {
                $('#divAccountingAccountsPerActivity').html(data);
                var tableColumns = $(data).find('tbody tr').first().find('td');
                $('#hResults').text($(data).find('tbody tr').length + ' Related Accounting Accounts');
                UI.searchResultsTable('tblActivityAccountingAccounts', tableColumns.length - 1);
                ACTIVITY.oAccountsPerActivityTable = $('#tblActivityAccountingAccounts').dataTable();
                ACTIVITY.makeTblActivityAccountingAccountsRowsSelectable();
            }
        });
    }

    var updateTableAccountingAccounts = function () {
        $.ajax({
            url: '/Activities/GetAccountingAccounts',
            cache: false,
            type: 'POST',
            beforeSend: function (xhr) {
                UI.checkForPendingRequests(xhr);
            },
            success: function (data) {
                $('#divTblExistingAccountingAccounts').html(data);
                ACTIVITY.accountingAccountsTable($('#divTblExistingAccountingAccounts'));
                ACTIVITY.makeTblAccountingAccountsRowsSelectable();
            }
        });
    }

    var updateTablePointsOfSale = function () {
        $.ajax({
            url: '/Activities/GetPointsOfSale',
            cache: false,
            type: 'POST',
            beforeSend: function (xhr) {
                UI.checkForPendingRequests(xhr);
            },
            success: function (data) {
                $('#divTblExistingPointsOfSale').html(data);
                ACTIVITY.pointsOfSaleTable($('#divTblExistingPointsOfSale'));
                ACTIVITY.makeTblPointsOfSaleRowsSelectable();
            }
        });
    }

    function restoreActivity(activityID) {
        $.ajax({
            url: '/Activities/RestoreActivity',
            cache: false,
            type: 'POST',
            data: { activityID: activityID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    ACTIVITY.oTable.$('#trActivity' + activityID).removeClass('disabled-row');
                    ACTIVITY.oTable.$('#trActivity' + activityID).children('td:last').html('<img src="/Content/themes/base/images/trash.png" id="delA' + activityID + '" class="right" />');
                    $('#PriceTypeRulesInfo_Service').append('<option value="' + activityID + '">' + ACTIVITY.oTable.$('#trActivity' + activityID).children('td:nth-child(3)').text().trim() + '</option>');// option[value="' + activityID + '"]').remove();
                    $('#PriceTypeRulesInfo_Service').multiselect('refresh');
                    ACTIVITY.makeTableRowsSelectable();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteActivity(activityID) {
        $.ajax({
            url: '/Activities/DeleteActivity',
            cache: false,
            type: 'POST',
            data: { activityID: activityID },
            beforeSend: function (xhr) {
                UI.checkForPendingRequests(xhr);
            },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#trActivity' + activityID).hasClass('selected-row')) {
                        $(document).find('.selected-row').each(function () {
                            var event = $.Event('keydown');
                            event.keyCode = 27;
                            $(document).trigger(event);
                        });
                    }
                    //ACTIVITY.oTable.fnDeleteRow($('#trActivity' + activityID)[0]);
                    ACTIVITY.oTable.$('#trActivity' + activityID).addClass('disabled-row');
                    ACTIVITY.oTable.$('#trActivity' + activityID).children('td:last').html('<img src="/Content/themes/base/images/revert-icon.png" id="delA' + activityID + '" class="right revert" />'
                        + '<span class="block">Deleted by: ' + $('#ufirstname').val() + ' ' + $('#ulastname').val() + '</span>'
                        + '<span class="block">Date deleted: ' + COMMON.getDate(undefined, true) + '</span>');
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    $('#PriceTypeRulesInfo_Service option[value="' + activityID + '"]').remove();
                    $('#PriceTypeRulesInfo_Service').multiselect('refresh');
                    ACTIVITY.makeTableRowsSelectable();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteActivityDescription(descriptionID) {
        $.ajax({
            url: '/Activities/DeleteActivityDescription',
            cache: false,
            type: 'POST',
            data: { activityDescriptionID: descriptionID },
            beforeSend: function (xhr) {
                UI.checkForPendingRequests(xhr);
            },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liActivityDescription' + descriptionID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liActivityDescription' + data.ItemID).remove();
                    if ($('#ulActivityDescriptions li').length == 0) {
                        $('#ulActivityDescriptions').append('<li id="liNoDescriptions">No Descriptions</li>');
                    }
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteActivitySchedule(scheduleID) {
        $.ajax({
            url: '/Activities/DeleteActivitySchedule',
            cache: false,
            type: 'POST',
            data: { activityScheduleID: scheduleID },
            beforeSend: function (xhr) {
                UI.checkForPendingRequests(xhr);
            },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liActivitySchedule' + scheduleID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liActivitySchedule' + data.ItemID).remove();
                    $('#ActivityMeetingPointInfo_WeeklySchedule option[value="' + data.ItemID + '"]').remove();
                    if ($('#ulActivitySchedules li').length == 0)
                        $('#ulActivitySchedules').append('<li id="liNoSchedules">No Schedules</li>');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteActivityMeetingPoint(meetingPointID) {
        $.ajax({
            url: '/Activities/DeleteActivityMeetingPoint',
            cache: false,
            type: 'POST',
            data: { activityMeetingPointID: meetingPointID },
            beforeSend: function (xhr) {
                UI.checkForPendingRequests(xhr);
            },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liMeetingPoint_' + meetingPointID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liMeetingPoint_' + data.ItemID).remove();
                    if ($('#ulActivityMeetingPoints li').length == 0)
                        $('#ulActivityMeetingPoints').append('<li id="liNoMeetingPoints">No Meeting Points</li>');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteActivityAccountingAccount(accountingAccountID) {
        $.ajax({
            url: '/Activities/DeleteActivityAccountingAccount',
            cache: false,
            type: 'POST',
            data: { accountingAccountID: accountingAccountID, serviceID: $('#ActivityAccountingAccountInfo_ActivityID').val() },
            beforeSend: function (xhr) {
                UI.checkForPendingRequests(xhr);
            },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#account_' + data.ItemID.accountID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keycode = 27;
                        $(document).trigger(event);
                    }
                    ACTIVITY.oAccountsPerActivityTable.fnDeleteRow($('#account_' + data.ItemID.accountID)[0]);
                    var accounts = '';
                    $.each(data.ItemID.accounts.split('|'), function (index, item) {
                        accounts += '<span class="block">' + item + '</span>';
                    });
                    accounts = accounts != '' ? accounts : '0';
                    $('#trActivity' + $('#ActivityInfo_ActivityID').val()).children('td:nth-child(6)').html(accounts);
                    ACTIVITY.makeTableRowsSelectable();
                    $('#hResults').text(ACTIVITY.oAccountsPerActivityTable.$('tbody tr').length + ' Related Accounting Accounts');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, data.InnerException);
            }
        });
    }

    var makeTblCategoriesRowsRemovable = function () {
        $('#tblCategoriesSelected tbody tr').not('theader').on('click', function (e) {
            if ($(e.target).is('img')) {
                $(this).remove();
                //UI.tablesStripedEffect();
            }
        });
    }

    var makeUlActivityDescriptionsRowsSelectable = function () {
        $('#ulActivityDescriptions li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(e.target).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $('#ActivityDescriptionInfo_ActivityDescriptionID').val($(e.target).attr('id').substr(21));
                    $.ajax({
                        url: '/Activities/GetActivityDescription',
                        cache: false,
                        type: 'POST',
                        data: { activityDescriptionID: $('#ActivityDescriptionInfo_ActivityDescriptionID').val() },
                        beforeSend: function (xhr) {
                            UI.checkForPendingRequests(xhr);
                        },
                        success: function (data) {
                            $('#ActivityDescriptionInfo_ActivityID').val($('#ActivityInfo_ActivityID').val());
                            $('#ActivityDescriptionInfo_Activity').val(data.ActivityDescriptionInfo_Activity);
                            $('#ActivityDescriptionInfo_ShortDescription').val(data.ActivityDescriptionInfo_ShortDescription);
                            $('#ActivityDescriptionInfo_FullDescription').val(data.ActivityDescriptionInfo_FullDescription);
                            $('#ActivityDescriptionInfo_Itinerary').val(data.ActivityDescriptionInfo_Itinerary);
                            $('#ActivityDescriptionInfo_Includes').val(data.ActivityDescriptionInfo_Includes);
                            $('#ActivityDescriptionInfo_Notes').val(data.ActivityDescriptionInfo_Notes);
                            $('#ActivityDescriptionInfo_Recommendations').val(data.ActivityDescriptionInfo_Recommendations);
                            $('#ActivityDescriptionInfo_Policies').val(data.ActivityDescriptionInfo_Policies);
                            $('#ActivityDescriptionInfo_CancelationPolicies').val(data.ActivityDescriptionInfo_CancelationPolicies);
                            $('#ActivityDescriptionInfo_Culture option[value="' + data.ActivityDescriptionInfo_Culture + '"]').attr('selected', true);
                            if (data.ActivityDescriptionInfo_IsActive == true) {
                                $('input:radio[name="ActivityDescriptionInfo_IsActive"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="ActivityDescriptionInfo_IsActive"]')[1].checked = true;
                            }
                            $('#ActivityDescriptionInfo_SeoItem option[value="' + data.ActivityDescriptionInfo_SeoItem + '"]').attr('selected', true);
                            $('#ActivityDescriptionInfo_Tag').val(data.ActivityDescriptionInfo_Tag);
                            UI.expandFieldset('fdsActivityDescriptionInfo');
                            UI.scrollTo('fdsActivityDescriptionInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteActivityDescription, [$(e.target).attr('id').substr(22)]);
            }
        });
    }

    var makeUlActivitySchedulesRowsSelectable = function () {
        $('#ulActivitySchedules li').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(e.target).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $('#ActivityScheduleInfo_ActivityScheduleID').val($(this).attr('id').substr(18));
                    $.ajax({
                        url: '/Activities/GetActivitySchedule',
                        cache: false,
                        type: 'POST',
                        data: { activityScheduleID: $('#ActivityScheduleInfo_ActivityScheduleID').val() },
                        beforeSend: function (xhr) {
                            UI.checkForPendingRequests(xhr);
                        },
                        success: function (data) {
                            $('#ActivityScheduleInfo_ActivityID').val($('#ActivityInfo_ActivityID').val());
                            $('input[name="ActivityScheduleInfo_Monday"]').attr('checked', data.ActivityScheduleInfo_Monday);
                            $('input[name="ActivityScheduleInfo_Tuesday"]').attr('checked', data.ActivityScheduleInfo_Tuesday);
                            $('input[name="ActivityScheduleInfo_Wednesday"]').attr('checked', data.ActivityScheduleInfo_Wednesday);
                            $('input[name="ActivityScheduleInfo_Thursday"]').attr('checked', data.ActivityScheduleInfo_Thursday);
                            $('input[name="ActivityScheduleInfo_Friday"]').attr('checked', data.ActivityScheduleInfo_Friday);
                            $('input[name="ActivityScheduleInfo_Saturday"]').attr('checked', data.ActivityScheduleInfo_Saturday);
                            $('input[name="ActivityScheduleInfo_Sunday"]').attr('checked', data.ActivityScheduleInfo_Sunday);
                            if (data.ActivityScheduleInfo_IsPermanent)
                                $('input:radio[name="ActivityScheduleInfo_IsPermanent"]')[0].checked = true;
                            else
                                $('input:radio[name="ActivityScheduleInfo_IsPermanent"]')[1].checked = true;
                            $('input:radio[name="ActivityScheduleInfo_IsPermanent"]').trigger('change');
                            $('#ActivityScheduleInfo_FromDate').val(data.ActivityScheduleInfo_FromDate);
                            $('#ActivityScheduleInfo_ToDate').val(data.ActivityScheduleInfo_ToDate);
                            if (data.ActivityScheduleInfo_IsSpecificTime) {
                                $('input:radio[name="ActivityScheduleInfo_IsSpecificTime"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="ActivityScheduleInfo_IsSpecificTime"]')[1].checked = true;
                            }
                            $('input:radio[name="ActivityScheduleInfo_IsSpecificTime"]').trigger('change');
                            $('#ActivityScheduleInfo_FromTime').val(data.ActivityScheduleInfo_FromTime);
                            $('#ActivityScheduleInfo_ToTime').val(data.ActivityScheduleInfo_ToTime);
                            $('#ActivityScheduleInfo_IntervalTime').val(data.ActivityScheduleInfo_IntervalTime);
                            UI.expandFieldset('fdsActivityScheduleInfo');
                            UI.scrollTo('fdsActivityScheduleInfo', null);
                        }
                    });
                }
            }
            else
                UI.confirmBox('Do you confirm you want to proceed?', deleteActivitySchedule, [$(e.target).attr('id').substr(19)]);
        });
    }

    var makeUlActivityMeetingPointsRowsSelectable = function () {
        $('#ulActivityMeetingPoints li').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(this).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $('#ActivityMeetingPointInfo_ActivityMeetingPointID').val($(this).attr('id').split('_')[1]);
                    $.ajax({
                        url: '/Activities/GetActivityMeetingPoint',
                        cache: false,
                        type: 'POST',
                        data: { activityMeetingPointID: $('#ActivityMeetingPointInfo_ActivityMeetingPointID').val() },
                        success: function (data) {
                            $('#tblDepartureTimes tbody').empty();
                            $('#ActivityMeetingPointInfo_ActivityID').val($('#ActivityInfo_ActivityID').val());
                            $('#ActivityMeetingPointInfo_DepartureTime').val(data.ActivityMeetingPointInfo_DepartureTime);
                            $('#ActivityMeetingPointInfo_WeeklySchedule option[value="' + data.ActivityMeetingPointInfo_WeeklySchedule + '"]').attr('selected', true);
                            $('#ActivityMeetingPointInfo_PlaceString').val(data.ActivityMeetingPointInfo_PlaceString);
                            $('#ActivityMeetingPointInfo_Place option:[value="' + data.ActivityMeetingPointInfo_Place + '"]').attr('selected', true);
                            $('input:checkbox[name="ActivityMeetingPointInfo_AtYourHotel"]').attr('checked', data.ActivityMeetingPointInfo_AtYourHotel);
                            $('input:checkbox[name="ActivityMeetingPointInfo_IsActive"]').attr('checked', data.ActivityMeetingPointInfo_IsActive);
                            $('#ActivityMeetingPointInfo_Notes').val(data.ActivityMeetingPointInfo_Notes);
                            UI.expandFieldset('fdsMeetingPointInfo');
                            UI.scrollTo('fdsMeetingPointInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteActivityMeetingPoint, [$(e.target).attr('id').substr(15)]);
            }
        });
        //$('#ulActivityMeetingPoints li').unbind('click').on('click', function (e) {
        //    if (!$(e.target).is('img')) {
        //        if (!$(this).hasClass('selected-row')) {
        //            $(this).parent('ul').find('.selected-row').removeClass('selected-row secondary');
        //            $(this).addClass('selected-row secondary');
        //            $('#ActivityMeetingPointInfo_ActivityMeetingPointID').val($(this).attr('id').substr(14));
        //            $.ajax({
        //                url: '/Activities/GetActivityMeetingPoint',
        //                cache: false,
        //                type: 'POST',
        //                data: { activityMeetingPointID: $('#ActivityMeetingPointInfo_ActivityMeetingPointID').val() },
        //                beforeSend: function (xhr) {
        //                    UI.checkForPendingRequests(xhr);
        //                },
        //                success: function (data) {
        //                    $('#divButtonContainer').hide();
        //                    $('#divTableContainer').hide();
        //                    $('#tblDepartureTimes tbody').empty();
        //                    $('#ActivityMeetingPointInfo_ActivityID').val($('#ActivityInfo_ActivityID').val());
        //                    $('#ActivityMeetingPointInfo_DepartureTime').val(data.ActivityMeetingPointInfo_DepartureTime);
        //                    $('#ActivityMeetingPointInfo_WeeklySchedule option[value="' + data.ActivityMeetingPointInfo_WeeklySchedule + '"]').attr('selected', true);
        //                    $('#ActivityMeetingPointInfo_PlaceString').val(data.ActivityMeetingPointInfo_PlaceString);
        //                    $('#ActivityMeetingPointInfo_Place option:[value="' + data.ActivityMeetingPointInfo_Place + '"]').attr('selected', true);
        //                    if (data.ActivityMeetingPointInfo_AtYourHotel) {
        //                        $('input:radio[name="ActivityMeetingPointInfo_AtYourHotel"]')[0].checked = true;
        //                    }
        //                    else {
        //                        $('input:radio[name="ActivityMeetingPointInfo_AtYourHotel"]')[1].checked = true;
        //                    }
        //                    if (data.ActivityMeetingPointInfo_IsActive) {
        //                        $('input:radio[name="ActivityMeetingPointInfo_IsActive"]')[0].checked = true;
        //                    }
        //                    else {
        //                        $('input:radio[name="ActivityMeetingPointInfo_IsActive"]')[1].checked = true;
        //                    }
        //                    $('input:radio[name="ActivityMeetingPointInfo_AtYourHotel"]').trigger('change');
        //                    $('#ActivityMeetingPointInfo_Notes').val(data.ActivityMeetingPointInfo_Notes);
        //                    UI.expandFieldset('fdsMeetingPointInfo');
        //                    UI.scrollTo('fdsMeetingPointInfo', null);
        //                }
        //            });
        //        }
        //    }
        //    else {
        //        UI.confirmBox('Do you confirm you want to proceed?', deleteActivityMeetingPoint, [$(e.target).attr('id').substr(15)]);
        //    }
        //});
    }

    var makeTblActivityAccountingAccountsRowsSelectable = function () {
        ACTIVITY.oAccountsPerActivityTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    ACTIVITY.oAccountsPerActivityTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    var id = $(this).attr('id').split('_')[1];
                    $.ajax({
                        url: '/Activities/GetActivityAccountingAccount',
                        cache: false,
                        type: 'POST',
                        data: { accountingAccountID: id, serviceID: $('#ActivityAccountingAccountInfo_ActivityID').val() },
                        //beforeSend: function (xhr) {
                        //    UI.checkForPendingRequests(xhr);
                        //},
                        success: function (data) {
                            $('#ActivityAccountingAccountInfo_ActivityAccountingAccountID').val(data.ActivityAccountingAccountInfo_ActivityAccountingAccountID);
                            $('#ActivityAccountingAccountInfo_AccountingAccount').multiselect('uncheckAll');
                            $('#ActivityAccountingAccountInfo_AccountingAccount option[value="' + data.ActivityAccountingAccountInfo_AccountingAccount[0] + '"]').attr('selected', true);
                            $('#ActivityAccountingAccountInfo_AccountingAccount').multiselect('refresh');
                            $('#ActivityAccountingAccountInfo_AccountingAccount').parents('.editor-alignment:first').hide();
                            $('#ActivityAccountingAccountInfo_AccountingAccountString').text(data.ActivityAccountingAccountInfo_AccountingAccountString);
                            $('#ActivityAccountingAccountInfo_AccountingAccountString').parents('.editor-alignment:first').show();
                            $('#ActivityAccountingAccountInfo_PointOfSale option:selected').each(function () {
                                $(this).removeAttr('selected');
                            });
                            $('#ActivityAccountingAccountInfo_PointOfSale').multiselect('uncheckAll');
                            if (data.ActivityAccountingAccountInfo_PointOfSale != null) {
                                $.each(data.ActivityAccountingAccountInfo_PointOfSale, function (index, item) {
                                    $('#ActivityAccountingAccountInfo_PointOfSale option[value="' + item + '"]').attr('selected', true);
                                });
                                $('#ActivityAccountingAccountInfo_PointOfSale').multiselect('refresh');
                            }
                            UI.expandFieldset('fdsAccountingAccountInfo');
                            UI.scrollTo('fdsAccountingAccountInfo', null);
                        }
                    });
                }
            }
            else {
                //UI.confirmBox('Do you confirm you want to proceed?', deleteActivityAccountingAccount, [$(e.target).parents('li:first').attr('id').substr(19)]);
                UI.confirmBox('Do you confirm you want to proceed?', deleteActivityAccountingAccount, [$(this).attr('id').split('_')[1]]);
            }
        });
    }

    var makeTblAccountingAccountsRowsSelectable = function () {
        $('#tblSearchAccountingAccountsResults tbody tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).parent().hasClass('selected-row')) {
                    var id = $(e.target).parent().attr('id').substr($(e.target).parent().attr('id').indexOf('_') + 1, $(e.target).parent().attr('id').length - $(e.target).parent().attr('id').indexOf('_'));
                    $(e.target).parents('tbody').find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).parent().addClass('selected-row secondary');
                    $.ajax({
                        url: '/Activities/GetAccountingAccount',
                        cache: false,
                        type: 'POST',
                        data: { accountingAccountID: id },
                        beforeSend: function (xhr) {
                            UI.checkForPendingRequests(xhr);
                        },
                        success: function (data) {
                            $('#AccountingAccountInfo_AccountingAccountID').val(data.AccountingAccountInfo_AccountingAccountID);
                            $('#AccountingAccountInfo_Account').val(data.AccountingAccountInfo_Account);
                            $('#AccountingAccountInfo_AccountName').val(data.AccountingAccountInfo_AccountName);
                            $('#AccountingAccountInfo_Company').val(data.AccountingAccountInfo_Company);
                            UI.expandFieldset('fdsAccountingAccountsInfo');
                            UI.scrollTo('fdsAccountingAccountsInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Activities/DeleteAccountingAccount', 'tr', $(e.target), ACTIVITY.oAccountingAccountsTable, UI.updateDependentLists, { url: '/Activities/GetDDLData', itemType: 'accountingAccount' });
            }
        });
    }

    var makeTblPointsOfSaleRowsSelectable = function () {
        ACTIVITY.oPointsOfSaleTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            //$('#tblSearchPointsOfSaleResults tbody tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).parent().hasClass('selected-row')) {
                    var id = $(e.target).parent().attr('id').substr($(e.target).parent().attr('id').indexOf('_') + 1, $(e.target).parent().attr('id').length - $(e.target).parent().attr('id').indexOf('_'));
                    $(e.target).parents('tbody').find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).parent().addClass('selected-row secondary');
                    $.ajax({
                        url: '/Activities/GetPointOfSale',
                        cache: false,
                        type: 'POST',
                        data: { pointOfSaleID: id },
                        beforeSend: function (xhr) {
                            UI.checkForPendingRequests(xhr);
                        },
                        success: function (data) {
                            $('#PointsOfSaleInfo_PointOfSaleID').val(data.PointsOfSaleInfo_PointOfSaleID);
                            $('#PointsOfSaleInfo_PointOfSale').val(data.PointsOfSaleInfo_PointOfSale);
                            $('#PointsOfSaleInfo_ShortName').val(data.PointsOfSaleInfo_ShortName);
                            $('#PointsOfSaleInfo_Place').val(data.PointsOfSaleInfo_Place);
                            UI.expandFieldset('fdsPointOfSaleInfo');
                            UI.scrollTo('fdsPointOfSaleInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Activities/DeletePointOfSale', 'tr', $(e.target), ACTIVITY.oPointsOfSaleTable, UI.updateDependentLists, { url: '/Activities/GetDDLData', itemType: 'pointOfSale' });
            }
        });
    }

    var saveActivitySuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Activity Saved') {
                var categories = ACTIVITY.categories != undefined ? ACTIVITY.categories : "";
                var oSettings = ACTIVITY.oTable.fnSettings();
                var iAdded = ACTIVITY.oTable.fnAddData([
                    data.ItemID,
                    $('#ActivityInfo_Provider option:selected').text(),
                    $('#ActivityInfo_Activity').val(),
                    $('#ActivityInfo_OriginalTerminal option:selected').text(),
                    categories,
                    '',
                    'No',
                    'None',
                    '0',
                    '0',
                    '0',
                    '0',
                    '0',
                    '0',
                    '0',
                    '<img class="right" src="/Content/themes/base/images/trash.png" id="delP' + data.ItemID + '">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'trActivity' + data.ItemID);
                ACTIVITY.oTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                ACTIVITY.makeTableRowsSelectable();
                $('#trActivity' + data.ItemID).click();
                UI.expandFieldset('fdsActivityDescriptions');
                UI.scrollTo('fdsActivityDescriptions', null);
            }
            else {
                var array = ACTIVITY.oTable.fnGetNodes();
                var nTr = ACTIVITY.oTable.$('tr.selected-row');
                var position = ACTIVITY.oTable.fnGetPosition(nTr[0]);
                ACTIVITY.oTable.fnDisplayRow(array[position]);
                ACTIVITY.oTable.fnUpdate([
                    $('#ActivityInfo_ActivityID').val(),
                    $('#ActivityInfo_Provider option:selected').text(),
                    $('#ActivityInfo_Activity').val(),
                    $('#ActivityInfo_OriginalTerminal option:selected').text(),
                    ACTIVITY.categories,
                    $('#trActivity' + data.ItemID).children('td:nth-child(6)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(7)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(8)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(9)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(10)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(11)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(12)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(13)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(14)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(15)').html(),
                    $('#trActivity' + data.ItemID).children('td:nth-child(16)').html()
                ], $('#trActivity' + data.ItemID)[0], undefined, false);
                if ($('#ActivityInfo_ItemType option:selected').val() == 4) {
                    $.getJSON('/Activities/GetStockBalance', { serviceID: data.ItemID }, function (response) {
                        $('#fdsStockTransactions').show();
                        $('#SearchStockTransactions_Stock').val(response.stockID);
                        $('#StockInfo_StockID').val(response.stockID);
                        $('#StockTransactionInfo_Stock').val(response.stockID);
                        $('#StockInfo_Service').val(data.ItemID);
                        $('#StockTransactionInfo_Service').val(data.ItemID);
                        $('#StockInfo_Quantity').val(response.quantity);
                        $('#StockInfo_MinimalStock').val(response.stock);
                        $('#btnSearchStockTransactions').click();
                    });
                }
                else {
                    $('#fdsStockTransactions').hide();
                    $('#SearchStockTransactions_Stock').val('');
                    $('#StockInfo_StockID').val('');
                    $('#StockTransactionInfo_Stock').val('');
                    $('#StockInfo_Service').val('');
                    $('#StockTransactionInfo_Service').val('');
                    $('#StockInfo_Quantity').val('');
                    $('#StockInfo_MinimalStock').val('');
                }
            }
            $.getJSON('/Activities/GetDDLData', { itemType: 'servicesPerProvider', itemID: $('#ActivityInfo_Provider option:selected').val() }, function (data) {
                $('#PriceTypeRulesInfo_Service').fillSelect(data);
                $('#PriceTypeRulesInfo_Service').multiselect('refresh');
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveCategoriesToActivitySuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0)
            $('#trActivity' + data.ItemID).children('td:nth-child(5)').html(ACTIVITY.categories);
        //$('#trActivity' + data.ItemID).children('td:nth-child(5)')[0].firstChild.textContent = ACTIVITY.categories;
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveActivityDescriptionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Activity Description Saved') {
                var builder = '<li id="liActivityDescription' + data.ItemID + '" >'
                + $('#ActivityDescriptionInfo_Culture option:selected').text()
                + '<img id="delActivityDescription' + data.ItemID + '" src="/Content/themes/base/images/trash.png" class="right"></li>';
                $('#liNoDescriptions').remove();
                $('#ulActivityDescriptions').append(builder);
                $('#frmActivityDescription').clearForm();
                $('#frmActivityDescription').find('textarea').each(function () {
                    $(this).val('');
                });
                UI.ulsHoverEffect('ulActivityDescriptions');
                ACTIVITY.makeUlActivityDescriptionsRowsSelectable();
            }
            else {
                $('#liActivityDescription' + data.ItemID)[0].firstChild.textContent = $('#ActivityDescriptionInfo_Culture option:selected').text();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveActivityScheduleSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Schedule Saved') {
                var optionBuilder = '<option value="' + data.ItemID + '">' + $('#ActivityScheduleInfo_FromTime').val() + ' ';
                var builder = '<li id="liActivitySchedule' + data.ItemID + '">'
                        + '<div class="weekdays-alignment"><span>' + data.ItemID + '</span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                if ($('#ActivityScheduleInfo_Monday').is(':checked')) {
                    builder += ' checked="' + $('#ActivityScheduleInfo_Monday').is(':checked') + '"';
                    optionBuilder += 'Monday, ';
                }
                builder += '/></span></div>'
                + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                if ($('#ActivityScheduleInfo_Tuesday').is(':checked')) {
                    builder += 'checked="' + $('#ActivityScheduleInfo_Tuesday').is(':checked') + '"';
                    optionBuilder += 'Tuesday, ';
                }
                builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                if ($('#ActivityScheduleInfo_Wednesday').is(':checked')) {
                    builder += 'checked="' + $('#ActivityScheduleInfo_Wednesday').is(':checked') + '"';
                    optionBuilder += 'Wednesday, ';
                }
                builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                if ($('#ActivityScheduleInfo_Thursday').is(':checked')) {
                    builder += 'checked="' + $('#ActivityScheduleInfo_Thursday').is(':checked') + '"';
                    optionBuilder += 'Thursday, ';
                }
                builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                if ($('#ActivityScheduleInfo_Friday').is(':checked')) {
                    builder += 'checked="' + $('#ActivityScheduleInfo_Friday').is(':checked') + '"';
                    optionBuilder += 'Friday, ';
                }
                builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                if ($('#ActivityScheduleInfo_Saturday').is(':checked')) {
                    builder += 'checked="' + $('#ActivityScheduleInfo_Saturday').is(':checked') + '"';
                    optionBuilder += 'Saturday, ';
                }
                builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span><input type="checkbox" disabled="disabled"';
                if ($('#ActivityScheduleInfo_Sunday').is(':checked')) {
                    builder += 'checked="' + $('#ActivityScheduleInfo_Sunday').is(':checked') + '"';
                    optionBuilder += 'Sunday, ';
                }
                builder += '/></span></div>'
                        + '<div class="weekdays-alignment"><span>' + $('#ActivityScheduleInfo_FromTime').val() + '</span></div>';
                if ($('#ActivityScheduleInfo_IsSpecificTime').val() == 'False') {
                    builder += '<div class="weekdays-alignment"><span>' + $('#ActivityScheduleInfo_ToTime').val() + '</span></div>';
                }
                else {
                    builder += '<div class="weekdays-alignment"><span> </span></div>';
                }
                builder += '<img id="delActivitySchedule' + data.ItemID + '" src="/Content/themes/base/images/trash.png" class="right" />'
                    + '</li>';
                $('#liNoSchedules').remove();
                $('#ulActivitySchedules').append(builder);
                optionBuilder = optionBuilder.substr(0, optionBuilder.length - 2);
                optionBuilder += '</option>';
                $('#ActivityMeetingPointInfo_WeeklySchedule').append(optionBuilder);
                $('#frmActivitySchedule').clearForm();
                UI.ulsHoverEffect('ulActivitySchedules');
                ACTIVITY.makeUlActivitySchedulesRowsSelectable();
            }
            else {
                var optionBuilder = '<option value="' + data.ItemID + '">' + $('#ActivityScheduleInfo_FromTime').val() + ' ';
                if ($('#ActivityScheduleInfo_Monday').is(':checked')) {
                    $('#liActivitySchedule' + data.ItemID)[0].children[1].children[0].children[0].setAttribute('checked', 'true');
                    optionBuilder += 'Monday, ';
                }
                else {
                    $('#liActivitySchedule' + data.ItemID)[0].children[1].children[0].children[0].removeAttribute('checked');
                }
                if ($('#ActivityScheduleInfo_Tuesday').is(':checked')) {
                    $('#liActivitySchedule' + data.ItemID)[0].children[2].children[0].children[0].setAttribute('checked', 'true');
                    optionBuilder += 'Tuesday, ';
                }
                else {
                    $('#liActivitySchedule' + data.ItemID)[0].children[2].children[0].children[0].removeAttribute('checked');
                }
                if ($('#ActivityScheduleInfo_Wednesday').is(':checked')) {
                    $('#liActivitySchedule' + data.ItemID)[0].children[3].children[0].children[0].setAttribute('checked', 'true');
                    optionBuilder += 'Wednesday, ';
                }
                else {
                    $('#liActivitySchedule' + data.ItemID)[0].children[3].children[0].children[0].removeAttribute('checked');
                }
                if ($('#ActivityScheduleInfo_Thursday').is(':checked')) {
                    $('#liActivitySchedule' + data.ItemID)[0].children[4].children[0].children[0].setAttribute('checked', 'true');
                    optionBuilder += 'Thursday, ';
                }
                else {
                    $('#liActivitySchedule' + data.ItemID)[0].children[4].children[0].children[0].removeAttribute('checked');
                }
                if ($('#ActivityScheduleInfo_Friday').is(':checked')) {
                    $('#liActivitySchedule' + data.ItemID)[0].children[5].children[0].children[0].setAttribute('checked', 'true');
                    optionBuilder += 'Friday, ';
                }
                else {
                    $('#liActivitySchedule' + data.ItemID)[0].children[5].children[0].children[0].removeAttribute('checked');
                }
                if ($('#ActivityScheduleInfo_Saturday').is(':checked')) {
                    $('#liActivitySchedule' + data.ItemID)[0].children[6].children[0].children[0].setAttribute('checked', 'true');
                    optionBuilder += 'Saturday, ';
                }
                else {
                    $('#liActivitySchedule' + data.ItemID)[0].children[6].children[0].children[0].removeAttribute('checked');
                }
                if ($('#ActivityScheduleInfo_Sunday').is(':checked')) {
                    $('#liActivitySchedule' + data.ItemID)[0].children[7].children[0].children[0].setAttribute('checked', 'true');
                    optionBuilder += 'Sunday, ';
                }
                else {
                    $('#liActivitySchedule' + data.ItemID)[0].children[7].children[0].children[0].removeAttribute('checked');
                }
                $('#liActivitySchedule' + data.ItemID)[0].children[8].children[0].textContent = $('#ActivityScheduleInfo_FromTime').val();
                if ($('input:radio[name="ActivityScheduleInfo_IsSpecificTime"]:checked').val() == 'False') {
                    $('#liActivitySchedule' + data.ItemID)[0].children[9].children[0].textContent = ' - ' + $('#ActivityScheduleInfo_ToTime').val();
                }
                else {
                    $('#liActivitySchedule' + data.ItemID)[0].children[9].children[0].textContent = ' ';
                }
            }
            optionBuilder = optionBuilder.substr(0, optionBuilder.length - 2);
            optionBuilder += '</option>';
            $('#ActivityMeetingPointInfo_WeeklySchedule option[value="' + data.ItemID + '"]').remove();
            $('#ActivityMeetingPointInfo_WeeklySchedule').append(optionBuilder);
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveMeetingPointSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Meeting Point Saved') {
                $.each(data.ItemID, function (index, item) {
                    var builder = '<li id="liMeetingPoint_' + item.meetingPointID + '">'
                    + '<div style="display:inline-block;width:70px"><span>' + item.time + '</span></div>'
                    + '<div style="display:inline-block;width:250px"><span>' + item.place + '</span></div>'
                    + '<div style="display:inline-block;width:100px"><span>' + $('#ActivityMeetingPointInfo_Notes').val() + '</span></div>'
                    + '<div style="display:inline-block;width:100px"><span>'
                        + '<input type="checkbox" disabled="disabled" ' + (item.atYourHotel ? 'checked="checked"' : '') + '/>'
                        + '</span></div>'
                    + '<div style="display:inline-block;width:100px"><span>'
                        + '<input type="checkbox" disabled="disabled" ' + (item.active ? 'checked="checked"' : '') + '/>'
                        + '</span></div>'
                    + '<img id="delMeetingPoint' + item.meetingPointID + '" src="/Content/themes/base/images/trash.png" class="right"></li>';
                    $('#liNoMeetingPoints').remove();
                    $('#ulActivityMeetingPoints').append(builder);
                });
                $('#frmMeetingPoint').clearForm();
                $('#tblDepartureTimes tbody').empty();
                $('#ActivityMeetingPointInfo_Notes').val('');
                UI.ulsHoverEffect('ulActivityMeetingPoints');
                ACTIVITY.makeUlActivityMeetingPointsRowsSelectable();
            }
            else {
                $('#liMeetingPoint_' + data.ItemID)[0].children[0].children[0].textContent = $('#ActivityMeetingPointInfo_DepartureTime').val();
                $('#liMeetingPoint_' + data.ItemID)[0].children[1].children[0].textContent = ($('#ActivityMeetingPointInfo_PlaceString').val() != '' ? $('#ActivityMeetingPointInfo_PlaceString').val() : 'At Your Hotel');
                $('#liMeetingPoint_' + data.ItemID)[0].children[2].children[0].textContent = $('#ActivityMeetingPointInfo_Notes').val();
                if ($('#ActivityMeetingPointInfo_AtYourHotel')[0].checked) {
                    $('#liMeetingPoint_' + data.ItemID)[0].children[3].children[0].children[0].setAttribute('checked', 'checked');
                }
                else {
                    $('#liMeetingPoint_' + data.ItemID)[0].children[3].children[0].children[0].removeAttribute('checked');
                }
                if ($('#ActivityMeetingPointInfo_IsActive')[0].checked) {
                    $('#liMeetingPoint_' + data.ItemID)[0].children[4].children[0].children[0].setAttribute('checked', 'checked');
                }
                else {
                    $('#liMeetingPoint_' + data.ItemID)[0].children[4].children[0].children[0].removeAttribute('checked');
                }
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
        //var duration = data.ResponseType < 0 ? data.ResponseType : null;
        //if (data.ResponseType > 0) {
        //    if (data.ResponseMessage == 'Meeting Point Saved') {
        //        $.each(data.ItemID, function (index, item) {
        //            var builder = '<li id="liMeetingPoint' + item.meetingPointID + '">'
        //            + '<div style="display:inline-block;width:70px"><span>' + item.time + '</span></div>'
        //            + '<div style="display:inline-block;width:250px"><span>' + item.place + '</span></div>'
        //            + '<div style="display:inline-block;width:100px"><span>' + $('#ActivityMeetingPointInfo_Notes').val() + '</span></div>'
        //            + '<div style="display:inline-block;width:100px"><span>'
        //                + '<input type="checkbox" disabled="disabled" ' + (item.atYourHotel ? 'checked="checked"' : '') + '/>'
        //                + '</span></div>'
        //            + '<div style="display:inline-block;width:100px"><span>'
        //                + '<input type="checkbox" disabled="disabled" ' + (item.active ? 'checked="checked"' : '') + '/>'
        //                + '</span></div>'
        //            + '<img id="delMeetingPoint' + item.meetingPointID + '" src="/Content/themes/base/images/trash.png" class="right"></li>';
        //            $('#liNoMeetingPoints').remove();
        //            $('#ulActivityMeetingPoints').append(builder);
        //        });
        //        $('#frmMeetingPoint').clearForm();
        //        $('#tblDepartureTimes tbody').empty();
        //        $('#ActivityMeetingPointInfo_Notes').val('');
        //        UI.ulsHoverEffect('ulActivityMeetingPoints');
        //        ACTIVITY.makeUlActivityMeetingPointsRowsSelectable();
        //    }
        //    else {
        //        $('#liMeetingPoint' + data.ItemID)[0].children[0].children[0].textContent = $('#ActivityMeetingPointInfo_DepartureTime').val();
        //        $('#liMeetingPoint' + data.ItemID)[0].children[1].children[0].textContent = $('#ActivityMeetingPointInfo_PlaceString').val();
        //        $('#liMeetingPoint' + data.ItemID)[0].children[2].children[0].textContent = $('#ActivityMeetingPointInfo_Notes').val();
        //        if ($('#ActivityMeetingPointInfo_AtYourHotel')[0].checked) {
        //            $('#liMeetingPoint' + data.ItemID)[0].children[3].children[0].children[0].setAttribute('checked', 'checked');
        //        }
        //        else {
        //            $('#liMeetingPoint' + data.ItemID)[0].children[3].children[0].children[0].removeAttribute('checked');
        //        }
        //        if ($('#ActivityMeetingPointInfo_IsActive')[0].checked) {
        //            $('#liMeetingPoint' + data.ItemID)[0].children[4].children[0].children[0].setAttribute('checked', 'checked');
        //        }
        //        else {
        //            $('#liMeetingPoint' + data.ItemID)[0].children[4].children[0].children[0].removeAttribute('checked');
        //        }
        //    }
        //}
        //UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveActivityAccountingAccountSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            var accounts = '';
            $.each(data.ItemID.accounts.split('|'), function (index, item) {
                accounts += '<span class="block">' + item + '</span>';
            });
            ACTIVITY.updateTblActivityAccountingAccounts($('#ActivityInfo_ActivityID').val());
            $('#frmAccountingAccount').clearForm();
            ACTIVITY.makeTblActivityAccountingAccountsRowsSelectable();
            $('#trActivity' + $('#ActivityInfo_ActivityID').val()).children('td:nth-child(6)').html(accounts);
            ACTIVITY.makeTableRowsSelectable();
            $('#hResults').text(ACTIVITY.oAccountsPerActivityTable.$('tbody tr').length + ' Related Accounting Accounts');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var updateDrpsActivityDependents = function (activityID, destinationID) {
        $.getJSON('/Activities/GetDDLData', { itemID: activityID, itemType: 'weeklyAvailabilities' }, function (data) {
            $('#ActivityMeetingPointInfo_WeeklySchedule').fillSelect(data);
        });
        $.getJSON('/Activities/GetDDLData', { itemID: destinationID, itemType: 'places' }, function (data) {
            $('#ActivityMeetingPointInfo_Place').fillSelect(data);
            var array = new Array();
            $.each(data, function (index, item) {
                array[index] = item.Text;
            });
            $('#ActivityMeetingPointInfo_PlaceString').autocomplete({
                source: array,
                minLength: 0,
                position: { my: 'right top', at: 'right bottom' },
                //select: function (evt, ui) {
                //    console.log('change');
                //    $('#ActivityMeetingPointInfo_Place').children('option').each(function () {
                //        if ($(this).text() == ui.item.value) {
                //            $(this).attr('selected', true);
                //        }
                //    });
                //},
                close: function (evt, ui) {
                    $('#ActivityMeetingPointInfo_AtYourHotel').removeAttr('checked');
                    $('#ActivityMeetingPointInfo_Place').children('option').each(function () {
                        if ($(this).text() == $('#ActivityMeetingPointInfo_PlaceString').val()) {
                            $(this).attr('selected', true);
                        }
                    });
                }
            });
        });
    }

    var saveAccountingAccountSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Accounting Account Saved') {
                ACTIVITY.oAccountingAccountsTable = $.fn.DataTable.fnIsDataTable('tblSearchAccountingAccountsResults') ? $('#tblSearchAccountingAccountsResults') : $('#tblSearchAccountingAccountsResults').dataTable();
                var oSettings = ACTIVITY.oAccountingAccountsTable.fnSettings();
                var iAdded = ACTIVITY.oAccountingAccountsTable.fnAddData([
                    $('#AccountingAccountInfo_Account').val(),
                    $('#AccountingAccountInfo_Company option:selected').text() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'accountingAccount_' + data.ItemID);
                ACTIVITY.oAccountingAccountsTable.fnDisplayRow(aRow);
                $('#frmAccountingAccountInfo').clearForm();
                UI.tablesHoverEffect();
                ACTIVITY.makeTblAccountingAccountsRowsSelectable();
            }
            else {
                $('#accountingAccount_' + data.ItemID).children('td:nth-child(1)').text($('#AccountingAccountInfo_Account').val());
                $('#accountingAccount_' + data.ItemID).children('td:nth-child(2)').text($('#AccountingAccountInfo_AccountName').val());
                $('#accountingAccount_' + data.ItemID).children('td:nth-child(3)')[0].firstChild.textContent = $('#AccountingAccountInfo_Company option:selected').text();
            }
            UI.updateDependentLists('/Activities/GetDDLData', 'accountingAccount', 0, false);
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var savePointOfSaleSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Point Of Sale Saved') {
                ACTIVITY.oPointsOfSaleTable = $.fn.DataTable.fnIsDataTable('tblSearchPointsOfSaleResults') ? $('#tblSearchPointsOfSaleResults') : $('#tblSearchPointsOfSaleResults').dataTable();
                var oSettings = ACTIVITY.oPointsOfSaleTable.fnSettings();
                var iAdded = ACTIVITY.oPointsOfSaleTable.fnAddData([
                    $('#PointsOfSaleInfo_PointOfSale').val(),
                    $('#PointsOfSaleInfo_ShortName').val(),
                    $('#PointsOfSaleInfo_Place option:selected').text() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'pointOfSale_' + data.ItemID);
                ACTIVITY.oPointsOfSaleTable.fnDisplayRow(aRow);
                $('#frmPointOfSaleInfo').clearForm();
                UI.tablesHoverEffect();
                ACTIVITY.makeTblPointsOfSaleRowsSelectable();
            }
            else {
                $('#pointOfSale_' + data.ItemID).children('td:nth-child(1)').text($('#PointsOfSaleInfo_PointOfSale').val());
                $('#pointOfSale_' + data.ItemID).children('td:nth-child(2)').text($('#PointsOfSaleInfo_ShortName').val());
                $('#pointOfSale_' + data.ItemID).children('td:nth-child(3)')[0].firstChild.textContent = $('#PointsOfSaleInfo_Place option:selected').text();
            }
            UI.updateDependentLists('/Activities/GetDDLData', 'pointOfSale', 0, false);
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var deleteRowFunctionality = function () {
        $('.delete-row').unbind('click').on('click', function () {
            $(this).parents('tr').remove();
            $('#ActivityMeetingPointInfo_DepartureTimes').attr('value', '');
            $('#tblDepartureTimes tbody').children('tr').each(function () {
                $('#ActivityMeetingPointInfo_DepartureTimes').attr('value', $('#ActivityMeetingPointInfo_DepartureTimes').val() + $(this).attr('id') + ',');
            });
        });
    }

    //price type rules

    var searchPriceTypeRulesResults = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblPriceTypeRules', tableColumns.length - 1);
        ACTIVITY.oRulesTable = $('#tblPriceTypeRules').dataTable();
        if (ACTIVITY.oRulesTable != undefined && ACTIVITY.oRulesTable.length > 0) {
            ACTIVITY.oRulesTable.fnSort([[1, 'asc']]);
        }
    }

    var searchFuturePriceTypeRulesResults = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblFuturePriceTypeRules', tableColumns.length - 1);
        ACTIVITY.oFutureRulesTable = $('#tblFuturePriceTypeRules').dataTable();
        if (ACTIVITY.oFutureRulesTable != undefined && ACTIVITY.oFutureRulesTable.length > 0) {
            ACTIVITY.oFutureRulesTable.fnSort([[1, 'asc']]);
        }
    }

    var updateTblPriceTypeRules = function (serviceID, terminalID, date) {
        $('#divPriceTypesRules').empty();
        $.ajax({
            url: '/Activities/GetPriceTypeRules',
            cache: false,
            type: 'POST',
            data: { serviceID: serviceID, terminalID: terminalID, date: $('#SearchRules_Date').val(), date: date },
            beforeSend: function (xhr) {
                UI.checkForPendingRequests(xhr);
            },
            success: function (data) {
                $('#divPriceTypesRules').html(data);
                ACTIVITY.searchPriceTypeRulesResults($('#tblPriceTypeRules'));
                ACTIVITY.searchFuturePriceTypeRulesResults($('#tblFuturePriceTypeRules'));
                //ACTIVITY.updatePriceTypesInRules();
                //ACTIVITY.makeTblRulesSelectable();//there wont be able to update rules
                //ACTIVITY.makeFutureRulesSelectable();
                ACTIVITY.actionsAfterRenderRulesTable();

                //ACTIVITY.actionsAfterRenderRulesTable();
                //$('#PriceTypeRulesInfo_Terminal').trigger('change');
            }
        });
    }

    var actionsAfterRenderRulesTable = function () {
        ACTIVITY.makeTblRulesSelectable();
        //var _priceTypeRules = new Array();
        //ACTIVITY.oRulesTable.$('tr').each(function (index, item) {
        //    _priceTypeRules.push($(this)[0].cells[1].textContent.trim());
        //});

        //$('.visibility-controlled option').each(function () {
        //    if ($.inArray($(this).text(), _priceTypeRules) == -1) {
        //        $(this).hide();
        //    }
        //    else {
        //        $(this).show();
        //    }
        //});
        ACTIVITY.makeFutureRulesSelectable();
        ACTIVITY.updatePriceTypesInRules();
    }

    var updatePriceTypesInRules = function () {
        var _priceTypeRules = new Array();
        ACTIVITY.oRulesTable.$('tr').each(function (index, item) {
            _priceTypeRules.push($(this)[0].cells[1].textContent.trim());
        });

        $('.visibility-controlled option').each(function (e) {
            if ($.inArray($(this).text(), _priceTypeRules) == -1) {
                $(this).hide();
            }
            else {
                $(this).show();
            }
        });
    }

    var makeTblRulesSelectable = function () {
        ACTIVITY.oRulesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img.delete-rule')) {
                //if ($(this).children('td:nth-child(5)').text().trim() != 'Terminal') {
                if ($(this).find('td[data-rule-level=""]').text().trim() != 'Terminal') {
                    if (!$(this).hasClass('selected-row')) {
                        ACTIVITY.oRulesTable.$('tr.selected-row').removeClass('selected-row secondary');
                        $(this).addClass('selected-row secondary');
                        $('#PriceTypeRulesInfo_PriceTypeRuleID').val(ACTIVITY.oRulesTable.$('tr.selected-row').attr('id').split('_')[1]);
                        //$('#PriceTypeRulesInfo_ToDate').datepicker({
                        //    dateFormat: 'yy-mm-dd'
                        //    //timeFormat: 'HH:mm',
                        //    //timeOnly: false,
                        //    //stepMinute: 5
                        //});
                        //$('#btnCancelClosing').on('click', function () {
                        //    ACTIVITY.oRulesTable.$('tr.selected-row').removeClass('selected-row secondary');
                        //    $('#PriceTypeRulesInfo_PriceTypeRuleID').val('');
                        //    $.fancybox.close(true);
                        //    UI.scrollTo('fdsPriceTypesRules', null);
                        //});
                        //$('#btnCloseRule').on('click', function () {
                        //    $.ajax({
                        //        type: 'POST',
                        //        url: '/Activities/ClosePriceTypeRule',
                        //        data: { priceTypeRuleID: $('#PriceTypeRulesInfo_PriceTypeRuleID').val(), toDate: $('#PriceTypeRulesInfo_ToDate').val() },
                        //        success: function (data) {
                        //            var duration = data.ResponseType < 0 ? data.ResponseType : null;
                        //            ACTIVITY.updateTblPriceTypeRules($('#ActivityInfo_ActivityID').val(), $('#ActivityInfo_OriginalTerminal').val(), null);
                        //            $('#frmPriceTypeRuleInfo').clearForm();
                        //            if (data.ResponseType > 0) {
                        //                PRICE.updateTblPrices();
                        //            }
                        //            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                        //        },
                        //        complete: function () {
                        //            ACTIVITY.oRulesTable.$('tr.selected-row').removeClass('selected-row secondary');
                        //            $('#PriceTypeRulesInfo_PriceTypeRuleID').val('');
                        //            $.fancybox.close(true);
                        //            UI.scrollTo('fdsPriceTypesRules', null);
                        //        }
                        //    });
                        //});
                        $.fancybox({
                            //type: 'html',
                            type: 'ajax',
                            href: '/Activities/RenderPriceTypeRuleClosing',
                            ajax: { data: { priceTypeRule: $('#PriceTypeRulesInfo_PriceTypeRuleID').val() } },
                            //href: $('#divPriceTypeRuleClosing'),
                            modal: true,
                            afterShow: function () {
                                UI.scrollTo('fdsPriceTypesRules', null);
                                //$('#PriceTypeRulesInfo_ToDate').datetimepicker({
                                //    dateFormat: 'yy-mm-dd',
                                //    timeFormat: 'HH:mm',
                                //    timeOnly: false,
                                //    stepMinute: 5
                                //});
                                
                                $('#PriceTypeRulesInfo_ToDate').datepicker({
                                    dateFormat: 'yy-mm-dd'
                                    //timeFormat: 'HH:mm',
                                    //timeOnly: false,
                                    //stepMinute: 5
                                });

                                $('#btnCancelClosing').on('click', function () {
                                    ACTIVITY.oRulesTable.$('tr.selected-row').removeClass('selected-row secondary');
                                    $('#PriceTypeRulesInfo_PriceTypeRuleID').val('');
                                    $.fancybox.close(true);
                                    UI.scrollTo('fdsPriceTypesRules', null);
                                });

                                $('#btnCloseRule').on('click', function () {
                                    $.ajax({
                                        type: 'POST',
                                        url: '/Activities/ClosePriceTypeRule',
                                        //data: { priceTypeRuleID: $('#PriceTypeRulesInfo_PriceTypeRuleID').val(), toDate: $('#PriceTypeRulesInfo_ToDate').val() },
                                        data: { priceTypeRuleID: $('#PriceTypeRulesInfo_PriceTypeRuleID').val(), toDate: $('#PriceTypeRulesInfo_ToDate').val().split('-')[0].length == 4 ? $('#PriceTypeRulesInfo_ToDate').val() : $('#PriceTypeRulesInfo_ToDate').val().split('-').reverse().join('-') },
                                        success: function (data) {
                                            var duration = data.ResponseType < 0 ? data.ResponseType : null;
                                            ACTIVITY.updateTblPriceTypeRules($('#ActivityInfo_ActivityID').val(), $('#ActivityInfo_OriginalTerminal').val(), null);
                                            $('#frmPriceTypeRuleInfo').clearForm();
                                            if (data.ResponseType > 0) {
                                                PRICE.updateTblPrices();
                                            }
                                            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                                        },
                                        complete: function () {
                                            ACTIVITY.oRulesTable.$('tr.selected-row').removeClass('selected-row secondary');
                                            $('#PriceTypeRulesInfo_PriceTypeRuleID').val('');
                                            $.fancybox.close(true);
                                            UI.scrollTo('fdsPriceTypesRules', null);
                                        }
                                    });
                                });
                            }
                        });
                    }
                }
                else {
                    UI.messageBox(0, 'Terminal Rules Cannot be Closed', null, null);
                }
            }
        });

        //ACTIVITY.oRulesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
        //    if (!$(e.target).is('img.delete-rule')) {
        //        if ($(this).children('td:nth-child(7)').text().trim() != 'Terminal') {
        //            if (!$(this).hasClass('selected-row')) {
        //                ACTIVITY.oRulesTable.$('tr.selected-row').removeClass('selected-row secondary');
        //                $(this).addClass('selected-row secondary');
        //                var id = ACTIVITY.oRulesTable.$('tr.selected-row').attr('id').substr(ACTIVITY.oRulesTable.$('tr.selected-row').attr('id').indexOf('_') + 1);
        //                $.ajax({
        //                    url: '/Activities/GetPriceTypeRule',
        //                    cache: false,
        //                    type: 'POST',
        //                    data: { ruleID: id },
        //                    beforeSend: function (xhr) {
        //                        UI.checkForPendingRequests(xhr);
        //                    },
        //                    success: function (data) {
        //                        $('#PriceTypeRulesInfo_PriceTypeRuleID').val(data.Service_PriceTypeID);
        //                        $('#PriceTypeRulesInfo_Terminal').val(data.TerminalID);
        //                        $('#PriceTypeRulesInfo_Provider').val(data.ProviderID);
        //                        //$('#PriceTypeRulesInfo_Terminal').trigger('change', { provider: data.ProviderID });
        //                        $('#PriceTypeRulesInfo_Service').val((data.ServiceID != null ? data.ServiceID : 'null'));
        //                        $('#PriceTypeRulesInfo_Service').multiselect('refresh');
        //                        $('#PriceTypeRulesInfo_Service').multiselect('close');
        //                        $('#PriceTypeRulesInfo_GenericUnit').val(data.GenericUnitID);
        //                        $('#PriceTypeRulesInfo_PriceType').val(data.PriceTypeID);
        //                        $('#PriceTypeRulesInfo_FromDate').val(data.PriceTypeRulesInfo_FromDate);
        //                        if (data.IsBasePrice) {
        //                            $('input:radio[name="PriceTypeRulesInfo_Base"]')[0].checked = true;
        //                        }
        //                        else {
        //                            $('input:radio[name="PriceTypeRulesInfo_Base"]')[1].checked = true;
        //                        }
        //                        $('input:radio[name="PriceTypeRulesInfo_Base"]').trigger('change');
        //                        $('#PriceTypeRulesInfo_Percentage').val((data.Percentage != null ? data.Percentage : ''));
        //                        if (data.MoreOrLess != null && data.MoreOrLess) {
        //                            $('input:radio[name="PriceTypeRulesInfo_MoreOrLess"]')[0].checked = true;
        //                        }
        //                        else {
        //                            $('input:radio[name="PriceTypeRulesInfo_MoreOrLess"]')[1].checked = true;
        //                        }
        //                        $('#PriceTypeRulesInfo_ThanPriceType').val((data.ThanPriceTypeID != null ? data.ThanPriceTypeID : 0));
        //                        UI.expandFieldset('fdsPriceTypeRuleInfo');
        //                        UI.scrollTo('fdsPriceTypeRuleInfo', null);
        //                    }
        //                });
        //            }
        //        }
        //    }
        //    else {
        //        UI.confirmBox('Do you confirm you want to proceed?', deletePriceTypeRule, [$(e.target).parents('tr:first')]);
        //    }
        //});
    }

    var makeFutureRulesSelectable = function () {
        ACTIVITY.oFutureRulesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if ($(e.target).is('img.delete-rule')) {
                //if ($(this).children('td:nth-child(7)').text().trim().toLowerCase() != 'terminal') {
                    if ($(this).find('td[data-rule-level=""]').text().trim() != 'Terminal') {
                    UI.confirmBox('Do you confirm you want to proceed?'
                        + '<br />This action will not restore previously closed rules.', deletePriceTypeRule, [$(this)]);
                }
                else {
                    UI.messageBox(0, 'Rules of Terminal Level CANNOT be Deleted', null, null);
                }
            }
            else {
                //if ($(this).children('td:nth-child(7)').text().trim().toLowerCase() != 'terminal') {
                    if ($(this).find('td[data-rule-level=""]').text().trim() != 'Terminal') {
                    if (!$(this).hasClass('selected-row')) {
                        ACTIVITY.oFutureRulesTable.$('tr.selected-row').removeClass('selected-row secondary');
                        $(this).addClass('selected-row secondary');
                        $.ajax({
                            url: '/Activities/GetPriceTypeRule',
                            cache: false,
                            type: 'POST',
                            data: { ruleID: ACTIVITY.oFutureRulesTable.$('tr.selected-row').attr('id').split('_')[1] },
                            //beforeSend: function (xhr) {
                            //    UI.checkForPendingRequests(xhr);
                            //},
                            success: function (data) {
                                $('#PriceTypeRulesInfo_PriceTypeRuleID').val(data.Service_PriceTypeID);
                                $('#PriceTypeRulesInfo_Terminal').val(data.TerminalID);
                                $('#PriceTypeRulesInfo_Provider').val(data.ProviderID);
                                $('#PriceTypeRulesInfo_Service').val((data.ServiceID != null ? data.ServiceID : 'null'));
                                $('#PriceTypeRulesInfo_Service').multiselect('refresh');
                                $('#PriceTypeRulesInfo_Service').multiselect('close');
                                $('#PriceTypeRulesInfo_GenericUnit').val(data.GenericUnitID);
                                $('#PriceTypeRulesInfo_PriceType').val(data.PriceTypeID);
                                var _date = new Date(parseInt(data.FromDate.substring(data.FromDate.indexOf('(') + 1, data.FromDate.indexOf(')'))));
                                $('#PriceTypeRulesInfo_FromDate').datepicker('setDate', _date);
                                $('#PriceTypeRulesInfo_FromDate').datepicker('option', 'minDate', 1);
                                if (data.IsBasePrice) {
                                    $('input:radio[name="PriceTypeRulesInfo_Base"]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name="PriceTypeRulesInfo_Base"]')[1].checked = true;
                                }
                                $('input:radio[name="PriceTypeRulesInfo_Base"]').trigger('change');
                                $('#PriceTypeRulesInfo_Percentage').val((data.Percentage != null ? data.Percentage : ''));
                                if (data.MoreOrLess != null && data.MoreOrLess) {
                                    $('input:radio[name="PriceTypeRulesInfo_MoreOrLess"]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name="PriceTypeRulesInfo_MoreOrLess"]')[1].checked = true;
                                }
                                $('#PriceTypeRulesInfo_ThanPriceType').val((data.ThanPriceTypeID != null ? data.ThanPriceTypeID : 0));
                                UI.expandFieldset('fdsPriceTypeRuleInfo');
                                UI.scrollTo('fdsPriceTypeRuleInfo', null);
                            }
                        });
                    }
                }
                else {
                    UI.messageBox(0, 'Rules of Terminal Level CANNOT be Modified', null, null);
                }
            }
        });
    }

    var savePriceTypeRuleSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;

        if (data.ResponseType != 0) {
            ACTIVITY.updateTblPriceTypeRules($('#ActivityInfo_ActivityID').val(), $('#ActivityInfo_OriginalTerminal').val(), null);
            $('#frmPriceTypeRuleInfo').clearForm();
            if (data.ResponseType > 0) {
                PRICE.updateTblPrices();
            }
            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
        }
        else {
            //change this for a twoActionButton, making cancel fires tableUpdate
            UI.twoActionBox(data.ResponseMessage, overwriteRule, [data.ItemID], 'Yes', function () { ACTIVITY.updateTblPriceTypeRules($('#ActivityInfo_ActivityID').val(), $('#ActivityInfo_OriginalTerminal').val()); $('#messageBoxClose').click(); $('#frmPriceTypeRuleInfo').clearForm(); }, [], 'No');
        }
    }

    function overwriteRule(rules) {
        var _rules = $.parseJSON(rules);
        var children = '';
        $('#ulRulesToClose').find('input:checkbox:checked').each(function () {
            children += (children == '' ? '' : ',') + $(this).attr('id');
        });
        _rules.children = children;
        //$('#PriceTypeRulesInfo_PriceTypeRules').val(rules);
        $('#PriceTypeRulesInfo_PriceTypeRules').val(JSON.stringify(_rules));
        $('#frmPriceTypeRuleInfo').submit();
    }

    function deletePriceTypeRule(tr) {
        $.ajax({
            url: '/Activities/DeletePriceTypeRule',
            cache: false,
            type: 'POST',
            data: { targetID: $(tr).attr('id').split('_')[1] },
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    $('#btnNewPriceTypeRuleInfo').trigger('click');
                    ACTIVITY.updateTblPriceTypeRules($('#ActivityInfo_ActivityID').val(), $('#ActivityInfo_OriginalTerminal option:selected').val(), null);
                }
                //UI.tablesStripedEffect();
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var updateServicesDependantLists = function (terminalID, providerID, serviceID) {
        $('#PriceTypeRulesInfo_Terminal').val(terminalID);
        if ($('#PriceTypeRulesInfo_Provider option:last').val() != providerID) {
            $('#PriceTypeRulesInfo_Provider').empty();
            $('#PriceTypeRulesInfo_Provider').append('<option value="' + providerID + '">' + $('#ActivityInfo_Provider option:selected').text() + '</option>');
            $.getJSON('/Activities/GetDDLData', { itemType: 'servicesPerProvider', itemID: providerID }, function (data) {
                $('#PriceTypeRulesInfo_Service').fillSelect(data);
                $('#PriceTypeRulesInfo_Service').multiselect('refresh');
            });
            $.getJSON('/Activities/GetDDLData', { itemType: 'currenciesPerProvider', itemID: providerID }, function (data) {
                $('#PriceInfo_Currency').fillSelect(data);
            });
            //agregar obtención de tipos de precio            
            $.getJSON('/Activities/GetDDLData', { itemType: 'priceTypes', itemID: terminalID }, function (data) {
                $('#PriceTypeRulesInfo_PriceType').fillSelect(data);
                $('#PriceTypeRulesInfo_ThanPriceType').fillSelect(data);
            });
        }
    }

    var searchStockTransactionsResultsTable = function (data) {
        UI.Notifications.workingOn("CMS > Activities");
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblStockTransactions', tableColumns.length - 1);
        ACTIVITY.oStockTransactionsTable = $('#tblStockTransactions').dataTable();
        //UI.exportToExcel();
    }

    var saveStockTransactionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;

        if (data.ResponseType > 0) {
            ACTIVITY.oStockTransactionsTable = $.fn.DataTable.fnIsDataTable('tblStockTransactions') ? $('#tblStockTransactions') : $('#tblStockTransactions').dataTable();
            var oSettings = ACTIVITY.oStockTransactionsTable.fnSettings();
            var iAdded = ACTIVITY.oStockTransactionsTable.fnAddData([
                ($('input[name="StockTransactionInfo_Ingress"]:checked').val().toLowerCase() == 'true' ? 'Ingress' : 'Egress'),
                $('#StockTransactionInfo_Quantity').val(),
                COMMON.getDate(),
                ($('#ufirstname').val() + ' ' + $('#ulastname').val()),
                $('#StockTransactionInfo_TransactionDescription').val(),
                ' '
            ]);
            var aRow = oSettings.aoData[iAdded[0]].nTr;
            aRow.setAttribute('id', data.ItemID.transaction);
            ACTIVITY.oStockTransactionsTable.fnDisplayRow(aRow);
            $('#StockTransactionInfo_Stock').val(data.ItemID.stock);
            var quantity = parseFloat($('#StockInfo_Quantity').val());
            var _quantity = parseFloat($('#StockTransactionInfo_Quantity').val());
            quantity = $('input[name="StockTransactionInfo_Ingress"]:checked').val().toLowerCase() == 'true' ? (quantity + _quantity) : (quantity - _quantity);
            $('#StockInfo_Quantity').val(quantity);
            $('#frmStockTransactionInfo').clearForm();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br /> ' + data.ExceptionMessage, duration, data.InnerException);
    }

    var updateStockSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            $('#StockInfo_StockID').val(data.ItemID);
            $('#StockTransactionInfo_Stock').val(data.ItemID);
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br /> ' + data.ExceptionMessage, duration, data.InnerException);
    }

    return {
        init: init,
        accountingAccountsTable: accountingAccountsTable,
        pointsOfSaleTable: pointsOfSaleTable,
        searchResultsTable: searchResultsTable,
        saveActivitySuccess: saveActivitySuccess,
        deleteRowFunctionality: deleteRowFunctionality,
        makeTableRowsSelectable: makeTableRowsSelectable,
        saveMeetingPointSuccess: saveMeetingPointSuccess,
        saveActivityAccountingAccountSuccess: saveActivityAccountingAccountSuccess,
        savePointOfSaleSuccess: savePointOfSaleSuccess,
        saveActivityScheduleSuccess: saveActivityScheduleSuccess,
        updateUlActivityDescriptions: updateUlActivityDescriptions,
        updateTableAccountingAccounts: updateTableAccountingAccounts,
        updateTablePointsOfSale: updateTablePointsOfSale,
        updateDrpsActivityDependents: updateDrpsActivityDependents,
        updateUlActivityMeetingPoints: updateUlActivityMeetingPoints,
        makeTblCategoriesRowsRemovable: makeTblCategoriesRowsRemovable,
        updateUlActivityAvailabilities: updateUlActivityAvailabilities,
        saveActivityDescriptionSuccess: saveActivityDescriptionSuccess,
        makeTblAccountingAccountsRowsSelectable: makeTblAccountingAccountsRowsSelectable,
        makeTblPointsOfSaleRowsSelectable: makeTblPointsOfSaleRowsSelectable,
        saveCategoriesToActivitySuccess: saveCategoriesToActivitySuccess,
        makeUlActivitySchedulesRowsSelectable: makeUlActivitySchedulesRowsSelectable,
        makeUlActivityDescriptionsRowsSelectable: makeUlActivityDescriptionsRowsSelectable,
        makeUlActivityMeetingPointsRowsSelectable: makeUlActivityMeetingPointsRowsSelectable,
        makeTblActivityAccountingAccountsRowsSelectable: makeTblActivityAccountingAccountsRowsSelectable,
        deleteActivityAccountingAccount: deleteActivityAccountingAccount,
        updateTblActivityAccountingAccounts: updateTblActivityAccountingAccounts,
        saveAccountingAccountSuccess: saveAccountingAccountSuccess,
        searchPriceTypeRulesResults: searchPriceTypeRulesResults,
        updateTblPriceTypeRules: updateTblPriceTypeRules,
        makeTblRulesSelectable: makeTblRulesSelectable,
        savePriceTypeRuleSuccess: savePriceTypeRuleSuccess,
        updateServicesDependantLists: updateServicesDependantLists,
        preselectTerminalOnActivityInfo: preselectTerminalOnActivityInfo,
        actionsAfterRenderRulesTable: actionsAfterRenderRulesTable,
        updatePriceTypesInRules: updatePriceTypesInRules,
        searchFuturePriceTypeRulesResults: searchFuturePriceTypeRulesResults,
        makeFutureRulesSelectable: makeFutureRulesSelectable,
        searchStockTransactionsResultsTable: searchStockTransactionsResultsTable,
        saveStockTransactionSuccess: saveStockTransactionSuccess,
        updateStockSuccess: updateStockSuccess
    }
}();

var PREVACTIVITY = function () {

    var oTable;

    var init = function () {

        PREVACTIVITY.searchResultsTable($('#tblPrevSearchActivitiesResults'));

        PREVACTIVITY.table = $('#tblSearchActivitiesResults').dataTable();

        $('#btnImportActivitiesInfo').on('click', function () {
            $(document).find('.selected-row').each(function () {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            });
            $('#fdsImportActivities').show();
            UI.expandFieldset('fdsImportActivities');
            UI.scrollTo('fdsImportActivities', null);
        });

        $('#frmPrevSearchActivities').on('submit', function () {
            $('#PrevSearch_Terminals').val($('#Search_Terminals').val());
        });

        $('#btnPrevSearchActivities').on('click', function () {
            $('#frmPrevSearchActivities').submit();
        });

        $('#btnNewActivitiesInfo').on('click', function () {
            $('#fdsImportActivities').hide();
        });

        $('#btnPrevImportActivity').on('click', function () {
            var descriptions = '';
            var schedules = '';
            var prices = '';
            if ($('#liNoImportDescriptions').length == 0) {
                $('#ulImportDescriptions').find('input:checkbox:checked').each(function () {
                    descriptions += $(this).attr('id').substr(17) + ',';
                });
                descriptions = descriptions.substr(0, descriptions.length - 1);
            }
            if ($('#liNoImportSchedules').length == 0) {
                $('#ulImportSchedules').children('li').find('input:checkbox:checked').each(function (index, item) {
                    schedules += $(this).attr('id').substr(14) + '_';
                    if ($(this).parent('li').nextAll().eq(1).is('ul')) {
                        $(this).parent('li').nextAll().eq(1).find('input:checkbox:checked').each(function (index2, item2) {
                            schedules += $(this).attr('id').substr(18) + '-';
                        });
                    }
                    schedules = schedules.substr(0, schedules.length - 1) + ',';
                });
                schedules = schedules.substr(0, schedules.length - 1);
            }
            if ($('#liNoImportPrices').length == 0) {
                $('#ulImportPrices').find('input:checkbox:checked').each(function () {
                    prices += $(this).attr('id').substr(11) + ',';
                });
                prices = prices.substr(0, prices.length - 1);
            }
            $('#ActivityImportInfo_Descriptions').val(descriptions);
            $('#ActivityImportInfo_Availability').val(schedules);
            $('#ActivityImportInfo_Prices').val(prices);
            $('#frmImportActivity').submit();
        });
    }

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblPrevSearchActivitiesResults', tableColumns.length - 1);
        PREVACTIVITY.oTable = $('#tblPrevSearchActivitiesResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { PREVACTIVITY.makeTableRowsSelectable(); });
        //});
        //$('#tblPrevSearchActivitiesResults_length').unbind('change').on('change', function () {
        //    PREVACTIVITY.makeTableRowsSelectable();
        //});
    }

    var makeTableRowsSelectable = function () {
        //$('#tblPrevSearchActivitiesResults tbody tr').not('theader').unbind('click').on('click', function () {
        PREVACTIVITY.oTable.$('tr').not('theader').unbind('click').on('click', function () {
            if (!$(this).hasClass('selected-row')) {
                $(document).find('.selected-row').each(function () {
                    var event = $.Event('keydown');
                    event.keyCode = 27;
                    $(document).trigger(event);
                });
                PREVACTIVITY.oTable.$('tr.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $('#ActivityImportInfo_PrevActivityID').val($(this).attr('id').substr(14));
                //body of table row selection
                $.ajax({
                    url: '/Activities/GetPrevActivitySettings',
                    cache: false,
                    type: 'POST',
                    data: { activityID: $('#ActivityImportInfo_PrevActivityID').val() },
                    beforeSend: function (xhr) {
                        UI.checkForPendingRequests(xhr);
                    },
                    success: function (data) {
                        PREVACTIVITY.updateUlImportDescriptions(data);
                        PREVACTIVITY.updateUlImportPrices(data);
                        PREVACTIVITY.updateUlImportSchedules(data);
                    }
                });
                $('#fdsImportActivityInfo').show();
                UI.expandFieldset('fdsImportActivityInfo');
                UI.scrollTo('fdsImportActivityInfo', null);
            }
        });
    }

    var updateUlImportDescriptions = function (data) {
        $('#ulImportDescriptions').empty();
        $('#ulImportDescriptionsHeader').remove();
        $('#pSelectAllImportDescriptions').remove();
        var builderDescriptions = '';
        if (data.ActivityImportInfo_ListDescriptions.length > 0) {
            var selectAllBuilder = '<p class="view-restricted" id="pSelectAllImportDescriptions"><input type="checkbox" class="chk-parent"/>Select All</p>';
            var importDescriptionsHeader = '<div id="ulImportDescriptionsHeader" class="view-restricted half-width left-ul-header-alignment">'
    //+ '<div class="li-fix-column"><span>Description</span></div>'
    + '<div class="li-fix-column"><span>Language</span></div></div>';
            $.each(data.ActivityImportInfo_ListDescriptions, function (index, item) {
                builderDescriptions += '<li>'
                    + '<input type="checkbox" class="chk-son" id="importDescription' + item.Value + '"/>'
                    //+ '<p class="li-fix-column">' + item.Value + '</p>'
                    + '<p class="li-fix-column">' + item.Text + '</p></li>';
            });
        }
        else
            builderDescriptions += '<li id="liNoImportDescriptions">No Descriptions</li>';
        $(selectAllBuilder).insertBefore($('#ulImportDescriptions'));
        $(importDescriptionsHeader).insertBefore($('#ulImportDescriptions'));
        $('#ulImportDescriptions').append(builderDescriptions);
        PREVACTIVITY.checkAllCheckBoxes('divImportDescriptions');
    }

    var updateUlImportSchedules = function (data) {
        $('#ulImportSchedules').empty();
        $('#ulImportSchedulesHeader').remove();
        $('#pSelectAllImportSchedules').remove();
        var importSchedulesHeader = '';
        var builderSchedules = '<div id="ulImportSchedulesHeader" class="view-restricted half-width">'
    + '<div class="li-fix-column"><span>Time</span></div>'
    + '<div class="li-fix-column"><span>Days</span></div></div>';
        if (data.ActivityImportInfo_ListAvailability.length > 0) {
            var selectAllBuilder = '<p class="view-restricted" id="pSelectAllImportSchedules"><input type="checkbox" class="chk-parent"/>Select All</p>';
            $.each(data.ActivityImportInfo_ListAvailability, function (index, item) {
                builderSchedules += '<li>'
                    + '<input type="checkbox" class="chk-son" id="importSchedule' + item.ActivityImportAvailability_AvailabilityID + '"/>'
                    + '<div class="" style="width: 70px; display:inline-block"><span>' + item.ActivityImportAvailability_FromTime + '</span></div>';
                if (item.ActivityImportAvailability_Monday == true)
                    builderSchedules += '<div class="weekdays-alignment "><span>Monday</span></div>';
                if (item.ActivityImportAvailability_Tuesday == true)
                    builderSchedules += '<div class="weekdays-alignment"><span>, Tuesday</span></div>';
                if (item.ActivityImportAvailability_Wednesday == true)
                    builderSchedules += '<div class="weekdays-alignment"><span>, Wednesday</span></div>';
                if (item.ActivityImportAvailability_Thursday == true)
                    builderSchedules += '<div class="weekdays-alignment"><span>, Thursday</span></div>';
                if (item.ActivityImportAvailability_Friday == true)
                    builderSchedules += '<div class="weekdays-alignment"><span>, Friday</span></div>';
                if (item.ActivityImportAvailability_Saturday == true)
                    builderSchedules += '<div class="weekdays-alignment"><span>, Saturday</span></div>';
                if (item.ActivityImportAvailability_Sunday == true)
                    builderSchedules += '<div class="weekdays-alignment"><span>, Sunday</span></div>';
                builderSchedules += '</li>';

                if (item.ActivityImportAvailability_MeetingPoint.length > 0) {
                    builderSchedules += '<div class="left-ul-header-alignment align-from-top"><div class="li-fix-column"><span>Place</span></div>'
                + '<div class="li-fix-column"><span>Time</span></div></div>';
                    builderSchedules += '<ul>';
                    $.each(item.ActivityImportAvailability_MeetingPoint, function (index2, item2) {
                        builderSchedules += '<li>'
                            + '<input type="checkbox" class="chk-son" id="importMeetingPoint' + item2.ActivityImportMeetingPoint_MeetingPointID + '"/>'
                            + '<div class="li-fix-column"><span>' + item2.ActivityImportMeetingPoint_Place + '</span></div>'
                            + '<div class="li-fix-column"><span>' + item2.ActivityImportMeetingPoint_Time + '</span></div></li>'
                    });
                    builderSchedules += '</ul>';
                }

            });
        }
        else
            builderSchedules += '<li id="liNoImportSchedules">No Schedules</li>';
        $(selectAllBuilder).insertBefore($('#ulImportSchedules'));
        $(importSchedulesHeader).insertBefore($('#ulImportSchedules'));
        $('#ulImportSchedules').append(builderSchedules);
        PREVACTIVITY.checkTwoLevelCheckBoxes('divImportSchedules');
    }

    var updateUlImportPrices = function (data) {
        $('#ulImportPrices').empty();
        $('#ulImportPricesHeader').remove();
        $('#pSelectAllImportPrices').remove();
        var builderPrices = '';
        var importPricesHeader = '<div id="ulImportPricesHeader" class="view-restricted half-width left-ul-header-alignment">'
            + '<div class="li-fix-column"><span>Price</span></div>'
            + '<div class="li-fix-column"><span>Currency</span></div>'
            + '<div class="li-fix-column"><span>Unit</span></div></div>';
        if (data.ActivityImportInfo_ListPrices.length > 0) {
            var selectAllBuilder = '<p class="view-restricted" id="pSelectAllImportPrices"><input type="checkbox" class="chk-parent"/>Select All</p>';
            $.each(data.ActivityImportInfo_ListPrices, function (index, item) {
                builderPrices += '<li>'
                    + '<input type="checkbox" class="chk-son" id="importPrice' + item.ActivityImportPrices_PriceID + '"/>'
                    + '<div class="li-fix-column"><span>' + item.ActivityImportPrices_Price + '</span></div>'
                    + '<div class="li-fix-column"><span>' + item.ActivityImportPrices_Currency + '</span></div>';
                if (item.ActivityImportPrices_Unit != null)
                    builderPrices += '<div class="li-fix-column"><span>' + item.ActivityImportPrices_Unit + '</span></div>';
                builderPrices += '</li>';
            });
        }
        else
            builderPrices += '<li id="liNoImportPrices">No Prices</li>';
        $(selectAllBuilder).insertBefore($('#ulImportPrices'));
        $(importPricesHeader).insertBefore($('#ulImportPrices'));
        $('#ulImportPrices').append(builderPrices);
        PREVACTIVITY.checkAllCheckBoxes('divImportPrices');
    }

    var checkAllCheckBoxes = function (container) {
        if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
            $('#' + container).find('.chk-parent').attr('checked', 'checked');
        }
        $('#' + container).find('.chk-parent').change(function () {
            $('#' + container).find('.chk-son').attr('checked', this.checked);
        });
        $('#' + container).find('.chk-son').change(function (e) {
            if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
                $('#' + container).find('.chk-parent').attr('checked', 'checked');
            }
            else {
                $('#' + container).find('.chk-parent').attr('checked', false);
            }
        });
    }

    var checkTwoLevelCheckBoxes = function (container) {
        $('#' + container).find('input:checkbox').unbind('click').on('click', function (e) {
            var $children = $(this).parent('li').nextAll('ul').first().find('li input:checkbox');
            $children.parents('ul').first().prevAll('li').first().find('input:checkbox').first().on('change', function (e) {
                if ($(e.target).is(':checked'))
                    $(this).parents('li').first().nextAll('ul').first().find('input:checkbox').attr('checked', 'checked');
                else
                    $(this).parents('li').first().nextAll('ul').first().find('input:checkbox').attr('checked', false);
            });
            $(this).on('change', function () {
                var parent = $(e.target).parents('ul').first().prevAll('li').first().find('input:checkbox').first();
                var anyChildrenChecked = $(this).parents('ul').first().find('li input:checkbox').is(':checked');
                $(parent).attr('checked', anyChildrenChecked);
            });
        });
        PREVACTIVITY.checkAllCheckBoxes(container);
    }

    var importActivitySettingsSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Service Imported') {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
                var oSettings = ACTIVITY.oTable.fnSettings();
                var iAdded = ACTIVITY.oTable.fnAddData([
                    data.ItemID.id,
                    data.ItemID.activity,
                    $('#chkTerminal' + data.ItemID.terminal).next('label').text(),
                    '',
                    'False<img class="right" src="/Content/themes/base/images/cross.png" id="delP' + data.ItemID.id + '">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'trActivity' + data.ItemID.id);
                ACTIVITY.oTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                ACTIVITY.makeTableRowsSelectable();
                $('#trActivity' + data.ItemID.id).click();
                $('#fdsImportActivities').hide();
                UI.expandFieldset('fdsActivityCategories');
                UI.scrollTo('fdsActivityCategories', null);
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    return {
        init: init,
        searchResultsTable: searchResultsTable,
        checkAllCheckBoxes: checkAllCheckBoxes,
        updateUlImportPrices: updateUlImportPrices,
        makeTableRowsSelectable: makeTableRowsSelectable,
        updateUlImportSchedules: updateUlImportSchedules,
        checkTwoLevelCheckBoxes: checkTwoLevelCheckBoxes,
        updateUlImportDescriptions: updateUlImportDescriptions,
        importActivitySettingsSuccess: importActivitySettingsSuccess
    }
}();