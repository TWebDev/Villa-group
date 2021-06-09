$(function () {
    REPORT.init()

    $('#SearchIndicator_I_ArrivalDate').datepicker({
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        onClose: function (dateText, inst) {
            if ($('#SearchIndicator_F_ArrivalDate').val() != '') {
                if (dateText != '') {
                    var fromDate = $('#SearchIndicator_I_ArrivalDate').datepicker('getDate');
                    var toDate = $('#SearchIndicator_F_ArrivalDate').datepicker('getDate');
                    if (fromDate > toDate)
                        $('#SearchIndicator_F_ArrivalDate').datepicker('setDate', fromDate);
                }
                else {
                    $('#SearchIndicator_F_ArrivalDate').val(dateText);
                }
            }
            else {
                $('#SearchIndicator_F_ArrivalDate').val(dateText);
            }
        },
        onSelect: function (selectedDate) {
            if ($('#SearchIndicator_F_ArrivalDate').val() != '') {
                $('#SearchIndicator_F_ArrivalDate').datepicker('setDate', $('#SearchIndicator_F_ArrivalDate').datepicker('getDate'));
            }
            $('#SearchIndicator_F_ArrivalDate').datepicker('option', 'minDate', $('#SearchIndicator_I_ArrivalDate').datepicker('getDate'));
        }
    });

    $('#SearchIndicator_F_ArrivalDate').datepicker({
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        onClose: function (dateText, inst) {
            if ($('#SearchIndicator_I_ArrivalDate').val() != '') {
                var fromDate = $('#SearchIndicator_I_ArrivalDate').datepicker('getDate');
                var toDate = $('#SearchIndicator_F_ArrivalDate').datepicker('getDate');
                if (fromDate > toDate)
                    $('#SearchIndicator_I_ArrivalDate').datepicker('setDate', toDate);
            }
            else {
                $('#SearchIndicator_I_ArrivalDate').val(dateText);
                //$('#SearchIndicator_I_ArrivalDate').val($('#SearchIndicator_F_ArrivalDate').datepicker('getDate'));
            }
        },
        onSelect: function (selectedDate) {
            if ($('#SearchIndicator_I_ArrivalDate').val() != '') {
                $('#SearchIndicator_I_ArrivalDate').datepicker('setDate', $('#SearchIndicator_I_ArrivalDate').datepicker('getDate'));
            }
            $('#SearchIndicator_I_ArrivalDate').datepicker('option', 'maxDate', $('#SearchIndicator_F_ArrivalDate').datepicker('getDate'));
        }
    });

    $('#SearchIndicator_I_TourDate').datepicker({
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        onClose: function (dateText, inst) {
            if ($('#SearchIndicator_F_TourDate').val() != '') {
                if (dateText != '') {
                    var fromDate = $('#SearchIndicator_I_TourDate').datepicker('getDate');
                    var toDate = $('#SearchIndicator_F_TourDate').datepicker('getDate');
                    if (fromDate > toDate)
                        $('#SearchIndicator_F_TourDate').datepicker('setDate', fromDate);
                }
                else {
                    $('#SearchIndicator_F_TourDate').val(dateText);
                }
            }
            else {
                $('#SearchIndicator_F_TourDate').val(dateText);
            }
        },
        onSelect: function (selectedDate) {
            if ($('#SearchIndicator_F_TourDate').val() != '') {
                $('#SearchIndicator_F_TourDate').datepicker('setDate', $('#SearchIndicator_F_TourDate').datepicker('getDate'));
            }
            $('#SearchIndicator_F_TourDate').datepicker('option', 'minDate', $('#SearchIndicator_I_TourDate').datepicker('getDate'));
        }
    });

    $('#SearchIndicator_F_TourDate').datepicker({
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        onClose: function (dateText, inst) {
            if ($('#SearchIndicator_I_TourDate').val() != '') {
                var fromDate = $('#SearchIndicator_I_TourDate').datepicker('getDate');
                var toDate = $('#SearchIndicator_F_TourDate').datepicker('getDate');
                if (fromDate > toDate)
                    $('#SearchIndicator_I_TourDate').datepicker('setDate', toDate);
            }
            else {
                $('#SearchIndicator_I_TourDate').val(dateText);
            }
            $(this).removeClass('input-validation-error');
        },
        onSelect: function (selectedDate) {
            if ($('#SearchIndicator_I_TourDate').val() != '') {
                $('#SearchIndicator_I_TourDate').datepicker('setDate', $('#SearchIndicator_I_TourDate').datepicker('getDate'));
            }
            $('#SearchIndicator_I_TourDate').datepicker('option', 'maxDate', $('#SearchIndicator_F_TourDate').datepicker('getDate'));
            $(this).removeClass('input-validation-error');
        }
    });

    //activity reports
    $('#tblActivityLogs tbody tr').remove();
    REPORT.bindDatePickerToFields();

    $('#SearchFromDate').on('focusout', function () {
        var date = $(this).val().split(' ');
        if (date[0] < '2018-03-06' && date != "") {
            UI.messageBox(0, 'THE REQUEST CAN´T SEARCH TERMINAL IN DATE´S LOWERS TO 2018-03-06', 10);
        }

    });
    //  UI.messageBox(0, 'YOU CAN SELECT A PROMO WITH DISCOUNT TYPE AT THE SAME TIME', 7);


});

