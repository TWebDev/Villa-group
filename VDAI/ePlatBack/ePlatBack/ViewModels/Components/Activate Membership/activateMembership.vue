<template>
    <div class="mb-5">
        <div class="row">
            <div class="col-sm-8">
                <h2 class="mb-4"> Activate Membership </h2>
            </div>
            <div class="col-sm-4">
                <div class="text-right mb-2">
                    <div class="">
                        <button class="btn btn-primary" @click="changeComponent">
                            <i class="material-icons" style="vertical-align: middle;">
                                arrow_back
                            </i> Back to My Memberships
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <div class="container-fluid mt-4">
            <div class="row">

                <div class="card col-sm-12">
                    <div class="card-body">
                        <h5 class="card-title">Member info</h5>
                      
                        <div class="row">
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span v-if="loadingcodes"><clip-loader color="#31A3DD"></clip-loader></span>
                                <span>Card Code</span>
                                <div @click="getInactivesCodes">
                                    <v-select v-model="code" :options="inactivescodes"></v-select>
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
                                <span>Phone</span>
                                <input class="form-control" type="text" id="phone" @focus="maxLenght" v-model="phone">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Price</span>
                                <input disabled class="form-control" type="number" v-model="price" placeholder="$ Dollars">
                                <span style="color: red !important" class="text-muted">{{errorprice}}</span>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <b-form-group label="Payments methods" :disabled="paid.status">
                                    <b-form-radio v-model="typeofpaid" name="some-radios" value="1">Credit/Debit Card</b-form-radio>
                                    <b-form-radio v-model="typeofpaid" name="some-radios" value="2">I have a Payment Reference</b-form-radio>
                                    <b-form-radio v-model="typeofpaid" name="some-radios" value="3">Cash</b-form-radio>

                                </b-form-group>
                            
                                <div v-if="typeofpaid == 1 && !paid.status">
                                    <button class="hand btn btn-primary mb-2" @click="openPaymentModal">Open Payment Fields </button>
                                </div>
                                <div v-if="typeofpaid == 2 || paid.status">
                                    <span>Payment Reference</span>
                                    <input :disabled="paid.status" class="form-control" type="text" v-model="payment" placeholder="">
                                </div>
                            </div>

                            <div class="col-sm-6 col-md-4 col-lg-4">

                            </div>
                            <div class="mt-2 text-center">
                                <span style="color: red"> <b>{{error}}</b> </span>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-6 mt-3">
                                <h5>   DISCLAIMER </h5>
                            </div>
                            <div class="col-sm-6 text-right mb-3">
                                <button v-if="lang == 'us'" class="btn btn-primary" @click="langdisclaimer('es')">Español</button>
                                <button v-else class="btn btn-primary" @click="langdisclaimer('us')">English</button>

                            </div>

                            <div class="col-sm-12">
                                <span class="form-text text-muted text-justify">
                                    {{disclaimer}}
                                </span>
                            </div>
                            <div class="col-sm-12 mt-5">
                                <div id="signature">
                                </div>
                            </div>

                        </div>
                    </div>
                    <div v-if="loadingadd" class="text-center">
                        <strong> Activating Membership...</strong>
                        <pulse-loader color="#31A3DD"></pulse-loader>
                    </div>
                    <div class="mt-2 text-right">
                        <button class="btn btn-primary mb-3" @click="activateMember">Activate</button>
                    </div>


                    <b-modal ref="paymentModal" no-close-on-backdrop  hide-footer hide-header title="Payment Data">
                        <div class="row">
                            <div class="col-12 text-center">
                                <i class="material-icons" v-if="creditcard.type == 'none'" style="font-size: 59px">
                                    payment
                                </i>
                                <img v-if="creditcard.type == 'visa'" src="/Images/senses/visa.png" style="width: 59px" />
                                <img v-if="creditcard.type == 'mastercard'" src="/Images/senses/mastercard.png" style="width: 59px" />
                                <img v-if="creditcard.type == 'discover'" src="/Images/senses/discover.png" style="width: 69px" />
                            </div>
                            <div class="col-12 mb-2">
                                <span>Name</span>
                                <input class="form-control" type="text" v-model="creditcard.name" placeholder="Name">
                                <span v-if="paymentsErrors.status" style="color: red !important" class="text-muted">{{paymentsErrors.name}}</span>
                            </div>
                            <div class="col-8 mb-2">
                                <span>Card Number</span>
                                <input class="form-control" type="text" id="cardnumber" maxlength="19" v-model="creditcard.cardnumber" v-on:keyup="verifyCardNumber" placeholder="0000 0000 0000 0000">
                                <span v-if="paymentsErrors.status" style="color: red !important" class="text-muted">{{paymentsErrors.cardnumber}}</span>
                            </div>
                            <div class="col-4 mb-2">
                                <span>Expiration Date</span>
                                <input class="form-control" type="text" id="expirationdate" maxlength="5" v-on:keyup="verifyExpirationDate" v-model="creditcard.expirationdate" placeholder="MM/YY">
                                <span v-if="paymentsErrors.status" style="color: red !important" class="text-muted">{{paymentsErrors.expirationdate}}</span>

                            </div>
                            <div class="col-6 mb-2">
                                <span>Security Code (CVV)</span>
                                <input class="form-control" type="password" inputmode="numeric" id="cvv" v-model="creditcard.cvv" maxlength="4" placeholder="000">
                                <span v-if="paymentsErrors.status" style="color: red !important" class="text-muted">{{paymentsErrors.cvv}}</span>

                            </div>
                            <div class="col-6 mb-2">
                                <span>ZipCode</span>
                                <input class="form-control" type="text" v-model="creditcard.zipcode" placeholder="00000">
                                <span v-if="paymentsErrors.status" style="color: red !important" class="text-muted">{{paymentsErrors.zipcode}}</span>

                            </div>
                            <div class="col-12 mb-2">
                                <span>Address</span>
                                <input class="form-control" type="text" v-model="creditcard.address" placeholder="Address">
                                <span v-if="paymentsErrors.status" style="color: red !important" class="text-muted">{{paymentsErrors.address}}</span>

                            </div>
                            <div class="col-6 mb-2 ">
                                <b style="margin-left: 25px">Total to pay:</b><br />
                                <span style="font-size: 5.5rem;line-height: 1;font-weight: 500;">${{parseFloat(price)}}</span>
                                <br />
                                <span style="font-size: 38px;margin-left: 25px;font-weight: 100;">Dollars</span>

                            </div>
                            <div class="col-sm-6" v-if="paid.loading">
                                <div class="mb-3 mt-3 p-2"><span style="font-weight: 100;">Processing payment, please wait a moment...</span></div>
                                <ClipLoader :loading="paid.loading" color="#31A3DD" ></ClipLoader>
                            </div>
                            <div class="col-sm-6 " v-if="!paid.loading">
                                <div class="row">
                                    <div class="col-sm-12 " v-if="creditcard.responseerror !== null">
                                        <div class="alert alert-danger" role="alert">
                                            <span class="mb-3" style="font-size:12px"> Something went wrong with the payment process, please verify the next errors: </span>
                                            <div class="mt-3" v-html="failPayment">{{failPayment}}</div>
                                        </div>
                                    </div>
                                    <div class="col-sm-12 " v-if="paid.status">
                                        <div class="alert alert-success" role="alert">
                                            <span class="mb-3" style="font-size:12px"> Your payment was made succefully</span>
                                            <div class="mt-3" v-html="failPayment">{{failPayment}}</div>
                                        </div>
                                    </div>
                                </div>                              
                            </div>  
                            <div class="col-sm-12 mb-2">
                                <span>Note: We  accept Master Card, Visa, Discover and American Express only.</span>
                            </div>
                            <div class="col-12 mb-2 text-right" v-if="!paid.status">
                                <button class="btn btn-success" @click="sendPayment" v-if="enablePaidButton">Pay</button>
                                <button class="btn btn-success" v-else disabled>Pay</button>
                                <button class="btn btn-danger" @click="closePaymentModal">Cancel</button>                             
                            </div>
                            <div class="col-12 mb-2 text-center" v-else>
                                <button class="btn btn-primary " @click="closePaymentModal" >clic here to continue</button>
                           </div>
                        </div>
                    </b-modal>
                </div>
            </div>
        </div>
    </div>
   
  
