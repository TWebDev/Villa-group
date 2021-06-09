<template>
    <div class="mb-5">
        <div class="text-center">
            <i class="fa fa-building fa-2x" aria-hidden="true"></i> <h3 style="position: relative">New Hotel</h3>
        </div>
        <div class="text-right">
            <button class="btn btn-primary" @click="changeComponent">
                <i class="material-icons" style="vertical-align: middle;">
                    arrow_back
                </i>Return to Hotels
            </button>
        </div>
        <div v-if="loading" class="text-center">

            <pulse-loader color="#31A3DD"></pulse-loader>
        </div>
        <div v-else>
            <!-- Formulario del hotel -->
            <div class="container">
                <b-modal :hide-footer="true" size="lg" :hide-header="true" :no-close-on-backdrop="true" ref="hotelslist" title="Similar Hotels">
                    <div v-if="loadingmodal" class="text-center">
                        {{typerequest}}
                        <pulse-loader color="#31A3DD"></pulse-loader>
                    </div>
                    <div v-else>

                        <div class="mb-3">
                            <h5>The following hotels are similar to the hotel you wrote. Select one of the list if you want to join it to Senses Of Mexico.</h5>
                        </div>

                        <div class="row m-2">
                            <b-table :per-page="perPage" striped hover
                                     :items="hotelslist" :fields="fields" :current-page="currentPage">
                                <template slot="button" slot-scope="row">

                                    <b-button variant="primary" size="sm" @click.stop="hotelSelected(row)" class="mr-2">
                                        Select
                                    </b-button>
                                </template>
                            </b-table>
                            <b-pagination size="md" :total-rows="countrows" v-model="currentPage" :per-page="perPage">
                            </b-pagination>
                        </div>
                        <div class="text-center">
                            <strong>If the hotel its a new one, clic here </strong> <button @click="hotelSelected('nuevo')" class="btn btn-primary">Its a New One!</button>
                            <button @click="hotelSelected('cancel')" class="btn btn-default">Cancel</button>
                        </div>
                    </div>
                </b-modal>
            </div>
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
                            Seo
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
                                <input class="form-control" type="text" v-model="hotelname" @change="buildUrl" @blur="verifyHotel">
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
                                <input class="form-control" type="text" v-model="phone" id="phone" @focus="maxLenght">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <span>*Destination</span>
                                <v-select v-model="destination" :options="destinations" @blur="buildUrl"></v-select>
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
                                <span>Check In</span>
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
                                <span>Check Out</span>
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
                        <div class="col-sm-6 col-md-4 col-lg-4">

                            <div class="custom-file">
                                <input type="file" ref="files" class="custom-file-input" id="customFileLang" multiple @change="upload($event)">
                                <label class="custom-file-label" for="customFileLang">Seleccionar Archivo</label>
                            </div>
                        </div>
                        <div id="preview" class="col-sm-12">
                            <div class="row">
                                <div class="col-sm-3" v-for="(url, key) in urls" v-if="url">
                                    <b-card :img-src="url"
                                            img-alt="Image"
                                            img-top
                                            tag="article"
                                            style="max-width: 30rem;"
                                            class="mb-2">
                                        <b-button @click="removePicture(key)" variant="danger"> <i class="material-icons md-48" style="vertical-align: middle;"> delete</i></b-button>
                                    </b-card>
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
                                <span>Other</span> <input type="text" class="form-control mt-1 mb-1"  name="other" value="" v-model="newactivity" />
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
                                <span>Other</span> <input type="text" class="form-control m-1" v-model="newservice" name="other" value="" /> <button class="btn btn-primary" @click="addFeature(5)">Add</button>
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
                                <span>Other</span> <input type="text" class="form-control m-1" v-model="newbenefit" name="other" value="" /><button class="btn btn-primary" @click="addFeature(3)">Add</button>
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
                        <span v-for="a in roomcount">
                            <newroom placeid="" data="new" picturesrooms=""></newroom>
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
                        <google-map></google-map>
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
            <div class="row mt-2">
                <div class="col-sm-12">
                    <div class="text-right">
                        <button class="btn btn-primary" @click="saveHotel">Save Hotel</button>
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
    </div>
