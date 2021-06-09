var PricesEditor = function () {
    var PricesPerUnit = [];
    var RoundingRules = [];
    var Prices = [];
    var NewRules = [];
    var NewPrices = [];
    const IVA = 16;
    var init = function (id, specialRate, terminalID) {
        var date = new Date();
        $('#ParamsPricesEditor_Date').val(date.yyyymmdd());
        $('#ParamsPricesEditor_ServiceID').val(id);
        if (specialRate) {
            $('.prices-editor-special-rate').show();
        } else {
            $('.prices-editor-special-rate').hide();
        }
        $('#ParamsPricesEditor_TerminalID').val(terminalID);
        $('#btnRun').off('click').on('click', function () {
            getPricesInfo();
        });
    }

    function unitExists(priceid) {
        var exists = false;
        $.each(PricesEditor.PricesPerUnit, function () {
            if (this.PriceID == priceid) {
                exists = true;
            }
        });
        return exists;
    }

    function priceTypeExists(priceid, pricetypeid) {
        var exists = false;
        $.each(PricesEditor.PricesPerUnit, function () {
            if (this.PriceID == priceid) {
                $.each(this.Prices, function (p, price) {
                    if (price.PriceTypeID == pricetypeid) {
                        exists = true;
                    }
                });
            }
        });
        return exists;
    }

    function getPricesInfo() {
        var dataObject = {
            ParamsPricesEditor_TerminalID: $('#ParamsPricesEditor_TerminalID').val(),
            ParamsPricesEditor_ServiceID: $('#ParamsPricesEditor_ServiceID').val(),
            ParamsPricesEditor_Date: $('#ParamsPricesEditor_Date').val(),
            ParamsPriceEditor_PointOfSaleID: $('#ParamsPriceEditor_PointOfSaleID').val(),
            ParamsPriceEditor_Nationality: $('#ParamsPriceEditor_Nationality').val()
        };

        $('#divPricesEditor').slideUp('fast');
        $.post('/PricesEditor/GetPricesInfo', dataObject, function (data) {
            PricesEditor.RoundingRules = data.RoundingRules;
            //params
            $('#pe-lblProvider').text(data.Provider);
            $('#pe-lblItem').text(data.Item);
            $('#pe-lblContractCurrency').text(data.ProviderContractCurrency);
            $('#pe-lblContractDate').text(data.ProviderContractDate);
            $('#pe-lblExchangeRate').text(data.ExchangeRate);
            $('#pe-lblExchangeType').text(data.ExchangeRateType);

            //each rule
            var RulePriceTypes = [];
            $('.rule-columns').html('');
            $.each(data.Rules, function (r, rule) {
                var formula = '';
                if (rule.Formula != null) {
                    formula = rule.Formula;
                }
                if (formula == '' && rule.Percentage != null) {
                    formula = '=P' + rule.ThanPriceTypeID + (rule.MoreOrLess ? '+' : '-') + rule.Percentage + '%';
                }
                var ruleHtml = (RulePriceTypes.indexOf(rule.PriceTypeID) < 0 ? '<div class="rule-box-group">' : '');
                ruleHtml += '<div class="rule-box" data-index="rb-' + r + '" data-pricetype="' + rule.PriceTypeID + '" style="' + (RulePriceTypes.indexOf(rule.PriceTypeID) >= 0 ? 'display:none;' : '') + '">';
                ruleHtml += '<span class="rb-PriceTypeID">P' + rule.PriceTypeID + '</span>';
                ruleHtml += '<span class="pe-label1">' + rule.RuleFor + '</span>';
                ruleHtml += '<span class="pe-label2">' + (rule.IsBasePrice ? '<strong>Base Price</strong>' : 'Rule') + '</span>';
                ruleHtml += '<span class="pe-label2">for ' + rule.RuleFrom + '</span>';
                if (!rule.IsBasePrice) {

                    ruleHtml += '<span class="pe-field"><em>fx</em> <input type="text" id="fx-' + r + '" value="' + formula + '" class="pe-formula" data-pricetype="' + rule.PriceTypeID + '" data-level="' + rule.Level + '" data-rulefrom="' + rule.RuleFrom + '" data-originalformula="' + formula + '" /></span>';

                } else {
                    ruleHtml += '<span class="pe-field"></span>';
                }
                ruleHtml += '<span class="pe-label3">Vigency</span>';
                ruleHtml += '<span class="pe-label2 rule-from-date" data-originaldate="' + UTILS.parseSerializedDate(rule.FromDate) + '">' + UTILS.parseSerializedDate(rule.FromDate) + '</span>';
                ruleHtml += '<span class="pe-label2 rule-to-date">' + (rule.ToDate != null ? UTILS.parseSerializedDate(rule.ToDate) : 'Permanent') + '</span>';
                ruleHtml += '</div>';

                if (RulePriceTypes.indexOf(rule.PriceTypeID) >= 0) {
                    $('.rule-box[data-pricetype="' + rule.PriceTypeID + '"]').eq(0).after(ruleHtml);
                    $('.rb-tabs[data-pricetype="' + rule.PriceTypeID + '"]').append('<span class="rb-tab" data-index="rb-' + r + '">' + (rule.Level == "Unit" ? rule.RuleFrom : rule.Level) + '</span>')
                } else {
                    ruleHtml += '<span class="rb-tabs" data-pricetype="' + rule.PriceTypeID + '"><span class="rb-tab rb-add">+</span><span class="rb-tab rb-tab-active" data-index="rb-' + r + '">' + (rule.Level == "Unit" ? rule.RuleFrom : rule.Level) + '</span></span></div>';
                    $('.rule-columns').append(ruleHtml);
                    RulePriceTypes.push(rule.PriceTypeID);
                }
                $('.rb-tab').not('.rb-add').off('click').on('click', function () {
                    var index = $(this).attr('data-index');
                    var pricetypeid = $(this).parent().attr('data-pricetype');
                    $(this).siblings().removeClass('rb-tab-active');
                    $(this).addClass('rb-tab-active');
                    $('.rule-box[data-pricetype="' + pricetypeid + '"]').hide();
                    $('.rule-box[data-index="' + index + '"][data-pricetype="' + pricetypeid + '"]').show();
                });
            });

            //each price
            PricesEditor.Prices = data.Prices;
            $.each(data.Prices, function (p, price) {
                if (PricesEditor.PricesPerUnit.length == 0 || !unitExists(price.PriceID)) {
                    var priceUnit = {
                        PriceID: price.PriceID,
                        UnitEn: (price.Culture == "en-US" ? price.Unit : ''),
                        AdditionalInfoEn: (price.Culture == "en-US" ? price.AdditionalInfo : ''),
                        UnitEs: (price.Culture == "es-MX" ? price.Unit : ''),
                        AdditionalInfoEs: (price.Culture == "es-MX" ? price.AdditionalInfo : ''),
                        Min: price.Min,
                        Max: price.Max,
                        BwFromDate: UTILS.parseSerializedDate(price.FromDate),
                        BwToDate: (price.Permanent == true ? 'Permanent' : UTILS.parseSerializedDate(price.ToDate)),
                        TwFromDate: UTILS.parseSerializedDate(price.TwFromDate),
                        TwToDate: (price.TwPermanent == true ? 'Permanent' : UTILS.parseSerializedDate(price.TwToDate)),
                        GenericUnit: price.GenericUnit,
                        CreatedBy: price.CreatedBy,
                        CreatedOn: price.CreatedOn,
                        OnSite: price.OnSite,
                        OnLine: price.OnLine,
                        DependingOnPriceID: price.DependingOnPriceID,
                        Prices: []
                    };
                    var newPrice = {
                        PriceTypeID: price.PriceTypeID,
                        PriceUSD: (price.CurrencyID == 1 ? price.Price : 0),
                        PriceMXN: (price.CurrencyID == 2 ? price.Price : 0),
                        PriceBIVAUSD: this.PriceUSD - (this.PriceUSD * IVA / 100),
                        PriceBIVAMXN: this.PriceMXN - (this.PriceMXN * IVA / 100),
                        Rounding: price.Rounding,
                        IsBase: price.Base
                    }
                    priceUnit.Prices.push(newPrice);
                    PricesEditor.PricesPerUnit.push(priceUnit);
                }
                $.each(PricesEditor.PricesPerUnit, function (u, unit) {
                    if (unit.PriceID == price.PriceID) {
                        if (price.Culture == "en-US") {
                            unit.UnitEn = (price.Culture == "en-US" ? price.Unit : '');
                            unit.AdditionalInfoEn = (price.Culture == "en-US" ? price.AdditionalInfo : '');
                        } else {
                            unit.UnitEs = (price.Culture == "es-MX" ? price.Unit : '');
                            unit.AdditionalInfoEs = (price.Culture == "es-MX" ? price.AdditionalInfo : '');
                        }
                        if (!priceTypeExists(price.PriceID, price.PriceTypeID)) {
                            var newPrice = {
                                PriceTypeID: price.PriceTypeID,
                                PriceUSD: (price.CurrencyID == 1 ? price.Price : 0),
                                PriceMXN: (price.CurrencyID == 2 ? price.Price : 0),
                                PriceBIVAUSD: this.PriceUSD - (this.PriceUSD * IVA / 100),
                                PriceBIVAMXN: this.PriceMXN - (this.PriceMXN * IVA / 100),
                                Rounding: price.Rounding,
                                IsBase: price.Base
                            }
                            unit.Prices.push(newPrice);
                        }

                        $.each(unit.Prices, function (x, cprice) {
                            if (cprice.PriceTypeID == price.PriceTypeID) {
                                if (price.CurrencyID == 1) {
                                    cprice.PriceUSD = (price.CurrencyID == 1 ? price.Price : 0);
                                    cprice.PriceBIVAUSD = this.PriceUSD - (this.PriceUSD * IVA / 100);
                                } else {
                                    cprice.PriceMXN = (price.CurrencyID == 2 ? price.Price : 0);
                                    cprice.PriceBIVAMXN = this.PriceMXN - (this.PriceMXN * IVA / 100);
                                }
                            }
                        });
                    }
                });
            });

            renderUnits();
            renderPrices();

            $('.pe-formula').off('keyup').on('keyup', function (e) {
                var code = e.keyCode || e.which;
                //console.log(code);
                if (code == 13 || code == undefined) {
                    const priceTypeID = $(this).attr('data-pricetype');
                    const dataLevel = $(this).attr('data-level');
                    let level = $(this).attr('data-level');
                    if (level == "Unit") {
                        level = $(this).attr('data-rulefrom');
                    }
                    let contractCurrency = $('#pe-lblContractCurrency').text();
                    if (contractCurrency == "USD & MXN") {
                        if ($('#ParamsPricesEditor_Nationality').val() == "en-US") {
                            contractCurrency = "USD";
                        } else if ($('#ParamsPricesEditor_Nationality').val() == "es-MX") {
                            contractCurrency = "MXN";
                        }
                    }
                    const exchangeRate = $('#pe-lblExchangeRate').text();

                    //formula
                    let formula = $(this).val().replace(/ /g, '').replace('=', '');
                    let formulaBasePriceTypeID = (isNaN(formula.substr(formula.indexOf("P") + 1, 2)) ? formula.substr(formula.indexOf("P") + 1, 1) : formula.substr(formula.indexOf("P") + 1, 2));
                    //console.log('base price: ' + formulaBasePriceTypeID);

                    //reemplazo de porcentajes
                    var separators = ['=', '\\\+', '-', '\\\(', '\\\)', '\\*', '/', ':', '\\\?'];
                    var segments = formula.split(new RegExp(separators.join('|'), 'g')).filter(function (x) { return (x.length > 0 ? true : false) });
                    for (let i = 0; i < segments.length; i++) {
                        if (segments[i].endsWith("%")) {
                            formula = formula.replace(segments[i], '(P' + formulaBasePriceTypeID + '*' + segments[i].substr(0, segments[i].length - 1) + '/100)');
                        }
                    }
                    segments = formula.split(new RegExp(separators.join('|'), 'g')).filter(function (x) { return (x.length > 0 ? true : false) });

                    //obtención de excepciones
                    var levelException = [];
                    $('.rb-tabs[data-pricetype="' + priceTypeID + '"] .rb-tab').each(function () {
                        if ($(this).html() != "Terminal" && $(this).html() != "Provider" && $(this).html() != "Activity" && $(this).html() != "+" && $(this).html() != level) {
                            levelException.push($(this).html());
                        }
                    });

                    //console.log('fórmula de tipo de precio ' + priceTypeID + ' que afecta a todos los precios de nivel ' + level + (levelException.length > 0 ? ' excepto a los niveles ' + levelException : ''));

                    //recálculo
                    $.each(PricesEditor.PricesPerUnit, function (u, unit) {
                        var currentFormula = formula;
                        $.each(unit.Prices, function (p, price) {
                            if (price.PriceTypeID == formulaBasePriceTypeID) {
                                //obtener precio base de fórmula                           
                                for (let i = 0; i < segments.length; i++) {
                                    //reemplazo por precio
                                    if (segments[i].toString().indexOf("P") >= 0) {
                                        currentFormula = currentFormula.replace(segments[i], (contractCurrency == "USD" ? price.PriceUSD : price.PriceMXN));
                                    }
                                }
                            }

                            if (price.PriceTypeID == priceTypeID) {
                                //calcular precios
                                if (dataLevel == "Unit") {
                                    //afectar sólo a la unidad
                                    if (unit.GenericUnit == level) {
                                        if (contractCurrency == "USD") {
                                            try {
                                                price.PriceUSD = roundPrice(eval(currentFormula), priceTypeID);
                                                price.PriceMXN = roundPrice(price.PriceUSD * exchangeRate, priceTypeID);
                                            } catch (err) {
                                                price.PriceUSD = 0;
                                                price.PriceMXN = 0;
                                            }
                                        } else {
                                            try {
                                                price.PriceMXN = roundPrice(eval(currentFormula), priceTypeID);
                                                price.PriceUSD = roundPrice(price.PriceMXN / exchangeRate, priceTypeID);
                                            } catch (err) {
                                                price.PriceMXN = 0;
                                                price.PriceUSD = 0;
                                            }
                                        }
                                    }
                                } else {
                                    //afectar a todos excepto a las excepciones
                                    if (levelException.indexOf(unit.GenericUnit) < 0) {
                                        if (contractCurrency == "USD") {
                                            price.PriceUSD = roundPrice(eval(currentFormula), priceTypeID);
                                            price.PriceMXN = roundPrice(price.PriceUSD * exchangeRate, priceTypeID);
                                        } else {
                                            price.PriceMXN = roundPrice(eval(currentFormula), priceTypeID);
                                            price.PriceUSD = roundPrice(price.PriceMXN / exchangeRate, priceTypeID);
                                        }
                                    }
                                }
                                price.PriceBIVAUSD = price.PriceUSD - (this.PriceUSD * IVA / 100);
                                price.PriceBIVAMXN = price.PriceMXN - (this.PriceMXN * IVA / 100);
                            }
                        });
                    });

                    //render
                    renderPrices();

                    //cambio de fecha
                    if ($(this).val() != $(this).attr('data-originalformula')) {
                        let today = new Date();
                        $(this).parent().siblings('.rule-from-date').text(today.yyyymmdd()).addClass('mb-warning');
                    } else {
                        let originalDate = $(this).parent().siblings('.rule-from-date').attr('data-originaldate');
                        $(this).parent().siblings('.rule-from-date').text(originalDate).removeClass('mb-warning');
                    }

                    if (code == 13) {
                        $('.pe-formula').not(this).trigger('keyup');
                        console.log('dispara formulas');
                    }                    
                }
            });

            $('#divPricesEditor').slideDown('fast');
        }, 'json');
    }

    function roundPrice(amount, priceTypeID) {
        let price = amount;
        let ceiling = Math.ceil(price);
        let floor = Math.floor(price);
        $.each(PricesEditor.RoundingRules, function (r, rule) {
            if (rule.PriceTypeID == priceTypeID) {
                if (rule.RoundUp && rule.RoundDown) {
                    //redondeo por cercanía a unidad
                    price = (ceiling - price > .5 ? floor : ceiling);
                } else if (rule.RoundUp && !rule.RoundDown) {
                    if (rule.RoundToFifty) {
                        //redondeo a .50 y 1
                        price = (ceiling - price > .5 ? floor + .5 : ceiling);
                    } else {
                        //redondeo hacia arriba
                        price = ceiling;
                    }
                } else if (!rule.RoundUp && rule.RoundDown) {
                    //redondeo hacia abajo
                    price = floor;
                }
            }
        });
        return price;
    }

    function renderUnits() {
        $('.unit-prices').html('');
        $.each(PricesEditor.PricesPerUnit, function (i, unit) {
            //units
            var unitHtml = '<div class="unit-price" data-priceid="' + unit.PriceID + '">';
            unitHtml += '<span class="pe-column" style="width:35%;"><span class="pe-label1">' + unit.UnitEn + '</span>&nbsp;<span>' + unit.Min + '-' + unit.Max + '</span><span class="pe-label3">' + (unit.AdditionalInfoEn != null ? unit.AdditionalInfoEn : 'No Additional Info') + '</span></span>';
            unitHtml += '<span class="pe-column" style="width:64%;"><span class="pe-label-2" title="Period of time where you can buy the item" style="cursor: help;">Booking Window</span><br /><span class="pe-label1 original-bw" data-originalbw="' + unit.BwFromDate.substr(0, 10) + ' / ' + unit.BwToDate + '" style="font-weight:normal;">' + unit.BwFromDate.substr(0, 10) + ' / ' + unit.BwToDate + '</span></span>';
            unitHtml += '<span class="pe-column" style="width:35%;"><span class="pe-label-1">' + unit.UnitEs + '</span>&nbsp;<span class="">' + unit.Min + '-' + unit.Max + '</span><span class="pe-label3">' + (unit.AdditionalInfoEs != null ? unit.AdditionalInfoEs : 'Sin Info Adicional') + '</span></span>';
            unitHtml += '<span class="pe-column" style="width:64%;"><span class="pe-label-2" style="cursor:help;" title="Period of time where you can use the coupon">Travel Window</span><br /><span class="pe-label1 original-tw" data-originaltw="' + unit.TwFromDate.substr(0, 10) + ' / ' + unit.TwToDate + '" style="font-weight:normal;">' + unit.TwFromDate.substr(0, 10) + ' / ' + unit.TwToDate + '</span></span>';
            unitHtml += '<span class="pe-column" style="width:35%;">Unit: ' + unit.GenericUnit + '<br /><span class="pe-label3">' + (unit.OnSite ? 'On Site' : '') + (unit.OnSite && unit.OnLine ? ' & ' : '') + (unit.OnLine ? 'On Line' : '') + '<br />' + (unit.DependingOnPriceID != null ? 'Dependent Price' : 'Independent Price') + '</span></span>';
            unitHtml += '<span class="pe-column" style="width:64%; text-align:right;"><span class="pe-label3">Created by</span> ' + unit.CreatedBy + '<br><span class="pe-label3">' + unit.CreatedOn + '</span></span>';
            unitHtml += '</div>';
            $('.unit-prices').append(unitHtml);
        });
    }

    function renderPrices() {
        $('.rule-prices').html('');
        let contractCurrency = $('#pe-lblContractCurrency').text();
        if (contractCurrency == "USD & MXN") {
            if ($('#ParamsPricesEditor_Nationality').val() == "en-US") {
                contractCurrency = "USD";
            } else if ($('#ParamsPricesEditor_Nationality').val() == "es-MX") {
                contractCurrency = "MXN";
            }
        }
        $.each(PricesEditor.PricesPerUnit, function (i, unit) {
            //prices
            var priceHtml = '<div class="rule-prices-row">';
            $.each(unit.Prices, function (p, price) {
                priceHtml += '<div class="rule-price" data-pricetype="' + price.PriceTypeID + '" data-unit="' + unit.GenericUnit + '">';
                priceHtml += '<strong><span ' + (!price.IsBase ? 'data-format="currency"' : '') + ' class="pe-label1">' + (price.IsBase ? '$<input type="text" class="pe-field-money" data-priceid="' + unit.PriceID + '" value="' : '') + (contractCurrency == "USD" ? price.PriceUSD : price.PriceMXN) + (price.IsBase ? '" />' : '') + (contractCurrency == "USD" ? ' USD' : ' MXN') + '</span></strong>';
                priceHtml += '<span data-format="currency" class="pe-label3" style="width: 141px;">' + (contractCurrency == "USD" ?price.PriceBIVAUSD + ' USD': price.PriceBIVAMXN + ' MXN') + '</span> <span class="pe-label3">b/ IVA</span><br /><br />';

                priceHtml += '<span data-format="currency" class="pe-label2">' + (contractCurrency == "USD" ? price.PriceMXN + ' MXN' : price.PriceUSD + ' USD') + '</span>';
                priceHtml += '<span data-format="currency" class="pe-label3"  style="width: 141px;">' + (contractCurrency == "USD" ? price.PriceBIVAMXN + " MXN" : price.PriceBIVAUSD + " USD") + '</span> <span class="pe-label3">b/ IVA</span><br /><br />';
                priceHtml += '<span class="pe-label3">' + price.Rounding + '</span>';
                priceHtml += '</div>';
            });
            priceHtml += '</div>';
            $('.rule-prices').append(priceHtml);
        });
        //formato de moneda
        UI.applyFormat('currency');
        //formato .00 en inputs
        $('.pe-field-money').each(function () {
            $(this).val(parseFloat($(this).val()).formatMoney(2));
        });
        //cambio de precio base
        $('.pe-field-money').off('keyup').on('keyup', function (e) {
            var code = e.keyCode || e.which;
            if(code == 13) { 
                const priceID = $(this).attr('data-priceid');
                let contractCurrency = $('#pe-lblContractCurrency').text();
                if (contractCurrency == "USD & MXN") {
                    if ($('#ParamsPricesEditor_Nationality').val() == "en-US") {
                        contractCurrency = "USD";
                    } else if ($('#ParamsPricesEditor_Nationality').val() == "es-MX") {
                        contractCurrency = "MXN";
                    }
                }
                const exchangeRate = $('#pe-lblExchangeRate').text();
                const priceValue = $(this).val();
                $.each(PricesEditor.PricesPerUnit, function (u, unit) {
                    if (unit.PriceID == priceID) {
                        //console.log('priceid: ' + priceID);
                        $.each(unit.Prices, function (p, price) {
                            if (price.IsBase) {
                                //console.log('is base');
                                if (contractCurrency == "USD") {
                                    //console.log('contract USD');
                                    price.PriceUSD = priceValue;
                                    price.PriceMXN = price.PriceUSD * exchangeRate;
                                } else {
                                    //console.log('contract MXN');
                                    price.PriceMXN = priceValue;
                                    price.PriceUSD = price.PriceMXN / exchangeRate;
                                }
                                price.PriceBIVAUSD = price.PriceUSD - (this.PriceUSD * IVA / 100);
                                price.PriceBIVAMXN = price.PriceMXN - (this.PriceMXN * IVA / 100);
                            }
                        });
                    }
                });

                //si el precio cambió, cambiar la vigencia
                let originalBasePrice = getBasePrice(priceID);
                if (priceValue != originalBasePrice.Price) {
                    let today = new Date();
                    $('.unit-price[data-priceid="' + priceID + '"] .original-bw').text(today.yyyymmdd() + ' / Permanent').addClass('mb-warning');
                    $('.unit-price[data-priceid="' + priceID + '"] .original-tw').text(today.yyyymmdd() + ' / Permanent').addClass('mb-warning');
                } else {
                    $('.unit-price[data-priceid="' + priceID + '"] .original-bw').text($('.unit-price[data-priceid="' + priceID + '"] .original-bw').attr('data-originalbw')).removeClass('mb-warning');
                    $('.unit-price[data-priceid="' + priceID + '"] .original-tw').text($('.unit-price[data-priceid="' + priceID + '"] .original-tw').attr('data-originaltw')).removeClass('mb-warning');
                }

                //desencadenar cálculo de fórmulas
                $('.pe-formula').trigger('keyup');
            }
        });
    }

    function getBasePrice(priceID) {
        let basePrice;
        $.each(PricesEditor.Prices, function (p, price) {
            if (price.Base && price.PriceID == priceID) {
                basePrice = price;
            }
        });
        return basePrice;
    }

    return {
        init: init,
        PricesPerUnit: PricesPerUnit,
        RoundingRules: RoundingRules,
        Prices: Prices
    }
}();