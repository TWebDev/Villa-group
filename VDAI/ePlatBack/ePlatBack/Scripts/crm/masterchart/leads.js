/// <reference path="/Scripts/Utils.js" />
/// <reference path="/Scripts/layout/ui.js" />
/// <reference path="../../Settings.js" />

///handlers

$(function () {
    // since the lead search is based on the terminals and is not restricted by workgroups, then the next line is not needed anymore.
    //$('#divAvailableWorkGroups').click(function () { location.reload(); window.setTimeout(function () { $.blockUI(); }, 300);  });

    COMMON.getServerDateTime();
    PURCHASE.allowAccessToServices();

    $('[data-table]').each(function () {
        var tableColumns = $(this).find('tbody tr').first().find('td');
        UI.searchResultsTable($(this).attr('id'), tableColumns.length - 1);
    });

    LEAD_GENERAL_INFORMATION.init();
    SEARCH.init();
    BILLING_INFO.init();
    IMPORT.init();
    //PURCHASE.init();
    //ACCOUNT.init();
    RESERVATION_GENERAL_INFORMATION.init();

    if (UTILS.isSetURLParameter('referralID') || UTILS.isSetURLParameter('leadID')) {

        $('#btnNewLead').hide();
        $('#fdsLeadSearch').hide();
        $('#fdsMassUpdate').hide();
        $('#fdsLeadImport').hide();
        //$('#pageTitle').html('Edit Referral');

        var currentTargetID = (UTILS.isSetURLParameter('referralID')) ? UTILS.getURLParameter('referralID') : UTILS.getURLParameter('leadID');

        var params = { currentTargetID: currentTargetID }
        REQUEST_HANDLERS.findLead(params);

        $(window).unload(function () {
            if (window.opener != null) {
                //window.opener.LEAD_GENERAL_INFORMATION.cancelEditReferral();
                var tableID = UTILS.getURLParameter('tableID');
                window.opener.UI.deselectAllSelectedRows(tableID);
                window.close();
            }
        });

    } else {
        UI.expandFieldset('fdsLeadSearch');
        $(window).unload(function () {
            //everytime we leave from the page, make sure we close the child Window
            var childWindow = window.open('', 'Masterchart');
            if (childWindow != null) {
                childWindow.close();
            }
        });
    }
});

var RESPONSE_HANDLERS = function () {
    function searchLead(response) {
        try {
            LEAD_GENERAL_INFORMATION.resetAll();
            if (PURCHASE.oCouponsTable != undefined) {
                PURCHASE.oCouponsTable.$('tr.selected-row').removeClass('selected-row primary');
            }
            var tableColumns = $(response).find("tbody").first("tr").find("td");
            //, tableColumns.length - 1  // we removed this part from the next function call
            // because aparently it is only needed when using colspan on the table.


            //latter remove this functionality, so the users have to click on the 

            //UI.searchResultsTable('tblSearchResult_Leads');
            UI.setTableRowsClickable({
                tblID: "tblSearchResult_Leads",
                onClickCallbackFunction: REQUEST_HANDLERS.findLead
            });

            //--//--//-

            //$("#fdsLeadSearch").each(function () {
            //    UI.collapseFieldset($(this).attr('id'));
            //}).queue(function () {
            //    UI.expandFieldset("fdsLeadSearchResults");
            //    $(this).dequeue();
            //}).queue(function () {
            //    UI.scrollTo("fdsLeadSearch");
            //    $(this).dequeue();
            //});

            $('#fdsLeadSearchResults').show();
            COMMON.collapseAndExpand('#fdsLeadSearch, #fdsAccountancyManagement', '#fdsLeadSearchResults').done(COMMON.executeDelayed(UI.scrollTo, "fdsLeadSearchResults"));
            //$('#fdsMassUpdate').show();
            //--///--//--


            //UI.collapseFieldset("fdsLeadSearch");
            //UI.expandFieldset("fdsLeadSearchResults");
            //UI.scrollTo("fdsLeadSearch");
            SEARCH.searchResultsTable(response);
            SEARCH.bindFunctionsToTable();

            //**this code show en hide columns in tblSearchResult_Leads for reservations and services users

            //var oTable = $('#tblSearchResult_Leads').dataTable();
            var showServices = $('#ShowServices').val() == 'True' ? true : false;
            var services = new Array();
            $('#tblSearchResult_Leads thead th.services-info').each(function (index, item) {
                services[index] = $(this).index();
            });
            services.reverse();
            $.each(services, function (index, item) {
                //oTable.fnSetColumnVis(item, showServices);
                SEARCH.oTable.fnSetColumnVis(item, showServices);
            });
            var showReservations = $('#ShowReservations').val() == 'True' ? true : false;
            var reservations = new Array();
            $('#tblSearchResult_Leads thead th.reservation-info').each(function (index, item) {
                reservations[index] = $(this).index();
            });
            reservations.reverse();
            $.each(reservations, function (index, item) {
                //oTable.fnSetColumnVis(item, showReservations);
                SEARCH.oTable.fnSetColumnVis(item, showReservations);
            });
            UI.showCommentsOnHover(SEARCH.oTable);
            //autoselect row when its only one
            if (SEARCH.oTable.$('tr').length == 1) {
                ///mike
                SEARCH.oTable.$('tr:first').click();
                //if (!showReservations) {
                //    SEARCH.oTable.$('tr:first').click();
                //}
                //end mike
            }
            else {
                if (location.hash != '') {
                    location.href = location.hash.substr('#')[0];
                }
            }
            //**
            LEAD_MASS_UPDATE.init();

            UI.Notifications.workingOn("CRM > MasterChart");
        }
        catch (ex) { }
    }

    function saveLead(response) {
        $("#GeneralInformation_LeadID").val(response.GeneralInformation_LeadID);
        //get composed or calculated values for the User notification
        var duration = (response.ResponseType < 0) ? duration = response.ResponseType : null;
        var message = response.ResponseMessage;
        if (response.ExceptionMessage != "") { message += "<br />" + response.ExceptionMessage; }

        //var m = {
        //    type: response.ResponseType,
        //    message: message,
        //    duration: duration,
        //    innerException: response.InnerException
        //}
        //UI.messageBox(m);

        if (response.ResponseType == 1) {
            var leadID = $("#GeneralInformation_LeadID").val();
            if (LEAD_GENERAL_INFORMATION.saveContinueFlag == true) {
                //var nextLeadID = $('#tblSearchResult_Leads tbody').find('#' + leadID).next().attr('id');
                var nextLeadID = $(SEARCH.oTable.$('tr#' + leadID)[0]).next().attr('id');
                if (nextLeadID != undefined) {
                    leadID = nextLeadID;
                    message += '<br />' + 'Next Lead Selected';
                    //$('#tblSearchResult_Leads tbody').find('tr.selected-row').removeClass('selected-row primary');
                    //$('#' + leadID).addClass('selected-row primary');
                    SEARCH.oTable.$('tr.selected-row').removeClass('selected-row primary');
                    SEARCH.oTable.$('tr#' + leadID).addClass('selected-row primary');
                }
                else {
                    var currentPage = SEARCH.oTable.fnPagingInfo().iPage;
                    SEARCH.oTable.fnPageChange('next');
                    if (currentPage == SEARCH.oTable.fnPagingInfo().iPage) {
                        message += '<br />' + 'Last Lead in List. Same Lead Selected';
                    }
                    else {
                        leadID = $('#tblSearchResult_Leads tbody tr').first().attr('id');
                        message += '<br />' + 'Next Lead Selected';
                        SEARCH.oTable.$('tr.selected-row').removeClass('selected-row primary');
                        SEARCH.oTable.$('tr#' + leadID).addClass('selected-row primary');
                    }
                    //var lastRow = $('#tblSearchResult_Leads tbody tr').first().attr('id');

                    //$('#tblSearchResult_Leads_next').click();
                    //if ($('#tblSearchResult_Leads tbody tr').first().attr('id') == lastRow) {
                    //    leadID = $('#tblSearchResult_Leads tbody tr').last().attr('id');
                    //    message += '<br />' + 'Last Lead in List. Same Lead Selected';
                    //}
                }
                var params = { currentTargetID: leadID };
                REQUEST_HANDLERS.findLead(params);
            }
            LEAD_GENERAL_INFORMATION.saveContinueFlag = false;
            //var leadID=$("#GeneralInformation_LeadID").val();
            var params = { currentTargetID: leadID };
            ///mike
            if (response.ResponseMessage == 'Lead Saved') {
                $('#frmLeadSearch').clearForm();
                $('#Search_LeadID').val(leadID);
                $('#frmLeadSearch').submit();
            }
            ///end mike

            //prevents find lead that is not in results table drawn yet.
            //if (response.ResponseMessage == 'Lead Saved' && UI.selectedWorkGroup == '19') {
            //    var oSettings = SEARCH.oTable.fnSettings();
            //    var iAdded = SEARCH.oTable.fnAddData([
            //        $('#GeneralInformation_FirstName').val(),
            //        $('#GeneralInformation_LastName').val(),
            //        '',
            //        'In Process',
            //        '',
            //        '',
            //        'Unpaid',

            //    ]);
            //    var aRow = oSettings.aoData[iAdded[0]].nTr;
            //    aRow.setAttribute('id', data.ItemID);
            //    SEARCH.oTable.fnDisplayRow(aRow);
            //    UI.tablesHoverEffect();
            //}

            //REQUEST_HANDLERS.findLead(params);

            //RESERVATION_GENERAL_INFORMATION.show(leadID);  //this line generates an extra ajax call
            //var m = {
            //    type: response.ResponseType,
            //    message: message,
            //    duration: duration,
            //    innerException: response.InnerException
            //}
            //UI.messageBox(m);
            //show reservations panel

            if (window.opener != null) {
                if (UTILS.isSetURLParameter('leadID')) {
                    window.opener.REQUEST_HANDLERS.searchReferringLead(UTILS.getURLParameter('leadID'));
                } else {
                    window.opener.REQUEST_HANDLERS.searchReferrals(UTILS.getURLParameter('referredByID'));
                }
                //since unload was set onload, for the cases where users don't save anything, here we unbind it, because now we want to do a different action
                $(window).unbind('unload');

                // //if we are going to close automatically the window after saving a referral, then we should alert the confirmation message
                // //since the UI.messageBox won't be seen.

                // //if all Ok close window incase it is "modal".
                //alert(m.message);
                //window.close();             
            }

        }
        var m = {
            type: response.ResponseType,
            message: message,
            duration: duration,
            innerException: response.InnerException
        }
        UI.messageBox(m);
    }
    function deleteLead(jqXHR, textStatus) {
        if (textStatus == "success") {
            //submit the search form again, so the table get updated
            $("#frmLeadSearch").submit();
            //Reset the form keeping the predefined values
            $("#frmLeadGeneralInformation").clearForm();
            UI.messageBox(1, "Lead deleted.");
        }
        else {
            UI.messageBox(-1, "Error Deleting Lead Information.");
        }
    }
    function findLead(jqXHR, textStatus) { // posible testStatuses:
        //"success", "notmodified", "error", "timeout", "abort", or "parsererror"    
        LEAD_GENERAL_INFORMATION.init();

        if (textStatus == "success") {
            var json = $.parseJSON(jqXHR.responseText);
            var toIgnore = [
                'GeneralInformation_InputMethodID',
                'GeneralInformation_Email'
            ];


            // fill json fields
            LEAD_GENERAL_INFORMATION.resetAll();

            UI.collapseFieldset('fdsLeadSearch');
            UI.validateForm('frmLeadGeneralInformation');

            if (UTILS.isSetURLParameter('leadID') || UTILS.isSetURLParameter('referralID')) {
                if (json.leadDetails.GeneralInformation_LeadID == null) {
                    alert('You are not allowed to view details of this lead.');
                    window.close();
                }
            }

            //make sure we clear the data-selected-value from PlaceID (for example)
            // we can do this using [data-single-item-method] as selector.
            $("[data-cascading-feeding-method]").data("selected-value", "");


            ////make sure the form is validated again, so we can remove unaccurate red fields.
            //var hls = new SETTINGS.highlightElements();
            //hls.highlightTime = -1
            //hls.elements = UI.validateRelatedElements("frmLeadGeneralInformation");

            //UI.highlightElements(hls);

            //UI.validateForm("frmLeadGeneralInformation");

            //UTILS.jsonToFormFields(json.leadDetails);

            //make sure the form is validated again, so we can remove unaccurate red fields.
            UTILS.jsonToFormFields(json.leadDetails);
            var hls = new SETTINGS.highlightElements();
            hls.highlightTime = -1
            hls.elements = UI.validateRelatedElements("frmLeadGeneralInformation");
            UI.highlightElements(hls);
            UI.validateForm("frmLeadGeneralInformation");

            var leadID = $("#GeneralInformation_LeadID").val();
            ///

            //show other lead-dependant panels
            COMMON.showRelatedForms('frmLeadGeneralInformation', leadID);
            PURCHASE.show(leadID);//line added to show Purchases
            $('#frmLeadGeneralInformation').show();
            $('#fdsLeadInformation').show();

            if ($('#uservices').val() == 'true') {
                COMMON.collapseAndExpand('#fdsLeadInformation > fieldset, #fdsLeadInformation', '#fdsPurchasesManagement');
                COMMON.expandAndCollapse('#fdsBillingInfo', '#fdsBillingInfo, #fdsBillingInfo > fieldset');
            }
            else {
                var _referredByID = json.leadDetails['GeneralInformation_ReferredByID'];
                if (_referredByID != null) {
                    REQUEST_HANDLERS.searchReferringLead(_referredByID);
                } else {
                    UTILS.clearTableRows('tblGeneralInformation_ReferredBy');
                }

                ///
                REQUEST_HANDLERS.searchReferrals(leadID);

                //item logs
                //var params = { referenceID: leadID, referenceText: 'Lead' };
                //REQUEST_HANDLERS.getItemLogs(params);

                var fdsReferralInfo = $("#fdsLeadGeneralInformation_CurrentReferrals");
                if (!fdsReferralInfo.is(':visible')) {
                    fdsReferralInfo.show();
                }

                $('#btnNewReferrals').unbind('click');
                //make sure we update the leadID so we dont add referrals to the wrong lead.
                $('#btnNewReferrals').on('click', function () {
                    LEAD_GENERAL_INFORMATION.addReferral(leadID);
                });

                RESERVATION_GENERAL_INFORMATION.show(leadID);

                var reservationsForm = $("#frmReservationGeneralInformation");
                if (!reservationsForm.is(':visible')) {
                    reservationsForm.show();
                }

                var cr = COMMON.collapseAndExpand('#fdsReservations #fdsCurrentReservations ~ fieldset', '#fdsReservations, #fdsCurrentReservations');
                //var cb = COMMON.collapseAndExpand('#fdsBillingInfo #fdsCurrentBillingInfo ~ fieldset', '#fdsBillingInfo, #fdsCurrentBillingInfo');
                var cl = COMMON.collapseAndExpand('#fdsLeadSearchResults', '#fdsLeadInformation, #fdsLeadGeneralInformation', 'fdsLeadGeneralInformation');
                var f = COMMON.focusFirstEditableElement('fdsLeadGeneralInformation');
                $.when(cr, cl, f).done(function () {
                    UI.scrollTo('fdsLeadGeneralInformation');
                    if ($('#tblGeneralInformation_Reservations tbody tr').length > 0) {
                        $('#tblGeneralInformation_Reservations tbody tr:first').click();
                    }
                });
            }


            UI.Notifications.workingOn("CRM > MasterChart > " + json.leadDetails.GeneralInformation_FirstName + " " + json.leadDetails.GeneralInformation_LastName);
        }
        else {
            var message = {
                type: -1,
                message: "Error Retrieving Lead Information",
                duration: null,
                innerException: jqXHR.responseText
            }
            UI.messageBox(message);
        }

    }
    //function findLead(jqXHR, textStatus) { // posible testStatuses:
    //    //"success", "notmodified", "error", "timeout", "abort", or "parsererror"    

    //    LEAD_GENERAL_INFORMATION.init();

    //    if (textStatus == "success") {
    //        var json = $.parseJSON(jqXHR.responseText);
    //        var toIgnore = [
    //                        'GeneralInformation_InputMethodID',
    //                        'GeneralInformation_Email'
    //        ];


    //        // fill json fields

    //        LEAD_GENERAL_INFORMATION.resetAll();

    //        UI.collapseFieldset('fdsLeadSearch');
    //        UI.validateForm('frmLeadGeneralInformation');

    //        if (UTILS.isSetURLParameter('leadID') || UTILS.isSetURLParameter('referralID')) {
    //            if (json.leadDetails.GeneralInformation_LeadID == null) {
    //                alert('You are not allowed to view details of this lead.');
    //                window.close();
    //            }
    //        }

    //        //make sure we clear the data-selected-value from PlaceID (for example)
    //        // we can do this using [data-single-item-method] as selector.
    //        $("[data-cascading-feeding-method]").data("selected-value", "");

    //        UTILS.jsonToFormFields(json.leadDetails);
    //        var _referredByID = json.leadDetails['GeneralInformation_ReferredByID'];
    //        if (_referredByID != null) {
    //            REQUEST_HANDLERS.searchReferringLead(_referredByID);
    //        } else {
    //            UTILS.clearTableRows('tblGeneralInformation_ReferredBy');
    //        }
    //        //make sure the form is validated again, so we can remove unaccurate red fields.
    //        var hls = new SETTINGS.highlightElements();
    //        hls.highlightTime = -1
    //        hls.elements = UI.validateRelatedElements("frmLeadGeneralInformation");

    //        UI.highlightElements(hls);

    //        UI.validateForm("frmLeadGeneralInformation");

    //        var leadID = $("#GeneralInformation_LeadID").val();

    //        REQUEST_HANDLERS.searchReferrals(leadID);

    //        //item logs
    //        //var params = { referenceID: leadID, referenceText: 'Lead' };
    //        //REQUEST_HANDLERS.getItemLogs(params);

    //        var fdsReferralInfo = $("#fdsLeadGeneralInformation_CurrentReferrals");
    //        if (!fdsReferralInfo.is(':visible')) {
    //            fdsReferralInfo.show();
    //        }

    //        $('#btnNewReferrals').unbind('click');
    //        //make sure we update the leadID so we dont add referrals to the wrong lead.
    //        $('#btnNewReferrals').on('click', function () {
    //            LEAD_GENERAL_INFORMATION.addReferral(leadID);
    //        });

    //        RESERVATION_GENERAL_INFORMATION.show(leadID);

    //        var reservationsForm = $("#frmReservationGeneralInformation");
    //        if (!reservationsForm.is(':visible')) {
    //            reservationsForm.show();
    //        }

    //        //show other lead-dependant panels
    //        COMMON.showRelatedForms('frmLeadGeneralInformation', leadID);
    //        PURCHASE.show(leadID);//line added to show Purchases
    //        $('#frmLeadGeneralInformation').show();
    //        $('#fdsLeadInformation').show();
    //        if (UI.selectedWorkGroup != '19') {
    //            var cr = COMMON.collapseAndExpand('#fdsReservations #fdsCurrentReservations ~ fieldset', '#fdsReservations, #fdsCurrentReservations');
    //            //var cb = COMMON.collapseAndExpand('#fdsBillingInfo #fdsCurrentBillingInfo ~ fieldset', '#fdsBillingInfo, #fdsCurrentBillingInfo');
    //            var cl = COMMON.collapseAndExpand('#fdsLeadSearchResults', '#fdsLeadInformation, #fdsLeadGeneralInformation', 'fdsLeadGeneralInformation');
    //            var f = COMMON.focusFirstEditableElement('fdsLeadGeneralInformation');
    //            $.when(cr, cl, f).done(function () {
    //                UI.scrollTo('fdsLeadGeneralInformation');
    //                if ($('#tblGeneralInformation_Reservations tbody tr').length > 0) {
    //                    $('#tblGeneralInformation_Reservations tbody tr:first').click();
    //                }
    //            });
    //        }
    //        else {
    //            //UI.collapseFieldset('fdsLeadSearchResults');
    //            COMMON.collapseAndExpand('#fdsLeadInformation > fieldset, #fdsLeadInformation', '#fdsPurchasesManagement');
    //            COMMON.expandAndCollapse('#fdsBillingInfo', '#fdsBillingInfo, #fdsBillingInfo > fieldset');
    //        }

    //        UI.Notifications.workingOn("CRM > MasterChart > " + json.leadDetails.GeneralInformation_FirstName + " " + json.leadDetails.GeneralInformation_LastName);
    //    }
    //    else {
    //        var message = {
    //            type: -1,
    //            message: "Error Retrieving Lead Information",
    //            duration: null,
    //            innerException: jqXHR.responseText
    //        }
    //        UI.messageBox(message);
    //    }

    //}
    function getItemLogs(jqXHR, textStatus) {
        if (textStatus == 'success') {
            var json = $.parseJSON(jqXHR.responseText);
            var builder = '';
            $.each(json, function (index, item) {
                builder += '<tr>'
                    + '<td>' + item.ItemLogs_Field + '</td>'
                    + '<td>' + item.ItemLogs_PreviousValue + '</td>'
                    + '<td>' + item.ItemLogs_CurrentValue + '</td>'
                    + '<td>' + item.ItemLogs_UserName + '</td>'
                    + '<td>' + item.ItemLogs_LogDateTime + '</td>'
                    + '</tr>';
            });

            $('#tblLeadChangesHistory tbody').empty();
            $('#tblLeadChangesHistory tbody').append(builder);
            UI.tablesStripedEffect();
        }
    }
    function searchReferrals(jqXHR, textStatus) {
        if (textStatus == "success") {
            var json = $.parseJSON(jqXHR.responseText);
            UTILS.jsonToFormFields(json);
        }
    }

    function searchReferringLead(jqXHR, textStatus) {
        if (textStatus == "success") {
            var json = $.parseJSON(jqXHR.responseText);
            UTILS.jsonToFormFields(json);
        }
    }

    //returns
    return {
        searchLead: searchLead,
        saveLead: saveLead,
        deleteLead: deleteLead,
        findLead: findLead,
        getItemLogs: getItemLogs,
        searchReferrals: searchReferrals,
        searchReferringLead: searchReferringLead
    }
}();
var REQUEST_HANDLERS = function () {
    function getCascadingDropDownItems(params) {
        var settings = new SETTINGS.getInfoAsync();
        settings.URL = "/CRM/masterchart/FindLead";
        settings.onCompleteCallback = RESPONSE_HANDLERS.findLead;
        settings.data = { leadID: params.currentTargetID };
        UTILS.getInfoAsync(settings);
    }

    function findLead(params) {
        var settings = new SETTINGS.getInfoAsync();
        settings.URL = "/CRM/masterchart/FindLead";
        settings.onCompleteCallback = RESPONSE_HANDLERS.findLead;
        settings.data = { leadID: params.currentTargetID };
        UTILS.getInfoAsync(settings);
    }

    function getItemLogs(params) {
        //$.ajax({
        //    url: '/CRM/masterchart/GetItemLogs',
        //    type: 'POST',
        //    cache: false,
        //    data: { referenceID: params.referenceID, referenceText: params.referenceText },
        //    success: function (data) {
        //        var builder = '';
        //        $.each(data, function (index, item) {
        //            builder += '<tr>'
        //                + '<td>' + item.ItemLogs_Field + '</td>'
        //                + '<td>' + item.ItemLogs_PreviousValue + '</td>'
        //                + '<td>' + item.ItemLogs_CurrentValue + '</td>'
        //                + '<td>' + item.ItemLogs_UserName + '</td>'
        //                + '<td>' + item.ItemLogs_LogDateTime + '</td>'
        //                + '</tr>';
        //        });

        //        $('#tblLeadChangesHistory tbody').empty();
        //        $('#tblLeadChangesHistory tbody').append(builder);
        //        UI.tablesStripedEffect();
        //    }
        //});
        var settings = new SETTINGS.getInfoAsync();
        settings.URL = '/CRM/masterchart/GetItemLogs';
        settings.onCompleteCallback = RESPONSE_HANDLERS.getItemLogs;
        settings.data = { referenceID: params.referenceID, referenceText: params.referenceText };
        UTILS.getInfoAsync(settings);
    }

    function searchReferrals(leadID) {

        if (leadID == null) {
            leadID = $("#GeneralInformation_LeadID").val();
        }

        var settings = new SETTINGS.getInfoAsync();
        settings.URL = "/CRM/masterchart/SearchReferrals";
        settings.onCompleteCallback = RESPONSE_HANDLERS.searchReferrals;
        settings.data = { leadID: leadID };
        UTILS.getInfoAsync(settings);
    }

    function searchReferringLead(leadID) {
        var settings = new SETTINGS.getInfoAsync();
        settings.URL = "/CRM/masterchart/SearchReferringLead";
        settings.onCompleteCallback = RESPONSE_HANDLERS.searchReferringLead;
        settings.data = { leadID: leadID };
        UTILS.getInfoAsync(settings);
    }

    function deleteLead(leadID, leadName) {
        var confirmation = confirm("Do you confirm you want to delete '" + leadName + "' ?");
        if (confirmation) {
            var settings = new SETTINGS.getInfoAsync();
            settings.URL = '/CRM/masterchart/DeleteLead';
            settings.onCompleteCallback = RESPONSE_HANDLERS.deleteLead;
            settings.data = { leadID: leadID };

            UTILS.getInfoAsync(settings);
        }
    }

    //returns
    return {
        findLead: findLead,
        getItemLogs: getItemLogs,
        deleteLead: deleteLead,
        searchReferrals: searchReferrals,
        searchReferringLead: searchReferringLead
    }
}();
var IMPORT = function () {

    var lastSection;

    var lastTables;

    var init = function () {
        //Begin: Import initialization
        $('#btnAddRow').on('click', function () {
            var rows = $('#tblImportedDataFormat tbody tr').length;
            var builder = '<tr id="trField' + rows + '">';
            builder += '<td>' + rows + '</td>';
            builder += '<td><input type="checkbox" class="comparable" /></td>';
            var selectBuilder = '<select class="list-section">';
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'new-row' }, function (data) {
                $.each(data, function (index, item) {
                    selectBuilder += '<option value="' + item.Value + '">' + item.Text + '</option>';
                });
                selectBuilder += '</select>';
                builder += '<td>' + selectBuilder + '</td>';//dataBase Field
                builder += '<td><select class="table-field"><option>--Select a Section--</option></select></td>';
                builder += '<td></td>';
                builder += '<td></td>';//default value
                builder += '<td><img src="/Content/themes/base/images/cross.png" class="remove-field" style="float:right"></td>';//example row data
                builder += '</tr>';
                $(builder).insertBefore('#trTableSettings');
                IMPORT.deleteTableRows();
                IMPORT.loadTableFields();
                IMPORT.createDefaultValueFields();
                UI.tablesStripedEffect();
            });
        });

        $('#btnUploadFile').on('click', function () {
            if ($('input[name="hasHeader"]:checked').length == 0) {
                UI.messageBox(0, 'Define if the file has headers row', null, null);
            }
        });

        $('input:radio[name="hasHeader"]').on('click', function () {
            $('#fileUploader').fineUploader({
                request: {
                    endpoint: '/MasterChart/UploadFile',
                    params: {
                        hasHeader: $('input:radio[name="hasHeader"]:checked').val()
                    }
                },
                button: $('#btnUploadFile'),
                multiple: false,
                validation: {
                    allowedExtensions: ['csv']
                },
                failedUploadTextDisplay: {
                    mode: 'none'
                }
            });
        });

        $('#fileUploader').on('complete', function (first, id, fileName, data) {
            $('#fileUploader').fineUploader('reset');//new code, recently added
            $('#tblImportedDataFormat tbody tr:not(:last)').remove();
            if (data["success"] != false) {
                $('#hdnFileName').val(fileName);
                $.each(data["listRows"], function (index, item) {
                    //var rows = $('#tblImportedDataFormat tbody tr').length;
                    var builder = '<tr id="trField' + (index + 1) + '">';
                    builder += '<td>' + (index + 1) + '</td>';
                    builder += '<td><input type="checkbox" class="comparable" /></td>';
                    var selectBuilder = '<select class="list-section">';
                    $.each(data["listSections"].listSections, function (index2, item2) {
                        selectBuilder += '<option value="' + item2.Value + '">' + item2.Text + '</option>';
                    });
                    selectBuilder += '</select>';
                    builder += '<td>' + selectBuilder + '</td>';//dataBase Field
                    builder += '<td><select class="table-field"><option>--Select a Section--</option></select></td>';
                    if ($('input:radio[name="hasHeader"]:checked').val() == 'True') {
                        builder += '<td>' + item.Header + '</td>';//header row
                    }
                    else {
                        builder += '<td></td>';//header row
                    }
                    builder += '<td></td>';//default value
                    builder += '<td>' + item.Value + '</td>';//example row data
                    builder += '</tr>';
                    $(builder).insertBefore('#trTableSettings');
                });
                IMPORT.loadTableFields();
                IMPORT.createDefaultValueFields();
                UI.tablesStripedEffect();
            }
            else {
                UI.messageBox(-1, 'File NOT uploaded', null, data["error"]);
            }
        });

        $('#btnImportLeads').on('click', function () {

            var comparableFlags = new Array();
            var fileDataArray = new Array();
            var tableFieldsArray = new Array();
            var defaultValuesArray = new Array();
            var secondaryValuesArray = new Array();
            var fileColumnsArray = new Array();
            var sectionsArray = new Array();

            var counter = 0;
            $('#tblImportedDataFormat tbody tr:not(:last)').each(function (index) {
                $tableRow = $(this);
                $tableFieldContainer = $tableRow.find('.table-field').first();
                if ($tableFieldContainer.val().substr(0, $tableFieldContainer.val().indexOf('_')) != 'noMap' && $tableFieldContainer.val().substr(0, $tableFieldContainer.val().indexOf('_')) != '') {
                    var tableData = $tableFieldContainer.val().substr($tableFieldContainer.val().indexOf('-') + 1, $tableFieldContainer.val().length).split(',');
                    var tableFields = $tableFieldContainer.val().substr(0, $tableFieldContainer.val().indexOf('-') + 1);
                    var secondaryValues = '';

                    //fill of defaultValuesArray -- values
                    if ($tableRow.children('td:nth-child(6)').children().is('input:checkbox'))
                        defaultValuesArray[counter] = $tableRow.children('td:nth-child(6)').children().is(':checked');
                    else
                        defaultValuesArray[counter] = $tableRow.children('td:nth-child(6)').children().val();

                    //fill of fileColumnsArray -- columns
                    fileColumnsArray[counter] = index + 1;

                    //fill of sectionsArray -- sections
                    //fill of tableFieldsArray --fields
                    $.each(tableData, function (index2, item) {
                        tableFields += item.substr(0, item.indexOf('_'));
                        if (tableData[index2 + 1] != undefined)
                            tableFields += ',';
                        sectionsArray[counter] = $tableRow.children('td:nth-child(3)').children('select').first().find('option:selected').val() + '_' +
                            $tableRow.children('td:nth-child(3)').children('select').first().find('option:selected').text().split(' ').join('');
                    });
                    tableFieldsArray[counter] = tableFields;

                    //fill of secondaryValuesArray - secValues
                    $tableRow.children('td:nth-child(4)').children(':not(.table-field)').each(function (index3) {
                        if ($(this).is('input:checkbox'))
                            secondaryValues += $(this).is(':checked') + ',';
                        if ($(this).is('select'))
                            secondaryValues += $(this).val() + ',';
                    });
                    secondaryValues = secondaryValues.substr(0, secondaryValues.length - 1);
                    secondaryValuesArray[counter] = secondaryValues;

                    //fill of comparableFlags
                    comparableFlags[counter] = $tableRow.find('.comparable').first().is(':checked');

                    //tableFieldsArray[counter] = tableFields;
                    //secondaryValuesArray[counter] = secondaryValues;
                    counter++;
                }
            });

            //fill of fileDataArray -- fileData
            fileDataArray[0] = $('#hdnFileName').val();
            fileDataArray[1] = $('input:radio[name="hasHeader"]:checked').val();

            $.ajax({
                url: '/MasterChart/ImportData',
                cache: false,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify({ comparableFlags: comparableFlags, fileData: fileDataArray, fields: tableFieldsArray, values: defaultValuesArray, secValues: secondaryValuesArray, columns: fileColumnsArray, sections: sectionsArray }),
                //beforeSend: function (xhr) {
                //    UI.checkForPendingRequests(xhr);
                //},
                success: function (response) {
                    var duration = (response.ResponseType < 0) ? duration = response.ResponseType : null;
                    if (response.ResponseType != 1)
                        $('#divFilesToDownload').show();
                    UI.messageBox(response.ResponseType, response.ResponseMessage + '<br />' + response.ExceptionMessage, duration, response.InnerException);
                }
            });
        });

        $('#btnCheckLeads').on('click', function () {
            $.ajax({
                url: '/Masterchart/ValidateData',
                cache: false,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify({ fileName: $('#hdnFileName').val() }),
                //beforeSend: function (xhr) {
                //    UI.checkForPendingRequests(xhr);
                //},
                success: function (response) {
                    var duration = response.ResponseType < 0 ? duration = response.ResponseType : null;
                    UI.messageBox(response.ResponseType, response.ResponseMessage + '<br />' + response.ExceptionMessage, duration, response.InnerException);
                }
            });
        });
        //End: Import initialization
    }
    var relatedElements =
    {
        "GeneralInformation_Emails": "tblGeneralInformation_Emails",
        "GeneralInformation_Phones": "tblGeneralInformation_Phones"
    };

    function beforeSubmit(frmID, evnt) {
        var validation = UI.validateRelatedElements(frmID);

        //var invalidElements = UI.validateRelatedElements(frmID);

        if (validation.invalidElements.length > 0) {
            var hld = new SETTINGS.highlightElements();
            hld.elements = validation.invalidElements;
            hld.highlightTime = -1;
            UI.highlightElements(hld);
        }

        //Validate the form so UL of the validation summary gets generated
        UI.validateForm(frmID);
        var validationErrorsList = UI.getValidationErrorsList(frmID, validation.errors);

        //Display all the errors
        UI.displayValidationErrors(validationErrorsList);

        if (validation.errors.length > 0) {

            evnt.stopPropagation();
            //var evt = window.event || arguments.callee.caller.arguments[0];
            return false;
        }
    }
    //function removeFromTable(tableID, rowIndex) {
    //    var fieldID = UTILS.getKeyByValue(relatedElements, tableID);
    //    UI.removeFromListTable(tableID, rowIndex)
    //    //UTILS.removeDataRowsFromTable(tableID, rowIndex);
    //    var params = new SETTINGS.getDataFromColumn();
    //    params.tableID = tableID;

    //    UI.validateAgainstRelatedField(fieldID, tableID);

    //}
    function addItemFrom(fieldID) {
        var fieldID = fieldID;
        var targetTableID = "tbl" + fieldID + "s";
        var field = $("#" + fieldID);
        var fieldToValidate = $("#" + fieldID + "s");
        var newValue = field.val();
        var add = new SETTINGS.addToListTable();
        add.fieldID = fieldID;
        add.tableID = targetTableID;
        add.newValue = newValue;
        add.newItemOptions.addDeleteIcon = true;
        add.newItemOptions.deleteIconCallBack = LEAD_GENERAL_INFORMATION.removeFromListTable;

        var added = UI.addToListTable(add);

        if (added.length > 0) {// jquery Object
            var hls = new SETTINGS.highlightElements();
            hls.elements = added;
            UI.highlightElements(hls);
        }
        else if (added == true) {
            var labelText = $('label[for="' + fieldID + '"]').text();
            var params = new SETTINGS.tableDataToJason();
            params.tableID = targetTableID;
            params.subject = labelText;
            params.columnIndexes = [1];
            fieldToValidate.val(UTILS.tableDataToJason(params));
        }

        UI.validateRelatedElement(fieldID + "s", targetTableID);
    }



    function removeFromTable(tableID, rowIndex) {
        UI.removeFromTable(tableID, rowIndex);
        UTILS.updateTableRelatedFieldValue(tableID);
        UI.validateDataTable(tableID);
    }

    var deleteTableRows = function () {
        $('#tblImportedDataFormat tbody tr').not('theader').on('click', function (e) {
            if ($(e.target).hasClass('remove-field')) {
                var id = $(e.target).parents('tr').first().attr('id').substr(7);
                $('#trField' + id).remove();
            }
        });
    }

    var loadTableFields = function () {
        $('.list-section').unbind('change').on('change', function (e) {
            $(e.target).parents('tr').first().find('td:nth-child(6)').empty();
            $(e.target).parents('tr').first().find('td:nth-child(4)').children(':not(.table-field)').remove();
            if ($(e.target).val() != 0) {
                var sysWorkGroup = new Array();
                var tables = $(e.target).val().split(',');
                var values = new Array();

                $.each(tables, function (index, item) {
                    values[index] = item.substr((item.indexOf('-') + 1), (item.length - item.indexOf('-')));
                });
                sysWorkGroup[0] = $('#divAvailableWorkGroups').find('input:radio:checked').val();
                if ($(IMPORT.lastSection).not(values).length == 0 && $(values).not(IMPORT.lastSection).length == 0) {//check if previous data is the same that actual, so prevent ajax call
                    var builder = '';
                    $.each(IMPORT.lastTables, function (index, item) {
                        builder += '<option value="' + item.Value + '">' + item.Text + '</option>';
                    });
                    $(e.target).parents('tr').first().children('td:nth-child(4)').children().empty().append(builder);
                }
                else {
                    IMPORT.lastSection = values;
                    $.ajax({
                        url: '/MasterChart/GetTablesFields',
                        data: { sysWorkGroup: sysWorkGroup, tableName: values },
                        traditional: true,
                        //beforeSend: function (xhr) {
                        //    UI.checkForPendingRequests(xhr);
                        //},
                        success: function (data) {
                            IMPORT.lastTables = data;
                            var builder = '';
                            $.each(data, function (index, item) {
                                builder += '<option value="' + item.Value + '">' + item.Text + '</option>';
                            });
                            $(e.target).parents('tr').first().children('td:nth-child(4)').children().empty().append(builder);
                        }
                    });
                }
            }
            else {
                var builder = '<option>--Select a Section--</option>';
                $(e.target).parents('tr').first().children('td:nth-child(4)').children().empty().append(builder);
                $(e.target).parents('tr').first().children('td:nth-child(4)').children('.table-field').trigger('change');
            }
        });
    }

    var createDefaultValueFields = function () {
        $('.table-field').unbind('change').on('change', function (e) {
            $(e.target).parents('tr').first().find('td:nth-child(6)').empty();
            $(e.target).parents('tr').first().find('td:nth-child(4)').children(':not(.table-field)').remove();
            if ($(e.target).val() != undefined) {
                var tableData = $(e.target).val().substr($(e.target).val().indexOf('-') + 1, $(e.target).val().length).split(',');
                $.each(tableData, function (index, item) {
                    var value = item.substr(0, item.indexOf('_'));    //phone; phoneTypeID
                    var type = item.substr(item.indexOf('_') + 1, item.length);  //int
                    var builder = '';
                    switch (type) {
                        case 'int': {
                            if (value.indexOf('ID') > 0) {
                                builder += '<select></select>';
                                $.getJSON('/MasterChart/GetDDLData', { itemType: value }, function (data) {
                                    if ($(e.target).val().indexOf(',') > 0) {
                                        if ($(e.target).find('option:selected').text() == 'Interaction Type' && value != 'interactionTypeID') {
                                            var tempBuilder = '<label>Comments Row </label><select class="comments-select"><option val="">None</option>';
                                            counter = 0;
                                            while (counter < $('#tblImportedDataFormat tbody tr').not('thead').length - 1) {
                                                counter++;
                                                tempBuilder += '<option val="' + counter + '">' + counter + '</option>';
                                            }
                                            tempBuilder += '</select>';
                                            $(tempBuilder).insertAfter($(e.target));
                                        }
                                        if (value != 'relatedColID') {
                                            $(e.target).parents('tr').first().find('td:nth-child(4)').append(builder);
                                            $(e.target).parents('tr').first().find('td:nth-child(4)').children('select:last').fillSelect(data);
                                        }
                                        $(e.target).parents('tr').first().find('td:nth-child(4)').children('select:not(.table-field)').addClass('secondary-value');
                                    }
                                    else {
                                        $(e.target).parents('tr').first().find('td:nth-child(6)').append(builder);
                                        $(e.target).parents('tr').first().find('td:nth-child(6)').children().fillSelect(data);
                                    }
                                });
                            }
                            else {
                                builder += '<input type="text" placeholder="number" ';
                                if (value == 'phone')
                                    builder += 'style="visibility:hidden"/>';
                                else
                                    builder += '/>';
                                $(e.target).parents('tr').first().find('td:nth-child(6)').append(builder);
                                if (value == 'phone')
                                    $(e.target).parents('tr').first().find('td:nth-child(6)').children('input:text').val('--');
                                $(e.target).parents('tr').first().find('td:nth-child(6)').children().numeric();
                            }
                            break;
                        }
                        case 'bit': {
                            var tag;
                            switch (item) {
                                case 'main_bit': {
                                    tag = 'Is Main';
                                    break;
                                }
                                case 'doNotCall_bit': {
                                    tag = 'Do Not Call';
                                    break;
                                }
                                default: {
                                    tag = '';
                                    //tag = $(e.target).find('option:selected').text();
                                    break;
                                }
                            }
                            //builder += '<label>' + tag + '</label><input type="checkbox"';
                            if ($(e.target).val().indexOf(',') > 0) {
                                //if ($(e.target).find('option:selected').text() != 'Interaction Type' && $(e.target).find('option:selected').text() != 'Interaction Comments')
                                builder += '<label>' + tag + '</label><input type="checkbox" class="secondary-value"/>';
                                $(e.target).parents('tr').first().find('td:nth-child(4)').append(builder);
                            }
                            else {
                                builder += '<input type="checkbox"/>';
                                $(e.target).parents('tr').first().find('td:nth-child(6)').append(builder);
                            }
                            break;
                        }
                        case 'date': {
                            builder += '<input type="text" placeholder="date" />';
                            $(e.target).parents('tr').first().find('td:nth-child(6)').append(builder);
                            $(e.target).parents('tr').first().find('td:nth-child(6)').children().datepicker({
                                dateFormat: 'yy-mm-dd',
                                changeMonth: true,
                                changeYear: true
                            });
                            break;
                        }
                        case 'time': {
                            builder += '<input type="text" placeholder="time" />';
                            $(e.target).parents('tr').first().find('td:nth-child(6)').append(builder);
                            $(e.target).parents('tr').first().find('td:nth-child(6)').children().datetimepicker({
                                timeFormat: 'HH:mm:ss',
                                timeOnly: true,
                                stepMinute: 5
                            });
                            break;
                        }
                        case 'string': {
                            builder += '<input type="text" placeholder="text" ';
                            if (value == 'email')
                                builder += 'style="visibility:hidden"/>';
                            else
                                builder += '/>';
                            $(e.target).parents('tr').first().find('td:nth-child(6)').append(builder);
                            if (value == 'email')
                                $(e.target).parents('tr').first().find('td:nth-child(6)').children('input:text').val('--');
                            //builder += '<input type="text" placeholder="text"/>';
                            //$(e.target).parents('tr').first().find('td:nth-child(6)').append(builder);
                            break;
                        }
                        case 'Guid': {
                            builder += '<select></select>';
                            value = value == 'assignedToUserID' ? 'assignedToUserMail' : value;
                            $.getJSON('/MasterChart/GetDDLData', { itemType: value }, function (data) {
                                //builder += '<input type="text" placeholder="text"/>';
                                $(e.target).parents('tr').first().find('td:nth-child(6)').append(builder);
                                $(e.target).parents('tr').first().find('td:nth-child(6)').children().fillSelect(data);
                            });
                            break;
                        }
                    }
                });
            }
        });
    }



    return {
        init: init,
        deleteTableRows: deleteTableRows,
        loadTableFields: loadTableFields,
        createDefaultValueFields: createDefaultValueFields
    }
}();
var SEARCH = function () {

    var oTable;

    var _requestSent = false;

    var init = function () {
        //$('#Search_ArrivalDateStart').datepicker({
        //    'changeMonth': true,
        //    'changeYear': true,
        //    'dateFormat': 'yy-mm-dd'
        //});

        //$('#Search_ArrivalDateEnd').datepicker({
        //    'changeMonth': true,
        //    'changeYear': true,
        //    'dateFormat': 'yy-mm-dd'
        //});
        $('select[multiple="multiple"]').multiselect({
            noneSelectedText: "--Select One--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();

        //$('.numeric-field').each(function () {
        //    $(this).numeric();
        //});

        //$(".ui-multiselect-checkboxes").selectable({
        //    selecting: function () {
        //        $(".ui-selecting", this).each(function () {
        //            $(this).addClass('ui-state-hover-multiselect');
        //        });
        //    },
        //    filter: 'label',
        //    stop: function () {
        //        $(".ui-selected input", this).each(function () {
        //            //this.checked = !this.checked;
        //            $(this).click();
        //        });
        //        $(".ui-state-hover-multiselect", this).each(function () {
        //            $(this).removeClass('ui-state-hover-multiselect');
        //            $(this).removeClass('ui-selected');
        //        });
        //    },
        //    unselecting: function () {

        //        //hover ! selecting
        //        $(".ui-state-hover-multiselect:not(.ui-selecting)", this).each(function () {
        //            $(this).removeClass('ui-state-hover-multiselect');
        //        });
        //    }
        //});

        ///$("#fdsLeadSearch select").multiselect("widget").selectable();
        //$("#fdsLeadSearch select ui-multiselect-checkboxes").selectable();

        $('#SearchCouponByDate_Provider').on('change', function (e, params) {
            //definir de donde leerá la terminal
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'servicesPerTerminal', itemID: ($(this).val() != '' && $(this).val() != null ? $(this).val() + '|' + 'null' : 'null') }, function (data) {
                $('#SearchCouponByDate_Service').fillSelect(data);
                $('#SearchCouponByDate_Service').multiselect('refresh');
            });
        });

        $('#btnSearchCoupons').on('click', function () {
            UI.collapseFieldset('fdsLeadSearch');
            UI.collapseFieldset('fdsLeadSearchResults');
            $('#tblSearchCouponsResults tbody').empty();
            $('#tblSearchResult_Leads tbody').empty();
            LEAD_GENERAL_INFORMATION.resetAll();
            if (SEARCH.oTable != undefined) {
                SEARCH.oTable.$('tr.selected-row').removeClass('selected-row primary');
            }
            if (localStorage.Eplat_RecentCoupons != undefined && localStorage.Eplat_RecentCoupons != '[]' && $('#SearchCoupon_Coupon').val() == '') {
                var json = $.parseJSON(localStorage.Eplat_RecentCoupons);
                var newJson = new Array();
                var found = false;
                var _date = new Date();
                _date.setDate(_date.getDate() - 5);
                if (json[0].PointOfSale != undefined && $('#hdnIsPoSOnline').val() == 'false') {
                    $.each(json, function (i, item) {
                        if (item.DateLastUpdate > COMMON.getDate(_date)) {
                            newJson.push({
                                "PointOfSale": item.PointOfSale,
                                "DateSaved": item.DateSaved,
                                "Coupons": item.Coupons,
                                "DateLastUpdate": item.DateLastUpdate
                            });
                        }
                        if (item.PointOfSale == $('#SearchCoupon_PointOfSale option:selected').val() && item.DateSaved == $('#SearchCoupon_PurchaseDate').val() && item.DateLastUpdate == COMMON.getDate()) {
                            if (item.Coupons.length > 1) {
                                found = true;
                                ///new 
                                var length = item.Coupons.length;
                                var str = JSON.stringify(item.Coupons).split('RecentCoupon_Location').length;
                                if (length != (str - 1)) {
                                    found = false;
                                }
                                ///
                            }
                        }
                    });
                    localStorage.Eplat_RecentCoupons = JSON.stringify(newJson);
                    if (!found) {
                        $('#frmSearchCoupons').submit();
                    }
                    else {
                        PURCHASE.loadCouponsTableFromJson();
                    }
                }
                else {
                    $('#frmSearchCoupons').submit();
                }
            }
            else {
                $('#frmSearchCoupons').submit();
            }
        });

        //$('#btnSearchCouponsByDate').on('click', function () {
        //    $('#frmSearchCoupons').submit();
        //});

        $('#btnClearLocal').on('click', function () {
            localStorage.Eplat_RecentCoupons = '[]';
            UI.messageBox(1, 'Cache has been cleared', null, null);
        });
    }

    var searchResultsTable = function (data) {


        //var dataTable = $('#tblSearchResult_Leads').dataTable();

        $('#tblSearchResult_Leads').dataTable({
            "bRetrieve": true,
            "bDestroy": true,
            //"bFilter": false,
            "bFilter": true,
            "bProcessing": true,
            "asStripeClasses": ['odd', 'striped'],
            "bAutoWidth": false,
            "aoColumnDefs": [
                { "bSortable": false, "aTargets": [0] }
            ],
            "aoRowCallback": [UI.tablesHoverEffect()],
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            },
            "aaSorting": [] // ver si esta linea se deshabilita el sorting inicial
            //, "sDom": '<"top"i>rt<"bottom"flp><"clear">'
        });
        SEARCH.oTable = $('#tblSearchResult_Leads').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //    var event = $.Event('keydown');
        //    event.keyCode = 27;
        //    $(document).trigger(event);
        //}
        //});
    }

    var bindFunctionsToTable = function () {

        $('#txtSelect').numeric();

        $('#lblAll').on('mouseover', function () {
            $(this).addClass('selectAll-decoration');
        });

        $('#lblAll').on('mouseout', function () {
            $(this).removeClass('selectAll-decoration');
        });

        $('#lblAll').on('click', function () {
            SEARCH.selectLeads();
        });

        $('#txtSelect').on('keydown', function (e) {
            if (e.keyCode === 13) {
                SEARCH.selectLeads($(this).val());
                $(this).blur();
            }
        });
    }

    var selectLeads = function (quantity) {
        $('#flagAll').val(false);

        if (quantity != undefined) {
            SEARCH.oTable.$('tr').each(function (index, item) {
                $(this)[0].cells[0].childNodes[1].checked = false;
            });
            SEARCH.oTable.$('tr').each(function (index, item) {
                if (index < quantity) {
                    $(this)[0].cells[0].childNodes[1].checked = true;
                }
                else
                    return false;
            });
        }
        else {
            $('#flagAll').val(true);
            SEARCH.oTable.$('tr').each(function (e) {
                $(this)[0].cells[0].childNodes[1].checked = true;
            });
        }
    }

    return {
        init: init,
        selectLeads: selectLeads,
        searchResultsTable: searchResultsTable,
        bindFunctionsToTable: bindFunctionsToTable
    }
}();
var LEAD_MASS_UPDATE = function () {
    var init = function () {

        $('.lead-selection').on('click', function (e) {
            $('#flagAll').val(false);
        });

        $('#MassUpdate_Terminal').unbind('change').on('change', function () {
            if ($(this).val() == '0') {
                $('#MassUpdate_AssignedToUser').clearSelect();
                $('#MassUpdate_AssignedToUser').append('<option value="">--Select Terminal--</option>');
            }
            else {
                if ($('#MassUpdate_AssignedToUser option').length == 1) {
                    $.getJSON('/crm/MasterChart/GetDDLData', { itemType: 'users', itemID: $(this).val() }, function (data) {
                        $('#MassUpdate_AssignedToUser').fillSelect(data);
                    });
                }
            }
        });

        $('#MassUpdate_Terminal').trigger('change');

        $('#btnMassUpdate').unbind('click').on('click', function () {
            var leads = '';
            if ($('#flagAll').val() == 'true') {
                var tempLeads = $('#Coincidences').val().split(',').join("','")
                leads = "'" + tempLeads;
                leads += "'";
            }
            else {
                //oTable = $('#tblSearchResult_Leads').dataTable();
                leads += "'";
                SEARCH.oTable.$('tr').each(function (index, item) {
                    if ($(this)[0].cells[0].childNodes[1].checked == true) {
                        leads += $(this).attr('id') + "','";
                    }
                });
                leads = leads.substr(0, leads.length - 2);
            }
            //$('#MassUpdate_Coincidences').val(leads);
            $('.mass-update-coincidences').val(leads);
            if (leads != '')
                $('#frmMassUpdate').submit();
            else
                UI.messageBox(0, "No Leads Selected", null, null);
        });

        $('#btnMassInsert').unbind('click').on('click', function () {
            var leads = '';
            if ($('#flagAll').val() == 'true') {
                var tempLeads = $('#Coincidences').val().split(',').join("','")
                leads = "'" + tempLeads;
                leads += "'";
            }
            else {
                leads += "'";
                SEARCH.oTable.$('tr').each(function (index, item) {
                    if ($(this)[0].cells[0].childNodes[1].checked == true) {
                        leads += $(this).attr('id') + "','";
                    }
                });
                leads = leads.substr(0, leads.length - 2);
            }
            //$('#MassUpdate_Coincidences').val(leads);
            $('.mass-update-coincidences').val(leads);
            if (leads != '')
                $('#frmMassInsert').submit();
            else
                UI.messageBox(0, "No Leads Selected", null, null);
        });

        $('.mass-sending').unbind('click').on('click', function (e) {
            var leads = '';
            if ($('#flagAll').val() == 'true') {
                var tempLeads = $('#Coincidences').val().split(',').join("','");
                leads = tempLeads;
                leads += "'";
            }
            else {
                leads += "'";
                SEARCH.oTable.$('tr').each(function (index, item) {
                    if ($(this)[0].cells[0].childNodes[1].checked == true) {
                        leads += $(this).attr('id') + "','";
                    }
                });
                leads = leads.substr(0, leads.length - 2);
            }
            $('.mass-update-coincidences').val(leads);
            //var _event = $(e.target).attr('value').toLowerCase().indexOf('member') > 0 ? 13 : $(e.target).attr('value').toLowerCase().indexOf('guest') > 0 ? 14 : 15;
            var _event = $(e.target).attr('data-sysevent');
            $('#MassUpdate_SendingEvent').val(_event);
            if (leads != '') {
                $('#frmMassSending').submit();
            }
            else {
                UI.messageBox(0, "No Leads Selected", null, null);
            }
        });
        //$('#btnDuplicateLeads').unbind('click').on('click', function () {
        //    var leads = '';
        //    if ($('#flagAll').val() == 'true') {
        //        var tempLeads = $('Coincidences').val().split(',').join("','");
        //        leads = "'" + tempLeads;
        //        leads += "'";
        //    }
        //    else {
        //        leads += "'";
        //        SEARCH.oTable.$('tr').each(function (index, item) {
        //            if ($(this)[0].cells[0].childNodes[1].checked == true) {
        //                leads += $(this).attr('id') + "','";
        //            }
        //        });
        //        leads = leads.substr(0, leads.length - 2);
        //    }
        //    $('.mass-update-coincidences').val(leads);
        //    if (leads != '') {
        //        $('#frmDuplicateLeads').submit();
        //    }
        //    else {
        //        UI.messageBox(0, 'No Leads Selected', null, null);
        //    }
        //});
    }

    var massUpdateSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {

        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var massInsertSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {

        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var massSendingSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {

        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }
    //var duplicateLeadsSuccess = function (data) {
    //    var duration = data.ResponseType < 0 ? data.ResponseType : null;
    //    if (data.ResponseType > 0) {

    //    }
    //    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    //}

    return {
        init: init,
        massUpdateSuccess: massUpdateSuccess,
        massInsertSuccess: massInsertSuccess,
        massSendingSuccess: massSendingSuccess
        //duplicateLeadsSuccess: duplicateLeadsSuccess
    }
}();
var LEAD_GENERAL_INFORMATION = function () {

    var assignedToUser;

    var saveContinueFlag = false;

    var addReferral = function (leadID) {
        var url = '/CRM/masterchart/AddReferral/?referredByID=' + leadID;
        COMMON.openWindow(url, REQUEST_HANDLERS.searchReferrals);
    }

    var editReferral = function (tableID, rowIndex, leadID) {
        var url = '/CRM/masterchart/?referralID=' + leadID + '&tableID=' + tableID;

        //make sure we close the aleady opened window, otherwise the unload event from the child window will unselect the new selected row.
        var childWindow = window.open('', 'Masterchart');
        if (childWindow != null || !childWindow.closed) {
            childWindow.close();
            COMMON.executeDelayed(function () {
                UI.setSelectedRow(tableID, rowIndex);
                COMMON.openWindow(url, REQUEST_HANDLERS.searchReferrals);
            }, null, 500);
        }
        else {
            UI.setSelectedRow(tableID, rowIndex);
            COMMON.openWindow(url, REQUEST_HANDLERS.searchReferrals);
        }
    }

    var viewLead = function (tableID, rowIndex, leadID) {
        var url = '/CRM/masterchart/?leadID=' + leadID + '&tableID=' + tableID;

        //make sure we close the already opened window, otherwise the unload event from the child window will unselect the new selected row.
        var childWindow = window.open('', 'Masterchart');
        if (childWindow != null || !childWindow.closed) {
            childWindow.close();
            COMMON.executeDelayed(function () {
                UI.setSelectedRow(tableID, rowIndex);
                COMMON.openWindow(url, REQUEST_HANDLERS.searchReferrals);
            }, null, 500);
        }
        else {
            UI.setSelectedRow(tableID, rowIndex);
            COMMON.openWindow(url, REQUEST_HANDLERS.searchReferrals);
        }
    }

    var cancelEditReferral = function (tableID) {
        UI.deselectAllSelectedRows(tableID);
        //UI.deselectAllSelectedRows('tblGeneralInformation_Referrals');
    }

    var newLead = function () {

        init();

        $(document).find('.selected-row').each(function () {
            var event = $.Event('keydown');
            event.keyCode = 27;
            $(document).trigger(event);
        });

        //if there is a referredByID in the URL populate it again after form reset.
        if (UTILS.isSetURLParameter('referralID')) {
            $("#GeneralInformation_ReferredByID").val(UTILS.getURLParameter('referralID'));
        }

        //UI.collapseFieldset('fdsLeadSearch');
        //UI.collapseFieldset("fdsLeadSearchResults");

        $('#fdsLeadInformation').show();
        $("#frmReservationGeneralInformation").hide();
        $('#fdsLeadGeneralInformation_CurrentReferrals').hide();
        $('#fdsPurchasesManagement').hide();
        //$('#fdsMassUpdate').hide();

        COMMON.collapseAndExpand(
            '#fdsLeadSearch, #fdsLeadSearchResults, #fdsLeadInformation #fdsLeadGeneralInformation ~ fieldset, #fdsAccountancyManagement',
            '#fdsLeadInformation, #fdsLeadGeneralInformation, #fdsLeadGeneralInformation_ContactInformation, #fdsLeadGeneralInformation_ContactInformation fieldset')
            .done(
                COMMON.focusFirstEditableElement('fdsLeadGeneralInformation')
                //UI.scrollTo('fdsLeadGeneralInformation', null)
            );

        COMMON.hideRelatedForms('frmLeadGeneralInformation');

        //$('#fdsLeadGeneralInformation [data-cascading-group]').each(function () {
        //    UTILS.cascadeDropDowns(this.id);
        //});
    }
    var init = function () {

        resetAll();

        $('#GeneralInformation_Emails_Email').on('focusout', function () {
            if ($(this).val() != '' && $('#tblGeneralInformation_Emails tbody tr').length == 0) {
                $('#GeneralInformation_Emails_Main').attr('checked', 'checked');
            }
        });

        $('#GeneralInformation_PhoneNumbers_PhoneNumber').on('focusout', function () {
            if ($(this).val() != '' && $('#tblGeneralInformation_Phones tbody tr').length == 0) {
                $('#GeneralInformation_PhoneNumbers_Main').attr('checked', 'checked');
            }
        });

        //frmLeadGeneralInformation
        $("#frmLeadGeneralInformation").validate().settings.ignore = "";
        var actionButtons = OPTIONS.dataTable.actionButtons;
        //Begin: add events to data table action buttons 
        $('[data-table-button-type]').each(
            function () {
                //if ($(this).data('avoid-unbind') == undefined && $(this).attr('data-avoid-unbind') == undefined) {
                $(this).unbind("click");
                //}

                var buttonType = $(this).data('table-button-type');
                switch (buttonType) {
                    case actionButtons.add:
                        $(this).on("click", function () {
                            UI.addDataRow($(this));
                            if ($(this).data('callback') != undefined) {
                                var _callback = $(this).data('callback');
                                var callback = eval(_callback);
                                if (typeof callback == 'function') {
                                    callback();
                                }
                            }
                        });
                        break;
                    case actionButtons.update:
                        $(this).on("click", function () {
                            UI.updateDataRow($(this));
                            if ($(this).data('callback') != undefined) {
                                var _callback = $(this).data('callback');
                                var callback = eval(_callback);
                                if (typeof callback == 'function') {
                                    callback();
                                }
                            }
                        });
                        break;
                    case actionButtons.cancel:
                        $(this).on("click", function () { UI.cancelDataRowEditing($(this)); });
                        break;
                    default:

                }

            }
        );
        //End: add action to data table buttons

        $('[data-cascading-group]').each(function () {
            $(this).unbind('change');
            $(this).on("change", function () { UTILS.cascadeDropDowns(this.id); });
            UTILS.cascadeDropDowns(this.id);
        });

        RESERVATION_GENERAL_INFORMATION.resetAll();

        $('#btnSaveContinueLead').unbind('click').on('click', function () {
            LEAD_GENERAL_INFORMATION.saveContinueFlag = true;
            $('#frmLeadGeneralInformation').submit();
            //var leadID = $("#GeneralInformation_LeadID").val();
            //console.log($('#tblSearchResult_Leads tbody').find('#' + leadID).next());
            //$('#tblSearchResult_Leads tbody').find('#'+ leadID).next().click();
        });

        $('#btnDuplicateLead').unbind('click').on('click', function () {
            LEAD_GENERAL_INFORMATION.assignedToUser = $('#GeneralInformation_AssignedToUserID').val();
            $('#GeneralInformation_DuplicateLead').val(true);
            $.fancybox({
                type: 'html',
                href: $('#divAssignedToUser'),
                modal: true,
                afterShow: function () {
                    $('#btnAcceptDuplicate').unbind('click').on('click', function () {
                        $.fancybox.close();
                        $('#messageBox').css('z-index', '3');
                        $('#messageBoxClose').click();
                        setTimeout(function () {
                            $('#frmLeadGeneralInformation').submit();
                        }, 1000);
                    });

                    $('#btnCancelDuplicate').unbind('click').on('click', function () {
                        $.fancybox.close();
                        $('#messageBox').css('z-index', '3');
                        $('#messageBoxClose').click();
                        $('#GeneralInformation_DuplicateLead').val('false');
                        $('#GeneralInformation_AssignedToUserID').val(LEAD_GENERAL_INFORMATION.assignedToUser);
                    });
                }
            });
            //$('#frmLeadGeneralInformation').submit();
        });

        $('#GeneralInformation_LastName').on('blur', function () {
            if ($('#GeneralInformation_FirstName').val() == '') {
                $('#GeneralInformation_FirstName').val(' ');
            }
        });

        $('#btnEmailNotSet').unbind('click').on('click', function () {
            $('#GeneralInformation_Emails_Email').val('notgiven@notgiven.com');
            $('#GeneralInformation_Emails_Main').attr('checked', true);
            $('#btnAddEmail').trigger('click');
            if ($('#tblGeneralInformation_Phones tbody tr').length > 0) {
                UI.collapseFieldset('fdsLeadGeneralInformation_ContactInformation');
            }
            else {
                UI.collapseFieldset('fdsLeadGeneralInformation_ContactInformationEmails');
            }
        });

        $('#btnPhoneNotSet').unbind('click').on('click', function () {
            $('#GeneralInformation_PhoneNumbers_PhoneTypeID option[value="4"]').attr('selected', true);
            $('#GeneralInformation_PhoneNumbers_PhoneNumber').val('1111111111');
            $('#GeneralInformation_PhoneNumbers_Main').attr('checked', true);
            $('#GeneralInformation_PhoneNumbers_DoNotCall').attr('checked', true);
            $('#btnAddPhone').trigger('click');
            if ($('#tblGeneralInformation_Emails tbody tr').length > 0) {
                UI.collapseFieldset('fdsLeadGeneralInformation_ContactInformation');
            }
            else {
                UI.collapseFieldset('fdsLeadGeneralInformation_ContactInformationPhoneNumbers');
            }
        });
    }

    function resetAll() {
        try {

            //clear all the form fields
            $("#frmLeadGeneralInformation").clearForm();


            UTILS.clearTableRows('tblGeneralInformation_ReferredBy');
            UTILS.clearTableRows('tblGeneralInformation_Referrals');
            UTILS.clearTableRows('tblPurchases');
            UTILS.clearTableRows('tblPurchasesServices');
            UTILS.clearTableRows('tblPurchasesPayments');
            $('#PurchaseInfo_PurchaseStatus').val(1);
            UI.collapseFieldset('fdsPurchasesManagement');
            //deselect the selected row of leads List
            //UI.deselectAllSelectedRows('tblSearchResult_Leads');

            //hide all data table action buttons;
            UI.hideAllDataTablesActionButtons();
            //Show all data table add buttons
            UI.showAllDataTablesAddButtons();
            // reset all selected-row-indexes

            // la siguiente linea ya no vige, por que ya no estamos guardando el selected index.
            $('[data-selected-row-index]').each(function () { $(this).data('selected-row-index', null) });

            /// make sure default values are set
            $("GeneralInformation_InputMethodID").val(1);
            $("GeneralInformation_LeadStatusID").val(1);
            $('#GeneralInformation_Interactions_BookingStatusID').val(2);
            // UI.expandFieldset('fdsLeadSearch');


            //UI.collapseFieldset('fdsLeadInformation');
            ////expand same fds ( fdsLeadInformation ) and its nested fdsLeadGeneralInformation
            //UI.expandFieldset('fdsLeadInformation');
            //UI.expandFieldset('fdsLeadGeneralInformation');
        }
        catch (e) { }
    }
    var relatedElements =
    {
        "GeneralInformation_Emails": "tblGeneralInformation_Emails",
        "GeneralInformation_Phones": "tblGeneralInformation_Phones"
    };

    //function beforeSubmit(frmID,evnt) {
    //    var validation = UI.validateRelatedElements(frmID);

    //    //var invalidElements = UI.validateRelatedElements(frmID);

    //    if (validation.invalidElements.length > 0) {
    //        var hld = new SETTINGS.highlightElements();
    //        hld.elements = validation.invalidElements;
    //        hld.highlightTime = -1;
    //        UI.highlightElements(hld);
    //    }

    //    //Validate the form so UL of the validation summary gets generated
    //    UI.validateForm(frmID);
    //    var validationErrorsList = UI.getValidationErrorsList(frmID, validation.errors);

    //    //Display all the errors
    //    UI.displayValidationErrors(validationErrorsList);

    //    if (validation.invalidElements.length > 0) {
    //        evnt.preventDefault();
    //        evnt.stopImmediatePropagation();
    //        return false;
    //    }
    //}
    //function removeFromTable(tableID, rowIndex) {
    //    var fieldID = UTILS.getKeyByValue(relatedElements, tableID);
    //    UI.removeFromListTable(tableID, rowIndex)
    //    //UTILS.removeDataRowsFromTable(tableID, rowIndex);
    //    var params = new SETTINGS.getDataFromColumn();
    //    params.tableID = tableID;

    //    UI.validateAgainstRelatedField(fieldID, tableID);

    //}
    function addItemFrom(fieldID) {
        var fieldID = fieldID;
        var targetTableID = "tbl" + fieldID + "s";
        var field = $("#" + fieldID);
        var fieldToValidate = $("#" + fieldID + "s");
        var newValue = field.val();
        var add = new SETTINGS.addToListTable();
        add.fieldID = fieldID;
        add.tableID = targetTableID;
        add.newValue = newValue;
        add.newItemOptions.addDeleteIcon = true;
        add.newItemOptions.deleteIconCallBack = LEAD_GENERAL_INFORMATION.removeFromListTable;

        var added = UI.addToListTable(add);

        if (added.length > 0) {// jquery Object
            var hls = new SETTINGS.highlightElements();
            hls.elements = added;
            UI.highlightElements(hls);
        }
        else if (added == true) {
            var labelText = $('label[for="' + fieldID + '"]').text();
            var params = new SETTINGS.tableDataToJason();
            params.tableID = targetTableID;
            params.subject = labelText;
            params.columnIndexes = [1];
            fieldToValidate.val(UTILS.tableDataToJason(params));
        }

        UI.validateRelatedElement(fieldID + "s", targetTableID);
    }

    function showRelatedForms(formID, leadID) {
        // buscar los formularios que tengan el data-lead-related-form
        $('form[data-related-form="' + formID + '"]').each(function () {
            var functionName = $(this).data("on-show");
            var onFormShowFunction = eval(functionName);
            onFormShowFunction.call(
                undefined,
                leadID
            );

            var relatedForm = $(this);
            if (!relatedForm.is(':visible')) {
                relatedForm.show();
            }

            var formItemsName = $(this).data("items-name");
            var cr = COMMON.collapseAndExpand('#fds' + formItemsName + ' #fdsCurrent' + formItemsName + ' ~ fieldset', '#fds' + formItemsName + ', #fdsCurrent' + formItemsName + '');

        });

    }

    function hideRelatedForms() {
        $("form[data-related-form]").each(function () {
            $(this).clearForm();
            $(this).hide();
        });
    }

    //function addDataToTable(tableID) {
    //    var rowadded = UI.addDataRowFromUiToTable(tableID);
    //    if (rowadded) {            
    //       UTILS.updateTableRelatedFieldValue(tableID);
    //       UI.validateDataTable(tableID);
    //       UI.clearRelatedSourceFields(tableID);
    //    }
    //}
    //returns

    //beforeSubmit: beforeSubmit,
    return {
        init: init,
        relatedElements: relatedElements,
        addItemFrom: addItemFrom,
        resetAll: resetAll,
        newLead: newLead,
        addReferral: addReferral,
        editReferral: editReferral,
        cancelEditReferral: cancelEditReferral,
        viewLead: viewLead

    }
}();
var RESERVATION_GENERAL_INFORMATION = function () {

    function resetAll() {
        try {
            //clear all the form fields

            //retrieve the lead ID so we can keep it after clearForm.
            var leadID = $("#GeneralInformation_LeadID").val();
            $("#frmReservationGeneralInformation").clearForm();
            //reservation LeadID
            $("#LeadID").val(leadID);
            //presentation LeadID
            $("#GeneralInformation_PresentationInformation_LeadID").val(leadID);
            //GeneralInformation_PresentationInformation_LeadID
            //BillingInfo
            //$("#BillingInfo_LeadID").val(leadID);
            //deselect the selected row of leads List
            UI.deselectAllSelectedRows('tblGeneralInformation_Reservations');

            //hide all data table action buttons;
            UI.hideAllDataTablesActionButtons();
            //Show all data table add buttons
            UI.showAllDataTablesAddButtons();
            //COMMON.collapseAndExpand('#fdsReservations #fdsHotelReservationsDetails ~ fieldset',
            //            '#fdsReservations, #fdsCurrentReservations, #fdsHotelReservationsDetails');
        }
        catch (e) { }
    }

    var newReservation = function () {
        resetAll();
        COMMON.collapseAndExpand('#fdsReservations #fdsHotelReservationsDetails ~ fieldset', '#fdsReservations, #fdsCurrentReservations, #fdsHotelReservationsDetails')
            .done(
                COMMON.executeDelayed(UI.scrollTo, "fdsHotelReservationsDetails"),
                COMMON.focusFirstEditableElement('fdsHotelReservationsDetails')
            );
    };

    var bindFunctionality = function () {
        UI.legendClickBind();
        UI.adjustLegends();
        //$("[data-uses-date-picker]").each(function () {
        //    $(this).datepicker({
        //        'changeMonth': true,
        //        'changeYear': true,
        //        'dateFormat': 'yy-mm-dd'
        //    });

        //});


        $('*[data-uses-date-picker="true"]').each(function () {
            $(this).datepicker({
                dateFormat: 'yy-mm-dd',
                changeMonth: true,
                changeYear: true,
                onClose: function (dateText, inst) {
                    if ($(this).attr('id').indexOf('_I_') > 0) {
                        if ($(this).next().val() != '') {
                            if (dateText != '') {
                                var fromDate = $(this).datepicker('getDate');
                                var toDate = $(this).next().datepicker('getDate');
                                if (fromDate > toDate) {
                                    $(this).next().datepicker('setDate', fromDate);
                                }
                            }
                            else {
                                $(this).next().val(dateText);
                            }
                        }
                        else {
                            $(this).next().val(dateText);
                        }
                    }
                    else if ($(this).attr('id').indexOf('_F_') > 0) {
                        if ($(this).prev().val() != '') {
                            var fromDate = $(this).prev().datepicker('getDate');
                            var toDate = $(this).datepicker('getDate');
                            if (fromDate > toDate) {
                                $(this).prev().datepicker('setDate', toDate);
                            }
                        }
                        else {
                            $(this).prev().val(dateText);
                        }
                    }
                },
                onSelect: function (selectedDate) {
                    if ($(this).attr('id').indexOf('_I_') > 0) {
                        if ($(this).next().val() != '') {
                            $(this).next().datepicker('setDate', $(this).next().datepicker('getDate'));
                        }
                        $(this).next().datepicker('option', 'minDate', $(this).datepicker('getDate'));
                    }
                    else if ($(this).attr('id').indexOf('_F_') > 0) {
                        if ($(this).prev().val() != '') {
                            $(this).prev().datepicker('setDate', $(this).prev().datepicker('getDate'));
                        }
                        //$(this).prev().datepicker('option', 'maxDate', $(this).datepicker('getDate'));
                    }
                }
            });

        });

        $('[data-cascading-group]').each(function () {
            $(this).unbind('change');
            $(this).on("change", function () {
                UTILS.cascadeDropDowns(this.id);
            });
        });

        //$('[data-cascading-group]').each(function () {
        //    $(this).find('option').each(function () { $(this).unbind('click') });

        //    $(this).find('option').each(function () { $(this).on("click", function () {
        //        var parentID = $(this).parent("select").attr("id");
        //        UTILS.cascadeDropDowns(parentID);
        //    });});
        //});

        $("[data-uses-time-picker]").each(function () {
            $(this).datetimepicker({
                timeFormat: 'HH:mm',
                timeOnly: true,
                stepMinute: 5
            });
        });

        $('*[data-uses-datetime-picker]').each(function () {
            $(this).datetimepicker({
                dateFormat: 'yy-mm-dd',
                timeFormat: 'HH:mm',
                timeOnly: false,
                stepMinute: 5
            });
        });
    };
    var init = function () {
        resetAll();
        try {
            $("#frmReservationGeneralInformation").validate().settings.ignore = "";
        } catch (ex) { }

        bindFunctionality();

        $('#GeneralInformation_FlightInformation_PassengersNames').on('keydown', function (e) {
            if ($(this).val().indexOf(',') > -1) {
                $('#GeneralInformation_FlightInformation_Passengers').val($(this).val().split(',').length);
            }
            else {
                if ($(this).val().length > 0) {
                    $('#GeneralInformation_FlightInformation_Passengers').val('1');
                }
            }
        });

        $('#GeneralInformation_FlightInformation_PassengersNames').on('blur', function () {
            if ($('#GeneralInformation_FlightInformation_DestinationID').is(':visible')) {
                $('#GeneralInformation_FlightInformation_DestinationID option[value="' + $('#DestinationID').val() + '"]').attr('selected', true);
            }
            else {
                $('#GeneralInformation_FlightInformation_DestinationID').val($('#DestinationID').val());
            }
        });

        $('#DestinationID').on('change', function () {
            if ($('#GeneralInformation_FlightInformation_DestinationID').is(':visible')) {
                $('#GeneralInformation_FlightInformation_DestinationID option[value="' + $(this).val() + '"]').attr('selected', true);
            }
            else {
                $('#GeneralInformation_FlightInformation_DestinationID').val($(this).val());
            }
        });

        $('#btnAddOptionSold, #btnUpdateOptionSold').on('click', function () {
            var _subtotal = 0;
            $('#tblOptionsSold tbody tr:not(:hidden)').each(function (i, item) {
                _subtotal += parseFloat($(this).children('td:nth-child(9)').text().trim());
            });
            $('#TotalPaid').val(_subtotal);
            $('span[data-read-only-text-for="TotalPaid"]').text(_subtotal);//if non-editable
        });

        $('.send-confirmation').on('click', function () {
            //var _eventID = $(this).attr('id') == 'btnMemberConfirmation' ? 13 : $(this).attr('id') == 'btnGuestConfirmation' ? 14 : 15;
            var _eventID = $(this).data('sysevent');
            $.ajax({
                url: '/crm/MasterChart/SendConfirmationLetter',
                data: { reservationID: $('#tblGeneralInformation_Reservations tbody tr.selected-row').attr('id'), evtID: _eventID },
                type: 'POST',
                //beforeSend: function (xhr) {
                //    UI.checkForPendingRequests(xhr);
                //},
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    var mails = data.ItemID != 0 ? " to these addresses:<br />" + data.ItemID.to.split(',').join('<br />') : '';
                    UI.messageBox(data.ResponseType, data.ResponseMessage + mails + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#GeneralInformation_OptionsSoldInformation_Quantity').on('change', function () {
            var _value = parseInt($('#GeneralInformation_OptionsSoldInformation_Quantity').val()) * parseFloat($('#GeneralInformation_OptionsSoldInformation_OptionPrice').val());
            $('#GeneralInformation_OptionsSoldInformation_TotalPaid').val(_value);
        });

        $('#GeneralInformation_OptionsSoldInformation_CreditAmount, #GeneralInformation_OptionsSoldInformation_TotalPaid').on('keydown', function (e) {
            e.preventDefault();
        });

        $('#GeneralInformation_OptionsSoldInformation_EligibleForCredit').on('change', function () {
            if ($(this).val().toLowerCase() == 'true') {
                $.getJSON('/MasterChart/GetOptionCreditAmount', { optionID: $('#GeneralInformation_OptionsSoldInformation_OptionID').val() }, function (data) {
                    $('#GeneralInformation_OptionsSoldInformation_CreditAmount').val(data);
                });
            }
            else {
                $('#GeneralInformation_OptionsSoldInformation_CreditAmount').val('0');
            }
        });
        //$('#GeneralInformation_OptionsSoldInformation_OptionType').on('change', function () {
        //$.ajax({
        //    url: '/CRM/masterchart/GetOptionTypesByPlace',
        //    type: 'POST',
        //    data: { resortID: $('#PlaceID').val(), optionCategory: $(this).val() },
        //    success: function (data) {
        //        $('#GeneralInformation_OptionsSoldInformation_OptionID').fillSelect(data);
        //    }
        //});
        //});
    };
    var show = function (leadID) {
        //hide reservations panel
        $('#HotelReservationsPanel').show();
        UI.collapseFieldset("fdsReservations");


        //--//--//-

        ////make sure fdsReservations  and fdsCurrentReservations are expanded
        //$('#fdsReservations, #fdsCurrentReservations, #fdsHotelReservationsDetails ').each(function () {
        //    UI.expandFieldset($(this).attr('id'));
        //}).queue(function () {
        //    //collapse any #fdsReservations's nested fieldset after #fdsCurrentReservations
        //    $("#fdsReservations #fdsCurrentReservations ~ fieldset ").each(function () {
        //        UI.collapseFieldset($(this).attr('id'));
        //        console.log($(this).attr('id'))
        //    }).queue(function () {
        //        UI.scrollTo("fdsReservations");
        //        $(this).dequeue();
        //    });

        //});

        //COMMON.expandAndCollapse(
        //    '#fdsReservations, #fdsCurrentReservations, #fdsHotelReservationsDetails',
        //    '#fdsReservations #fdsCurrentReservations ~ fieldset'          
        //    );
        //console.log("scrolling to fdsReservations");
        //UI.scrollTo('fdsReservations');
        //--//--//-


        //fdsCurrentReservations
        //fdsHotelReservationsDetails

        // set leadID
        var leadID = $("#GeneralInformation_LeadID").val();
        $("#frmReservationGeneralInformation").clearForm();
        //reservation LeadID
        $("#LeadID").val(leadID);
        //presentation LeadID
        $("#GeneralInformation_PresentationInformation_LeadID").val(leadID);
        RESERVATION_GENERAL_INFORMATION.searchLeadReservations(leadID);
        bindFunctionality();

        //$.ajax({
        //    url: '/CRM/Masterchart/GetReservationPanel',
        //    data: {leadID:leadID},
        //    dataType: 'html',
        //    success: function (data) {
        //        $('#HotelReservationsPanel').html(data);
        //        //expand the fieldset here
        //        RESERVATION_GENERAL_INFORMATION.searchLeadReservations(leadID);
        //        bindFunctionality();
        //    }
        //});


    }
    var searchLeadReservations = function (leadID, reservationID) {
        var ReservationID = reservationID;
        $.ajax({
            url: '/CRM/Masterchart/SearchReservations',
            data: { leadID: leadID },
            type: "POST",
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                UTILS.jsonToFormFields($.parseJSON(data));
                UI.setTableRowsClickable({
                    tblID: "tblGeneralInformation_Reservations",
                    onClickCallbackFunction: RESERVATION_GENERAL_INFORMATION.findReservation
                });

                if (reservationID) { // if there is an specified reservation, show it.
                    RESERVATION_GENERAL_INFORMATION.findReservation(ReservationID);
                }

            }
        });

    }
    var findReservation = function (params) {

        $.ajax({
            url: '/CRM/Masterchart/FindReservation',
            data: { reservationID: ($.isPlainObject(params)) ? params.currentTargetID : params },
            type: "POST",
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var json = $.parseJSON(data);
                //make sure we clear the data-selected-value from PlaceID (for example)
                // we can also do this using [data-single-item-method] as selector.
                $("[data-cascading-feeding-method]").data("selected-value", "");
                UTILS.jsonToFormFields(json.reservationDetails);
                //$('#DestinationID').trigger('change');

                $("#GeneralInformation_PresentationInformation_LeadID").val(json.reservationDetails.LeadID);

                //set letters confirmation number
                $('.options-conf').attr('href', 'https://www.resortcom.com/ConfirmationRpt/WebPDFCreator.aspx?Enc=False&ConfirmType=1&ActiveOnly=true&Confirm=' + json.reservationDetails.HotelConfirmationNumber);
                $('.transportation-conf').attr('href', 'https://www.resortcom.com/ConfirmationRpt/WebPDFCreator.aspx?Enc=False&ConfirmType=2&ActiveOnly=true&Confirm=' + json.reservationDetails.HotelConfirmationNumber);

                COMMON.collapseAndExpand(
                    '#fdsReservations #fdsHotelReservationsDetails ~ fieldset',
                    '#fdsHotelReservationsDetails')
                    .done(
                        COMMON.executeDelayed(UI.scrollTo, "fdsHotelReservationsDetails", 400),
                        COMMON.focusFirstEditableElement('fdsHotelReservationsDetails')
                    );

                ;
            }
        });
    }
    var RESPONSE_HANDLERS = function () {



        function saveReservation(response) {

            var duration = (response.ResponseType < 0) ? duration = response.ResponseType : null;
            var message = response.ResponseMessage;
            if (response.ExceptionMessage != "") { message += "<br />" + response.ExceptionMessage; }

            var m = {
                type: response.ResponseType,
                message: message,
                duration: duration,
                innerException: response.InnerException
            }
            UI.messageBox(m);

            if (response.ResponseType == 1) {
                //show reservations panel

                //RESERVATION_GENERAL_INFORMATION.searchLeadReservations(response.ObjectIds.LeadID, response.ObjectIds.ReservationID);
                RESERVATION_GENERAL_INFORMATION.searchLeadReservations(response.ObjectIds.LeadID);
                RESERVATION_GENERAL_INFORMATION.resetAll();

                COMMON.collapseAndExpand('#fdsReservations #fdsCurrentReservations ~ fieldset', '#fdsReservations, #fdsCurrentReservations')
                    .done(
                        COMMON.executeDelayed(UI.scrollTo, "fdsCurrentReservations"),
                        CHARGES_INFO.searchLeadPayments(response.ObjectIds.LeadID)
                    );

            }



            //$("#GeneralInformation_LeadID").val(response.GeneralInformation_LeadID);
            ////get composed or calculated values for the User notification
            //var duration = (response.ResponseType < 0) ? duration = response.ResponseType : null;
            //var message = response.ResponseMessage;
            //if (response.ExceptionMessage != "") { message += "<br />" + response.ExceptionMessage; }

            //var m = {
            //    type: response.ResponseType,
            //    message: message,
            //    duration: duration,
            //    innerException: response.InnerException
            //}
            //UI.messageBox(m);

            //if (response.ResponseType == 1) {
            //    var leadID = $("#GeneralInformation_LeadID").val();
            //    var params = { currentTargetID: leadID }
            //    REQUEST_HANDLERS.findLead(params);
            //    RESERVATION_GENERAL_INFORMATION.show(leadID);
            //    //show reservations panel
            //}

        }

        return {
            saveReservation: saveReservation
        }

    }();
    var REQUEST_HANDLERS = function () {
        return {}
    }();
    var setTotalPaid = function () {
        var _subtotal = 0;
        $('#tblOptionsSold tbody tr:not(:hidden)').each(function (i, item) {
            _subtotal += parseFloat($(this).children('td:nth-child(10)').text().trim());
        });
        $('#TotalPaid').val(_subtotal);
        $('span[data-read-only-text-for="TotalPaid"]').text(_subtotal);//if non-editable
    }
    //function beforeSubmit(frmID, evnt) {
    //    var validation = UI.validateRelatedElements(frmID);

    //    //var invalidElements = UI.validateRelatedElements(frmID);

    //    if (validation.invalidElements.length > 0) {
    //        var hld = new SETTINGS.highlightElements();
    //        hld.elements = validation.invalidElements;
    //        hld.highlightTime = -1;
    //        UI.highlightElements(hld);
    //    }

    //    //Validate the form so UL of the validation summary gets generated
    //    UI.validateForm(frmID);
    //    var validationErrorsList = UI.getValidationErrorsList(frmID, validation.errors);

    //    //Display all the errors
    //    UI.displayValidationErrors(validationErrorsList);

    //    if (validation.invalidElements.length > 0) {
    //        evnt.preventDefault();
    //        evnt.stopImmediatePropagation();
    //        return false;
    //    }
    //}
    //beforeSubmit: beforeSubmit,
    return {
        init: init,
        show: show,
        bindFunctionality: bindFunctionality,
        searchLeadReservations: searchLeadReservations,
        findReservation: findReservation,
        RESPONSE_HANDLERS: RESPONSE_HANDLERS,
        resetAll: resetAll,
        newReservation: newReservation,
        setTotalPaid: setTotalPaid
    }
}();

var BILLING_INFO = function () {

    var init = function () {

        $('#CardNumber').numeric();

        $('#CardCVV').numeric();

        $('#btnSaveBillingInfo').on('click', function () {
            PURCHASE.saveAndReturnFlag = false;
            $('#frmBillingInfo').submit();
        });
    }

    function resetAll() {
        try {
            var subject = "BillingInfo";
            $("#frm" + subject).clearForm();
            UI.deselectAllSelectedRows('tbl' + subject);
            COMMON.getRelationshipsValues("frm" + subject);
        }
        catch (e) { }
    }

    var newBillingInfo = function () {
        $("#fdsBillingInfoDetails").clearForm();
        UI.deselectAllSelectedRows("tblBillingInfo");
        COMMON.getRelationshipsValues("frmBillingInfo");
        COMMON.collapseAndExpand('#fdsBillingInfo #fdsBillingInfoDetails ~ fieldset', '#fdsBillingInfo, #fdsCurrentBillingInfo, #fdsBillingInfoDetails')
            .done(
                COMMON.executeDelayed(UI.scrollTo, "fdsBillingInfoDetails"),
                COMMON.focusFirstEditableElement('fdsBillingInfoDetails')
            );
    };

    var searchLeadBillingInfo = function (leadID, billingInfoID) {
        var BillingInfoID = billingInfoID;
        $.ajax({
            url: '/CRM/Masterchart/SearchBillingInfo',
            data: { leadID: leadID },
            type: "POST",
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                UTILS.jsonToFormFields($.parseJSON(data));
                UI.setTableRowsClickable({
                    tblID: "tblBillingInfo",
                    onClickCallbackFunction: BILLING_INFO.findBillingInfo
                });

                if (BillingInfoID) { // if there is an specified billingInfo, show it.
                    BILLING_INFO.findBillingInfo(BillingInfoID);
                }

                try {
                    CHARGES_INFO.searchLeadBillingInfo(leadID);
                } catch (e) {

                }



            }
        });

    }

    var findBillingInfo = function (params) {

        $.ajax({
            url: '/CRM/Masterchart/FindBillingInfo',
            data: { billingInfoID: ($.isPlainObject(params)) ? params.currentTargetID : params },
            type: "POST",
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var json = $.parseJSON(data);
                UTILS.jsonToFormFields(json.billingInfoDetails);


                // $("#BillingInfo_LeadID").val(json.reservationDetails.BillingInfo_LeadID);

                COMMON.collapseAndExpand(
                    '#fdsBillingInfo #fdsBillingInfoDetails ~ fieldset',
                    '#fdsBillingInfoDetails')
                    .done(
                        COMMON.executeDelayed(UI.scrollTo, "fdsBillingInfoDetails", 400),
                        COMMON.focusFirstEditableElement('fdsBillingInfoDetails')
                    );

                ;
            }
        });
    }

    var show = function (leadID) {
        //hide reservations panel
        $('#frmBillingInfo').show();
        UI.collapseFieldset("fdsBillinginfo");

        // set leadID
        // var leadID = $("#BillingInfo_LeadID").val();
        $("#frmBillingInfo").clearForm();
        //Billing Info LeadID
        //$("#BillingInfo_LeadID").val(leadID);
        COMMON.getRelationshipsValues("frmBillingInfo");

        BILLING_INFO.searchLeadBillingInfo(leadID);
        //bindFunctionality();

        if ($('#tblPurchases tbody tr.selected-row').length > 0) {
            UI.updateDependentLists('/MasterChart/GetDDLData', 'billingInfo', leadID, false);
            COMMON.collapseAndExpand('#fdsBillingInfo, #fdsBillingInfo > fieldset', '#fdsPurchasesManagement, #fdsPurchaseInfo, #fdsPurchasesServices, #fdsPurchasesPayments, #fdsPurchasePaymentInfo').done(
                COMMON.executeDelayed(UI.scrollTo, 'fdsPurchasesPayments', 400)
            );
        }
    }

    var RESPONSE_HANDLERS = function () {

        function saveBillingInfo(response) {

            var duration = (response.ResponseType < 0) ? duration = response.ResponseType : null;
            var message = response.ResponseMessage;
            if (response.ExceptionMessage != "") { message += "<br />" + response.ExceptionMessage; }

            var m = {
                type: response.ResponseType,
                message: message,
                duration: duration,
                innerException: response.InnerException
            }
            UI.messageBox(m);

            if (response.ResponseType == 1) {
                //show billing info panel
                if (PURCHASE.saveAndReturnFlag == true) {
                    $.getJSON('/MasterChart/GetDDLData', { itemType: 'billingInfo', itemID: $('#GeneralInformation_LeadID').val() }, function (data) {
                        $('#PurchasePayment_BillingInfo').fillSelect(data);
                        $('#PurchasePayment_BillingInfo option[value="' + response.ObjectIds.BillingInfoID + '"]').attr('selected', true);
                        COMMON.collapseAndExpand('#fdsBillingInfo fieldset, #fdsBillingInfo', '#fdsPurchasesManagement, #fdsPurchasesManagement fieldset:not(#fdsPurchaseServiceInfo)').done(
                            COMMON.executeDelayed(UI.scrollTo, "fdsPurchasesPayments"));
                    });
                }
                else {
                    BILLING_INFO.searchLeadBillingInfo(response.ObjectIds.LeadID);
                    BILLING_INFO.newBillingInfo();
                }
                //COMMON.collapseAndExpand('#fdsBillingInfo #fdsCurrentBillingInfo ~ fieldset', '#fdsBillingInfo, #fdsCurrentBillingInfo')
                //    .done(
                //        COMMON.executeDelayed(UI.scrollTo, "fdsCurrentBillingInfo")
                //    );
            }
        }

        return {
            saveBillingInfo: saveBillingInfo
        }

    }();

    function pullInfo() {
        $('#btnPullInfo').unbind('click').on('click', function () {
            $('#FirstName').val($('#GeneralInformation_FirstName').val());
            $('#LastName').val($('#GeneralInformation_LastName').val());
            $('#Address').val($('#GeneralInformation_Address').val());
            $('#City').val($('#GeneralInformation_City').val());
            $('#State').val($('#GeneralInformation_State').val());
            $('#CountryID option[value="' + $('#GeneralInformation_countryID option:selected').val() + '"]').attr('selected', true);
            $('#Zipcode').val($('#GeneralInformation_ZipCode').val());
            $('#CardHolderName').val($('#GeneralInformation_FirstName').val() + ' ' + $('#GeneralInformation_LastName').val());
        });
    }

    pullInfo();

    return {
        init: init,
        resetAll: resetAll,
        newBillingInfo: newBillingInfo,
        show: show,
        pullInfo: pullInfo,
        searchLeadBillingInfo: searchLeadBillingInfo,
        findBillingInfo: findBillingInfo,
        RESPONSE_HANDLERS: RESPONSE_HANDLERS
    }
}();
var CHARGES_INFO = function () {
    var show = function (leadID) {
        //hide reservations panel
        $('#frmChargesInfo').show();
        UI.collapseFieldset("frmChargesInfo");
        $("#frmChargesInfo").clearForm();
        COMMON.getRelationshipsValues("frmChargesInfo");

        //CHARGES_INFO.searchLeadBillingInfo(leadID); // this call is been made from BILLING_INFO.searchLeadBillingInfo
        CHARGES_INFO.searchLeadPayments(leadID);
        $("#CHARGES_INFO_LeadID").val(leadID);
        //bindFunctionality();




    }
    var searchLeadBillingInfo = function (leadID) {
        var tblID = "tblCharges_BillingInfo";
        var table = document.getElementById(tblID);
        if ($.fn.DataTable.fnIsDataTable(table)) {
            var oTable = $('#' + tblID).dataTable();
            oTable.fnClearTable();
            oTable.fnDestroy();
        }

        $.ajax({
            url: '/CRM/Masterchart/SearchBillingInfo',
            data: { leadID: leadID },
            type: "POST",
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var billingInfo = $.parseJSON(data).leadBillingInfo;
                var ddl = $("#fdsChargesInfo #" + tblID);
                for (var dr in billingInfo) {
                    UTILS.addJsonRowToTable(tblID, billingInfo[dr]);
                }
                //"asStripeClasses": ['odd', 'striped'], //we took out this classes so we can see when a row is selected
                $('#' + tblID).dataTable(
                    {

                        "sDom": 'T<"clear"><"left"t><"clear">',
                        "oTableTools": {
                            "sRowSelect": "single",
                            "aButtons": []
                        }
                    });
            }
        });
    }

    var searchLeadPayments = function (leadID) {
        try {
            var tblID = "tblCharges_ReservationPayments";
            var table = document.getElementById(tblID);
            if ($.fn.DataTable.fnIsDataTable(table)) {
                var oTable = $('#' + tblID).dataTable();
                oTable.fnClearTable();
                oTable.fnDestroy();
            }

        } catch (e) {

        }

        $.ajax({
            url: '/CRM/Masterchart/SearchPendingCharges',
            data: { leadID: leadID },
            type: "POST",
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var pendingCharges = $.parseJSON(data).pendingCharges;
                for (var dr in pendingCharges) {
                    UTILS.addJsonRowToTable(tblID, pendingCharges[dr]);
                }
                //"asStripeClasses": ['odd', 'striped'], //we took out this classes so we can see when a row is selected
                $('#' + tblID).dataTable(
                    {
                        "sDom": '<"left"T>t',
                        "aoRowCallback": [UI.tablesHoverEffect()],
                        "oTableTools": {
                            "sRowSelect": "multi",
                            "aButtons": ["select_all", "select_none"]
                        }
                    });
            }
        });



    }

    var getSelectedBillingInfo = function () {
        //var params = new SETTINGS.tableDataToJason();
        //params.tableID = "tblCharges_BillingInfo";
        //params.subject = "billingInfo";
        //var tableJsonData = UTILS.tableDataValuesToJson(params);
        //var jsonSelectedRows = [];
        //var selectedRows = $("#tblCharges_BillingInfo  tr").each(function () {
        //    if ($(this).hasClass('DTTT_selected')) {
        //        var index = $(this).index();
        //    jsonSelectedRows.push(tableJsonData[index]);
        //    }

        //});
        var jsonSelectedRows = getSelectedRowsJson('tblCharges_BillingInfo');
        return jsonSelectedRows;
    }

    var getSelectedPaymentsInfo = function () {
        //var params = new SETTINGS.tableDataToJason();
        //params.tableID = "tblCharges_ReservationPayments";
        //params.subject = "billingInfo";
        //var tableJsonData = UTILS.tableDataValuesToJson(params);
        //var jsonSelectedRows = [];
        //var selectedRows = $("#tblCharges_ReservationPayments  tr").each(function () {
        //    if ($(this).hasClass('DTTT_selected')) {
        //        var index = $(this).index();
        //        jsonSelectedRows.push(tableJsonData[index]);
        //    }

        //});


        var jsonSelectedRows = getSelectedRowsJson('tblCharges_ReservationPayments');
        return jsonSelectedRows;
    }

    var getSelectedRowsJson = function (tableID) {
        var params = new SETTINGS.tableDataToJason();
        params.tableID = tableID;
        params.subject = "items";
        var tableJsonData = UTILS.tableDataValuesToJson(params);
        var jsonSelectedRows = [];
        var selectedRows = $("#" + tableID + "  tr").each(function () {
            if ($(this).hasClass('DTTT_selected')) {
                var index = $(this).index();
                jsonSelectedRows.push(tableJsonData[index]);
            }

        });
        return jsonSelectedRows;
    }

    var paymentSummary = function (selectedBillingInfo, totalAmount, totalSelectPayments) {


        var bt = "<table>";
        bt += "     <thead>";
        bt += "         <tr><th>Charge Details</th><th></th></tr>";
        bt += "     </thead>";
        bt += "     <tbody>";
        bt += "         <tr><td>Card Holder Name</td><td>" + selectedBillingInfo[0].CardHolderName + "</td></tr>";
        bt += "         <tr><td>Card Number</td><td>" + selectedBillingInfo[0].CardNumber + "</td></tr>";
        bt += "         <tr><td>Card Expiry</td><td>" + selectedBillingInfo[0].CardExpiry + "</td></tr>";
        bt += "         <tr><td>Card CVV</td><td>" + selectedBillingInfo[0].CardCVV + "</td></tr>";
        bt += "         <tr><td><hr /></td><td><hr /></td></tr>";
        bt += "         <tr><td>Payments Selected</td><td class='right'>" + totalSelectPayments + "</td></tr>";
        bt += "         <tr><td><strong>Total Amount</strong></td><td class='right'><strong>$ " + totalAmount.toFixed(2); + "</strong></td></tr>";
        bt += "     </tbody>";
        bt += " </table>";

        return bt;

    }

    var confirmChargeData = function () {
        var selectedBillingInfo = CHARGES_INFO.getSelectedRowsJson('tblCharges_BillingInfo');
        var selectedPayments = CHARGES_INFO.getSelectedRowsJson('tblCharges_ReservationPayments');

        if (selectedBillingInfo.length != 1) {
            UI.messageBox(-1, "You must select the <strong>Billing Info</strong> to be used in this charge.", 5);
            return;
        }

        if (selectedPayments.length <= 0) {
            UI.messageBox(-1, "You must select at least 1 <strong>Reservation Payment</strong> to be charged.", 5);
            return;
        }

        var totalAmount = 0;

        var selectedMerchants = [];

        for (x in selectedPayments) {
            var paymentSubtotal = parseFloat(selectedPayments[x].Amount)
            if (paymentSubtotal <= 0) {
                UI.messageBox(-1, "The payment marked as <strong>" + selectedPayments[x].ChargeType + " - " + selectedPayments[x].ChargeDescription + "</strong>  has an invalid amount ($ " + paymentSubtotal.toFixed('N2') + "), the charge cannot be made including this payment, please deselect it and try again.");
                return;
            }
            totalAmount = totalAmount + paymentSubtotal;

            // keep track of the different merchant accounts selected.
            var merchantID = selectedPayments[x].ToBeBilledByID;
            if ($.grep(selectedMerchants, function (e, i) { return (e.merchantID == merchantID) }).length == 0) {
                selectedMerchants.push({ merchantID: merchantID, merchantName: selectedPayments[x].ToBeBilledBy });
            }
        }


        if (selectedMerchants.length > 1) {
            var multipleMerchantsMsg = "The payments you've selected go for different accounts:<ul>";
            for (m in selectedMerchants) {
                multipleMerchantsMsg += "<li>" + selectedMerchants[m].merchantName + "</li>";
            }
            multipleMerchantsMsg += "</ul><br />Please select payments to be billed for the same account only and try again.";
            UI.messageBox(-1, multipleMerchantsMsg, 5);
            return;
        }
        ////the next condition doesn't apply anymore since we have a similar one in the for loop that calculates the total to be charged.
        //if (total <= 0) {
        //    UI.messageBox(-1, "The total to be charge is 0, please revise the ammount of each selected payment.", 5);
        //    return;
        //}

        var confirmMessage = "You are about to make a charge with the following information :<br /><br />";
        confirmMessage += paymentSummary(selectedBillingInfo, totalAmount, selectedPayments.length);
        confirmMessage += "<br />Do you want to proceed?";
        UI.confirmBox(
            confirmMessage,
            CHARGES_INFO.makeCharge,
            [{ selectedBillingInfo: selectedBillingInfo, selectedPayments: selectedPayments }]
        );

        $("#messageBoxContent table").dataTable(
            {
                "asStripeClasses": ['odd', 'striped'],
                "bSort": false,
                "aoColumns": [{ "bSortable": false }, { "bSortable": false }],
                "sDom": ''
            });
    }

    function makeCharge(chargeDetails) {
        //chargeDetails.selectedBillingInfo
        //chargeDetails.selectedPayments
        var billingInfoID = chargeDetails.selectedBillingInfo[0].BillingInfoID;
        var paymentDetailsIDs = [];
        for (x in chargeDetails.selectedPayments) {
            paymentDetailsIDs.push(chargeDetails.selectedPayments[x].ReservationPaymentDetailsID);
        }
        var LeadID = $("#CHARGES_INFO_LeadID").val();
        // make the charge request.

        //MakeCharge(int BillingInfoID, string ReservationPaymentIds, Guid LeadID)
        $.ajax({
            url: '/CRM/Masterchart/MakeCharge',
            data: { BillingInfoID: billingInfoID, ReservationPaymentIds: paymentDetailsIDs.toString(), LeadID: LeadID },
            type: "POST",
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (d) {
                var data = $.parseJSON(d);
                UI.messageBox(data.ResponseType, data.ResponseMessage + "<br >" + data.ExceptionMessage, 5, data.InnerException);
                //once the charge is made, search payments again, so we can update the payments list even if there
                // was an error in the charge, so the user can see the error codes.
                CHARGES_INFO.searchLeadPayments(data.ObjectIds);
            }
        });


    }

    return {
        show: show,
        searchLeadBillingInfo: searchLeadBillingInfo,
        searchLeadPayments: searchLeadPayments,
        getSelectedBillingInfo: getSelectedBillingInfo,
        getSelectedPaymentsInfo: getSelectedPaymentsInfo,
        getSelectedRowsJson: getSelectedRowsJson,
        paymentSummary: paymentSummary,
        confirmChargeData: confirmChargeData,
        makeCharge: makeCharge
    }
}();

$.validator.unobtrusive.adapters.add('arrayminlength', ['PurchasePayment_Coupons'], function (options) {
    options.rules['arrayminlength'] = {
        other: options.params.other,
        PurchasePayment_Coupons: options.params.PurchasePayment_Coupons
    };
    options.messages['arrayminlength'] = options.message;
});

$.validator.addMethod('arrayminlength', function (value, element, params) {
    if (value.length == 0) {
        return false;
    }
    else {
        return true;
    }
});

var PURCHASE = function () {

    var c = 1;

    var oPurchaseTable;
    var disabledDays;
    var indicator;
    var saveAndReturnFlag;

    var oCouponsTable;

    var oPurchaseServiceTable;

    var oPurchasePaymentTable;

    var init = function () {

        function PurchaseServiceModel() {
            this.PurchaseService_PurchaseServiceID = null;
            this.PurchaseService_Purchase = null;
            this.PurchaseService_Service = null;
            this.PurchaseService_ServiceText = null;
            this.PurchaseService_WholeStay = false;
            this.PurchaseService_OpenCoupon = null;
            this.PurchaseService_ServiceDateTime = null;
            this.PurchaseService_WeeklyAvailability = null;
            this.PurchaseService_WeeklyAvailabilityString = null;
            this.PurchaseService_MeetingPoint = null;
            this.PurchaseService_ServiceStatus = null;
            this.PurchaseService_ConfirmationDateTime = null;
            this.PurchaseService_ConfirmationNumber = null;
            this.PurchaseService_ConfirmedByUser = null;
            this.PurchaseService_Promo = null;
            this.PurchaseService_Total = null;
            this.PurchaseService_Currency = null;
            this.PurchaseService_ReservedFor = null;
            this.PurchaseService_ChildrenAges = null;
            this.PurchaseService_CustomMeetingPoint = null;
            this.PurchaseService_CustomMeetingTime = null;
            this.PurchaseService_Note = null;
            this.PurchaseService_Airline = null;
            this.PurchaseService_FlightNumber = null;
            this.PurchaseService_TransportationZone = null;
            this.PurchaseService_VehicleType = null;
            this.PurchaseService_SpecialRequest = null;
            this.PurchaseService_CancelationCharge = null;
            this.PurchaseService_CancelationDateTime = null;
            this.PurchaseService_CancelationNumber = null;
            this.PurchaseService_Audit = null;
            this.PurchaseService_CloseOut = null;
            this.PurchaseService_Issued = null;
            this.PurchaseService_CouponReference = null;
            this.PurchaseService_SoldByOPC = null;
            this.PurchaseService_Round = null;
            this.PurchaseService_RoundAirline = null;
            this.PurchaseService_RoundFlightNumber = null;
            this.PurchaseService_RoundFlightTime = null;
            this.PurchaseService_RoundMeetingTime = null;
            this.ListPurchaseServiceDetails = null;
            this.PurchaseService_CurrentUnits = null;
            this.PurchaseService_ReplacementOf = null;
            this.PurchaseService_CanceledByUser = null;
        }

        function ListPurchaseServiceDetailModel() {
            this.ServiceDetailID = null;
            this.PurchaseServiceID = null;
            this.PriceTypeID = null;
            this.DealPrice = null;
            this.PriceID = null;
            this.Unit = null;
            this.Coupon = null;
            this.Quantity = null;
            this.CustomPrice = null;
            //this.PurchaseServiceDetail_NetPrice = null;
            this.Promo = null;
            this.ExchangeRateID = null;
        }

        function getMinDate() {
            var date = COMMON.serverDateTime;
            var month = (date.getMonth() + 1) >= 10 ? (date.getMonth() + 1) : padNumber((date.getMonth() + 1), 2);
            var minDate = date.getFullYear() + '-' + month + '-' + date.getDate();
            return minDate;
        }

        PURCHASE.searchResultsTable($('#tblPurchases'));

        $('#divCouponsTabs').tabs();
        $('#SearchCoupon_PointOfSale option[value="' + ((localStorage.Eplat_PointOfSale != undefined && localStorage.Eplat_PointOfSale) != '' ? localStorage.Eplat_PointOfSale : 0) + '"]').attr('selected', true);
        var _interval = setInterval(function () {
            if ($('#SearchCoupon_PurchaseDate').hasClass('hasDatepicker')) {
                $('#SearchCoupon_PurchaseDate').datepicker('setDate', new Date());
                $('#SearchCoupon_PurchaseDate').datepicker('refresh');
                clearInterval(_interval);
            }
        }, 1000);

        //$.ajax({
        //    url: '/crm/MasterChart/QuickSaleDependantLists',
        //    cache: false,
        //    success: function (data) {
        //        UI.loadDependantFields(data);
        //    }
        //});

        $('#SearchCoupon_PointOfSale').on('change', function () {
            $.getJSON('/crm/MasterChart/IsPoSOnline', { pointOfSaleID: $(this).val() }, function (data) {
                $('#hdnIsPoSOnline').val(data.isOnline);
            });
        });

        $('#PurchaseInfo_Terminal').on('change', function (e, params) {
            if ($(this).val() != 'null' && $(this).val() != null) {
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'allSalesAgentsByTerminal', itemID: $(this).val() }, function (data) {
                    var _agent = params != undefined ? params.agent : '';
                    var _user = params != undefined ? params.user : '';
                    $('#PurchaseInfo_User').fillSelect(data);
                    $('#PurchaseInfo_Agent').fillSelect(data);
                    ///new lines to refresh list of users
                    $('#PurchaseService_ConfirmedByUser').fillSelect(data);
                    $('#PurchaseService_CanceledByUser').fillSelect(data);
                    ////
                    $('#PurchaseInfo_User option[value="' + _user + '"]').attr('selected', true);
                    $('#PurchaseInfo_Agent option[value="' + _agent + '"]').attr('selected', true);
                });
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'place', itemID: $(this).val() }, function (data) {
                    var _stayingAtPlace = params != undefined ? params.stayingAtPlace : '0';
                    $('#PurchaseInfo_StayingAtPlace').fillSelect(data);
                    $('#PurchaseInfo_StayingAtPlace option[value="' + _stayingAtPlace + '"]').attr('selected', true);
                    $('#PurchaseInfo_StayingAtPlace').multiselect('refresh');
                    $('#PurchaseInfo_StayingAtPlace').trigger('change', params);
                });
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'location', itemID: $(this).val() }, function (data) {
                    $('#PurchasePayment_Location').fillSelect(data);
                });
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'providersPerTerminal', itemID: $(this).val() }, function (data) {
                    $('#PurchaseService_Provider').fillSelect(data);
                    $('#PurchaseService_Provider').multiselect('refresh');
                });
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'promosPurchases', itemID: $(this).val() }, function (data) {
                    $('#PurchaseInfo_Promo').fillSelect(data);
                    var _promo = params != undefined ? params.promo : '0';
                    $('#PurchaseInfo_Promo option[value="' + _promo + '"]').attr('selected', true);
                    if (_promo != '0' && _promo != 0) {
                        $.getJSON('/MasterChart/GetPromoInfo', { promoID: _promo }, function (data) {
                            $('#hdnPurchasePromo').val(JSON.stringify(data));
                        });
                    }
                    else {
                        $('#hdnPurchasePromo').val('');
                    }
                });
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'opcsPerPurchaseTerminal', itemID: $(this).val(), ts: new Date().getTime() }, function (data) {
                    $('#PurchasePayment_OPC').fillSelect(data);
                    $('#PurchasePayment_OPC').trigger('change');
                });
                $('#PurchaseInfo_Currency').trigger('change');
            }
        });

        $('.update-opc-list').on('click', function () {
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'opcsPerPurchaseTerminal', itemID: $('#PurchaseInfo_Terminal option:selected').val(), ts: new Date().getTime() }, function (data) {
                $('#PurchasePayment_OPC').fillSelect(data);
                $('#PurchasePayment_OPC').trigger('change');
            });
        });

        $('#PurchaseInfo_Currency').on('change', function () {
            if (PURCHASE.oPurchaseServiceTable == undefined || $('#tblPurchasesServices tbody tr').length == 0) {
                $('#PurchaseService_Currency').val($(this).val());
                $('#PurchasePayment_Currency').val($(this).val());
                $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').val(0).trigger('change');
            }
        });

        $('#PurchaseInfo_StayingAtPlace').on('change', function (e, params) {
            var _params = params != undefined ? params.stayingAt : '';
            $('#PurchaseInfo_StayingAt').val(_params);
            if ($(this).val() == 'null') {
                $('#divStayingAt').show();
            }
            else {
                $('#divStayingAt').hide();
            }
        });

        $('#btnSavePurchase').on('click', function () {
            //set date in purchaseInfo
            if ($('#PurchaseInfo_PurchaseID').val() == '') {
                var date = COMMON.serverDateTime;
                var month = (date.getMonth() + 1) >= 10 ? (date.getMonth() + 1) : padNumber((date.getMonth() + 1), 2);
                $('#PurchaseInfo_PurchaseDateTime').val(date.getFullYear() + '-' + month + '-' + date.getDate());
            }
            $('#frmPurchaseInfo').submit();
        });

        //----------------------------------------------------------------------------

        $('#PurchaseService_CancelationCharge').numeric();

        $('#PurchaseService_ServiceDateTime').datepicker({
            'changeMonth': true,
            'changeYear': true,
            'dateFormat': 'yy-mm-dd',
            constrainInput: true,
            beforeShowDay: PURCHASE.closedDays,
            onClose: function (dateText, inst) {
                onCloseServiceDate(dateText);
            }
        });

        $('#btnNewPurchaseServiceInfo').on('click', function (e, params) {
            $('#PurchaseService_Provider option.option-hidden').remove();
            $('#PurchaseService_Service option.option-hidden').remove();
            $('#PurchaseService_Provider').multiselect('refresh');
            $('#PurchaseService_Service').multiselect('refresh');
            if ($('#tblPurchasesServices tbody').find('.selected-row').length > 0) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            }
            else {
                $('#frmPurchaseServiceInfo').clearForm(false);
                $('#frmPurchaseServiceInfo .secondary-selected-row-dependent').each(function () {
                    if ($(this).is('span')) {
                        $(this).text('');
                    }
                });
                $('#tblPrices tbody').empty();
            }
            $('input:radio[name="PurchaseService_Issued"]')[1].checked = true;
            $('#PurchaseService_ReplacementOf option').each(function () {
                $(this).show();
            });
            $('#PurchaseService_OpenCoupon option[value="' + UI.returnMostSelectedValue(localStorage.Eplat_Counter_OpenCoupon, 'OpenCoupon') + '"]').attr('selected', true);
            if (params != undefined) {
                if (params.doScroll) {
                    UI.expandFieldset('fdsPurchaseServiceInfo');
                    UI.scrollTo('fdsPurchaseServiceInfo', null);
                }
            }
            else {
                $('#PurchaseService_ServiceStatus option[value="3"]').attr('selected', true).trigger('change');
                UI.expandFieldset('fdsPurchaseServiceInfo');
                UI.scrollTo('fdsPurchaseServiceInfo', null);
            }
            blockFormEdition(0, false);
            if (PURCHASE.oPurchaseServiceTable.$('tr').length > 0) {
                $('#PurchaseInfo_PointOfSale').on('mousedown', function (e) { e.preventDefault(); });
                $('#PurchaseInfo_Currency').on('mousedown', function (e) { e.preventDefault(); });
                $('#PurchaseInfo_Culture').on('mousedown', function (e) { e.preventDefault(); });
            }
            else {
                $('#PurchaseInfo_PointOfSale').unbind('mousedown');
                $('#PurchaseInfo_Currency').unbind('mousedown');
                $('#PurchaseInfo_Culture').unbind('mousedown');
            }
            UI.resetValidation('frmPurchaseServiceInfo');
        });

        $('#PurchaseService_ServiceDateTime').on('focus', function () {
            if ($(this).datepicker('option', 'minDate') == null) {
                $(this).datepicker('option', 'minDate', getMinDate());
            }
        });

        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').numeric({ decimal: '.', negative: false });

        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').on('keydown', function () {
            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').removeClass('input-validation-error');
        });

        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').on('keyup', function () {
            var _value = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() != '' ? parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val()) : $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder') != '' ? parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder')) : 0
            $('#tblPrices tfoot tr:first').children('td:nth-child(5)').text((parseFloat($(this).val()) * _value));
            UI.applyFormat('currency');
        });

        $('#PurchaseService_ServiceStatus').on('change', function (e) {
            $('.cancelation-info-container').hide();

            switch ($(this).children('option:selected').text().trim()) {
                case "Canceled": {
                    if ($('#PurchaseService_PurchaseServiceID').val() == '') {
                        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val('');
                        $('#tblPrices tbody tr').each(function () {
                            $(this).children('td:nth-child(7)').text('');
                        });
                    }
                    $('.cancelation-info-container').show();
                    break;
                }
                case "Refund": {
                    if ($('#PurchaseService_PurchaseServiceID').val() == '') {
                        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val('');
                        $('#tblPrices tbody tr').each(function () {
                            $(this).children('td:nth-child(7)').text('');
                        });
                    }
                    $('.cancelation-info-container').show();
                    break;
                }
                case "Confirmed": {
                    //en este punto obtengo el cupon siguiente, lo asigno tanto a las filas existentes de unidades como a las nuevas que se puedan agregar
                    var d = new Date();
                    //verificar que existe locación seleccionada
                    if ($('#PurchaseInfo_PointOfSale').val() != '0') {
                        if ($('#hdnCurrentPurchaseService').val() != '' && $.parseJSON($('#hdnCurrentPurchaseService').val()).ListPurchaseServiceDetails.length > 0) {
                            var _currentCoupon = $.parseJSON($('#hdnCurrentPurchaseService').val());
                            if (_currentCoupon.ListPurchaseServiceDetails[0].Coupon != '') {
                                if (_currentCoupon.PurchaseService_CancelationDateTime != '') {
                                    //attempt to confirm canceled coupon
                                    var _subtotal = 0;
                                    $('#tblPrices tbody tr').each(function () {
                                        _subtotal += parseFloat($(this).children('td:nth-child(5)').text().trim().replace('$', '').replace(',', ''));
                                    });
                                    $('#PurchaseService_Total').val(_subtotal);
                                    $('#lblPurchaseService_Total').html($('#PurchaseService_Total').val());
                                    UI.applyFormat('currency', 'tblPrices');
                                    $('#PurchaseService_CancelationDateTime').val('');
                                    $('#PurchaseService_CanceledByUser').val('');
                                }
                            }
                            else {
                                //esta condicion genera un nuevo folio cuando se reconfirma un cupon cancelado sin reseleccionar el cupón, esto es porque no se actualiza el campo hdnCurrentPurchaseService con el folio de la primer confirmación
                                //como no hay cupón asignado a unidades se obtiene el cupón en cola de acuerdo a la locación
                                $.ajax({
                                    url: '/MasterChart/GetNextCouponFolio',
                                    type: 'POST',
                                    cache: false,
                                    data: { purchaseID: $('#PurchaseInfo_PurchaseID').val(), timeSpan: d.getTime() },
                                    success: function (data) {
                                        //si la peticion regresa información se asigna al campo, de lo contrario se arroja mensaje diciendo que no existen cupones vigentes en la locación
                                        if (data.folio != '-1') {
                                            if (data.folio != '0') {
                                                if (data.folio == 'null') {
                                                    UI.messageBox(0, 'No folios assigned to: ' + data.exception
                                                        + '<br />Please be sure that purchase was created on the correct point of sale and save any change', null, null);
                                                }
                                                else {
                                                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(data.folio.split(',')[0] + padNumber(data.folio.split(',')[1], data.padding));
                                                    //una vez obtenido el cupon, se actualiza el campo en cada fila de la tabla de unidades
                                                    $('#tblPrices tbody tr').each(function (index) {
                                                        $(this).children('td:nth-child(7)').text($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '-' + String.fromCharCode(65 + index));
                                                    });
                                                    if ($('#PurchaseService_Promo option:selected').val() != 0 && $.parseJSON($('#hdnGetPromo').val()).isPackable) {
                                                        var promoApplied = false;
                                                        var _couponFolio = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val();
                                                        PURCHASE.oPurchaseServiceTable.$('tr').not('theader').each(function (index, item) {
                                                            var _json = $.parseJSON($(this).children('td:nth-child(7)').children(':hidden').val());
                                                            var a = COMMON.getDate();
                                                            var b = a.split('-');
                                                            var c = '';
                                                            $.each(b, function (i, x) {
                                                                c += (c == '' ? '' : '-') + UI.padNumber(x, 2);
                                                            });

                                                            if (_json.promoID != 0 && _json.promoID == $('#PurchaseService_Promo option:selected').val() && _json.isPackable && !$(this).hasClass('canceled-row') && _json.date == c) {
                                                                promoApplied = true;
                                                                _couponFolio = $(this).children('td:nth-child(3)').text().trim() != '' ? $(this).children('td:nth-child(3)').text().trim() : _couponFolio;
                                                                return false;
                                                            }
                                                        });
                                                        //overwrite couponFolio used for the coupon in field and table
                                                        if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() != '' || $('#PurchaseService_ServiceStatus option:selected').val() == 3) {
                                                            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(_couponFolio);
                                                            $('#tblPrices tbody tr').each(function (index) {
                                                                $(this).children('td:nth-child(7)').text(_couponFolio + '-' + String.fromCharCode(65 + index));
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            else {
                                                UI.messageBox(-1, 'There is an error with the folios assignment.<br />Please contact your System Administrator', null, null);
                                            }
                                        }
                                        else {
                                            UI.messageBox(-1, 'Could not get coupon folio: <br />' + data.exception, null, null);
                                        }
                                    }
                                });
                            }
                        }
                        else {
                            $.ajax({
                                url: '/MasterChart/GetNextCouponFolio',
                                type: 'POST',
                                cache: false,
                                data: { purchaseID: $('#PurchaseInfo_PurchaseID').val(), timeSpan: d.getTime() },
                                beforeSend: function (xhr) {
                                    UI.checkForPendingRequests(xhr);
                                },
                                success: function (data) {
                                    //si la peticion regresa información se asigna al campo, de lo contrario se arroja mensaje diciendo que no existen cupones vigentes en la locación
                                    if (data.folio != '-1') {
                                        if (data.folio != '0') {
                                            if (data.folio == 'null') {
                                                UI.messageBox(0, 'No folios assigned to: ' + data.exception
                                                    + '<br />Please be sure that purchase was created on the correct point of sale and save any change', null, null);
                                            }
                                            else {
                                                $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(data.folio.split(',')[0] + padNumber(data.folio.split(',')[1], data.padding));
                                                if ($('#tblPrices tbody tr').length > 0 && $('#tblPrices tbody tr:first').children('td:nth-child(7)')[0].textContent == '') {
                                                    $('#tblPrices tbody tr').each(function (index) {
                                                        $(this).children('td:nth-child(7)').text($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '-' + String.fromCharCode(65 + index));
                                                    });
                                                }
                                                if ($('#PurchaseService_Promo option:selected').val() != 0 && $.parseJSON($('#hdnGetPromo').val()).isPackable) {
                                                    var promoApplied = false;
                                                    var _couponFolio = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val();
                                                    PURCHASE.oPurchaseServiceTable.$('tr').not('theader').each(function (index, item) {
                                                        var _json = $.parseJSON($(this).children('td:nth-child(7)').children(':hidden').val());
                                                        if (_json.promoID != 0 && _json.promoID == $('#PurchaseService_Promo option:selected').val() && _json.isPackable && !$(this).hasClass('canceled-row')) {
                                                            promoApplied = true;
                                                            _couponFolio = $(this).children('td:nth-child(3)').text().trim();
                                                            return false;
                                                        }
                                                    });
                                                    //overwrite couponFolio used for the coupon in field and table
                                                    if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() != '' || $('#PurchaseService_ServiceStatus option:selected').val() == 3) {
                                                        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(_couponFolio);
                                                        $('#tblPrices tbody tr').each(function (index) {
                                                            $(this).children('td:nth-child(7)').text(_couponFolio + '-' + String.fromCharCode(65 + index));
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            UI.messageBox(-1, 'There is an error with the folios assignment.<br />Please contact your System Administrator', null, null);
                                        }
                                    }
                                    else {
                                        UI.messageBox(-1, 'Could not get coupon folio: <br />' + data.exception, null, null);
                                    }
                                }
                            });
                        }
                    }
                    else {
                        UI.messageBox(0, 'No Point Of Sale assigned to this purchase', null, null);
                    }
                    break;
                }
                case "No Show": {
                    if ($('#PurchaseService_PurchaseServiceID').val() == '') {
                        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val('');
                        $('#tblPrices tbody tr').each(function () {
                            $(this).children('td:nth-child(7)').text('');
                        });
                    }
                    $('.cancelation-info-container').hide();
                    break;
                }
                default: {
                    $('.cancelation-info-container').hide();
                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val('');
                    $('#tblPrices tbody tr').each(function (index) {
                        $(this).children('td:nth-child(7)').text('');
                    });
                    break;
                }
            }
        });

        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').on('change', function () {
            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val('');
            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder', '');
            if ($('#PurchaseService_Service option:selected').val() != '0') {
                $.ajax({
                    url: '/MasterChart/GetDDLData',
                    cache: false,
                    type: 'POST',
                    data: { itemType: 'priceType', itemID: $(this).val() + '|' + $('#PurchaseService_Service').val() + '|' + $('#PurchaseService_ServiceDateTime').val() + '|' + $('#PurchaseInfo_Terminal').val() + '|' + $('#PurchaseService_Currency').val() + '|' + $('#PurchaseInfo_PointOfSale').val() + '|' + $('#PurchaseInfo_Culture option:selected').val() },
                    //beforeSend: function (xhr) {
                    //    UI.checkForPendingRequests(xhr);
                    //},
                    success: function (data) {
                        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceUnit').fillSelect(data);
                    }
                });
            }
            else {
                $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceUnit').clearSelect();
                $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceUnit').trigger('change');
            }
        });

        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceUnit').on('change', function () {
            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Price').val(($(this).val() != null ? $(this).val().split('|')[0] : ''));
            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder', ($(this).val() != null ? UI.addDecimalPart($(this).val().split('|')[1]) : ''));
            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_ExchangeRate').val(($(this).val() != null ? UI.addDecimalPart($(this).val().split('|')[2]) : ''));
            var _value = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() != '' ? parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val()) : $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder') != '' ? parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder')) : 0
            $('#tblPrices tfoot tr:first').children('td:nth-child(5)').text((parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val()) * _value));
            UI.applyFormat('currency');
        });

        $('#btnAddPrice').on('click', function () {
            //verificar que cantidad no esté en blanco y que el status no sea cancelado
            //if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val() != '' && $('#PurchaseService_ServiceStatus option:selected').val() != 4) {
            if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val() != '') {
                $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').removeClass('input-validation-error');
                if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceUnit').val() != null) {
                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceUnit').removeClass('input-validation-error');
                    if ($('#PurchaseService_ServiceStatus option:selected').val() < 4) {
                        //apply purchase promo
                        var _newSubTotal;
                        if ($('#hdnPurchasePromo').val() != '') {
                            _purchasePromo = $.parseJSON($('#hdnPurchasePromo').val());
                            //if (!_purchasePromo.isPackable && !_purchasePromo.combinable && !_purchasePromo.applyOnPerson) 
                            if ($('#hdnGetPromo').val() == '') {
                                var _price = parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder'));
                                _newSubTotal = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val() * (_purchasePromo.amount == null ? _price - ((_price * _purchasePromo.percentage) / 100) : _price - _purchasePromo.amount);
                                //$('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val(_newPrice);
                                //$('#PurchaseService_Total').val(_newPrice);
                                //se está actualizando el campo customprice y esto modifica también el precio de los cupones. 
                                //se necesita forzar la modificacion solo del total y que la funcion setTotalOfCouponsSold no lo revierta
                            }
                        }

                        /// minimal Price
                        if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() != '' && parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val()) != parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder'))) {
                            $.getJSON('/MasterChart/GetDDLData', { itemType: 'minimalPrice', itemID: ($('#PurchaseInfo_PurchaseID').val() + '|' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Price').val() + '|' + $('#PurchaseService_Promo').val()) }, function (data) {
                                if (data[0] != undefined && parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val()) < parseFloat(data[0].Value.split('|')[1])) {
                                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val('');
                                    UI.messageBox(0, 'New price CANNOT be lower than price set as minimal: $' + UI.addDecimalPart(data[0].Value.split('|')[1]), null, null);
                                }
                                else {
                                    //se obtiene la información de la unidad y se crea la fila en la tabla
                                    var price = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() != '' ? $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() : $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder');
                                    var unit = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceUnit option:selected').text();
                                    var coupon = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() != '' ? $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '-' + String.fromCharCode(65 + $('#tblPrices tbody tr').length) : '';
                                    var subtotal = parseFloat(parseFloat(price) * parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val()));
                                    var builder = '<tr>'
                                        + '<td>' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType option:selected').text()
                                        + '<input type="hidden" value="' + ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() != '' ? $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() : '') + '">'
                                        + '</td>'
                                        + '<td>' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val() + '</td>'
                                        + '<td>'
                                        + '<input type="hidden" value="' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').val() + '">'
                                        + '<input type="hidden" value="' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Price').val() + '">'
                                        + '<input type="hidden" value="' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_ExchangeRate').val() + '">'
                                        + unit
                                        + '</td>'
                                        + '<td class="format-currency">'
                                        + ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() != '' ? $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() : $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder'))
                                        + '</td>'
                                        + '<td class="format-currency">' + ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Promo').is(':checked') ? $.parseJSON($('#hdnGetPromo').val()).isDiscountType ? $.parseJSON($('#hdnGetPromo').val()).amount != null ? UI.addDecimalPart(subtotal - parseFloat($.parseJSON($('#hdnGetPromo').val()).amount)) : UI.addDecimalPart(subtotal - ((subtotal * parseFloat($.parseJSON($('#hdnGetPromo').val()).percentage)) / 100)) : UI.addDecimalPart('0') : UI.addDecimalPart(subtotal.toString())) + '</td>'
                                        + '<td>' + ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Promo').is(':checked') ? 'Yes' : 'No') + '</td>'
                                        + '<td>' + coupon + '</td>'
                                        + '<td><img src="/Content/themes/base/images/trash.png" class="right delete-item">'
                                        + '</td>'
                                        + '</tr>';
                                    $('#tblPrices tbody').append(builder);
                                    UI.applyFormat('currency', 'tblPrices');
                                    //limpia los campos después de agregar unidad a la tabla
                                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val('');
                                    UI.updateLocalStorageCounter('PriceType', $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').val());
                                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').val(0).trigger('change');
                                    //actualiza el valor en los campos "Total" de acuerdo al contenido de unidades haciendo trigger en el drop de promos
                                    //setTotalOfCouponsSold();
                                    setTotalOfCouponsSold(null, null, _newSubTotal);
                                    //habilita el borrado de filas de la tabla de unidades y con esto actualiza la letra del cupón además de los totales
                                    PURCHASE.deleteItemFromTable();
                                }
                            });
                            /// end minimal price
                        }
                        else {
                            //se obtiene la información de la unidad y se crea la fila en la tabla
                            var price = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() != '' ? $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() : $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder');
                            var unit = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceUnit option:selected').text();
                            var coupon = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() != '' ? $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '-' + String.fromCharCode(65 + $('#tblPrices tbody tr').length) : '';
                            var subtotal = parseFloat(parseFloat(price) * parseFloat($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val()));
                            var builder = '<tr>'
                                + '<td>' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType option:selected').text()
                                + '<input type="hidden" value="' + ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() != '' ? $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() : '') + '">'
                                + '</td>'
                                + '<td>' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val() + '</td>'
                                + '<td>'
                                + '<input type="hidden" value="' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').val() + '">'
                                + '<input type="hidden" value="' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Price').val() + '">'
                                + '<input type="hidden" value="' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_ExchangeRate').val() + '">'
                                + unit
                                + '</td>'
                                + '<td class="format-currency">'
                                + ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() != '' ? $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').val() : $('#PurchaseServiceDetailModel_PurchaseServiceDetail_CustomPrice').attr('placeholder'))
                                + '</td>'
                                + '<td class="format-currency">' + ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Promo').is(':checked') ? $.parseJSON($('#hdnGetPromo').val()).isDiscountType ? $.parseJSON($('#hdnGetPromo').val()).amount != null ? UI.addDecimalPart(subtotal - parseFloat($.parseJSON($('#hdnGetPromo').val()).amount)) : UI.addDecimalPart(subtotal - ((subtotal * parseFloat($.parseJSON($('#hdnGetPromo').val()).percentage)) / 100)) : UI.addDecimalPart('0') : UI.addDecimalPart(subtotal.toString())) + '</td>'
                                + '<td>' + ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Promo').is(':checked') ? 'Yes' : 'No') + '</td>'
                                + '<td>' + coupon + '</td>'
                                + '<td><img src="/Content/themes/base/images/trash.png" class="right delete-item">'
                                + '</td>'
                                + '</tr>';
                            $('#tblPrices tbody').append(builder);
                            UI.applyFormat('currency', 'tblPrices');
                            //limpia los campos después de agregar unidad a la tabla
                            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').val('');
                            UI.updateLocalStorageCounter('PriceType', $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').val());
                            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').val(0).trigger('change');
                            //actualiza el valor en los campos "Total" de acuerdo al contenido de unidades haciendo trigger en el drop de promos
                            //setTotalOfCouponsSold();
                            setTotalOfCouponsSold(null, null, _newSubTotal);
                            //habilita el borrado de filas de la tabla de unidades y con esto actualiza la letra del cupón además de los totales
                            PURCHASE.deleteItemFromTable();
                        }

                    }
                }
                else {
                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceUnit').addClass('input-validation-error');
                }
            }
            else {
                $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Quantity').addClass('input-validation-error');
            }
        });

        $('#btnSavePurchaseService').on('click', function (e, couponFlag) {
            var _prices = new Array();

            $('#tblPrices tbody tr').each(function (index, item) {
                var _model = new ListPurchaseServiceDetailModel();
                _model.PurchaseServiceID = $('#PurchaseService_PurchaseServiceID').val();
                _model.ServiceDetailID = $(this).attr('id') != undefined ? $(this).attr('id').split('_')[1] : null;
                _model.DealPrice = $(this).children('td:nth-child(1)').children(':hidden:first').val();
                _model.Quantity = $(this).children('td:nth-child(2)').text().trim();
                _model.PriceID = $(this).children('td:nth-child(3)').children(':hidden:nth-child(2)').val();
                _model.Coupon = $(this).children('td:nth-child(7)').text().trim();
                _model.CustomPrice = $(this).children('td:nth-child(4)').text().trim().replace('$', '').replace(',', '');
                _model.PriceTypeID = $(this).children('td:nth-child(3)').children(':hidden:nth-child(1)').val();
                _model.Promo = $(this).children('td:nth-child(6)').text().trim() == 'Yes' ? true : false;
                _model.ExchangeRateID = $(this).children('td:nth-child(3)').children(':hidden:nth-child(3)').val();
                _prices[index] = _model;
            });

            var model = new PurchaseServiceModel();
            model.PurchaseService_PurchaseServiceID = $('#PurchaseService_PurchaseServiceID').val();
            model.PurchaseService_Purchase = $('#PurchaseService_Purchase').val();
            model.PurchaseService_Service = $('#PurchaseService_Service').val();
            model.PurchaseService_WholeStay = $('input:radio[name="PurchaseService_WholeStay"]:checked').val();
            model.PurchaseService_OpenCoupon = $('#PurchaseService_OpenCoupon option:selected').val();
            model.PurchaseService_ServiceDateTime = $('#PurchaseService_ServiceDateTime').val(),
                model.PurchaseService_WeeklyAvailability = $('#PurchaseService_WeeklyAvailability').val();
            model.PurchaseService_WeeklyAvailabilityString = $('#PurchaseService_WeeklyAvailability option:selected').text();
            model.PurchaseService_MeetingPoint = $('#PurchaseService_MeetingPoint').val();
            model.PurchaseService_ServiceStatus = $('#PurchaseService_ServiceStatus').val();
            model.PurchaseService_ConfirmationDateTime = $('#PurchaseService_ConfirmationDateTime').val();
            model.PurchaseService_ConfirmationNumber = $('#PurchaseService_ConfirmationNumber').val();
            model.PurchaseService_ConfirmedByUser = $('#PurchaseService_ConfirmedByUser option:selected').val();
            model.PurchaseService_Promo = $('#PurchaseService_Promo').val();
            model.PurchaseService_Total = $('#PurchaseService_Total').val();
            model.PurchaseService_Currency = $('#PurchaseService_Currency').val();
            model.PurchaseService_ReservedFor = $('#PurchaseService_ReservedFor').val();
            model.PurchaseService_ChildrenAges = $('#PurchaseService_ChildrenAges').val();
            model.PurchaseService_CustomMeetingPoint = $('#PurchaseService_CustomMeetingPoint').val();
            model.PurchaseService_CustomMeetingTime = $('#PurchaseService_CustomMeetingTime').val();
            model.PurchaseService_Note = $('#PurchaseService_Note').val();
            model.PurchaseService_Airline = $('#PurchaseService_Airline').val();
            model.PurchaseService_FlightNumber = $('#PurchaseService_FlightNumber').val();
            model.PurchaseService_TransportationZone = $('#PurchaseService_TransportationZone').val();
            model.PurchaseService_VehicleType = $('#PurchaseService_VehicleType').val();
            model.PurchaseService_SpecialRequest = $('#PurchaseService_SpecialRequest').val();
            model.PurchaseService_CancelationCharge = $('#PurchaseService_CancelationCharge').val();
            model.PurchaseService_CancelationDateTime = $('#PurchaseService_CancelationDateTime').val();
            model.PurchaseService_CancelationNumber = $('#PurchaseService_CancelationNumber').val();
            model.PurchaseService_CanceledByUser = $('#PurchaseService_CanceledByUser option:selected').val();
            model.PurchaseService_Audit = $('input:radio[name="PurchaseService_Audit"]:checked').val();
            model.PurchaseService_CouponReference = $('#PurchaseService_CouponReference').val();
            model.PurchaseService_SoldByOPC = $('input:radio[name="PurchaseService_SoldByOPC"]:checked').val();
            model.PurchaseService_CloseOut = $('input:radio[name="PurchaseService_CloseOut"]:checked').val();
            model.PurchaseService_Issued = $('input:radio[name="PurchaseService_Issued"]:checked').val();
            model.PurchaseService_Round = $('input:radio[name="PurchaseService_Round"]:checked').val();
            model.PurchaseService_RoundAirline = $('#PurchaseService_RoundAirline').val();
            model.PurchaseService_RoundFlightNumber = $('#PurchaseService_RoundFlightNumber').val();
            model.PurchaseService_RoundFlightTime = $('#PurchaseService_RoundFlightTime').val();
            model.PurchaseService_RoundDate = $('#PurchaseService_RoundDate').val();
            model.PurchaseService_RoundMeetingTime = $('#PurchaseService_RoundMeetingTime').val();
            model.PurchaseService_ReplacementOf = $('#PurchaseService_ReplacementOf').val();
            model.ListPurchaseServiceDetails = _prices;
            model.PurchaseService_CurrentUnits = $('#PurchaseService_CurrentUnits').val();
            var jsonObj = JSON.stringify(model);
            var flag = false;
            if ($('#PurchaseService_ServiceStatus option:selected').val() == 3) {
                if (_prices.length > 0 && $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() != '') {
                    flag = true;
                }
            }
            else {
                flag = true;
            }
            if (flag) {
                if ($('#frmPurchaseServiceInfo').valid()) {
                    $.ajax({
                        url: '/MasterChart/SavePurchaseService',
                        cache: false,
                        type: 'POST',
                        data: jsonObj,
                        dataType: 'json',
                        traditional: true,
                        contentType: 'application/json; charset=utf-8',
                        //beforeSend: function (xhr) {
                        //    UI.checkForPendingRequests(xhr);
                        //},
                        success: function (data) {
                            PURCHASE.savePurchaseServiceSuccess(data, couponFlag);
                        }
                    });
                }
                else {
                    UI.showValidationSummary('frmPurchaseServiceInfo');
                }
            }
        });

        $('#PurchaseService_Provider').on('change', function (e, params) {
            if ($('#PurchaseInfo_Terminal').val() != null) {
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'servicesPerTerminal', itemID: ($(this).val() != '' ? $(this).val() + '|' + $('#PurchaseInfo_Terminal').val() : $('#PurchaseInfo_Terminal').val()) }, function (data) {
                    $('#PurchaseService_Service').fillSelect(data);
                    $('#PurchaseService_Service').multiselect('refresh');
                });
            }
        });

        $('#PurchaseService_Service').on('change', function (e, params) {
            PURCHASE.indicator = undefined;
            //clear and set datetime
            $('#PurchaseService_ServiceDateTime').val('');
            $('#PurchaseService_OpenCoupon option[value="' + (params != undefined ? params.openCoupon : '0') + '"]').attr('selected', true);
            $('#PurchaseService_OpenCoupon').trigger('change', params);
            altDateFormat('PurchaseService_ServiceDateTime', 'altDateFormat', params, $(this).val());
            $('#PurchaseService_CustomMeetingTime').val('');

            //get promos per serviceDate
            //esto se dispara en el change de opencoupon
            /*GG : VOLVI A DESCOMENTAR ESTO PORQUE AL ABRIR UN CUPON NO ESTABA CARGANDO LAS PROMOS*/
            var _params = params != undefined ? params : { service: $(this).val(), datetime: $('#PurchaseService_ServiceDateTime').val() };
            PURCHASE.updatePromosPerServiceDate(_params);

            //get info of service provider
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'serviceProvider', itemID: $(this).val() }, function (data) {
                if (data.length > 0) {
                    $('#PurchaseService_Provider option[value="' + data[0].Value.split('|')[0] + '"]').attr('selected', true);
                    $('#PurchaseService_Provider').multiselect('refresh');
                    $('#serviceProviderName').text((data.length > 0 ? data[0].Value.split('|')[1] : ''));
                    $('#serviceProviderPhone').text((data.length > 0 ? data[0].Text.split('|')[0] : ''));
                    $('#PurchaseService_ProviderEmail').val((data.length > 0 ? data[0].Text.split('|')[1].length > 0 ? data[0].Text.split('|')[1] : 'Not Defined' : 'Not Defined'));
                }
            });

            //code for rules
            //if ($('#PurchaseInfo_Terminal').val() != null) {
            //    $.getJSON('/MasterChart/GetDDLData', { itemType: 'priceTypeRule', itemID: ($(this).val() + '|' + $('#PurchaseInfo_Terminal').val() + '|' + $('#PurchaseService_Promo').val() + '|' + $('#PurchaseInfo_PointOfSale option:selected').val()) }, function (data) {
            //        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').fillSelect(data);
            //        if ($('#PurchaseService_ServiceDateTime').val() != '') {
            //            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType option[value="' + UI.returnMostSelectedValue(localStorage.Eplat_Counter_PriceType, 'PriceType') + '"]').attr('selected', true).trigger('change');
            //        }
            //    });
            //}
            //clear fields related to service selected
            $('#tblPrices tbody').empty();
            $('#PurchaseService_Total').val('');
            $('#lblPurchaseService_Total').html($('#PurchaseService_Total').val());
            UI.applyFormat('currency');
            ///

            //check if service selected is transportationService
            if ($(this).val() != "0" && $(this).val() != null) {
                $.getJSON('/MasterChart/IsTransportationService', { serviceID: $(this).val() }, function (data) {
                    if (data.IsTransportation == true) {
                        $('#divTransportationInfoContainer').show();
                        if (data.OffersRoundTrip == true) {
                            $('#divIsRound').show();
                        }
                        else {
                            $('#divIsRound').hide();
                            $('#divRoundTripInfoContainer').hide();
                        }
                    }
                    else {
                        $('#divIsRound').hide();
                        $('#divTransportationInfoContainer').hide();
                        $('#divRoundTripInfoContainer').hide();
                    }
                });
            }
            else {
                $('#divIsRound').hide();
                $('#divTransportationInfoContainer').hide();
                $('#divRoundTripInfoContainer').hide();
            }

            if (params != undefined) {
                if (params.confirmedByUser != null && params.confirmedByUser != '') {
                    $('#PurchaseService_ConfirmedByUser option[value="' + params.confirmedByUser + '"]').attr('selected', true);
                }
                else {
                    $('#PurchaseService_ConfirmedByUser option[value="' + $('#uid').val() + '"]').attr('selected', true);
                }
                if (params.canceledByUser != null && params.canceledByUser != '') {
                    $('#PurchaseService_CanceledByUser option[value="' + params.canceledByUser + '"]').attr('selected', true);
                }
                else {
                    $('#PurchaseService_CanceledByUser option[value="' + $('#uid').val() + '"]').attr('selected', true);
                }
            }
            else {
                $('#PurchaseService_ConfirmedByUser option[value="' + $('#uid').val() + '"]').attr('selected', true);
                $('#PurchaseService_CanceledByUser option[value="' + $('#uid').val() + '"]').attr('selected', true);
            }

        });

        $('input:radio[name="PurchaseService_Round"]').on('change', function () {
            if ($('input:radio[name="PurchaseService_Round"]:checked').val() == 'True') {
                $('#divRoundTripInfoContainer').show();
            }
            else {
                $('#divRoundTripInfoContainer').hide();
            }
        });

        $('#PurchaseService_WeeklyAvailability').on('change', function (e, params) {
            $.ajax({
                url: '/MasterChart/GetDDLData',
                data: { itemType: 'weeklyAvailability', itemID: $(this).val() },
                cache: false,
                beforeSend: function (xhr) {
                    //if (xhr && xhr.readyState != 4) {
                    //    xhr.abort();
                    //}
                },
                success: function (data) {
                    $('#PurchaseService_MeetingPoint').fillSelect(data);
                    var _meetingPoint = params != undefined ? params.meetingPoint : UI.returnMostSelectedValue(localStorage.Eplat_Counter_MeetingPoint, 'MeetingPoint');
                    $('#PurchaseService_MeetingPoint option[value="' + _meetingPoint + '"]').attr('selected', true).trigger('change');
                }
            });
            //$.getJSON('/MasterChart/GetDDLData', { itemType: 'weeklyAvailability', itemID: $(this).val() }, function (data) {
            //    $('#PurchaseService_MeetingPoint').fillSelect(data);
            //    var _meetingPoint = params != undefined ? params.meetingPoint : UI.returnMostSelectedValue(localStorage.Eplat_Counter_MeetingPoint, 'MeetingPoint');
            //    $('#PurchaseService_MeetingPoint option[value="' + _meetingPoint + '"]').attr('selected', true).trigger('change');
            //});
        });

        $('#PurchaseService_MeetingPoint').on('change', function () {
            if ($(this).val() == 'null') {
                $('#PurchaseService_CustomMeetingPoint').parents('div.editor-alignment:first').show();
            }
            else {
                $('#PurchaseService_CustomMeetingPoint').parents('div.editor-alignment:first').hide();
            }
        });

        $('#btnGetCoupon').on('click', function () {
            var _serviceID = $('#PurchaseService_PurchaseServiceID').val();
            if ((PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).data('issued').toString().toLowerCase() != 'true' && PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).attr('data-issued').toString().toLowerCase() != 'true') || ($('#tblPurchasesServices').data('resend').toString().toLowerCase() != 'false' && $('#tblPurchasesServices').attr('data-resend').toString().toLowerCase() != 'false')) {
                $.ajax({
                    url: '/Masterchart/GetCouponRefObj',
                    cache: false,
                    type: 'POST',
                    data: { purchaseServiceID: _serviceID },
                    //beforeSend: function (xhr) {
                    //    console.log(xhr);
                    //    UI.checkForPendingRequests(xhr);
                    //},
                    success: function (data) {
                        if (data.CouponRef != '') {
                            var _link = '';
                            if ((PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).data('issued').toString().toLowerCase() != 'true' && PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).attr('data-issued').toString().toLowerCase() != 'true') || ($('#tblPurchasesServices').data('reprint').toString().toLowerCase() != 'false' && $('#tblPurchasesServices').attr('data-reprint').toString().toLowerCase() != 'false')) {
                                _link = '<a href="' + (data.CouponRef + '-' + _serviceID) + '" target="_blank">' + (data.CouponRef + '-' + _serviceID) + '</a>';
                            }



                            UI.twoActionBox('Coupon <br />' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val()
                                + '<br />'
                                + _link
                                //+ '<a href="' + (data.CouponRef + '-' + _serviceID) + '" target="_blank">' + (data.CouponRef + '-' + _serviceID) + '</a>'
                                + '<br /><div class="align-from-top align-from-bottom"><input type="checkbox" id="chkSendProvider" /> Send to Provider: ' + (data.ProviderEmail != '' ? data.ProviderEmail : 'Not Defined')
                                + '<br /><br /><input type="checkbox" id="chkSendOther" /> Send to Other: <input type="text" id="txtSendOther" /></div>', printCoupon, [(data.CouponRef + '-' + _serviceID)], 'print coupon', sendCouponsByEmail, [$('#PurchaseInfo_PurchaseID').val(), _serviceID], 'send by email');
                        }
                        else {
                            UI.messageBox(-1, 'An error ocurred trying to get the coupon(s) reference. Please contact System Administrator', -1, null);
                        }
                    }
                });
            }
            else {
                UI.messageBox(0, 'Coupon has been issued', null, null);
            }
            //    //if service status is confirmed
            //    if ($('#PurchaseService_PurchaseServiceID').val() != '' && $('#PurchaseService_PurchaseServiceID').val() != '0' && $('#PurchaseService_ServiceStatus option:selected').val() == 3) {
            //        //change Issued flag in server, if saved, get coupon
            //        $('input:radio[name="PurchaseService_Issued"]')[0].checked = true;
            //        $('#btnSavePurchaseService').trigger('click', true);
            //    }
        });

        $('.send-coupons').on('click', function () {
            sendCouponsByEmail($('#PurchaseInfo_PurchaseID').val());
        });

        //new printing function
        $('#btnPrintAllCoupons').on('click', function () {
            $.ajax({
                async: false,
                url: '/Masterchart/PrintAllCoupons',
                cache: false,
                type: 'POST',
                data: { purchaseID: $('#PurchaseInfo_PurchaseID').val() },
                success: function (data) {
                    if (data.CouponRef != '') {
                        var counter = 0;
                        var couponsArray = data.Coupons != '' ? data.Coupons.split(',') : undefined;

                        if (couponsArray != undefined) {
                            var mQuery = window.matchMedia('print');

                            mQuery.addListener(function (m) {
                                if (!m.matches) {
                                    counter++;
                                    if (counter < couponsArray.length) {
                                        printCoupon(data.CouponRef + '-' + couponsArray[counter]);
                                    }
                                    //else {
                                    //    mQuery.removeListener();
                                    //}
                                }
                            });
                            printCoupon(data.CouponRef + '-' + couponsArray[counter]);
                        }
                        else {
                            UI.messageBox(0,'No pending coupons to print', null, null);
                        }
                    }
                }
            });
        });


        $('.print-coupons').on('click', function () {
            $.ajax({
                async: false,
                url: '/Masterchart/GetCouponRefObj',
                cache: false,
                type: 'POST',
                data: { purchaseServiceID: PURCHASE.oPurchaseServiceTable.$('tr:first').attr('id').split('_')[1] },
                //beforeSend: function (xhr) {
                //    UI.checkForPendingRequests(xhr);
                //},
                success: function (data) {
                    if (data.CouponRef != '') {
                        var counter = 0;
                        var couponsArray = new Array();

                        PURCHASE.oPurchaseServiceTable.$('tr[data-issued="false"]').find('.print-coupon').each(function () {
                            couponsArray.push(data.CouponRef + '-' + $(this).parents('tr:first').attr('id').split('_')[1]);
                        });
                        if (couponsArray.length > 0) {
                            printCoupon(couponsArray[counter]);
                            if (window.matchMedia) {
                                var mediaQueryList = window.matchMedia('print');
                                mediaQueryList.addListener(function (mql) {
                                    if (!mql.matches) {//after print
                                        counter++;
                                        if (counter < couponsArray.length) {
                                            printCoupon(couponsArray[counter]);
                                        }
                                    }
                                });
                            }
                        }
                        else {
                            UI.messageBox(0, 'There are no pending coupons to print', 0, null);
                        }
                    }
                    else {
                        UI.messageBox(-1, 'An error ocurred trying to get the coupon(s) reference. Please contact System Administrator', -1, null);
                    }
                }
            });
        });

        $('#PurchaseService_Promo').on('change', function (e, params) {
            var $this = $(this).val();
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'priceTypeRule', itemID: ($('#PurchaseService_Service').val() + '|' + $('#PurchaseInfo_Terminal').val() + '|' + $(this).val() + '|' + $('#PurchaseInfo_PointOfSale option:selected').val()) }, function (data) {
                $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').fillSelect(data);
                if ($('#PurchaseService_ServiceDateTime').val() != '') {
                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType option[value="' + UI.returnMostSelectedValue(localStorage.Eplat_Counter_PriceType, 'PriceType') + '"]').attr('selected', true).trigger('change');
                }
                if ($('#PurchaseService_Promo').val() != 0) {
                    //if promo is distinct of nothing
                    $('#divApplyPromo').show();
                    $.getJSON('/MasterChart/GetPromoInfo', { promoID: $('#PurchaseService_Promo').val() }, function (data) {
                        $('#hdnGetPromo').val(JSON.stringify(data));
                        if (data.applyOnPerson == true) {
                            //if promo applies to persons
                            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Promo').unbind('click');
                        }
                        else {
                            //if promo applies to whole tours
                            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Promo').attr('checked', 'checked');
                            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Promo').unbind('click').on('click', function (e) {
                                e.preventDefault();
                            });
                            $('#tblPrices tbody tr').each(function () {
                                $(this).children('td:nth-child(6)')[0].firstChild.textContent = 'Yes';
                                var subtotal = parseFloat($(this).children('td:nth-child(2)').text()) * parseFloat($(this).children('td:nth-child(4)').text().replace('$', '').replace(',', ''));
                                var _text = data.isDiscountType ? data.amount != null ? UI.addDecimalPart(subtotal - parseFloat(data.amount)) : UI.addDecimalPart(subtotal - ((subtotal * parseFloat(data.percentage)) / 100)) : UI.addDecimalPart('0')
                                $(this).children('td:nth-child(5)').text(_text);
                            });
                            setTotalOfCouponsSold();
                        }

                        if ($('#hdnCurrentPurchaseService').val() != '' && $('#hdnCurrentPurchaseService').val() != null) {
                            //if is a coupon edition
                            if ($.parseJSON($('#hdnCurrentPurchaseService').val()).PurchaseService_Promo != 0 && $.parseJSON($('#hdnCurrentPurchaseService').val()).PurchaseService_Promo != null) {
                                //if coupon been edited has promo selected
                                if ($('#PurchaseService_ServiceStatus').val() == 3 | $('#PurchaseService_Promo').hasClass('field-disabled')) {
                                    //if coupon been edited is confirmed
                                    if ($('#PurchaseService_Promo option[value="' + $.parseJSON($('#hdnCurrentPurchaseService').val()).PurchaseService_Promo + '"]').length > 0) {
                                        //if previous promo selected exists on new promos loaded
                                        $('#PurchaseService_Promo option[value="' + $.parseJSON($('#hdnCurrentPurchaseService').val()).PurchaseService_Promo + '"]').attr('selected', true);
                                    }
                                    else {
                                        //if previous promo selected does not exist in new promos loaded
                                        function returnDate(previousDate) {
                                            $('#PurchaseService_ServiceDateTime').val(previousDate);
                                            altDateFormat('PurchaseService_ServiceDateTime', 'altDateFormat', undefined, $('#PurchaseService_Service').val());
                                        }
                                        UI.twoActionBox('Promo is not available on target date.<br />Date cannot be changed', returnDate, [$.parseJSON($('#hdnCurrentPurchaseService').val()).PurchaseService_ServiceDateTime], 'Accept', returnDate, [$.parseJSON($('#hdnCurrentPurchaseService').val()).PurchaseService_ServiceDateTime], 'Cancel');
                                    }
                                }
                                else {
                                    //if coupon been edited is not confirmed
                                    if (params == undefined) {
                                        //if change was made by user
                                        $('#tblPrices tbody tr').each(function () {
                                            if ($(this).children('td:nth-child(6)').text() == 'Yes') {
                                                $($(this).children('td:nth-child(8)')[0].childNodes[0]).click();
                                            }
                                        });
                                    }
                                }
                            }
                            //mike
                            //check if its packable promo and check if there is any coupon with same promo
                            if (data.isPackable) {
                                var promoApplied = false;
                                //var _promoApplied = '';
                                var _couponFolio = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val();
                                var a = COMMON.getDate();
                                var b = a.split('-');
                                PURCHASE.oPurchaseServiceTable.$('tr').not('theader').each(function (index, item) {
                                    //PURCHASE.oPurchaseServiceTable.$('tr').not('theader').not('.canceled-row').each(function (index, item) {
                                    var _json = $.parseJSON($(this).children('td:nth-child(7)').children(':hidden').val());
                                    var c = '';
                                    $.each(b, function (i, x) {
                                        c += (c == '' ? '' : '-') + UI.padNumber(x, 2);
                                    });
                                    if (_json.promoID != 0 && _json.isPackable && !$(this).hasClass('canceled-row') && _json.date == c) {
                                        promoApplied = true;
                                        if (_json.promoID == $('#PurchaseService_Promo option:selected').val()) {
                                            promoApplied = true;
                                            _couponFolio = $(this).children('td:nth-child(3)').text().trim();
                                        }
                                    }
                                    ////
                                    //if (_json.promoID != 0 && _json.promoID == $('#PurchaseService_Promo option:selected').val() && _json.isPackable && !$(this).hasClass('canceled-row') && _json.date == c) {
                                    //    promoApplied = true;
                                    //    _couponFolio = $(this).children('td:nth-child(3)').text().trim();
                                    //    return false;
                                    //}
                                });
                                if (!promoApplied) {
                                    //to get next coupon folio again
                                    $.ajax({
                                        url: '/MasterChart/GetNextCouponFolio',
                                        type: 'POST',
                                        cache: false,
                                        data: { purchaseID: $('#PurchaseInfo_PurchaseID').val(), timeSpan: new Date().getTime() },
                                        //beforeSend: function (xhr) {
                                        //    UI.checkForPendingRequests(xhr);
                                        //},
                                        success: function (data) {
                                            //si la peticion regresa información se asigna al campo, de lo contrario se arroja mensaje diciendo que no existen cupones vigentes en la locación
                                            if (data.folio != '-1') {
                                                if (data.folio != '0') {
                                                    if (data.folio == 'null') {
                                                        UI.messageBox(0, 'No folios assigned to: ' + data.exception
                                                            + '<br />Please be sure that purchase was created on the correct point of sale and save any change', null, null);
                                                    }
                                                    else {
                                                        $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(data.folio.split(',')[0] + padNumber(data.folio.split(',')[1], data.padding));
                                                        //if ($('#tblPrices tbody tr').length > 0 && $('#tblPrices tbody tr:first').children('td:nth-child(6)')[0].textContent == '') {
                                                        if ($('#tblPrices tbody tr').length > 0 && $('#tblPrices tbody tr:first').children('td:nth-child(7)')[0].textContent == '') {
                                                            $('#tblPrices tbody tr').each(function (index) {
                                                                //$(this).children('td:nth-child(6)').text($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '-' + String.fromCharCode(65 + index));
                                                                $(this).children('td:nth-child(7)').text($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '-' + String.fromCharCode(65 + index));
                                                            });
                                                        }
                                                        if ($('#PurchaseService_Promo option:selected').val() != 0 && $.parseJSON($('#hdnGetPromo').val()).isPackable) {
                                                            var promoApplied = false;
                                                            var _couponFolio = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val();
                                                            PURCHASE.oPurchaseServiceTable.$('tr').not('theader').each(function (index, item) {
                                                                var _json = $.parseJSON($(this).children('td:nth-child(7)').children(':hidden').val());
                                                                //if (_json.promoID != 0 && _json.promoID == $('#PurchaseService_Promo option:selected').val() && _json.isPackable && !$(this).hasClass('canceled-row')) {
                                                                if ((_json.promoID != 0 && _json.promoID == $('#PurchaseService_Promo option:selected').val() && _json.isPackable && !$(this).hasClass('canceled-row')) || $(this).hasClass('selected-row')) {
                                                                    promoApplied = true;
                                                                    _couponFolio = $(this).children('td:nth-child(3)').text().trim();
                                                                    return false;
                                                                }
                                                            });
                                                            //overwrite couponFolio used for the coupon in field and table
                                                            if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() != '' || $('#PurchaseService_ServiceStatus option:selected').val() == 3) {
                                                                $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(_couponFolio);
                                                                $('#tblPrices tbody tr').each(function (index) {
                                                                    //$(this).children('td:nth-child(6)').text(_couponFolio + '-' + String.fromCharCode(65 + index));
                                                                    $(this).children('td:nth-child(7)').text(_couponFolio + '-' + String.fromCharCode(65 + index));
                                                                });
                                                            }
                                                        }
                                                    }
                                                }
                                                else {
                                                    UI.messageBox(-1, 'There is an error with the folios assignment.<br />Please contact your System Administrator', null, null);
                                                }
                                            }
                                            else {
                                                UI.messageBox(-1, 'Could not get coupon folio: <br />' + data.exception, null, null);
                                                //UI.messageBox(0, 'No folios assigned to: ' + $('#PurchaseInfo_PointOfSale option:selected').text(), null, null);
                                            }
                                        }
                                    });
                                }
                                //overwrite couponFolio used for the coupon in field and table
                                if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() != '') {
                                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(_couponFolio);
                                    $('#tblPrices tbody tr').each(function (index) {
                                        $(this).children('td:nth-child(7)').text(_couponFolio + '-' + String.fromCharCode(65 + index));
                                    });
                                }
                            }
                        }
                        else {
                            //if is new coupon
                            $.getJSON('/MasterChart/CanApplyPromo', { PurchaseService_Purchase: $('#PurchaseInfo_PurchaseID').val() }, function (data2) {
                                if (data2.promosApplied == true) {
                                    //if there are other coupons with promos applied
                                    if (data2.canApplyPromo == true) {
                                        //if current promo could be applied to coupon
                                        $('#tblPrices tbody tr').each(function () {
                                            //if ($(this).children('td:nth-child(7)').text() == 'Yes') {
                                            if ($(this).children('td:nth-child(6)').text() == 'Yes') {
                                                //$($(this).children('td:nth-child(7)')[0].childNodes[1]).click();
                                                $($(this).children('td:nth-child(8)')[0].childNodes[0]).click();
                                            }
                                        });
                                        //check if its packable promo and check if there is any coupon with same promo
                                        if (data.isPackable) {
                                            var promoApplied = false;
                                            var _couponFolio = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val();
                                            PURCHASE.oPurchaseServiceTable.$('tr').not('theader').each(function (index, item) {
                                                var _json = $.parseJSON($(this).children('td:nth-child(7)').children(':hidden').val());
                                                var a = COMMON.getDate();
                                                var b = a.split('-');
                                                var c = '';
                                                $.each(b, function (i, x) {
                                                    c += (c == '' ? '' : '-') + UI.padNumber(x, 2);
                                                });
                                                if (_json.promoID != 0 && _json.isPackable && !$(this).hasClass('canceled-row') && _json.date == c) {
                                                    //promoApplied = true;
                                                    if (_json.promoID == $('#PurchaseService_Promo option:selected').val()) {
                                                        promoApplied = true;
                                                        _couponFolio = $(this).children('td:nth-child(3)').text().trim();
                                                    }
                                                }
                                                //if (_json.promoID != 0 && _json.promoID == $('#PurchaseService_Promo option:selected').val() && _json.isPackable && !$(this).hasClass('canceled-row') && _json.date == c) {
                                                //    promoApplied = true;
                                                //    _couponFolio = $(this).children('td:nth-child(3)').text().trim();
                                                //    return false;
                                                //}
                                            });
                                            if (!promoApplied) {
                                                //to get next coupon folio again
                                                $.ajax({
                                                    url: '/MasterChart/GetNextCouponFolio',
                                                    type: 'POST',
                                                    cache: false,
                                                    data: { purchaseID: $('#PurchaseInfo_PurchaseID').val(), timeSpan: new Date().getTime() },
                                                    //beforeSend: function (xhr) {
                                                    //    UI.checkForPendingRequests(xhr);
                                                    //},
                                                    success: function (data) {
                                                        //si la peticion regresa información se asigna al campo, de lo contrario se arroja mensaje diciendo que no existen cupones vigentes en la locación
                                                        if (data.folio != '-1') {
                                                            if (data.folio != '0') {
                                                                if (data.folio == 'null') {
                                                                    UI.messageBox(0, 'No folios assigned to: ' + data.exception
                                                                        + '<br />Please be sure that purchase was created on the correct point of sale and save any change', null, null);
                                                                }
                                                                else {
                                                                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(data.folio.split(',')[0] + padNumber(data.folio.split(',')[1], data.padding));
                                                                    if ($('#tblPrices tbody tr').length > 0 && $('#tblPrices tbody tr:first').children('td:nth-child(7)')[0].textContent == '') {
                                                                        $('#tblPrices tbody tr').each(function (index) {
                                                                            $(this).children('td:nth-child(7)').text($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '-' + String.fromCharCode(65 + index));
                                                                        });
                                                                    }
                                                                    if ($('#PurchaseService_Promo option:selected').val() != 0 && $.parseJSON($('#hdnGetPromo').val()).isPackable) {
                                                                        var promoApplied = false;
                                                                        var _couponFolio = $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val();
                                                                        PURCHASE.oPurchaseServiceTable.$('tr').not('theader').each(function (index, item) {
                                                                            var _json = $.parseJSON($(this).children('td:nth-child(7)').children(':hidden').val());
                                                                            var a = COMMON.getDate();
                                                                            var b = a.split('-');
                                                                            var c = '';
                                                                            $.each(b, function (i, x) {
                                                                                c += (c == '' ? '' : '-') + UI.padNumber(x, 2);
                                                                            });
                                                                            if (_json.promoID != 0 && _json.promoID == $('#PurchaseService_Promo option:selected').val() && _json.isPackable && !$(this).hasClass('canceled-row') && _json.date == c) {
                                                                                promoApplied = true;
                                                                                _couponFolio = $(this).children('td:nth-child(3)').text().trim();
                                                                                return false;
                                                                            }
                                                                        });
                                                                        //overwrite couponFolio used for the coupon in field and table
                                                                        if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() != '' || $('#PurchaseService_ServiceStatus option:selected').val() == 3) {
                                                                            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(_couponFolio);
                                                                            $('#tblPrices tbody tr').each(function (index) {
                                                                                //$(this).children('td:nth-child(6)').text(_couponFolio + '-' + String.fromCharCode(65 + index));
                                                                                $(this).children('td:nth-child(7)').text(_couponFolio + '-' + String.fromCharCode(65 + index));
                                                                            });
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else {
                                                                UI.messageBox(-1, 'There is an error with the folios assignment.<br />Please contact your System Administrator', null, null);
                                                            }
                                                        }
                                                        else {
                                                            UI.messageBox(-1, 'Could not get coupon folio: <br />' + data.exception, null, null);
                                                            //UI.messageBox(0, 'No folios assigned to: ' + $('#PurchaseInfo_PointOfSale option:selected').text(), null, null);
                                                        }
                                                    }
                                                });
                                            }
                                            //overwrite couponFolio used for the coupon in field and table
                                            if ($('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() != '') {
                                                $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(_couponFolio);
                                                $('#tblPrices tbody tr').each(function (index) {
                                                    //$(this).children('td:nth-child(6)').text(_couponFolio + '-' + String.fromCharCode(65 + index));
                                                    $(this).children('td:nth-child(7)').text(_couponFolio + '-' + String.fromCharCode(65 + index));
                                                });
                                            }
                                        }
                                    }
                                    else {
                                        //if current promo could not be applied
                                        UI.messageBox(-1, 'Promo could not be applied because another coupon(s) already have promos applied', null, null);
                                    }
                                }
                            });
                        }
                    });
                }
                else {
                    //if promo is nothing
                    $('#divApplyPromo').hide();
                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Promo').unbind('click');
                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Promo').removeAttr('checked');
                    $('#tblPrices tbody tr').each(function () {
                        if ($(this).children('td:nth-child(6)').text() == 'Yes') {
                            $($(this).children('td:nth-child(8)')[0].childNodes[0]).click();
                        }
                    });
                }
                UI.applyFormat('currency', 'tblPrices');
            });
        });

        $('#PurchaseService_Destination').on('change', function (e, params) {
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'destination', itemID: $(this).val() }, function (data) {
                $('#PurchaseService_TransportationZone').fillSelect(data, false);
                var _transportationZone = params != undefined ? params.transportationZone : 0;
                $('#PurchaseService_TransportationZone option[value="' + _transportationZone + '"]').attr('selected', true);
                $('#PurchaseService_TransportationZone').trigger('change');
            });
        });

        $('#PurchaseService_OpenCoupon').on('change', function (e, params) {
            if ($(this).val() != '0') {
                $('#PurchaseService_ServiceDateTime').focusin();
                $('#PurchaseService_ServiceDateTime').on('mousedown', function (e) {
                    e.preventDefault();
                });
            }
            else {
                if ($('#PurchaseService_ServiceStatus option:selected').val() == 3 && $('#PurchaseService_PurchaseServiceID').val() != '') {
                    $('#PurchaseService_ServiceDateTime').on('mousedown', function (e) {
                        e.preventDefault();
                    });
                }
                else {
                    $('#PurchaseService_ServiceDateTime').unbind('mousedown');
                    $('#PurchaseService_ServiceDateTime').datepicker('destroy');
                    $('#PurchaseService_ServiceDateTime').datepicker({
                        'changeMonth': true,
                        'changeYear': true,
                        'dateFormat': 'yy-mm-dd',
                        constrainInput: true,
                        beforeShowDay: PURCHASE.closedDays,
                        onClose: function (dateText, inst) {
                            onCloseServiceDate(dateText);
                        }
                    });
                }
                //$('#PurchaseService_ServiceDateTime').unbind('mousedown');
            }
            if (params != undefined) {
                $('#PurchaseService_ServiceDateTime').val(params.datetime);
            }
            else {
                if ($('#PurchaseService_Service').val() != '0') {
                    $('#PurchaseService_ServiceDateTime').datepicker('setDate', new Date());
                    onCloseServiceDate($('#PurchaseService_ServiceDateTime').val());
                }
            }
        });
        //--------------------------------------------------------------

        $('#PurchasePayment_DateSaved').datepicker({
            'changeMonth': true,
            'changeYear': true,
            'dateFormat': 'yy-mm-dd',
            maxDate: 0,
            constrainInput: true,
            onClose: function (dateText, inst) {
                $('#PurchasePayment_CardType').trigger('change');
            }
        }).datepicker('setDate', new Date(COMMON.serverDateTime)).on('keydown', function (e) { e.preventDefault(); });

        $('#PurchasePayment_Amount').numeric({
            negative: false,
            decimalPlaces: 2
        });

        $('#PurchasePayment_ReferenceNumber').numeric({
            negative: false
        });

        $('#btnNewPurchasePaymentInfo').on('click', function (e) {
            $('#PurchasePayment_TransactionType option[value="1"]').attr('selected', true).trigger('change');
            var due = parseFloat($('#spanDue').find('.money-amount').text().split(' ')[0].replace(',', ''));
            var placeholder = Number(due).toFixed(2) * (Number(due).toFixed(2) < 0 ? -1 : 1);
            $('#PurchasePayment_Amount').attr('placeholder', placeholder);
            $('input[name="PurchasePayment_ApplyCommission"]')[0].checked = true;
        });

        $('#btnNewRefund').on('click', function () {
            $('#btnNewPurchasePaymentInfo').click();
            $('#PurchasePayment_TransactionType option[value="2"]').attr('selected', true).trigger('change');
        });

        $('input:radio[name="PurchasePayment_ApplyCharge"]').on('change', function () {
            //if ($('input:radio[name="PurchasePayment_ApplyCharge"]:checked').val() == 'True') {
            if ($('input:radio[name="PurchasePayment_ApplyCharge"]:checked').val() == 'True') {
                if ($('#PurchasePayment_TransactionType').val() == '2') {
                    $('#divApplyChargeDependent').show();
                }
                else {
                    $('#PurchasePayment_TransactionCode').val('');
                    $('#divApplyChargeDependent').hide();
                }
            }
            else {
                $('#divApplyChargeDependent').show();
            }
        });

        $('#PurchasePayment_BillingInfo').on('change', function () {
            if ($(this).val() == -1) {//new billing info
                UI.collapseFieldset('fdsPurchasesManagement');
                $('#btnNewBillingInfo').click();
            }
            else if ($(this).val() == -2) {//add reference number
                $('#divBillingInfo').hide();
                $('#divReferenceNumber').show();
                $('input[name="PurchasePayment_ApplyCharge"]')[1].checked = true;
                $('input[name="PurchasePayment_ApplyCharge"]').trigger('change');
                //$('#PurchasePayment_CardType').trigger('change');
            }
            else {//billing info
                $('#divBillingInfo').show();
                $('#divReferenceNumber').hide();
            }
        });

        $('#btnSaveAndReturnBillingInfo').on('click', function () {
            PURCHASE.saveAndReturnFlag = true;
            $('#frmBillingInfo').submit();
        });

        $('#PurchasePayment_TransactionType').on('change', function () {
            $('#PurchasePayment_TransactionType').removeClass('field-disabled');
            $('#PurchasePayment_TransactionType').unbind('mousedown').on('mousedown', function () {
                return true;
            });
            //$('input:radio[name="PurchasePayment_ApplyCommission"]')[0].checked = true;
            //$('input:radio[name="PurchasePayment_ApplyCommission"]').trigger('change');
            $('#PurchasePayment_DateSaved').datepicker('setDate', new Date(COMMON.serverDateTime));
        });

        $('#PurchasePayment_PaymentType').on('change', function (e, params) {
            $('.credit-card-dependent').hide();
            $('#divPaymentTypeDependent').hide();
            $('.chargeback-dependent').hide();
            $('.bank-commission-dependent').hide();
            $('.certificate-dependent').hide();
            $('#divRefundAccountContainer').hide();
            $('#PurchasePayment_BillingInfo option[value="0"]').attr('selected', true).trigger('change');

            var _due = $('#spanDue').find('.money-amount').text().split(' ')[0].replace(',', '');
            $('#PurchasePayment_Amount').attr('placeholder', (parseFloat(_due) < 0 ? parseFloat(_due) * -1 : parseFloat(_due)));
            switch ($(this).val()) {
                case '3': {
                    $.getJSON('/TimeShare/GetDDLData', { itemType: 'conceptsPerTerminal', itemID: $('#PurchaseInfo_Terminal option:selected').val() + '|' + $(this).val() }, function (data) {
                        $('#PurchasePayment_ChargeBackConcept').fillSelect(data, false);
                        if (params != undefined) {
                            //$('#PurchasePayment_ChargeBackConcept option[value="' + params.chargeBackConcept + '"]').attr('selected', true).trigger('change', params);
                            $('#PurchasePayment_ChargeBackConcept option[value="' + params.chargeBackConcept + '"]').attr('selected', true);
                        }
                        $('#PurchasePayment_ChargeBackConcept').trigger('change', params);
                    });
                    $('#divPaymentTypeDependent').show();
                    $('.chargeback-dependent').show();
                    break;
                }
                case '2': {
                    $('.credit-card-dependent').show();
                    if (params != undefined) {
                        //$('.credit-card-dependent').has($('#divApplyCharge')).hide();
                        $('#PurchasePayment_CardType option[value="' + params.cardType + '"]').attr('selected', true);
                    }
                    $('#PurchasePayment_CardType').trigger('change', params);
                    break;
                }
                case '4': {
                    $('#divPaymentTypeDependent > div').hide();
                    $('#divPaymentTypeDependent, #divPaymentTypeDependent > div.allow-if-company:has(#PurchasePayment_Company)').show();
                    $('.certificate-dependent').hide();
                    $.getJSON('/MasterChart/GetDDLData', { itemType: 'opc', itemID: 'null' + '|' + ($('#PurchaseInfo_Terminal option:selected').val() != undefined ? $('#PurchaseInfo_Terminal option:selected').val() : 0) + '|' + 'false' + '|5' }, function (data) {
                        $('#PurchasePayment_Company').fillSelect(data, false);
                        if (params != undefined) {
                            $('#PurchasePayment_Company option[value="' + params.company + '"]').attr('selected', true);
                        }
                    });
                    break;
                }
                case '5': {
                    if ($('#PurchasePayment_TransactionType option:selected').text() == 'Refund') {
                        $('#divRefundAccountContainer').show();
                    }
                    //$.getJSON('/MasterChart/GetBankCommission', { terminalID: $('#PurchaseInfo_Terminal').val(), date: ($('#PurchasePayment_DateSaved').val() != undefined ? $('#PurchasePayment_DateSaved').val() : COMMON.getDate()) }, function (data) {
                    //    $('#PurchasePayment_CardCommission').val(data.commission);
                    //    if (data.commission > 0) {
                    //        if (params != undefined && params.applyCommission == true) {
                    //            $('input:radio[name="PurchasePayment_ApplyCommission"]')[0].checked = true;//testing
                    //        }
                    //        else {
                    //            $('input:radio[name="PurchasePayment_ApplyCommission"]')[1].checked = true;//testing
                    //        }
                    //        $('.bank-commission-dependent').show();
                    //    }
                    //    else {
                    //        $('.bank-commission-dependent').hide();
                    //    }
                    //});
                    break;
                }
                case '6': {
                    $('.certificate-dependent').show();
                    break;
                }
                case '1': {
                    if ($('#PurchasePayment_TransactionType option:selected').text() == 'Refund') {
                        $('#divRefundAccountContainer').show();
                    }
                    break;
                }

            }
        });

        $('#PurchasePayment_CardType').on('change', function (e, params) {
            var $this = $(this);
            if ($('#PurchasePayment_PaymentType option:selected').val() == 2) {
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'acceptCharges', itemID: $('#PurchasePayment_Purchase').val() }, function (data) {
                    $.getJSON('/MasterChart/GetBankCommission', { terminalID: $('#PurchaseInfo_Terminal').val(), date: ($('#PurchasePayment_DateSaved').val() != undefined ? $('#PurchasePayment_DateSaved').val() : COMMON.getDate()), cardTypeID: $this.val() }, function (_data) {
                        $('#PurchasePayment_CardCommission').val(_data.commission);
                        document.getElementById("btnAddCommission").value = 'add ' + _data.commission + ' %';
                        if (_data.commission > 0) {
                            $('.bank-commission-dependent').show();
                        }
                        else {
                            $('.bank-commission-dependent').hide();
                        }
                        if (params != undefined) {
                            if (params.applyCommission) {
                                $('input:radio[name="PurchasePayment_ApplyCommission"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="PurchasePayment_ApplyCommission"]')[1].checked = true;
                            }
                            //changeApplyCommission(params.applyCommission);
                        }
                        else if (data[0].Value.split('|')[1].toLowerCase() == 'true') {
                            //it is an online PoS
                            //remain hidden bank-commission-dependent fields and select "No"
                            $('input:radio[name="PurchasePayment_ApplyCommission"]')[1].checked = true;
                            //changeApplyCommission()
                        }
                        else {
                            //    //it is not an online PoS, check if it has commission different from zero
                            //    if ($('#PurchasePayment_CardCommission').val() > 0) {
                            //        //it has commission, so it needs to be visible and has "Yes" preselected
                            //        $('input:radio[name="PurchasePayment_ApplyCommission"]')[0].checked = true;
                            //    }
                            //    else {
                            //        //it does not charge commission
                            //$('input:radio[name="PurchasePayment_ApplyCommission"]')[1].checked = true;
                            //    }
                            //    //changeApplyCommission()
                        }
                    });
                });
            }
        });

        $('#_PurchasePayment_CardType').on('change', function (e, params) {
            if ($('#PurchasePayment_PaymentType option:selected').val() == 2) {
                $.getJSON('/MasterChart/GetBankCommission', { terminalID: $('#PurchaseInfo_Terminal').val(), date: ($('#PurchasePayment_DateSaved').val() != undefined ? $('#PurchasePayment_DateSaved').val() : COMMON.getDate()), cardTypeID: $(this).val() }, function (data) {
                    $('#PurchasePayment_CardCommission').val(data.commission);
                    document.getElementById("btnAddCommission").value = 'add ' + data.commission + ' %';
                    if (data.commission > 0) {
                        $('.bank-commission-dependent').show();
                    }
                    else {
                        $('.bank-commission-dependent').hide();
                    }
                    $.getJSON('/MasterChart/GetDDLData', { itemType: 'acceptCharges', itemID: $('#PurchasePayment_Purchase').val() }, function (data) {
                        if (params != undefined) {
                            if (params.applyCommission) {
                                $('input:radio[name="PurchasePayment_ApplyCommission"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="PurchasePayment_ApplyCommission"]')[1].checked = true;
                            }
                            changeApplyCommission(params.applyCommission);
                        }
                        else if (data[0].Value.split('|')[1].toLowerCase() == 'true') {
                            //it is an online PoS
                            //remain hidden bank-commission-dependent fields and select "No"
                            $('input:radio[name="PurchasePayment_ApplyCommission"]')[1].checked = true;
                            changeApplyCommission()
                        }
                        else {
                            //it is not an online PoS, check if it has commission different from zero
                            if ($('#PurchasePayment_CardCommission').val() > 0) {
                                //it has commission, so it needs to be visible and has "Yes" preselected
                                $('input:radio[name="PurchasePayment_ApplyCommission"]')[0].checked = true;
                            }
                            else {
                                //it does not charge commission
                                $('input:radio[name="PurchasePayment_ApplyCommission"]')[1].checked = true;
                            }
                            changeApplyCommission()
                        }
                        //$('input:radio[name="PurchasePayment_ApplyCommission"]').trigger('change');
                        //changeApplyCommission()
                    });
                });
            }
        });

        function _changeApplyCommission(applyCommission) {

            var commission = parseFloat($('#PurchasePayment_CardCommission').val());

            var amount = $('#PurchasePayment_Amount').val();

            document.getElementById("btnAddCommission").value = 'add ' + commission + ' %';


            if (amount != '') {
                amount = parseFloat(amount);
                if ($('#PurchasePayment_PaymentDetailsID').val() != 0 && $('#PurchasePayment_PaymentDetailsID').val() != '') {
                    if ($('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val().toLowerCase() == 'true') {
                        //if (applyCommission == false) {
                        //    console.log('a');
                        //    $('#PurchasePayment_Amount').val(Number(amount / (1 + (commission / 100))).toFixed(2));
                        //}
                        //else if (applyCommission == true) {
                        //    console.log('b');
                        //    $('#PurchasePayment_Amount').val(PURCHASE.oPurchasePaymentTable.$('tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0]);
                        //}
                        //else {
                        //    console.log('f');
                        //    $('#PurchasePayment_Amount').val(Number(amount * (1 + (commission / 100))).toFixed(2));
                        //}
                    }
                    else {
                        $('#spanCommissionPercentage').text('0.00 %');
                        //if (applyCommission == false) {
                        //    $('#PurchasePayment_Amount').val(PURCHASE.oPurchasePaymentTable.$('tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0]);
                        //}
                        //else if (applyCommission == true) {
                        //    $('#PurchasePayment_Amount').val(Number(amount / (1 + (commission / 100))).toFixed(2));
                        //}
                        //else {
                        //    $('#PurchasePayment_Amount').val(Number(amount / (1 + (commission / 100))).toFixed(2));
                        //}
                    }
                }
                else {
                    if ($('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val().toLowerCase() == 'true') {
                        //$('#PurchasePayment_Amount').val(Number(amount * (1 + (commission / 100))).toFixed(2));
                        $('#btnAddCommission').trigger('click');
                    }
                    else {
                        $('#spanCommissionPercentage').text('0.00 %');
                        //$('#PurchasePayment_Amount').val(Number(amount / (1 + (commission / 100))).toFixed(2));
                        $('#btnAddCommission').trigger('click');
                    }
                }
            }
        }

        function _changeApplyCommission(applyCommission) {
            var due = parseFloat($('#spanDue').find('.money-amount').text().split(' ')[0].replace(',', ''));
            var commission = parseFloat($('#PurchasePayment_CardCommission').val());
            var placeholder = Number(due + (due * (commission / 100))).toFixed(2) * (Number(due + (due * (commission / 100))).toFixed(2) < 0 ? -1 : 1);
            var amount = $('#PurchasePayment_Amount').val();
            $('#spanCommissionPercentage').text(commission + ' %');
            $('#PurchasePayment_Amount').attr('placeholder', placeholder);

            if (amount != '') {
                amount = parseFloat(amount);
                if ($('#PurchasePayment_PaymentDetailsID').val() != 0 && $('#PurchasePayment_PaymentDetailsID').val() != '') {
                    if ($('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val().toLowerCase() == 'true') {
                        if (applyCommission == false) {
                            $('#PurchasePayment_Amount').val(Number(amount / (1 + (commission / 100))).toFixed(2));
                        }
                        else if (applyCommission == true) {
                            $('#PurchasePayment_Amount').val(PURCHASE.oPurchasePaymentTable.$('tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0]);
                        }
                        else {
                            $('#PurchasePayment_Amount').val(Number(amount * (1 + (commission / 100))).toFixed(2));
                        }
                    }
                    else {
                        $('#spanCommissionPercentage').text('0.00 %');
                        if (applyCommission == false) {
                            $('#PurchasePayment_Amount').val(PURCHASE.oPurchasePaymentTable.$('tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0]);
                        }
                        else if (applyCommission == true) {
                            $('#PurchasePayment_Amount').val(Number(amount / (1 + (commission / 100))).toFixed(2));
                        }
                        else {
                            $('#PurchasePayment_Amount').val(Number(amount / (1 + (commission / 100))).toFixed(2));
                        }
                        //$('#PurchasePayment_Amount').val(Number(amount / (1 + (commission / 100))).toFixed(2));
                    }
                }
                else {
                    if ($('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val().toLowerCase() == 'true') {
                        $('#PurchasePayment_Amount').val(Number(amount * (1 + (commission / 100))).toFixed(2));
                    }
                    else {
                        $('#spanCommissionPercentage').text('0.00 %');
                        $('#PurchasePayment_Amount').val(Number(amount / (1 + (commission / 100))).toFixed(2));
                    }
                }
            }
        }

        //$('input:radio[name="PurchasePayment_ApplyCommission"]').on('change', changeApplyCommission);

        $('#btnAddCommission').on('click', function (e) {
            if ($('#PurchasePayment_CardCommission').val() != '') {
                if (!isNaN($('#PurchasePayment_Amount').val()) && $('#PurchasePayment_Amount').val() != '0') {
                    var commission = parseFloat($('#PurchasePayment_CardCommission').val());
                    var amount = $('#PurchasePayment_Amount').val() != '' && !isNaN($('#PurchasePayment_Amount').val()) ? parseFloat($('#PurchasePayment_Amount').val()) : parseFloat($('#PurchasePayment_Amount').attr('placeholder'));
                    amount += (amount * commission / 100);
                    $('#PurchasePayment_Amount').val(amount.toFixed(2));
                    $('input:radio[name="PurchasePayment_ApplyCommission"]')[0].checked = true;
                }
            }
        });

        $('#btnSubstractCommission').on('click', function (e) {
            if ($('#spanCommissionPercentage').text() != '0.00 %') {
                if ($('#PurchasePayment_Amount').val() != '' && $('#PurchasePayment_Amount').val() != '0') {
                    var commission = parseFloat($('#spanCommissionPercentage').text().split(' ')[0]);
                    var amount = parseFloat($('#PurchasePayment_Amount').val());
                    amount = (amount / ((commission / 100) + 1));
                    $('#PurchasePayment_Amount').val(amount.toFixed(2));
                }
            }
        });

        $('#PurchasePayment_ChargeBackConcept').on('change', function (e, params) {
            if ($('#PurchasePayment_ChargeBackConcept option:selected').text().indexOf('Company') > 0) {
                //$('#PurchasePayment_OPC option[value=""]').attr('selected', true).trigger('change', params);
                $('#divPaymentTypeDependent > div:not(.allow-if-company)').hide();
                $('#divPaymentTypeDependent > div.allow-if-company').show();
            }
            else {
                $('#divPaymentTypeDependent>div').show();
            }
            var opcID = params != undefined ? params.opc : '';
            $('#PurchasePayment_OPC option[value="' + opcID + '"]').attr('selected', true).trigger('change', params);
        });

        $('#PurchasePayment_OPC').on('change', function (e, params) {
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'opcTeam', itemID: $(this).val() }, function (data) {
                $('#PurchasePayment_PromotionTeam').fillSelect(data, false);
                var teamID = params != undefined ? params.promotionTeam : '';
                //$('#PurchasePayment_PromotionTeam').trigger('change', params);
                if (teamID != '') {
                    $('#PurchasePayment_PromotionTeam option[value=' + teamID + ']').attr('selected', true);
                }
                $('#PurchasePayment_PromotionTeam').trigger('change', params);
            });
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'opcPhone', itemID: $(this).val() }, function (data) {
                $('#PurchasePayment_OPCNumber').val(data[0].Text);
            });
            if ($(this).val() == 'null') {
                $('#divOtherOPC').show();
            }
            else {
                $('#divOtherOPC').hide();
            }
            $(this).multiselect('refresh');
        });

        $('#PurchasePayment_PromotionTeam').on('change', function (e, params) {
            $('#divUseExtension').hide();
            $('input:radio[name="PurchasePayment_IsVoucher"]')[1].checked = true;
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'teamWithBudget', itemID: $(this).val() }, function (data) {
                $('#hdnPromotionTeamInfo').val('');
                if (data.length > 0) {
                    $('#hdnPromotionTeamInfo').val(JSON.stringify(data));
                }
                var item = params != undefined ? params.company : data.length > 0 ? data[0].Text : $('#PurchasePayment_ChargeBackConcept option:selected').text().indexOf('Company') > 0 ? UI.returnMostSelectedValue(localStorage.Eplat_Counter_CXCCompany, 'CXCCompany') : '';
                if (item != '') {
                    $('#PurchasePayment_Company option[value="' + item + '"]').attr('selected', true);
                }
            });
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'budgetsPerTeam', itemID: $(this).val() }, function (data) {
                $('#PurchasePayment_Budget').fillSelect(data, false);
                $('#PurchasePayment_Budget option').each(function () {
                    $(this).show();
                    if ($(this).val().indexOf('Extension') != -1) {
                        $('#divUseExtension').show();
                    }
                });
                //new code
                if ($('#PurchasePayment_ChargeBackConcept option:selected').text().toLowerCase().indexOf('cxc') == -1) {
                    $('#PurchasePayment_Budget option').filter(function () { return $(this).val() != '0' }).hide();
                }
                //
                if (params != undefined) {
                    if (params.budget != undefined) {
                        $('#PurchasePayment_Budget option[value="' + params.budget + '"]').attr('selected', true);
                    }
                }
                else {
                    $('#PurchasePayment_Budget option').each(function () {
                        if ($(this).val().indexOf('Extension') != -1) {
                            $(this).hide();
                        }
                    });
                }
                $('#PurchasePayment_Budget').trigger('change', params);
            });
        });

        $('#PurchasePayment_Budget').on('change', function (e, params) {
            var _budget = $(this).val();
            if ($(this).val() != '0' && $('#PurchasePayment_Budget option[value*="Extension"]').length > 0) {
                $('#divUseExtension').show();
            }
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'opc', itemID: $('#PurchasePayment_OPC option:selected').val() + '|' + ($('#PurchaseInfo_Terminal option:selected').val() != undefined ? $('#PurchaseInfo_Terminal option:selected').val() : 0) + '|' + $('#PurchasePayment_ChargeBackConcept option:selected').text() }, function (data) {
                $('#PurchasePayment_Company').fillSelect(data, false);
                var _flags = $.parseJSON($('#PurchaseInfo_FlagsByTerminalPurchase').val());

                $.each(data, function (index, item) {
                    if (_flags != null && _flags.invoiceChargeBackToMarketingCompany != undefined && _flags.invoiceChargeBackToMarketingCompany == false) {
                        //invoice chargebacks to 
                        if ($('#PurchasePayment_ChargeBackConcept option:selected').text().toLowerCase().indexOf('company') > -1) {
                            //CXC Company, show subclient companies
                            if (item.Value.split('|')[1] == '4') {
                                $('#PurchasePayment_Company option[value="' + item.Value + '"]').addClass('option-visible').removeClass('option-hidden');
                            }
                            else {
                                $('#PurchasePayment_Company option[value="' + item.Value + '"]').addClass('option-hidden').removeClass('option-visible');
                            }
                        }
                        else {
                            //CXC OPC
                            if ($('#PurchasePayment_OPC option:selected').val() != '') {
                                if (_budget != "0") {
                                    //hide payingCompanies if opc and budget are selected
                                    $('#PurchasePayment_Company option[value=""]').addClass('option-hidden').removeClass('option-visible');
                                    if (item.Value.split('|')[1] == '3') {
                                        $('#PurchasePayment_Company option[value="' + item.Value + '"]').addClass('option-hidden').removeClass('option-visible');
                                    }
                                    else {
                                        $('#PurchasePayment_Company option[value="' + item.Value + '"]').addClass('option-visible').removeClass('option-hidden');
                                    }
                                }
                                else {
                                    //show payingCompanies and select one option if opc not registered
                                    if ($('#PurchasePayment_OPC option:selected').val() == 'null') {
                                        $('#PurchasePayment_Company option[value=""]').addClass('option-visible').removeClass('option-hidden');
                                    }
                                    else {
                                        $('#PurchasePayment_Company option[value=""]').addClass('option-hidden').removeClass('option-visible');
                                    }
                                    if (item.Value.split('|')[1] == '3') {
                                        $('#PurchasePayment_Company option[value="' + item.Value + '"]').addClass('option-visible').removeClass('option-hidden');
                                    }
                                    else {
                                        $('#PurchasePayment_Company option[value="' + item.Value + '"]').addClass('option-hidden').removeClass('option-visible');
                                    }
                                }
                            }
                            else {
                                //non-opc selected, hide all options
                                $('#PurchasePayment_Company option').addClass('option-hidden').removeClass('option-visible');
                            }
                        }
                    }
                    else {
                        //invoice chargebacks to mkt company
                        //show all options if opc not registered or there are more than one company on list
                        if ($('#PurchasePayment_OPC option:selected').val() == 'null' || $('#PurchasePayment_Company option[value!=""]').length > 1) {
                            $('#PurchasePayment_Company option').addClass('option-visible').removeClass('option-hidden');
                        }
                        else {
                            $('#PurchasePayment_Company option[value=""]').addClass('option-hidden').removeClass('option-visible');
                            $('#PurchasePayment_Company option[value!=""]').addClass('option-visible').removeClass('option-hidden');
                        }
                    }
                });

                var item = params != undefined ? params.company : $('#PurchasePayment_ChargeBackConcept option:selected').text().indexOf('Company') > 0 ? UI.returnMostSelectedValue(localStorage.Eplat_Counter_CXCCompany, 'CXCCompany') : '';
                if (item != '') {
                    $('#PurchasePayment_Company option[value="' + item + '"]').attr('selected', true);
                }
                else {
                    $.each(data, function (index, item) {
                        if (item.Selected == true) {
                            $('#PurchasePayment_Company option[value="' + item.Value + '"]').attr('selected', true);
                        }
                    });
                }
                //new code
                if ($('#PurchasePayment_ChargeBackConcept option:selected').text().toLowerCase().indexOf('cxc') > -1) {
                    $('#PurchasePayment_Company option[value="null"]').addClass('option-hidden').removeClass('option-visible');
                }
                else {
                    $('#PurchasePayment_Company option[value="null"]').addClass('option-visible').removeClass('option-hidden');
                }
                //
                $('#PurchasePayment_Company option.option-hidden').hide();
                $('#PurchasePayment_Company option.option-visible').show();
                //$('#PurchasePayment_Company option.option-hidden').attr('selected', false);

                if (params == undefined) {
                    $('#PurchasePayment_Company option.option-hidden').attr('selected', false);
                    var _itemToSelect = $('#PurchasePayment_Company option.option-visible').length > 2 ? $('#PurchasePayment_ChargeBackConcept option:selected').text().indexOf('Company') > 0 ? '[value="' + UI.returnMostSelectedValue(localStorage.Eplat_Counter_CXCCompany, 'CXCCompany') + '"]' : ':first' : ':last';
                    $('#PurchasePayment_Company option.option-visible' + _itemToSelect).attr('selected', true);
                }
            });
        });

        $('#PurchasePayment_Currency').on('change', function () {

        });

        $('#btnSavePurchasePayment').on('click', function () {
            if ($('#PurchasePayment_Amount').val() == '') {
                $('#PurchasePayment_Amount').val($('#PurchasePayment_Amount').attr('placeholder'));
            }
            if ($('#frmPurchasePaymentInfo').valid()) {
                proceedWithPayment();
            }
            else {
                UI.showValidationSummary('frmPurchasePaymentInfo');
            }
        });

        $('.print-chargeback').on('click', function (e) {
            var _dates = new Array();
            PURCHASE.oPurchasePaymentTable.$('tr').each(function () {
                _dates.push($(this).children('td:nth-child(6)').text().trim().split(' ')[0]);
            });
            _dates.sort(function (d1, d2) {
                function parse(str) {
                    var parts = str.match(/-/g);
                    return new Date(parts[0], parts[1] - 1, parts[2]);
                }
                return parse(d1) - parse(d2);
            });
            $('#txtPrintFromDate').datepicker('destroy');
            $('#txtPrintFromDate').datepicker({
                dateFormat: 'yy-mm-dd',
                minDate: _dates[0],
                maxDate: _dates[PURCHASE.oPurchasePaymentTable.$('tr').length - 1],
                onClose: function (dateText, inst) {
                    dateText = dateText != '' ? dateText : _dates[0];
                    $('#txtPrintToDate').datepicker('destroy');
                    $('#txtPrintToDate').datepicker({
                        dateFormat: 'yy-mm-dd',
                        minDate: dateText,
                        maxDate: _dates[PURCHASE.oPurchasePaymentTable.$('tr').length - 1],
                        onClose: function (dt, ins) {
                            dt = dt != '' ? dt : _dates[PURCHASE.oPurchasePaymentTable.$('tr').length - 1];
                            $.post('/MasterChart/GetChargeBackTicketInfo', { purchaseID: $('#PurchaseInfo_PurchaseID').val(), printFromDate: dateText, printToDate: dt }, function (data) {
                                var tables = new Array();
                                var counter = 0;
                                $.each(data, function (index, item) {
                                    var table = '';
                                    table += '<div>';
                                    table += '<table class="table">'
                                        + '<thead><tr class="no-border-left">'
                                        + '<th colspan="2" class="no-border-top no-border-left no-border-bottom" style="border-top:none;border-left:none;border-bottom:none;">&nbsp;</th>'
                                        + '<th colspan="3" class="align-center">CHARGE BACK</th>'
                                        + '<th colspan="3">FOLIO <span class="chargeback-folio right" id="chargebackFolio">' + item.Folio + '</span></th>'
                                        + '</tr>'
                                        + '<tr class="no-border-left">'
                                        + '<td id="" class="no-border-top no-border-left no-border-bottom" style="border-top:none;border-left:none;border-bottom:none;" colspan="2">&nbsp;</td>'
                                        + '<td id="personToCharge" class="person-to-charge align-center" colspan="3"><strong>' + item.PersonToCharge + '</strong></td>'
                                        + '<td colspan="2" id="payingCompany" class="paying-company">' + item.PayingCompany + '</td>'
                                        + '<td id="invitation" class="intivation align-center">' + item.Invitation + '</td>'
                                        + '</tr>'
                                        + '<tr class="field-name no-border-left">'
                                        + '<td id="" class="no-border-top no-border-left no-border-bottom" style="border-top:none;border-left:none;border-bottom:none;" colspan="2">&nbsp;</td>'
                                        + '<td colspan="3" class="align-center">NAME TO WHOM MUST BE CHARGE</td>'
                                        + '<td colspan="2" class="align-center">' + (item.ProgramOPC != null && item.ProgramOPC != "" ? 'OPC' : 'COMPANY') + '</td>'
                                        + '<td class="align-center">INVITATION</td>'
                                        + '</tr>'
                                        + '<tr class="no-border-left">'
                                        + '<td id="" class="no-border-top no-border-left no-border-bottom" style="border-top:none;border-left:none;border-bottom:none;" colspan="2">&nbsp;</td>'
                                        + '<td colspan="3" class="client-name align-center" id="clientName">' + item.ClientName + '</td>'
                                        + '<td colspan="2" class="program-opc align-center" id="programOpc">' + item.ProgramOPC + '</td>'
                                        + '<td id="budgetLetter" class="budget-letter align-center">' + item.BudgetLetter + '</td>'
                                        + '</tr>'
                                        + '<tr class="field-name no-border-left">'
                                        + '<td id="" class="no-border-top no-border-left no-border-bottom" style="border-top:none;border-left:none;border-bottom:none;" colspan="2">&nbsp;</td>'
                                        + '<td colspan="3" class="align-center">CUSTOMER\'S NAME</td>'
                                        + '<td colspan="2" class="align-center">PROMOTION TEAM</td>'
                                        + '<td class="align-center">BUDGET LETTER</td>'
                                        + '</tr>'
                                        + '<tr class="field-name">'
                                        + '<td colspan="5" class="align-center">CONCEPT DESCRIPTION</td>'
                                        + '<td colspan="3" class="align-center">TOTAL AMOUNT IN USD</td>'
                                        + '</tr>'
                                        + '</thead>';
                                    var _tbody = '';
                                    $.each(item.Services, function (index, item) {
                                        _tbody += '<tr>'
                                            + '<td colspan="5">' + item.Value.split('|').join(' | ') + '</td>'
                                            + '<td colspan="3" data-format="currency">' + item.Text + '</td>'
                                            + '</tr>';
                                    });
                                    table += '<tbody class="table-tbody">' + _tbody + '</tbody>';
                                    table += '<tfoot>'
                                        + '<tr class="field-name">'
                                        + '<td colspan="3" class="align-center">COMMENTS</td>'
                                        + '<td colspan="2" class="align-right">TOTAL DUE IN USD</td>'
                                        + '<td id="totalDue" class="total-due" colspan="3" data-format="currency">' + item.TotalDue + '</td>'
                                        + '</tr>'
                                        + '<tr class="field-name">'
                                        + '<td id="notes" rowspan="5" class="notes" colspan="3">' + item.Notes + '</td>'
                                        + '<td colspan="2" class="align-right">AMOUNT PAID BY CUSTOMER</td>'
                                        + '<td id="amountPaidByCustomer" class="amount-paid-by-customer negative-amount" colspan="3" data-format="currency">' + item.AmountPaidByCustomer + '</td>'
                                        + '</tr>'
                                        + '<tr class="field-name">'
                                        + '<td colspan="2" class="align-right">TOTAL TO CHARGE BACK</td>'
                                        + '<td id="totalDue2" class="total-due-2" colspan="3" data-format="currency">' + item.TotalDue2 + '</td>'
                                        + '</tr>'
                                        + '<tr class="field-name">';
                                    if (item.Container == 'tblCompanyChargeBack') {
                                        table += '<td colspan="2" class="align-right">OPC CHARGE BACK</td>';
                                    }
                                    else {
                                        table += '<td colspan="2" class="align-right">"<span class="budget-letter" id="budgetLetter2">' + item.BudgetLetter + '</span>" BUDGET AMOUNT</td>';
                                    }
                                    table += '<td id="budgetAmount" class="budget-amount negative-amount" colspan="3" data-format="currency">' + item.BudgetAmount + '</td>'
                                        + '</tr>'
                                        + '<tr class="field-name">'
                                        + '<td colspan="2" class="align-right">TOTAL TO CHARGE BACK IN USD</td>'
                                        + '<td id="totalUSD" class="total-usd" colspan="3" data-format="currency">' + item.TotalUSD + '</td>'
                                        + '</tr>'
                                        + '<tr class="field-name">'
                                        + '<td colspan="2" class="align-right">EQUIVALENCY IN MXN</td>'
                                        + '<td id="totalMXN" class="total-mxn" colspan="3" data-format="currency">' + item.TotalMXN + '</td>'
                                        + '</tr>'
                                        + '<tr class="field-name">'
                                        + '<td class="align-center">AGENT</td>'
                                        + '<td id="salesAgent" class="sales-agent">' + item.SalesAgent + '</td>'
                                        + '<td class="align-center">EXCHANGE RATE</td>'
                                        + '<td id="exchangeRateUsed" class="exchange-rate-used align-center" data-format="currency">' + item.ExchangeRateUsed + '</td>'
                                        + '<td class="align-center">POINT OF SALE</td>'
                                        + '<td id="varPointOfSale" class="point-of-sale">' + item.PointOfSale + '</td>'
                                        + '<td class="align-center">DATE</td>'
                                        + '<td id="date" class="purchase-date align-center">' + item.Date + '</td>'
                                        + '</tr>'
                                        + '</tfoot>'
                                        + '</table>';
                                    table += '</div>';
                                    tables.push(table);
                                });

                                function afterPrint(index) {
                                    $('.printable-chargeback').html('');
                                    $('.printable-chargeback').html(tables[index]);
                                    $('.printable:not(.printable-chargeback)').addClass('non-printable');
                                    $('.printable-chargeback').removeClass('non-printable');
                                    UI.applyFormat('currency');
                                    $('.printable-chargeback').printPage();
                                }

                                afterPrint(counter);

                                if (window.matchMedia) {
                                    var mediaQueryList = window.matchMedia('print');
                                    mediaQueryList.addListener(function (mql) {
                                        if (!mql.matches) {//after print
                                            counter++;
                                            if (counter < tables.length) {
                                                afterPrint(counter);
                                            }
                                        }
                                    });
                                }
                            }, 'json');
                        }
                    });
                    $('#txtPrintToDate').datepicker('show');
                    $('#txtPrintToDate').on('mousedown', function (e) {
                        e.preventDefault();
                    });
                    if ($('#ui-datepicker-div :last-child').is('table')) {
                        $('#ui-datepicker-div').prepend('<p style="text-align:center"><strong>Print To Date</strong></p>');
                    }
                    //$.post('/MasterChart/GetChargeBackTicketInfo', { purchaseID: $('#PurchaseInfo_PurchaseID').val(), printFromDate: dateText }, function (data) {
                    //    var tables = new Array();
                    //    var counter = 0;
                    //    $.each(data, function (index, item) {
                    //        var table = '';
                    //        table += '<div>';
                    //        table += '<table class="table">'
                    //        + '<thead><tr>'
                    //        + '<th colspan="5" class="align-center">CHARGE BACK</th>'
                    //        + '<th colspan="3">FOLIO <span class="chargeback-folio right" id="chargebackFolio">' + item.Folio + '</span></th>'
                    //        + '</tr>'
                    //            + '<tr>'
                    //                + '<td id="personToCharge" class="person-to-charge" colspan="5">' + item.PersonToCharge + '</td>'
                    //                + '<td colspan="3" id="payingCompany" class="paying-company">' + item.PayingCompany + '</td>'
                    //            + '</tr>'
                    //            + '<tr class="field-name">'
                    //                + '<td colspan="5" class="align-center">NAME TO WHOM MUST BE CHARGE</td>'
                    //                + '<td colspan="3" class="align-center">COMPANY</td>'
                    //            + '</tr>'
                    //            + '<tr>'
                    //                + '<td colspan="5" class="client-name" id="clientName">' + item.ClientName + '</td>'
                    //                + '<td colspan="2" class="program-opc align-center" id="programOpc">' + item.ProgramOPC + '</td>'
                    //                + '<td id="budgetLetter" class="budget-letter align-center">' + item.BudgetLetter + '</td>'
                    //            + '</tr>'
                    //            + '<tr class="field-name">'
                    //                + '<td colspan="5" class="align-center">CUSTOMER\'S NAME</td>'
                    //                + '<td colspan="2" class="align-center">PROMOTION TEAM</td>'
                    //                + '<td class="align-center">BUDGET LETTER</td>'
                    //            + '</tr>'
                    //            + '<tr class="field-name">'
                    //                + '<td colspan="5" class="align-center">CONCEPT DESCRIPTION</td>'
                    //                + '<td colspan="3" class="align-center">TOTAL AMOUNT IN USD</td>'
                    //            + '</tr>'
                    //        + '</thead>';
                    //        var _tbody = '';
                    //        $.each(item.Services, function (index, item) {
                    //            _tbody += '<tr>'
                    //                + '<td colspan="5">' + item.Value.split('|').join(' | ') + '</td>'
                    //                + '<td colspan="3" data-format="currency">' + item.Text + '</td>'
                    //                + '</tr>';
                    //        });
                    //        table += '<tbody class="table-tbody">' + _tbody + '</tbody>';
                    //        table += '<tfoot>'
                    //            + '<tr class="field-name">'
                    //                + '<td colspan="3" class="align-center">COMMENTS</td>'
                    //                + '<td colspan="2" class="align-right">TOTAL DUE IN USD</td>'
                    //                + '<td id="totalDue" class="total-due" colspan="3" data-format="currency">' + item.TotalDue + '</td>'
                    //            + '</tr>'
                    //            + '<tr class="field-name">'
                    //                + '<td id="notes" rowspan="5" class="notes" colspan="3">' + item.Notes + '</td>'
                    //                + '<td colspan="2" class="align-right">AMOUNT PAID BY CUSTOMER</td>'
                    //                + '<td id="amountPaidByCustomer" class="amount-paid-by-customer negative-amount" colspan="3" data-format="currency">' + item.AmountPaidByCustomer + '</td>'
                    //            + '</tr>'
                    //            + '<tr class="field-name">'
                    //                + '<td colspan="2" class="align-right">TOTAL TO CHARGE BACK</td>'
                    //                + '<td id="totalDue2" class="total-due-2" colspan="3" data-format="currency">' + item.TotalDue2 + '</td>'
                    //            + '</tr>'
                    //            + '<tr class="field-name">';
                    //        if (item.Container == 'tblCompanyChargeBack') {
                    //            table += '<td colspan="2" class="align-right">OPC CHARGE BACK</td>';
                    //        }
                    //        else {
                    //            table += '<td colspan="2" class="align-right">"<span class="budget-letter" id="budgetLetter2">' + item.BudgetLetter + '</span>" BUDGET AMOUNT</td>';
                    //        }
                    //        table += '<td id="budgetAmount" class="budget-amount negative-amount" colspan="3" data-format="currency">' + item.BudgetAmount + '</td>'
                    //            + '</tr>'
                    //            + '<tr class="field-name">'
                    //                + '<td colspan="2" class="align-right">TOTAL TO CHARGE BACK IN USD</td>'
                    //                + '<td id="totalUSD" class="total-usd" colspan="3" data-format="currency">' + item.TotalUSD + '</td>'
                    //            + '</tr>'
                    //            + '<tr class="field-name">'
                    //                + '<td colspan="2" class="align-right">EQUIVALENCY IN MXN</td>'
                    //                + '<td id="totalMXN" class="total-mxn" colspan="3" data-format="currency">' + item.TotalMXN + '</td>'
                    //            + '</tr>'
                    //            + '<tr class="field-name">'
                    //                + '<td class="align-center">AGENT</td>'
                    //                + '<td id="salesAgent" class="sales-agent">' + item.SalesAgent + '</td>'
                    //                + '<td class="align-center">EXCHANGE RATE</td>'
                    //                + '<td id="exchangeRateUsed" class="exchange-rate-used align-center" data-format="currency">' + item.ExchangeRateUsed + '</td>'
                    //                + '<td class="align-center">POINT OF SALE</td>'
                    //                + '<td id="varPointOfSale" class="point-of-sale">' + item.PointOfSale + '</td>'
                    //                + '<td class="align-center">DATE</td>'
                    //                + '<td id="date" class="purchase-date align-center">' + item.Date + '</td>'
                    //            + '</tr>'
                    //        + '</tfoot>'
                    //    + '</table>';
                    //        table += '</div>';
                    //        tables.push(table);
                    //    });

                    //    function afterPrint(index) {
                    //        $('.printable-chargeback').html(tables[index]);
                    //        $('.printable:not(.printable-chargeback)').addClass('non-printable');
                    //        $('.printable-chargeback').removeClass('non-printable');
                    //        UI.applyFormat('currency');
                    //        $('.printable-chargeback').printPage();
                    //    }

                    //    afterPrint(counter);

                    //    if (window.matchMedia) {
                    //        var mediaQueryList = window.matchMedia('print');
                    //        mediaQueryList.addListener(function (mql) {
                    //            if (!mql.matches) {//after print
                    //                counter++;
                    //                if (counter < tables.length) {
                    //                    afterPrint(counter);
                    //                }
                    //            }
                    //        });
                    //    }
                    //}, 'json');
                }
            });
            $('#txtPrintFromDate').datepicker('show');
            $('#txtPrintFromDate').on('mousedown', function (e) {
                e.preventDefault();
            });
            ////
            if ($('#ui-datepicker-div :last-child').is('table')) {
                $('#ui-datepicker-div').prepend('<p style="text-align:center"><strong>Print From Date</strong></p>');
            }
        });

        $('#btnPrintChargeVoucher').on('click', function () {
            $.post('/MasterChart/GetChargeVoucher', { purchaseID: $('#PurchaseInfo_PurchaseID').val() }, function (data) {
                var tables = new Array();
                var counter = 0;
                $.each(data, function (index, item) {
                    var table = '';
                    table += '<div>';
                    table += '<table class="table">'
                        + '<thead>'
                        + '<tr>'
                        + '<th colspan="5" class="align-center">CHARGE VOUCHER</th>'
                        + '<th colspan="3">FOLIO <span class="chargeback-folio right">' + item.Folio + '</span></th>'
                        + '</tr>'
                        + '<tr>'
                        + '<td colspan="8" class="align-center">' + item.Enterprise + ', ' + item.RFC + '<br />' + item.Address + '</td>'
                        + '</tr>'
                        + '<tr>'
                        + '<td colspan="4" class="">CHARGE TO: ' + item.PersonToCharge + '<br />CUSTOMER: ' + item.ClientName + '</td>'
                        + '<td colspan="4" class="">COMPANY: ' + item.PayingCompany + '<br />PROGRAM: ' + item.ProgramOPC + '</td>'
                        + '</tr>'
                        + '<tr>'
                        + '<td colspan="7" class="align-center">CONCEPT DESCRIPTION</td>'
                        + '<td class="">BUDGET: ' + item.BudgetLetter + '</td>'
                        + '</tr>'
                        + '</thead>';
                    var _services = '';
                    var _charges = '';
                    $.each(item.Services, function (index, item) {
                        _services += '<tr>'
                            + '<td colspan="7">' + item.Value.split('|').join(' | ') + '</td>'
                            + '<td data-format="currency">' + item.Text + '</td>'
                            + '</tr>';
                    });
                    $.each(item.Egresses, function (index, item) {
                        _charges += '<tr>'
                            + '<td colspan="7">' + item.Value + '</td>'
                            + '<td data-format="currency">' + item.Text + '</td>'
                            + '</tr>';
                    });
                    table += '<tbody class="table-body">'
                        + _services
                        + '<tr class="field-name">'
                        + '<td colspan="7" class="align-center">TRANSACTIONS</td>'
                        + '<td  class="align-center"></td>'
                        + '</tr>'
                        + _charges
                        + '</tbody>';
                    table += '<tfoot>'
                        + '<tr class="field-name">'
                        + '<td colspan="3" class="align-center">COMMENTS</td>'
                        + '<td colspan="3" class="align-right">TOTALS</td>'
                        + '<td class="align-center">USD</td>'
                        + '<td class="align-center">MXN</td>'
                        + '</tr>'
                        + '<tr class="field-name">'
                        + '<td rowspan="11" colspan="3">' + item.Notes + '</td>'
                        + '<td colspan="3" class="align-right">TOTAL DUE</td>'
                        + '<td data-format="currency">' + item.TotalDue + '</td>'
                        + '<td class="align-right">&nbsp;</td>'
                        + '</tr>'
                        + '<tr class="field-name">'
                        + '<td colspan="3" class="align-right">TOTAL DUE</td>'
                        + '<td>&nbsp;</td>'
                        + '<td data-format="currency">' + item.TotalDue2 + '</td>'
                        + '</tr>'
                        + '<tr class="field-name">'
                        + '<td colspan="3" class="align-right">PAID BY CUSTOMER</td>'
                        //+ '<td  data-format="currency">' + item.DepositAppliedToTours + '</td>'
                        + '<td  data-format="currency">' + item.AmountPaidByCustomer + '</td>'
                        + '<td class="align-right">&nbsp;</td>'
                        + '</tr>'
                        + '<tr class="display-name">'
                        + '<td colspan="3" class="align-right">PAID IN ADVANCE</td>'
                        + '<td data-format="currency">' + item.DepositAppliedToTours + '</td>'
                        //+ '<td data-format="currency">' + item.AmountPaidByCustomer + '</td>'
                        + '<td class="align-right">&nbsp;</td>'
                        + '</tr>'
                        + '<tr class="display-name">'
                        + '<td colspan="3" class="align-right">TOTAL TO CHARGE BACK</td>'
                        + '<td data-format="currency">' + item.TotalUSD + '</td>'
                        + '<td class="align-right">&nbsp;</td>'
                        + '</tr>'
                        + '<tr class="display-name">'
                        + '<td colspan="3" class="align-right">BUDGET AMOUNT</td>'
                        + '<td data-format="currency">' + item.BudgetAmount + '</td>'
                        + '<td class="align-right">&nbsp;</td>'
                        + '</tr>'
                        + '<tr class="display-name">'
                        + '<td colspan="3" class="align-right">CASH GIFT</td>'
                        + '<td data-format="currency">' + item.CashGiftUSD + '</td>'
                        + '<td data-format="currency">' + item.CashGiftMXN + '</td>'
                        + '</tr>'
                        + '<tr class="display-name">'
                        + '<td colspan="3" class="align-right">COMMISSION CASH GIFT</td>'
                        + '<td data-format="currency">' + item.CommissionCashGiftUSD + '</td>'
                        + '<td data-format="currency">' + item.CommissionCashGiftMXN + '</td>'
                        + '</tr>'
                        + '<tr class="display-name">'
                        + '<td colspan="3" class="align-right">CASH ACTIVITY</td>'
                        + '<td data-format="currency">' + item.CashActivityUSD + '</td>'
                        + '<td data-format="currency">' + item.CashActivityMXN + '</td>'
                        + '</tr>'
                        + '<tr class="display-name">'
                        + '<td colspan="3" class="align-right">TOTAL TO CHARGE BACK IN</td>'
                        + '<td data-format="currency">' + item.TotalToChargeBackInUSD + '</td>'
                        + '<td data-format="currency">' + item.TotalToChargeBackInMXN + '</td>'
                        + '</tr>'
                        + '<tr class="display-name">'
                        + '<td colspan="3" class="align-right">EQUIVALENCY IN</td>'
                        + '<td>&nbsp;</td>'
                        + '<td data-format="currency">' + item.TotalToChargeMXN + '</td>'
                        + '</tr>'
                        + '<tr class="display-name">'
                        + '<td class="align-center">AGENT</td>'
                        + '<td class="sales-agent">' + item.SalesAgent + '</td>'
                        + '<td class="align-center">EXCHANGE RATE</td>'
                        + '<td class="exchange-rate-used align-center" data-format="currency">' + item.ExchangeRateUsed + '</td>'
                        + '<td class="align-center">POINT OF SALE</td>'
                        + '<td class="point-of-sale">' + item.PointOfSale + '</td>'
                        + '<td class="align-center">DATE</td>'
                        + '<td class="purchase-date align-center">' + item.Date + '</td>'
                        + '</tr>'
                        + '</tfoot>'
                        + '</table>';
                    table += '</div>';
                    tables.push(table);
                });
                //$.each(data, function (index, item) {
                //    var table = '';
                //    table += '<div>';
                //    table += '<table class="table">'
                //    + '<thead>'
                //    + '<tr>'
                //        + '<th colspan="5" class="align-center">CHARGE VOUCHER</th>'
                //        + '<th colspan="3">FOLIO <span class="chargeback-folio right">' + item.Folio + '</span></th>'
                //    + '</tr>'
                //    + '<tr>'
                //        + '<td colspan="8" class="align-center">' + item.Enterprise + '<br />' + item.RFC + '<br />' + item.Address + '</td>'
                //    + '</tr>'
                //    + '<tr>'
                //        + '<td colspan="5" class="person-to-charge">' + item.PersonToCharge + '</td>'
                //        + '<td colspan="3" class="paying-company">' + item.PayingCompany + '</td>'
                //    + '</tr>'
                //    + '<tr class="field-name">'
                //        + '<td colspan="5" class="align-center">NAME TO WHOM MUST BE CHARGE</td>'
                //        + '<td colspan="3" class="align-center">COMPANY</td>'
                //    + '</tr>'
                //    + '<tr>'
                //        + '<td colspan="5" class="client-name">' + item.ClientName + '</td>'
                //        + '<td colspan="2" class="program-opc align-center">' + item.ProgramOPC + '</td>'
                //        + '<td class="budget-letter align-center">' + item.BudgetLetter + '</td>'
                //    + '</tr>'
                //    + '<tr class="field-name">'
                //        + '<td colspan="5" class="align-center">CUSTOMER NAME</td>'
                //        + '<td colspan="2" class="align-center">PROMOTION TEAM</td>'
                //        + '<td class="align-center">BUDGET LETTER</td>'
                //    + '</tr>'
                //    + '<tr class="field-name">'
                //        + '<td colspan="6" class="align-center">CONCEPT DESCRIPTION</td>'
                //        + '<td colspan="2" class="align-center">TOTALS</td>'
                //    + '</tr>'
                //    + '</thead>';
                //    var _services = '';
                //    var _charges = '';
                //    $.each(item.Services, function (index, item) {
                //        _services += '<tr>'
                //        + '<td colspan="6">' + item.Value.split('|').join(' | ') + '</td>'
                //        + '<td colspan="2" data-format="currency">' + item.Text + '</td>'
                //        + '</tr>';
                //    });
                //    $.each(item.Egresses, function (index, item) {
                //        _charges += '<tr>'
                //        + '<td colspan="6">' + item.Value + '</td>'
                //        + '<td colspan="2" data-format="currency">' + item.Text + '</td>'
                //        + '</tr>';
                //    });
                //    table += '<tbody class="table-body">'
                //    + _services
                //    + '<tr class="field-name">'
                //        + '<td colspan="6" class="align-center">TRANSACTIONS</td>'
                //        + '<td colspan="2" class="align-center">TOTALS</td>'
                //    + '</tr>'
                //    + _charges
                //    + '</tbody>';
                //    table += '<tfoot>'
                //    + '<tr class="field-name">'
                //    + '<td colspan="3" class="align-center">COMMENTS</td>'
                //    + '<td colspan="3" class="align-right">TOTAL DUE</td>'
                //    + '<td class="align-center">USD</td>'
                //    + '<td class="align-center">MXN</td>'
                //    + '</tr>'
                //    + '<tr class="field-name">'
                //    + '<td rowspan="11" colspan="3">' + item.Notes + '</td>'
                //    + '<td colspan="3" class="align-right">TOTAL DUE</td>'
                //    + '<td data-format="currency">' + item.TotalDue + '</td>'
                //    + '<td class="align-right">&nbsp;</td>'
                //    + '</tr>'
                //    + '<tr class="field-name">'
                //    + '<td colspan="3" class="align-right">TOTAL DUE</td>'
                //    + '<td>&nbsp;</td>'
                //    + '<td data-format="currency">' + item.TotalDue2 + '</td>'
                //    + '</tr>'
                //    + '<tr class="field-name">'
                //    + '<td colspan="3" class="align-right">PAID BY CUSTOMER</td>'
                //    + '<td  data-format="currency">' + item.AmountPaidByCustomer + '</td>'
                //    + '<td class="align-right">&nbsp;</td>'
                //    + '</tr>'
                //    + '<tr class="display-name">'
                //    + '<td colspan="3" class="align-right">APPLIED TO TOURS</td>'
                //    + '<td data-format="currency">' + item.DepositAppliedToTours + '</td>'
                //    + '<td class="align-right">&nbsp;</td>'
                //    + '</tr>'
                //    + '<tr class="display-name">'
                //    + '<td colspan="3" class="align-right">TOTAL TO CHARGE BACK</td>'
                //    + '<td data-format="currency">' + item.TotalUSD + '</td>'
                //    + '<td class="align-right">&nbsp;</td>'
                //    + '</tr>'
                //    + '<tr class="display-name">'
                //    + '<td colspan="3" class="align-right">BUDGET AMOUNT</td>'
                //    + '<td data-format="currency">' + item.BudgetAmount + '</td>'
                //    + '<td class="align-right">&nbsp;</td>'
                //    + '</tr>'
                //    + '<tr class="display-name">'
                //    + '<td colspan="3" class="align-right">CASH GIFT</td>'
                //    + '<td data-format="currency">' + item.CashGiftUSD + '</td>'
                //    + '<td data-format="currency">' + item.CashGiftMXN + '</td>'
                //    + '</tr>'
                //    + '<tr class="display-name">'
                //    + '<td colspan="3" class="align-right">COMMISSION CASH GIFT</td>'
                //    + '<td data-format="currency">' + item.CommissionCashGiftUSD + '</td>'
                //    + '<td data-format="currency">' + item.CommissionCashGiftMXN + '</td>'
                //    + '</tr>'
                //    + '<tr class="display-name">'
                //    + '<td colspan="3" class="align-right">CASH ACTIVITY</td>'
                //    + '<td data-format="currency">' + item.CashActivityUSD + '</td>'
                //    + '<td data-format="currency">' + item.CashActivityMXN + '</td>'
                //    + '</tr>'
                //    + '<tr class="display-name">'
                //    + '<td colspan="3" class="align-right">TOTAL TO CHARGE BACK IN</td>'
                //    + '<td data-format="currency">' + item.TotalToChargeBackInUSD + '</td>'
                //    + '<td data-format="currency">' + item.TotalToChargeBackInMXN + '</td>'
                //    + '</tr>'
                //    + '<tr class="display-name">'
                //    + '<td colspan="3" class="align-right">EQUIVALENCY IN</td>'
                //    + '<td>&nbsp;</td>'
                //    + '<td data-format="currency">' + item.TotalToChargeMXN + '</td>'
                //    + '</tr>'
                //    + '<tr class="display-name">'
                //    + '<td class="align-center">AGENT</td>'
                //    + '<td class="sales-agent">' + item.SalesAgent + '</td>'
                //    + '<td class="align-center">EXCHANGE RATE</td>'
                //    + '<td class="exchange-rate-used align-center" data-format="currency">' + item.ExchangeRateUsed + '</td>'
                //    + '<td class="align-center">POINT OF SALE</td>'
                //    + '<td class="point-of-sale">' + item.PointOfSale + '</td>'
                //    + '<td class="align-center">DATE</td>'
                //    + '<td class="purchase-date align-center">' + item.Date + '</td>'
                //    + '</tr>'
                //    + '</tfoot>'
                //    + '</table>';
                //    table += '</div>';
                //    tables.push(table);
                //});

                function afterPrint(index) {
                    $('.printable-chargeback').html('');
                    $('.printable-chargeback').html(tables[index]);
                    $('.printable:not(.printable-chargeback)').addClass('non-printable');
                    $('.printable-chargeback').removeClass('non-printable');
                    UI.applyFormat('currency');
                    $('.printable-chargeback').printPage();
                }

                afterPrint(counter);

                if (window.matchMedia) {
                    var mediaQueryList = window.matchMedia('print');
                    mediaQueryList.addListener(function (mql) {
                        if (!mql.matches) {//after print
                            counter++;
                            if (counter < tables.length) {
                                afterPrint(counter);
                            }
                        }
                    });
                }
            }, 'json');
        });

        if ($(window).width() > 768) {
            $('#PurchaseInfo_StayingAtPlace').multiselect({
                multiple: false,
                noneSelectedText: '--Select One--',
                selectedList: 1
            }).multiselectfilter();
            $('#PurchaseService_Service').multiselect({
                multiple: false,
                noneSelectedText: '--Select One--',
                selectedList: 1
            }).multiselectfilter();
            $('#PurchaseService_Provider').multiselect({
                multiple: false,
                noneSelectedText: '--Select One--',
                selectedList: 1
            }).multiselectfilter();
            $('*[data-uses-multiselect-single=true]').multiselect({
                multiple: false,
                noneSelectedText: '--Select One--',
                selectedList: 1
            }).multiselectfilter();
            $('#PurchasePayment_OPC').multiselect({
                multiple: false,
                noneSelectedText: '--Select One--',
                selectedList: 1
            }).multiselectfilter();
        }

        PURCHASE.preselectFieldsValue();

        UI.updateListsOnTerminalsChange();

        $('#btnFastSale').on('click', function () {
            $.fancybox({
                //type: 'html',
                type: 'ajax',
                href: '/MasterChart/RenderFastSale',
                //href: $('#divFastSaleContainer'),
                modal: true,
                afterShow: function () {
                    FS.init();
                }
            });
        });

        $('#imgCloseLead').on('click', function () {
            while ($(document).find('.selected-row').length > 0) {
                var e = $.Event('keydown');
                e.keyCode = 27;
                $(document).trigger(e);
            }
        });

        ////allow call every page load
        ////if (localStorage.Eplat_Agency_Manifest == undefined || localStorage.Eplat_Agency_Manifest == null || localStorage.Eplat_Agency_Manifest == '')
        //{
        //    PURCHASE.getAgencyManifest();
        //}

    }

    function onCloseServiceDate(dateText) {
        altDateFormat('PurchaseService_ServiceDateTime', 'altDateFormat', undefined, $('#PurchaseService_Service').val());
        var params = { service: $('#PurchaseService_Service').val(), datetime: dateText, promo: $('#PurchaseService_Promo').val() };
        //PURCHASE.updatePromosPerServiceDate(params);
        if ($('#PurchaseService_Service').val() != '0') {
            PURCHASE.updatePromosPerServiceDate(params);
            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType option[value="' + UI.returnMostSelectedValue(localStorage.Eplat_Counter_PriceType, 'PriceType') + '"]').attr('selected', true).trigger('change');
        }
    }

    function restrictPriceTypes(priceType) {
        if (priceType != 0) {
            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType option').each(function () {
                if ($(this).val() != priceType && $(this).val() != 0) {
                    $(this).hide();
                }
            });
        }
        else {
            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType option').each(function () {
                $(this).show();
            });
        }
    }

    function proceedWithPayment() {
        var status = '';
        var _paymentDate = null;
        var dueCurrency = $('#spanDue').text().split(' ')[1];
        var dueAmount = $('#spanDue').text().split(' ')[0].substr(1).replace(',', '');
        var exchangeRates = '';
        if (COMMON.getDate() == $('#PurchasePayment_DateSaved').val()) {
            exchangeRates = $('#currentExchangeRates').text().split(' ').join('').split(',');
        }
        else {
            $.ajax({
                url: '/MasterChart/GetExchangeRates',
                async: false,
                contentType: 'JSON',
                data: { date: ($('#PurchasePayment_DateSaved').val() != undefined ? $('#PurchasePayment_DateSaved').val() : COMMON.getDate()), pointOfSaleID: $('#PurchaseInfo_PointOfSale option:selected').val() },
                success: function (data) {
                    exchangeRates = data.split(' ').join('').split(',');
                }
            });
        }
        exchangeRates.push('MXN=1.00');

        var applyCommission = $('#PurchasePayment_ApplyCommission').is(':visible') == true ? $('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val() : 'False';
        var transactionAmount = $('#PurchasePayment_Amount').val() != '' ? parseFloat($('#PurchasePayment_Amount').val()) : 0;
        var amountConverted = convertAmountToRate(exchangeRates, transactionAmount, $('#PurchasePayment_Currency option:selected').val(), dueCurrency);
        var _amount = amountConverted.amount;
        var _rate = amountConverted.rate;

        $.ajax({
            url: '/Masterchart/GetExchangeRateOfPurchase',
            type: 'POST',
            cache: false,
            data: { purchaseID: $('#PurchaseInfo_PurchaseID').val() },
            success: function (data) {

                if ($('#PurchasePayment_PaymentDetailsID').val() != '' && $('#PurchasePayment_PaymentDetailsID').val() != '0') {
                    //update
                    _paymentDate = PURCHASE.oPurchasePaymentTable.$('tr.selected-row')[0].cells[5].textContent.trim().split(' ')[0];
                    if (($('#PurchasePayment_TransactionType option:selected').val() == '1' && $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') != -1) || ($('#PurchasePayment_TransactionType option:selected').val() == '2' && $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') == -1)) {
                        status = 'Cannot change transaction type';
                    }
                    //update payment even with due in 0
                    var _tableAmountInDueCurrency = Math.trunc(convertAmountToRate(exchangeRates, $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0], $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[1], dueCurrency).amount);
                    //agregar codigo para considerar cuando se agrega o quita comision bancaria en una actualizacion
                    _amount = Math.trunc((applyCommission == 'True' ? (_amount / (1 + (parseFloat($('#PurchasePayment_CardCommission').val()) / 100))) : _amount));
                    if (_amount <= dueAmount || dueAmount == 0 || (_amount - _tableAmountInDueCurrency) <= dueAmount) {
                        status = 'valid';
                    }
                    else if (_amount > dueAmount) {
                        status = 'exceeds';
                    }

                    //revisar el contenido de la bandera en la terminal para limitar el importe de los reembolsos
                    if ($('#PurchasePayment_TransactionType option:selected').val() == '2') {
                        $.ajax({
                            async: false,
                            type: 'GET',
                            url: '/MasterChart/LimitRefundsPerType',
                            data: { terminalID: $('#PurchaseInfo_Terminal option:selected').val() },
                            success: function (data) {
                                var paymentType = ($('#PurchasePayment_PaymentType option:selected').val() >= 1 && $('#PurchasePayment_PaymentType option:selected').val() <= 3) ? $('#PurchasePayment_PaymentType option:selected').text().trim().toLowerCase() : 'allow';
                                if (paymentType != 'allow' && data.limitRefunds == true) {
                                    var amountOfPayments = 0;
                                    var amountOfRefunds = 0;
                                    if (paymentType == 'cash' || paymentType == 'credit card') {
                                        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row):not(.selected-row)').not('theader').each(function () {
                                            if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == 'cash') {
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                            else if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == 'credit card') {//credit card
                                                var netAmount = $(this).children('td:nth-child(2)').find('.hdn-commission-applied').val().toLowerCase() == 'true' ? ($(this).children('td:nth-child(4)').text().trim().split(' ')[0] / (1 + (parseFloat($(this).children('td:nth-child(2)').find('.hdn-bank-commission').val()) / 100))) : $(this).children('td:nth-child(4)').text().trim().split(' ')[0];
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, netAmount, $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, netAmount, $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                        });
                                    }
                                    else {
                                        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row):not(.selected-row)').not('theader').each(function () {
                                            if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == paymentType) {
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                        });
                                    }

                                    if ((amountOfRefunds + _amount) > amountOfPayments && ((amountOfRefunds + _amount) - amountOfPayments) > 0.1) {
                                        status = 'refundExceedsPayments';
                                    }
                                }
                            }
                        });
                    }
                }
                else {
                    /*GGP*/
                    //convert transactionAmount to same currency as purchase
                    //get real amount if charge commission is included

                    //linea descomentada para tener la comision bancaria correcta.
                    _amount = Math.trunc((applyCommission == 'True' ? (_amount / (1 + (parseFloat($('#PurchasePayment_CardCommission').val()) / 100))) : _amount));//neto
                    //_amount = Math.trunc((applyCommission == 'True' ? (_amount / (1 + (parseFloat(data.commission) / 100))) : _amount));

                    if ((dueAmount > 0 && _amount <= dueAmount) || $('#PurchasePayment_TransactionType option:selected').val() == '2' || (_amount - dueAmount) < 1) {
                        status = 'valid';
                    }
                    else if (dueAmount == 0) {
                        status = 'paid';
                    }
                    else if (_amount > dueAmount) {
                        status = 'exceeds';
                    }
                    //revisar el contenido de la bandera en la terminal para limitar el importe de los reembolsos
                    if ($('#PurchasePayment_TransactionType option:selected').val() == '2') {
                        $.ajax({
                            async: false,
                            type: 'GET',
                            url: '/MasterChart/LimitRefundsPerType',
                            data: { terminalID: $('#PurchaseInfo_Terminal option:selected').val() },
                            success: function (data) {
                                var paymentType = ($('#PurchasePayment_PaymentType option:selected').val() >= 1 && $('#PurchasePayment_PaymentType option:selected').val() <= 3) ? $('#PurchasePayment_PaymentType option:selected').text().trim().toLowerCase() : 'allow';
                                if (paymentType != 'allow' && data.limitRefunds == true) {
                                    var amountOfPayments = 0;
                                    var amountOfRefunds = 0;
                                    if (paymentType == 'cash' || paymentType == 'credit card') {
                                        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row)').not('theader').each(function () {
                                            if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == 'cash') {
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                            else if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == 'credit card') {//credit card
                                                var netAmount = $(this).children('td:nth-child(2)').find('.hdn-commission-applied').val().toLowerCase() == 'true' ? ($(this).children('td:nth-child(4)').text().trim().split(' ')[0] / (1 + (parseFloat($(this).children('td:nth-child(2)').find('.hdn-bank-commission').val()) / 100))) : $(this).children('td:nth-child(4)').text().trim().split(' ')[0];
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, netAmount, $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, netAmount, $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                        });

                                    }
                                    else {
                                        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row)').not('theader').each(function () {
                                            if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == paymentType) {
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                        });
                                    }

                                    if ((amountOfRefunds + _amount) > amountOfPayments && ((amountOfRefunds + _amount) - amountOfPayments) > 0.1) {
                                        status = 'refundExceedsPayments';
                                    }
                                }
                            }
                        });
                    }
                }

                switch (status) {
                    case 'valid': {
                        savePayment('frmPurchasePaymentInfo');
                        break;
                    }
                    case 'exceeds': {
                        UI.messageBox(0, 'Payments Amount Exceeds Total Required', null, null);
                        break;
                    }
                    case 'paid': {
                        UI.messageBox(0, 'Purchase is Paid', null, null);
                        break;
                    }
                    case 'exceedsBudget': {
                        UI.messageBox(0, "Payment Amount Exceeds Team's Budget", null, null);
                        break;
                    }
                    case 'error': {
                        UI.messageBox(-1, 'An Error has ocurred processing the payment', null, null);
                        break;
                    }
                    case 'refundExceedsPayments': {
                        UI.messageBox(-1, 'Refunds cannot be higher than payments with same type', null, null);
                        break;
                    }
                    default: {
                        UI.messageBox(-1, status, null, null);
                        break;
                    }
                }
            }
        });
    }

    function _proceedWithPayment() {
        var status = '';
        var _paymentDate = null;
        var dueCurrency = $('#spanDue').text().split(' ')[1];
        var dueAmount = $('#spanDue').text().split(' ')[0].substr(1).replace(',', '');
        var exchangeRates = '';
        if (COMMON.getDate() == $('#PurchasePayment_DateSaved').val()) {
            exchangeRates = $('#currentExchangeRates').text().split(' ').join('').split(',');
        }
        else {
            $.ajax({
                url: '/MasterChart/GetExchangeRates',
                async: false,
                contentType: 'JSON',
                data: { date: ($('#PurchasePayment_DateSaved').val() != undefined ? $('#PurchasePayment_DateSaved').val() : COMMON.getDate()), pointOfSaleID: $('#PurchaseInfo_PointOfSale option:selected').val() },
                success: function (data) {
                    exchangeRates = data.split(' ').join('').split(',');
                }
            });
        }
        exchangeRates.push('MXN=1.00');

        var applyCommission = $('#PurchasePayment_ApplyCommission').is(':visible') == true ? $('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val() : 'False';
        var transactionAmount = $('#PurchasePayment_Amount').val() != '' ? parseFloat($('#PurchasePayment_Amount').val()) : 0;
        var amountConverted = convertAmountToRate(exchangeRates, transactionAmount, $('#PurchasePayment_Currency option:selected').val(), dueCurrency);
        var _amount = amountConverted.amount;
        var _rate = amountConverted.rate;

        $.ajax({
            url: '/Masterchart/GetExchangeRateOfPurchase',
            type: 'POST',
            cache: false,
            data: { purchaseID: $('#PurchaseInfo_PurchaseID').val() },
            success: function (data) {

                if ($('#PurchasePayment_PaymentDetailsID').val() != '' && $('#PurchasePayment_PaymentDetailsID').val() != '0') {
                    //update
                    _paymentDate = PURCHASE.oPurchasePaymentTable.$('tr.selected-row')[0].cells[5].textContent.trim().split(' ')[0];
                    if (($('#PurchasePayment_TransactionType option:selected').val() == '1' && $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') != -1) || ($('#PurchasePayment_TransactionType option:selected').val() == '2' && $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') == -1)) {
                        status = 'Cannot change transaction type';
                    }
                    //update payment even with due in 0
                    var _tableAmountInDueCurrency = Math.trunc(convertAmountToRate(exchangeRates, $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0], $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[1], dueCurrency).amount);
                    //agregar codigo para considerar cuando se agrega o quita comision bancaria en una actualizacion
                    //_amount = Math.trunc((applyCommission == 'True' ? (_amount / (1 + (data.commission / 100))) : _amount));
                    _amount = Math.trunc((applyCommission == 'True' ? (_amount / (1 + (parseFloat($('#PurchasePayment_CardCommission').val()) / 100))) : _amount));
                    if (_amount <= dueAmount || dueAmount == 0 || (_amount - _tableAmountInDueCurrency) <= dueAmount) {
                        status = 'valid';
                    }
                    else if (_amount > dueAmount) {
                        status = 'exceeds';
                    }
                    //if (_amount <= _tableAmountInDueCurrency || (parseFloat(dueAmount) + parseFloat(_tableAmountInDueCurrency)) >= _amount) {
                    //    status = 'valid';
                    //}

                    //if (status == '') {
                    //    if (dueAmount > 0 || $('#PurchasePayment_TransactionType option:selected').val() == '2') {
                    //        if ((parseFloat(dueAmount) + (dueCurrency != $('#PurchasePayment_Currency').val() ? (_rate / 10) : 0)) >= _amount || ((parseFloat(dueAmount) + (dueCurrency != $('#PurchasePayment_Currency').val() ? (_rate / 10) : 0)) - _amount) < 0.001 || $('#PurchasePayment_TransactionType option:selected').val() == '2') {
                    //            status = 'valid';
                    //        }
                    //        else {
                    //            status = 'exceeds';
                    //        }
                    //    }
                    //    else {
                    //        status = 'paid';
                    //    }
                    //}

                    //revisar el contenido de la bandera en la terminal para limitar el importe de los reembolsos
                    if ($('#PurchasePayment_TransactionType option:selected').val() == '2') {
                        $.ajax({
                            async: false,
                            type: 'GET',
                            url: '/MasterChart/LimitRefundsPerType',
                            data: { terminalID: $('#PurchaseInfo_Terminal option:selected').val() },
                            success: function (data) {
                                //var paymentType = ($('#PurchasePayment_PaymentType option:selected').val() == 3 || $('#PurchasePayment_PaymentType option:selected').val() == 1) ? $('#PurchasePayment_PaymentType option:selected').text().trim().toLowerCase() : 'allow';
                                var paymentType = ($('#PurchasePayment_PaymentType option:selected').val() >= 1 && $('#PurchasePayment_PaymentType option:selected').val() <= 3) ? $('#PurchasePayment_PaymentType option:selected').text().trim().toLowerCase() : 'allow';
                                if (paymentType != 'allow' && data.limitRefunds == true) {
                                    var amountOfPayments = 0;
                                    var amountOfRefunds = 0;
                                    if (paymentType == 'cash' || paymentType == 'credit card') {
                                        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row):not(.selected-row)').not('theader').each(function () {
                                            if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == 'cash') {
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                            else if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == 'credit card') {//credit card
                                                var netAmount = $(this).children('td:nth-child(2)').find('.hdn-commission-applied').val().toLowerCase() == 'true' ? ($(this).children('td:nth-child(4)').text().trim().split(' ')[0] / (1 + (parseFloat($(this).children('td:nth-child(2)').find('.hdn-bank-commission').val()) / 100))) : $(this).children('td:nth-child(4)').text().trim().split(' ')[0];
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, netAmount, $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, netAmount, $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                        });
                                    }
                                    else {
                                        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row):not(.selected-row)').not('theader').each(function () {
                                            if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == paymentType) {
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                        });
                                    }

                                    //if ((amountOfRefunds + _amount) > amountOfPayments) {
                                    if ((amountOfRefunds + _amount) > amountOfPayments && ((amountOfRefunds + _amount) - amountOfPayments) > 0.1) {
                                        status = 'refundExceedsPayments';
                                    }
                                }
                            }
                        });
                    }
                }
                else {
                    /*GGP*/
                    //convert transactionAmount to same currency as purchase
                    //_amount = _amount / _rate;
                    //get real amount if charge commission is included

                    //_amount = Math.trunc((applyCommission == 'True' ? (_amount / (1 + (data.commission / 100))) : _amount));//neto
                    _amount = Math.trunc((applyCommission == 'True' ? (_amount / (1 + (parseFloat($('#PurchasePayment_CardCommission').val()) / 100))) : _amount));//neto

                    //if ((dueAmount > 0 && _amount <= dueAmount) || $('#PurchasePayment_TransactionType option:selected').val() == '2') {
                    if ((dueAmount > 0 && _amount <= dueAmount) || $('#PurchasePayment_TransactionType option:selected').val() == '2' || (_amount - dueAmount) < 1) {
                        status = 'valid';
                    }
                    else if (dueAmount == 0) {
                        status = 'paid';
                    }
                    else if (_amount > dueAmount) {
                        status = 'exceeds';
                    }
                    //revisar el contenido de la bandera en la terminal para limitar el importe de los reembolsos
                    if ($('#PurchasePayment_TransactionType option:selected').val() == '2') {
                        $.ajax({
                            async: false,
                            type: 'GET',
                            url: '/MasterChart/LimitRefundsPerType',
                            data: { terminalID: $('#PurchaseInfo_Terminal option:selected').val() },
                            success: function (data) {
                                var paymentType = ($('#PurchasePayment_PaymentType option:selected').val() >= 1 && $('#PurchasePayment_PaymentType option:selected').val() <= 3) ? $('#PurchasePayment_PaymentType option:selected').text().trim().toLowerCase() : 'allow';
                                if (paymentType != 'allow' && data.limitRefunds == true) {
                                    var amountOfPayments = 0;
                                    var amountOfRefunds = 0;
                                    if (paymentType == 'cash' || paymentType == 'credit card') {
                                        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row)').not('theader').each(function () {
                                            if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == 'cash') {
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                            else if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == 'credit card') {//credit card
                                                var netAmount = $(this).children('td:nth-child(2)').find('.hdn-commission-applied').val().toLowerCase() == 'true' ? ($(this).children('td:nth-child(4)').text().trim().split(' ')[0] / (1 + (parseFloat($(this).children('td:nth-child(2)').find('.hdn-bank-commission').val()) / 100))) : $(this).children('td:nth-child(4)').text().trim().split(' ')[0];
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, netAmount, $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, netAmount, $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                        });

                                    }
                                    else {
                                        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row)').not('theader').each(function () {
                                            if ($(this).children('td:nth-child(2)').text().trim().toLowerCase() == paymentType) {
                                                if (!$(this).children('td:nth-child(4)').hasClass('negative-amount')) {
                                                    amountOfPayments += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                                else {
                                                    amountOfRefunds += parseFloat(convertAmountToRate(exchangeRates, $(this).children('td:nth-child(4)').text().trim().split(' ')[0], $(this).children('td:nth-child(4)').text().trim().split(' ')[1], $('#PurchasePayment_Currency option:selected').val()).amount);
                                                }
                                            }
                                        });
                                    }
                                    //if ((amountOfRefunds + _amount) > amountOfPayments) {
                                    if ((amountOfRefunds + _amount) > amountOfPayments && ((amountOfRefunds + _amount) - amountOfPayments) > 0.1) {
                                        status = 'refundExceedsPayments';
                                    }
                                }
                            }
                        });
                    }
                }
                //status = 'Test done';//testing
                switch (status) {
                    case 'valid': {
                        savePayment('frmPurchasePaymentInfo');
                        break;
                    }
                    case 'exceeds': {
                        UI.messageBox(0, 'Payments Amount Exceeds Total Required', null, null);
                        break;
                    }
                    case 'paid': {
                        UI.messageBox(0, 'Purchase is Paid', null, null);
                        break;
                    }
                    case 'exceedsBudget': {
                        UI.messageBox(0, "Payment Amount Exceeds Team's Budget", null, null);
                        break;
                    }
                    case 'error': {
                        UI.messageBox(-1, 'An Error has ocurred processing the payment', null, null);
                        break;
                    }
                    case 'refundExceedsPayments': {
                        UI.messageBox(-1, 'Refunds cannot be higher than payments with same type', null, null);
                        break;
                    }
                    default: {
                        UI.messageBox(-1, status, null, null);
                        break;
                    }
                }
            }
        });
    }

    function convertAmountToRate(exchangeRates, amount, amountCurrency, targetCurrency) {
        var _amount;
        var _rate;
        //convert transactionAmount to mxn
        $.each(exchangeRates, function (index, item) {
            if (item.split('=')[0] == amountCurrency) {
                _amount = amount * parseFloat(item.split('=')[1]);
            }
        });
        //get rate of purchase's currency
        $.each(exchangeRates, function (index, item) {
            if (item.split('=')[0] == targetCurrency) {
                _rate = item.split('=')[1];
            }
        });
        //convert transactionAmount to same currency as purchase
        _amount = _amount / _rate;
        return { amount: _amount, rate: _rate };
    }

    function proceedWithPaymentaa(changeStatusFlag) {
        var status = '';
        var currentExchangeRates = $('#currentExchangeRates').text().split(' ').join('').split(',');
        var transactionRate = 1;
        var applyCommission = $('#PurchasePayment_PaymentType option:selected').val() == '2' ? $('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val() : 'False';
        //if refund, make it negative
        var transactionAmount = $('#PurchasePayment_Amount').val() != '' && $('#PurchasePayment_TransactionType option:selected').val() == '1' ? parseFloat($('#PurchasePayment_Amount').val()) : (parseFloat($('#PurchasePayment_Amount').val()) * -1);

        $.each(currentExchangeRates, function (index, item) {
            if (item.split('=')[0] == $('#PurchasePayment_Currency option:selected').val()) {
                transactionRate = parseFloat(item.split('=')[1]);
                transactionAmount = transactionAmount * parseFloat(item.split('=')[1]);
            }
        });



        $.ajax({
            url: '/Masterchart/GetExchangeRateOfPurchase',
            type: 'POST',
            cache: false,
            data: { purchaseID: $('#PurchaseInfo_PurchaseID').val() },
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var purchaseTotal = $('#PurchaseInfo_Total').val() != '' ? (parseFloat($('#PurchaseInfo_Total').val()) * data.exchangeRate) : 0;
                var paymentsHistory = getPaymentsHistory(data.commission);
                if (status == '') {
                    if ($('#PurchasePayment_PaymentDetailsID').val() != '' && $('#PurchasePayment_PaymentDetailsID').val() != '0') {
                        //edition of existing payment
                        if (($('#PurchasePayment_TransactionType option:selected').val() == '1' && $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') == -1) || ($('#PurchasePayment_TransactionType option:selected').val() == '2' && $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') != -1)) {
                            //verify that the update is for a payment or a refund, but not a change of transactionType
                            var _exchangeRate = parseFloat($('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(3)').text());
                            //line changed for considering refunds
                            var _amount = $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') == -1 ? parseFloat($('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0]) : 0;
                            var _previousAmount = _exchangeRate * _amount;
                            var _payments = paymentsHistory.payments - _previousAmount;

                            if (paymentsHistory.paymentsInSameCurrency > (purchaseTotal / data.exchangeRate) || paymentsHistory.payments > purchaseTotal) {
                                status = 'exceeds';
                                if (paymentsHistory.paymentsInSameCurrency > (purchaseTotal / data.exchangeRate) || paymentsHistory.payments > purchaseTotal) {
                                    status = 'paid';
                                }
                            }
                            else {
                                status = 'error';
                                if ((_payments + transactionAmount - (applyCommission == 'True' ? transactionAmount - (transactionAmount / (1 + (data.commission / 100))) : 0)) <= (purchaseTotal + (data.exchangeRate / 10))
                                    || (Math.abs(parseFloat($('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0])) - $('#PurchasePayment_Amount').val()) == 0) {
                                    status = 'valid';
                                }
                            }
                            if ($('#PurchasePayment_TransactionType option:selected').val() == '2' || _amount == transactionAmount || _amount == parseFloat($('#PurchasePayment_Amount').val())) {
                                status = 'valid';
                            }
                        }
                        else {
                            status = 'Cannot change Transaction Type';
                        }
                    }
                    else {
                        if (paymentsHistory.paymentsInSameCurrency >= (purchaseTotal / data.exchangeRate) || paymentsHistory.payments >= purchaseTotal) {
                            status = 'exceeds';
                            if (paymentsHistory.paymentsInSameCurrency == (purchaseTotal / data.exchangeRate) || paymentsHistory.payments == purchaseTotal) {
                                status = 'paid';
                            }
                        }
                        else {
                            status = 'error';
                            if (paymentsHistory.payments + (transactionAmount - (applyCommission == 'True' ? transactionAmount - (transactionAmount / (1 + (data.commission / 100))) : 0)) <= (purchaseTotal + (data.exchangeRate / 10))
                                || paymentsHistory.paymentsInSameCurrency + (parseFloat($('#PurchasePayment_Amount').val()) - (applyCommission == 'True' ? parseFloat($('#PurchasePayment_Amount').val()) - (parseFloat($('#PurchasePayment_Amount').val()) / (1 + (data.commission / 100))) : 0)) <= parseFloat($('#PurchaseInfo_Total').val())
                            ) {
                                status = 'valid';
                            }
                        }
                        if ($('#PurchasePayment_TransactionType option:selected').val() == '2') {
                            status = 'valid';
                        }
                    }
                }
                switch (status) {
                    case 'valid': {
                        savePayment('frmPurchasePaymentInfo', changeStatusFlag);
                        break;
                    }
                    case 'exceeds': {
                        UI.messageBox(0, 'Payments Amount Exceeds Total Required', null, null);
                        break;
                    }
                    case 'paid': {
                        UI.messageBox(0, 'Purchase is Paid', null, null);
                        break;
                    }
                    case 'exceedsBudget': {
                        UI.messageBox(0, "Amount Exceeds Team's Budget", null, null);
                        break;
                    }
                    case 'error': {
                        UI.messageBox(-1, 'An Error has ocurred processing the payment', null, null)
                    }
                    default: {
                        UI.messageBox(-1, status, null, null);
                        break;
                    }
                }
            }
        });
    }

    function proceedWithPaymenta(changeStatusFlag) {
        var status = '';
        //var paymentsHistory = getPaymentsHistory().payments;
        //var paymentsHistory = 0;
        var currentExchangeRates = $('#currentExchangeRates').text().split(' ').join('').split(',');
        var currentRate = 1;
        var applyCommission = $('#PurchasePayment_PaymentType option:selected').val() == '2' ? $('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val() : 'False';

        //var hasBudget = $.parseJSON($('#hdnPromotionTeamInfo').val());//teamHasBudget($('#PurchasePayment_PromotionTeam').val());
        //var budget = hasBudget != null ? hasBudget[0].Value.indexOf('true') != -1 ? parseFloat(hasBudget[0].Value.split('|')[1].split('_')[0]) : 0 : 0;
        //line changed for considering refunds
        var currentPaymentAmount = $('#PurchasePayment_Amount').val() != '' && $('#PurchasePayment_TransactionType option:selected').val() == '1' ? parseFloat($('#PurchasePayment_Amount').val()) : (parseFloat($('#PurchasePayment_Amount').val()) * -1);

        $.each(currentExchangeRates, function (index, item) {
            if (item.split('=')[0] == $('#PurchasePayment_Currency option:selected').val()) {
                currentRate = parseFloat(item.split('=')[1]);
                currentPaymentAmount = currentPaymentAmount * parseFloat(item.split('=')[1]);
            }
            //if (hasBudget != null && hasBudget[0].Value.split('|')[0] == 'true') {
            //    if (item.split('=')[0] == hasBudget[0].Value.split('|')[1].split('_')[1]) {
            //        budget = budget * parseFloat(item.split('=')[1]);
            //    }
            //}
        });
        //if (hasBudget != null && hasBudget[0].Value.split('|')[0] == 'true' && $('#PurchasePayment_Company option:selected').val() != 'null') {
        //    if (currentPaymentAmount > budget) {
        //        status = 'exceedsBudget';
        //    }
        //}
        $.ajax({
            url: '/Masterchart/GetExchangeRateOfPurchase',
            type: 'POST',
            cache: false,
            data: { purchaseID: $('#PurchaseInfo_PurchaseID').val() },
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var purchaseTotal = $('#PurchaseInfo_Total').val() != '' ? (parseFloat($('#PurchaseInfo_Total').val()) * data.exchangeRate) : 0;
                var paymentsHistory = getPaymentsHistory(data.commission);
                if (status == '') {
                    if ($('#PurchasePayment_PaymentDetailsID').val() != '' && $('#PurchasePayment_PaymentDetailsID').val() != '0') {
                        //edition of existing payment
                        if (($('#PurchasePayment_TransactionType option:selected').val() == '1' && $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') == -1) || ($('#PurchasePayment_TransactionType option:selected').val() == '2' && $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') != -1)) {
                            //verify that the update is for a payment or a refund, but not a change of transactionType
                            var _exchangeRate = parseFloat($('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(3)').text());
                            //line changed for considering refunds
                            var _amount = $('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().indexOf('-') == -1 ? parseFloat($('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0]) : 0;
                            var _previousAmount = _exchangeRate * _amount;
                            var _payments = paymentsHistory.payments - _previousAmount;

                            if (paymentsHistory.paymentsInSameCurrency > (purchaseTotal / data.exchangeRate) || paymentsHistory.payments > purchaseTotal) {
                                status = 'exceeds';
                                if (paymentsHistory.paymentsInSameCurrency > (purchaseTotal / data.exchangeRate) || paymentsHistory.payments > purchaseTotal) {
                                    status = 'paid';
                                }
                            }
                            else {
                                status = 'error';
                                //if ((_payments + currentPaymentAmount - (applyCommission == 'True' ? currentPaymentAmount - (currentPaymentAmount / (1 + (data.commission / 100))) : 0)) <= (purchaseTotal + (currentRate / 10))
                                if ((_payments + currentPaymentAmount - (applyCommission == 'True' ? currentPaymentAmount - (currentPaymentAmount / (1 + (data.commission / 100))) : 0)) <= (purchaseTotal + (data.exchangeRate / 10))
                                    //|| paymentsHistory.paymentsInSameCurrency < (purchaseTotal / data.exchangeRate)
                                    || (Math.abs(parseFloat($('#tblPurchasesPayments tbody tr.selected-row').children('td:nth-child(4)').text().trim().split(' ')[0])) - $('#PurchasePayment_Amount').val()) == 0) {
                                    status = 'valid';
                                }
                            }
                            if ($('#PurchasePayment_TransactionType option:selected').val() == '2' || _amount == currentPaymentAmount || _amount == parseFloat($('#PurchasePayment_Amount').val())) {
                                status = 'valid';
                            }
                        }
                        else {
                            status = 'Cannot change Transaction Type';
                        }
                    }
                    else {
                        if (paymentsHistory.paymentsInSameCurrency >= (purchaseTotal / data.exchangeRate) || paymentsHistory.payments >= purchaseTotal) {
                            status = 'exceeds';
                            if (paymentsHistory.paymentsInSameCurrency == (purchaseTotal / data.exchangeRate) || paymentsHistory.payments == purchaseTotal) {
                                status = 'paid';
                            }
                        }
                        else {
                            status = 'error';
                            //if (paymentsHistory.payments + (currentPaymentAmount - (applyCommission == 'True' ? currentPaymentAmount - (currentPaymentAmount / (1 + (data.commission / 100))) : 0)) <= (purchaseTotal + (currentRate / 10))
                            if (paymentsHistory.payments + (currentPaymentAmount - (applyCommission == 'True' ? currentPaymentAmount - (currentPaymentAmount / (1 + (data.commission / 100))) : 0)) <= (purchaseTotal + (data.exchangeRate / 10))
                                || paymentsHistory.paymentsInSameCurrency + (parseFloat($('#PurchasePayment_Amount').val()) - (applyCommission == 'True' ? parseFloat($('#PurchasePayment_Amount').val()) - (parseFloat($('#PurchasePayment_Amount').val()) / (1 + (data.commission / 100))) : 0)) <= parseFloat($('#PurchaseInfo_Total').val())
                            ) {
                                status = 'valid';
                            }
                        }
                        if ($('#PurchasePayment_TransactionType option:selected').val() == '2') {
                            status = 'valid';
                        }



                        //save of new payment
                        //if ((paymentsHistory.payments + currentPaymentAmount - (applyCommission == 'True' ? currentPaymentAmount - (currentPaymentAmount / (1 + (data.commission / 100))) : 0)) <= (purchaseTotal + (currentRate / 10))
                        //    || paymentsHistory.paymentsInSameCurrency < (purchaseTotal/data.exchangeRate)
                        //    || $('#PurchasePayment_TransactionType option:selected').val() == '2') {
                        //    status = 'valid';
                        //}
                        //else {
                        //    if (paymentsHistory.payments >= purchaseTotal) {
                        //        status = 'paid';
                        //    }
                        //        //else if ((paymentsInfo.payments + currentPaymentAmount) > paymentsInfo.total) {
                        //    else {
                        //        status = 'exceeds';
                        //    }
                        //}
                    }
                }
                switch (status) {
                    case 'valid': {
                        savePayment('frmPurchasePaymentInfo', changeStatusFlag);
                        break;
                    }
                    case 'exceeds': {
                        UI.messageBox(0, 'Payments Amount Exceeds Total Required', null, null);
                        break;
                    }
                    case 'paid': {
                        UI.messageBox(0, 'Purchase is Paid', null, null);
                        break;
                    }
                    case 'exceedsBudget': {
                        UI.messageBox(0, "Amount Exceeds Team's Budget", null, null);
                        break;
                    }
                    case 'error': {
                        UI.messageBox(-1, 'An Error has ocurred processing the payment', null, null)
                    }
                    default: {
                        UI.messageBox(-1, status, null, null);
                        break;
                    }
                }
            }
        });
    }

    function padNumber(number, positions, symbol) {
        symbol = symbol || '0';
        number = number + '';
        return number.length >= positions ? number : new Array(positions - number.length + 1).join(symbol) + number;
    }

    function altDateFormat(sourceFieldID, destFieldID, params, service) {
        var altDate = '';
        if ($('#' + sourceFieldID).datepicker('getDate') != null) {
            var day = $('#' + sourceFieldID).datepicker('getDate').getDate();
            var month = $('#' + sourceFieldID).datepicker('getDate').getMonth();
            var monthNames = $('#' + sourceFieldID).datepicker('option', 'monthNamesShort');
            altDate = monthNames[month] != undefined && padNumber(day, 2) != undefined ? monthNames[month] + '-' + padNumber(day, 2) : '';
        }
        $('#' + destFieldID).text(altDate);
        if (service != undefined) {
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'service', itemID: service + '|' + $('#' + sourceFieldID).val() }, function (data) {
                $('#PurchaseService_WeeklyAvailability').fillSelect(data);

                var _schedule = params != undefined ? params.schedule : 0;
                if (_schedule != 0) {
                    if (params.scheduleString != '00:00 a.m.') {
                        $('#PurchaseService_WeeklyAvailability option').filter(function () { return $(this).html() == params.scheduleString; }).attr('selected', true);
                    }
                    else {
                        $('#PurchaseService_WeeklyAvailability option[value="' + _schedule + '"]').attr('selected', true);
                    }
                }
                else {

                    if ($('#PurchaseService_WeeklyAvailability option').length == 2) {

                        $('#PurchaseService_WeeklyAvailability option:last').attr('selected', true);
                    }
                }
                $('#PurchaseService_WeeklyAvailability').trigger('change', params);
            });
        }
    }

    function setTotalOfCouponsSold(discount, isPercentage, total) {
        var subtotal = 0;
        $('#tblPrices tbody tr').each(function () {
            //subtotal += parseFloat($(this).children('td:nth-child(5)').find('.money-amount').text().replace(',',''));
            subtotal += parseFloat($(this).children('td:nth-child(5)').text().replace(',', '').replace('$', ''));
        });
        //actualiza visiblemente el total de compras conforme se asignan cupones
        subtotal = total != undefined ? total : subtotal;
        $('#PurchaseService_Total').val(UI.addDecimalPart(subtotal));
        $('#lblPurchaseService_Total').html($('#PurchaseService_Total').val());
        UI.applyFormat('currency', 'tblPrices');
    }

    function blockFormEdition(serviceStatus, isClosed, avoidEdition, isAudited) {
        $('#frmPurchaseServiceInfo .field-disabled:not(.not-editable)').removeAttr('readonly');
        $('#frmPurchaseServiceInfo input').not('.button').not('.submit').not('.updatable-content').removeClass('field-disabled');
        $('#frmPurchaseServiceInfo select').each(function () {
            $(this).unbind('mousedown');
        });
        $('#frmPurchaseServiceInfo select').not('.updatable-content').removeClass('field-disabled');
        $('.removable-content').text('');
        $('#PurchaseService_ServiceDateTime').datepicker({
            'changeMonth': true,
            'changeYear': true,
            'dateFormat': 'yy-mm-dd',
            beforeShowDay: PURCHASE.closedDays,
            onClose: function (dateText, inst) {
                altDateFormat('PurchaseService_ServiceDateTime', 'altDateFormat', undefined, $('#PurchaseService_Service').val());
                var params = { service: $('#PurchaseService_Service').val(), datetime: dateText, promo: $('#PurchaseService_Promo').val() };
                PURCHASE.updatePromosPerServiceDate(params);
            }
        });
        $('#PurchaseService_CustomMeetingTime').datetimepicker({
            timeFormat: 'HH:mm',
            timeOnly: true,
            stepMinute: 5
        });
        $('#PurchaseService_ServiceStatus option').each(function () {
            $(this).removeAttr('style');
        });
        $('#PurchaseService_CanceledByUser').removeAttr('disabled');
        $('#PurchaseService_ConfirmedByUser').removeAttr('disabled');
        //muestra los campos de unidades
        $('#divAddPricesContainer').show();
        $('.cancelation-info-container').hide();
        $('#aGetCoupon').removeAttr('href');
        $('#btnGetCoupon').hide();
        $('.delete-item').show();
        PURCHASE.deleteItemFromTable();
        //status confirmado o cancelado
        if (serviceStatus >= 3) {
            //---bloqueo de formulario
            if (serviceStatus >= 4) {
                //canceled, refunded, no show
                $('#frmPurchaseServiceInfo input').not('.button').not('.submit').not('.updatable-content').addClass('field-disabled');
                $('#frmPurchaseServiceInfo select').not('.updatable-content').addClass('field-disabled');
                $('#frmPurchaseServiceInfo select').not('.updatable-content').on('mousedown', function (e) {
                    e.preventDefault();
                });
                $('.cancelation-info-container').show();
                $('#btnGetCoupon').hide();
            }
            else {
                //confirmed
                //add .field-disabled to all input fields that aren't .button, .submit, .update-on-confirmed, .updatable-content
                $('#frmPurchaseServiceInfo input').not('.button').not('.submit').not('.update-on-confirmed').not('.updatable-content').addClass('field-disabled');
                //add .field-disabled to all select that aren't .update-on-confirmed
                $('#frmPurchaseServiceInfo select').not('.update-on-confirmed').addClass('field-disabled');
                //prevent selection of items in all selects that aren't .update-on-confirmed
                $('#frmPurchaseServiceInfo select').not('.update-on-confirmed').on('mousedown', function (e) {
                    e.preventDefault();
                });
                //shows button of get coupon
                $('#btnGetCoupon').show();
                //remove readonly attribute and .field-disabled from all .update-on-confirmed
                $('#frmPurchaseServiceInfo .update-on-confirmed').removeClass('field-disabled').removeAttr('readonly');
                //restore selection of items of all .update-on-confirmed selects
                $('#frmPurchaseServiceInfo select .update-on-confirmed').each(function () {
                    $(this).unbind('mousedown');
                });
            }
            //cambio hecho debido a que se permitirá la edicion de los campos aún con el status confirmado
            //if ($('#PurchaseService_ServiceDateTime').datepicker() && serviceStatus >= 4) {
            if ($('#PurchaseService_ServiceDateTime').datepicker() && serviceStatus >= 3) {//este cambio es temporal pues se necesita ajustar la edición de acuerdo a los precios vendidos y los disponibles
                $('#PurchaseService_ServiceDateTime').datepicker('destroy');
                $('#PurchaseService_CustomMeetingTime').datepicker('destroy');
            }
            if ($('#PurchaseService_ConfirmedByUser option:selected').val() == $('#uid').val() && !isClosed) {
                $('#PurchaseService_ConfirmedByUser').removeAttr('disabled');
            }
            else if (($('#divSelectedRole').text().indexOf('Administrator') != -1 || $('#divSelectedRole').text().indexOf('Operation Manager') != -1 || $('#divSelectedRole').text().indexOf('Comptroller') != -1) && !isClosed) {
                $('#PurchaseService_ConfirmedByUser').removeAttr('disabled');
            }
            else {
                $('#PurchaseService_ConfirmedByUser').attr('disabled', 'disabled');
            }
            //aquí se inhabilita el campo canceled by user
            if ($('#PurchaseService_CanceledByUser option:selected').val() == $('#uid').val()) {
                $('#PurchaseService_CanceledByUser').removeAttr('disabled');
            }
            else if ($('#divSelectedRole').text().indexOf('Administrator') != -1 || $('#divSelectedRole').text().indexOf('Operation Manager') != -1 || $('#divSelectedRole').text().indexOf('Comptroller') != -1 || $('#divSelectedRole').text().indexOf('External Reservations Supervisor') != -1 || $('#divSelectedRole').text().indexOf('Accounting C') != -1) {
                $('#PurchaseService_CanceledByUser').removeAttr('disabled');
            }
            else {
                $('#PurchaseService_CanceledByUser').attr('disabled', 'disabled');
            }
            //---
            //---allows specific roles to modify cancelation date
            var confirmationDate = $('#PurchaseService_ConfirmationDateTime').val();
            var confirmationDateMonth = new Date(confirmationDate).getMonth();
            var confirmationDateYear = new Date(confirmationDate).getFullYear();
            if (($('#divSelectedRole').text().indexOf('Administrative Assistant MCA') != -1 || $('#divSelectedRole').text().indexOf('Administrator') != -1 || $('#divSelectedRole').text().indexOf('Operation Manager') != -1) && confirmationDateMonth == new Date().getMonth() && confirmationDateYear == new Date().getFullYear() && $('#PurchaseService_CancelationDateTime').val() != '') {
                $('#PurchaseService_CancelationDateTime').datepicker({
                    dateFormat: 'yy-mm-dd',
                    minDate: confirmationDate,
                    maxDate: new Date(confirmationDateYear, (confirmationDateMonth + 1), 0)
                });
            }
            else if ($('#PurchaseService_CancelationDateTime').datepicker()) {
                $('#PurchaseService_CancelationDateTime').datepicker('destroy');
            }
            //---
            //ocultar los campos de unidades
            $('#divAddPricesContainer').hide();

            //deja visibles sólo las opciones mayores o iguales a confirmado
            $('#PurchaseService_ServiceStatus option').each(function () {
                if ($(this).val() < 3) {
                    $(this).css('display', 'none');
                }
            });

            var _confirmedDate = $('#PurchaseService_CancelationDateTime').val() != '' ? new Date($('#PurchaseService_CancelationDateTime').val()) : new Date($('#PurchaseService_ConfirmationDateTime').val());//check if it will consider confirmation or cancelation date
            if ((new Date(COMMON.serverDateTime.getFullYear() + '-' + (COMMON.serverDateTime.getMonth() + 1) + '-' + COMMON.serverDateTime.getDate()).getTime() == new Date(_confirmedDate.getFullYear() + '-' + (_confirmedDate.getMonth() + 1) + '-' + _confirmedDate.getDate()).getTime()
                && !isClosed) || (isClosed && avoidEdition)) {
                //&& !isClosed) || (isClosed && allowEdition== false)) {
                //&& !isClosed) {
                $('#PurchaseService_ServiceStatus option[value="5"]').css('display', 'none');
            }
            //si el cupon está seleccionado y el status está cancelado, se esconde la opción confirmado
            if ($('#PurchaseService_PurchaseServiceID').val() != '' && serviceStatus >= 4) {
                $('#PurchaseService_ServiceStatus option[value="6"]').css('display', 'none');
                if (new Date(COMMON.serverDateTime.getFullYear() + '-' + (COMMON.serverDateTime.getMonth() + 1) + '-' + COMMON.serverDateTime.getDate()).getTime() != new Date(_confirmedDate.getFullYear() + '-' + (_confirmedDate.getMonth() + 1) + '-' + _confirmedDate.getDate()).getTime()
                    || isClosed) {
                    $('#PurchaseService_ServiceStatus option[value="3"]').css('display', 'none');
                }
                //permitir reconfirmación de cupones por administrador
                if ($('#divSelectedRole').text().indexOf('Administrator') != -1 && !isClosed) {
                    $('#PurchaseService_ServiceStatus option[value="3"]').css('display', 'inline');
                }
                //permitir reconfirmar noShow por contralores
                if (($('#divSelectedRole').text().indexOf('Comptroller') != -1 || $('#divSelectedRole').text().indexOf('Administrator') != -1) && !isAudited && serviceStatus == 6) {
                    $('#PurchaseService_ServiceStatus option[value="3"]').css('display', 'inline');
                }
            }
            $('.delete-item').hide().unbind('click');
            UI.disableFieldWithClass();
        }
        else {
            $('#PurchaseService_ServiceDateTime').unbind('mousedown');
            $('#PurchaseService_ServiceDateTime').datepicker('destroy');
            $('#PurchaseService_ServiceDateTime').datepicker({
                'changeMonth': true,
                'changeYear': true,
                'dateFormat': 'yy-mm-dd',
                constrainInput: true,
                beforeShowDay: PURCHASE.closedDays,
                onClose: function (dateText, inst) {
                    onCloseServiceDate(dateText);
                }
            });
            var exists = false;
            $('#PurchaseService_ConfirmedByUser option').each(function () {
                if ($('#uid').val() == $(this).val()) {
                    exists = true;
                }
            });
            if (exists == false) {
                $('#PurchaseService_ConfirmedByUser').append('<option value="' + $('#uid').val() + '">' + $('#ufirstname').val() + ' ' + $('#ulastname').val() + '</option>');
            }
        }
    }

    function printCoupon(url) {
        var purchaseServiceID = UI.parseURL(url).pathname.split('-').reverse()[0];

        $.ajax({
            url: '/MasterChart/SetPurchaseServiceAsIssued',
            cache: false,
            type: 'POST',
            data: { PurchaseService_PurchaseServiceID: purchaseServiceID },
            success: function (data) {
                if (data.Issued == true) {
                    $('.printable:not(#printableCoupon)').addClass('non-printable');
                    if ($.parseJSON($('#PurchaseInfo_FlagsByTerminalPurchase').val()).printInColumn == true) {
                        url += '#print';
                    }
                    else {
                        url += '#a4-' + $('#PurchaseInfo_Terminal option:selected').val();
                    }

                    if ($('#tblPurchasesServices').data('reprint').toString().toLowerCase() != 'true' && $('#tblPurchasesServices').attr('data-reprint').toString().toLowerCase() != 'true') {
                        PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + purchaseServiceID).find('.print-coupon').remove();
                        if ($('#tblPurchasesServices').data('resend').toString().toLowerCase() != 'true' && $('#tblPurchasesServices').attr('data-resend').toString().toLowerCase() != 'true') {
                            PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + purchaseServiceID).find('.get-coupon').remove();
                        }
                    }
                    $('#printableCoupon').removeClass('non-printable').printPage(url);
                }
                else {
                    UI.messageBox(0, 'There was a problem with the coupon issue process.<br />Please contact System Administrator', null, null);
                }
            }
        });
    }

    function _printCoupon(url) {//setcouponasissued
        //var _serviceID = url.split('-').reverse()[0];
        var _serviceID = UI.parseURL(url).pathname.split('-').reverse()[0];
        if ((PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).data('issued').toString().toLowerCase() != 'true' && PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).attr('data-issued').toString().toLowerCase() != 'true') || ($('#tblPurchasesServices').data('reprint').toString().toLowerCase() != 'false' && $('#tblPurchasesServices').attr('data-reprint').toString().toLowerCase() != 'false')) {
            $.ajax({
                url: '/MasterChart/SetPurchaseServiceAsIssued',
                cache: false,
                type: 'POST',
                data: { PurchaseService_PurchaseServiceID: _serviceID },
                //beforeSend: function (xhr) {
                //    UI.checkForPendingRequests(xhr);
                //},
                success: function (data) {
                    if (data.Issued == true) {

                        PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).attr('data-issued', 'true');
                        PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).data('issued', 'true');
                        $('.printable:not(#printableCoupon)').addClass('non-printable');
                        if ($.parseJSON($('#PurchaseInfo_FlagsByTerminalPurchase').val()).printInColumn == true) {
                            url += '#print';
                        }
                        else {
                            url += '#a4-' + $('#PurchaseInfo_Terminal option:selected').val();
                        }
                        if ($('#tblPurchasesServices').data('reprint').toString().toLowerCase() != 'true' && $('#tblPurchasesServices').attr('data-reprint').toString().toLowerCase() != 'true') {
                            PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).find('.print-coupon').remove();
                            if ($('#tblPurchasesServices').data('resend').toString().toLowerCase() != 'true' && $('#tblPurchasesServices').attr('data-resend').toString().toLowerCase() != 'true') {
                                PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).find('.get-coupon').remove();
                            }
                        }
                        $('#printableCoupon').removeClass('non-printable').printPage(url);
                        //$('#printableCoupon').removeClass('non-printable').printPage(url);
                    }
                    else {
                        UI.messageBox(0, 'There was a problem with the coupon issue process.<br />Please contact System Administrator', null, null);
                    }
                }
            });
        }
        else {
            UI.messageBox(0, 'Coupon has been issued', null, null);
        }
    }

    function sendCouponsByEmail(purchaseID, serviceID) {
        var couponsArray = new Array();
        if (serviceID != undefined && ((PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + serviceID).data('issued').toString().toLowerCase() != 'true' && PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + serviceID).attr('data-issued').toString().toLowerCase() != 'true') || ($('#tblPurchasesServices').data('resend').toString().toLowerCase() != 'false' && $('#tblPurchasesServices').attr('data-resend').toString().toLowerCase() != 'false'))) {
            couponsArray.push(parseInt(serviceID));
        }
        else if (serviceID != undefined) {
            UI.messageBox(0, 'Coupon has been issued', null, null);
        }
        else {
            $('#chkSendProvider').attr('checked', false);
            $('#chkSendOther').attr('checked', false);
            $('#txtSendOther').val('');
            PURCHASE.oPurchaseServiceTable.$('tr[data-issued="false"]').find('.get-coupon').each(function (i) {
                couponsArray.push(parseInt($(this).parents('tr:first').attr('id').split('_')[1]));
            });
        }

        if (couponsArray.length > 0) {
            $.ajax({
                url: '/MasterChart/SendCouponsByEmail',
                type: 'POST',
                cache: false,
                //data: { PurchaseService_Purchase: purchaseID, PurchaseService_Service: (serviceID != undefined ? serviceID : 0), SendToProvider: $('#chkSendProvider').is(':checked'), SendToOther: $('#chkSendOther').is(':checked'), OtherMail: $('#txtSendOther').val() },
                data: { PurchaseService_Purchase: purchaseID, PurchaseService_Service: couponsArray.toString(), SendToProvider: $('#chkSendProvider').is(':checked'), SendToOther: $('#chkSendOther').is(':checked'), OtherMail: $('#txtSendOther').val() },
                //beforeSend: function (xhr) {
                //    UI.checkForPendingRequests(xhr);
                //},
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    if (data.ItemID.Coupons.length > 0) {
                        for (var i = 0; i < data.ItemID.Coupons.length; i++) {
                            PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + data.ItemID.Coupons[i]).data('issued', 'true');
                            PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + data.ItemID.Coupons[i]).attr('data-issued', 'true');
                            if ($('#tblPurchasesServices').data('resend').toString().toLowerCase() != 'true' && $('#tblPurchasesServices').attr('data-resend').toString().toLowerCase() != 'true') {
                                PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + data.ItemID.Coupons[i]).find('.get-coupon').remove();
                                if ($('#tblPurchasesServices').data('reprint').toString().toLowerCase() != 'true' && $('#tblPurchasesServices').attr('data-reprint').toString().toLowerCase() != 'true') {
                                    PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + data.ItemID.Coupons[i]).find('.print-coupon').remove();
                                }
                            }
                        }
                    }
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        }
        else {
            UI.messageBox(0, 'There are no pending coupons to send', 0, null);
        }
    }

    var searchCouponsResult = function (response) {
        if (response != '[]') {
            PURCHASE.updateTblRecentCouponsAndJson(response);
        }

        COMMON.collapseAndExpand('#fdsLeadSearch, #fdsAccountancyManagement', '#fdsLeadSearchResults').done(COMMON.executeDelayed(UI.scrollTo, "fdsLeadSearchResults"));
        //autoselect row when its only one
        if (PURCHASE.oCouponsTable != undefined && PURCHASE.oCouponsTable.$('tr').length == 1) {
            PURCHASE.oCouponsTable.$('tr:first').click();
        }
        UI.Notifications.workingOn("CRM > MasterChart");
    }

    var searchCouponsByDateResult = function (response) {
        PURCHASE.loadCouponsTableFromJson(response);
        COMMON.collapseAndExpand('#fdsLeadSearch, #fdsAccountancyManagement', '#fdsLeadSearchResults').done(COMMON.executeDelayed(UI.scrollTo, "fdsLeadSearchResults"));
        //autoselect row when its only one
        if (PURCHASE.oCouponsTable != undefined && PURCHASE.oCouponsTable.$('tr').length == 1) {
            PURCHASE.oCouponsTable.$('tr:first').click();
        }
        UI.Notifications.workingOn("CRM > MasterChart");
    }

    var loadCouponsTableFromJson = function (response) {
        var data = new Array();
        var json;
        if (response != undefined) {
            //response != undefined means that the search is from "Activities" tab
            json = $.parseJSON(response);
            $.each(json, function (i, item) {
                data = data.concat(item.Coupons);
            });
        }
        else {
            //response == undefined means that the search is from "Sales" tab
            json = $.parseJSON(localStorage.Eplat_RecentCoupons);
            $.each(json, function (i, item) {
                if (item.PointOfSale == $('#SearchCoupon_PointOfSale option:selected').val() && (item.DateSaved == $('#SearchCoupon_PurchaseDate').val() || $('#SearchCoupon_Coupon').val() != '') && item.DateLastUpdate == COMMON.getDate()) {
                    data = item.Coupons;
                    return false;
                }
            });
        }

        PURCHASE.oCouponsTable = $('#tblSearchCouponsResults').dataTable({
            "bDestroy": true,
            "bFilter": true,
            "bProcessing": true,
            "bAutoWidth": false,
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            },
            "aaData": eval(data),//this line will change in order of the new object's model
            "aoColumns": [
                { "mData": "RecentCoupon_Coupon" },
                { "mData": "RecentCoupon_FirstName" },
                { "mData": "RecentCoupon_ServiceStatus" },
                { "mDataProp": "RecentCoupon_Total", "sClass": "format-currency" },
                { "mData": "RecentCoupon_DateSaved" },
                { "mData": "RecentCoupon_ServiceDate" },
                { "mData": "RecentCoupon_Location" }],
            "aaSorting": [[0, 'desc']],
            "aoRowCallback": [UI.applyFormat('currency', 'tblSearchCouponsResults')]
        });
        UI.applyFormat('currency', 'tblSearchCouponsResults');
        UI.setTableRowsClickable({
            tblID: "tblSearchCouponsResults",
            onClickCallbackFunction: REQUEST_HANDLERS.findLead
        });
    }

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblPurchases', tableColumns.length - 1);
        PURCHASE.oPurchaseTable = $('#tblPurchases').dataTable();
        PURCHASE.oPurchaseTable.fnSort([[2, 'desc']]);
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { PURCHASE.makeTblPurchasesSelectable(); });
        //});
        //$('#tblPurchases_length').unbind('change').on('change', function () {
        //    PURCHASE.makeTblPurchasesSelectable();
        //});
    }

    var searchResultsPurchaseServicesTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblPurchasesServices', tableColumns.length - 1);
        PURCHASE.oPurchaseServiceTable = $('#tblPurchasesServices').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { PURCHASE.makeTblPurchaseServiceRowsSelectable(); });
        //});
        //$('#tblPurchasesServices_length').unbind('change').on('change', function () {
        //    PURCHASE.makeTblPurchaseServiceRowsSelectable();
        //});
    }

    var searchResultsPurchasePaymentsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblPurchasesPayments', tableColumns.length - 1);
        PURCHASE.oPurchasePaymentTable = $('#tblPurchasesPayments').dataTable();
        if (PURCHASE.oPurchasePaymentTable != undefined && PURCHASE.oPurchasePaymentTable.length > 0) {
            var oSettings = PURCHASE.oPurchasePaymentTable.fnSettings();
            oSettings._iDisplayLength = 100;
            PURCHASE.oPurchasePaymentTable.fnSort([[5, 'asc']]);
            PURCHASE.oPurchasePaymentTable.fnDraw();
        }
    }

    var show = function (leadID) {
        if ($('#fdsPurchasesManagement').length > 0) {
            $('#fdsPurchasesManagement').show();
            UI.expandFieldset('fdsPurchasesManagement');
            PURCHASE.updateTblPurchases(leadID);

            $('#customerName').text($('#GeneralInformation_Title option:selected').text() + ' ' + $('#GeneralInformation_FirstName').val() + ' ' + $('#GeneralInformation_LastName').val());
            //try {
            //    $('#' + leadID).children('td:nth-child(1)')[0].textContent = $('#GeneralInformation_FirstName').val();
            //    $('#' + leadID).children('td:nth-child(2)')[0].textContent = $('#GeneralInformation_LastName').val();
            //} catch (ex) { }
            UI.updateDependentLists('/MasterChart/GetDDLData', 'billingInfo', leadID, false);
        }
    }

    var deleteItemFromTable = function () {
        $('.delete-item').unbind('click').on('click', function () {
            $(this).parents('tr').remove();
            if ($('#tblPrices tbody tr:first').children('td:nth-child(7)').text() != '') {
                $('#tblPrices tbody tr').each(function () {
                    $(this).children('td:nth-child(7)').text($(this).children('td:nth-child(7)').text().substr(0, $(this).children('td:nth-child(7)').text().indexOf('-') + 1) + String.fromCharCode(65 + $(this).index()));
                });
            }
            if ($('#tblPrices tbody tr').length == 0) {
                restrictPriceTypes(0);
            }
            setTotalOfCouponsSold();
        });

        $('.edit-item').unbind('click').on('click', function () {
            var $this = $(this);
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'minimalPrice', itemID: ($('#PurchaseService_PurchaseServiceID').val() + '|' + $(this).parents('tr:first').children('td:nth-child(3)').children(':hidden:nth-child(2)').val()) }, function (data) {
                $this.parents('tr:first').find('.edition-tools').toggle();
                $this.parents('tr:first').children('td.allow-edition').each(function (index, item) {
                    var _value = $(this).find('.money-amount').length > 0 ? $(this).find('.money-amount').text().trim() : $(this).text().trim();
                    $(this).attr('data-previous-value', _value);
                    $(this).html('<input type="text" style="width:50px" value="' + _value + '"/>');
                });

                $('.discard').unbind('click').on('click', function () {
                    $(this).parents('tr:first').find('.edition-tools').toggle();
                    $(this).parents('tr:first').children('td.allow-edition').each(function (index, item) {
                        $(this).html('');
                        $(this).text($(this).attr('data-previous-value'));
                    });
                    UI.applyFormat('currency', 'tblPrices');
                });

                $('.confirm').unbind('click').on('click', function () {
                    if (data[0] != undefined && parseFloat($(this).parents('tr:first').children('td:nth-child(4)').children(':text').val()) < parseFloat(data[0].Value.split('|')[1])) {
                        UI.messageBox(0, 'New price CANNOT be lower than price set as minimal: $' + UI.addDecimalPart(data[0].Value.split('|')[1]), null, null);
                    }
                    else {
                        $(this).parents('tr:first').find('.edition-tools').toggle();
                        $(this).parents('tr:first').children('td.allow-edition').each(function (index, item) {
                            var _value = $(this).children('input:text').val();
                            $(this).html('');
                            $(this).text(_value);
                        });
                        $(this).parents('tr:first').children('td:nth-child(5)').html('');
                        //var _price = parseFloat($(this).parents('tr:first').children('td:nth-child(2)').text().trim()) * parseFloat($(this).parents('tr:first').children('td:nth-child(4)').text());
                        var _price = parseFloat($(this).parents('tr:first').children('td:nth-child(4)').text());
                        var _subtotal = parseFloat($(this).parents('tr:first').children('td:nth-child(2)').text().trim()) * _price;
                        $(this).parents('tr:first').children('td:nth-child(1)').find(':hidden:first').val(_price);
                        $(this).parents('tr:first').children('td:nth-child(5)').text(_subtotal);
                        UI.applyFormat('currency', 'tblPrices');
                        setTotalOfCouponsSold();
                    }
                });
            });
        });
    }

    var updateTblPurchases = function (leadID) {
        $('#PurchaseInfo_Lead').val(leadID);
        $.ajax({
            url: '/MasterChart/GetLeadPurchases',
            cache: false,
            type: 'GET',
            data: { PurchaseInfo_Lead: $('#PurchaseInfo_Lead').val() },
            contentType: 'application/html; charset=utf-8',
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                $('#tblExistingPurchasesContainer').html(data);
                PURCHASE.searchResultsTable('tblPurchases');
            },
            complete: function () {
                PURCHASE.makeTblPurchasesSelectable();
                $('#PurchaseService_ReservedFor').val($('#GeneralInformation_FirstName').val() + ' ' + $('#GeneralInformation_LastName').val()).attr('data-keep-value', '');
                if ($('#tblPurchases tbody tr').attr('id') != undefined) {
                    $('#tblPurchases tbody tr:first').click();
                }
                UI.applyFormat('currency', 'tblPurchases');
            }
        });
    }

    var makeTblPurchasesSelectable = function () {
        PURCHASE.oPurchaseTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            $('#customerBanner').asFixed({ 'position': 'fixed', 'top': '21px', 'background-color': 'rgb(241, 241, 241)', 'color': '#484848', 'width': $('#main').width() + 40, 'height': '70px', 'padding': '20px 0 0 20px', 'z-index': '2', 'left': '0', 'box-shadow': '0 0 5px #a5a5a5' });
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    PURCHASE.oPurchaseTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/MasterChart/GetPurchase',
                        cache: false,
                        type: 'POST',
                        data: { PurchaseInfo_PurchaseID: $(this).attr('id').substr($(this).attr('id').indexOf('_') + 1) },
                        success: function (data) {
                            window.location.hash = 'purchaseid=' + data.PurchaseInfo_PurchaseID;
                            $('#PurchaseInfo_PurchaseID').val(data.PurchaseInfo_PurchaseID);
                            $('#PurchaseInfo_BankCommission').val(data.PurchaseInfo_BankCommission);
                            $('#PurchaseInfo_FlagsByTerminalPurchase').val(data.PurchaseInfo_FlagsByTerminalPurchase);
                            $('#PurchaseInfo_AllowUnitsEdition').val(data.PurchaseInfo_AllowUnitsEdition);
                            ///////////
                            var terminalFlags = $.parseJSON($('#PurchaseInfo_FlagsByTerminalPurchase').val());
                            var style = document.createElement('link');
                            style.rel = 'stylesheet';
                            style.type = 'text/css';
                            style.setAttribute('class', terminalFlags.className);
                            style.setAttribute('class', 'print-css');
                            var terminalid = data.PurchaseInfo_Terminal;
                            var _timespan = new Date().getTime();
                            if (terminalFlags.printInColumn) {
                                style.href = "//eplatfront.villagroup.com/content/themes/mex/css/coupon-ticket.css?v=" + _timespan;
                            } else {
                                switch (terminalid) {
                                    case 26: {
                                        style.href = "//eplatfront.villagroup.com/content/themes/mex/css/coupon-page-vea.css?v=" + _timespan;
                                        break;
                                    }
                                    case 32: {
                                        style.href = "//eplatfront.villagroup.com/content/themes/mex/css/coupon-page-mca.css?v=" + _timespan;
                                        break;
                                    }
                                    default: {
                                        style.href = "//eplatfront.villagroup.com/content/themes/mex/css/coupon-page.css?v=" + _timespan;
                                        break;
                                    }
                                }
                            }
                            if (document.getElementsByClassName(terminalFlags.className)[0] == undefined) {
                                if (document.getElementsByClassName('print-css')[0] != undefined) {
                                    document.getElementsByTagName('head')[0].removeChild(document.getElementsByClassName('print-css')[0]);
                                }
                                document.getElementsByTagName('head')[0].appendChild(style);
                            }
                            //////////////////////

                            $('#spanCurrentChargeCommission').html($('#PurchaseInfo_BankCommission').val() + ' %');
                            if ($.parseJSON($('#PurchaseInfo_FlagsByTerminalPurchase').val()).bankCommission > 0) {
                                $('.bank-commission-dependent:not(span)').show();
                            }
                            else {
                                $('.bank-commission-dependent').hide();
                            }
                            $('#PurchaseInfo_Culture option[value="' + data.PurchaseInfo_Culture + '"]').attr('selected', true);
                            $('#PurchaseInfo_PurchaseDateTime').val(data.PurchaseInfo_PurchaseDateTime);
                            $('#PurchaseInfo_Promo option[value="' + data.PurchaseInfo_Promo + '"]').attr('selected', true);
                            //if (data.PurchaseInfo_Promo != '0' && data.PurchaseInfo_Promo != 0) {
                            //    $.getJSON('/MasterChart/GetPromoInfo', { promoID: data.PurchaseInfo_Promo }, function (data) {
                            //        $('#hdnPurchasePromo').val(JSON.stringify(data));
                            //    });
                            //}
                            //else {
                            //    $('#hdnPurchasePromo').val('');
                            //}
                            //$('#PurchaseInfo_Promo').trigger('change');
                            $('#PurchaseInfo_Terminal option[value="' + data.PurchaseInfo_Terminal + '"]').attr('selected', true);
                            $('#PurchaseInfo_Terminal').trigger('change', { stayingAtPlace: data.PurchaseInfo_StayingAtPlace, stayingAt: data.PurchaseInfo_StayingAt, agent: data.PurchaseInfo_Agent, user: data.PurchaseInfo_User, promo: data.PurchaseInfo_Promo });
                            $('#PurchaseInfo_CustomerRequests').val(data.PurchaseInfo_CustomerRequests);
                            $('#PurchaseInfo_Total').val(data.PurchaseInfo_Total);
                            $('#lblPurchaseInfo_Total').html($('#PurchaseInfo_Total').val());
                            UI.applyFormat('currency');
                            $('#PurchaseInfo_Currency option[value="' + data.PurchaseInfo_Currency + '"]').attr('selected', true);
                            $('#PurchaseInfo_Currency').trigger('change');
                            $('#PurchaseInfo_PurchaseStatus option[value="' + data.PurchaseInfo_PurchaseStatus + '"]').attr('selected', true);
                            $('#PurchaseInfo_Agent option[value="' + (data.PurchaseInfo_Agent != null ? data.PurchaseInfo_Agent : '') + '"]').attr('selected', true);
                            $('#PurchaseInfo_PurchaseComments').val(data.PurchaseInfo_PurchaseComments);
                            $('#PurchaseInfo_Notes').val(data.PurchaseInfo_Notes);
                            $('#PurchaseInfo_Feedback').val(data.PurchaseInfo_Feedback);
                            $('#PurchaseInfo_User option[value="' + (data.PurchaseInfo_User != null ? data.PurchaseInfo_User : '') + '"]').attr('selected', true);
                            $('#PurchaseInfo_PointOfSale option[value="' + data.PurchaseInfo_PointOfSale + '"]').attr('selected', true);
                            $('#PurchaseInfo_Location option[value="' + data.PurchaseInfo_Location + '"]').attr('selected', true);
                            //manifest fields
                            $('#PurchaseInfo_MarketingProgram').val(data.MarketingProgram);
                            $('#PurchaseInfo_Subdivision').val(data.Subdivision);
                            $('#PurchaseInfo_Source').val(data.Source);
                            $('#PurchaseInfo_OPCID').val(data.OPCID);
                            $('#PurchaseInfo_SpiOPCID').val(data.SpiOPCID);
                            $('#PurchaseInfo_TourID').val(data.TourID);

                            $.getJSON('/MasterChart/GetDDLData', { itemType: 'paymentTypesPerLocation', itemID: data.PurchaseInfo_PointOfSale }, function (data) {
                                $('#PurchasePayment_PaymentType').fillSelect(data);
                            });
                            $.ajax({
                                url: '/MasterChart/GetExchangeRates',
                                async: false,
                                contentType: 'JSON',
                                data: { date: ($('#PurchasePayment_DateSaved').val() != undefined ? $('#PurchasePayment_DateSaved').val() : COMMON.getDate()), pointOfSaleID: $('#PurchaseInfo_PointOfSale option:selected').val() },
                                success: function (data) {
                                    //exchangeRates = data.split(' ').join('').split(',');
                                    $('#currentExchangeRates').text(data);
                                }
                            });
                            //var valid = true;
                            //if (PURCHASE.oPurchaseServiceTable != undefined) {
                            //    PURCHASE.oPurchaseServiceTable.$('tr').not('theader').children('td:nth-child(3)').each(function () {
                            //        if ($(this).text() != '') {
                            //            valid = false;
                            //        }
                            //    });
                            //}
                            //if (!valid) {
                            //    $('#PurchaseInfo_PointOfSale').on('mousedown', function (e) {
                            //        e.preventDefault();
                            //    });
                            //    $('#PurchaseInfo_Currency').on('mousedown', function (e) {
                            //        e.preventDefault();
                            //    });
                            //}
                            //else {
                            //    $('#PurchaseInfo_PointOfSale').unbind('mousedown');
                            //    $('#PurchaseInfo_Currency').unbind('mousedown');
                            //}
                            $('#PurchaseInfo_StayingAtRoom').val(data.PurchaseInfo_StayingAtRoom);
                            $.getJSON('/crm/MasterChart/GetDDLData', { itemType: 'siblingsCoupons', itemID: data.PurchaseInfo_PurchaseID }, function (data) {
                                $('#PurchaseService_ReplacementOf').fillSelect(data);
                            });
                            if (!data.PurchaseInfo_PointOfSaleAcceptCharges) {
                                $('input[name="PurchasePayment_ApplyCharge"]')[1].checked = true;
                                $('input[name="PurchasePayment_ApplyCharge"]').trigger('change');
                                $('#divApplyCharge').hide();
                            }
                            else {
                                $('input[name="PurchasePayment_ApplyCharge"]')[0].checked = true;
                                $('input[name="PurchasePayment_ApplyCharge"]').trigger('change');
                                $('#divApplyCharge').show();
                            }
                            UI.expandFieldset('fdsPurchaseInfo');
                            UI.scrollTo('fdsPurchaseInfo', null);
                            PURCHASE.updateTblPurchaseServices($('#PurchaseInfo_PurchaseID').val());
                            PURCHASE.updateTblPurchasePayments($('#PurchaseInfo_PurchaseID').val());
                            PURCHASE.updateTblPurchaseTickets($('#PurchaseInfo_PurchaseID').val());
                        }
                    });
                }
                else {
                    if (!$('#fdsPurchaseInfo').children('div:first').is(':visible')) {
                        UI.expandFieldset('fdsPurchaseInfo');
                    }
                    UI.scrollTo('fdsPurchaseInfo', null);
                }
            }
        });
    }

    var makeTblPurchaseServiceRowsSelectable = function () {
        PURCHASE.oPurchaseServiceTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is(':button')) {
                if (!$(this).hasClass('selected-row')) {
                    PURCHASE.oPurchaseServiceTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/MasterChart/GetPurchaseService',
                        cache: false,
                        type: 'POST',
                        data: { PurchaseService_PurchaseServiceID: $(this).attr('id').substr($(this).attr('id').indexOf('_') + 1) },
                        //beforeSend: function (xhr) {
                        //    UI.checkForPendingRequests(xhr);
                        //},
                        success: function (data) {
                            $('#PurchaseService_ReplacementOf option').each(function () {
                                if ($(this).val() == $(e.target).parents('tr:first').attr('id').split('_')[1]) {
                                    $(this).hide();
                                }
                                else {
                                    $(this).show();
                                }
                            });
                            $('#hdnCurrentPurchaseService').val(JSON.stringify(data));
                            $('#PurchaseService_CurrentUnits').val(data.PurchaseService_CurrentUnits);
                            $('#PurchaseService_PurchaseServiceID').val(data.PurchaseService_PurchaseServiceID);
                            if ($('#PurchaseService_Provider option[value="' + data.PurchaseService_Provider + '"]').length == 0) {
                                $('#PurchaseService_Provider').append('<option class="option-hidden" value="' + data.PurchaseService_Provider + '">' + data.PurchaseService_ProviderText + '</option>');
                            }
                            $('#PurchaseService_Provider option[value="' + data.PurchaseService_Provider + '"]').attr('selected', true);
                            $('#PurchaseService_Provider').multiselect('refresh');
                            if ($('#PurchaseService_Service option[value="' + data.PurchaseService_Service + '"]').length == 0) {
                                $('#PurchaseService_Service').append('<option class="option-hidden" value="' + data.PurchaseService_Service + '">' + data.PurchaseService_ServiceText + '</option>');
                            }
                            $('#PurchaseService_Service option[value="' + data.PurchaseService_Service + '"]').attr('selected', true);
                            $('#PurchaseService_Service').multiselect('refresh');
                            $('#PurchaseService_Service').trigger('change', { service: data.PurchaseService_Service, schedule: data.PurchaseService_WeeklyAvailability, scheduleString: data.PurchaseService_WeeklyAvailabilityString, meetingPoint: data.PurchaseService_MeetingPoint, promo: data.PurchaseService_Promo, datetime: data.PurchaseService_ServiceDateTime, confirmedByUser: data.PurchaseService_ConfirmedByUser, canceledByUser: data.PurchaseService_CanceledByUser, openCoupon: data.PurchaseService_OpenCoupon });
                            $('#PurchaseService_ServiceStatus option[value="' + data.PurchaseService_ServiceStatus + '"]').attr('selected', true);
                            $('#PurchaseService_DateSaved').val(data.PurchaseService_DateSaved);
                            $('#PurchaseService_ConfirmationDateTime').val(data.PurchaseService_ConfirmationDateTime);
                            if (data.PurchaseService_ConfirmedByUser != '' && data.PurchaseService_ConfirmedByUser != null) {
                                var exists = false;
                                $('#PurchaseService_ConfirmedByUser option').each(function () {
                                    if (data.PurchaseService_ConfirmedByUser == $(this).val()) {
                                        exists = true;
                                    }
                                });
                                if (exists == false) {
                                    $('#PurchaseService_ConfirmedByUser').append('<option value="' + data.PurchaseService_ConfirmedByUser + '">' + data.PurchaseService_ConfirmedByUserName + '</option>');
                                }
                            }
                            $('#PurchaseService_ConfirmedByUser option[value="' + data.PurchaseService_ConfirmedByUser + '"]').attr('selected', true);
                            $('#PurchaseService_ConfirmedByUserName').val(data.PurchaseService_ConfirmedByUserName);
                            $('#PurchaseService_ConfirmationNumber').val(data.PurchaseService_ConfirmationNumber);
                            $('#PurchaseService_Total').val(data.PurchaseService_Total);
                            $('#lblPurchaseService_Total').html($('#PurchaseService_Total').val());
                            //UI.applyFormat('currency');
                            $('#PurchaseService_Savings').val(data.PurchaseService_Savings);
                            $('#PurchaseService_Currency option[value="' + data.PurchaseService_Currency + '"]').attr('selected', true);
                            $('#PurchaseService_ReservedFor').val(data.PurchaseService_ReservedFor);
                            $('#PurchaseService_ChildrenAges').val(data.PurchaseService_ChildrenAges);
                            $('#PurchaseService_CustomMeetingPoint').val(data.PurchaseService_CustomMeetingPoint);
                            $('#PurchaseService_CustomMeetingTime').val(data.PurchaseService_CustomMeetingTime);
                            $('#PurchaseService_Note').val(data.PurchaseService_Note);
                            $('#PurchaseService_Airline').val(data.PurchaseService_Airline);
                            $('#PurchaseService_FlightNumber').val(data.PurchaseService_FlightNumber);
                            if (data.PurchaseService_Audit) {
                                $('input[name="PurchaseService_Audit"]')[0].checked = true;
                            }
                            else {
                                $('input[name="PurchaseService_Audit"]')[1].checked = true;
                            }
                            if (data.PurchaseService_Issued) {
                                $('input[name="PurchaseService_Issued"]')[0].checked = true;
                            }
                            else {
                                $('input[name="PurchaseService_Issued"]')[1].checked = true;
                            }
                            if (data.PurchaseService_Round) {
                                $('input[name="PurchaseService_Round"]')[0].checked = true;
                            }
                            else {
                                $('input[name="PurchaseService_Round"]')[1].checked = true;
                            }
                            $('#PurchaseService_CouponReference').val(data.PurchaseService_CouponReference);
                            if (data.PurchaseService_SoldByOPC) {
                                $('input:[name="PurchaseService_SoldByOPC"]')[0].checked = true;
                            }
                            else {
                                $('input:[name="PurchaseService_SoldByOPC"]')[1].checked = true;
                            }
                            $('#PurchaseService_RoundAirline').val(data.PurchaseService_RoundAirline);
                            $('#PurchaseService_RoundFlightNumber').val(data.PurchaseService_RoundFlightNumber);
                            $('#PurchaseService_RoundFlightTime').val(data.PurchaseService_RoundFlightTime);
                            $('#PurchaseService_RoundDate').val(data.PurchaseService_RoundDate);
                            $('#PurchaseService_RoundMeetingTime').val(data.PurchaseService_RoundMeetingTime);
                            $('#PurchaseService_Round').trigger('change');
                            $('#PurchaseService_Destination option[value="' + data.PurchaseService_Destination + '"]').attr('selected', true);
                            $('#PurchaseService_Destination').trigger('change', { transportationZone: data.PurchaseService_TransportationZone });//trigger change event to load transportationZone
                            $('#PurchaseService_VehicleType option[value="' + data.PurchaseService_VehicleType + '"]').attr('selected', true);
                            $('#PurchaseService_SpecialRequest').val(data.PurchaseService_SpecialRequest);
                            $('#PurchaseService_CancelationCharge').val(data.PurchaseService_CancelationCharge);
                            $('#PurchaseService_CancelationNumber').val(data.PurchaseService_CancelationNumber);
                            $('#PurchaseService_CancelationDateTime').val(data.PurchaseService_CancelationDateTime);
                            if (data.PurchaseService_CanceledByUser != '' && data.PurchaseService_CanceledByUser != null) {
                                var exists = false;
                                $('#PurchaseService_CanceledByUser option').each(function () {
                                    if (data.PurchaseService_CanceledByUser == $(this).val()) {
                                        exists = true;
                                    }
                                });
                                if (exists == false) {
                                    $('#PurchaseService_CanceledByUser').append('<option value="' + data.PurchaseService_CanceledByUser + '">' + data.PurchaseService_CanceledByUserName + '</option>');
                                }
                            }
                            $('#PurchaseService_CanceledByUser option[value="' + data.PurchaseService_CanceledByUser + '"]').attr('selected', true);
                            $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(data.PurchaseService_CouponNumber);
                            $('#PurchaseService_ReplacementOf option[value="' + data.PurchaseService_ReplacementOf + '"]').attr('selected', true);
                            //--obtener cupon y hacer el link visible
                            if (data.PurchaseService_ServiceStatus == 3) {
                                $.post('/MasterChart/GetCouponRef', { purchaseID: data.PurchaseService_Purchase }, function (info) {
                                    var href = info + '-' + data.PurchaseService_PurchaseServiceID;
                                    $('#couponRef').val(href);
                                });
                                $('#btnGetCoupon').show();
                            }
                            //--
                            $('#tblPrices tbody').empty();
                            if (data.ListPurchaseServiceDetails.length > 0) {
                                $.getJSON('/MasterChart/GetPromoInfo', { promoID: data.PurchaseService_Promo }, function (_data) {
                                    $('#hdnGetPromo').val(JSON.stringify(_data));
                                    var builder = '';
                                    $.each(data.ListPurchaseServiceDetails, function (index, item) {
                                        //en este punto el campo hdnGetPromo está vacío debido a que la petición que lo llena aún no se ejecuta, está en el change del campo promo
                                        var subtotal = Number(parseFloat(item.Quantity) * parseFloat(item.CustomPrice)).toFixed(2);
                                        builder += '<tr id = "serviceDetail_' + item.ServiceDetailID + '">'
                                            + '<td>' + item.PriceType
                                            + '<input type="hidden" value="' + item.DealPrice + '" />'
                                            + '</td>'
                                            + '<td class="allow-edition">' + item.Quantity + '</td>'
                                            + '<td>'
                                            + '<input type="hidden" value="' + item.PriceTypeID + '">'
                                            + '<input type="hidden" value="' + item.PriceID + '">'
                                            + '<input type="hidden" value="' + item.ExchangeRateID + '">'
                                            + item.Unit
                                            + '</td>'
                                            + '<td class="format-currency allow-edition">' + UI.addDecimalPart(item.CustomPrice) + '</td>'
                                            + '<td class="format-currency">' + ($('#hdnGetPromo').val() != '' && item.Promo == true ? $.parseJSON($('#hdnGetPromo').val()).isDiscountType == true ? $.parseJSON($('#hdnGetPromo').val()).amount != null ? UI.addDecimalPart((subtotal - parseFloat($.parseJSON($('#hdnGetPromo').val()).amount))) : (UI.addDecimalPart((subtotal - ((subtotal * parseFloat($.parseJSON($('#hdnGetPromo').val()).percentage)) / 100)))) : (UI.addDecimalPart('0')) : (UI.addDecimalPart(subtotal.toString()))) + '</td>'
                                            + '<td>' + (item.Promo == true ? 'Yes' : 'No') + '</td>'
                                            + '<td>' + item.Coupon + '</td>'
                                            + '<td><img src="/Content/themes/base/images/trash.png" class="right delete-item" ' + (data.PurchaseService_ServiceStatus >= 3 ? '' : 'style="display:none"') + '>';
                                        if (data.PurchaseService_ServiceStatus == 3 && $('#PurchaseService_ServiceStatus option:selected').val() == 3 && $('#PurchaseInfo_AllowUnitsEdition').val().toLowerCase() == 'true' && ($('#divSelectedRole').text().trim() == 'Administrator' || $('#divSelectedRole').text().trim() == 'External Reservations Supervisor CAX') && data.PurchaseService_IsClosed == false) {
                                            builder += '<img src="/Images/edit-icon.png" class="right edit-item edition-tools">'//edit icon based on profile
                                                + '<img src="/Content/themes/base/images/confirm.png" class="right edition-tools confirm marg-left" style="display:none" title="confirm">'//edit icon based on profile
                                                + '<img src="/Content/themes/base/images/cross.png" class="right edition-tools discard" style="display:none" title="discard">';//edit icon based on profile
                                        }
                                        builder += '</td>'
                                            + '</tr>';
                                    });
                                    $('#tblPrices tbody').append(builder);
                                    if ($('#PurchaseService_ServiceStatus').val() < 3) {
                                        PURCHASE.deleteItemFromTable();
                                    }
                                    UI.expandFieldset('fdsPurchaseServiceInfo');
                                    UI.scrollTo('fdsPurchaseServiceInfo', null);
                                    $('#PurchaseService_ServiceStatus').trigger('change');
                                    blockFormEdition(data.PurchaseService_ServiceStatus, data.PurchaseService_IsClosed, data.PurchaseService_AvoidEdition, data.PurchaseService_Audit);
                                });
                            }
                            else {
                                restrictPriceTypes(0);
                                UI.expandFieldset('fdsPurchaseServiceInfo');
                                UI.scrollTo('fdsPurchaseServiceInfo', null);
                                $('#PurchaseService_ServiceStatus').trigger('change');
                                blockFormEdition(data.PurchaseService_ServiceStatus, data.PurchaseService_IsClosed, data.PurchaseService_AvoidEdition, PurchaseService_Audit);
                            }
                            UI.applyFormat('currency', 'tblPrices');
                        }
                    });
                }
                else {
                    if (!$('#fdsPurchaseServiceInfo').children('div:first').is(':visible')) {
                        UI.expandFieldset('fdsPurchaseServiceInfo');
                    }
                    UI.scrollTo('fdsPurchaseServiceInfo', null);
                }
            }
        });
    }

    var updateTblPurchaseServices = function (purchaseID) {
        $('#PurchaseService_Purchase').val(purchaseID);
        $.ajax({
            url: '/MasterChart/GetPurchaseServices',
            cache: false,
            type: 'GET',
            data: { PurchaseService_PurchaseID: $('#PurchaseService_Purchase').val() },
            contentType: 'application/html; charset=utf-8',
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                $('#tblExistingPurchasesServicesContainer').html(data);
                PURCHASE.searchResultsPurchaseServicesTable('tblPurchasesServices');
                $('#fdsPurchasesServices').show();
                COMMON.expandAndCollapse('#fdsPurchasesServices', '#fdsPurchaseServiceInfo');
                //UI.expandFieldset('fdsPurchasesServices');
            },
            complete: function () {
                PURCHASE.makeTblPurchaseServiceRowsSelectable();
                PURCHASE.bindPrintFunction();
                UI.applyFormat('currency', 'tblPurchasesServices');
                //if ($('#tblPurchasesServices tbody tr').attr('id') == undefined) {
                $('#btnNewPurchaseServiceInfo').click();
                //}
            }
        });
    }

    var savePurchaseSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {

            if (data.ResponseMessage == 'Purchase Saved') {
                var oSettings = PURCHASE.oPurchaseTable.fnSettings();
                var iAdded = PURCHASE.oPurchaseTable.fnAddData([
                    '0 Coupons',
                    $('#PurchaseInfo_PurchaseStatus option:selected').text(),
                    $('#PurchaseInfo_PurchaseDateTime').val(),
                    $('#PurchaseInfo_Total').val(),
                    $('#PurchaseInfo_Currency option:selected').val(),
                    $('#PurchaseInfo_User option:selected').val() != '' ? $('#PurchaseInfo_User option:selected').text() : '',
                    $('#PurchaseInfo_PointOfSale option:selected').text().split('-')[1].trim(),
                    $('#PurchaseInfo_Terminal option:selected').text() + '<img class="right" src="/Content/themes/base/images/cross.png" id="delP' + data.ItemID + '">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'purchase_' + data.ItemID);
                PURCHASE.oPurchaseTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                PURCHASE.makeTblPurchasesSelectable();
                $('#purchase_' + data.ItemID).click();
            }
            else {
                if ($('#PurchaseInfo_PointOfSale option:selected').text().split('-')[1].trim() != $('#purchase_' + data.ItemID).children('td:nth-child(7)').text().trim()) {
                    $('#PurchaseService_ServiceStatus').trigger('change');//actualizar folio
                }
                $('#purchase_' + data.ItemID).children('td:nth-child(1)').text($('#tblPurchasesServices tbody tr').length + ' Coupons');
                $('#purchase_' + data.ItemID).children('td:nth-child(2)').text($('#PurchaseInfo_PurchaseStatus option:selected').text());
                $('#purchase_' + data.ItemID).children('td:nth-child(3)').text($('#PurchaseInfo_PurchaseDateTime').val());
                $('#purchase_' + data.ItemID).children('td:nth-child(4)').text($('#PurchaseInfo_Total').val());
                $('#purchase_' + data.ItemID).children('td:nth-child(5)').text($('#PurchaseInfo_Currency option:selected').val());
                $('#purchase_' + data.ItemID).children('td:nth-child(6)').text($('#PurchaseInfo_User option:selected').val() != '' ? $('#PurchaseInfo_User option:selected').text() : '');
                $('#purchase_' + data.ItemID).children('td:nth-child(7)').text($('#PurchaseInfo_PointOfSale option:selected').text().split('-')[1].trim());
                $('#purchase_' + data.ItemID).children('td:nth-child(8)')[0].firstChild.textContent = $('#PurchaseInfo_Terminal option:selected').text();
                $('#purchase_' + data.ItemID).removeClass('selected-row').click();
            }
            localStorage.Eplat_PointOfSale = $('#PurchaseInfo_PointOfSale option:selected').val();
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'paymentTypesPerLocation', itemID: $('#PurchaseInfo_PointOfSale option:selected').val() }, function (data) {
                $('#PurchasePayment_PaymentType').fillSelect(data);
            });
            //update exchange rates
            $.ajax({
                url: '/MasterChart/GetExchangeRates',
                async: false,
                contentType: 'JSON',
                data: { date: ($('#PurchasePayment_DateSaved').val() != undefined ? $('#PurchasePayment_DateSaved').val() : COMMON.getDate()), pointOfSaleID: $('#PurchaseInfo_PointOfSale option:selected').val() },
                success: function (data) {
                    //exchangeRates = data.split(' ').join('').split(',');
                    $('#currentExchangeRates').text(data);
                }
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
        UI.applyFormat('currency', 'tblPurchases');
        //update column pointOfSale, terminal, status in tblLeads
        var _stayingAt = $('#PurchaseInfo_StayingAtPlace option:selected').val() != '0' ? $('#PurchaseInfo_StayingAtPlace option:selected').val() != 'null' ? $('#PurchaseInfo_StayingAtPlace option:selected').text() : $('#PurchaseInfo_StayingAt').val() : '';

        var _oTable = SEARCH.oTable != undefined ? SEARCH.oTable : $('#tblSearchResult_Leads tbody');
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(8)').text($('#PurchaseInfo_PointOfSale option:selected').text().trim().split('-')[0]);
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(9)').text($('#PurchaseInfo_Terminal option:selected').text());
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(4)').text($('#PurchaseInfo_PurchaseStatus option:selected').text());
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(3)').text(_stayingAt);

    }

    var _savePurchaseSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Purchase Saved') {
                var oSettings = PURCHASE.oPurchaseTable.fnSettings();
                var iAdded = PURCHASE.oPurchaseTable.fnAddData([
                    '0 Coupons',
                    $('#PurchaseInfo_PurchaseStatus option:selected').text(),
                    $('#PurchaseInfo_PurchaseDateTime').val(),
                    $('#PurchaseInfo_Total').val(),
                    $('#PurchaseInfo_Currency option:selected').val(),
                    $('#PurchaseInfo_User option:selected').val() != '' ? $('#PurchaseInfo_User option:selected').text() : '',
                    $('#PurchaseInfo_PointOfSale option:selected').text().split('-')[1].trim(),
                    $('#PurchaseInfo_Terminal option:selected').text() + '<img class="right" src="/Content/themes/base/images/cross.png" id="delP' + data.ItemID + '">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'purchase_' + data.ItemID);
                PURCHASE.oPurchaseTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                PURCHASE.makeTblPurchasesSelectable();
                $('#purchase_' + data.ItemID).click();
            }
            else {
                if ($('#PurchaseInfo_PointOfSale option:selected').text().split('-')[1].trim() != $('#purchase_' + data.ItemID).children('td:nth-child(7)').text().trim()) {
                    $('#PurchaseService_ServiceStatus').trigger('change');//actualizar folio
                }
                $('#purchase_' + data.ItemID).children('td:nth-child(1)').text($('#tblPurchasesServices tbody tr').length + ' Coupons');
                $('#purchase_' + data.ItemID).children('td:nth-child(2)').text($('#PurchaseInfo_PurchaseStatus option:selected').text());
                $('#purchase_' + data.ItemID).children('td:nth-child(3)').text($('#PurchaseInfo_PurchaseDateTime').val());
                $('#purchase_' + data.ItemID).children('td:nth-child(4)').text($('#PurchaseInfo_Total').val());
                $('#purchase_' + data.ItemID).children('td:nth-child(5)').text($('#PurchaseInfo_Currency option:selected').val());
                $('#purchase_' + data.ItemID).children('td:nth-child(6)').text($('#PurchaseInfo_User option:selected').val() != '' ? $('#PurchaseInfo_User option:selected').text() : '');
                $('#purchase_' + data.ItemID).children('td:nth-child(7)').text($('#PurchaseInfo_PointOfSale option:selected').text().split('-')[1].trim());
                $('#purchase_' + data.ItemID).children('td:nth-child(8)')[0].firstChild.textContent = $('#PurchaseInfo_Terminal option:selected').text();
            }
            localStorage.Eplat_PointOfSale = $('#PurchaseInfo_PointOfSale option:selected').val();
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'paymentTypesPerLocation', itemID: $('#PurchaseInfo_PointOfSale option:selected').val() }, function (data) {
                $('#PurchasePayment_PaymentType').fillSelect(data);
            });
            //update exchange rates
            $.ajax({
                url: '/MasterChart/GetExchangeRates',
                async: false,
                contentType: 'JSON',
                data: { date: ($('#PurchasePayment_DateSaved').val() != undefined ? $('#PurchasePayment_DateSaved').val() : COMMON.getDate()), pointOfSaleID: $('#PurchaseInfo_PointOfSale option:selected').val() },
                success: function (data) {
                    //exchangeRates = data.split(' ').join('').split(',');
                    $('#currentExchangeRates').text(data);
                }
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
        UI.applyFormat('currency', 'tblPurchases');
        //update column pointOfSale, terminal, status in tblLeads
        var _stayingAt = $('#PurchaseInfo_StayingAtPlace option:selected').val() != '0' ? $('#PurchaseInfo_StayingAtPlace option:selected').val() != 'null' ? $('#PurchaseInfo_StayingAtPlace option:selected').text() : $('#PurchaseInfo_StayingAt').val() : '';

        var _oTable = SEARCH.oTable != undefined ? SEARCH.oTable : $('#tblSearchResult_Leads tbody');
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(8)').text($('#PurchaseInfo_PointOfSale option:selected').text().trim().split('-')[0]);
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(9)').text($('#PurchaseInfo_Terminal option:selected').text());
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(4)').text($('#PurchaseInfo_PurchaseStatus option:selected').text());
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(3)').text(_stayingAt);
    }

    var savePurchaseServiceSuccess = function (data, couponFlag) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            //update localStorage counters
            UI.updateLocalStorageCounter('OpenCoupon', $('#PurchaseService_OpenCoupon').val());
            UI.updateLocalStorageCounter('MeetingPoint', $('#PurchaseService_MeetingPoint').val());
            //UI.updateLocalStorageCounter('PriceType', $('#PurchaseServiceDetailModel_PurchaseServiceDetail_PriceType').val());
            var _schedule = $('#PurchaseService_Airline').is(':visible') ? $('#PurchaseService_CustomMeetingTime').val() : $('#PurchaseService_WeeklyAvailability option:selected').val() != '0' ? $('#PurchaseService_WeeklyAvailability option:selected').text().substr(0, 10) : '';
            var a = COMMON.getDate();
            var b = a.split('-');
            var c = '';
            $.each(b, function (i, x) {
                c += (c == '' ? '' : '-') + UI.padNumber(x, 2);
            });
            if (data.ResponseMessage == 'Coupon Saved') {
                var _picture = data.ItemID.mainPicture != '' ? '<img src="//eplatfront.villagroup.com' + data.ItemID.mainPicture + '?width=70&height=50&mode=crop" style="vertical-align:inherit; margin:5px;" />' : '<div style="width:70px;height:50px;margin:5px;display:inline-block;vertical-align:inherit;"></div>';
                var oSettings = PURCHASE.oPurchaseServiceTable.fnSettings();
                var iAdded = PURCHASE.oPurchaseServiceTable.fnAddData([
                    _picture,
                    $('#PurchaseService_Service option:selected').text().split('-')[1],
                    data.ItemID.newFolio != null ? data.ItemID.newFolio.split('-')[0] : $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(),
                    $('#PurchaseService_ServiceDateTime').val(),
                    _schedule,
                    $('#PurchaseService_MeetingPoint option:selected').val() != '0' ? $('#PurchaseService_MeetingPoint option:selected').val() != 'null' ? $('#PurchaseService_MeetingPoint option:selected').text() : $('#PurchaseService_CustomMeetingPoint').val() : '',
                    ($('#PurchaseService_Promo option:selected').val() != 0 ? $('#PurchaseService_Promo option:selected').text() : 'None') + '<input type="hidden" class="promo-info" value="" />',
                    (parseInt($('#PurchaseService_ServiceStatus option:selected').val()) < 4 ? $('#PurchaseService_Total').val() : ($('#PurchaseService_CancelationCharge').val() != '' ? UI.addDecimalPart($('#PurchaseService_CancelationCharge').val()) : '0.00'))
                    + ' ' + $('#PurchaseService_Currency').val(),
                    '<span class="block">'
                    + $('#PurchaseService_ServiceStatus option:selected').text()
                    + ($('#PurchaseService_ServiceStatus option:selected').val() >= 3 ? '<span class="block">' + COMMON.getDate() + '</span>' : '') + '</span>',
                    ($('#PurchaseService_ServiceStatus option:selected').val() == 3 ? '<span class="block align-from-top"><input type="button" class="print-coupon button" value="print" /></span>' : '')
                    //+ ($('#PurchaseService_ServiceStatus option:selected').val() == 3 ? '<span class="block align-from-top"><input type="button" class="get-coupon button" value="get" /></span>' : '')
                    + ($('#PurchaseService_ServiceStatus option:selected').val() == 3 && $('.send-coupons').length > 0 ? '<span class="block align-from-top"><input type="button" class="get-coupon button" value="get" /></span>' : '')
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'purchaseService_' + data.ItemID.purchaseServiceID);
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(8)').addClass('format-currency');
                PURCHASE.oPurchaseServiceTable.fnDisplayRow(aRow);
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(7)').children(':hidden:first').val('{"promoID": "' + $('#PurchaseService_Promo option:selected').val() + '", "promo": "' + ($('#PurchaseService_Promo option:selected').val() != 0 ? $('#PurchaseService_Promo option:selected').text() : '') + '", "isPackable": ' + ($('#PurchaseService_Promo option:selected').val() != 0 ? $.parseJSON($('#hdnGetPromo').val()).isPackable.toString() : 'false') + ', "date": "' + c + '"}');
                PURCHASE.makeTblPurchaseServiceRowsSelectable();
                PURCHASE.updatePaymentDetailsSpan();
                PURCHASE.updateTblRecentCouponsAndJson();
                $('#purchaseService_' + data.ItemID.purchaseServiceID).attr('data-issued', false);

                //$('#btnNewPurchaseServiceInfo').click();
            }
            else {
                var _picture = data.ItemID.mainPicture != '' ? '<img src="//eplatfront.villagroup.com' + data.ItemID.mainPicture + '?width=70&height=50&mode=crop" style="vertical-align:inherit; margin:5px;" />' : '<div style="width:70px;height:50px;margin:5px;display:inline-block;vertical-align:inherit;"></div>';
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(1)').html(_picture);
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(2)').html($('#PurchaseService_Service option:selected').text().split('-')[1]);
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(3)').text(data.ItemID.newFolio != null ? data.ItemID.newFolio.split('-')[0] : $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val());
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(4)').text($('#PurchaseService_ServiceDateTime').val());
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(5)').text(_schedule);
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(6)').text($('#PurchaseService_MeetingPoint option:selected').val() != '0' ? $('#PurchaseService_MeetingPoint option:selected').val() != 'null' ? $('#PurchaseService_MeetingPoint option:selected').text() : $('#PurchaseService_CustomMeetingPoint').val() : '');
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(7)').html(($('#PurchaseService_Promo option:selected').val() != '0' ? $('#PurchaseService_Promo option:selected').text() : 'None') + '<input type="hidden" value="" class="promo-info" />');//[0].childNodes[0].nodeValue = $('#PurchaseService_Promo option:selected').val() != '0' ? $('#PurchaseService_Promo option:selected').text() : 'None';
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(7)').children(':hidden:first').val('{"promoID": "' + $('#PurchaseService_Promo option:selected').val() + '", "promo": "' + ($('#PurchaseService_Promo option:selected').val() != 0 ? $('#PurchaseService_Promo option:selected').text() : '') + '", "isPackable": ' + ($('#PurchaseService_Promo option:selected').val() != 0 ? $.parseJSON($('#hdnGetPromo').val()).isPackable.toString() : 'false') + ', "date": "' + ($('#PurchaseService_ConfirmationDateTime').val() != '' ? $('#PurchaseService_ConfirmationDateTime').val().split(' ')[0] : '') + '"}');
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(8)').text((parseInt($('#PurchaseService_ServiceStatus option:selected').val()) < 4 || parseInt($('#PurchaseService_ServiceStatus option:selected').val()) == 6 ? $('#PurchaseService_Total').val() : ($('#PurchaseService_CancelationCharge').val() != '' ? UI.addDecimalPart($('#PurchaseService_CancelationCharge').val()) : '0.00')) + ' ' + $('#PurchaseService_Currency').val());
                //$('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(9)').text($('#PurchaseService_Currency').val());
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(9)').html('<span class="block">'
                    + $('#PurchaseService_ServiceStatus option:selected').text()
                    //+ ($('#PurchaseService_ServiceStatus option:selected').val() >= 3 ? '<span class="block">' + COMMON.getDate() + '</span>' : '') + '</span>');
                    + ($('#PurchaseService_ServiceStatus option:selected').val() >= 3 ? $('#PurchaseService_ServiceStatus option:selected').val() == 3 ? '<span class="block">' + ($('#PurchaseService_ConfirmationDateTime').val() != '' ? $('#PurchaseService_ConfirmationDateTime').val().split(' ')[0] : COMMON.getDate()) + '</span>' + '</span>' : '<span class="block">' + ($('#PurchaseService_CancelationDateTime').val() != '' ? $('#PurchaseService_CancelationDateTime').val().split(' ')[0] : COMMON.getDate()) + '</span>' : '') + '</span>');
                $('#purchaseService_' + data.ItemID.purchaseServiceID).children('td:nth-child(10)').html(
                    ($('#PurchaseService_ServiceStatus option:selected').val() == 3 ? '<span class="block align-from-top"><input type="button" class="print-coupon button" value="print" /></span>' : '')
                    //+ ($('#PurchaseService_ServiceStatus option:selected').val() == 3 ? '<span class="block align-from-top"><input type="button" class="get-coupon button" value="get" /></span>' : '')
                    + ($('#PurchaseService_ServiceStatus option:selected').val() == 3 && $('.send-coupons').length > 0 ? '<span class="block align-from-top"><input type="button" class="get-coupon button" value="get" /></span>' : '')
                );
                if (data.ItemID.newFolio != null) {
                    $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(data.ItemID.newFolio.split('-')[0]);
                }
                blockFormEdition($('#PurchaseService_ServiceStatus option:selected').val(), data.ItemID.isClosed, null, false);
                //UI.collapseFieldset('fdsPurchaseServiceInfo');
                //$('#btnNewPurchaseServiceInfo').click();
            }
            $.getJSON('/crm/MasterChart/GetDDLData', { itemType: 'siblingsCoupons', itemID: $('#PurchaseInfo_PurchaseID').val() }, function (data) {
                $('#PurchaseService_ReplacementOf').fillSelect(data);
            });
        }

        if ($('#PurchaseService_ServiceStatus option:selected').val() == 3) {
            $('#PurchaseService_ConfirmationDateTime').val(data.ItemID.datetime);
            $('#PurchaseService_ConfirmedByUser option[value="' + data.ItemID.user + '"]').attr('selected', true);
            $('#purchaseService_' + data.ItemID.purchaseServiceID).removeClass('canceled-row');
        }
        //change background color if is canceled
        if ($('#PurchaseService_ServiceStatus option:selected').val() >= 4) {
            $('#purchaseService_' + data.ItemID.purchaseServiceID).addClass('canceled-row');
            if ($('#PurchaseService_ServiceStatus option:selected').val() < 6) {
                $('#PurchaseService_CancelationDateTime').val(data.ItemID.datetime);
                $('#PurchaseService_CanceledByUser option[value="' + data.ItemID.user + '"]').attr('selected', true);
            }
        }
        //else if ($('#purchaseService_' + data.ItemID.purchaseServiceID).hasClass('canceled-row')) {
        //else if (!$('#purchaseService_' + data.ItemID.purchaseServiceID).hasClass('canceled-row')) {
        //    $('#purchaseService_' + data.ItemID.purchaseServiceID).addClass('canceled-row');
        //}
        //actualiza lista de actividades en paymentInfo
        PURCHASE.updateSelectOfCoupons($('#PurchaseInfo_PurchaseID').val());
        PURCHASE.bindPrintFunction();
        //actualiza la celda coupons en tblPurchases
        $('#tblPurchases tbody').find('tr.selected-row').children('td:first')[0].textContent = $('#tblPurchasesServices tbody tr').length + ' Coupons';
        //actualiza el campo total en purchaseInfo, en purchaseServiceInfo, en tblPurchases y en tblLeads
        $('#PurchaseInfo_Total').val(UI.addDecimalPart(data.ItemID.purchaseTotal));
        $('#lblPurchaseInfo_Total').html($('#PurchaseInfo_Total').val());
        $('#tblPurchases tbody').find('tr.selected-row').children('td:nth-child(4)')[0].textContent = $('#PurchaseInfo_Total').val();
        var _oTable = SEARCH.oTable != undefined ? SEARCH.oTable : $('#tblSearchResult_Leads tbody');
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(6)').text($('#PurchaseInfo_Total').val() + ' ' + $('#PurchaseInfo_Currency option:selected').val());
        $('#PurchaseService_Total').val((parseInt($('#PurchaseService_ServiceStatus option:selected').val()) < 4 ? $('#PurchaseService_Total').val() : ($('#PurchaseService_CancelationCharge').val() != '' ? UI.addDecimalPart($('#PurchaseService_CancelationCharge').val()) : '0.00')));
        $('#lblPurchaseService_Total').html($('#PurchaseService_Total').val());
        UI.applyFormat('currency', 'tblPurchases,tblPurchasesServices,tblPrices');
        //actualiza el campo status en purchaseInfo y en tblLeads
        $('#PurchaseInfo_PurchaseStatus option[value="' + data.ItemID.purchaseStatus + '"]').attr('selected', true);
        _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(4)').text($('#PurchaseInfo_PurchaseStatus option:selected').text());
        //actualiza los totales en pagos
        PURCHASE.updatePaymentDetailsSpan();
        $('#btnNewPurchaseServiceInfo').click();//la saqué de actualizacion/pago
        if (couponFlag == undefined) {
            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
        }
        else {
            UI.twoActionBox('Coupon <br />' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '<br /><a href="' + $('#couponRef').val() + '" target="_blank">' + $('#couponRef').val() + '</a>'
                + '<br /><div class="align-from-top align-from-bottom"><input type="checkbox" id="chkSendProvider" /> Send to Provider: ' + $('#PurchaseService_ProviderEmail').val() + '<br /><br /><input type="checkbox" id="chkSendOther" /> Send to Other: <input type="text" id="txtSendOther" /></div>', printCoupon, [$('#couponRef').val()], 'print coupon', sendCouponsByEmail, [$('#PurchaseService_Purchase').val(), data.ItemID.purchaseServiceID], 'send by email');
            //UI.twoActionBox('Coupon <br />' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '<br /><a href="' + $('#couponRef').val() + '" target="_blank">' + $('#couponRef').val() + '</a>', printCoupon, [$('#couponRef').val()], 'print coupon', sendCouponsByEmail, [$('#PurchaseService_Purchase').val(), data.ItemID.purchaseServiceID], 'send by email');
        }
    }

    //update localStorage variable
    var updateTblRecentCouponsAndJson = function (response) {
        if (localStorage.Eplat_RecentCoupons != undefined && localStorage.Eplat_RecentCoupons != '[]' && response == undefined) {
            //local variable has content
            var found = false;
            var _json = $.parseJSON(localStorage.Eplat_RecentCoupons);

            if (_json[0].PointOfSale != undefined) {
                //json has new format
                $.each(_json, function (i, item) {
                    if (item.DateSaved == $('#SearchCoupon_PurchaseDate').val() && item.PointOfSale == $('#SearchCoupon_PointOfSale option:selected').val()) {
                        //if an object of same PoS and purchaseDate already exists
                        found = true;
                        if (response != undefined) {
                            var _response = $.parseJSON(response)[0].Coupons;
                            item.Coupons = _response;
                        }
                        else {
                            item.Coupons.push({
                                "DT_RowId": $('#GeneralInformation_LeadID').val(),
                                "RecentCoupon_LeadID": $('#GeneralInformation_LeadID').val(),
                                "RecentCoupon_PurchaseID": $('#PurchaseInfo_PurchaseID').val(),
                                "RecentCoupon_Coupon": $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(),
                                "RecentCoupon_FirstName": $('#PurchaseService_ReservedFor').val(),
                                "RecentCoupon_ServiceStatus": $('#PurchaseService_ServiceStatus option:selected').text(),
                                "RecentCoupon_Total": $('#PurchaseService_Total').val(),
                                "RecentCoupon_DateSaved": COMMON.getDate(),
                                "RecentCoupon_ServiceDate": $('#PurchaseService_ServiceDateTime').val(),
                                "RecentCoupon_Location": ($('#PurchaseInfo_Location').length != 0 ? $('#PurchaseInfo_Location option:selected').val() : 0)
                            });
                        }
                        item.DateLastUpdate = COMMON.getDate();
                    }
                });
                if (!found) {
                    //if an object of same PoS and purchaseDate doesn't exist
                    if (response != undefined) {
                        var _response = $.parseJSON(response)[0];
                        _json.push({
                            "PointOfSale": _response.PointOfSale,
                            "DateSaved": _response.DateSaved,
                            "Coupons": _response.Coupons,
                            "DateLastUpdate": COMMON.getDate()
                        });
                    }
                    else {
                        _json.push({
                            "PointOfSale": $('#SearchCoupon_PointOfSale option:selected').val(),
                            "DateSaved": COMMON.getDate(),
                            "Coupons": [{
                                "DT_RowId": $('#GeneralInformation_LeadID').val(),
                                "RecentCoupon_LeadID": $('#GeneralInformation_LeadID').val(),
                                "RecentCoupon_PurchaseID": $('#PurchaseInfo_PurchaseID').val(),
                                "RecentCoupon_Coupon": $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(),
                                "RecentCoupon_FirstName": $('#PurchaseService_ReservedFor').val(),
                                "RecentCoupon_ServiceStatus": $('#PurchaseService_ServiceStatus option:selected').text(),
                                "RecentCoupon_Total": $('#PurchaseService_Total').val() + ' ' + $('#PurchaseInfo_Currency').val(),
                                "RecentCoupon_DateSaved": COMMON.getDate(),
                                "RecentCoupon_ServiceDate": $('#PurchaseService_ServiceDateTime').val(),
                                "RecentCoupon_Location": ($('#PurchaseInfo_Location').length != 0 ? $('#PurchaseInfo_Location option:selected').val() : 0)
                            }],
                            "DateLastUpdate": COMMON.getDate()
                        });
                    }
                }
            }
            else {
                //json has old format. delete variable and push just created coupon
                _json = new Array();
                if (response != undefined) {
                    var _response = $.parseJSON(response)[0];
                    _json.push({
                        "PointOfSale": _response.PointOfSale,
                        "DateSaved": _response.DateSaved,
                        "Coupons": _response.Coupons,
                        "DateLastUpdate": COMMON.getDate()
                    });
                }
                else {
                    _json.push({
                        "PointOfSale": $('#SearchCoupon_PointOfSale option:selected').val(),
                        "DateSaved": COMMON.getDate(),
                        "Coupons": [{
                            "DT_RowId": $('#GeneralInformation_LeadID').val(),
                            "RecentCoupon_LeadID": $('#GeneralInformation_LeadID').val(),
                            "RecentCoupon_PurchaseID": $('#PurchaseInfo_PurchaseID').val(),
                            "RecentCoupon_Coupon": $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(),
                            "RecentCoupon_FirstName": $('#PurchaseService_ReservedFor').val(),
                            "RecentCoupon_ServiceStatus": $('#PurchaseService_ServiceStatus option:selected').text(),
                            "RecentCoupon_Total": $('#PurchaseService_Total').val() + ' ' + $('#PurchaseInfo_Currency').val(),
                            "RecentCoupon_DateSaved": COMMON.getDate(),
                            "RecentCoupon_ServiceDate": $('#PurchaseService_ServiceDateTime').val(),
                            "RecentCoupon_Location": ($('#PurchaseInfo_Location').length != 0 ? $('#PurchaseInfo_Location option:selected').val() : 0)
                        }],
                        "DateLastUpdate": COMMON.getDate()
                    });
                }
            }
            localStorage.Eplat_RecentCoupons = JSON.stringify(_json);
        }
        else {
            //local variable hasn't content
            var _json = new Array();
            if (response != undefined) {
                var _response = $.parseJSON(response)[0];
                _json.push({
                    "PointOfSale": _response.PointOfSale,
                    "DateSaved": _response.DateSaved,
                    "Coupons": _response.Coupons,
                    "DateLastUpdate": COMMON.getDate()
                });
            }
            else {
                _json.push({
                    "PointOfSale": $('#SearchCoupon_PointOfSale option:selected').val(),
                    "DateSaved": COMMON.getDate(),
                    "Coupons": [{
                        "DT_RowId": $('#GeneralInformation_LeadID').val(),
                        "RecentCoupon_LeadID": $('#GeneralInformation_LeadID').val(),
                        "RecentCoupon_PurchaseID": $('#PurchaseInfo_PurchaseID').val(),
                        "RecentCoupon_Coupon": $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val(),
                        "RecentCoupon_FirstName": $('#PurchaseService_ReservedFor').val(),
                        "RecentCoupon_ServiceStatus": $('#PurchaseService_ServiceStatus option:selected').text(),
                        "RecentCoupon_Total": $('#PurchaseService_Total').val() + ' ' + $('#PurchaseInfo_Currency').val(),
                        "RecentCoupon_DateSaved": COMMON.getDate(),
                        "RecentCoupon_ServiceDate": $('#PurchaseService_ServiceDateTime').val(),
                        "RecentCoupon_Location": ($('#PurchaseInfo_Location').length != 0 ? $('#PurchaseInfo_Location option:selected').val() : 0)
                    }],
                    "DateLastUpdate": COMMON.getDate()
                });
            }
            localStorage.Eplat_RecentCoupons = JSON.stringify(_json);
        }
        PURCHASE.loadCouponsTableFromJson();
    }

    var getDisabledDaysOfService = function (serviceID, year, month, day, terminal) {
        $.ajax({
            url: '/Activities/GetAvailableDates',
            dataType: 'json',
            async: false,
            data: { service: serviceID, month: parseInt(month) + 1, year: year, terminalid: terminal },
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                PURCHASE.disabledDays = data.DatesString.split(',');
                PURCHASE.indicator = month;
            }
        });
    }

    var closedDays = function (date) {
        //if (date.getMonth() != PURCHASE.indicator && c > 1 && date.getDay() == 0) {
        if ((date.getMonth() != PURCHASE.indicator && c > 1 && date.getDay() == 0) || (date.getDate() == 1 && date.getDay() == 0)) {
            c = 1;
        }
        if (c == date.getDate()) {
            c++;
            if (PURCHASE.indicator != date.getMonth()) {
                PURCHASE.getDisabledDaysOfService($('#PurchaseService_Service option:selected').val(), date.getFullYear(), date.getMonth(), date.getDate(), $('#PurchaseInfo_Terminal').val());
            }
            var m = date.getMonth(), d = date.getDate(), y = date.getFullYear();
            if (PURCHASE.disabledDays != undefined) {
                for (i = 0; i < PURCHASE.disabledDays.length; i++) {
                    //if($.inArray(y + '-' + (m + 1) + '-' + d, PURCHASE.disabledDays) != -1 || new Date().getTime() + 1 * 24 * 60 * 60 * 1000 >= date){
                    if ($.inArray(y + '-' + (m + 1) + '-' + d, PURCHASE.disabledDays) != -1) {
                        return [false];
                    }
                }
            }
            else {
                c = 1;
                return [false];
            }
            return [true];
        }
        else {
            c = 1;
            return [false];
        }
    }

    //----------------------------------------------------------

    function savePayment(formID, changeStatusFlag) {
        $('#PurchasePayment_ChangeCouponStatus').val((changeStatusFlag != undefined ? changeStatusFlag : 'null'));
        if ($('#' + formID).valid()) {
            $('#' + formID).submit();
        }
        else {
            UI.showValidationSummary(formID);
        }
    }

    var updateTblPurchasePayments = function (purchaseID) {
        $('#PurchasePayment_Purchase').val(purchaseID);
        $.ajax({
            url: '/MasterChart/GetPurchasePayments',
            cache: false,
            type: 'GET',
            data: { PurchasePayment_Purchase: $('#PurchasePayment_Purchase').val() },
            contentType: 'application/html; charset=utf-8',
            dataType: 'html',
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                $('#tblExistingPurchasesPaymentsContainer').html(data);
                PURCHASE.searchResultsPurchasePaymentsTable($('#tblPurchasesPayments'));
                $('#fdsPurchasesPayments').show();
                //UI.expandFieldset('fdsPurchasesPayments');
                COMMON.expandAndCollapse('#fdsPurchasesPayments', '#fdsPurchasePaymentInfo');
                //PURCHASE.updatePaymentDetailsSpan();
            },
            complete: function () {
                //PURCHASE.updateSelectOfCoupons(purchaseID);
                PURCHASE.makeTblPurchasePaymentRowsSelectable();
                if ($('#tblPurchasesPayments tbody tr:first').attr('id') != undefined) {
                    $('.print-ticket').show();
                }
                else {
                    $('.print-ticket').hide();
                }
                PURCHASE.updatePaymentDetailsSpan();
                UI.scrollTo('fdsPurchasePaymentInfo', null);
                //$('#btnNewPurchasePaymentInfo').trigger('click');
            }
        });
    }

    var updateTblPurchaseTickets = function (purchaseID) {
        $.ajax({
            url: '/MasterChart/GetPurchaseTickets',
            cache: false,
            type: 'GET',
            data: { PurchasePayment_Purchase: $('#PurchasePayment_Purchase').val() },
            contentType: 'application/html; charset=utf-8',
            dataType: 'html',
            success: function (data) {
                $('#divPurchaseTickets').html(data);
                UI.applyFormat('currency');
            },
            complete: function () {
                PURCHASE.makePurchaseTicketsPrintable();
            }
        });
    }

    var makePurchaseTicketsPrintable = function () {
        $('.print-ticket').unbind('click').on('click', function () {
            var purchaseTicketID = 0;
            if ($(this).hasClass('reprint')) {
                purchaseTicketID = $(this).parents('tr:first').attr('id').split('_')[1];
            }
            $.ajax({
                type: 'POST',
                url: '/MasterChart/RenderPurchaseTicket',
                data: { PurchaseID: $('#PurchaseInfo_PurchaseID').val(), PurchaseTicketID: purchaseTicketID },
                success: function (data) {
                    $('.printable:not(#printableTicket)').addClass('non-printable');
                    $('#printableTicket').removeClass('non-printable');
                    $('#printableTicket').html(data);
                    if (purchaseTicketID == 0) {
                        PURCHASE.updateTblPurchaseTickets();
                    }
                }
            });
        });
    }

    var updateSelectOfCoupons = function (purchaseID) {
        //$.getJSON('/MasterChart/GetDDLData', { itemType: 'purchase', itemID: purchaseID }, function (data) {
        //    //if ($('#PurchasePayment_Coupons').multiselect()) {
        //    //    $('#PurchasePayment_Coupons').multiselect('destroy');
        //    //}
        //    $('#PurchasePayment_Coupons').fillSelect(data);
        //    $('#PurchasePayment_Coupons option').each(function () {
        //        if ($(this).text().indexOf('Canceled') == -1 && $(this).text().indexOf('Refund') == -1)
        //            $(this).attr('selected', true);
        //    });
        //    $('#PurchasePayment_Coupons').multiselect('refresh');
        //});
    }

    var savePurchasePaymentSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if ($('#PurchasePayment_ChargeBackConcept option:selected').text().indexOf('Company') > 0) {
                UI.updateLocalStorageCounter('CXCCompany', $('#PurchasePayment_Company option:selected').val());
            }
            PURCHASE.updateTblPurchasePayments(data.ItemID.purchase);
            //$('#btnNewPurchasePaymentInfo').trigger('click');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var _savePurchasePaymentSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            //var _coupons = '';
            //var _couponsID = '';
            var amount = UI.addDecimalPart($('#PurchasePayment_Amount').val());
            var _transactionType = $('#PurchasePayment_TransactionType option:selected').val();
            //$('#PurchasePayment_Coupons option').each(function (index, item) {
            //    if ($(this).is(':selected')) {
            //        //_coupons += (_coupons != '' ? ', ' : '') + $('#PurchasePayment_Coupons option[value="' + $(item).val() + '"]').text().split('|')[0];
            //        _coupons += (_coupons != '' ? ', ' : '') + $('#PurchasePayment_Coupons option[value="' + $(item).val() + '"]').text();
            //        _couponsID += (_couponsID != '' ? ',' : '') + $(item).val();
            //    }
            //});

            if (data.ResponseMessage.toString().indexOf('Transaction Saved') != -1) {
                var oSettings = PURCHASE.oPurchasePaymentTable.fnSettings();
                var iAdded = PURCHASE.oPurchasePaymentTable.fnAddData([
                    ($('#PurchasePayment_Other').val() != '' ? $('#PurchasePayment_Other').val() : $('#PurchasePayment_Company option:selected').text()),
                    $('#PurchasePayment_PaymentType option:selected').text() + '<input type="hidden" class="hdn-commission-applied" value="' + $('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val() + '">'
                    + '<input type="hidden" class="hdn-bank-commission" value="' + $('#PurchasePayment_CardCommission').val() + '">',
                    UI.addDecimalPart(data.ItemID.exchangeRate),
                    ($('#PurchasePayment_TransactionType option:selected').val() == 1 ? '' : '-') + UI.addDecimalPart($('#PurchasePayment_Amount').val()) + ' ' + $('#PurchasePayment_Currency option:selected').val(),
                    //_coupons,
                    data.ItemID.authCode,
                    data.ItemID.dateSaved,
                    data.ItemID.reference,
                    '<span class="table-cell">' + data.ItemID.errorCode + '</span>'
                    + '<span class="table-cell"><img src="/Content/themes/base/images/cross.png" class="right" /></span>'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'purchasePayment_' + data.ItemID.paymentID);
                //check why this next line was commented
                PURCHASE.oPurchasePaymentTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                PURCHASE.makeTblPurchasePaymentRowsSelectable();
                $('#frmPurchasePaymentInfo').clearForm(true);
            }
            else {
                var array = PURCHASE.oPurchasePaymentTable.fnGetNodes();
                var nTr = PURCHASE.oPurchasePaymentTable.$('tr.selected-row');
                var position = PURCHASE.oPurchasePaymentTable.fnGetPosition(nTr[0]);
                PURCHASE.oPurchasePaymentTable.fnDisplayRow(array[position]);
                var _amount = (($('#PurchasePayment_TransactionType option:selected').val() == '1' ? '' : '-') + UI.addDecimalPart($('#PurchasePayment_Amount').val()) + ' ' + $('#PurchasePayment_Currency option:selected').val());
                PURCHASE.oPurchasePaymentTable.fnUpdate([
                    ($('#PurchasePayment_Other').val() != '' ? $('#PurchasePayment_Other').val() : $('#PurchasePayment_Company option:selected').text()),
                    $('#PurchasePayment_PaymentType option:selected').text() + '<input type="hidden" class="hdn-commission-applied" value="' + $('input:radio[name="PurchasePayment_ApplyCommission"]:checked').val() + '">'
                    + '<input type="hidden" class="hdn-bank-commission" value="' + $('#PurchasePayment_CardCommission').val() + '">',
                    UI.addDecimalPart(data.ItemID.exchangeRate),
                    _amount,
                    //_coupons,
                    data.ItemID.authCode,
                    data.ItemID.dateSaved,
                    data.ItemID.reference,
                    '<span class="table-cell">' + data.ItemID.errorCode + '</span>'
                    + '<span class="table-cell"><img src="/Content/themes/base/images/cross.png" class="right" /></span>'
                ], $('#purchasePayment_' + data.ItemID.paymentID)[0], undefined, false);
            }
            if (_transactionType == '2') {
                if ($('#PurchasePayment_ChangeCouponStatus').val() == 'true') {
                    //debido a que la actualización puede contener varios cupones y la cantidad puede ser distinta, es preferible volver a cargar la tabla de cupones
                    PURCHASE.updateTblPurchaseServices($('#PurchaseInfo_PurchaseID').val());
                    $('#PurchasePayment_ChangeCouponStatus').removeAttr('value');

                    //change total of purchase displayed in purchase row
                    PURCHASE.oPurchaseTable.$('tr#purchase_' + data.ItemID.purchase)[0].cells[3].textContent = UI.addDecimalPart(data.ItemID.purchaseTotal);
                    //change purchase total in textbox
                    $('#PurchaseInfo_Total').val(UI.addDecimalPart(data.ItemID.purchaseTotal));
                    $('#lblPurchaseInfo_Total').html($('#PurchaseInfo_Total').val());
                    UI.applyFormat('currency');
                    PURCHASE.updateSelectOfCoupons($('#PurchaseInfo_PurchaseID').val());
                    //ask to the user to change related fields in service(s) refunded to apply changes in totals
                    //data.ResponseMessage += '<br />Please save changes on service(s) refunded in order to apply them to the total of the purchase.';
                }
                $(PURCHASE.oPurchasePaymentTable.$('tr#purchasePayment_' + data.ItemID.paymentID)[0].cells[3]).addClass('negative-amount');
            }
            if (data.ItemID.errorCode != 'Approved') {
                $(PURCHASE.oPurchasePaymentTable.$('tr#purchasePayment_' + data.ItemID.paymentID)[0].cells[7]).addClass('declined');
            }
            else if ($(PURCHASE.oPurchasePaymentTable.$('tr#purchasePayment_' + data.ItemID.paymentID)[0].cells[7]).hasClass('declined')) {
                $(PURCHASE.oPurchasePaymentTable.$('tr#purchasePayment_' + data.ItemID.paymentID)[0].cells[7]).removeClass('declined');
            }
            PURCHASE.updatePaymentDetailsSpan();
            $('.print-ticket').show();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var makeTblPurchasePaymentRowsSelectable = function () {
        PURCHASE.oPurchasePaymentTable = $.fn.DataTable.fnIsDataTable('tblPurchasesPayments') ? $('#tblPurchasesPayments') : $('#tblPurchasesPayments').dataTable();
        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row)').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    PURCHASE.oPurchasePaymentTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/MasterChart/GetPurchasePayment',
                        cache: false,
                        type: 'POST',
                        data: { PurchasePayment_PaymentDetailsID: $(this).attr('id').substr($(this).attr('id').indexOf('_') + 1) },
                        //beforeSend: function (xhr) {
                        //    UI.checkForPendingRequests(xhr);
                        //},
                        success: function (data) {
                            $('#PurchasePayment_PaymentDetailsID').val(data.PurchasePayment_PaymentDetailsID);
                            //$('#PurchasePayment_Amount').val(data.PurchasePayment_Amount);
                            $('#PurchasePayment_Amount').val((data.PurchasePayment_Amount < 0 ? (data.PurchasePayment_Amount * -1) : data.PurchasePayment_Amount));
                            $('#PurchasePayment_TransactionType option[value="' + data.PurchasePayment_TransactionType + '"]').attr('selected', true);
                            $('#PurchasePayment_TransactionType option[value="' + data.PurchasePayment_TransactionType + '"]').trigger('change')
                            $('#PurchasePayment_TransactionType').on('mousedown', function (e) {
                                e.preventDefault();
                            }).addClass('field-disabled');
                            $('#PurchasePayment_PaymentType option[value="' + data.PurchasePayment_PaymentType + '"]').attr('selected', true);
                            $('#PurchasePayment_PaymentType option[value="' + data.PurchasePayment_PaymentType + '"]').trigger('change', { chargeBackConcept: data.PurchasePayment_ChargeBackConcept, applyCommission: data.PurchasePayment_ApplyCommission, opc: data.PurchasePayment_OPC, company: data.PurchasePayment_Company, promotionTeam: data.PurchasePayment_PromotionTeam, budget: data.PurchasePayment_Budget, cardType: data.PurchasePayment_CardType });
                            $('#PurchasePayment_CertificateNumbers').val(data.PurchasePayment_CertificateNumbers);

                            $('#PurchasePayment_Currency option[value="' + data.PurchasePayment_Currency + '"]').attr('selected', true);
                            $('#PurchasePayment_RefundAccount').val(data.PurchasePayment_RefundAccount);
                            var _billingInfo = 0;
                            if (data.PurchasePayment_ReferenceNumber != null && data.PurchasePayment_ReferenceNumber != '') {
                                _billingInfo = -2;
                            }
                            else {
                                _billingInfo = data.PurchasePayment_BillingInfo;
                            }
                            $('#PurchasePayment_BillingInfo option[value="' + _billingInfo + '"]').attr('selected', true);
                            $('#PurchasePayment_BillingInfo').trigger('change');
                            //$('#PurchasePayment_OPC option[value="' + data.PurchasePayment_OPC + '"]').attr('selected', true);
                            //$('#PurchasePayment_OPC option[value="' + data.PurchasePayment_OPC + '"]').trigger('change', { company: data.PurchasePayment_Company });
                            $('#PurchasePayment_Other').val(data.PurchasePayment_Other);
                            $('#PurchasePayment_Location option[value="' + data.PurchasePayment_Location + '"]').attr('selected', true);
                            $('#PurchasePayment_Invitation').val(data.PurchasePayment_Invitation);
                            $('#PurchasePayment_PaymentComments').val(data.PurchasePayment_PaymentComments);
                            $('#PurchasePayment_TransactionCode').val(data.PurchasePayment_TransactionCode);
                            $('#PurchasePayment_ReferenceNumber').val(data.PurchasePayment_ReferenceNumber);
                            //$('#PurchasePayment_CardType option[value="' + data.PurchasePayment_CardType + '"]').attr('selected', true);
                            //if (data.PurchasePayment_ApplyCommission) {
                            //    $('input[name="PurchasePayment_ApplyCommission"]')[0].checked = true;
                            //}
                            //else {
                            //    $('input[name="PurchasePayment_ApplyCommission"]')[1].checked = true;
                            //}
                            $('#PurchasePayment_DateSaved').val(data.PurchasePayment_DateSaved);

                            //if (!data.PurchasePayment_ApplyCharge) {
                            //    $('input[name="PurchasePayment_ApplyCharge"]')[1].checked = true;
                            //    $('input[name="PurchasePayment_ApplyCharge"]').trigger('change');
                            //    $('#divApplyCharge').hide();
                            //}
                            //else {
                            //    $('input[name="PurchasePayment_ApplyCharge"]')[0].checked = true;
                            //    $('input[name="PurchasePayment_ApplyCharge"]').trigger('change');
                            //    $('#divApplyCharge').show();
                            //}
                            UI.expandFieldset('fdsPurchasePaymentInfo');
                            UI.scrollTo('fdsPurchasePaymentInfo', null);
                        }
                    });
                }
                else {
                    if (!$('#fdsPurchasePaymentInfo').children('div:first').is(':visible')) {
                        UI.expandFieldset('fdsPurchasePaymentInfo');
                    }
                    UI.scrollTo('fdsPurchasePaymentInfo', null);
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deletePayment, [$(this).attr('id').substr($(this).attr('id').indexOf('_') + 1)]);
            }
        });
    }

    var allowAccessToServices = function () {
        $.post('/MasterChart/AllowAccessToServices', null, function (data) {
            if (data.ItemID.ManageServices == true) {
                PURCHASE.init();
                //$.getJSON('/MasterChart/GetExchangeRates', {}, function (data) {
                //    $('#currentExchangeRates').text(data);
                //});
                if (location.hash != '') {
                    $.getJSON('/crm/MasterChart/GetBuyer', { purchaseID: location.hash.substr(12) }, function (data) {
                        var _terminals = JSON.parse('[' + UI.selectedTerminals + ']');
                        if (_terminals.indexOf(data.terminalID) != -1) {
                            $('#frmLeadSearch').find('input:reset').click();
                            $('#Search_LeadID').val(data.leadID);
                            $('#frmLeadSearch').find('input:submit').click();
                            $('#Search_LeadID').val('');
                        }
                        else {
                            if ($('#divAvailableTerminals').find('input:checkbox[value="' + data.terminalID + '"]').length > 0) {
                                $('#divAvailableTerminals').find('input:checkbox[value="' + data.terminalID + '"]').attr('checked', true);
                                UI.terminalsClose();
                                UI.saveTicket(function () {
                                    $('#frmLeadSearch').find('input:reset').click();
                                    $('#Search_LeadID').val(data.leadID);
                                    $('#frmLeadSearch').find('input:submit').click();
                                    $('#Search_LeadID').val('');
                                    UI.loadFlag = true;
                                });
                            }
                            else {
                                UI.messageBox(-1, "You cannot access to the purchase because it is related to a terminal you don't have access", null, null);
                            }
                        }
                    });
                }
                else {
                    $('#frmLeadSearch').find('input:reset').click();
                }
            }
        });
    }

    var preselectFieldsValue = function () {
        //preselect last point of sale used, stored in localStorage
        $('#PurchaseInfo_PointOfSale option[value="' + localStorage.Eplat_PointOfSale + '"]').attr('selected', true);

        //preselect english as language
        $('#PurchaseInfo_Culture option[value="en-US"]').attr('selected', true);

        //preselect in process as default purchaseStatus
        $('#PurchaseInfo_PurchaseStatus option[value="1"]').attr('selected', true);

        //preselect requested as default serviceStatus
        $('#PurchaseService_ServiceStatus option[value="1"]').attr('selected', true);

        //preselect terminal if only one is selected
        if (UI.selectedTerminals.split(',').length == 1) {
            //preselect mxn as currency in forms
            if (UI.selectedTerminals == '5') {//Vallarta
                $('#PurchaseInfo_Currency option[value="MXN"]').attr('selected', true);
            } else if (UI.selectedTerminals == '7') {//Cancun
                $('#PurchaseInfo_Currency option[value="USD"]').attr('selected', true);
            }
            //leadGeneralInformation
            $('#GeneralInformation_TerminalID option[value="' + UI.selectedTerminals + '"]').attr('selected', true);
            //purchaseInfo
            $('#PurchaseInfo_Terminal option[value="' + UI.selectedTerminals + '"]').attr('selected', true);
            $('#PurchaseInfo_Terminal').trigger('change');
        }

        //preselect leadSatatus "New" in lgi
        $('#GeneralInformation_LeadStatusID option[value="1"]').attr('selected', true);

        $('#GeneralInformation_LeadSourceID option[value="20"]').attr('selected', true);
    }

    var updatePromosPerServiceDate = function (params) {
        //get value of promo previously selected
        var _promo = params != undefined ? params.promo != undefined ? params.promo : 0 : 0;
        //get promos of service in specified date
        $.getJSON('/MasterChart/GetDDLData', { itemType: 'promo', itemID: (params != undefined ? params.service : 0) + '|' + (params != undefined ? params.datetime : 0) }, function (data) {
            //fill list of promos
            $('#PurchaseService_Promo').fillSelect(data, false);
            $('#PurchaseService_Promo option[value="' + _promo + '"]').attr('selected', true);
            $('#PurchaseService_Promo').trigger('change', { promo: _promo });
        });
    }

    var bindPrintFunction = function () {
        //event assigned to print button on each row
        PURCHASE.oPurchaseServiceTable.$('.print-coupon').unbind('click').on('click', function () {
            //var _serviceID = $(this).attr('id') != undefined ? $('#PurchaseService_PurchaseServiceID').val().split('_')[1] : $(this).parents('tr:first').attr('id').split('_')[1];
            var _serviceID = $(this).parents('tr:first').attr('id').split('_')[1];
            $.ajax({
                url: '/Masterchart/GetCouponRefObj',
                cache: false,
                type: 'POST',
                data: { purchaseServiceID: _serviceID },
                //beforeSend: function(xhr){
                //    UI.checkForPendingRequests(xhr);
                //},
                success: function (data) {
                    if (data.CouponRef != '') {
                        printCoupon(data.CouponRef + '-' + _serviceID + '?username=' + $('#ufirstname').val() + '_' + $('#ulastname').val());
                        //printCoupon(data.CouponRef + '-' + _serviceID);
                    }
                    else {
                        UI.messageBox(-1, 'An error ocurred trying to get the coupon(s) reference. Please contact System Administrator', -1, null);
                    }
                }
            });
        });
        //event assigned to get button on each row and also to button get coupon inside the form
        PURCHASE.oPurchaseServiceTable.$('.get-coupon').unbind('click').on('click', function () {
            var _serviceID = $(this).parents('tr:first').attr('id').split('_')[1];
            //if ($('#purchaseService_' + _serviceID).data('issued').toString() != 'true' && $('#purchaseService_' + _serviceID).attr('data-issued').toString() != 'true') {
            $.ajax({
                url: '/Masterchart/GetCouponRefObj',
                cache: false,
                type: 'POST',
                data: { purchaseServiceID: _serviceID },
                success: function (data) {
                    if (data.CouponRef != '') {
                        var _link = '';
                        if ((PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).data('issued').toString().toLowerCase() != 'true' && PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).attr('data-issued').toString().toLowerCase() != 'true') || ($('#tblPurchasesServices').data('reprint').toString().toLowerCase() != 'false' && $('#tblPurchasesServices').attr('data-reprint').toString().toLowerCase() != 'false')) {
                            //_link = '<a id="aGetCoupon" href="' + (data.CouponRef + '-' + _serviceID) + '" target="_blank">' + (data.CouponRef + '-' + _serviceID) + '</a>';
                            _link = '<a id="aGetCoupon" href="' + (data.CouponRef + '-' + _serviceID) + '#a4' + '" target="_blank">Open Coupon</a>';
                        }
                        UI.twoActionBox('Coupon <br />' + $('#PurchaseServiceDetailModel_PurchaseServiceDetail_Coupon').val() + '<br />'
                            + _link
                            + '<br /><div class="align-from-top align-from-bottom"><input type="checkbox" id="chkSendProvider" /> Send to Provider: ' + (data.ProviderEmail != '' ? data.ProviderEmail : 'Not Defined') + '<br /><br /><input type="checkbox" id="chkSendOther" /> Send to Other: <input type="text" id="txtSendOther" /></div>', printCoupon, [(data.CouponRef + '-' + _serviceID)], 'print coupon', sendCouponsByEmail, [$('#PurchaseInfo_PurchaseID').val(), _serviceID], 'send by email');
                        $('#aGetCoupon').unbind('click').on('click', function () {
                            $.ajax({
                                url: '/crm/MasterChart/SetPurchaseServiceAsIssued',
                                type: 'POST',
                                cache: false,
                                data: { PurchaseService_PurchaseServiceID: _serviceID },
                                success: function (data) {
                                    if (data.Issued == true) {
                                        PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).attr('data-issued', 'true');
                                        PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).data('issued', 'true');
                                        if ($('#tblPurchasesServices').data('reprint').toString().toLowerCase() != 'true' && $('#tblPurchasesServices').attr('data-reprint').toString().toLowerCase() != 'true') {
                                            PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).find('.print-coupon').remove();
                                            if ($('#tblPurchasesServices').data('resend').toString().toLowerCase() != 'true' && $('#tblPurchasesServices').attr('data-resend').toString().toLowerCase() != 'true') {
                                                PURCHASE.oPurchaseServiceTable.$('tr#purchaseService_' + _serviceID).find('.get-coupon').remove();
                                            }
                                        }
                                    }
                                }
                            });
                        });
                    }
                    else {
                        UI.messageBox(-1, 'An error ocurred trying to get the coupon(s) reference. Please contact System Administrator', -1, null);
                    }
                }
            });
        });
    }

    var updatePaymentDetailsSpan = function () {
        var _total = $('#PurchaseInfo_Total').val() != '' && $('#PurchaseInfo_Total').val() != 0 ? parseFloat($('#PurchaseInfo_Total').val()) : 0;
        $('#spanPaymentsDetails').html('Total:<br /><span data-format="currency">' + UI.addDecimalPart(_total) + ' ' + $('#PurchaseInfo_Currency').val() + '</span>'
            + '<br />Payments:<br /><span data-format="currency">0.00 ' + $('#PurchaseInfo_Currency').val() + '</span>'
            + '<br />Due:<br /><span id="spanDue" ' + (_total > 0 ? 'style="color:#FF0000"' : '') + ' data-format="currency">' + UI.addDecimalPart(_total) + ' ' + $('#PurchaseInfo_Currency').val() + '</span>');
        var purchaseCurrency = $('#PurchaseInfo_Currency').val();
        var paymentCurrency = purchaseCurrency;
        var paymentsInPurchaseCurrency = 0;
        var _payments = 0;
        var due = 0;
        var _rows = PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row)').length;

        PURCHASE.oPurchasePaymentTable.$('tr:not(.disabled-row)').each(function (index, item) {
            _rows--;
            if ($(item).attr('id') != undefined && $(item).children('td').hasClass('declined') == false) {
                $.ajax({
                    async: false,
                    url: '/MasterChart/GetSpecificRate',
                    type: 'POST',
                    cache: false,
                    data: { date: $(item)[0].cells[5].textContent.trim(), currency: purchaseCurrency, terminalid: $('#PurchaseInfo_Terminal').val(), pos: $('#PurchaseInfo_PointOfSale').val() },
                    success: function (data) {
                        var absAmount = $(item)[0].cells[3].textContent.trim().split(' ')[0].indexOf('-') != -1 ? $(item)[0].cells[3].textContent.trim().split(' ')[0].substr(1) : $(item)[0].cells[3].textContent.trim().split(' ')[0];
                        var _amount = 0;
                        if ($(item)[0].cells[3].textContent.trim().split(' ')[1] == purchaseCurrency) {
                            _amount = parseFloat(absAmount);
                        }
                        else {
                            _amount = (parseFloat(absAmount) * parseFloat($(item)[0].cells[2].textContent.trim())) / parseFloat(data);
                            paymentCurrency = $(item)[0].cells[3].textContent.trim().split(' ')[1];
                        }
                        _amount = _amount - ($(item).find('td:nth-child(2)').find(':hidden.hdn-commission-applied').val() == 'True' ? _amount - (_amount / (1 + ($(item).find('td:nth-child(2)').find(':hidden.hdn-bank-commission').val() / 100))) : 0);
                        if ($(item)[0].cells[3].textContent.trim().split(' ')[0].indexOf('-') != -1) {
                            _payments -= _amount;
                        }
                        else {
                            _payments += _amount;
                        }
                    }
                });
            }
        });
        if (_rows == 0) {
            due = _total - _payments;
            $('#spanPaymentsDetails').html('Total:<br /><span data-format="currency">' + UI.addDecimalPart(_total) + ' ' + purchaseCurrency + '</span>'
                //+ '<br />Payments:<br /><span data-format="currency">' + UI.addDecimalPart(_payments) + ' ' + purchaseCurrency + '</span>'
                + '<br />Payments:<br /><span data-format="currency">' + UI.addDecimalPart(_payments) + ' ' + purchaseCurrency + '</span>'
                + '<br />Due:<br /><span id="spanDue" ' + (Number(UI.addDecimalPart(due)).toFixed(2) > 0 ? 'style="color:#FF0000"' : '') + ' data-format="currency">' + Number(UI.addDecimalPart(due)).toFixed(2) + ' ' + purchaseCurrency + '</span>');

            var _oTable = SEARCH.oTable != undefined ? SEARCH.oTable : $('#tblSearchResult_Leads tbody');
            if (due <= 0) {
                _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(7)').text('Paid');
            }
            else {
                _oTable.find('tr#' + $('#GeneralInformation_LeadID').val()).children('td:nth-child(7)').text('Unpaid');
            }
        }
        UI.applyFormat('currency');
        //UI.applyFormat('currency');
    }

    function getPaymentsHistory(commission, currency) {
        var _payments = 0;
        PURCHASE.oPurchasePaymentTable.$('tr').each(function () {
            if ($(this).attr('id') != undefined && $(this).children('td').hasClass('declined') == false) {
                var _row = $(this);
                //returns exchange rate
                $.ajax({
                    url: '/MasterChart/GetSpecificRate',
                    type: 'POST',
                    cache: false,
                    //data: { date: $(this)[0].cells[6].textContent.trim(), currency: currency, terminalid: $('#PurchaseInfo_Terminal').val() },
                    data: { date: $(this)[0].cells[6].textContent.trim(), currency: currency, terminalid: $('#PurchaseInfo_Terminal').val(), pos: $('#PurchaseInfo_PointOfSale').val() },
                    //beforeSend: function (xhr) {
                    //    UI.checkForPendingRequests(xhr);
                    //},
                    success: function (data) {
                        var _amount = 0;
                        if ($(_row)[0].cells[3].textContent.trim().split(' ')[1] == currency) {
                            _amount = parseFloat($(_row)[0].cells[3].textContent.trim().split(' ')[0]);
                        }
                        else {
                            _amount = (parseFloat($(_row)[0].cells[3].textContent.trim().split(' ')[0]) * parseFloat($(_row)[0].cells[2].textContent.trim())) / parseFloat(data);
                        }
                        //_amount = _amount - ($(_row).find('td:nth-child(2)').find(':hidden.hdn-commission-applied').val() == 'True' ? _amount - (_amount / (1 + (commission / 100))) : 0);
                        _amount = _amount - ($(_row).find('td:nth-child(2)').find(':hidden.hdn-commission-applied').val() == 'True' ? _amount - (_amount / (1 + ($(_row).find('td:nth-child(2)').find(':hidden.hdn-bank-commission').val() / 100))) : 0);
                        _payments += _amount;
                        //console.log('b');
                    }
                });
            }
            //console.log('c');
        });
        //console.log(_payments);
        return { payments: parseFloat(Number(_payments).toFixed(2)) };
    }

    var deletePayment = function (paymentID) {
        $.ajax({
            url: '/MasterChart/DeletePayment',
            cache: false,
            type: 'POST',
            data: { targetID: paymentID },
            //beforeSend: function (xhr) {
            //    UI.checkForPendingRequests(xhr);
            //},
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    $('#purchasePayment_' + data.ItemID).addClass('disabled-row');
                    $('#purchasePayment_' + data.ItemID).unbind('click');
                    //PURCHASE.oPurchasePaymentTable.fnDeleteRow($('#purchasePayment_' + data.ItemID)[0]);
                    //UI.tablesHoverEffect();
                    //UI.tablesStripedEffect();
                    PURCHASE.updatePaymentDetailsSpan();
                    $('#frmPurchasePaymentInfo').clearForm(true);
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function teamHasBudget(promotionTeamID) {
        $.getJSON('/MasterChart/GetDDLData', { itemType: 'teamWithBudget', itemID: promotionTeamID }, function (data) {
            return data;
        });
    }


    return {
        init: init,
        show: show,
        getDisabledDaysOfService: getDisabledDaysOfService,
        searchResultsTable: searchResultsTable,
        closedDays: closedDays,
        deleteItemFromTable: deleteItemFromTable,
        savePurchaseSuccess: savePurchaseSuccess,
        updateTblPurchases: updateTblPurchases,
        updateSelectOfCoupons: updateSelectOfCoupons,
        updateTblPurchaseServices: updateTblPurchaseServices,
        savePurchaseServiceSuccess: savePurchaseServiceSuccess,
        makeTblPurchasesSelectable: makeTblPurchasesSelectable,
        searchResultsPurchaseServicesTable: searchResultsPurchaseServicesTable,
        makeTblPurchaseServiceRowsSelectable: makeTblPurchaseServiceRowsSelectable,
        searchResultsPurchasePaymentsTable: searchResultsPurchasePaymentsTable,
        updateTblPurchasePayments: updateTblPurchasePayments,
        updateTblPurchaseTickets: updateTblPurchaseTickets,
        makePurchaseTicketsPrintable: makePurchaseTicketsPrintable,
        savePurchasePaymentSuccess: savePurchasePaymentSuccess,
        makeTblPurchasePaymentRowsSelectable: makeTblPurchasePaymentRowsSelectable,
        allowAccessToServices: allowAccessToServices,
        preselectFieldsValue: preselectFieldsValue,
        updatePromosPerServiceDate: updatePromosPerServiceDate,
        bindPrintFunction: bindPrintFunction,
        updatePaymentDetailsSpan: updatePaymentDetailsSpan,
        deletePayment: deletePayment,
        searchCouponsResult: searchCouponsResult,
        updateTblRecentCouponsAndJson: updateTblRecentCouponsAndJson,
        loadCouponsTableFromJson: loadCouponsTableFromJson,
        searchCouponsByDateResult: searchCouponsByDateResult
    }
}();