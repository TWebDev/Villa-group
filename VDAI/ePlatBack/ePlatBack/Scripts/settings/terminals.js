$(function () {
    TERMINAL.init();
    //CKEDITOR.replace('Description', {
    //    uiColor: '#E1F7FF',
    //    skin: 'kama'
    //});
    $('#CategoryDescriptionInfo_Description').ckeditor();
    //$('#CategoryDescriptionInfo_Policies').ckeditor();
    $('#SeoItemInfo_ItemType').val('Categories');
});

var TERMINAL = function () {

    var _path;

    var previousIndex;

    var oTable;

    var init = function () {
        //disabled when loaded in terminals module
        $('#SeoItemInfo_TerminalItem').attr('disabled', 'disabled');
        $('#SeoItemInfo_TerminalItem').attr('data-keep-value', '');

        TERMINAL.getCatalogs();

        TERMINAL.getDestinationsPerTerminal();

        UI.searchResultsTable('tblSearchTerminalsResults');

        $('input:checkbox[name="TerminalInfo_IsNew"]').on('change', function () {
            if (!$(this).is(':checked')) {
                $('#divCollections').hide();
                $('#divPrefix').show();
                $('#divTerminal').show();
            }
            else {
                if ($('#tblSearchTerminalsResults tbody').find('.selected-row').length == 0) {
                    UI.messageBox(0, 'Select a terminal to copy', null, null);
                }
                $('#divCollections').show();
                $('#divPrefix').hide();
                $('#divTerminal').hide();
            }
            $('#TerminalInfo_IsNew').val($(this).is(':checked'));
        }).change();

        $('#fdsTerminalsInfo legend').children('img').first().on('DOMAttrModified', function (e) {
            if (e.attrName === 'src') {
                $('input:checkbox[name="TerminalInfo_IsNew"]').change();
            }
        });

        $('#btnSaveCategory').on('click', function () {
            $('#CategoryInfo_CatalogID').val($('#CatalogInfo_CatalogID').val());
            $('#frmCategory').submit();
        });

        $('#btnSaveCategoriesPerCatalog').on('click', function () {
            var catalog = new Array($('#CatalogInfo_CatalogID').val());
            var categories = new Array();
            var counter = 0;
            $('#ulCategories li').each(function (index) {
                if ($(this).children('p').children('input:checkbox').is(':checked')) {
                    categories[counter] = $(this).attr('id').substr(10);
                    counter++;
                }
            });
            $.ajax({
                url: '/Terminals/SaveCategoriesPerCatalog',
                cache: false,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify({ catalogID: catalog, categories: categories }),
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveCatalogsPerTerminal').on('click', function () {
            var terminal = new Array($('#CatalogInfo_TerminalID').val());
            var catalogs = new Array();
            var counter = 0;
            $('#ulCatalogs li').each(function () {
                if ($(this).children('input:checkbox').is(':checked')) {
                    catalogs[counter] = $(this).attr('id').substr(9);
                    counter++;
                }
            });
            $.ajax({
                url: '/Terminals/SaveCatalogsPerTerminal',
                cache: false,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify({ terminal: terminal, catalogs: catalogs }),
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveDestinationsPerTerminal').on('click', function () {
            var terminal = new Array($('#CatalogInfo_TerminalID').val());
            var destinations = new Array();
            var counter = 0;
            $('#ulDestinationsPerTerminal li').each(function () {
                if ($(this).children('input:checkbox').is(':checked')) {
                    destinations[counter] = $(this).attr('id').substr(24);
                    counter++;
                }
            });
            $.ajax({
                url: '/Terminals/SaveDestinationsPerTerminal',
                cache: false,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify({ terminal: terminal, destinations: destinations }),
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('#btnSaveCategoryDescription').on('click', function () {
            var flag = true;
            $('#ulCategoryDescriptions li').each(function () {
                if ($(this).text() == $('#CategoryDescriptionInfo_Culture option:selected').text() && !$(this).hasClass('selected-row')) {
                    flag = false;
                }
            });
            if (flag) {
                $('#CategoryDescriptionInfo_CategoryID').val($('#CategoryInfo_CategoryID').val());
                $('#frmCategoryDescription').submit();
            }
            else {
                UI.messageBox(-1, "Description Language already exists", null, null);
            }
        });

        //$('#fdsBannerInfo legend').children('img').first().on('DOMAttrModified', function (e) {
        //    if (e.attrName === 'src') {
        //        if ($('#BannerInfo_BannerID').val() == '') {
        //            $('#pBtnUpdateBanner').hide();
        //            $('#bannerUploader').show();
        //        }
        //        else {
        //            $('#pBtnUpdateBanner').show();
        //            $('#bannerUploader').hide();
        //        }
        //    }
        //});

        $('#BannerInfo_FromDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onClose: function (dateText, inst) {
                if ($('#BannerInfo_ToDate').val() != "") {
                    var fromDate = $('#BannerInfo_FromDate').datepicker('getDate');
                    var toDate = $('#BannerInfo_ToDate').datepicker('getDate');
                    if (fromDate > toDate)
                        $('#BannerInfo_ToDate').datepicker('setDate', fromDate);
                }
                else {
                    $('#BannerInfo_ToDate').val(dateText);
                }
            },
            onSelect: function (selectedDateTime) {
                if ($('#BannerInfo_ToDate').val() != '')
                    $('#BannerInfo_ToDate').datepicker('setDate', $('#BannerInfo_ToDate').datepicker('getDate'));
                $('#BannerInfo_ToDate').datepicker('option', 'minDate', $('#BannerInfo_FromDate').datepicker('getDate'));
            }
        });

        $('#BannerInfo_ToDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onClose: function (dateText, inst) {
                if ($('#BannerInfo_FromDate').val() != "") {
                    var fromDate = $('#BannerInfo_FromDate').datepicker('getDate');
                    var toDate = $('#BannerInfo_ToDate').datepicker('getDate');
                    if (fromDate > toDate)
                        $('#BannerInfo_FromDate').datepicker('setDate', toDate);
                }
                else {
                    $('#BannerInfo_FromDate').val(dateText);
                }
            },
            onSelect: function (selectedDateTime) {
                if ($('#BannerInfo_FromDate').val() != '')
                    $('#BannerInfo_FromDate').datepicker('setDate', $('#BannerInfo_FromDate').datepicker('getDate'));
                $('#BannerInfo_FromDate').datepicker('option', 'maxDate', $('#BannerInfo_ToDate').datepicker('getDate'));
            }
        });

        $('#BannerInfo_FromDate').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#BannerInfo_ToDate').on('keypress', function (e) {
            e.preventDefault();
        });

        $('input:radio[name="BannerInfo_Permanent"]').on('change', function () {
            if ($('input:radio[name="BannerInfo_Permanent"]:checked').val() == "True") {
                $('#divBannerFromDate').hide();
                $('#divBannerToDate').hide();
            }
            else {
                $('#divBannerFromDate').show();
                $('#divBannerToDate').show();
            }
        });

        //$('#btnNewBannerInfo').on('click', function () {
        //    $('#pBtnUpdateBanner').hide();
        //    $('#bannerUploader').show();
        //});

        //$('#bannerUploader').fineUploader({
        //    request: {
        //        endpoint: '/Terminals/UploadBanner',
        //        params: {
        //            path: TERMINAL._path,
        //            bannerGroup: $('#BannerInfo_BannerGroupID').val(),
        //            url: $('#BannerInfo_Url').val(),
        //            permanent: $('input:radio[name="BannerInfo_Permanent"]:checked').val(),
        //            from: $('#BannerInfo_FromDate').val(),
        //            to: 'cosa mas grande'//$('#BannerInfo_ToDate').val()
        //        }
        //    },
        //    multiple: true,
        //    failedUploadTextDisplay: {
        //        mode: 'default'
        //    }
        //});

        $('#bannerUploader').on('complete', function (first, id, fileName, data) {
            $('#bannerUploader').fineUploader('reset');
            var duration = data['response'].Type < 0 ? data['response'].Type : null;
            if (data['success'] != false) {
                $('#liNoBanners').remove();
                var builder = '<li id="liBanner' + data['response'].ObjectID + '">'
                            + '<img class="picture-sizing" src="//eplatfront.villagroup.com' + data['path'].path + '?width=150&height=75&mode=crop">'
                            + '<img src="/Content/themes/base/images/cross.png" class="right delete-banner">'
                            + '</li>';
                $('#ulBanners').append(builder);
                $('#ulBanners').siblings().show();
                //$('#ulBanners').sortable({ axis: 'y' }).disableSelection();
                UI.ulsHoverEffect('ulBanners');
                UI.expandFieldset('fdsBannersInfo');
                UI.scrollTo('fdsBannersInfo', null);
                $('#frmBanner').clearForm();
                TERMINAL.makeUlBannersSelectable();
                TERMINAL.bindSortFunction();
            }
            var exception = data['response'].Exception != null ? data['response'].Exception : '';
            UI.messageBox(data['response'].Type, data['response'].Message + '<br />' + exception, duration, null);
        });

        $('#btnSaveBanner').on('click', function () {
            if ($('#BannerInfo_BannerID').val() != '' && $('#BannerInfo_BannerID').val() != undefined) {
                $('#frmBanner').submit();
            }
            else {
                $('#bannerUploader').fineUploader({
                    request: {
                        endpoint: '/Terminals/UploadBanner',
                        params: {
                            bannerName: $('#BannerInfo_BannerName').val(),
                            path: TERMINAL._path,
                            bannerGroup: $('#BannerInfo_BannerGroupID').val(),
                            url: $('#BannerInfo_Url').val(),
                            permanent: $('input:radio[name="BannerInfo_Permanent"]:checked').val(),
                            from: $('#BannerInfo_FromDate').val(),
                            to: $('#BannerInfo_ToDate').val(),
                            culture: $('#BannerInfo_Culture').val(),
                            terminalID: $('#BannerInfo_TerminalID').val()
                        }
                    },
                    multiple: true,
                    failedUploadTextDisplay: {
                        mode: 'default'
                    }
                });
                $('#bannerUploader').find('input:file').click();
            }
        });
    }

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchTerminalsResults', tableColumns.length - 1);
        TERMINAL.oTable = $('#tblSearchTerminalsResults').dataTable();
        $('.paging_two_button').children().on('mousedown', function (e) {
            if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            }
            $(this).on('click', function () { TERMINAL.makeTableRowsSelectable(); });
        });
        $('#tblSearchTerminalsResults_length').unbind('change').on('change', function () {
            TERMINAL.makeTableRowsSelectable();
        });
    }

    var makeTableRowsSelectable = function () {
        $('#tblSearchTerminalsResults tbody tr').not('theader').click(function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(document).find('.selected-row').each(function () {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    });
                    $(e.currentTarget).parent('tbody').find('.selected-row').removeClass('selected-row primary');
                    $(this).addClass('selected-row primary');
                    $('#fdsTerminalsInfo').find('.view-restricted:not(#fdsCategoriesManagement):not(#fdsCategoryDescriptionsManagement):not(#fdsSeoItems)').each(function () {
                        $(this).show();
                    });
                    $('#TerminalInfo_TerminalID').val($(this).attr('id').substr(10));
                    $('#DomainInfo_TerminalID').val($(this).attr('id').substr(10));
                    $('#CatalogInfo_TerminalID').val($(this).attr('id').substr(10)); //catalogs related
                    $('input:checkbox[name="TerminalInfo_IsNew"]').trigger('change');
                    $.ajax({
                        url: '/Terminals/GetTerminal',
                        cache: false,
                        type: 'POST',
                        data: { terminalID: $('#TerminalInfo_TerminalID').val() },
                        success: function (data) {
                            $('#TerminalInfo_Prefix').val(data.TerminalInfo_Prefix);
                            $('#TerminalInfo_Terminal').val(data.TerminalInfo_Terminal);
                            $('#SeoItemInfo_Terminal').val($('#TerminalInfo_TerminalID').val());
                            $('#SeoItemInfo_TerminalItem option[value="' + $('#SeoItemInfo_Terminal').val() + '"]').attr('selected', true);
                            UI.expandFieldset('fdsTerminalsInfo');
                            UI.scrollTo('fdsTerminalsInfo', null);
                        }
                    });
                    TERMINAL.getDomainsPerTerminal($('#TerminalInfo_TerminalID').val());
                    TERMINAL.getCatalogs($('#TerminalInfo_TerminalID').val());
                    TERMINAL.getDestinationsPerTerminal($('#TerminalInfo_TerminalID').val());
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteTerminal, [$(e.target).attr('id').substr(4)]);
            }
        });
    }

    var saveTerminalSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == "Terminal Saved") {
                var oSettings = TERMINAL.oTable.fnSettings();
                var iAdded = TERMINAL.oTable.fnAddData([
                    $('#TerminalInfo_Prefix').val(),
                    $('#TerminalInfo_Terminal').val(),
                    '<img src="/Content/themes/base/images/cross.png" id="delT' + data.ItemID + '">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'trTerminal' + data.ItemID);
                TERMINAL.oTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                TERMINAL.makeTableRowsSelectable();
                $('#trTerminal' + data.ItemID).click();
                UI.expandFieldset('fdsCatalogsManagement');
                UI.scrollTo('fdsCatalogsManagement', null);
            }
            else {
                $('#trTerminal' + data.ItemID).children('td:nth-child(1)').text($('#TerminalInfo_Prefix').val());
                $('#trTerminal' + data.ItemID).children('td:nth-child(2)').text($('#TerminalInfo_Terminal').val());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveCatalogSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Catalog Saved') {
                var builder = '<li id="liCatalog' + data.ItemID + '" ><input type="checkbox" id="chkCatalog' + data.ItemID + '" class="chk-son" />'
                + $('#CatalogInfo_Catalog').val() + ' - ' + $('#TerminalInfo_Prefix').val() + '<img id="delCatalog' + data.ItemID + '" style="float:right" src="/Content/themes/base/images/cross.png"></li>';
                        //+ $('#CatalogInfo_Catalog').val() + ' - ' + $('#liCatalog' + data.ItemID).text().split('-')[1] + '<img id="delCatalog' + data.ItemID + '" style="float:right" src="/Content/themes/base/images/cross.png"></li>';
                $('#ulCatalogs').append(builder);
                UI.ulsHoverEffect('ulCatalogs');
                TERMINAL.checkAllCheckBoxes('ulCatalogs');
                $('#liCatalog' + data.ItemID).click();
                UI.expandFieldset('fdsCategoriesManagement');
            }
            else {
                $('#liCatalog' + data.ItemID)[0].childNodes[1].textContent = $('#CatalogInfo_Catalog').val() + ' - ' + $('#liCatalog' + data.ItemID).text().split('-')[1];
            }
            TERMINAL.ulCatalogsSelection();
            if ($('#ulCatalogs').prev('p').is(':not(:visible)'))
                $('#ulCatalogs').prev('p').show();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveCategorySuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            TERMINAL.getCategoriesPerCatalog($('#CategoryInfo_CatalogID').val(), true, data.ItemID);
            //if (data.ResponseMessage == 'Category Saved') {
            //var builder = '<li id="liCategory' + data.ItemID + '" >'
            //    + '<p>'
            //    + '<input type="checkbox" id="chkCategory' + data.ItemID + '" class="chk-son" checked="checked"/>'
            //    + $('#CategoryInfo_Category').val() + '<img id="delCategory' + data.ItemID + '" style="float:right" src="/Content/themes/base/images/cross.png">'
            //    + '</p><ul></ul>'
            //    + '</li>';
            //$('#ulCategories').append(builder);
            //UI.ulsHoverEffect('ulCategories');
            //TERMINAL.checkAllCheckBoxes('ulCategories');
            //$('#frmCategory').clearForm();
            //}
            //else {
            //    $('#liCategory' + data.ItemID)[0].childNodes[1].textContent = $('#CategoryInfo_Category').val();
            //TERMINAL.getCategoriesPerCatalog($('#CategoryInfo_CatalogID').val(), true, data.ItemID);
            //}
            //--

            //var parentCategory = $('#CategoryInfo_ParentCategory option:selected').val();
            //$.getJSON('/Terminals/GetDDLCategories', { catalogID: $('#CategoryInfo_CatalogID').val() }, function (data) {
            //    $('#CategoryInfo_ParentCategory').fillSelect(data);
            //    $('#CategoryInfo_ParentCategory option[value="' + parentCategory + '"]').attr('selected', true);
            //});
            //TERMINAL.ulCategoriesSelection();
            //if ($('#ulCategories').prev('p').is(':not(:visible)')) {
            //    $('#ulCategories').prev('p').show();
            //    $('#btnSaveCategoriesPerCatalog').show();//only visible when there are categories to add
            //}
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveCategoryDescriptionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Category Description Saved') {
                var builder = '<li id="liCategoryDescription' + data.ItemID + '" >'
                        + $('#CategoryDescriptionInfo_Culture option:selected').text() + '<img id="delCatDescription' + data.ItemID + '" style="float:right" src="/Content/themes/base/images/cross.png"></li>';
                $('#liNoDescriptions').remove();
                $('#ulCategoryDescriptions').append(builder);
                $('#frmCategoryDescription').clearForm();
                $('#CategoryDescriptionInfo_Description').val('');
                $('#CategoryDescriptionInfo_Description').ckeditor();
                //$('#CategoryDescriptionInfo_Policies').val('');
                //$('#CategoryDescriptionInfo_Policies').ckeditor();
            }
            else {
                $('#liCategoryDescription' + data.ItemID)[0].firstChild.textContent = $('#CategoryDescriptionInfo_Culture option:selected').text();
            }
            UI.ulsHoverEffect('ulCategoryDescriptions');
            TERMINAL.ulCategoryDescriptionsSelection();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveDomainSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Terminal Domain Saved') {
                var builder = '<tr id="trDomain' + data.ItemID + '"><td>' + $('#DomainInfo_Domain').val() + '</td>'
                + '<td>' + $('#DomainInfo_Culture option:selected').val() + '</td>'
                + '<td>' + $('#DomainInfo_DefaultPage').val() + '</td>'
                + '<td>' + $('#DomainInfo_DefaultMasterPage').val()
                + '<img id="delDomain' + data.ItemID + '" src="/Content/themes/base/images/cross.png" style="float:right"></td></tr>';
                $('#tblDomains tbody').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                $('#frmDomain').clearForm();
                TERMINAL.tblDomainsSelection();
            }
            else {
                $('#trDomain' + data.ItemID).children('td:nth-child(1)').text($('#DomainInfo_Domain').val());
                $('#trDomain' + data.ItemID).children('td:nth-child(2)').text($('#DomainInfo_Culture option:selected').val());
                $('#trDomain' + data.ItemID).children('td:nth-child(3)').text($('#DomainInfo_DefaultPage').val());
                $('#trDomain' + data.ItemID).children('td:nth-child(4)')[0].firstChild.textContent = $('#DomainInfo_DefaultMasterPage').val();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var getCatalogs = function (terminalID) {
        var terminal = terminalID != undefined ? terminalID : 0;
        $.ajax({
            url: '/Terminals/GetCatalogs',
            type: 'POST',
            cache: false,
            data: { terminalID: terminal },
            success: function (data) {
                if (terminal == 0) {
                    $('#ulCatalogs').empty();
                    var builder = '';
                    $.each(data, function (index, item) {
                        builder += '<li id="liCatalog' + item.Key + '" class="skip-one-level"><input type="checkbox" id="chkCatalog' + item.Key + '" class="chk-son" />'
                    + item.Value + '<img id="delCatalog' + item.Key + '" style="float:right" src="/Content/themes/base/images/cross.png"></li>';
                    });
                }
                else {
                    $('#ulCatalogs').find('input:checkbox').removeAttr('checked');
                    $.each(data, function (index, item) {
                        $('#chkCatalog' + item.Key).attr('checked', 'checked');
                    });
                }
                $('#ulCatalogs').append(builder);
                TERMINAL.ulCatalogsSelection();
                UI.ulsHoverEffect('ulCatalogs');
                TERMINAL.checkAllCheckBoxes('ulCatalogs');
            }
        });
    }

    var getDestinationsPerTerminal = function (terminalID) {
        var terminal = terminalID != undefined ? terminalID : 0;
        $.ajax({
            url: '/Terminals/GetDestinationsPerTerminal',
            type: 'POST',
            cache: false,
            data: { terminalID: terminal },
            success: function (data) {
                if (terminal == 0) {
                    $('#ulDestinationsPerTerminal').empty();
                    var builder = '';
                    $.each(data, function (index, item) {
                        builder += '<li id="liDestinationPerTerminal' + item.Key + '" class="skip-one-level"><input type="checkbox" id="chkDestination' + item.Key + '" class="chk-son"/>'
                        + item.Value + '</li>';
                    });
                }
                else {
                    $('#ulDestinationsPerTerminal').find('input:checkbox').removeAttr('checked');
                    $.each(data, function (index, item) {
                        $('#chkDestination' + item.Key).attr('checked', 'checked');
                    });
                }
                $('#ulDestinationsPerTerminal').append(builder);
                TERMINAL.checkAllCheckBoxes('ulDestinationsPerTerminal');
            }
        });
    }

    var getCategoriesPerCatalog = function (catalogID, updated, categoryID) {
        $.ajax({
            url: '/Terminals/GetCategoriesPerCatalog',
            type: 'POST',
            cache: false,
            data: { catalogID: catalogID },
            success: function (data) {
                $('#ulCategories').prev('p').hide();
                $('#btnSaveCategoriesPerCatalog').hide();
                $('#ulCategories').empty();
                var builder = '';
                var flag = false;
                $.each(data, function (index, item) {
                    if (item.CategoryInfo_ParentCategory == 0) {
                        flag = false;
                        builder += '<li id="liCategory' + item.CategoryInfo_CategoryID + '" >'
                            + '<p style="margin:0 !important">'
                        + '<input type="checkbox" id="chkCategory' + item.CategoryInfo_CategoryID + '" class="chk-son"';
                        if (item.CategoryInfo_IsActive == true) {
                            builder += 'checked="checked" />';
                        }
                        else {
                            builder += '/>';
                        }
                        builder += item.CategoryInfo_Category + '<img id="delCategory' + item.CategoryInfo_CategoryID
                            + '" style="float:right" src="/Content/themes/base/images/cross.png">';
                        builder += '</p>';
                        builder += '<ul style="list-style:none">';
                        recursive(item.CategoryInfo_CategoryID);
                    }
                });

                function recursive(categoryID) {
                    $.each(data, function (index2, item2) {
                        if (categoryID == item2.CategoryInfo_ParentCategory) {
                            flag = true;
                        }
                    });
                    if (flag == false) {
                        builder += '</ul></li>';
                    }
                    else {
                        $.each(data, function (index2, item2) {
                            if (item2.CategoryInfo_ParentCategory == categoryID) {
                                builder += '<li id="liCategory' + item2.CategoryInfo_CategoryID + '" >'
                                    + '<p style="margin:0 !important">'
                + '<input type="checkbox" id="chkCategory' + item2.CategoryInfo_CategoryID + '" class="chk-son"';
                                if (item2.CategoryInfo_IsActive == true) {
                                    builder += 'checked="checked" />';
                                }
                                else {
                                    builder += '/>';
                                }
                                builder += item2.CategoryInfo_Category + '<img id="delCategory' + item2.CategoryInfo_CategoryID
                                    + '" style="float:right" src="/Content/themes/base/images/cross.png">';
                                builder += '</p>';
                                builder += '<ul style="list-style:none">';
                                recursive(item2.CategoryInfo_CategoryID);
                            }
                        });
                        builder += '</ul></li>';
                    }
                    return builder;
                }
                $('#ulCategories').append(builder);
                if (builder != '') {
                    $('#ulCategories').prev('p').children('input:checkbox').checked = false;
                    $('#ulCategories').prev('p').show();
                    $('#btnSaveCategoriesPerCatalog').show();//only visible when there are categories to add
                }
                TERMINAL.ulCategoriesSelection(data);
                UI.ulsHoverEffect('ulCategories');
                TERMINAL.checkAllCheckBoxes('ulCategories');
                $.getJSON('/Terminals/GetDDLCategories', { catalogID: catalogID }, function (data) {
                    $('#CategoryInfo_ParentCategory').fillSelect(data);
                    if (updated == true) {
                        $('#liCategory' + categoryID).children('p').click();
                    }
                });
                //this trigger a.nodename is undefined error
                //if (updated == true) {
                //    $('#liCategory' + categoryID).children('p').click();
                //}
            }
        });
    }

    var getDescriptionsPerCategory = function (categoryID) {
        $.ajax({
            url: '/Terminals/GetDescriptionsPerCategory',
            type: 'POST',
            cache: false,
            data: { categoryID: categoryID },
            success: function (data) {
                $('#ulCategoryDescriptions').empty();
                if (data[0] != undefined) {
                    var builder = '';
                    $.each(data, function (index, item) {
                        builder += '<li id="liCategoryDescription' + item.CategoryDescriptionInfo_CategoryDescriptionID + '" >';
                        builder += $('#CategoryDescriptionInfo_Culture option[value="' + item.CategoryDescriptionInfo_Culture + '"]').text()
                            + '<img id="delCatDescription' + item.CategoryDescriptionInfo_CategoryDescriptionID + '" src="/Content/themes/base/images/cross.png" style="float:right" /></li>';
                    });
                    $('#ulCategoryDescriptions').append(builder);
                    TERMINAL.ulCategoryDescriptionsSelection(data);
                    UI.ulsHoverEffect('ulCategoryDescriptions');
                }
                else {
                    var builder = '<li id="liNoDescriptions">No descriptions added to this category</li>';
                    $('#ulCategoryDescriptions').append(builder);
                }
            }
        });
    }

    var getDomainsPerTerminal = function (terminalID) {
        $.ajax({
            url: '/Terminals/GetDomainsPerTerminal',
            type: 'POST',
            cache: false,
            data: { terminalID: terminalID },
            success: function (data) {
                var builder = '';
                if (data != '') {
                    //var builder = '';
                    $.each(data, function (index, item) {
                        builder += '<tr class="skip-one-level" id="trDomain' + item.DomainInfo_DomainID + '"><td>' + item.DomainInfo_Domain + '</td><td>' + item.DomainInfo_Culture + '</td><td>'
                            + item.DomainInfo_DefaultPage + '</td><td>'
                            + item.DomainInfo_DefaultMasterPage
                            + '<img id="delDomain' + item.DomainInfo_DomainID + '" src="/Content/themes/base/images/cross.png" style="float:right"></td></tr>';
                    });
                    $('#tblDomains tbody').empty();
                    $('#tblDomains tbody').append(builder);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    TERMINAL.tblDomainsSelection(data);
                }
                else {
                    $('#tblDomains tbody').empty();
                    builder += '<tr id="trNotDomainsAdvice"><td colspan="3">No Domains Assigned to this Terminal</td></tr>';
                    $('#tblDomains tbody').append(builder);
                }
            }
        });
    }

    var ulCatalogsSelection = function () {
        $('#ulCatalogs').unbind('click').bind('click', function (e) {
            if ($(e.target).is('li')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(this).find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $('#CatalogInfo_CatalogID').val($(e.target).attr('id').substr(9));
                    $('#CategoryInfo_CatalogID').val($(e.target).attr('id').substr(9)); //categories related
                    $('#CatalogInfo_Catalog').val($(e.target).text().split('-')[0].trim());
                    TERMINAL.getCategoriesPerCatalog($('#CatalogInfo_CatalogID').val(), false, 0);
                    UI.expandFieldset('fdsCatalogsInfo');
                    UI.scrollTo('fdsCatalogsInfo', null);
                    $('#fdsCategoriesManagement').show();
                    UI.expandFieldset('fdsCategoriesManagement');
                }
            }
            else {
                if ($(e.target).is('img')) {
                    UI.confirmBox('Do you confirm you want to proceed?', deleteCatalog, [$(e.target).attr('id').substr(10)]);
                }
            }
        });
    }

    var ulCategoriesSelection = function (data) {
        $('#ulCategories').unbind('click').on('click', function (e) {
            if ($(e.target).is('p')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(this).find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).parent('li').find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $('#CategoryInfo_CategoryID').val($(e.target).parent('li').attr('id').substr(10));
                    $('#SeoItemInfo_ItemID').val($('#CategoryInfo_CategoryID').val());//seoItems related
                    var textUrl = '';
                    $(e.target).parents('li').each(function () {
                        textUrl += $(this).children('p')[0].textContent + ',';
                    });
                    SEO.urlText = textUrl.substring(0, textUrl.length - 1);
                    $.ajax({
                        url: '/Terminals/GetCategory',
                        cache: false,
                        type: 'POST',
                        data: { categoryID: $(e.target).parent('li').attr('id').substr(10) },
                        success: function (data) {
                            $('#CategoryInfo_CategoryID').val($(e.target).parent('li').attr('id').substr(10));
                            $('#CategoryInfo_Category').val(data.CategoryInfo_Category);
                            $('#CategoryDescriptionInfo_CategoryID').val($('#CategoryInfo_CategoryID').val());  //categoryDescription related
                            $('#CategoryInfo_ParentCategory option[value="' + data.CategoryInfo_ParentCategory + '"]').attr('selected', true);
                            if (data.CategoryInfo_ShowOnWebsite) {
                                $('input:radio[name="CategoryInfo_ShowOnWebsite"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="CategoryInfo_ShowOnWebsite"]')[1].checked = true;
                            }
                            SEO.updateTblSeoItems();
                            TERMINAL.getDescriptionsPerCategory($('#CategoryInfo_CategoryID').val());
                        }
                    });
                    UI.expandFieldset('fdsCategoriesInfo');
                    UI.scrollTo('fdsCategoriesInfo', null);
                    $('#fdsCategoryDescriptionsManagement').show();
                    $('#fdsSeoItems').show();
                    UI.expandFieldset('fdsCategoryDescriptionsManagement');
                    $('#fdsCategoryDescriptionsInfo>div').hide();
                }
            }
            else {
                if ($(e.target).is('img')) {
                    UI.confirmBox('Do you confirm you want to proceed?', deleteCategory, [$(e.target).attr('id').substr(11)]);
                }
            }
        });
    }

    var ulCategoryDescriptionsSelection = function (data) {
        $('#ulCategoryDescriptions').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img') && $(e.target).attr('id') != 'liNoDescriptions') {
                if (!$(e.target).hasClass('selected-row')) {
                    $(this).find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $('#CategoryDescriptionInfo_CategoryDescriptionID').val($(e.target).attr('id').substr(21));
                    var categoryDescriptionID = $(e.target).attr('id').substr(21);
                    $.ajax({
                        url: '/Terminals/GetCategoryDescription',
                        cache: false,
                        type: 'POST',
                        data: { categoryDescriptionID: $(e.target).attr('id').substr(21) },
                        success: function (data) {
                            $('#CategoryDescriptionInfo_Culture option[value="' + data.CategoryDescriptionInfo_Culture + '"]').attr('selected', true);
                            $('#CategoryDescriptionInfo_Category').val(data.CategoryDescriptionInfo_Category);
                            if (data.CategoryDescriptionInfo_ShowOnWebsite) {
                                $('input:radio[name="CategoryDescriptionInfo_ShowOnWebsite"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="CategoryDescriptionInfo_ShowOnWebsite"]')[1].checked = true;
                            }
                            $('#CategoryDescriptionInfo_ImgIcon').val(data.CategoryDescriptionInfo_ImgIcon);
                            $('#CategoryDescriptionInfo_ImgHeader').val(data.CategoryDescriptionInfo_ImgHeader);
                            $('#CategoryDescriptionInfo_Description').val(data.CategoryDescriptionInfo_Description);
                            //$('#CategoryDescriptionInfo_Policies').val(data.CategoryDescriptionInfo_Policies);
                            UI.expandFieldset('fdsCategoryDescriptionsInfo');
                            UI.scrollTo('fdsCategoryDescriptionsInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteCategoryDescription, [$(e.target).attr('id').substr(17)])
            }
        });
    }

    var tblDomainsSelection = function (data) {
        $('#tblDomains tbody tr').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $('#DomainInfo_DomainID').val($(this).attr('id').substr(8));
                    $.ajax({
                        url: '/Terminals/GetDomain',
                        cache: false,
                        type: 'POST',
                        data: { terminalDomainID: $('#DomainInfo_DomainID').val() },
                        success: function (data) {
                            $('#DomainInfo_Domain').val(data.DomainInfo_Domain);
                            $('#DomainInfo_DefaultPage').val(data.DomainInfo_DefaultPage);
                            $('#DomainInfo_DefaultMasterPage').val(data.DomainInfo_DefaultMasterPage);
                            $('#DomainInfo_MasterPageHeader').val(data.DomainInfo_MasterPageHeader);
                            $('#DomainInfo_MasterPageFooter').val(data.DomainInfo_MasterPageFooter);
                            $('#DomainInfo_ScriptsHeader').val(data.DomainInfo_ScriptsHeader);
                            $('#DomainInfo_ScriptsAfterBody').val(data.DomainInfo_ScriptsAfterBody);
                            $('#DomainInfo_ScriptsFooter').val(data.DomainInfo_ScriptsFooter);
                            $('#DomainInfo_Logo').val(data.DomainInfo_Logo);
                            $('#DomainInfo_PhoneUS').val(data.DomainInfo_PhoneUS);
                            $('#DomainInfo_PhoneMX').val(data.DomainInfo_PhoneMX);
                            $('#DomainInfo_PhoneUSMobile').val(data.DomainInfo_PhoneUSMobile);
                            $('#DomainInfo_PhoneMXMobile').val(data.DomainInfo_PhoneMXMobile);
                            $('#DomainInfo_PhoneAlt1').val(data.DomainInfo_PhoneAlt1);
                            $('#DomainInfo_PhoneAlt1Mobile').val(data.DomainInfo_PhoneAlt1Mobile);
                            $('#DomainInfo_PhoneAlt2').val(data.DomainInfo_PhoneAlt2);
                            $('#DomainInfo_PhoneAlt2Mobile').val(data.DomainInfo_PhoneAlt2Mobile);
                            $('#DomainInfo_Culture option[value="' + data.DomainInfo_Culture + '"]').attr('selected', true);
                            UI.expandFieldset('fdsDomainsInfo');
                            UI.scrollTo('fdsDomainsInfo', null);
                            $('#BannerGroupInfo_TerminalDomainID').val($('#DomainInfo_DomainID').val());
                            TERMINAL.getBannerGroupsPerDomain($('#BannerGroupInfo_TerminalDomainID').val());
                            $('#fdsBannersGroups').show();
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteDomain, [$(e.target).attr('id').substr(9)]);
            }
        });
    }

    var checkAllCheckBoxes = function (container) {
        if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
            $('#' + container).prev('p').find('.chk-parent').attr('checked', 'checked');
        }
        $('#' + container).prev('p').find('.chk-parent').change(function () {
            $('#' + container).find('.chk-son').attr('checked', this.checked);
        });
        $('#' + container).find('.chk-son').change(function (e) {
            if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
                $('#' + container).prev('p').find('.chk-parent').attr('checked', 'checked');
            }
            else {
                $('#' + container).prev('p').find('.chk-parent').attr('checked', false);
            }
        });
    }

    function deleteTerminal(terminalID) {
        $.ajax({
            url: '/Terminals/DeleteTerminal',
            cache: false,
            type: 'POST',
            data: { terminalID: terminalID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#trTerminal' + terminalID).hasClass('selected-row')) {
                        $(document).find('.selected-row').each(function () {
                            var event = $.Event('keydown');
                            event.keyCode = 27;
                            $(document).trigger(event);
                        });
                    }
                    $('#trTerminal' + data.ItemID).remove();
                    UI.tablesStripedEffect();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
        return true;
    }

    function deleteCatalog(catalogID) {
        $.ajax({
            url: '/Terminals/DeleteCatalog',
            cache: false,
            type: 'POST',
            data: { catalogID: catalogID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liCatalog' + catalogID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liCatalog' + data.ItemID).remove();
                    if ($('#ulCatalogs li').length == 0) {
                        $('#ulCatalogs').prev('p').hide();
                    }
                    //UI.collapseFieldset('fdsCatalogsInfo');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteCategory(categoryID) {
        $.ajax({
            url: '/Terminals/DeleteCategory',
            cache: false,
            type: 'POST',
            data: { categoryID: categoryID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liCategory' + categoryID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liCategory' + data.ItemID).remove();
                    $('#CategoryInfo_ParentCategory option[value="' + categoryID + '"]').remove();
                    if ($('#ulCategories li').length == 0) {
                        $('#ulCategories').prev('p').hide();
                        $('#btnSaveCategoriesPerCatalog').hide();
                    }
                    UI.collapseFieldset('fdsCategoriesInfo');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteCategoryDescription(categoryDescriptionID) {
        $.ajax({
            url: '/Terminals/DeleteCategoryDescription',
            cache: false,
            type: 'POST',
            data: { categoryDescriptionID: categoryDescriptionID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liCategoryDescription' + categoryDescriptionID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liCategoryDescription' + data.ItemID).remove();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, data.InnerException);
            }
        });
    }

    function deleteDomain(domainID) {
        $.ajax({
            url: '/Terminals/DeleteDomain',
            cache: false,
            type: 'POST',
            data: { domainID: domainID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#trDomain' + domainID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#trDomain' + data.ItemID).remove();
                    UI.tablesStripedEffect();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }
    /////////////////////////////////////////////////////////////////////////////////////////

    var loadBannersTree = function () {
        //gets destination and name of current item to define the node that will be initially selected
        TERMINAL.getTree();
    }

    var getTree = function () {
        $.getJSON('/Terminals/GetTree', { itemID: $('#BannerGroupInfo_BannerGroupID').val() }, function (data) {
            var jsonData = JSON.parse(data);
            $('#divBannersTree').empty();
            var node = 'b' + $('#BannerGroupInfo_BannerGroupID').val() + '_b' + $('#BannerGroupInfo_BannerGroupID').val();
            $('#divBannersTree').jstree({
                'json_data': {
                    'data': jsonData
                },
                'themes': {
                    url: '/Content/themes/base/classic/style.css'
                },
                'ui': {
                    initially_select: [node],
                    selected_parent_close: false
                },
                'plugins': ['themes', 'json_data', 'ui']
            });
            TERMINAL.bindClickEventToTree(node);
        });
    }

    var bindClickEventToTree = function (node) {
        $('#divBannersTree').unbind('select_node.jstree').bind('select_node.jstree', function (e, data) {
            var builder = '';
            var _builder = '';
            //if (data.args[0] == '#' + node || $(data.args[0]).parent('li').attr('id') == node) {
            //$('#' + node).find('a:first').addClass('selected-row secondary');
            $('#divBannersTree').find('.selected-row').removeClass('selected-row secondary');
            $(data.rslt.obj[0]).find('a:first').addClass('selected-row secondary');
                $('#divBannersTree').find('.selected-row').parents('li').each(function () {
                    var start = $(this).attr('id').indexOf('_') + 1;
                    builder += $(this).attr('id').substr(start, $(this).attr('id').length - start) + '\\';
                });
                builder = builder.split('\\');
                for (var i = (builder.length - 1) ; i >= 0; i--) {
                    _builder += builder[i] + '\\';
                }
                TERMINAL._path = _builder;
                $.ajax({
                    async: false,
                    url: '/Terminals/GetFiles',
                    cache: false,
                    type: 'POST',
                    data: { directory: _builder },
                    success: function (data) {
                        $('#ulBanners').empty();
                        var builder = '';
                        if (data.length > 0) {
                            $.each(data, function (index, item) {
                                builder += '<li id="liBanner' + item.BannerInfo_BannerID + '">'
                                    + '<img class="picture-sizing" src="//eplatfront.villagroup.com' + item.BannerInfo_Path + '?width=150&height=75&mode=crop">'
                                    //+ item.BannerInfo_BannerName
                                    //+ '<span><img class="picture-sizing" src="//eplatfront.villagroup.com' + item.BannerInfo_Path + item.BannerInfo_BannerName + '?width=100&height=100&mode=crop"></span>'
                                    + '<img src="/Content/themes/base/images/cross.png" class="right delete-banner">'
                                    + '</li>';
                            });
                        }
                        else {
                            builder = '<li id="liNoBanners">No Banners In This Group</li>';
                        }
                        $('#ulBanners').append(builder);
                        $('#ulBanners').siblings().show();

                        //$('#ulBanners').sortable({ axis: 'y'}).disableSelection();
                        UI.ulsHoverEffect('ulBanners');
                        $('#fdsBannersInfo').show();
                        UI.expandFieldset('fdsBannersInfo');
                        UI.scrollTo('fdsBannersInfo', null);
                        TERMINAL.bindSortFunction();
                        TERMINAL.makeUlBannersSelectable();
                    }
                });
            //}
        });
    }

    var getBannersOfGroup = function(bannerGroupID, terminalDomainID){
        $.ajax({
            async: false,
            url: '/Terminals/GetFiles',
            cache: false,
            type: 'POST',
            data: { directory: '\\b' + bannerGroupID + '\\', terminalDomainID: terminalDomainID },
            success: function (data) {
                $('#ulBanners').empty();
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="liBanner' + item.BannerInfo_BannerID + '">'
                            + '<img class="picture-sizing" src="//eplatfront.villagroup.com' + item.BannerInfo_Path + '?width=150&height=75&mode=crop">'
                            //+ item.BannerInfo_BannerName
                            //+ '<span><img class="picture-sizing" src="//eplatfront.villagroup.com' + item.BannerInfo_Path + item.BannerInfo_BannerName + '?width=100&height=100&mode=crop"></span>'
                            + '<img src="/Content/themes/base/images/cross.png" class="right delete-banner">'
                            + '</li>';
                    });
                }
                else {
                    builder = '<li id="liNoBanners">No Banners In This Group</li>';
                }
                $('#ulBanners').append(builder);
                $('#ulBanners').siblings().show();

                //$('#ulBanners').sortable({ axis: 'y'}).disableSelection();
                UI.ulsHoverEffect('ulBanners');
                $('#fdsBannersInfo').show();
                UI.expandFieldset('fdsBannersInfo');
                UI.scrollTo('fdsBannersInfo', null);
                TERMINAL.bindSortFunction();
                TERMINAL.makeUlBannersSelectable();
            }
        });
    }

    var getBannerGroupsPerDomain = function (terminalDomainID) {
        $.ajax({
            url: '/Terminals/GetBannerGroupsPerDomain',
            cache: false,
            type: 'POST',
            data: { terminalDomainID: terminalDomainID },
            success: function (data) {
                var builder = '';
                if (data != '') {
                    $.each(data, function (index, item) {
                        //builder += '<tr id="trBannerGroup' + item.BannerGroupInfo_BannerGroupID + '" class="skip-one-level">'
                        builder += '<tr id="trBannerGroup' + item.BannerGroupInfo_BannerGroupID + '" class="">'
                            + '<td>' + item.BannerGroupInfo_BannerGroup + '</td>'
                            + '<td>' + item.BannerGroupInfo_Width + '</td>'
                            + '<td>' + item.BannerGroupInfo_Height
                            + '<img class="delete-banner-group right" src="/Content/themes/base/images/cross.png">'
                            + '</td>'
                            + '</tr>';
                    });
                    $('#tblBannersGroups tbody').empty();
                    $('#tblBannersGroups tbody').append(builder);
                    UI.tablesHoverEffect();
                    UI.tablesStripedEffect();
                    TERMINAL.makeTblBannersGroupsSelectable();
                }
                else {
                    $('#tblBannersGroups tbody').empty();
                    builder += '<tr id="trNotBannersGroupsAdvice" style="text-align:center"><td colspan="3">No Banner Groups Assigned to this Domain</td></tr>';
                    $('#tblBannersGroups tbody').append(builder);
                }
            }
        });
    }

    var makeTblBannersGroupsSelectable = function () {
        $('#tblBannersGroups tbody tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    $(this).parent().find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $('#BannerGroupInfo_BannerGroupID').val($(this).attr('id').substr(13));
                    $('#BannerInfo_BannerGroupID').val($('#BannerGroupInfo_BannerGroupID').val());
                    $.ajax({
                        url: '/Terminals/GetBannerGroup',
                        cache: false,
                        type: 'POST',
                        data: { bannerGroupID: $('#BannerGroupInfo_BannerGroupID').val() },
                        success: function (data) {
                            $('#BannerGroupInfo_BannerGroup').val(data.BannerGroupInfo_BannerGroup);
                            $('#BannerGroupInfo_Width').val(data.BannerGroupInfo_Width);
                            $('#BannerGroupInfo_Height').val(data.BannerGroupInfo_Height);
                            UI.expandFieldset('fdsBannerGroupInfo');
                            UI.scrollTo('fdsBannerGroupInfo', null);
                            $('#fdsBannersInfo').show();
                            $('#divBannersTree').show();
                            TERMINAL._path = '\\b' + $('#BannerGroupInfo_BannerGroupID').val() + '\\';
                            TERMINAL.getBannersOfGroup($('#BannerGroupInfo_BannerGroupID').val(), $('#BannerGroupInfo_TerminalDomainID').val());
                            //TERMINAL.loadBannersTree();
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteBannerGroup, [$(e.target)]);
            }
        });
    }

    var makeUlBannersSelectable = function () {
        //$('#ulBanners li').unbind('click').on('click', function (e) {
        $('#ulBanners li').unbind('click').on('click', function (e) {
            if (!$(e.target).hasClass('delete-banner')) {
                //var element = $(e.target).is('img') ? $(e.target).parent('li') : $(this);
                if (!$(e.target).is('img')) {
                    if (!$(this).hasClass('selected-row') && $(this).attr('id') != 'liNoBanners') {
                        var id = $(this).attr('id').substr(8);
                        if ($(this).parent().find('.selected-row').length > 0) {
                            var event = $.Event('keydown');
                            event.keyCode = 27;
                            $(document).trigger(event);
                        }
                        $(this).addClass('selected-row secondary');
                        $.ajax({
                            url: '/Terminals/GetBannerInfo',
                            cache: false,
                            type: 'POST',
                            data: { bannerID: id },
                            success: function (data) {
                                $('#BannerInfo_BannerID').val(id);
                                $('#BannerInfo_BannerName').val(data.BannerInfo_BannerName);
                                $('#BannerInfo_Path').val(data.BannerInfo_Path);
                                $('#BannerInfo_Url').val(data.BannerInfo_Url);
                                if (data.BannerInfo_Permanent == true) {
                                    $('input:radio[name="BannerInfo_Permanent"]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name="BannerInfo_Permanent"]')[1].checked = true;
                                }
                                $('input:radio[name="BannerInfo_Permanent"]').trigger('change');
                                $('#BannerInfo_FromDate').val(data.BannerInfo_FromDate);
                                $('#BannerInfo_ToDate').val(data.BannerInfo_ToDate);
                                $('#BannerInfo_Culture').val(data.BannerInfo_Culture);
                                $('#BannerInfo_TerminalID').val(data.BannerInfo_TerminalID);
                                UI.expandFieldset('fdsBannerInfo');
                                UI.scrollTo('fdsBannerInfo', null);
                            }
                        });
                    }
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deleteBanner, [$(e.target)]);
            }
        });
    }

    var bindSortFunction = function () {
        $('#ulBanners li').unbind('mousedown').on('mousedown', function (evt) {
            if ($(evt.target).hasClass('picture-sizing')) {
                //if ($('#ulBanners').sortable()) {
                //    $('#ulBanners').sortable('destroy');
                //}
                $('#ulBanners').sortable({ axis: 'y' });

                $('#ulBanners').unbind('sortstart').bind('sortstart', function (e, ui) {
                    TERMINAL.previousIndex = $(ui.item[0]).index();
                });

                $('#ulBanners').unbind('sortstop').bind('sortstop', function (e, ui) {
                    if (TERMINAL.previousIndex != $(ui.item[0]).index()) {
                        //sort made
                        var items = new Array();
                        $('#ulBanners li').each(function (index, item) {
                            items[index] = $(this).attr('id').substr(8);
                        });
                        $.ajax({
                            url: '/Terminals/UpdateBannersOrder',
                            cache: false,
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: JSON.stringify({ items: items }),
                            success: function (data) {
                                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                            }
                        });
                    }
                });
            }
            else {
                if ($('#ulBanners').sortable()) {
                    $('#ulBanners').sortable('destroy');
                }
            }
        });
    }
    //var getBannersPerGroup = function (bannerGroupID) {
    //    $.ajax({
    //        url: '/Terminals/GetBannersPerGroup',
    //        cache: false,
    //        type: 'POST',
    //        data: { bannerGroupID: bannerGroupID },
    //        success: function (data) {
    //            $('#ulBanners').empty();
    //            var builder = '';
    //            if (data.length > 0) {
    //                $.each(data, function (index, item) {
    //                    builder += '<li id="liBanner' + item.BannerInfo_BannerID + '">'
    //                        + '<img class="picture-sizing" src="//eplatfront.villagroup.com' + item.BannerInfo_Path + item.BannerInfo_BannerName + '?width=100&height=100&mode=crop">'
    //                        //+ item.BannerInfo_BannerName
    //                        //+ '<span><img class="picture-sizing" src="//eplatfront.villagroup.com' + item.BannerInfo_Path + item.BannerInfo_BannerName + '?width=100&height=100&mode=crop"></span>'
    //                        + '<img src="/Content/themes/base/images/cross.png" class="right delete-banner">'
    //                        + '</li>';
    //                });
    //                $('#ulBanners').siblings().show();
    //            }
    //            else {
    //                builder = '<li id="liNoBanners">No Banners Added to this Group</li>';
    //            }
    //            TERMINAL.fillBannersPerGroupContainer(builder);
    //        }
    //    });
    //}

    //var fillBannersPerGroupContainer = function (builder) {
    //    $('#ulBanners').append(builder);

    //    $('#ulBanners').sortable({ axis: 'y' }).disableSelection();

    //    $('.delete-banner').unbind('click').on('click', function (e) {
    //        UI.confirmBox('Do you confirm you want to proceed?', deleteBanner, [$(e.target)]);
    //    });

    //    $('#ulBanners').unbind('sortstop').bind('sortstop', function (e, ui) {
    //        console.log('sortstop');
    //    });
    //    UI.ulsHoverEffect('ulBanners');
    //    TERMINAL.makeUlBannersSelectable();
    //}

    var saveBannerGroupSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Banner Group Saved') {
                var builder = '<tr id="trBannerGroup' + data.ItemID + '" class="">'
                    + '<td>' + $('#BannerGroupInfo_BannerGroup').val() + '</td>'
                    + '<td>' + $('#BannerGroupInfo_Width').val() + '</td>'
                    + '<td>' + $('#BannerGroupInfo_Height').val()
                    + '<img src="/Content/themes/base/images/cross.png" class="delete-banner-group right"></td></tr>';
                $('#trNotBannersGroupsAdvice').remove();
                $('#tblBannersGroups tbody').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                $('#frmBannerGroup').clearForm();
                TERMINAL.makeTblBannersGroupsSelectable();
                $('#trBannerGroup' + data.ItemID).click();
            }
            else {
                $('#trBannerGroup' + data.ItemID).children('td:nth-child(1)').text($('#BannerGroupInfo_BannerGroup').val());
                $('#trBannerGroup' + data.ItemID).children('td:nth-child(2)').text($('#BannerGroupInfo_Width').val());
                $('#trBannerGroup' + data.ItemID).children('td:nth-child(3)').text($('#BannerGroupInfo_Height').val());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    function deleteBannerGroup(target) {
        var elementID = $(target).parents('tr').attr('id').substr(13)
        $.ajax({
            url: '/Terminals/DeleteBannerGroup',
            cache: false,
            type: 'POST',
            data: { bannerGroupID: elementID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#ulBanners').find('.selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    if ($('#trBannerGroup' + elementID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#trBannerGroup' + elementID).remove();
                    if ($('#tblBannersGroups tbody tr').length == 0) {
                        var builder = '<tr id="trNotBannersGroupsAdvice" style="text-align:center"><td colspan="3">No Banner Groups Assigned to this Domain</td></tr>';
                        $('#tblBannersGroups tbody').append(builder);
                    }
                    else {
                        UI.tablesStripedEffect();
                    }
                    $('#divBannersTree').empty();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteBanner(target) {
        var elementID = $(target).parents('li').attr('id').substr(8);
        $.ajax({
            url: '/Terminals/DeleteBanner',
            cache: false,
            type: 'POST',
            data: { bannerID: elementID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liBanner' + data.ItemID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liBanner' + data.ItemID).remove();
                    UI.ulsHoverEffect('ulBanners');
                    if ($('#ulBanners li').length == 0) {
                        $('#ulBanners').append('<li id="liNoBanners">No Banners In This Group</li>');
                    }
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var saveBannerSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Banner Saved') {
                $('#liNoBanners').remove();
                var builder = '<li id="liBanner' + data.ItemID + '">'
                    + '<img class="picture-sizing" id="banner' + data.ItemID + '" src="//eplatfront.villagroup.com' + $('#BannerInfo_Path').val() + '?width=150&height=75&mode=crop">'
                + '<img class="delete-banner right" id="del' + data.ItemID + '" src="/Content/themes/base/images/cross.png">'
                + '</li>';
                $('#ulBanners').append(builder);
                $('#ulBanners').siblings().show();
                //$('#ulBanners').sortable({ axis: 'y' }).disableSelection();
                UI.ulsHoverEffect('ulBanners');
                UI.expandFieldset('fdsBannersInfo');
                UI.scrollTo('fdsBannersInfo', null);
                TERMINAL.makeUlBannersSelectable();
                TERMINAL.bindSortFunction();
            }
            else {
                $('#liBanner' + data.ItemID).find('img:not(.delete-banner)').attr('src', '//eplatfront.villagroup.com' + $('#BannerInfo_Path').val() + '?width=150&height=75&mode=crop');
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }



    return {
        init: init,
        getCatalogs: getCatalogs,
        saveDomainSuccess: saveDomainSuccess,
        searchResultsTable: searchResultsTable,
        tblDomainsSelection: tblDomainsSelection,
        saveCatalogSuccess: saveCatalogSuccess,
        checkAllCheckBoxes: checkAllCheckBoxes,
        saveTerminalSuccess: saveTerminalSuccess,
        ulCatalogsSelection: ulCatalogsSelection,
        saveCategorySuccess: saveCategorySuccess,
        getDomainsPerTerminal: getDomainsPerTerminal,
        ulCategoriesSelection: ulCategoriesSelection,
        getCategoriesPerCatalog: getCategoriesPerCatalog,
        makeTableRowsSelectable: makeTableRowsSelectable,
        getDescriptionsPerCategory: getDescriptionsPerCategory,
        getDestinationsPerTerminal: getDestinationsPerTerminal,
        saveCategoryDescriptionSuccess: saveCategoryDescriptionSuccess,
        ulCategoryDescriptionsSelection: ulCategoryDescriptionsSelection,
        loadBannersTree: loadBannersTree,
        getTree: getTree,
        bindClickEventToTree: bindClickEventToTree,
        //fillDivBannersContainer: fillDivBannersContainer,
        getBannerGroupsPerDomain: getBannerGroupsPerDomain,
        makeTblBannersGroupsSelectable: makeTblBannersGroupsSelectable,
        //getBannersPerGroup: getBannersPerGroup,
        //fillBannersPerGroupContainer: fillBannersPerGroupContainer,
        saveBannerGroupSuccess: saveBannerGroupSuccess,
        saveBannerSuccess: saveBannerSuccess,
        bindSortFunction: bindSortFunction,
        makeUlBannersSelectable: makeUlBannersSelectable,
        getBannersOfGroup: getBannersOfGroup
    }
}();

//var checkAllCheckBoxes = function (container) {
//    if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
//        $('#' + container).find('.chk-parent').attr('checked', 'checked');
//    }
//    $('#' + container).find('.chk-parent').change(function () {
//        $('#' + container).find('.chk-son').attr('checked', this.checked);
//    });
//    $('#' + container).find('.chk-son').change(function (e) {
//        if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
//            $('#' + container).find('.chk-parent').attr('checked', 'checked');
//        }
//        else {
//            $('#' + container).find('.chk-parent').attr('checked', false);
//        }
//    });
//}