<template>
    <div>
        <h1>Report</h1>
        <div class="row">          
            <div class="col-sm-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="row">
                                    <div class="col-sm-6 col-md-6 col-lg-2">
                                        <span>From </span>
                                        <div class="form-group">
                                            <div class="input-group date" id="datetimepicker" data-target-input="nearest">
                                                <input type="text" v-on:change="calendarInput" id="calendario" class="form-control datetimepicker-input" data-target="#datetimepicker"  />
                                                <div class="input-group-append" data-target="#datetimepicker" data-toggle="datetimepicker">
                                                    <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-2">
                                        <span>To </span>
                                        <div class="form-group">
                                            <div class="input-group date" id="datetimepicker2" data-target-input="nearest">
                                                <input type="text" v-on:change="calendarInput" id="calendarioto" class="form-control datetimepicker-input" data-target="#datetimepicker2"  />
                                                <div class="input-group-append" data-target="#datetimepicker2" data-toggle="datetimepicker">
                                                    <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-3">

                                        <button class="btn btn-primary mt-4" @click="searchByDate">Get</button>
                                    </div>
                                    <div class="col-sm-12 text-center m-3" v-if="loading">
                                        <pulse-loader color="#31A3DD"></pulse-loader>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-12" v-if="showtable">
                                <div>
                                    <div style="font-size: 15px;font-weight: bold;">Polls Contact Info: {{pollsNMCI}} </div>
                                    <div style="font-size: 15px;font-weight: bold;">Polls No Contact Info: {{totalPollsDate_nci}}</div>
                                    <div style="font-size: 15px;font-weight: bold;">Polls with Free Trial: {{totalFreeTrial}}</div>
                                    <div style="font-size: 15px;font-weight: bold;">Polls with Subscription: {{totalPollsSubscription}}</div>
                                    <div style="font-size: 15px;font-weight: bold;">Direct Subscription: {{totalDirectSubscriptions}}</div>
                                </div>

                                <b-table striped hover :items="lead_ambassadors" :per-page="perPage" :current-page="currentPage" :fields="fields">
                                    <template slot="name" slot-scope="row" v-if="row.item.id !== '81f3e5c7-08dd-4949-9332-884f61b9cdee'">
                                        <div>
                                            <div class="text-center">
                                                <span>  {{row.item.name}}</span>
                                            </div>
                                        </div>
                                    </template>
                                    <template slot="ambassador" slot-scope="row" v-if="row.item.id !== '81f3e5c7-08dd-4949-9332-884f61b9cdee' ">
                                        <div class="text-center" v-for="ambassador in ambassadorsFilter" v-if="row.item.id == ambassador.supervisorid  ">
                                            {{ambassador.name}}<hr />
                                        </div>
                                        <div class="text-center" v-for="ambassador in leadsambassadors" v-if="row.item.id == ambassador.ambassadorid">
                                            {{ambassador.name}}<hr />
                                        </div>
                                    </template>
                                    <template slot="cards" slot-scope="row" v-if="row.item.id !== '81f3e5c7-08dd-4949-9332-884f61b9cdee' ">
                                        <div class="text-center" v-for="ambassador in ambassadorsFilter" v-if="row.item.id == ambassador.supervisorid ">
                                            {{ambassador.memberbshipscount}}<hr />
                                        </div>
                                        <div class="text-center" v-for="ambassador in leadsambassadors" v-if="row.item.id == ambassador.ambassadorid">
                                            {{ambassador.memberbshipscount}}<hr />
                                        </div>
                                    </template>
                                    <template slot="polls" slot-scope="row" v-if="row.item.id !== '81f3e5c7-08dd-4949-9332-884f61b9cdee' ">
                                        <div class="text-center" v-for="ambassador in ambassadorsFilter" v-if="row.item.id == ambassador.supervisorid  ">
                                            {{ambassador.surveyscount}}<hr />
                                        </div>
                                        <div class="text-center" v-for="ambassador in leadsambassadors" v-if="row.item.id == ambassador.ambassadorid">
                                            {{ambassador.surveyscount}}<hr />
                                        </div>
                                        
                                    </template>
                                    <template slot="polls_nci" slot-scope="row" v-if="row.item.id !== '81f3e5c7-08dd-4949-9332-884f61b9cdee' ">
                                        <div class="text-center" v-for="ambassador in ambassadorsFilter" v-if="row.item.id == ambassador.supervisorid  ">
                                            {{ambassador.surveyscount_nci}}<hr />
                                        </div>
                                        <div class="text-center" v-for="ambassador in leadsambassadors" v-if="row.item.id == ambassador.ambassadorid">
                                            {{ambassador.surveyscount_nci}}<hr />
                                        </div>

                                    </template>
                                    <template slot="total_cards" slot-scope="row" v-if="row.item.id !== '81f3e5c7-08dd-4949-9332-884f61b9cdee'">
                                        <div class="text-center" v-for="total in totalcount" v-if="row.item.id == total.leadid ">
                                            <b>{{total.cards}}</b>
                                        </div>

                                    </template>
                                    <template slot="total_polls" slot-scope="row" v-if="row.item.id !== '81f3e5c7-08dd-4949-9332-884f61b9cdee'">
                                        <div style="height: 100%">
                                            <div class="text-center" v-for="total in totalcount" v-if="row.item.id == total.leadid">
                                                <b>{{total.polls}}</b>
                                            </div>
                                        </div>
                                    </template>
                                    <template slot="total_polls_nci" slot-scope="row" v-if="row.item.id !== '81f3e5c7-08dd-4949-9332-884f61b9cdee'">
                                        <div style="height: 100%">
                                            <div class="text-center" v-for="total in totalcount" v-if="row.item.id == total.leadid">
                                                <b>{{total.polls_nci}}</b>
                                            </div>
                                        </div>
                                    </template>
                                </b-table>
                                <b-pagination size="md" :total-rows="lead_ambassadors.length" v-model="currentPage" :per-page="perPage">
                                </b-pagination>

                                
                            </div>
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



    export default {
        props: ['cards'],
        data: function () {
            return {
                totalDirectSubscriptions:0,
                totalPollsSubscription: 0,
                totalFreeTrial:0,
                pollsNMCI:0,
                totalPollsByDate:0,
                perPage: 5,
                currentPage:1,
                loading: false,
                today: '',
                showtable: false,
                search: {
                    from: '',
                    to:'',
                },
                ambassadors: [],
                totalcount: [],
                lead_ambassadors: [],
                leadsambassadors:[],
                surveys: [],
                fields: [
                    {
                        key: 'name',
                        label: 'Lead Ambassador',
                        sortable: true
                    },
                    {
                        key: 'ambassador',
                        label: 'Ambassador',
                        sortable: true
                    },
                    {
                        key: 'cards',
                        label: 'Memberships',
                        sortable: true
                    },
                    {
                        key: 'polls',
                        label: 'Polls CI',
                        sortable: true
                    },
                    {
                        key: 'polls_nci',
                        label: 'Polls NCI',
                        sortable: true
                    },
                    {
                        key: 'total_cards',
                        label: 'Total Memberships',
                        sortable: true,
                        variant: 'secondary'
                    },
                    {
                        key: 'total_polls',
                        label: 'Total Polls CI',
                        sortable: true,
                        variant: 'secondary'
                    },
                    {
                        key: 'total_polls_nci',
                        label: 'Total Polls NCI',
                        sortable: true,
                        variant: 'secondary'
                    },                  
                ],

            }
        },
        components: {
            'PulseLoader': PulseLoader,


        },
        computed: {
            totalCardsDate: function () {
                var suma = 0
                Array.from(this.ambassadorsFilter).forEach(function (data) {
                    suma = suma + parseInt(data.memberbshipscount);
                });

                return suma;
            },
            totalPollsDate: function () {
                var suma = 0
                var total = 0;
                Array.from(this.ambassadorsFilter).forEach(function (data) {
                    suma = suma + parseInt(data.surveyscount);
                 
                });
                return suma;
            },
            totalPollsDate_nci: function () {
                var suma = 0
                Array.from(this.ambassadorsFilter).forEach(function (data) {
                    suma = suma + parseInt(data.surveyscount_nci);
                });

                return suma;
            },
            ambassadorsFilter: function () {
                function eliminarObjetosDuplicados(arr, prop) {
                    var nuevoArray = [];
                    var lookup = {};

                    for (var i in arr) {
                        lookup[arr[i][prop]] = arr[i];
                    }

                    for (i in lookup) {
                        nuevoArray.push(lookup[i]);
                    }

                    return nuevoArray;
                }

                var duplicadosEliminados = eliminarObjetosDuplicados(this.ambassadors, 'ambassadorid');
                return duplicadosEliminados;

            },
        },
        methods: {
            searchByDate: function () {                              
                this.getInfo();
            },
            calendarInput: function (wrongDate, to) {
                            
                var dia = parseInt(wrongDate[0] + wrongDate[1]);
                var mes = parseInt(wrongDate[3] + wrongDate[4] ) - 1;
                var año = parseInt(wrongDate[6] + wrongDate[7] + wrongDate[8] + wrongDate[9]);             
                if (to) {
                    return new Date(año, mes, dia, 23, 59);
                } else {
                    return new Date(año, mes, dia);
                }
              
            },
            dateFormat: function () {
                var r = this;
                $(function () {
                    $('#datetimepicker').datetimepicker({
                        format: 'DD-MM-YYYY',
                        date: r.setStartDates('start')
                    });
                });

                $(function () {
                    $('#datetimepicker2').datetimepicker({
                        format: 'DD-MM-YYYY',
                        date: r.setStartDates('end')
                    });
                });

            },
            setStartDates: function (type) {
                var date = new Date();
                var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
                var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
                if (type == "start") {
                    return firstDay;
                } else {
                    return lastDay;

                }
            },
            clenFields: function () {
                this.surveys = [];
                this.lead_ambassadors = [];
                this.ambassadors = [];
                this.leadsambassadors = [];
                this.totalcount = [];
                
            },
            getInfo: function () {
                this.loading = true;
                this.clenFields();
                var from = this.calendarInput($('#calendario').val());
                var to = this.calendarInput($('#calendarioto').val(), true);

                axios.post('/crm/reports/GetDataByAmbassador', {
                    fromDate: from,
                    toDate: to

                }).then(response => {
                   
                    
                    this.lead_ambassadors = response.data[0][0];
                    this.ambassadors = response.data[1][0];
                    this.leadsambassadors = response.data[2][0];
                    this.totalcount = response.data[3][0];
                    this.pollsNMCI = response.data[5][0];
                    this.totalFreeTrial = response.data[6][0];
                    this.totalPollsSubscription = response.data[7][0];
                    this.totalDirectSubscriptions = response.data[8][0];
                    //this.totalPollsByDate = response.data[4][0];
                    this.showtable = true;
                    this.loading = false;

                   
                });
            },

        },

        mounted() {
            
            this.dateFormat();
            

        }
    }

</script>
<style>
    .hand {
        cursor: pointer;
    }
    .table td, .table th {
        padding: .75rem;
        vertical-align: inherit;
        border-top: 1px solid #dee2e6;
    }
</style>
