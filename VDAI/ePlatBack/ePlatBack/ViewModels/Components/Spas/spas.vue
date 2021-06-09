<template>

    <div>
            <div class="row mb-5">
                <div class="text-left col-sm-8">
                   <h2 style="position: relative">Spas</h2>
                </div>
                <div class="col-sm-4 text-right">
                    <button class="btn btn-primary" @click="changeComponent">
                        <i class="material-icons" style="vertical-align: middle;">
                            add_circle
                        </i>Add Spa
                    </button>
                </div>

            </div>
            <div v-if="loading" class="text-center">
                <pulse-loader color="#31A3DD"></pulse-loader>
            </div>
            <div v-else>
                <div class="card">
                    <div class="card-body">
                        <div class="row">

                                <b-table  class="hand" @row-clicked="showDetails" :per-page="perPage" striped hover
                                         :items="hotels" :fields="fields" :current-page="currentPage">
                                    <template slot="orientations" slot-scope="row">
                                        <div v-for="ori in orient" v-if="row.item.id == ori.placeid">
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


            <div v-if="details" class="mt-2">
               <spadetails :id="hotelid"></spadetails>
            </div>
        </div>

</template>


<script>
    import { bus } from './app.js';
    import spadetails from './spadetails.vue';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
  




    export default {
        props: ['cards'],
            data: function () {
                return {
                    loading: false,
                    perPage: 5,
                    hotels: [],
                    currentPage: 1,
                    countrows: 2,
                    hotelid: null,
                    details: false,
                    orient: '',
                    
                    fields: [
                        {
                            key: 'name',
                            label: 'Restaurant Name',
                            sortable: true

                        },
                        {
                            key: 'destination',
                            label: 'Destination',
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
                    


                }
        },
        components: {
       
            'PulseLoader': PulseLoader,
            'spadetails': spadetails
         


        },
        methods: {
            changeComponent: function () {
                this.$root.currentspa = "addspa";
                
            },
            showDetails: function (key) {
                this.details = false;
                if (this.tempkey !== null) {
                    this.tempkey._rowVariant = "";
                } 
                key._rowVariant = "primary";
                this.tempkey = key;
                this.hotelid = key.id;
                this.details = true;
                
                bus.$emit('update', {
                    id: key.id
                });
                this.$forceUpdate();
                
                
                
            },
            update: function () {
                this.$forceUpdate();
            },
            getHotels: function () {
                this.loading = true;
                axios.post('/Content/management/getPlaces', {
                    type: 16,

                }).then(response => {
                    this.loading = false;
                    this.hotels = response.data[0][0];
                    this.countrows = this.hotels.length;
                    this.orient = response.data[1][0];
                   
                });
            },
            
          
        },

        mounted() {
            this.getHotels();
            bus.$on('updatehotel', obj => {
                console.log("deleted");
                if (obj.response == "deleted") {
                    this.details = false;
                    this.getHotels();

                }
                if (obj.response == "updated") {

                    console.log("el obj es " + this.tempkey);
                    this.tempkey.name = obj.hotelname;
                    this.tempkey.destination = obj.destination;
                    this.tempkey.zone = obj.zone;
                   
                    axios.get('/Content/management/getPlacesOrientationsUpdated', {
                       
                    }).then(response => {
                        this.orient = response.data;
                        
                    });
                }
            });

            },
        }

</script>
<style>
    .hand {
        cursor: pointer;
    }
</style>
