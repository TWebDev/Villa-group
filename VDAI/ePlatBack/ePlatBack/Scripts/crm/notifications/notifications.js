$(function () {
    NOTIFICATION.init();
});

var NOTIFICATION = new function () {
    var oFormsTable;
    var oNotificationsTable;
    var Titles;

    var init = function () {

        //$('#divNotificationsTab').tabs();

        //$('#RequestDate').on('keydown', function (e) {
        //    e.preventDefault();
        //});

        //$('#RequestDate').datepicker({
        //    dateFormat: 'yy-mm-dd',
        //    minDate: -22,
        //    maxDate: 0,
        //    onClose: function (dateText, inst) {
        //        if (dateText != '') {
        //            localStorage.Eplat_VLO_ManifestRequest = new Date($('#RequestDate').datepicker('getDate'));
        //            NOTIFICATION.getVLOManifest(dateText);
        //        }
        //    }
        //});



        NOTIFICATION.searchFormsResultsTable();

        $('#FormSearch_Date').datepicker({
            dateFormat: 'yy-mm-dd',
            onClose: function (dateText, inst) {
                NOTIFICATION.getNotificationHistory($('#FormSearch_FormID').val(), dateText);
            }
        });

        $('.change-date').on('click', function () {
            var _date = new Date($('#FormSearch_Date').datepicker('getDate'));
            if ($(this).hasClass('previous')) {
                $('#FormSearch_Date').datepicker('setDate', UTILS.addDays(_date, -1));
            }
            else if ($(this).hasClass('forward')) {
                $('#FormSearch_Date').datepicker('setDate', UTILS.addDays(_date, 1));
            }
            $('#FormSearch_Date').datepicker('refresh');
            NOTIFICATION.getNotificationHistory($('#FormSearch_FormID').val(), $('#FormSearch_Date').val());
        });

        $('.img-back').on('click', function () {
            $('.notification-history').toggle();
        });


        //$('#RequestDate').datepicker('setDate', new Date());
        //$('#RequestDate').datepicker('refresh');

        //if (localStorage.Eplat_VLO_ManifestRequest != undefined && localStorage.Eplat_VLO_ManifestRequest != null && localStorage.Eplat_VLO_ManifestRequest != '') {
        //    $('#lastRequestDate').text(new Date(localStorage.Eplat_VLO_ManifestRequest).toDateString());
        //    if (new Date(localStorage.Eplat_VLO_ManifestRequest).toDateString() != new Date().toDateString()) {
        //        $('#RequestDate').addClass('mb-warning');
        //    }
        //    else {
        //        $('#RequestDate').removeClass('mb-warning');
        //    }
        //}
        //else {
        //    if (new Date(localStorage.Eplat_VLO_ManifestUpdate).toDateString() != new Date().toDateString()) {
        //        $('#RequestDate').addClass('mb-warning');
        //    }
        //    else {
        //        $('#RequestDate').removeClass('mb-warning');
        //    }
        //}
        //if (localStorage.Eplat_VLO_Manifest == undefined || localStorage.Eplat_VLO_Manifest == null || localStorage.Eplat_VLO_Manifest == '') {
        //    localStorage.Eplat_VLO_ManifestRequest = new Date($('#RequestDate').datepicker('getDate'));
        //    NOTIFICATION.getVLOManifest($('#RequestDate').val());
        //}
        //else {
        //    NOTIFICATION.renderVLOManifest();
        //    $('#lastUpdate').text(localStorage.Eplat_VLO_ManifestUpdate);
        //}

        //$('#btnGetManifest').on('click', function () {
        //    localStorage.Eplat_VLO_ManifestRequest = new Date($('#RequestDate').datepicker('getDate'));
        //    NOTIFICATION.getVLOManifest($('#RequestDate').val());
        //});

        
    }

    var searchFormsResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchFormsResults', tableColumns.length - 1);
        NOTIFICATION.oFormsTable = $('#tblSearchFormsResults').dataTable();
    }

    var searchNotificationsHistoryResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblNotificationsHistoryResults', tableColumns.length - 1);
        NOTIFICATION.oNotificationsTable = $('#tblNotificationsHistoryResults').dataTable();
    }

    var makeFormsTableRowsSelectable = function () {
        NOTIFICATION.oFormsTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            var _this = $(this);
            if (!$(e.target).hasClass('send-notification')) {
                //row
                $('#FormSearch_FormID').val($(this).attr('id'));
                $('#FormSearch_Date').datepicker('setDate', new Date());
                $('#FormSearch_Date').datepicker('refresh');
                $('#tblNotificationsHistoryResults tbody').html('');
                NOTIFICATION.getNotificationHistory($(this).attr('id'), $('#FormSearch_Date').val());
                $('.notification-history').toggle();
            }
            else {
                //button
                $.fancybox({
                    width: '500',
                    overlay: {
                        closeClick: true
                    },
                    type: 'ajax',
                    href: '/Notifications/RenderSingleSending',
                    ajax: {
                        type: 'POST',
                        data: { fieldGroupID: $(this).attr('id') }
                    },
                    //href: $('#divFastSaleContainer'),
                    //modal: true,
                    afterShow: function () {
                        $('*[data-uses-date-picker="true"]').each(function (index, item) {
                            $(this).datepicker({
                                dateFormat: 'yy-mm-dd',
                                changeMonth: true,
                                changeYear: true,
                                minDate: 0
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
                            + '"SurveyID":"' + _this.attr('data-formguid') + '",'
                            + '"TransactionID":"' + UI.generateGUID() + '",'
                                + '"Fields":';
                            var fields = '[';
                            $('#frmFieldValueInfo :input:not(:button,:submit,:hidden)').each(function () {
                                fields += (fields != '[' ? ',' : '') + '{"FieldID":"' + $(this).attr('data-fieldID') + '", "Value":"' + $(this).val() + '"}'
                            });
                            fields += ']';
                            Survey += fields;
                            Survey += '}';
                            $('#frmFieldValueInfo').removeAttr('novalidate');

                            if ($('#frmFieldValueInfo').valid()) {
                                $('#FieldValue_FieldValues').val(Survey);
                                $('#frmFieldValueInfo').submit();
                                $.fancybox.close(true);
                            }
                        });
                    }
                });
            }
        });
    }

    var saveFieldValueSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        $('#btnCancelSingleSending').trigger('click');
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var getNotificationHistory = function (formID, date) {
        $.ajax({
            url: '/Notifications/GetNotificationHistory',
            cache: false,
            type: 'POST',
            dataType: 'html',
            data: { FormID: formID, Date: date },
            success: function (data) {
                $('#divNotificationsHistory').html(data);
                NOTIFICATION.deleteNotificationValues();
            }
        });
    }

    var deleteNotificationValues = function () {
        $('.delete-notification').unbind('click').on('click', function () {
            var id = $(this).parents('tr:first').attr('id');
            UI.confirmBox('Do you confirm you want to proceed?', function () {
                $.ajax({
                    url: '/Notifications/DeleteNotificationValues',
                    cache: false,
                    type: 'POST',
                    data: { transactionID: id },
                    success: function (data) {
                        var duration = data.ResponseType < 0 ? data.ResponseType : null;
                        if (data.ResponseType > 0) {
                            $('#' + data.ItemID).remove();
                        }
                        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                    }
                });
            }, [id]);
        });
    }

    var getVLOManifest = function (date) {
        ///modificar las funciones de agencia para mandar como parámetro qué manifiesto se solicita y asi poder usar la misma parcial

        //$.ajax({
        //    url: '/crm/SPI/GetManifestForVLO',
        //    cache: false,
        //    type: 'POST',
        //    data: { date: date },
        //    success: function (data) {
        //        localStorage.Eplat_VLO_Manifest = data;
        //        NOTIFICATION.renderVLOManifest();
        //        var _date = new Date();
        //        localStorage.Eplat_VLO_ManifestUpdate = _date.toDateString() + ' ' + _date.toLocaleTimeString();
        //        $('#lastUpdate').text(localStorage.Eplat_VLO_ManifestUpdate);
        //        $('#lastRequestDate').text(new Date(localStorage.Eplat_VLO_ManifestRequest).toDateString());
        //        if (new Date(localStorage.Eplat_VLO_MAnifestRequest).toDateString() != _date.toDateString()) {
        //            $('#RequestDate').addClass('mb-warning');
        //        }
        //        else {
        //            $('#RequestDate').removeClass('mb-warning');
        //        }
        //    }
        //});
    }

    var renderVLOManifest = function () {
        //$.ajax({
        //    async: false,
        //    url: '/Notifications/GetPersonalTitles',
        //    success: function (data) {
        //        var titles = new Array();
        //        $.each(data, function (index, item) {
        //            titles.push(item.Text);
        //        });
        //        NOTIFICATION.Titles = titles;
        //    }
        //});

        //var data = new Array();
        //var json = $.parseJSON(localStorage.Eplat_VLO_Manifest);
        //var tbody = '';
        //$.each(json, function (index, item) {
        //    var cached = item.SendStatus != '' ? 'cached' : '';
        //    tbody += '<tr>'
        //    + '<td ><form class="manifest-row">'
        //    + '<div class="editor-field right"><textarea name="PD" class="pd" placeholder="PD"></textarea></div>';
        //    if (item.Title != null && item.Title != '') {
        //        tbody += '<input type="text" name="Title" class="title editor-field" value="' + item.Title + '" placeholder="Title">';
        //    }
        //    else {
        //        tbody += '<select name="Title" class="title" style="width:91px;">';
        //        $.each(NOTIFICATION.Titles, function (i, it) {
        //            tbody += '<option value="'+ it +'">'+ it +'</option>';
        //        });
        //            tbody +='</select>';
        //    }
        //    tbody += '<input type="text" name="FirstName" class="first-name no-editable editor-field" value="' + item.FirstName + '" placeholder="First Name">'
        //    + '<input type="text" name="LastName" class="last-name no-editable editor-field" value="' + item.LastName + '" placeholder="Last Name">'
        //    + '<input type="text" name="VPANumber" class="vpa-number no-editable editor-field" value="' + item.VPANumber + '" placeholder="VPA Number">'
        //    + '<input type="text" name="CollectDate" class="collect-date editor-field" value="' + (item.CollectDateString != null ? item.CollectDateString : '') + '" placeholder="Collect Date" style="width:91px;">'
        //    + '<input type="text" name="ActivationDate" class="activation-date editor-field" value="' + (item.ActivationDateString != null ? item.ActivationDateString : '') + '" placeholder="Activation Date" style="width:91px;">'
        //    + '<input type="text" name="VLO" class="vlo editor-field" value="' + item.VLO + '" placeholder="VLO">'
        //    + '<input type="text" name="Email" value="' + item.Email + '" placeholder="Email" class="email ' + (item.Email.indexOf('*') != -1 ? 'no-editable' : '') + ' editor-field">'
        //    + '<input type="text" name="EmailString" class="email-string fakeDisplayNone" value="' + item.EmailString + '">'
        //    + '<input type="text" name="Culture" class="culture fakeDisplayNone" value="' + item.Culture + '">'
        //    + '<input type="text" name="ContractStatus" class="contract-status fakeDisplayNone" value="' + item.ContractStatus + '">'
        //    + '<input type="text" name="CustomerID" class="customer-id fakeDisplayNone" value="' + item.CustomerID + '">'
        //    + '<input type="text" name="TourID" class="tour-id fakeDisplayNone" value="' + item.TourID + '">'
        //    + '</form></td>'
        //    + '<td>'+ item.SendStatus +'</td>'
        //    + '<td class="coupon send-status ' + cached + '"></td>'
        //    + '</tr>';
        //});

        //$('#tblVLOManifest tbody').html(tbody);

        //$('.no-editable').on('keydown', function (e) {
        //    e.preventDefault();
        //});

        //$('.email:not(.no-editable)').on('keyup', function (e) {
        //    var value = $(this).val();
        //    $(this).next('.email-string').attr('value', value);
        //});

        //$('.collect-date, .activation-date').on('keydown', function (e) {
        //    e.preventDefault();
        //});

        //$('.collect-date, .activation-date').datepicker({
        //    dateFormat: 'yy-mm-dd',
        //    minDate: 0
        //});

        //NOTIFICATION.bindSendAllEmails();
    }

    function convertToJson(form) {
        //var formArray = $(form).serializeArray();
        //var returnArray = {};
        //for (var i = 0; i < formArray.length; i++) {
        //    returnArray[formArray[i]['name']] = formArray[i]['value'];
        //}
        //return returnArray;
    }

    var bindSendAllEmails = function () {
        //$('.send-all').unbind('click').on('click', function () {
        //    if ($('.send-status:not(.cached)').length > 0) {
        //        $('.send-status:not(.cached)').addClass('processing').removeClass('error');
        //        $('.send-status:not(.cached)').slowEach(2000, function (index, item) {
        //            NOTIFICATION.sendLetter(item, $(this).parents('tr:first').index());
        //        });
        //    }
        //    else {
        //        UI.messageBox(0, 'No emails pending to send', null, null);
        //    }
        //});
    }

    var sendLetter = function (td, index) {
        //var _id = $(td).parent('tr').attr('id') == undefined ? ('tr' + index) : $(td).parent('tr').attr('id');
        //$(td).parent('tr').attr('id', _id);
        //var $td = $(td);
        //var jsonForm = convertToJson($td.parent('tr').find('form.manifest-row'));
        //$.ajax({
        //    cache: false,
        //    url: '/Notifications/SendLetter',
        //    type: 'POST',
        //    data: { data: JSON.stringify(jsonForm) },
        //    beforeSend: function (jqXHR) {
        //        $td.addClass('pending');
        //        UI.scrollTo(_id, null);
        //    },
        //    success: function (data) {
        //        $td.prev('td').text(data.ResponseMessage);
        //        if (data.ResponseType == 1) {
        //            $td.removeClass('pending processing').addClass('cached');
        //        }
        //        else {
        //            $td.removeClass('pending processing').addClass('error');
        //            $td.html('<i class="material-icons re-send" title="resend email">redo</i>');
        //        }
        //    }
        //}).always(function () {
        //    NOTIFICATION.bindResendEmail();
        //});
    }

    var bindResendEmail = function () {
        //$('.re-send').unbind('click').on('click', function () {
        //    NOTIFICATION.sendLetter($(this).parent('td'), $(this).parents('tr:first').index());
        //    $(this).remove();
        //});
    }

    return {
        init: init,
        searchFormsResultsTable: searchFormsResultsTable,
        searchNotificationsHistoryResultsTable: searchNotificationsHistoryResultsTable,
        makeFormsTableRowsSelectable: makeFormsTableRowsSelectable,
        saveFieldValueSuccess: saveFieldValueSuccess,
        getNotificationHistory: getNotificationHistory,
        deleteNotificationValues: deleteNotificationValues,
        getVLOManifest: getVLOManifest,
        renderVLOManifest: renderVLOManifest,
        bindSendAllEmails: bindSendAllEmails,
        sendLetter: sendLetter,
        bindResendEmail: bindResendEmail,
        Titles: Titles
    }
}();


