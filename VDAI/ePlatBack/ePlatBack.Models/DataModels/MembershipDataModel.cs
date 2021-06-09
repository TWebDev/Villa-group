using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Globalization;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.DataModels
{
    public class Memberships
    {
        public static UserSession session = new UserSession();
        ePlatEntities db = new ePlatEntities();


        public object addPreview(int location, string name, string lastname, string email,
            int freeTrialDays, string phone, string comment, string bookingStatus, Boolean sendEmail)
        {


            Guid id = Guid.NewGuid();
            string pass = Convert.ToString(id);
            string newpass = Convert.ToString(pass[0]) + Convert.ToString(pass[1]) + Convert.ToString(pass[2]) + Convert.ToString(pass[3]) + Convert.ToString(pass[4]) + Convert.ToString(pass[5]);

            string password = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(newpass);
            password = Convert.ToBase64String(encryted);

            tblSimpleAuthentication user = new tblSimpleAuthentication();
            user.password = password;
            user.username = email;
            user.userid = id;
            user.terminalID = 62;
            user.userType = "guest";
            db.tblSimpleAuthentication.AddObject(user);
            DateTime today = DateTime.Today;
            tblHarvesting preview = (new tblHarvesting
            {
                harvestingID = id,
                email = email,
                name = name,
                lastname = lastname,
                locationID = location,
                activation = today,
                duedate = today.AddDays(freeTrialDays),
                userID = session.UserID,
                dateSaved = today,
                password = newpass,
                phone = phone,
                comment = comment


            });

            tblLeads newLead = (new tblLeads
            {
                leadID = id,
                firstName = name,
                lastName = lastname,
                initialTerminalID = 62,
                terminalID = 62,
                inputByUserID = session.UserID,
                inputDateTime = DateTime.Now,
                inputMethodID = 2,
                isTest = false,
                deleted = false,
                leadSourceChannelID = 11,
                interestedInDestinationID = 1,         
                leadTypeID = 59,
                leadSourceID = 55,
                leadStatusID = 2,

        });
            if (bookingStatus != null && bookingStatus != "" )
            {
                Int32 bookingStatusID = Convert.ToInt32(bookingStatus);
                newLead.bookingStatusID = bookingStatusID;
            }
            tblLeadEmails newEmail = (new tblLeadEmails
            {
                email = email,
                leadID = id,
                main = true,
            });
            if (phone != null && phone != "")
            {
                tblPhones phoneToAdd = new tblPhones();
                phoneToAdd.phone = Convert.ToString(phone);
                phoneToAdd.phoneTypeID = 1;
                phoneToAdd.doNotCall = false;
                phoneToAdd.leadID = id;
                phoneToAdd.main = true;

                db.tblPhones.AddObject(phoneToAdd);

            }
            db.tblLeadEmails.AddObject(newEmail);
            db.tblLeads.AddObject(newLead);
            db.tblHarvesting.AddObject(preview);
            db.SaveChanges();
            if (sendEmail)
            {
                sendEmailPreview(name, lastname, 190, email, newpass);
            }
            return id;
        }

        public Array getPreviewsPerDay(DateTime fromDate, DateTime toDate)
        {
            Guid ambassador = Guid.Parse("469F240F-3410-4E62-B621-F3FB087633F9");
            Guid lead_ambassador = Guid.Parse("325E12C8-AA81-4DE6-95FB-5A9BB9607D9C");
            if (session.RoleID == ambassador)
            {
                var previews = (from prev in db.tblHarvesting
                                join location in db.tblLocations on prev.locationID equals location.locationID
                                join u in db.tblUserProfiles on prev.userID equals u.userID
                                join l in db.tblLeads on prev.harvestingID equals l.leadID into leads
                                from lead in leads.DefaultIfEmpty()
                                where prev.activation >= fromDate && prev.activation <= toDate
                                where prev.userID == session.UserID
                                select new
                                {
                                    location = location.location,
                                    locationid = location.locationID,
                                    name = prev.name,
                                    lastname = prev.lastname,
                                    dateSaved = prev.dateSaved,
                                    fullname = prev.name + " " + prev.lastname,
                                    activation = prev.activation,
                                    duedate = prev.duedate,
                                    email = prev.email,
                                    harvestingid = prev.harvestingID,
                                    savedBy = u.firstName + " " + u.lastName,
                                    phone = prev.phone,
                                    comment = prev.comment,
                                    
                                    bookingStatusID = (Int32?)lead.bookingStatusID
                                }

                                            ).ToList();
                return previews.ToArray();

            }
            else if (session.RoleID == lead_ambassador)
            {
                var ambassadores = (from a in db.tblSupervisors_Agents
                                    where a.supervisorUserID == session.UserID
                                    select a
                                   
                                    
                                    ).ToList();

                var amb_list = ambassadores.Select(m => (Guid?) m.agentUserID).ToArray();


                var previews = (from prev in db.tblHarvesting
                                join location in db.tblLocations on prev.locationID equals location.locationID
                                join u in db.tblUserProfiles on prev.userID equals u.userID
                                join l in db.tblLeads on prev.harvestingID equals l.leadID into leads
                                from lead in leads.DefaultIfEmpty()
                                where prev.activation >= fromDate && prev.activation <= toDate
                                where amb_list.Contains(prev.userID) || prev.userID == session.UserID
                                select new
                                {
                                    location = location.location,
                                    locationid = location.locationID,
                                    name = prev.name,
                                    dateSaved = prev.dateSaved,
                                    lastname = prev.lastname,
                                    fullname = prev.name + " " + prev.lastname,
                                    activation = prev.activation,
                                    duedate = prev.duedate,
                                    email = prev.email,
                                    harvestingid = prev.harvestingID,
                                    savedBy = u.firstName + " " + u.lastName,
                                    phone = prev.phone,
                                    comment = prev.comment,
                                    bookingStatusID = (Int32?)lead.bookingStatusID


                                }

                                            ).ToList();
                return previews.ToArray();

            }
            else
            {
                var previews = (from prev in db.tblHarvesting
                                join location in db.tblLocations on prev.locationID equals location.locationID
                                join u in db.tblUserProfiles on prev.userID equals u.userID
                                join l in db.tblLeads on prev.harvestingID equals l.leadID// into leads
                                //from lead in leads.DefaultIfEmpty()
                                where prev.activation >= fromDate && prev.activation <= toDate

                                select new
                                {
                                    location = location.location,
                                    locationid = location.locationID,
                                    name = prev.name,
                                    dateSaved = prev.dateSaved,
                                    lastname = prev.lastname,
                                    fullname = prev.name + " " + prev.lastname,
                                    activation = prev.activation,
                                    duedate = prev.duedate,
                                    email = prev.email,
                                    harvestingid = prev.harvestingID,
                                    savedBy = u.firstName + " " + u.lastName,
                                    phone = prev.phone,
                                    comment = prev.comment,
                                    bookingStatusID = l.bookingStatusID
                                }

                                            ).ToList();
                return previews.ToArray();
            }


        }
        public string updatePreview(Guid harvestingid, string name, string lastname, string email,
            int location, string phone, string comment, DateTime dueDate, string bookingStatus)
        {

            var update = (from prev in db.tblHarvesting
                          where prev.harvestingID == harvestingid
                          select prev
                          ).FirstOrDefault();

            update.email = email;
            update.name = name;
            update.lastname = lastname;
            update.locationID = location;
            update.phone = phone;
            update.comment = comment;
            update.duedate = dueDate;

            var lead = (from prev in db.tblLeads
                          where prev.leadID == harvestingid
                          select prev
                         ).FirstOrDefault();
            if (lead != null)
            {
                lead.firstName = name;
                lead.lastName = lastname;
                var leadEmail = (from prev in db.tblLeadEmails
                              where prev.leadID == harvestingid
                              select prev
                          ).FirstOrDefault();
                if (leadEmail != null)
                {
                    leadEmail.email = email;
                }
                var leadPhone = (from prev in db.tblPhones
                                 where prev.leadID == harvestingid
                                 select prev
                          ).FirstOrDefault();
                if (leadPhone != null)
                {
                    leadPhone.phone = phone;
                }
                Int32 bookingID = Convert.ToInt32(bookingStatus);
                lead.bookingStatusID = bookingID;
            }else
            {
                tblLeads newLead = (new tblLeads
                {
                    leadID = harvestingid,
                    firstName = name,
                    lastName = lastname,
                    initialTerminalID = 62,
                    terminalID = 62,
                    inputByUserID = session.UserID,
                    inputDateTime = DateTime.Now,
                    inputMethodID = 2,
                    isTest = false,
                    deleted = false,
                    leadSourceChannelID = 11,
                    interestedInDestinationID = 1,
                    leadTypeID = 59,
                    leadSourceID = 55,
                    leadStatusID = 2,

                });
                if (bookingStatus != null)
                {
                    Int32 bookingStatusID = Convert.ToInt32(bookingStatus);
                    newLead.bookingStatusID = bookingStatusID;
                }
                tblLeadEmails newEmail = (new tblLeadEmails
                {
                    email = email,
                    leadID = harvestingid,
                    main = true,
                });
                if (phone != null && phone != "")
                {
                    tblPhones phoneToAdd = new tblPhones();
                    phoneToAdd.phone = Convert.ToString(phone);
                    phoneToAdd.phoneTypeID = 1;
                    phoneToAdd.doNotCall = false;
                    phoneToAdd.leadID = harvestingid;
                    phoneToAdd.main = true;

                    db.tblPhones.AddObject(phoneToAdd);

                }
                db.tblLeadEmails.AddObject(newEmail);
                db.tblLeads.AddObject(newLead);
                db.SaveChanges();
            }
               

            db.SaveChanges();

            



            return "ok";
        }

        public static System.Net.Mail.MailMessage sendEmailPreview( string name, string lastname, int emailid, string emailtosend, string password)
        {          
            ePlatEntities db = new ePlatEntities();
            var emailnotifications = (from emails in db.tblEmailNotifications
                                      where emails.emailNotificationID == emailid
                                      select emails
                         ).FirstOrDefault();
            var mail = (from emails in db.tblEmails
                        where emails.emailID == emailnotifications.emailID
                        select emails
                         ).FirstOrDefault();

            System.Net.Mail.MailMessage emailObj = null;
            emailObj = new System.Net.Mail.MailMessage();
            emailObj.From = new System.Net.Mail.MailAddress("concierge@sensesofmexico.com", mail.alias);
            emailObj.Subject = mail.subject;
            emailObj.Body = mail.content_.Replace("$name", lastname).Replace("$email", emailtosend).Replace("$password", password).Replace("$fullname", name);               
            emailObj.IsBodyHtml = true;
            emailObj.Priority = System.Net.Mail.MailPriority.Normal;
            emailObj.Bcc.Add("arturo.renteria@villagroup.com"); 
            emailObj.Bcc.Add("concierge@sensesofmexico.com");
            emailObj.Bcc.Add(emailtosend);         
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.villagroup.com";
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("concierge@sensesofmexico.com", "Con38cierge19@");
            smtp.Credentials = credentials;
            smtp.Port = 2525;
            smtp.Send(emailObj);
            return emailObj;
        }
        public object createCredentialsPlaces(string data)
        {
            var credentials = System.Web.Helpers.Json.Decode(data);
            Guid newguid = Guid.NewGuid();

            string username = Convert.ToString(credentials.username);
            var checkusername = (from c in db.tblSimpleAuthentication
                                 where c.username == username
                                 select c
                                 ).FirstOrDefault(); ;
            if (checkusername != null)
            {
                return false;
            }else
            {
                string password = string.Empty;
                byte[] encryted = System.Text.Encoding.Unicode.GetBytes(credentials.password);
                password = Convert.ToBase64String(encryted);
                tblSimpleAuthentication user = new tblSimpleAuthentication();
                user.userid = newguid;
                user.password = password;
                user.username = credentials.username;
                user.userType = "affiliate";
                user.terminalID = 62;
                db.tblSimpleAuthentication.AddObject(user);
                tblPlacesSimpleAuthentication place = new tblPlacesSimpleAuthentication();
                if (credentials.type == "provider")
                {
                    place.providerID = credentials.providerID;
                }
                else
                {
                    place.placeID = credentials.placeid;
                }
                //place.placeID = credentials.placeid;
                place.userid = newguid;
                db.tblPlacesSimpleAuthentication.AddObject(place);
                db.SaveChanges();
                return "ok";
            }         
        }
        public object checkCredentials(Int64 placeid)
        {
            var credentials = (from c in db.tblSimpleAuthentication
                               join p in db.tblPlacesSimpleAuthentication on c.userid equals p.userid
                               where p.placeID == placeid
                               select c
                               );
            if (credentials.FirstOrDefault() == null)
            {
                var credentialsP = (from c in db.tblSimpleAuthentication
                                   join p in db.tblPlacesSimpleAuthentication on c.userid equals p.userid
                                   where p.providerID == placeid
                                   select c
                               );
                return credentialsP.FirstOrDefault();
            }
            else
            {
                return credentials.FirstOrDefault(); 
            }          
        }
        public object updateCredentialsPlaces(string data)
        {
            var credentials = System.Web.Helpers.Json.Decode(data);
            string username = Convert.ToString(credentials.username);                
            Int64 userid = Convert.ToInt64(credentials.id);
            var user = (from c in db.tblSimpleAuthentication
                        where c.simpleAuthenticationID == userid
                        select c
                                ).FirstOrDefault();
            if (user != null)
            {
                var checkusername = (from c in db.tblSimpleAuthentication
                                     where c.username == username
                                     select c
                                 ).FirstOrDefault(); ;

                if (checkusername != null)
                {
                    if (checkusername.userid == user.userid)
                    {
                        user.username = credentials.username;
                        string password = string.Empty;
                        byte[] encryted = System.Text.Encoding.Unicode.GetBytes(credentials.password);
                        password = Convert.ToBase64String(encryted);
                        user.password = password;
                        db.SaveChanges();
                    }else
                    {
                        return false;
                    }
                }else
                {
                    user.username = credentials.username;
                    string password = string.Empty;
                    byte[] encryted = System.Text.Encoding.Unicode.GetBytes(credentials.password);
                    password = Convert.ToBase64String(encryted);
                    user.password = password;
                    db.SaveChanges();
                }                
            }
                return "ok";           
        }
        public object sendPayment(string data)
        {
            AttemptResponse attempt = new AttemptResponse();

            var purchase = System.Web.Helpers.Json.Decode(data);
           //try
           // {
                //INICIA el proceso de pago                              
                RescomDataModel.ApplyPayment_Data paymentData = new RescomDataModel.ApplyPayment_Data();
                Random rand = new Random();
            Guid guidPurchase = Guid.NewGuid();

            //revisar si existe merchant para el punto de venta
            paymentData.UserName = "promotoreswebgarza";
                paymentData.Password = "G@rz@C3nt3r";
                paymentData.Card_Holder = purchase.name;
                paymentData.Card_Holder_Address = purchase.address;
                paymentData.Card_Holder_Zip = purchase.zipcode;
                paymentData.Card_Number = purchase.cardnumbertrimed;
                paymentData.Card_Security_Number = purchase.cvv;
                paymentData.Expiration_Date = purchase.emonth + "/01/" + purchase.eyear;
                paymentData.Reference_Code = TerminalDataModel.GetPrefix(62) + "-" + guidPurchase;
                paymentData.Transaction_Amount = Convert.ToDouble(purchase.amount);
                paymentData.CurrencyID = 1;


            Guid guid = Guid.Parse(purchase.guid);
            tblLeads newLead = new tblLeads();

            var exists = (from l in db.tblLeads
                         where l.leadID == guid
                         select l
                         ).FirstOrDefault();

            if (exists == null)
            {
                newLead.leadID = guid;
                newLead.initialTerminalID = 62;
                newLead.terminalID = 62;
                newLead.leadSourceChannelID = 11;
                newLead.interestedInDestinationID = 1;
                newLead.firstName = purchase.name;
                newLead.lastName = ".";
                newLead.leadTypeID = 59;
                newLead.leadSourceID = 55;
                newLead.inputByUserID = session.UserID;
                newLead.inputDateTime = DateTime.Today;
                newLead.inputMethodID = 8;
                newLead.leadStatusID = 2;
                newLead.isTest = false;
                newLead.deleted = false;
                db.tblLeads.AddObject(newLead);
            }else
            {
                newLead = exists; 
            }
            
            



            RescomDataModel.ApplyPayment_Response paymentResponse = RescomDataModel.ApplyPayment(paymentData);

                        
                tblBillingInfo newBillingInfo = new tblBillingInfo();
                newBillingInfo.address = purchase.Address;
                newBillingInfo.billingComments = "";              
                newBillingInfo.cardCVV = purchase.cvv;
                newBillingInfo.cardExpiry = purchase.emonth+ "/" + "20"+purchase.eyear;
                newBillingInfo.cardHolderName = purchase.name;
                newBillingInfo.cardNumber = mexHash.mexHash.EncryptString(purchase.cardnumbertrimed);
                newBillingInfo.cardTypeID = Convert.ToInt32(purchase.cardtype); 
                newBillingInfo.city = null;
                newBillingInfo.countryID = null;
                newBillingInfo.dateSaved = DateTime.Now;
                newBillingInfo.firstName = purchase.name;
                newBillingInfo.lastName = null;
                newBillingInfo.leadID = newLead.leadID;
                newBillingInfo.state = null;
                newBillingInfo.zipcode = purchase.zipcode;
                db.tblBillingInfo.AddObject(newBillingInfo);

                tblMoneyTransactions newMoneyTransaction = new tblMoneyTransactions();
                newMoneyTransaction.terminalID = 62;
                newMoneyTransaction.authCode = paymentResponse.Auth_Code;
                newMoneyTransaction.authAmount = (decimal)paymentResponse.Authorization_Amount;
                newMoneyTransaction.authDate = paymentResponse.Authorization_Date;
                newMoneyTransaction.errorCode = paymentResponse.Error_Code.ToString();
                newMoneyTransaction.transactionDate = DateTime.Now;
                newMoneyTransaction.transactionTypeID = 1;
                newMoneyTransaction.merchantAccountID = 9;
                newMoneyTransaction.reference = paymentData.Reference_Code;
                newMoneyTransaction.madeByEplat = true;
                newMoneyTransaction.billingInfoID = newBillingInfo.billingInfoID;
                db.tblMoneyTransactions.AddObject(newMoneyTransaction);

                tblPaymentDetails newPaymentDetails = new tblPaymentDetails();
                newPaymentDetails.amount = Convert.ToDecimal(purchase.amount);
                newPaymentDetails.purchaseID = guidPurchase;
                newPaymentDetails.currencyID = 1;
                newPaymentDetails.chargeDescriptionID = 1;
                newPaymentDetails.moneyTransactionID = newMoneyTransaction.moneyTransactionID;
                newPaymentDetails.chargeTypeID = 1;
                newPaymentDetails.dateSaved = DateTime.Now;
                newPaymentDetails.paymentType = 5;
                newPaymentDetails.savedByUserID = session.UserID;
                db.tblPaymentDetails.AddObject(newPaymentDetails);

                tblPurchases newPurchase = new tblPurchases();
                newPurchase.purchaseID = guidPurchase;
                newPurchase.leadID = newLead.leadID;
                newPurchase.userID = session.UserID;
                newPurchase.purchaseDateTime = DateTime.Now;
                newPurchase.terminalID = 62;
                newPurchase.currencyID = 1;
                newPurchase.purchaseStatusID = 1;
                newPurchase.deleted = false;
                newPurchase.isTest = false;
                newPurchase.pointOfSaleID = 60;
                newPurchase.total = Convert.ToDecimal(purchase.amount);
                //string culture = Utils.GeneralFunctions.GetCulture();
                newPurchase.culture = "en-US";

                db.tblPurchases.AddObject(newPurchase);
                db.tblBillingInfo.AddObject(newBillingInfo);
                db.tblPaymentDetails.AddObject(newPaymentDetails);
                db.tblMoneyTransactions.AddObject(newMoneyTransaction);

                db.SaveChanges();
                if (paymentResponse.Error_Code == 0)
                {
                    //pagado
                    object[] obj = new[]
                       {
                        new object[] {paymentResponse.Auth_Code},                       
                        new object[] {1},

                         };

                    return obj;

                }
                else
                {
                    attempt.Type = Attempt_ResponseTypes.Error;
                    attempt.Message = RescomDataModel.ApplyPayment_ErrorCodes[paymentResponse.Error_Code];
                    return attempt.Message;
                }

                
        //   }
      /*    catch
            {
                return "Invalid Credit Card.";
            }
                    */     
        }
    }

}
