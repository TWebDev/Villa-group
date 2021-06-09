
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using System.Web.Mvc;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;

namespace ePlatBack.Models.DataModels
{
    public class ControlsDataModel
    {

        public class MultiFields
        {
            public static void SaveValue(long fieldID, string value, Guid transactionID)
            {
                ePlatEntities db = new ePlatEntities();
                tblFieldsValues field = new tblFieldsValues()
                {
                    fieldID = fieldID,
                    value = value,
                    terminalID = Utils.GeneralFunctions.GetTerminalID(),
                    dateSaved = DateTime.Now,
                    transactionID = transactionID
                };
                db.tblFieldsValues.AddObject(field);
                db.SaveChanges();
            }
        }

        public static string GetHeadScript()
        {
            ePlatEntities db = new ePlatEntities();
            string domain = HttpContext.Current.Request.Url.Host;
            var headScript = (from s in db.tblTerminalDomains
                              where s.domain == domain
                              select s.scriptsHeader).FirstOrDefault();
            return headScript;
        }


        public static string GetConversionCode(int eventID, int? pointOfSaleID = null)
        {
            ePlatEntities db = new ePlatEntities();
            string script = "";
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            string culture = Utils.GeneralFunctions.GetCulture();
            var conversionQuery = (from c in db.tblSysEvents_Terminals
                                   where c.eventID == eventID
                                   && c.terminalID == terminalID
                                   && c.culture == culture
                                   && (c.pointOfSaleID == pointOfSaleID || pointOfSaleID == null)
                                   select c.conversionScript).FirstOrDefault();

            if (conversionQuery != null)
            {
                script = conversionQuery;
            }

            return script;
        }

        public class FreeVacationDataModel
        {
            ePlatEntities db = new ePlatEntities();

            public AttemptResponse Save(FreeVacationViewModel model)
            {
                Guid transactionID = Guid.NewGuid();

                AttemptResponse response = new AttemptResponse();
                try
                {
                    MultiFields.SaveValue(1, model.FreeVacation_FirstName, transactionID);
                    MultiFields.SaveValue(2, model.FreeVacation_LastName, transactionID);
                    MultiFields.SaveValue(3, model.FreeVacation_Email, transactionID);
                    MultiFields.SaveValue(4, model.FreeVacation_Phone, transactionID);
                    MultiFields.SaveValue(5, HttpContext.Current.Request.Url.OriginalString, transactionID);

                    var cookie = HttpContext.Current.Request.Cookies["dvh_campaign"];
                    string campaign = "Undefined";
                    if (cookie != null)
                    {
                        campaign = cookie.Value;
                    }
                    MultiFields.SaveValue(227, campaign, transactionID);

                    //revisar si se asigna o se envía por correo

                    //sending email notification for quote request
                    System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(3);
                    email.Body = email.Body
                        .Replace("$FirstName", model.FreeVacation_FirstName)
                        .Replace("$LastName", model.FreeVacation_LastName)
                        .Replace("$Email", model.FreeVacation_Email)
                        .Replace("$Phone", model.FreeVacation_Phone);

                    //Utils.EmailNotifications.Send(email);
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                    //
                    //Enviar Lead a WebHoook
                    long? terminalID = Utils.GeneralFunctions.GetTerminalID();
                    var WebHookID = (from w in db.tblWebhooks
                                     where w.terminalID == terminalID
                                     && w.eventID == 3
                                     select w.webhookID).FirstOrDefault();

                    if (WebHookID != null)
                    {
                        string whURL = "https://developers.eplat.villagroup.com/api/webhooks/incoming/genericjson/newlead?code=80f32f6123104d09a72c000047564e51";

                        WebhooksViewModel.NewLeadWH newLead = new WebhooksViewModel.NewLeadWH();

                        newLead.webhookID = WebHookID;
                        newLead.referenceID = null;
                        newLead.firstName = model.FreeVacation_FirstName;
                        newLead.lastName = model.FreeVacation_LastName;
                        newLead.phone = model.FreeVacation_Phone;
                        newLead.email = model.FreeVacation_Email;

                        using (WebClient Client = new WebClient())
                        {
                            var json = Newtonsoft.Json.JsonConvert.SerializeObject(newLead);
                            Client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            Client.UploadString(whURL, json);
                        }
                    }

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = "";
                    response.Message = "Thank you for participating!";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    //response.ErrorMessage = ex.Message;
                    response.Exception = ex;
                    response.Message = "Something weird happened trying to save your information, please try again.";
                    return response;
                }
            }
        }

        public class GroupsForm
        {
            ePlatEntities db = new ePlatEntities();

