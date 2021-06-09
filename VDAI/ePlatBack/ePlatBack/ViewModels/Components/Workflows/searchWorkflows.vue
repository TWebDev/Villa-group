
<template>
    <div id="searchWorkflowContainer">
        <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pb-2 border-bottom mb-4">
            <h3>Search</h3>
        </div>
        <div class="row mb-3">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-12">
                                <button id="btnNewWorkflow" class="btn btn-primary wf-btn float-right mb-2 btn-action" data-action="new" @click="newWorkflow">new</button>
                            </div>
                            <div class="form-group col-lg-6">
                                <label for="searchTerminal">Terminal: </label>
                                <select v-model="Search.TerminalID" class="form-control">
                                    <option v-for="(option, key) in Terminals" :value="option.TerminalID">{{option.Terminal}}</option>
                                </select>
                            </div>
                            <div class="form-group col-lg-6">
                                <label for="searchName">Name: </label>
                                <input type="text" class="form-control" placeholder="name" v-model="Search.Name" />
                            </div>
                        </div>
                        <div>
                            <!--table container-->
                            <table id="tblWorkflows" class="table">
                                <thead class="thead-dark">
                                    <tr>
                                        <th>ID</th>
                                        <th>Name</th>
                                        <th>Terminal</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr :id="row.WorkflowID" v-for="(row,key) in Workflows" @click="selectWorkflow(key)" :aria-rowindex="key">
                                        <td>{{row.WorkflowID}}</td>
                                        <td>{{row.Name}}</td>
                                        <td>{{row.Terminal}}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="card-footer">
                        <button type="button" class="btn btn-secondary wf-btn float-right" @click="search">Search</button>
                    </div>
                </div>
            </div>
        </div>
        <div>
            <!--<workflows-management :new-wf="NewWf" :selected-row="SelectedRow" :terminals="Terminals" v-on:newworkflow="newworkflow"></workflows-management>-->
            <workflows-management :new-wf="NewWf" :selected-row="SelectedRow" :terminals="Terminals"></workflows-management>
        </div>
    </div>
</template>
<script>
    import axios from 'axios';

    export default {
        mixins: [ePlatUtils],
        data: function () {
            return {
                Shared: ePlatStore,//Obtengo la herencia de los campos compartidos en todos los módulos
                Terminals: [],
                Workflows: [],
                NewWf: 0,
                Search: {
                    TerminalID: null,
                    Name: ''
                },
                SelectedRow: null
            }
        },
        methods: {
            newWorkflow: function () {
                self.NewWf++;
                $('#tblWorkflows tbody tr').removeClass('selected');
            },
            search: function () {
                let self = this;
                self.SelectedRow = {
                    System: null,
                    WorkflowID: null,
                    TerminalID: null,
                    DestinationID: null,
                    PlaceID: null,
                    SenderAddress: null,
                    SenderPassword: null,
                    ReplyTo: null,
                    Name: '',
                    WorkflowJson: []
                };
                self.Workflows = [];
                axios({
                    method: 'post',
                    url: '/Workflows/SearchWorkflows',
                    data: { model: self.Search }
                }).then(response => { this.Workflows = response.data });
            },
            selectWorkflow: function (key) {
                let self = this;
                self.SelectedRow = self.Workflows[key];
                $('#tblWorkflows tr').removeClass('selected');
                $('#tblWorkflows tr[aria-rowindex="' + key + '"]').addClass('selected');
                this.UI().scrollTo('chartContainer');
            }
        },
        mounted: function () {
            this.Session().getSessionDetails();
        }
    }
</script>