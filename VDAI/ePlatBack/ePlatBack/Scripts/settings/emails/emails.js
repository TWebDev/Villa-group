$(function () {
    EMAIL.init();
});

var EMAIL = new function () {

    var oEmailTable;
    var oNotificationsTable;
    var oFieldGroupsTable;
    var oFieldsTable;

    var init = function () {

        UI.applyCKEditor();
        EMAIL.searchResultsTable();
        EMAIL.searchNotificationsResultsTable();
        EMAIL.searchFieldGroupsResultsTable();
        EMAIL.searchFieldsResultsTable();

        var _resorts;

        $('select[multiple="multiple"]').multiselect({
            noneSelectedText: "--Select One--",
            minWidth: "auto",
            selectedList: 1
        });

        $('#EmailNotificationInfo_Destinations').multiselect({
            noneSelectedText: "--Select One--",
            minWidth: "auto",
            selectedList: 1,
            close: function (e, ui) {//find out how to receive extra parameters
                var _dest = '';
                $('#EmailNotificationInfo_Destinations').multiselect('getChecked').each(function () { _dest += (_dest != '' ? ',' : '') + $(this).val(); });
                $.getJSON('/Emails/GetDDLData', { itemType: 'resorts', itemID: _dest }, function (data) {
                    $('#EmailNotificationInfo_Resorts').clearSelect();
                    $('#EmailNotificationInfo_Resorts').fillSelect(data);
                    if (_resorts != undefined) {
                        $.each(_resorts, function (index, item) {
                            $('#EmailNotificationInfo_Resorts option[value="' + item + '"]').attr('selected', true);
                        });
                    }
                    $('#EmailNotificationInfo_Resorts').multiselect('refresh');
                });
            }
        });

        $('#EmailInfo_Culture').on('change', function (e, params) {
            $.getJSON('/Emails/GetDDLData', { itemType: 'templatesByCulture', itemID: $(this).val() }, function (data) {
                $('#EmailInfo_EmailTemplate').clearSelect();
                $('#EmailInfo_EmailTemplate').fillSelect(data);
                var _template = params != undefined ? params.template : 0;
                $('#EmailInfo_EmailTemplate option[value="' + _template + '"]').attr('selected', true);
            });
        });

        $('#FieldsInfo_FieldType').on('change', function (e, params) {
            $.getJSON('/Emails/GetDDLData', { itemType: 'fieldSubTypes', itemID: $(this).val() }, function (data) {
                $('#FieldsInfo_FieldSubType').clearSelect();
                $('#FieldsInfo_FieldSubType').fillSelect(data);
                var _subType = params != undefined ? params.subType : 0;
                $('#FieldsInfo_FieldSubType option[value="' + _subType + '"]').attr('selected', true);
            });
        });

        $('#EmailNotificationInfo_Terminal').on('change', function (e, params) {
            $.getJSON('/Emails/GetDDLData', { itemType: 'destinationsPerTerminal', itemID: $(this).val() }, function (data) {
                $('#EmailNotificationInfo_Destinations').clearSelect();
                $('#EmailNotificationInfo_Destinations').fillSelect(data);
                if (params != undefined) {
                    $.each(params.destinations, function (index, item) {
                        $('#EmailNotificationInfo_Destinations option[value="' + item + '"]').attr('selected', true);
                    });
                }
                _resorts = params != undefined ? params.resorts : params;
                $('#EmailNotificationInfo_Destinations').multiselect('refresh');
                $('#EmailNotificationInfo_Destinations').multiselect('close');//find out how to send parameters
            });
            $.getJSON('/Emails/GetDDLData', { itemType: 'emails', itemID: $(this).val() }, function (data) {
                $('#EmailNotificationInfo_Email').fillSelect(data);
                var _email = params != undefined ? params.email : 0;
                $('#EmailNotificationInfo_Email option[value="' + _email + '"]').attr('selected', true);
            });
            $.getJSON('/Emails/GetDDLData', { itemType: 'events', itemID: $(this).val() }, function (data) {
                $('#EmailNotificationInfo_Event').fillSelect(data);
                var _event = params != undefined ? params.event : 0;
                $('#EmailNotificationInfo_Event option[value="' + _event + '"]').attr('selected', true);
            });
        });

        $('#btnSingleEmailSending').on('click', function () {
            $.fancybox({
                overlay : {
                    closeClick : true
                },
                type: 'ajax',
                href: '/Emails/RenderSingleSending',
                ajax: { type: 'POST', data: { fieldGroupID: $('#FieldGroupsInfo_FieldGroupID').val() } },
                //href: $('#divFastSaleContainer'),
                //modal: true,
                afterShow: function () {
                    $('*[data-uses-date-picker="true"]').each(function (index, item) {
                        $(this).datepicker({
                            dateFormat: 'yy-mm-dd',
                            changeMonth: true,
                            changeYear: true,
                            minDate: 0
                            //minDate: $(item).attr('data-start-date-picker') != undefined ? $(item).attr('data-start-date-picker') == 'null' ? null : parseInt($(item).attr('data-start-date-picker')) : 0,
                        });

                    });
                    $('*[data-uses-datetime-picker="true"]').each(function (index, item) {
                        $(this).datetimepicker({
                            dateFormat: 'yy-mm-dd',//01/01/0001
                            timeFormat: 'hh:mm TT',// 12:00/am
                            stepMinute: 5,
                            changeMonth: true,
                            changeYear: true
                        });
                    });
                    $('*[data-uses-time-picker="true"]').each(function (index, item) {
                        $(this).timepicker({
                            timeFormat: 'hh:mm TT',// 12:00/am
                            stepMinute: 5
                        });
                    });
                    $('#btnCancelSingleSending').on('click', function () {
                        $('#frmFieldValueInfo').clearForm();
                        $.fancybox.close(true);
                    });
                    $('#btnConfirmSingleSending').on('click', function () {
                        var Survey = '{'
                        + '"SurveyID":"' + $('#FieldGroupsInfo_FieldGroupGUID').val() + '",'
                        + '"TransactionID":"' + UI.generateGUID() +'",'
                            + '"Fields":';
                        var fields = '[';
                        $('#frmFieldValueInfo :input:not(:button,:submit)').each(function () {
                            fields += (fields != '[' ? ',' : '') + '{"FieldID":"' + $(this).attr('data-fieldID') + '", "Value":"' + $(this).val() + '"}'
                        });
                        fields += ']';
                        Survey += fields;
                        Survey += '}';
                        $('#frmFieldValueInfo').removeAttr('novalidate');
                        //ajax request to send form data
                        if ($('#frmFieldValueInfo').valid()) {
                            $.ajax({
                                url: '/Emails/SingleSending',
                                type: 'POST',
                                cache: false,
                                data: { model: Survey },
                                success: function (data) {
                                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                                    $('#btnCancelSingleSending').trigger('click');
                                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                                }
                            });
                        }
                    });
                }
            });
        });
    }

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchEmailsResults', tableColumns.length - 1);
        EMAIL.oEmailTable = $('#tblSearchEmailsResults').dataTable();
    }

    var makeEmailsTableRowsSelectable = function () {
        EMAIL.oEmailTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    EMAIL.oEmailTable.$('tr.selected-row').removeClass('selected-row primary');
                    $(this).addClass('selected-row primary');
                    $.ajax({
                        url: '/Emails/GetEmailInfo',
                        cache: false,
                        type: 'POST',
                        data: { EmailID: $(this).attr('id') },
                        success: function (data) {
                            $('#EmailInfo_EmailID').val(data.EmailInfo_EmailID);
                            $('#EmailInfo_Terminals').multiselect('uncheckAll');
                            $.each(data.EmailInfo_Terminals, function (index, item) {
                                $('#EmailInfo_Terminals option[value="' + item + '"]').attr('selected', true);
                            });
                            $('#EmailInfo_Terminals').multiselect('refresh');
                            $('#EmailInfo_Culture').val(data.EmailInfo_Culture).trigger('change', { template: data.EmailInfo_EmailTemplate });
                            $('#EmailInfo_Subject').val(data.EmailInfo_Subject);
                            $('#EmailInfo_Sender').val(data.EmailInfo_Sender);
                            $('#EmailInfo_Alias').val(data.EmailInfo_Alias);
                            $('#EmailInfo_Description').val(data.EmailInfo_Description);
                            $('#EmailInfo_Content').val(data.EmailInfo_Content);
                            UI.expandFieldset('fdsEmailsInfo');
                            UI.scrollTo('fdsEmailsInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteEmail, [$(this).attr('id')]);
            }
        });
    }

    var saveEmailSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;

        if (data.ResponseType > 0) {
            var _terminals = '';
            $.each($('#EmailInfo_Terminals').multiselect('getChecked'), function (index, item) {
                _terminals += (_terminals == '' ? '' : ', ') + $('#EmailInfo_Terminals option[value="' + $(this).val() + '"]').text();
            });
            if (data.ResponseMessage == 'Email Saved') {

                var oSettings = EMAIL.oEmailTable.fnSettings();
                var iAdded = EMAIL.oEmailTable.fnAddData([
                    data.ItemID,
                    $('#EmailInfo_EmailTemplate option:selected').text(),
                    _terminals,
                    $('#EmailInfo_Description').val(),
                    $('#EmailInfo_Culture option:selected').text(),
                    $('#EmailInfo_Subject').val(),
                    $('#EmailInfo_Sender').val(),
                    $('#EmailInfo_Alias').val(),
                    ''//'<img src="/Content/themes/base/images/trash.png" class="right" />'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID);
                EMAIL.oEmailTable.fnDisplayRow(aRow);
                EMAIL.makeEmailsTableRowsSelectable();
                $('#frmEmailsInfo').clearForm();
            }
            else {
                var array = EMAIL.oEmailTable.fnGetNodes();
                var nTr = EMAIL.oEmailTable.$('tr.selected-row');
                var position = EMAIL.oEmailTable.fnGetPosition(nTr[0]);
                EMAIL.oEmailTable.fnUpdate([
                    data.ItemID,
                    $('#EmailInfo_EmailTemplate option:selected').text(),
                    _terminals,
                    $('#EmailInfo_Description').val(),
                    $('#EmailInfo_Culture option:selected').text(),
                    $('#EmailInfo_Subject').val(),
                    $('#EmailInfo_Sender').val(),
                    $('#EmailInfo_Alias').val(),
                    ''//'<img src="/Content/themes/base/images/trash.png" class="right" />'
                ], nTr[0], undefined, false);
            }
            $.getJSON('/Emails/GetDDLData', { itemType: 'emails', itemID: '' }, function (data) {
                $('#EmailNotificationInfo_Email').fillSelect(data);
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var deleteEmail = function (emailID) {
        $.ajax({
            url: '/Emails/DeleteEmail',
            cache: false,
            type: 'POST',
            data: { targetID: emailID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {

                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var searchNotificationsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchEmailNotificationsResults', tableColumns.length - 1);
        EMAIL.oNotificationsTable = $('#tblSearchEmailNotificationsResults').dataTable();
    }

    var makeEmailNotificationsTableRowsSelectable = function () {
        EMAIL.oNotificationsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    EMAIL.oNotificationsTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/Emails/GetEmailNotificationInfo',
                        cache: false,
                        type: 'POST',
                        data: { EmailNotificationID: $(this).attr('id') },
                        success: function (data) {
                            $('#EmailNotificationInfo_EmailNotificationID').val(data.EmailNotificationInfo_EmailNotificationID);
                            $('#EmailNotificationInfo_EmailAccounts').val(data.EmailNotificationInfo_EmailAccounts);
                            $('#EmailNotificationInfo_AlternateAddresses').val(data.EmailNotificationInfo_AlternateAddresses);
                            $('#EmailNotificationInfo_Terminal option[value="' + data.EmailNotificationInfo_Terminal + '"]').attr('selected', true);
                            $('#EmailNotificationInfo_Terminal').trigger('change', { destinations: data.EmailNotificationInfo_Destinations, resorts: data.EmailNotificationInfo_Resorts, email: data.EmailNotificationInfo_Email, event: data.EmailNotificationInfo_Event });
                            if (data.EmailNotificationInfo_LeadSources != null) {
                                $.each(data.EmailNotificationInfo_LeadSources, function (index, item) {
                                    $('#EmailNotificationInfo_LeadSources option[value="' + item + '"]').attr('selected', true);
                                });
                            }
                            $('#EmailNotificationInfo_LeadSources').multiselect('refresh');
                            if (data.EmailNotificationInfo_PointsOfSale != null) {
                                $.each(data.EmailNotificationInfo_PointsOfSale, function (index, item) {
                                    $('#EmailNotificationInfo_PointsOfSale option[value="' + item + '"]').attr('selected', true);
                                });
                            }
                            $('#EmailNotificationInfo_PointsOfSale').multiselect('refresh');
                            if (data.EmailNotificationInfo_LeadStatus != null) {
                                $.each(data.EmailNotificationInfo_LeadStatus, function (index, item) {
                                    $('#EmailNotificationInfo_LeadStatus option[value="' + item + '"]').attr('selected', true);
                                });
                            }
                            $('#EmailNotificationInfo_LeadStatus').multiselect('refresh');
                            if (data.EmailNotificationInfo_BookingStatus != null) {
                                $.each(data.EmailNotificationInfo_BookingStatus, function (index, item) {
                                    $('#EmailNotificationInfo_BookingStatus option[value="' + item + '"]').attr('selected', true);
                                });
                            }
                            $('#EmailNotificationInfo_BookingStatus').multiselect('refresh');
                            if (data.EmailNotificationInfo_BrokerContracts != null) {
                                $.each(data.EmailNotificationInfo_BrokerContracts, function (index, item) {
                                    $('#EmailNotificationInfo_BrokerContracts option[value="' + item + '"]').attr('selected', true);
                                });
                            }
                            $('#EmailNotificationInfo_BrokerContracts').multiselect('refresh');
                            $('#EmailNotificationInfo_RequiredFields').val(data.EmailNotificationInfo_RequiredFields);
                            if (data.EmailNotificationInfo_CopyLead.toString().toLowerCase() == 'true') {
                                $('#EmailNotificationInfo_CopyLead').attr('checked', true);
                            }
                            else {
                                $('#EmailNotificationInfo_CopyLead').removeAttr('checked');
                            }
                            if (data.EmailNotificationInfo_CopySender.toString().toLowerCase() == 'true') {
                                $('#EmailNotificationInfo_CopySender').attr('checked', true);
                            }
                            else {
                                $('#EmailNotificationInfo_CopySender').removeAttr('checked');
                            }
                            if (data.EmailNotificationInfo_IsActive.toString().toLowerCase() == 'true') {
                                $('#EmailNotificationInfo_IsActive').attr('checked', true);
                            }
                            else {
                                $('#EmailNotificationInfo_IsActive').removeAttr('checked');
                            }
                            if (data.EmailNotificationInfo_RequiredFields != null) {
                                $.each(data.EmailNotificationInfo_RequiredFields, function (index, item) {
                                    $('#EmailNotificationInfo_RequiredFields option[value="' + item + '"]').attr('selected', true);
                                });
                            }
                            $('#EmailNotificationInfo_RequiredFields').multiselect('refresh');
                            $('#EmailNotificationInfo_Description').val(data.EmailNotificationInfo_Description);
                            UI.expandFieldset('fdsEmailNotificationsInfo');
                            UI.scrollTo('fdsEmailNotificationsInfo', null);
                        }
                    });
                }
            }
        });
    }

    var saveEmailNotificationSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;

        if (data.ResponseType > 0) {
            var _destinations = '';
            var _resorts = '';
            var _leadSources = '';
            var _pos = '';
            var _leadStatus = '';
            var _bookingStatus = '';
            var _brokerContracts = '';
            var _requiredFields = '';
            $('#EmailNotificationInfo_Destinations').multiselect('getChecked').each(function () {
                _destinations += (_destinations != '' ? ', ' : '') + $('#EmailNotificationInfo_Destinations option[value="' + $(this).val() + '"]').text();
            });
            $('#EmailNotificationInfo_Resorts').multiselect('getChecked').each(function () {
                _resorts += (_resorts != '' ? ', ' : '') + $('#EmailNotificationInfo_Resorts option[value="' + $(this).val() + '"]').text();
            });
            $('#EmailNotificationInfo_LeadSources').multiselect('getChecked').each(function () {
                _leadSources += (_leadSources != '' ? ', ' : '') + $('#EmailNotificationInfo_LeadSources option[value="' + $(this).val() + '"]').text();
            });
            $('#EmailNotificationInfo_PointsOfSale').multiselect('getChecked').each(function () {
                _pos += (_pos != '' ? ', ' : '') + $('#EmailNotificationInfo_PointsOfSale option[value="' + $(this).val() + '"]').text();
            });
            $('#EmailNotificationInfo_LeadStatus').multiselect('getChecked').each(function () {
                _leadStatus += (_leadStatus != '' ? ', ' : '') + $('#EmailNotificationInfo_LeadStatus option[value="' + $(this).val() + '"]').text();
            });
            $('#EmailNotificationInfo_BookingStatus').multiselect('getChecked').each(function () {
                _bookingStatus += (_bookingStatus != '' ? ', ' : '') + $('#EmailNotificationInfo_BookingStatus option[value="' + $(this).val() + '"]').text();
            });
            $('#EmailNotificationInfo_BrokerContracts').multiselect('getChecked').each(function () {
                _brokerContracts += (_brokerContracts != '' ? ', ' : '') + $('#EmailNotificationInfo_BrokerContracts option[value="' + $(this).val() + '"]').text();
            });
            $('#EmailNotificationInfo_RequiredFields').multiselect('getChecked').each(function () {
                _requiredFields += (_requiredFields != '' ? ', ' : '') + $('#EmailNotificationInfo_RequiredFields option[value="' + $(this).val() + '"]').text();
            });
            if (data.ResponseMessage == 'Email Notification Saved') {
                var oSettings = EMAIL.oNotificationsTable.fnSettings();
                var iAdded = EMAIL.oNotificationsTable.fnAddData([
                    data.ItemID,
                    $('#EmailNotificationInfo_Terminal option:selected').text(),
                    _pos,
                    $('#EmailNotificationInfo_Description').val(),
                    _resorts,
                    $('#EmailNotificationInfo_EmailAccounts').val(),
                    _leadSources,
                    _leadStatus,
                    _brokerContracts,
                    _bookingStatus,
                    ''
                    //_requiredFields
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID);
                EMAIL.oNotificationsTable.fnDisplayRow(aRow);
                EMAIL.makeEmailNotificationsTableRowsSelectable();
                $('#frmEmailNotificationsInfo').clearForm();
            }
            else {
                var array = EMAIL.oNotificationsTable.fnGetNodes();
                var nTr = EMAIL.oNotificationsTable.$('tr.selected-row');
                var position = EMAIL.oNotificationsTable.fnGetPosition(nTr[0]);
                EMAIL.oNotificationsTable.fnUpdate([
                    data.ItemID,
                    $('#EmailNotificationInfo_Terminal option:selected').text(),
                    _pos,
                    $('#EmailNotificationInfo_Description').val(),
                    _resorts,
                    $('#EmailNotificationInfo_EmailAccounts').val(),
                    _leadSources,
                    _leadStatus,
                    _brokerContracts,
                    _bookingStatus,
                    ''
                    //_requiredFields
                ], nTr[0], undefined, false);
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var searchFieldGroupsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchFieldGroupsResults', tableColumns.length - 1);
        EMAIL.oFieldGroupsTable = $('#tblSearchFieldGroupsResults').dataTable();
    }

    var makeFieldGroupsTableRowsSelectable = function () {
        EMAIL.oFieldGroupsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    var _id = $(this).attr('id');
                    EMAIL.oFieldGroupsTable.$('tr.selected-row').removeClass('selected-row primary');
                    $(this).addClass('selected-row primary');
                    $.ajax({
                        url: '/Emails/GetFieldGroupInfo',
                        cache: false,
                        type: 'POST',
                        data: { FieldGroupID: _id },
                        success: function (data) {
                            $('#FieldsInfo_FieldGroup').val(data.FieldGroupsInfo_FieldGroupID);
                            $('#FieldGroupsInfo_FieldGroupID').val(data.FieldGroupsInfo_FieldGroupID);
                            $('#FieldGroupsInfo_FieldGroupGUID').val(data.FieldGroupsInfo_FieldGroupGUID);
                            $('#FieldGroupsInfo_FieldGroup').val(data.FieldGroupsInfo_FieldGroup);
                            $('#FieldGroupsInfo_Description').val(data.FieldGroupsInfo_Description);
                            $('#FieldGroupsInfo_EmailNotification').val(data.FieldGroupsInfo_EmailNotification);
                            $('#FieldGroupsInfo_Logo').val(data.FieldGroupsInfo_Logo);
                            $.getJSON('/Emails/GetDDLData', { itemType: 'fieldsPerGroup', itemID: _id }, function (data) {
                                $('#FieldsInfo_ParentField').clearSelect();
                                $('#FieldsInfo_ParentField').fillSelect(data);
                            });
                            $.getJSON('/Emails/GetDDLData', { itemType: 'fieldGuidsPerGroup', itemID: _id }, function (data) {
                                $('#FieldsInfo_VisibleIfGuid').clearSelect();
                                $('#FieldsInfo_VisibleIfGuid').fillSelect(data);
                            });
                            $.ajax({
                                url: '/Emails/GetFieldsPerGroup',
                                cache: false,
                                contentType: 'application/json; charset=utf-8',
                                type: 'POST',
                                data: JSON.stringify(data.FieldsInGroup),
                                success: function (data) {
                                    $('#divTblExistingFields').html(data);
                                    EMAIL.searchFieldsResultsTable('tblSearchFieldsResults');
                                },
                                complete: function () {
                                    EMAIL.makeFieldsTableRowsSelectable();
                                    UI.expandFieldset('fdsFieldsPerGroup');
                                }
                            });
                            UI.expandFieldset('fdsFieldGroupsInfo');
                            UI.scrollTo('fdsFieldGroupsInfo', null);
                        }
                    });
                }
            }
        });
    }

    var saveFieldGroupSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;

        if (data.ResponseType > 0) {
            if (data.ResponseMessage.indexOf('Saved') != -1) {
                var oSettings = EMAIL.oFieldGroupsTable.fnSettings();
                var iAdded = EMAIL.oFieldGroupsTable.fnAddData([
                    data.ItemID.fieldGroupID,
                    $('#FieldGroupsInfo_FieldGroup').val(),
                    $('#FieldGroupsInfo_Description').val(),
                    data.ItemID.savedByUser,
                    data.ItemID.dateSaved
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID.fieldGroupID);
                EMAIL.oFieldGroupsTable.fnDisplayRow(aRow);
                EMAIL.makeFieldGroupsTableRowsSelectable();
                $('#frmFieldGroupsInfo').clearForm();
            }
            else {
                var array = EMAIL.oFieldGroupsTable.gnGetNodes();
                var nTr = EMAIL.oFieldGroupsTable.$('tr.selected-row');
                var position = EMAIL.oFieldGroupsTable.fnGetPosition(nTr[0]);
                EMAIL.oFieldGroupsTable.fnUpdate([
                    data.ItemID.fieldGroupID,
                    $('#FieldGroupsInfo_FieldGroup').val(),
                    $('#FieldGroupsInfo_Description').val(),
                    data.ItemID.savedByUser,
                    data.ItemID.dateSaved
                ], nTr[0], undefined, false);
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveFieldSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;

        if (data.ResponseType > 0) {
            if (data.ResponseMessage.indexOf('Saved') != -1) {
                var oSettings = EMAIL.oFieldsTable.fnSettings();
                var iAdded = EMAIL.oFieldsTable.fnAddData([
                    data.ItemID.fieldID,
                    $('#FieldsInfo_Field').val(),
                    ($('#FieldsInfo_ParentField option:selected').val() != '0' ? $('#FieldsInfo_ParentField option:selected').text() : ''),
                    ($('#FieldsInfo_FieldType option:selected').val() != '0' ? $('#FieldsInfo_FieldType option:selected').text() : ''),
                    ($('#FieldsInfo_FieldSubType option:selected').val() != '0' ? $('#FieldsInfo_FieldSubType option:selected').text() : ''),
                    $('#FieldsInfo_Description').val(),
                    $('#FieldsInfo_Options').val(),
                    $('#FieldsInfo_Order').val(),
                    '<img src="/content/themes/base/images/trash.png" class="delete-item" />'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID.fieldID);
                EMAIL.oFieldsTable.fnDisplayRow(aRow);
                EMAIL.makeFieldsTableRowsSelectable();
                $('#frmFieldInfo').clearForm();
            }
            else {
                var array = EMAIL.oFieldsTable.fnGetNodes();
                var nTr = EMAIL.oFieldsTable.$('tr.selected-row');
                var position = EMAIL.oFieldsTable.fnGetPosition(nTr[0]);
                EMAIL.oFieldsTable.fnUpdate([
                    data.ItemID.fieldID,
                    $('#FieldsInfo_Field').val(),
                    ($('#FieldsInfo_ParentField option:selected').val() != '0' ? $('#FieldsInfo_ParentField option:selected').text() : ''),
                    ($('#FieldsInfo_FieldType option:selected').val() != '0' ? $('#FieldsInfo_FieldType option:selected').text() : ''),
                    ($('#FieldsInfo_FieldSubType option:selected').val() != '0' ? $('#FieldsInfo_FieldSubType option:selected').text() : ''),
                    $('#FieldsInfo_Description').val(),
                    $('#FieldsInfo_Options').val(),
                    $('#FieldsInfo_Order').val(),
                    '<img src="/content/themes/base/images/trash.png" class="delete-item" />'
                ], nTr[0], undefined, false);
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var searchFieldsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchFieldsResults', tableColumns.length - 1);
        EMAIL.oFieldsTable = $('#tblSearchFieldsResults').dataTable();
    }

    var makeFieldsTableRowsSelectable = function () {
        EMAIL.oFieldsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    EMAIL.oFieldsTable.$('tr.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/Emails/GetFieldInfo',
                        cache: false,
                        type: 'POST',
                        data: { FieldID: $(this).attr('id') },
                        success: function (data) {
                            $('#FieldsInfo_FieldID').val(data.FieldsInfo_FieldID);
                            $('#FieldsInfo_Field').val(data.FieldsInfo_Field);
                            $('#FieldsInfo_Description').val(data.FieldsInfo_Description);
                            $('#FieldsInfo_FieldType option[value="' + data.FieldsInfo_FieldType + '"]').attr('selected', true);
                            $('#FieldsInfo_FieldType').trigger('change', { subType: data.FieldsInfo_FieldSubType });
                            $('#FieldsInfo_ParentField option[value="' + data.FieldsInfo_ParentField + '"]').attr('selected', true);
                            $('#FieldsInfo_Visibility').val(data.FieldsInfo_Visibility);
                            if (data.FieldsInfo_VisibleIf != null) {
                                if (data.FieldsInfo_VisibleIf == true) {
                                    $('input:radio[name=FieldsInfo_VisibleIf]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name=FieldsInfo_VisibleIf]')[1].checked = true;
                                }
                            }
                            $('#FieldsInfo_VisibleIfGuid option[value="' + data.FieldsInfo_VisibleIfGuid + '"]').attr('selected', true);
                            $('#FieldsInfo_Options').val(data.FieldsInfo_Options);
                            $('#FieldsInfo_Order').val(data.FieldsInfo_Order);
                            UI.expandFieldset('fdsFieldInfo');
                            UI.scrollTo('fdsFieldInfo', null);
                        }
                    });
                }
            }
        });
    }

    return {
        init: init,
        searchResultsTable: searchResultsTable,
        makeEmailsTableRowsSelectable: makeEmailsTableRowsSelectable,
        saveEmailSuccess: saveEmailSuccess,
        deleteEmail: deleteEmail,
        searchNotificationsResultsTable: searchNotificationsResultsTable,
        makeEmailNotificationsTableRowsSelectable: makeEmailNotificationsTableRowsSelectable,
        saveEmailNotificationSuccess: saveEmailNotificationSuccess,
        searchFieldGroupsResultsTable: searchFieldGroupsResultsTable,
        makeFieldGroupsTableRowsSelectable: makeFieldGroupsTableRowsSelectable,
        saveFieldGroupSuccess: saveFieldGroupSuccess,
        saveFieldSuccess: saveFieldSuccess,
        searchFieldsResultsTable: searchFieldsResultsTable,
        makeFieldsTableRowsSelectable: makeFieldsTableRowsSelectable
    }
}();