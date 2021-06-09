<template>
    <div>
        <div class="row">
            <div class="col-sm-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row ">
                            <div class="col-sm-12">

                            </div>
                            <div class="col-sm-9 centerdiv " style="color: #696969 !important">
                                <h2> {{survey.name}}</h2>
                                <span class="bolds" style="font-size:13.3px">{{survey.description}}</span>
                                <div class="row mt-2">
                                    <div class="col-sm-6 ">
                                        <span><strong>Select an ambassador</strong></span>
                                        <v-select v-model="ambassador" :options="ambassadorsFilter"></v-select>
                                        <span style="color: red"><strong>{{errorAmbassador}}</strong></span>
                                    </div>
                                    <div class="col-sm-6 ">
                                        <span>Date Submited </span>
                                        <div class="form-group">
                                            <div class="input-group date" id="datetimepicker5" data-target-input="nearest">
                                                <input type="text" id="calendario5" class="form-control datetimepicker-input" data-target="#datetimepicker5" />
                                                <div class="input-group-append" data-target="#datetimepicker5" data-toggle="datetimepicker">
                                                    <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 ">
                                        <span><strong>Location</strong></span>
                                        <v-select v-model="locationPoll" :options="locations"></v-select>
                                        <span style="color: red"><strong>{{errorLocation}}</strong></span>
                                    </div>
                                    <div class="col-sm-6 ">
                                        <span>Departure Date </span>
                                        <div class="form-group">
                                            <div class="input-group date" id="datetimepicker10" data-target-input="nearest">
                                                <input type="text" id="calendario10" class="form-control datetimepicker-input" data-target="#datetimepicker10" />
                                                <div class="input-group-append" data-target="#datetimepicker10" data-toggle="datetimepicker">
                                                    <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-12">
                                        <b-form-checkbox id="checkbox-1"
                                                         v-model="contactInfo"
                                                         name="checkbox-1"
                                                         class=""
                                                         disabled>
                                            Contact Info
                                        </b-form-checkbox>
                                    </div>
                                    <div class="col-sm-12 mt-3">
                                        <b-form-group :disabled="membershipSent" label="Privilege Card">
                                            <b-form-radio-group v-model="hasMembership"
                                                                :options="optionsMembership"></b-form-radio-group>
                                        </b-form-group>
                                    </div>
                                    <div class="col-sm-12">
                                        <div class="row p-2 pb-3" v-if="hasMembership" style="background-color:#E1F7FF; border-radius: 5px">
                                            <div class="col-sm-12 text-center">
                                                <span style="font-weight: 600">Activate Membership</span>
                                            </div>
                                            <div class="col-sm-12 text-center">
                                                <b-form-group :disabled="membershipSent" label="Mode">
                                                    <b-form-radio-group v-model="modeOfMembership"
                                                                        :options="optionsMode"></b-form-radio-group>
                                                </b-form-group>
                                            </div>

                                            <div class="col-sm-6 " v-if="modeOfMembership == 1">
                                                <span v-if="loadingcodes"><clip-loader color="#31A3DD"></clip-loader></span>
                                                <span style="font-weight: 600">Card Code</span>
                                                <div @click="getInactivesCodes">
                                                    <v-select :disabled="membershipSent" v-model="code" :options="inactivescodes"></v-select>
                                                    <span style="color: red">{{errorMembership.code}}</span>
                                                </div>
                                            </div>
                                            <div class="col-sm-6 " v-if="modeOfMembership == 1">
                                                <span>Membership's Ambassador</span>
                                                <input class="form-control" type="text" disabled :value="membershipAmbassador">

                                            </div>
                                            <div class="col-sm-6 " v-if="modeOfMembership == 1">
                                                <span>Location</span>
                                                <v-select :disabled="membershipSent" v-model="location" :options="locations"></v-select>
                                                <span style="color: red">{{errorMembership.location}}</span>

                                            </div>
                                            <div class="col-sm-6" v-if="modeOfMembership == 2">
                                                <span v-if="loadingcodesactives"><clip-loader color="#31A3DD"></clip-loader></span>
                                                <span style="font-weight: 600">Card Code</span>
                                                <div @click="getActivesCodes">
                                                    <v-select :disabled="membershipSent" v-model="activeCode" :options="activesCodes"></v-select>
                                                    <span style="color: red">{{errorMembership.codeActive}}</span>
                                                </div>
                                            </div>
                                            <div class="col-sm-6" v-if="modeOfMembership == 2">
                                                <span>Membership's Ambassador</span>
                                                <input class="form-control" type="text" disabled :value="membershipAmbassadorActive">

                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!--primer grupo de preguntas sin parent id-->
                                <div class="mt-2" v-for="template in orderedSurvey" v-if="template.fieldtypeid == 1" style="font-size: 10pt">
                                    <span v-if="template.subfieldid == 1 && checkVisible(template.visibleif)">
                                        <span class="bold">{{template.description}} </span>

                                    </span>
                                    <div class="row" v-if="template.subfieldid == 2 && checkVisible(template.visibleif)">
                                        <div class="col-sm-6">
                                            <span>{{parent.description}} </span>
                                        </div>
                                        <div class="col-sm-6 text-left">
                                            <ratingQuestion :key=" _fieldGroupID" :clearData="completeAnswer" :answer="answer" :fieldid="template.fieldid" :parentfieldid="template.parentfieldid" :subfieldid="template.subfieldid"></ratingQuestion>
                                        </div>
                                    </div>
                                    <div class="row" v-if="template.subfieldid == 3 && checkVisible(template.visibleif)">
                                        <div class="col-sm-12">
                                            <span>{{template.description}} </span>
                                        </div>
                                        <div class="col-sm-12 text-center">
                                            <yesOrNot :key=" _fieldGroupID" :clearData="completeAnswer" :answer="answer" :guidsurvey="template.guidfield" :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></yesOrNot>
                                        </div>
                                    </div>
                                    <div class="row" v-if="template.subfieldid == 5 && checkVisible(template.visibleif)">
                                        <div class="col-sm-6 bold">{{template.description}}  {{template.fieldid}} </div>
                                        <div class="col-sm-6">
                                            <optionsQuestion :key=" _fieldGroupID" :clearData="completeAnswer" :answer="answer" :fieldid="template.fieldid" :parentfieldid="template.parentfieldid" :subfieldid="template.subfieldid" :options="template.fieldoptions"></optionsQuestion>
                                        </div>
                                    </div>
                                    <div class="row" v-if="(template.subfieldid == 6 && checkVisible(template.visibleif)) && template.fieldid !== 1201 ">
                                        <!--&& template.fieldid !== 1201-->
                                        <div class="col-sm-12">
                                            <span class="bold">{{template.description}}  {{template.fieldid}}  </span>
                                        </div>
                                        <div class="col-sm-6">
                                            <textField :key=" _fieldGroupID" :clearData="completeAnswer" :answer="answer" :fieldid="template.fieldid" :parentfieldid="template.parentfieldid" :subfieldid="template.subfieldid"></textField>
                                        </div>
                                    </div>
                                    <div class="row" v-if="template.subfieldid == 7 && checkVisible(template.visibleif)">
                                        <div class="col-sm-12">
                                            <span>{{template.description}} </span>
                                        </div>
                                        <div class="col-sm-12">
                                            <comments :key=" _fieldGroupID" :clearData="completeAnswer" :answer="answer" :fieldid="template.fieldid" :parentfieldid="template.parentfieldid" :subfieldid="template.subfieldid"></comments>
                                        </div>
                                    </div>

                                    <!--se generan las preguntas que tienen parent id-->
                                    <div class="mt-2" v-for="parent in surveytypes.parent" v-if="parent.parentfieldid == template.fieldid">
                                        <div class="row" v-if="parent.subfieldid == 1 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-6">
                                                <span>{{parent.description}}</span>
                                            </div>

                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 2 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-6">
                                                <span>{{parent.description}}</span>
                                            </div>
                                            <div class="col-sm-6 text-left">

                                                <ratingQuestion :clearData="completeAnswer" :answer="answer" :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></ratingQuestion>
                                            </div>
                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 3 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-12">
                                                <span>{{parent.description}}  </span>
                                            </div>
                                            <div class="col-sm-12 text-center">
                                                <yesOrNot :clearData="completeAnswer" :answer="answer" :guidsurvey="parent.guidfield" :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></yesOrNot>
                                            </div>
                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 5 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-6 bold">{{parent.description}}</div>
                                            <div class="col-sm-6">
                                                <optionsQuestion :clearData="completeAnswer" :answer="answer" :options="parent.fieldoptions" :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></optionsQuestion>
                                            </div>
                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 6 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-12">
                                                <span class="bold">{{parent.description}}</span>
                                            </div>
                                            <div class="col-sm-6">
                                                <textField :clearData="completeAnswer" :answer="answer" :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></textField>
                                            </div>
                                        </div>
                                        <div class="row" v-if="parent.subfieldid == 7 && checkVisible(parent.visibleif)">
                                            <div class="col-sm-12">
                                                <span>{{parent.description}}</span>
                                            </div>
                                            <div class="col-sm-12">
                                                <comments :clearData="completeAnswer" :answer="answer" :fieldid="parent.fieldid" :parentfieldid="parent.parentfieldid" :subfieldid="parent.subfieldid"></comments>
                                            </div>
                                        </div>



                                        <!--3er grupo de preguntas con parent id-->
                                        <div class="mt-2" v-for="parent2 in surveytypes.parent" v-if="parent2.parentfieldid == parent.fieldid">
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
                                                    <ratingQuestion :clearData="completeAnswer" :answer="answer" :fieldid="parent2.fieldid && checkVisible(parent2.visibleif, parent2.guidsurvey)" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></ratingQuestion>
                                                </div>
                                            </div>
                                            <div class="row" v-if="parent2.subfieldid == 3  && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-12">
                                                    <span>{{parent2.description}} </span>
                                                </div>
                                                <div class="col-sm-12 text-center">
                                                    <yesOrNot :clearData="completeAnswer" :answer="answer" :guidsurvey="parent2.guidfield" :fieldid="parent2.fieldid" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></yesOrNot>
                                                </div>
                                            </div>
                                            <div class="row" v-if="parent2.subfieldid == 5  && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-6 bold">{{parent2.description}}</div>
                                                <div class="col-sm-6">
                                                    <optionsQuestion :clearData="completeAnswer" :answer="answer" :options="parent2.fieldoptions" :fieldid="parent2.fieldid" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></optionsQuestion>
                                                </div>
                                            </div>
                                            <div class="row" v-if="parent2.subfieldid == 6 && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-12">
                                                    <span class="bold">{{parent2.description}}</span>
                                                </div>
                                                <div class="col-sm-6">
                                                    <textField :clearData="completeAnswer" :answer="answer" :fieldid="parent2.fieldid" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></textField>
                                                </div>
                                            </div>
                                            <div class="row" v-if="parent2.subfieldid == 7 && checkVisible(parent2.visibleif)">
                                                <div class="col-sm-12">
                                                    <span>{{parent2.description}}</span>
                                                </div>
                                                <div class="col-sm-12">
                                                    <comments :clearData="completeAnswer" :answer="answer" :fieldid="parent2.fieldid" :parentfieldid="parent2.parentfieldid" :subfieldid="parent2.subfieldid"></comments>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-4 mt-4" v-if="picturePoll !== null">
                                        <img class="img-fluid" :src="'/Images/surveys/'+ picturePoll" alt="Alternate Text" />
                                    </div>
                                    <div class="col-sm-12 mt-3">
                                        <label style="margin-left: -6px" class="hand" for="customFileLang"><i style="font-size: 45px" :class="[files.length > 0 ? 'green' : '', 'material-icons']">insert_photo</i> </label>
                                        <input hidden type="file" ref="files" class="form-control-file" id="customFileLang"@change="upload($event)">
                                        <span>{{photoFileName}}</span>
                                    </div>
                                    <div class="col-sm-12" v-if="files.length > 0">
                                        <span style="color: dodgerblue" @click="deleteImage">Delete Image</span>
                                    </div>
                                    <div class="col-sm-12 mt-3 text-center">
                                        <br />
                                        <div v-if="sendingPoll" class="text-center">
                                            <span v-if="hasMembership"> Activating membership and saving poll, please wait a moment...</span>
                                            <span v-else> Saving poll, please wait a moment...</span>
                                            <pulse-loader color="#31A3DD"></pulse-loader>
                                        </div>
                                        <span style="color: red"><strong>{{errorAmbassador}}</strong></span><br />
                                        <span style="color: red"><strong>{{errorMembership.notEmail}}</strong></span><br />
                                        <span style="color: red"><strong>{{errorMembership.notFolio}}</strong></span><br />
                                        <span style="color: red"><strong>{{errorLocation}}</strong></span><br />

                                        <button v-if="$root.Shared.Session.RoleID !== 'c96146b2-e61e-4296-a8bb-9ed16d27c66a' &&
                                        $root.Shared.Session.RoleID !== 'fe357275-29fe-422a-9b39-5ce5ca42bbea'" class="submit" @click="saveData">
                                            SUBMIT MY ANSWERS
                                        </button>
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
        {{cAnswer}}
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
    import ClipLoader from 'vue-spinner/src/ClipLoader.vue'
    Vue.component('v-select', vSelect)

    export default {
        props: ['fieldGroupID','surveyname', 'description', 'answer' ,'completeAnswer' ],
            data: function () {
                return {
                    tempevent: "",
                    picturePoll: null,
                    photoFileName: null,
                    files:[],
                    locations: [],
                    locationPoll: null,
                    hasFolio:false,
                    hasEmail:false,
                    modeOfMembership: 1,
                    membershipSent: false,
                    locations: [],
                    sendingPoll: false,
                    errorMembership: {
                        codeActive: '',
                        code: '',
                        ambassador: '',
                        location: '',
                        notEmail: '',
                        notFolio:'',
                    },
                    location: null,
                    hasMembership: false,
                    
                    optionsMembership: [
                        { text: 'Yes', value: true },
                        { text: 'No', value: false },
                    ],
                    optionsMode: [
                        { text: 'Free Trial', value: 1 },
                        { text: 'Already Paid', value: 2 },
                    ],
                    membershipAmbassador: null,
                    membershipAmbassadorActive: null,
                    inactivescodes: [],
                    activesCodes: [],
                    loadingcodes: false,
                    loadingcodesactives: false,
                    check: {
                        cell: 0,
                        phone: 0,
                        email: 0,
                    },
                    saveContact: false,
                    contactInfo: false,
                    hasData: {
                        phone: false,
                        cell: false,
                        email: false
                    },
                    errorAmbassador: '',
                    errorLocation: '',
                    ambassadors: [],
                    ambassador: '',
                    surveytemplate: [],
                    surveytypes: {
                        parent: [],
                        noparent: [],
                    },
                    code: null,
                    activeCode:null,
                    survey: {
                        name: '',
                        description: '',
                    },
                    count: 0,
                    surveyInJson: [],
                    visibleGuids: [],
                    test: 0,
                    formdata: null,
                    urls:[],
                    
                }
        },

        components: {
            'PulseLoader': PulseLoader,
            'ratingQuestion': ratingQuestion,
            'textField': textField,
            'comments': comments,
            'optionsQuestion': optionsQuestion,
            'yesOrNot': yesOrNot,
            'ClipLoader': ClipLoader,
        },
        computed: {
            cAnswer: function () {
                if (this.answer !== undefined) {
                    this.ambassador = {
                        label: this.completeAnswer.ambassadorName,
                        value: this.completeAnswer.ambassadorid
                    };
                    console.log(this.completeAnswer.locationPoll + this.completeAnswer.locationIDPoll);
                    if (this.completeAnswer.locationPoll !== undefined) {
                        this.locationPoll = {
                            label: this.completeAnswer.locationPoll,
                            value: this.completeAnswer.locationIDPoll
                        };
                    } else {
                        this.locationPoll = null;
                    }                  
                }
                if (this.completeAnswer !== undefined) {
                    if (this.completeAnswer.cardCode !== 0) {
                        this.code = {
                            label: this.completeAnswer.cardCode,
                            value: this.completeAnswer.cardID
                        }
                        this.activeCode = {
                            label: this.completeAnswer.cardCode,
                            value: this.completeAnswer.cardID
                        }
                        if (!this.completeAnswer.isProgress || this.completeAnswer.isProgress == null) {
                            this.modeOfMembership = 2
                        } else {
                            this.modeOfMembership = 1;
                        }
                        this.hasMembership = true;
                        this.location = {
                            label: this.completeAnswer.location,
                            value: this.completeAnswer.locationID
                        }
                        this.membershipAmbassador = this.completeAnswer.ambassador;
                        this.membershipSent = true;
                    } else {
                        this.code = null;
                        this.location = null;
                        this.hasMembership = false;
                        this.membershipAmbassador = "";
                    }
                    this.hasData.phone = this.completeAnswer.contactInfo;
                    this.picturePoll = this.completeAnswer.picture;
                }
                return ""
            },
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
           
            surveyFinished: function () {
                const r = this;              
                if (this.count == this.surveyInJson.length && this.surveyInJson.length !== 0) {                                    
                    var survey = JSON.stringify(this.surveyInJson);
                    console.log(this.surveyInJson);
                    var url = "/Content/management/SaveSurvey";
                    var answer = 0;
                   

                    var fecha = this.createDate($('#calendario5').val());
                    var departureDate = this.createDate($('#calendario10').val());
                    if (this.completeAnswer !== undefined) {
                        url = "/Content/management/UpdateSurvey";
                        answer = this.completeAnswer.id;                   
                    }
                    var code = "no";
                    var toLink = 0;
                    var location = 0;
                    if (this.membershipSent) {
                        code = "no";
                        
                    } else {
                        if (this.modeOfMembership == 1 ) {
                            if (this.code !== null) {
                                code = {
                                    password: this.code.label.toString().padStart(7, "0"),
                                    code: this.code.label,
                                    membershipID: this.code.value,
                                };
                                code = JSON.stringify(code);
                            } 
                            if (this.location !== null) location = this.location.value;
                        }
                        if (this.modeOfMembership == 2) {
                            if (this.activeCode !== null) {
                                toLink = this.activeCode.membershipID;
                            } 
                        }
                        this.sendingPoll = true;
                    }
                   
                    
                    console.log(this.files.length);
                    axios.post(url, {
                        fieldGroupID: this.fieldGroupID,
                        data: survey,
                        ambassadorID: this.ambassador.value,
                        contactInfo: this.saveContact,
                        culture: this.cCulture,
                        answerID: answer,                        
                        date: fecha,                       
                        membership: code,
                        location: location,
                        membershipToLink: toLink,
                        locationID: this.locationPoll.value,
                        departureDate: departureDate
                    }).then(response => {
                        console.log(this.files.length);
                        if (this.files.length > 0) {
                            /************* **************************************************************/
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
                            /*************/
                            if (this.completeAnswer == undefined) {
                                files.append('id', response.data);
                            } else {
                                files.append('id', this.completeAnswer.id);
                            }
                            
                            axios.post('/Content/management/UploadSurvey', r.formdata, config).then(
                                response => {
                                    if (this.completeAnswer !== undefined) {
                                        this.picturePoll = response.data;
                                    }
                                    
                                    console.log("imagen" + response.data);
                                    this.deleteImage();
                                    $.alert('Survey submited succefully');

                                }
                            );
                            /***************************************/
                        } else {
                            $.alert('Survey submited succefully');
                        }
                            
                            if (this.completeAnswer == undefined) {
                                this.code = null;
                                this.location = null;
                                this.membershipAmbassador = "";
                                this.hasMembership = false;
                            } else {

                                if (this.hasMembership) {
                                    if (!this.membershipSent) {
                                        if (this.modeOfMembership == 1) {
                                            bus.$emit('updateCardCode', {
                                                cardCode: this.code.value
                                            });
                                        } else {
                                            bus.$emit('updateCardCode', {
                                                cardCode: this.activeCode.membershipID
                                            });
                                        }
                                       
                                    }
                                    

                                    this.membershipSent = true;
                                }
                            }
                            this.sendingPoll = false;
                            this.surveyInJson = [];
                            
                            bus.$emit('closedetails', {
                               
                            });
                            this.activeCode = null;
                            bus.$emit('updateTable', {
                                folio: survey,
                                ambassador: this.ambassador.label,
                                contactInfo: this.saveContact,
                                fecha: $('#calendario5').val()

                            });
                            
                                             
                        })/*.catch(error => {
                        this.sendingPoll = false;
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
                    })*/;                                       
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
        },
        methods: {
            deleteImage() {
                this.files = [];
                this.photoFileName = null;
            },
            pictureName() {
                var fullPath = document.getElementById('customFileLang').value;
                if (fullPath) {
                    var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
                    var filename = fullPath.substring(startIndex);
                    if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
                        filename = filename.substring(1);
                    }
                    this.photoFileName = filename;
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
                this.hasimage = true;
                this.pictureName();

            },
            createDate(date) {
                var dia = parseInt(date[0] + date[1]);
                var mes = parseInt(date[3] + date[4]);
                var anio = parseInt(date[6] + date[7] + date[8] + date[9]);
                return new Date(anio, mes - 1, dia);
            },
            getLocations: function () {
                axios.get('/membership/activateMembership/getLocations', {
                }).then(response => {
                    this.locations = response.data;
                });
            },
            getInactivesCodes: function () {
                if (this.membershipSent) return false;

                this.loadingcodes = true;
                axios.get('/membership/activateMembership/getInactivesCodes', {
                }).then(response => {
                    this.inactivescodes = response.data;
                    if (this.code !== null) {
                        this.membershipAmbassador = this.code.name;
                    } else {
                        this.membershipAmbassador = "";
                    }
                    this.loadingcodes = false;
                });
            },
            getActivesCodes: function () {
                if (this.membershipSent) return false;

                this.loadingcodesactives = true;
                axios.get('/Content/management/GetActivesCodes', {
                }).then(response => {
                    this.activesCodes = response.data;
                    if (this.activeCode !== null) {
                        this.membershipAmbassadorActive = this.activeCode.name;
                    } else {
                        this.membershipAmbassadorActive = "";
                    }
                    this.loadingcodesactives = false;
                });

            },
            getDate: function (d) {
                if (d == null) {
                    return new date();
                } else {                
                    var getdate = d.split("/Date(").join("");
                    getdate = getdate.split(")/").join("");
                    var date = new Date(parseInt(getdate));
                    
                    var dia = date.getUTCDate();
                    var mes = date.getUTCMonth();
                    var anio = date.getUTCFullYear();
                    var fecha = new Date(anio, mes, dia);
                    
                    var month = fecha.getUTCMonth() + 1;
                    var fix =  month.toString().padStart(2, "0") + "/" + fecha.getUTCDate().toString().padStart(2, "0") + "/" + fecha.getUTCFullYear();
                    console.log(fix);
                    return fix;
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
            dateFormat: function () {
                var r = this;
                var toDate = new Date();
                var departureDate = new Date();

                if (r.completeAnswer !== undefined) {
                    toDate = this.getDate(this.completeAnswer.dateSaved);
                    if (this.completeAnswer.departureDate !== null) {
                        departureDate = this.getDate(this.completeAnswer.departureDate)
                    } else {
                        departureDate = toDate;
                    }                 
                }             
                $(function () {
                    $('#datetimepicker5').datetimepicker({
                        format: 'DD-MM-YYYY',
                        date: toDate
                    });
                    $('#datetimepicker10').datetimepicker({
                        format: 'DD-MM-YYYY',
                        date: departureDate
                    });
                });
          
            },
            
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
            verifyFields: function () {
                var error = false;
                this.errorMembership.code = "";
                this.errorAmbassador = "";
                this.errorMembership.ambassador = "";
                this.errorMembership.location = "";
                this.errorMembership.activeCode = "";
                this.errorMembership.notEmail = "";
                this.errorMembership.notFolio = "";
                this.errorLocation = "";
                if (this.ambassador == '' || this.ambassador == null) {
                    this.errorAmbassador = "Please select an ambassador";
                    error = true;
                }
                if (this.locationPoll == null || this.locationPoll == "" ) {
                    this.errorLocation = "Please select location";
                    error = true;
                }   
                if (this.hasMembership) {
                    if (this.modeOfMembership == 1) {
                        if (this.code == null) {
                            error = true;
                            this.errorMembership.code = "Select a Card Code"
                        }

                        if (this.location == null) {
                            error = true;
                            this.errorMembership.location = "Select a location"
                        }
                    } else {
                        if (this.activeCode == null) {
                            error = true;
                            this.errorMembership.activeCode = "Select a Card Code"
                        }
                    }

                    if (!this.hasEmail) {
                        error = true;
                        this.errorMembership.notEmail ="Please if you check YES on Privilege Card, fill de E-mail field."
                    }

                }
                if (!this.hasFolio) {
                    error = true;
                    this.errorMembership.notFolio = "Please write a Polls Folio"
                }

                return error;
            },
            saveData: function () {

                var error = this.verifyFields();
                if (error) return false;  
                
                const r = this;
                var title = "Submit survey";
                if (this.completeAnswer != undefined) title = "Update survey";
               
                this.saveContact = this.contactInfo;
                $.confirm({
                    title: title,
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
            getLocations: function () {
                axios.get('/Content/management/GetLocations', {

                }).then(response => {
                    console.log(response.data);
                    this.locations = response.data;
                }).catch(function () {
                });;
            },
        },

        mounted() {
           /* axios.get('/Content/management/getEmails', {

            }).then(response => {
                console.log("folio es " + response.data);
            }).catch(function () {
            });;
            */
            this.getLocations();
            this.getAmbassadors();
            this.surveyInJson = [];
            bus.$on('savedata', obj => {
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
            bus.$on('hasEmail', obj => {
                this.hasEmail = obj.email;
            });
            bus.$on('hasFolio', obj => {
                this.hasFolio = obj.folio;
            });
            bus.$on('componentcount', obj => {
                this.count = this.count + 1;
            });
            bus.$on('saveGuid', obj => {            
                this.visibleGuids.push(obj.guid);
            });
            bus.$on('contactInfoPhone', obj => {             
                    this.hasData.phone = obj.contactInfo;
                         
            });

            bus.$on('contactInfoCell', obj => {
                    this.hasData.cell = obj.contactInfo;                      
            });

            bus.$on('contactInfoEmail', obj => {            
                    this.hasData.email = obj.contactInfo;         
            });
            bus.$on('deleteGuid', obj => {
                var r = this;
                Array.from(this.visibleGuids).forEach(function (data, index) {
                    if (obj.guid == data) {
                        r.visibleGuids.splice(index, 1);
                    } 
                });              
            });
            this.dateFormat();
            this.getLocations();
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
    .custom-control-input {
        margin-top: 6px;
        position: absolute;
        z-index: 5;
        opacity: 19;
        margin-left: -18px;
    }

    .custom-control {
        position: relative;
        display: inline !important;
        min-height: 1.5rem;
        padding-left: 1.5rem;
    }
    .green {
        color:green;
    }
    
</style>
