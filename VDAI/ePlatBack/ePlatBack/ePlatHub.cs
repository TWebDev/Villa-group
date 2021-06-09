using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using ePlatBack.Models;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using System.Data.SqlClient;
using System.Web.Mvc;
namespace ePlatBack
{
    public class ePlatHub : Hub
    {

        private static List<ActiveUser> users = new List<ActiveUser>();

        ePlatEntities db = new ePlatEntities();

        public void userConnection(string jsonModel)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            ConnectionParams model = js.Deserialize<ConnectionParams>(jsonModel);
            //sino existe en la coleccion, crealo
            //usuario nuevo
            if (users.Count(x => x.UserID == model.UserID) == 0)
            {
                ActiveUser newUser = new ActiveUser();
                tblUserProfiles userProfile = db.tblUserProfiles.Single(x => x.userID == model.UserID);
                newUser.UserID = model.UserID;
                newUser.FirstName = userProfile.firstName;
                newUser.LastName = userProfile.lastName;
                newUser.Role = db.aspnet_Roles.SingleOrDefault(r => r.RoleId == model.RoleID).RoleName;
                List<Connection> connections = new List<Connection>();
                Connection newConnection = new Connection();
                newConnection.ConnectionID = Context.ConnectionId;
                newConnection.Browser = model.Browser;
                newConnection.BrowserVersion = model.BrowserVersion;
                newConnection.Url = model.Url;
                newConnection.Title = model.Title;
                newConnection.DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                newConnection.SelectedTerminalIDs = model.SelectedTerminalIDs;
                if (model.SelectedTerminalIDs != null)
                {
                    long[] selectedTerminals = model.SelectedTerminalIDs.Split(',').Select(m => long.Parse(m)).ToArray();
                    var terminals = db.tblTerminals.Where(t => selectedTerminals.Contains(t.terminalID)).Select(t => t.terminal);
                    newConnection.SelectedTerminals = "";
                    foreach (var terminal in terminals)
                    {
                        if (newConnection.SelectedTerminals != "")
                        {
                            newConnection.SelectedTerminals += ",\n";
                        }
                        newConnection.SelectedTerminals += terminal;
                    }
                }
                else
                {
                    newConnection.SelectedTerminals = "";
                }
                newConnection.WorkGroupID = model.WorkGroupID;
                connections.Add(newConnection);
                newUser.Connections = connections;

                users.Add(newUser);
            }
            else
            {
                //modificar sólo conexión
                var currentUser = users.FirstOrDefault(u => u.UserID == model.UserID);
                if (currentUser.Connections.Count(c => c.ConnectionID == Context.ConnectionId) > 0)
                {
                    var currentConnection = currentUser.Connections.Single(c => c.ConnectionID == Context.ConnectionId);
                    currentConnection.Url = model.Url;
                    currentConnection.Title = model.Title;
                    currentConnection.DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                    currentConnection.SelectedTerminalIDs = model.SelectedTerminalIDs;
                    if (model.SelectedTerminalIDs != null)
                    {
                        long[] selectedTerminals = model.SelectedTerminalIDs.Split(',').Select(m => long.Parse(m)).ToArray();
                        var terminals = db.tblTerminals.Where(t => selectedTerminals.Contains(t.terminalID)).Select(t => t.terminal);
                        currentConnection.SelectedTerminals = "";
                        foreach (var terminal in terminals)
                        {
                            if (currentConnection.SelectedTerminals != "")
                            {
                                currentConnection.SelectedTerminals += ",\n";
                            }
                            currentConnection.SelectedTerminals += terminal;
                        }
                    }
                    else
                    {
                        currentConnection.SelectedTerminals = "";
                    }
                    currentConnection.WorkGroupID = model.WorkGroupID;
                }
                else// solo una conexion por usuario
                {   //
                        currentUser.Connections.Clear();
                        //
                        Connection newConnection = new Connection();
                        newConnection.ConnectionID = Context.ConnectionId;
                        newConnection.Browser = model.Browser;
                        newConnection.BrowserVersion = model.BrowserVersion;
                        newConnection.Url = model.Url;
                        newConnection.Title = model.Title;
                        newConnection.DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                        newConnection.SelectedTerminalIDs = model.SelectedTerminalIDs;
                        if (model.SelectedTerminalIDs != null)
                        {
                            long[] selectedTerminals = model.SelectedTerminalIDs.Split(',').Select(m => long.Parse(m)).ToArray();
                            var terminals = db.tblTerminals.Where(t => selectedTerminals.Contains(t.terminalID)).Select(t => t.terminal);
                            newConnection.SelectedTerminals = "";
                            foreach (var terminal in terminals)
                            {
                                if (newConnection.SelectedTerminals != "")
                                {
                                    newConnection.SelectedTerminals += ",\n";
                                }
                                newConnection.SelectedTerminals += terminal;
                            }
                        }
                        else
                        {
                            newConnection.SelectedTerminals = "";
                        }
                        newConnection.WorkGroupID = model.WorkGroupID;
                        currentUser.Connections.Add(newConnection);
                    /*    if (notifications.Count(x => x.forUserID == currentUser.UserID)>0)
                        {
                           getPendientNotifications(currentUser.UserID);
                        }*/
                }
            }
            Clients.All.updateConnectedUsers(users.OrderBy(x => x.FirstName));
        }

