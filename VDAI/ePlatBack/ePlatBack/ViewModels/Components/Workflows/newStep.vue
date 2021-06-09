<template>
    <div class="modal" id="newStepModal" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">New Step</h5>
                </div>
                <div class="bodal-body">
                    <div class="card p-2">
                        <div class="card-header row">
                            <div class="form-group col-12">
                                <label>Step</label>
                                <select class="form-control" v-model="Step.Type">
                                    <option v-for="(option, key) in Types" :value="option.Text">{{option.Text}}</option>
                                </select>
                            </div>
                            <div class="form-group col-12" v-if="Step.Type == 'start'">
                                <label>Execution Type</label>
                                <select v-model="Step.ExecutionType" class="form-control">
                                    <option v-for="(option, key) in ExecutionTypes" :value="option.Value">{{option.Text}}</option>
                                </select>
                            </div>
                        </div>
                        <div class="card-body row">
                            <div class="form-group col-12" v-if="Step.Type == 'start' && Step.ExecutionType == 'periodical'">
                                <label for="infoInterval">Periodicity</label>
                                <select id="infoInterval" v-model="Step.Interval" class="form-control">
                                    <option v-for="(option, key) in Intervals" :value="option.Value">{{option.Text}}</option>
                                </select>
                            </div>
                            <div class="form-group col-6" v-if="Step.Type == 'start' && Step.ExecutionType == 'periodical'">
                                <label>Breakpoint</label>
                                <select v-model="Step.Breakpoint" class="form-control">
                                    <option v-for="(option, key) in Breakpoints" :value="option.Value">{{option.Text}}</option>
                                </select>
                            </div>
                            <div class="form-group col-6" v-if="Step.Type == 'start' && Step.ExecutionType == 'periodical'">
                                <label>Date Type</label>
                                <select v-model="Step.BpComparison" class="form-control">
                                    <option v-for="(option, key) in BpComparisons" :value="option.Value">{{option.Text}}</option>
                                </select>
                            </div>
                            <div class="form-group col-12" v-if="Step.Type == 'send'">
                                <label>Email Template</label>
                                <select class="form-control" v-model="Step.Email">
                                    <option v-for="(option, key) in Emails" :value="option.Value">{{option.Text}}</option>
                                </select>
                            </div>
                            <div class="form-group col-12" v-if="Step.Type == 'delay'">
                                <input type="text" class="form-control" placeholder="days" v-model="Step.DelayDays" />
                                <input type="text" class="form-control" placeholder="hours" v-model="Step.DelayHours" />
                            </div>
                            <div class="form-group col-12" v-if="Step.Type == 'click'">
                                <input type="text" class="form-control" placeholder="link" v-model="Step.LinkTo" />
                            </div>
                        </div>
                        <!--<div class="card-footer">
                        </div>-->
                    </div>
                </div>
                <div class="modal-footer row mx-1">
                    <button type="button" class="btn col order-4 order-lg-1 btn-secondary wf-btn w-auto mx-1 my-1" @click="closeModal">Cancel</button>
                    <button type="button" class="btn col order-3 order-lg-2 btn-danger wf-btn w-auto mx-1 my-1" @click="removeStep">Delete</button>
                    <button type="button" class="btn col order-2 order-lg-3 btn-primary wf-btn w-auto mx-1 my-1" @click="updateStep">Update</button>
                    <button type="button" class="btn col order-1 order-lg-4 btn-primary wf-btn w-auto mx-1 my-1" @click="saveStep">Add</button>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    //import axios from 'axios';
    export default {
        props: ['firstNode', 'actionType', 'emails', 'step'],
        data: function () {
            return {
                Types: [{ Text: 'start' }, { Text: 'open' }, { Text: 'yes' }, { Text: 'no' }, { Text: 'click' }, { Text: 'submit' }, { Text: 'delay' }, { Text: 'send' }, { Text: 'end' }],
                ExecutionTypes: [],
                Emails: [],
                Intervals: [],
                Breakpoints: [],
                BpComparisons: [],
                Step: {
                    ExecutionType: null,
                    Action: null,
                    Step: '',
                    Type: null,
                    Interval: null,
                    Breakpoint: null,
                    BpComparison: null,
                    Email: '',
                    DelayDays: '',
                    DelayHours: '',
                    LinkTo: '',
                    ParentStep: '',
                    Value: ''
                },
                TempStep: {
                    ExecutionType: null,
                    Action: null,
                    Step: '',
                    Type: null,
                    Interval: null,
                    Breakpoint: null,
                    BpComparison: null,
                    Email: '',
                    DelayDays: '',
                    DelayHours: '',
                    LinkTo: '',
                    ParentStep: ''
                }
            }
        },
        methods: {
            clearStep: function () {
                this.Step = {
                    ExecutionType: null,
                    Action: null,
                    Type: null,
                    Interval: null,
                    Breakpoint: null,
                    BpComparison: null,
                    Email: '',
                    DelayDays: '',
                    DelayHours: '',
                    LinkTo: '',
                    ParentStep: ''
                }
                this.TempStep = {
                    ExecutionType: null,
                    Action: null,
                    Type: null,
                    Interval: null,
                    Breakpoint: null,
                    BpComparison: null,
                    Email: '',
                    DelayDays: '',
                    DelayHours: '',
                    LinkTo: '',
                    ParentStep: ''
                }
            },
            saveStep: function () {
                let self = this;
                switch (self.Step.Type) {
                    case 'delay': {
                        self.Interval = null;
                        self.Step.Email = '';
                        self.Step.LinkTo = '';
                        break;
                    }
                    case 'send': {
                        self.Interval = null;
                        self.Step.DelayDays = '';
                        self.Step.DelayHours = '';
                        self.Step.LinkTo = '';
                        break;
                    }
                    default: {
                        //self.Interval = null;
                        self.Step.Email = '';
                        self.Step.DelayDays = '';
                        self.Step.DelayHours = '';
                        self.Step.LinkTo = '';
                        break;
                    }
                }
                self.Step.Action = 'Save';
                self.$emit('renderstep', _.clone(self.Step));//method from lodash
                self.clearStep();
                $('#newStepModal').modal('hide');
            },
            updateStep: function () {
                let self = this;
                switch (self.Step.Type) {
                    case 'delay': {
                        self.Interval = null;
                        self.Step.Email = '';
                        self.Step.LinkTo = '';
                        break;
                    }
                    case 'send': {
                        self.Interval = null;
                        self.Step.DelayDays = '';
                        self.Step.DelayHours = '';
                        self.Step.LinkTo = '';
                        break;
                    }
                }
                self.Step.Action = 'Update';
                self.Step.Value = self.Step.Type + self.Step.Step;
                self.$emit('renderstep', _.clone(self.Step));//method from lodash
                self.clearStep();
                $('#newStepModal').modal('hide');
            },
            removeStep: function () {
                let self = this;
                self.Step.Action = 'Delete';
                self.$emit('renderstep', _.clone(self.Step));//method from lodash
                self.clearStep();
                $('#newStepModal').modal('hide');
            },
            closeModal: function () {
                let self = this;
                if (!_.eq(self.TempStep, self.Step) && self.TempStep.Type != null) {
                    self.TempStep.Action = 'Update';
                    self.$emit('renderstep', _.clone(self.TempStep));//method from lodash
                }
                self.clearStep();
                $('#newStepModal').modal('hide');
            },
            fillIntervals: function () {
                let self = this;
                for (var i = 0; i <= 30; i++) {
                    var interval = {
                        Value: i,
                        Text: i == 0 ? 'No Repeat' : i
                    };
                    self.Intervals.push(interval);
                }

                self.Breakpoints = [{ Value: 'before', Text: 'Before' }, { Value: 'after', Text: 'After' }];
                self.BpComparisons = [{ Value: 'arrival', Text: 'Arrival Date' }, { Value: 'departure', Text: 'Departure Date' }, { Value: 'purchase', Text: 'Purchase Date' }, { Value: 'last', Text: 'Last Sending Date' }];
                self.ExecutionTypes = [{ Value: 'periodical', Text: 'Periodical' }, { Value: 'triggered', Text: 'Based in Action' }];
            },
        },
        watch: {
            firstNode: function (nVal) {
                let self = this;
                if (nVal == true) {
                    self.Step = {
                        Action: 'Save',
                        Step: '',
                        Type: 'start',
                        Breakpoint: null,
                        BpComparison: null,
                        Email: '',
                        DelayDays: '',
                        DelayHours: '',
                        LinkTo: '',
                        ParentStep: ''
                    };
                    //self.saveStep();
                }
            },
            emails: function (nVal, oVal) {
                this.Emails = nVal;
            },
            step: function (nVal, oVal) {
                this.TempStep = _.clone(nVal);
                this.Step = _.clone(nVal);
            },
            actionType: function (nVal) {
                this.Step.Type = nVal;
            }
        },
        mounted: function () {
            let self = this;
            self.fillIntervals();
        }
    }
</script>