<template>
    <div>
        <div class="row">
            <div class="col-sm-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row ">
                            <div class="col-sm-9 centerdiv " style="color: #696969 !important">
                                <h2> {{survey.name}}</h2>

                                <span class="bolds" style="font-size:13.3px">{{survey.description}}</span>
                                <div class="row">
                                    <div class="col-sm-6 m-3">
                                        <span><strong>Select an ambassador</strong></span>
                                        <v-select v-model="ambassador" :options="ambassadorsFilter"></v-select>
                                        <span style="color: red"><strong>{{errorAmbassador}}</strong></span>
                                        <b-form-checkbox id="checkbox-1"
                                                         v-model="contactInfo"
                                                         name="checkbox-1"
                                                         class="mt-2"
                                                         disabled>
                                            Contact Info 
                                        </b-form-checkbox>
                                    </div>
                                </div>
                                <!--primer grupo de preguntas sin parent id-->
                                <div class="m-3" v-for="template in orderedSurvey" v-if="template.fieldtypeid == 1" style="font-size: 10pt">
                                    <span v-if="template.subfieldid == 1 && checkVisible(template.visibleif)">
                                        <span class="bold">{{template.description}} </span>

                                    </span>

                                    <div class="row" v-if="template.subfieldid == 2 && checkVisible(template.visibleif)">
                                        <div class="col-sm-6">
                                            <span>{{parent.description}}</span>
                                        </div>
                                        <div class="col-sm-6 text-left">
                                            <ratingQuestion :fieldid="template.fieldid" :parentfieldid="template.parentfieldid" :subfieldid="template.subfieldid"></ratingQuestion>
                                        </div>
                                    </div>
                                    <div class="row" v-if="template.subfieldid == 3 && checkVisible(template.visibleif)">
                                        <div class="col-sm-12">
                                            <span>{{template.description}} </span>
                                        </div>
                                        <div class="col-sm-12 text-center">
                                            <yesOrNot :guidsurvey="template.guidfield" :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></yesOrNot>
                                        </div>
                                    </div>
                                    <div class="row" v-if="template.subfieldid == 5 && checkVisible(template.visibleif)">
                                        <div class="col-sm-6 bold">{{template.description}} </div>
                                        <div class="col-sm-6">
                                            <optionsQuestion :fieldid="template.fieldid" :parentfieldid="template.parentfieldid" :subfieldid="template.subfieldid" :options="template.fieldoptions"></optionsQuestion>
                                        </div>
                                    </div>
                                    <div class="row" v-if="template.subfieldid == 6 && checkVisible(template.visibleif)">
                                        <div class="col-sm-12">
                                            <span class="bold">{{template.description}} </span>
                                        </div>
                                        <div class="col-sm-6">
                                            <textField :fieldid="template.fieldid" :parentfieldid="template.parentfieldid" :subfieldid="template.subfieldid"></textField>
                                        </div>
                                    </div>
                                    <div class="row" v-if="template.subfieldid == 7 && checkVisible(template.visibleif)">
                                        <div class="col-sm-12">
                                            <span>{{template.description}} </span>
                                        </div>
                                        <div class="col-sm-12">
                                            <comments :fieldid="template.fieldid" :parentfieldid="template.parentfieldid" :subfieldid="template.subfieldid"></comments>
                                        </div>
                                    </div>




                                    <!--se generan las preguntas que tienen parent id-->
                                    <div class="m-3" v-for="parent in surveytypes.parent" v-if="parent.parentfieldid == template.fieldid">
                                        <div class="row" v-if="parent.subfieldid == 1 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-6">
                                                <span>{{parent.description}} </span>
                                            </div>

                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 2 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-6">
                                                <span>{{parent.description}}</span>
                                            </div>
                                            <div class="col-sm-6 text-left">

                                                <ratingQuestion :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></ratingQuestion>
                                            </div>
                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 3 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-12">
                                                <span>{{parent.description}} </span>
                                            </div>
                                            <div class="col-sm-12 text-center">
                                                <yesOrNot :guidsurvey="parent.guidfield" :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></yesOrNot>
                                            </div>
                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 5 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-6 bold">{{parent.description}}</div>
                                            <div class="col-sm-6">
                                                <optionsQuestion :options="parent.fieldoptions" :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></optionsQuestion>
                                            </div>
                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 6 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-12">
                                                <span class="bold">{{parent.description}}</span>
                                            </div>
                                            <div class="col-sm-6">
                                                <textField :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></textField>
                                            </div>
                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 7 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-12">
                                                <span>{{parent.description}}</span>
                                            </div>
                                            <div class="col-sm-12">
                                                <comments :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></comments>
                                            </div>
                                        </div>



                                        <!--3er grupo de preguntas con parent id-->
                                        <div class="m-3" v-for="parent2 in surveytypes.parent" v-if="parent2.parentfieldid == parent.fieldid">
                                            <div class="row" v-if="parent2.subfieldid == 1  && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-6">
                                                    <span>{{parent2.description}} </span>
                                                </div>

                                            </div>
                                            <div class="row" v-if="parent2.subfieldid == 2  && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-6">
                                                    <span>{{parent2.description}}</span>
                                                </div>
                                                <div class="col-sm-6 text-left">
                                                    <ratingQuestion :fieldid="parent2.fieldid && checkVisible(parent2.visibleif, parent2.guidsurvey)" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></ratingQuestion>
                                                </div>
                                            </div>
                                            <div class="row" v-if="parent2.subfieldid == 3  && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-12">
                                                    <span>{{parent2.description}} </span>
                                                </div>
                                                <div class="col-sm-12 text-center">
                                                    <yesOrNot :guidsurvey="parent2.guidfield" :fieldid="parent2.fieldid" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></yesOrNot>
                                                </div>
                                            </div>
                                            <div class="row" v-if="parent2.subfieldid == 5  && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-6 bold">{{parent2.description}}</div>
                                                <div class="col-sm-6">
                                                    <optionsQuestion :options="parent2.fieldoptions" :fieldid="parent2.fieldid" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></optionsQuestion>
                                                </div>
                                            </div>
                                            <div class="row" v-if="parent2.subfieldid == 6 && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-12">
                                                    <span class="bold">{{parent2.description}}</span>
                                                </div>
                                                <div class="col-sm-6">
                                                    <textField :fieldid="parent2.fieldid" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></textField>
                                                </div>
                                            </div>

                                            <div class="row" v-if="parent2.subfieldid == 7 && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-12">
                                                    <span>{{parent2.description}}</span>
                                                </div>
                                                <div class="col-sm-12">
                                                    <comments :fieldid="parent2.fieldid" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></comments>
                                                </div>
                                            </div>


                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12 mt-3 text-center">
                                        <br />
                                        <span style="color: red"><strong>{{errorAmbassador}}</strong></span><br />
                                        <br />
                                        <button class="submit" @click="saveData">SUBMIT MY ANSWERS</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        {{refreshSurvey}}
        {{surveyFinished}}
        {{refreshContactInfo}}
    </div>
