$.extend(jQuery.fn.dataTableExt.oSort, {
    "datetime-us-pre": function (a) {
        a = a != 'Permanent' ? a : '';
        if (a != '') {
            //var b = a.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2}) (am|pm|AM|PM|Am|Pm|a.m.|p.m.)/),
            var b = a.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2})/),
                year = b[1],
                month = b[2],
                day = b[3];
            //hour = b[4],
            //min = b[5],
            //ap = b[7];

            //if (hour == '12') hour = '0';
            //if (ap == 'p.m.') hour = parseInt(hour, 10) + 12;

            if (year.length == 2) {
                if (parseInt(year, 10) < 70) year = '20' + year;
                else year = '19' + year;
            }
            if (month.length == 1) month = '0' + month;
            if (day.length == 1) day = '0' + day;
            //if (hour.length == 1) hour = '0' + hour;
            //if (min.length == 1) min = '0' + min;
        }
        //var tt = a != '' ? year + month + day + hour + min : 0;
        var tt = a != '' ? year + month + day : 0;
        return tt;
    },
    "datetime-us-asc": function (a, b) {
        return a - b;
    },
    "datetime-us-desc": function (a, b) {
        return b - a;
    }
});

$.fn.dataTableExt.aTypes.unshift(
   function (sData) {
       try {
           //if (sData !== null && sData.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2}) (am|pm|AM|PM|Am|Pm|a.m.|p.m.)/)) {
           if (sData !== null && sData.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2})/)) {
               return 'datetime-us';
           }
       }
       catch (ex) {
       }
       return null;
   }
);

$(function () {
    PRICE.init();
    $('#PriceInfo_FromDate').datepicker({
        dateFormat: 'yy-mm-dd',
        //timeFormat: 'hh:mm:ss TT',
        minDate: 0,
        changeMonth: true,
        changeYear: true,
        stepMinute: 5,
        onClose: function (dateText, inst) {
            if ($('#PriceInfo_ToDate').val() != "" && $('#divPriceToDate').is(':visible')) {
                var fromDate = $('#PriceInfo_FromDate').datepicker('getDate');
                var toDate = $('#PriceInfo_ToDate').datepicker('getDate');
                if (fromDate > toDate)
                    $('#PriceInfo_ToDate').datepicker('setDate', fromDate);
            }
            else if ($('#divPriceToDate').is(':visible')) {
                $('#PriceInfo_ToDate').val(dateText);
            }
            if ($('#PriceInfo_TWFromDate').val() == '') {
                $('#PriceInfo_TWFromDate').datepicker('setDate', fromDate);
            }
        }
        //},
        //onSelect: function (selectedDateTime) {
        //    if ($('#PriceInfo_ToDate').val() != '' && $('#divPriceToDate').is(':visible')) {
        //        $('#PriceInfo_ToDate').datepicker('setDate', $('#PriceInfo_ToDate').datepicker('getDate'));
        //    }
        //    $('#PriceInfo_ToDate').datepicker('option', 'minDate', $('#PriceInfo_FromDate').datepicker('getDate'));
        //}
    });

    $('#PriceInfo_ToDate').datepicker({
        dateFormat: 'yy-mm-dd',
        //timeFormat: 'hh:mm:ss TT',
        changeMonth: true,
        changeYear: true,
        minDate: 0,
        //stepMinute: 5,
        onClose: function (dateText, inst) {
            if ($('#PriceInfo_FromDate').val() != "") {
                var fromDate = $('#PriceInfo_FromDate').datepicker('getDate');
                var toDate = $('#PriceInfo_ToDate').datepicker('getDate');
                if (fromDate > toDate) {
                    $('#PriceInfo_FromDate').datepicker('setDate', toDate);
                }
            }
            else {
                $('#PriceInfo_FromDate').val(dateText);
            }
        }
        //onSelect: function (selectedDateTime) {
        //if ($('#PriceInfo_FromDate').val() != '') {
        //    $('#PriceInfo_FromDate').datepicker('setDate', $('#PriceInfo_FromDate').datepicker('getDate'));
        //}
        //$('#PriceInfo_FromDate').datepicker('option', 'maxDate', $('#PriceInfo_ToDate').datepicker('getDate'));
        //}
    });

    $('#PriceInfo_TWFromDate').datepicker({
        dateFormat: 'yy-mm-dd',
        minDate: 0,
        changeMonth: true,
        changeYear: true,
        onClose: function (dateText, inst) {
            if ($('#PriceInfo_TWToDate').val() != "" && $('#divTWToDate').is(':visible')) {
                var fromDate = $('#PriceInfo_TWFromDate').datepicker('getDate');
                var toDate = $('#PriceInfo_TWToDate').datepicker('getDate');
                if (fromDate > toDate)
                    $('#PriceInfo_TWToDate').datepicker('setDate', fromDate);
            }
            else if ($('#divTWToDate').is(':visible')) {
                $('#PriceInfo_TWToDate').val(dateText);
            }
        },
        onSelect: function (selectedDateTime) {
            if ($('#PriceInfo_TWToDate').val() != '' && $('#divTWToDate').is(':visible')) {
                $('#PriceInfo_TWToDate').datepicker('setDate', $('#PriceInfo_TWToDate').datepicker('getDate'));
            }
            $('#PriceInfo_TWToDate').datepicker('option', 'minDate', $('#PriceInfo_TWFromDate').datepicker('getDate'));
        }
    });

    $('#PriceInfo_TWToDate').datepicker({
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        minDate: 0,
        onClose: function (dateText, inst) {
            if ($('#PriceInfo_TWFromDate').val() != "") {
                var fromDate = $('#PriceInfo_TWFromDate').datepicker('getDate');
                var toDate = $('#PriceInfo_TWToDate').datepicker('getDate');
                if (fromDate > toDate) {
                    $('#PriceInfo_TWFromDate').datepicker('setDate', toDate);
                }
            }
            else {
                $('#PriceInfo_TWFromDate').val(dateText);
            }
        }
    });
});

