$(function () {

});

var INTERACTION = function () {

    var init = function () {

    }

    var getInteractions = function (leadID) {
        $('#divPreArrivalInteractions').html('');
        $.ajax({
            url: '/PreArrival/GetInteractions',
            cache: false,
            type: 'POST',
            data: { leadID: leadID },
            success: function (data) {
                INTERACTION.renderInteractions(data);
            }
        });
    }

    var renderInteractions = function (data) {
        $('#divPreArrivalInteractions').html('');
        var _str = '';
        var interactionCounter = 0;
        $.each(data, function (index, item) {
            if (item.InteractionsInfo_ParentInteraction == null || item.InteractionsInfo_ParentInteraction == '') {
                interactionCounter++;
                if (interactionCounter == 3) {
                    _str += '<a class="interaction-anchor to-hide">show _counter more interactions</a>';
                    _str += '<div class="interactions-wrapper" style="display:none;">';
                }
                _str += '<div id="interaction_' + item.InteractionsInfo_InteractionID + '" class="interaction-wrapper">'
                    + '<div class="" style="width:200px;display:inline-block;">'
                    + '<div class="full-width"><i class="material-icons">' + getIconCodeFromText($('#InteractionsInfo_InteractionType option[value="' + item.InteractionsInfo_InteractionType + '"]').text()) + '</i></div>'
                    + '<div class="full-width">' + (item.InteractionsInfo_BookingStatus != null && item.InteractionsInfo_BookingStatus != '' ? $('#InteractionsInfo_BookingStatus option[value="' + item.InteractionsInfo_BookingStatus + '"]').text() : '') + '</div>'
                    + '</div>'
                    + '<div class="right" style="width:500px;">'
                    + '<span class="comments-wrapper">' + item.InteractionsInfo_InteractionComments + '</span>'
                    + '</div>'
                    + '<div class="full-width">'
                    + '<div class="full-width text-left">' + $('#InteractionsInfo_InteractedWithUser option[value="' + item.InteractionsInfo_InteractedWithUser + '"]').text() + '</div>'
                    + '<div class="full-width text-left">' + item.InteractionsInfo_TotalSold + '</div>'//new line
                    + '<div class="text-right text-italic">saved by ' + item.InteractionsInfo_SavedByUserName + ' on ' + item.InteractionsInfo_DateSaved
                    + '<i class="material-icons align-from-left" title="create notification">add_alarm</i><i class="material-icons align-from-left add-comment" title="comment interaction">mode_comment</i>'
                    + '</div>'
                    + '</div>'
                    + '</div>';

                var _counter = 0;
                _str += '<a class="reply-anchor to-hide">show _counter replies</a>';
                $.each(data, function (index, i) {
                    if (item.InteractionsInfo_InteractionID == i.InteractionsInfo_ParentInteraction) {
                        _counter++;
                        _str += '<div class="reply-list" style="display:none">';
                        _str += '<div id="interaction_' + i.InteractionsInfo_InteractionID + '" class="interaction-wrapper comments-wrapper reply-wrapper">'
                            + '<div>' + i.InteractionsInfo_InteractionComments + '</div>'
                            //+ '<div class="text-italic text-right" style="margin-top:15px;">' + $('#InteractionsInfo_InteractedWithUser option[value="' + i.InteractionsInfo_SavedByUser + '"]').text() + ' on ' + i.InteractionsInfo_DateSaved + '</div></div>';
                            + '<div class="text-italic text-right" style="margin-top:15px;">' + i.InteractionsInfo_SavedByUserName + ' on ' + i.InteractionsInfo_DateSaved + '</div></div>';
                        _str += '</div>';
                    }
                });
                if (_counter > 0) {
                    _str += '<a class="reply-anchor to-show" style="display:none;">hide replies</a>';
                    _str = _str.replace('<a class="reply-anchor to-hide">show _counter replies</a>', '<a class="reply-anchor to-hide">show ' + _counter + ' replies</a>');
                }
                else {
                    _str = _str.replace('<a class="reply-anchor to-hide">show _counter replies</a>', '');
                }
            }
        });
        if (interactionCounter > 2) {
            var _result = (parseInt(interactionCounter) - 2);
            _str += '</div>';
            _str += '<a class="interaction-anchor to-show" style="display:none;">hide interaction' + (_result > 1 ? 's' : '') + '</a>';
            _str = _str.replace('show _counter more interactions', 'show ' + _result + ' more interaction' + (_result > 1 ? 's' : ''));
        }
        $('#divPreArrivalInteractions').html(_str);
        toggleComments();
        INTERACTION.commentInteraction();
    }

    function toggleComments() {
        $('.reply-anchor').unbind('click').on('click', function (e) {
            if ($(this).hasClass('to-hide')) {
                $(this).nextUntil('.reply-anchor.to-show').toggle('fast');
                //$(this).siblings('.reply-anchor.to-show').show();
                $(this).nextAll('.reply-anchor.to-show:first').show();
            }
            else {
                $(this).prevUntil('.reply-anchor.to-hide').toggle();
                //$(this).siblings('.reply-anchor.to-hide').show();
                $(this).prevAll('.reply-anchor.to-hide:first').show();
            }
            $(this).hide();
        });
        $('.interaction-anchor').unbind('click').on('click', function (e) {
            if ($(this).hasClass('to-hide')) {
                $(this).nextUntil('.interaction-anchor.to-show').toggle();
                //$(this).hide();
                $(this).siblings('.interaction-anchor.to-show').show();
            }
            else {
                $(this).prevUntil('.interaction-anchor.to-hide').toggle();
                //$(this).hide();
                $(this).siblings('.interaction-anchor.to-hide').show();
            }
            $(this).hide();
        });
    }

    function getIconCodeFromText(text) {
        switch (text) {
            case 'Phone Call': {
                return 'phone';
                break;
            }
            case 'Email': {
                return 'email';
                break;
            }
            case 'Live Chat': {
                return 'chat';
                break;
            }
            case 'Note': {
                return 'comment';
                break;
            }

        }
    }

    var saveInteractionSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {

            var _str = '<div id="interaction_' + data.ItemID.itemID + '" class="interaction-wrapper">'
                + '<div class="" style="width:200px;display:inline-block;">'
                + '<div class="full-width"><i class="material-icons">' + getIconCodeFromText($('#InteractionsInfo_InteractionType option:selected').text()) + '</i></div>'
                + '<div class="full-width">' + $('#InteractionsInfo_BookingStatus option:selected').text() + '</div>'
                + '</div>'
                + '<div class="right" style="width:500px;">'
                + '<span class="comments-wrapper">' + $('#InteractionsInfo_InteractionComments').val() + '</span>'
                + '</div>'
                + '<div class="full-width">'
                + '<div class="full-width text-left">' + $('#InteractionsInfo_InteractedWithUser option:selected').text() + '</div>'
                + '<div class="full-width text-left">' + parseFloat($('#InteractionsInfo_TotalSold').val()).formatMoney(2) + '</div>'
                + '<div class="text-right text-italic">saved by ' + $('#ufirstname').val() + ' ' + $('#ulastname').val() + ' on ' + data.ItemID.dateSaved
                + '<i class="material-icons align-from-left" title="create notification">add_alarm</i><i class="material-icons align-from-left add-comment" title="comment interaction">mode_comment</i>'
                + '</div>'
                + '</div>'
                + '</div>';

            if ($('#divPreArrivalInteractions').children('.interaction-wrapper').length > 1) {
                if ($('.interactions-wrapper').length == 0) {
                    var newStr = '<a class="interaction-anchor to-hide">show _counter more interactions</a>';
                    newStr += '<div class="interactions-wrapper" style="display:none;"></div>';
                    newStr += '<a class="interaction-anchor to-show" style="display:none;">hide interactions</a>';
                    $('#divPreArrivalInteractions').append(newStr);

                }

                $('#divPreArrivalInteractions').children('.interaction-wrapper').last().prev().nextUntil('.interaction-anchor.to-hide').prependTo($('.interactions-wrapper'));
            }
            var _length = $('.interactions-wrapper').children('.interaction-wrapper').length;
            $('.interaction-anchor.to-hide').text('show ' + _length + ' more ' + (_length > 1 ? 'interactions' : 'interaction'));
            $('#divPreArrivalInteractions').prepend(_str);
            $('#frmInteraction').clearForm();
            toggleComments();
            INTERACTION.commentInteraction();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var commentInteraction = function () {
        $('.add-comment').unbind('click').on('click', function (e) {
            var _str = '<div class="interaction-wrapper comments-wrapper reply-wrapper"><input type="text" class="comment-reply double-width" /><i class="material-icons right clear-reply align-from-left">clear</i><i class="material-icons right send-reply">send</i></div>';
            $(this).parents('.interaction-wrapper').after(_str);
            INTERACTION.replyActions();
        });
    }

    var replyActions = function () {
        $('.clear-reply').unbind('click').on('click', function () {
            $(this).parent('.reply-wrapper').remove();
        });

        $('.send-reply').unbind('click').on('click', function (e) {
            var interactionID = $(e.target).parent('.reply-wrapper').prev('.interaction-wrapper').attr('id').split('_')[1];
            $.ajax({
                url: '/PreArrival/SaveInteractionReply',
                cache: false,
                type: 'POST',
                data: { interactionID: interactionID, reply: $(e.target).prevAll('.comment-reply').val() },
                success: function (data) {
                    var _reply = $(e.target).prevAll('.comment-reply').val();

                    var _str = '<div class="reply-list"><div id="interaction_' + data.ItemID.itemID + '" class="interaction-wrapper comments-wrapper reply-wrapper"><div>' + _reply + '</div><div class="text-italic text-right">' + $('#ufirstname').val() + ' ' + $('#ulastname').val() + ' on ' + data.ItemID.dateSaved + '</div></div></div>';
                    if ($(e.target).parent('.interaction-wrapper').next('.reply-anchor').is(':visible')) {//anchor to show replies
                        $(e.target).parent('.interaction-wrapper').next('.reply-anchor').trigger('click');
                    }
                    else {//it not exist yet, or replies are already expanded
                        if ($(e.target).parent('.interaction-wrapper').next('.reply-anchor').length == 0) {
                            $(e.target).parent('.interaction-wrapper').after('<a class="reply-anchor to-show">hide replies</a>');
                            $(e.target).parent('.interaction-wrapper').after('<a class="reply-anchor to-hide" style="display:none">show 0 replies</a>');
                            toggleComments();
                        }
                    }
                    $(e.target).parent('.interaction-wrapper').next('.reply-anchor').after(_str);
                    var _anchorText = $(e.target).parent('.interaction-wrapper').next('.reply-anchor').text().split(' ');
                    $(e.target).parent('.interaction-wrapper').next('.reply-anchor').text(_anchorText[0] + ' ' + (parseInt(_anchorText[1]) + 1) + ' ' + _anchorText[2]);
                    $('.clear-reply').trigger('click');

                    //var _reply = $(e.target).prevAll('.comment-reply').val();
                    //var _str = '<div class="reply-list"><div id="interaction_' + data.ItemID.itemID + '" class="interaction-wrapper comments-wrapper reply-wrapper"><div>' + _reply + '</div><div class="text-italic text-right">' + $('#ufirstname').val() + ' ' + $('#ulastname').val() + ' on ' + data.ItemID.dateSaved + '</div></div></div>';
                    //if ($(e.target).parent('.interaction-wrapper').next('.reply-anchor').is(':visible')) {
                    //    $(e.target).parent('.interaction-wrapper').next('.reply-anchor').trigger('click');
                    //}
                    //$(e.target).parent('.interaction-wrapper').next('.reply-anchor').after(_str);
                    //var _anchorText = $(e.target).parent('.interaction-wrapper').next('.reply-anchor').text().split(' ');
                    //$(e.target).parent('.interaction-wrapper').next('.reply-anchor').text(_anchorText[0] + ' ' + (parseInt(_anchorText[1]) + 1) + ' ' + _anchorText[2]);
                    //$('.clear-reply').trigger('click');
                }
            });
        });
    }

    return {
        init: init,
        getInteractions: getInteractions,
        saveInteractionSuccess: saveInteractionSuccess,
        commentInteraction: commentInteraction,
        replyActions: replyActions,
        renderInteractions: renderInteractions
    }
}();