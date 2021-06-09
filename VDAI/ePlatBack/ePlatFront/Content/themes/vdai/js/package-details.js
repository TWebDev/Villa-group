$(function () {
    Slider.play();

    $('.question').click(function () {
        if (!$(this).children().eq(1).is(':visible')) {
            $('.answer').slideUp('fast');
        }
        $(this).children().eq(1).toggle('fast', function () {
            var targetOffset = $(this).parent().offset().top;
        });
    });
});