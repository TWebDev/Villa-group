
import Vue from 'vue';
import Vuex from 'vuex';
import axios from 'axios';
import moment from 'moment';
import $ from 'jquery';
Vue.use(Vuex);

export const store = new Vuex.Store({
    state:
    {
        dataLoad:false,
        session: {
            UserID: '',
            TerminalIDs: '',//ID's terminales seleccionadas
            TerminalsIDsArr: [],//
            SelectedTerminalsNames:'',//
            WorkGroupID: null,
            RoleID: null,
            Username: '',
            FirstName: '',
            LastName: '',
            Photo: '',
            Role: '',
            WorkGroup: '',
            UserTerminals: [],
            Culture: 'en-US',
            TerminalsList: [],
            ePlatHub: {
                Connected: false,
                Data: {
                    UserID:'',
                    Browser: '',
                    BrowserVersion: null,
                    Url: '',
                    Title: '',
                    SelectedTerminalIDs: '',
                    WorkGroupID: null,
                    RoleID: null,
                    dataLoad: false
                }
            }
        },
        help: [],
        tasks: [],
        notifications: {
            notification:
            {
                notificationID: null,
                itemID: null,
                item: 'Prueba',
                terminalID: null,
                terminal: '',
                forUserID: null,
                forUseFirstName: '',
                forUserLastName: '',
                description: '',
                read: false,
                eventDateTime: '',
                eventByUserID: null,
                eventUserName: '',
                eventUserLastName: '',
                eventByUser: '',
                deliveredDateTime: '',
                readDateTime: '',
                url: ''
            },
            notifications:[]
        },
        settings: {
            notificationsPreferences: [],
            nSettings: {
                notificationReferenceID: null,
                notificationTypeID: null,
                notificationType: '',
                userID: null,
                user: '',
                color: '',
                terminalID: null,
                terminal: ''
            }
        },
        log: [],
        email: [],
        sms: []
    },
    mutations: {
        //ePlatHub
        hubConnected(state,status) {
            state.session.ePlatHub.Connected = status;
        },
        addData(state,info) {
            state.session.ePlatHub.Data = info;
        },
        //notifications
        addNotification(state, n) {
            console.log('notification in Mutation');
            state.notifications.notifications.push(n);
            //state.notificationsModel.notification.push(n);
        },
        fillNotifications(state, data) {
            $.each(data, function (i, n) {
                n.deliveredDateTime = moment(moment(), 'YYYY-MM-DD hh:mm a').format('YYYY-MM-DD hh:mm a');
                // n.eventDateTime = moment(n.eventDateTime, 'YYYY-MM-DD hh:mm a').format('YYYY-MM-DD hh:mm a');
            });
            state.notificationsModel.notifications = data;
        },
        //notificationsSettings
        fillNotificationsSettings(state, data) {
            state.notificationsSettings = data;
        },
        //session
        fillSession(state, data) {
            state.session.Culture = data.Culture;
            state.session.FirstName = data.FirstName;
            state.session.Photo = data.Photo;
            state.session.LastName = data.LastName;
            state.session.Role = data.Role;
            state.session.TerminalIDs = data.TerminalIDs;
            state.session.UserTerminals = data.UserTerminals;
            state.session.Username = data.Username;
            state.session.WorkGroup = data.WorkGroup;
            state.session.RoleID = data.RoleID;
            state.session.WorkGroupID = data.WorkGroupID
            state.session.TerminalsIDsArr = data.TerminalIDs.split(',');
            state.session.StatusConection = false;
           // state.session.SelectedTerminalsNames = data.SelectedTerminalsNames;
            state.session.UserID = data.UserID;
            state.dataLoad = true;
        },
        //terminals
        showTerminalsSameGroup(state) {
           let workgroupID = state.session.WorkGroupID;
           $.each(state.session.UserTerminals, function (i, terminal) { 
                terminal.Visible = (terminal.WorkGroupID == workgroupID ? true : false);
            });     
        },
        showAllTerminals(state) {
            $.each(state.session.UserTerminals, function (i, terminal) {

                terminal.Visible = true;
            });     
        },
        updateSelectedTerminals(state, objT) {
            state.session.WorkGroupID = objT[0].WorkGroupID;
            state.session.WorkGroup = objT[0].WorkGroup;
            state.session.RoleID = objT[0].RoleID;
            state.session.Role = objT[0].Role;
        },
        getTerminalsNames(state) {          
            let Terminals = state.session.TerminalIDs.split(',');
            if (Terminals.length == 1) {
                let tid = parseInt(Terminals[0]);
                let currentTerminal = state.session.UserTerminals.filter(ter => ter.TerminalID == tid);
                state.session.SelectedTerminalsNames = currentTerminal[0].Terminal;
            } else {
                state.session.SelectedTerminalsNames = Terminals.length + ' Selected Terminal' + (Terminals.length > 1 ? 's' : '');
            }
        },
        addSelectedTerminal(state, terminalID) {
            state.session.TerminalsIDsArr = [...state.session.TerminalsIDsArr, terminalID];
            state.session.TerminalsIDsArr.splice(-1, 1);
        },
        deleteSelectedTerminal(state, terminalID) {
            var index = state.session.TerminalsIDsArr.findIndex(tid => terminalID == tid);
            if( index > -1 )
                state.session.TerminalsIDsArr.splice(index, 1);
        },
        getTerminalIDsString(state) {
            state.TerminalIDs = state.session.TerminalsIDsArr.join(',');
        },
        loadTerminalDependentLists(state) {
            let terminalsList = [];
            if (state.session.UserTerminals.length > 0) {
                let Terminals = state.session.UserTerminals.filter(function (item) {
                    return state.session.TerminalIDs.split(',').indexOf(item.TerminalID.toString()) >= 0;
                })
                $.each(Terminals, function (t, terminal) {
                    terminalsList.push({
                        label: terminal.Terminal,
                        title: terminal.Terminal,
                        value: terminal.TerminalID
                    });
                });
            }
            state.session.TerminalsList = terminalsList;
        }
        //chat
        //task
        //settings
        //help
    },
    actions:{
        //ePlatHUB
        hubConnected({ state, commit },status) {
            commit('hubConnected',status);
        },
        fillePlatHubData({ state, commit },data) {
            let info;
            info = state.session.ePlatHub.Data;
            info.UserID = state.session.UserID;
            info.Browser = data.Browser;
            info.BrowserVersion = data.BrowserVersion;
            info.Url = data.Url;
            info.Title = data.Title;
            info.SelectedTerminalIDs = state.session.TerminalIDs;
            info.WorkGroupID = state.session.WorkGroupID;
            info.RoleID = state.session.RoleID;      
            info.dataLoad = true;
            commit('addData',info);
        },
        //notifications
        getUserNotifications: async function ({ commit }) {
            const data = await axios.post('/crm/Notifications/GetePlatNotifications', {})
                .then(response => {
                    const info = response.data;
                    commit('fillNotifications', info);
                });
        },
        addNotification({ state, commit }, object) {
            console.log('Notification in Action')
            commit('addNotification', object);
        },
        deliveredDateTime({ state }) {
            //send Delivered date
        },
        readDateTime({ state }) {
            //send readDateTime
        },
        //notificationSettings
        getUserNotificationsSettings: async function ({ commit }) {
        /*    const data = await axios.post('/crm/Notifications/getUserNotificationsSettings'{})
                .then(response => {
                    const info = response.data;
                    commit('fillNotificationsSettings', info);
                });*/
        },
        //session
        getUserInfo: async function ({ state, commit }) {
            const data = await axios.get('/Account/SessionDetails', {})
                .then(response => {
                    const info = response.data;
                    commit('fillSession', info);
                });
        },
        saveTicket: async function ({ state, commit }) {
            const data = await axios.post('/Account/SaveTicket', {
                workgroupID: state.session.WorkGroupID,
                roleID: state.session.RoleID,
                terminals: state.session.TerminalIDs
            }).then(response => {
                const data = response.data;
                commit('loadTerminalDependentLists');
            });
        },
        loadMenu: async function ({ state }) {
            var url = $(location).attr('protocol') + '//' + $(location).attr('host');
            const info = await axios.post('/Account/GetMenuComponents',{
                selectedWorkGroupID: state.session.WorkGroupID,
                selectedRoleID: state.session.RoleID
            }).then(response => {
                const data = response.data;
                let builder = '';
                let flag;
                $.each(data, function (index, item) {
                    if (data[index].SysComponentTypeID == 1) {
                        flag = false;
                        if ((item.SysComponentUrl != undefined && item.SysComponentUrl != null) || !hasChildren(item.SysComponentID)) {
                            builder += '<li class="nav-item">';
                            builder += '<a class="nav-link" href="' + item.SysComponentUrl + '">' + item.SysComponentName + '</a>';
                        }
                        else {
                            builder += '<li class="nav-item dropdown">';
                            builder += '<a class="nav-link dropdown-toggle" href="#" id="m_' + item.SysComponentID + '" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' + item.SysComponentName + '</a>';
                        }
                        builder += '<ul class="dropdown-menu" aria-labelledby="m_' + item.SysComponentID + '">';
                        recursive(item.SysComponentID);
                    }
                });
                function hasChildren(componentID) {
                    let has = false;
                    $.each(data, function (index2, item2) {
                        if (item2.SysParentComponentID == componentID) {
                            has = true;
                            return false;
                        }
                    });
                    return has;
                }
                function recursive(componentID) {
                    $.each(data, function (index2, item2) {
                        if (item2.SysParentComponentID == componentID) {
                            flag = true;
                        }
                    });
                    if (flag == false) {//has children
                        builder += '</ul></li>';
                    }
                    else {
                        $.each(data, function (index2, item2) {
                            if (item2.SysParentComponentID == componentID) {
                                if ((item2.SysComponentUrl != undefined && item2.SysComponentUrl != null) || !hasChildren(item2.SysComponentID)) {
                                    builder += '<li>';
                                    builder += '<a class="dropdown-item" href="' + url + item2.SysComponentUrl + '">' + item2.SysComponentName.toString().trim() + '</a>';
                                }
                                else {
                                    builder += '<li class="dropdown-submenu">';
                                    builder += '<a class="dropdown-item dropdown-toggle" href="#">' + item2.SysComponentName.toString().trim() + '</a>';
                                }
                                builder += '<ul class="dropdown-menu submenu">';
                                recursive(item2.SysComponentID);
                            }
                        });
                        builder += '</ul></li>';
                    }
                    return builder;
                }
                $('#menu').empty();
                while (builder.indexOf('<ul class="dropdown-menu submenu"></ul>') > 0) {
                    builder = builder.substr(0, builder.indexOf('<ul class="dropdown-menu submenu"></ul>')) + builder.substr(builder.indexOf('<ul class="dropdown-menu submenu"></ul>') + 39, builder.length);
                }
                $('#menu').append(builder);
                $('.dropdown-menu a.dropdown-toggle').off('click').on('click', function (e) {
                    if (!$(this).next().hasClass('show')) {
                        $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
                    }
                    var $subMenu = $(this).next(".dropdown-menu");
                    $subMenu.toggleClass('show');


                    $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
                        $('.dropdown-submenu .show').removeClass("show");
                    });

                    return false;
                });
            });
        },
        //terminals
        selectedTerminal: function ({state, commit}, terminalID, roleID, workgroupID) {
            commit('showTerminalsSameGroup', terminalID, roleID, workgroupID);            
        },
        updateSelectedTerminals({ state, commit, dispatch }) {
                state.session.TerminalIDs = state.session.TerminalsIDsArr.join(',');
                //obtener WorkGroup y RoleID
                let tid = state.session.TerminalsIDsArr[0];
                let objT = state.session.UserTerminals.filter(terminal => terminal.TerminalID == tid);
                commit('updateSelectedTerminals', objT);
                if (state.session.WorkGroupID != localStorage.Eplat_SelectedWorkGroupID) {
                    dispatch('loadMenu');
                }
                localStorage.Eplat_SelectedTerminals = state.session.TerminalIDs;
                localStorage.Eplat_SelectedWorkGroupID = state.session.WorkGroupID;
                localStorage.Eplat_SelectedRole = state.session.RoleID;
            dispatch('saveTicket');
            dispatch('loadTerminals');
           // commit('loadTerminalDependentLists');
        },
        terminalsNames({commit, dispatch}) {     
            commit('getTerminalsNames');
            commit('showTerminalsSameGroup');
            commit('loadTerminalDependentLists');
        },
        loadTerminals({ commit }) {
            commit('getTerminalsNames');
            commit('showTerminalsSameGroup');
        },
        deleteSelectedTerminal({state, commit}, terminalID) {
           commit('deleteSelectedTerminal', terminalID);
        },
        addSelectedTerminal({state, commit}, terminalID) {
            commit('addSelectedTerminal', terminalID)
        },
        showAllTerminals({ commit }) {
            commit('showAllTerminals');
        }
        //chat hub
        //settings
    },
    getters: {
        //load Data
        load: state => {
            return state.dataLoad
        },
        loadHubData: state => {
            return state.session.ePlatHub.Data.dataLoad;
        },
        //HUB
        //notifications
        nPendient: state => {
            return state.notifications.notifications.length;
        },
        //Terminal
        terminalsSelected: state => {
            var terminals = [];
            $.each(state.session.TerminalsList, function (t, terminal) {
                terminals.push(terminal.value);
            });
            return terminals.join(',');
        }
        //chat
    }
})