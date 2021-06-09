<template>
    <div class="mb-5">
        <div v-if="loading" class="text-center">
            <span>{{typerequest}}</span>
            <pulse-loader color="#31A3DD"></pulse-loader>
        </div>
        <div v-else>

            <div class="text-center">
                <i class="fa fa-building fa-2x" aria-hidden="true"></i> <h3 style="position: relative"> {{hotelname}} Details</h3>
            </div>
            <!-- Formulario del hotel -->
            <div class="container">
                <div class="row mb-2">
                    <div class="col-sm-12 card mt-2">
                        <div class="card-body text-center ">
                            <div class="card-title">
                            </div>
                            <button type="button" :class="{'btn btn-primary mb-2':menu=='general info',
                                    'btn btn-outline-primary mb-2': menu!=='general info'}" @click="menu = 'general info'">
                                General info
                            </button>
                            <button type="button" :class="{'btn btn-primary mb-2':menu=='images',
                                    'btn btn-outline-primary mb-2': menu!=='images'}" @click="menu = 'images'">
                                Images
                            </button>
                            <button type="button" :class="{'btn btn-primary mb-2':menu=='activities',
                                    'btn btn-outline-primary mb-2': menu!=='activities'}" @click="menu = 'activities'">
                                Activities
                            </button>
                            <button type="button" :class="{'btn btn-primary mb-2':menu=='services',
                                    'btn btn-outline-primary mb-2': menu!=='services'}" @click="menu = 'services'">
                                Services
                            </button>
                            <button type="button" :class="{'btn btn-primary mb-2':menu=='benefits',
                                    'btn btn-outline-primary mb-2': menu!=='benefits'}" @click="menu = 'benefits'">
                                Benefits
                            </button>
                            <button type="button" :class="{'btn btn-primary mb-2':menu=='rooms',
                                    'btn btn-outline-primary mb-2': menu!=='rooms'}" @click="menu = 'rooms'">
                                Rooms Type
                            </button>
                            <button type="button" :class="{'btn btn-primary mb-2':menu=='map',
                                    'btn btn-outline-primary mb-2': menu!=='map'}" @click="menu = 'map'">
                                Map
                            </button>
                            <button type="button" :class="{'btn btn-primary mb-2':menu=='seo',
                                    'btn btn-outline-primary mb-2': menu!=='seo'}" @click="buildUrl('seo')">
                                SEO
                            </button>
                            <button type="button" :class="{'btn btn-primary mb-2':menu=='credentials',
                                    'btn btn-outline-primary mb-2': menu!=='credentials'}" @click="menu = 'credentials'">
                                Credentials
                            </button>
                        </div>
                    </div>
                </div>


                <div class="row">
                    <div class="col-sm-12 mt-2 card  " v-show="menu == 'general info'">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-sm-3"></div>
                                <div class="col-sm-6 card m-2">
                                    <div class="card-body text-center">
                                        <div class="card-title">
                                            <h5>Orientation</h5>
                                        </div>
                                        <b-form-group>
                                            <b-form-checkbox-group v-model="selectedori" :options="orientations">
                                            </b-form-checkbox-group>
                                        </b-form-group>
                                        <span style="color: red"><strong>{{errororientation}}</strong></span>
                                    </div>
                                </div>
                                <div class="col-sm-3"></div>
                            </div>

                            <div class="row">
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>*Hotel Name</span>
                                    <input class="form-control" type="text" v-model="hotelname">
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>Slogan</span>
                                    <input class="form-control" type="text" v-model="slogan">
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>*Address</span>
                                    <input class="form-control" type="text" v-model="address">
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>*Phone</span>
                                    <input class="form-control" type="text" v-model="phone" ref="phone" id="phone" @focus="maxLenght">
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>*Destination</span>
                                    <v-select @change="" v-model="destination" :options="destinations"></v-select>
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>*Zone</span>
                                    <v-select v-model="zone" :options="zones"></v-select>
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>*Clasification</span>
                                    <v-select v-model="clasification" :options="clasifications"></v-select>
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>*Check In</span>
                                    <div class="form-group">
                                        <div class="input-group date" id="datetimepicker2" data-target-input="nearest">
                                            <input type="text" v-model="checkin" @blur="saveTime" class="form-control datetimepicker-input" id="checkin" data-target="#datetimepicker2" />
                                            <div class="input-group-append" data-target="#datetimepicker2" data-toggle="datetimepicker">
                                                <div class="input-group-text"><i class="fa fa-clock-o"></i></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>*Check Out</span>
                                    <div class="form-group">
                                        <div class="input-group date" id="datetimepicker3" data-target-input="nearest">
                                            <input type="text" v-model="checkout" @blur="saveTime" class="form-control datetimepicker-input" id="checkout" data-target="#datetimepicker3" />
                                            <div class="input-group-append" data-target="#datetimepicker3" data-toggle="datetimepicker">
                                                <div class="input-group-text"><i class="fa fa-clock-o"></i></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>Twitter Account</span>
                                    <input class="form-control" type="text" v-model="twitter">
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4 ">
                                    <b-form-group label="Status">
                                        <b-form-radio v-model="active" inline name="radio-inline" value="true">Active</b-form-radio>
                                        <b-form-radio v-model="active" inline name="radio-inline" value="false">Inactive</b-form-radio>
                                    </b-form-group>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <span style="color: red"><strong>{{error}}</strong></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row mb-2">
                            <div class="col-sm-12 ">
                                <span>Short Description</span>
                                <textarea class="form-control" type="text" v-model="shortdescription" rows="3"></textarea>
                                <div>{{cShortDescriptionLength}}/200</div>
                                <span style="color: red">{{errorshortdescription}}</span>
                            </div>
                            <div class=" col-sm-12">
                                <div class="">
                                    <div class="">
                                        <h5>*Description</h5>
                                    </div>
                                    <ckeditor :editor="editor" v-model="description"></ckeditor>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row" v-show="menu == 'images'">
                    <div class="col-sm-12 card mt-2">
                        <div class="card-body ">
                            <div class="card-title">
                                <h5>Images</h5>
                            </div>

                            <div id="preview" class="col-sm-12">
                                <div class="row">

                                    <div class="col-sm-12 col-md-6 col-lg-4" v-for="(url, key) in images">

                                        <b-card :img-src="'/Images/senses/'+ url.name+'?width=300&height=200&mode=crop&quality=100&autorotate=true'"
                                                img-alt="Image"
                                                img-top
                                                tag="article"
                                                style="max-width: 30rem; "
                                                class="mb-2">
                                            <b-button @click="removePicture(key, url.idsys, url.id)" variant="danger"> <i class="material-icons md-48" style="vertical-align: middle;"> delete</i></b-button>

                                            <span v-if="url.main == false">
                                                <b-button @click="selectMain(url)" variant="outline-primary"> Select as main</b-button>
                                            </span>
                                            <span v-else class="text-right">
                                                <b-button @click="selectMain(url)" variant="success"> Main</b-button>
                                            </span>
                                            <div v-if="url.logo == false" class="d-inline-flex" @click="selectLogo(url)">
                                                <div class="d-inline-flex logo hand" style="background-color: white">

                                                </div>
                                            </div>
                                            <div v-else class="d-inline-flex" @click="selectLogo(url)">
                                                <div class="d-inline-flex logo hand" style="background-color: #28a745">

                                                </div>
                                            </div>
                                        </b-card>

                                    </div>

                                </div>

                                <div class="row">
                                    <div class="col-sm-12">
                                        <hr />

                                        <div class="col-sm-6 col-md-4 col-lg-4">
                                            <div class="custom-file mb-3">
                                                <input type="file" ref="files" class="custom-file-input" id="customFileLang" multiple @change="upload($event)">
                                                <label class="custom-file-label" for="customFileLang">Seleccionar Archivo</label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-sm-3" v-for="(url, key) in urls" v-if="url">
                                                <b-card :img-src="url"
                                                        img-alt="Image"
                                                        img-top
                                                        tag="article"
                                                        style="max-width: 30rem;"
                                                        class="mb-2">

                                                    <b-button @click="removePicture(key,'new')" variant="primary"> <i class="material-icons md-48" style="vertical-align: middle;"> delete</i></b-button>
                                                </b-card>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row" v-show="menu == 'activities'">
                    <div class="col-sm-12 card mt-2">
                        <div class="card-body ">
                            <div class="card-title">
                                <h5>Activities</h5>
                            </div>
                            <b-form-group>
                                <b-form-checkbox-group id="activities" name="activities" v-model="selectedactivities">
                                    <div class="row">
                                        <div class="col-sm-6 col-md-4 col-lg-3" v-for="(act, index ) in activities">
                                            <b-form-checkbox :value="act.value">{{act.text}}</b-form-checkbox>
                                            <i style="font-size: 14px" class="material-icons  hand" @click="editFeature(act,index, 'act')">edit</i>
                                        </div>
                                    </div>
                                </b-form-checkbox-group>
                            </b-form-group>
                            <div class="row">
                                <div class="col-sm-4">
                                    <span>Other</span> <input type="text" class="form-control m-1" name="other" value="" v-model="newactivity" />
                                    <button class="btn btn-primary" @click="addFeature(1)">Add</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row" v-show="menu == 'services'">
                    <div class="col-sm-12 card mt-2">
                        <div class="card-body ">
                            <div class="card-title">
                                <h5>Services</h5>
                            </div>
                            <b-form-group>
                                <b-form-checkbox-group id="services" name="services" v-model="selectedservices">
                                    <div class="row">
                                        <div class="col-sm-6 col-md-4 col-lg-3" v-for="(ser, index) in services">
                                            <b-form-checkbox :value="ser.value">{{ser.text}}</b-form-checkbox>
                                            <i style="font-size: 14px" class="material-icons  hand" @click="editFeature(ser,index,'ser')">edit</i>
                                        </div>
                                    </div>
                                </b-form-checkbox-group>
                            </b-form-group>
                            <div class="row">
                                <div class="col-sm-4">
                                    <span>Other</span> <input type="text" class="form-control m-1" v-model="newservice" name="other" value="" />
                                    <button class="btn btn-primary" @click="addFeature(5)">Add</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row mb-2" v-show="menu == 'benefits'">
                    <div class="col-sm-12 card mt-2">
                        <div class="card-body ">
                            <div class="card-title">
                                <h5>Benefits</h5>
                            </div>
                            <b-form-group>
                                <b-form-checkbox-group id="benefits" name="benefits" v-model="selectedbenefits">
                                    <div class="row">
                                        <div class="col-sm-6 col-md-4 col-lg-3" v-for="(ben, index) in benefits">
                                            <b-form-checkbox :value="ben.value">{{ben.text}}</b-form-checkbox>
                                            <i style="font-size: 14px" class="material-icons  hand" @click="editFeature(ben,index,'ben')">edit</i>
                                        </div>
                                    </div>
                                </b-form-checkbox-group>
                            </b-form-group>
                            <div class="row">
                                <div class="col-sm-4">
                                    <span>Other</span>
                                    <input type="text" class="form-control m-1" v-model="newbenefit" name="other" value="" />
                                    <button class="btn btn-primary" @click="addFeature(3)">Add</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row mb-2" v-show="menu == 'rooms'">
                    <div class="col-sm-12 card mt-2">
                        <div class="card-body ">
                            <div class="card-title">
                                <h5>Add Room</h5>
                            </div>
                            <span v-for="room in rooms">
                                <newroom :placeid="hotelid" :data="room" :picturesrooms="picturesrooms"></newroom>
                            </span>
                            <span v-for="a in roomcount">
                                <newroom :placeid="hotelid" data="new" picturesrooms=""></newroom>
                            </span>
                            <div class="col-sm-12 text-right">
                                <button class="btn btn-primary" @click="newRoom">Add New Room</button>
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div class="row mb-2" v-show="menu== 'map' ">
                    <div class="card col-sm-12">
                        <div class="card-body">
                            <google-map :lat="latitude" :lng="longitude"></google-map>
                        </div>
                    </div>
                </div>
                <div class="row mb-2" v-show="menu== 'seo' ">
                    <div class="card col-sm-12">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-sm-6 col-md-6 col-lg-6 mt-4">
                                    <span> Title </span>
                                    <div class="input-group">
                                        <input type="text" class="form-control" id="inlineFormInputGroup" v-model="title">
                                    </div>
                                </div>
                                <div class="col-sm-6 col-md-6 col-lg-6 mt-4">
                                    <span>Friendly URL</span>
                                    <div class="input-group">
                                        <input type="text" class="form-control" id="inlineFormInputGroup" v-model="friendlyurl">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row mb-2" v-show="menu == 'credentials' ">
                    <div class="card col-sm-12">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-sm-12">
                                    <div class="col-sm-4 centerdiv">
                                        <div>Username</div>
                                        <div class="input-group">
                                            <input type="text" class="form-control" id="inlineFormInputGroup" v-model="credentials.username">
                                            <br />
                                        </div>
                                        <div class="text-muted" style="color:red !important">{{credentials.error.username}}</div>

                                        <div>Password</div>
                                        <div class="input-group">
                                            <input type="password" class="form-control" id="inlineFormInputGroup" v-model="credentials.password">
                                        </div>
                                        <div>Repeat Password</div>
                                        <div class="input-group">
                                            <input type="password" class="form-control" id="inlineFormInputGroup" v-model="credentials.repeatpassword">
                                            <br />
                                        </div>
                                        <div class="text-muted" style="color:red !important">{{credentials.error.passwords}}</div>
                                        <button v-if="credentials.hasCredentials" class="btn btn-primary mt-2" @click="updateCredentials">Update Credentials</button>
                                        <button v-else class="btn btn-primary mt-2" @click="createCredentials">Create Credentials</button>
                                    </div>
                                </div>
                                
                            </div>
                        </div>

                    </div>
                </div>
                <div class="row mt-2">
                    <div class="col-sm-12">
                        <div class="text-right">
                            <button class="btn btn-primary" @click="saveHotel">Save Hotel</button>
                            <button class="btn btn-danger" @click="unlinkHotel">Unlink</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- ********************* -->
        <b-modal ref="features" hide-footer title="Edit Feature">
            <div class="col-sm-12">
                <input class="form-control" type="text" v-model="feature.text">
            </div>
            <div class="col-12 text-right mt-3">
                <button class=" btn btn0 btn-success" @click="updateFeature('update')"><i class="material-icons">done</i></button>
                <button class=" btn  btn0 btn-danger" @click="updateFeature('delete')"><i class="material-icons">delete</i></button>
            </div>
        </b-modal>
    </div>
