$.fn.multiNav = function () {
    var site = function () {
        this.navLi = $('#menu li').children('ul').hide().end();
        this.init();
    };

    site.prototype = {

        init: function () {
            this.setMenu();
        },

        // Enables the slidedown menu, and adds support for IE6

        setMenu: function () {

            $.each(this.navLi, function () {
                if ($(this).children('ul')[0]) {
                    $(this)
                        .append('<span />')
                        .children('span')
                            .addClass('hasChildren')
                }
            });

            //this.navLi.hover(function () {
            //    // mouseover
            //    $(this).find('> ul').stop(true, true).slideDown('slow', 'easeOutBack');
            //}, function () {
            //    // mouseout
            //    $(this).find('> ul').stop(true, true).hide();
            //});

            $('#menu>li').off('mouseover').on('mouseover', function () {
                $(this).siblings().find('>ul').hide();
                $(this).find('>ul').stop(true, true).show();
                $(this).find('>ul').off('mouseout').on('mouseout', function (e) {
                    $('#menu>li>ul.submenu').not(this).hide();
                    if ($(e.target).is('ul')) {
                        $(this).hide();
                    }
                });
            });
            $('#menu>li>ul>li').off('mouseover').on('mouseover', function () {
                $(this).find('>ul').stop(true, true).show();
            });
        }
    }
    new site();
}