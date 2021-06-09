Number.prototype.pad = function (size) {
    var s = String(this);
    while (s.length < (size || 2)) { s = "0" + s; }
    return s;
}

var UI = function () {
    var init = function () {
        //tiles
        $('.tile-package').hover(function () {
            $(this).find('img').css('margin-top', '-165px');
        }, function () {
            $(this).find('img').css('margin-top', '0');
        });

        /*
        $('.tile-package a[title="Puerto Vallarta All Inclusive"]').eq(0).parent().parent().prepend('<img src="/content/themes/dvh/images/4july/banderas.png" class="bf-tag" />');
        $('.tile-package a[title="Cabo All Inclusive"]').eq(0).parent().parent().prepend('<img src="/content/themes/dvh/images/4july/banderas.png" class="bf-tag" />');
        $('.tile-package a[title="Cancun Palmar All Inclusive"]').eq(0).parent().parent().prepend('<img src="/content/themes/dvh/images/4july/banderas.png" class="bf-tag" />');
        $('.tile-package a[title="Loreto All Inclusive Package"]').eq(0).parent().parent().prepend('<img src="/content/themes/dvh/images/4july/banderas.png" class="bf-tag" />');
        */

        /*$('.tile-package a[title="Tulum Lite All Inclusive"]').eq(0).parent().parent().prepend('<img src="/content/themes/dvh/images/tag-sold-out.png" class="egg-tag" />');
        $('.tile-package a[title="Playacar All Inclusive"]').eq(0).parent().parent().prepend('<img src="/content/themes/dvh/images/tag-sold-out.png" class="egg-tag" />');
        $('.tile-package a[title="Coco Beach All Inclusive"]').eq(0).parent().parent().prepend('<img src="/content/themes/dvh/images/tag-sold-out.png" class="egg-tag" />');
        $('.tile-package a[title="Reef 28 All Inclusive"]').eq(0).parent().parent().prepend('<img src="/content/themes/dvh/images/tag-sold-out.png" class="egg-tag" />');*/

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

        //$('body').append('<div id="special2" style="display: none; position: fixed; bottom: -175px; left: 10px; box-shadow: 0 0 5px #252525;"><span alt="close" class="close" style="background-color: white; margin-left: -50px; padding: 0.4em 0.5em .4em .7em; opacity: 0.5; text-align: center;">&times;</span><img src="/content/themes/dvh/images/spring/spring-giveaways-2018.png" alt="Spring Giveaways" style="width: 320px;" /></div>');
        //    $('#special2').fadeIn('fast').animate({ bottom: '10px' }, 300);
        //    $('#special2 .close').off('click').on('click', function () {
        //        $('#special2').fadeOut('fast');
        //    });

        ////banner easter

        //    ////console.log('easter');
        //$('body').append('<div id="special" style="display: none; position: fixed; bottom: -175px; right: 16px; z-index: 100;"><span alt="close" class="close" style="background-color: white; margin-left: -50px; padding: 0.4em 0.5em .4em .7em; opacity: 0.5; text-align: center;">&times;</span><img src="/content/themes/dvh/images/black-friday/black-friday-pop-up.jpg" alt="Black Friday 2018" style="width: 238px;" /></div>');
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

        //setTimeout(UI.centralBanner, 3000);

        $('.whatsapp').on('click', function () {
            if ($('.whatsapp').css('right') == '-170px') {
                $('.whatsapp').animate({ right: 10 }, 300);
            } else {
                $('.whatsapp').animate({ right: -170 }, 300);
            }

        });

        //$('.tile-package a[title="Grand Vallarta All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/dvh/images/limited-time-special.png" class="limited-time-special-1" />');
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

    //banner central    
    var centralBanner = function () {
        if ((sessionStorage.CH2019 == undefined || sessionStorage.CH2019 <= 2) && location.href.indexOf("congratulations") < 0) {
            $('body').append('<div id="centralBanner" style="background-color: rgba(0,0,0,.8); position:fixed; left: 0; top: 0; width: 100%; height: 100%; text-align: center; z-index:1031; padding-top: 150px;"></span><img src="/content/themes/dvh/images/4july/4-july-2020.jpg" alt="4th of July Special" style="margin: 0 auto; max-width: 850px; width: 100%;" class="img-responsive" /></div>');

            //<span class="bfcd-countdown"><span class="label">DAYS</span><span class="bfcd-day"></span><span class="label">HOURS</span><span class="bfcd-hour"></span ><span class="label">MINUTES</span><span class="bfcd-minute"></span><span class="label">SECONDS</span><span class="bfcd-second"></span></span>

            //$(window).resize(function () {
            //    //console.log('window width:' + $(window).width());
            //    //console.log('banner width:' + $('#centralBanner>img').width());
            //    //console.log($(window).width() / 2);
            //    //console.log($('#centralBanner').width() / 2);
            //    //console.log(($(window).width() / 2) - ($('#centralBanner>img').width() / 2));

            //    $("#centralBanner").css('left', ($(window).width() / 2) - ($('#centralBanner>img').width() / 2));
            //});


            //console.log($(window).width() / 2) - ($('#centralBanner>img').width() / 2);
            //$("#centralBanner").css('left', ($(window).width() / 2) - ($('#centralBanner>img').width() / 2));
            $("#centralBanner").fadeIn('fast');
            $('#centralBanner').off('click').on('click', function () {
                $('#centralBanner').fadeOut('fast');
            });
            $('#centralBanner>img').off('click').on('click', function () {
                $('#centralBanner').fadeOut('fast');
            });

            if (sessionStorage.CH2019 == undefined) {
                sessionStorage.CH2019 = 2;
            } else {
                sessionStorage.CH2019++;
            }
        }
        /*
        $('body').append('<div id="special" style="display: none; position: fixed; bottom: -175px; right: 16px; z-index: 100;"><span alt="close" class="close" style="background-color: white; margin-left: -50px; padding: 0.4em 0.5em .4em .7em; opacity: 0.5; text-align: center;">&times;</span><img src="/content/themes/dvh/images/march/marzo2020-banner.jpg" alt="March Special" style="width: 290px;" /></div>');
        $('#special .close').off('click').on('click', function () {
            $('#special').fadeOut('fast');
        });
        $('#special').fadeIn('fast').animate({ bottom: '35px' }, 300);
        */

        // $('body').append('<div id="special" class="bfcd-fix"><div>COUNTDOWN FOR BLACK FRIDAY</div><span class="bfcd-countdown"><span class="label">DAYS</span><span class="bfcd-day"></span><span class="label">HOURS</span><span class="bfcd-hour"></span ><span class="label">MINUTES</span><span class="bfcd-minute"></span><span class="label">SECONDS</span><span class="bfcd-second"></span></span></div>');
        //$('#special').fadeIn('fast').animate({ bottom: '35px' }, 300);

        //countdown
        /*var countDownDate = new Date("November 28, 2019 09:00:00").getTime();
        console.log(countDownDate);
        $('#bfcd2018').show();

        // Update the count down every 1 second
        var x = setInterval(function () {

            // Get todays date and time
            var now = new Date().getTime();

            // Find the distance between now and the count down date
            var distance = countDownDate - now;
            console.log(distance);

            // Time calculations for days, hours, minutes and seconds
            var days = Math.floor(distance / (1000 * 60 * 60 * 24));
            var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            var seconds = Math.floor((distance % (1000 * 60)) / 1000);

            // Display the result in the element with id="demo"
            $('.bfcd-day').html(days.pad());
            $('.bfcd-hour').html(hours.pad());
            $('.bfcd-minute').html(minutes.pad());
            $('.bfcd-second').html(seconds.pad());

            //document.getElementById("demo").innerHTML = days + "d " + hours + "h " + minutes + "m " + seconds + "s ";

            // If the count down is finished, write some text 
            //if (distance < 0) {
            //    clearInterval(x);
            //    $('#centralBanner').remove();
            //    $('#bfcd2018').remove();
            //    //document.getElementById("demo").innerHTML = "EXPIRED";
            //}
        }, 1000);*/
    };

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