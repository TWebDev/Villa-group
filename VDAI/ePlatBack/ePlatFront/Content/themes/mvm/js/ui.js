$(function () {
    UI.init();
});

var UI = function () {
    var phoneConversion = false;
    var nights = 0;
    var init = function () {
        setEvents();
        Slider.play();
    }

    function setEvents() {
        $('.go-top').off('click').on('click', function () {
            setLayout('home');
        });
        $('#submenu ul li').on('click', function () {
            $('#submenu ul li').removeClass('selected');
            $(this).addClass('selected');
            $('.content-cell').not($('.content-cell')[$(this).index()]).slideUp('fast');
            $('.content-cell').eq($(this).index()).slideDown('fast');
            if ($(this).index() == 0) {
                setLayout('home');
            } else {
                setLayout('content');
            }
            if ($(this).index() == 5) {
                $('#mapContainer').trigger('loaded');
            }
        });
        $('.question').click(function () {
            if (!$(this).children().eq(1).is(':visible')) {
                $('.answer').slideUp('fast');
            }
            $(this).children().eq(1).toggle('fast', function () {
                var targetOffset = $(this).parent().offset().top;
            });
        });
        $('#bookNowButton').on('click', function () {
            $('#booknow').addClass('open');
            $('#bookingOptions').show().animate({
                marginLeft: '-680px',
                width: '680px'
            }, 300, 'easeOutQuart');
            goToOptions();
        });
        $('#bookingOptionsClose').on('click', function () {
            $('#booknow').removeClass('open');
            $('#bookingOptions').animate({
                marginLeft: '0px',
                width: '0px',
                height: '290px'
            }, 300, 'easeOutQuart', function () {
                $(this).hide();
            });
            $('#btnGotoOptions').hide();
        });
        $('#bookingOptionsTerms').on('click', function () {
            $('#bookingStream').animate({
                marginLeft: '-1300px'
            }, 300, 'easeOutQuart');
            $('#bookingTitle').html('Terms and Conditions');
            $('#btnGotoOptions').fadeIn('fast');
            $('#bookingOptions').animate({
                height: '640px'
            }, 300, 'easeOutQuart');
        });
        $('#showPhone').on('click', function () {
            $(this).slideUp('fast');
            $('#phoneNumber').slideDown('fast');
            if (phoneConversion == false) {
                Controls.triggerConversion(5);
                phoneConversion = true;
            }
        });
        $('#btnStartBooking').on('click', function () {
            // Mostrar form
            $('#bookingStream').animate({
                marginLeft: '-650px'
            }, 300, 'easeOutQuart');
            $('#bookingTitle').html('Booking Online');
            $('#btnGotoOptions').fadeIn('fast');
            $('#bookingOptions').animate({
                height: '640px'
            }, 300, 'easeOutQuart');
        });
        $('#btnGotoOptions').on('click', function () {
            goToOptions();
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
        $('#QuoteRequestForm').on('confirmed', function () {
            $('#QuoteRequestForm').slideUp('fast');
            $('#formConfirmation').slideDown('fast');
            $('#bookingOptions').animate({
                height: '290px'
            }, 300, 'easeOutQuart');
        });
        $('#QuoteRequest_Arrival').on('change', function () {
            setDepartureDate();
        })
    }

    function setDepartureDate() {
        var milisegundos = parseInt(parseInt(UI.nights) * 24 * 60 * 60 * 1000);
        var fecha = new Date();
        fecha = $("#QuoteRequest_Arrival").datepicker("getDate");
        var tiempo = fecha.getTime();
        fecha.setTime(parseInt(tiempo + milisegundos));
        $("#QuoteRequest_Departure").val(fecha.getFullYear() + '-' + parseInt(fecha.getMonth() + parseInt(1))  + '-' + fecha.getDate());
    }

    function goToOptions() {
        $('#bookingStream').animate({
            marginLeft: '0px'
        }, 300, 'easeOutQuart');
        $('#bookingTitle').html('You have 2 options to book');
        $('#btnGotoOptions').fadeOut('fast');
        $('#bookingOptions').animate({
            height: '290px'
        }, 300, 'easeOutQuart');
    }

    function setLayout(layout) {
        if (layout == 'home') {
            $('#body').animate({
                marginTop: '0px'
            }, 500, 'easeOutQuart');
            $('#right-column').animate({
                marginTop: '-70px'
            }, 500, 'easeOutQuart');
            $('#submenu').css('position', 'relative');
            $('.go-top').fadeOut('fast');
            $('#right-column').css('position', 'relative');
            $('html,body').animate({ scrollTop: 0 }, 300);
        } else {
            $('#body').animate({
                marginTop: '-410px'
            }, 500, 'easeOutQuart');
            $('#right-column').animate({
                marginTop: '0px'
            }, 500, 'easeOutQuart');
            $('#submenu').css('position', 'fixed');
            $('.go-top').fadeIn('fast');
            $('#right-column').css('position', 'fixed');
        }
    }

    var showTerms = function () {
        $('#bookingOptions').show().animate({
            marginLeft: '-680px',
            width: '680px'
        }, 300, 'easeOutQuart');
        $('#bookingOptionsTerms').trigger('click');
        return false;
    }

    return {
        init: init,
        showTerms: showTerms,
        nights: nights
    }
}();