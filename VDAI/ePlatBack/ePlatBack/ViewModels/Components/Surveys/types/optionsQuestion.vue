<template>
    <div>
        {{update}}

        <select class="question" v-model="optionSelected">
            <option v-for="option in _optionQuestion" style="color: #696969 !important" class="question" :value="option">{{option}}</option>
        </select>
    </div>
</template>
<script>
    import { bus } from '../app.js';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'

    export default {
        props: ['options', 'subfieldid', 'parentfieldid','fieldid'],
            data: function () {
                return {
                    optionsQuestion: [],
                    optionSelected: '',
                    now: false,
                    uflag: 0,
                    flag: 0,
                }
        },
        components: {
            'PulseLoader': PulseLoader,
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
            _optionQuestion: function () {
               return this.getOptions();  
            },
            _fieldid: function () {
                
                return this.fieldid;
            },
            _option: function () {
              
                return this.options;

            },
            _parentfieldid: function () {
                return this.parentfieldid;
            },
            update: function () {
                if (this.now) {
                    bus.$emit('savedata', {                       
                        response: 'textfield',
                        subfieldid: this._subfieldid,
                        answer: this.optionSelected,
                        parentfieldid: this._parentfieldid,
                        fieldid: this._fieldid,
                    });
                    this.optionSelected = "";
                    this.now = false;

                }
                return "";
            },
            _subfieldid: function () {
                return this.subfieldid;
            },
        },
        methods: {
            dividirCadena: function (cadenaADividir, separador) {
                var arrayDeCadenas = cadenaADividir.split(separador);
                return arrayDeCadenas;
            },
            getOptions: function () {
                var coma = ",";
                this.optionsQuestion = [];
                this.optionsQuestion = this.dividirCadena(this._option, coma);
                return this.optionsQuestion;
               
            }
                     
        },

        mounted() {
            this.getOptions();
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
    .question select {
        margin: 10px 0;
        padding: .5em 1em;
        border: #00AAA5 solid 1px;
        border-radius: 3px;
        background-color: white;
        width: 40%;
        height: 24px;
    }
    .question {
        margin: 10px 0;
        padding: .5em 1em;
        border: #00AAA5 solid 1px;
        border-radius: 3px;
        background-color: white;
        min-height: 40px;
        width: 270px;
    }
</style>
