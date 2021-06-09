$(function () {
    PLACE.init();
});

var PLACE = function () {

    var oTable;

    var map, map2;
    var myLatLng;
    var markersArray = new Array();

    var init = function () {

        $('#PlaceInfo_CheckInTime').datetimepicker({
            timeFormat: 'HH:mm',
            timeOnly: true,
            stepMinute: 5
        });
        $('#PlaceInfo_CheckOutTime').datetimepicker({
            //timeFormat: 'HH:mm',
            timeOnly: true
            //stepMinute: 5
        });
        $('#PlaceDescription_ShortDescription').ckeditor();
        $('#PlaceDescription_FullDescription').ckeditor();
        $('#PlaceDescription_FAQ').ckeditor();
        $('#PlaceDescription_Amenities').ckeditor();
        $('#PlaceDescription_AllInclusive').ckeditor();
        $('#DestinationInfo_Latitude').numeric();
        $('#DestinationInfo_Longitude').numeric();
        $('#PlaceInfo_Latitude').numeric();
        $('#PlaceInfo_Longitude').numeric();
        $('#PlaceInfo_TaxesPercentage').numeric();
        $('#RoomTypeDescription_Description').ckeditor();

        if (!$.fn.DataTable.fnIsDataTable('tblSearchPlacesResults')) {
            PLACE.searchResultsTable($('#tblSearchPlacesResults'));
        }

        $('#fdsPlacesInfo').find('div').first().on('mouseover', function () {
            if (map == undefined) {
                initMap();
            }
        });

        $('#fdsDestinationsInfo').find('div').first().on('mouseover', function () {
            if (map2 == undefined) {
                initMap2();
            }
        });

        $('.search').unbind('click').bind('click', function (e) {
            $('#tblSearch' + $(e.target).attr('id').substr(9) + 'Results > tbody').remove();
            $.ajax({
                url: '/Places/SearchPlacesRelatedItems',
                cache: false,
                type: 'POST',
                data: { item: $(e.target).attr('id').substr(9), path: $(e.target).parents('form').first().find('input:text').first().val() },
                success: function (data) {
                    var builder = '';
                    $.each(data, function (index, item) {
                        builder += '<tr id="trResult' + item.ItemID + '"><td id="td' + item.ItemName + '">' + item.ItemName + '</td><td class="tds"><img id="delItem' + item.ItemID + '" src="/Content/themes/base/images/cross.png" class="right" ></td></tr>';
                    });
                    $('#tblSearch' + $(e.target).attr('id').substr(9) + 'Results').append(builder);
                    PLACE.placeItemSelection();
                },
                complete: function () {
                    UI.tablesStripedEffect();
                    UI.tablesHoverEffect();
                }
            });
        });

        $('#PlaceInfo_Destination').on('change', function (e, params) {
            var path = 'Zone';
            var path2 = 'TransportationZone';
            var zoneID = params != undefined ? params.infoZoneID : 0;
            var tZoneID = params != undefined ? params.infoTZoneID : 0;
            var id = $(e.target).val();
            $.getJSON('/Places/GetDDLData', { path: path, id: id }, function (data) {
                $('#PlaceInfo_Zone').fillSelect(data);
                if (zoneID != 0) {
                    $('#PlaceInfo_Zone option[value=' + params.infoZoneID + ']').attr('selected', true);
                }
            });
            $.getJSON('/Places/GetDDLData', { path: path2, id: id }, function (data) {
                $('#PlaceInfo_TransportationZone').fillSelect(data);
                if (tZoneID != 0) {
                    $('#PlaceInfo_TransportationZone option[value=' + params.infoTZoneID + ']').attr('selected', true);
                }
            });
            //PICTURE.loadPicturesTree();
        });

        $('#PlaceInfo_PlaceType').on('change', function (e, infoPClasificationID) {
            var path = 'PlaceClasification';
            var id = $(e.target).val();
            $.getJSON('/Places/GetDDLData', { path: path, id: id }, function (data) {
                $('#PlaceInfo_PlaceClasification').fillSelect(data);
                if (infoPClasificationID != 0) {
                    $('#PlaceInfo_PlaceClasification option[value=' + infoPClasificationID + ']').attr('selected', true);
                }
            });
        });

        $('#btnSavePlaceDescription').on('click', function () {
            var flag = true;
            $('#ulPlaceDescriptions li').each(function () {
                if ($(this).text() == $('#PlaceDescription_Culture option:selected').text() && !$(this).hasClass('selected-row'))
                    flag = false;
            });
            if (flag) {
                UI.ckeditorUpdateInstances('frmPlaceDescription');
                $('#frmPlaceDescription').data('validator').settings.ignore = '.ignore-validation';
                $('#frmPlaceDescription').submit();
            }
            else
                UI.messageBox(-1, "Description Language already exists", null, null);
        });

        $('#btnSaveRoomTypeDescription').on('click', function () {
            var flag = true;
            $('#ulRoomTypeDescriptions li').each(function () {
                if ($(this).text() == $('#RoomTypeDescription_Culture option:selected').text() && !$(this).hasClass('selected-row'))
                    flag = false;
            });
            if (flag) {
                UI.ckeditorUpdateInstances('frmRoomTypeDescription');
                $('#frmRoomTypeDescription').data('validator').settings.ignore = '.ignore-validation';
                $('#frmRoomTypeDescription').submit();
            }
            else
                UI.messageBox(-1, "Description Language already exists", null, null);
        });

        $('#PlaceInfo_CheckInTime').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#PlaceInfo_CheckOutTime').on('keypress', function (e) {
            e.preventDefault();
        });

        $('#btnAddTerminalToPlace').on('click', function () {
            var rows = $('#tblTerminalsPerPlace > tbody > tr').length;
            var builder = '<tr><td id="tdTerminal' + $('#PlaceInfo_DrpTerminal option:selected').val() + '">'
                + '<input type="hidden" val="' + $('#PlaceInfo_DrpTerminal option:selected').val() + '"/>'
                + $('#PlaceInfo_DrpTerminal option:selected').text()
                + '</td>'
                + '<td><img class="right" src="/Content/themes/base/images/cross.png"/></td>'
                + '</tr>';
            if ($('#PlaceInfo_DrpTerminal option:selected').val() != 0) {
                var counter = 0;
                for (var i = 0; i <= rows; i++) {
                    if ($('#PlaceInfo_DrpTerminal option:selected').text() != $('#tdTerminal' + $('#PlaceInfo_DrpTerminal option:selected').val()).text())
                        counter++;
                }
                if (counter == (rows + 1)) {
                    $('#tblTerminalsPerPlace tbody').append(builder);
                }
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                PLACE.makeTblTerminalsRowsRemovable();
            }
        });

        $('#btnSavePlace').on('click', function () {
            var terminals = new Array();
            $('#PlaceInfo_Terminal').empty();
            $('#tblTerminalsPerPlace tbody tr').each(function (index) {
                terminals[index] = $(this).children('td:nth-child(1)').attr('id').substr(10);
            });
            var message = terminals != '' ? terminals : '';
            $('#PlaceInfo_Terminal').val(message);
            $('#frmPlace').validate().settings.ignore = [];
            $('#frmPlace').submit();
        });
    }

    function initMap() {
        latLng = new google.maps.LatLng(20.679767, -105.254180);//Pto. Vallarta
        var myOptions = {
            zoom: 8,
            center: latLng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("gMap"), myOptions);
        moveMapMarker(latLng);
        new google.maps.event.addListener(map, 'click', function (e) {
            updateLatLng(e.latLng);
            moveMapMarker();
        });
    }
    function initMap2() {
        latLng = new google.maps.LatLng(20.679767, -105.254180);//Pto. Vallarta
        var myOptions = {
            zoom: 8,
            center: latLng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map2 = new google.maps.Map(document.getElementById("gMap2"), myOptions);
        moveMapMarker2(latLng);
        new google.maps.event.addListener(map2, 'click', function (e) {
            updateLatLng2(e.latLng);
            moveMapMarker2();
        });
    }
    function updateLatLng(latLng) {
        $('#PlaceInfo_Latitude').val(latLng.lat());
        $('#PlaceInfo_Longitude').val(latLng.lng());
    }
    function updateLatLng2(latLng) {
        $('#DestinationInfo_Latitude').val(latLng.lat());
        $('#DestinationInfo_Longitude').val(latLng.lng());
    }
    function moveMapMarker(latLng) {
        if (!latLng) {
            var latLng = new google.maps.LatLng($('#PlaceInfo_Latitude').val(), $('#PlaceInfo_Longitude').val());
            markersArray[markersArray.length - 1].setMap(null);
        }
        var marker = new google.maps.Marker({
            position: latLng,
            map: map,
            draggable: true,
            title: 'Drag marker to desired location'
        });
        markersArray.push(marker);
        new google.maps.event.addListener(marker, 'dragend', function (e) {
            updateLatLng(marker.getPosition());
        });
        map.panTo(latLng);
    }
    function moveMapMarker2(latLng) {
        if (!latLng) {
            var latLng = new google.maps.LatLng($('#DestinationInfo_Latitude').val(), $('#DestinationInfo_Longitude').val());
            markersArray[markersArray.length - 1].setMap(null);
        }
        var marker = new google.maps.Marker({
            position: latLng,
            map: map2,
            draggable: true,
            title: 'Drag marker to desired location'
        });
        markersArray.push(marker);
        new google.maps.event.addListener(marker, 'dragend', function (e) {
            updateLatLng2(marker.getPosition());
        });
        map2.panTo(latLng);
    }

    var savePlaceSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Place Saved') {
                var oSettings = PLACE.oTable.fnSettings();
                var iAdded = PLACE.oTable.fnAddData([
                    $('#PlaceInfo_Place').val(),
                    $('#PlaceInfo_PlaceLabel').val(),
                    $('#PlaceInfo_Destination option:selected').text(),
                    $('#PlaceInfo_Zone option:selected').text(),
                    $('#PlaceInfo_PlaceType optin:selected').text(),
                    '<img src="/Content/themes/base/images/cross.png" id="delP' + data.ItemID + '">'
                ]);
                var aRow = oSettings.aoData[iAdded[0]].nTr;
                aRow.setAttribute('id', 'trPlace' + data.ItemID);
                PLACE.oTable.fnDisplayRow(aRow);
                UI.tablesHoverEffect();
                PLACE.makeTableRowsSelectable();
                $('#trPlace' + data.ItemID).click();
                UI.expandFieldset('fdsPlaceDescriptions');
                UI.scrollTo('fdsPlaceDescriptions', null);
            }
            else {
                $('#trPlace' + data.ItemID).children('td:nth-child(1)').text($('#PlaceInfo_Place').val());
                $('#trPlace' + data.ItemID).children('td:nth-child(2)').text($('#PlaceInfo_PlaceLabel').val());
                $('#trPlace' + data.ItemID).children('td:nth-child(3)').text($('#PlaceInfo_Destination option:selected').text());
                $('#trPlace' + data.ItemID).children('td:nth-child(4)').text($('#PlaceInfo_Zone option:selected').text());
                $('#trPlace' + data.ItemID).children('td:nth-child(5)').text($('#PlaceInfo_PlaceType option:selected').text());
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var savePlaceDescriptionSuccess = function (data) {
        var duration = (data.ResponseType < 0) ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Place Description Saved') {
                var builder = '<li id="liDescription' + data.ItemID + '" >' + $('#PlaceDescription_Culture option:selected').text()
                    + '<img id="imDescription' + data.ItemID + '" src="/Content/themes/base/images/cross.png" class="right"></li>';
                $('#liNoDescriptions').remove();
                $('#ulPlaceDescriptions').append(builder);
                UI.ulsHoverEffect('ulPlaceDescriptions');
                $('#frmPlaceDescription').clearForm();
                $('#frmPlaceDescription').find('textarea').each(function () {
                    $(this).val('');
                    $(this).ckeditor();
                });
                PLACE.ulPlaceDescriptionsSelection();
            }
            else {
                $('#liDescription' + data.ItemID)[0].childNodes[0] = $('#PlaceDescription_Culture option:selected').text();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }




    var savePlacePlanTypeSuccess = function (data)
    {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage =='Plan Type Saved') {//condicion anterior if(data.ResponseMessage =='Place-Plan Type Saved')
                var builder = '<li id="liPlacePlanType' + data.ItemID + '">' + $('#PlacePlanType_PlanType option:selected').text()
                + '<img id="imPlacePlanType' + data.ItemID + '" src="/Content/themes/base/images/cross.png" class="right"></li>';
                $('#liNoPlacePlanTypes').remove();
                $('#ulPlacePlanTypes').append(builder);
                UI.ulsHoverEffect('ulPlacePlanTypes');
                $('#frmPlacePlanType').clearForm();
                PLACE.ulPlacePlanTypesSelection();
            }
            else
                $('#liPlacePlanType' + data.ItemID)[0].childNodes[0].textContent = $('#PlacePlanType_PlanType option:selected').text();
            
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }




    var saveRoomTypeSuccess = function (data) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Room Type Saved') {
                var builder = '<li id="liRoomType' + data.ItemID + '">' + $('#RoomTypeInfo_RoomType').val()
                + '<img id="imRoomType' + data.ItemID + '" src="/Content/themes/base/images/cross.png" class="right"></li>';
                $('#liNoRoomTypes').remove();
                $('#ulRoomTypes').append(builder);
                UI.ulsHoverEffect('ulRoomTypes');
                $('#frmRoomType').clearForm();
                PLACE.ulRoomTypesSelection();
                $('#liRoomType' + data.ItemID).click();
            }
            else {
                $('#liRoomType' + data.ItemID)[0].childNodes[0].textContent = $('#RoomTypeInfo_RoomType').val();
            }
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveRoomTypeDescriptionSuccess = function (data) {
        var duration = (data.ResponseType < 0) ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Room Type Description Saved') {
                var builder = '<li id="liRoomTypeDescription' + data.ItemID + '" >' + $('#RoomTypeDescription_Culture option:selected').text()
                    + '<img id="imRoomTypeDescription' + data.ItemID + '" src="/Content/themes/base/images/cross.png" class="right"></li>';
                $('#liNoRoomTypeDescriptions').remove();
                $('#ulRoomTypeDescriptions').append(builder);
                UI.ulsHoverEffect('ulRoomTypeDescriptions');
                $('#frmRoomTypeDescription').clearForm();
                $('#RoomTypeDescription_Description').val('');
                PLACE.ulRoomTypeDescriptionsSelection();
            }
            else
                $('#liRoomTypeDescription' + data.ItemID)[0].childNodes[0].textContent = $('#RoomTypeDescription_Culture option:selected').text();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveZoneSuccess = function (data) {
        var duration = (data.ResponseType < 0) ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Zone Saved') {
                var builder = '<tr id="trResult' + data.ItemID + '"><td>' + $('#ZoneInfo_Zone').val()
                    + '</td><td class="tds"><img id="delItem' + data.ItemID
                    + '" src="/Content/themes/base/images/cross.png" class="right"></td></tr>';
                $('#tblSearchZonesResults tbody').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                $('#frmZoneInfo').clearForm();
                PLACE.placeItemSelection();
                //revisar si funciona la seleccion de file despues de insercion
            }
            else {
                $('#trResult' + data.ItemID).children('td:nth-child(1)').text($('#ZoneInfo_Zone').val());
            }
            $.getJSON('/Places/GetDDLData', { path: 'Zone', id: $('#PlaceInfo_Destination option:selected').val() }, function (data) {
                $('#PlaceInfo_Zone').fillSelect(data);
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveDestinationSuccess = function (data) {
        var duration = (data.ResponseType > 0) ? duration = data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Destination Saved') {
                var builder = '<tr id="trResult' + data.ItemID + '"><td>' + $('#DestinationInfo_Destination').val()
                    + '</td><td class="tds"><img id="delItem' + data.ItemID
                    + '" src="/Content/themes/base/images/cross.png" class="right"></td></tr>';
                $('#tblSearchDestinationsResults tbody').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                $('#frmDestinationInfo').clearForm();
                PLACE.placeItemSelection();
            }
            else {
                $('#trResult' + data.ItemID).children('td:nth-child(1)').text($('#DestinationInfo_Destination').val());
            }
            $.getJSON('/Places/GetDDLData', { path: 'Destination', id: 0 }, function (data) {
                $('#PlaceInfo_Destination').fillSelect(data);
                $('#ZoneInfo_Destination').fillSelect(data);
                $('#TransportationZoneInfo_Destination').fillSelect(data);

            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var savePlaceClasificationSuccess = function (data) {
        var duration = (data.ResponseType > 0) ? duration = data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Place Clasification Saved') {
                var builder = '<tr id="trResult' + data.ItemID + '"><td>' + $('#PlaceClasificationInfo_PlaceClasification').val()
                    + '</td><td class="tds"><img id="delItem' + data.ItemID
                    + '" src="/Content/themes/base/images/cross.png" class="right"></td></tr>';
                $('#tblSearchPlaceClasificationsResults tbody').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                $('#frmPlaceClasificationInfo').clearForm();
                PLACE.placeItemSelection();
            }
            else {
                $('#trResult' + data.ItemID).children('td:nth-child(1)').text($('#PlaceClasificationInfo_PlaceClasification').val());
            }
            $.getJSON('/Places/GetDDLData', { path: 'PlaceClasification', id: $('#PlaceInfo_PlaceType option:selected').val() }, function (data) {
                $('#PlaceInfo_PlaceClasification').fillSelect(data);
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var savePlaceTypeSuccess = function (data) {
        var duration = (data.ResponseType < 0) ? data.ResponseType : null;
        if (data.ResponseType != 0) {
            if (data.ResponseMessage == 'Place Type Saved') {
                var builder = '<tr id="trResult' + data.ItemID + '"><td>' + $('#PlaceTypeInfo_PlaceType').val()
                    + '</td><td class="tds"><img id="delItem' + data.ItemID
                    + '" src="/Content/themes/base/images/cross.png" class="right"></td></tr>';
                $('#tblSearchPlaceTypesResults tbody').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                $('#frmPlaceTypeInfo').clearForm();
                PLACE.placeItemSelection();
            }
            else {
                $('#trResult' + data.ItemID).children('td:nth-child(1)').text($('#PlaceTypeInfo_PlaceType').val());
            }
            $.getJSON('/Places/GetDDLData', { path: 'PlaceType', id: 0 }, function (data) {
                $('#PlaceInfo_PlaceType').fillSelect(data);
                $('#PlaceClsification_PlaceType').fillSelect(data);
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var saveTransportationZoneSuccess = function (data) {
        var duration = (data.ResponseType > 0) ? duration = data.ResponseType : null;
        if (data.ResponseType > 0) {
            if (data.ResponseMessage == 'Transportation Zone Saved') {
                var builder = '<tr id="trResult' + data.ItemID + '"><td>' + $('#TransportationZoneInfo_TransportationZone').val()
                    + '</td><td class="tds"><img id="delItem' + data.ItemID
                    + '" src="/Content/themes/base/images/cross.png" class="right"></td></tr>';
                $('#tblSearchTransportationZonesResults tbody').append(builder);
                UI.tablesHoverEffect();
                UI.tablesStripedEffect();
                $('#frmTransportationZoneInfo').clearForm();
                PLACE.placeItemSelection();
            }
            else {
                $('#trResult' + data.ItemID).children('td:nth-child(1)').text($('#TransportationZoneInfo_TransportationZone').val());
            }
            $.getJSON('/Places/GetDDLData', { path: 'TransportationZone', id: 0 }, function (data) {
                $('#PlaceInfo_TransportationZone').fillSelect(data);
            });
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var deletePlaceRelatedItemSuccess = function (data, path) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        $.getJSON('/Places/GetDDLData', { path: path.substr(0, path.length - 1), id: 0 }, function (data) {
            if (path != 'Zones') {
                $('#PlaceInfo_' + path.substr(0, (path.length - 1))).fillSelect(data);
                if (path == 'Destinations') {
                    $('#ZoneInfo_' + path.substr(0, (path.length - 1))).fillSelect(data);
                    $('#TransportationZoneInfo_' + path.substr(0, (path.length - 1))).fillSelect(data);
                }
                if (path == 'PlaceTypes') {
                    $('#PlaceClasificationInfo_' + path.substr(0, (path.length - 1))).fillSelect(data);
                }
            }
        });
        if ($('#tblSearch' + path + 'Results tbody').find('.selected-row').length > 0) {
            var event = $.Event('keydown');
            event.keyCode = 27;
            event.source = 'btnNew' + path.substr(0, (path.length - 1)) + 'Info';
            $(document).trigger(event);
        }
        $('#tblSearch' + path + 'Results tbody').find('#trResult' + data.ItemID).remove();
        UI.tablesStripedEffect();
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var searchResultsTable = function (data) {
        var tableColumns = $(data).find('tbody tr').first().find('td');
        UI.searchResultsTable('tblSearchPlacesResults', tableColumns.length - 1);
        PLACE.oTable = $('#tblSearchPlacesResults').dataTable();
        $('.paging_two_button').children().on('mousedown', function (e) {
            if ($(this).parent().siblings('table').find('.selected-row').length > 0) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                $(document).trigger(event);
            }
            $(this).bind('click', function () { PLACE.makeTableRowsSelectable(); });
        });
        $('#tblSearchPlacesResults_length').unbind('change').on('change', function () {
            PLACE.makeTableRowsSelectable();
        });
    }

    var makeTableRowsSelectable = function () {
        $('#tblSearchPlacesResults tbody tr').not('theader').click(function (e) {
            var $target = $(e.target);
            if (!$target.is('img')) {
                if (!$(this).hasClass('selected-row')) {
                    //new portion
                    var event = $.Event('keydown');
                    event.keyCode = 27;
                    $(document).trigger(event);

                    $(this).parent('tbody').find('.selected-row').removeClass('selected-row primary');
                    $(this).addClass('selected-row primary');
                    $('#frmPlacesSearch').addClass('selected-row-active');
                    $('#fdsPlaceDescriptions').show();
                    $('#fdsPlacePlanTypes').show();
                    $('#fdsRoomTypes').show();
                    $('#fdsPlacesPictures').show();
                    //$('#fdsPlacesPictures').show();
                    var id = $(this).attr('id').substr(7);
                    $.ajax({
                        url: '/Places/GetPlace',
                        cache: false,
                        type: 'POST',
                        data: { id: id },
                        success: function (data) {
                            if (data != undefined) {
                                $('#PlaceInfo_PlaceID').val(id);
                                $('#PlacePlanType_PlaceID').val(id);//planType related
                                $('#RoomTypeInfo_PlaceID').val(id);//roomType related
                                $('#PlaceDescription_PlaceID').val(id);
                                $('#PlaceInfo_Place').val(data.PlaceInfo_Place);
                                $('#PlaceInfo_PlaceLabel').val(data.PlaceInfo_PlaceLabel);
                                $('#PlaceInfo_Address').val(data.PlaceInfo_Address);
                                $('#PlaceInfo_Phone').val(data.PlaceInfo_Phone);
                                $('#PlaceInfo_Latitude').val(data.PlaceInfo_Latitude);
                                $('#PlaceInfo_Longitude').val(data.PlaceInfo_Longitude);
                                $('#PlaceInfo_Destination option[value=' + data.PlaceInfo_Destination + ']').attr('selected', true);
                                $('#PlaceInfo_Destination').trigger('change', { infoZoneID: data.PlaceInfo_Zone, infoTZoneID: data.PlaceInfo_TransportationZone });
                                var builder = '';
                                $.each(data.PlaceInfo_ListTerminals, function (index, item) {
                                    builder += '<tr><td id="tdTerminal' + item.Key + '">'
                                    + '<input type="hidden" val="' + item.Key + '"/>'
                                    + item.Value
                                    + '</td>'
                                    + '<td>';
                                    if($.inArray(item.Key, data.PlaceInfo_AllowedTerminals) > -1){
                                        builder += '<img class="right" src="/Content/themes/base/images/cross.png"/>';
                                    }
                                    builder += '</td>'
                                    + '</tr>';
                                });
                                $('#tblTerminalsPerPlace tbody').empty();
                                $('#tblTerminalsPerPlace tbody').append(builder);
                                UI.tablesHoverEffect();
                                UI.tablesStripedEffect();
                                PLACE.makeTblTerminalsRowsRemovable();
                                if (data.PlaceInfo_IsVillaGroup == false) {
                                    $('input:radio[name="PlaceInfo_IsVillaGroup"]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name="PlaceInfo_IsVillaGroup"]')[1].checked = true;
                                }
                                $('#PlaceInfo_PlaceType option[value=' + data.PlaceInfo_PlaceType + ']').attr('selected', true);
                                $('#PlaceInfo_PlaceType').trigger('change', data.PlaceInfo_PlaceClasification);
                                if (data.PlaceInfo_Prospectation == false) {
                                    $('input:radio[name="PlaceInfo_Prospectation"]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name="PlaceInfo_Prospectation"]')[1].checked = true;
                                }
                                if (data.PlaceInfo_IsForSale == false) {
                                    $('input:radio[name="PlaceInfo_IsForSale"]')[0].checked = true;
                                }
                                else {
                                    $('input:radio[name="PlaceInfo_IsForSale"]')[1].checked = true;
                                }
                                $('#PlaceInfo_CheckInTime').val(data.PlaceInfo_CheckInTime);
                                $('#PlaceInfo_CheckOutTime').val(data.PlaceInfo_CheckOutTime);
                                $('#PlaceInfo_TaxesPercentage').val(data.PlaceInfo_TaxesPercentage);
                                if ($('#fdsPlacesManagement').children('div').css('display') == 'none') {
                                    $('#btnNewPlaceInfo').trigger('click');
                                }
                                if (data.PlaceInfo_PlaceType == 1) {
                                    $('#divAllInclusive').show();
                                }
                                else {
                                    $('#PlaceInfo_AllInclusive').val('');
                                    $('#divAllInclusive').hide();
                                }
                                PLACE.updateUlPlaceDescriptions(id);
                                PLACE.updateUlPlacePlanTypes(id);
                                PLACE.updateUlRoomTypes(id);
                                PICTURE._class = '.places-pictures ';
                                $(PICTURE._class + '.picture-info-item-type').val('Places');
                                PICTURE.getItemNames(true);
                                $(PICTURE._class + '.picture-info-item-id').val(id);
                                PICTURE.getGalleryName($(PICTURE._class + '.picture-info-item-type').val(), $(PICTURE._class + '.picture-info-item-id').val());
                                PICTURE.GetImagesPerItemType($(PICTURE._class + '.picture-info-item-type').val(), $(PICTURE._class + '.picture-info-item-id').val(), undefined, PICTURE._class);
                                PICTURE.loadPicturesTree();
                                UI.expandFieldset('fdsPlacesInfo');
                                UI.scrollTo('fdsPlacesInfo', null);
                                initMap();
                                moveMapMarker();
                            }
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deletePlace, [$(e.target).attr('id').substr(4)]);
            }
        });
    }

    var deletePlaceSuccess = function (data, id) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if ($('#trPlace' + id).hasClass('selected-row')) {
                $(document).find('.selected-row').each(function () {
                    var event = $.Event('keydown');
                    event.keyCode = 27;
                    $(document).trigger(event);
                });
            }
            PLACE.oTable.fnDeleteRow($('#trPlace' + id)[0]);
            UI.tablesHoverEffect();
            UI.tablesStripedEffect();
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var deletePlaceDescriptionSuccess = function (data, id) {
        var duration = (data.ResponseType > 0) ? duration = data.ResponseType : null;
        if (data.ResponseType > 0) {
            if ($('#liDescription' + id).hasClass('selected-row')) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                event.source = 'btnNewPlaceDescription';
                $(document).trigger(event);
            }
            $('#liDescription' + id).remove();
            UI.ulsHoverEffect('ulPlaceDescriptions');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + "<br />" + data.ExceptionMessage, duration, data.InnerException);
    }

    var deletePlacePlanTypeSuccess = function (data, id) {
        var duration = data.ResponseType < 0 ? data.ResponseType : null;
        if (data.ResponseType > 0) {
            if ($('#liPlacePlanType' + data.ItemID).hasClass('selected-row')) {
                var event = $.Event('keydown');
                event.keyCode = 27;
                event.source = 'btnNewPlacePlanType';
                $(document).trigger(event);
            }
            $('#liPlacePlanType' + id).remove();
            UI.ulsHoverEffect('ulPlacePlanTypes');
        }
        UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    }

    var updateUlPlaceDescriptions = function (id, prevID) {
        $.ajax({
            url: '/Places/GetPlaceDescriptions',
            cache: false,
            type: 'POST',
            data: { id: id },
            success: function (data) {
                $('#ulPlaceDescriptions').empty();
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="liDescription' + item.PlaceDescription_PlaceDescriptionID + '">' + $('#PlaceDescription_Culture option[value="' + item.PlaceDescription_Culture + '"]').text() + '<img id="imDescription' + item.PlaceDescription_PlaceDescriptionID + '" src="/Content/themes/base/images/cross.png" class="right" /></li>';
                    });
                    $('#ulPlaceDescriptions').append(builder);
                    UI.ulsHoverEffect('ulPlaceDescriptions');
                    PLACE.ulPlaceDescriptionsSelection();
                }
                else {
                    builder += '<li id="liNoDescriptions">No Descriptions For This Place</li>';
                    $('#ulPlaceDescriptions').append(builder);
                }
            }
        });
    }

    var updateUlPlacePlanTypes = function (id) {
        $.ajax({
            url: '/Places/GetPlacePlanTypes',
            cache: false,
            type: 'POST',
            data: { id: id },
            success: function (data) {
                $('#ulPlacePlanTypes').empty();
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="liPlacePlanType' + item.PlacePlanType_PlacePlanTypeID + '">'
                            + $('#PlacePlanType_PlanType option[value="' + item.PlacePlanType_PlanType + '"]').text()
                            + '<img id="imPlacePlanType' + item.PlacePlanType_PlacePlanTypeID + '" src="/Content/themes/base/images/cross.png" class="right"/></li>';
                    });
                    $('#ulPlacePlanTypes').append(builder);
                    UI.ulsHoverEffect('ulPlacePlanTypes');
                    PLACE.ulPlacePlanTypesSelection();
                }
                else {
                    builder += '<li id="liNoPlacePlanTypes">No Plan Types For This Place</li>';
                    $('#ulPlacePlanTypes').append(builder);
                }
            }
        });
    }

    var updateUlRoomTypes = function (id) {
        $.ajax({
            url: '/Places/GetRoomTypes',
            cache: false,
            type: 'POST',
            data: { id: id },
            success: function (data) {
                $('#ulRoomTypes').empty();
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="liRoomType' + item.RoomTypeInfo_RoomTypeID + '" class="skip-one-level">'
                            + item.RoomTypeInfo_RoomType + '<img id="imRoomType' + item.RoomTypeInfo_RoomTypeID
                            + '" src="/Content/themes/base/images/cross.png" class="right"/></li>';
                    });
                    $('#ulRoomTypes').append(builder);
                    UI.ulsHoverEffect('ulRoomTypes');
                    PLACE.ulRoomTypesSelection();
                }
                else {
                    builder += '<li id="liNoRoomTypes">No Room Types For This Place</li>';
                    $('#ulRoomTypes').append(builder);
                }
            }
        });
    }

    var updateUlRoomTypeDescriptions = function (id) {
        $.ajax({
            url: '/Places/GetRoomTypeDescriptions',
            cache: false,
            type: 'POST',
            data: { roomTypeID: id },
            success: function (data) {
                $('#ulRoomTypeDescriptions').empty();
                var builder = '';
                if (data.length > 0) {
                    $.each(data, function (index, item) {
                        builder += '<li id="liRoomTypeDescription' + item.RoomTypeDescription_RoomTypeDescriptionID + '">'
                            + $('#RoomTypeDescription_Culture option[value="' + item.RoomTypeDescription_Culture + '"]').text()
                            + '<img id="imRoomTypeDescription' + item.RoomTypeDescription_RoomTypeDescriptionID
                            + '" src="/Content/themes/base/images/cross.png" class="right"/></li>';
                    });
                    $('#ulRoomTypeDescriptions').append(builder);
                    UI.ulsHoverEffect('ulRoomTypeDescriptions');
                    PLACE.ulRoomTypeDescriptionsSelection();
                }
                else {
                    builder += '<li id="liNoRoomTypeDescriptions">No Descriptions For This Room Type</li>';
                    $('#ulRoomTypeDescriptions').append(builder);
                }
            }
        });
    }

    var ulPlaceDescriptionsSelection = function () {
        $('#ulPlaceDescriptions').unbind('click').bind('click', function (e) {
            console.log($(e.target));
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(this).find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $(this).parents('ul').addClass('selected-row-active');
                    $('#PlaceDescription_PlaceDescriptionID').val($(e.target).attr('id').substr(13));
                    $.ajax({
                        url: '/Places/GetPlaceDescription',
                        cache: false,
                        type: 'POST',
                        data: { placeDescriptionID: $('#PlaceDescription_PlaceDescriptionID').val() },
                        success: function (data) {
                            $('#PlaceDescription_Culture option[value=' + data.PlaceDescription_Culture + ']').attr('selected', true);
                            $('#PlaceDescription_ShortDescription').val(data.PlaceDescription_ShortDescription);
                            $('#PlaceDescription_FullDescription').val(data.PlaceDescription_FullDescription);
                            $('#PlaceDescription_FAQ').val(data.PlaceDescription_FAQ);
                            $('#PlaceDescription_Amenities').val(data.PlaceDescription_Amenities);
                            $('#PlaceDescription_AllInclusive').val(data.PlaceDescription_AllInclusive);
                            UI.expandFieldset('fdsPlaceDescriptionInfo');
                            UI.scrollTo('fdsPlaceDescriptionInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deletePlaceDescription, [$(e.target).attr('id').substr(13)]);
            }
        });
    }

    var ulPlacePlanTypesSelection = function () {
        $('#ulPlacePlanTypes').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(this).find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $(this).parents('ul').addClass('selected-row-active');
                    $('#PlacePlanType_PlacePlanTypeID').val($(e.target).attr('id').substr(15));
                    $.ajax({
                        url: '/Places/GetPlacePlanType',
                        cache: false,
                        type: 'POST',
                        data: { placePlanTypeID: $('#PlacePlanType_PlacePlanTypeID').val() },
                        success: function (data) {
                            $('#PlacePlanType_PlanType option[value=' + data.PlacePlanType_PlanType + ']').attr('selected', true);
                            $('#PlacePlanType_Terminal option[value=' + data.PlacePlanType_Terminal + ']').attr('selected', true);
                            UI.expandFieldset('fdsPlacePlanTypeInfo');
                            UI.scrollTo('fdsPlacePlanTypeInfo', null);
                        }
                    });
                }
            }
            else {
                UI.confirmBox('Do you confirm you want to proceed?', deletePlacePlanType, [$(e.target).attr('id').substr(15)]);
            }
        });
    }

    var ulRoomTypesSelection = function () {
        
        $('#ulRoomTypes').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(this).find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $(this).parents('ul').addClass('selected-row-active');
                    $('#RoomTypeInfo_RoomTypeID').val($(e.target).attr('id').substr(10));
                    $('#RoomTypeDescription_RoomTypeID').val($(e.target).attr('id').substr(10));
                    $.ajax({
                        url: '/Places/GetRoomType',
                        cache: false,
                        type: 'POST',
                        data: { roomTypeID: $('#RoomTypeInfo_RoomTypeID').val() },
                        success: function (data) {
                            PICTURE.getItemNames(true, $('#PlaceInfo_PlaceID').val());
                            $('#RoomTypeInfo_RoomType').val(data.RoomTypeInfo_RoomType);
                            $('#RoomTypeInfo_Quantity').val(data.RoomTypeInfo_Quantity);
                            UI.expandFieldset('fdsRoomTypeInfo');
                            PLACE.updateUlRoomTypeDescriptions($('#RoomTypeInfo_RoomTypeID').val());
                            $('#fdsRoomTypeDescriptions').show();
                            PICTURE._class = '.roomTypes-pictures ';
                            $(PICTURE._class + '.picture-info-item-type').val('Room Type');
                            if ($(PICTURE._class + '.items-galleries option').length <= 1) {
                                PICTURE.getItemNames(true, $('#PlaceInfo_PlaceID').val());
                            }
                            $(PICTURE._class + '.picture-info-item-id').val($('#RoomTypeInfo_RoomTypeID').val());
                            PICTURE.getGalleryName($(PICTURE._class + '.picture-info-item-type').val(), $(PICTURE._class + '.picture-info-item-id').val());
                            PICTURE.GetImagesPerItemType($(PICTURE._class + '.picture-info-item-type').val(), $(PICTURE._class + '.picture-info-item-id').val(), undefined, PICTURE._class);
                            //$('#fdsPlacesPictures').show();
                            $('#fdsRoomTypePictures').show();
                            UI.scrollTo('fdsRoomTypeInfo', null);
                            PICTURE.loadPicturesTree();
                        }
                    });
                }
            }
            else
                UI.confirmBox('Do you confirm you want to proceed?', deleteRoomType, [$(e.target).attr('id').substr(10)]);
        });
    }

    var ulRoomTypeDescriptionsSelection = function () {
        $('#ulRoomTypeDescriptions').unbind('click').bind('click', function (e) {
            if (!$(e.target).is('img')) {
                if (!$(e.target).hasClass('selected-row')) {
                    $(this).find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).addClass('selected-row secondary');
                    $('#RoomTypeDescription_RoomTypeDescriptionID').val($(e.target).attr('id').substr(21));
                    $.ajax({
                        url: '/Places/GetRoomTypeDescription',
                        cache: false,
                        type: 'POST',
                        data: { roomTypeDescriptionID: $('#RoomTypeDescription_RoomTypeDescriptionID').val() },
                        success: function (data) {
                            $('#RoomTypeDescription_RoomType').val(data.RoomTypeDescription_RoomType);
                            $('#RoomTypeDescription_Description').val(data.RoomTypeDescription_Description);
                            $('#RoomTypeDescription_Culture option[value="' + data.RoomTypeDescription_Culture + '"]').attr('selected', true);
                            UI.expandFieldset('fdsRoomTypeDescriptionInfo');
                            UI.scrollTo('fdsRoomTypeDescriptionInfo', null);
                        }
                    });
                }
            }
            else
                UI.confirmBox('Do you confirm you want to proceed?', deleteRoomTypeDescription, [$(e.target).attr('id').substr(21)]);
        });
    }

    var placeItemSelection = function () {
        $('.search-results').unbind('click').bind('click', function (e) {
            var length = $(this).attr('id').substr(9).length - 7;
            var path = $(this).attr('id').substr(9, length);
            var id = $(e.target).parents('tr').first().attr('id').substr(8);
            if ($(e.target).is('img')) {
                UI.confirmBox('Do you confirm you want to proceed?', deletePlaceItem, [id, path]);
            }
            else {
                if (!$(e.target).is('th')) {
                    $(e.currentTarget).find('.selected-row').removeClass('selected-row secondary');
                    $(e.target).parents('tr').first().addClass('selected-row secondary');
                    $.ajax({
                        url: '/Places/GetPlaceItem',
                        cache: false,
                        type: 'POST',
                        data: { id: id, path: path },
                        success: function (data) {
                            switch (path) {
                                case 'TransportationZones': {
                                    $('#TransportationZoneInfo_TransportationZoneID').val(id);
                                    $('#TransportationZoneInfo_TransportationZone').val(data.firstItem);
                                    $('#TransportationZoneInfo_Destination option[value=' + data.secondItem + ']').attr('selected', true);
                                    break;
                                }
                                case 'PlaceClasifications': {
                                    $('#PlaceClasificationInfo_PlaceClasificationID').val(id);
                                    $('#PlaceClasificationInfo_PlaceClasification').val(data.firstItem);
                                    $('#PlaceClasificationInfo_PlaceType option[value=' + data.secondItem + ']').attr('selected', true);
                                    if (data.thirdItem == false) {
                                        $('input:radio[name=PlaceClasificationInfo_Hosting]')[1].checked = true;
                                    }
                                    else {
                                        $('input:radio[name=PlaceClasificationInfo_Hosting]')[0].checked = true;
                                    }
                                    break;
                                }
                                case 'Destinations': {
                                    $('#DestinationInfo_DestinationID').val(id);
                                    $('#DestinationInfo_Destination').val(data.firstItem);
                                    $('#DestinationInfo_Latitude').val(data.secondItem);
                                    $('#DestinationInfo_Longitude').val(data.thirdItem);
                                    break;
                                }
                                case 'PlaceTypes': {
                                    $('#PlaceTypeInfo_PlaceTypeID').val(id);
                                    $('#PlaceTypeInfo_PlaceType').val(data.firstItem);
                                    if (data.secondItem == false) {
                                        $('input:radio[name=PlaceTypeInfo_IsActive]')[1].checked = true;
                                    }
                                    else {
                                        $('input:radio[name=PlaceTypeInfo_IsActive]')[0].checked = true;
                                    }
                                    break;
                                }
                                case 'Zones': {
                                    $('#ZoneInfo_ZoneID').val(id);
                                    $('#ZoneInfo_Zone').val(data.firstItem);
                                    $('#ZoneInfo_Destination option[value=' + data.secondItem + ']').attr('selected', true);
                                    break;
                                }
                            }
                            UI.expandFieldset('fds' + path + 'Info');
                            UI.scrollTo('fds' + path + 'Info', null);
                            initMap2();
                            moveMapMarker2();
                        }
                    });
                }
            }
        });
    }

    function deletePlace(placeID) {
        $.ajax({
            url: '/Places/DeletePlace',
            cache: false,
            type: 'POST',
            data: { id: placeID },
            success: function (data) {
                PLACE.deletePlaceSuccess(data, placeID);
            }
        });
    }

    function deletePlaceDescription(placeDescriptionID) {
        $.ajax({
            url: '/Places/DeletePlaceDescription',
            cache: false,
            type: 'POST',
            data: { placeDescriptionID: placeDescriptionID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liDescription' + placeDescriptionID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liDescription' + data.ItemID).remove();
                    if ($('#ulPlaceDescriptions li').length == 0)
                        $('#ulPlaceDescriptions').append('<li id="liNoDescriptions">No Descriptions For This Place</li>');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deletePlacePlanType(placePlanTypeID) {
        $.ajax({
            url: '/Places/DeletePlacePlanType',
            cache: false,
            type: 'POST',
            data: { placePlanTypeID: placePlanTypeID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liPlacePlanType' + placePlanTypeID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liPlacePlanType' + data.ItemID).remove();
                    if ($('#ulPlacePlanTypes li').length == 0)
                        $('#ulPlacePlanTypes').append('<li id="liNoPlacePlanTypes">No Plan Types For This Place</li>');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteRoomType(roomTypeID) {
        $.ajax({
            url: '/Places/DeleteRoomType',
            cache: false,
            type: 'POST',
            data: { roomTypeID: roomTypeID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liRoomType' + data.ItemID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liRoomType' + data.ItemID).remove();
                    if ($('#ulRoomTypes li').length == 0)
                        $('#ulRoomTypes').append('<li id="liNoRoomTypes">No Room Types For This Place</li>');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    function deleteRoomTypeDescription(roomTypeDescriptionID) {
        $.ajax({
            url: '/Places/DeleteRoomTypeDescription',
            cache: false,
            type: 'POST',
            data: { roomTypeDescriptionID: roomTypeDescriptionID },
            success: function (data) {
                var duration = data.ResponseType < 0 ? data.ResponseType : null;
                if (data.ResponseType > 0) {
                    if ($('#liRoomTypeDescription' + data.ItemID).hasClass('selected-row')) {
                        var event = $.Event('keydown');
                        event.keyCode = 27;
                        $(document).trigger(event);
                    }
                    $('#liRoomTypeDescription' + data.ItemID).remove();
                    if ($('#ulRoomTypeDescriptions li').length == 0)
                        $('#ulRoomTypeDescriptions').append('<li id="liNoRoomTypeDescriptions">No Descriptions For This Room Type</li>');
                }
                UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
            }
        });
    }

    //function deletePlaceDescription(placePlanTypeID) {
    //    $.ajax({
    //        url: '/Places/DeletePlacePlanType',
    //        cache: false,
    //        type: 'POST',
    //        data: { placePlanTypeID: placePlanTypeID },
    //        success: function (data) {
    //            var duration = data.ResponseType < 0 ? data.ResponseType : null;
    //            if (data.ResponseType > 0) {
    //                if ($('#liPlacePlanType' + placePlanTypeID).hasClass('selected-row')) {
    //                    var event = $.Event('keydown');
    //                    event.keyCode = 27;
    //                    $(document).trigger(event);
    //                }
    //                $('#liPlacePlanType' + data.ItemID).remove();
    //            }
    //            UI.messageBox(data.ResponseType, data.ResponseMessage + '<br />' + data.ExceptionMessage, duration, data.InnerException);
    //        }
    //    });
    //}

    function deletePlaceItem(placeItemID, placeItem) {
        $.ajax({
            url: '/Places/DeletePlaceItem',
            cache: false,
            type: 'POST',
            data: { id: placeItemID, path: placeItem },
            success: function (data) {
                PLACE.deletePlaceRelatedItemSuccess(data, placeItem);
            }
        });
    }

    var makeTblTerminalsRowsRemovable = function () {
        $('#tblTerminalsPerPlace tbody tr').not('theader').on('click', function (e) {
            if ($(e.target).is('img')) {
                $(this).remove();
                UI.tablesStripedEffect();
            }
        });
    }

    return {
        ulRoomTypeDescriptionsSelection: ulRoomTypeDescriptionsSelection,
        saveRoomTypeDescriptionSuccess: saveRoomTypeDescriptionSuccess,
        makeTblTerminalsRowsRemovable: makeTblTerminalsRowsRemovable,
        savePlaceClasificationSuccess: savePlaceClasificationSuccess,
        deletePlaceRelatedItemSuccess: deletePlaceRelatedItemSuccess,
        deletePlaceDescriptionSuccess: deletePlaceDescriptionSuccess,
        saveTransportationZoneSuccess: saveTransportationZoneSuccess,
        ulPlaceDescriptionsSelection: ulPlaceDescriptionsSelection,
        updateUlRoomTypeDescriptions: updateUlRoomTypeDescriptions,
        savePlaceDescriptionSuccess: savePlaceDescriptionSuccess,
        deletePlacePlanTypeSuccess: deletePlacePlanTypeSuccess,
        updateUlPlaceDescriptions: updateUlPlaceDescriptions,
        ulPlacePlanTypesSelection: ulPlacePlanTypesSelection,
        savePlacePlanTypeSuccess: savePlacePlanTypeSuccess,
        makeTableRowsSelectable: makeTableRowsSelectable,
        updateUlPlacePlanTypes: updateUlPlacePlanTypes,
        saveDestinationSuccess: saveDestinationSuccess,
        savePlaceTypeSuccess: savePlaceTypeSuccess,
        ulRoomTypesSelection: ulRoomTypesSelection,
        saveRoomTypeSuccess: saveRoomTypeSuccess,
        searchResultsTable: searchResultsTable,
        deletePlaceSuccess: deletePlaceSuccess,
        placeItemSelection: placeItemSelection,
        updateUlRoomTypes: updateUlRoomTypes,
        savePlaceSuccess: savePlaceSuccess,
        saveZoneSuccess: saveZoneSuccess,
        init: init
    }
}();