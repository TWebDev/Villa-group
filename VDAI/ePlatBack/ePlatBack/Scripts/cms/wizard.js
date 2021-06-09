$(function () {
    WIZARD.init();
});

var WIZARD = new function () {

    var init = function () {
        $('#pricesManagementWizard').tabs();

        if ($('input:radio[name="ActivityInfo_TransportationService"]:checked').val().toLowerCase() == 'true') {
            $('#pricesManagementWizard').tabs('enable', 7);
        }
        else {
            $('#pricesManagementWizard').tabs('option', 'disabled', [7]);
        }

        $.getJSON('/Activities/GetDDLData', { itemID: 0, itemType: 'units' }, function (data) {
            var array = new Array();
            $.each(data, function (index, item) {
                array[index] = item.Text;
            });
            $('#PriceWizard_PriceUnit').autocomplete({
                //appendTo: '.fancybox-skin',
                source: array,
                minLength: 0,
                position: { my: 'right top', at: 'right bottom' },
                close: function (evt, ui) {
                    $('#PriceWizard_PriceUnit').children('option').each(function () {
                        if ($(this).text() == $('#PriceWizard_PriceUnit').val()) {
                            $(this).attr('selected', true);
                        }
                    });
                }
            });
        });

        $('#PriceWizard_StartBookingDate').datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: 0,
            changeMonth: true,
            changeYear: true,
            stepMinute: 5,
            onClose: function (dateText, inst) {
                var startDate = $('#PriceWizard_StartBookingDate').datepicker('getDate');
                var toDate = $('#PriceWizard_EndBookingDate').datepicker('getDate');
                if ($('input:radio[name="PriceWizard_IsBookingPermanent"]:checked').val().toLowerCase() == 'true') {
                    $('#PriceWizard_EndBookingDate').val('');
                }
                else if (toDate < startDate) {
                    $('#PriceWizard_EndBookingDate').datepicker('setDate', startDate);
                }
                $('#PriceWizard_EndBookingDate').datepicker('option', 'minDate', $('#PriceWizard_StartBookingDate').datepicker('getDate'));
            }
        });

        $('#PriceWizard_EndBookingDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            minDate: 0,
            onClose: function (dateText, inst) {
                if ($('#PriceWizard_StartBookingDate').val() == '') {
                    $('#PriceWizard_StartBookingDate').datepicker('setDate', $('#PriceWizard_EndBookingDate').datepicker('getDate'));
                }
            }
        });

        $('#PriceWizard_StartTravelDate').datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: 0,
            changeMonth: true,
            changeYear: true,
            stepMinute: 5,
            onClose: function (dateText, inst) {
                var startDate = $('#PriceWizard_StartTravelDate').datepicker('getDate');
                var toDate = $('#PriceWizard_EndTravelDate').datepicker('getDate');
                if ($('input:radio[name="PriceWizard_IsTravelPermanent"]:checked').val().toLowerCase() == 'true') {
                    $('#PriceWizard_EndTravelDate').val('');
                }
                else if (toDate < startDate) {
                    $('#PriceWizard_EndTravelDate').datepicker('setDate', startDate);
                }
                $('#PriceWizard_EndTravelDate').datepicker('option', 'minDate', $('#PriceWizard_StartTravelDate').datepicker('getDate'));
            }
        });

        $('#PriceWizard_EndTravelDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            minDate: 0,
            onClose: function (dateText, inst) {
                if ($('#PriceWizard_StartTravelDate').val() == '') {
                    $('#PriceWizard_StartTravelDate').datepicker('setDate', $('#PriceWizard_EndTravelDate').datepicker('getDate'));
                }
            }
        });
        
        $('input:radio[name="PriceWizard_IsNewUnit"]').on('change', function () {
            if ($('input:radio[name="PriceWizard_IsNewUnit"]:checked').val().toLowerCase() == 'true') {
                $('#PriceWizard_Unit').parents('.editor-alignment:first').hide();
            }
            else {
                $('#PriceWizard_Unit').parents('.editor-alignment:first').show();
            }
        });

        $('input:radio[name="PriceWizard_IsBookingPermanent"]').on('change', function () {
            if ($('input:radio[name="PriceWizard_IsBookingPermanent"]:checked').val().toLowerCase() == 'true') {
                $('#PriceWizard_EndBookingDate').parents('.editor-alignment:first').hide();
            }
            else {
                $('#PriceWizard_EndBookingDate').parents('.editor-alignment:first').show();
            }
        });

        $('input:radio[name="PriceWizard_IsTravelPermanent"]').on('change', function () {
            if ($('input:radio[name="PriceWizard_IsTravelPermanent"]:checked').val().toLowerCase() == 'true') {
                $('#PriceWizard_EndTravelDate').parents('.editor-alignment:first').hide();
            }
            else {
                $('#PriceWizard_EndTravelDate').parents('.editor-alignment:first').show();
            }
        });

        $('.prev-tab').on('click', function () {
            var _position = 1;
            if ($('#pricesManagementWizard').tabs('option', 'disabled')) {
                while ($('#pricesManagementWizard').tabs('option', 'disabled').indexOf(($('#pricesManagementWizard').tabs('option', 'active') - _position)) > -1) {
                    _position++;
                }
            }
            $('#pricesManagementWizard').tabs('option', 'active', ($('#pricesManagementWizard').tabs('option', 'active') - _position));
        });

        $('.next-tab').on('click', function () {
            var _position = 1;
            if ($('#pricesManagementWizard').tabs('option', 'disabled')) {
                while ($('#pricesManagementWizard').tabs('option', 'disabled').indexOf(($('#pricesManagementWizard').tabs('option', 'active') + _position)) > -1) {
                    _position++;
                }
            }
            $('#pricesManagementWizard').tabs('option', 'active', ($('#pricesManagementWizard').tabs('option', 'active') + _position));
        });

        $('#btnSavePriceFromWizard').on('click', function () {
            //get itemID
            $('#PriceWizard_ItemID').val($('#ActivityInfo_ActivityID').val());
            //get itemType
            if ($('input:radio[name="ActivityInfo_TransportationService"]:checked').val() == 'True') {
                $('#PriceWizard_ItemType').val('transportation');
            }
            else {
                $('#PriceWizard_ItemType').val('service');
            }
            //get terminal
            $('#PriceWizard_Terminal').val($('#ActivityInfo_OriginalTerminal option:selected').val());
            //get priceType
            var _priceType = '';
            ACTIVITY.oRulesTable.$('tr').not('theader').children('td:nth-child(3)').each(function (index, item) {
                if ($(this).text().trim().toLowerCase() == 'true') {
                    _priceType = $(this).parent('tr').children('td:nth-child(2)').text().trim();
                    return false;
                }
            });
            $('#PriceWizard_PriceType').val($('#PriceInfo_PriceType option').filter(function () { return $(this).html() == _priceType }).val());
            //get priceclasification
            //it comes as 1 from the model
            if ($('#frmPriceWizard').valid()) {
                $('#frmPriceWizard').submit();
            }
        });

        $('#btnAddPriceUnit').on('click', function (e) {
            var _tr = '';
            if ($('#tblUnits tbody tr').children('td:nth-child(1)').text().indexOf($('#PriceWizard_Language option:selected').text()) == -1) {
                _tr += '<tr>'
                    + '<td>' + $('#PriceWizard_Language option:selected').text() + '</td>'
                    + '<td>' + $('#PriceWizard_PriceUnit').val() + '</td>'
                    + '<td>' + $('#PriceWizard_Min').val() + '</td>'
                    + '<td>' + $('#PriceWizard_Max').val() + '</td>'
                    + '<td>' + $('#PriceWizard_AdditionalInfo').val() + '</td>'
                    + '<td class="tds"><img src="/Content/themes/base/images/cross.png" class="delete-row right" /></td>'
                    + '</tr>';
                $('#tblUnits tbody').append(_tr);
                updatePriceUnitsJson();
                WIZARD.deleteRow(updatePriceUnitsJson);
            }
            else {
                UI.messageBox(-1, 'You cannot add more than one unit with the same language', null, null);
            }
        });

        function updatePriceUnitsJson() {
            var _str = '[';
            $('#tblUnits tbody tr').each(function () {
                _str += (_str == '[' ? '' : ',') + '{'
                + '"PriceWizardUnit_Language":"' + $(this).children('td:nth-child(1)').text().trim().toLowerCase() + '",'
                + '"PriceWizardUnit_Unit":"' + $(this).children('td:nth-child(2)').text().trim().toLowerCase() + '",'
                + '"PriceWizardUnit_Min":"' + $(this).children('td:nth-child(3)').text().trim().toLowerCase() + '",'
                + '"PriceWizardUnit_Max":"' + $(this).children('td:nth-child(4)').text().trim().toLowerCase() + '",'
                + '"PriceWizardUnit_AdditionalInfo":"' + $(this).children('td:nth-child(5)').text().trim().toLowerCase() + '"'
                + '}';
            });
            _str += ']';
            $('#PriceWizard_PriceUnits').val(_str);
        }
    }

    var deleteRow = function (callback) {
        $('.delete-row').unbind('click').on('click', function () {
            $(this).parents('tr:first').remove();
            if (typeof callback == 'function') {
                callback();
            }
        });
    }

    var savePriceWizardSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            $('#openForm').trigger('click');
            $('#tblPriceUnits tbody').empty();
            PRICE.updateTblPrices();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    return {
        init: init,
        deleteRow: deleteRow,
        savePriceWizardSuccess: savePriceWizardSuccess
    }
}();