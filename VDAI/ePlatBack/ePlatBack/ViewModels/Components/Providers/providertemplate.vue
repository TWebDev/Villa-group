<template>
    <div class="mb-5">
        <div v-if="provider.loading" class="text-center">           
            <pulse-loader color="#31A3DD"></pulse-loader>
        </div>
        <div v-else>
            <div class="row">
                <div class="col-12 mb-3">
                    <div class="col-sm-4 centerdiv">
                        <div class="row">
                            <div class="col-sm-12">
                                <span class="">Picture</span>
                                <div class="card" style="height: 260px; padding: 25px" v-if="!hasimage && picture == ''">
                                    <div class="custom-file" style="margin-top: 83px">
                                        <input type="file" ref="files" class="custom-file-input" id="customFileLang" @change="upload($event)">
                                        <label class="custom-file-label" for="customFileLang">Select Image </label>
                                    </div>
                                </div>
                                <div class="card" style="height: 260px" v-for="(url, key) in urls " v-if="picture == '' ">
                                    <img :src="url" class="card-img-top" style="height: 220px">
                                    <b-button @click="removePicture(key)" variant="danger"> <i class="material-icons" style="vertical-align: middle;"> delete</i></b-button>
                                </div>
                            </div>
                            <div class="col-sm-12">
                                <div class="card" style="height: 260px" v-if="picture !== ''">
                                    <img :src="'/Images/Senses/'+ picture" class="card-img-top" style="height: 220px">
                                    <b-button @click="deleteImage" variant="primary"> <i class="material-icons" style="vertical-align: middle;"> delete</i></b-button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Provider ID</span>
                    <input class="form-control" disabled type="text" v-model="provider.providerID">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Destination</span>
                    <v-select v-model="provider.destination" :options="dataProviders.destinations"></v-select>
                    <small class="text-muted" style="color: red !important;">{{provider.error.destination}}</small>
                         
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Comercial Name</span>
                    <input class="form-control" type="text" v-model="provider.comercialName">
                    <small class="text-muted" style="color: red !important;">{{provider.error.comercialName}}</small>

                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Short Name</span>
                    <input class="form-control" type="text" v-model="provider.shortName">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>RFC</span>
                    <input class="form-control" type="text" v-model="provider.rfc">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Legal Entity</span>
                    <input class="form-control" type="text" v-model="provider.legalEntity">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Phone 1</span>
                    <input class="form-control" type="text" v-model="provider.phone1">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Ext 1</span>
                    <input class="form-control" type="text" v-model="provider.ext1">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Phone 2</span>
                    <input class="form-control" type="text" v-model="provider.phone2">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Ext 2</span>
                    <input class="form-control" type="text" v-model="provider.ext2">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Contact Name</span>
                    <input class="form-control" type="text" v-model="provider.contactName">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Contact Email</span>
                    <input class="form-control" type="text" v-model="provider.contactEmail">
                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Provider Type</span>
                    <v-select v-model="provider.type" :options="dataProviders.types"></v-select>
                    <small class="text-muted" style="color: red !important;">{{provider.error.type}}</small>

                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <span>Contract Currency</span>
                    <v-select v-model="provider.currency" :options="dataProviders.currencies"></v-select>
                    <small class="text-muted" style="color: red !important;">{{provider.error.currency}}</small>

                </div>
                <div class="col-sm-6 col-md-4 col-lg-3">
                    <b-form-group label="Is Active">
                        <b-form-radio-group v-model="provider.isActive"
                                            :options="dataProviders.isActive"></b-form-radio-group>
                    </b-form-group>
                </div>

            </div>
           
            <div class="row">
                <div class="col-sm-12">
                    <span>Short Description</span>
                    <b-form-group label="Html Format">
                        <b-form-radio-group v-model="provider.htmlFormat"
                                            :options="dataProviders.htmlFormat"></b-form-radio-group>
                    </b-form-group>
                    <div v-show="!provider.htmlFormat">
                        <textarea class="form-control" type="text" v-model="provider.shortDescription"></textarea>
                        <div>{{cShortDescriptionLength}}/200</div>
                        <small class="text-muted" style="color: red !important;">{{provider.error.shortDescriptionLength}}</small>
                    </div>
                    <div v-show="provider.htmlFormat">
                        <ckeditor :editor="editor" v-model="provider.shortDescriptionHtml"></ckeditor>
                    </div>
                </div>
                <div class="col-sm-12 mt-2">
                    <span>Description</span>
                    <ckeditor :editor="editor" v-model="provider.fullDescription"></ckeditor>
                </div>
            </div>
            <div class="row">
                <div v-if="provider.saving" class="col-sm-12 text-center m-3">
                    <pulse-loader color="#31A3DD"></pulse-loader>
                </div>
                <div class="col-sm-12 text-right">
                    <b-button style="margin-top: 17px;" v-if="provider.providerID !== null" class="text-right" @click="deleteProvider" variant="danger">
                        <i class="material-icons md-48" style="vertical-align: middle;"> delete</i>
                    </b-button>
                    <button class="btn btn-primary mt-3" @click="saveProvider">Save</button>
                </div>
            </div>
        </div>                       
        {{cProviderID}}
     </div>
