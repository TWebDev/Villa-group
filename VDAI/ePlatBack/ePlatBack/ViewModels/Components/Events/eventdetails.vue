<template>
    <div class="mb-5">
        <div v-if="loading" class="text-center mt-3">
           
            <pulse-loader color="#31A3DD"></pulse-loader>
        </div>
        <div v-else>
          
            <div class="text-center mt-4">
                <h3 style="position: relative"> {{event}} Details</h3>
                {{updateevent}}
            </div>
            <!-- Formulario del Place -->
            <div class="container">
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-sm-3"></div>
                            <div class="col-sm-6 card m-2">
                                <div class="card-body text-center">
                                    <div class="card-title">
                                        <h5>Orientation</h5>
                                    </div>
                                    <b-form-group>
                                        <b-form-checkbox-group v-model="orientations" :options="allorientations">
                                        </b-form-checkbox-group>
                                    </b-form-group>
                                </div>
                            </div>
                            <div class="col-sm-3"></div>
                        </div>
                        <div class="row">
                            <div class="col-sm-12">
                                <h5>Create Event</h5>
                                <hr />
                            </div>
                            <div class="col-sm-12 text-right ">
                                <b-form-group >
                                    <b-form-radio v-model="active" inline name="radio-inline" value="true">Active</b-form-radio>
                                    <b-form-radio v-model="active" inline name="radio-inline" value="false">Inactive</b-form-radio>
                                </b-form-group>
                            </div>
                            <div class="col-sm-4">
                                <div class="row">
                                    <div class="col-sm-12">
                                        Picture
                                    </div>
                                    <div class="col-sm-12">
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
                            <div class="col-sm-8">
                                <b-form-group id="exampleInputGroup2"
                                              label="Event"
                                              label-for="event">
                                    <b-form-input id="event"
                                                  type="text"
                                                  v-model="event"
                                                  @change="buildUrl"
                                                  required>
                                    </b-form-input>
                                </b-form-group>
                                <b-form-group id="exampleInputGroup2"
                                              label="Description"
                                              label-for="description">
                                    <b-form-textarea id="description"
                                                     type="text"
                                                     rows="3"
                                                     v-model="description"
                                                     required>
                                    </b-form-textarea>
                                </b-form-group>
                                <div class="row">
                                    <div class="col-sm-6">
                                        <b-form-group id="exampleInputGroup2"
                                                      label="link"
                                                      label-for="link">
                                            <b-form-input id="link"
                                                          type="text"
                                                          v-model="link"
                                                          required>
                                            </b-form-input>
                                        </b-form-group>
                                    </div>
                                    <div class="col-sm-6">
                                        <a href="#" @click="openLocations">Select location</a>
                                        <b-form-input id="location"
                                                      type="text"
                                                      v-model="location"
                                                      disabled
                                                      style="margin-top: 7px;"
                                                      required>
                                        </b-form-input>
                                    </div>
                                    <b-modal ref="modalLocations" size="lg" hide-footer title="Select a location">
                                        <div class="row">
                                            <div class="col-sm-12">
                                                <h5>Find a location</h5>
                                            </div>
                                            <div class="col-sm-12">
                                                <v-select v-model="locationselected" :options="places"></v-select>
                                            </div>
                                        </div>
                                        <div class="row " v-if="haslocation">
                                            <div class="col-sm-12 mt-5">
                                                <h5>Address</h5>
                                            </div>
                                            <div class="col-sm-12">
                                                <p>{{locationselected.address}}</p>
                                            </div>
                                            <div class="col-sm-6">
                                                <h5>Destination</h5>
                                                <p>{{locationselected.destination}}</p>
                                            </div>
                                            <div class="col-sm-6">
                                                <h5>Zone</h5>
                                                <p>{{locationselected.zone}}</p>
                                            </div>

                                            <div class="col-sm-12 text-right">
                                                <button class="btn btn-primary mt-3" @click="selectLocation">Select location</button>
                                            </div>
                                        </div>



                                    </b-modal>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <b-form-group>
                                    <b-form-checkbox-group v-model="repeat" name="options" :options="options">
                                    </b-form-checkbox-group>
                                    <b-form-checkbox id="checkbox1"
                                                     v-model="isaffiliate">
                                        Is Affiliate
                                    </b-form-checkbox>
                                </b-form-group>
                            </div>
                            <div class="col-sm-8">
                                <div class="row">
                                    <div class="col-sm-6">
                                        <div class="row">
                                            <div class="col-sm-6">

                                                <span>From Date</span>
                                                <div class="input-group date" id="datetimepicker" data-target-input="nearest" style="margin-top: 7px">
                                                    <input type="text" id="datefrom" v-model="datefrom" @blur="saveTime" class="form-control datetimepicker-input" data-target="#datetimepicker" />
                                                    <div class="input-group-append" data-target="#datetimepicker" data-toggle="datetimepicker">
                                                        <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                    </div>
                                                </div>

                                            </div>
                                            <div class="col-sm-3">
                                                <b-form-group id="exampleInputGroup2"
                                                              label="Hrs"
                                                              label-for="event">
                                                    <b-form-select id="event"
                                                                   class="mb-3"
                                                                   :disabled="allday"
                                                                   v-model="fromhour"
                                                                   :options="hours"
                                                                   required>
                                                    </b-form-select>
                                                </b-form-group>
                                            </div>
                                            <div class="col-sm-3">
                                                <b-form-group id="exampleInputGroup2"
                                                              label="Min"
                                                              label-for="event">
                                                    <b-form-select id="event"
                                                                   class="mb-3"
                                                                   :disabled="allday"
                                                                   v-model="fromminute"
                                                                   :options="minutes"
                                                                   required>
                                                    </b-form-select>
                                                </b-form-group>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="row">
                                            <div class="col-sm-6">
                                                <span>To Date</span>
                                                <div class="input-group date" id="datetimepicker2" data-target-input="nearest" style="margin-top: 7px">
                                                    <input type="text" id="dateto" v-model="dateto" @blur="saveTime" class="form-control datetimepicker-input" data-target="#datetimepicker2" />
                                                    <div class="input-group-append" data-target="#datetimepicker2" data-toggle="datetimepicker">
                                                        <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-sm-3">
                                                <b-form-group id="exampleInputGroup2"
                                                              label="Hrs"
                                                              label-for="event">
                                                    <b-form-select id="event"
                                                                   class="mb-3"
                                                                   :disabled="allday"
                                                                   v-model="tohour"
                                                                   :options="hours"
                                                                   required>
                                                    </b-form-select>
                                                </b-form-group>
                                            </div>
                                            <div class="col-sm-3">
                                                <b-form-group id="exampleInputGroup2"
                                                              label="Min"
                                                              label-for="event">
                                                    <b-form-select id="event"
                                                                   class="mb-3"
                                                                   :disabled="allday"
                                                                   v-model="tominute"
                                                                   :options="minutes"
                                                                   required>
                                                    </b-form-select>
                                                </b-form-group>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <span v-show="repeated">
                            <div class="row">
                                <div class="col-sm-4">
                                    <b-form-group id="exampleInputGroup2"
                                                  label="Repeat type"
                                                  label-for="event">
                                        <b-form-select id="event"
                                                       class="mb-3"
                                                       v-model="repeattype"
                                                       :options="repeatoptions"
                                                       required>
                                        </b-form-select>
                                    </b-form-group>
                                </div>
                                <div class="col-sm-4">
                                    <b-form-group id="exampleInputGroup2"
                                                  label="Numbers of Repeats"
                                                  label-for="event">
                                        <b-form-input id="event"
                                                      type="number"
                                                      v-model="numberrepeat"
                                                      required>
                                        </b-form-input>
                                    </b-form-group>
                                </div>
                                <div class="col-sm-4">
                                    <b-form-group id="exampleInputGroup2"
                                                  label="Gap Between Repeats"
                                                  label-for="event">
                                        <b-form-input id="event"
                                                      type="number"
                                                      v-model="gaprepeat"
                                                      required>
                                        </b-form-input>
                                    </b-form-group>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-4">
                                    <b-form-group id="exampleInputGroup2"
                                                  label="Repeat Mode"
                                                  label-for="event">
                                        <b-form-select id="event"
                                                       class="mb-3"
                                                       v-model="moderepeat"
                                                       :options="repeatmodeoptions"
                                                       required>
                                        </b-form-select>
                                    </b-form-group>
                                </div>

                                <div class="col-sm-8" v-if="moderepeat == 1">

                                    <b-form-group id="days"
                                                  label="Days of the Week"
                                                  label-for="event">
                                        <b-form-checkbox-group id="days " v-model="daysofweek" :options="alldaysofweek">
                                        </b-form-checkbox-group>
                                    </b-form-group>
                                </div>
                            </div>
                        </span>
                        <div class="row">
                            <div class="col-sm-12">
                                <h5>
                                    SEO Section
                                </h5>
                                <hr />
                            </div>

                            <div class="col-sm-6">
                                <b-form-group id="exampleInputGroup2"
                                              label="Title "
                                              label-for="event">
                                    <b-form-input id="event"
                                                  type="text"
                                                  v-model="titleseo"
                                                  required>
                                    </b-form-input>
                                </b-form-group>
                            </div>
                            <div class="col-sm-6">
                                <b-form-group id="exampleInputGroup2"
                                              label="Friendly URL"
                                              label-for="event">
                                    <b-form-input id="event"
                                                  type="text"
                                                  v-model="slug"
                                                  required>
                                    </b-form-input>
                                </b-form-group>
                            </div>
                        </div>
                        <div class="col-sm-12">
                            <div class="text-center">
                                <span style="color: red"><strong>{{error}}</strong></span>
                            </div>
                        </div>
                        <div v-if="loadingsave" class=" col-sm-12 text-center">
                            <pulse-loader color="#31A3DD"></pulse-loader>
                        </div>

                        <div class="col-sm-12  text-right">
                            

                            <div class="text-right">
                                <b-button class="text-right" @click="deleteEvent" variant="danger">
                                    <i class="material-icons md-48" style="vertical-align: middle;"> delete</i>
                                </b-button>
                                <button class="btn btn-primary " @click="saveEvent">Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- ********************* -->
    </div>




