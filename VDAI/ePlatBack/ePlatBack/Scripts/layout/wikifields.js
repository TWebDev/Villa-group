/// <reference path="../jquery-1.7.1.min.js" />

$(function () {
    WikiFields.init();
});

var WikiFields = function () {
    var culture = "en-US";
    var type = "1";
    var wikiID = 0;

    var init = function () {
        var DataObject = {
            id: getFieldsetIDs()
        }
        $.post('/Home/WikiFieldsWithContent/', DataObject, function (data) {
            $('fieldset').each(function (f, field) {
                var icon = '<div class="wiki-icon" data-for="' + $(this).attr('id') + '">?</div>';
                $(this).children('legend:first-child').prepend(icon);
            });
            $('.wiki-icon').on('click', function () {
                $.fancybox.open([
                    {
                        type: 'ajax',
                        href: '/Home/WikiField/' + $(this).attr('data-for')
                    }
                ], {
                    'width': 640,
                    'height': 600,
                    'autoSize': false,
                    'openEffect': 'fadeIn',
                    'closeEffect': 'fadeOut',
                    'openEasing': 'easeOutBack',
                    'closeEasing': 'easeInBack',
                    'title': null,
                    'closeBtn': true,
                    'afterShow': function () {
                        WikiFields.loaded();
                    },
                    'beforeShow': function () {
                        this.wrap.draggable();
                        $('.fancybox-overlay').css({
                            'overflow': 'hidden',
                            'overflow-y': 'hidden',
                        });
                    }
                });
            });

            if (data.Fields != "") {
                var fieldsWithContent = data.Fields.split(',');
                for (var i = 0; i < fieldsWithContent.length; i++) {
                    $('.wiki-icon[data-for=' + fieldsWithContent[i] + ']').addClass('with-content');
                }
            }
        }, 'json');
    }

    function getFieldsetIDs() {
        var fields = "";

        $('fieldset').each(function() {
            if (fields != "") {
                fields += ',';
            }
            fields += $(this).attr('id');
        });

        return fields;
    }

    var loaded = function () {
        WikiFields.setCulture();

        //crear estructura de historial del wiki
        $('.wiki-timeline .wiki-edition:last-child').addClass('active');
        $('.wiki-edition').off('click').on('click', function () {
            $(this).siblings().removeClass('active');
            $(this).addClass('active');
            $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki').hide();
            $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki').eq($(this).attr('data-index')).show();            
            $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki-author .wiki-author-name').html($(this).find('.wiki-edition-author-name').val());
            $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki-author .wiki-author-date').html($(this).find('.wiki-edition-author-date').val());
        });

        //agrega WYSIWYG al editor
        $('#wiki-content').ckeditor();

        //habilitar botones submenu
        $('.sub-menu-tabs li').off('click').on('click', function () {
            $(this).siblings().removeClass('active');
            $(this).addClass('active');
            WikiFields.type = $(this).attr('data-for');
            WikiFields.setCulture();
        })

        //eventos de wiki-editor
        $('#wiki-cancel').off('click').on('click', function () {
            $('#wiki-editor').hide();
            WikiFields.setCulture();
        });

        $('#wiki-save').off('click').on('click', function () {
            var DataObject = {
                WikiFieldID: WikiFields.WikiFieldID,
                Content: $('#wiki-content').val(),
                Type: WikiFields.type,
                Culture: WikiFields.culture,
                SysComponentID: $('#wiki-syscomponent').val()
            }
            $.post('/Home/SaveWikiField', DataObject, function (data) {
                if (WikiFields.WikiFieldID != 0) {
                    //editar el existente
                    $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki:last .wiki-content').eq(0).html($('#wiki-content').val());
                } else {
                    //agregar el nuevo a la lista
                    $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + ']').append('<div class="wiki" data-wiki="' + data.ItemID + '"><div class="wiki-content">' + $('#wiki-content').val() + '</div></div>');
                    assignWikiEditEvents();
                }
                WikiFields.setCulture();
                $('#wiki-editor').hide();
            }, 'json');
        });

        //evento de wiki-list para abrir editor
        assignWikiEditEvents();
    }

    function assignWikiEditEvents() {
        $('.wiki').off('click').on('click', function () {
            $('.wiki-list').hide();
            //llenar el ckeditor
            //si hay wikis editables
            if ($('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki:last').attr('data-wiki') != "0") {
                //es del mismo usuario, lo puede editar
                WikiFields.WikiFieldID = $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki:last').attr('data-wiki');
                $('#wiki-content').val($('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki:last .wiki-content').html());
            } else {
                //es de otro usuario o no hay wiki, crear nuevo
                WikiFields.WikiFieldID = 0;
                $('#wiki-content').val($('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki:last .wiki-content').html());
            }

            //mostrar el ckeditor
            $('#wiki-editor').show();
        });
    }

    var setCulture = function (culture) {
        if (culture != null) {
            WikiFields.culture = culture;
        }

        $('#wiki-editor').hide();
        $('.wiki-culture').removeClass('active');
        $('.wiki-culture[data-for=' + WikiFields.culture + ']').addClass('active');

        //mostrar layout de wikifields de acuerdo al lenguaje seleccionado
        $('.sub-menu-tabs').hide();
        $('.sub-menu-tabs.' + WikiFields.culture).show();
        $('.sub-menu-tabs.' + WikiFields.culture + ' li').removeClass('active');
        $('.sub-menu-tabs.' + WikiFields.culture + ' li[data-for=' + WikiFields.type + ']').addClass('active');

        $('.wiki-list').hide();
        $('div[data-type=' + WikiFields.type + '].' + WikiFields.culture).show();
        $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki').hide();
        $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki:last').show();

        $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki-edition').removeClass('active');
        $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki-edition:last-child').addClass('active');
        $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki-author .wiki-author-name').html($('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki-edition:last-child .wiki-edition-author-name').val());
        $('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki-author .wiki-author-date').html($('.' + WikiFields.culture + '.wiki-list[data-type=' + WikiFields.type + '] .wiki-edition:last-child .wiki-edition-author-date').val());
    }

    return {
        init: init,
        culture: culture,
        setCulture: setCulture,
        type: type,
        loaded: loaded,
        wikiID: wikiID
    }
}();