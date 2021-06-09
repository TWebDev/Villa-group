$(window).resize(function () {
    Slider.setGalleryHeight();
});

$(function () {
    Slider.setGalleryHeight();
});

var Slider = function () {
    var sliderInterval;
    var slidesCounter = $('.slide-picture').length;
    var currentIndex = 0;

    var setGalleryHeight = function () {
        if ($(window).width() < 767) {
            $('#pagerRow').draggable({ axis: "x" });
            $('#slidesGallery').height($('#sliderPicturesContainer').children().eq(0).height() + 60);
            if ($('.slider-video').parent().attr('id') != "destinationDescription") {
                $('.slider-video').appendTo("#destinationDescription");
            }
            if ($('.slider-info').parent().attr('id') != "body") {
                $('.slider-info').insertBefore("#slidesGallery");
            }
        } else if ($(window).width() >= 768) {
            if ($('.slider-video').parent().attr('id') != "sliderOverlay") {
                $('.slider-video').insertAfter($('#slidesGalleryCaption'));
            }
            if ($('.slider-info').parent().attr('id') != "slidesGallery") {
                $('.slider-info').appendTo($('#slidesGallery'));
            }
        }
    }

    var play = function (index) {
        clearInterval(sliderInterval);
        if (index != null) {
            currentIndex = index;
        }
        showSlide(currentIndex);
        $('.slide-button').not($('.slide-button')[currentIndex]).removeClass('slide-button-selected');
        $('.slide-button').eq(currentIndex).addClass('slide-button-selected');
        if (currentIndex < slidesCounter - 1) {
            currentIndex++;
        } else {
            currentIndex = 0;
        }
        sliderInterval = setInterval('Slider.play()', 5000);
    }

    function showSlide(index) {
        $('#sliderPicturesContainer').children().css('z-index', '1').fadeOut('slow');
        $('#slidesGalleryCaption').fadeOut('fast', function () {
            $(this).html($('#sliderPicturesContainer').children().eq(index).attr('alt')).fadeIn('fast');
        });
        $('#sliderPicturesContainer').children().eq(index).css('z-index', '2').fadeIn();
    }

    return {
        play: play,
        setGalleryHeight: setGalleryHeight
    }
}();