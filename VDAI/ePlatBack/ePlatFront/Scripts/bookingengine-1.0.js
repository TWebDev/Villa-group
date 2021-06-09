var BookingEngine = function () {
    var scheduleContainer;
    var serviceBooker;
    var serviceType;
    var serviceTransportationFields;
    var serviceDatePicker;
    var serviceSchedulePicker;
    var serviceFlexibleSchedulePicker;
    var serviceAirline;
    var serviceFlight;
    var serviceHotelID;
    var serviceZoneID;
    var serviceRoundDiv;
    var serviceRound;
    var serviceRoundFields;
    var serviceRoundDate;
    var serviceRoundAirline;
    var serviceRoundFlightNumber;
    var serviceRoundTime;
    var serviceDetailsPicker;
    var serviceAddButton;
    var serviceTotal;
    var serviceSavings;
    var labelTotal;
    var labelSavings;
    var labelConfirmation;
    var form;
    var Cart;
    var paymentsProviderName;
    var paymentsProviderAccount;
    var paymentsMerchantID;
    var domain = "";

    var init = function () {
        //cargar elementos del carrito
        getElements();
        //cargar calendario
        $("#" + BookingEngine.serviceDatePicker).datepicker({
            showOn: 'both',
            buttonImage: '//eplatfront.villagroup.com/content/themes/mex/images/icon_calendar.jpg',
            buttonImageOnly: true,
            minDate: -1,
            changeMonth: true,
            changeYear: true,
            constrainInput: true,
            dateFormat: 'yy-mm-dd',
            beforeShowDay: BookingEngine.Service.closedDays
        });
        $("#" + BookingEngine.serviceRoundDate).datepicker({
            showOn: 'both',
            buttonImage: '//eplatfront.villagroup.com/content/themes/mex/images/icon_calendar.jpg',
            buttonImageOnly: true,
            minDate: -1,
            changeMonth: true,
            changeYear: true,
            constrainInput: true,
            dateFormat: 'yy-mm-dd'
        });
        $('#' + BookingEngine.serviceFlexibleSchedulePicker).datetimepicker({
            timeFormat: 'hh:mm tt',
            timeOnly: true,
            stepMinute: 5
        });
        $('#' + BookingEngine.serviceRoundTime).datetimepicker({
            timeFormat: 'hh:mm tt',
            timeOnly: true,
            stepMinute: 5
        });
        //verificar el tipo de proveedor de pago
        $.getJSON('//eplatfront.villagroup.com/Purchase/GetPaymentsProvider', null, function (data) {
            BookingEngine.paymentsProviderName = data.ProviderName;
            BookingEngine.paymentsProviderAccount = data.ProviderAccount;
            BookingEngine.paymentsMerchantID = data.MerchantID;
        });

        //eventos
        $("#" + BookingEngine.serviceDatePicker).on('change', function () {
            if (BookingEngine.Service.serviceType == 1) {
                BookingEngine.Service.getSchedulesAndPrices();
            } else {
                BookingEngine.Service.getTranportationPrices(BookingEngine.Service.serviceID);
            }            
        });

        $('#' + BookingEngine.serviceAddButton).on('click', function () {
            BookingEngine.Service.save();
            return false;
        });

        /*Transportation Quotes*/
        if ($('#txtResort').length > 0) {
            BookingEngine.TransportationQuotes.init();
        }
    }

    var getCart = function () {
        /*if (localStorage["Eplat." + window.location.host + ".BookingEngine.Cart"] == undefined || localStorage["Eplat." + window.location.host + ".BookingEngine.Cart"] == "undefined") {
            var _terminalid = 5;
            var _pointofsale = 43;
            var _currencyid = 1;
            if (window.location.href.indexOf("myvallartaexperience.com") >= 0) {
                _terminalid = 5;
                _pointofsale = 43;
            }
            if (window.location.href.indexOf("mx.") >= 0) {
                _currencyid = 2;
            }
            
            return {
                CartID: '00000000-0000-0000-0000-000000000000',
                Items: [],
                Total: 0,
                Savings: 0,
                CurrencyID: _currencyid,
                PointOfSaleID: _pointofsale,
                TerminalID: _terminalid,
                PromoID: null
            }
        } else {
            return eval("(" + localStorage["Eplat." + window.location.host + ".BookingEngine.Cart"] + ")");
        }*/
        var cart = UI.Storage.get("BookingEngine.Cart");
        if (cart == undefined || cart.CartID == "empty") {
            var _terminalid = 7;
            var _pointofsale = 77;
            var _currencyid = 1;
            if (window.location.href.indexOf("myvallartaexperience.com") >= 0) {
                _terminalid = 5;
                _pointofsale = 43;
            } else if (window.location.href.indexOf("mycaboexperience.com") >= 0) {
                _terminalid = 6;
                _pointofsale = 70;
            } else if (window.location.href.indexOf("mycancunexperience.com") >= 0) {
                _terminalid = 7;
                _pointofsale = 77;
            } else if (window.location.href.indexOf("myloretoexperience.com") >= 0) {
                _terminalid = 8;
                _pointofsale = 81;
            }
            if (window.location.href.indexOf("mx.") >= 0) {
                
                _currencyid = 2;
            }
            if (window.location.href.indexOf("localhost") >= 0) {
                _terminalid = 5;
                _pointofsale = 43;
            }

            return {
                CartID: '00000000-0000-0000-0000-000000000000',
                Items: [],
                Total: 0,
                Savings: 0,
                CurrencyID: _currencyid,
                PointOfSaleID: _pointofsale,
                TerminalID: _terminalid,
                PromoID: null,
                CampaignTag: null,
            }
        } else {
            return cart;
        }
    }

    var setCart = function (cart) {
        //localStorage["Eplat." + window.location.host + ".BookingEngine.Cart"] = $.toJSON(cart);
        UI.Storage.save("BookingEngine.Cart", cart, 90);
    }

    var deleteCart = function () {
        //localStorage["Eplat." + window.location.host + ".BookingEngine.Cart"] = undefined;
        var cart = { CartID: "empty" };
        UI.Storage.save("BookingEngine.Cart", cart, 90);
    }

    var getElements = function (stayInStep) {
        Cart = BookingEngine.getCart();
        var prop = 'ServiceDate';
        var asc = true;
        Cart.Items = Cart.Items.sort(function (a, b) {
            if (asc) return (a[prop] > b[prop]) ? 1 : ((a[prop] < b[prop]) ? -1 : 0);
            else return (b[prop] > a[prop]) ? 1 : ((b[prop] < a[prop]) ? -1 : 0);
        });
        if (Cart.Items.length == 0) {
            //no hay elementos aún
            $('#' + BookingEngine.scheduleContainer).html('<div style="width: 250px;">' + Language.No_Activities + '</div>');
            //ajustar altura
            if ($(window).width() < 768) {
                $('#purchase-process').animate({
                    marginLeft: '0px',
                    height: 50 + 'px'
                }, 'fast');
                $('#sub-shopping-cart').animate({
                    height: (50 + 150) + 'px'
                }, 'fast');
            }
        } else {
            //ya hay elementos, cargarlos
            var items = '';
            var lastDate = '';
            Cart.Items = Cart.Items.sort(custom_sort);
            $.each(Cart.Items, function (i, item) {
                if (item.Deleted != true) {
                    var details = '';
                    $.each(item.Details, function (j, detail) {
                        if (j > 0) {
                            if (j == item.Details.length - 1) {
                                details += ' & ';
                            } else {
                                details += ', ';
                            }
                        }
                        details += detail.Quantity + ' ' + detail.Unit;
                    });
                    if (item.ServiceDate != lastDate) {
                        if (i > 0) {
                            items += '</div>';
                        }
                        items += '<div class="cart-day"><span>' + item.ServiceDate + '</span>';
                    }

                    items += '<div class="cart-item ' + (item.PromoID != null ? 'promo-item' : '') + '" id="' + item.CartItemID + '">'
                    + '<span class="delete"><img src="/Content/themes/mex/images/icon_delete.png" width="16" /></span>'
                    + '<span class="edit"><img src="/Content/themes/mex/images/icon_edit.png" width="16" /></span>'
                    + '<span class="total ' + (item.PromoID != null ? 'promo-total' : '') + '">$' + item.PromoTotal.toFixed(2) + '</span>'
                    + '<span class="activity"><span class="activity-inside">' + item.Service + '</span></span>'
                    + '<span class="schedule">' + item.Schedule + '</span>'
                    + '<span class="details">' + details + '</span>'
                    + (item.PromoID != null ? '<span class="promo-applied">' + Language.Promo_Applied + '</span>' : '')
                    + '</div>';

                    lastDate = item.ServiceDate;
                }
            });
            items += '</div>';
            $('#' + BookingEngine.scheduleContainer).html(items);
            $('.cart-item span').not('.delete').on('click', function () {
                BookingEngine.Service.edit($(this).parent().attr('id'));
            });

            $('.cart-item span.delete').on('click', function () {
                $(this).parent().slideUp('fast', function () {
                    BookingEngine.Service.deleteItem($(this).attr('id'));
                });
            });
            //ajustar altura
            if (stayInStep != true) {
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
                        height: $('#step1').height() + 'px'
                    }, 'fast');
                    $('#sub-shopping-cart').animate({
                        height: '300px'
                    }, 'fast');
                }
            }

            //$('#' + BookingEngine.scheduleContainer).trigger('loaded');
        }
        $('#' + BookingEngine.labelTotal).text('$' + Cart.Total.toFixed(2));
        $('#' + BookingEngine.labelSavings).text('$' + Cart.Savings.toFixed(2));
    }

    var applyPromo = function () {
        var Cart = BookingEngine.getCart();
        if (Cart.Items.length > 0) {
            //recalcular servicios antes de aplicar la promo
            $.each(Cart.Items, function (j, item) {
                //if (item.PromoID == null) {
                    item.PromoTotal = item.Total;
                    item.PromoSavings = item.Savings;
                    Cart.Items[j] = item;
                //}                
            });
            //buscar promos
            var serviceids = '';
            var travelDates = '';
            var conditionalPrice = false;
            $.each(Cart.Items, function (i, cartitem) {
                if (cartitem.Deleted != true) {
                    if (serviceids != '') {
                        serviceids += ',';
                        travelDates += ',';
                    }
                    serviceids += cartitem.ServiceID;
                    travelDates += cartitem.ServiceDate;
                }
            });

            if (serviceids != '') {
                var dataObject = {
                    services: serviceids,
                    traveldates: travelDates,
                    terminalid: Cart.TerminalID
                }
                $.getJSON('//eplatfront.villagroup.com/Purchase/GetPromo', dataObject, function (data) {
                    //aplicar la promo a los items y guardarlos en el cartlocal
                    if (data.ResponseType == 1) {
                        $.each(data.Promos, function (p, promo) {
                            if (promo.Apply) {

                                //aplicar promo
                                var arrServices = promo.ServiceIDs.split(',');
                                switch (promo.PromoTypeID) {
                                    case 1:
                                    case 2:
                                    case 3:
                                        if (promo.ApplyOnPerson) {
                                            for (var i = 0; i < arrServices.length; i++) {
                                                $.each(Cart.Items, function (j, item) {
                                                    var AppliedPromo = (item.PromoID != null ? true : false);
                                                    if (item.ServiceID == arrServices[i]) {
                                                        var totalPax = 0;
                                                        var cheapestUnit = 0;
                                                        $.each(item.Details, function (k, detail) {
                                                            //console.log(detail);
                                                            if (!detail.ConditionalPrice) {
                                                                //console.log('a');
                                                                totalPax += parseInt(detail.Quantity);
                                                            } else {
                                                                //console.log('b');
                                                                conditionalPrice = true;
                                                            }
                                                            if ((parseFloat(detail.Price) < parseFloat(cheapestUnit) && parseFloat(detail.Price) != 0) || parseFloat(cheapestUnit) == 0) {
                                                                cheapestUnit = detail.Price;
                                                            }
                                                        });
                                                        if ((totalPax >= 4 && promo.PromoTypeID == 3) || (totalPax >= 3 && promo.PromoTypeID == 2) || (totalPax >= 2 && promo.PromoTypeID == 1)) {
                                                            var savings = 0;
                                                            var totalPromo = 0;

                                                            if (totalPax >= 4 && promo.PromoTypeID == 3) {
                                                                totalPromo = parseFloat(item.Total) - parseFloat(cheapestUnit * (parseInt(totalPax / 4)));
                                                                savings = parseFloat(cheapestUnit * (parseInt(totalPax / 4))) + parseFloat(item.Savings);
                                                            } else if (totalPax >= 3 && promo.PromoTypeID == 2) {
                                                                totalPromo = parseFloat(item.Total) - parseFloat(cheapestUnit * (parseInt(totalPax / 3)));
                                                                savings = parseFloat(cheapestUnit * (parseInt(totalPax / 3))) + parseFloat(item.Savings);
                                                            } else if (totalPax >= 2 && promo.PromoTypeID == 1) {
                                                                totalPromo = parseFloat(item.Total) - parseFloat(cheapestUnit * (parseInt(totalPax / 2)));
                                                                savings = parseFloat(cheapestUnit * (parseInt(totalPax / 2))) + parseFloat(item.Savings);
                                                            }
                                                            if (!conditionalPrice) {
                                                                Cart.Items[j].PromoSavings = savings;
                                                                Cart.Items[j].PromoID = promo.PromoID;
                                                                Cart.Items[j].PromoTotal = totalPromo;
                                                                if (!AppliedPromo) {
                                                                    alert(promo.PromoType + ' has been applied to ' + item.Service);
                                                                }
                                                            }
                                                        }
                                                    }
                                                });
                                            }
                                        } else {
                                            var cheapestActivity;
                                            var cheapestTotal = 0;
                                            var promoUnits = 0;
                                            for (var i = 0; i < arrServices.length; i++) {
                                                $.each(Cart.Items, function (j, item) {
                                                    //item.PromoID = null;
                                                    if (item.ServiceID == arrServices[i]) {
                                                        var units = 0;
                                                        $.each(item.Details, function (k, detail) {
                                                            units += parseInt(detail.Quantity);
                                                        });
                                                        if (cheapestTotal == 0) {
                                                            cheapestTotal = item.Total;
                                                            cheapestActivity = item.ServiceID;
                                                            promoUnits = units;
                                                        } else {
                                                            if (item.Total < cheapestTotal) {
                                                                cheapestTotal = item.Total;
                                                                cheapestActivity = item.ServiceID;
                                                            }
                                                        }
                                                        if (cheapestActivity != item.ServiceID) {
                                                            if (units < promoUnits || promoUnits == 0) {
                                                                promoUnits = units;
                                                            }
                                                        }
                                                    }
                                                });

                                            }
                                            $.each(Cart.Items, function (j, item) {
                                                var AppliedPromo = (item.PromoID != null ? true : false);
                                                if (item.ServiceID == cheapestActivity) {
                                                    var units = 0;
                                                    $.each(item.Details, function (k, detail) {
                                                        units += parseInt(detail.Quantity);
                                                    });
                                                    if (units == promoUnits) {
                                                        //aplicar el 0
                                                        Cart.Items[j].PromoTotal = 0;
                                                        Cart.Items[j].PromoSavings = parseFloat(item.Total) + parseFloat(item.Savings);
                                                        Cart.Items[j].PromoID = promo.PromoID;
                                                        if (!AppliedPromo) {
                                                            alert(promo.PromoType + Language.Applied_to + item.Service);
                                                        }
                                                    }
                                                }
                                            });
                                        }

                                        break;
                                    case 4: //discount
                                        for (var i = 0; i < arrServices.length; i++) {
                                            $.each(Cart.Items, function (j, item) {
                                                var AppliedPromo = (item.PromoID != null ? true : false);
                                                //item.PromoID = null;
                                                if (item.ServiceID == arrServices[i]) {
                                                    if (promo.DiscountType == '%') {
                                                        //descuento de porcentaje
                                                        Cart.Items[j].PromoTotal = parseFloat(item.Total) * parseFloat(promo.Discount) / 100;
                                                        Cart.Items[j].PromoSavings = parseFloat(item.Total) + parseFloat(item.Savings);
                                                        Cart.Items[j].PromoID = promo.PromoID;
                                                        if (!AppliedPromo) {
                                                            alert(promo.PromoType + Language.Applied_to + item.Service);
                                                        }
                                                    } else {
                                                        //descuento monetario
                                                    }
                                                }
                                            });
                                        }                                        
                                        break;
                                }
                            } else {
                                Cart.PromoID = null;
                                var arrServices = promo.ServiceIDs.split(',');
                                for (var i = 0; i < arrServices.length; i++) {
                                    $.each(Cart.Items, function (j, item) {
                                        if (item.ServiceID == arrServices[i]) {
                                            item.PromoID = null;
                                        }
                                    });
                                }
                                if (promo.Condition != null && promo.Condition != '') {
                                    alert(promo.Condition);
                                }
                            }
                        });
                    }
                    BookingEngine.setCart(Cart);
                    BookingEngine.calculateTotal(Cart);
                    BookingEngine.getElements();
                });
            }
        } else {
            BookingEngine.calculateTotal(Cart);
            BookingEngine.getElements();
        }
    }

    var calculateTotal = function (Cart) {
        //sumar los elementos
        var total = 0;
        $.each(Cart.Items, function (i, cartitem) {
            if (cartitem.Deleted != true) {
                total += parseFloat(cartitem.PromoTotal);
            }
        });

        Cart.Total = total;

        //sumar savings de los elementos
        var savings = 0;
        $.each(Cart.Items, function (j, item) {
            if (item.Deleted != true) {
                savings += parseFloat(item.PromoSavings);
            }
        })

        Cart.Savings = savings;

        BookingEngine.setCart(Cart);

        return Cart;
    }

    function custom_sort(a, b) {
        return new Date(a.serviceDate).getTime() - new Date(b.serviceDate).getTime();
    }

    var showValidationSummary = function () {
        if (!$("#" + BookingEngine.form).valid()) {
            if ($('#divPurchaseValidation .step2-validation .field-validation-error span').length > 0) {
                $('.step2-validation').show();
            } else {
                $('.step2-validation').hide();
            }
            if ($('#divPurchaseValidation .step3-validation .field-validation-error span').length > 0) {
                $('.step3-validation').show();
            } else {
                $('.step3-validation').hide();
            }
            if ($(window).width() < 768) {
                $('#divPurchaseValidation').height($(window).height() + 'px');
            }
            $('#divPurchaseValidation').slideDown('fast', function () {
                $('#sub-shopping-cart').scrollTop(0);
            });
        }
    }

    var Service = function () {
        var serviceID;
        var service;
        var serviceType;
        var cartItemID;
        var process;
        var c = 1;
        var indicador, disabledDays, calendarServiceID;

        function clearBookingForm() {
            $('#' + BookingEngine.serviceDatePicker).val('');
            $('#' + BookingEngine.serviceSchedulePicker).clearSelect();
            $('#' + BookingEngine.serviceDetailsPicker).html('');
            $('#' + BookingEngine.serviceTotal).html('');

            $('#' + serviceFlexibleSchedulePicker).val('');
            $('#' + BookingEngine.serviceHotelID).val(0);
            $('#' + BookingEngine.serviceZoneID).val(0);
            $('#divTransportationHotel').show();
            $('#divTransportationZone').hide();

            $('#' + BookingEngine.serviceAirline).val('');
            $('#' + BookingEngine.serviceFlight).val('');

            $('#' + BookingEngine.serviceRoundFields).hide();
            $('#' + BookingEngine.serviceRoundAirline).val('');
            $('#' + BookingEngine.serviceRoundFlightNumber).val('');
            $('#' + BookingEngine.serviceRoundTime).val('');
        }

        function showBookingForm() {
            $('#' + BookingEngine.serviceBooker + ' .booking').text(Language.Booking + ' ' + BookingEngine.Service.service);
            $('#' + BookingEngine.serviceBooker).slideDown('fast', function () {
                $('#' + BookingEngine.serviceBooker).trigger('shown');
                UI.adjustCartHeight();
            });            
        }

        var edit = function (cartItemID) {
            BookingEngine.Service.cartItemID = cartItemID;
            BookingEngine.Service.process = 'editing';
            //limpiar el formulario
            clearBookingForm();
            //cargar datos
            Cart = BookingEngine.getCart();
            $.each(Cart.Items, function (i, item) {
                if (item.CartItemID == cartItemID) {
                    BookingEngine.Service.serviceID = item.ServiceID;
                    BookingEngine.Service.service = item.Service;
                    BookingEngine.Service.serviceType = item.ServiceType;
                    //preparar form según tipo de servicio
                    prepareServiceType(item.ServiceID);
                    //asignar fecha
                    $('#' + BookingEngine.serviceDatePicker).val(item.ServiceDate);
                    $("#" + BookingEngine.serviceDatePicker).trigger('change');
                    if (BookingEngine.Service.serviceType == 1) {
                        //actividad
                        $('#' + BookingEngine.serviceSchedulePicker).off('loaded').on('loaded', function () {
                            //asignar horario
                            //$('#' + BookingEngine.serviceSchedulePicker).val(item.WeeklyAvailabilityID);
                            $("#ActivitySchedule").find("option").filter(function (index) {
                                return item.Schedule === $(this).text();
                            }).prop("selected", "selected");
                        });
                        showBookingForm();
                    } else {
                        //transportacion
                        $('#' + BookingEngine.serviceFlexibleSchedulePicker).val(item.Schedule);
                        $('#' + BookingEngine.serviceHotelID).off('loaded').on('loaded', function () {
                            $('#' + BookingEngine.serviceHotelID).val(item.HotelID);
                            if (item.HotelID == '-1') {
                                $('#divTransportationHotel').slideUp('fast');
                                $('#divTransportationZone').slideUp('fast');
                                $('#' + BookingEngine.serviceZoneID).val(item.TransportationZoneID);
                            }
                            $('#' + BookingEngine.serviceHotelID).trigger('change');
                        });
                        $('#' + BookingEngine.serviceAirline).val(item.Airline);
                        $('#' + BookingEngine.serviceFlight).val(item.Flight);
                        $('input[name="' + BookingEngine.serviceRound + '"][value="' + item.Round + '"]').prop('checked', true);
                        if (item.Round == true) {
                                $('#' + BookingEngine.serviceRoundFields).show();
                                $('#' + BookingEngine.serviceRoundDate).val(item.RoundDate);
                                $('#' + BookingEngine.serviceRoundAirline).val(item.RoundAirline);
                                $('#' + BookingEngine.serviceRoundFlightNumber).val(item.RoundFlightNumber);
                                $('#' + BookingEngine.serviceRoundTime).val(item.RoundMeetingTime);
                        } else {
                            $('#' + BookingEngine.serviceRoundFields).hide();
                        }
                        UI.adjustCartHeight();
                    }
                    $('#' + BookingEngine.serviceDetailsPicker).off('loaded').on('loaded', function () {
                        //asignar detalles
                        $.each(item.Details, function (j, detail) {
                            $('#' + detail.PriceID).find('input[type=text]').val(detail.Quantity).trigger('keyup');
                        });
                    });
                }
            });
            //guardar el cart
            BookingEngine.setCart(Cart);
        }

        var book = function (id, service) {
            BookingEngine.Service.serviceID = id;
            BookingEngine.Service.service = service;
            BookingEngine.Service.process = 'adding';
            //limpiar el formulario
            clearBookingForm();
            //revisar si se trata de un tour o de transportación
            prepareServiceType(id);
            //asegurar que el slide está en el step 1
            $('#purchase-process').animate({
                marginLeft: '0px',
                height: $('#step1').height() + 'px'
            }, 'fast');
            $('#divTotals').fadeIn('fast');
            $('#btnGoCheckout').show();
            $('#btnGoCheckout2').hide();
            $('#divAddActivityBg').fadeIn('fast');
            //si está en villagroup resorts ocultar el panel de booking.
            if (window.location.href.indexOf("villagroupresorts.com") > -1 && $('.blue.ocultar').length == 0) {
                $('#book_show').trigger('click');
                $('html,body').animate({ scrollTop: $('#' + BookingEngine.serviceBooker).offset().top }, 500);
                //console.log('a');
            }
            else if (window.location.href.indexOf("villagroupresorts.com") > -1) {
                $('html,body').animate({ scrollTop: $('#' + BookingEngine.serviceBooker).offset().top }, 500);
                //console.log('b');
            } else if (window.location.href.indexOf("villagroupresorts.com") == -1) {
                $('html,body').animate({ scrollTop: 0 }, 500);
                //console.log('c');
            }
        }

        function prepareServiceType(id) {
            var Cart = BookingEngine.getCart();
            var dataObject = {
                terminalid: Cart.TerminalID
            }
            $.getJSON('//eplatfront.villagroup.com/Activities/GetServiceType/' + id, dataObject, function (data) {
                $('#' + BookingEngine.serviceType).val(data.ServiceType);
                BookingEngine.Service.serviceType = data.ServiceType;
                if (data.ServiceType == 1) {
                    //activity
                    $('.fixed-time').show();
                    $('.flexible-time').hide();
                    $('#' + BookingEngine.serviceTransportationFields).hide();
                } else {
                    //transportation
                    /*airline, flight number, hotel*/
                    $('#' + BookingEngine.serviceHotelID).fillSelect(data.Hotels).off('change').on('change', function () {
                        if ($('#' + BookingEngine.serviceHotelID).val() != 0) {
                            if ($('#' + BookingEngine.serviceHotelID).val() == '-1') {
                                $('#divTransportationHotel').slideUp('fast');
                                $('#divTransportationZone').slideDown('fast');
                            } else {
                                //obtener los precios
                                BookingEngine.Service.getTranportationPrices(id);
                            }
                        }
                    });
                    $('#' + BookingEngine.serviceZoneID).fillSelect(data.Zones).off('change').on('change', function () {
                        BookingEngine.Service.getTranportationPrices(id);
                    });
                    $('#' + BookingEngine.serviceHotelID).trigger('loaded');
                    $('.fixed-time').hide();
                    $('.flexible-time').show();
                    if (data.OffersRoundTrip) {
                        $('#' + BookingEngine.serviceRoundDiv).show();
                        $('input:radio[name=' + BookingEngine.serviceRound + ']').off('click').on('click', function () {
                            if ($('input:radio[name=' + BookingEngine.serviceRound + ']:checked').val() == 'true') {
                                $('#' + BookingEngine.serviceRoundFields).show();
                                UI.adjustCartHeight();
                            } else {
                                $('#' + BookingEngine.serviceRoundFields).hide();
                                UI.adjustCartHeight();
                            }
                            $('#' + BookingEngine.serviceHotelID).trigger('change');
                        });
                    } else {
                        $('#' + BookingEngine.serviceRoundDiv).hide();
                    }
                    $('#' + BookingEngine.serviceTransportationFields).show();
                }
                //mostrar el formulario
                showBookingForm();
            });
        }

        var getTranportationPrices = function(id) {
            if ($('#' + BookingEngine.serviceHotelID).val() != 0) {
                $('#' + BookingEngine.serviceBooker + ' .working').show();
                var Cart = BookingEngine.getCart();
                var dataObject = {
                    id: id,
                    date: $('#' + BookingEngine.serviceDatePicker).val(),
                    placeId: $('#' + BookingEngine.serviceHotelID).val(),
                    transportationZoneId: $('#' + BookingEngine.serviceZoneID).val(),
                    round: $('input:radio[name=' + BookingEngine.serviceRound + ']:checked').val(),
                    terminalid: Cart.TerminalID,
                    culture: (Cart.CurrencyID == 2 ? "es-MX" : "en-US")
                };
                $.getJSON('//eplatfront.villagroup.com/Activities/GetTransportationPrices/', dataObject, function (data) {
                    setPricesUI(data);
                    $('#' + BookingEngine.serviceBooker + ' .working').hide();
                });
            }
        }

        var deleteItem = function (id) {
            $('#' + BookingEngine.serviceBooker).slideUp('fast', function () {
                var Cart = BookingEngine.getCart();
                var items = [];
                $.each(Cart.Items, function (i, item) {
                    if (item.CartItemID != id) {
                        items.push(item);
                    } else {
                        if (item.DateSaved != null) {
                            item.Deleted = true;
                            items.push(item);
                        }
                    }
                });
                Cart.Items = items;
                Cart = BookingEngine.calculateTotal(Cart);
                //guardar el cart
                BookingEngine.setCart(Cart);
                //buscar promos y actualizar el cart
                BookingEngine.applyPromo();
            });
        }

        var closedDays = function (date) {
            if ((date.getMonth() != indicador && c > 1 && date.getDay() == 0) || (date.getDate() == 1 && date.getDay() == 0)) {
                c = 1;
            }
            if (c == date.getDate()) {
                c++;
                if (indicador != date.getMonth() || calendarServiceID != BookingEngine.Service.serviceID) {
                    BookingEngine.Service.getDisabledDays(date.getMonth(), date.getDate(), date.getFullYear());
                }
                var m = date.getMonth(), d = date.getDate(), y = date.getFullYear();
                for (i = 0; i < disabledDays.length; i++) {
                    if ($.inArray(y + '-' + (m + 1) + '-' + d, disabledDays) != -1 || new Date().getTime() + 1 * 24 * 60 * 60 * 1000 >= date) {
                        return [false];
                    }
                }
                return [true];
            } else {
                c = 1;
                return [false];
            }
        }

        var getDisabledDays = function (mes, dia, ano) {
            $('#' + BookingEngine.serviceBooker + ' .working').show();
            var myData = {
                month: parseInt(mes) + 1,
                year: ano
            };

            $.ajax({
                url: '/Activities/GetAvailableDates/' + BookingEngine.Service.serviceID,
                dataType: 'json',
                async: false,
                data: myData,
                success: function (data) {
                    disabledDays = data.DatesString.split(',');
                    indicador = mes;
                    calendarServiceID = BookingEngine.Service.serviceID;
                    $('#' + BookingEngine.serviceBooker + ' .working').hide();
                }
            });
        }

        var getSchedulesAndPrices = function () {
            if (BookingEngine.Service.serviceType == 1) {
                $('#' + BookingEngine.serviceBooker + ' .working').show();
                var Cart = BookingEngine.getCart();
                var dataObject = {
                    date: $("#" + BookingEngine.serviceDatePicker).val(),
                    terminalid: Cart.TerminalID,
                    culture: (Cart.CurrencyID == 2 ? "es-MX" : "en-US")
                }
                $.getJSON('//eplatfront.villagroup.com/Activities/GetSchedulesAndPrices/' + BookingEngine.Service.serviceID, dataObject, function (data) {
                    $('#' + BookingEngine.serviceDetailsPicker).html('');
                    $('#' + BookingEngine.serviceSchedulePicker).fillSelect(data.Schedules);
                    $('#' + BookingEngine.serviceSchedulePicker).trigger('loaded');
                    if ($('#' + BookingEngine.serviceSchedulePicker + ' option').length == 2) {
                        $('#' + BookingEngine.serviceSchedulePicker + ' option').eq(1).prop('selected', true);
                    }
                    setPricesUI(data);
                    $('#' + BookingEngine.serviceBooker + ' .working').hide();
                });
            }
        }

        function setPricesUI(data) {
            $('#' + BookingEngine.serviceDetailsPicker).html('');
            $.each(data.Prices, function (i, price) {
                $('#' + BookingEngine.serviceDetailsPicker).append('<div class="trbody" id="' + price.PriceID + '" ' + (price.DependingOnPriceID != null ? 'data-dependentid="' + price.DependingOnPriceID + '" data-dependentqty="' + price.DependingOnPriceQuantity + '" style="display:none"' : '') + '><div class="td-qty"><input type="text" /></div><div class="td-unit">' + price.Unit + (price.UnitMin != null ? ' (' + price.UnitMin + ' - ' : '') + (price.UnitMax != null ? price.UnitMax + ' ' + ($('#' + BookingEngine.serviceType).val() == '1' ? 'years' : 'pax') + ')' : '') + '</div><div class="td-price">$<span class="td-price-offer">' + price.OfferPrice + '</span><span class="td-price-retail" ' + (parseInt(price.RetailPrice) == 0 ? 'style="display:none;"' : '') + '>$' + price.RetailPrice + '</span><input class="td-price-savings" type="hidden" value="' + (parseFloat(price.RetailPrice) - parseFloat(price.OfferPrice)) + '" /><input type="hidden" class="price-type" value="' + price.PriceTypeID + '" /><input type="hidden" class="price-exchange" value="' + price.ExchangeRateID + '" /></div><div class="td-totals">$<span class="td-totals-amount">0</span><input type="hidden" class="td-totals-savings" value="0" /></div></div>');
            });
            $('#' + BookingEngine.serviceDetailsPicker + ' input[type=text]').on('keyup', function () {
                var content = $(this).val().replace(/\s+/g, '').split('');
                if (content.length > 0) {
                    var qty = '';
                    for (var i = 0; i < content.length; i++) {
                        if (!isNaN(content[i])) {
                            qty += content[i];
                        }
                    }

                    if (qty == '') {
                        qty = 0;
                    }
                } else {
                    qty = 0;
                }

                $(this).val((qty > 0 ? qty : ''));

                //calcular total de esa unidad
                $(this).parent().siblings('.td-totals').children().eq(0).text((parseFloat($(this).parent().siblings('.td-price').children().eq(0).text()) * parseInt(qty)).toFixed(2));

                //calcular savings de esa unidad
                $(this).parent().siblings('.td-totals').children().eq(1).val((parseFloat($(this).parent().siblings('.td-price').children().eq(2).val()) * parseInt(qty)).toFixed(2));

                //calcular total de actividad
                var total = 0;
                $('.td-totals-amount').each(function (i) {
                    total += parseFloat($(this).text());
                });
                $('#' + BookingEngine.serviceTotal).text('$' + total.toFixed(2));

                //calcular total de ahorros
                var savings = 0;
                $('.td-totals-savings').each(function (i) {
                    savings += (parseFloat($(this).val()) > 0 ? parseFloat($(this).val()) : 0);
                });
                $('#' + BookingEngine.serviceSavings).val(savings.toFixed(2));

                //buscar precios dependientes y mostrarlos
                if (qty > 0 && $('[data-dependentid="' + $(this).closest('.trbody').attr('id') + '"]').length > 0 && qty >= parseFloat($('[data-dependentid="' + $(this).closest('.trbody').attr('id') + '"]').attr('data-dependentqty'))) {
                    $('[data-dependentid="' + $(this).closest('.trbody').attr('id') + '"]').show();
                }
                if (qty <= 0 && $('[data-dependentid="' + $(this).closest('.trbody').attr('id') + '"]').length > 0 && qty < parseFloat($('[data-dependentid="' + $(this).closest('.trbody').attr('id') + '"]').attr('data-dependentqty'))) {
                    $('[data-dependentid="' + $(this).closest('.trbody').attr('id') + '"]').hide();
                }

                //validar que el precio dependiente cumpla con la relación de cantidad
                if ($(this).closest('.trbody').attr('data-dependentid') != null) {
                    //suma de otros precios dependientes
                    var conditionalUnits = 0;
                    $('div[data-dependentid="' + $(this).closest('.trbody').attr('data-dependentid') + '"] input[type="text"]').not(this).each(function () {
                        //console.log('x "' + $(this).val() != '' && !isNaN($(this).val() + '"'));
                       conditionalUnits += ($(this).val() != '' && !isNaN($(this).val()) ? parseFloat($(this).val()) : 0);
                        //console.log('x ' + conditionalUnits);
                    });
                    //console.log(conditionalUnits);

                    //
                    var cvalue = ($(this).val() != "" && !isNaN($(this).val()) ? parseFloat($(this).val()) : 0);
                    var pvalue = parseFloat($('#' + $(this).closest('.trbody').attr('data-dependentid') + ' input[type="text"]').val()) / parseFloat($(this).closest('.trbody').attr('data-dependentqty'));
                    //console.log(pvalue + '-' + conditionalUnits + '-' + cvalue);
                    
                    if (conditionalUnits > 0) {
                        //console.log('a ' + (cvalue + conditionalUnits));
                        if (pvalue < (cvalue + conditionalUnits)) {
                            $(this).val('');
                        }
                    } else {
                        //console.log('b ' + pvalue + ' - ' + cvalue);
                        if (pvalue < cvalue) {
                            $(this).val(parseInt(pvalue));
                        }
                    }                    
                }

                UI.adjustCartHeight();
            });
            $('#' + BookingEngine.serviceDetailsPicker).trigger('loaded');
            UI.adjustCartHeight();
        }

        var save = function () {
            //verificar que se pueda agregar
            var valid = true;
            var msg = Language.Check_Issues;
            if ($("#" + BookingEngine.serviceDatePicker).val() == "" || $("#" + BookingEngine.serviceDatePicker).val() == "yyyy-mm-dd") {
                valid = false;
                msg += '\n- ' + Language.Field_Date;
            } else {
                if (BookingEngine.Service.serviceType == 1) {
                    //activity
                    if ($('#' + BookingEngine.serviceSchedulePicker).val() == "" || $('#' + BookingEngine.serviceSchedulePicker).val() == "0") {
                        valid = false;
                        msg += '\n- ' + Language.Field_Schedule;
                    }
                } else {
                    //transportation
                    //console.log('validation');
                    if ($('#' + BookingEngine.serviceFlexibleSchedulePicker).val() == "" || $('#' + BookingEngine.serviceFlexibleSchedulePicker).val() == "0") {
                        valid = false;
                        msg += '\n- ' + Language.Field_Schedule;
                    }
                    if ($('#' + BookingEngine.serviceHotelID).val() == '') {
                        valid = false;
                        msg += '\n- ' + Language.Field_Hotel;
                    } else if ($('#' + BookingEngine.serviceHotelID).val() == '-1') {
                        if ($('#' + BookingEngine.serviceZoneID).val() == '0') {
                            valid = false;
                            msg += '\n- ' + Language.Field_Zone;
                        }
                    }
                    if ($('#' + BookingEngine.serviceAirline).val() == '') {
                        valid = false;
                        msg += '\n- ' + Language.Field_Airline;
                    }
                    if ($('#' + BookingEngine.serviceFlight).val() == '') {
                        valid = false;
                        msg += '\n- ' + Language.Field_FlightNumber;
                    }
                    if ($('input:radio[name=' + BookingEngine.serviceRound + ']:checked').val() == "true") {
                        if ($('#' + BookingEngine.serviceRoundDate).val() == '') {
                            valid = false;
                            msg += '\n- ' + Language.Field_RoundDate;
                        }
                        if ($('#' + BookingEngine.serviceRoundAirline).val() == '') {
                            valid = false;
                            msg += '\n- ' + Language.Field_RoundAirline;
                        }
                        if ($('#' + BookingEngine.serviceRoundFlightNumber).val() == '') {
                            valid = false;
                            msg += '\n- ' + Language.Field_RoundFlightNumber;
                        }
                        if ($('#' + BookingEngine.serviceRoundTime).val() == '') {
                            valid = false;
                            msg += '\n- ' + Language.Field_RoundTime;
                        }
                    }
                }
                if ($('#' + BookingEngine.serviceTotal).text() == "$0" || $('#' + BookingEngine.serviceTotal).text() == "") {
                    valid = false;
                    msg += '\n- ' + Language.Field_Details;
                }
            }
            if (isNaN($('#' + BookingEngine.serviceTotal).text().replace('$', '')) || (!isNaN($('#' + BookingEngine.serviceTotal).text().replace('$', '')) && parseFloat($('#' + BookingEngine.serviceTotal).text().replace('$', '')) <= 0)) {
                valid = false;
                msg += '\n- ' + Language.Total_More_Than_Zero;
            }

            //verificar que no haya ya el mismo tour para la misma fecha

            if (valid) {
                Cart = BookingEngine.getCart();
                //get item
                var item = {
                    ServiceID: BookingEngine.Service.serviceID,
                    Service: BookingEngine.Service.service,
                    ServiceType: BookingEngine.Service.serviceType,
                    ServiceDate: $("#" + BookingEngine.serviceDatePicker).val(),
                    WeeklyAvailabilityID: $('#' + BookingEngine.serviceSchedulePicker).val(),
                    Schedule: (BookingEngine.Service.serviceType == 1 ? $('#' + BookingEngine.serviceSchedulePicker + ' option:selected').text() : $('#' + BookingEngine.serviceFlexibleSchedulePicker).val()),
                    Airline: $('#' + BookingEngine.serviceAirline).val(),
                    Flight: $('#' + BookingEngine.serviceFlight).val(),
                    HotelID: $('#' + BookingEngine.serviceHotelID).val(),
                    TransportationZoneID: $('#' + BookingEngine.serviceZoneID).val(),
                    Round: $('input:radio[name=' + BookingEngine.serviceRound + ']:checked').val(),
                    RoundDate: $('#' + BookingEngine.serviceRoundDate).val(),
                    RoundAirline: $('#' + BookingEngine.serviceRoundAirline).val(),
                    RoundFlightNumber: $('#' + BookingEngine.serviceRoundFlightNumber).val(),
                    RoundMeetingTime: $('#' + BookingEngine.serviceRoundTime).val(),
                    Total: parseFloat($('#' + BookingEngine.serviceTotal).text().replace('$', '')),
                    PromoTotal: parseFloat($('#' + BookingEngine.serviceTotal).text().replace('$', '')),
                    Savings: parseFloat($('#' + BookingEngine.serviceSavings).val()),
                    PromoSavings: parseFloat($('#' + BookingEngine.serviceSavings).val()),
                    PromoID: null,
                    Details: []
                }
                //get details
                $('#' + BookingEngine.serviceDetailsPicker + ' .trbody').each(function (i, row) {
                    if (!isNaN($(row).find('input').val()) && $(row).find('input').val() > 0) {
                        var detail = {
                            PriceID: $(row).attr('id'),
                            PriceTypeID: $(row).find('.price-type').val(),
                            Quantity: $(row).find('.td-qty input').val(),
                            Price: $(row).find('.td-price-offer').text(),
                            Unit: $(row).find('.td-unit').text(),
                            ExchangeRateID: (!isNaN($(row).find('.price-exchange').val()) ? $(row).find('.price-exchange').val() : null),
                            ConditionalPrice: ($(row).attr('data-dependentid') != undefined ? true : false)
                        }
                        item.Details.push(detail);
                    }
                });
                if (BookingEngine.Service.process == "adding") {
                    item.CartItemID = new Date().getTime();
                    Cart.Items.push(item);
                } else {
                    $.each(Cart.Items, function (i, cartitem) {
                        if (cartitem.CartItemID == BookingEngine.Service.cartItemID) {
                            item.CartItemID = BookingEngine.Service.cartItemID;
                            item.DateSaved = Cart.Items[i].DateSaved;
                            Cart.Items[i] = item;
                        }
                    });
                }

                BookingEngine.setCart(Cart);

                $('#' + BookingEngine.serviceBooker).slideUp('fast');
                $('#divAddActivityBg').fadeOut('fast');

                //buscar promos y actualizar el cart
                BookingEngine.applyPromo();
            } else {
                alert(msg);
            }
        }

        return {
            serviceID: serviceID,
            service: service,
            serviceType: serviceType,
            process: process,
            book: book,
            edit: edit,
            deleteItem: deleteItem,
            getDisabledDays: getDisabledDays,
            closedDays: closedDays,
            getSchedulesAndPrices: getSchedulesAndPrices,
            getTranportationPrices: getTranportationPrices,
            save: save
        }
    }()

    var Checkout = function () {

    }()

    var TransportationQuotes = function () {
        var Resorts;

        function searchStringFilter(str) {
            str = str.toLowerCase();
            str = str.replace(/á/g, 'a');
            str = str.replace(/é/g, 'e');
            str = str.replace(/í/g, 'i');
            str = str.replace(/ó/g, 'o');
            str = str.replace(/ú/g, 'u');
            return str;
        }

        function selectResort(selectedIndex) {
            $('#hdnResortID').val($('#ulResorts>li').eq(selectedIndex).attr('data-value'));
            $('#txtResort').val($('#ulResorts>li').eq(selectedIndex).text());
            $('#ulResorts').html('');

            //obtener Precios
            var Cart = BookingEngine.getCart();

            var date = new Date();
            var yyyy = date.getFullYear().toString();
            var mm = (date.getMonth() + 1).toString();
            var dd = date.getDate().toString();
            var cdate = yyyy + '-' + (mm[1] ? mm : "0" + mm[0]) + '-' + (dd[1] ? dd : "0" + dd[0]);
            
            var dataObject = {
                id: $('#TransportationServiceID').val(),
                date:cdate,
                placeId: $('#hdnResortID').val(),
                transportationZoneId: 0,
                terminalid: Cart.TerminalID,
                culture: (Cart.CurrencyID == 2 ? "es-MX" : "en-US"),
                orderAscending: true
            };
            
            $('#tblZonePrices tbody').html('<tr><td><img src="/images/loading.gif" style="width: 20px; vertical-align: bottom;" /> ' + Language.Transportation_Getting_Prices + '</td></tr>');

            $.getJSON('//eplatfront.villagroup.com/Activities/GetTransportationPrices/', dataObject, function (data) {
                $('#tblZonePrices tbody').html('');
                if (data.Prices.length > 0) {
                    $.each(data.Prices, function (p, price) {
                        var htmlPrice = '<tr data-priceid="' + price.PriceID + '"><td>' + price.Unit + '</td><td><a href="#shopping-cart">$' + price.OfferPrice + ' ' + price.Currency + '</a></td></tr>';
                        $('#tblZonePrices tbody').append(htmlPrice);
                    });
                    $('#tblZonePrices tbody tr').off('click').on('click', function () {
                        BookingEngine.Service.book(dataObject.id, 'Transportation');
                        $('#' + BookingEngine.serviceHotelID).on('loaded', function () {
                            $('#' + BookingEngine.serviceHotelID).val(dataObject.placeId);
                        });
                    });
                } else {
                    $('#tblZonePrices tbody').html('<tr><td>' + Language.Transportation_No_Results + '</td></tr>');
                }
            });
        }

        var init = function () {
            var Cart = BookingEngine.getCart();
            var selectedIndex = -1;

            $.getJSON('/controls/GetResortsByTerminal/' + Cart.TerminalID, null, function (data) {
                BookingEngine.TransportationQuotes.Resorts = data.Resorts;
                $('#txtResort').attr('disabled', null);
            });
            $('#txtResort').on('keyup', function (e) {
                if (e.keyCode == 40) {
                    //abajo
                    selectedIndex += 1;
                    $('#ulResorts>li').removeClass('resort-selected');
                    $('#ulResorts>li').eq(selectedIndex).addClass('resort-selected');
                } else if (e.keyCode == 38) {
                    //arriba
                    selectedIndex -= 1;
                    $('#ulResorts>li').removeClass('resort-selected');
                    $('#ulResorts>li').eq(selectedIndex).addClass('resort-selected');
                } else if (e.keyCode == 13) {
                    //enter
                    if (selectedIndex >= 0) {
                        selectResort(selectedIndex);
                    } else {
                        if ($('#ulResorts>li').length == 1) {
                            selectedIndex = 0;
                            selectResort(selectedIndex);
                        }
                    }
                    
                } else if (e.keyCode == 27) {
                    //escape
                    $('#txtResort').val('').trigger('keyup');
                    $('#tblZonePrices tbody').html('');
                } else {
                    //cualquier otra tecla
                    $('#ulResorts').html('');
                    selectedIndex = -1
                    if ($('#txtResort').val() != "") {
                        $.each(BookingEngine.TransportationQuotes.Resorts, function (r, resort) {
                            if (searchStringFilter(resort.Text).indexOf(searchStringFilter($('#txtResort').val())) >= 0) {
                                $('#ulResorts').append('<li data-value="' + resort.Value + '">' + resort.Text + '</li>');
                            }
                        });
                        //asignación de evento clic a los li para la selección
                        $('#ulResorts>li').off('click').on('click', function () {
                            selectedIndex = $(this).index();
                            selectResort(selectedIndex);
                        });
                    }
                }                
            });            
        }

        return {
            init: init,
            Resorts: Resorts
        }
    }();

    return {
        scheduleContainer: scheduleContainer,
        serviceBooker: serviceBooker,
        serviceDatePicker: serviceDatePicker,
        serviceSchedulePicker: serviceSchedulePicker,
        serviceDetailsPicker: serviceDetailsPicker,
        serviceType: serviceType,
        serviceFlexibleSchedulePicker: serviceFlexibleSchedulePicker,
        serviceAirline: serviceAirline,
        serviceFlight: serviceFlight,
        serviceHotelID: serviceHotelID,
        serviceZoneID: serviceZoneID,
        serviceRoundDiv: serviceRoundDiv,
        serviceRound: serviceRound,
        serviceRoundFields: serviceRoundFields,
        serviceRoundDate: serviceRoundDate,
        serviceRoundAirline: serviceRoundAirline,
        serviceRoundFlightNumber: serviceRoundFlightNumber,
        serviceRoundTime: serviceRoundTime,
        serviceTransportationFields: serviceTransportationFields,
        serviceAddButton: serviceAddButton,
        serviceTotal: serviceTotal,
        serviceSavings: serviceSavings,
        showValidationSummary: showValidationSummary,
        paymentsProviderName: paymentsProviderName,
        paymentsProviderAccount: paymentsProviderAccount,
        paymentsMerchantID: paymentsMerchantID,
        form: form,
        labelTotal: labelTotal,
        labelSavings: labelSavings,
        getCart: getCart,
        getElements: getElements,
        init: init,
        Service: Service,
        Checkout: Checkout,
        applyPromo: applyPromo,
        calculateTotal: calculateTotal,
        deleteCart: deleteCart,
        setCart: setCart,
        TransportationQuotes: TransportationQuotes
    }
}();