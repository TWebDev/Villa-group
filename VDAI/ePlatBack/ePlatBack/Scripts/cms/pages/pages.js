$(function () {
    PAGE.init();
    BLOCK.init();
    $('#PageDescription_Content').ckeditor();
    $('#BlockDescription_Content').ckeditor();
    $('#SeoItemInfo_ItemType').val('Pages');
});

var PAGE = function () {

    var init = function () {

        $('#btnSearchPages').on('click', function () {
            if ($('#Search_Page').val() != '')
                $('#divPagesTree').jstree('search', $('#Search_Page').val());
        });

        $('#Search_Page').on('focus', function () {
            $('#divPagesTree').jstree('clear_search');
        });

        $('#btnClearSearch').on('click', function () {
            $('#Search_Page').val('');
            $('#divPagesTree').jstree('clear_search');
            PAGE.deletePage();
        });

        $('#btnPages').on('click', function () {
            PAGE.getTree();
        });

        $('#btnSavePageDescription').on('click', function () {
            var flag = true;
            $('#ulPageDescriptions li').each(function () {
                if ($(this).text() == $('#PageDescription_Culture option:selected').text() && !$(this).hasClass('selected-row'))
                    flag = false;
            });
            if (flag) {
                //$('#frmPageDescription').validate().settings.ignore = '.ignore-validation';
                //$('#frmPageDescription').validate({
                //ignore: ['.ignore-validation']
                //});
                //for (var i in CKEDITOR.instances) {
                //    CKEDITOR.instances[i].updateElement();
                //}
                UI.ckeditorUpdateInstances('frmPageDescription');
                $('#frmPageDescription').data('validator').settings.ignore = '.ignore-validation';
                $('#frmPageDescription').submit();
            }
            else
                UI.messageBox(-1, "Description Language already exists", null, null);
        });

        $('#btnSavePage').on('click', function () {
            if ($('#PageInfo_ParentPage option:selected').val() != $('#PageInfo_PageID').val())
                $('#frmPage').submit();
            else
                UI.messageBox(-1, 'Page NOT Saved <br /> Page cannot be parent of itself.', -1, null);
        });
    }

    var getTree = function (createdNode) {
        $.getJSON('/Pages/GetTree', { terminals: $('#Search_Terminals').val() }, function (data) {
            var jsonData = data;
            $('#divPagesTree').empty();
            $('#divPagesTree').jstree({
                'json_data': {
                    data: jsonData
                },
                'grid': {
                    columns: [
                        { header: 'Pages', width: 500 },
                        { header: 'Clickable', value: 'clickable', source: 'metadata' },
                        { header: 'Show In Menu', value: 'showInMenu', source: 'metadata' },
                        { header: '', value: 'delete', source: 'metadata' }
                    ],
                    width: 100
                },
                'themes': {
                    url: '/Content/themes/base/classic/style.css'
                },
                'ui': {
                    selected_parent_close: false,
                    initially_select: [createdNode]
                },
                'search': {
                    case_insensitive: true,
                    show_only_matches: true
                },
                'plugins': ['themes', 'json_data', 'ui', 'search', 'grid']
            });
            PAGE.bindClickEventToPagesTree();
        });

        //$.ajax({
        //    url: '/Pages/GetTree',
        //    data: { terminals: $('#Search_Terminals').val() },
        //    cache: false,
        //    type: 'GET',
        //    success: function (data) {
        //        var jsonData = JSON.parse(data);
        //        console.log(data);
        //        console.log(jsonData);
        //        //'html_data': {
        //        //    data: data
        //        //},
        //        //'grid': {
        //        //    columns: [{header: 'Pages', width: 300}, { header: 'Clickable' }, {header: 'Show In Menu'}],
        //        //    width: 100
        //        //},
        //        $('#divPagesTree').empty();
        //        $('#divPagesTree').jstree({
        //            'json_data': {
        //                data: jsonData
        //            },
        //            'themes': {
        //                url: '/Content/themes/base/classic/style.css'
        //            },
        //            'ui': {
        //                selected_parent_close: false,
        //                initially_select: [createdNode]
        //            },
        //            'search': {
        //                case_insensitive: true,
        //                show_only_matches: true
        //            },
        //            'plugins': ['themes', 'ui', 'search', 'html_data', 'json_data']
        //        });
        //        PAGE.bindClickEventToPagesTree();
        //    }
        //});
    }

    var bindClickEventToPagesTree = function () {
        $('#divPagesTree').unbind('select_node.jstree').bind('select_node.jstree', function (e, data) {
            //data.rslt.obj[0].childNodes[1].lastChild.data = 'hola';
            var builder = '';
            var secBuilder = '';

            if ($('#divPagesTree').find('.selected-row').length > 0) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            }
            $(data.rslt.obj[0]).find('a:first').addClass('selected-row primary');
            $('#divPagesTree').find('.selected-row').parents('li').each(function () {
                builder += $(this).children('a').text().trim().split(' ').join('-').toLowerCase() + '/';
            });
            builder = builder.split('/');
            for (var i = (builder.length - 1) ; i >= 0; i--) {
                secBuilder += builder[i] + '/';
            }
            SEO.urlText = secBuilder;
            var pageID = $(e.target).find('.selected-row').parent('li').attr('id');
            $.ajax({
                url: '/Pages/GetPageInfo',
                cache: false,
                type: 'POST',
                data: { pageID: pageID },
                success: function (data) {
                    $('#PageInfo_PageID').val(data.PageInfo_PageID);
                    $('#SeoItemInfo_ItemID').val(data.PageInfo_PageID);
                    $('#PageInfo_Page').val(data.PageInfo_Page);
                    $('#PageInfo_Terminal option[value="' + data.PageInfo_Terminal + '"]').attr('selected', true);
                    if (data.PageInfo_ShowInMenu) {
                        $('input:radio[name="PageInfo_ShowInMenu"]')[0].checked = true;
                    }
                    else {
                        $('input:radio[name="PageInfo_ShowInMenu"]')[1].checked = true;
                    }
                    $('#PageInfo_Order').val(data.PageInfo_Order);
                    $('#PageInfo_ParentPage option[value="' + data.PageInfo_ParentPage + '"]').attr('selected', true);
                    $('#PageInfo_PageType option[value="' + data.PageInfo_PageType + '"]').attr('selected', true);
                    if (data.PageInfo_Clickable) {
                        $('input:radio[name="PageInfo_Clickable"]')[0].checked = true;
                    }
                    else {
                        $('input:radio[name="PageInfo_Clickable"]')[1].checked = true;
                    }
                    $('#SeoItemInfo_Terminal').val($('#PageInfo_Terminal option:selected').val());
                    PAGE.updateUlPageDescriptions(pageID);
                    SEO.updateTblSeoItems();
                    $('#fdsPageDescriptions').show();
                    $('#fdsSeoItems').show();
                    UI.expandFieldset('fdsPagesInfo');
                    UI.scrollTo('fdsPagesInfo', null);

                }
            });
        });
        PAGE.deletePage();
    }

    var deletePage = function () {
        var imgs = document.querySelectorAll('.delete-item');
        imagesLoaded(imgs, function () {
            $('.delete-item').unbind('click').on('click', function () {
                UI.confirmBox('Do you confirm you want to proceed?', deletePages, [$(this).parents('li').first().attr('id')]);
            });
        });
    }

    function deletePages(id) {
        $.ajax({
            url: '/Pages/DeletePage',
            type: 'POST',
            cache: false,
            data: { pageID: id },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#' + data.ItemID).find('li').length == 0) {
                        if ($('#' + data.ItemID).children(':nth-child(2)').hasClass('selected-row')) {
                            var event = $.Event('keydown');
                            event.keyCode = 27;
                            $(document).trigger(event);
                        }
                        $('#' + data.ItemID).remove();
                    }
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var updateUlPageDescriptions = function (pageID) {
        $('#PageDescription_PageID').val(pageID);
        $.ajax({
            url: '/Pages/GetPageDescriptions',
            type: 'POST',
            cache: false,
            data: { pageID: pageID },
            success: function (data) {
                $('#ulPageDescriptions').empty();
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="liPageDescription' + item.PageDescription_PageDescriptionID + '">'
                        + $('#PageDescription_Culture option[value="' + item.PageDescription_Culture + '"]').text()
                        + '<img id="delPageDescription' + item.PageDescription_PageDescriptionID + '"'
                        + 'src="/Content/themes/base/images/cross.png" class="right" /></li>';
                    });
                    $('#ulPageDescriptions').append(builder);
                    UI.ulsHoverEffect('ulPageDescriptions');
                    PAGE.makeUlPageDescriptionsRowsSelectable();
                }
                else {
                    builder += '<li id="liNoDescriptions">No Descriptions</li>';
                    $('#ulPageDescriptions').append(builder);
                }
            }
        });
    }

    var makeUlPageDescriptionsRowsSelectable = function () {
        $('#ulPageDescriptions li').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(e.target).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $('#PageDescription_PageDescriptionID').val($(e.target).attr('id').substr(17));
                    $.ajax({
                        url: '/Pages/GetPageDescription',
                        cache: false,
                        type: 'POST',
                        data: { pageDescriptionID: $('#PageDescription_PageDescriptionID').val() },
                        success: function (data) {
                            $('#PageDescription_Culture option[value="' + data.PageDescription_Culture + '"]').attr('selected', true);
                            $('#PageDescription_Header').val(data.PageDescription_Header);
                            $('#PageDescription_ContentHeader').val(data.PageDescription_ContentHeader);
                            $('#PageDescription_Content').val(data.PageDescription_Content);
                            $('#PageDescription_AfterBody').val(data.PageDescription_AfterBody);
                            $('#PageDescription_Footer').val(data.PageDescription_Footer);
                            $('#PageDescription_PageStructure option[value="' + data.PageDescription_PageStructure + '"]').attr('selected', true);
                            if (data.PageDescription_IsActive)
                                $('input:radio[name="PageDescription_IsActive"]')[0].checked = true;
                            else
                                $('input:radio[name="PageDescription_IsActive"]')[1].checked = true;
                            $('#PageDescription_Url').val(data.PageDescription_Url);
                            UI.expandFieldset('fdsPageDescriptionInfo');
                            UI.scrollTo('fdsPageDescriptionInfo', null);
                        }
                    });
                }
            }
            else
                UI.confirmBox('Do you confirm you want to proceed?', deletePageDescription, [$(e.target).attr('id').substr(18)]);
        });
    }

    var savePageSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == "Page Saved") {
                $('#frmPage').clearForm();
                PAGE.getTree(data.ItemID);
                $.getJSON('/Pages/GetDDLData', { itemType: 'pages', itemID: 0 }, function (data) {
                    $('#PageInfo_ParentPage').fillSelect(data);
                });
            }
            else {
                $('#divPagesTree').find('.selected-row')[0].childNodes[1].data = $('#PageInfo_Page').val();
                $('#PageInfo_ParentPage option[value="' + data.ItemID + '"]').text($('#PageInfo_Page').val());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var savePageDescriptionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == "Page Description Saved") {
                var builder = '<li id="liPageDescription' + data.ItemID + '">'
                        + $('#PageDescription_Culture option:selected').text()
                        + '<img id="delPageDescription' + data.ItemID + '"'
                        + 'src="/Content/themes/base/images/cross.png" class="right" /></li>';
                $('#liNoDescriptions').remove();
                $('#ulPageDescriptions').append(builder);
                $('#frmPageDescription').clearForm();
                $('#frmPageDescription').find('textarea').each(function () {
                    $(this).val('');
                });
                //$('#PageDescription_Header').ckeditor();
                //$('#PageDescription_ContentHeader').ckeditor();
                $('#PageDescription_Content').ckeditor();
                //$('#PageDescription_AfterBody').ckeditor();
                //$('#PageDescription_Footer').ckeditor();
                UI.ulsHoverEffect('ulPageDescriptions');
                PAGE.makeUlPageDescriptionsRowsSelectable();
            }
            else
                $('#liPageDescription' + data.ItemID)[0].firstChild.textContent = $('#PageDescription_Culture option:selected').text();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    function deletePageDescription(pageDescriptionID) {
        $.ajax({
            url: '/Pages/DeletePageDescription',
            cache: false,
            type: 'POST',
            data: { pageDescriptionID: pageDescriptionID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liPageDescription' + data.ItemID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liPageDescription' + data.ItemID).remove();
                    if ($('#ulPageDescriptions li').length == 0)
                        $('#ulPageDescriptions').append('<li id="liNoDescriptions">No Descriptions</li>');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    return {
        init: init,
        getTree: getTree,
        deletePage: deletePage,
        savePageSuccess: savePageSuccess,
        updateUlPageDescriptions: updateUlPageDescriptions,
        bindClickEventToPagesTree: bindClickEventToPagesTree,
        savePageDescriptionSuccess: savePageDescriptionSuccess,
        makeUlPageDescriptionsRowsSelectable: makeUlPageDescriptionsRowsSelectable
    }
}();

var BLOCK = function () {
    var editingBlock;
    var init = function () {
        //eventos
        setEvents();
    }

    function setEvents() {
        $('.delete-block').unbind('click').on('click', function () {
            UI.confirmBox('Do you confirm you want to proceed?', deleteBlock, [$(this).parents('tr').first().attr('id')]);
        });
        $('.edit-block').unbind('click').on('click', function () {
            editBlock($(this).parents('tr').first().attr('id'));
        });
        $('.table tr').on('click', function () {
            $(this).siblings().removeClass('selected-row primary');
            $(this).addClass('selected-row primary');
        });
        $('.edit-blockDesc').unbind('click').on('click', function () {
            editBlockDescription($(this).parents('tr').first().attr('id'));
        });
        $('.delete-blockDesc').unbind('click').on('click', function () {
            UI.confirmBox('Do you confirm you want to proceed?', deleteBlockDesc, [$(this).parents('tr').first().attr('id')]);
        });
    }

    function editBlockDescription(id) {
        $.each(BLOCK.editingBlock.BlockItem_Descriptions, function (i, desc) {
            if (desc.BlockDescription_ID == id.replace('blockDesc', '')) {
                $('#BlockDescription_ID').val(desc.BlockDescription_ID);
                $('#BlockDescription_BlockID').val(desc.BlockDescription_BlockID);
                $('#BlockDescription_Content').val(desc.BlockDescription_Content);
                $('#BlockDescription_Culture').val(desc.BlockDescription_Culture);
                $('#BlockDescription_ID').val(desc.BlockDescription_ID);
                $('#BlockDescription_Language').val(desc.BlockDescription_Language);
                $('#fdsBlockDescriptionItem>div').slideDown('fast');
            }
        });
    }

    function editBlock(id) {
        $.ajax({
            url: '/Pages/GetBlock',
            type: 'GET',
            cache: false,
            data: { blockID: id.replace('blockID','') },
            success: function (data) {
                BLOCK.editingBlock = data;
                $('#BlockItem_BlockID').val(data.BlockItem_BlockID);
                $('#BlockItem_TerminalID').val(data.BlockItem_TerminalID);
                $('#BlockItem_Block').val(data.BlockItem_Block);
                $('#BlockItem_General').val(data.BlockItem_General);
                $('#BlockItem_Frame').val(data.BlockItem_Frame);
                $('#tblBlockDescriptions tbody').html('');
                $.each(data.BlockItem_Descriptions, function (d, desc) {
                    var tr = '<tr id="blockDesc' + desc.BlockDescription_ID + '"><td class="edit-blockDesc">' + desc.BlockDescription_Language + '</td><td class=delete-blockDesc><span style="margin-right: 0px; display: inline-block;"><img src="/Content/themes/base/images/cross.png" class="right"></span></td></tr>';
                    $('#tblBlockDescriptions tbody').append(tr);
                });
                $('#fdsBlockItem>div').slideDown('fast');
                $('#fdsBlockDescriptions').show();
                $('#fdsBlockDescriptions>div').slideDown('fast');
                $('#BlockDescription_BlockID').val(data.BlockItem_BlockID);
                setEvents();
            }
        });
    }

    function deleteBlock(id) {
        $.ajax({
            url: '/Pages/DeleteBlock',
            type: 'POST',
            cache: false,
            data: { blockID: id.replace('blockID', '') },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    $('#blockID' + data.ItemID).remove();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                $('#frmBlockItem').clearForm();
                $('#fdsBlockDescriptions').hide();
            }
        });
    }

    function deleteBlockDesc(id) {
        $.ajax({
            url: '/Pages/DeleteBlockDescription',
            type: 'POST',
            cache: false,
            data: { blockDescID: id.replace('blockDesc', '') },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    $('#blockDesc' + data.ItemID).remove();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                $('#frmBlockDescriptionItem').clearForm();
                $('#fdsBlockDescriptionItem>div').hide();
            }
        });
    }

    var saveBlockSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Block Saved') {
                $('#tblBlocks').append('<tr id="blockID' + data.ItemID + '"><td class="edit-block">' + $('#BlockItem_TerminalID option:selected').text() + '</td><td class="edit-block">' + $('#BlockItem_Block').val() + '</td><td class="delete-block"><span class="" style="margin-right: 0px; display: inline-block;"><img src="/Content/themes/base/images/cross.png" class="right"></span></td></tr>');
                $('#BlockItem_BlockID').val(data.ItemID);
                $('#BlockDescription_BlockID').val(data.ItemID);
                $('#fdsBlockDescriptions').show();
                $('#fdsBlockDescriptions>div').slideDown('fast');
            } else {
                $('#blockID' + data.ItemID + ' td').eq(0).html($('#BlockItem_TerminalID option:selected').text());
                $('#blockID' + data.ItemID + ' td').eq(1).html($('#BlockItem_Block').val());
            }
            setEvents();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveBlockDescriptionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Block Description Saved') {
                $('#tblBlockDescriptions').append('<tr id="blockDesc' + data.ItemID + '"><td class="edit-blockDesc">' + $('#BlockDescription_Culture option:selected').text() + '</td><td class="delete-blockDesc"><span class="" style="margin-right: 0px; display: inline-block;"><img src="/Content/themes/base/images/cross.png" class="right"></span></td></tr>');
                $('#BlockDescription_ID').val(data.ItemID);
            } else {
                $('#blockDesc' + data.ItemID + ' td').eq(0).html($('#BlockDescription_Culture option:selected').text());
            }
            setEvents();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    return {
        init: init,
        saveBlockSuccess: saveBlockSuccess,
        editingBlock: editingBlock,
        saveBlockDescriptionSuccess: saveBlockDescriptionSuccess
    }
}();
