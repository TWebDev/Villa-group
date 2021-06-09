<template>
    <div>
        <input type="text" v-model="respuesta" class="bordergreen form-control" />

        {{update}}
        {{updateCount}}
        {{contactInfoPhone}}
        {{contactInfoCell}}
        {{contactInfoEmail}}

    </div>
</template>
<script>
    import { bus } from '../app.js';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'

    export default {
        props: ['subfieldid','parentfieldid', 'fieldid'],
            data: function () {
                return {
                    respuesta: '',
                    now: false,
                    uflag: 0,
                    flag:0,

                }
        },
        components: {
            'PulseLoader': PulseLoader,
        },
        methods: {     
        },
        computed: {
            contactInfoPhone: function () {
                if ((this.fieldid == 1170 || this.fieldid == 1154) && this.respuesta.trim().length > 0) {
                    bus.$emit('contactInfoPhone', {
                        contactInfo: true,
                    });
                } else {
                    bus.$emit('contactInfoPhone', {
                        contactInfo: false
                    });
                }
                return "";
            },
            contactInfoCell: function () {
                if ((this.fieldid == 1171 || this.fieldid == 1197) && this.respuesta.trim().length > 0) {
                    bus.$emit('contactInfoCell', {
                        contactInfo: true,
                    });
                } else {
                    bus.$emit('contactInfoCell', {
                        contactInfo: false
                    });
                }
                return "";
            },
            contactInfoEmail: function () {
                if ((this.fieldid == 1172 || this.fieldid == 1153) && this.respuesta.trim().length > 0) {
                    bus.$emit('contactInfoEmail', {
                        contactInfo: true,
                    });
                } else {
                    bus.$emit('contactInfoEmail', {
                        contactInfo: false
                    });
                }
                return "";
            },



            
            updateCount: function () {
                var id = this._fieldid;
                if (this.uflag !==  this.flag) {
                   
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
            bus.$on('flag', obj => {
                this.uflag = this.uflag + 1;
            });
            bus.$emit('componentcount', {

            });
            //this.uflag = 1;
            },
        }

</script>
<style>
    .hand {
        cursor: pointer;
    }
</style>