            public AttemptResponse Save(GroupsFormViewModel model)
            {
                Guid transactionID = Guid.NewGuid();

                AttemptResponse response = new AttemptResponse();
                try
                {
                    MultiFields.SaveValue(675, model.GroupsForm_Hotel, transactionID);
                    MultiFields.SaveValue(676, model.GroupsForm_NumberOfPassengers, transactionID);
                    MultiFields.SaveValue(677, model.GroupsForm_FromDate, transactionID);
                    MultiFields.SaveValue(678, model.GroupsForm_ToDate, transactionID);
                    MultiFields.SaveValue(679, model.GroupsForm_Name, transactionID);
                    MultiFields.SaveValue(680, model.GroupsForm_Phone, transactionID);
                    MultiFields.SaveValue(682, model.GroupsForm_Email, transactionID);

                    //sending email notification for contact forms
                    System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(33);
                    email.To.Add(model.GroupsForm_Email);
                    email.Body = email.Body
                        .Replace("$GroupsForm_Hotel", model.GroupsForm_Hotel)
                        .Replace("$GroupsForm_NumberOfPassengers", model.GroupsForm_NumberOfPassengers)
                        .Replace("$GroupsForm_FromDate", model.GroupsForm_FromDate)
                        .Replace("$GroupsForm_ToDate", model.GroupsForm_ToDate)
                        .Replace("$GroupsForm_Name", model.GroupsForm_Name)
                        .Replace("$GroupsForm_Phone", model.GroupsForm_Phone)
                        .Replace("$GroupsForm_Email", model.GroupsForm_Email);

                    //Utils.EmailNotifications.Send(email);
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = "";
                    response.Message = "Thank you for contacting us!";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    //response.ErrorMessage = ex.Message;
                    response.Exception = ex;
                    response.Message = "Something weird happened trying to send your message, please try again.";
                    return response;
                }
            }
        }

        public class ContactForm
        {
            ePlatEntities db = new ePlatEntities();

            public AttemptResponse Save(ContactFormViewModel model)
            {
                Guid transactionID = Guid.NewGuid();

                AttemptResponse response = new AttemptResponse();
                try
                {
                    MultiFields.SaveValue(26, model.Contact_FirstName, transactionID);
                    MultiFields.SaveValue(27, model.Contact_LastName, transactionID);
                    MultiFields.SaveValue(28, model.Contact_Phone, transactionID);
                    MultiFields.SaveValue(29, model.Contact_Email, transactionID);
                    MultiFields.SaveValue(30, model.Contact_TimeToReach, transactionID);
                    MultiFields.SaveValue(31, model.Contact_Comment, transactionID);
                    MultiFields.SaveValue(32, HttpContext.Current.Request.Url.OriginalString, transactionID);

                    //sending email notification for contact forms
                    System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(6);
                    email.To.Add(model.Contact_Email);
                    email.Body = email.Body
                        .Replace("$ContactForm_FirstName", model.Contact_FirstName)
                        .Replace("$ContactForm_LastName", model.Contact_LastName)
                        .Replace("$ContactForm_Email", model.Contact_Email)
                        .Replace("$ContactForm_Phone", model.Contact_Phone)
                        .Replace("$ContactForm_TimeToReach", model.Contact_TimeToReach);
                    if (model.Contact_Comment != null)
                    {
                        email.Body = email.Body.Replace("$ContactForm_Comments", model.Contact_Comment.ToString());
                    }
                    else
                    {
                        email.Body = email.Body.Replace("$ContactForm_Comments", "");
                    }

                    //Utils.EmailNotifications.Send(email);
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = "";
                    response.Message = "Thank you for contacting us!";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    //response.ErrorMessage = ex.Message;
                    response.Exception = ex;
                    response.Message = "Something weird happened trying to send your message, please try again.";
                    return response;
                }
            }
        }

        public class LeadsGenerationDataModel
        {
            ePlatEntities db = new ePlatEntities();

            public AttemptResponse Save(LeadsGenerationViewModel model)
            {
                Guid transactionID = Guid.NewGuid();

                AttemptResponse response = new AttemptResponse();
                try
                {
                    //first name
                    MultiFields.SaveValue(18, model.FirstName, transactionID);
                    //last name
                    MultiFields.SaveValue(19, model.LastName, transactionID);
                    //email
                    MultiFields.SaveValue(20, model.Email, transactionID);
                    //country
                    MultiFields.SaveValue(21, model.Country, transactionID);
                    //state
                    MultiFields.SaveValue(22, model.State, transactionID);
                    //mobile phone
                    MultiFields.SaveValue(23, model.MobilePhone, transactionID);
                    //home phone
                    MultiFields.SaveValue(24, model.HomePhone, transactionID);
                    //339 - Time to Reach
                    MultiFields.SaveValue(339, model.TimeToReach, transactionID);

                    //sending email notification for contact forms
                    System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(3);
                    email.To.Add(model.Email);
                    email.Body = email.Body
                        .Replace("$FirstName", model.FirstName)
                        .Replace("$LastName", model.LastName)
                        .Replace("$Email", model.Email)
                        .Replace("$Country", model.Country)
                        .Replace("$State", model.State)
                        .Replace("$MobilePhone", model.MobilePhone)
                        .Replace("$HomePhone", model.HomePhone)
                        .Replace("$TimeToReach", model.TimeToReach);

                    //Utils.EmailNotifications.Send(email);
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = transactionID;
                    response.Message = "Thank you for participating!";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = transactionID;
                    //response.ErrorMessage = ex.Message;
                    response.Exception = ex;
                    response.Message = "Something weird happened trying to save your information, please try again.";
                    return response;
                }
            }
        }

