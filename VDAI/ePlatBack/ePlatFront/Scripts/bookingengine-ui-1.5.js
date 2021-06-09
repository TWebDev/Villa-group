const BookingEngineUI = {
    datePicker: null,

    ServicesAvailability: [],

    loadCart: function () {
        Cart = BookingEngineModel.init();
        Cart.Culture = UI.BookingEngine.getCulture();
        Cart.CurrencyID = (Cart.Culture == 'es-MX' ? 2 : 1);

        if (window.location.href.toLowerCase().indexOf("/purchase/") >= 0) {
            UI.BookingEngine.loadShoppingCart();
            if (window.location.href.toLowerCase().indexOf("purchase/confirmationpage") >= 0) {
                UI.Controls.triggerConversion(1, Cart.PointOfSaleID);
                UI.BookingEngine.deleteCart();
            }
        } else {
            UI.BookingEngine.refreshCartTotals();
        }

        //eventos
        //inicializar datepicker
        UI.BookingEngine.datePicker = $('.datepicker').pickadate({
            selectMonths: true,
            selectYears: 5,
            today: 'Today',
            clear: 'Clear',
            close: 'Ok',
            closeOnSelect: false, // Close upon selecting a date,
            format: 'yyyy-mm-dd',
            autoclose: true,
            onRender: function () {
                //console.log('onRender UI.BookingEngine.BookingForm.getDisabledDays()')
                UI.BookingEngine.BookingForm.getDisabledDays();
            },
            onOpen: function () {
                //console.log('onOpen UI.BookingEngine.BookingForm.getDisabledDays()')
                UI.BookingEngine.BookingForm.getDisabledDays();
            }
        });
        //Time Picker:
        $('.timepicker').pickatime({
            default: 'now',
            twelvehour: false, // change to 12 hour AM/PM clock from 24 hour
            donetext: 'OK',
            autoclose: false,
            vibrate: true // vibrate the device when dragging clock hand
        });
        $('#ActivityDate').on('change', function () {
            if (Cart.Session.ServiceType == 1) {
                UI.BookingEngine.BookingForm.getSchedulesAndPrices();
            } else {
                if (($('#TransportationHotelID').val() > 0 || $('#TransportationZoneID').val() > 0) && $('#ActivityDate').val() != '') {
                    UI.Controls.blockForm('divBookingEngineForm', 'Getting Prices...');
                    Cart.getTransportationPrices(
                        $('#ActivityDate').val(),
                        $('#TransportationHotelID').val(),
                        $('#TransportationZoneID').val(),
                        $('input:radio[name="Round"]:checked').val(),
                        UI.BookingEngine.BookingForm.setPricesUI
                    );
                }
            }
        });
        //evento agregar al carrito
        $('#btnAddActivity').on('click', function () {
            UI.BookingEngine.add();
        });
        //formato de campos
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

        //controls
        if ($('#divSubmitReview').length > 0) {
            UI.Controls.Reviews.init();
        }
        //Transportation Quotes
        $('body').off('CartLoaded').on('CartLoaded', function () {
            if (window.location.href.indexOf("/transportation") >= 0 || window.location.href.indexOf("/detailv2/12154") >= 0) {
                UI.BookingEngine.TransportationQuotes.init();
            }
        });        
    },

    getCulture: function () {
        let lan = '';
        if (window.location.href.indexOf("mx.") >= 0) {
            lan = 'es-MX';
        } else if (window.location.href.indexOf("localhost") >= 0) {
            lan = 'es-MX';
        } else {
            lan = 'en-US';
        }
        return lan;
    },

    book: function (serviceID, service) {
        UI.BookingEngine.BookingForm.clearBookingForm();
        //obtiene la información necesaria para el formulario
        Cart.Session.ServiceID = serviceID;
        Cart.Session.Service = service;
        Cart.Session.Process = 'adding';
        Cart.getServiceType(function (data) {
            UI.BookingEngine.BookingForm.setFormType(data);
        });

        //abre el formulario para reservar
        $('#divBookingEngineForm').modal('open');
        $('.booking').html(Language.Booking + ' ' + service);
    },

    add: function () {
        //verificar que se pueda agregar
        var valid = true;
        var msg = Language.Check_Issues;
        if ($("#ActivityDate").val() == "" || $("#ActivityDate").val() == "yyyy-mm-dd") {
            valid = false;
            msg += '\n- ' + Language.Field_Date;
        } else {
            if (Cart.Session.ServiceType == 1) {
                //activity
                if ($('#ActivitySchedule').val() == "" || $('#ActivitySchedule').val() == "0") {
                    valid = false;
                    msg += '\n- ' + Language.Field_Schedule;
                }
            } else {
                //transportation
                //console.log('validation');
                if ($('#ActivityFlexibleSchedule').val() == "" || $('#ActivityFlexibleSchedule').val() == "0") {
                    valid = false;
                    msg += '\n- ' + Language.Field_Schedule;
                }
                if ($('#TransportationHotelID').val() == '') {
                    valid = false;
                    msg += '\n- ' + Language.Field_Hotel;
                } else if ($('#TransportationHotelID').val() == '-1') {
                    if ($('#TransportationZoneID').val() == '0') {
                        valid = false;
                        msg += '\n- ' + Language.Field_Zone;
                    }
                }
                if ($('#TransportationAirline').val() == '') {
                    valid = false;
                    msg += '\n- ' + Language.Field_Airline;
                }
                if ($('#TransportationFlight').val() == '') {
                    valid = false;
                    msg += '\n- ' + Language.Field_FlightNumber;
                }
                if ($('input:radio[name="Round"]:checked').val() == "true") {
                    if ($('#RoundDate').val() == '') {
                        valid = false;
                        msg += '\n- ' + Language.Field_RoundDate;
                    }
                    if ($('#RoundAirline').val() == '') {
                        valid = false;
                        msg += '\n- ' + Language.Field_RoundAirline;
                    }
                    if ($('#RoundFlightNumber').val() == '') {
                        valid = false;
                        msg += '\n- ' + Language.Field_RoundFlightNumber;
                    }
                    if ($('#RoundMeetingTime').val() == '') {
                        valid = false;
                        msg += '\n- ' + Language.Field_RoundTime;
                    }
                }
            }
            if ($('#ActivityTotal').text() == "$0" || $('#ActivityTotal').text() == "") {
                valid = false;
                msg += '\n- ' + Language.Field_Details;
            }
        }
        let activityTotal = $('#ActivityTotal').text().replace('$', '').replace(' USD', '').replace(' MXN', '');
        if (isNaN(activityTotal) || (!isNaN(activityTotal) && parseFloat(activityTotal) <= 0)) {
            valid = false;
            msg += '\n- ' + Language.Total_More_Than_Zero;
        }

        //verificar que no haya ya el mismo tour para la misma fecha

        if (valid) {
            //get item
            var item = {
                ServiceID: Cart.Session.ServiceID,
                Service: Cart.Session.Service,
                ServiceType: Cart.Session.ServiceType,
                ServiceDate: $("#ActivityDate").val(),
                WeeklyAvailabilityID: $('#ActivitySchedule').val(),
                Schedule: (Cart.Session.ServiceType == 1 ? $('#ActivitySchedule option:selected').text() : $('#ActivityFlexibleSchedule').val()),
                Airline: $('#TransportationAirline').val(),
                Flight: $('#TransportationFlight').val(),
                HotelID: $('#TransportationHotelID').val(),
                TransportationZoneID: $('#TransportationZoneID').val(),
                Round: $('input:radio[name="Round"]:checked').val(),
                RoundDate: $('#RoundDate').val(),
                RoundAirline: $('#RoundAirline').val(),
                RoundFlightNumber: $('#RoundFlightNumber').val(),
                RoundMeetingTime: $('#RoundMeetingTime').val(),
                Total: parseFloat($('#ActivityTotal').text().replace('$', '')),
                PromoTotal: parseFloat($('#ActivityTotal').text().replace('$', '')),
                Savings: parseFloat($('#ActivitySavings').val()),
                PromoSavings: parseFloat($('#ActivitySavings').val()),
                PromoID: null,
                Details: []
            }
            //get details
            $('#ActivityDetails .trbody').each(function (i, row) {
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
            if (Cart.Session.Process == "adding") {
                item.CartItemID = new Date().getTime();
                Cart.Items.push(item);
            } else {
                $.each(Cart.Items, function (i, cartitem) {
                    if (cartitem.CartItemID == Cart.Session.CartItemID) {
                        item.CartItemID = Cart.Session.CartItemID;
                        item.DateSaved = Cart.Items[i].DateSaved;
                        Cart.Items[i] = item;
                        return false;
                    }
                });
            }

            Cart.PromoID = null;
            Cart.PromoCode = null;
            $('#divPromoCodeApplied').hide();
            $('#divPromoCodeAdd').show();
            $('.cart-subtotal').show();
            $('#btnOpenPromocodePanel').prop('checked', false);
            Cart.calculateTotal();
            Cart.save();

            $('#divBookingEngineForm').modal('close');

            //buscar promos y actualizar el cart
            Cart.applyPromo('', function (total) {
                if (Cart.Session.Process == "editing") {
                    UI.BookingEngine.loadShoppingCartItems();
                }
                UI.BookingEngine.refreshCartTotals();
            });
        } else {
            alert(msg);
        }
    },

    refreshCartTotals: function () {
        //contador del botón de shopping cart
        if ($('.btnShoppingCart .notification-circle').length > 0) {
            $('.btnShoppingCart .notification-circle').fadeOut('fast', function () {
                $('.btnShoppingCart .notification-circle').remove();
                if (Cart.Items != undefined && Cart.Items.filter(x => !x.Deleted).length > 0) {
                    $('.btnShoppingCart').append('<span class="notification-circle">' + Cart.Items.filter(x => !x.Deleted).length + '</span>');
                    $('.btnShoppingCart .notification-circle').fadeIn('fast');
                }
            });
        } else {
            if (Cart.Items.length > 0) {
                $('.btnShoppingCart').append('<span class="notification-circle">' + Cart.Items.filter(x => !x.Deleted).length + '</span>');
                $('.btnShoppingCart .notification-circle').fadeIn('fast');
            }
        }

        //labels totales
        if (Cart.Items != undefined) {
            $('#CartSubTotal').text('$' + Cart.ItemsTotal.toFixed(2) + (Cart.CurrencyID == 1 ? ' USD' : ' MXN'));
            $('#CartTotal').text('$' + Cart.Total.toFixed(2) + (Cart.CurrencyID == 1 ? ' USD' : ' MXN'));

            if (Cart.Total <= 0) {
                $('#btnCheckOut').attr('disabled', 'disabled');
            } else {
                $('#btnCheckOut').attr('disabled', null);
            }
        } else {
            $('#CartSubTotal').text('$0.00');
            $('#CartTotal').text('$0.00');
        }
    },

    edit: function (cartItemID) {
        UI.BookingEngine.BookingForm.clearBookingForm();
        //obtiene la información necesaria para el formulario
        let item = Cart.getItem(cartItemID);
        Cart.Session.CartItemID = cartItemID;
        Cart.Session.ServiceID = item.ServiceID;
        Cart.Session.Service = item.Service;
        Cart.Session.Process = 'editing';
        Cart.Session.ServiceType = item.ServiceType;
        Cart.getServiceType(function (data) {
            UI.BookingEngine.BookingForm.setFormType(data);
        });
        $('.booking').html(Language.Editing + ' ' + item.Service);
        //asignar fecha
        $('#ActivityDate').val(item.ServiceDate).trigger('change');
        if (Cart.Session.ServiceType == 1) {
            //actividad
            $('#ActivitySchedule').off('loaded').on('loaded', function () {
                //asignar horario
                $("#ActivitySchedule").find("option").filter(function (index) {
                    return item.Schedule === $(this).text();
                }).prop("selected", "selected");
                $('#ActivitySchedule').material_select();
            });
        } else {
            //transportacion
            $('#ActivityFlexibleSchedule').val(item.Schedule);
            $('#TransportationHotelID').off('loaded').on('loaded', function () {
                $('#TransportationHotelID').val(item.HotelID);
                $('#TransportationHotelID').material_select();
                if (item.HotelID == '-1') {
                    $('#divTransportationHotel').slideUp('fast');
                    $('#divTransportationZone').slideUp('fast');
                    $('#TransportationZoneID').val(item.TransportationZoneID);
                    $('#TransportationZoneID').material_select();
                }
                $('#TransportationHotelID').trigger('change');
            });
            $('#TransportationAirline').val(item.Airline);
            $('#TransportationFlight').val(item.Flight);
            $('input[name="Round"][value="' + item.Round + '"]').prop('checked', true);
            if (item.Round == true) {
                $('.round-trip').show();
                $('#RoundDate').val(item.RoundDate);
                $('#RoundAirline').val(item.RoundAirline);
                $('#RoundFlightNumber').val(item.RoundFlightNumber);
                $('#RoundMeetingTime').val(item.RoundMeetingTime);
            } else {
                $('.round-trip').hide();
            }
            Materialize.updateTextFields();
        }
        $('#ActivityDetails').off('loaded').on('loaded', function () {
            //asignar detalles
            $.each(item.Details, function (j, detail) {
                $('#' + detail.PriceID).find('input[type=text]').val(detail.Quantity).trigger('keyup');
            });
        });

        //abre el formulario para reservar
        $('#divBookingEngineForm').modal('open');
    },

    showPromoCodeDiscount: function (totalDiscount) {
        if (totalDiscount > 0) {
            $('#divPromoCode').slideUp('fast');
            $('#divPromoCodeApplied').slideDown('fast');
            $('#PromoCodeTotal').text('$-' + totalDiscount.toFixed(2) + (Cart.CurrencyID == 1 ? ' USD' : ' MXN'));
            $('#CartTotal').text('$' + Cart.Total.toFixed(2) + (Cart.CurrencyID == 1 ? ' USD' : ' MXN'));
            $('#divPromoCodeAdd').slideUp('fast');
        } else {
            $('#divPromoCode').slideUp('fast');
            alert('Activities in Shopping Cart don\'t apply for this Promotional Code.');
        }
    },

    loadShoppingCartItems: function () {
        $('#CartItems').html('');
        if (Cart.Items.length > 0) {

            $.each(Cart.Items, function (i, item) {
                if (item.Deleted != true) {
                    var details = '';
                    $.each(item.Details, function (j, detail) {
                        if (j > 0) {
                            if (j == item.Details.length - 1) {
                                details += ' ' + Language.And + ' ';
                            } else {
                                details += ', ';
                            }
                        }
                        details += detail.Quantity + ' ' + detail.Unit;
                    });
                    let li = '<li class="collection-item avatar" data-cartitemid="' + item.CartItemID + '">';
                    li += '<img class="responsive-img circle" src="//eplatfront.villagroup.com' + UI.BookingEngine.getPicture(item.ServiceID) + '?w=240&h=240&mode=crop" />';
                    let url = UI.BookingEngine.getUrl(item.ServiceID);
                    if (url != '') {
                        li += '<a href="' + url + '">';
                    }
                    li += '<span class="title">' + item.Service + '</span>';
                    if (url != '') {
                        li += '</a>';
                    }
                    li += '<span class="total ' + (item.PromoID != null ? 'promo-total' : '') + '">$' + item.PromoTotal.toFixed(2) + (Cart.CurrencyID == 1 ? ' USD' : ' MXN') + '</span>';

                    li += '<span class="schedule">' + item.ServiceDate + ' ' + Language.At + ' ' + item.Schedule + '</span>';

                    li += '<span class="details">' + details + '</span>';
                    li += (item.PromoID != null ? '<br /><span class="promo-applied">' + Language.Promo_Applied + '</span>' : '');

                    li += '<span class="secondary-content">';
                    li += '<a onclick="UI.BookingEngine.edit(' + item.CartItemID + ')"><i class="material-icons light-green-text text-darken-3">edit</i></a>';
                    li += '<a onclick="UI.BookingEngine.deleteItem(' + item.CartItemID + ')"><i class="material-icons light-green-text text-darken-3">delete</i></a>';
                    li += '</span>';

                    li += '</li>';
                    $('#CartItems').append(li);
                }
            });
        } else {
            $('#CartItems').append('<h3 class="center-align">' + Language.No_Activities  + '</h3>');
        }
    },

    loadShoppingCart: function () {
        //totals
        UI.BookingEngine.refreshCartTotals();

        //promocode
        $('#btnOpenPromocodePanel').off('click').on('click', function () {
            $('#divPromoCode').slideToggle('fast');
        });
        $('#btnApplyPromoCode').off('click').on('click', function () {
            if ($('#PromoCode').val().trim() != "") {
                Cart.applyPromo($('#PromoCode').val().trim(), UI.BookingEngine.showPromoCodeDiscount);
            } else {
                alert('Promotional code field is empty.');
            }
        });

        $('#PromoCode').off('keyup').on('keyup', function (e) {
            if (e.which == 13 && $('#PromoCode').val().trim() != "") {
                Cart.applyPromo($('#PromoCode').val().trim(), UI.BookingEngine.showPromoCodeDiscount);
            }
        });
        if (Cart.PromoID != null) {
            $('#divPromoCodeAdd').hide();
            //$('#PromoCodeTotal').text('$' + (Cart.Total - Cart.ItemsTotal).toFixed(2) + (Cart.CurrencyID == 1 ? ' USD' : ' MXN'));
            $('.cart-subtotal').hide();
            $('#divPromoCodeApplied .label').text('Applied Promo Code');

            $('#PromoCodeTotal').text(Cart.PromoCode);
            $('#divPromoCodeApplied').show();
        }

        //items
        UI.BookingEngine.loadShoppingCartItems();

        //checkout events
        $('#StayingAtPlaceID').off('change').on('change', function () {
            if ($('#StayingAtPlaceID').val() == '0') {
                $('#divStayingAt').slideDown('fast');
            } else {
                $('#divStayingAt').slideUp('fast');
            }
        });

        $('#btnClearForm').off('click').on('click', function () {
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

            $('#CCHolderName').val('');
            $('#CCType').val('');
            $('#CCNumber').val('');
            $('#CCExpirationMonth').val('');
            $('#CCExpirationYear').val('');
            $('#CCSecurityNumber').val('');

            $('#Comments').val('');

            $('select').material_select();
        });

        if ($('#ItemsJSON').length > 0) {
            UI.BookingEngine.CheckOut.fillCartForm();
            var PurchaseInfoLocal = Storage.get("PurchaseInfo");
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
        }
    },

    deleteCart: function () {
        var cart = { CartID: "empty" };
        Cart = cart;
        Storage.save("BookingEngine.Cart", cart, 90);
    },

    getPicture: function (id) {
        let picture = '';
        $.each(UI.Search.Index.Activities, function (s, service) {
            if (service.ActivityID == id) {
                picture = service.Picture;
                return false;
            }
        });
        return picture;
    },

    getUrl: function (id) {
        let url = '';
        $.each(UI.Search.Index.Activities, function (s, service) {
            if (service.ActivityID == id) {
                url = service.Url;
                return false;
            }
        });
        return url;
    },

    deleteItem: function (id) {
        $('li[data-cartitemid="' + id + '"]').slideUp('fast', function () {
            Cart.deleteItem(id);
            /*Cart.calculateTotal();
            //buscar promos y actualizar el cart
            Cart.applyPromo('', function () {
                UI.BookingEngine.loadShoppingCartItems();
                UI.BookingEngine.refreshCartTotals();
            });
            //guardar
            Cart.save();
            UI.BookingEngine.loadShoppingCartItems();
            //refrescar totales
            UI.BookingEngine.refreshCartTotals();
            */

            Cart.PromoID = null;
            Cart.PromoCode = null;
            $('#divPromoCodeApplied').hide();
            $('#divPromoCodeAdd').show();
            $('.cart-subtotal').show();
            $('#btnOpenPromocodePanel').prop('checked', false);
            Cart.calculateTotal();
            Cart.save();

            $('#divBookingEngineForm').modal('close');

            //buscar promos y actualizar el cart
            Cart.applyPromo('', function (total) {
                UI.BookingEngine.loadShoppingCartItems();
                UI.BookingEngine.refreshCartTotals();
            });
        });
    },

    BookingForm: {
        setFormType: function (data) {
            Cart.Session.ServiceType = data.ServiceType;
            if (data.ServiceType == 1) {
                $('.fixed-time').show();
                $('.flexible-time').hide();
                $('.transportation').hide();
            } else {
                $('#TransportationHotelID')
                    .fillSelect(data.Hotels)
                    .off('change')
                    .on('change', function () {
                        if ($('#TransportationHotelID').val() != 0) {
                            if ($('#TransportationHotelID').val() == '-1') {
                                $('#divTransportationHotel').slideUp('fast');
                                $('#divTransportationZone').slideDown('fast');
                            } else {
                                if ($('#ActivityDate').val() != '') {
                                    UI.Controls.blockForm('divBookingEngineForm', 'Getting Prices...');
                                    Cart.getTransportationPrices(
                                        $('#ActivityDate').val(),
                                        $('#TransportationHotelID').val(),
                                        $('#TransportationZoneID').val(),
                                        $('input:radio[name="Round"]:checked').val(),
                                        UI.BookingEngine.BookingForm.setPricesUI
                                    );
                                }
                            }
                        }
                    });
                $('#TransportationHotelID').material_select();

                $('#TransportationZoneID')
                    .fillSelect(data.Zones)
                    .off('change')
                    .on('change', function () {
                        if ($('#TransportationZoneID').val() != "0") {
                            UI.Controls.blockForm('divBookingEngineForm', 'Getting Prices...');
                            Cart.getTransportationPrices(
                                $('#ActivityDate').val(),
                                $('#TransportationHotelID').val(),
                                $('#TransportationZoneID').val(),
                                $('input:radio[name="Round"]:checked').val(),
                                UI.BookingEngine.BookingForm.setPricesUI
                            );
                        }
                    });
                $('#TransportationZoneID').material_select();

                $('#TransportationHotelID').trigger('loaded');
                $('.fixed-time').hide();
                $('.flexible-time').show();
                if (data.OffersRoundTrip) {
                    $('#divOffersRoundTrip').show();
                    $('input:radio[name="Round"]')
                        .off('click')
                        .on('click', function () {
                            if ($('input:radio[name="Round"]:checked').val() == 'true') {
                                $('.round-trip').show();
                            } else {
                                $('.round-trip').hide();
                            }
                            if ($('#TransportationHotelID').val() > 0) {
                                $('#TransportationHotelID').trigger('change');
                            }
                            if ($('#TransportationZoneID').val() != "0") {
                                $('#TransportationZoneID').trigger('change');
                            }
                        });
                }
                else {
                    $('#divOffersRoundTrip').hide();
                }
                $('.transportation').show();
            }
        },

        setPricesUI: function (data) {
            $('#ActivityDetails tbody').html('');
            UI.Controls.unblockForm('divBookingEngineForm');

            $.each(data.Prices, function (i, price) {
                let unit = '';
                unit = '<tr class="trbody" id="' + price.PriceID + '" ' + (price.DependingOnPriceID != null ? 'data-dependentid="' + price.DependingOnPriceID + '" data-dependentqty="' + price.DependingOnPriceQuantity + '" style="display:none"' : '') + '>';
                unit += '<td class="td-qty"><input type="text" class="browser-default" style="width:30px;" /></td>';
                unit += '<td class="td-unit">' + price.Unit + (price.UnitMin != null ? ' (' + price.UnitMin + ' - ' : '') + (price.UnitMax != null ? price.UnitMax + ' ' + (Cart.Session.ServiceType == '1' ? 'years' : 'pax') + ')' : '') + '</td>';
                unit += '<td class="td-price">$<span class="td-price-offer">' + price.OfferPrice.toFixed(2) + '</span><span class="td-price-retail left" ' + (parseInt(price.RetailPrice) == 0 ? 'style="display:none;"' : '') + '>$' + price.RetailPrice.toFixed(2) + '</span><input class="td-price-savings" type="hidden" value="' + (parseFloat(price.RetailPrice) - parseFloat(price.OfferPrice)) + '" /><input type="hidden" class="price-type" value="' + price.PriceTypeID + '" /><input type="hidden" class="price-exchange" value="' + price.ExchangeRateID + '" /></td>';
                unit += '<td class="td-totals">$<span class="td-totals-amount">0</span><input type="hidden" class="td-totals-savings" value="0" /></td>';
                unit += '</tr>';
                $('#ActivityDetails tbody').append(unit);
            });
            $('#ActivityTotal').text('$0.00' + (Cart.CurrencyID == 1 ? ' USD' : ' MXN'));
            $('#ActivityDetails tbody input[type=text]').on('keyup', function () {
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
                $('#ActivityTotal').text('$' + total.toFixed(2) + (Cart.CurrencyID == 1 ? ' USD' : ' MXN'));

                //calcular total de ahorros
                var savings = 0;
                $('.td-totals-savings').each(function (i) {
                    savings += (parseFloat($(this).val()) > 0 ? parseFloat($(this).val()) : 0);
                });
                $('#ActivitySavings').val(savings.toFixed(2));

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
            });
            $('#ActivityDetails').trigger('loaded');
        },

        getDisabledDays: function () {
            //console.log('BookingForm.getDisabledDays()');
            if (Cart.Session != undefined && Cart.Session.ServiceID != 0 && (Cart.Session.CurrentMonth != parseInt($('.picker__select--month').val()) + 1 || Cart.Session.CurrentYear != $('.picker__select--year').val() || Cart.Session.ServiceID != Cart.Session.CalendarServiceID)) {
                Cart.Session.CurrentYear = $('.picker__select--year').val();
                Cart.Session.CurrentMonth = parseInt($('.picker__select--month').val()) + 1;

                let disabledDays = '';
                $.each(UI.BookingEngine.ServicesAvailability, function () {
                    if (this.ServiceID == Cart.Session.ServiceID && this.Month == Cart.Session.CurrentMonth && this.Year == Cart.Session.CurrentYear) {
                        Cart.Session.CalendarServiceID = Cart.Session.ServiceID;
                        disabledDays = this.DisabledDays;
                    }
                });
                let picker = UI.BookingEngine.datePicker.pickadate('picker');
                if (disabledDays != '') {
                    picker.set('disable', eval(disabledDays));
                } else {
                    Cart.getDisabledDays(function (data) {
                        Cart.Session.CalendarServiceID = Cart.Session.ServiceID;
                        let serviceAvailability = {
                            ServiceID: Cart.Session.ServiceID,
                            Month: Cart.Session.CurrentMonth,
                            Year: Cart.Session.CurrentYear,
                            DisabledDays: data
                        };
                        UI.BookingEngine.ServicesAvailability.push(serviceAvailability);
                        picker.set('disable', eval(data));
                    });
                }
            }
        },

        getSchedulesAndPrices: function () {
            UI.Controls.blockForm('divBookingEngineForm', Language.Getting_Availability + '...');
            Cart.getSchedulesAndPrices($('#ActivityDate').val(), function (data) {
                $('#ActivitySchedule').fillSelect(data.Schedules);
                $('#ActivitySchedule').material_select();
                $('#ActivitySchedule').trigger('loaded');
                if ($('#ActivitySchedule option').length == 2) {
                    $('#ActivitySchedule option').eq(1).prop('selected', true);
                }
                UI.BookingEngine.BookingForm.setPricesUI(data);
            });
        },

        clearBookingForm: function () {
            $('#ActivityDate').val('');

            $('#ActivitySchedule').clearSelect();
            $('#ActivitySchedule').material_select();
            $('#ActivityDetails tbody').html('');
            $('#ActivityTotal').html('');

            $('#ActivityFlexibleSchedule').val('');
            $('#TransportationHotelID').val(0);
            $('#TransportationZoneID').val(0);
            $('#divTransportationHotel').show();
            $('#divTransportationZone').hide();

            $('#TransportationAirline').val('');
            $('#TransportationFlight').val('');

            $('.round-trip').hide();
            $('#RoundDate').val('');
            $('#RoundAirline').val('');
            $('#RoundFlightNumber').val('');
            $('#RoundMeetingTime').val('');
        }
    },

    CheckOut: {
        fillCartForm: function () {
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
            $('#StayingAtPlaceID').material_select();
            if (Cart.CurrencyID == 1) {
                $('.payment-info').show();
            } else {
                $('.payment-info').hide();
            }
        },

        validateForm: function () {
            if (!$("#CheckOutForm").valid()) {
                $('#divPurchaseValidation').slideDown('fast');
                return false;
            } else {
                $('#divPurchaseValidation').slideUp('fast');
                return true;
            }
        },

        onBegin: function () {
            UI.Controls.blockForm('divCartTotals', Language.Processing + '...');
        },

        onSuccess: function (data) {
            if (data.ResponseType == 1) {
                if (Cart.CurrencyID == 1) {
                    window.location.href = "/Purchase/ConfirmationPage/" + data.PurchaseID;
                } else {
                    Cart.CartID = data.PurchaseID;
                    Cart.Items = data.Items;
                    Cart.save();
                    $('#PurchaseID').val(data.PurchaseID);
                    $('#ItemsJSON').val($.toJSON(Cart.Items));

                    var configuration = {
                        //merchant: 'TEST6264329',
                        merchant: '6264329',
                        order: {
                            amount: Cart.Total,
                            currency: 'MXN', /*código alfa ISO 4217*/
                            description: function () {
                                let items = '';
                                for (var x = 0; x < Cart.Items.length; x++) {
                                    if (!Cart.Items[x].Deleted) {
                                        items += (items != '' ? ', ' : '') + Cart.Items[x].Service;
                                    }
                                }
                                return items;
                            },
                            id: Cart.CartID
                        },
                        billing: {
                            address: {
                                street: $('#Address').val(),
                                city: $('#City').val(),
                                postcodeZip: $('#ZipCode').val(),
                                stateProvince: $('#State').val(),
                                country: function () {
                                    /*ISO_3166-1_alpha-3*/
                                    let countryCode = '';
                                    switch ($('#Country option:selected').text()) {
                                        case 'Mexico':
                                            countryCode = 'MEX';
                                            break;
                                        case 'United States':
                                            countryCode = 'USA';
                                            break;
                                        case 'Canada':
                                            countryCode = 'CAN';
                                            break;
                                    }
                                    return countryCode;
                                }
                            }
                        },
                        customer: {
                            firstName: $('#FirstName').val(),
                            lastName: $('#LastName').val(),
                            phone: '+52 ' + $('#Phone').val(),
                            mobilePhone: '+52 ' + $('#Mobile').val(),
                            email: $('#Email').val()
                        },
                        interaction: {
                            merchant: {
                                name: 'My Experience Tours',
                                address: {
                                    line1: 'Blvd. Francisco Medina Ascencio Km. 3.5',
                                    line2: 'Puerto Vallarta, Jalisco, Mexico'
                                },
                                phone: '01 800 638 7774',
                                email: 'info@' + window.location.hostname.replace('www.', '')
                            },
                            locale: 'es_MX',
                            theme: 'default',
                            displayControl: {
                                billingAddress: 'OPTIONAL',
                                customerEmail: 'HIDE',
                                orderSummary: 'SHOW',
                                shipping: 'HIDE'
                            }
                        },
                        session: { id: data.SessionID }
                    };
                    //console.log(configuration);
                    Checkout.configure(configuration);

                    Checkout.showPaymentPage();
                }
            } else {
                UI.Controls.unblockForm('divCartTotals');
                Cart.CartID = data.PurchaseID;
                Cart.Items = data.Items;
                Cart.save();
                $('#PurchaseID').val(data.PurchaseID);
                $('#ItemsJSON').val($.toJSON(Cart.Items));

                UI.BookingEngine.loadShoppingCartItems();

                $('#divCartTotals .interaction-message').html(data.ResponseMessage).removeClass('confirmation').addClass('error');
                $('#divCartTotals .status-bar').slideDown('fast');
            }
        },

        onFailure: function (data) {
            $('#divCartTotals .interaction-message').html("Error trying to save your purchase, please try again.").removeClass('confirmation').addClass('error');
            $('#divCartTotals .status-bar').slideDown('fast');
        }
    },

    TransportationQuotes: {
        init: function () {
            Cart.Session.ServiceID = $('#TransportationServiceID').val();
            Cart.Session.Service = 'Transportation';
            Cart.Session.Process = 'adding';

            let date = new Date();
            let yyyy = date.getFullYear().toString();
            let mm = (date.getMonth() + 1).toString();
            let dd = date.getDate().toString();
            let cdate = yyyy + '-' + (mm[1] ? mm : "0" + mm[0]) + '-' + (dd[1] ? dd : "0" + dd[0]);

            Cart.getResortsByTerminal(function (data) {
                var Resorts = {};
                $.each(data.Resorts, function (r, resort) {
                    Resorts[resort.Text] = null;
                });

                $('.chips-autocomplete').material_chip({
                    autocompleteOptions: {
                        data: Resorts,
                        limit: Infinity,
                        minLength: 1
                    }
                });

                $('.chips').on('chip.add', function (e, chip) {
                    //console.log('chip add');
                    $('.chip').not(':last').remove();
                    let resort = chip.tag;
                    $.each(data.Resorts, function (i, item) {
                        if (item.Text == resort) {
                            let resortid = item.Value;
                            $('#tblZonePrices').prepend('<div id="imgLoading" style="text-align:center;"><img src="/images/loading.gif" /></div>');
                            Cart.getTransportationPrices(cdate, resortid, 0, false, function (data) {
                                $('#imgLoading').remove();
                                $('#tblZonePrices tbody').html('');
                                if (data.Prices.length > 0) {
                                    $.each(data.Prices, function (p, price) {
                                        var htmlPrice = '<tr data-priceid="' + price.PriceID + '"><td>' + price.Unit + '</td><td><a href="#shopping-cart">$' + price.OfferPrice + ' ' + price.Currency + '</a></td></tr>';
                                        $('#tblZonePrices tbody').append(htmlPrice);
                                    });
                                    $('#tblZonePrices tbody tr').off('click').on('click', function () {
                                        UI.BookingEngine.book(Cart.Session.ServiceID, Cart.Session.Service);
                                        $('#TransportationHotelID').on('loaded', function () {
                                            $('#TransportationHotelID').val(resortid);
                                            $('#TransportationHotelID').material_select();
                                        });
                                        UI.BookingEngine.BookingForm.setPricesUI(data);
                                    });
                                } else {
                                    $('#tblZonePrices tbody').html('<tr><td>' + Language.Transportation_No_Results + '</td></tr>');
                                }
                            });

                            return false;
                        }
                    });
                })
            });
        }
    }
}

const SearchUI = {
    Index: [],
    init: function () {
        //cargar la lista de información
        UI.Search.Index = Storage.get("SearchIndex");

        if (UI.Search.Index == undefined) {
            UI.Search.getIndex();
        } else {
            //si la fecha es 2 dias menor, pedir el indice
            var parts = UI.Search.Index.Date.split('-');
            var theSavedDate = new Date(parts[0], parts[1] - 1, parts[2]);
            var yesterday = new Date();
            yesterday.setDate(yesterday.getDate() - 1);
            if (theSavedDate < yesterday) {
                UI.Search.getIndex();
            } else {
                $('#divSearchBox').slideDown('fast');
            }
        }

        //evento de búsqueda
        $('#txtSearch').on('keyup', function () {
            UI.Search.search($('#txtSearch').val());
        });
        $('#btnSearch').on('click', function () {
            UI.Search.search($('#txtSearch').val());
        });
        $('#btnCloseSearch').on('click', function () {
            UI.Search.close();
        });
    },

    getIndex: function () {
        $.getJSON('/Activities/GetIndex', null, function (data) {
            Storage.save("SearchIndex", data, 8);
            UI.Search.Index = data;
            $('#divSearchBox').slideDown('fast');
        });
    },

    close: function () {
        $('#searchQuickResults .container').html('');
        $('#searchQuickResults').slideUp('fast');
    },

    preg_quote: function (str) {
        return (str + '').replace(/([\\\.\+\*\?\[\^\]\$\(\)\{\}\=\!\<\>\|\:])/g, "\\$1");
    },

    highlight: function (data, search) {
        return data.replace(new RegExp("(" + UI.Search.preg_quote(search) + ")", 'gi'), "<b>$1</b>");
    },

    search: function (keyword) {
        if (keyword != "") {
            let results = '';
            let namesArr = [];
            let contentArr = [];
            let pointsArr = [];
            let providerArr = [];
            let namesStr = '';
            let contentStr = '';
            let pointsStr = '';
            let providerStr = '';
            let keywordSplit = keyword.trim().split(' ');
            let rating = 0;
            let content = '';
            let indexOfContent = 0;
            $.each(UI.Search.Index.Activities, function (i, item) {
                //check names
                rating = 0;
                content = item.Name;
                for (var x = 0; x < keywordSplit.length; x++) {
                    if (content.toLowerCase().indexOf(keywordSplit[x].toLowerCase()) >= 0) {
                        content = UI.Search.highlight(content, keywordSplit[x]);
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
                            content = UI.Search.highlight(content, keywordSplit[x]);
                            indexOfContent = content.toLowerCase().indexOf(keywordSplit[x].toLowerCase());
                            rating++;
                        }
                    }
                    //cortar contenido para mostrar la keyword

                    if (rating > 0) {
                        contentArr.push({ Rating: rating, Content: (item.Picture != "" ? '<img src="//eplatfront.villagroup.com' + item.Picture + '?width=30&height=30&mode=crop" />' : '<span class="img-replacement"></span>') + '<a href="' + item.Url + '">' + item.Name + '</a><br />...' + content.substr((indexOfContent > 30 ? indexOfContent - 30 : 0), 60) + '...' });
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
                            content = UI.Search.highlight(content, keywordSplit[x]);
                            indexOfContent = content.toLowerCase().indexOf(keywordSplit[x].toLowerCase());
                            rating++;
                        }
                    }

                    if (rating > 0) {
                        pointsArr.push({ Rating: rating, Content: (item.Picture != "" ? '<img src="//eplatfront.villagroup.com' + item.Picture + '?width=30&height=30&mode=crop" />' : '<span class="img-replacement"></span>') + '<a href="' + item.Url + '">' + item.Name + '</a><br />...' + content.substr((indexOfContent > 30 ? indexOfContent - 30 : 0), 60) + '...' });
                    }
                }

                //check providers
                if (item.Provider != null) {
                    rating = 0;
                    content = item.Provider;
                    if (content.toLowerCase().indexOf(keyword.toLowerCase()) >= 0) {
                        content = UI.Search.highlight(content, keywordSplit[x]);
                        rating++;
                    }

                    if (rating > 0) {
                        providerArr.push({ Rating: rating, Content: (item.Picture != "" ? '<img src="//eplatfront.villagroup.com' + item.Picture + '?width=30&height=30&mode=crop" />' : '<span class="img-replacement"></span>') + '<a href="' + item.Url + '">' + item.Name + '</a><br />Provided by ' + content });
                    }
                }
            });

            // order results
            namesArr = namesArr.sort(UI.Search.custom_sort);
            $.each(namesArr.reverse(), function (n, name) {
                namesStr += '<li>' + name.Content + '</li>';
            });

            contentArr = contentArr.sort(UI.Search.custom_sort);
            $.each(contentArr.reverse(), function (c, item) {
                contentStr += '<li>' + item.Content + '</li>';
            });

            pointsArr = pointsArr.sort(UI.Search.custom_sort);
            $.each(pointsArr.reverse(), function (c, item) {
                pointsStr += '<li>' + item.Content + '</li>';
            });
            $.each(providerArr.reverse(), function (c, item) {
                providerStr += '<li>' + item.Content + '</li>';
            });

            results = '<ul>';
            if (namesStr != '') {
                results += '<li class="header">' + Language.Search_by_Name + '</li>' + namesStr;
            }
            if (providerStr != '') {
                results += '<li class="header">' + Language.Search_by_Provider + '</li>' + providerStr;
            }
            if (contentStr != '') {
                results += '<li class="header">' + Language.Search_in_Content + '</li>' + contentStr;
            }
            if (pointsStr != '') {
                results += '<li class="header">' + Language.Search_by_Meeting_Point + '</li>' + pointsStr;
            }
            results += '</ul>';

            $('#searchQuickResults').html(results);
            $('#searchQuickResults').slideDown('fast');
        } else {
            UI.Search.close();
        }
    },

    custom_sort: function (a, b) {
        return a.Rating - b.Rating;
    }
}