</template>

<script>
    import { bus } from './app.js';


    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    Vue.component('v-select', vSelect)
    var arreglo = [];
    export default {
        props: ['eventid'],
        data: function () {
            return {
                hasimage: false,
                loading: false,
                titleseo: '',
                picture: '',
                isaffiliate: false,
                slug: '',
                loadingsave: false,
                event: '',
                files: [],
                urls: [],
                allorientations: [],
                orientations: [],
                repeat: [],
                description: '',
                locationselected: null,
                active: false,
                options: [
                    { text: 'Repeating Event', value: '1' },
                    { text: 'All day', value: '2' },
                ],
                daysofweek: '',
                alldaysofweek: [
                    { text: 'Sunday', value: '1' },
                    { text: 'Monday', value: '2' },
                    { text: 'Tuesday', value: '3' },
                    { text: 'Wednesday', value: '4' },
                    { text: 'Thursday', value: '5' },
                    { text: 'Friday', value: '6' },
                    { text: 'Saturday', value: '7' },

                ],
                repeatoptions: [
                    { text: 'Daily', value: '1' },
                    { text: 'Weekly', value: '2' },
                    { text: 'Monthly', value: '3' },
                    { text: 'Yearly', value: '4' },

                ],
                repeatmodeoptions: [
                    { text: 'Days of the Week', value: '1' },
                    { text: 'Single Day', value: '2' },
                ],
                location: '',
                locationid:'',
                datefrom: '',
                fromhour: '00',
                fromminute: '00',
                dateto: '',
                tohour: '00',
                repeattype: 1,
                numberrepeat: '',
                gaprepeat: '',
                moderepeat: 1,
                places: [],
                tominute: '00',
                tempevent: '',
                error: '',
                link: '',
            }
        },
        components: {
            'PulseLoader': PulseLoader,


        },
        computed: {
            fecha: function () {
                var fecha = this.date;
                if (fecha !== undefined) {

                    fecha = fecha.split("/Date(").join("");
                    fecha = fecha.split(")/").join("");
                    var dia = new Date(parseInt(fecha)).getDate();
                    var mes = new Date(parseInt(fecha)).getMonth();
                    mes = mes + 1;
                    var año = new Date(parseInt(fecha)).getFullYear();
                    fecha = dia + "-" + mes + "-" + año;
                    return fecha;
                }
            },
            updateevent: function () {
                this.runTime();
                this.GetEventSelected();
                this.removePicture();
                var id = this.eventid;
                return "";
            },
            numberrepeatc: function () {
                if (this.numberrepeat == "") {
                    return 0;
                } else {
                    return this.numberrepeat;
                }
            },
            gaprepeatc: function () {
                if (this.gaprepeat == "") {
                    return 0;
                } else {
                    return this.gaprepeat;
                }
            },
            haslocation: function () {
                if (this.locationselected !== null) {
                    return true;
                } else {
                    return false;
                }
            },
            hours: function () {
                if (this.allday) {
                    this.fromhour = "00";
                    this.fromminute = "00";

                }
                var horas = [];
                for (var i = 0; i < 24; i++) {
                    horas.push({
                        text: i.toString().padStart(2, "0"),
                        value: i.toString().padStart(2, "0")
                    });
                }
                return horas;
            },
            minutes: function () {
                if (this.allday) {
                    this.tohour = "00";
                    this.tominute = "00";

                }
                var minutes = [];
                for (var i = 0; i < 60; i++) {
                    minutes.push({
                        text: i.toString().padStart(2, "0"),
                        value: i.toString().padStart(2, "0"),
                    });
                }
                return minutes;
            },
            repeated: function () {
                var type = "";
                Array.from(this.repeat).forEach(function (data) {
                    if (data == 1) {
                        type = data
                    }
                });

                if (type == 1) {
                    return true;
                } else {
                    return false;
                }
            },
            sunday: function () {
                var type = "";
                Array.from(this.daysofweek).forEach(function (data) {
                    if (data == 1) {
                        type = data
                    }
                });

                if (type == 1) {
                    return true;
                } else {
                    return false;
                }
            },
            monday: function () {
                var type = "";
                Array.from(this.daysofweek).forEach(function (data) {
                    if (data == 2) {
                        type = data
                    }
                });

                if (type == 2) {
                    return true;
                } else {
                    return false;
                }
            },
            tuesday: function () {
                var type = "";
                Array.from(this.daysofweek).forEach(function (data) {
                    if (data == 3) {
                        type = data
                    }
                });

                if (type == 3) {
                    return true;
                } else {
                    return false;
                }
            },
            wednesday: function () {
                var type = "";
                Array.from(this.daysofweek).forEach(function (data) {
                    if (data == 4) {
                        type = data
                    }
                });

                if (type == 4) {
                    return true;
                } else {
                    return false;
                }
            },
            thursday: function () {
                var type = "";
                Array.from(this.daysofweek).forEach(function (data) {
                    if (data == 5) {
                        type = data
                    }
                });

                if (type == 5) {
                    return true;
                } else {
                    return false;
                }
            },
            friday: function () {
                var type = "";
                Array.from(this.daysofweek).forEach(function (data) {
                    if (data == 6) {
                        type = data
                    }
                });

                if (type == 6) {
                    return true;
                } else {
                    return false;
                }
            },
            saturday: function () {
                var type = "";
                Array.from(this.daysofweek).forEach(function (data) {
                    if (data == 7) {
                        type = data
                    }
                });

                if (type == 7) {
                    return true;
                } else {
                    return false;
                }
            },
            allday: function () {
                var type = "";
                Array.from(this.repeat).forEach(function (data) {
                    if (data == 2) {
                        type = data
                    }
                });

                if (type == 2) {
                    return true;
                } else {
                    return false;
                }
            },

        },
        methods: {
            deleteEvent: function () {
                var r = this;
                $.confirm({
                    title: 'Delete Event',
                    content: 'Are you sure ?',
                    typeAnimated: true,
                    type: 'red',
                    buttons: {
                        delete: {
                            text: 'Delete',
                            btnClass: 'btn-danger',
                            action: function () {

                                axios.post('/Content/management/deleteEvent', {
                                    eventid: r.eventid,
                                }).then(response => {
                                    console.log(response.data);
                                    bus.$emit('deleteevent', {
                                        hola: "hola"
                                    });
                                });



                            }
                        },
                        Cancel: function () {
                        },
                    }
                });

            },
            test: function () {
                console.log(this.locationselected);
            },
            selectLocation: function () {
                this.location = this.locationselected.name;
                this.locationid = this.locationselected.id;
                this.$refs.modalLocations.hide()
            },
            openLocations: function () {
                this.$refs.modalLocations.show()
            },
            buildUrl: function (evento) {


                var evento = this.event.toLowerCase().split(' ').join('-');
                var url = evento;
                this.slug = "/event/"+url;

                this.titleseo = this.event;


            },
            removePicture: function (key) {
                this.hasimage = false;;

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
            changeComponent: function () {
                if (this.event !== "" || this.description !== "") {
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
                                    r.$root.currentevents = "events";
                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });
                } else {
                    this.$root.currentevents = "events";
                }

            },
            getData: function () {

                axios.get('/Content/management/GetOrientations', {

                }).then(response => {
                    this.allorientations = response.data[0][0];
                });
                axios.get('/Content/management/GetAllPlaces', {

                }).then(response => {
                    this.places = response.data[0][0];
                });
            },
            getFields: function () {
                var hour = parseInt(this.fromhour);
                var minute = parseInt(this.fromminute);
                var year = this.datefrom.substring(6, 10);
                var month = this.datefrom.substring(3, 5) - 1;
                var day = this.datefrom.substring(0, 2);

                var tohour = parseInt(this.tohour);
                var tominute = parseInt(this.tominute);
                var toyear = this.dateto.substring(6, 10);
                var tomonth = this.dateto.substring(3, 5) - 1;
                var today = this.dateto.substring(0, 2);
                console.log("year " + year);
                console.log("mes " + month);
                console.log("dia " + day);

                

                var datefrom = new Date(year, month, day, hour, minute, 0);
                var dateto = new Date(toyear, tomonth, today, tohour, tominute, 0);

                console.log("time " + datefrom);
                console.log("time " + dateto);

                var event = {
                    eventid: this.eventid,
                    evento: this.event,
                    description: this.description,
                    link: this.link,
                    locationid: this.locationid,
                    fromdate: datefrom,
                    todate: dateto,
                    orientations: this.orientations,
                    repeattype: this.repeattype,
                    numberrepeat: this.numberrepeatc,
                    gaprepeat: this.gaprepeatc,
                    repeatingevent: this.repeated,
                    allday: this.allday,
                    moderepeat: this.moderepeat,
                    sunday: this.sunday,
                    monday: this.monday,
                    tuesday: this.tuesday,
                    wednesday: this.wednesday,
                    thursday: this.thursday,
                    friday: this.friday,
                    saturday: this.saturday,
                    slug: this.slug,
                    titleseo: this.titleseo,
                    isaffiliate: this.isaffiliate,
                    active: this.active,


                }


                return event;

            },
            maxLenght: function () {
                document.getElementById("phone").maxLength = "15";
            },
            verifyFields: function () {
                if (this.event.trim() == "" || this.datefrom.trim() == "" || this.description.trim() == "" ||
                    this.orientations.length < 1 || this.datefrom.trim() == "" || this.dateto.trim() == "" || this.location == "") {
                    this.error = "Please fill all de fields";
                    return true;
                } else {
                    this.error = "";
                    return false;
                }
            },
            saveTime: function () {
                this.runTime();

                this.datefrom = $('#datefrom').val();
                this.dateto = $('#dateto').val();
            },



            cleanFields: function () {
                this.event = "";
                this.description = "";
                this.link = "";
                this.locationid = "";
                this.datefrom = "";
                this.fromhour = "00";
                this.fromminute = "00";
                this.dateto = "";
                this.tohour = "00";
                this.tominute = "00";
                this.orientations = "";
                this.repeattype = 1;
                this.numberrepeat = "";
                this.gaprepeat = "";
                this.location = "";
                this.repeat = [];
                this.moderepeat = 1;
                this.daysofweek = [];
                this.slug = "";
                this.titleseo = "";
                this.removePicture();
                this.orientations = [];

            },
            saveEvent: function () {
                var error = this.verifyFields();
                if (!error) {
                    var evento = this.getFields();
                    var data = JSON.stringify(evento);

                    var r = this;
                    var text = "Do you want to add " + this.event + "?"
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

                    $.confirm({
                        title: 'Update Event ',
                        content: text,
                        typeAnimated: true,
                        type: 'blue',
                        buttons: {
                            delete: {
                                text: 'Continue',
                                btnClass: 'btn-blue',
                                action: function () {
                                   
                                    r.typerequest = "Please wait a moment...";

                                    axios.post('/Content/management/UpdateEvent', {
                                        data: data,
                                    }).then(response => {

                                        files.append('id', r.eventid);
                                        axios.post('/Content/management/UploadImageEvent', r.formdata, config).then(
                                            response => {
                                                if (response.data !== "not") {
                                                    r.picture = response.data;
                                                    r.removePicture();
                                                }
                                               
                                            }
                                        );
                                        bus.$emit('updateevent', {
                                            response: 'update',
                                            event: r.event,
                                            place: r.location,
                                            zone: r.locationselected.zone,
                                            orientations: r.orientations,
                                        });
                                        $.alert('Event Updated succefully');
                                    });
                                    
                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });
                                        
                }
            },
            GetEventSelected: function () {
                this.loading = true;


                axios.post('/Content/management/GetEventSelected', {
                    id: this.eventid,
                }).then(response => {
                    this.event = response.data[0][0].evento;
                    this.description = response.data[0][0].description;
                    this.link = response.data[0][0].link;
                    this.fromdate = response.data[0][0].datefrom;
                    this.location = response.data[0][0].place;
                    if (this.location == undefined) {
                        this.locationid = null;
                    } else {
                        this.locationid = response.data[0][0].placeid;
                    }
                    console.log(this.locationid);
                   
                    if (response.data[0][0].repeattype == null) {
                        this.repeattype = 1;
                    } else {
                        this.repeattype = response.data[0][0].repeattype;
                    }
                    if (response.data[0][0].moderepeat == null) {
                        this.moderepeat = 1;
                    } else {
                        this.moderepeat = response.data[0][0].moderepeat;
                    }
                    this.numberrepeat = response.data[0][0].numberrepeat;
                    this.gaprepeat = response.data[0][0].gaprepeat;
                    if (response.data[0][0].picture == null) {
                        this.picture = "";

                    } else {
                        this.picture = response.data[0][0].picture;
                    }
             

                    this.locationselected = {
                        zone: response.data[0][0].zone,
                        destination: response.data[0][0].destination,
                        address: response.data[0][0].address,
                        label: response.data[0][0].place,
                    };
                    var days = [];
                    var repeat = [];
                    if (response.data[0][0].sunday == true) {
                        days.push(1);
                    }
                    if (response.data[0][0].monday == true) {
                        days.push(2);
                    }
                    if (response.data[0][0].tuesday == true) {
                        days.push(3);
                    }
                    if (response.data[0][0].wednesday == true) {
                        days.push(4);
                    }
                    if (response.data[0][0].thursday == true) {
                        days.push(5);
                    }
                    if (response.data[0][0].friday == true) {
                        days.push(6);
                    }
                    if (response.data[0][0].saturday == true) {
                        days.push(7);
                    }
                    if (response.data[0][0].repetingevent == true) {
                        repeat.push(1);
                    }
                    if (response.data[0][0].allday == true) {
                        repeat.push(2);
                    }
                    this.active = response.data[0][0].active;
                    this.slug = response.data[0][0].slug;
                    this.titleseo = response.data[0][0].titleseo;
                    this.repeat = repeat;
                    this.daysofweek = days;
                    var orientations = [];
                    Array.from(response.data[1][0]).forEach(function (data) {
                        orientations.push(data.orientationid);
                    });
                    this.orientations = orientations;
                    /* Darle formato al DateFrom, HourFrom y MinuteFrom */
                    var fecha = response.data[0][0].datefrom;
                    fecha = fecha.split("/Date(").join("");
                    fecha = fecha.split(")/").join("");
                    var fromhour = new Date(parseInt(fecha));
                    this.fromhour = fromhour.getHours().toString().padStart(2, "0");
                    this.fromminute = fromhour.getMinutes().toString().padStart(2, "0");
                    var dia = new Date(parseInt(fecha)).getDate();
                    dia = dia.toString().padStart(2, "0");
                    var mes = new Date(parseInt(fecha)).getMonth();
                    mes = mes + 1;
                    mes = mes.toString().padStart(2, "0");
                    var año = new Date(parseInt(fecha)).getFullYear();
                    fecha = dia + "-" + mes + "-" + año;
                    this.datefrom = fecha;
              
                      /* Darle formato al DateTo HourTo y MinuteTo*/
                    var fecha = response.data[0][0].dateto;
                    fecha = fecha.split("/Date(").join("");
                    fecha = fecha.split(")/").join("");
                    var tohour = new Date(parseInt(fecha));
                    this.tohour = tohour.getHours().toString().padStart(2, "0");
                    this.tominute = tohour.getMinutes().toString().padStart(2, "0");
                    var dia = new Date(parseInt(fecha)).getDate();
                    dia = dia.toString().padStart(2, "0");
                    var mes = new Date(parseInt(fecha)).getMonth();
                    mes = mes + 1;
                    mes = mes.toString().padStart(2, "0");
                    var año = new Date(parseInt(fecha)).getFullYear();
                    fecha = dia + "-" + mes + "-" + año;
                    this.dateto = fecha;
                    var h = new Date(parseInt(fecha)).getMinutes();

                    this.isaffiliate = response.data[0][0].isaffiliate;
                    this.loading = false;
                    this.runTime();

                });
            },
            deleteImage: function () {
                var id = this.eventid;
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
                              

                                axios.post('/Content/management/DeleteImageEvent', {
                                    id: id,
                                    
                                }).then(response => {

                                    if (response.data == "ok") {
                                        r.picture = "";
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
            runTime: function () {
                $(function () {
                    $('#datetimepicker').datetimepicker({
                        format: 'DD-MM-YYYY'
                    });
                });
                $(function () {
                    $('#datetimepicker2').datetimepicker({
                        format: 'DD-MM-YYYY'
                    });
                });
            },
        },

        mounted() {
            this.runTime();
            this.getData();
            //  this.runTime();

        }
    }

</script>

