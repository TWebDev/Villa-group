/// <reference path="/Scripts/Utils.js" />
$.fn.clearSelect = function () {
    return this.each(function () {
        if (this.tagName == 'SELECT') {
            this.options.length = 0;
            $(this).text('');
        }
    });
}

$.fn.fillSelect = function (data, trigger) {
    this.clearSelect().each(function () {
        if (this.tagName == 'SELECT') {
            var dropdownList = this;
            $.each(data, function (index, optionData) {
                var option = new Option(optionData.Text, optionData.Value);

                if ($.browser.msie) {
                    dropdownList.add(option);
                }
                else {
                    dropdownList.add(option, null);
                }
            });
        }
    });
    if (trigger == undefined) {
        this.trigger('change');
    }
    //else if (trigger == true) {
    //    this.trigger('change');
    //}
    return this;
}

Number.prototype.formatMoney = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

$.fn.asFixed = function (cssObject, _class) {
    $selector = this;
    var _top = $($selector).offset().top;
    $(window).scroll(function () {
        if (_top < $(this).scrollTop()) {
            if (cssObject != null) {
                $($selector).css(cssObject);
            }
            else {
                $($selector).addClass(_class);
            }
        }
        else {
            $($selector).removeClass(_class).removeAttr('style');
        }
    });
}

$.fn.slowEach = function (interval, callback) {
    var array = this;
    if (!array.length) return;
    var i = 0;
    next();
    function next() {
        if (callback.call(array[i], i, array[i]) !== false)
            if (++i < array.length)
                setTimeout(next, interval);
    }
};
//$.fn.fillSelect = function (data) {
//    return this.clearSelect().each(function () {
//        if (this.tagName == 'SELECT') {
//            var dropdownList = this;
//            $.each(data, function (index, optionData) {
//                var option = new Option(optionData.Text, optionData.Value);

//                if ($.browser.msie) {
//                    dropdownList.add(option);
//                }
//                else {
//                    dropdownList.add(option, null);
//                }
//            });
//        }
//    }).trigger('change');
//}

$.fn.clearForm = function (changeSelect) {
    UI.ckeditorUpdateInstances($(this).attr('id'));
    return this.each(function (changeSelect) {
        if ($(this).is(':input')) {
            if (!$(this).is('[data-keep-value]')) // keep hidden fields with predefined values
            {
                var type = this.type, tag = this.tagName.toLowerCase();
                if (type == 'checkbox' || type == 'radio') {
                    this.checked = false;
                }
                else if (tag == 'select') {
                    this.selectedIndex = 0;
                    $(this).children('option:not(:hidden):first').attr('selected', true);
                    if (changeSelect != undefined) {
                        $(this).trigger('change');
                    }
                    //line added on 07/11/14
                    //$(this).trigger('change');
                    try {
                        $(this).multiselect('uncheckAll');
                    }
                    catch (ex) { }
                }
                else {
                    this.value = '';
                    //if ($(this).is('textarea')) {
                    //    if ($(this).css('display') == 'none') {
                    //        $(this).ckeditor();
                    //    }
                    //}
                }
            }
            if ($(this).hasClass('visible-on-update')) {
                $(this).parents('.editor-alignment').first().hide();
            }
        }
        else if ($(this).is('table[data-table]')) {
            var tableID = $(this).attr('id');
            var tabla = document.getElementById(tableID);

            if ($.fn.DataTable.fnIsDataTable(tabla)) {
                var oTable = $('#' + tableID).dataTable();
                oTable.fnClearTable();
            }
            $(this).find("tbody tr").remove()
        }
        else {
            $(':input,table[data-table]', this).not(':button').not(':submit').not(':reset').clearForm();
        }
    });
}

$.fn.printPage = function (url) {
    if ($('#sidebarTriangle').hasClass('right-triangle')) {
        $('#sidebarOpener').click();
    }
    if ($('#sideMenuTriangle').hasClass('left-triangle')) {
        $('#sideMenuOpener').click();
    }
    if (url == undefined) {
        setTimeout(function () {
            window.print();
        }, (60 * 5));
    }
    else {
        $.post('/MasterChart/GetCouponString', { url: url }, function (data) {
            $('#printableCoupon').html(data._coupon);

            eval(data._script);

            //setTimeout(function () {
            //    window.print();
            //}, (60 * 5));
        });

        //var strFrame = ('printable' + (new Date()).getTime()).toString();
        //var $frame = $('<iframe id="' + strFrame + '" name="' + strFrame + '" src="' + url + '">');
        //$frame.css({
        //    'width': '1px',
        //    'height': '1px',
        //    'position': 'absolute',
        //    'left': '-9999px'
        //});
        //$frame.appendTo($('body:first'));
        //setTimeout(function () {
        //    $frame.remove();
        //}, (60 * 1000));
    }
    setTimeout(function () { return false; }, (60, 1000));
}

$.validator.unobtrusive.adapters.add('requiredif', ['otherproperty', 'equalsto'], function (options) {
    options.rules['requiredif'] = options.params;
    options.messages['requiredif'] = options.message;
});

$.validator.unobtrusive.adapters.add('requiredifdistinct', ['otherproperty', 'distinctof'], function (options) {
    options.rules['requiredifdistinct'] = options.params;
    options.messages['requiredifdistinct'] = options.message;
});

$.validator.addMethod('requiredif', function (value, element, params) {
    if (params.otherproperty == '') {
        return true;
    }
    var otherElement = $('[name="' + params.otherproperty + '"]');
    var otherValue = $(otherElement).val();

    if (otherElement[0].nodeName.toLowerCase() == 'input') {
        if (this.checkable(otherElement)) {
            otherValue = $('[name="' + params.otherproperty + '"]:checked').val();
        }
    }
    
    if (otherValue == params.equalsto) {
        if (element.nodeName.toLowerCase() == 'select') {
            if (value == '0' || value == 'null') {
                return false;
            }
        }
        else {
            if (value == null || value == '') {
                return false;
            }
        }
    }
    //---
    else if (params.equalsto.indexOf(',') > -1) {
        if (params.equalsto.indexOf(otherValue) > -1) {
            //var array = JSON.parse('[' + params.equalsto + ']');
            if (value == null || value == '') {
                return false;
            }
        }
    }
    //---
    return true;
});

$.validator.addMethod('requiredifdistinct', function (value, element, params) {
    var otherElement = $('[name="' + params.otherproperty + '"]');
    var otherValue = $(otherElement).val();

    //if (otherElement[0].nodeName.toLowerCase() == 'input') {
    //    if (this.checkable(otherElement)) {
    //        otherValue = $('[name="' + params.otherproperty + '"]:checked').val();
    //    }
    //}
    //console.log(params);
    //if (params.distinctof == null) {
    //if (otherValue == params.equalsto) {
    //    if (value == null || value == '' || value == '0' || value == 'null') {
    //        return false;
    //    }
    //}
    //}
    //else {
    if (otherValue != params.distinctof) {
        if (value == null || value == '' || value == '0' || value == 'null') {
            return false;
        }
    }
    //}
    return true;
});

