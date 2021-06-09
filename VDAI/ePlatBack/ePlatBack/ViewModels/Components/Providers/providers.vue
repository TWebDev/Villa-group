<template>

    <div>
            <div class="row mb-5">
                <div class="text-left col-sm-8">
                   <h2 style="position: relative">Providers Management</h2>
                </div>
                <div class="col-sm-4 text-right">
                    <button class="btn btn-primary" @click="addProvider">
                        <i class="material-icons" style="vertical-align: middle;">
                            add_circle
                        </i>Add Provider
                    </button>
                </div>

            </div>
            <div v-if="loading" class="text-center">
                <pulse-loader color="#31A3DD"></pulse-loader>
            </div>
            <div v-else>
                <div class="row">
                    <div class="col-sm-12">
                        <div class="card">
                            <div class="card-body">
                                <div class="row">
                                    <b-table v-scroll-to="'#providerDetails'" class="hand" @row-clicked="showDetailsProvider" :per-page="table.perPage" striped hover
                                             :items="providers" :fields="providersFields" :current-page="table.currentPage">
                                        <template slot="dateSaved" slot-scope="row">
                                            <span>
                                                {{cleanDate(row.item.dateSaved)}}
                                            </span>
                                        </template>
                                    </b-table>
                                    <b-pagination size="md" :total-rows="providers.length" v-model="table.currentPage" :per-page="table.perPage">
                                    </b-pagination>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="newProvider" class="card">
                    <div class="card-body">
                        <div v-if="newProviderShow">
                            <providerdetails :key="componentKey" ></providerdetails>
                        </div>
                    </div>
                </div>
                <div id="providerDetails" class="card">
                    <div class="card-body">
                        <div v-if="providerSelected.show">
                            <providerdetails :key="componentKey" :providerID="providerSelected.providerID"></providerdetails>
                        </div>
                    </div>
                </div>
            </div>          
     </div>
</template>


<script>
    import { bus } from './app.js';
    import providerdetails from './providertemplate.vue';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'

    export default {
        props: ['cards'],
        data: function () {
            return {
                newProviderShow: false,
                componentKey: 0,
                loading: false,

                providerSelected: {
                    show: false,
                    key: null,
                    providerID: null,

                },
                table: {
                    currentPage: 1,
                    perPage: 5,
                },
                hotelid: null,
                details: false,
                orient: '',
                providers: [],
                providersFields: [
                    {
                        key: 'name',
                        label: 'Provider',
                        sortable: true
                    },
                    {
                        key: 'destination',
                        label: 'Destination',
                        sortable: true,
                    },
                    {
                        key: 'dateSaved',
                        label: 'Date Created',
                        sortable: true,
                    },
                ],
                tempkey: null,
            }
        },
        components: {

            'PulseLoader': PulseLoader,
            'providerdetails': providerdetails

        },
        methods: {
            addProvider: function () {
                this.componentKey = this.componentKey + 1;
                this.providerSelected.show = false;
                if (this.providerSelected.key !== null) {
                    this.providerSelected.key._rowVariant = "";
                }
                this.newProviderShow = true;
            },
            cleanDate: function (fecha) {
                if (fecha !== undefined && fecha !== null) {
                    fecha = fecha.split("/Date(").join("");
                    fecha = fecha.split(")/").join("");
                    var dia = new Date(parseInt(fecha)).getDate();
                    var mes = new Date(parseInt(fecha)).getMonth() + 1;
                    var año = new Date(parseInt(fecha)).getFullYear();
                    return dia.toString().padStart(2, "0") + "-" + mes.toString().padStart(2, "0") + "-" + año.toString().padStart(2, "0");
                } else {
                    return "";
                }
            },
            showDetailsProvider: function (key) {
                this.newProviderShow = false;
                this.componentKey = this.componentKey + 1;
                this.providerSelected.show = false;
                if (this.providerSelected.key !== null) {
                    this.providerSelected.key._rowVariant = "";
                }
                key._rowVariant = "primary";
                this.providerSelected.key = key;
                this.providerSelected.providerID = key.providerID;
                this.providerSelected.show = true;
                this.$forceUpdate();
            },
            getProviders: function () {
                this.loading = true;
                axios.post('/Content/management/GetProviders', {
                    terminalid: 62,
                }).then(response => {
                    this.loading = false;
                    this.providers = response.data;
                });
            },
        },

        mounted() {
            this.getProviders();
            bus.$on('updateProviders', obj => {
                this.providerSelected.show = false;
                this.getProviders();
            });
        }
    }
</script>
<style>
    .hand {
        cursor: pointer;
    }
</style>
