<template>
    <div>
        <div class="container-fluid mt-5">
            <div class="row">
               
                <div class="card col-sm-12">
                    <div class="card-body">
                        <h5 class="card-title">Cards Details</h5>
                        <div class="row">
                            <div class="col-sm-6 col-md-4 col-lg-3">
                                <span>Ambassador</span>
                                <v-select :options="ambassadorsFilter" v-model="ambassador" ></v-select>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-3">
                                <span>Card Code</span>
                                <input class="form-control" type="text" :value="sheet"   disabled >
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-3">
                                <span>status</span>
                                <input class="form-control" type="text" :value="status" disabled>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-3">
                                <span>Saved User By</span>
                                <input class="form-control" type="text" :value="savedby" disabled>
                            </div>
                            <div class="col-sm-6 col-md-4 col-lg-3">
                                <span>Date Saved</span>
                                <input class="form-control" type="text" :value="fecha" disabled>
                            </div>
                        </div>
                        
                        <div class="mt-2 text-right">
                            <button class="btn btn-primary" @click="saveCard">Save</button>

                            <button class=" btn btn-danger" @click="deleteCard" v-if="status == 'Inactive'"><i class="material-icons md-48" style="vertical-align: middle;"> delete</i>  </button>

                            <button type="button" class="btn btn-danger" data-toggle="tooltip" v-else disabled data-placement="top" title="For delete this card, you need to inactive it.">
                                <i class="material-icons md-48" style="vertical-align: middle;"> delete</i> 
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    
    import { bus } from './app.js';
    import axios from 'axios';
    import vSelect from 'vue-select';
    var Bus = new Vue();
     Vue.component('v-select', vSelect)
    export default {
        props: ['ambassador', 'sheet', 'status', 'date','savedby', 'idambassador'],
            data: function () {
                return {
                    names: [],
                    ranges: false,
                    editar: false,
                    newambassador: '',

                }
        },
            computed: {
                fecha: function () {
                    var fecha = this.date;
                    if (fecha !== undefined) {

                        fecha = fecha.split("/Date(").join("");
                        fecha = fecha.split(")/").join("");
                        var dia = new Date(parseInt(fecha)).getDate();
                        var mes = new Date(parseInt(fecha)).getMonth();
                        mes = mes + 1;
                        var año = new Date(parseInt(fecha)).getFullYear();
                        fecha = dia + "-" + mes + "-" + año;
                        return fecha;
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

                    var duplicadosEliminados = eliminarObjetosDuplicados(this.names, 'value');
                    return duplicadosEliminados;

                },
            
        },
        methods: {
            editCard: function () {
                this.editar = true;
            },
            deleteCard: function () {
                
                var text = "Do you want delete this Cards Codes. ";
                var r = this;
                $.confirm({
                    title: 'Are you sure? ',
                    content: text,
                    typeAnimated: true,
                    type: 'red',
                    buttons: {
                        delete: {
                            text: 'Delete',
                            btnClass: 'btn-red',
                            action: function () {
                                r.loading = true;
                                axios.post('/membership/cardsManagement/deleteCard', {
                                    code: r.sheet,
                                   
                                }).then(response => {
                                    
                                    if (response.data == "ok") {
                                        $.alert('Card was deleted correctly.');
                                        bus.$emit('someEvent', {
                                            delete: 1,
                                            id: r.sheet,
                                        });
                                    }
                                   
                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
            },
            saveCard: function (event) {
                var id = "";
                var code = this.sheet;
                var text = "Do you want to update this Card? ";
                console.log("embajador " + this.ambassador.value);
                if (this.ambassador.value == undefined) {
                    id = this.idambassador;
                } else {
                    id = this.ambassador.value;
                }
                var r = this;
                $.confirm({
                    title: 'Confirm!',
                    content: text,
                    typeAnimated: true,
                    type: 'blue',
                    buttons: {
                        update: {
                            text: 'Update',
                            btnClass: 'btn-blue',
                            action: function () {
                               
                                axios.post('/membership/cardsManagement/updateSingleCard', {
                                    ambassadorid: id,
                                    code: code,
                                }).then(response => {
                                    if (response.data == "ok") {
                                        $.alert('Cards updated correctly.');
                                       

                                        bus.$emit('someEvent', {
                                            name: r.ambassador,
                                            id:id
                                        });
                                    }

                                    });
                              
                            }
                        },
                        Cancel: function () {
                        },
                    }
                  
                });

               
              
            },
     
        },
        async mounted() {
          
           
            await axios.get('/membership/cardsManagement/getAmbassadors', {

            }).then(response => {
                this.names = response.data;
            });

            },
        }

</script>