        public class ReviewDataModel
        {
            ePlatEntities db = new ePlatEntities();

            public AttemptResponse SetBookingExperienceReview(ActivityReviewsViewModel.BookingExperience model)
            {
                AttemptResponse response = new AttemptResponse();
                try
                {
                    long terminalID = Utils.GeneralFunctions.GetTerminalID();

                    //rating
                    tblFieldsValues newRating = new tblFieldsValues();
                    newRating.fieldID = 174;
                    newRating.value = model.Rating.ToString();
                    newRating.terminalID = terminalID;
                    newRating.dateSaved = DateTime.Now;
                    newRating.transactionID = model.TransactionID;
                    db.tblFieldsValues.AddObject(newRating);

                    //review
                    if (model.Review != null)
                    {
                        tblFieldsValues newReview = new tblFieldsValues();
                        newReview.fieldID = 173;
                        newReview.value = model.Review;
                        newReview.terminalID = terminalID;
                        newReview.dateSaved = DateTime.Now;
                        newReview.transactionID = model.TransactionID;
                        db.tblFieldsValues.AddObject(newReview);
                    }

                    //submitted
                    long? SubmittedFieldID = SurveyDataModel.getFieldIDByName(15, "Submitted");
                    if (SubmittedFieldID != null)
                    {
                        tblFieldsValues newSubmitted = new tblFieldsValues();
                        newSubmitted.fieldID = (long)SubmittedFieldID;
                        newSubmitted.value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                        newSubmitted.terminalID = terminalID;
                        newSubmitted.dateSaved = DateTime.Now;
                        newSubmitted.transactionID = model.TransactionID;
                        db.tblFieldsValues.AddObject(newSubmitted);
                    }

                    db.SaveChanges();

                    response.Type = Attempt_ResponseTypes.Ok;
                    //response.ObjectID = newReview.reviewID;
                    response.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Submit_Feedback_Success;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Submit_Fails;
                    return response;
                }
            }
            public AttemptResponse SetPurchaseServiceReview(ActivityReviewsViewModel.ServiceReviewItem model)
            {
                AttemptResponse response = new AttemptResponse();
                //try
                //{
                tblReviews newReview = new tblReviews();
                var ReviewQuery = (from r in db.tblReviews
                                   where r.purchase_ServiceID == model.PurchaseServiceID
                                   select r).FirstOrDefault();

                if (ReviewQuery != null)
                {
                    newReview = ReviewQuery;
                    newReview.review = model.Review;
                    db.SaveChanges();
                }
                else
                {
                    var purchaseServiceQuery = (from p in db.tblPurchases_Services
                                                where p.purchase_ServiceID == model.PurchaseServiceID
                                                select new
                                                {
                                                    p.tblPurchases.tblLeads.firstName,
                                                    p.tblPurchases.tblLeads.lastName,
                                                    p.tblPurchases.tblLeads.city,
                                                    p.tblPurchases.tblLeads.state,
                                                    p.tblPurchases.tblLeads.tblCountries.country,
                                                    p.tblServices.transportationService,
                                                    p.tblServices.service,
                                                    p.serviceDateTime
                                                }).FirstOrDefault();


                    newReview.review = model.Review;
                    newReview.rating = model.Rating;
                    newReview.purchase_ServiceID = model.PurchaseServiceID;

                    newReview.author = purchaseServiceQuery.firstName + " " + purchaseServiceQuery.lastName;
                    newReview.from_ = purchaseServiceQuery.city + ", " + purchaseServiceQuery.state + ", " + purchaseServiceQuery.country;
                    newReview.sysItemTypeID = (purchaseServiceQuery.transportationService ? 3 : 1);
                    newReview.itemID = model.ServiceID;
                    newReview.culture = Utils.GeneralFunctions.GetCulture();
                    newReview.dateSaved = DateTime.Now;
                    newReview.active = false;
                    newReview.picture = model.Picture;
                    db.tblReviews.AddObject(newReview);
                    db.SaveChanges();

                    //enviar correo de aprobación
                    System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(7);
                    email.Body = email.Body
                        .Replace("$AuthCode", "Automatic Email for " + purchaseServiceQuery.service)
                        .Replace("$Rating", newReview.rating.ToString())
                        .Replace("$Author", newReview.author)
                        .Replace("$From", newReview.from_)
                        .Replace("$Review", newReview.review)
                        .Replace("$Picture", newReview.picture)
                        .Replace("$ItemID", newReview.reviewID.ToString())
                        .Replace("$Client", newReview.author)
                        .Replace("$Date", purchaseServiceQuery.serviceDateTime.ToString("yyyy-MM-dd"));

                        //Utils.EmailNotifications.Send(email);
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                }

                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = newReview.reviewID;
                response.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Review_Success;
                return response;
                //}
                //catch (Exception ex)
                //{
                //response.Type = Attempt_ResponseTypes.Error;
                //response.ObjectID = 0;
                //response.Exception = ex;
                //response.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Submit_Fails;
                //return response;
                //}
            }
            public AttemptResponse Save(ReviewListItem model)
            {
                AttemptResponse response = new AttemptResponse();
                try
                {
                    tblReviews newReview = new tblReviews();
                    newReview.authCode = model.AuthCode;
                    newReview.review = model.Review;
                    newReview.rating = model.Rating;
                    newReview.author = model.Author;
                    newReview.from_ = model.From;
                    newReview.sysItemTypeID = model.ReviewItemTypeID;
                    newReview.itemID = model.ReviewItemID;
                    newReview.culture = Utils.GeneralFunctions.GetCulture();
                    newReview.dateSaved = DateTime.Now;
                    newReview.active = false;
                    db.tblReviews.AddObject(newReview);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = newReview.reviewID;
                    response.Message = "Thank you for sharing your experience! We are validating your review and will be posted in the next 24 hours.";

                    //enviar correo de aprobación
                    System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(7);
                    email.Body = email.Body
                        .Replace("$AuthCode", model.AuthCode)
                        .Replace("$Rating", model.Rating.ToString())
                        .Replace("$Author", model.Author)
                        .Replace("$From", model.From)
                        .Replace("$Review", model.Review)
                        .Replace("$ItemID", newReview.reviewID.ToString());

                    //buscar compra relacionada al authcode $Client $Date
                    var RelatedPurchase = (from r in db.tblMoneyTransactions
                                           where r.authCode == model.AuthCode
                                           orderby r.transactionDate
                                           select new
                                           {
                                               Client = r.tblBillingInfo.tblLeads.firstName + " " + r.tblBillingInfo.tblLeads.lastName,
                                               Date = r.transactionDate
                                           }).FirstOrDefault();

                    if (RelatedPurchase != null)
                    {
                        email.Body = email.Body
                            .Replace("$Client", RelatedPurchase.Client)
                            .Replace("$Date", RelatedPurchase.Date.ToString("yyyy-MM-dd"));

                        //Utils.EmailNotifications.Send(email);
                        EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                    }
                    else
                    {
                        email.Body = email.Body
                            .Replace("$Client", "NOT FOUND")
                            .Replace("$Date", "NOT FOUND");
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Something weird happened trying to save your information, please try again.";
                    return response;
                }
            }

            public PageViewModel ApproveReview(long id)
            {
                string activityLink = "";
                string activityName = "";
                PageViewModel page = new PageViewModel();
                var Review = db.tblReviews.FirstOrDefault(x => x.reviewID == id);
                if (Review != null)
                {
                    Review.active = true;
                    if (Review.sysItemTypeID == 3)
                    {
                        Review.sysItemTypeID = 1;
                    }
                    db.SaveChanges();

                    var SeoItem = (from s in db.tblSeoItems
                                   where s.itemID == Review.itemID
                                   && s.sysItemTypeID == Review.sysItemTypeID
                                   select new
                                   {
                                       Url = s.friendlyUrl,
                                       Name = s.title
                                   }).FirstOrDefault();

                    if (SeoItem != null)
                    {
                        activityLink = SeoItem.Url;
                        activityName = SeoItem.Name;
                    }

                    page.Culture = Utils.GeneralFunctions.GetCulture().Substring(0, 2).ToLower();
                    DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
                    if (objMaster != null)
                    {
                        page.Scripts_Header = objMaster.Scripts_Header;
                        page.Scripts_AfterBody = objMaster.Scripts_AfterBody;
                        page.Scripts_Footer = objMaster.Scripts_Footer;
                        page.Template_Header = objMaster.Template_Header;
                        page.Template_Footer = objMaster.Template_Footer;
                        page.Template_Logo = objMaster.Template_Logo;
                        page.Template_Phone_Desktop = objMaster.Template_Phone_Desktop;
                        page.Template_Phone_Mobile = objMaster.Template_Phone_Mobile;
                        page.CanonicalDomain = objMaster.CanonicalDomain;
                    }

                    //Submenus
                    SubmenusViewModel objSubmenus = PageDataModel.GetSubmenus();
                    if (objSubmenus != null)
                    {
                        page.Submenu1 = objSubmenus.Submenu1;
                        page.Submenu2 = objSubmenus.Submenu2;
                        page.Submenu3 = objSubmenus.Submenu3;
                    }

                    page.Seo_Title = "Review Approved";
                    page.Content = "<h1>Review Approved</h1><p>You can see the publication going to the next link: <a href=" + activityLink + ">" + activityName + "</p>";
                    page.Seo_FriendlyUrl = "/";
                }

                return page;
            }
        }

