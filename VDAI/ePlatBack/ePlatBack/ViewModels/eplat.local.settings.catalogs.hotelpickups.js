var HotelPickUps = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data: {
        Shared: ePlatStore,
        Hotels: [],
        DataTable: {
            fields: [
                { key: 'SpiHotelID', label: 'ID', sortable: true },
                { key: 'Hotel', sortable: true },
                { key: 'destination', label: 'Destination' },
                { key: 'pickup', label: 'Pick Up' },
                { key: 'actions', label: '' },
            ],
            sortBy: 'Hotel',
            sortDesc: false,
            perPage: 20,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: false
        },
        SelectedHotel: {
            Hotel: '',
            Lat: '',
            Lng: ''
        },
        map: null,
        marker: null
    },
    methods: {
        setSearchResults: function (data) {
            //asignar resultados de la búsqueda a la lista de bancos
            this.Hotels = data;
            let self = this;
            $.each(this.Hotels, function (i, h) {
                h.Destinations = self.Shared.State.DependentFields.Fields[0].Values;
            });
            //Vue.nextTick(function () {
                
            //});
        },
        getZones: function (destinationID) {
            if (destinationID != null) {
                let zones = self.Shared.State.DependentFields.Fields[1].Values.filter(function (x) {
                    return x.ParentValue == destinationID
                });
                zones.push({
                    Value: "",
                    Text: "Unknown"
                });
                return zones;
            }            
        },
        selectHotel: function (item) {
            this.SelectedHotel = item;
        },
        openUploadModal: function (item) {
            $('#messages').html('');
            this.selectHotel(item);
            
            //uploader
            $fub = $('#fine-uploader-basic');
            $messages = $('#messages');
            let self = this;

            let uploader = new qq.FineUploaderBasic({
                button: $fub[0],
                request: {
                    endpoint: '/Catalogs/UploadPickUpPicture',
                    params: {
                        SpiHotelID: self.SelectedHotel.SpiHotelID
                    }
                },
                validation: {
                    allowedExtensions: ['jpeg', 'jpg', 'gif', 'png'],
                    sizeLimit: 5242880 // 5MB
                },
                callbacks: {
                    onSubmit: function (id, fileName) {
                        $messages.append('<div id="file-' + id + '" class="alert" style="margin: 20px 0 0"></div><div class="progress-' + id + '"><div class="progress-bar" role="progressbar" style="width: 0%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div></div>');
                    },
                    onUpload: function (id, fileName) {
                        $('#file-' + id).addClass('alert-info')
                                        .html('<img src="/images/loading.gif" alt="Initializing. Please hold." style="width:16px;"> ' +
                                              'Initializing ' +
                                              '“' + fileName + '”');
                    },
                    onProgress: function (id, fileName, loaded, total) {
                        if (loaded < total) {
                            progress = Math.round(loaded / total * 100) + '% of ' + Math.round(total / 1024) + ' kB';
                            $('#file-' + id).removeClass('alert-info')
                                            .html('<img src="/images/loading.gif" alt="In progress. Please hold." style="width:16px;"> ' +
                                                  'Uploading ' +
                                                  '“' + fileName + '” ' +
                                                  progress);
                            $('#progress-' + id).css('width', progress + '%');
                        } else {
                            $('#file-' + id).addClass('alert-info')
                                            .html('<img src="/images/loading.gif" alt="Saving. Please hold." style="width:16px;"> ' +
                                                  'Saving ' +
                                                  '“' + fileName + '”');

                        }
                    },
                    onComplete: function (id, fileName, responseJSON) {
                        if (responseJSON.success) {
                            $('#file-' + id).removeClass('alert-info')
                                            .addClass('alert-success')
                                            .html('<i class="icon-ok"></i> ' +
                                                  'Successfully saved ' +
                                                  '“' + fileName + '”' +
                                                  '<br><img src="' + responseJSON.path.path + '" alt="' + fileName + '" class="img-fluid">');
                            //integrar respuesta a la colección
                            //$.each(self.Hotels, function (i, h) {
                            //    if (h.SpiHotelID == responseJSON.response.ObjectID.SpiHotelID) {
                            //        h.Picture = responseJSON.path.path;
                            //        h.HotelPickUpID = responseJSON.response.ObjectID.HotelPickUpID;
                            //    }
                            //});
                            self.SelectedHotel.Picture = responseJSON.path.path;
                            self.SelectedHotel.HotelPickUpID = responseJSON.response.ObjectID.HotelPickUpID;
                        } else {
                            $('#file-' + id).removeClass('alert-info')
                                            .addClass('alert-error')
                                            .html('<i class="icon-exclamation-sign"></i> ' +
                                                  'Error with ' +
                                                  '“' + fileName + '”: ' +
                                                  responseJSON.response.Exception.Message);
                        }
                    }
                },
                debug: true
            });
        },
        updatePickUp: function (item) {
            let self = this;
            $.ajax({
                url: '/crm/Catalogs/SavePickUp',
                cache: false,
                type: 'POST',
                data: item,
                success: function (data) {
                    if (data.ResponseType == 1) {
                        //si la respuesta es OK
                        //notificación
                        $.alert({
                            title: 'Pick Up Succesfully Saved',
                            content: 'Pick Up for ' + item.Hotel + ' was succesfully saved.',
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'green'
                        });
                        //quitar el icono de guardar
                        $.each(self.Hotels, function (i, h) {
                            if (h.SpiHotelID == item.SpiHotelID) {
                                h.PendingChanges = false;
                                h.HotelPickUpID = data.HotelPickUpID;
                            }
                        });
                    } else {
                        //notificar el error al guardar
                        $.alert({
                            title: 'Error Saving',
                            content: 'There was an error trying to save the pick up for ' + item.Hotel + '. Please, try again.',
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'red'
                        });
                    }
                }
            });
        },
        pendingChanges: function (item) {
            item.PendingChanges = true;
            this.selectHotel(item);
            //$.each(this.Hotels, function (i, h) {
            //    if (h.SpiHotelID == item.SpiHotelID) {
            //        h.PendingChanges = true;
            //    }
            //});
        },
        openMapModal: function (item) {
            this.selectHotel(item);
            self = this;
            let uluru = { lat: 19.45072024966178, lng: -99.09160048126546 };
            let zoom = 4;
            if (item.Lat != null) {
                uluru = { lat: parseFloat(item.Lat), lng: parseFloat(item.Lng) };
                zoom = 18;
            }
            this.map = new google.maps.Map(document.getElementById('map'), {
                zoom: zoom,
                center: uluru
            });
            google.maps.event.addListener(this.map, 'click', function (event) {
                self.placeMarker(event.latLng);
            });
            if (item.Lat != null) {
                this.marker = new google.maps.Marker({
                    position: { lat: parseFloat(item.Lat), lng: parseFloat(item.Lng) },
                    map: this.map
                });
            }
        },
        placeMarker: function (location) {
            this.SelectedHotel.Lat = location.lat();
            this.SelectedHotel.Lng = location.lng();
            if (this.marker != null) {
                this.marker.setMap(null);
            }            
            this.marker = new google.maps.Marker({
                position: location,
                map: this.map
            });
        },
        savePickUpLocation: function () {
            this.SelectedHotel.PendingChanges = true;
        }
    },
    mounted: function () {
        //iniciar la sesión
        this.Session().getSessionDetails();

        //dependent fields
        this.UI().loadDependentFields('/crm/Catalogs/GetDependentFields', true);

        //show search card
        this.UI().showSearchCard();
    }
});