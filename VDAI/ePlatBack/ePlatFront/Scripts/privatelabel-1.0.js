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

var UI = function () {
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
            //console.log('b: ' + $.toJSON(Cart.Items));
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
                        $('#formProsa>input[name="order_id"]').val(data.PurchaseID.replace(/-/g, "").substr(0, 26));
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
        $('#sub-shopping-cart').animate({
            height: (parseInt($('#divAddActivity').height()) + parseInt(40)) + 'px'
        }, 300, 'easeOutQuart');
    }

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

    var init = function () {
        //console.log('UI.init()');
        //console.log(BookingEngine);
        enablePlaceHolders();
        defineEvents();

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
    }

    return {
        Storage: Storage,
        adjustCartHeight: adjustCartHeight,
        sendingInfo: sendingInfo,
        confirmation: confirmation,
        init: init
    }
}();

var Utils = function () {
    var getParam = function (key) {
        var scripts = document.getElementsByTagName('script');
        var url;
        var value = "";

        for (var s = 0; s < scripts.length; s++) {
            if (scripts[s].src.indexOf('privatelabel-') > 0) {
                url = scripts[s].src;
            }
        }

        if (url != "") {
            var x = url.substr(url.indexOf('?')).replace(/\?/g, '').split("&")
            for (var i = 0; i < x.length; i++) {
                var y = x[i].split("=");
                if (y[0] == key) {
                    value = y[1];
                }
            }
        }
        return value;
    }

    return {
        getParam: getParam
    }
}();