        public class BlockContentDataModel
        {
            public static BlockContentViewModel getBlockContent(string blockName)
            {
                ePlatEntities db = new ePlatEntities();
                BlockContentViewModel block = new BlockContentViewModel();
                string culture = Utils.GeneralFunctions.GetCulture();
                long terminalID = Utils.GeneralFunctions.GetTerminalID();
                var blockQ = (from b in db.tblBlockDescriptions
                              join qblock in db.tblBlocks on b.blockID equals qblock.blockID
                              where b.culture == culture
                              && qblock.block == blockName
                              && qblock.terminalID == terminalID
                              select b.content_).FirstOrDefault();
                block.Content = blockQ;
                return block;
            }

            public static BlockContentViewModel getSidebarContent()
            {
                ePlatEntities db = new ePlatEntities();
                BlockContentViewModel block = new BlockContentViewModel();
                string culture = Utils.GeneralFunctions.GetCulture();
                long terminalID = Utils.GeneralFunctions.GetTerminalID();
                string domain = HttpContext.Current.Request.Url.Host;
                //limpiar puerto en localhost
                domain = domain.Replace(":37532", "");

                //buscar sidebar en tblTerminalDomains
                var SidebarQ = (from s in db.tblTerminalDomains
                                where s.terminalID == terminalID
                                && s.domain == domain
                                && s.culture == culture
                                select s.sidebar).FirstOrDefault();

                if (SidebarQ != null)
                {
                    block.Content = SidebarQ;
                }
                else
                {
                    //si no está, buscar por el nombre de right column

                    var BlockQ = (from b in db.tblBlockDescriptions
                                  join qblock in db.tblBlocks on b.blockID equals qblock.blockID
                                  where b.culture == culture
                                  && qblock.block == "Right Column"
                                  && qblock.terminalID == terminalID
                                  select b.content_).FirstOrDefault();

                    if (BlockQ != null)
                    {
                        block.Content = BlockQ;
                    }
                }

                return block;
            }
        }