var PRICE = function () {

    var oTable;

    var currentTablePage = 0;

    function PriceInfoModel() {
        this.PriceInfo_PriceID = null;
        this.PriceInfo_PriceType = null;
        this.PriceInfo_ItemType = null;
        this.PriceInfo_ItemID = null;
        this.PriceInfo_PriceClasification = null;
        this.PriceInfo_Price = null;
        this.PriceInfo_Currency = null;
        this.PriceInfo_CurrencyCode = null;
        this.PriceInfo_IsPermanent = null;
        this.PriceInfo_FromDate = null;
        this.PriceInfo_ToDate = null;
        this.PriceInfo_TWFromDate = null;
        this.PriceInfo_TWToDate = null;
        this.PriceInfo_TWPermanent = null;
        this.PriceInfo_GenericUnit = null;
        this.PriceInfo_Terminal = null;
        this.PriceInfo_TaxesIncluded = null;
        this.PriceInfo_FromTransportationZone = null;
        this.PriceInfo_ToTransportationZone = null;
        this.PriceInfo_UrlRedeem = null;
        this.PriceInfo_UrlCompare = null;
        this.PriceInfo_PriceUnits = null;
    }

    function PriceUnitInfoModel() {
        this.PriceUnitInfo_PriceUnitID = null;
        this.PriceUnitInfo_PriceID = null;
        this.PriceUnitInfo_Unit = null;
        this.PriceUnitInfo_AdditionalInfo = null;
        this.PriceUnitInfo_Culture = null;
        this.PriceUnitInfo_Min = null;
        this.PriceUnitInfo_Max = null;
    }

    var init = function () {

        $('.price-unit').hide();

        //if ($('#btnGroupPrices').val().toLowerCase() == 'group') {
        //    $('#priceTypesGroups').hide();
        //    $('#btnSavePricesGroup').hide();
        //    $('#pricesToGroupContainer').hide();
        //}
        //else {
        //    $('#priceTypesGroups').show();
        //    $('#btnSavePricesGroup').show();
        //    $('#pricesToGroupContainer').show();
        //}

        if ($('#PriceInfo_ItemType').val() == 'Transportation') {
            $('#divFromZone').show();
            $('#divToZone').show();
        }
        if ($('#PriceInfo_ItemType').val() == 'Packages') {
            $('#divUrlRedeem').show();
            $('#divUrlCompare').show();
        }
        if ($('#PriceInfo_ItemType').val() == 'Activities') {
            $('#PriceInfo_PriceClasification').attr('data-keep-value', true);
            $('#PriceInfo_PriceClasification option').filter(function () {
                return $(this).html() == 'Per Unit';
            }).attr('selected', true);
            $('#PriceInfo_PriceClasification').parents('.editor-alignment').first().hide();
            $('.price-unit').show();
            $('#PriceInfo_Currency').on('change', function () {
                //var value;
                //switch ($(this).val()) {
                //    case 'USD': {
                //        value = 'en-US';
                //        break;
                //    }
                //    case 'CAD': {
                //        value = 'en-US';
                //        break;
                //    }
                //    case 'MXN': {
                //        value = 'es-MX';
                //        break;
                //    }
                //    default: {
                //        value = '0';
                //        break;
                //    }
                //}
                //$('#PriceUnitInfo_Culture option[value="' + value + '"]').attr('selected', true);
                //$('#PriceUnitInfo_Culture_Hdn').val($('#PriceUnitInfo_Culture option:selected').val());
            });
        }

        $('#PriceInfo_FromDate').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#PriceInfo_ToDate').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#PriceInfo_TWFromDate').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#PriceInfo_TWToDate').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#PriceInfo_Price').numeric();

        $('input:radio[name="PriceInfo_IsPermanent"]').on('change', function () {
            if ($('input:radio[name="PriceInfo_IsPermanent"]:checked').val() == "True") {
                //$('#divPriceFromDate').hide();
                $('#PriceInfo_ToDate').val('');
                $('#PriceInfo_FromDate').datepicker('option', 'maxDate', null);
                $('#divPriceToDate').hide();
            }
            else {
                //$('#divPriceFromDate').show();
                $('#divPriceToDate').show();
            }
        });

        $('input:radio[name="PriceInfo_TWPermanent"]').on('change', function () {
            if ($('input:radio[name="PriceInfo_TWPermanent"]:checked').val() == 'True') {
                $('#PriceInfo_TWToDate').val('');

                $('#divTWToDate').hide();
            }
            else {
                $('#divTWToDate').show();
            }
        });

        $('#btnNewPriceInfo').on('click', function () {
            $('#divTravelWindowDates input').each(function () {
                $(this).removeAttr('disabled');
            });
        });

        $('#btnGroupPrices').on('click', function () {
            if ($(this).val().toLowerCase() == 'group') {
                $('#priceTypesGroups').show();
                $('#btnSavePricesGroup').show();
                $('#pricesToGroupContainer').show();
                $('#btnNewPriceInfo').hide();
                $(this).val('done');
                //unselect any selected row in tblPrices
                PRICE.oTable.$('tr.selected-row').each(function () {
                    var event = $.Event('keydown');
                    event.keyCode = 27;
                    $(document).trigger(event);
                });
                //collapse fdsPriceInfo
                COMMON.expandAndCollapse('#fdsPriceInfo', '#fdsPriceInfo');
                PRICE.makeTableRowsGroupable();
            }
            else {
                $('#btnNewPriceInfo').show();
                $('#priceTypesGroups').hide();
                $(this).val('group');
                $('#btnSavePricesGroup').hide();
                $('#pricesToGroupContainer').hide();
                //clear all previously values including selected rows
                $('#hdnPricesToGroup').val('');
                $('#pricesToGroup').text('');
                $('.price-groups').css('font-weight', 'normal');
                $('#priceGroupID').val('');
                PRICE.oTable.$('tr.selected-row').each(function () {
                    $(this).removeClass('selected-row');
                });
                PRICE.makeTableRowsSelectable();
            }
        });

        $('#btnSavePricesGroup').on('click', function () {
            if (PRICE.oTable.$('tr.selected-row').length > 0) {
                var priceGroupID = $('#priceGroupID').val();
                var pricesExpired = 0;
                var minimumPrices = 0;
                var minimumPriceTypes = 'Online Price,Public Price,Cost Price';
                //check if minimumPriceTypes are selected
                PRICE.oTable.$('tr.selected-row').each(function () {
                    var toDate = $(this).children('td:nth-child(4)').text().trim().toString();
                    var expired = toDate != '' && toDate != 'Permanent' ? UI.validateStartEndDates(toDate) : false;
                    if (expired) {
                        pricesExpired++;
                    }
                    if (minimumPriceTypes.indexOf($(this).children('td:nth-child(5)').text().trim().toString()) > -1) {
                        minimumPrices++;
                    }
                });

                if (pricesExpired == 0 && minimumPrices >= 3) {
                    $.ajax({
                        url: '/Prices/SavePriceGroup',
                        cache: false,
                        type: 'POST',
                        data: { sysItemType: $('#PriceInfo_ItemType').val(), itemID: $('#PriceInfo_ItemID').val(), priceGroupID: priceGroupID, prices: $('#hdnPricesToGroup').val() },
                        success: function (data) {
                            var duration = data.ResponseType < 0 ? data.ResponseType : null;
                            if (data.ResponseType > 0) {
                                //deselect rows and clear fields
                                PRICE.oTable.$('tr.selected-row').each(function () {
                                    $(this).removeClass('selected-row');
                                });

                                $('#priceGroupID').val('');
                                $('#pricesToGroup').text('');
                                if (data.ResponseMessage == 'Prices Group Saved') {
                                    //if saved, add group
                                    $('#priceTypesGroups').append('<div class="price-groups" id="group' + data.ItemID + '"><input type="hidden" value="' + $('#hdnPricesToGroup').val() + '"/>Group ' + data.ItemID + '</div>');
                                }
                                else {
                                    //if update, redraw
                                    $('#group' + data.ItemID).children(':hidden').val($('#hdnPricesToGroup').val());
                                    $('#group' + data.ItemID).removeAttr('style');
                                }
                                $('#hdnPricesToGroup').val('');
                                PRICE.makePriceGroupsClickable();
                            }
                            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                        }
                    });
                }
                else {
                    UI.messageBox(0, 'Cannot Save or Edit a Group With Expired/Invalid Prices' + '<br />' + '', null, null);
                    //reselect prices selected before update attempt
                    if (priceGroupID != '') {
                        $('#group' + priceGroupID).click();
                    }
                }
            }
        });

        $('#btnAddUnit').on('click', function () {
            var builder = '<tr>';
            builder += '<td>' + $('#PriceUnitInfo_Culture').val() + '</td>'
            + '<td>' + $('#PriceUnitInfo_Unit').val() + '</td>'
            + '<td>' + $('#PriceUnitInfo_AdditionalInfo').val() + '</td>'
            + '<td>' + $('#PriceUnitInfo_Min').val() + '</td>'
            + '<td>' + $('#PriceUnitInfo_Max').val()
            + '<img src="/Content/themes/base/images/cross.png" class="right delete-item" />'
            + '</td>'
            + '</tr>';
            $('#tblPriceUnits tbody').append(builder);
            PRICE.deleteItemFunction();
        });

        $('#btnSavePrice').on('click', function () {
            if ($('#frmPrice').valid()) {
                var units = new Array();
                $('#tblPriceUnits tbody tr').each(function (index, item) {
                    var model = new PriceUnitInfoModel();
                    model.PriceUnitInfo_PriceID = $('#PriceInfo_PriceID').val();
                    model.PriceUnitInfo_PriceUnitID = $(this).attr('id') != undefined ? $(this).attr('id') : 0;
                    model.PriceUnitInfo_Culture = $(this).children('td:nth-child(1)').text().trim();
                    model.PriceUnitInfo_Unit = $(this).children('td:nth-child(2)').text().trim();
                    model.PriceUnitInfo_AdditionalInfo = $(this).children('td:nth-child(3)').text().trim();
                    model.PriceUnitInfo_Min = $(this).children('td:nth-child(4)').text().trim();
                    model.PriceUnitInfo_Max = $(this).children('td:nth-child(5)').text().trim();
                    units.push(model);
                });
                var _model = new PriceInfoModel();
                _model.PriceInfo_PriceID = $('#PriceInfo_PriceID').val();
                _model.PriceInfo_PriceType = $('#PriceInfo_PriceType').val();
                _model.PriceInfo_ItemType = $('#PriceInfo_ItemType').val();
                _model.PriceInfo_ItemID = $('#PriceInfo_ItemID').val();
                _model.PriceInfo_PriceClasification = $('#PriceInfo_PriceClasification').val();
                _model.PriceInfo_Price = $('#PriceInfo_Price').val();
                _model.PriceInfo_Currency = $('#PriceInfo_Currency').val();
                _model.PriceInfo_IsPermanent = $('input:radio[name="PriceInfo_IsPermanent"]:checked').val();
                _model.PriceInfo_FromDate = $('#PriceInfo_FromDate').val();
                _model.PriceInfo_ToDate = $('#PriceInfo_ToDate').val();
                _model.PriceInfo_TWFromDate = $('#PriceInfo_TWFromDate').is(':disabled') ? null : $('#PriceInfo_TWFromDate').val();
                _model.PriceInfo_TWToDate = $('#PriceInfo_TWToDate').is(':disabled') ? null : $('#PriceInfo_TWToDate').val();
                _model.PriceInfo_TWPermanent = $('input:radio[name="PriceInfo_TWPermanent"]').is(':disabled') ? null : $('input:radio[name="PriceInfo_TWPermanent"]:checked').val();
                _model.PriceInfo_GenericUnit = $('#PriceInfo_GenericUnit').val();
                _model.PriceInfo_Terminal = $('#PriceInfo_Terminal').val();
                _model.PriceInfo_TaxesIncluded = $('#PriceInfo_TaxesIncluded').val();
                _model.PriceInfo_FromTransportationZone = $('#PriceInfo_FromTransportationZone').val();
                _model.PriceInfo_ToTransportationZone = $('#PriceInfo_ToTransportationZone').val();
                _model.PriceInfo_UrlRedeem = $('#PriceInfo_UrlRedeem').val();
                _model.PriceInfo_UrlCompare = $('#PriceInfo_UrlCompare').val();
                _model.PriceInfo_PriceUnits = units;
                _model.PriceInfo_PriceToReplace = $('#PriceInfo_PriceToReplace').val();
                var jsonObj = JSON.stringify(_model);
                $.ajax({
                    url: '/Prices/SavePrice',
                    cache: false,
                    type: 'POST',
                    data: jsonObj,
                    dataType: 'json',
                    traditional: true,
                    contentType: 'application/json; charset=utf-8',
                    success: function (data) {
                        PRICE.savePriceSuccess(data);
                    }
                });
            }
            else {
                UI.showValidationSummary('frmPrice');
            }
        });

        $('#PriceInfo_Culture').on('change', function () {
            PRICE.updateTblPrices(undefined, true);
        });

        $('#PriceInfo_PointOfSale').on('change', function () {
            PRICE.updateTblPrices(undefined, true);
        });

        $('#openForm').on('click', function (e) {
            UI.expandFieldset('fdsPrices');
            $('#fdsPriceInfo').show();
            $('#fdsPriceInfoWizard').hide();
            $('#btnNewPriceInfo').trigger('click');
        });

        $('#openWizard').on('click', function (e) {
            UI.expandFieldset('fdsPrices');
            $.ajax({
                //url:'/Prices/RenderPricesWizard?providerID=' + $('#ActivityInfo_Provider option:selected').val() + '&serviceID=' + $('#ActivityInfo_ActivityID').val(),
                url:'/Prices/RenderPricesWizard',
                type: 'POST',
                cache: false,
                data: { providerID: $('#ActivityInfo_Provider option:selected').val(), serviceID: $('#ActivityInfo_ActivityID').val() },
                success: function (data) {
                    $('#fdsPriceInfo').hide();
                    $('#fdsPriceInfoWizard').show();
                    $('#fdsPriceInfoWizard').children('div:first').html(data);
                    WIZARD.init();
                    UI.expandFieldset('fdsPriceInfoWizard');
                    UI.scrollTo('fdsPriceInfoWizard', null);
                }
            });
            //$.fancybox({
            //    //type: 'html',
            //    type: 'ajax',
            //    href: '/Prices/RenderPricesWizard?providerID=' + $('#ActivityInfo_Provider option:selected').val(),
            //    //href: $('#divFastSaleContainer'),
            //    //modal: true,
            //    afterShow: function () {
            //        WIZARD.init();
            //    }
            //});
        });
    }

    var updateTblPrices = function (priceID, hasSpecialRate) {
        $('#divTblExistingPrices').empty();
        $.ajax({
            url: '/Prices/GetPrices',
            cache: false,
            type: 'POST',
            //data: { itemType: $('#PriceInfo_ItemType').val(), itemID: $('#PriceInfo_ItemID').val(), culture: ($('#PriceInfo_Culture').is(':visible') ? $('#PriceInfo_Culture').val() : '') },
            //data: { itemType: $('#PriceInfo_ItemType').val(), itemID: $('#PriceInfo_ItemID').val(), culture: ((hasSpecialRate != undefined && hasSpecialRate) ? $('#PriceInfo_Culture').val() : '') },
            data: { itemType: $('#PriceInfo_ItemType').val(), itemID: $('#PriceInfo_ItemID').val(), culture: ((hasSpecialRate != undefined && hasSpecialRate) ? $('#PriceInfo_Culture').val() : $('#PriceInfo_Culture').hasClass('special-rate') ? $('#PriceInfo_Culture').val() : ''), pos: $('#PriceInfo_PointOfSale option:selected').val() },
            success: function (data) {
                $('#divTblExistingPrices').html(data);
                PRICE.searchResultsTable($('#tblSearchPricesResults'));
                PRICE.actionsAfterRenderTable(priceID);
            }
        });
    }

    var actionsAfterRenderTable = function (priceID) {
        PRICE.makeTableRowsSelectable();
        var _basePriceTypes = new Array();
        var _priceTypeRules = new Array();
        var _cells = 3;
        if ($('#PriceInfo_ItemType').val() == 'Activities' || $('#PriceInfo_ItemType').val() == 'Transportation') {
            _cells = 2;

            if (PRICE.oTable.$('tr').length == 0) {
                $.post('/Prices/GetRules', { serviceID: $('#PriceInfo_ItemID').val(), terminalID: $('#ActivityInfo_OriginalTerminal').val() }, function (data) {
                    $.each(data, function (index, item) {
                        if (item.IsBasePrice) {
                            _basePriceTypes.push(item.RuleFor.trim() + ' (P'+ item.PriceTypeID + ')');
                        }
                    });
                    $('#PriceInfo_PriceType option').each(function () {
                        if ($.inArray($(this).text(), _basePriceTypes) == -1) {
                            $(this).hide();
                        }
                        else {
                            $(this).show();
                        }
                    });
                });
            }
            else {
                PRICE.oTable.$('tr').each(function (index, item) {
                    //var _index = $(this).find('.mb-confirmation').index();
                    var _index = $(this).find('.mb-warning').index();
                    if (_index != -1) {
                        _basePriceTypes.push($($($(PRICE.oTable.$('tr:first').parent('tbody').prev()[0]).find('tr:first')[0])[0]).find('th:nth-child(' + (_index + 1) + ')')[0].textContent.trim());
                    }
                });
                
                $('#PriceInfo_PriceType option').each(function () {
                    if ($.inArray($(this).text(), _basePriceTypes) == -1) {
                        $(this).hide();
                    }
                    else {
                        $(this).show();
                    }
                });
            }
            $('#divTravelWindowDates input').each(function () {
                $(this).removeAttr('disabled');
            });
        }
        if (!$('#tblSearchPricesResults tbody tr').children('td').hasClass('dataTables_empty')) {
            PRICE.oTable.$('tr').each(function (index) {
                var toDate = $(this)[0].cells[_cells].textContent.trim();
                var active = PRICE.isPriceActive(toDate.split('/')[0].trim(), toDate.split('/')[1].trim());
                if (!active) {
                    $(this).addClass('expired');
                }
                else {
                    $(this).removeClass('expired');
                }
            });
        }
        if (priceID != undefined) {
            $('#trPrice' + priceID).click();
        }
        //**Muestra en select solo los precios hábiles
        $.getJSON('/Prices/FillDrpPrices', { itemType: $('#PriceInfo_ItemType').val(), itemID: $('#PriceInfo_ItemID').val() }, function (data) {
            $('.prices-related').fillSelect(data);
            $('.prices-related').children('option').each(function () {
                if (PRICE.oTable.$('tr#trPrice' + $(this).val()).hasClass('expired')) {
                    $(this).hide();
                }
            });
        });
        PRICE.pricesLoaded();
    }

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchPricesResults', tableColumns.length - 1);
        PRICE.oTable = $('#tblSearchPricesResults').dataTable();
        if (PRICE.oTable != undefined && PRICE.oTable.length > 0) {
            var oSettings = PRICE.oTable.fnSettings();
            oSettings._iDisplayLength = 100;
            PRICE.oTable.fnDraw();
            PRICE.oTable.fnSort([[0, 'desc']]);
        }
    }

    var savePriceSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if ($('#PriceInfo_ItemType').val() == 'Activities' || $('#PriceInfo_ItemType').val() == 'Transportation') {
                $('#btnNewPriceInfo').trigger('click');
                $('#tblPriceUnits tbody').empty();
                PRICE.updateTblPrices();
            }
            else {
                var priceClasification = $('#PriceInfo_PriceClasification option:selected').text();

                if ($('input:radio[name="PriceInfo_IsPermanent"]:checked').val() == 'True') {
                    $('#PriceInfo_ToDate').val('');
                }
                var date = $('#PriceInfo_FromDate').val() + ' / ' + ($('#PriceInfo_ToDate').val() != '' ? $('#PriceInfo_ToDate').val() : 'Permanent');
                if (data.ResponseMessage == 'Price Saved') {
                    var oSettings = PRICE.oTable.fnSettings();
                    var iAdded = PRICE.oTable.fnAddData([
                        UI.addDecimalPart($('#PriceInfo_Price').val()),
                        $('#PriceInfo_Currency option:selected').val(),
                        $('#chkTerminal' + $('#PriceInfo_Terminal').val()).next('label').text(),
                        date,
                        $('#PriceInfo_PriceType option:selected').text(),
                        priceClasification,
                        '<img class="right" src="/Content/themes/base/images/trash.png" id="delP' + data.ItemID.priceID + '">'
                    ]);
                    var aRow = oSettings.aoData[iAdded[0]].nTr;
                    aRow.setAttribute('id', 'trPrice' + data.ItemID.priceID);
                    PRICE.oTable.fnDisplayRow(aRow);
                    PRICE.actionsAfterRenderTable();
                    UI.tablesHoverEffect();
                }
                else {
                    var array = PRICE.oTable.fnGetNodes();
                    var nTr = PRICE.oTable.$('tr.selected-row');
                    var position = PRICE.oTable.fnGetPosition(nTr[0]);
                    PRICE.oTable.fnDisplayRow(array[position]);
                    PRICE.oTable.fnUpdate([
                        UI.addDecimalPart($('#PriceInfo_Price').val()),
                        $('#PriceInfo_Currency option:selected').val(),
                        $('#chkTerminal' + $('#PriceInfo_Terminal').val()).next('label').text(),
                        date,
                        $('#PriceInfo_PriceType option:selected').text(),
                        priceClasification,
                        $('#trPrice' + data.ItemID.priceID).children('td:nth-child(7)').html()
                    ], $('#trPrice' + data.ItemID.priceID)[0], undefined, false);
                }
            }
        }
        if (data.ResponseType == 0) {
            if (data.ResponseMessage == 'Price Already Exists') {
                //try to save price
                var builder = '';
                $.each(data.ItemID, function (index, item) {
                    builder += '<span><input type="radio" name="prices" value="' + item.priceID + '" ' + (index == 0 ? 'checked="checked"' : '') + '>' + item.price + ' ' + item.currencyCode + ' - ' + item.priceUnit.unit + ' ' + (item.priceUnit.min != null ? item.priceUnit.min + '-' + item.priceUnit.max : '') + '</span><br />';
                });
                UI.twoActionBox(data.ResponseMessage + '<br />What price do you want to replace?<br />' + builder, replacePrice, [], 'Replace', noReplacePrice, [], 'Create New');
                PRICE.bindFunctionToPricesRadios();
                $('#PriceInfo_PriceToReplace').val($('input:radio[name="prices"]:checked').val());
            }
            else {
                //try to update price being used
                UI.confirmBox(data.ResponseMessage + '<br />Do you want to create a new price with same data?', clonePrice, [data.ItemID.priceID, $('#PriceInfo_Price').val()]);
            }
        }
        else {
            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
        }
        PRICE.actionsAfterRenderTable();
    }

    function replacePrice() {
        $('#btnSavePrice').trigger('click');
    }

    function noReplacePrice() {
        $('#PriceInfo_PriceToReplace').val('null');
        $('#btnSavePrice').trigger('click');
    }

    var bindFunctionToPricesRadios = function () {
        $('input:radio[name="prices"]').on('change', function () {
            $('#PriceInfo_PriceToReplace').val($('input:radio[name="prices"]:checked').val());
        });
    }
    //var countPriceTypes = function () {
    //    if ($('#PriceInfo_ItemType').val() == 'Activities' || $('#PriceInfo_ItemType').val() == 'Transportation') {
    //        var _public = 0;
    //        var online = 0;
    //        var cost = 0;
    //        var chargeBack = 0;
    //        var goldCard = 0;
    //        //var oPriceTable = $('#tblSearchPricesResults').dataTable();
    //        //oPriceTable.$('tr').each(function (index, item) {
    //        PRICE.oTable.$('tr').each(function (index, item) {
    //            switch ($(this)[0].cells[4].textContent.trim()) {
    //                case 'Public Price': {
    //                    _public++;
    //                    break;
    //                }
    //                case 'Online Price': {
    //                    online++;
    //                    break;
    //                }
    //                case 'Cost Price': {
    //                    cost++;
    //                    break;
    //                }
    //                case 'Charge Back Price': {
    //                    chargeBack++;
    //                    break;
    //                }
    //                case 'Gold Card Price': {
    //                    goldCard++;
    //                    break;
    //                }
    //            }
    //        });
    //        var pendingTypes = '<span id="tagPublic">Public Prices: ' + _public + '</span>, <span id="tagGoldCard">Gold Card Prices: ' + goldCard + '</span>, <span id="tagOnline">Online Prices: ' + online + '</span>, <span id="tagChargeBack">Charge Back Prices: ' + chargeBack + '</span>, <span id="tagCost">Cost Prices: ' + cost + '</span>';
    //        $('#priceTypesPending').html('');
    //        $('#priceTypesPending').html(pendingTypes);
    //        PRICE.checkForPendingPriceTypes();
    //    }
    //}

    //var checkForPendingPriceTypes = function () {
    //    var _public = parseInt($('#tagPublic').text().split(':')[1].trim());
    //    var online = parseInt($('#tagOnline').text().split(':')[1].trim());
    //    var cost = parseInt($('#tagCost').text().split(':')[1].trim());
    //    var chargeBack = parseInt($('#tagChargeBack').text().split(':')[1].trim());
    //    var goldCard = parseInt($('#tagGoldCard').text().split(':')[1].trim());
    //    var array = [_public, online, cost, chargeBack, goldCard];
    //    var max = Math.max.apply(Math, array);
    //    if (_public != max) {
    //        $('#tagPublic').css({ 'color': 'red' });
    //    }
    //    else {
    //        $('#tagPublic').removeAttr('style');
    //    }
    //    if (online != max) {
    //        $('#tagOnline').css({ 'color': 'red' });
    //    }
    //    else {
    //        $('#tagOnline').removeAttr('style');
    //    }
    //    if (cost != max) {
    //        $('#tagCost').css({ 'color': 'red' });
    //    }
    //    else {
    //        $('#tagCost').removeAttr('style');
    //    }
    //    if (chargeBack != max) {
    //        $('#tagChargeBack').css({ 'color': 'red' });
    //    }
    //    else {
    //        $('#tagChargeBack').removeAttr('style');
    //    }
    //    if (goldCard != max) {
    //        $('#tagGoldCard').css({ 'color': 'red' });
    //    }
    //    else {
    //        $('#tagGoldCard').removeAttr('style');
    //    }
    //}

    function clonePrice(priceID, price) {
        $.ajax({
            url: '/Prices/ClonePrice',
            type: 'POST',
            cache: false,
            data: { priceID: priceID, price: price },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseMessage == 'Price Saved') {
                    PRICE.updateTblPrices(data.ItemID.priceID);
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />Modify neccesary fields and Update', duration, data.InnerException);
                    //var priceClasification = '';
                    //if ($('#PriceInfo_ItemType').val() == 'Activities') {
                    //    priceClasification = $('#PriceUnitInfo_Unit').val() + ' ' + $('#PriceUnitInfo_Min').val() + ' - ' + $('#PriceUnitInfo_Max').val();
                    //}
                    //else if ($('#PriceInfo_ItemType').val() == 'Transportation') {
                    //    var _toTransportationZone = $('#PriceInfo_ToTransportationZone option:selected').text().substr($('#PriceInfo_ToTransportationZone option:selected').text().indexOf('-') + 1).trim();
                    //    priceClasification = $('#PriceUnitInfo_Unit').val() + ', ' + _toTransportationZone;
                    //}
                    //else {
                    //    priceClasification = $('#PriceInfo_PriceClasification option:selected').text();
                    //}
                    ////var date = $('#PriceInfo_ToDate').val() != '' ? $('#PriceInfo_ToDate').val() : 'Permanent';
                    //var date = $('#PriceInfo_ToDate').val() != '' ? ($('#PriceInfo_ToDate').val() + ' / ' + $('#PriceInfo_ToDate').val()) : 'Permanent';
                    //var oSettings = PRICE.oTable.fnSettings();
                    //var iAdded = PRICE.oTable.fnAddData([
                    //    UI.addDecimalPart($('#PriceInfo_Price').val()),
                    //    $('#PriceInfo_Currency option:selected').val(),
                    //    $('#chkTerminal' + $('#PriceInfo_Terminal').val()).next('label').text(),
                    //    date,
                    //    $('#PriceInfo_PriceType option:selected').text(),
                    //    priceClasification,
                    //    '<img class="right" src="/Content/themes/base/images/cross.png" id="delP' + data.ItemID.priceID + '">'
                    //]);
                    //var aRow = oSettings.aoData[iAdded[0]].nTr;
                    //aRow.setAttribute('id', 'trPrice' + data.ItemID.priceID);
                    //PRICE.oTable.fnDisplayRow(aRow);
                    //UI.tablesHoverEffect();
                    //PRICE.actionsAfterRenderTable();
                    //$('#trPrice' + priceID).addClass('expired');
                    //$('#trPrice' + data.ItemID.priceID).click();
                    //UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />Modify neccesary fields and Update', duration, data.InnerException);
                    ////PRICE.updateTblPrices();
                    ////$('#tblSearchPricesResults').find('.selected-row').removeClass('selected-row secondary');
                    ////$('#trPrice' + data.ItemID[0]).addClass('selected-row secondary');
                    ////$('#PriceInfo_PriceID').val(data.ItemID.priceID);
                    ////$('#PriceInfo_FromDate').val(data.ItemID.date);
                    ////UI.messageBox(data.ResponseType, "Modify neccesary fields and Save", duration, data.InnerException)
                }
                else {
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            }
        });
    }

    var searchPriceClasificationSuccess = function (data) {
        var builder = '';
        $.each(data, function (index, item) {
            builder += '<tr id="trPriceClasification' + item.ItemID + '"><td id="td'
                + item.ItemName + '">' + item.ItemName
                + '</td><td class="tds"><img id="delItem' + item.ItemID
                + '" src="/Content/themes/base/images/cross.png" style="float:right" ></td></tr>';
        });
        $('#tblSearchPriceClasificationsResults tbody').empty();
        $('#tblSearchPriceClasificationsResults').append(builder);
        UI.tablesStripedEffect();
        UI.tablesHoverEffect();
        PRICE.makeTablePricesClasificationRowsSelectable();
    }

    var searchCurrenciesResultsSuccess = function (data) {
        var builder = '';
        $.each(data, function (index, item) {
            builder += '<tr id="trCurrency' + item.ItemID + '"><td id="td'
                + item.ItemName + '">' + item.ItemName
                + '</td><td class="tds"><img id="delItem' + item.ItemID
                + '" src="/Content/themes/base/images/cross.png" style="float:right" ></td></tr>';
        });
        $('#tblSearchCurrenciesResults tbody').empty();
        $('#tblSearchCurrenciesResults').append(builder);
        UI.tablesStripedEffect();
        UI.tablesHoverEffect();
        PRICE.makeTableCurrenciesRowsSelectable();
    }

    var makeTableRowsSelectable = function (data) {
        //var _target = $('#PriceInfo_ItemType').val() == 'Activities' || $('#PriceInfo_ItemType').val() == 'Transportation' ? '.mb-warning' : '';
        var _target = $('#PriceInfo_ItemType').val() == 'Activities' || $('#PriceInfo_ItemType').val() == 'Transportation' ? '.selectable-row' : '';
        PRICE.oTable.$('tr' + _target).not('theader').unbind('click').on('click', function (e) {
            //if ($(this).hasClass('base-price')) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    PRICE.oTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    var id = PRICE.oTable.$('tr.selected-row').attr('id').substr(7);
                    $.ajax({
                        url: '/Prices/GetPricePerID',
                        cache: false,
                        type: 'POST',
                        data: { priceID: id },
                        success: function (data) {
                            $('#PriceInfo_PriceType option[value=' + data.PriceInfo_PriceType + ']').attr('selected', true);
                            $('#PriceInfo_PriceID').val(id);
                            $('#PriceInfo_PriceClasification option[value=' + data.PriceInfo_PriceClasification + ']').attr('selected', true);
                            $('#PriceInfo_Price').val(data.PriceInfo_Price);
                            $('#PriceInfo_Currency option[value=' + data.PriceInfo_Currency + ']').attr('selected', true);
                            if (data.PriceInfo_IsPermanent == true) {
                                $('input:radio[name="PriceInfo_IsPermanent"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="PriceInfo_IsPermanent"]')[1].checked = true;
                            }
                            $('input:radio[name="PriceInfo_IsPermanent"]').trigger('change');
                            $('#PriceInfo_FromDate').val(data.PriceInfo_FromDate);
                            $('#PriceInfo_ToDate').val(data.PriceInfo_ToDate);
                            $('#PriceInfo_TWFromDate').val(data.PriceInfo_TWFromDate);
                            if (data.PriceInfo_TWPermanent == true) {
                                $('input:radio[name="PriceInfo_TWPermanent"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="PriceInfo_TWPermanent"]')[1].checked = true;
                            }
                            $('input:radio[name="PriceInfo_TWPermanent"]').trigger('change');
                            $('#PriceInfo_TWToDate').val(data.PriceInfo_TWToDate);
                            if ($('#divSelectedRole').text().indexOf('Administrator') == -1) {
                                $('#divTravelWindowDates input').each(function () {
                                    $(this).attr('disabled', 'disabled');
                                });
                            }
                            $('#PriceInfo_GenericUnit').val(data.PriceInfo_GenericUnit);
                            $('#PriceInfo_Terminal option[value=' + data.PriceInfo_Terminal + ']').attr('selected', true);
                            if (data.PriceInfo_TaxesIncluded == true) {
                                $('input:radio[name="PriceInfo_TaxesIncluded"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="PriceInfo_TaxesIncluded"]')[1].checked = true;
                            }
                            $('#PriceInfo_FromTransportationZone option[value=' + data.PriceInfo_FromTransportationZone + ']').attr('selected', true);
                            $('#PriceInfo_ToTransportationZone option[value=' + data.PriceInfo_ToTransportationZone + ']').attr('selected', true);
                            $('#PriceInfo_UrlRedeem').val(data.PriceInfo_UrlRedeem);
                            $('#PriceInfo_UrlCompare').val(data.PriceInfo_UrlCompare);
                            //PriceUnit
                            var builder = '';
                            $.each(data.PriceInfo_PriceUnits, function (index, item) {
                                builder += '<tr id="' + item.PriceUnitInfo_PriceUnitID + '">'
                                + '<td>' + item.PriceUnitInfo_Culture + '</td>'
                                + '<td>' + item.PriceUnitInfo_Unit + '</td>'
                                + '<td>' + (item.PriceUnitInfo_AdditionalInfo != null ? item.PriceUnitInfo_AdditionalInfo : '') + '</td>'
                                + '<td>' + (item.PriceUnitInfo_Min != null ? item.PriceUnitInfo_Min : '') + '</td>'
                                + '<td>' + (item.PriceUnitInfo_Max != null ? item.PriceUnitInfo_Max : '')
                                + '<img src="/Content/themes/base/images/trash.png" class="right delete-item">'
                                + '</td>'
                                + '</tr>';
                            });
                            $('#tblPriceUnits tbody').empty();
                            $('#tblPriceUnits tbody').append(builder);
                            PRICE.deleteItemFunction();
                            //--
                            UI.expandFieldset('fdsPriceInfo');
                            UI.scrollTo('fdsPriceInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deletePrice, [$(this).attr('id').substr(7)]);
            }
            //}
        });
    }

    var deleteItemFunction = function () {
        $('.delete-item').unbind('click').on('click', function () {
            $(this).parents('tr:first').remove();
        });
    }

    var makeTablePricesClasificationRowsSelectable = function (data) {
        $('#tblSearchPriceClasificationsResults tbody tr').not('theader').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    var id = $(this).attr('id').substr(20);
                    $.ajax({
                        url: '/Prices/GetPriceClasificationPerID',
                        cache: false,
                        type: 'POST',
                        data: { priceClasificationID: id },
                        success: function (data) {
                            $.each(data, function (index, item) {
                                $('#PriceClasification_PriceClasificationID').val(id);
                                $('#PriceClasification_PriceClasification').val(item.ItemName);
                            });
                            UI.expandFieldset('fdsPricesClasificationInfo');
                        }
                    });
                }
            }
            else {
                var result = UI.confirmToDelete('Do you really want to delete this Price Clasification?');
                if (result) {
                    if ($(e.target).parent('tr').hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $.ajax({
                        url: '/Prices/DeletePriceClasification',
                        cache: false,
                        type: 'POST',
                        data: { priceClasificationID: $(e.target).attr('id').substr(5) },   //img id="delPC'+ priceClasification_id +'"
                        success: function (data) {
                            var duration = data.ResponseType < 0 ? data.ResponseType : null;
                            if (data.ResponseType > 0) {
                                $('#trPriceClasification' + data.ItemID).remove();
                                UI.tablesStripedEffect();
                            }
                            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, data.InnerException);
                        }
                    });
                }
            }
        });
    }

    var makeTableCurrenciesRowsSelectable = function () {
        $('#tblSearchCurrenciesResults tbody tr').not('theader').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    var id = $(this).attr('id').substr(10);
                    $.ajax({
                        url: '/Prices/GetCurrencyPerID',
                        cache: false,
                        type: 'POST',
                        data: { currencyID: id },
                        success: function (data) {
                            $.each(data, function (index, item) {
                                $('#CurrencyInfo_CurrencyID').val(item.ItemID);
                                $('#CurrencyInfo_Currency').val(item.ItemName);
                                $('#CurrencyInfo_CurrencyCode').val(item.Item2);
                            });
                            UI.expandFieldset('fdsCurrenciesInfo');
                        }
                    });
                }
            }
            else {
                var result = UI.confirmToDelete('Do you really want to delete this Currency?');
                if (result) {
                    if ($(e.target).parent('tr').hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $.ajax({
                        url: '/Prices/DeleteCurrency',
                        cache: false,
                        type: 'POST',
                        data: { currencyID: $(e.target).attr('id').substr(4) }, //img id="delC'+ currency_id +'"
                        success: function (data) {
                            var duration = data.ResponseType < 0 ? data.ResponseType : null;
                            if (data.ResponseType > 0) {
                                $('#trCurrency' + data.ItemID).remove();
                                UI.tablesStripedEffect();
                            }
                            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, data.InnerException);
                        }
                    });
                }
            }
        });
    }

    var savePriceClasificationSuccess = function (data) {
        if (data.ResponseType > 0) {
            $('#btnSearchPriceClasifications').trigger('click');
            $.getJSON('/Prices/FillDrpPriceClasifications', null, function (data) {
                $('#PriceClasification_DrpPriceClasifications').fillSelect(data);
            });
        }
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveCurrencySuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            $('#btnSearchCurrencies').trigger('click');
            $.getJSON('/Prices/FillDrpCurrencies', null, function (data) {
                $('#CurrencyInfo_DrpCurrencies').fillSelect(data);
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    function deletePrice(priceID) {
        $.ajax({
            url: '/Prices/DeletePrice',
            type: 'POST',
            cache: false,
            data: { priceID: priceID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#PriceInfo_ItemType').val() == 'Activities' || $('#PriceInfo_ItemType').val() == 'Transportation') {
                        $('#btnNewPriceInfo').trigger('click');
                        $('#tblPriceUnits tbody').empty();
                        PRICE.updateTblPrices();
                    }
                    else {
                        if ($('#trPrice' + priceID).hasClass('selected-row')) {
                            var event = $.Event('keydown');
                            event.keyCode = 27;
                            $(document).trigger(event);
                        }
                        PRICE.oTable.fnDeleteRow($('#trPrice' + data.ItemID)[0]);
                    }
                    $('.prices-related').each(function () {
                        $(this).children('option').find('#' + data.ItemID).remove();
                    });
                }
                //PRICE.countPriceTypes();
                UI.tablesStripedEffect();
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var updatePriceGroupsContainer = function () {
        $.ajax({
            url: '/Prices/GetGroupPrices',
            cache: false,
            type: 'POST',
            data: { sysItemType: $('#PriceInfo_ItemType').val(), itemID: $('#PriceInfo_ItemID').val() },
            success: function (data) {
                $('#priceTypesGroups').empty();
                //$('#pricesInGroup').val('');
                var builder = '';
                $.each(JSON.parse(data), function (index, item) {
                    builder += '<div id="group' + item.Key + '" class="price-groups"';
                    if (item.Value.split('-')[0].toLowerCase() == 'true') {
                        builder += '>';
                    }
                    else {
                        builder += 'style="color:red">';
                    }
                    builder += '<input type="hidden" value="' + item.Value.split('-')[1] + '" />';
                    builder += 'Group ' + item.Key + '</div>';
                });
                $('#priceTypesGroups').append(builder);
                PRICE.makePriceGroupsClickable();
            }
        });
        //$.ajax({
        //    url: '/Prices/GetGroupPrices',
        //    cache: false,
        //    type: 'POST',
        //    data: { sysItemType: $('#PriceInfo_ItemType').val(), itemID: $('#PriceInfo_ItemID').val() },
        //    success: function (data) {
        //        $('#priceTypesGroups').empty();
        //        $('#pricesInGroup').val('');
        //        var builder = '';
        //        $.each(JSON.parse(data), function (index, item) {
        //            builder += '<div id="group' + item.Key + '" class="price-groups"';
        //            if (item.Value.toLowerCase() == 'true') {
        //                builder += '>';
        //            }
        //            else {
        //                builder += 'style="color:red">';
        //            }
        //            builder += 'Group ' + item.Key + '</div>';
        //        });
        //        $('#priceTypesGroups').append(builder);
        //        PRICE.makePriceGroupsClickable();
        //    }
        //});
    }

    var makeTableRowsGroupable = function () {
        PRICE.oTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            var trID = $(this).attr('id');
            if (!$(this).hasClass('selected-row')) {
                //only distinct priceTypes and non-expired prices allowed
                if ($('#pricesToGroup').text().indexOf($(this)[0].cells[4].textContent.trim()) == -1 && !$(this).hasClass('expired')) {
                    $('#hdnPricesToGroup').val($('#hdnPricesToGroup').val() + ($('#hdnPricesToGroup').val() != '' ? ',' : '') + trID.substr(7));
                    $('#pricesToGroup').text($('#pricesToGroup').text() + ($('#pricesToGroup').text() != '' ? ',' : '') + $(this)[0].cells[0].textContent.trim() + ' ' + $(this)[0].cells[1].textContent.trim() + '-' + $(this)[0].cells[4].textContent.trim());
                    $(this).addClass('selected-row');
                }
            }
            else {
                var _array = $('#hdnPricesToGroup').val().split(',');
                var _indexArray = $.inArray(trID.substr(7), _array);
                _array.splice(_indexArray, 1);
                var _prices = $('#pricesToGroup').text().split(',');
                var _indexPrice = $.inArray(($(this)[0].cells[0].textContent.trim() + ' ' + $(this)[0].cells[1].textContent.trim() + '-' + $(this)[0].cells[4].textContent.trim()), _prices);
                _prices.splice(_indexPrice, 1);
                $('#hdnPricesToGroup').val(_array.toString());
                $('#pricesToGroup').text(_prices.toString());
                $(this).removeClass('selected-row');
            }
        });
    }

    var makePriceGroupsClickable = function () {
        $('.price-groups').unbind('click').on('click', function () {
            $('.price-groups').css('font-weight', 'normal');
            $(this).css('font-weight', 'bold');
            var _prices = $(this).children(':hidden').val().split(',');
            //clear values of previous selected group
            PRICE.oTable.$('tr.selected-row').each(function () {
                $(this).removeClass('selected-row');
            });
            $('#hdnPricesToGroup').val('');
            $('#pricesToGroup').text('');
            $('#priceGroupID').val($(this).attr('id').substr(5));
            $.each(_prices, function (index, item) {
                if (item != 'null') {
                    PRICE.oTable.$('tr#trPrice' + item).addClass('selected-row');
                    $('#hdnPricesToGroup').val($('#hdnPricesToGroup').val() + ($('#hdnPricesToGroup').val() != '' ? ',' : '') + item);
                    $('#pricesToGroup').text($('#pricesToGroup').text() + ($('#pricesToGroup').text() != '' ? ',' : '') + PRICE.oTable.$('tr#trPrice' + item)[0].cells[0].textContent.trim() + ' ' + PRICE.oTable.$('tr#trPrice' + item)[0].cells[1].textContent.trim() + '-' + PRICE.oTable.$('tr#trPrice' + item)[0].cells[4].textContent.trim());
                }
            });
        });
    }

    var isPriceActive = function (fromDate, toDate) {
        var date = new Date();
        var cYear = date.getFullYear().toString();
        var cMonth = date.getMonth();
        cMonth = cMonth + 1;
        cMonth = cMonth.toString();
        var cDay = date.getDate().toString();
        if (cYear.length == 2) {
            if (parseInt(cYear, 10) < 70) cYear = '20' + cYear;
            else cYear = '19' + cYear;
        }
        if (cMonth.length == 1) cMonth = '0' + cMonth;
        if (cDay.length == 1) cDay = '0' + cDay;
        var todayDate = cYear + cMonth + cDay;

        var tDate = fromDate.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2})/),
            fyear = tDate[1],
            fmonth = tDate[2],
            fday = tDate[3];
        if (fyear.length == 2) {
            if (parseInt(fyear, 10) < 70) fyear = '20' + fyear;
            else fyear = '19' + fyear;
        }
        if (fmonth.length == 1) fmonth = '0' + fmonth;
        if (fday.length == 1) fday = '0' + fday;
        var startDate = fyear + fmonth + fday;

        if (toDate == 'Permanent') {
            if (startDate > todayDate) {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            var tDate = toDate.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2})/),
            fyear = tDate[1],
            fmonth = tDate[2],
            fday = tDate[3];
            if (fyear.length == 2) {
                if (parseInt(fyear, 10) < 70) fyear = '20' + fyear;
                else fyear = '19' + fyear;
            }
            if (fmonth.length == 1) fmonth = '0' + fmonth;
            if (fday.length == 1) fday = '0' + fday;
            var endDate = fyear + fmonth + fday;
            if (startDate > todayDate) {
                return false;
            }
            else {
                if (endDate > todayDate) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
    }

    var pricesLoaded = function () {
        $('.table .price').hover(function () {
            $(this).siblings().fadeIn('fast');
        }, function () {
            $(this).siblings().fadeOut('fast');
        });
        UI.addExtras();
    }

    return {
        init: init,
        updateTblPrices: updateTblPrices,
        //countPriceTypes: countPriceTypes,
        savePriceSuccess: savePriceSuccess,
        searchResultsTable: searchResultsTable,
        saveCurrencySuccess: saveCurrencySuccess,
        //validateStartEndDates: validateStartEndDates,
        makeTableRowsGroupable: makeTableRowsGroupable,
        makeTableRowsSelectable: makeTableRowsSelectable,
        actionsAfterRenderTable: actionsAfterRenderTable,
        makePriceGroupsClickable: makePriceGroupsClickable,
        //checkForPendingPriceTypes: checkForPendingPriceTypes,
        updatePriceGroupsContainer: updatePriceGroupsContainer,
        savePriceClasificationSuccess: savePriceClasificationSuccess,
        searchCurrenciesResultsSuccess: searchCurrenciesResultsSuccess,
        searchPriceClasificationSuccess: searchPriceClasificationSuccess,
        makeTableCurrenciesRowsSelectable: makeTableCurrenciesRowsSelectable,
        makeTablePricesClasificationRowsSelectable: makeTablePricesClasificationRowsSelectable,
        deleteItemFunction: deleteItemFunction,
        bindFunctionToPricesRadios: bindFunctionToPricesRadios,
        isPriceActive: isPriceActive,
        pricesLoaded: pricesLoaded
    }
}();

