<template>

    <div>
            <div class="row mb-3">
                <div class="text-left col-sm-8">
                   <h2 style="position: relative">Reviews</h2>
                </div>
                <div class="col-sm-4 text-right">
                   
                </div>
            </div>
            <div v-if="loading" class="text-center">
                <pulse-loader color="#31A3DD"></pulse-loader>
            </div>
            <div v-else>
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                                <b-table class="hand" @row-clicked="showDetails" :per-page="perPage" striped hover
                                         :items="reviews" :fields="fields" :current-page="currentPage">
                                    <template slot="date" slot-scope="row">
                                        {{getDate(row.item.date)}}
                                    </template>
                                    <template slot="status" slot-scope="row">
                                        <div v-if="row.item.status == true">
                                            <button class="btn btn-success" @click="changeStatus(row.item.id,row)">Active</button>
                                        </div>
                                        <div v-else>
                                            <button class="btn btn-danger" @click="changeStatus(row.item.id,row)">Inactive</button>
                                        </div>
                                    </template>
                                </b-table>
                            <b-pagination size="md" :total-rows="countrows" v-model="currentPage" :per-page="perPage">
                            </b-pagination>
                        </div>
                    </div>
                </div>
            </div>


            
        </div>

</template>


<script>

    import axios from 'axios';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
  




    export default {
        props: ['cards'],
            data: function () {
                return {
                    loading: false,
                    perPage: 10,
                    reviews: [],
                    currentPage: 1,
                    countrows: 2,
                    hotelid: null,
                    details: false,


                    fields: [
                        {
                            key: 'name',
                            label: 'Author',
                            sortable: true

                        },
                        {
                            key: 'typeplace',
                            label: 'Type',
                            sortable: true,

                        },
                        {
                            key: 'placename',
                            label: 'Name',
                            sortable: true,

                        },
                        {
                            key: 'date',
                            label: 'Date Published',
                            sortable: true,

                        },
                        {
                            key: 'review',
                            label: 'Review',
                            sortable: true,

                        },
                       
                        {
                            key: 'stars',
                            label: 'Stars',
                            sortable: true,

                        },
                       
                        {
                            key: 'status',
                            label: 'Status',
                            sortable: true,
                           
                        },
                        
                       

                    ],
                    tempkey: null,
                    


                }
        },
        components: {
       
            'PulseLoader': PulseLoader,
         


        },
        methods: {
            getDate: function (fecha) {
                if (fecha !== undefined) {

                    fecha = fecha.split("/Date(").join("");
                    fecha = fecha.split(")/").join("");
                    var dia = new Date(parseInt(fecha)).getDate();
                    var mes = new Date(parseInt(fecha)).getMonth();
                    mes = mes + 1;
                    var año = new Date(parseInt(fecha)).getFullYear();
                    fecha = dia.toString().padStart(2, "0") + "-" + mes.toString().padStart(2, "0") + "-" + año.toString().padStart(2, "0");
                    return fecha;
                }
            },
            changeStatus: function (id,row) {
                axios.post('/Content/management/statusReview', {
                    id: id,
                }).then(response => {
                    console.log(response.data);
                    row.item.status = !row.item.status;

                });
                
            },
            showDetails: function (key) {
                this.details = false;
                if (this.tempkey !== null) {
                    this.tempkey._rowVariant = "";
                } 
               // key._rowVariant = "primary";
                this.tempkey = key;
                this.hotelid = key.id;
                this.details = true;
                
              
                this.$forceUpdate();
                
                
                
            },
            update: function () {
                this.$forceUpdate();
            },
            getReviews: function () {
                this.loading = true;
                axios.post('/Content/management/getReviews', {
                    type: 16,

                }).then(response => {
                    this.loading = false;
                    this.reviews = response.data;
                    this.countrows = this.reviews.length;
                   
                   
                });
            },
            
          
        },

        mounted() {
            this.getReviews();
            bus.$on('updatehotel', obj => {
                console.log("deleted");
                if (obj.response == "deleted") {
                    this.details = false;
                    this.getReviews();

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