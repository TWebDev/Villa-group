var ePlatHub;
var ePlatHubConnected = false;
$(function () {
    //if (localStorage["Eplat_NoHub"] != 'true') {
    //    ePlatHub = $.connection.ePlatHub;

    //    ePlatHub.client.renderTickets = function (tickets) {
    //        Tickets.renderTickets(tickets);
    //    }

    //    ePlatHub.client.updateConnectedUsers = function (users) {
    //        HUB.updateConnectedUsers(users);
    //    }

    //    ePlatHub.client.addNotification = function (notification) {
    //        HUB.addNotification(notification);
    //    }

    //    ePlatHub.client.deleteNotification = function (notificationid) {
    //        HUB.deleteNotification(notificationid);
    //    }

    //    ePlatHub.client.addMessage = function (message) {
    //        HUB.addMessage(message);
    //    }

    //    ePlatHub.client.ticketsUpdated = function (updated) {
    //        Tickets.ticketUpdated(updated);
    //    }

    //    $.connection.hub.disconnected(function () {
    //        ePlatHubConnected = false;
    //        $('.user-status').removeClass('status-connected').removeClass('active-tab').addClass('status-disconnected');
    //        UI.messageBox(-1, "Connection Lost. Trying to reconnect with Server.");

    //        setTimeout(function () {
    //            $.connection.hub.start();
    //        }, 5000); // Restart connection after 5 seconds.
    //    });

    //    $.connection.hub.start().done(function () {
    //        ePlatHubConnected = true;
    //        //enviar datos de conexión
    //        var browserInfo = UI.browserInfo();
    //        if ($('#uid').val() != "") {
    //            var dataObject = {
    //                UserID: $('#uid').val(),
    //                Browser: browserInfo.browserName,
    //                BrowserVersion: browserInfo.fullVersion,
    //                Url: window.location.href,
    //                Title: document.title,
    //                SelectedTerminalIDs: localStorage.Eplat_SelectedTerminals,
    //                WorkGroupID: localStorage.Eplat_SelectedWorkGroupID,
    //                RoleID: localStorage.Eplat_SelectedRole
    //            }
    //            ePlatHub.server.userConnection($.toJSON(dataObject));
    //        }
    //        //agregar botón para enviar notificaciones
    //        $('#btnSendNotification').off('click').on('click', function () {
    //            ePlatHub.server.sendNotification($('#txtNotification').val(), $('#uid').val(), ($('#chkNotificationImportant').is(':checked') ? 'true' : 'false'), $('#ddlNotificationWorkGroup').val());
    //            $('#pnlNewNotification').slideUp('fast');
    //        });

    //        //agregar evento a texto de mensajes
    //        $('#txtConversation').off('keyup').on('keyup', function (e) {
    //            if (e.which == 13) {
    //                if ($('[data-user-id="' + $('#hdnChatWith').val() + '"] .status-connected').length > 0) {
    //                    ePlatHub.server.sendMessage($(this).val(), $('#uid').val(), $('#hdnChatWith').val());
    //                    $(this).val('');
    //                } else {
    //                    UI.messageBox(-1, 'User not connected');
    //                }
    //            }
    //        });

    //        //solicitar lista de tickets
    //        ePlatHub.server.getMyTickets($('#uid').val());
    //    });

    //    UI.Notifications.load();
    //}    
});

