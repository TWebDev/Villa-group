$(function () {
    ACCOUNT.init();
});

var ACCOUNT = function () {

    var oEgressesTable;

    var oIncomesTable;

    var init = function () {

        $('select[multiple="multiple"]').multiselect({
            noneSelectedText: "--All--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();

        if ($(window).width() > 768) {
            $('#OutcomeInfo_Opc').multiselect({
                multiple: false,
                selectedList: 1,
                noneSelectedText: '--Select One--'
            }).multiselectfilter();
        }

        //if (localStorage.Eplat_FastSale_Terminals != null && $('#OutcomeInfo_Terminal option[value="' + FS.returnMostSelectedValue(localStorage.Eplat_FastSale_Terminals, 'Terminals') + '"]').length > 0) {
        //    $('#OutcomeInfo_Terminal option[value="' + FS.returnMostSelectedValue(localStorage.Eplat_FastSale_Terminals, 'Terminals') + '"]').attr('selected', true);
        //    $('#OutcomeInfo_Terminal option[value="' + FS.returnMostSelectedValue(localStorage.Eplat_FastSale_Terminals, 'Terminals') + '"]').trigger('change');
        //}


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

        $('#OutcomeInfo_Amount').numeric({ negative: false });

        $('#OutcomeInfo_AdminFee').numeric({ negative: false });

        $('#OutcomeInfo_AmountOfSale').numeric({ negative: false });

        $('#IncomeInfo_Amount').numeric({ negative: false });

        $('#OutcomeInfo_EgressType').on('change', function (e, params) {
            $('#OutcomeInfo_EgressConcept option').eq(0).prop('selected', true);
        });

        $.ajax({
            url: '/crm/TimeShare/GetDependantEgressConcepts',
            cache: false,
            //data: { terminalID: $('#OutcomeInfo_Terminal').val() },
            success: function (data) {
                localStorage.Eplat_DependantEgressConcepts = JSON.stringify(data);
                UI.loadDependantFields(data);
                //console.log(params);
                //if (params != undefined) {

                //    $('#OutcomeInfo_EgressConcept').off('OutcomeInfo_EgressConceptLoaded').on('OutcomeInfo_EgressConceptLoaded', function () {
                //        //$('#OutcomeInfo_EgressType option[value="' + params.egressType + '"]').attr('selected', true);
                //        $('#OutcomeInfo_EgressConcept option[value="' + params.egressConcept + '"]').attr('selected', true);
                //    });
                //}
                ////$('#OutcomeInfo_EgressType').trigger('change');
                //$('#OutcomeInfo_EgressConcept').trigger('change');
            }
        });

        $('#OutcomeInfo_Terminal').on('change', function (e, params) {

            $.getJSON('/TimeShare/GetDDLData', { itemType: 'location', itemID: $(this).val() }, function (data) {
                $('#OutcomeInfo_Location').fillSelect(data);
                if (params != undefined) {
                    $('#OutcomeInfo_Location option[value="' + params.location + '"]').attr('selected', true);
                }
            });

            $.getJSON('/TimeShare/GetDDLData', { itemType: 'pointOfSale', itemID: $(this).val() }, function (data) {
                $('#OutcomeInfo_PointOfSale').fillSelect(data);
                if (params != undefined) {
                    $('#OutcomeInfo_PointOfSale option[value="' + params.pointOfSale + '"]').attr('selected', true);
                }
                else {
                    if (localStorage.Eplat_FastSale_PointsOfSale != null && $('#OutcomeInfo_PointOfSale option[value="' + FS.returnMostSelectedValue(localStorage.Eplat_FastSale_PointsOfSale, 'PointsOfSale').split('|')[0] + '"]').length > 0) {
                        $('#OutcomeInfo_PointOfSale option[value="' + FS.returnMostSelectedValue(localStorage.Eplat_FastSale_PointsOfSale, 'PointsOfSale').split('|')[0] + '"]').attr('selected', true);
                        $('#OutcomeInfo_PointOfSale').trigger('change');
                    }
                }
            });

            $.getJSON('/TimeShare/GetDDLData', { itemType: 'opcs', itemID: $(this).val() }, function (data) {
                $('#OutcomeInfo_Opc').fillSelect(data);
                if (params != undefined) {
                    $('#OutcomeInfo_Opc option[value="' + params.opc + '"]').attr('selected', true);
                    $('#OutcomeInfo_Opc').trigger('change', params);
                }
            });
        });

        $('.update-opc-list').on('click', function () {
            $.getJSON('/TimeShare/GetDDLData', {
                itemType: 'opcs', itemID: $('#OutcomeInfo_Terminal option:selected').val(), ts: new Date().getTime() }, function (data) {
                $('#OutcomeInfo_Opc').fillSelect(data);
                $('#OutcomeInfo_Opc').trigger('change');
            });
        });

        $('#OutcomeInfo_EgressConcept').on('change', function () {
            if ($(this).val() != '') {
                $.ajax({
                    url: '/crm/TimeShare/ConceptAllowsBudget',
                    cache: false,
                    data: { egressConceptID: $(this).val() },
                    success: function (data) {
                        $('#hdnConceptAllowsBudget').val(data);
                        if ($('#OutcomeInfo_EgressConcept option:selected').val() == 24) {
                            $('.amount-of-sale').show();
                        }
                        else {
                            $('#OutcomeInfo_AmountOfSale').val('');
                            $('.amount-of-sale').hide();
                        }
                        //if ($('#OutcomeInfo_EgressConcept option:selected').text().toLowerCase() != 'cash gift') {
                        if (data == true) {
                            $('#OutcomeInfo_Budget option').filter(function () { return $(this).val() != '0' }).show();
                        }
                        else {
                            $('#OutcomeInfo_Budget option').filter(function () { return $(this).val() != '0' }).hide();
                            $('#OutcomeInfo_Budget option[value="0"]').attr('selected', true);
                        }
                    }
                });
            }
        });

        $('#OutcomeInfo_Opc').on('change', function (e, params) {

            if ($('#OutcomeInfo_Terminal').val() != null) {
                $.getJSON('/TimeShare/GetDDLData', { itemType: 'opc', itemID: $('#OutcomeInfo_Terminal').val() + '|' + $(this).val() }, function (data) {
                    $('#OutcomeInfo_ChargedToCompany').fillSelect(data);
                    if (params != undefined) {
                        $('#OutcomeInfo_ChargedToCompany option[value="' + (params.company != null ? params.company : 'null') + '"]').attr('selected', true);
                    }
                });
            }
            $.getJSON('/TimeShare/GetDDLData', { itemType: 'opcTeam', itemID: $(this).val() }, function (data) {
                $('#OutcomeInfo_PromotionTeam').clearSelect();
                $('#OutcomeInfo_PromotionTeam').fillSelect(data, false);
                if (params != undefined) {
                    $('#OutcomeInfo_PromotionTeam option[value="' + params.promotionTeam + '"]').attr('selected', true);
                }
                $('#OutcomeInfo_PromotionTeam').trigger('change', params);
            });
            if ($(this).val() == 'null') {
                var _opc = '';
                if (params != undefined) {
                    _opc = params.otheropc;
                }
                else {
                    if ($('#OPC').val() != '') {
                        _opc = $('#OPC').val();
                    }
                }
                $('#OutcomeInfo_OtherOpc').val(_opc);
                $('#divOtherOpc').show();
                //var _opc = params != undefined ? params.otheropc : '';
                //$('#OutcomeInfo_OtherOpc').val(_opc);
                //$('#divOtherOpc').show();
            }
            else {
                $('#OutcomeInfo_OtherOpc').val('');
                $('#divOtherOpc').hide();
            }
            $(this).multiselect('refresh');
        });

        $('#OutcomeInfo_PromotionTeam').on('change', function (e, params) {
            $.ajax({
                url: '/MasterChart/GetDDLData',
                dataType: 'json',
                data: { itemType: 'budgetsPerTeam', itemID: $(this).val() },
                success: function (data) {
                    $('#OutcomeInfo_Budget').fillSelect(data, false);
                    $('#OutcomeInfo_Budget option').each(function () {
                        $(this).show();
                        if ($(this).val().indexOf('Extension') != -1) {
                            $('#divUseExtension').show();
                        }
                    });
                    //if ($('#OutcomeInfo_EgressConcept option:selected').text().toLowerCase() != 'cash gift') {
                    if ($('#hdnConceptAllowsBudget').val().toLowerCase() == 'true') {
                        $('#OutcomeInfo_Budget option').filter(function () { return $(this).val() != '0' }).show();
                    }
                    else {
                        $('#OutcomeInfo_Budget option').filter(function () { return $(this).val() != '0' }).hide();
                    }
                    //posible falta el else para .show
                    if (params != undefined) {
                        if (params.budget != undefined) {
                            $('#OutcomeInfo_Budget option[value="' + params.budget + '"]').attr('selected', true);
                        }
                    }
                    else {
                        $('#OutcomeInfo_Budget option').each(function () {
                            if ($(this).val().indexOf('Extension') != -1) {
                                $(this).hide();
                            }
                        });
                    }
                }
            });
        });

        $('#btnSaveAndPrintEgress').on('click', function () {
            $('#OutcomeInfo_SaveAndPrint').val('True');
            if ($('#frmEgressInfo').valid()) {
                $('#frmEgressInfo').submit();
            }
            else {
                UI.showValidationSummary('frmEgressInfo');
            }
        });

        //$('#btnSaveEgress').on('click', function () {
        $('#btnSaveEgress, #btnEditEgress').on('click', function (e) {
            $('#OutcomeInfo_SaveAndPrint').val('False');
            if ($('#frmEgressInfo').valid()) {
                $('#frmEgressInfo').submit();
            }
            else {
                UI.showValidationSummary('frmEgressInfo');
            }
        });

        $('#btnNewEgressInfo').on('click', function () {
            $('#divCreateEgress').show();
            $('#divCreatePrintEgress').show();
            $('#divEditEgress').hide();
        });

        $('#btnResetVarApp').on('click', function () {
            UI.confirmBox('Do you confirm you want to proceed?', resetFund, [$('#IncomeInfo_ResetFund option:selected').val(), $('#IncomeInfo_ResetAmount').val()]);
        });

        function resetFund(fund, amount) {
            $.ajax({
                url: '/TimeShare/ResetVarApp',
                type: 'POST',
                cache: false,
                data: { fundID: fund, amount: amount },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    //call to update funds
                    ACCOUNT.getFundsBalance();
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        }

        ACCOUNT.searchResultsTable();

        ACCOUNT.searchIncomeResultsTable();

        ACCOUNT.getFundsBalance();

        UI.updateListsOnTerminalsChange($('#OutcomeInfo_Terminal').attr('id'), ACCOUNT.preselectTerminalOnLoad);
    }

    var preselectTerminalOnLoad = function () {
        //if (UI.selectedTerminals.split(',').length == 1) {
        //    $('#OutcomeInfo_Terminal option[value="' + UI.selectedTerminals + '"]').attr('selected', true);
        //}

        if (localStorage.Eplat_FastSale_Terminals != null && $('#OutcomeInfo_Terminal option[value="' + FS.returnMostSelectedValue(localStorage.Eplat_FastSale_Terminals, 'Terminals') + '"]').length > 0) {
            $('#OutcomeInfo_Terminal option[value="' + FS.returnMostSelectedValue(localStorage.Eplat_FastSale_Terminals, 'Terminals') + '"]').attr('selected', true);
            //$('#OutcomeInfo_Terminal').trigger('change');
        }
        else {
            if (UI.selectedTerminals.split(',').length == 1) {
                $('#OutcomeInfo_Terminal option[value="' + UI.selectedTerminals + '"]').attr('selected', true);
            }
        }
        $('#OutcomeInfo_Terminal').trigger('change');
        //$('#OutcomeInfo_EgressType').trigger('change');
        $('#OutcomeInfo_Opc').trigger('change');
    }

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblEgressesSearchResults', tableColumns.length - 1);
        ACCOUNT.oEgressesTable = $('#tblEgressesSearchResults').dataTable();
        ACCOUNT.bindPrintingFunction();
    }

    var makeTableEgressesRowsSelectable = function () {
        ACCOUNT.oEgressesTable.$('tr').not('theader').off('click').on('click', function (e) {
            if (!$(e.target).is('input:button')) {
                if (!$(this).hasClass('selected-row')) {
                    ACCOUNT.oEgressesTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $('#OutcomeInfo_EgressID').val($(this).attr('id').substr($(this).attr('id').indexOf('_') + 1));
                    $.ajax({
                        url: '/TimeShare/GetEgressInfo',
                        cache: false,
                        type: 'POST',
                        data: { OutcomeInfo_EgressID: $('#OutcomeInfo_EgressID').val() },
                        beforeSend: function (xhr) {
                            UI.checkForPendingRequests(xhr);
                        },
                        success: function (data) {
                            $('#OutcomeInfo_Terminal option[value="' + data.OutcomeInfo_Terminal + '"]').attr('selected', true);
                            $('#OutcomeInfo_Terminal').trigger('change', { location: data.OutcomeInfo_Location, pointOfSale: data.OutcomeInfo_PointOfSale, opc: data.OutcomeInfo_Opc, otheropc: data.OutcomeInfo_OtherOpc, company: data.OutcomeInfo_ChargedToCompany, promotionTeam: data.OutcomeInfo_PromotionTeam, budget: data.OutcomeInfo_Budget, egressType: data.OutcomeInfo_EgressType, egressConcept: data.OutcomeInfo_EgressConcept });

                            $('#OutcomeInfo_EgressType option[value="' + data.OutcomeInfo_EgressType + '"]').attr('selected', true);
                            $('#OutcomeInfo_EgressType').trigger('change');
                            $('#OutcomeInfo_EgressConcept option[value="' + data.OutcomeInfo_EgressConcept + '"]').attr('selected', true);
                            $('#OutcomeInfo_EgressConcept').trigger('change');
                            $('#OutcomeInfo_Customer').val(data.OutcomeInfo_Customer);
                            $('#OutcomeInfo_Invitation').val(data.OutcomeInfo_Invitation);
                            $('#OutcomeInfo_Amount').val(data.OutcomeInfo_Amount);
                            $('#OutcomeInfo_AmountOfSale').val(data.OutcomeInfo_AmountOfSale);
                            $('#OutcomeInfo_Currency option[value="' + data.OutcomeInfo_Currency + '"]').attr('selected', true);
                            $('#OutcomeInfo_PointOfSale option[value="' + data.OutcomeInfo_PointOfSale + '"]').attr('selected', true);
                            $('#OutcomeInfo_AdminFee').val(data.OutcomeInfo_AdminFee);
                            $('#OutcomeInfo_AgentComments').val(data.OutcomeInfo_AgentComments);
                            $('#OutcomeInfo_DateSaved').val(data.OutcomeInfo_DateSaved);
                            //manifest fields
                            $('#CustomerID').val(data.CustomerID);
                            $('#FrontOfficeGuestID').val(data.FrontOfficeGuestID);
                            $('#FrontOfficeResortID').val(data.FrontOfficeResortID);
                            $('#MarketingProgram').val(data.MarketingProgram);
                            $('#OPCID').val(data.OPCID);
                            $('#Source').val(data.Source);
                            $('#Subdivision').val(data.Subdivision);
                            $('#TourID').val(data.TourID);
                            $('#TourDate').val(data.TourDate);

                            $('#divCreateEgress').hide();
                            $('#divCreatePrintEgress').hide();
                            if (data.OutcomeInfo_UpdateEgress) {
                                $('#divEditEgress').show();
                            }
                            else {
                                $('#divEditEgress').hide();
                            }
                            //$('.visible-on-update').parents('.editor-alignment').first().show();
                            dateSavedDatePicker();
                            UI.expandFieldset('fdsEgressInfo');
                            UI.scrollTo('fdsEgressInfo', null);
                        }
                    });
                }
                else {
                    if (!$('#fdsEgressInfo').children('div:first').is(':visible')) {
                        UI.expandFieldset('fdsEgressInfo');
                    }
                    UI.scrollTo('fdsEgressInfo', null);
                }
            }
        });
    }

    function dateSavedDatePicker() {
        if ($('#OutcomeInfo_DateSaved').is(':text')) {
            var egressMonth = new Date($('#OutcomeInfo_DateSaved').val()).getMonth();
            var egressYear = new Date($('#OutcomeInfo_DateSaved').val()).getFullYear();
            if ($('#OutcomeInfo_DateSaved').hasClass('hasDatepicker')) {
                $('#OutcomeInfo_DateSaved').datepicker('destroy');
                //if ($('#divSelectedRole').text().indexOf('Administrative Assistant MCA') != -1 || $('#divSelectedRole').text().indexOf('Administrator') != -1) {
                $('#OutcomeInfo_DateSaved').datepicker({
                    dateFormat: 'yy-mm-dd',
                    minDate: new Date(egressYear, egressMonth, 1),
                    //maxDate: new Date(egressYear, (egressMonth + 1), 0)
                    maxDate: new Date(egressYear, egressMonth, (new Date().getDate()))
                });
                //}
            }
        }
    }

    var saveEgressSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Egress Saved') {
                var oSettings = ACCOUNT.oEgressesTable.fnSettings();
                var iAdded = ACCOUNT.oEgressesTable.fnAddData([
                    data.ItemID.date,
                    ($('#OutcomeInfo_Opc option:selected').val() != 'null' ? $('#OutcomeInfo_Opc option:selected').val() != '' ? $('#OutcomeInfo_Opc option:selected').text() : '' : $('#OutcomeInfo_OtherOpc').val()),
                    $('#OutcomeInfo_ChargedToCompany option:selected').text(),
                    $('#OutcomeInfo_Customer').val(),
                    UI.addDecimalPart($('#OutcomeInfo_Amount').val()) + ' ' + $('#OutcomeInfo_Currency').val(),
                    data.ItemID.agent,
                    $('#OutcomeInfo_Invitation').val(),
                    $('#OutcomeInfo_EgressType option:selected').text() + ' > ' + $('#OutcomeInfo_EgressConcept option:selected').text(),
                    '<span class="block">'
                    + $('#OutcomeInfo_PointOfSale option:selected').text().split('-')[1].trim()
                    + '</span>',
                    '<span class="block-right">'
                    + ($('#OutcomeInfo_Terminal option:selected').val() == 26 || $('#OutcomeInfo_Terminal option:selected').val() == 61 ? '<input type="button" class="button print-egress-ticket" value="print" />' : '')
                    + '</span>'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'egress_' + data.ItemID.egressID);
                ACCOUNT.oEgressesTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                ACCOUNT.makeTableEgressesRowsSelectable();
                $('#frmEgressInfo').clearForm(true);
                $('#divCreateEgress').show();
                $('#divCreatePrintEgress').show();
                $('#divEditEgress').hide();
            }
            else {
                var array = ACCOUNT.oEgressesTable.fnGetNodes();
                var nTr = ACCOUNT.oEgressesTable.$('tr.selected-row');
                var position = ACCOUNT.oEgressesTable.fnGetPosition(nTr[0]);
                ACCOUNT.oEgressesTable.fnDisplayRow(array[position]);
                ACCOUNT.oEgressesTable.fnUpdate([
                    data.ItemID.date,
                    ($('#OutcomeInfo_Opc option:selected').val() != 'null' ? $('#OutcomeInfo_Opc option:selected').val() != '' ? $('#OutcomeInfo_Opc option:selected').text() : '' : $('#OutcomeInfo_OtherOpc').val()),
                    $('#OutcomeInfo_ChargedToCompany option:selected').text(),
                    $('#OutcomeInfo_Customer').val(),
                    UI.addDecimalPart($('#OutcomeInfo_Amount').val()) + ' ' + $('#OutcomeInfo_Currency').val(),
                    data.ItemID.agent,
                    $('#OutcomeInfo_Invitation').val(),
                    $('#OutcomeInfo_EgressType option:selected').text() + ' > ' + $('#OutcomeInfo_EgressConcept option:selected').text(),
                    '<span class="block">' +
                    $('#OutcomeInfo_PointOfSale option:selected').text().split('-')[1].trim()
                    + '</span>',
                    '<span class="block right">'
                    + ($('#OutcomeInfo_Terminal option:selected').val() == 26 || $('#OutcomeInfo_Terminal option:selected').val() == 61 ? '<input type="button" class="button print-egress-ticket" value="print" />' : '')
                    + '</span>'
                ], $('#egress_' + data.ItemID.egressID)[0], undefined, false);
            }
            ACCOUNT.getFundsBalance();
            ACCOUNT.bindPrintingFunction();
            if (data.ItemID.print) {
                $('#egress_' + data.ItemID.egressID).children('td:nth-child(10)').find('input:button:first').click();
            }
        }
        if (data.ResponseType == 0) {
            var existingFunds = '';
            $.each(data.ItemID.existingFunds.split('|'), function (index, item) {
                existingFunds += (existingFunds != '' ? '<br />' : '') + '<input type="radio" name="existingFunds" value="' + item.split('_')[0] + '" ' + (existingFunds != '' ? '' : 'checked') + ' />' + item.split('_')[1];
            });
            //UI.confirmBox(data.ResponseMessage + '<br />Select which fund to use:<br />' + existingFunds, function () { $('#btnSaveEgress').click(); }, [$('input:radio[name="existingFunds"]:checked').val()]);
            UI.twoActionBox(data.ResponseMessage + '<br />Select which fund to use:<br />' + existingFunds, function () { $('#btnSaveEgress').click(); }, [$('input:radio[name="existingFunds"]:checked').val()], 'confirm', function () { $('#OutcomeInfo_Fund').val(''); }, [], 'cancel');
            ACCOUNT.bindFunctionToRadioFunds();
            $('#OutcomeInfo_Fund').val($('input:radio[name="existingFunds"]:checked').val());
        }
        else {
            $('#OutcomeInfo_Fund').val('');
            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
        }
    }

    var bindFunctionToRadioFunds = function () {
        $('input:radio[name="existingFunds"]').on('change', function () {
            $('#OutcomeInfo_Fund').val($(this).val());
        });
    }

    var searchIncomeResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblIncomesSearchResults', tableColumns.length - 1);
        ACCOUNT.oIncomesTable = $('#tblIncomesSearchResults').dataTable();
    }

    var makeTableIncomesRowsSelectable = function () {
        ACCOUNT.oIncomesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            //if (!$(e.target).is('img')) {
            if (!$(this).hasClass('selected-row')) {
                ACCOUNT.oIncomesTable.$('tr.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $('#IncomeInfo_IncomeID').val($(this).attr('id').substr($(this).attr('id').indexOf('_') + 1));
                $.ajax({
                    url: '/TimeShare/GetIncomeInfo',
                    cache: false,
                    type: 'POST',
                    data: { IncomeInfo_IncomeID: $('#IncomeInfo_IncomeID').val() },
                    beforeSend: function (xhr) {
                        UI.checkForPendingRequests(xhr);
                    },
                    success: function (data) {
                        $('#IncomeInfo_IncomeConcept option[value="' + data.IncomeInfo_IncomeConcept + '"]').attr('selected', true);
                        $('#IncomeInfo_Company option[value="' + data.IncomeInfo_Company + '"]').attr('selected', true);
                        $('#IncomeInfo_Amount').val(data.IncomeInfo_Amount);
                        $('#IncomeInfo_Currency option[value="' + data.IncomeInfo_Currency + '"]').attr('selected', true);
                        $('#IncomeInfo_Fund option[value="' + data.IncomeInfo_Fund + '"]').attr('selected', true);
                        //$('#IncomeInfo_ReceiverUser option[value="' + data.IncomeInfo_ReceiverUser + '"]').attr('selected', true);
                        UI.expandFieldset('fdsIncomeInfo');
                        UI.scrollTo('fdsIncomeInfo', null);
                    }
                });
            }
            else {
                if (!$('#fdsIncomeInfo').children('div:first').is(':visible')) {
                    UI.expandFieldset('fdsIncomeInfo');
                }
                UI.scrollTo('fdsIncomeInfo', null);
            }
            //}
            //else {
            //    UI.deleteDataTableItemFunction('/TimeShare/DeleteIncome', 'tr', $(e.target), ACCOUNT.oIncomesTable, ACCOUNT.getFundsBalance);
            //}
        });
    }

    var saveIncomeSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Income Saved') {
                var oSettings = ACCOUNT.oIncomesTable.fnSettings();
                var iAdded = ACCOUNT.oIncomesTable.fnAddData([
                    data.ItemID.date,
                    $('#IncomeInfo_Company option:selected').text(),
                    UI.addDecimalPart($('#IncomeInfo_Amount').val()) + ' ' + $('#IncomeInfo_Currency').val(),
                    data.ItemID.receiver,
                    $('#IncomeInfo_IncomeConcept option:selected').text()
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'income_' + data.ItemID.incomeID);
                ACCOUNT.oIncomesTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                ACCOUNT.makeTableIncomesRowsSelectable();
                $('#frmIncomeInfo').clearForm();
            }
            else {
                var array = ACCOUNT.oIncomesTable.fnGetNodes();
                var nTr = ACCOUNT.oIncomesTable.$('tr.selected-row');
                var position = ACCOUNT.oIncomesTable.fnGetPosition(nTr[0]);
                ACCOUNT.oIncomesTable.fnDisplayRow(array[position]);
                ACCOUNT.oIncomesTable.fnUpdate([
                    data.ItemID.date,
                    $('#IncomeInfo_Company option:selected').text(),
                    UI.addDecimalPart($('#IncomeInfo_Amount').val()) + ' ' + $('#IncomeInfo_Currency').val(),
                    data.ItemID.receiver,
                    $('#IncomeInfo_IncomeConcept option:selected').text()
                ], $('#income_' + data.ItemID.incomeID)[0], undefined, false);
            }
            ACCOUNT.getFundsBalance();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var getFundsBalance = function () {
        $.getJSON('/TimeShare/GetFundsBalance', {}, function (data) {
            var builder = '';
            if (data != null) {
                try {
                    $.each(data.split('|'), function (index, item) {
                        builder += '<div class="table-cell"><div class="table-row"><span>' + item.split('_')[0] + ': </span>';
                        $.each(item.split('_')[1].split(','), function (index, item) {
                            builder += '<span data-format="currency">' + item + '</span>';
                        });
                        builder += '</div></div>';
                        // builder += '<div class="table-cell"><div class="table-row"><span>' + item.split('_')[0] + ': </span><span data-format="currency">' + item.split('_')[1] + '</span></div></div>';
                    });
                }
                catch (ex) { }
            }
            $('#divFundsBalance').html(builder);
            UI.applyFormat('currency');
        });
    }

    var bindPrintingFunction = function () {
        if (ACCOUNT.oEgressesTable.length > 0) {
            ACCOUNT.oEgressesTable.$('.print-egress-ticket').unbind('click').on('click', function () {
                var _id = $(this).parents('tr:first').attr('id').split('_')[1];
                $.ajax({
                    url: '/TimeShare/GetEgressInfo',
                    cache: false,
                    type: 'POST',
                    data: { OutcomeInfo_EgressID: _id },
                    success: function (data) {
                        var _class = data.OutcomeInfo_EgressConceptText.toString().toLowerCase().split(' ').join('-');
                        $('#divEgressTicket span').each(function () {
                            if ($(this).attr('id')) {
                                $(this).empty();
                            }
                            else {
                                if ($(this).hasClass(_class)) {
                                    $(this).addClass('printable').removeClass('non-printable');
                                }
                                else {
                                    $(this).addClass('non-printable').removeClass('printable');
                                }
                            }
                        });

                        $('#divEgressTicket > p').each(function () {
                            if ($(this).hasClass(_class)) {
                                $(this).removeClass('non-printable').addClass('printable block');
                            }
                            else {
                                $(this).addClass('non-printable').removeClass('printable block');
                            }
                        });

                        $('#ticketConcept').text(data.OutcomeInfo_EgressConceptText + (_class == 'burned-invitation' ? ' Authorization' : ''));
                        $('#egressFolio').text(UI.padNumber(data.OutcomeInfo_EgressID, 4));
                        $('#egressLocation').text((data.OutcomeInfo_LocationText != 'null' ? data.OutcomeInfo_LocationText : "Unknown"));
                        $('#egressDate').text(data.OutcomeInfo_DateSaved);
                        $('#egressSalesAgent').text(data.OutcomeInfo_SavedByUser);
                        $('#egressClient').text(data.OutcomeInfo_Customer);
                        $('#egressOPC').text(data.OutcomeInfo_OtherOpc != null ? data.OutcomeInfo_OtherOpc : $('#OutcomeInfo_Opc option[value="' + data.OutcomeInfo_Opc + '"]').text());
                        $('#egressSaleAmount').text(data.OutcomeInfo_AmountOfSale + ' ' + (data.OutcomeInfo_CurrencyOfSale != '' ? data.OutcomeInfo_CurrencyOfSale : data.OutcomeInfo_Currency));
                        $('#egressBurnAmount').text(data.OutcomeInfo_Amount + ' ' + data.OutcomeInfo_Currency);
                        $('#egressReceivedBy').text(data.OutcomeInfo_Customer);
                        $('#divEgressTicket [data-format=currency]').each(function () {
                            $(this).text(parseFloat($(this).text().split(' ')[0]).formatMoney(2) + ' ' + $(this).text().split(' ')[1]);
                            $(this).prepend('$');
                        });
                        $('#divEgressTicket').printPage();
                    }
                });
            });
        }
    }

    return {
        init: init,
        preselectTerminalOnLoad: preselectTerminalOnLoad,
        searchResultsTable: searchResultsTable,
        makeTableEgressesRowsSelectable: makeTableEgressesRowsSelectable,
        saveEgressSuccess: saveEgressSuccess,
        searchIncomeResultsTable: searchIncomeResultsTable,
        makeTableIncomesRowsSelectable: makeTableIncomesRowsSelectable,
        saveIncomeSuccess: saveIncomeSuccess,
        getFundsBalance: getFundsBalance,
        bindPrintingFunction: bindPrintingFunction,
        bindFunctionToRadioFunds: bindFunctionToRadioFunds
    }
}();
