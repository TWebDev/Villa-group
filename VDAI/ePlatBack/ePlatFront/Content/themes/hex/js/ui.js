/// <reference path="G:\Gerardo\Documentos\Visual Studio 2013\Projects\ePlat\ePlatBack\ePlatFront\Scripts/bookingengine-1.5.js" />
var Cart;

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

if (window.location.href.indexOf("loreto") == -1 && window.location.href.indexOf("cancun") == -1) {
    //si no es ni loreto ni cancun mostrar transporte y travel guide
    $('#navTabs li').eq(4).removeClass('hide');
    $('#navTabs li').eq(5).removeClass('hide');
    if (window.location.href.indexOf("vallarta") > -1) {
        //$('#navTabs li').eq(3).removeClass('hide');
    }
} else if (window.location.href.indexOf("www.mycancun") > -1 || window.location.href.indexOf("beta.mycancun") > -1) {
    //si es cancun en inglés mostrar travel guide
    $('#navTabs li').eq(5).removeClass('hide');
} else if (window.location.href.indexOf("mx.mycancun") > -1) {
    //cancun español
}

$(document).ready(function () {
    /* CATEGORY */
    var zone = UI.Url.queryString('z');
    $('#ZoneID').val(zone.toLowerCase());
    $('#ZoneID').off('change').on('change', function () {
        Url.changeQueryString('z', $('#ZoneID').val());
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
    //chat
    UI.loadChat();
    //season alert
    UI.seasonAlert();
    //inicializar búsqueda
    UI.Search.init();
    //banners
    UI.Banners.init();
    //inicializa el botón de menu para resoluciones pequeñas
    $(".button-collapse").sideNav();
    //abre menu actividades terrestres
    $('.btnLandActivities').on('click', function () {
        if ($('#land-activities').is(':visible')) {
            console.log('a');
            $('#land-activities').slideUp('fast');
        } else {
            console.log('b');
            $('#water-activities').slideUp('fast', function () {
                $('#land-activities').slideDown('fast');
                $("#sidenav-overlay").trigger("click");
            });
        }        
        $('html,body').animate({ scrollTop: 0 }, 500);
    });
    //cierra el submenu de actividades terrestres
    $('#btnCloseLandActivities').on('click', function () {
        $('#land-activities').slideUp('fast');
        $('html,body').animate({ scrollTop: 0 }, 500);
    });
    //abre el submenu de actividades acuáticas
    $('.btnWaterActivities').on('click', function () {
        if ($('#water-activities').is(':visible')) {
            console.log('a');
            $('#water-activities').slideUp('fast');
        } else {
            console.log('b');
            $('#land-activities').slideUp('fast', function () {
                $('#water-activities').slideDown('fast');
                $("#sidenav-overlay").trigger("click");
            });
        }        
        $('html,body').animate({ scrollTop: 0 }, 500);
    });
    //cierra el submenu de actividades acuáticas
    $('#btnCloseWaterActivities').on('click', function () {
        $('#water-activities').slideUp('fast');
        $('html,body').animate({ scrollTop: 0 }, 500);
    });
    //recorre el scroll al inicio de la página al seleccionar submenu
    $('ul.tabs').tabs({
        onShow: function (tab) {
            $('html,body').animate({ scrollTop: 0 }, 500);
        }
    });
    //inicializar forms
    $('select').material_select();
    //inicializar modales
    $('.modal').modal();
    //cargar Cart
    UI.BookingEngine.loadCart();
    //resaltar sección
    UI.Navigation.highlightSection();
    //scroll
    $(window).on('scroll', function () {
        if ($(window).width() > 600) {
            $('#divCartTotals').css('margin-top', $('html').scrollTop() + 'px');
        }        
    });
});

const UI = {
    MainCarousel: {
        autoplay: function autoplay() {
            $.each($('.carousel'), function () {
                if ($(this).children('.carousel-item').length > 1) {
                    $(this).carousel('next');
                }                
            });
            setTimeout(UI.MainCarousel.autoplay, 4500);
        }
    },

    BookingEngine: BookingEngineUI,

    Search: SearchUI,

    Controls: ControlsUI,

    MarketingAssistant: MarketingAssistantUI,

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

    /*Banners*/
    Banners: {
        init: function () {
            let banners = [];
            $('.banner-slider').each(function (i, item) {
                banners.push($(this).attr('data-group'));
            });
            if (banners.length > 0) {
                $.getJSON('/Controls/GetBanners/' + banners.join(','), null, function (data) {
                    $.each(data, function (i, item) {
                        var bannersList = '';
                        $.each(item.Banners, function (b, banner) {
                            if (banner.Url != "") {
                                bannersList += '<a class="carousel-item" href="' + banner.Url + '">';
                            } else {
                                bannersList += '<div class="carousel-item" ' + (banner.Html != null ? 'style="background-image:url(\'//eplatfront.villagroup.com' + banner.Path + '\')"' : '') + '>';
                            }

                            if (banner.Html == null) {
                                bannersList += '<img src="//eplatfront.villagroup.com' + banner.Path + '" />';
                            } else {

                            }

                            if (banner.Url != "") {
                                bannersList += '</a>';
                            } else {
                                bannersList += '</div>';
                            }
                        });
                        //console.log('agrega bannersList a grupo ' + item.BannerGroupID);
                        $('*[data-group=' + item.BannerGroupID + ']').html(bannersList);
                        
                    });
                    //inicializa el carrusel después de agregar banners
                    //console.log('inicializa el carrusel después de agregar banners');
                    $('.carousel.carousel-slider').carousel({ fullWidth: true });
                    //autoplay del carrusel después de agregar banners
                    //console.log('autoplay del carrusel después de agregar banners');
                    UI.MainCarousel.autoplay();
                });
            } else {
                //inicializa el carrusel
                $('.carousel.carousel-slider').carousel({ fullWidth: true });
                //autoplay del carrusel
                UI.MainCarousel.autoplay();
            }
            if (window.location.href.indexOf("mx") >= 0) {
                //setTimeout(UI.Banners.centralBannerMXN, 5000);
            } else {
                //setTimeout(UI.Banners.centralBannerUSD, 3000);
            }
        },
        centralBannerUSD: function () {
            if (sessionStorage.BF2018 == undefined || sessionStorage.BF2018 < 3) {
                $('body').append('<div id="centralBanner" style="background-color: rgba(0,0,0,.8); position:fixed; left: 0; top: 0; width: 100%; height: 100%; text-align: center; z-index:1031; padding-top: 150px;"><a href="/black-friday-2018"><img src="/content/themes/mex/images/v2/black-friday/black-friday-popup.jpg" alt="Black Friday 2018" style="box-shadow: 0 0 30px #111; margin: 0 auto;" class="responsive-img" /></a></div>');

                $("#centralBanner").fadeIn('fast');
                $('#centralBanner').off('click').on('click', function () {
                    $('#centralBanner').fadeOut('fast');
                });
                $('#centralBanner>img').off('click').on('click', function () {
                    $('#centralBanner').fadeOut('fast');
                });

                if (sessionStorage.BF2018 == undefined) {
                    sessionStorage.BF2018 = 1;
                } else {
                    sessionStorage.BF2018++;
                }
            }
        },
        centralBannerMXN: function () {
            if (localStorage.BF2018 == undefined || localStorage.BF2018 < 3) {
                $('body').append('<div id="centralBanner" style="background-color: rgba(0,0,0,.8); position:fixed; left: 0; top: 0; width: 100%; height: 100%; text-align: center; z-index:1031; padding-top: 150px;"><a href="/buen-fin-2018"><img src="/content/themes/mex/images/buen-fin/buen-fin-popup.jpg" alt="Buen Fin 2018" style="box-shadow: 0 0 30px #111; margin: 0 auto; max-width:602px;" class="img-responsive" /></a></div>');

                $("#centralBanner").fadeIn('fast');
                $('#centralBanner').off('click').on('click', function () {
                    $('#centralBanner').fadeOut('fast');
                });
                $('#centralBanner>img').off('click').on('click', function () {
                    $('#centralBanner').fadeOut('fast');
                });

                if (localStorage.BF2018 == undefined) {
                    localStorage.BF2018 = 1;
                } else {
                    localStorage.BF2018++;
                }
            }
        }
    },

    loadChat: function () {
        //$.ajax({
        //    url: 'https://mylivechat.com/chatwidget.aspx?hccid=60131222',
        //    dataType: "script"
        //});
        //$.ajax({
        //    url: 'https://mylivechat.com/chatinline.aspx?hccid=60131222',
        //    dataType: "script"
        //});
    },

    seasonAlert: function () {
        if ((Storage.get('TemporalAlert') == undefined || Storage.get('TemporalAlert') != true) && window.location.href.indexOf("activities/reviews") == -1) {
            $('footer').after('<div class="temporal-alert hide-on-small-only lime lighten-5"><div class="container"><div class="row"><div class="col s12"><span class="close right"><i class="small material-icons">close</i></span><h4>' + Language.Temporal_Alert_Title + '</h4><p>' + Language.Temporal_Alert_Content + '</p></div></div></div></div>').fadeIn('fast');
            $('.temporal-alert .close').on('click', function () {
                $('.temporal-alert').fadeOut('fast');
                Storage.save("TemporalAlert", true, 90);
            });
        }
    }
}