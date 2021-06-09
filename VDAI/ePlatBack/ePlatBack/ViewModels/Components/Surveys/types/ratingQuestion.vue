<template>
    <div>
        {{update}}

        <div class="answer-button" :style="{ backgroundColor: b1, color: c1}" @click="rating(1)">1</div>
        <div class="answer-button" :style="{ backgroundColor: b2, color: c2}" @click="rating(2)">2</div>
        <div class="answer-button" :style="{ backgroundColor: b3, color: c3}" @click="rating(3)">3</div>
        <div class="answer-button" :style="{ backgroundColor: b4, color: c4}" @click="rating(4)">4</div>
        <div class="answer-button" :style="{ backgroundColor: b5, color: c5}" @click="rating(5)">5</div>
    </div>

</template>
<script>
    import { bus } from '../app.js';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'

    export default {
        props: ['subfieldid', 'parentfieldid', 'fieldid'],
            data: function () {
                return {
                    calification: 0,
                    now: false,
                    uflag: 0,
                    flag: 0,
                }
        },
        components: {
            'PulseLoader': PulseLoader,
        },
        methods: {
            background: function (n) {
                if (n == 1) {
                    return "#00AAA5";
                }
            },
            colorwhite: function () {

            },
            rating: function (n) {
                this.calification = n;
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
            c1: function () {
                if (this.calification > 0) {
                    return "white";
                }
            },
            c2: function () {
                if (this.calification > 1) {
                    return "white";
                }
            },
            c3: function () {
                if (this.calification > 2) {
                    return "white";
                }
            },
            c4: function () {
                if (this.calification > 3) {
                    return "white";
                }
            },
            c5: function () {
                if (this.calification == 5) {
                    return "white";
                }
            },
            b1: function () {
                if (this.calification > 0) {
                    return "#00AAA5"; 
                }
            },
            b2: function () {
                if (this.calification > 1) {
                    return "#00AAA5";
                }
            },
            b3: function () {
                if (this.calification > 2) {
                    return "#00AAA5";
                }
            },
            b4: function () {
                if (this.calification > 3) {
                    return "#00AAA5";
                }
            },
            b5: function () {
                if (this.calification == 5) {
                    return "#00AAA5";
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
                        answer: this.calification,
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
