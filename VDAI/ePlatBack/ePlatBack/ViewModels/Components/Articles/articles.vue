<template>

    <div>
        
        <div class="mb-5">
            <div class="text-center">
                <i class="fa fa-pencil-square-o fa-2x" aria-hidden="true"></i> <h3 style="position: relative">Articles</h3>
            </div>
            <div class="text-right">
                <button class="btn btn-primary" @click="changeComponent">
                    <i class="material-icons" style="vertical-align: middle;">
                        add_circle
                    </i>Add Article
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
                                <h5>Search Articles</h5>
                            </div>
                            <div class="row">
                                <div class="col-sm-6 col-md-6 col-lg-2">
                                    <span>Article ID</span>
                                    <input class="form-control" type="number" v-model="sArticleid">
                                </div>
                                <div class="col-sm-6 col-md-6 col-lg-4">
                                    <span>Article </span>
                                    <input class="form-control" type="text" v-model="sTitle">
                                </div>
                                <div class="col-sm-6 col-md-6 col-lg-3">
                                    <span>Status</span>
                                    <v-select v-model="sStatus" :options="activeinactive"></v-select>
                                </div>
                                <div class="col-sm-6 col-md-6 col-lg-3">
                                    <span>Orientation</span>
                                    <v-select v-model="sOrientations" :options="orientations"></v-select>
                                </div>
                            </div>

                            <div class="mt-2 text-right">
                                <button class="btn btn-secondary" @click="clear">Clear</button>
                                <button class="btn btn-primary" @click="search">Search</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-sm-12 mb-3">
                    <div class="card">
                        <div class="card-body ">
                            <b-table class="hand" @row-clicked="showDetails" :per-page="perPage" striped hover v-scroll-to="'#details'"
                                     :items="articles" :fields="fields" :current-page="currentPage">
                                <template slot="status" slot-scope="row">
                                    <div v-if="row.item.status == false || row.item.status == 0">

                                        Inactive
                                    </div>
                                    <div v-else>

                                        Active
                                    </div>
                                </template>
                                <template slot="dateSaved" slot-scope="row">
                                    <div>
                                        {{cleanDate(row.item.dateSaved)}}
                                    </div>
                                </template>
                                <template slot="publicationDate" slot-scope="row">
                                    <div>
                                        {{cleanDate(row.item.publicationDate)}}
                                    </div>
                                </template>
                                <template slot="orientations" slot-scope="row">
                                    <div v-for="ori in orient" v-if="row.item.pageid == ori.itemid">
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
                    </div>

                    <div id="details">
                        <div v-if="details">
                            <articledetails :id="articleid"></articledetails>
                        </div>
                    </div>
                </div>

</template>


<script>
    import { bus } from './app.js';

    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    import articledetails from './articledetails.vue';
    



    export default {
        props: ['cards'],
            data: function () {
                return {
                    loading: false,
                    perPage: 10,
                    articles: [],
                    sStatus: null,
                    sTitle: '',
                    sArticleid: null,
                    sOrientations: null,
                    orientations: [],
                    activeinactive: [
                        {
                            value: true,
                            label: "Active"
                        },
                        {
                            value: false,
                            label: "Inactive"
                        }
                    ],
                    currentPage: 1,
                    countrows: 2,
                    articleid: null,
                    details: false,
                    orient: '',
                    loadingdetails: false,

                    fields: [
                        {
                            key: 'pageid',
                            label: 'Article ID',
                            sortable: true

                        },
                        {
                            key: 'title',
                            label: 'Article',
                            sortable: true

                        },
                        {
                            key: 'dateSaved',
                            label: 'Date Created',
                            sortable: true,

                        },
                        {
                            key: 'publicationDate',
                            label: 'Publication Date',
                            sortable: true,

                        },
                        {
                            key: 'status',
                            label: 'Status',
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
            'articledetails': articledetails,


        },
        methods: {
            cleanDate: function (d) {
                if (d == undefined) return "";
                var getdate = d.split("/Date(").join("");
                getdate = getdate.split(")/").join("");                
                var date = new Date(parseInt(getdate));
                var dia = date.getUTCDate();
                var mes = date.getUTCMonth();
                var anio = date.getUTCFullYear();
                var fecha = new Date(anio, mes, dia);
                var month = fecha.getUTCMonth() + 1;
                return month.toString().padStart(2, "0") + "-" + fecha.getUTCDate().toString().padStart(2, "0") + "-" + fecha.getUTCFullYear();
            },
            clear: function () {
                this.sArticleid = null;
                this.sTitle = '';
                this.sOrientations = null;
                this.sStatus = null;
                this.getArticles();
                this.getData();
                
            },
            search: function () {

                if (this.sStatus == null) {
                    var status = null;
                } else {
                    var status = this.sStatus.value;
                }
                if (this.sOrientations == null) {
                    var ori = null;
                } else {
                    var ori = this.sOrientations.value;
                }

                var busqueda = {
                    articleid: this.sArticleid,
                    article: this.sTitle,
                    status: status,
                    orientation: ori
                };
                busqueda = JSON.stringify(busqueda);

                axios.post('/Content/management/searchArticles', {
                    data: busqueda
                }).then(response => {
                    console.log("search: " + response.data);
                    if (response.data !== false) {
                        this.articles = response.data;
                        this.countrows = this.articles.length;                     
                    }                                   
                });

                this.details = false;
                console.log(busqueda);

            },
            changeComponent: function () {
                this.$root.currentarticles = "addarticle";
                
            },
            showDetails: function (key) {
                this.details = false;
                if (this.tempkey !== null) {
                    this.tempkey._rowVariant = "";
                } 
                key._rowVariant = "primary";
                this.tempkey = key;
                this.articleid = key.pageid;
                
                bus.$emit('updatearticle', {
                    id: this.tempkey.pageid
                });
                this.$forceUpdate();
                
                
                this.details = true;
            },
            update: function () {
                this.$forceUpdate();
            },
            getArticles: function () {
                this.loading = true;
                axios.get('/Content/management/getArticles', {
                   
                }).then(response => {
                    this.loading = false;
                    this.articles = response.data;
                    this.countrows = this.articles.length;
                    console.log("count " + this.articles.length);
                   
                });
            },
            getData: function () {

                axios.get('/Content/management/getDataPlaces', {


                }).then(response => {
                    this.orientations = response.data[2][0];
                    var arreglo = [];
                    Array.from(this.orientations).forEach(function (data) {
                        arreglo.push({
                            label: data.text,
                            value: data.value
                        });

                    });
                    this.orientations = arreglo;


                });
            },
            getOrientations: function () {
                axios.post('/Content/management/getItemsOrientationsUpdated', {
                    itemid: 9,

                }).then(response => {
                    this.orient = response.data;

                });
            },
            
          
        },

        mounted() {
            this.getArticles();
            this.getOrientations();
            this.getData();
            bus.$on('updatearticle', obj => {
                console.log("deleted");
                if (obj.response == "deleted") {
                    this.details = false;
                    this.getArticles();

                }
                if (obj.response == "updated") {
                    this.tempkey.title = obj.articlename;
                    this.tempkey.status = obj.status;
                    this.getOrientations();

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