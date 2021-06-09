$(function () {
    TASKSCHEDULER.init();
});

var TASKSCHEDULER = function () {

    var oTaskSchedukerTable;

    var init = function ()
    {
        COMMON.getServerDateTime();        
        UI.updateListsOnTerminalsChange();
        $('select[multiple="multiple"]').multiselect({
            noneSelectedText: "--All--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();
    }

    $(".DateFormat").datetimepicker({
        dateFormat: 'yy-mm-dd',//01/01/0001
        timeFormat: 'hh:mm TT',// 12:00/am
        stepMinute: 2,
        changeMonth: true,
        changeYear: true
    }).val("");

    $('#SearchFromDate').removeClass("input-validation-error");
    $('#SearchToDate').removeClass("input-validation-error");


    $('#btnNewTask').on('click', function ()
    {
        $('#TaskSchedulerID').val('');
        $('#Description').val("");
        $('#Url').val("");
        $('#FromDate').val("");
        $('#ToDate').val("");
        $('#Permanent').removeAttr('checked');     
        $('#DatePermanent').slideDown('fast');
        $('#Monday').removeAttr('checked');
        $('#Tuesday').removeAttr('checked');
        $('#WednesDay').removeAttr('checked');
        $('#Thursday').removeAttr('checked');
        $('#Friday').removeAttr('checked');
        $('#Saturday').removeAttr('checked');
        $('#Sunday').removeAttr('checked');
        $('#January').removeAttr('checked');
        $('#February').removeAttr('checked');
        $('#March').removeAttr('checked');
        $('#April').removeAttr('checked');
        $('#May').removeAttr('checked');
        $('#June').removeAttr('checked');
        $('#July').removeAttr('checked');
        $('#August').removeAttr('checked');
        $('#September').removeAttr('checked');
        $('#October').removeAttr('checked');
        $('#November').removeAttr('checked');
        $('#December').removeAttr('checked');
        $('#days').slideUp ('fast');
        $('#Months').slideUp('fast');
        $('#Lapse').val('0');
        $('#minutes').val('');
        //$('#NumberDays').clearSelect();
        //$('#NumberDays').multiselect('refresh');
        $('select[multiple="multiple"]').multiselect('refresh');
        UI.tablesHoverEffect();
        UI.expandFieldset('fdsTaskSchedulerManagement');
        UI.scrollTo('fdsTaskSchedulerManagement', null);
    });

    $('#Permanent').on('click', function () {
        var check = $('#Permanent');
        if (check.is(':checked')) {
            $('#DatePermanent').slideUp('fast');
        }
        else {
            $('#DatePermanent').slideDown('fast');
        }
    });
  
    $('#SearchActive').prop('checked', true);
    $('#SearchPermanent').prop('checked', true);
    $('#Lapse').on('change', function ()
    {
        var value = $(this).val();
        switch(value)
        {
            case "1"://dialy
                {
                    $('#DatePermanent').slideUp('fast');
                    $('#Permanent').prop('checked', true);
                    $('#days').slideUp('fast');
                    $('#Months').slideUp('fast');
                    $('#oneTime').slideDown('fast');
                    $('#NumberDays').multiselect('refresh');
                    break;
                }
            case "2"://some days week
                {
                    $('#DatePermanent').slideDown('fast');
                    $('#Permanent').prop('checked', false);
                    $('#Months').slideUp('fast');
                    $('#days').slideDown('fast');
                    $('#oneTime').slideDown('fast');
                    $('#NumberDays').multiselect('refresh');
                    break;
                }
            case "3"://some days month
                {
                    $('#Permanent').prop('checked', false);
                    $('#days').slideUp('fast');
                    $('#oneTime').slideDown('fast');
                    $('#Months').slideDown('fast');
                    $('#NumberDays').multiselect('refresh');
                    break;
                }
            case "4"://one time
                {
                    $('#DatePermanent').slideDown('fast');
                    $('#DatePermanent').slideUp('fast');
                    $('#Permanent').prop('checked', false);
                    $('#oneTime').slideUp('fast');
                    $('#days').slideUp('fast');
                    $('#Months').slideUp('fast');          
                    $('#NumberDays').multiselect('refresh');
                    break;
                }
            case "5"://many times at day
                {
                    $('#DatePermanent').slideUp('fast');
                    $('#Permanent').prop('checked', true);
                    $('#days').slideUp('fast');
                    $('#Months').slideUp('fast');
                    $('#oneTime').slideDown('fast');
                    $('#NumberDays').multiselect('refresh');
                    $('#MinutesAtDay').slideDown('fast');
                    break;
                }
        }
    });

    //functions
   
    function deleteTask(TaskSchedulerID)
    {
        $.ajax
            ({
                url: '/TaskScheduler/DeleteTask',
                cache: false,
                type: 'POST',
                data: { TargetID: TaskSchedulerID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ?
                        data.ResponseType : null;
                    TASKSCHEDULER.oTaskSchedukerTable
                           .fnDeleteRow($('#' + TaskSchedulerID)[0]);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
    }

    var TaskResultTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblTaskScheduler', tableColumns - 1);
        TASKSCHEDULER.oTaskSchedukerTable = $('#tblTaskScheduler').dataTable();
    }

    var makeTaskSelectable = function () {
        TASKSCHEDULER.oTaskSchedukerTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img'))
            {
                if (!$(this).hasClass('selected-row'))
                {                   
                    UI.unselectPrimaryByEsc(TASKSCHEDULER.oTaskSchedukerTable, $(this));
                    var TaskSchedulerID = $(this).attr('id')
                    $.ajax(
                        {
                            url: '/TaskScheduler/GetTask',
                            cache: false,
                            type: 'POST',
                            data: { targetID: TaskSchedulerID},
                            success: function (data)
                            {                                
                                //campos
                                $('#TaskSchedulerID').val(data.GetTaskSchedulerID);
                                $('#Description').val(data.GetDescription);
                                $('#FromDate').val(data.GetFromDate);
                                $('#ToDate').val(data.GetToDate);
                                $('#Url').val(data.GetUrl);                               
                                $('#Lapse option[value="' + data.GetRecur + '"]').attr('selected', true).trigger('change');
                                switch(data.GetRecur)
                                {
                                    case "2"://semanal
                                        {
                                            $('input:checkbox[name="Monday"]')[0].checked = data.GetMonday;
                                            $('input:checkbox[name="Tuesday"]')[0].checked = data.GetTuesday;
                                            $('input:checkbox[name="WednesDay"]')[0].checked = data.GetWednesday;
                                            $('input:checkbox[name="Thursday"]')[0].checked = data.GetThursday;
                                            $('input:checkbox[name="Friday"]')[0].checked = data.GetFriday;
                                            $('input:checkbox[name="Saturday"]')[0].checked = data.GetSaturday;
                                            $('input:checkbox[name="Sunday"]')[0].checked = data.GetSunday;                                           
                                            break;
                                        }
                                    case "3"://month
                                        {
                                            $('input:checkbox[name="January"]')[0].checked = data.GetJanuary;
                                            $('input:checkbox[name="February"]')[0].checked = data.GetFebruary;
                                            $('input:checkbox[name="March"]')[0].checked = data.GetMarch;
                                            $('input:checkbox[name="April"]')[0].checked = data.GetApril;
                                            $('input:checkbox[name="May"]')[0].checked = data.GetMay;
                                            $('input:checkbox[name="June"]')[0].checked = data.GetJune;
                                            $('input:checkbox[name="July"]')[0].checked = data.GetJuly;
                                            $('input:checkbox[name="August"]')[0].checked = data.GetAugust;
                                            $('input:checkbox[name="September"]')[0].checked = data.GetSeptember;
                                            $('input:checkbox[name="October"]')[0].checked = data.GetOctober;
                                            $('input:checkbox[name="November"]')[0].checked = data.GetNovember;
                                            $('input:checkbox[name="December"]')[0].checked = data.GetDecember;
                                            var days = data.GetNumberDays.split(',');
                                                $.each(days, function (index, day) {
                                                    $('#NumberDays').find('option[value="' + day + '"]').attr('selected', true);
                                                });
                                             $('select[multiple="multiple"]').multiselect('refresh')
                                        }
                                }
                                $('input:checkbox[name="Permanent"]')[0].checked = data.GetPermanent;

                                if (data.GetPermanent == true) {
                                    $('#DatePermanent').slideUp('fast');
                                }
                                else {
                                    $('#DatePermanent').slideDown('fast');
                                }

                                UI.expandFieldset('fdsTaskSchedulerManagement');
                                UI.scrollTo('fdsTaskSchedulerManagement', null);                                
                            }
                        });
                 }
            }
            else
            {
                UI.confirmBox('Do you confirm you want to proceed?', deleteTask, [$(this).attr('id')]);
            }
        });
    }

    var saveTaskScheduler = function (data)
    {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            TASKSCHEDULER.oTaskSchedukerTable = $.fn.DataTable.fnIsDataTable('tblTaskScheduler') ? $('#tblTaskScheduler') : $('#tblTaskScheduler').dataTable();
            var permanent = $('#Permanent').is(':checked');
            var active = true;

            if (data.ResponseMessage == 'Task Save Success') {              
                var oSettings = TASKSCHEDULER.oTaskSchedukerTable.fnSettings();
                var iAdded = TASKSCHEDULER.oTaskSchedukerTable.fnAddData([
                    $('#Description').val(),
                    $('#Url').val(),
                    $('#Lapse option[value=' + $('#Lapse').val(/*data.UserInfo_Company*/) + ']').text(),
                    $('#FromDate').val(),
                    $('#ToDate').val(),                                    
                    permanent == true ? '<input type="checkbox" checked disabled/>' : '<input type="checkbox" disabled/>',
                    active == true ? '<input type="checkbox" checked disabled/>' : '<input type="checkbox" disabled/>',
                    '',//$('#SaveByUser').val(''),
                    ''//$('#DateSaved').val('')                    
                ]);
                $('#btnNewTask').click();
                UI.tablesHoverEffect();
                TASKSCHEDULER.makeTaskSelectable();
            }
            else {

                var prueba = $('#Lapse option:selected').text();

                $('#' + data.ItemID).children('tr').text($('#TaskSchedulerID').val());
                $('#' + data.ItemID).children('td:nth-child(1)').text($('#Description').val());
                $('#' + data.ItemID).children('td:nth-child(2)').text($('#Url').val());               
                $('#' + data.ItemID).children('td:nth-child(3)').text($('#Lapse option:selected').text());
                $('#' + data.ItemID).children('td:nth-child(4)').text($('#FromDate').val());
                $('#' + data.ItemID).children('td:nth-child(5)').text($('#ToDate').val());                
                $('#' + data.ItemID).children('td:nth-child(6)').html( permanent == true ? '<input type="checkbox" checked disabled/>' : '<input type="checkbox" disabled/>');
                $('#' + data.ItemID).children('td:nth-child(7)').html( active == true ? '<input type="checkbox" checked disabled/>' : '<input type="checkbox" disabled/>');
                $('#' + data.ItemID).children('td:nth-child(8)').text($('#SaveByUser').val());
                $('#' + data.ItemID).children('td:nth-child(9)').text($('#DateSaved').val());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    return {
        init: init,
        TaskResultTable:TaskResultTable,
        makeTaskSelectable: makeTaskSelectable,
        saveTaskScheduler: saveTaskScheduler
    }
}();