        public void userDisconnect(string user)
        {
            if (user != null)
            {
                Guid userID = Guid.Parse(user);

                var userConnection = users.FirstOrDefault(x => x.UserID == userID);
                if (userConnection != null)
                {
                    userConnection.Connections.Clear();
                    users.RemoveAll(n => n.UserID == userID);
                }
            }
            

        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            foreach (var u in users)
            {
                var connToRemove = u.Connections.SingleOrDefault(c => c.ConnectionID == Context.ConnectionId);
                if (connToRemove != null)
                    u.Connections.Remove(connToRemove);
            }

            Clients.All.updateConnectedUsers(users.OrderBy(x => x.FirstName));
            
            return base.OnDisconnected(true);
        }

        //call directamente desde el ePlatHub.js
        public void sendNotification(string text, string fromUserID, string toUserID, string important, string workgroupid)
        {
            //save notifications in dB
            ePlatEntities db = new ePlatEntities();
            tblNotifications notification = new tblNotifications();
            notification.notificationTypeID = 1;//test
            notification.sysItemTypeID = 9;//test
            notification.terminalID = 5;
            notification.description = text;
            notification.eventByUserID = Guid.Parse(fromUserID);
            notification.read_ = false;
            notification.eventDateTime = DateTime.Now;
            notification.forUserID = Guid.Parse(toUserID);
            db.tblNotifications.AddObject(notification);
           // db.SaveChanges();


            //ENVIAR NOTIFICACIONES AL CLIENTE 
            var user = users.Single(x => x.UserID == new Guid(fromUserID));
            NotificationModel newNotification = new NotificationModel();
            newNotification.NotificationID = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newNotification.Notification = text+ " - Respuesta";
            newNotification.From = user.FirstName + ' ' + user.LastName;
            newNotification.Date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
       //     newNotification.Important = (important == "true" ? true : false);
            newNotification.WorkGroupID = workgroupid;
            //to userID
            Guid userID = Guid.Parse(toUserID);
            var connections = users.Single(u => u.UserID == userID).Connections;
            foreach (var conn in connections)
            {
                Clients.Client(conn.ConnectionID).addNotification(notification);
            }
            Clients.All.addNotification(notification);
        }

        public void deleteNotification(string notificationid)
        {
            Clients.All.deleteNotification(notificationid);
        }

        public void sendMessage(string text, string fromUserID, string toUserID)
        {
            MessageModel newMessage = new MessageModel();
            newMessage.MessageID = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newMessage.Message = text;
            newMessage.DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
            newMessage.FromUserID = fromUserID;
            newMessage.ToUserID = toUserID;
            //fromuserid
            Guid fromUid = new Guid(fromUserID);
            var fromConnections = users.Single(u => u.UserID == fromUid).Connections;
            foreach (var conn in fromConnections)
            {
                Clients.Client(conn.ConnectionID).addMessage(newMessage);
            }            

            //touserid
            Guid toUid = new Guid(toUserID);
            var toConnections = users.Single(u => u.UserID == toUid).Connections;
            foreach (var conn in toConnections)
            {
                Clients.Client(conn.ConnectionID).addMessage(newMessage);
            }            
        }

        public void getMyTickets(string uid)
        {
            if (uid != string.Empty) {
                Guid userid = new Guid(uid);
                List<TicketViewModel.TicketItem> myTickets = TicketDataModel.GetMyTickets(userid);
                var fromConnections = users.Single(u => u.UserID == userid).Connections;
                foreach (var conn in fromConnections)
                {
                    Clients.Client(conn.ConnectionID).renderTickets(myTickets);
                } 
            }            
        }

        public void updateTickets(string tickets, int mode) //mode[1: order, 2: info]
        {
            bool updated = TicketDataModel.UpdateMyTickets(tickets);
            if (mode == 1)
            {
                Clients.Client(Context.ConnectionId).ticketsUpdated(updated);
            }
            else
            {
                getMyTickets(new UserSession().UserID.ToString());
            }            
        }

