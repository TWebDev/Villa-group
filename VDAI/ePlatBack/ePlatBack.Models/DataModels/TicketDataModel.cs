using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;

namespace ePlatBack.Models.DataModels
{
    public class TicketDataModel
    {
        ePlatEntities db = new ePlatEntities();
        public static UserSession session = new UserSession();

        public class TicketCatalogs
        {
            public static List<SelectListItem> FillDrpTicketStatus()
            {
                List<SelectListItem> status = new List<SelectListItem>();
                ePlatEntities db = new ePlatEntities();

                foreach (var st in db.tblSupportTicketStatus)
                {
                    status.Add(new SelectListItem()
                    {
                        Value = st.supportTicketStatusID.ToString(),
                        Text = st.supportTicketStatus
                    });
                }

                return status;
            }
        }

        public List<TicketViewModel.TicketItem> GetTickets(TicketViewModel.Search_Ticket model)
        {
            List<TicketViewModel.TicketItem> list = new List<TicketViewModel.TicketItem>();
            int statusLength = 0;
            int[] statusArray = new int[] { };
            int topStatus = 1;
            if (model.Search_SupportTicketStatusID != null)
            {
                statusArray = model.Search_SupportTicketStatusID.ToArray();
                statusLength = model.Search_SupportTicketStatusID.Length;
            }
            foreach (int ts in statusArray)
            {
                if (ts > topStatus)
                {
                    topStatus = ts;
                }
            }

            var Tickets = from t in db.tblSupportTickets
                          where (t.tblSupportTickets_SupportTicketStatus.Count(x => x.savedByUserID == model.Search_CreatedByUserID && x.supportTicketStatusID == 1) > 0 || model.Search_CreatedByUserID == null)
                          && (t.assignedToUserID == model.Search_AssignedToUserID || model.Search_AssignedToUserID == null)
                          && ((t.tblSupportTickets_SupportTicketStatus.Count(x => statusArray.Contains(x.supportTicketStatusID)) > 0 && t.tblSupportTickets_SupportTicketStatus.Count(x => x.supportTicketStatusID > topStatus ) == 0) || statusLength == 0)
                          select t;

            foreach (var t in Tickets)
            {
                TicketViewModel.TicketItem ti = new TicketViewModel.TicketItem();
                ti.supportTicketID = t.supportTicketID;
                ti.subject = t.subject;
                ti.body = t.body;
                ti.terminalID = t.terminalID;
                ti.Terminal = (t.terminalID != null ? t.tblTerminals.terminal :  "Any");
                ti.reference = t.reference;
                ti.assignedToUserID = t.assignedToUserID;
                if (t.assignedToUserID != null)
                {
                    ti.AssignedToUser = t.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + t.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName;
                }
                else
                {
                    ti.AssignedToUser = "Unassigned";
                }          
                tblUserProfiles requestedBy = t.tblSupportTickets_SupportTicketStatus.OrderBy(x => x.supportTicketStatusID).FirstOrDefault().aspnet_Users.tblUserProfiles.FirstOrDefault();
                ti.RequestedByUser = requestedBy.firstName + " " + requestedBy.lastName;
                ti.changeset = t.changeset;
                ti.SupportTicketStatusID = t.tblSupportTickets_SupportTicketStatus.OrderByDescending(s => s.supportTicketStatusID).FirstOrDefault().tblSupportTicketStatus.supportTicketStatusID;
                ti.SupportTicketStatus = t.tblSupportTickets_SupportTicketStatus.OrderByDescending(s => s.supportTicketStatusID).FirstOrDefault().tblSupportTicketStatus.supportTicketStatus;
                ti.StatusHistory = new List<TicketViewModel.TicketStatus>();
                foreach (var s in t.tblSupportTickets_SupportTicketStatus.OrderBy(u =>u.dateStatus))
                {
                    TicketViewModel.TicketStatus st = new TicketViewModel.TicketStatus();
                    st.SupportTicketStatus = s.tblSupportTicketStatus.supportTicketStatus;
                    st.Date = s.dateStatus.ToString("yyyy-MM-dd hh:mm:ss tt");
                    st.SavedByUser = s.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + s.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName;
                    st.Comments = s.comments;
                    ti.StatusHistory.Add(st);
                }

                list.Add(ti);
            }

            return list.OrderBy(x => x.StatusHistory.FirstOrDefault(l => l.SupportTicketStatus == "Requested").Date).ToList();
        }

        public static AttemptResponse SaveMyTicket(string subject, string body, int order)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse attempt = new AttemptResponse();
            tblSupportTickets newTicket = new tblSupportTickets();
            //nuevo, agregar primer status
            newTicket.supportTicketID = Guid.NewGuid();
            if (subject != null)
            {
                newTicket.subject = subject;
            }
            if (body != null)
            {
                newTicket.body = body;
            }
            else
            {
                newTicket.body = "";
            }
            newTicket.terminalID = null;
            newTicket.reference = null;
            newTicket.assignedToUserID = new UserSession().UserID;
            newTicket.changeset = null;
            newTicket.order_ = order;
            
