var ip = '';
$.fn.vgSign = function (input) {
    return this.each(function () {
        var id = this.id,
            msg = 'By checking this box I understand and agree with the Terms and Conditions and/or the Cancelation Policy as stated in this document.';
        if (input != undefined) {
            if (input.msg != '' && input.msg != undefined) {
                //msg = input.message;
            }
        }

        var transactionID = '';
        if (location.search != '') {
            var params = location.search.substr(1).split('&');
            for (var i = 0; i < params.length; i++) {
                var param = params[i].split('=');
                if (param[0] == 'transaction') {
                    transactionID = param[1];
                }
            }
        }

        //var htmlElements = '<div style="height:340px;">'
        var htmlElements = '<div>'
        + '<span id="vgsignature_clear" onclick="Sign.clear();" style="border: 1px solid #E3E3E3;border-radius: 2px 2px 2px 2px;color: #0D7DE4;cursor: pointer; padding: 7px;position: absolute; margin-top: 37px; z-index: 2;">Clear</span>'
        + '<span id="vgsignature_styles" style="margin-top: 29px; position: absolute; z-index: 2; display:none;">'
        + '<span style="display:inline-block; vertical-align:top;"><br><input type="text" placeholder="type your name" id="vgsignature_name" value="" style="border: 1px solid #DDDDDD; height: 21px;" onkeyup="Sign.drawStyle()" /></span>'
        + '<span style="display: inline-block; margin-left: 5px; vertical-align: top;">Select your Style<br><span id="vgsignature_style" style="border: 1px solid #DDDDDD; display: block; height: 23px; overflow: hidden; background-color:white; cursor: default;">'
        + '<span style="color: #CCCCCC; float: right; margin-right: 5px; margin-top: -2px;">▼</span>'
        + '<ul id="vgsignature_style_ul" style="margin: 0; padding: 0 0 0 5px;">'
        + '<li id="vgsignature_style_1" onclick="Sign.selectStyle(1)" style="font-family: \'Just Another Hand\'; font-size: 13pt;">Style 1</li>'
        + '<li id="vgsignature_style_2" onclick="Sign.selectStyle(2)" style="font-family: \'Sue Ellen Francisco\'; font-size: 11pt;">Style 2</li>'
        + '<li id="vgsignature_style_3" onclick="Sign.selectStyle(3)" style="font-family: \'Reenie Beanie\'; font-size: 19pt;">Style 3</li>'
        + '<li id="vgsignature_style_4" onclick="Sign.selectStyle(4)" style="font-family: \'Alex Brush\'; font-size: 17pt;">Style 4</li>'
        + '<li id="vgsignature_style_5" onclick="Sign.selectStyle(5)" style="font-family: \'Shadows Into Light\'; font-size: 13pt;">Style 5</li>'
        + '<li id="vgsignature_style_6" onclick="Sign.selectStyle(6)" style="font-family: \'Loved by the King\'; font-size: 13pt;">Style 6</li>'
        + '<li id="vgsignature_style_7" onclick="Sign.selectStyle(7)" style="font-family: \'Annie Use Your Telescope\'; font-size: 15pt;">Style 7</li>'
        + '<li id="vgsignature_style_8" onclick="Sign.selectStyle(8)" style="font-family: \'Dawning of a New Day\'; font-size: 13pt;">Style 8</li>'
        + '<li id="vgsignature_style_9" onclick="Sign.selectStyle(9)" style="font-family: \'Architects Daughter\'; font-size: 15pt;">Style 9</li>'
        + '</ul></span></span>'
        + '</span>'
        + '<span style="font-weight:bold;" id="vgsignature_title">'
        + '<span class="d-inline d-md-none vgsignature-opt1" style="background-color: white; border-left: 1px dashed #DDDDDD; border-right: 1px dashed #DDDDDD; border-top: 1px dashed #DDDDDD; margin-right: 2px; padding: 10px; position: relative; z-index: 2;">Draw</span>'
        + '<span class="d-none d-md-inline vgsignature-opt1" style="background-color: white; border-left: 1px dashed #DDDDDD; border-right: 1px dashed #DDDDDD; border-top: 1px dashed #DDDDDD; margin-right: 2px; padding: 10px; position: relative; z-index: 2;">Draw your Signature</span>'
        + '<span class="d-inline d-md-none vgsignature-opt2" style="background-color: #EEEEEE; color: #0D7DE4; cursor: pointer; padding: 9px; position: relative; z-index: 2;">Write</span>'
        + '<span class="d-none d-md-inline vgsignature-opt2" style="background-color: #EEEEEE; color: #0D7DE4; cursor: pointer; padding: 9px; position: relative; z-index: 2;">Select a Signature Style</span>'
        + '</span><br>'
        //+ '<canvas id="vgsignature" height="150" width="' + ($('#paperSheet').width() - $('#signature').position().left) + '" style="border: dashed #ddd 1px; position:relative; cursor:crosshair; margin-top:5px; margin-bottom:5px;">'
        //+ '<canvas id="vgsignature" height="150" width="' + ($('#paperSheet').width() - ($('#signature').position().left/2)) + '" style="border: dashed #ddd 1px; position:relative; cursor:crosshair; margin-top:5px; margin-bottom:5px;">'
        + '<canvas id="vgsignature" class="mw-100" height="150" style="border: dashed #ddd 1px; position:relative; cursor:crosshair; margin-top:5px; margin-bottom:5px;">'
        + 'Your browser does not support the technology to sign this document.'
        + '<br>Please update your browser version or use a newer browser such as:<br>'
        + '<ul><li>Internet Explorer 9 +</li><li>Firefox 2.0 +</li><li>Safari 3.1 +</li><li>Chrome 4.0 +</li><li>Opera 9.0 +</li></ul>.'
        + '</canvas><br>'
        //+ '<div id="vgsignature_msg" style="width:570px;">'
        //+ '<div style="width:' + ($('#paperSheet').width() - $('#signature').position().left) + 'px;">'
        + '<div class="col-auto d-block">'
        + '<div id="vgsignature_msg">'
        + '<input type="checkbox" id="chkAccept" onchange="Sign.init();" />' + msg + '</div><br>'
        + '<input type="button" class="btn btn-primary" id="vgsignature_save" onclick="Sign.sign(\'' + transactionID + '\');" value="Send" />'
        + '<div class="alert alert-warning mt-3" id="spnPleaseAccept">Please accept using the check box above.</div>'
        + '</div>'
        + '</div>';

        if (transactionID != '') {
            //check if already signed
            if (transactionID.length == 36) {
                $.ajax({
                    url: '/Public/GetSignature',
                    cache: false,
                    type: 'POST',
                    data: { transaction: transactionID },
                    success: function (data) {
                        if (data != null && data != '') {
                            ip = data.IP;
                            if (data.signed) {
                                var sign = data.signature;
                                //var html = '<div style="width:570px;"></div><br><span style="font-weight:bold;" id="vgsignature_title">This document is already signed</span><br><img id="vgsignature_img" style="margin-top:5px;border:dashed #ddd 1px;" />';
                                var html = '<div></div><br><span class="font-weight-bold" id="vgsignature_title">This document is already signed</span><br><img id="vgsignature_img" class="img-fluid border" />';
                                $('#' + id).html(html);
                                $('#' + id).append('<br><input type="checkbox" id="chkAccept" checked="checked" disabled="disabled" />' + msg + '</div>');

                                var imgElement = document.getElementById('vgsignature_img');
                                imgElement.src = sign;
                            }
                            else {
                                $('#' + id).html(htmlElements);
                                //$('#vgsignature_opt1').click(Sign.optionDraw);
                                //$('#vgsignature_opt2').click(Sign.optionStyle);
                                $('.vgsignature-opt1').click(Sign.optionDraw);
                                $('.vgsignature-opt2').click(Sign.optionStyle);
                                Sign.init();
                            }
                        }
                    }
                });
            }
            else {
                $('#' + id).html('transaction is not valid or is corrupted');
            }
        }
    });
};