        public class BlogContentDataModel
        {
            public static BlogContentViewModel getBlogsList(int length)
            {
                ePlatEntities db = new ePlatEntities();
                BlogContentViewModel blogList = new BlogContentViewModel();

                string culture = Utils.GeneralFunctions.GetCulture();
                long terminalID = Utils.GeneralFunctions.GetTerminalID();
                var blogsQ = (from b in db.tblContentPageDescriptions
                              where b.tblContentPages.tblContentPages2.page == "Blog"
                              && b.tblContentPages.terminalID == terminalID
                              && b.culture == culture
                              && b.active == true
                              orderby b.pageID descending
                              select b).Distinct().Take(length);

                foreach (var blog in blogsQ)
                {

                    string body = blog.content_;
                    string h1 = "";
                    if (body.IndexOf("<h1>") > -1)
                    {
                        h1 = body.Substring(body.IndexOf("<h1>") + 4);
                        h1 = h1.Remove(h1.IndexOf("</h1>"));
                    }

                    string img = "";
                    if (body.IndexOf("<img") > -1)
                    {
                        img = body.Substring(body.IndexOf("<img") + 4);
                        img = img.Substring(img.IndexOf("src=\"") + 5);
                        img = img.Remove(img.IndexOf("\""));
                    }

                    string txt = "";

                    txt = body;
                    txt = Regex.Replace(txt, "<img.*?>", "");
                    txt = Regex.Replace(txt, "<p.*?>", "");
                    txt = Regex.Replace(txt, "<h1.*?>", "");
                    txt = txt.Replace("</p>", "");
                    txt = txt.Replace("</h1>", "");
                    if (txt.Length > 290)
                    {
                        txt = txt.Remove(290) + "...";
                    }


                    tblSeoItems objSeo = PageDataModel.GetSeo(5, blog.tblContentPages.pageID, culture);
                    string href = "";

                    if (objSeo != null)
                    {
                        if (objSeo.friendlyUrl != null)
                        {
                            href = objSeo.friendlyUrl;
                        }
                        else
                        {
                            href = objSeo.url;
                        }
                    }
                    else
                    {
                        href = "/pages/" + blog.tblContentPages.pageID;
                    }

                    blogList.Content += "<div class=\"blog-item\">";
                    blogList.Content += "<a href=\"" + href + "\"><img src=\"" + img + "\" /></a><div class=\"content\"><h3><a href=\"" + href + "\">" + h1 + "</a></h3>" + txt + "</div>";
                    blogList.Content += "</div>";
                }

                return blogList;
            }
        }

