$(function () {
    FS.init();
});

var FS = function () {

    var init = function () {

        UI.alertDoubleClick();

        function FastSaleModel() {
            this.FastSaleInfo_Title = null;
            this.FastSaleInfo_FirstName = null;
            this.FastSaleInfo_LastName = null;
            this.FastSaleInfo_Email = null;
            this.FastSaleInfo_Phone = null;
            this.FastSaleInfo_Terminal = null;
            this.FastSaleInfo_PointOfSale = null;
            this.FastSaleInfo_Location = null;
            this.FastSaleInfo_Currency = null;
            this.FastSaleInfo_Language = null;
            this.FastSaleInfo_StayingAtPlace = null;
            this.FastSaleInfo_StayingAtOtherPlace = null;
            this.FastSaleInfo_RoomNumber = null;
            this.FastSaleInfo_LeadSource = null;
            this.CustomerID = null;
            this.Country = null;
            this.MarketingProgram = null;
            this.Subdivision = null;
            this.Source = null;
            this.OPCID = null;
            this.OPC = null;
            this.FrontOfficeGuestID = null;
            this.FrontOfficeResortID = null;
            this.TourID = null;
            this.TourDate = null;
        }

        UI.applyTextFormat();

        $.ajax({
            url: '/crm/MasterChart/QuickSaleDependantLists',
            cache: false,
            success: function (data) {
                UI.loadDependantFields(data);
            }
        });

        //trigger on each render to get info according the current selected terminals
        $.getJSON('/MasterChart/GetDDLData', { itemType: 'fastSaleTerminals' }, function (data) {
            $('#FastSaleInfo_Terminal').fillSelect(data, false);
            if (localStorage.Eplat_FastSale_Terminals != undefined && localStorage.Eplat_FastSale_Terminals != null) {
                $('#FastSaleInfo_Terminal option[value="' + UI.returnMostSelectedValue(localStorage.Eplat_FastSale_Terminals, 'Terminals') + '"]').attr('selected', true);
            }
            $('#FastSaleInfo_Terminal').trigger('change');
            if ($('#FastSaleInfo_Terminal option').length == 1) {
                $('#divTerminal').hide();
            }
            else {
                $('#divTerminal').show();
            }
        });

        if (localStorage.Eplat_FastSale_Currencies != undefined && localStorage.Eplat_FastSale_Currencies != null) {
            $('#FastSaleInfo_Currency option[value="' + UI.returnMostSelectedValue(localStorage.Eplat_FastSale_Currencies, 'Currencies') + '"]').attr('selected', true);
        }

        if (localStorage.Eplat_FastSale_Languages != undefined && localStorage.Eplat_FastSale_Languages != null) {
            $('#FastSaleInfo_Language option[value="' + UI.returnMostSelectedValue(localStorage.Eplat_FastSale_Languages, 'Languages') + '"]').attr('selected', true);
        }

        $('#FastSaleInfo_StayingAtPlace').multiselect({
            multiple: false,
            noneSelectedText: '--Select One--',
            selectedList: 1
        }).multiselectfilter();

        $('#FastSaleInfo_Language').on('change', function () {
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'titlesPerCulture', itemID: $(this).val() }, function (data) {
                $('#FastSaleInfo_Title').fillSelect(data);
            });
        });

        $('#FastSaleInfo_FirstName').focus();

        $('#FastSaleInfo_FirstName').on('blur', function () {
            $(this).removeClass('input-validation-error');
            if ($(this).val() == '') {
                $(this).val(' ');
                $('#spanFirstName').text($('#FastSaleInfo_Title option:selected').text());
            }
            else {
                $('#spanFirstName').text($(this).val());
            }
        });

        $('#FastSaleInfo_LastName').on('keyup', function () {
            $(this).removeClass('input-validation-error');
            $('#spanLastName').text($(this).val());
        });

        

        $('#FastSaleInfo_Terminal').on('change', function () {
            $(this).removeClass('input-validation-error');
            $.getJSON('/MasterChart/GetDDLData', { itemType: 'pointsOfSalePerTerminal', itemID: $(this).val() }, function (data) {
                $('#FastSaleInfo_PointOfSale').fillSelect(data, false);
                if (localStorage.Eplat_FastSale_PointsOfSale != undefined && localStorage.Eplat_FastSale_PointsOfSale != null) {
                    $('#FastSaleInfo_PointOfSale option[value="' + UI.returnMostSelectedValue(localStorage.Eplat_FastSale_PointsOfSale, 'PointsOfSale') + '"]').attr('selected', true);
                }
                $('#FastSaleInfo_PointOfSale').trigger('change');
            });

            //$.getJSON('/MasterChart/GetDDLData', { itemType: 'place', itemID: $(this).val() }, function (data) {
            //    $('#FastSaleInfo_StayingAtPlace').fillSelect(data);
            //    $('#FastSaleInfo_StayingAtPlace').multiselect('refresh');
            //});
        });

        $('#FastSaleInfo_StayingAtPlace').on('change', function () {
            if ($(this).val() == 'null') {
                $('#divStayingAtOtherPlace').show();
            }
            else {
                $('#FastSaleInfo_StayingAtOhterPlace').val('');
                $('#divStayingAtOtherPlace').hide();
            }
        })

        $('#FastSaleInfo_PointOfSale').on('change', function () {
            if ($(this).val() != null && $(this).val() != undefined) {
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'place', itemID: $('#FastSaleInfo_Terminal').val() }, function (data) {
                    $('#FastSaleInfo_StayingAtPlace').fillSelect(data, false);
                    //$('#FastSaleInfo_StayingAtPlace').multiselect('refresh');

                    if ($('#FastSaleInfo_PointOfSale').val() != undefined && $('#FastSaleInfo_StayingAtPlace option[value="' + $('#FastSaleInfo_PointOfSale option:selected').val().split('|')[1] + '"]').length > 0) {
                        //place of PoS exists in list of places
                        $('#FastSaleInfo_StayingAtPlace option[value="' + $('#FastSaleInfo_PointOfSale').val().split('|')[1] + '"]').attr('selected', true);
                    }
                    else {
                        $('#FastSaleInfo_StayingAtPlace option[value="0"]').attr('selected', true);
                    }
                    $('#FastSaleInfo_StayingAtPlace').multiselect('refresh');
                    $('#FastSaleInfo_StayingAtPlace').trigger('change');
                });

                if ($(this).val().split('|')[2] == 1) {
                    //place of PoS is hotel
                    $('#FastSaleInfo_LeadSource option[value="19"]').attr('selected', true);
                }
                else {
                    if (localStorage.Eplat_FastSale_LeadSources != undefined && localStorage.Eplat_FastSale_LeadSources != null) {
                        $('#FastSaleInfo_LeadSource option[value="' + UI.returnMostSelectedValue(localStorage.Eplat_FastSale_LeadSources, 'LeadSources') + '"]').attr('selected', true);
                    }
                }
            }
        });

        $('#btnResetCounters').on('click', function () {
            for (var key in localStorage) {
                if (key.indexOf('FastSale') != -1) {
                    localStorage.removeItem(key);
                }
            }
            alert("Reset Finished");
        });

        $('#FastSaleInfo_Language').trigger('change');

        //$('#ManifestSearch_Date').on('keydown', function (e) {
        //    e.preventDefault();
        //});

        //$('#ManifestSearch_Date').datepicker({
        //    dateFormat: 'yy-mm-dd',
        //    minDate: -22,
        //    maxDate: 0,
        //    onClose: function (dateText, inst) {
        //        console.log($('#ManifestSearch_Date').datepicker('getDate'));
        //        if (dateText != '') {
        //            localStorage.Eplat_AgencyManifest_Request = new Date($('#ManifestSearch_Date').datepicker('getDate'));
        //            PURCHASE.getAgencyManifest(dateText);
        //        }
        //    }
        //});

        //$('#btnShowManifest').on('click', function () {
        //    $('#ManifestSearch_Date').datepicker('setDate', COMMON.getDate());
        //    $('#ManifestSearch_Date').datepicker('refresh');
        //    //localStorage.Eplat_AgencyManifest_Request contiene la última fecha usada como parámetro  
        //    //comparar fecha usada en parámetro con fecha del campo del formulario.
        //    if (localStorage.Eplat_AgencyManifest_Request != undefined && localStorage.Eplat_AgencyManifest_Request != null && localStorage.Eplat_AgencyManifest_Request != '') {
        //        $('#manifestRequestDate').text(new Date(localStorage.Eplat_AgencyManifest_Request).toDateString());
        //        if (new Date(localStorage.Eplat_AgencyManifest_Request).toDateString() != new Date().toDateString()) {
        //            $('#ManifestSearch_Date').addClass('mb-warning');
        //        }
        //        else {
        //            $('#ManifestSearch_Date').removeClass('mb-warning');
        //        }
        //    }
        //    else {
        //        $('#ManifestSearch_Date').removeClass('mb-warning');
        //    }
        //    if (localStorage.Eplat_Agency_Manifest == undefined || localStorage.Eplat_Agency_Manifest == null || localStorage.Eplat_Agency_Manifest == '') {
        //        localStorage.Eplat_AgencyManifest_Request = new Date($('#ManifestSearch_Date').datepicker('getDate'));
        //        PURCHASE.getAgencyManifest($('#ManifestSearch_Date').val());
        //    }
        //    else {
        //        PURCHASE.renderAgencyManifest();
        //        $('#displayLastUpdate').text(localStorage.Eplat_AgencyManifest_Update);
        //    }
        //    $('.manifest-related').show();
        //});

        //$('#btnGetManifest').on('click', function () {
        //    $('#btnResetQuickSale').trigger('click');
        //    localStorage.Eplat_AgencyManifest_Request = new Date($('#ManifestSearch_Date').datepicker('getDate'));
        //    PURCHASE.getAgencyManifest($('#ManifestSearch_Date').val());
        //});

        $('#btnResetQuickSale').on('click', function () {
            $('#fastSaleFieldset').clearForm();
            $('#FastSaleInfo_FirstName').trigger('keyup');
            $('#FastSaleInfo_LastName').trigger('keyup');
            if (AMANIFEST.oManifestTable != undefined) {
                AMANIFEST.oManifestTable.$('tr.selected-row').removeClass('selected-row');
            }
        });

        $('#btnCancelFastSale').on('click', function () {
            $('#btnResetQuickSale').trigger('click');
            $.fancybox.close(true);
        });

        $('#btnSaveFastSale').on('click', function () {
            var model = new FastSaleModel();

            $('#fastSaleFieldset').find('input[data-val-required]').each(function (index, item) {
                if ($(this).val() == '' || $(this).val() == '0') {
                    $(this).addClass('input-validation-error');
                }
                else {
                    $(this).removeClass('input-validation-error');
                }
            });

            if ($('#fastSaleFieldset').find('input[data-val-required].input-validation-error').length == 0) {
                model.FastSaleInfo_Title = $('#FastSaleInfo_Title').val();
                model.FastSaleInfo_FirstName = $('#FastSaleInfo_FirstName').val();
                model.FastSaleInfo_LastName = $('#FastSaleInfo_LastName').val();
                model.FastSaleInfo_Email = $('#FastSaleInfo_Email').val();
                model.FastSaleInfo_Phone = $('#FastSaleInfo_Phone').val();
                model.FastSaleInfo_Terminal = $('#FastSaleInfo_Terminal').val();
                model.FastSaleInfo_PointOfSale = $('#FastSaleInfo_PointOfSale').val();
                model.FastSaleInfo_Location = $('#FastSaleInfo_Location').val();
                model.FastSaleInfo_Currency = $('#FastSaleInfo_Currency').val();
                model.FastSaleInfo_Language = $('#FastSaleInfo_Language').val();
                model.FastSaleInfo_StayingAtPlace = $('#FastSaleInfo_StayingAtPlace').val();
                model.FastSaleInfo_StayingAtOtherPlace = $('#FastSaleInfo_StayingAtOtherPlace').val();
                model.FastSaleInfo_RoomNumber = $('#FastSaleInfo_RoomNumber').val();
                model.FastSaleInfo_LeadSource = $('#FastSaleInfo_LeadSource').val();
                //manifest fields
                model.CustomerID = $('#CustomerID').val();
                model.Country = $('#Country').val();
                model.MarketingProgram = $('#MarketingProgram').val();
                model.Subdivision = $('#Subdivision').val();
                model.Source = $('#Source').val();
                model.OPCID = $('#OPCID').val();
                model.OPC = $('#OPC').val();
                model.FrontOfficeGuestID = $('#FrontOfficeGuestID').val();
                model.FrontOfficeResortID = $('#FrontOfficeResortID').val();
                model.TourID = $('#TourID').val();
                model.TourDate = $('#TourDate').val();

                var jsonObj = JSON.stringify(model);

                $.ajax({
                    url: '/MasterChart/SaveFastSale',
                    cache: false,
                    type: 'POST',
                    dataType: 'json',
                    data: jsonObj,
                    traditional: true,
                    contentType: 'application/json; charset=utf-8',
                    beforeSend: function (xhr) {
                        UI.checkForPendingRequests(xhr);
                    },
                    success: function (data) {
                        var duration = data.ResponseType < 0 ? data.ResponseType : null;
                        if (data.ResponseType > 0) {
                            //update localStorage values
                            UI.updateLocalStorageCounter('Terminals', $('#FastSaleInfo_Terminal').val());
                            UI.updateLocalStorageCounter('PointsOfSale', $('#FastSaleInfo_PointOfSale').val());
                            UI.updateLocalStorageCounter('Currencies', $('#FastSaleInfo_Currency').val());
                            UI.updateLocalStorageCounter('Languages', $('#FastSaleInfo_Language').val());
                            UI.updateLocalStorageCounter('LeadSources', $('#FastSaleInfo_LeadSource').val());
                            //trigger close button click
                            $('#btnCancelFastSale').trigger('click');
                            //display message of response (this probably will not be neccesary, it will not be visible because the window update
                            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                            //add hash to url and update window
                            window.location.hash = 'purchaseid=' + data.ItemID.purchaseID;
                            $('#frmLeadSearch').find('input:reset').click();
                            $('#Search_LeadID').val(data.ItemID.leadID);
                            $('#frmLeadSearch').find('input:submit').click();
                            $('#Search_LeadID').val('');
                        }
                        else {
                            $('#btnCancelFastSale').trigger('click');
                            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                        }
                    }
                });
            }
        });
    }

    var returnMostSelectedValue = function (localList, item) {
        var _selected = 0;
        var _value = 0;
        var json = $.parseJSON(localList);
        $.each(json[item], function (index, item) {
            if (parseInt(item.Selected) > _selected) {
                _selected = item.Selected;
                _value = item.Value;
            }
        });

        return _value;
    }

    function updateLocalStorageFastSaleLists(item, value) {
        var found = false;
        var list;
        switch (item) {
            case 'Terminals': {
                if (localStorage.Eplat_FastSale_Terminals != null && localStorage.Eplat_FastSale_Terminals != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_Terminals);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_Terminals = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_Terminals = '{"Terminals" : [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'PointsOfSale': {
                if (localStorage.Eplat_FastSale_PointsOfSale != null && localStorage.Eplat_FastSale_PointsOfSale != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_PointsOfSale);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_PointsOfSale = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_PointsOfSale = '{"PointsOfSale" :[{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'Currencies': {
                if (localStorage.Eplat_FastSale_Currencies != null && localStorage.Eplat_FastSale_Currencies != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_Currencies);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_Currencies = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_Currencies = '{"Currencies" : [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'Languages': {
                if (localStorage.Eplat_FastSale_Languages != null && localStorage.Eplat_FastSale_Languages != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_Languages);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_Languages = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_Languages = '{"Languages" : [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'LeadSources': {
                if (localStorage.Eplat_FastSale_LeadSources != null && localStorage.Eplat_FastSale_LeadSources != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_LeadSources);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_LeadSources = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_LeadSources = '{"LeadSources" : [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            default: {
                list = null;
            }
        }
        //if (list != null && list != undefined) {
        //    var json = $.parseJSON(JSON.stringify(list));
        //    $.each(json[item], function (index, item) {
        //        if (item.Value == value) {
        //            item.Selected = parseInt(item.Selected) + 1;
        //            found = true;
        //        }
        //    });
        //    if (!found) {
        //        json[item].push({ "Value": '"' + value + '"', "Selected": "1" });
        //    }
        //    list = JSON.stringify(json);
        //}
        //else {
        //    list[item] = { "Value": '"' + value + '"', "Selected": "1" };
        //    //list = '"' + item + '":[{"Value":"'+ value +'","Selected":"1"}]';
        //}
        //switch (item) {
        //    case 'Terminals': {
        //        localStorage.Eplat_FastSale_Terminals = list;
        //        break;
        //    }
        //    case 'PointsOfSale': {
        //        localStorage.Eplat_FastSale_PointsOfSale = list;
        //        break;
        //    }
        //    case 'Currencies': {
        //        localStorage.Eplat_FastSale_Currencies = list;
        //        break;
        //    }
        //    case 'Languages': {
        //        localStorage.Eplat_FastSale_Languages = list;
        //        break;
        //    }
        //    case 'LeadSources': {
        //        localStorage.Eplat_FastSale_LeadSources = list;
        //        break;
        //    }
        //    default: {
        //        list = null;
        //    }
        //}
    }

    return {
        init: init,
        returnMostSelectedValue: returnMostSelectedValue
    }
}();