var Sign = function () {
    var canvas;
    var initState;
    var context, selectedStyle = 1;

    var init = function () {
        Sign.canvas = document.getElementById('vgsignature');
        //Sign.initState = Sign.canvas.toDataURL();
        if (Sign.canvas.getContext) {
            resize();
            context = Sign.canvas.getContext("2d");
            var drawing = false;
            //attach the touchstart, touchmove, touchend event listeners.

            Sign.canvas.addEventListener('touchstart', Sign.draw, false);
            Sign.canvas.addEventListener('touchmove', Sign.draw, false);
            Sign.canvas.addEventListener('touchend', Sign.draw, false);
            Sign.canvas.addEventListener('mousedown', Sign.draw, false);
            Sign.canvas.addEventListener('mousemove', Sign.draw, false);
            Sign.canvas.addEventListener('mouseup', Sign.draw, false);

            context.font = "12px Verdana";
            //context.font = "0.7em Verdana";
            context.fillStyle = "#777777";
            context.fillText(ip, 20, 130);
            Sign.initState = Sign.canvas.toDataURL();
            var chkAcceptChecked = $('#chkAccept').is(':checked');
            if (chkAcceptChecked) {
                $('#vgsignature_save').prop('disabled', false);
                $('#spnPleaseAccept').hide();
            }
            else {
                $('#vgsignature_save').prop('disabled', true);
                $('#spnPleaseAccept').show();
            }

        } else {
            $('#vgsignature_title').hide();
            $('#vgsignature_save').hide();
            $('#vgsignature_clear').hide();
            $('#vgsignature_msg').hide();
        }
    }

    var optionDraw = function () {
        $('.vgsignature-opt1').css({
            'background-color': 'white',
            'border-left': '1px dashed #DDDDDD',
            'border-right': '1px dashed #DDDDDD',
            'border-top': '1px dashed #DDDDDD',
            'margin-right': '2px',
            'padding': '10px',
            'position': 'relative',
            'z-index': '2'
        });
        $('.vgsignature-opt2').css({
            'background-color': '#EEEEEE',
            'color': '#0D7DE4',
            'cursor': 'pointer',
            'padding': '9px',
            'position': 'relative',
            'z-index': '2'
        });
        $('#vgsignature_clear').show();
        $('#vgsignature_styles').hide();
        if (Sign.canvas.getContext) {
            clear();
            Sign.canvas.addEventListener('touchstart', Sign.draw, false);
            Sign.canvas.addEventListener('touchmove', Sign.draw, false);
            Sign.canvas.addEventListener('touchend', Sign.draw, false);
            Sign.canvas.addEventListener('mousedown', Sign.draw, false);
            Sign.canvas.addEventListener('mousemove', Sign.draw, false);
            Sign.canvas.addEventListener('mouseup', Sign.draw, false);
        }
    }

    var optionStyle = function () {
        $('.vgsignature-opt2').css({
            'background-color': 'white',
            'border-left': '1px dashed #DDDDDD',
            'border-right': '1px dashed #DDDDDD',
            'border-top': '1px dashed #DDDDDD',
            'margin-right': '2px',
            'padding': '10px',
            'position': 'relative',
            'z-index': '2'
        });
        $('.vgsignature-opt1').css({
            'background-color': '#EEEEEE',
            'color': '#0D7DE4',
            'cursor': 'pointer',
            'padding': '9px',
            'position': 'relative',
            'z-index': '2'
        });
        $('#vgsignature_clear').hide();
        $('#vgsignature_styles').show();
        if (Sign.canvas.getContext) {
            Sign.canvas.removeEventListener('touchstart', Sign.draw, false);
            Sign.canvas.removeEventListener('touchmove', Sign.draw, false);
            Sign.canvas.removeEventListener('touchend', Sign.draw, false);
            Sign.canvas.removeEventListener('mousedown', Sign.draw, false);
            Sign.canvas.removeEventListener('mousemove', Sign.draw, false);
            Sign.canvas.removeEventListener('mouseup', Sign.draw, false);
            $('#vgsignature_style').hover(
              function () {
                  $('#vgsignature_style_ul').animate({
                      marginTop: '0px'
                  }, 400);
                  $(this).animate({
                      height: '225px'
                  }, 400);
              },
              function () {
                  $('#vgsignature_style_ul').animate({
                      marginTop: getMarginForStyle()
                  }, 400);
                  $(this).animate({
                      height: '23px'
                  }, 400);
              }
            );
            drawStyle();
        }
    }

    var selectStyle = function (styleIndex) {
        selectedStyle = styleIndex;
        drawStyle();
        $('#vgsignature_style_ul').animate({
            marginTop: getMarginForStyle()
        }, 400);
        $('#vgsignature_style').animate({
            height: '23px'
        }, 400);
    }

    function getMarginForStyle() {
        var margin = '';
        switch (selectedStyle) {
            case 1:
                margin = '0px;';
                break;
            case 2:
                margin = '-28px';
                break;
            case 3:
                margin = '-53px';
                break;
            case 4:
                margin = '-76px';
                break;
            case 5:
                margin = '-103px';
                break;
            case 6:
                margin = '-127px';
                break;
            case 7:
                margin = '-152px';
                break;
            case 8:
                margin = '-177px';
                break;
            case 9:
                margin = '-203px';
                break;
        }
        return margin;
    }

    var drawStyle = function () {
        clear();
        var font = '';
        switch (selectedStyle) {
            case 1:
                font = 'Just Another Hand';
                break;
            case 2:
                font = 'Sue Ellen Francisco';
                break;
            case 3:
                font = 'Reenie Beanie';
                break;
            case 4:
                font = 'Alex Brush';
                break;
            case 5:
                font = 'Shadows Into Light';
                break;
            case 6:
                font = 'Loved by the King';
                break;
            case 7:
                font = 'Annie Use Your Telescope';
                break;
            case 8:
                font = 'Dawning of a New Day';
                break;
            case 9:
                font = 'Architects Daughter';
                break;
        }
        context.font = "50px \'" + font + "\'";
        context.fillStyle = "#000000";
        context.fillText($('#vgsignature_name').val(), 20, 100);
    }

    var drawer = {
        isDrawing: false,
        touchstart: function (coors) {
            context.beginPath();
            context.moveTo(coors.x, coors.y);
            this.isDrawing = true;
        },
        touchmove: function (coors) {
            if (this.isDrawing) {
                context.lineTo(coors.x, coors.y);
                context.stroke();
            }
        },
        touchend: function (coors) {
            if (this.isDrawing) {
                context.stroke();
                context.closePath();
                this.isDrawing = false;
            }
        },
        mousedown: function (coors) {
            context.beginPath();
            context.moveTo(coors.x, coors.y);
            this.isDrawing = true;
        },
        mousemove: function (coors) {
            if (this.isDrawing) {
                context.lineTo(coors.x, coors.y);
                context.stroke();
            }
        },
        mouseup: function (coors) {
            if (this.isDrawing) {
                this.mousemove(coors);
                this.isDrawing = false;
            }
        }
    }

    var saveSign = function (transactionID) {
        var myImage = Sign.canvas.toDataURL("image/png");
        //var actionData = {
        //    method: 'setSignature',
        //    signature: myImage,
        //    signatureid: idFirma,
        //    accept: $('#chkAccept').is(':checked')
        //}

        //enviar firma al server
        //$('#vgsignature_title').html('Saving your signature...');
        //$('#vgsignature_title').fadeIn('fast');
        //$('#vgsignature_save').hide();
        //$('#vgsignature_clear').hide();
        //$('#vgsignature_styles').hide();
        if (Sign.initState != Sign.canvas.toDataURL()) {
            $('#vgsignature_title').html('Saving your signature...');
            $('#vgsignature_title').fadeIn('fast');
            $('#vgsignature_save').hide();
            $('#vgsignature_clear').hide();
            $('#vgsignature_styles').hide();
            $.ajax({
                url: '/Public/SetSignature',
                cache: false,
                type: 'POST',
                data: { transaction: transactionID, signature: myImage },
                success: function (data) {
                    if (data) {
                        $('#vgsignature').before('<img id="vgsignature_img" class="img-fluid border" />');
                        var imageElement = document.getElementById("vgsignature_img");
                        imageElement.src = myImage;
                        $('#vgsignature').hide();
                        $('#vgsignature_title').fadeOut('fast', function () {
                            $('#vgsignature_title').html('Your signature is saved');
                            $('#vgsignature_title').fadeIn('fast');
                            $('#chkAccept').prop('disabled', true);
                        });
                    }
                    else {
                        alert('There was some problem saving your signature. Click the "Sign" button again, please.')
                        $('#vgsignature_save').show();
                    }
                }
            });
        }
        else {
            alert('There is not signature drawn. Please sign the document before sending.');
        }
    }

    var clear = function () {
        if (Sign.canvas) {
            context.clearRect(0, 0, Sign.canvas.width, Sign.canvas.height);
            var w = Sign.canvas.width;
            Sign.canvas.width = 1;
            Sign.canvas.width = w;
            context.beginPath();
            context.font = "12px Verdana";
            context.fillStyle = "#777777";
            context.fillText(ip, 20, 130);
            Sign.initState = Sign.canvas.toDataURL('image/png');
        }
    }

    var draw = function (event) {

        // get the touch coordinates
        var coors;
        if (event.type == "mousemove" || event.type == "mousedown" || event.type == "mouseup") {
            if ((event.layerX || event.layerX == 0) && navigator.appName != 'Microsoft Internet Explorer') { // Firefox || Webkit
                coors = {
                    x: event.layerX,
                    y: event.layerY
                }
            } else if (event.offsetX || event.offsetX == 0) { // Opera || IE
                coors = {
                    x: event.offsetX,
                    y: event.offsetY
                }
            }
        } else {
            event.preventDefault();
            var r = Sign.canvas.getBoundingClientRect();
            coors = {
                x: event.changedTouches[0].clientX - r.left,
                y: event.changedTouches[0].clientY - r.top
            };
        }
        // pass the coordinates to the appropriate handler
        drawer[event.type](coors);
    }

    return {
        sign: saveSign,
        clear: clear,
        draw: draw,
        init: init,
        optionStyle: optionStyle,
        optionDraw: optionDraw,
        drawStyle: drawStyle,
        selectStyle: selectStyle
    }
}();

function resize() {
    if ($("#vgsignature").length > 0) {
        var imgData = Sign.canvas.getContext('2d').getImageData(0, 0, Sign.canvas.width, Sign.canvas.height);
        $("#vgsignature").outerWidth($('.card-body').width() - Math.abs($("#vgsignature").outerWidth(true) - $("#vgsignature").outerWidth()));
        $("#vgsignature").attr('width', ($('.card-body').width() - Math.abs($("#vgsignature").outerWidth(true) - $("#vgsignature").outerWidth())));
        Sign.canvas.getContext('2d').putImageData(imgData, 0, 0);
        
    }
}

$(function () {
    //resize();
    $(window).on('resize', function () {
        resize();
        Sign.clear();
    });
});