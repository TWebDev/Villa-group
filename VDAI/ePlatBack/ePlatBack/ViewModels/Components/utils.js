var ePlatUtils = {
    data: function () {
        return {
            UIData: {
                showSearchCard: false
            },
            DependentFields: {
                Fields: [],
                Url: '',
                UpdateOnTerminalChange: false
            }
        }
    },
    methods: {
        UI: function () {
            self = this;
            return {
                showSearchCard: function () {
                    if (self.UIData.showSearchCard) {
                        self.UIData.showSearchCard = false
                    } else {
                        self.UIData.showSearchCard = true
                    }
                },
                scrollTo: function (elementID, scrollingDuration) {
                    if (scrollingDuration == null) { scrollingDuration = 500 }
                    //let headerHeight = parseInt($("header").height());
                    let navHeight = parseInt($("nav").height());
                    let currentOffSet = $("#" + elementID).offset().top;
                    let newScrollTop = currentOffSet - navHeight;
                    if ($('.fixed-panel').length > 0 && newScrollTop > 1120) {
                        newScrollTop -= 120;
                    }
                    $('html, body').animate({ scrollTop: newScrollTop }, scrollingDuration);
                },
                loadTerminalDependentLists: function () {
                    let terminalsList = [];
                    if (self.Shared.Session.UserTerminals.length > 0) {
                        let Terminals = self.Shared.Session.UserTerminals.filter(function (item) {
                            return self.Shared.Session.TerminalIDs.split(',').indexOf(item.TerminalID.toString()) >= 0;
                        });

                        $.each(Terminals, function (t, terminal) {
                            terminalsList.push({
                                label: terminal.Terminal,
                                title: terminal.Terminal,
                                value: terminal.TerminalID
                            });
                        });
                    }
                    self.Shared.Session.TerminalsList = terminalsList;
                    $('.terminal-dependent-list[multiple="multiple"]').multiselect('dataprovider', self.Shared.Session.TerminalsList);
                    $('body').trigger('selectedTerminalChanged');
                },
                stripTags: function (str) {
                    return str.replace(/<\/?[^>]+>/gi, '');
                },
                loadDependentFields: function (url, updateOnTerminalChange, callback) {
                    self.DependentFields.Url = url;
                    self.DependentFields.UpdateOnTerminalChange = updateOnTerminalChange;
                    $.ajax({
                        url: url,
                        cache: false,
                        type: 'GET',
                        success: function (data) {
                            self.DependentFields.Fields = data.Fields;
                            $.each(self.DependentFields.Fields, function (f, field) {
                                $('#' + field.ParentField).on('change', function () {
                                    var options = '';
                                    var parentValue = $(this).val();
                                    var validFields = 0;
                                    $('#' + field.Field).html('');
                                    $.each(field.Values, function (v, value) {
                                        var grandParentValue = $('#' + field.GrandParentField).val();
                                        if (value.GrandParentValue == grandParentValue || value.GrandParentValue == null) {
                                            if (value.ParentValue == parentValue || value.ParentValue == null) {
                                                if (value.Value != '') {
                                                    validFields += 1;
                                                }
                                                options += '<option value="' + value.Value + '">' + value.Text + '</option>';
                                            }
                                        }
                                    });
                                    $('#' + field.Field).html(options);
                                    if ($('#' + field.Field).prop('multiple')) {
                                        $('[multiple="multiple"]').multiselect('rebuild');
                                    }
                                    if (validFields == 1) {
                                        $('#' + field.Field + ' option').eq(1).prop('selected', true).trigger('change');
                                    }
                                });
                            });
                            if (typeof callback == 'function') {
                                callback();
                            }
                        }
                    });
                },
                filterTarget: function (url) {
                    //eliminar dominio
                    if (url.indexOf('//') >= 0) {
                        url = url.replace('//', '');
                        url = url.substr(url.indexOf('/'));
                    }
                    //reemplazar null, números y arreglos de los parámetros
                    var params = url.split('&');
                    var newUrl = '';
                    $.each(params, function (s, string) {
                        var value = string.substr(string.indexOf('=') + 1);
                        if (value == '') {
                            newUrl += (newUrl != "" ? "&" : "") + string + 'x';
                        }
                        else if (!isNaN(value) || value.indexOf(',') >= 0 || value.indexOf('-') >= 0 || value.indexOf('%7C') >= 0 || value == "null" || value == "true" || value == "false") {
                            newUrl += (newUrl != "" ? "&" : "") + string.replace(value, 'x');
                        } else {
                            newUrl += (newUrl != "" ? "&" : "") + string;
                        }
                    });
                    //reemplazar /x/x/id
                    var folders = newUrl.split('/');
                    var newUrl2 = '';
                    $.each(folders, function (s, string) {
                        if (string != '') {
                            var pathParams = string.split('?');
                            if (!isNaN(pathParams[0])) {
                                newUrl2 += '/x';
                            } else {
                                newUrl2 += '/' + pathParams[0];
                            }
                            if (pathParams.length > 1) {
                                newUrl2 += '?' + pathParams[1];
                            }
                        }
                    });

                    url = newUrl2;
                    return url;
                },
                xhrTargets: function () {
                    $(document).ajaxSend(
                        function (evt, request, settings) {
                            //$.blockUI();
                            var registered = false;

                            $.each(RequestTargets, function (r, request) {
                                //console.log(self.UI().filterTarget(settings.url));
                                if (request.url == self.UI().filterTarget(settings.url)) {
                                    //console.log(request.url);
                                    registered = true;
                                    if (request.target == '') {
                                        //no mostrar status
                                    }
                                    else if (request.target == 'StatusBar') {
                                        if ($('#statusBar .message span[data-xhr="' + request.url + '"]').length == 0) {
                                            $('#statusBar .message').append('<span data-xhr="' + request.url + '">' + request.message + '...</span>');
                                            $('#statusBar .message').addClass('loading');
                                            $('#statusBar').fadeIn('fast');
                                        }
                                    } else {
                                        $('#' + request.target).before('<div class="loading ldg ldg-' + request.target + '"></div>');
                                        $('.ldg-' + request.target).width($('#' + request.target).width());
                                        $('.ldg-' + request.target).height($('#' + request.target).height());
                                    }
                                    if (request.block != '') {
                                        $.each(request.block.split(','), function (index, item) {
                                            if (item.indexOf('.') > -1) {
                                                $(item).attr('disabled', 'disabled');
                                            }
                                            else {
                                                $('#' + item).attr('disabled', 'disabled');
                                            }
                                        });
                                    }
                                }
                            });
                            if (!registered) {
                                //no registrado
                                //console.log('xhr: ' + self.UI().filterTarget(settings.url));
                            }
                        }).ajaxComplete(
                            //$.unblockUI
                            function (evt, request, settings) {
                                $.each(RequestTargets, function (r, request) {
                                    if (request.url == self.UI().filterTarget(settings.url)) {
                                        if (request.target == 'StatusBar') {
                                            $('#statusBar .message span[data-xhr="' + request.url + '"]').fadeOut('fast', function () {
                                                $(this).remove();
                                                if ($('#statusBar .message span').length == 0) {
                                                    $('#statusBar').removeClass('loading').fadeOut('fast');
                                                }
                                            });
                                        } else {
                                            $('.ldg-' + request.target).fadeOut('fast', function () { $(this).remove(); });
                                        }
                                        if (request.block != '') {
                                            $.each(request.block.split(','), function (index, item) {
                                                if (item.indexOf('.') > -1) {
                                                    $(item).removeAttr('disabled');
                                                }
                                                else {
                                                    $('#' + item).removeAttr('disabled');
                                                }
                                            });
                                        }
                                    }
                                });
                                //UI.exportToExcel();
                            }
                        ).ajaxStop(function () {
                            $('#statusBar .message').html('');
                            $('#statusBar').removeClass('loading').fadeOut('fast');
                        });
                }
            };
        },
        DateFormat: function () {
            return {
                toDateYYYYMMDD: function (value) {
                    //if (!value || value.indexOf("Date") == -1) return value;
                    //let re = /-?\d+/;
                    //let m = re.exec(value);
                    //let d = new Date(parseInt(m[0]));
                    return moment(value).format('YYYY-MM-DD');
                }
            };
        },
        Filters: function () {
            return {
                currency: function (value, moneyChar) {
                    if (value) {
                        let amount = value.toFixed(2);
                        var parts = amount.toString().split(".");
                        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                        return moneyChar + parts.join(".");
                    }
                    return value;
                },
                currencyToDecimal: function (value) {
                    if (value) {
                        return value.replace('$', '').replace(/,/g, '');
                    }
                    return value;
                },
                maskedPhone: function (value) {
                    if (value) {
                        let regex = /(\d+)/g;
                        let phoneNumbers = value.match(regex).join();
                        if (phoneNumbers != null && phoneNumbers.length >= 10) {
                            return phoneNumbers.substr(0, 3) + " ••• ••" + phoneNumbers.slice(-2);
                        }
                    }
                    return value;
                },
                maskedEmail: function (value) {
                    if (value) {
                        if (value.indexOf("@") >= 0) {
                            return value.substr(0, 2) + '•' + value.substr(value.indexOf("@"));
                        }
                    }
                    return value;
                },
                breakLines: function (value) {
                    if (value) {
                        value = value.replace(/\n/g, '<br />');
                    }
                    return value;
                },
                filterHtmlMail: function (value) {
                    if (value) {
                        if (value.indexOf('<body') >= 0) {
                            var pattern = /<body[^>]*>((.|[\n\r])*)<\/body>/im;
                            var array_matches = pattern.exec(value);
                            value = array_matches[1];
                            value = value.replace(/\n/g, ' ');
                        }
                    }
                    return value;
                },
                highlight: function (value, text) {
                    if (value && text) {
                        return value
                            .replace(text, '<mark>' + text + '</mark>')
                            .replace(text.charAt(0).toUpperCase() + text.slice(1), '<mark>' + text.charAt(0).toUpperCase() + text.slice(1) + '</mark>');
                    }
                    return value;
                }
            }
        },
        Forms: function () {
            self = this;
            return {
                showValidationSummary: function (frmID) {
                    var target = $("#" + frmID);
                    if (!$("#" + frmID).valid()) {
                        var errorsHolder = "#";
                        if (!$(target).is("form")) {
                            errorsHolder += $("#" + frmID).parents("form").attr("id");
                        } else {
                            errorsHolder += frmID;
                        }

                        errorsHolder += "_ErrorMessages";
                        var validationSummary = "There were some validation errors:";

                        //get validationMessages when @Html.ValidationSummary(false)
                        var selector = errorsHolder + " div.validation-summary-errors";
                        if ($(selector).length > 0) {
                            validationSummary += $(selector).html();
                        }
                        else { //get validationMessages when @Html.ValidationSummary(true)    
                            validationSummary += "<ul>";
                            $(errorsHolder + " > span").each(
                                function (index) {
                                    validationSummary += "<li>";
                                    validationSummary += self.UI().stripTags($(this).html());
                                    validationSummary += "</li>";
                                });
                            validationSummary += "</ul>"
                        }
                        validationSummary = validationSummary.replace(/<li><\/li>/g, '');
                        console.log(validationSummary);
                        $.alert({
                            title: 'Error Saving',
                            content: validationSummary,
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            type: 'red'
                        });
                    }
                }
            }
        },
        Guid: function () {
            let self = this;
            return {
                G4: function () {
                    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substr(1)
                },
                newGuid: function () {
                    return (self.Guid().G4() + self.Guid().G4() + "-" + self.Guid().G4() + "-" + self.Guid().G4() + "-" + self.Guid().G4() + "-" + self.Guid().G4() + self.Guid().G4() + self.Guid().G4());
                }
            }
        }
    }
}
