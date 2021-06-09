var COMMON = function () {


    function beforeSubmit(frmID, evnt) {
        var validation = UI.validateRelatedElements(frmID);

        //var invalidElements = UI.validateRelatedElements(frmID);
        if (validation.invalidElements.length > 0) {
            var hld = new SETTINGS.highlightElements();
            hld.elements = validation.invalidElements;
            hld.highlightTime = -1;
            UI.highlightElements(hld);
        }

        //Validate the form so UL of the validation summary gets generated
        UI.validateForm(frmID);
        var validationErrorsList = UI.getValidationErrorsList(frmID, validation.errors);
        //Display all the errors
        UI.displayValidationErrors(validationErrorsList);

        if (validation.invalidElements.length > 0) {
            evnt.preventDefault();
            evnt.stopImmediatePropagation();
            return false;
        } else {
            ///send the selected terminals in each submit
            var currentAction = $($('#' + frmID).get(0)).attr("action");
            var _newAction = COMMON.addSelectedTerminalsToURL(currentAction);
            $($('#' + frmID).get(0)).attr("action", _newAction);

        }
    }

    function expandAndCollapse(expandSelector, collapseSelector) {
        var collapsing = $.Deferred();
        var expanding = $.Deferred();
        var toExpand = $(expandSelector);
        var toCollapse = $(collapseSelector)
        var expandCounter = 0;
        var collapseCounter = 0;
        //--//--//-
        toExpand.each(function () {
            UI.expandFieldset($(this).attr('id'));
            expandCounter++;
            if (expandCounter == toExpand.length) {
                expanding.resolve();
            }
        });

        toCollapse.each(function () {
            UI.collapseFieldset($(this).attr('id'));
            collapseCounter++;
            if (collapseCounter == toCollapse.length) {
                collapsing.resolve();
            }
        });

        return $.Deferred(function (def) {
            $.when(expanding, collapsing).done(function () {
                def.resolve();
            })
        });
    }

    function collapseAndExpand(collapseSelector, expandSelector) {
        var collapsing = $.Deferred();
        var expanding = $.Deferred();
        var toExpand = $(expandSelector);
        var toCollapse = $(collapseSelector)
        var expandCounter = 0;
        var collapseCounter = 0;
        //--//--//-
        toCollapse.each(function () {
            //console.log('Collapsing ' + $(this).attr('id'));
            UI.collapseFieldset($(this).attr('id'));
            collapseCounter++;
            if (collapseCounter == toCollapse.length) {
                collapsing.resolve();
            }
        });


        var expandingInterval = setInterval(function () {
            //--//--
            toExpand.each(function () {
                //if fieldset is already expanded, do not expand it again, to avoid the 
                //blink, because when expanding, the element's display is set to none and then Slided Down.
                if ($(this).children('div').css('display') == 'none') {
                    //console.log('Expanding ' + $(this).attr('id'));
                    UI.expandFieldset($(this).attr('id'));
                }

                expandCounter++;
                if (expandCounter == toExpand.length) {
                    expanding.resolve();
                    clearInterval(expandingInterval);
                }
            });

            //--//--
        }, 200);

        return $.Deferred(function (def) {
            $.when(collapsing, expanding).done(function () {
                // console.log("collapseAndExpand("+collapseSelector +"//--//"+ expandSelector + ") finished");
                def.resolve();
            })
        });
    }

    var focusFirstEditableElement = function (container) {
        var focusing = $.Deferred();
        var attemps = 0;
        var focusInterval = setInterval(function () {
            attemps++;
            var toFocus = $();
            toFocus = $("#" + container + " :input:visible:enabled:first");
            if (toFocus.length > 0) {
                var elementID = toFocus.attr('id');
                if (elementID != undefined) {
                    toFocus.focus();
                    document.getElementById(elementID).focus();
                    if (toFocus.is(':focus')) {
                        focusing.resolve();
                        //console.log("Focusing finished");
                        clearInterval(focusInterval);
                    }
                } else {
                    // console.log('element undefined');
                }
            } else {
                //console.log('No element found');
            }

            if (attemps >= 3) {
                clearInterval(focusInterval);
                focusing.reject();
            }
        }, 400);

        return focusing;

    }

    var executeDelayed = function (func, params, delay) {
        if (!delay) { delay = 500; }
        var funcInterval = setInterval(function () {
            func.call(null, params);
            clearInterval(funcInterval);
        }, delay);


    }

    var openModal = function (url, onModalCloseCallBack, onModalCloseCallBackParam) {
        var r = window.showModalDialog(url, null, "dialogwidth: 450; dialogheight: 300; resizable: yes");
        onModalCloseCallBack.call(this, onModalCloseCallBackParam);
    }

    var openWindow = function (url, onWindowCloseCallBack) {

        var currentOpenedWindow = window.open('', 'Masterchart');
        if (currentOpenedWindow != null) {
            currentOpenedWindow.close();
        }

        var r = window.open(url, "Masterchart", "width=800,height=600,resizable=yes,scrollbars=yes");
        if (r != null) {
            r.focus();
        }
        //onWindowCloseCallBack.call(this, null);
    }

    var addSelectedTerminalsToURL = function (URL) {
        var _newURL = "";
        var currentSelectetTerminals = UI.selectedTerminals;
        _newURL = COMMON.addParamToURL(URL, "selectedTerminals", currentSelectetTerminals);
        return _newURL;
    }

    var addParamToURL = function (URL, param, value) {
        var _newURL = "";
        var newParam = param + "=" + value;
        if (UTILS.isSetURLParameter(param, URL)) {
            var currentParamValue = UTILS.getURLParameter(param, URL);
            var searchPattern = param + "=" + currentParamValue;
            _newURL = URL.replace(searchPattern, newParam);
        }
        else {
            if (URL.indexOf("?")) {
                _newURL = URL + "&" + newParam;
            } else {
                _newURL = URL + "?" + newParam;
            }
        }
        return _newURL;
    }

    var preventEmptyTableCells = function (tableID) {
        $('#' + tableID).find('tbody tr').each(function () {

        });
    }

    function showRelatedForms(formID, leadID) {
        // buscar los formularios que tengan el data-related-to-form-id
        $('form[data-related-to-form-id="' + formID + '"]').each(function () {
            var functionName = $(this).data("on-show");
            var onFormShowFunction = eval(functionName);
            onFormShowFunction.call(
                undefined,
               leadID
            );

            // fill up the relationship fields
            COMMON.getRelationshipsValues($(this).id);

            var relatedForm = $(this);
            if (!relatedForm.is(':visible')) {
                relatedForm.show();
            }

            var formItemsName = $(this).data("items-name");
            var cr = COMMON.collapseAndExpand('#fds' + formItemsName + ' #fdsCurrent' + formItemsName + ' ~ fieldset', '#fds' + formItemsName + ', #fdsCurrent' + formItemsName + '');

        });

    }

    function hideRelatedForms(formID) {
        $('form[data-related-to-form-id="' + formID + '"]').each(function () {
            $(this).clearForm();
            $(this).hide();
        });
    }

    function getRelationshipsValues(formID) {
        var relationShipsData = $("#" + formID).data("relationships");
        //var formRelationships = $.parseJSON(relationShipsData);
        for (var fr in relationShipsData) {
            $("#" + fr).val($("#" + relationShipsData[fr]).val());
        }
    }

    var serverDateTime = null;
    var serverDateTime_Timer;
    var clientTimeZoneMinutesOffset = 0;

    function startServerDateTime_Timer() {
        var serverDateTime_Timer = window.setInterval(function () {
            COMMON.serverDateTime.setSeconds(COMMON.serverDateTime.getSeconds() + 1);
        }, 1000);

    }

    function getServerDateTime() {
        var _timespan = new Date().getTime();
        $.post("/CRM/masterchart/getServerTimestamp?v="+_timespan, function (json) {
            COMMON.serverDateTime = new Date(json.year, json.month - 1, json.day, json.hour, json.minute, json.second);
            COMMON.startServerDateTime_Timer();
            COMMON.clientTimeZoneMinutesOffset = COMMON.getClientTimeZoneMinutesOffset();
        });
    }

    //function getClientTimezoneOffset() {       
    //    var _sDateTime = COMMON.serverDateTime; // compare only till minutes
    //    var sDateTime = new Date(_sDateTime.getFullYear(), _sDateTime.getMonth(), _sDateTime.getDate(), _sDateTime.getHours(), _sDateTime.getMinutes());
    //    var _cDateTime = new Date();// compare only till minutes
    //    var cDateTime = new Date(_cDateTime.getFullYear(), _cDateTime.getMonth(), _cDateTime.getDate(), _cDateTime.getHours(), _cDateTime.getMinutes());

    //    var difference = sDateTime - cDateTime;
    //    var absolutDifference = Math.abs(difference);
    //    // if difference is less than 1 hour, then return 0
    //    var hoursDifference = (absolutDifference >= 3600000) ? Math.floor(difference / 3600000) : 0;
    //    return hoursDifference;
    //}

    function getClientTimeZoneMinutesOffset() {
        var _sDateTime = COMMON.serverDateTime; // compare only till minutes
        var sDateTime = new Date(_sDateTime.getFullYear(), _sDateTime.getMonth(), _sDateTime.getDate(), _sDateTime.getHours(), _sDateTime.getMinutes());
        var _cDateTime = new Date();// compare only till minutes
        var cDateTime = new Date(_cDateTime.getFullYear(), _cDateTime.getMonth(), _cDateTime.getDate(), _cDateTime.getHours(), _cDateTime.getMinutes());

        var difference = sDateTime - cDateTime;
        return difference / 60000;
    }

    var getDate = function (date, concatTime) {

        var now = date != undefined ? date : COMMON.serverDateTime;
        var sNow = now.getFullYear() + '-' + (now.getMonth() + 1) + '-' + now.getDate();
        var sTime = '';
        //mike
        var a = sNow;
        var b = a.split('-');
        var c = '';
        $.each(b, function (i, x) {
            c += (c == '' ? '' : '-') + UI.padNumber(x, 2);
        });

        if (concatTime != undefined && concatTime) {
            sTime = ' ' + UI.padNumber(now.getHours(), 2) + ':' + UI.padNumber(now.getMinutes(), 2) + ':' + UI.padNumber(now.getSeconds(), 2);
        }
        sNow = c + sTime;
        //
        return sNow;
    }

    var isAlpha = function (s) {
        return s.match("^[a-zA-Z\(\)]+$");
    }

    return {
        beforeSubmit: beforeSubmit,
        expandAndCollapse: expandAndCollapse,
        collapseAndExpand: collapseAndExpand,
        focusFirstEditableElement: focusFirstEditableElement,
        executeDelayed: executeDelayed,
        openModal: openModal,
        openWindow: openWindow,
        addSelectedTerminalsToURL: addSelectedTerminalsToURL,
        addParamToURL: addParamToURL,
        preventEmptyTableCells: preventEmptyTableCells,
        showRelatedForms: showRelatedForms,
        hideRelatedForms: hideRelatedForms,
        getRelationshipsValues: getRelationshipsValues,
        getServerDateTime: getServerDateTime,
        getClientTimeZoneMinutesOffset: getClientTimeZoneMinutesOffset,
        serverDateTime: serverDateTime,
        serverDateTime_Timer: serverDateTime_Timer,
        startServerDateTime_Timer: startServerDateTime_Timer,
        clientTimeZoneMinutesOffset: clientTimeZoneMinutesOffset,
        getDate: getDate,
        isAlpha: isAlpha
    }
}();