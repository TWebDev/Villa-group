var ActivityByPromo = function () {
    var init = function () {
        $('.category-activities').eq(0).show();

        $('.category-name').off('click').on('click', function () {
            var categoryid = $(this).attr('data-categoryid');
            $('.category-activities').slideUp('fast');
            $('div[data-categoryid="' + categoryid + '"]').slideDown('fast');
            
        })
    }

    return {
        init: init
    }
}();

$(function () {
    ActivityByPromo.init();
});