/// <reference path="Settings.js" />
/// <reference path="layout/ui.js" />

//Contains reusable functions available for the whole project.
var UTILS = function () {
    var dataTable = {
        toDataAttributes: function (properties) {
            var oDtDa = OPTIONS.dataTable.dataAttributes;
            var cellProperties = [];
            for (var a in properties) {
                cellProperties.push(dataTable.toDataAttribute(a, properties[a]));
            }

            return cellProperties.join(" ");

        },
        toDataAttribute: function (dataName, dataContent) {
            return 'data-' + dataName + '="' + dataContent + '"';
        },
        actionCell: function (properties) {
            var actionType = OPTIONS.dataTable.actionTypes.remove;
            var dtO = OPTIONS.dataTable;
            var dtP = dtO.cellProperties;
            var title = properties[dtO.dataAttributes.actionType];

            var cellDataAttributes = UTILS.dataTable.toDataAttributes(properties);

            var newCell = "<td " + cellDataAttributes + ">";
            newCell += "<a title='" + title + "' href='javascript: void(0)'>";
            newCell += "<img style='border:none' alt='" + title + "' src='" + OPTIONS.dataTable.actionIcons.remove + "'/>";
            newCell += "</a>";
            newCell += "</td>";
            return newCell;
        },
        actionCells: {
            remove: function () {
                var actionType = OPTIONS.dataTable.actionTypes.remove;
                var dtO = OPTIONS.dataTable;
                var dtA = dtO.dataAttributes;
                var properties = {}
                properties[dtA.cellType] = dtO.cellTypes.action;
                properties[dtA.actionType] = dtO.actionTypes.remove;
                return UTILS.dataTable.actionCell(properties);
            },
            edit: function () {
                var actionType = OPTIONS.dataTable.actionTypes.edit;
                var dtO = OPTIONS.dataTable;
                var dtA = dtO.dataAttributes;
                var properties = {}
                properties[dtA.cellType] = dtO.cellTypes.action;
                properties[dtA.actionType] = dtO.actionTypes.edit;
                return UTILS.dataTable.actionCell(properties);
            }
        },
        dataCell: function (text, value, visible, isPK) {



            var newCell = "<td";
            newCell += " data-cell-type='data'";
            if (visible != undefined && visible != null && visible == false) {
                newCell += "  style='display:none'";
            }
            //newCell += " data-text='" + dataText + "'";
            newCell += " data-value='" + value + "'";
            //newCell += isPK != undefined && isPK ? " data-cell-pk='"+ value +"'" : "";
            newCell += ">";
            newCell += text;
            newCell += "</td>";
            return newCell;
        },
        hiddenCell: function (value) {
            var _value = value || "";
            var newCell = "<td";
            newCell += "  style='display:none'";
            newCell += " data-cell-type='hidden'";
            newCell += " data-value='" + _value + "'";
            newCell += ">";
            newCell += "</td>";
            return newCell;
        }
    };
    //globals
    var searchTextInColumns_hilightTimeOut = [];
    //var searchTextInColumns_hilightTimeOutV2;

    function unhighlightElements(params) {

        $(params.elements).each(function () {
            $(this).removeClass(params.cssClass);
        });
    }

    function highlightElements(params) {
        //add new class
        $(params.elements).each(function () {
            $(this).addClass(params.cssClass);
        });

        if (params.highlightTime > 0) {
            //make sure no other timer is runing.
            clearTimeout(UTILS.searchTextInColumns_hilightTimeOut);
            //unhighlight Elements when the time passes 
            UTILS.searchTextInColumns_hilightTimeOut = setTimeout(
                function () {
                    UI.unhighlightElements(params);
                }, params.highlightTime
            );
        }

        //clean all "searchTextInColumns" evidence 
        $(params.elements).each(function () {
            var defaults = new SETTINGS.searchTextInColumns();
            $(this).removeClass(defaults.matchIdentifier);
        });

    }
    //Table Functions
    function searchTextInColumns(settings) {
        //return the matching elements
        var text = settings.text || "";
        var searchType = settings.searchType || "containsExact";
        var tableID = settings.tableID || "";
        var totalTableColumns = $("#" + tableID + ">tbody>tr:first td").length;
        var defaults = new SETTINGS.searchTextInColumns();
        var matchIdentifier = settings.matchIdentifier || defaults.matchIdentifier;

        //don't even search if there is no rows in the specified table
        if ($("#" + tableID + ">tbody>tr").length <= 0) { return []; }

        //make sure previous search don't mess with a new search
        $("#" + tableID + ">tbody>tr td." + matchIdentifier).removeClass(matchIdentifier);

        var specifiedColumns = UTILS.rangeToArray(
                                 settings.specifiedColumns,
                                 totalTableColumns
                                 );

        for (var x in specifiedColumns) {
            var sci = x;

            try {
                var selector = "#" + tableID
                selector += " tbody tr :nth-child(" + specifiedColumns[x] + "):"
                selector += searchType + "('" + text + "')";
                $(selector).addClass(matchIdentifier);
            } catch (e) {
                alert(e.message);
            }
        }

        //count total
        var selectorMatches = "#" + tableID + " tbody ." + matchIdentifier;
        var matches = $(selectorMatches);
        return matches;
    }
    function arrayToTableRow(data, totalTableColumns) {
        var dataFields = UTILS.toArray(data);
        var tableRow = '<tr>';
        for (x in dataFields) {
            tableRow += "<td>" + dataFields[x] + "</td>";
        }

        var missingCells = totalTableColumns - dataFields.length;
        for (var m = 1; m < missingCells; m++) {
            tableRow += "<td></td>";
        }

        return tableRow + "</tr>";
    }
    function getColumnsRowSelector(tableID) {
        var columnsRowSelector = "";

        if ($('#' + tableID + '>thead>tr[data-row-type="columns"]').length > 0) {
            columnsRowSelector = '[data-row-type="columns"]';
        }
        else { columnsRowSelector = ':last'; }
        return columnsRowSelector;
    }
    function getAllTableColumnReferences(tableID) {
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);

        var allTableColumns = [];

        $('#' + tableID + '>thead>tr:' + columnsRowSelector + '>th').each(
            function (i) {
                allTableColumns.push(this);
            }
        );
        return allTableColumns;
    }
    function getAllTableDataColumnIndexes(tableID) {
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);

        var allTableColumns = [];

        $('#' + tableID + '>thead>tr:' + columnsRowSelector + '>th[data-column-type="data"]').each(
            function (i) {
                allTableColumns.push(i + 1);
            }
        );
        return allTableColumns;
    }
    function getAllTableDataColumnReferences(tableID) {
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);

        var allTableDataColumns = $();

        $('#' + tableID + '>thead>tr:' + columnsRowSelector + '>th[data-column-type="data"]').each(
            function (i) {
                // allTableColumns.push(i + 1);
                allTableDataColumns = allTableDataColumns.add(this);
            }
        );
        return allTableDataColumns;
    }
    function getTableColumnsProperties(tableID) {
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);
        var columnProperties = [];
        $('#' + tableID + '>thead>tr' + columnsRowSelector + '>th[data-column-type="data"]').each(
           function (i) {
               columnProperties.push($(this).data("column-properties"));
           }
       );
        return columnProperties;
    }
    function updateJsonRowToTable(tableID, rowIndex, jsonRow) {
        var cols = getAllTableDataColumnIndexes(tableID);
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);
        for (var c in cols) {
            var columnSelector = '#' + tableID + '>thead>tr:'
            columnSelector += columnsRowSelector
            columnSelector += '>th:nth-child(' + cols[c] + ')';

            var p = $.parseJSON($(columnSelector).data('column-properties'));

            var cellSelector = '#' + tableID + '>tbody>tr:'
            cellSelector += 'eq(' + rowIndex + ')'
            cellSelector += '>td:nth-child(' + cols[c] + ')';

            var sourceField = $("#" + p.sourceField);
            var dataText = "";
            var dataValue = jsonRow[p.name];
            if (sourceField.is("select")) {
                //the next line works , but searching by value isn't that safe.
                //dataText = sourceField.find('option[value="' + dataValue + '"]').text();
                dataText = sourceField.find('option:selected').text();
            }
            else { dataText = dataValue; }
            var cell = $(cellSelector);
            cell.data('value', dataValue);
            cell.text(dataText);
        }
    }

    function addJsonRowToTable(tableID, jsonRow) {
        var allTableColumns = getAllTableColumnReferences(tableID);
        var rp = $.parseJSON($('#' + tableID).data("rows-properties"));
        var htmlRow = "<tr tabindex='0' ";

        if (rp.rowID != undefined && rp.rowID != null) {
            htmlRow += "id='" + jsonRow[rp.rowID] + "' ";
        }

        htmlRow += ">";

        var cellTypes = OPTIONS.dataTable.cellTypes;

        for (var c in allTableColumns) {
            var columType = $(allTableColumns[c]).data("column-type");
            switch (columType) {
                case cellTypes.data:
                    var cp = $.parseJSON($(allTableColumns[c]).data("column-properties"));
                    if (jsonRow[cp.name] != null && jsonRow[cp.name] != undefined) {
                        var element = cp.sourceField;
                        var _valor = jsonRow[cp.name];
                        var _isIsoDate = isIsoDate(_valor);
                        var _isJavascriptDate = false;
                        if (_valor.toString().indexOf("/Date(") != -1) { _isJavascriptDate = true; }
                        if (_isJavascriptDate || _isIsoDate) {
                            if (_isIsoDate) {
                                _valor = serializedDateToDate(element, UTILS.validIsoDatePart(_valor), cp.usesTime);
                            } else {
                                _valor = serializedDateToDate(element, _valor, cp.usesTime);
                            }
                            dataValue = _valor;
                        } else {
                            dataValue = jsonRow[cp.name];
                        }
                        // dataValue = jsonRow[cp.name];
                    }
                    else {
                        if ($(allTableColumns[c]).data('delete-flag') != undefined) {
                            dataValue = 'false';
                        }
                        else {
                            dataValue = "";
                        }
                    }

                    //get the text from the source field
                    var dataText = "";
                    var sourceField = $("#" + cp.sourceField);
                    if (sourceField.is("select")) {
                        //the next line works , but searching by value isn't that safe.
                        dataText = sourceField.find('option[value="' + dataValue + '"]').text();
                        //dataText = sourceField.find('option:selected').text();
                    }
                    else { dataText = dataValue; }

                    var visible = $(allTableColumns[c]).data("column-visible");
                    //htmlRow += UTILS.dataTable.dataCell(dataText, dataValue, visible, isPK);
                    htmlRow += UTILS.dataTable.dataCell(dataText, dataValue, visible);

                    break;
                case cellTypes.hidden:
                    htmlRow += UTILS.dataTable.hiddenCell(dataValue);

                    break;
                case cellTypes.action:
                    var actionType = $(allTableColumns[c]).data("action-type");

                    if (actionType == OPTIONS.dataTable.actionTypes.edit) {
                        if (eval(rp.showRemoveIcon) == true) {
                            htmlRow += UTILS.dataTable.actionCells.edit();
                        }
                    }


                    //Remove
                    if (actionType == OPTIONS.dataTable.actionTypes.remove) {
                        if (eval(rp.showRemoveIcon) == true) {
                            htmlRow += UTILS.dataTable.actionCells.remove();
                        }
                    }
                    break;
                default:
                    htmlRow += "<td></td>";
            }
        }
        htmlRow += "</tr>";

        //$("#" + tableID).append(htmlRow);


        var tabla = document.getElementById(tableID);

        if ($.fn.DataTable.fnIsDataTable(tabla)) {
            var oTable = $('#' + tableID).dataTable();
            oTable.fnAddTrAndDisplay($(htmlRow)[0]);
            // oTable.fnAddDataAndDisplay($(htmlRow)[0]);
        }
        else {
            $("#" + tableID).append(htmlRow);
        }



        var oDtDa = OPTIONS.dataTable.dataAttributes;
        //dataTable.dataAttributes
        var removeSelector = '#' + tableID + ' tr td[data-' + oDtDa.actionType + '="';
        removeSelector += OPTIONS.dataTable.actionTypes.remove;
        removeSelector += '"]>a';
        $(removeSelector).each(
                function () {
                    $(this).unbind("click");
                    $(this).click(
                        function (e) {
                            //alert("Delete Clicked");
                            var callBackFn = eval(rp.onRemoveCallBack);
                            callBackFn.call(
                                undefined,
                                tableID,
                               $(this).parents("tr").index()
                            );
                            e.stopPropagation();
                        }
                    );
                }
            );


        if (eval(rp.onRowEvents) != undefined && eval(rp.onRowEvents) != null) {

            for (rowEvent in rp.onRowEvents) {
                var rowsSelector = '#' + tableID + '  tr';
                $(rowsSelector).each(function () {
                    if ($(this).parent().is("tbody")) { // attach events only to rows under tbody
                        $(this).unbind(rowEvent);
                        $(this).on(rowEvent, function () {
                            var callBackFn = eval(rp.onRowEvents[rowEvent]);
                            callBackFn.call(
                                undefined,
                                tableID,
                                $(this).index(),
                                $(this).attr('id')
                            );
                        });


                        $(this).unbind("keydown");
                        $(this).keydown(function (e) {
                            //handle keyboard "Click" (Enter Or Space Bar)
                            if ((e.which == 13 || e.which == 32) && rowEvent == "click") {
                                var callBackFn = eval(rp.onRowEvents[rowEvent]);
                                callBackFn.call(
                                    undefined,
                                    tableID,
                                    $(this).index()
                                );
                                e.preventDefault(); // prevent form submiting (enter) or page scroll down (space bar)
                            }
                            //add arows navigation
                            if (e.which == 38) {
                                //alert('focusing prev');
                                $(this).prev('tr').focus();
                                e.preventDefault(); //prevent page scrolling
                            }
                            if (e.which == 40) {
                                //alert('focusing next');
                                $(this).next('tr').focus();
                                e.preventDefault();//prevent page scrolling
                            }
                            //e.stopPropagation(); //// do not apply this here, otherwise keyCodes not set above wont work, like tab.
                            //e.preventDefault(); //// do not apply this here, otherwise keyCodes not set above wont work, like tab.

                        });

                        $(this).on("focus", function () {
                            $(this).not('.trheader').attr('style', 'background-color: rgb(226, 222, 214); cursor:pointer');
                        });
                        $(this).on("blur", function () {
                            $(this).not('.trheader').attr('style', null);
                        });
                    }
                });
            }

        }

        var editSelector = '#' + tableID + ' tr td[data-' + oDtDa.actionType + '="';
        editSelector += OPTIONS.dataTable.actionTypes.edit;
        editSelector += '"]>a';
        //$(editSelector).each(
        //        function () {
        //            $(this).unbind("click");
        //            $(this).click(
        //                function (e) {
        //                    var callBackFn = eval(rp.onEditCallBack);
        //                    callBackFn.call(
        //                        undefined,
        //                        tableID,
        //                       $(this).parents("tr").index()
        //                    );
        //                    e.stopPropagation();
        //                }
        //            );
        //        }
        //    );
    }//heree

    function addDataRowsToTable(tableID, newRows) {
        var tableColumns = $('#' + tableID + '>thead>tr:last>th');
        for (row in newRows.dataRows) {
            var newRow = newRows.settings;

            var stringTR = UTILS.arrayToTableRows(newRows.dataRows[row], tableColumns.length);

            var tabla = document.getElementById(tableID);

            if ($.fn.DataTable.fnIsDataTable(tabla)) {
                $('#' + tableID).dataTable().fnAddTrAndDisplay($(stringTR));
            }
            else {
                $("#" + tableID).append(stringTR);
            }


        }

        if (newRows.options.addDeleteIcon == true) {
            var deleteImgSrc = "/Content/themes/base/images/cross.png";
            var deleteCell = "";
            deleteCell += "<td class='deleteIcon'>";
            deleteCell += "<a href='javascript: void(0)'>";
            deleteCell += "<img src='" + deleteImgSrc + "'/>";
            deleteCell += "</a>";
            deleteCell += "</td>";

            $("#" + tableID + " tbody tr").not(":has(td.deleteIcon)").append(deleteCell);
            $("#" + tableID + " tbody td.deleteIcon > a").each(
                function (i) {
                    $(this).unbind("click"); //remove click from all delete cells
                    $(this).click( // re-bind click for all delete cells
                        function () {
                            newRows.options.deleteIconCallBack.call(
                                undefined,
                                tableID,
                               $(this).parents("tr").index()
                            );
                        }
                    );
                }
            );
        }
    }

    function removeDataRowsFromTable(tableID, rowsIndexes) {
        rowsIndexes = toArray(rowsIndexes);
        rowsIndexes.sort();
        var total = rowsIndexes.length;
        var _table = document.getElementById(tableID);
        for (var i = (total - 1) ; i >= 0; i--) {
            //only set as hidden and set property delete as true
            //get index of td with deleted flag value
            var _index = $('#' + tableID + ' thead tr:last th[data-delete-flag]').index();
            var _pkIndex = $('#' + tableID + ' thead tr:last th[data-column-pk]').index();
            //mike
            if ($("#" + tableID + " tbody tr:eq(" + rowsIndexes[i] + ") td:eq(" + _pkIndex + ")").text() == '') {
                //check if is dataTable
                if ($.fn.DataTable.fnIsDataTable(_table)) {
                    var _oTable = $('#' + tableID).dataTable();
                    //var _node = _oTable.fnGetNodes(rowsIndexes[i]);
                    ////var array = _oTable.fnGetNodes();
                    //console.log(array);
                    //console.log(rowsIndexes[i]);
                    //console.log($("#" + tableID + " tbody tr:eq(" + rowsIndexes[i] + ")"));
                    //console.log(_oTable.fnGetNodes(rowsIndexes[i]));
                    ////var nTr = _oTable.$('tr:eq('+ rowsIndexes[i] +')');
                    //var position = _oTable.fnGetPosition(node[0]);
                    ////_oTable.fnDeleteRow(array[position]);
                    //console.log(position);
                }
                else {
                    $("#" + tableID + " tbody tr:eq(" + rowsIndexes[i] + ")").remove();
                }
            }
            else {
                $("#" + tableID + " tbody tr:eq(" + rowsIndexes[i] + ") td:eq(" + _index + ")").attr('data-value', 'true');
                $("#" + tableID + " tbody tr:eq(" + rowsIndexes[i] + ") td:eq(" + _index + ")").data('value', 'true');
                $("#" + tableID + " tbody tr:eq(" + rowsIndexes[i] + ") td:eq(" + _index + ")").text('true');
                $("#" + tableID + " tbody tr:eq(" + rowsIndexes[i] + ")").hide();
                if ($.fn.DataTable.fnIsDataTable(_table)) {
                    //if plugin applied, it is neccesary update tr to hide it
                    $('#' + tableID).dataTable().fnUpdate('true', rowsIndexes[i], _index);
                }
            }
        }
    }
    function clearTableRows(tableID) {
        ///<summary>Deletes all the rows whithin the table body of the specified table</summary>
        /// <param name="tableID" type="String">The target table's ID.</param>
        try {
            $('#' + tableID).dataTable().fnClearTable();
            $('#' + tableID).dataTable().fnDestroy();
            $("#" + tableID + " tbody tr").remove();
        }
        catch (e) { }
    }
    //ranges
    function rangeToArray(range, totalItems) {
        var rangeArray = [];
        if ($.isArray(range)) { //array
            rangeArray = range;
        }
        else if ($.type(range) === "string") {
            if (range.toLowerCase() == "all") {
                for (var i = 1; i <= totalItems; i++) {
                    rangeArray.push(i);
                }
            }
            else if (range.indexOf("-") >= 0) { //string range : [n-m] or n-m            
                var cadena = range.replace("[", "");
                cadena = cadena.replace("]", "");
                var rango = cadena.split("-");
                var start = parseInt(rango[0]);
                var end = parseInt(rango[1]);

                for (var i = start; i <= end; i++) {
                    rangeArray.push(i);
                }
            } else if (range.indexOf(",") >= 0) {
                rangeArray = range.split(",");
            }
        }
        else if (!isNaN(parseInt(range))) {
            var columnNumber = parseInt(range);
            if (range < 0) // negative number
            {
                var calculatedColumn = (totalItems - Math.abs(columnNumber)) + 1;
                rangeArray.push(calculatedColumn);
            }
            else { // positive number
                rangeArray.push(range);
            }
        }


        else if ($.isPlainObject(range)) {  //object range{start:1, end:5}

            var start = range["start"];
            var end = range["end"];

            for (var i = start; i <= end; i++) {
                rangeArray.push(i);
            }
        }

        return rangeArray;
    }
    //Ajax
    function getInfoAsync(params) {    // params{.URL,.method,.data,.onCompleteCallback}
        $.ajax({
            url: params.URL,
            type: params.method || "POST",
            data: params.data,
            complete: function (jqXHR, textStatus) { params.onCompleteCallback.call(this, jqXHR, textStatus, params.data); } // "call" method receives a list of parameters
            //complete: function (jqXHR, textStatus) { params.onCompleteCallback.apply(this, [jqXHR, textStatus]); } // "apply" method receives an array of parameters

        });
    }

    /// <summary>Deletes the properties in the array from the given Json.</summary>
    /// <param name="Json">The Json object to delete properties from.</param>
    /// <param name="propertiesArray">An array containing the properties to delete from the Json.</param>
    /// <returns></returns>    
    function removePropertiesFromJson(json, propertiesArray) {

        for (x in propertiesArray) {
            delete json[propertiesArray[x]];
        }
    }

    function getInputValueByType(inputID) {
        var selector = "#" + inputID;
        var value = null;

        if ($(selector).is("input:checkbox")) {
            value = $(selector).is(':checked').toString();
        }
        else { value = $.trim($(selector).val()); }

        return value;
    }

    function setInputValueByType(inputID, value) {
        var selector = "#" + inputID;
        if ($(selector).is("input:checkbox")) {
            var parsedValue = eval(value);
            $(selector).attr('checked', parsedValue);
        }
        else if($(selector).is('select')){
            $(selector + ' option[value="' + value + '"]').attr('selected', true);
            $(selector).trigger('change');
        }
        else { $(selector).val(value); }
    }

    function serializedDateToDate(elementSelector, serializedDate, usesTime) {
        var _valor = serializedDate;
        var _isIsoDate = isIsoDate(_valor);
        var _isJavascriptDate = false;

        if (_valor.toString().indexOf("/Date(") != -1) { _isJavascriptDate = true; }

        if (_isJavascriptDate || _isIsoDate) {

            var _date;
            var __date
            var clientTimeZoneMinutesOffset = COMMON.clientTimeZoneMinutesOffset;
            if (_isJavascriptDate) {
                /* /Date(1370626189827)/ */
                var _dateStr = _valor.replace("/", "").replace("/", "").replace("Date", "").replace("(", "").replace(")", "");
                var __date = new Date(parseInt(_dateStr));
                //now since the serialized dates are based on our timezone, prearrival people reported that all the dates were showing 1 day off
                //hence we have to detect the timezone offset and add it to the parsed date.
                //var _date = new Date(__date.getTime() + (__date.getTimezoneOffset() * 60000));
                _date = new Date(__date.getTime() + (clientTimeZoneMinutesOffset * 60000));
            }
            else {
                var isoDateTime = UTILS.validIsoDatePart(_valor).split('T');
                var isoDate = isoDateTime[0];
                var isoTime = isoDateTime[1];

                var isoDateParts = isoDate.split('-');
                var _yyyy = isoDateParts[0];
                var _mm = isoDateParts[1];
                var _dd = isoDateParts[2];

                var isoTimeparts = isoTime.split(':');
                var _hh = isoTimeparts[0];
                var _ii = isoTimeparts[1];
                var _ss = isoTimeparts[2];

                //__date = new Date(_yyyy, _mm, _dd, _hh, _mm, _ss);
                //_date = new Date(__date.getTime());

                _date = new Date(_yyyy, _mm - 1, _dd, _hh, _ii, _ss);

            }


            var _formatedDateTime = $.datepicker.formatDate('yy-mm-dd', _date);
            //$.datepicker.formatTime('HH:mm:ss:l', { hour: _hours, minute: _minutes,second:_seconds, millisecond:_milliseconds });

            if ($(elementSelector).data('uses-date-picker-time') == true || usesTime) {
                //if ($(elementSelector).data('uses-date-picker-time') == true || usesTime || $(elementSelector).data('uses-datetime-picker') == true) {
                var _hours = ("00" + _date.getHours()).slice(-2);
                var _minutes = ("00" + _date.getMinutes()).slice(-2);
                var _seconds = ("00" + _date.getSeconds()).slice(-2);
                var _milliseconds = _date.getMilliseconds();
                var _time = _hours + ":" + _minutes + ":" + _seconds;

                _formatedDateTime += " " + _time;
            }


            _valor = _formatedDateTime
        }
        return _valor;
    }

    function jsonToFormFields(json, prefix) {
        /// <summary>
        /// Tries to copy to form fields the value of each property matching by property name and the form field id.
        /// </summary>
        /// <param name="json">The JSON where the values come from.</param>
        /// <param name="scrollingDuration">The duration in milliseconds of the scrolling.</param>
        /// <returns></returns>

        var idsToCascade = [];
        if (prefix == undefined) { prefix = ""; } else { prefix += "_"; }

        for (x in json) {
            try {
                var _valor = json[x];
                var element = "#" + prefix + x;
                $(element).clearForm(); //clear the field.
                //$(element).clearForm(true); //clear the field.//mike
                //set the field value            
                //GeneralInformation_PresentationInformation_Presentations_ContractsHistoryList
                var relatedTable = $('[data-related-validation-field="' + prefix + x + '"]');
                var sourceName = $('[data-source-name="' + x + '"]');

                var _valor = json[x];

                if (UTILS.isIsoDate(_valor)) {
                    _valor = serializedDateToDate(element, UTILS.validIsoDatePart(_valor));
                    $(element).val(_valor);

                    //new
                    var dataReadOnlyTextFor_Selector = '[data-read-only-text-for="' + prefix + x + '"]';
                    var dataTextSource_Selector = '[data-text-source="' + prefix + x + '_readOnly"]';

                    $(dataReadOnlyTextFor_Selector).text(_valor);
                    if ($(dataReadOnlyTextFor_Selector).is(dataTextSource_Selector)) {
                        var textSource = $(dataReadOnlyTextFor_Selector).data('text-source');
                        var textSelector = '#' + textSource + ' option[value="' + _valor + '"]';
                        var valueText = $(textSelector).text();
                        $(dataReadOnlyTextFor_Selector).text(valueText);
                    }
                    else {
                        $('[data-text-source="' + prefix + x + '"]').text(_valor);
                    }
                    //
                    
                }
                else if (sourceName.length > 0) {
                    var tableID = sourceName.attr('id');
                    UTILS.clearTableRows(tableID);
                    for (var dr in json[x]) {
                        UTILS.addJsonRowToTable(tableID, json[x][dr]);
                    }
                    UI.searchResultsTable(tableID);
                }
                else if (relatedTable.length > 0) {
                    var tableID = relatedTable.attr('id');
                    if (json[x] != null) {
                        $(element).val(JSON.stringify(json[x]));
                    }
                    //reset the related table
                    UTILS.clearTableRows(tableID);

                    var jsonObj = json[x];
                    //for each dataRow in the object add a row to the related table
                    for (var dr in jsonObj) {
                        //UTILS.addJsonRowToTable(tableID, jsonObj[dr]);
                        var _index = $('#' + tableID + ' thead tr:last th[data-delete-flag]').index();
                        if (_index != -1) {//if has column with delete flag
                            var pkIndex = $('#' + tableID + ' thead tr:last th[data-column-pk]').index();
                            var name = $.parseJSON($('#' + tableID + ' thead tr:last th[data-delete-flag]').data('column-properties')).name;
                            UTILS.addJsonRowToTable(tableID, jsonObj[dr]);

                            ////
                            var oTable;
                            if ($.fn.DataTable.fnIsDataTable(tableID)) {
                                oTable = $('#' + tableID).dataTable();
                            }
                            else {
                                oTable = $("#" + tableID + ' tbody');
                            }
                            if (oTable.find('tr:last').find('td:eq(' + _index + ')').text().toLowerCase() == 'true') {
                                oTable.find('tr:last').hide();
                            }
                            /////

                            //if (jsonObj[dr][name] == false) {
                            //    UTILS.addJsonRowToTable(tableID, jsonObj[dr]);
                            //}
                            //else {
                            //    //set row as hidden
                            //}
                        }
                        else {
                            UTILS.addJsonRowToTable(tableID, jsonObj[dr]);
                        }
                    }
                    //agregar condición para aplicar plugin dataTable
                    if ($('#' + tableID).data('table')) {
                        UI.searchResultsTable(tableID);
                    }
                    UI.validateDataTable(tableID);
                }
                else if ($(element).is('[data-uses-time-only]')) {
                    if (json[x] != "") {
                        var time = new Date("1970/01/01 " + json[x]);
                        var timeObj = { hour: time.getHours(), minute: time.getMinutes(), second: time.getSeconds() };
                        var timeFormat = $(element).datepicker('option', 'timeFormat');

                        if (timeFormat != null && timeFormat.length <= 0) {
                            timeFormat = 'HH:mm'; // set a default time format. 
                            //$(element).val($.datepicker.formatTime(timeFormat, timeObj));
                        }
                        $(element).val($.datepicker.formatTime(timeFormat, timeObj));

                    }
                }
                else if ($.isPlainObject(json[x])) {
                    if ($(element).is('[data-uses-time-picker]')) {
                        //"Presentations_TourTime":{"Hours":10,"Minutes":30,"Seconds":0,"Milliseconds":0,"Ticks":378000000000,"Days":0,"TotalDays":0.4375,"TotalHours":10.5,"TotalMilliseconds":37800000,"TotalMinutes":630,"TotalSeconds":37800}
                        var _hours = json[x].Hours;
                        var _minutes = json[x].Minutes;
                        $(element).val($.datepicker.formatTime('HH:mm', { hour: _hours, minute: _minutes }));
                    }
                    else {
                        jsonToFormFields(json[x], x);
                    }
                }
                else {
                    //json[x] != null
                    if (true) {
                        if ($(element).is(":checkbox")) {
                            $(element).prop("checked", _valor);
                        } else {
                            if (json[x] != null && _valor.toString().indexOf("/Date(") != -1) {
                                _valor = serializedDateToDate(element, _valor);
                            }

                            $(element).val(_valor);
                            
                            var feedingMethodSelector = "[data-cascading-feeding-method]";
                            if ($(element).is(feedingMethodSelector)) {//$(element) will be feed by ajax
                                //if($(element + "  option[value='" + _valor + "']").length == 0){
                                //if the option to select (_val) does not exist yet in the dropdown, store it in a data-selected-value
                                $(element).data('selected-value', (_valor != null) ? _valor : "");
                                //save the parent it so we can trigger the event at the end of this function.
                                var cascadingParentSelector = $(element).data('cascading-parent');
                                if ($.inArray(cascadingParentSelector, idsToCascade) == -1) {
                                    idsToCascade.push(cascadingParentSelector);
                                    //cascadingParentSelector.split(',');//mike
                                    //idsToCascade.push(cascadingParentSelector.split(','));//mike
                                }
                                //}
                            }

                            //if this is a hidden field, probably there is a readOnly tag to show this value also.
                            var dataReadOnlyTextFor_Selector = '[data-read-only-text-for="' + prefix + x + '"]';
                            var dataTextSource_Selector = '[data-text-source="' + prefix + x + '_readOnly"]';

                            $(dataReadOnlyTextFor_Selector).text(_valor);
                            if ($(dataReadOnlyTextFor_Selector).is(dataTextSource_Selector)) {
                                var textSource = $(dataReadOnlyTextFor_Selector).data('text-source');
                                var textSelector = '#' + textSource + ' option[value="' + _valor + '"]';
                                var valueText = $(textSelector).text();
                                $(dataReadOnlyTextFor_Selector).text(valueText);
                            }
                            else {
                                $('[data-text-source="' + prefix + x + '"]').text(_valor);
                            }

                        }
                    }
                }
            }
            catch (ex) { }
        }
        //trigger the change event on the parent ddls so the child ddls can get their corresponding items and show the selectedd values.
        for (x in idsToCascade) {
            UTILS.cascadeDropDowns(idsToCascade[x]);
            //$('#' +  idsToCascade[x]).trigger('change');
        }

    }

    function getTypeName(obj) {
        try {
            if (obj) {
                var funcNameRegex = /function (.{1,})\(/;
                var results = (funcNameRegex).exec(obj.constructor.toString());
                return (results && results.length > 1) ? results[1] : "";
            } else { return undefined; }
        }
        catch (e) {
            return undefined;
        }
    }

    function toArray(data, separator) {
        var dataFields;
        if ($.type(data) === "object" || $.type(data) === "array") { dataFields = data; }
        else if ($.type(data) === "string" && separator) {
            dataFields = data.split(separator);
        }
        else { dataFields = [data]; }

        return dataFields;
    }

    function tableDataToJason(params) {
        var tableID = params.tableID || "";
        var subject = "items";
        if (params.subject) { subject = params.subject; }
        else if ($("#" + tableID).data('items-name')) {
            subject = $("#" + tableID).data('items-name');
        }

        var allTableColumns = UTILS.getAllTableDataColumnIndexes(tableID);
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);

        var cols = params.columnIndexes || allTableColumns;
        var headers = [];
        var jsonObj = {};
        jsonObj[subject] = [];
        if (cols.length > 0) {//get Header text for each column
            for (c in cols) {
                var columnSelector = '#' + tableID + '>thead>tr:'
                columnSelector += columnsRowSelector
                columnSelector += '>th:nth-child(' + cols[c] + ')';

                var p = $.parseJSON(
                        $(columnSelector).data("column-properties")
                    );
                headers.push(p.name);
            }
            //get cell values
            $('#' + tableID + '>tbody>tr').each( //for each row in tableBody
                function () {
                    //if(cols.length>1){
                    var dataRow = {};
                    for (c in cols) {
                        var columnSelector = 'td:nth-child(' + cols[c] + ')';
                        var text = $.trim($(this).find(columnSelector).data("text"));
                        var value = $.trim($(this).find(columnSelector).data("value"));
                        dataRow[headers[c]] = { dataValue: value, dataText: text };
                    }
                    jsonObj[subject].push(dataRow);
                    //}
                    //    else {
                    //        var cellText = $(this).find('td:nth-child(' + cols[0] + ')').text();
                    //        jsonObj[subject].push(cellText);
                    //    }

                }
            );

        }

        return jsonObj;

    }

    function tableDataRowValuesToJson(params, rowIndex, row) {

        //row is beign used if working over a .dataTable()

        var tableID = params.tableID || "";
        var allTableColumns = UTILS.getAllTableDataColumnIndexes(tableID);
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);

        var cols = params.columnIndexes || allTableColumns;
        var headers = [];
        var jsonObj = [];
        // jsonObj[subject] = [];
        if (cols.length > 0) {//get Header text for each column
            for (c in cols) {
                var columnSelector = '#' + tableID + '>thead>tr:';
                columnSelector += columnsRowSelector
                columnSelector += '>th:nth-child(' + cols[c] + ')';
                var p = $.parseJSON(
                        $(columnSelector).data("column-properties")
                    );
                headers.push(p.name);
            }
            var dataRow = {};

            if (row) {

                for (c in cols) {
                    var columnSelector = 'td:nth-child(' + cols[c] + ')';
                    //var text = $.trim($(this).find(columnSelector).data("text"));
                    var value = $.trim($(row).find(columnSelector).data("value"));

                    if ($.isNumeric(value)) { value = parseInt(value); }
                    else if (value == undefined) { value = null; }
                    try {
                        dataRow[headers[c]] = value.toString();
                    } catch (e) {
                        alert(e.message);
                    }

                }

            } else {
                //get cell values
                $('#' + tableID + '>tbody>tr:eq(' + rowIndex + ')').each( //for the row at specified index
                    function () {
                        //if(cols.length>1){
                        for (c in cols) {
                            var _c = parseInt(c.toString());
                            //console.log('header: ' + headers[c]);
                            var columnProperties = $.parseJSON($('#' + tableID + '>thead>tr:' + UTILS.getColumnsRowSelector(tableID) + '>th:nth-child(' + (_c+1) + ')').data('column-properties'));
                            //console.log($('#' + tableID + '>thead>tr:' + UTILS.getColumnsRowSelector(tableID) + '>th:nth-child(' + (_c + 1) + ')'));
                            //if (columnProperties != null && columnProperties.usesTime == true) {
                            //    console.log(columnProperties);
                            //    console.log('usesTime: ' + columnProperties.usesTime);
                            //}
                            var columnSelector = 'td:nth-child(' + cols[c] + ')';
                            //var text = $.trim($(this).find(columnSelector).data("text"));
                            var value = $.trim($(this).find(columnSelector).data("value"));

                            //_date = new Date(__date.getTime() + (clientTimeZoneMinutesOffset * 60000));
                            if ($.isNumeric(value)) { value = parseInt(value); }
                            else if (value == undefined) { value = null; }
                            //mike
                            if (columnProperties != null && columnProperties.usesTime == true) {
                                value = new Date(new Date(value).getTime() + (COMMON.clientTimeZoneMinutesOffset * 60000));
                            }
                            //end mike
                            try {
                                dataRow[headers[c]] = value.toString();
                            } catch (e) {
                                alert(e.message);
                            }
                        }
                    }
                );
            }
            //console.log(dataRow);
            return dataRow;

        }

        return jsonObj;

    }

    function tableDataValuesToJson(params) {
        var tableID = params.tableID || "";
        var localParams = params;
        var jsonObj = [];
        //get cell values

        //--//--/--//-
        var tabla = document.getElementById(tableID);
        if ($.fn.DataTable.fnIsDataTable(tabla)) {

            var allRows = $('#' + tableID).dataTable().fnGetNodes();
            //for each row in tableBody
            for (var i = 0; i < allRows.length; i++) {
                var dataRow = UTILS.tableDataRowValuesToJson(localParams, i, allRows[i]);
                jsonObj.push(dataRow);
            }
        }
        else {
            $('#' + tableID + '>tbody>tr').each( //for each row in tableBody
                function (i) {
                    var dataRow = UTILS.tableDataRowValuesToJson(localParams, i);
                    jsonObj.push(dataRow);
                }
            );
        }
        //-///-//-/-/-




        return jsonObj;

    }
    function tableDataTextsToJson(params) {
        var tableID = params.tableID || "";
        var subject = "items";
        if (params.subject) { subject = params.subject; }
        else if ($("#" + tableID).data('items-name')) {
            subject = $("#" + tableID).data('items-name');
        }

        var allTableColumns = UTILS.getAllTableDataColumnIndexes(tableID);
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);

        var cols = params.columnIndexes || allTableColumns;
        var headers = [];
        var jsonObj = [];
        if (cols.length > 0) {//get Header text for each column
            for (c in cols) {
                var columnSelector = '#' + tableID + '>thead>tr:'
                columnSelector += columnsRowSelector
                columnSelector += '>th:nth-child(' + cols[c] + ')';
                var p = $.parseJSON(
                        $(columnSelector).data("column-properties")
                    );
                headers.push(p.name);
            }
            //get cell values
            $('#' + tableID + '>tbody>tr').each( //for each row in tableBody
                function () {
                    var dataRow = {};
                    for (c in cols) {
                        var columnSelector = 'td:nth-child(' + cols[c] + ')';
                        var text = $.trim($(this).find(columnSelector).text());
                        //var value = $.trim($(this).find(columnSelector).data("value"));
                        dataRow[headers[c]] = text;
                    }
                    jsonObj.push(dataRow);
                }
            );

        }

        return jsonObj;

    }
    function getDataFromColumn(params) {
        var tableID = params.tableID || "";
        var columnIndex = params.columnIndex || 1;
        var separator = params.separator || ";";
        var columnTexts = [];
        if (columnIndex > 0) {
            $('#' + tableID + '>tbody>tr>td:nth-child(' + columnIndex + ')').each(function () {
                columnTexts.push($(this).text());
            });
        }
        return columnTexts.join(separator);
    }
    function dump(obj) {
        var dumpResult = "";
        if (!$.isPlainObject(obj) && $.type(obj) !== "object") {
            dumpResult = "(" + getTypeName(obj) + "-" + $.type(obj) + ") :";
        }
        dumpResult += " <ul>";
        for (x in obj) {
            if (!($.type(obj[x]) === "string") && ($.isArray(obj[x]) || $.isPlainObject(obj[x]) || $.type(obj[x]) === "object")) {
                var simbols = ($.isArray(obj[x])) ? ["[", "]", "="] : ["{", "}", ":"];
                dumpResult += "<li>(" + getTypeName(obj[x]) + "-" + $.type(obj[x]);
                dumpResult += ") <b>" + x + "</b> ";
                dumpResult += simbols[2] + " " + simbols[0] + dump(obj[x]) + simbols[1];
                dumpResult += "</li>";
            }
            else {
                dumpResult += "<li>(" + getTypeName(obj[x]) + "-" + $.type(obj[x]) + ") ";
                dumpResult += "<b>" + x + "</b> : " + obj[x] + "</li>";
            }
        }
        dumpResult += "</ul>";
        return dumpResult;
    }
    function showDump(obj) {
        UI.messageBox(1, dump(obj));
    }
    function stripTags(str) {
        return str.replace(/<\/?[^>]+>/gi, '');
    }
    function getKeyByValue(obj, value) {
        for (x in obj) {
            if (obj[x] == value) { return x; }
        }
        return "";
    }

    function getLableFor(fieldID) {
        return $('label[for="' + fieldID + '"]').text();
    }

    function updateTableRelatedFieldValue(tableID) {
        var itemsName = $("#" + tableID).data('items-name');
        var params = new SETTINGS.tableDataToJason();
        params.tableID = tableID;
        params.subject = itemsName;

        var relatedField = $("#" + tableID).data('related-validation-field');
        var tableJsonData = UTILS.tableDataValuesToJson(params);

        if (tableJsonData.length <= 0) {
            $("#" + relatedField).val("");
        }
        else { $("#" + relatedField).val(JSON.stringify(tableJsonData)); }

    }
    function findDuplicatedDataRows(tableID, dataRow) {
        //gat all table.dataRows
        var settings = new SETTINGS.tableDataToJason();
        settings.tableID = tableID;
        settings.subject = "rows";
        settings.columnIndexes = null;

        //var jsonDataRows = UTILS.tableDataToJason(settings);
        var jsonDataRows = UTILS.tableDataValuesToJson(settings);
        var rowMatches = $();

        var selectedRowIndex = UI.getSelectedRow(tableID);
        for (var jdr in jsonDataRows) {
            if (UTILS.dataRowsAreEqual(dataRow, jsonDataRows[jdr]) && jdr != selectedRowIndex) {
                rowMatches = rowMatches.add($("#" + tableID + " tbody tr:eq(" + jdr + ")"));
            }
        }

        return rowMatches;
    }

    function valueInColumn(tableID, columnName, value) {
        //gat all table.dataRows
        var settings = new SETTINGS.tableDataToJason();
        settings.tableID = tableID;
        settings.subject = "rows";
        settings.columnIndexes = null;

        //var jsonDataRows = UTILS.tableDataToJason(settings);
        var jsonDataRows = UTILS.tableDataValuesToJson(settings);
        var matches = 0;

        for (var r in jsonDataRows) {
            var columnValue = eval(jsonDataRows[r][columnName]);
            if (columnValue == value) { matches++; }
        }

        return matches;

    }

    function verifyRequiredColumnValues(tableID) {
        var columnsProperties = UTILS.getTableColumnsProperties(tableID);
        //obtener las columnas con data-column-properties:requiredAndUniqueValue != undefined

        var verifyColumns = [];

        for (cp in columnsProperties) {
            var cProperties = columnsProperties[cp];
            var cpObj = $.parseJSON(cProperties);

            if (cpObj != null && cpObj != undefined) {

                if (cpObj.requiredAndUniqueValue != null && cpObj.requiredAndUniqueValue != undefined) {
                    verifyColumns.push({ "name": cpObj.name, "value": cpObj.requiredAndUniqueValue, "sourceField": cpObj.sourceField });
                }
            }
        }

        var errores = [];

        var c = 0;


        for (var vc in verifyColumns) {
            var matches = valueInColumn(tableID, verifyColumns[vc].name, verifyColumns[vc].value);
            if (matches > 1) { //there are too many
                errores.push("In <b>" + UTILS.getLableFor(tableID) + "</b> the value <b>" + verifyColumns[vc].value + "</b> appears more than once in the column <b>" + verifyColumns[vc].name + "</b>.");
            } else if (matches == 0) { //please specify one
                errores.push("In <b>" + UTILS.getLableFor(tableID) + "</b> the value <b>" + verifyColumns[vc].value + "</b> must be specified once for the column <b>" + verifyColumns[vc].name + "</b>.");
            }
        }

        return errores;

    }
    function verifyUniqueAndRequiredColumnValues(tableID, dataRow) {
        var columnsProperties = UTILS.getTableColumnsProperties(tableID);
        //obtener las columnas con data-column-properties:requiredAndUniqueValue != undefined

        var verifyColumns = [];



        for (cp in columnsProperties) {
            var cProperties = columnsProperties[cp];
            var cpObj = $.parseJSON(cProperties);

            if (cpObj != null && cpObj != undefined) {

                if (cpObj.requiredAndUniqueValue != null && cpObj.requiredAndUniqueValue != undefined) {
                    verifyColumns.push({ "name": cpObj.name, "value": cpObj.requiredAndUniqueValue, "sourceField": cpObj.sourceField });
                }
            }
        }


        //gat all table.dataRows
        var settings = new SETTINGS.tableDataToJason();
        settings.tableID = tableID;
        settings.subject = "rows";
        settings.columnIndexes = null;

        //var jsonDataRows = UTILS.tableDataToJason(settings);
        var jsonDataRows = UTILS.tableDataValuesToJson(settings);
        if (jsonDataRows.length > 0) {


            var rowMatches = $();
            var selectedRowIndex = UI.getSelectedRow(tableID);

            var errores = [];
            //errores.length = verifyColumns.length;

            var c = 0;

            for (var jdr in jsonDataRows) {
                //console.log("Json row index : " + jdr);
                //console.log("Selected row index : " + selectedRowIndex);
                for (var vc in verifyColumns) {
                    var rowValue = jsonDataRows[jdr][verifyColumns[vc].name];
                    var colValue = verifyColumns[vc].value;

                    var parsedRowValue = eval(rowValue);
                    var parsedJdr = eval(jdr);
                    var parserDataRowValue = eval(dataRow[verifyColumns[vc].name]);

                    if (selectedRowIndex > -1) { // updating
                        if (parsedRowValue == colValue && parsedJdr != selectedRowIndex && parserDataRowValue == colValue) {
                            //errores.push({ name: verifyColumns[vc].name, value: colValue, sourceField: verifyColumns[vc].sourceField });
                            errores[eval(vc)] = { name: verifyColumns[vc].name, value: colValue, sourceField: verifyColumns[vc].sourceField };
                        }
                    } else { //inserting
                        if (parsedRowValue == colValue && parserDataRowValue == colValue) {
                            //errores.push({ name: verifyColumns[vc].name, value: colValue, sourceField: verifyColumns[vc].sourceField });
                            errores[eval(vc)] = { name: verifyColumns[vc].name, value: colValue, sourceField: verifyColumns[vc].sourceField };
                        }
                    }
                }

            }
            return { rowMatches: rowMatches, errores: errores };
        }
        else { return { rowMatches: rowMatches, errores: new Array() }; }

    }

    function verifyUniqueAndRequiredColumns(tableID, dataRow) {
        var columnsProperties = UTILS.getTableColumnsProperties(tableID);
        //obtener las columnas con data-column-properties:requiredAndUnique != undefined

        var verifyColumns = [];
        var specifiedColumns = [];



        for (cp in columnsProperties) {
            var cProperties = columnsProperties[cp];
            var cpObj = $.parseJSON(cProperties);

            if (cpObj != null && cpObj != undefined) {

                if (cpObj.requiredAndUnique != null && cpObj.requiredAndUnique != undefined) {
                    verifyColumns.push({ "name": cpObj.name, "sourceField": cpObj.sourceField });
                    specifiedColumns.push(eval(cp) + 1);
                }
            }
        }


        //gat all table.dataRows
        var settings = new SETTINGS.tableDataToJason();
        settings.tableID = tableID;
        settings.subject = "rows";
        settings.columnIndexes = null;

        //var jsonDataRows = UTILS.tableDataToJason(settings);
        var jsonDataRows = UTILS.tableDataValuesToJson(settings);
        if (jsonDataRows.length > 0) {


            var rowMatches = $();
            var selectedRowIndex = UI.getSelectedRow(tableID);

            var errores = [];
            //errores.length = verifyColumns.length;

            var c = 0;

            for (var jdr in jsonDataRows) {
                var jsonDataRow = jsonDataRows[jdr];
                for (var vc in verifyColumns) {
                    var colName = verifyColumns[vc].name;
                    var colValue = verifyColumns[vc].value;
                    var colSourceField = verifyColumns[vc].sourceField;
                    var rowValue = jsonDataRow[colName];

                    //var parsedRowValue = eval(rowValue);
                    var parsedRowValue = rowValue;
                    var parsedJdr = eval(jdr);
                    //var parsedDataRowValue = eval(dataRow[verifyColumns[vc].name]);
                    var parsedDataRowValue = dataRow[colName];
                    //var s = new SETTINGS.searchTextInColumns();
                    //s.text = parsedDataRowValue;
                    //s.searchType = "containsExact";
                    //s.tableID = tableID;
                    //s.specifiedColumns = specifiedColumns;

                    //var matches = UTILS.searchTextInColumns(s);

                    if (selectedRowIndex > -1) { // updating
                        //if the text es empty or the text exists in table and the row is diferent
                        if (parsedDataRowValue.toString == "" || (parsedJdr != (selectedRowIndex) && parsedRowValue == parsedDataRowValue)) {
                            errores[eval(vc)] = { name: colName, value: parsedDataRowValue, sourceField: colSourceField };
                        }
                    } else { //inserting
                        //if the text es empty or the text exists in table and the row is diferent
                        if (parsedDataRowValue.toString == "" || (parsedRowValue == parsedDataRowValue)) {
                            errores[eval(vc)] = { name: colName, value: parsedDataRowValue, sourceField: colSourceField };
                        }
                    }
                }
            }
            return { rowMatches: rowMatches, errores: errores };
        }
        else { return { rowMatches: rowMatches, errores: new Array() }; }

    }

    function dataRowsAreEqual(row1, row2) {
        for (var x in row1) {
            if ($.trim(row1[x]) != $.trim(row2[x])) { return false }
        }
        return true;
    }

    function startsWithVowel(text) {
        var vowelRegex = /^[aeiou]/;
        if (text.match(vowelRegex)) {
            return true;
        } else { return false; }
    }
    function dataRowWarningTextsToJson(tableID, rowIndex) {
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);
        var cols = UTILS.getAllTableDataColumnIndexes(tableID);
        var headers = [];
        var dataRow = {};

        if (cols.length > 0) {//get Header text for each column
            for (c in cols) {
                var columnSelector = '#' + tableID + '>thead>tr:'
                columnSelector += columnsRowSelector
                columnSelector += '>th:nth-child(' + cols[c] + ')';

                var column = $(columnSelector);
                var p = $.parseJSON(column.data("column-properties"));
                if (p.warnOnDeleting == true) {
                    headers.push({ name: $.trim(column.text()), nth: parseInt(c) + 1 });

                }
            }
            //get cell values only if there are columns for warning
            if (headers.length > 0) {
                for (h in headers) {
                    var cellSelector = '#' + tableID + '>tbody>tr:eq(' + rowIndex + ') td:nth-child(' + headers[h].nth + ')';
                    var text = $.trim($(cellSelector).text());
                    //var value = $.trim($(this).find(columnSelector).data("value"));
                    dataRow[headers[h].name] = text;
                }
            }

        }

        return dataRow;
    }
    function dataRowValuesToJson(tableID, rowIndex) {
        var columnsRowSelector = UTILS.getColumnsRowSelector(tableID);
        var cols = UTILS.getAllTableDataColumnIndexes(tableID);
        var headers = [];
        var dataRow = {};

        if (cols.length > 0) {//get Header text for each column
            for (c in cols) {
                var columnSelector = '#' + tableID + '>thead>tr:'
                columnSelector += columnsRowSelector
                columnSelector += '>th:nth-child(' + cols[c] + ')';

                var column = $(columnSelector);
                var p = $.parseJSON(column.data("column-properties"));

                headers.push($.trim(p.name));

            }

            if (headers.length > 0) {
                for (c in cols) {
                    var cellSelector = '#' + tableID + '>tbody>tr:eq(' + rowIndex + ') td:nth-child(' + cols[c] + ')';

                    var value = $(cellSelector).data("value");
                    dataRow[headers[c]] = value;
                }
            }

        }

        return dataRow;
    }
    function arrayToListItems(errors) {
        var listItems = "";
        for (var x in errors) {
            listItems += "<li>" + errors[x] + "</li>";
        }

        return listItems;
    }
    function cascadeResetDescendants(ddlId) {
        var cascadingGroup = $("#" + ddlId).data('cascading-group');
        var selector = "[data-cascading-group='" + cascadingGroup + "'][data-cascading-parent='" + ddlId + "']";
        $(selector).each(function () {
            var cascadingDdlID = $(this).attr("id");
            var cascadingDdlSelector = "#" + cascadingDdlID;
            //$(cascadingDdlSelector).clearSelect();

            var ddlLabel = UTILS.getLableFor(ddlId);
            var n = "";
            if (UTILS.startsWithVowel(ddlLabel)) { n = "n"; }

            var itemText = "--Select a" + n + " " + ddlLabel + "--";
            $(cascadingDdlSelector).append($('<option></option>').val("0").html(itemText));



            // var dataSelectedValue = $(cascadingDdlSelector).data('selected-value');
            // if (dataSelectedValue != undefined && dataSelectedValue !="") { 
            // if ($(cascadingDdlSelector + "  option[value='" + dataSelectedValue + "']").length == 0) {
            //     //if the options to select does not exists on the ddl, add it, so we can keep the value when submiting, and not causing
            //     // the deletion of that value on the DB.
            //     var singleItemMethod = $(cascadingDdlSelector).data('single-item-method');
            //     var singleItemParameterName = $(cascadingDdlSelector).data('single-item-parameter-name');
            //     if (singleItemMethod != undefined && singleItemMethod != "" && singleItemParameterName != undefined && singleItemParameterName != "") { 

            //         var _dataParams = {};
            //         _dataParams[singleItemParameterName] = dataSelectedValue;
            //     $.ajax({
            //         url: singleItemMethod,
            //         type: "POST",
            //         data: _dataParams,
            //         complete: function (_jqXHR, textStatus) {
            //             var _json = $.parseJSON(_jqXHR.responseText);
            //             var option = new Option(_json.Text, _json.Value);
            //             $(cascadingDdlSelector).prepend("<option selected='selected' value='" + _json.Value + "'>" + _json.Text + "</option>");

            //             window.setTimeout(function () {
            //                 $(cascadingDdlSelector).val(dataSelectedValue);
            //                 $(cascadingDdlSelector).trigger('change');
            //                 //$(cascadingDdlSelector).data('selected-value', ''); // clear the data attribute
            //                 //in this case we don't clean the selected value, so next time the parent ddl changes, the selected option remains selected

            //                 // // //

            //                 var ddlLabel = UTILS.getLableFor(cascadingDdlID);
            //                 var parentID = $(cascadingDdlSelector).data('cascading-parent');
            //                 var parentSelectedText = $("#" + parentID + " option:selected").text();
            //                 if (parentSelectedText == "" || parentSelectedText == "--Select One--") { parentSelectedText = "[None]"; }
            //                 var parentLabel = UTILS.getLableFor(parentID);

            //                 var warning = "The selected <u>" + ddlLabel + "</u> <strong>" + _json.Text + "</strong> ";
            //                 warning += "is not related to the selected <u>" + parentLabel + "</u> <strong>" + parentSelectedText + "</strong>.";
            //                 warning += "<br />Please, confirm this information when saving or updating.";


            //                 if ($(cascadingDdlSelector + "  option[value='" + dataSelectedValue + "']").length == 0) {
            //                     UI.messageBox(0, warning, 5);
            //                 }

            //                 // // //

            //             },500);

            //         }
            //     });
            //     }
            // }
            //}

            cascadeResetDescendants(cascadingDdlID);
        });
    }

    function removeNonMissingOptions(ddlID) {
        $("#" + ddlID + " option").each(function () {
            if (!$(this).is('[data-missing-option]')) {
                $(this).remove();
            }
        });
    }

    function cascadeDropDowns(ddlId) {
        cascadeResetDescendants(ddlId);

        var cascadingGroup = $("#" + ddlId).data('cascading-group');
        var ddlSelectedValue = $("#" + ddlId).val();
        ddlSelectedValue = ddlSelectedValue == null || ddlSelectedValue == '' || ddlSelectedValue == 0 ? $("#" + ddlId).data('selected-value') : ddlSelectedValue;//mike
        var selector = "[data-cascading-group='" + cascadingGroup + "'][data-cascading-parent='" + ddlId + "']";
        if (ddlSelectedValue != null && ddlSelectedValue != "") {
            if (ddlSelectedValue != "0") {
                var feedingMethods = {};
                $(selector).each(function () {
                    var feedingMethod = $(this).data('cascading-feeding-method');
                    var feedingParameterName = $(this).data('cascading-feeding-parameter-name');
                    //mike
                    var extraParameterName = $(this).data('additional-parameter-name') != undefined ? $(this).data('additional-parameter-name') : '';
                    var extraParameterSourceField = $(this).data('additional-parameter-source-field') != undefined ? $(this).data('additional-parameter-source-field') : '';
                    feedingMethods[feedingMethod] = { method: feedingMethod, paramName: feedingParameterName, extraParam: extraParameterName, extraSourceField: extraParameterSourceField };
                    ///
                    //feedingMethods[feedingMethod] = { method: feedingMethod, paramName: feedingParameterName };
                });

                for (var fmethod in feedingMethods) {
                    var _methodName = feedingMethods[fmethod].method;
                    var _paramName = feedingMethods[fmethod].paramName;
                    var dataParams = {};
                    dataParams[_paramName] = ddlSelectedValue;
                    //mike
                    if (feedingMethods[fmethod].extraParam != '') {
                        dataParams[feedingMethods[fmethod].extraParam] = $('#' + feedingMethods[fmethod].extraSourceField).val();
                    }
                    ///
                    $.ajax({
                        async: false,
                        url: _methodName,
                        type: "POST",
                        data: dataParams,
                        complete: function (jqXHR, textStatus) {
                            var selectorByMethod = "[data-cascading-group='" + cascadingGroup + "'][data-cascading-feeding-method='" + _methodName + "']";
                            $(selectorByMethod).each(
                                function (index, item) {
                                    var cascadingDdlId = $(this).attr("id");
                                    var cascadingSelector = "#" + cascadingDdlId;
                                    if (textStatus == "success") {
                                        var json = $.parseJSON(jqXHR.responseText);
                                        if (json.length > 0) {
                                            var itemText = { "Selected": false, "Text": "--Select One--", "Value": "0" };
                                            if (!$(this).is('[data-cascading-exclude-default]')) {//mike
                                                json.splice(0, 0, itemText); //le agregamos la opcion al json antes de mandarselo a fillSelect, por que fillSelect triggerea onchange
                                                //y eso haria que se disparara el cascading antes de agregar la opcion --Select One-- al dropDown
                                            }
                                            $(cascadingSelector).clearSelect();
                                            $(cascadingSelector).fillSelect(json);
                                        }
                                        else {
                                            $(cascadingSelector).clearSelect();///
                                            var itemText = "--No " + UTILS.getLableFor(cascadingDdlId) + "s Found--";
                                            $(cascadingSelector).append($('<option></option>').val("0").html(itemText));
                                        }
                                        $(cascadingSelector).data('parent-selected-value', ddlSelectedValue);
                                    } else {
                                        var itemText = "--Error Retrieving " + UTILS.getLableFor(cascadingDdlId) + "s --";
                                        $(cascadingSelector).append($('<option></option>').val("0").html(itemText).data("missing-option", "true"));
                                    }
                                }
                            );
                        }
                    });
                }
            }
            else {
                $(selector).each(function () {
                    addMissingOptionAndWarn($(this).attr('id'));
                });
            }

        }
        // // // //

        var cascadingSelector = "#" + ddlId;
        var dataSelectedValue = $(cascadingSelector).data('selected-value');
        dataSelectedValue = dataSelectedValue == undefined || dataSelectedValue == '' ? $(cascadingSelector).val() : dataSelectedValue;//mike
        if (dataSelectedValue != undefined && dataSelectedValue.toString() != "") {
            if ($(cascadingSelector + "  option[value='" + dataSelectedValue + "']").length == 0) {
                addMissingOptionAndWarn($(cascadingSelector).attr("id"));
            }
            else {
                // $(cascadingSelector).trigger('change');
                window.setTimeout(function () { $(cascadingSelector).val(dataSelectedValue); }, 500);
            }
        }
    }
    function addMissingOptionAndWarn(ddlID) {
        var cascadingDdlID = ddlID;
        var cascadingDdlSelector = "#" + cascadingDdlID;
        //(cascadingDdlSelector).clearSelect();        
        //removeNonMissingOptions(ddlID);

        var dataSelectedValue = $(cascadingDdlSelector).data('selected-value');
        var singleItemMethod = $(cascadingDdlSelector).data('single-item-method');
        var singleItemParameterName = $(cascadingDdlSelector).data('single-item-parameter-name');

        if (dataSelectedValue != undefined && dataSelectedValue != ""
            && singleItemMethod != undefined && singleItemMethod != ""
            && singleItemParameterName != undefined && singleItemParameterName != ""
            ) {
            var _dataParams = {};
            _dataParams[singleItemParameterName] = dataSelectedValue;
            $.ajax({
                url: singleItemMethod, type: "POST", data: _dataParams,
                complete: function (_jqXHR, textStatus) {
                    var _json = $.parseJSON(_jqXHR.responseText);
                    var option = new Option(_json.Text, _json.Value);
                    $(cascadingDdlSelector).prepend("<option data-missing-option='true' selected='selected' value='" + _json.Value + "'>" + _json.Text + "</option>");
                    window.setTimeout(function () { // wait a bit so the next line works  ( .val ).
                        $(cascadingDdlSelector).val(dataSelectedValue); // make the just-added option selected and visible
                        $(cascadingDdlSelector).trigger('change'); // trigger the change event as if the selected value was just selected so we can update dependand dropdowns                        
                        var ddlLabel = UTILS.getLableFor(cascadingDdlID);
                        var parentID = $(cascadingDdlSelector).data('cascading-parent');
                        var parentSelectedText = $("#" + parentID + " option:selected").text();
                        if (parentSelectedText == "" || parentSelectedText == "--Select One--") { parentSelectedText = "[None]"; }
                        var parentLabel = UTILS.getLableFor(parentID);
                        var warning = "The selected <u>" + ddlLabel + "</u> <strong>" + _json.Text + "</strong> ";
                        warning += "is not related to the selected <u>" + parentLabel + "</u> <strong>" + parentSelectedText + "</strong>.";
                        warning += "<br />Please, confirm this information when saving or updating.";
                        if ($(cascadingDdlSelector + "  option[data-missing-option]").length > 0) {
                            UI.messageBox(0, warning, 5);
                        }
                    }, 500);
                }
            });
        }
    }
    function JsonToTable(tableID, json) {

        //get table headers

        //for each header get its content from the json matching by name

        //add functionality to table.

    }
    function getURLParameter(name, url) {
        var queryString = "";

        if (url != null) {
            //get queryString from URL
            var urlParts = url.split("?");
            queryString = (urlParts.length > 1) ? '?' + urlParts[1] : urlParts[0];
        } else {
            queryString = location.search;
        }

        return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)', 'i').exec(queryString) || [, ""])[1].replace(/\+/g, '%20')) || null;
    }
    function isSetURLParameter(name, url) {

        var queryString = "";

        if (url != null) {
            //get queryString from URL
            var urlParts = url.split("?");
            queryString = (urlParts.length > 1) ? urlParts[1] : urlParts[0];
        } else {
            queryString = location.search;
        }

        return (new RegExp('[?|&]' + name + '(?:[=|&|#|;|]|$)', 'i').exec(queryString) !== null)
    }

    function getServerTimeStamp() {
        $.post("/CRM/masterchart/getServerTimestamp", function (json) {
            return json;
        });
    }

    function getClientTimezoneOffset() {
        $.post("/CRM/masterchart/getServerTimestamp", function (json) {
            var st = json;
            //new Date(year, month, day, hours, minutes, seconds, milliseconds)

            var serverDateTime = new Date(st.year, parseInt(st.month) - 1, st.day, st.hour);
            var tempdt = new Date();
            var clientDateTime = new Date(tempdt.getFullYear(), tempdt.getMonth(), tempdt.getDate(), tempdt.getHours());

            var hoursDiference = Math.floor((serverDateTime - clientDateTime) / 3600000);

            //if (hoursDiference > 0) {

            //    if (serverDateTime < clientDateTime) { hoursDiference = hoursDiference * (-1); }
            //}

            return hoursDiference;
        });




    }

    function validIsoDatePart(value) {
        // return the matching part of value, this way we can make sure we get the part that matched the regexp when using UTILS.isIsoDate, so we dont wet parsing errors.
        var re = new RegExp(/(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d)(:[0-5]\d(\.(\d{3}|\d{2}|\d{1}))?)?((Z)|((\+|\-)\d{2}:\d{2})?)?/);
        try {
            var result = value.match(re);

            if (value.match(re)) {
                return result[0];
            } else {
                return "";
            }
        } catch (e) {
            return "";
        }
    }

    function isIsoDate(value) {
        var re = new RegExp(/(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d)(:[0-5]\d(\.(\d{3}|\d{2}|\d{1}))?)?((Z)|((\+|\-)\d{2}:\d{2})?)?/);
        try {
            if (value.match(re)) {
                return true;
            } else {
                return false;
            }
        } catch (e) {
            return false;
        }

    }

    function getFullDayName(day) {
        var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        return days[day];
    }

    var parseSerializedDate = function(dateStr){
        var re = /-?\d+/;
        var m = re.exec(dateStr);
        var d = new Date(parseInt(m[0]));
        return d.yyyymmdd() + ' ' + ('0' + d.getHours().toString()).slice(-2) + ':' + ('0' + d.getMinutes().toString()).slice(-2) + ':' + ('0' + d.getSeconds().toString()).slice(-2);
    }

    var addDays = function(date, amount) {
        var tzOff = date.getTimezoneOffset() * 60 * 1000,
            t = date.getTime(),
            d = new Date(),
            tzOff2;

        t += (1000 * 60 * 60 * 24) * amount;
        d.setTime(t);

        tzOff2 = d.getTimezoneOffset() * 60 * 1000;
        if (tzOff != tzOff2) {
            var diff = tzOff2 - tzOff;
            t += diff;
            d.setTime(t);
        }

        return d;
    }

    return {
        searchTextInColumns: searchTextInColumns,
        addDataRowsToTable: addDataRowsToTable,
        clearTableRows: clearTableRows,
        rangeToArray: rangeToArray,
        getInfoAsync: getInfoAsync,
        removePropertiesFromJson: removePropertiesFromJson,
        jsonToFormFields: jsonToFormFields,
        dump: dump,
        showDump: showDump,
        arrayToTableRows: arrayToTableRow,
        removeDataRowsFromTable: removeDataRowsFromTable,
        toArray: toArray,
        stripTags: stripTags,
        getDataFromColumn: getDataFromColumn,
        getKeyByValue: getKeyByValue,
        searchTextInColumns_hilightTimeOut: searchTextInColumns_hilightTimeOut,
        tableDataToJason: tableDataToJason,
        getLableFor: getLableFor,
        addJsonRowToTable: addJsonRowToTable,
        getAllTableColumnReferences: getAllTableColumnReferences,
        getColumnsRowSelector: getColumnsRowSelector,
        getTableColumnsProperties: getTableColumnsProperties,
        getAllTableDataColumnIndexes: getAllTableDataColumnIndexes,
        updateTableRelatedFieldValue: updateTableRelatedFieldValue,
        findDuplicatedDataRows: findDuplicatedDataRows,
        dataRowsAreEqual: dataRowsAreEqual,
        tableDataValuesToJson: tableDataValuesToJson,
        tableDataTextsToJson: tableDataTextsToJson,
        dataRowWarningTextsToJson: dataRowWarningTextsToJson,
        dataRowValuesToJson: dataRowValuesToJson,
        startsWithVowel: startsWithVowel,
        getAllTableDataColumnReferences: getAllTableDataColumnReferences,
        dataTable: dataTable,
        updateJsonRowToTable: updateJsonRowToTable,
        tableDataRowValuesToJson: tableDataRowValuesToJson,
        arrayToListItems: arrayToListItems,
        getInputValueByType: getInputValueByType,
        setInputValueByType: setInputValueByType,
        verifyUniqueAndRequiredColumnValues: verifyUniqueAndRequiredColumnValues,
        verifyUniqueAndRequiredColumns: verifyUniqueAndRequiredColumns,
        verifyRequiredColumnValues: verifyRequiredColumnValues,
        valueInColumn: valueInColumn,
        cascadeDropDowns: cascadeDropDowns,
        JsonToTable: JsonToTable,
        serializedDateToDate: serializedDateToDate,
        getURLParameter: getURLParameter,
        isSetURLParameter: isSetURLParameter,
        isIsoDate: isIsoDate,
        validIsoDatePart: validIsoDatePart,
        getFullDayName: getFullDayName,
        parseSerializedDate: parseSerializedDate,
        addDays: addDays
    }
}();

