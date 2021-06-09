$(function () {
    SEO.init();
});

var SEO = function () {

    var oTable;

    var urlText = '';

    var init = function () {

        $('#SeoItemInfo_Title').on('keyup', function () {
            var itemType = $('#SeoItemInfo_ItemType').val().toLowerCase();
            var secondLevelRoute;
            var secondLevelRoute = '';
            var friendlyUrl = '';
            var realUrl = '';
            switch (itemType) {
                case 'packages':
                    // /itemType/destination/packageName
                    secondLevelRoute = SEO.urlText.toString().trim().split(' ').join('-').toLowerCase();
                    realUrl = '/packages/' + secondLevelRoute + '/' + $('#SeoItemInfo_Title').val()
                    break;
                case 'pages':
                    // /itemType('pages')/pageRoute/pageName
                    secondLevelRoute = SEO.urlText.substr(1, (SEO.urlText.toString().lastIndexOf('/') - 1));
                    realUrl = '/pages/' + $('#SeoItemInfo_ItemID').val();
                    break;
                case 'destination':
                    // /destination
                    friendlyUrl = '/' + SEO.urlText.toString().trim().split(' ').join('-').toLowerCase();
                    break;
                case 'activities':
                    // /mainCategory/secondaryCategory/service
                    realUrl = '/activities/detail/' + $('#SeoItemInfo_ItemID').val();
                    break;
                case 'categories':
                    //for main categories
                    // /category
                    //for subcategories
                    // /parent category/subcategory
                    var urlText = SEO.urlText.split(',');
                    for (var i = urlText.length; i > 0; i--) {
                        if (i != urlText.length) {
                            secondLevelRoute += '/';
                        }
                        secondLevelRoute += urlText[i - 1].split(' ').join('-').toLowerCase();
                    }
                    realUrl = '/activities/category/' + $('#SeoItemInfo_ItemID').val();
                    break;
            }
            if (friendlyUrl == '') {
                if (itemType == 'pages') {
                    $('#SeoItemInfo_FriendlyUrl').val('/' + $('#SeoItemInfo_Title').val());
                } else if (itemType == 'categories') {
                    $('#SeoItemInfo_FriendlyUrl').val('/' + urlText[1].split(' ').join('-').toLowerCase() + '/' + $('#SeoItemInfo_Title').val());
                } else {
                    $('#SeoItemInfo_FriendlyUrl').val('/' + itemType + '/' + secondLevelRoute + '/' + $('#SeoItemInfo_Title').val());
                }
            }
            else {
                $('#SeoItemInfo_FriendlyUrl').val(friendlyUrl);
            }
            validateUrl($('#SeoItemInfo_FriendlyUrl').attr('id'));
            $('#SeoItemInfo_Url').val(realUrl);
        });

        $('#SeoItemInfo_TerminalItem').on('change', function (e) {
            $('#SeoItemInfo_Terminal').val($(this).val());
        });

        $('#SeoItemInfo_FriendlyUrl').on('keyup', function (e) {
            var start = this.selectionStart,
            end = this.selectionEnd;

            $(this).val(filterUrl($(this).val()));

            this.setSelectionRange(start, end);
        });
    }

    function validateUrl(control) {
        $('#' + control).val(filterUrl($('#' + control).val()));
    }

    function filterUrl(url) {
        url = url.toLowerCase();
        url = url.replace(/---/g, '-');
        url = url.replace(/--/g, '-');
        url = url.replace(/ /g, '-');
        url = url.replace(/\?/g, '');
        url = url.replace(/!/g, '');
        url = url.replace(/,/g, '');
        url = url.replace(/á/g, 'a');
        url = url.replace(/é/g, 'e');
        url = url.replace(/í/g, 'i');
        url = url.replace(/ó/g, 'o');
        url = url.replace(/ú/g, 'u');
        url = url.replace(/&/g, 'and');
        url = url.replace(/ñ/g, 'n');
        url = url.replace(/=/g, '');
        return url;
    }

    var updateTblSeoItems = function () {
        if ($('#divTblExistingSeoItems').length > 0) {
            $('#divTblExistingSeoItems').empty();
            $.ajax({
                url: '/SeoItems/GetSeoItems',
                cache: false,
                type: 'POST',
                //data: { itemType: $('#SeoItemInfo_ItemType').val(), itemID: $('#SeoItemInfo_SeoItemID').val() },
                data: { itemType: $('#SeoItemInfo_ItemType').val(), itemID: $('#SeoItemInfo_ItemID').val() },
                success: function (data) {
                    $('#divTblExistingSeoItems').html(data);
                    SEO.searchResultsTable($('#tblSearchSeoItemsResults'));
                    SEO.makeTableRowsSelectable();
                }
            });
        }
    }

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchSeoItemsResults', tableColumns.length - 1);
        SEO.oTable = $('#tblSearchSeoItemsResults').dataTable();
    }

    var makeTableRowsSelectable = function () {
        SEO.oTable.$('tr').not('theader').unbind('click').on('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    var id = $(this).attr('id').substr(6);
                    $(this).parent('tbody').find('.selected-row').removeClass('selected-row secondary');
                    $(this).addClass('selected-row secondary');
                    $.ajax({
                        url: '/SeoItems/GetSeoItemPerID',
                        cache: false,
                        type: 'POST',
                        data: { seoItemID: id },
                        success: function (data) {
                            $('#SeoItemInfo_SeoItemID').val(id);
                            $('#SeoItemInfo_Title').val(data.SeoItemInfo_Title);
                            $('#SeoItemInfo_Keywords').val(data.SeoItemInfo_Keywords);
                            $('#SeoItemInfo_Description').val(data.SeoItemInfo_Description);
                            $('#SeoItemInfo_FriendlyUrl').val(data.SeoItemInfo_FriendlyUrl);
                            $('#SeoItemInfo_Url').val(data.SeoItemInfo_Url);
                            $('#SeoItemInfo_Culture option[value=' + data.SeoItemInfo_Culture + ']').attr('selected', true);
                            if (data.SeoItemInfo_Index == true) {
                                $('input:radio[name="SeoItemInfo_Index"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="SeoItemInfo_Index"]')[1].checked = true;
                            }
                            if (data.SeoItemInfo_Follow == true) {
                                $('input:radio[name="SeoItemInfo_Follow"]')[0].checked = true;
                            }
                            else {
                                $('input:radio[name="SeoItemInfo_Follow"]')[1].checked = true;
                            }
                            $('#SeoItemInfo_TerminalItem option[value="' + data.SeoItemInfo_TerminalItem + '"]').attr('selected', true);
                            $('#SeoItemInfo_TerminalItem').change();
                            UI.expandFieldset('fdsSeoItemInfo');
                            UI.scrollTo('fdsSeoItemInfo', null);
                        }
                    });
                }
            }
            else
                UI.confirmBox('Do you confirm you want to proceed?', deleteSeoItem, [$(e.target).attr('id').substr(4)]);
        });
    }

    var saveSeoItemSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Seo Item Saved') {
                var oSettings = SEO.oTable.fnSettings();
                var iAdded = SEO.oTable.fnAddData([
                    $('#SeoItemInfo_Title').val(),
                    $('#SeoItemInfo_Culture option:selected').val(),
                    $('#SeoItemInfo_TerminalItem option:selected').text(),
                    $('#SeoItemInfo_FriendlyUrl').val(),
                    $('#SeoItemInfo_Url').val(),
                    '<img class="right" src="/Content/themes/base/images/trash.png" id="delI' + data.ItemID + '">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'trItem' + data.ItemID);
                SEO.oTable.fnDisplayRow(aRow);
                SEO.makeTableRowsSelectable();
                UI.tablesHoverEffect();
                $('#frmSeoItem').clearForm();
            }
            else {
                var array = SEO.oTable.fnGetNodes();
                var nTr = SEO.oTable.$('tr.selected-row');
                var position = SEO.oTable.fnGetPosition(nTr[0]);
                SEO.oTable.fnDisplayRow(array[position]);
                SEO.oTable.fnUpdate([
                    $('#SeoItemInfo_Title').val(),
                    $('#SeoItemInfo_Culture option:selected').text(),
                    $('#SeoItemInfo_TerminalItem option:selected').text(),
                    $('#SeoItemInfo_FriendlyUrl').val(),
                    $('#SeoItemInfo_Url').val(),
                    '<img class="right" src="/Content/themes/base/images/trash.png" id="delI' + data.ItemID + '">'
                ], $('#trItem' + data.ItemID)[0], undefined, false);
            }
            SEO.updateSeoItemRelatedLists();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    function deleteSeoItem(seoItemID) {
        $.ajax({
            url: '/SeoItems/DeleteSeoItem',
            type: 'POST',
            cache: false,
            data: { seoItemID: seoItemID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#trItem' + seoItemID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    SEO.oTable.fnDeleteRow($('#trItem' + data.ItemID)[0]);
                }
                UI.tablesStripedEffect();
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var updateSeoItemRelatedLists = function () {
        $.getJSON('/SeoItems/GetDDLData', { path: 'seo-item-related', itemType: $('#SeoItemInfo_ItemType').val(), itemID: $('#SeoItemInfo_ItemID').val() }, function (data) {
            $('.seo-items-related').each(function () {
                var itemSelected = $(this).val();
                $(this).fillSelect(data);
                $('#' + $(this).attr('id') + ' option[value="' + itemSelected + '"]').attr('selected', true);
            });
        });
    }

    return {
        init: init,
        updateTblSeoItems: updateTblSeoItems,
        searchResultsTable: searchResultsTable,
        saveSeoItemSuccess: saveSeoItemSuccess,
        makeTableRowsSelectable: makeTableRowsSelectable,
        updateSeoItemRelatedLists: updateSeoItemRelatedLists
    }
}();