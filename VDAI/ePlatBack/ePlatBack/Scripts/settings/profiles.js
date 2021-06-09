$(function () {
    PROFILE.init();
});

var PROFILE = function () {

    var flag;

    var previousParent;

    function ComponentTypes() {
        var componentTypes = '';
        $.ajax({
            async: false,
            url: '/Admin/GetComponentsTypes',
            type: 'POST',
            cache: false,
            success: function (data) {
                componentTypes = '<select id="drpComponentTypes" class="type-component">';
                componentTypes += '<option value"0" selected="selected" >Select One</option>';
                $.each(data, function (index, item) {
                    componentTypes += '<option value="' + item.Value + '" >' + item.Text + '</option>';
                });
                componentTypes += '</select>';
            }
        });
        return componentTypes;
    }

    function getTableNames() {
        var selectBuilder = new Array();
        $.ajax({
            async: false,
            url: '/Admin/GetTables',
            type: 'POST',
            cache: false,
            success: function (data) {
                selectBuilder = data;
            }
        });
        return selectBuilder;
    }

    function liNewComponent(componentID) {
        var componentTypes = ComponentTypes();
        var builder = '';
        var info = new Object();
        if (!componentID) {
            builder = '<li id="liNewComponent" class="new-component">'
                + '<div style="height:25px">'
                + componentTypes
                + '<input type="text" id="newComponent" placeholder="Component Name" style="margin-left: 20px"/>'
                + '<input type="textarea" id="description" placeholder="description"/>'
                + '<input type="button" class="submit" id="saveComponent" value="Save" style="margin-left: 20px"/>'
                + '</div>'
                + '</li>';
        }
        else {
            $.ajax({
                async: false,
                url: '/Admin/GetComponentInfo',
                cache: false,
                type: 'POST',
                data: { componentID: componentID },
                success: function (data) {
                    info = data;
                    //code update 2014-11-20
                    builder = '<li id="liNewComponent" class="new-component">'
                        + '<div style="height:25px">'
                        + '<input type="hidden" id="newComponentID" />'
                        + componentTypes
                        + '<input type="text" id="newComponent" placeholder="Component Name" style="margin-left: 20px"/>';
                    if (data.ProfileInfo_TableName != null) {
                        builder += '<input type="text" class="table-names"/>'
                        + '<input type="text" class="table-fields"/>';
                    }
                    builder += '<input type="textarea" id="description" placeholder="description"/>'
                    + '<input type="button" class="submit" id="saveComponent" value="Save" style="margin-left: 20px"/>'
                    + '</div>'
                    + '</li>';
                    //end update


                    //builder = '<li id="liNewComponent" class="new-component">'
                    //    + '<div style="height:25px">'
                    //    + '<input type="hidden" id="newComponentID" />'
                    //    + componentTypes
                    //    + '<input type="text" id="newComponent" placeholder="Component Name" style="margin-left: 20px"/>'
                    //    + '<input type="textarea" id="description" placeholder="description"/>'
                    //    + '<input type="button" class="submit" id="saveComponent" value="Save" style="margin-left: 20px"/>'
                    //    + '</div>'
                    //    + '</li>';
                    //if (data.ProfileInfo_TableName != null) {
                    //    builder += '<input type="text" class="table-names"/>'
                    //    + '<input type="text" class="table-fields"/>';
                    //}
                }
            });
        }
        return { builder: builder, params: info };
    }

    var init = function () {
        PROFILE.getAllLeadTypes();
        PROFILE.getAllLeadTypes();
        PROFILE.getAllLeadSources();
        PROFILE.getAllBookingStatus();
        PROFILE.getAllResortFeeTypes();
        PROFILE.getAllReservationStatus();
        PROFILE.getAllVerificationAgreements();
        PROFILE.getAllQualificationRequirements();

        function ProfileModel() {
            this.Profile_ListComponents = '';
            this.Profile_Resorts = '';
            this.Profile_SysWorkGroup = '';
            this.Profile_Role = '';
        }

        function ProfileInfoModel() {
            this.ProfileInfo_ComponentID = '';
            this.ProfileInfo_Create = false;
            this.ProfileInfo_Edit = false;
            this.ProfileInfo_View = false;
            this.ProfileInfo_TableName = '';
            this.ProfileInfo_FieldName = '';
            this.ProfileInfo_Url = '';
            this.ProfileInfo_Alias = '';
            this.ProfileInfo_ComponentOrder = '';
        }

        //$('#fdsProfilesManagement').find('div').first().on('mouseover', function () {
        //    if ($('#ulComponentsList').find('li').length == 1) {
        //        PROFILE.getComponents();
        //    }
        //});
        PROFILE.getComponents();

        $('#ProfileInfo_Role').on('change', function () {

            $('#ulComponentsList').find('input:checkbox:checked').each(function () {
                $(this).attr('checked', false);
            });

            $('#ulComponentsList').find('.order').each(function () {
                $(this).val('');
            });

            $('#ulComponentsList').find('.alias').each(function () {
                $(this).val('');
            });

            if ($('#ProfileInfo_WorkGroup option:selected').val() != 0 && $(this).val() != 0) {
                PROFILE.getPrivileges($('#ProfileInfo_WorkGroup option:selected').val(), $('#ProfileInfo_Role option:selected').val());
                $('#btnSaveProfile').show();
                $('#btnAddResortToTable').show();
                PROFILE.cascadeCheckboxSelection();
                PROFILE.orderComponentsByValue();
            }
            else {
                $('input:checkbox').unbind('click');
                $('input:text:not(.order)').unbind('change');
                $('.changes-flag').each(function () {
                    $(this).removeAttr('value');
                });
                $('#btnSaveProfile').hide();
                $('#btnAddResortToTable').hide();
                //--//
                $('#tblProfileResorts tbody').empty();
            }
        });

        $('#ProfileInfo_WorkGroup').on('change', function () {

            $('#ulComponentsList').find('input:checkbox:checked').each(function () {
                $(this).attr('checked', false);
            });

            $('#ulComponentsList').find('.order').each(function () {
                $(this).val('');
            });

            $('#ulComponentsList').find('.alias').each(function () {
                $(this).val('');
            });

            if ($('#ProfileInfo_Role option:selected').val() != 0 && $(this).val() != 0) {
                PROFILE.getPrivileges($('#ProfileInfo_WorkGroup option:selected').val(), $('#ProfileInfo_Role option:selected').val());
                $('#btnSaveProfile').show();
                $('#btnAddResortToTable').show();
                PROFILE.cascadeCheckboxSelection();
                PROFILE.orderComponentsByValue();
            }
            else {
                $('input:checkbox').unbind('click');
                $('input:text:not(.order)').unbind('change');
                $('.changes-flag').each(function () {
                    $(this).removeAttr('value');
                });
                $('#btnSaveProfile').hide();
                $('#btnAddResortToTable').hide();
                //--//
                $('#tblProfileResorts tbody').empty();
            }
        });

        $('#btnSaveProfile').on('click', function (e, params) {
            var resorts = new Array();
            var components = new Array();
            var listComponents = new Array();
            var counter = 0;
            //get components changed
            $('.changes-flag').each(function (index) {
                //if ($(this).val() == 'true') {
                if ($(this).val() == 'true' || params != undefined) {//params distinct of undefined means that is clonation attempt
                    components[counter] = $(this).parents('li').first().attr('id').substr(2);
                    counter++;
                }
            });

            //fill list of components properties
            $.each(components, function (index, item) {
                var componentModel = new ProfileInfoModel();
                componentModel.ProfileInfo_ComponentID = item;
                componentModel.ProfileInfo_Create = $('#chkCrea' + item).is(':checked');
                componentModel.ProfileInfo_Edit = $('#chkEdit' + item).is(':checked');
                componentModel.ProfileInfo_View = $('#chkView' + item).is(':checked');
                componentModel.ProfileInfo_TableName = $('#table' + item).val() != undefined ? $('#table' + item).val() : null;
                componentModel.ProfileInfo_FieldName = $('#field' + item).val() != undefined ? $('#field' + item).val() : null;
                componentModel.ProfileInfo_Url = $('#txtUrl' + item).val() != undefined ? $('#txtUrl' + item).val() : null;
                componentModel.ProfileInfo_Alias = $('#txtAlias' + item).val() != undefined ? $('#txtAlias' + item).val() : null;
                componentModel.ProfileInfo_ComponentOrder = $('#txtComponentOrder' + item).val() != undefined ? $('#txtComponentOrder' + item).val() : '';
                listComponents[index] = componentModel;
            });

            //get resorts added to profile
            $('#tblProfileResorts tbody tr').each(function (index, item) {
                resorts[index] = $(this).attr('id').substr(6);
            });

            //fill model of profile
            var profileModel = new ProfileModel();
            profileModel.Profile_ListComponents = listComponents;
            profileModel.Profile_Resorts = resorts;

            profileModel.Profile_SysWorkGroup = params != undefined ? params.targetWorkGroup : $('#ProfileInfo_WorkGroup option:selected').val();//UI.selectedWorkGroup;
            profileModel.Profile_Role = params != undefined ? params.targetRole : $('#ProfileInfo_Role option:selected').val();//UI.selectedRole;
            var jsonObj = JSON.stringify(profileModel);

            $.ajax({
                url: '/Admin/SaveSysProfile',
                type: 'POST',
                cache: false,
                data: jsonObj,
                traditional: true,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    if (data.ResponseType > 0) {
                        //restore changesFlags value
                        $.each(components, function (index, item) {
                            $('#hdnChangesFlag' + item).attr('value', '');
                        });
                        UI.loadMenuComponents();
                    }
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnRemoveComponent').on('click', function () {
            var targetLiID = $('#ulComponentsList').find('.selected-row').parent('li').attr('id');
            UI.confirmBox('Do you confirm you want to proceed?', deleteComponent, [targetLiID]);
        });

        $('#divElementsContainer').on('mouseover', function () {
            if (PROFILE.flag == true) {
                PROFILE.floatButtons();
            }
        });

        //resorts
        $('#ProfileInfo_Destination').on('change', function () {
            $.getJSON('/Admin/GetDDLData', { path: 'Destination', id: $(this).val() }, function (data) {
                $('#ProfileInfo_Resorts').fillSelect(data);
            });
        });

        $('#btnAddResortToTable').on('click', function () {
            if ($('#resort' + $('#ProfileInfo_Resorts').val()).length > 0) {
                UI.messageBox(-1, "Resort already exists", null, null);
            }
            else {
                var builder = '<tr id="resort' + $('#ProfileInfo_Resorts option:selected').val() + '">'
                    + '<td>' + $('#ProfileInfo_Resorts option:selected').text() + '</td>'
                    + '<td><img src="/Content/themes/base/images/cross.png" class="remove-row right"></td>'
                    + '</tr>';
                $('#tblProfileResorts tbody').append(builder);
                PROFILE.removeRow();
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
            }
        });

        //roles
        $('#btnSearchRoles').unbind('click').bind('click', function () {
            $('#tblSearchRolesResults > tbody').remove();
            $.ajax({
                url: '/Admin/SearchRoles',
                cache: false,
                type: 'POST',
                data: { role: $('#RoleSearch_Role').val() },
                success: function (data) {
                    var builder = '';
                    $.each(data, function (index, item) {
                        builder += '<tr id="trRole' + item.Item2 + '"><td>' + item.ItemName + '</td>'
                        + '<td class="tds"><img class="right" src="/Content/themes/base/images/cross.png" ></td></tr>';
                    });
                    $('#tblSearchRolesResults').append(builder);
                    PROFILE.makeTblRolesRowsSelectable();
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                }
            });
        });

        //sysWorkGroups
        $('#btnSearchWorkGroups').on('click', function () {
            $('#tblSearchWorkGroupsResults > tbody').remove();
            $.ajax({
                url: '/Admin/SearchWorkGroups',
                cache: false,
                type: 'POST',
                data: { workgroup: $('#WorkGroupSearch_WorkGroup').val() },
                success: function (data) {
                    var builder = '';
                    $.each(data, function (index, item) {
                        builder += '<tr id="trWorkgroup' + item.ItemID + '"><td>' + item.ItemName + '</td>'
                        + '<td class="tds"><img class="right" src="/Content/themes/base/images/cross.png" ></td></tr>';
                    });
                    $('#tblSearchWorkGroupsResults').append(builder);
                    PROFILE.makeTblWorkGroupsRowsSelectable();
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                }
            });
        });

        $('#btnSaveBookingStatusPerWorkGroup').on('click', function () {
            var bookingStatus = new Array();
            $('#ulBookingStatus li:has(.chk-son:checked)').each(function (index) {
                bookingStatus[index] = $(this).attr('id').substr(15);
            });
            $.ajax({
                url: '/Admin/SaveBookingStatusPerWorkGroup',
                cache: false,
                type: 'POST',
                data: JSON.stringify({ workgroupID: $('#WorkGroupInfo_WorkGroupID').val(), bookingStatus: bookingStatus }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveLeadTypesPerWorkGroup').on('click', function () {
            var leadTypes = new Array();
            $('#ulLeadTypes li:has(.chk-son:checked)').each(function (index) {
                leadTypes[index] = $(this).attr('id').substr(10);
            });
            $.ajax({
                url: '/Admin/SaveLeadTypesPerWorkGroup',
                cache: false,
                type: 'POST',
                data: JSON.stringify({ workgroupID: $('#WorkGroupInfo_WorkGroupID').val(), leadTypes: leadTypes }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveLeadSourcesPerWorkGroup').on('click', function () {
            var leadSources = new Array();
            $('#ulLeadSources li:has(.chk-son:checked)').each(function (index) {
                leadSources[index] = $(this).attr('id').substr(12);
            });
            $.ajax({
                url: '/Admin/SaveLeadSourcesPerWorkGroup',
                cache: false,
                type: 'POST',
                data: JSON.stringify({ workgroupID: $('#WorkGroupInfo_WorkGroupID').val(), leadSources: leadSources }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveQualificationRequirementsPerWorkGroup').on('click', function () {
            var qualificationRequirements = new Array();
            $('#ulQualificationRequirements li:has(.chk-son:checked)').each(function (index) {
                qualificationRequirements[index] = $(this).attr('id').substr(26);
            });
            $.ajax({
                url: '/Admin/SaveQualificationRequirementsPerWorkGroup',
                cache: false,
                type: 'POST',
                data: JSON.stringify({ workgroupID: $('#WorkGroupInfo_WorkGroupID').val(), qualificationRequirements: qualificationRequirements }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveResortFeeTypesPerWorkGroup').on('click', function () {
            var resortFeeTypes = new Array();
            $('#ulResortFeeTypes li:has(.chk-son:checked)').each(function (index) {
                resortFeeTypes[index] = $(this).attr('id').substr(15);
            });
            $.ajax({
                url: '/Admin/SaveResortFeeTypesPerWorkGroup',
                cache: false,
                type: 'POST',
                data: JSON.stringify({ workgroupID: $('#WorkGroupInfo_WorkGroupID').val(), resortFeeTypes: resortFeeTypes }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveReservationStatusPerWorkGroup').on('click', function () {
            var reservationStatus = new Array();
            $('#ulReservationStatus li:has(.chk-son:checked)').each(function (index) {
                reservationStatus[index] = $(this).attr('id').substr(19);
            });
            $.ajax({
                url: '/Admin/SaveReservationStatusPerWorkGroup',
                cache: false,
                type: 'POST',
                data: JSON.stringify({ workgroupID: $('#WorkGroupInfo_WorkGroupID').val(), reservationStatus: reservationStatus }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveVerificationAgreementsPerWorkGroup').on('click', function () {
            var verificationAgreements = new Array();
            $('#ulVerificationAgreement li:has(.chk-son:checked)').each(function (index) {
                verificationAgreements[index] = $(this).attr('id').substr(19);
            });
            $.ajax({
                url: '/Admin/SaveVerificationAgreementsPerWorkGroup',
                cache: false,
                type: 'POST',
                data: JSON.stringify({ workgroupID: $('#WorkGroupInfo_WorkGroupID').val(), verificationAgreements: verificationAgreements }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnCloneProfile').on('click', function () {
            if ($('#ProfileInfo_WorkGroup option:selected').val() != '0' && $('#ProfileInfo_Role option:selected').val() != '0') {
                $('#ProfileInfo_TargetWorkGroup option[value="0"]').attr('selected', true);
                $('#ProfileInfo_TargetRole option[value="0"]').attr('selected', true);
                $('.btn-clone-profile').toggle();
            }
            else {
                UI.messageBox(-1, 'You must select a profile first', null, null);
            }
        });

        $('#btnCancelClone').on('click', function () {
            $('.btn-clone-profile').toggle();
        });

        $('#btnSaveClone').on('click', function () {
            $('.btn-clone-profile').toggle();
            if ($('#ProfileInfo_TargetWorkGroup option:selected').val() != 0 && $('#ProfileInfo_TargetRole option:selected').val() != 0) {//comprobar el valor de las listas que cargan el perfil
                $('#btnSaveProfile').trigger('click', { targetWorkGroup: $('#ProfileInfo_TargetWorkGroup option:selected').val(), targetRole: $('#ProfileInfo_TargetRole option:selected').val() });
            }
            else {

            }
        });
    }

    //components
    ///ulComponents structure
    ///ul > (li#li + 'componentID') 
    ///         > div 
    ///             > span.disclose
    ///                 + (:hidden#hdnCompType + 'componentID') 
    ///                 + (span.right 
    ///                       > (span > (:checkbox.create#chkCrea + 'componentID')) 
    ///                       + (span.align-from-left > (:checkbox.edit#chkEdit + 'componentID')) 
    ///                       + (span.align-from-left > (:checkbox.view#chkView + 'componentID')) 
    ///                       + (:text#txtUrl + 'componentID') 
    ///                       + (:text.order#txtComponentOrder + 'componentID') 
    ///                       + (:hidden.changes-flag#hdnChangesFlag + 'componentID'))

    var floatButtons = function () {
        $('#divActionButtons').asFixed(null,'fixed');
        //var top_ = $('#divActionButtons').offset().top - parseFloat($('#divActionButtons').css('margin-top').replace(/auto/, 0));
        //$(window).scroll(function () {
        //    var y = $(this).scrollTop();
        //    if (y >= top_) {
        //        $('#divActionButtons').addClass('fixed');
        //    }
        //    else {
        //        $('#divActionButtons').removeClass('fixed');
        //    }
        //});
        PROFILE.flag = false;
    }

    var getComponents = function () {
        $.ajax({
            url: '/Admin/GetComponentsTree',
            type: 'POST',
            success: function (data) {
                $('#divActionButtons').show();
                PROFILE.flag = true;
                var builder = '';

                function getChildrenNodes(componentID, componentName) {
                    var children = '';
                    ///getChildren
                    $.each(data, function (index, item) {
                        if (item.ParentComponentID == componentID) {
                            children += index + ',';
                        }
                    });
                    ///end getChildren
                    if (children == '') {
                        builder += '</ul></li>';
                    }
                    else {
                        $.each(data, function (index, item) {
                            $.each(children.split(','), function (index2, item2) {
                                if (index == item2 && index != 0) {
                                    //nestedSortable
                                    builder += '<li id="li' + data[index].ComponentID + '">';
                                    //nestable

                                    builder += '<div class="dd-handle" style="height:25px"><span class="disclose"><span></span></span>' + data[index].ComponentName;
                                    builder += '<input type="hidden" id="hdnCompType' + data[index].ComponentID + '" value="' + data[index].ComponentTypeID + '"/>'
                                        + '<span class="right">';
                                    ///condition to add table and tableField fields
                                    if (data[index].ComponentTypeID != 2 && data[index].ComponentTypeID != 1 && data[index].ComponentTypeID != 3) {
                                        builder += '<input type="text" id="table' + data[index].ComponentID + '" class="table-names" placeholder="type table name" value="' + data[index].TableName + '" />';
                                        builder += '<input type="text" id="field' + data[index].ComponentID + '" class="table-fields" placeholder="type table field" value="' + data[index].TableField + '" />';
                                    }
                                    ///block of chekboxes
                                    builder += '<span><input type="checkbox" class="create" id="chkCrea' + data[index].ComponentID + '" /></span>';
                                    builder += '<span class="align-from-left"><input type="checkbox" class="edit" id="chkEdit' + data[index].ComponentID + '" /></span>';
                                    builder += '<span class="align-from-left"><input type="checkbox" class="view" id="chkView' + data[index].ComponentID + '" /></span>';
                                    ///block to add Alias field and value
                                    if (data[index].ComponentTypeID != 2 && data[index].ComponentTypeID != 1) {
                                        builder += '<input class="marg-left alias" type="text" id="txtAlias' + data[index].ComponentID + '" placeholder="Alias" />';
                                    }
                                    ///block to add url field and value if needed
                                    if (data[index].ComponentTypeID == 2) {
                                        if (data[index].Url == undefined) {
                                            builder += '<input class="marg-left" type="text" id="txtUrl' + data[index].ComponentID + '" placeholder="Url" />';
                                        }
                                        else {
                                            builder += '<input class="marg-left" type="text" id="txtUrl' + data[index].ComponentID + '" placeholder="Url" value="' + data[index].Url + '"/>';
                                        }
                                    }
                                    ///block to add componentsOrder fields
                                    if (data[index].ComponentID != 1) {
                                        builder += '&nbsp<input type="text" id="txtComponentOrder' + data[index].ComponentID + '" class="order"/>'
                                            + '<input type="hidden" id="hdnChangesFlag' + data[index].ComponentID + '" class="changes-flag" />';
                                    }
                                    builder += '</span>';
                                    builder += '</div>';
                                    ///end or customized text
                                    builder += '<ul class="sortable dd-list _sortable">';
                                    getChildrenNodes(data[index].ComponentID, data[index].ComponentName);
                                }
                            });
                        });
                        builder += '</ul></li>';
                    }
                    return builder;
                }

                //draw first level of tree
                $.each(data, function (index, item) {
                    if (item.ParentComponentID == 0) {
                        //nestedSortable
                        builder += '<li id="li' + item.ComponentID + '">';
                        //nestable
                        //builder += '<li id="li' + item.ComponentID + '" class="dd-item" data-id="li' + item.ComponentID + '">';

                        builder += '<div class="dd-handle" style="height:25px"><span class="disclose"><span></span></span>' + item.ComponentName;
                        //builder += '<div style="height:25px"><span class="disclose dd-handle"><span></span></span>' + item.ComponentName;
                        builder += '<input type="hidden" id="hdnChangesFlag' + item.ComponentID + '" class="changes-flag" />'
                            + '<input type="hidden" id="hdnCompType' + item.ComponentID + '" value="' + item.ComponentTypeID + '"/>'
                            //+ '<span class="component" id="' + item.ComponentID + '" >' + item.ComponentName + '</span>'
                            + '<span class="right padd-module">'
                            + '<span><input type="checkbox" class="create" id="chkCrea' + item.ComponentID + '" /></span>'
                            + '<span class="align-from-left"><input type="checkbox" class="edit" id="chkEdit' + item.ComponentID + '" /></span>'
                            + '<span class="align-from-left"><input type="checkbox" class="view" id="chkView' + item.ComponentID + '" /></span>'
                            + '<span class="marg-left"><input type="text" id="txtUrl' + item.ComponentID + '" placeholder="Url"';
                        if (item.Url != undefined) {
                            builder += 'value="' + item.Url + '"';
                        }
                        builder += ' /></span></span>';
                        //div class="dd-item"
                        builder += '</div>';
                        builder += '<ul class="sortable dd-list _sortable">'
                        getChildrenNodes(item.ComponentID, item.ComponentName);
                    }
                });
                $('#ulComponentsList').empty();
                $('#ulComponentsList').append(builder);
                $('#ulComponentsList').find('ul:empty').each(function () {
                    $(this).remove();
                });
                PROFILE.nestedSortableFunction();
                PROFILE.expandableFunction();
                PROFILE.ulComponentsHoverEffect();
                PROFILE.makeComponentsListSelectable();
                PROFILE.ableComponentsChangeDetection();
            }
        });
    }

    var getPrivileges = function (workgroup, role) {
        $.ajax({
            url: '/Admin/GetPrivileges',
            type: 'POST',
            cache: false,
            data: { workgroupID: workgroup, roleID: role },
            success: function (data) {
                $.each(data.ListSysComponentsPrivileges, function (index, item) {
                    $('#chkCrea' + item.ComponentID).attr('checked', item.Create);
                    $('#chkEdit' + item.ComponentID).attr('checked', item.Edit);
                    $('#chkView' + item.ComponentID).attr('checked', item.View);
                    $('#txtComponentOrder' + item.ComponentID).val(item.ComponentOrder);
                    if (item.Alias != 'null') {
                        $('#txtAlias' + item.ComponentID).val(item.Alias);
                    }
                });
                $('#tblProfileResorts tbody').empty();
                var builder = '';
                $.each(data.ListProfileResorts, function (index, item) {
                    builder += '<tr id="resort' + item.Value + '">'
                        + '<td>' + item.Text + '</td>'
                        + '<td><img src="/Content/themes/base/images/cross.png" class="remove-row right"></td>'
                        + '</tr>';
                });
                $('#tblProfileResorts tbody').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                PROFILE.removeRow();
                PROFILE.orderComponentsByValue();
                //$('#ulComponentsList').find('ul:has(li)').each(function () {
                //    $('#ulComponentsList li:has(ul)').find('li').tsort('.order', { attr: 'value' });
                //});
            }
        });
    }

    var removeRow = function () {
        $('.remove-row').unbind('click').on('click', function () {
            $(this).parents('tr').first().remove();
        });
    }

    var expandableFunction = function () {
        $('.mjs-nestedSortable-collapsed div span.disclose').unbind('click').on('click', function (e) {
            if (!$(e.target).is(':input')) {
                $(e.target).parents('li').first().toggleClass('mjs-nestedSortable-collapsed').toggleClass('mjs-nestedSortable-expanded');
            }
        });
    }

    var nestedSortableFunction = function () {
        //nestedSortable
        $('ul.sortable').nestedSortable({
            listType: 'ul',
            disableNesting: 'no-nest',
            forcePlaceholderSize: true,
            handle: 'div',
            helper: 'clone',
            items: 'li',
            opacity: .6,
            placeholder: 'placeholder',
            revert: 250,
            tabSize: 25,
            tolerance: 'pointer',
            toleranceElement: '> div',
            rootID: 'ulComponentsList',
            isTree: true,
            startCollapsed: true,
            start: function (e, ui) {
                previousParent = $(ui.item[0]).parents('li').first().attr('id');
            },
            stop: function (e, ui) {
                if (previousParent != $(ui.item[0]).parents('li').first().attr('id')) {
                    if ($('#txtComponentOrder' + $(ui.item[0]).attr('id').substr(2)).length > 0) {
                        $.ajax({
                            url: '/Admin/ChangeComponentParent',
                            cache: false,
                            type: 'POST',
                            data: { componentID: $(ui.item[0]).attr('id').substr(2), parentComponentID: $(ui.item[0]).parents('li').first().attr('id').substr(2) },
                            success: function (data) {
                                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                                if (data.ResponseType > 0) {
                                    $('#' + previousParent).children('ul').children('li').each(function (index, item) {
                                        $(item).find('.order:first').val($(item).index() + 1);
                                        $(item).find('.changes-flag:first').val(true);
                                    });
                                }
                                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                            }
                        });
                    }
                }
                PROFILE.defineOrderValueByPosition($(ui.item[0]).attr('id'));
            }
        });
    }

    var ableComponentsChangeDetection = function () {
        $('#divComponentsListContainer input').unbind('change').on('change', function (e) {
            var componentID = $(e.target).parents('li').first().attr('id').substr(2);
            if ($(e.target).hasClass('order')) {
                PROFILE.verifyComponentsOrderValue(componentID);
                PROFILE.orderComponentsByValue(componentID);
            }
            $('#hdnChangesFlag' + componentID).val(true);
        });
    }

    var createEditFunction = function () {
        $('#btnCreateComponent').unbind('click').on('click', function (event) {
            if ($('#ulComponentsList').find('#liNewComponent').length == 0) {
                if ($('#ulComponentsList').find('.selected-row').parent().hasClass('mjs-nestedSortable-leaf')) {
                    $('#ulComponentsList').find('.selected-row').parent().removeClass('mjs-nestedSortable-leaf')
                        .addClass('mjs-nestedSortable-branch').addClass('mjs-nestedSortable-collapsed');
                    PROFILE.expandableFunction();
                }
                if ($('#ulComponentsList').find('.selected-row').siblings('ul').length > 0) {
                    $('#ulComponentsList').find('.selected-row').siblings('ul').append(liNewComponent().builder);
                }
                else {
                    var newComponent = '<ul class="sortable dd-list _sortable">' + liNewComponent().builder + '</ul>';
                    $('#ulComponentsList').find('.selected-row').parent().append(newComponent);
                }
                if ($('#ulComponentsList').find('.selected-row').parent().hasClass('mjs-nestedSortable-collapsed')) {
                    $('#ulComponentsList').find('.selected-row').find('.disclose').click();
                }
                UI.scrollTo('liNewComponent', null);
                PROFILE.renderTablesFields();
                PROFILE.saveNewComponent();
                PROFILE.autocompleteTablesFields();
            }
        });

        $('#btnEditComponent').unbind('click').bind('click', function (e) {
            var parentComponent = $('#ulComponentsList').find('.selected-row').parent('li').attr('id');
            var data = liNewComponent(parentComponent.substr(2));
            $(data.builder).insertAfter('#' + parentComponent);
            $('#newComponentID').val(data.params.ProfileInfo_ComponentID);
            $('#drpComponentTypes option[value="' + data.params.ProfileInfo_ComponentType + '"]').attr('selected', true);
            $('#newComponent').val(data.params.ProfileInfo_Component);
            $('#description').val(data.params.ProfileInfo_Description);
            $('#drpComponentTypes').siblings('.table-names').val(data.params.ProfileInfo_TableName);
            $('#drpComponentTypes').siblings('.table-fields').val(data.params.ProfileInfo_FieldName);
            $('#componentOrder').val(data.params.ProfileInfo_ComponentOrder);
            UI.scrollTo('liNewComponent', null);
            PROFILE.saveNewComponent();
            PROFILE.renderTablesFields();
            PROFILE.autocompleteTablesFields();
            //SM.inputValidations();
        });
    }

    var _cascadeCheckboxSelection = function () {
        $('input:checkbox').unbind('click').on('click', function (e) {
            var checkClass = $(this).attr('class');
            var childCheckBoxes = $('#divComponentsListContainer').find('input:checkbox').filter('.' + checkClass);
            if ($(e.target).is(':checked') == true) {
                //to check
                //check children
                childCheckBoxes.unbind('change').on('change', function (e) {
                    $('#hdnChangesFlag' + $(e.target).attr('id').substr(7)).val(true);
                    //var firstParent = $(e.target).parents().prevAll('span').find('input:checkbox').filter('.' + checkClass).first();
                    var firstParent = $(e.target).closest('ul').parent('li').find('.' + checkClass).first();
                    //var parents = $(e.target).parents().prevAll('span').find('input:checkbox').filter('.' + checkClass);
                    var parents = $(e.target).parents('ul').prev('div').find('.' + checkClass);
                    var childrenChecked = $(e.target).closest('ul').find('input:checkbox').filter('.' + checkClass).is(':checked');
                    var childrenLis = $(e.target).parents('li').first().children('ul').find('input:checkbox').filter('.' + checkClass);
                    $.each(childrenLis, function () {
                        $('#hdnChangesFlag' + $(this).attr('id').substr(7)).val(true);
                    });
                    if (childrenChecked == false) {
                        $(firstParent).removeAttr('checked');
                    }
                    else {
                        $(firstParent).attr('checked', childrenChecked);
                    }
                    $.each(parents, function (index) {
                        var id = $(this).attr('id').substr(7);
                        $(parents[index + 1]).attr('checked', $(firstParent).is(':checked'));
                        $('#hdnChangesFlag' + id).val(true);
                    });
                });
                //check parents
                childCheckBoxes.parents().find('input:checkbox').filter('.' + checkClass).on('change', function () {
                    $(this).parents('li').first().find('input:checkbox').filter('.' + checkClass).attr('checked', $(this).is(':checked'));
                    $('#hdnChangesFlag' + $(this).attr('id').substr(7)).val(true);
                });
            }
            else {
                //to uncheck
                childCheckBoxes.unbind('change').on('change', function (e) {
                    $('#hdnChangesFlag' + $(e.target).attr('id').substr(7)).val(true);
                    //var firstParent = $(e.target).parents().prevAll('span').find('input:checkbox').filter('.' + checkClass).first();
                    //var parents = $(e.target).parents().prevAll('span').find('input:checkbox').filter('.' + checkClass);
                    var firstParent = $(e.target).closest('ul').parent('li').find('.' + checkClass).first();
                    var parents = $(e.target).parents('ul').prev('div').find('.' + checkClass);
                    var childrenChecked = $(e.target).closest('ul').find('input:checkbox').filter('.' + checkClass).is(':checked');
                    if (childrenChecked == false) {
                        $(firstParent).removeAttr('checked');
                    }
                    else {
                        $(firstParent).attr('checked', childrenChecked);
                    }
                    var parentsChildrenChecked;
                    $.each(parents, function (index) {
                        var id = $(this).attr('id').substr(7);
                        parentsChildrenChecked = $(parents[index]).closest('ul').find('input:checkbox').filter('.' + checkClass).is(':checked');
                        $(parents[index + 1]).attr('checked', parentsChildrenChecked);
                        $('#hdnChangesFlag' + id).val(true);
                    });
                });
                $(e.target).parents('li').first().find('ul').find('input:checkbox').filter('.' + checkClass).each(function () {
                    $(this).attr('checked', $(e.target).is(':checked'));
                    $('#hdnChangesFlag' + $(this).attr('id').substr(7)).val(true);
                });
            }
        });
    }

    var cascadeCheckboxSelection = function () {
        $('input:checkbox').unbind('click').on('click', function (e) {
            var _class = $(e.target).attr('class');
            var _children = $(e.target).parents('li:first').find('.' + _class).not($(e.target));
            var _state = $(e.target).is(':checked');

            //change state of children
            $.each(_children, function (index, item) {
                $(item).attr('checked', _state);
                $(item).parents('li:first').children('div').find('.changes-flag:first').val(true);
            });

            $(e.target).parents('li').not($(e.target).parents('li:first')).each(function () {
                var _siblingsState = false;
                $(this).find('ul:first').children('li').each(function (index, item) {
                    if ($(item).find('.' + _class + ':first').is(':checked')) {
                        _siblingsState = true;
                    }
                });

                if (_siblingsState) {
                    $(this).find('.' + _class + ':first').attr('checked', true);
                    $(this).children('div').find('.changes-flag:first').val(true);
                }
                else {
                    $(this).find('.' + _class + ':first').attr('checked', _state);
                    $(this).children('div').find('.changes-flag:first').val(true);
                }
            });
        });
    }

    var autocompleteTablesFields = function () {
        //$('.table-names').autocomplete('destroy');
        $('.table-names').autocomplete({
            source: getTableNames(), minLength: 4, position: { my: "right top", at: "right bottom" },
            change: function (e, ui) {
                $.ajax({
                    async: false,
                    url: '/Admin/GetFields',
                    type: 'POST',
                    cache: false,
                    data: { tableName: ui.item.value },
                    success: function (data) {
                        //$(e.target).siblings('.table-fields').first().autocomplete('destroy');
                        $(e.target).siblings('.table-fields').first().autocomplete({
                            source: data,
                            position: { my: "right top", at: "right bottom" },
                            minLength: 0
                            //change: function (e, ui) {
                            //    var componentID = $(e.target).parents('li').first().attr('id').substr(2);
                            //    var obj = {
                            //        ProfileInfo_ComponentID: componentID,
                            //        ProfileInfo_TableName: $(e.target).siblings('.table-names').first().val(),
                            //        ProfileInfo_TableField: ui.item.value
                            //    }
                            //    var jsonObj = JSON.stringify(obj);
                            //    $.ajax({
                            //        url: '/Admin/SaveComponent',
                            //        cache: false,
                            //        type: 'POST',
                            //        data: jsonObj,
                            //        dataType: 'json',
                            //        contentType: 'application/json; charset=utf-8',
                            //        traditional: true,
                            //        success: function (data) {
                            //            var duration = data.ResponseType < 0 ? data.ResponseType : null;
                            //            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                            //        }
                            //    });
                            //}
                        });
                    }
                });
            }
        });
    }

    var verifyComponentsOrderValue = function (componentID) {
        //check if is the biggest value
        var numberOfSiblings = $('#li' + componentID).parent('ul').children('li').length;
        if ($('#li' + componentID).children('div').find('.order').val() >= numberOfSiblings) {
            $('#li' + componentID).parent('ul').append($('#li' + componentID));
        }
        else {
            //update 2014-11-20
            $('#li' + componentID).parent('ul').find('li:eq(' + ($('#txtComponentOrder' + componentID).val() - 1) + ')').before($('#li' + componentID));
            //end update
        }
        PROFILE.defineOrderValueByPosition('li' + componentID);
    }

    var defineOrderValueByPosition = function (liChangedID) {
        $('#' + liChangedID).parent('ul').children('li').each(function (index, item) {
            //update 2014-11-20
            $(item).find('.order:first').val($(item).index() + 1);
            $(item).find('.changes-flag:first').val(true);
            //$(item).find('.order').val($(item).index() + 1);
            //$(item).find('.changes-flag').val(true);
            //end update
        });
    }

    var orderComponentsByValue = function (componentID) {
        if (componentID != undefined) {
            $('#li' + componentID).parent('ul').children('li').tsort('.order', { attr: 'value' });
            $('#li' + componentID).parent('ul').children('li').each(function (index, item) {
                $(item).children('div').find('.changes-flag').val(true);
            });
        }
        else {
            $('#ulComponentsList').find('ul:has(li)').children('li').tsort('.order', { attr: 'value' });
        }
    }

    var makeComponentsListSelectable = function () {
        $('.sortable li div').unbind('click').on('click', function (e) {
            if (!$(this).hasClass('selected-row') && !$(e.target).is('span') && !$(e.target).is('input')) {
                $('#ulComponentsList').find('#liNewComponent').remove();
                $('#ulComponentsList').find('.selected-row').removeClass('selected-row secondary');
                var parentComponent = $(this).parent('li').attr('id');
                $('#' + parentComponent).children('div').addClass('selected-row secondary');
                PROFILE.createEditFunction();
            }
        });
    }

    var saveNewComponent = function () {
        $('#saveComponent').unbind('click').on('click', function (e) {
            if ($(e.target).closest('ul').attr('id') != 'ulComponentsList') {
                var parentLi = $(e.target).closest('ul').closest('li').attr('id');
                var parentComponentType = $('#hdnCompType' + parentLi.substr(2)).val();
                if ($('#drpComponentTypes option:selected').val() != 0 && $('#newComponent').val() != "") {
                    var newComponentType = parseInt($('#drpComponentTypes option:selected').val());
                    var flag = true;
                    switch (newComponentType) {
                        case 1: flag = false;
                            break;
                        case 2: if (parentComponentType != 1 && parentComponentType != 2) { flag = false; }
                            break;
                        case 3: if (parentComponentType != 2 && parentComponentType != 3) { flag = false; }
                            break;
                        case 4: if (parentComponentType != 3 && parentComponentType != 2 && parentComponentType != 14) { flag = false; }
                            break;
                        case 5: if (parentComponentType != 3 && parentComponentType != 2 && parentComponentType != 14 && parentComponentType != 13) { flag = false; }
                            break;
                        case 6: if (parentComponentType != 3 && parentComponentType != 2 && parentComponentType != 14 && parentComponentType != 13) { flag = false; }
                            break;
                    }
                    if (flag) {
                        var obj = {
                            ProfileInfo_ComponentID: $('#newComponentID').val(),
                            ProfileInfo_ComponentType: $('#drpComponentTypes option:selected').val(),
                            ProfileInfo_Component: $('#newComponent').val(),
                            ProfileInfo_Description: $('#description').val(),
                            ProfileInfo_ParentComponent: parentLi.substr(2),
                            ProfileInfo_TableName: $('#drpComponentTypes').siblings('.table-names').val() != undefined ? $('#drpComponentTypes').siblings('.table-names').val() : null,
                            ProfileInfo_FieldName: $('#drpComponentTypes').siblings('.table-fields').val() != undefined ? $('#drpComponentTypes').siblings('.table-fields').val() : null
                        }
                        //--//
                        //add value to .order field
                        var orderArray = new Array();
                        $('#' + parentLi).children('ul').find('li:not(#liNewComponent').find('.order').each(function (index) {
                            orderArray[index] = $(this).val();
                        });
                        maxOrderValue = 0;
                        for (var i = 0; i < orderArray.length; i++) {
                            if (maxOrderValue < orderArray[i])
                                maxOrderValue = orderArray[i];
                        }
                        var componentOrder = $('#componentOrder').val();
                        componentOrder = componentOrder == '' ? 1 : componentOrder == undefined ? '' : componentOrder;
                        maxValue = parseInt(maxOrderValue) + 1;
                        componentOrder = componentOrder > maxValue ? maxValue : componentOrder;
                        //--//
                        var jsonObj = JSON.stringify(obj);
                        $.ajax({
                            url: '/Admin/SaveComponent',
                            type: 'POST',
                            cache: false,
                            data: jsonObj,
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            traditional: true,
                            success: function (data) {
                                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                                if (data.ResponseType > 0) {
                                    if (data.ResponseMessage == 'Component Saved') {
                                        var builder = '';
                                        builder += '<li id="li' + data.ItemID + '">'
                                        + '<div style="height:25px"><span class="disclose"><span></span></span>' + $('#newComponent').val()
                                        + '<input type="hidden" id="hdnChangesFlag' + data.ItemID + '" class="changes-flag"/>'
                                        + '<input type="hidden" id="hdnCompType' + data.ItemID + '" value="' + $('#drpComponentTypes option:selected').val() + '"/>'
                                        + '<span class="right">';
                                        if ($('#drpComponentTypes option:selected').val() != 1 && $('#drpComponentTypes option:selected').val() != 2 && $('#drpComponentTypes option:selected').val() != 3) {
                                            var table = $('#liNewComponent').find('.table-names').val() != undefined ? $('#liNewComponent').find('.table-names').val() : '';
                                            var field = $('#liNewComponent').find('.table-fields').val() != undefined ? $('#liNewComponent').find('.table-fields').val() : '';
                                            builder += '<input type="text" class="table-names" placeholder="type table name" value="' + table + '" />';
                                            builder += '<input type="text" class="table-fields" placeholder="type table field" value="' + field + '" />';
                                        }
                                        builder += '<span><input type="checkbox" class="create" id="chkCrea' + data.ItemID + '"/></span>'
                                        + '<span class="align-from-left"><input type="checkbox" class="edit" id="chkEdit' + data.ItemID + '" /></span>'
                                        + '<span class="align-from-left"><input type="checkbox" class="view" id="chkView' + data.ItemID + '" /></span>';
                                        //condition to add alias field if needed
                                        if ($('#drpComponentTypes option:selected').val() != 1 && $('#drpComponentTypes option:selected').val() != 2) {
                                            builder += '<input class="marg-left alias" type="text" id="txtAlias' + data.ItemID + '" placeholder="Alias" />';
                                        }
                                        //condition to add url field if needed
                                        if ($('#drpComponentTypes option:selected').val() == 2) {
                                            builder += '<input class="marg-left" type="text" id="txtUrl' + data.ItemID + '" placeholder="Url" />';
                                        }
                                        builder += '&nbsp<input type="text" id="txtComponentOrder' + data.ItemID + '" value="' + componentOrder + '" class="order"/>'
                                        + '<input type="hidden" id="hdnChangesFlag' + data.ItemID + '" class="changes-flag"/>'
                                        + '</span>'
                                        + '</div>'
                                        + '</li>';
                                        $('#liNewComponent').parent().append(builder);
                                        $('#liNewComponent').remove();
                                        if (1 >= $('#drpComponentTypes option:selected').val() <= 2) {
                                            UI.loadMenuComponents();
                                        }
                                        PROFILE.nestedSortableFunction();
                                        PROFILE.ulComponentsHoverEffect();
                                        PROFILE.makeComponentsListSelectable();
                                        PROFILE.ableComponentsChangeDetection();
                                    }
                                    else {
                                        //update component edition changes
                                        $('#hdnCompType').val($('#drpComponentTypes option:selected').val());
                                        $('#li' + data.ItemID).children()[0].childNodes[1].textContent = $('#newComponent').val();
                                        if ($('#drpComponentTypes option:selected').val() == 2 || $('#drpComponentTypes option:selected').val() == 1) {
                                            UI.loadMenuComponents();
                                        }
                                        if ($('#drpComponentTypes').siblings('.table-names').length > 0) {
                                            if ($('#table' + data.ItemID).length > 0) {
                                                $('#table' + data.ItemID).val($('#drpComponentTypes').siblings('.table-names').val());
                                                $('#field' + data.ItemID).val($('#drpComponentTypes').siblings('.table-fields').val());
                                            }
                                            else {
                                                var builder = '<input type="text" class="table-names" placeholder="type table name" id="table' + data.ItemID + '"/>'
                                                + '<input type="text" class="table-fields" placeholder="type table field" id="field' + data.ItemID + '"/>';
                                                $('#li' + data.ItemID).find('span.right').prepend(builder);
                                                PROFILE.autocompleteTablesFields();
                                                $('#table' + data.ItemID).val($('#drpComponentTypes').siblings('.table-names').val());
                                                $('#field' + data.ItemID).val($('#drpComponentTypes').siblings('.table-fields').val());
                                            }
                                        }
                                        else {
                                            $('#table' + data.ItemID).remove();
                                            $('#field' + data.ItemID).remove();
                                        }
                                        $('#ulComponentsList').find('#liNewComponent').remove();
                                    }
                                }
                                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                            }
                        });
                    }
                }
            }
        });
    }

    var renderTablesFields = function () {
        $('.type-component').on('change', function (e) {
            $('#liNewComponent').find('.table-fields').remove();
            $('#liNewComponent').find('.table-names').remove();
            if ($(this).val() != 2 && $(this).val() != 1 && $(this).val() != 0 && $(this).val() != 3 && $(this).val() != 6) {   //validate componentType to assign table and field
                var builder;
                builder += '<input type="text" class="table-names" placeholder="type table name" />';
                builder += '<input type="text" class="table-fields" placeholder="type table field" />';
                $(builder).insertAfter($('#liNewComponent').find('input:text').first());
                PROFILE.autocompleteTablesFields();
            }
        });
    }

    var ulComponentsHoverEffect = function () {
        var oldStyle = '';
        $('.sortable li div').hover(function () {
            oldStyle = 'height:25px;';//$(this).attr('style').toString();
            if (oldStyle.substr(oldStyle.length - 1, oldStyle.length) != ';') {
                oldStyle += ';';
            }
            $(this).attr('style', oldStyle + 'background-color: rgb(226,222,214); cursor:pointer');
        }, function () {
            $(this).attr('style', oldStyle);
        });
    }

    var autocompleteTablesFields = function () {
        $('.table-names').autocomplete({
            source: getTableNames(), minLength: 4, position: { my: "right top", at: "right bottom" },
            change: function (e, ui) {
                $.ajax({
                    async: false,
                    url: '/Admin/GetFields',
                    type: 'POST',
                    cache: false,
                    data: { tableName: ui.item.value },
                    success: function (data) {
                        $(e.target).siblings('.table-fields').first().autocomplete({
                            source: data,
                            position: { my: "right top", at: "right bottom" },
                            minLength: 0
                        });
                    }
                });
            }
        });
    }

    function deleteComponent(targetLiID) {
        var parentLiID = $('#' + targetLiID).parents('li').first().attr('id');
        $.ajax({
            url: '/Admin/RemoveComponent',
            type: 'POST',
            cache: false,
            data: { componentID: targetLiID.substr(2) },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    $('#ulComponentsList').find('#' + targetLiID).remove();
                    if ($('#' + parentLiID).find('ul:has(li)').length == 0) {
                        $('#' + parentLiID).find('ul').first().remove();
                        $('#' + parentLiID).removeClass('mjs-nestedSortable-branch').removeClass('mjs-nestedSortable-collapsed')
                        .removeClass('mjs-nestedSortable-expanded').addClass('mjs-nestedSortable-leaf');
                        //    $('#' + parentLiID).removeClass('collapsed');
                        //    $('#' + parentLiID).removeClass('expanded');
                    }
                    //SM.selectCheckBoxParents();
                    PROFILE.cascadeCheckboxSelection();
                }
                //SM.orderComponents(parentLiID);
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    //roles
    var makeTblRolesRowsSelectable = function () {
        $('#tblSearchRolesResults tbody tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    var event = $.Event('keydown');
                    event.keyCode = 27;
                    $(document).trigger(event);
                    $(this).parent('tbody').find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/Admin/GetRole',
                        cache: false,
                        type: 'POST',
                        data: { roleID: $(this).attr('id').substr(6) },
                        success: function (data) {
                            $('#RoleInfo_RoleID').val(data.RoleInfo_RoleID);
                            $('#RoleInfo_Role').val(data.RoleInfo_Role);
                            UI.expandFieldset('fdsRolesInfo');
                            UI.scrollTo('fdsRolesInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteRole, [$(e.target).parents('tr').first().attr('id').substr(6)]);
            }
        });
    }

    function deleteRole(roleID) {
        $.ajax({
            url: '/Admin/DeleteRole',
            cache: false,
            type: 'POST',
            data: { roleID: roleID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#trRole' + data.ItemID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#trRole' + data.ItemID).remove();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var saveRoleSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Role Saved') {
                var builder = '<tr id="trRole' + data.ItemID + '"><td>' + $('#RoleInfo_Role').val() + '</td>'
                        + '<td class="tds"><img class="right" src="/Content/themes/base/images/cross.png" ></td></tr>';
                $('#tblSearchRolesResults').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                PROFILE.makeTblRolesRowsSelectable();
                $('#frmRole').clearForm();
            }
            else {
                $('#trRole' + data.ItemID).children('td:nth-child(1)').text($('#RoleInfo_Role').val());
                $('label[for="radRole' + data.ItemID + '"]').text($('#RoleInfo_Role').val());
                if ($('#radRole' + data.ItemID).is(':checked'))
                    $('#divSelectedRole').text($('#RoleInfo_Role').val());
            }
            $.getJSON('/Admin/GetDDLData', { path: 'Roles', id: "0" }, function (data) {
                $('#ProfileInfo_Role').fillSelect(data);
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //sysWorkGroups
    var makeTblWorkGroupsRowsSelectable = function () {
        $('#tblSearchWorkGroupsResults tbody tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    var event = $.Event('keydown');
                    event.keyCode = 27;
                    $(document).trigger(event);
                    $(this).parent('tbody').find('selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/Admin/GetWorkGroup',
                        cache: false,
                        type: 'POST',
                        data: { workgroupID: $(this).attr('id').substr(11) },
                        success: function (data) {
                            $('#WorkGroupInfo_WorkGroupID').val(data.WorkGroupInfo_WorkGroupID);
                            $('#WorkGroupInfo_WorkGroup').val(data.WorkGroupInfo_WorkGroup);
                            PROFILE.getBookingStatusPerWorkGroup($('#WorkGroupInfo_WorkGroupID').val());
                            PROFILE.getLeadTypesPerWorkGroup($('#WorkGroupInfo_WorkGroupID').val());
                            PROFILE.getLeadSourcesPerWorkGroup($('#WorkGroupInfo_WorkGroupID').val());
                            PROFILE.getQualificationRequirementsPerWorkGroup($('#WorkGroupInfo_WorkGroupID').val());
                            PROFILE.getResortFeeTypesPerWorkGroup($('#WorkGroupInfo_WorkGroupID').val());
                            PROFILE.getReservationStatusPerWorkGroup($('#WorkGroupInfo_WorkGroupID').val());
                            PROFILE.getVerificationAgreementsPerWorkGroup($('#WorkGroupInfo_WorkGroupID').val());
                            UI.expandFieldset('fdsWorkGroupsInfo');
                            UI.expandFieldset('fdsWorkGroupRelatedItems');
                            UI.scrollTo('fdsWorkGroupRelatedItems', null);
                        }
                    });
                }
            }
            else
                UI.confirmBox('Do you confirm you want to proceed?', deleteWorkGroup, [$(e.target).parents('tr').first().attr('id').substr(11)]);
        });
    }

    function deleteWorkGroup(workgroupID) {
        $.ajax({
            url: '/Admin/DeleteWorkGroup',
            cache: false,
            type: 'POST',
            data: { workgroupID: workgroupID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0)
                    $('#trWorkgroup' + data.ItemID).remove();
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var saveWorkGroupSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == "WorkGroup Saved") {
                var builder = '<tr id="trWorkgroup' + data.ItemID + '"><td>' + $('#WorkGroupInfo_WorkGroup').val() + '</td>'
                        + '<td class="tds"><img class="right" src="/Content/themes/base/images/cross.png" ></td></tr>';
                $('#tblSearchWorkGroupsResults').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                PROFILE.makeTblWorkGroupsRowsSelectable();
                //code needed if created workgroup would be assigned automatically to the current user
                //besides add ajax call to save assignation
                $.ajax({
                    url: 'Admin/AssignWorkGroupToUser',
                    cache: false,
                    type: 'POST',
                    data: { workgroupID: data.ItemID, roleID: $('#divAvailableRoles').find('input:radio:checked').attr('id').substr(7) },
                    success: function (response) {
                        if (response.ResponseType > 0) {
                            var builder = '<span class="header-settings-menu-item"><input type="radio" id="radWorkGroup' + response.ItemID + '" value="' + response.ItemID + '" name="radWorkGroup"/>&nbsp;<label for="radWorkGroup' + response.ItemID + '">' + $('#WorkGroupInfo_WorkGroup').val() + '</label></span>';
                            $('#divAvailableWorkGroups').append(builder);
                        }
                    }
                });
                PROFILE.makeTblWorkGroupsRowsSelectable();
                $('#trWorkgroup' + data.ItemID).click();
                UI.expandFieldset('fdsWorkGroupRelatedItems');
                UI.scrollTo('fdsWorkGroupRelatedItems', null);
            }
            else {
                $('#trWorkgroup' + data.ItemID).children('td:nth-child(1)').text($('#WorkGroupInfo_WorkGroup').val());
                $('label[for="radWorkGroup' + data.ItemID + '"]').text($('#WorkGroupInfo_WorkGroup').val());
                if ($('#radWorkGroup' + data.ItemID).is(':checked'))
                    $('#divSelectedWorkGroup').text($('#WorkGroupInfo_WorkGroup').val());
            }
            $.getJSON('/Admin/GetDDLData', { path: 'WorkGroups', id: "0" }, function (data) {
                $('#ProfileInfo_WorkGroup').fillSelect(data);
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var checkAllCheckBoxes = function (container) {
        if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
            $('#' + container).find('.chk-parent').attr('checked', 'checked');
        }
        $('#' + container).find('.chk-parent').change(function () {
            $('#' + container).find('.chk-son').attr('checked', this.checked);
        });
        $('#' + container).find('.chk-son').change(function (e) {
            if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
                $('#' + container).find('.chk-parent').attr('checked', 'checked');
            }
            else {
                $('#' + container).find('.chk-parent').attr('checked', false);
            }
        });
    }

    //booking status
    var getAllBookingStatus = function () {
        $.ajax({
            url: '/Admin/GetAllBookingStatus',
            type: 'POST',
            cache: false,
            success: function (data) {
                var builder = '<p><input type="checkbox" class="chk-parent"/>Select All</p><ul id="ulBookingStatus" class="partial-width">';
                $.each(data, function (index, item) {
                    builder += '<li id="liBookingStatus' + item.ItemID + '">'
                        + '<input type="checkbox" class="chk-son" id="chkBookingStatus' + item.ItemID + '" />'
                        //+ '<input type="hidden" id="hdnBookingStatus' + item.ItemID + '" />'
                        + item.ItemName
                    + '<input type="hidden" class="bookingStatus-order" id="txtBookingStatusOrder' + item.ItemID + '" /></li>';
                    //+ '<input type="text" class="bookingStatus-order" id="txtBookingStatusOrder' + item.ItemID + '" /></li>';
                });
                $('#divBookingStatus').contents(':not(span)').remove();
                $('#divBookingStatus').prepend(builder);
                UI.ulsHoverEffect('ulBookingStatus');
                $('#ulBookingStatus').sortable({
                    axis: 'y',
                    stop: function (e, ui) {
                        $('.bookingStatus-order').each(function () { $(this).val(''); });
                        $('#ulBookingStatus li:has(.chk-son:checked)').each(function (evt) {
                            $(this).find('.bookingStatus-order').first().val($(this).index('li:has(.chk-son:checked)') + 1);
                        });
                    },
                    items: 'li:has(.chk-son:checked)'
                }).disableSelection();
                PROFILE.makeUlBookingStatusRowsSelectable();
                PROFILE.checkAllCheckBoxes('divBookingStatus');
            }
        });
    }   //make only checked sortable

    var makeUlBookingStatusRowsSelectable = function () {
        $('#ulBookingStatus li').unbind('click').on('click', function (e) {
            if (!$(e.target).is('input') && !$(e.target).hasClass('selected-row')) {
                $(this).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $('#BookingStatusInfo_BookingStatusID').val($(this).attr('id').substr(15));
                $('#BookingStatusInfo_BookingStatus').val($(this)[0].textContent);
                UI.expandFieldset('fdsBookingStatusInfo');
                UI.scrollTo('fdsBookingStatusInfo', null);
            }
        });
    }   //not able to delete booking status

    var getBookingStatusPerWorkGroup = function (workgroupID) {
        $.ajax({
            url: '/Admin/GetActiveBookingStatusPerWorkGroup',
            type: 'POST',
            cache: false,
            data: { sysWorkGroupID: workgroupID },
            success: function (data) {
                $('#divBookingStatus').find('input:checkbox').removeAttr('checked');
                $('#divBookingStatus').find('input:text').val("");
                $.each(data, function (index, item) {
                    $('#chkBookingStatus' + item.ItemID).attr('checked', true);
                    $('#txtBookingStatusOrder' + item.ItemID).attr('value', item.Item2);
                });
            },
            complete: function () {
                PROFILE.checkAllCheckBoxes('divBookingStatus');
                $('#divBookingStatus').find('li:has(.chk-son:checked)').tsort('.bookingStatus-order', { attr: 'value' });
            }
        });
    }       //order them using tsort()

    var saveBookingStatusSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == "Booking Status Saved") {
                var builder = '<li id="liBookingStatus' + data.ItemID + '">'
                        + '<input type="checkbox" class="chk-son" id="chkBookingStatus' + data.ItemID + '" checked="checked" />'
                        + $('#BookingStatusInfo_BookingStatus').val()
                        + '<input type="text" class="bookingStatus-order" id="txtBookingStatusOrder' + data.ItemID + '" /></li>';
                $('#ulBookingStatus').append(builder);
                $('#ulBookingStatus').sortable('refresh');
            }
            else {
                $('#liBookingStatus' + data.ItemID)[0].childNodes[1].textContent = $('#BookingStatusInfo_BookingStatus').val();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //leadTypes
    var getAllLeadTypes = function () {
        $.ajax({
            url: '/Admin/GetAllLeadTypes',
            type: 'POST',
            cache: false,
            success: function (data) {
                var builder = '<p><input type="checkbox" class="chk-parent"/>Select All</p><ul id="ulLeadTypes" class="partial-width">';
                $.each(data, function (index, item) {
                    builder += '<li id="liLeadType' + item.ItemID + '">'
                        + '<input type="checkbox" id="chkLeadType' + item.ItemID + '" class="chk-son" />'
                        + item.ItemName
                        + '</li>';
                });
                $('#divLeadTypes').contents(':not(span)').remove();
                $('#divLeadTypes').prepend(builder);
                UI.ulsHoverEffect('ulLeadTypes');
                PROFILE.makeUlLeadTypesRowsSelectable();
                PROFILE.checkAllCheckBoxes('divLeadTypes');
            }
        });
    }

    var makeUlLeadTypesRowsSelectable = function () {
        $('#ulLeadTypes li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('input') && !$(e.target).hasClass('selected-row')) {
                $(this).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $('#LeadTypeInfo_LeadTypeID').val($(this).attr('id').substr(10));
                $('#LeadTypeInfo_LeadType').val($(this)[0].textContent);
                UI.expandFieldset('fdsLeadTypesInfo');
                UI.scrollTo('fdsLeadTypesInfo', null);
            }
        });
    }   //not able to delete lead types

    var getLeadTypesPerWorkGroup = function (workgroupID) {
        $.ajax({
            url: '/Admin/GetActiveLeadTypesPerWorkGroup',
            type: 'POST',
            cache: false,
            data: { sysWorkGroupID: workgroupID },
            success: function (data) {
                $('#divLeadTypes').find('input:checkbox').removeAttr('checked');
                $.each(data, function (index, item) {
                    $('#chkLeadType' + item.ItemID).attr('checked', 'checked');
                });
            },
            complete: function () {
                PROFILE.checkAllCheckBoxes('divLeadTypes');
            }
        });
    }

    var saveLeadTypeSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == "Lead Type Saved") {
                var builder = '<li id="liLeadType' + data.ItemID + '">'
                        + '<input type="checkbox" id="chkLeadType' + data.ItemID + '" class="chk-son" checked="checked"/>'
                        + $('#LeadTypeInfo_LeadType').val()
                        + '</li>';
                $('#ulLeadTypes').append(builder);
            }
            else {
                $('#liLeadType' + data.ItemID)[0].childNodes[1].textContent = $('#LeadTypeInfo_LeadType').val();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //leadSources
    var getAllLeadSources = function () {
        $.ajax({
            url: '/Admin/GetAllLeadSources',
            type: 'POST',
            cache: false,
            success: function (data) {
                var builder = '<p><input type="checkbox" class="chk-parent"/>Select All</p><ul id="ulLeadSources" class="partial-width">';
                $.each(data, function (index, item) {
                    builder += '<li id="liLeadSource' + item.ItemID + '">'
                        + '<input type="checkbox" id="chkLeadSource' + item.ItemID + '" class="chk-son" />'
                        + item.ItemName
                        + '</li>';
                });
                $('#divLeadSources').contents(':not(span)').remove();
                $('#divLeadSources').prepend(builder);
                UI.ulsHoverEffect('ulLeadSources');
                PROFILE.makeUlLeadSourcesRowsSelectable(data);
                PROFILE.checkAllCheckBoxes('divLeadSources');
            }
        });
    }

    var makeUlLeadSourcesRowsSelectable = function (data) {
        $('#ulLeadSources li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('input') && !$(e.target).hasClass('selected-row')) {
                $(this).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $('#LeadSourceInfo_LeadSourceID').val($(this).attr('id').substr(12));
                $('#LeadSourceInfo_LeadSource').val($(this)[0].textContent);

                $.each(data, function (index, item) {
                    if (item.ItemID == $('#LeadSourceInfo_LeadSourceID').val())
                        $('#LeadSourceInfo_LeadSourceInitials').val(item.Item2);
                });
                UI.expandFieldset('fdsLeadSourcesInfo');
                UI.scrollTo('fdsLeadSourcesInfo', null);
            }
        });
    }   //not able to delete lead sources

    var getLeadSourcesPerWorkGroup = function (workgroupID) {
        $.ajax({
            url: '/Admin/GetActiveLeadSourcesPerWorkGroup',
            type: 'POST',
            cache: false,
            data: { sysWorkGroupID: workgroupID },
            success: function (data) {
                $('#divLeadSources').find('input:checkbox').removeAttr('checked');
                $.each(data, function (index, item) {
                    $('#chkLeadSource' + item.ItemID).attr('checked', true);
                });
            },
            complete: function () {
                PROFILE.checkAllCheckBoxes('divLeadSources');
            }
        });
    }

    var saveLeadSourceSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Lead Source Saved') {
                var builder = '<li id="liLeadSource' + data.ItemID + '">'
                    + '<input type="checkbox" id="chkLeadSource' + data.ItemID + '" class="chk-son" checked="checked"/>'
                    + $('#LeadSourceInfo_LeadSource').val();
                + '</li>';
                $('#ulLeadSources').append(builder);
            }
            else {
                $('#liLeadSource' + data.ItemID)[0].childNodes[1].textContent = $('#LeadSourceInfo_LeadSource').val();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //qualificationRequirements
    var getAllQualificationRequirements = function () {
        $.ajax({
            url: '/Admin/GetAllQualificationRequirements',
            type: 'POST',
            cache: false,
            success: function (data) {
                var builder = '<p><input type="checkbox" class="chk-parent"/>Select All</p><ul id="ulQualificationRequirements" class="partial-width">';
                $.each(data, function (index, item) {
                    builder += '<li id="liQualificationRequirement' + item.ItemID + '">'
                        + '<input type="checkbox" id="chkQualificationRequirement' + item.ItemID + '" class="chk-son" />'
                        + item.ItemName
                        + '</li>';
                });
                $('#divQualificationRequirements').contents(':not(span)').remove();
                $('#divQualificationRequirements').prepend(builder);
                UI.ulsHoverEffect('ulQualificationRequirements');
                PROFILE.makeUlQualificationRequirementsRowsSelectable();
                PROFILE.checkAllCheckBoxes('divQualificationRequirements');
            }
        });
    }

    var makeUlQualificationRequirementsRowsSelectable = function () {
        $('#ulQualificationRequirements li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('input') && !$(e.target).hasClass('selected-row')) {
                $(this).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $('#QualificationRequirementInfo_QualificationRequirementID').val($(this).attr('id').substr(26));
                $('#QualificationRequirementInfo_QualificationRequirement').val($(this)[0].textContent);
                UI.expandFieldset('fdsQualificationRequirementsInfo');
                UI.scrollTo('fdsQualificationRequirementsInfo', null);
            }
        });
    }

    var getQualificationRequirementsPerWorkGroup = function (workgroupID) {
        $.ajax({
            url: '/Admin/GetActiveQualificationRequirementsPerWorkGroup',
            cache: false,
            type: 'POST',
            data: { sysWorkGroupID: workgroupID },
            success: function (data) {
                $('#divQualificationRequirements').find('input:checkbox').removeAttr('checked');
                $.each(data, function (index, item) {
                    $('#chkQualificationRequirement' + item.ItemID).attr('checked', true);
                });
            },
            complete: function () {
                PROFILE.checkAllCheckBoxes('divQualificationRequirements');
            }
        });
    }

    var saveQualificationRequirementSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseMessage == "Qualification Requirement Saved") {
            var builder = '<li id="liQualificationRequirement' + data.ItemID + '">'
                        + '<input type="checkbox" id="chkQualificationRequirement' + data.ItemID + '" class="chk-son" />'
                        + $('#QualificationRequirementInfo_QualificationRequirement').val()
                        + '</li>';
            $('#ulQualificationRequirements').append(builder);
        }
        else {
            $('#liQualificationRequirement' + data.ItemID)[0].childNodes[1].textContent = $('#QualificationRequirementInfo_QualificationRequirement').val();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //resortFeeTypes
    var getAllResortFeeTypes = function () {
        $.ajax({
            url: '/Admin/GetAllResortFeeTypes',
            cache: false,
            type: 'POST',
            success: function (data) {
                var builder = '<p><input type="checkbox" class="chk-parent"/>Select All</p><ul id="ulResortFeeTypes" class="partial-width">';
                $.each(data, function (index, item) {
                    builder += '<li id="liResortFeeType' + item.ItemID + '">'
                        + '<input type="checkbox" id="chkResortFeeType' + item.ItemID + '" class="chk-son" />'
                        + item.ItemName
                        + '</li>';
                });
                $('#divResortFeeTypes').contents(':not(span)').remove();
                $('#divResortFeeTypes').prepend(builder);
                UI.ulsHoverEffect('ulResortFeeTypes');
                PROFILE.makeUlResortFeeTypesRowsSelectable();
                PROFILE.checkAllCheckBoxes('divResortFeeTypes');
            }
        });
    }

    var makeUlResortFeeTypesRowsSelectable = function () {
        $('#ulResortFeeTypes li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('input') && !$(e.target).hasClass('selected-row')) {
                $(this).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $('#ResortFeeTypeInfo_ResortFeeTypeID').val($(this).attr('id').substr(15));
                $('#ResortFeeTypeInfo_ResortFeeType').val($(this)[0].textContent);
                UI.expandFieldset('fdsResortFeeTypesInfo');
                UI.scrollTo('fdsResortFeeTypesInfo', null);
            }
        });
    }

    var getResortFeeTypesPerWorkGroup = function (workgroupID) {
        $.ajax({
            url: '/Admin/GetActiveResortFeeTypesPerWorkGroup',
            cache: false,
            type: 'POST',
            data: { sysWorkGroupID: workgroupID },
            success: function (data) {
                $('#divResortFeeTypes').find('input:checkbox').removeAttr('checked');
                $.each(data, function (index, item) {
                    $('#chkResortFeeType' + item.ItemID).attr('checked', true);
                });
            },
            complete: function () {
                PROFILE.checkAllCheckBoxes('divResortFeeTypes');
            }
        });
    }

    var saveResortFeeTypeSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseMessage == "Resort Fee Type Saved") {
            var builder = '<li id="liResortFeeType' + data.ItemID + '">'
                        + '<input type="checkbox" id="chkResortFeeType' + data.ItemID + '" class="chk-son" />'
                        + $('#ResortFeeTypeInfo_ResortFeeType').val()
                        + '</li>';
            $('#ulResortFeeTypes').append(builder);
        }
        else {
            $('#liResortFeeType' + data.ItemID)[0].childNodes[1].textContent = $('#ResortFeeTypeInfo_ResortFeeType').val();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //reservationStatus
    var getAllReservationStatus = function () {
        $.ajax({
            url: '/Admin/GetAllReservationStatus',
            cache: false,
            type: 'POST',
            success: function (data) {
                var builder = '<p><input type="checkbox" class="chk-parent"/>Select All</p><ul id="ulReservationStatus" class="partial-width">';
                $.each(data, function (index, item) {
                    builder += '<li id="liReservationStatus' + item.ItemID + '">'
                        + '<input type="checkbox" id="chkReservationStatus' + item.ItemID + '" class="chk-son" />'
                        + item.ItemName
                        + '</li>';
                });
                $('#divReservationStatus').contents(':not(span)').remove();
                $('#divReservationStatus').prepend(builder);
                UI.ulsHoverEffect('ulReservationStatus');
                PROFILE.makeUlReservationStatusRowsSelectable();
                PROFILE.checkAllCheckBoxes('divReservationStatus');
            }
        });
    }

    var makeUlReservationStatusRowsSelectable = function () {
        $('#ulReservationStatus li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('input') && !$(e.target).hasClass('selected-row')) {
                $(this).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $('#ReservationStatusInfo_ReservationStatusID').val($(this).attr('id').substr(19));
                $('#ReservationStatusInfo_ReservationStatus').val($(this)[0].textContent);
                UI.expandFieldset('fdsReservationStatusInfo');
                UI.scrollTo('fdsReservationStatusInfo', null);
            }
        });
    }

    var getReservationStatusPerWorkGroup = function (workgroupID) {
        $.ajax({
            url: '/Admin/GetActiveReservationStatusPerWorkGroup',
            cache: false,
            type: 'POST',
            data: { sysWorkGroupID: workgroupID },
            success: function (data) {
                $('#divReservationStatus').find('input:checkbox').removeAttr('checked');
                $.each(data, function (index, item) {
                    $('#chkReservationStatus' + item.ItemID).attr('checked', true);
                });
            },
            complete: function () {
                PROFILE.checkAllCheckBoxes('divReservationStatus');
            }
        });
    }

    var saveReservationStatusSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseMessage == "Reservation Status Saved") {
            var builder = '<li id="liReservationStatus' + data.ItemID + '">'
                        + '<input type="checkbox" id="chkReservationStatus' + data.ItemID + '" class="chk-son" />'
                        + $('#ReservationStatusInfo_ReservationStatus').val()
                        + '</li>';
            $('#ulReservationStatus').append(builder);
        }
        else {
            $('#liReservationStatus' + data.ItemID)[0].childNodes[1].textContent = $('#ReservationStatusInfo_ReservationStatus').val();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    //verificationAgreements
    var getAllVerificationAgreements = function () {
        $.ajax({
            url: '/Admin/GetAllVerificationAgreements',
            cache: false,
            type: 'POST',
            success: function (data) {
                var builder = '<p><input type="checkbox" class="chk-parent"/>Select All</p><ul id="ulVerificationAgreements" class="partial-width">';
                $.each(data, function (index, item) {
                    builder += '<li id="liVerificationAgreement' + item.ItemID + '">'
                        + '<input type="checkbox" id="chkVerificationAgreement' + item.ItemID + '" class="chk-son" />'
                        + item.ItemName
                        + '</li>';
                });
                $('#divVerificationAgreements').contents(':not(span)').remove();
                $('#divVerificationAgreements').prepend(builder);
                UI.ulsHoverEffect('ulVerificationAgreements');
                PROFILE.makeUlVerificationAgreementsRowsSelectable();
                PROFILE.checkAllCheckBoxes('divVerificationAgreements');
            }
        });
    }

    var makeUlVerificationAgreementsRowsSelectable = function () {
        $('#ulVerificationAgreements li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('input') && !$(e.target).hasClass('selected-row')) {
                $(this).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                $(this).addClass('selected-row secondary');
                $('#VerificationAgreementInfo_VerificationAgreementID').val($(this).attr('id').substr(23));
                $('#VerificationAgreementInfo_VerificationAgreement').val($(this)[0].textContent);
                UI.expandFieldset('fdsVerificationAgreementsInfo');
                UI.scrollTo('fdsVerificationAgreementsInfo', null);
            }
        });
    }

    var getVerificationAgreementsPerWorkGroup = function (workgroupID) {
        $.ajax({
            url: '/Admin/GetActiveVerificationAgreementsPerWorkGroup',
            cache: false,
            type: 'POST',
            data: { sysWorkGroupID: workgroupID },
            success: function (data) {
                $('#divVerificationAgreements').find('input:checkbox').removeAttr('checked');
                $.each(data, function (index, item) {
                    $('#chkVerificationAgreement' + item.ItemID).attr('checked', true);
                });
            },
            complete: function () {
                PROFILE.checkAllCheckBoxes('divVerificationAgreements');
            }
        });
    }

    var saveVerificationAgreementSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseMessage == "Reservation Status Saved") {
            var builder = '<li id="liVerificationAgreement' + data.ItemID + '">'
                        + '<input type="checkbox" id="chkVerificationAgreement' + data.ItemID + '" class="chk-son" />'
                        + $('#VerificationAgreementInfo_VerificationAgreement').val()
                        + '</li>';
            $('#ulVerificationAgreements').append(builder);
        }
        else {
            $('#liVerificationAgreement' + data.ItemID)[0].childNodes[1].textContent = $('#VerificationAgreementInfo_VerificationAgreement').val();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    return {
        init: init,
        //components
        removeRow: removeRow,
        floatButtons: floatButtons,
        getComponents: getComponents,
        getPrivileges: getPrivileges,
        saveNewComponent: saveNewComponent,
        createEditFunction: createEditFunction,
        renderTablesFields: renderTablesFields,
        expandableFunction: expandableFunction,
        orderComponentsByValue: orderComponentsByValue,
        nestedSortableFunction: nestedSortableFunction,
        ulComponentsHoverEffect: ulComponentsHoverEffect,
        autocompleteTablesFields: autocompleteTablesFields,
        autocompleteTablesFields: autocompleteTablesFields,
        cascadeCheckboxSelection: cascadeCheckboxSelection,
        verifyComponentsOrderValue: verifyComponentsOrderValue,
        defineOrderValueByPosition: defineOrderValueByPosition,
        makeComponentsListSelectable: makeComponentsListSelectable,
        ableComponentsChangeDetection: ableComponentsChangeDetection,
        //roles
        deleteRole: deleteRole,
        saveRoleSuccess: saveRoleSuccess,
        makeTblRolesRowsSelectable: makeTblRolesRowsSelectable,
        //sysWorkGroups
        deleteWorkGroup: deleteWorkGroup,
        checkAllCheckBoxes: checkAllCheckBoxes,
        saveWorkGroupSuccess: saveWorkGroupSuccess,
        makeTblWorkGroupsRowsSelectable: makeTblWorkGroupsRowsSelectable,
        //bookingStatus
        getAllBookingStatus: getAllBookingStatus,
        saveBookingStatusSuccess: saveBookingStatusSuccess,
        getBookingStatusPerWorkGroup: getBookingStatusPerWorkGroup,
        makeUlBookingStatusRowsSelectable: makeUlBookingStatusRowsSelectable,
        //leadTypes
        getAllLeadTypes: getAllLeadTypes,
        saveLeadTypeSuccess: saveLeadTypeSuccess,
        getLeadTypesPerWorkGroup: getLeadTypesPerWorkGroup,
        makeUlLeadTypesRowsSelectable: makeUlLeadTypesRowsSelectable,
        //leadSources
        getAllLeadSources: getAllLeadSources,
        saveLeadSourceSuccess: saveLeadSourceSuccess,
        getLeadSourcesPerWorkGroup: getLeadSourcesPerWorkGroup,
        makeUlLeadSourcesRowsSelectable: makeUlLeadSourcesRowsSelectable,
        //qualificationRequirements
        getAllQualificationRequirements: getAllQualificationRequirements,
        saveQualificationRequirementSuccess: saveQualificationRequirementSuccess,
        getQualificationRequirementsPerWorkGroup: getQualificationRequirementsPerWorkGroup,
        makeUlQualificationRequirementsRowsSelectable: makeUlQualificationRequirementsRowsSelectable,
        //resortFeeTypes
        getAllResortFeeTypes: getAllResortFeeTypes,
        makeUlResortFeeTypesRowsSelectable: makeUlResortFeeTypesRowsSelectable,
        getResortFeeTypesPerWorkGroup: getResortFeeTypesPerWorkGroup,
        saveResortFeeTypeSuccess: saveResortFeeTypeSuccess,
        //reservationStatus
        getAllReservationStatus: getAllReservationStatus,
        makeUlReservationStatusRowsSelectable: makeUlReservationStatusRowsSelectable,
        getReservationStatusPerWorkGroup: getReservationStatusPerWorkGroup,
        saveReservationStatusSuccess: saveReservationStatusSuccess,
        //verificationAgreements
        getAllVerificationAgreements: getAllVerificationAgreements,
        makeUlVerificationAgreementsRowsSelectable: makeUlVerificationAgreementsRowsSelectable,
        getVerificationAgreementsPerWorkGroup: getVerificationAgreementsPerWorkGroup,
        saveVerificationAgreementSuccess: saveVerificationAgreementSuccess
    }
}();