</template>

<script>

    import { bus } from './app.js';
    import GoogleMap from "./GoogleMap.vue";
    import imagenn from "./image.vue";
    import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
    import CKEditor from '@ckeditor/ckeditor5-vue';
    import newroom from "./addroom.vue";

    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    Vue.component('v-select', vSelect)
    var arreglo = [];
    export default {
        props: ['cards'],
            data: function () {
                return {
                    error: '',
                    errorshortdescription: '',
                    shortdescription:'',
                    title: '',
                    roomcount:1,
                    loading: true,
                    editor: ClassicEditor,
                    perPage: 5,
                    currentPage: 1,
                    countrows: 2,
                    url: '',
                    friendlyurl: '',
                    feature: {
                        text: '',
                        id: '',
                        index: '',
                        type:'',
                    },
                    staticurl: '/boutique/',
                    loadingmodal: false,
                    fields: [
                        {
                            key: 'name',
                            label: 'Hotel Name',
                            sortable: true
                        },
                        {
                            key: 'address',
                            label: 'Address',
                            sortable: true
                        },
                        {
                            key: 'button',
                            label: 'Select',
                            
                        }

                    ],
                    active: false,

                    // campos del hotel
                    hotelname: '',
                    menu: "general info",
                    destinations: [],
                    twitter:'',
                    destination: '',
                    zones: [],
                    zone: '',
                    title:'',
                    urls: [],
                    errororientation:'',
                    slogan: '',
                    newactivity: '',
                    newservice: '',
                    newbenefit: '',
                    files:[],
                    address: '',
                    phone: '',
                    longitude: '',
                    latitude: '',
                    type: 1,
                    checkin: '',
                    checkout: '',
                    clasifications: [],
                    clasification:[],
                    orientations: [],
                    orientation:'',
                    selectedori:[],
                    description: '',
                    activities:[],
                    services: [],
                    selectedservices:[],
                    selectedactivities: [],
                    images: [],
                    selectedbenefits: [],
                    benefits: [],
                    hotelslist: [],
                    temporalhotelname: '',
                    typerequest: '',
                    formdata: [],
                    tempevent:'',
                    //*************


                }
        },
        components: {
            'PulseLoader': PulseLoader,           
            GoogleMap,
            ckeditor: CKEditor.component,
            imagenn,
            'newroom': newroom
        },
        computed: {
            cShortDescriptionLength: function () {
                return this.shortdescription.length;
            },
        },
        methods: {
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
            editFeature: function (feature, i,type) {
                console.log(i);
                this.feature.text = feature.text;
                this.feature.id = feature.value;
                this.feature.index = i;
                this.feature.type = type;


                this.$refs.features.show()
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
            removePicture: function (key) {
                this.urls.splice(key, 1);
                this.files.splice(key, 1);
                this.tempevent = this.files;
                console.log("event lenght borrado " + this.tempevent.length);
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
            changeComponent: function () {
                if (this.hotelname !== "" || this.address !== "" || this.destination !== ""
                    || this.zone !== "" ) {
                    var r = this;
                    $.confirm({
                        title: 'Changes without save',
                        content: 'You have changes without save, Do you want to continue?',
                        typeAnimated: true,
                        type: 'red',
                        buttons: {
                            delete: {
                                text: 'Continue',
                                btnClass: 'btn-blue',
                                action: function () {
                                    r.$root.currenthotels = "hotels";
                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });
                } else {
                    this.$root.currenthotels = "hotels";
                }
               
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
                    this.loading = false;
                    this.runTime();
                });
            },
            getFields: function () {

                var hotel = {
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
                    lng: this.longitude,
                    clasification: this.clasification.value,
                    placetypeid: 1,
                    sysitem: 6,
                    friendlyurl: this.friendlyurl,
                    title: this.title,
                    staticurl: this.staticurl,
                    shortdescription: this.shortdescription,
                    twitter: this.twitter,
                    active: this.active,

                  
                };
                return hotel;
            },
            maxLenght: function () {
                document.getElementById("phone").maxLength = "15";
            },
            verifyFields: function () {
                this.errororientation = ""
                var error = false;
                if (this.shortdescription.length > 200) {
                    this.errorshortdescription = "It must be 200 characters or lower.";
                    error = true;
                } else {
                    this.errorshortdescription = "";
                    error = false;
                }
                if (this.hotelname == "" || this.address == ""  || this.destination == ""
                    || this.zone == "" || this.clasification == "" || this.clasification == null) {
                    this.error = "Please complete all the require fileds '*'.";
                    this.menu = "general info";
                    return true;
                } else {
                    this.error = "";
                    if (this.selectedori.length < 1) {
                       if (this.selectedori.length < 1) {
                        this.errororientation = "Please, select at least one orientation."
                        return true;
                    } else {
                        this.errororientation = ""
                    } "Please, select at least one orientation."
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
                if (this.files == "") {
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
                console.log(errorsections);
                var cadena = errorsections.toString();
                var patron = /,/g;
                cadena = cadena.replace(patron, "");
                if (!error) {
                    if (errorsections.length == 0) {
                        var text = "Do you want to add " + this.hotelname+"?"
                    } else {
                        var text = "The following sections doesnt have info, do you want to continue?:" + cadena;
                    }

                    let files = new FormData();
                    // let file = event.target.files[0];
                    console.log("event lenght " + this.files.length);
                    for (var i = 0; i < this.files.length; i++) {
                        files.append('file', this.files[i])
                      
                    }
                    this.formdata = files;

                   var  place = JSON.stringify(hotel);
                    var r = this;
                    $.confirm({
                        title: 'New Hotel ',
                        content: text,
                        typeAnimated: true,
                        type: 'blue',
                        buttons: {
                            delete: {
                                text: 'Continue',
                                btnClass: 'btn-blue',
                                action: function () {
                                    r.loading = true;
                                    r.typerequest = "Please wait a moment...";
                                    console.log("hotel: "+ hotel);

                                    axios.post('/Content/management/addPlace', {
                                        data: place,

                                    }).then(response => {
                                     
                                        if (response.data !== "ok") {
                                            var id = response.data;
                                            bus.$emit('newroom', {
                                                id: id,
                                            });


                                            let config = {
                                                header: {
                                                    'Content-Type': 'multipart/form-data'
                                                }
                                            }

                                            if (r.files.length > 0) {
                                                files.append('id', id);
                                                files.append('sys', 6)
                                                axios.post('/Content/management/uploadImage', r.formdata, config).then(
                                                    response => {
                                                        if (response.data == "ok") {
                                                            r.files = [];
                                                            r.files = [];
                                                            r.tempevent = [];
                                                            r.typerequest = "";
                                                            r.urls = [];
                                                            r.$refs.files.files.value = "";
                                                        }
                                                    }
                                                );
                                            }

                                            r.loading = false;
                                            $.alert('Place added succefully');
                                            r.cleanFields();
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
            addFeature: function (type) {
                console.log("feature type : " + type);
                if (type == 1 && this.newactivity.trim() !== "") {
                    axios.post('/Content/management/addFeature', {
                        id: type,
                        feature: this.newactivity,

                    }).then(response => {
                        console.log("respuesta es : " + response.data);
                        this.activities.push({ value: response.data, text: this.newactivity });
                        this.selectedactivities.push(response.data);
                        this.newactivity = "";
                        $.alert('Activity added succesfully.');
                    });
                }

                if (type == 3 && this.newbenefit.trim() !== "") {
                    axios.post('/Content/management/addFeature', {
                        id: type,
                        feature: this.newbenefit,

                    }).then(response => {
                        console.log("respuesta es : " + response.data);
                        this.benefits.push({ value: response.data, text: this.newbenefit });
                        this.selectedbenefits.push(response.data);
                        this.newbenefit = "";
                        $.alert('Benefit added succesfully.');

                    });
                }
                if (type == 5 && this.newservice.trim() !== "") {
                    axios.post('/Content/management/addFeature', {
                        id: type,
                        feature: this.newservice,

                    }).then(response => {
                        console.log("respuesta es : " + response.data);
                        this.services.push({ value: response.data, text: this.newservice });
                        this.selectedservices.push(response.data);
                        this.newservice = "";
                        $.alert('Service added succesfully.');

                    });
                }


            },
            cleanFields: function () {
                this.hotelname = "";
                this.slogan = "";
                this.address = "";
                this.phone = "";
                this.zone = "";
                this.checkin = "";
                this.checkout = "";
                this.description = "";
                this.destination = "";
                this.selectedactivities = [];
                this.selectedbenefits = [];
                this.selectedori = [];
                this.selectedservices = [];
                this.latitude = "";
                this.longitude = "";
                this.urls = [];
                this.fields = [];
                this.twitter = "";
                
            },
            verifyHotel: function () {

                if (this.temporalhotelname !== this.hotelname) {

                    if (this.hotelname.trim() !== "") {

                        axios.post('/Content/management/verifyPlace', {
                            name: this.hotelname,
                            type: 1,

                        }).then(response => {
                            this.hotelslist = response.data;
                            console.log(this.hotelslist.length);
                            this.countrows = this.hotelslist.length;
                            if (this.countrows > 0) {
                                this.$refs.hotelslist.show();
                            } else {
                                this.temporalhotelname = this.hotelname;
                            }
                            
                        });
                    }
                }
                

            },
            hotelSelected: function (data) {
                var r = this;
               
                if (data == "nuevo") {
                    this.temporalhotelname = this.hotelname;
                    this.$refs.hotelslist.hide();
                }
                if (data == "cancel") {
                    this.hotelname = "";
                    this.$refs.hotelslist.hide();


                } else {
                    var text = "Do you want to join " + data.item.name + " to Senses Of Mexico?"
                    $.confirm({
                        title: 'Senses Of Mexico',
                        content: text,
                        typeAnimated: true,
                        type: 'blue',
                        buttons: {
                            delete: {
                                text: 'Join',
                                btnClass: 'btn-blue',
                                action: function () {
                                    r.loadingmodal = true;
                                    r.typerequest = "Please wait a moment...";
                                    axios.post('/Content/management/joinPlace', {
                                        id: data.item.id,
                                        sys: 6,

                                    }).then(response => {
                                        r.loadingmodal = false;
                                        r.typerequest = "";
                                        if (response.data == "existe") {
                                            $.alert('This place is already joined to Senses Of Mexico, please go to hotels to edit it.');
                                        }
                                        if (response.data == "ok") {

                                            $.confirm({
                                                title: 'Senses Of Mexico',
                                                content: 'Joined succefully, please go to Hotels to edit it,',
                                                typeAnimated: true,
                                                type: 'blue',
                                                buttons: {
                                                    delete: {
                                                        text: 'Go to Hotels',
                                                        btnClass: 'btn-blue',
                                                        action: function () {
                                                            r.$root.currenthotels = "hotels";
                                                        }
                                                    },

                                                }
                                            });
                                        }
                                        console.log(response.data);
                                    });
                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });

                }
                

            },
            runTime: function () {

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
            },
            

        },

        mounted() {
         

           

            this.getData();
            this.runTime();
            
            bus.$on('maps', obj => {
                console.log(obj);
                this.longitude = obj.lon;
                this.latitude = obj.lat;
            });
            },
        }

</script>

<style>
    .bounce-enter-active {
        animation: bounce-in .5s;
    }

    .bounce-leave-active {
        animation: bounce-in .5s reverse;
    }

    @keyframes bounce-in {
        0% {
            transform: scale(0);
        }

        50% {
            transform: scale(1.5);
        }

        100% {
            transform: scale(1);
        }
    }
    .btn0 {
        padding-top: 3px;
        padding-left: 3px;
        padding-bottom: 0px;
        padding-right: 4px;
    }
</style>