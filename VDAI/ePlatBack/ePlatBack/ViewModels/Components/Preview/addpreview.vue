<template>
    <div class="mb-5">
        <div class="row">
            <div class="col-sm-8">
                <h2 class="mb-4"> Send a Free Trial </h2>
            </div>
            <div class="col-sm-4">
                <div class="text-right mb-2">
                    <div class="">
                        <button class="btn btn-primary" @click="changeComponent">
                            <i class="material-icons" style="vertical-align: middle;">
                                arrow_back
                            </i> Back to My Free Trails
                        </button>
                    </div>
                </div>
            </div>
        </div>     
        <div class="container-fluid mt-4">
            <div class="row">
                <div class="card col-sm-12">
                    <div class="card-body">
                        <h5 class="card-title">Send Free Trial</h5>
                        <div class="row">
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Name</span>
                                <input class="form-control" type="text" v-model="name">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Last Name</span>
                                <input class="form-control" type="text" v-model="lastname">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>Location</span>
                                <v-select v-model="location" :options="locations"></v-select>
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
                                <span>Days for Free Trial</span>
                                <input class="form-control" type="number" v-model="freeTrialDays">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span> Booking Status</span>
                                <b-form-select v-model="bookingStatus" :options="bookingStatusOptions"></b-form-select>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <b-form-group label="Send E-mail">
                                    <b-form-radio v-model="sendEmail" name="some-radios" value="true">Yes</b-form-radio>
                                    <b-form-radio v-model="sendEmail" name="some-radios" value="false">Not</b-form-radio>
                                </b-form-group>
                             </div>
                                <div class="mt-2 text-center">
                                    <span style="color: red"> <b>{{error}}</b> </span>
                                </div>
                            </div>

                            <div class="row mt-3">

                                <div class="col-sm-6">
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
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label>Comments</label>
                                        <textarea v-model="comment" class="form-control" id="exampleFormControlTextarea1" rows="4"></textarea>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 mt-2 text-right">
                                    <button class="btn btn-primary" @click="sendPreview">Send</button>
                                </div>
                            </div>
                            <div v-if="loadingadd" class="text-center">
                                <strong> Sending Preview...</strong>

                                <pulse-loader color="#31A3DD"></pulse-loader>
                            </div>
                        </div>
                </div>
            </div>
        </div>
    </div>




</template>

