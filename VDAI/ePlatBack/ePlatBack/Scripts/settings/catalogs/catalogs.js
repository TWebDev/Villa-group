$(function () {
    CATALOG.init();
    $('#DestinationDescription_Description').ckeditor();
});

var CATALOG = function () {
    var oBudgetsTable;
    var oPointsOfSaleTable;
    var oCompaniesTable;
    var oZonesTable;
    var oPlaceTypesTable;
    var oTransportationZonesTable;
    var oDestinationsTable;
    var oPlaceClasificationsTable;
    var oAccountingAccountsTable;
    var oCommissionsTable;
    var oProvidersTable;
    var oExchangeRatesTable;
    var oLocationsTable;
    var oOPCSTable;
    var oPromotionTeamsTable;
    var oCouponFoliosTable;
    var oSalesRoomsPartiesTable;
    var oPromoTable;
    var oOptionsTable;
    var oBanksPartiesTable;
    var oBoatsPartiesTable;
    var oSalesChannelsPartiesTable;
    var oBraceletsPartiesTable;
    var oRemindersPartiesTable;
    var oMarketCodesTable;
    var oUsersLeadSourcesTable;
    var marketCodesArray = new Array();

    var map;
    var myLatLng;
    var markersArray = new Array();

    var init = function () {

        COMMON.getServerDateTime();
        //UI.updateListsOnTerminalsChange(CATALOG.refreshActiveSearchResults);
        UI.updateListsOnTerminalsChange();

        //$('select[multiple="multiple"]').multiselect({
        $('select[multiple="multiple"]:not([data-not-set="true"])').multiselect({
            noneSelectedText: "--All--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();

        $('select[multiple="multiple"][data-not-set="true"]').multiselect({
            noneSelectedText: "--Not Set--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();

        //value droppable only when loaded in catalogs module
        $('#SeoItemInfo_Terminal').removeAttr('data-keep-value');

        $('#txtFilterCatalogs').on('keyup', function (e) {
            if ($(this).val().length > 0) {
                $('#fdsCatalogsManagement').find('div:not(.wiki-icon)').first().children('fieldset').each(function () {
                    //if ($(this).children('legend')[0].textContent.trim().toLowerCase().indexOf($('#txtFilterCatalogs').val()) < 0) {
                    if ($(this).children('legend').text().trim().toLowerCase().indexOf($('#txtFilterCatalogs').val().toLowerCase()) < 0) {
                        $(this).hide();
                    }
                    else {
                        $(this).show();
                    }
                });
            }
            else {
                $('#fdsCatalogsManagement').find('div:not(.wiki-icon)').first().children('fieldset').each(function () {
                    $(this).show();
                });
            }
        });

        $('#imgClearText').on('click', function () {
            $('#txtFilterCatalogs').val('');
            $('#txtFilterCatalogs').trigger('keyup');
        });
        $('#fdsDestinationsInfo').children('div').first().on('mouseover', function () {
            if (map == undefined) {
                initMap();
            }
        });

        $('#DestinationInfo_Latitude').numeric();

        $('#DestinationInfo_Latitude').on('keydown', function (e) {
            e.preventDefault();
        });

        $('#DestinationInfo_Longitude').on('keydown', function (e) {
            e.preventDefault();
        }).numeric();

        $('#SeoItemInfo_TerminalItem').on('change', function () {
            $('#SeoItemInfo_Terminal').val($(this).val());
        });

        $('#CommissionsInfo_CommissionPercentage').numeric({ negative: false });

        $('#CommissionsInfo_Terminal').on('change', function (e, params) {
            $('#CommissionsInfo_PointsOfSale').clearSelect();
            $('#CommissionsInfo_PointsOfSale').multiselect('refresh');
            $.getJSON('/Catalogs/GetDDLData', { itemType: 'pointOfSale', itemID: $(this).val() }, function (data) {
                $('#CommissionsInfo_PointsOfSale').fillSelect(data);
                if (params != undefined) {
                    $.each(params.pointsOfSale, function (index, item) {
                        $('#CommissionsInfo_PointsOfSale option[value="' + item + '"]').attr('selected', true);
                    });
                }
                $('#CommissionsInfo_PointsOfSale').multiselect('refresh');
            });
        });

        $('input:radio[name="CommissionsInfo_IsPermanent"]').on('change', function () {
            if ($('input:radio[name="CommissionsInfo_IsPermanent"]:checked').val() == "True") {
                $('#CommissionsInfo_ToDate').val('');
                $('#divPriceToDate').hide();
            }
            else {
                $('#divPriceToDate').show();
            }
        });

        $('#CommissionsInfo_IsBonus').on('change', function () {
            if ($('#CommissionsInfo_IsBonus').val() == "true") {
                $('.for-bonus').show();
            } else {
                $('.for-bonus').hide();
            }
        });

        $('#CompanyInfo_ZipCode').numeric({ negative: false });

        $('#ExchangeRatesInfo_PointsOfSale').multiselect({
            noneSelectedText: "--None--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();

        $('#ExchangeRatesInfo_ExchangeRate').numeric({ negative: false });

        $('input:radio[name="ExchangeRatesInfo_Permanent"]').on('change', function () {
            $('#ExchangeRatesInfo_F_Date').val('');
            if ($('input:radio[name="ExchangeRatesInfo_Permanent"]:checked').val() == 'True') {
                $('.rate-visibility-controlled').hide();
            }
            else {
                $('.rate-visibility-controlled').show();
            }
        });


        $('*[data-uses-date-picker="true"]').each(function (index, item) {
            $(this).datepicker({
                dateFormat: 'yy-mm-dd',
                changeMonth: true,
                changeYear: true,
                //minDate: 0,
                minDate: $(item).attr('data-start-date-picker') != undefined ? $(item).attr('data-start-date-picker') == 'null' ? null : parseInt($(item).attr('data-start-date-picker')) : 0,
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
                            } else if (toDate < fromDate) {
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
                        //    //$(this).prev().datepicker('option', 'maxDate', $(this).datepicker('getDate'));
                    }
                }
            });

        });

        $('#SearchExchangeRates_FromDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true
        });

        $('#btnDownloadOPCSList').on('click', function () {
            window.open('/Catalogs/GetOPCsList/' + UI.selectedTerminals, '_blank');
        });

        $('#btnDownloadHistory').on('click', function () {
            window.open('/Catalogs/GetOPCsHistory/' + UI.selectedTerminals, '_blank');
        });

        $('#OPCSInfo_EnlistDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true
        });

        $('#OPCTeamInfo_EnlistDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true
        });

        $('#OPCTeamInfo_TerminateDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true
        });

        $('#btnNewOPCSInfo').on('click', function () {
            $('#tblOPCTeams tbody').html('');
            $('#OPCSInfo_TeamInfoEditing').val('valid');
            $('#divEditingOPC').html('New OPC');
            $('#btnOPCAddTeam').slideDown('fast');
            $('#divTeamInfo').slideUp('fast');
        });

        $('#btnOPCAddTeam').on('click', function () {
            $('#btnOPCAddTeam').slideUp('fast');
            $('#divTeamInfo').clearForm();
            $('#OPCTeamInfo_PromotionTeam').prop('disabled', false);
            UI.resetValidation();
            $('#OPCSInfo_TeamInfoEditing').val('');
            $('#divTeamInfo').slideDown('fast');
        });

        $('.terminal-dependent-list').on('loaded', function () {
            if ($('#divTblExistingOPCS').html() != "") {
                $('#divTblExistingOPCS').html('');
            }
        });

        $('#btnSaveTeamInfo').on('click', function () {
            $('#OPCSInfo_TeamInfoEditing').val('valid');
            UI.resetValidation();
            addTeamToOPC();
        });

        $('#btnCancelTeamInfo').on('click', function () {
            $('#divTeamInfo').slideUp('fast');
            $('#OPCSInfo_TeamInfoEditing').val('valid');
            UI.resetValidation();
            $('#btnOPCAddTeam').slideDown('fast');
        });

        $('#btnNotifyChanges').on('click', function () {
            $.getJSON('/Catalogs/SendChangesReport', null, function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            });
        });

        $('#CouponFoliosInfo_ToFolio').on('keyup', function () {
            if ($('#CouponFolios_Info_FromFolio').val() != "") {
                var delivered = (parseInt($(this).val()) - parseInt($('#CouponFoliosInfo_FromFolio').val())) + 1;
                $('#CouponFoliosInfo_Delivered').val(delivered);
                $('#CouponFoliosInfo_Available').val(delivered);
            }
        });

        $('#ExchangeRatesInfo_Terminal').on('change', function (e, params) {
            $.getJSON('/Catalogs/GetDDLData', { itemType: 'providersPerTerminals', itemID: $(this).val() }, function (data) {
                $('#ExchangeRatesInfo_Provider').fillSelect(data);
                if (params != undefined) {
                    $('#ExchangeRatesInfo_Provider').val(params.providerID);
                }
            });
            $('#ExchangeRatesInfo_PointsOfSale').clearSelect();
            $('#ExchangeRatesInfo_PointsOfSale').multiselect('refresh');
            $.getJSON('/Catalogs/GetDDLData', { itemType: 'pointOfSale', itemID: $(this).val() }, function (data) {
                $('#ExchangeRatesInfo_PointsOfSale').fillSelect(data);
                if (params != undefined) {
                    $.each(params.pointsOfSale, function (index, item) {
                        $('#ExchangeRatesInfo_PointsOfSale option[value="' + item + '"]').attr('selected', true);
                    });
                }
                $('#ExchangeRatesInfo_PointsOfSale').multiselect('refresh');
            });
        });

        $('#fileToUpload').on('change', function () {
            if ($(this).val() != undefined) {
                $('#ProviderInfo_FileToUpload').val($(this).val().substr($(this).val().lastIndexOf('\\') + 1));
            }
            else {
                $('#ProviderInfo_FileToUpload').val('');
            }
        });

        $('#imgClearFileToUpload').on('click', function () {
            $('#fileToUpload').val(null);
            $('#fileToUpload').trigger('change');
        });

        $('#fileUploader').on('complete', function (first, id, fileName, data) {
            $('#fileUploader').fineUploader('reset');
            var duration = data['response'].Type < 0 ? data['response'].Type : null;
            if (data['success'] != false) {
                CATALOG.getFilesOfProvider(data['response'].ObjectID);
            }
            var exception = data['response'].Exception != null ? data['response'].Exception.Message : "";
            UI.messageBox(data['response'].Type, data['response'].Message + '<br />' + exception, duration, data['response'].InnerException);
        });

        $('#ProviderInfo_ContractCurrency').on('change', function () {
            $('#divUSDAvanceProvider').show();
            $('#divMXNAvanceProvider').show();
            if ($(this).val() == '1') {
                $('#ProviderInfo_MXNAvanceProvider').val('');
                $('#divMXNAvanceProvider').hide();
            }
            if ($(this).val() == '2') {
                $('#ProviderInfo_AvanceProvider').val('');
                $('#divUSDAvanceProvider').hide();
            }
        });

        $('fieldset legend').on('click', function () {
            if (document.location.href.indexOf("settings") >= 0) {
                var regex = /(<([^>]+)>)/ig;
                UI.Notifications.workingOn("Settings > Catalogs > " + $(this).html().replace(regex, '').replace('?', ''));
            }
        });

        $('input:radio[name="BudgetInfo_BudgetExt"]').on('change', function () {
            if ($('input:radio[name="BudgetInfo_BudgetExt"]:checked').val().toLowerCase() == 'true') {
                $('input:radio[name="BudgetInfo_PerClient"]')[0].checked = true;
                $('input:radio[name="BudgetInfo_PerClient"]').trigger('change');
            }
            //else {
            //    $('input:radio[name="BudgetInfo_PerClient"]')[1].checked = true;
            //    $('input:radio[name="BudgetInfo_PerClient"]').trigger('change');
            //}
        });

        $('input:radio[name="BudgetInfo_Permanent"]').on('change', function () {
            $('#BudgetInfo_ToDate').val('');
            if ($('input:radio[name="BudgetInfo_Permanent"]:checked').val().toLowerCase() == 'true') {
                $('#divBudgetToDate').hide();
            }
            else {
                $('#divBudgetToDate').show();
            }
        });

        $('input:radio[name="BudgetInfo_PerWeek"]').on('change', function () {
            if ($('input:radio[name="BudgetInfo_PerWeek"]:checked').val().toLowerCase() == 'true') {
                $('input:radio[name="BudgetInfo_PerClient"]')[1].checked = true;
                $('#divBudgetResetDayOfWeek').show();
            }
            else {
                $('input:radio[name="BudgetInfo_PerClient"]')[0].checked = true;
                $('#divBudgetResetDayOfWeek').hide();
            }
        });

        $('input:radio[name="BudgetInfo_PerClient"]').on('change', function () {
            if ($('input:radio[name="BudgetInfo_PerClient"]:checked').val().toLowerCase() == 'true') {
                $('input:radio[name="BudgetInfo_PerWeek"]')[1].checked = true;
            }
            else {
                $('input:radio[name="BudgetInfo_PerWeek"]')[0].checked = true;
            }
            $('input:radio[name="BudgetInfo_PerWeek"]').trigger('change');
        });

        $('#ExchangeRatesInfo_ExchangeRateType').on('change', function () {
            if ($(this).val() >= 4 && $(this).val() <= 6) {
                $('#ExchangeRatesInfo_I_Date').datepicker('option', 'minDate', null);
            }
            else {
                $('#ExchangeRatesInfo_I_Date').datepicker('option', 'minDate', 0);
            }
        });

        $('input:radio[name="SearchExchangeRates_OptionProvider"]').on('change', function () {
            if ($(this).val().toLowerCase() == 'true') {
                $('#divProvider').slideDown('fast');
            }
            else {
                $('#divProvider').slideUp('fast');
            }
        });

        //partiesinfo
        //$('#SearchDate_FromDate').datetimepicker({
        //    dateFormat: 'yy-mm-dd',//01/01/0001
        //   // timeFormat: 'hh:mm TT',//12:00 am
        //   //stepMinute: 5,
        //    changeMonth: true,
        //    changeYear: true,
        //    required: false
        //}).val("");

        $('#SearchDate_FromDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true
        }).val("");

        $('.SearchDate_ToDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true
        }).val("");

        $('#SearchFromDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onClose: function (dateText, inst) {
                if ($('#SearchToDate').val() != '') {
                    if (dateText != '') {
                        var fromDate = $('#SearchFromDate').datepicker('getDate');
                        var toDate = $('#SearchToDate').datepicker('getDate');
                        if (fromDate > toDate)
                            $('#SearchToDate').datepicker('setDate', fromDate);
                    }
                    else {
                        $('#SearchToDate').val(dateText);
                    }
                }
                else {
                    $('#SearchToDate').val(dateText);
                }
            },
            onSelect: function (selectedDate) {
                if ($('#SearchToDate').val() != '') {
                    $('#SearchToDate').datepicker('setDate', $('#SearchToDate').datepicker('getDate'));
                }
                $('#SearchToDate').datepicker('option', 'minDate', $('#SearchFromDate').datepicker('getDate'));
            }
        });

        $('#PartiesInfo_FromDate').datetimepicker({
            dateFormat: 'yy-mm-dd',//01/01/0001
            timeFormat: 'hh:mm TT',// 12:00/am
            stepMinute: 5,
            changeMonth: true,
            changeYear: true
        }).val("");

        $('fdsSalesRooms').on('click', function () {
            //  $('#fdsSalesRoomsManagement').slideDown('fast');
            UI.expandFieldset('fdsSalesRooms');
            UI.scrollTo('divTblExistingSalesParties', null);
        });

        $('#btnNewPartiesInfo').on('click', function () {
            $('#PartiesInfo_Room option[value=' + "0" + ']').attr('selected', true);
            $('#PartiesInfo_PlaceID option[value="' + "0" + '"]').attr('selected', true);
            $('#PartiesInfo_ProgramName option[value="' + "0" + '"]').attr('selected', true);
            $('#PartiesInfo_Terminal option[value="' + "0" + '"').attr('selected', true);
            $('#frmSalesRoomPartiesInfo').clearForm();
            $('#fdsSalesRoomsManagementDuplicate').slideUp('fast');
            UI.expandFieldset('fdsSalesRoomsManagement');
            UI.scrollTo('fdsSalesRoomsManagement', null);
        });
        $('.TexBoxDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true
        }).val("");

        $('#btnDuplicatePartiesInfo').off('click').on('click', function () {
            $('#fdsSalesRoomsManagementDuplicate').slideDown('fast');
            UI.scrollTo('fdsSalesRoomsManagementDuplicate', null);
        });
        $('#btnSaveDuplicateSalesRoomsParties').off('click').on('click', function () {
            saveSalesRoomsPartiesDuplicateSuccess();
            $('#DuplicateFromDate').val('');
            $('#DuplicateToDate').val('');
            $('#PartiesInfo_Terminal option[value=""0""]').attr('selected', true);
        });

        //Promo Info, promosearch
        $('.PromoDatepicker').datepicker(
            {
                dateFormat: 'yy-mm-dd',
                changeMonth: true,
                changeYear: true
            });

        $('#PromoInfo_Type').on('change', function () {
            if ($('#PromoInfo_Type option:selected').text() == 'Discount') {
                $('#divPromoInfo_Percentage').slideDown('fast');
                $('#divPromoInfo_Amount').slideUp('fast');
                $('#PromoInfo_Amount').attr('disabled', disabled);
                $('#PromoInfo_Amount').val('');
            }
            else {
                $('#divPromoInfo_Amount').slideDown('fast');
                $('#divPromoInfo_Percentage').slideUp('fast');
                $('#PromoInfo_Percentage').attr('disabled', disabled)
                $('#PromoInfo_Percentage').val('');
            }

            if ($('#PromoInfo_Type option:selected').length > 1 && $('#PromoInfo_Type:contains(Discount) option:selected').text() == 'Discount') {
                UI.messageBox(0, 'YOU CAN SELECT A PROMO WITH DISCOUNT TYPE AT THE SAME TIME', 7);
            }
        });

        $('.add-button').bind('click').on('click', function (e) {
            var flag = false; //bandera si la fila existe

            if ($('tblPromoDescription tbody').find('tr.editing-row').lengh > 0) {
                flag = true;
            }

            var valid = flag == false ? validateInsertionAttemptPromos('tblPromoDescription', 'PromoInfo_DescriptionTag', 'PromoInfo_DescriptionCulture') : '1'
            switch (valid) {
                case "-1": {
                    UI.messageBox(-1, "Duplicated Value", null, null);//duplicado
                    break;
                }
                case "0": {
                    UI.messageBox(0, "Option Not Valid", null, null);//invalido
                    break;
                }
                case "1": {//crear fila y agregar a a tabla
                    var builder = '<tr id="' + $('#PromoInfo_DescriptionID').val()
                        + '|' + $('#PromoInfo_DescriptionTag').val()
                        + '|' + $('#PromoInfo_DescriptionCulture option:selected').text() + '">'

                        + '<td>' + $('#PromoInfo_DescriptionTag').val() + '</td>'
                        + '<td>' + $('#PromoInfo_DescriptionCulture option:selected').text() + '</td>'
                        + '<td>' + $('#PromoInfo_DescriptionTitle').val() + '</td>'
                        + '<td>' + $('#PromoInfo_DescriptionDescription').val() + '</td>'
                        + '<td>' + $('#PromoInfo_DescriptionInstructions').val() + '</td>'
                        + '</tr>';
                    if (!flag) {
                        $('#tblPromoDescription').append(builder);
                    }
                    else {
                        $('#tblPromoDescription tbody tr.editing-row').before(builder);
                        $('#tblPromoDescription tbody tr.editing-row').remove();
                    }
                    $('#PromoInfo_DescriptionId').val('');
                    $('#PromoInfo_DescriptionTag').val('');
                    $('#PromoInfo_DescriptionCulture').find('option[value=0]').attr('selected', true);
                    $('#PromoInfo_DescriptionTitle').val('');
                    break;
                }
            }
        });

        $('#btnNewCommissionsInfo').on('click', function () {
            $('#CommissionsInfo_PriceType').multiselect('uncheckAll');
            $('#CommissionsInfo_PriceType').multiselect({ multiple: true });
        });

        $('#CommissionsInfo_FromDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onSelect: function (selectedDate) {
                var fromDate = $('#CommissionsInfo_FromDate').datepicker('getDate');
                var toDate = $('#CommissionsInfo_ToDate').datepicker('getDate');
                if ($('#CommissionsInfo_ToDate').val() != '') {
                    if (fromDate > toDate) {
                        $('#CommissionsInfo_ToDate').datepicker('setDate', fromDate);
                    }
                }
                else if ($('input:radio[name="CommissionsInfo_IsPermanent"]')[1].checked) {
                    $('#CommissionsInfo_ToDate').datepicker('setDate', fromDate);
                }
            }
        });

        $('#CommissionsInfo_ToDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onSelect: function (selectedDate) {
                var fromDate = $('#CommissionsInfo_FromDate').datepicker('getDate');
                var toDate = $('#CommissionsInfo_ToDate').datepicker('getDate');
                if ($('#CommissionsInfo_FromDate').val() != '') {
                    if (fromDate > toDate) {
                        $('#CommissionsInfo_FromDate').datepicker('setDate', toDate);
                    }
                }
                else {
                    $('#CommissionsInfo_FromDate').datepicker('setDate', toDate);
                }
            }
        });

        //Banks
        $('#btnNewBankInfo').on('click', function () {

            $('.SearchDate_ToDate').datepicker({
                dateFormat: 'yy-mm-dd',
                changeMonth: true,
                changeYear: true
            }).val("");

            $('#NewBankName').val('');
            $('#NewBankID').val('');
            $('#NewCveSat').val('');
            $('#NewTerminalID option[value="' + 0 + '"]').attr('selected', true);
            $('#divTblExistingBanks tbody').html('');
            UI.expandFieldset('fdsBankManagement');
            UI.scrollTo('fdsBankManagement', null);
        });
        //Boats
        $('#btnNewSearchInfo').on('click', function () {
            $('#BoatID').val('');
            $('#Boat').val('');
            $('#Qouta').val('');
            $('#Shortname').val('');
            $('#ProvidersID option[value="' + 0 + '"]').attr('selected', true);
            $('#TerminalID option[value="' + 0 + '"]').attr('selected', true);
            $('#divTblExistingBoats tbody').html('');
            UI.expandFieldset('fdsBoatManagement');
            UI.scrollTo('fdsBoatManagement', null);
        });
        //SalesChannels
        $('#btnNewSearchChannelInfo').on('click', function () {
            $('#SalesChannnelID').val('');
            $('#SavedByUser').val('');
            $('#DateSaved').val('');
            $('#SalesChannel').val('');
            $('#TerminalID option[value="' + 0 + '"]').attr('selected', true);
            $('#divTblExistingSalesChannels tbody').html('');
            UI.expandFieldset('fdsSalesChannelsManagement');
            UI.scrollTo('fdsSalesChannelsManagement', null);
        });
        //Bracelets
        $('#btnNewBraceletInfo').on('click', function () {
            $('#BraceletID').val('');
            $('#Bracelet').val('');
            $('#TerminalID option[value="' + 0 + '"]').attr('selected', true);
            $('#tblBraceletsSearchResult tbody').html('');
            UI.expandFieldset('fdsBraceletManagement');
            UI.scrollTo('fdsBraceletManagement', null);
        });
        //Reminders
        $('#btnNewReminderInfo').on('click', function () {
            $('#ReminderID').val('');
            $('#FromDate').val('');
            $('#ToDate').val('');
            $('#Message').val('');
            $('#ServiceID option[value="' + 0 + '"]').attr('selected', true);
            $('#TerminalID option[value="' + 0 + '"]').attr('selected', true);
            $('#tblRemindersSearchResult tbody').html('');
            UI.expandFieldset('fdsReminderManagement');
            UI.scrollTo('fdsReminderManagement', null);
        });


        //$('#MarketCodeSearch_Users').multiselect({
        //    noneSelectedText: "--Not Assigned--",
        //    minWidth: "auto", selectedList: 1
        //});

        //$('#MarketCodeInfo_AssignTo').multiselect({
        //    noneSelectedText: "--None--",
        //    minWidth: "auto", selectedList: 1
        //});
        //}).multiselectfilter();

        $('#btnAssignMarketCodes').on('click', function () {
            $('#MarketCodeInfo_Assign').val(true);
            $('#frmMarketCodeInfo').submit();
        });

        $('#btnSaveMarketCode').on('click', function () {
            $('#MarketCodeInfo_Assign').val(false);
            $('#frmMarketCodeInfo').submit();
        });
    }

    var refreshActiveSearchResults = function () {
        var _table = $(document).find('input.search:visible').parents('.fds-active:first').find('table.search-results');
        if ($(_table).find('tbody tr').length != 0) {
            $(document).find('input.search:visible').trigger('click');
        }
    }

    //accounting accounts
    var accountingAccountsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchAccountingAccountsResults', tableColumns - 1);
        CATALOG.oAccountingAccountsTable = $('#tblSearchAccountingAccountsResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makeAccountingAccountsSelectable(); });
        //});
        //$('#tblSearchAccountingAccountsResults_length').unbind('change').on('change', function () {
        //    CATALOG.makeAccountingAccountsSelectable();
        //});
    }

    var makeAccountingAccountsSelectable = function () {
        //$('#tblSearchAccountingAccountsResults tbody tr').not('theader').unbind('click').on('click', function (e) {
        CATALOG.oAccountingAccountsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oAccountingAccountsTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var accountingAccountID = $(this).attr('id').substr(18);
                    $.ajax({
                        url: '/AccountingAccounts/GetAccountingAccount',
                        cache: false,
                        type: 'POST',
                        data: { AccountingAccountInfo_AccountingAccountID: accountingAccountID },
                        success: function (data) {
                            $('#AccountingAccountInfo_AccountingAccountID').val(data.AccountingAccountInfo_AccountingAccountID);
                            $('#AccountingAccountInfo_Account').val(data.AccountingAccountInfo_Account);
                            $('#AccountingAccountInfo_AccountName').val(data.AccountingAccountInfo_AccountName);
                            $('#AccountingAccountInfo_Company option[value="' + data.AccountingAccountInfo_Company + '"]').attr('selected', true);
                            $('#AccountingAccountInfo_PriceType').multiselect('uncheckAll');
                            $('#AccountingAccountInfo_PriceType option[value="' + data.AccountingAccountInfo_PriceType[0] + '"]').attr('selected', true);
                            $('#AccountingAccountInfo_PriceType').multiselect('refresh');
                            $('#AccountingAccountInfo_ArticleMXN').val(data.AccountingAccountInfo_ArticleMXN);
                            $('#AccountingAccountInfo_ArticleUSD').val(data.AccountingAccountInfo_ArticleUSD);
                            if (data.AccountingAccountInfo_AccountType) {
                                $('input:radio[name="AccountingAccountInfo_AccountType"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="AccountingAccountInfo_AccountType"]')[1].checked = true;
                            }
                            UI.expandFieldset('fdsAccountingAccountsInfo');
                            UI.scrollTo('fdsAccountingAccountsInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/AccountingAccounts/DeleteAccountingAccount', 'tr', $(e.target), CATALOG.oAccountingAccountsTable);
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oAccountingAccountsTable);
            }
        });
    }

    var saveAccountingAccountSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Accounting Account Saved') {
                CATALOG.oAccountingAccountsTable = $.fn.DataTable.fnIsDataTable('tblSearchAccountingAccountsResults') ? $('#tblSearchAccountingAccountsResults') : $('#tblSearchAccountingAccountsResults').dataTable();
                var oSettings = CATALOG.oAccountingAccountsTable.fnSettings();
                $.each(data.ItemID.accountingAccountID, function (index, item) {
                    var iAdded = CATALOG.oAccountingAccountsTable.fnAddData([
                        $('#AccountingAccountInfo_Account').val(),
                        $('#AccountingAccountInfo_AccountName').val(),
                        ($('input:radio[name="AccountingAccountInfo_AccountType"]:checked').val().toLowerCase() == 'true' ? 'Income' : 'Outcome'),
                        $('#AccountingAccountInfo_Company option:selected').text(),
                        //$('#AccountingAccountInfo_PriceType option:selected').text()
                        item.Value,
                        '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                    ]);
                    var aRow = oSettings.aoData[iAdded[0]].nTr;
                    aRow.setAttribute('id', 'accountingAccount_' + item.Key);
                    CATALOG.oAccountingAccountsTable.fnDisplayRow(aRow);
                });
                $('#frmAccountingAccountInfo').clearForm();
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                CATALOG.makeAccountingAccountsSelectable();
            }
            else {
                var array = CATALOG.oAccountingAccountsTable.fnGetNodes();
                var nTr = CATALOG.oAccountingAccountsTable.$('tr.selected-row');
                var position = CATALOG.oAccountingAccountsTable.fnGetPosition(nTr[0]);
                CATALOG.oAccountingAccountsTable.fnDisplayRow(array[position]);
                CATALOG.oAccountingAccountsTable.fnUpdate([
                    $('#AccountingAccountInfo_Account').val(),
                    $('#AccountingAccountInfo_AccountName').val(),
                    ($('input:radio[name="AccountingAccountInfo_AccountType"]:checked').val().toLowerCase() == 'true' ? 'Income' : 'Outcome'),
                    $('#AccountingAccountInfo_Company option:selected').text(),
                    $('#AccountingAccountInfo_PriceType option:selected').text(),
                    '<img src="/Content/themes/base/images/cross.png" class="delete-item right" />'
                ], $('#accountingAccount_' + data.ItemID)[0], undefined, false);
                //$('#accountingAccount_' + data.ItemID).children('td:nth-child(1)').text($('#AccountingAccountInfo_Account').val());
                //$('#accountingAccount_' + data.ItemID).children('td:nth-child(2)')[0].firstChild.textContent = $('#AccountingAccountInfo_Company option:selected').text();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //budgets
    var budgetsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchBudgetsResults', tableColumns - 1);
        CATALOG.oBudgetsTable = $('#tblSearchBudgetsResults').dataTable();
    }

    var makeBudgetsSelectable = function () {
        CATALOG.oBudgetsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oBudgetsTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var budgetID = $(this).attr('id').split('_')[1];
                    $.ajax({
                        url: '/Catalogs/GetBudget',
                        cache: false,
                        type: 'POST',
                        data: { BudgetInfo_BudgetID: budgetID },
                        success: function (data) {
                            $('#BudgetInfo_BudgetID').val(data.BudgetInfo_BudgetID);
                            $('#BudgetInfo_LeadCode').val(data.BudgetInfo_LeadCode);
                            $('#BudgetInfo_LeadQualification').val(data.BudgetInfo_LeadQualification);
                            $('#BudgetInfo_Budget').val(data.BudgetInfo_Budget);
                            //$('#BudgetInfo_BudgetExt').val(data.BudgetInfo_BudgetExt);
                            if (data.BudgetInfo_BudgetExt) {
                                $('input:radio[name="BudgetInfo_BudgetExt"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="BudgetInfo_BudgetExt"]')[1].checked = true;
                            }
                            $('input:radio[name="BudgetInfo_BudgetExt"]').trigger('change');
                            $('#BudgetInfo_Currency option[value="' + data.BudgetInfo_Currency + '"]').attr('selected', true);
                            $('#BudgetInfo_FromDate').val(data.BudgetInfo_FromDate);

                            if (data.BudgetInfo_Permanent) {
                                $('input:radio[name="BudgetInfo_Permanent"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="BudgetInfo_Permanent"]')[1].checked = true;
                            }
                            $('input:radio[name="BudgetInfo_Permanent"]').trigger('change');
                            $('#BudgetInfo_ToDate').val(data.BudgetInfo_ToDate);
                            if (data.BudgetInfo_PerClient) {
                                $('input:radio[name="BudgetInfo_PerClient"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="BudgetInfo_PerClient"]')[1].checked = true;
                            }
                            $('input:radio[name="BudgetInfo_PerClient"]').trigger('change');
                            if (data.BudgetInfo_PerWeek) {
                                $('input:radio[name="BudgetInfo_PerWeek"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="BudgetInfo_PerWeek"]')[1].checked = true;
                            }
                            $('input:radio[name="BudgetInfo_PerWeek"]').trigger('change');
                            $('#BudgetInfo_ResetDayOfWeek option[value="' + data.BudgetInfo_ResetDayOfWeek + '"]').attr('selected', true);

                            $.each(data.BudgetInfo_PromotionTeam, function (index, item) {
                                $('#BudgetInfo_PromotionTeam option[value="' + item + '"]').attr('selected', true);
                            });
                            $('#BudgetInfo_PromotionTeam').multiselect('refresh');
                            UI.expandFieldset('fdsBudgetsInfo');
                            UI.scrollTo('fdsBudgetsInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteBudget', 'tr', $(e.target), CATALOG.oBudgetsTable);
            }
        });
    }

    var saveBudgetSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseMessage : null;
        if (data.ResponseType > 0) {
            var vigency = $('#BudgetInfo_FromDate').val() + ' / ' + ($('input:radio[name="BudgetInfo_Permanent"]:checked').val().toLowerCase() == 'true' ? 'Permanent' : $('#BudgetInfo_ToDate').val());
            var teams = '';
            $('#BudgetInfo_PromotionTeam option:selected').each(function () {
                teams += (teams != '' ? ', ' : '') + $(this).text();
            });
            if (data.ResponseMessage == 'Budget Saved') {
                CATALOG.oBudgetsTable = $.fn.DataTable.fnIsDataTable('tblSearchBudgetsResults') ? $('tblSearchBudgetsResults') : $('#tblSearchBudgetsResults').dataTable();
                var oSettings = CATALOG.oBudgetsTable.fnSettings();
                var iAdded = CATALOG.oBudgetsTable.fnAddData([
                    $('#BudgetInfo_LeadCode').val() + ' - ' + $('#BudgetInfo_LeadQualification').val(),
                    $('#BudgetInfo_Budget').val() + ' ' + ($('#BudgetInfo_Currency option:selected').val() == '1' ? 'USD' : $('#BudgetInfo_Currency option:selected').val() == '2' ? 'MXN' : 'CAD'),
                    vigency,
                    ($('input:radio[name="BudgetInfo_BudgetExt"]:checked').val().toLowerCase() == 'true' ? 'True' : 'False'),
                    teams
                    + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'budget_' + data.ItemID);
                CATALOG.oBudgetsTable.fnDisplayRow(aRow);
                $('#frmBudgetInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeBudgetsSelectable();
            }
            else {
                var array = CATALOG.oBudgetsTable.fnGetNodes();
                var nTr = CATALOG.oBudgetsTable.$('tr.selected-row');
                var position = CATALOG.oBudgetsTable.fnGetPosition(nTr[0]);
                CATALOG.oBudgetsTable.fnDisplayRow(array[position]);
                CATALOG.oBudgetsTable.fnUpdate([
                    $('#BudgetInfo_LeadCode').val() + ' - ' + $('#BudgetInfo_LeadQualification').val(),
                    $('#BudgetInfo_Budget').val() + ' ' + ($('#BudgetInfo_Currency option:selected').val() == '1' ? 'USD' : $('#BudgetInfo_Currency option:selected').val() == '2' ? 'MXN' : 'CAD'),
                    vigency,
                    ($('input:radio[name="BudgetInfo_BudgetExt"]:checked').val().toLowerCase() == 'true' ? 'True' : 'False'),
                    teams
                    + (data.ItemID.budgetInUse == true ? '' : '<img src="/Content/themes/base/images/cross.png" class="right delete-item">')
                ], $('#budget_' + data.ItemID.budgetID)[0], undefined, false);
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //commissions
    var commissionsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchCommissionsResults', tableColumns - 1);
        CATALOG.oCommissionsTable = $('#tblSearchCommissionsResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makeCommissionsSelectable(); });
        //});
        //$('#tblSearchCommissionsResults_length').unbind('change').on('change', function () {
        //    CATALOG.makeCommissionsSelectable();
        //});
    }

    var makeCommissionsSelectable = function () {
        CATALOG.oCommissionsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oCommissionsTable, $(this));

                    var commissionID = $(this).attr('id').substr(11);
                    $.ajax({
                        url: '/Catalogs/GetCommission',
                        cache: false,
                        type: 'POST',
                        data: { CommissionsInfo_CommissionID: commissionID },
                        success: function (data) {
                            $('#CommissionsInfo_CommissionID').val(data.CommissionsInfo_CommissionID);
                            $('#CommissionsInfo_IsBonus').val(data.CommissionsInfo_IsBonus.toString()).trigger('change');
                            $('#CommissionsInfo_CommissionPercentage').val(data.CommissionsInfo_CommissionPercentage);
                            $('#CommissionsInfo_CommissionAmount').val(data.CommissionsInfo_CommissionAmount);
                            $('#CommissionsInfo_CommissionCurrency option[value="' + data.CommissionsInfo_CommissionCurrency + '"]').attr('selected', true);
                            $('#CommissionsInfo_MinVolume').val(data.CommissionsInfo_MinVolume);
                            $('#CommissionsInfo_MaxVolume').val(data.CommissionsInfo_MaxVolume);
                            $('#CommissionsInfo_VolumeCurrencyCode').val(data.CommissionsInfo_VolumeCurrencyCode);
                            $('#CommissionsInfo_MinProfit').val(data.CommissionsInfo_MinProfit);
                            $('#CommissionsInfo_MaxProfit').val(data.CommissionsInfo_MaxProfit);
                            $('#CommissionsInfo_PriceType').multiselect('uncheckAll');
                            $('#CommissionsInfo_PriceType').multiselect({ multiple: false });
                            $('#CommissionsInfo_PriceType option[value="' + data.CommissionsInfo_PriceType[0] + '"]').attr('selected', true);
                            //$('#CommissionsInfo_PriceType').multiselect('refresh');
                            $('#CommissionsInfo_JobPosition').val(data.CommissionsInfo_JobPosition);
                            $('#CommissionsInfo_SysWorkGroup').val(data.CommissionsInfo_SysWorkGroup);
                            $('#CommissionsInfo_Terminal').val(data.CommissionsInfo_Terminal);
                            $('#CommissionsInfo_Terminal').trigger('change', { pointsOfSale: data.CommissionsInfo_PointsOfSale });

                            $('#CommissionsInfo_FromDate').val(data.CommissionsInfo_FromDate);

                            if (data.CommissionsInfo_IsPermanent == true) {
                                $('input:radio[name="CommissionsInfo_IsPermanent"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="CommissionsInfo_IsPermanent"]')[1].checked = true;
                            }

                            $('input:radio[name="CommissionsInfo_IsPermanent"]').trigger('change');

                            $('#CommissionsInfo_ToDate').val(data.CommissionsInfo_ToDate);

                            //$('#CommissionsInfo_PointsOfSale option').each(function () {
                            //    $(this).removeAttr('selected');
                            //});
                            //$.each(data.CommissionsInfo_PointsOfSale.split(','), function (index, item) {
                            //    $('#CommissionsInfo_PointsOfSale option[value="' + item + '"]').attr('selected', true);
                            //});
                            if (data.CommissionsInfo_Override == true) {
                                $('input:radio[name="CommissionsInfo_Override"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="CommissionsInfo_Override"]')[1].checked = true;
                            }

                            if (data.CommissionsInfo_ApplyOnSales == true) {
                                $('input:radio[name="CommissionsInfo_ApplyOnSales"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="CommissionsInfo_ApplyOnSales"]')[1].checked = true;
                            }

                            if (data.CommissionsInfo_ApplyOnDealPrice == true) {
                                $('input:radio[name="CommissionsInfo_ApplyOnDealPrice"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="CommissionsInfo_ApplyOnDealPrice"]')[1].checked = true;
                            }

                            if (data.CommissionsInfo_ApplyOnAdultSales == true) {
                                $('input:radio[name="CommissionsInfo_ApplyOnAdultSales"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="CommissionsInfo_ApplyOnAdultSales"]')[1].checked = true;
                            }

                            if (data.CommissionsInfo_OnlyIfCharged == true) {
                                $('input:radio[name="CommissionsInfo_OnlyIfCharged"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="CommissionsInfo_OnlyIfCharged"]')[1].checked = true;
                            }

                            UI.expandFieldset('fdsCommissionsInfo');
                            UI.scrollTo('fdsCommissionsInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteCommission', 'tr', $(e.target), CATALOG.oCommissionsTable);
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oCommissionsTable);
            }
        });
    }

    var saveCommissionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Commission Saved') {
                CATALOG.oCommissionsTable = $.fn.DataTable.fnIsDataTable('tblSearchCommissionsResults') ? $('#tblSearchCommissionsResults') : $('#tblSearchCommissionsResults').dataTable();
                var oSettings = CATALOG.oCommissionsTable.fnSettings();
                $.each(data.ItemID.commissionID, function (index, item) {
                    var iAdded = CATALOG.oCommissionsTable.fnAddData([
                        $('#CommissionsInfo_Terminal option:selected').text(),
                        $('#CommissionsInfo_JobPosition option:selected').text(),
                        $('#CommissionsInfo_IsBonus option:selected').text(),
                        data.ItemID.pointsOfSale,
                        UI.addDecimalPart($('#CommissionsInfo_CommissionPercentage').val()) + '%',
                        //$('#CommissionsInfo_PriceType option:selected').text(),
                        item.Value,
                        $('input:radio[name="CommissionsInfo_Override"]:checked').val(),
                        ($('#CommissionsInfo_MinVolume').val() != '' ? '$ ' + UI.addDecimalPart($('#CommissionsInfo_MinVolume').val()) : '$ 0.00') + ' ' + $('#CommissionsInfo_VolumeCurrencyCode').val(),
                        ($('#CommissionsInfo_MaxVolume').val() != '' ? '$ ' + UI.addDecimalPart($('#CommissionsInfo_MaxVolume').val()) + ' ' + $('#CommissionsInfo_VolumeCurrencyCode').val() : $('#CommissionsInfo_MaxVolume').val()),
                        $('#CommissionsInfo_MinProfit').val() != '' ? UI.addDecimalPart($('#CommissionsInfo_MinProfit').val()) + '%' : '0.00%',
                        $('#CommissionsInfo_MaxProfit').val() != '' ? UI.addDecimalPart($('#CommissionsInfo_MaxProfit').val()) + '%' : $('#CommissionsInfo_MaxProfit').val(),
                        ($('#CommissionsInfo_FromDate').val() != '' ? $('#CommissionsInfo_FromDate').val() : "Today") + " / " + ($('input:radio[name="CommissionsInfo_IsPermanent"]:checked').val() == "True" ? "Permanent" : $('#CommissionsInfo_ToDate').val()),
                        '<img src="/Content/themes/base/images/trash.png" class="right delete-item">'
                    ]);
                    var aRow = oSettings.aoData[iAdded[0]].nTr;
                    aRow.setAttribute('id', 'commission_' + item.Key);
                    CATALOG.oCommissionsTable.fnDisplayRow(aRow);
                });
                $('#frmCommissionInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeCommissionsSelectable();
            }
            else {
                var array = CATALOG.oCommissionsTable.fnGetNodes();
                var nTr = CATALOG.oCommissionsTable.$('tr.selected-row');
                var position = CATALOG.oCommissionsTable.fnGetPosition(nTr[0]);
                CATALOG.oCommissionsTable.fnDisplayRow(array[position]);
                CATALOG.oCommissionsTable.fnUpdate([
                    $('#CommissionsInfo_Terminal option:selected').text(),
                    $('#CommissionsInfo_JobPosition option:selected').text(),
                    $('#CommissionsInfo_IsBonus option:selected').text(),
                    data.ItemID.pointsOfSale,
                    UI.addDecimalPart($('#CommissionsInfo_CommissionPercentage').val()) + '%',
                    $('#CommissionsInfo_PriceType option:selected').text(),
                    $('input:radio[name="CommissionsInfo_Override"]:checked').val(),
                    ($('#CommissionsInfo_MinVolume').val() != '' ? '$ ' + UI.addDecimalPart($('#CommissionsInfo_MinVolume').val()) : '$ 0.00') + ' ' + $('#CommissionsInfo_VolumeCurrencyCode').val(),
                    $('#CommissionsInfo_MaxVolume').val() != '' ? '$ ' + UI.addDecimalPart($('#CommissionsInfo_MaxVolume').val()) + ' ' + $('#CommissionsInfo_VolumeCurrencyCode').val() : $('#CommissionsInfo_MaxVolume').val(),
                    $('#CommissionsInfo_MinProfit').val() != '' ? UI.addDecimalPart($('#CommissionsInfo_MinProfit').val()) + '%' : '0.00%',
                    $('#CommissionsInfo_MaxProfit').val() != '' ? UI.addDecimalPart($('#CommissionsInfo_MaxProfit').val()) + '%' : $('#CommissionsInfo_MaxProfit').val(),
                    ($('#CommissionsInfo_FromDate').val() != '' ? $('#CommissionsInfo_FromDate').val() : "Today") + " / " + ($('input:radio[name="CommissionsInfo_IsPermanent"]:checked').val() == "True" ? "Permanent" : $('#CommissionsInfo_ToDate').val()),
                    '<img src="/Content/themes/base/images/trash.png" class="right delete-item">'
                ], $('#commission_' + data.ItemID.commissionID)[0], undefined, false);
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //marketCodes
    var marketCodesResultsTable = function (data) {
        //var tableColumns = $(data).find('tbody tr').first().find('td');
        //UI.searchResultsTable('tblMarketCodes', tableColumns - 1);
        //CATALOG.oMarketCodesTable = $('#tblMarketCodes').dataTable();

        var tableColumns = $(data).find('tbody tr').first().find('td');
        CATALOG.oMarketCodesTable = $('#tblMarketCodes').dataTable({
            "bFilter": false,
            "bProcessing": true,
            "bAutoWidth": false,
            "aoRowCallback": [UI.tablesHoverEffect()],
            "aoColumns": [
                {
                    'bVisible': true,
                    'bSortable': false,
                    'sClass': 'align-center'
                },
                {
                    'bVisible': true,
                    'bSortable': true
                },
                {
                    'bVisible': true,
                    'bSortable': true
                }
                ,
                {
                    'bVisible': true,
                    'bSortable': true
                }
                //{
                //    'bVisible': true,
                //    'bSortable': true
                //}
                //{
                //    'bVisible': true,
                //    'bSortable': true
                //},
                //{
                //    'bVisible': true,
                //    'bSortable': true
                //}
            ],
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            }
        });

        if (CATALOG.oMarketCodesTable.$('.chk-son').length == CATALOG.oMarketCodesTable.$('.chk-son:checked').length) {
            $('#tblMarketCodes thead .chk-parent')[0].checked = true;
        }

        initCheckboxes();
        CATALOG.bindToCheckBoxes();
    }

    var bindToCheckBoxes = function () {
        $('#tblMarketCodes thead .chk-parent').unbind('click').on('click', function (e) {
            CATALOG.oMarketCodesTable.$('tr').each(function () {
                var $this = $(this);
                if ($(e.target).is(':checked')) {
                    $(this).children('td:nth-child(1)').children('.chk-son')[0].checked = true;
                    marketCodesArray.push($(this).children('td:nth-child(1)').children('.chk-son').data('id'));
                }
                else {
                    $(this).children('td:nth-child(1)').children('.chk-son')[0].checked = false;
                    marketCodesArray = marketCodesArray.filter(function (evt) { return evt !== $this.children('td:nth-child(1)').children('.chk-son').data('id') });
                }
            });
            $('#MarketCodeInfo_MarketCodes').val(marketCodesArray.toString());
            //$('#MarketCodeInfo_MarketCodes').val(marketCodesArray);
        });

        CATALOG.oMarketCodesTable.$('.chk-son').unbind('click').on('click', function (e) {
            if ($(e.target).is(':checked')) {
                marketCodesArray.push($(e.target).data('id'));
            }
            else {
                marketCodesArray = marketCodesArray.filter(function (evt) { return evt !== $(e.target).data('id') });
            }
            $('#MarketCodeInfo_MarketCodes').val(marketCodesArray.toString());
            //$('#MarketCodeInfo_MarketCodes').val(marketCodesArray);
        });
    }

    function initCheckboxes() {
        marketCodesArray = new Array();
        CATALOG.oMarketCodesTable.$('.chk-son:checked').each(function () {
            marketCodesArray.push($(this).data('id'));
        });
        $('#MarketCodeInfo_MarketCodes').val(marketCodesArray.toString());
        //$('#MarketCodeInfo_MarketCodes').val(marketCodesArray);
    }

    var makeTblMarketCodesSelectable = function () {
        CATALOG.oMarketCodesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is(':input')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oMarketCodesTable, $(this));
                    $.ajax({
                        url: '/Catalogs/GetMarketCode',
                        cache: false,
                        type: 'POST',
                        data: { marketCodeID: $(this).attr('id') },
                        success: function (data) {
                            $('#MarketCodeInfo_MarketCodeID').val(data.MarketCodeInfo_MarketCodeID);
                            $('#MarketCodeInfo_MarketCode').val(data.MarketCodeInfo_MarketCode);
                            $('#MarketCodeInfo_Place option[value="' + data.MarketCodeInfo_Place + '"]').attr('selected', true);
                            //if (data.MarketCodeInfo_Users != null) {
                            //    $.each(data.MarketCodeInfo_Users, function (e, item) {
                            //        $('#MarketCodeInfo_Users option[value="' + item + '"]').attr('selected', true);
                            //    });
                            //}
                            //$('#MarketCodeInfo_Users').multiselect('refresh');
                            $('#MarketCodeInfo_Terminal option[value="' + data.MarketCodeInfo_Terminal + '"]').attr('selected', true);
                            $('#MarketCodeInfo_LeadSource option[value="' + data.MarketCodeInfo_LeadSource + '"]').attr('selected', true);
                            //UI.expandFieldset('fdsMarketCodesInfo');
                            //UI.scrollTo('fdsMarketCodesInfo', null);
                        }
                    });
                }
            }
        });
    }

    var saveMarketCodeSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            var users = '';
            //$('#MarketCodeInfo_Users option:selected').each(function (index, item) {
            //    users += (users == '' ? '' : ',') + $(item).text();
            //});
            if (data.ResponseMessage == 'Market Code Saved') {
                CATALOG.oMarketCodesTable = $.fn.DataTable.fnIsDataTable('tblMarketCodes') ? $('#tblMarketCodes') : $('#tblMarketCodes').dataTable();
                var oSettings = CATALOG.oMarketCodesTable.fnSettings();
                var iAdded = CATALOG.oMarketCodesTable.fnAddData([
                    '<input type="checkbox" data-id="' + data.ItemID + '" class="chk-son" />',
                    $('#MarketCodeInfo_MarketCode').val(),
                    //users,
                    $('#MarketCodeInfo_Place option:selected').text(),
                    $('#MarketCodeInfo_LeadSource option:selected').text()
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID);
                CATALOG.oMarketCodesTable.fnDisplayRow(aRow);
                $('#frmMarketCodeInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeTblMarketCodesSelectable();
            }
            else if (data.ResponseMessage == 'Market Code Updated') {
                CATALOG.oMarketCodesTable.$('#' + data.ItemID).children('td:nth-child(2)').text($('#MarketCodeInfo_MarketCode').val());
                //CATALOG.oMarketCodesTable.$('#' + data.ItemID).children('td:nth-child(3)').text(users);
                CATALOG.oMarketCodesTable.$('#' + data.ItemID).children('td:nth-child(3)').text($('#MarketCodeInfo_Place option:selected').text());
                CATALOG.oMarketCodesTable.$('#' + data.ItemID).children('td:nth-child(4)').text($('#MarketCodeInfo_LeadSource option:selected').text());
            }
            else if (data.ResponseMessage == 'Market Codes Succesfully Assigned') {
                users = '';
                users = $('#MarketCodeInfo_AssignTo option:selected').text();
                //$('#MarketCodeInfo_AssignTo option:selected').each(function (index, item) {
                //    users += (users == '' ? '' : ',') + $(item).text();
                //});
                CATALOG.oMarketCodesTable.$('tr').each(function (index, item) {
                    if ($(this).find('.chk-son:checked').length > 0) {
                        $(this).children('td:nth-child(3)').text(users);
                    }
                });
            }
            CATALOG.bindToCheckBoxes();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //companies
    var companiesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchCompaniesResults', tableColumns - 1);
        CATALOG.oCompaniesTable = $('#tblSearchCompaniesResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makeCompaniesSelectable(); });
        //});
        //$('#tblSearchCompaniesResults_length').unbind('change').on('change', function () {
        //    CATALOG.makeCompaniesSelectable();
        //});
    }

    var makeCompaniesSelectable = function () {
        CATALOG.oCompaniesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oCompaniesTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var companyID = $(this).attr('id').substr(8);
                    $.ajax({
                        url: '/Catalogs/GetCompany',
                        cache: false,
                        type: 'POST',
                        data: { CompanyInfo_CompanyID: companyID },
                        success: function (data) {
                            $('#CompanyInfo_CompanyID').val(data.CompanyInfo_CompanyID);
                            $('#CompanyInfo_Company').val(data.CompanyInfo_Company);
                            $('#CompanyInfo_ShortName').val(data.CompanyInfo_ShortName);
                            $('#CompanyInfo_Address').val(data.CompanyInfo_Address);
                            $('#CompanyInfo_City').val(data.CompanyInfo_city);
                            $('#CompanyInfo_State').val(data.CompanyInfo_State);
                            $('#CompanyInfo_Country').val(data.CompanyInfo_Country);
                            $('#CompanyInfo_ZipCode').val(data.CompanyInfo_ZipCode);
                            $('#CompanyInfo_RFC').val(data.CompanyInfo_RFC);
                            $('#CompanyTypeID').val(data.CompanyTypeID);
                            $.each(data.CompanyInfo_TerminalID, function (index, item) {
                                $('#CompanyInfo_TerminalID option[value="' + item + '"]').attr('selected', true);
                            });
                            $('#CompanyInfo_TerminalID').multiselect('refresh');
                            UI.expandFieldset('fdsCompanyInfo');
                            UI.scrollTo('fdsCompanyInfo', null);
                            //commented because this property doesn't come on the response and it hits an error
                            //$.each(data.BudgetInfo_PromotionTeam, function (index, item) {
                            //    $('#BudgetInfo_PromotionTeam option[value="' + item + '"]').attr('selected', true);
                            //});
                            //$('#BudgetInfo_PromotionTeam').multiselect('refresh');
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteCompany', 'tr', $(e.target), CATALOG.oCompaniesTable, UI.updateDependentLists, { url: '/Catalogs/GetDDLData', itemType: 'company' });
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oCompaniesTable);
            }
        });
    }

    var saveCompanySuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Company Saved') {
                CATALOG.oCompaniesTable = $.fn.DataTable.fnIsDataTable('tblSearchCompaniesResults') ? $('#tblSearchCompaniesResults') : $('#tblSearchCompaniesResults').dataTable();
                var oSettings = CATALOG.oCompaniesTable.fnSettings();
                var iAdded = CATALOG.oCompaniesTable.fnAddData([
                    $('#CompanyInfo_Company').val(),
                    $('#CompanyInfo_ShortName').val(),
                    $('#CompanyInfo_Country option:selected').text(),
                    $('#CompanyInfo_ZipCode').val(),
                    $('#CompanyInfo_RFC').val() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">',
                    $('#CompanyTypeID option:selected').text(),
                    $('#CompanyInfo_TerminalID option:selected').text()

                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'company_' + data.ItemID);
                CATALOG.oCompaniesTable.fnDisplayRow(aRow);
                $('#frmCompanyInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeCompaniesSelectable();
            }
            else {
                $('#company_' + data.ItemID).children('td:nth-child(1)').text($('#CompanyInfo_Company').val());
                $('#company_' + data.ItemID).children('td:nth-child(2)').text($('#CompanyInfo_ShortName').val());
                $('#company_' + data.ItemID).children('td:nth-child(3)').text($('#CompanyInfo_Country option:selected').text());
                $('#company_' + data.ItemID).children('td:nth-child(4)').text($('#CompanyInfo_ZipCode').val());
                $('#company_' + data.ItemID).children('td:nth-child(5)')[0].firstChild.textContent = $('#CompanyInfo_RFC').val();
                $('#company_' + data.ItemID).children('td:nth-child(6)').text($('#CompanyTypeID option:selected').text());
                $('#company_' + data.ItemID).children('td:nth-child(6)').text($('#CompanyInfo_TerminalID option:selected').text());

            }
            UI.updateDependentLists('/Catalogs/GetDDLData', 'company');
            //CATALOG.updateDependentLists('company');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //couponFolios
    var couponFoliosResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchCouponFoliosResults', tableColumns - 1);
        CATALOG.oCouponFoliosTable = $('#tblSearchCouponFoliosResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makeCouponFoliosSelectable(); });
        //});
        //$('#tblSearchCouponFoliosResults_length').unbind('change').on('change', function () {
        //    CATALOG.makeCouponFoliosSelectable();
        //});
    }

    var makeCouponFoliosSelectable = function () {
        CATALOG.oCouponFoliosTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(document).find('.selected-row').each(function () {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    });
                    $(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    var couponFolioID = $(this).attr('id').substr(12);
                    $.ajax({
                        url: '/Catalogs/GetCouponFolio',
                        cache: false,
                        type: 'POST',
                        data: { CouponFoliosInfo_CouponFolioID: couponFolioID },
                        success: function (data) {
                            $('#CouponFoliosInfo_CouponFolioID').val(data.CouponFoliosInfo_CouponFolioID);
                            $('#CouponFoliosInfo_FromFolio').val(data.CouponFoliosInfo_FromFolio);
                            $('#CouponFoliosInfo_ToFolio').val(data.CouponFoliosInfo_ToFolio);
                            $('#CouponFoliosInfo_Serial').val(data.CouponFoliosInfo_Serial);
                            $('#CouponFoliosInfo_Delivered').val(data.CouponFoliosInfo_Delivered);
                            $('#CouponFoliosInfo_Generated').val(data.CouponFoliosInfo_Generated);
                            $('#CouponFoliosInfo_Available').val(data.CouponFoliosInfo_Available);
                            $('#CouponFoliosInfo_Company').val(data.CouponFoliosInfo_Company);
                            $('#CouponFoliosInfo_PointOfSale').val(data.CouponFoliosInfo_PointOfSale);
                            UI.expandFieldset('fdsCouponFolioInfo');
                            UI.scrollTo('fdsCouponFolioInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteCouponFolio', 'tr', $(e.target), CATALOG.oCouponFoliosTable);
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oCouponFoliosTable);
            }
        });
    }

    var saveCouponFolioSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Coupon Folio Saved') {
                CATALOG.oCouponFoliosTable = $.fn.DataTable.fnIsDataTable('tblSearchCouponFoliosResults') ? $('#tblSearchCouponFoliosResults') : $('#tblSearchCouponFoliosResults').dataTable();
                var oSettings = CATALOG.oCouponFoliosTable.fnSettings();
                var iAdded = CATALOG.oCouponFoliosTable.fnAddData([
                    $('#CouponFoliosInfo_FromFolio').val(),
                    $('#CouponFoliosInfo_ToFolio').val(),
                    $('#CouponFoliosInfo_Serial').val(),
                    $('#CouponFoliosInfo_Delivered').val(),
                    $('#CouponFoliosInfo_Generated').val(),
                    $('#CouponFoliosInfo_Available').val(),
                    ($('#CouponFoliosInfo_Company option:selected').val() != "0" ? $('#CouponFoliosInfo_Company option:selected').text() : "") + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'couponFolio_' + data.ItemID);
                CATALOG.oCouponFoliosTable.fnDisplayRow(aRow);
                $('#frmCouponFolioInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeCouponFoliosSelectable();
            }
            else {
                $('#couponFolio_' + data.ItemID).children('td:nth-child(1)').text($('#CouponFoliosInfo_FromFolio').val());
                $('#couponFolio_' + data.ItemID).children('td:nth-child(2)').text($('#CouponFoliosInfo_ToFolio').val());
                $('#couponFolio_' + data.ItemID).children('td:nth-child(3)').text($('#CouponFoliosInfo_Serial').text());
                $('#couponFolio_' + data.ItemID).children('td:nth-child(4)').text($('#CouponFoliosInfo_Delivered').val());
                $('#couponFolio_' + data.ItemID).children('td:nth-child(5)').text($('#CouponFoliosInfo_Generated').val());
                $('#couponFolio_' + data.ItemID).children('td:nth-child(6)').text($('#CouponFoliosInfo_Available').val());
                var _node = $('#couponFolio_' + data.ItemID).children('td:nth-child(7)')[0].firstChild;
                if ($(_node).is('img')) {
                    $('#couponFolio_' + data.ItemID).children('td:nth-child(7)').prepend($('#CouponFoliosInfo_Company option:selected').val() != "0" ? $('#CouponFoliosInfo_Company option:selected').text() : "");
                }
                else {
                    $('#couponFolio_' + data.ItemID).children('td:nth-child(7)')[0].firstChild.textContent = $('#CouponFoliosInfo_Company option:selected').val() != "0" ? $('#CouponFoliosInfo_Company option:selected').text() : "";
                }
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //options
    var optionsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblOptionsResults', tableColumns - 1);
        CATALOG.oOptionsTable = $('#tblOptionsResults').dataTable();
        UI.applyFormat('currency', 'tblOptionsResults');
    }

    var makeOptionsSelectable = function () {
        CATALOG.oOptionsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('expired') && !$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oOptionsTable, $(this));
                    var _id = $(this).attr('id').split('_')[1];
                    $.ajax({
                        url: '/catalogs/GetOption',
                        cache: false,
                        type: 'POST',
                        data: { OptionsInfo_OptionID: _id },
                        success: function (data) {
                            $('#OptionsInfo_OptionID').val(data.OptionsInfo_OptionID);
                            $('#OptionsInfo_OptionType option[value="' + data.OptionsInfo_OptionType + '"]').attr('selected', true);
                            $('#OptionsInfo_OptionName').val(data.OptionsInfo_OptionName);
                            $('#OptionsInfo_OptionDescription').val(data.OptionsInfo_OptionDescription);
                            $('#OptionsInfo_Place').multiselect('uncheckAll');
                            $.each(data.OptionsInfo_Place, function (index, item) {
                                $('#OptionsInfo_Place option[value="' + item + '"]').attr('selected', true);
                            });
                            $('#OptionsInfo_Place').multiselect('refresh');
                            $('#OptionsInfo_GoldCardPrice').val(data.OptionsInfo_GoldCardPrice);
                            $('#OptionsInfo_ResortCredit').val(data.OptionsInfo_ResortCredit);
                            UI.expandFieldset('fdsOptionsInfo');
                            UI.scrollTo('fdsOptionsInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteOption', 'tr', $(e.target), CATALOG.oOptionsTable);
            }
        });
    }

    var saveOptionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            var _places = '';
            $('#OptionsInfo_Place option:selected').each(function () {
                _places += (_places != '' ? ', ' : '') + $(this).text();
            });
            if (data.ResponseMessage.indexOf('Updated') == -1) {

                CATALOG.oOptionsTable = $.fn.DataTable.fnIsDataTable('tblOptionsResults') ? $('#tblOptionsResults') : $('#tblOptionsResults').dataTable();
                var oSettings = CATALOG.oOptionsTable.fnSettings();
                var iAdded = CATALOG.oOptionsTable.fnAddData([
                    $('#OptionsInfo_OptionName').val(),
                    $('#OptionsInfo_OptionType option:selected').text(),
                    _places,
                    $('#OptionsInfo_GoldCardPrice').val(),
                    $('#OptionsInfo_ResortCredit').val(),
                    '<img src="/Content/themes/base/images/trash.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'option_' + data.ItemID);
                aRow.cells[3].setAttribute('class', 'format-currency');
                aRow.cells[4].setAttribute('class', 'format-currency');
                CATALOG.oOptionsTable.fnDisplayRow(aRow);
                $('#frmOptionInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeOptionsSelectable();
            }
            else {
                var array = CATALOG.oOptionsTable.fnGetNodes();
                var nTr = CATALOG.oOptionsTable.$('tr.selected-row');
                var position = CATALOG.oOptionsTable.fnGetPosition(nTr[0]);
                CATALOG.oOptionsTable.fnDisplayRow(array[position]);
                CATALOG.oOptionsTable.fnUpdate([
                    $('#OptionsInfo_OptionName').val(),
                    $('#OptionsInfo_OptionType option:selected').text(),
                    _places,
                    $('#OptionsInfo_GoldCardPrice').val(),
                    $('#OptionsInfo_ResortCredit').val(),
                    '<img src="/Content/themes/base/images/trash.png" class="right delete-item">'
                ], $('#option_' + data.ItemID)[0], undefined, false);
            }
            UI.applyFormat('currency', 'tblOptionsResults');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //destinations
    var destinationsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchDestinationsResults', tableColumns - 1);
        CATALOG.oDestinationsTable = $('#tblSearchDestinationsResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makeDestinationsSelectable(); });
        //});
        //$('#tblSearchDestinationsResults_length').unbind('change').on('change', function () {
        //    CATALOG.makeDestinationsSelectable();
        //});
    }

    var makeDestinationsSelectable = function () {
        CATALOG.oDestinationsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oDestinationsTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row primary');
                    //$(this).addClass('selected-row primary');
                    $('#fdsDestinationDescriptions').show();
                    $('#fdsDestinationSeoItems').show();
                    var destinationID = $(this).attr('id').substr(12);
                    $('#SeoItemInfo_ItemType').val('Destination');
                    $('#SeoItemInfo_ItemID').val(destinationID);
                    $('#DestinationDescription_DestinationID').val(destinationID);
                    SEO.urlText = $(this).children('td:nth-child(1)')[0].textContent.trim().split(',')[0];
                    $.ajax({
                        url: '/Catalogs/GetDestination',
                        cache: false,
                        type: 'POST',
                        data: { DestinationInfo_DestinationID: destinationID },
                        success: function (data) {
                            $('#DestinationInfo_DestinationID').val(data.DestinationInfo_DestinationID);
                            $('#DestinationInfo_Destination').val(data.DestinationInfo_Destination);
                            $('#DestinationInfo_Latitude').val(data.DestinationInfo_Latitude);
                            $('#DestinationInfo_Longitude').val(data.DestinationInfo_Longitude);
                            CATALOG.updateUlDestinationDescriptions(destinationID);
                            SEO.updateTblSeoItems();
                            //SEO.updateSeoItemRelatedLists();
                            $('#fdsPictures').trigger('loadPicturesTree');
                            UI.expandFieldset('fdsDestinationsInfo');
                            UI.scrollTo('fdsDestinationsInfo', null);
                            initMap();
                            moveMapMarker();
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteDestination', 'tr', $(e.target), CATALOG.oDestinationsTable, UI.updateDependentLists, { url: '/Catalogs/GetDDLData', itemType: 'destination' });
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oDestinationsTable);
            }
        });
    }

    var saveDestinationSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Destination Saved') {
                CATALOG.oDestinationsTable = $.fn.DataTable.fnIsDataTable('tblSearchDestinationsResults') ? $('#tblSearchDestinationsResults') : $('#tblSearchDestinationsResults').dataTable();
                var oSettings = CATALOG.oDestinationsTable.fnSettings();
                var iAdded = CATALOG.oDestinationsTable.fnAddData([
                    $('#DestinationInfo_Destination').val(),
                    $('#DestinationInfo_Latitude').val(),
                    $('#DestinationInfo_Longitude').val() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'destination_' + data.ItemID);
                CATALOG.oDestinationsTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                CATALOG.makeDestinationsSelectable();
                $('#destination_' + data.ItemID).click();
                UI.expandFieldset('fdsDestinationDescriptions');
                UI.scrollTo('fdsDestinationDescriptions', null);
            }
            else {
                $('#destination_' + data.ItemID).children('td:nth-child(1)').text($('#DestinationInfo_Destination').val());
                $('#destination_' + data.ItemID).children('td:nth-child(2)').text($('#DestinationInfo_Latitude').val());
                $('#destination_' + data.ItemID).children('td:nth-child(3)')[0].firstChild.textContent = $('#DestinationInfo_Longitude').val();
            }
            UI.updateDependentLists('/Catalogs/GetDDLData', 'destination');
            //CATALOG.updateDependentLists('destination');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var updateUlDestinationDescriptions = function (id) {
        $.ajax({
            url: '/Catalogs/GetDestinationDescriptions',
            cache: false,
            type: 'POST',
            data: { DestinationInfo_DestinationID: id },
            success: function (data) {
                $('#ulDestinationDescriptions').empty();
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="destinationDescription_' + item.DestinationDescription_DestinationDescriptionID + '">'
                            + '<div style="display:inline-block;width:300px">' + $('#DestinationDescription_Culture option[value="' + item.DestinationDescription_Culture + '"]').text() + '</div>'
                            + '<div style="display:inline">' + $('#DestinationDescription_Terminal option[value="' + item.DestinationDescription_Terminal + '"]').text() + '</div>'
                            + '<img src="/Content/themes/base/images/cross.png" class="right delete-item delete-li" /></li>';
                    });
                    $('#ulDestinationDescriptions').append(builder);
                    UI.ulsHoverEffect('ulDestinationDescriptions');
                    CATALOG.ulDestinationDescriptionsSelection();
                }
                else {
                    builder += '<li id="liNoDescriptions">No Descriptions Added</li>';
                    $('#ulDestinationDescriptions').append(builder);
                }
            }
        });
    }

    var ulDestinationDescriptionsSelection = function () {
        $('#ulDestinationDescriptions li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(this).find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $('#DestinationDescription_DestinationDescriptionID').val($(this).attr('id').substr($(this).attr('id').indexOf('_') + 1));
                    $.ajax({
                        url: '/Catalogs/GetDestinationDescription',
                        type: 'POST',
                        cache: false,
                        data: { DestinationDescription_DestinationDescriptionID: $('#DestinationDescription_DestinationDescriptionID').val() },
                        success: function (data) {
                            $('#DestinationDescription_Description').val(data.DestinationDescription_Description);
                            $('#DestinationDescription_Culture option[value="' + data.DestinationDescription_Culture + '"]').attr('selected', true);
                            $('#DestinationDescription_Terminal option[value="' + data.DestinationDescription_Terminal + '"]').attr('selected', true);
                            $('#DestinationDescription_VideoURL').val(data.DestinationDescription_VideoURL);
                            $('#DestinationDescription_VideoTitle').val(data.DestinationDescription_VideoTitle);
                            UI.expandFieldset('fdsDestinationDescriptionsInfo');
                            UI.scrollTo('fdsDestinationDescriptionsInfo', null);
                        }
                    });
                }
            }
            else {
                CATALOG.deleteItemFunction('li');
            }
        });
    }

    var saveDestinationDescriptionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Destination Description Saved') {
                var builder = '<li id="destinationDescription_' + data.ItemID + '">'
                    + $('#DestinationDescription_Culture option:selected').text()
                    + '<img src="/Content/themes/base/images/cross.png" class="right delete-item delete-li" /></li>';
                $('#liNoDescriptions').remove();
                $('#ulDestinationDescriptions').append(builder);
                UI.ulsHoverEffect('ulDestinationDescriptions');
                CATALOG.ulDestinationDescriptionsSelection();
                $('#btnNewDestinationDescriptionsInfo').click();
            }
            else {
                $('#destinationDescription_' + data.ItemID)[0].firstChild.textContent = $('#DestinationDescription_Culture option:selected').text();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    function initMap() {
        latLng = new google.maps.LatLng(20.679767, -105.254180);//Pto. Vallarta
        var myOptions = {
            zoom: 8,
            center: latLng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("gMap"), myOptions);
        moveMapMarker(latLng);
        new google.maps.event.addListener(map, 'click', function (e) {
            updateLatLng(e.latLng);
            moveMapMarker();
        });
    }

    function updateLatLng(latLng) {
        $('#DestinationInfo_Latitude').val(latLng.lat());
        $('#DestinationInfo_Longitude').val(latLng.lng());
    }

    function moveMapMarker(latLng) {
        if (!latLng) {
            var latLng = new google.maps.LatLng($('#DestinationInfo_Latitude').val(), $('#DestinationInfo_Longitude').val());
            markersArray[markersArray.length - 1].setMap(null);
        }
        var marker = new google.maps.Marker({
            position: latLng,
            map: map,
            draggable: true,
            title: 'Drag marker to desired location'
        });
        markersArray.push(marker);
        new google.maps.event.addListener(marker, 'dragend', function (e) {
            updateLatLng(marker.getPosition());
        });
        map.panTo(latLng);
    }

    //exchange rates

    var exchangeRatesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchExchangeRatesResults', tableColumns - 1);
        CATALOG.oExchangeRatesTable = $('#tblSearchExchangeRatesResults').dataTable();
        //CATALOG.oExchangeRatesTable.fnSort([[1,'desc']]);
        CATALOG.afterRenderFunctions();
    }

    var makeExchangeRatesSelectable = function () {
        CATALOG.oExchangeRatesTable = $.fn.DataTable.fnIsDataTable('tblSearchExchangeRatesResults') ? $('#tblSearchExchangeRatesResults') : $('#tblSearchExchangeRatesResults').dataTable();
        CATALOG.oExchangeRatesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oExchangeRatesTable, $(this));
                    //CATALOG.oExchangeRatesTable.$('tr.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var exchangeRateID = CATALOG.oExchangeRatesTable.$('tr.selected-row').attr('id').substr(13);
                    $.ajax({
                        url: '/Catalogs/GetExchangeRate',
                        cache: false,
                        type: 'POST',
                        data: { ExchangeRatesInfo_ExchangeRateID: exchangeRateID },
                        success: function (data) {
                            $('#ExchangeRatesInfo_ExchangeRateID').val(data.ExchangeRatesInfo_ExchangeRateID);
                            $('#ExchangeRatesInfo_ExchangeRate').val(data.ExchangeRatesInfo_ExchangeRate);
                            $('#ExchangeRatesInfo_I_Date').val(data.ExchangeRatesInfo_I_Date);
                            if (data.ExchangeRatesInfo_Permanent) {
                                $('input:radio[name="ExchangeRatesInfo_Permanent"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="ExchangeRatesInfo_Permanent"]')[1].checked = true;
                            }
                            $('input:radio[name="ExchangeRatesInfo_Permanent"]').trigger('change');
                            $('#ExchangeRatesInfo_F_Date').val(data.ExchangeRatesInfo_F_Date);
                            $('#ExchangeRatesInfo_FromCurrency option[value="' + data.ExchangeRatesInfo_FromCurrency + '"]').attr('selected', true);
                            $('#ExchangeRatesInfo_ToCurrency option[value="' + data.ExchangeRatesInfo_ToCurrency + '"]').attr('selected', true);
                            $('#ExchangeRatesInfo_Terminal').val(data.ExchangeRatesInfo_Terminal);
                            $('#ExchangeRatesInfo_Terminal').trigger('change', { providerID: data.ExchangeRatesInfo_Provider, pointsOfSale: data.ExchangeRatesInfo_PointsOfSale });
                            $('#ExchangeRatesInfo_ExchangeRateType option[value = "' + data.ExchangeRatesInfo_ExchangeRateType + '"]').attr('selected', true);
                            //
                            $('#ExchangeRatesInfo_ExchangeRateDateSaved').val(data.ExchangeRatesInfo_ExchangeRateDateSaved);
                            $('#ExchangeRatesInfo_ExchangeRateUser').val(data.ExchangeRatesInfo_ExchangeRateUser);
                            $('#ExchangeRatesInfo_ExchangeRateLastModifyDate').val(data.ExchangeRatesInfo_ExchangeRateLastModifyDate);
                            $('#ExchangeRatesInfo_ExchangeRateLastModifyUser').val(data.ExchangeRatesInfo_ExchangeRateLastModifyUser);
                            UI.expandFieldset('fdsExchangeRatesInfo');
                            UI.scrollTo('fdsExchangeRatesInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteExchangeRate', 'tr', $(e.target), CATALOG.oExchangeRatesTable);
            }
        });
    }

    var saveExchangeRateSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Exchange Rate Saved') {
                //commented because whole table need to be redraw in order to hide expired rates
                //CATALOG.oExchangeRatesTable = $.fn.DataTable.fnIsDataTable('tblSearchExchangeRatesResults') ? $('#tblSearchExchangeRatesResults') : $('#tblSearchExchangeRatesResults').dataTable();
                //var oSettings = CATALOG.oExchangeRatesTable.fnSettings();
                //var iAdded = CATALOG.oExchangeRatesTable.fnAddData([
                //    UI.addDecimalPart($('#ExchangeRatesInfo_ExchangeRate').val()),
                //    data.ItemID.fromDate,
                //    data.ItemID.toDate,
                //    $('#ExchangeRatesInfo_FromCurrency option:selected').text().split('-')[1].trim() + ' - ' + $('#ExchangeRatesInfo_ToCurrency option:selected').text().trim(),
                //    $('#ExchangeRatesInfo_ExchangeRateType option:selected').text(),
                //    ($('#ExchangeRatesInfo_Provider option:selected').val() != '0' ? $('#ExchangeRatesInfo_Provider option:selected').text() : $('#ExchangeRatesInfo_Terminal option:selected').val() != '0' ? $('#ExchangeRatesInfo_Terminal option:selected').text() : '')
                //]);
                //var aRow = oSettings.aoData[iAdded[0]].nTr;
                //aRow.setAttribute('id', 'exchangeRate_' + data.ItemID.exchangeRateID);
                //CATALOG.oExchangeRatesTable.fnDisplayRow(aRow);
                $('#SearchExchangeRates_FromDate').datepicker('setDate', new Date());
                $('#SearchExchangeRates_Terminal option[value="' + $('#ExchangeRatesInfo_Terminal option:selected').val() + '"]').attr('selected', true);
                $('#frmExchangeRatesSearch').submit();
                $('#frmExchangeRateInfo').clearForm();
                //UI.tablesHoverEffect();
                //UI.tablesStripedEffect();
                CATALOG.makeExchangeRatesSelectable();
            }
            else {
                var array = CATALOG.oExchangeRatesTable.fnGetNodes();
                var nTr = CATALOG.oExchangeRatesTable.$('tr.selected-row');
                var position = CATALOG.oExchangeRatesTable.fnGetPosition(nTr[0]);
                CATALOG.oExchangeRatesTable.fnDisplayRow(array[position]);
                CATALOG.oExchangeRatesTable.fnUpdate([
                    UI.addDecimalPart($('#ExchangeRatesInfo_ExchangeRate').val()),
                    data.ItemID.fromDate,
                    data.ItemID.toDate,
                    $('#ExchangeRatesInfo_FromCurrency option:selected').text().split('-')[1].trim() + ' - ' + $('#ExchangeRatesInfo_ToCurrency option:selected').text().trim(),
                    $('#ExchangeRatesInfo_ExchangeRateType option:selected').text(),
                    data.ItemID.dateSaved,
                    data.ItemID.saveByUser,
                    data.ItemID.lastModifiedDate,
                    data.ItemID.lastModifiedUser,
                    ($('#ExchangeRatesInfo_Provider option:selected').val() != '0' ? $('#ExchangeRatesInfo_Provider option:selected').text() : $('#ExchangeRatesInfo_Terminal option:selected').val() != '0' ? $('#ExchangeRatesInfo_Terminal option:selected').text() : '')
                ], $('#exchangeRate_' + data.ItemID.exchangeRateID)[0], undefined, false);
            }
            //CATALOG.oExchangeRatesTable.fnSort([[1, 'desc']]);
            CATALOG.afterRenderFunctions();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var _saveExchangeRateSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            //var date = $('input:radio[name="ExchangeRatesInfo_Permanent"]:checked').val() != 'True' ? $('#ExchangeRatesInfo_F_Date').val() : 'Permanent';
            if (data.ResponseMessage == 'Exchange Rate Saved') {
                //CATALOG.oExchangeRatesTable = $.fn.DataTable.fnIsDataTable('tblSearchExchangeRatesResults') ? $('#tblSearchExchangeRatesResults') : $('#tblSearchExchangeRatesResults').dataTable();
                //var oSettings = CATALOG.oExchangeRatesTable.fnSettings();
                //var iAdded = CATALOG.oExchangeRatesTable.fnAddData([
                //    UI.addDecimalPart($('#ExchangeRatesInfo_ExchangeRate').val()),
                //    $('#ExchangeRatesInfo_I_Date').val(),
                //    date,
                //    $('#ExchangeRatesInfo_FromCurrency option:selected').text().split('-')[1].trim() + ' - ' + $('#ExchangeRatesInfo_ToCurrency option:selected').text().trim(),
                //    $('#ExchangeRatesInfo_ExchangeRateType option:selected').text(),
                //    ($('#ExchangeRatesInfo_Provider option:selected').val() != '0' ? $('#ExchangeRatesInfo_Provider option:selected').text() : $('#ExchangeRatesInfo_Terminal option:selected').val() != '0' ? $('#ExchangeRatesInfo_Terminal option:selected').text() : '')
                //]);
                //var aRow = oSettings.aoData[iAdded[0]].nTr;
                //aRow.setAttribute('id', 'exchangeRate_' + data.ItemID.exchangeRateID);
                //CATALOG.oExchangeRatesTable.fnDisplayRow(aRow);
                $('#frmExchangeRatesSearch').submit();
                $('#frmExchangeRateInfo').clearForm();
                //UI.tablesHoverEffect();
                //UI.tablesStripedEffect();
                CATALOG.makeExchangeRatesSelectable();
            }
            else {
                var array = CATALOG.oExchangeRatesTable.fnGetNodes();
                var nTr = CATALOG.oExchangeRatesTable.$('tr.selected-row');
                var position = CATALOG.oExchangeRatesTable.fnGetPosition(nTr[0]);
                CATALOG.oExchangeRatesTable.fnDisplayRow(array[position]);
                CATALOG.oExchangeRatesTable.fnUpdate([
                    UI.addDecimalPart($('#ExchangeRatesInfo_ExchangeRate').val()),
                    data.ItemID.fromDate,
                    data.ItemID.toDate,
                    $('#ExchangeRatesInfo_FromCurrency option:selected').text().split('-')[1].trim() + ' - ' + $('#ExchangeRatesInfo_ToCurrency option:selected').text().trim(),
                    $('#ExchangeRatesInfo_ExchangeRateType option:selected').text(),
                    data.ItemID.dateSaved,
                    data.ItemID.saveByUser,
                    data.ItemID.lastModifiedDate,
                    data.ItemID.lastModifiedUser,
                    ($('#ExchangeRatesInfo_Provider option:selected').val() != '0' ? $('#ExchangeRatesInfo_Provider option:selected').text() : $('#ExchangeRatesInfo_Terminal option:selected').val() != '0' ? $('#ExchangeRatesInfo_Terminal option:selected').text() : '')
                ], $('#exchangeRate_' + data.ItemID.exchangeRateID)[0], undefined, false);
            }
            //CATALOG.oExchangeRatesTable.fnSort([[1, 'desc']]);
            CATALOG.afterRenderFunctions();
        }
        if (data.ResponseType == 0) {
            $('#ExchangeRatesInfo_IsClonation').val(true);
            //var _isUpdate = ($('#ExchangeRatesInfo_ExchangeRateID').val() != 0 && $('#ExchangeRatesInfo_ExchangeRateID').val() != '' ? true : false);
            UI.twoActionBox(data.ResponseMessage + '<br />Do you want to create a new rate with same data?', cloneExchangeRate, [data.ItemID.exchangeRateID], 'Confirm', function () { $('#ExchangeRatesInfo_IsClonation').val(''); $('#ExchangeRatesInfo_CloneRate').val(''); $('#ExchangeRatesInfo_RateToBeCloned').val(''); UI.messageBoxExit(); }, [], 'Cancel');
        }
        else {
            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
        }
    }

    function cloneExchangeRate(rateToBeCloned) {
        //$('#ExchangeRatesInfo_CloneRate').val(cloneRate);
        $('#ExchangeRatesInfo_RateToBeCloned').val(rateToBeCloned);
        $('#frmExchangeRateInfo').submit();
    }

    //function _cloneExchangeRate(exchangeRateID, exchangeRate, isUpdate) {
    //    $.ajax({
    //        url: '/Catalogs/CloneExchangeRate',
    //        cache: false,
    //        type: 'POST',
    //        data: { exchangeRateID: exchangeRateID, exchangeRate: exchangeRate, isUpdate: isUpdate },
    //        success: function (data) {
    //            var duration = data.ResponseType < 0 ? data.ResponseType : null;
    //            if (data.ResponseMessage == 'Exchange Rate Saved') {
    //                var date = $('input:radio[name="ExchangeRatesInfo_Permanent"]:checked').val() != 'True' ? $('#ExchangeRatesInfo_F_Date').val() : 'Permanent';
    //                CATALOG.oExchangeRatesTable = $.fn.DataTable.fnIsDataTable('tblSearchExchangeRatesResults') ? $('#tblSearchExchangeRatesResults') : $('#tblSearchExchangeRatesResults').dataTable();
    //                var oSettings = CATALOG.oExchangeRatesTable.fnSettings();
    //                var iAdded = CATALOG.oExchangeRatesTable.fnAddData([
    //                    UI.addDecimalPart($('#ExchangeRatesInfo_ExchangeRate').val()),
    //                $('#ExchangeRatesInfo_I_Date').val(),
    //                date,
    //                $('#ExchangeRatesInfo_FromCurrency option:selected').text().split('-')[1].trim() + ' - ' + $('#ExchangeRatesInfo_ToCurrency option:selected').text().trim(),
    //                $('#ExchangeRatesInfo_ExchangeRateType option:selected').text(),
    //                ($('#ExchangeRatesInfo_Provider option:selected').val() != '0' ? $('#ExchangeRatesInfo_Provider option:selected').text() : $('#ExchangeRatesInfo_Terminal option:selected').val() != '0' ? $('#ExchangeRatesInfo_Terminal option:selected').text() : '')
    //                ]);
    //                var aRow = oSettings.aoData[iAdded[0]].nTr;
    //                aRow.setAttribute('id', 'exchangeRate_' + data.ItemID.exchangeRateID);

    //                var array = CATALOG.oExchangeRatesTable.fnGetNodes();
    //                try {
    //                    var nTr = CATALOG.oExchangeRatesTable.$('tr#exchangeRate_' + data.ItemID.oldRateID);
    //                    var position = CATALOG.oExchangeRatesTable.fnGetPosition(nTr[0]);
    //                    CATALOG.oExchangeRatesTable.fnDisplayRow(array[position]);
    //                    CATALOG.oExchangeRatesTable.fnUpdate(data.ItemID.oldRateVigency, $('#exchangeRate_' + data.ItemID.oldRateID)[0], 2);
    //                }
    //                catch (ex) { }
    //                //CATALOG.oExchangeRatesTable.fnSort([[1, 'desc']]);
    //                CATALOG.afterRenderFunctions();
    //                CATALOG.oExchangeRatesTable.fnDisplayRow(aRow);
    //                UI.tablesHoverEffect();
    //                UI.tablesStripedEffect();
    //                CATALOG.makeExchangeRatesSelectable();
    //                $('#exchangeRate_' + data.ItemID.exchangeRateID).click();
    //                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />Modify neccesary fields and update', duration, data.InnerException);
    //            }
    //            else {
    //                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    //            }
    //        }
    //    });
    //}///////////////////

    var afterRenderFunctions = function () {
        //sort rows
        //CATALOG.oExchangeRatesTable.fnSort([[1, 'desc']]);
        CATALOG.oExchangeRatesTable.fnSort([[2, 'desc']]);
        //set expired rows
        CATALOG.oExchangeRatesTable.$('tr').each(function (index) {
            var toDate = $(this)[0].cells[2].textContent.trim();
            var expired = toDate != '' && toDate != 'Permanent' ? UI.validateStartEndDateTimes(toDate, undefined, true) : false;
            if (expired) {
                $(this).addClass('expired');
            }
            else {
                $(this).removeClass('expired');
            }
        });
    }

    //locations
    var locationsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchLocationsResults', tableColumns - 1);
        CATALOG.oLocationsTable = $('#tblSearchLocationsResults').dataTable();
    }

    var makeLocationsSelectable = function () {
        CATALOG.oLocationsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oLocationsTable, $(this));
                    //CATALOG.oLocationsTable.$('tr.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var locationID = CATALOG.oLocationsTable.$('tr.selected-row').attr('id').substr(9);
                    $.ajax({
                        url: '/Catalogs/GetLocation',
                        cache: false,
                        type: 'POST',
                        data: { LocationInfo_LocationID: locationID },
                        success: function (data) {
                            $('#LocationInfo_LocationID').val(data.LocationInfo_LocationID);
                            $('#LocationInfo_Location').val(data.LocationInfo_Location);
                            $('#LocationInfo_LocationCode').val(data.LocationInfo_LocationCode);
                            $('#LocationInfo_Destination option[value="' + data.LocationInfo_Destination + '"]').attr('selected', true);
                            $('#LocationInfo_Terminal option[value="' + data.LocationInfo_Terminal + '"]').attr('selected', true);
                            UI.expandFieldset('fdsLocationInfo');
                            UI.scrollTo('fdsLocationInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteLocation', 'tr', $(e.target), CATALOG.oLocationsTable);
            }
        });
    }

    var saveLocationSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Location Saved') {
                CATALOG.oLocationsTable = $.fn.DataTable.fnIsDataTable('tblSearchLocationsResults') ? $('#tblSearchLocationsResults') : $('#tblSearchLocationsResults').dataTable();
                var oSettings = CATALOG.oLocationsTable.fnSettings();
                var iAdded = CATALOG.oLocationsTable.fnAddData([
                    $('#LocationInfo_Location').val(),
                    $('#LocationInfo_LocationCode').val(),
                    $('#LocationInfo_Destination option:selected').text(),
                    $('#LocationInfo_Terminal option:selected').text()
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'location_' + data.ItemID);
                CATALOG.oLocationsTable.fnDisplayRow(aRow);
                $('#frmLocationInfo').clearForm();
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                CATALOG.makeLocationsSelectable();
            }
            else {
                var array = CATALOG.oLocationsTable.fnGetNodes();
                var nTr = CATALOG.oLocationsTable.$('tr.selected-row');
                var position = CATALOG.oLocationsTable.fnGetPosition(nTr[0]);
                CATALOG.oLocationsTable.fnDisplayRow(array[position]);
                CATALOG.oLocationsTable.fnUpdate([
                    $('#LocationInfo_Location').val(),
                    $('#LocationInfo_LocationCode').val(),
                    $('#LocationInfo_Destination option:selected').text(),
                    $('#LocationInfo_Terminal option:selected').text()
                ], $('#location_' + data.ItemID)[0], undefined, false);
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //opcs
    var addTeamToOPC = function () {
        //validar
        var valid = true;
        var msg = 'There are some required fields to fill.';
        if ($('#OPCTeamInfo_PromotionTeam').val() == "0") {
            valid = false;
            msg += '<br>Team is required.';
        }
        if ($('#OPCTeamInfo_ParentOpc').val() == "0") {
            valid = false;
            msg += '<br>Supervisor is required.';
        }
        if ($('#OPCTeamInfo_JobPosition').val() == "0") {
            valid = false;
            msg += '<br>Job Position is required.';
        }

        if (valid) {
            //agregar
            var Teams = ($('#OPCSInfo_TeamsStr').val() != "" ? eval("(" + $('#OPCSInfo_TeamsStr').val() + ")") : []);
            if ($('#OPCPromotionTeamID').val() == '') {
                //nuevo
                var newTeam = {
                    OPCPromotionTeamID: 0,
                    OPCTeamInfo_PromotionTeam: $('#OPCTeamInfo_PromotionTeam').val(),
                    OPCTeamInfo_PromotionTeamText: $('#OPCTeamInfo_PromotionTeam option:selected').text(),
                    OPCTeamInfo_DrpPromotionTeams: null,
                    OPCTeamInfo_JobPosition: $('#OPCTeamInfo_JobPosition').val(),
                    OPCTeamInfo_JobPositionText: $('#OPCTeamInfo_JobPosition option:selected').text(),
                    OPCTeamInfo_DrpJobPositions: null,
                    OPCTeamInfo_ParentOpc: $('#OPCTeamInfo_ParentOpc').val(),
                    OPCTeamInfo_ParentOpcText: $('#OPCTeamInfo_ParentOpc option:selected').text(),
                    OPCTeamInfo_DrpParentOpcs: null,
                    OPCTeamInfo_EnlistDate: $('#OPCTeamInfo_EnlistDate').val(),
                    OPCTeamInfo_Deleted: $('#OPCTeamInfo_Deleted').is(':checked'),
                    OPCTeamInfo_TerminateDate: $('#OPCTeamInfo_TerminateDate').val(),
                    OPCTeamInfo_TerminateReason: $('#OPCTeamInfo_TerminateReason').val(),
                    OPCTeamInfo_AssignToParentOpc: $('#OPCTeamInfo_AssignToParentOpc').val(),
                    OPCTeamInfo_HasSubs: (!isNaN($('#OPCTeamInfo_AssignToParentOpc').val()) && $('#OPCTeamInfo_AssignToParentOpc').val() != "-1" ? false : true),
                    OPCTeamInfo_DateSaved: COMMON.getDate(),
                    OPCTeamInfo_TemporalID: UI.generateGUID()
                }
                Teams.push(newTeam);
                $('#OPCSInfo_TeamsStr').val($.toJSON(Teams));
            } else {
                //editando
                $.each(Teams, function (t, team) {
                    if (team.OPCTeamInfo_TemporalID == $('#OPCTeamInfo_TemporalID').val()) {
                        team.OPCTeamInfo_PromotionTeam = $('#OPCTeamInfo_PromotionTeam').val();
                        team.OPCTeamInfo_PromotionTeamText = $('#OPCTeamInfo_PromotionTeam option:selected').text();
                        team.OPCTeamInfo_DrpPromotionTeams = null;
                        team.OPCTeamInfo_JobPosition = $('#OPCTeamInfo_JobPosition').val();
                        team.OPCTeamInfo_JobPositionText = $('#OPCTeamInfo_JobPosition option:selected').text();
                        team.OPCTeamInfo_DrpJobPositions = null;
                        team.OPCTeamInfo_ParentOpc = $('#OPCTeamInfo_ParentOpc').val();
                        team.OPCTeamInfo_ParentOpcText = $('#OPCTeamInfo_ParentOpc option:selected').text();
                        team.OPCTeamInfo_DrpParentOpcs = null;
                        team.OPCTeamInfo_EnlistDate = $('#OPCTeamInfo_EnlistDate').val();
                        team.OPCTeamInfo_Deleted = $('#OPCTeamInfo_Deleted').is(':checked');
                        team.OPCTeamInfo_TerminateDate = $('#OPCTeamInfo_TerminateDate').val();
                        team.OPCTeamInfo_TerminateReason = $('#OPCTeamInfo_TerminateReason').val();
                        team.OPCTeamInfo_AssignToParentOpc = $('#OPCTeamInfo_AssignToParentOpc').val();
                        team.OPCTeamInfo_HasSubs = (!isNaN($('#OPCTeamInfo_AssignToParentOpc').val()) && $('#OPCTeamInfo_AssignToParentOpc').val() != "-1" ? false : true),
                            OPCTeamInfo_DateSaved = COMMON.getDate()
                    }
                });
                $('#OPCSInfo_TeamsStr').val($.toJSON(Teams));
            }
            $('#divTeamInfo').slideUp('fast');
            $('#btnOPCAddTeam').slideDown('fast');
            updateOPCTeamsTable(Teams);
        } else {
            UI.messageBox(-1, msg);
        }
    }

    var OPCSResultsTable = function (data) {
        UI.collapseFieldset('fdsOPCSInfo');
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblOPCSResults', tableColumns - 1);
        CATALOG.oOPCSTable = $('#tblOPCSResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makeOPCSSelectable(); });
        //});
        //$('#tblOPCSResults_length').unbind('change').on('change', function () {
        //    CATALOG.makeOPCSSelectable();
        //});
    }

    var makeOPCSSelectable = function () {
        CATALOG.oOPCSTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oOPCSTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var opcID = $(this).attr('id').substr(4);
                    $.ajax({
                        url: '/Catalogs/GetOPC',
                        cache: false,
                        type: 'POST',
                        data: { OPCSInfo_OpcID: opcID },
                        success: function (data) {
                            $('#divEditingOPC').html(data.OPCSInfo_FirstName + " " + data.OPCSInfo_LastName);
                            $('#OPCSInfo_OpcID').val(data.OPCSInfo_OpcID);
                            $('#OPCSInfo_FirstName').val(data.OPCSInfo_FirstName);
                            $('#OPCSInfo_MiddleName').val(data.OPCSInfo_MiddleName);
                            $('#OPCSInfo_LastName').val(data.OPCSInfo_LastName);
                            $('#OPCSInfo_SecondSurname').val(data.OPCSInfo_SecondSurname);
                            $('#OPCSInfo_Credential').val(data.OPCSInfo_Credential);
                            $('#OPCSInfo_Phone1').val(data.OPCSInfo_Phone1);
                            $('#OPCSInfo_Phone2').val(data.OPCSInfo_Phone2);
                            if (data.OPCSInfo_EnlistDate != null) {
                                $('#OPCSInfo_EnlistDate').val(UTILS.serializedDateToDate('#OPCSInfo_EnlistDate', data.OPCSInfo_EnlistDate));
                            }
                            $('#OPCSInfo_PayingCompany').val(data.OPCSInfo_PayingCompany);
                            $('#OPCSInfo_LegacyKey').val(data.OPCSInfo_LegacyKey);
                            $('#OPCSInfo_AvanceID').val(data.OPCSInfo_AvanceID);
                            $('#OPCSInfo_Company').val(data.OPCSInfo_Company);
                            $('#OPCSInfo_TeamInfoEditing').val('valid');

                            //teams
                            $('#OPCSInfo_TeamsStr').val($.toJSON(data.OPCSInfo_Teams));
                            updateOPCTeamsTable(data.OPCSInfo_Teams);
                            UI.expandFieldset('fdsOPCSInfo');
                            UI.scrollTo('fdsOPCSInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteOPC', 'tr', $(e.target), CATALOG.oOPCSTable, UI.updateDependentLists, { url: '/Catalogs/GetDDLData', itemType: 'opc' });
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oOPCSTable);
            }
        });
    }

    function updateOPCTeamsTable(OPCTeams) {
        $('#tblOPCTeams tbody').html('');
        $.each(OPCTeams, function (i, team) {
            var tr = '<tr id="OPCTeamID' + team.OPCPromotionTeamID + '" data-temporalID="' + team.OPCTeamInfo_TemporalID + '"><td>' + team.OPCTeamInfo_PromotionTeamText + '</td><td>' + team.OPCTeamInfo_JobPositionText + '</td><td>' + team.OPCTeamInfo_ParentOpcText + '</td><td id="OPCTeamEnlistDate' + i + '">' + (team.OPCTeamInfo_EnlistDate != "" ? team.OPCTeamInfo_EnlistDate : '') + '</td><td>' + (team.OPCTeamInfo_Deleted ? 'Terminated' : 'Active') + '</td><td>' + (team.OPCTeamInfo_TerminateDate != "" ? team.OPCTeamInfo_TerminateDate : '')
                + '</td><td>' + (team.OPCTeamInfo_TerminateReason != null ? team.OPCTeamInfo_TerminateReason : '') + '</td>' + (team.OPCTeamInfo_DateSaved == COMMON.getDate() ? '<td><img src="/Content/themes/base/images/cross.png" class="right delete-opc-team"></td>' : '<td></td>') + '</tr>';
            $('#tblOPCTeams tbody').append(tr);
        });
        UI.tablesHoverEffect();
        UI.tablesStripedEffect();
        $('.delete-opc-team').off('click').on('click', function () {
            var id = $(this).parent().parent().attr('data-temporalID');
            console.log(id);
            $('tr[data-temporalID="' + id + '"]').remove();
            var Teams = eval('(' + $('#OPCSInfo_TeamsStr').val() + ')');
            var indexToDelete = -1;
            $.each(Teams, function (t, team) {
                if (team.OPCTeamInfo_TemporalID == id) {
                    indexToDelete = t;
                }
            });
            if (indexToDelete != -1) {
                Teams.splice(indexToDelete, 1);
                $('#OPCSInfo_TeamsStr').val($.toJSON(Teams));
            }
            if (id == $('#OPCTeamInfo_TemporalID').val() && $('#divTeamInfo').is(':visible')) {
                $('#btnCancelTeamInfo').trigger('click');
            }
        });
        $('#tblOPCTeams>tbody>tr').off('click').on('click', function () {
            var temporalID = $(this).attr('data-temporalID');
            var Teams = eval('(' + $('#OPCSInfo_TeamsStr').val() + ')');
            $.each(Teams, function (t, team) {
                if (team.OPCTeamInfo_TemporalID == temporalID) {
                    $('#OPCPromotionTeamID').val(team.OPCPromotionTeamID);
                    $('#OPCTeamInfo_TemporalID').val(team.OPCTeamInfo_TemporalID);
                    $('#OPCTeamInfo_PromotionTeam').val(team.OPCTeamInfo_PromotionTeam);
                    if (team.OPCTeamInfo_DateSaved == COMMON.getDate()) {
                        $('#OPCTeamInfo_PromotionTeam').prop('disabled', false);
                    } else {
                        $('#OPCTeamInfo_PromotionTeam').prop('disabled', true);
                    }
                    if ($('#OPCTeamInfo_ParentOpc option[value=54]').length == 0) {
                        $('#OPCTeamInfo_ParentOpc').append('<option value="' + team.OPCTeamInfo_ParentOpc + '">' + team.OPCTeamInfo_ParentOpcText + '</option>');
                    }
                    $('#OPCTeamInfo_ParentOpc').val(team.OPCTeamInfo_ParentOpc);
                    $('#OPCTeamInfo_JobPosition').val(team.OPCTeamInfo_JobPosition);
                    $('#OPCTeamInfo_EnlistDate').val(team.OPCTeamInfo_EnlistDate);
                    $('#OPCTeamInfo_Deleted').prop('checked', team.OPCTeamInfo_Deleted);
                    $('#OPCTeamInfo_Deleted').trigger('valueChange')
                    $('#OPCTeamInfo_TerminateDate').val(team.OPCTeamInfo_TerminateDate);
                    $('#OPCTeamInfo_TerminateReason').val(team.OPCTeamInfo_TerminateReason);
                    $('#OPCTeamInfo_HasSubs').prop('checked', team.OPCTeamInfo_HasSubs);
                    $('#OPCTeamInfo_HasSubs').trigger('valueChange');
                    $('#OPCTeamInfo_Subs').text(team.OPCTeamInfo_Subs);
                    $('#OPCTeamInfo_AssignToParentOpc').val(team.OPCTeamInfo_AssignToParentOpc);
                    $('#OPCSInfo_TeamInfoEditing').val('');
                    $('#divTeamInfo').slideDown('fast');
                }
            });
        });
    }

    var saveOPCSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'OPC Saved') {
                CATALOG.oOPCSTable = $.fn.DataTable.fnIsDataTable('tblOPCSResults') ? $('#tblOPCSResults') : $('#tblOPCSResults').dataTable();
                var oSettings = CATALOG.oOPCSTable.fnSettings();
                if (oSettings != null) {
                    var iAdded = CATALOG.oOPCSTable.fnAddData([
                        data.ItemID,
                        $('#OPCTeamInfo_PromotionTeam option:selected').val() != 0 ? $('#OPCTeamInfo_PromotionTeam option:selected').text() : "",
                        $('#OPCSInfo_Credential').val(),
                        ($('#OPCSInfo_FirstName').val() + ' ' + $('#OPCSInfo_MiddleName').val() + ' ' + $('#OPCSInfo_LastName').val() + ' ' + $('#OPCSInfo_SecondSurname').val()).replace('  ', ' '),
                        $('#OPCTeamInfo_JobPosition option:selected').val() != 0 ? $('#OPCTeamInfo_JobPosition option:selected').text() : "",
                        $('#OPCSInfo_Phone1').val() + " / " + $('#OPCSInfo_Phone2').val(),
                        $('#OPCSInfo_PayingCompany option:selected').val() != 0 ? $('#OPCSInfo_PayingCompany option:selected').text() : "",
                        $('#OPCSInfo_Company option:selected').val() != 0 ? $('#OPCSInfo_Company option:selected').text() : "",
                        $('#OPCSInfo_LegacyKey').val(),
                        $('#OPCSInfo_AvanceID').val(),
                        $('#OPCSInfo_EnlistDate').val(),
                        ($('#OPCTeamInfo_Deleted').is(':checked') ? 'No' : 'Yes')
                    ]);
                    var aRow = oSettings.aoData[iAdded[0]].nTr;
                    aRow.setAttribute('id', 'opc_' + data.ItemID);
                    CATALOG.oOPCSTable.fnDisplayRow(aRow);
                }
                $('#frmOPCInfo').clearForm();
                $('#tblOPCTeams tbody').html('');
                UI.tablesHoverEffect();
                CATALOG.makeOPCSSelectable();
            }
            else {
                var array = CATALOG.oOPCSTable.fnGetNodes();
                var nTr = CATALOG.oOPCSTable.$('tr.selected-row');
                var position = CATALOG.oOPCSTable.fnGetPosition(nTr[0]);
                CATALOG.oOPCSTable.fnDisplayRow(array[position]);
                CATALOG.oOPCSTable.fnUpdate([
                    $('#OPCSInfo_OpcID').val(),
                    $('#OPCTeamInfo_PromotionTeam option:selected').val() != 0 ? $('#OPCTeamInfo_PromotionTeam option:selected').text() : "",
                    $('#OPCSInfo_Credential').val(),
                    ($('#OPCSInfo_FirstName').val() + ' ' + $('#OPCSInfo_MiddleName').val() + ' ' + $('#OPCSInfo_LastName').val() + ' ' + $('#OPCSInfo_SecondSurname').val()).replace('  ', ' '),
                    $('#OPCTeamInfo_JobPosition option:selected').val() != 0 ? $('#OPCTeamInfo_JobPosition option:selected').text() : "",
                    $('#OPCSInfo_Phone1').val() + " / " + $('#OPCSInfo_Phone2').val(),
                    $('#OPCSInfo_PayingCompany option:selected').val() != 0 ? $('#OPCSInfo_PayingCompany option:selected').text() : "",
                    $('#OPCSInfo_Company option:selected').val() != 0 ? $('#OPCSInfo_Company option:selected').text() : "",
                    $('#OPCSInfo_LegacyKey').val(),
                    $('#OPCSInfo_AvanceID').val(),
                    $('#OPCSInfo_EnlistDate').val(),
                    ($('#OPCTeamInfo_Deleted').is(':checked') ? 'No' : 'Yes')
                ], $('#opc_' + data.ItemID)[0], undefined, false);
            }
            UI.updateDependentLists('/Catalogs/GetDDLData', 'opc');
            //CATALOG.updateDependentLists('opc');
            //$('#fdsOPCSInfo legend').trigger('click');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //placeClasifications
    var placeClasificationsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchPlaceClasificationsResults', tableColumns - 1);
        CATALOG.oPlaceClasificationsTable = $('#tblSearchPlaceClasificationsResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makePlaceClasificationsSelectable(); });
        //});
        //$('#tblSearchPlaceClasificationsResults_length').unbind('change').on('change', function () {
        //    CATALOG.makePlaceClasificationsSelectable();
        //});
    }

    var makePlaceClasificationsSelectable = function () {
        CATALOG.oPlaceClasificationsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oPlaceClasificationsTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var placeClasificationID = $(this).attr('id').substr(19);
                    $.ajax({
                        url: '/Catalogs/GetPlaceClasification',
                        cache: false,
                        type: 'POST',
                        data: { PlaceClasificationInfo_PlaceClasificationID: placeClasificationID },
                        success: function (data) {
                            $('#PlaceClasificationInfo_PlaceClasificationID').val(data.PlaceClasificationInfo_PlaceClasificationID);
                            $('#PlaceClasificationInfo_PlaceClasification').val(data.PlaceClasificationInfo_PlaceClasification);
                            $('#PlaceClasificationInfo_PlaceType option[value="' + data.PlaceClasificationInfo_PlaceType + '"]').attr('selected', true);
                            if (data.PlaceClasificationInfo_Hosting) {
                                $('input:radio[name=PlaceClasificationInfo_Hosting]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PlaceClasificationInfo_Hosting]')[1].checked = true;
                            }
                            UI.expandFieldset('fdsPlaceClasificationsInfo');
                            UI.scrollTo('fdsPlaceClasificationsInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeletePlaceClasification', 'tr', $(e.target), CATALOG.oPlaceClasificationsTable);
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oPlaceClasificationsTable);
            }
        });
    }

    var savePlaceClasificationSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Place Clasification Saved') {
                CATALOG.oPlaceClasificationsTable = $.fn.DataTable.fnIsDataTable('tblSearchPlaceClasificationsResults') ? $('#tblSearchPlaceClasificationsResults') : $('#tblSearchPlaceClasificationsResults').dataTable();
                var oSettings = CATALOG.oPlaceClasificationsTable.fnSettings();
                var iAdded = CATALOG.oPlaceClasificationsTable.fnAddData([
                    $('#PlaceClasificationInfo_PlaceClasification').val(),
                    $('#PlaceClasificationInfo_PlaceType option:selected').text(),
                    $('input:radio[name="PlaceClasificationInfo_Hosting"]:checked').val() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'placeClasification_' + data.ItemID);
                CATALOG.oPlaceClasificationsTable.fnDisplayRow(aRow);
                $('#frmPlaceClasificationInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makePlaceClasificationsSelectable();
            }
            else {
                $('#placeClasification_' + data.ItemID).children('td:nth-child(1)').text($('#PlaceClasificationInfo_PlaceClasification').val());
                $('#placeClasification_' + data.ItemID).children('td:nth-child(2)').text($('#PlaceClasificationInfo_PlaceType option:selected').text());
                $('#placeClasification_' + data.ItemID).children('td:nth-child(3)')[0].firstChild.textContent = $('input:radio[name="PlaceTypeInfo_IsActive"]:checked').val();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //placeTypes
    var placeTypesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchPlaceTypesResults', tableColumns - 1);
        CATALOG.oPlaceTypesTable = $('#tblSearchPlaceTypesResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makePlaceTypesSelectable(); });
        //});
        //$('#tblSearchPlaceTypesResults_length').unbind('change').on('change', function () {
        //    CATALOG.makePlaceTypesSelectable();
        //});
    }

    var makePlaceTypesSelectable = function () {
        CATALOG.oPlaceTypesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oPlaceTypesTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var placeTypeID = $(this).attr('id').substr(10);
                    $.ajax({
                        url: '/Catalogs/GetPlaceType',
                        cache: false,
                        type: 'POST',
                        data: { PlaceTypeInfo_PlaceTypeID: placeTypeID },
                        success: function (data) {
                            $('#PlaceTypeInfo_PlaceTypeID').val(data.PlaceTypeInfo_PlaceTypeID);
                            $('#PlaceTypeInfo_PlaceType').val(data.PlaceTypeInfo_PlaceType);
                            if (data.PlaceTypeInfo_IsActive) {
                                $('input:radio[name=PlaceTypeInfo_IsActive]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PlaceTypeInfo_IsActive]')[1].checked = true;
                            }
                            UI.expandFieldset('fdsPlaceTypesInfo');
                            UI.scrollTo('fdsPlaceTypesInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeletePlaceType', 'tr', $(e.target), CATALOG.oPlaceTypesTable, UI.updateDependentLists, { url: '/Catalogs/GetDDLData', itemType: 'placeType' });
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oPlaceTypesTable);
            }
        });
    }

    var savePlaceTypeSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Place Type Saved') {
                CATALOG.oPlaceTypesTable = $.fn.DataTable.fnIsDataTable('tblSearchPlaceTypesResults') ? $('#tblSearchPlaceTypesResults') : $('#tblSearchPlaceTypesResults').dataTable();
                var oSettings = CATALOG.oPlaceTypesTable.fnSettings();
                var iAdded = CATALOG.oPlaceTypesTable.fnAddData([
                    $('#PlaceTypeInfo_PlaceType').val(),
                    $('input:radio[name="PlaceTypeInfo_IsActive"]:checked').val() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'placeType_' + data.ItemID);
                CATALOG.oPlaceTypesTable.fnDisplayRow(aRow);
                $('#frmPlaceTypeInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makePlaceTypesSelectable();
            }
            else {
                $('#placeType_' + data.ItemID).children('td:nth-child(1)').text($('#PlaceTypeInfo_PlaceType').val());
                $('#placeType_' + data.ItemID).children('td:nth-child(2)')[0].firstChild.textContent = $('input:radio[name="PlaceTypeInfo_IsActive"]:checked').val();
            }
            UI.updateDependentLists('/Catalogs/GetDDLData', 'placeType');
            //CATALOG.updateDependentLists('placeType');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //pointsOfSale
    var pointsOfSaleResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchPointsOfSaleResults', tableColumns - 1);
        CATALOG.oPointsOfSaleTable = $('#tblSearchPointsOfSaleResults').dataTable();
    }

    var makePointsOfSaleSelectable = function () {
        CATALOG.oPointsOfSaleTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oPointsOfSaleTable, $(this));
                    //CATALOG.oPointsOfSaleTable.$('tr.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var pointOfSaleID = CATALOG.oPointsOfSaleTable.$('tr.selected-row').attr('id').substr(12);
                    $.ajax({
                        url: '/PointsOfSale/GetPointOfSale',
                        cache: false,
                        type: 'POST',
                        data: { PointsOfSaleInfo_PointOfSaleID: pointOfSaleID },
                        success: function (data) {
                            $('#PointsOfSaleInfo_PointOfSaleID').val(data.PointsOfSaleInfo_PointOfSaleID);
                            $('#PointsOfSaleInfo_PointOfSale').val(data.PointsOfSaleInfo_PointOfSale);
                            $('#PointsOfSaleInfo_ShortName').val(data.PointsOfSaleInfo_ShortName);
                            $('#PointsOfSaleInfo_Place').val(data.PointsOfSaleInfo_Place);
                            $('#PointsOfSaleInfo_PoliciesBlock').val(data.PointsOfSaleInfo_PoliciesBlock);
                            if ($('input:radio[name="PointsOfSaleInfo_AcceptCharges"]').length > 0) {
                                if (data.PointsOfSaleInfo_AcceptCharges) {
                                    $('input:radio[name="PointsOfSaleInfo_AcceptCharges"]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name="PointsOfSaleInfo_AcceptCharges"]')[1].checked = true;
                                }
                            }
                            $('#PointsOfSaleInfo_Terminal').val(data.PointsOfSaleInfo_Terminal);
                            if (!$('#fdsPointOfSaleInfo').children('div:first').is(':visible')) {
                                UI.expandFieldset('fdsPointOfSaleInfo');
                            }
                            UI.scrollTo('fdsPointOfSaleInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/PointsOfSale/DeletePointOfSale', 'tr', $(e.target), CATALOG.oPointsOfSaleTable, UI.updateDependentLists, { url: '/Catalogs/GetDDLData', itemType: 'pointOfSale' });
            }
        });
    }

    var savePointOfSaleSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Point Of Sale Saved') {
                CATALOG.oPointsOfSaleTable = $.fn.DataTable.fnIsDataTable('tblSearchPointsOfSaleResults') ? $('#tblSearchPointsOfSaleResults') : $('#tblSearchPointsOfSaleResults').dataTable();
                var oSettings = CATALOG.oPointsOfSaleTable.fnSettings();
                var iAdded = CATALOG.oPointsOfSaleTable.fnAddData([
                    $('#PointsOfSaleInfo_PointOfSale').val(),
                    $('#PointsOfSaleInfo_ShortName').val(),
                    $('#PointsOfSaleInfo_Terminal option:selected').text(),
                    $('#PointsOfSaleInfo_Place option:selected').text(),
                    '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'pointOfSale_' + data.ItemID);
                CATALOG.oPointsOfSaleTable.fnDisplayRow(aRow);
                $('#frmPointOfSaleInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makePointsOfSaleSelectable();
            }
            else {
                var array = CATALOG.oPointsOfSaleTable.fnGetNodes();
                var nTr = CATALOG.oPointsOfSaleTable.$('tr.selected-row');
                var position = CATALOG.oPointsOfSaleTable.fnGetPosition(nTr[0]);
                CATALOG.oPointsOfSaleTable.fnDisplayRow(array[position]);
                CATALOG.oPointsOfSaleTable.fnUpdate([
                    $('#PointsOfSaleInfo_PointOfSale').val(),
                    $('#PointsOfSaleInfo_ShortName').val(),
                    $('#PointsOfSaleInfo_Terminal option:selected').text(),
                    $('#PointsOfSaleInfo_Place option:selected').text(),
                    '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ], $('#pointOfSale_' + data.ItemID)[0], undefined, false);
            }
            UI.updateDependentLists('/Catalogs/GetDDLData', 'pointOfSale');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //promotionTeams
    var promotionTeamsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchPromotionTeamsResults', tableColumns - 1);
        CATALOG.oPromotionTeamsTable = $('#tblSearchPromotionTeamsResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makePromotionTeamsSelectable(); });
        //});
        //$('#tblSearchPromotionTeamsResults_length').unbind('change').on('change', function () {
        //    CATALOG.makePromotionTeamsSelectable();
        //});
    }

    var makePromotionTeamsSelectable = function () {
        CATALOG.oPromotionTeamsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oPromotionTeamsTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var promotionTeamID = $(this).attr('id').substr(14);
                    $.ajax({
                        url: '/Catalogs/GetPromotionTeam',
                        cache: false,
                        type: 'POST',
                        data: { PromotionTeamsInfo_PromotionTeamID: promotionTeamID },
                        success: function (data) {
                            $('#PromotionTeamsInfo_PromotionTeamID').val(data.PromotionTeamsInfo_PromotionTeamID);
                            $('#PromotionTeamsInfo_PromotionTeam').val(data.PromotionTeamsInfo_PromotionTeam);
                            $('#PromotionTeamsInfo_Destination').val(data.PromotionTeamsInfo_Destination);
                            $('#PromotionTeamsInfo_Company').val(data.PromotionTeamsInfo_Company);
                            $('#PromotionTeamsInfo_GiftingBudget').val(data.PromotionTeamsInfo_GiftingBudget);
                            UI.expandFieldset('fdsPromotionTeamInfo');
                            UI.scrollTo('fdsPromotionTeamInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeletePromotionTeam', 'tr', $(e.target), CATALOG.oPromotionTeamsTable, UI.updateDependentLists, { url: '/Catalogs/GetDDLData', itemType: 'promotionTeam' });
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oPromotionTeamsTable);
            }
        });
    }

    var savePromotionTeamSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Promotion Team Saved') {
                CATALOG.oPromotionTeamsTable = $.fn.DataTable.fnIsDataTable('tblSearchPromotionTeamsResults') ? $('#tblSearchPromotionTeamsResults') : $('#tblSearchPromotionTeamsResults').dataTable();
                var oSettings = CATALOG.oPromotionTeamsTable.fnSettings();
                var iAdded = CATALOG.oPromotionTeamsTable.fnAddData([
                    $('#PromotionTeamsInfo_PromotionTeam').val(),
                    $('#PromotionTeamsInfo_Destination option:selected').text(),
                    $('#PromotionTeamsInfo_Company option:selected').text() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'promotionTeam_' + data.ItemID);
                CATALOG.oPromotionTeamsTable.fnDisplayRow(aRow);
                $('#frmPromotionTeamInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makePromotionTeamsSelectable();
            }
            else {
                $('#promotionTeam_' + data.ItemID).children('td:nth-child(1)').text($('#PromotionTeamsInfo_PromotionTeam').val());
                $('#promotionTeam_' + data.ItemID).children('td:nth-child(2)').text($('#PromotionTeamsInfo_Destination option:selected').text());
                $('#promotionTeam_' + data.ItemID).children('td:nth-child(3)')[0].firstChild.textContent = $('#PromotionTeamsInfo_Company option:selected').text();
            }
            UI.updateDependentLists('/Catalogs/GetDDLData', 'promotionTeam');
            //CATALOG.updateDependentLists('promotionTeam');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //providers
    var providersResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchProvidersResults', tableColumns - 1);
        CATALOG.oProvidersTable = $('#tblSearchProvidersResults').dataTable();
    }

    var makeProvidersSelectable = function () {
        CATALOG.oProvidersTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oProvidersTable, $(this));
                    //CATALOG.oProvidersTable.$('tr.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var providerID = CATALOG.oProvidersTable.$('tr.selected-row').attr('id').substr(9);
                    $.ajax({
                        url: '/Providers/GetProvider',
                        cache: false,
                        type: 'POST',
                        data: { ProviderInfo_ProviderID: providerID },
                        success: function (data) {
                            $('#ProviderInfo_ProviderID').val(data.ProviderInfo_ProviderID);
                            $('#ProviderInfo_Terminal').val(data.ProviderInfo_Terminal);
                            $('#ProviderInfo_Destination').val(data.ProviderInfo_Destination);
                            $('#ProviderInfo_ComercialName').val(data.ProviderInfo_ComercialName);
                            $('#ProviderInfo_ShortName').val(data.ProviderInfo_ShortName);
                            $('#ProviderInfo_RFC').val(data.ProviderInfo_RFC);
                            $('#ProviderInfo_TaxesName').val(data.ProviderInfo_TaxesName);
                            $('#ProviderInfo_Phone1').val(data.ProviderInfo_Phone1);
                            $('#ProviderInfo_Ext1').val(data.ProviderInfo_Ext1);
                            $('#ProviderInfo_Phone2').val(data.ProviderInfo_Phone2);
                            $('#ProviderInfo_Ext2').val(data.ProviderInfo_Ext2);
                            $('#ProviderInfo_ContactName').val(data.ProviderInfo_ContactName);
                            $('#ProviderInfo_ContactEmail').val(data.ProviderInfo_ContactEmail);
                            $('#ProviderInfo_ProviderType').val(data.ProviderInfo_ProviderType);
                            $('#ProviderInfo_ForCompany').val(data.ProviderInfo_ForCompany);
                            $('#ProviderInfo_ContractCurrency').val(data.ProviderInfo_ContractCurrency);
                            $('#ProviderInfo_AvanceProvider').val(data.ProviderInfo_AvanceProvider);
                            $('#ProviderInfo_MXNAvanceProvider').val(data.ProviderInfo_MXNAvanceProvider);
                            $('#ProviderInfo_InvoiceCurrency').val(data.ProviderInfo_InvoiceCurrency);
                            if (data.ProviderInfo_IsActive) {
                                $('input:radio[name="ProviderInfo_IsActive"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="ProviderInfo_IsActive"]')[1].checked = true;
                            }
                            UI.expandFieldset('fdsProviderInfo');
                            UI.scrollTo('fdsProviderInfo', null);
                            CATALOG.uploadFileToProvider(data.ProviderInfo_ProviderID);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Providers/DeleteProvider', 'tr', $(e.target), CATALOG.oProvidersTable);
            }
        });
    }

    var saveProviderSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Provider Saved') {
                CATALOG.oProvidersTable = $.fn.DataTable.fnIsDataTable('tblSearchProvidersResults') ? $('#tblSearchProvidersResults') : $('#tblSearchProvidersResults').dataTable();
                var oSettings = CATALOG.oProvidersTable.fnSettings();
                var iAdded = CATALOG.oProvidersTable.fnAddData([
                    $('#ProviderInfo_ComercialName').val(),
                    $('#ProviderInfo_TaxesName').val(),
                    $('#ProviderInfo_RFC').val(),
                    $('#ProviderInfo_Terminal option:selected').text(),
                    $('#ProviderInfo_ContractCurrency option:selected').text(),
                    $('input:radio[name="ProviderInfo_IsActive"]:checked').val() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'provider_' + data.ItemID);
                CATALOG.oProvidersTable.fnDisplayRow(aRow);
                $('#frmProviderInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeProvidersSelectable();
            }
            else {
                var array = CATALOG.oProvidersTable.fnGetNodes();
                var nTr = CATALOG.oProvidersTable.$('tr.selected-row');
                var position = CATALOG.oProvidersTable.fnGetPosition(nTr[0]);
                CATALOG.oProvidersTable.fnDisplayRow(array[position]);
                CATALOG.oProvidersTable.fnUpdate([
                    $('#ProviderInfo_ComercialName').val(),
                    $('#ProviderInfo_TaxesName').val(),
                    $('#ProviderInfo_RFC').val(),
                    $('#ProviderInfo_Terminal option:selected').text(),
                    $('#ProviderInfo_ContractCurrency option:selected').text(),
                    $('input:radio[name="ProviderInfo_IsActive"]:checked').val()
                    + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ], $('#provider_' + data.ItemID)[0], undefined, false);
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var uploadFileToProvider = function (providerID) {
        if (providerID != undefined) {
            $('#divFileUploaderContainer').show();
            $('#fileToUploadContainer').hide();
            providerID = providerID != undefined ? providerID : 0;
            if (providerID != 0) {
                CATALOG.getFilesOfProvider(providerID);
            }
            $('#fileUploader').fineUploader({
                request: {
                    endpoint: '/Providers/UploadFile',
                    params: {
                        path: 'p_' + providerID,
                        newProvider: false
                    }
                },
                multiple: true,
                failedUploadTextDisplay: {
                    mode: 'default'
                }
            });
        }
        else {
            $('#divFileUploaderContainer').hide();
            $('#fileToUploadContainer').show();
        }
    }

    var getFilesOfProvider = function (providerID) {
        $.getJSON('/Providers/GetFilesOfProvider', { providerID: providerID }, function (data) {
            var builder = '';
            if (data != null) {
                $.each(data, function (index, item) {
                    builder += '<tr id="' + item.substr(item.indexOf('p_') + 2) + '"><td><a target="_blank" href="' + item + '">' + item + '</a></td>'
                        + '<td><img src="/Content/themes/base/images/cross.png" class="right delete-icon"/></td></tr>';
                });
            }
            $('#tblFilesUploaded tbody').empty();
            $('#tblFilesUploaded tbody').append(builder);
            //CATALOG.uploadFileToProvider();
            //bind event to delete icon
            CATALOG.deleteFileOfProvider();
        });
    }

    var deleteFileOfProvider = function () {
        $('.delete-icon').unbind('click').on('click', function () {
            UI.confirmBox('Do you confirm you want to proceed?', deleteFile, [$(this).parents('tr:first').attr('id'), $(this).parents('tr:first')]);
        });

    }

    function deleteFile(file, element) {
        $.ajax({
            url: '/Providers/DeleteFileOfProvider',
            type: 'POST',
            cache: false,
            data: { file: file },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if (data.ResponseMessage == 'File Deleted') {
                        $(element).remove();
                    }
                    else {
                        UI.confirmBox('Do you confirm you want to remove row from table?', fileNotFound, [data.ItemID]);
                    }
                    CATALOG.getFilesOfProvider($(element).attr('id').split('\\')[0]);
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function fileNotFound(file) {
        $('#' + file).remove();
    }

    //transportationZones
    var transportationZonesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchTransportationZonesResults', tableColumns - 1);
        CATALOG.oTransportationZonesTable = $('#tblSearchTransportationZonesResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makeTransportationZonesSelectable(); });
        //});
        //$('#tblSearchTransportationZonesResults_length').unbind('change').on('change', function () {
        //    CATALOG.makeTransportationZonesSelectable();
        //});
    }

    var makeTransportationZonesSelectable = function () {
        CATALOG.oTransportationZonesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oTransportationZonesTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var transportationZoneID = $(this).attr('id').substr(19);
                    $.ajax({
                        url: '/Catalogs/GetTransportationZone',
                        cache: false,
                        type: 'POST',
                        data: { TransportationZoneInfo_TransportationZoneID: transportationZoneID },
                        success: function (data) {
                            $('#TransportationZoneInfo_TransportationZoneID').val(data.TransportationZoneInfo_TransportationZoneID);
                            $('#TransportationZoneInfo_TransportationZone').val(data.TransportationZoneInfo_TransportationZone);
                            $('#TransportationZoneInfo_Destination option[value="' + data.TransportationZoneInfo_Destination + '"]').attr('selected', true);
                            UI.expandFieldset('fdsTransportationZonesInfo');
                            UI.scrollTo('fdsTransportationZonesInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteTransportationZone', 'tr', $(e.target), CATALOG.oTransportationZonesTable);
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oTransportationZonesTable);
            }
        });
    }

    var saveTransportationZoneSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Transportation Zone Saved') {
                CATALOG.oTransportationZonesTable = $.fn.DataTable.fnIsDataTable('tblSearchTransportationZonesResults') ? $('#tblSearchTransportationZonesResults') : $('#tblSearchTransportationZonesResults').dataTable();
                var oSettings = CATALOG.oTransportationZonesTable.fnSettings();
                var iAdded = CATALOG.oTransportationZonesTable.fnAddData([
                    $('#TransportationZoneInfo_TransportationZone').val(),
                    $('#TransportationZoneInfo_Destination option:selected').text() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'transportationZone_' + data.ItemID);
                CATALOG.oTransportationZonesTable.fnDisplayRow(aRow);
                $('#frmTransportationZoneInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeTransportationZonesSelectable();
            }
            else {
                $('#transportationZone_' + data.ItemID).children('td:nth-child(1)').text($('#TransportationZoneInfo_TransportationZone').val());
                $('#transportationZone_' + data.ItemID).children('td:nth-child(2)')[0].firstChild.textContent = $('#TransportationZoneInfo_Destination option:selected').text();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //users lead sources
    var usersLeadSourcesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td') - 1;
        UI.searchResultsTable('tblUserLeadSourcesResults', tableColumns);
        CATALOG.oUsersLeadSourcesTable = $.fn.DataTable.fnIsDataTable('tblUserLeadSourcesResults') ? $('#tblUserLeadSourcesResults') : $('#tblUserLeadSourcesResults').dataTable();
        CATALOG.oUsersLeadSourcesTable.fnSort([[4, 'desc']]);
    }

    var makeTblUserLeadSourcesSelectable = function () {
        CATALOG.oUsersLeadSourcesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(this).hasClass('expired')) {
                CATALOG.oUsersLeadSourcesTable.$('tr.selected-row').removeClass('selected-row secondary');
                $('#frmSaveUserLeadSource').clearForm();
                $.ajax({
                    url: '/Catalogs/GetUserLeadSource',
                    cache: false,
                    type: 'POST',
                    data: { id: $(this).attr('id') },
                    success: function (data) {
                        CATALOG.oUsersLeadSourcesTable.$('tr#'+data.User_LeadSourceID).addClass('selected-row secondary');
                        $('#User_LeadSourceID').val(data.User_LeadSourceID);
                        $('#Users option[value="' + data.Users[0] + '"]').attr('selected', true);
                        $('#Users').multiselect('refresh');
                        $('#Resorts option[value="' + data.Resorts[0] + '"]').attr('selected', true);
                        $('#Resorts').multiselect('refresh');
                        $('#LeadSources option[value="' + data.LeadSources[0] + '"]').attr('selected', true);
                        $('#LeadSources').multiselect('refresh');
                        $('#FromDate').val(data.FromDate);
                        $('#ToDate').val(data.ToDate);
                        UI.expandFieldset('fdsUserLeadSourceInfo');
                        UI.scrollTo('fdsUserLeadSourceInfo', null);
                    }
                });
            }
        });
    }

    var saveUserLeadSourceSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Rule(s) Saved') {
                var oSettings = CATALOG.oUsersLeadSourcesTable.fnSettings();
                $.each($.parseJSON(data.ItemID), function (index, item) {
                    var iAdded = CATALOG.oUsersLeadSourcesTable.fnAddData([
                        item.User,
                        item.Resort,
                        item.LeadSource,
                        item.FromDate,
                        (item.ToDate != '' ? item.ToDate : 'Permanent')
                    ]);
                    var aRow = oSettings.aoData[iAdded[0]].nTr;
                    aRow.setAttribute('id', item.User_LeadSourceID);
                    CATALOG.oUsersLeadSourcesTable.fnDisplayRow(aRow);
                });
                CATALOG.makeTblUserLeadSourcesSelectable();
                $('#frmSaveUserLeadSource').clearForm();
            }
            else if (data.ResponseMessage == 'Rule Updated') {
                CATALOG.oUsersLeadSourcesTable.fnUpdate([
                    $('#Users option:selected').text(),
                    $('#Resorts option:selected').text(),
                    $('#LeadSources option:selected').text(),
                    $('#FromDate').val(),
                    ($('#ToDate').val() != '' ? $('#ToDate').val() : 'Permanent'),
                    data.ItemID.user
                ], $('#' + data.ItemID.id)[0], undefined, false);
            }
            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
        }
    }

    //zones
    var zonesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchZonesResults', tableColumns - 1);
        CATALOG.oZonesTable = $('#tblSearchZonesResults').dataTable();
        //$('.paging_two_button').children().on('mousedown', function (e) {
        //    if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
        //        var event = $.Event('keydown');
        //        event.keyCode = 27;
        //        $(document).trigger(event);
        //    }
        //    $(this).on('click', function () { CATALOG.makeZonesSelectable(); });
        //});
        //$('#tblSearchZonesResults_length').unbind('change').on('change', function () {
        //    CATALOG.makeZonesSelectable();
        //});
    }

    var makeZonesSelectable = function () {
        CATALOG.oZonesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oZonesTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var zoneID = $(this).attr('id').substr(5);
                    $.ajax({
                        url: '/Catalogs/GetZone',
                        cache: false,
                        type: 'POST',
                        data: { ZoneInfo_ZoneID: zoneID },
                        success: function (data) {
                            $('#ZoneInfo_ZoneID').val(data.ZoneInfo_ZoneID);
                            $('#ZoneInfo_Zone').val(data.ZoneInfo_Zone);
                            $('#ZoneInfo_Destination option[value="' + data.ZoneInfo_Destination + '"]').attr('selected', true);
                            UI.expandFieldset('fdsZonesInfo');
                            UI.scrollTo('fdsZonesInfo', null);
                        }
                    });
                }
            }
            else {
                UI.deleteDataTableItemFunction('/Catalogs/DeleteZone', 'tr', $(e.target), CATALOG.oZonesTable);
                //CATALOG.deleteItemFunction('tr', $(e.target), CATALOG.oZonesTable);
            }
        });
    }

    var saveZoneSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Zone Saved') {
                CATALOG.oZonesTable = $.fn.DataTable.fnIsDataTable('tblSearchZonesResults') ? $('#tblSearchZonesResults') : $('#tblSearchZonesResults').dataTable();
                var oSettings = CATALOG.oZonesTable.fnSettings();
                var iAdded = CATALOG.oZonesTable.fnAddData([
                    $('#ZoneInfo_Zone').val(),
                    $('#ZoneInfo_Destination option:selected').text() + '<img src="/Content/themes/base/images/cross.png" class="right delete-item">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'zone_' + data.ItemID);
                CATALOG.oZonesTable.fnDisplayRow(aRow);
                $('#frmZoneInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeZonesSelectable();
            }
            else {
                $('#zone_' + data.ItemID).children('td:nth-child(1)').text($('#ZoneInfo_Zone').val());
                $('#zone_' + data.ItemID).children('td:nth-child(2)')[0].firstChild.textContent = $('#ZoneInfo_Destination option:selected').text();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //general
    var deleteItemFunction = function (itemType, $target, oTable) {
        var itemID = $target.parents(itemType).first().attr('id');
        UI.confirmBox('Do you confirm you want to proceed?', deleteItem, [itemID.substr(0, itemID.indexOf('_')), itemID.substr(itemID.indexOf('_') + 1, itemID.length - itemID.indexOf('_')), itemType, oTable]);
    }

    function deleteItem(item, itemID, tag, oTable) {
        var itemType = item.substr(0, 1).toUpperCase() + item.substr(1, item.length - 1);
        $.ajax({
            url: '/Catalogs/Delete' + itemType,
            type: 'POST',
            cache: false,
            data: { targetID: itemID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#' + item + '_' + itemID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    if ($('#' + item + '_' + itemID).parent().children().length == 1) {
                        $('#ulDestinationDescriptions').append('<li id="liNoDescriptions">No Descriptions Added</li>');
                    }
                    oTable.fnDeleteRow($('#' + item + '_' + itemID)[0]);
                    //personalized only for tr and li
                    if (tag == 'tr') {
                        exchangeRatesResultsTable
                        UI.tablesHoverEffect();
                        UI.tablesStripedEffect();
                    }
                    else {
                        UI.ulsHoverEffect();
                    }
                    if ($(document).find('.' + item + '-dependent-list').length > 0) {
                        UI.updateDependentLists('/Catalogs/GetDDLData', item);
                        //CATALOG.updateDependentLists(item);
                    }
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    ////Sales Rooms
    var SalesRoomPartiesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSalesRoomsPartiesResult', tableColumns - 1);
        CATALOG.oSalesRoomsPartiesTable = $('#tblSalesRoomsPartiesResult').dataTable();

        //$('#tblSalesRoomsPartiesResult').unbind('change').on('change', function () {
        //    CATALOG.makeSalesRoomPartialSelectable();
        //});
    }

    var makeSalesRoomPartialSelectable = function () {
        CATALOG.oSalesRoomsPartiesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oSalesRoomsPartiesTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var SalesRoomID = $(this).attr('id')
                    $.ajax(
                        {
                            url: '/Catalogs/GetSalesRoomParties',
                            cache: false,
                            type: 'POST',
                            data: { PartiesInfo_SalesPartyID: SalesRoomID },
                            success: function (data) {
                                $('#PartiesInfo_SalesPartyID').val(data.SalesRoomsPartiesID);
                                $('#PartiesInfo_FromDate').val(data.Date);
                                $('#PartiesInfo_Allotment').val(data.Allotment);
                                $('#PartiesInfo_PlaceID option[value="' + data.Place + '"]').attr('selected', true).trigger('change');
                                $('#PartiesInfo_Room').on('change', function () {
                                    if (data.SalesRoom != undefined) {
                                        $('#PartiesInfo_Room').off('change');
                                        $('#PartiesInfo_Room option[value="' + data.SalesRoom + '"]').attr('selected', true);
                                    }
                                    else {
                                        $('#PartiesInfo_Room').off('change');
                                        $('#PartiesInfo_Room option[value=' + "0" + ']').text();
                                    }
                                });
                                $('#PartiesInfo_Terminal option[value="' + data.Terminal + '"]').attr('selected', true).trigger('change');
                                $('#PartiesInfo_ProgramName').on('change', function () {
                                    $('#PartiesInfo_ProgramName').off('change');
                                    $('#PartiesInfo_ProgramName option[value="' + data.Program + '"]').attr('selected', true);
                                });

                                $('#PartiesInfo_DateSaved').val(data.DateSaved),
                                    $('#PartiesInfo_SavedByUser').val(data.SavedByUser)
                                UI.expandFieldset('fdsSalesRoomsManagement');
                                UI.scrollTo('fdsSalesRoomsManagement', null);
                            }
                        });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteSalesRoomParties, [$(this).attr('id')]);
            }
        });
    }

    function deleteSalesRoomParties(salesRoomPartieID) {
        $.ajax(
            {
                url: '/Catalogs/DeleteSalesRoomParties',
                cache: false,
                type: 'POST',
                data: { TargetID: salesRoomPartieID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    CATALOG.oSalesRoomsPartiesTable.fnDeleteRow($('#' + salesRoomPartieID)[0]);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
    }

    var saveSalesRoomsPartiesSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            CATALOG.oSalesRoomsPartiesTable = $.fn.DataTable.fnIsDataTable('tblSalesRoomsPartiesResult') ? $('#tblSalesRoomsPartiesResult') : $('#tblSalesRoomsPartiesResult').dataTable();
            if (data.ResponseMessage == 'Sales Room Saved') {
                var oSettings = CATALOG.oSalesRoomsPartiesTable.fnSettings();
                var iAdded = CATALOG.oSalesRoomsPartiesTable.fnAddData([
                    $('#PartiesInfo_FromDate').val(),
                    $('#PartiesInfo_PlaceID option:selected').text(),
                    $('#PartiesInfo_Room option:selected').text() == "--Any--" ? " " : $('#PartiesInfo_Room option:selected').text(),
                    $('#PartiesInfo_ProgramName option:selected').text(),
                    $('#PartiesInfo_Terminal option:selected').text(),
                    $('#PartiesInfo_Allotment').val(),
                    $('#PartiesInfo_DateSaved').val(),
                    $('#PartiesInfo_SavedByUser').val()
                ]);
                $('#tblSalesRoomsPartiesResult tr').attr("id", data.ItemID),
                    $('#PartiesInfo_Room option[value=' + "0" + ']').attr('selected', true);
                $('#PartiesInfo_PlaceID option[value="' + "0" + '"]').attr('selected', true);
                $('#PartiesInfo_ProgramName option[value="' + "0" + '"]').attr('selected', true);
                $('#frmSalesRoomPartiesInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeSalesRoomPartialSelectable();
            }
            else {
                $('#' + data.ItemID).children('tr').text($('#PartiesInfo_SalesPartyID').val());
                $('#' + data.ItemID).children('td:nth-child(1)').text($('#PartiesInfo_FromDate').val());
                $('#' + data.ItemID).children('td:nth-child(2)').text($('#PartiesInfo_PlaceID option:selected').text());
                $('#' + data.ItemID).children('td:nth-child(3)').text($('#PartiesInfo_Room option:selected').text() == "--Any--" ? " " : $('#PartiesInfo_Room option:selected').text());
                $('#' + data.ItemID).children('td:nth-child(4)').text($('#PartiesInfo_ProgramName option:selected').text());
                $('#' + data.ItemID).children('td:nth-child(5)').text($('#PartiesInfo_Terminal option:selected').text());
                $('#' + data.ItemID).children('td:nth-child(6)').text($('#PartiesInfo_Allotment').val());
                $('#' + data.ItemID).children('td:nth-child(7)').text($('#PartiesInfo_DateSaved').val());
                $('#' + data.ItemID).children('td:nth-child(8)').text($('#PartiesInfo_SavedByUser').val());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveSalesRoomsPartiesDuplicateSuccess = function () {
        var fromDate = $('#DuplicateFromDate').val();
        var toDate = $('#DuplicateToDate').val();
        var TerminalID = $('#TerminalID option:selected').val();
        var ItemID = "";
        if ($('#DuplicateToDate').text() == "" && $('#DuplicateFromDate').text() == "" && $('#TerminalID').val() == "0") {
            $('#DuplicateToDate').val() == "" ? $('#DuplicateToDate').addClass('input-validation-error') : $('#DuplicateToDate').text();
            $('#DuplicateFromDate').val() == "" ? $('#DuplicateFromDate').addClass('input-validation-error') : $('#DuplicateFromDate').text();
            $('#TerminalID option:selected').val() == "0" ? $('#TerminalID').addClass('input-validation-error') : $('#TerminalID option:selected').val();
            UI.messageBox(-1, "There were some validation errors:", 7);
        }
        else {
            DuplicateRoomParties(fromDate, toDate, TerminalID);
        }
    }

    var DuplicateRoomParties = function saveSalesRoomsPartiesDuplicate(TakeFromDate, CopyToDate, TerminalID) {
        $.ajax({
            url: '/Catalogs/DuplicateSalesRoomParties',
            cache: false,
            type: 'POST',
            data: { TakeFromDate: TakeFromDate, CopyToDate: CopyToDate, TerminalID: TerminalID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                $('#SearchDate_FromDate').val(CopyToDate);
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                if (data.ResponseType > 0) {
                    // setTimeout(function () {
                    $('#btnSearchSalesParties').click();
                }
            }
        });
    }

    //Banks
    var BanksPartiesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblBanksSearchResult', tableColumns - 1);
        CATALOG.oBanksPartiesTable = $('#tblBanksSearchResult').dataTable();
    }

    var makeBanksPartialSelectable = function () {
        CATALOG.oBanksPartiesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oBanksPartiesTable, $(this));
                    var BankID = $(this).attr('id')
                    $.ajax(
                        {
                            url: '/Catalogs/GetBankFromModel',
                            cache: false,
                            type: 'POST',
                            data: { BankID: BankID },
                            success: function (data) {
                                $('#NewBankID').val(data.BankID);
                                $('#NewBankName').val(data.Bank);
                                $('#NewCveSat').val(data.CveSat);
                                $('#NewTerminalID option[value="' + data.TerminalID + '"]').attr('selected', true);
                                $('#NewFromDate').val(data.FromDate),
                                    $('#NewToDate').val(data.toDate)
                                UI.expandFieldset('fdsBankManagement');
                                UI.scrollTo('fdsBankManagement', null);
                            }
                        });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteBank, [$(this).attr('id')]);
            }
        });
    }

    function deleteBank(BankID) {
        var x = BankID;
        $.ajax(
            {
                url: '/Catalogs/DeleteBank',
                cache: false,
                type: 'POST',
                data: { BankID: BankID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    CATALOG.oBanksPartiesTable.fnDeleteRow($('#' + BankID)[0]);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
    }

    var saveBanksPartiesSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            CATALOG.oBanksPartiesTable = $.fn.DataTable.fnIsDataTable('tblBanksSearchResult') ? $('#tblBanksSearchResult') : $('#tblBanksSearchResult').dataTable();
            if (data.ResponseMessage == 'Bank save success') {
                var oSettings = CATALOG.oBanksPartiesTable.fnSettings();
                var iAdded = CATALOG.oBanksPartiesTable.fnAddData([
                    $('#NewBankName').val(),
                    $('#NewCveSat').val(),
                    $('#NewFromDate').val(),
                    $('#NewToDate').val(),
                    $('#NewTerminalID option:selected').text()
                ]);
                $('#tblBanksSearchResult tr').attr("id", data.ItemID),
                    $('#NewTerminalID option[value=' + "0" + ']').attr('selected', true);
                $('#frmBanksPartiesInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makeBanksPartialSelectable();
            }
            else {
                $('#' + data.ItemID).children('tr').text($('#NewBankID').val());
                $('#' + data.ItemID).children('td:nth-child(1)').text($('#NewBankName').val());
                $('#' + data.ItemID).children('td:nth-child(2)').text($('#NewCveSat').val());
                $('#' + data.ItemID).children('td:nth-child(3)').text($('#NewFromDate').val());
                $('#' + data.ItemID).children('td:nth-child(4)').text($('#NewToDate').val());
                $('#' + data.ItemID).children('td:nth-child(5)').text($('#NewTerminalID option:selected').text());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //Boats
    var saveBoatsPartiesSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            CATALOG.oBoatsPartiesTable = $.fn.DataTable.fnIsDataTable('tblBoatSearchResult') ? $('#tblBoatSearchResult') : $('#tblBoatSearchResult').dataTable();
            if (data.ResponseMessage == 'Boat save success') {
                var oSettings = CATALOG.oBoatsPartiesTable.fnSettings();
                var iAdded = CATALOG.oBoatsPartiesTable.fnAddData([
                    $('#Boat').val(),
                    $('#Qouta').val(),
                    $('#Shortname').val(),
                    $('#ProvidersID option:selected').text(),
                    $('#TerminalID option:selected').text()
                ]);
                $('#tblBoatSearchResult tr').attr("id", data.ItemID),
                    $('#ProvidersID option[value=' + "0" + ']').attr('selected', true);
                $('#TerminalID option[value=' + "0" + ']').attr('selected', true);
                $('#frmBoatsPartiesInfo').clearForm();

                UI.tablesHoverEffect();
                CATALOG.makeBoatsPartialSelectable();
            }
            else {
                $('#' + data.ItemID).children('tr').text($('#BoatID').val());
                $('#' + data.ItemID).children('td:nth-child(1)').text($('#Boat').val());
                $('#' + data.ItemID).children('td:nth-child(2)').text($('#Qouta').val());
                $('#' + data.ItemID).children('td:nth-child(3)').text($('#Shortname').val());
                $('#' + data.ItemID).children('td:nth-child(4)').text($('#ProvidersID option:selected').text());
                $('#' + data.ItemID).children('td:nth-child(5)').text($('#TerminalID option:selected').text());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var BoatsPartiesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblBoatSearchResult', tableColumns - 1);
        CATALOG.oBoatsPartiesTable = $('#tblBoatSearchResult').dataTable();
    }

    var makeBoatsPartialSelectable = function () {
        CATALOG.oBoatsPartiesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oBoatsPartiesTable, $(this));
                    var BoatID = $(this).attr('id')
                    $.ajax(
                        {
                            url: '/Catalogs/GetBoatFromModel',
                            cache: false,
                            type: 'POST',
                            data: { BoatID: BoatID },
                            success: function (data) {
                                $('#BoatID').val(data.BoatID);
                                $('#Boat').val(data.Boat);
                                $('#Qouta').val(data.Qouta);
                                $('#TerminalID option[value="' + data.Terminals + '"]').attr('selected', true);
                                $('#Shortname').val(data.Alias),
                                    $('#ProvidersID option[value="' + data.Providers + '"]').attr('selected', true);
                                UI.expandFieldset('fdsBoatManagement');
                                UI.scrollTo('fdsBoatManagement', null);
                            }
                        });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteBoat, [$(this).attr('id')]);
            }
        });
    }

    function deleteBoat(BoatID) {
        var x = BoatID;
        $.ajax(
            {
                url: '/Catalogs/DeleteBoat',
                cache: false,
                type: 'POST',
                data: { BoatID: BoatID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    CATALOG.oBoatsPartiesTable.fnDeleteRow($('#' + BoatID)[0]);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
    }

    //Sales Channels

    var SalesChannelsPartiesResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSalesChannelsResult', tableColumns - 1);
        CATALOG.oSalesChannelsPartiesTable = $('#tblSalesChannelsResult').dataTable();
    }

    var makeSalesChannelPartialSelectable = function () {
        CATALOG.oSalesChannelsPartiesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oSalesChannelsPartiesTable, $(this));
                    var SalesChannelID = $(this).attr('id')
                    $.ajax(
                        {
                            url: '/Catalogs/GetSalesChannels',
                            cache: false,
                            type: 'POST',
                            data: { SalesChannelID: SalesChannelID },
                            success: function (data) {
                                $('#SalesChannnelID').val(data.SalesChannelID);
                                $('#SalesChannel').val(data.SaleChannel);
                                $('#SavedByUser').val(data.UserId);
                                $('#DateSaved').val(data.DateSaved);
                                $('#TerminalID option[value="' + data.Terminal + '"]').attr('selected', true);
                                UI.expandFieldset('fdsSalesChannelsManagement');
                                UI.scrollTo('fdsSalesChannelsManagement', null);
                            }
                        });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteChannels, [$(this).attr('id')]);
            }
        });
    }

    function deleteChannels(SalesChannelsID) {
        var x = SalesChannelsID;
        $.ajax(
            {
                url: '/Catalogs/DeleteSalesChannels',
                cache: false,
                type: 'POST',
                data: { SalesChannelsID: SalesChannelsID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    CATALOG.oSalesChannelsPartiesTable.fnDeleteRow($('#' + SalesChannelsID)[0]);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
    }

    var saveSalesChannelsPartiesSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            CATALOG.oSalesChannelsPartiesTable = $.fn.DataTable.fnIsDataTable('tblSalesChannelsResult') ? $('#tblSalesChannelsResult') : $('#tblSalesChannelsResult').dataTable();
            if (data.ResponseMessage == 'Sale Channel save success') {
                var oSettings = CATALOG.oSalesChannelsPartiesTable.fnSettings();
                var iAdded = CATALOG.oSalesChannelsPartiesTable.fnAddData([
                    $('#SalesChannel').val(),
                    $('#SavedByUser').val(),
                    $('#DateSaved').val(),
                    $('#TerminalID option:selected').text()
                ]);
                $('#tblSalesChannelsResult tr').attr("id", data.ItemID),
                    $('#TerminalID option[value=' + "0" + ']').attr('selected', true);
                $('#frmSalesChannelsPartiesInfo').clearForm();

                UI.tablesHoverEffect();
                CATALOG.makeSalesChannelPartialSelectable();
            }
            else {
                $('#' + data.ItemID).children('tr').text($('#SalesChannnelID').val());
                $('#' + data.ItemID).children('td:nth-child(1)').text($('#SalesChannel').val());
                $('#' + data.itemID).children('td:nth-child(2)').text($('#SavedByUser').val());
                $('#' + data.itemID).children('td:nth-child(3)').text($('#DateSaved').val());
                $('#' + data.ItemID).children('td:nth-child(4)').text($('#TerminalID option:selected').text());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //Bracelets
    var BraceletsPartiesResultsTable = function (data) {
        var tableColums = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblBraceletsSearchResult', tableColums - 1);
        CATALOG.oBraceletsPartiesTable = $('#tblBraceletsSearchResult').dataTable();
    }

    var makeBraceletsPartialSelectable = function () {
        CATALOG.oBraceletsPartiesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oBraceletsPartiesTable, $(this));
                    var BraceletID = $(this).attr('id')
                    $.ajax(
                        {
                            url: '/Catalogs/GetBracelets',
                            cache: false,
                            type: 'POST',
                            data: { BraceletID: BraceletID },
                            success: function (data) {
                                $('#BraceletID').val(data.GetBraceletID);
                                $('#Bracelet').val(data.GetBracelet);
                                $('#TerminalID option[value="' + data.GetTerminalID + '"]').attr('selected', true);
                                UI.expandFieldset('fdsBraceletManagement');
                                UI.scrollTo('fdsBraceletManagement', null);
                            }
                        });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteBracelets, [$(this).attr('id')]);
            }
        });
    }

    function deleteBracelets(BraceletID) {
        var x = BraceletID;
        $.ajax(
            {
                url: '/Catalogs/DeleteBracelets',
                cache: false,
                type: 'POST',
                data: { BraceletID: BraceletID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    CATALOG.oBraceletsPartiesTable.fnDeleteRow($('#' + BraceletID)[0]);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
    }

    var saveBraceletsPartiesSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            CATALOG.oBraceletsPartiesTable = $.fn.DataTable.fnIsDataTable('tblBraceletsSearchResult') ? $('#tblBraceletsSearchResult') : $('#tblBraceletsSearchResult').dataTable();
            if (data.ResponseMessage == 'Bracelet Saved') {
                var oSettings = CATALOG.oBraceletsPartiesTable.fnSettings();
                var iAdded = CATALOG.oBraceletsPartiesTable.fnAddData([
                    $('#Bracelet').val(),
                    $('#TerminalID option:selected').text()
                ]);
                $('#tblBraceletsSearchResult tr').attr("id", data.ItemID),
                    $('#TerminalID option[value=' + "0" + ']').attr('selected', true);
                $('#frmBraceletsPartiesInfo').clearForm();

                UI.tablesHoverEffect();
                CATALOG.makeBraceletsPartialSelectable();
            }
            else {
                $('#' + data.ItemID).children('tr').text($('#BraceletID').val());
                $('#' + data.ItemID).children('td:nth-child(1)').text($('#Bracelet').val());
                $('#' + data.ItemID).children('td:nth-child(2)').text($('#TerminalID option:selected').text());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //Reminders
    var RemindersPartiesResultsTable = function (data) {
        var tableColums = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblRemindersSearchResult', tableColums - 1);
        CATALOG.oRemindersPartiesTable = $('#tblRemindersSearchResult').dataTable();
    }

    var makeRemindersPartialSelectable = function () {
        CATALOG.oRemindersPartiesTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oRemindersPartiesTable, $(this));
                    var ReminderID = $(this).attr('id')
                    $.ajax(
                        {
                            url: '/Catalogs/GetReminders',
                            cache: false,
                            type: 'POST',
                            data: { ReminderID: ReminderID },
                            success: function (data) {
                                $('#ReminderID').val(data.GetReminderID);
                                $('#ServiceID option[value="' + data.GetServiceID + '"]').attr('selected', true);
                                $('#FromDate').val(data.GetFromDate);
                                $('#ToDate').val(data.GetToDate);
                                $('#Message').val(data.GetMessage);
                                $('#TerminalID option[value="' + data.GetTerminalID + '"]').attr('selected', true);
                                UI.expandFieldset('fdsReminderManagement');
                                UI.scrollTo('fdsReminderManagement', null);
                            }
                        });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteReminders, [$(this).attr('id')]);
            }
        });
    }

    function deleteReminders(ReminderID) {
        var x = ReminderID;
        $.ajax(
            {
                url: '/Catalogs/DeleteReminders',
                cache: false,
                type: 'POST',
                data: { ReminderID: ReminderID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    CATALOG.oRemindersPartiesTable.fnDeleteRow($('#' + ReminderID)[0]);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
    }

    var saveRemindersPartiesSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            CATALOG.oRemindersPartiesTable = $.fn.DataTable.fnIsDataTable('tblRemindersSearchResult') ? $('#tblRemindersSearchResult') : $('#tblRemindersSearchResult').dataTable();
            if (data.ResponseMessage == 'Reminder Saved') {
                var oSettings = CATALOG.oRemindersPartiesTable.fnSettings();
                var iAdded = CATALOG.oRemindersPartiesTable.fnAddData([
                    $('#ServiceID option:selected').text(),
                    $('#FromDate').val(),
                    $('#ToDate').val(),
                    $('#Message').val(),
                    $('#TerminalID option:selected').text()
                ]);
                $('#tblRemindersSearchResult tr').attr("id", data.ItemID),
                    $('#TerminalID option[value=' + "0" + ']').attr('selected', true);
                $('#ServiceID option[value=' + "0" + ']').attr('selected', true);
                $('#frmRemindersPartiesInfo').clearForm();

                UI.tablesHoverEffect();
                CATALOG.makeRemindersPartialSelectable();
            }
            else {
                $('#' + data.ItemID).children('tr').text($('#ReminderID').val());
                $('#' + data.ItemID).children('td:nth-child(1)').text($('#ServiceID option:selected').text());
                $('#' + data.ItemID).children('td:nth-child(2)').text($('#FromDate').val());
                $('#' + data.ItemID).children('td:nth-child(3)').text($('#ToDate').val());
                $('#' + data.ItemID).children('td:nth-child(4)').text($('#Message').val());
                $('#' + data.ItemID).children('td:nth-child(5)').text($('#TerminalID option:selected').text());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //Promo
    var promosResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchPromosResults', tableColumns - 1);
        CATALOG.oPromoTable = $('#tblSearchPromosResults').dataTable();

        //$('#tblSearchPromosResults').unbind('change').on('change', function () {
        //    CATALOG.makePromosSelectable();
        //});
    }

    var makePromosSelectable = function () {
        CATALOG.oPromoTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    UI.unselectPrimaryByEsc(CATALOG.oPromoTable, $(this));
                    //$(document).find('.selected-row').each(function () {
                    //    var event = $.Event('keydown');
                    //    event.keyCode = 27;
                    //    $(document).trigger(event);
                    //});
                    //$(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    //$(this).addClass('selected-row secondary');
                    var PromoID = $(this).attr('id')//.substr(5);//0
                    $.ajax({
                        url: '/Catalogs/GetPromo',
                        cache: false,
                        type: 'POST',
                        data: { PromoInfo_PromoID: PromoID },
                        success: function (data) {
                            var builder = '';
                            $('#PromoInfo_PromoID').val(data.GetPromoID);
                            $('#PromoInfo_Promo').val(data.GetPromoName);
                            $('#PromoInfo_FromDateBW').val(data.GetPromoFromDateBW);
                            $('#PromoInfo_ToDateBW').val(data.GetPromoToDateBW);
                            $('#PromoInfo_FromDateTW').val(data.GetPromoFromDateTW);
                            $('#PromoInfo_ToDateTW').val(data.GetPromoToDateTW);
                            $('#PromoInfo_Amount').val(data.GetPromoAmount);
                            $('#PromoInfo_Percentage').val(data.GetPromoPercenage);
                            $('#PromoInfo_PromoCode').val(data.GetPromoCode);
                            //single list
                            $('#PromoInfo_Terminal option[value="' + data.GetPromoTerminal + '"]').attr('selected', true);
                            $('#PromoInfo_Currency option[value="' + data.GetPromoCurrency + '"]').attr('selected', true);
                            //radios
                            if (data.GetPromoAnyPrice) {
                                $('input:radio[name=PromoInfo_AnyPrice]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PromoInfo_AnyPrice]')[1].checked = true;
                            }
                            if (data.GetPromoWholeStay) {
                                $('input:radio[name=PromoInfo_WholeStay]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PromoInfo_WholeStay]')[1].checked = true;
                            }
                            if (data.GetPromoPerNight) {
                                $('input:radio[name=PromoInfo_PerNight]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PromoInfo_PerNight]')[1].checked = true;
                            }
                            if (data.GetPromoCombinable) {
                                $('input:radio[name=PromoInfo_Combinable]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PromoInfo_Combinable]')[1].checked = true;
                            }
                            if (data.GetPromoApplyPerson) {
                                $('input:radio[name=PromoInfo_applyOnPerson]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PromoInfo_applyOnPerson]')[1].checked = true;
                            }
                            if (data.GetPromoPublish) {
                                $('input:radio[name=PromoInfo_Publish]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PromoInfo_Publish]')[1].checked = true;
                            }

                            if (data.GetPromoMonday) {
                                $('#PromoInfo_Monday').prop('checked', true);
                            }
                            if (data.GetPromoTuesday) {
                                $('#PromoInfo_Tuesday').prop('checked', true);
                            }
                            if (data.GetPromoWednesday) {
                                $('#PromoInfo_Wednesday').prop('checked', true);
                            }
                            if (data.GetPromoThursday) {
                                $('#PromoInfo_Thursday').prop('checked', true);
                            }
                            if (data.GetPromoFriday) {
                                $('#PromoInfo_Friday').prop('checked', true);
                            }
                            if (data.GetPromoSaturday) {
                                $('#PromoInfo_Saturday').prop('checked', true);
                            }
                            if (data.GetPromoSunday) {
                                $('#PromoInfo_Sunday').prop('checked', true);
                            }
                            if (data.GetPromoPackage) {
                                $('input:radio[name=PromoInfo_isPackage]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PromoInfo_isPackage]')[1].checked = true;
                            }
                            //multplelist
                            $.each(data.GetPromoType, function (index, item) {
                                $('#PromoInfo_Type').find('option[value="' + item.PromoTypeID + '"]').attr('selected', true);
                            });
                            //Publish
                            if (data.GetPromoPublish) {
                                $('#PromoDescription').slideDown('fast');
                                $.each(data.GetPromoDescription, function (index, items) {//iterar en los valores PromotionDescription
                                    var values = items.split('|');
                                    builder += '<tr id="' + values[0] + '">' +//id
                                        + '<td>' + values[1] + '</td>'//mainTag
                                        + '<td>' + values[2] + '</td>'//culture
                                        + '<td>' + values[3] + '</td>'//title
                                        + '<td>' + values[4] + '</td>'//description
                                        + '<td>' + values[6] + '</td>'//instructions
                                        + '<td><div style="padding : 3px; width : 200px; height : 80px; overflow : auto;">'
                                        + values[5] + '</div><br><img id="tdsWG" style="float:right" class="delete-row" src="/Content/themes/base/images/trash.png"/></td>'
                                });
                                $('#tblPromoDescription tbody').append(builder);
                            }
                            //Description
                            $('select[multiple="multiple"]').multiselect('refresh');
                            UI.expandFieldset('fdsPromosInfo');
                            UI.scrollTo('fdsPromosInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deletePromo, [$(this).attr('id')]);
            }
        });
    }

    function deletePromo(PromoInfo_PromoID) {
        $.ajax(
            {
                url: '/Catalogs/DeletePromo',
                cache: false,
                type: 'POST',
                data: { TargetID: PromoInfo_PromoID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    CATALOG.oPromoTable.fnDeleteRow($('#' + PromoInfo_PromoID)[0]);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
    }

    var savePromoSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            CATALOG.oPromoTable = $.fn.DataTable.fnIsDataTable('#tblSearchPromosResults') ? $('#tblSearchPromosResults') : $('#tblSearchPromosResults').dataTable();
            if (data.ResponseMessage == 'Promotion Saved') {
                var oSettings = CATALOG.oPromoTable.fnSettings();
                var iAdded = CATALOG.oPromoTable.fnAddData([
                    $('#PromoInfo_PromoCode').val(),
                    $('#PromoInfo_Promo').val(),
                    $('#PromoInfo_FromDateBW').val() + ' - ' + $('#PromoInfo_ToDateBW').val(),
                    $('#PromoInfo_FromDateTW').val() + ' - ' + $('#PromoInfo_ToDateTW').val(),
                    $('input:radio[name=PromoInfo_Publish]:checked').val()
                ]);
                $('#PromoInfo_Terminal option[value="' + "0" + '"]').attr('selected', true);
                $('#PromoInfo_Currency option[value="' + "0" + '"]').attr('selected', true);
                //$('#PromoInfo_Type option[value="' + "0" + '"]').attr('selected', true);
                $('#frmPromoInfo').clearForm();
                UI.tablesHoverEffect();
                CATALOG.makePromosSelectable();
            }
            else {
                $('#' + data.ItemID).children('td:nth-child(1)').text($('#PromoInfo_PromoCode').val());
                $('#' + data.ItemID).children('td:nth-child(2)').text($('#PromoInfo_Promo').val());
                $('#' + data.ItemID).children('td:nth-child(3)').text($('#PromoInfo_ToDateBW').val() + ' - ' + $('#PromoInfo_FromDateBW').val());
                $('#' + data.ItemID).children('td:nth-child(4)').text($('#PromoInfo_ToDateTW').val() + ' - ' + $('#PromoInfo_FromDateTW').val());
                $('#' + data.ItemID).children('td:nth-child(5)').text($('input:radio[name=PromoInfo_Publish]:checked').val());
                // $('#' + data.ItemID).children('td:nth-child(7)').text();
                // $('#' + data.ItemID).children('td:nth-child(8)').text($('#PromoInfo_ToDateTW').val());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var makePublishSelectable = function (data) {
        $('#tbltblPromoDescription tbody tr').not('theader').unbind('click').on('click', function () {
            $('#PromoInfo_DescriptionTag').val();
            $(this).parent('tbody').find('.editing-row').removeClass('editing-row selected-row');
            $(this).addClass('editing-row selected-row');
        });
    }
    var deleteAuxTablesRowsPromos = function () {
        $('.delete-row').unbind('click').on('click', function () {
            $(this).parents('tr').first().remove();
            UI.tablesStripedEffect();
        });
    }

    function validateInsertionAttemptPromos(table, Tag, Culture) {
        var result = '';
        if ($('#' + tag).val() == 0)
            result = "0";
        if ($('#' + Culture + 'option:selected').val() == 0)
            result = "0";
        else {
            var n = 0;
            $('#' + table + ' tbody tr').not('theader').each(function (e) {
                var cellText = $(this).children('td:nth-child(1)')[0].textContent;//tag
                var secondCellText = culture != undefined ? $(this).children('td:nth-child(2)')[0].textContent : '';
                if ($('#' + tag).text() == cellText) {
                    if (select2 != undefined) {
                        if ($('#' + culture + ' option:selected').text() == secondCellText)
                            result = "-1";
                        else
                            n++;
                    }
                    else
                        result = "cosa-1";
                }
                else
                    n++;
            });
            if (n == $('#' + table + ' tbody tr').not('theader').length && result == '')
                result = "1";
        }
        return result;
    }


    //var updateDependentLists = function (itemType) {
    //    $.getJSON('/Catalogs/GetDDLData', { itemType: itemType }, function (data) {
    //        $(document).find('.' + itemType + '-dependent-list').each(function () {
    //            $(this).fillSelect(data);
    //            if ($(this).attr('multiple') == undefined && $(this).find('option[value="0"]').length == 0) {
    //                $(this).prepend('<option value="0" selected="selected">--Select One--</option>');
    //            }
    //        });
    //    });
    //}

    //var Budgets = function () {
    //    var budgetLoaded = function () {
    //        UI.tablesStripedEffect();
    //        $('#tblBudgets tbody tr').off('click').on('click', function () {
    //            var id = $(this).attr('id');
    //            $('.budget-detail').slideUp('fast');
    //            $('div[data-id="' + id + '"]').slideDown('fast');

    //            $('#tblBudgets tbody tr').removeClass('selected-row primary');
    //            $(this).addClass('selected-row primary');
    //        });
    //        REPORT.addExtras();
    //    }
    //    return {
    //        budgetLoaded: budgetLoaded,
    //    }
    //}();


    //var ShowProviderExchangeRateCatalog = function () {
    //    var ShowProvider = function () {
    //        UI.tablesStripedEffect();
    //        $('input:radio[name="SearchExchangeRates_OptionProvider"]').on('change', function () {
    //            if ($(this).is(':checked') && $(this).val() == true) {
    //                $('#provider').slideDown('fast');
    //            }
    //                $('#provider').slideUp('fast');
    //        });
    //    }
    //    return {
    //        ShowProvider: ShowProvider,
    //    }
    //}();


    return {
        init: init,
        zonesResultsTable: zonesResultsTable,
        makeZonesSelectable: makeZonesSelectable,
        saveZoneSuccess: saveZoneSuccess,
        accountingAccountsResultsTable: accountingAccountsResultsTable,
        makeAccountingAccountsSelectable: makeAccountingAccountsSelectable,
        saveAccountingAccountSuccess: saveAccountingAccountSuccess,
        commissionsResultsTable: commissionsResultsTable,
        makeCommissionsSelectable: makeCommissionsSelectable,
        saveCommissionSuccess: saveCommissionSuccess,
        companiesResultsTable: companiesResultsTable,
        makeCompaniesSelectable: makeCompaniesSelectable,
        saveCompanySuccess: saveCompanySuccess,
        couponFoliosResultsTable: couponFoliosResultsTable,
        makeCouponFoliosSelectable: makeCouponFoliosSelectable,
        saveCouponFolioSuccess: saveCouponFolioSuccess,
        exchangeRatesResultsTable: exchangeRatesResultsTable,
        makeExchangeRatesSelectable: makeExchangeRatesSelectable,
        saveExchangeRateSuccess: saveExchangeRateSuccess,
        afterRenderFunctions: afterRenderFunctions,
        locationsResultsTable: locationsResultsTable,
        makeLocationsSelectable: makeLocationsSelectable,
        saveLocationSuccess: saveLocationSuccess,
        OPCSResultsTable: OPCSResultsTable,
        makeOPCSSelectable: makeOPCSSelectable,
        saveOPCSuccess: saveOPCSuccess,
        pointsOfSaleResultsTable: pointsOfSaleResultsTable,
        makePointsOfSaleSelectable: makePointsOfSaleSelectable,
        savePointOfSaleSuccess: savePointOfSaleSuccess,
        promotionTeamsResultsTable: promotionTeamsResultsTable,
        makePromotionTeamsSelectable: makePromotionTeamsSelectable,
        savePromotionTeamSuccess: savePromotionTeamSuccess,
        providersResultsTable: providersResultsTable,
        makeProvidersSelectable: makeProvidersSelectable,
        saveProviderSuccess: saveProviderSuccess,
        uploadFileToProvider: uploadFileToProvider,
        getFilesOfProvider: getFilesOfProvider,
        deleteFileOfProvider: deleteFileOfProvider,
        placeTypesResultsTable: placeTypesResultsTable,
        makePlaceTypesSelectable: makePlaceTypesSelectable,
        savePlaceTypeSuccess: savePlaceTypeSuccess,
        deleteItemFunction: deleteItemFunction,
        transportationZonesResultsTable: transportationZonesResultsTable,
        makeTransportationZonesSelectable: makeTransportationZonesSelectable,
        saveTransportationZoneSuccess: saveTransportationZoneSuccess,
        usersLeadSourcesResultsTable: usersLeadSourcesResultsTable,
        makeTblUserLeadSourcesSelectable: makeTblUserLeadSourcesSelectable,
        saveUserLeadSourceSuccess: saveUserLeadSourceSuccess,
        placeClasificationsResultsTable: placeClasificationsResultsTable,
        makePlaceClasificationsSelectable: makePlaceClasificationsSelectable,
        savePlaceClasificationSuccess: savePlaceClasificationSuccess,
        destinationsResultsTable: destinationsResultsTable,
        makeDestinationsSelectable: makeDestinationsSelectable,
        saveDestinationSuccess: saveDestinationSuccess,
        updateUlDestinationDescriptions: updateUlDestinationDescriptions,
        ulDestinationDescriptionsSelection: ulDestinationDescriptionsSelection,
        saveDestinationDescriptionSuccess: saveDestinationDescriptionSuccess,
        marketCodesResultsTable: marketCodesResultsTable,
        makeTblMarketCodesSelectable: makeTblMarketCodesSelectable,
        saveMarketCodeSuccess: saveMarketCodeSuccess,
        bindToCheckBoxes: bindToCheckBoxes,
        //updateDependentLists: updateDependentLists,
        refreshActiveSearchResults: refreshActiveSearchResults,
        budgetsResultsTable: budgetsResultsTable,
        makeBudgetsSelectable: makeBudgetsSelectable,
        saveBudgetSuccess: saveBudgetSuccess,
        //
        SalesRoomPartiesResultsTable: SalesRoomPartiesResultsTable,
        makeSalesRoomPartialSelectable: makeSalesRoomPartialSelectable,
        saveSalesRoomsPartiesSuccess: saveSalesRoomsPartiesSuccess,
        DuplicateRoomParties: DuplicateRoomParties,
        //
        promosResultsTable: promosResultsTable,
        makePromosSelectable: makePromosSelectable,
        savePromoSuccess: savePromoSuccess,
        deleteAuxTablesRowsPromos: deleteAuxTablesRowsPromos,
        optionsResultsTable: optionsResultsTable,
        makeOptionsSelectable: makeOptionsSelectable,
        saveOptionSuccess: saveOptionSuccess,
        //Banks
        BanksPartiesResultsTable: BanksPartiesResultsTable,
        makeBanksPartialSelectable: makeBanksPartialSelectable,
        saveBanksPartiesSuccess: saveBanksPartiesSuccess,
        ////Boats
        saveBoatsPartiesSuccess: saveBoatsPartiesSuccess,
        BoatsPartiesResultsTable: BoatsPartiesResultsTable,
        makeBoatsPartialSelectable: makeBoatsPartialSelectable,
        //SalesChannels
        SalesChannelsPartiesResultsTable: SalesChannelsPartiesResultsTable,
        makeSalesChannelPartialSelectable: makeSalesChannelPartialSelectable,
        saveSalesChannelsPartiesSuccess: saveSalesChannelsPartiesSuccess,
        //Bracelets
        BraceletsPartiesResultsTable: BraceletsPartiesResultsTable,
        makeBraceletsPartialSelectable: makeBraceletsPartialSelectable,
        saveBraceletsPartiesSuccess: saveBraceletsPartiesSuccess,
        //Reminders
        RemindersPartiesResultsTable: RemindersPartiesResultsTable,
        makeRemindersPartialSelectable: makeRemindersPartialSelectable,
        saveRemindersPartiesSuccess: saveRemindersPartiesSuccess

    }
}();