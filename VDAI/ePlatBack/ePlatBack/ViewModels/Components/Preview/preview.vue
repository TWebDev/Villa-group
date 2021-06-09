<template>
    <div>
        <div v-show="loading">
            <pulse-loader color="#31A3DD"></pulse-loader>
        </div>
        <div v-show="!loading">         
                <div class="row mb-4">
                    <div class="col-sm-8">
                        <h2>My Free Trials</h2>
                    </div>
                    <div class="col-sm-4 mb-2">
                        <div class="text-right mb-2">
                            <div class="">

                                <button class="btn btn-primary" @click="changeComponent">
                                    <i class="material-icons" style="vertical-align: middle;">
                                        add_circle
                                    </i>Send a Free Trial
                                </button>
                            </div>

                        </div>
                    </div>
                </div>        
            <div class="container-fluid">
                <div class="row mb-2">
                    <div class="card col-sm-12 ">
                        <div class="card-body">
                            <div class="row">
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
                                    <button class="btn btn-primary mt-4" @click="searchByDate">Get</button>
                                </div>
                                <div class="col-sm-12">
                                    <span style="font-weight: 200;font-size: 25px;">{{mypreviews.length}} Free trials found.</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div v-if="loading2">
                    <pulse-loader color="#31A3DD"></pulse-loader>
                </div>
                <div v-else class="row">
                    <b-table @row-clicked="showDetails" v-scroll-to="'#details'" :per-page="perPage" striped hover
                             :items="mypreviews" :fields="fields" :current-page="currentPage">

                    </b-table>
                    <b-pagination size="md" :total-rows="countrows" v-model="currentPage" :per-page="perPage">
                    </b-pagination>
                </div>
            </div>
            <div id="details">
                <div class="card col-sm-12" v-show="showdetails">
                    <div class="card-body">
                        <h5 class="card-title">Member info</h5>
                        <div class="row">

                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Location</span>
                                <v-select v-model="location" :options="locations"></v-select>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Name</span>
                                <input class="form-control" type="text" v-model="name">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Last Name</span>
                                <input class="form-control" type="text" v-model="lastname">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Email</span>
                                <input class="form-control" type="email" v-model="email" v-on:keyup="toLowerCase">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Confirm Email</span>
                                <input class="form-control" type="email" v-model="confirmemail" v-on:keyup="toLowerCase">
                                <small class="form-text text-muted">
                                    <span style="color: red">
                                        {{emailerror}}
                                    </span>
                                </small>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Phone</span>
                                <input class="form-control" type="text" id="phone" @focus="maxLenght" v-model="phone">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>DueDate</span>
                                <div class="form-group">
                                    <div class="input-group date" id="datetimepicker5" data-target-input="nearest">
                                        <input type="text" v-model="dueDate" v-on:change="calendarInput" id="calendario5" class="form-control datetimepicker-input" data-target="#datetimepicker5" />
                                        <div class="input-group-append" data-target="#datetimepicker5" data-toggle="datetimepicker">
                                            <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span> Booking Status</span>
                                <b-form-select v-model="bookingStatus" :options="bookingStatusOptions"></b-form-select>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label>Comments</label>
                                    <textarea v-model="comment" class="form-control" id="exampleFormControlTextarea1" rows="4"></textarea>
                                </div>
                            </div>

                            <div class="mt-2 text-center">
                                <span style="color: red"> <b>{{error}}</b> </span>
                            </div>
                            <div class="col-sm-12 mt-3 ">
                                <span>Submited by: </span> <span><b>{{ambassador}}</b></span>
                            </div>
                        </div>
                        <div class="mt-2 text-right mb-5">
                            <button class="btn btn-primary" @click="saveUpdates">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    import vSelect from 'vue-select';
    import axios from 'axios';
    
    Vue.component('v-select', vSelect)

    export default {
        props: ['cards'],
        data: function () {
            return {
                bookingStatus: '',
                bookingStatusOptions: [],
                dueDate:'',
                comment:'',
                phone: '',
                freeTrialDays:'',
                loading: false,
                loading2: false,
                perPage: 5,
                showdetails: false,
                currentPage: 1,
                countrows: 1,
                mypreviews: [],
                emailerror: '',
                name: '',
                lastname: '',
                email: '',
                confirmemail: '',
                error: '',
                loadingcodes: false,
                locations: [],
                location: '',
                updatedata: '',
                events: [],
                today: '',
                countdays: 0,
                datesearch: '',
                pickeddate: '',
                fields: [
                    {
                        key: 'fullname',
                        label: 'Name',
                        sortable: true

                    },
                    {
                        key: 'email',
                        label: 'Email',
                        sortable: true,

                    },
                    {
                        key: 'location',
                        label: 'Location',
                        sortable: true,

                    },
                    {
                        key: 'savedBy',
                        label: 'Ambassador',
                        sortable: true,

                    },

                ],
                tempkey: null,
                ambassador:'',


            }
        },
        components: {
            'PulseLoader': PulseLoader,
        },

        methods: {
            searchByDate: function () {
                this.loading = true;
                var from = this.calendarInput($('#calendario3').val());
                var to = this.calendarInput($('#calendarioto4').val(), true);
                axios.post('/membership/Preview/getPreviewsPerDay', {
                    fromDate: from,
                    toDate: to
                }).then(response => {
                    console.log(response.data);
                    this.loading = false;
                    this.mypreviews = response.data;
                    var a = this.mypreviews.length;
                    this.countrows = a;
                });
              
            },
            getLocations: function () {
                axios.get('/membership/activateMembership/getLocations', {
                }).then(response => {
                    this.locations = response.data;
                });
            },
            changeComponent: function () {
                this.$root.currentcomponentpreview = "addpreview";
                
            },
            
            toLowerCase: function () {
                this.email = this.email.toLowerCase();
                this.confirmemail = this.confirmemail.toLowerCase();
            },
            maxLenght: function () {
                document.getElementById("phone").maxLength = "15";
            },
            showDetails: function (key) {
                this.showdetails = true;
                this.code = key.code;
                this.ambassador = key.savedBy;
                this.location = key.location;
                this.name = key.name;
                this.lastname = key.lastname;
                this.email = key.email;
                this.confirmemail = key.email;
                this.price = key.price;
                this.bookingStatus = key.bookingStatusID;
                this.payment = key.payment;
                this.phone = key.phone;
                this.comment = key.comment;
                console.log(key.duedate.length);
                if (key.duedate.length < 12) {
                    this.dueDate = key.duedate;
                } else {
                    this.dueDate = this.getDate(key.duedate); 
                }
                                           
                if (this.tempkey !== null) {
                    this.tempkey._rowVariant = "";
                }
                this.tempkey = key;
                this.showresults = true;
                this.showresultsrange = true;
                key._rowVariant = "primary";
            },
            saveUpdates: function () {
                var haserror = this.verifyFields();
                if (haserror) {
                    return this;
                }
                var dueDate = this.calendarInput($('#calendario5').val());

                var member = this.getFields();
                this.updatedata = member;
                console.log(member);
                var text = "Do you want to update the info ? ";
                var r = this;
                $.confirm({
                    title: 'Update info',
                    content: text,
                    typeAnimated: true,
                    type: 'blue',
                    buttons: {
                        delete: {
                            text: 'Update',
                            btnClass: 'btn-blue',
                            action: function () {
                                axios.post('/membership/Preview/updatePreview', 
                                    {
                                        location: member.location,
                                        name: member.name,
                                        lastname: member.lastname,
                                        email: member.email,
                                        harvestingid: member.harvestingid,
                                        phone: member.phone,    
                                        comment: member.comment,
                                        dueDate: dueDate,
                                        bookingStatus: member.bookingStatus
                                }).then(response => {
                                    console.log(response.data);
                                    if (response.data == "ok") {
                                        $.alert('Membership updated correctly.');
                                        text = "";                                     
                                        r.tempkey.email = member.email;
                                        r.tempkey.confirmemail = member.email;
                                        r.tempkey.fullname = member.name + " " + member.lastname;
                                        r.tempkey.name = member.name;
                                        r.tempkey.lastname = member.lastname;
                                        r.tempkey.locationid = member.location;
                                        r.tempkey.comment = member.comment;
                                        r.tempkey.phone = member.phone;
                                        r.tempkey.bookingStatusID = member.bookingStatus;
                                        r.tempkey.duedate = $('#calendario5').val();

                                        console.log("location name: " + $('#calendario5').val());
                                        if (typeof (r.location) == "string") {
                                            r.tempkey.location = r.location;
                                        } else {
                                            r.tempkey.location = r.location.label;
                                        }
                                    }
                                    console.log(response.data);
                                });
                            }
                        },
                        Cancel: function () {
                            return this;
                        },
                    }
                });
            },
            getFields: function () {
                var location = "";
                console.log(typeof (this.location));
                if (typeof (this.location) == "string") {
                    location = this.tempkey.locationid;
                    console.log(location);

                } else {
                    location = this.location.value;
                    if (location == undefined) {
                        location = this.tempkey.locationid;
                    }

                }               
                var update = {
                    location: location,
                    name: this.name,
                    lastname: this.lastname,
                    email: this.email,
                    harvestingid: this.tempkey.harvestingid,
                    phone: this.phone,
                    comment: this.comment,
                    bookingStatus: this.bookingStatus
                }
                return update;
            },
            verifyFields: function () {
                if (this.email !== this.confirmemail) {
                    this.emailerror = "The emails are not the same, please verify.";
                    return true;
                } else {
                    this.emailerror = "";
                }
                if ( this.name == "" || this.confirmemail == "" || this.lastname == ""
                    || this.email == "" || this.location == "" || this.location == null ) {
                    this.error = "Please, fill out the fields correctly.";
                    return true;
                } else {
                    this.error = "";
                }            
                var email = this.email;
                if (email.indexOf('@') != -1) {
                    return false;
                } else {
                    this.emailerror = "the email doesn't have an @ "
                    return true;
                }
            }, calendarInput: function (wrongDate, to) {
                var dia = parseInt(wrongDate[0] + wrongDate[1]);
                var mes = parseInt(wrongDate[3] + wrongDate[4]) - 1;
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
                $(function () {
                    $('#datetimepicker5').datetimepicker({
                        format: 'DD-MM-YYYY',
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
            getDates: function (dato) {
                var date = new Date();
                var dia = date.getUTCDate() + dato;
                var mes = date.getUTCMonth();
                var anio = date.getUTCFullYear();
                var fecha = new Date(anio, mes, dia);
                this.datesearch = fecha;
                var month = fecha.getUTCMonth() + 1;
                this.today = fecha.getUTCDate() + "-" + month + "-" + fecha.getUTCFullYear();
                return fecha;
            },
            nextPreviousDay: function (tipo) {
                this.loading2 = true;
                if (tipo == "next") {
                    this.countdays = this.countdays + 1;
                } if (tipo == "today") {
                    this.countdays = 0;
                    this.getDates(this.countdays);
                } if (tipo == "previous") {
                    this.countdays = this.countdays - 1;
                }

                if (tipo == "calendario") {
                    this.datesearch = $('#calendario').val();
                    this.today = this.datesearch;
                    console.log(this.datesearch);
                } else {
                    var fecha = this.getDates(this.countdays);
                }
                axios.post('/membership/Preview/getPreviewsPerDay', {
                    date: this.datesearch,

                }).then(response => {
                    console.log(response.data);
                    this.showdetails = false;
                    this.loading2 = false;
                    this.mypreviews = response.data;
                    var a = this.mypreviews.length;
                    this.countrows = a;

                });

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
            this.getDates(0);
            this.getLocations();
          
        },



    }


</script>