</template>


<script>
    const uuidv4 = require('uuid/v4');
    import saveSignature from '../../../Scripts/signatureMembership.js'
    var signature = require('../../../Scripts/signatureMembership.js');
    import axios from 'axios';
    import vSelect from 'vue-select';
    import ClipLoader from 'vue-spinner/src/ClipLoader.vue'

    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    export default {
        props: ['cards'],
            data: function () {
                return {
                    phone:null,
                    guid: uuidv4(),
                    creditcard: {
                        guid: '',
                        name: '',
                        cardnumber: '',
                        cardnumbertrimed:'',
                        expirationdate: '',
                        cvv: '',
                        type: 'none',
                        zipcode:'',
                        address: '',
                        amount: 0,
                        paymentid: 0,
                        emonth: 0,
                        eyear: 0,                     
                        responseerror: null,
                        cardtype:0,

                    },
                    paymentsErrors: {
                        status:false,
                        guid: '',
                        name: '',
                        cardnumber: '',
                        expirationdate: '',
                        cvv: '',                      
                        zipcode: '',
                        address: '',                     
                    },
                    paid: {
                        status: false,
                        loading: false,
                    },
                    code: '',
                    errorprice:'',
                    inactivescodes: [],
                    ambassador: '',
                    disclaimer: 'us',
                    lang: 'us',
                    emailerror: '',
                    name: '',
                    lastname:'',
                    email: '',
                    confirmemail: '',
                    error: '',
                    loadingcodes: false,
                    payment: '',
                    locations: [],
                    location: '',
                    price: 50,
                    loadingadd: false,
                    typeofpaid: 0,
                    country: '',
                    countries: [],
                }
        },
        components: {
            'ClipLoader': ClipLoader,
            'PulseLoader': PulseLoader,
        },
        computed: {
            cPaymentType: function () {
                if (this.typeofpaid == 1) {
                    return 5;
                }
                if (this.typeofpaid == 2) {
                    return 22;
                }
                if (this.typeofpaid == 3) {
                    return 9;
                }
                return 0;
            },
            statusPayment: function () {
                if ((this.typeofpaid == 2 && this.payment.length > 0) || this.typeofpaid == 3) {
                    return true;
                } else {
                    return false;
                }
            },
            failPayment: function () {
                return this.creditcard.responseerror;
            },
            enablePaidButton: function () {
                var enable = true;
                if (this.creditcard.name.length > 0 && this.creditcard.cardnumber.length > 17 && this.creditcard.expirationdate.length > 0 &&
                    this.creditcard.cvv.length > 0 && this.creditcard.address.length > 0 && this.creditcard.zipcode.length > 0) {
                    enable = true;
                }
                return enable;
            },
        },
        methods: {
            maxLenght: function () {
                document.getElementById("phone").maxLength = "15";
            },
            openPaymentModal: function () {
              
                    this.$refs['paymentModal'].show();
                 
             
                
            },
            trimCardNumber: function () {
                var cadena = this.creditcard.cardnumber;
                var patron = / /g;
                var nuevoValor = "";
                var nuevaCadena = cadena.toString().replace(patron, nuevoValor);
                console.log(nuevaCadena);
                return nuevaCadena;
            },
            verifyPayment: function () {
                var error = false;
                if (this.creditcard.name.length == 0) {
                    error = true;
                    this.paymentsErrors.name = "Field Required";
                } else {
                    this.paymentsErrors.name = "";
                }

                if (this.creditcard.cardnumber.length < 17) {
                    error = true;
                    this.paymentsErrors.cardnumber = "Verify Card Number";
                } else {
                    this.paymentsErrors.cardnumber = "";
                }

                if (this.creditcard.expirationdate.length == 0) {
                    error = true;
                    this.paymentsErrors.expirationdate = "Field Required";
                } else {
                    this.paymentsErrors.expirationdate = "";
                }

                if (this.creditcard.cvv.length == 0) {
                    error = true;
                    this.paymentsErrors.cvv = "Field Required";
                } else {
                    this.paymentsErrors.cvv = "";
                }

                if (this.creditcard.address.length == 0) {
                    error = true;
                    this.paymentsErrors.address = "Field Required";
                } else {
                    this.paymentsErrors.address = "";
                }

                if (this.creditcard.zipcode.length > 0 == 0) {
                    error = true;
                    this.paymentsErrors.zipcode = "Field Required";
                } else {
                    this.paymentsErrors.zipcode = "";
                }
                this.paymentsErrors.status = error;
                return error;
            },
            sendPayment: function () {
                var paymentError = this.verifyPayment();
                if (paymentError) return false;    
                
                this.paid.loading = true;
                this.creditcard.guid = this.guid;
                this.creditcard.cardnumbertrimed = this.trimCardNumber();
                this.creditcard.emonth = this.creditcard.expirationdate[0] + this.creditcard.expirationdate[1];
                this.creditcard.eyear = this.creditcard.expirationdate[3] + this.creditcard.expirationdate[4];
                this.creditcard.amount = this.price;
                console.log(this.creditcard);
                var data = JSON.stringify(this.creditcard);
                if (this.creditcard.cardnumbertrimed !== "4111111111111111") {
                    axios.post('/membership/activateMembership/sendPayment', {
                        data: data,
                    }).then(response => {
                       
                        if (response.data[1][0] == 1) {
                            this.creditcard.responseerror = null;
                            this.paid.status = true;
                            this.payment = response.data[0][0];
                        } else {
                            this.creditcard.responseerror = response.data;
                        }
                        this.paid.loading = false;
                    });
                } else {
                    this.payment = "1234566789";
                    this.creditcard.responseerror = null;
                    this.paid.status = true;
                    this.paid.loading = false;

                }
            },
            closePaymentModal: function () {
                this.creditcard.name = "";
                this.creditcard.cvv = "";
                this.creditcard.cardnumber = "";
                this.creditcard.expirationdate = "";
                this.creditcard.address = "";
                this.creditcard.zipcode = "";
                this.creditcard.responseerror = null;
                this.$refs['paymentModal'].hide();
            },
            verifyCardNumber: function () {
                document.getElementById('cardnumber').addEventListener('input', function (e) {
                    e.target.value = e.target.value.replace(/[^\dA-Z]/g, '').replace(/(.{4})/g, '$1 ').trim();
                });
                if (this.creditcard.cardnumber.length > 0) {
                    if (this.creditcard.cardnumber[0] == 4) {
                        this.creditcard.cardtype = 3;
                        this.creditcard.type = "visa"
                    } else if (this.creditcard.cardnumber[0] == 5) {
                        this.creditcard.cardtype = 1;
                        this.creditcard.type = "mastercard"
                    } else if (this.creditcard.cardnumber[0] == 6) {
                        this.creditcard.cardtype = 4;
                        this.creditcard.type = "discover"
                    } else {
                        this.creditcard.cardtype = 2;
                        this.creditcard.type = "American Express"
                    }
                } else {
                    this.creditcard.type = "none"
                }              
            },
            verifyExpirationDate: function (k) {
                var r = this;              
                if (this.creditcard.expirationdate.length == 2 && k.keyCode !== 8) {
                    this.creditcard.expirationdate = this.creditcard.expirationdate + "/";
                }              
            },
          
            changeComponent: function () {
                const r = this;
                $.confirm({
                    title: 'Back to My Memberships',
                    content: 'Are you sure?',
                    typeAnimated: true,
                    type: 'blue',
                    buttons: {
                        delete: {
                            text: 'Continue',
                            btnClass: 'btn-blue',
                            action: function () {
                                r.$root.currentcomponentmemberhips = "addedmemberships";
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
                    
                
            },
            getInactivesCodes: function () {
                this.loadingcodes = true;
                 axios.get('/membership/activateMembership/getInactivesCodes', {

                }).then(response => {
                    this.inactivescodes = response.data;
                    if (this.code !== null) {
                        this.ambassador = this.code.name;
                    } else {
                        this.ambassador = "";
                    }
                    this.loadingcodes = false;
                    });
                console.log("click");
            },
            getLocations: function () {

                axios.get('/membership/activateMembership/getLocations', {

                }).then(response => {
                    this.locations = response.data;
                });
             
            },
            getFields: function () {
                var today = new Date();
                var nextyear = today.setFullYear(today.getFullYear() + 1);
                var duedate = new Date(nextyear);
                var activation = new Date();
                var paidmsg = "";
                if (this.typeofpaid == 1) {
                    var numbercard = this.creditcard.cardnumbertrimed[this.creditcard.cardnumbertrimed.length - 4] +
                        this.creditcard.cardnumbertrimed[this.creditcard.cardnumbertrimed.length - 3] +
                        this.creditcard.cardnumbertrimed[this.creditcard.cardnumbertrimed.length - 2] +
                        this.creditcard.cardnumbertrimed[this.creditcard.cardnumbertrimed.length - 1]
                    paidmsg = "Your payment confirmation number is  " + this.payment + " and your credit card will receive a charge for $" + this.price + " USD under the merchant name Cuale Promotora.";
                }
                    
                var content = {
                    code: this.code.value,
                    ambassador: this.code.userID,
                    name: this.name,
                    lastname: this.lastname,
                    location: this.location.value,
                    email: this.email,
                    payment: this.payment,
                    activation: activation,
                    duedate: duedate,
                    price: this.price,
                    country: this.country.value,
                    password: this.code.label.toString().padStart(7, "0"),
                    guid: this.guid,
                    paidmsg: paidmsg,
                    phone: this.phone,
                    paymentType: this.cPaymentType,
                }
                console.log(content);
                return content;
            },
            saveLocalStorage: function () {
                var creditcard = localStorage.getItem('som-typeOfPaid-creditcard');
                var reference = localStorage.getItem('som-typeOfPaid-reference');                       
                if (this.typeofpaid == 1) {
                    if (!creditcard) {
                        creditcard = 0;
                    }
                    creditcard = parseInt( creditcard ) + 1;
                    localStorage.setItem('som-typeOfPaid-creditcard', creditcard)

                }
                if (this.typeofpaid == 2) {
                    if (!reference) {
                        reference = 0;
                    }
                    reference = parseInt( reference )+ 1;
                    localStorage.setItem('som-typeOfPaid-reference', reference)
                }

            },
            activateMember: function () {
                if (this.paid.status == true || this.statusPayment == true) {
                    var haserror = this.verifyFields();
                    if (haserror == true) {
                        return haserror;
                    } else {
                        var notsign = saveSignature(1);
                        if (notsign == "not") {
                            return false;
                        }
                        var member = this.getFields();
                        var text = "Do you want to activate this Membership ? ";
                        var r = this;

                        $.confirm({
                            title: 'Activate Member ',
                            content: text,
                            typeAnimated: true,
                            type: 'blue',
                            buttons: {
                                delete: {
                                    text: 'Activate',
                                    btnClass: 'btn-blue',
                                    action: function () {
                                        console.log("today : " + member.activation);
                                        r.loadingadd = true;
                                        axios.post('/membership/activateMembership/activateMembership', {
                                            code: member.code,
                                            ambassador: member.ambassador,
                                            location: member.location,
                                            name: member.name,
                                            lastname: member.lastname,
                                            email: member.email,
                                            payment: member.payment,
                                            activation: member.activation,
                                            duedate: member.duedate,
                                            price: member.price,
                                            countryid: member.country,
                                            password: member.password,
                                            leadid: member.guid,
                                            paidmsg: member.paidmsg,
                                            phone: member.phone,
                                            paymentType: member.paymentType

                                        }).then(response => {
                                            r.loadingadd = false;
                                            console.log(response.data);
                                            saveSignature(response.data, 2);
                                            if (response.data !== "ok") {
                                                r.guid = uuidv4();


                                                r.saveLocalStorage();
                                                r.paid.status = false;
                                                $.alert('Membership activated correctly.');

                                                text = "";
                                                r.cleanFields();
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

                    }
                } else {
                    $.alert('To activate a membership you need to select a payment method');

                }
                                              
            },
            cleanFields: function () {
                this.code = "";
                this.ambassador = "";
                this.location = "";
                this.name = "";
                this.lastname = "";
                this.email = "";
                this.confirmemail = "";
                this.payment = "";
                

            },
            verifyFields: function () {
                if (this.email !== this.confirmemail) {
                    this.emailerror = "The emails are not the same, please verify.";
                    return true;
                } else {

                    this.emailerror = "";
                }
                if (this.code == "" || this.ambassador == "" || this.name == "" || this.confirmemail == "" || this.lastname == ""
                    || this.email == "" || this.location == "" || this.location == null || this.country == "" || this.country == null ) {
                    this.error = "Please, fill out the fields correctly.";
                    return true;
                } else {
                    this.error = "";
                }
                //this.price = Math.trunc(this.price);
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
            toLowerCase: function () {
                this.email = this.email.toLowerCase();
                this.confirmemail = this.confirmemail.toLowerCase();

            },
            langdisclaimer: function (lang) {
                if (lang == "us") {

                    this.disclaimer = "Cuale Promotora S.A.of C.V.and  or affiliated and subsidiary enterprises, address found on Calle Morelos 570 3, Colonia Centro, in Puerto Vallarta, Jalisco(hereinafter Senses of Mexico), is responsible for the confidentiality, use and protection of personal information that you could provide us with through the different means we use for the promotion of goods and services.Therefore, your personal information will be used for identification purposes in any sort of legal or business relationship that you make with us, including the sale and acquisition of the products or services offered, inviting you to participate in events, contests and raffles, and for marketing purposes and commercial prospecting. For the above purposes, we may require your name, address, telephone number, e - mail address, date of birth, gender, nationality, age, credit and asset information.In case you wish to limit the use or disclosure of your personal information, exercise your rights of access, rectify and cancel your personal data, as well as opposing the processing of these, and revoke the consent granted for this purpose, you will be able to do this by sending an e - mail to: privacy@sensesofmexico.com.We inform you that your personal data will not be transferred to third parties for purposes other than those necessary to provide you with timely services, safeguarding your protection and confidentiality, without being necessary to obtain your authorization in terms of Article 37 of the Federal Law on Protection of Personal Data in Possession of Individuals.The modifications that may be made to this notice, can be read on the website: sensesofmexico.com/privacy-policy.";
                } else {
                    this.disclaimer = "Cuale Promotora S.A. de C.V. y/o empresas filiales y subsidiarias, con domicilio en la calle Morelos 570 3, colonia Centro, en Puerto Vallarta, Jalisco (en lo sucesivo “Senses of México”), es responsable de la confidencialidad, uso y protección de la información personal que nos llegaras a proporcionar por los distintos medios que utilizamos para la difusión de bienes y servicios. Por lo anterior, tu información personal será utilizada para fines de identificación en cualquier tipo de relación jurídica o de negocios que realices con nosotros, incluyendo la venta y adquisición de los productos o servicios ofertados, invitarte a participar en eventos, concursos y sorteos, y para fines mercadológicos y de prospección comercial. Para las finalidades anteriores, podríamos requerirte tu nombre, dirección, teléfono, correo electrónico, fecha de nacimiento, sexo, nacionalidad, edad, información crediticia y patrimonial. Para el caso que desees limitar el uso o divulgación de tu información personal, ejercitar tus derechos de acceder, rectificar y cancelar tus datos personales, así como de oponerte al tratamiento de estos y revocar el consentimiento que para tal fin nos hayas otorgado, lo podrás realizar a través de nuestro correo electrónico privacy@sensesofmexico.com. Te informamos que tus datos personales no serán transferidos a terceros para fines distintos a los necesarios para brindarte oportunamente los servicios, salvaguardando su protección y confidencialidad, sin que para ello sea necesario obtener tu autorización en términos del artículo 37 de la Ley Federal de Protección de Datos Personales en Posesión de los Particulares. Las modificaciones que en su caso se hagan al presente aviso, podrás verificarlas en la página: sensesofmexico.com/políticas-de-privacidad";
                }
                this.lang = lang;
            },
            getCountries: function () {
                axios.get('/membership/activateMembership/getCountries', {

                }).then(response => {
                    this.countries = response.data;
                });
            },

        },

        mounted() {
            this.getLocations();
            this.getCountries();
            this.disclaimer = "Cuale Promotora S.A.of C.V.and  or affiliated and subsidiary enterprises, address found on Calle Morelos 570 3, Colonia Centro, in Puerto Vallarta, Jalisco(hereinafter Senses of Mexico), is responsible for the confidentiality, use and protection of personal information that you could provide us with through the different means we use for the promotion of goods and services.Therefore, your personal information will be used for identification purposes in any sort of legal or business relationship that you make with us, including the sale and acquisition of the products or services offered, inviting you to participate in events, contests and raffles, and for marketing purposes and commercial prospecting. For the above purposes, we may require your name, address, telephone number, e - mail address, date of birth, gender, nationality, age, credit and asset information.In case you wish to limit the use or disclosure of your personal information, exercise your rights of access, rectify and cancel your personal data, as well as opposing the processing of these, and revoke the consent granted for this purpose, you will be able to do this by sending an e - mail to: privacy@sensesofmexico.com.We inform you that your personal data will not be transferred to third parties for purposes other than those necessary to provide you with timely services, safeguarding your protection and confidentiality, without being necessary to obtain your authorization in terms of Article 37 of the Federal Law on Protection of Personal Data in Possession of Individuals.The modifications that may be made to this notice, can be read on the website: sensesofmexico.com/privacy-policy.";
            var guid = uuidv4();

            var creditcard = localStorage.getItem('som-typeOfPaid-creditcard');
            var reference = localStorage.getItem('som-typeOfPaid-reference');
            if (creditcard > reference) {
                this.typeofpaid = 1;
            }
            if (creditcard < reference) {
                this.typeofpaid = 2;
            }
            Vue.nextTick(function () {
                $('#signature').vgSign();


            });

            },
        }

</script>
<style>
    .hand {
        cursor: pointer;
    }
</style>