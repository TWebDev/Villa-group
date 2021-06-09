<template>

    <div>
       
        <div class="row">
            <div class="col-sm-4">
                <h1>Polls from Senses Of México program</h1>
            </div>
            <div class="col-sm-2 text-left d-none d-print-inline">
                <img src="/Content/images/puertologo.png"  class="img-fluid" alt="Alternate Text" />
            </div>
            <div class="col-sm-2 text-right d-none d-print-inline">
                <img src="/Content/images/logovallarta.png" class="img-fluid" alt="Alternate Text" />
            </div>
            <div class="col-sm-2 text-right d-none d-print-inline">
                <img src="/Content/images/turismologo.png"  class="img-fluid" alt="Alternate Text" />
            </div>
            <div class="col-sm-2 text-right d-none d-print-inline">
                <img src="/Content/images/logosenses.jpg"  class="img-fluid" alt="Alternate Text" />
            </div>
        </div>
        <div class="row">
            <div class="col-sm-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="row d-print-none">
                                    <div class="col-sm-6 col-md-4 col-lg-2">
                                        <b-form-group label="Select Poll">

                                            <b-form-checkbox-group id="checkbox-group-1"
                                                                   v-model="pollType"
                                                                   :options="pollTypes"
                                                                   name="flavour-1"></b-form-checkbox-group>
                                        </b-form-group>
                                    </div>
                                    <div class="col-sm-6 col-md-4 col-lg-2">
                                        <span>From </span>
                                        <div class="form-group">
                                            <div class="input-group date" id="datetimepicker3" data-target-input="nearest">
                                                <input type="text" v-on:change="calendarInput" id="calendario3" class="form-control datetimepicker-input" data-target="#datetimepicker3" />
                                                <div class="input-group-append" data-target="#datetimepicker3" data-toggle="datetimepicker">
                                                    <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-sm-6 col-md-4 col-lg-2">
                                        <span>To </span>
                                        <div class="form-group">
                                            <div class="input-group date" id="datetimepicker4" data-target-input="nearest">
                                                <input type="text" v-on:change="calendarInput" id="calendarioto4" class="form-control datetimepicker-input" data-target="#datetimepicker4" />
                                                <div class="input-group-append" data-target="#datetimepicker4" data-toggle="datetimepicker">
                                                    <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-4 col-lg-2 ">
                                        <span><strong>Location</strong></span>
                                        <v-select v-model="location" :options="locations"></v-select>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-3">
                                        <button class="btn btn-primary mt-4" @click="searchByDate">Get</button>
                                    </div>
                                    <div class="col-sm-12 text-center m-3" v-if="loading">
                                        <pulse-loader color="#31A3DD"></pulse-loader>
                                    </div>
                                </div>
                            </div>
                            <div class="row" v-if="totalSurveys > 0">
                                <div class="col-sm-12 m-4 ">

                                    <div class="row d-print-flex">
                                        <div class="col-sm-2 text-center">

                                        </div>
                                        <div class="col-sm-2 text-center">
                                            <label style="font-weight: 400;"><b>Polls Found</b></label><br />
                                            <span>{{totalSurveys}}</span>
                                        </div>
                                        <div class="col-sm-2 text-center">
                                            <label> <b>From</b></label><br />
                                            <span style="font-weight: 400;">{{fromReport}}</span>
                                        </div>
                                        <div class="col-sm-2 text-center">
                                            <label><b>To</b></label><br />
                                            <span style="font-weight: 400;">{{toReport}}</span>
                                        </div>
                                        <div class="col-sm-2 text-center">
                                            <label><b>Location</b></label><br />
                                            <span style="font-weight: 400;">{{location.label}}</span>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6 text-center" v-if="chartData1.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart1}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData1"
                                            :options="chartOptions1" />
                                </div>
                                <div class="col-sm-6 text-center" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart2}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData2"
                                            :options="chartOptions1" />
                                </div>
                                <div class="col-sm-6 text-center" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart3}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData3"
                                            :options="chartOptions1" />
                                </div>
                                <div class="col-sm-6 text-center" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart4}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData4"
                                            :options="chartOptions1" />
                                </div>
                                <div class="col-sm-6 text-center mt-5" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart5}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData5"
                                            :options="chartOptions1" />
                                </div>
                                <div class="col-sm-6 text-center mt-5" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart6}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData6"
                                            :options="chartOptions1" />
                                </div>
                                <div class="col-sm-6 text-center mt-5" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart7}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData7"
                                            :options="chartOptions1" />
                                </div>
                                <div class="col-sm-6 text-center mt-5" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart8}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData8"
                                            :options="chartOptions1" />
                                </div>

                                <div class="col-sm-6 text-center mt-5" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart9}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData9"
                                            :options="chartOptions1" />
                                </div>
                                <div class="col-sm-6 text-center mt-5" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart10}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData10"
                                            :options="chartOptions1" />
                                </div>
                                <div class="col-sm-6 text-center mt-5" v-if="chartData2.length > 0">
                                    <h5 style="font-weight: 300">{{titleChart11}}</h5>
                                    <GChart type="PieChart"
                                            :data="chartData11"
                                            :options="chartOptions1" />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
   // import { Chart } from 'highcharts-vue'
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'

    export default {
        props: ['cards'],
        data: function () {
            return {
                totalSurveys: 0,
                toReport: null,
                fromReport: null,
                pollTypes: [
                    {
                        value: 'english',
                        text: 'English'
                    },
                    {
                        value: 'spanish',
                        text: 'Spanish'
                    }
                ],
                pollType:['english'],
                resultStatistics: [],
                valuesStatistics: [],
                titleChart1:'',
                chartData1: [],
                titleChart2: '',
                chartData2: [],
                titleChart3: '',
                chartData3: [],
                titleChart4: '',
                chartData4: [],
                titleChart5: '',
                chartData5: [],
                titleChart6: '',
                chartData6: [],
                titleChart7: '',
                chartData7: [],
                titleChart8: '',
                chartData8: [],
                titleChart9: '',
                chartData9: [],
                titleChart10: '',
                chartData10: [],
                titleChart11: '',
                chartData11: [],
                chartOptions1: {
                    hAxis: {
                        textStyle: {
                            fontSize: 20 // or the number you want
                        }

                    },
                    width: 'auto',
                    height: 500,
                    is3D: true,
                    chart: {
                        title: 'Company Performance',
                        subtitle: 'Sales, Expenses, and Profit: 2014-2017',
                    }
                },
                locations: [],
                location: {
                    label: 'All Locations',
                    value: 0,
                },
                loading: false,
                search: {
                    from: '',
                    to:'',
                },
            }
        },
        components: {
            'PulseLoader': PulseLoader,
            'vSelect': vSelect,
           
        },
        computed: {
            cFrom() {
                return $('#calendario3').val();
            },
            cTo() {
                return $('#calendarioto4').val();
            },
            cPollType() {
                if (this.pollType.length == 2) {
                    return "both";
                } else if (this.pollType.length == 1) {
                    return this.pollType[0];
                } else {
                    return "english"
                }

            },
        },
        filters: {
            toNumero(value) {
                let val = (value / 1).toFixed(0).replace(',', ',');
                return val.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
            },
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
                    $('#datetimepicker3').datetimepicker({
                        format: 'DD-MM-YYYY',
                        date: r.setStartDates('start')
                    });
                });
                $(function () {
                    $('#datetimepicker4').datetimepicker({
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
         
            getLocations: function () {
                axios.get('/Content/management/GetLocations', {

                }).then(response => {
                    var arr = [];
                    this.locations = response.data;
                    this.locations.unshift({
                        label: 'All Locations',
                        value: 0,
                    });
                }).catch(function () {
                });
            },
            getInfo: function () {
                this.totalSurveys = 0;
                this.loading = true;             
                var from = this.calendarInput($('#calendario3').val());
                var to = this.calendarInput($('#calendarioto4').val(), true);
                var location = null;
                if (this.location !== null) {
                    location = this.location.value;
                }
                axios.post('/crm/reports/GetPollsStatistics', {
                    fromDate: from,
                    toDate: to,
                    locationID: location,
                    pollType: this.cPollType

                }).then(response => {
                    let self = this;
                    //question 1
                    var per = 0;
                    var total = response.data[0][7];
                    this.chartData1 = [];
                    this.chartData1.push(['Answer', 'Count', 'Question']);      
                    Array.from(response.data[0][0]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][12]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer , data.count, data.question);
                        self.titleChart1 = data.question;
                        self.chartData1.push(nuevo);
                    });
                    //question 2
                    this.chartData2 = [];
                    this.chartData2.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][1]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][13]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer, data.count, data.question);
                        self.titleChart2 = data.question;
                        self.chartData2.push(nuevo);
                    });
                    //question 3
                    this.chartData3 = [];
                    this.chartData3.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][2]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][14]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer, data.count, data.question);
                        self.titleChart3 = data.question;
                        self.chartData3.push(nuevo);
                    });
                    //question 4
                    this.chartData4 = [];
                    this.chartData4.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][3]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][15]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer, data.count, data.question);
                        self.titleChart4 = data.question;
                        self.chartData4.push(nuevo);
                    });
                    //question 5 
                    this.chartData5 = [];
                    this.chartData5.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][4]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][16]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer, data.count, data.question);
                        self.titleChart5 = data.question;
                        self.chartData5.push(nuevo);
                    });
                    //question 6
                    this.chartData6 = [];
                    this.chartData6.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][5]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][17]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer, data.count, data.question);
                        self.titleChart6 = data.question;
                        self.chartData6.push(nuevo);
                    });
                    //question 7
                    this.chartData7 = [];
                    this.chartData7.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][6]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][18]) * 100).toFixed(0);                
                        nuevo.push(per + '%' + ' ' + data.answer , data.count, data.question);
                        self.titleChart7 = data.question;
                        self.chartData7.push(nuevo);
                    });

                    //question 8
                    this.chartData8 = [];
                    this.chartData8.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][7]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][19]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer, data.count, data.question);
                        self.titleChart8 = data.question;
                        self.chartData8.push(nuevo);
                    });
                    //question 9
                    this.chartData9 = [];
                    this.chartData9.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][8]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][20]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer, data.count, data.question);
                        self.titleChart9 = data.question;
                        self.chartData9.push(nuevo);
                    });
                    //question 10
                    this.chartData10 = [];
                    this.chartData10.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][9]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][21]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer, data.count, data.question);
                        self.titleChart10 = data.question;
                        self.chartData10.push(nuevo);
                    });
                    //question 111
                    this.chartData11 = [];
                    this.chartData11.push(['Answer', 'Count', 'Question']);
                    Array.from(response.data[0][10]).forEach(function (data) {
                        var nuevo = [];
                        per = (parseInt(data.count) / parseInt(response.data[0][22]) * 100).toFixed(0);
                        nuevo.push(per + '%' + ' ' + data.answer, data.count, data.question);
                        self.titleChart11 = data.question;
                        self.chartData11.push(nuevo);
                    });
                   
                    this.totalSurveys = response.data[0][11];
                   
                    //this.chartData.values = arr;
                    this.loading = false;
                    this.showtable = true;
                    this.fromReport = moment(from).format('DD-MM-YYYY');
                    this.toReport = moment(to).format('DD-MM-YYYY');
                });
            },

        },

        mounted() {
            this.getLocations();
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
    .custom-control-input {
        margin-top: 6px;
        position: absolute;
        z-index: 5;
        opacity: 19;
        margin-left: -18px;
    }

    .custom-control {
        position: relative;
        display: inline !important;
        min-height: 1.5rem;
        padding-left: 1.5rem;
    }
</style>