</template>
<script>
    import { bus } from './app.js';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    import ratingQuestion from './types/ratingQuestion.vue';
    import comments from './types/comments.vue';
    import textField from './types/textField.vue';
    import optionsQuestion from './types/optionsQuestion.vue';
    import yesOrNot from './types/yesOrNot.vue';



    export default {
        props: ['fieldGroupID','surveyname', 'description'],
            data: function () {
                return {
                    saveContact: false,
                    contactInfo: false,
                    hasData: {
                        phone: false,
                        cell: false,
                        email: false
                    },
                    getError: false,
                    errorAmbassador:'',
                    ambassadors: [],
                    ambassador:'',
                    surveytemplate: [],
                    surveytypes: {
                        parent: [],
                        noparent: [],
                    },
                    survey: {
                        name: '',
                        description: '',
                    },
                    count: 0,
                    surveyInJson: [],
                    visibleGuids: [],
                    test: 0,
                    
                }
        },
        components: {
            'PulseLoader': PulseLoader,
            'ratingQuestion': ratingQuestion,
            'textField': textField,
            'comments': comments,
            'optionsQuestion': optionsQuestion,
            'yesOrNot': yesOrNot
        },
        computed: {
            cCulture: function () {
                if (this.fieldGroupID == 77) {
                    return "en-US";
                } else if (this.fieldGroupID == 78) {
                    return "es-MX";
                } else {
                    return null;
                }
            },
            refreshContactInfo: function () {
                this.contactInfo = this.cHasData;
                return "";
            },
            cHasData: function () {

                if (this.hasData.phone || this.hasData.email || this.hasData.cell) {
                    return true;
                } else {
                    return false;
                }

            },
            ambassadorsFilter: function () {
                function eliminarObjetosDuplicados(arr, prop) {
                    var nuevoArray = [];
                    var lookup = {};

                    for (var i in arr) {
                        lookup[arr[i][prop]] = arr[i];
                    }

                    for (i in lookup) {
                        nuevoArray.push(lookup[i]);
                    }

                    return nuevoArray;
                }

                var duplicadosEliminados = eliminarObjetosDuplicados(this.ambassadors, 'value');
                return duplicadosEliminados;

            },
            surveyFinished: function () {
                const r = this;
              
                
                console.log();
               
                if (this.count == this.surveyInJson.length && this.surveyInJson.length !== 0) {
                    
                    
                    console.log("contact " + this.saveContact);
                   
                    var survey = JSON.stringify(this.surveyInJson);
                    console.log(this.surveyInJson);
                    axios.post('/Content/management/saveSurvey', {
                        fieldGroupID: this.fieldGroupID,
                        data: survey,
                        ambassadorID: this.ambassador.value,
                        contactInfo: this.saveContact,
                        culture: this.cCulture,
                    }).then(response => {
                        if (response.data == "ok") {
                            $.alert('Survey submited succefully');
                            this.surveyInJson = [];
                            console.log("survey completed");
                            bus.$emit('closedetails', {                             
                            });                           
                       }                       
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
                            r.surveyInJson = [];
                           
                        });                                       
                }
                return "";
            },
            orderedSurvey: function () {
                var array2 = $.map(this.surveytypes.noparent, function (value, index) {
                    return [value];
                });
                return array2.slice(0).sort((a, b) => a.order - b.order);
            },
            _fieldGroupID: function () {
                this.survey.name = this.surveyname;
                this.survey.description = this.description;
                return this.fieldGroupID;               
            },
            refreshSurvey: function () {

                var id = this.fieldGroupID;
                this.count = 0;
                this.getTemplateSurvey();                
                return "";
            },
        },
        methods: {
            testing: function () {
                bus.$emit('flag', {
                });
            },
            checkVisible: function (guid, e) {              
                if (guid == null) {
                    return true;
                } else {                
                    var flag = false;
                    Array.from(this.visibleGuids).forEach(function (data) {
                        
                        if (data == guid) {
                           
                            flag = true;
                        } else {                                                 
                        }
                    });              
                    return flag;
                }
            },
            changeComponent: function () {
                this.$root.currentpolls = "surveys";
            },
            getTemplateSurvey: function () {
                console.log("get template");
                axios.post('/Content/management/getTemplateSurvey', {
                    fieldGroupID: this._fieldGroupID,
                }).then(response => {
                    this.surveytemplate = [];
                    this.surveytemplate = response.data;
                    var alone = [];
                    var parent = []
                    Array.from(response.data).forEach(function (data) {
                        if (data.parentfieldid == null) {
                            alone.push(data);
                        } else {
                            parent.push(data);
                        }
                    });
                    this.surveytypes.parent = parent;
                    this.surveytypes.noparent = alone;
                    bus.$emit('flagg', {
                       
                    });
                    this.$forceUpdate();
                }).catch(function () {
                });;
            },
            saveData: function () {   
                if (this.ambassador == '' || this.ambassador == null) {
                    this.errorAmbassador = "Please select an ambassador";
                    return false;
                } else {
                    this.errorAmbassador = "";
                }
                console.log(this.ambassador.value);
                const r = this;
                this.saveContact = this.contactInfo;
                $.confirm({
                    title: 'Submit Survey',
                    content: 'Do you want to continue?',
                    typeAnimated: true,
                    type: 'blue',
                    buttons: {
                        delete: {
                            text: 'Continue',
                            btnClass: 'btn-blue',
                            action: function () {
                                bus.$emit('save', {

                                });
                                bus.$emit('close', {

                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
            },    
            getAmbassadors: function () {
                axios.get('/Content/management/GetAmbassadors', {

                }).then(response => {
                    this.ambassadors = response.data;
                }).catch(function () {
                });;
            },
        },

        mounted() {
            bus.$on('contactInfoPhone', obj => {
                this.hasData.phone = obj.contactInfo;
            });
            bus.$on('contactInfoCell', obj => {
                this.hasData.cell = obj.contactInfo;
            });
            bus.$on('contactInfoEmail', obj => {
                this.hasData.email = obj.contactInfo;
                


            });
            this.getAmbassadors();
            this.surveyInJson = [];
           
                bus.$on('savedata', obj => {
                    console.log(obj.parentfieldid);
                    var pfid = null;
                    if (obj.parentfieldid !== undefined) {
                        pfid = obj.parentfieldid;
                    }
                    this.surveyInJson.push({
                        subfieldid: obj.subfieldid,
                        answer: obj.answer,
                        parentfieldid: pfid,
                        fieldid: obj.fieldid,
                        fieldgroupid: this.fieldGroupID,
                    })
                });
           
            
            bus.$on('componentcount', obj => {
                this.count = this.count + 1;
            });
            bus.$on('saveGuid', obj => {            
                this.visibleGuids.push(obj.guid);
            });

            bus.$on('deleteGuid', obj => {
                var r = this;
                Array.from(this.visibleGuids).forEach(function (data, index) {
                    console.log(data + " "  + index);
                    if (obj.guid == data) {
                        r.visibleGuids.splice(index, 1);
                    } 
                });
                
            });
            
           

            },
        }

</script>
<style>
    .hand {
        cursor: pointer;
    }
    .centerdiv {
        position: relative;
        margin: auto;
    }
    .surveyname{
        border-left: white;
        border-top: white;
        border-right: white;
        font-size: 40px;
    }
    .bordergreen {
        border-color: #00AAA5;
    }
    .answer-button {
        border: #00AAA5 solid 1px;
        width: 40px;
        display: inline-block;
        text-align: center;
        margin: 0 5px 0;
        border-radius: 21px;
        height: 40px;
        vertical-align: middle;
        line-height: 39px;
        cursor: pointer;
    }
    .bolds {
        font-weight: bold;
    }
    .submit {
        background-color: #00AAA5;
        border: #00AAA5 solid 1px;
        -moz-border-radius: 3px;
        -webkit-border-radius: 3px;
        border-radius: 3px;
        color: white;
        font-size: 1.02em;
        padding: 7px 20px;
        text-transform: uppercase;
        -webkit-appearance: none;
    }

    
</style>
