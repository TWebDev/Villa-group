var MultipleReviews = function () {
    var ratings = [Language.I_hate_it, Language.I_dont_like_it, Language.Its_okay, Language.I_like_it, Language.I_love_it];
    var transactionID = "";

    var init = function () {
        var url = window.location.href;
        surveyid = "03ba7276-5a77-4349-aaf6-641bd425ae44";
        purchaseid = url.substr(url.lastIndexOf('/') + 1);

        //verificar que se pueda contestar
        var dataObject = {
            purchaseid: purchaseid
        }
        $.getJSON('/survey/GetTransactionID/' + surveyid, dataObject, function (data) {
            if (!data.Status) {
                $('#blocked').text(data.Message);
                $('#blocked').fadeIn('fast');
            } else {
                MultipleReviews.transactionID = data.TransactionID
                $('.div-submit').slideDown('fast');
            }
        });

        $('.answer-button').on('click touchstart', function (event) {
            var value = $(this).attr('data-value');
            var fullStar = '/Content/themes/mex/images/icon_star_full.png';
            var emptyStar = '/Content/themes/mex/images/icon_star_empty.png';
            $(this).siblings('.answer-button').each(function (i) {
                if (!isNaN($(this).attr('data-value')) && parseInt($(this).attr('data-value')) <= value) {
                    $(this).attr('src', fullStar);
                } else {
                    $(this).attr('src', emptyStar);
                }
            });
            $(this).attr('src', fullStar);
            $(this).siblings('[type=hidden]').val(value);
            $(this).siblings('.rating-message').text(ratings[value - 1]);
            event.stopPropagation();
            event.preventDefault();
        });

        $('.hdnRating').each(function () {
            if (parseInt($(this).val()) > 0) {
                var purchaseServiceID = $(this).parents('.review').attr('id');
                var index = parseInt($(this).val()) - 1;
                $('#' + purchaseServiceID + ' .answer-button').eq(index).trigger('click');
                $(this).siblings().off('click');
            }
        });

        $('.txtReview').on('keyup', function () {
            validateReviewForm($(this).parents('.review').attr('id'));
        });

        if (window.FormData !== undefined) {
            $('.upload').slideDown('fast');
            $('.upload-label').slideDown('fast');
        }

        $('.fupPicture').on('change', function () {
            if ($(this).val() != "") {
                var purchaseServiceID = $(this).parents('.review').attr('id');
                $(this).siblings('.upload-status').text(Language.Uploading_File);
                $(this).parents('.upload').addClass('selected');
                //enviar archivo
                // Checking whether FormData is available in browser  
                if (window.FormData !== undefined) {

                    var fileUpload = $(this).get(0);
                    var files = fileUpload.files;

                    // Create FormData object  
                    var fileData = new FormData();

                    // Looping over all files and add it to FormData object  
                    for (var i = 0; i < files.length; i++) {
                        fileData.append(files[i].name, files[i]);
                    }

                    // Adding one more key to FormData object  
                    //fileData.append('username', ‘Manas’);  

                    $('#' + purchaseServiceID + ' .btnSubmit').val(Language.Wait_Please).attr('disabled', 'disabled');
                    $.ajax({
                        url: '/Controls/UploadReviewPicture',
                        type: "POST",
                        contentType: false, // Not to set any content header  
                        processData: false, // Not to process data  
                        data: fileData,
                        success: function (result) {
                            $('#' + purchaseServiceID + ' .upload').css('background-image', 'url(\'/content/themes/base/images/reviews/' + result.FileName + '?w=314\')');
                            //$('#' + purchaseServiceID + ' .upload').html('');
                            console.log('#' + purchaseServiceID + ' .upload-status')
                            $('#' + purchaseServiceID + ' .upload-status').hide();
                            $('#' + purchaseServiceID + ' .fupPicture').hide();
                            $('#' + purchaseServiceID + ' .upload-close').show();

                            $('#' + purchaseServiceID + ' .hdnPicture').val('/content/themes/base/images/reviews/' + result.FileName);
                            $('#' + purchaseServiceID + ' .btnSubmit').val(Language.Submit).attr('disabled', null);
                        },
                        error: function (err) {
                            console.log(err.statusText);
                        }
                    });
                } /*else {  
                        alert("FormData is not supported.");  
                    }  */
            }
        });

        $('.upload-close').on('click', function () {
            $(this).parent().css('background-image', 'none').removeClass('selected');
            $(this).siblings('.upload-status').text(Language.Select_your_File);
            $(this).siblings().fadeIn('fast');
            $(this).fadeOut('fast');
        });

        $('.btnSubmit').off('click').on('click', function () {
            var purchaseServiceID = $(this).parents('.review').attr('id');
            if (validateReviewForm(purchaseServiceID)) {
                $('#' + purchaseServiceID + ' .btnSubmit').slideUp('fast');
                $('#' + purchaseServiceID + ' .interaction-message').text("Wait please...").slideDown('fast');
                var model = {
                    PurchaseServiceID: purchaseServiceID,
                    ServiceID: $('#' + purchaseServiceID + ' .hdnServiceID').val(),
                    Rating: $('#' + purchaseServiceID + ' .hdnRating').val(),
                    Review: $('#' + purchaseServiceID + ' .txtReview').val(),
                    Picture: $('#' + purchaseServiceID + ' .hdnPicture').val()
                }
                $.post('/controls/PurchaseReviewSave', model, function (data) {
                    if (data.ResponseType == 1) {
                        //ok
                        $('#' + purchaseServiceID + ' .txtReview').attr('disabled', 'disabled');
                    } else {
                        $('#' + purchaseServiceID + ' .btnSubmit').slideDown('fast');
                    }
                    $('#' + purchaseServiceID + ' .interaction-message').text(data.ResponseMessage).slideDown('fast');
                }, 'json');
            }
        });

        $('.btnSubmitBookingExperience').off('click').on('click', function () {
            var purchaseID = $(this).parents('.col-left').attr('id');
            if (validateReviewForm(purchaseID)) {
                $('#' + purchaseID + ' .btnSubmitBookingExperience').slideUp('fast');
                $('#' + purchaseID + ' .interaction-message').text("Wait please...").slideDown('fast');
                var model = {
                    PurchaseID: purchaseID,
                    Rating: $('#' + purchaseID + ' .hdnRating').val(),
                    Review: $('#' + purchaseID + ' .txtReview').val(),
                    TransactionID: MultipleReviews.transactionID
                }
                $.post('/controls/BookingExperienceSave', model, function (data) {
                    if (data.ResponseType == 1) {
                        //ok
                        $('#' + purchaseID + ' .txtReview').attr('disabled', 'disabled');
                    } else {
                        $('#' + purchaseID + ' .btnSubmitBookingExperience').slideDown('fast');
                    }
                    $('#' + purchaseID + ' .interaction-message').text(data.ResponseMessage).slideDown('fast');
                }, 'json');
            }
        });
    }

    function validateReviewForm(purchaseServiceID) {
        var errorMessage = '';
        var valid = true;

        //console.log(purchaseServiceID);
        if ($('#' + purchaseServiceID + ' .hdnRating').val() == '0') {
            valid = false;
            errorMessage = "Please rate your experience."
        }

        //if (valid && $('#' + purchaseServiceID + ' .txtReview').val() == '') {
        //    valid = false;
        //    errorMessage = "Please write some comments."
        //}
        if (!valid) {
            $('#' + purchaseServiceID + ' .interaction-message').text(errorMessage).slideDown('fast');
        } else {
            $('#' + purchaseServiceID + ' .interaction-message').text('').slideUp('fast');
        }
        return valid;
    }

    return {
        init: init,
        transactionID: transactionID
    }
}();

MultipleReviews.init();