//Extendind Jquery:

$.extend($.expr[":"], {
    containsExact: $.expr.createPseudo ?
     $.expr.createPseudo(function (text) {
         return function (elem) {
             return $.trim(elem.innerHTML.toLowerCase()) === text.toLowerCase();
         };
     }) :
     // support: jQuery <1.8
     function (elem, i, match) {
         return $.trim(elem.innerHTML.toLowerCase()) === match[3].toLowerCase();
     },

    containsExactCase: $.expr.createPseudo ?
     $.expr.createPseudo(function (text) {
         return function (elem) {
             return $.trim(elem.innerHTML) === text;
         };
     }) :
     // support: jQuery <1.8
     function (elem, i, match) {
         return $.trim(elem.innerHTML) === match[3];
     },

    containsRegex: $.expr.createPseudo ?
     $.expr.createPseudo(function (text) {
         var reg = /^\/((?:\\\/|[^\/]) )\/([mig]{0,3})$/.exec(text);
         return function (elem) {
             return RegExp(reg[1], reg[2]).test($.trim(elem.innerHTML));
         };
     }) :
     // support: jQuery <1.8
     function (elem, i, match) {
         var reg = /^\/((?:\\\/|[^\/]) )\/([mig]{0,3})$/.exec(match[3]);
         return RegExp(reg[1], reg[2]).test($.trim(elem.innerHTML));
     }

});

Date.prototype.yyyymmdd = function () {
    var mm = this.getMonth() + 1; // getMonth() is zero-based
    var dd = this.getDate();

    return [this.getFullYear(),
            (mm > 9 ? '' : '0') + mm,
            (dd > 9 ? '' : '0') + dd
    ].join('-');
};