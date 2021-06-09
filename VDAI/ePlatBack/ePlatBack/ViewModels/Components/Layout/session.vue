
<template>
    <div id="account" class="col col-md-4 col-lg-3 d-none bg-light sidebar pt-4 px-4">
        <button type="button" class="close float-right" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
        <h5>My Account</h5>
        <i class="material-icons edit-picture">photo_camera</i>
        <i class="material-icons account-icon d-block mt-4 mx-auto text-secondary">account_circle</i>
        <h5 class="text-center">{{ Session.FirstName }} {{ Session.LastName }}</h5>
        <div class="text-muted text-center">
            <small>{{ Session.Role }}<br />in {{ Session.WorkGroup }}</small>
        </div>
        <br />
        <ul class="list-unstyled mt-3">
            <li class="py-2">
                <a href="/account/edit">Edit my profile</a>
            </li>
            <li class="py-2">
                <a href="/account/changepassword">Change my password</a>
            </li>
        </ul>
        <!--<button id="sendN" v-on:click="send">send Notification</button>-->
    </div>
</template>

<script>

    import { mapState,mapActions } from 'vuex';
    export default ({
        computed: {
            ...mapState({
                Session: "session"
            })
        },
        methods: {
            ...mapActions({        
                selectedTerminals: 'selectedTerminal',
                updateSelectedTerminals: 'updateSelectedTerminals',
            }),
        },
        mounted() {
            //TurnOn Hub
           /* var ePlatHub;
            let self = this;
            $('document').ready(function () {
                console.log('session');
                // entire view has been rendered
                ePlatHub = $.connection.ePlatHub;
                ePlatHub.client.addNotification = function (notification) {
                    HUB.addNotification(notification);
                }
                //Conection 
                $.connection.hub.start().done(function () {
                    console.log('hub on ');
                    console.log(self.Session.UserID);
                    console.log(self.Session.TerminalIDs);
                    var dataObject = {
                        UserID: self.Session.UserID,
                        Browser: '',
                        BrowserVersion: '',
                        Url: window.location.href,
                        Title: document.title,
                        SelectedTerminalIDs: self.Session.selectedTerminalsIDs,
                        WorkGroupID: self.Session.WorkGroupID,
                        RoleID: self.Session.RoleID
                    }
                    ePlatHub.server.userConnection($.toJSON(dataObject));
                    ePlatHub.server.sendNotification('Ping', self.Session.UserID, self.Session.UserID, 'yes', self.Session.WorkGroupID);
                    console.log('datos enviados al hub ' + dataObject);
                });

                //function to store
                var HUB = function () {
                    var addNotification = function (notification) {
                        console.log(notification.Notification);
                        //state.actions.addObject
                        self.addDataObject(notification);
                    }
                    return {
                        addNotification: addNotification
                    }
                }();
            });*/
        }
    })
</script>