        public class FreeGetawayDataModel
        {
            ePlatEntities db = new ePlatEntities();
            public AttemptResponse Save(FreeGetawayViewModel model)
            {
                Guid transactionID = Guid.NewGuid();

                AttemptResponse response = new AttemptResponse();
                try
                {
                    MultiFields.SaveValue(181, model.FreeGetaway_FirstName, transactionID);
                    MultiFields.SaveValue(182, model.FreeGetaway_LastName, transactionID);
                    MultiFields.SaveValue(183, model.FreeGetaway_PhoneDay, transactionID);
                    MultiFields.SaveValue(184, model.FreeGetaway_PhoneEvening, transactionID);
                    MultiFields.SaveValue(185, model.FreeGetaway_Email, transactionID);
                    MultiFields.SaveValue(186, model.FreeGetaway_Age, transactionID);
                    MultiFields.SaveValue(187, model.FreeGetaway_MaritalStatus, transactionID);
                    MultiFields.SaveValue(188, model.FreeGetaway_SecurityQuestion, transactionID);
                    MultiFields.SaveValue(189, model.FreeGetaway_SecurityAnswer.ToString(), transactionID);
                    MultiFields.SaveValue(190, HttpContext.Current.Request.Url.OriginalString, transactionID);

                    var cookie = HttpContext.Current.Request.Cookies["dvh_campaign"];
                    string campaign = "Undefined";
                    if (cookie != null)
                    {
                        campaign = cookie.Value;
                    }
                    MultiFields.SaveValue(228, campaign, transactionID);

                    //sending email notification for quote request
                    System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(17);
                    email.To.Add(model.FreeGetaway_Email);
                    email.Body = email.Body
                        .Replace("$FreeGetaway_FirstName", model.FreeGetaway_FirstName)
                        .Replace("$FreeGetaway_LastName", model.FreeGetaway_LastName)
                        .Replace("$FreeGetaway_PhoneDay", model.FreeGetaway_PhoneDay)
                        .Replace("$FreeGetaway_PhoneEvening", model.FreeGetaway_PhoneEvening)
                        .Replace("$FreeGetaway_Email", model.FreeGetaway_Email)
                        .Replace("$FreeGetaway_Age", model.FreeGetaway_Age)
                        .Replace("$FreeGetaway_MaritalStatus", model.FreeGetaway_MaritalStatus)
                        .Replace("$FreeGetaway_SecurityQuestion", model.FreeGetaway_SecurityQuestion)
                        .Replace("$FreeGetaway_SecurityAnswer", model.FreeGetaway_SecurityAnswer)
                        .Replace("$FreeGetaway_Campaign", campaign);

                    //Utils.EmailNotifications.Send(email);
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = "";
                    response.Message = "Thank you for sending us your information!";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    //response.ErrorMessage = ex.Message;
                    response.Exception = ex;
                    response.Message = "Something weird happened trying to save your information, please try again.";
                    return response;
                }
            }
        }

        public class RedeemMyPackageDataModel
        {
            public static AttemptResponse Save(RedeemMyPackageViewModel model)
            {
                AttemptResponse response = new AttemptResponse();
                ePlatEntities db = new ePlatEntities();

                Guid transactionID = Guid.NewGuid();

                //try
                //{
                MultiFields.SaveValue(1183, model.Redeem_FirstName, transactionID);
                MultiFields.SaveValue(1184, model.Redeem_LastName, transactionID);
                MultiFields.SaveValue(1185, model.Redeem_Email, transactionID);
                MultiFields.SaveValue(1186, model.Redeem_Phone, transactionID);
                MultiFields.SaveValue(1187, model.Redeem_CertificateNumber, transactionID);
                MultiFields.SaveValue(1188, model.Redeem_Comments, transactionID);
                MultiFields.SaveValue(1189, HttpContext.Current.Request.Url.OriginalString, transactionID);

                //sending email notification for quote request
                System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(39);
                email.Body = email.Body
                    .Replace("$Redeem_FirstName", model.Redeem_FirstName)
                    .Replace("$Redeem_LastName", model.Redeem_LastName)
                    .Replace("$Redeem_Email", model.Redeem_Email)
                    .Replace("$Redeem_Phone", model.Redeem_Phone)
                    .Replace("$Redeem_CertificateNumber", model.Redeem_CertificateNumber);

                if (model.Redeem_Comments != null)
                {
                    email.Body = email.Body.Replace("$Redeem_Comments", model.Redeem_Comments.ToString());
                }
                else
                {
                    email.Body = email.Body.Replace("$Redeem_Comments", "");
                }

                    //Utils.EmailNotifications.Send(email);
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = "";
                response.Message = "Thank you. We will contact you soon!";
                //}
                //catch (Exception ex)
                //{
                //    response.Type = Attempt_ResponseTypes.Error;
                //    response.ObjectID = 0;
                //    //response.ErrorMessage = ex.Message;
                //    response.Exception = ex;
                //    response.Message = "Something weird happened trying to save your information, please try again.";
                //}

                return response;
            }
        }

