/// <reference path="G:\Gerardo\Documentos\Visual Studio 2013\Projects\ePlat\ePlatBack\ePlatFront\Scripts/bookingengine-1.5.js" />

var Cart;
var $datePicker;

$.fn.fillSelect = function (data) {
    return this.clearSelect().each(function () {
        if (this.tagName == 'SELECT') {
            var dropdownList = this;
            $.each(data, function (index, optionData) {
                var option = new Option(optionData.Text, optionData.Value);

                if (navigator.appName == 'Microsoft Internet Explorer') {
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

$(document).ready(function () {
    //inicializa el carrusel
    $('.carousel.carousel-slider').carousel({ fullWidth: true });
    //autoplay del carrusel
    UI.MainCarousel.autoplay();

    /* CATEGORY */
    var zone = UI.Url.queryString('z');
    $('#ZoneID').val(zone.toLowerCase());
    $('#ZoneID').off('change').on('change', function () {
        UI.Url.changeQueryString('z', $('#ZoneID').val());
    });

    /* DETAIL */
    if ($('#divAvailability').length > 0) {
        $('#divSchedules').children().each(function (i) {
            $('div[data-day=' + $(this).attr('id') + ']').addClass('day-button-avaiable');
        });
        $('#divSchedules').children().eq(0).show();
        $('.day-button-avaiable').eq(0).addClass('day-button-selected');
        $('.day-button-avaiable').on('click', function () {
            $('.day-button-selected').removeClass('day-button-selected');
            $(this).addClass('day-button-selected');
            $('.day-schedule').hide();
            $('#' + $(this).attr('data-day')).show();
        });
    }
    $('.show-reviews').off('click').on('click', function () {
        let targetOffset = $("#divReviews").offset().top - 100;
        $('html,body').animate({ scrollTop: targetOffset }, 500);
    });

    /* GENERAL */
    //inicializar búsqueda
    UI.Search.init();
    //inicializa el botón de menu para resoluciones pequeñas
    $(".button-collapse").sideNav();
    //abre menu actividades terrestres
    $('.btnLandActivities').on('click', function () {
        $('#submenu-water-activities').slideUp('fast', function () {
            $('#submenu-land-activities').slideDown('fast');
            $("#sidenav-overlay").trigger("click");
        });
        $('html,body').animate({ scrollTop: 0 }, 500);
    });
    //cierra el submenu de actividades terrestres
    $('#btnCloseLandActivities').on('click', function () {
        $('#submenu-land-activities').slideUp('fast');
    });
    //abre el submenu de actividades acuáticas
    $('.btnWaterActivities').on('click', function () {
        $('#submenu-land-activities').slideUp('fast', function () {
            $('#submenu-water-activities').slideDown('fast');
            $("#sidenav-overlay").trigger("click");
        });
        $('html,body').animate({ scrollTop: 0 }, 500);
    });
    //cierra el submenu de actividades acuáticas
    $('#btnCloseWaterActivities').on('click', function () {
        $('#submenu-water-activities').slideUp('fast');
    });

    //inicializar forms
    $('select').material_select();
    //inicializar modales
    $('.modal').modal({
        dismissible: false
    });
    //cargar Cart
    UI.BookingEngine.loadCart();
    //resaltar sección
    UI.Navigation.highlightSection();
    //scroll
    $(window).on('scroll', function () {
        if ($(window).width() > 600) {
            $('#divCartTotals').css('margin-top', $('html').scrollTop() + 'px');
            if ($('html').scrollTop() > 150) {
                $('.top-nav').slideUp('fast');
                $('.logo-fixed').fadeIn('fast');
                $('.phone-fixed').fadeIn('fast');
                $('.subnav .l10').removeClass('center-align');
            } else {
                $('.top-nav').slideDown('fast');
                $('.logo-fixed').fadeOut('fast');
                $('.phone-fixed').fadeOut('fast');
                $('.subnav .l10').addClass('center-align');
            }
        }
    });


});

const UI = {
    MainCarousel: {
        autoplay: function autoplay() {
            $('.carousel').carousel('next');
            setTimeout(UI.MainCarousel.autoplay, 4500);
        }
    },

    BookingEngine: BookingEngineUI,

    Search: SearchUI,

    Controls: ControlsUI,

    MarketingAssistant: MarketingAssistantUI,

    /*Urls*/
    Url: {
        queryString: function (key) {
            var query = window.location.search;
            var value = "";
            if (query.length > 1) {
                query = query.substr(1);
            }
            var couples = query.split('&');
            $.each(couples, function (i, item) {
                var couple = item.split('=');
                if (couple[0] == key) {
                    value = couple[1];
                }
            });
            return value;
        },

        changeQueryString: function (key, value) {
            if (value == '') {
                window.location.href = window.location.origin + window.location.pathname;
            } else {
                //modificar el search y redirigir
                var query = window.location.search;
                var newQuery = "?";
                if (query.length > 1) {
                    query = query.substr(1);
                    var couples = query.split('&');
                    $.each(couples, function (i, item) {
                        var couple = item.split('=');
                        if (newQuery.length > 1) {
                            newQuery += '&';
                        }
                        if (couple[0] == key) {
                            newQuery += key + '=' + value;
                        } else {
                            newQuery += couple[0] + '=' + couple[1];
                        }
                    });
                } else {
                    newQuery += key + '=' + value;
                }
                window.location.search = newQuery;
            }
        }
    },

    /*Navegación*/
    Navigation: {
        highlightSection: function () {
            if (location.hash.indexOf("#ref=") >= 0) {
                var tag = location.hash.substr(location.hash.indexOf("=") + 1);
                Cart.CampaignTag = tag;
                Cart.save();
            } else {
                //highlight current section
                $('#navTabs .tab a').each(function () {
                    let tabUrl = $(this).attr('href');
                    if (location.href.indexOf(tabUrl.replace('#', '')) >= 0) {
                        $('#navTabs .tab a').removeClass('active');
                        $(this).addClass('active');
                    }
                });
            }
        },

        breadCrumbs: function () {

        }
    },
};