<template>
    <div id="workflowsManagementContainer">
        <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pb-2 border-bottom mb-4">
            <h3>Workflow Info</h3>
        </div>

        <!-- Modal Upload Picture -->
        <div class="modal fade" id="uploadFileModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <!--<h5 class="modal-title" id="exampleModalLabel">Upload Picture for</h5>-->
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body text-center">
                        <div id="fine-uploader-basic" class="btn btn-success">
                            <i class="material-icons">file_upload</i>Browse
                        </div>
                        <div id="messages"></div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" data-dismiss="modal">OK</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">Workflow Panel</div>
                    <div id="workflowPanel" class="card-body row">
                        <div class="col-12">
                            <div class="form-group col">
                                <h6 class="full-width text-left">Receiver(s) Configuration</h6>
                                <label for="infoDifussionWays">Difussion Universe</label>
                                <select id="infoDifussionWays" v-model="Body.DifussionWay" class="form-control" @change="loadFlowDifussion">
                                    <option v-for="(option, key) in DifussionWays" :value="option.Value">{{option.Text}}</option>
                                </select>
                            </div>
                            <!--<div class=" form-group col-6">-->
                            <!--<label v-if="Body.DifussionWay == 1">SQL Query</label>-->
                            <!--<label v-if="Body.DifussionWay == 1">Rule</label>
                            <label v-if="Body.DifussionWay == 2">Attach File</label>-->
                            <!--<label v-if="Body.DifussionWay == 3">Open Step Modal</label>-->
                            <div class="input-group col" v-if="Body.DifussionWay == 1">
                                <!--<div class="input-group-prepend">
                                    <button type="button" class="btn btn-outline-secondary">
                                        <i class="material-icons">edit</i>
                                    </button>
                                </div>
                                <input type="text" class="form-control" v-model="Body.DifussionWayDetails" />-->
                                <h6 class="full-width text-left">Configure Settings</h6>
                                <div class="form-group col">
                                    <label for="infoSystem">System: </label>
                                    <select id="infoSystem" class="form-control" v-model="Body.System" @change="selectSystem">
                                        <option v-for="(option, key) in Systems" :value="option.Value">{{option.Text}}</option>
                                    </select>
                                </div>
                                <div class="form-group col">
                                    <label for="infoName">Name: </label>
                                    <input type="text" id="infoName" class="form-control" placeholder="name" v-model="Body.Name" />
                                </div>
                                <div class="form-group col">
                                    <input type="hidden" class="form-control" v-model="Body.WorkflowID" />
                                    <label for="infoTerminal">Terminal: </label>
                                    <select id="infoTerminal" class="form-control" v-model="Body.TerminalID" @change="selectTerminal">
                                        <option v-for="(option, key) in Terminals" :value="option.Value">{{option.Text}}</option>
                                    </select>
                                </div>
                                <div class="form-group col">
                                    <label for="infoDestination">Destination: </label>
                                    <select id="infoDestination" v-model="Body.DestinationID" class="form-control" @change="selectDestination">
                                        <option v-for="(option, key) in Destinations" :value="option.Value">{{option.Text}}</option>
                                    </select>
                                </div>
                                <div class="form-group col">
                                    <label for="infoHotel">Hotel: </label>
                                    <select id="infoHotel" v-model="Body.PlaceID" class="form-control">
                                        <option v-for="(option, key) in Hotels" :value="option.Value">{{option.Text}}</option>
                                    </select>
                                </div>
                                <div class="form-group col">
                                    <label for="infoSenderAddress">Address</label>
                                    <input type="text" id="infoSenderAddress" class="form-control" placeholder="Sender Address" v-model="Body.SenderAddress" />
                                </div>
                                <div class="form-group col">
                                    <label for="infoSenderPassword">Password</label>
                                    <input type="text" id="infoSenderPassword" class="form-control" placeholder="Sender Password" v-model="Body.SenderPassword" />
                                </div>
                                <div class="form-group col">
                                    <label for="infoReplyTo">Reply To</label>
                                    <input type="text" id="infoReplyTo" class="form-control" placeholder="comma(,) separated" v-model="Body.ReplyTo" />
                                </div>
                            </div>

                            <div class="input-group col" v-if="Body.DifussionWay == 2" data-toggle="modal" data-target="#uploadFileModal" @click="showUploadModal()">
                                <div class="input-group-prepend">
                                    <button type="button" class="btn btn-outline-secondary">
                                        <!--<i class="material-icons">attach_file</i>-->
                                        <i class="material-icons">file_upload</i>
                                    </button>
                                </div>
                                <input type="text" class="form-control" v-model="Body.DifussionWayDetails" disabled />
                            </div>
                            <!--<div class="input-group" v-if="Body.DifussionWay == 3">
                                <div class="input-group-prepend">
                                    <button class="btn btn-outline-secondary" @click="newWorkflow">New</button>
                                </div>
                            </div>-->
                            <!--</div>-->
                        </div>
                        <div class="form-group col-12">
                            <div class="col">
                                <h6 class="full-width text-left">Wokflow Structure</h6>
                                <div class="form-group col">
                                    <div id="chartContainer" class="text-center"></div>
                                </div>
                                <div class="input-group-prepend">
                                    <button class="btn btn-outline-primary" @click="newWorkflow">New Flow</button>
                                </div>
                            </div>
                        </div>

                        <!----------------->
                        <!--<h6 v-if="Body.DifussionWay == 3" class="full-width text-left">Wokflow Structure</h6>
                        <div v-if="Body.DifussionWay == 3" class="form-group col-12">
                            <div id="chartContainer" class="text-center"></div>
                        </div>-->
                        <!--<div class="col-12">
                            <h6 class="full-width text-left">Wokflow Structure</h6>
                            <div class="form-group col-12">
                                <div id="chartContainer" class="text-center"></div>
                            </div>
                        </div>-->
                    </div>
                    <div class="card-footer">
                        <button class="btn btn-primary wf-btn float-right" @click="saveWorkflow">save</button>
                    </div>
                </div>
            </div>
        </div>
        <new-step :first-node="FirstNode" :action-type="Type" v-on:renderstep="getStep" :emails="Emails" :step="SelectedStep"></new-step>
    </div>