<script>

   
    var signature = require('../../../Scripts/signatureMembership.js');
    import saveSignature from '../../../Scripts/signatureMembership.js'
   import axios from 'axios';
    import vSelect from 'vue-select';
    import ClipLoader from 'vue-spinner/src/ClipLoader.vue'
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'

    export default {
        props: ['cards'],
        data: function () {
            return {
                sendEmail: true,
                bookingStatus: '',
                bookingStatusOptions:[],
                comment:'',
                phone:'',
                freeTrialDays: '',
                code: '',
                disclaimer: 'us',
                lang: 'us',
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
                loadingadd: false,
                firma: 'firma',

            }
        },
        components: {
            'ClipLoader': ClipLoader,
            'PulseLoader': PulseLoader,
        },
        methods: {
            maxLenght: function () {
                document.getElementById("phone").maxLength = "15";
            },
            changeComponent: function () {
                this.$root.currentcomponentpreview = "preview";

            },
            getLocations: function () {
                axios.get('/membership/activateMembership/getLocations', {
                }).then(response => {
                    this.locations = response.data;
                });
            },
            getFields: function () {              
                var content = {
                    name: this.name,
                    lastname: this.lastname,
                    location: this.location.value,
                    email: this.email,
                    freeTrialDays: this.freeTrialDays,
                    phone: this.phone,
                    comment: this.comment   ,
                    bookingStatus: this.bookingStatus
                }
                return content;
            },
            sendPreview: function () {             
                var haserror = this.verifyFields();
                if (haserror == true) {
                    return haserror;
                } else {
                    var notsign = saveSignature(1);
                    if (notsign == "not") {
                        return false;
                    }
                    var member = this.getFields();
                    console.log(this.sendEmail);
                    var text = "Do you want to send the Free Trial? ";
                    var r = this;
                    $.confirm({
                        title: 'Send Preview ',
                        content: text,
                        typeAnimated: true,
                        type: 'blue',
                        buttons: {
                            delete: {
                                text: 'Send',
                                btnClass: 'btn-blue',
                                action: function () {

                                    r.loadingadd = true;
                                    axios.post('/membership/Preview/addPreview', {
                                        location: member.location,
                                        name: member.name,
                                        lastname: member.lastname,
                                        email: member.email,
                                        freeTrialDays: member.freeTrialDays,
                                        phone: member.phone,
                                        comment: member.comment,
                                        bookingStatus: member.bookingStatus,
                                        sendEmail: r.sendEmail

                                    }).then(response => {
                                        r.loadingadd = false;
                                        console.log(response.data);
                                        saveSignature(response.data,1);


                                        if (response.data !== "ok") {
                                            $.alert('Preview was sent correctly.');
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
            },
            cleanFields: function () {
                this.location = "";
                this.name = "";
                this.lastname = "";
                this.email = "";
                this.confirmemail = "";
                this.freeTrialDays = "";
            },
            verifyFields: function () {
                var fail = false;
                this.emailerror = "";
                this.error = "";
                console.log(this.freeTrialDays.trim());
                if (this.email !== this.confirmemail) {
                    this.emailerror = "The emails are not the same, please verify.";
                    fail = true;
                }
                if (this.name == "" || this.confirmemail == "" || this.lastname == ""
                    || this.email == "" || this.location == "" || this.location == null || this.freeTrialDays.trim() == "") {
                    this.error = "Please, fill out the fields correctly.";
                    fail = true;
                } 
                var email = this.email;
                if (email.indexOf('@') != -1) {
                } else {
                    this.emailerror = "the email doesn't have an @ ";
                    fail = true;
                }
                return fail;
            },
            toLowerCase: function () {
                this.email = this.email.toLowerCase();
                this.confirmemail = this.confirmemail.toLowerCase();

            },
            langdisclaimer: function (lang) {
                if (lang == "us") {
                   
                    this.disclaimer = "Cuale Promotora S.A.of C.V.and  or affiliated and subsidiary enterprises, address found on Calle Morelos 570 3, " +
                        "Colonia Centro, in Puerto Vallarta, Jalisco(hereinafter Senses of Mexico), is responsible for the confidentiality, use " +
                        "and protection of personal information that you could provide us with through the different means we use for the promotion of" +
                        " goods and services.Therefore, your personal information will be used for identification purposes in any sort of legal or business" +
                        " relationship that you make with us, including the sale and acquisition of the products or services offered, inviting you to participate" +
                        " in events, contests and raffles, and for marketing purposes and commercial prospecting. For the above purposes, we may require your name," +
                        " address, telephone number, e - mail address, date of birth, gender, nationality, age, credit and asset information.In case you wish to limit" +
                        " the use or disclosure of your personal information, exercise your rights of access, rectify and cancel your personal data, as well as opposing" +
                        " the processing of these, and revoke the consent granted for this purpose, you will be able to do this by sending an e - mail to: " +
                        "privacy@sensesofmexico.com.We inform you that your personal data will not be transferred to third parties for purposes other " +
                        "than those necessary to provide you with timely services, safeguarding your protection and confidentiality, without being necessary" +
                        " to obtain your authorization in terms of Article 37 of the Federal Law on Protection of Personal Data in Possession of Individuals.The" +
                        " modifications that may be made to this notice, can be read on the website: sensesofmexico.com/privacy-policy.";
                } else {
                    this.disclaimer = "Cuale Promotora S.A. de C.V. y/o empresas filiales y subsidiarias, con domicilio en la calle Morelos 570 3,"+
                        " colonia Centro, en Puerto Vallarta, Jalisco(en lo sucesivo “Senses of México”), es responsable de la confidencialidad, uso y"+
                        " protección de la información personal que nos llegaras a proporcionar por los distintos medios que utilizamos para la" +
                        " difusión de bienes y servicios.Por lo anterior, tu información personal será utilizada para fines de identificación en " +
                        "cualquier tipo de relación jurídica o de negocios que realices con nosotros, incluyendo la venta y adquisición de los productos" +
                        " o servicios ofertados, invitarte a participar en eventos, concursos y sorteos, y para fines mercadológicos y de prospección comercial." +
                        "Para las finalidades anteriores, podríamos requerirte tu nombre, dirección, teléfono, correo electrónico, fecha de nacimiento, sexo," +
                        " nacionalidad, edad, información crediticia y patrimonial.Para el caso que desees limitar el uso o divulgación de tu información personal," +
                        " ejercitar tus derechos de acceder, rectificar y cancelar tus datos personales, así como de oponerte al tratamiento de estos y revocar el" +
                        " consentimiento que para tal fin nos hayas otorgado, lo podrás realizar a través de nuestro correo electrónico privacy@sensesofmexico.com.Te " +
                        "informamos que tus datos personales no serán transferidos a terceros para fines distintos a los necesarios para brindarte oportunamente los " +
                        "servicios, salvaguardando su protección y confidencialidad, sin que para ello sea necesario obtener tu autorización en términos del artículo" +
                        " 37 de la Ley Federal de Protección de Datos Personales en Posesión de los Particulares.Las modificaciones que en su caso se hagan al presente" +
                        " aviso, podrás verificarlas en la página: sensesofmexico.com/políticas-de-privacidad";
                }
                this.lang = lang;
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
             this.getLocations();
            this.disclaimer = "Cuale Promotora S.A.of C.V.and  or affiliated and subsidiary enterprises, address found on Calle Morelos 570 3, " +
                "Colonia Centro, in Puerto Vallarta, Jalisco(hereinafter Senses of Mexico), is responsible for the confidentiality, use " +
                "and protection of personal information that you could provide us with through the different means we use for the promotion of" +
                " goods and services.Therefore, your personal information will be used for identification purposes in any sort of legal or business" +
                " relationship that you make with us, including the sale and acquisition of the products or services offered, inviting you to participate" +
                " in events, contests and raffles, and for marketing purposes and commercial prospecting. For the above purposes, we may require your name," +
                " address, telephone number, e - mail address, date of birth, gender, nationality, age, credit and asset information.In case you wish to limit" +
                " the use or disclosure of your personal information, exercise your rights of access, rectify and cancel your personal data, as well as opposing" +
                " the processing of these, and revoke the consent granted for this purpose, you will be able to do this by sending an e - mail to: " +
                "privacy@sensesofmexico.com.We inform you that your personal data will not be transferred to third parties for purposes other " +
                "than those necessary to provide you with timely services, safeguarding your protection and confidentiality, without being necessary" +
                " to obtain your authorization in terms of Article 37 of the Federal Law on Protection of Personal Data in Possession of Individuals.The" +
                " modifications that may be made to this notice, can be read on the website: sensesofmexico.com/privacy-policy.";
            Vue.nextTick(function () {
                $('#signature').vgSign();
             

            });

        },
    }

</script>
<style>
    .centerbutton {
        position: absolute;
        margin: auto;
        bottom: 0;
        top: 0;
        height: 8%;
    }
</style>
