<template>
    <div>
        <div v-show="loading">
            <pulse-loader color="#31A3DD"></pulse-loader>
        </div>
        <div v-show="!loading">

            <div class="container-fluid  mb-3">
                <div class="row">
                    <div class="col-sm-8">
                        <h2>My Memberships</h2>
                    </div>
                    
                    <div class="col-sm-4">
                        <div class="text-right mb-2">
                            <div class="">

                                <button class="btn btn-primary" @click="changeComponent">
                                    <i class="material-icons" style="vertical-align: middle;">
                                        add_circle
                                    </i> Activate a Membership
                                </button>
                            </div>
                        </div>
                    </div>
                </div>             
            </div>        
            <div class="container-fluid">
                <div class="row mb-2">
                    <div class="card col-sm-12 ">
                        <div class="card-body">
                            <div class="row">
                                <div class="">

                                    <button class="btn btn-primary mb-3" style=" padding: 5px;" @click="nextPreviousDay('previous')">
                                        <i class="material-icons" style="vertical-align: middle; padding: 0px;">
                                            arrow_back
                                        </i>
                                    </button>
                                    <button class="btn btn-primary mb-3 " @click="nextPreviousDay('today')">
                                        Today
                                    </button>
                                    <button class="btn btn-primary mb-3" style=" padding: 5px;" @click="nextPreviousDay('next')">
                                        <i class="material-icons" style="vertical-align: middle;">
                                            arrow_forward
                                        </i>
                                    </button>
                                    <div class="form-group">
                                        <div class="input-group date" id="datetimepicker4" data-target-input="nearest">
                                            <input type="text" v-on:change="calendarInput" id="calendario" class="form-control datetimepicker-input" data-target="#datetimepicker4" :value="today" />
                                            <div class="input-group-append" data-target="#datetimepicker4" data-toggle="datetimepicker">
                                                <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                            </div>
                                        </div>
                                    </div>
                                    <button class="btn btn-outline-secondary mr-3" @click="nextPreviousDay('calendario')">Search</button>
                                    <strong>You have {{countrows}} Memberships.</strong>
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
                             :items="mymemberships" :fields="fields" :current-page="currentPage">
                        <template slot="cancel" slot-scope="row">
                            <button v-if="row.item.status == 'Inactive'" class="btn btn-danger" disabled @click="inactivateMembership(row.item)">Inactivated</button>
                            <button v-else class="btn btn-danger" @click="inactivateMembership(row.item)">Inactivate</button>
                        </template>

                    </b-table>
                    <b-pagination size="md" :total-rows="countrows" v-model="currentPage" :per-page="perPage">
                    </b-pagination>
                </div>
                <div class="col-sm-12 text-center">
                    <div v-show="loadingEmail">
                        <span>Inactivating Membership</span>
                        <pulse-loader color="#31A3DD"></pulse-loader>
                    </div>
                </div>
               
            </div>

            <div id="details">
                <div class="card col-sm-12" v-if="showdetails">
                    <div class="card-body">
                        <h5 class="card-title">Member info</h5>
                        <div class="row">
                            <div class="col-sm-6 col-md-4 col-lg-4">

                                <span>Card Code</span>
                                <div>
                                    <v-select v-model="code" disabled></v-select>
                                </div>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Ambassador</span>
                                <input class="form-control" type="text" disabled :value="ambassador">
                            </div>
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
                                <span>Country</span>
                                <v-select v-model="country" :options="countries"></v-select>
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
                                <span>Price</span>
                                <input class="form-control" type="number" v-model="price" placeholder="$ Dollars">
                            </div>

                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Payment Reference</span>
                                <input class="form-control" type="text" v-model="payment">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">

                            </div>
                            <div class="mt-2 text-center">
                                <span style="color: red"> <b>{{error}}</b> </span>
                            </div>
                            <div class="col-sm-12 mt-3 ">
                                <span>Activated by: </span> <span><b>{{ambassador}}</b></span>
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
                membershipStatus: false,
                loadingEmail: false,
                loading: true,
                loading2: false,
                perPage: 5,
                showdetails: false,
                currentPage: 1,
                countrows:1,
                mymemberships: [],
                code: '',
                inactivescodes: [],
                ambassador: '',
                emailerror: '',
                name: '',
                lastname: '',
                email: '',
                confirmemail: '',
                error: '',
                loadingcodes: false,
                payment: '',
                locations: [],
                location: '',
                price: '',
                updatedata: '',
                events: [],
                today: '',
                countdays: 0,
                datesearch: '',
                pickeddate: '',
                countries: [],
                country:[],
                fields: [
                    {
                        key: 'code',
                        label: 'Card Code',
                        sortable: true

                    },
                    {
                        key: 'member',
                        label: 'Member',
                        sortable: true,

                    },
                    {
                        key: 'email',
                        label: 'Email',
                        sortable: true,

                    },
                    {
                        key: 'ambassador',
                        label: 'Ambassador',
                        sortable: true,

                    },
                    {
                        key: 'cancel',
                        label: 'Cancel',
                        sortable: true,

                    },                  
                ], 
                tempkey:null,
           

            }
        },
        components: {
            'PulseLoader': PulseLoader,
            
        },

        methods: {
            inactivateMembership: function (key) {
                var r = this;
                
                $.confirm({
                    title: 'Inactive Membership',
                    content: 'Are you sure?',
                    typeAnimated: true,
                    type: 'red',
                    buttons: {
                        delete: {
                            text: 'Delete',
                            btnClass: 'btn-red',
                            action: function () {
                                r.loadingEmail = true;
                                var membership = {
                                    code: key.code,
                                    locationname: key.locationname,
                                    name: key.name,
                                    lastname: key.lastname,
                                    email: key.email,
                                    price: key.price,
                                    payment: key.payment,
                                    membershipid: key.salesid,
                                    ambassador: key.ambassador,
                                    cardid: key.cardid,

                                }
                                console.log(key);
                                var data = JSON.stringify(membership);
                                axios.post('/membership/activateMembership/InactiveMembership', {
                                    data: data,
                                }).then(response => {
                                    if (response.data == "ok") {
                                        $.alert('Membership was inactivated correctly.');
                                        key.status  = "Inactive";
                                        r.loadingEmail = false;
                                    }
                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });

                
              
            },
            getLocations: function () {
                axios.get('/membership/activateMembership/getLocations', {

                }).then(response => {
                    this.locations = response.data;
                });
            },
            changeComponent: function () {
                this.$root.currentcomponentmemberhips = "activatemembership";
                console.log("click");
            },
            getMemberships: function () {
               
                axios.post('/membership/activateMembership/getMembershipsDates', {
                    date: this.datesearch,
                    }).then(response => {

                        this.loading = false;
                        this.mymemberships = response.data;
                        var a = this.mymemberships.length;
                        this.countrows = a;
                       
                      
                    });
                
            },
            toLowerCase: function () {
                this.email = this.email.toLowerCase();
                this.confirmemail = this.confirmemail.toLowerCase();

            },
            showDetails: function (key) {
                this.showdetails = true;
                this.code = key.code;
                this.ambassador = key.ambassador;
                this.location = key.locationname;
                this.name = key.name;
                this.lastname = key.lastname;
                this.email = key.email;
                this.confirmemail = key.email;
                this.price = key.price;
                this.payment = key.payment;

                this.country = {
                    label: key.country,
                    value: key.countryid,
                };
               
                
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
                              
                                axios.post('/membership/activateMembership/updateMemberships', {
                                    code: member.code,
                                    location: member.location,
                                    name: member.name,
                                    lastname: member.lastname,
                                    email: member.email,
                                    payment: member.payment,
                                    price: member.price,
                                    leadid: member.leadid,
                                    leademailid: member.emailid,
                                    membershipsalesid: member.membershipsalesid,
                                    countryid: member.countryid,
                                }).then(response => {

                                    console.log(response.data);
                                    if (response.data == "ok") {
                                        $.alert('Membership updated correctly.');
                                        text = "";
                                        r.tempkey.price = member.price;
                                        r.tempkey.email = member.email;
                                        r.tempkey.confirmemail = member.email;
                                        r.tempkey.member = member.name + " " + member.lastname;
                                        r.tempkey.name = member.name;
                                        r.tempkey.lastname = member.lastname;
                                        r.tempkey.payment = member.payment;
                                        r.tempkey.locationid = member.location;
                                        console.log("location name: " + r.location);
                                        if (typeof (r.location) == "string") {
                                            r.tempkey.locationname = r.location;
                                        } else {
                                            r.tempkey.locationname = r.location.label;
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
                if (typeof(this.location) == "string") {
                    location = this.tempkey.locationid;
                    console.log(location);

                } else {
                    location = this.location.value;
                    if (location == undefined) {
                        location = this.tempkey.locationid;
                    }
                    
                }
                console.log(" location " + location);
                console.log(" tempkey  " + this.tempkey.locationid);
                var update = {
                    code: this.code,
                    location: location,
                    name: this.name,
                    lastname: this.lastname,
                    email: this.email,
                    price: this.price,
                    payment: this.payment,
                    leadid: this.tempkey.leadid,
                    membershipsalesid: this.tempkey.salesid,
                    emailid: this.tempkey.leademailid,
                    countryid: this.country.value,
                   
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
                if (this.code == "" || this.ambassador == "" || this.name == "" || this.confirmemail == "" || this.lastname == ""
                    || this.email == "" || this.payment == "" || this.location == "" || this.location == null || this.price == ""
                    || this.country == "" || this.country == null) {
                    this.error = "Please, fill out the fields correctly.";
                    return true;
                } else {
                    this.error = "";
                }
                this.price = Math.trunc(this.price);
                if (this.price < 0) {
                    this.error = "The minimum accepted price is 0.";
                    return true;
                }
                var email = this.email;
                if (email.indexOf('@') != -1) {
                    return false;
                } else {
                    this.emailerror = "the email doesn't have an @ "
                    return true;
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
                this.today =  fecha.getUTCDate()+ "-"  + month +"-" + fecha.getUTCFullYear();
                return fecha;
            },
            nextPreviousDay: function (tipo) {
                this.loading2 = true;
                if (tipo == "next") {
                    this.countdays = this.countdays + 1;
                } if (tipo == "today") {
                    this.countdays = 0;
                    this.getDates(this.countdays);
                } if (tipo == "previous"){
                    this.countdays = this.countdays - 1;
                }
                
                if (tipo == "calendario") {
                    this.datesearch = $('#calendario').val();
                    this.today = this.datesearch;
                } else {
                    var fecha = this.getDates(this.countdays);
                }
                axios.post('/membership/activateMembership/getMembershipsDates', {
                    date: this.datesearch,
                   
                }).then(response => {
                    this.loading2 = false;
                    this.mymemberships = response.data;
                    var a = this.mymemberships.length;
                    this.countrows = a;
                    this.showdetails = false;
                        
                });

            },
            calendarInput: function () {
               

                this.pickeddate = $('#calendario').val();
               
            },
            getCountries: function () {
                axios.get('/membership/activateMembership/getCountries', {

                }).then(response => {
                    this.countries = response.data;
                });
            },
        },
        mounted() {
            this.getDates(0);
            this.getLocations();
            this.getCountries();
            this.getMemberships();
            $(function () {
                $('#datetimepicker4').datetimepicker({
                    format: 'DD-MM-YYYY'
                });
            });

         
           

            
            

        },
       
         

    }


</script>