/// Provides functionality to manipulate User Interface. If it modifies in any way the UI, you'll find it here.
var UI = function () {
    var oTable;
    var messageBoxInterval;
    var messageBoxIsOpen = false;
    var selectedTerminals = '';
    var currentTerminals = '';
    var selectedWorkGroup = '';
    var selectedRole = '';
    var currentWorkGroup = '';
    var currentRole = '';
    var dontBlock = false;
    var searchTextInColumns_hilightTimers = null;
    var searchTextInColumns_previousHilightedElements = $();
    var loadFlag = true;
    var pendingRequest = null;
    var windowFocus = true;
    var DependantFields = [];

    //console.log('c: ' + localStorage.Eplat_Cache);
    if (localStorage.Eplat_Cache == undefined) {
        localStorage.Eplat_Cache = '[]';
    }
    //console.log('d: ' + localStorage.Eplat_Cache);
    var localCache = {
        /**
         * timeout for cache in millis /8 horas
         * @type {number}
         */
        timeout: 28800000,
        /** 
         * @type {{_: number, data: {}}}
         **/
        remove: function (url) {
            var data = eval('(' + localStorage.Eplat_Cache + ')');
            //console.log('a: ' + data);
            var newData = [];
            $.each(data, function (i, item) {
                if (item.url != url) {
                    newData.push(item);
                }
            });
            localStorage.Eplat_Cache = $.toJSON(newData);
        },
        exist: function (url) {
            var data = eval('(' + localStorage.Eplat_Cache + ')');
            //console.log('b: ' + data);
            var exist = false;
            $.each(data, function (i, item) {
                if (item.url == url && ((new Date().getTime() - item._) < localCache.timeout)) {
                    exist = true;
                }
            });
            //console.log('exist ' + url + ' ' + exist);
            return exist;
        },
        get: function (url) {
            //console.log('get' + url);
            var data = eval('(' + localStorage.Eplat_Cache + ')');
            var cacheObj;
            $.each(data, function (i, item) {
                if (item.url == url) {
                    cacheObj = item.data;
                    return [false];
                }
            });
            //console.log('cacheObj: ' + $.toJSON(cacheObj));
            return cacheObj;
        },
        set: function (url, cachedData, callback) {
            //console.log('set' + url);
            //console.log('cachedData: ' + cachedData);
            localCache.remove(url);
            var data = eval('(' + localStorage.Eplat_Cache + ')');
            cacheItem = {
                url: url,
                _: new Date().getTime(),
                data: cachedData
            };
            data.push(cacheItem);
            localStorage.Eplat_Cache = $.toJSON(data);

            if ($.isFunction(callback)) callback(cachedData);
        }
    };

    var init = function () {
        /// <summary>
        /// Initializes User Interface elements.
        /// </summary>
        UI.unselectRow();
        UI.legendClickBind();
        UI.expandFirstFieldset();
        UI.disableFieldWithClass();
        UI.alertDoubleClick();
        /*begin GERARDO 150326 - agregué esta linea para que detecte el cambio desde el inicio de la carga y dispare las listas consecuentes*/
        UI.dependentLists();
        UI.updateListsOnTerminalsChange();
        /*end GERARDO*/
        function filterTarget(url) {
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
        }

        $(window).on('focus', function () {
            UI.windowFocus = true;
            UI.Notifications.workingOn();
        }).on('blur', function () {
            UI.windowFocus = false;
            UI.Notifications.workingOn();
        });

        $('#spnFooterClearCache').on('click', function () {
            localStorage.Eplat_Cache = '[]';
            UI.messageBox(1, "Cache Clean");
        });

        //$.ajaxPrefilter(function (options, originalOptions, jqXHR) {
        //    if (originalOptions.url.indexOf('signalr') == -1 && (originalOptions.type == "GET" || originalOptions.type == "get")) {
        //        var complete = originalOptions.complete || $.noop,
        //        success = originalOptions.success || $.noop,
        //            url = options.url;
        //        options.cache = false;
        //        options.beforeSend = function () {
        //            if (options.url.indexOf('_=') > -1) {
        //                url = options.url.substr(0, options.url.indexOf('_='));
        //            } else {
        //                url = options.url;
        //            }
        //            if (localCache.exist(url)) {
        //                var cacheObj = localCache.get(url);
        //                success(eval('(' + cacheObj.responseText + ')'));                      
        //                complete(cacheObj);
        //                return false;
        //            }
        //            return true;
        //        };
        //        options.complete = function (data, textStatus) {
        //            if (options.url.indexOf('_=') > -1) {
        //                url = options.url.substr(0, options.url.indexOf('_='));
        //            } else {
        //                url = options.url;
        //            }
        //            ////para revisar si un request cumple con requisitos de cache
        //            //if (url.indexOf('GetLeadPurchases') > -1) {
        //            //    console.log(url);
        //            //    console.log(textStatus);
        //            //    console.log(originalOptions.cache);
        //            //    console.log(options.dataType);
        //            //}
        //            if (textStatus == "success" && originalOptions.cache != false && (options.dataType == "json" || options.dataType == undefined)) {
        //                localCache.set(url, data, complete);
        //            }
        //            complete(data, textStatus);
        //        };
        //    }
        //});
        //$.ajaxPrefilter(function (options, originalOptions, jqXHR) {
        //    if (originalOptions.data != undefined && originalOptions.data.itemID != undefined && (originalOptions.data.itemID == 0  || originalOptions.data.itemID == '')) {
        //        options.beforeSend = function () {
        //            return false;
        //            //jqXHR.abort();
        //        }
        //    }
        //});
        $(document).ajaxSend(
            function (evt, request, settings) {
                if (!UI.dontBlock) {
                    //$.blockUI();
                    var registered = false;

                    $.each(RequestTargets, function (r, request) {
                        //console.log(filterTarget(settings.url));
                        //if (request.url == filterTarget(settings.url)) {
                            if (request.url.toLowerCase() == filterTarget(settings.url).toLowerCase()) {
                            registered = true;
                            if (request.target == '') {
                                //no mostrar status
                            }
                            else if (request.target == 'StatusBar') {
                                if ($('#StatusBar .message span[data-xhr="' + request.url + '"]').length == 0) {
                                    $('#StatusBar .message').append('<span data-xhr="' + request.url + '">' + request.message + '...</span>');
                                    $('#StatusBar .message').addClass('loading');
                                    $('#StatusBar').fadeIn('fast');
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
                        //console.log('xhr: ' + filterTarget(settings.url));
                    }
                }
            }).ajaxComplete(
            //$.unblockUI
            function (evt, request, settings) {
                $.each(RequestTargets, function (r, request) {
                    //if (request.url == filterTarget(settings.url)) {
                    if (request.url.toLowerCase() == filterTarget(settings.url).toLowerCase()) {
                        if (request.target == 'StatusBar') {
                            if (UI.pendingRequest == undefined || UI.pendingRequest.readyState == 4) {
                                $('#StatusBar .message span[data-xhr="' + request.url + '"]').fadeOut('fast', function () {
                                    $(this).remove();
                                    if ($('#StatusBar .message span').length == 0) {
                                        $('#StatusBar').removeClass('loading').fadeOut('fast');
                                    }
                                });
                            }
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
                UI.exportToExcel();
            }
        ).ajaxStop(function () {
            if (UI.pendingRequest == undefined || UI.pendingRequest.readyState == 4) {
                $('#StatusBar .message').html('');
                $('#StatusBar').removeClass('loading').fadeOut('fast');
            }
        });
        // ajaxStart ajaxStop ajaxComplete ajaxError ajaxSuccess

        UI.selectedTerminals = localStorage.Eplat_SelectedTerminals;
        UI.selectedWorkGroup = localStorage.Eplat_SelectedWorkGroupID;
        UI.selectedRole = localStorage.Eplat_SelectedRole;
        //UI.menuActions();

        if (window.location.pathname.indexOf('Logon') == -1) {
            UI.loadWorkGroups();
            UI.loadTerminals();
        }
        //detect clicks outside menu
        $(document).click(function (e) {
            if ($(e.target).parents('#divAvailableTerminals').length == 0) {
                UI.terminalsClose();
            }
            if ($(e.target).parents('#headerWorkGroup').length == 0) {
                UI.workGroupsClose();
            }
            UI.rolesClose();
            $('.submenu').hide();
            //UI.submenusClose();
        });

        //terminals selector actions
        $('#divSelectedTerminals').on('click', function () {
            if ($('#divAvailableTerminals').is(':visible')) {
                $('#divAvailableTerminals').css('display', 'none');
            } else {
                //$('#divAvailableTerminals').css('display', 'block');
                $('#divAvailableTerminals').css('display', 'block');
            }
        });
        $('#divSelectedTerminals').on('mouseover', function () {
            UI.terminalsOpen();
        });
        $('#divAvailableTerminals').on('mouseout', function (e) {
            if ($(e.target).is('div')) {
                UI.terminalsClose();
            }
        });

        //workgroup selector actions
        $('#divSelectedWorkGroup').on('click', function () {
            if ($('#divAvailableWorkGroups').is(':visible')) {
                $('#divAvailableWorkGroups').css('display', 'none');
            } else {
                $('#divAvailableWorkGroups').css('display', 'block');
            }
        });
        $('#divSelectedWorkGroup').on('mouseover', function () {
            UI.workGroupsOpen();
        });
        $('#divAvailableWorkGroups').on('mouseout', function (e) {
            if ($(e.target).is('div')) {
                UI.workGroupsClose();
            }
        });

        $('#divAvailableWorkGroups').click(UI.selectWorkGroup);

        //roles selector actions
        $('#divSelectedRole').hover(function () {
            UI.rolesOpen();
        }, function () { });
        $('#divAvailableRoles').hover(function () { }, function () {
            UI.rolesClose();
        });

        //$('#divAvailableRoles input[type=radio]').click(UI.selectRole);
        $('#divAvailableRoles').click(UI.selectRole);

        //sidebar
        $(window).resize(function () {
            //adjustSideBar();
            //UI.adjustLegends();
            //UI.adjustMenuOptions();
            //$('#customerBanner').width($('#main').width() + 30);
        });

        //adjustSideBar();

        //agregar evento del opener
        $('#sidebarOpener').on('click', function () {
            if ($('#sidebar').css('margin-right') == '0px') {
                $('#sidebar').animate({
                    marginRight: '-205px'
                    //marginRight: '-240px'
                }, 'fast');
                $('#sidebarTriangle').removeClass('right-triangle');
                $('#sidebarTriangle').addClass('left-triangle');
                //$('#customerBanner').animate({
                //    width:$('#main').width() +245
                //}, 'fast', function () {
                //    $('#main').animate({
                //        marginRight: '30px'
                //    }, 'fast')
                //});

                $('#main').animate({
                    marginRight: '30px'
                });
                $('#customerBanner').animate({
                    width: $('#main').width() + 245
                });
                //$('#main').animate({
                //    marginRight: '30px'
                //}, 'fast', function () {
                //    //UI.adjustLegends();
                //    //$('#customerBanner').animate({ width: $('#main').width()+30 }, 'fast');
                //    $('#customerBanner').width($('#main').width() + 30);
                //});
                localStorage.Eplat_SidebarOpen = '0';
            } else {
                $('#sidebar').animate({
                    marginRight: '0px'
                }, 'fast');
                $('#sidebarTriangle').removeClass('left-triangle');
                $('#sidebarTriangle').addClass('right-triangle');
                //$('#customerBanner').animate({
                //    width: $('#main').width() - 170
                //}, 'fast', function () {
                //    $('#main').animate({
                //        marginRight: '240px'
                //    }, 'fast')
                //});

                $('#main').animate({
                    marginRight: '240px'
                }, 'fast');
                $('#customerBanner').animate({
                    width: $('#main').width() - 170
                });
                //$('#main').animate({
                //    marginRight: '240px'
                //}, 'fast', function () {
                //    //UI.adjustLegends();
                //    $('#customerBanner').width($('#main').width()+30);
                //    //$('#customerBanner').animate({ width: $('#main').width()+30 }, 'fast');
                //});
                localStorage.Eplat_SidebarOpen = '1';
            }
        });

        //revisar el estatus inicial 
        //if (localStorage.Eplat_SidebarOpen == '0') {
        //    $('#sidebarOpener').trigger('click');
        //}

        $('#pleca-tickets').not('>#btnSupportTickets').on('click', function () {
            if ($('#content-tickets').height() == 0) {
                $('#content-notifications').animate({
                    height: '0px'
                }, 'fast');
                $('#content-users').animate({
                    height: '0px'
                }, 'fast');
                $('#content-tickets').animate({
                    height: parseInt($(window).height() - 152) + 'px'
                }, 'fast');
            } else {
                var height = (parseInt($(window).height() - 152) / 2);
                $('#content-notifications').animate({
                    height: height + 'px'
                }, 'fast');
                $('#content-users').animate({
                    height: height + 'px'
                }, 'fast');
                $('#content-tickets').animate({
                    height: '0px'
                }, 'fast');
                $('#pnlConversation').animate({
                    height: (height - 83) + 'px'
                }, 'fast');
                $('#pnlUsers').animate({
                    height: height + 'px'
                }, 'fast');
            }
        });

        $('.numeric-field').each(function () {
            $(this).numeric();
        });

        //notifications button
        if (localStorage.Eplat_SelectedRole == '87e4708c-14fb-426b-a69b-05f28fc5dcfc') {
            $('.notification-add').show();
            $('.notification-add').on('click', function () {
                $('#txtNotification').val('');
                $('#chkNotificationImportant').prop('checked', false);
                $('#pnlNewNotification').slideToggle('fast');
            });
        } else {
            $('.notification-add').hide();
        }

        $(document).ready(function () {
            //UI.adjustLegends();
            UI.verticalMenu();
        });
        function adjustSideBar() {
            if ($('#content-tickets').height() == 0) {
                var height = (parseInt($(window).height() - 152) / 2);
                $('#content-notifications').height(height + 'px');
                $('#content-users').height(height + 'px');
                $('#pnlConversation').height((height - 83) + 'px');
                $('#pnlUsers').height(height + 'px');
            } else {
                $('#content-tickets').height(parseInt($(window).height() - 152) + 'px');
            }
        }
        //**********collapsable fieldsets
        //$('legend').on('click', function () {
        //    //make sure the element has its visibility properly set again.
        //    if ($(this).parent('fieldset').children('div').hasClass('fakeDisplayNone')) {
        //        $(this).parent('fieldset').children('div').css("display", "none");
        //        $(this).parent('fieldset').children('div').removeClass('fakeDisplayNone');
        //    }

        //    if ($(this).parent('fieldset').children('div').is(':visible')) {
        //        UI.collapseFieldset($(this).parent('fieldset').attr('id'));
        //        //$(this).parent('fieldset').find('fieldset').each(function () {
        //        //    if ($(this).children('div').is(':visible')) {
        //        //        UI.collapseFieldset($(this).attr('id'));
        //        //    }
        //        //});
        //    } else {
        //        UI.expandFieldset($(this).parent('fieldset').attr('id'));
        //        //$(this).parent('fieldset').siblings('fieldset').each(function () {
        //        //    //collapse siblings and their children
        //        //    if ($(this).children('div').is(':visible')) {
        //        //        UI.collapseFieldset($(this).attr('id'));
        //        //    }
        //        //});
        //    }
        //});

        //$('.fieldset-expander').unbind('click').bind('click', function (e) {
        //    var event = $.Event('keydown');
        //    event.keyCode = 27;
        //    source = $(e.target).attr('id');
        //    $(document).trigger(event, source);
        //    UI.resetValidation();
        //});

        //**********end collapsable fieldsets

        $('.terminals-dependent').on('click', function () {
            $(document).find('.selected-row').each(function () {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            });
        });

        //aplicar formatos
        UI.applyTextFormat();
        //$('input[data-format=phone]').on('keyup', function () {
        //    var start = this.selectionStart,
        //    end = this.selectionEnd;
        //    var chars = $(this).val().replace(/\s+/g, '').split('');
        //    var value = "";
        //    //get number
        //    for (var i = 0; i < chars.length; i++) {
        //        if (!isNaN(chars[i])) {
        //            value += chars[i];
        //        }
        //    }
        //    //format number
        //    if (value.length >= 10) {
        //        var digits = value.split('');
        //        value = '';
        //        for (var x = 0; x < digits.length; x++) {
        //            if (x == 0) {
        //                value += '(';
        //            } else if (x == 3) {
        //                value += ') ';
        //            } else if (x == 6) {
        //                value += ' ';
        //            }
        //            value += digits[x];
        //        }
        //        $(this).val(value);
        //    }

        //    this.setSelectionRange(start, end);
        //});

        //$('input[data-format=card-number]').on('keyup', function () {
        //    var start = this.selectionStart,
        //    end = this.selectionEnd;
        //    var chars = $(this).val().replace(/\s+/g, '').split('');
        //    var value = "";
        //    //get number
        //    for (var i = 0; i < chars.length; i++) {
        //        if (!isNaN(chars[i])) {
        //            value += chars[i];
        //        }
        //    }
        //    //format number
        //    if (value.length >= 4) {
        //        var digits = value.split('');
        //        value = '';
        //        for (var x = 0; x < digits.length; x++) {
        //            if (x % 4 == 0 && x > 0 && x < 16) {
        //                value += ' ';
        //            }
        //            if (x < 16) {
        //                value += digits[x];
        //            }
        //        }
        //        $(this).val(value);
        //    }
        //    this.setSelectionRange(start, end);
        //});

        //$('input[type=text]').not('[data-format=free-case]').on('keyup', function (e) {
        //    var start = this.selectionStart,
        //    end = this.selectionEnd;

        //    var string1 = $(this).val().replace(/\w\S*/g, function (s) { return s.charAt(0).toUpperCase() + s.substr(1).toLowerCase(); });

        //    var stringArr = string1.split(" ");
        //    var string2 = '';
        //    for (var i = 0; i < stringArr.length; i++) {
        //        if ($.inArray(stringArr[i], pronounsArr) !== -1 && i != 0) {
        //            string2 += (i != 0 ? ' ' : '') + stringArr[i].toLowerCase();
        //        } else if (stringArr[i].indexOf('.') > 0) {
        //            string2 += (i != 0 ? ' ' : '') + stringArr[i].toUpperCase();
        //        } else {
        //            string2 += (i != 0 ? ' ' : '') + stringArr[i];
        //        }
        //    }

        //    $(this).val(string2);
        //    this.setSelectionRange(start, end);
        //});

        //var pronounsArr = [ "Me", "We", "Us", "You", "She", "Her", "He", "Him", "It", "They", "Them", "This", "That", "These", "Those", "Which", "Who", "Whom", "Whose", "Whichever", "Whoever", "Whomever", "Anybody", "Anyone", "Anything", "Each", "Either", "Everybody", "Everyone", "Everything", "Neither", "Nobody", "Somebody", "Someone", "Something", "Myself", "Ourselves", "Yourself", "Yourselves", "Himself", "Herself", "Itself", "Themselves", "My", "Your", "His", "Its", "Our", "Their", "Mine", "Yours", "Ours", "Yours", "Theirs", "The", "Aboard", "About", "Above", "Across", "After", "Against", "Along", "Amid", "Among", "Anti", "Around", "As", "At", "Before", "Behind", "Below", "Beneath", "Beside", "Besides", "Between", "Beyond", "But", "By", "Concerning", "Considering", "Despite", "Down", "During", "Except", "Excepting", "Excluding", "Following", "For", "From", "In", "Inside", "Into", "Like", "Minus", "Near", "Of", "Off", "On", "Onto", "Opposite", "Outside", "Over", "Past", "Per", "Plus", "Regarding", "Since", "Than", "Through", "Thru", "To", "Toward", "Towards", "Under", "Underneath", "Unlike", "Until", "Up", "Upon", "Versus", "Via", "With", "Within", "Without", "Yo", "Nosotros", "Nos", "Tu", "Ella", "Su", "El", "Él", "Eso", "Ellos", "Esto", "Eso", "Estos", "Esos", "Cual", "Quien", "Quién", "Cuál", "Cualquier", "Cada", "Si", "Uno", "Un", "Una", "Nadie", "Alguien", "Alguno", "Algo", "Mismo", "Mismos", "Mi", "Lo", "A", "Ante", "Bajo", "Cabe", "Con", "Contra", "De", "Desde", "En", "Entre", "Hacia", "Hasta", "Para", "Por", "Según", "Segun", "Sin", "So", "Sobre", "Tras", "Del", "La", "Los", "Las", "Se", "Es", "And", "Y", "Or", "O", "U", "E", "A", "Al" ];

        //$('input[data-format=capital-case]').on('keyup', function (e) {
        //    var start = this.selectionStart,
        //    end = this.selectionEnd;
        //    $(this).val($(this).val().toLowerCase());
        //    $(this).val($(this).val().charAt(0).toUpperCase() + $(this).val().slice(1));
        //    this.setSelectionRange(start, end);
        //});

        //$('input[data-format=upper-case]').on('keyup', function (e) {
        //    var start = this.selectionStart,
        //    end = this.selectionEnd;

        //    $(this).val($(this).val().toUpperCase());

        //    this.setSelectionRange(start, end);
        //});

        //$('input[data-format=lower-case]').on('keyup', function (e) {
        //    var start = this.selectionStart,
        //    end = this.selectionEnd;

        //    $(this).val($(this).val().toLowerCase());

        //    this.setSelectionRange(start, end);
        //});

        UI.applyFormat('currency');

        $('[data-showon]').hide();
        $('[data-showon]').each(function (i, item) {
            $('#' + $(this).attr('data-showon')).off('click valueChange').on('click valueChange', function () {
                if ($(this).is(':checked')) {
                    $('[data-showon=' + $(this).attr('id') + ']').show();
                } else {
                    $('[data-showon=' + $(this).attr('id') + ']').hide();
                }
            });
        });

        //nav height on mobiles
        if ($(window).width() <= 767) {
            $('nav').height($(window).height());
        }

        $('#btnUpdateCoupons').on('click', function () {
            $.ajax({
                url: '/Home/Index3',
                cache: false,
                type: 'POST',
                data: { idInicio: $('#txtIDInicio').val(), idFin: $('#txtIDFin').val() },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        });

        $('.keydown-enter').on('keydown', function (e) {
            if (e.keyCode == 13) {
                $('#' + $(e.target).data('keydown-enter-target')).click();
            }
        });
    }
    //********** end init function
    //var workgroupDependentListActions = function () {
    //    $('.workgroup-dependent-list').each(function () {
    //        var id = $(this).attr('id');
    //        var route = $(this).attr('data-route');
    //        var parameter = $(this).attr('data-route-parameter');
    //        $.getJSON(route, { path: parameter, id: UI.selectedWorkGroup }, function (data) {
    //            $('#'+ id).fillSelect(data);
    //        });
    //    });
    //}

    //var terminalDependentListActions = function () {
    //    $('.terminal-dependent-list').each(function () {
    //        var id = $(this).attr('id');
    //        var route = $(this).attr('data-route');
    //        var parameter = $(this).attr('data-route-parameter');
    //        $.getJSON(route, { itemID: 0, path: parameter }, function (data) {
    //        //$.getJSON(route, { path: parameter, id: UI.selectedTerminals }, function (data) {
    //            $('#' + id).fillSelect(data);
    //        });
    //    });
    //}

    var applyDatePicker = function () {
        $('input[data-uses-date-picker="true"]').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onClose: function (dateText, inst) {
                var id = $(this).attr('id');
                var altId = '';
                if (id.indexOf('_I_') > 0) {
                    //init of range
                    altId = id.replace('_I_', '_F_');
                    if ($('#' + altId).val() != '') {
                        if (dateText != '') {
                            var fromDate = $(this).datepicker('getDate');
                            var toDate = $('#' + altId).datepicker('getDate');
                            if (fromDate > toDate) {
                                $('#' + altId).datepicker('setDate', fromDate);
                            }
                        }
                        else {
                            $('#' + altId).val(dateText);
                        }
                    }
                    else {
                        $('#' + altId).val(dateText);
                    }
                }
                else {
                    //end of range
                    altId = id.replace('_F_', '_I_');
                    if ($('#' + altId).val() != '') {
                        var fromDate = $('#' + altId).datepicker('getDate');
                        var toDate = $(this).datepicker('getDate');
                        if (fromDate > toDate) {
                            $('#' + altId).datepicker('setDate', toDate);
                        }
                    }
                    else {
                        $('#' + altId).val(dateText);
                    }
                }
            },
            onSelect: function (selectedDate) {
                var id = $(this).attr('id');
                var altId = '';
                if (id.indexOf('_I_') > 0) {
                    //init of range
                    altId = id.replace('_I_', '_F_');
                    if ($('#' + altId).val() != '') {
                        $('#' + altId).datepicker('setDate', $('#' + altId).datepicker('getDate'));
                    }
                    //$('#' + altId).datepicker('option', 'minDate', $(this).datepicker('getDate'));
                }
                else {
                    //end of range
                    altId = id.replace('_F_', '_I_');
                    if ($('#' + altId).val() != '') {
                        $('#' + altId).datepicker('setDate', $('#' + altId).datepicker('getDate'));
                    }
                    //$('#' + altId).datepicker('option', 'maxDate', $(this).datepicker('getDate'));
                }
            }
        });
        $('*[data-uses-datetime-picker]').each(function () {
            $(this).datetimepicker({
                dateFormat: 'yy-mm-dd',
                timeFormat: 'HH:mm',
                timeOnly: false,
                stepMinute: 5
            });
        });

        $("[data-uses-time-picker]").each(function () {
            $(this).datetimepicker({
                timeFormat: 'HH:mm',
                timeOnly: true,
                stepMinute: 5
            });
        });

        $('[data-uses-cc-date-picker]').each(function () {
            $(this).datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                    $(".ui-datepicker-calendar").hide();
                },
                beforeShow: function (dateText, inst) { $('.ui-datepicker-calendar').hide(); },
                onSelect: function (dateText, inst) { $(".ui-datepicker-calendar").hide(); }
            });
        });

        $('[data-uses-cc-date-picker]').focus(function () {
            $(".ui-datepicker-calendar").hide();
        });
    }

    var applyMultiselect = function () {
        $('select[multiple="multiple"]').multiselect({
            noneSelectedText: "--All--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();
    }

    var applyTextFormat = function () {
        var pronounsArr = ["Me", "We", "Us", "You", "She", "Her", "He", "Him", "It", "They", "Them", "This", "That", "These", "Those", "Which", "Who", "Whom", "Whose", "Whichever", "Whoever", "Whomever", "Anybody", "Anyone", "Anything", "Each", "Either", "Everybody", "Everyone", "Everything", "Neither", "Nobody", "Somebody", "Someone", "Something", "Myself", "Ourselves", "Yourself", "Yourselves", "Himself", "Herself", "Itself", "Themselves", "My", "Your", "His", "Its", "Our", "Their", "Mine", "Yours", "Ours", "Yours", "Theirs", "The", "Aboard", "About", "Above", "Across", "After", "Against", "Along", "Amid", "Among", "Anti", "Around", "As", "At", "Before", "Behind", "Below", "Beneath", "Beside", "Besides", "Between", "Beyond", "But", "By", "Concerning", "Considering", "Despite", "Down", "During", "Except", "Excepting", "Excluding", "Following", "For", "From", "In", "Inside", "Into", "Like", "Minus", "Near", "Of", "Off", "On", "Onto", "Opposite", "Outside", "Over", "Past", "Per", "Plus", "Regarding", "Since", "Than", "Through", "Thru", "To", "Toward", "Towards", "Under", "Underneath", "Unlike", "Until", "Up", "Upon", "Versus", "Via", "With", "Within", "Without", "Yo", "Nosotros", "Nos", "Tu", "Ella", "Su", "El", "Él", "Eso", "Ellos", "Esto", "Eso", "Estos", "Esos", "Cual", "Quien", "Quién", "Cuál", "Cualquier", "Cada", "Si", "Uno", "Un", "Una", "Nadie", "Alguien", "Alguno", "Algo", "Mismo", "Mismos", "Mi", "Lo", "A", "Ante", "Bajo", "Cabe", "Con", "Contra", "De", "Desde", "En", "Entre", "Hacia", "Hasta", "Para", "Por", "Según", "Segun", "Sin", "So", "Sobre", "Tras", "Del", "La", "Los", "Las", "Se", "Es", "And", "Y", "Or", "O", "U", "E", "A", "Al", "Que"];

        $('input[data-format=number]').on('keydown', function (e) {
            //if (e.keyCode < 48 || (e.keyCode > 57 && e.keyCode < 96) || e.keyCode > 105) {
            //    e.preventDefault();
            //}
            if ((isNaN(Number(e.key)) || e.keyCode == 32) && (e.keyCode != 8 && e.keyCode != 46) && !(e.keyCode >= 37 && e.keyCode <= 40)){
                e.preventDefault();
            }
        });

        $('input[data-format=phone]').on('keyup', function () {
            var start = this.selectionStart,
            end = this.selectionEnd;
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

            this.setSelectionRange(start, end);
        });

        $('input[data-format=card-number]').on('keyup', function () {
            var start = this.selectionStart,
            end = this.selectionEnd;
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
            this.setSelectionRange(start, end);
        });

        $('input[type=text]').not('[data-format=free-case]').on('keyup', function (e) {
            var start = this.selectionStart,
            end = this.selectionEnd;

            var string1 = $(this).val().replace(/\w\S*/g, function (s) { return s.charAt(0).toUpperCase() + s.substr(1).toLowerCase(); });

            var stringArr = string1.split(" ");
            var string2 = '';
            for (var i = 0; i < stringArr.length; i++) {
                if ($.inArray(stringArr[i], pronounsArr) !== -1 && i != 0) {
                    string2 += (i != 0 ? ' ' : '') + stringArr[i].toLowerCase();
                } else if (stringArr[i].indexOf('.') > 0) {
                    string2 += (i != 0 ? ' ' : '') + stringArr[i].toUpperCase();
                } else {
                    string2 += (i != 0 ? ' ' : '') + stringArr[i];
                }
            }

            $(this).val(string2);
            this.setSelectionRange(start, end);
        });

        $('input[data-format=capital-case]').on('keyup', function (e) {
            var start = this.selectionStart,
            end = this.selectionEnd;
            $(this).val($(this).val().toLowerCase());
            $(this).val($(this).val().charAt(0).toUpperCase() + $(this).val().slice(1));
            this.setSelectionRange(start, end);
        });

        $('input[data-format=upper-case]').on('keyup', function (e) {
            var start = this.selectionStart,
            end = this.selectionEnd;

            $(this).val($(this).val().toUpperCase());

            this.setSelectionRange(start, end);
        });

        $('input[data-format=lower-case]').on('keyup', function (e) {
            var start = this.selectionStart,
            end = this.selectionEnd;

            $(this).val($(this).val().toLowerCase());

            this.setSelectionRange(start, end);
        });
    }

    var dependentLists = function () {
        //console.log('declaracion del onchange');
        $('.onchange').on('change', function () {
            $(this).each(function () {
                if (!isNaN($(this).val()) && $(this).val() != null) {
                    var route = $(this).attr('data-route');
                    var parameter = $(this).attr('data-onchange-route-parameter').split(',');
                    var dependent = $(this).attr('data-dependent-list').split(',');
                    var id = null;
                    if ($(this).val() instanceof Array) {
                        id = $(this).val()[0];
                    } else {
                        id = $(this).val();
                    }

                    for (var i = 0; i < parameter.length; i++) {
                        if ($('#' + dependent[i]).is('select')) {
                            $('#' + dependent[i]).clearSelect();
                        } else if ($('#' + dependent[i]).is("input") && $('#' + dependent[i]).prop('type') == 'text') {
                            $('#' + dependent[i]).attr('placeholder', '');
                        }
                        //console.log('onchange ' + route);
                        $.getJSON(route, { itemType: parameter[i], itemID: id },
                            (function (thisi) {
                                return function (data) {
                                    if ($('#' + dependent[thisi]).is('select')) {
                                        $('#' + dependent[thisi]).fillSelect(data);
                                        if ($('#' + dependent[thisi]).prop('multiple')) {
                                            $('#' + dependent[thisi]).multiselect('refresh');
                                        }
                                        //$('#' + dependent[thisi]).trigger('loaded');
                                    } else if ($('#' + dependent[thisi]).is("input") && $('#' + dependent[thisi]).prop('type') == 'text') {
                                        $('#' + dependent[thisi]).attr('placeholder', data[0].Text);
                                    }
                                };
                            }(i))
                        );
                    }

                    if ($(this).attr('data-onchange-validID-show') != undefined) {
                        var showIDs = $(this).attr('data-onchange-validID-show').split(',');
                        for (var i = 0; i < showIDs.length; i++) {
                            $('#' + showIDs[i]).show();
                        }
                    }
                    if ($(this).attr('data-onchange-invalidID-show') != undefined) {
                        var hideIDs = $(this).attr('data-onchange-invalidID-show').split(',');
                        for (var i = 0; i < hideIDs.length; i++) {
                            $('#' + hideIDs[i]).val('').hide();
                        }
                    }
                } else {
                    if ($(this).attr('data-onchange-validID-show') != undefined) {
                        var hideIDs = $(this).attr('data-onchange-validID-show').split(',');
                        for (var i = 0; i < hideIDs.length; i++) {
                            $('#' + hideIDs[i]).val('').hide();
                        }
                    }
                    if ($(this).attr('data-onchange-invalidID-show') != undefined) {
                        var showIDs = $(this).attr('data-onchange-invalidID-show').split(',');

                        for (var i = 0; i < showIDs.length; i++) {
                            $('#' + showIDs[i]).show();
                        }
                    }
                }
            });
        });
    }

    var applyFormat = function (format, id) {
        //mike
        //if (id != undefined && document.getElementById(id) == null)
        //    return '';
        var _target = '';
        if (id != undefined) {
            $.each(id.split(','), function (index, item) {
                if (document.getElementById(id) == null)
                    return '';
                switch ($('#' + item).get(0).tagName.toLowerCase()) {
                    case 'table': {
                        if ($.fn.DataTable.fnIsDataTable(document.getElementById(item))) {
                            _target = $('#' + item).dataTable();
                            _target = _target.$('.format-currency');//.$ enters to the tbody
                        }
                        else {
                            _target = $('#' + item + ' tbody .format-currency');
                        }
                        break;
                    }
                }
                if (format == "currency") {
                    //$('[data-format=currency]').each(function (i, item) {
                    $(_target).each(function (i, item) {//mike
                        if ($(this).find('.money-char').length == 0) {
                            var currency = '';
                            var _text = $(this).text().trim();
                            if ($(this).text().trim().indexOf(" ") > 0) {
                                currency = $(this).text().trim().substr($(this).text().trim().indexOf(" ") + 1, 3);
                            }
                            var amount = parseFloat($(this).text().trim());
                            $(this).text(parseFloat($(this).text().trim()).formatMoney(2));
                            $(this).html('<span class="' + (currency != '' ? 'money-currency' : 'money') + (amount == 0 ? ' expired' : (amount < 0 ? ' mb-warning' : '')) + '"><span class="money-char">$</span><span class="money-amount">' + $(this).text().trim() + ' ' + currency + '</span></span>');
                        }
                    });
                }
            });
        }
        if (format == "percentage") {
            $('[data-format=percentage]').each(function (i, item) {
                if ($(this).find('.percentage-char').length == 0) {
                    var amount = parseFloat($(this).text());
                    $(this).text(parseFloat($(this).text()).formatMoney(2));
                    $(this).html('<span class="' + (amount == 0 ? 'expired' : '') + '"><span class="percentage-amount">' + $(this).text() + '</span><span class="percentage-char">%</span></span>');
                }
            });
        }
        //end mike
        if (format == "currency") {
            //$('[data-format=currency]').each(function (i, item) {
            $('[data-format=currency]').each(function (i, item) {//mike
                if ($(this).find('.money-char').length == 0) {
                    var currency = '';
                    if ($(this).text().indexOf(" ") > 0) {
                        currency = $(this).text().substr($(this).text().indexOf(" ") + 1, 3);
                    }
                    var amount = parseFloat($(this).text());
                    $(this).text(parseFloat($(this).text()).formatMoney(2));
                    $(this).html('<span class="' + (currency != '' ? 'money-currency' : 'money') + (amount == 0 ? ' expired' : (amount < 0 ? ' mb-warning' : '')) + '"><span class="money-char">$</span><span class="money-amount">' + $(this).text() + (currency != '' ? ' ' + currency : '') + '</span></span>');
                }
            });
        }
    }

    var exportToExcel = function (fileName) {
        if ($('.export-excel').length > 0) {
            $('.export-excel').remove();
        }
        $('.exportable').each(function (i, item) {
            //mike
            //if ($(this).is(':visible')) {
                $(this).before('<img src="/content/images/xls.png" class="export-excel non-printable" style="width:40px;" id="btnGetExcelReport' + i + '" alt="Export to Excel" title="Export to Excel" />');
                var tableHtml;
                if ($(this).attr('id') == undefined) {
                    $(this).attr('id', 'table_' + i);
                }
                var tbl = document.getElementById($(this).attr('id'));
                if ($.fn.DataTable.fnIsDataTable(tbl)) {
                    var _oTable = $(this);
                    var thead = $(this).find('thead').clone();
                    var tbody = $(this).dataTable().fnGetAllTrNodes();
                    $('#' + _oTable.attr('id') + '_clone').remove();
                    $('body').append('<table style="display:none" id="' + _oTable.attr('id') + '_clone"><tbody></tbody></table>');
                    $('#' + _oTable.attr('id') + '_clone').prepend(thead);
                    $.each(tbody, function (i, item) {
                        $('#' + _oTable.attr('id') + '_clone tbody').prepend(item);
                    });
                    $('#btnGetExcelReport' + i).on('click', function () {
                        var _tbl = $(this).next('.exportable').attr('id');
                        fileName = fileName != undefined ? fileName : $('#' + _tbl).parents('fieldset').length > 0 ? $('#' + _tbl).parents('fieldset').find('legend img')[0].nextSibling.nodeValue.trim() : $('h1').eq(0).html();
                        tableHtml = $('#' + _tbl + '_clone').html();
                        tableHtml = tableHtml.replace(/<span .*?class="(.*?rule.*?)">(.*?)<\/span>/gi, "");
                        tableHtml = tableHtml.replace(/'/g, "");
                        $('#frmExcelReport').remove();
                        var iframe = $("<iframe id='frmExcelReport' style='display: none' src='about:blank'></iframe>").appendTo("body");
                        var formDoc = iframe[0].contentWindow || iframe[0].contentDocument;
                        if (formDoc.document) {
                            formDoc = formDoc.document;
                        }
                        formDoc.write("<html><head><title>" + fileName + '.xls' + "</title></head><body><form method='post' action='/crm/Reports/GetExcelFile?Length=3'><input type='hidden' id='content' name='content' value='" + '<table border="1">' + tableHtml + '</table>' + "' /><input type='hidden' id='filename' name='filename' value='" + fileName + "' /><input type='hidden' id='time' name='time' value='" + new Date().getTime() + "' /></form></body></html>");
                        var form = $(formDoc).find('form');
                        form.submit();
                    });
                }
                else {
                    fileName = fileName != undefined ? fileName : $('h1').eq(0).html();
                    var clone = $(this).clone();
                    $('.cell-detail', clone).remove();
                    tableHtml = clone.html();
                    $('#btnGetExcelReport' + i).on('click', function () {
                        tableHtml = tableHtml.replace(/<span .*?class="(.*?rule.*?)">(.*?)<\/span>/gi, "");
                        tableHtml = tableHtml.replace(/'/g, "");
                        $('#frmExcelReport').remove();
                        var iframe = $("<iframe id='frmExcelReport' style='display: none' src='about:blank'></iframe>").appendTo("body");
                        var formDoc = iframe[0].contentWindow || iframe[0].contentDocument;
                        if (formDoc.document) {
                            formDoc = formDoc.document;
                        }
                        formDoc.write("<html><head><title>" + fileName + '.xls' + "</title></head><body><form method='post' action='/crm/Reports/GetExcelFile?Length=3'><input type='hidden' id='content' name='content' value='" + '<table border="1">' + tableHtml + '</table>' + "' /><input type='hidden' id='filename' name='filename' value='" + fileName + "' /><input type='hidden' id='time' name='time' value='" + new Date().getTime() + "' /></form></body></html>");
                        var form = $(formDoc).find('form');
                        form.submit();
                    });
                }
            //}
        });
    }

    var _exportToExcel = function (fileName) {
        $('.export-excel').remove();
        $('.exportable').each(function (i, item) {
            $(this).before('<img src="/content/images/xls.png" class="export-excel non-printable" style="width:40px;" id="btnGetExcelReport' + i + '" alt="Export to Excel" title="Export to Excel" />');
            var clone = $(this).clone();
            $('.cell-detail', clone).remove();
            var tableHtml = clone.html();
            $('#btnGetExcelReport' + i).on('click', function () {
                tableHtml = tableHtml.replace(/<span .*?class="(.*?rule.*?)">(.*?)<\/span>/gi, "");
                tableHtml = tableHtml.replace(/'/g, "");
                $('#frmExcelReport').remove();
                var iframe = $("<iframe id='frmExcelReport' style='display: none' src='about:blank'></iframe>").appendTo("body");
                var formDoc = iframe[0].contentWindow || iframe[0].contentDocument;
                if (formDoc.document) {
                    formDoc = formDoc.document;
                }
                formDoc.write("<html><head><title>" + fileName + '.xls' + "</title></head><body><form method='post' action='/crm/Reports/GetExcelFile?Length=3'><input type='hidden' id='content' name='content' value='" + '<table border="1">' + tableHtml + '</table>' + "' /><input type='hidden' id='filename' name='filename' value='" + fileName + "' /><input type='hidden' id='time' name='time' value='" + new Date().getTime() + "' /></form></body></html>");
                var form = $(formDoc).find('form');
                form.submit();
            });
        });
    }

    var exportToCSV = function (fileName) {
        $('.exportcsv').each(function (i, item) {
            $(this).before('<input type="button" class="submit non-printable" style="margin-top:10px; float:right;" id="btnGetCSVReport' + i + '" alt="Export to CSV" title="Export to CSV" value="Export to CSV" />');
            var clone = $(this).clone();
            var csv = clone.html();
            $('#btnGetCSVReport' + i).on('click', function () {
                $('#frmCSVReport').remove();
                var iframe = $("<iframe id='frmCSVReport' style='display: none' src='about:blank'></iframe>").appendTo("body");
                var formDoc = iframe[0].contentWindow || iframe[0].contentDocument;
                if (formDoc.document) {
                    formDoc = formDoc.document;
                }
                formDoc.write("<html><head><title>" + fileName + '.csv' + "</title></head><body><form method='post' action='/crm/Reports/GetCSVFile?Length=3'><input type='hidden' id='content' name='content' value='" + csv + "' /><input type='hidden' id='filename' name='filename' value='" + fileName + "' /><input type='hidden' id='time' name='time' value='" + new Date().getTime() + "' /></form></body></html>");
                var form = $(formDoc).find('form');
                form.submit();
            });
        });
    }

    var verticalMenu = function () {
        //$('#sideMenuOpener').siblings().remove();
        $('#main').find('fieldset').each(function () {
            var numberOfParents = $(this).parents('fieldset').length;
            $('#optionsContainer').append('<a class="go-to level-' + numberOfParents + '" data-fdsid="' + $(this).attr('id') + '" ' + (numberOfParents > 1 ? 'style="display:none;"' : '') + '>' + $(this).children('legend').text() + '</a>');
        });
        $('#sideMenuOpener').unbind('click').on('click', function () {
            if ($('#sideMenuTriangle').hasClass('right-triangle')) {
                UI.openSideMenu();
            }
            else {
                UI.closeSideMenu();
            }
        });
        UI.adjustMenuOptions();
        //UI.closeSideMenu();
        UI.gotoFieldset();
    }

    var closeSideMenu = function () {
        $('#sideMenuTriangle').removeClass('left-triangle').addClass('right-triangle');
        $('#sideMenu').animate({
            marginLeft: '0px'
        }, 'fast');
        //animateMenuOpener(true);
    }

    var openSideMenu = function () {
        $('#sideMenuTriangle').removeClass('right-triangle').addClass('left-triangle');
        $('#sideMenu').animate({
            marginLeft: '215px'
            //marginLeft: '0px'
        }, 'fast');
        //animateMenuOpener(false);
    }

    function animateMenuOpener(isOpen) {
        //$('#sideMenuOpener').animate({
        //    marginLeft: isOpen ? '0px' : (($('#sideMenu').width() + 20)).toString() + 'px'
        //    //marginLeft: isOpen ? '0px' : (($('#sideMenu').width() + 20) * -1).toString() + 'px'//'-110px'
        //});
    }

    var gotoFieldset = function () {
        $('.go-to').unbind('click').on('click', function () {
            UI.expandFieldset($(this).attr('data-fdsid'));
            UI.scrollTo($(this).attr('data-fdsid'), null);
            UI.closeSideMenu();
        });
    }

    var adjustMenuOptions = function () {
        if ((($('#optionsContainer').height() * 100) / $(window).height()) > 80) {
            $('#optionsContainer').css({
                'height': ($(window).height() - 35).toString() + 'px',
                'overflow-y': 'scroll',
                'overflow-x': 'hidden'
            });
        }
        else {
            $('#optionsContainer').css({
                'overflow-y': 'hidden',
                'overflow-x': 'hidden'
            });
        }
    }

    var adjustLegends = function () {
        //$('legend').each(function () {
        //    if (($(window).width() - 90 - ($(this).parents('fieldset').length * 30)) > $(this).parent().width() && $(this).parent().width() > 0) {
        //        $(this).width(($(this).parent().width() * 0.98) + 'px');
        //    }
        //    else {
        //        $(this).width(($(window).width() - 90 - ($(this).parents('fieldset').length * 30)) + 'px');
        //    }
        //});

    }

    var legendClickBind = function () {
        $('legend').unbind('click').bind('click', function (e) {
            if ($(e.target).hasClass('wiki-icon') || $(e.target).hasClass('wizard-icon')) { //e.target !== this &&
                return;
            }

            //make sure the element has its visibility properly set again.
            if ($(this).parent('fieldset').children('div').hasClass('fakeDisplayNone')) {
                $(this).parent('fieldset').children('div').css("display", "none");
                $(this).parent('fieldset').children('div').removeClass('fakeDisplayNone');
            }

            if ($(this).parent('fieldset').children('div').is(':visible')) {
                UI.collapseFieldset($(this).parent('fieldset').attr('id'));
            } else {
                UI.expandFieldset($(this).parent('fieldset').attr('id'));
            }
        });

        $('.fieldset-expander').unbind('click').bind('click', function (e) {
            var event = $.Event('keydown');
            event.keyCode = 27;
            _source = { source: $(e.target).attr('id'), changeSelect: $(e.target).attr('data-trigger-change-on-clear') };
            //source = $(e.target).attr('id');
            $(document).trigger(event, _source);
            UI.resetValidation('frm' + $(e.target).attr('id').substr(6, $(e.target).attr('id').length - 6));
            UI.scrollTo('fds' + $(e.target).attr('id').substr(6, $(e.target).attr('id').length - 6));
        });
    }

    var collapseFieldset = function (fieldset) {
        if (!$('#' + fieldset).hasClass('non-collapsible')) {
            $('#' + fieldset).css('border-top', '#dddddd solid 1px');
            $('#' + fieldset).children('div').slideUp('fast', function () {
                $(this).css("display", "none");
                //$(this).css("display", "block");
                //$(this).addClass('fakeDisplayNone');
            });
            $('#' + fieldset).removeClass('fds-active');
            //$('#' + fieldset).children('legend').children('.fieldset-indicator:first-child').attr('src', '/Content/themes/base/images/eplat_show.jpg');
            $('#' + fieldset).children('legend').children('.fieldset-indicator').text('add_circle_outline');
            //in case legend has <a> :
            //$('#' + fieldset).children('legend').children('a').children('.fieldset-indicator:first-child').attr('src', '/Content/themes/base/images/eplat_show.jpg');
            $('#' + fieldset).children('legend').children('a').children('.fieldset-indicator').text('add_circle_outline');

            //hide in vertical menu
            $.each($('#' + fieldset).find('fieldset'), function () {
                $('a[data-fdsid=' + $(this).attr('id') + ']').slideUp('fast');
            });
        }

        //collapse children
        $('#' + fieldset).find('fieldset').each(function () {
            UI.collapseFieldset($(this).attr('id'));
        });

    }

    var expandFieldset = function (fieldset) {
        $('#' + fieldset).css('border-top', '#44c0e8 solid 1px');
        //$('#' + fieldset).children('div').removeClass('fakeDisplayNone');
        $('#' + fieldset).children('div').css('display', 'none');
        $('#' + fieldset).children('div').slideDown('fast');
        $('#' + fieldset).addClass('fds-active');
        //$('#' + fieldset).children('legend').children('.fieldset-indicator:first-child').attr('src', '/Content/themes/base/images/eplat_hide.jpg');
        $('#' + fieldset).children('legend').children('.fieldset-indicator').text('remove_circle_outline');
        // in case legend has <a> before img :
        //$('#' + fieldset).children('legend').children('a').children('.fieldset-indicator:first-child').attr('src', '/Content/themes/base/images/eplat_hide.jpg');
        $('#' + fieldset).children('legend').children('a').children('.fieldset-indicator').text('remove_circle_outline');
        UI.autoselectOnlyItemInList('#' + fieldset);
        //collapse siblings
        if ($('#' + fieldset).attr('data-close-siblings')) {
            $('#' + fieldset).siblings('fieldset').each(function () {
                UI.collapseFieldset($(this).attr('id'));
            });
        }

        //show in vertical menu
        $.each($('#' + fieldset).find('fieldset'), function () {
            if ($('#' + $(this).attr('id')).is(':visible')) {
                $('a[data-fdsid=' + $(this).attr('id') + ']').slideDown('fast');
            }
        });
    }

    //********** load respective workgroups in div
    var validatingWorkGroups = null;
    var userState = 0;
    var loadWorkGroups = function (user) {
        //UI.dontBlock = true;
        UI.validatingWorkGroups = true;
        if (window.location.pathname.indexOf('Logon') > 0) {
            $('#btnLogOn').attr('disabled', 'disabled').addClass('disabled');
        }
        $.ajax({
            url: '/Account/GetWorkGroupsByUser',
            type: 'GET',
            data: { userName: user },
            success: function (json) {
                //UI.dontBlock = false;
                UI.validatingWorkGroups = false;
                UI.userState = json.Model.UserState;
                if (json.ResponseType == 1) {
                    if (json.Model.UserID != '00000000-0000-0000-0000-000000000000' && json.ResponseMessage != null && json.ResponseMessage != '') {
                        $('.validation-summary-valid ul').append('<li>' + json.ResponseMessage + '</li>');
                        $('.validation-summary-valid').addClass('validation-summary-errors');
                        $('.validation-summary-errors').removeClass('validation-summary-valid').show();
                        $('.login-link').remove();
                        $('.login-validation').append("<a class=\"login-link\" href=\"javascript:UI.approveUnlock('" + json.Model.UserID + "')\">Click here to ask a supervisor to unlock your username.</a>");
                    }
                    //picture 

                    //work groups
                    var numberWorkGroups = json.Model.WorkGroups.length;
                    if (numberWorkGroups > 0) {
                        //i forgot
                        $('.login-link').remove();
                        $('.login-validation').append('<a class=\"login-link\" href="javascript:UI.recoverPassword(\'' + json.Model.UserID + '\')">I forgot my password</a>');

                        var builder = '';
                        var builderOpts = '<option value="">All</option>';
                        $.each(json.Model.WorkGroups, function (i, item) {
                            builder += '<span class="header-settings-menu-item"><input type="radio" id="radWorkGroup' + item.WorkGroupID + '" value="' + item.WorkGroupID + '" name="radWorkGroup" ' + (numberWorkGroups == 1 ? 'checked="checked"' : '') + ' />&nbsp;<label for="radWorkGroup' + item.WorkGroupID + '">' + item.WorkGroupName + '</label></span>';
                            builderOpts += '<option value="' + item.WorkGroupID + '">' + item.WorkGroupName + '</option>';
                        });

                        $('#divAvailableWorkGroups').html('').append(builder);
                        $('#ddlNotificationWorkGroup').html(builderOpts);
                        if (numberWorkGroups > 1) {
                            if ($(window).width() > 767) {
                                $('#headerWorkGroup').show();
                            }
                            //--10-22
                            if (localStorage.Eplat_SelectedWorkGroupID != '' && localStorage.Eplat_SelectedWorkGroupID != undefined) {
                                UI.selectedWorkGroup = localStorage.Eplat_SelectedWorkGroupID;
                            }
                            //if ($('#UserName').length > 0) {
                            //    UI.messageBox(0, 'Select a WorkGroup Before Login');
                            //}
                            //--end
                        }
                        UI.loadWorkGroup();
                    } else {
                        UI.messageBox(0, json.ResponseMessage, null, null);
                    }
                } else if (json.ResponseType == -1) {
                    UI.messageBox(0, json.ResponseMessage, null, null);
                }
            }
        });
    }

    var loadRoles = function (user, workgroup) {
        var userName;
        if (user != undefined) {
            userName = user;
        }
        $.ajax({
            url: '/Account/GetRolesByUser',
            type: 'GET',
            data: {
                userName: userName,
                workGroupID: workgroup
            },
            success: function (data) {
                var numberRoles = data.length;
                var builder = '';
                for (var i = 0; i < numberRoles; i++) {
                    var radioID = data[i].RoleID;
                    var radioElement = data[i].RoleName.toString();
                    builder += '<span class="header-settings-menu-item"><input type="radio" id="radRole' + radioID + '" value="' + radioID + '" name="radRole" />&nbsp;<label for="radRole' + radioID + '">' + radioElement + '</label></span>';
                }
                $('#divAvailableRoles').html('');
                $('#divAvailableRoles').append(builder);
                if (numberRoles > 1) {
                    $('#headerRoles').show();
                }
                else {
                    $('#headerRoles').hide();
                }
                UI.loadRole();
            }
        });
    }

    var loadTerminals = function () {
        $('#divAvailableTerminals').html('');
        $.ajax({
            url: '/Account/GetTerminalsByUser',
            type: 'GET',
            success: function (json) {
                var builder = '';
                if (json.length > 0) {
                    $.each(json, function (i, item) {
                        builder += '<span class="header-settings-menu-item"><input type="checkbox" id="chkTerminal' + item.TerminalID + '" class="chk-son" value="' + item.TerminalID + '" />&nbsp;<label for="chkTerminal' + item.TerminalID + '">' + item.Terminal.toString() + '</label></span>';
                    });
                    builder = '<span class="header-settings-menu-item" style="border-bottom: #444 solid 1px;"><input type="checkbox" id="chkAllTerminals" class="chk-parent" value="" />&nbsp;<label for="chkAllTerminals">Check / Uncheck All</label></span>' + builder;
                    $('#divAvailableTerminals').append(builder);
                    if ($(window).width() > 767) {
                        $('#headerTerminals').show();
                    }

                    UI.checkAllCheckBoxes('divAvailableTerminals');
                    //mike 15-05-21
                    if (localStorage.Eplat_SelectedTerminals != undefined && localStorage.Eplat_SelectedTerminals != '') {
                        UI.selectedTerminals = localStorage.Eplat_SelectedTerminals;
                    }
                    //end mike
                    if (UI.selectedTerminals != '' && UI.selectedTerminals != undefined) {
                        //ya hay terminales seleccionadas
                        var terminals = UI.selectedTerminals.split(',');
                        for (var j = 0; j < terminals.length; j++) {
                            $('#divAvailableTerminals input[type=checkbox][value=' + terminals[j] + ']').attr('checked', true);
                        }
                        //mike
                        currentTerminals = UI.selectedTerminals;
                        //console.log('call onterminalschange');
                        UI.onTerminalsChanged();
                        //end mike
                        //$('#divAvailableTerminals').trigger('click');
                    } else {
                        if ($('#divAvailableTerminals input[type=checkbox]').length > 0) {
                            //$('#divAvailableTerminals input[type=checkbox]').attr('checked', true);
                            //$('#divAvailableTerminals').trigger('click');
                            $('#divAvailableTerminals .chk-son').eq(0).attr('checked', true);
                        }
                    }
                }
            }
        });
    }

    var showValidationSummary = function (frmID) {
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
                        validationSummary += UTILS.stripTags($(this).html());
                        validationSummary += "</li>";
                    });
                validationSummary += "</ul>"
            }
            validationSummary = validationSummary.replace(/<li><\/li>/g, '');
            UI.messageBox(-1, validationSummary, -1);
        }

    }

    var showCurrentValidationErrors = function (frmID) {

        var errorsHolder = "#" + frmID + "_ErrorMessages";
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
                    validationSummary += UTILS.stripTags($(this).html());
                    validationSummary += "</li>";
                });
            validationSummary += "</ul>"
        }
        validationSummary = validationSummary.replace(/<li><\/li>/g, '');

        UI.messageBox(-1, validationSummary, -1);

    }

    var messageBox = function (type, message, duration, innerException) {
        // make sure the messabeBox is closed so the user can  
        // notice the messageBox is opening.
        if (UI.messageBoxIsOpen == true) {
            messageBoxReset();
            messageBoxExit();
        }

        //always colapse the details, so the user needs to open them again.
        collapseFieldset("messageBoxInnerException");

        // find out whether the first parameter is an object or number

        var _type;
        var _message;
        var _duration;
        var _innerException;
        var _boxType;

        if (!isNaN(type)) {
            _type = type;
            _message = message;
            _duration = duration;
            _innerException = innerException;
            _boxType = DEFAULTS.messageBox.boxType;
        } else {
            _type = type.type;
            _message = type.message;
            _duration = type.duration;
            _innerException = type.innerException;
            _boxType = type.boxType || DEFAULTS.messageBox.boxType;
            _onAcceptCallBack = type.onAcceptCallBack || null;
            _onAcceptParams = type.onAcceptParams || null;
            //mike
            _onAcceptButtonValue = type.onAcceptButtonValue || null;
            _onCancelCallBack = type.onCancelCallBack || null;
            _onCancelParams = type.onCancelParams || null;
            _onCancelButtonValue = type.onCancelButtonValue || null;
        }

        if (!_duration) { _duration = DEFAULTS.messageBox.duration; }
        if (!_innerException) { _innerException = ""; }
        //common behavior.
        $('#messageBoxTitle').removeAttr('class');
        switch (_type) {
            case 1:
                $('#messageBoxTitle').addClass('mb-confirmation');
                $('#messageBoxTitle').html('CONFIRMATION');
                break;
            case 0:
                $('#messageBoxTitle').addClass('mb-warning');
                $('#messageBoxTitle').html('WARNING');
                break;
            case -1:
                $('#messageBoxTitle').addClass('mb-error');
                $('#messageBoxTitle').html('ERROR');
                break
        }
        $('#messageBoxContent').html(_message);
        //end common behavior.

        //show action buttons by default

        //hide elements and show them depending on the boxType
        $("#messageBoxActionButtons").hide();
        $('#messageBoxClose').show();

        //begin ying 
        if (_boxType == OPTIONS.messageBox.boxTypes.messageBox) {
            $('#messageBoxClose').show();
            if (_innerException != "") {
                $("#messageBoxInnerException").show();
            } else { $("#messageBoxInnerException").hide(); }

            var iframeContent = $('#messageBoxDetails').contents()
            if (iframeContent.find('html').find("style").length == 0)
            { iframeContent.find('head').append('<link href="/Content/Site.css" rel="stylesheet" type="text/css" />'); }
            iframeContent.find('body').html(_innerException);
        } else if (_boxType == OPTIONS.messageBox.boxTypes.confirmBox) {
            $('#messageBoxClose').hide();
            $('#messageBoxBtnConfirm').attr('value', 'Confirm');
            $('#messageBoxBtnCancel').attr('value', 'Cancel');
            $("#messageBoxActionButtons").show();
            //Accept
            if (_onAcceptCallBack != null) {
                $("#messageBoxBtnConfirm").click(function () {
                    _onAcceptCallBack.apply(this, _onAcceptParams);
                });
            }
            //close the box no matter what the user's choice was
            $("#messageBoxActionButtons").find("input:button").each(
                function () {
                    $(this).click(function () {
                        messageBoxReset();
                        messageBoxExit();
                    });
                }
            );
        }
            //end ying    begin mike
        else if (_boxType == OPTIONS.messageBox.boxTypes.twoActionBox) {
            //$('#messageBoxClose').hide();
            //change value of buttons
            $('#messageBoxBtnConfirm').attr('value', _onAcceptButtonValue);
            $('#messageBoxBtnCancel').attr('value', _onCancelButtonValue);
            //$('#messageBoxBtnConfirm').attr('value', 'print coupon');
            //$('#messageBoxBtnCancel').attr('value', 'send by email');
            //
            $("#messageBoxActionButtons").show();
            //$("#messageBoxActionButtons").find("input:button").each(
            //    function () {
            //        $(this).click(function () {
            //            messageBoxReset();
            //            messageBoxExit();
            //        });
            //    }
            //);
            //Print
            if (_onAcceptCallBack != null) {
                $('#messageBoxBtnConfirm').unbind('click').click(function () {
                    _onAcceptCallBack.apply(this, _onAcceptParams);
                });
                //$('#messageBoxBtnConfirm').on('dblclick', function (e) {
                //    UI.notifyDoubleClick($(this).attr('id'));
                //});
            }
            //Send 
            if (_onCancelCallBack != null) {
                $('#messageBoxBtnCancel').unbind('click').click(function () {
                    _onCancelCallBack.apply(this, _onCancelParams);
                });
                //$('#messageBoxBtnCancel').on('dblclick', function () {
                //    UI.notifyDoubleClick($(this).attr('id'));
                //});
            }
        }
        //end mike

        //input animation
        $('#messageBox').animate({
            marginTop: '0px'
        }, 400, 'easeOutBack');

        //duration
        if (_duration >= 0) {
            $('#messageBoxCounter').html(_duration);
            //make sure we kill any other timer already started so we can prevent weird behaviors.
            clearInterval(UI.messageBoxInterval);
            messageBoxInterval = setInterval('UI.messageBoxTimer()', 1000);
        }
        else {
            //no need to show the countdown
            clearInterval(UI.messageBoxInterval);
            $('#messageBoxCounter').html("");
        }

        $('#messageBoxClose').click(function () {
            messageBoxExit();

        });

        //messageBoxOpen
        $('#messageBoxOpener').click(function () {
            $('#messageBox').animate({
                marginTop: '0px'
            }, 400, 'easeOutBack');
            //make sure the opener is hidden
            //$('#messageBoxOpener').hide();
            //unbind click
            //$('#messageBox').unbind("click");
            $('#messageBoxOpener').slideUp('fast');
        });
        UI.messageBoxIsOpen = true;
        UI.adjustLegends();
    }

    var messageBoxReset = function () {
        $('#messageBoxClose').hide();
        $('#messageBoxTitle').html('');
        $('#messageBoxContent').html('');
        $('#messageBoxActionButtons').hide();
        $("#messageBoxActionButtons").find("input:button").each(
               function () {

                   $(this).unbind("click");
               }
           );
        $('#messageBoxInnerException').hide();
        $('#messageBoxOpener').unbind("click");
    }
    var messageBoxExit = function () {
        var desiredMarginTop = (($('#messageBox').height() + 35) * -1);
        var currentMargingTop = $('#messageBox').css('marginTop');
        ////do the animation only when these two values are diferent
        //if (!messageBoxInterval || parseInt(currentMargingTop) != desiredMarginTop)
        //{;}
        $('#messageBox').animate({
            marginTop: desiredMarginTop + 'px'
        }, 400, 'easeOutBack');

        UI.messageBoxIsOpen = false;
        clearInterval(UI.messageBoxInterval);

        //// every time the message is closed, show the opener and clean the counter.
        $('#messageBoxOpener').slideDown('fast');
        $('#messageBoxCounter').html("");

    }

    var messageBoxTimer = function () {
        $('#messageBoxCounter').html(parseInt($('#messageBoxCounter').html()) - 1);

        if (parseInt($('#messageBoxCounter').html()) <= 0) {
            messageBoxExit();
            $('#messageBoxOpener').click(function () {
                $('#messageBox').animate({
                    marginTop: '0px'
                }, 400, 'easeOutBack');
                ////make sure the opener is hidden
                //$('#messageBoxOpener').hide();
            });
        } else if (isNaN(parseInt($('#messageBoxCounter').html()))) //before this condition the messageBox was showign a NaN text.
        {
            clearInterval(UI.messageBoxInterval);
            $('#messageBoxCounter').html("");
        }
    }

    //var menuOver = function () {
    //    $(this).children('.submenu').animate({
    //        marginTop: '35px'
    //    }, 200, 'linear');
    //    $(this).addClass('selected');

    //    //$(this).children('.submenu').animate({
    //    //    marginTop: '35px'
    //    //}, 200, 'linear');
    //    //$(this).addClass('selected');
    //}

    //var menuOut = function () {
    //    var margintop = (($(this).height() + 35) * -1);
    //    $(this).animate({
    //        marginTop: margintop + 'px'
    //    }, 200, 'linear', function () {
    //        $(this).parent().removeClass('selected');
    //    });
    //}

    //var submenusClose = function () {
    //    $('.submenu').each(function (index) {
    //        var margintop = (($(this).height() + 35) * -1);
    //        $(this).animate({
    //            marginTop: margintop + 'px'
    //        }, 200, 'linear', function () {
    //            $(this).parent().removeClass('selected');
    //        });
    //    });
    //}

    var selectTerminals = function () {
        UI.selectedTerminals = '';
        var selectedTerminalsCounter = $('#divAvailableTerminals input[type=checkbox]:checked:not(.chk-parent)').length;
        if (selectedTerminalsCounter == 0) {
            //$('#divAvailableTerminals').find('input:checkbox').first().attr('checked', true);
            $('#divAvailableTerminals').find('input:checkbox.chk-son').first().attr('checked', true);//this will autoselect first terminal in list instead of .chk-parent item
            selectedTerminalsCounter = $('#divAvailableTerminals input[type=checkbox]:checked').length;
        }
        if (selectedTerminalsCounter != 1) {
            //selectedTerminalsCounter--;//to avoid count of .chk-parent item
            $('#divSelectedTerminals').html(selectedTerminalsCounter + ' Terminals Selected');
            for (var i = 0; i < selectedTerminalsCounter; i++) {
                if (UI.selectedTerminals != '') {
                    UI.selectedTerminals += ',';
                }
                UI.selectedTerminals += $('#divAvailableTerminals input[type=checkbox]:checked:not(.chk-parent)')[i].value;
            };
        } else {
            $('#divSelectedTerminals').html($('#divAvailableTerminals label[for=chkTerminal' + $('#divAvailableTerminals input[type=checkbox]:checked').val() + ']').html());
            UI.selectedTerminals = $('#divAvailableTerminals input[type=checkbox]:checked').val();
        }
        if (currentTerminals != UI.selectedTerminals) {
            localStorage.Eplat_SelectedTerminals = UI.selectedTerminals;
            currentTerminals = UI.selectedTerminals;
            UI.saveTicket(UI.onTerminalsChanged);
        }
    }

    var terminalsOpen = function () {
        currentTerminals = UI.selectedTerminals;
        $('#divAvailableTerminals').show();
    }

    var terminalsClose = function () {
        UI.selectTerminals();
        //if (currentTerminals != UI.selectedTerminals) {
        //    $('body').trigger('UI.onTerminalsChanged', UI.selectedTerminals);
        //}
        $('#divAvailableTerminals').hide();
    }

    var workGroupsOpen = function () {
        currentWorkGroup = UI.selectedWorkGroup;
        $('#divAvailableWorkGroups').show();
    }

    var workGroupsClose = function () {
        $('#divAvailableWorkGroups').hide();
        if (currentWorkGroup != UI.selectedWorkGroup && $('#divAvailableWorkGroups input[type=radio]:checked').length > 0) {
            //$('body').trigger('UI.onWorkGroupChanged',UI.selectedWorkGroup);
        }
    }

    var rolesOpen = function () {
        currentRole = UI.selectedRole;
        $('#divAvailableRoles').slideDown('fast');
    }

    var rolesClose = function () {
        $('#divAvailableRoles').slideUp('fast');
    }

    var selectWorkGroup = function () {
        UI.selectedWorkGroup = '';
        if ($('#divAvailableWorkGroups input[type=radio]:checked').length > 0) {
            //$('#selectedWorkGroupID').attr('value', $('#divAvailableWorkGroups input[type=radio]:checked').val());
            $('#divSelectedWorkGroup').html($('#divAvailableWorkGroups label[for=radWorkGroup' + $('#divAvailableWorkGroups input[type=radio]:checked').val() + ']').html());
            UI.selectedWorkGroup = $('#divAvailableWorkGroups input[type=radio]:checked').val();
            localStorage.Eplat_SelectedWorkGroupID = UI.selectedWorkGroup;
            if ($('#UserName').length > 0) {
                UI.loadRoles($('#UserName').val(), UI.selectedWorkGroup);
            }
            else {
                UI.loadRoles(null, UI.selectedWorkGroup);
            }

        } else {
            $('#divSelectedWorkGroup').html('Work Groups');
        }
    }

    var selectRole = function () {
        UI.selectedRole = '';
        if ($('#divAvailableRoles input[type=radio]:checked').length > 0) {
            //$('#selectedRoleID').attr('value', $('#divAvailableRoles input[type=radio]:checked').val());
            $('#divSelectedRole').html($('#divAvailableRoles label[for=radRole' + $('#divAvailableRoles input[type=radio]:checked').val() + ']').html());
            UI.selectedRole = $('#divAvailableRoles input[type=radio]:checked').val();
            localStorage.Eplat_SelectedRole = UI.selectedRole;
            //if ($('#divAvailableRoles input[type=radio]:checked').length > 0 && window.location.pathname.indexOf('Logon') > -1) {
            //    if ($('#Password').val() != '') {
            //        $('#btnLogOn').trigger('click');
            //    }
            //}
            if (!$('#UserName').length > 0) {
                UI.loadMenuComponents();
            }
        }
        else {
            $('#divSelectedRole').html('Roles');
            UI.rolesOpen();
            UI.messageBox(0, 'Select a Role');
        }
    }

    var loadMenuComponents = function () {
        var url = $(location).attr('protocol') + '//' + $(location).attr('host');
        $.ajax({
            url: '/Account/GetMenuComponents',
            type: 'GET',
            data: {
                selectedWorkGroupID: UI.selectedWorkGroup,
                selectedRoleID: UI.selectedRole
            },
            success: function (data) {
                var builder = '';
                var flag;
                $.each(data, function (index, item) {
                    if (data[index].SysComponentTypeID == 1) {
                        flag = false;
                        builder += '<li>';
                        if (item.SysComponentUrl != undefined && item.SysComponentUrl != null) {
                            builder += '<a href="' + item.SysComponentUrl + '">' + item.SysComponentName + '</a>';
                        }
                        else {
                            builder += '<a href="#">' + item.SysComponentName + '</a>';
                        }
                        builder += '<ul class="submenu">';
                        recursive(item.SysComponentID);
                    }
                });
                function recursive(componentID) {
                    $.each(data, function (index2, item2) {
                        if (item2.SysParentComponentID == componentID) {
                            flag = true;
                        }
                    });
                    if (flag == false) {//has children
                        builder += '</ul></li>';
                    }
                    else {
                        $.each(data, function (index2, item2) {
                            if (item2.SysParentComponentID == componentID) {
                                builder += '<li>';
                                if (item2.SysComponentUrl != undefined && item2.SysComponentUrl != null) {
                                    builder += '<a href="' + url + item2.SysComponentUrl + '">' + item2.SysComponentName.toString().trim() + '</a>';
                                }
                                else {
                                    builder += '<a href="#">' + item2.SysComponentName.toString().trim() + '</a>';
                                }
                                builder += '<ul class="submenu">';
                                recursive(item2.SysComponentID);
                            }
                        });
                        builder += '</ul></li>';
                    }
                    return builder;
                }
                $('#menu').empty();
                while (builder.indexOf('<ul class="submenu"></ul>') > 0) {
                    builder = builder.substr(0, builder.indexOf('<ul class="submenu"></ul>')) + builder.substr(builder.indexOf('<ul class="submenu"></ul>') + 25, builder.length);
                }
                $('#menu').append(builder);
                UI.saveTicket();
                UI.menuActions();
                //load workgroup-dependent items
                //UI.workgroupDependentActions();
                $('#menuOpener').on('click', function () {
                    if ($('#menu').is(':visible')) {
                        $('#menu').hide();
                        $('.submenu').hide();
                    } else {
                        $('#menu').show();
                        $('.submenu').show();
                    }
                });
                $('header .user-icon').on('click', function () {
                    if ($('#headerTerminals').is(':visible')) {
                        $('#headerTerminals').hide();
                        $('#headerWorkGroup').hide();
                    } else {
                        $('#headerTerminals').show();
                        $('#headerWorkGroup').show();
                    }
                });
            }
        });
    }

    var loadWorkGroup = function () {
        //--10-22
        //if (UI.selectedWorkGroup != undefined && UI.selectedWorkGroup != '') {
        //    $('#radWorkGroup' + UI.selectedWorkGroup).attr('checked', true);
        //}
        //else {
        //    UI.workGroupsOpen();
        //}
        if (UI.selectedWorkGroup != undefined && UI.selectedWorkGroup != '') {
            $('#radWorkGroup' + UI.selectedWorkGroup).attr('checked', true);
            if (window.location.pathname.indexOf('Logon') > 0) {
                $('#btnLogOn').attr('disabled', null).removeClass('disabled');
            }
        }
        else if (localStorage.Eplat_SelectedWorkGroupID != undefined && localStorage.Eplat_SelectedWorkGroupID != '') {
            $('#radWorkGroup' + localStorage.Eplat_SelectedWorkGroupID).attr('checked', true);
            if (window.location.pathname.indexOf('Logon') > 0) {
                $('#btnLogOn').attr('disabled', null).removeClass('disabled');
            }
        }
        else if (window.location.pathname.indexOf('Logon') > 0) {
            UI.messageBox(0, 'Select a WorkGroup Before Login');
            $('#btnLogOn').attr('disabled', null).removeClass('disabled');
            UI.workGroupsOpen();
        }
        //--end
        UI.selectWorkGroup();
    }

    var loadRole = function () {
        if ($('#divAvailableRoles input[type=radio]').length > 0) {
            if ($('#divAvailableRoles input[type=radio]').length == 1) {
                $('#radRole' + $('#divAvailableRoles input[type=radio]').val()).attr('checked', true);
            }
            else {
                if (UI.selectedRole != undefined && UI.selectedRole != '') {
                    $('#radRole' + UI.selectedRole).attr('checked', true);
                }
            }
        }
        else {
            UI.rolesOpen();
            UI.messageBox(0, 'Select a Role');
        }
        //$('#selectedRoleID').val('');
        UI.selectRole();
    }

    var menuActions = function () {
        if ($('#menu').length > 0) {
            $('#menu').multiNav();
            //select menu section
            var path = window.location.pathname;
            path = path.substr(1);
            path = path.substr(0, path.indexOf('/'));
            if (path == "") {
                $('a:contains(\'Dashboard\')').parent().addClass('selected-section');
            }
            else {
                $('a:contains(\'' + path.toUpperCase() + '\')').parent().addClass('selected-section');
            }
        }
    }

    var tablesHoverEffect = function () {
        //$('.table tr').hover(function () {
        //    $(this).not('.trheader').attr('style', 'background-color: rgb(226, 222, 214); cursor:pointer');
        //},
        //        function () {
        //            $(this).not('.trheader').attr('style', null);
        //        });
    }

    var tablesStripedEffect = function () {
        //$('.table tr').each(function () {
        //    $(this).removeClass('odd');
        //    $(this).removeClass('striped');
        //});
        //$('.table tr:nth-child(even)').addClass('striped');
    }

    var ulsHoverEffect = function (ulName) {
        //$('ul li').hover(function () {
        if ($('#' + ulName + ' li p').length > 0) {
            var oldStyle = '';
            $('#' + ulName + ' li p').hover(function () {
                oldStyle = $(this).attr('style').toString();
                if (oldStyle.substr(oldStyle.length - 1, oldStyle.length) != ';') {
                    oldStyle += ';';
                }
                $(this).attr('style', oldStyle + 'background-color: rgb(226,222,214); cursor:pointer');
            }, function () {
                $(this).attr('style', oldStyle);
            });
        }
        else {
            $('#' + ulName + ' li').hover(function () {
                $(this).attr('style', 'background-color: rgb(226,222,214); cursor:pointer');
            }, function () {
                $(this).attr('style', null);
            });
        }
    }

    var resetValidation = function (form) {
        $('#' + form).find('.input-validation-error').each(function () {
            $(this).addClass('input-validation-valid').removeClass('input-validation-error');
        });
        $('#' + form).find('.field-validation-error').each(function () {
            $(this).addClass('field-validation-valid').removeClass('field-validation-error');
        });
        $('#' + form).find('.validation-summary-errors').removeClass('validation-summary-errors');
    }

    var checkAllCheckBoxes = function (container) {
        if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
            $('#' + container).find('.chk-parent').attr('checked', 'checked');
        }
        $('#' + container).find('.chk-parent').change(function () {
            $('#' + container).find('.chk-son').attr('checked', this.checked);
        });
        $('#' + container).find('.chk-son').change(function (e) {
            if ($('#' + container).find('.chk-son').length == $('#' + container).find('.chk-son:checked').length) {
                $('#' + container).find('.chk-parent').attr('checked', 'checked');
            }
            else {
                $('#' + container).find('.chk-parent').attr('checked', false);
            }
        });
    }

    var saveTicket = function (callback) {
        $.ajax({
            url: '/Account/SaveTicket',
            type: 'POST',
            cache: false,
            data: {
                workGroupID: UI.selectedWorkGroup,
                roleID: (UI.selectedRole != undefined ? UI.selectedRole.toString() : $('#divAvailableRoles input[type=radio]').eq(1).val()),
                terminals: UI.selectedTerminals
            },
            success: function (data) {
                if (data.ok != undefined) {
                    //$('.workgroup-dependent-list').trigger('loaded');//there is not any action defined for this event
                    //$('.terminal-dependent-list').trigger('loaded');
                    UI.Notifications.workingOn();

                    if (typeof callback == 'function') {
                        callback();
                    }
                }
                else {
                    $('.logout > a')[0].click();
                }
            }
        });
    }

    var searchResultsTable = function (tableName, index, paramsObject) {
        UI.oTable = $('#' + tableName).dataTable({
            "bFilter": false,
            "bProcessing": true,
            //"asStripeClasses": ['odd', 'striped'],
            "bAutoWidth": false,
            //'aoRowCreatedCallback': [makeTableRowsSelectable()],
            //"aoColumnDefs": [{ 'aTargets': [index] }],
            "aoRowCallback": [UI.tablesHoverEffect()],
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            }
        });
    }

    function setTableRowsClickable(params) {
        // params object definition
        var tblID = params.tblID; // the htmlTable.id to manipulate // at least this property must be specified when calling.
        var onClickCallbackFunction = params.onClickCallbackFunction || null; // the function to call when a row is clicked
        var callbackParams = params.callbackParams || null; // the parameters for the function to be called when a row is clicked.

        //mike begin
        var oTable;
        if ($.fn.DataTable.fnIsDataTable(document.getElementById(tblID))) {
            oTable = $('#' + tblID).dataTable();
            oTable = oTable.$('tr');
        }
        else {
            oTable = $('#' + tblID + ' tbody tr');
        }
        //$("#" + tblID + " tbody tr").not('theader').click(function (e) {
        //end mike

        oTable.not('theader').unbind('click').on('click', function (e) {
            if (!$(this).hasClass("selected-row") && !$(e.target).is('input')) {
                oTable.removeClass("selected-row primary");
                $(this).addClass("selected-row primary");
                var rowIndex = $(this).closest('tr')[0].sectionRowIndex + 1;
                if (onClickCallbackFunction != null) {
                    var _callbackParams = callbackParams || {};
                    _callbackParams["currentTargetID"] = e.currentTarget.id;
                    onClickCallbackFunction.call(null, _callbackParams);
                }
            }
        });
    }

    var unselectRow = function () {
        $(document).unbind('keydown').bind('keydown', function (evt, source) {
            if (evt.keyCode === 27) {
                var $element = source != undefined ? $('#' + $('#' + source.source).parents('fieldset').attr('id')).find('.selected-row').first() : $(document).find('.fds-active:has(.selected-row:visible)').last().find('.selected-row').first();
                if ($element.length > 0) {
                    if ($element.hasClass('primary')) {
                        $(document).find('.primary-selected-row-dependent').each(function () {
                            if ($(this).is('fieldset')) {
                                $(this).find('form').each(function () {
                                    $(this).clearForm((source != undefined ? source.changeSelect : undefined));
                                    $(this).find('textarea').each(function () {
                                        $(this).val('');
                                        if ($(this).css('display') == 'none') {
                                            $(this).ckeditor();
                                        }
                                    });
                                });
                                if (source == undefined) {
                                    UI.collapseFieldset($(this).attr('id'));
                                }
                            }
                            if ($(this).is('ul') | $(this).is('tbody')) {
                                if ($(this).find('input:checkbox').length > 0) {
                                    $(this).find('input:checkbox').each(function () {
                                        $(this).attr('checked', false);
                                    });
                                }
                                else {
                                    $(this).empty();
                                }
                            }
                            if ($(this).is('div')) {
                                $(this).html('');
                            }
                            $(this).val('');
                            if ($(this).is('span')) {
                                $(this).text('');
                            }
                        });
                        $element.removeClass('selected-row primary');
                        $(document).find('.secondary-selected-row-dependent').each(function () {
                            $(this).val('');
                            if ($(this).hasClass('selected-row secondary')) {
                                $(this).removeClass('selected-row secondary');
                            }
                            if ($(this).hasClass('jstree')) {
                                $(this).empty();
                            }
                        });
                    }
                    if ($element.hasClass('secondary')) {
                        $element.parents('fieldset').first().find('fieldset').first().find('.secondary-selected-row-dependent').each(function () {
                            if (!$element.hasClass('skip-one-level')) {
                                if ($(this).hasClass('skip-one-level')) {

                                }
                                else {
                                    $(this).val('');
                                }
                            }
                            else {
                                $(this).val('');
                            }
                            if ($(this).is('span')) {
                                $(this).text('');
                            }
                            //if ($(this).is('ul')) {
                            if ($(this).is('ul') || $(this).is('tbody')) {
                                $(this).empty();
                            }
                            if ($(this).hasClass('jstree')) {
                                $(this).empty();
                            }
                        });
                        $element.parents('fieldset').first().find('fieldset').first().find('form').clearForm((source != undefined ? source.changeSelect : undefined));

                        UI.resetValidation($element.parents('fieldset').first().find('fieldset').first().find('form').attr('id'));
                        $element.parents('fieldset').first().find('fieldset').first().find('form').first().find('textarea').each(function () {
                            $(this).val('');
                            if ($(this).css('display') == 'none') {
                                $(this).ckeditor();
                            }
                        });
                        //code changed to remove class for more than one rows at a time
                        //$element.parent().find('.selected-row').each(function () {
                        //    $(this).removeClass('selected-row secondary');
                        //});
                        if ($element.hasClass('related-price')) {
                            $(document).find('.price-groups').each(function () {
                                $(this).css('font-weight', 'normal');
                                //$(this).removeAttr('style');
                            });
                            $element.removeClass('related-price');
                        }
                        $element.removeClass('selected-row secondary');
                    }
                    if ($element.is('li')) {
                        $element.parent().siblings('fieldset').first().find('.view-restricted').each(function () {
                            if ($(this).is('fieldset'))
                                UI.collapseFieldset($(this).attr('id'));
                            $(this).hide();
                        });
                    }
                    //new code used in categories, terminals module
                    if ($element.is('p')) {
                        $element.parents('.secondary-selected-row-dependent').siblings('fieldset').first().find('.view-restricted').each(function () {
                            if ($(this).is('fieldset')) {
                                UI.collapseFieldset($(this).attr('id'));
                            }
                            $(this).hide();
                        });
                    }
                    if ($element.is('tr')) {
                        $element.parents('fieldset').first().find('.view-restricted').each(function () {
                            if ($(this).is('fieldset'))
                                UI.collapseFieldset($(this).attr('id'));
                            $(this).hide();
                        });
                        $element.parents('fieldset').first().find('.view-allowed').each(function () {
                            $(this).show();
                        });
                        $element.parents('fieldset').first().find('.clear-content').each(function () {
                            $(this).find('input:not(.button submit)').each(function () {
                                if ($(this).is(':checkbox'))
                                    $(this).removeAttr('checked');
                                if ($(this).is(':text'))
                                    $(this).val('');
                            });
                            //new-line
                            $(this).find('div.clear-content').empty();
                        });
                    }
                    if ($element.is('a')) {
                        $element.parents('fieldset').first().find('.view-restricted').each(function () {
                            if ($(this).is('fieldset')) {
                                UI.collapseFieldset($(this).attr('id'));
                            }
                            $(this).hide();
                        });
                        $element.removeClass('selected-row secondary');
                        //used only in pictures, not banners
                        if ($('.fds-active').find('.secondary-selected-row-dependent:not(input)').length > 0) {
                            if ($('.fds-active').find('.secondary-selected-row-dependent:not(input):last').attr('id').toLowerCase().indexOf('banner') == -1) {
                                $('.fds-active').find('.secondary-selected-row-dependent:not(input):last').empty();
                                $('.fds-active').find('.secondary-selected-row-dependent:not(input):last').siblings().hide();
                            }
                        }
                    }
                    if ($element.is('span')) {
                        $element.removeClass('selected-row secondary');
                        $('#liNewComponent').remove();
                        if ($element.parents('li').first().find('ul:has(li)').length == 0) {
                            $element.parents('li').first().removeAttr('class');
                        }
                    }
                    if ($element.is('div')) {
                        $element.siblings('ul').find('.new-component').remove();
                    }
                    if (source != undefined && $('#fds' + source.source.substr(6)).children('div').is(':not(:visible)')) {
                        UI.expandFieldset($element.parent().siblings('fieldset').first().attr('id'));
                    }
                    if (source != undefined && $element.parents('fieldset').first().find('fieldset').first().children('div').is(':not(:visible)')) {
                        UI.expandFieldset($('#fds' + source.source.substr(6)).attr('id'));
                    }
                }
                else {
                    try {
                        var $fieldset = $('#fds' + source.source.substr(6));
                        $fieldset.find('form').first().clearForm((source != undefined ? source.changeSelect : undefined));
                        //new code added (check if works in every place) 27/08
                        $fieldset.find('.primary-selected-row-dependent').each(function () {
                            if ($(this).is(':not(fieldset)')) {
                                $(this).empty();
                            }
                        });
                        //end new code
                        UI.resetValidation($fieldset.find('form').first().attr('id'));
                        //new code for reset ckeditor
                        if ($fieldset.find('form').first().find('form').first().find('textarea').length > 0) {
                            $fieldset.find('form').first().find('form').first().find('textarea').each(function () {
                                $(this).val('');
                                if ($(this).css('display') == 'none') {
                                    $(this).ckeditor();
                                }
                            });
                        }
                        else {
                            $fieldset.find('form').first().find('textarea').each(function () {
                                $(this).val('');
                                if ($(this).css('display') == 'none') {
                                    $(this).ckeditor();
                                }
                            });
                        }
                        //end new code ckeditor
                        if ($fieldset.children('div').hasClass('fakeDisplayNone') || $fieldset.children('div').is(':not(:visible)'))
                            UI.expandFieldset($fieldset.attr('id'));
                    }
                    catch (ex) { }
                }
            }
            UI.autoselectOnlyItemInList();
        });
    }

    var unselectPrimaryByEsc = function (_oTable, _tr) {
        _oTable.$('tr.selected-row').removeClass('selected-row primary');
        $(document).find('.selected-row').each(function () {
            if ($(this).parents('fieldset:first').hasClass('fds-active')) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            }
        });
        _tr.addClass('selected-row primary');
    }

    function ckeditorUpdateInstances(formID) {
        for (var i in CKEDITOR.instances) {
            var instance = CKEDITOR.instances[i].name;
            if ($('#' + formID).find($('#' + instance)).length > 0 && $('#' + instance).attr('data-val-required')) {
                CKEDITOR.instances[i].updateElement();
            }
        }
    }

    function scrollTo(elementID, scrollingDuration) {
        /// <summary>Scrolls the page until the specified element is visible.</summary>
        /// <param name="elementID">The element to scroll to.</param>
        /// <param name="scrollingDuration">The duration in milliseconds of the scrolling.</param>
        /// <returns></returns>

        if (scrollingDuration == null) { scrollingDuration = 500 }
        var headerHeight = parseInt($("header").height());
        var navHeight = parseInt($("nav").height());
        var currentOffSet = $("#" + elementID).offset().top;
        var newScrollTop = currentOffSet - (headerHeight + navHeight);
        //console.log('a: ' + elementID + '-' + newScrollTop);
        if ($('#customerName').length > 0 && newScrollTop > 1120) {
            //newScrollTop -= 90;
            newScrollTop -= 120;
        }
        $('html, body').delay(600).animate({ scrollTop: newScrollTop }, scrollingDuration);
    }
    /// <summary>
    /// Runs the form validation without submit.
    /// </summary>
    /// <param name="formID">The form to be validated.</param>
    /// <returns></returns> 
    function validateForm(formID) {
        $("#" + formID).valid();
    }
    function validateRelatedElement(fieldID, relatedElement) {

        var hls = new SETTINGS.highlightElements();
        hls.elements = $("#" + relatedElement);

        if (!$("#" + fieldID).valid()) {
            hls.highlightTime = 0;
            UI.highlightElements(hls);
        } else {
            UI.unhighlightElements(hls);
        }
    }

    function validateRelatedElements(frmID) {
        var invalidElements = $();
        var errors = [];
        var tables = $("#" + frmID + " [data-related-validation-field]");
        tables.each(
            function () {
                var fieldID = $(this).data('related-validation-field');
                //var invalidFields = UI.validateWithRelatedField();
                //validate the table
                //entireTableIsValid
                //RowsWithErrors
                var tableID = $(this).attr('id');
                var tableValidation = UI.isDataTableValid(tableID);

                if (tableValidation.entireTableIsValid == false) {
                    if (tableValidation.errors.length > 0) {
                        // errors.concat(tableValidation.errors);
                        for (var err in tableValidation.errors) {
                            errors.push(tableValidation.errors[err]);
                        }

                    } else {
                        errors.push("Some " + $(this).data('items-name') + "s have errors");
                    }

                    //add the table 
                    invalidElements = invalidElements.add(this);
                    //also add the rows with errors
                    tableValidation.rowsWithErrors.each(
                        function () {
                            invalidElements = invalidElements.add(this);
                        }
                    );


                }
            }
        );

        // return invalidElements;
        return { invalidElements: invalidElements, errors: errors };
    }

    function unhighlightElements(params) {
        if (params.elements.length > 0) {
            $(params.elements).each(function (i) {
                $(this).removeClass(params.cssClass);
            });
        }
    }

    function highlightElements(params) {
        if (params.highlightTime > 0) {
            //resest other timed hilights
            if ($(UI.searchTextInColumns_previousHilightedElements).length > 0) {
                var uhls = new SETTINGS.highlightElements();
                uhls.elements = UI.searchTextInColumns_previousHilightedElements;
                UI.unhighlightElements(uhls);
                //reset the jquery object
                UI.searchTextInColumns_previousHilightedElements = $();
                for (var t in UI.searchTextInColumns_hilightTimers) {
                    window.clearTimeout(UI.searchTextInColumns_hilightTimers[t]);
                }
                UI.searchTextInColumns_hilightTimers = null;
            }
        }
        //add new class
        $(params.elements).each(
            function () {
                $(this).addClass(params.cssClass);
                if (params.highlightTime > 0) {
                    UI.searchTextInColumns_previousHilightedElements =
                        UI.searchTextInColumns_previousHilightedElements.add(this);
                }
            }
        );

        if (params.highlightTime > 0) {
            //unhighlight Elements when the time passes 
            var unhParams = params;
            unhParams.elements = UI.searchTextInColumns_previousHilightedElements;
            UI.searchTextInColumns_hilightTimers =
                setTimeout(
                    function () { UI.unhighlightElements(unhParams); },
                    unhParams.highlightTime
                    );
        }
    }

    function removeFromTable(tableID, rowIndex) {
        UTILS.removeDataRowsFromTable(tableID, rowIndex);
        UTILS.updateTableRelatedFieldValue(tableID);
        UI.validateDataTable(tableID);
        //mike
        if ($('#' + tableID).data('after-remove-callback') != undefined) {
            var _callback = $('#' + tableID).data('after-remove-callback');
            var callback = eval(_callback);
            if (typeof callback == 'function') {
                callback();
            }
        }
    }

    function clearRelatedSourceFields(tableID) {
        var cols = UTILS.getTableColumnsProperties(tableID);
        for (var c in cols) {
            var col = $.parseJSON(cols[c]);
            $("#" + col.sourceField).clearForm();
        }
    }

    //get dataRow
    function sourceFieldsValuesToJson(tableID) {
        var cols = UTILS.getTableColumnsProperties(tableID);
        var tp = $.parseJSON($("#" + tableID).data('table-properties'));
        var invalidElements = $();
        var noValidationErrors = true;
        var validationMessages = [];
        var row = {};
        for (var c in cols) {
            try {
                var jsonStr = JSON.stringify(cols[c]);
                var col = $.parseJSON($.parseJSON(jsonStr));
                var colSourceField = col.sourceField;

                if (col.sourceField != undefined) { // some columns, like ids, aren't gotten from source fields.
                    var error = false;
                    var selector = "#" + col.sourceField;
                    var selectorIsValid = $(selector).valid();
                    if (selectorIsValid) {
                        var value = UTILS.getInputValueByType(col.sourceField);
                        //$(selector).is("input:checkbox") ? $(selector).is(':checked').toString() : $.trim($(selector).val());
                        if ((col.allowEmptyValues == true) || (!col.allowEmptyValues == true && value.toString() != "")) {
                            if (col.requiredIfField != undefined) {//mike
                                //col has requiredif attribute
                                if (UTILS.getInputValueByType(col.requiredIfField) == col.requiredIfValue) {
                                    if (value.toString() != '') {
                                        row[col.name] = value;
                                    }
                                    else {
                                        error = true;
                                    }
                                }
                                else {
                                    row[col.name] = value;
                                }
                            }
                            else {
                                row[col.name] = value;
                            }
                        } else {
                            error = true;
                        }

                    } else {
                        error = true;
                    }


                    if (error == true) {
                        noValidationErrors = false;
                        invalidElements = invalidElements.add($(selector));
                        validationMessages.push(UTILS.getLableFor(col.sourceField) + " is invalid. ");
                    }
                }


            } catch (e) {
                //alert("Col : " + c + " => " + e.message);
            }
        }

        if (noValidationErrors) {
            return { noValidationErrors: noValidationErrors, itemsObject: row, validationMessages: validationMessages };
        } else {
            return { noValidationErrors: noValidationErrors, itemsObject: invalidElements, validationMessages: validationMessages };
        }
        return row;
    }

    function validateJsonDataRow(tableID, row) {
        var cols = UTILS.getTableColumnsProperties(tableID);
        var tp = $.parseJSON($("#" + tableID).data('table-properties'));
        var entireRowIsValid = true;
        var isDuplicated = false;
        var validationErrors = [];
        //function generateErrors(fieldID, errorType) {
        //    var label = UTILS.getLableFor(fieldID);
        //    validationErrors += "<li>" + label;
        //    switch (errorType) {
        //        case "empty":
        //            validationErrors += " must be specified.</li>";
        //            break;
        //        case "invalid":
        //            validationErrors += " is not valid.</li>";
        //            break;
        //        default:
        //    }
        //    elementsToHilight = elementsToHilight.add($("#" + fieldID));
        //    //return;
        //}
        for (var c in cols) {
            var col = $.parseJSON(cols[c]);
            var value = row[col.name];
            var sf = col.sourceField;
            var selector = "#" + sf;
            var rowInvalid = false;
            if (col.allowEmptyValues != undefined && col.allowEmptyValues == false) {
                if (value == undefined || value.toString == "") {
                    rowInvalid = true;
                    validationErrors.push("<b>" + UTILS.getLableFor(sf) + "</b> cannot be empty");
                }
            }
            if (col.required != undefined && col.required == true) {
                if (col.initialValue != undefined && col.initialValue == value) {
                    rowInvalid = true;
                    validationErrors.push("<b>" + UTILS.getLableFor(sf) + "</b> is invalid");
                }
            }

            //if (col.requiredAndUniqueValue != undefined) {
            //    //search value in column
            //}
            //if row is still valid, validate it against its regexp
            if (!rowInvalid && col.sourceField != undefined) {
                var sourceField = $("#" + col.sourceField);
                var validationRegex = sourceField.data('val-regex-pattern') || "";
                if (validationRegex.toString() != "") {
                    var regExp = new RegExp(validationRegex);
                    if (!regExp.test(value)) {
                        rowInvalid = true;
                        validationErrors.push(UTILS.getLableFor(sf) + " is invalid");
                    }
                }
            }

            if (rowInvalid == true) {
                entireRowIsValid = false;
            }


        }


        return { entireRowIsValid: entireRowIsValid, validationErrors: validationErrors };

    }

    //validate dataRow    

    function displayDataRowErrors(elmentsWithError, errorType) {
        var validationErrors = "";
        var tipo = errorType;
        var display = this;
        elmentsWithError.each(
            function () {
                validationErrors += "<li>";
                validationErrors += UTILS.getLableFor($(this).attr("id"));
                if (tipo == "empty") { validationErrors += " cannot be empty."; }
                if (tipo == "invalid") { validationErrors += " is not valid"; }

                validationErrors += "</li>";
            }
        );
        var errors = "There were some errors:<ul>"
        errors += validationErrors
        errors += "</ul>"
        UI.displayValidationErrors(errors);

    }
    function getDataRowFromUiToTable(tableID) {
        var entireRowIsValid = true;
        var newRow = sourceFieldsValuesToJson(tableID);
        var validationErrors = [];
        validationErrors = validationErrors.concat(newRow.validationMessages);

        if (newRow.noValidationErrors == true) {
            var validation = validateJsonDataRow(tableID, newRow.itemsObject);
            if (validation.entireRowIsValid == false) {
                validationErrors = validationErrors.concat(validation.validationErrors);
                var hles = new SETTINGS.highlightElements();
                hles.elements = validation.invalidElements;
                UI.highlightElements(hles);
                entireRowIsValid = false;
            }
        } else {
            entireRowIsValid = false;
        }

        if (entireRowIsValid) {
            return { entireRowIsValid: entireRowIsValid, newRow: newRow.itemsObject }
        } else {
            return { entireRowIsValid: entireRowIsValid, validationErrors: validationErrors }
        }
    }

    function addToListTable(params) {
        var fieldID = params.fieldID;
        var labelText = UTILS.getLableFor(fieldID);

        var validateFieldId = fieldID + "s";
        var tableID = "tbl" + fieldID + "s";
        var field = $("#" + fieldID);
        var newEmail = field.val();

        if (params.validateWithDataAnnotation == true && !field.valid()) {
            UI.messageBox(-1, "The " + labelText + " is invalid.");
            return false;
        }

        if (params.allowEmptyValues == false && $.trim(newEmail) == "") {
            UI.messageBox(-1, "The " + labelText + " cannot be an empty string.");
            return false;
        }

        if (params.allowDuplicateValues == false) {
            var s = new SETTINGS.searchTextInColumns();
            s.text = newEmail;
            s.searchType = "containsExact";
            s.tableID = tableID;
            s.specifiedColumns = 1;

            var matches = UTILS.searchTextInColumns(s);
            if (matches.length > 0) {
                //var hls = new SETTINGS.highlightElements();
                //hls.elements = matches;
                //UI.highlightElements(hls);
                UI.messageBox(-1, "The " + labelText + " is duplicated.");
                return matches;
            }
        }


        var rows = new SETTINGS.newTableRows();
        //rows.options.addDeleteIcon = params.newItemOptions.deleteIconCallBack;
        //rows.options.deleteIconCallBack = params.newItemOptions.deleteIconCallBack;
        rows.options = params.newItemOptions;
        rows.dataRows = [newEmail];
        UTILS.addDataRowsToTable(tableID, rows);
        return true;

        //var params = new SETTINGS.getDataFromColumn();
        //params.tableID = tableID;
        //field.val("");
        //field.val(UTILS.getDataFromColumn(params));
        //UI.validateRelatedElement(validateFieldId, tableID);


    }

    function isDataTableValid(tableID) {
        ///check whether the dataTable is valid against its related hidden field.
        var table = $("#" + tableID);
        var tableID = tableID;
        var relatedField = table.data('related-validation-field');
        var entireTableIsValid = true;

        var jRelatedField = $("#" + relatedField);

        if (!jRelatedField.valid()) {
            return false
        }
        var rowsWithErrors = $();

        var validationErrors = 0;
        var validationErrorMessages = [];
        var params = new SETTINGS.tableDataToJason();
        params.tableID = tableID;
        params.subject = "";
        //Also make sure all the rows meet the cols rules. 
        $("#" + tableID + " tbody tr").each(function (i) {
            //get dataRow as json
            var row = UTILS.tableDataRowValuesToJson(params, i);
            var validation = validateJsonDataRow(tableID, row);
            if (validation.entireRowIsValid == false) {
                rowsWithErrors = rowsWithErrors.add(this);
                validationErrorMessages = validationErrorMessages.concat(validation.validationErrors);
                validationErrors++;
            }
        });

        //Make sure the requiredAndUniqueValue rule is met
        var columnsVerificationErrors = UTILS.verifyRequiredColumnValues(tableID);
        if (columnsVerificationErrors.length > 0) {
            entireTableIsValid = false;
        }


        if (rowsWithErrors.length > 0) {
            //var hls = new SETTINGS.highlightElements();
            //hls.elements = rowsWithErrors;
            //hls.highlightTime = -1;
            //UI.highlightElements(hls);
            entireTableIsValid = false;
        }
        return { entireTableIsValid: entireTableIsValid, rowsWithErrors: rowsWithErrors, errors: columnsVerificationErrors, validationErrors: validationErrors, validationErrorMessages: validationErrorMessages };

    }
    function validateDataTable(tableID) {
        var elements = $();
        var jqTable = $("#" + tableID);
        jqTable.find("tr").each(
            function () {
                elements = elements.add($(this));
            }
        );

        elements = elements.add(jqTable);
        var uhls = new SETTINGS.highlightElements();
        uhls.elements = elements;
        //unhiglight the table and its rows
        UI.unhighlightElements(uhls);

        //reset "elements"
        element = $();

        //Validate the dataTable against ist related field, highlighting if invalid
        var hls = new SETTINGS.highlightElements();
        //hls.elements = $("#" + tableID);
        var tableValidation;
        try {
            tableValidation = isDataTableValid(tableID);
        } catch (e) {
            alert(e.message);
        }

        if (tableValidation.entireTableIsValid == false) {
            //higlight the invalid rows
            elements = tableValidation.rowsWithErrors;
            //tableValidation.rowsWithErrors.each(
            //    function () {
            //        element
            //    }
            //    );
            elements = elements.add(jqTable);
            //higlight also the entire table
            hls.elements = elements;
            hls.highlightTime = 0;
            UI.highlightElements(hls);

        } else {
            UI.unhighlightElements(hls);
        }

        return tableValidation;


    }
    function getValidationErrorsList(frmID, additionalErrors) {
        var target = $("#" + frmID);
        var errorsHolder = "#" + frmID + "_ErrorMessages";
        var validationErrorsList = "";
        //get validationMessages when @Html.ValidationSummary(false)            
        var selector = errorsHolder + " div.validation-summary-errors";
        if ($(selector).length > 0) {
            validationErrorsList += $(selector).html();
        }
        else {
            validationErrorsList = UI.getPartialValidationErrors(frmID);
        }

        if (additionalErrors) {
            var htmlStr = $(validationErrorsList);
            var listItems = htmlStr.html();
            for (var e in additionalErrors) {
                var newLi = $("<li>");
                listItems += "<li>" + additionalErrors[e] + "</li>";
            }
            validationErrorsList = "<ul>" + listItems + "</ul>";
        }

        return validationErrorsList;
    }
    function getPartialValidationErrors(containerID) {
        var frmID = "";
        if (!$("#" + containerID).is("form")) {
            frmID = $("#" + containerID).parents("form").attr("id");
        } else { frmID = containerID; }

        var errorsHolder = "#" + frmID + "_ErrorMessages";

        var validationErrorsList = "";
        validationErrorsList += "<ul>";
        $(errorsHolder + " > span").each(
            function (index) {
                validationErrorsList += "<li>";
                validationErrorsList += UTILS.stripTags($(this).html());
                validationErrorsList += "</li>";
            });
        validationErrorsList += "</ul>"
        return validationErrorsList;
    }
    function displayValidationErrors(validationErrorsList) {
        var _stripedStr = UTILS.stripTags(validationErrorsList);

        if (_stripedStr.length > 0) {
            var message = "There were some validation errors:<ul>" + validationErrorsList + "</ul>";
            validationErrorsList = validationErrorsList.replace('<li></li>', '');
            UI.messageBox(-1, message, 5);
        }
    }
    function relatedHiddenFieldDataToTable() {

    }
    function confirmBox(confirmMessage, onAcceptCallBack, onAcceptParams) {
        /// <summary>
        /// Prompts the User for a confirmation of an action.
        /// </summary>
        /// <param name="confirmMessage">The message to display.</param>
        /// <param name="onAcceptCallBack">The function to execute when user confirm.</param>
        /// <param name="onAcceptParams">The parameters for onAcceptCallBack. If onAcceptCallBack is an anonymous function this parameter is not used</param>
        /// <returns></returns>
        var cbs = new SETTINGS.confirmBox();
        cbs.message = confirmMessage;
        cbs.onAcceptCallBack = onAcceptCallBack;
        cbs.onAcceptParams = onAcceptParams;
        UI.messageBox(cbs);
    }

    function twoActionBox(message, firstActionCallBack, firstActionParams, firstButtonValue, secondActionCallBack, secondActionParams, secondButtonValue) {
        var tab = new SETTINGS.twoActionBox();
        //tab.boxType = type;
        tab.message = message;
        tab.onAcceptCallBack = firstActionCallBack;
        tab.onAcceptParams = firstActionParams;
        tab.onAcceptButtonValue = firstButtonValue;
        tab.onCancelCallBack = secondActionCallBack;
        tab.onCancelParams = secondActionParams;
        tab.onCancelButtonValue = secondButtonValue;
        UI.messageBox(tab);
    }

    function confirmRemoveFromTable() {
        //args: [0]="tableID", [1]="rowIndex"

        //if editing, cancel editing before deleting
        UI.cancelDataRowEditing(arguments[0]);

        var itemName = $("#" + arguments[0]).data('items-name');
        var confirmationText = "You are about to delete a";
        if (UTILS.startsWithVowel(itemName)) {
            confirmationText += "n";
        }
        confirmationText += " <strong>" + itemName + "</strong>"

        //get all the text for the colums to warn on deleting
        var dataRow = UTILS.dataRowWarningTextsToJson(arguments[0], arguments[1]);
        if (Object.keys(dataRow).length > 0) {
            confirmationText += ":<ul>";
            for (var x in dataRow) {
                confirmationText += "<li><strong>" + x + "</strong> : " + dataRow[x] + "</li>";
            }
            confirmationText += "</ul>";
        } else { confirmationText += ".</br>"; }
        confirmationText += "Do you confirm you want to proceed?";
        UI.confirmBox(confirmationText, removeFromTable, arguments);
    }

    function addDataRow(jObj) {
        var tableID = jObj.data('table-group');
        var row = UI.getDataRowFromUiToTable(tableID);
        var ok = true;
        var strMessages = [];
        if (row.entireRowIsValid == true) {
            var tp = $.parseJSON($("#" + tableID).data('table-properties'));
            if (tp.allowDuplicates == false) {
                var duplicates = UTILS.findDuplicatedDataRows(tableID, row.newRow);
                if (duplicates.length > 0) {
                    var hls = new SETTINGS.highlightElements();
                    hls.elements = duplicates;
                    UI.highlightElements(hls);
                    strMessages.push("The row is duplicated.");
                    ok = false;
                }
            }

            var requiredAndUniqueValueMatches = UTILS.verifyUniqueAndRequiredColumnValues(tableID, row.newRow);
            if (requiredAndUniqueValueMatches.errores.length > 0) {
                var hls = new SETTINGS.highlightElements();
                hls.elements = duplicates;
                UI.highlightElements(hls);
                for (var x in requiredAndUniqueValueMatches.errores) {
                    var error = requiredAndUniqueValueMatches.errores[0];
                    strMessages.push("The field <b>" + UTILS.getLableFor(error.sourceField) + "</b> its been already set to <b>" + error.value + "</b>; You can have this value only once.");
                }
                ok = false;
            }

            var requiredAndUniqueMatches = UTILS.verifyUniqueAndRequiredColumns(tableID, row.newRow);
            if (requiredAndUniqueMatches.errores.length > 0) {
                var hls = new SETTINGS.highlightElements();
                hls.elements = duplicates;
                UI.highlightElements(hls);
                //var strMessages = "";
                for (var x in requiredAndUniqueMatches.errores) {
                    var error = requiredAndUniqueMatches.errores[0];
                    strMessages.push("The field <b>" + UTILS.getLableFor(error.sourceField) + "</b> its been already set to <b>" + error.value + "</b>; You can have this value only once.");
                }
                ok = false;
            }

            if (ok == true) {
                UTILS.addJsonRowToTable(tableID, row.newRow);
                UTILS.updateTableRelatedFieldValue(tableID);
                UI.clearRelatedSourceFields(tableID);
            }

            var tableValidation = UI.validateDataTable(tableID);
            if (tableValidation.validationErrorMessages != undefined && tableValidation.validationErrorMessages.length > 0) {
                // ok = false;

                strMessages = strMessages.concat(tableValidation.validationErrorMessages);

                //UI.displayValidationErrors(UTILS.arrayToListItems());
            }


            if (strMessages.length > 0) {
                UI.displayValidationErrors(UTILS.arrayToListItems(strMessages));
            }

        }
        else {
            UI.displayValidationErrors(UTILS.arrayToListItems(row.validationErrors));
        }
    }
    function editDataRow(tableID, rowIndex) {
        //hide all the buttons
        UI.hideDataTableActionButtons(tableID);
        //show only update button
        var btns = OPTIONS.dataTable.actionButtons;
        var jsonRowProperties = JSON.parse($('#' + tableID).data('rows-properties'));
        if (jsonRowProperties.blockEditOnSavedRows == true) {
            if ($('#' + tableID + ' tbody tr:eq(' + rowIndex + ')').children('td:first').data('value') == '') {
                $('[data-table-group="' + tableID + '"][data-table-button-type="' + btns.update + '"]')
                .show();
            }
        }
        else {
            $('[data-table-group="' + tableID + '"][data-table-button-type="' + btns.update + '"]')
                .show();
        }
        $('[data-table-group="' + tableID + '"][data-table-button-type="' + btns.cancel + '"]')
        .show();


        var dataColumns = UTILS.getAllTableDataColumnReferences(tableID);
        var rowValues = UTILS.dataRowValuesToJson(tableID, rowIndex);
        //console.log(dataColumns);
        dataColumns.each(function () {
            var columJsonData = $.parseJSON($(this).data('column-properties'));
            var dataSourceFieldName = columJsonData.sourceField;
            var columnName = columJsonData.name;
            var cellValue = rowValues[columnName];
            UTILS.setInputValueByType(dataSourceFieldName, cellValue);
            //$("#" + dataSourceFieldName).val(cellValue);
        });
        UI.setSelectedRow(tableID, rowIndex);
    }

    function setSelectedRow(tableID, rowIndex) {
        //deselectAllRows
        UI.deselectAllSelectedRows(tableID);

        //select the specified row
        $("#" + tableID + " tbody tr:eq(" + rowIndex + ")").addClass('selected-row');
    }
    function getSelectedRow(tableID) {
        return $("#" + tableID + " tbody tr.selected-row").index();
    }

    function deselectAllSelectedRows(tableID) {
        $("#" + tableID).find("tr").each(function () {
            $(this).removeClass('selected-row');
        });
    }
    function updateDataRow(jObj) {
        var tableID = jObj.data('table-group');

        var rowIndex = $("#" + tableID + " tbody tr.selected-row").index();
        var row = UI.getDataRowFromUiToTable(tableID);
        var ok = true;
        var strMessages = [];
        // validate duplicates

        if (row.entireRowIsValid != false) {
            var tp = $.parseJSON($("#" + tableID).data('table-properties'));
            var requiredAndUniqueValueMatches = UTILS.verifyUniqueAndRequiredColumnValues(tableID, row.newRow);
            var requiredAndUniqueMatches = UTILS.verifyUniqueAndRequiredColumns(tableID, row.newRow);

            if (tp.allowDuplicates == false) {
                var duplicates = UTILS.findDuplicatedDataRows(tableID, row.newRow);
                if (duplicates.length > 0) {
                    var hls = new SETTINGS.highlightElements();
                    hls.elements = duplicates;
                    UI.highlightElements(hls);
                    strMessages.push("The row is duplicated");
                    ok = false;
                }
            }

            if (requiredAndUniqueValueMatches.errores.length > 0) {
                var hls = new SETTINGS.highlightElements();
                hls.elements = duplicates;
                UI.highlightElements(hls);
                for (var x in requiredAndUniqueValueMatches.errores) {
                    var error = requiredAndUniqueValueMatches.errores[0];
                    strMessages.push("The column <b>" + UTILS.getLableFor(error.sourceField) + "</b> its been already set to <b>" + error.value + "</b>; You can have this value only once.");
                    ok = false;
                }
            }

            if (requiredAndUniqueMatches.errores.length > 0) {
                var hls = new SETTINGS.highlightElements();
                hls.elements = duplicates;
                UI.highlightElements(hls);
                for (var x in requiredAndUniqueMatches.errores) {
                    var error = requiredAndUniqueMatches.errores[0];
                    strMessages.push("The column <b>" + UTILS.getLableFor(error.sourceField) + "</b> its been already set to <b>" + error.value + "</b>; You can have this value only once.");
                    ok = false;
                }
            }


            if (ok == true) {
                //hide all the buttons
                UI.hideDataTableActionButtons(tableID);
                //show only add button
                var btns = OPTIONS.dataTable.actionButtons;
                $('[data-table-group="' + tableID + '"][data-table-button-type="' + btns.add + '"]')
                .show();
                UTILS.updateJsonRowToTable(tableID, rowIndex, row.newRow);
                UTILS.updateTableRelatedFieldValue(tableID);
                //reset the selected row
                $("#" + tableID + " tbody tr.selected-row").removeClass('selected-row');
                UI.clearRelatedSourceFields(tableID);
            }

            var tableValidation = UI.validateDataTable(tableID);
            if (tableValidation.validationErrorMessages.length > 0) {
                strMessages = strMessages.concat(tableValidation.validationErrorMessages);
            }

            if (strMessages.length > 0) {
                UI.displayValidationErrors(UTILS.arrayToListItems(strMessages));
            }

        }
        else {
            var validationErrors = [];
            if (row.validationErrors == undefined) {
                var itemsName = $("#" + tableID).data('items-name');
                validationErrors.push("The " + itemsName + " is invalid.");
            } else {
                validationErrors = row.validationErrors;
            }

            UI.displayValidationErrors(UTILS.arrayToListItems(validationErrors));
        }
    }
    function cancelDataRowEditing(jObj) {
        var tableID = ($.type(jObj) === "string") ? jObj : jObj.data('table-group');
        //hide all the buttons
        UI.hideDataTableActionButtons(tableID);
        var btns = OPTIONS.dataTable.actionButtons;
        $('[data-table-group="' + tableID + '"][data-table-button-type="' + btns.add + '"]')
        .show();
        UI.clearRelatedSourceFields(tableID);
        UI.deselectAllSelectedRows(tableID);
    }
    function hideDataTableActionButtons(tableID) {
        $('[data-table-group="' + tableID + '"]').each(function () { $(this).hide(); });
    }

    function hideAllDataTablesActionButtons() {
        //hide all data table action buttons;
        $('[data-table-group]').each(function () { $(this).hide(); });
    }
    function showAllDataTablesAddButtons() {
        //Show all data table add buttons
        var btns = OPTIONS.dataTable.actionButtons;
        $('[data-table-button-type="' + btns.add + '"]').each(function () { $(this).show(); });
    }
    function validateTableDataRows() {
        $('data-related-validation-field').each(
            function () {

            }
        );
    }
    //console.log('declaration onterminalschange');
    var onTerminalsChanged = function () {
        //console.log('excecution onterminalschange')
        $(document).find('.selected-row').each(function () {
            var event = $.Event('keydown');
            event.keyCode = 27;
            $(document).trigger(event);
        });
        $('#Search_Terminals').val(UI.selectedTerminals);
        if (UI.loadFlag == true | UI.loadFlag == undefined) {
            $('.terminals-dependent:not(.avoid-trigger-on-load)').trigger('click');
            $(document).trigger('click');
        }
        else {
            $('.terminals-dependent').trigger('click');
        }
        $('.terminal-dependent-list').trigger('loaded');
        UI.loadFlag = false;
    }

    var alertDoubleClick = function () {
        $('.alert-dblclick').on('dblclick', function (e) {
            UI.notifyDoubleClick($(this).attr('id'));
        });
    }

    var applyCKEditor = function (formName) {
        if (formName != undefined) {
            $('#' + formName).find('textarea').each(function () {
                $(this).val('');
                $(this).ckeditor();
            });
        }
        else {
            $(document).find('textarea').each(function () {
                $(this).val('');
                $(this).ckeditor();
            });
        }
    }

    var expandFirstFieldset = function () {
        $(document).ready(function () {
            UI.expandFieldset($('#main').find('fieldset').first().attr('id'));
        });
    }

    var getItemDestination = function (itemType) {
        var column;
        var destination = '';
        $('#tblSearch' + itemType + 'Results > thead > tr > th').each(function () {
            if ($(this)[0].textContent.trim().indexOf('Destination') >= 0)
                column = parseInt($(this).index());
        });
        $('#tblSearch' + itemType + 'Results').find('.selected-row').children('td').each(function (index) {
            if (index == column)
                destination = $(this)[0].textContent;
        });
        if (destination == '') {
            $('#fds' + itemType + 'Info').find('form').first().find('select').each(function (index, item) {
                if ($(this).attr('name').indexOf('Destination') > 0) {
                    destination = $(this).children('option:selected').text();
                }
            });
        }
        return destination;
    }

    var deleteDataTableItemFunction = function (url, itemType, $target, oTable, callback, callbackParams) {
        var itemID = $target.parents(itemType).first().attr('id');
        UI.confirmBox('Do you confirm you want to proceed?', deleteDataTableItem, [url, itemType, itemID.substr(0, itemID.indexOf('_')), itemID.substr(itemID.indexOf('_') + 1, itemID.length - itemID.indexOf('_')), oTable, callback, callbackParams]);
    }

    function deleteDataTableItem(url, tag, item, itemID, oTable, callback, params) {
        $.ajax({
            url: url,
            type: 'POST',
            cache: false,
            data: { targetID: itemID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#' + item + '_' + itemID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    oTable.fnDeleteRow($('#' + item + '_' + itemID)[0]);
                    //personalized only for tr and li
                    if (tag == 'tr') {
                        UI.tablesHoverEffect();
                        UI.tablesStripedEffect();
                    }
                    else {
                        UI.ulsHoverEffect();
                    }
                    if (callback != undefined) {
                        if (params != undefined) {
                            callback(params.url, params.itemType, params.itemID);
                        }
                        else {
                            callback();
                        }
                    }
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    var updateDependentLists = function (url, itemType, itemID, defaultFlag, container, insertFirstOption) {
        $.getJSON(url, { itemID: itemID != undefined ? itemID : 0, itemType: itemType }, function (data) {
            var $container = container != undefined ? container : $(document);
            $container.find('.' + itemType + '-dependent-list').each(function (e) {
                var selected = $('#' + $(this).attr('id')).val();
                var value = $('#' + $(this).attr('id') + ' option:first').val();
                var text = $('#' + $(this).attr('id') + ' option:first').text();

                $(this).fillSelect(data);
                if ((value == '0' || value == '' || value == 'null') && insertFirstOption == undefined) {
                    if (defaultFlag == undefined || defaultFlag == true) {
                        $(this).prepend('<option value="' + value + '" selected="selected">' + '--Select One--' + '</option>');
                    }
                    else {
                        if ($(this).find('option[value="' + value + '"]').length == 0) {
                            $(this).prepend('<option value="' + value + '" selected="selected">' + text + '</option>');
                        }
                    }
                }
                $('#' + $(this).attr('id')).val(selected);
            });
        });
    }

    var clearSelectedRowContent = function () {
        $(document).find('.selected-row').each(function () {
            var event = $.Event('keydown');
            event.keyCode = 27;
            $(document).trigger(event);
        });
    }

    var disableFieldWithClass = function () {
        $('.field-disabled').each(function (e) {
            $(this).attr('readonly', 'readonly');
        });
    }

    var updateListsOnTerminalsChangeON = false;
    var updateListsOnTerminalsChange = function (selectorID, _callback) {
        if (updateListsOnTerminalsChangeON == false) {
            updateListsOnTerminalsChangeON = true;
            //console.log('exc updateListsOnTerminalsChange');

            $('.terminal-dependent-list').off('loaded').on('loaded', function (e) {
                if ($(this).hasClass('execute-callback')) {
                    //console.log('hascallback');
                    var callback = new Function($(this).attr('data-callback'));
                    callback();
                }
                else {
                    //console.log('no hascallback');
                    $(this).clearSelect();
                    //$(this).each(function (index, item) {
                    var id = $(this).attr('id');
                    var route = $(this).attr('data-route');
                    var parameter = $(this).attr('data-route-parameter');
                    $.getJSON(route, { itemID: 0, itemType: parameter }, function (data) {
                        //console.log('cambio de valor el terminaldependentlist');
                        $('#' + id).fillSelect(data);
                        try {
                            $('#' + id).multiselect('refresh');
                        }
                        catch (ex) { }
                        if (selectorID != undefined && id == selectorID) {
                            if (_callback != undefined) {
                                if (typeof _callback == 'function') {
                                    _callback();
                                }
                            }
                        }
                        //if (_callback != undefined) {
                        //    if (typeof _callback == 'function') {
                        //        _callback();
                        //    }
                        //}
                    });
                }
            });
        }
    }

    var addDecimalPart = function (stringNumber) {
        if (!isNaN(stringNumber)) {
            var _string = parseFloat(stringNumber).toString().indexOf('.') == -1 ? (parseFloat(stringNumber).toString() + '.00') : Number(stringNumber).toFixed(2);
            return _string;
        }
        else {
            return stringNumber;
        }
    }

    var validateStartEndDateTimes = function (toDate, fromDate, isTodayExpired) {
        if (fromDate != undefined) {
            var fDate = fromDate.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2}) (AM|PM)/),
                year = fDate[1],
                month = fDate[2],
                day = fDate[3];
            hour = fDate[4],
            min = fDate[5],
            ap = fDate[7];
            if (hour == '12') hour = '0';
            if (ap.toLowerCase() == 'pm') hour = parseInt(hour, 10) + 12;
            if (year.length == 2) {
                if (parseInt(year, 10) < 70) year = '20' + year;
                else year = '19' + year;
            }
            if (month.length == 1) month = '0' + month;
            if (day.length == 1) day = '0' + day;
            if (hour.length == 1) hour = '0' + hour;
            if (min.length == 1) min = '0' + min;
            var startDate = year + month + day + hour + min;
        }
        else {
            var date = new Date();
            var cYear = date.getFullYear().toString();
            var cMonth = date.getMonth();
            cMonth = cMonth + 1;
            cMonth = cMonth.toString();
            var cDay = date.getDate().toString();
            var cHour = date.getHours().toString();
            var cMin = date.getMinutes().toString();
            if (cYear.length == 2) {
                if (parseInt(cYear, 10) < 70) cYear = '20' + cYear;
                else cYear = '19' + cYear;
            }
            if (cMonth.length == 1) cMonth = '0' + cMonth;
            if (cDay.length == 1) cDay = '0' + cDay;
            if (cHour.length == 1) cHour = '0' + cHour;
            if (cMin.length == 1) cMin = '0' + cMin;
            var startDate = cYear + cMonth + cDay + cHour + cMin;
        }
        var tDate = toDate.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2}) (AM|PM)/),
            fyear = tDate[1],
            fmonth = tDate[2],
            fday = tDate[3];
        fhour = tDate[4],
        fmin = tDate[5],
        fap = tDate[7];
        if (fhour == '12') fhour = '0';
        if (fap.toLowerCase() == 'pm') fhour = parseInt(fhour, 10) + 12;
        if (fyear.length == 2) {
            if (parseInt(fyear, 10) < 70) fyear = '20' + fyear;
            else fyear = '19' + fyear;
        }
        if (fmonth.length == 1) fmonth = '0' + fmonth;
        if (fday.length == 1) fday = '0' + fday;
        if (fhour.length == 1) fhour = '0' + fhour;
        if (fmin.length == 1) fmin = '0' + fmin;
        var endDate = fyear + fmonth + fday + fhour + fmin;
        if (endDate < startDate) {
            return true;
        }
        else {
            if (isTodayExpired != undefined && isTodayExpired && endDate == startDate) {
                return true;
            }
            return false;
        }
    }

    var validateStartEndDates = function (toDate, fromDate, isTodayExpired) {
        if (fromDate != undefined) {
            //var fDate = fromDate.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2}) (AM|PM)/),
            var fDate = fromDate.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2})/),
                year = fDate[1],
                month = fDate[2],
                day = fDate[3];
            //hour = fDate[4],
            //min = fDate[5],
            //ap = fDate[7];
            //if (hour == '12') hour = '0';
            //if (ap == 'p.m.') hour = parseInt(hour, 10) + 12;
            if (year.length == 2) {
                if (parseInt(year, 10) < 70) year = '20' + year;
                else year = '19' + year;
            }
            if (month.length == 1) month = '0' + month;
            if (day.length == 1) day = '0' + day;
            //if (hour.length == 1) hour = '0' + hour;
            //if (min.length == 1) min = '0' + min;
            //var startDate = year + month + day + hour + min;
            var startDate = year + month + day;
        }
        else {
            var date = new Date();
            var cYear = date.getFullYear().toString();
            var cMonth = date.getMonth();
            cMonth = cMonth + 1;
            cMonth = cMonth.toString();
            var cDay = date.getDate().toString();
            //var cHour = date.getHours().toString();
            //var cMin = date.getMinutes().toString();
            if (cYear.length == 2) {
                if (parseInt(cYear, 10) < 70) cYear = '20' + cYear;
                else cYear = '19' + cYear;
            }
            if (cMonth.length == 1) cMonth = '0' + cMonth;
            if (cDay.length == 1) cDay = '0' + cDay;
            //if (cHour.length == 1) cHour = '0' + cHour;
            //if (cMin.length == 1) cMin = '0' + cMin;
            //var startDate = cYear + cMonth + cDay + cHour + cMin;
            var startDate = cYear + cMonth + cDay;
        }
        //var tDate = toDate.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2}) (AM|PM)/),
        var tDate = toDate.match(/(\d{2,4})\-(\d{1,2})\-(\d{1,2})/),
            fyear = tDate[1],
            fmonth = tDate[2],
            fday = tDate[3];
        //fhour = tDate[4],
        //fmin = tDate[5],
        //fap = tDate[7];
        //if (fhour == '12') fhour = '0';
        //if (fap == 'p.m.') fhour = parseInt(fhour, 10) + 12;
        if (fyear.length == 2) {
            if (parseInt(fyear, 10) < 70) fyear = '20' + fyear;
            else fyear = '19' + fyear;
        }
        if (fmonth.length == 1) fmonth = '0' + fmonth;
        if (fday.length == 1) fday = '0' + fday;
        //if (fhour.length == 1) fhour = '0' + fhour;
        //if (fmin.length == 1) fmin = '0' + fmin;
        //var endDate = fyear + fmonth + fday + fhour + fmin;
        var endDate = fyear + fmonth + fday;
        if (endDate < startDate) {
            return true;
        }
        else {
            if (isTodayExpired != undefined && isTodayExpired && endDate == startDate) {
                return true;
            }
            return false;
        }
    }

    var showCommentsOnHover = function (oTable) {
        oTable.$('.comments').hover(function () {
            $(this).find('.comment').show();
        }, function () {
            $(this).find('.comment').hide();
        });
    }

    var addExtras = function () {
        $('.show-detail').on('click', function () {
            $('.cell-detail').slideUp('fast').parent().css('background-color', 'transparent');

            if ($(this).find('.cell-detail').is(':visible')) {
                $(this).find('.cell-detail').slideUp();
                $(this).css('background-color', 'transparent');
            } else {
                if ($(this).find('.cell-detail').length > 0) {
                    $(this).find('.cell-detail').slideDown();
                    $(this).css('background-color', 'rgb(249, 249, 199)');
                }
            }
        });

        $('.show-tbody').on('click', function () {
            var tbody = $(this).parent().parent().parent().siblings('tbody').get(0);
            if ($(tbody).is(':visible')) {
                $(tbody).slideUp('fast');
                $(this).html('+ Show Details');
            } else {
                $(tbody).slideDown('fast');
                $(this).html('- Hide Details');
            }
        });

        $('.comments').hover(function () {
            $(this).find('.comment').show();
        }, function () {
            $(this).find('.comment').hide();
        });

        UI.applyFormat('currency');
    }

    var checkForPendingRequests = function (xhr) {
        if (UI.pendingRequest && UI.pendingRequest.readyState != 4) {
            UI.pendingRequest.abort();
        }
        UI.pendingRequest = xhr;
    }

    var browserInfo = function () {
        var nVer = navigator.appVersion;
        var nAgt = navigator.userAgent;
        var browserName = navigator.appName;
        var fullVersion = '' + parseFloat(navigator.appVersion);
        var majorVersion = parseInt(navigator.appVersion, 10);
        var nameOffset, verOffset, ix;

        // In Opera, the true version is after "Opera" or after "Version"
        if ((verOffset = nAgt.indexOf("Opera")) != -1) {
            browserName = "Opera";
            fullVersion = nAgt.substring(verOffset + 6);
            if ((verOffset = nAgt.indexOf("Version")) != -1)
                fullVersion = nAgt.substring(verOffset + 8);
        }
            // In MSIE, the true version is after "MSIE" in userAgent
        else if ((verOffset = nAgt.indexOf("MSIE")) != -1) {
            browserName = "Microsoft Internet Explorer";
            fullVersion = nAgt.substring(verOffset + 5);
        }
            // In Chrome, the true version is after "Chrome" 
        else if ((verOffset = nAgt.indexOf("Chrome")) != -1) {
            browserName = "Chrome";
            fullVersion = nAgt.substring(verOffset + 7);
        }
            // In Safari, the true version is after "Safari" or after "Version" 
        else if ((verOffset = nAgt.indexOf("Safari")) != -1) {
            browserName = "Safari";
            fullVersion = nAgt.substring(verOffset + 7);
            if ((verOffset = nAgt.indexOf("Version")) != -1)
                fullVersion = nAgt.substring(verOffset + 8);
        }
            // In Firefox, the true version is after "Firefox" 
        else if ((verOffset = nAgt.indexOf("Firefox")) != -1) {
            browserName = "Firefox";
            fullVersion = nAgt.substring(verOffset + 8);
        }
            // In most other browsers, "name/version" is at the end of userAgent 
        else if ((nameOffset = nAgt.lastIndexOf(' ') + 1) <
                  (verOffset = nAgt.lastIndexOf('/'))) {
            browserName = nAgt.substring(nameOffset, verOffset);
            fullVersion = nAgt.substring(verOffset + 1);
            if (browserName.toLowerCase() == browserName.toUpperCase()) {
                browserName = navigator.appName;
            }
        }
        // trim the fullVersion string at semicolon/space if present
        if ((ix = fullVersion.indexOf(";")) != -1)
            fullVersion = fullVersion.substring(0, ix);
        if ((ix = fullVersion.indexOf(" ")) != -1)
            fullVersion = fullVersion.substring(0, ix);

        majorVersion = parseInt('' + fullVersion, 10);
        if (isNaN(majorVersion)) {
            fullVersion = '' + parseFloat(navigator.appVersion);
            majorVersion = parseInt(navigator.appVersion, 10);
        }

        var BrowserInfo = {
            browserName: browserName,
            fullVersion: fullVersion,
            majorVersion: majorVersion
        }

        return BrowserInfo;
    }

    var Chat = function () {
        var chatWith = function (userid, name) {

            $('#spnChattingWith').html(name);
            $('#hdnChatWith').val(userid);
            UI.Chat.loadChatMessages();
            $('#pnlSlideUsers').animate({
                marginLeft: '-240px'
            }, 'fast');
            $('[data-chat-id="' + userid + '"]').removeClass('pending-messages');
            //revisar si ya no hay mensajes sin leer para quitar el pending chats
            var unreadmessages = 0;
            var Chat = eval('(' + localStorage.Eplat_Chat + ')');
            if (Chat == undefined) {
                Chat = [];
            }
            $.each(Chat, function (u, user) {
                $.each(user.Messages, function (m, message) {
                    if (message.ReadDateTime == null) {
                        unreadmessages++;
                    }
                });
            });

            if (unreadmessages == 0) {
                $('#iconUsers').removeClass('pending-chats');
            }

            $('.chat-close').off('click').on('click', function () {
                $('#pnlSlideUsers').animate({
                    marginLeft: '0px'
                }, 'fast');
                $('#pnlConversation').html('');
            });
            $('.chat-clear').off('click').on('click', function () {
                clearChatWith(userid);
            });
        }

        function clearChatWith(userid) {
            var Chat = eval('(' + localStorage.Eplat_Chat + ')');
            $.each(Chat, function (u, user) {
                if (user.UserID == userid) {
                    user.Messages = [];
                }
            });
            localStorage.Eplat_Chat = $.toJSON(Chat);
            $('#pnlConversation').html('');
        }

        var loadChatMessages = function () {
            var Chat = eval('(' + localStorage.Eplat_Chat + ')');
            if (Chat == undefined) {
                Chat = [];
            }
            $.each(Chat, function (u, user) {
                if (user.UserID == $('#hdnChatWith').val()) {
                    $.each(user.Messages, function (m, message) {
                        if ($('#' + message.MessageID).length == 0) {
                            var chatmessage = '<span id="' + message.MessageID + '" class="message' + (message.FromUserID == $('#uid').val() ? ' from-me' : ' from-other') + '">' + setEmoticons(message.Message) + '<span class="message-time">' + message.DateTime + '</span></span>';
                            $('#pnlConversation').append(chatmessage);
                            var today = new Date();
                            var dd = today.getDate();
                            var MM = today.getMonth() + 1; //January is 0!
                            var yyyy = today.getFullYear();
                            var hh = today.getHours();
                            var mm = today.getMinutes();
                            var ss = today.getSeconds();
                            if (dd < 10) {
                                dd = '0' + dd
                            }
                            if (MM < 10) {
                                MM = '0' + MM
                            }
                            var strtoday = yyyy + '-' + MM + '-' + dd + ' ' + hh + ':' + mm + ':' + ss;
                            message.ReadDateTime = strtoday;
                        }
                    });
                }
            });
            localStorage.Eplat_Chat = $.toJSON(Chat);
            var pnl = $('#pnlConversation');
            var height = pnl[0].scrollHeight;
            pnl.animate({ scrollTop: height }, 500);
        }

        var checkPendingMessages = function () {
            var Chat = eval('(' + localStorage.Eplat_Chat + ')');
            if (Chat == undefined) {
                Chat = [];
            }
            var unreadmessages = 0;
            $.each(Chat, function (u, user) {
                $.each(user.Messages, function (m, message) {
                    if (message.ReadDateTime == null) {
                        unreadmessages++;
                        if (!$('[data-chat-id="' + user.UserID + '"]').hasClass('pending-messages')) {
                            $('[data-chat-id="' + user.UserID + '"]').addClass('pending-messages');
                        }
                    }
                });
            });

            if (unreadmessages == 0) {
                $('#iconUsers').removeClass('pending-chats');
            } else {
                $('#iconUsers').addClass('pending-chats');
            }
        }

        function setEmoticons(strMensaje) {
            strMensaje = strMensaje.replace(":)", "<img src=\"/images/msn/regular_smile.gif\" />");
            strMensaje = strMensaje.replace(/:D|:d/g, "<img src=\"/images/msn/teeth_smile.gif\" />");
            strMensaje = strMensaje.replace(/=D|=d/g, "<img src=\"/images/msn/teeth_smile.gif\" />");
            strMensaje = strMensaje.replace(";)", "<img src=\"/images/msn/wink_smile.gif\" />");
            strMensaje = strMensaje.replace(/:-O|:-o|:o|:O/g, "<img src=\"/images/msn/omg_smile.gif\" />");
            strMensaje = strMensaje.replace(/:p|:P/g, "<img src=\"/images/msn/tounge_smile.gif\" />");
            strMensaje = strMensaje.replace(/\(H\)|\(h\)/g, "<img src=\"/images/msn/shades_smile.gif\" />");
            strMensaje = strMensaje.replace(":@", "<img src=\"/images/msn/angry_smile.gif\" />");
            strMensaje = strMensaje.replace(/:s|:S/g, "<img src=\"/images/msn/confused_smile.gif\" />");
            strMensaje = strMensaje.replace(":$", "<img src=\"/images/msn/embaressed_smile.gif\" />");
            strMensaje = strMensaje.replace(":(", "<img src=\"/images/msn/sad_smile.gif\" />");
            strMensaje = strMensaje.replace(":&#39;(", "<img src=\"/images/msn/cry_smile.gif\" />");
            strMensaje = strMensaje.replace(":|", "<img src=\"/images/msn/whatchutalkingabout_smile.gif\" />");
            strMensaje = strMensaje.replace(/\(a\)|\(A\)/g, "<img src=\"/images/msn/angel_smile.gif\" />");
            strMensaje = strMensaje.replace(/\(y\)|\(Y\)/g, "<img src=\"/images/msn/thumbs_up.gif\" />");
            strMensaje = strMensaje.replace(/\(n\)|\(N\)/g, "<img src=\"/images/msn/thumbs_down.gif\" />");
            return strMensaje;
        }

        return {
            chatWith: chatWith,
            loadChatMessages: loadChatMessages,
            checkPendingMessages: checkPendingMessages
        }
    }();

    var Notifications = function () {
        var load = function () {
            var Notifications = eval('(' + localStorage.Eplat_Notifications + ')');
            if (Notifications == undefined) {
                Notifications = [];
            }
            $.each(Notifications, function (n, notification) {
                if ($('#' + notification.NotificationID).length == 0) {
                    var notificationHtml = '<div id="' + notification.NotificationID + '" class="notification' + (notification.Important ? ' notification-important' : '') + '"><input type="checkbox" class="notification-close" />'
                    + (localStorage.Eplat_SelectedRole == '87e4708c-14fb-426b-a69b-05f28fc5dcfc' ? '<span class="notification-delete btn-close"></span>' : '')
                    + '<span class="notificaion-text">' + notification.Notification + '</span><span class="notification-from">Published by ' + notification.From + ' on ' + notification.Date + '</span></div>';
                    $('#pnlNotifications').prepend(notificationHtml);
                    if (!$('#iconNotifications').hasClass('pending-notifications')) {
                        $('#iconNotifications').addClass('pending-notifications');
                    }
                }
            });

            $('.notification-close').off('click').on('click', function () {
                var notificationid = $(this).parent().attr('id');
                var Notifications = eval('(' + localStorage.Eplat_Notifications + ')');
                if (Notifications == undefined) {
                    Notifications = [];
                }
                var notificationsarr = [];
                $.each(Notifications, function (i, noti) {
                    if (noti.NotificationID != notificationid) {
                        notificationsarr.push(noti);
                    }
                })
                Notifications = notificationsarr;
                if (Notifications.length == 0) {
                    $('#iconNotifications').removeClass('pending-notifications');
                }
                $(this).parent().slideUp('fast', function () {
                    $(this).remove();
                    localStorage.Eplat_Notifications = $.toJSON(Notifications);
                });
            });

            $('.notification-delete').off('click').on('click', function () {
                var notificationid = $(this).parent().attr('id');
                ePlatHub.server.deleteNotification(notificationid);
            });
        }

        var desktopNotification = function (title, body, chatuserid, chatusername) {
            if (Notification) {
                if (Notification.permission !== "granted") {
                    Notification.requestPermission();
                }

                var notification = new Notification(title, {
                    icon: 'http://eplat.villagroup.com/images/eplat_logo.png',
                    body: body
                });

                notification.onclick = function () {
                    window.focus();
                    if (typeof chatuserid != 'undefined') {
                        UI.Chat.chatWith(chatuserid, chatusername);
                    }
                }
            }
        }

        var workingOn = function (title) {
            if (title != undefined) {
                document.title = "ePlat > " + title;
            }
            if (ePlatHubConnected && $('#uid').val() != "") {
                var browserInfo = UI.browserInfo();
                var dataObject = {
                    UserID: $('#uid').val(),
                    Browser: browserInfo.browserName,
                    BrowserVersion: browserInfo.fullVersion,
                    Url: window.location.href,
                    Title: document.title + ' : ' + (UI.windowFocus ? 'Active' : 'Inactive'),
                    SelectedTerminalIDs: localStorage.Eplat_SelectedTerminals,
                    WorkGroupID: localStorage.Eplat_SelectedWorkGroupID,
                    RoleID: localStorage.Eplat_SelectedRole
                }
                ePlatHub.server.userConnection($.toJSON(dataObject));
            }
        }

        return {
            load: load,
            desktopNotification: desktopNotification,
            workingOn: workingOn
        }
    }();

    var notifyDoubleClick = function (targetName) {
        if (ePlatHubConnected) {
            var notification = { NotificationID: COMMON.serverDateTime.toLocaleString(), Notification: 'No es necesario realizar doble click sobre ninguno de los botones de la plataforma.', From: 'Edgar Falcon', Date: COMMON.serverDateTime.toLocaleString(), Important: true, WorkGroupID: UI.selectedWorkGroup };
            ePlatHub.server.sendMessage('I just did double click on ' + targetName, $('#uid').val(), '8cd24473-d223-430d-bd8f-a3db71168b6b');
            HUB.addNotification(notification);
        }
    }

    var padNumber = function (number, positions, symbol) {
        symbol = symbol || '0';
        number = number + '';
        return number.length >= positions ? number : new Array(positions - number.length + 1).join(symbol) + number;
    }

    var generateGUID = function () {
        return (G4() + G4() + "-" + G4() + "-" + G4() + "-" + G4() + "-" + G4() + G4() + G4());
    }

    function G4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substr(1)
    }

    var autoselectOnlyItemInList = function (parent) {
        var _parent = parent != undefined ? parent + ' ' : '*';
        $(_parent + '[data-autoselect-if-only-one]').each(function () {
            if ($(this).children('option').length == 1) {
                $(this).children('option:first').attr('selected', true);
                $(this).trigger('change');
            }
            else if ($(this).children('option').length == 2) {
                if ($(this).children('option:first').val() == '' || $(this).children('option:first').val() == 'null' || $(this).children('option:first').val() == '0') {
                    $(this).children('option:last').attr('selected', true);
                    $(this).trigger('change');
                }
            }
        });
    }

    var updateLocalStorageCounter = function (item, value) {
        var found = false;
        var list;
        switch (item) {
            case 'MeetingPoint': {
                if (localStorage.Eplat_Counter_MeetingPoint != null && localStorage.Eplat_Counter_MeetingPoint != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_Counter_MeetingPoint);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_Counter_MeetingPoint = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_Counter_MeetingPoint = '{"MeetingPoint":[{"Value":"' + value + '", "Selected": "1"}]}';
                }
                break;
            }
            case 'OpenCoupon': {
                if (localStorage.Eplat_Counter_OpenCoupon != null && localStorage.Eplat_Counter_OpenCoupon != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_Counter_OpenCoupon);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_Counter_OpenCoupon = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_Counter_OpenCoupon = '{"OpenCoupon":[{"Value":"' + value + '", "Selected": "1"}]}';
                }
                break;
            }
            case 'Terminals': {
                if (localStorage.Eplat_FastSale_Terminals != null && localStorage.Eplat_FastSale_Terminals != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_Terminals);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_Terminals = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_Terminals = '{"Terminals" : [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'PointsOfSale': {
                if (localStorage.Eplat_FastSale_PointsOfSale != null && localStorage.Eplat_FastSale_PointsOfSale != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_PointsOfSale);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_PointsOfSale = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_PointsOfSale = '{"PointsOfSale" :[{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'Currencies': {
                if (localStorage.Eplat_FastSale_Currencies != null && localStorage.Eplat_FastSale_Currencies != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_Currencies);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_Currencies = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_Currencies = '{"Currencies" : [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'CXCCompany': {
                if (localStorage.Eplat_Counter_CXCCompany != null && localStorage.Eplat_Counter_CXCCompany != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_Counter_CXCCompany);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_Counter_CXCCompany = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_Counter_CXCCompany = '{"CXCCompany": [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'Languages': {
                if (localStorage.Eplat_FastSale_Languages != null && localStorage.Eplat_FastSale_Languages != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_Languages);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_Languages = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_Languages = '{"Languages" : [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'LeadSources': {
                if (localStorage.Eplat_FastSale_LeadSources != null && localStorage.Eplat_FastSale_LeadSources != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_FastSale_LeadSources);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_FastSale_LeadSources = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_FastSale_LeadSources = '{"LeadSources" : [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            case 'PriceType': {
                if (localStorage.Eplat_Counter_PriceType != null && localStorage.Eplat_Counter_PriceType != undefined) {
                    var json = $.parseJSON(localStorage.Eplat_Counter_PriceType);
                    $.each(json[item], function (index, item) {
                        if (item.Value == value) {
                            item.Selected = parseInt(item.Selected) + 1;
                            found = true;
                        }
                    });
                    if (!found) {
                        json[item].push({ "Value": value, "Selected": "1" });
                    }
                    localStorage.Eplat_Counter_PriceType = JSON.stringify(json);
                }
                else {
                    localStorage.Eplat_Counter_PriceType = '{"PriceType" : [{ "Value": "' + value + '", "Selected": "1" }]}';
                }
                break;
            }
            default: {
                list = null;
            }
        }
    }

    var returnMostSelectedValue = function (localList, item) {
        var _selected = 0;
        var _value = 0;
        var json = $.parseJSON(localList);
        if (json != undefined) {
            $.each(json[item], function (index, item) {
                if (parseInt(item.Selected) > _selected) {
                    _selected = item.Selected;
                    _value = item.Value;
                }
            });
        }
        return _value;
    }

    var approveUnlock = function (userid) {
        $.ajax({
            url: '/Account/AskForUnlock/' + userid,
            type: 'GET',
            success: function (json) {
                UI.messageBox(json.ResponseType, json.ResponseMessage, 10, null)
            }
        });
    }

    var recoverPassword = function (userid) {
        $.ajax({
            url: '/Account/AskForRecoverPassword/' + userid,
            type: 'GET',
            success: function (json) {
                UI.messageBox(json.ResponseType, json.ResponseMessage, 10, null)
            }
        });
    }

    var loadDependantFields = function (data) {
        UI.DependantFields = data;
        $.each(UI.DependantFields.Fields, function (f, field) {
            $('#' + field.ParentField).on('change', function () {
                var options = '';
                var parentValue = $(this).val();
                var validFields = 0;
                $('#' + field.Field).html('');
                //mike
                if ($('#' + field.Field).hasClass('use-placeholder')) {
                    var item = field.Values.filter(function (e) { return e.ParentValue == parentValue })[0];
                    if (item != undefined) {
                        $('#' + field.Field).attr('placeholder', item.Value);
                    }
                    else {
                        $('#' + field.Field).attr('placeholder', '');
                    }
                }
                else {
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

                    if (validFields == 1) {
                        $('#' + field.Field + ' option').eq(1).prop('selected', true).trigger('change');
                    }
                }
            });
        });
    }

    var _loadDependantFields = function (data) {
        UI.DependantFields = data;
        $.each(UI.DependantFields.Fields, function (f, field) {
            $('#' + field.ParentField).on('change', function () {
                var options = '';
                var parentValue = $(this).val();
                var validFields = 0;
                $('#' + field.Field).html('');
                $.each(field.Values, function (v, value) {
                    /*console.log(v + '-----------------------------');
                    console.log('value.parentvalue: ' + value.ParentValue);
                    console.log('val: ' + parentValue);*/
                    if (value.ParentValue == parentValue || value.ParentValue == null) {
                        //console.log('value true');
                        if (value.Value != '') {
                            validFields += 1;
                        }
                        options += '<option value="' + value.Value + '">' + value.Text + '</option>';
                    }
                });
                $('#' + field.Field).html(options);

                if (validFields == 1) {
                    $('#' + field.Field + ' option').eq(1).prop('selected', true).trigger('change');
                }
            });
        });
    }

    var DateFunctions = function () {
        var dayName = function (dayOfWeek) {
            var name = '';
            switch (dayOfWeek) {
                case 0:
                    name = 'Sunday';
                    break;
                case 1:
                    name = 'Monday';
                    break;
                case 2:
                    name = 'Tuesday';
                    break;
                case 3:
                    name = 'Wednesday';
                    break;
                case 4:
                    name = 'Thursday';
                    break;
                case 5:
                    name = 'Friday';
                    break;
                case 6:
                    name = 'Saturday';
                    break;
            }
            return name;
        }

        var monthName = function (month) {
            var name = '';
            switch (month) {
                case 0:
                    name = 'January';
                    break;
                case 1:
                    name = 'February';
                    break;
                case 2:
                    name = 'March';
                    break;
                case 3:
                    name = 'April';
                    break;
                case 4:
                    name = 'May';
                    break;
                case 5:
                    name = 'June';
                    break;
                case 6:
                    name = 'July';
                    break;
                case 7:
                    name = 'August';
                    break;
                case 8:
                    name = 'September';
                    break;
                case 9:
                    name = 'October';
                    break;
                case 10:
                    name = 'November';
                    break;
                case 11:
                    name = 'December';
                    break;
            }
            return name;
        }

        return {
            dayName: dayName,
            monthName: monthName
        }
    }();

    var parseURL = function (url) {
        var a = document.createElement('a');
        a.href = url;
        return a;
    }

    var renderEmailPreview = function (data, params, callback, callbackParams, type) {
        localStorage.Eplat_EmailPreview = JSON.stringify(data);
        localStorage.Eplat_EmailPreviewParams = JSON.stringify(params);
        if (type == undefined) {
            $.fancybox({
                type: 'iframe',
                href: '/Public/PreviewEmail',
                beforeClose: function (e, x) {
                    console.log(e);
                    console.log(x);
                },
                afterClose: function () {
                    localStorage.Eplat_EmailPreview = '';
                    localStorage.Eplat_EmailPreviewParams = '';
                }
            });
        }
        else {
            var w = window.open();
            var openTab = setInterval(function () { 
                if (w != null) {
                    $(w.document.body).html($.parseJSON(localStorage.Eplat_EmailPreview).Body);
                    clearInterval(openTab);
                }
            }, 500);
        }
    }

    return {
        checkForPendingRequests: checkForPendingRequests,
        updateListsOnTerminalsChange: updateListsOnTerminalsChange,
        disableFieldWithClass: disableFieldWithClass,
        deleteDataTableItemFunction: deleteDataTableItemFunction,
        updateDependentLists: updateDependentLists,
        oTable: oTable,
        getItemDestination: getItemDestination,
        showValidationSummary: showValidationSummary,
        messageBox: messageBox,
        messageBoxExit: messageBoxExit,
        messageBoxTimer: messageBoxTimer,
        selectTerminals: selectTerminals,
        selectedTerminals: selectedTerminals,
        terminalsOpen: terminalsOpen,
        terminalsClose: terminalsClose,
        selectWorkGroup: selectWorkGroup,
        selectedWorkGroup: selectedWorkGroup,
        workGroupsOpen: workGroupsOpen,
        workGroupsClose: workGroupsClose,
        loadWorkGroup: loadWorkGroup,
        loadWorkGroups: loadWorkGroups,
        validatingWorkGroups: validatingWorkGroups,
        loadRoles: loadRoles,
        loadRole: loadRole,
        rolesOpen: rolesOpen,
        rolesClose: rolesClose,
        selectRole: selectRole,
        menuActions: menuActions,
        loadTerminals: loadTerminals,
        tablesHoverEffect: tablesHoverEffect,
        ulsHoverEffect: ulsHoverEffect,
        resetValidation: resetValidation,
        collapseFieldset: collapseFieldset,
        expandFieldset: expandFieldset,
        loadMenuComponents: loadMenuComponents,
        saveTicket: saveTicket,
        searchResultsTable: searchResultsTable,
        tablesStripedEffect: tablesStripedEffect,
        checkAllCheckBoxes: checkAllCheckBoxes,
        unselectRow: unselectRow,
        init: init,
        setTableRowsClickable: setTableRowsClickable,
        scrollTo: scrollTo,
        validateForm: validateForm,
        validateRelatedElements: validateRelatedElements,
        validateRelatedElement: validateRelatedElement,
        highlightElements: highlightElements,
        unhighlightElements: unhighlightElements,
        removeFromTable: removeFromTable,
        addToListTable: addToListTable,
        searchTextInColumns_hilightTimers: searchTextInColumns_hilightTimers,
        messageBoxIsOpen: messageBoxIsOpen,
        messageBoxInterval: messageBoxInterval,
        validateDataTable: validateDataTable,
        getValidationErrorsList: getValidationErrorsList,
        getPartialValidationErrors: getPartialValidationErrors,
        displayValidationErrors: displayValidationErrors,
        searchTextInColumns_previousHilightedElements: searchTextInColumns_previousHilightedElements,
        isDataTableValid: isDataTableValid,
        clearRelatedSourceFields: clearRelatedSourceFields,
        relatedHiddenFieldDataToTable: relatedHiddenFieldDataToTable,
        getDataRowFromUiToTable: getDataRowFromUiToTable,
        messageBoxReset: messageBoxReset,
        confirmBox: confirmBox,
        twoActionBox: twoActionBox,
        editDataRow: editDataRow,
        confirmRemoveFromTable: confirmRemoveFromTable,
        addDataRow: addDataRow,
        cancelDataRowEditing: cancelDataRowEditing,
        updateDataRow: updateDataRow,
        hideDataTableActionButtons: hideDataTableActionButtons,
        showAllDataTablesAddButtons: showAllDataTablesAddButtons,
        hideAllDataTablesActionButtons: hideAllDataTablesActionButtons,
        setSelectedRow: setSelectedRow,
        getSelectedRow: getSelectedRow,
        deselectAllSelectedRows: deselectAllSelectedRows,
        //isValidAgainstRelatedHiddenField: isValidAgainstRelatedHiddenField,
        //cleanRelatedSourceFields: cleanRelatedSourceFields,
        setTableRowsClickable: setTableRowsClickable,
        onTerminalsChanged: onTerminalsChanged,
        applyCKEditor: applyCKEditor,
        legendClickBind: legendClickBind,
        adjustLegends: adjustLegends,
        expandFirstFieldset: expandFirstFieldset,
        ckeditorUpdateInstances: ckeditorUpdateInstances,
        clearSelectedRowContent: clearSelectedRowContent,
        verticalMenu: verticalMenu,
        gotoFieldset: gotoFieldset,
        openSideMenu: openSideMenu,
        closeSideMenu: closeSideMenu,
        adjustMenuOptions: adjustMenuOptions,
        addDecimalPart: addDecimalPart,
        validateStartEndDates: validateStartEndDates,
        showCommentsOnHover: showCommentsOnHover,
        applyFormat: applyFormat,
        exportToExcel: exportToExcel,
        exportToCSV: exportToCSV,
        dependentLists: dependentLists,
        addExtras: addExtras,
        applyTextFormat: applyTextFormat,
        browserInfo: browserInfo,
        windowFocus: windowFocus,
        Chat: Chat,
        Notifications: Notifications,
        notifyDoubleClick: notifyDoubleClick,
        generateGUID: generateGUID,
        alertDoubleClick: alertDoubleClick,
        padNumber: padNumber,
        autoselectOnlyItemInList: autoselectOnlyItemInList,
        updateLocalStorageCounter: updateLocalStorageCounter,
        returnMostSelectedValue: returnMostSelectedValue,
        validateStartEndDateTimes: validateStartEndDateTimes,
        recoverPassword: recoverPassword,
        approveUnlock: approveUnlock,
        userState: userState,
        DependantFields: DependantFields,
        loadDependantFields: loadDependantFields,
        DateFunctions: DateFunctions,
        unselectPrimaryByEsc: unselectPrimaryByEsc,
        parseURL: parseURL,
        applyMultiselect: applyMultiselect,
        applyDatePicker: applyDatePicker,
        renderEmailPreview: renderEmailPreview
        //terminalDependentListActions: terminalDependentListActions
        //workgroupDependentListActions: workgroupDependentListActions
    }
}();

$(function () {
    try {
        UI.init();
    } catch (ex) { }
});