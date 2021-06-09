//ocultar botones
if (window.location.href.indexOf("loreto") == -1 && window.location.href.indexOf("cancun") == -1) {
    //si no es ni loreto ni cancun mostrar transporte y travel guide
    $('#menu li').eq(2).show();
    $('#menu li').eq(4).show();
    if (window.location.href.indexOf("vallarta") > -1) {
        $('#menu li').eq(3).show();
    }
} else if (window.location.href.indexOf("www.mycancun") > -1 || window.location.href.indexOf("beta.mycancun") > -1) {
    //si es cancun en inglés mostrar travel guide
    //$('#menu li').eq(2).show();
    $('#menu li').eq(4).show();
} else if (window.location.href.indexOf("mx.mycancun") > -1) {
    //$('#menu li').eq(2).show();
}
    

$(function () {
    if ($(window).width() < 768) {
        $('#divScheduleTable').on('loaded', function () {
            $('#purchase-process').height((160 * $('.cart-item').length) + 'px');
        });
    } else {
        $.ajax({
            url: 'https://mylivechat.com/chatwidget.aspx?hccid=60131222',
            dataType: "script"
        });
        $.ajax({
            url: 'https://mylivechat.com/chatinline.aspx?hccid=60131222',
            dataType: "script"
        });
    }

    UI.init();

    $(window).hashchange(function () {
        $('#menu li a.selected').removeClass('selected');
        if (location.hash.indexOf('#') >= 0) {
            if (location.hash != "#share-your-experience") {
                $('html,body').animate({ scrollTop: '0px' }, 500);
            }

            if (location.hash.indexOf("#ref=") >= 0) {
                var tag = location.hash.substr(location.hash.indexOf("=") + 1);
                var Cart = BookingEngine.getCart();
                Cart.CampaignTag = tag;
                BookingEngine.setCart(Cart);
            } else {
                $('.submenu').not('#sub-' + location.hash.substr(1)).slideUp('fast');
                $('#sub-' + location.hash.substr(1)).slideDown('fast');

                $('.main-header').slideUp('fast');
                $('a[href=' + location.hash + ']').addClass('selected');
                if ($(window).width() <= 850) {
                    $('#menu').slideUp('fast');
                }
            }
        }
    });
    $(window).trigger('hashchange');

    UI.highlightSection();

    if ($('#divGallery').length > 0 && $(window).width() < 768) {
        $('#divGallery figure img').eq(0).on('load', function () {
            $('#divGallery').height($('#divGallery figure img').eq(0).height() + 45);
        });
    }
    if ($(window).width() < 768) {
        $('#divPrices').insertAfter('#divFeatures');
        $('#divTotals').insertAfter('#purchase-process');
        $('#divPurchaseValidation .fix-button').insertAfter('#divPurchaseValidation .step3-validation');
        $('.step').width($(window).width());
    }
});

