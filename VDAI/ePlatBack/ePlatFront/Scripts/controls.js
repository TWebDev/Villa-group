$.fn.fillSelect = function (data) {
    return this.clearSelect().each(function () {
        if (this.tagName == 'SELECT') {
            var dropdownList = this;
            $.each(data, function (index, optionData) {
                var option = new Option(optionData.Text, optionData.Value);

                if (navigator.appName == 'Microsoft Internet Explorer'){
                    dropdownList.add(option);
                }
                else {
                    dropdownList.add(option, null);
                }
            });
        }
    }).trigger('change');
}

$.fn.clearSelect = function () {
    return this.each(function () {
        if (this.tagName == 'SELECT') {
            this.options.length = 0;
        }
    });
}

var Controls = function () {
    var showMessage = function (data, formSelector, closeBox, eventID) {
        if (data.ResponseType >= 0) {
            $('#' + formSelector + ' .interaction-message').html(data.ResponseMessage).removeClass('error').addClass('confirmation');
            $('#' + formSelector).trigger('confirmed');
            //buscar si tiene código de conversión para cargar
            Controls.triggerConversion(eventID);

        } else {
            $('#' + formSelector + ' .interaction-message').html(data.ResponseMessage + data.ExceptionMessage).removeClass('confirmation').addClass('error');
            unblockForm(formSelector);
            $('#QuoteRequestForm input[type="submit"]').show();
        }
        if (data.ResponseType >= 0 && closeBox) {
            if ($('#quoteRequest').attr('role') == 'dialog') {
                setTimeout(function () {
                    //$('#btnCloseQuoteRequest').trigger('click');
                    $('[role="dialog"]').modal('hide');
                }, 3000);
            } else {
                setTimeout('$.fancybox.close()', 3000);
            }            
        }
    }

    var showOffer = function (data, formSelector, closeBox, eventID) {
        if (data.ResponseType >= 0) {
            var offer = '<span class="qr-offer"><span class="qr-t1"><span class="qr-t1-l1">Don\'t wait for the perfect moment.</span><span class="qr-t1-l2">Take the moment and make it perfect!</span></span><span class="qr-t2"><span class="qr-t2-l1">Call us now at</span><span class="qr-t2-l2">1 855 283 2170</span><span class="qr-t2-l3">and mention the promo code <b>PVO30</b> to receive a </span><span class="qr-t2-l4">$30 USD off</span><span class="qr-t2-l5"> the original package price.</span></span></span>';
            $('#' + formSelector + ' .interaction-message').html(offer).removeClass('error');
            $('.fancybox-inner').animate({ scrollTop: 1000 }, 500);
            $('#' + formSelector).trigger('confirmed');
            //buscar si tiene código de conversión para cargar
            Controls.triggerConversion(eventID);
        } else {
            $('#' + formSelector + ' .interaction-message').html(data.ResponseMessage + data.ExceptionMessage).removeClass('confirmation').addClass('error');
            unblockForm(formSelector);
            $('#QuoteRequestForm input[type="submit"]').show();
        }
        if (data.ResponseType >= 0 && closeBox) {
            if ($('#quoteRequest').attr('role') == 'dialog') {
                setTimeout(function () {
                    $('#btnCloseQuoteRequest').trigger('click');
                }, 3000);
            } else {
                setTimeout('$.fancybox.close()', 3000);
            }
        }
    }

    var triggerConversion = function (eventID) {
        if (!isNaN(eventID)) {
            var iframe = document.createElement('iframe');
            iframe.style.width = '0px';
            iframe.style.height = '0px';
            document.body.appendChild(iframe);
            iframe.src = '/Home/ConversionCode/' + eventID;
        }
    };

    var sendingInfo = function (formSelector) {
        $('#' + formSelector + ' .interaction-message').html('Saving information...').removeClass('error');
        blockForm(formSelector);
    }

    function blockForm(formSelector) {
        //$('#' + formSelector + ' div fieldset input[type=submit]').slideUp('fast');
        $('#' + formSelector).find('input[type=submit]').slideUp('fast');
    }

    function unblockForm(formSelector) {
        $('#' + formSelector).find('input[type=submit]').slideDown('fast');
    }

    var Reviews = function () {
        var init = function () {
            $('#ReviewItemTypeID').val($('#ItemTypeID').val());
            $('#ReviewItemID').val($('#ActivityID').val());
            $('.answer-button').on('click touchstart', function (event) {
                var value = $(this).attr('data-value');
                var fullStar = '/Content/themes/mex/images/icon_star_full.png';
                var emptyStar = '/Content/themes/mex/images/icon_star_empty.png';
                $(this).siblings('.answer-button').each(function (i) {
                    if (!isNaN($(this).attr('data-value')) && parseInt($(this).attr('data-value')) <= value) {
                        $(this).attr('src', fullStar);
                    } else {
                        $(this).attr('src', emptyStar);
                    }
                });
                $(this).attr('src', fullStar);
                //$(this).siblings('[type=hidden]').val(value);
                $('#Rating').val(value);
                if ($('.input-validation-error').length > 0) {
                    $("#ReviewForm").validate().form();
                }
                event.stopPropagation();
                event.preventDefault();
            });
        }

        var onSuccess = function (data, form) {
            //Controls.showMessage(data,'ReviewForm');
            var htmlString = '<div id="' + data.ObjectID + '" class="review" style="display:none;"><div class="review-info">';
            for (var s = 1; s <= parseInt($('#Rating').val()); s++)
            {
                htmlString += '<img src="/Content/themes/mex/images/stars_full.png" />';
            }
            for (var v = 1; v <= 5 - parseInt($('#Rating').val()); v++)
            {
                htmlString += '<img src="/Content/themes/mex/images/stars_empty.png" />';
            }
            htmlString += '<br />';
            htmlString += $('#Author').val() + '<br />';
            htmlString += $('#From').val() + '<br />';
            var today = new Date();
            var dd = today.getDate();
            var monthNames = [ "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" ];
            var mm = today.getMonth(); //January is 0!
            var yyyy = today.getFullYear();
            today = monthNames[mm] + ' ' + dd + ', ' + yyyy;
            htmlString += today + '<br />';
            htmlString += '</div><div class="review-description">' + $('#Review').val() + '</div></div>';

            $('#divRecentReviews').append(htmlString);
            $('#' + form).slideUp('fast');
            $('.review-invitation').slideUp('fast');
            $('#' + data.ObjectID).slideDown('fast');
        }

        return {
            init: init,
            onSuccess: onSuccess
        }
    }();

    var Reviews2 = function() {
        var init = function () {
            $('#ReviewItemTypeID').val($('#ItemTypeID').val());
            $('#ReviewItemID').val($('#ActivityID').val());
            $('.answer-button').on('click touchstart', function (event) {
                var value = $(this).attr('data-value');
                var fullStar = '/Content/themes/mex/images/icon_star_full.png';
                var emptyStar = '/Content/themes/mex/images/icon_star_empty.png';
                $(this).siblings('.answer-button').each(function (i) {
                    if (!isNaN($(this).attr('data-value')) && parseInt($(this).attr('data-value')) <= value) {
                        $(this).attr('src', fullStar);
                    } else {
                        $(this).attr('src', emptyStar);
                    }
                });
                $(this).attr('src', fullStar);
                $('#Rating').val(value);
                if ($('.input-validation-error').length > 0) {
                    $("#ReviewForm").validate().form();
                }
                event.stopPropagation();
                event.preventDefault();
            });
        };

        var onSuccess = function (data, form) {
                $('#divSubmitReview2').modal('close');
                var htmlString = '<li class="collection-item row yellow lighten-5" id="' + data.ObjectID + '" style="display:none;"><span class="review-info col s12 m3">';
                for (var s = 1; s <= parseInt($('#Rating').val()); s++)
                {
                    htmlString += '<img src="/Content/themes/mex/images/stars_full.png" />';
                }
                for (var v = 1; v <= 5 - parseInt($('#Rating').val()); v++)
                {
                    htmlString += '<img src="/Content/themes/mex/images/stars_empty.png" />';
                }
                htmlString += '<br />';
                htmlString += $('#Author').val();
                htmlString += '<span class="review-info-from">' + $('#From').val() + '</span>';
                var today = new Date();
                var dd = today.getDate();
                var monthNames = [ "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" ];
                var mm = today.getMonth(); //January is 0!
                var yyyy = today.getFullYear();
                today = monthNames[mm] + ' ' + dd + ', ' + yyyy;
                htmlString += '<span class="review-info-saved">' + today + '</span>';
                htmlString += '</span><span class="review-description col s12 m9">' + $('#Review').val() + '</span></li>';

                $('#divReviews .collection').append(htmlString);
                $('.review-invitation').slideUp('fast');
                $('#' + data.ObjectID).slideDown('fast');
        }

        return {
            init: init,
            onSuccess: onSuccess
        }
    }();

    var Banners = function () {
        var bannersIndex = [];
        var bannersTimeout;

        var init = function () {
            var banners = [];
            $('.banner-slider').each(function (i, item) {
                banners.push($(this).attr('data-group'));
                bannersIndex.push({ 'BannerGroupID': $(this).attr('data-group'), 'CurrentIndex': 0 });
            });
            if(banners.length > 0){
                $.getJSON('/Controls/GetBanners/' + banners.join(','), null, function (data) {
                    $.each(data, function (i, item) {
                        var bannersList = '';
                        $.each(item.Banners, function (b, banner) {
                            bannersList += '<span class="banner" style=display:none;>';
                            if (banner.Url != "") {
                                bannersList += '<a href="' + banner.Url + '">';
                            }
                            bannersList += '<img src="//eplatfront.villagroup.com' + banner.Path + '" width="' + item.Width + '" />';
                            if (banner.Url != "") {
                                bannersList += '</a>';
                            }
                            bannersList += '</span>';
                        });
                        $('*[data-group=' + item.BannerGroupID + ']').width(item.Width).height(item.Height).html(bannersList);
                        if (item.Banners.length > 1) {
                            var bannersNav = '<div data-groupnav="' + item.BannerGroupID + '" class="banner-slider-nav"><div class="prev" onclick="Controls.Banners.play(' + item.BannerGroupID + ', true)"><span></span></div><div class="counter"><span class="counter-index">1</span>/' + item.Banners.length + '</div><div class="next" onclick="Controls.Banners.play(' + item.BannerGroupID + ')"><span></span></div></div>';
                            $('*[data-group=' + item.BannerGroupID + ']').after(bannersNav);
                        } else {

                        }
                        Controls.Banners.play(item.BannerGroupID);
                    });
                });
            }
        }

        var play = function (bgid, prev) {
            $('*[data-group=' + bgid + '] .banner').fadeOut('fast');
            var currentIndex = 0;
            $.each(bannersIndex, function (x, current) {
                if (current.BannerGroupID == bgid) {
                    if (current.CurrentIndex >= $('*[data-group=' + bgid + '] .banner').length || current.CurrentIndex < 0) {
                        current.CurrentIndex = 1;
                    } else {
                        currentIndex = current.CurrentIndex;
                        if (prev) {
                            current.CurrentIndex--;
                        } else {
                            current.CurrentIndex++;
                        }                        
                    }
                }
            });
            $('*[data-group=' + bgid + '] .banner').eq(currentIndex).fadeIn('fast');
            if ($('*[data-group=' + bgid + '] .banner').length > 1) {
                clearTimeout(Controls.Banners.bannersTimeout);
                Controls.Banners.bannersTimeout = setTimeout('Controls.Banners.play(' + bgid + ')', 5000);                
            }
            $('*[data-groupnav=' + bgid + '] .counter-index').text(currentIndex + 1);
        }

        return {
            init: init,
            play: play,
            bannersTimeout: bannersTimeout
        }
    }();

    return {
        showMessage: showMessage,
        showOffer: showOffer,
        sendingInfo: sendingInfo,
        Reviews: Reviews,
        Reviews2: Reviews2,
        Banners: Banners,
        triggerConversion: triggerConversion
    }
}();

$(function () {
    /*Quote Request Control*/
    if ($('#QuoteRequestForm').length > 0) {
        $('#QuoteRequest_Arrival').val('');
        $('#QuoteRequest_Departure').val('');
        $('#QuoteRequest_Arrival').datepicker({
            dateFormat: 'yy-mm-dd'
        });
        $('#QuoteRequest_Departure').datepicker({
            dateFormat: 'yy-mm-dd'
        });
        $('#QuoteRequest_Destination').change(function () {
            $.getJSON("/Controls/GetResortsByDestination/" + $('#QuoteRequest_Destination').val(), null, function (data) {
                $('#QuoteRequest_Resort').fillSelect(data.Resorts);
                if (data.Resorts.length == 2) {
                    $('#QuoteRequest_Resort').children().eq(1).attr('selected', 'selected');
                }
            });
        });
    }
    /*Submit Review Control*/
    if ($('#divSubmitReview').length > 0) {
        Controls.Reviews.init();
    }

    if ($('#divSubmitReview2').length > 0) {
        Controls.Reviews2.init();
    }
});