var Survey = function () {
    var Structure;
    var SurveyValuesModel;
    var surveyid;
    var transactionid;

    var init = function () {
        var url = window.location.href;
        surveyid = url.substr(url.lastIndexOf('/') + 1, 36);
        transactionid = url.substr(url.lastIndexOf('/') + 38, 36);
        getSurveyForm(surveyid, transactionid);
    }

    function getSurveyValuesObject() {
        if (Survey.SurveyValuesModel == undefined) {
            Survey.SurveyValuesModel = {
                SurveyID: surveyid,
                TransactionID: transactionid,
                Fields: []
            };
        }
    }

    function getSurveyForm(surveyid, transactionid) {
        $.getJSON('/survey/GetSurvey/' + surveyid, null, function (data) {
            if (data.FieldGroupID != 0) {
                Survey.Structure = data;
                document.title = Survey.Structure.SurveyName;
                $('#SurveyName').text(Survey.Structure.SurveyName);
                $('#Instructions').text(Survey.Structure.Instructions);
                $('#pnlInfoFields').html('');
                $.each(Survey.Structure.InfoFields, function (i, field) {
                    var fieldStr = '';
                    if (field.I_Visibility == "1") {
                        fieldStr = '<span class="field-visible">'
                            + '<input type="hidden" value="' + field.I_FieldID + '" />'
                            + '<span class="field-question">' + field.I_Question + '</span>: '
                            + '<span class="field-name">$' + field.I_FieldName + '</span>'
                            + '</span>';
                    }
                    $('#pnlInfoFields').append(fieldStr);
                });

                $('#pnlFormFields').html('');
                $('#pnlFormFields').append(renderFormFields(Survey.Structure.FormFields));

                //revisar si se puede contestar
                loadSurveyInfo(surveyid, transactionid);

                $('input[data-format=phone]').off('keyup').on('keyup', function () {
                    var chars = $(this).val().replace(/\s+/g, '').split('');
                    var value = "";
                    //get number
                    for (var i = 0; i < chars.length; i++) {
                        if (!isNaN(chars[i])) {
                            value += chars[i];
                        }
                    }
                    //format number
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
                });

                $('.referral input').on('change', function () {
                    $(this).closest('.question').find('.field-value').val($.toJSON(getReferrals($(this).closest('.question').find('.field-value')))).trigger('change');
                });

                $('.field-value').on('change', function () {
                    //console.clear();
                    //actualizar promedios de grupo
                    $('#pnlFormFields > .subtype-1:not(:has(.subtype-1))').each(function (g, group) {
                        var tempid = $(this).attr('data-tempid')
                        $(this).find('>.field-value').val(getAverage(tempid));
                    });

                    $('.subtype-1 .subtype-1').each(function (g, group) {
                        var tempid = $(this).attr('data-tempid')
                        $(this).find('>.field-value').val(getAverage(tempid));
                    });

                    $('.subtype-1:has(.subtype-1)').each(function (g, group) {
                        var tempid = $(this).attr('data-tempid')
                        $(this).find('>.field-value').val(getAverage(tempid));
                    });

                    //actualizar objeto
                    getSurveyValuesObject();

                    var values = [];
                    $('.field-value').each(function (f, fvalue) {
                        var FieldValue = {
                            FieldID: $(this).attr('data-field-id'),
                            Value: $(this).val()
                        }
                        values.push(FieldValue);
                    });
                    //Value: ($(this).parent().hasClass('subtype-4') ? JSON.parse($(this).val()) : $(this).val())
                    Survey.SurveyValuesModel.Fields = values;
                    //console.log($.toJSON(Survey.SurveyValuesModel));
                    $('#model').val($.toJSON(Survey.SurveyValuesModel));
                });

                $('.field-value').eq(0).trigger('change');

                $('.answer-button').on('click touchstart', function (event) {
                    var value = $(this).attr('data-value');
                    var color = '#00AAA5';
                    $(this).siblings('.answer-button').each(function (i) {
                        if (!isNaN($(this).attr('data-value')) && parseInt($(this).attr('data-value')) < value) {
                            $(this).css('background-color', color);
                            $(this).css('color', 'white');
                        } else {
                            $(this).css('background-color', 'white');
                            $(this).css('color', 'black');
                        }
                    });
                    $(this).css('background-color', color);
                    $(this).css('color', 'white');
                    $(this).siblings('[type=hidden]').val(value).trigger('change');

                    //revisar si tiene una pregunta dependiente
                    if ($(this).parent().parent().hasClass('subtype-3')) {
                        var tempid = $(this).parent().parent().attr('data-tempid');
                        if ($('[data-visible-if="' + tempid + '"]').length > 0) {
                            if ($('[data-visible-if="' + tempid + '"]>.label').text().indexOf(value) >= 0) {
                                $('[data-visible-if="' + tempid + '"]').slideDown('fast');
                            } else {
                                $('[data-visible-if="' + tempid + '"]').slideUp('fast');
                            }
                        }
                    }

                    event.stopPropagation();
                    event.preventDefault();
                });

                $('#btnAddReferral').on('click touchstart', function (event) {
                    var referralTemplate = $('.referral')[0].innerHTML;
                    $('#btnAddReferral').before('<div class="referral"><div class="referral-close">x</div>' + referralTemplate + '</div>');
                    $('.referral-close').off('click touchstart').on('click touchstart', function (event) {
                        $(this).parent().slideUp('fast', function () { $(this).remove() });
                        event.stopPropagation();
                        event.preventDefault();
                    });
                    $('input[data-format=phone]').off('keyup').on('keyup', function () {
                        var chars = $(this).val().replace(/\s+/g, '').split('');
                        var value = "";
                        //get number
                        for (var i = 0; i < chars.length; i++) {
                            if (!isNaN(chars[i])) {
                                value += chars[i];
                            }
                        }
                        //format number
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
                    });
                    $('.referral input').off('change').on('change', function () {
                        $(this).closest('.question').find('.field-value').val($.toJSON(getReferrals($(this).closest('.question').find('.field-value')))).trigger('change');
                    });
                    event.stopPropagation();
                    event.preventDefault();
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
            } else {
                $('#blocked').html('<h2>Ups! it seems like survey URL is wrong.</h2><h4> If you copied the URL, copy and paste again please.</h4>');
                $('#blocked').fadeIn('fast');
            }
        });
    }

    function loadSurveyInfo(surveyid, transactionid) {
        var dataObject = {
            transactionid: transactionid
        }
        $.getJSON('/survey/GetSurveyInfo/' + surveyid, dataObject, function (data) {
            $.each(data.InfoFields, function (f, field) {
                $('.field-name').each(function () {
                    if ('$' + field.FieldName == $(this).text()) {
                        $(this).text(field.Value);
                    }
                });
            });
            $('.field-name').each(function () {
                if ($(this).text().indexOf("$") >= 0) {
                    $(this).text('');
                }
            });
            console.log(data.Status);
            if (!data.Status) {
                $('#blocked').text(data.Message);
                $('#blocked').fadeIn('fast');
            } else {
                console.log('mostrar');
                $('.div-submit').slideDown('fast');
            }
        });
    }

    function blockForm(msg) {
        $('#blocked').html(msg);
        $('#blocked').fadeIn('fast');
        $('#btnSubmit').remove();
    }

    function getAverage(answerGroup) {
        var average = 0;
        var sum = 0;
        var validAnswers = 0;

        var answers = $('[data-tempid="' + answerGroup + '"]>.field-children>.question.subtype-1>.field-value');
        answers = answers.add('[data-tempid="' + answerGroup + '"]>.field-children>.question.subtype-2>.answer>.field-value');
        answers.each(function () {
            if (parseFloat($(this).val()) > 0) {
                validAnswers++;
            }
            sum += parseFloat($(this).val());
        });
        if (sum > 0) {
            average = sum / validAnswers;
        }
        return average;
    }

    function getReferrals(obj) {
        var referrals = [];
        $(obj).parent().find('.referral').each(function () {
            referrals.push({
                Name: $(this).find('input[data-value=Name]').val(),
                Email: $(this).find('input[data-value=Email]').val(),
                Mobile: $(this).find('input[data-value=Mobile]').val(),
                HomePhone: $(this).find('input[data-value=HomePhone]').val()
            });
        });
        return referrals;
    }

    function renderFormFields(fieldsList) {
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
                    fieldStr += '<div class="question form-field-visible-if subtype-' + field.F_FieldSubTypeID + '" data-tempid="' + field.F_TemporalID + '" data-fieldname="' + field.F_FieldName + '" style="display:none;" data-visible-if="' + field.F_VisibleIf + '" >';
                    break;
            }
            fieldStr += '<span class="label">' + field.F_Question + '</span>';
            switch (parseInt(field.F_FieldSubTypeID)) {
                case 1: //Questions Group
                    fieldStr += '<span class="right">1 = least desirable :: 5 = most desirable</span><input type="hidden" class="field-value" data-field-id="' + field.F_FieldID + '" value="0" />';
                    break;
                case 2: //Rating Question
                    fieldStr += '<div class="answer"><div class="answer-button" data-value="1">1</div>'
                                + '<div class="answer-button" data-value="2">2</div>'
                                + '<div class="answer-button" data-value="3">3</div>'
                                + '<div class="answer-button" data-value="4">4</div>'
                                + '<div class="answer-button" data-value="5">5</div>'
                                + '<input type="hidden" class="field-value" data-field-id="' + field.F_FieldID + '" value="0"></div>';
                    break;
                case 3: //Yes or No Question
                    fieldStr += '<div class="answer">'
                        + '<div class="answer-button" data-value="Yes">Yes</div>'
                        + '<div class="answer-button" data-value="No">No</div>'
                        + '<input type="hidden" class="field-value" data-field-id="' + field.F_FieldID + '" value="0">'
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
                        + '</div>'
                        + '<input type="hidden" class="field-value" data-field-id="' + field.F_FieldID + '" value="[]" />';
                    break;
                case 5: //Options Question
                    fieldStr += '<div class="answer"><select class="field-value" data-field-id="' + field.F_FieldID + '">';
                    $.each(field.F_Options.split(','), function (o, option) {
                        fieldStr += '<option value="' + option + '">' + option + '</option>';
                    });
                    fieldStr += '</select></div>';
                    break;
                case 6: //Text Field Question
                    fieldStr += '<div class="answer"><span class="param"><input type="text" class="field-value" data-field-id="' + field.F_FieldID + '" /></span></div>';
                    break;
                case 7: //Text Area / Comments Field
                    fieldStr += '<div class="answer"><span class="param"><textarea class="field-value" data-field-id="' + field.F_FieldID + '"></textarea></span></div>';
                    break;
            }
            //F_Fields
            if (field.F_Fields.length > 0) {
                fieldStr += '<div class="field-children">' + renderFormFields(field.F_Fields) + '</div>';
            }

            fieldStr += '</div>';
        });
        return fieldStr;
    }

    var saved = function (data) {
        $('#blocked').text(data.ResponseMessage);
        $('#blocked').fadeIn('fast');
    }

    var saving = function () {
        $('#blocked').text("Saving your answers... Wait please...");
        $('#blocked').fadeIn('fast');
    }

    return {
        init: init,
        Structure: Structure,
        SurveyValuesModel: SurveyValuesModel,
        getSurveyForm: getSurveyForm,
        saved: saved,
        saving: saving
    }
}();

$(function () {
    Survey.init();
});