        public class QuoteRequestDataModel
        {
            ePlatEntities db = new ePlatEntities();
            public AttemptResponse Save(QuoteRequestViewModel model)
            {
                Guid transactionID = Guid.NewGuid();
                long? terminalID = Utils.GeneralFunctions.GetTerminalID();

                AttemptResponse response = new AttemptResponse();
                try
                {
                    DateTime arrival = DateTime.Parse(model.QuoteRequest_Arrival);
                    DateTime departure = DateTime.Parse(model.QuoteRequest_Departure);

                    MultiFields.SaveValue(6, model.QuoteRequest_FirstName, transactionID);
                    MultiFields.SaveValue(7, model.QuoteRequest_LastName, transactionID);
                    MultiFields.SaveValue(8, model.QuoteRequest_Email, transactionID);
                    MultiFields.SaveValue(9, model.QuoteRequest_Phone, transactionID);
                    MultiFields.SaveValue(10, model.QuoteRequest_Destination, transactionID);
                    MultiFields.SaveValue(11, model.QuoteRequest_Resort, transactionID);
                    MultiFields.SaveValue(12, model.QuoteRequest_Arrival, transactionID);
                    MultiFields.SaveValue(13, model.QuoteRequest_Departure, transactionID);
                    MultiFields.SaveValue(14, model.QuoteRequest_Adults.ToString(), transactionID);
                    MultiFields.SaveValue(15, model.QuoteRequest_Children.ToString(), transactionID);
                    MultiFields.SaveValue(16, model.QuoteRequest_Comments, transactionID);
                    MultiFields.SaveValue(17, HttpContext.Current.Request.Url.OriginalString, transactionID);
                    MultiFields.SaveValue(25, model.QuoteRequest_TimeToReach, transactionID);


                    var cookie = HttpContext.Current.Request.Cookies["dvh_campaign"];
                    string campaign = "Undefined";
                    if (cookie != null)
                    {
                        campaign = cookie.Value;
                    }
                    MultiFields.SaveValue(226, campaign, transactionID);

                    //Enviar Lead a WebHoook
                    var WebHookID = (from w in db.tblWebhooks
                                     where w.terminalID == terminalID
                                     && w.eventID == 2
                                     select w.webhookID).FirstOrDefault();

                    if (WebHookID != null)
                    {
                        string whURL = "https://developers.eplat.villagroup.com/api/webhooks/incoming/genericjson/newlead?code=80f32f6123104d09a72c000047564e51";

                        WebhooksViewModel.NewLeadWH newLead = new WebhooksViewModel.NewLeadWH();

                        newLead.webhookID = WebHookID;
                        newLead.referenceID = null;
                        newLead.firstName = model.QuoteRequest_FirstName;
                        newLead.lastName = model.QuoteRequest_LastName;
                        newLead.phone = model.QuoteRequest_Phone;
                        newLead.email = model.QuoteRequest_Email;
                        newLead.destination = model.QuoteRequest_Destination;
                        newLead.timeToReach = model.QuoteRequest_TimeToReach;
                        if (model.QuoteRequest_Arrival != null)
                        {
                            newLead.arrivalDate = DateTime.Parse(model.QuoteRequest_Arrival);
                        }
                        if (model.QuoteRequest_Departure != null)
                        {
                            newLead.departureDate = DateTime.Parse(model.QuoteRequest_Departure);
                        }
                        newLead.notes = "Resort: " + model.QuoteRequest_Resort;
                        newLead.notes += "<br />Adults: " + model.QuoteRequest_Adults;
                        newLead.notes += "<br />Children: " + model.QuoteRequest_Children;
                        newLead.notes += "<br />Comments: " + model.QuoteRequest_Comments;

                        using (WebClient Client = new WebClient())
                        {
                            var json = Newtonsoft.Json.JsonConvert.SerializeObject(newLead);
                            Client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            Client.UploadString(whURL, json);
                        }
                    }

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = "";
                    response.Message = "Thank you for sending us your Request. We will contact you soon!";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    //response.ErrorMessage = ex.Message;
                    response.Exception = ex;
                    response.Message = "Something weird happened trying to save your information, please try again.";
                    return response;
                }
            }

