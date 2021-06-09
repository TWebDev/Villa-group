
var PREARRIVAL = function () {
    var oTable, oBillingsTable, oReservationsTable, oOptionsTable, oRCOptionsTable, oFlightsTable, oPaymentsTable;
    var Emails, Phones, Member, Billings, Interactions, Reservations, Presentation, Options, RCOptions, Flights, Payments, Letters;
    var Layouts = [];
    var LeadGroups = [];
    var Airlines = [];
    var Filters = { Tables: [], Ids: [], Fields: [], Names: [], Types: [], Properties: [] };
    var Columns = { Text: [], Value: [], Ids: [] };
    //var leads = new Array();

    var init = function () {

        UI.applyMultiselect();

        UI.applyDatePicker();
        PREARRIVAL.clearForms();
        $('#preArrivalSearchTabs').tabs();

        $('.clear-form').on('click', function () {
            $(this).parents('section:first').find('form').each(function () {
                $(this).clearForm();
            });
            $(this).parents('section:first').find('.selected-row').each(function () {
                $(this).removeClass('selected-row secondary primary');
            });
            $(this).parents('section:first').find('.secondary-selected-row-dependent').not('tbody:first,span').each(function () {
                $(this).empty();
            });
            UI.scrollTo($(this).parents('section:first').find('form:first').attr('id'), null);
        });

        $('#btnImportFromFront').on('click', function () {
            //window.open('https://eplat.villagroup.com/crm/prearrival/renderimport');
            window.open(window.location.origin + '/crm/prearrival/renderimport');
        });

        $('.get-history').on('click', function (e) {
            var srcID = $('#' + $(e.target).data('reference-id')).val();
            $.ajax({
                url: '/Prearrival/GetHistory',
                type: 'POST',
                data: { referenceID: srcID },
                success: function (data) {
                    var html = '';
                    var json = $.parseJSON(data.data);
                    var headers = getHeaders(json);

                    $.fancybox({
                        //type: 'html',
                        content: createTable(json, headers)
                    });

                    function createTable(json, cols) {
                        var table = $('<table></table>');
                        var th = $('<tr></tr>');

                        for (var i = 0; i < cols.length; i++) {
                            //th.append('<th>' + cols[i].split('_').join(' ') + '</th>');
                            th.append('<th>' + (cols[i].indexOf('_') != -1 ? (cols[i].split('_')[1]) : cols[i]) + '</th>');
                        }
                        table.append(th);

                        for (var i = 0; i < json.length; i++) {
                            var item = json[i];
                            var tr = $('<tr></tr>');
                            for (var j = 0; j < cols.length; j++) {
                                var colName = cols[j];
                                //tr.append('<td>' + item[colName] + '</td>');
                                //****
                                var str = '';
                                if (typeof (item[colName]) === 'object') {
                                    if (item[colName] !== null) {
                                        var _json = $.parseJSON('[' + JSON.stringify(item[colName]) + ']');
                                        var hdrs = getHeaders(_json);
                                        var tbl = createTable(_json, hdrs);
                                        //str += tbl;
                                        tr.append('<td>' + tbl.prop('outerHTML') + '</td>');
                                    }
                                    else {
                                        tr.append('<td></td>');
                                    }
                                }
                                else {
                                    //str = item[colName];
                                    tr.append('<td>' + item[colName] + '</td>');
                                }
                                //tr.append('<td>' + str + '</td>');
                                //****
                            }
                            table.append(tr);
                        }
                        return table;
                    }
                    function getHeaders(json) {
                        var cols = new Array();
                        var str = json[0];
                        for (var key in str) {
                            cols.push(key);
                        }
                        return cols;
                    }
                }
            });

        });

        $('.click-to-call').on('click', function (e) {
            var srcID = $('#' + $(e.target).data('reference-id')).val();
            var phoneID = $(e.target).data('reference-phone-id') != undefined ? $(e.target).data('reference-phone-id') : null;
            $.ajax({
                url: '/Prearrival/ClickToCall',
                type: 'POST',
                data: { leadID: srcID, phoneID: phoneID },
                success: function (data) {
                    UI.messageBox(data.Type, data.Message, null, null);
                }
            });
        });

        $('.get-from-rc').on('click', function (e) {
            var srcID = $('#' + $(e.target).data('reference-id')).val();
            $.ajax({
                url: '/Prearrival/GetDataFromRC',
                type: 'POST',
                data: { leadID: srcID },
                success: function (data) {
                    if (data.Type == 1) {
                        var info = $.parseJSON(data.ItemID);
                        $('#Info_FirstName').val(info.firstName);
                        $('#Info_LastName').val(info.lastName);
                    }
                    UI.messageBox(data.Type, data.Message, null, null);
                }
            });
        });

        $('.unload-lead').on('click', function (e) {
            var srcID = $('#' + $(e.target).data('reference-id')).val();
            $.ajax({
                url: '/Prearrival/UnloadLead',
                type: 'POST',
                data: { leadID: srcID },
                success: function (data) {
                    UI.messageBox(data.Type, data.Message, null, null);
                }
            });
        });

        $('.unload-reservation').on('click', function (e) {
            var srcID = $('#' + $(e.target).data('reference-id')).val();
            $.ajax({
                url: '/Prearrival/UnloadReservation',
                type: 'POST',
                data: { reservationID: srcID },
                success: function (data) {
                    UI.messageBox(data.Type, data.Message, null, null);
                }
            });
        });

        $('#btnToMassActions').on('click', function () {
            $.fancybox({
                type: 'html',
                href: $('#fdsPreArrivalInfo'),
                centerOnScroll: true,
                scrolling: 'no'
            });
        });

        $.getJSON('/PreArrival/GetFields', {}, function (data) {//PreArrival

            $.each(data, function (index, item) {
                PREARRIVAL.Filters.Tables[index] = item.TableName;
                PREARRIVAL.Filters.Ids[index] = item.FieldID;
                PREARRIVAL.Filters.Fields[index] = item.FieldName;
                PREARRIVAL.Filters.Names[index] = item.DisplayName;
                PREARRIVAL.Filters.Types[index] = item.FieldType;
                PREARRIVAL.Filters.Properties[index] = item.PropertyName;
                PREARRIVAL.Columns.Ids[index] = item.FieldID;
                //PREARRIVAL.Columns.Value[index] = item.FilterID;
                PREARRIVAL.Columns.Text[index] = item.DisplayName;
            });

            $('#Search_SearchFilters').bind('keydown', function (e) {
                if (e.keyCode === $.ui.keyCode.TAB && $(this).data('ui-autocomplete').menu.active) {
                    e.preventDefault();
                }
            }).autocomplete({
                source: PREARRIVAL.Filters.Names,
                minLength: 0,
                position: { my: 'right top', at: 'right bottom' },
                autoFocus: true,
                select: function (e, ui) {
                    $(this).val(ui.item.value);
                },
                _resizeMenu: function () {
                    this.menu.element.outerWidth(50);
                }
            }).on('focus', function () {
                $(this).autocomplete('search', '');
            });

            $('#Search_ColumnHeaders').bind('keydown', function (e) {
                if (e.keyCode === $.ui.keyCode.TAB && $(this).data('ui-autocomplete').menu.active) {
                    e.preventDefault();
                }
            }).autocomplete({
                source: PREARRIVAL.Columns.Text,
                minLength: 0,
                position: { my: 'right top', at: 'right bottom' },
                autoFocus: true,
                select: function (e, ui) {
                    $(this).val(ui.item.value);
                }
            }).on('focus', function () {
                $(this).autocomplete('search', '');
            });
        });

        $.getJSON('/PreArrival/GetDDLData', { itemType: 'airlines' }, function (data) {
            $.each(data, function (index, item) {
                PREARRIVAL.Airlines[index] = { "value": item.Value, "label": item.Text };
            });

            $('#FlightInfo_AirlineName').bind('keydown', function (e) {
                if (e.keyCode === $.ui.keyCode.TAB && $(this).data('ui-autocomplete').menu.active) {
                    e.preventDefault();
                }
            }).autocomplete({
                source: PREARRIVAL.Airlines,
                minLength: 0,
                position: { my: 'right top', at: 'right bottom' },
                autoFocus: true,
                select: function (e, ui) {
                    e.preventDefault();
                    $('#FlightInfo_Airline').val(ui.item.value);
                    $('#FlightInfo_AirlineName').val(ui.item.label);

                }
            }).on('focus', function () {
                $(this).autocomplete('search', '');
            });
        });

        PREARRIVAL.loadReportLayouts();

        $.ajax({
            url: '/PreArrival/GetDependantFields',
            cache: false,
            success: function (data) {
                UI.loadDependantFields(data);
            }
        });

        $('#PaymentInfo_DateSaved').datepicker({
            'changeMonth': true,
            'changeYear': true,
            'dateFormat': 'yy-mm-dd',
            maxDate: 0,
            constrainInput: true
            //onClose: function (dateText, inst) {
            //    $('#PurchasePayment_CardType').trigger('change');
            //}
        }).datepicker('setDate', new Date(COMMON.serverDateTime)).on('keydown', function (e) { e.preventDefault(); });

        $('#FlightInfo_FlightDateTime').each(function () {
            $(this).datetimepicker({
                dateFormat: 'yy-mm-dd',
                timeFormat: 'HH:mm',
                timeOnly: false,
                stepMinute: 1
            });
        });

        $('#btnNewLead').on('click', function () {
            $('#frmPreArrival').clearForm();
            $('#frmReservationInfo').clearForm();
            if (!$('#fdsPreArrivalInfo').hasClass('fds-active')) {
                $('#fdsPreArrivalInfo legend').trigger('click');
            }
        });

        $('#addFilterToForm').on('click', function () {
            var index = PREARRIVAL.Filters.Names.indexOf($('#Search_SearchFilters').val());
            renderFormField([PREARRIVAL.Filters.Ids[index]]);
            $('#Search_SearchFilters').val('');
        });

        $('#addColumnToForm').on('click', function () {
            var index = PREARRIVAL.Columns.Text.indexOf($('#Search_ColumnHeaders').val());
            if ($('#' + PREARRIVAL.Columns.Value[index]).length > 0) {
                UI.messageBox(-1, 'Column already added', null, null);
            }
            else {
                renderColumnHeader([PREARRIVAL.Columns.Ids[index]]);
            }
            $('#Search_ColumnHeaders').val('');
        });

        $('#btnSaveLayout').on('click', function () {
            var fields = '';

            $('#searchTab .dynamic-field').each(function () {
                fields += (fields == '' ? '' : ',') + $(this).attr('data-field-id');
            });

            $('#columnsTab .dynamic-field').each(function () {
                fields += (fields == '' ? '' : ',') + $(this).attr('data-field-id');
            });

            $('#Search_Fields').val(fields);

            var searchModel = new function PreArrivalSearchModel() {
                this.Search_Fields = $('#Search_Fields').val();
                this.Search_ReportLayout = $('#Search_ReportLayout').val();
                this.Search_LayoutName = $('#Search_LayoutName').val();
                this.Search_Public = $('input:radio[name="Search_Public"]:checked').val();
            }

            var jsonObj = JSON.stringify(searchModel);

            if ($('#frmSaveLayout').valid()) {
                $.ajax({
                    url: '/PreArrival/SaveLayout',
                    cache: false,
                    type: 'POST',
                    data: jsonObj,
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    traditional: true,
                    success: function (data) {
                        var duration = data.ResponseType < 0 ? data.ResponseType : null;
                        if (data.ResponseType > 0) {
                            PREARRIVAL.loadReportLayouts();
                        }
                        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                    }
                });
            }
        });

        $('#btnNewLayout').on('click', function () {
            $('#frmSaveLayout').clearForm();
            $('#searchTab .dynamic-field').remove();
            $('#columnsTab .dynamic-field').remove();
        });

        $('#btnCopyLayout').on('click', function () {
            if ($('#Search_ReportLayout').val() != '0' && $('#Search_ReportLayout').val() != undefined) {
                $.ajax({
                    url: '/PreArrival/CopyLayout',
                    cache: false,
                    type: 'POST',
                    data: { reportLayoutID: $('#Search_ReportLayout').val() },
                    success: function (data) {
                        var duration = data.ResponseType < 0 ? data.ResponseType : null;
                        if (data.ResponseType > 0) {
                            var value = data.ItemID.layoutID + '|' + data.ItemID.fields + '|' + $('input:radio[name="Search_Public"]:checked').val().toString().toLowerCase();
                            PREARRIVAL.Layouts.push({ "value": value, "label": data.ItemID.layout });
                            var ui = { item: { value: value, label: data.ItemID.layout } };
                            PREARRIVAL.selectLayout(ui);
                        }
                        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                    }
                });
            }
            else {
                UI.messageBox(0, 'No Layout is selected', null, null);
            }
        });

        $('#btnDeleteLayout').on('click', function () {
            if ($('#Search_ReportLayout').val() != undefined && $('#Search_ReportLayout').val() != '0') {
                $.ajax({
                    url: '/PreArrival/DeleteLayout',
                    cache: false,
                    type: 'POST',
                    data: { reportLayoutID: $('#Search_ReportLayout').val() },
                    success: function (data) {
                        var duration = data.ResponseType < 0 ? data.ResponseType : null;
                        if (data.ResponseType > 0) {
                            $('#btnNewLayout').trigger('click');
                            PREARRIVAL.loadReportLayouts();
                        }
                        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                    }
                });
            }
        });

        $('#btnSearchPreArrival').on('click', function () {
            var _str = '';
            $('#columnsTab .dynamic-field').each(function () {
                var index = PREARRIVAL.Columns.Ids.indexOf($(this).attr('data-field-id').split('_')[1]);
                var id = $(this).attr('id').substr($(this).attr('id').lastIndexOf('.') + 1);
                _str += (_str == '' ? '' : ',') + id + '/' + PREARRIVAL.Columns.Text[index];
            });
            $('#Search_Columns').val(_str);

            //pending validate before submit
            $('#frmSearchPreArrival').submit();
        });

        $('#btnSaveContinueLead').on('click', function () {
            PREARRIVAL.saveContinueFlag = true;
            $('#frmPreArrival').submit();
        });

        $('#btnDuplicateLead').on('click', function () {
            $('#Info_DuplicateLead').val(true);
            $('#frmPreArrival').submit();
        });

        $('.email-add').on('click', function () {
            var _data = $.parseJSON($('#PreArrivalEmails').val());
            var index = _data != null ? _data.map(function (_item) { return _item.EmailsInfo_Main; }).indexOf(true) : -1;
            if (index != -1 && $('#PreArrivalEmailsInfoModel_EmailsInfo_Main')[0].checked) {
                if ($('#PreArrivalEmailsInfoModel_EmailsInfo_Email').val() != '' && $('#PreArrivalEmailsInfoModel_EmailsInfo_Email').hasClass('valid')) {
                    _data[index].EmailsInfo_Main = false;
                    $('#PreArrivalEmails').val(JSON.stringify(_data));
                    var data = [{ 'EmailsInfo_LeadEmailID': 0, 'EmailsInfo_LeadID': $('#Info_LeadID').val(), 'EmailsInfo_Email': $('#PreArrivalEmailsInfoModel_EmailsInfo_Email').val(), 'EmailsInfo_Main': $('#PreArrivalEmailsInfoModel_EmailsInfo_Main')[0].checked }];
                    renderEmail(data, true);
                }
            }
            else {
                if ($('#PreArrivalEmailsInfoModel_EmailsInfo_Email').val() != '' && $('#PreArrivalEmailsInfoModel_EmailsInfo_Email').hasClass('valid')) {
                    var data = [{ 'EmailsInfo_LeadEmailID': 0, 'EmailsInfo_LeadID': $('#Info_LeadID').val(), 'EmailsInfo_Email': $('#PreArrivalEmailsInfoModel_EmailsInfo_Email').val(), 'EmailsInfo_Main': $('#PreArrivalEmailsInfoModel_EmailsInfo_Main')[0].checked }];
                    renderEmail(data, true);
                }
            }
        });

        $('.email-edit').on('click', function () {
            var $tr = $(this).parents('tr:first');
            var array = new Array();
            //var flag = false;
            $('#tblPreArrivalEmails tbody tr:not(.selected-row)').each(function () {
                //if ($(this).children('td:nth-child(3)').attr('data-val') == 'true') {
                //    flag = true;
                //}
                if ($(this).children('td:nth-child(3)').attr('data-val') == 'true' && $('#PreArrivalEmailsInfoModel_EmailsInfo_Main')[0].checked) {
                    $(this).children('td:nth-child(3)').attr('data-val', 'false');
                    $(this).children('td:nth-child(3)').html('');
                }
                var data = { 'EmailsInfo_LeadEmailID': $(this).children('td:nth-child(1)').attr('data-val'), 'EmailsInfo_LeadID': $('#Info_LeadID').val(), 'EmailsInfo_Email': $(this).children('td:nth-child(2)').attr('data-val'), 'EmailsInfo_Main': $(this).children('td:nth-child(3)').attr('data-val') };
                array.push(data);
            });
            //if (flag == false || $tr.children('td:nth-child(3)').children('input:checkbox')[0].checked == false)
            {
                var bTr = $tr.parents('table:first').children('tbody').find('tr.selected-row');
                var data = { 'EmailsInfo_LeadEmailID': $tr.children('td:nth-child(1)').children('input:hidden').val(), 'EmailsInfo_LeadID': $('#Info_LeadID').val(), 'EmailsInfo_Email': $tr.children('td:nth-child(2)').children('input:text').val(), 'EmailsInfo_Main': $tr.children('td:nth-child(3)').children('input:checkbox')[0].checked };
                array.push(data);
                $('#PreArrivalEmails').val(JSON.stringify(array));
                bTr.children('td:nth-child(2)').text($('#PreArrivalEmailsInfoModel_EmailsInfo_Email').val());
                bTr.children('td:nth-child(3)').html(($('#PreArrivalEmailsInfoModel_EmailsInfo_Main')[0].checked ? '<i class="material-icons sm">bookmark</i>' : ''));
                bTr.removeClass('editing-row selected-row');
                $tr.find('.item-edition').toggle();
                $tr.children('td:nth-child(1)').children('input:hidden').val('');
                $tr.children('td:nth-child(2)').children('input:text').val('');
                $tr.children('td:nth-child(3)').children('input:checkbox')[0].checked = false;
            }
            //else {
            //    UI.messageBox(-1, 'Only one email address could be marked as main', null, null);
            //}
        });

        $('.phone-add').on('click', function () {
            var _data = $.parseJSON($('#PreArrivalPhones').val());
            var index = _data != null ? _data.map(function (_item) { return _item.PhonesInfo_Main; }).indexOf(true) : -1;
            if (index != -1 && $('#PreArrivalPhonesInfoModel_PhonesInfo_Main')[0].checked) {
                //UI.messageBox(-1, 'Only one phone number could be marked as main', null, null);
                if ($('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneNumber').val() != '' && $('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneNumber').hasClass('valid')) {
                    _data[index].PhonesInfo_Main = false;
                    $('#PreArrivalPhones').val(JSON.stringify(_data));
                    var data = [{ 'PhonesInfo_LeadPhoneID': 0, 'PhonesInfo_LeadID': $('#Info_LeadID').val(), 'PhonesInfo_PhoneType': $('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneType').val(), 'PhonesInfo_PhoneNumber': $('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneNumber').val(), 'PhonesInfo_ExtensionNumber': $('#PreArrivalPhonesInfoModel_PhonesInfo_ExtensionNumber').val(), 'PhonesInfo_DoNotCall': $('#PreArrivalPhonesInfoModel_PhonesInfo_DoNotCall')[0].checked, 'PhonesInfo_Main': $('#PreArrivalPhonesInfoModel_PhonesInfo_Main')[0].checked }];
                    renderPhone(data, true);
                }
            }
            else {
                if ($('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneNumber').val() != '' && $('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneNumber').hasClass('valid')) {
                    var data = [{ 'PhonesInfo_LeadPhoneID': 0, 'PhonesInfo_LeadID': $('#Info_LeadID').val(), 'PhonesInfo_PhoneType': $('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneType').val(), 'PhonesInfo_PhoneNumber': $('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneNumber').val(), 'PhonesInfo_ExtensionNumber': $('#PreArrivalPhonesInfoModel_PhonesInfo_ExtensionNumber').val(), 'PhonesInfo_DoNotCall': $('#PreArrivalPhonesInfoModel_PhonesInfo_DoNotCall')[0].checked, 'PhonesInfo_Main': $('#PreArrivalPhonesInfoModel_PhonesInfo_Main')[0].checked }];
                    renderPhone(data, true);
                }
            }
        });

        $('.phone-edit').on('click', function () {
            var $tr = $(this).parents('tr:first');
            var array = new Array();
            //var flag = false;
            $('#tblPreArrivalPhones tbody tr:not(.selected-row)').each(function () {
                //if ($(this).children('td:nth-child(6)').attr('data-val') == 'true') {
                //    flag = true;
                //}
                if ($(this).children('td:nth-child(6)').attr('data-val') == 'true' && $('#PreArrivalPhonesInfoModel_PhonesInfo_Main')[0].checked) {
                    $(this).children('td:nth-child(6)').attr('data-val', 'false');
                    $(this).children('td:nth-child(6)').html('');
                }
                var data = { 'PhonesInfo_LeadPhoneID': $(this).children('td:nth-child(1)').attr('data-val'), 'PhonesInfo_LeadID': $('#Info_LeadID').val(), 'PhonesInfo_PhoneType': $(this).children('td:nth-child(2)').attr('data-val'), 'PhonesInfo_PhoneNumber': $(this).children('td:nth-child(3)').attr('data-val'), 'PhonesInfo_ExtensionNumber': $(this).children('td:nth-child(4)').attr('data-val'), 'PhonesInfo_DoNotCall': $(this).children('td:nth-child(5)').attr('data-val'), 'PhonesInfo_Main': $(this).children('td:nth-child(6)').attr('data-val') };
                array.push(data);
            });

            var bTr = $tr.parents('table:first').children('tbody').find('tr.selected-row');
            //if (flag == false || $tr.children('td:nth-child(6)').children('input:checkbox')[0].checked == false)
            {
                var data = { 'PhonesInfo_LeadPhoneID': $tr.children('td:nth-child(1)').children('input:hidden').val(), 'PhonesInfo_LeadID': $('#Info_LeadID').val(), 'PhonesInfo_PhoneType': $tr.children('td:nth-child(2)').children('select').children('option:selected').val(), 'PhonesInfo_PhoneNumber': $tr.children('td:nth-child(3)').children('input:text').val(), 'PhonesInfo_ExtensionNumber': $tr.children('td:nth-child(4)').children('input:text').val(), 'PhonesInfo_DoNotCall': $tr.children('td:nth-child(5)').children('input:checkbox')[0].checked, 'PhonesInfo_Main': $tr.children('td:nth-child(6)').children('input:checkbox')[0].checked };
                array.push(data);
                $('#PreArrivalPhones').val(JSON.stringify(array));
                bTr.children('td:nth-child(2)').text($('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneType option:selected').text());
                bTr.children('td:nth-child(3)').text($('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneNumber').val());
                bTr.children('td:nth-child(4)').text($('#PreArrivalPhonesInfoModel_PhonesInfo_ExtensionNumber').val());
                bTr.children('td:nth-child(5)').html(($('#PreArrivalPhonesInfoModel_PhonesInfo_DoNotCall')[0].checked ? '<i class="material-icons sm">bookmark</i>' : ''));
                bTr.children('td:nth-child(6)').html(($('#PreArrivalPhonesInfoModel_PhonesInfo_Main')[0].checked ? '<i class="material-icons sm">bookmark</i>' : ''));
                bTr.removeClass('editing-row selected-row');
                $tr.find('.item-edition').toggle();
                $tr.children('td:nth-child(1)').children('input:hidden').val('');
                $tr.children('td:nth-child(3)').children('input:text').val('');
                $tr.children('td:nth-child(4)').children('input:text').val('');
                $tr.children('td:nth-child(5)').children('input:checkbox')[0].checked = false;
                $tr.children('td:nth-child(6)').children('input:checkbox')[0].checked = false;
            }
            //else {
            //UI.messageBox(-1, 'Only one phone number could be marked as main', null, null);
            //}
        });

        $('#BillingInfo_CardExpiry').datepicker({
            dateFormat: 'mm/yy',
            changeMonth: true,
            changeYear: true,
            minDate: 0
        });

        $('#BillingInfo_CardNumber').on('keyup', function () {
            if ($(this).val().length == 4 || $(this).val().length == 9 || $(this).val().length == 14) {
                $(this).val($(this).val() + '-');
            }
        });

        $('#PaymentInfo_TransactionType').on('change', function () {
            if ($(this).val() == '1') {
                $('.refund-dependent').hide();
            }
            else if ($(this).val() == '2') {
                $('.refund-dependent').show();
                $('input:radio[name="PaymentInfo_MadeByEplat"]')[1].checked = true;
                $.ajax({
                    url: '/PreArrival/InvoicesToRefund',
                    cache: false,
                    type: 'POST',
                    data: { reservationID: $('#ReservationInfo_ReservationID').val() },
                    success: function (data) {
                        $('#PaymentInfo_InvoiceToRefund').fillSelect(data);
                    }
                });
            }
        });

        $('#PaymentInfo_PaymentType').on('change', function (e, params) {
            $('#PaymentInfo_BillingInfo option[value="0"]').attr('selected', true).trigger('change');
            $('.credit-card-dependent').hide();
            $('.points-redemption-dependent').hide();
            $('.referrals-dependent').hide();
            $('#PaymentInfo_Quantity').val('');
            $('#PaymentInfo_Quantity').trigger('keyup');
            localStorage.Eplat_CreateBilling_Flag = false;
            if ($(this).val() == 2) {
                //credit card
                $('.credit-card-dependent').show();
                if (params != undefined) {
                    $('#PaymentInfo_BillingInfo option[value="' + params.billing + '"]').attr('selected', true).trigger('change');
                }
                else {
                    $('#PaymentInfo_BillingInfo option:first').attr('selected', true).trigger('change');
                }
            }
            else if ($(this).val() == 7) {
                //points
                $.ajax({
                    url: '/PreArrival/GetPointsRedemptionRate',
                    cache: false,
                    type: 'POST',
                    data: { id: $('#PaymentInfo_ReservationID').val() },
                    success: function (data) {
                        $('#PaymentInfo_PointsRates').val(JSON.stringify(data));
                    }
                });
                $('.points-redemption-dependent').show();

            }
            else if ($(this).val() == 8) {
                $('.referrals-dependent').show();
                $('#lblReferralCredits').text('$');
                if ($('#ReservationInfo_FrontContractNumber').val() != '' || $('#PreArrivalMemberInfoModel_MemberInfo_MemberNumber').val() != '') {
                    $.ajax({
                        url: '/Public/GetAvailableAccountCredits',
                        cache: false,
                        type: 'POST',
                        data: {
                            account: $('#ReservationInfo_FrontContractNumber').val() != '' ? $('#ReservationInfo_FrontContractNumber').val() : $('#PreArrivalMemberInfoModel_MemberInfo_MemberNumber').val()
                        },
                        success: function (data) {
                            if (data.Status == 1) {
                                $('#lblReferralCredits').text('$' + data.Amount);
                            }
                        }
                    });
                }
                else {
                    UI.messageBox(0, 'Contract Number invalid', null, null);
                }
            }
            else {
                //$('#PaymentInfo_BillingInfo option[value="0"]').attr('selected', true).trigger('change');
                //$('.credit-card-dependent').hide();
                //$('.points-redemption-dependent').hide();
                //localStorage.Eplat_CreateBilling_Flag = false;
            }
        });

        $('#updateReferralCredits').on('click', function (e) {
            $('#lblReferralCredits').text('$');
            if ($('#ReservationInfo_FrontContractNumber').val() != '' || $('#PreArrivalMemberInfoModel_MemberInfo_MemberNumber').val() != '') {
                $.ajax({
                    url: '/Public/GetAvailableAccountCredits',
                    cache: false,
                    type: 'POST',
                    data: {
                        account: $('#ReservationInfo_FrontContractNumber').val() != '' ? $('#ReservationInfo_FrontContractNumber').val() : $('#PreArrivalMemberInfoModel_MemberInfo_MemberNumber').val()
                    },
                    success: function (data) {
                        if (data.Status == 1) {
                            $('#lblReferralCredits').text('$' + data.Amount);
                        }
                    }
                });
            }
            else {
                UI.messageBox(0, 'Contract Number invalid', null, null);
            }
        });

        $('#PaymentInfo_BillingInfo').on('change', function () {
            switch ($(this).val()) {
                case '-1': case -1: {
                    localStorage.Eplat_CreateBilling_Flag = false;
                    $('.cc-reference-dependent').show();
                    break;
                }
                case 'null': case null: {
                    localStorage.Eplat_CreateBilling_Flag = true;
                    UI.messageBox(0, 'You will be redirected to the corresponding form in order to save it.', null, null);
                    setTimeout(function () {
                        UI.scrollTo('btnNewBillingInfo', null);
                    }, 3000);
                    break;
                }
                //case '0': case 0: {
                default: {
                    console.log($(this).val());
                    if ($(this).val() >= 0) {
                        localStorage.Eplat_CreateBilling_Flag = false;
                        $('.cc-reference-dependent').hide();
                        $('#PaymentInfo_CCReferenceNumber').val('');
                        $('#PaymentInfo_CCType option[value="0"]').attr('selected', true);
                    }
                    break;
                }
            }
            //if (($(this).val() == 'null' || $(this).val() == null) && $('#PaymentInfo_PaymentType option:selected').val() == 2) {
            //    //new billing info
            //    localStorage.Eplat_CreateBilling_Flag = true;
            //    UI.messageBox(0, 'You will be redirected to the corresponding form in order to save it.', null, null);
            //    setTimeout(function () {
            //        //$('#btnNewBillingInfo').trigger('click');
            //        UI.scrollTo('btnNewBillingInfo', null);
            //    }, 3000);
            //}
            //else {
            //    localStorage.Eplat_CreateBilling_Flag = false;
            //}
        });

        $('#btnNewPayment').on('click', function () {
            //$('.refund-dependent').hide();
            $('#PaymentInfo_TransactionType option[value="1"]').attr('selected', true).trigger('change');
        });

        $('#btnNewRefund').on('click', function () {
            //$('.refund-dependent').show();
            $('#PaymentInfo_TransactionType option[value="2"]').attr('selected', true).trigger('change');
        });

        $('#btnApplyPendingCharges').on('click', function () {
            var ids = '';
            PREARRIVAL.oPaymentsTable.$('tr[data-pending-charge="true"]').each(function () {
                ids += (ids == '' ? '' : ',') + $(this).attr('id');
            });
            $.ajax({
                url: '/PreArrival/ApplyPendingCharges',
                cache: false,
                type: 'POST',
                data: { id: ids },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    $.each(data.ItemID.id, function (index, item) {
                        PREARRIVAL.oPaymentsTable.$('tr#' + item.Value).removeAttr('data-pending-charge');
                        PREARRIVAL.oPaymentsTable.$('tr#' + item.Value).children('td:nth-child(8)').html(item.Text);
                        if (item.Text == 'CC Approved') {
                            PREARRIVAL.oPaymentsTable.$('tr#' + item.Value).children('td:nth-child(8)').addClass('success');
                        }
                    });
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#ReservationInfo_Destination').on('change', function () {
            $('#FlightInfo_Destination').val($(this).val());
        });

        $('#OptionInfo_Quantity').on('keyup', function () {
            //var _total = !isNaN(parseFloat($('#OptionInfo_Price option:selected').val())) && !isNaN(parseFloat($(this).val())) ? (parseFloat($('#OptionInfo_Price option:selected').val()) * parseFloat($(this).val())) : 0;
            //var _total = !isNaN(parseFloat($('#OptionInfo_Price').val())) && !isNaN(parseFloat($(this).val())) ? (parseFloat($('#OptionInfo_Price').val()) * parseFloat($(this).val())) : !isNaN(parseFloat($(this).val())) ? (parseFloat($('#OptionInfo_Price').attr('placeholder')) * parseFloat($(this).val())) : 0;
            var _total;
            if (!isNaN(parseFloat($('#OptionInfo_Price').val()))) {
                if (!isNaN(parseFloat($(this).val()))) {
                    _total = (parseFloat($('#OptionInfo_Price').val()) * parseFloat($(this).val())).toFixed(2);
                }
                else {
                    _total = 0;
                }
            }
            else if (!isNaN(parseFloat($('#OptionInfo_Price').attr('placeholder')))) {
                if (!isNaN(parseFloat($(this).val()))) {
                    _total = (parseFloat($('#OptionInfo_Price').attr('placeholder')) * parseFloat($(this).val())).toFixed(2);
                }
                else {
                    _total = 0;
                }
            }
            else {
                _total = 0;
            }
            $('#OptionInfo_TotalPaid').val(_total);
            //if (_total != 0 && $('#OptionInfo_PointsRedemption').attr('placeholder') != undefined && $('#OptionInfo_PointsRedemption').attr('placeholder') != '') {
            if (_total != 0 && $('#OptionInfo_MaxRateRedemption').attr('placeholder') != undefined && $('#OptionInfo_MaxRateRedemption').attr('placeholder') != '') {
                console.log('inside');
                $('#OptionInfo_PointsRedemption').val((parseFloat($('#OptionInfo_MaxRateRedemption').attr('placeholder')) / 100 * _total).toFixed(2));
            }
            else {
                $('#OptionInfo_PointsRedemption').val('');
            }
        });

        $('#OptionInfo_Option').on('change', function () {
            console.log('change');
            setTimeout(function () {
                $('#OptionInfo_Quantity').trigger('keyup');
                $('#OptionInfo_OptionDescription option:first').attr('selected', true);
                if ($('#OptionInfo_OptionDescription option:selected').val() != null && $('#OptionInfo_OptionDescription option:selected').val() != 'null')
                    $('#txtOptionDescription').val($('#OptionInfo_OptionDescription option:selected').val());
                else
                    $('#txtOptionDescription').val('');
            }, 200);
            $('#lblRedemptionRate').text($('#OptionInfo_MaxRateRedemption').attr('placeholder') + '%');
        });

        $('#OptionInfo_Price').on('keyup', function () {
            setTimeout(function () {
                $('#OptionInfo_Quantity').trigger('keyup');
            }, 200);
        });

        $('#OptionInfo_TotalPaid').on('keydown', function (e) {
            e.preventDefault();
        });

        $('#FlightInfo_FlightType').on('change', function () {
            if ($(this).val() == 2) {
                $('#divPickupTime').show();
            }
            else {
                $('#divPickupTime').hide();
            }
        });

        $('.mass-sending').unbind('click').on('click', function (e) {
            var _event = $(e.target).attr('data-sysevent');
            $('#MassUpdate_SendingEvent').val(_event);
            if (leads != '') {
                $('#frmMassSending').submit();
            }
            else {
                UI.messageBox(0, "No Leads Selected", null, null);
            }
        });

        $('.send-confirmation').on('click', function () {
            var evtID = $(this).data('sysevent');
            $.ajax({
                url: '/crm/PreArrival/SendEmail',
                type: 'POST',
                data: { reservationID: PREARRIVAL.oReservationsTable.$('tr.selected-row').attr('id'), eventID: evtID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    var mails = data.ItemID != 0 ? " to these addresses:<br />" + data.ItemID.to.split(',').join('<br />') : '';
                    UI.messageBox(data.ResponseType, data.ResponseMessage + mails + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveOptionSold').on('click', function () {
            if ($('#OptionInfo_Price').val() == '') {
                $('#OptionInfo_Price').val($('#OptionInfo_Price').attr('placeholder'));
            }
            //$('#OptionInfo_PointsRedemption').val($('#OptionInfo_PointsRedemption').attr('placeholder'));

            $('#frmOptionSoldInfo').submit();
        });

        $('#btnNewReservationInfo').on('click', function () {
            var times = $(this).parents('section:first').find('tr.selected-row').length;
            for (var i = 0; i < times; i++) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            }
            $(this).parents('section:first').find('.secondary-selected-row-dependent').each(function () {
                $(this).html('');
                $(this).val('');
            });
        });

        $('#PresentationInfo_SpiTour').on('change', function () {
            $.ajax({
                type: 'POST',
                cache: false,
                url: '/crm/PreArrival/GetTourInfo',
                data: { tourID: $(this).val() },
                success: function (data) {
                    $('#PresentationInfo_RealTourDate').val(data.realTourDate);
                    $('#PresentationInfo_FinalTourStatus option[value="' + data.tourStatus + '"]').attr('selected', true);
                    $('#PresentationInfo_SPISource').val(data.source);
                    $('#PresentationInfo_VolumeSold').val(data.volumeSold);
                }
            });
        });

        $('#PresentationInfo_SpiTourDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            //minDate: -2,
            //beforeShow: function () {
            //    $('#PresentationInfo_SpiTourDate').datepicker('option','minDate', -2);
            //},
            onClose: function (dateText, inst) {
                if (dateText != '') {
                    //$.ajax({
                    //    type: 'POST',
                    //    url: '/crm/PreArrival/GetManifestByDate',
                    //    cache: false,
                    //    data: { place: $('#ReservationInfo_Place option:selected').val(), date: dateText },
                    //    success: function (data) {
                    //        $('#PresentationInfo_SpiTour').fillSelect(data);
                    //    }
                    //});
                    getManifestByDate(dateText);
                }
            }
        });

        $('.field-disabled').each(function () {
            if ($(this).is('select') || $(this).is('input:text')) {
                $(this).on('mousedown', function (e) {
                    e.preventDefault();
                });
            }
        });

        $('#PaymentInfo_Quantity').on('keyup', function () {

            if ($('#PaymentInfo_PointsRates').val() != '') {
                var obj = JSON.parse($('#PaymentInfo_PointsRates').val());

                //_.filter(obj.rates, function (o) { return o.optionType == 'optionTypeIDTpPay' });
                var rate = obj.rates.find(function (o) { return o.rate != 0; }).rate / 100;
                //var rate = parseFloat(obj.rate) / 100;

                if ($('#PaymentInfo_Quantity').val() != '') {
                    var newAmount = parseFloat($('#PaymentInfo_Quantity').val()) * rate;
                    $('#PaymentInfo_Amount').val(newAmount);
                    $('#PaymentInfo_Currency option[value="USD"]').attr('selected', true);
                }
                else {
                    $('#PaymentInfo_Amount').val('');
                }
            }
            else {
                $('#PaymentInfo_Amount').val('');
            }
        });

        $('#PaymentInfo_Amount').on('keyup', function () {
            if ($('#PaymentInfo_PointsRates').val() != '' && $('#PaymentInfo_PaymentType option:selected').val() == 7) {
                var obj = JSON.parse($('#PaymentInfo_PointsRates').val());

                //var rate = obj.rates.find(function (o) { return o.rate != 0; }).rate / 100;
                var rate = obj.rates.find(function (o) { return o.rate != 0; });
                if (rate != null) {
                    rate = rate.rate / 100;

                    if ($('#PaymentInfo_Quantity').val() == '') {
                        //$(this).val()
                    }
                }
            }
        });

        $('#btnSavePayment').on('click', function () {
            if ($('#PaymentInfo_PaymentType').val() == 7) {
                var obj = JSON.parse($('#PaymentInfo_PointsRates').val());
                var payments = 0;
                var refunds = 0;
                PREARRIVAL.oPaymentsTable.$('tr').each(function (index, item) {
                    if ($(item).children('td:nth-child(1)').text() == 'Payment' && $(item).children('td:nth-child(2)').text() == 'Points Redemption' && $(item).children('td:nth-child(8)').text().indexOf('Approved') != -1) {
                        payments += parseFloat($(item).children('td:nth-child(3)').text().split(' ')[0]);
                    }
                    if ($(item).children('td:nth-child(1)').text() == 'Refund' && $(item).children('td:nth-child(2)').text() == 'Points Redemption' && $(item).children('td:nth-child(8)').text().indexOf('Approved') != -1) {
                        refunds += parseFloat($(item).children('td:nth-child(3)').text().split(' ')[0]);
                    }
                });
                //if ((payments - refunds) + parseFloat($('#PaymentInfo_Amount').val()) <= parseFloat(obj.maxAllowed)) {
                if ((payments - refunds) + (parseFloat($('#PaymentInfo_Amount').val()) * ($('#PaymentInfo_TransactionType option:selected').val() == 1 ? 1 : -1)) <= parseFloat(obj.maxAllowed)) {
                    $('#frmPaymentInfo').submit();
                }
                else {
                    UI.messageBox(0, 'amount exceeds total permitted to pay with points', null, null);
                }
            }
            else {
                $('#frmPaymentInfo').submit();
            }
        });

        $('#OptionInfo_OptionDescription').on('change', function (e) {
            if ($('#OptionInfo_OptionDescription option:selected').val() != null && $('#OptionInfo_OptionDescription option:selected').val() != 'null') {
                $('#txtOptionDescription').val($('#OptionInfo_OptionDescription option:selected').val());
            }
            else {
                $('#txtOptionDescription').val('');
            }
            //$('#txtOptionDescription').val($(this).val());
        });

        $('#btnResetVar').on('click', function () {
            $.ajax({
                url: '/crm/PreArrival/ResetAppVar',
                cache: false,
                type: 'POST',
                success: function (data) {

                }
            });
        });

        $('#btnGroupLeads').on('click', function () {
            if ($('.mass-update-coincidences').val() != '') {
                $.fancybox({
                    type: 'iframe',
                    href: location.hostname === 'localhost' ? 'http://localhost:45001/crm/prearrival/groupleads?leads=' + $('.mass-update-coincidences').val() : 'https://eplat.villagroup.com/crm/prearrival/GroupLeads?leads=' + $('.mass-update-coincidences').val(),
                    scrolling: 'no',
                    modal: true,
                    afterClose: function () {
                        console.log('closed');
                        $('.mass-update-coincidences').val('');
                        PREARRIVAL.leads = new Array();
                        $('#tblSearchPreArrivalResults thead .chk-parent')[0].checked = false;
                        PREARRIVAL.oTable.$('.chk-son').each(function () {
                            $(this)[0].checked = false;
                        });
                        $('.span-selected-leads').html('0 lead(s) selected');
                    }
                });
                //guardar grupo en tblleads. identificar dónde visualizar/editar grupos guardados
            }
        });
    }

    function getManifestByDate(dateText, tourID) {
        $.ajax({
            type: 'POST',
            url: '/crm/PreArrival/GetManifestByDate',
            cache: false,
            data: { place: $('#ReservationInfo_Place option:selected').val(), date: dateText },
            success: function (data) {
                $('#PresentationInfo_SpiTour').fillSelect(data);
                if (tourID != undefined && tourID != null && tourID != '') {
                    console.log(tourID);
                    $('#PresentationInfo_SpiTour option[value="' + tourID + '"]').attr('selected', true).trigger('change');
                }
            }
        });
    }

    var getFields = function (parentComponentID) {
        //parentComponentID = parentComponentID != undefined ? parentComponentID : 0;
        //$.getJSON('/PreArrival/GetFields', { sysComponentID: parentComponentID }, function (data) {//PreArrival

        //    $.each(data, function (index, item) {
        //        PREARRIVAL.Filters.Tables[index] = item.TableName;
        //        PREARRIVAL.Filters.Ids[index] = item.FieldID;
        //        PREARRIVAL.Filters.Fields[index] = item.FieldName;
        //        PREARRIVAL.Filters.Names[index] = item.DisplayName;
        //        PREARRIVAL.Filters.Types[index] = item.FieldType;
        //        PREARRIVAL.Filters.Properties[index] = item.PropertyName;
        //        PREARRIVAL.Columns.Ids[index] = item.FieldID;
        //        PREARRIVAL.Columns.Value[index] = item.FilterID;
        //        PREARRIVAL.Columns.Text[index] = item.DisplayName;
        //    });

        //    $('#Search_SearchFilters').bind('keydown', function (e) {
        //        if (e.keyCode === $.ui.keyCode.TAB && $(this).data('ui-autocomplete').menu.active) {
        //            e.preventDefault();
        //        }
        //    }).autocomplete({
        //        source: PREARRIVAL.Filters.Names,
        //        minLength: 0,
        //        position: { my: 'right top', at: 'right bottom' },
        //        autoFocus: true,
        //        select: function (e, ui) {
        //            $(this).val(ui.item.value);
        //        },
        //        _resizeMenu: function () {
        //            this.menu.element.outerWidth(50);
        //        }
        //    }).on('focus', function () {
        //        $(this).autocomplete('search', '');
        //    });

        //    $('#Search_ColumnHeaders').bind('keydown', function (e) {
        //        if (e.keyCode === $.ui.keyCode.TAB && $(this).data('ui-autocomplete').menu.active) {
        //            e.preventDefault();
        //        }
        //    }).autocomplete({
        //        source: PREARRIVAL.Columns.Text,
        //        minLength: 0,
        //        position: { my: 'right top', at: 'right bottom' },
        //        autoFocus: true,
        //        select: function (e, ui) {
        //            $(this).val(ui.item.value);
        //        }
        //    }).on('focus', function () {
        //        $(this).autocomplete('search', '');
        //    });
        //});
    }

    var searchResultsTable = function (data) {
        PREARRIVAL.oTable = $('#tblSearchPreArrivalResults').dataTable({
            "bFilter": false,
            "bProcessing": true,
            "bAutoWidth": false,
            "aoRowCallback": [UI.tablesHoverEffect()],
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            },
            'aoColumnDefs': [{ 'aTargets': [0], 'bSortable': false }],
            'fnDrawCallback': function () {
                $('#lblVisible').text($('#tblSearchPreArrivalResults tbody tr').length);
            }
        });
        $('#lblAllLeads').text(PREARRIVAL.oTable.$('tr').length);
        PREARRIVAL.bindToCheckBoxes();
        PREARRIVAL.resetAll();
        //if (PREARRIVAL.oTable.$('tr').length == 1) {
        //    PREARRIVAL.oTable.$('tr:first').trigger('click');
        //}
    }

    var resetAll = function () {
        $('#btnNewReservationInfo').trigger('click');
        $('#fdsPreArrivalInfo').find('form').each(function () {
            $(this).clearForm();
        });
        $('#fdsPreArrivalInfo').find('.primary-selected-row-dependent:not(fieldset)').each(function () {
            $(this).html('');
            $(this).val('');
        });

    }

    var loadReportLayouts = function () {
        $.getJSON('/PreArrival/FillDrpReportLayoutsByUser', {}, function (data) {
            $.each(data, function (index, item) {
                PREARRIVAL.Layouts[index] = { "value": item.Value, "label": item.Text };
            });

            $('#Search_LayoutName').bind('keydown', function (e) {
                if (e.keyCode === $.ui.keyCode.TAB && $(this).data('ui-autocomplete').menu.active) {
                    e.preventDefault();
                }
            }).autocomplete({
                source: PREARRIVAL.Layouts,
                minLength: 0,
                position: { my: 'right top', at: 'right bottom' },
                autoFocus: true,
                select: function (e, ui) {
                    e.preventDefault();
                    PREARRIVAL.selectLayout(ui);
                }
            }).on('focus', function () {
                $(this).autocomplete('search', '');
            });

            if (location.hash != '') {
                //$.fancybox({
                //    type: 'html',
                //    content:'<img id="imgOverlay" src="/Content/themes/base/images/loading.gif"/>',
                //    centerOnScroll: true,
                //    hideOnOverlayClick: false,
                //    scrolling:'no'
                //});
                setTimeout(function () {
                    var value = _.filter(PREARRIVAL.Layouts, function (o) { return (o.label.indexOf('Administrador') != -1 || o.label.indexOf('Administrator') != -1); });
                    $('#Search_LayoutName').data('ui-autocomplete')._trigger('select', 'autocompleteselect', { item: { value: value[0].value, label: value[0].label } });
                    setTimeout(function () {
                        //$.fancybox.close();
                        $('#Search_ReservationID').val(location.hash.substr(4));
                        $('#btnSearchPreArrival').trigger('click');
                    }, 2000);
                }, 2000);
            }
        });
    }

    var selectLayout = function (ui) {
        var value = ui.item.value;
        var array = value.split('|')[1].split(',');

        $('#Search_ReportLayout').val(value.split('|')[0]);
        $('#Search_Fields').val(value.split('|')[1]);
        $('#Search_LayoutName').val(ui.item.label);
        if (value.split('|')[2].toLowerCase() == 'true') {
            $('input:radio[name="Search_Public"]')[0].checked = true;
        }
        else {
            $('input:radio[name="Search_Public"]')[1].checked = true;
        }
        $('#searchTab .dynamic-field').remove();
        renderFormField(array.filter(function (text) { return (text.indexOf('field') > -1); }));
        $('#columnsTab .dynamic-field').remove();
        renderColumnHeader(array.filter(function (text) { return (text.indexOf('col') > -1); }));
    }

    function renderFormField(data) {
        $.each(data, function (i, _item) {
            var item = _item.indexOf('_') > 0 ? _item.split('_')[1] : _item;
            var index = PREARRIVAL.Filters.Ids.indexOf(item);

            var _str = '<div class="editor-alignment dynamic-field" data-field-id="field_' + PREARRIVAL.Filters.Ids[index] + '">'
                + '<div class="editor-label"><label for="' + PREARRIVAL.Filters.Properties[index] + '">' + PREARRIVAL.Filters.Names[index] + '</label><i class="material-icons sm right remove-property">clear</i></div>'
                + '<div class="editor-field">';

            switch (PREARRIVAL.Filters.Types[index]) {
                case 'int': case 'Guid': {
                    //***
                    if (PREARRIVAL.Filters.Fields[index] == 'reservationID' || PREARRIVAL.Filters.Fields[index] == 'totalNights') {
                        _str += '<input type="text" id="' + PREARRIVAL.Filters.Properties[index] + '" name="' + PREARRIVAL.Filters.Properties[index] + '" />';
                    }
                    else {
                        //***
                        $.ajax({
                            async: false,
                            url: '/crm/PreArrival/GetDDLData',
                            data: { itemType: PREARRIVAL.Filters.Fields[index] },
                            //type: 'POST',
                            cache: false,
                            success: function (data) {
                                _str += '<select id="' + PREARRIVAL.Filters.Properties[index] + '" multiple="multiple" name="' + PREARRIVAL.Filters.Properties[index] + '">';
                                $.each(data, function (index, item) {
                                    _str += '<option value="' + item.Value + '">' + item.Text + '</option>';
                                });
                                _str += '</select>';
                            }
                        });
                    }
                    break;
                }
                case 'bit': {
                    _str += '<label for="' + PREARRIVAL.Filters.Properties[index] + 'True">Yes</label>'
                        + '<input type="radio" id="' + PREARRIVAL.Filters.Properties[index] + '" value="True" name="' + PREARRIVAL.Filters.Properties[index] + '" checked="checked" />';
                    _str += '<label for="' + PREARRIVAL.Filters.Properties[index] + 'False">No</label>'
                        + '<input type="radio" id="' + PREARRIVAL.Filters.Properties[index] + '" value="False" name="' + PREARRIVAL.Filters.Properties[index] + '" />';
                    break;
                }
                case 'date': {
                    var altProperty = PREARRIVAL.Filters.Properties[index].toString();
                    altProperty = altProperty.replace('_I_', '_F_');
                    _str += '<input type="text" id="' + PREARRIVAL.Filters.Properties[index] + '" data-uses-date-picker="true" name="' + PREARRIVAL.Filters.Properties[index] + '" />';
                    _str += '<input type="text" class="right" id="' + altProperty + '" data-uses-date-picker="true" name="' + altProperty + '" />';
                    break;
                }
                case 'string': {
                    _str += '<input type="text" id="' + PREARRIVAL.Filters.Properties[index] + '" name="' + PREARRIVAL.Filters.Properties[index] + '" />';
                    break;
                }
            }
            _str += '</div></div>';
            $('#Search_SearchFilters').parents('.editor-alignment').before(_str);
            PREARRIVAL.removeProperty();
            UI.applyMultiselect();
            UI.applyDatePicker();
        });
        $('.dynamic-field .editor-field >input').off('keydown').on('keydown', function (e) {
            if (e.keyCode === 13) {
                $('#btnSearchPreArrival').trigger('click');
            }
        });
    }

    function renderColumnHeader(data) {
        $.each(data, function (i, _item) {
            var item = _item.indexOf('_') > 0 ? _item.split('_')[1] : _item;
            var index = PREARRIVAL.Columns.Ids.indexOf(item);
            //currently working line
            //var _str = '<div id="' + PREARRIVAL.Filters.Tables[index] + '.' + PREARRIVAL.Filters.Fields[index] + '" class="column-headers dynamic-field" data-field-id="col_' + PREARRIVAL.Columns.Ids[index] + '" data-filter="' + PREARRIVAL.Columns.Value[index] + '">' + PREARRIVAL.Columns.Text[index] + '<i class="material-icons sm right remove-property">clear</i></div>';
            var _str = '<div id="' + PREARRIVAL.Filters.Fields[index] + '" class="column-headers dynamic-field" data-field-id="col_' + PREARRIVAL.Columns.Ids[index] + '">' + PREARRIVAL.Columns.Text[index] + '<i class="material-icons sm right remove-property">clear</i></div>';
            $('#Search_ColumnHeaders').parents('.editor-alignment').before(_str);

            PREARRIVAL.removeProperty();
        });
    }

    var removeProperty = function () {
        $('.remove-property').unbind('click').on('click', function (e) {
            $(e.target).parents('.dynamic-field').first().remove();
        });
    }

    var makeTableRowsSelectable = function () {
        PREARRIVAL.oTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            //$('.refund-dependent').hide();
            $('#PaymentInfo_TransactionType option[value="1"]').attr('selected', true);
            $('#PaymentInfo_TransactionType').trigger('change');
            if (!$(e.target).is(':input')) {
                $('#frmPreArrival').clearForm();
                $('#frmReservationInfo').clearForm();
                PREARRIVAL.oTable.$('tr.selected-row').removeClass('selected-row primary');
                $(this).addClass('selected-row primary');
                $.ajax({
                    url: '/PreArrival/GetLead',
                    cache: false,
                    type: 'POST',
                    data: { id: $(this).attr('id') },
                    success: function (data) {
                        $('#Info_LeadID').val(data.Info_LeadID);
                        $('#BillingInfo_LeadID').val(data.Info_LeadID);
                        $('#InteractionsInfo_LeadID').val(data.Info_LeadID);
                        $('#ReservationInfo_LeadID').val(data.Info_LeadID);
                        $('#PresentationInfo_LeadID').val(data.Info_LeadID);

                        $('#Info_FirstName').val(data.Info_FirstName);
                        $('#Info_LastName').val(data.Info_LastName);
                        $('#Info_Terminal option[value="' + data.Info_Terminal + '"]').attr('selected', true);
                        $('#Info_Terminal').trigger('change');
                        $('#Info_LeadStatus option[value="' + data.Info_LeadStatus + '"]').attr('selected', true);
                        $('#Info_LeadStatusDescription').val(data.Info_LeadStatusDescription);
                        $('#Info_LeadSource option[value="' + data.Info_LeadSource + '"]').attr('selected', true);
                        $('#PresentationInfo_LeadSource option[value="' + data.Info_LeadSource + '"]').attr('selected', true);
                        $('#Info_CallClasification option[value="' + data.Info_CallClasification + '"]').attr('selected', true);
                        $('#Info_TimeZone option[value="' + data.Info_TimeZone + '"]').attr('selected', true);
                        $('#Info_Address').val(data.Info_Address);
                        $('#Info_City').val(data.Info_City);
                        $('#Info_State').val(data.Info_State);
                        $('#Info_ZipCode').val(data.Info_ZipCode);
                        $('#Info_Country option[value="' + data.Info_Country + '"]').attr('selected', true);
                        PREARRIVAL.editionBasedOnStatus();
                        if (data.Info_LeadSource == null || data.Info_LeadSource == '' || data.Info_LeadSource == '0' || data.Info_LeadSource == 0) {
                            alert('Lead doesn\'t have Lead Source assigned, please assign before proceed');
                        }

                        PREARRIVAL.Emails = data.ListPreArrivalEmails;
                        PREARRIVAL.Phones = data.ListPreArrivalPhones;
                        PREARRIVAL.Member = data.PreArrivalMemberInfoModel;
                        PREARRIVAL.Billings = data.ListPreArrivalBillings;
                        INTERACTION.renderInteractions(data.PreArrivalInteractions);
                        PREARRIVAL.Reservations = data.ListPreArrivalReservations;

                        //$('#PreArrivalEmails').val(JSON.stringify(data.ListPreArrivalEmails));
                        //$('#PreArrivalPhones').val(JSON.stringify(data.ListPreArrivalPhones));

                        loadLeadInfo();
                        renderReservation(PREARRIVAL.Reservations);
                        localStorage.Eplat_CreateBilling_Flag = false;
                        UI.expandFieldset('fdsPreArrivalInfo');
                        UI.scrollTo('fdsPreArrivalInfo', null);
                    },
                    error: function (jqXHR, status, error) {
                        UI.messageBox(-1, error + '<br />Lead NOT Loaded', null, null);
                    }
                });
            }
        });
        if (PREARRIVAL.oTable.$('tr').length == 1) {
            PREARRIVAL.oTable.$('tr:first').trigger('click');
        }
    }

    var makeTblBillingsSelectable = function () {
        if ($('#tblBillingResults').length > 0) {
            PREARRIVAL.oBillingsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
                if ($(e.target).hasClass('show-history')) {
                    $.ajax({
                        url: '/PreArrival/GetTransactionsHistory',
                        cache: false,
                        type: 'POST',
                        data: { id: $(this).attr('id') },
                        success: function (data) {
                            var str = '<table><thead>';
                            str += '<tr><th>Transaction ID</th><th>Invoice</th><th>Authorization Date</th><th>Error Code</th><th>Transaction Date</th><th>Merchant Account</th></tr>';
                            str += '</thead><tbody>';
                            $.each(data, function (index, item) {                                
                                str += '<tr>'
                                    + '<td>' + item.moneyTransactionID + '</td>'
                                    + '<td>' + item.authCode + '</td>'
                                    + '<td>' + item.authDate + '</td>'
                                    + '<td>' + item.errorCode + '</td>'
                                    + '<td>' + item.transactionDate + '</td>'
                                    + '<td>' + item.billingName + '</td>'
                                    + '</tr>'
                            });
                            str += '</tbody></table>';
                            $.fancybox({
                                type: 'inline',
                                content: str                                
                            });
                        }
                    });
                }
                else {
                    $(this).parents('form:first').clearForm();
                    PREARRIVAL.oBillingsTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/PreArrival/GetBilling',
                        cache: false,
                        type: 'POST',
                        data: { id: $(this).attr('id') },
                        success: function (data) {
                            $('#BillingInfo_BillingInfoID').val(data.BillingInfo_BillingInfoID);
                            $('#BillingInfo_FirstName').val(data.BillingInfo_FirstName);
                            $('#BillingInfo_LastName').val(data.BillingInfo_LastName);
                            $('#BillingInfo_Address').val(data.BillingInfo_Address);
                            $('#BillingInfo_Country option[value="' + data.BillingInfo_Country + '"]').attr('selected', true);
                            $('#BillingInfo_ZipCode').val(data.BillingInfo_ZipCode);
                            $('#BillingInfo_CardHolderName').val(data.BillingInfo_CardHolderName);
                            $('#BillingInfo_CardNumber').val(data.BillingInfo_CardNumber);
                            $('#BillingInfo_CardType option[value="' + data.BillingInfo_CardType + '"]').attr('selected', true);
                            $('#BillingInfo_CardExpiry').val(data.BillingInfo_CardExpiry);
                            $('#BillingInfo_CardCVV').val(data.BillingInfo_CardCVV);
                            $('#BillingInfo_Comments').val(data.BillingInfo_Comments);
                            UI.scrollTo('frmBillingInfo', null);
                        }
                    });
                }
            });
        }
    }

    function loadLeadInfo() {
        var str = '';
        $('#tblPreArrivalEmails tbody').html('');
        $('#tblPreArrivalPhones tbody').html('');
        renderEmail(PREARRIVAL.Emails);
        renderPhone(PREARRIVAL.Phones);
        renderBilling(PREARRIVAL.Billings);
        $('#PreArrivalMemberInfoModel_MemberInfo_ClubType').val(PREARRIVAL.Member.MemberInfo_ClubType);
        $('#PreArrivalMemberInfoModel_MemberInfo_CoOwner').val(PREARRIVAL.Member.MemberInfo_CoOwner);
        $('#PreArrivalMemberInfoModel_MemberInfo_MemberNumber').val(PREARRIVAL.Member.MemberInfo_MemberNumber);
        $('#PreArrivalMemberInfoModel_MemberInfo_MemberName').val(PREARRIVAL.Member.MemberInfo_MemberName);
        $('#PreArrivalMemberInfoModel_MemberInfo_ContractNumber').val(PREARRIVAL.Member.MemberInfo_ContractNumber);
    }

    function loadReservationInfo() {
        $('#PresentationInfo_LeadID').val(PREARRIVAL.Presentation.PresentationInfo_LeadID);
        $('#PresentationInfo_PresentationID').val(PREARRIVAL.Presentation.PresentationInfo_PresentationID);
        $('#PresentationInfo_FinalBookingStatus option[value="' + PREARRIVAL.Presentation.PresentationInfo_FinalBookingStatus + '"]').attr('selected', true);
        $('#PresentationInfo_FinalTourStatus option[value="' + PREARRIVAL.Presentation.PresentationInfo_FinalTourStatus + '"]').attr('selected', true);
        $('#PresentationInfo_HostessComments').val(PREARRIVAL.Presentation.PresentationInfo_HostessComments);
        $('#PresentationInfo_RealTourDate').datepicker('setDate', PREARRIVAL.Presentation.PresentationInfo_RealTourDate);//verify format with datepicker plugin

        //$('#PresentationInfo_SpiTourDate').datepicker('option', 'minDate', PREARRIVAL.Presentation.PresentationInfo_RealTourDate);
        $('#PresentationInfo_SpiTourDate').datepicker('setDate', PREARRIVAL.Presentation.PresentationInfo_RealTourDate);
        getManifestByDate(PREARRIVAL.Presentation.PresentationInfo_RealTourDate, PREARRIVAL.Presentation.PresentationInfo_SPITourID);
        //$('#PresentationInfo_RealTourDate').datepicker('setDate', PREARRIVAL.Presentation.PresentationInfo_SpiTourDate);

        $('#PresentationInfo_DatePresentation').val(PREARRIVAL.Presentation.PresentationInfo_DatePresentation);
        $('#PresentationInfo_TimePresentation').val(PREARRIVAL.Presentation.PresentationInfo_TimePresentation);
        $('#PresentationInfo_SecondaryBookingStatus option[value="' + PREARRIVAL.Presentation.PresentationInfo_SecondaryBookingStatus + '"]').attr('selected', true);
        $('#PresentationInfo_TourStatus option[value="' + PREARRIVAL.Presentation.PresentationInfo_TourStatus + '"]').attr('selected', true);
        $('#PresentationInfo_TourStatusText').val(PREARRIVAL.Presentation.PresentationInfo_TourStatusText);
        renderOption(PREARRIVAL.Options);
        renderRCOption(PREARRIVAL.RCOptions);
        renderFlight(PREARRIVAL.Flights);
        renderPayment(PREARRIVAL.Payments);
        renderLetter(PREARRIVAL.Letters);
    }

    function renderEmail(data, visible) {
        var str = '';
        if (visible == undefined || visible == false) {
            if (data.length > 0) {
                $.each(data, function (index, item) {
                    str += '<tr>'
                        + '<td style="display:none"></td>'
                        + '<td>' + item.EmailsInfo_Email + '</td>'
                        + '<td data-type="bool">' + (item.EmailsInfo_Main == 'true' || item.EmailsInfo_Main == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                        + '<td><i class="right material-icons sm email-visible">visibility</i></td></tr>';
                });

                //str += '<tr>'
                //    + '<td style="display:none"></td>'
                //    + '<td>' + data[0].EmailsInfo_Email + '</td>'
                //    + '<td data-type="bool">' + (data[0].EmailsInfo_Main == 'true' || data[0].EmailsInfo_Main == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                //    + '<td><i class="right material-icons sm email-visible">visibility</i></td></tr>';
            }
        }
        else {
            var array = new Array();
            if ($('#PreArrivalEmails').val() != null && $('#PreArrivalEmails').val() != '') {
                var _data = $.parseJSON($('#PreArrivalEmails').val());
                array = _data;
            }
            $.each(data, function (index, item) {
                var arr = { 'EmailsInfo_LeadEmailID': item.EmailsInfo_LeadEmailID, 'EmailsInfo_Email': item.EmailsInfo_Email, 'EmailsInfo_LeadID': $('#Info_LeadID').val(), 'EmailsInfo_Main': item.EmailsInfo_Main };
                array.push(arr);

                if (item.EmailsInfo_Main == 'true' || item.EmailsInfo_Main == true) {
                    $('#tblPreArrivalEmails tbody').find('i.material-icons').remove();
                }

                str += '<tr>'
                    + '<td data-val="' + item.EmailsInfo_LeadEmailID + '" style="display:none">' + item.EmailsInfo_LeadEmailID + '</td>'
                    + '<td data-val="' + item.EmailsInfo_Email + '">' + item.EmailsInfo_Email + '</td>'
                    + '<td data-val="' + item.EmailsInfo_Main + '" data-type="bool">' + (item.EmailsInfo_Main == 'true' || item.EmailsInfo_Main == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                    + '<td>'
                    + '</td></tr>';
            });
            $('#PreArrivalEmails').val(JSON.stringify(array));
        }
        $('#tblPreArrivalEmails tbody').append(str);
        PREARRIVAL.removeEmail();
    }

    function renderPhone(data, visible) {
        var str = '';
        if (visible == undefined || visible == false) {
            if (data.length > 0) {
                $.each(data, function (index, item) {
                    str += '<tr>'
                        + '<td style="display:none"></td>'
                        + '<td>' + item.PhonesInfo_PhoneTypeText + '</td>'
                        + '<td>' + item.PhonesInfo_PhoneNumber + '</td>'
                        + '<td></td>'
                        + '<td data-type="bool">' + (item.PhonesInfo_DoNotCall == 'true' || item.PhonesInfo_DoNotCall == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                        + '<td data-type="bool">' + (item.PhonesInfo_Main == 'true' || item.PhonesInfo_Main == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                        + '<td>' + (item.PhonesInfo_DoNotCall == 'true' || item.PhonesInfo_DoNotCall == true ? '' : '<i class="material-icons sm click-to-call" data-reference-id="' + item.PhonesInfo_LeadID + '" data-reference-phone-id="' + item.PhonesInfo_LeadPhoneID + '" title="click to call">phone</i>')
                        //+ (item.PhonesInfo_ShowContactInfo == 'true' || item.PhonesInfo_ShowContactInfo == true ? '<i class="right material-icons sm phone-visible">visibility</i>' : '')
                        + ($('#showContactInfo').val().toLowerCase() == 'true' || $('#showContactInfo').val() == true ? '<i class="right material-icons sm phone-visible">visibility</i>' : '')
                        + '</td>'
                        + '</tr > ';
                });
            }
        }
        else {
            var array = new Array();
            if ($('#PreArrivalPhones').val() != '') {
                var _data = $.parseJSON($('#PreArrivalPhones').val());
                array = _data;
            }

            $.each(data, function (index, item) {
                var arr = { 'PhonesInfo_LeadPhoneID': item.PhonesInfo_LeadPhoneID, 'PhonesInfo_LeadID': $('#Info_LeadID').val(), 'PhonesInfo_PhoneType': item.PhonesInfo_PhoneType, 'PhonesInfo_PhoneNumber': item.PhonesInfo_PhoneNumber, 'PhonesInfo_ExtensionNumber': item.PhonesInfo_ExtensionNumber, 'PhonesInfo_DoNotCall': item.PhonesInfo_DoNotCall, 'PhonesInfo_Main': item.PhonesInfo_Main };
                array.push(arr);
                if (item.PhonesInfo_Main == 'true' || item.PhonesInfo_Main == true) {
                    $('#tblPreArrivalPhones tbody tr').each(function (index, item) {
                        $(this).children('td:nth-child(6)').find('i.material-icons').remove();
                    });
                }
                str += '<tr>'
                    + '<td data-val="' + item.PhonesInfo_LeadPhoneID + '" style="display:none">' + item.PhonesInfo_LeadPhoneID + '</td>'
                    + '<td data-val="' + item.PhonesInfo_PhoneType + '">' + $('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneType option[value="' + item.PhonesInfo_PhoneType + '"]').text() + '</td>'
                    + '<td data-val="' + item.PhonesInfo_PhoneNumber + '">' + item.PhonesInfo_PhoneNumber + '</td>'
                    + '<td data-val="' + item.PhonesInfo_ExtensionNumber + '">' + (item.PhonesInfo_ExtensionNumber != null ? item.PhonesInfo_ExtensionNumber : '') + '</td>'
                    + '<td data-val="' + item.PhonesInfo_DoNotCall + '" data-type="bool">' + (item.PhonesInfo_DoNotCall == 'true' || item.PhonesInfo_DoNotCall == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                    + '<td data-val="' + item.PhonesInfo_Main + '" data-type="bool">' + (item.PhonesInfo_Main == 'true' || item.PhonesInfo_Main == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                    + '<td>'
                    + '</td></tr>';
            });
            $('#PreArrivalPhones').val(JSON.stringify(array));
        }
        $('#tblPreArrivalPhones tbody').append(str);
        PREARRIVAL.removePhone();
    }

    function _renderPhone(data, visible) {
        var str = '';
        if (visible == undefined || visible == false) {
            if (data.length > 0) {
                $.each(data, function (index, item) {
                    str += '<tr>'
                        + '<td style="display:none"></td>'
                        + '<td>' + item.PhonesInfo_PhoneTypeText + '</td>'
                        + '<td>' + item.PhonesInfo_PhoneNumber + '</td>'
                        + '<td></td>'
                        + '<td data-type="bool">' + (item.PhonesInfo_DoNotCall == 'true' || item.PhonesInfo_DoNotCall == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                        + '<td data-type="bool">' + (item.PhonesInfo_Main == 'true' || item.PhonesInfo_Main == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                        + '<td>' + (item.PhonesInfo_DoNotCall == 'true' || item.PhonesInfo_DoNotCall == true ? '' : '<i class="material-icons sm click-to-call" data-reference-id="' + item.PhonesInfo_LeadID + '" data-reference-phone-id="' + item.PhonesInfo_LeadPhoneID + '" title="click to call">phone</i>') + '</td>'
                        + '</tr > ';
                });
            }
        }
        else {
            var array = new Array();
            if ($('#PreArrivalPhones').val() != '') {
                var _data = $.parseJSON($('#PreArrivalPhones').val());
                array = _data;
            }

            $.each(data, function (index, item) {
                var arr = { 'PhonesInfo_LeadPhoneID': item.PhonesInfo_LeadPhoneID, 'PhonesInfo_LeadID': $('#Info_LeadID').val(), 'PhonesInfo_PhoneType': item.PhonesInfo_PhoneType, 'PhonesInfo_PhoneNumber': item.PhonesInfo_PhoneNumber, 'PhonesInfo_ExtensionNumber': item.PhonesInfo_ExtensionNumber, 'PhonesInfo_DoNotCall': item.PhonesInfo_DoNotCall, 'PhonesInfo_Main': item.PhonesInfo_Main };
                array.push(arr);
                if (item.PhonesInfo_Main == 'true' || item.PhonesInfo_Main == true) {
                    $('#tblPreArrivalPhones tbody tr').each(function (index, item) {
                        $(this).children('td:nth-child(6)').find('i.material-icons').remove();
                    });
                }
                str += '<tr>'
                    + '<td data-val="' + item.PhonesInfo_LeadPhoneID + '" style="display:none">' + item.PhonesInfo_LeadPhoneID + '</td>'
                    + '<td data-val="' + item.PhonesInfo_PhoneType + '">' + $('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneType option[value="' + item.PhonesInfo_PhoneType + '"]').text() + '</td>'
                    + '<td data-val="' + item.PhonesInfo_PhoneNumber + '">' + item.PhonesInfo_PhoneNumber + '</td>'
                    + '<td data-val="' + item.PhonesInfo_ExtensionNumber + '">' + (item.PhonesInfo_ExtensionNumber != null ? item.PhonesInfo_ExtensionNumber : '') + '</td>'
                    + '<td data-val="' + item.PhonesInfo_DoNotCall + '" data-type="bool">' + (item.PhonesInfo_DoNotCall == 'true' || item.PhonesInfo_DoNotCall == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                    + '<td data-val="' + item.PhonesInfo_Main + '" data-type="bool">' + (item.PhonesInfo_Main == 'true' || item.PhonesInfo_Main == true ? '<i class="material-icons sm">bookmark</i>' : '') + '</td>'
                    + '<td>'
                    + '</td></tr>';
            });
            $('#PreArrivalPhones').val(JSON.stringify(array));
        }
        $('#tblPreArrivalPhones tbody').append(str);
        PREARRIVAL.removePhone();
    }

    function renderBilling(data) {
        var str = '';
        var options = '';
        $.each(data, function (index, item) {
            str += '<tr id="' + item.BillingInfo_BillingInfoID + '" style="' + (item.BillingInfo_ShowInfo == true ? '' : 'display:none;') + '">'
                + '<td>' + item.BillingInfo_CardHolderName + '</td>'
                + '<td>' + item.BillingInfo_CardType + '</td>'
                + '<td>' + item.BillingInfo_CardNumber + '</td>'
                + '<td>' + item.BillingInfo_CardExpiry + '</td>'
                + '<td>' + item.BillingInfo_CardCVV + '</td>'
                + '<td>' + (item.BillingInfo_IsAdmin == true ? '<i class="material-icons show-history">visibility</i>' : '') + '</td>'
                + '</tr>';
            options += '<option value="' + item.BillingInfo_BillingInfoID + '">' + item.BillingInfo_CardType + ' - ' + item.BillingInfo_CardNumber.substr(12, 4) + '</option>';
        });

        $('#PaymentInfo_BillingInfo option').filter(function () {
            if ($(this).val() > 0) {
                $('#PaymentInfo_BillingInfo option[value="' + $(this).val() + '"]').remove();
            }
        });
        $('#PaymentInfo_BillingInfo option[value="0"]').after(options);

        if (document.getElementById('tblBillingResults') != undefined) {
            if ($.fn.DataTable.fnIsDataTable(document.getElementById('tblBillingResults'))) {
                PREARRIVAL.oBillingsTable.fnDestroy();
            }
            $('#tblBillingResults tbody').html(str);
            var tableColumns = $('#tblBillingResults').find('tbody tr').first().find('td');
            UI.searchResultsTable('tblBillingResults', tableColumns.length - 1);
            PREARRIVAL.oBillingsTable = $('#tblBillingResults').dataTable();
            PREARRIVAL.makeTblBillingsSelectable();
        }
    }

    function renderReservation(data) {
        var str = '';
        $.each(data, function (index, item) {
            str += '<tr class="' + (item.ReservationInfo_FoundInFront == false ? 'expired' : '') + '" id="' + item.ReservationInfo_ReservationID + '" title="' + (item.ReservationInfo_FoundInFront == false ? 'Reservation Not Found in Front' : '') + '">'
                + '<td> ' + item.ReservationInfo_HotelConfirmationNumber + '</td>'
                + '<td>' + item.ReservationInfo_FrontCertificateNumber + '</td>'
                + '<td> ' + item.ReservationInfo_ArrivalDate + '</td>'
                + '<td>' + item.ReservationInfo_DestinationText + '</td>'
                + '<td>' + item.ReservationInfo_Distinctives + '</td>'
                + '<td>' + item.ReservationInfo_ReservationStatusText + '</td>'
                + '<td>' + (item.ReservationInfo_OptionsSold ? 'Yes' : 'No') + '</td>'
                + '<td><i class="material-icons reassign-options" title="reassign options" data-reservation="' + item.ReservationInfo_ReservationID + '">settings</i></td>'
                + '</tr>';
        });

        if (document.getElementById('tblReservationsResults') != undefined) {
            if ($.fn.DataTable.fnIsDataTable(document.getElementById('tblReservationsResults'))) {
                PREARRIVAL.oReservationsTable.fnDestroy();
            }
            $('#tblReservationsResults tbody').html(str);
            var tableColumns = $('#tblReservationsResults').find('tbody tr').first().find('td');
            UI.searchResultsTable('tblReservationsResults', tableColumns.length - 1);
            PREARRIVAL.oReservationsTable = $('#tblReservationsResults').dataTable();
            PREARRIVAL.oReservationsTable.fnSort([[2, 'desc'], [6, 'desc']]);
            PREARRIVAL.makeTblReservationsSelectable();
            UI.expandFieldset('fdsReservationInfo');
            PREARRIVAL.oReservationsTable.$('tr:first').trigger('click');
            PREARRIVAL.reassignOptions();
        }
    }

    function renderOption(data) {
        var str = '';
        var total = 0;
        $.each(data, function (index, item) {
            total += parseFloat(item.OptionInfo_TotalPaid);
            str += '<tr id="' + item.OptionInfo_OptionSoldID + '">'
                + '<td>' + item.OptionInfo_Option + '</td>'
                + '<td>' + item.OptionInfo_Quantity + '</td>'
                + '<td>' + item.OptionInfo_Price + '</td>'
                + '<td>' + item.OptionInfo_GuestNames + '</td>'
                + '<td>' + item.OptionInfo_Eligible + '</td>'
                + '<td>' + item.OptionInfo_CreditAmount + '</td>'
                + '<td>' + item.OptionInfo_PointsRedemption + '</td>'
                + '<td>' + item.OptionInfo_TotalPaid + '</td>'
                + '<td>' + item.OptionInfo_Comments + '</td>'
                + '<td>' + (item.OptionInfo_OptionSoldID != undefined && item.OptionInfo_OptionSoldID != null ? '<i class="material-icons delete-option" title="delete option">delete</i>' : '') + '</td>'
                + '</tr>';
        });
        $('#spanTotalOptionsSold').html(total);
        UI.applyFormat('currency', 'spanTotalOptionsSold');
        //$('#tblOptionsSoldResults tbody').html(str);
        if (document.getElementById('tblOptionsSoldResults') != undefined) {
            if ($.fn.DataTable.fnIsDataTable(document.getElementById('tblOptionsSoldResults'))) {
                PREARRIVAL.oOptionsTable.fnDestroy();
            }
            $('#tblOptionsSoldResults tbody').html(str);
            var tableColumns = $('#tblOptionsSoldResults').find('tbody tr').first().find('td');
            UI.searchResultsTable('tblOptionsSoldResults', tableColumns.length - 1);
            PREARRIVAL.oOptionsTable = $('#tblOptionsSoldResults').dataTable();
            PREARRIVAL.makeTblOptionsSelectable();
        }
    }

    function renderRCOption(data) {
        var str = '';
        var total = 0;
        $.each(data, function (index, item) {
            total += parseFloat(item.OptionInfo_TotalPaid);
            str += '<tr id="' + item.OptionInfo_OptionSoldID + '">'
                + '<td>' + item.OptionInfo_Option + '</td>'
                + '<td>' + item.OptionInfo_Quantity + '</td>'
                + '<td>' + item.OptionInfo_Price + '</td>'
                + '<td>' + item.OptionInfo_GuestNames + '</td>'
                + '<td>' + item.OptionInfo_Eligible + '</td>'
                + '<td>' + item.OptionInfo_CreditAmount + '</td>'
                + '<td>' + item.OptionInfo_PointsRedemption + '</td>'
                + '<td>' + item.OptionInfo_TotalPaid + '</td>'
                + '<td>' + item.OptionInfo_Comments + '</td>'
                + '</tr>';
        });

        if (document.getElementById('tblRCOptionsSoldResults') != undefined) {
            if ($.fn.DataTable.fnIsDataTable(document.getElementById('tblRCOptionsSoldResults'))) {
                PREARRIVAL.oRCOptionsTable.fnDestroy();
            }
            $('#tblRCOptionsSoldResults tbody').html(str);
            var tableColumns = $('#tblRCOptionsSoldResults').find('tbody tr').first().find('td');
            UI.searchResultsTable('tblRCOptionsSoldResults', tableColumns.length - 1);
            PREARRIVAL.oRCOptionsTable = $('#tblRCOptionsSoldResults').dataTable();
            //PREARRIVAL.makeTblOptionsSelectable();
            if ($('#Info_Terminal option:selected').val() == 10) {
                $('#tblRCOptionsSoldResults').show();
            }
            else {
                if ($.fn.DataTable.fnIsDataTable(document.getElementById('tblRCOptionsSoldResults'))) {
                    PREARRIVAL.oRCOptionsTable.fnDestroy();
                }
                $('#tblRCOptionsSoldResults').hide();
            }
        }
    }

    function renderFlight(data) {
        var str = '';
        $.each(data, function (index, item) {
            str += '<tr id="' + item.FlightInfo_FlightID + '">'
                + '<td>' + (item.FlightInfo_AirlineText != null ? item.FlightInfo_AirlineText : '') + '</td>'
                + '<td>' + (item.FlightInfo_FlightNumber != null ? item.FlightInfo_FlightNumber : '') + '</td>'
                + '<td>' + (item.FlightInfo_PassengerNames != null ? item.FlightInfo_PassengerNames : '') + '</td>'
                + '<td>' + (item.FlightInfo_Passengers != null ? item.FlightInfo_Passengers : '') + '</td>'
                + '<td>' + (item.FlightInfo_FlightTypeText != null ? item.FlightInfo_FlightTypeText : '') + '</td>'
                + '<td>' + (item.FlightInfo_FlightDateTime != null ? item.FlightInfo_FlightDateTime : '') + '</td>'
                + '<td>' + (item.FlightInfo_FlightComments != null ? item.FlightInfo_FlightComments : '') + '</td>'
                + '<td><i class="material-icons delete-flight" title="delete flight">delete</i></td>'
                + '</tr>';
        });
        //$('#tblFlightsResults tbody').html(str);
        if (document.getElementById('tblFlightsResults') != undefined) {
            if ($.fn.DataTable.fnIsDataTable(document.getElementById('tblFlightsResults'))) {
                PREARRIVAL.oFlightsTable.fnDestroy();
            }
            $('#tblFlightsResults tbody').html(str);
            var tableColumns = $('#tblFlightsResults').find('tbody tr').first().find('td');
            UI.searchResultsTable('tblFlightsResults', tableColumns.length - 1);
            PREARRIVAL.oFlightsTable = $('#tblFlightsResults').dataTable();
            PREARRIVAL.makeTblFlightsSelectable();
        }
    }

    function renderPayment(data) {
        var str = '';
        var total = 0;
        $.each(data, function (index, item) {
            if (item.PaymentInfo_TransactionTypeText == 'Refund') {
                total -= parseFloat(item.PaymentInfo_Amount);
            }
            else {
                if (item.PaymentInfo_PendingCharge == false && item.PaymentInfo_Status.indexOf('Approved') != -1) {
                    total += parseFloat(item.PaymentInfo_Amount);
                }
            }
            str += '<tr id="' + item.PaymentInfo_PaymentDetailsID + '" ' + (item.PaymentInfo_PendingCharge == true ? 'data-pending-charge="true"' : '') + '>'
                + '<td>' + item.PaymentInfo_TransactionTypeText + '</td>'
                + '<td>' + item.PaymentInfo_PaymentTypeText + '</td>'
                + '<td>' + (item.PaymentInfo_Amount + ' ' + item.PaymentInfo_Currency) + '</td>'
                + '<td>' + item.PaymentInfo_ChargeTypeText + '</td>'
                + '<td>' + item.PaymentInfo_Invoice + (item.PaymentInfo_TransactionTypeText == 'Refund' && item.PaymentInfo_InvoiceToRefund != '' ? ' Refunding ' + item.PaymentInfo_InvoiceToRefund : '') + '</td>'
                + '<td>' + item.PaymentInfo_PaymentComments + '</td>'
                + '<td>' + item.PaymentInfo_DateSaved + '</td>'
                ////+ '<td ' + (item.PaymentInfo_PendingCharge == false && item.PaymentInfo_TransactionTypeText == 'Payment' && item.PaymentInfo_Status.indexOf('Approved') != -1 ? 'class="success"' : '') + '>' + (item.PaymentInfo_PendingCharge == false ? item.PaymentInfo_Status : '<i class="material-icons apply-charge" title="apply charge">payment</i>') + '</td>'
                //+ '<td ' + (item.PaymentInfo_PendingCharge == false && item.PaymentInfo_TransactionTypeText == 'Payment' && item.PaymentInfo_Status.indexOf('Approved') != -1 ? 'class="success"' : '') + '>' + item.PaymentInfo_Status + (item.PaymentInfo_PendingCharge == false ? '' : '<i class="material-icons apply-charge" title="apply charge">payment</i>') + '</td>'
                + '<td ' + (item.PaymentInfo_PendingCharge == false && item.PaymentInfo_TransactionTypeText == 'Payment' && item.PaymentInfo_Status.indexOf('Approved') != -1 ? 'class="success"' : '') + '>' + item.PaymentInfo_Status + '</td>'
                + '<td>' + (item.PaymentInfo_PendingCharge == false ? '' : '<i class="material-icons apply-charge" title="apply charge">payment</i>') + '</td>'
                + '<td>' + (item.PaymentInfo_TransactionType == '2' || item.PaymentInfo_PaymentType != '2' || item.PaymentInfo_PendingCharge == true ? '<i class="material-icons delete-transaction">delete</i>' : '') + '</td>'
                + '</tr>';
        });
        $('#spanTotalOptionsPayments').html(total);
        var due = parseFloat($('#spanTotalOptionsSold').find('.money-amount').text().trim().replace(/\,/g, '')) - parseFloat(total);
        $('#spanTotalOptionsDue').html(due);
        UI.applyFormat('currency', 'spanTotalOptionsPayments,spanTotalOptionsDue');

        if (document.getElementById('tblPaymentsResults') != undefined) {
            if ($.fn.DataTable.fnIsDataTable(document.getElementById('tblPaymentsResults'))) {
                PREARRIVAL.oPaymentsTable.fnDestroy();
            }
            $('#tblPaymentsResults tbody').html(str);
            var tableColumns = $('#tblPaymentsResults').find('tbody tr').first().find('td');
            UI.searchResultsTable('tblPaymentsResults', tableColumns.length - 1);
            PREARRIVAL.oPaymentsTable = $('#tblPaymentsResults').dataTable();
            var oSettings = PREARRIVAL.oPaymentsTable.fnSettings();
            PREARRIVAL.oPaymentsTable.fnSort([[6, 'desc']]);
            PREARRIVAL.makeTblPaymentsSelectable();
        }
    }

    function renderLetter(data) {
        var str = '';
        $.each(data, function (index, item) {
            str += '<tr id="' + item.ID + '">'
                + '<td>' + item.Description + '</td>'
                + '<td>' + item.DateSent + '</td>'
                + '<td>' + (item.DateRead != null ? item.DateRead : '') + '</td>'
                + '<td>' + item.DateSigned + '</td>'
                + '<td>'
                + (item.DateSent != null && item.DateSent != '' ? '<i class="material-icons right preview-tab" style="margin-right:5px;" title="' + item.Subject + ' ' + item.DateSent + '" data-email-id="' + item.EmailID + '" data-transaction="' + item.Transaction + '" data-id="' + item.ID + '">open_in_new</i>' : '')
                + '<i class="material-icons right preview" data-email-id="' + item.EmailID + '" data-transaction="' + item.Transaction + '" data-id="' + item.ID + '" title="' + (item.Sent == true ? 'preview' : 'send') + '">' + (item.Sent == true ? 'visibility' : 'email') + '</i>'
                + '</td>'
                + '</tr>'
        });

        if (document.getElementById('tblAvailableLetters') != undefined) {
            $('#tblAvailableLetters tbody').html(str);
            //add event to preview icons
            $('.preview').unbind('click').on('click', function () {
                var notification = $(this).attr('data-id');
                var transaction = $(this).attr('data-transaction');
                $.ajax({
                    url: '/PreArrival/PreviewEmail',
                    cache: false,
                    data: { reservationID: PREARRIVAL.oReservationsTable.$('tr.selected-row').attr('id'), emailNotificationID: notification, transactionID: transaction },
                    success: function (data) {
                        if (data.Status == null || data.Status == '') {
                            UI.renderEmailPreview(data, [PREARRIVAL.oReservationsTable.$('tr.selected-row').attr('id'), notification, transaction]);
                        }
                        else {
                            UI.messageBox(-1, data.Status, null, null);
                        }
                    }
                });
            });
            $('.preview-tab').unbind('click').on('click', function () {
                var notification = $(this).attr('data-id');
                var transaction = $(this).attr('data-transaction');
                $.ajax({
                    url: '/PreArrival/PreviewEmail',
                    cache: false,
                    data: { reservationID: PREARRIVAL.oReservationsTable.$('tr.selected-row').attr('id'), emailNotificationID: notification, transactionID: transaction },
                    success: function (data) {
                        if (data.Status == null || data.Status == '') {
                            UI.renderEmailPreview(data, [PREARRIVAL.oReservationsTable.$('tr.selected-row').attr('id'), notification, transaction], null, null, 'html');
                        }
                        else {
                            UI.messageBox(-1, data.Status, null, null);
                        }
                    }
                });
            });
        }
    }

    var reassignOptions = function () {
        $('.reassign-options').unbind('click').on('click', function (e) {
            var url = location.hostname === 'localhost' ? 'http://localhost:45001/crm/prearrival/ReassignOptions?id=' + $(this).data('reservation') : 'https://eplat.villagroup.com/crm/prearrival/ReassignOptions?id=' + $(this).data('reservation');
            //var url = 'http://localhost:45001/crm/prearrival/ReassignOptions?id=' + $(this).data('reservation');
            $.fancybox({
                type: 'iframe',
                href: url
            });
        });
    }

    var makeTblReservationsSelectable = function () {
        PREARRIVAL.oReservationsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).hasClass('reassign-options')) {
                $('#frmReservationInfo').clearForm();
                PREARRIVAL.oReservationsTable.$('tr.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $.ajax({
                    url: '/PreArrival/GetReservation',
                    cache: false,
                    type: 'POST',
                    data: { id: $(this).attr('id') },
                    success: function (data) {
                        $('#ReservationInfo_ReservationID').val(data.ReservationInfo_ReservationID);
                        //$('#PresentationInfo_LeadID').val(data.ReservationInfo_LeadID);
                        $('#PresentationInfo_ReservationID').val(data.ReservationInfo_ReservationID);
                        $('#OptionInfo_ReservationID').val(data.ReservationInfo_ReservationID);
                        $('#FlightInfo_ReservationID').val(data.ReservationInfo_ReservationID);
                        $('#PaymentInfo_ReservationID').val(data.ReservationInfo_ReservationID);

                        $('#ReservationInfo_Destination option[value="' + data.ReservationInfo_Destination + '"]').attr('selected', true);
                        $('#ReservationInfo_Destination').trigger('change');

                        $('#ReservationInfo_Place option[value="' + data.ReservationInfo_Place + '"]').attr('selected', true);
                        $('#ReservationInfo_Place').trigger('change');
                        $('#ReservationInfo_RoomType option[value="' + data.ReservationInfo_RoomType + '"]').attr('selected', true);
                        $('#ReservationInfo_RoomNumber').val(data.ReservationInfo_RoomNumber);
                        $('#ReservationInfo_ReservationStatus option[value="' + data.ReservationInfo_ReservationStatus + '"]').attr('selected', true);
                        $('#ReservationInfo_FrontOfficeAgencyName').val(data.ReservationInfo_FrontOfficeAgencyName);
                        $('#ReservationInfo_HotelConfirmationNumber').val(data.ReservationInfo_HotelConfirmationNumber);
                        $('#ReservationInfo_ArrivalDate').val(data.ReservationInfo_ArrivalDate);
                        $('#ReservationInfo_DepartureDate').val(data.ReservationInfo_DepartureDate);
                        $('#ReservationInfo_NumberAdults').val(data.ReservationInfo_NumberAdults);
                        $('#ReservationInfo_NumberChildren').val(data.ReservationInfo_NumberChildren);
                        $('#ReservationInfo_PlanType option[value="' + data.ReservationInfo_PlanType + '"]').attr('selected', true);
                        $('#ReservationInfo_FrontPlanType').val(data.ReservationInfo_FrontPlanType);
                        $('#ReservationInfo_TotalNights').val(data.ReservationInfo_TotalNights);
                        $('#ReservationInfo_TotalPaid').val(data.ReservationInfo_TotalPaid);
                        $('#ReservationInfo_ConfirmedTotalPaid').val(data.ReservationInfo_ConfirmedTotalPaid);
                        $('#ReservationInfo_DiamanteTotalPaid').val(data.ReservationInfo_DiamanteTotalPaid);
                        $('#ReservationInfo_GreetingRep option[value="' + data.ReservationInfo_GreetingRep + '"]').attr('selected', true);
                        if (data.ReservationInfo_IsSpecialOcassion == true) {
                            $('input[name="ReservationInfo_IsSpecialOcassion"]')[0].checked = true;
                        }
                        else {
                            $('input[name="ReservationInfo_IsSpecialOcassion"]')[1].checked = true;
                        }
                        $('#ReservationInfo_SpecialOcassionComments').val(data.ReservationInfo_SpecialOCassionComments);
                        $('#ReservationInfo_ConciergeComments').val(data.ReservationInfo_ConciergeComments);
                        $('#ReservationInfo_FrontComments').val(data.ReservationInfo_FrontComments);
                        $('#ReservationInfo_ReservationComments').val(data.ReservationInfo_ReservationComments);
                        $('#ReservationInfo_ResortConnectReservationComments').val(data.ReservationInfo_ResortConnectReservationComments);
                        $('#ReservationInfo_GuestsNames').val(data.ReservationInfo_GuestsNames);
                        $('input[name="ReservationInfo_RoomUpgraded"')[0].checked = data.ReservationInfo_RoomUpgraded;
                        $('input[name="ReservationInfo_PreCheckIn"]')[0].checked = data.ReservationInfo_PreCheckIn;
                        $('#ReservationInfo_FrontContractNumber').val(data.ReservationInfo_FrontContractNumber);
                        $('#ReservationInfo_FrontCertificateNumber').val(data.ReservationInfo_FrontCertificateNumber);
                        PREARRIVAL.Presentation = data.PreArrivalPresentationsModel;
                        PREARRIVAL.Options = data.ListPreArrivalOptions;
                        PREARRIVAL.RCOptions = data.ListPreArrivalRCOptions;
                        PREARRIVAL.Flights = data.ListPreArrivalFlights;
                        PREARRIVAL.Payments = data.ListPreArrivalPayments;
                        PREARRIVAL.Letters = data.ListAvailableLetters;
                        loadReservationInfo();
                        //UI.scrollTo('frmReservationInfo', null);
                    },
                    error: function (jqXHR, status, error) {
                        UI.messageBox(-1, error + '<br />Reservation NOT Loaded', null, null);
                    }
                });
            }
        });
    }

    var makeTblOptionsSelectable = function () {
        if ($('#tblOptionsSoldResults').length > 0) {
            PREARRIVAL.oOptionsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
                var $tr = $(this);
                if (!$(e.target).hasClass('delete-option')) {
                    $(this).parents('form:first').clearForm();
                    PREARRIVAL.oOptionsTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/PreArrival/GetOptionSold',
                        cache: false,
                        type: 'POST',
                        data: { id: $(this).attr('id') },
                        success: function (data) {
                            $('#OptionInfo_OptionSoldID').val(data.OptionInfo_OptionSoldID);
                            $('#OptionInfo_OptionType option[value="' + data.OptionInfo_OptionType + '"]').attr('selected', true).trigger('change');
                            $('#OptionInfo_Option option[value="' + data.OptionInfo_Option + '"]').attr('selected', true).trigger('change');
                            $('#OptionInfo_Price').val(data.OptionInfo_Price);
                            $('#OptionInfo_Quantity').val(data.OptionInfo_Quantity);
                            $('#OptionInfo_DateTime').val(data.OptionInfo_DateTime);
                            $('#OptionInfo_PointsRedemption').val(data.OptionInfo_PointsRedemption);
                            $('#OptionInfo_TotalPaid').val(data.OptionInfo_TotalPaid);
                            $('#OptionInfo_GuestNames').val(data.OptionInfo_GuestNames);
                            $('#OptionInfo_Eligible option[value="' + data.OptionInfo_Eligible + '"]').attr('selected', true);
                            $('#OptionInfo_CreditAmount').val(data.OptionInfo_CreditAmount);
                            $('#OptionInfo_Comments').val(data.OptionInfo_Comments);
                            UI.scrollTo('frmOptionSoldInfo', null);
                        }
                    });
                }
                else {
                    UI.confirmBox('Do you confirm you want to proceed?',
                        function (item) {
                            $.ajax({
                                url: '/PreArrival/DeleteOptionSold',
                                cache: false,
                                type: 'POST',
                                data: { id: item },
                                success: function (data) {
                                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                                    if (data.ResponseType > 0) {
                                        PREARRIVAL.oOptionsTable.fnDeleteRow($tr[0]);
                                        PREARRIVAL.updatePaymentsDueSection();
                                    }
                                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, null);
                                }
                            });
                        },
                        [$(this).attr('id')]);
                }
            });
        }
    }

    var makeTblFlightsSelectable = function () {
        if ($('#tblFlightsResults').length > 0) {
            PREARRIVAL.oFlightsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
                var $tr = $(this);
                if ($(e.target).hasClass('delete-flight')) {
                    UI.confirmBox('Do you want to proceed?',
                        function (item) {
                            $.ajax({
                                url: '/PreArrival/DeleteFlight',
                                cache: false,
                                type: 'POST',
                                data: { id: item },
                                success: function (data) {
                                    var duration = data.Type < 0 ? data.Type : null;
                                    if (data.Type > 0) {
                                        PREARRIVAL.oFlightsTable.fnDeleteRow($tr[0]);
                                    }
                                    UI.messageBox(data.Type, data.Message + '<br />' + data.Message, duration, null);
                                }
                            });
                        }, [$(this).attr('id')]);
                }
                else {
                    $(this).parents('form:first').clearForm();
                    PREARRIVAL.oFlightsTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/PreArrival/GetFlight',
                        cache: false,
                        type: 'POST',
                        data: { id: $(this).attr('id') },
                        success: function (data) {
                            $('#FlightInfo_FlightType').val(data.FlightInfo_FlightType);
                            $('#FlightInfo_FlightType').trigger('change');
                            $('#FlightInfo_FlightID').val(data.FlightInfo_FlightID);
                            $('#FlightInfo_Airline option[value="' + data.FlightInfo_Airline + '"]').attr('selected', true);
                            var airline = _.find(PREARRIVAL.Airlines, function (o) { return o.value == data.FlightInfo_Airline });
                            $('#FlightInfo_AirlineName').data('ui-autocomplete')._trigger('select', 'autocompleteselect', { item: { value: airline.value, label: airline.label } });
                            $('#FlightInfo_FlightNumber').val(data.FlightInfo_FlightNumber);
                            $('#FlightInfo_PassengerNames').val(data.FlightInfo_PassengerNames);
                            $('#FlightInfo_Passengers option[value="' + data.FlightInfo_Passengers + '"]').attr('selected', true);
                            $('#FlightInfo_FlightComments').val(data.FlightInfo_FlightComments);
                            $('#FlightInfo_FlightDateTime').val(data.FlightInfo_FlightDateTime);
                            $('#FlightInfo_PickUpTime').val(data.FlightInfo_PickUpTime);
                            UI.scrollTo('frmFlightInfo', null);
                        }
                    });
                }
            });
        }
    }

    var makeTblPaymentsSelectable = function () {
        if ($('#tblPaymentsResults').length > 0) {
            PREARRIVAL.oPaymentsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
                if (!$(e.target).hasClass('apply-charge') && !$(e.target).hasClass('delete-transaction') && ($(this).attr('data-pending-charge') == 'true' || $(this).children('td:nth-child(1)').text().trim() == 'Refund')) {
                    $(this).parents('form:first').clearForm();
                    PREARRIVAL.oPaymentsTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/PreArrival/GetPayment',
                        cache: false,
                        type: 'POST',
                        data: { id: $(this).attr('id') },
                        success: function (data) {
                            $('#PaymentInfo_PaymentDetailsID').val(data.PaymentInfo_PaymentDetailsID);
                            $('#PaymentInfo_TransactionType option[value="' + data.PaymentInfo_TransactionType + '"]').attr('selected', true);
                            $('#PaymentInfo_TransactionType').trigger('change');
                            $('#PaymentInfo_PaymentType option[value="' + data.PaymentInfo_PaymentType + '"]').attr('selected', true).trigger('change', { billing: data.PaymentInfo_BillingInfo });
                            $('#PaymentInfo_CCReferenceNumber').val(data.PaymentInfo_ReferenceNumber);
                            $('#PaymentInfo_CCType option[value="' + data.PaymentInfo_CCType + '"]').attr('selected', true);
                            $('#PaymentInfo_RefundAccount').val(data.PaymentInfo_RefundAccount);
                            $('#PaymentInfo_ChargeType option[value="' + data.PaymentInfo_ChargeType + '"]').attr('selected', true);
                            $('#PaymentInfo_ChargeDescription option[value="' + data.PaymentInfo_ChargeDescription + '"]').attr('selected', true);
                            $('#PaymentInfo_Amount').val(data.PaymentInfo_Amount);
                            $('#PaymentInfo_Currency option[value="' + data.PaymentInfo_Currency + '"]').attr('selected', true);
                            $('#PaymentInfo_PaymentComments').val(data.PaymentInfo_PaymentComments);
                            $('#PaymentInfo_Transaction').val(data.PaymentInfo_Transaction);
                            $('#PaymentInfo_DateSaved').val(data.PaymentInfo_DateSaved);
                            if (data.PaymentInfo_MadeByEplat) {
                                $('input[name="PaymentInfo_MadeByEplat"]')[0].checked = true;
                            }
                            else {
                                $('input[name="PaymentInfo_MadeByEplat"]')[1].checked = true;
                            }
                            UI.scrollTo('frmPaymentInfo', null);
                        }
                    });
                }
                else if ($(e.target).hasClass('apply-charge')) {
                    $.ajax({
                        url: '/PreArrival/ApplyCharge',
                        cache: false,
                        type: 'POST',
                        data: { id: $(this).attr('id') },
                        success: function (data) {
                            var duration = data.ResponseType < 0 ? data.ResponseType : null;
                            if (data.ResponseType > 0) {
                                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(8)').html('').html(data.ItemID.status);
                                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).removeAttr('data-pending-charge');
                                if (data.ItemID.status == 'CC Approved') {
                                    PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(8)').addClass('success');
                                    PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(9)').html('');
                                    PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(5)').html('').html(data.ItemID.authCode);
                                    $('#PaymentInfo_InvoiceToRefund').append('<option value="' + data.ItemID.authCode + '">' + data.ItemID.authCode + '</option>')
                                }
                            }
                            PREARRIVAL.updatePaymentsDueSection();
                            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                        }
                    });
                }
                else if ($(e.target).hasClass('delete-transaction')) {
                    UI.confirmBox('Do you confirm you want to proceed?', deleteTransaction, [$(this).attr('id')]);
                }
            });
        }
    }

    function deleteTransaction(id) {
        $.ajax({
            url: '/PreArrival/DeleteTransaction',
            cache: false,
            type: 'POST',
            data: { id: id },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    PREARRIVAL.oPaymentsTable.fnDeleteRow($('#' + data.ItemID)[0]);
                }
                PREARRIVAL.updatePaymentsDueSection();
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var saveLeadSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        var message = data.ResponseMessage;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Lead Saved') {
                //not defined since there is no specific columns for table.
                $('#Search_LeadID').val(data.ItemID.leadID);
                $('#btnSearchPreArrival').trigger('click');
            }
            else {
                if (PREARRIVAL.saveContinueFlag == true) {
                    var currentRow = PREARRIVAL.oTable.$('tr.selected-row').attr('id');
                    var nextRow = $(PREARRIVAL.oTable.$('tr#' + currentRow)[0]).next().attr('id');
                    if (nextRow != undefined) {
                        PREARRIVAL.oTable.$('tr#' + nextRow).trigger('click');
                        message += '<br />Next Lead Selected';
                    }
                    else {
                        var currentPage = PREARRIVAL.oTable.fnPagingInfo().iPage;
                        PREARRIVAL.oTable.fnPageChange('next');
                        if (currentPage != PREARRIVAL.oTable.fnPagingInfo().iPage) {
                            message += '<br />Next Lead Selected';
                            PREARRIVAL.oTable.$('tr:first').trigger('click');
                        }
                    }
                    PREARRIVAL.saveContinueFlag = false;
                }
                else if ($('#Info_DuplicateLead').val() == true) {
                    $('#Info_DuplicateLead').val(false);
                    message += '<br />You need to hit search again in order to see the duplicated lead';
                }
                else {
                    PREARRIVAL.oTable.$('tr.selected-row').trigger('click');
                }
                //$('#PreArrivalPhonesInfoModel_PhonesInfo_PhoneNumber').val('');
                //$('#PreArrivalPhonesInfoModel_PhonesInfo_ExtensionNumber').val('');
                //$('#PreArrivalEmailsInfoModel_EmailsInfo_Email').val('');
            }
            //PREARRIVAL.bindToCheckBoxes();
            PREARRIVAL.editionBasedOnStatus();
        }
        UI.messageBox(data.ResponseType, message + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveBillingSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            var cardTermination = $('#BillingInfo_CardNumber').val().substr(($('#BillingInfo_CardNumber').val().length - 4), 4);
            if (data.ResponseMessage == 'Billing Info Saved') {
                var oSettings = PREARRIVAL.oBillingsTable.fnSettings();
                var iAdded = PREARRIVAL.oBillingsTable.fnAddData([
                    $('#BillingInfo_CardHolderName').val(),
                    $('#BillingInfo_CardType option:selected').text(),
                    ('************' + cardTermination),
                    $('#BillingInfo_CardExpiry').val(),
                    ('**' + $('#BillingInfo_CardCVV').val().substr(($('#BillingInfo_CardCVV').val().length - 2), 2))
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID);
                //$('#PaymentInfo_BillingInfo').prepend('<option value="' + data.ItemID + '">' + $('#BillingInfo_CardType option:selected').text() + ' - ' + cardTermination + '</option>');
                $('#PaymentInfo_BillingInfo option').eq(($('#PaymentInfo_BillingInfo option').length - 1)).before('<option value="' + data.ItemID + '">' + $('#BillingInfo_CardType option:selected').text() + ' - ' + cardTermination + '</option>');
                if (localStorage.Eplat_CreateBilling_Flag == true || localStorage.Eplat_CreateBilling_Flag == 'true') {
                    $('#PaymentInfo_BillingInfo option[value="' + data.ItemID + '"]').attr('selected', true).trigger('change');
                    UI.scrollTo('frmPaymentInfo', null);
                }
                else {
                    PREARRIVAL.oBillingsTable.fnDisplayRow(aRow);
                    UI.tablesHoverEffect();
                    PREARRIVAL.makeTblBillingsSelectable();
                }

            }
            else {
                PREARRIVAL.oBillingsTable.$('tr#' + data.ItemID).children('td:nth-child(1)').text($('#BillingInfo_CardHolderName').val());
                PREARRIVAL.oBillingsTable.$('tr#' + data.ItemID).children('td:nth-child(2)').text($('#BillingInfo_CardType option:selected').text());
                PREARRIVAL.oBillingsTable.$('tr#' + data.ItemID).children('td:nth-child(3)').text(('************' + cardTermination));
                PREARRIVAL.oBillingsTable.$('tr#' + data.ItemID).children('td:nth-child(4)').text($('#BillingInfo_CardExpiry').val());
                PREARRIVAL.oBillingsTable.$('tr#' + data.ItemID).children('td:nth-child(5)').text('**' + $('#BillingInfo_CardCVV').val().substr(($('#BillingInfo_CardCVV').val().length - 2), 2));
                $('#PaymentInfo_BillingInfo').find('option[value="' + data.ItemID + '"]').text($('#BillingInfo_CardType option:selected').text() + ' - ' + cardTermination);
                PREARRIVAL.oBillingsTable.$('tr#' + data.ItemID).removeClass('selected-row secondary');
            }
            $('#frmBillingInfo').clearForm();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveReservationSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Reservation Saved') {
                var oSettings = PREARRIVAL.oReservationsTable.fnSettings();
                var iAdded = PREARRIVAL.oReservationsTable.fnAddData([
                    $('#ReservationInfo_HotelConfirmationNumber').val(),
                    $('#ReservationInfo_FrontCertificateNumber').val(),
                    $('#ReservationInfo_ArrivalDate').val(),
                    $('#ReservationInfo_Destination option:selected').text()
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID);
                PREARRIVAL.oReservationsTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                PREARRIVAL.makeTblReservationsSelectable();
                $('#frmReservationInfo').clearForm();
                PREARRIVAL.oReservationsTable.$('tr#' + data.ItemID).trigger('click');
            }
            else {
                PREARRIVAL.oReservationsTable.$('tr#' + data.ItemID).children('td:nth-child(1)').text($('#ReservationInfo_HotelConfirmationNumber').val());
                PREARRIVAL.oReservationsTable.$('tr#' + data.ItemID).children('td:nth-child(2)').text($('#ReservationInfo_FrontCertificateNumber').val());
                PREARRIVAL.oReservationsTable.$('tr#' + data.ItemID).children('td:nth-child(3)').text($('#ReservationInfo_ArrivalDate').val());
                PREARRIVAL.oReservationsTable.$('tr#' + data.ItemID).children('td:nth-child(4)').text($('#ReservationInfo_Destination option:selected').text());
                //if ($('#ReservationInfo_ReservationComments').val() != '') {
                if ($('#ReservationInfo_ReservationComments').val() != '' && data.ResponseMessage.indexOf('Comments Updated') != -1) {
                    var comments = $('#ReservationInfo_FrontComments').val() + '\r\n'
                        + $('#ufirstname').val() + ' ' + $('#ulastname').val() + ': '
                        + $('#ReservationInfo_ReservationComments').val();
                    $('#ReservationInfo_FrontComments').val(comments);
                    $('#ReservationInfo_ReservationComments').val('');
                }
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var savePresentationSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Presentation Saved') {
                $('#PresentationInfo_PresentationID').val(data.ItemID);
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveOptionSoldSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Option Saved') {
                var oSettings = PREARRIVAL.oOptionsTable.fnSettings();
                var iAdded = PREARRIVAL.oOptionsTable.fnAddData([
                    $('#OptionInfo_Option option:selected').text(),
                    $('#OptionInfo_Quantity').val(),
                    ($('#OptionInfo_Price').val() != '' ? $('#OptionInfo_Price').val() : $('#OptionInfo_Price').attr('placeholder')),
                    $('#OptionInfo_GuestNames').val(),
                    ($('#OptionInfo_Eligible option:selected').val() != 'null' ? $('#OptionInfo_Eligible option:selected').text() : 'False'),
                    ($('#OptionInfo_CreditAmount option:selected').val() != 'null' ? $('#OptionInfo_CreditAmount option:selected').val() : ''),
                    $('#OptionInfo_PointsRedemption').val(),
                    $('#OptionInfo_TotalPaid').val(),
                    $('#OptionInfo_Comments').val(),
                    '<i class="material-icons delete-option" title="delete option">delete</i>'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID);
                PREARRIVAL.oOptionsTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                PREARRIVAL.makeTblOptionsSelectable();
                //$('#frmOptionSoldInfo').clearForm();
            }
            else {
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).children('td:nth-child(1)').text($('#OptionInfo_Option option:selected').text());
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).children('td:nth-child(2)').text($('#OptionInfo_Quantity').val());
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).children('td:nth-child(3)').text(($('#OptionInfo_Price').val() != '' ? $('#OptionInfo_Price').val() : $('#OptionInfo_Price').attr('placeholder')));
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).children('td:nth-child(4)').text($('#OptionInfo_GuestNames').val());
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).children('td:nth-child(5)').text(($('#OptionInfo_Eligible option:selected').val() != 'null' ? $('#OptionInfo_Eligible option:selected').text() : 'False'));
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).children('td:nth-child(6)').text(($('#OptionInfo_CreditAmount').val() != 'null' ? $('#OptionInfo_CreditAmount').val() : ''));
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).children('td:nth-child(7)').text($('#OptionInfo_PointsRedemption').val());
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).children('td:nth-child(8)').text($('#OptionInfo_TotalPaid').val());
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).children('td:nth-child(9)').text($('#OptionInfo_Comments').val());
                PREARRIVAL.oOptionsTable.$('tr#' + data.ItemID).removeClass('selected-row secondary');
            }
            $('#frmOptionSoldInfo').clearForm();
            PREARRIVAL.updatePaymentsDueSection();
            //var options = 0;
            //PREARRIVAL.oOptionsTable.$('tr').each(function () {
            //    options += parseFloat($(this).children('td:nth-child(8)').text().split(' ')[0]);
            //});
            //$('#spanTotalOptionsSold').html(options);
            //var due = parseFloat(options) - parseFloat($('#spanTotalOptionsPayments').find('.money-amount').text().trim().replace(/\,/g, ''));
            //$('#spanTotalOptionsDue').html(due);
            //UI.applyFormat('currency', 'spanTotalOptionsSold,spanTotalOptionsDue');
            $.getJSON('/PreArrival/GetAvailableLetters', { reservationID: $('#ReservationInfo_ReservationID').val() }, function (data) {
                renderLetter(data);
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveFlightSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Flight Saved') {
                var oSettings = PREARRIVAL.oFlightsTable.fnSettings();
                var iAdded = PREARRIVAL.oFlightsTable.fnAddData([
                    $('#FlightInfo_Airline option:selected').text(),
                    $('#FlightInfo_FlightNumber').val(),
                    $('#FlightInfo_PassengerNames').val(),
                    $('#FlightInfo_Passengers option:selected').val(),
                    $('#FlightInfo_FlightType option:selected').text(),
                    $('#FlightInfo_FlightDateTime').val(),
                    $('#FlightInfo_FlightComments').val(),
                    '<i class="material-icons delete-flight" title="delete flight">delete</i>'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID);
                PREARRIVAL.oFlightsTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                PREARRIVAL.makeTblFlightsSelectable();
                //$('#frmFlightInfo').clearForm();
            }
            else {
                PREARRIVAL.oFlightsTable.$('tr#' + data.ItemID).children('td:nth-child(1)').text($('#FlightInfo_Airline option:selected').text());
                PREARRIVAL.oFlightsTable.$('tr#' + data.ItemID).children('td:nth-child(2)').text($('#FlightInfo_FlightNumber').val());
                PREARRIVAL.oFlightsTable.$('tr#' + data.ItemID).children('td:nth-child(3)').text($('#FlightInfo_PassengerNames').val());
                PREARRIVAL.oFlightsTable.$('tr#' + data.ItemID).children('td:nth-child(4)').text($('#FlightInfo_Passengers option:selected').val());
                PREARRIVAL.oFlightsTable.$('tr#' + data.ItemID).children('td:nth-child(5)').text($('#FlightInfo_FlightType option:selected').text());
                PREARRIVAL.oFlightsTable.$('tr#' + data.ItemID).children('td:nth-child(6)').text($('#FlightInfo_FlightDateTime').val());
                PREARRIVAL.oFlightsTable.$('tr#' + data.ItemID).children('td:nth-child(7)').text($('#FlightInfo_FlightComments').val());
                PREARRIVAL.oFlightsTable.$('tr#' + data.ItemID).removeClass('selected-row secondary');
            }
            $('#frmFlightInfo').clearForm();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var savePaymentSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Payment Saved') {
                var oSettings = PREARRIVAL.oPaymentsTable.fnSettings();
                var iAdded = PREARRIVAL.oPaymentsTable.fnAddData([
                    $('#PaymentInfo_TransactionType option:selected').text(),
                    $('#PaymentInfo_PaymentType option:selected').text(),
                    ($('#PaymentInfo_Amount').val() + ' ' + $('#PaymentInfo_Currency option:selected').val()),
                    $('#PaymentInfo_ChargeType option:selected').text(),
                    //$('#PaymentInfo_ChargeDescription option:selected').text(),
                    ($('#PaymentInfo_Transaction').val() + ($('#PaymentInfo_TransactionType option:selected').val() == 2 && $('#PaymentInfo_PaymentType option:selected').val() == 2 ? 'Refunding ' + $('#PaymentInfo_InvoiceToRefund option:selected').text() : '')),
                    $('#PaymentInfo_PaymentComments').val(),
                    data.ItemID.dateSaved,
                    //($('#PaymentInfo_PaymentType option:selected').val() == 2 && data.ItemID.status == "Pending" && $('#PaymentInfo_TransactionType option:selected').val() == 1 ? '<i class="material-icons apply-charge" title="apply charge">payment</i>' : data.ItemID.status)
                    data.ItemID.status,
                    ($('#PaymentInfo_PaymentType option:selected').val() == 2 && $('#PaymentInfo_TransactionType option:selected').val() == 1 && data.ItemID.status == 'Pending' ? '<i class="material-icons apply-charge" title="apply charge">payment</i>' : ''),
                    ($('#PaymentInfo_TransactionType option:selected').val() == 2 || $('#PaymentInfo_PaymentType option:selected').val() == 2 || data.ItemID.status == 'Pending' ? '<i class="material-icons delete-transaction">delete</i>' : '')
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID.id);
                if (data.ItemID.status == 'Pending') {
                    aRow.setAttribute('data-pending-charge', 'true');
                }
                PREARRIVAL.oPaymentsTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                PREARRIVAL.makeTblPaymentsSelectable();
                $(document).tooltip();
                $('#frmPaymentInfo').clearForm();
                $('#PaymentInfo_TransactionType option[value="1"]').attr('selected', true).trigger('change');
            }
            else {
                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(1)').text($('#PaymentInfo_TransactionType option:selected').text());
                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(2)').text($('#PaymentInfo_PaymentType option:selected').text());
                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(3)').text($('#PaymentInfo_Amount').val() + ' ' + $('#PaymentInfo_Currency option:selected').val());
                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(4)').text($('#PaymentInfo_ChargeType option:selected').text());
                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(5)').text(($('#PaymentInfo_Transaction').val() + ($('#PaymentInfo_TransactionType option:selected').val() == 2 && $('#PaymentInfo_PaymentType option:selected').val() == 2 ? 'Refunding ' + $('#PaymentInfo_InvoiceToRefund option:selected').text() : '')));
                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(6)').text($('#PaymentInfo_PaymentComments').val());
                if ($('#PaymentInfo_DateSaved').is(':visible')) {
                    PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(7)').text($('#PaymentInfo_DateSaved').val());
                }
                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(8)').html(data.ItemID.status);
                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(9)').html($('#PaymentInfo_PaymentType option:selected').val() == 2 && data.ItemID.status == "Pending" && $('#PaymentInfo_TransactionType option:selected').val() == 1 ? '<i class="material-icons apply-charge" title="apply charge">payment</i>' : '');
                PREARRIVAL.oPaymentsTable.$('tr#' + data.ItemID.id).children('td:nth-child(10)').html($('#PaymentInfo_TransactionType option:selected').val() == 2 || $('#PaymentInfo_PaymentType option:selected').val() == 2 || data.ItemID.status == 'Pending' ? '<i class="material-icons delete-transaction">delete</i>' : '');
            }
            PREARRIVAL.updatePaymentsDueSection();

        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var updatePaymentsDueSection = function () {
        var payments = 0;
        var options = 0;
        PREARRIVAL.oPaymentsTable.$('tr').each(function () {
            if ($(this).children('td:nth-child(1)').text().toLowerCase().trim() == 'payment') {
                if ($(this).children('td:nth-child(8)').text().toLowerCase().trim().indexOf('approved') != -1) {
                    payments += parseFloat($(this).children('td:nth-child(3)').text().split(' ')[0]);
                }
            }
            else {
                payments -= parseFloat($(this).children('td:nth-child(3)').text().split(' ')[0]);
            }
        });
        PREARRIVAL.oOptionsTable.$('tr').each(function () {
            options += parseFloat($(this).children('td:nth-child(8)').text().split(' ')[0]);
        });
        $('#spanTotalOptionsPayments').html(payments);
        $('#spanTotalOptionsSold').html(options);
        var due = parseFloat(options) - parseFloat(payments);
        $('#spanTotalOptionsDue').html(due);
        UI.applyFormat('currency', 'spanTotalOptionsSold,spanTotalOptionsPayments,spanTotalOptionsDue');
    }

    var removeEmail = function () {
        //$('.email-visible').unbind('click').on('click', function () {
        //    var $this = $(this);
        //    UI.confirmBox(
        //        'You are informed that the information displayed on this page is protected by the federal law of protection of personal data in possession of individuals, by accessing this information constitutes your express acceptance as responsible for the treatment of personal data, we ask you not make misuse of them, since you could be punished for crimes in the matter of misrepresentation of personal data.',
        //        getEmail,
        //        [$('#Info_LeadID').val(), $this.parents('tr:first')]
        //        );

        //    function getEmail(id, tr) {
        //        $.ajax({
        //            url: 'PreArrival/GetEmail',
        //            cache: false,
        //            type: 'POST',
        //            data: { id: id },
        //            success: function (data) {
        //                if (data[0].EmailsInfo_LeadEmailID == 0) {
        //                    UI.messageBox(-1, 'There is no Email Address related to this lead.', null, null);
        //                }
        //                else {
        //                    tr.remove();
        //                    renderEmail(data, true);
        //                }
        //            }
        //        });
        //    }
        //});

        $('#tblPreArrivalEmails tbody tr').not('theader').unbind('click').on('click', function (e) {
            var $this = $(this);
            if (!$(e.target).hasClass('email-visible') && $this.find('.email-visible').length == 0) {
                if (!$(this).hasClass('selected-row')) {
                    $(this).parent('tbody').find('editing-row').removeClass('editing-row selected-row');
                    $(this).addClass('editing-row selected-row');
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(1)').children('input:hidden').val($(this).children('td:nth-child(1)').text().trim());
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(2)').children('input:text').val($(this).children('td:nth-child(2)').text().trim());
                    if ($(this).children('td:nth-child(3)').text().trim() == '' || $(this).children('td:nth-child(3)').text().trim() == 'false') {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(3)').children('input:checkbox').removeAttr('checked');
                    }
                    else {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(3)').children('input:checkbox').attr('checked', 'checked');
                    }
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(4)').children('.item-edition').toggle();
                }
            }
            else if ($(e.target).hasClass('email-visible')) {
                //code for admin
                if ($('#divSelectedRole').text() == 'Administrator') {
                    getEmail($('#Info_LeadID').val(), $this.parents('tr:first'));
                }
                else {
                    //
                    UI.confirmBox(
                        'You are informed that the information displayed on this page is protected by the federal law of protection of personal data in possession of individuals, by accessing this information constitutes your express acceptance as responsible for the treatment of personal data, we ask you not make misuse of them, since you could be punished for crimes in the matter of misrepresentation of personal data.',
                        getEmail,
                        [$('#Info_LeadID').val(), $this.parents('tr:first')]
                    );
                }
                function getEmail(id, tr) {
                    $.ajax({
                        url: 'PreArrival/GetEmail',
                        cache: false,
                        type: 'POST',
                        data: { id: id },
                        success: function (data) {
                            //if (data[0].EmailsInfo_LeadEmailID == 0) {
                            if (data[0].EmailsInfo_Email == '') {
                                UI.messageBox(-1, 'There is no Email Address related to this lead.', null, null);
                            }
                            else {
                                $('.email-visible').parents('tr:first').remove();
                                renderEmail(data, true);
                            }
                        }
                    });
                }
            }
        });
        //PREARRIVAL.contactInfoEdition();

    }

    var removePhone = function () {
        $('.remove-phone').unbind('click').on('click', function () {
            var $tr = $(this).parents('tr:first');
            var array = $.parseJSON($('#PreArrivalPhones').val());
            var index = array.map(function (_item) { return _item.PhonesInfo_LeadPhoneID; }).indexOf(parseInt($tr.children('td:nth-child(1)').text()));
            if (index != -1) {
                array.splice(index, 1);
                $('#PreArrivalPhones').val(JSON.stringify(array));
                $tr.remove();
            }
        });

        $('#tblPreArrivalPhones tbody tr').not('theader').unbind('click').on('click', function (e) {
            var $this = $(this);
            if (!$(e.target).hasClass('click-to-call')) {
                if ($(e.target).hasClass('phone-visible')) {
                    if ($('#divSelectedRole').text() == 'Administrator') {
                        getPhone($('#Info_LeadID').val(), $this.parents('tr:first'));
                    }
                    else {
                        UI.confirmBox(
                            'You are informed that the information displayed on this page is protected by the federal law of protection of personal data in possession of individuals, by accessing this information constitutes your express acceptance as responsible for the treatment of personal data, we ask you not make misuse of them, since you could be punished for crimes in the matter of misrepresentation of personal data.',
                            getPhone,
                            [$('#Info_LeadID').val(), $this.parents('tr:first')]
                        );
                    }
                    function getPhone(id, tr) {
                        $.ajax({
                            url: 'PreArrival/GetPhone',
                            cache: false,
                            type: 'POST',
                            data: { id: id },
                            success: function (data) {
                                if (data[0].PhonesInfo_PhoneNumber == '') {
                                    UI.messageBox(-1, 'There is no Phone Number related to this lead.', null, null);
                                }
                                else {
                                    $('.phone-visible').parents('tr:first').remove();
                                    renderPhone(data, true);
                                }
                            }
                        });
                    }
                }
                else if (!$(this).hasClass('selected-row')) {
                    $(this).parent('tbody').find('.editing-row').removeClass('editing-row selected-row');
                    $(this).addClass('editing-row selected-row');
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(1)').children('input:hidden').val($(this).children('td:nth-child(1)').text().trim());
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(2)').children('select').children('option').filter(function () { return $(this).html() == $this.children('td:nth-child(2)').text().trim(); }).attr('selected', true);
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(3)').children('input:text').val($(this).children('td:nth-child(3)').text().trim());
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(4)').children('input:text').val($(this).children('td:nth-child(4)').text().trim());
                    if ($(this).children('td:nth-child(5)').text().trim() == '' || $(this).children('td:nth-child(5)').text().trim() == 'false') {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(5)').children('input:checkbox').removeAttr('checked');
                    }
                    else {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(5)').children('input:checkbox').attr('checked', 'checked');
                    }
                    if ($(this).children('td:nth-child(6)').text().trim() == '' || $(this).children('td:nth-child(6)').text().trim() == 'false') {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(6)').children('input:checkbox').removeAttr('checked');
                    }
                    else {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(6)').children('input:checkbox').attr('checked', 'checked');
                    }
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(7)').children('.item-edition').toggle();
                }
            }
            else if ($(e.target).hasClass('click-to-call')) {
                var leadID = $('#Info_LeadID').val();
                var phoneID = $(e.target).data('reference-phone-id') != undefined ? $(e.target).data('reference-phone-id') : null;
                $.ajax({
                    url: '/Prearrival/ClickToCall',
                    type: 'POST',
                    data: { leadID: leadID, phoneID: phoneID },
                    success: function (data) {
                        UI.messageBox(data.Type, data.Message, null, null);
                    }
                });
            }
        });
    }

    var _removePhone = function () {
        $('.remove-phone').unbind('click').on('click', function () {
            var $tr = $(this).parents('tr:first');
            var array = $.parseJSON($('#PreArrivalPhones').val());
            var index = array.map(function (_item) { return _item.PhonesInfo_LeadPhoneID; }).indexOf(parseInt($tr.children('td:nth-child(1)').text()));
            if (index != -1) {
                array.splice(index, 1);
                $('#PreArrivalPhones').val(JSON.stringify(array));
                $tr.remove();
            }
        });

        $('#tblPreArrivalPhones tbody tr').not('theader').unbind('click').on('click', function (e) {
            var $this = $(this);
            if (!$(e.target).hasClass('phone-visible') && $this.find('.phone-visible').length == 0) {
                if (!$(this).hasClass('selected-row')) {
                    $(this).parent('tbody').find('.editing-row').removeClass('editing-row selected-row');
                    $(this).addClass('editing-row selected-row');
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(1)').children('input:hidden').val($(this).children('td:nth-child(1)').text().trim());
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(2)').children('select').children('option').filter(function () { return $(this).html() == $this.children('td:nth-child(2)').text().trim(); }).attr('selected', true);
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(3)').children('input:text').val($(this).children('td:nth-child(3)').text().trim());
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(4)').children('input:text').val($(this).children('td:nth-child(4)').text().trim());
                    if ($(this).children('td:nth-child(5)').text().trim() == '' || $(this).children('td:nth-child(5)').text().trim() == 'false') {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(5)').children('input:checkbox').removeAttr('checked');
                    }
                    else {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(5)').children('input:checkbox').attr('checked', 'checked');
                    }
                    if ($(this).children('td:nth-child(6)').text().trim() == '' || $(this).children('td:nth-child(6)').text().trim() == 'false') {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(6)').children('input:checkbox').removeAttr('checked');
                    }
                    else {
                        $this.parents('table').find('tfoot tr:first').children('td:nth-child(6)').children('input:checkbox').attr('checked', 'checked');
                    }
                    $this.parents('table').find('tfoot tr:first').children('td:nth-child(7)').children('.item-edition').toggle();
                }
            }
            else if ($(e.target).hasClass('phone-visible')) {
                if ($('#divSelectedRole').text() == 'Administrator') {
                    getPhone($('#Info_LeadID').val(), $this.parents('tr:first'));
                }
                else {
                    UI.confirmBox(
                        'You are informed that the information displayed on this page is protected by the federal law of protection of personal data in possession of individuals, by accessing this information constitutes your express acceptance as responsible for the treatment of personal data, we ask you not make misuse of them, since you could be punished for crimes in the matter of misrepresentation of personal data.',
                        getPhone,
                        [$('#Info_LeadID').val(), $this.parents('tr:first')]
                    );
                }
                function getPhone(id, tr) {
                    $.ajax({
                        url: 'PreArrival/GetPhone',
                        cache: false,
                        type: 'POST',
                        data: { id: id },
                        success: function (data) {
                            //if (data[0].PhonesInfo_LeadPhoneID == 0) {
                            if (data[0].PhonesInfo_PhoneNumber == '') {
                                UI.messageBox(-1, 'There is no Phone Number related to this lead.', null, null);
                            }
                            else {
                                $('.phone-visible').parents('tr:first').remove();
                                renderPhone(data, true);
                            }
                        }
                    });
                }
            }
        });
    }

    var searchToImportSuccess = function (data) {
        var str = '<div class="table-row">';
        $.each(data, function (index, item) {
            str += '<div class="table-cell">'
                + '<div class="table-row text-center"><h3>' + item.Resort + '</h3></div>';
            $.each(item.Cosa, function (index, item) {
                str += '<div class="table-row"><div class="table-cell text-center"><strong>' + item.Key + '</strong></div></div>';
                $.each(item.Value, function (index, item) {
                    str += '<div class="table-row"><div class="table-cell" style="text-transform:capitalize;">' + item.Value.toLowerCase() + '</div><div class="table-cell">' + item.Text + '</div></div>';
                });
            });
            str += '<div class="table-row"></div>'
                + '</div>';

            //str += '<div class="table-cell">'
            //    //+ '<div class="table-row text-center"><h3>' + $('#SearchToImport_Resort option[value="' + item.Resort + '"]').text() + '</h3></div>';
            //    + '<div class="table-row text-center"><h3>' +  item.Resort + '</h3></div>';
            //$.each(item.UserCodeCountList, function (index, item) {
            //    str += '<div class="table-row"><div class="table-cell text-center">' + item.Key + '</div></div>';
            //    str += '<div class="table-row"><div class="table-cell">' + item.Value.Value + '</div><div class="table-cell">' + item.Value.Text + '</div></div>';
            //});
            //str += '<div class="table-row"></div>'
            //+ '</div>';
        });
        str += '</div>';
        $('.import-results').html(str);
        $('#importRange').text($('#Search_I_ImportArrivalDate').val() + ' - ' + $('#Search_F_ImportArrivalDate').val());
        $.fancybox.close();
    }

    //var searchToImport = function (data) {
    //    vArrival.Arrivals = data;

    //    //var tableColumns = $(data).find('tbody tr').first().find('td');
    //    //UI.searchResultsTable('tblArrivalsToImport', tableColumns.length - 1);
    //    //PREARRIVAL.oImport = $('#tblArrivalsToImport').dataTable();
    //}

    var clearForms = function () {
        $(document).unbind('keydown').on('keydown', function (e) {
            if (e.keyCode === 27) {
                var $element = $(document).find('.fds-active:has(.selected-row:visible)').last();
                $element.find('form').clearForm(true);
                var $row = $element.find('.selected-row').first();
                if ($row.hasClass('secondary')) {
                    var $items = $element.find('.secondary-selected-row-dependent');
                    /////new
                    var counter = 0;
                    $.each($items, function (i, item) {
                        if ($(item).is('tbody')) {
                            counter++;
                            if (counter > 1) {
                                $(this).empty();
                            }
                        }
                    });
                    /////new
                    //$.each($items, function (i, item) {
                    //    if (i != 0) {
                    //        //$(this).val();
                    //        if ($(item).is('tbody')) {
                    //            $(this).empty();
                    //        }
                    //        //$(this).text('');
                    //    }
                    //});
                    $row.removeClass('selected-row secondary');
                }
                else if ($row.hasClass('primary')) {
                    var $items = $element.find('.primary-selected-row-dependent');
                    $.each($items, function (i, item) {
                        if (i != 0) {
                            //$(this).val();
                            if ($(item).is('tbody')) {
                                $(this).empty();
                            }
                            else {

                            }
                            //$(this).text('');
                        }
                    });
                    $row.removeClass('selected-row primary');
                }
            }
        });
    }

    var bindToCheckBoxes = function () {

        $('#lblSelectAll').unbind('click').on('click', function (e) {
            e.preventDefault();
            if ($('.mass-update-coincidences').val().split(',').length == PREARRIVAL.oTable.$('tr').length) {
                PREARRIVAL.oTable.$('tr').each(function () {
                    var $this = $(this);
                    $(this).children('td:nth-child(1)').children('.chk-son')[0].checked = false;
                    PREARRIVAL.leads = PREARRIVAL.leads.filter(function (evt) { return evt !== $this.children('td:nth-child(1)').children('.chk-son').data('id') });
                });
            }
            else {
                PREARRIVAL.oTable.$('tr').each(function () {
                    var $this = $(this);
                    if (!$(this).children('td:nth-child(1)').children('.chk-son').is(':checked')) {
                        $(this).children('td:nth-child(1)').children('.chk-son')[0].checked = true;
                        PREARRIVAL.leads.push($(this).children('td:nth-child(1)').children('.chk-son').data('id'));
                    }
                });
            }
            $('.mass-update-coincidences').val(PREARRIVAL.leads.toString());
            $('.span-selected-leads').html(PREARRIVAL.leads.length + ' lead(s) selected');
        });

        $('#tblSearchPreArrivalResults thead .chk-visible').unbind('click').on('click', function (e) {
            e.preventDefault();
            if ($('#tblSearchPreArrivalResults tbody').find('.chk-son:checked').length == $('#tblSearchPreArrivalResults tbody tr').length) {
                $('#tblSearchPreArrivalResults tbody tr').each(function () {
                    var $this = $(this);
                    $(this).children('td:nth-child(1)').children('.chk-son')[0].checked = false;
                    PREARRIVAL.leads = PREARRIVAL.leads.filter(function (evt) { return evt !== $this.children('td:nth-child(1)').children('.chk-son').data('id') });
                });
            }
            else {
                $('#tblSearchPreArrivalResults tbody tr').each(function () {
                    var $this = $(this);
                    if (!$(this).children('td:nth-child(1)').children('.chk-son').is(':checked')) {
                        $(this).children('td:nth-child(1)').children('.chk-son')[0].checked = true;
                        PREARRIVAL.leads.push($(this).children('td:nth-child(1)').children('.chk-son').data('id'));
                    }
                });
            }
            $('.mass-update-coincidences').val(PREARRIVAL.leads.toString());
            $('.span-selected-leads').html(PREARRIVAL.leads.length + ' lead(s) selected');
        });

        PREARRIVAL.oTable.$('.chk-son').unbind('click').on('click', function (e) {
            if ($(e.target).is(':checked')) {
                PREARRIVAL.leads.push($(e.target).data('id'));
            }
            else {
                PREARRIVAL.leads = PREARRIVAL.leads.filter(function (evt) { return evt !== $(e.target).data('id') });
            }
            $('.mass-update-coincidences').val(PREARRIVAL.leads.toString());
            $('.span-selected-leads').html(PREARRIVAL.leads.length + ' lead(s) selected');
        });
    }

    var _bindToCheckBoxes = function () {
        $('#tblSearchPreArrivalResults thead .chk-parent').unbind('click').on('click', function (e) {
            PREARRIVAL.oTable.$('tr').each(function () {
                var $this = $(this);
                if ($(e.target).is(':checked')) {
                    $(this).children('td:nth-child(1)').children('.chk-son')[0].checked = true;
                    PREARRIVAL.leads.push($(this).children('td:nth-child(1)').children('.chk-son').data('id'));
                }
                else {
                    $(this).children('td:nth-child(1)').children('.chk-son')[0].checked = false;
                    PREARRIVAL.leads = PREARRIVAL.leads.filter(function (evt) { return evt !== $this.children('td:nth-child(1)').children('.chk-son').data('id') });
                }
            });
            $('.mass-update-coincidences').val(PREARRIVAL.leads.toString());
            $('.span-selected-leads').html(PREARRIVAL.leads.length + ' lead(s) selected');
        });

        PREARRIVAL.oTable.$('.chk-son').unbind('click').on('click', function (e) {
            if ($(e.target).is(':checked')) {
                PREARRIVAL.leads.push($(e.target).data('id'));
            }
            else {
                PREARRIVAL.leads = PREARRIVAL.leads.filter(function (evt) { return evt !== $(e.target).data('id') });
            }
            $('.mass-update-coincidences').val(PREARRIVAL.leads.toString());
            $('.span-selected-leads').html(PREARRIVAL.leads.length + ' lead(s) selected');
        });
    }

    var contactInfoEdition = function () {
        //$('.edit-email, .edit-phone').unbind('click').on('click', function () {
        //    $(this).parents('tr:first').children('td').not(':last').each(function (index, item) {
        //        var val = $(item).text().trim();
        //        $(this).attr('data-val', val);

        //        if (val == 'check') {
        //            $(item).html('<input type="checkbox" checked="checked" />');
        //        }
        //        else if ($(this).attr('data-type') != undefined && $(this).attr('data-type') == 'bool') {
        //            $(item).html('<input type="checkbox" />');
        //        }
        //        else {
        //            $(item).html('<input type="text" value="' + val + '" />');
        //        }

        //        //if (val != 'check') {
        //        //    $(item).html('<input type="text" value="' + val + '" />');
        //        //}
        //        //else {
        //        //    $(item).html('<input type="checkbox" checked="checked" />');
        //        //}
        //    });

        //    $(this).parents('tr:first').find('.item-edition').toggle();
        //});

        //$('.confirm-update').unbind('click').on('click', function () {
        //    $.ajax({
        //        url: '',
        //        type: 'POST',
        //        cache: false,
        //        data: {},
        //        success: function (data) {

        //        }
        //    });
        //});

        //$('.cancel-update').unbind('click').on('click', function () {
        //    $(this).parents('tr:first').children('td').not(':last').each(function (index, item) {
        //        var val = $(this).attr('data-val');
        //        if (val == 'check') {
        //            $(this).html('<i class="material-icons sm">check</i>');
        //        }
        //        else {
        //            $(this).html('');
        //            $(this).text(val);
        //        }


        //    });

        //    $(this).parents('tr:first').find('.item-edition').toggle();
        //});

        //$('#tblPreArrivalPhones tbody tr').not('theader').unbind('click').on('click', function () {
        //    var $this = $(this);
        //    $(this).parent('tbody').find('.editing-row').removeClass('editing-row selected-row');
        //    $(this).addClass('editing-row selected-row');
        //    $this.parents('table').find('tfoot tr:first').children('td:nth-child(1)').children('input:hidden').val($(this).children('td:nth-child(1)').attr('data-val'));
        //    $this.parents('table').find('tfoot tr:first').children('td:nth-child(2)').children('select option[text="' + $(this).children('td:nth-child(2)').text().trim() + '"]').attr('selected', true);
        //    $this.parents('table').find('tfoot tr:first').children('td:nth-child(3)').children('input:text').val($(this).children('td:nth-child(3)').text().trim());
        //    $this.parents('table').find('tfoot tr:first').children('td:nth-child(4)').children('input:text').val($(this).children('td:nth-child(4)').text().trim());
        //    if ($(this).children('td:nth-child(5)').attr('data-val') == 'false') {
        //        $this.parents('table').find('tfoot tr:first').children('td:nth-child(5)').children('input:checkbox').removeAttr('checked');
        //    }
        //    else {
        //        $this.parents('table').find('tfoot tr:first').children('td:nth-child(5)').children('input:checkbox').attr('checked', 'checked');
        //    }
        //    if ($(this).children('td:nth-child(6)').attr('data-val') == 'false') {
        //        $this.parents('table').find('tfoot tr:first').children('td:nth-child(6)').children('input:checkbox').removeAttr('checked');
        //    }
        //    else {
        //        $this.parents('table').find('tfoot tr:first').children('td:nth-child(6)').children('input:checkbox').attr('checked', 'checked');
        //    }
        //    $this.parents('table').find('tfoot tr:first').children('td:nth-child(7)').children('.item-edition').toggle();

        //});
    }

    var clearTable = function (item) {

        if (!$.fn.DataTable.fnIsDataTable(document.getElementById(item))) {
            $('#' + item + ' tbody').empty();
        }
        else {
            $('#' + item).dataTable().fnClearTable();
        }
    }

    var editionBasedOnStatus = function () {
        if ($('#Info_LeadStatus option:selected').val() == 10) {
            $('#btnSaveOptionSold').css('display', 'none');
            $('#btnSavePayment').css('display', 'none');
        }
        else {
            $('#btnSaveOptionSold').css('display', 'inline-block');
            $('#btnSavePayment').css('display', 'inline-block');
        }
    }

    return {
        LeadGroups: LeadGroups,
        Layouts: Layouts,
        Airlines: Airlines,
        Filters: Filters,
        Columns: Columns,
        init: init,
        getFields: getFields,
        removeProperty: removeProperty,
        selectLayout: selectLayout,
        loadReportLayouts: loadReportLayouts,
        searchResultsTable: searchResultsTable,
        makeTableRowsSelectable: makeTableRowsSelectable,
        makeTblBillingsSelectable: makeTblBillingsSelectable,
        makeTblReservationsSelectable: makeTblReservationsSelectable,
        makeTblOptionsSelectable: makeTblOptionsSelectable,
        makeTblFlightsSelectable: makeTblFlightsSelectable,
        makeTblPaymentsSelectable: makeTblPaymentsSelectable,
        saveLeadSuccess: saveLeadSuccess,
        saveBillingSuccess: saveBillingSuccess,
        saveReservationSuccess: saveReservationSuccess,
        savePresentationSuccess: savePresentationSuccess,
        saveOptionSoldSuccess: saveOptionSoldSuccess,
        saveFlightSuccess: saveFlightSuccess,
        savePaymentSuccess: savePaymentSuccess,
        removeEmail: removeEmail,
        removePhone: removePhone,
        searchToImportSuccess: searchToImportSuccess,
        clearForms: clearForms,
        bindToCheckBoxes: bindToCheckBoxes,
        contactInfoEdition: contactInfoEdition,
        resetAll: resetAll,
        updatePaymentsDueSection: updatePaymentsDueSection,
        clearTable: clearTable,
        reassignOptions: reassignOptions,
        editionBasedOnStatus: editionBasedOnStatus
        //searchToImport: searchToImport
    }
}();

var MASSUPDATE = function () {
    var init = function () {
        PREARRIVAL.leads = new Array();
        $('#btnMassUpdate').unbind('click').on('click', function () {
            var leads = '';
            if ($('#flagAll').val() == 'true') {
                var tempLeads = $('#Coincidences').val().split(',').join("','");
                leads = "'" + tempLeads;
                leads += "'";
            }
            else {
                leads += "'";
                PREARRIVAL.oTable.$('tr').each(function (index, item) {
                    if ($(this)[0].cells[0].childNodes[1].checked == true) {
                        leads += $(this).attr('id') + "','";
                    }
                });
                leads = leads.substr(0, leads.length - 2);
            }
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
                PREARRIVAL.oTable.$('tr').each(function (index, item) {
                    if ($(this)[0].cells[0].childNodes[1].checked == true) {
                        leads += $(this).attr('id') + "','";
                    }
                });
                leads = leads.substr(0, leads.length - 2);
            }
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
                leads = "'" + tempLeads;
                leads += "'";
            }
            else {
                leads += "'";
                PREARRIVAL.oTable.$('tr').each(function (index, item) {
                    if ($(this)[0].cells[0].childNodes[1].checked == true) {
                        leads += $(this).attr('id') + "','";
                    }
                });
                leads = leads.substr(0, leads.length - 2);
            }
            $('.mass-update-coincidences').val(leads);
            var _event = $(e.target).attr('data-sysevent');
            $('#MassUpdate_SendingEvent').val(_event);
            if (leads != '') {
                $('#frmMassSending').submit();
                console.log(leads);
            }
            else {
                UI.messageBox(0, "No Leads Selected", null, null);
            }
        });
    }

    var massUpdateSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            $('.mass-update-coincidences').val('');
            PREARRIVAL.leads = new Array();
            $('#tblSearchPreArrivalResults thead .chk-parent')[0].checked = false;
            PREARRIVAL.oTable.$('.chk-son').each(function () {
                $(this)[0].checked = false;
            });
            $('.span-selected-leads').html('0 lead(s) selected');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    return {
        init: init,
        massUpdateSuccess: massUpdateSuccess
    }
}();

//var vArrival = new Vue({
//    mixins: [ePlatUtils],
//    el: '#app',
//    data:
//        function () {
//            return {
//                Shared: ePlatStore,
//                Arrivals: [],
//                XHRArrivals: null
//            }
//        },
//    //},
//    methods: {
//        //getArrivalsToImport: function () {
//        //    let self = this;
//        //    if (this.XHRArrivals && this.XHRArrivals.readyState != 4) {
//        //        this.XHRArrivals.abort();
//        //    }
//        //    this.XHRArrivals = $.ajax({
//        //        url: '/crm/PreArrival/GetArrivalsToImport',
//        //        cache: false,
//        //        type: 'POST',
//        //        data: {},
//        //        success: function (data) {

//        //        }
//        //    });
//        //}
//    },
//    mounted: function () {
//        let self = this;

//    }
//})

$(function () {
    PREARRIVAL.init();
    MASSUPDATE.init();
});