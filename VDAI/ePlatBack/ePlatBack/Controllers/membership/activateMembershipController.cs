using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ePlatBack.Models.DataModels;
using System.Web.Mvc;
using ePlatBack.Models;
using ePlatBack.Models.Utils;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ePlatBack.Controllers.membership.activateMembership
{
    public class activateMembershipController : Controller
    {
        
        ePlatEntities db = new ePlatEntities();
        Memberships memberships = new Memberships();


        public static System.Net.Mail.MailMessage GetEmail(int emailid, int code, string name, string lastname, DateTime activation, DateTime duedate,
            string payment, int price, string email, string emailtosend , string password , string paidmsg, Guid leadID, bool track)
        {

            var cod = Convert.ToString(code);
            var activa = activation.ToShortDateString();
            var due = duedate.ToShortDateString();
            var precio = Convert.ToString(price);
            ePlatEntities db = new ePlatEntities();
            var emailnotifications = (from emails in db.tblEmailNotifications
                         where emails.emailNotificationID == emailid
                         select emails
                         ).FirstOrDefault();

            var mail = (from emails in db.tblEmails
                          where emails.emailID == emailnotifications.emailID
                          select emails
                         ).FirstOrDefault();
            Guid transactionID = Guid.NewGuid();
            System.Net.Mail.MailMessage emailObj = null;
            emailObj = new System.Net.Mail.MailMessage();
            emailObj.From = new System.Net.Mail.MailAddress(mail.sender, mail.alias);
            emailObj.Subject = mail.subject;
            var tempBody = mail.content_.Replace("$name", name).Replace("$lastname", lastname)
                .Replace("$code", cod).Replace("$activation", activa).Replace("$duedate", due).Replace("$payment", payment)
                .Replace("$email", email).Replace("$price", precio).Replace("$password", password).Replace("$paidmsg", paidmsg);

            if (track)
            {
                emailObj.Body = ePlatBack.Models.Utils.EmailNotifications.InsertTrackerEmailLog(tempBody, transactionID.ToString());

            }else
            {
                emailObj.Body = tempBody;
            }


            emailObj.IsBodyHtml = true;
            emailObj.Priority = System.Net.Mail.MailPriority.Normal;
            emailObj.Bcc.Add(emailtosend);
            emailObj.Bcc.Add(emailnotifications.replyTo);


            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.villagroup.com";
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("info@sensesofmexico.com", "In43fo19@");
            smtp.Credentials = credentials;
            smtp.Port = 2525;
                smtp.Send(emailObj);
            emailObj.Body = tempBody;
            if (track)
            {
                tblEmailNotificationLogs query = new tblEmailNotificationLogs();
                query.leadID = leadID;
                query.trackingID = transactionID;
                query.subject = "Subscription Activated";
                query.emailPreviewJson = JsonConvert.SerializeObject(emailObj);
                query.sentByUserID = session.UserID;
                query.dateSent = DateTime.Now;
                query.emailNotificationID = 247;
                query.trackingID = transactionID;
                db.tblEmailNotificationLogs.AddObject(query);
                db.SaveChanges();

            }
            
            return emailObj;
        }
        public static UserSession session = new UserSession();

        public ActionResult Index()
        {
            return View("~/Views/cardsManagement/activateMembership.cshtml");
        }

        //obtiene los folios inactivos de las membresias 
        public JsonResult getInactivesCodes()
        {
            var codes = (from ambassadors in db.tblMembershipCards
                           join users in db.tblUserProfiles on ambassadors.userID equals users.userID
                           where ambassadors.Status == "Inactive"
                           orderby ambassadors.Code ascending
                           select new
                           {
                               name = users.firstName + " " + users.lastName,
                               userID = ambassadors.userID,
                               label = ambassadors.Code,
                               value = ambassadors.MembershipCardID,
                               
                           }).ToList();
            
            return Json(codes, JsonRequestBehavior.AllowGet);
        }

        //obtiene las locaciones vinculadas a senses of mexico 
        public JsonResult getLocations()
        {
            var allocations = (from locations in db.tblLocations
                         where locations.terminalID == 62
                         select new
                         {
                             label = locations.location,
                             value = locations.locationID,

                         }).ToList();
            return Json(allocations, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCountries()
        {
            var countries = (from c in db.tblCountries
                             select new
                             {
                                 label = c.country,
                                 value = c.countryID
                             }
                             ).ToList();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult activateMembership(int code, Guid ambassador, int location, string name, string lastname, string email, string payment,
            DateTime activation, DateTime duedate, int price, int countryid, string password, Guid leadid, string paidmsg, string phone, int paymentType)
        {
            phone = Regex.Replace(phone, @"[^\d]", "");
            Guid id = leadid;
            tblLeads newlead = new tblLeads();

            var exists = (from l in db.tblLeads
                          where l.leadID == id && l.terminalID == 62
                          select l
                                     ).FirstOrDefault();

            var hasEmail = (from e in db.tblLeads
                            join c in db.tblLeadEmails on e.leadID equals c.leadID
                            where c.email == email && e.terminalID == 62
                            select e.leadID
                            ).FirstOrDefault();
            if (hasEmail != new Guid("00000000-0000-0000-0000-000000000000"))
            {
                exists = (from l in db.tblLeads
                          where l.leadID == hasEmail
                          select l
                                      ).FirstOrDefault();
            }else
            {
                if (phone.Length >= 10)
                {
                    phone = phone.Substring(phone.Length - 10);
                    var hasPhone = (from e in db.tblLeads
                                    join c in db.tblPhones on e.leadID equals c.leadID
                                    where c.phone == phone && e.terminalID == 62
                                    select e.leadID
                                            ).FirstOrDefault();
                    if (hasPhone != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        exists = (from l in db.tblLeads
                                  where l.leadID == hasPhone
                                  select l
                                          ).FirstOrDefault();
                    }
                }
                
                
            }




            if (exists == null)
            {
                newlead.leadID = id;
                newlead.initialTerminalID = 62;
                newlead.terminalID = 62;
                newlead.firstName = name;
                newlead.lastName = lastname;
                newlead.countryID = countryid;
                newlead.inputByUserID = session.UserID;
                newlead.inputDateTime = activation;
                newlead.isTest = false;
                newlead.deleted = false;
                newlead.tags ="#ClassicSuscriber";
                newlead.leadTypeID = 59;
                newlead.leadSourceID = 55;
                newlead.inputMethodID = 8;
                newlead.leadStatusID = 2;
                newlead.leadSourceChannelID = 11;
                newlead.interestedInDestinationID = 1;
                db.tblLeads.AddObject(newlead);
            }else
            {
                id = exists.leadID;
                newlead = exists;          
                //newlead.firstName = name;
                //newlead.lastName = lastname;
                newlead.countryID = countryid;
                newlead.modifiedByUserID = session.UserID;
                newlead.modificationDate = activation;
                newlead.isTest = false;
                newlead.deleted = false;
                newlead.leadTypeID = 59;
                newlead.tags = newlead.tags + "#ClassicSuscriber";
            }

            if (phone != null && phone != "")
            {
                tblPhones phoneToAdd = new tblPhones();
                phoneToAdd.phone = Convert.ToString(phone);
                phoneToAdd.phoneTypeID = 1;
                phoneToAdd.doNotCall = false;
                phoneToAdd.leadID = id;
                phoneToAdd.main = true;
                phoneToAdd.dateLastModification = DateTime.Now;

                db.tblPhones.AddObject(phoneToAdd);

            }

            tblLeadEmails emails = (new tblLeadEmails {
                email = email,
                leadID = newlead.leadID,
                main = true,
                dateLastModification = DateTime.Now,
            });
            db.tblLeadEmails.AddObject(emails);

            tblMembershipSales newcard = (new tblMembershipSales {
                membershipCardID = code,
                leadID = newlead.leadID,
                locationID = location,
                activationDate = activation,
                dueDate = duedate,
                dateSaved = activation,
                savedUserBy = session.UserID,
                payment = payment,
                price = price,
                paymentTypeID = paymentType

            });
             db.tblMembershipSales.AddObject(newcard);

            var update = (from cards in db.tblMembershipCards
                          where cards.MembershipCardID == code
                          select cards
                          ).FirstOrDefault();
            update.Status = "Active";
            int codigo = Convert.ToInt32(update.Code);
            var x = (from users in db.aspnet_Membership
                                   where users.UserId == update.userID
                                   select users
                                   ).FirstOrDefault();

            string newpass = string.Empty;
            string epass = Convert.ToString(codigo);
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(epass);
            newpass = Convert.ToBase64String(encryted);

            tblSimpleAuthentication user = new tblSimpleAuthentication();
            user.password = newpass;
            user.username = email;
            user.userid = id;
            user.terminalID = 62;
            user.userType = "member";
           db.tblSimpleAuthentication.AddObject(user);


            

           var emailtosend = email;
           db.SaveChanges();

            var emailid = 188;
            GetEmail(emailid, codigo, name, lastname, activation, duedate, payment, price, email, emailtosend, password, paidmsg, id, true);
            emailid = 189;
            emailtosend = x.Email;
            GetEmail(emailid, codigo, name, lastname, activation, duedate, payment, price, email, emailtosend, password, paidmsg, id, false);

            return Json(id, JsonRequestBehavior.AllowGet);
        }
        //obtiene las locaciones vinculadas a senses of mexico 
        [System.Web.Http.HttpPost]
        public JsonResult getMemberships(DateTime date)
        {
            
            var memberships = (from membershipcards in db.tblMembershipCards
                               join users in db.tblUserProfiles on membershipcards.userID equals users.userID
                               join membershipssales in db.tblMembershipSales on membershipcards.MembershipCardID equals membershipssales.membershipCardID
                               join leads in db.tblLeads on membershipssales.leadID equals leads.leadID
                               join location in db.tblLocations on membershipssales.locationID equals location.locationID
                               join emails in db.tblLeadEmails on membershipssales.leadID equals emails.leadID
                               where membershipcards.userID == session.UserID
                               where membershipcards.Status == "Active"
                               where membershipssales.activationDate == date
                               select new
                               {
                                   code = membershipcards.Code,
                                   leadid = leads.leadID,
                                   salesid = membershipssales.membershipSalesID,
                                   leademailid = emails.emailID,
                                   price = membershipssales.price,
                                   member = leads.firstName + " " + leads.lastName,
                                   name = leads.firstName,
                                   lastname = leads.lastName,
                                   locationid = location.locationID,
                                   locationname = location.location,
                                   email = emails.email,
                                   payment = membershipssales.payment,
                                   ambassador = users.firstName + " " + users.lastName,
                                   status = membershipcards.Status,
                                   cardid = membershipcards.MembershipCardID

                                   
                               }).ToList();
            return Json(memberships, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public JsonResult updateMemberships(int location,string payment,
             int price, Int64 leademailid, int membershipsalesid, string email, Guid leadid, string name, string lastname,
             int countryid)
        {
            var members = (from lead in db.tblLeads
                          where lead.leadID == leadid
                          select lead
                       ).FirstOrDefault();

            members.firstName = name;
            members.lastName = lastname;
            members.countryID = countryid;
          
            var emails = (from e in db.tblLeadEmails
                            where e.emailID == leademailid
                          select e
                         ).FirstOrDefault();
            emails.email = email;

            var memsales = (from memsale in db.tblMembershipSales
                          where memsale.membershipSalesID == membershipsalesid
                          select memsale
                         ).FirstOrDefault();

            memsales.locationID = location;
            memsales.price = price;          
            memsales.payment = payment;
           
            db.SaveChanges();

            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult getMembershipsDates(DateTime date)
        {

            var roleid = session.RoleID;
            Guid ambassador =Guid.Parse( "469F240F-3410-4E62-B621-F3FB087633F9");
            Guid lead_ambassador = Guid.Parse("325E12C8-AA81-4DE6-95FB-5A9BB9607D9C");
            var ambassadores = (from a in db.tblSupervisors_Agents
                                where a.supervisorUserID == session.UserID
                                select a


                                   ).ToList();
            var amb_list = ambassadores.Select(m => (Guid?)m.agentUserID).ToArray();

            if (session.RoleID == ambassador)
            {
                var memberships = (from membershipcards in db.tblMembershipCards
                                   join users in db.tblUserProfiles on membershipcards.userID equals users.userID
                                   join membershipssales in db.tblMembershipSales on membershipcards.MembershipCardID equals membershipssales.membershipCardID
                                   join leads in db.tblLeads on membershipssales.leadID equals leads.leadID
                                   join location in db.tblLocations on membershipssales.locationID equals location.locationID
                                   join country in db.tblCountries on leads.countryID equals country.countryID
                                   join emails in db.tblLeadEmails on membershipssales.leadID equals emails.leadID
                                   where membershipcards.userID == session.UserID
                            //       where membershipcards.Status == "Active"
                                   where membershipssales.activationDate == date
                                   select new
                                   {
                                       code = membershipcards.Code,
                                       leadid = leads.leadID,
                                       salesid = membershipssales.membershipSalesID,
                                       leademailid = emails.emailID,
                                       price = membershipssales.price,
                                       member = leads.firstName + " " + leads.lastName,
                                       name = leads.firstName,
                                       lastname = leads.lastName,
                                       locationid = location.locationID,
                                       locationname = location.location,
                                       email = emails.email,
                                       payment = membershipssales.payment,
                                       ambassador = users.firstName + " " + users.lastName,
                                       countryid = leads.countryID,
                                       country = country.country,
                                       cardid = membershipcards.MembershipCardID,
                                       status = membershipcards.Status



                                   }).ToList();
                return Json(memberships, JsonRequestBehavior.AllowGet);
            }
            else if (session.RoleID == lead_ambassador)
            {
                var memberships = (from membershipcards in db.tblMembershipCards
                                   join users in db.tblUserProfiles on membershipcards.userID equals users.userID
                                   join membershipssales in db.tblMembershipSales on membershipcards.MembershipCardID equals membershipssales.membershipCardID
                                   join leads in db.tblLeads on membershipssales.leadID equals leads.leadID
                                   join location in db.tblLocations on membershipssales.locationID equals location.locationID
                                   join country in db.tblCountries on leads.countryID equals country.countryID
                                   join emails in db.tblLeadEmails on membershipssales.leadID equals emails.leadID
                                   where amb_list.Contains(membershipcards.userID) || membershipcards.userID == session.UserID

                                  // where membershipcards.Status == "Active"
                                   where membershipssales.activationDate == date
                                   select new
                                   {
                                       code = membershipcards.Code,
                                       leadid = leads.leadID,
                                       salesid = membershipssales.membershipSalesID,
                                       leademailid = emails.emailID,
                                       price = membershipssales.price,
                                       member = leads.firstName + " " + leads.lastName,
                                       name = leads.firstName,
                                       lastname = leads.lastName,
                                       locationid = location.locationID,
                                       locationname = location.location,
                                       email = emails.email,
                                       payment = membershipssales.payment,
                                       ambassador = users.firstName + " " + users.lastName,
                                       countryid = leads.countryID,
                                       country = country.country,
                                       cardid = membershipcards.MembershipCardID,
                                       status = membershipcards.Status




                                   }).ToList();
                return Json(memberships, JsonRequestBehavior.AllowGet);
            } else
            {
                var memberships = (from membershipcards in db.tblMembershipCards
                                   join users in db.tblUserProfiles on membershipcards.userID equals users.userID
                                   join membershipssales in db.tblMembershipSales on membershipcards.MembershipCardID equals membershipssales.membershipCardID
                                   join leads in db.tblLeads on membershipssales.leadID equals leads.leadID
                                   join location in db.tblLocations on membershipssales.locationID equals location.locationID
                                   join country in db.tblCountries on leads.countryID equals country.countryID
                                   join emails in db.tblLeadEmails on membershipssales.leadID equals emails.leadID
                           
                        //           where membershipcards.Status == "Active"
                                   where membershipssales.activationDate == date
                                   select new
                                   {
                                       code = membershipcards.Code,
                                       leadid = leads.leadID,
                                       salesid = membershipssales.membershipSalesID,
                                       leademailid = emails.emailID,
                                       price = membershipssales.price,
                                       member = leads.firstName + " " + leads.lastName,
                                       name = leads.firstName,
                                       lastname = leads.lastName,
                                       locationid = location.locationID,
                                       locationname = location.location,
                                       email = emails.email,
                                       payment = membershipssales.payment,
                                       ambassador = users.firstName + " " + users.lastName,
                                       countryid = leads.countryID,
                                       country = country.country,
                                       cardid = membershipcards.MembershipCardID,
                                       status = membershipcards.Status



                                   }).ToList();
                return Json(memberships, JsonRequestBehavior.AllowGet);
            }

           
        }
        public JsonResult createCredentialsPlaces(string data)
        {
            var response = memberships.createCredentialsPlaces(data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult updateCredentialsPlaces(string data)
        {
            var response = memberships.updateCredentialsPlaces(data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkCredentials(Int64 placeid)
        {
            var response = memberships.checkCredentials(placeid);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult sendPayment(string data)
        {
            var response = memberships.sendPayment(data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InactiveMembership(string data)
        {
            var mem = System.Web.Helpers.Json.Decode(data);
            var username = (from u in db.tblUserProfiles
                            where u.userID == session.UserID
                            select u
                            ).FirstOrDefault();
            Int64 cardid = Convert.ToInt64(mem.cardid);
            var card = (from c in db.tblMembershipCards
                        where c.MembershipCardID == cardid
                        select c
                        ).FirstOrDefault();

            card.Status = "Inactive";
            db.SaveChanges();
            System.Net.Mail.MailMessage emailObj = null;
            emailObj = new System.Net.Mail.MailMessage();
            emailObj.From = new System.Net.Mail.MailAddress("info@sensesofmexico.com", "Cancel Membership");
            emailObj.Subject = "Cancel Membership";
            emailObj.Body = " <div>" +
                            "<h1> Cancel Membership </h1>" +
                            "<span> <b>Name:  </b>" + mem.name + " </span> <br>" +
                            "<span> <b>Last Name:  </b>" + mem.lastname + " </span> <br>" +
                          "<span> <b>E-mail:  </b>" + mem.email + " </span> <br>" +
                            "<span> <b>Location:  </b>" + mem.locationname + " </span> <br>" +
                           "<span> <b>Payment:  </b>" + mem.payment + " </span> <br>" +
                             "<span> <b>MembershipID:  </b>" + mem.membershipid + " </span> <br>" +
                             "<span> <b>Membership Code:  </b>" + mem.code + " </span> <br>" +
                              "<span> <b>Abassador:  </b>" + mem.ambassador + " </span> <br>" +
                               "<span> <b>Email sent by:  </b>" + username.firstName + " " + username.lastName + " </span> <br>" +
                            "</div>";
            emailObj.IsBodyHtml = true;
            emailObj.Priority = System.Net.Mail.MailPriority.Normal;
            emailObj.Bcc.Add("gtesistemas.netcenter@taferresorts.com");
            //emailObj.Bcc.Add("arturo.Renteria@villagroup.com");
            emailObj.Bcc.Add("arturo.renteria@villagroup.com");

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.villagroup.com";
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("info@sensesofmexico.com", "In43fo19@");
            smtp.Credentials = credentials;
            smtp.Port = 2525;
            smtp.Send(emailObj);


            
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
    }
   
}