var PrivateLabel = function () {
    var container, catalogid, culture;
    var site = 'https://eplatfront.villagroup.com';
    var siteTest = 'http://localhost:37532';

    var Settings = [
        {
            catalogid: 1024,
            domain: 'beta.villagroupresorts.com',
            terminalid: 5
        }
    ];

    function getTerminal() {
        var terminalid = 0;
        $.each(Settings, function (s, setting) {
            if (setting.catalogid == catalogid && setting.domain == window.location.hostname) {
                terminalid = setting.terminalid
            }
        });

        //console.log('terminal:' + terminalid);

        return terminalid;
    }

    var init = function () {
        //console.log('PrivateLabel.init()');
        //cargar estilo
        $('head').append($('<link rel="stylesheet" type="text/css" />').attr('href', siteTest + '/Content/themes/vgr/css/privatelabel-1.0.css'));
        $('head').append($('<link rel="stylesheet" type="text/css" />').attr('href', siteTest + '/content/themes/base/datepicker'));

        $.getScript(siteTest + '/bundles/modernizr', function () {
            $.getScript(siteTest + '/scripts/bookingengine-1.0.js', function () {
                $.getScript(siteTest + '/Scripts/jquery-ui-timepicker-addon.js', function () {
                    $.getScript(siteTest + '/content/themes/mex/js/language.en.js', function () {
                        $.getScript(siteTest + '/Content/plugins/hashchange/jquery.hashchange.min.js', function () {
                            launch();
                        });
                    });
                });
            });
        });  
    }

    function launch() {
        $(window).hashchange(function () {
            $('a[href=' + location.hash + ']').trigger('click');
        });
        $(window).trigger('hashchange');

        //params
        container = Utils.getParam('container');
        catalogid = Utils.getParam('catalogid');
        culture = Utils.getParam('lan');
        if (culture == '') {
            culture = 'en-US';
        }

        //cargar estructura
        console.log('cargar estructura');
        $.getJSON(siteTest + '/PrivateLabel/GetHTMLStructure', null, function (response) {
            $('#' + container).html(response.HTML);
            //console.log('estructura cargada');

            //cargar UI
            //console.log('trigger UI.init()');
            UI.init();

            //obtener menus
            //console.log('culture' + culture);
            //console.log('terminal' + getTerminal());
            var dataObject = {
                culture: culture,
                terminalid: getTerminal()
            }
            //console.log(dataObject);
            //console.log('cargar catalogo');
            $.getJSON(siteTest + '/PrivateLabel/GetCatalog/' + catalogid, dataObject, function (response) {
                var activeParentCategory = 0;
                $.each(response.ParentCategories, function (p, parentCat) {
                    if (p == 0) {
                        activeParentCategory = parentCat.ParentCategoryID;
                    }
                    $('#pl-menu li.shopping-cart').before('<li ' + (p == 0 ? 'class="active"' : '') + '><a href="#sectionid-' + parentCat.ParentCategoryID + '" data-id="' + parentCat.ParentCategoryID + '" class="parent-category">' + parentCat.ParentCategoryName + '</a></li>');

                    $('#pl-shoppingCart').before('<ul class="submenu" data-parent="' + parentCat.ParentCategoryID + '" style="display:none"></ul>');

                    $.each(parentCat.Categories, function (c, cat) {
                        $('ul.submenu[data-parent="' + parentCat.ParentCategoryID + '"]').append('<li><a href="#categoryid-' + cat.CategoryID + '" data-id="' + cat.CategoryID + '">' + cat.CategoryName + '</a></li>');
                    });
                });
                //mostrar default
                $('ul.submenu[data-parent="' + activeParentCategory + '"]').slideDown('fast');
                //agregar eventos
                $('#pl-menu li a.parent-category').off('click').on('click', function () {
                    $('ul.submenu').slideUp('fast');
                    $('#pl-shoppingCart').slideUp('fast');
                    $('ul.submenu[data-parent="' + $(this).attr('data-id') + '"]').stop().slideDown('fast');
                    $(this).parent().addClass('active').siblings().removeClass('active');
                });
                $('li.shopping-cart a').on('click', function () {
                    $(this).parent().addClass('active').siblings().removeClass('active');
                    $('ul.submenu').slideUp('fast');
                    $('#pl-shoppingCart').slideDown('fast');
                });
                $('ul.submenu li a').off('click').on('click', function () {
                    var categoryName = $(this).text();
                    $('#pl-categoryName').text('Getting ' + categoryName + ' Activities...');
                    $('#pl-category').slideDown('fast');
                    $('#pl-activities').slideUp('fast').html('');
                    $('#pl-activity').slideUp('fast');
                    //cargar actividades de categoría
                    $.getJSON(site + '/PrivateLabel/GetActivitiesForCategory/' + $(this).attr('data-id'), dataObject, function (data) {
                        $.each(data, function (a, item) {
                            $('#pl-activities').append('<figure class="tile-activity" id="' + item.ActivityID + '">' + (item.Tag != null && item.Tag != "" ? '<div class="tile-activity-tag" style="background-color:' + item.TagColor + '">' + item.Tag + '</div>' : '') + '<img alt="' + item.Activity + '" src="//eplatfront.villagroup.com/' + item.Pictures[0].Picture + '?width=392&height=220&mode=crop&quality=50" width="196" height="110" /><figcaption class="tile-text"><span class="activity-price">$' + item.RetailPrice + '</span><h2>' + item.Activity + '</h2><a class="a-info" href="#activityID-' + item.ActivityID + '" data-id="' + item.ActivityID + '" title="More Info about ' + item.Activity + '">See Details</a><a class="a-book" href="#shopping-cart" onclick="BookingEngine.Service.book(' + item.ActivityID + ',\'' + item.Activity.replace("'", "") + '\')" title="Book ' + item.Activity + '">Book This</a></figcaption></figure>');
                        });
                        $('#pl-categoryName').text(categoryName);
                        $('#pl-activities').stop().slideDown('fast');
                        //agregar eventos a actividades
                        $('.a-info').on('click', function () {
                            var id = $(this).attr('data-id');
                            $.getJSON(site + '/PrivateLabel/GetActivityDetail/' + id, dataObject, function (model) {
                                $('#pl-activityName').text(model.Activity);
                                //gallery
                                var gallerypics = '';
                                var gallerysels = '';
                                $.each(model.Pictures, function (p, picture) {
                                    gallerypics += '<figure id="' + picture.PictureID + '" ' + (p != 0 ? 'style="display:none;"' : '') + '><img src="//eplatfront.villagroup.com' + picture.Picture + '?width=920&height=520&mode=crop&quality=50" alt="' + (picture.Description != null ? picture.Description : model.Activity) + '" /><figcaption>' + (picture.Description != null ? picture.Description : '') + '</figcaption></figure>';
                                    gallerysels += '<img data-picid="' + picture.PictureID + '" src="//eplatfront.villagroup.com/' + picture.Picture + '?width=110&height=72&mode=crop&quality=50" width="55" height="36" />';
                                });
                                $('#pl-gallerySelector').before(gallerypics);
                                $('#pl-gallerySelector').append(gallerysels);
                                $('#pl-gallerySelector img').on('click', function () {
                                    $('#pl-gallery figure').fadeOut('fast');
                                    $('#' + $(this).attr('data-picid')).fadeIn('fast');
                                });
                                //features
                                if (!model.Features_BabiesAllowed) {
                                    $('#pl-babiesAllowed').css('opacity', '.2');
                                }
                                if (!model.Features_ChildrenAllowed) {
                                    $('#pl-childrenAllowed').css('opacity', '.2');
                                }
                                if (!model.Features_AdultsAllowed) {
                                    $('#pl-adultsAllowed').css('opacity', '.2');
                                }
                                if (!model.Features_PregnantsAllowed) {
                                    $('#pl-pregnantsAllowed').css('opacity', '.2');
                                }
                                if (!model.Features_OldiesAllowed) {
                                    $('#pl-oldiesAllowed').css('opacity', '.2');
                                }
                                $('#pl-activityLength').text(model.Length);
                                //offer
                                if (model.Promo_MainTag != null && model.Promo_MainTag != "") {
                                    $('#pl-offer')
                                        .show()
                                        .css('background-color', model.Promo_TagColor)
                                        .css('color', model.Promo_TextColor);
                                    $('#pl-offer .main-tag').text(model.Promo_MainTag);
                                    $('#pl-offer .secondary-tag h3').text(model.Promo_TitleTag);
                                    $('#pl-offer .secondary-tag span').html(model.Promo_Description);
                                    $('#pl-offerInstructions').html(model.Promo_Instructions);
                                } else {
                                    $('#pl-offer').hide();
                                }
                                //description
                                $('#pl-description span').html(model.Description);
                                if (model.Itinerary != null && model.Itinerary != "") {
                                    $('#pl-itinerary span').html(model.Itinerary);
                                    $('#pl-itinerary').show();
                                } else {
                                    $('#pl-itinerary').hide();
                                }
                                if (model.Included != null && model.Included != "") {
                                    $('#pl-included span').html(model.Included);
                                    $('#pl-included').show();
                                } else {
                                    $('#pl-included').hide();
                                }
                                if (model.Recommendations != null && model.Recommendations != "") {
                                    $('#pl-recommendations span').html(model.Recommendations);
                                    $('#pl-recommendations').show();
                                } else {
                                    $('#pl-recommendations').hide();
                                }
                                if (model.Notes != null && model.Notes != "") {
                                    $('#pl-notes span').html(model.Notes);
                                    $('#pl-notes').show();
                                } else {
                                    $('#pl-notes').hide();
                                }
                                if (model.Restrictions != null && model.Restrictions != "") {
                                    $('#pl-restrictions span').html(model.Restrictions);
                                    $('#pl-restrictions').show();
                                } else {
                                    $('#pl-restrictions').hide();
                                }

                                //prices
                                $('#pl-prices-detail').html('');
                                if (!model.IsTransportation) {
                                    $.each(model.Prices, function (p, price) {
                                        var priceStr = '<div class="price"><div class="price-unit">' + price.Unit + (price.UnitMin != null ? ' <text>(' + price.UnitMin + ' - ' + price.UnitMax + ' years)</text>' : '') + '</div><div class="price-offer">$' + price.RetailPrice + price.Currency + '</div></div>';
                                        $('#pl-prices-detail').append(priceStr);
                                    });
                                }

                                //schedules
                                $('#pl-schedules').html('');
                                $.each(model.Schedules, function (s, schedule) {
                                    if (schedule.Time != "") {
                                        $('#pl-schedules').append('<span id="' + schedule.Day + '" class="day-schedule" ' + (s != 0 ? 'style="display:none;"' : '') + '>' + schedule.Time + '</span>');
                                    }
                                });

                                $('#pl-schedules').children().each(function (i) {
                                    $('div[data-day=' + $(this).attr('id') + ']').addClass('day-button-avaiable');
                                });
                                $('#pl-schedules').children().eq(0).show();
                                $('.day-button-avaiable').eq(0).addClass('day-button-selected');
                                $('.day-button-avaiable').on('click', function () {
                                    $('.day-button-selected').removeClass('day-button-selected');
                                    $(this).addClass('day-button-selected');
                                    $('.day-schedule').hide();
                                    $('#' + $(this).attr('data-day')).show();
                                });

                                //meeting points
                                $('ul.meeting-points').html('');
                                $.each(model.MeetingPoints, function (p, place) {
                                    $('ul.meeting-points').append('<li data-lat="' + place.Lat + '" data-lng="' + place.Lng + '"><img src="//eplatfront.villagroup.com/content/themes/mex/images/icon_location.png" class="place-icon" /><span class="place"><span class="name">' + place.Place + '</span><span class="address">' + place.Address + '</span></span></li>');
                                });

                                //mostrar
                                $('#pl-category').slideUp('fast');
                                $('#pl-activity').slideDown('fast');
                            });
                        });
                    });
                });
            });
        });
    }

    return {
        init: init
    }
}();

$(function () {
    //console.log('trigger PrivateLabel.init()');
    PrivateLabel.init();
});

//guardar ultima categoria abierta
//guardar historial de peticiones de listados de actividades y detalle de actividad