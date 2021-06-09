<template>
    <div>
        {{update}}
        <textarea rows="2" v-model="respuesta" class="bordergreen form-control"></textarea>
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
                    respuesta: '',
                    now: false,
                    uflag: 0,
                    flag: 0,
                }
        },
        components: {
            'PulseLoader': PulseLoader,
        },
        methods: {
            
          
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
            this.uflag = 1;
            bus.$emit('componentcount', {

            });
        },
        }

</script>
<style>
    .hand {
        cursor: pointer;
    }
</style>
