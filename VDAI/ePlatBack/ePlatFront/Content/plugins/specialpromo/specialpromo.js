//$(function () {
//    if ($('#SpecialPromoAd').length > 0) {
//        if(localStorage.watchedPromo != "true"){
//            $.getJSON("/Controls/GetPromo", null, function (data) {
//                if (data.City != "") {
//                    var banner = data.BannerAd;
//                    banner = banner.replace("$PhoneDesktop", data.PhoneDesktop);
//                    banner = banner.replace("$PhoneMobile", data.PhoneMobile);
//                    banner = banner.replace(/\$City/g, data.City);
//                    $('#SpecialPromoAd').html(banner).delay(5000).show().animate({
//                        top: '0px'
//                    }, 300);
//                    $('#SpecialPromoAd .close').on('click', function () {
//                        $('#SpecialPromoAd').animate({ top: '-260px' }, 300);
//                        localStorage.watchedPromo = true;
//                    });
//                }
//            });
//        }
//    }
//});