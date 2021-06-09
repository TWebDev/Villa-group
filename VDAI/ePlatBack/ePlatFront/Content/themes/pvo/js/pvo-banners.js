$(function () {
    Banners.init();
});
let Banners = function () {
    var init = function () {
        $('.tile-package a[title="Xpu Ha Resort All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/sold-out_03.png" class="sold-out" />');
        $('.tile-package a[title="Cancun Bel All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/sold-out_03.png" class="sold-out" />');
        /*
        $('.tile-package a[title="Cancun Palmar All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/black-friday/tag-black-friday-2020.png" class="promotag" />');
        $('.tile-package a[title="Barcelo All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/black-friday/tag-black-friday-2020.png" class="promotag" />');
        $('.tile-package a[title="Faro All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/black-friday/tag-black-friday-2020.png" class="promotag" />');
        $('.tile-package a[title="San Jose All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/black-friday/tag-black-friday-2020.png" class="promotag" />');
        $('.tile-package a[title="Cancun Bel All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/black-friday/tag-black-friday-2020.png" class="promotag" />');
        $('.tile-package a[title="Grand Sens All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/black-friday/tag-black-friday-2020.png" class="promotag" />');
        $('.tile-package a[title="Xpu Ha Resort All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/black-friday/tag-black-friday-2020.png" class="promotag" />');
        $('.tile-package a[title="Esmeralda All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/black-friday/tag-black-friday-2020.png" class="promotag" />');  */      

        //setTimeout(Banners.centralBanner, 5000);
    };

    var centralBanner = function () {
        console.log('central');
        if (sessionStorage.BF2019 == undefined || sessionStorage.BF2019 < 3) {
            $('body').append('<div id="centralBanner" style="background-color: rgba(0,0,0,.8); position:fixed; left: 0; top: 0; width: 100%; height: 100%; text-align: center; z-index:1031; padding-top: 150px;"><img src="/Content/themes/pvo/images/black-friday/banner-black-friday-2020.jpg" alt="Black Friday Special" style="box-shadow: 0 0 30px #111; max-height:100%; class="img-responsive" /></div>');

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
        }
        /*SMALL
        $('body').append('<div id="special" style="display: none; position: fixed; bottom: -175px; right: 2px; z-index: 100;"><span alt="close" class="close" style="background-color: white; margin-left: -50px; padding: 0.4em 0.5em .4em .7em; opacity: 0.5; text-align: center;">&times;</span><img src="/Content/themes/pvo/images/cyber-monday/cyber-monday-2019-mini.jpg" alt="Cyber Monday Special" style="width: 200px;" /></div>');
        $('#special').fadeIn('fast').animate({ bottom: '35px' }, 300);
        $('#special .close').off('click').on('click', function () {
            $('#special').fadeOut('fast');
        });
        */
        //$('.tile-package a[title="Faro All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/presidents-day/tag.png" class="presidents" />');
        //$('.tile-package a[title="Grand Hotel & Spa All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/presidents-day/tag.png" class="presidents" />');
        //$('.tile-package a[title="Tulum Lite All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/presidents-day/tag.png" class="presidents" />');
        //$('.tile-package a[title="Cancun Palm All Inclusive"]').eq(0).parent().prepend('<img src="/content/themes/pvo/images/presidents-day/tag.png" class="presidents" />');
    };

    return {
        init: init,
        centralBanner: centralBanner
    }
}();

