<template>
    <div>
        <div class="row">
            <div class="col-sm-6 text-left ">
                <h2>Memberships Report</h2>

            </div>
        </div>
        <div class="row">
            <div class="col-sm-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-sm-6 col-md-4 col-lg-2">
                                <span>Code</span>
                                <input class="form-control" type="text" v-model="code">
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
                            <div class="col-sm-6 col-md-6 col-lg-3">
                                <button class="btn btn-primary mt-4" @click="getMemberships">Get</button>
                            </div>
                            <div class="col-sm-12" v-if="membershipsByDate.length > 0">

                                <div class="col-sm-12 filtercss">
                                    <div class="row">
                                        <div class="col-sm-12 text-center">
                                            <h4>Filters </h4>
                                        </div>
                                        <div class="col-sm-3">
                                            <b-form-group label="Paid">
                                                <b-form-radio-group v-model="filters.paid"
                                                                    :options="paidOptions"
                                                                    name="Paid"></b-form-radio-group>
                                            </b-form-group>
                                        </div>
                                        <div class="col-sm-3">
                                            <b-form-group label="Payment Type">
                                                <b-form-radio-group v-model="filters.paymentType"
                                                                    :options="paymentTypeOptions"
                                                                    name="Payment"></b-form-radio-group>
                                            </b-form-group>
                                        </div>
                                        <div class="col-sm-3">
                                            <b-form-group label="Delivered By">
                                                <b-form-radio-group v-model="filters.activated"
                                                                    :options="deliveredOptions"
                                                                    name="Delivered"></b-form-radio-group>
                                            </b-form-group>
                                        </div>
                                        <div class="col-sm-3">
                                            <b-form-group label="Status">
                                                <b-form-radio-group v-model="filters.status"
                                                                    :options="statusOptions"
                                                                    name="expired"></b-form-radio-group>
                                            </b-form-group>
                                        </div>

                                    </div>
                                </div>
                            </div>





                            <div v-if="loadingSearch" class=" col-sm-12 m-3 text-center">
                                <pulse-loader color="#31A3DD"></pulse-loader>
                            </div>
                            <!--Tabla de Resultados-->
                            <div class="col-sm-12" v-if="showtable">
                                <span style="font-size: 20px;font-weight: 500;" v-if="membershipsByDate.length > 0">Result: {{cMembershipsByDate.length}} <span v-if="cMembershipsByDate.length > 1">Memberships.</span> <span v-else>Membership.</span></span>
                                <b-table v-scroll-to="'#details'" @row-clicked="getContactInfo" class="hand" :per-page="tableResults.perPage" striped hover
                                         :items="cMembershipsByDate" :fields="tableResults.fields" :current-page="tableResults.currentPage">
                                    <template slot="dateSaved" slot-scope="row">
                                        {{getDate(row.item.dateSaved)}}
                                    </template>
                                    <template slot="dateActivated" slot-scope="row">
                                        {{getDate(row.item.dateActivated)}}
                                    </template>
                                    <template slot="dueDate" slot-scope="row">
                                        {{getDate(row.item.dueDate)}}
                                    </template>
                                    <template slot="paymentTypeID" slot-scope="row">
                                        <span v-if="row.item.paymentTypeID == 5">
                                            Credit Card
                                        </span>
                                        <span v-if="row.item.paymentTypeID == 10">
                                            Clip
                                        </span>
                                        <span v-if="row.item.paymentTypeID == 9">
                                            Cash
                                        </span>
                                    </template>
                                    <template slot="isProcess" slot-scope="row">
                                        <div class="row">
                                            <div class="col-sm-12 text-center" v-if="row.item.paymentTypeID !== null">
                                                <i class="material-icons" style="color:forestgreen">
                                                    check_box
                                                </i>
                                            </div>
                                            <div v-else class="col-sm-12 text-center">
                                                <i class="material-icons" style="color:red">
                                                    indeterminate_check_box
                                                </i>
                                            </div>
                                        </div>
                                    </template>
                                    <template slot="activatedByPoll" slot-scope="row">
                                        <div class="row">
                                            <div class="col-sm-12 text-center" v-if="row.item.activatedByPoll">
                                                <i class="material-icons" style="color:forestgreen">
                                                    check_box
                                                </i>
                                            </div>
                                            <div v-else class="col-sm-12 text-center">
                                                <i class="material-icons" style="color:red">
                                                    indeterminate_check_box
                                                </i>
                                            </div>
                                        </div>
                                    </template>
                                </b-table>
                                <b-pagination size="md" :total-rows="cMembershipsByDate.length" v-model="tableResults.currentPage" :per-page="tableResults.perPage">
                                </b-pagination>
                            </div>
                            <div v-if="loadingDetails" class=" col-sm-12 m-3 text-center">
                                <pulse-loader color="#31A3DD"></pulse-loader>
                            </div>
                            <!--contact info details-->
                            <div id="details" class="col-sm-12 text-center card">
                               
                                <div class="row card-body" v-show="showDetails">
                                    <div class="col-sm-12 text-center">
                                        <h4>Contact Info</h4>
                                    </div>
                                    <div class="col-sm-8">
                                        <div class="row">
                                            <div class="col-sm-2">
                                                <strong>Membership Code</strong>
                                                <div>
                                                    {{member.code}}
                                                </div>
                                            </div>
                                            <div class="col-sm-2">
                                                <strong>Name</strong>
                                                <div>
                                                    {{member.name}}
                                                </div>
                                            </div>
                                            <div class="col-sm-2">
                                                <strong>Phone</strong>
                                                <div>
                                                    {{member.phone}}
                                                </div>
                                            </div>
                                            <div class="col-sm-2">
                                                <strong>Email</strong>
                                                <div>
                                                    {{member.email}}
                                                </div>
                                            </div>
                                            <div class="col-sm-2">
                                                <strong>Poll's Folio</strong>
                                                <div>
                                                    {{member.folio}}

                                                </div>
                                            </div>
                                           
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="col-12">
                                            <b>Due Date </b>
                                            <div class="">
                                                <div class="input-group date" id="datetimepicker6" data-target-input="nearest">
                                                    <input v-model="member.dueDate" type="text" v-on:change="calendarInput" id="calendario6" class="form-control datetimepicker-input" data-target="#datetimepicker6" />
                                                    <div class="input-group-append" data-target="#datetimepicker6" data-toggle="datetimepicker">
                                                        <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-s12  p-3">
                                            <label> <b>Booking Status</b></label>
                                            <b-form-select v-model="member.bookingStatus" :options="bookingStatusOptions"></b-form-select>
                                        </div>
                                        <div class="col-12 ">
                                            <textarea v-model="comment" class="form-control" id="exampleFormControlTextarea1" rows="10"></textarea>
                                            <button class="btn btn-primary mt-2" @click="saveComment(member.key.membershipSalesID)"> Save </button>
                                        </div>
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
import { freemem } from 'os';



    export default {
        props: ['cards'],
        data: function () {
            return {
                bookingStatusOptions: [],
                comment:'',
                loadingDetails: false,
                showDetails: false,
                code:'',
                showtable: false,
                loadingSearch: false,
                membershipsByDate: [],
                deliveredOptions: [
                    { text: 'All', value: 1 },
                    { text: 'Poll', value: null },
                    { text: 'Paid', value: true },
                ],
                paidOptions: [
                    { text: 'All', value: 1 },
                    { text: 'Yes', value: true },
                    { text: 'Free', value: 'Free' },
                    { text: 'No', value: null },
                ],
                paymentTypeOptions: [
                    { text: 'All', value: 1 },
                    { text: 'Credit Card', value: 5 },
                    { text: 'Clip', value: 22 },
                    { text: 'Cash', value: 9 },
                ],
                statusOptions: [
                    { text: 'All', value: 1 },
                    { text: 'Active', value: true },
                    { text: 'Expired', value: false },
                ],
                filters: {
                    paid: 1, 
                    activated: 1,
                    status: 1,
                    paymentType: 1,
                    free: true,
                    
                },
                member: {
                    name: '',
                    key: null,
                    code: 0,
                    email: '',
                    phone: '',
                    folio: '',
                    bookingStatus: '',
                    dueDate: null,
                    
                },
                tableResults: {
                    tempkey: [],
                    perPage: 15,
                    currentPage: 1,
                    countrows: 2,
                    
                    fields: [
                        {
                            key: 'code',
                            label: 'Code',
                            sortable: true,
                        },
                      /*  {
                            key: 'lead',
                            label: 'Lead',
                            sortable: true,
                        },*/
                        {
                            key: 'ambassador',
                            label: 'Ambassador',
                            sortable: true,
                        },
                        {
                            key: 'dateSaved',
                            label: 'Date Saved',
                            sortable: true,
                        },
                        {
                            key: 'dateActivated',
                            label: 'Activation Date',
                            sortable: true,
                        },
                        {
                            key: 'dueDate',
                            label: 'Due Date',
                            sortable: true,
                        },

                        {
                            key: 'payment',
                            label: 'Payment Reference',
                            sortable: true,
                        },
                        {
                            key: 'paymentTypeID',
                            label: 'Payment Type',
                            sortable: true,

                        },
                        {
                            key: 'location',
                            label: 'Location',
                            sortable: true,

                        },
                        {
                            key: 'isProcess',
                            label: 'Paid',
                            sortable: true,

                        },
                        {
                            key: 'activatedByPoll',
                            label: 'Delivered By Poll',
                            sortable: true,

                        },
                       /* {
                            key: 'bookingStatus',
                            label: 'Booking Status',
                            sortable: true,
                        },
                        {
                            key: 'comment',
                            label: 'Comment',
                            sortable: true,
                        },*/
                    ],
                },
            }
        },
        components: {
            'PulseLoader': PulseLoader,


        },
        computed: {
            cMembershipsByDate: function () {
                this.showDetails = false;
                var paid = this.filters.paid;
                var activated = this.filters.activated;
                var status = this.filters.status;
                var paymentType = this.filters.paymentType;
                var free = this.filters.paid;
                if (free == 'Free') {
                    free = true;
                } else if (free === 1) {
                    free = 'All';
                } else {
                    free = null;
                }
                var fecha = new Date();
                var dueDate = new Date(2030, 1 , 1 );
                if (status === 1) {
                    return this.membershipsByDate.filter((i, index) => (i.isProcess !== paid &&
                        ((paymentType == 1) ? i.paymentTypeID > 0 || i.paymentTypeID == null : i.paymentTypeID == paymentType)
                        && i.activatedByPoll !== activated && dueDate > this.getDate(i.dueDate, true) &&
                        (free == 'All' ? i.free != 5 : free == true ? i.free == free : i.free == null)))
                }
                if (status === true) {
                    dueDate = new Date(fecha.getTime() - 24 * 60 * 60 * 1000);
                    return this.membershipsByDate.filter((i, index) => (i.isProcess !== paid &&
                        ((paymentType == 1) ? i.paymentTypeID > 0 || i.paymentTypeID == null : i.paymentTypeID == paymentType)
                        && i.activatedByPoll !== activated && this.getDate(i.dueDate, true) > dueDate))
                }
                if (status === false) {
                    dueDate =  new Date(fecha.getTime() - 24 * 60 * 60 * 1000); 
                    return this.membershipsByDate.filter((i, index) =>
                        (i.isProcess !== paid && ((paymentType == 1) ? i.paymentTypeID > 0 || i.paymentTypeID == null : i.paymentTypeID == paymentType)
                        && i.activatedByPoll !== activated && this.getDate(i.dueDate, true) < dueDate  ))
                }
               // console.log("due date " + dueDate);
               
            },
        },
        filters: {
            toNumero(value) {
                let val = (value / 1).toFixed(0).replace(',', ',');
                return val.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
            },
        },
        methods: {
            saveComment(membershipSaleID) {                            
                let r = this;
                var dueDate = this.calendarInput($('#calendario6').val());
                console.log(this.member.bookingStatus);
                axios.post('/Content/management/saveMembershipComment', {
                    membershipSaleID: membershipSaleID,
                    comment: this.comment,
                    bookingStatus: this.member.bookingStatus,
                    dueDate: dueDate,
                }).then(response => {
                    $.confirm({
                        title:'Success',
                        content: 'Changes saved',
                        type: 'blue',
                        typeAnimated: true,
                        buttons: {
                            tryAgain: {
                                text: 'Close',
                                btnClass: 'btn-blue',
                                action: function () {
                                }
                            },

                        }
                    });
                    }).catch(error => {
                        $.confirm({
                            title: 'Error',
                            content: 'Something went wrong, please try again',
                            type: 'red',
                            typeAnimated: true,
                            buttons: {
                                tryAgain: {
                                    text: 'Close',
                                    btnClass: 'btn-red',
                                    action: function () {
                                    }
                                },

                            }
                        });
                        r.loadingSearch = false;

                    });
            },
            getContactInfo: function (key) {
                console.log(key);
                this.member.name = "";
                this.member.phone = "";
                this.member.email = "";
                this.member.folio = "";
                this.member.code = "";
                this.member.comment = "";
                this.member.dueDate = null,
                this.member.bookingStatus = null;
                const r = this;
                this.showDetails = false;
                this.loadingDetails = true;
                if (this.member.key !== null) {
                    this.member.key._rowVariant = "";
                }
                this.member.key = key;
                key._rowVariant = "primary";
                axios.post('/crm/reports/getContactInfo', {
                    membershipSalesID: key.membershipSalesID,
                }).then(response => {
                    this.showDetails = true;;

                    this.member.name = response.data[0][0].name;
                    this.member.code = response.data[0][0].code;
                    this.comment = response.data[0][0].comment;
                    this.member.dueDate = this.getDate(response.data[0][0].dueDate);
                    if (response.data[0][0].bookingStatusID !== null) {
                        this.member.bookingStatus = response.data[0][0].bookingStatusID;
                    }
                   
                    if (response.data[2][0] !== null) {
                        this.member.email = response.data[2][0].email;
                    }
                    
                    if (response.data[1][0] !== null) {
                        this.member.phone = response.data[1][0].phone;
                    }
                    if (response.data[3][0] !== null) {
                        this.member.folio = response.data[3][0].folio;
                    }


                    this.loadingDetails = false;
                });
            },
            calendarInput: function (wrongDate, to) {
                var dia = parseInt(wrongDate[0] + wrongDate[1]);
                var mes = parseInt(wrongDate[3] + wrongDate[4]) - 1;
                var año = parseInt(wrongDate[6] + wrongDate[7] + wrongDate[8] + wrongDate[9]);
                if (to) {
                    return new Date(año, mes, dia, 23, 59);
                } else {
                    return new Date(año, mes, dia);
                }
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
            getDate: function (date, filter) {
                var getdate = date;
                if (date.length == 10) return date;
                getdate = getdate.split("/Date(").join("");
                getdate = getdate.split(")/").join("");
                var newdate = new Date(parseInt(getdate));
                
                if (filter) return newdate;
                var month = newdate.getMonth() + 1;
                return newdate.getDate().toString().padStart(2, "0") + "-" + month.toString().padStart(2, "0") + "-" + newdate.getFullYear();
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

                $(function () {
                    $('#datetimepicker6').datetimepicker({
                        format: 'DD-MM-YYYY',
                       
                    });
                });

            },
            getMemberships: function () {
              
                const r = this;
                this.loadingSearch = true;
                var from = this.calendarInput($('#calendario3').val());
                var to = this.calendarInput($('#calendarioto4').val(), true);
                if (this.code !== null) {
                    if (this.code.trim() == "") {
                        this.code = null;
                    }
                }


                axios.post('/crm/reports/getMembershipsInfo', {
                    fromDate: from,
                    toDate: to,
                    code: this.code,
                    type: 'memberships'

                }).then(response => {
                    this.membershipsByDate = response.data;

                    this.loadingSearch = false;
                    this.showtable = true;
                   

                }).catch(error => {
                    $.confirm({
                        title: 'Error',
                        content: 'Something went wrong, please try again',
                        type: 'red',
                        typeAnimated: true,
                        buttons: {
                            tryAgain: {
                                text: 'Close',
                                btnClass: 'btn-red',
                                action: function () {
                                }
                            },

                        }
                    });
                    r.loadingSearch = false;
                 
                });
                this.par = 0;
                this.newSurvey = false;
            },
            getBookingStatus() {
                axios.get('/Content/management/getBookingStatusOptions', {
                    
                }).then(response => {
                    this.bookingStatusOptions = response.data;
                }).catch(error => {
                });
            },
          
        },

        mounted() {
            this.getBookingStatus();
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
    .filtercss {
        border-style: groove;
        padding: 10px;
        border-color: rgba(0, 0, 0, 0.125);
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