var HUB = function () {

    var updateConnectedUsers = function (users) {
        if ($('#uid').val() != "") {
            var openTabs = 0;
            var activeTabs = 0;
            var connectedUsers = 0;
            $.each(users, function (u, user) {
                //comprobar si están en el mismo grupo de trabajo
                if (user.Connections.length > 0) {
                    connectedUsers++;
                    var sameGroup = false;
                    openTabs += user.Connections.length;
                    $.each(user.Connections, function (c, conn) {
                        if (conn != null) {
                            if (localStorage.Eplat_SelectedWorkGroupID == conn.WorkGroupID) {
                                sameGroup = true;
                            }
                        }                        
                    });

                    if (sameGroup) {
                        //agregar a lista
                        if ($('[data-user-id="' + user.UserID + '"]').length == 0) {
                            //agregar
                            var userItem = '<div class="user-item" data-user-id="' + user.UserID + '">'
                                + '<div class="user-item-name"><span class="user-status ' + (user.Connections.length > 0 ? 'status-connected' : 'status-disconnected') + '"></span>'
                                + ($('#uid').val() != user.UserID ? '<span class="chat-icon" data-chat-id="' + user.UserID + '" data-chat-name="' + user.FirstName + '"><span class="chat-icon-box"></span><span class="chat-icon-triangle"></span></span>' : '')
                                + '<span class="user-item-name-text" title="' + user.Role + '">' + ($('#uid').val() == user.UserID ? "Me" : user.FirstName + ' ' + user.LastName) + '</span></div>'
                                + '<div class="user-item-info">'
                                + '</div>'
                                + '</div>';
                            if ($('#uid').val() == user.UserID) {
                                $('#pnlUsers').prepend(userItem);
                            } else {
                                $('#pnlUsers').append(userItem);
                            }

                        };

                        //agregar conexiones
                        $('[data-user-id="' + user.UserID + '"] .user-item-info').html('');
                        var hasActiveTabs = false;
                        $.each(user.Connections, function (c, conn) {
                            if (conn != null) {
                                if (conn.Title.indexOf("Inactive") == -1) {
                                    activeTabs++;
                                    hasActiveTabs = true;
                                }
                                var connHtml = '<div class="user-item-info-conn' + (conn.Title.indexOf("Inactive") > 0 ? '' : ' conn-active') + '">'
                                + '<span class="block">' + (localStorage.Eplat_SelectedRole == '87e4708c-14fb-426b-a69b-05f28fc5dcfc' ? '<a href="' + conn.Url + '" target="_blank">' + conn.Title + '</a>' : conn.Title) + '</span>'
                                + '<span class="block">' + conn.DateTime + '</span>'
                                + '<span class="block" title="' + conn.SelectedTerminals + '">' + (conn.SelectedTerminals.split('\n').length == 1 ? conn.SelectedTerminals : conn.SelectedTerminals.split('\n').length + ' Terminals Selected') + '</span>'
                                + '<span class="block">' + conn.Browser + ' - ' + conn.BrowserVersion + '</span>'
                                + '</div>';
                                $('[data-user-id="' + user.UserID + '"] .user-item-info').append(connHtml);
                            }
                        });
                        if (hasActiveTabs) {
                            $('[data-user-id="' + user.UserID + '"] span.user-status').addClass('active-tab');
                        } else {
                            $('[data-user-id="' + user.UserID + '"] span.user-status').removeClass('active-tab');
                        }
                        UI.Chat.checkPendingMessages();
                    } else {
                        $.each(user.Connections, function (c, conn) {
                            if (conn.Title.indexOf("Inactive") == -1) {
                                activeTabs++;
                            }
                        });
                    }
                } else {
                    $('[data-user-id="' + user.UserID + '"] .user-item-info').html('');
                    $('[data-user-id="' + user.UserID + '"] span.user-status').removeClass('active-tab');
                }
                //mostrar conexión
                $('[data-user-id="' + user.UserID + '"] .user-status').removeClass('status-connected').removeClass('status-disconnected').addClass((user.Connections.length > 0 ? 'status-connected' : 'status-disconnected'));
            });
            $('.user-item-name-text').off('click').on('click', function () {
                $(this).parent().parent().find('.user-item-info').slideToggle('fast');
            });
            $('.chat-icon').off('click').on('click', function () {
                UI.Chat.chatWith($(this).attr('data-chat-id'), $(this).attr('data-chat-name'));
            });
            $('#spnOnlineUsers').text(connectedUsers);
            $('#spnOpenTabs').text(openTabs);
            $('#spnActiveTabs').text(activeTabs);
        }
    }

    var addNotification = function (notification) {
        if ($('#uid').val() != "") {
            var Notifications = eval('(' + localStorage.Eplat_Notifications + ')');
            if (Notifications == undefined) {
                Notifications = [];
            }

            if (notification.WorkGroupID == localStorage.Eplat_SelectedWorkGroupID || notification.WorkGroupID == "") {
                Notifications.push(notification);
                if (notification.Important) {
                    UI.Notifications.desktopNotification('ePlat Notifications', notification.Notification);
                }
            }

            localStorage.Eplat_Notifications = $.toJSON(Notifications);
            UI.Notifications.load();
        }
    }

    var deleteNotification = function (notificationid) {
        $('#' + notificationid + ' .notification-close').trigger('click');
    }

    var addMessage = function (message) {
        var chatWith = '';
        if (message.FromUserID == $('#uid').val()) {
            chatWith = message.ToUserID;
        } else {
            chatWith = message.FromUserID;
        }
        var Chat = eval('(' + localStorage.Eplat_Chat + ')');
        if (Chat == undefined) {
            Chat = [];
        }
        var match = false;
        $.each(Chat, function (u, user) {
            if (user.UserID == chatWith) {
                match = true;
                var exists = false;
                $.each(user.Messages, function (m, msg) {
                    if (msg.MessageID == message.MessageID) {
                        exists = true
                    }
                });
                if (!exists) {
                    user.Messages.push(message);
                }                
            }
        });
        if (!match) {
            var user = {
                UserID: chatWith,
                Messages: []
            }
            user.Messages.push(message);
            Chat.push(user);
        }
        
        localStorage.Eplat_Chat = $.toJSON(Chat);
        if ($('#pnlSlideUsers').css('margin-left') != '0px') {
            UI.Chat.loadChatMessages();
            if (UI.windowFocus) {
                if ($('#hdnChatWith').val() != message.FromUserID && message.FromUserID != $('#uid').val()) {
                    if (!$('#iconUsers').hasClass('pending-chats')) {
                        $('#iconUsers').addClass('pending-chats');
                    }
                    UI.Notifications.desktopNotification('ePlat Chat', $('[data-user-id="' + message.FromUserID + '"] .user-item-name-text').text() + ' sent you a message.', message.FromUserID, $('[data-user-id="' + message.FromUserID + '"] .user-item-name-text').text().substr(0, $('[data-user-id="' + message.FromUserID + '"] .user-item-name-text').text().indexOf(" ")));
                }
            } else {
                if (message.FromUserID != $('#uid').val()) {
                    UI.Notifications.desktopNotification('ePlat Chat', $('[data-user-id="' + message.FromUserID + '"] .user-item-name-text').text() + ' sent you a message.', message.FromUserID, $('[data-user-id="' + message.FromUserID + '"] .user-item-name-text').text().substr(0, $('[data-user-id="' + message.FromUserID + '"] .user-item-name-text').text().indexOf(" ")));
                }
            }            
        } else {
            if (!$('[data-chat-id="' + chatWith + '"]').hasClass('pending-messages')) {
                $('[data-chat-id="' + chatWith + '"]').addClass('pending-messages');
                if (message.FromUserID != $('#uid').val()) {
                    UI.Notifications.desktopNotification('ePlat Chat', $('[data-chat-id="' + chatWith + '"]').attr('data-chat-name') + ' sent you a message.', chatWith, $('[data-chat-id="' + chatWith + '"]').attr('data-chat-name'));
                }                
            }
            if (!$('#iconUsers').hasClass('pending-chats')) {
                $('#iconUsers').addClass('pending-chats');
            }           
        }
    }

    return {
        updateConnectedUsers: updateConnectedUsers,
        addNotification: addNotification,
        deleteNotification: deleteNotification,
        addMessage: addMessage
    }
}();