const ControlsUI = {
    showMessage: function (data, selector, closeBox, eventID) {
        $('#' + selector + ' .status-bar').slideDown('fast');
        if (data.ResponseType >= 0) {
            $('#' + selector + ' .interaction-message').html(data.ResponseMessage).removeClass('error').addClass('confirmation');
            $('#' + selector).trigger('confirmed');
            //buscar si tiene código de conversión para cargar
            UI.Controls.triggerConversion(eventID);
            $('#' + selector + ' .interaction-loading').removeClass('active');
        } else {
            $('#' + selector + ' .interaction-message').html(data.ResponseMessage + data.ExceptionMessage).removeClass('confirmation').addClass('error');
            $('#' + selector).find('.modal-action').attr('disabled', null);
            $('#' + selector + ' .interaction-loading').removeClass('active');
        }
        if (data.ResponseType >= 0 && closeBox) {
            $('#' + selector).modal('close');
        }
    },

    triggerConversion: function (eventID, pointOfSaleID) {
        if (!isNaN(eventID)) {
            var iframe = document.createElement('iframe');
            iframe.style.width = '0px';
            iframe.style.height = '0px';
            document.body.appendChild(iframe);
            iframe.src = '/Home/ConversionCode/' + eventID + '?pointOfSaleID=' + pointOfSaleID;
        }
    },

    blockForm: function (selector, message) {
        $('#' + selector + ' .interaction-loading').addClass('active');
        $('#' + selector + ' .interaction-message').html(message);
        $('#' + selector).find('.modal-action').attr('disabled', 'disabled');
        $('#' + selector + ' .status-bar').slideDown('fast');
    },

    unblockForm: function (selector) {
        $('#' + selector + ' .interaction-loading').removeClass('active');
        $('#' + selector + ' .interaction-message').html('');
        $('#' + selector).find('.modal-action').attr('disabled', null);
        $('#' + selector + ' .status-bar').slideUp('fast');
    },

    Reviews: {
        init: function () {
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
        },

        onSuccess: function (data) {
            UI.Controls.unblockForm('divSubmitReview');
            $('#divSubmitReview').modal('close');
            var htmlString = '<li class="collection-item row yellow lighten-5" id="' + data.ObjectID + '" style="display:none;"><span class="review-info col s12 m3">';
            for (var s = 1; s <= parseInt($('#Rating').val()); s++) {
                htmlString += '<img src="/Content/themes/mex/images/stars_full.png" />';
            }
            for (var v = 1; v <= 5 - parseInt($('#Rating').val()); v++) {
                htmlString += '<img src="/Content/themes/mex/images/stars_empty.png" />';
            }
            htmlString += '<br />';
            htmlString += $('#Author').val();
            htmlString += '<span class="review-info-from">' + $('#From').val() + '</span>';
            var today = new Date();
            var dd = today.getDate();
            var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var mm = today.getMonth(); //January is 0!
            var yyyy = today.getFullYear();
            today = monthNames[mm] + ' ' + dd + ', ' + yyyy;
            htmlString += '<span class="review-info-saved">' + today + '</span>';
            htmlString += '</span><span class="review-description col s12 m9">' + $('#Review').val() + '</span></li>';

            $('#divReviews .collection').append(htmlString);
            $('.review-invitation').slideUp('fast');
            $('#' + data.ObjectID).slideDown('fast');
            let targetOffset = $('#divReviews .collection').offset().top;
            $('html,body').animate({ scrollTop: targetOffset }, 500);

            //limpiar form
            $('#AuthCode').val('');
            $('#divRating .answer-button').eq(4).trigger('click');
            $('#Author').val('');
            $('#From').val('');
            $('#Review').val('');
        }
    }
}