        public void quickSaveTicket(string subject, string body, int order)
        {
            TicketDataModel.SaveMyTicket(subject, body, order);
            getMyTickets(new UserSession().UserID.ToString());
        }
        /// <summary>
        /// ///////////
        /// </summary>
        
     
        public void addNewNotification(string text, string fromUserID, string toUserID, string important, string workgroupid)
        {
            ePlatEntities db = new ePlatEntities();
            //send and save with save notification module
            var forUser = Guid.Parse(toUserID);
            //ENVIAR NOTIFICACIONES AL CLIENTE 
            var user = users.Single(x => x.UserID == new Guid(fromUserID));
            NotificationModel newNotification = new NotificationModel();
            newNotification.NotificationID = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newNotification.Notification = text + " - Respuesta " +DateTime.Now;
            newNotification.From = user.FirstName + ' ' + user.LastName;
            newNotification.Date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
            newNotification.Important = (important == "true" ? true : false);
            newNotification.WorkGroupID = workgroupid;

            //save notifications in dB
            tblNotifications notification = new tblNotifications();
            notification.notificationTypeID = 1;//test
            notification.sysItemTypeID = 9;//test
            notification.terminalID = 5;
            notification.description = text;
            notification.eventByUserID = Guid.Parse(fromUserID);
            notification.read_ = false;
            notification.eventDateTime = DateTime.Now;
            notification.forUserID = Guid.Parse(toUserID);
            db.tblNotifications.AddObject(notification);
            // db.SaveChanges();

         //   notifications.Add(newNotification);

            var connection = users.Single(x => x.UserID == forUser).Connections;
            foreach(var conn in connection)
            {
                Clients.Client(conn.ConnectionID).addNotification(newNotification);
            }
        }

        public void addClientNotification(string _itemID, string _sysItemTypeID, string _terminalID, string _forUserID, string _description, string _eventUserID)
        {
            ePlatEntities db = new ePlatEntities();
            var forUserID = Guid.Parse(_forUserID);
            var forUser = db.tblUserProfiles.Single(x => x.userID == forUserID);
            var notification = new Notification();
            notification.itemID = _itemID;
            notification.sysItemTypeID = _sysItemTypeID;
            notification.terminalID = _terminalID;
            notification.forUserID = _forUserID;
            notification.description = _description;
            notification.read = false;
            notification.eventDateTime = DateTime.Now.ToString();
            notification.eventByUserID = _eventUserID;
            notification.eventUserFirstName = forUser.firstName;
            notification.eventUserLastName = forUser.lastName;

            tblNotifications newNotification = new tblNotifications();
            newNotification.notificationTypeID = long.Parse(_itemID);
            newNotification.sysItemTypeID = 14;
            newNotification.terminalID = long.Parse(_terminalID);
            newNotification.forUserID = forUserID;
            newNotification.description = _description;
            newNotification.read_ = false;
            newNotification.eventDateTime = DateTime.Parse(notification.eventDateTime);
            newNotification.eventByUserID = Guid.Parse(_eventUserID);
            db.tblNotifications.AddObject(newNotification);
            db.SaveChanges();
            notification.notificationID = newNotification.notificationID.ToString();
            var connection = users.Single(x => x.UserID == forUserID).Connections;
            foreach (var conn in connection)
            {
                Clients.Client(conn.ConnectionID).addNotification(notification);
            }
        }

        public void updateNotifications(string userID)
        {
            ePlatEntities db = new ePlatEntities();
            Guid forUserID = Guid.Parse(userID);
            var getNotificationsByUser = db.tblNotifications.Single(x => x.read_ == false && x.readDateTime != null && x.forUserID == forUserID);
            var connection = users.Single(x => x.UserID == forUserID).Connections;
            foreach (var conn in connection)
                Clients.Client(conn.ConnectionID).addNotification(getNotificationsByUser);
        }

        public void updateTarget(Guid userID)
        {
            // si el usuario esta online
            if(users.Count(x => x.UserID == userID) > 0)
            {
                var connections = users.Single(x => x.UserID == userID).Connections;
                foreach (var conn in connections)
                {
                    Clients.Client(conn.ConnectionID).updateTarget();
                }
            }
        }

       /* public void getPendientNotifications (Guid userID)
        {
            if(notifications.Count()>0)
            {
                var connections = users.Single(x => x.UserID == userID).Connections;
                foreach(var con in connections)
                {
                    foreach(var notification in notifications)
                    {
                        Clients.Client(con.ConnectionID).addNotification(notification);
                    }
                }
            }
        }*/
    }

}