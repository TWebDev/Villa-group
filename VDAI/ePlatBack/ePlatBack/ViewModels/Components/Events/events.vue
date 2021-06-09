<template>

    <div>
            <div class="row mb-5">
                <div class="text-left col-sm-8">
                   <h2 style="position: relative">Events</h2>
                </div>
                <div class="col-sm-4 text-right">
                    <button class="btn btn-primary" @click="changeComponent">
                        <i class="material-icons" style="vertical-align: middle;">
                            add_circle
                        </i>Add Event
                    </button>
                </div>

            </div>
            <div v-if="loading" class="text-center">
                <pulse-loader color="#31A3DD"></pulse-loader>
            </div>
            <div v-else>
                <div class="row">

                    <div class="col-sm-12 mb-3">
                        <div class="card">
                            <div class="card-body">
                                <div class="card-title">
                                    <h5>Search Events</h5>
                                </div>
                                <div class="row">
                                    <div class="col-sm-6 col-md-6 col-lg-1">
                                        <span>Event ID</span>
                                        <input class="form-control" v-model="eventidsearch" type="number">
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-3">
                                        <span>Event </span>
                                        <input class="form-control" v-model="event" type="text">
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-3">
                                        <span>Location</span>
                                        <v-select :options="locations" v-model="location"></v-select>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-3">
                                        <span>Zone</span>
                                        <v-select :options="zones" v-model="zone"></v-select>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-2">
                                        <span>Orientation</span>
                                        <v-select :options="allorientations" v-model="orientation"></v-select>
                                    </div>
                                </div>

                                <div class="mt-2 text-right">
                                    <button class="btn btn-secondary" @click="clear">Clear</button>
                                    <button class="btn btn-primary" @click="search">Search</button>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>



                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <b-table class="hand" @row-clicked="showDetails" :per-page="perPage" striped hover
                                     :items="hotels" :fields="fields" :current-page="currentPage">
                                <template slot="orientations" slot-scope="row">
                                    <div v-for="ori in orient" v-if="row.item.id == ori.eventoid">
                                        {{ori.orientation}}
                                    </div>
                                </template>
                            </b-table>
                            <b-pagination size="md" :total-rows="countrows" v-model="currentPage" :per-page="perPage">
                            </b-pagination>
                        </div>
                    </div>
                </div>
            </div>


            <div v-if="details">
               <eventdetails :eventid="eventid" ></eventdetails>
            </div>
        </div>

</template>


<script>
    import { bus } from './app.js';
    import eventdetails from './eventdetails.vue';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
  




    export default {
        props: ['cards'],
            data: function () {
                return {
                    loading: false,
                    perPage: 10,
                    hotels: [],
                    currentPage: 1,
                    countrows: 2,
                    eventid: '',
                    details: false,
                    orient: '',
                    eventidsearch:'',
                    fields: [
                        {
                            key: 'id',
                            label: 'Event ID',
                            sortable: true

                        },
                        {
                            key: 'evento',
                            label: 'Event',
                            sortable: true

                        },
                        {
                            key: 'place',
                            label: 'Location',
                            sortable: true,

                        },
                       
                        {
                            key: 'zone',
                            label: 'Zone',
                            sortable: true,

                        },
                        {
                            key: 'orientations',
                            label: 'Orientations',
                           
                        },
                        
                       

                    ],
                    tempkey: null,
                    allorientations: [],
                    zones: [],
                    locations: [],
                    event: '',
                    
                    orientation: '',
                    location: '',
                    zone:'',
                }
        },
        components: {
       
            'PulseLoader': PulseLoader,
            'eventdetails': eventdetails,
          
         


        },
        computed: {
            geteventid: function () {
                if (this.eventidsearch.trim() == "") {
                    return null;
                } else {
                    return this.eventidsearch;
                }
            },
            getevent: function () {
                if (this.event.trim() == "") {
                    return null;
                } else {
                    return this.event;
                }
            },
            getlocation: function () {
                if (this.location == null) {
                    return null;
                }

                if (this.location.value == undefined) {
                    return null;
                } else {
                    return this.location.value;
                }
            },
            getzone: function () {
                if (this.zone == null) {
                    return null;
                }
                if (this.zone.value == undefined) {
                    return null;
                } else {
                    return this.zone.value;
                }
            },
            getorientation: function () {
                if (this.orientation == null) {
                    return null;
                }
                if (this.orientation.value == undefined) {
                    return null;
                } else {
                    return this.orientation.value;
                }
            },
        },
        methods: {
            changeComponent: function () {
                this.$root.currentevents = "addevent";
                
            },
            search: function () {
                this.loading = true;
                this.details = false;
                var data = {
                    eventid: this.geteventid,
                    evento: this.getevent,
                    location: this.getlocation,
                    zone: this.getzone,
                    orientation: this.getorientation,
                };
                var evento = JSON.stringify(data);
                axios.post('/Content/management/SearchEvents', {
                    data: evento,

                }).then(response => {
                    this.loading = false;
                    this.hotels = response.data;
                    this.countrows = this.hotels.length;
                   // this.orient = response.data[1][0];

                });
                console.log(evento);

            },
            clear: function () {
                this.eventidsearch = "";
                this.event = "";
                this.orientation = [];
                this.zone = [];
                this.location = []
            },
            showDetails: function (key) {
                this.details = false;
                if (this.tempkey !== null) {
                    this.tempkey._rowVariant = "";
                } 
                key._rowVariant = "primary";
                this.tempkey = key;
                this.eventid = key.id;
                this.details = true;
                
                bus.$emit('update', {
                    id: key.id
                });
                console.log(this.eventid);
                this.$forceUpdate();
                
                
                
            },
            update: function () {
                this.$forceUpdate();
            },
            getEvents: function () {
                this.loading = true;
                axios.post('/Content/management/GetEvents', {
                    type: 2,

                }).then(response => {
                    this.loading = false;
                    this.hotels = response.data[0][0];
                    this.countrows = this.hotels.length;
                    this.orient = response.data[1][0];
                   
                });
            },
            updateOrientations: function () {
               
                axios.post('/Content/management/GetEvents', {
                    type: 2,

                }).then(response => {
                    console.log(response.data[1][0]);
                    this.orient = response.data[1][0];

                });
            },
            getOrientations: function () {
                axios.get('/Content/management/GetOrientations', {

                }).then(response => {

                    this.allorientations = response.data[1][0];
                });
            },
            getAllPlaces: function () {
                axios.get('/Content/management/GetAllPlaces', {

                }).then(response => {

                    this.zones = response.data[1][0];
                    this.locations = response.data[0][0];

                    });
               
            },
            
          
        },

        mounted() {
            this.getEvents();
            this.getOrientations();
            this.getAllPlaces();
            bus.$on('updateevent', obj => {
                if (obj.response == "update") {
                    console.log("eso");
                    this.tempkey.evento = obj.event;
                    this.tempkey.place = obj.place;
                    this.tempkey.zone = obj.zone;
                    this.updateOrientations();
                   
                }
            });
            bus.$on('deleteevent', obj => {
                this.getEvents();
                console.log("delete event");
                this.details = false;
            });

            },
        }

</script>
<style>
    .hand {
        cursor: pointer;
    }
</style>