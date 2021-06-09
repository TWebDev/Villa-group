var DMV = function () {
    var playing = true;
    var currentPlaying = 0;
    /*
    var BgImages = [
        {
            image: '/Content/themes/dmv/images/bg_pv01.jpg',
            destination: 'Puerto Vallarta',
            roomImage: '/Content/themes/dmv/images/room_pv.jpg'
        },
        {
            image: '/Content/themes/dmv/images/bg_lo01.jpg',
            destination: 'Loreto',
            roomImage: '/Content/themes/dmv/images/room_lo.jpg'
        },
        {
            image: '/Content/themes/dmv/images/bg_or01.jpg',
            destination: 'Orlando',
            roomImage: '/Content/themes/dmv/images/room_or.jpg'
        },
        {
            image: '/Content/themes/dmv/images/bg_pv02.jpg',
            destination: 'Puerto Vallarta',
            roomImage: '/Content/themes/dmv/images/room_pv.jpg'
        },
        {
            image: '/Content/themes/dmv/images/bg_lo02.jpg',
            destination: 'Loreto',
            roomImage: '/Content/themes/dmv/images/room_lo.jpg'
        },
        {
            image: '/Content/themes/dmv/images/bg_or02.jpg',
            destination: 'Orlando',
            roomImage: '/Content/themes/dmv/images/room_or.jpg'
        }
    ];
    */

    var init = function () {
        //slide
        //sliding();
        $.vegas('slideshow', {
            backgrounds: [
              { src: '/Content/themes/dmv/images/bg_pv01.jpg' },
              { src: '/Content/themes/dmv/images/bg_lo01.jpg' },
              { src: '/Content/themes/dmv/images/bg_or01.jpg' },
              { src: '/Content/themes/dmv/images/bg_pv02.jpg' },
              { src: '/Content/themes/dmv/images/bg_lo02.jpg' },
              { src: '/Content/themes/dmv/images/bg_or02.jpg' }
            ],
            walk: function (step) {
                switch (step) {
                    case 0:
                    case 3:
                        selectDestination('Puerto Vallarta');
                        $('#rooms').css('background-image', 'url(/Content/themes/dmv/images/room_pv.jpg)');
                        break;
                    case 1:
                    case 4:
                        selectDestination('Loreto');
                        $('#rooms').css('background-image', 'url(/Content/themes/dmv/images/room_lo.jpg)');
                        break;
                    case 2:
                    case 5:
                        selectDestination('Orlando');
                        $('#rooms').css('background-image', 'url(/Content/themes/dmv/images/room_or.jpg)');
                        break;
                }
            }
        })('overlay');

        //eventos
        $('#rooms').on('click', function () {
            if ($('#rooms').width() == 160) {
                $('#enlargeRoomPicture').trigger('click');
            } else {
                $('#contractRoomPicture').trigger('click');
            }            
        });
        $('#enlargeRoomPicture').on('click', function () {
            $('#enlargeRoomPicture').fadeOut('fast');
            $('#contractRoomPicture').fadeIn('fast');
            $('#rooms').animate({
                width: '660px',
                marginLeft: '-500px'
            }, 500, 'easeInOutQuart');
            return false;
        });

        $('#contractRoomPicture').on('click', function () {
            $('#contractRoomPicture').fadeOut('fast');
            $('#enlargeRoomPicture').fadeIn('fast');
            $('#rooms').animate({
                width: '160px',
                marginLeft: '0px'
            }, 500, 'easeInOutQuart');
            return false;
        });

        $('#closeOfferDescription').on('click', function () {
            $('#divContent').animate({
                width: '0px'
            }, 500, 'easeInOutQuart', function () {
                $('#openOfferDescription').fadeIn('fast');
            });
            return false;
        });

        $('#openOfferDescription').on('click', function () {
            $('#openOfferDescription').fadeOut('fast');
            $('#divContent').animate({
                width: '660px'
            }, 500, 'easeInOutQuart');
            return false;
        });

        $('#player').on('click', function () {
            if (playing) {
                $.vegas('pause');
                playing = false;
                $('#player').css('background-image', 'url(/content/themes/dmv/images/play.png)');
            } else {
                $.vegas('slideshow');
                playing = true;
                $('#player').css('background-image', 'url(/content/themes/dmv/images/pause.png)');
            }
            return false;
        });

        $('#openTerms').on('click', function () {
            $('#terms').fadeIn('fast');
            return false;
        });

        $('#openPolicy').on('click', function () {
            $('#policy').fadeIn('fast');
            return false;
        });

        $('.close').on('click', function () {
            $(this).parent().fadeOut('fast');
            return false;
        });

        $('input[data-format=phone]').on('keyup', function () {
            var chars = $(this).val().replace(/\s+/g, '').split('');
            var value = "";
            //get number
            for (var i = 0; i < chars.length; i++) {
                if (!isNaN(chars[i])) {
                    value += chars[i];
                }
            }
            //format number
            if (value.length >= 10) {
                var digits = value.split('');
                value = '';
                for (var x = 0; x < digits.length; x++) {
                    if (x == 0) {
                        value += '(';
                    } else if (x == 3) {
                        value += ') ';
                    } else if (x == 6) {
                        value += ' ';
                    }
                    value += digits[x];
                }
                $(this).val(value);
            }
        });
    }

    function selectDestination(destination) {
        $('#destinations').children().each(function (i) {
            if ($(this).attr('id') == destination.replace(' ','')) {
                $(this).addClass('selected');
            } else {
                $(this).removeClass('selected');
            }
        });
    }

    //function sliding() {
    //    var delay = 0;
    //    $.each(BgImages, function (i, img) {
    //        if (i > currentPlaying) {
    //            var bg = new Image();
    //            bg.src = img.image;
    //            setTimeout(function () {
    //                if (playing) {
    //                    currentPlaying = i;
    //                    $('#background').css('background-image', 'url(' + img.image + ')');
    //                    $('#rooms').css('background-image', 'url(' + img.roomImage + ')');
    //                    selectDestination(img.destination);
    //                    if (i == BgImages.length - 1) {
    //                        currentPlaying = -1;
    //                        sliding();
    //                    }
    //                }
    //            }, 5000 + (delay * 5000));
    //            delay++;
    //        }
    //    });
    //}

    var onSuccess = function (data) {
        if (data.ResponseType == 1) {
            //si se registró
            $('#divForm').slideUp('fast');
            $('#divRegistered').slideDown('fast');
            var iframe = document.createElement('iframe');
            iframe.style.width = '0px';
            iframe.style.height = '0px';
            document.body.appendChild(iframe);
            iframe.src = '/Home/ConversionCode/3';
        }
    }

    var onBegin = function () {
        $('#btnRegisterNow').slideUp('fast');
    }

    return {
        init: init,
        onSuccess: onSuccess,
        onBegin: onBegin
    }
}();

$(function () {
    DMV.init();
});