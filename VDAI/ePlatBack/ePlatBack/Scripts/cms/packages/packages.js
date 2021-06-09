$(function () {
    $('#PackageDescriptionInfo_Description').ckeditor();
    $('#PackageDescriptionInfo_Includes').ckeditor();
    $('#PackageInfo_Nights').numeric();
    $('#PackageInfo_Availability').numeric();
    $('#PackageInfo_Purchased').numeric();
    $('#PriceInfo_ItemType').val('Packages');
    $('#PictureInfo_ItemType').val('Packages');
    $('#SeoItemInfo_ItemType').val('Packages');
    PACKAGE.init();
});

var PACKAGE = function () {

    var categories;

    var oTable;

    var init = function () {

        UI.updateListsOnTerminalsChange();

        PACKAGE.searchResultsTable($('#tblSearchPackagesResults'));

        $('#PackageInfo_Terminal').on('change', function (e, params) {
            //$.getJSON('/Packages/GetDDLData', { itemID: $('#PackageInfo_Terminal option:selected').val(), itemType: 'terms' }, function (data) {
            //    $('#PackageInfo_TermsBlock').fillSelect(data);
            //    $('#PackageInfo_TermsBlock option[value="' + terms + '"]').attr('selected', true);
            //});
            //$.getJSON('/Packages/GetCatalogsPerTerminal', { terminalID: $('#PackageInfo_Terminal option:selected').val() }, function (data) {
            //    $('#drpCatalogsPerTerminal').fillSelect(data);
            //});
            //$('#ulCategoriesPerCatalog').empty();
            //$('#ulCategoriesPerCatalog').append('<li>No Catalog Selected</li>');
            //PACKAGE.getCatalogsPerTerminal();
            $.getJSON('/Packages/GetDDLData', { itemID: $(this).val(), itemType: 'terms' }, function (data) {
                $('#PackageInfo_TermsBlock').fillSelect(data);
                if (params != undefined) {
                    $('#PackageInfo_TermsBlock option[value="' + params.terms + '"]').attr('selected', true);
                }
            });
        });


        //$('#drpCatalogsPerTerminal').on('change', function () {
        //    $.getJSON('/Packages/GetDDLData', { itemID: $(this).val(), path: 'categories' }, function (data) {
        //        $('#PackageInfo_Category').fillSelect(data);
        //    });
        //    //$('#ulCategoriesPerCatalog').empty();
        //    //$('#ulCategoriesPerCatalog').append('<li>No Catalog Selected</li>');
        //    //PACKAGE.getCategoriesPerCatalog($(this).val(), categories);
        //});

        $('#PackageSettingsInfo_Place').on('change', function (e, roomTypeID) {
            $.getJSON('/Packages/GetDDLData', { itemID: $(this).val(), itemType: 'roomTypes' }, function (data) {
                $('#PackageSettingsInfo_RoomType').fillSelect(data);
                if (roomTypeID != undefined)
                    $('#PackageSettingsInfo_RoomType option[value="' + roomTypeID + '"]').attr('selected', true);
            });
        });

        $('#PackageSettingsInfo_Destination').on('change', function (e, params) {
            $.getJSON('/Packages/GetDDLData', { itemID: $(this).val(), itemType: 'places' }, function (data) {
                $('#PackageSettingsInfo_Place').fillSelect(data);
                var place = params != undefined ? params.place : 0;
                $('#PackageSettingsInfo_Place option[value="' + place + '"]').attr('selected', true);
                if (place != 0)
                    $('#PackageSettingsInfo_Place').trigger('change', params.roomType);
            });
        });

        $('#btnSavePackage').on('click', function (evt) {
            var categories = '';
            $('#PackageInfo_Categories').empty();
            $('#tblCategoriesSelected tbody tr').each(function (index) {
                categories += $(this).children('td:nth-child(1)').attr('id').substr(10) + ',';
            });
            categories = categories.substr(0, categories.length - 1);
            var content = (categories != '') ? categories : '';
            $('#PackageInfo_Categories').val(content);
            $('#frmPackage').validate().settings.ignore = '.ignore-validation';//Forces validation of input:hidden
            $('#frmPackageDescription').validate();
            $('#frmPackage').submit();
        });

        $('#btnSavePackageDescription').unbind('click').on('click', function (e) {
            var valid = true;
            $('#ulPackageDescriptions li').each(function () {
                if ($(this).text() == $('#PackageDescriptionInfo_Culture option:selected').text() && !$(this).hasClass('selected-row'))
                    valid = false;
            });
            if (valid == true) {
                UI.ckeditorUpdateInstances('frmPackageDescription');
                $('#frmPackageDescription').data('validator').settings.ignore = '.ignore-validation';
                //$('#frmPackageDescription').validate({
                //    ignore: '.ignore-validation'
                //});
                $('#frmPackageDescription').submit();
            }
            else
                UI.messageBox(-1, "Description Language already exists", null, null);
        });

        $('#btnAddCategory').on('click', function () {
            var rows = $('#tblCategoriesSelected > tbody > tr').length;
            var builder = '<tr><td id="tdCategory' + $('#PackageInfo_Category option:selected').val() + '">'
                + '<input type="hidden" val="' + $('#PackageInfo_Category option:selected').val() + '"/>'
                + $('#PackageInfo_Category option:selected').text()
                + '</td>'
                + '<td><img class="right" src="/Content/themes/base/images/cross.png"/></td>'
                + '</tr>';
            if ($('#PackageInfo_Category option:selected').val() != 0) {
                var counter = 0;
                for (var i = 0; i <= rows; i++) {
                    if ($('#PackageInfo_Category option:selected').text() != $('#tdCategory' + $('#PackageInfo_Category option:selected').val()).text())
                        counter++;
                }
                if (counter == (rows + 1)) {
                    $('#tblCategoriesSelected').append(builder);
                    $('#tblCategoriesSelected').show();
                }
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                PACKAGE.makeTblCategoriesRowsRemovable();
            }
        });
    }

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchPackagesResults', tableColumns.length - 1);
        PACKAGE.oTable = $('#tblSearchPackagesResults').dataTable();
        $('.paging_two_button').children().on('mousedown', function (e) {
            if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            }
            $(this).on('click', function () { PACKAGE.makeTableRowsSelectable(); });
        });
        $('#tblSearchPackagesResults_length').unbind('change').on('change', function () {
            PACKAGE.makeTableRowsSelectable();
        });
    }

    var savePackageSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Package Saved') {
                var oSettings = PACKAGE.oTable.fnSettings();
                var iAdded = PACKAGE.oTable.fnAddData([
                    $('#PackageInfo_Package').val(),
                    '',
                    $('#PackageInfo_Nights').val(),
                    0,
                    0,
                    0,
                    0,
                    $('#PackageInfo_Relevance').val(),
                    $('input:radio[name="PackageInfo_IsActive"]:checked').val(),
                    '<img src="/Content/themes/base/images/cross.png" id="delP' + data.ItemID + '">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'trPackage' + data.ItemID);
                PACKAGE.oTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                PACKAGE.makeTableRowsSelectable();
                $('#trPackage' + data.ItemID).click();
                UI.expandFieldset('fdsPackageDescriptions');
                UI.scrollTo('fdsPackageDescriptions', null);
            }
            else {
                $('#trPackage' + data.ItemID).children('td:nth-child(1)').text($('#PackageInfo_Package').val());
                $('#trPackage' + data.ItemID).children('td:nth-child(3)').text($('#PackageInfo_Nights').val());
                $('#trPackage' + data.ItemID).children('td:nth-child(4)').text($('#ulPackageDescriptions li').length);
                $('#trPackage' + data.ItemID).children('td:nth-child(5)').text($('#ulPackageSettings li').length);
                $('#trPackage' + data.ItemID).children('td:nth-child(8)').text($('#PackageInfo_Relevance').val());
                $('#trPackage' + data.ItemID).children('td:nth-child(9)').text($('input:radio[name="PackageInfo_IsActive"]:checked').val());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var savePackageDescriptionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Package Description Saved') {
                var builder = '<li id="liPackageDescription' + data.ItemID + '" >'
                + $('#PackageDescriptionInfo_Culture option:selected').text()
                + '<img id="delPackageDescription' + data.ItemID + '" src="/Content/themes/base/images/cross.png" class="right"></li>';
                $('#liNoDescriptions').remove();
                $('#ulPackageDescriptions').append(builder);
                $('#frmPackageDescription').clearForm();
                $('#frmPackageDescription').find('textarea').each(function () {
                    $(this).val('');
                });
                UI.ulsHoverEffect('ulPackageDescriptions');
                PACKAGE.makeUlPackageDescriptionsRowsSelectable();
            }
            else {
                $('#liPackageDescription' + data.ItemID)[0].firstChild.textContent = $('#PackageDescriptionInfo_Culture option:selected').text();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var savePackageSettingSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Package Settings Saved') {
                var builder = '<li id="liPackageSetting' + data.ItemID + '" >'
                + '<div class="li-fix-column"><span>' + $('#PackageSettingsInfo_Place option:selected').text() + '</span></div>'
                + '<div class="li-fix-column"><span>' + $('#PackageSettingsInfo_RoomType option:selected').text() + '</span></div>'
                + '<div class="li-fix-column"><span>' + $('#PackageSettingsInfo_Price option:selected').text() + '</span></div>'
                + '<img id="delPackageSetting' + data.ItemID + '" src="/Content/themes/base/images/cross.png" class="right">'
                + '</li>';
                $('#liNoSettings').remove();
                $('#ulPackageSettings').append(builder);
                if ($('#trPackage' + $('#PackageInfo_PackageID').val()).children('td:nth-child(2)').text().trim() == '')
                    $('#trPackage' + $('#PackageInfo_PackageID').val()).children('td:nth-child(2)').text($('#PackageSettingsInfo_Destination option:selected').text());
                $('#frmPackageSettings').clearForm();
                UI.ulsHoverEffect('ulPackageSettings');
                PACKAGE.makeUlPackageSettingsRowsSelectable();
            }
            else {
                $('#liPackageSetting' + data.ItemID)[0].children[0].childNodes[0].textContent = $('#PackageSettingsInfo_Place option:selected').text();
                $('#liPackageSetting' + data.ItemID)[0].children[1].childNodes[0].textContent = $('#PackageSettingsInfo_RoomType option:selected').text();
                $('#liPackageSetting' + data.ItemID)[0].children[2].childNodes[0].textContent = $('#PackageSettingsInfo_Price option:selected').text();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var makeTableRowsSelectable = function () {
        PICTURE.getItemNames(true);
        $('#tblSearchPackagesResults tbody tr').not('theader').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(document).find('.selected-row').each(function () {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    });
                    $(this).parent().find('.selected-row').removeClass('selected-row primary');
                    $(this).addClass('selected-row primary');
                    $('#PackageInfo_PackageID').val($(this).attr('id').substr(9));
                    var id = $('#PackageInfo_PackageID').val();
                    $('#fdsPackageDescriptions').show();
                    $('#fdsPackageSettings').show();
                    $('#fdsPrices').show();
                    $('#fdsSeoItems').show();
                    $('#fdsPictures').show();
                    $('#tblCategoriesSelected tbody').empty();
                    $('#PriceInfo_ItemID').val(id); //prices related
                    $('#PictureInfo_ItemID').val(id);//pictures related
                    $('#SeoItemInfo_ItemID').val(id);//seoItems related
                    //PICTURE.destination = $(this).children('td:nth-child(2)')[0].textContent;
                    SEO.urlText = $(this).children('td:nth-child(2)')[0].textContent.trim().split(',')[0];
                    PICTURE.getGalleryName($('#PictureInfo_ItemType').val(), $('#PictureInfo_ItemID').val());
                    $.ajax({
                        url: '/Packages/GetPackageInfo',
                        cache: false,
                        type: 'POST',
                        data: { packageID: id },
                        success: function (data) {
                            $('#itemName').text(data.PackageInfo_Package);
                            $('#PackageInfo_Package').val(data.PackageInfo_Package);
                            $('#PackageInfo_PlanType option[value="' + data.PackageInfo_PlanType + '"]').attr('selected', true);
                            $('#PackageInfo_Nights').val(data.PackageInfo_Nights);
                            $('#PackageInfo_Adults').val(data.PackageInfo_Adults);
                            $('#PackageInfo_Children').val(data.PackageInfo_Children);
                            if (data.PackageInfo_IsActive == true) {
                                $('input:radio[name=PackageInfo_IsActive]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name=PackageInfo_IsActive]')[1].checked = true;
                            }
                            $('#PackageInfo_Relevance option[value="' + data.PackageInfo_Relevance + '"]').attr('selected', true);
                            $('#PackageInfo_Availability').val(data.PackageInfo_Availability);
                            $('#PackageInfo_Purchased').val(data.PackageInfo_Purchased);
                            $('#PackageInfo_Terminal option[value="' + data.PackageInfo_Terminal + '"]').attr('selected', true);
                            $('#PackageInfo_Terminal').trigger('change', { params: data.PackageInfo_TermsBlock });
                            $('#PriceInfo_Terminal').val(data.PackageInfo_Terminal);
                            $('#SeoItemInfo_Terminal').val(data.PackageInfo_Terminal);
                            var builderCategories = '';
                            $.each(data.PackageInfo_ListCategories, function (index, item) {
                                builderCategories += '<tr><td id="tdCategory' + item.Key + '">' + item.Value + '</td><td>'
                            + '<img src="/Content/themes/base/images/cross.png" class="right"></td></tr>';
                            });
                            if (builderCategories != '') {
                                $('#tblCategoriesSelected').show();
                                $('#tblCategoriesSelected').append(builderCategories);
                                UI.tablesStripedEffect();
                                UI.tablesHoverEffect();
                                PACKAGE.makeTblCategoriesRowsRemovable();
                            }
                            PACKAGE.updateUlPackageDescriptions(id);
                            PACKAGE.updateUlPackageSettings(id);
                            PRICE.updateTblPrices();
                            SEO.updateTblSeoItems();
                            PICTURE.GetImagesPerItemType($('#PictureInfo_ItemType').val(), $('#PictureInfo_ItemID').val())
                            PICTURE.loadPicturesTree();
                            UI.expandFieldset('fdsPackagesInfo');
                            UI.scrollTo('fdsPackagesInfo', null);
                        }
                    });
                }
            }
            else
                UI.confirmBox('Do you confirm you want to proceed?', deletePackage, [$(e.target).attr('id').substr(4)]);
        });
    }

    var makeTblCategoriesRowsRemovable = function () {
        $('#tblCategoriesSelected tbody tr').not('theader').on('click', function (e) {
            if ($(e.target).is('img')) {
                $(this).remove();
                UI.tablesStripedEffect();
            }
        });
    }

    var updateUlPackageDescriptions = function (id) {
        $('#PackageDescriptionInfo_PackageID').val(id);
        $.ajax({
            url: '/Packages/GetPackageDescriptions',
            cache: false,
            type: 'POST',
            data: { packageID: id },
            success: function (data) {
                $('#ulPackageDescriptions').empty();
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="liPackageDescription' + item.PackageDescriptionInfo_PackageDescriptionID + '">' + $('#PackageDescriptionInfo_Culture option[value="' + item.PackageDescriptionInfo_Culture + '"]').text() + '<img id="delPackageDescription' + item.PackageDescriptionInfo_PackageDescriptionID + '" src="/Content/themes/base/images/cross.png" class="right" /></li>';
                    });
                    $('#ulPackageDescriptions').append(builder);
                    UI.ulsHoverEffect('ulPackageDescriptions');
                    PACKAGE.makeUlPackageDescriptionsRowsSelectable();
                }
                else {
                    builder += '<li id="liNoDescriptions">No Descriptions</li>';
                    $('#ulPackageDescriptions').append(builder);
                }

            }
        });
    }

    var updateUlPackageSettings = function (id) {
        $('#PackageSettingsInfo_PackageID').val(id);
        $.ajax({
            url: '/Packages/GetPackageSettings',
            cache: false,
            type: 'POST',
            data: { packageID: id },
            success: function (data) {
                $('#ulPackageSettings').empty();
                $('#ulSettingsHeader').remove();
                var builder = '';
                if (data.length > 0) {
                    var settingsHeader = '<div id="ulSettingsHeader" class="half-width left-ul-header-alignment">'
                    + '<div class="li-fix-column"><span>Place</span></div>'
                    + '<div class="li-fix-column"><span>Room Type</span></div>'
                    + '<div class="li-fix-column"><span>Price</span></div></div>';
                    $.each(data, function (index, item) {
                        builder += '<li id="liPackageSetting' + item.PackageSettingsInfo_PackageSettingsID + '">'
                            + '<div class="li-fix-column"><span>' + item.PackageSettingsInfo_PlaceName + '</span></div>'
                            + '<div class="li-fix-column"><span>' + item.PackageSettingsInfo_RoomTypeName + '</span></div>'
                            + '<div class="li-fix-column"><span>' + item.PackageSettingsInfo_PriceName + '</span></div>'
                            + '<img id="delPackageSetting' + item.PackageSettingsInfo_PackageSettingsID + '" src="/Content/themes/base/images/cross.png" class="right" />'
                            + '</li>';
                        //builder += '<li id="liPackageSetting' + item.PackageSettingsInfo_PackageSettingsID + '">' + $('#PackageSettingsInfo_Place option[Value="' + item.PackageSettingsInfo_Place + '"]').text() + '<img id="delPackageSetting' + item.PackageSettingsInfo_PackageSettingsID + '" src="/Content/themes/base/images/cross.png" class="right" /></li>';
                    });
                    $(settingsHeader).insertBefore($('#ulPackageSettings'));
                    $('#ulPackageSettings').append(builder);
                    UI.ulsHoverEffect('ulPackageSettings');
                    PACKAGE.makeUlPackageSettingsRowsSelectable();
                }
                else {
                    builder += '<li id="liNoSettings">No Settings</li>';
                    $('#ulPackageSettings').append(builder);
                }
            }
        });
    }

    var makeUlPackageSettingsRowsSelectable = function () {
        $('#ulPackageSettings li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $('#PackageSettingsInfo_PackageSettingsID').val($(this).attr('id').substr(16));
                    $.ajax({
                        url: '/Packages/GetPackageSetting',
                        cache: false,
                        type: 'POST',
                        data: { packageSettingID: $('#PackageSettingsInfo_PackageSettingsID').val() },
                        success: function (data) {
                            $('#PackageSettingsInfo_Destination option[value="' + data.PackageSettingsInfo_Destination + '"]').attr('selected', true);
                            $('#PackageSettingsInfo_Destination').trigger('change', { place: data.PackageSettingsInfo_Place, roomType: data.PackageSettingsInfo_RoomType });
                            $('#PackageSettingsInfo_Price option[value="' + data.PackageSettingsInfo_Price + '"]').attr('selected', true);
                            UI.expandFieldset('fdsPackageSettingsInfo');
                            UI.scrollTo('fdsPackageSettingsInfo', null);
                        }
                    });
                }
            }
            else
                UI.confirmBox('Do you confirm you want to proceed?', deletePackageSettings, [$(e.target).attr('id').substr(17), $('#PackageSettingsInfo_PackageID').val()]);
        });
    }

    var makeUlPackageDescriptionsRowsSelectable = function () {
        $('#ulPackageDescriptions li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(e.target).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $('#PackageDescriptionInfo_PackageDescriptionID').val($(e.target).attr('id').substr(20));
                    $.ajax({
                        url: '/Packages/GetPackageDescription',
                        cache: false,
                        type: 'POST',
                        data: { packageDescriptionID: $('#PackageDescriptionInfo_PackageDescriptionID').val() },
                        success: function (data) {
                            $('#PackageDescriptionInfo_PackageID').val($('#PackageInfo_PackageID').val());
                            $('#PackageDescriptionInfo_Package').val(data.PackageDescriptionInfo_Package);
                            if (data.PackageDescriptionInfo_IsActive == true) {
                                $('input:radio[name="PackageDescriptionInfo_IsActive"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="PackageDescriptionInfo_IsActive"]')[1].checked = true;
                            }
                            $('#PackageDescriptionInfo_Culture option[value="' + data.PackageDescriptionInfo_Culture + '"]').attr('selected', true);
                            $('#PackageDescriptionInfo_Description').val(data.PackageDescriptionInfo_Description);
                            $('#PackageDescriptionInfo_Includes').val(data.PackageDescriptionInfo_Includes);
                            UI.expandFieldset('fdsPackageDescriptionInfo');
                            UI.scrollTo('fdsPackageDescriptionInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deletePackageDescription, [$(e.target).attr('id').substr(21)]);
            }
        });
    }

    function deletePackage(packageID) {
        $.ajax({
            url: '/Packages/DeletePackage',
            cache: false,
            type: 'POST',
            data: { packageID: packageID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#trPackage' + packageID).hasClass('selected-row')) {
                        $(document).find('.selected-row').each(function () {
                            var event = $.Event('keydown');
                            event.keyCode = 27;
                            $(document).trigger(event);
                        });
                    }
                    PACKAGE.oTable.fnDeleteRow($('#trPackage' + packageID)[0]);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deletePackageSettings(packageSettingsID, packageID) {
        $.ajax({
            url: '/Packages/DeletePackageSetting',
            cache: false,
            type: 'POST',
            data: { packageSettingID: packageSettingsID, packageID: packageID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liPackageSetting' + packageSettingsID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liPackageSetting' + data.ItemID).remove();
                    if ($('#ulPackageSettings li').length == 0)
                        $('#ulPackageSettings').append('<li id="liNoSettings">No Settings</li>');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, data.InnerException);
            }
        });
    }

    function deletePackageDescription(packageDescriptionID) {
        $.ajax({
            url: '/Packages/DeletePackageDescription',
            cache: false,
            type: 'POST',
            data: { packageDescriptionID: packageDescriptionID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liPackageDescription' + packageDescriptionID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liPackageDescription' + data.ItemID).remove();
                    if ($('#ulPackageDescriptions li').length == 0)
                        $('#ulPackageDescriptions').append('<li id="liNoDescriptions">No Descriptions</li>');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    return {
        init: init,
        savePackageSuccess: savePackageSuccess,
        searchResultsTable: searchResultsTable,
        //addCategoriesToTable: addCategoriesToTable,
        //getCatalogsPerTerminal: getCatalogsPerTerminal,
        makeTableRowsSelectable: makeTableRowsSelectable,
        updateUlPackageSettings: updateUlPackageSettings,
        //getCategoriesPerCatalog: getCategoriesPerCatalog,
        savePackageSettingSuccess: savePackageSettingSuccess,
        updateUlPackageDescriptions: updateUlPackageDescriptions,
        savePackageDescriptionSuccess: savePackageDescriptionSuccess,
        makeTblCategoriesRowsRemovable: makeTblCategoriesRowsRemovable,
        makeUlPackageSettingsRowsSelectable: makeUlPackageSettingsRowsSelectable,
        makeUlPackageDescriptionsRowsSelectable: makeUlPackageDescriptionsRowsSelectable
    }
}();
/*
****itemID y itemType son campos hidden
ulprices = precios pertenecientes al packageID
****itemType=> activities, packages, transportation
****price clasification=> perpackage, additional night, 
****los campos de url deberian permitir nulos??? se agrega cadena en blanco
*/


//var getCatalogsPerTerminal = function () {
//    $.getJSON('/Packages/GetCatalogsPerTerminal', { terminalID: $('#PackageInfo_Terminal option:selected').val() }, function (data) {
//        $('#drpCatalogsPerTerminal').fillSelect(data);
//    });
//    //$('#drpCatalogsPerTerminal').empty();
//    //var builder = '';
//    //if ($('#PackageInfo_Terminal option:selected').val() != 0) {
//    //    $.ajax({
//    //        url: '/Packages/GetCategoriesPerTerminal',
//    //        type: 'POST',
//    //        cache: false,
//    //        data: { terminalID: $('#PackageInfo_Terminal option:selected').val() },
//    //        success: function (data) {
//    //            if (data.length > 0) {
//    //                builder += '<option value = "0">--Select One--</option>';
//    //                $.each(data, function (index, item) {
//    //                    builder += '<option value="' + item.CatalogID + '">' + item.CatalogName + '</option>';
//    //                });
//    //                categories = data;
//    //            }
//    //            else {
//    //                builder += '<option value = "0">--Select One--</option>';
//    //            }
//    //            $('#drpCatalogsPerTerminal').append(builder);
//    //        }
//    //    });
//    //}
//    //else {
//    //    builder += '<option value = "0">--Select One--</option>';
//    //    $('#drpCatalogsPerTerminal').append(builder);
//    //    $('#ulCategoriesPerCatalog').empty();
//    //    $('#ulCategoriesPerCatalog').append('<li>No Terminal/Catalog Selected</li>');
//    //    $('#divUlCategoriesPerCatalog').children('p:nth-child(2)').remove();
//    //}
//}

//var getCategoriesPerCatalog = function (catalog, data) {

//    $('#divUlCategoriesPerCatalog').children('p:nth-child(2)').remove();
//    if (catalog != 0) {

//        //$('#ulCategoriesPerCatalog').empty();
//        //var builder = '';
//        //var builder1 = '<p><input type="checkbox" class="chk-parent" />Select All</p>';
//        //var catalogIndex;
//        //$.each(data, function (index, item) {
//        //    if (item.CatalogID == catalog) {
//        //        catalogIndex = index;
//        //    }
//        //});
//        //$.each(data[catalogIndex].Categories, function (index, item) {
//        //    builder += '<li id="liCategory' + item.ItemID + '"><input type="checkbox" class="chk-son" id="chkCategory' + item.ItemID + '" />' + item.ItemName + '</li>';
//        //});
//        //if (builder != '') {
//        //    $(builder1).insertBefore($('#ulCategoriesPerCatalog'));
//        //}
//        //else {
//        //    builder += '<li>No Categories in this Catalog</li>';
//        //}
//        //if (data[catalogIndex].Categories.length > 0) {
//        //    builder += '<input id="btnAddCategories" type="button" class="button right" value="Add" />';
//        //}
//        //$('#ulCategoriesPerCatalog').append(builder);
//        PACKAGE.addCategoriesToTable();
//        UI.checkAllCheckBoxes('divUlCategoriesPerCatalog');
//        //UI.ulsHoverEffect('ulCategoriesPerCatalog');
//    }
//}

//var addCategoriesToTable = function () {
//    $('#btnAddCategories').on('click', function () {
//        var builder = '';
//        var categories = new Array();
//        var flag = true;
//        $('#tblCategoriesSelected tbody tr').each(function (index) {
//            categories[index] = $(this).attr('id').substr(10);
//        });
//        $('#ulCategoriesPerCatalog').find('input:checkbox').each(function (e) {
//            if ($(this).is(':checked')) {
//                var id = $(this).attr('id').substr(11);
//                $.each(categories, function (index, item) {
//                    if (categories[index] == id) {
//                        flag = false;
//                    }
//                });
//                if (flag == true) {
//                    builder += '<tr id="trCategory' + id + '"><td>' + $('#liCategory' + id).text() + '</td><td>'
//                    + '<img id="delCategory' + id + '" src="/Content/themes/base/images/cross.png" class="right"></td></tr>';
//                }
//            }
//        });
//        $('#tblCategoriesSelected').show();
//        $('#tblCategoriesSelected tbody').append(builder);
//        UI.tablesStripedEffect('tblCategoriesSelected');
//        UI.tablesHoverEffect();
//        PACKAGE.makeTblCategoriesRowsRemovable();
//    });
//}