</template>
<script>
    import axios from 'axios';
    var chart;
    var chartTable;

    export default {
        //props: ["newWf", "selectedRow", "terminals"],
        props: ["newWf", "selectedRow"],
        mixins: [ePlatUtils],
        data: function () {
            return {
                Systems: [],
                Shared: ePlatStore,//Obtengo la herencia de los campos compartidos en todos los módulos
                Terminals: [],
                Destinations: [],
                Hotels: [],
                DifussionWays: [],
                Counter: 0,
                FirstNode: null,
                SelectedNode: null,
                SelectedStep: {
                    Action: null,
                    Step: '',
                    Value: '',
                    Type: null,
                    Email: '',
                    DelayDays: '',
                    DelayHours: '',
                    LinkTo: '',
                    ParentStep: ''
                },
                Type: '',
                Emails: [],
                Body: {
                    WorkflowID: null,
                    System: null,
                    TerminalID: null,
                    DestinationID: null,
                    PlaceID: null,
                    SenderAddress: null,
                    SenderPassword: null,
                    ReplyTo: null,
                    DifussionWay: null,
                    DifussionWayDetails: '',
                    Name: '',
                    WorkflowJson: []
                }
            }
        },
        methods: {
            selectNode: function () {
                let self = this;
                var clickedNode = chart.getSelection()[0].row;
                self.FirstNode = false;
                self.SelectedNode = clickedNode;
                self.SelectedStep = self.Body.WorkflowJson[clickedNode];
                $('#newStepModal').modal('show');
            },
            getStep: function (step) {
                let self = this;

                if (step.Action == 'Save') {
                    self.createStep(step);
                }
                else if (step.Action == 'Update') {
                    self.updateStep(step);
                }
                else if (step.Action == 'Delete') {
                    self.removeStep(step.Step);
                }

                self.clearSelectedStep();
            },
            createStep: function (step) {
                let self = this;
                step.ParentStep = self.FirstNode == false ? chartTable.getValue(self.SelectedNode, 0) : '';//new node. parent is the node clicked

                var currentIndex = self.Body.WorkflowJson.push(step) - 1;
                self.Body.WorkflowJson[currentIndex].Step = self.Counter;
                self.Body.WorkflowJson[currentIndex].Value = step.Type + self.Counter.toString();

                var chartItem = {};
                chartItem.v = self.Body.WorkflowJson[currentIndex].Value;
                chartItem.f = '<div class="wf-step">'
                    + '<strong style = "text-transform:capitalize;"> ' + step.Type + '</strong><br />' + self.getEmailName(step.Email) + ' ' + self.getDelay(step.DelayDays, step.DelayHours) + '</div>';
                var chartSet = [chartItem, step.ParentStep, ''];
                chartTable.addRow(chartSet);
                if (self.FirstNode == true) {
                    self.drawChart();
                }
                else {
                    chart.draw(chartTable, { allowHtml: true });
                }
                self.Counter++;
                chart.setSelection(null);//prevents error when selecting same node twice
            },
            updateStep: function (step) {
                let self = this;

                self.Body.WorkflowJson[self.SelectedNode] = step;
                chartTable.setValue(self.SelectedNode, 0, step.Value);
                chartTable.setFormattedValue(self.SelectedNode, 0, '<div class="wf-step">'
                    + '<strong style = "text-transform:capitalize;">' + step.Type + '</strong><br />' + self.getEmailName(step.Email) + ' ' + self.getDelay(step.DelayDays, step.DelayHours) + '</div>');
                chart.draw(chartTable, { allowHtml: true });
                chart.setSelection(null);//prevents error when selecting same node twice
            },
            removeStep: function (key) {
                let self = this;

                var node = _.clone(self.Body.WorkflowJson[key]);
                var children = chart.getChildrenIndexes(self.SelectedNode);

                $.each(children, function (i, item) {
                    self.Body.WorkflowJson[item].ParentStep = node.ParentStep;//actualiza el padre de los hijos al padre del componente borrado
                    chartTable.setValue(item, 1, chartTable.getValue(item, 0));
                });
                self.Body.WorkflowJson.splice(key, 1);

                //para que no haya futuros nodos huérfanos debido a asignción basada en índice
                self.drawChart();
            },
            clearSelectedStep: function () {
                this.SelectedStep = {
                    Action: '',
                    DelayDays: '',
                    DelayHours: '',
                    LinkTo: '',
                    Email: '',
                    ParentStep: '',
                    Step: '',
                    Type: null,
                    Value: ''
                }
            },
            getEmailName: function (id) {
                let self = this;
                if (id != null && id != '')
                    return _.filter(self.Emails, function (e) { return e.Value == id })[0].Text;
                return '';
            },
            getDelay: function (d, h) {
                var val = '';
                if (d != null && d != '')
                    val += d + ' Day(s)';
                if (h != null && h != '')
                    val += ' ' + h + ' Hour(s)';
                return val;
            },
            drawChart: function () {
                let self = this;
                var workflow = self.Body.WorkflowJson;
                chart = new google.visualization.OrgChart(document.getElementById('chartContainer'));
                var chartOptions = {
                    allowHtml: true
                };
                chartTable = new google.visualization.DataTable();
                var flowChart = [];
                chartTable.addColumn('string', 'Name');
                chartTable.addColumn('string', 'Manager');
                chartTable.addColumn('string', 'Tooltip');

                $.each(workflow, function (e, i) {
                    var parent = i.ParentStep;
                    var chartItem = {};
                    chartItem.v = i.Value;
                    chartItem.f = ''
                        + '<div class="wf-step">'
                        + '<strong style = "text-transform:capitalize;"> ' + i.Type + '</strong><br />' + self.getEmailName(i.Email) + ' ' + self.getDelay(i.DelayDays, i.DelayHours) + '</div>';

                    var chartSet = [];
                    chartSet.push(chartItem, parent, '');
                    flowChart.push(chartSet);
                });

                chartTable.addRows(flowChart);
                chart.draw(chartTable, chartOptions);

                //google
                google.visualization.events.addListener(chart, 'select', self.selectNode);
            },
            saveWorkflow: function () {
                let self = this;
                //******

                //******

                axios({
                    headers: {
                        //    'Content-Type': 'multipart/form-data'
                        'enctype': 'multipart/form-data'
                    },
                    method: 'post',
                    url: '/Workflows/SaveWorkflow',
                    data: { model: self.Body }
                    //data: { model: JSON.stringify(self.Body) }
                    //data: { id: self.Body.WorkflowID, name: self.Body.WorkflowName, workflow: JSON.stringify(self.Body.Workflow) }
                    //data: { workflow: JSON.stringify(self.StepTree), name: self.WorkflowName }
                })
                    .then(function (response) {
                        $.alert({
                            title: response.data.ResponseMessage,
                            content: response.data.ResponseMessage + '<br />' + response.data.ExceptionMessage,
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: response.data.ResponseType == 1 ? 'green' : response.data.ResponseType == -1 ? 'red' : 'orange'
                        });
                    });
            },
            orderChildren: function () {
                let self = this;
                var _item = {
                    Step: '',
                    StepDetails: '',
                    Children: []
                };
                var clone = _.clone(self.Body.WorkflowJson);

                _item.Step = clone[0].Value;
                _item.StepDetails = clone[0].Email != '' ? clone[0].Email : clone[0].DelayDays != '' ? clone[0].DelayDays + ',' + clone[0].DelayHours : '';
                _item.Children = b(clone[0]);
                self.StepTree = _item;

                function b(child) {
                    var cosa = [];
                    var item = {
                        Step: '',
                        StepDetails: '',
                        Children: []
                    };
                    var _children = _.filter(clone, function (o) { return o.ParentStep == child.Value; });

                    $.each(_children, function (index, i) {
                        item.Step = i.Value;
                        item.StepDetails = i.Email != '' ? i.Email : i.DelayDays != '' ? i.DelayDays + ',' + i.DelayHours : '',
                            item.Children = b(i);
                        cosa.push(_.clone(item));
                    });
                    return cosa;
                }
            },
            newWorkflow: function (firstStep) {
                let self = this;
                self.Type = 'start';
                self.Counter = 0;
                self.FirstNode = true;
                self.Body.System = null;
                self.Body.WorkflowID = null;
                self.Body.TerminalID = null;
                self.Body.DestinationID = null;
                self.Body.PlaceID = null;
                self.Body.SenderAddress = null;
                self.Body.SenderPassword = null;
                self.Body.ReplyTo = null;
                self.Body.Name = '';
                self.Body.WorkflowJson = [];//revisar esta linea. a ver si el boton new borra el esquema
                $('#newStepModal').modal('show');
            },
            getLists: function () {
                let self = this;
                this.UI().loadDependentFields('/Workflows/GetWorkflowsDependentFields', false, function () {
                    self.Systems = self.Shared.State.DependentFields.Fields.filter(function (item) {
                        return item.Field == "System"
                    })[0].Values;
                });

                //self.DifussionWays = [{ Value: '0', Text: 'Select One' }, { Value: '1', Text: 'Rule' }, { Value: '2', Text: 'File' }, { Value: '3', Text: 'Flow' }];
                self.DifussionWays = [{ Value: '0', Text: 'Select One' }, { Value: '1', Text: 'Rule' }, { Value: '2', Text: 'File' }, { Value: '3', Text: 'List' }];
            },
            selectSystem: function () {
                let self = this;
                self.Terminals = self.Shared.State.DependentFields.Fields.filter(function (item) {
                    return item.Field == "Terminals"
                })[0].Values.filter(function (item) { return item.ParentValue == self.Body.System });
            },
            selectTerminal: function () {
                let self = this;
                self.Destinations = self.Shared.State.DependentFields.Fields.filter(function (item) {
                    return item.Field == "Destinations"
                })[0].Values.filter(function (item) { return item.ParentValue == self.Body.TerminalID });
            },
            selectDestination: function () {
                let self = this;
                self.Hotels = self.Shared.State.DependentFields.Fields.filter(function (item) {
                    return item.Field == "Hotels"
                })[0].Values.filter(function (item) { return item.GrandParentValue == self.Body.TerminalID && item.ParentValue == self.Body.DestinationID });
            },
            showUploadModal: function () {
                let self = this;
                var $fub = $('#fine-uploader-basic');
                var $messages = $('#messages');

                let uploader = new qq.FineUploaderBasic({
                    button: $fub[0],
                    request: {
                        endpoint: '/Workflows/UploadFile',
                        params: {
                            //additional params to save the file
                            model: self.Body
                        }
                    },
                    validation: {
                        allowedExtensions: ['doc', 'docx', 'xls', 'xlsx', 'txt'],
                        sizeLimit: 5242880
                    },
                    callbacks: {
                        onSubmit: function (id, fileName) {
                            $messages.append('<div id="file-' + id + '" class="alert" style="margin: 20px 0 0"></div><div class="progress-' + id + '"><div class="progress-bar" role="progressbar" style="width: 0%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div></div>');
                        },
                        onUpload: function (id, fileName) {
                            $('#file-' + id).addClass('alert-info')
                                .html('<img src="/images/loading.gif" alt="Initializing. Please hold." style="width:16px;"> ' +
                                    'Initializing ' +
                                    '“' + fileName + '”');
                        },
                        onProgress: function (id, fileName, loaded, total) {
                            if (loaded < total) {
                                progress = Math.round(loaded / total * 100) + '% of ' + Math.round(total / 1024) + ' kB';
                                $('#file-' + id).removeClass('alert-info')
                                    .html('<img src="/images/loading.gif" alt="In progress. Please hold." style="width:16px;"> ' +
                                        'Uploading ' +
                                        '“' + fileName + '” ' +
                                        progress);
                                $('#progress-' + id).css('width', progress + '%');
                            } else {
                                $('#file-' + id).addClass('alert-info')
                                    .html('<img src="/images/loading.gif" alt="Saving. Please hold." style="width:16px;"> ' +
                                        'Saving ' +
                                        '“' + fileName + '”');

                            }
                        },
                        onComplete: function (id, fileName, responseJSON) {
                            if (responseJSON.success) {
                                $('#file-' + id).removeClass('alert-info')
                                    .addClass('alert-success')
                                    .html('<i class="icon-ok"></i> ' +
                                        'Successfully saved ' +
                                        '“' + fileName + '”' +
                                        '<br><img src="' + responseJSON.path.path + '" alt="' + fileName + '" class="img-fluid">');
                                //integrar respuesta a la colección
                                //$.each(self.Hotels, function (i, h) {
                                //    if (h.SpiHotelID == responseJSON.response.ObjectID.SpiHotelID) {
                                //        h.Picture = responseJSON.path.path;
                                //        h.HotelPickUpID = responseJSON.response.ObjectID.HotelPickUpID;
                                //    }
                                //});
                                self.Body.DifussionWayDetails = responseJSON.path.path;
                                //self.SelectedHotel.HotelPickUpID = responseJSON.response.ObjectID.HotelPickUpID;
                            } else {
                                $('#file-' + id).removeClass('alert-info')
                                    .addClass('alert-error')
                                    .html('<i class="icon-exclamation-sign"></i> ' +
                                        'Error with ' +
                                        '“' + fileName + '”: ' +
                                        responseJSON.response.Exception.Message);
                            }
                        }
                    },
                    debug: true
                });
            },
            loadFlowDifussion: function () {
                let self = this;
                if (self.Body.DifussionWay == 3) {
                    //self.Body.DifussionWayDetails = '';
                    //google.charts.load('current', { packages: ['orgchart'] });
                    //google.charts.setOnLoadCallback(function () {
                    //    chart = new google.visualization.OrgChart(document.getElementById('chartContainer'));
                    //    chartTable = new google.visualization.DataTable();
                    //    chartTable.addColumn('string', '');
                    //    chartTable.addColumn('string', '');
                    //    chartTable.addColumn('string', '');
                    //    chart.draw(chartTable, { allowHtml: true });
                    //    google.visualization.events.addListener(chart, 'select', self.selectNode);
                    //});
                    //if (self.Body.WorkflowJson.length == 0) {
                    //    self.newWorkflow(null);
                    //}
                }
                else {
                    self.Body.WorkflowJson = [];
                }
            }
        },
        watch: {
            newWf: function (nVal) {
                let self = this;
                var firstStep = {};

                if (nVal == 0) {
                    firstStep = null;
                }
                else {
                    firstStep = {
                        Action: 'Save',
                        DelayDays: '',
                        DelayHours: '',
                        Email: null,
                        Step: '0',
                        Type: 'start',
                        Value: 'start0'
                    };
                }
                self.newWorkflow(firstStep);
            },
            selectedRow: function (nVal) {
                let self = this;
                self.Body.WorkflowID = nVal.WorkflowID;
                self.Body.System = nVal.System;
                self.Body.TerminalID = nVal.TerminalID;
                self.Body.DestinationID = nVal.DestinationID;
                self.Body.PlaceID = nVal.PlaceID;
                self.Body.SenderAddress = nVal.SenderAddress;
                self.Body.SenderPassword = '********';
                self.Body.ReplyTo = nVal.ReplyTo;
                self.Body.Name = nVal.Name;
                self.Body.WorkflowJson = nVal.WorkflowJson;

                if (nVal != null && nVal.WorkflowJson.length > 0) {
                    var value = self.Body.WorkflowJson[self.Body.WorkflowJson.length - 1].Value;
                    self.Counter = parseInt(value.replace(/^\D+/g, '')) + 1;
                }
                self.drawChart();
            }
        },
        mounted: function () {
            let self = this;
            this.getLists();
            axios.get('/Workflows/GetEmails', {
            }).then(response => { this.Emails = response.data });

            //google.charts.load('current', { packages: ['orgchart'] });
            //google.charts.setOnLoadCallback(function () {
            //    chart = new google.visualization.OrgChart(document.getElementById('chartContainer'));
            //    chartTable = new google.visualization.DataTable();
            //    chartTable.addColumn('string', '');
            //    chartTable.addColumn('string', '');
            //    chartTable.addColumn('string', '');
            //    chart.draw(chartTable, { allowHtml: true });
            //    google.visualization.events.addListener(chart, 'select', self.selectNode);
            //});

        }
    }

</script>