                tblSupportTickets_SupportTicketStatus newStatus0 = new tblSupportTickets_SupportTicketStatus();
                newStatus0.supportTicketStatusID = 1;
                newStatus0.dateStatus = DateTime.Now;
                newStatus0.savedByUserID = new UserSession().UserID;
                newStatus0.comments = null;
                newTicket.tblSupportTickets_SupportTicketStatus.Add(newStatus0);
            
            tblSupportTickets_SupportTicketStatus newStatus = new tblSupportTickets_SupportTicketStatus();
            newStatus.supportTicketStatusID = 2;
            newStatus.dateStatus = DateTime.Now;
            newStatus.savedByUserID = new UserSession().UserID;
            newStatus.comments = null;
            newTicket.tblSupportTickets_SupportTicketStatus.Add(newStatus);

            db.tblSupportTickets.AddObject(newTicket);

            db.SaveChanges();

            attempt.Type = Attempt_ResponseTypes.Ok;
            attempt.Exception = null;
            attempt.ObjectID = newTicket.supportTicketID;
            return attempt;
        }

        public AttemptResponse SaveTicket(TicketViewModel.TicketItem ticket)
        {
            AttemptResponse attempt = new AttemptResponse();
            tblSupportTickets newTicket = new tblSupportTickets();
            if (ticket.supportTicketID == new Guid("00000000-0000-0000-0000-000000000000"))
            {
                //nuevo, agregar primer status
                newTicket.supportTicketID = Guid.NewGuid();
                if (ticket.subject != null)
                {
                    newTicket.subject = ticket.subject;
                }
                if (ticket.body != null)
                {
                    newTicket.body = ticket.body;
                }
                else
                {
                    newTicket.body = "";
                }
                newTicket.terminalID = ticket.terminalID;
                newTicket.reference = ticket.reference;
                newTicket.assignedToUserID = ticket.assignedToUserID;
                newTicket.changeset = ticket.changeset;
                if (ticket.SupportTicketStatusID != 1)
                {
                    tblSupportTickets_SupportTicketStatus newStatus0 = new tblSupportTickets_SupportTicketStatus();
                    newStatus0.supportTicketStatusID = 1;
                    newStatus0.dateStatus = DateTime.Now;
                    newStatus0.savedByUserID = new UserSession().UserID;
                    newStatus0.comments = null;
                    newTicket.tblSupportTickets_SupportTicketStatus.Add(newStatus0);
                }
                tblSupportTickets_SupportTicketStatus newStatus = new tblSupportTickets_SupportTicketStatus();
                newStatus.supportTicketStatusID = ticket.SupportTicketStatusID;
                newStatus.dateStatus = DateTime.Now;
                newStatus.savedByUserID = new UserSession().UserID;
                newStatus.comments = ticket.StatusComment;
                newTicket.tblSupportTickets_SupportTicketStatus.Add(newStatus);

                db.tblSupportTickets.AddObject(newTicket);
            }
            else
            {
                //editar, ver cual fue el último status para compararlo
                newTicket = db.tblSupportTickets.FirstOrDefault(x => x.supportTicketID == ticket.supportTicketID);
                int lastStatusID = newTicket.tblSupportTickets_SupportTicketStatus.OrderByDescending(x => x.supportTicketStatusID).FirstOrDefault().supportTicketStatusID;
                if (lastStatusID != 6)
                {
                    if (ticket.subject != null)
                    {
                        newTicket.subject = ticket.subject;
                    }
                    if (ticket.body != null)
                    {
                        newTicket.body = ticket.body;
                    }
                    else
                    {
                        newTicket.body = "";
                    }
                    if (ticket.terminalID != null)
                    {
                        newTicket.terminalID = ticket.terminalID;
                    }                
                    newTicket.reference = ticket.reference;
                    newTicket.assignedToUserID = ticket.assignedToUserID;
                    newTicket.changeset = ticket.changeset;
                
                    if (ticket.SupportTicketStatusID != lastStatusID)
                    {
                        //agregar status
                        tblSupportTickets_SupportTicketStatus newStatus = new tblSupportTickets_SupportTicketStatus();
                        newStatus.supportTicketStatusID = ticket.SupportTicketStatusID;
                        newStatus.dateStatus = DateTime.Now;
                        newStatus.savedByUserID = new UserSession().UserID;
                        newStatus.comments = ticket.StatusComment;
                        newTicket.tblSupportTickets_SupportTicketStatus.Add(newStatus);
                    }
                }
                
            }
            db.SaveChanges();

            attempt.Type = Attempt_ResponseTypes.Ok;
            attempt.Exception = null;
            attempt.ObjectID = newTicket.supportTicketID;
            return attempt;
        }

