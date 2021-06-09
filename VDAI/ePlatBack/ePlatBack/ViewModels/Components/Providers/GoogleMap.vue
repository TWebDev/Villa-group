<template>
    <div>
        <div>
            <h5>Ubication </h5>
            <div class="row">
                <div class="col-sm-12">
                    <div class="col-sm-6">
                        <gmap-autocomplete placeholder="Find a restaurant..." class="form-control mb-2" @place_changed="setPlace">
                        </gmap-autocomplete>
                    </div>
                    <div class="col-sm-6">
                        <button class="btn btn-primary d-inline " @click="addMarker">Add</button>
                    </div>
                </div>
            </div>

            <br />
            <div>

            </div>
        </div>
        <br>
        <gmap-map :center="center"
                  :zoom="12"
                  style="width:100%;  height: 400px;"
                   @click="addLocation"
                  >
            <gmap-marker :key="index"
                         v-for="(m, index) in markers"
                         :position="m.position"
                         :clickable="true"
                        
                        ></gmap-marker>
        </gmap-map>
    </div>
</template>

<script>
    import { bus } from './app.js';

    export default {
        name: "GoogleMap",
        props: ['lat', 'lng'],
        data() {
            return {
                // default to Montreal to keep it simple
                // change this to whatever makes sense
                center: { lat: 20.6445035, lng: -105.23821279999999 },
                markers: [],
                places: [],
                currentPlace: null,
                latitud: 'latitud es',
            };
        },

        mounted() {
            this.geolocate();
            console.log(this.lat);
            if (this.lat !== undefined) {

                const marker = {
                    lat: parseFloat(this.lat),
                    lng: parseFloat(this.lng)
                };
                this.markers.push({ position: marker });

                this.center = marker;
                this.currentPlace = null;
                this.latitud = this.center.lat;


            } else {
                const marker = {
                    lat: 20.65340699999999,
                    lng: -105.2253316
                };
                this.markers.push({ position: marker });

            }
        },

        methods: {
            // receives a place object via the autocomplete component
            setPlace(place) {
                this.currentPlace = place;
            },
            addMarker() {
                if (this.currentPlace) {
                    this.markers.pop();
                    console.log("ey");
                    const marker = {
                        lat: this.currentPlace.geometry.location.lat(),
                        lng: this.currentPlace.geometry.location.lng()
                    };
                    this.markers.push({ position: marker });
                    this.places.push(this.currentPlace);
                    this.center = marker;
                    this.currentPlace = null;
                    this.latitud = this.center.lat;

                    bus.$emit('maps', {
                        lat: this.center.lat,
                        lon: this.center.lng,
                    });
                }
            },
            addLocation: function (location) {


                this.center = {
                    lat: location.latLng.lat(),
                    lng: location.latLng.lng(),
                };
                this.markers = [];
                const marker = {
                    lat: location.latLng.lat(),
                    lng: location.latLng.lng(),
                };
                this.markers.push({ position: marker });
                console.log(this.center);
                bus.$emit('maps', {
                    lat: this.center.lat,
                    lon: this.center.lng,
                });
            },
            geolocate: function () {

            }
        }
    };
</script>