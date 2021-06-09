$(function () {
    LOGON.init();
});

var LOGON = function () {

    var init = function () {


        $('#UserName').unbind('keyup').on('keyup', function (e) {
            console.log('a');
            //$('#UserName').val($('#UserName').val().trim(' '));
            if (e.keyCode === 13) {
                $('#Password').focus();
            }
            else if (e.keyCode === 32) {
                e.preventDefault();
            }
        });

        $('#UserName').unbind('focusout').on('focusout', function () {
            console.log('b');
            UI.selectedWorkGroup = '';
            UI.selectedRole = '';
            $('#UserName').val($('#UserName').val().trim(' '));
            UI.loadWorkGroups($('#UserName').val());
            $('#headerWorkGroup').show();
            $('#divAvailableWorkGroups').show();
        });
      
        //********** clear workgroups div if username field gets focus
        $('#UserName').unbind('focus').on('focus', function (e) {
            UI.workGroupsClose();
            $('#divAvailableWorkGroups').html('');
            $('#divSelectedWorkGroup').html('Work Groups');
            $('#divAvailableRoles').html('');
            $('#divSelectedRole').html('Roles');
            //$('#UserName').unbind('keydown').on('keydown', function (e) {
            //    if (e.keyCode === 13) {
            //        $('#Password').focus();
            //    }
            //});
        });

        //$('#UserName').unbind('keydown').on('keydown', function (e) {
        //    console.log('d');
        //    if (e.keyCode === 13) {
        //        $('#Password').focus();
        //    }
        //});
        //**********

        //********** clear workgroups div if password field gets focus

        $('#Password').unbind('keydown').on('keydown', function (e) {
            console.log('e');
            if (e.keyCode === 13) {
                $('#btnLogOn').trigger('click');
                ////verificar que ya se haya verificado el username
                //if (UI.selectedWorkGroup != undefined && UI.selectedWorkGroup != '') {
                //    $('#Password').blur();
                //    $('#btnLogOn').trigger('click');
                //} else {
                //    //notificar que espere 
                //    if (UI.validatingWorkGroups) {
                //        UI.messageBox(0, 'Wait until LOG ON button is blue color. We are validating your username.');
                //    } else {

                //        UI.messageBox(0, 'Your username is locked.');
                //    }
                //}
            } else {
                if (UI.validatingWorkGroups == null) {
                    UI.loadWorkGroups();
                }
                //if (UI.selectedWorkGroup != undefined && UI.selectedWorkGroup != '' && $('#btnLogOn').attr('disabled') == "disabled") {
                //    $('#btnLogOn').attr('disabled', null).removeClass('disabled');
                //}
            }
        });

        //**********

        //$(window).bind('keydown', function (e) {
        //    if (e.keyCode === 13) {
        //        $('#btnLogOn').trigger('click');
        //        //if ($('#UserName').val() != '' && $('#Password').val != '' && $('#divAvailableWorkGroups input[type=radio]:checked').length > 0) {
        //        //    //verificar que ya se haya verificado el username
        //        //    if (UI.selectedWorkGroup != undefined && UI.selectedWorkGroup != '') {
        //        //        $('#btnLogOn').trigger('click');
        //        //    } else {
        //        //        //notificar que espere
        //        //        if (UI.validatingWorkGroups) {
        //        //            UI.messageBox(0, 'Wait until LOG ON button is blue color. We are validating your username.');
        //        //        } else {
        //        //            UI.messageBox(0, 'Your username is locked.');
        //        //        }                    
        //        //    }                
        //        //}
        //    }
        //});

        $('#btnLogOn').on('click', function () {
            console.log('f');
            if ($('#UserName').val() != '' && $('#Password').val() != '') {
                if (localStorage.Eplat_LastLogin == $('#UserName').val()) {
                    $('#WorkGroupID').val(localStorage.Eplat_SelectedWorkGroupID);
                    $('#RoleID').val(localStorage.Eplat_SelectedRole);
                    $('#Terminals').val(localStorage.Eplat_SelectedTerminals);

                    if ($('#WorkGroupID').val() != '' && $('#RoleID').val() != '') {
                        $('#frmLogIn').submit();
                    }
                }
                else {
                    if (UI.selectedWorkGroup != undefined && UI.selectedWorkGroup != '') {
                        if ($('#divAvailableWorkGroups input[type=radio]:checked').length > 0) {
                            localStorage.Eplat_LastLogin = $('#UserName').val();
                            $('#WorkGroupID').val(UI.selectedWorkGroup);
                            $('#RoleID').val(UI.selectedRole);
                            $('#Terminals').val(UI.selectedTerminals);
                            $('#frmLogIn').submit();
                        }
                    } else {
                        if (UI.validatingWorkGroups == null) {
                            UI.loadWorkGroups();
                        } else if (UI.validatingWorkGroups == true) {
                            UI.messageBox(0, 'Wait until LOG ON button is blue color. We are validating your username.');
                        } else if (UI.validatingWorkGroups == false) {
                            if (UI.userState == 3) {
                                UI.messageBox(0, 'The username doesn\'t exist, please verify if it\'s correctly typed.');
                            } else if (UI.userState == 2) {
                                UI.messageBox(-1, 'Your username is locked.');
                            }
                        }
                    }
                }
            } else if ($('#UserName').val() != '' && $('#Password').val() == '') {
                if (UI.userState == 1) {
                    UI.messageBox(0, 'Please type your password.', null, null);
                } else if (UI.userState == 3) {
                    UI.messageBox(0, 'The username doesn\'t exist, please verify if it\'s correctly typed.', 5, null);
                } else if (UI.userState == 2) {
                    UI.messageBox(-1, 'Your username is locked.', 5, null);
                }
            } else {
                UI.messageBox(0, 'Please type your credentials.', 3, null);
            }
        });
    }

    return {
        init: init
    }
}();