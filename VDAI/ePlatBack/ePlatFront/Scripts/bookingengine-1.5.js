/// <reference path="storage-1.0.js" />

let source = window.location.host;
//let source = 'localhost:37532';

const BookingEngineModel = {
    init: function () {
        let obj = Object.create(this);
        obj.CartID = '00000000-0000-0000-0000-000000000000';
        obj.Items = [];
        obj.ItemsTotal = 0;
        obj.Total = 0;
        obj.Savings = 0;
        obj.PromoID = null;
        obj.PromoCode = '';
        obj.CampaignTag = null;

        let domain = window.location.host;
        if (domain.indexOf("mx") >= 0) {
            obj.Culture = 'es-MX';
            obj.CurrencyID = 2;
        } else {
            obj.Culture = 'en-US';
            obj.CurrencyID = 1;
        }

        obj.CatalogID = 0;
        obj.PointOfSaleID = 0;
        obj.TerminalID = 0;

        obj.Session = {
            ServiceID: 0,
            Service: '',
            CurrentMonth: 0,
            CurrentYear: 0,
            CalendarServiceID: 0,
            ServiceType: 1,
            Process: '',
            CartItemID: ''
        };

        obj.XHR = {
            Availability: null,
            Prices: null,
            Promos: null
        };

        let myCart = Storage.get("BookingEngine.Cart");
        if (myCart == undefined || myCart.CartID == "empty") {
            //obtener
            $.getJSON('//' + source + '/Purchase/GetCartSettings', null, function (data) {
                obj.TerminalID = data.TerminalID;
                obj.PointOfSaleID = data.PointOfSaleID;
                obj.CatalogID = data.CatalogID;
                $("body").trigger('CartLoaded');
            });
        } else {
            obj.CartID = myCart.CartID;
            obj.Items = myCart.Items;
            obj.ItemsTotal = myCart.ItemsTotal;
            obj.Total = myCart.Total;
            obj.Savings = myCart.Savings;
            obj.PromoID = myCart.PromoID;
            obj.PromoCode = myCart.PromoCode;
            obj.CampaignTag = myCart.CampaignTag;

            obj.Culture = myCart.Culture;
            obj.CatalogID = myCart.CatalogID;
            obj.CurrencyID = myCart.CurrencyID;
            obj.PointOfSaleID = myCart.PointOfSaleID;
            obj.TerminalID = myCart.TerminalID;
        }

        return obj;
    },

    save: function () {
        Storage.save("BookingEngine.Cart", this, 90);
    },

    deleteCart: function () {
        this.CartID = 'empty';
        Storage.save("BookingEngine.Cart", this, 90);
    },

    getServiceType: function (callback) {
        let dataObject = {
            terminalid: this.TerminalID
        }
        $.getJSON('//' + source + '/Activities/GetServiceType/' + this.Session.ServiceID, dataObject, function (data) {
            callback(data);
        });
    },

    getTransportationPrices: function (date, placeID, zoneID, round, callback) {
        if (this.XHR.Prices && this.XHR.Prices.readyState != 4) {
            this.XHR.Prices.abort();
        }
        this.XHR.Prices = $.ajax({
            url: '//' + source + '/Activities/GetTransportationPrices/' + this.Session.ServiceID,
            data: {
                id: this.Session.ServiceID,
                date: date,
                placeId: placeID,
                transportationZoneId: zoneID,
                round: round,
                terminalid: this.TerminalID,
                culture: this.Culture
            },
            success: function (data) {
                callback(data);
            }
        });
    },

    getResortsByTerminal: function (callback) {
        $.getJSON('//' + source + '/Controls/GetResortsByTerminal/' + Cart.TerminalID, null, function (data) {
            callback(data);
        });
    },

    getDisabledDays: function (callback) {
        if (this.XHR.Availability && this.XHR.Availability.readyState != 4) {
            this.XHR.Availability.abort();
        }
        this.XHR.Availability = $.ajax({
            url: '//' + source + '/Activities/GetAvailableDatesV2/' + this.Session.ServiceID,
            data: {
                month: this.Session.CurrentMonth,
                year: this.Session.CurrentYear,
                terminalid: this.TerminalID
            },
            success: function (data) {
                callback(data.DatesString);
            }
        });

    },

    getSchedulesAndPrices: function (date, callback) {
        if (this.XHR.Prices && this.XHR.Prices.readyState != 4) {
            this.XHR.Prices.abort();
        }
        this.XHR.Prices = $.ajax({
            url: '//' + source + '/Activities/GetSchedulesAndPrices/' + this.Session.ServiceID,
            data: {
                date: date,
                terminalid: this.TerminalID,
                culture: this.Culture
            },
            success: function (data) {
                callback(data);
            }
        });
    },

    calculateTotal: function () {
        var total = 0;
        $.each(this.Items, function (i, cartitem) {
            if (cartitem.Deleted != true) {
                total += parseFloat(cartitem.PromoTotal);
            }
        });

        this.ItemsTotal = total;
        if (this.PromoID == null) {
            this.Total = total;
        }

        //sumar savings de los elementos
        var savings = 0;
        $.each(this.Items, function (j, item) {
            if (item.Deleted != true) {
                savings += parseFloat(item.PromoSavings);
            }
        })

        this.Savings = savings;
    },

    deleteItem: function (cartitemid) {
        let indexToDelete = null;
        $.each(this.Items, function (i, item) {
            if (item.CartItemID == cartitemid) {
                if (item.DateSaved != null) {
                    item.Deleted = true;
                } else {
                    indexToDelete = i;
                }
                return false;
            }
        });
        if (indexToDelete != null) {
            this.Items.splice(indexToDelete, 1);
        }
    },

    applyPromo: function (promoCode, callback) {
        if (this.Items.length > 0) {
            //recalcular servicios antes de aplicar la promo
            if (promoCode == null || promoCode == '') {
                $.each(this.Items, function (j, item) {
                    item.PromoTotal = item.Total;
                    item.PromoSavings = item.Savings;
                    Cart.Items[j] = item;
                });
            }
            //buscar promos
            let serviceids = '';
            let travelDates = '';
            let conditionalPrice = false;
            $.each(this.Items, function (i, cartitem) {
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
                if (this.XHR.Promos && this.XHR.Promos.readyState != 4) {
                    this.XHR.Promos.abort();
                }
                this.XHR.Promos = $.ajax({
                    url: '//' + source + '/Purchase/GetPromo',
                    data: {
                        services: serviceids,
                        traveldates: travelDates,
                        terminalid: this.TerminalID,
                        promocode: promoCode
                    },
                    success: function (data) {
                        //console.log(data);
                        let promoCodeTotalDiscount = 0;
                        //aplicar la promo a los items y guardarlos en el cartlocal
                        if (data.ResponseType == 1) {
                            $.each(data.Promos, function (p, promo) {
                                if (promo.Apply) {
                                    //aplicar promo
                                    let arrServices = (promo.ServiceIDs != "" ? promo.ServiceIDs.split(',') : serviceids.split(','));
                                    switch (promo.PromoTypeID) {
                                        case 1:
                                        case 2:
                                        case 3:
                                            if (promo.ApplyOnPerson) {
                                                for (var i = 0; i < arrServices.length; i++) {
                                                    $.each(Cart.Items, function (j, item) {
                                                        let AppliedPromo = (item.PromoID != null ? true : false);
                                                        if (item.ServiceID == arrServices[i]) {
                                                            let totalPax = 0;
                                                            let cheapestUnit = 0;
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
                                                                let savings = 0;
                                                                let totalPromo = 0;

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
                                                let cheapestActivity;
                                                let cheapestTotal = 0;
                                                let promoUnits = 0;
                                                for (var i = 0; i < arrServices.length; i++) {
                                                    $.each(Cart.Items, function (j, item) {
                                                        //item.PromoID = null;
                                                        if (item.ServiceID == arrServices[i]) {
                                                            let units = 0;
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
                                                    let AppliedPromo = (item.PromoID != null ? true : false);
                                                    if (item.ServiceID == cheapestActivity) {
                                                        let units = 0;
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
                                                    let AppliedPromo = (item.PromoID != null ? true : false);
                                                    //item.PromoID = null;
                                                    if (item.ServiceID == arrServices[i]) {
                                                        //promotional code
                                                        if (promo.PromoCode != null && Cart.PromoID == null) {
                                                            Cart.PromoID = promo.PromoID;
                                                            Cart.PromoCode = promoCode;
                                                        }
                                                        if (promo.PromoCode != null) {
                                                            if (promo.DiscountType == '%') {
                                                                //descuento de porcentaje
                                                                let itemDiscount = (parseFloat(Cart.Items[j].PromoTotal) * parseFloat(promo.Discount) / 100);
                                                                Cart.Items[j].PromoTotal = Cart.Items[j].PromoTotal - itemDiscount;
                                                                Cart.Total -= itemDiscount;
                                                                promoCodeTotalDiscount += itemDiscount;
                                                            } else {
                                                                //descuento monetario

                                                            }
                                                        } else {
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
                                                    }
                                                });
                                            }

                                            break;
                                    }
                                } else {
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

                        Cart.calculateTotal();
                        if (promoCode != undefined && callback != undefined) {
                            callback(promoCodeTotalDiscount);
                        }
                        Cart.save();
                    }
                });
            }
        } else {
            this.calculateTotal();
        }

        //if (promoCode == "" && Cart.PromoID != null) {
        //    Cart.applyPromo(Cart.PromoCode, callback);
        //}
    },

    getItem: function (cartitemid) {
        let currentItem = null;
        $.each(this.Items, function (i, item) {
            if (item.CartItemID == cartitemid) {
                currentItem = item;
                return false;
            }
        });
        return currentItem;
    }
}