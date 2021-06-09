
<template>
    <div id="TerminalsModuleDrpDown" class="mr-3 flex-md-nowrap">
        <button id="btnDropTerminals" class="btn btn-secondary btn-block dropdown-toggle" type="button"data-toggle="collapse" data-target="#ddlSelectedTerminals" aria-expanded="false" aria-controls="ddlSelectedTerminals">
             {{ Session.SelectedTerminalsNames }}
        </button>
        <div id="ddlSelectedTerminals" class="collapse position-absolute bg-light" style="font-size:.9rem">
            <div class="terminals-list text-left">
                <span v-for="t in Session.UserTerminals" v-bind:class="{ 'd-none' : !t.Visible }">
                    <label class="mb-0">
                        <input type="checkbox" v-bind:key="`terminal-${t.TerminalID}`" v-bind:value="t.TerminalID" v-model="Session.TerminalsIDsArr" v-on:change="selectedTerminals(t.TerminalID, t.RoleID, t.WorkGroupID)" />
                        {{ t.Terminal }}
                    </label><br />
                    <small class="text-muted ml-3 mt-0 mb-1 d-block">as {{ t.Role }} in {{ t.WorkGroup }}</small>
                </span>
            </div>
            <input type="button" class="btn btn-secondary mx-auto d-block" style="width:47px;" value="OK" v-on:click="updateSelectedTerminals" />
        </div>
    </div>
</template>
<script>
    import { mapState, mapGetters } from 'vuex';
    export default ({
        computed: {
            ...mapState({ Session: 'session' }),
            ...mapGetters({ terminalsSelected:'terminalsSelected' })
        },
        watch: {
            terminalsSelected() {
                this.loadTerminalDependentList();
            }
        },
        methods: {
            selectedTerminals(terminalID, roleID, workgroupID) {
                let self = this;
                var terminalsArrys = self.$store.state.session.TerminalsIDsArr;
                if (terminalsArrys != 0) {
                    if (self.$store.state.session.TerminalsIDsArr.indexOf(terminalID) != -1) {
                        self.$store.dispatch("addSelectedTerminal");
                    } else {
                        self.$store.dispatch("deleteSelectedTerminal");
                    }
                }
                else {
                    self.$store.dispatch("showAllTerminals");
                }
            },
            updateSelectedTerminals() {
                    let self = this;
                    var terminalsArrys = self.$store.state.session.TerminalsIDsArr;
                    if (terminalsArrys.length > 0) {
                        self.$store.dispatch("updateSelectedTerminals");
                        $('#btnDropTerminals').dropdown('toggle');
                    }
                    else {
                        alert('You need to select 1 terminal at least');
                    }
            },
            loadTerminalDependentList() {
                let self = this;
                $('.terminal-dependent-list[multiple="multiple"]').multiselect('dataprovider', self.Session.TerminalsList);
                $('body').trigger('selectedTerminalChanged');
            }
        },
        mounted() {
          //  this.loadTerminalDependentList();
        }
    })
</script>
<style>

</style>