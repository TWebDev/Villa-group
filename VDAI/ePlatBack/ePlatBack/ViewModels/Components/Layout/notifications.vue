
<template>
    <div id="notifications" class="col col-md-4 col-lg-3 d-none bg-light sidebar pt-4 px-4">
        <button type="button" class="close float-right" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
        <h5>Notifications</h5>
        <div class="container">
            <div class="card" v-for="n in notifications" :id="n.notifications">
                <div class="card-body">
                    <div container>
                        <div class="row align-middle">
                            <div class="col-sm-2 align-middle">
                                <i class="material-icons align-middle">info</i>
                            </div>
                            <div class="col-sm-10">
                                <p>{{n.description}}</p>
                            </div>
                        </div>
                        <div class="row small">
                            <div class=" col-sm-4">
                            </div>
                            <div class=" col-sm-8 align-bottom">
                                <footer class="blockquote-footer align-bottom">
                                    {{n.deliveredDateTime}}
                                </footer>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <br />
        </div>
     </div>
</template>

<script>   
    //import $ from 'jquery';
    import { mapState, mapActions, mapGetters } from 'vuex'
    import { store } from './StateManagement';
    export default ({
        store: store,
        computed: {
            ...mapState({
                notifications: 'notifications'
            })
        },
        watch: {

        },
        methods: {
            getNotifications: function () {       
                
            },
            changeIcon: function () {
               /* if (store.getters.notReadNoti != 0) {         
                    $('#notificationIcon').text('notification_important');
                }
                else {
                    $('#notificationIcon').text('notifications_none');
                }*/
            }
        }
    })
</script>