</template>

<script>

    

    import { bus } from './app.js';
    import GoogleMap from "./GoogleMap.vue";
    import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
    import CKEditor from '@ckeditor/ckeditor5-vue';  
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    Vue.component('v-select', vSelect)
    var arreglo = [];
    export default {
        props: ['providerID'],
            data: function () {
                return {
                    picture: '',
                    errorshortdescription:'',
                    hasimage: false,
                    editor: ClassicEditor,
                    files: [],
                    urls: [],
                    tempevent:[],
                    provider: {
                        terminalID: 62,
                        comercialName: '',
                        shortName:'',
                        providerID: null,
                        loading: false,
                        isActive: false,
                        description: '',
                        htmlFormat: false,
                        fullDescription:'',
                        shortDescription: '',
                        shortDescriptionHtml: '',
                        shortDescriptionFinal:'',
                        destination: null,
                        sysItemType: 15,
                        rfc: '',
                        phone1: '',
                        phone2: '',
                        ext1: '',
                        ext2: '',
                        type: null,
                        currency: null,
                        isActive: false,
                        seo: {
                            friendlyUrl: '',
                            title:'',
                        },
                        error: {
                            comercialName: '',
                            destination: '',
                            currency: '',
                            type: '',
                            shortDescriptionLength:'',
                        },
                        saving: false,
                        culture: 'en-US',
                        destinationID: 0,
                        currencyID: 0,
                        typeID: 0,


                    },
                    dataProviders: {
                        destinations: [],
                        types: [],
                        currencies: [],
                        isActive: [
                            { text: 'Yes', value: true },
                            { text: 'No', value: false },
                        ],
                         htmlFormat: [
                            { text: 'Yes', value: true },
                            { text: 'No', value: false },
                        ]
                    }
                }
        },
        components: {
            'PulseLoader': PulseLoader,          
            GoogleMap,
            ckeditor: CKEditor.component,       
        },
        computed: {
            cShortDescriptionLength: function () {
                if (this.provider.shortDescription !== null) {
                    return this.provider.shortDescription.length;
                } else {
                    return 0;
                }
            },
            cProviderID: function () {
                if (this.providerID !== undefined) {
                    this.provider.providerID = this.providerID;
                }
                return "";
            },
        },
        methods: {
            deleteProvider: function () {
                var r = this;
                $.confirm({
                    title: 'Delete Provider',
                    content: 'Are you sure ?',
                    typeAnimated: true,
                    type: 'red',
                    buttons: {
                        delete: {
                            text: 'Delete',
                            btnClass: 'btn-danger',
                            action: function () {
                                axios.post('/Content/management/deleteProvider', {
                                    id: r.provider.providerID,
                                }).then(response => {
                                    $.alert('Provider deleted correctly.');
                                    bus.$emit('updateProviders', {
                                    });
                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });

            },
            saveProvider: function () {
                const r = this;
                this.buildUrl();
                var error = this.verifyFields();
                console.log(this.provider.seo.friendlyUrl);
                if (error) return false;
                if (this.provider.htmlFormat) {
                    this.provider.shortDescriptionFinal = this.provider.shortDescriptionHtml;
                } else {
                    this.provider.shortDescriptionFinal = this.provider.shortDescription;
                }
                console.log(this.provider);
                //imagen variables
                let files = new FormData();
                // let file = event.target.files[0];
                for (var i = 0; i < this.files.length; i++) {
                    files.append('file', this.files[i])
                }
                this.formdata = files;
                let config = {
                    header: {
                        'Content-Type': 'multipart/form-data'
                    }
                }
                //imagen
                $.confirm({
                    title: 'Save Provider ',
                    content: 'Do you want to continue?',
                    typeAnimated: true,
                    type: 'blue',
                    buttons: {
                        delete: {
                            text: 'continue',
                            btnClass: 'btn-blue',
                            action: function () {
                                r.provider.currencyID = r.provider.currency.value;
                                r.provider.destinationID = r.provider.destination.value;
                                r.provider.typeID = r.provider.type.value;
                                var providerInfo = JSON.stringify(r.provider);
                                var url = "/Content/management/saveProvider";
                                var data = {
                                    data: providerInfo,
                                };
                                if (r.provider.providerID !== null) {
                                    url = "/Content/management/updateProvider";
                                    data = {
                                        providerID: r.provider.providerID,
                                        data: providerInfo,
                                    };
                                }
                                r.provider.saving = true;
                                axios.post(url, data).then(response => {
                                    r.provider.providerID = response.data;
                                    files.append('id', response.data);
                                        axios.post('/Content/management/UploadImageProvider', r.formdata, config).then(
                                            response => { 
                                                if (response.data !== "not") {
                                                    r.picture = response.data;
                                                }
                                          
                                            r.provider.saving = false;
                                                $.alert('Provider saved correctly.');
                                                if (url =="/Content/management/saveProvider") {
                                                    bus.$emit('updateProviders', {
                                                    });
                                                }
                                                                              
                                            });
                                    });
    
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
            },
            deleteImage: function () {
                var id = this.provider.providerID;
                var r = this;
                var text = "Do you want to delete this image ?";
                $.confirm({
                    title: 'Delete Image ',
                    content: text,
                    typeAnimated: true,
                    type: 'red',
                    buttons: {
                        delete: {
                            text: 'Delete',
                            btnClass: 'btn-red',
                            action: function () {

                                axios.post('/Content/management/DeleteImageProvider', {
                                    id: id,

                                }).then(response => {

                                    if (response.data == "ok") {
                                        r.picture = "";
                                        r.removePicture();
                                        $.alert('Deleted succesfully.');
                                    }
                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });

            },
            removePicture: function (key) {
                this.hasimage = false;
                this.urls = [];
                this.files = [];
                this.tempevent = [];
            },
            upload: function (event) {
                // this.url = URL.createObjectURL(file);
                let uploadedFiles = this.$refs.files.files;
                /*
                  Adds the uploaded file to the files array
                */
                for (var i = 0; i < uploadedFiles.length; i++) {
                    this.files.push(uploadedFiles[i]);
                }
                for (var i = 0; i < event.target.files.length; i++) {

                    this.urls.push(URL.createObjectURL(event.target.files[i]));
                }
                this.tempevent = event.target.files;
                this.hasimage = true;
            },
            checkCredentials: function () {
                this.credentials.id = 0;
                this.credentials.hasCredentials = false;
                this.credentials.error.username = "";
                this.credentials.error.passwords = "";
                this.credentials.password = "";
                this.credentials.repeatpassword = "";
                axios.post('/membership/activateMembership/checkCredentials', {
                    placeid: this.placeid
                }).then(response => {
                    if (response.data !== "") {
                        this.credentials.hasCredentials = true;
                        this.credentials.username = response.data.username;
                        this.credentials.id = response.data.simpleAuthenticationID;
                    } else {
                        this.credentials.username = "";
                    }
                });
            },
            createCredentials: function () {
                console.log(this.credentials);
                var error = this.verifyCredentials();
                if (!error) {
                    this.credentials.placeid = this.placeid;
                    var data = JSON.stringify(this.credentials);
                    axios.post('/membership/activateMembership/createCredentialsPlaces', {
                        data: data
                    }).then(response => {
                        if (response.data == "ok") {
                            $.alert('Credentials createad succefully');
                            this.credentials.hasCredentials = true;
                            this.credentials.password = "";
                            this.credentials.repeatpassword = "";
                        } else {
                            this.credentials.error.username = "The username it's already taken, please write another one."
                        }
                    });
                }
            },
            updateCredentials: function () {
                console.log(this.credentials);
                var error = this.verifyCredentials();
                if (!error) {
                    this.credentials.placeid = this.placeid;
                    var data = JSON.stringify(this.credentials);
                    axios.post('/membership/activateMembership/updateCredentialsPlaces', {
                        data: data
                    }).then(response => {
                        if (response.data == "ok") {
                            $.alert('Credentials updated succefully');
                            this.credentials.has = true;
                            this.credentials.password = "";
                            this.credentials.repeatpassword = "";
                        } else {
                            this.credentials.error.username = "The username it's already taken, please write another one."
                        }
                    });
                }
            },
            verifyCredentials: function () {
                var error = false;
                if (this.credentials.username.trim() == "") {
                    this.credentials.error.username = "Field Required"
                    error = true;
                } else {
                    this.credentials.error.username = ""
                }
                if (this.credentials.password.trim() !== this.credentials.repeatpassword.trim() ||
                    (this.credentials.password.trim() == "" && this.credentials.repeatpassword.trim() == "")) {
                    this.credentials.error.passwords = "Passwords do not match"
                    error = true;
                } else {
                    this.credentials.error.passwords = ""
                }
                return error;
            },
           
                    
            buildUrl: function (evento) {
                var destination = "";
                if (this.provider.destination !== null ) {
                    destination = this.provider.destination.label.toLowerCase().split(' ').join('-');                    
                } 

                var provider = this.provider.comercialName.toLowerCase().split(' ').join('-');             
                this.provider.seo.friendlyUrl = "/activities/providers/" + destination + "/" + provider;
                this.provider.seo.title = destination + " " + this.provider.comercialName;               
            },

            GetInfoForProviders: function () {
                this.provider.loading = true;
                axios.get('/Content/management/GetInfoForProviders', {
                }).then(response => {
                    this.dataProviders.destinations = response.data[0][0];
                    this.dataProviders.types = response.data[1][0];
                    this.dataProviders.currencies = response.data[2][0];
                    this.provider.loading = false;

                });
            },                 
            changeComponent: function () {
                this.$root.currentspa = "spas";

            },                     
            verifyFields: function () {
                var error = false;
                this.provider.error.destination = "";
                this.provider.error.comercialName = "";
                this.provider.error.type = "";
                this.provider.error.currency = "";
                this.provider.error.shortDescriptionLength = "";
                if (this.cShortDescriptionLength > 200) {
                    error = true
                    this.provider.error.shortDescriptionLength = "It must be 200 characters or lower.";
                }
                if (this.provider.destination == null) {
                    error = true;
                    this.provider.error.destination = "Field Required"
                }
                if (this.provider.currency == null) {
                    error = true;
                    this.provider.error.currency = "Field Required"
                }
                if (this.provider.type == null) {
                    error = true;
                    this.provider.error.type = "Field Required"
                }
                if (this.provider.comercialName.trim() == "") {
                    error = true;
                    this.provider.error.comercialName = "Field Required"
                }
                return error;
            },
          
            saveTime: function () {
                this.openat = $('#checkin').val();
                this.closedat = $('#checkout').val();


            },           
            maxLenght: function () {
                document.getElementById("phone").maxLength = "15";
            },
            getProvider: function () {

                axios.post('/Content/management/getProvider', {
                    providerID: this.provider.providerID
                }).then(response => {
                    this.provider.comercialName = response.data[0][0].name;
                    this.provider.shortName = response.data[0][0].shortName;
                    this.provider.rfc = response.data[0][0].rfc;
                    this.provider.phone1 = response.data[0][0].phone1;
                    this.provider.phone2 = response.data[0][0].phone2;
                    this.provider.ext1 = response.data[0][0].ext1;
                    this.provider.ext2 = response.data[0][0].ext2;
                    this.provider.contactName = response.data[0][0].contactName;
                    this.provider.contactEmail = response.data[0][0].contactEmail;
                    this.provider.legalEntity = response.data[0][0].legalName;

                    if (response.data[0][0].providerTypeID !== null) {
                        this.provider.type = {
                            label: response.data[0][0].providerType,
                            value: response.data[0][0].providerTypeID
                        };
                    }
                    if (response.data[0][0].contractCurrencyID !== null) {
                        this.provider.currency = {
                            label: response.data[0][0].contractCurrency,
                            value: response.data[0][0].contractCurrencyID
                        };
                    }
                    if (response.data[0][0].destinationID !== null) {
                        this.provider.destination = {
                            label: response.data[0][0].destination,
                            value: response.data[0][0].destinationID
                        };
                    }
                    if (response.data[0][0].picture == null) {
                        this.picture = "";

                    } else {
                        this.picture = response.data[0][0].picture;
                    }
                    this.provider.isActive = response.data[0][0].isActive;
                    if (response.data[1][0] !== null) {
                        this.provider.fullDescription = response.data[1][0].fullDescription;
                        this.provider.shortDescription = response.data[1][0].shortDescription;
                    }
                  

                });
            },
        },

        mounted() {
            console.log(this.provider.providerID);
            if (this.provider.providerID !== null) {
                this.getProvider();
            }
            //editor
            let editor;
            ClassicEditor
                .create(document.querySelector('#editor'))
                .then(newEditor => {
                    editor = newEditor;
                })
                .catch(error => {
                });
            this.GetInfoForProviders();                
                  
            bus.$on('maps', obj => {
                this.longitude = obj.lon;
                this.latitude = obj.lat;
            });
           
         
            },
        }

</script>
<style>
    .logo {
        position: absolute;
        width: 31px;
        height: 30px;
        border-radius: 100px;
        top: 0;
        right: 0;
        margin: 10px;
        box-shadow: rgba(0, 0, 0, 0.2) 0px 4px 8px 0px, rgba(0, 0, 0, 0.19) 0px 6px 20px 0px;
    }
    .centerdiv {
        position: relative;
        margin: auto;
    }
</style>