            public static List<SelectListItem> GetDestinationsListTextValue()
            {
                ePlatEntities db = new ePlatEntities();
                long terminalID = Utils.GeneralFunctions.GetTerminalID();
                var destinationsInTerminal = from p in db.tblTerminals_Destinations
                                             where p.terminalID == terminalID
                                             select p.destinationID;

                var Destinations = from d in db.tblDestinations
                                   where destinationsInTerminal.Contains(d.destinationID)
                                   select d.destination;
                List<SelectListItem> destinations = new List<SelectListItem>();
                destinations.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select one"
                });
                foreach (string destination in Destinations)
                {
                    destinations.Add(new SelectListItem()
                    {
                        Value = destination,
                        Text = destination
                    });
                }
                return
                 destinations;
            }

            public static List<SelectListItem> GetResortsListTextValue(string destination)
            {
                ePlatEntities db = new ePlatEntities();
                long terminalID = Utils.GeneralFunctions.GetTerminalID();

                var Resorts = (from r in db.tblPackageSettings
                               where r.tblPackages.tblCategories_Packages.Count(x => x.tblCategories.category == destination) > 0
                               && r.tblPackages.tblCategories_Packages.Count(x => x.tblCategories.tblCatalogs.tblCatalogs_Terminals.FirstOrDefault().terminalID == terminalID) > 0
                               && r.tblPackages.active
                               select r.tblPlaces.place).Distinct();

                List<SelectListItem> resorts = new List<SelectListItem>();
                resorts.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = (destination == "" ? "Select a destination" : "Any Resort")
                });
                foreach (string resort in Resorts)
                {
                    resorts.Add(new SelectListItem()
                    {
                        Value = resort,
                        Text = resort
                    });
                }

                return resorts;
            }
        }

        public class SpecialPromoForCityDataModel
        {
            public static SpecialPromoForCityViewModel GetPromo()
            {
                string city = string.Empty;
                string region = string.Empty;
                string country = string.Empty;
                string bannerAd = string.Empty;
                string phoneDesktop = string.Empty;
                string phoneMobile = string.Empty;

                try
                {
                    XDocument doc = XDocument.Load("http://api.ipinfodb.com/v3/ip-city/?key=3c6ede46b04fada26c0be61d50bd9441cd5a052f94f6d015a0cca483cb505399&ip=" + HttpContext.Current.Request.UserHostAddress.ToString() + "&format=xml");
                    var ipQuery = from x in doc.Descendants("Response")
                                  select new
                                  {
                                      city = x.Element("cityName").Value,
                                      region = x.Element("regionName").Value,
                                      country = x.Element("countryCode").Value
                                  };

                    if (ipQuery.Count() > 0)
                    {
                        city = ipQuery.FirstOrDefault().city;
                        region = ipQuery.FirstOrDefault().region;
                        country = ipQuery.FirstOrDefault().country;
                    }

                    bannerAd = "<img src=\"/Content/themes/dvh/images/special.jpg\" />"
                    + "<div class=\"promo\">"
                    + "<span class=\"close\"></span>"
                    + "<span class=\"title\">Special Promo</span>"
                    + "<span class=\"greeting\">Hey, are you from</span>"
                    + "<span class=\"city\">$City?</span>"
                    + "<span class=\"action\">Call to our toll free number</span>"
                    + "<span class=\"phone-number\">"
                    + "<span class=\"phone-desktop\">$PhoneDesktop</span>"
                    + "<span class=\"phone-mobile\">$PhoneMobile</span>"
                    + "</span>"
                    + "<span class=\"action2\">and ask for</span>"
                    + "<span class=\"promocode\">$City Discount</span>"
                    + "<span class=\"discount\">We have an additional discount for you today!</span>"
                    + "</div>";

                    DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
                    string culture = Utils.GeneralFunctions.GetCulture();
                    phoneDesktop = objMaster.Template_Phone_Desktop;
                    phoneMobile = objMaster.Template_Phone_Mobile;
                }
                catch (Exception e)
                {
                }

                return new SpecialPromoForCityViewModel()
                {
                    BannerAd = bannerAd,
                    PhoneDesktop = phoneDesktop,
                    PhoneMobile = phoneMobile,
                    City = city,
                    Region = region,
                    Country = country
                };

            }
        }

        public class TransportationQuotes
        {
            public static List<SelectListItem> GetResorts(string terminals)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                ePlatEntities db = new ePlatEntities();

                var terminalids = terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var destinations = from d in db.tblTerminals_Destinations
                                   where terminalids.Contains(d.terminalID)
                                   select d.destinationID;

                var resorts = from r in db.tblPlaces
                              where r.placeTypeID == 1
                              && destinations.Contains(r.destinationID)
                              orderby r.place
                              select new { r.placeID, r.place };

                foreach (var resort in resorts)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = resort.place,
                        Value = resort.placeID.ToString()
                    });
                }

                return list;
            }
        }

        public class Excel
        {
            public static string GenerateBasicFile(string table)
            {
                string fileString = "<!doctype html><html lang=\"es\"><head><meta charset=\"utf-8\"></head><body>";
                fileString += table;
                fileString += "</body></html>";
                return fileString;
            }
        }
    }
}
