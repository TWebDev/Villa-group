<template>
    <div>
        <div class="answer-button" :style="{ backgroundColor: yes, color: color1}" @click="selectOption(1)">Yes</div>
        <div class="answer-button" :style="{ backgroundColor: no, color: color2}" @click="selectOption(2)">No</div>
        {{update}}
    </div>
</template>
<script>
    import { bus } from '../app.js';
    import axios from 'axios';
    import vSelect from 'vue-select';

    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'

    export default {
        props: ['subfieldid','parentfieldid', 'fieldid', 'guidsurvey'],
            data: function () {
                return {
                    respuesta: '',
                    now: false,
                    option: '',
                    color1: '',
                    color2: '',
                    count: 0,
                    uflag: 0,
                    flag: 0,
                }
        },
        components: {
            'PulseLoader': PulseLoader,
        },
        methods: {
            selectOption: function (n) {
                console.log(n);
                this.respuesta = n;          
            },
            colorwhite: function () {

            },
          
        },
        computed: {
            updateCount: function () {
                var id = this._fieldid;
                if (this.uflag !== this.flag) {
                    bus.$emit('componentcount', {

                    });
                    this.uflag = this.flag;
                }

                return "";
            },
            _guidsurvey: function () {
                return this.guidsurvey;
            },
            yes: function () {
                if (this.respuesta == 1) {
                    this.count = 0; 
                    this.color1 = "white";

                    bus.$emit('deleteGuid', {
                        guid: this._guidsurvey,
                    });

                    return "#00AAA5";
                }
                if (this.respuesta == 2) {
                    this.color1 = "black";
                    return "";
                }
            },
            no: function () {
                if (this.respuesta == 2) {
                    this.count = this.count + 1;                
                    this.color2 = "white";
                    if (this.count == 1) {
                        bus.$emit('saveGuid', {
                            guid: this._guidsurvey,
                        });
                    }
                    this.count = this.count + 1;                
                    return "#00AAA5";
                } 
                if (this.respuesta == 1) {
                    this.color2 = "black";
                    this.count = 0;   
                    return "";
                }
            },
            _fieldid: function () {
                return this.fieldid;
            },
            _parentfieldid: function () {
                return this.parentfieldid;
            },
            update: function () {
                if (this.now) {
                    bus.$emit('savedata', {
                        response: 'textfield',
                        subfieldid: this._subfieldid,
                        answer: this.respuesta,
                        parentfieldid: this._parentfieldid,
                        fieldid: this._fieldid,
                    });
                    this.respuesta = "";
                    this.now = false;
                }
                return "";                  
            },
            _subfieldid: function () {
                return this.subfieldid;
            },
        },

        mounted() {
            bus.$on('save', obj => {
                this.now = true;
            });
            bus.$emit('componentcount', {

            });
            this.uflag = 1;
            },
        }

</script>
<style>
    .hand {
        cursor: pointer;
    }
</style>
