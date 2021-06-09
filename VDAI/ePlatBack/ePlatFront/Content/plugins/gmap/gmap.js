var Gmap = function () {
    var map;
    var markersArray = new Array();
    var latLng;

    var init = function (node) {
        latLng = new google.maps.LatLng(20.679767, -105.254180);
        var myOptions = {
            zoom: 14,
            center: latLng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById(node), myOptions);
    }

    var moveMarker = function (lat, lng, title) {
        latLng = new google.maps.LatLng(lat, lng);
        markersArray[0] = null;
        var marker = new google.maps.Marker({
            position: latLng,
            map: map,
            draggable: false,
            title: title
        });
        markersArray.push(marker);
        map.panTo(latLng);
    }

    var resize = function () {
        google.maps.event.trigger(map, 'resize');
    }

    return {
        init: init,
        moveMarker: moveMarker,
        resize: resize
    }
}();