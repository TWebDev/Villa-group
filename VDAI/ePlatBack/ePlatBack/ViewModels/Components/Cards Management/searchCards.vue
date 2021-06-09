<template>
    <div class="mb-5">
        <div>
          
            <div class="text-right mb-2">
                <div class="">
                    <button class="btn btn-primary" @click="changeComponent">
                        <i class="material-icons" style="vertical-align: middle;"> add_circle </i> Assing Cards
                    </button>
                </div>
            </div>
            <div class="container-fluid">
                <div class="row">
                    <div class="card col-sm-12">
                        <div class="card-body">
                            <h5 class="card-title">Search Cards</h5>
                            <div class="row">
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>Card Code</span>
                                    <input class="form-control" type="text" v-model="codesearch">
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>Ambassador</span>
                                    <v-select v-model="ambassadorsearch" :options="ambassadorsFilter"></v-select>
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">
                                    <span>Status</span>
                                    <v-select :options="['Active','Inactive']" v-model="statussearch"></v-select>
                                </div>
                            </div>
                            <div class="mt-2 text-right">
                                <button class="btn btn-secondary" @click="clear">Clear</button>
                                <button class="btn btn-primary" @click="search">Search</button>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <div class="container-fluid mt-5" id="details" v-if="resultsrange">
                <div class="row">
                    <div class="card col-sm-12">
                        <div class="card-body">
                            <h5 class="card-title">Result By Range</h5>
                            <div class="row">
                                <b-table @row-clicked="showResults" v-scroll-to="'#details'" :per-page="perPage" striped hover
                                         :items="items" :fields="fields" :current-page="currentPage">

                                </b-table>
                                <b-pagination size="md" :total-rows="itemscount" v-model="currentPage" :per-page="perPage">
                                </b-pagination>
                            </div>
                            <div class="row" v-if="showresultsrange">
                                <div class="card col-sm-12 shadow ">
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-sm-6 col-md-4 col-lg-4">
                                                <span class="p-3 mb-5 bg-white rounded">Ambassador</span>
                                                <v-select v-model="ambassador" :options="ambassadorsFilter"></v-select>
                                            </div>
                                            <div class="col-sm-6 col-md-4 col-lg-4">
                                                <span>Initial Card Code</span>
                                                <input class="form-control" v-model="initial" type="number" min="0">
                                            </div>
                                            <div class="col-sm-6 col-md-4 col-lg-4">
                                                <span>Final Card Code</span>
                                                <input class="form-control" v-model="final" type="number" min="0">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="mt-2 text-center col-sm-12">
                                                <span style="color: red"> <b>{{erroreditranges}}</b></span>
                                                <span v-if="loading" class="mt-2">
                                                    <h5> {{typerequest}} </h5><pulse-loader color="#31A3DD"></pulse-loader>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="col-sm-12">
                                            <div class="row">

                                                <div class="mt-2 text-right col-sm-12">
                                                    <button class="btn btn-primary" @click="saveCards">Save</button>
                                                    <button class=" btn btn-danger" @click="deleteRange"><i class="material-icons md-48" style="vertical-align: middle;"> delete</i>  </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="detailsss">
                <div class="container-fluid mt-5" v-if="showresults">
                    <div class="row">
                        <div class="card col-sm-12">
                            <div class="card-body">
                                <h5 class="card-title">Result</h5>
                                <div class="row">
                                    <b-table @row-clicked="showDetails" v-scroll-to="'#details2'" :per-page="perPage" striped hover
                                             :items="items2" :fields="fields2" :current-page="currentPage2">

                                    </b-table>
                                    <b-pagination size="md" :total-rows="itemscount2" v-model="currentPage2" :per-page="perPage">
                                    </b-pagination>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="details2">
                <div v-if="details" class="mt-5">
                    <cardsdetails :ambassador="ambassadorselected" :status="statusselected" :sheet="sheetselected" :date="dateselected"
                                  :savedby="savedbyselected" :idambassador="idambassador"></cardsdetails>

                </div>
            </div>
        </div>

    </div>
