
 <template>
     <div>
         <log></log>
         <email></email>
         <sms></sms>
         <chat></chat>
         <tasks></tasks>
         <notifications></notifications>
         <help></help>
         <settings></settings>
         <session></session>
     </div>
</template>

<script> 
    import { mapGetters } from 'vuex';
    import notifications from './notifications.vue';
    import session from './session.vue';
    import tasks from './tasks.vue';
    import help from './help.vue';
    import settings from './settings.vue';
    import log from './log.vue';
    import email from './email.vue';
    import sms from './sms.vue';
    import chat from './chat.vue';
    export default ({
       // store: store,
        components: {
            'session': session,
            'notifications': notifications,
            'tasks': tasks,
            'help': help,
            'settings': settings,
            'log': log,
            'email': email,
            'sms': sms,
            'chat': chat,
        },
        computed: {
            ...mapGetters({
                load: 'load',//know if already load user data
                loadHubData:'loadHubData'
            })
        },
        methods: {
            //load components events
            setEvents() {
                $('.eplat-tool').off('click').on('click', function () {
                    $('.sidebar').addClass('d-none');
                    $('#' + $(this).attr('title')).removeClass('d-none');
                    $('#main').addClass('col-md-8 col-lg-9');
                });
                $('.sidebar .close').off('click').on('click', function () {
                    $('.sidebar').addClass('d-none');
                    $('#main').removeClass('col-md-8 col-lg-9');
                });
            },
            conectToePlatHub() {
                let self = this;
                let ePlatHub;
                ePlatHub = $.connection.ePlatHub
                var data = self.$store.state.session.ePlatHub.Data;
                if (data.UserID != "") {
                    //send user data
                    ePlatHub.server.userConnection(JSON.stringify(data));
                    //recibe notifications and send to vuex state
                    ePlatHub.client.addNotification = function (notification) {
                        console.log('add one');
                        self.HUB.addNotifications(notification);
                    }
                }
            },
            disconectePlatHub(data) {
                //EVENTO PENDIENTE
                /*$.connection.hub.disconnected(function () {
                    ePlatHubConnected = false;
                    $('.user-status').removeClass('status-connected').removeClass('active-tab').addClass('status-disconnected');
                    UI.messageBox(-1, "Connection Lost. Trying to reconnect with Server.");

                    setTimeout(function () {
                        $.connection.hub.start();
                    }, 5000); // Restart connection after 5 seconds.
                });*/
            },
            //CLIENT SERVICES
            HUB() {
                let self = this;
                var addNotifications = function (notification) {
                    console.log('added');
                    self.$store.dispatch('addNotification');
                }
                return {
                    addnotifications:addNotifications
                }
            }          
        },
        watch: {
            load() {//run loadMenu
                var object = {
                    Browser: '',
                    BrowserVersion: '',
                    Url: window.location.href,
                    Title: document.title,
                };
                this.$store.dispatch('terminalsNames');
                this.$store.dispatch('fillePlatHubData',object);
                this.$store.dispatch('loadMenu');
            },
            loadHubData() {//cuando este cargada la informacion entonces enviala al hub
                let self = this;
                var ePlatStatusCon = self.$store.state.session.ePlatHub;
                if(ePlatStatusCon) {
                    self.conectToePlatHub();
                } else {
                    console.log('disconected');
                    this.disconectePlatHub();
                }
            },
            status() {
                /*let self = this;
                var ePlatStatusCon = self.$store.state.session.ePlatHub;
                if (ePlatStatusCon) {
                    self.conectToePlatHub();
                } else {
                    console.log('disconected');
                    this.disconectePlatHub();
                }*/
            }

        },
        beforeCreate() {// get User information before create elements in html and turn on hub
            let self = this;
            console.log('inicio')
            this.$store.dispatch('getUserInfo');
           // this.$store.dispatch('getUserNotifications');
           // this.$store.dispatch('getUserNotificationsSettings');
            $.connection.hub.start().done(function () {
                console.log('HUB ON');
                self.$store.dispatch('hubConnected', true);
            });
        },
        mounted: function () {
            this.setEvents();
          /*  var ePlatHub = $.connection.ePlatHub;
            ePlatHub.client.add = function (notification) {
                console.log('ADD', notification)
                HUB.add(notification);
            };

            var HUB = function () {
                var add = function (notification) {
                    console.log(notification);
                    //state.actions.addObject
                }
                return {
                    add: add
                }
            }();*/
        }
    })
</script>