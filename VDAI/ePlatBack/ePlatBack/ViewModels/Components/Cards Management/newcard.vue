<template>
    <div>
        <div class="text-right mb-2">
            <div class="">
                <button class="btn btn-primary" @click="changeComponent">
                    <i class="material-icons" style="vertical-align: middle;">
                        arrow_back
                    </i> Back to Management
                </button>
            </div>
        </div>
        <div class="container-fluid ">
            <div class="row">
                <div class="card col-sm-12">
                    <div class="card-body">
                        <h5 class="card-title">Assing Cards</h5>
                        <div class="row">
                            <div class="col-sm-6 col-md-4 col-lg-3">
                                <span>Ambassador</span>
                                <v-select v-model="ambassador" :options="ambassadorsFilter"></v-select>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-3">
                                <span>Initial Card Code</span>
                                <input class="form-control" type="number" min="0" v-model="initial">
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-3">
                                <span>Final Card Code</span>
                                <input class="form-control" type="number" min="0" v-model="final">
                            </div>
                        </div>
                        <div class="mt-2 text-center">
                            <span style="color: red"> <b>{{error}}</b> </span>
                        </div>
                        <div v-if="loading" class="mt-2 text-center">
                            <h5>Adding cards, please wait a moment </h5><pulse-loader color="#31A3DD"></pulse-loader>
                        </div>
                        <div class="mt-2 text-right">
                            <button class="btn btn-success" @click="saveCards">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    import vSelect from 'vue-select';
    import axios from 'axios';
   

    Vue.component('v-select', vSelect)
      
    export default {
        props: ['cards'],
        data: function () {
            return {
                test: 'SPC single page component',
                ambassador: null,
                initial: '',
                error: '',
                users:'',
                loading: false,
                final: '',
                names: [],
                date:'',

            }
        },
        components: {
            'PulseLoader': PulseLoader,
        },
        computed: {
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

                var duplicadosEliminados = eliminarObjetosDuplicados(this.names, 'value');
                return duplicadosEliminados;

            },
        },
        
        methods: {
            changeComponent: function () {
                this.$root.currentcomponent = "searchcards";
            },
            saveCards: function () {
                this.error = "";
                this.initial = Math.trunc(this.initial);
                this.final = Math.trunc(this.final);
                var status = "Inactive";
               

                console.log(this.initial);

                if (this.ambassador == null || this.initial == "" || this.final == "") {
                    this.error = "Please, fill out the fields correctly.";
                    return this.error;
                }
                if (this.initial < 0 || this.final < 0  ) {
                    this.error = "The codes must be greater than 0.";
                    return this.error;
                } 
                if (this.initial > this.final) {
                    this.error = "The Initial Code can't be greater than Final Code.";
                    return this.error;
                }
                    var text = "Assign to " + this.ambassador.label + " the Card Codes from " + this.initial + " to " + this.final;
                    let r = this;
                    $.confirm({
                        title: 'Confirm!',
                        content: text,
                        buttons: {
                            confirm: function () {
                                r.loading = true;
                                axios.post('/membership/cardsManagement/addCards', {
                                    ambassadorid: r.ambassador.value,
                                    initial: r.initial,
                                    final: r.final,
                                    status: status,
                                    date: r.date
                                }).then(response => {
                                    r.loading = false;
                                    if (response.data == "existe") {
                                        $.alert('The Card Code you wrote its already exists');
                                    }
                                    if (response.data == "ok") {
                                        $.alert('Cards assingned succesfully.');
                                        text = "";
                                        r.ambassador = "";
                                        r.initial = "";
                                        r.final = "";
                                    }
                                    console.log(response.data);
                                });
                            },
                            cancel: function () {
                            },
                        }
                    });
            },
        },
        async  mounted() {

               await axios.get('/membership/cardsManagement/getAmbassadors', {
                   
                }).then(response => {
                    this.names = response.data;
                });
            this.date = new Date();
             
            

            },
            
    }
        

</script>
