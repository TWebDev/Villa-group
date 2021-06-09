<template>
    <div>

        <div class="row">
            <div class="col-sm-6 text-left ">
                <h2>Survey Management</h2>
            </div>

        </div>
        <div v-if="loading" class="text-center">
            <pulse-loader color="#31A3DD"></pulse-loader>
        </div>
        <div v-else>
            <div class="row">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <!-- <div class="col-sm-4">
                                    <span>Survey name</span>
                                    <input type="text" class="form-control" v-model="survey.name" />
                                </div>
                                <div class="col-sm-12 text-right mb-3">
                                    <button class="btn btn-primary">Search</button>
                                </div>

                                   -->
                                <div class="col-sm-12" v-if="surveys.length > 0">
                                    <b-table @row-clicked="showDetails" class="hand" :per-page="table.perPage" striped hover
                                             :items="surveys" :fields="table.fields" :current-page="table.currentPage">
                                        
                                        <template slot="date" slot-scope="row">
                                            {{getDate(row.item.date)}}
                                        </template>
                                    </b-table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row" v-if="parImpar('par') && par !== 0">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <addsurvey :fieldGroupID="survey.fieldGroupID" :surveyname="survey.name" :description="survey.description"></addsurvey>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row" v-if="parImpar('impar') && par !== 0">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <addsurveys :fieldGroupID="survey.fieldGroupID" :surveyname="survey.name" :description="survey.description"></addsurveys>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import { bus } from './app.js';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    import addsurvey from './addsurvey.vue';
    import addsurveys from './addsurveys.vue';


    export default {
        props: ['cards'],
        data: function () {
            return {
                details: false,
                survey: {
                    fieldGroupID: '',
                    name: '',
                },
                table: {
                    tempkey: [],
                    perPage: 5,
                    currentPage: 1,
                    countrows: 2,
                    fields: [
                        {
                            key: 'name',
                            label: 'Polls Name',
                            sortable: true,
                        },


                        {
                            key: 'username',
                            label: 'Created by',
                            sortable: true,
                        },
                        {
                            key: 'date',
                            label: 'Created On',
                            sortable: true,
                        },
                    ],
                },
                surveys: [],
                par: 0,
                loading: false,
            }
        },
        components: {
            'PulseLoader': PulseLoader,
            'addsurvey': addsurvey,
            'addsurveys': addsurveys

        },
        methods: {
            parImpar: function (type) {
                var tipo = (this.par % 2) ? "impar" : "par";
                if (tipo == type) {
                    console.log(type);

                    return true;
                } else {
                    return false;
                }
            },
            showDetails: function (key) {
                if (this.table.tempkey !== null) {
                    this.table.tempkey._rowVariant = "";
                }
                key._rowVariant = "primary";
                this.table.tempkey = key;
                this.survey.fieldGroupID = key.id;
                this.survey.name = key.name;
                this.survey.description = key.description;
                console.log(key.id);
                this.par = this.par + 1;

                this.$forceUpdate();
            },
            getDate: function (date) {
                var getdate = date
                getdate = getdate.split("/Date(").join("");
                getdate = getdate.split(")/").join("");
                var newdate = new Date(parseInt(getdate));
                var month = newdate.getMonth() + 1;
                return newdate.getFullYear() + "-" + month.toString().padStart(2, "0") + "-" + newdate.getDate().toString().padStart(2, "0");
            },
            changeComponent: function () {
                this.$root.currentpolls = "addsurvey";

            },
            getSurveysGroup: function () {
                this.loading = true;
                axios.get('/Content/management/getSurveysGroup', {

                }).then(response => {
                    this.surveys = response.data;
                    this.loading = false;
                }).catch(function () {
                });;
            },

        },

        mounted() {
            this.getSurveysGroup();
            bus.$on('close', obj => {
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
