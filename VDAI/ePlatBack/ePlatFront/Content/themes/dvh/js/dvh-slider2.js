var Slider = function () {
    var sliderInterval;
    var slidesCounter = $('.slide-picture').length;
    var currentIndex = 0;

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
        $('#sliderPicturesContainer').children().eq(index).css('z-index', '2').fadeIn('fast');
    }

    return {
        play: play
    }
}();