</template>
<script>
    import { bus } from './app.js';
    import GoogleMap from "./GoogleMap.vue";
    import newroom from "./addroom.vue";

    import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
    import CKEditor from '@ckeditor/ckeditor5-vue';
   
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    Vue.component('v-select', vSelect)
    var arreglo = [];
    export default {
        props: ['id'],
            data: function () {
                return {
                    errorshortdescription: '',
                    credentials: {
                        id: null,
                        username: '',
                        password: '',
                        repeatpassword: '',
                        placeid: '',
                        hasCredentials:false,
                        error: {
                            username: '',
                            passwords:'',
                        },
                    },
                    roomcount: 0,
                    roomname: '',
                    twitter:'',
                    price: '',
                    active: false,
                    squarefeet:'',
                    error: '',
                    hotelid: '',
                    title:'',
                    newactivity: '',
                    errororientation:'',
                    newservice: '',
                    newbenefit: '',
                    urls: [],
                    roomurls: [],   
                    roomurl: [],
                    loading: true,
                    editor: ClassicEditor,
                    generalinfo: '',
                    staticurl:'',
                    // campos del hotel
                    hotelname: '',
                    friendlyurl: '',
                    menu: "general info",
                    destinations: [],
                    destination: '',
                    zones: [],
                    zone: '',
                    slogan: '',
                    address: '',
                    phone: '',
                    longitude: '',
                    main:'',
                    latitude: '',
                    type: 1,
                    checkin: '',
                    checkout: '',
                    orientations: [],
                    orientation:'',
                    selectedori:[],
                    description: '',
                    activities:[],
                    services: [],
                    selectedservices:[],
                    selectedactivities: [],
                    images: null,
                    selectedbenefits: [],
                    benefits: [],
                    placeid: '',
                    typerequest: '',
                    files: [],
                    roomfiles: [],
                    tempevent: [],
                    roomtempevent: [],
                    clasifications: [],
                    clasification:[],
                    formdata: [],
                    roomformdata: [],
                    firstdestination: '',
                    firsthotelname: '',
                    shortdescription:'',
                    //*************
                    rooms: '',
                    picturesrooms: [],
                    feature: {
                        text: '',
                        id: '',
                        index: '',
                        type: '',
                    },

                }
        },
        components: {

            'PulseLoader': PulseLoader,
          
            GoogleMap,
            ckeditor: CKEditor.component,
            'newroom': newroom


        },
        computed: {
            cShortDescriptionLength: function () {
                return this.shortdescription.length;
            },
            logoid: function () {
                var id = 0;
                Array.from(this.images).forEach(function (data) {
                    if (data.logo == true) {
                        id = data.idsys;
                    }
                });
                return id;
            },
        },
        methods: {
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
            updateFeature: function (type, feature) {
                if (type == 'update') {
                    axios.post('/Content/management/updateFeature', {
                        id: this.feature.id,
                        text: this.feature.text,
                    }).then(response => {
                        if (response.data == 'ok') {
                            $.alert('Updated Succefully');
                            this.$refs.features.hide();
                            if (this.feature.type == 'act') {
                                this.activities[this.feature.index].text = this.feature.text;
                            }
                            if (this.feature.type == 'ser') {
                                this.services[this.feature.index].text = this.feature.text;
                            }
                            if (this.feature.type == 'ben') {
                                this.benefits[this.feature.index].text = this.feature.text;
                            }

                        }
                    });


                } else {
                    var r = this;
                    $.confirm({
                        title: 'Delete Feature',
                        content: 'Are you sure?',
                        typeAnimated: true,
                        type: 'red',
                        buttons: {
                            delete: {
                                text: 'Continue',
                                btnClass: 'btn-blue',
                                action: function () {
                                    axios.post('/Content/management/deleteFeature', {
                                        id: r.feature.id,

                                    }).then(response => {
                                        if (response.data == 'ok') {
                                            $.alert('Deleted Succefully');
                                            r.$refs.features.hide();
                                            if (r.feature.type == 'act') {
                                                r.activities.splice(r.feature.index, 1);
                                            }
                                            if (r.feature.type == 'ser') {
                                                r.services.splice(r.feature.index, 1);
                                            }
                                            if (r.feature.type == 'ben') {
                                                r.benefits.splice(r.feature.index, 1);
                                            }
                                        }
                                    });

                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });
                }

            },
            editFeature: function (feature, i, type) {
                console.log(i);
                this.feature.text = feature.text;
                this.feature.id = feature.value;
                this.feature.index = i;
                this.feature.type = type;


                this.$refs.features.show()
            },
            selectLogo: function (data) {
                Array.from(this.images).forEach(function (data) {
                    data.logo = false;
                });
                data.logo = true;
                this.logoid = data.idsys;
            },
            test: function () {
                var files = new FormData();
                files.append('id', 132423)
                var config = {
                    header: {
                        'Content-Type': 'multipart/form-data'
                    }
                }
                axios.post('/Content/management/uploadImage',files
                    
                    ,config).then(
                    response => {
                        });
            },
            newRoom: function () {
                this.roomcount = this.roomcount + 1;
            },
            buildUrl: function (evento) {
                
              
                if (this.destination == null || this.destination == "") {
                    console.log("es nulo desnitation");
                    var des = this.destination = "";
                    var destination = "";
                } else {
                    var destination = this.destination.label.toLowerCase().split(' ').join('-');
                    var des = this.destination.label;

                }
                var hotelname = this.hotelname.toLowerCase().split(' ').join('-');
                var url = destination + "/" + hotelname;
                this.friendlyurl = "/hotel/" + url;

                this.title = des + " " + this.hotelname;


                if (evento == "seo") {
                    this.menu = "seo";
                }
                this.$forceUpdate();
            },

            selectMain: function (data) {
                Array.from(this.images).forEach(function (data) {
                    data.main = false;
                });
                data.main = true;
                this.main = data.idsys;
            },
            removePicture: function (key, idsys, id) {

                if (idsys == "new") {
                    this.urls.splice(key, 1);
                    this.files.splice(key, 1);
                    this.tempevent = this.files;
                } else {
                    
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
                                    r.typerequest = "Please wait a moment...";

                                    axios.post('/Content/management/deleteImage', {
                                        id: id,
                                        idsys: idsys,
                                        sys: 6
                                    }).then(response => {

                                        if (response.data == "ok") {
                                            r.images.splice(key, 1);
                                            $.alert('Deleted succesfully.');
                                            r.typerequest = "";

                                        }
                                    });
                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });

                }

            },
            removePictureRoom: function (key, idsys, id) {


                if (idsys == "new") {
                    this.roomurls.splice(key, 1);
                    this.roomfiles.splice(key, 1);
                    this.roomtempevent = this.files;
                } else {

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
                                    r.typerequest = "Please wait a moment...";

                                    axios.post('/Content/management/deleteImage', {
                                        id: id,
                                        idsys: idsys,
                                        sys: 6
                                    }).then(response => {

                                        if (response.data == "ok") {
                                            r.images.splice(key, 1);
                                            $.alert('Deleted succesfully.');
                                            r.typerequest = "";

                                        }
                                    });
                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });

                }

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

            },
            uploadRoom: function (event) {
                // this.url = URL.createObjectURL(file);
                let uploadedFiles = this.$refs.files.files;

                /*
                  Adds the uploaded file to the files array
                */
                for (var i = 0; i < uploadedFiles.length; i++) {
                    this.roomfiles.push(uploadedFiles[i]);
                }
                for (var i = 0; i < event.target.files.length; i++) {

                    this.roomurls.push(URL.createObjectURL(event.target.files[i]));
                }
                this.roomtempevent = event.target.files;

            },
            addFeature: function (type) {
                if (type == 1 && this.newactivity.trim() !== "") {
                    axios.post('/Content/management/addFeature', {
                        id: type,
                        feature: this.newactivity,

                    }).then(response => {
                        this.activities.push({ value: response.data, text: this.newactivity });
                        this.selectedactivities.push(response.data);
                        this.newactivity = "";
                        $.alert('Activity added succesfully.');
                        })
                        .catch(function () {
                            $.alert({ title: 'Error!', content: 'Something went wrong, please retry again after sometime.', type: 'red', });
                        });
                }
               
                if (type == 3 && this.newbenefit.trim() !== "") {
                    axios.post('/Content/management/addFeature', {
                        id: type,
                        feature: this.newbenefit,

                    }).then(response => {
                        this.benefits.push({ value: response.data, text: this.newbenefit });
                        this.selectedbenefits.push(response.data);
                        this.newbenefit = "";
                        $.alert('Benefit added succesfully.');

                        }).catch(function () {
                            $.alert({ title: 'Error!', content: 'Something went wrong, please retry again after sometime.', type: 'red', });
                        });;
                }
                if (type == 5 && this.newservice.trim() !== "") {
                    axios.post('/Content/management/addFeature', {
                        id: type,
                        feature: this.newservice,

                    }).then(response => {
                        this.services.push({ value: response.data, text: this.newservice });
                        this.selectedservices.push(response.data);
                        this.newservice = "";
                        $.alert('Service added succesfully.');

                        }).catch(function () {
                            $.alert({ title: 'Error!', content: 'Something went wrong, please retry again after sometime.', type: 'red', });
                        });;
                }
                
                
            },
            changeComponent: function () {
                this.$root.currenthotels = "hotels";

            },
            getData: function () {
                
                axios.get('/Content/management/getDataPlaces', {


                }).then(response => {
                    this.destinations = response.data[0][0];
                    this.zones = response.data[1][0];
                    this.orientations = response.data[2][0];
                    this.activities = response.data[3][0];
                    this.services = response.data[4][0];
                    this.benefits = response.data[5][0];
                    this.clasifications = response.data[6][0];

                    });
                
            },
            getDataSelected: function (id) {
                this.error = "";
                this.errororientation = "";
                this.errorsections = "";
                this.loading = true;
                if (id == undefined) {
                    id = this.id;
                }
              
                axios.post('/Content/management/getDataPlaceSelected', {
                    id: id,
                    sys: 6

                }).then(response => {
                    var act = [];
                    var ori = [];
                    var ser = [];
                    var ben = [];
                    this.clasification = [];
                    if (response.data[0][0] !== null) {
                        Array.from(response.data[0][0]).forEach(function (data) {
                            act.push(data.value);
                        });
                    }
                    if (response.data[1][0] !== null) {
                        Array.from(response.data[1][0]).forEach(function (data) {
                            ori.push(data.value);
                        });
                    }
                    if (response.data[2][0] !== null) {
                        Array.from(response.data[2][0]).forEach(function (data) {
                            ser.push(data.value);
                        });
                    }
                    if (response.data[3][0] !== null) {
                        Array.from(response.data[3][0]).forEach(function (data) {
                            ben.push(data.value);
                        });
                    }
                   
                    if (response.data[4][0] !== null) {
                        this.description = response.data[4][0].value;
                        this.shortdescription = response.data[4][0].shortdescription;
                    }
                    var rooms = [];
                    if (response.data[9][0] !== null) {
                        this.rooms = response.data[9][0];
                    }
                    if (response.data[10][0] !== null) {
                        this.picturesrooms = response.data[10][0];
                    }
                    this.images = response.data[6][0];
                    if (response.data[7][0] !== null) {
                        this.clasification = {
                            value: response.data[7][0].value,
                            label: response.data[7][0].label,
                        };
                    }
                    
                   
                   
                    
                    this.selectedactivities = act;
                    this.selectedbenefits = ben;
                    this.selectedori = ori;
                    this.selectedservices = ser;
                   
                    var hotel = response.data[5][0];
                    this.hotelname = hotel.hotelname;
                    this.hotelid = hotel.hotelid;
                    this.address = hotel.address;
                    this.twitter = hotel.twitter;
                    this.slogan = hotel.slogan;
                    this.phone = hotel.phone;
                    this.checkin = hotel.checkin;
                    this.checkout = hotel.checkout;
                    this.destination = {
                        label: hotel.destinationname,
                        value: hotel.destinationid
                    };
                    this.firstdestination = hotel.destinationname;
                    this.firsthotelname = hotel.hotelname;
                    this.active = hotel.active;
                    this.latitude = hotel.lat;
                    this.longitude = hotel.lng;
                    this.longitude = hotel.lng;
                    this.zone = {
                        label: hotel.zonename,
                        value: hotel.zoneid
                    };
                    this.placeid = hotel.hotelid;
                  
                    if (response.data[8][0] !== null) {
                        this.title = response.data[8][0].title;
                        this.friendlyurl = response.data[8][0].friendlyurl;

                    } else {

                        this.firstdestination = "";
                        this.buildUrl();
                    }

                    $(function () {
                        $('#datetimepicker3').datetimepicker({

                            format: 'HH:mm'
                        });
                    });
                    $(function () {
                        $('#datetimepicker2').datetimepicker({

                            format: 'HH:mm'
                        });
                    });
                    this.checkCredentials();
                    this.loading = false;
                   
                });
            },
            getFields: function () {
                this.buildUrl();
                var hotel = {
                    placeid: this.placeid,
                    orientations: this.selectedori,
                    hotelname: this.hotelname,
                    slogan: this.slogan,
                    address: this.address,
                    phone: this.phone,
                    destinationid: this.destination.value,
                    zoneid: this.zone.value,
                    checkin: this.checkin,
                    checkout: this.checkout,
                    description: this.description,
                    activities: this.selectedactivities,
                    services: this.selectedservices,
                    benefits: this.selectedbenefits,
                    lat: this.latitude,
                    clasification: this.clasification.value,
                    lng: this.longitude,
                    placetypeid: 1,
                    sysitem: 6,
                    friendlyurl: this.friendlyurl,
                    title: this.title,
                    shortdescription: this.shortdescription,
                    logoid: this.logoid,
                    twitter: this.twitter,
                    active: this.active,


                };
                return hotel;
            },
            verifyFields: function () {
                this.errororientation = "";
                var error = false;
                if (this.shortdescription.length > 200) {
                    this.errorshortdescription = "It must be 200 characters or lower.";
                    error = true;
                } else {
                    this.errorshortdescription = "";
                    error = false;
                }
                if (this.hotelname == "" || this.address == "" || this.destination == ""
                    || this.zone == ""  || this.clasification == "" || this.clasification == null) {
                    this.error = "Please complete all the require fileds '*'.";
                    this.menu = "general info";
                    return true;
                } else {
                    this.error = "";
                    if (this.selectedori.length < 1) {
                        this.errororientation = "Please, select at least one orientation."
                        return true;
                    } else {
                        this.errororientation = ""
                    }
                    if (error) return true;
                    return false;
                }
            },
            verifySections: function () {
                var errorsections = [];
                if (this.images == null) {
                    errorsections.push("<br/>Images");
                }
                if (this.selectedactivities == "") {
                    errorsections.push("<br/>Activities");
                }
                if (this.selectedservices == "") {
                    errorsections.push("<br/>Services");
                }
                if (this.selectedbenefits == "") {
                    errorsections.push("<br/>Benefits");
                }
                if (this.latitude == "") {
                    errorsections.push("<br/>Map");
                }
                return errorsections;
            },
            saveTime: function () {
                this.checkin = $('#checkin').val();
                this.checkout = $('#checkout').val();


            },
            saveHotel: function () {
                this.buildUrl();
                var hotel = this.getFields();
                var error = this.verifyFields();
                var errorsections = this.verifySections();
                var cadena = errorsections.toString();
                var patron = /,/g;
                cadena = cadena.replace(patron, "");
                if (!error) {
                    if (errorsections.length == 0) {
                        var text = "Do you want to update " + this.hotelname+"?"
                    } else {
                        var text = "<b>The following sections do not have info, Do you want to continue?:</b>" + cadena;
                    }

                    var place = JSON.stringify(hotel);

                    let files = new FormData();
                    // let file = event.target.files[0];
                    for (var i = 0; i < this.files.length; i++) {
                        files.append('file', this.files[i])

                    }
                   
                    this.formdata = files;
                    var r = this;
                   
                    $.confirm({
                        title: 'Update Hotel ',
                        content: text,
                        typeAnimated: true,
                        type: 'blue',
                        buttons: {
                            delete: {
                                text: 'Update',
                                btnClass: 'btn-blue',
                                action: function () {
                                   
                                    r.loading = true;
                                    r.typerequest = "Please wait a moment...";
                                    
                                    axios.post('/Content/management/updatePlace', {
                                        data: place,
                                    }).then(response => {
                                        if (r.main !== "") {
                                            axios.post('/Content/management/updateMain', {
                                                id: r.placeid,
                                                idsys: r.main,
                                                sys: 6,
                                            }).then(
                                                response => {

                                                });
                                        }

                                        let config = {
                                            header: {
                                                'Content-Type': 'multipart/form-data'
                                            }
                                        }
                                        if (r.files.length > 0) {
                                            files.append('id', r.placeid);
                                            files.append('sys', 6)

                                            axios.post('/Content/management/uploadImage', r.formdata, config).then(
                                                response => {
                                                    
                                                    if (response.data == "ok") {
                                                        axios.post('/Content/management/getImagesPlace', {
                                                            id: r.placeid,
                                                            sys: 6,
                                                        }).then(
                                                            response => {
                                                                r.images = [];
                                                                r.images = response.data;
                                                                r.files = [];
                                                                r.tempevent = [];
                                                                r.formdata = [];
                                                                r.typerequest = "";
                                                                r.urls = [];
                                                                r.$refs.files.files.value = "";

                                                            });
                                                        /*
                                                        axios.post('/Content/management/updatePLaceID', {
                                                            id: r.placeid,
                                                            sys: 6
                                                         
                                                        }).then(
                                                            response => {

                                                                a
                                                                   

                                                            }
                                                        );
                                                        */
                                                    }
                                                }
                                            );
                                        }
                                        r.loading = false;
                                        $.alert('Updated succesfully.');
                                        r.typerequest = "";
                                        r.roomcount = 0;
                                        bus.$emit('updatehotel', {
                                            response: 'updated',
                                            hotelname: r.hotelname,
                                            destination: r.destination.label,
                                            zone: r.zone.label,
                                            orientations: r.selectedori,
                                        });
                                        bus.$emit('newroom', {
                                            id: '',
                                        });
                                    });
                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });
                }
            },
            maxLenght: function () {
                document.getElementById("phone").maxLength = "15";
            },
            unlinkHotel: function () {
                var r = this;
                var text = "Dou you want to unlink " + this.hotelname + " ?";
                $.confirm({
                    title: 'Unlink Hotel ',
                    content: text,
                    typeAnimated: true,
                    type: 'red',
                    buttons: {
                        Unlink: {
                            text: 'Unlink',
                            btnClass: 'btn-red',
                            action: function () {
                                r.loading = true;
                                r.typerequest = "Please wait a moment...";
                             
                                axios.post('/Content/management/unlinkPlace', {
                                    id: r.hotelid,
                                    sys: 6
                                }).then(response => {
                                    r.loading = false;
                                    bus.$emit('updatehotel', {
                                        response: 'deleted'
                                    });
                                    if (response.data == "ok") {
                                        $.alert('Unjoined succesfully.');
                                        r.typerequest = "";
                                    }
                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
            },
            getRooms: function () {
                this.rooms = "";
                this.picturesrooms = "";
                this.roomcount = 0;
                axios.post('/Content/management/GetRooms', {
                    id: this.id,
                }).then(response => {
                        this.rooms = response.data[0][0];
                        this.picturesrooms = response.data[1][0];
                });
            },


        },

        mounted() {
            let editor;
           
            ClassicEditor
                .create(document.querySelector('#editor'))
                .then(newEditor => {
                    editor = newEditor;
                })
                .catch(error => {
                });


            this.getData();
            this.getDataSelected();
            this.buildUrl();
            $(function () {
                $('#datetimepicker3').datetimepicker({

                    format: 'HH:mm'
                });
            });
            $(function () {
                $('#datetimepicker2').datetimepicker({

                    format: 'HH:mm'
                });
            });

            bus.$on('update', obj => {
                this.getDataSelected(obj.id);


                
            });
            bus.$on('maps', obj => {
                this.longitude = obj.lon;
                this.latitude = obj.lat;
            });
            bus.$on('updaterooms', obj => {
                this.getRooms();
            });
           // this.$refs.phone.maxLength = 4;
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

