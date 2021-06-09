$(function () {
    UR.init();
});

var UR = function () {

    var oTable;

    var init = function () {

        $('#UserInfo_UserName').on('keypress', function (e) {
            e.preventDefault();
        });

        //UR.searchResultsTable($('#tblSearchUsersResults'));

        $('#UserInfo_Email').on('keyup', function () {
            $('#UserInfo_UserName').val($(this).val());
        });

        $('#btnSaveUser').on('click', function () {
            var wgRoles = getTableContentIDs('tblWorkGroupsRolesPerUser');
            $('#UserInfo_WorkGroup').empty();
            $('#UserInfo_WorkGroup').val(wgRoles);
            $('#frmUser').validate().settings.ignore = '.ignore-validation';
            $('#frmUser').validate();
            $('#frmUser').submit();
        });

        $('.add-button').unbind('click').on('click', function (e) {
            var flag = false;
            if ($('#tblWorkGroupsRolesPerUser tbody').find('tr.editing-row').length > 0) {
                flag = true;
                //$('#tblWorkGroupsRolesPerUser tbody tr.editing-row').remove();
            }
            var valid = flag == false ? validateInsertionAttempt('tblWorkGroupsRolesPerUser', 'WorkGroupsList', 'RolesList') : '1';
            switch (valid) {
                case "-1": {
                    UI.messageBox(-1, "Duplicated Value", null, null);
                    break;
                }
                case "0": {
                    UI.messageBox(-1, 'Option NOT Valid', null, null);
                    break;
                }
                case "1": {
                    var builder = '<tr id="' + $('#WorkGroupsList option:selected').val() + '|' + $('#RolesList option:selected').val() + '|' + $('input:checkbox[name="UserInfo_ManageReservations"]').is(':checked') + '|' + $('input:checkbox[name="UserInfo_ManageServices"]').is(':checked') + '">'
                                + '<td>' + $('#WorkGroupsList option:selected').text() + '</td>'
                                + '<td>' + $('#RolesList option:selected').text() + '</td>'
                                + '<td style="text-transform:capitalize">' + $('input:checkbox[name="UserInfo_ManageReservations"]').is(':checked') + '</td>'
                                + '<td style="text-transform:capitalize">' + $('input:checkbox[name="UserInfo_ManageServices"]').is(':checked') + '</td>'
                                + '<td class="tds"><img class="delete-row" src="/Content/themes/base/images/cross.png"/></td>'
                                + '</tr>';
                    if (!flag) {
                        $('#tblWorkGroupsRolesPerUser').append(builder);
                    }
                    else {
                        $('#tblWorkGroupsRolesPerUser tbody tr.editing-row').before(builder);
                        $('#tblWorkGroupsRolesPerUser tbody tr.editing-row').remove();                       
                    }
                    $('#WorkGroupsList').val(0);
                    $('#RolesList').val(0);
                    $('input:checkbox[name="UserInfo_ManageReservations"]').removeAttr('checked');
                    $('input:checkbox[name="UserInfo_ManageServices"]').removeAttr('checked');
                    break;
                }
            }
            UI.tablesHoverEffect();
            UI.tablesStripedEffect();
            UR.deleteAuxTablesRows();
            UR.makeTblWorkGroupsSelectable();
        });

        function validateInsertionAttempt(table, select, select2) {
            var result = '';
            if ($('#' + select + ' option:selected').val() == 0)
                result = "0";
            else {
                if (select2 != undefined)
                    if ($('#' + select2 + ' option:selected').val() == 0)
                        result = "0";
                var n = 0;
                $('#' + table + ' tbody tr').not('theader').each(function (e) {
                    var cellText = $(this).children('td:nth-child(1)')[0].textContent;
                    var secondCellText = select2 != undefined ? $(this).children('td:nth-child(2)')[0].textContent : '';
                    if ($('#' + select + ' option:selected').text() == cellText) {
                        if (select2 != undefined) {
                            if ($('#' + select2 + ' option:selected').text() == secondCellText)
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

        function getTableContentIDs(table) {
            var ids = '';
            $('#' + table + ' tbody tr').each(function () {
                ids += $(this).attr('id') + ',';
            });
            ids = ids.substr(0, ids.length - 1);
            return ids;
        }

        $('select[multiple="multiple"]').multiselect({
            noneSelectedText: "--Select One--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();

        UR.workgroupDependentListActions();

        UI.updateListsOnTerminalsChange();

        $('#btnNewUsersInfo').on('click', function () {
            $('#addWorkGroupRole').prop('value', 'ADD');
        });

        $('#btnSaveUser').on('click', function () {
            $('#addWorkGroupRole').prop('value', 'ADD');
        });
}

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchUsersResults', tableColumns.length - 1);
        UR.oTable = $('#tblSearchUsersResults').dataTable();
        UR.oTable.fnSort([[6, 'desc']]);
        $('.paging_two_button').children().on('mousedown', function (e) {
            if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            }
            $(this).on('click', function () { UR.makeTableRowsSelectable(); });
        });
        $('#tblSearchUsersResults_length').unbind('change').on('change', function () {
            //UR.makeTableRowsSelectable();
            UR.actionsAfterRenderTable();
        });
        UR.actionsAfterRenderTable();
    }

    var actionsAfterRenderTable = function () {
        UR.makeTableRowsSelectable();
        $.getJSON('/Users/GetDDLData', { itemID: 0, path: 'supervisors' }, function (data) {
            $('#SupervisorsList').fillSelect(data);
        });
        if (!$('#tblSearchUsersResults tbody tr').children('td').hasClass('dataTables_empty')) {
            $('#tblSearchUsersResults tbody tr').each(function (index) {
                var approved = $(this).children('td:nth-child(7)').text().trim().toString();
                if (approved == 'False')
                    $(this).addClass('expired');
                else
                    $(this).removeClass('expired');
            });
        }
    }

    var saveUserSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'User Saved') {
                if (UR.oTable == undefined) {
                    UR.oTable = $('#tblSearchUsersResults').dataTable();
                }                
                var oSettings = UR.oTable.fnSettings();
                var iAdded = UR.oTable.fnAddData([
                    $('#UserInfo_UserName').val(),
                    $('#UserInfo_FirstName').val(),
                    $('#UserInfo_LastName').val(),
                    $('#UserInfo_JobPositions option[value=' + $('#UserInfo_JobPositions').val()[0] + ']').text(),
                    $('#WorkGroupsList option[value=' + $('#UserInfo_WorkGroup').val().split(',')[0].split('|')[0] + ']').text(),
                    $('#RolesList option[value=' + $('#UserInfo_WorkGroup').val().split(',')[0].split('|')[1] + ']').text(),
                    $('#UserInfo_Email').val(),
                    $('#UserInfo_Company option[value=' + $('#UserInfo_Company').val(data.UserInfo_Company) + ']').text(),
                    $('#UserInfo_Departament option[value=' + $('#UserInfo_Departament').val(data.UserInfo_Departament) + ']').text(),
                    $('#UserInfo_DepartamentPhone').val(data.UserInfo_DepartamentPhone),
                    $('#UserInfo_PhoneEXT').val(data.UserInfo_PhoneEXT),
                    $('#UserInfo_PersonalPhoneNumber').val(data.UserInfo_PersonalPhoneNumber),
                    $('#UserInfo_Language option[value =' + $('#UserInfo_Language').val(data.UserInfo_Language) + ']').text(),
                    $('#UserInfo_OPC option[value=' + $('#UserInfo_OPC').val(data.UserInfo_OPC) + ']').text(),
                    $('input:radio[name="UserInfo_IsApproved"]:checked').val(),
                    $('input:radio[name="UserInfo_IsLockedOut"]:checked').val(),
                    '',
                    //$('#UserInfo_LastDateActivity').val(data.ItemID.lastDate),
                    '<img src="/Content/themes/base/images/cross.png" class="right" id="delP' + data.ItemID.userID + '">'
                ]);
                $('#UserInfo_LastDateActivity').val(data.ItemID.lastDate)
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', data.ItemID.userID);
                UR.oTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                UR.makeTableRowsSelectable();
                $('#btnNewUsersInfo').click();
                //$('#' + data.ItemID.userID).click();
                //$('#frmUser').clearForm();
                UpdateSupervisor(data.ItemID.userID);
            }
            else{
                $('#' + data.ItemID).children('td:nth-child(1)').text($('#UserInfo_UserName').val());
                $('#' + data.ItemID).children('td:nth-child(2)').text($('#UserInfo_FirstName').val());
                $('#' + data.ItemID).children('td:nth-child(3)').text($('#UserInfo_LastName').val());
                //$('#' + data.ItemID).children('td:nth-child(4)').text($('#UserInfo_JobPositions option[value="' + $('#UserInfo_JobPositions').val()[0] + '"]').text());
                if ($('#UserInfo_JobPositions').length > 0) {
                    $('#' + data.ItemID).children('td:nth-child(4)').text($('#UserInfo_JobPositions option[value="' + $('#UserInfo_JobPositions').val()[0] + '"]').text());
                }
                else{
                    $('#' + data.ItemID).children('td:nth-child(4)').text("");               
                }
                if ($('#UserInfo_WorkGroup').length > 0)
                {
                   $('#' + data.ItemID).children('td:nth-child(5)').text($('#WorkGroupsList option[value=' + $('#UserInfo_WorkGroup').val().split(',')[0].split('|')[0] + ']').text());
                }
                else
                {
                    $('#' + data.ItemID).children('td:nth-child(5)').text("");
                }
                if($('#RolesList').length > 0)
                {
                    $('#' + data.ItemID).children('td:nth-child(6)').text($('#RolesList option[value=' + $('#UserInfo_WorkGroup').val().split(',')[0].split('|')[1] + ']').text());
                }
                else
                {
                    $('#' + data.ItemID).children('td:nth-child(6)').text("");
                }
                $('#' + data.ItemID).children('td:nth-child(7)').text($('#UserInfo_Email').val());
                $('#' + data.ItemID).children('td:nth-child(8)').text($('#UserInfo_Company option[value="' + data.UserInfo_Company + '"]').attr('selected', true));
                $('#' + data.ItemID).children('td:nth-child(9)').text($('#UserInfo_Departament option[value="' + data.UserInfo_Departament + '"]').attr('selected', true));
                $('#' + data.ItemID).children('td:nth-child(10)').text($('#UserInfo_DepartamentPhone').val());
                $('#' + data.ItemID).children('td:nth-child(11)').text($('#UserInfo_PhoneEXT').val());
                $('#' + data.ItemID).children('td:nth-child(12)').text($('#UserInfo_PersonalPhoneNumber').val());
                $('#' + data.ItemID).children('td:nth-child(13)').text($('#UserInfo_Language option[value="' + data.UserInfo_Language + '"]').attr('selected', true));
                $('#' + data.ItemID).children('td:nth-child(14)').text($('#UserInfo_OPC option[value=' + $('#UserInfo_OPC').val(data.UserInfo_OPC) + ']').text());
                $('#' + data.ItemID).children('td:nth-child(15)').text($('input:radio[name="UserInfo_IsApproved"]:checked').val());
                $('#' + data.ItemID).children('td:nth-child(16)').text($('input:radio[name="UserInfo_IsLockedOut"]:checked').val());
                $('#' + data.ItemID).children('td:nth-child(17)').text($('#UserInfo_LastDateActivity').val());                
            }
            UR.actionsAfterRenderTable();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }
    function UpdateSupervisor(UserID)
    {
        $.ajax({
            url: '/Users/GetDDLData',
            cache: false,
            type: 'POST',
            data: {itemType: 'supervisorsUpdate'},
            success: function (data) {                
                $('#UserInfo_Supervisors').html('');
                for (var x = 0; x < data.length; x++) {
                    var userID = data[x].Value;
                    var userName = data[x].Text;
                    $('#UserInfo_Supervisors').append('<option Value="' + userID + '">' + userName + '</option>');
                }
                $('#UserInfo_Supervisors').multiselect('refresh');
            }
        });
     }

    var deleteAuxTablesRows = function () {
        $('.delete-row').unbind('click').on('click', function () {
            $(this).parents('tr').first().remove();
            $('#addWorkGroupRole').prop('value', 'ADD');
            UI.tablesStripedEffect();
        });
    }

    var makeTableRowsSelectable = function ()  {
        //$('#tblSearchUsersResults tbody tr').not('theader').unbind('click').on('click', function (e) {
        UR.oTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    var event = $.Event('keydown');
                    event.keyCode = 27;
                    $(document).trigger(event);
                    $(this).parent().find('.selected-row').removeClass('selected-row primary');
                    $(this).addClass('selected-row primary');
                    $('#frmUser').clearForm();
                    $('#UserInfo_UserID').val($(this).attr('id'));
                    $('#tblWorkGroupsRolesPerUser tbody').empty();
                    $.ajax({
                        url: 'Users/GetUserInfo',
                        cache: false,
                        type: 'POST',
                        data: { userID: $(this).attr('id') },
                        success: function (data) {
                            var builder = '';
                            $('#UserInfo_LastDateActivity').val(data.UserInfo_LastDateActivity);
                            $('#UserInfo_UserName').val(data.UserInfo_UserName);
                            $('#UserInfo_FirstName').val(data.UserInfo_FirstName);
                            $('#UserInfo_LastName').val(data.UserInfo_LastName);
                            $('#UserInfo_SPIUserName').val(data.UserInfo_SPIUserName);
                            $('#UserInfo_Password').val('123456');
                            $('#UserInfo_ConfirmPassword').val('123456');
                            $('#UserInfo_Email').val(data.UserInfo_Email);
                            $('#UserInfo_Company option[value="' + data.UserInfo_Company + '"]').attr('selected', true)
                              .trigger('change');

                            if($('#UserInfo_Company option[value="' + data.UserInfo_Company + '"]').length == 0)
                                UI.messageBox(0, 'The company from  user selected  it doesn´t exist in the selected terminals', 7);
                            
                            $('#UserInfo_Departament').on('change', function()
                            {
                                if (data.UserInfo_Departament != undefined)
                                  $('#UserInfo_Departament').off('change'),
                                  $('#UserInfo_Departament option[value="' + data.UserInfo_Departament + '"]').attr('selected', true);                               
                            });
                           
                            $('#UserInfo_Language option[value="' + data.UserInfo_Language + '"]').attr('selected', true);
                            $('#UserInfo_DepartamentPhone').val(data.UserInfo_DepartamentPhone);
                            $('#UserInfo_PhoneEXT').val(data.UserInfo_PhoneEXT);
                            $('#UserInfo_PersonalPhoneNumber').val(data.UserInfo_PersonalPhoneNumber);
                            $('#UserInfo_OPC option[value="' + data.UserInfo_OPC + '"]').attr('selected', true)
                            if (data.UserInfo_IsApproved) { 
                                $('input:radio[name=UserInfo_IsApproved]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=UserInfo_IsApproved]')[1].checked = true;
                            }
                            if (data.UserInfo_IsLockedOut) {
                                $('input:radio[name=UserInfo_IsLockedOut]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=UserInfo_IsLockedOut]')[1].checked = true;
                            }

                            if ($('input[name=UserInfo_JobPositions]:hidden').length > 0)
                            {
                                $.each(data.UserInfo_JobPositions, function (index, item) {
                                    $('input[name=UserInfo_JobPositions]').val(item);
                                });
                            }
                            else
                            {
                                $.each(data.UserInfo_JobPositions, function (index, item) {
                                    $('#UserInfo_JobPositions').find('option[value="' + item + '"]').attr('selected', true);
                                });
                            }
                            if($('input[name=UserInfo_Terminals]:hidden').length > 0)
                            {
                                $.each(data.UserInfo_Terminals, function (index, item) {
                                    $('input[name=UserInfo_Terminals]').val(item);
                                });
                            }
                            else
                            {
                                $.each(data.UserInfo_Terminals, function (index, item) {
                                    $('#UserInfo_Terminals').find('option[value="' + item + '"]').attr('selected', true);
                                });
                            }

                            if ($('input[name=UserInfo_Supervisors]:hidden').length > 0)
                            {
                                $.each(data.UserInfo_Supervisors, function (index, item) {
                                    $('input[name=UserInfo_Supervisors]').val(item);
                                });
                            }
                            else
                            {
                                $.each(data.UserInfo_Supervisors, function (index, item) {
                                    $('#UserInfo_Supervisors').find('option[value="' + item + '"]').attr('selected', true);
                                });
                            }
                            $.each(data.UserInfo_Destinations, function (index, item) {
                                $('#UserInfo_Destinations').find('option[value="' + item + '"]').attr('selected', true);
                            });
                            //$.each(data.UserInfo_JobPositionsIDs, function (index, item) {
                            //    builder += '<tr id="' + item.Key + '"><td>'
                            //    + item.Value + '</td>'
                            //    + '<td class="tds"><img class="delete-row" src="/Content/themes/base/images/cross.png"/></td>'
                            //    + '</tr>';
                            //    $('#JobPositionsList option[value="' + item.Key + '"]').attr('selected', true);
                            //});
                            //$('#tblJobPositionsPerUser').append(builder);
                            //builder = '';
                            //$.each(data.UserInfo_DestinationsIDs, function (index, item) {
                            //    builder += '<tr id="' + item.Key + '"><td>'
                            //    + item.Value + '</td>'
                            //    + '<td class="tds"><img class="delete-row" src="/Content/themes/base/images/cross.png"/></td>'
                            //    + '</tr>';
                            //    $('#DestinationsList option[value="' + item.Key + '"]').attr('selected', true);
                            //});
                            //$('#tblDestinationsPerUser').append(builder);
                            //builder = '';
                            //$.each(data.UserInfo_TerminalsIDs, function (index, item) {
                            //    builder += '<tr id="' + item.Key + '"><td>'
                            //    + item.Value + '</td>'
                            //    + '<td class="tds"><img class="delete-row" src="/Content/themes/base/images/cross.png"/></td>'
                            //    + '</tr>';
                            //    $('#TerminalsList option[value="' + item.Key + '"]').attr('selected', true);
                            //});
                            //$('#tblTerminalsPerUser').append(builder);
                            //builder = '';
                            //$.each(data.UserInfo_SupervisorsIDs, function (index, item) {
                            //    builder += '<tr id="' + item.Key + '"><td>'
                            //    + item.Value + '</td>'
                            //    + '<td class="tds"><img class="delete-row" src="/Content/themes/base/images/cross.png"/></td>'
                            //    + '</tr>';
                            //    $('#SupervisorsList option[value="' + item.Key + '"]').attr('selected', true);
                            //});
                            //$('#tblSupervisorsPerUser').append(builder);

                            $.each(data.UserInfo_WorkGroupsRolesIDs, function (index, item) {
                                var values = item.Value.toString().split('|');
                                builder += '<tr id="' + item.Key + '">'
                                + '<td>' + values[0] + '</td>'
                                + '<td>' + values[1] + '</td>'
                                + '<td>' + values[2] + '</td>'
                                + '<td>' + values[3] + '</td>'
                                + '<td class="tds"><img id="tdsWG" class="delete-row" src="/Content/themes/base/images/cross.png"/></td>'
                                + '</tr>';
                            });
                         
                            $('#tblWorkGroupsRolesPerUser tbody').append(builder);
                            $('select[multiple="multiple"]').multiselect('refresh');
                            UI.tablesHoverEffect();
                            UI.tablesStripedEffect();
                            UR.deleteAuxTablesRows();
                            UR.makeTblWorkGroupsSelectable();
                            UI.expandFieldset('fdsUsersInfo');
                            UI.scrollTo('fdsUsersInfo', null);

                            //Remove supervisor Selected
                            var UserID = $('#UserInfo_UserID').val();
                            var Position = 0;
                            // $('#UserInfo_Supervisors').find('option[value =' + UserID + ']').remove();
                            $('#UserInfo_Supervisors option').each(function (contador, x) {
                                if ($(x).attr('value') == UserID)
                                { Position = contador; }
                            });
                            $("label[for='ui-multiselect-UserInfo_Supervisors-option-" + Position + "']").remove();                           
                        }
                    });
                }
            }
            else
            UI.confirmBox('Do you confirm you want to proceed?', deleteUser, [$(this).attr('id')]);
        });
    }

    var makeTblWorkGroupsSelectable = function () {
        $('#tblWorkGroupsRolesPerUser tbody tr').not('theader').unbind('click').on('click', function () {            
            $('#WorkGroupsList option[value="' + $(this).attr('id').split('|')[0] + '"]').attr('selected', true);
            $('#RolesList option[value="' + $(this).attr('id').split('|')[1] + '"]').attr('selected', true);
            $(this).parent('tbody').find('.editing-row').removeClass('editing-row selected-row');
            $(this).addClass('editing-row selected-row');
            $('#addWorkGroupRole').prop('value', 'UPDATE');
            if ($(this).attr('id').split('|')[2] == 'True' || $(this).attr('id').split('|')[2] == 'true') {
                $('input:checkbox[name="UserInfo_ManageReservations"]')[0].checked = true;
            }
            else {
                $('input:checkbox[name="UserInfo_ManageReservations"]')[0].checked = false;
            }
            if ($(this).attr('id').split('|')[3] == 'True' || $(this).attr('id').split('|')[3] == 'true') {
                $('input:checkbox[name="UserInfo_ManageServices"]')[0].checked = true;
            }
            else {
                $('input:checkbox[name="UserInfo_ManageServices"]')[0].checked = false;
            }
        });
        if ($('#addWorkGroupRole').val() == 'UPDATE' && $('#tblWorkGroupsRolesPerUser tbody').children('.editing-row selected-row').length == 0) {
            $('#addWorkGroupRole').prop('value', 'ADD');
        }
    }

    function deleteUser(userID) {
        $.ajax({
            url: '/Users/DeleteUser',
            cache: false,
            type: 'POST',
            data: { userID: userID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#' + userID).hasClass('selected-row')) {
                        $(document).find('.selected-row').each(function () {
                            var event = $.Event('keydown');
                            event.keyCode = 27;
                            $(document).trigger(event);
                        });
                    }
                    $('#' + data.ItemID).children('td:nth-child(7)').text('False');
                    UR.actionsAfterRenderTable();
                    UR.oTable.fnDeleteRow($('#' + userID)[0]);
                    ///UI.tablesHoverEffect();
                    //UI.tablesStripedEffect();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    //change supervisorListDependent
    var workgroupDependentListActions = function () {
        $('#divAvailableWorkGroups').on('change', function () {
            $("input[name=radWorkGroup]:checked").each(function () {
                    var workGroupID = $(this).attr('value');
                    $('.workgroup-dependent-list').each(function () {
                        var id = $(this).attr('id');
                        var route = $(this).attr('data-route');
                        var parameter = $(this).attr('data-route-parameter');
                        $.getJSON(route, { itemType: parameter, itemID:workGroupID }, function (data) {
                            $('#' + id).fillSelect(data);
                            $('#' + id).multiselect('refresh');
                        });
                    });
                    });
        });
    }

    //load supervisor List
    //var workgroupDependentListActions = function () {
    //    $('.workgroup-dependent-list').on('loaded', function () {
    //        $(this).each(function () {
    //            var id = $(this).attr('id');
    //            var route = $(this).attr('data-route');
    //            var parameter = $(this).attr('data-route-parameter');
    //            $.getJSON(route, { path: parameter }, function (data) {
    //                $('#' + id).fillSelect(data);
    //                $('#' + id).multiselect('refresh');
    //            });
    //        });
    //    });
    //}
   
    return {
        init: init,
        saveUserSuccess: saveUserSuccess,
        searchResultsTable: searchResultsTable,
        deleteAuxTablesRows: deleteAuxTablesRows,
        makeTableRowsSelectable: makeTableRowsSelectable,
        actionsAfterRenderTable: actionsAfterRenderTable,
        makeTblWorkGroupsSelectable: makeTblWorkGroupsSelectable,
        workgroupDependentListActions: workgroupDependentListActions,
        oTable: oTable
    }
}();