        public static List<TicketViewModel.TicketItem> GetMyTickets(Guid userid)
        {
            ePlatEntities db = new ePlatEntities();
            List<TicketViewModel.TicketItem> tickets = new List<TicketViewModel.TicketItem>();
            var Tickets = from t in db.tblSupportTickets
                          where (t.assignedToUserID == userid
                          || t.assignedToUserID == null)
                          && t.tblSupportTickets_SupportTicketStatus.Count(s => s.supportTicketStatusID == 6) == 0
                          orderby t.order_ ascending
                          select t;

            foreach (var t in Tickets)
            {
                TicketViewModel.TicketItem ti = new TicketViewModel.TicketItem();
                ti.supportTicketID = t.supportTicketID;
                ti.subject = t.subject;
                ti.body = t.body;
                ti.terminalID = t.terminalID;
                ti.Terminal = (t.terminalID != null ? t.tblTerminals.terminal : "Any");
                ti.reference = t.reference;
                ti.assignedToUserID = t.assignedToUserID;
                if (t.assignedToUserID != null)
                {
                    ti.AssignedToUser = t.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + t.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName;
                }
                else
                {
                    ti.AssignedToUser = "Unassigned";
                }
                tblUserProfiles requestedBy = t.tblSupportTickets_SupportTicketStatus.OrderBy(x => x.supportTicketStatusID).FirstOrDefault().aspnet_Users.tblUserProfiles.FirstOrDefault();
                ti.RequestedByUser = requestedBy.firstName + " " + requestedBy.lastName;
                ti.changeset = t.changeset;
                ti.order = t.order_;
                ti.SupportTicketStatusID = t.tblSupportTickets_SupportTicketStatus.OrderByDescending(s => s.supportTicketStatusID).FirstOrDefault().tblSupportTicketStatus.supportTicketStatusID;
                ti.SupportTicketStatus = t.tblSupportTickets_SupportTicketStatus.OrderByDescending(s => s.supportTicketStatusID).FirstOrDefault().tblSupportTicketStatus.supportTicketStatus;
                ti.StatusHistory = new List<TicketViewModel.TicketStatus>();
                foreach (var s in t.tblSupportTickets_SupportTicketStatus.OrderBy(u => u.dateStatus))
                {
                    TicketViewModel.TicketStatus st = new TicketViewModel.TicketStatus();
                    st.SupportTicketStatus = s.tblSupportTicketStatus.supportTicketStatus;
                    st.Date = s.dateStatus.ToString("yyyy-MM-dd hh:mm:ss tt");
                    st.SavedByUser = s.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + s.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName;
                    st.Comments = s.comments;
                    ti.StatusHistory.Add(st);
                }

                tickets.Add(ti);
            }
            return tickets;
        }

        public static bool UpdateMyTickets(string ticketsStr)
        {
            ePlatEntities db = new ePlatEntities();
            bool done = true;
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<TicketViewModel.TicketUpdateItem> Tickets = js.Deserialize < List<TicketViewModel.TicketUpdateItem>>(ticketsStr);

            List<Guid> ticketIDs = Tickets.Select(x => x.SupportTicketID).ToList();

            var TicketsQ = from t in db.tblSupportTickets
                           where ticketIDs.Contains(t.supportTicketID)
                           select t;

            foreach (var ticket in TicketsQ) {
                TicketViewModel.TicketUpdateItem uTicket = Tickets.FirstOrDefault(t=>t.SupportTicketID == ticket.supportTicketID);

                ticket.order_ = uTicket.Order;
                foreach (int statusid in uTicket.Status)
                {
                    if (ticket.tblSupportTickets_SupportTicketStatus.Count(s => s.supportTicketStatusID == statusid) == 0)
                    {
                        //si es asignación, actualizar la asignación del ticket
                        if (statusid == 2)
                        {
                            ticket.assignedToUserID = new UserSession().UserID;
                        } else
                        {
                            //si no es asignación, pero no tiene asignación
                            if (ticket.tblSupportTickets_SupportTicketStatus.Count(p => p.supportTicketStatusID == 2) == 0)
                            {
                                ticket.assignedToUserID = new UserSession().UserID;
                                tblSupportTickets_SupportTicketStatus assignationStatus = new tblSupportTickets_SupportTicketStatus();
                                assignationStatus.supportTicketStatusID = 2;
                                assignationStatus.dateStatus = DateTime.Now;
                                assignationStatus.savedByUserID = new UserSession().UserID;
                                assignationStatus.comments = "";
                                ticket.tblSupportTickets_SupportTicketStatus.Add(assignationStatus);
                            }
                        }
                        //no existe el status, agregar
                        tblSupportTickets_SupportTicketStatus newStatus = new tblSupportTickets_SupportTicketStatus();
                        newStatus.supportTicketStatusID = statusid;
                        newStatus.dateStatus = DateTime.Now;
                        newStatus.savedByUserID = new UserSession().UserID;
                        newStatus.comments = "";
                        ticket.tblSupportTickets_SupportTicketStatus.Add(newStatus);
                    }
                }
                //buscar status que estaban pero ya no están seleccionados
            }
            db.SaveChanges();

            return done;
        }
    }
}