</template>
<script>
    import { bus } from './app.js';
    import axios from 'axios';
    import cardsdetails from './cardsDetails.vue';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    Vue.component('v-select', vSelect)





    export default {

        data: function () {
            return {
                ceros:0,
                showresults: false,
                ambassadorsearch: '',
                codesearch: '',
                statussearch:'',
                names: [],
                currentPage: 1,
                loading: false,
                currentPage2: 1,
                card: '',
                perPage: 5,
                rowVariant: 'danger',
                details: false,
                itemscount: 0,
                itemscount2: 0,
                ambassador: null,
                names: [],
                ambassadorselected: null,
                statusselected: null,
                sheetselected: null,
                tempkey: null,
                tempkey2: null,
                initial: '',
                final: '',
                amabssador: '',
                dateselected: '',
                savedbyselected: '',
                showresultsrange: false,
                erroreditranges: '',
                typerequest: '',
                resultsrange: true,
                fields: [
                    {
                        key: 'names',
                        label: 'Ambassador',
                        sortable: true,

                    },
                    {
                        key: 'initialcode',
                        label: 'Initial Code',
                        sortable: true
                    },
                    {
                        key: 'finalcode',
                        label: 'Final Code',
                        sortable: true,

                    }
                ],
                fields2: [
                    {
                        key: 'name',
                        label: 'Ambassador',
                        sortable: true

                    },
                    {
                        key: 'Code',
                        label: 'Card Code',
                        sortable: true,

                    },
                    {
                        key: 'status',
                        label: 'Status',
                        sortable: true,

                    },
                    {
                        key: 'date',
                        label: 'Date',
                        thClass: 'd-none',
                        tdClass: 'd-none'

                    },
                    {
                        key: 'savedby',
                        label: 'saved by',
                        thClass: 'd-none',
                        tdClass: 'd-none'
                    }
                ],
                items: [],
                items2: [],
                idambassador: '',
            }
        },
        props: ['cards'],

        methods: {
            showResults: function (key) {
                this.rowProperties(key);
                this.details = false;



                axios.post('/membership/cardsManagement/getRangeSearch', {
                    initial: this.initial,
                    final: this.final,

                }).then(response => {

                    var newarr = response.data;
                    Array.from(newarr).forEach(function (data) {
                        data.Code = data.Code.toString().padStart(7, "0");
                        
                    });
                    this.items2 = newarr;
                   
                    this.itemscount2 = this.items2.length;
                    console.log(this.itemscount2);
                });

            },
            rowProperties: function (key) {
                if (this.tempkey !== null) {
                    this.tempkey._rowVariant = "";
                }
                this.tempkey = key;
                this.showresults = true;
                this.showresultsrange = true;
                key._rowVariant = "primary";
                this.ambassador = key.names;
                this.initial = key.initialcode;
                this.final = key.finalcode;

            },
            showDetails: function (key) {
                if (this.tempkey2 !== null) {
                    this.tempkey2._rowVariant = "";
                    console.log("temp keys " + this.tempkey2);
                }

                this.tempkey2 = key;
                this.details = true;
                this.ambassadorselected = key.name;
                this.idambassador = key.userID;
                this.statusselected = key.status;
                this.sheetselected = key.Code.toString().padStart(7, "0");
                this.dateselected = key.date;
                this.savedbyselected = key.savedby;


                key._rowVariant = "primary";
            },
            changeComponent: function () {
                this.$root.currentcomponent = "newcard";
            },
            deleteRange: function () {

                var error = this.verifyNumbers();
                if (error == 1) {
                    return this;
                }
                var text = "Do you want delete the Cards Codes from " + this.initial + " to " + this.final + ".";
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
                                r.typerequest = "Deleting cards, please wait a moment...";
                                axios.post('/membership/cardsManagement/deleteRangeCard', {
                                    initial: r.initial,
                                    final: r.final,
                                }).then(response => {
                                    r.loading = false;
                                    if (response.data == "existe") {
                                        $.alert('The Card Code you wrote its already exists');
                                    }
                                    if (response.data == "ok") {
                                        $.alert('Cards were deleted correctly.');
                                        text = "";
                                        r.ambassador = "";
                                        r.initial = "";
                                        r.final = "";
                                        r.getRanges();
                                        r.showresultsrange = false;
                                        r.showresults = false;
                                        r.loading = false;
                                    }
                                    console.log(response.data);
                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
            },
            getRanges: function (nuevoarreglo) {
                console.log("el nuevo arreglo es : "+ nuevoarreglo);
                axios.get('/membership/cardsManagement/getAmbassadorsRanges', {
                   
                }).then(response => {
                    if (nuevoarreglo == undefined) {
                        this.card = response.data;
                        
                    } else {
                        this.card = nuevoarreglo;
                    }
                    // variables a utilizar en el ciclo
                    var data = []; var arreglo = []; var zcfinal = 0;
                    var tempid = 1; var zcinicial = 0; var zcid = 0;
                    var id = 0; var temp3 = 0; var suma = 0;
                    var codeinitial = 0; var codefinal = 0; var insertar = 0;
                    var ultimo = 1; var temp = 0; var counter = 0; var name = 0;
                    var newarray = 0;
                    data.push(this.card);
                    var largo = this.card.length; var cardssorted = this.cardssorted;
                    this.items = this.card; var unrecord = true;

                    // Ciclo que arma la tabla de los rangos de las card codes de cada ambassador
                    Array.from(this.cardssorted).forEach(function (data) {
                        zcinicial = data.Code;
                        if (cardssorted[1] == undefined) {
                            arreglo.push({ initialcode: data.Code.toString().padStart(7, "0"), finalcode: data.Code.toString().padStart(7, "0"), userid: data.userID, names: data.name });
                        }

                        tempid = zcid;
                        suma = temp3 + 1;
                        if (suma != data.Code) {
                            newarray = 1;
                        }
                        temp3 = data.Code;
                        
                        counter = counter + 1;
                        if (temp == 0) {
                            codeinitial = zcinicial;
                            id = data.userID;
                            name = data.name;

                        }
                        zcid = data.userID;
                        if (largo == counter) {
                            ultimo = 2;
                            zcfinal = data.Code;
                        }

                        if (tempid == zcid && ultimo == 1 && newarray == 0) {
                            unrecord = false;
                            insertar = 0;
                            zcfinal = data.Code;
                        } else {
                            if (temp == 1) {
                                insertar = 1;
                                codefinal = zcfinal;

                            }
                            if (insertar == 1) {

                                if (arreglo.length > 0) {

                                    if (data.Code == cardssorted[cardssorted.length - 1].Code && id !== cardssorted[cardssorted.length - 1].userID) {



                                        arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: cardssorted[cardssorted.length - 2].Code.toString().padStart(7, "0"), userid: id, names: name });
                                        arreglo.push({ initialcode: data.Code.toString().padStart(7, "0"), finalcode: data.Code.toString().padStart(7, "0"), userid: data.userID, names: data.name });

                                    }
                                    else if (data.Code == cardssorted[cardssorted.length - 1].Code && id == cardssorted[cardssorted.length - 1].userID) {

                                        if (data.Code == cardssorted[cardssorted.length - 2].Code + 1) {
                                            arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: data.Code.toString().padStart(7, "0"), userid: data.userID, names: data.name });

                                        }
                                        else {
                                            arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: cardssorted[cardssorted.length - 2].Code.toString().padStart(7, "0"), userid: id, names: name });
                                            arreglo.push({ initialcode: data.Code.toString().padStart(7, "0"), finalcode: data.Code.toString().padStart(7, "0"), userid: data.userID, names: data.name });
                                        }
                                    }
                                    else {

                                        if (arreglo[arreglo.length - 1].finalcode == codefinal) {
                                            arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: codeinitial.toString().padStart(7, "0"), userid: id, names: name });
                                            zcfinal = codeinitial;

                                        } else {


                                            if (codefinal == 0) {
                                                codefinal = codeinitial;
                                            }
                                            arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: codefinal.toString().padStart(7, "0"), userid: id, names: name });
                                        }
                                    }
                                }
                                else {


                                        if (cardssorted[1].Code == undefined) {
                                            arreglo.push({ initialcode: data.Code.toString().padStart(7, "0"), finalcode: data.Code.toString().padStart(7, "0"), userid: data.userID, names: data.name });

                                        }

                                    if (cardssorted[0].Code !== cardssorted[1].Code - 1) {
                                        arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: codeinitial.toString().padStart(7, "0"), userid: id, names: name });

                                        if (cardssorted[counter] == undefined) {
                                            arreglo.push({ initialcode: data.Code.toString().padStart(7, "0"), finalcode: data.Code.toString().padStart(7, "0"), userid: data.userID, names: data.name });
                                        }


                                        } else {
                                            if (codefinal == 0) {

                                                codefinal = codeinitial;
                                            }

                                            if (cardssorted[counter] == undefined && cardssorted[counter - 1].userID !==id ) {
                                                
                                                arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: cardssorted[counter - 2].Code.toString().padStart(7, "0"), userid: id, names: name });
                                                arreglo.push({ initialcode: data.Code.toString().padStart(7, "0"), finalcode: data.Code.toString().padStart(7, "0"), userid: data.userID, names: data.name });
                                            } else {


                                                console.log("control  " + cardssorted[counter - 2].Code + "   " + codefinal);
                                                if (cardssorted[counter - 2].Code + 1 == codefinal) {
                                                    arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: codefinal.toString().padStart(7, "0"), userid: id, names: name });
                                                }
                                                else {
                                                    if (cardssorted[counter - 2].userID !== id) {
                                                        arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: cardssorted[counter - 2].Code.toString().padStart(7, "0"), userid: id, names: name });
                                                        arreglo.push({ initialcode: data.Code.toString().padStart(7, "0"), finalcode: data.Code.toString().padStart(7, "0"), userid: data.userID, names: data.name });
                                                    }
                                                    arreglo.push({ initialcode: codeinitial.toString().padStart(7, "0"), finalcode: cardssorted[counter - 2].Code.toString().padStart(7, "0"), userid: id, names: name });
                                                }
                                            }

                                        }
                                   
                                }
                                codeinitial = data.Code;
                                id = data.userID;
                                name = data.name;
                            }
                        }
                        newarray = 0;
                        temp = 1;
                    });
                    // fin de ciclo que arma la tabla de los rangos de las card codes de cada ambassador
                    this.items = arreglo;
                    this.itemscount = arreglo.length;
                });
            },
            saveCards: function () {
                var error = this.verifyNumbers();
                if (error == 1) {
                    return this;
                }
                var id = "";
                var initial = this.initial;
                var final = this.final;
                var ambassadorname = "";
                if (this.ambassador.value == undefined) {
                    id = this.tempkey.userid;
                    ambassadorname = this.tempkey.names;
                } else {
                    id = this.ambassador.value;
                    ambassadorname = this.ambassador.label;

                }
                var text = "Do you want to update Cards from " + initial + " to " + final + " to " + ambassadorname + ".";
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
                                r.typerequest = "Updating cards, please wait a moment...";
                                r.loading = true;
                                axios.post('/membership/cardsManagement/updateRangeCard', {
                                    ambassadorid: id,
                                    initial: initial,
                                    final: final
                                }).then(response => {
                                    if (response.data == "ok") {
                                        $.alert('Cards updated correctly.');
                                        r.getRanges();
                                        r.showresults = false;
                                        r.loading = false;

                                    }

                                });

                            }
                        },
                        Cancel: function () {
                        },
                    }

                });




            },
            verifyNumbers: function () {

                if (this.tempkey.initialcode > this.initial || this.tempkey.finalcode < this.final) {
                    this.erroreditranges = "You can only edit or delete between " + this.tempkey.initialcode + " and " + this.tempkey.finalcode + ".";
                    return 1;
                } else {
                    this.erroreditranges = "";
                }
                if (this.ambassador == null || this.initial == "" || this.final == "") {
                    this.erroreditranges = "Please, fill out the fields correctly.";
                    return 1;
                }
                if (this.initial < 0 || this.final < 0) {
                    this.erroreditranges = "The codes must be greater than 0.";
                    return 1;
                }
                if (this.initial > this.final) {
                    this.erroreditranges = "The Initial Code can't be greater than Final Code.";
                    return 1;
                }
            },
            searchByAmbassadors: function () {


                axios.post('/membership/cardsManagement/getAmbassadorsSearch', {
                    ambassadorid: this.ambassadorsearch.value,
                   
                }).then(response => {
                    this.resultsrange = true,
                        this.showresults = false;
                    this.showresultsrange = false;
                    this.details = false;

                    this.getRanges(response.data);
                    console.log(response.data);
                });

            },
            search: function () {
                if (this.codesearch !="") {
                    this.statussearch = "";
                    this.ambassadorsearch = "";
                }
                if (this.statussearch == null) {
                    this.statussearch = "";
                }
                console.log(" estatus " + this.statussearch);
                if (this.codesearch == "" && this.statussearch == "" && this.ambassadorsearch !== "") {
                    this.searchByAmbassadors();
                    return this;
                }
                if (this.codesearch !== "" || this.statussearch !== "") {
                    this.searchBy();
                    return this;
                }
            },
            searchBy: function () {
                var ambassadorid = "";
                var search = 0;
                if (this.ambassadorsearch == null) {
                     ambassadorid = this.ambassadorsearch;
                    console.log("es nulo ");
                } else {
                     ambassadorid = this.ambassadorsearch.value;
                    console.log("no es nulo ");

                }
               
                console.log(" ambassador id: " + ambassadorid);
                axios.post('/membership/cardsManagement/getAmbassadorsSearchBy', {
                    ambassadorid: ambassadorid,
                    code: this.codesearch,
                    status: this.statussearch,
                    search: search,

                }).then(response => {
                    // this.getRanges(response.data);
                    this.resultsrange = false,
                    this.showresults = true;
                    this.details = false;
                    this.itemscount2 = response.data.length;
                    var newarr = response.data;
                    Array.from(newarr).forEach(function (data) {
                        data.Code = data.Code.toString().padStart(7, "0");

                    });
                    this.items2 = newarr;



                    console.log(" el response es : "+response.data);
                });
            },
            clear: function () {
                this.resultsrange = true;
                this.showresults = false;
                this.details = false;
                this.showresultsrange = false;
                this.codesearch = "";
                this.statussearch = "";
                this.ambassadorsearch = "";
                this.getRanges();
            },
        },
        components: {

            'cardsdetails': cardsdetails,
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
            cardssorted: function () {
                return this.card.slice(0).sort((a, b) => a.Code - b.Code)
            },
        },
        async mounted() {

            this.getRanges();
            await axios.get('/membership/cardsManagement/getAmbassadors', {
            }).then(response => {
              
               this.names = response.data;                         
            });
            //bus en comunicacion con el componente de cardsDetails
            bus.$on('someEvent', obj => {
                if (obj.delete == 1) {
                    this.items2 = this.items2.filter(function (dato) {
                        if (dato.Code == obj.id) {

                            return false;
                        } else {
                            return true;
                        }
                    });
                    this.details = false;
                    this.showresultsrange = false;
                    this.getRanges();

                    return this;
                }
                if (obj.name.value == undefined) {
                    this.tempkey2.name = obj.name;
                    this.tempkey2.userID = obj.id;
                    this.idambassador = obj.id;
                    this.ambassadorselected = obj.name;

                } else {
                    this.tempkey2.name = obj.name.label;
                    this.tempkey2.userID = obj.name.value;
                    this.idambassador = obj.name.value;
                    this.ambassadorselected = obj.name.label;

                }
                this.getRanges();
                this.showresultsrange = false;
                console.log(obj);
            });
       
            var numero = 3;
            this.ceros = numero.toString().padStart(7, "0");
            console.log("hola"); 


        },
    }

</script>