const MarketingAssistantUI = {
    alerts: function (title) {
        let randomPeople = Math.floor(Math.random() * 71);
        let randomPurchases = UI.MarketingAssistant.getEstimatedPurchases(title);
        let randomDiv = Math.floor(Math.random() * 2);
        let alerts = [];
        alerts[0] = randomPeople + Language.People_are_browsing + title + Language.Right_now;
        alerts[1] = randomPurchases + Language.People_have_purchased + title + Language.In_the_last_24_hours;
        Materialize.toast(alerts[randomDiv], 4000, 'rounded');
    },

    getEstimatedPurchases: function (title) {
        var purchases = 0;
        //obtener el registro
        var valueObj = Storage.get('ActivityPurchases');
        if (valueObj == undefined) {
            purchases = UI.MarketingAssistant.getRandomPurchases();
            //guardar el registro
            Storage.save('ActivityPurchases', [{ activity: title, purchases: purchases }])
        } else {
            //buscar el registro en el objeto guardado.
            $.each(valueObj, function (i, item) {
                if (item.activity == title) {
                    purchases = item.purchases;
                }
            });

            //si no lo encuentra, crearlo y guardar el registro
            if (purchases == 0) {
                purchases = UI.MarketingAssistant.getRandomPurchases();
                valueObj.push({ activity: title, purchases: purchases });
                Storage.save('ActivityPurchases', valueObj);
            }
        }
        return purchases;
    },

    getRandomPurchases: function () {
        return Math.floor(Math.random() * 26);
    }
}