
<template>
    <div id="reminders" class="col col-md-4 col-lg-3 d-none bg-light sidebar pt-4 px-4">
        <button type="button" class="close float-right" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
        <h5>My Reminders</h5>
        <div class="form-group">
            <button class="btn btn-primary" v-on:click="openNewReminder">New Reminder</button>
        </div>
        <div id="newReminders" class="container" v-if="newReminder">
            <div class="row">
                <div class="col">
                    <div class="form-group">
                        <label>Date</label>
                        <div id="datetimepicker3" data-target-input="nearest">
                            <div class="input-group-append" data-target="#datetimepicker3" data-toggle="datetimepicker">
                                <input type="text"
                                       v-model="date"
                                       class="form-control datetimepicker-input"
                                       data-target="#datetimepicker3"
                                       placeholder="yyyy-mm-dd" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <label>Name</label>
                        <input id="txtName" type="text" class="form-control" v-model="name" />
                    </div>
                    <div class="form-group">
                        <label>Description</label>
                        <textarea id="txtDescription" class="form-control" v-model="description"></textarea>
                    </div>
                    <div class="form-group">
                        <label>Repeat</label>
                        <select class="form-control" v-model="recurr">
                            <option v-for="(option, i) in reminders.types" 
                                    v-bind:value="option.value">
                                    <!--v-bind:value="{text:option.text,value:option.value}"-->
                                    {{option.text}}
                            </option>
                        </select>
                    </div>
                    <div class="form-group">
                        <div class="form-control"
                             v-show="weekly"
                             v-for="(day,week,i) in reminders.reminder.repeat.weekly"
                             @change="setWeek(week,!day)">
                            <div class="form-check form-check-inline">
                                <input class="form-check-input" type="checkbox"
                                       v-bind:id="week"
                                       v-bind:value="day" />
                                <label class="form-check-label" :for="week">{{week}}</label>
                            </div>
                        </div>
                    </div>
                    <div v-if="montly" class="form-group">
                        <div class="form-control"
                             v-for="(month,montly,i) in reminders.reminder.repeat.montly"
                             @change="setMonth(montly,!month)">
                            <div class="form-check form-check-inline">
                                <input class="form-check-input" type="checkbox"
                                       v-bind:id="montly"
                                       v-bind:value="month">
                                <label class="form-check-label" :for="montly">{{montly}}</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row text-righ">
                <div class="form-group">
                    <button class="btn btn-primary" v-on:click="clear">Clear</button>
                    <button class="btn btn-primary" v-on:click="save">Save</button>
                </div>
            </div>
        </div>
        <div class="container">
                <div class="row"
                     v-for="(reminder,t,r) in reminders.reminders"
                     v-bind:id="`rem-${reminders.reminderID}`"
                     v-bind:key="reminder.reminderID">
                    <div class="col card border rounded">
                        <div class=" row">
                            <div class="col-sm-6 font-weight-bold">
                                <label>{{reminder.name}}</label>
                            </div>
                            <div class="col text-right">
                                <label>on/off</label>
                            </div>
                        </div>
                        <div class="col">
                            <p>{{reminder.description}}</p>
                            <label class="text-muted">{{reminder.type}}-{{reminder.dateAlarm}}</label>
                        </div>
                    </div>
                </div>
        </div>
    </div>
</template>

<script>
    import { mapState } from 'vuex';
    export default ({
        data: function(){
            return {
                newReminder: false,
                daily: false,
                weekly: false,
                montly: false,
                oneTime: false
            }
        },
        computed: {
            ...mapState({
                reminders:'reminders'
            }),
            name:{
                get() {
                    return this.$store.state.reminders.reminder.name;
                },
                set(value) {
                    this.$store.commit('UPDATE_REMINDER_NAME', value);
                }
            },
            date: {
                get() {
                    return moment().add('1', 'days').format("YYYY-MM-DD hh:mm A");
                },
                set(value) {
                    setDate(value);
                }
            },
            description: {
                get() {
                    return this.$store.state.reminders.reminder.description;
                },
                set(value) {
                    this.$store.commit('UPDATE_REMINDER_DESCRIPTION', value);
                }
            },
            checked: {
                get() {
                    return this.$store.state.reminders.reminder.url;
                },
                set(value) {
                    this.url(value);
                }
            },
            recurr:{
                get() {
                    return this.$store.state.reminders.reminder.recur;
                },
                set(value) {
                    this.getRecurr(value);
                }
            }
        },
        methods: {
            getRecurr(option) {
                let self = this;
                console.log(self.$store.state.reminders.types);
               // this.$store.commit('UPDATE_REMINDER_TYPE', { option.text, option.value })
                switch (option) {
                    case "1": {//daily
                        self.$store.commit('UPDATE_REMINDER_RECUR', option);
                        self.dialy = true;
                        self.weekly = false;
                        self.oneTime = false;
                        self.montly = false;
                        break;
                    }
                    case "2": {//weekly
                        self.$store.commit('UPDATE_REMINDER_RECUR', option);
                        self.weekly = true;
                        self.montly = false;
                        self.daily = false;
                        self.oneTime = false;
                        break;
                    }
                    case "3": {//montly
                        self.$store.commit('UPDATE_REMINDER_RECUR', option);
                        self.montly = true;
                        self.weekly = false;
                        self.daily = false;
                        self.oneTime = false;
                        break;
                    } 
                    case "4": {//one time 
                        self.$store.commit('UPDATE_REMINDER_RECUR', option);
                        self.oneTime = true;
                        self.weekly = false;
                        self.montly = false;
                        self.daily = false;
                        break;
                    }
                }
            },
            setDate(value) {
                this.$store.commit('UPDATE_REMINDER_DATE', value);
            },
            url(value) {
                if (value) {
                    this.$store.commit('UPDATE_REMINDER_URL', window.location.href);
                }
            },
            clear() {
                this.$store.commit('CLEAR_REMINDER');
            },
            save() {

                this.$store.dispatch('saveReminder');
                this.clear();
            },
            runTime: function () {
                $(function () {
                    $('#datetimepicker3').datetimepicker({
                        format: 'YYYY-MM-DD hh:mm A'
                    });
                });
            },
            setWeek(day,value) {
                day = day.toUpperCase();
                this.$store.commit('UPDATE_REMINDER_WEEK_' + day, value);
            },
            setMonth(month, value) {
                month = month.toUpperCase();               
                this.$store.commit('UPDATE_REMINDER_MONTH_' + month, value);
            },
            openNewReminder() {
                let self = this;
                self.clear();
                self.newReminder = true;
            }
        },
        mounted() {
            this.runTime();
        }
    })
</script>

<style>
    #RemindersModalTable{
        z-index:3000;
    }
    .modal-backdrop {
        z-index: 10;
    }
    .modal-dialog{
        max-width:650px;
    }
    #reminders{
        overflow:auto;
    }
 
</style>