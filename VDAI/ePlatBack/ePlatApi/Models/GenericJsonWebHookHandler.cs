using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.AspNet.WebHooks;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using ePlatBack.Models;
using ePlatBack.Models.DataModels;
using System.Text.RegularExpressions;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using System.Net;
using System.Web;


public class GenericJsonWebHookHandler : WebHookHandler
{
    public GenericJsonWebHookHandler()
    {
        this.Receiver = "genericjson";
    }

    public override Task ExecuteAsync(string generator, WebHookHandlerContext context)
    {
        // Get JSON from WebHook
        JObject data = context.GetDataOrDefault<JObject>();
        ePlatEntities db = new ePlatEntities();
        var urlReferrer = "NO BROWSER";
        //verifica si viene de un browser
        if (HttpContext.Current.Request.UrlReferrer != null)
        {
            //Extrae la URL del cliente
            urlReferrer = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
        }

        if (context.Id == "zapier")
        {
            tblWebhooksLog newWHL = new tblWebhooksLog();
            try
            {
                //registrar log del webhook
                newWHL.webhook = "Zapier Facebook Ads";
                newWHL.webhookID = new Guid("c61bd1cc-6113-4753-ad97-c0198a560e66");
                newWHL.dateSaved = DateTime.Now;
                newWHL.json = data.ToString(Newtonsoft.Json.Formatting.None);
                db.tblWebhooksLog.AddObject(newWHL);
                db.SaveChanges();

                long terminalid = (long)data["terminalID"];

                //obtener datos del lead
                tblLeads newLead = new tblLeads();
                Guid? leadID = null;
                string email = null;
                string phone = null;
                string mobile = null;
                string filteredPhone = "";
                string filteredMobile = "";
                string filteredEmail = "";
                bool matchPhone = false;
                bool matchMobile = false;
                bool matchEmail = false;

                if (data["email"] != null && (string)data["email"] != "")
                {
                    email = (string)data["email"];
                    filteredEmail = email.Trim();
                }
                if (data["phone"] != null && (string)data["phone"] != "")
                {
                    phone = (string)data["phone"];
                    filteredPhone = Regex.Replace(phone, @"[^\d]", "");
                    if (filteredPhone.Length > 10)
                    {
                        filteredPhone = filteredPhone.Substring(filteredPhone.Length - 10);
                    }
                }
                if (data["mobile"] != null && (string)data["mobile"] != "")
                {
                    mobile = (string)data["mobile"];
                    filteredMobile = Regex.Replace(mobile, @"[^\d]", "");
                    if (filteredMobile.Length > 10)
                    {
                        filteredMobile = filteredMobile.Substring(filteredMobile.Length - 10);
                    }
                }

                if (email != null || phone != null || mobile != null)
                {
                    Guid? PhoneLeadID = null;
                    if (phone != null && phone.Length >= 10)
                    {
                        PhoneLeadID = (from p in db.tblPhones
                                       join l in db.tblLeads
                                       on p.leadID equals l.leadID
                                       where p.phone.Contains(filteredPhone)
                                       && l.initialTerminalID == terminalid
                                       orderby p.phoneID descending
                                       select p.leadID).FirstOrDefault();
                    }

                    if (PhoneLeadID != null && PhoneLeadID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        leadID = PhoneLeadID;
                        matchPhone = true;
                    }
                    else
                    {
                        if (mobile != null && mobile.Length >= 10)
                        {
                            PhoneLeadID = (from p in db.tblPhones
                                           join l in db.tblLeads
                                           on p.leadID equals l.leadID
                                           where p.phone.Contains(filteredMobile)
                                           && l.initialTerminalID == terminalid
                                           orderby p.phoneID descending
                                           select p.leadID).FirstOrDefault();
                        }
                        if (PhoneLeadID != null && PhoneLeadID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            leadID = PhoneLeadID;
                            matchMobile = true;
                        }
                        else
                        {
                            var EmailLeadID = (from e in db.tblLeadEmails
                                               join l in db.tblLeads
                                               on e.leadID equals l.leadID
                                               into e_l
                                               from l in e_l.DefaultIfEmpty()
                                               where e.email.Contains(filteredEmail)
                                               && l.initialTerminalID == terminalid
                                               select e.leadID).FirstOrDefault();

                            if (EmailLeadID != null && EmailLeadID != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                leadID = EmailLeadID;
                                matchEmail = true;
                            }
                        }
                    }
                }

                if (leadID == null)
                {
                    //new lead
                    newLead.leadID = Guid.NewGuid();
                    newLead.firstName = (string)data["firstName"];
                    newLead.lastName = (string)data["lastName"];
                    newLead.state = (string)data["state"];
                    newLead.timeToReach = (string)data["timeToReach"];
                    newLead.referenceID = (string)data["referenceID"];
                    newLead.initialTerminalID = terminalid;
                    newLead.terminalID = terminalid;
                    if (data["destinationID"] != null)
                    {
                        newLead.interestedInDestinationID = (int)data["destinationID"];
                    }
                    if (data["inputDateTime"] != null)
                    {
                        newLead.inputDateTime = DateTime.Now;
                        newLead.dateCollected = (DateTime)data["inputDateTime"];
                    }

                    //constantes
                    newLead.leadTypeID = 49; //Form: Facebook
                    newLead.bookingStatusID = 19; //Not Contacted
                    newLead.leadSourceID = 50; //Facebook
                    newLead.leadSourceChannelID = 5; //Facebook Ads
                    newLead.inputMethodID = 7; //webhook
                    newLead.submissionForm = true;

                    newLead.inputByUserID = new Guid("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                    newLead.deleted = false;
                    newLead.isTest = false;

                    //asignación
                    if (phone != null || email != null || mobile != null)
                    {
                        Guid? assignateToUserID = NetCenterDataModel.GetUserForAssignation(newLead.leadTypeID, newLead.terminalID);
                        newLead.leadStatusID = 29;
                        if (assignateToUserID != null)
                        {
                            newLead.assignationDate = DateTime.Now;
                            newLead.assignedToUserID = assignateToUserID;

                            tblLeadAssignations newAssignation = new tblLeadAssignations();
                            newAssignation.leadID = newLead.leadID;
                            newAssignation.assignedToUserID = (Guid)assignateToUserID;
                            newAssignation.dateSaved = DateTime.Now;
                            newAssignation.assignedByUserID = new Guid("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                            newLead.tblLeadAssignations.Add(newAssignation);

                            //notificar el nuevo lead.
                            HttpWebRequest http = (HttpWebRequest)WebRequest.Create("https://eplat.com/notifications/sendservernotification?title=New+Lead&description=A+new+lead+from+Facebook+has+been+assigned+to+you.&notificationTypeID=1&forUserID=" + newAssignation.assignedToUserID.ToString() + "&terminalID=" + newLead.initialTerminalID.ToString() + "&button=Open+Lead&onlyRealTime=true&action=true&url=/profile%23leadid%3D" + newLead.leadID.ToString());
                            WebResponse response = http.GetResponse();
                        }
                    }

                    db.tblLeads.AddObject(newLead);
                    db.SaveChanges();
                }
                else
                {
                    newLead = (from l in db.tblLeads
                               where l.leadID == leadID
                               select l).FirstOrDefault();

                    //notificar nuevo registro del lead.
                    if (newLead.assignedToUserID != null)
                    {
                        HttpWebRequest http = (HttpWebRequest)WebRequest.Create("https://eplat.com/notifications/sendservernotification?title=Existing+Lead&description=An+existing+lead+assigned+to+you+has+registered+on+Facebook.&notificationTypeID=1&forUserID=" + newLead.assignedToUserID.ToString() + "&terminalID=" + newLead.initialTerminalID.ToString() + "&button=Open+Lead&onlyRealTime=true&action=true&url=/profile%23leadid%3D" + newLead.leadID.ToString());
                        WebResponse response = http.GetResponse();
                    }
                }

                //asignación a Webhook log
                newWHL.leadID = newLead.leadID;

                //email & phone
                //Verificar que el lead no tenga ese email y/o teléfono

                if (!matchEmail)
                {
                    var LeadEmails = (from l in db.tblLeadEmails
                                      where l.leadID == newLead.leadID
                                      select l).ToList();

                    if (LeadEmails.FirstOrDefault(x => x.email == filteredEmail) == null)
                    {
                        tblLeadEmails newEmail = new tblLeadEmails();
                        newEmail.email = filteredEmail;
                        newEmail.leadID = newLead.leadID;
                        newEmail.main = LeadEmails.Count() > 0 ? false : true;
                        newEmail.dateLastModification = DateTime.Now;
                        db.tblLeadEmails.AddObject(newEmail);
                    }
                }

                //ubicación por el teléfono
                if (mobile != null && mobile.Length >= 10)
                {
                    if (!matchMobile)
                    {
                        var LeadPhones = (from l in db.tblPhones
                                          where l.leadID == newLead.leadID
                                          select l).ToList();

                        if (LeadPhones.FirstOrDefault(x => x.phone == filteredMobile) == null)
                        {
                            tblPhones newPhone = new tblPhones();
                            newPhone.phone = filteredMobile;
                            newPhone.phoneTypeID = 1;
                            newPhone.doNotCall = false;
                            newPhone.leadID = newLead.leadID;
                            newPhone.main = LeadPhones.Count() > 0 ? false : true;
                            newPhone.dateLastModification = DateTime.Now;
                            db.tblPhones.AddObject(newPhone);
                            db.SaveChanges();

                            string areaCode = filteredPhone.Substring(0, 3);
                            var Area = (from a in db.tblAreaCodes
                                        join ac in db.tblAreas on a.areaID equals ac.areaID
                                        into a_ac
                                        from ac in a_ac.DefaultIfEmpty()
                                        join s in db.tblStates on ac.stateID equals s.stateID
                                        into a_s
                                        from s in a_s.DefaultIfEmpty()
                                        where a.code == areaCode
                                        select new
                                        {
                                            ac.area,
                                            ac.stateID,
                                            s.state,
                                            s.countryID
                                        }).FirstOrDefault();

                            if (Area != null)
                            {
                                newLead.countryID = Area.countryID;
                                newLead.stateID = Area.stateID;
                                newLead.state = Area.state;
                                newLead.city = Area.area;
                            }
                        }
                    }
                }

                if (phone != null && phone.Length >= 10)
                {
                    if (!matchPhone)
                    {
                        var LeadPhones = (from l in db.tblPhones
                                          where l.leadID == newLead.leadID
                                          select l).ToList();

                        if (LeadPhones.FirstOrDefault(x => x.phone == filteredPhone) == null)
                        {
                            tblPhones newPhone = new tblPhones();
                            newPhone.phone = filteredPhone;
                            newPhone.phoneTypeID = 4;
                            newPhone.doNotCall = false;
                            newPhone.leadID = newLead.leadID;
                            newPhone.main = LeadPhones.Count() > 0 ? false : true;
                            newPhone.dateLastModification = DateTime.Now;
                            db.tblPhones.AddObject(newPhone);

                            string areaCode = filteredPhone.Substring(0, 3);
                            var Area = (from a in db.tblAreaCodes
                                        join ac in db.tblAreas on a.areaID equals ac.areaID
                                        into a_ac
                                        from ac in a_ac.DefaultIfEmpty()
                                        join s in db.tblStates on ac.stateID equals s.stateID
                                        into a_s
                                        from s in a_s.DefaultIfEmpty()
                                        where a.code == areaCode
                                        select new
                                        {
                                            ac.area,
                                            ac.stateID,
                                            s.state,
                                            s.countryID
                                        }).FirstOrDefault();

                            if (Area != null)
                            {
                                newLead.countryID = Area.countryID;
                                newLead.stateID = Area.stateID;
                                newLead.state = Area.state;
                                newLead.city = Area.area;
                            }
                        }
                    }
                }

                //actualizar interacción de lead
                tblInteractions newInteraction = new tblInteractions();
                newInteraction.leadID = newLead.leadID;
                newInteraction.interactionTypeID = 12; //form
                newInteraction.interactionComments = "Customer registered through a Facebook Ads campaign.";
                if (data["age"] != null)
                {
                    newInteraction.interactionComments += "<br />Age: " + (string)data["age"] + ".";
                }
                if (data["gender"] != null)
                {
                    newInteraction.interactionComments += "<br />Gender: " + (string)data["gender"] + ".";
                }
                if (data["destination"] != null)
                {
                    newInteraction.interactionComments += "<br />Destination: " + (string)data["destination"] + ".";
                }
                if (data["platform"] != null)
                {
                    newInteraction.interactionComments += "<br />Platform: " + (string)data["platform"] + ".";
                }
                if (data["campaignName"] != null)
                {
                    newInteraction.interactionComments += "<br />Campaign: " + (string)data["campaignName"] + ".";
                }
                if (data["adsetName"] != null)
                {
                    newInteraction.interactionComments += "<br />Ad Set: " + (string)data["adsetName"] + ".";
                }
                newInteraction.savedByUserID = new Guid("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                newInteraction.dateSaved = DateTime.Now;
                newInteraction.bookingStatusID = newLead.bookingStatusID;
                db.tblInteractions.AddObject(newInteraction);

                //detalles de campaña
                tblLeadsCampaignDetails newCampaignDetail = new tblLeadsCampaignDetails();
                newCampaignDetail.lead_campaignDetailID = Guid.NewGuid();
                newCampaignDetail.dateSaved = DateTime.Now;
                newCampaignDetail.leadID = newLead.leadID;
                if (data["platform"] != null)
                {
                    newCampaignDetail.platform = (string)data["platform"];
                }
                if (data["campaignID"] != null)
                {
                    newCampaignDetail.campaignID = (string)data["campaignID"];
                }
                if (data["campaignName"] != null)
                {
                    newCampaignDetail.campaignName = (string)data["campaignName"];
                }
                if (data["adsetID"] != null)
                {
                    newCampaignDetail.adsetID = (string)data["adsetID"];
                }
                if (data["adsetName"] != null)
                {
                    newCampaignDetail.adsetName = (string)data["adsetName"];
                }
                if (data["adID"] != null)
                {
                    newCampaignDetail.adID = (string)data["adID"];
                }
                if (data["adName"] != null)
                {
                    newCampaignDetail.adName = (string)data["adName"];
                }
                if (data["formID"] != null)
                {
                    newCampaignDetail.formID = (string)data["formID"];
                }
                if (data["formName"] != null)
                {
                    newCampaignDetail.formName = (string)data["formName"];
                }
                if (data["inputDateTime"] != null)
                {
                    newCampaignDetail.inputDateTime = (DateTime?)data["inputDateTime"];
                }

                db.tblLeadsCampaignDetails.AddObject(newCampaignDetail);


                db.SaveChanges();
            }
            catch (Exception e)
            {
                newWHL.error = e.Message;
                newWHL.stackTrace = e.StackTrace;
                db.SaveChanges();
            }
        }
        else if (context.Id == "newlead")
        {
            tblWebhooksLog newWHL = new tblWebhooksLog();
            Guid? leadIDInProgress = null;
            try
            {
                //buscar el webhook
                Guid webhookID = (Guid)data["webhookID"];
                var webhook = (from w in db.tblWebhooks
                               where w.webhookID == webhookID
                               select w).FirstOrDefault();

                if (webhook != null)
                {
                    newWHL.webhook = webhook.webhook;
                    newWHL.webhookID = webhookID;
                    newWHL.dateSaved = DateTime.Now;
                    newWHL.urlReferrer = urlReferrer;
                    newWHL.json = data.ToString(Newtonsoft.Json.Formatting.None);
                    db.tblWebhooksLog.AddObject(newWHL);
                    db.SaveChanges();

                    //limpiar datos de contacto del lead
                    Guid? leadID = null;
                    string email = null;
                    string phone = null;
                    string mobile = null;
                    string filteredPhone = "";
                    string filteredMobile = "";
                    string filteredEmail = "";
                    bool matchPhone = false;
                    bool matchMobile = false;
                    bool matchEmail = false;

                    if (data["email"] != null && (string)data["email"] != "")
                    {
                        email = (string)data["email"];
                        filteredEmail = email.Trim();
                    }
                    if (data["phone"] != null && (string)data["phone"] != "")
                    {
                        phone = (string)data["phone"];
                        filteredPhone = Regex.Replace(phone, @"[^\d]", "");
                        if (filteredPhone.Length > 10)
                        {
                            filteredPhone = filteredPhone.Substring(filteredPhone.Length - 10);
                        }
                    }
                    if (data["mobile"] != null && (string)data["mobile"] != "")
                    {
                        mobile = (string)data["mobile"];
                        filteredMobile = Regex.Replace(mobile, @"[^\d]", "");
                        if (filteredMobile.Length > 10)
                        {
                            filteredMobile = filteredMobile.Substring(filteredMobile.Length - 10);
                        }
                    }

                    //definir terminal a la cual se envía
                    long? terminalid = null;
                    if (webhook.terminalID != null)
                    {
                        terminalid = (long)webhook.terminalID;
                    }

                    if (terminalid == null)
                    {
                        //es un webhook sin terminal, hay que definir a cual terminal le toca de las disponibles
                        var AvailableTerminals = (from a in db.tblAssignationSettings
                                                  where a.leadTypeID == webhook.leadTypeID
                                                  select a.terminalID).ToList();

                        //primero buscar si no existe ya en alguna terminal
                        if (email != null || phone != null || mobile != null)
                        {
                            WebhooksViewModel.PhoneTerminal PhoneLeadID = new WebhooksViewModel.PhoneTerminal();
                            if (filteredPhone != null && filteredPhone.Length >= 10)
                            {
                                PhoneLeadID = (from p in db.tblPhones
                                               join l in db.tblLeads
                                               on p.leadID equals l.leadID
                                               where p.phone.Contains(filteredPhone)
                                               && AvailableTerminals.Contains(l.initialTerminalID)
                                               orderby p.phoneID descending
                                               select new WebhooksViewModel.PhoneTerminal()
                                               {
                                                   LeadID = p.leadID,
                                                   TerminalID = l.initialTerminalID
                                               }).FirstOrDefault();
                            }

                            if (PhoneLeadID != null && PhoneLeadID.LeadID != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                leadID = PhoneLeadID.LeadID;
                                terminalid = PhoneLeadID.TerminalID;
                                matchPhone = true;
                            }
                            else
                            {
                                if (filteredMobile != null && filteredMobile.Length >= 10)
                                {
                                    PhoneLeadID = (from p in db.tblPhones
                                                   join l in db.tblLeads
                                                   on p.leadID equals l.leadID
                                                   where p.phone.Contains(filteredMobile)
                                                   && AvailableTerminals.Contains(l.initialTerminalID)
                                                   orderby p.phoneID descending
                                                   select new WebhooksViewModel.PhoneTerminal()
                                                   {
                                                       LeadID = p.leadID,
                                                       TerminalID = l.initialTerminalID
                                                   }).FirstOrDefault();
                                }
                                if (PhoneLeadID != null && PhoneLeadID.LeadID != new Guid("00000000-0000-0000-0000-000000000000"))
                                {
                                    leadID = PhoneLeadID.LeadID;
                                    terminalid = PhoneLeadID.TerminalID;
                                    matchMobile = true;
                                }
                                else
                                {
                                    var EmailLeadID = (from e in db.tblLeadEmails
                                                       join l in db.tblLeads
                                                       on e.leadID equals l.leadID
                                                       into e_l
                                                       from l in e_l.DefaultIfEmpty()
                                                       where e.email.Contains(filteredEmail)
                                                       && AvailableTerminals.Contains(l.initialTerminalID)
                                                       select new
                                                       {
                                                           e.leadID,
                                                           l.initialTerminalID
                                                       }).FirstOrDefault();

                                    if (EmailLeadID != null && EmailLeadID.leadID != new Guid("00000000-0000-0000-0000-000000000000"))
                                    {
                                        leadID = EmailLeadID.leadID;
                                        terminalid = EmailLeadID.initialTerminalID;
                                        matchEmail = true;
                                    }
                                }
                            }
                        }


                        //si no existe, entonces revisamos a cual terminal asignarlo
                        if (terminalid == null)
                        {
                            //definir terminales posibles
                            var PosibleTerminals = (from x in db.tblAssignationSettings
                                                    where x.leadTypeID == webhook.leadTypeID
                                                    && x.distribute == true
                                                    && x.active == true
                                                    select x.terminalID).ToList();
                            //obtener fecha de reset 
                            var assignationSettings = (from r in db.tblAssignationSettings
                                                       where PosibleTerminals.Contains(r.terminalID)
                                                       && r.leadTypeID == webhook.leadTypeID
                                                       select new
                                                       {
                                                           r.terminalID,
                                                           r.dateReset,
                                                           r.sharedPercentage
                                                       }).ToList();


                            //buscar cuantos leads de ese leadtype hay en las terminales que participan
                            DateTime dateReset = assignationSettings.FirstOrDefault().dateReset;
                            //DateTime dateReset = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                            var TerminalLeads = (from x in db.tblLeads
                                                 where PosibleTerminals.Contains(x.initialTerminalID)
                                                 && x.leadTypeID == webhook.leadTypeID
                                                 && x.inputDateTime >= dateReset
                                                 group x by x.initialTerminalID
                                                into grp
                                                 select new
                                                 {
                                                     terminalID = grp.Key,
                                                     count = grp.Distinct().Count(),
                                                 }).ToList();

                            List<WebhooksViewModel.TerminalCounter> Terminals = new List<WebhooksViewModel.TerminalCounter>();
                            decimal terminalsTotal = TerminalLeads.Sum(x => x.count);
                            foreach (var t in PosibleTerminals)
                            {
                                var terminalLeads = TerminalLeads.FirstOrDefault(x => x.terminalID == t);
                                Terminals.Add(new WebhooksViewModel.TerminalCounter()
                                {
                                    TerminalID = t,
                                    Count = (terminalLeads != null ? TerminalLeads.FirstOrDefault(x => x.terminalID == t).count : 0),
                                    CurrentPercentage = (terminalsTotal > 0 ? decimal.Round((terminalLeads != null ? TerminalLeads.FirstOrDefault(x => x.terminalID == t).count : 0) * 100 / terminalsTotal, 2) : 0),
                                    SharedPercentage = assignationSettings.FirstOrDefault(x => x.terminalID == t).sharedPercentage
                                });
                            }

                            foreach (var t in Terminals)
                            {
                                if (t.SharedPercentage != null)
                                {
                                    t.Difference = (decimal)t.SharedPercentage - t.CurrentPercentage;
                                }
                                else
                                {
                                    t.Difference = terminalsTotal - t.Count;
                                }
                            }

                            //ordernar de mayor a menor y tomar la primera
                            terminalid = Terminals.OrderByDescending(x => x.Difference).ThenBy(x => x.TerminalID).Select(x => x.TerminalID).FirstOrDefault();
                        }
                    }

                    //obtener datos del lead
                    tblLeads newLead = new tblLeads();

                    if ((email != null || phone != null || mobile != null) && leadID == null)
                    {
                        Guid? PhoneLeadID = null;
                        if (filteredPhone != null && filteredPhone.Length >= 10)
                        {
                            PhoneLeadID = (from p in db.tblPhones
                                           join l in db.tblLeads
                                           on p.leadID equals l.leadID
                                           where p.phone.Contains(filteredPhone)
                                           && l.initialTerminalID == terminalid
                                           orderby p.phoneID descending
                                           select p.leadID).FirstOrDefault();
                        }

                        if (PhoneLeadID != null && PhoneLeadID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            leadID = PhoneLeadID;
                            matchPhone = true;
                        }
                        else
                        {
                            if (filteredMobile != null && filteredMobile.Length >= 10)
                            {
                                PhoneLeadID = (from p in db.tblPhones
                                               join l in db.tblLeads
                                               on p.leadID equals l.leadID
                                               where p.phone.Contains(filteredMobile)
                                               && l.initialTerminalID == terminalid
                                               orderby p.phoneID descending
                                               select p.leadID).FirstOrDefault();
                            }
                            if (PhoneLeadID != null && PhoneLeadID != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                leadID = PhoneLeadID;
                                matchMobile = true;
                            }
                            else
                            {
                                var EmailLeadID = (from e in db.tblLeadEmails
                                                   join l in db.tblLeads
                                                   on e.leadID equals l.leadID
                                                   where e.email.ToLower().Contains(filteredEmail)
                                                   && l.initialTerminalID == terminalid
                                                   select e.leadID).FirstOrDefault();

                                if (EmailLeadID != null && EmailLeadID != new Guid("00000000-0000-0000-0000-000000000000"))
                                {
                                    leadID = EmailLeadID;
                                    matchEmail = true;
                                }
                            }
                        }
                    }

                    if (leadID == null)
                    {
                        //new lead
                        newLead.leadID = Guid.NewGuid();
                        newLead.firstName = (string)data["firstName"];
                        newLead.lastName = (string)data["lastName"];
                        newLead.initialTerminalID = (long)terminalid;
                        newLead.terminalID = (long)terminalid;
                        if (data["inputDateTime"] != null)
                        {
                            newLead.inputDateTime = DateTime.Now;
                            newLead.dateCollected = DateTime.Parse((string)data["inputDateTime"]);
                        }
                        else
                        {
                            newLead.inputDateTime = DateTime.Now;
                        }
                        if (data["referenceID"] != null)
                        {
                            newLead.referenceID = (string)data["referenceID"];
                        }
                        if (data["tags"] != null)
                        {
                            newLead.tags = (string)data["tags"];
                        }
                        if (data["timeToReach"] != null)
                        {
                            newLead.timeToReach = (string)data["timeToReach"];
                        }
                        if (data["destinationID"] != null)
                        {
                            newLead.interestedInDestinationID = (int)data["destinationID"];
                        }

                        //variables del webhook
                        newLead.leadTypeID = webhook.leadTypeID;
                        newLead.bookingStatusID = 19; //Not Contacted
                        newLead.leadSourceID = webhook.leadSourceID;
                        newLead.leadSourceChannelID = webhook.leadSourceChannelID;
                        newLead.inputMethodID = 7; //webhook
                        newLead.submissionForm = true;
                        newLead.leadStatusID = 29;

                        newLead.inputByUserID = new Guid("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                        newLead.deleted = false;
                        newLead.isTest = false;

                        //asignación
                        int assignationType = 0;
                        List<AssignationItem> Assignations = new List<AssignationItem>();
                        if (phone != null || email != null || mobile != null)
                        {
                            if (webhook.assignation == null)
                            {
                                //automática
                                assignationType = 1;
                            }
                            else
                            {
                                Guid[] assignations = webhook.assignation.Split(',').Select(x => Guid.Parse(x)).ToArray();
                                if (Array.IndexOf(assignations, new Guid("00000000-0000-0000-0000-000000000000")) >= 0)
                                {
                                    //no asignar
                                    assignationType = 2;
                                }
                                else
                                {
                                    //asignación definida
                                    assignationType = 3;

                                    //crear el arreglo de asignaciones
                                    foreach (var uID in assignations)
                                    {
                                        if (uID != new Guid("00000000-0000-0000-0000-000000000000"))
                                        {
                                            Assignations.Add(new AssignationItem()
                                            {
                                                UserID = uID,
                                                Assigned = 0
                                            });
                                        }
                                    }
                                }
                            }

                            Guid? assignateToUserID = null;
                            switch (assignationType)
                            {
                                case 1:
                                    //requiere asignación automática
                                    assignateToUserID = NetCenterDataModel.GetUserForAssignation(newLead.leadTypeID, newLead.terminalID);
                                    break;
                                case 3:
                                    assignateToUserID = null;
                                    var userToAssign = Assignations.OrderBy(x => x.Assigned).FirstOrDefault();
                                    if (userToAssign != null)
                                    {
                                        assignateToUserID = userToAssign.UserID;
                                        userToAssign.Assigned++;
                                    }
                                    break;
                            }
                            if (assignateToUserID != null)
                            {
                                newLead.assignationDate = DateTime.Now;
                                newLead.assignedToUserID = assignateToUserID;

                                tblLeadAssignations newAssignation = new tblLeadAssignations();
                                newAssignation.leadID = newLead.leadID;
                                newAssignation.assignedToUserID = (Guid)assignateToUserID;
                                newAssignation.dateSaved = DateTime.Now;
                                newAssignation.assignedByUserID = new Guid("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                                newLead.tblLeadAssignations.Add(newAssignation);
                            }
                        }

                        db.tblLeads.AddObject(newLead);
                        db.SaveChanges();
                        leadIDInProgress = newLead.leadID;
                    }
                    else
                    {
                        newLead = (from l in db.tblLeads
                                   where l.leadID == leadID
                                   select l).FirstOrDefault();
                        //notificar nuevo registro del lead.
                        if (newLead.assignedToUserID != null)
                        {
                            HttpWebRequest http = (HttpWebRequest)WebRequest.Create("https://eplat.com/notifications/sendservernotification?title=Existing+Lead&description=An+existing+lead+assigned+to+you+has+registered+on+Facebook.&notificationTypeID=1&forUserID=" + newLead.assignedToUserID.ToString() + "&terminalID=" + newLead.initialTerminalID.ToString() + "&button=Open+Lead&onlyRealTime=true&action=true&url=/profile%23leadid%3D" + newLead.leadID.ToString());
                            WebResponse response = http.GetResponse();
                        }
                    }

                    //asignación a Webhook log
                    newWHL.leadID = newLead.leadID;

                    //email & phone
                    //Verificar que el lead no tenga ese email y/o teléfono

                    if (!matchEmail)
                    {
                        var LeadEmails = (from l in db.tblLeadEmails
                                          where l.leadID == newLead.leadID
                                          select l).ToList();

                        if (LeadEmails.FirstOrDefault(x => x.email == filteredEmail) == null)
                        {
                            tblLeadEmails newEmail = new tblLeadEmails();
                            newEmail.email = filteredEmail;
                            newEmail.leadID = newLead.leadID;
                            newEmail.main = LeadEmails.Count() > 0 ? false : true;
                            newEmail.dateLastModification = DateTime.Now;
                            db.tblLeadEmails.AddObject(newEmail);
                        }
                    }

                    //ubicación por el teléfono
                    if (mobile != null && mobile.Length >= 10)
                    {
                        if (!matchMobile)
                        {
                            var LeadPhones = (from l in db.tblPhones
                                              where l.leadID == newLead.leadID
                                              select l).ToList();

                            if (LeadPhones.FirstOrDefault(x => x.phone == filteredMobile) == null)
                            {
                                tblPhones newPhone = new tblPhones();
                                newPhone.phone = filteredMobile;
                                newPhone.phoneTypeID = 1;
                                newPhone.doNotCall = false;
                                newPhone.leadID = newLead.leadID;
                                newPhone.main = LeadPhones.Count() > 0 ? false : true;
                                newPhone.dateLastModification = DateTime.Now;
                                db.tblPhones.AddObject(newPhone);
                                db.SaveChanges();

                                string areaCode = filteredPhone.Substring(0, 3);
                                var Area = (from a in db.tblAreaCodes
                                            join ac in db.tblAreas on a.areaID equals ac.areaID
                                            into a_ac
                                            from ac in a_ac.DefaultIfEmpty()
                                            join s in db.tblStates on ac.stateID equals s.stateID
                                            into a_s
                                            from s in a_s.DefaultIfEmpty()
                                            where a.code == areaCode
                                            select new
                                            {
                                                ac.area,
                                                ac.stateID,
                                                s.state,
                                                s.countryID
                                            }).FirstOrDefault();

                                if (Area != null)
                                {
                                    newLead.countryID = Area.countryID;
                                    newLead.stateID = Area.stateID;
                                    newLead.state = Area.state;
                                    newLead.city = Area.area;
                                }
                            }
                        }
                    }


                    if (phone != null && phone.Length >= 10)
                    {
                        if (!matchPhone)
                        {
                            var LeadPhones = (from l in db.tblPhones
                                              where l.leadID == newLead.leadID
                                              select l).ToList();

                            if (LeadPhones.FirstOrDefault(x => x.phone == filteredPhone) == null)
                            {
                                tblPhones newPhone = new tblPhones();
                                newPhone.phone = filteredPhone;
                                newPhone.phoneTypeID = 4;
                                newPhone.doNotCall = false;
                                newPhone.leadID = newLead.leadID;
                                newPhone.main = LeadPhones.Count() > 0 ? false : true;
                                newPhone.dateLastModification = DateTime.Now;
                                db.tblPhones.AddObject(newPhone);

                                string areaCode = filteredPhone.Substring(0, 3);
                                var Area = (from a in db.tblAreaCodes
                                            join ac in db.tblAreas on a.areaID equals ac.areaID
                                            into a_ac
                                            from ac in a_ac.DefaultIfEmpty()
                                            join s in db.tblStates on ac.stateID equals s.stateID
                                            into a_s
                                            from s in a_s.DefaultIfEmpty()
                                            where a.code == areaCode
                                            select new
                                            {
                                                ac.area,
                                                ac.stateID,
                                                s.state,
                                                s.countryID
                                            }).FirstOrDefault();

                                if (Area != null)
                                {
                                    newLead.countryID = Area.countryID;
                                    newLead.stateID = Area.stateID;
                                    newLead.state = Area.state;
                                    newLead.city = Area.area;
                                }
                            }
                        }
                    }

                    //actualizar interacción de lead
                    tblInteractions newInteraction = new tblInteractions();
                    newInteraction.leadID = newLead.leadID;
                    newInteraction.interactionTypeID = 12; //form
                    newInteraction.interactionComments = "Customer registered through " + webhook.webhook;
                    if (data["arrivalDate"] != null && (string)data["arrivalDate"] != "")
                    {
                        newInteraction.interactionComments += "<br />Arrival: " + (string)data["arrivalDate"] + ".";
                    }
                    if (data["departureDate"] != null && (string)data["departureDate"] != "")
                    {
                        newInteraction.interactionComments += "<br />Departure: " + (string)data["departureDate"] + ".";
                    }
                    if (data["age"] != null)
                    {
                        newInteraction.interactionComments += "<br />Age: " + (string)data["age"] + ".";
                    }
                    if (data["gender"] != null)
                    {
                        newInteraction.interactionComments += "<br />Gender: " + (string)data["gender"] + ".";
                    }
                    if (data["destination"] != null && (string)data["destination"] != "")
                    {
                        newInteraction.interactionComments += "<br />Destination: " + (string)data["destination"] + ".";
                    }
                    if (data["platform"] != null)
                    {
                        newInteraction.interactionComments += "<br />Platform: " + (string)data["platform"] + ".";
                    }
                    if (data["campaignName"] != null)
                    {
                        newInteraction.interactionComments += "<br />Campaign: " + (string)data["campaignName"] + ".";
                    }
                    if (data["adsetName"] != null)
                    {
                        newInteraction.interactionComments += "<br />Ad Set: " + (string)data["adsetName"] + ".";
                    }
                    if (data["notes"] != null)
                    {
                        newInteraction.interactionComments += "<br />" + (string)data["notes"];
                    }
                    newInteraction.savedByUserID = new Guid("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                    newInteraction.dateSaved = DateTime.Now;

                    newInteraction.bookingStatusID = newLead.bookingStatusID;

                    db.tblInteractions.AddObject(newInteraction);

                    //detalles de campaña
                    if ((string)data["platform"] == "fbk" && newLead.leadSourceID != 50)
                    {
                        newLead.leadSourceID = 50;
                        newLead.leadSourceChannelID = 5;
                    }

                    if (data["platform"] != null)
                    {
                        tblLeadsCampaignDetails newCampaignDetail = new tblLeadsCampaignDetails();
                        newCampaignDetail.lead_campaignDetailID = Guid.NewGuid();
                        newCampaignDetail.dateSaved = DateTime.Now;
                        newCampaignDetail.leadID = newLead.leadID;

                        newCampaignDetail.platform = (string)data["platform"];

                        if (data["campaignID"] != null)
                        {
                            newCampaignDetail.campaignID = (string)data["campaignID"];
                        }
                        if (data["campaignName"] != null)
                        {
                            newCampaignDetail.campaignName = (string)data["campaignName"];
                        }
                        if (data["adsetID"] != null)
                        {
                            newCampaignDetail.adsetID = (string)data["adsetID"];
                        }
                        if (data["adsetName"] != null)
                        {
                            newCampaignDetail.adsetName = (string)data["adsetName"];
                        }
                        if (data["adID"] != null)
                        {
                            newCampaignDetail.adID = (string)data["adID"];
                        }
                        if (data["adName"] != null)
                        {
                            newCampaignDetail.adName = (string)data["adName"];
                        }
                        if (data["formID"] != null)
                        {
                            newCampaignDetail.formID = (string)data["formID"];
                        }
                        if (data["formName"] != null)
                        {
                            newCampaignDetail.formName = (string)data["formName"];
                        }
                        if (data["inputDateTime"] != null)
                        {
                            newCampaignDetail.inputDateTime = DateTime.Parse((string)data["inputDateTime"]);
                        }

                        db.tblLeadsCampaignDetails.AddObject(newCampaignDetail);
                    }

                    db.SaveChanges();

                    //envío de correo al cliente
                    /*
                    if(webhook.triggerEvent && filteredEmail != null && filteredEmail != "")
                    {
                        var NotificationQ = (from e in db.tblEmailNotifications
                                             where e.eventID == 50
                                             && e.terminalID == newLead.terminalID
                                             && e.tblEmails.culture == "en-US"
                                             && e.tblEmailNotifications_PointsOfSale.Count() == 0
                                             select new { e.emailNotificationID }).FirstOrDefault();

                        if (NotificationQ != null)
                        {
                            System.Net.Mail.MailMessage emailObj = EmailNotifications.GetEmailByNotification((int)NotificationQ.emailNotificationID);
                            emailObj.Body = emailObj.Body.Replace("$FirstName", newLead.firstName);
                            emailObj.Body = emailObj.Body.Replace("$LastName", newLead.lastName);
                            emailObj.Body = emailObj.Body.Replace("$Email", filteredEmail);

                            if(newLead.assignedToUserID != null)
                            {

                            }


                            emailObj.Body = emailObj.Body.Replace("Agent", filteredEmail);

                            emailObj.To.Add(filteredEmail);

                            ApiReferralsDataModel.SendEmail(emailObj);
                            //Utils.EmailNotifications.Send(email);
                            //EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                        }
                    }   
                    */
                }
            }
            catch (Exception e)
            {
                newWHL.error = e.Message;
                newWHL.stackTrace = e.StackTrace;

                //eliminar lead incompleto
                var leadToDelete = db.tblLeads.FirstOrDefault(x => x.leadID == leadIDInProgress);
                if(leadToDelete != null)
                {
                    db.tblLeads.DeleteObject(leadToDelete);
                }                

                db.SaveChanges();
            }
        }
        else if (context.Id == "sms_voxtelesys")
        {
            string type = (string)data["type"];

            if (type == "mo")  //Inbound message
            {
                DateTime ts = DateTime.Now;

                tblSmsMessages newMsg = new tblSmsMessages();

                string last10From = (string)data["from"];
                if (last10From.Length > 10)
                {
                    last10From = last10From.Substring(last10From.Length - 10);
                }

                string last10To = (string)data["to"];
                if (last10To.Length > 10)
                {
                    last10To = last10To.Substring(last10To.Length - 10);
                }

                newMsg.smsMessageID = Guid.NewGuid();
                newMsg.fromNumber = last10From;
                newMsg.toNumber = last10To;
                newMsg.body = (string)data["body"];
                newMsg.io = true; //true|1 for incoming message / false|0 for outgoing
                newMsg.dateSaved = ts;
                newMsg.received_at = (string)data["received_at"];
                newMsg.response = data.ToString();
                db.tblSmsMessages.AddObject(newMsg);
                db.SaveChanges();

                //PENDIENTE NOTIFICAR

            }
            else if (type == "per_recipient") //Per recipient
            {
                string batchID = "";
                batchID = (string)data["batch_id"];

                tblSmsEvents newEvent = new tblSmsEvents();
                newEvent.smsEventDateTime = DateTime.Now;
                newEvent.eventData = data.ToString();
                newEvent.batch_id = batchID;
                db.tblSmsEvents.AddObject(newEvent);
                db.SaveChanges();

                //update status on tblSmsMessages
                var msg = (from x in db.tblSmsMessages where x.api_id == batchID select x).FirstOrDefault();

                if (msg != null)
                {
                    msg.status = (string)data["status"];
                    //msg.response += "," + data.ToString();
                }

                db.SaveChanges();
            }
            else if (type == "summary") //Batch summary 
            {
                tblSmsEvents newEvent = new tblSmsEvents();

                newEvent.smsEventDateTime = DateTime.Now;
                newEvent.eventData = data.ToString();
                db.tblSmsEvents.AddObject(newEvent);
                db.SaveChanges();
            }
            else if (type == "detailed")//Batch detailed
            {
                tblSmsEvents newEvent = new tblSmsEvents();

                newEvent.smsEventDateTime = DateTime.Now;
                newEvent.eventData = data.ToString();
                db.tblSmsEvents.AddObject(newEvent);
                db.SaveChanges();
            }
            else
            {
                tblSmsEvents newEvent = new tblSmsEvents();

                newEvent.smsEventDateTime = DateTime.Now;
                newEvent.eventData = "ELSE: " + data.ToString();
                db.tblSmsEvents.AddObject(newEvent);
                db.SaveChanges();
            }



        }
        return Task.FromResult(true);
    }
}