var UI = function () {
    var isiPad = navigator.userAgent.match(/iPad/i) != null;
    var scrolling = false;
    var menuOpen = true;
    var playImagesCurrent = 0;
    var playImagesFlag = true;
    var contentForSearch = '';
    var init = function () {

        if (isiPad) {
            //document.head.insertAdjacentHTML('beforeEnd', '<meta name="viewport" content="width=device-width, user-scalable = no" />');
            $('meta [name=viewport]').remove();
        }

        $("#divScheduleContainer").mCustomScrollbar({
            axis: "x",
            theme: "minimal"
        });        

        //termporal alert
        if ((localStorage["Eplat." + window.location.host + ".TemporalAlert"] == undefined || localStorage["Eplat." + window.location.host + ".TemporalAlert"] != 'true') && window.location.href.indexOf("activities/reviews") == -1) {
            $('footer').after('<div class="temporal-alert"><div class="alert-content"><span class="close">×</span><h4>' + Language.Temporal_Alert_Title + '</h4><p>' + Language.Temporal_Alert_Content + '</p><div></div></div></div>').fadeIn('fast');
            $('.temporal-alert .close').on('click', function () {
                $('.temporal-alert').fadeOut('fast');
                localStorage["Eplat." + window.location.host + ".TemporalAlert"] = 'true';
            });
        }

        //fancybox|
        if ($(window).width() < 768) {
            $(".fancybox").fancybox({
                maxWidth: 800,
                maxHeight: 600,
                fitToView: false,
                width: '99%',
                height: '99%',
                autoSize: false,
                closeClick: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        } else {
            $(".fancybox").fancybox({
                maxWidth: 800,
                maxHeight: 600,
                fitToView: false,
                width: '70%',
                height: '70%',
                autoSize: false,
                closeClick: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        }

        enablePlaceHolders();
        defineEvents();

        //activity detail
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
        if ($('#divGallery').length > 0) {
            $('#divGallery figure').eq(0).fadeIn('fast');
            $('#divGallerySelector img').on('click', function () {
                $('#divGallery figure').fadeOut('fast');
                $('#' + $(this).attr('data-picid')).fadeIn('fast');
            });
        }
        $('.read-reviews').on('click', function () {
            /*scroll*/
            var targetOffset = $("#divReviews").offset().top;
            $('html,body').animate({ scrollTop: targetOffset }, 500);
        });
        $('.share-your-experience').on('click', function () {
            $('#divSubmitReview').slideDown('fast');
            $('.share-your-experience').slideUp('fast');
            /*scroll*/
            var targetOffset = $("#divSubmitReview").offset().top;
            $('html,body').animate({ scrollTop: targetOffset }, 500);
        });

        $('.clear-form').on('click', function () {
            $('#FirstName').val('');
            $('#LastName').val('');
            $('#Email').val('');
            $('#ConfirmEmail').val('');
            $('#Phone').val('');
            $('#Mobile').val('');
            $('#Address').val('');
            $('#City').val('');
            $('#State').val('');
            $('#Country').val('');
            $('#ZipCode').val('');
            $('#StayingAtPlaceID').val('');
        });

        //category
        if ($('.tile-activity').length > 0) {
            $('.tile-activity').each(function () {
                $(this).fadeIn('slow');
            });

            var zone = Url.queryString('z');
            console.log('zone = ' + zone);
            $('#ZoneID').val(zone.toLowerCase());
            $('#ZoneID').off('change').on('change', function () {
                Url.changeQueryString('z', $('#ZoneID').val());
            });

            if ($(window).width() > 768 && !isiPad) {
                $('.tile-activity-images').hover(function () {
                    playImagesFlag = true;
                    playImagesCurrent = 0;
                    playImages($(this).parent().attr('id'));
                }, function () {
                    playImagesFlag = false;
                    playImagesCurrent = -2;
                    var id = $(this).parent().attr('id');
                    $(this).find('.tile-progress-indicator').animate({
                        width: '1px',
                        marginLeft: '0px'
                    }, 'fast', null, function () {
                        $('#' + id).find('tile-activity-images img:not(:first)').fadeOut('fast');
                        $('#' + id).find('tile-activity-images img:first').fadeIn('fast');
                    });
                });
            }
        }

        //banners
        Controls.Banners.init();

        //set booking engine
        BookingEngine.scheduleContainer = 'divScheduleTable';
        BookingEngine.serviceBooker = 'divAddActivity';
        BookingEngine.serviceDatePicker = 'ActivityDate',
        BookingEngine.serviceSchedulePicker = 'ActivitySchedule',
        BookingEngine.serviceDetailsPicker = 'ActivityDetails',
        BookingEngine.serviceType = 'ServiceType',
        BookingEngine.serviceTransportationFields = 'divTransportation',
        BookingEngine.serviceFlexibleSchedulePicker = 'ActivityFlexibleSchedule',
        BookingEngine.serviceAirline = 'TransportationAirline',
        BookingEngine.serviceFlight = 'TransportationFlight',
        BookingEngine.serviceHotelID = 'TransportationHotelID',
        BookingEngine.serviceZoneID = 'TransportationZoneID',

        BookingEngine.serviceRoundDiv = 'divOffersRoundTrip',
        BookingEngine.serviceRound = 'Round',
        BookingEngine.serviceRoundFields = 'divRoundTrip',
        BookingEngine.serviceRoundDate = 'RoundDate',
        BookingEngine.serviceRoundAirline = 'RoundAirline',
        BookingEngine.serviceRoundFlightNumber = 'RoundFlightNumber',
        BookingEngine.serviceRoundTime = 'RoundMeetingTime',

        BookingEngine.serviceAddButton = 'btnAddActivity',
        BookingEngine.serviceTotal = 'ActivityTotal',
        BookingEngine.serviceSavings = 'ActivitySavings',
        BookingEngine.labelTotal = 'lblTotal',
        BookingEngine.labelSavings = 'lblSavings',
        BookingEngine.form = 'PurchaseForm';

        BookingEngine.init();

        //mobile adaptations
        if ($(window).width() < 768) {
            $('#step2 .main-content').css({
                'width': ($(window).width() - 90) + 'px',
                'padding': '15px'
            });

            $('#step3 .main-content').css({
                'width': ($(window).width() - 90) + 'px',
                'padding': '15px'
            });
        }

        //start search functionality
        UI.Search.init();

        //load bread crumbs
        breadCrumbs();
    };

    function breadCrumbs() {
        if (location.pathname != '/' && location.pathname.indexOf("/activities/reviews/") == -1) {
            var linksStr = '';
            var links = location.pathname.split('/');
            for (var l = 0; l < links.length; l++) {
                linksStr += ' > <a href="';
                for (var i = 0; i <= l; i++) {
                    linksStr += '/' + links[i];
                }
                linksStr += '">' + (links[l] == '' ? 'home' : links[l].replace(/-/g,' ')) + '</a>';
                linksStr = linksStr.replace('//', '/').replace(/"\/land-activities"/g, '"#land-activities"').replace(/"\/water-activities"/g, '"#water-activities"').replace(/"\/actividades-terrestres"/g, '"#land-activities"').replace(/"\/actividades-acuaticas"/g, '"#water-activities"');
            }
            var bc = '<div class="bread-crumbs">' + linksStr.substr(3) + '</div>';
            $('#body .main-header').after(bc);
        }        
    }

    function playImages(control) {
        if (playImagesCurrent != -2 && playImagesFlag) {
            if (playImagesCurrent < $('#' + control).find('.tile-activity-images img').length - 1) {
                playImagesCurrent++;
            } else {
                playImagesCurrent = 0;
            }
            $('#' + control).find('.tile-activity-images img').fadeOut('fast');
            $('#' + control).find('.tile-activity-images img').eq(playImagesCurrent).fadeIn('slow');
            if ($('#' + control).find('.tile-activity-images .tile-progress-indicator').width() == 1) {
                $('#' + control).find('.tile-activity-images .tile-progress-indicator').animate({
                    width: '196px'
                }, 2000, function () {
                    if (playImagesCurrent == -2) {
                        $(this).animate({
                            'width': '1px',
                        }, 1000);
                    }
                    playImages(control);
                });
            } else {
                $('#' + control).find('.tile-activity-images .tile-progress-indicator').animate({
                    width: '1px',
                    marginLeft: '195px'
                }, 2000, function () {
                    $('#' + control).find('.tile-activity-images .tile-progress-indicator').css('margin-left', '0');
                    playImages(control);
                });
            }
        } else {
            playImagesCurrent = 0;
        }
    }

    var confirmation = function (data, form) {
        $('#divPurchaseSubmit .primary-button').val('PAY NOW');
        $('#divPurchaseSubmit').slideDown('fast');
        $('#lblSubmit').slideUp('fast');
        if (data.ResponseType == 1) {
            Controls.triggerConversion(1);
            $('#lblConfirmationCode').text((data.AuthCode != null ? data.AuthCode : "TEST"));
            $('#lblConfirmationTotal').text($('#lblTotal').text());
            if ($(window).width() < 768) {
                $('#purchase-process').animate({
                    marginLeft: '-' + ($(window).width() * 3) + 'px',
                    minHeight: '635px'
                }, 'fast');
            } else {
                $('#purchase-process').animate({
                    marginLeft: '-2950px'
                }, 'fast');
            }
            $('#divTotals').slideUp('fast');
            $('#step4').slideDown('fast');
            BookingEngine.deleteCart();
            BookingEngine.getElements();
        } else {
            var Cart = BookingEngine.getCart();
            Cart.CartID = data.PurchaseID;
            Cart.Items = data.Items;
            BookingEngine.setCart(Cart);
            $('#PurchaseID').val(data.PurchaseID);
            console.log('b: ' + $.toJSON(Cart.Items));
            $('#ItemsJSON').val($.toJSON(Cart.Items));
            if ($('#' + form + ' .interaction-message').length > 0) {
                $('#' + form + ' .interaction-message').html(data.ResponseMessage);
            } else {
                $('.message-label').slideUp('fast');
                $('.interaction-message').html(data.ResponseMessage).slideDown('fast');
                $('.message').animate({
                    marginLeft: '0px'
                }, 500, 'easeOutQuart');
            }
            BookingEngine.getElements(true);
        }
    }

    var sendingInfo = function () {
        $('#divPurchaseSubmit .primary-button').val(Language.Sending + '...');
        $('#divPurchaseSubmit').slideUp('fast');
        $('#lblSubmit').html('<img src="/Images/loading.gif" width="10" height="10" style="margin-right:5px;">' + Language.Processing);
        $('#lblSubmit').slideDown('fast');
        $('.message').animate({
            marginLeft: '270px'
        }, 500, 'easeOutQuart');
    }

    function fillCartForm() {
        var Cart = BookingEngine.getCart();
        console.log('a: ' + $.toJSON(Cart.Items));
        $('#ItemsJSON').val($.toJSON(Cart.Items));
        $('#Total').val(Cart.Total);
        $('#Savings').val(Cart.Savings);
        $('#PromoID').val(Cart.PromoID);
        $('#CampaignTag').val(Cart.CampaignTag);
        $('#CurrencyID').val(Cart.CurrencyID);
        $('#TerminalID').val(Cart.TerminalID);
        $('#PointOfSaleID').val(Cart.PointOfSaleID);
        $('#PurchaseID').val(Cart.CartID);
        var hotelid = 0;
        $.each(Cart.Items, function (s, service) {
            if (service.ServiceType == 3) {
                hotelid = service.HotelID;
            }
        });
        if (hotelid != 0) {
            $('#StayingAtPlaceID').val(hotelid);
        } else {
            $('#StayingAtPlaceID').val('');
        }
        $('#divStayingAt').hide();
        $('#StayingAtPlaceID').on('click', function () {
            if ($('#StayingAtPlaceID').val() == "0") {
                $('#divStayingAt').show();
            } else {
                $('#divStayingAt').hide();
            }
        });
    }

    function defineEvents() {
        $('#aLandActivities').on('click', function (e) {
            if ($('#sub-land-activities').is(':visible')) {
                e.preventDefault();
                UI.removeHash();
            }
        });

        $('#aWaterActivities').on('click', function (e) {
            if ($('#sub-water-activities').is(':visible')) {
                e.preventDefault();
                UI.removeHash();
            }
        });

        $('#aShoppingCart').on('click', function (e) {
            if ($('#sub-shopping-cart').is(':visible')) {
                e.preventDefault();
                UI.removeHash();
            }
        });

        $('#btnCancelBooking').on('click', function () {
            $('#divAddActivity').fadeOut('fast');
            $('#divAddActivityBg').fadeOut('fast');
            $('#sub-shopping-cart').animate({
                height: '300px'
            }, 300, 'easeOutQuart');
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

        $('input[data-format=card-number]').on('keyup', function () {
            var chars = $(this).val().replace(/\s+/g, '').split('');
            var value = "";
            //get number
            for (var i = 0; i < chars.length; i++) {
                if (!isNaN(chars[i])) {
                    value += chars[i];
                }
            }
            //format number
            if (value.length >= 4) {
                var digits = value.split('');
                value = '';
                for (var x = 0; x < digits.length; x++) {
                    if (x % 4 == 0 && x > 0 && x < 16) {
                        value += ' ';
                    }
                    if (x < 16) {
                        value += digits[x];
                    }
                }
                $(this).val(value);
            }
        });

        $('#btnGoCheckout').on('click', function () {
            if ($('#lblTotal').text() != "$0" && $('#lblTotal').text() != "$0.00") {
                //cargar
                fillCartForm();
                //llenar datos de compra si hay almacenados
                var PurchaseInfoLocal = UI.Storage.get("PurchaseInfo");
                if (PurchaseInfoLocal != undefined) {
                    $('#FirstName').val(PurchaseInfoLocal.FirstName);
                    $('#LastName').val(PurchaseInfoLocal.LastName);
                    $('#Email').val(PurchaseInfoLocal.Email);
                    $('#ConfirmEmail').val(PurchaseInfoLocal.ConfirmEmail);
                    $('#Phone').val(PurchaseInfoLocal.Phone);
                    $('#Mobile').val(PurchaseInfoLocal.Mobile);
                    $('#Address').val(PurchaseInfoLocal.Address);
                    $('#City').val(PurchaseInfoLocal.City);
                    $('#State').val(PurchaseInfoLocal.State);
                    $('#Country').val(PurchaseInfoLocal.Country);
                    $('#ZipCode').val(PurchaseInfoLocal.ZipCode);
                    $('#StayingAtPlaceID').val(PurchaseInfoLocal.StayingAtPlaceID);
                }

                //mostrar
                if ($(window).width() <= 786) {
                    $('#purchase-process').animate({
                        marginLeft: '-' + $(window).width() + 'px',
                        minHeight: '670px'
                    }, 'fast');
                    $('#step2 .inline-button').animate({
                        minHeight: '785px'
                    }, 'fast');
                    $('#sub-shopping-cart').animate({
                        height: '820px'
                    }, 'fast');
                } else {
                    $('#purchase-process').animate({
                        marginLeft: '-980px'
                    }, 'fast');
                    $('#sub-shopping-cart').animate({
                        height: '400px'
                    }, 'fast');
                }
                $('html,body').animate({ scrollTop: 0 }, 500);
                $('#btnGoCheckout').hide();
                $('#btnGoCheckout2').show();
            } else {
                alert('You don\'t have activities to pay yet.');
            }
            return false;
        });

        $('#btnGoCheckout2').on('click', function () {
            if (BookingEngine.paymentsProviderName == "Prosa") {
                //guardar datos de formulario en local para volver a poblar si es necesario
                var PurchaseInfoLocal = {
                    FirstName: $('#FirstName').val(),
                    LastName: $('#LastName').val(),
                    Email: $('#Email').val(),
                    ConfirmEmail: $('#ConfirmEmail').val(),
                    Phone: $('#Phone').val(),
                    Mobile: $('#Mobile').val(),
                    Address: $('#Address').val(),
                    City: $('#City').val(),
                    State: $('#State').val(),
                    Country: $('#Country').val(),
                    ZipCode: $('#ZipCode').val(),
                    StayingAtPlaceID: $('#StayingAtPlaceID').val()
                }
                UI.Storage.save("PurchaseInfo", PurchaseInfoLocal, 90);
                //proceder
                $('#btnGoCheckout2').hide();
                //prosa
                //guardar datos de compra -- //'1.00' //$('#Total').val()
                var purchase = {
                    PaymentsProvider: BookingEngine.paymentsProviderName,
                    PaymentsProviderAccount: BookingEngine.paymentsProviderAccount,
                    PaymentsMerchantID: BookingEngine.paymentsMerchantID,
                    ItemsJSON: $('#ItemsJSON').val(),
                    Total: $('#Total').val(),
                    Savings: $('#Savings').val(),
                    PromoID: $('#PromoID').val(),
                    CurrencyID: $('#CurrencyID').val(),
                    TerminalID: $('#TerminalID').val(),
                    PointOfSaleID: $('#PointOfSaleID').val(),
                    PurchaseID: $('#PurchaseID').val(),
                    
                    FirstName: $('#FirstName').val(),
                    LastName: $('#LastName').val(),
                    City: $('#City').val(),
                    State: $('#State').val(),
                    Country: $('#Country').val(),
                    Address: $('#Address').val(),
                    ZipCode: $('#ZipCode').val(),
                    Email: $('#Email').val(),
                    Phone: $('#Phone').val(),
                    Mobile: $('#Mobile').val(),
                    PointOfSale: $('#PointOfSale').val(),
                    StayingAtPlaceID: $('#StayingAtPlaceID').val(),
                    StayingAt: $('#StayingAt').val()
                };
                $.post('/Purchase/PartialSave', purchase, function (data) {
                    if (data.ResponseType == 1) {
                        //guardado correcto
                        var Cart = BookingEngine.getCart();
                        Cart.CartID = data.PurchaseID;
                        Cart.Items = data.Items;
                        BookingEngine.setCart(Cart);
                        $('#PurchaseID').val(data.PurchaseID);
                        console.log('c: ' + $.toJSON(Cart.Items));
                        $('#ItemsJSON').val($.toJSON(Cart.Items));
                        BookingEngine.getElements(true);

                        //cargar formulario
                        var arrTotal = purchase.Total.toString().split('.');
                        var totalString = '';
                        if (arrTotal.length == 1) {
                            totalString = arrTotal[0] + '00';
                        } else {
                            if (arrTotal[1].length == 1) {
                                totalString = arrTotal[0] + arrTotal[1] + '0';
                            } else {
                                totalString = arrTotal[0] + arrTotal[1];
                            }
                        }
                        $('#formProsa>input[name="total"]').val(totalString);
                        $('#formProsa>input[name="order_id"]').val(data.PurchaseID.replace(/-/g, "").substr(0,26));
                        $('#formProsa>input[name="digest"]').val(data.ResponseMessage);
                        $('#formProsa>input[name="merchant"]').val(BookingEngine.paymentsMerchantID);
                        $('#formProsa>input[name="urlBack"]').val($('#formProsa>input[name="urlBack"]').val().replace('domain', window.location.host).replace('value', data.PurchaseID));
                        $('#formProsa').attr('action', $('#formProsa').attr('action').replace('merchant', BookingEngine.paymentsProviderAccount).replace('domain', window.location.host));

                        //enviar a página de pago
                        $('#formProsa').submit();
                    } else {
                        //error
                        alert(data.ResponseMessage)
                        $('#btnGoCheckout2').show();
                    }
                }, 'json');
            } else {
                //resortcom
                if ($(window).width() < 768) {
                    $('#purchase-process').animate({
                        marginLeft: '-' + ($(window).width() * 2) + 'px',
                        minHeight: '545px'
                    }, 'fast');
                    $('#step3 .inline-button').animate({
                        minHeight: '655px'
                    }, 'fast');

                    if (parseInt($('#purchase-process').css('margin-left')) < $(window).width() * 2) {
                        $('#divTotals .message').hide();
                        $('#divTotals').height('150px');
                        $(this).hide();
                    }

                    $('#sub-shopping-cart').animate({
                        height: '695px'
                    }, 'fast');
                } else {
                    $('#purchase-process').animate({
                        marginLeft: '-1960px'
                    }, 'fast');

                    $('#sub-shopping-cart').animate({
                        height: '300px'
                    }, 'fast');

                    if (parseInt($('#purchase-process').css('margin-left')) < 1959) {
                        $(this).hide();
                    }
                }
                $('html,body').animate({ scrollTop: 0 }, 500);
            }
            return false;
        });

        $('#step2 .inline-button').on('click', function () {
            if ($(window).width() < 768) {
                $('#purchase-process').animate({
                    marginLeft: '0px',
                    height: (160 * $('.cart-item').length + 20) + 'px'
                }, 'fast');
                $('#sub-shopping-cart').animate({
                    height: (160 * $('.cart-item').length + 170) + 'px'
                }, 'fast');
            } else {
                $('#purchase-process').animate({
                    marginLeft: '0px',
                    minHeight: $('#step1').height() + 'px'
                }, 'fast');
                $('#sub-shopping-cart').animate({
                    height: '300px'
                }, 'fast');
            }
            $('#btnGoCheckout').show();
            $('#btnGoCheckout2').hide();
        });

        $('#step3 .inline-button').on('click', function () {
            if ($(window).width() < 768) {
                $('#purchase-process').animate({
                    marginLeft: '-' + $(window).width() + 'px',
                    minHeight: '670px'
                }, 'fast');
                $('#step2 .inline-button').animate({
                    minHeight: '785px'
                }, 'fast');
            } else {
                $('#purchase-process').animate({
                    marginLeft: '-980px'
                }, 'fast');
                $('#sub-shopping-cart').animate({
                    height: '400px'
                }, 'fast');
            }
            $('#btnGoCheckout2').show();
            $('#divTotals .message').show();
        });

        $('#divPurchaseValidation').on("swipeleft", function (event) {
            $('#btnFixErrors').trigger('click');
        });

        $('#btnFixErrors').on('click', function () {
            $('#divPurchaseValidation').slideUp('fast');
            if ($('#divPurchaseValidation .step2-validation .field-validation-error span').length > 0) {
                if ($(window).width() < 768) {
                    $('#purchase-process').animate({
                        marginLeft: '-' + $(window).width() + 'px',
                        minHeight: '670px'
                    }, 'fast');
                    $('#step2 .inline-button').animate({
                        minHeight: '785px'
                    }, 'fast');
                    $('#sub-shopping-cart').animate({
                        height: '820px'
                    }, 'fast');
                } else {
                    $('#purchase-process').animate({
                        marginLeft: '-980px'
                    }, 'fast');
                    $('#sub-shopping-cart').animate({
                        height: '400px'
                    }, 'fast');
                }
                $('#btnGoCheckout2').show();
            } else {
                if ($(window).width() < 768) {
                    $('#purchase-process').animate({
                        marginLeft: '-' + ($(window).width() * 2) + 'px'
                    }, 'fast');
                    $('#sub-shopping-cart').animate({
                        height: '695px'
                    }, 'fast');
                } else {
                    $('#purchase-process').animate({
                        marginLeft: '-1960px'
                    }, 'fast');
                }
            }
            $('html,body').animate({ scrollTop: 0 }, 500);
        });

        $('#Accept').val('false');
        $('#Accept').on('click', function () {
            if ($('#Accept').is(':checked')) {
                $('#Accept').val('true');
            } else {
                $('#Accept').val('false');
            }
        });

        $('#showMenu').on('click', function () {
            $('#menu').slideToggle('fast');
            return false;
        });

        $('.showTerms').on('click', function () {
            $('#divTerms').slideDown('fast');
        });

        $('.close').on('click', function () {
            $(this).parent().slideUp('fast');
        });
    }

    function enablePlaceHolders() {
        if (!Modernizr.input.placeholder) {

            $('[placeholder]').focus(function () {
                var input = $(this);
                if (input.val() == input.attr('placeholder')) {
                    input.val('');
                    input.removeClass('placeholder');
                }
            }).blur(function () {
                var input = $(this);
                if (input.val() == '' || input.val() == input.attr('placeholder')) {
                    input.addClass('placeholder');
                    input.val(input.attr('placeholder'));
                }
            }).blur();
            $('[placeholder]').parents('form').submit(function () {
                $(this).find('[placeholder]').each(function () {
                    var input = $(this);
                    if (input.val() == input.attr('placeholder')) {
                        input.val('');
                    }
                });
            });

        }
    }

    var adjustCartHeight = function () {
        $('#sub-shopping-cart').stop().animate({
            height: (parseInt($('#divAddActivity').height()) + parseInt(40)) + 'px'
        }, 300, 'easeOutQuart');
    }

    var removeHash = function () {
        var scrollV, scrollH, loc = window.location;
        if ("pushState" in history)
            history.pushState("", document.title, loc.pathname + loc.search);
        else {
            // Prevent scrolling by storing the page's current scroll offset
            scrollV = document.body.scrollTop;
            scrollH = document.body.scrollLeft;

            loc.hash = "";

            // Restore the scroll offset, should be flicker free
            document.body.scrollTop = scrollV;
            document.body.scrollLeft = scrollH;
        }
        //customization for UI
        $('#menu li a.selected').removeClass('selected');
        $('.main-header').slideDown('fast');
        $('.submenu').slideUp('fast');
        UI.highlightSection();
    }

    var highlightSection = function () {
        //highlight current section
        $('#menu li a').each(function () {
            if (location.href.indexOf($(this).attr('href').replace('#', '')) >= 0) {
                $(this).addClass('selected');
            };
        });
    }

    var alerts = function (title) {
        var randomPeople = Math.floor(Math.random() * 71);
        var randomPurchases = getEstimatedPurchases(title);
        var randomDiv = Math.floor(Math.random() * 2);
        var alerts = [];
        alerts[0] = '<div id="divAlert0" class="alert"><span class="close"></span><p>' + randomPeople + ' people are browsing ' + title + ' right now. </p></div>';
        alerts[1] = '<div id="divAlert1" class="alert"><span class="close"></span><p>' + randomPurchases + ' people have purchased ' + title + ' in the last 24 hours.</p></div>';
        $('#body').append(alerts[randomDiv]);
        $('#divAlert' + randomDiv).delay(4000).fadeIn('fast');
    }

    function getEstimatedPurchases(title) {
        var purchases = 0;
        //obtener el registro
        var valueObj = UI.Storage.get('ActivityPurchases');
        if (valueObj == undefined) {
            purchases = getRandomPurchases();
            //guardar el registro
            UI.Storage.save('ActivityPurchases', [{ activity: title, purchases: purchases }])
        } else {
            //buscar el registro en el objeto guardado.
            $.each(valueObj, function (i, item) {
                if (item.activity == title) {
                    purchases = item.purchases;
                }
            });

            //si no lo encuentra, crearlo y guardar el registro
            if (purchases == 0) {
                purchases = getRandomPurchases();
                valueObj.push({ activity: title, purchases: purchases });
                UI.Storage.save('ActivityPurchases', valueObj);
            }
        }
        return purchases;
    }

    function getRandomPurchases() {
        return Math.floor(Math.random() * 26);
    }

    var Search = function () {
        var Index;
        var init = function () {
            //cargar la lista de información
            Index = UI.Storage.get("SearchIndex");
            if (Index == undefined) {
                getIndex();
            } else {
                //si la fecha es 2 dias menor, pedir el indice
                var parts = Index.Date.split('-');
                var theSavedDate = new Date(parts[0], parts[1] - 1, parts[2]);
                var yesterday = new Date();
                yesterday.setDate(yesterday.getDate() - 1);
                if (theSavedDate < yesterday) {
                    getIndex();
                } else {
                    $('#divSearchBox').slideDown('fast');
                }
            }

            //evento de búsqueda
            $('#txtSearch').on('keyup', function () {
                search($('#txtSearch').val());
            });
            $('#btnSearch').on('click', function () {
                search($('#txtSearch').val());
            });
            $('#btnCloseSearch').on('click', function () {
                close();
            });
        }

        function getIndex() {
            $.getJSON('/Activities/GetIndex', null, function (data) {
                UI.Storage.save("SearchIndex", data, 8);
                Index = data;
                $('#divSearchBox').slideDown('fast');
            });
        }

        function close() {
            $('#searchQuickResults .container').html('');
            $('#searchQuickResults').slideUp('fast');
        }

        function preg_quote(str) {
            return (str + '').replace(/([\\\.\+\*\?\[\^\]\$\(\)\{\}\=\!\<\>\|\:])/g, "\\$1");
        }

        function highlight(data, search) {
            return data.replace(new RegExp("(" + preg_quote(search) + ")", 'gi'), "<b>$1</b>");
        }

        function search(keyword) {
            if (keyword != "") {
                var results = '';
                var namesArr = [];
                var contentArr = [];
                var pointsArr = [];
                var providerArr = [];
                var namesStr = '';
                var contentStr = '';
                var pointsStr = '';
                var providerStr = '';
                var keywordSplit = keyword.trim().split(' ');
                var rating = 0;
                var content = '';
                var indexOfContent = 0;
                $.each(Index.Activities, function (i, item) {
                    //check names
                    rating = 0;
                    content = item.Name;
                    for (var x = 0; x < keywordSplit.length; x++) {

                        if (content.toLowerCase().indexOf(keywordSplit[x].toLowerCase()) >= 0) {
                            content = highlight(content, keywordSplit[x]);
                            rating++;
                        }
                    }
                    if (rating > 0) {
                        namesArr.push({ Rating: rating, Content: (item.Picture != "" ? '<img src="//www.myvallartaexperience.com' + item.Picture + '?width=30&height=30&mode=crop" />' : '<span class="img-replacement"></span>') + '<a href="' + item.Url + '">' + content + '</a>' });
                    }

                    //check content
                    if (item.Content != null) {
                        rating = 0;
                        content = item.Content;
                        indexOfContent = 0;

                        for (var x = 0; x < keywordSplit.length; x++) {
                            if (content.toLowerCase().indexOf(keywordSplit[x].toLowerCase()) >= 0) {
                                var regex = /(<([^>]+)>)/ig, content = content.replace(regex, "");
                                content = highlight(content, keywordSplit[x]);
                                indexOfContent = content.toLowerCase().indexOf(keywordSplit[x].toLowerCase());
                                rating++;
                            }
                        }
                        //cortar contenido para mostrar la keyword

                        if (rating > 0) {
                            contentArr.push({ Rating: rating, Content: (item.Picture != "" ? '<img src="//www.myvallartaexperience.com' + item.Picture + '?width=30&height=30&mode=crop" />' : '<span class="img-replacement"></span>') + '<a href="' + item.Url + '">' + item.Name + '</a><br />...' + content.substr((indexOfContent > 30 ? indexOfContent - 30 : 0), 60) + '...' });
                        }
                    }

                    //check points
                    if (item.MeetingPoints != null) {
                        rating = 0;
                        content = item.MeetingPoints;
                        indexOfContent = 0;

                        for (var x = 0; x < keywordSplit.length; x++) {
                            if (content.toLowerCase().indexOf(keywordSplit[x].toLowerCase()) >= 0) {
                                var regex = /(<([^>]+)>)/ig, content = content.replace(regex, "");
                                content = highlight(content, keywordSplit[x]);
                                indexOfContent = content.toLowerCase().indexOf(keywordSplit[x].toLowerCase());
                                rating++;
                            }
                        }

                        if (rating > 0) {
                            pointsArr.push({ Rating: rating, Content: (item.Picture != "" ? '<img src="//www.myvallartaexperience.com' + item.Picture + '?width=30&height=30&mode=crop" />' : '<span class="img-replacement"></span>') + '<a href="' + item.Url + '">' + item.Name + '</a><br />...' + content.substr((indexOfContent > 30 ? indexOfContent - 30 : 0), 60) + '...' });
                        }
                    }

                    //check providers
                    if (item.Provider != null) {
                        rating = 0;
                        content = item.Provider;
                        if (content.toLowerCase().indexOf(keyword.toLowerCase()) >= 0) {
                            content = highlight(content, keywordSplit[x]);
                            rating++;
                        }

                        if (rating > 0) {
                            providerArr.push({ Rating: rating, Content: (item.Picture != "" ? '<img src="//www.myvallartaexperience.com' + item.Picture + '?width=30&height=30&mode=crop" />' : '<span class="img-replacement"></span>') + '<a href="' + item.Url + '">' + item.Name + '</a><br />Provided by ' + content });
                        }
                    }
                });

                // order results
                namesArr = namesArr.sort(custom_sort);
                $.each(namesArr.reverse(), function (n, name) {
                    namesStr += '<li>' + name.Content + '</li>';
                });

                contentArr = contentArr.sort(custom_sort);
                $.each(contentArr.reverse(), function (c, item) {
                    contentStr += '<li>' + item.Content + '</li>';
                });

                pointsArr = pointsArr.sort(custom_sort);
                $.each(pointsArr.reverse(), function (c, item) {
                    pointsStr += '<li>' + item.Content + '</li>';
                });
                $.each(providerArr.reverse(), function (c, item) {
                    providerStr += '<li>' + item.Content + '</li>';
                });

                results = '<ul><li class="header">' + Language.Search_by_Name + '</li>' + namesStr + '<li class="header">' + Language.Search_in_Content + '</li>' + contentStr + '<li class="header">' + Language.Search_by_Meeting_Point + '</li>' + pointsStr + '<li class="header">' + Language.Search_by_Provider + '</li>' + providerStr + '</ul>';
                $('#searchQuickResults .container').html(results);
                $('#searchQuickResults').slideDown('fast');
            } else {
                close();
            }
        }

        function custom_sort(a, b) {
            return a.Rating - b.Rating;
        }

        return {
            init: init
        }
    }();

    var Storage = function () {
        function supports_html5_storage() {
            try {
                return 'localStorage' in window && window['localStorage'] !== null;
            } catch (e) {
                return false;
            }
        }

        var save = function (name, value, days) {
            value = $.toJSON(value);
            if (supports_html5_storage) {
                localStorage["Eplat." + location.hostname + "." + name] = value;
            } else {
                UI.Storage.Cookies.set("Eplat." + location.hostname + "." + name, value, days);
            }
        }

        var get = function (name) {
            var value = '';
            if (supports_html5_storage) {
                value = localStorage["Eplat." + location.hostname + "." + name];
            } else {
                value = UI.Storage.Cookies.get("Eplat." + location.hostname + "." + name);
            }
            return eval("(" + value + ")");
        }

        var Cookies = function () {
            var set = function (name, value, days) {
                if (days) {
                    var date = new Date();
                    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                    var expires = "; expires=" + date.toGMTString();
                }
                else var expires = "";
                document.cookie = name + "=" + value + expires + "; path=/";
            }

            var get = function (name) {
                var nameEQ = name + "=";
                var ca = document.cookie.split(';');
                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i];
                    while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                    if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
                }
                return null;
            }

            var clear = function (name) {
                Cookies.set(name, "", -1);
            }

            return {
                set: set,
                get: get,
                clear: clear
            }
        }();

        return {
            save: save,
            get: get,
            Cookies: Cookies
        }
    }();

    var Url = function () {
        var queryString = function (key) {
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
            console.log('queryString = ' + value);
            return value;
        }

        var changeQueryString = function (key, value) {
            if (value == '') {
                console.log('--');
                window.location.href =  window.location.origin + window.location.pathname;
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
                console.log(newQuery);
                window.location.search = newQuery;
            }
        }

        return {
            queryString: queryString,
            changeQueryString: changeQueryString
        }
    }();

    return {
        init: init,
        confirmation: confirmation,
        sendingInfo: sendingInfo,
        adjustCartHeight: adjustCartHeight,
        removeHash: removeHash,
        highlightSection: highlightSection,
        alerts: alerts,
        Search: Search,
        Storage: Storage,
        Url: Url
    }
}();

