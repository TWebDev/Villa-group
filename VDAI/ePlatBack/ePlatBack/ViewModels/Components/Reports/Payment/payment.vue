<template>
    <div>
        <h1>Payments Report</h1>
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
                                <b-table :per-page="perPage.value" :current-page="currentPage" striped hover :items="payments" :fields="fields">
                                    <template slot="date" slot-scope="row">
                                        <div class="text-left">
                                            {{getDate(row.item.date)}}
                                        </div>
                                    </template>
                                    <template slot="amount" slot-scope="row">
                                        <div class="text-left">
                                            ${{row.item.amount | toNumero}}
                                        </div>
                                    </template>
                                </b-table>
                                <div class="row">
                                    <div class="col-sm-2 text-left ">
                                        <b-form-select v-model="perPage.value" :options="options"></b-form-select>
                                    </div>
                                    <div class="col-sm-10 text-right">
                                        <span style="font-size: 1.5rem">Total Amount </span>
                                        <span style="font-size: 1.5rem; font-weight: 900">${{totalamount | toNumero}}</span>
                                    </div>

                                    <div class="col-sm-12 text-center mt-2 ">
                                        <b-pagination size="md" :total-rows="payments.length" v-model="currentPage" :per-page="perPage.value">
                                        </b-pagination>
                                    </div>
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
    import { bus } from './app.js';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
import { parse } from 'path';



    export default {
        props: ['cards'],
        data: function () {
            return {
                options: [
                    { value: 5, text: '5' },
                    { value: 10, text: '10' },
                    { value: 15, text: '15' },
                    { value: 20, text: '20' },
                   
                ],
                perPage: {
                    value: 5,
                    text:'5'
                },
                currentPage: 1,
                totalamount: 0,
                loading: false,
                payments: [],
                showtable: false,
                search: {
                    from: '',
                    to:'',
                },
                    
                fields: [
                    {
                        key: 'member',
                        label: 'Card Holder Name ',
                        sortable: true
                    },
                    {
                        key: 'date',
                        label: 'Payment Date',
                        sortable: true
                    },
                    {
                        key: 'savedByUser',
                        label: 'Charged By User',
                        sortable: true
                    },
                    {
                        key: 'amount',
                        label: 'Amount',
                        sortable: true
                    },
                    {
                        key: 'authcode',
                        label: 'Authcode',
                        sortable: true,
                        
                    },
                   
                ],

            }
        },
        components: {
            'PulseLoader': PulseLoader,


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
                this.payments = [];
                this.totalamount = 0;
                
            },
            getInfo: function () {
                this.loading = true;
                this.clenFields();
                var from = this.calendarInput($('#calendario').val());
                var to = this.calendarInput($('#calendarioto').val(),true);

                axios.post('/crm/reports/GetDataPayments', {
                    fromDate: from,
                    toDate: to

                }).then(response => {
                    var total = 0;
                    this.payments = response.data;
                    Array.from(this.payments).forEach(function (data) {
                        total = total + parseInt( data.amount);
                    });
                    this.totalamount = total;
                    this.loading = false;
                    this.showtable = true;

                   
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
