<template>
    <div>
        <div class="row">
            <div class="col-sm-6 text-left ">
                <h2>Poll Management</h2>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-sm-6 col-md-4 col-lg-2">
                                <span>Folio</span>
                                <input class="form-control" type="text" v-model="folio">
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
                            <div class="col-sm-6 col-md-4 col-lg-2 ">
                                <span><strong>Location</strong></span>
                                <v-select v-model="location" :options="locations"></v-select>
                            </div>
                            <div class="col-sm-6 col-md-6 col-lg-3">
                                <button class="btn btn-primary mt-4" @click="searchByDate">Get</button>
                            </div>
                            <div class="col-sm-6 col-md-6 col-lg-3 text-left" v-if="">

                                <button class="btn btn-primary mt-4 mb-3" @click="getSurveysGroup"
                                        v-if="$root.Shared.Session.RoleID !== 'c96146b2-e61e-4296-a8bb-9ed16d27c66a' &&
                                        $root.Shared.Session.RoleID !== 'fe357275-29fe-422a-9b39-5ce5ca42bbea'">
                                    New Survey
                                </button>
                            </div>
                            <div v-if="loadingSearch" class=" col-sm-12 m-3 text-center">
                                <pulse-loader color="#31A3DD"></pulse-loader>
                            </div>
                            <!--Tabla de Resultados-->
                            <div class="col-sm-12" v-if="showtable">
                                <span style="font-size: 20px;font-weight: 500;" v-if="surveysByDate.length > 0">Result: {{surveysByDate.length}} <span v-if="surveysByDate.length > 1">Polls.</span> <span v-else>Poll.</span></span>
                                <b-table v-scroll-to="'#detailsToEdit'" @row-clicked="showDetailsToEdit" class="hand" :per-page="tableResults.perPage" striped hover
                                         :items="surveysByDate" :fields="tableResults.fields" :current-page="tableResults.currentPage">
                                    <template slot="location" slot-scope="row">
                                        {{nameLocation(row.item.locationID)}}
                                    </template>
                                    <template slot="dateSaved" slot-scope="row">
                                        {{getDate(row.item.dateSaved)}}
                                    </template>
                                    <template slot="contactInfo" slot-scope="row">
                                        <div class="row">
                                            <div class="col-sm-12 text-center" v-if="row.item.contactInfo == true">
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
                                    <template slot="delete" slot-scope="row">
                                        <div class="row" @click="deletePoll(row.item)">
                                            <div class="col-sm-12 text-center">
                                                <i class="material-icons" style="color:red">
                                                    delete
                                                </i>
                                            </div>
                                        </div>

                                    </template>
                                </b-table>
                                <b-pagination size="md" :total-rows="surveysByDate.length" v-model="tableResults.currentPage" :per-page="tableResults.perPage">
                                </b-pagination>
                            </div>
                            <!--Tabla seleccionar poll para agregar nueva-->
                            <div class="col-sm-12" v-if="surveys.length > 0 && newSurvey">
                                <b-table @row-clicked="showDetails" class="hand" :per-page="table.perPage" striped hover
                                         :items="surveys" :fields="table.fields" :current-page="table.currentPage">

                                    <template slot="date" slot-scope="row">
                                        {{getDate(row.item.date)}}
                                    </template>
                                </b-table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--Templates de polls-->
        <div id="detailsToEdit">
            <div class="row" v-if="surveyToEdit.show">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <addsurvey :key="surveyToEditKey" :completeAnswer="surveyToEdit.answer" :answer="surveyToEdit.answer.answer" :fieldGroupID="surveyToEdit.answer.fieldgroupid" :surveyname="surveyToEdit.answer.surveyname" :description="surveyToEdit.answer.description"></addsurvey>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row" v-if="showSurveyToAdd">
            <div class="col-sm-12">
                <div class="card">
                    <div class="card-body">
                        <addsurvey :key="surveyToAddKey" :fieldGroupID="survey.fieldGroupID" :surveyname="survey.name" :description="survey.description"></addsurvey>
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
    import addsurvey from './addsurvey.vue';
    import addsurveys from './addsurveys.vue';


    export default {
        props: ['cards'],
        data: function () {
            return {
                locations: [],
                location: {
                    label: 'All Locations',
                    value: 0,
                },
                surveyToAddKey: 0,
                surveyToEditKey:0,
                showSurveyToAdd: false,
                folio: null,
                loadingSearch: false,
                surveysByDate: [],
                showtable: false,
                newSurvey: false,
                details: false,
                survey: {
                    fieldGroupID: '',
                    name: '',
                },
                surveyToEdit: {
                    key: null,
                    getFolio: [],
                    ambassadorName: '',
                    ambassadorid: '',
                    template: [],
                    answer: [],
                    folio: 0,
                    contactInfo: false,
                    culture: "",
                    dateSaved: '',
                    show: false,
                },
                table: {
                    tempkey: [],
                    perPage: 5,
                    currentPage: 1,
                    countrows: 2,
                    fields: [

                        {
                            key: 'name',
                            label: 'Polls Name',
                            sortable: true,
                        },


                        {
                            key: 'username',
                            label: 'Created by',
                            sortable: true,
                        },

                    ],
                },
                tableResults: {
                    tempkey: [],
                    perPage: 10,
                    currentPage: 1,
                    countrows: 2,
                    fields: [
                        {
                            key: 'folio',
                            label: 'Folio',
                            sortable: true,
                        },
                        {
                            key: 'name',
                            label: 'Poll´s Name',
                            sortable: true,
                        },
                        {
                            key: 'savedBy',
                            label: 'Ambassador',
                            sortable: true,
                        },
                        {
                            key: 'location',
                            label: 'Location',
                            sortable: true,
                        },
                        {
                            key: 'culture',
                            label: 'Culture',
                            sortable: true,
                        },
                        {
                            key: 'contactInfo',
                            label: 'Contact Info',
                            sortable: true,
                        },

                        {
                            key: 'dateSaved',
                            label: 'Date Saved',
                            sortable: true,
                        },
                        {
                            key: 'delete',
                            label: 'Delete Poll',

                        },
                    ],
                },
                surveys: [],
                par: 0,
                parEdit: 0,
                loading: false,
            }
        },
        components: {
            'PulseLoader': PulseLoader,
            'addsurvey': addsurvey,
            'addsurveys': addsurveys

        },
        methods: {
            nameLocation(locationID) {
                var location = "";
                Array.from(this.locations).forEach(function (data) {
                    if (data.value == locationID) {
                        location = data.label;
                    }
                    
                });
                return location;
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
            clenFields: function () {
                this.surveysByDate = [];

            },
            getLocations: function () {
                axios.get('/Content/management/GetLocations', {

                }).then(response => {
                    var arr = [];
                    this.locations = response.data;
                    this.locations.unshift({
                        label: 'All Locations',
                        value: 0,
                    });
                }).catch(function () {
                });;
            },
            getInfo: function () {
                this.parEdit = 0;
                const r = this;
                this.loadingSearch = true;
                this.clenFields();
                var from = this.calendarInput($('#calendario3').val());
                var to = this.calendarInput($('#calendarioto4').val(), true);
                if (this.folio !== null) {
                    if (this.folio.trim() == "") {
                        this.folio = null;
                    }
                }
                var location = null;
                if (this.location !== null) {
                    location = this.location.value;
                }
                axios.post('/Content/management/GetSurveysSaved', {
                    fromDate: from,
                    toDate: to,
                    folio: this.folio,
                    locationID: location,

                }).then(response => {
                    var total = 0;
                    this.surveysByDate = response.data;
                    console.log("surveys date");        
                    console.log(response.data);        

                    this.loadingSearch = false;
                    this.showtable = true;
                    this.surveyToEdit.show = false;

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
                    r.surveyToEdit.show = false;
                });
                this.par = 0;
                this.newSurvey = false;
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
            parImpar: function (type) {
                var tipo = (this.par % 2) ? "impar" : "par";
                if (tipo == type) {
                    return true;
                } else {
                    return false;
                }
            },
            parImparEdit: function (type) {
                var tipo = (this.parEdit % 2) ? "impar" : "par";
                if (tipo == type) {
                    return true;
                } else {
                    return false;
                }
            },
            showDetailsToEdit: function (key) {             
                this.showSurveyToAdd = false;
                if (this.surveyToEdit.key !== null) {
                    this.surveyToEdit.key._rowVariant = "";
                }
                var membershipID = 0;
                if (key.membershipID !== null) membershipID = parseInt(key.membershipID);
                axios.post('/Content/management/GetSurveyToEdit', {
                    fieldGroupID: key.fieldgroupid,
                    answerID: key.id,
                    membershipID: membershipID
                }).then(response => {
                    this.surveyToEdit.answer = response.data[1][0];
                    this.surveyToEdit.template = response.data[0][0];
                    this.surveyToEdit.show = true;
                    this.surveyToEdit.key = key;                  
                    key._rowVariant = "primary";
                    this.surveyToEditKey = this.surveyToEditKey + 1;
                }).catch(function () {
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
                });

            },
            showDetails: function (key) {

                if (this.table.tempkey !== null) {
                    this.table.tempkey._rowVariant = "";
                }
                key._rowVariant = "primary";
                this.table.tempkey = key;
                this.survey.fieldGroupID = key.id;
                this.survey.name = key.name;
                this.survey.description = key.description;
                this.par = this.par + 1;
                this.parEdit = 0;
                this.showSurveyToAdd = true;
                this.surveyToAddKey = this.surveyToAddKey + 1;

                this.$forceUpdate();
            },
            searchByDate: function () {
                this.showSurveyToAdd = false;
                this.getInfo();
            },
            getDate: function (date) {
                var getdate = date;

                if (date.length == 10) return date;

                getdate = getdate.split("/Date(").join("");
                getdate = getdate.split(")/").join("");
                var newdate = new Date(parseInt(getdate));
                var month = newdate.getMonth() + 1;
                return newdate.getDate().toString().padStart(2, "0") + "-" + month.toString().padStart(2, "0") + "- " + newdate.getFullYear();
            },
            changeComponent: function () {
                this.$root.currentpolls = "addsurvey";
            },
            getSurveysGroup: function () {
                this.loading = true;
                this.parEdit = 0;
                axios.get('/Content/management/getSurveysGroup', {

                }).then(response => {
                    this.surveys = response.data;
                    this.newSurvey = true;
                    this.showtable = false;
                    this.surveyToEdit.show = false;

                }).catch(function () {
                });;
            },
            deletePoll: function (id) {
                const r = this;
                $.confirm({
                    title: 'Delete Poll folio - ' + id.folio,
                    content: 'Do you want to continue?',
                    typeAnimated: true,
                    type: 'red',
                    buttons: {
                        delete: {
                            text: 'Delete',
                            btnClass: 'btn-blue',
                            action: function () {
                                axios.post('/Content/management/deletePoll', {
                                    id: id.id
                                }).then(response => {
                                    $.alert('Poll deleted succefully');
                                    r.surveysByDate = [];
                                    r.getInfo();

                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
            },

        },
        computed: {


        },

        mounted() {
            this.dateFormat();
            this.getLocations();

            bus.$on('close', obj => {
                this.details = false;
            });
           
            bus.$on('updateCardCode', obj => {

                this.surveyToEdit.key.membershipID = obj.cardCode;
            });
            bus.$on('updateTable', obj => {
                this.surveyToEdit.key.contactInfo = obj.contactInfo;
                this.surveyToEdit.key.dateSaved = obj.fecha;
                this.surveyToEdit.key.savedBy = obj.ambassador;
                this.surveyToEdit.getFolio = JSON.parse(obj.folio);
                var folio = this.surveyToEdit.getFolio.filter((i, index) => (i.fieldid == 1198 || i.fieldid == 1200));
                this.surveyToEdit.key.folio = folio[0].answer;


            });

        },
    }

</script>
<style>
    .hand {
        cursor: pointer;
    }
</style>
<!--<div class="row" v-if="parImpar('impar') && par !== 0">
          <div class="col-sm-12">
              <div class="card">
                  <div class="card-body">
                      <addsurveys :fieldGroupID="survey.fieldGroupID" :surveyname="survey.name" :description="survey.description"></addsurveys>
                  </div>
              </div>
          </div>
        </div>
        -->
