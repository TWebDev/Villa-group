var Survey = function () {
    var Structure;
    var Stats;
    var Referrals;

    var init = function () {
        $('#I_Visibility').on('change', function () {
            if ($(this).val() != "0") {
                $('#divI_Question').show();
            } else {
                $('#divI_Question').hide();
            }
        });

        $('#btnInformativeField').on('click', function () {
            $('#paramsInfoField').slideToggle('fast');
        });

        $('#btnFormField').on('click', function () {
            $('#paramsFormField').slideToggle('fast');
        });

        $('#btnSaveInfoField').on('click', function () {
            Survey.addInfoField();
        });

        $('#btnClearFormField').on('click', function () {
            Survey.clearFormField();
        });

        $('#btnSaveFormField').on('click', function () {
            Survey.addFormField();
        });

        $('#btnNewSurveyInfo').on('click', function () {
            clearSurvey();
        });

        $('#F_Visibility').on('change', function () {
            if ($('#F_Visibility').val() == '2') {
                $('#divVisibleIf').show();
            } else {
                $('#divVisibleIf').hide();
            }
        });
        $('#F_Visibility').val(1);

        $('#F_FieldSubTypeID').on('change', function () {
            if ($('#F_FieldSubTypeID').val() == "5") {
                $('#divF_Options').show();
            } else {
                $('#divF_Options').hide();
            }
        });

        $('#SurveyName').on('keyup', function () {
            Survey.loadSurveyObject();
        })

        $('#Instructions').on('keyup', function () {
            Survey.loadSurveyObject();
        })

        $('#Stats_I_ReferralsFromDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onSelect: function (selectedDate) {
                if ($('#Stats_F_ReferralsToDate').val() != '') {
                    $('#Stats_F_ReferralsToDate').datepicker('setDate', $('#Stats_F_ReferralsToDate').datepicker('getDate'));
                }
                $('#Stats_F_ReferralsToDate').datepicker('option', 'minDate', $('#Stats_I_ReferralsFromDate').datepicker('getDate'));
            }
        });

        $('#Stats_F_ReferralsToDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onSelect: function () {
                Survey.getStatsSearchParams();
            }
        });

        $('#Stats_I_FromDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onSelect: function (selectedDate) {
                if ($('#Stats_F_ToDate').val() != '') {
                    $('#Stats_F_ToDate').datepicker('setDate', $('#Stats_F_ToDate').datepicker('getDate'));
                }
                $('#Stats_F_ToDate').datepicker('option', 'minDate', $('#Stats_I_FromDate').datepicker('getDate'));
            }
        });

        $('#Stats_F_ToDate').datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            onSelect: function () {
                Survey.getStatsSearchParams();
            }
        });

        $('#btnBackToTable').on('click', function () {
            $('#divSurveyDetailsSlider').animate({
                'marginLeft': '0'
            }, 500);
            $('#spnDetailsCounter').text($('#tblSurveyDetails table tbody tr').length);
        });

        $('#btnSendMail').on('click',function () {
            $('#spanSendStatus').html('');
            $('#spanSendStatus').text('');
            $('#divMailBody').slideDown('fast');
        });

        $('#btnCloseMail').on('click', function () {
            $('#divMailBody').slideUp('fast');
        });

        $('#btnSendMailContent').on('click', function () {
            //var html1 = '<div style="font-family:Verdana;font-size:12px;">';
            //html1 = html1.split('class="details-tag-alignment"').join('style="font-weight:bold"');
            //html1 = html1.split('class="details-content"').join('style="font-weight:normal"');
            //html1 = html1.split('</span>').join('</span><br />');
            var $detail = $('#spnSurveyContent').clone();
            $detail.find('.survey-content-field.subtype-2').each(function () {
                $(this).find('.survey-content-field-value').prepend(' ' + $(this).find('.rating-circle.full').length + '/' + $(this).find('.rating-circle').length + ' : ');
            //    var empty = $(this).find('.rating-circle.full').length;
            //    if (empty != 0 | fill != 0) {
            //        $(this).prev('span').append(fill + '/' + (fill + empty));
            //    }
            });

            var html = '<div style="font-family:Verdana;font-size:12px;">' + $detail.html();
            html = html.split('<span class="survey-content-field-value"').join('<br><span class="survey-content-field-value"');
            html = html.split('<span class="rating-circle full"></span>').join('<img src="http://eplat.villagroup.com/images/full_circle.png"/>');
            html = html.split('<span class="rating-circle"></span>').join('<img src="http://eplat.villagroup.com/images/empty_circle.png"/>');
            html = html.split('class="survey-content-field-name"').join('style="font-weight:bold"');
            html = html.split('class="survey-content-field-value"').join('style="font-weight:normal"');
            html = html.split('<span class="survey-content-field').join('<br><br><span class="survey-content-field');
            html = html + '</div>';
            var dataObj = {
                to: $('#txtMailTo').val(),
                cc: $('#txtMailCC').val(),
                subject: $('#txtMailSubject').val(),
                content: $('#txtMailContent').val(),
                survey: html
            }
            if ($('#txtMailTo').val() != "") {
                $('#spanSendStatus').html('<img src="/images/loading.gif" />');
                $.post('/settings/surveys/SendEmail/', dataObj, function (data) {
                    if (data.Success == true) {
                        $('#spanSendStatus').html('');
                        $('#spanSendStatus').css({ 'color': 'green' });
                        $('#spanSendStatus').text('Mail Succesfully Sent');
                        setTimeout(function () {
                            $('#btnCloseMail').click();
                        }, 3000);
                    }
                    else {
                        $('#spanSendStatus').html('');
                        $('#spanSendStatus').css({ 'color': 'red' });
                        $('#spanSendStatus').text('Mail NOT Sent, please contact system administrator.');
                    }
                }, 'json');
            }
        });

        $('#btnGetReferrals').on('click', function () {
            if (Survey.Structure != undefined) {
                var dataObject = {
                    fromDate: $('#Stats_I_ReferralsFromDate').val(),
                    toDate: $('#Stats_F_ReferralsToDate').val(),
                    fieldGroupID: Survey.Structure.FieldGroupID
                }
                $.getJSON('/settings/surveys/GetReferrals', dataObject, function (data) {
                    $.each(data, function (r, referral) {
                        let tr = '<tr>';
                        tr += '<td>' + referral.Referral + '</td>';
                        tr += '<td>' + referral.Email + '</td>';
                        tr += '<td>' + referral.Mobile + '</td>';
                        tr += '<td>' + referral.HomePhone + '</td>';
                        tr += '<td>' + referral.ReferredBy + '</td>';
                        tr += '<td>' + referral.Resort + '</td>';
                        tr += '<td>' + referral.Stay + '</td>';
                        tr += '<td>' + referral.SavedOn + '</td>';
                        tr += '<td>' + referral.Terminal + '</td>';
                        tr += '</tr>';
                        $('#tblReferralsReport tbody').append(tr);
                    });
                    UI.exportToExcel('Referrals Report');
                });
            } else {
                UI.messageBox(0, 'Please select a survey');
            }
        });
    }

    function clearSurvey() {
        $('#SurveyName').val('');
        $('#Instructions').val('');
        $('#pnlFormFields').html('');
        $('#pnlInfoFields').html('');
        $('#model').val('');
        Survey.Structure = undefined;
    }

    var loadSurveyObject = function () {
        if (Survey.Structure == undefined) {
            Survey.Structure = {
                FieldGroupID: 0,
                SurveyName: $('#SurveyName').val(),
                SurveyGuid: UI.generateGUID(),
                Instructions: $('#Instructions').val(),
                Logo: '',
                FormFields: [],
                InfoFields: []
            }
        } else {
            Survey.Structure.SurveyName = $('#SurveyName').val();
            Survey.Structure.Instructions = $('#Instructions').val();
            $('#model').val($.toJSON(Survey.Structure));
        }
    }

    var addInfoField = function () {
        Survey.loadSurveyObject();

        if ($('#I_FieldID').val() == '0') {
            var infoField = {
                I_FieldID: $('#I_FieldID').val(),
                I_TemporalID: UI.generateGUID(),
                I_Visibility: $('#I_Visibility').val(),
                I_FieldName: $('#I_FieldName').val(),
                I_FieldSubTypeID: $('#I_FieldSubTypeID').val(),
                I_Question: $('#I_Question').val()
            };
            Survey.Structure.InfoFields.push(infoField);
        } else {
            //edicion
        }
        //clear
        $('#I_FieldID').val('0');
        $('#I_FieldName').val('');
        $('#I_FieldSubTypeID').val('');
        $('#I_Question').val('');

        Survey.renderObject();
        $('#paramsInfoField').slideUp('fast');
        $('#model').val($.toJSON(Survey.Structure));
    }

    var addFormField = function () {
        Survey.loadSurveyObject();

        if ($('#F_TemporalID').val() == "") {
            var formField = {
                F_FieldID: $('#F_FieldID').val(),
                F_TemporalID: UI.generateGUID(),
                F_Question: $('#F_Question').val(),
                F_FieldName: $('#F_FieldName').val(),
                F_FieldSubTypeID: $('#F_FieldSubTypeID').val(),
                F_Visibility: $('#F_Visibility').val(),
                F_VisibleIf: $('#F_VisibleIf').val(),
                F_Options: $('#F_Options').val(),
                F_ParentFieldID: $('#F_ParentFieldID').val(),
                F_Order: 0,
                F_Fields: []
            };
            if ($('#F_ParentFieldID').val() != "") {
                //va dentro de otro campo
                $.each(Survey.Structure.FormFields, function (p, parent) {
                    if (parent.F_TemporalID == $('#F_ParentFieldID').val()) {
                        formField.F_Order = parent.F_Fields.length;
                        parent.F_Fields.push(formField);
                        return false;
                    } else {
                        var found = false;
                        $.each(parent.F_Fields, function (c, child) {
                            if (child.F_TemporalID == $('#F_ParentFieldID').val()) {
                                formField.F_Order = child.F_Fields.length;
                                child.F_Fields.push(formField);
                                found = true;
                                return false;
                            }
                        });
                        if (found) {
                            return false;
                        }
                    }
                });
            } else {
                //va en raiz
                formField.F_Order = Survey.Structure.FormFields.length;
                Survey.Structure.FormFields.push(formField);
            }
        } else {
            //edicion
            $.each(Survey.Structure.FormFields, function (p, firstLevelItem) {
                if (firstLevelItem.F_TemporalID == $('#F_TemporalID').val()) {
                    //modificar al item
                    firstLevelItem.F_FieldID = $('#F_FieldID').val();
                    firstLevelItem.F_Question = $('#F_Question').val();
                    firstLevelItem.F_FieldName = $('#F_FieldName').val();
                    firstLevelItem.F_FieldSubTypeID = $('#F_FieldSubTypeID').val();
                    firstLevelItem.F_Visibility = $('#F_Visibility').val();
                    firstLevelItem.F_VisibleIf = $('#F_VisibleIf').val();
                    firstLevelItem.F_Options = $('#F_Options').val();
                    if ($('#F_ParentFieldID').val() != "") {
                        firstLevelItem.F_ParentFieldID = $('#F_ParentFieldID').val();
                        //meter a campos de parent
                        $.each(Survey.Structure.FormFields, function (p, parent) {
                            if (parent.F_TemporalID == $('#F_ParentFieldID').val()) {
                                parent.F_Fields.push(firstLevelItem);
                                return false;
                            } else {
                                //buscar en nietos
                                var found = false;
                                $.each(parent.F_Fields, function (c, child) {
                                    if (child.F_TemporalID == $('#F_ParentFieldID').val()) {
                                        child.F_Fields.push(firstLevelItem);
                                        found = true;
                                        return false;
                                    }
                                });
                                if (found) {
                                    return false;
                                }
                            }
                        });

                        Survey.Structure.FormFields.splice(p, 1);
                    }
                    //romper ciclo
                    return false;
                } else {
                    $.each(firstLevelItem.F_Fields, function (s, secondLevelItem) {
                        if (secondLevelItem.F_TemporalID == $('#F_TemporalID').val()) {
                            //modificar al hijo
                            secondLevelItem.F_FieldID = $('#F_FieldID').val();
                            secondLevelItem.F_TemporalID = UI.generateGUID();
                            secondLevelItem.F_Question = $('#F_Question').val();
                            secondLevelItem.F_FieldName = $('#F_FieldName').val();
                            secondLevelItem.F_FieldSubTypeID = $('#F_FieldSubTypeID').val();
                            secondLevelItem.F_Visibility = $('#F_Visibility').val();
                            secondLevelItem.F_VisibleIf = $('#F_VisibleIf').val();
                            secondLevelItem.F_Options = $('#F_Options').val();
                            if (secondLevelItem.F_ParentFieldID != $('#F_ParentFieldID').val()) {
                                secondLevelItem.F_ParentFieldID = $('#F_ParentFieldID').val();
                                //cambio en el padre
                                if ($('#F_ParentFieldID').val() != "") {
                                    $.each(Survey.Structure.FormFields, function (p, parent) {
                                        if (parent.F_TemporalID == $('#F_ParentFieldID').val()) {
                                            parent.F_Fields.push(secondLevelItem);
                                            return false;
                                        } else {
                                            var found = false;
                                            $.each(parent.F_Fields, function (c, child) {
                                                if (child.F_TemporalID == $('#F_ParentFieldID').val()) {
                                                    child.F_Fields.push(secondLevelItem);
                                                    found = true;
                                                    return false;
                                                }
                                            });
                                            if (found) {
                                                return false;
                                            }
                                        }
                                    });
                                } else {
                                    //mover a raiz
                                    Survey.Structure.FormFields.push(secondLevelItem);
                                }
                                firstLevelItem.F_Fields.splice(s, 1);
                            }
                            return false;
                        } else {
                            $.each(secondLevelItem.F_Fields, function (t, thirdLevelItem) {
                                if (thirdLevelItem.F_TemporalID == $('#F_TemporalID').val()) {
                                    //modificar nieto
                                    thirdLevelItem.F_FieldID == $('#F_FieldID').val();
                                    thirdLevelItem.F_TemporalID = UI.generateGUID();
                                    thirdLevelItem.F_Question = $('#F_Question').val();
                                    thirdLevelItem.F_FieldName = $('#F_FieldName').val();
                                    thirdLevelItem.F_FieldSubTypeID = $('#F_FieldSubTypeID').val();
                                    thirdLevelItem.F_Visibility = $('#F_Visibility').val();
                                    thirdLevelItem.F_VisibleIf = $('#F_VisibleIf').val();
                                    thirdLevelItem.F_Options = $('#F_Options').val();
                                    if (thirdLevelItem.F_ParentFieldID != $('#F_ParentFieldID').val()) {
                                        thirdLevelItem.F_ParentFieldID = $('#F_ParentFieldID').val();
                                        //cambio en el padre
                                        if ($('#F_ParentFieldID').val() != "") {
                                            $.each(Survey.Structure.FormFields, function (p, parent) {
                                                if (parent.F_TemporalID == $('#F_ParentFieldID').val()) {
                                                    parent.F_Fields.push(thirdLevelItem);
                                                    return false;
                                                } else {
                                                    var found = false;
                                                    $.each(parent.F_Fields, function (c2, child) {
                                                        if (child.F_TemporalID == $('#F_ParentFieldID').val()) {
                                                            child.F_Fields.push(thirdLevelItem);
                                                            found = true;
                                                            return false;
                                                        }
                                                    });
                                                    if (found) {
                                                        return false;
                                                    }
                                                }
                                            });
                                        } else {
                                            //mover a raiz
                                            Survey.Structure.FormFields.push(thirdLevelItem);
                                        }
                                        secondLevelItem.F_Fields.splice(s, 1);
                                    }
                                    return false;
                                }
                            });
                        }
                    })
                }
            });
        }
        //actualizar la lista de campos del survey
        updateFieldsOnDDL();
        //clear
        Survey.clearFormField();

        Survey.renderObject();

        //set order
        fieldsOrder();
    }

    var clearFormField = function () {
        $('#F_FieldID').val('0');
        $('#F_TemporalID').val('');
        $('#F_Question').val('');
        $('#F_FieldName').val('');
        $('#F_FieldSubTypeID').val('');
        $('#F_Visibility').val('1');
        $('#divVisibleIf').hide();
        $('#F_Options').val('');
        $('#F_ParentFieldID').val('');
        $('#F_Question').focus();
    }

    function updateFieldsOnDDL() {
        if (Survey.Structure != undefined) {
            var options = '<option value="">None</option>';
            var visibleIfOptions = '<option value="">None</option>';
            $.each(Survey.Structure.FormFields, function (i, field) {
                options += '<option value="' + field.F_TemporalID + '">' + field.F_FieldName + '</option>';
                visibleIfOptions += '<option value="' + field.F_TemporalID + '">' + field.F_FieldName + '</option>';
                $.each(field.F_Fields, function (c, child) {
                    options += '<option value="' + child.F_TemporalID + '">  ' + child.F_FieldName + '</option>';
                    visibleIfOptions += '<option value="' + child.F_TemporalID + '">  ' + child.F_FieldName + '</option>';
                    $.each(child.F_Fields, function (g, gchild) {
                        visibleIfOptions += '<option value="' + gchild.F_TemporalID + '">  ' + gchild.F_FieldName + '</option>';
                    });
                });
            });
            $('#F_VisibleIf').html(visibleIfOptions);
            $('#F_ParentFieldID').html(options);
        }
    }

    var renderObject = function () {
        $('#SurveyName').val(Survey.Structure.SurveyName);
        $('#Instructions').val(Survey.Structure.Instructions);
        $('#pnlInfoFields').html('');
        $.each(Survey.Structure.InfoFields, function (i, field) {
            var fieldStr = '';
            if (field.I_Visibility == "1") {
                fieldStr = '<span class="field-visible">'
                    + '<input type="hidden" value="' + field.I_FieldID + '" />'
                    + '<span class="btn-close info-field" data-delete="' + field.I_TemporalID + '"></span>'
                    + '<span class="field-question">' + field.I_Question + '</span> : '
                    + '<span class="field-name">' + field.I_FieldName + ' [' + $('#I_FieldSubTypeID option[value="' + field.I_FieldSubTypeID + '"]').text() + ']</span>'
                    + '</span>';
            } else {
                fieldStr = '<span class="field-hidden">'
                    + '<input type="hidden" value="' + field.I_FieldID + '" />'
                    + '<span class="btn-close info-field" data-delete="' + field.I_TemporalID + '"></span>'
                    + '<span class="field-name">' + field.I_FieldName + ' [' + $('#I_FieldSubTypeID option[value="' + field.I_FieldSubTypeID + '"]').text() + ']</span>'
                    + '</span>';
            }
            $('#pnlInfoFields').append(fieldStr);
        });
        updateFieldsOnDDL();

        $('.btn-close.info-field').on('click', function () {
            var id = $(this).attr('data-delete');
            $.each(Survey.Structure.InfoFields, function (i, field) {
                if (field.I_TemporalID == id) {
                    Survey.Structure.InfoFields.splice(i, 1);
                    return false;
                }
            });
            fieldsOrder();
            Survey.renderObject();
        });

        $('#pnlFormFields').html('');
        $('#pnlFormFields').append(renderFormFields(Survey.Structure.FormFields));

        $('.btn-close.form-field').on('click', function () {
            var id = $(this).attr('data-delete');
            $.each(Survey.Structure.FormFields, function (i, field) {
                if (field.F_TemporalID == id) {
                    Survey.Structure.FormFields.splice(i, 1);
                    return false;
                } else {
                    var childfound = false;
                    $.each(field.F_Fields, function (c, child) {
                        if (child.F_TemporalID == id) {
                            field.F_Fields.splice(c, 1);
                            childfound = true;
                            return false;
                        }
                    });
                    if (childfound) {
                        return false;
                    }
                }
            });
            fieldsOrder();
            Survey.renderObject();
        });
        $('.btn-edit.form-field').on('click', function () {
            var id = $(this).attr('data-edit');
            $('#F_ParentFieldID option').show();
            $('#F_ParentFieldID option[value="' + id + '"]').hide();
            $.each(Survey.Structure.FormFields, function (i, field) {
                var childfound = false;
                if (field.F_TemporalID == id) {
                    $('#F_FieldID').val(field.F_FieldID);
                    $('#F_TemporalID').val(field.F_TemporalID);
                    $('#F_Question').val(field.F_Question);
                    $('#F_FieldName').val(field.F_FieldName);
                    $('#F_FieldSubTypeID').val(field.F_FieldSubTypeID);
                    $('#F_Visibility').val(field.F_Visibility);
                    $('#F_Visibility').trigger('change');
                    $('#F_VisibleIf').val(field.F_VisibleIf);
                    $('#F_Options').val(field.F_Options);
                    $('#F_ParentFieldID').val(field.F_ParentFieldID);
                    return false;
                } else {
                    //checar en los hijos
                    $.each(field.F_Fields, function (c, child) {
                        if (child.F_TemporalID == id) {
                            $('#F_FieldID').val(child.F_FieldID);
                            $('#F_TemporalID').val(child.F_TemporalID);
                            $('#F_Question').val(child.F_Question);
                            $('#F_FieldName').val(child.F_FieldName);
                            $('#F_FieldSubTypeID').val(child.F_FieldSubTypeID);
                            $('#F_Visibility').val(child.F_Visibility);
                            $('#F_Visibility').trigger('change');
                            $('#F_VisibleIf').val(child.F_VisibleIf);
                            $('#F_Options').val(child.F_Options);
                            $('#F_ParentFieldID').val(child.F_ParentFieldID);
                            childfound = true;
                            return false;
                        } else {
                            $.each(child.F_Fields, function (g, grandchild) {
                                if (grandchild.F_TemporalID == id) {
                                    $('#F_FieldID').val(grandchild.F_FieldID);
                                    $('#F_TemporalID').val(grandchild.F_TemporalID);
                                    $('#F_Question').val(grandchild.F_Question);
                                    $('#F_FieldName').val(grandchild.F_FieldName);
                                    $('#F_FieldSubTypeID').val(grandchild.F_FieldSubTypeID);
                                    $('#F_Visibility').val(grandchild.F_Visibility);
                                    $('#F_Visibility').trigger('change');
                                    $('#F_VisibleIf').val(grandchild.F_VisibleIf);
                                    $('#F_Options').val(grandchild.F_Options);
                                    $('#F_ParentFieldID').val(grandchild.F_ParentFieldID);
                                    childfound = true;
                                    return false;
                                }
                            });
                            if (childfound) {
                                return false;
                            }
                        }
                    });

                    if (childfound) {
                        return false;
                    }
                }
            });

            $('#paramsFormField').slideDown('fast');
            var targetOffset = $("#paramsFormField").offset().top;
            $('html,body').animate({ scrollTop: targetOffset }, 500);
        });
        $('.btn-order.form-field').on('click', function () {
            if ($('span[data-move-id="' + $(this).attr('data-order') + '"]').length == 0) {
                var orderForm = '<span class="move-form" data-move-id="' + $(this).attr('data-order') + '">Move field <select class="design-mode"><option value="1">before</option><option value="2">after</option></select> the field <select class="design-mode">';
                $.each($(this).parent().siblings(), function (f, field) {
                    orderForm += '<option value="' + $(this).attr('data-tempid') + '">' + $(this).attr('data-fieldname') + '</option>';
                });
                orderForm += '</select><input type="button" value="MOVE" class="mini-button button-centered btn-move" /></span>';
                $(this).after(orderForm);
                $('.btn-move').off('click').on('click', function () {
                    var baseelementid = $(this).parent().find('select').eq(1).val();
                    if ($(this).parent().find('select').eq(0).val() == '1') {
                        //before
                        $('div[data-tempid="' + baseelementid + '"]').before($(this).parent().parent());
                    } else {
                        //after
                        $('div[data-tempid="' + baseelementid + '"]').after($(this).parent().parent());
                    }

                    //reasignar orden
                    fieldsOrder();

                    $('span[data-move-id="' + $(this).parent().parent().attr('data-tempid') + '"]').remove();
                });
            } else {
                $('span[data-move-id="' + $(this).attr('data-order') + '"]').remove();
            }
        });
    }

    function fieldsOrder() {
        //obtener el orden del elemento base
        //var level = 0;
        //var baseelementid = $(this).parent().find('select').eq(1).val();
        //$.each(Survey.Structure.FormFields, function (p, fl) {
        //    if (fl.F_TemporalID == baseelementid) {
        //        level = 1;
        //        return false;
        //    } else {
        //        var found = false;
        //        $.each(fl.F_Fields, function (s, sl) {
        //            if (sl.F_TemporalID == baseelementid) {
        //                level = 2;
        //                found = true;
        //                return false;
        //            } else {
        //                $.each(sl.F_Fields, function (t, tl) {
        //                    if (tl.F_TemporalID == baseelementid) {
        //                        level = 3;
        //                        found = true;
        //                        return false;
        //                    }
        //                });
        //                if (found) {
        //                    return false;
        //                }
        //            }
        //        });
        //        if (found) {
        //            return false;
        //        }
        //    }
        //});

        $('#pnlFormFields>.question').each(function (i, item) {
            var elementid = $(this).attr('data-tempid');
            $.each(Survey.Structure.FormFields, function (f, field) {
                if (field.F_TemporalID == elementid) {
                    field.F_Order = i;
                }
            });

            //second level
            $('#pnlFormFields>.question>.field-children>.question').each(function (c, citem) {
                var childid = $(this).attr('data-tempid');
                $.each(Survey.Structure.FormFields, function (f1, f1field) {
                    var found = false;
                    $.each(f1field.F_Fields, function (g1, g1field) {
                        if (g1field.F_TemporalID == childid) {
                            g1field.F_Order = c;
                            found = true;
                            return false;
                        }
                    });
                    if (found) {
                        return false;
                    }
                });

                //third level
                $(this).find('.field-children>.question').each(function (g, gitem) {
                    var grandchild = $(this).attr('data-tempid');
                    $.each(Survey.Structure.FormFields, function (f2, f2field) {
                        var found = false;
                        $.each(f2field.F_Fields, function (j, jfield) {
                            $.each(jfield.F_Fields, function (h, hfield) {
                                if (hfield.F_TemporalID == grandchild) {
                                    hfield.F_Order = g;
                                    found = true;
                                    return false;
                                }
                            });
                            if (found) {
                                return false;
                            }
                        });
                        if (found) {
                            return false;
                        }
                    });
                });
            });
        });

        //loop para mostrar el orden de los elementos
        //console.log('order');
        //$.each(Survey.Structure.FormFields, function (x, xf) {
        //    console.log(x + ' : ' + xf.F_Order);
        //    $.each(xf.F_Fields, function (y, yf) {
        //        console.log(' ' + y + ' : ' + yf.F_Order);
        //        $.each(yf.F_Fields, function (z, zf) {
        //            console.log('   ' + z + ' : ' + zf.F_Order);
        //        });
        //    });
        //});

        //actualización del objeto para envío
        $('#model').val($.toJSON(Survey.Structure));
    }

    var renderFormFields = function (fieldsList) {
        var fieldStr = '';
        $.each(fieldsList, function (i, field) {
            switch (parseInt(field.F_Visibility)) {
                case 0: //hidden
                    fieldStr += '<div class="question form-field-hidden subtype-' + field.F_FieldSubTypeID + '" data-tempid="' + field.F_TemporalID + '" data-fieldname="' + field.F_FieldName + '">';
                    break;
                case 1: //visible
                    fieldStr += '<div class="question form-field-visible subtype-' + field.F_FieldSubTypeID + '" data-tempid="' + field.F_TemporalID + '" data-fieldname="' + field.F_FieldName + '">';
                    break;
                case 2: //visible if
                    fieldStr += '<div class="question form-field-visible-if subtype-' + field.F_FieldSubTypeID + '" data-tempid="' + field.F_TemporalID + '" data-fieldname="' + field.F_FieldName + '">';
                    break;
            }
            fieldStr += '<input type="hidden" value="' + field.F_FieldID + '" />';
            fieldStr += '<span class="label">' + field.F_Question + '</span>';
            fieldStr += '<span class="btn-close form-field" data-delete="' + field.F_TemporalID + '"></span><span class="btn-edit form-field" data-edit="' + field.F_TemporalID + '"></span><span class="btn-order form-field" data-order="' + field.F_TemporalID + '"></span>';
            switch (parseInt(field.F_FieldSubTypeID)) {
                case 1: //Questions Group
                    fieldStr += '<span class="right">1 = least desirable :: 5 = most desirable</span>';
                    break;
                case 2: //Rating Question
                    fieldStr += '<div class="answer"><div class="answer-button" data-value="1">1</div>'
                                + '<div class="answer-button" data-value="2">2</div>'
                                + '<div class="answer-button" data-value="3">3</div>'
                                + '<div class="answer-button" data-value="4">4</div>'
                                + '<div class="answer-button" data-value="5">5</div>'
                                + '<input type="hidden" id="fld' + field.I_FieldID + '" value="0"></div>';
                    break;
                case 3: //Yes or No Question
                    fieldStr += '<div class="answer">'
                        + '<div class="answer-button" data-value="Yes">Yes</div>'
                        + '<div class="answer-button" data-value="No">No</div>'
                        + '<input type="hidden" id="fld' + field.I_FieldID + '" value="">'
                        + '</div>';
                    break;
                case 4: //Referrals Form
                    fieldStr += '<div class="answer">'
                        + '<div class="referral">'
                        + '<div class="param">'
                            + '<div class="param-description">Name</div>'
                            + '<input type="text" data-value="Name">'
                        + '</div>'
                        + '<div class="param">'
                            + '<div class="param-description">Email</div>'
                            + '<input type="text" data-value="Email">'
                        + '</div>'
                        + '<div class="param">'
                            + '<div class="param-description">Cellphone</div>'
                            + '<input type="text" data-format="phone" maxlength="14" data-value="Mobile">'
                        + '</div>'
                        + '<div class="param">'
                            + '<div class="param-description">Home Phone</div>'
                            + '<input type="text" data-format="phone" maxlength="14" data-value="HomePhone">'
                        + '</div>'
                        + '</div>'
                        + '<div class="referral-add" id="btnAddReferral">+ Share this promo with one more friend</div>'
                        + '</div>';
                    break;
                case 5: //Options Question
                    fieldStr += '<div class="answer"><select id="fld' + field.I_FieldID + '">';
                    $.each(field.F_Options.split(','), function (o, option) {
                        fieldStr += '<option value="' + option + '">' + option + '</option>';
                    });
                    fieldStr += '</select></div>';
                    break;
                case 6: //Text Field Question
                    fieldStr += '<div class="answer"><span class="param"><input type="text" /></span></div>';
                    break;
                case 7: //Text Area / Comments Field
                    fieldStr += '<div class="answer"><span class="param"><textarea></textarea></span></div>';
                    break;
            }
            //F_Fields
            if (field.F_Fields.length > 0) {
                fieldStr += '<div class="field-children">' + Survey.renderFormFields(field.F_Fields) + '</div>';
            }

            fieldStr += '</div>';
        });
        return fieldStr;
    }

    var saved = function () {
        $('#btnSearchSurvey').trigger('click');
        $('#fdsSurveyInfo>div').slideUp('fast');
    }

    var searchResults = function () {
        UI.tablesStripedEffect();
        UI.tablesHoverEffect();
        Survey.clearFormField();
        $('#tblSurveys tbody tr').off('click').on('click', function () {
            $(this).parent().find('.selected-row').removeClass('selected-row primary');
            $(this).addClass('selected-row primary');
            Survey.clearFormField();
            $.post('/settings/surveys/GetSurvey/' + $(this).attr('id'), null, function (data) {
                Survey.Structure = data;

                //fdsSurveyInfo
                Survey.renderObject();
                $('#model').val($.toJSON(Survey.Structure));

                //fdsSurveyStats
                Survey.renderStatsFilters();
                $('#fdsSurveyStats>div').slideDown('fast');
            }, 'json');
        });
    }

    var renderStatsFilters = function () {
        $('#surveyStatsParams>.editor-alignment').not('.stats-original-fields').remove();
        $.each(Survey.Structure.InfoFields, function (i, info) {
            if (info.I_FieldName.indexOf('ID') < 0 && info.I_Visibility == 0 && info.I_FieldSubTypeID == 11) {
                var htmlfield = '<div class="editor-alignment">'
                        + '<div class="editor-label">'
                            + '<label for="Stats_' + info.I_FieldName + '">' + info.I_FieldName.replace(/([a-z])([A-Z])/g, '$1 $2').replace('$','') + '</label>'
                        + '</div>'
                        + '<div class="editor-field">'
                            + '<select id="Stats_' + info.I_FieldName.replace('$','') + '" name="Stats_' + info.I_FieldName + '" multiple="multiple" class="field-value"></select>'
                        + '</div>'
                    + '</div>';
                $('.stats-original-fields:last').after(htmlfield);
            }
        });
        $('#surveyStatsParams').append('<div class="editor-alignment align-from-top"><div class="editor-field"><input type="button" class="button" id="btnGetStats" value="GET"></div></div>');
        $('select[multiple="multiple"]').multiselect({
            noneSelectedText: "All",
            minWidth: "auto",
            selectedList: 1
        }).multiselectfilter();
        $('#btnGetStats').off('click').on('click', function () {
            var valid = true;
            if ($('#Stats_I_FromDate').val() == "") {
                valid = false;
                UI.messageBox(-1, "You need to specify a range of dates.");
            }
            if (valid && $('#Stats_F_ToDate').val() == "") {
                valid = false;
                UI.messageBox(-1, "You need to specify a range of dates.");
            }

            if (valid) {
                //enviar parámetros para obtener lista de surveys
                var fields = [];
                $('#surveyStatsParams .field-value').each(function () {
                    //console.log($(this).prop("type"));
                    var fieldValue = {
                        FieldName: $(this).attr('id').substr($(this).attr('id').lastIndexOf("_") + 1),
                        Value: (Array.isArray($(this).val()) ? $(this).val().join() : getFieldValues($(this).attr('id')))
                    }
                    fields.push(fieldValue);
                });
                var objectData = {
                    fields: $.toJSON(fields)
                }
                $.post('/settings/surveys/GetStats/' + Survey.Structure.FieldGroupID, objectData, function (data) {
                    Survey.Stats = data;
                    $('#pSent').text(data.Sent);
                    $('#pOpen').text(data.Open);
                    $('#percOpen').text((data.Open * 100 / data.Sent).toFixed(1) + '%');
                    $('#pAnswered').text(data.Submitted);
                    $('#percAnswered').text((data.Submitted * 100 / data.Sent).toFixed(1) + '%');
                    $('#pReferrals').text(getReferrals());

                    $('#pSent').off('click').on('click', function () {
                        Survey.showList(getSurveysWithField('$Sent'), "Sent Surveys", null);
                    });
                    $('#pOpen').off('click').on('click', function () {
                        Survey.showList(getSurveysWithField('Open'), "Open Surveys", null);
                    });
                    $('#pAnswered').off('click').on('click', function () {
                        Survey.showList(getSurveysWithField('Submitted'), "Answered Surveys", null);
                    });
                    $('#pReferrals').off('click').on('click', function () {
                        $('#spnDetailsDescription').text('Referrals');
                        $('#spnDetailsCounter').text(Survey.Referrals.length);
                        $('#tblSurveyDetails').html('<table style="width:100%" class="table"><thead><tr><th>Name</th><th>Email</th><th>Mobile</th><th>Home Phone</th></tr></thead><tbody></tbody></table>');
                        $.each(Survey.Referrals, function (r, referred) {
                            $('#tblSurveyDetails table tbody').append('<tr id="' + referred.SurveyID + '"><td>' + referred.Name + '</td><td>' + referred.Email + '</td><td>' + referred.Mobile + '</td><td>' + referred.HomePhone + '</td></tr>');
                        });
                        UI.tablesHoverEffect();
                        UI.tablesStripedEffect();
                        makeTableClickable();
                    });

                    var referrals = 0;

                    renderStatsReport();

                    $('#surveyStatsReport').slideDown('fast', function () {
                        $('html,body').animate({ scrollTop: $("#surveyStatsReport").offset().top - 50 }, 500);
                    });
                }, 'json');
            }
        });
    }

    function getFieldValues(id) {
        var values = '';
        if ($('#' + id).prop('type') == 'text') {
            values = $('#' + id).val();
        } else {
            $('#' + id + ' option').each(function () {
                if (values != '') {
                    values += ',';
                }
                values += $(this).val()
            });
        }
        //console.log(id + ': ' + values);
        return values;        
    }

    function getReferrals() {
        Survey.Referrals = [];
        $.each(Survey.Stats.Surveys, function (s, survey) {
            $.each(survey.Fields, function (f, field) {
                if (field.SubTypeID == 4 && field.Value != '') {
                    var refObj = JSON.parse(field.Value);
                    $.each(refObj, function (o, object) {
                        object.SurveyID = survey.TransactionID;
                        Survey.Referrals.push(object);
                    });
                }
            });
        });
        return Survey.Referrals.length;
    }

    function renderStatsReport() {
        $('#divSurveyTopics').html('');
        $('#tblSurveyDetails').html('');
        $('#spnDetailsDescription').html('');
        $('#spnDetailsCounter').html('');
        $.each(Survey.Structure.FormFields, function (f, field) {
            getHtmlAnswer(field, 1);
            $.each(field.F_Fields, function (f2, field2) {
                getHtmlAnswer(field2, 2);
                $.each(field2.F_Fields, function (f3, field3) {
                    getHtmlAnswer(field3, 3);
                });
            });
        });
    }

    function getReportValues(fieldObj) {
        var Params = {
            FieldGuid: fieldObj.F_TemporalID,
            Answers: 0,
            Abstentions: 0,
            Average: 0,
            MaxAmount: 0,
            Graphic: []
        }

        //creación de barras de gráfica
        switch (fieldObj.F_FieldSubTypeID) {
            case 2: //Rating Question
                for (var i = 1; i <= 5; i++) {
                    var bar = {
                        Value: i.toString(),
                        Percentage: 0,
                        Amount: 0,
                        SurveyIDs: []
                    }
                    Params.Graphic.push(bar);
                }
                break;
            case 3: //Yes or No
                Params.Graphic = [
                    {
                        Value: "Yes",
                        Percentage: 0,
                        Amount: 0,
                        SurveyIDs: []
                    },
                    {
                        Value: "No",
                        Percentage: 0,
                        Amount: 0,
                        SurveyIDs: []
                    }
                ]
                break;
            case 5: //Options Questions
                $.each(fieldObj.F_Options.split(','), function (o, opt) {
                    var bar = {
                        Value: opt,
                        Percentage: 0,
                        Amount: 0,
                        SurveyIDs: []
                    }
                    Params.Graphic.push(bar);
                });
                break;
        }

        var total = 0;
        $.each(Survey.Stats.Surveys, function (s, survey) { // búsqueda en cada survey
            $.each(survey.Fields, function (f, field) { // búsqueda en cada campo
                if (field.FieldGuid == fieldObj.F_TemporalID) {
                    if (field.Value == "0" || field.Value == "") {
                        Params.Abstentions++;
                    } else {
                        Params.Answers++;
                        total += parseFloat(field.Value);
                        $.each(Params.Graphic, function (b, bar) {
                            if (bar.Value == field.Value) {
                                bar.Amount++;
                                bar.SurveyIDs.push(survey.TransactionID);
                            }
                        });
                    }
                    return false;
                }
            });
        });

        //obtener porcentajes de barras de gráficas, maxamount y average
        Params.Average = total / Params.Answers;
        $.each(Params.Graphic, function (b, bar) {
            bar.Percentage = bar.Amount * 100 / Params.Answers;
            if (Params.MaxAmount < bar.Amount) {
                Params.MaxAmount = bar.Amount;
            }
        });

        if (Params.Average.toString().indexOf('.') >= 0) {
            Params.Average = Params.Average.toString().substr(0, Params.Average.toString().indexOf('.') + 2);
        }        

        return Params;
    }

    function getHtmlAnswer(field, level) {
        var htmlString = '';
        var Params;
        switch (parseInt(field.F_FieldSubTypeID)) {
            case 1: //Questions Group
                Params = getReportValues(field);
                htmlString = '<span class="answer subtype-1 level-' + level + '" data-tempid="' + field.F_TemporalID + '"><span class="answer-pleca">' + field.F_FieldName + '<span class="answer-average">' + Params.Average + '</span></span></span>';
                break;
            case 2: //Rating Question
                Params = getReportValues(field);
                htmlString = '<span class="answer subtype-2 level-' + level + '" data-tempid="' + field.F_TemporalID + '"><span class="answer-pleca">' + field.F_FieldName + '</span><span class="answer-content">'
                    + '<span class="answer-col-1">'
                    + '<span class="summary-elements-alignment"><span class="summary-labels" title="Answers">Ans</span><span class="summary-fields">' + Params.Answers + '</span></span>'
                    + '<span class="summary-elements-alignment"><span class="summary-labels" title="Abstentions">Abs</span><span class="summary-fields">' + Params.Abstentions + '</span></span>'
                    + '<span class="summary-elements-alignment"><span class="summary-labels" title="Average">Avg</span><span class="summary-fields">' + Params.Average + '</span></span>'
                    + '</span>'
                    + '<span class="answer-col-2">'

                        + '<span class="answer-graphic" data-tempid="' + field.F_TemporalID + '">';
                htmlString += '<span class="graphic-area">';
                $.each(Params.Graphic, function (b, bar) {
                    htmlString += '<span class="graphic-bar" data-surveys="' + bar.SurveyIDs.join() + '" data-value="' + bar.Value + '"><span class="graphic-bar-percentage">' + bar.Percentage.toFixed(1) + '%</span><span class="graphic-bar-amount">' + bar.Amount + '</span></span>';
                });
                htmlString += '</span><span class="graphic-legend">'
                $.each(Params.Graphic, function (b, bar) {
                    htmlString += '<span class="graphic-bar-legend">' + bar.Value + '</span>';
                });
                htmlString += '</span>';
                htmlString += '</span>'

            + '</span>'
            + '</span></span>';
                break;
            case 3: //Yes or No
                Params = getReportValues(field);
                htmlString = '<span class="answer subtype-3 level-' + level + '" data-tempid="' + field.F_TemporalID + '"><span class="answer-pleca">' + field.F_FieldName + '</span><span class="answer-content">'
                    + '<span class="answer-col-1">'
                    + '<span class="summary-elements-alignment"><span class="summary-labels" title="Answers">Ans</span><span class="summary-fields">' + Params.Answers + '</span></span>'
                    + '<span class="summary-elements-alignment"><span class="summary-labels" title="Abstentions">Abs</span><span class="summary-fields">' + Params.Abstentions + '</span></span>'
                    + '</span>'
                    + '<span class="answer-col-2">'

                        + '<span class="answer-graphic" data-tempid="' + field.F_TemporalID + '">';
                htmlString += '<span class="graphic-area">';
                $.each(Params.Graphic, function (b, bar) {
                    htmlString += '<span class="graphic-bar" data-surveys="' + bar.SurveyIDs.join() + '" data-value="' + bar.Value + '"><span class="graphic-bar-percentage">' + bar.Percentage + '%</span><span class="graphic-bar-amount">' + bar.Amount + '</span></span>';
                });
                htmlString += '</span><span class="graphic-legend">'
                $.each(Params.Graphic, function (b, bar) {
                    htmlString += '<span class="graphic-bar-legend">' + bar.Value + '</span>';
                });
                htmlString += '</span>';
                htmlString += '</span>'

            + '</span>'
            + '</span></span>';
                break;
            case 4: //Referrals

                break;
            case 5: //Options Questions
                Params = getReportValues(field);
                htmlString = '<span class="answer subtype-3 level-' + level + '" data-tempid="' + field.F_TemporalID + '"><span class="answer-pleca">' + field.F_FieldName + '</span><span class="answer-content">'
                    + '<span class="answer-col-1">'
                    + '<span class="summary-elements-alignment"><span class="summary-labels" title="Answers">Ans</span><span class="summary-fields">' + Params.Answers + '</span></span>'
                    + '<span class="summary-elements-alignment"><span class="summary-labels" title="Abstentions">Abs</span><span class="summary-fields">' + Params.Abstentions + '</span></span>'
                    + '</span>'
                    + '<span class="answer-col-2">'

                    + '<span class="answer-graphic" data-tempid="' + field.F_TemporalID + '">';
                htmlString += '<span class="graphic-area">';
                $.each(Params.Graphic, function (b, bar) {
                    htmlString += '<span class="graphic-bar" data-surveys="' + bar.SurveyIDs.join() + '" data-value="' + bar.Value + '"><span class="graphic-bar-percentage">' + bar.Percentage + '%</span><span class="graphic-bar-amount">' + bar.Amount + '</span></span>';
                });
                htmlString += '</span><span class="graphic-legend">'
                $.each(Params.Graphic, function (b, bar) {
                    htmlString += '<span class="graphic-bar-legend">' + bar.Value + '</span>';
                });
                htmlString += '</span>';
                htmlString += '</span>'

            + '</span>'
            + '</span></span>';
                
                break;
        }
        $('#divSurveyTopics').append(htmlString);
        if(Params != undefined){
            setGraphic(Params);
        }
        $('.graphic-bar').off('click').on('click', function () {
            Survey.showList($(this).attr('data-surveys').split(','), $(this).parent().parent().parent().parent().parent().find('.answer-pleca').text(), $(this).attr('data-value'));
        });
        $('.graphic-bar-percentage').hover(function () {
            $(this).siblings().fadeIn('fast');
        }, function () {
            $(this).siblings().fadeOut('fast');
        });
    }

    function setGraphic(Params) {
        $.each($('.answer-graphic[data-tempid="' + Params.FieldGuid + '"] > .graphic-area > .graphic-bar'), function (b, bar) {
            $(this).height(parseFloat(Params.Graphic[b].Amount) * $(this).parent().height() / parseFloat(Params.MaxAmount));
            var rgb = parseInt(255 - (parseFloat(Params.Graphic[b].Amount) * 255 / parseFloat(Params.MaxAmount)));
            $(this).css('background-color', 'rgba(' + rgb + ',' + rgb + ',' + rgb + ', 0.6)');
        });
    }

    var getStatsSearchParams = function () {
        if (Survey.Structure != undefined) {
            var dataObject = {
                from: $('#Stats_I_FromDate').val(),
                to: $('#Stats_F_ToDate').val()
            }
            $.getJSON('/settings/surveys/GetStatsParams/' + Survey.Structure.FieldGroupID, dataObject, function (data) {
                $.each(data, function (f, field) {
                    $('#Stats_' + field.FieldName.replace('$','')).fillSelect(field.Values);
                });
                $('select[multiple="multiple"]').multiselect('refresh');
            });
        }
    }

    function getCommentFields() {
        var arrIDs = [];
        $.each(Survey.Structure.FormFields, function (x, field) {
            if (field.F_FieldSubTypeID == 7) {
               arrIDs.push(field.F_TemporalID);
            }
            $.each(field.F_Fields, function (x2, field2) {
                if (field2.F_FieldSubTypeID == 7) {
                    arrIDs.push(field2.F_TemporalID);
                }
                $.each(field2.F_Fields, function (x3, field3) {
                    if (field3.F_FieldSubTypeID == 7) {
                        arrIDs.push(field3.F_TemporalID);
                    }
                });
            });
        });
        return arrIDs;
    }

    function getSurveysWithField(fieldname) {
        var surveyids = [];
        $.each(Survey.Stats.Surveys, function (s, survey) {
            $.each(survey.Fields, function (f, field) {
                if (field.FieldName == fieldname) {
                    surveyids.push(survey.TransactionID);
                }
            });
        });
        return surveyids;
    }

    var showList = function (arrayids, detail, value) {
        $('html,body').animate({ scrollTop: $("#surveyStatsReport").offset().top - 50 }, 500);
        $('#spnDetailsDescription').text(detail + (value != null ? ' :: ' + value : ''));
        $('#spnDetailsCounter').text(arrayids.length);
        $('#tblSurveyDetails').html('<table style="width:100%" class="table"><thead><tr><th>Name</th><th>Sent</th><th>Open</th><th>Submitted</th><th>Terminal</th><th>...</th></tr></thead><tbody></tbody></table>');
        var arrayCommentsGuids = getCommentFields();
        $.each(Survey.Stats.Surveys, function (s, survey) {
            if ($.inArray(survey.TransactionID, arrayids) >= 0) {
                var table = '<tr id="' + survey.TransactionID + '"><td></td><td></td><td></td><td></td><td></td><td></td></tr>';
                $('#tblSurveyDetails table tbody').append(table);
                var comments = 0;
                $.each(survey.Fields, function (f, field) {
                    if (field.FieldName.indexOf("GuestName") >= 0) {
                        $('#tblSurveyDetails table tbody tr:last td').eq(0).text(field.Value);
                    }
                    if (field.FieldName.indexOf("FirstName") >= 0) {
                        $('#tblSurveyDetails table tbody tr:last td').eq(0).append(field.Value);
                    }
                    if (field.FieldName.indexOf("LastName") >= 0) {
                        $('#tblSurveyDetails table tbody tr:last td').eq(0).append("  " + field.Value);
                    }
                    if (field.FieldName.indexOf("Sent") >= 0) {
                        $('#tblSurveyDetails table tbody tr:last td').eq(1).text(field.Value);;
                    }
                    if (field.FieldName.indexOf("Open") >= 0) {
                        $('#tblSurveyDetails table tbody tr:last td').eq(2).text(field.Value);
                    }
                    if (field.FieldName.indexOf("Submitted") >= 0) {
                        $('#tblSurveyDetails table tbody tr:last td').eq(3).text(field.Value);
                    }
                    if (field.FieldName.indexOf("Terminal") >= 0) {
                        $('#tblSurveyDetails table tbody tr:last td').eq(4).text(field.Value);
                    }
                    //comentarios
                    if ($.inArray(field.FieldGuid, arrayCommentsGuids) >= 0 && field.Value != "") {
                        comments++;
                    }
                });
                if (comments > 0) {
                    $('#tblSurveyDetails table tbody tr:last td').eq(5).html('<div class="comment-box"><div class="comment-square"></div><div class="comment-triangle"></div></div>');
                }
            }
        });
        UI.tablesStripedEffect();
        UI.tablesHoverEffect();
        makeTableClickable();
        $('#divSurveyDetailsSlider').animate({
            'marginLeft': '0'
        }, 500);
    }

    function makeTableClickable() {
        $('#tblSurveyDetails table tbody tr').off('click').on('click', function () {
            var i = $("#tblSurveyDetails table tbody tr").index(this);
            $('.survey-content-infofields').html('');
            $('.survey-content-formfields').html('');
            $(this).parent().find('.selected-row').removeClass('selected-row primary');
            $(this).addClass('selected-row primary');
            var id = $(this).attr('id');
            $.each(Survey.Stats.Surveys, function (s, survey) {
                if (survey.TransactionID == id) {
                    //paginación
                    $('#tblSurveyDetails table tbody tr')
                    $('#spnDetailsCounter').text((i + 1) + '/' + $('#tblSurveyDetails table tbody tr').length);
                    if (i == 0) {
                        $('#btnPreviousSurvey').attr('src', '/Images/btn_prev_shadow.png');
                    } else {
                        $('#btnPreviousSurvey').attr('src', '/Images/btn_prev.png').off('click').on('click', function () {
                            $('#tblSurveyDetails table tbody tr').eq(i - 1).trigger('click');
                        });
                    }
                    if (i + 1 == $('#tblSurveyDetails table tbody tr').length) {
                        $('#btnNextSurvey').attr('src', '/Images/btn_next_shadow.png');
                    } else {
                        $('#btnNextSurvey').attr('src', '/Images/btn_next.png').off('click').on('click', function () {
                            $('#tblSurveyDetails table tbody tr').eq(i + 1).trigger('click');
                        });
                    }

                    //llenado de survey
                    $.each(survey.Fields, function (f, field) {
                        if (field.TypeID == 2 || field.TypeID == 3) {
                            if (field.FieldName != 'DB' && field.FieldName.indexOf('ID') < 0) {
                                $('.survey-content-infofields').append('<span class="survey-content-field"><span class="survey-content-field-name">' + field.FieldName.replace(/([a-z])([A-Z])/g, '$1 $2').replace('$', '') + '</span><span class="survey-content-field-value">' + field.Value + '</span></span>');
                            }
                        } else {
                            if (field.SubTypeID != 4) {
                                var htmlFormField = '<span class="survey-content-field subtype-' + field.SubTypeID + '">';
                                if (field.SubTypeID == 7 ){
                                    htmlFormField += '<span class="publish ' + (field.Publish ? 'published' : 'unpublished') + '" data-transactionid="' + id + '" data-fieldguid="' + field.FieldGuid + '"></span>';
                                }
                                htmlFormField += '<span class="survey-content-field-name">' + field.FieldName + '</span><span class="survey-content-field-value">';
                                if (field.SubTypeID == 2) {
                                    for (var i = 1; i <= 5; i++) {
                                        if (i <= parseInt(field.Value)) {
                                            htmlFormField += '<span class="rating-circle full"></span>';
                                        } else {
                                            htmlFormField += '<span class="rating-circle"></span>';
                                        }
                                    }
                                } else {
                                    if (field.Value != null && !isNaN(field.Value)) {
                                        if (field.Value.toString().indexOf('.') >= 0) {
                                            htmlFormField += field.Value.toString().substr(0, field.Value.toString().indexOf('.') + 2);
                                        } else {
                                            htmlFormField += field.Value;
                                        }
                                    } else {
                                        htmlFormField += field.Value;
                                    }
                                }
                                htmlFormField += '</span></span>';
                                $('.survey-content-formfields').append(htmlFormField);

                                //clic en publish
                                $('.publish').off('click').on('click', function () {
                                    var dataObject = {
                                        transactionID: $(this).attr('data-transactionid'),
                                        fieldGuid: $(this).attr('data-fieldguid')
                                    }
                                    $.post('/settings/surveys/PublishField', dataObject, function (data) {
                                        if (data.ResponseType == 1) {
                                            if (data.Published) {
                                                $('span[data-fieldguid="' + dataObject.fieldGuid + '"]').removeClass('unpublished').addClass('published');
                                                UI.messageBox(1, "Comment was published.");
                                            } else {
                                                $('span[data-fieldguid="' + dataObject.fieldGuid + '"]').removeClass('published').addClass('unpublished');
                                                UI.messageBox(1, "Comment was unpublished.");
                                            }
                                        } else {
                                            UI.messageBox(-1, "There was a problem trying to publish, try again please.");
                                        }
                                    },'json');
                                });
                            }
                        }
                    });
                }
            });

            $('#divSurveyDetailsSlider').animate({
                'marginLeft': '-100%'
            });
        });
    }

    function getFieldIDByName(fieldname) {

    }

    return {
        init: init,
        Structure: Structure,
        Stats: Stats,
        Referrals: Referrals,
        renderObject: renderObject,
        addInfoField: addInfoField,
        addFormField: addFormField,
        loadSurveyObject: loadSurveyObject,
        renderFormFields: renderFormFields,
        clearFormField: clearFormField,
        saved: saved,
        searchResults: searchResults,
        renderStatsFilters: renderStatsFilters,
        getStatsSearchParams: getStatsSearchParams,
        showList: showList
    }
}();

$(function () {
    Survey.init();
});