var REPORT = function () {

    var oIndicatorsTable;

    var oDuplicateTable;

    var init = function () {
        REPORT.Audit.init();
        REPORT.CouponsHistory.init();
        function split(value) {
            return value.split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
        }

        //$.fn.slowEach = function (interval, callback) {
        //    var array = this;
        //    if (!array.length) return;
        //    var i = 0;
        //    next();
        //    function next() {
        //        if (callback.call(array[i], i, array[i]) !== false)
        //            if (++i < array.length)
        //                setTimeout(next, interval);
        //    }
        //};

        $('select[multiple="multiple"]').multiselect({
            noneSelectedText: "--All--",
            minWidth: "auto", selectedList: 1
        }).multiselectfilter();

        $('.custom-multiple-label').each(function () {
            var label = $(this).attr('data-multiple-default-label');
            $(this).multiselect({ noneSelectedText: label });
        });

        $.getJSON('/crm/Reports/GetDDLData', { itemType: 'leadSourceID' }, function (data) {
            $('#SearchIndicator_LeadSource').fillSelect(data);
            $('#GenericSearchFieldsReport_LeadSource').fillSelect(data);
        });

        REPORT.searchResultsTable($(document).find('table').first());

        //$('.prevent-default').on('keypress', function (e) {
        //    e.preventDefault();
        //});
        $('.prevent-default').on('keydown', function (e) {
            e.preventDefault();
        });

        $('#btnClearArrivalDates').on('click', function () {
            $('#SearchIndicator_I_ArrivalDate').val('');
            $('#SearchIndicator_F_ArrivalDate').val('');
        });

        $('#btnClearTourDates').on('click', function () {
            $('#SearchIndicator_I_TourDate').val('');
            $('#SearchIndicator_F_TourDate').val('');
        });

        $('#btnSearchDynamic').on('click', function () {
            var array = new Array();
            $('#divColumnHeadersContainer').find('p').each(function () {
                array.push($(this).attr('id'));
                //array.push($(this).attr('id') + '_' + $(this)[0].textContent);
            });
            $('#SearchDynamic_Columns').val(array);
            if ($('#SearchDynamic_Columns').val().length > 0) {
                $('#frmSearchDynamic').submit();
            }
            else {
                UI.messageBox(0, "Select at least one column to display", null, null);
            }
        });

        $('#btnEditLayout').on('click', function () {
            if ($('#Search_Layout').val() > 0) {
                $.getJSON('/Reports/GetReportLayout/' + $('#Search_Layout').val(), null, function (data) {
                    //if (data.LayoutID != null) {
                    $('#LayoutID').val(data.LayoutID);
                    $('#btnSubmitLayout').show();
                    $('#btnDeleteLayout').show();
                    $('#btnDeleteLayout').off('click').on('click', function () {
                        $.post('/Reports/DeleteReportLayout/' + data.LayoutID, null, function (data) {
                            if (data.ResponseType == 1) {
                                $('#fdsLayoutInfo').slideUp('fast');
                                $('#Search_Layout option[value=' + $('#LayoutID').val() + ']').remove();
                            }
                            UI.messageBox(data.ResponseType, data.ResponseMessage);
                        }, 'json');
                    });
                    //} else {
                    //    $('#btnSubmitLayout').hide();
                    //    $('#btnDeleteLayout').hide();
                    //}

                    $('#Layout').val(data.Layout);
                    $('#Culture').val(data.Culture);

                    $('#TerminalID').val(data.TerminalID);
                    $('#PointOfSaleID').val(data.PointOfSaleID);
                    $.each(data.SelectedServices, function (s, service) {
                        $('#SelectedServices option[value=' + service + ']').attr('selected', 'selected');
                    });
                    $.each(data.SelectedPriceTypes, function (t, type) {
                        $('#SelectedPriceTypes option[value=' + type + ']').attr('selected', 'selected');
                    });
                    $.each(data.SelectedRoles, function (r, role) {
                        $('#SelectedRoles option[value=' + role + ']').attr('selected', 'selected');
                    });
                    $.each(data.SelectedCurrencies, function (c, currency) {
                        $('#SelectedCurrencies option[value=' + currency + ']').attr('selected', 'selected');
                    });

                    $('#SelectedServices').multiselect('refresh');
                    $('#SelectedPriceTypes').multiselect('refresh');
                    $('#SelectedRoles').multiselect('refresh');
                    $('#SelectedCurrencies').multiselect('refresh');

                    $('#LayoutUser').text(data.User);
                    $('#LayoutDate').text(data.DateSaved);

                    $('#fdsLayoutInfo').slideDown('fast');

                    $('#btnCloseLayout').off('click').on('click', function () {
                        $('#fdsLayoutInfo').slideUp('fast');
                    });
                });
            }
        });

        $('#Search_Layout').on('change', function () {
            if ($('#Search_Layout').val() == "-1") {
                $('#fdsLayoutInfo').slideDown('fast');
                $('#LayoutID').val('');
                $('#Layout').val('');
                $('#TerminalID').val('');
                $('#SelectedServices').val('');
                $('#SelectedPriceTypes').val('');
                $('#SelectedRoles').val('');
                $('#SelectedCurrencies').val('');
                $('#LayoutUser').text('');
                $('#LayoutDate').text('');
                $('#btnDeleteLayout').hide();
                $('#btnCloseLayout').off('click').on('click', function () {
                    $('#fdsLayoutInfo').slideUp('fast');
                });
            }
        });

        $('.workgroup-dependent-list').on('loaded', function () {
            $(this).each(function () {
                var id = $(this).attr('id');
                var route = $(this).attr('data-route');
                var parameter = $(this).attr('data-route-parameter');
                $.getJSON(route, { itemType: parameter, itemID: UI.selectedWorkGroup }, function (data) {
                    $('#' + id).fillSelect(data);
                });
            });
        });

        //$('.terminal-dependent-list').on('loaded', function () {
        //    $(this).each(function () {
        //        var id = $(this).attr('id');
        //        var route = $(this).attr('data-route');
        //        var parameter = $(this).attr('data-route-parameter');
        //        $.getJSON(route, { itemType: parameter, id: UI.selectedTerminals }, function (data) {
        //            $('#' + id).fillSelect(data);
        //        });
        //    });
        //});
        UI.updateListsOnTerminalsChange();

        //for the moment, the call is only made for dynamic report page
        if ($(document).find('table').length <= 0) {
            REPORT.callsForDynamicReport();
        }

        $('#MassUpdate_Terminal').on('change', function () {
            if ($(this).val() == '0') {
                $('#MassUpdate_AssignedToUser').clearSelect();
                $('#MassUpdate_AssignedToUser').append('<option value="">--Select Terminal--</option>');
            }
            else {
                $.getJSON('/MasterChart/GetDDLData', { itemType: 'users', itemID: $(this).val() }, function (data) {
                    $('#MassUpdate_AssignedToUser').fillSelect(data);
                });
            }
        });

        $('#btnMassUpdate').unbind('click').on('click', function () {
            var leads = '';

            $('#tblDuplicateLeadsResults tbody').find('.chk-son:checked').each(function () {
                leads = leads + (leads == '' ? '' : ',') + $(this).attr('id');
            });
            leads = "'" + leads.split(',').join("','") + "'";
            //if ($('#flagAll').val() == 'true') {
            //    var tempLeads = $('#Coincidences').val().split(',').join("','")
            //    leads = "'" + tempLeads;
            //    leads += "'";
            //}
            //else {
            //    leads += "'";
            //    SEARCH.oTable.$('tr').each(function (index, item) {
            //        if ($(this)[0].cells[0].childNodes[1].checked == true) {
            //            leads += $(this).attr('id') + "','";
            //        }
            //    });
            //    leads = leads.substr(0, leads.length - 2);
            //}
            //$('#MassUpdate_Coincidences').val(leads);
            $('.mass-update-coincidences').val(leads);
            if (leads != '') {
                $('#frmMassUpdate').submit();
            }
            else {
                UI.messageBox(0, "No Leads Selected", null, null);
            }
        });

        $(".DateFormat").datetimepicker({
            dateFormat: 'yy-mm-dd',//01/01/0001
            timeFormat: 'hh:mm TT',// 12:00/am
            stepMinute: 5,
            changeMonth: true,
            changeYear: true
        }).val("");

        $(".DateSalesByTeam").datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true
        }).val("");
    }

    var addOption = function (data, field) {
        if (data.ResponseType == 1) {
            var exists = false;
            $.each($('#' + field + ' option'), function () {
                if ($(this).val() == data.LayoutID) {
                    exists = true;
                }
            });
            if (!exists) {
                $('#' + field).append('<option value="' + data.LayoutID + '">' + $('#Layout').val() + '</option>');

            }
            UI.messageBox(data.ResponseType, 'Report Layout Updated');
            $('#fdsLayoutInfo').slideUp('fast');
        } else {
            UI.messageBox(data.ResponseType, 'Report Layout NOT Updated');
        }
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

        $('.open-id').on('click', function () {
            var id = $(this).attr('data-id');
            if ($('#' + id).is(':visible')) {
                $('#' + id).slideUp();
            } else {
                $('#' + id).slideDown();
            }
        })

        $('.comments').hover(function () {
            $(this).find('.comment').show();
        }, function () {
            $(this).find('.comment').hide();
        });

        $('#txtNotes').on('keyup', function () {
            $('#divNotes').html($('#txtNotes').val());
        });

        $('.cb-btn').off('click').on('click', function () {
            if ($('.cb-detail').is(':visible')) {
                $('.cb-detail').hide();
            } else {
                $('.cb-detail').show();
            }
        });

        UI.applyFormat('currency');
        UI.exportToExcel($('h1').eq(0).html());
        UI.exportToCSV($('h1').eq(0).html());

        $('[data-display]').on('click', function () {
            var item = $(this).attr('data-display');
            if (item.indexOf('*') != -1) {
                if ($('[data-display-item*="' + item.split('-')[0] + '"]:visible').length > 0) {
                    $('[data-display-item*="' + item.split('-')[0] + '"]').hide();
                    $('[data-display*="' + item.split('-')[0] + '"]').html('+');
                }
                else {
                    $('[data-display-item*="' + item.split('-')[0] + '"]').show();
                    $('[data-display*="' + item.split('-')[0] + '"]').html('-');
                }
            }
            else {
                if ($('[data-display-item="' + item + '"]').is(':visible')) {
                    $('[data-display-item="' + item + '"]').hide();
                    $('[data-display="' + item + '"]').html('+');
                }
                else {
                    $('[data-display-item="' + item + '"]').show();
                    $('[data-display="' + item + '"]').html('-');
                }
            }
            UI.exportToExcel($('h1').eq(0).html());
        });
    }

    //function that format results table in reports
    var searchResultsTable = function (data) {
        //if ($(data).find('tbody tr').first().find('td').length > 0) {
        if ($(data).find('thead tr').first().find('th').length > 0) {
            var tableColumns = $(data).find('tbody tr').first().find('td').length - 1;
            REPORT.oIndicatorsTable = $(document).find('table').not('.no-plugin').first().dataTable({
                "sDom": 'Rlfrtip',
                "bFilter": false,
                "bProcessing": true,
                "asStripeClasses": ['odd', 'striped'],
                "bPaginate": false,
                "bAutoWidth": false,
                "aoColumnDefs": [{ 'aTargets': [tableColumns] }],
                "oLanguage": {
                    "oPaginate": {
                        "sPrevious": "",
                        "sNext": ""
                    }
                }
            });
            REPORT.makeTableRowsSelectable();
            REPORT.showDownloadLinks();
            //REPORT.totalRow();
        }
    }

    var searchDynamicResultsTable = function (data) {
        if ($(data).find('thead tr').first().find('th').length > 0) {
            var tableColumns = $(data).find('tbody tr').first().find('td').length - 1;
            REPORT.oIndicatorsTable = $(document).find('table').first().dataTable({
                "sDom": 'Rlfrtip',
                "bFilter": false,
                "bProcessing": true,
                "asStripeClasses": ['odd', 'striped'],
                "bPaginate": false,
                "bAutoWidth": false,
                "aoColumnDefs": [{ 'aTargets': [tableColumns] }],
                "oLanguage": {
                    "oPaginate": {
                        "sPrevious": "",
                        "sNext": ""
                    }
                }
            });
        }
    }

    //show details of each row clicked
    var makeTableRowsSelectable = function () {
        REPORT.reportTablesUnselectRow();
        var tableName = $(document).find('table:not(.non-selectable)').first().attr('id');
        function createSecondaryTable(json) {
            var jsonObj = $.parseJSON(json);
            if (jsonObj != null) {
                var builder = '<table id="" class="full-width">'
                    + '<thead><tr>'
                    + '<th>User</th>'
                    + '<th>Arrivals</th>'
                    + '<th>Arrivals With Options</th>'
                    + '<th>% Options Capture</th>'
                    + '<th>Total Paid</th>'
                    //+ '<th>User</th>'
                    + '<th>Total Nights</th>'
                    + '</tr></thead>'
                    + '<tbody>';
                if (jsonObj.length > 0) {
                    $.each(jsonObj, function (index, item) {
                        builder += '<tr>'
                            + '<td>' + item.Generic_Property1 + '</td>'
                            + '<td>' + item.Generic_Property2 + '</td>'
                            + '<td>' + item.Generic_Property3 + '</td>'
                            + '<td>' + item.Generic_Property4 + '</td>'
                            + '<td>' + item.Generic_Property5 + '</td>'
                            + '<td>' + item.Generic_Property6 + '</td>'
                            + '</tr>';
                    });
                }
                else {
                    builder += '<tr><td colspan="6" align="center">No data available in table</td></tr>';
                }
                builder += '</tbody></table>';
            } else {
                builder += '<table></table>';
            }

            return builder;
        }
        if ($('#' + tableName).find('tbody tr').children('td').children(':hidden').length > 0) {
            UI.tablesHoverEffect();
            $('#' + tableName).find('tbody tr').not('theader').unbind('click').bind('click', function (e) {
                if (!$(this).hasClass('selected-row')) {
                    $(document).find('.selected-row').each(function () {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                        var evt = $.Event('keyup');
                        evt.keyCode = 27;
                        $(document).trigger(evt);
                    });
                    $(this).parent().find('.selected-row').removeClass('selected-row primary');
                    $(this).addClass('selected-row primary');
                    if (REPORT.oIndicatorsTable.fnIsOpen($(this)[0])) {
                        REPORT.oIndicatorsTable.fnClose($(this)[0]);
                    }
                    else {
                        REPORT.oIndicatorsTable.fnOpen($(this)[0], createSecondaryTable($(this).children('td').children(':hidden').val()), 'temp-table');
                    }
                }
            });
        }
    }

    //var reportTablesUnselectRow = function (data) {
    var reportTablesUnselectRow = function () {
        $(document).unbind('keyup').bind('keyup', function (evt) {
            if (evt.keyCode === 27) {
                REPORT.oIndicatorsTable.fnClose($('.temp-table').parent('tr').prev()[0]);
                $('.temp-table').parent('tr').prev().removeClass('selected-row primary');
            }
        });
    }

    //function that format total row of indicators report
    var totalRow = function (data) {
        if ($('#' + data + ' tfoot').find('tr').length > 0) {
            $('#totalRow').children('td').first().removeClass('sorting_1').attr('style', 'font-weight: bold');
            $('#totalRow').removeClass('odd striped').addClass('total-row');
        }
    }

    //function that remove columns to display in dynamic report
    var bindRemoveHeadersFunction = function () {
        $('.remove-header').unbind('click').bind('click', function () {
            $(this).parents('.column-headers').first().remove();
        });
    }

    var callsForDynamicReport = function () {
        $.getJSON('/crm/Reports/GetDDLData', { itemType: 'searchFilters' }, function (data) {
            var arrayTable = new Array();
            var arrayField = new Array();
            var arrayName = new Array();
            var arrayType = new Array();
            var arrayProperty = new Array();

            $.each(data, function (index, item) {
                arrayTable[index] = item.TableName;
                arrayField[index] = item.FieldName;
                arrayName[index] = item.DisplayName;
                arrayType[index] = item.FieldType;
                arrayProperty[index] = item.PropertyName;
            });

            $('#SearchDynamic_SearchFilters').bind('keydown', function (e) {
                if (e.keyCode === $.ui.keyCode.TAB && $(this).data('ui-autocomplete').menu.active) {
                    e.preventDefault();
                }
            }).autocomplete({
                source: arrayName,
                minLength: 0,
                position: { my: 'right top', at: 'right bottom' },
                autoFocus: true,
                select: function (e, ui) {
                    var index = $.inArray(ui.item.value, arrayName);
                    var builder = '<div class="editor-alignment">'
                        + '<p class="editor-label remove-property"><label for="' + arrayProperty[index] + '">' + ui.item.value + '</label></p>'
                        + '<p class="editor-field">';
                    switch (arrayType[index]) {
                        case 'int': {
                            builder += '<select id="' + arrayProperty[index] + '" style="height:68px" multiple="multiple" name="' + arrayProperty[index] + '">'
                            var selectBuilder = '';
                            $.getJSON('/crm/Reports/GetDDLData', { itemType: arrayField[index] }, function (data) {
                                $.each(data, function (index2, item) {
                                    selectBuilder += '<option value="' + item.Value + '">' + item.Text + '</option>';
                                });
                            });
                            $(document).ajaxStop(function () {
                                $(this).unbind('ajaxStop');
                                builder += selectBuilder + '</select>';
                                builder += '</p></div>';
                                $('#divSearchFiltersContainer').prepend(builder);
                                REPORT.addRemoveIconToSearchFilters();
                            });
                            break;
                        }
                        case 'Guid': {
                            builder += '<select id="' + arrayProperty[index] + '" style="height:68px" multiple="multiple" name="' + arrayProperty[index] + '">'
                            var selectBuilder = '';
                            $.getJSON('/crm/Reports/GetDDLData', { itemType: arrayField[index] }, function (data) {
                                $.each(data, function (index2, item) {
                                    selectBuilder += '<option value="' + item.Value + '">' + item.Text + '</option>';
                                });
                            });
                            $(document).ajaxStop(function () {
                                $(this).unbind('ajaxStop');
                                builder += selectBuilder + '</select>';
                                builder += '</p></div>';
                                $('#divSearchFiltersContainer').prepend(builder);
                                REPORT.addRemoveIconToSearchFilters();
                            });
                            break;
                        }
                        case 'bit': {
                            builder += '<label for="' + arrayProperty[index] + 'True">Yes</label>'
                                + '<input type="radio" id="' + arrayProperty[index] + '" value="True" name="' + arrayProperty[index] + '" checked="checked">'
                                + '<label for="' + arrayProperty[index] + 'False">No</label>'
                                + '<input type="radio" id="' + arrayProperty[index] + '" value="False" name="' + arrayProperty[index] + '">'
                                + '</p></div>';
                            $('#divSearchFiltersContainer').prepend(builder);
                            break;
                        }
                        case 'date': {
                            var otherProperty = arrayProperty[index].toString();
                            otherProperty = otherProperty.replace('_I_', '_F_');
                            builder += '<input type="text" id="' + arrayProperty[index] + '" data-uses-date-picker="true" name="' + arrayProperty[index] + '" >'
                                + '<input type="text" class="right" id="' + otherProperty + '" data-uses-date-picker="true" name="' + otherProperty + '" >'
                                + '</p></div>';
                            $('#divSearchFiltersContainer').prepend(builder);
                            REPORT.bindDatePickerToFields();
                            break;
                        }
                        case 'time': {
                            //not considered yet
                            break;
                        }
                        case 'string': {
                            builder += '<input type="text" id="' + arrayProperty[index] + '" name="' + arrayProperty[index] + '">'
                                + '</p></div>';
                            $('#divSearchFiltersContainer').prepend(builder);
                            break;
                        }
                    }
                    REPORT.addRemoveIconToSearchFilters();
                },
                close: function () {
                    $(this).val('');
                },
                _resizeMenu: function () {
                    this.menu.element.outerWidth(50);
                }
            });
        });

        $.getJSON('/crm/Reports/GetDDLData', { itemType: 'tableFields' }, function (data) {
            var arrayText = new Array();
            var arrayValue = new Array();
            $.each(data, function (index, item) {
                arrayText[index] = item.Text;
                arrayValue[index] = item.Value;
            });

            $('#SearchDynamic_ColumnHeaders').bind('keydown', function (e) {
                if (e.keyCode === $.ui.keyCode.TAB && $(this).data('ui-autocomplete').menu.active) {
                    e.preventDefault();
                }
            }).autocomplete({
                source: arrayText,
                minLength: 0,
                position: { my: 'right top', at: 'right bottom' },
                autoFocus: true,
                select: function (e, ui) {
                    var index = $.inArray(ui.item.value, arrayText);
                    var builder = '<p id="' + arrayValue[index] + '" class="column-headers">' + ui.item.value + ' <img src="/Content/themes/base/images/cross.png" class="right remove-header" style="padding-top:2px"></p>';
                    if ($('#' + arrayValue[index]).length > 0)
                        UI.messageBox(-1, 'Column already added', null, null);
                    else
                        $('#divColumnHeadersContainer').append(builder);
                    REPORT.bindRemoveHeadersFunction();
                },
                close: function () {
                    $(this).val('');
                }
            });
        });
    }

    var showDownloadLinks = function () {
        //SanDiego-DepartmentAdministrator
        if (REPORT.oIndicatorsTable.find('tbody tr').length > 1 && UI.selectedWorkGroup == '8' && UI.selectedRole == '7262fa30-3efa-4868-ad08-58ae11bff603') {
            $('.download-file').show();
        }
    }

    var bindDatePickerToFields = function () {
        $('*[data-uses-date-picker="true"]').each(function () {
            $(this).datepicker({
                dateFormat: 'yy-mm-dd',
                changeMonth: true,
                changeYear: true,
                onClose: function (dateText, inst) {
                    if ($(this).attr('id').indexOf('_I_') > 0) {
                        if ($(this).next().val() != '') {
                            if (dateText != '') {
                                var fromDate = $(this).datepicker('getDate');
                                var toDate = $(this).next().datepicker('getDate');
                                if (fromDate > toDate) {
                                    $(this).next().datepicker('setDate', fromDate);
                                }
                            }
                            else {
                                $(this).next().val(dateText);
                            }
                        }
                        else {
                            $(this).next().val(dateText);
                        }
                    }
                    else {
                        if ($(this).prev().val() != '') {
                            var fromDate = $(this).prev().datepicker('getDate');
                            var toDate = $(this).datepicker('getDate');
                            if (fromDate > toDate) {
                                $(this).prev().datepicker('setDate', toDate);
                            }
                        }
                        else {
                            $(this).prev().val(dateText);
                        }
                    }
                },
                onSelect: function (selectedDate) {
                    if ($(this).attr('id').indexOf('_I_') > 0) {
                        if ($(this).next().val() != '') {
                            $(this).next().datepicker('setDate', $(this).next().datepicker('getDate'));
                        }
                        $(this).next().datepicker('option', 'minDate', $(this).datepicker('getDate'));
                    }
                    else {
                        if ($(this).prev().val() != '') {
                            $(this).prev().datepicker('setDate', $(this).prev().datepicker('getDate'));
                        }
                        //$(this).prev().datepicker('option', 'maxDate', $(this).datepicker('getDate'));
                    }
                }
            });
        });
    }

    var addRemoveIconToSearchFilters = function () {
        //$('.remove-property').hoverIntent({
        //    over: function () {
        //        $(this).append('<img src="/Content/themes/base/images/cross.png" class="property-removing right"/>');
        //        REPORT.bindRemoveFunctionToSearchFilters();
        //    },
        //    out: function () {
        //        $(this).find('.property-removing').remove();
        //    },
        //    timeout: 100
        //});
        $('.remove-property').unbind('mouseover').on('mouseover', function () {
            $(this).append('<img src="/Content/themes/base/images/cross.png" class="property-removing right"/>');
            REPORT.bindRemoveFunctionToSearchFilters();
        });
        $('.remove-property').unbind('mouseout').on('mouseout', function () {
            $(this).find('.property-removing').remove();
        }).delay(400, null);
    }

    var bindRemoveFunctionToSearchFilters = function () {
        $('.property-removing').unbind('click').on('click', function (e) {
            $(e.target).parents('.editor-alignment').first().remove();
        });
    }

    var dynamicResults = function (data) {
        if ($(data).length > 0) {
            $('#divDynamicResultsButtons').show();
        }
        else {
            $('#divDynamicResultsButtons').hide();
        }
    }

    var workgroupDependentListActions = function () {
        $('.workgroup-dependent-list').on('loaded', function () {
            $(this).each(function () {
                var id = $(this).attr('id');
                var route = $(this).attr('data-route');
                var parameter = $(this).attr('data-route-parameter');
                $.getJSON(route, { itemType: parameter, itemID: UI.selectedWorkGroup }, function (data) {
                    $('#' + id).fillSelect(data);
                });
            });
        });
    }

    var loadCharts = function (_class) {
        google.charts.load('current', { 'packages': ['corechart'] });
        $('.' + _class).each(function () {
            var id = $(this).data('info-id');
            var container = $(this).attr('id');

            google.charts.setOnLoadCallback(
                function () {
                    var data = new google.visualization.DataTable();
                    data.addColumn('string', 'Options');
                    data.addColumn('number', 'Quantity');

                    var _data = $('#' + id).val();
                    var src = [];
                    $.each($.parseJSON(_data), function (index, item) {
                        src.push([item.Key, parseInt(item.Value)]);
                    });

                    data.addRows(src);
                    var options = {
                        //title: 'Options Percentage'
                        slices: [{}],
                        chartArea: { left: 20, top: 0, width: '100%', height: '100%' },
                        fontSize: 18,
                        tooltip: { trigger: 'selection' }
                    };

                    var chart = new google.visualization.PieChart(document.getElementById(container));
                    chart.draw(data, options);
                });
        });
    }

    var CloseOut = function () {
        let init = function () {
            setTimeout(function () {
                $("#SearchCloseOut_Date").datepicker("option", "maxDate", 0);
            }, 500);
        }

        var submit = function () {
            var dataObject = {
                CloseOutDate: $('#SearchCloseOut_Date').val(),
                PointOfSaleID: $('#SearchCloseOut_PointOfSaleID').val(),
                SalesAgentUserID: $('#SearchCloseOut_SalesAgentID').val(),
                TerminalID: $('#SearchCloseOut_TerminalID').val(),
                JsonModel: $('#hdnCloseOutModel').val().replace(/&quot;/g, '"'),
                Notes: $('#txtNotes').val()
            };

            $.post('/crm/Reports/SaveCloseOut', dataObject, function (data) {
                if (data.ResponseType == 1) {
                    $('#btnPrintCloseOut').slideUp('fast');
                    window.print();
                    var closeoutid = data.CloseOutID;
                    $('#hdnCloseOutID').val(closeoutid);
                    $('#afterSave').slideDown('fast');
                } else {
                    UI.messageBox(0, data.ResponseMessage);
                }
            }, 'json');
        }

        var historyLoaded = function () {
            UI.tablesStripedEffect();
            UI.tablesHoverEffect();
            $('#tblCloseOutsHistory tbody tr').off('click').on('click', function (e) {
                if (!$(e.target).hasClass('delete-row')) {
                    $("#displayCloseOut").html('')
                    $("#displayCloseOut").load("/crm/Reports/GetSavedCloseOut/" + $(this).attr('id'), function () {
                        REPORT.addExtras();
                    });
                    $('#historyCloseOut').slideUp('fast');
                    $('#savedCloseOut').slideDown('fast');
                }
                else {
                    UI.confirmBox('Do you confirm you want to proceed?', deleteCloseOut, [$(e.target).parents('tr:first').attr('id')]);
                }
            });
            $('#savedCloseOut .back').off('click').on('click', function () {
                $('#historyCloseOut').slideDown('fast');
                $('#savedCloseOut').slideUp('fast');
            });
            REPORT.addExtras();
        }

        function deleteCloseOut(closeOutID) {
            $.ajax({
                url: '/crm/Reports/DeleteCloseOut',
                cache: false,
                type: 'POST',
                data: { closeOutID: closeOutID },
                success: function (data) {
                    var duration = data.ResponseType < 0 ? data.ResponseType : null;
                    if (data.ResponseType > 0) {
                        $('#frmSearchCloseOutsHistory').submit();
                        $('#divTblCloseOutResults').html('');
                    }
                    UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
                }
            });
        }

        var closeOutLoaded = function () {
            $('#btnDeleteCloseOut').off('click').on('click', function (e) {
                UI.confirmBox('Do you really want to delete this Close Out?', deleteCloseOut, [$('#hdnCloseOutID').val()]);
            });
        }

        return {
            init: init,
            submit: submit,
            historyLoaded: historyLoaded,
            closeOutLoaded: closeOutLoaded
        }
    }();

    var Audit = function () {
        var lastLength = 0;

        var save = function (id) {
            var dataObject = {
                id: id,
                invoice: $('#invoice_' + id).val()
            };
            $.post('/crm/Reports/SaveAudit', dataObject, function (data) {
                if (data.ResponseType == 1) {
                    $('#divaudit_' + id).slideUp('fast').html('Audited').slideDown('fast');
                }
                $('#ProviderInvoiceID').val(data.ProviderInvoiceID);
            }, 'json');
        }

        var loaded = function () {
            UI.tablesStripedEffect();
        }

        function validateDuplicatedFolios() {
            $.each($('#tblCouponsReport tbody tr').find('td:first'), function () {
                var folio = $(this).text();
                var counter = 0;
                $.each($('#tblCouponsReport tbody tr').find('td:first'), function () {
                    if (folio == $(this).text()) {
                        counter++;
                    }
                });
                if (counter > 1) {
                    $(this).addClass('custom-cost');
                }
            });
        }

        var showForm = function (id) {
            $('#divaudit_' + id).slideDown('fast');
            $('#btnaudit_' + id).slideUp('fast');
        }

        var couponSearchResponse = function (data) {
            if (data.Coupons.length > 0) {
                $('#spnTotalMXN').text('0.00');
                $('#spnTotalUSD').text('0.00');
                var msg = '';
                var tr = '';
                $.each(data.Coupons, function (c, coupon) {
                    if ((coupon.ProviderInvoiceID == null && coupon.Audited != true) || coupon.ProviderInvoiceID == $('#ProviderInvoiceID').val()) {
                        if ($('#tblCouponsReport tbody tr#' + coupon.ItemID).length == 0) {
                            tr += '<tr id="' + coupon.ItemID + '" data-purchaseid="' + coupon.PurchaseID + '">'
                                + '<td><a title="Open Coupon" href="http://' + coupon.WebsiteUrl + '/coupons/' + coupon.PurchaseID + '-' + coupon.ItemID + '" target="_blank">' + coupon.CouponNumber + (coupon.CouponReference != null && coupon.CouponReference != '' ? '<br />[' + coupon.CouponReference + ']' : '') + '</a></td>'
                                + '<td>' + coupon.Website + '</td>'
                                + '<td><a title="Open Purchase Details" href="/crm/masterchart#purchaseid=' + coupon.PurchaseID + '" target="_blank">' + coupon.GuestName + '</a></td>'
                                + '<td>' + coupon.PurchaseDate + '</td>'
                                + '<td>' + coupon.Provider + '<br />' + coupon.ActivityName + '</td>'
                                + '<td>' + coupon.ActivityDateAndSchedule + '</td>'
                                + '<td>' + coupon.Units.replace('.00', '') + '</td>'
                                + '<td>' + (coupon.ConfirmationNumber != null ? coupon.ConfirmationNumber : '') + '</td>'
                                + '<td>' + coupon.SalesAgent + '</td>'
                                + '<td>' + coupon.Status + '</td>'
                                + '<td>' + (coupon.CloseOut != null ? coupon.CloseOut : '') + '</td>';
                            if ((coupon.StatusID == 4 || coupon.StatusID == 5) && !coupon.CustomCost) {
                                tr += '<td class="cost-usd"><input type="hidden" class="coupon-cost-usd" value="0" /><span data-format="currency">0 ' + '</span></td><td class="cost-usd">' + coupon.Cost[0].Currency + '</td>';
                                tr += '<td class="cost-mxn"><input type="hidden" class="coupon-cost-mxn" value="0" /><span data-format="currency">0 ' + '</span></td><td class="cost-mxn">' + coupon.Cost[1].Currency + '</td>';
                            } else {
                                tr += '<td class="cost-usd ' + (coupon.CustomCost ? 'custom-cost' : '') + '"><input type="hidden" class="coupon-cost-usd" value="' + coupon.Cost[0].Amount + '" />' + (coupon.Cost[0].Amount >= 0 ? '<span data-format="currency">' + coupon.Cost[0].Amount + '</span>' : 'Undefined') + '</td><td class="cost-usd ' + (coupon.CustomCost ? 'custom-cost' : '') + '">' + coupon.Cost[0].Currency + '</td>';
                                tr += '<td class="cost-mxn ' + (coupon.CustomCost ? 'custom-cost' : '') + '"><input type="hidden" class="coupon-cost-mxn" value="' + coupon.Cost[1].Amount + '" />' + (coupon.Cost[1].Amount >= 0 ? '<span data-format="currency">' + coupon.Cost[1].Amount + '</span>' : 'Undefined') + '</td><td class="cost-mxn ' + (coupon.CustomCost ? 'custom-cost' : '') + '">' + coupon.Cost[1].Currency + '</td>';
                            }
                            tr += '<td><span data-format="currency">' + coupon.CouponTotal + '</span></td><td>' + coupon.CouponCurrencyCode + '</td>';
                            tr += '<td>' + (coupon.Invitation !== null ? coupon.Invitation : "") + '</td>';
                            tr += '<td>';                            
                            if (coupon.Audited) {
                                if (coupon.AbleToUnaudit) {
                                    tr += '<input type="checkbox" class="chk-audit" id="chk-' + coupon.ItemID + '" checked="checked" /><span class="block">' + coupon.Audit + '</span>';
                                } else {
                                    tr += '<input type="checkbox" class="chk-audit" id="chk-' + coupon.ItemID + '" checked="checked" style="display:none;" /><span class="block">' + coupon.Audit + '</span>';
                                }
                            } else if (((coupon.StatusID == 4 || coupon.StatusID == 5) && coupon.CancelationCharge == 0) || coupon.StatusID == 6) {
                                tr += '<span class="block">Non Payable</span>';
                            } else {
                                tr += '<input type="checkbox" class="chk-audit" id="chk-' + coupon.ItemID + '" />';
                            }
                            tr += '</td>';
                            tr += '<td>';
                            //paid to provider
                            if (coupon.PaidToProvider) {
                                tr += '<input type="checkbox" class="chk-paid" id="chkpaid-' + coupon.ItemID + '" checked="checked" /><span class="block">' + coupon.PaidToProviderInfo + '</span>';
                            } else if (((coupon.StatusID == 4 || coupon.StatusID == 5) && coupon.CancelationCharge == 0) || coupon.StatusID == 6) {
                                tr += '';
                            } else {
                                tr += '<input type="checkbox" class="chk-paid" id="chkpaid-' + coupon.ItemID + '" />';
                            }
                            tr += '</td>';
                            tr += '<td>' + (!coupon.Audited ? '<span class="btn-close"></span>' : '') + '</td>'
                                + '</tr>';
                        }
                    } else {
                        if (msg != '') {
                            msg += '<br />';
                        }
                        if (coupon.Audited) {
                            msg += coupon.CouponNumber + ' is already audited: <br />' + coupon.Audit;
                        } else {
                            msg += coupon.CouponNumber + ' is not audited but is already in another invoice. Try searching the invoice by Coupon Folio.';
                        }
                    }
                });
                $('.search-coupon').before(tr);
                validateDuplicatedFolios();
                if (data.Coupons.length > 0) {
                    $("#ProviderID option:contains(" + data.Coupons[0].Provider + ")").attr('selected', 'selected');
                    $('#InvoiceCurrency').val(data.Coupons[0].InvoiceCurrency);
                    if (data.Coupons[0].InvoiceCurrency == "Pesos") {
                        $('.cost-usd').hide();
                        $('.cost-mxn').show();
                    } else if (data.Coupons[0].InvoiceCurrency == "Dollars") {
                        $('.cost-usd').show();
                        $('.cost-mxn').hide();
                    } else {
                        $('.cost-usd').show();
                        $('.cost-mxn').show();
                    }
                }
                $('#tblCouponsReport tbody tr input[type="checkbox"]').off('click').on('click', function () {
                    setCouponIDs();
                });
                $('#tblCouponsReport tbody tr .btn-close').off('click').on('click', function () {
                    $('#' + $(this).parent().parent().attr('id')).remove();
                    setCouponIDs();
                });
                //total
                var totalmxn = 0;
                var totalusd = 0;
                $('.coupon-cost-mxn').each(function () {
                    totalmxn += parseFloat($(this).val());
                });
                $('.coupon-cost-usd').each(function () {
                    totalusd += parseFloat($(this).val());
                });

                $('#spnTotalMXN').text((totalmxn).toFixed(2));
                $('#spnTotalUSD').text((totalusd).toFixed(2));
                UI.applyFormat('currency');

                UI.addExtras();
                if (msg != '') {
                    UI.messageBox(0, msg);
                }
                //focus
                $('#Search_Folio').val('');
                //guardar ids de cupones
                setCouponIDs();
            } else {
                UI.messageBox(0, "Coupon(s) not found on this terminal.");
            }
            setTimeout(function () {
                $('#Search_Folio').focus();
            }, 1);

        }

        function setCouponIDs() {
            var ids = '';
            var sids = '';
            var pids = '';
            $('#tblCouponsReport tbody tr').not('.search-coupon').each(function () {
                if (ids != '') {
                    ids += ',';
                }
                ids += $(this).attr('id');
                if ($('#chk-' + $(this).attr('id')).is(':checked')) {
                    if (sids != '') {
                        sids += ',';
                    }
                    sids += $(this).attr('id');
                }
                if ($('#chkpaid-' + $(this).attr('id')).is(':checked')) {
                    if (pids != '') {
                        pids += ',';
                    }
                    pids += $(this).attr('id');
                }
            });
            if (ids == '') {
                $('#spnTotal').text("0 MXN");
                UI.addExtras();
            }
            else {  //mike
                //total
                var totalmxn = 0;
                var totalusd = 0;
                $('.coupon-cost-mxn').each(function () {
                    totalmxn += parseFloat($(this).val());
                });
                $('.coupon-cost-usd').each(function () {
                    totalusd += parseFloat($(this).val());
                });

                $('#spnTotalMXN').text((totalmxn).toFixed(2));
                $('#spnTotalUSD').text((totalusd).toFixed(2));
                UI.applyFormat('currency');
            }
            $('#PurchaseServicesIDs').val(ids);
            $('#PurchaseSelectedServicesIDs').val(sids);
            $('#PaidPurchaseServicesIDs').val(pids);

        }

        var init = function () {
            $('#btnClearCoupons').on('click', function () {
                REPORT.Audit.clearForm();
                $('#btnGetDetails').hide();
                //$('#spnCurrentExchangeRate').hide();
                $('#divProviderInvoice').slideDown('fast');
                $('#divAuditDetails').slideUp('fast');
                $('#InvoiceCurrency').val('');
                $('#spnTotalUSD').text('0');
                $('#spnTotalMXN').text('0');
                $('.cost-usd').show();
                $('.cost-mxn').show();
                UI.applyFormat('currency');
            });
            $('#chkAuditAll').on('click', function () {
                if ($(this).is(':checked')) {
                    $('.chk-audit').prop('checked', true);
                } else {
                    $('.chk-audit').prop('checked', false);
                }
                setCouponIDs();
            });

            $('#chkPaidAll').on('click', function () {
                if ($(this).is(':checked')) {
                    $('.chk-paid').prop('checked', true);
                } else {
                    $('.chk-paid').prop('checked', false);
                }
                setCouponIDs();
            });

            $('#Search_Folio').on('keypress keydown keyup', function (e) {
                //console.log('last length: ' + Audit.lastLength);
                //console.log('folio length: ' + $('#Search_Folio').val().length);
                //console.log('dif: ' + (parseInt($('#Search_Folio').val().length) - parseInt(Audit.lastLength)));
                if ((parseInt($('#Search_Folio').val().length) - parseInt(Audit.lastLength)) >= 6) {
                    if (e.keyCode == '13') {
                        e.preventDefault();
                        return false;
                    }
                    if (e.keyCode != '8') {
                        $('#btnAddCoupon').trigger('click');
                    }
                }
                Audit.lastLength = $('#Search_Folio').val().length;
            }).focus();

            window.addEventListener("beforeunload", function (e) {
                if ($('#ProviderInvoiceID').val() == "0" && $('#tblCouponsReport tbody tr').length > 1) {


                    var confirmationMessage = 'It looks like you have an unsaved invoice. '
                        + 'If you leave before saving, your coupons list will be lost.';

                    (e || window.event).returnValue = confirmationMessage; //Gecko + IE
                    return confirmationMessage; //Gecko + Webkit, Safari, Chrome etc.
                } else {
                    return true;
                }
            });
        }

        var clearForm = function () {
            $('#tblCouponsReport tbody tr').not('.search-coupon').remove();
            $('#ProviderID').val('');
            $('#Invoice').val('');
            $('#Search_Folio').val('');
            $('#ProviderInvoiceID').val('0');
            $('#PurchaseServicesIDs').val('');
            $('#PurchaseSelectedServicesIDs').val('');
            $('#PaidPurchaseServicesIDs').val('');
            $('#spnTotal').text('0 MXN');
            $('#chkAuditAll').prop('checked', false);
        }

        function GetAuditDetails(providerInvoiceID) {
            $('#divAuditDetails').html('');
            $('#divAuditDetails').load('/crm/Reports/GetAuditDetails/' + providerInvoiceID, function () {
                REPORT.addExtras();
                $(this).slideDown('fast');
                $('#divProviderInvoice').slideUp('fast');
                $('#btnAuditDetailsPrint').off('click').on('click', function () {
                    $(window).printPage();
                });
                $('#btnCloseAuditDetails').off('click').on('click', function () {
                    $('#divProviderInvoice').slideDown('fast');
                    $('#divAuditDetails').slideUp('fast');
                });
                $('.edit-cost').off('click').on('click', function () {
                    $.fancybox.open([
                        {
                            type: 'ajax',
                            href: '/crm/Reports/GetCouponCost/' + $(this).attr('data-purchaseserviceid') + '?PurchaseServiceDetailID=' + $(this).attr('data-purchaseservicedetailid')
                        }
                    ], {
                            'width': 450,
                            'height': 600,
                            'autoSize': false,
                            'openEffect': 'fadeIn',
                            'closeEffect': 'fadeOut',
                            'openEasing': 'easeOutBack',
                            'closeEasing': 'easeInBack',
                            'title': null,
                            'closeBtn': true,
                            'afterShow': function () {
                                UI.addExtras();
                            },
                            'beforeShow': function () {
                                this.wrap.draggable();
                                $('.fancybox-overlay').css({
                                    'overflow': 'hidden',
                                    'overflow-y': 'hidden',
                                });
                            }
                        });
                });
            });
        }

        var invoiceSaved = function (data) {
            if (data.ResponseType == 1) {
                $('#ProviderInvoiceID').val(data.ProviderInvoiceID);
                UI.messageBox(1, "Invoice Successfully Saved!");
                $('#btnGetDetails').off('click').on('click', function () {
                    GetAuditDetails($('#ProviderInvoiceID').val());
                }).show();
                //$('#spnCurrentExchangeRate').show();
            } else {
                UI.messageBox(1, data.ResponseMessage);
            }
        }

        var invoiceResults = function () {
            UI.tablesStripedEffect();
            UI.tablesHoverEffect();
            REPORT.Audit.clearForm();
            $('#btnClearCoupons').trigger('click');
            $('#tblInvoices tbody tr').off('click').on('click', function () {
                $(this).parent().find('.selected-row').removeClass('selected-row primary');
                $(this).addClass('selected-row primary');
                REPORT.Audit.clearForm();
                $.post('/crm/Reports/GetProviderInvoice/' + $(this).attr('id'), null, function (data) {
                    $('#ProviderInvoiceID').val(data.ProviderInvoiceID);
                    $('#ProviderID').val(data.ProviderID);
                    $('#InvoiceCurrency').val(data.InvoiceCurrency);
                    $('#Invoice').val(data.Invoice);
                    $('#tblCouponsReport tbody tr').not('.search-coupon').remove();
                    REPORT.Audit.couponSearchResponse(data);
                    $('#chkAuditAll').prop('checked', false);
                    $('#btnGetDetails').off('click').on('click', function () {
                        GetAuditDetails($('#ProviderInvoiceID').val());
                    }).show();
                    //$('#spnCurrentExchangeRate').show();
                    $('#divAuditDetails').slideUp('fast');
                    $('#divProviderInvoice').slideDown('fast');
                }, 'json');
            });
        }

        return {
            init: init,
            save: save,
            showForm: showForm,
            loaded: loaded,
            couponSearchResponse: couponSearchResponse,
            clearForm: clearForm,
            invoiceSaved: invoiceSaved,
            invoiceResults: invoiceResults,
            lastLength: lastLength
        }
    }();

    var Prices = function () {
        var pricesLoaded = function () {
            $('.table .price').hover(function () {
                $(this).siblings().fadeIn('fast');
            }, function () {
                $(this).siblings().fadeOut('fast');
            });
            REPORT.addExtras();
        }

        return {
            pricesLoaded: pricesLoaded
        }
    }();

    var QuoteRequests = function () {
        var lastOffset = 0;
        var loaded = function () {
            REPORT.addExtras();
            REPORT.searchResultsTable($(document).find('table').first());
            UI.tablesHoverEffect();
            UI.tablesStripedEffect();
            $(document).find('table tr').on('click', function () {
                $(this).siblings().removeClass('selected-row');
                $(this).addClass('selected-row');
                var data = eval('(' + $('#Json').val() + ')');
                var id = $(this).attr('id');
                $.each(data, function (i, item) {
                    if (item.TransactionID == id) {
                        $('#DateSaved').val(item.DateSaved);
                        $('#FirstName').val(item.FirstName);
                        $('#LastName').val(item.LastName);
                        $('#Email').val(item.Email);
                        $('#Phone').val(item.Phone);
                        $('#Destination').val(item.Destination);
                        $('#Resort').val(item.Resort);
                        $('#Arrival').val(item.Arrival);
                        $('#Departure').val(item.Departure);
                        $('#Adults').val(item.Adults);
                        $('#Children').val(item.Children);
                        $('#TimeToBeReached').val(item.TimeToBeReached);
                        $('#Comments').val(item.Comments);
                    }
                });
                $('#divQuoteRequestInfo').slideDown('fast');
                lastOffset = $("body").scrollTop();
                var targetOffset = $("#divQuoteRequestInfo").offset().top;
                $('html,body').animate({ scrollTop: targetOffset }, 500);
                $('#fdsQuoteRequestInfo legend').off('click').on('click', function () {
                    $('html,body').animate({ scrollTop: lastOffset }, 500);
                });
            });
        }
        return {
            loaded: loaded
        }
    }();

    var Sweepstakes = function () {
        var lastOffset = 0;
        var loaded = function () {
            REPORT.addExtras();
            REPORT.searchResultsTable($(document).find('table').first());
            UI.tablesHoverEffect();
            UI.tablesStripedEffect();
            $(document).find('table tr').on('click', function () {
                $(this).siblings().removeClass('selected-row');
                $(this).addClass('selected-row');
                var data = eval('(' + $('#Json').val() + ')');
                var id = $(this).attr('id');
                $.each(data, function (i, item) {
                    if (item.TransactionID == id) {
                        $('#Date').val(item.Date);
                        $('#FirstName').val(item.FirstName);
                        $('#LastName').val(item.LastName);
                        $('#Email').val(item.Email);
                        $('#Phone').val(item.Phone);
                    }
                });
                $('#divSweepstakesInfo').slideDown('fast');
                lastOffset = $("body").scrollTop();
                var targetOffset = $("#divSweepstakesInfo").offset().top;
                $('html,body').animate({ scrollTop: targetOffset }, 500);
                $('#fdsSweepstakeInfo legend').off('click').on('click', function () {
                    $('html,body').animate({ scrollTop: lastOffset }, 500);
                });
            });
        }
        return {
            loaded: loaded
        }
    }();

    var CouponsHistory = function () {
        var init = function () {
            $('.mark-all').off('click').on('click', function () {
                if ($('.mark-all').is(':checked') == false) {
                    $('.mark').prop('checked', false);
                } else {
                    $('.mark').prop('checked', true);
                }
            });

            $('#btnNoShow').off('click').on('click', function () {
                var psids = '';
                $.each($('.mark:checked'), function () {
                    if (psids != '') {
                        psids += ',';
                    }
                    psids += $(this).closest('tr').attr('id');
                });
                //enviar ids a servidor para guardar el status
                var dataObject = {
                    id: psids
                }
                $.post('/crm/Reports/SaveAsNoShow', dataObject, function (data) {
                    if (data.ResponseType == 1) {
                        //cambiar status en tabla de vista
                        $.each($('.mark:checked'), function () {
                            $('#' + $(this).closest('tr').attr('id') + ' .status').text('No Show');
                        });
                    } else {
                        UI.messageBox(0, data.ResponseMessage);
                    }
                }, 'json');
            });

            $('#btnConfirmed').off('click').on('click', function () {
                var psids = '';
                $.each($('.mark:checked'), function () {
                    if (psids != '') {
                        psids += ',';
                    }
                    psids += $(this).closest('tr').attr('id');
                });
                //enviar ids a servidor para guardar el status
                var dataObject = {
                    id: psids
                }
                $.post('/crm/Reports/SaveAsConfirmed', dataObject, function (data) {
                    if (data.ResponseType == 1) {
                        //cambiar status en tabla de vista
                        $.each($('.mark:checked'), function () {
                            $('#' + $(this).closest('tr').attr('id') + ' .status').text('Confirmed');
                        });
                    } else {
                        UI.messageBox(0, data.ResponseMessage);
                    }
                }, 'json');
            });

            $('#Search_FromCoupon').off('keyup').on('keyup', function () {
                if ($('#Search_FromCoupon').val() != "") {
                    $('.date').hide();
                } else {
                    $('.date').show();
                }
            });
        }

        return {
            init: init
        }
    }();

    var addPluginToTable = function (data) {
        var tableName = $(data).attr('id');
        $('#' + tableName).dataTable({
            //"bFilter": false,
            "bProcessing": true,
            //"asStripeClasses": ['odd', 'striped'],
            "bAutoWidth": false,
            //'aoRowCreatedCallback': [makeTableRowsSelectable()],
            //"aoColumnDefs": [{ 'aTargets': [index] }],
            "aoRowCallback": [UI.tablesHoverEffect()],
            "bPaginate": false
        });
    }

    var allowColumnFilterOnTblDuplicateRows = function () {
        REPORT.oDuplicateTable = $('#tblDuplicateLeadsResults').dataTable({
            "bProcessing": true,
            "bAutoWidth": false,
            //"aoRowCallback": [UI.tablesHoverEffect()],
            "bPaginate": false
        });
        var $container = $('#divTblDuplicateLeadsResults .dataTables_filter');
        $container.hide();
        var $searchTextBox = $container.find('input:text');
        //REPORT.oDuplicateTable.$('tr').not('theader').unbind('click').on('click', function (e) {
        REPORT.oDuplicateTable.$('td').not('td:nth-child(1)').unbind('click').on('click', function (e) {
            $('#tblDuplicateLeadsResults thead tr:first').find('th.mb-confirmation').removeClass('mb-confirmation');
            $searchTextBox.val($(e.target).text().trim());
            $searchTextBox.trigger('keyup');
            $('#tblDuplicateLeadsResults thead tr:first').find('th:nth-child(' + ($(e.target).index() + 1) + ')').addClass('mb-confirmation');
        });

        $('#btnClearColumnFilter').unbind('click').on('click', function () {
            $searchTextBox.val('');
            $searchTextBox.trigger('keyup');
            $('#tblDuplicateLeadsResults thead tr:first').find('th.mb-confirmation').removeClass('mb-confirmation');
        });

        $('#chkSelectAllDups').on('click', function () {
            if ($(this).is(':checked')) {
                $('#tblDuplicateLeadsResults tbody').find('.chk-son').each(function () {
                    $(this).attr('checked', 'checked');
                });
            }
            else {
                $('#tblDuplicateLeadsResults tbody').find('.chk-son').each(function () {
                    $(this).removeAttr('checked');
                });
            }
        });

        $('.chk-son').unbind('click').on('click', function () {
            if ($(this).is(':checked')) {
                if ($('#tblDuplicateLeadsResults tbody').find('.chk-son:not(:checked)').length == 0) {
                    $('#chkSelectAllDups').attr('checked', 'checked');
                }
            }
            else {
                $('#chkSelectAllDups').removeAttr('checked');
            }
        });
    }

    var massUpdateSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {

        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var Budgets = function () {
        var budgetLoaded = function () {
            UI.tablesStripedEffect();
            $('#tblBudgets tbody tr').off('click').on('click', function () {
                var id = $(this).attr('id');
                $('.budget-detail').slideUp('fast');
                $('div[data-id="' + id + '"]').slideDown('fast');

                $('#tblBudgets tbody tr').removeClass('selected-row primary');
                $(this).addClass('selected-row primary');
            });
            REPORT.addExtras();
        }
        return {
            budgetLoaded: budgetLoaded,
        }
    }();

    var MasterCloseOut = function () {
        var errors = 0;
        var init = function () {
            $('#btnProcessCoupons').off('click').on('click', function () {
                MasterCloseOut.errors = 0;
                $('#btnProcessCoupons').hide();
                var totalCoupons = $('.coupon.pending').length;
                if ($('.coupon.pending').length > 0) {
                    //var couponIDS = '';
                    $('.coupon.pending').slowEach(1000, function () {
                        //procesar
                        var id = $(this).attr('data-id');

                        $('td[data-id="' + id + '"]').removeClass('pending').addClass('processing');
                        $.ajax({
                            url: '/crm/Reports/ProcessCouponInfo/' + id,
                            dataType: 'json',
                            //async: false,
                            success: function (data) {
                                if (data.Processed) {
                                    $('td[data-id="' + id + '"]').removeClass('processing').addClass('cached');
                                } else {
                                    $('td[data-id="' + id + '"]').removeClass('processing').addClass('error');
                                }
                                $('.cached-label').text(parseInt($('.cached-label').text()) + 1);
                                if ($('td[data-id="' + id + '"]').text().indexOf("OUT OF") >= 0 || $('td[data-id="' + id + '"]').text().indexOf("ERROR") >= 0) {
                                    $('.out-of-date-label').text(parseInt($('.out-of-date-label').text()) - 1);
                                } else if ($('td[data-id="' + id + '"]').text().indexOf("Not Cached") >= 0) {
                                    $('.not-cached-label').text(parseInt($('.not-cached-label').text()) - 1);
                                }
                            }
                        }).fail(function () {
                            $('td[data-id="' + id + '"]').removeClass('processing').addClass('error');
                            MasterCloseOut.errors += 1;
                            $('#spnErrors').text(MasterCloseOut.errors + " coupons with errors. Please check them and process again.")
                        }).always(function () {
                            var pending = parseInt($('.coupon.pending').length) + parseInt($('.coupon.processing').length);
                            var processed = parseInt(totalCoupons) - parseInt(pending);
                            var percentage = processed * 100 / totalCoupons;
                            $('#spnProgress').text(processed + "/" + totalCoupons + " : " + percentage.toFixed(2) + "%");
                        });
                    });
                }
            });

            $('#spnProgress').text('0/' + $('.coupon.pending').length);
        }

        return {
            init: init,
            errors: errors
        }
    }();

    var ChargeBacks = function () {
        var init = function () {
        }

        var loaded = function () {
            //$('.charged').off('click').on('click', function () {
            //    var id = $(this).attr('data-id');
            //    $(this).val('Saving...');
            //    $.post('/crm/reports/SaveCharged/' + id, null, function (data) {
            //        if (data.ResponseType == 1) {
            //            $('input[data-id="' + id + '"]').after('<span>' + data.ResponseMessage + '</span>');
            //            $('input[data-id="' + id + '"]').remove();
            //        } else {
            //            UI.messageBox(data.ResponseType, data.ResponseMessage);
            //            $('input[data-id="' + id + '"]').val('Charged');
            //        }
            //    }, 'json');
            //});
            $('.cxc-amount').off('click').on('click', function () {
                let chargeid = $(this).attr('data-chargeid');
                $('#partials-' + chargeid).slideToggle('fast');
            });
            $('input[data-field="amount"]').off('keyup').on('keyup', calculateTransaction);
            $('select[data-field="currencyid"]').off('change').on('change', calculateTransaction);
            setEventDeletePartial();
            $('.save-partial').off('click').on('click', function () {
                //console.log('-------------CLICK--------------');

                let ChargeID = $(this).attr('data-chargeid');
                //console.log('ChargeID: ' + ChargeID);

                let Description = $('input[data-field="description"][data-chargeid="' + ChargeID + '"]').val();
                let Amount = $('input[data-field="amount"][data-chargeid="' + ChargeID + '"]').val();
                let CurrencyID = $('select[data-field="currencyid"][data-chargeid="' + ChargeID + '"]').val();
                let AmountInPaymentCurrency = $('span[data-field="transaction"][data-chargeid="' + ChargeID + '"]').text();

                //console.log('AmountInPaymentCurrency: ' + AmountInPaymentCurrency);
                //console.log('CurrencyID: ' + CurrencyID);

                let payment = $('span[data-field="payment"][data-chargeid="' + ChargeID + '"]').text();
                let partials = $('span[data-field="partials"][data-chargeid="' + ChargeID + '"]').text();
                let balance = $('span[data-field="balance"][data-chargeid="' + ChargeID + '"]').text();
                let currencyCode = $('span[data-field="payment"][data-chargeid="' + ChargeID + '"]').attr('data-currency');

                //console.log('payment: ' + payment);
                //console.log('partials: ' + partials);
                //console.log('balance: ' + balance);
                //console.log('currencyCode: ' + currencyCode);

                partials = parseFloat(partials) + parseFloat(AmountInPaymentCurrency);
                balance = parseFloat(payment) + parseFloat(partials);
                //console.log('partials: ' + partials);

                if ((payment < 0 && balance <= 0) || (payment > 0 && balance >= 0)) {
                    $.ajax({
                        url: '/crm/reports/SavePartial',
                        cache: false,
                        type: 'POST',
                        data: {
                            ChargeID: ChargeID,
                            Description: Description,
                            Amount: Amount,
                            CurrencyID: CurrencyID,
                            AmountInPaymentCurrency: AmountInPaymentCurrency,
                            Balance: balance
                        },
                        success: function (data) {
                            if (data.ResponseType == 1) {
                                $('span[data-field="partials"][data-chargeid="' + ChargeID + '"]').text(parseFloat(partials).toFixed(2));
                                $('span[data-field="balance"][data-chargeid="' + ChargeID + '"]').text(parseFloat(balance).toFixed(2));
                                $('span[data-field="balance-payment"][data-chargeid="' + ChargeID + '"]').text((parseFloat(balance) * -1).toFixed(2));

                                let tr = '';
                                tr += '<tr data-partialid="' + data.ObjectID + '">';
                                tr += '<td>' + Description + '</td>';
                                tr += '<td>' + $('span[data-field="date"][data-chargeid="' + ChargeID + '"]').text() + '</td>';
                                tr += '<td>Me</td>';
                                tr += '<td class="text-right">$' + parseFloat(Amount).toFixed(2) + (CurrencyID == "1" ? " USD" : " MXN") + '</td>';
                                tr += '<td class="text-right">$<span data-field="transaction-amount">' + AmountInPaymentCurrency + ' ' + currencyCode + '</span></td>';
                                tr += '<td class="text-right">$' + parseFloat(balance).toFixed(2) + ' ' + currencyCode + '</td>';
                                tr += '<td class="text-center"><i class="material-icons delete-partial" data-partialid="' + data.ObjectID + '" data-chargeid="' + ChargeID + '">delete</i></td>';
                                tr += '</tr>';

                                $('table[data-field="partials-table"][data-chargeid="' + ChargeID + '"] tbody').append(tr);

                                setEventDeletePartial();

                                //limpiar formulario
                                $('input[data-field="description"][data-chargeid="' + ChargeID + '"]').val('');
                                $('input[data-field="amount"][data-chargeid="' + ChargeID + '"]').val('');
                            } else {
                                UI.messageBox(0, 'Balance exceeds the payment amount by ' + parseFloat(balance).toFixed(2), null, null);
                            }
                        }
                    });
                } else {
                    UI.messageBox(0, 'Balance exceeds the payment amount by ' + parseFloat(balance).toFixed(2), null, null);
                }
            });

            UI.applyTextFormat();
        }

        function setEventDeletePartial() {
            $('.delete-partial').off('click').on('click', function () {
                let partialid = $(this).attr('data-partialid');
                let chargeid = $(this).attr('data-chargeid');

                //console.log('chargeid: ' + chargeid);

                //solicitud de eliminación
                $.ajax({
                    url: '/crm/reports/DeletePartial/' + partialid,
                    cache: false,
                    type: 'POST',
                    success: function (data) {
                        if (data.ResponseType == 1) {
                            let payment = parseFloat($('span[data-field="payment"][data-chargeid="' + chargeid + '"]').text());
                            let partials = parseFloat($('span[data-field="partials"][data-chargeid="' + chargeid + '"]').text());
                            let balance = parseFloat($('span[data-field="balance"][data-chargeid="' + chargeid + '"]').text());

                            let deletedTransaction = parseFloat($('tr[data-partialid="' + partialid + '"] span[data-field="transaction-amount"]').text());

                            console.log('deletedTransaction: ' + deletedTransaction);

                            partials = partials - deletedTransaction;
                            balance = payment + partials;

                            console.log('partials: ' + partials);
                            console.log('balance: ' + balance);

                            $('span[data-field="partials"][data-chargeid="' + chargeid + '"]').text(partials.toFixed(2));
                            $('span[data-field="balance"][data-chargeid="' + chargeid + '"]').text(balance.toFixed(2));
                            $('span[data-field="balance-payment"][data-chargeid="' + chargeid + '"]').text((balance * -1).toFixed(2));

                            $('tr[data-partialid="' + partialid + '"]').addClass('partial-deleted');
                            $('.delete-partial[data-partialid="' + partialid + '"]').remove();
                        }
                    }
                });
            });
        }

        function calculateTransaction() {
            let chargeid = $(this).attr('data-chargeid');
            let amount = $('input[data-field="amount"][data-chargeid="' + chargeid + '"]').val();
            amount = (amount != "" ? amount : 0);
            let eramount = amount;
            let er = $('#partials-' + chargeid + ' .exchange-rate').text();
            let paymentCurrencyID = $('#partials-' + chargeid + ' .exchange-rate').attr('data-paymentcurrencyid');
            let partialCurrencyID = $('select[data-field="currencyid"][data-chargeid="' + chargeid + '"]').val();
            if (partialCurrencyID == "1" && paymentCurrencyID == "2") {
                eramount = amount * er;
            } else if (partialCurrencyID == "2" && paymentCurrencyID == "1") {
                eramount = amount / er;
            }
            $('span[data-field="transaction"][data-chargeid="' + chargeid + '"]').text(parseFloat(eramount).toFixed(2));
            let balance = $('span[data-field="balance"][data-chargeid="' + chargeid + '"]').text();
            $('span[data-field="transaction-balance"][data-chargeid="' + chargeid + '"]').text((parseFloat(balance) + parseFloat(eramount)).toFixed(2));
        }

        return {
            init: init,
            loaded: loaded
        }
    }();

    var GetActivityLogs = function () {
        var interval
        var ActivityTable;
        var oSettings;
        var cont = 0;

        var ButtonActions = ($('#btnCurrentActivity').off('click').on('click', function () {
            var stop = $(this).hasClass('stopRecording');
            if (stop == true) {
                clearInterval(interval);
                $(this).removeClass('stopRecording').val("CURRENT ACTIVITY");
            } else {
                interval = setInterval(RepeatActivitylogs, 3000);
                $(this).addClass('stopRecording').val("STOP");
                //$('#tblActivityLogs tbody tr').remove();
            }
        }));
        function AddNewRow(Row) {
            var checked = Row.ContactInfo == true ? 'checked' : '';
            ActivityTable = $.fn.DataTable
                .fnIsDataTable('tblActivityLogs') ? $('#tblActivityLogs') : $('#tblActivityLogs').DataTable();
            oSettings = ActivityTable.fnSettings();
            var iAdded = ActivityTable.fnAddData([//agrega dar datos a la tabla
                Row.UserLogActivity,
                Row.DateSaved,
                Row.UserName,
                Row.Controller,
                Row.Method,
                '<div style="width:100%; max-height:150px; overflow:auto">' + Row.Description + '</div>',
                Row.UrlMethod,
                '<a target="_blank" href="' + Row.Url + '">' + Row.Url + '</a>',
                '<input type="checkbox"' + checked + ' disabled/>'
            ], Row.UserLogActivity);
            var aRow = oSettings.aoData[iAdded[0]].nTr;//Fila
            aRow.childNodes[0].setAttribute('style', 'display:none');
            //aRow.setAttribute('style="visibility:hidden"')
            aRow.setAttribute('id', Row.UserLogActivity);//agrega el id a la fila      
            ActivityTable.fnDraw();
            ActivityTable.fnSort([[0, 'desc']]);//ordena de modo descendente comparando el contenido de la primer columna

            ActivityTable.find($('#' + Row.UserLogActivity)).effect("highlight", 2000);//agregar efecto a la columna
            UI.tablesHoverEffect();
            UI.tablesStripedEffect();
        }
        function RepeatActivitylogs() {
            $.ajax(
                {
                    url: '/crm/Reports/GetActivityLogsByTime',
                    cache: false,
                    type: 'POST',
                    success: function (data) {//agregar los tr de la tabla a un array
                        var TableRows;
                        $.each(data, function (k, v) {
                            TableRows = $('#tblActivityLogs tbody tr').map(//agregar id's a arreglo
                                function () { return this.id }).get();
                        })

                        if (TableRows == undefined) {//si no hay nada en el tbody
                            if ($('#tblActivityLogs tbody tr > td:first').text() == "") //si la primer fila esta vacia 
                                $('#tblActivityLogs tbody').prepend("<tr><td align='center' colspan='8'> There is not activity yet </td><tr>");

                            clearInterval(interval);//pausa la llamada al metodo
                            $('#btnCurrentActivity').removeClass('stopRecording').val("CURRENT ACTIVITY");
                            UI.messageBox(0, "There is not activity, try it again more late", null, null);
                        }
                        else {
                            if (TableRows.length == 0) {//si contiene informacion
                                $('#tblActivityLogs tbody tr').remove();
                                $.each(data, function (k, v) {
                                    AddNewRow(v);//agregar los registros
                                });
                            }
                            else {
                                $.each(data, function (k, v) {
                                    var ActivityID = v.UserLogActivity.toString();
                                    if (TableRows.indexOf(ActivityID) < 0) {// sino existe el registro en la tabla
                                        AddNewRow(v);// agrega
                                        cont = 0;//resetear contador
                                    }
                                    cont++;
                                    if (cont > 15)//si se repite la funcion y no hay registros aún 
                                    {
                                        $('#btnCurrentActivity').addClass('stopRecording').click();//
                                        UI.messageBox(0, "There is not activity, try again more late", null, null);//
                                    }
                                });
                            }
                        }
                    }
                });
        }
    }();

    var AgentsPerformance = function () {
        var init = function () {

        }

        var loaded = function () {
            $("#tabs").tabs();
            UI.exportToExcel($('h1').eq(0).html());
            $('.sortable').dataTable({
                "sDom": 'Rlfrtip',
                "bProcessing": true,
                "asStripeClasses": ['odd', 'striped'],
                "bPaginate": false,
                "bAutoWidth": false,
                "oLanguage": {
                    "oPaginate": {
                        "sPrevious": "",
                        "sNext": ""
                    }
                }
            });
        }

        return {
            init: init,
            loaded: loaded
        }
    }();

    var CallsByLocation = function () {
        var init = function () {
            $('#bannerUploader').fineUploader({
                request: {
                    endpoint: '/CallsLog/UploadCallsCSV',
                    params: {}
                },
                multiple: false,
                failedUploadTextDisplay: {
                    mode: 'default'
                }
            });
            $('.qq-upload-button div').text("Import CSV");
            $('.qq-upload-button').css({
                'width': '80px',
                'padding': '6px 10px 0 10px',
                'margin': '1px 2px 0 0'
            });
        }

        var loaded = function () {
            $("#tabs").tabs();
            UI.exportToExcel($('h1').eq(0).html());
            $('.sortable').dataTable({
                "sDom": 'Rlfrtip',
                "bProcessing": true,
                "asStripeClasses": ['odd', 'striped'],
                "bPaginate": false,
                "bAutoWidth": false,
                "oLanguage": {
                    "oPaginate": {
                        "sPrevious": "",
                        "sNext": ""
                    }
                }
            });
        }

        return {
            init: init,
            loaded: loaded
        }
    }();

    var highlightRows = function (table) {

        $('.selectable tbody tr').unbind('click').on('click', function () {
            //if ($(this).children('td').length == 8) {
            if ($(this).children('td').length > 0) {
                var confNumber = $(this).children('td').first().text();
                var indexes = $(this).parent('tbody').children('tr').find('td:contains(' + confNumber + ')');

                $(this).parent('tbody').find('tr.highlight-row').removeClass('highlight-row');
                $.each(indexes, function (index, item) {
                    $(this).parent('tr').addClass('highlight-row');
                });
            }
            else {
                $(this).parent('tbody').children('tr.highlight-row').removeClass('highlight-row');
            }
        });
    }

    var bindEmailPreview = function () {
        $('.preview').unbind('click').on('click', function (e) {
            e.preventDefault();
            var notification = $(this).attr('data-id');
            var transaction = $(this).attr('data-transaction');
            var reservation = $(this).attr('data-reservation');
            $.ajax({
                url: '/PreArrival/PreviewEmail',
                cache: false,
                data: { reservationID: reservation, emailNotificationID: notification, transactionID: transaction },
                success: function (data) {
                    if (data.Status == null || data.Status == '') {
                        UI.renderEmailPreview(data, [reservation, notification, transaction]);
                    }
                    else {
                        UI.messageBox(-1, data.Status, null, null);
                    }
                }
            });
        });
    }

    var clearTable = function (item) {
        if (!$.fn.DataTable.fnIsDataTable(document.getElementById(item))) {
            $('#' + item + ' tbody').empty();
        }
        else if (item == undefined) {
            $('body').find('table').each(function () {
                if ($(this).attr('id') != undefined) {
                    $('#' + $(this).attr('id')).dataTable().fnClearTable();
                }
            });
        }
        else {
            $('#' + item).dataTable().fnClearTable();
        }
    }

    return {
        init: init,
        addOption: addOption,
        addExtras: addExtras,
        totalRow: totalRow,
        dynamicResults: dynamicResults,
        showDownloadLinks: showDownloadLinks,
        searchResultsTable: searchResultsTable,
        callsForDynamicReport: callsForDynamicReport,
        bindDatePickerToFields: bindDatePickerToFields,
        makeTableRowsSelectable: makeTableRowsSelectable,
        reportTablesUnselectRow: reportTablesUnselectRow,
        searchDynamicResultsTable: searchDynamicResultsTable,
        bindRemoveHeadersFunction: bindRemoveHeadersFunction,
        addRemoveIconToSearchFilters: addRemoveIconToSearchFilters,
        workgroupDependentListActions: workgroupDependentListActions,
        bindRemoveFunctionToSearchFilters: bindRemoveFunctionToSearchFilters,
        CloseOut: CloseOut,
        Audit: Audit,
        Prices: Prices,
        QuoteRequests: QuoteRequests,
        Sweepstakes: Sweepstakes,
        CouponsHistory: CouponsHistory,
        MasterCloseOut: MasterCloseOut,
        ChargeBacks: ChargeBacks,
        addPluginToTable: addPluginToTable,
        allowColumnFilterOnTblDuplicateRows: allowColumnFilterOnTblDuplicateRows,
        massUpdateSuccess: massUpdateSuccess,
        Budgets: Budgets,
        GetActivityLogs: GetActivityLogs,
        Budgets: Budgets,
        CallsByLocation: CallsByLocation,
        AgentsPerformance: AgentsPerformance,
        loadCharts: loadCharts,
        highlightRows: highlightRows,
        bindEmailPreview: bindEmailPreview,
        clearTable: clearTable
    }
}();