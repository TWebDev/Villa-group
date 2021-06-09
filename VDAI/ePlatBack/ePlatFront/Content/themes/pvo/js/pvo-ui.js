$(function () {
    UI.init();
    if (window.location.hash == "#quoteRequest") {
        $('#aQuoteRequest').trigger('click');
    }
});

var UI = function () {
    var isiPad = navigator.userAgent.match(/iPad/i) != null;
    var pageWidth = 980;
    var scrolling = false;
    var menuOpen = false;
    var init = function () {
        if (!isiPad) {
            document.head.insertAdjacentHTML('beforeEnd', '<meta name="viewport" content="width=device-width, user-scalable = no" />');
        }

        //fancybox
        if ($(window).width() < 768) {
            $(".fancybox").fancybox({
                maxWidth: 800,
                maxHeight: 600,
                fitToView: false,
                width: '99%',
                height: '99%',
                autoSize: false,
                closeClick: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        } else {
            $(".fancybox").fancybox({
                maxWidth: 800,
                maxHeight: 600,
                fitToView: false,
                width: '70%',
                height: '70%',
                autoSize: false,
                closeClick: false,
                openEffect: 'none',
                closeEffect: 'none'
            });            
        }

        //summer special
        //cabo 2nd package
        //if (window.location.href.indexOf("packages/cabo-san-lucas") >= 0) {
        //    $('.tile-package').eq(1).append('<div class="summer-special">SUMMER SPECIAL</div>');
        //}
        

        //header
        $('.slide-title').click(function () {
            $('.slide-header').removeClass('selected-header')
            $('.slide-header').animate({
                width: '50px'
            }, { duration: 300, queue: false }, 'jswing');
            $(this).parent().addClass('selected-header');
            $(this).parent().animate({
                width: (UI.pageWidth - 150) + 'px'
            }, { duration: 300, queue: false }, 'jswing');
        });
        $(window).resize(function () {
            //redefine width of fixed elements
            setPageWidth();
        });
        setPageWidth();
        //scroll for menu
        if ($(window).width() < 768) {
            $('#showMenu').click(function () {
                if (menuOpen) {
                    closeMenu();
                } else {
                    openMenu();
                }
            });
            //$(window).scroll(function () {
            //    if ($(window).scrollTop() >= 200) {
            //        $('#showMenu').css({
            //            position: 'fixed',
            //            top: '59px'
            //        });
            //        $('#showMenu img').attr('src', '/Content/themes/dvh/images/down.png');
            //    } else {
            //        $('#showMenu').css({
            //            position: 'absolute',
            //            top: 'auto'
            //        });
            //        $('#showMenu img').attr('src', '/Content/themes/dvh/images/up.png');
            //    }
            //});
            //$(window).scroll($.debounce(250, function () {
                
            //    if ($(window).scrollTop() > 20 && $(window).scrollTop() < 199) {
            //        closeMenu();
            //    }
            //}));
        }

        function openMenu() {
            $('#menu').slideDown('fast');
            $('#showMenu img').addClass('rotated');
            menuOpen = true;
        }

        function closeMenu() {
            $('#menu').slideUp('fast');
            $('#showMenu img').removeClass('rotated');
            menuOpen = false;
        }

        $('.content-row').height($('.content-row').children().eq(0).height());
        enablePlaceHolders();

        //setTimeout(UI.centralBanner, 5000);
    }

    var centralBanner = function () {
        if (sessionStorage.CR2019 == undefined || sessionStorage.CR2019 < 3) {
            $('body').append('<div id="centralBanner" style="background-color: rgba(0,0,0,.8); position:fixed; left: 0; top: 0; width: 100%; height: 100%; text-align: center; z-index:1031; padding-top: 50px;"><img src="/content/themes/pvo/images/celebrating-2019/crusero-2019-web.jpg" alt="Celebrating 2019" style="box-shadow: 0 0 30px #111; margin: 0 auto; max-width:400px;" class="img-responsive" /></div>');

            $("#centralBanner").fadeIn('fast');
            $('#centralBanner').off('click').on('click', function () {
                $('#centralBanner').fadeOut('fast');
            });
            $('#centralBanner>img').off('click').on('click', function () {
                $('#centralBanner').fadeOut('fast');
            });

            if (sessionStorage.CR2019 == undefined) {
                sessionStorage.CR2019 = 1;
            } else {
                sessionStorage.CR2019++;
            }
        }
        $('body').append('<div id="special" style="display: none; position: fixed; bottom: -175px; right: 16px; z-index: 100;"><span alt="close" class="close" style="background-color: white; margin-left: -50px; padding: 0.4em 0.5em .4em .7em; opacity: 0.5; text-align: center;">&times;</span><img src="/content/themes/pvo/images/celebrating/crusero-2019.jpg" alt="Celebrating 2019" style="width: 238px;" /></div>');
        $('#special').fadeIn('fast').animate({ bottom: '35px' }, 300);
        $('#special .close').off('click').on('click', function () {
            $('#special').fadeOut('fast');
        });

        $('.tile-package a[title="Faro All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/christmas/regalo-1.png" class="christmas-1" />');
        $('.tile-package a[title="Grand Hotel Spa"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/christmas/regalo-2.png" class="christmas-2" />');
        $('.tile-package a[title="Tulum Lite All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/christmas/regalo-1.png" class="christmas-3" />');
        $('.tile-package a[title="Cancun Lite All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/christmas/regalo-2.png" class="christmas-4" />');
    };

    function enablePlaceHolders() {
        if (!Modernizr.input.placeholder) {

            $('[placeholder]').focus(function () {
                var input = $(this);
                if (input.val() == input.attr('placeholder')) {
                    input.val('');
                    input.removeClass('placeholder');
                }
            }).blur(function () {
                var input = $(this);
                if (input.val() == '' || input.val() == input.attr('placeholder')) {
                    input.addClass('placeholder');
                    input.val(input.attr('placeholder'));
                }
            }).blur();
            $('[placeholder]').parents('form').submit(function () {
                $(this).find('[placeholder]').each(function () {
                    var input = $(this);
                    if (input.val() == input.attr('placeholder')) {
                        input.val('');
                    }
                })
            });

        }
    }

    function setPageWidth() {
        if ($(window).width() < 980) {
            UI.pageWidth = $(window).width();
        } else {
            UI.pageWidth = 980;
        }
        $('.selected-header').width(UI.pageWidth-150);
    }

    var showContentLayer = function (index) {
        if ($(window).width() > 768) {
            $('html,body').animate({ scrollTop: $("#submenu").offset().top - 25 }, 200);
        }
        var w = $('.content-row').children().eq(0).width();
        $('.content-row').animate({
            marginLeft: '-' + (index * w) + 'px'
        }, { duration: 300, queue: false }, 'jswing');
        $('#submenu>li>a').removeClass('selected');
        $('#submenu').children().eq(index).children().eq(0).addClass('selected');
        $('.content-row').height($('.content-row').children().eq(index).height());
        switch (index) {
            case 0:
                //console.log('Offer');
                ga('send', 'event', 'tab', 'view', 'Offer', { 'page': window.location.pathname });
                break;
            case 1:
                //console.log('The Resort');
                ga('send', 'event', 'tab', 'view', 'The Resort', { 'page': window.location.pathname });
                break;
            case 2:            
                //console.log('Your Room');
                ga('send', 'event', 'tab', 'view', 'Your Room', { 'page': window.location.pathname });
                break;
            case 3:
                //console.log('Amenities');
                ga('send', 'event', 'tab', 'view', 'Amenities', { 'page': window.location.pathname });
                break;
            case 4:
                //console.log('FAQ');
                ga('send', 'event', 'tab', 'view', 'FAQ', { 'page': window.location.pathname });
                break;
            case 5:
                //console.log('Map');
                ga('send', 'event', 'tab', 'view', 'Map', { 'page': window.location.pathname });
                break;
            case 6:
                //console.log('Reviews');
                ga('send', 'event', 'tab', 'view', 'Reviews', { 'page': window.location.pathname });
                break;
        }
    }

    return {
        init: init,
        pageWidth: pageWidth,
        showContentLayer: showContentLayer
    }
}();