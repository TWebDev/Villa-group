/// <reference path="/Scripts/layout/ui.js" />

$(function () {
    PICTURE.init();
});

var PICTURE = function () {
    var path;

    var destination = '';

    var _class = '';

    var init = function () {
        $('.items-galleries').unbind('change').on('change', function () {
            var parentClass = $(this).parents('fieldset').first().children('div').first().attr('class');
            parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
            var parentContainer = $(this).parents('fieldset').first().attr('id');
            if ($(this).val() != 0) {
                PICTURE.GetImagesPerItemType($(parentClass + '.picture-info-item-type').val(), $(this).val(), false, parentClass);
            }
        });

        $('.ul-pictures-gallery').droppable();

        $('.btn-save-pictures-on-gallery').on('click', function () {
            var parentClass = $(this).parents('fieldset').first().children('div').first().attr('class');
            parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
            var array = $(parentClass + '.ul-pictures-gallery').sortable('toArray');
            $.ajax({
                url: '/Pictures/SavePicturesOnGallery',
                cache: false,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ sysItemType: $(parentClass + '.picture-info-item-type').val(), itemID: $(parentClass + '.picture-info-item-id').val(), picturesArray: array }),
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('.picture-uploader').on('complete', function (first, id, fileName, data) {
            var parentClass = $(this).parents('fieldset').first().children('div').first().attr('class');
            parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
            $(parentClass + '.picture-uploader').fineUploader('reset');
            var duration = data['response'].Type < 0 ? data['response'].Type : null;
            if (data['success'] != false) {
                $(parentClass + '.li-no-pictures').remove();
                var builder = '<li id="liPicture' + data['response'].ObjectID + '">'
                            + '<span><img id="picture' + data['response'].ObjectID + '" class="ui-draggable picture-sizing"'
                            + 'src="//eplatfront.villagroup.com' + data['path'].path + '?width=150&height=75&mode=crop" >'
                            + '</span>'
                            + '<img class="delete-picture right" src="/Content/themes/base/images/trash.png"/>'
                            + '<input class="main-indicator right" type="checkbox"';
                if ($(parentClass + '.ul-pictures-gallery li').length == 0) {
                    builder += 'checked="checked" ';
                }
                builder += '/></li>';
                $(parentClass + '.ul-pictures-gallery').append(builder);
                $(parentClass + '.ul-pictures-gallery').sortable({ axis: 'y' }).disableSelection();
                PICTURE.bindPicturesGalleryEvents();
                var builder = '<h3 class="header">'
                    + '<span class="accordion-icon"><img class="icon-delete right" id="delImage' + data['response'].ObjectID + '" src="/Content/themes/base/images/trash.png"></span>'
                    + '<span><img class="picture-sizing" id="picture' + data['response'].ObjectID + '" src="//eplatfront.villagroup.com' + data['path'].path + '?width=150&height=75&mode=crop"></span>'
                    + '<span class="picture-descriptions" id="pictureDescriptions">Descriptions: 0</span>'
                    + '</h3>'
                    + '<div class="picture-container" id="divImage' + data['response'].ObjectID + '"></div>';
                //var builder = '<span class="accordion-icon"><img class="icon-delete right" id="delImage' + data['response'].ObjectID + '" src="/Content/themes/base/images/trash.png"></span>'
                //    + '<h3 class="header">'
                //    + '<span><img class="picture-sizing" id="picture' + data['response'].ObjectID + '" src="//eplatfront.villagroup.com' + data['path'].path + '?width=150&height=75&mode=crop"></span>'
                //    + '<span class="picture-descriptions" id="pictureDescriptions">Descriptions: 0</span>'
                //    + '</h3>'
                //    + '<div class="picture-container" id="divImage' + data['response'].ObjectID + '"></div>';
                PICTURE.fillDivPicturesContainer(parentClass, builder);
            }
            var exception = data['response'].Exception != null ? data['response'].Exception : "";
            UI.messageBox(data['response'].Type, data['response'].Message + '<br />' + exception, duration, null);
        });
    }

    //var loadPicturesTree = function (provider) {
    //    var column;
    //    var itemType;
    //    var $itemType = $(PICTURE._class + '.picture-info-item-type').val();
    //    var $itemID = $(PICTURE._class + '.picture-info-item-id').val();

    //    switch ($itemType) {
    //        case 'Room Type': {
    //            itemType = 'Places';
    //            break;
    //        }
    //        default: {
    //            itemType = $itemType;
    //            break;
    //        }
    //    }
    //    PICTURE.destination = UI.getItemDestination(itemType);
    //    if ($(PICTURE._class + '.div-pictures-tree').children('ul').length == 0) {
    //        PICTURE.getTree($itemType, $itemID, PICTURE.destination, provider);
    //    }
    //    else {
    //        var dest = PICTURE.destination.toString().trim().split(' ').join('-').toLowerCase();
    //        var node = dest + '_' + $(PICTURE._class + '.picture-info-item-type').val().substr(0, 1).toLowerCase() + $(PICTURE._class + '.picture-info-item-id').val();
    //        //$(PICTURE._class + '.div-pictures-tree').jstree('select_node', '#' + node);
    //        PICTURE.bindClickEventToPicturesTree();
    //    }
    //}

    var loadPicturesTree = function (provider) {
        var column;
        var itemType;
        var $itemType = $(PICTURE._class + '.picture-info-item-type').val();
        var $itemID = $(PICTURE._class + '.picture-info-item-id').val();

        switch ($itemType) {
            case 'Room Type': {
                itemType = 'Places';
                break;
            }
            default: {
                itemType = $itemType;
                break;
            }
        }
        PICTURE.destination = UI.getItemDestination(itemType);
        if ($(PICTURE._class + '.div-pictures-tree').children('ul').length == 0) {
            PICTURE.getTree($itemType, $itemID, PICTURE.destination, provider);
        }
        else {
            var dest = PICTURE.destination.toString().trim().split(' ').join('-').toLowerCase();
            var node = dest + '_' + $(PICTURE._class + '.picture-info-item-type').val().substr(0, 1).toLowerCase() + $(PICTURE._class + '.picture-info-item-id').val();
            $(PICTURE._class + '.div-pictures-tree').jstree('select_node', '#' + node);
            PICTURE.bindClickEventToPicturesTree();
        }
    }

    //fill list of galleries to copy pictures
    var getItemNames = function (ajaxFlag, extraParams) {
        var params = extraParams != undefined ? extraParams : $('#Search_Terminals').val();
        $(document).ajaxStop(function (e) {
            if (ajaxFlag) {
                $.ajax({
                    url: '/Pictures/GetItemNames',
                    cache: false,
                    type: 'POST',
                    data: { parameters: params, itemType: $(PICTURE._class + '.picture-info-item-type').val() },
                    success: function (data) {
                        $(PICTURE._class + '.items-galleries').empty();
                        var builder = '<option value="0">--Select ' + $(PICTURE._class + '.picture-info-item-type').val() + '--</option>';
                        $.each(data, function (index, item) {
                            builder += '<option value="' + item.Key + '">' + item.Value + '</option>';
                        });
                        $(PICTURE._class + '.items-galleries').append(builder);
                        ajaxFlag = false;
                    }
                });
            }
        });
    }

    var getGalleryName = function (itemType, itemID) {
        $.ajax({
            url: '/Pictures/GetGalleryName',
            cache: false,
            type: 'POST',
            data: { itemType: itemType, itemID: itemID },
            success: function (data) {
                var parentClass = PICTURE._class != undefined ? PICTURE._class : '';
                $(parentClass + '.gallery-header').text(data + ' ');
            }
        });
    }

    //var getTree = function (itemType, itemID, destination, provider) {
    //    var $picturesTree = $(PICTURE._class + '.div-pictures-tree');
    //    var $itemType = $(PICTURE._class + '.picture-info-item-type').val();
    //    var currentProvider = provider != undefined ? provider : "";
    //    var dest = destination.toString().trim().split(' ').join('-').toLowerCase();
    //    $.getJSON('/Pictures/GetTree', { sysItemType: itemType, itemID: itemID, currentProvider: currentProvider, destination: dest }, function (data) {
    //        var jsonData = $.parseJSON(data);
    //        console.log(jsonData);
    //        $picturesTree.empty();
    //        //check if always is the same destination where there are more than one
    //        var node = dest + '_' + $(PICTURE._class + '.picture-info-item-type').val().substr(0, 1).toLowerCase() + $(PICTURE._class + '.picture-info-item-id').val();
    //        if ($(PICTURE._class + '#' + node).length == 0) {
    //            //$picturesTree.jstree('create', $(PICTURE._class + '#cabo-san-lucas_room type'), 'inside', { 'data': node }, false, true);
    //            $picturesTree.jstree('create_node', ([$(PICTURE._class + '#cabo-san-lucas_room type'), node, 'last', false, true]));
    //        }
    //        $picturesTree.jstree({
    //            'core': {
    //                'data': jsonData
    //            }
    //        });
    //        $picturesTree.jstree('select_node', ([node]));
    //        PICTURE.bindClickEventToPicturesTree(node);
    //    });
    //}

    var getTree = function (itemType, itemID, destination, provider) {
        if ($(PICTURE._class + '.picture-info-item-type').length > 0) {
            var $picturesTree = $(PICTURE._class + '.div-pictures-tree');
            var $itemType = $(PICTURE._class + '.picture-info-item-type').val();
            var currentProvider = provider != undefined ? provider : "";
            var dest = destination.toString().trim().split(' ').join('-').toLowerCase();
            $.getJSON('/Pictures/GetTree', { sysItemType: itemType, itemID: itemID, currentProvider: currentProvider, destination: dest }, function (data) {
                var jsonData = data;
                $picturesTree.empty();
                //check if always is the same destination where there are more than one
                var node = dest + '_' + $(PICTURE._class + '.picture-info-item-type').val().substr(0, 1).toLowerCase() + $(PICTURE._class + '.picture-info-item-id').val();
                if ($(PICTURE._class + '#' + node).length == 0) {
                    $picturesTree.jstree('create', $(PICTURE._class + '#cabo-san-lucas_room type'), 'inside', { 'data': node }, false, true);
                }
                $picturesTree.jstree({
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
                PICTURE.bindClickEventToPicturesTree(node);
            });
        }
    }

    //var bindClickEventToPicturesTree = function (node) {
    //    //$('.div-pictures-tree').unbind('select_node.jstree').bind('select_node.jstree', function (e, data) {
    //    //    var parentClass = $(this).parents('fieldset').first().children('div').attr('class');
    //    //    parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
    //    //    var builder = '';
    //    //    var secBuilder = '';
    //    //    if ($(parentClass + ' .div-pictures-tree').find('.selected-row').length > 0) {
    //    //        if ($(parentClass + ' .div-pictures-tree').parents('fieldset:first').hasClass('fds-active')) {
    //    //            var event = $.Event('keydown');
    //    //            event.keyCode = 27;
    //    //            $(document).trigger(event);
    //    //        }
    //    //        else {
    //    //            $(parentClass + ' .div-pictures-tree').find('.selected-row').removeClass('selected-row');
    //    //        }
    //    //    }
    //    //    $(data.rslt.obj[0]).find('a:first').addClass('selected-row secondary');
    //    //    $(parentClass + ' .div-pictures-tree').find('.selected-row').parents('li').each(function () {
    //    //        var start = $(this).attr('id').indexOf('_') + 1;
    //    //        builder += $(this).attr('id').substr(start, $(this).attr('id').length - start) + '\\';
    //    //    });
    //    //    builder = builder.split('\\');
    //    //    for (var i = (builder.length - 1) ; i >= 0; i--) {
    //    //        secBuilder += builder[i] + '\\';
    //    //    }
    //    //    path = secBuilder;
    //    //    $.ajax({
    //    //        async: false,
    //    //        url: '/Pictures/GetFiles',
    //    //        cache: false,
    //    //        type: 'POST',
    //    //        data: { directory: secBuilder },
    //    //        success: function (data) {
    //    //            $(parentClass + '.div-pictures-container').empty();
    //    //            var builder = '';
    //    //            $.each(data, function (index, item) {
    //    //                //builder += '<span class="accordion-icon"><img class="right icon-delete" id="delImage' + item.PictureID + '" src="/Content/themes/base/images/trash.png"></span>'
    //    //                //        + '<h3 class="header">'
    //    //                //        + '<span><img class="picture-sizing" id="picture' + item.PictureID + '" src="//eplatfront.villagroup.com' + item.Path + item.File + '?width=150&height=75&mode=crop"></span>'
    //    //                //        + '<span class="picture-descriptions" id="pictureDescriptions">Descriptions: ' + item.Descriptions + '</span>'
    //    //                //        + '</h3>'
    //    //                //    + '<div class="picture-container" id="divImage' + item.PictureID + '"></div>';
    //    //                builder += '<h3 class="header">'
    //    //                        + '<span class="accordion-icon"><img class="right icon-delete" id="delImage' + item.PictureID + '" src="/Content/themes/base/images/trash.png"></span>'
    //    //                        + '<span><img class="picture-sizing" id="picture' + item.PictureID + '" src="//eplatfront.villagroup.com' + item.Path + item.File + '?width=150&height=75&mode=crop"></span>'
    //    //                        + '<span class="picture-descriptions" id="pictureDescriptions">Descriptions: ' + item.Descriptions + '</span>'
    //    //                        + '</h3>'
    //    //                    + '<div class="picture-container" id="divImage' + item.PictureID + '"></div>';
    //    //            });
    //    //            PICTURE.fillDivPicturesContainer(parentClass, builder);
    //    //            $(parentClass + '.picture-uploader').fineUploader({
    //    //                request: {
    //    //                    endpoint: '/Pictures/UploadPicture',
    //    //                    params: {
    //    //                        path: path,
    //    //                        sysItemType: $(parentClass + '.picture-info-item-type').val()
    //    //                    }
    //    //                },
    //    //                multiple: true,
    //    //                failedUploadTextDisplay: {
    //    //                    mode: 'default'
    //    //                }
    //    //            });
    //    //        }
    //    //    });
    //    //});
    //}

    var bindClickEventToPicturesTree = function (node) {
        $('.div-pictures-tree').unbind('select_node.jstree').bind('select_node.jstree', function (e, data) {
            var parentClass = $(this).parents('fieldset').first().children('div').attr('class');
            parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
            var builder = '';
            var secBuilder = '';
            if ($(parentClass + ' .div-pictures-tree').find('.selected-row').length > 0) {
                if ($(parentClass + ' .div-pictures-tree').parents('fieldset:first').hasClass('fds-active')) {
                    var event = $.Event('keydown');
                    event.keyCode = 27;
                    $(document).trigger(event);
                }
                else {
                    $(parentClass + ' .div-pictures-tree').find('.selected-row').removeClass('selected-row');
                }
            }
            $(data.rslt.obj[0]).find('a:first').addClass('selected-row secondary');
            $(parentClass + ' .div-pictures-tree').find('.selected-row').parents('li').each(function () {
                var start = $(this).attr('id').indexOf('_') + 1;
                builder += $(this).attr('id').substr(start, $(this).attr('id').length - start) + '\\';
            });
            builder = builder.split('\\');
            for (var i = (builder.length - 1) ; i >= 0; i--) {
                secBuilder += builder[i] + '\\';
            }
            path = secBuilder;
            $.ajax({
                async: false,
                url: '/Pictures/GetFiles',
                cache: false,
                type: 'POST',
                data: { directory: secBuilder },
                success: function (data) {
                    $(parentClass + '.div-pictures-container').empty();
                    var builder = '';
                    $.each(data, function (index, item) {
                        //builder += '<span class="accordion-icon"><img class="right icon-delete" id="delImage' + item.PictureID + '" src="/Content/themes/base/images/trash.png"></span>'
                        //        + '<h3 class="header">'
                        //        + '<span><img class="picture-sizing" id="picture' + item.PictureID + '" src="//eplatfront.villagroup.com' + item.Path + item.File + '?width=150&height=75&mode=crop"></span>'
                        //        + '<span class="picture-descriptions" id="pictureDescriptions">Descriptions: ' + item.Descriptions + '</span>'
                        //        + '</h3>'
                        //    + '<div class="picture-container" id="divImage' + item.PictureID + '"></div>';
                        builder += '<h3 class="header">'
                                + '<span class="accordion-icon"><img class="right icon-delete" id="delImage' + item.PictureID + '" src="/Content/themes/base/images/trash.png"></span>'
                                + '<span><img class="picture-sizing" id="picture' + item.PictureID + '" src="//eplatfront.villagroup.com' + item.Path + item.File + '?width=150&height=75&mode=crop"></span>'
                                + '<span class="picture-descriptions" id="pictureDescriptions">Descriptions: ' + item.Descriptions + '</span>'
                                + '</h3>'
                            + '<div class="picture-container" id="divImage' + item.PictureID + '"></div>';
                    });
                    PICTURE.fillDivPicturesContainer(parentClass, builder);
                    $(parentClass + '.picture-uploader').fineUploader({
                        request: {
                            endpoint: '/Pictures/UploadPicture',
                            params: {
                                path: path,
                                sysItemType: $(parentClass + '.picture-info-item-type').val()
                            }
                        },
                        multiple: true,
                        failedUploadTextDisplay: {
                            mode: 'default'
                        }
                    });
                }
            });
        });
    }

    function deletePicture(parentClass, pictureID) {
        $.ajax({
            url: '/Pictures/RemovePicture',
            cache: false,
            type: 'POST',
            data: { pictureID: pictureID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    $(parentClass + '#divImage' + data.ItemID).prev('h3').remove();
                    $(parentClass + '#divImage' + data.ItemID).prev('span').remove();
                    $(parentClass + '#divImage' + data.ItemID).remove();
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var GetImagesPerItemType = function (itemType, itemID, flag, parentClass) {
        if (parentClass == undefined) {
            PICTURE._class = '';
            parentClass = '';
        }
        if ($('.ul-pictures-gallery').length > 0) {
            $.ajax({
                url: '/Pictures/GetImagesPerItemType',
                cache: false,
                type: 'POST',
                data: { sysItemType: itemType, itemID: itemID },
                success: function (data) {
                    if (flag == undefined) {
                        $(parentClass + '.ul-pictures-gallery').empty();
                    }
                    var builder = '';
                    if (data.length > 0) {
                        $(parentClass + '.li-no-pictures').remove();
                        $.each(data, function (index, item) {
                            if ($(parentClass + '#liPicture' + item.PictureID).length == 0) {
                                builder += '<li id="liPicture' + item.PictureID + '">'
                                    + '<span><img id="picture' + item.PictureID + '" class="ui-draggable picture-sizing"'
                                    + 'src="//eplatfront.villagroup.com' + item.Path + item.File + '?width=150&height=75&mode=crop" >'
                                    //+ 'src="//eplatfront.villagroup.com' + item.Path + item.File + '" >' //modified line to cut images
                                    + '</span>'
                                    + '<img class="delete-picture right" src="/Content/themes/base/images/trash.png"/>'
                                    + '<input class="main-indicator right" type="checkbox"';
                                if (item.Main == true) {
                                    if (flag == undefined && $(parentClass + '.ul-pictures-gallery').find('.main-indicator:checked').length == 0) {
                                        builder += 'checked="checked" ';
                                    }
                                }
                                builder += '/></li>';
                            }
                        });
                    }
                    else {
                        if (flag == undefined) {
                            builder += '<li id="liNoPictures" class="picture-gallery li-no-pictures">Drop Pictures Here</li>';
                        }
                    }
                    $(parentClass + '.ul-pictures-gallery').append(builder);
                    $(parentClass + '.ul-pictures-gallery').droppable({ accept: '.ui-draggable', greedy: true });
                    $(parentClass + '.ul-pictures-gallery').sortable({ axis: 'y' }).disableSelection();
                    PICTURE.bindPicturesGalleryEvents();
                    $(parentClass + '.ul-pictures-gallery').unbind('drop').on('drop', function (e, ui) {
                        var flag = true;
                        $(parentClass + '.ul-pictures-gallery li').each(function () {
                            if ($(this).attr('id').toLowerCase() == 'li' + $(ui.draggable)[0].id) {
                                flag = false;
                            }
                        });
                        if (flag) {
                            $(parentClass + '.li-no-pictures').remove();
                            var builder = '<li id="liP' + $(ui.draggable)[0].id.toString().substr(1, $(ui.draggable)[0].id.length) + '">'
                                + '<span>' + $(ui.draggable)[0].outerHTML.toString() + '</span>'
                                + '<img class="delete-picture right" src="/Content/themes/base/images/trash.png"/>'
                                + '<input class="main-indicator right" type="checkbox"';
                            if ($(parentClass + '.ul-pictures-gallery li').length == 0) {
                                builder += 'checked="checked" ';
                            }
                            builder += '/></li>';
                            $(this).append(builder);
                            $(parentClass + '.ul-pictures-gallery').sortable({ axis: 'y' }).disableSelection();
                            PICTURE.bindPicturesGalleryEvents();
                        }
                        else {
                            UI.messageBox(-1, "Picture already exists in gallery", null, null);
                        }
                    });

                }
            });
        }
    }

    var bindPicturesGalleryEvents = function () {
        $('.delete-picture').unbind('click').bind('click', function (e) {
            var parentClass = $(this).parents('fieldset').children('div').attr('class');
            parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
            if ($(parentClass + '.ul-pictures-gallery li').length == 1)
                $(parentClass + '.ul-pictures-gallery').append('<li id="liNoPictures" class="picture-gallery li-no-pictures">Drop Pictures Here</li>');
            $(parentClass + '#' + $(e.target).parents('li').first().attr('id')).remove();
            if (!$(parentClass + '.ul-pictures-gallery li').first().children('.main-indicator').is(':checked'))
                $(parentClass + '.ul-pictures-gallery li').first().find('.main-indicator').trigger('click');
        });

        $('.main-indicator').unbind('click').bind('click', function (e) {
            var parentClass = $(this).parents('fieldset').children('div').attr('class');
            parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
            if ($(this).is(':checked')) {
                var li = $(e.target).parents('li').first();
                $(e.target).parents('li').first().remove();
                $(parentClass + '.ul-pictures-gallery').find('input:checkbox').each(function () {
                    $(this).attr('checked', false);
                });
                $(parentClass + '.ul-pictures-gallery').prepend(li);
                PICTURE.bindPicturesGalleryEvents();
            }
        });

        $('.ul-pictures-gallery').unbind('sortstop').bind('sortstop', function (e, ui) {
            var parentClass = $(this).parents('fieldset').children('div').attr('class');
            parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
            $(ui.item[0].parentElement).find('.main-indicator').each(function () {
                $(this).attr('checked', false);
            }).first().trigger('click');
        });
    }

    var fillDivPicturesContainer = function (parentClass, builder) {
        $(parentClass + '.div-pictures-container').append(builder);
        if ($(parentClass + '.div-pictures-container').accordion()) {
            $(parentClass + '.div-pictures-container').accordion('destroy');
        }
        $(parentClass + '.div-pictures-container').accordion({
            heightStyle: 'content',
            icons: false,
            header: '.header',
            animate: false,
            collapsible: true,
            active: false
        });
        $(parentClass + '.div-pictures-container').siblings().show();
        $(parentClass + '.div-pictures-container img:not(.icon-delete)').draggable({ helper: 'clone' });
        $(parentClass + '.div-pictures-container').unbind('accordionbeforeactivate').on('accordionbeforeactivate', function (e, ui) {
            $(ui.oldPanel).empty();
            $(ui.newPanel).unbind('load').load('/Pictures/RenderPictureDescription', function () {
                $(parentClass + '.picture-description-info-picture-id').val($(ui.newPanel).attr('id').substr(8));
                UI.legendClickBind();
                UI.adjustLegends();
                PICTURE.bindPicturesContainerEvents($(ui.newPanel).attr('id').substr(8), parentClass);
            });
        });
        $(parentClass + '.icon-delete').unbind('click').on('click', function (e) {
            UI.confirmBox('Do you confirm you want to proceed?', deletePicture, [parentClass, $(e.target).attr('id').substr(8)]);
        });
    }

    var savePictureDescriptionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if ($(PICTURE._class + '.li-no-picture-descriptions').length > 0) {
                $(PICTURE._class + '.ul-picture-descriptions').empty();
            }
            if (data.ResponseMessage == 'Picture Description Saved') {
                var builder = '<li id="liPictureDescription' + data.ItemID + '" >'
                + $(PICTURE._class + '.picture-description-info-culture option:selected').text()
                + '<img class="right" id="delPictureDescription' + data.ItemID + '" src="/Content/themes/base/images/trash.png"></li>';
                $(PICTURE._class + '.ul-picture-descriptions').append(builder);
                $(PICTURE._class + '.frm-picture-description').clearForm();
                UI.ulsHoverEffect('ulPictureDescriptions');
                PICTURE.makeUlPictureDescriptionsRowsSelectable();
            }
            else {
                $(PICTURE._class + '#liPictureDescription' + data.ItemID)[0].firstChild.textContent = $(PICTURE._class + '.picture-description-info-culture option:selected').text();
            }
            $(PICTURE._class + '#liPictureDescription' + data.ItemID).parents('div').prev('h3').find('.picture-descriptions')[0].childNodes[0].textContent = 'Descriptions: ' + $(PICTURE._class + '.ul-picture-descriptions li').length;
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var bindPicturesContainerEvents = function (pictureID, parentClass) {
        $('.btn-save-picture-description').unbind('click').on('click', function (e) {
            var parentClass = $(this).parents('fieldset').parents('fieldset').children('div').attr('class');
            parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
            PICTURE._class = parentClass;
            $.validator.unobtrusive.parse($(parentClass + '.frm-picture-description')); //validator needs to be recreated for dynamic content
            var flag = true;
            $(parentClass + '.ul-picture-descriptions li').each(function () {
                if ($(this).text() == $(parentClass + '.picture-description-info-culture option:selected').text() && !$(this).hasClass('selected-row'))
                    flag = false;
            });
            if (flag) {
                $(parentClass + '.frm-picture-description').data('validator').settings.ignore = '.ignore-validation';
                $(parentClass + '.frm-picture-description').submit();
            }
            else
                UI.messageBox(-1, "Description Language already exists", null, null);
        });

        $.ajax({
            url: '/Pictures/GetPictureDescriptions',
            cache: false,
            type: 'POST',
            data: { pictureID: pictureID },
            success: function (data) {
                $(parentClass + '.ul-picture-descriptions').empty();
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="liPictureDescription' + item.PictureDescriptionInfo_PictureDescriptionID + '">'
                            + $(parentClass + '.picture-description-info-culture option[value="' + item.PictureDescriptionInfo_Culture + '"]').text()
                            + '<img id="delPictureDescription' + item.PictureDescriptionInfo_PictureDescriptionID
                            + '" src="/Content/themes/base/images/trash.png" class="right"/></li>';
                    });
                    $(parentClass + '.ul-picture-descriptions').append(builder);
                    UI.ulsHoverEffect('ulPictureDescriptions');
                    PICTURE.makeUlPictureDescriptionsRowsSelectable();
                }
                else {
                    builder += '<li id="liNoPictureDescriptions" class="li-no-picture-descriptions">No Descriptions</li>';
                    $(parentClass + '.ul-picture-descriptions').append(builder);
                }
            }
        });
    }

    function deletePictureDescription(parentClass, pictureDescriptionID) {
        $.ajax({
            url: '/Pictures/DeletePictureDescription',
            cache: false,
            type: 'POST',
            data: { pictureDescriptionID: pictureDescriptionID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($(parentClass + '#liPictureDescription' + pictureDescriptionID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $(parentClass + '#liPictureDescription' + data.ItemID).parents('div').prev('h3').find('.picture-descriptions')[0].childNodes[0].textContent = 'Descriptions: ' + ($(parentClass + '.ul-picture-descriptions li').length - 1);
                    $(parentClass + '#liPictureDescription' + data.ItemID).remove();
                    if ($(parentClass + '.ul-picture-descriptions li').length == 0) {
                        $(parentClass + '.ul-picture-descriptions').append('<li id="liNoPictureDescriptions" class="li-no-picture-descriptions">No Descriptions</li>');
                    }
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var makeUlPictureDescriptionsRowsSelectable = function () {
        $('.ul-picture-descriptions li').unbind('click').on('click', function (e) {
            var parentClass = $(this).parents('fieldset').children('div').attr('class');
            parentClass = parentClass == undefined ? '' : '.' + parentClass + ' ';
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(e.target).parent('ul').find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $(PICTURE._class + '.picture-description-info-picture-description-id').val($(e.target).attr('id').substr(20));
                    $.ajax({
                        url: '/Pictures/GetPictureDescription',
                        cache: false,
                        type: 'POST',
                        data: { pictureDescriptionID: $(PICTURE._class + '.picture-description-info-picture-description-id').val() },
                        success: function (data) {
                            $(PICTURE._class + '.picture-description-info-alt').val(data.PictureDescriptionInfo_Alt);
                            $(PICTURE._class + '.picture-description-info-description').val(data.PictureDescriptionInfo_Description);
                            $(PICTURE._class + '.picture-description-info-culture option[value="' + data.PictureDescriptionInfo_Culture + '"]').attr('selected', true);
                            UI.expandFieldset('fdsPictureDescription');
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deletePictureDescription, [parentClass, $(e.target).attr('id').substr(21)]);
            }
        });
    }

    //not used
    function deletePictureFromGallery(pictureID) {
        var id = $(PICTURE._class + '#' + pictureID).find('img').first().attr('id');
        $.ajax({
            url: '/Pictures/RemovePictureFromGallery',
            cache: false,
            type: 'POST',
            data: { sysItemType: $(PICTURE._class + '#PictureInfo_ItemType').val(), itemID: $(PICTURE._class + '#PictureInfo_ItemID').val(), pictureID: id },
            success: function (data) {
                var duration = data.ResponseType > 0 ? data.ResponseType : null;
                if (data.ResponseType > 0)
                    $(PICTURE._class + '#' + pictureID).remove();
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    return {
        init: init,
        getTree: getTree,
        loadPicturesTree: loadPicturesTree,
        destination: destination,
        getItemNames: getItemNames,
        getGalleryName: getGalleryName,
        GetImagesPerItemType: GetImagesPerItemType,
        fillDivPicturesContainer: fillDivPicturesContainer,
        bindPicturesGalleryEvents: bindPicturesGalleryEvents,
        bindPicturesContainerEvents: bindPicturesContainerEvents,
        bindClickEventToPicturesTree: bindClickEventToPicturesTree,
        savePictureDescriptionSuccess: savePictureDescriptionSuccess,
        makeUlPictureDescriptionsRowsSelectable: makeUlPictureDescriptionsRowsSelectable
    }
}();