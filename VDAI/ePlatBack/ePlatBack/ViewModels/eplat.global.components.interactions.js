Vue.component('interactions', {
    props: ['trialid', 'leadid', 'readonly', 'bookingStatusList', 'agentsList', 'role'],
    data: function () {
        return {
            Interactions: [],
            Interaction: {
                InteractionID: null,
                InteractionLeadID: null,
                InteractionTrialID: null,
                InteractionBookingStatusID: null,
                InteractionTypeID: 1,
                InteractionComments: '',
                InteractionInterestLevelID: 1,
                InteractedWithUserID: null,
                ParentInteractionID: null,
                InteractionNotes: [],
                Editing: false
            },
            OriginalInteraction: {},
            Note: {
                InteractionID: null,
                InteractionLeadID: null,
                InteractionTrialID: null,
                InteractionTypeID: 13,
                InteractionComments: '',
                ParentInteractionID: null
            },
            InteractionTypes: [],
            InterestLevels: [],
            InteractionIcons: [
                {
                    InteractionTypeID: 1,
                    Icon: 'phone'
                },
                {
                    InteractionTypeID: 2,
                    Icon: 'email'
                },
                {
                    InteractionTypeID: 10,
                    Icon: 'chat'
                },
                {
                    InteractionTypeID: 11,
                    Icon: 'textsms'
                },
                {
                    InteractionTypeID: 12,
                    Icon: 'speaker_notes'
                },
            ],
            showAgentsList: true,
            showInteractionForm: false
        }
    },
    template: '<div>'
        + '<div class="btn-toolbar mb-2 mb-md-0 float-right">'
        //+ '<button type="button" class="btn btn-sm mr-1 btn-outline-secondary"><i class="material-icons">alarm</i></button>'
        + '<button type="button" v-if="!readonly" class="btn btn-sm btn-primary" @click="newInteraction()"><i class="material-icons">add</i></button>'
        + '</div>'
        + '<h5 class="mb-3">Interactions</h5>'

        + '<div id="divInteractionForm" class="card mt-3 mb-2" v-show="showInteractionForm">' //tarjeta de nueva interacción
        + '<div class="card-body">'
         + '<div class="form-row">'
         + '<div class="form-group col-md-4">'
          + '<select class="form-control mb-1" v-model="Interaction.InteractionTypeID">'
          + '<option v-for="it in InteractionTypes" v-bind:value="it.Value">{{ it.Text }}</option>'
          + '</select>'
          + '<select class="form-control mb-1" v-model="Interaction.InteractionBookingStatusID">'
          + '<option v-for="bs in bookingStatusList" v-bind:value="bs.Value">{{ bs.Text }}</option>'
          + '</select>'
          + '<select class="form-control mb-1" v-model="Interaction.InteractionInterestLevelID">'
          + '<option v-for="il in InterestLevels" v-bind:value="il.Value">{{ il.Text }}</option>'
          + '</select>'
          + '<select class="form-control" v-if="showAgentsList" v-model="Interaction.InteractedWithUserID">'
          + '<option v-for="ag in agentsList" v-bind:value="ag.Value" v-bind:selected="ag.Selected">{{ ag.Text }}</option>'
          + '</select>'
         + '</div>'
         + '<div class="form-group col-md-8">'
          + '<textarea class="form-control w-100 h-100" v-model="Interaction.InteractionComments"></textarea>'
         + '</div>'
         + '</div>'
         + '<div class="form-row">'
         + '<div class="text-right col">'
          + '<input type="button" class="btn btn-sm btn-primary mr-1" value="Save" @click="saveInteraction(1)" />'
          + '<input type="button" class="btn btn-sm btn-outline-secondary" value="Cancel" @click="cancelInteraction(Interaction.InteractionID)" />'
         + '</div>'
         + '</div>'
        + '</div>'
        + '</div>'

        + '<div class="card mt-2" v-if="Interactions.length == 0"><div class="card-body">There are not Interactions yet.</div></div>'

        + '<div v-for="(interaction, index) in NotEditingInteractions">'
        + '<div class="card mt-2" v-bind:data-interactionid="interaction.InteractionID" v-bind:key="interaction.InteractionID">' //tarjetas de interacciones pasadas
         + '<div class="card-body">'
         + '<div class="row">'
         + '<div class="col-md-4">'
         + '<span class="d-block mb-1"><i class="material-icons" style="font-size:2em">{{ getInteractionTypeIcon(interaction.InteractionTypeID) }}</i></span>'
         + '<span class="d-block mb-1"><small class="text-muted" style="font-size: .6em;">BOOKING STATUS</small><br />{{ interaction.InteractionBookingStatus }}<br /><small class="text-muted" style="font-size: .6em;">INTEREST LEVEL</small><br />{{ interaction.InteractionInterestLevel }}</span>'
         + '</div>'
         + '<div class="col-md-8">'
         + '<span v-html="breakLines(interaction.InteractionComments)"></span>'
         + '</div>'
         + '</div>'
         + '<div class="row">'
          + '<div class="col">'
          + '<div class="btn-toolbar mb-2 mt-3 mb-md-0 float-right">'
          + '<button type="button" class="btn btn-sm btn-outline-secondary mr-1" @click="editInteraction(interaction.InteractionID)" v-if="index == 0 && Interaction.InteractedWithUserID == interaction.InteractedWithUserID && lessThanMinutes(interaction.InteractionDate, 60)"><i class="material-icons">edit</i></button>'
          + '<button type="button" class="btn btn-sm btn-outline-secondary" v-bind:class="{ active: interaction.ShowNotes }" @click="interaction.ShowNotes = !interaction.ShowNotes"><i class="material-icons">comment</i><span class="badge badge-dark" v-if="interaction.InteractionNotes.length > 0">{{ interaction.InteractionNotes.length }}</span></button>'
          + '</div>'
          + '<div class="text-muted mt-4"><em><small>Interacted with {{ interaction.InteractedWithUser }} on {{ dateYYYYMMDDhhmmss(interaction.InteractionDate) }}</small></em></div>'
          + '</div>'
         + '</div>'
         + '</div>'
        + '</div>'// cierra card
        + '<div class="card mt-flat mx-2 bg-light" v-show="interaction.ShowNotes"><div class="card-body pt-0">' //notas
         + '<div v-for="(note, index) in interaction.InteractionNotes" class="row bg-light mt-3 pb-3 border-bottom">' //nota
         + '<div class="col-1"><div class="rounded-circle bg-secondary text-white initials-avatar">{{ getInitials(note.InteractedWithUser) }}</div></div><div class="col">{{ note.InteractionComments }}<br /><small><em>Written by {{ note.InteractedWithUser }} on {{ dateYYYYMMDDhhmmss(note.InteractionDate) }}</em></small></div><div class="col-1" style="cursor:pointer" v-if="Interaction.InteractedWithUserID == interaction.InteractedWithUserID && lessThanMinutes(note.InteractionDate, 10)"><i class="material-icons text-secondary" @click="deleteNote(interaction.InteractionID, note.InteractionID, index)">delete</i></div>'
         + '</div>'
         + '<div class="row mt-3" v-if="!readonly">' //formulario para nueva nota
         + '<div class="col-1"><div class="rounded-circle bg-secondary text-white initials-avatar">{{ getInitials(Interaction.InteractedWithUser) }}</div></div><div class="col-11">'
         + '<div class="input-group"><textarea class="form-control" v-model="Note.InteractionComments" v-on:click="Note.ParentInteractionID = interaction.InteractionID" ></textarea><div class="input-group-append"><button class="btn btn-outline-secondary bg-secondary text-white" type="button" @click="saveInteraction(2)">Post</button></div></div>'
         + '</div>'
         + '</div>'
        + '</div></div>' //cierra notas
        + '</div>' //cierra interaccion

        + '</div>',
    computed: {
        NotEditingInteractions: function () {
            return this.Interactions.filter(function (interaction) {
                return interaction.Editing == false;
            });
        }
    },
    watch: {
        trialid: function (newVal, oldVal) {
            let self = this;
            self.Interaction.InteractionTrialID = newVal;
            self.Note.InteractionTrialID = newVal;
            //cargar las interacciones para el trialid
            self.getInteractions();
        },
        leadid: function (newVal, oldVal) {
            let self = this;
            self.Interaction.InteractionLeadID = newVal;
            self.Note.InteractionLeadID = newVal;
            //cargar las interacciones para el leadid
            self.getInteractions();
        },
        agentsList: function (newVal, oldVal) {
            //asignar el usuario logueado
            this.Interaction.InteractedWithUserID = _.find(newVal, 'Selected').Value;
            this.Interaction.InteractedWithUser = _.find(newVal, 'Selected').Text;
        },
        bookingStatusList: function (newVal, oldVal) {
            this.Interaction.InteractionBookingStatusID = newVal[0].Value;
        }
    },
    mounted: function () {
        //obtener lista de tipos de interacción
        this.getCatalogs();
        
        //verificar si es agente
        if (this.role.indexOf("agent") >= 0) {
            this.showAgentsList = false;
        }
        
        this.Interaction.InteractionTrialID = this.trialid;
        this.Note.InteractionTrialID = this.trialid;
        this.Interaction.InteractionLeadID = this.leadid;
        this.Note.InteractionLeadID = this.leadid;
        this.getInteractions();
    },
    methods: {
        getInteractionTypeIcon: function (id) {
            return _.find(this.InteractionIcons, function (o) { return o.InteractionTypeID == id }).Icon;
        },
        getCatalogs: function () {
            let self = this;
            $.ajax({
                url: '/Interactions/GetInteractionTypes',
                cache: false,
                type: 'GET',
                success: function (data) {
                    self.InteractionTypes = data;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });

            //obtener lista de niveles de interés
            $.ajax({
                url: '/Interactions/GetInterestLevels',
                cache: false,
                type: 'GET',
                success: function (data) {
                    self.InterestLevels = data;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        },
        getInteractions: function () {
            let self = this;
            if (this.Interaction.InteractionTrialID) {
                $.ajax({
                    url: '/Interactions/GetInteractionsForTrial/' + this.Interaction.InteractionTrialID,
                    cache: false,
                    type: 'GET',
                    success: function (data) {
                        self.Interactions = data;
                    },
                    error: function (xhr, status, error) {
                        console.log(error);
                    }
                });
            } else if (this.Interaction.InteractionLeadID) {
                $.ajax({
                    url: '/Interactions/GetInteractionsForLead/' + this.Interaction.InteractionLeadID,
                    cache: false,
                    type: 'GET',
                    success: function (data) {
                        self.Interactions = data;
                    },
                    error: function (xhr, status, error) {
                        console.log(error);
                    }
                });
            }
        },
        deleteNote: function (interactionid, noteid, index) {
            let self = this;
            $.ajax({
                url: '/Interactions/DeleteInteraction/' + noteid,
                cache: false,
                type: 'DELETE',
                success: function (data) {
                    let interaction = _.find(self.Interactions, { 'InteractionID': interactionid });
                    Vue.delete(interaction.InteractionNotes, index);
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        },
        saveInteraction: function (type) {
            let self = this;
            $.ajax({
                url: '/Interactions/SaveInteractionInfo',
                cache: false,
                type: 'POST',
                data: (type == 1 ? self.Interaction : self.Note),
                success: function (data) {
                    if (data.ResponseType == 1) {
                        //agregar a la lista
                        if (type == 1) {
                            //emitir evento con el cambio de booking date
                            self.$emit('bschanged', data.Interaction.InteractionBookingStatusID, data.Interaction.InteractionBookingStatus);

                            //interacción
                            self.showInteractionForm = false;
                            if (self.Interaction.InteractionID == null) {
                                //nuevo
                                self.Interactions.unshift(data.Interaction);
                            } else {
                                //editando
                                self.Interaction.Editing = false;
                                self.Interaction.InteractedWithUser = data.Interaction.InteractedWithUser;
                                self.Interaction.InteractedWithUserID = data.Interaction.InteractedWithUserID;
                                self.Interaction.InteractionBookingStatus = data.Interaction.InteractionBookingStatus;
                                self.Interaction.InteractionBookingStatusID = data.Interaction.InteractionBookingStatusID;
                                self.Interaction.InteractionComments = data.Interaction.InteractionComments;
                                self.Interaction.InteractionInterestLevel = data.Interaction.InteractionInterestLevel;
                                self.Interaction.InteractionInterestLevelID = data.Interaction.InteractionInterestLevelID;
                                self.Interaction.InteractionType = data.Interaction.InteractionType;
                                self.Interaction.InteractionTypeID = data.Interaction.InteractionTypeID;
                            }
                        } else {
                            //nota
                            self.Note.InteractionComments = '';
                            self.Interactions.map(function (interaction) {
                                return (interaction.InteractionID == data.Interaction.ParentInteractionID ? interaction.InteractionNotes.push(data.Interaction) : interaction);
                            });
                        }                        
                    } else {
                        //alerta de error
                        $.alert({
                            title: 'Error: ' + data.ResponseMessage,
                            content: data.ExceptionMessage + '<br /><small>' + data.InnerException + '</small>',
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            type: 'red'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        },
        breakLines: function (value) {
            if (value) {
                value = value.replace(/\n/g, '<br />');
            }
            return value
        },
        dateYYYYMMDDhhmmss: function(value){
            return moment(value).format("YYYY-MM-DD hh:mm:ss A");
        },
        newInteraction: function () {
            this.Interaction.Editing = false;
            this.Interaction = {
                InteractionID: null,
                InteractionLeadID: this.leadid,
                InteractionTrialID: this.trialid,
                InteractionBookingStatusID: this.bookingStatusList[0].Value,
                InteractionTypeID: 1,
                InteractionComments: '',
                InteractionInterestLevelID: 1,
                InteractedWithUserID: _.find(this.agentsList, 'Selected').Value,
                ParentInteractionID: null,
                InteractionNotes: [],
                Editing: false
            };
            this.showInteractionForm = true;
        },
        editInteraction: function (interactionID) {
            //this.OriginalInteraction = ;
            this.Interaction = _.find(this.Interactions, { 'InteractionID': interactionID });
            this.Interaction.Editing = true;
            this.showInteractionForm = true;
        },
        cancelInteraction: function (interactionID) {
            console.log(this.OriginalInteraction);
            this.showInteractionForm = false;
            //this.Interaction = this.OriginalInteraction;
            this.Interaction.Editing = false;
        },
        lessThanMinutes: function (value, minutes) {
            let ms = moment().diff(moment(value));
            let d = moment.duration(ms);
            let m = d.asMinutes();
            return (m < minutes ? true : false);
        },
        getInitials: function (name) {
            if (name) {
                return name.split(" ")[0].substr(0, 1) + name.split(" ")[1].substr(0, 1)
            }
        }
        //onchange: function (ev) {
        //    this.$emit('checked', this.curso.value, ev.target.checked)
        //}
    }
});