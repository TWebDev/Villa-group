var UI = function () {
    var init = function () {
        //tiles
        //$('.tile-package').hover(function () {
        //    $(this).find('img').css('margin-top', '-165px');
        //}, function () {
        //    $(this).find('img').css('margin-top', '0');
        //});

        //pop up click
        $('.banner-free-getaway').on('click', function () {
            if ($('.banner-free-getaway').hasClass('active')) {
                $('.banner-free-getaway').removeClass('active');
                $('#imgFreeGetaway').removeClass('active');
                $('#spnCloseFreeGetaway').fadeOut('fast', function () {
                    $('.banners').css('height', '60px');
                });
            } else {
                $('.banner-free-getaway').addClass('active');
                $('#imgFreeGetaway').addClass('active');
                $('#spnCloseFreeGetaway').fadeIn('fast');
                $('.banners').css('height', '370px');
            }
        });

        $('#spnCloseFreeGetaway').on('click', function () {
            $('.banner-free-getaway').trigger('click');
        });

        //pop up automático
        if (lsTest() === true) {
            if (localStorage.GetawayPopup == undefined) {
                $('.banner-free-getaway').delay(5000).queue(function () {
                    $('.banner-free-getaway').trigger('click').dequeue();
                });
                localStorage.GetawayPopup = "true";
            }
        }

        $('#spnGoPackages').on('click', function () {
            var targetOffset = $("#packages").offset().top - 50;
            $('html,body').animate({ scrollTop: targetOffset }, 500);
        });

        //fix secondary buttons
        $(window).on("scroll", function (e) {
            if ($(document).scrollTop() > 431) {
                $('.banners').addClass("fixed");
            } else {
                $('.banners').removeClass("fixed");
            }
        });

        //$('body').append('<div id="special2" style="display: none; position: fixed; bottom: -175px; left: 10px; box-shadow: 0 0 5px #252525;"><span alt="close" class="close" style="background-color: white; margin-left: -50px; padding: 0.4em 0.5em .4em .7em; opacity: 0.5; text-align: center;">&times;</span><img src="/content/themes/dvh/images/cinco-de-mayo/cinco-de-mayo-2018.png" alt="Cinco de Mayo Special" style="width: 320px;" /></div>');
        //    $('#special2').fadeIn('fast').animate({ bottom: '10px' }, 300);
        //    $('#special2 .close').off('click').on('click', function () {
        //        $('#special2').fadeOut('fast');
        //    });

        ////banner easter

        //    ////console.log('easter');
        //$('body').append('<div id="special" style="display: none; position: fixed; bottom: -175px; right: 16px; box-shadow: 0 0 20px #252525;"><span alt="close" class="close" style="background-color: white; margin-left: -50px; padding: 0.4em 0.5em .4em .7em; opacity: 0.5; text-align: center;">&times;</span><img src="/content/themes/vdai/images/halloween/halloween-2018.png" alt="Halloween Sale" style="width: 238px;" /></div>');
        //$('#special').fadeIn('fast').animate({ bottom: '35px' }, 300);
        //$('#special .close').off('click').on('click', function () {
        //    $('#special').fadeOut('fast');
        //});
        /*(UI.getCookie('dvh_campaign') != undefined ? UI.getCookie('dvh_campaign') : "gsh") +*/

        /*videos*/
        //Loreto Golf
        if (window.location.href.indexOf('https://www.discountvacationhotels.com/packages/loreto/golf-package') >= 0) {
            $('.package-rating').after('<div id="golfVideo" style="display:none; margin: -18px -15px 30px -15px;"><div id="bgvid" class="embed-responsive embed-responsive-16by9"><video class="embed-responsive-item" autoplay loop><source src="/Content/themes/dvh/videos/golf_intro2.mp4" type="video/mp4"></video></div></div>');
            $('#golfVideo').slideDown('fast');
            $('.big-header-right').after('<div class="embed-responsive embed-responsive-16by9" style="margin-top: 20px;"><iframe class="embed-responsive-item" src="https://www.youtube.com/embed/72W7swYyvA8?rel=0&amp;controls=0&amp;showinfo=0" frameborder="0" allowfullscreen></iframe></div>');
            $('.big-header-right').after('<div class="embed-responsive embed-responsive-16by9" style="margin-top: 20px;"><iframe class="embed-responsive-item" src="https://www.youtube.com/embed/96iYTlFOBhs?rel=0&amp;controls=0&amp;showinfo=0" frameborder="0" allowfullscreen></iframe></div>');
        }

        //setTimeout(UI.centralBanner, 5000);
    }

    var centralBanner = function () {
        if (sessionStorage.BF2019 == undefined || sessionStorage.BF2019 < 3) {
            $('body').append('<div id="centralBanner" style="background-color: rgba(0,0,0,.8); position:fixed; left: 0; top: 0; width: 100%; height: 100%; text-align: center; z-index:1031; padding-top: 50px;"><img src="/Content/themes/vdai/images/mothers-day/moms2020.jpg" alt="Mothers Special" style="box-shadow: 0 0 30px #111; margin: 0 auto;" class="img-responsive" /></div>');

            $("#centralBanner").fadeIn('fast');
            $('#centralBanner').off('click').on('click', function () {
                $('#centralBanner').fadeOut('fast');
            });
            $('#centralBanner>img').off('click').on('click', function () {
                $('#centralBanner').fadeOut('fast');
            });

            if (sessionStorage.BF2019 == undefined) {
                sessionStorage.BF2019 = 1;
            } else {
                sessionStorage.BF2019++;
            }
        } /*else {
            $('body').append('<div id="special" style="display: none; position: fixed; bottom: -175px; right: 16px; box-shadow: 0 0 20px #252525;"><span alt="close" class="close" style="background-color: white; margin-left: -50px; padding: 0.4em 0.5em .4em .7em; opacity: 0.5; text-align: center;">&times;</span><img src="/Content/themes/vdai/images/july-4/4julio2019.jpg" alt="July Special" style="width: 420px;" /></div>');
            $('#special').fadeIn('fast').animate({ bottom: '35px' }, 300);
            $('#special .close').off('click').on('click', function () {
                $('#special').fadeOut('fast');
            });
        }*/

        //$('.tile-package a[title="Palmar Cabo All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/easter/tag-transportation.png" class="spring" />');
        //$('.tile-package a[title="Cabo Inn All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/easter/tag-transportation.png" class="spring" />');
        //$('.tile-package a[title="Cabo El Faro Package"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/easter/tag-transportation.png" class="spring" />');
        //$('.tile-package a[title="Lite All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/halloween/calabaza.png" class="spring" />');
        //$('.tile-package a[title="Palmar Vallarta All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/easter/tag-upgrade.png" class="spring" />');
        /*$('.tile-package a[title="Sens All Inclusive Package"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/halloween/calabaza.png" class="spring" />');
        $('.tile-package a[title="Cancun Suites All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/halloween/calabaza.png" class="spring" />');
        $('.tile-package a[title="Coral and Turquoise"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/halloween/calabaza.png" class="spring" />');
        $('.tile-package a[title="Maya Grand Resort"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/halloween/calabaza.png" class="spring" />');
        $('.tile-package a[title="Esmeralda All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/halloween/calabaza.png" class="spring" />');
        $('.tile-package a[title="Ocean Paradise All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/vdai/images/halloween/calabaza.png" class="spring" />');*/
    };

    var lsTest = function () {
        var test = 'test';
        try {
            localStorage.setItem(test, test);
            localStorage.removeItem(test);
            return true;
        } catch (e) {
            return false;
        }
    }

    var getCookie = function (name) {
        var value = "; " + document.cookie;
        var parts = value.split("; " + name + "=");
        if (parts.length == 2) return parts.pop().split(";").shift();
    }

    return {
        init: init,
        lsTest: lsTest,
        getCookie: getCookie,
        centralBanner: centralBanner
    }
}();

$(function () {
    UI.init();
});