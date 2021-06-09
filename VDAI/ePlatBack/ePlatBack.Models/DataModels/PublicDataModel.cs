using System;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ePlatBack.Models.ViewModels;
using System.Web.Script.Serialization;
using ePlatBack.Models.Utils;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Mail;
using ePlatBack.Models.eplatformDataModel;
using System.Runtime.InteropServices.WindowsRuntime;
using RestSharp;
using Newtonsoft.Json;
//using Microsoft.AspNet.Scaffolding;

namespace ePlatBack.Models.DataModels
{
    public class PublicDataModel
    {
        public PaymentConfirmationModel GetPurchaseConfirmationInfo(Guid? transaction)
        {
            ePlatEntities db = new ePlatEntities();
            PaymentConfirmationModel model = new PaymentConfirmationModel();

            if (transaction != null)
            {
                var query = db.tblFieldsValues.Where(m => m.transactionID == transaction);

                model.FirstName = query.Count(m => m.tblFields.field == "$FirstName") > 0 ? query.Where(m => m.tblFields.field == "$FirstName").FirstOrDefault().value : "";
                model.LastName = query.Count(m => m.tblFields.field == "$LastName") > 0 ? query.Where(m => m.tblFields.field == "$LastName").FirstOrDefault().value : "";
                model.UserFirstName = query.Count(m => m.tblFields.field == "$UserFirstName") > 0 ? query.Where(m => m.tblFields.field == "$UserFirstName").FirstOrDefault().value : "";
                model.OptionsSold = query.Count(m => m.tblFields.field == "$OptionsSold") > 0 ? query.Where(m => m.tblFields.field == "$OptionsSold").FirstOrDefault().value : "";
                model.Resort = query.Count(m => m.tblFields.field == "$Resort") > 0 ? query.Where(m => m.tblFields.field == "$Resort").FirstOrDefault().value : "";
                model.ConfirmationNumber = query.Count(m => m.tblFields.field == "$ConfirmationNumber") > 0 ? query.Where(m => m.tblFields.field == "$ConfirmationNumber").FirstOrDefault().value : "";
                model.ArrivalDate = query.Count(m => m.tblFields.field == "$ArrivalDate") > 0 ? query.Where(m => m.tblFields.field == "$ArrivalDate").FirstOrDefault().value : "";
                model.DepartureDate = query.Count(m => m.tblFields.field == "$DepartureDate") > 0 ? query.Where(m => m.tblFields.field == "$DepartureDate").FirstOrDefault().value : "";
                model.Amount = query.Count(m => m.tblFields.field == "$Amount") > 0 ? query.Where(m => m.tblFields.field == "$Amount").FirstOrDefault().value : "";
                model.Invoice = query.Count(m => m.tblFields.field == "$Invoice") > 0 ? query.Where(m => m.tblFields.field == "$Invoice").FirstOrDefault().value : "";
                model.PaymentDate = query.Count(m => m.tblFields.field == "$PaymentDate") > 0 ? query.Where(m => m.tblFields.field == "$PaymentDate").FirstOrDefault().value : "";
                model.CardHolder = query.Count(m => m.tblFields.field == "$CardHolder") > 0 ? query.Where(m => m.tblFields.field == "$CardHolder").FirstOrDefault().value : "";
                model.CardType = query.Count(m => m.tblFields.field == "$CardType") > 0 ? query.Where(m => m.tblFields.field == "$CardType").FirstOrDefault().value : "";
                model.CardNumber = query.Count(m => m.tblFields.field == "$CardNumber") > 0 ? query.Where(m => m.tblFields.field == "$CardNumber").FirstOrDefault().value : "";
                model.MerchantAccount = query.Count(m => m.tblFields.field == "$MerchantAccount") > 0 ? query.Where(m => m.tblFields.field == "$MerchantAccount").FirstOrDefault().value : "";
                model.PaymentInformation = query.Count(m => m.tblFields.field == "$PaymentInfo") > 0 ? query.Where(m => m.tblFields.field == "$PaymentInfo").FirstOrDefault().value : "";
            }
            else
            {
                model.FirstName = "";
                model.LastName = "";
                model.UserFirstName = "";
                model.Resort = "";
                model.ConfirmationNumber = "";
                model.ArrivalDate = "";
                model.DepartureDate = "";
                model.Amount = "";
                model.Invoice = "";
                model.PaymentDate = "";
                model.CardHolder = "";
                model.CardType = "";
                model.CardNumber = "";
                model.MerchantAccount = "";
                model.PaymentInformation = "";
            }
            return model;
        }

        public object GetSignature(Guid transaction, string ip)
        {
            ePlatEntities db = new ePlatEntities();
            var query = from fv in db.tblFieldsValues
                        join f in db.tblFields on fv.fieldID equals f.fieldID
                        where fv.transactionID == transaction
                        && f.field.ToLower() == "$signature"
                        select fv;

            var response = new { IP = ip, signature = (query.Count() > 0 ? query.FirstOrDefault().value : ""), signed = (query.Count() > 0 ? query.FirstOrDefault().value != null : false) };
            return response;
        }

        public bool SetSignature(Guid transaction, string signature)
        {
            ePlatEntities db = new ePlatEntities();

            try
            {
                var fv = db.tblFieldsValues.Where(m => m.transactionID == transaction);
                var field = fv.Where(m => m.tblFields.field.ToLower() == "$signature");

                if (field.Count() > 0)
                {
                    field.FirstOrDefault().value = signature;
                }
                else
                {
                    var signatureField = db.tblFields.Where(m => m.fieldGroupID == fv.FirstOrDefault().tblFields.fieldGroupID && m.field == "$signature");
                    var newValue = new tblFieldsValues();

                    if (signatureField.Count() > 0)
                    {
                        newValue.fieldID = signatureField.FirstOrDefault().fieldID;
                        newValue.value = signature;
                        newValue.terminalID = fv.FirstOrDefault().terminalID;
                        newValue.dateSaved = DateTime.Now;
                        newValue.transactionID = transaction;
                        db.tblFieldsValues.AddObject(newValue);
                    }
                    else
                    {
                        var newField = new tblFields();
                        newField.fieldGuid = Guid.NewGuid();
                        newField.field = "$Signature";
                        newField.fieldGroupID = fv.FirstOrDefault().tblFields.fieldGroupID;
                        newField.description = "$Signature";
                        newField.fieldTypeID = 1;
                        newField.fieldSubTypeID = 7;
                        newField.visibility = 0;
                        newField.order_ = 0;

                        newValue.value = signature;
                        newValue.terminalID = fv.FirstOrDefault().terminalID;
                        newValue.dateSaved = DateTime.Now;
                        newValue.transactionID = transaction;

                        newField.tblFieldsValues.Add(newValue);
                        db.tblFields.AddObject(newField);
                    }
                }
                db.SaveChanges();
                //generar correo
                var pdm = new PreArrivalDataModel();
                var email = EmailNotifications.GetEmailByNotification(259);
                var reservation = fv.Where(m => m.tblFields.field.ToLower() == "$itemid");
                if (reservation.Count() > 0)
                {
                    var id = Guid.Parse(reservation.FirstOrDefault().value);
                    var r = db.tblReservations.FirstOrDefault(m => m.reservationID == id);

                    email.Body = email.Body.Replace("$FirstName", r.tblLeads.firstName)
                        .Replace("$LastName", r.tblLeads.lastName)
                        .Replace("$Resort", r.tblPlaces.place + " " + r.tblDestinations.destination)
                        .Replace("$ConfirmationNumber", r.hotelConfirmationNumber);

                    EmailNotifications.Send(email);
                }


                return true;
            }
            catch
            {
                return false;
            }
        }

        public EmailPreViewModel ReplaceFieldsValues(System.Net.Mail.MailMessage mail, string culture, List<KeyValue> fieldsValues, int fieldGroupID, Guid? transactionID = null)
        {
            ePlatEntities db = new ePlatEntities();
            tblEmailNotifications notification = new tblEmailNotifications();
            PaymentConfirmationModel model = new PaymentConfirmationModel();//new

            var preview = new EmailPreViewModel();

            var email = mail;

            if (notification != null)
            {
                preview.Transaction = transactionID != null ? transactionID.ToString() : null;
                preview.FieldGroup = fieldGroupID.ToString();
                preview.FromAddress = email.From.Address;
                preview.FromAlias = email.From.DisplayName;
                preview.Subject = email.Subject;
                preview.To = string.Join(", ", email.To.Select(m => new { item = m.DisplayName + " " + m.Address }).Select(m => m.item));
                preview.ReplyTo = string.Join(", ", email.ReplyToList.Select(m => new { item = m.DisplayName + " " + m.Address }).Select(m => m.item));
                preview.CC = string.Join(", ", email.CC.Select(m => new { item = m.DisplayName + " " + m.Address }).Select(m => m.item));
                preview.BCC = string.Join(", ", email.Bcc.Select(m => new { item = m.DisplayName + " " + m.Address }).Select(m => m.item));

                preview.Body = email.Body.Replace("á", "&aacute;")
                    .Replace("é", "&eacute;")
                    .Replace("í", "&iacute;")
                    .Replace("ó", "&oacute;")
                    .Replace("ú", "&uacute;")
                    .Replace("ñ", "&ntilde;")
                    .Replace("Á", "&Aacute;")
                    .Replace("É", "&Eacute;")
                    .Replace("Í", "&Iacute;")
                    .Replace("Ó", "&Oacute;")
                    .Replace("Ú", "&Uacute;")
                    .Replace("Ñ", "&Ntilde;");
                preview.ListFieldsValues = fieldsValues;
                foreach (var pair in fieldsValues)
                {
                    preview.Subject = preview.Subject.Replace(pair.Key, pair.Value);
                    preview.Body = preview.Body.Replace(pair.Key, pair.Value);
                }
            }

            return preview;
        }

        public EmailPreViewModel _ReplaceFieldsValues(System.Net.Mail.MailMessage mail, string culture, List<KeyValue> fieldsValues, int fieldGroupID, Guid? transactionID = null)
        {
            ePlatEntities db = new ePlatEntities();
            tblEmailNotifications notification = new tblEmailNotifications();
            var preview = new EmailPreViewModel();

            var email = mail;

            if (notification != null)
            {
                //var email = Utils.EmailNotifications.GetEmailByNotification(notification.emailNotificationID);
                preview.Transaction = transactionID != null ? transactionID.ToString() : null;
                preview.FieldGroup = fieldGroupID.ToString();
                preview.FromAddress = email.From.Address;
                preview.FromAlias = email.From.DisplayName;
                preview.Subject = email.Subject;
                preview.To = string.Join(", ", email.To.Select(m => new { item = m.DisplayName + " " + m.Address }).Select(m => m.item));
                preview.ReplyTo = string.Join(", ", email.ReplyToList.Select(m => new { item = m.DisplayName + " " + m.Address }).Select(m => m.item));
                preview.CC = string.Join(", ", email.CC.Select(m => new { item = m.DisplayName + " " + m.Address }).Select(m => m.item));
                preview.BCC = string.Join(", ", email.Bcc.Select(m => new { item = m.DisplayName + " " + m.Address }).Select(m => m.item));
                //preview.Body = email.Body;
                preview.Body = email.Body.Replace("á", "&aacute;")
                    .Replace("é", "&eacute;")
                    .Replace("í", "&iacute;")
                    .Replace("ó", "&oacute;")
                    .Replace("ú", "&uacute;")
                    .Replace("ñ", "&ntilde;")
                    .Replace("Á", "&Aacute;")
                    .Replace("É", "&Eacute;")
                    .Replace("Í", "&Iacute;")
                    .Replace("Ó", "&Oacute;")
                    .Replace("Ú", "&Uacute;")
                    .Replace("Ñ", "&Ntilde;");
                preview.ListFieldsValues = fieldsValues;
                foreach (var pair in fieldsValues)
                {
                    preview.Subject = preview.Subject.Replace(pair.Key, pair.Value);
                    preview.Body = preview.Body.Replace(pair.Key, pair.Value);
                }
            }

            return preview;
        }

        //public NotificationsModel SendSalesRoomsReport(NotificationsModel.SearchNotificationsModel model, bool showVLOClassification, System.Web.HttpRequestBase request)
        public NotificationsModel SendSalesRoomsReport(NotificationsModel.SearchNotificationsModel model, bool showVLOClassification, System.Web.HttpRequestBase request, ref string masterEmailBody, ref ePlatEntities context)
        {

            var response = new NotificationsModel();

            //var content = new ReportDataModel().GetNotificationsReport(model, showVLOClassification, request);
            var content = new ReportDataModel().GetNotificationsReport(model, showVLOClassification, request, ref masterEmailBody, ref context);
            response.FromDate = model.SearchNotifications_I_Date;
            response.ToDate = model.SearchNotifications_F_Date;
            response.ListNotifications = content;
            response.ShowVLOClassification = showVLOClassification;

            return response;
        }

        public void SendPreArrivalPurchaseReminder(System.Web.HttpContextBase context)
        {
            ePlatEntities db = new ePlatEntities();
            PreArrivalDataModel pdm = new PreArrivalDataModel();

            var today = DateTime.Today;

            var notifications = from n in db.tblEmailNotifications
                                where n.tblSysEvents.@event == "Reminder"
                                //&& n.active == true
                                && n.emailNotificationID == 201 || n.emailNotificationID == 202
                                select n;


            foreach (var notification in notifications)
            {
                var description = notification.description;
                var terminal = notification.terminalID;
                var destinations = notification.tblEmailNotifications_Destinations.Count() > 0 ? notification.tblEmailNotifications_Destinations.Select(m => (long?)m.destinationID).ToArray() : new long?[] { };
                var resorts = notification.tblEmailNotifications_Places.Count() > 0 ? notification.tblEmailNotifications_Places.Select(m => (long?)m.placeID).ToArray() : new long?[] { };
                var sources = notification.tblEmailNotifications_LeadSources.Count() > 0 ? notification.tblEmailNotifications_LeadSources.Select(m => (long?)m.leadSourceID).ToArray() : new long?[] { };
                var leadStatus = notification.tblEmailNotifications_LeadStatus.Count() > 0 ? notification.tblEmailNotifications_LeadStatus.Select(m => (int?)m.leadStatusID).ToArray() : new int?[] { };
                var bookingStatus = notification.tblEmailNotifications_BookingStatus.Count() > 0 ? notification.tblEmailNotifications_BookingStatus.Select(m => (int?)m.bookingStatusID).ToArray() : new int?[] { };
                var requiredFields = notification.requiredFields != null ? GeneralFunctions.RequiredFields.Where(m => notification.requiredFields.Split(',').ToArray().Contains(m.Key)).Select(m => m.Value).ToList() : null;

                var days = (int)notification.days;
                //var before = notification.before;
                //var compareTo = notification.compareTo;
                var before = true;
                var compareTo = "arrivalDate";
                var date = today.AddDays(days * (before == false ? -1 : 1));

                //revise if notification needs optionsSold in order to be implemented

                var query = from rsv in db.tblReservations
                            join l in db.tblLeads on rsv.leadID equals l.leadID into rsv_l
                            from l in rsv_l.DefaultIfEmpty()
                            where l.terminalID == terminal
                            && (sources.Count() == 0 || sources.Contains(l.leadSourceID))
                            && (leadStatus.Count() == 0 || leadStatus.Contains(l.leadStatusID))
                            && (bookingStatus.Count() == 0 || bookingStatus.Contains(l.bookingStatusID))
                            && (destinations.Count() == 0 || destinations.Contains(rsv.destinationID))
                            && (resorts.Count() == 0 || resorts.Contains(rsv.placeID))
                            && ((compareTo == "arrivalDate" && rsv.arrivalDate == date)
                            || (compareTo == "departureDate" && rsv.departureDate == date)
                            )
                            select rsv;

                foreach (var rsv in query)
                {
                    var missingFields = ValidateRequiredFields(requiredFields, rsv);

                    if (missingFields == "")
                    {
                        //var emailObj = pdm.PreviewEmail(rsv.reservationID, notification.emailNotificationID, null, context);
                        var emailObj = pdm.PreviewEmail(rsv.reservationID, notification.emailNotificationID, null, null);
                        emailObj.ReservationID = rsv.reservationID.ToString();
                        emailObj.LeadID = rsv.leadID.ToString();
                        emailObj.To = "efalcon@villagroup.com";
                        emailObj.CC = "";

                        //var sendResponse = pdm.SendEmail(new JavaScriptSerializer().Serialize(emailObj), true, context);
                        var sendResponse = pdm.SendEmail(new JavaScriptSerializer().Serialize(emailObj), true, null);
                        //condicionar el guardado en tblFieldsValues de los correos enviados.
                        //revisar el metodo que guarda y envía PreArrivalDataModel.SendEmail>NotificationsDataModel.SaveFieldValues
                    }
                }
            }


        }

        public static void ReplaceReservedWords(tblReservations query, List<string> fields, ref List<KeyValue> list, System.Web.HttpContextBase context)
        {
            ePlatEntities db = new ePlatEntities();

            var session = context != null && context.User.Identity.IsAuthenticated ? new UserSession() : null;

            #region "get fields values"
            foreach (var field in fields)
            {
                switch (field)
                {
                    case "$Airline":
                        {
                            var airline = query.tblFlights.Count(m => m.flightTypeID == 1) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 1).OrderByDescending(m => m.flightDateTime).FirstOrDefault().tblAirLines.airLine : "";
                            list.Add(new KeyValue() { Key = field, Value = airline });
                            break;
                        }
                    case "$Amount":
                        {
                            //los reembolsos no se guardan como negativo. convertirlos aquí para la sumatoria
                            var amount = query.tblPaymentDetails.Where(m => m.deleted != true && m.tblMoneyTransactions.authCode != null && (m.tblMoneyTransactions.authCode != "" || m.paymentType != 2)).Sum(m => (m.tblMoneyTransactions.transactionTypeID == 1 ? m.amount : (m.amount * -1))).ToString();
                            list.Add(new KeyValue() { Key = field, Value = amount });
                            break;
                        }
                    case "$ArrivalDate":
                        {
                            list.Add(new KeyValue() { Key = field, Value = (query.arrivalDate != null ? ((DateTime)query.arrivalDate).ToString("yyyy-MM-dd") : "") });
                            break;
                        }
                    case "$ServicesPurchasedInfo":
                        {
                            var services = "";
                            if (query.tblOptionsSold.Count() > 0 && query.tblOptionsSold.Count(m => m.deleted != true && m.tblOptionTypes.optionType != "Transportation" && m.tblOptionTypes.optionType != "Flights") > 0)
                            {
                                var s = query.tblOptionsSold.Where(m => m.deleted != true && m.tblOptionTypes.optionType != "Transportation" && m.tblOptionTypes.optionType != "Flights");
                                services += "<table align=\"center\" style=\"font-family:Verdana;font-size:10pt;text-align:center;border-collapse:separate;border-spacing:15px;\">"
                                            + "<thead><tr><th>Quantity</th>"
                                                    + "<th>Service / Amenitie</th>"
                                                    + "<th>Comments</th>"
                                                    + "</tr></thead><tbody style=\"text-align: center;font-size:10pt;\">";
                                foreach (var i in s)
                                {
                                    services += "<tr>"
                                            + "<td>" + i.quantity + "</td>"
                                            + "<td>" + i.tblOptions.optionName + "</td>"
                                            + "<td>" + i.comments + "</td>"
                                            + "</tr>";
                                }
                                services += "</tbody></table>";
                            }
                            list.Add(new KeyValue() { Key = field, Value = services });
                            break;
                        }
                    case "$ArrivalsFlightInfo":
                        {
                            var arrivals = "";

                            if (session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray().Contains(10))
                            {
                                var options = GetResortConnectOptionals(query);
                                if (query.tblFlights.Count(m => m.flightTypeID == 1) > 0)
                                {
                                    arrivals += "<table align=\"center\" style=\"font-family:Verdana;font-size:10pt;text-align:center;border-collapse:separate;border-spacing:15px;\">"
                                    + "<thead><tr><th>Arrival Date</th>"
                                                + "<th>Airline</th>"
                                                + "<th>Flight Number</th>"
                                                + "<th>Arrival Time</th>"
                                                + "<th>Passengers</th><th>Passengers Names</th></tr></thead><tbody style=\"text-align: center;font-size:10pt;\">";
                                    foreach (var i in query.tblFlights.Where(m => m.flightTypeID == 1).OrderByDescending(m => m.flightDateTime))
                                    {
                                        arrivals += "<tr>"
                                            + "<td>" + i.flightDateTime.Date.ToString("MMM dd, yyyy") + "</td>"
                                            + "<td>" + i.tblAirLines.airLine + "</td>"
                                            + "<td>" + i.flightNumber + "</td>"
                                            + "<td>" + (i.flightDateTime != null ? GeneralFunctions.DateFormat.ToMilitarHour(i.flightDateTime.TimeOfDay.ToString()) : "") + "</td>"
                                            + "<td>" + i.passengers.ToString() + "</td>"
                                            + "<td>" + i.passengersNames + "</td>"
                                            + "</tr>";
                                    }
                                    arrivals += "</tbody></table>";
                                }
                                else if (options.Count(m => m.ProductName.Contains("rrival") || m.ProductName.Contains("Airport to Resort")) > 0)
                                {
                                    arrivals += "<table align=\"center\" style=\"font-family:Verdana;font-size:10pt;text-align:center;border-collapse:separate;border-spacing:15px;\">"
                                    + "<thead><tr>";
                                    if (options.Count(m => (m.ProductName.Contains("rrival") || m.ProductName.Contains("Airport to Resort")) && m.CarrierName != null) > 0)
                                    {
                                        arrivals += "<th>Carrier</th><th>Flight Number</th><th>Notes</th>";
                                    }
                                    else
                                    {
                                        arrivals += "<th>Flight(s) Information</th>";
                                    }
                                    arrivals += "</tr></thead><tbody style=\"text-align: center;font-size:10pt;\">";

                                    foreach (var i in options.Where(m => m.ProductName.Contains("rrival") || m.ProductName.Contains("Airport to Resort")))
                                    {
                                        arrivals += "<tr>";
                                        if (i.CarrierName != null)
                                        {
                                            arrivals += "<td>" + i.CarrierName + "</td><td>" + i.FlightNumber + "</td><td>" + i.Note + "</td>";
                                        }
                                        else
                                        {
                                            arrivals += "<td>" + i.Note + "</td>";
                                        }

                                        arrivals += "</tr>";
                                    }
                                    arrivals += "</tbody></table>";
                                }
                            }
                            else
                            {
                                if (query.tblFlights.Count(m => m.flightTypeID == 1) > 0)
                                {
                                    //arrivals
                                    arrivals += "<table align=\"center\" style=\"font-family:Verdana;font-size:10pt;text-align:center;border-collapse:separate;border-spacing:15px;\">"
                                        + "<thead><tr><th>Arrival Date &amp; Time</th>"
                                                    + "<th>Airline</th>"
                                                    + "<th>Flight Number</th>"
                                                    //+ "<th>Arrival Time</th>"
                                                    + "<th>Destination</th>"
                                                    + "<th>Passengers</th><th>Passengers Names</th>"
                                                    //+ "<th>Meeting Point</th>"
                                                    + "</tr></thead><tbody style=\"text-align: center;font-size:10pt;\">";
                                    foreach (var i in query.tblFlights.Where(m => m.flightTypeID == 1).OrderByDescending(m => m.flightDateTime))
                                    {
                                        arrivals += "<tr>"
                                            //+ "<td>" + i.flightDateTime.Date.ToString("MMM dd, yyyy") + "</td>"
                                            + "<td>" + i.flightDateTime.Date.ToString("MMM dd, yyyy")
                                            + "<br />" + (i.flightDateTime != null ? i.flightDateTime.TimeOfDay.ToString(@"hh\:mm") : "")
                                            + "</td>"
                                            + "<td>" + i.tblAirLines.airLine + "</td>"
                                            + "<td>" + i.flightNumber + "</td>"
                                            //+ "<td>" + (i.flightDateTime != null ? GeneralFunctions.DateFormat.ToMilitarHour(i.flightDateTime.TimeOfDay.ToString()) : "") + "</td>"
                                            + "<td>" + query.tblPlaces.place + " " + query.tblPlaces.tblDestinations.destination + "</td>"
                                            + "<td>" + i.passengers.ToString() + "</td>"
                                            + "<td>" + i.passengersNames + "</td>"
                                            //+ "<td>Outside of the terminal, after the baggage claim</td>"
                                            + "</tr>";
                                    }
                                    arrivals += "</tbody></table>";
                                }
                            }
                            list.Add(new KeyValue() { Key = field, Value = arrivals });
                            break;
                        }
                    case "$ArrivalTime":
                        {
                            var _time = query.tblFlights.Count(m => m.flightTypeID == 1) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 1).OrderByDescending(m => m.flightDateTime).FirstOrDefault().flightDateTime.TimeOfDay : (TimeSpan?)null;
                            var time = _time != null ? GeneralFunctions.DateFormat.ToMeridianHour(_time.ToString()) : "";
                            list.Add(new KeyValue() { Key = field, Value = time });
                            break;
                        }
                    case "$AssignedToUser":
                        {
                            var profile = db.tblUserProfiles.Single(m => m.userID == query.tblLeads.aspnet_Users1.UserId);
                            var usr = profile.firstName + " " + profile.lastName;
                            list.Add(new KeyValue() { Key = field, Value = usr });
                            break;
                        }
                    case "$PaymentInfo":
                        {
                            var str = "";
                            //var payments = query.tblPaymentDetails.Where(m => m.deleted != true && (m.tblMoneyTransactions.errorCode == "" || m.tblMoneyTransactions.errorCode == "0"));
                            var payments = query.tblPaymentDetails.Where(m => m.deleted != true && m.tblMoneyTransactions.errorCode == "0");
                            if (payments.Count() > 0)
                            {
                                var types = payments.Select(m => m.paymentType).Distinct();
                                foreach (var i in types)
                                {
                                    str += "<table style=\"width:100%;margin-left:auto;margin-right:auto;font-family:verdana;text-align:center;font-size:10pt;border-collapse:separate;border-spacing:15px;\" align=\"center\"><thead>";

                                    switch (i)
                                    {
                                        case 2://cc
                                            {
                                                str += "<tr><th colspan=\"5\" style=\"text-align:center;\">" + GeneralFunctions.PaymentTypes.Where(m => m.Key == Convert.ToString(i)).Select(m => m.Value).FirstOrDefault() + "</th></tr>";
                                                str += "<tr><th>Date</th><th>Card Info</th><th>Holder Name</th><th>Amount</th><th>Invoice</th></tr>";
                                                str += "</thead><tbody style=\"text-align:center;font-size:10pt;\">";
                                                foreach (var a in payments.Where(m => m.paymentType == i))
                                                {
                                                    str += "<tr><td style=\"text-align:left;\">" + a.dateSaved.ToString("yyyy-MM-dd") + "</td>"
                                                        + "<td>" + (a.tblMoneyTransactions.billingInfoID != null ? a.tblMoneyTransactions.tblBillingInfo.tblCardTypes.cardType : (a.ccType != null ? a.tblCardTypes.cardType : ""))
                                                        + "-" + (a.tblMoneyTransactions.billingInfoID != null ? GeneralFunctions.MaskCreditCard(mexHash.mexHash.DecryptString(a.tblMoneyTransactions.tblBillingInfo.cardNumber)).Substring(12) : (a.ccReferenceNumber != null ? a.ccReferenceNumber.ToString() : "")) + "</td>"
                                                        + "<td>" + (a.tblMoneyTransactions.billingInfoID != null ? a.tblMoneyTransactions.tblBillingInfo.cardHolderName : "") + "</td>"
                                                        + "<td style=\"" + (a.tblMoneyTransactions.transactionTypeID == 2 ? "color:red;" : "") + "\">" + (a.tblMoneyTransactions.transactionTypeID == 1 ? "" : "-") + " $" + a.amount + "</td>"
                                                        + "<td>" + a.tblMoneyTransactions.authCode + "</td></tr>";
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                str += "<tr><th colspan=\"2\" style=\"text-align:center;\">" + GeneralFunctions.PaymentTypes.Where(m => m.Key == Convert.ToString(i)).Select(m => m.Value).FirstOrDefault() + "</th></tr>";
                                                str += "<tr><th>Date</th><th>Amount</th></tr>";
                                                str += "</thead><tbody style=\"text-align:center;font-size:10pt;\">";
                                                foreach (var a in payments.Where(m => m.paymentType == i))
                                                {
                                                    str += "<tr><td>" + a.dateSaved.ToString("yyyy-MM-dd") + "</td>"
                                                        + "<td>" + (a.tblMoneyTransactions.transactionTypeID == 1 ? "" : "-") + " $" + a.amount + "</td></tr>";
                                                }
                                                break;
                                            }
                                    }
                                    str += "</tbody></table>";
                                }
                            }
                            list.Add(new KeyValue() { Key = field, Value = str });
                            break;
                        }
                    case "$AgencyName":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.frontOfficeAgencyName != null ? query.frontOfficeAgencyName : "" });
                            break;
                        }
                    case "$CardHolder":
                        {
                            var holders = "";
                            if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2).Count() > 0)
                            {
                                if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions).Count(m => m.billingInfoID != null) > 0)
                                {
                                    //holders = string.Join(", ", query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions.tblBillingInfo.cardHolderName).Distinct());
                                    var transactions = query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions);
                                    foreach (var t in transactions)
                                    {
                                        if (t.billingInfoID != null)
                                        {
                                            holders += (holders == "" ? "" : ",") + t.tblBillingInfo.cardHolderName;
                                        }
                                    }
                                    holders = string.Join(", ", holders.Split(',').Distinct());
                                }
                            }
                            list.Add(new KeyValue() { Key = field, Value = holders });
                            break;
                        }
                    case "$CardNumber":
                        {
                            var numbers = "";
                            if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2).Count() > 0)
                            {
                                if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions).Count(m => m.billingInfoID != null) > 0)
                                {
                                    //numbers = string.Join(", ", query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => GeneralFunctions.MaskCreditCard(mexHash.mexHash.DecryptString(m.tblMoneyTransactions.tblBillingInfo.cardNumber)).Substring(12)).Distinct());
                                    var transactions = query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions);
                                    foreach (var t in transactions)
                                    {
                                        if (t.billingInfoID != null)
                                        {
                                            numbers += (numbers == "" ? "" : ",") + GeneralFunctions.MaskCreditCard(mexHash.mexHash.DecryptString(t.tblBillingInfo.cardNumber)).Substring(12);
                                        }
                                        else
                                        {
                                            numbers += (numbers == "" ? "" : ",") + t.tblPaymentDetails.FirstOrDefault().ccReferenceNumber;
                                        }
                                    }
                                    numbers = string.Join(", ", numbers.Split(',').Distinct());
                                }
                            }
                            list.Add(new KeyValue() { Key = field, Value = numbers });
                            break;
                        }
                    case "$CardType":
                        {
                            var types = "";
                            if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2).Count() > 0)
                            {
                                if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions).Count(m => m.billingInfoID != null) > 0)
                                {
                                    //types = string.Join(", ", query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions.tblBillingInfo.tblCardTypes.cardType).Distinct());
                                    var transactions = query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions);
                                    foreach (var t in transactions)
                                    {
                                        if (t.billingInfoID != null)
                                        {
                                            if (t.tblBillingInfo.cardTypeID != 0)
                                            {
                                                types += (types == "" ? "" : ",") + t.tblBillingInfo.tblCardTypes.cardType;
                                            }
                                            else
                                            {
                                                types += (types == "" ? "" : ",") + t.tblPaymentDetails.FirstOrDefault().ccType;
                                            }
                                        }
                                    }
                                    types = string.Join(", ", types.Split(',').Distinct());
                                }
                            }
                            list.Add(new KeyValue() { Key = field, Value = types });
                            break;
                        }
                    case "$ClientName":
                        {
                            var name = query.tblLeads.firstName + " " + query.tblLeads.lastName;
                            list.Add(new KeyValue() { Key = field, Value = name });
                            break;
                        }
                    case "$Company":
                        {
                            //var company = query.destinationID == 5 ? "The Villa Group" : "Tafer";
                            var company = query.tblLeads.tblTerminals.tblCompaniesGroups.companiesGroup;
                            list.Add(new KeyValue() { Key = field, Value = company });
                            break;
                        }
                    case "$FrontOfficeCertificateNumber":
                    case "$CRS":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.frontOfficeCertificateNumber ?? "" });
                            break;
                        }
                    case "$ConfirmationNumber":
                    case "$HotelConfirmation":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.hotelConfirmationNumber });
                            break;
                        }
                    case "$DepartureDate":
                        {
                            list.Add(new KeyValue() { Key = field, Value = (query.departureDate != null ? ((DateTime)query.departureDate).ToString("yyyy-MM-dd") : "") });
                            break;
                        }
                    case "$DeparturesFlightInfo":
                        {
                            var departures = "";

                            if (session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray().Contains(10))
                            {
                                var options = GetResortConnectOptionals(query);
                                if (query.tblFlights.Count(m => m.flightTypeID == 2) > 0)
                                {
                                    departures += "<table align=\"center\" style=\"font-family:Verdana;font-size:10pt;text-align:center;border-collapse:separate;border-spacing:15px;\"><thead>"
                                            + "<tr>"
                                            + "<th>Departure Date &amp; Time</th>"
                                            + "<th>Airline</th>"
                                            + "<th>Flight Number</th>"
                                                //+ "<th>Departure Time</th>"
                                                + "<th>Passengers</th>"
                                                + "<th>Passengers Names</th></tr></thead><tbody style=\"text-align: center;font-size:10pt;\">";
                                    foreach (var i in query.tblFlights.Where(m => m.flightTypeID == 2).OrderByDescending(m => m.flightDateTime))
                                    {
                                        departures += "<tr>"
                                            + "<td>" + i.flightDateTime.Date.ToString("MMM dd, yyyy")
                                            + "<br />" + (i.flightDateTime != null ? i.flightDateTime.TimeOfDay.ToString(@"HH\:mm") : "") + "</td>"
                                            + "<td>" + i.tblAirLines.airLine + "</td>"
                                            + "<td>" + i.flightNumber + "</td>"
                                            //+ "<td>" + (i.flightDateTime != null ? GeneralFunctions.DateFormat.ToMilitarHour(i.flightDateTime.TimeOfDay.ToString()) : "") + "</td>"
                                            + "<td>" + i.passengers.ToString() + "</td>"
                                            + "<td>" + i.passengersNames + "</td>"
                                            + "</tr>";
                                        if (i.pickupTime != null)
                                        {
                                            departures += "<tr><td colspan=\"6\" align=\"center\" style=\"text-align:center;\">Please be at the lobby at: <mark>" + (i.pickupTime != null ? GeneralFunctions.DateFormat.ToMilitarHour(i.pickupTime.ToString()) : "") + "</mark></td></tr>";
                                        }
                                        else
                                        {
                                            departures += "<tr><td colspan=\"6\" align=\"center\" style=\"text-align:center;\"></td></tr>";
                                        }
                                        //if (session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray().Contains(10))
                                        //{
                                        //    departures += "<tr><td colspan=\"6\" align=\"center\" style=\"text-align:center;\"></td></tr>";
                                        //}
                                        //else
                                        //{
                                        //    departures += "<tr><td colspan=\"6\" align=\"center\" style=\"text-align:center;\">Please be at the lobby at: <mark>" + (i.pickupTime != null ? GeneralFunctions.DateFormat.ToMilitarHour(i.pickupTime.ToString()) : "") + "</mark></td></tr>";
                                        //}

                                    }
                                    departures += "</tbody></table>";
                                }
                                else if (options.Count(m => m.ProductName.Contains("eparture")) > 0)
                                {
                                    departures += "<table align=\"center\" style=\"font-family:Verdana;font-size:10pt;text-align:center;border-collapse:separate;border-spacing:15px;\">"
                                    + "<thead><tr>";
                                    if (options.Count(m => m.ProductName.Contains("eparture") && m.CarrierName != null) > 0)
                                    {
                                        departures += "<th>Carrier</th><th>Flight Number</th><th>Notes</th>";
                                    }
                                    else
                                    {
                                        departures += "<th>Flight(s) Information</th>";
                                    }
                                    departures += "</tr></thead><tbody style=\"text-align: center;font-size:10pt;\">";
                                    foreach (var i in options.Where(m => m.ProductName.Contains("eparture")))
                                    {
                                        departures += "<tr>";
                                        if (i.CarrierName != null)
                                        {
                                            departures += "<td>" + i.CarrierName + "</td><td>" + i.FlightNumber + "</td><td>" + i.Note + "</td>";
                                        }
                                        else
                                        {
                                            departures += "<td>" + i.Note + "</td>";
                                        }

                                        departures += "</tr>";
                                    }
                                    departures += "</tbody></table>";
                                }
                            }
                            else
                            {
                                if (query.tblFlights.Count(m => m.flightTypeID == 2) > 0)
                                {
                                    departures += "<table align=\"center\" style=\"font-family:Verdana;font-size:10pt;text-align:center;border-collapse:separate;border-spacing:15px;\"><thead>"
                                                + "<tr><th>Departure Date &amp; Time</th>"
                                                + "<th>Airline</th>"
                                                + "<th>Flight Number</th>"
                                                    //+ "<th>Departure Time</th>"
                                                    + "<th>Destination</th>"
                                                    + "<th>Passengers</th>"
                                                    + "<th>Passengers Names</th></tr></thead><tbody style=\"text-align: center;font-size:10pt;\">";
                                    foreach (var i in query.tblFlights.Where(m => m.flightTypeID == 2).OrderByDescending(m => m.flightDateTime))
                                    {
                                        departures += "<tr>"
                                            + "<td>" + i.flightDateTime.Date.ToString("MMM dd, yyyy") + "<br />" + (i.flightDateTime != null ? i.flightDateTime.TimeOfDay.ToString(@"hh\:mm") : "") + "</td>"
                                            + "<td>" + i.tblAirLines.airLine + "</td>"
                                            + "<td>" + i.flightNumber + "</td>"
                                            //+ "<td>" + (i.flightDateTime != null ? GeneralFunctions.DateFormat.ToMilitarHour(i.flightDateTime.TimeOfDay.ToString()) : "") + "</td>"
                                            + "<td>" + query.tblPlaces.tblDestinations.destination + " International Airport" + "</td>"
                                            + "<td>" + i.passengers.ToString() + "</td>"
                                            + "<td>" + i.passengersNames + "</td>"
                                            + "</tr>";

                                        if (i.pickupTime != null)
                                        {
                                            departures += "<tr><td colspan=\"6\" align=\"center\" style=\"text-align:center;\">Please be at the lobby at: <mark>" + (i.pickupTime != null ? GeneralFunctions.DateFormat.ToMilitarHour(i.pickupTime.ToString()) : "") + "</mark></td></tr>";
                                        }
                                        else
                                        {
                                            departures += "<tr><td colspan=\"6\" align=\"center\" style=\"text-align:center;\"></td></tr>";
                                        }
                                    }
                                    departures += "</tbody></table>";
                                }
                            }
                            list.Add(new KeyValue() { Key = field, Value = departures });
                            break;
                        }
                    case "$Destination":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.tblDestinations.destination });
                            break;
                        }
                    case "$Extension":
                    case "$SentByPhoneExt":
                    case "$PhoneExt":
                        {
                            var ext = "";
                            if (context != null && context.User.Identity.IsAuthenticated)
                            {
                                ext = session.Extension;
                            }
                            else
                            {
                                var user = db.aspnet_Users.FirstOrDefault(m => m.UserId == query.tblLeads.assignedToUserID);
                                var profile = user != null ? user.tblUserProfiles.FirstOrDefault() : null;
                                ext = profile.phoneEXT;
                            }
                            list.Add(new KeyValue() { Key = field, Value = ext });
                            break;
                        }
                    case "$DepartmentPhone":
                        {
                            var phone = "";
                            if (context != null && context.User.Identity.IsAuthenticated)
                            {
                                phone = session.Phone;
                            }
                            else
                            {
                                var user = db.aspnet_Users.FirstOrDefault(m => m.UserId == query.tblLeads.assignedToUserID);
                                var profile = user != null ? user.tblUserProfiles.FirstOrDefault() : null;
                                phone = profile.departamentPhone ?? "";
                            }
                            list.Add(new KeyValue() { Key = field, Value = phone });
                            break;
                        }
                    case "$FirstName":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.tblLeads.firstName });
                            break;
                        }
                    case "$FlightNumber":
                        {
                            var number = query.tblFlights.Count(m => m.flightTypeID == 1) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 1).OrderByDescending(m => m.flightDateTime).FirstOrDefault().flightNumber : "";
                            list.Add(new KeyValue() { Key = field, Value = number });
                            break;
                        }
                    case "$HeaderCardHolder":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.tblPaymentDetails.Count(m => m.deleted != true && m.paymentType == 2) > 0 ? "Card Holder" : "" });
                            break;
                        }
                    case "$HeaderCardNumber":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.tblPaymentDetails.Count(m => m.deleted != true && m.paymentType == 2) > 0 ? "Card Number" : "" });
                            break;
                        }
                    case "$HeaderCardType":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.tblPaymentDetails.Count(m => m.deleted != true && m.paymentType == 2) > 0 ? "Card Type" : "" });
                            break;
                        }
                    case "$HeaderInvoice":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.tblPaymentDetails.Count(m => m.deleted != true && m.paymentType == 2) > 0 ? "Invoice" : "" });
                            break;
                        }
                    case "$Invoice":
                        {
                            var invoice = "";
                            if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2).Count() > 0)
                            {
                                invoice = string.Join(", ", query.tblPaymentDetails.Where(m => m.deleted != true && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions.authCode).Distinct());
                            }
                            list.Add(new KeyValue() { Key = field, Value = invoice });
                            break;
                        }
                    case "$ItemID":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.reservationID.ToString() });
                            break;
                        }
                    case "$LastName":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.tblLeads.lastName });
                            break;
                        }
                    case "$MerchantAccount":
                        {
                            var accounts = "";
                            if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2).Count() > 0)
                            {
                                if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions).Count(m => m.billingInfoID != null) > 0)
                                {
                                    if (query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions.tblMerchantAccounts).Count() > 0)
                                    {
                                        var acc = query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions.tblMerchantAccounts);
                                        var accc = "";
                                        foreach (var t in acc)
                                        {
                                            accc += (accc == "" ? "" : ",") + (t == null ? "" : t.merchantAccountBillingName);
                                        }

                                        accounts = "Please be aware that your card has been charged under the merchant account name(s) <strong>";
                                        //accounts += string.Join(", ", query.tblPaymentDetails.Where(m => m.deleted != true && m.paymentType == 2 && m.tblMoneyTransactions.authCode != null && m.tblMoneyTransactions.authCode != "").Select(m => m.tblMoneyTransactions.tblMerchantAccounts.merchantAccountBillingName).Distinct());
                                        accounts += string.Join(", ", accc.Split(',').Distinct());
                                        accounts += "</strong>";
                                    }
                                }
                            }
                            list.Add(new KeyValue() { Key = field, Value = accounts });
                            break;
                        }
                    case "$OptionsSold":
                        {
                            var _str = "";
                            if (session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray().Contains(10))
                            {
                                var options = GetResortConnectOptionals(query);
                                var _options = query.tblOptionsSold.Where(m => m.deleted != true).Select(m => new { option = m.tblOptions.optionName, date = m.optionDateTime, m.quantity, cost = m.optionPrice, m.tblOptionTypes.optionType });
                                _str = "<table style=\"width:100%;margin-left:auto;margin-right:auto;font-family:verdana;text-align:center;font-size:10pt;border-collapse:separate;border-spacing:15px;\" align=\"center\">"
                                + "<thead><tr><th>Qty</th><th>Option</th><th>Date</th><th>Total</th></tr></thead><tbody style=\"text-align:center;font-size:10pt;\">";
                                if (options.Count() > 0)
                                {
                                    foreach (var i in options)
                                    {
                                        _str += "<tr><td>" + i.Quantity + "</td><td>" + i.ProductName + "</td><td>" + (i.Date != null ? i.Date.Value.ToString("MMM dd, yyyy hh:mm tt") : "") + "</td><td>" + Decimal.Round((decimal.Parse(i.BaseCurrencyAmount) * (decimal)i.Quantity), 2).ToString() + "</td></tr>";
                                    }
                                }
                                if (_options.Count() > 0)
                                {
                                    foreach (var i in _options)
                                    {
                                        var disclaimer = i.optionType.ToLower().IndexOf("spa") != -1 ? "Certificates cannot be used in conjunction with the gold card discount or other spa promotions." : "";
                                        _str += "<tr><td>" + i.quantity + "</td><td>" + i.option + (disclaimer != "" ? "<br /><span style=\"font-size:8pt;\">Disclaimer: " + disclaimer + "</span>" : "") + "</td><td>" + (i.date != null ? i.date.Value.ToString("MMM dd, yyyy hh:mm tt") : "") + "</td><td>" + Decimal.Round((decimal.Parse(i.cost) * i.quantity), 2) + "</td></tr>";
                                    }
                                }
                                _str += "</tbody></table>";


                                //if (options.Count() > 0)
                                //{
                                //    _str = "<table style=\"width:100%;margin-left:auto;margin-right:auto;font-family:verdana;text-align:center;font-size:10pt;border-collapse:separate;border-spacing:15px;\" align=\"center\">"
                                //+ "<thead><tr><th>Qty</th><th>Option</th><th>Date</th><th>Total</th></tr></thead><tbody style=\"text-align:center;font-size:10pt;\">";
                                //    foreach (var i in options)
                                //    {
                                //        _str += "<tr><td>" + i.Quantity + "</td><td>" + i.ProductName + "</td><td>" + (i.Date != null ? i.Date.Value.ToString("MMM dd, yyyy hh:mm tt") : "") + "</td><td>" + Decimal.Round((decimal.Parse(i.BaseCurrencyAmount) * (decimal)i.Quantity), 2).ToString() + "</td></tr>";
                                //    }
                                //    _str += "</tbody></table>";
                                //}
                                //else
                                //{
                                //    var _options = query.tblOptionsSold.Where(m => m.deleted != true).Select(m => new { option = m.tblOptions.optionName, date = m.optionDateTime, m.quantity, cost = m.optionPrice, m.tblOptionTypes.optionType });
                                //    var str = new JavaScriptSerializer().Serialize(options);

                                //    if (_options.Count() > 0)
                                //    {
                                //        _str = "<table style=\"width:100%;margin-left:auto;margin-right:auto;font-family:verdana;text-align:center;font-size:10pt;border-collapse:separate;border-spacing:15px;\" align=\"center\">"
                                //    + "<thead><tr><th>Qty</th><th>Option</th><th>Date</th><th>Total</th></tr></thead><tbody style=\"text-align:center;font-size:10pt;\">";
                                //        foreach (var i in _options)
                                //        {
                                //            var disclaimer = i.optionType.ToLower().IndexOf("spa") != -1 ? "Certificates cannot be used in conjunction with the gold card discount or other spa promotions." : "";
                                //            _str += "<tr><td>" + i.quantity + "</td><td>" + i.option + (disclaimer != "" ? "<br /><span style=\"font-size:8pt;\">Disclaimer: " + disclaimer + "</span>" : "") + "</td><td>" + (i.date != null ? i.date.Value.ToString("MMM dd, yyyy hh:mm tt") : "") + "</td><td>" + Decimal.Round((decimal.Parse(i.cost) * i.quantity), 2) + "</td></tr>";
                                //        }
                                //        _str += "</tbody></table>";
                                //    }
                                //}
                            }
                            else
                            {
                                var options = query.tblOptionsSold.Where(m => m.deleted != true).Select(m => new { option = m.tblOptions.optionName, date = m.optionDateTime, m.quantity, cost = m.optionPrice, m.tblOptionTypes.optionType });
                                var str = new JavaScriptSerializer().Serialize(options);

                                if (options.Count() > 0)
                                {
                                    //_str = "<table style=\"font-family:verdana;text-align:center;font-size:10pt;border-collapse:separate;border-spacing:15px;\" align=\"center\">"
                                    _str = "<table style=\"width:100%;margin-left:auto;margin-right:auto;font-family:verdana;text-align:center;font-size:10pt;border-collapse:separate;border-spacing:15px;\" align=\"center\">"
                                + "<thead><tr><th>Qty</th><th>Option</th><th>Date</th><th>Total</th></tr></thead><tbody style=\"text-align:center;font-size:10pt;\">";
                                    foreach (var i in options)
                                    {
                                        var disclaimer = i.optionType.ToLower().IndexOf("spa") != -1 ? "Certificates cannot be used in conjunction with the gold card discount or other spa promotions." :
                                        i.option.ToLower().IndexOf("beverage") != -1 && i.optionType.ToLower().IndexOf("request") != -1 ? "Resort credit cannot be exchanged for cash nor be combined with any other promotions (happy hour, Gold and VIP cards). Palmita Market Not Included. Credit applied at Check Out." : "";
                                        _str += "<tr><td>" + i.quantity + "</td><td>" + i.option + (disclaimer != "" ? "<br /><span style=\"font-size:7pt;\">Disclaimer: " + disclaimer + "</span>" : "") + "</td><td>" + (i.date != null ? i.date.Value.ToString("MMM dd, yyyy hh:mm tt") : "") + "</td><td>" + Decimal.Round((decimal.Parse(i.cost) * i.quantity), 2) + "</td></tr>";
                                    }
                                    _str += "</tbody></table>";
                                }
                            }
                            list.Add(new KeyValue() { Key = field, Value = _str });
                            break;
                        }
                    case "$PassengerNames":
                        {
                            var names = query.tblFlights.Count(m => m.flightTypeID == 1) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 1).OrderByDescending(m => m.flightDateTime).FirstOrDefault().passengersNames : "";
                            list.Add(new KeyValue() { Key = field, Value = names });
                            break;
                        }
                    case "$Passengers":
                        {
                            var passengers = query.tblFlights.Count(m => m.flightTypeID == 1) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 1).OrderByDescending(m => m.flightDateTime).FirstOrDefault().passengers.ToString() : "";
                            list.Add(new KeyValue() { Key = field, Value = passengers });
                            break;
                        }
                    case "$PaymentDate":
                        {
                            var dates = string.Join(", ", query.tblPaymentDetails.Where(m => m.deleted != true && m.tblMoneyTransactions.authCode != null && (m.tblMoneyTransactions.authCode != "" || m.paymentType != 2)).Select(m => m.dateSaved.ToString("yyyy-MM-dd")).Distinct());
                            list.Add(new KeyValue() { Key = field, Value = dates });
                            break;
                        }
                    case "$PaymentType":
                        {
                            var types = "";

                            if (query.tblPaymentDetails.Count(m => m.deleted != true) > 0)
                            {
                                var ids = query.tblPaymentDetails.Where(m => m.deleted != true).Select(m => Convert.ToString(m.paymentType));
                                types = string.Join(", ", GeneralFunctions.PaymentTypes.Where(m => ids.Contains(m.Key)).Select(m => m.Value));
                            }
                            list.Add(new KeyValue() { Key = field, Value = types });
                            break;
                        }
                    case "$PickUpTime":
                        {
                            var time = query.tblFlights.Count(m => m.flightTypeID == 2) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 2).OrderByDescending(m => m.flightDateTime).FirstOrDefault().pickupTime : (TimeSpan?)null;
                            var _time = time != null ? GeneralFunctions.DateFormat.ToMeridianHour(time.Value.ToString()) : "";
                            list.Add(new KeyValue() { Key = field, Value = _time });
                            break;
                        }
                    case "$PresentationDateTime":
                        {
                            var pst = query.tblPresentations.Count() > 0 ? ((DateTime)query.tblPresentations.OrderByDescending(m => m.dateSaved).FirstOrDefault().datePresentation).ToString("yyyy-MM-dd") + " " + ((TimeSpan)query.tblPresentations.OrderByDescending(m => m.dateSaved).FirstOrDefault().timePresentation).ToString("c") : "";
                            list.Add(new KeyValue() { Key = field, Value = pst });
                            break;
                        }
                    case "$Resort":
                        {
                            list.Add(new KeyValue() { Key = field, Value = query.tblPlaces.place });
                            break;
                        }
                    case "$RoundAirline":
                        {
                            var airline = query.tblFlights.Count(m => m.flightTypeID == 2) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 2).OrderByDescending(m => m.flightDateTime).FirstOrDefault().tblAirLines.airLine : "";
                            list.Add(new KeyValue() { Key = field, Value = airline });
                            break;
                        }
                    case "$RoundFlightNumber":
                        {
                            var number = query.tblFlights.Count(m => m.flightTypeID == 2) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 2).OrderByDescending(m => m.flightDateTime).FirstOrDefault().flightNumber : "";
                            list.Add(new KeyValue() { Key = field, Value = number });
                            break;
                        }
                    case "$RoundFlightTime":
                        {
                            var _time = query.tblFlights.Count(m => m.flightTypeID == 2) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 2).OrderByDescending(m => m.flightDateTime).FirstOrDefault().flightDateTime.TimeOfDay : (TimeSpan?)null;
                            var time = _time != null ? GeneralFunctions.DateFormat.ToMeridianHour(_time.ToString()) : "";
                            list.Add(new KeyValue() { Key = field, Value = time });
                            break;
                        }
                    case "$RoundPassengerNames":
                        {
                            var names = query.tblFlights.Count(m => m.flightTypeID == 2) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 2).OrderByDescending(m => m.flightDateTime).FirstOrDefault().passengersNames : "";
                            list.Add(new KeyValue() { Key = field, Value = names });
                            break;
                        }
                    case "$RoundPassengers":
                        {
                            var passengers = query.tblFlights.Count(m => m.flightTypeID == 2) > 0 ? query.tblFlights.Where(m => m.flightTypeID == 2).OrderByDescending(m => m.flightDateTime).FirstOrDefault().passengers.ToString() : "";
                            list.Add(new KeyValue() { Key = field, Value = passengers });
                            break;
                        }
                    case "$SentByEmail":
                        {
                            var str = "";
                            if (context != null && context.User.Identity.IsAuthenticated)
                            {
                                str = session.Email;
                            }
                            else
                            {
                                var user = db.aspnet_Users.FirstOrDefault(m => m.UserId == query.tblLeads.assignedToUserID);
                                str = user.aspnet_Membership.Email;
                            }
                            list.Add(new KeyValue() { Key = field, Value = str });
                            break;
                        }
                    case "$SentByUser":
                        {
                            var str = "";
                            if (context != null && context.User.Identity.IsAuthenticated)
                            {
                                str = session.User;
                            }
                            else
                            {
                                var user = db.aspnet_Users.FirstOrDefault(m => m.UserId == query.tblLeads.assignedToUserID);
                                var profile = user != null ? user.tblUserProfiles.FirstOrDefault() : null;
                                str = profile.firstName + " " + profile.lastName;
                            }
                            list.Add(new KeyValue() { Key = field, Value = str });
                            break;
                        }
                    case "$TransactionID":
                        {
                            var value = "$TransactionID";
                            list.Add(new KeyValue() { Key = field, Value = value });
                            break;
                        }
                    case "$SignatureLink":
                        {
                            //var link = "<a href=\"#\">Please click on this link in order to e-sign this document. Your signature is required to finalize the process.</a>";
                            var link = "$SignatureLink";
                            list.Add(new KeyValue() { Key = field, Value = link });
                            break;
                        }
                    case "$UserFirstName":
                        {
                            var str = "";
                            if (context != null && context.User.Identity.IsAuthenticated)
                            {
                                str = session.User.Split(' ')[0];
                            }
                            else
                            {
                                var user = db.aspnet_Users.FirstOrDefault(m => m.UserId == query.tblLeads.assignedToUserID);
                                var profile = user != null ? user.tblUserProfiles.FirstOrDefault() : null;
                                str = profile.firstName;
                            }
                            list.Add(new KeyValue() { Key = field, Value = str });
                            break;
                        }
                    case "$UserLastName":
                        {
                            var str = "";
                            if (context != null && context.User.Identity.IsAuthenticated)
                            {
                                var name = session.User.Split(' ');
                                str = name.Length > 2 ? name[2] : name[1];
                                //str = session.User.Split(' ')[1];
                            }
                            else
                            {
                                var user = db.aspnet_Users.FirstOrDefault(m => m.UserId == query.tblLeads.assignedToUserID);
                                var profile = user != null ? user.tblUserProfiles.FirstOrDefault() : null;
                                str = profile.lastName;
                            }
                            list.Add(new KeyValue() { Key = field, Value = str });
                            break;
                        }
                }
            }
            #endregion
        }

        public static void ReplaceReservedWords(string context, object query, Type type, ref List<KeyValue> list, DateTime now)
        {
            //var _type = Type.GetType("ePlatBack.Models." + type);
            //var query = Convert.ChangeType(_query, type);

            //dynamic query = Convert.ChangeType(_query, type);
            foreach (var i in list)
            {
                switch (i.Key)
                {
                    case "$GuestName":
                        {
                            if (context == "ePlat")
                            {
                                var q = ((tblReservations)query);
                                i.Value = q.tblLeads.firstName + " " + q.tblLeads.lastName;
                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaProspectos.firstName + " " + q.tbaProspectos.lastName;
                            }
                            break;
                        }
                    case "$FirstName":
                        {
                            if (context == "ePlat")
                            {
                                if (type.FullName.IndexOf("tblReservations") != -1)
                                {
                                    var q = ((tblReservations)query);
                                    i.Value = q.tblLeads.firstName;
                                }
                                else if (type.FullName.IndexOf("tblPurchases") != -1)
                                {
                                    var q = ((tblPurchases)query);
                                    i.Value = q.tblLeads.firstName;
                                }
                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaProspectos.firstName;
                            }
                            break;
                        }
                    case "$LastName":
                        {
                            if (context == "ePlat")
                            {
                                if (type.FullName.IndexOf("tblReservations") != -1)
                                {
                                    var q = ((tblReservations)query);
                                    i.Value = q.tblLeads.lastName;
                                }
                                else if (type.FullName.IndexOf("tblPurchases") != -1)
                                {
                                    var q = ((tblPurchases)query);
                                    i.Value = q.tblLeads.lastName;
                                }
                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaProspectos.lastName;
                            }
                            break;
                        }
                    case "$Stay":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = (q.llegada != null ? ((DateTime)q.llegada).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "") + " / " + (q.salida != null ? ((DateTime)q.salida).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "");
                            }
                            break;
                        }
                    case "$City":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaProspectos.city;
                            }
                            break;
                        }
                    case "$ReservationID":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.idReservacion.ToString();
                            }
                            break;
                        }
                    case "$DestinationID":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.idDestino.ToString();
                            }
                            break;
                        }
                    case "$Destination":
                        {
                            if (context == "ePlat")
                            {
                                var q = ((tblReservations)query);
                                i.Value = q.tblDestinations.destination;
                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaDestinos.destino;
                            }
                            break;
                        }
                    case "$Resort":
                        {
                            if (context == "ePlat")
                            {
                                var q = ((tblReservations)query);
                                i.Value = q.tblPlaces.place;
                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaLugares.lugar;
                            }
                            break;
                        }
                    case "$CheckIn":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.llegada != null ? ((DateTime)q.llegada).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                            }
                            break;
                        }
                    case "$CheckOut":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.salida != null ? ((DateTime)q.salida).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                            }
                            break;
                        }
                    case "$LeadID":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaProspectos.idProspecto.ToString();
                            }
                            break;
                        }
                    case "$MarketingSourceID":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaProspectos.idMarketingSource.ToString();
                            }
                            break;
                        }
                    case "$DB":
                        {
                            //i.Value = "ecommerce";
                            i.Value = context;
                            break;
                        }
                    case "$Sent":
                        {
                            i.Value = now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);//24 hrs with fractional seconds
                            break;
                        }
                    case "$MarketingSource":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaProspectos.tbaMarketingSources.marketingSource;
                            }
                            break;
                        }
                    case "$VueloLlegada":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaDatosProspectos.Count() > 0 ? ((DateTime)q.tbaDatosProspectos.FirstOrDefault(m => m.idReservacion == q.idReservacion).vueloLlegada).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                            }
                            break;
                        }
                    case "$Travelers":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaProspectos.firstName + " " + q.tbaProspectos.lastName + " / " + q.tbaProspectos.spouseFirstName + " " + q.tbaProspectos.spouseLastName;
                            }
                            break;
                        }
                    case "$ResortConfirmation":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.certificateNumber;
                            }
                            break;
                        }
                    case "$StayingAt":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaLugares.lugar;
                            }
                            break;
                        }
                    case "$Airline":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaDatosProspectos.Count() > 0 ? q.tbaDatosProspectos.FirstOrDefault(m => m.idReservacion == q.idReservacion).vueloAerolinea : "";
                            }
                            break;
                        }
                    case "$FlightNumber":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = (tbaReservaciones)query;
                                i.Value = q.tbaDatosProspectos.Count() > 0 ? q.tbaDatosProspectos.FirstOrDefault(m => m.idReservacion == q.idReservacion).vueloNumero : "";
                            }
                            break;
                        }
                    case "$MeetingTime":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = (tbaReservaciones)query;
                                i.Value = q.tbaDatosProspectos.Count() > 0 ? q.tbaDatosProspectos.FirstOrDefault(m => m.idReservacion == q.idReservacion).vueloHoraLlegada : "";
                            }
                            break;
                        }
                    case "$PartyOf":
                        {
                            if (context == "ePlat")
                            {

                            }
                            else
                            {
                                var q = (tbaReservaciones)query;
                                i.Value = q.tbaDatosProspectos.Count() > 0 ? q.tbaDatosProspectos.FirstOrDefault(m => m.idReservacion == q.idReservacion).vueloNPersonas : "";
                            }
                            break;
                        }
                    ///service
                    case "$PurchaseID":
                        {
                            var q = ((tblPurchases)query);
                            i.Value = q.purchaseID.ToString();
                            break;
                        }
                    case "$Terminal":
                        {
                            if (context == "ePlat")
                            {
                                if (type.FullName.IndexOf("tblReservations") != -1)
                                {
                                    var q = ((tblReservations)query);
                                    i.Value = q.tblLeads.tblTerminals.terminal;
                                }
                                else if (type.FullName.IndexOf("tblPurchases") != -1)
                                {
                                    var q = ((tblPurchases)query);
                                    i.Value = q.tblLeads.tblTerminals.terminal;
                                }
                            }
                            else
                            {
                                var q = ((tbaReservaciones)query);
                                i.Value = q.tbaProspectos.tbaTerminales.terminal;
                            }
                            break;
                        }
                    //prearrival tafer
                    case "UserFirstName":
                        {
                            if (context == "ePlat")
                            {
                                var q = ((tblReservations)query);
                                Guid userID = (Guid)q.tblLeads.assignedToUserID;
                                i.Value = new ePlatEntities().tblUserProfiles.FirstOrDefault(m => m.userID == userID).firstName;
                            }
                            break;
                        }
                    case "$Extension":
                        {
                            if (context == "ePlat")
                            {
                                var q = ((tblReservations)query);
                                Guid userID = (Guid)q.tblLeads.assignedToUserID;
                                i.Value = new ePlatEntities().tblUserProfiles.FirstOrDefault(m => m.userID == userID).phoneEXT;
                            }
                            break;
                        }
                    case "$DepartmentPhone":
                        {
                            if (context == "ePlat")
                            {
                                var q = ((tblReservations)query);
                                Guid userID = (Guid)q.tblLeads.assignedToUserID;
                                var profile = new ePlatEntities().tblUserProfiles.FirstOrDefault(m => m.userID == userID);
                                i.Value = profile.departamentPhone;
                            }
                            break;
                        }
                    case "$AssignedToUser":
                        {
                            if (context == "ePlat")
                            {
                                var q = ((tblReservations)query);
                                Guid userID = (Guid)q.tblLeads.assignedToUserID;
                                var profile = new ePlatEntities().tblUserProfiles.FirstOrDefault(m => m.userID == userID);
                                i.Value = profile.firstName + " " + profile.lastName;
                            }
                            break;
                        }
                    case "$Company":
                        {
                            if (context == "ePlat")
                            {
                                var q = ((tblReservations)query);
                                i.Value = q.destinationID == 5 ? "The Villa Group" : "Tafer";
                            }
                            else
                            {

                            }
                            break;
                        }
                    case "$DateSent":
                        {
                            i.Value = now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            break;
                        }
                }
            }
        }

        public static string ValidateRequiredFields(List<string> requiredFields, tblReservations query)
        {
            var missingFields = "";
            if (requiredFields != null)
            {
                foreach (var field in requiredFields)
                {
                    #region "required fields"
                    switch (field)
                    {
                        case "First Name":
                            {
                                if (query.tblLeads.firstName == null)
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                        case "Last Name":
                            {
                                if (query.tblLeads.lastName == null)
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                        case "Email":
                            {
                                if (query.tblLeads.tblLeadEmails == null)
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                        case "Hotel Confirmation Number":
                            {
                                if (query.hotelConfirmationNumber == null)
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                        case "Arrival Date":
                            {
                                if (query.arrivalDate == null)
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                        case "Destination":
                            {
                                if (query.destinationID == null)
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                        case "Resort":
                            {
                                if (query.placeID == null)
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                        case "Presentation Date":
                            {
                                if (query.tblPresentations != null)
                                {
                                    if (query.tblPresentations.FirstOrDefault().datePresentation == null)
                                    {
                                        missingFields += (missingFields != "" ? ", " : "") + field;
                                    }
                                }
                                else
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                        case "Presentation Time":
                            {
                                if (query.tblPresentations != null)
                                {
                                    if (query.tblPresentations.FirstOrDefault().timePresentation == null)
                                    {
                                        missingFields += (missingFields != "" ? ", " : "") + field;
                                    }
                                }
                                else
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                        case "Optionals":
                            {
                                if (query.tblOptionsSold.Count() == 0 || query.tblOptionsSold.Count() == query.tblOptionsSold.Count(m => m.deleted == true))
                                {
                                    missingFields += (missingFields != "" ? ", " : "") + field;
                                }
                                break;
                            }
                    }
                    #endregion
                }
            }
            return missingFields;
        }

        public void InvitationSaveData(Guid trackingID, int sysEventID, string url)
        {
            try
            {
                if (trackingID != Guid.Empty)
                {
                    ePlatEntities db = new ePlatEntities();
                    tblEmailNotificationEvents log = new tblEmailNotificationEvents();
                    var logN = db.tblEmailNotificationLogs.Single(x => x.trackingID == trackingID);
                    log.emailNotificationLogID = logN.emailNotificationLogID;
                    log.sysEventID = sysEventID;
                    log.dateEvent = DateTime.Now;
                    log.url = url;
                    db.tblEmailNotificationEvents.AddObject(log);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                email.From = new System.Net.Mail.MailAddress("rsalinas@villagroup.com");
                email.Subject = "Invitation event error " + DateTime.Now.Date;
                email.Body = "Data= trackingID=" + trackingID.ToString() + " sysEventID= " + sysEventID.ToString() + " url=" + url;
                email.To.Add("rsalinas@villagroup.com");
                //EmailNotifications.Send(email);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
            }
        }
        public static dynamic InvitationSaveData2(Guid trackingID, int sysEventID, string url)
        {
            try
            {
                if (trackingID != Guid.Empty)
                {
                    ePlatEntities db = new ePlatEntities();
                    tblEmailNotificationEvents log = new tblEmailNotificationEvents();
                    var logN = db.tblEmailNotificationLogs.Single(x => x.trackingID == trackingID);
                    log.emailNotificationLogID = logN.emailNotificationLogID;
                    log.sysEventID = sysEventID;
                    log.dateEvent = DateTime.Now;
                    log.url = url;
                    db.tblEmailNotificationEvents.AddObject(log);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                email.From = new System.Net.Mail.MailAddress("rsalinas@villagroup.com");
                email.Subject = "Invitation event error " + DateTime.Now.Date;
                email.Body = "Data= trackingID=" + trackingID.ToString() + " sysEventID= " + sysEventID.ToString() + " url=" + url;
                email.To.Add("rsalinas@villagroup.com");
                //EmailNotifications.Send(email);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
            }

            return 1;
        }

        public static AttemptResponse FrontOfficeSync(int resortID, string range)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            DateTime now = DateTime.Now;
            var date = DateTime.Today.Date;
            var dateTime = date.AddHours(now.Hour).AddMinutes(now.Minute);//now without seconds nor miliseconds
            var query = db.tblFrontOfficeSync.FirstOrDefault(m => m.frontOfficeResortID == resortID);
            var properties = query.GetType().GetProperties().Where(m => m.Name.IndexOf("lastSync" + range) != -1);

            //var p = properties.FirstOrDefault();//for testing
            foreach (var p in properties)
            {
                DateTime from = new DateTime();
                DateTime to = new DateTime();
                try
                {
                    var colName = p.Name;//lastSync1to7
                    string[] strings = Regex.Split(colName, @"\D+");//1,7
                    List<int> numbersList = new List<int>();
                    foreach (var s in strings)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            numbersList.Add(int.Parse(s));
                        }
                    }
                    int[] numbers = numbersList.ToArray();
                    var lastSyncValue = p.GetValue(query, null);
                    var intervalName = "interval" + numbers[0] + "to" + numbers[1];
                    var intervalProperty = query.GetType().GetProperty(intervalName);
                    var intervalValue = intervalProperty.GetValue(query, null);

                    var lastSyncParsed = lastSyncValue != null ? DateTime.Parse(lastSyncValue.ToString()) : dateTime;
                    var intervalParsed = intervalValue != null ? int.Parse(intervalValue.ToString()) : 0;
                    if (lastSyncParsed.AddMinutes(intervalParsed) <= dateTime)
                    {
                        var _from = numbers[0];
                        var _to = numbers[1] + 1;
                        from = date.AddDays(_from);
                        to = date.AddDays(_to).AddSeconds(-1);
                        p.SetValue(query, dateTime, null);

                        var listArrivals = GetReservationsToSync(resortID, from, to);
                        //UpdateReservationsFromEplat(resortID, from, to, listArrivals);
                        //var list = SyncReservations(listArrivals);

                        response.Message += "<br />Interval: " + colName + ". Records Updated: " + listArrivals.Count() + "/" + listArrivals.Count(m => m.Status == true);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    var email = new System.Net.Mail.MailMessage();
                    email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                    email.To.Add("efalcon@villagroup.com");
                    email.Subject = "SyncReservations Error";
                    email.IsBodyHtml = true;
                    email.Body = "El método SyncReservations no se completó.<br>ResortID: " + resortID + "<br>From: " + from.ToString("yyyy-MM-dd") + "<br>To: " + to.ToString("yyyy-MM-dd") + "<br>Error: " + e.InnerException;
                    EmailNotifications.SendSync(email);
                }
            }
            return response;
        }

        public static void SyncArrivals(List<FrontOfficeViewModel.LlegadasResult> results, int resortID, DateTime fromDate, DateTime toDate)
        {
            ePlatEntities db = new ePlatEntities();
            toDate = toDate.AddDays(1);

            //obtener terminal para la importación
            var TerminalID = (from t in db.tblPlaces_Terminals
                              join p in db.tblPlaces.DefaultIfEmpty()
                              on t.placeID equals p.placeID
                              join tml in db.tblTerminals.DefaultIfEmpty()
                              on t.terminalID equals tml.terminalID
                              where p.frontOfficeResortID == resortID
                              && tml.terminalTypeID == 4
                              select t.terminalID).FirstOrDefault();

            if (TerminalID != null && TerminalID != 0)
            {
                //obtener Market Codes
                var MarketCodes = (from mc in db.tblMarketCodes
                                   where mc.frontOfficeResortID == resortID
                                   select new
                                   {
                                       mc.marketCode,
                                       mc.arrivalsTravelSourceID,
                                       mc.programID
                                   }).ToList();

                //obtener IDs de Front Office de reservaciones para buscar los registros que ya existan
                var FrontOfficeReservationIDs = results.Select(x => x.idReservacion).ToList();

                //obtener los registros de PA
                //-obtener las terminales de PA
                var leadStatusExcluded = new int?[] { 10, 4 };//duplicate, canceled
                //var PrearrivalTerminals = (from t in db.tblTerminals
                //                           where t.terminalTypeID == 7
                //                           select t.terminalID).ToList();

                //var PAReservations = (from r in db.tblReservations
                //                      join lead in db.tblLeads
                //                         on r.leadID equals lead.leadID
                //                      join place in db.tblPlaces
                //                         on r.placeID equals place.placeID
                //                      where r.arrivalDate >= fromDate
                //                     && r.arrivalDate < toDate
                //                     && PrearrivalTerminals.Contains(lead.initialTerminalID)
                //                     && place.frontOfficeResortID == resortID
                //                     && (lead.bookingStatusID == 16 ||
                //                     lead.bookingStatusID == 1 ||
                //                     lead.secondaryBookingStatusID == 1 ||
                //                     lead.secondaryBookingStatusID == 32)
                //                     && !leadStatusExcluded.Contains(lead.leadStatusID)
                //                      orderby lead.firstName, lead.lastName
                //                      select new
                //                      {
                //                          r.leadID,
                //                          r.arrivalDate,
                //                          r.hotelConfirmationNumber,
                //                          lead.bookingStatusID,
                //                          lead.secondaryBookingStatusID,
                //                          lead.initialTerminalID
                //                      }).ToList();

                /*
                //obtener los registros de PA Rescom
                List<int?> LeadStatuses = new List<int?>() { 1, 2, 6, 13, 15 };
                var PARescom = (from r in db.tblReservations
                                join lead in db.tblLeads
                                    on r.leadID equals lead.leadID
                                join place in db.tblPlaces
                                    on r.placeID equals place.placeID
                                where r.arrivalDate >= fromDate
                                && r.arrivalDate < toDate
                                && lead.initialTerminalID == 10
                                && place.frontOfficeResortID == resortID
                                && LeadStatuses.Contains(lead.leadStatusID)
                                orderby lead.firstName, lead.lastName
                                select new
                                {
                                    r.leadID,
                                    r.arrivalDate,
                                    r.hotelConfirmationNumber,
                                    lead.bookingStatusID
                                }).ToList();

                //obtener los registros de PA Tafer
                var leadStatusExcluded = new int?[] { 10, 4 };//duplicate, canceled
                var PATafer = (from lead in db.tblLeads
                               join rsv in db.tblReservations
                                on lead.leadID equals rsv.leadID
                               join option in db.tblOptionsSold
                                on rsv.reservationID equals option.reservationID
                               join pst in db.tblPresentations
                                on rsv.reservationID equals pst.reservationID
                                into rsv_pst
                               from pst in rsv_pst.DefaultIfEmpty()
                               join place in db.tblPlaces.DefaultIfEmpty()
                                on rsv.placeID equals place.placeID
                               where lead.initialTerminalID == 16
                               && lead.isTest != true
                                 && !leadStatusExcluded.Contains(lead.leadStatusID)
                                 && rsv.reservationStatusID != 3
                               && fromDate <= rsv.arrivalDate && rsv.arrivalDate < toDate
                               && place.frontOfficeResortID == resortID
                               && option.deleted != true
                               select new
                               {
                                   lead.leadID,
                                   rsv.arrivalDate,
                                   rsv.hotelConfirmationNumber,
                                   lead.bookingStatusID
                               }).Distinct().ToList();
                */
                /*var x1 = (from x in results
                         where x.numconfirmacion.Contains("487540")
                         select x).FirstOrDefault();*/

                foreach (var reservation in results)
                {
                    bool exists = false;
                    var arrival = (from a in db.tblArrivals
                                   where a.frontOfficeReservationID == reservation.idReservacion
                                   select a).FirstOrDefault();
                    if (arrival == null)
                    {
                        arrival = new tblArrivals();
                        arrival.terminalID = TerminalID;
                        arrival.arrivalID = Guid.NewGuid();
                        arrival.dateSaved = DateTime.Now;
                        arrival.dateLastUpdate = DateTime.Now;
                        arrival.lastUpdateUserID = new Guid("c53613b6-c8b8-400d-95c6-274e6e60a14a");
                    }
                    else
                    {
                        exists = true;
                    }

                    arrival.split = reservation.Split;
                    arrival.frontOfficeReservationID = (long)reservation.idReservacion;
                    arrival.frontOfficeResortID = (int)reservation.idresort;
                    arrival.frontOfficeGuestID = (long)reservation.idhuesped;
                    arrival.arrivalDate = DateTime.Parse(reservation.llegada.Value.ToString("yyyy-MM-dd ") + reservation.HLlegada);
                    arrival.roomNumber = reservation.NumHab ?? "";
                    arrival.nights = (int)reservation.CuartosNoche;
                    arrival.guest = reservation.Huesped;
                    arrival.firstName = reservation.nombres;
                    arrival.lastName = reservation.apellidopaterno;
                    arrival.adults = (int)reservation.Adultos;
                    if (reservation.Ninos != null)
                    {
                        arrival.children = (int)reservation.Ninos;
                    }
                    else
                    {
                        arrival.children = 0;
                    }
                    if (reservation.Infantes != null)
                    {
                        arrival.infants = (int)reservation.Infantes;
                    }
                    else
                    {
                        arrival.infants = 0;
                    }
                    arrival.country = reservation.namepais;
                    arrival.countryCode = reservation.codepais;
                    if (arrival.nationality == null && reservation.codigostatusreservacion == "IN")
                    {
                        arrival.nationality = (reservation.codepais == "MX" ? "NAC" : "INT");
                    }
                    if (reservation.nameagencia != null)
                    {
                        arrival.agencyName = reservation.nameagencia;
                    }
                    if (reservation.codeagencia != null)
                    {
                        arrival.agencyCode = reservation.codeagencia;
                    }
                    arrival.source = reservation.Procedencia;
                    arrival.marketCode = reservation.CodigoMerc;
                    arrival.reservationStatus = reservation.codigostatusreservacion;
                    arrival.roomType = reservation.TipoHab;
                    arrival.confirmationNumber = reservation.numconfirmacion;
                    arrival.crs = reservation.CRS.Trim();
                    arrival.dateLastUpdate = DateTime.Now;
                    if (reservation.DistintivoPrecheckin != null && reservation.DistintivoPrecheckin == "PRECHECKIN")
                    {
                        arrival.preCheckIn = true;
                    }
                    if (reservation.FechaHoraCheckin != null)
                    {
                        arrival.checkinDateTime = reservation.FechaHoraCheckin;
                    }
                    if (reservation.Contrato != null && reservation.Contrato != "" && !reservation.Contrato.Contains("PENDIENTE"))
                    {
                        arrival.contract = reservation.Contrato.Trim();
                    }
                    else
                    {
                        arrival.contract = null;
                    }

                    if (reservation.TipoPlan != null)
                    {
                        arrival.planType = reservation.TipoPlan;
                    }
                    arrival.arrivalComments = reservation.Comentario;
                    arrival.roomListID = reservation.idroomlist;
                    if (reservation.Tarifa != null)
                    {
                        arrival.rate = (decimal)reservation.Tarifa;
                    }
                    arrival.currencyCode = reservation.codetipodemoneda;

                    var currentMarketCode = MarketCodes.FirstOrDefault(m => m.marketCode == reservation.Procedencia);
                    if (currentMarketCode != null && arrival.promotionTeamID == null)
                    {
                        if (arrival.programID == null || arrival.source != currentMarketCode.marketCode)
                        {
                            arrival.programID = currentMarketCode.programID;
                            if (arrival.arrivalsTravelSourceID == null || arrival.arrivalsTravelSourceID != currentMarketCode.arrivalsTravelSourceID)
                            {
                                arrival.arrivalsTravelSourceID = currentMarketCode.arrivalsTravelSourceID;
                            }
                        }
                    }
                    if (arrival.leadID == null)
                    {
                        //foreach (var pa in PAReservations)
                        //{
                        //    if (pa.hotelConfirmationNumber != null && (pa.hotelConfirmationNumber.Trim() == arrival.crs.Trim() || pa.hotelConfirmationNumber.Trim() == arrival.confirmationNumber.Trim()))
                        //    {
                        //        if (pa.bookingStatusID == 1 || pa.bookingStatusID == 16 || pa.secondaryBookingStatusID == 1 || pa.secondaryBookingStatusID == 32)
                        //        {
                        //            arrival.manifestedAsPA = true;
                        //        }
                        //        arrival.leadID = pa.leadID;
                        //        arrival.prearrivalTerminalID = pa.initialTerminalID;
                        //        break;
                        //    }
                        //}
                        /*
                        //búsqueda de llegada en lista de manifiesto de PARescom
                        foreach (var pa in PARescom)
                        {
                            if (pa.hotelConfirmationNumber != null && (pa.hotelConfirmationNumber.Trim() == arrival.crs.Trim() || pa.hotelConfirmationNumber.Trim() == arrival.confirmationNumber.Trim()))
                            {
                                if (pa.bookingStatusID == 1)
                                {
                                    arrival.manifestedAsPA = true;
                                }
                                arrival.leadID = pa.leadID;
                                arrival.prearrivalTerminalID = 10;
                                break;
                            }
                        }

                        //búsqueda de llegada en lista de manifiesto de PATafer
                        if (arrival.leadID == null)
                        {
                            foreach (var pa in PATafer)
                            {
                                if (pa.hotelConfirmationNumber != null && pa.hotelConfirmationNumber.Trim() == arrival.confirmationNumber.Trim())
                                {
                                    if (pa.bookingStatusID == 1)
                                    {
                                        arrival.manifestedAsPA = true;
                                    }
                                    arrival.leadID = pa.leadID;
                                    arrival.prearrivalTerminalID = 16;
                                    break;
                                }
                            }
                        }
                        */
                    }
                    if (!exists)
                    {
                        db.tblArrivals.AddObject(arrival);
                    }
                    db.SaveChanges();
                }//foreach                

                //buscar reservaciones del mismo cliente
                var CurrentArrivals = (from a in db.tblArrivals
                                       where FrontOfficeReservationIDs.Contains(a.frontOfficeReservationID)
                                       && a.arrivalDate >= fromDate
                                       && a.arrivalDate < toDate
                                       select a.arrivalID).ToList();

                foreach (var arrID in CurrentArrivals)
                {
                    var arr = (from a in db.tblArrivals
                               where a.arrivalID == arrID
                               select a).FirstOrDefault();
                    if (arr.extensionFromArrivalID == null && arr.contract != null && arr.contract != "")
                    {
                        var CustomerReservations = (from cr in db.tblArrivals
                                                    where cr.contract == arr.contract
                                                    && cr.arrivalDate < arr.arrivalDate
                                                    && !cr.reservationStatus.Contains("CA")
                                                    select new
                                                    {
                                                        cr.arrivalID,
                                                        cr.arrivalDate,
                                                        cr.nights,
                                                        cr.confirmationNumber
                                                    });

                        foreach (var res in CustomerReservations)
                        {
                            DateTime resDeparture = res.arrivalDate.Date.AddDays(res.nights);
                            if (arr.arrivalDate.Date == resDeparture)
                            {
                                arr.extensionFromArrivalID = res.arrivalID;
                            }
                        }
                    }

                    //si es una extensión, marcar como NQ
                    if (arr.extensionFromArrivalID != null)
                    {
                        arr.hostessQualificationStatusID = 2;
                    }
                    db.SaveChanges();
                }
            }
        }

        public static List<ImportModel> GetReservationsToSync(int resortID, DateTime from, DateTime to)
        {

            ePlatEntities db = new ePlatEntities();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var list = new List<ImportModel>();
            var query = FrontOfficeDataModel.GetArrivals(resortID, from, to);
            //var places = db.tblPlaces.Where(m => m.frontOfficeResortID != null);

            foreach (var item in query)
            {
                var _str = serializer.Serialize(item);
                var arrival = serializer.Deserialize<ImportModel>(_str);

                arrival.Resort = "";
                arrival.Arrival = arrival.llegada != null ? arrival.llegada.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                arrival.Departure = arrival.salida != null ? arrival.salida.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                arrival.llegada = (DateTime?)null;
                arrival.salida = (DateTime?)null;
                arrival.CheckIn = arrival.FechaHoraCheckin != null ? arrival.FechaHoraCheckin.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                arrival.FechaHoraCheckin = (DateTime?)null;
                list.Add(arrival);
            }

            try
            {
                //SyncArrivals(query, resortID, from, to);
            }
            catch (Exception e)
            {
                var email = new System.Net.Mail.MailMessage();
                email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                email.To.Add("gguerrap@villagroup.com");
                email.Subject = "SyncArrivals Error";
                email.IsBodyHtml = true;
                email.Body = "El método SyncArrivals no se completó.<br>ResortID: " + resortID + "<br>From: " + from.ToString("yyyy-MM-dd") + "<br>To: " + to.ToString("yyyy-MM-dd") + "<br>Error: " + e.InnerException;
                EmailNotifications.SendSync(email);
            }

            try
            {
                SyncReservations(list);
            }
            catch (Exception e)
            {
                var email = new System.Net.Mail.MailMessage();
                email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                email.To.Add("efalcon@villagroup.com");
                email.Subject = "SyncReservations Error";
                email.IsBodyHtml = true;
                email.Body = "El método SyncReservations no se completó.<br>ResortID: " + resortID + "<br>From: " + from.ToString("yyyy-MM-dd") + "<br>To: " + to.ToString("yyyy-MM-dd") + "<br>Error: " + e.InnerException;
                EmailNotifications.SendSync(email);
            }

            try
            {
                //UpdateReservationsFromEplat(resortID, from, to, list);
            }
            catch (Exception e)
            {
                var email = new System.Net.Mail.MailMessage();
                email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                email.To.Add("efalcon@villagroup.com");
                email.Subject = "UpdateReservations Error";
                email.IsBodyHtml = true;
                email.Body = "El método UpdateReservations no se completó.<br>ResortID: " + resortID + "<br>From: " + from.ToString("yyyy-MM-dd") + "<br>To: " + to.ToString("yyyy-MM-dd") + "<br>Error: " + e.InnerException;
                EmailNotifications.SendSync(email);
            }

            return list;
        }

        public static List<ImportModel> SyncReservations(List<ImportModel> arrivals)
        {
            ePlatEntities db = new ePlatEntities();
            db.CommandTimeout = int.MaxValue;

            DateTime now = DateTime.Now;
            var frontOfficeResortID = (int)arrivals.FirstOrDefault().idresort;
            var resort = db.tblPlaces.Where(m => m.frontOfficeResortID == frontOfficeResortID).Select(m => new { m.placeID, m.oldPlaceName }).FirstOrDefault();
            //var roomTypes = db.tblRoomTypes.Where(m => resort.placeID == m.placeID).Select(m => new { m.placeID, m.roomTypeID, m.roomTypeCode }).ToList();
            var counter = 0;
            foreach (var arrival in arrivals.OrderBy(m => m.Arrival))
            {
                int? frontOfficeGuestID = (int?)arrival.idhuesped;
                var frontOfficeReservationID = (long?)arrival.idReservacion;
                var frontOfficeRoomListID = arrival.idroomlist != null ? arrival.idroomlist : (int?)null;
                //var roomType = roomTypes.FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab);
                //var guestHub = db.tblGuestHub_FrontOffice.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID && m.frontOfficeGuestID == frontOfficeGuestID);

                var _existing = (from r in db.tblReservations
                                 join l in db.tblLeads on r.leadID equals l.leadID
                                 where l.frontOfficeResortID == frontOfficeResortID
                                 && r.frontOfficeReservationID == frontOfficeReservationID
                                 && r.isTest != true
                                 && (frontOfficeRoomListID == null || r.frontOfficeRoomListID == frontOfficeRoomListID)
                                 select r.reservationID).Distinct().ToList();

                if (_existing.Count() == 1)
                {
                    var id = _existing.FirstOrDefault();
                    var reservation = db.tblReservations.Single(m => m.reservationID == id);
                    var lead = reservation.tblLeads;

                    lead.personalTitleID = arrival.Titulo != null && arrival.Titulo.Trim() != "" ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : reservation.tblLeads.personalTitleID : reservation.tblLeads.personalTitleID;
                    lead.firstName = arrival.apellidopaterno != null && arrival.apellidopaterno.Trim() != "" && arrival.apellidopaterno.ToLower().IndexOf("pending") == -1 && arrival.apellidopaterno.ToLower().IndexOf("verified") == -1 && arrival.apellidopaterno.ToLower().IndexOf("deposit") == -1 ? arrival.nombres : reservation.tblLeads.firstName;
                    lead.lastName = arrival.apellidopaterno != null && arrival.apellidopaterno.Trim() != "" && arrival.apellidopaterno.ToLower().IndexOf("pending") == -1 && arrival.apellidopaterno.ToLower().IndexOf("verified") == -1 && arrival.apellidopaterno.ToLower().IndexOf("deposit") == -1 ? (arrival.apellidopaterno + " " + (arrival.apellidomaterno ?? "")) : reservation.tblLeads.lastName;
                    lead.countryID = arrival.codepais != null && arrival.codepais.Trim() != "" ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : reservation.tblLeads.countryID : reservation.tblLeads.countryID;
                    lead.frontOfficeGuestID = frontOfficeGuestID;

                    VerifyContactInfo(reservation.leadID);

                    if (lead.tblLeadEmails.Count() > 0 || lead.tblPhones.Count() > 0)
                    {
                        if (lead.bookingStatusID == 15)
                        {
                            lead.bookingStatusID = 10;
                        }
                    }

                    ///
                    /// si front nulo => status actual
                    /// si front != CA y eplat == CA => asignado
                    /// si front != CA y eplat != CA => status actual
                    /// si front == CA y eplat == CA(todas) => cancelado
                    /// si front == CA y eplat != CA(alguna) => asignado
                    ///
                    //reservation.tblLeads.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion != "CA" ? reservation.tblLeads.leadStatusID == 4 ? 2 : reservation.tblLeads.leadStatusID : reservation.tblLeads.tblReservations.Count(m => m.reservationStatusID == 3) == reservation.tblLeads.tblReservations.Count() ? 4 : 2 : reservation.tblLeads.leadStatusID;

                    if (arrival.codigostatusreservacion.Trim() == "CA" && reservation.reservationStatusID != 3)
                    {
                        //generar correo de notificacion de cancelacion en Front para el agente con la reservacion asignada
                        var body = "";
                        try
                        {
                            body = "<br /><br /><br /><strong>Subject:</strong> Reservation have been canceled in Front.";
                            body += "<br /><strong>Arrival Date:</strong> " + arrival.Arrival;
                            body += "<br /><strong>Hotel:</strong> " + resort.oldPlaceName;
                            body += "<br /><strong>Hotel Confirmation Number:</strong> " + arrival.numconfirmacion;
                            body += "<br /><strong>CRS:</strong> " + arrival.CRS;
                            body += "<br /><strong>Guest Name:</strong> " + arrival.Huesped;
                        }
                        catch
                        {
                            body = "<br /><br /><br /><strong>Subject:</strong> Reservation have been canceled in Front.";
                            body += "<br /><strong>ePlat ReservationID:</strong> " + reservation.reservationID.ToString();
                            body += "<br /><strong>Hotel:</strong> " + resort.oldPlaceName;
                            body += "<br /><strong>Hotel Confirmation Number:</strong> " + arrival.numconfirmacion;
                            body += "<br /><strong>Guest Name:</strong> " + arrival.Huesped;
                        }

                        var email = new System.Net.Mail.MailMessage();
                        email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                        //email.To.Add("efalcon@villagroup.com");
                        email.To.Add(lead.aspnet_Users1.UserName + ",efalcon@villagroup.com");
                        email.Subject = "Reservation Canceled in Front";
                        email.IsBodyHtml = true;
                        email.Body = body;
                        EmailNotifications.Send(email);
                        //EmailNotifications.SendSync(email);
                    }

                    reservation.arrivalDate = arrival.Arrival != null && arrival.Arrival.Trim() != "" ? DateTime.Parse(arrival.Arrival) : reservation.arrivalDate;
                    reservation.departureDate = arrival.Departure != null && arrival.Departure.Trim() != "" ? DateTime.Parse(arrival.Departure) : reservation.departureDate;
                    reservation.frontOfficeCertificateNumber = arrival.CRS;
                    reservation.hotelConfirmationNumber = arrival.numconfirmacion;
                    reservation.totalNights = arrival.CuartosNoche;
                    reservation.guestsNames = arrival.Huesped;
                    reservation.reservationComments = arrival.Comentario;
                    //reservation.reservationStatusDate = arrival.codigostatusreservacion != null ? reservation.reservationStatusID != Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value ? now : reservation.reservationStatusDate : reservation.reservationStatusDate;
                    //reservation.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : reservation.reservationStatusID : reservation.reservationStatusID;
                    reservation.reservationStatusDate = arrival.codigostatusreservacion != null && Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value != reservation.reservationStatusID ? now : reservation.reservationStatusDate : reservation.reservationStatusDate;
                    reservation.reservationStatusID = arrival.codigostatusreservacion != null && Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : reservation.reservationStatusID;

                    reservation.adults = arrival.Adultos ?? 0;
                    reservation.children = arrival.Ninos ?? 0;
                    reservation.frontOfficePlanType = arrival.TipoPlan;
                    reservation.frontOfficeAgencyName = arrival.nameagencia;
                    reservation.frontOfficeMarketCode = arrival.Procedencia;
                    reservation.frontOfficeRoomListID = arrival.idroomlist;

                    #region "lead status update"
                    if (arrival.codigostatusreservacion != null)
                    {
                        if (arrival.codigostatusreservacion == "CA")
                        {
                            if (lead.tblReservations.Count(m => m.reservationStatusID == 3) == lead.tblReservations.Count())
                            {
                                lead.leadStatusID = 4;
                            }
                            else if (lead.leadStatusID == 4)
                            {
                                lead.leadStatusID = 2;
                            }
                            else
                            {
                                lead.leadStatusID = lead.leadStatusID;
                            }
                        }
                        else if (lead.leadStatusID == 4)
                        {
                            lead.leadStatusID = 2;
                        }
                        else
                        {
                            lead.leadStatusID = lead.leadStatusID;
                        }
                    }
                    #endregion

                    db.SaveChanges();
                    counter++;
                }
                else if (_existing.Count() > 1)
                {
                    var existing = db.tblReservations.Where(m => _existing.Contains(m.reservationID)).Select(m => new { m.leadID, m.reservationID, m.tblOptionsSold }).ToList();
                    if (existing.Count() == existing.Count(m => m.leadID == existing.FirstOrDefault().leadID))//la multiplicidad de reservaciones está en el mismo lead.
                    {
                        if (existing.Count(m => m.tblOptionsSold.Count() > 0) > 0)
                        {
                            var toTest = existing.Where(m => m.tblOptionsSold.Count() == 0);
                            foreach (var i in toTest)
                            {
                                var rsv = db.tblReservations.FirstOrDefault(m => m.reservationID == i.reservationID);
                                rsv.isTest = true;
                                db.SaveChanges();
                            }

                            var email = new System.Net.Mail.MailMessage();
                            email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                            email.To.Add("efalcon@villagroup.com");
                            email.Subject = "Multiple Reservations Detected";
                            email.IsBodyHtml = true;
                            email.Body = "El método SyncReservations detectó múltiples reservaciones y las marcó como prueba. Por favor revisar y corroborar los cambios realizados.<br>Reservations ID: " + string.Join(",", toTest.Select(m => m.reservationID.ToString()).ToArray());
                            EmailNotifications.Send(email);
                            //EmailNotifications.SendSync(email);
                        }
                        else
                        {
                            var toTest = existing.Skip(1);
                            foreach (var i in toTest)
                            {
                                var rsv = db.tblReservations.FirstOrDefault(m => m.reservationID == i.reservationID);
                                rsv.isTest = true;
                                db.SaveChanges();
                            }

                            var email = new System.Net.Mail.MailMessage();
                            email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                            email.To.Add("efalcon@villagroup.com");
                            email.Subject = "Multiple Reservations Detected";
                            email.IsBodyHtml = true;
                            email.Body = "El método SyncReservations detectó múltiples reservaciones y las marcó como prueba. Por favor revisar y corroborar los cambios realizados.<br>Reservations ID: " + string.Join(",", toTest.Select(m => m.reservationID.ToString()).ToArray());
                            EmailNotifications.Send(email);
                            //EmailNotifications.SendSync(email);
                        }
                    }
                    else
                    {
                        var email = new System.Net.Mail.MailMessage();
                        email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                        email.To.Add("efalcon@villagroup.com");
                        email.Subject = "Multiple Reservations Detected";
                        email.IsBodyHtml = true;
                        email.Body = "El método SyncReservations detectó múltiples reservaciones pertenecientes a distintos leads. Por favor revisar y realizar los cambios necesarios.<br>Resort: " + resort.oldPlaceName + "<br>Reservations ID: " + string.Join(",", _existing.Select(m => m.ToString()).ToArray());
                        EmailNotifications.Send(email);
                        //EmailNotifications.SendSync(email);
                    }
                }
            }

            return arrivals;
        }

        //public static List<ImportModel> SyncReservations(List<ImportModel> arrivals, int? terminal = null)
        public static void __SyncReservations(List<ImportModel> arrivals, int? terminal = null)
        {
            ePlatEntities db = new ePlatEntities();
            PreArrivalDataModel pdm = new PreArrivalDataModel();

            DateTime now = DateTime.Now;
            var resort = db.tblPlaces.FirstOrDefault(m => m.frontOfficeResortID == (int?)arrivals.FirstOrDefault().idresort);
            int? frontOfficeResortID = resort.frontOfficeResortID; //(int?)arrivals.FirstOrDefault().idresort;

            foreach (var arrival in arrivals)
            {
                var frontOfficeGuestID = arrival.idhuesped;
                var frontOfficeReservationID = arrival.idReservacion;
                var frontOfficeRoomListID = arrival.idroomlist != null ? arrival.idroomlist : (int?)null;

                var exists = from r in db.tblReservations
                             join l in db.tblLeads on r.leadID equals l.leadID
                             where l.frontOfficeResortID == frontOfficeResortID
                             && r.frontOfficeReservationID == frontOfficeReservationID
                             && ((r.frontOfficeRoomListID == null && frontOfficeRoomListID == null) || r.frontOfficeRoomListID == frontOfficeRoomListID)
                             select r;

                if (exists != null && exists.Count() > 0)
                {
                    var reservation = exists.FirstOrDefault();

                    reservation.tblLeads.personalTitleID = arrival.Titulo != null && arrival.Titulo.Trim() != "" ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : reservation.tblLeads.personalTitleID : reservation.tblLeads.personalTitleID;
                    reservation.tblLeads.firstName = arrival.apellidopaterno != null && arrival.apellidopaterno.Trim() != "" && arrival.apellidopaterno.ToLower().IndexOf("pending") == -1 && arrival.apellidopaterno.ToLower().IndexOf("verified") == -1 && arrival.apellidopaterno.ToLower().IndexOf("deposit") == -1 ? arrival.nombres : reservation.tblLeads.firstName;
                    reservation.tblLeads.lastName = arrival.apellidopaterno != null && arrival.apellidopaterno.Trim() != "" && arrival.apellidopaterno.ToLower().IndexOf("pending") == -1 && arrival.apellidopaterno.ToLower().IndexOf("verified") == -1 && arrival.apellidopaterno.ToLower().IndexOf("deposit") == -1 ? (arrival.apellidopaterno + " " + (arrival.apellidomaterno ?? "")) : reservation.tblLeads.lastName;
                    reservation.tblLeads.frontOfficeGuestID = (int?)frontOfficeGuestID;

                    if (reservation.tblLeads.tblLeadEmails.Count() == 0 && reservation.tblLeads.tblPhones.Count() == 0)
                    {
                        PreArrivalDataModel.GetContactInfo(reservation.leadID);
                    }

                    if (reservation.tblLeads.tblLeadEmails.Count() > 0 || reservation.tblLeads.tblPhones.Count() > 0)
                    {
                        if (reservation.tblLeads.bookingStatusID == 15)
                        {
                            reservation.tblLeads.bookingStatusID = 10;
                        }
                    }
                    #region "notify reservation cancelation by email"
                    if (arrival.codigostatusreservacion == "CA" && reservation.reservationStatusID != 3)
                    {

                        var body = "";
                        try
                        {
                            body = "<br /><br /><br /><strong>Subject:</strong> Reservation have been canceled in Front.";
                            body += "<br /><strong>Arrival Date:</strong> " + arrival.Arrival;
                            body += "<br /><strong>Hotel:</strong> " + resort.oldPlaceName;
                            body += "<br /><strong>Hotel Confirmation Number:</strong> " + arrival.numconfirmacion;
                            body += "<br /><strong>CRS:</strong> " + arrival.CRS;
                            body += "<br /><strong>Guest Name:</strong> " + arrival.Huesped;
                        }
                        catch
                        {
                            body = "<br /><br /><br /><strong>Subject:</strong> Reservation have been canceled in Front.";
                            body += "<br /><strong>ePlat ReservationID:</strong> " + reservation.reservationID.ToString();
                            body += "<br /><strong>Hotel:</strong> " + resort.oldPlaceName;
                            body += "<br /><strong>Hotel Confirmation Number:</strong> " + arrival.numconfirmacion;
                            body += "<br /><strong>Guest Name:</strong> " + arrival.Huesped;
                        }

                        var email = new System.Net.Mail.MailMessage();
                        email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                        email.To.Add(reservation.tblLeads.aspnet_Users1.UserName);
                        email.Bcc.Add("efalcon@villagroup.com");
                        email.Subject = "Reservation Canceled in Front";
                        email.IsBodyHtml = true;
                        email.Body = body;

                        EmailNotifications.Send(email);
                    }
                    #endregion

                    reservation.arrivalDate = arrival.Arrival != null && arrival.Arrival.Trim() != "" ? DateTime.Parse(arrival.Arrival) : reservation.arrivalDate;
                    reservation.departureDate = arrival.Departure != null && arrival.Departure.Trim() != "" ? DateTime.Parse(arrival.Departure) : reservation.departureDate;
                    reservation.frontOfficeCertificateNumber = arrival.CRS;
                    reservation.hotelConfirmationNumber = arrival.numconfirmacion;
                    reservation.totalNights = arrival.CuartosNoche;
                    reservation.guestsNames = arrival.Huesped;
                    reservation.reservationComments = arrival.Comentario;
                    reservation.reservationStatusDate = arrival.codigostatusreservacion != null ? reservation.reservationStatusID != Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value ? now : reservation.reservationStatusDate : reservation.reservationStatusDate;
                    reservation.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : reservation.reservationStatusID : reservation.reservationStatusID;
                    reservation.adults = arrival.Adultos ?? 0;
                    reservation.children = arrival.Ninos ?? 0;
                    reservation.frontOfficePlanType = arrival.TipoPlan;
                    reservation.frontOfficeAgencyName = arrival.Procedencia;
                    reservation.frontOfficeRoomListID = arrival.idroomlist;

                    #region "lead status update"
                    if (arrival.codigostatusreservacion != null)
                    {
                        if (arrival.codigostatusreservacion.Trim() == "CA")
                        {
                            if (reservation.tblLeads.tblReservations.Count(m => m.reservationStatusID == 3) == reservation.tblLeads.tblReservations.Count())
                            {
                                if (reservation.tblLeads.leadStatusID != 35)
                                {
                                    reservation.tblLeads.leadStatusID = 4;
                                }
                            }
                            else if (reservation.tblLeads.leadStatusID == 4)
                            {
                                reservation.tblLeads.leadStatusID = 2;
                            }
                            else
                            {
                                reservation.tblLeads.leadStatusID = reservation.tblLeads.leadStatusID;
                            }
                        }
                        else if (reservation.tblLeads.leadStatusID == 4)
                        {
                            reservation.tblLeads.leadStatusID = 2;
                        }
                        else
                        {
                            reservation.tblLeads.leadStatusID = reservation.tblLeads.leadStatusID;
                        }
                    }
                    #endregion

                    db.SaveChanges();
                }
                else
                {

                }
            }

            //var result = pdm.UpdateArrivals(arrivals);

            //return result;
        }

        public static List<ImportModel> _SyncReservations(List<ImportModel> arrivals)
        {
            ePlatEntities db = new ePlatEntities();
            db.CommandTimeout = int.MaxValue;

            DateTime now = DateTime.Now;
            var frontOfficeResortID = (int)arrivals.FirstOrDefault().idresort;
            var resort = db.tblPlaces.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID);
            var roomTypes = db.tblRoomTypes.Where(m => resort.placeID == m.placeID).Select(m => new { m.placeID, m.roomTypeID, m.roomTypeCode });

            foreach (var arrival in arrivals.OrderBy(m => m.Arrival))
            {
                int? frontOfficeGuestID = (int?)arrival.idhuesped;
                var frontOfficeReservationID = (long?)arrival.idReservacion;
                var frontOfficeRoomListID = arrival.idroomlist != null ? arrival.idroomlist : (int?)null;
                var roomType = roomTypes.FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab);
                var guestHub = db.tblGuestHub_FrontOffice.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID && m.frontOfficeGuestID == frontOfficeGuestID);

                //var contactInfo = FrontOfficeDataModel.GetContactInfo(frontOfficeResortID, frontOfficeGuestID);


                var _existing = (from r in db.tblReservations
                                 join l in db.tblLeads on r.leadID equals l.leadID
                                 where l.frontOfficeResortID == frontOfficeResortID
                                 && r.frontOfficeReservationID == frontOfficeReservationID
                                 //&& r.frontOfficeRoomListID == frontOfficeRoomListID
                                 && r.isTest != true
                                 && (frontOfficeRoomListID == null || r.frontOfficeRoomListID == frontOfficeRoomListID)
                                 select r).ToList();

                if (_existing.Count() == 1)
                {
                    var reservation = _existing.FirstOrDefault();
                    reservation.tblLeads.personalTitleID = arrival.Titulo != null && arrival.Titulo.Trim() != "" ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : reservation.tblLeads.personalTitleID : reservation.tblLeads.personalTitleID;
                    reservation.tblLeads.firstName = arrival.apellidopaterno != null && arrival.apellidopaterno.Trim() != "" && arrival.apellidopaterno.ToLower().IndexOf("pending") == -1 && arrival.apellidopaterno.ToLower().IndexOf("verified") == -1 && arrival.apellidopaterno.ToLower().IndexOf("deposit") == -1 ? arrival.nombres : reservation.tblLeads.firstName;
                    reservation.tblLeads.lastName = arrival.apellidopaterno != null && arrival.apellidopaterno.Trim() != "" && arrival.apellidopaterno.ToLower().IndexOf("pending") == -1 && arrival.apellidopaterno.ToLower().IndexOf("verified") == -1 && arrival.apellidopaterno.ToLower().IndexOf("deposit") == -1 ? (arrival.apellidopaterno + " " + (arrival.apellidomaterno ?? "")) : reservation.tblLeads.lastName;
                    reservation.tblLeads.countryID = arrival.codepais != null && arrival.codepais.Trim() != "" ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : reservation.tblLeads.countryID : reservation.tblLeads.countryID;
                    reservation.tblLeads.frontOfficeGuestID = frontOfficeGuestID;

                    PreArrivalDataModel.GetContactInfo(reservation.leadID);

                    if (reservation.tblLeads.tblLeadEmails.Count() > 0 || reservation.tblLeads.tblPhones.Count() > 0)
                    {
                        if (reservation.tblLeads.bookingStatusID == 15)
                        {
                            reservation.tblLeads.bookingStatusID = 10;
                        }
                    }

                    ///
                    /// si front nulo => status actual
                    /// si front != CA y eplat == CA => asignado
                    /// si front != CA y eplat != CA => status actual
                    /// si front == CA y eplat == CA(todas) => cancelado
                    /// si front == CA y eplat != CA(alguna) => asignado
                    ///
                    //reservation.tblLeads.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion != "CA" ? reservation.tblLeads.leadStatusID == 4 ? 2 : reservation.tblLeads.leadStatusID : reservation.tblLeads.tblReservations.Count(m => m.reservationStatusID == 3) == reservation.tblLeads.tblReservations.Count() ? 4 : 2 : reservation.tblLeads.leadStatusID;

                    if (arrival.codigostatusreservacion == "CA" && reservation.reservationStatusID != 3)
                    {
                        //generar correo de notificacion de cancelacion en Front para el agente con la reservacion asignada
                        var body = "";
                        try
                        {
                            body = "<br /><br /><br /><strong>Subject:</strong> Reservation have been canceled in Front.";
                            body += "<br /><strong>Arrival Date:</strong> " + arrival.Arrival;
                            body += "<br /><strong>Hotel:</strong> " + resort.oldPlaceName;
                            body += "<br /><strong>Hotel Confirmation Number:</strong> " + arrival.numconfirmacion;
                            body += "<br /><strong>CRS:</strong> " + arrival.CRS;
                            body += "<br /><strong>Guest Name:</strong> " + arrival.Huesped;
                        }
                        catch
                        {
                            body = "<br /><br /><br /><strong>Subject:</strong> Reservation have been canceled in Front.";
                            body += "<br /><strong>ePlat ReservationID:</strong> " + reservation.reservationID.ToString();
                            body += "<br /><strong>Hotel:</strong> " + resort.oldPlaceName;
                            body += "<br /><strong>Hotel Confirmation Number:</strong> " + arrival.numconfirmacion;
                            body += "<br /><strong>Guest Name:</strong> " + arrival.Huesped;
                        }

                        var email = new System.Net.Mail.MailMessage();
                        email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                        email.To.Add(reservation.tblLeads.aspnet_Users1.UserName);
                        //enviar correo a mí y a agente 
                        email.Subject = "Reservation Canceled in Front";
                        email.IsBodyHtml = true;
                        email.Body = body;
                        //EmailNotifications.SendSync(email);
                        EmailNotifications.Send(email);
                    }

                    reservation.arrivalDate = arrival.Arrival != null && arrival.Arrival.Trim() != "" ? DateTime.Parse(arrival.Arrival) : reservation.arrivalDate;
                    reservation.departureDate = arrival.Departure != null && arrival.Departure.Trim() != "" ? DateTime.Parse(arrival.Departure) : reservation.departureDate;
                    reservation.frontOfficeCertificateNumber = arrival.CRS;
                    reservation.hotelConfirmationNumber = arrival.numconfirmacion;
                    reservation.totalNights = arrival.CuartosNoche;
                    reservation.guestsNames = arrival.Huesped;
                    reservation.reservationComments = arrival.Comentario;
                    reservation.reservationStatusDate = arrival.codigostatusreservacion != null ? reservation.reservationStatusID != Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value ? now : reservation.reservationStatusDate : reservation.reservationStatusDate;
                    reservation.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : reservation.reservationStatusID : reservation.reservationStatusID;
                    reservation.adults = arrival.Adultos ?? 0;
                    reservation.children = arrival.Ninos ?? 0;
                    reservation.frontOfficePlanType = arrival.TipoPlan;
                    reservation.frontOfficeAgencyName = arrival.nameagencia;
                    reservation.frontOfficeMarketCode = arrival.Procedencia;
                    reservation.frontOfficeRoomListID = arrival.idroomlist;

                    #region "lead status update"
                    if (arrival.codigostatusreservacion != null)
                    {
                        if (arrival.codigostatusreservacion.Trim() == "CA")
                        {
                            if (reservation.tblLeads.tblReservations.Count(m => m.reservationStatusID == 3) == reservation.tblLeads.tblReservations.Count())
                            {
                                if (reservation.tblLeads.leadStatusID != 35)
                                {
                                    reservation.tblLeads.leadStatusID = 4;
                                }
                            }
                            else if (reservation.tblLeads.leadStatusID == 4)
                            {
                                reservation.tblLeads.leadStatusID = 2;
                            }
                            else
                            {
                                reservation.tblLeads.leadStatusID = reservation.tblLeads.leadStatusID;
                            }
                        }
                        else if (reservation.tblLeads.leadStatusID == 4)
                        {
                            reservation.tblLeads.leadStatusID = 2;
                        }
                        else
                        {
                            reservation.tblLeads.leadStatusID = reservation.tblLeads.leadStatusID;
                        }
                    }
                    #endregion

                    db.SaveChanges();
                }
                else if (_existing.Count() > 1)
                {
                    if (_existing.Count() == _existing.Count(m => m.leadID == _existing.FirstOrDefault().leadID))//la multiplicidad de reservaciones está en el mismo lead.
                    {
                        if (_existing.Count(m => m.tblOptionsSold.Count() > 0) > 0)
                        {
                            var toTest = _existing.Where(m => m.tblOptionsSold.Count() == 0).ToList();
                            foreach (var i in toTest)
                            {
                                var rsv = db.tblReservations.FirstOrDefault(mbox => mbox.reservationID == i.reservationID);
                                rsv.isTest = true;
                                db.SaveChanges();
                            }

                            var email = new System.Net.Mail.MailMessage();
                            email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                            email.To.Add("efalcon@villagroup.com");
                            email.Subject = "Multiple Reservations Detected";
                            email.IsBodyHtml = true;
                            email.Body = "El método SyncReservations detectó múltiples reservaciones y las marcó como prueba. Por favor revisar y corroborar los cambios realizados.<br>Reservations ID: " + string.Join(",", toTest.Select(m => m.reservationID).ToArray());
                            EmailNotifications.SendSync(email);
                        }
                        else
                        {
                            var toTest = new List<tblReservations>();
                            if (_existing.Count(m => m.tblLeads.leadStatusID == 10) > 0)
                            {
                                toTest = _existing.Where(m => m.tblLeads.leadStatusID == 10).ToList();
                            }
                            else
                            {
                                toTest = _existing.Skip(1).ToList();
                            }
                            //var toTest = _existing.Skip(1).ToList();
                            foreach (var i in toTest)
                            {
                                var rsv = db.tblReservations.FirstOrDefault(mbox => mbox.reservationID == i.reservationID);
                                rsv.isTest = true;
                                db.SaveChanges();
                            }

                            var email = new System.Net.Mail.MailMessage();
                            email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                            email.To.Add("efalcon@villagroup.com");
                            email.Subject = "Multiple Reservations Detected";
                            email.IsBodyHtml = true;
                            email.Body = "El método SyncReservations detectó múltiples reservaciones y las marcó como prueba. Por favor revisar y corroborar los cambios realizados.<br>Reservations ID: " + string.Join(",", toTest.Select(m => m.reservationID).ToArray());
                            EmailNotifications.SendSync(email);
                        }
                    }
                    else
                    {
                        var email = new System.Net.Mail.MailMessage();
                        email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                        email.To.Add("efalcon@villagroup.com");
                        email.Subject = "Multiple Reservations Detected";
                        email.IsBodyHtml = true;
                        email.Body = "El método SyncReservations detectó múltiples reservaciones pertenecientes a distintos leads. Por favor revisar y realizar los cambios necesarios.<br>Resort: " + resort.oldPlaceName + "<br>Reservations ID: " + string.Join(",", _existing.Select(m => m.reservationID).ToArray());
                        EmailNotifications.SendSync(email);
                    }
                }
            }

            return arrivals;
        }

        public static bool UpdateReservationsFromEplat(int resortID, DateTime _from, DateTime to, List<ImportModel> arrivals)
        {
            ePlatEntities db = new ePlatEntities();

            try
            {
                var eplatQuery = from r in db.tblReservations
                                 join l in db.tblLeads on r.leadID equals l.leadID
                                 where (r.arrivalDate.Value >= _from && r.arrivalDate.Value <= to)
                                 && l.frontOfficeResortID == resortID
                                 && l.inputMethodID == 2
                                 select r;

                var frontQuery = arrivals;
                //var lamera = eplatQuery.Where(m => m.hotelConfirmationNumber.IndexOf("474198") != -1);
                foreach (var r in eplatQuery)
                {
                    try
                    {
                        if (frontQuery.Count(m => m.numconfirmacion == r.hotelConfirmationNumber) == 0)
                        {
                            var _guest = FrontOfficeDataModel.GetReservationsHistory(resortID, r.tblLeads.frontOfficeGuestID);
                            if (_guest != null)
                            {
                                var guest = _guest.OrderByDescending(m => m.llegada).FirstOrDefault();
                                r.numberAdults = guest.numadultos != null ? guest.numadultos.ToString() : null;
                                r.numberChildren = guest.numchilds != null ? guest.numchilds.ToString() : null;
                                r.arrivalDate = guest.llegada;
                                r.departureDate = guest.salida;
                            }
                            else
                            {
                                r.foundInFront = false;
                            }
                        }
                    }
                    catch { }
                }

                var arrivalsQuery = from a in db.tblArrivals
                                    where a.frontOfficeResortID == resortID
                                    && (a.arrivalDate >= _from && a.arrivalDate <= to)
                                    select a;

                foreach (var a in arrivalsQuery)
                {
                    try
                    {
                        if (frontQuery.Count(m => m.idReservacion == a.frontOfficeReservationID) == 0)
                        {
                            var _guest = FrontOfficeDataModel.GetReservationsHistory(resortID, (int?)a.frontOfficeGuestID);
                            if (_guest != null)
                            {
                                var guest = _guest.OrderByDescending(m => m.llegada).FirstOrDefault();
                                a.adults = guest.numadultos ?? 0;
                                a.children = guest.numchilds ?? 0;
                                a.arrivalDate = (DateTime)guest.llegada;
                            }
                        }
                    }
                    catch { }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //var email = new System.Net.Mail.MailMessage();
                //email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                //email.To.Add("efalcon@villagroup.com");
                //email.Subject = "UpdateReservations Error";
                //email.IsBodyHtml = true;
                //email.Body = "El método UpdateReservations no se completó.<br>ResortID: " + resortID + "<br>From: " + _from.ToString("yyyy-MM-dd") + "<br>To: " + to.ToString("yyyy-MM-dd") + "<br>Error: " + e.InnerException;
                //EmailNotifications.SendSync(email);
                //return false;
            }
        }

        public static string VerifyContactInfo(Guid id)
        {
            ePlatEntities db = new ePlatEntities();
            try
            {
                //var lead = db.tblLeads.Where(m => m.leadID == id).Select(m => new { m.frontOfficeGuestID, m.frontOfficeResortID, m.tblLeadEmails, m.tblPhones }).FirstOrDefault();
                var lead = (from l in db.tblLeads
                            join e in db.tblLeadEmails on l.leadID equals e.leadID into l_e
                            from e in l_e.DefaultIfEmpty()
                            join p in db.tblPhones on l.leadID equals p.leadID into l_p
                            from p in l_p.DefaultIfEmpty()
                            where l.leadID == id
                            select new
                            {
                                l.leadID,
                                l.frontOfficeGuestID,
                                l.frontOfficeResortID,
                                emails = e.emailID,
                                phones = p.phoneID
                            }).FirstOrDefault();

                var update = false;
                var emails = lead.emails;
                var phones = lead.emails;
                var resortID = lead.frontOfficeResortID;
                var guestID = lead.frontOfficeGuestID;

                //if (emails.Count() == 0 || phones.Count() == 0)
                if (emails == null || emails == 0 || phones == null || phones == 0)
                {
                    var contactInfo = FrontOfficeDataModel.GetContactInfo((int)resortID, guestID);
                    if ((emails == null || emails == 0) && contactInfo.Email != null)
                    {
                        var email = new tblLeadEmails();
                        email.leadID = id;
                        email.main = true;
                        email.email = contactInfo.Email;
                        db.tblLeadEmails.AddObject(email);
                        db.SaveChanges();
                        update = true;
                    }

                    //if (phones.Count() == 0 && contactInfo.Phone != null)
                    if ((phones == null || phones == 0) && contactInfo.Phone != null)
                    {
                        var phone = new tblPhones();
                        phone.leadID = id;
                        phone.phoneTypeID = 1;
                        phone.phone = contactInfo.Phone;
                        phone.main = true;
                        phone.ext = null;
                        phone.doNotCall = false;
                        db.tblPhones.AddObject(phone);
                        db.SaveChanges();
                        update = true;
                    }

                    //if (emails.Count() == 0 || phones.Count() == 0)//verificar si estas variables actualizan su contenido al guardar info en las tablas.
                    if (!update)//verificar si estas variables actualizan su contenido al guardar info en las tablas.
                    {
                        try
                        {
                            resortConnectEntities rce = new resortConnectEntities();

                            var reservations = db.tblReservations.Where(m => m.leadID == lead.leadID && m.reservationStatusID != 3).ToList(); //lead.tblReservations.Where(m => m.reservationStatusID != 3);
                            var certificates = reservations.Select(m => m.frontOfficeCertificateNumber).ToList();
                            var reservation = reservations.OrderByDescending(m => m.arrivalDate).FirstOrDefault();
                            var rcResortID = reservation.tblPlaces.resortConnectResortID;
                            var aDate = reservations.OrderBy(m => m.arrivalDate).FirstOrDefault().arrivalDate.Value.Date;
                            var dDate = reservations.OrderByDescending(m => m.arrivalDate).FirstOrDefault().departureDate.Value.Date;

                            var rc = (from r in rce.Reservation
                                      join c in rce.Reservation_Contact on r.ReservationId equals c.ReservationId
                                      where certificates.Contains(r.ConfirmationNumber)
                                      && r.CheckInDate == aDate
                                      && (r.CheckOutDate == dDate || dDate == null)
                                      && (r.ResortNumber == rcResortID || rcResortID == null)
                                      select new
                                      {
                                          c.PhoneNumber,
                                          c.EmailAddress
                                      }).FirstOrDefault();

                            if (rc != null)
                            {
                                //if (emails.Count() == 0 && rc.EmailAddress != null)
                                if ((emails == null || emails == 0) && rc.EmailAddress != null)
                                {
                                    var email = new tblLeadEmails();
                                    email.leadID = id;
                                    email.main = true;
                                    email.email = rc.EmailAddress;
                                    db.tblLeadEmails.AddObject(email);
                                    db.SaveChanges();
                                }
                                //if (phones.Count() == 0 && rc.PhoneNumber != null)
                                if ((phones == null || phones == 0) && rc.PhoneNumber != null)
                                {
                                    var phone = new tblPhones();
                                    phone.leadID = id;
                                    phone.phoneTypeID = 4;
                                    phone.phone = rc.PhoneNumber;
                                    phone.main = true;
                                    phone.ext = null;
                                    phone.doNotCall = false;
                                    db.tblPhones.AddObject(phone);
                                    db.SaveChanges();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message + " Inner Exception: " + e.InnerException ?? "");
                        }
                    }
                }
                return "True";
            }
            catch (Exception e) { return e.Message; }
        }

        public void SendEmailsByRule(int ruleID = 0, string ruleDate = null)
        {
            ePlatEntities db = new ePlatEntities();
            ecommerceEntities dba = new ecommerceEntities();
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var now = DateTime.Now;
            var startDay = ruleDate != null ? DateTime.Parse(ruleDate).Date : DateTime.Today;
            var endDay = startDay.AddDays(1).AddSeconds(-1);
            var listTerminalsByContext = new List<KeyValuePair<int, long>>();
            var rules = ruleID != 0 ? dba.tbaEnviosAutomaticos.Where(x => x.idEnvioAutomatico == ruleID).ToList() : dba.tbaEnviosAutomaticos.ToList();
            foreach (var i in db.tblTerminals.Where(m => m.ecommerceTerminalID != null))
            {
                listTerminalsByContext.Add(new KeyValuePair<int, long>((int)i.ecommerceTerminalID, i.terminalID));
            }

            //var rule = dba.tbaEnviosAutomaticos.Single(m => m.idEnvioAutomatico == 35);//testing
            //foreach (var rule in rules.Where(m => m.idEnvioAutomatico >=42))
            foreach (var rule in rules)
            {
                if (rule.active)
                {
                    var fieldGroupGuid = rule.fieldGroupID != null ? db.tblFieldGroups.FirstOrDefault(m => m.fieldGroupID == rule.fieldGroupID).fieldGroupGUID.ToString() : "";
                    var fields = rule.fieldGroupID != null ? db.tblFields.Where(m => m.fieldGroupID == rule.fieldGroupID && (m.fieldTypeID == 2 || m.fieldTypeID == 3) && m.field.ToLower() != "open" && m.field.ToLower() != "submitted") : null;
                    var iDate = rule.diasDespues > 0 ? startDay.AddDays(-rule.diasDespues) : startDay.AddDays((-1 * rule.diasDespues));
                    var fDate = iDate.AddDays(1).AddSeconds(-1);
                    var ruleTerminals = new List<long>();
                    var destinations = new List<string>();
                    var resorts = new List<string>();
                    var cco = rule.cco?.Replace(";", ",");
                    var replyTo = rule.correoResponderA?.Replace(";", ",");
                    var errorMessagesByRule = "";

                    var listInstances = new List<MailMessageResponse>();

                    foreach (var i in rule.terminales.Split(','))
                    {
                        if (ruleTerminals.Count(m => m.ToString() == i.Split('|')[0]) == 0)
                            ruleTerminals.Add(long.Parse(i.Split('|')[0]));

                        var tempDest = i.IndexOf('|') != -1 ? i.Split('|')[1].Split('_')[0] : null;
                        if (tempDest != null && destinations.Count(m => m == tempDest) == 0)
                            destinations.Add(tempDest);

                        if (i.IndexOf("null") == -1)
                        {
                            if (i.IndexOf('_') != -1)
                            {
                                resorts.Concat(i.Split('_')[1].Split('-').ToList());
                            }
                        }
                    }

                    switch (rule.tipoRegla)
                    {
                        case "reservation"://tbaReservaciones
                            {
                                try
                                {
                                    var reservationStatus = new int?[] { 2, 5, 8 };
                                    var _destinations = Array.ConvertAll(destinations.ToArray(), int.Parse).Cast<int?>().ToArray();
                                    var _resorts = resorts.Count() > 0 ? Array.ConvertAll(resorts.ToArray(), int.Parse).Cast<int?>().ToArray() : new int?[] { };

                                    var query = from r in dba.tbaReservaciones
                                                where !reservationStatus.Contains(r.idEstatusReservacion)
                                                && ruleTerminals.Contains(r.tbaProspectos.idTerminal)
                                                && (_destinations.Count() == 0 || _destinations.Contains(r.idDestino))
                                                && (_resorts.Count() == 0 || _resorts.Contains(r.idLugar))
                                                && (rule.llegada == false || iDate <= r.tbaRegistrosPresentacion.FirstOrDefault().fechaHora && r.tbaRegistrosPresentacion.FirstOrDefault().fechaHora <= fDate)
                                                && (rule.llegada == true || iDate <= r.salida && r.salida <= fDate)
                                                orderby r.tbaProspectos.email
                                                select r;

                                    foreach (var r in query)
                                    {
                                        if (rule.cultura == null || rule.cultura == "en-US" || (rule.cultura == "es-MX" && r.tbaProspectos.idPais == 3))
                                        {
                                            if (GeneralFunctions.IsEmailValid(r.tbaProspectos.email))
                                            {
                                                var list = new List<KeyValue>();
                                                var id = r.idReservacion.ToString();
                                                var transactionID = Guid.NewGuid();
                                                var _terminals = listTerminalsByContext.Where(m => ruleTerminals.Contains(m.Key)).Select(m => (long?)m.Value).ToList();
                                                var _exists = db.tblFieldsValues.Where(m => _terminals.Contains(m.terminalID));
                                                var mailBody = rule.cuerpoCorreo;
                                                var mailSubject = rule.asuntoCorreo;

                                                if (_exists.Count(m => m.value == id && startDay <= m.dateSaved && m.dateSaved <= endDay) == 0)
                                                {
                                                    //if (fields != null)
                                                    if (fields != null || rule.fieldGroupID == null)
                                                    {
                                                        //new
                                                        if (fields != null)
                                                        {
                                                            foreach (var field in fields)
                                                            {
                                                                list.Add(new KeyValue() { Key = field.field, Value = null });
                                                            }
                                                            ReplaceReservedWords("ecommerce", r, query.GetType(), ref list, now);
                                                            foreach (var i in list)
                                                            {
                                                                tblFieldsValues instance = new tblFieldsValues();
                                                                instance.fieldID = fields.Single(m => m.field == i.Key).fieldID;
                                                                instance.terminalID = listTerminalsByContext.FirstOrDefault(m => m.Key == r.tbaProspectos.idTerminal).Value;
                                                                instance.value = i.Value;
                                                                instance.dateSaved = now;
                                                                instance.transactionID = transactionID;
                                                                instance.publish = false;
                                                                db.tblFieldsValues.AddObject(instance);

                                                                mailBody = mailBody.Replace(i.Key, i.Value);
                                                                mailSubject = mailSubject.Replace(i.Key, i.Value);
                                                            }
                                                        }

                                                        mailBody = mailBody.Replace("$FieldGroupGUID", fieldGroupGuid)
                                                            .Replace("$LastName", r.tbaProspectos.lastName)//para el caso de las reglas que no tienen fieldGroupID relacionado
                                                            .Replace("$TransactionID", transactionID.ToString());

                                                        try
                                                        {
                                                            db.SaveChanges();

                                                            #region "mail instance"
                                                            MailMessage email = new MailMessage();
                                                            email.From = (rule.aliasRemitente != null ? new MailAddress(rule.correoRemitente, rule.aliasRemitente) : new MailAddress(rule.correoRemitente));
                                                            email.To.Add(r.tbaProspectos.email);
                                                            email.Subject = mailSubject;
                                                            email.Body = mailBody;
                                                            email.IsBodyHtml = true;
                                                            if (cco != null)
                                                                email.Bcc.Add(cco);
                                                            if (replyTo != null)
                                                                email.ReplyToList.Add(replyTo);

                                                            MailMessageResponse x = new MailMessageResponse
                                                            {
                                                                MailMessage = email
                                                            };
                                                            listInstances.Add(x);
                                                            #endregion
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            errorMessagesByRule += "<br />Rule ID: " + rule.idEnvioAutomatico;
                                                            errorMessagesByRule += "<br />Reservation ID: " + r.idReservacion.ToString();
                                                            errorMessagesByRule += "<br />Transaction ID: " + transactionID.ToString();
                                                            errorMessagesByRule += "<br />Detail: When trying to save changes.<br />Message: " + ex.Message + "<br />Inner Exception: " + ex.InnerException != null ? ex.InnerException.Message != null ? ex.InnerException.Message : "" : "";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorMessagesByRule += "<br />Rule ID: " + rule.idEnvioAutomatico;
                                    errorMessagesByRule += "<br />Detail: When assigning values.<br />Message: " + ex.Message + "<br />Inner Exception: " + (ex.InnerException != null ? ex.InnerException.Message ?? "" : "");
                                }
                                break;
                            }
                        case "service"://tblPurchases
                            {
                                try
                                {
                                    var _terminals = listTerminalsByContext.Select(m => m.Value).ToList();
                                    var query = from p in db.tblPurchases
                                                where ruleTerminals.Contains(p.terminalID)
                                                && p.culture == rule.cultura
                                                && p.tblPointsOfSale.online
                                                && p.tblPurchases_Services.Count(m => m.serviceStatusID == 3 && iDate <= m.serviceDateTime && m.serviceDateTime <= fDate) > 0
                                                select p;

                                    foreach (var p in query)
                                    {
                                        if (p.tblLeads.tblLeadEmails.Count() > 0 && GeneralFunctions.IsEmailValid(p.tblLeads.tblLeadEmails.OrderByDescending(m => m.main).FirstOrDefault().email))
                                        {
                                            var list = new List<KeyValue>();
                                            var id = p.purchaseID.ToString();
                                            var domain = GeneralFunctions.GetDomain(id);
                                            var transactionID = Guid.NewGuid();
                                            var _exists = db.tblFieldsValues.Where(m => ruleTerminals.Contains((long)m.terminalID));
                                            var mailBody = rule.cuerpoCorreo;
                                            var mailSubject = rule.asuntoCorreo;
                                            var str = "";
                                            var services = p.tblPurchases_Services.Where(m => m.serviceStatusID == 3).Select(m => new { service = m.tblServices.tblServiceDescriptions.Count() > 0 ? m.tblServices.tblServiceDescriptions.FirstOrDefault(x => x.culture == p.culture).service : m.tblServices.service, m.tblServices.tblProviders.comercialName, m.serviceDateTime });

                                            foreach (var i in services)
                                            {
                                                str += (str == "" ? "" : "<br />")
                                            + "<p><strong>" + i.service + "</strong>"
                                            + "<br />" + (p.culture == "es-MX" ? "por " : "by ") + i.comercialName
                                            + "<br />" + (p.culture == "es-MX" ? "visitado el " : "visited on ") + i.serviceDateTime.ToString("D", CultureInfo.GetCultureInfo(p.culture)) + "</p>";
                                            }


                                            if (_exists.Count(m => m.value == id && startDay <= m.dateSaved && m.dateSaved <= endDay) == 0)
                                            {
                                                if (fields != null)
                                                {
                                                    //new
                                                    foreach (var field in fields)
                                                    {
                                                        list.Add(new KeyValue() { Key = field.field, Value = null });
                                                    }
                                                    ReplaceReservedWords("ePlat", p, query.GetType(), ref list, now);
                                                    foreach (var i in list)
                                                    {
                                                        tblFieldsValues instance = new tblFieldsValues();
                                                        instance.fieldID = fields.Single(m => m.field == i.Key).fieldID;
                                                        instance.terminalID = p.terminalID;
                                                        instance.value = i.Value;
                                                        instance.dateSaved = now;
                                                        instance.transactionID = transactionID;
                                                        instance.publish = false;
                                                        db.tblFieldsValues.AddObject(instance);

                                                        mailBody = mailBody.Replace(i.Key, i.Value);
                                                        mailSubject = mailSubject.Replace(i.Key, i.Value);
                                                    }

                                                    mailBody = mailBody.Replace("$FieldGroupGUID", fieldGroupGuid)
                                                        .Replace("$TransactionID", transactionID.ToString())
                                                        .Replace("$Domain", domain)
                                                        .Replace("$BodyMail", str);

                                                    mailSubject = mailSubject.Replace("$Domain", domain.Replace("https://", ""));

                                                    try
                                                    {
                                                        db.SaveChanges();

                                                        #region "mail instance"
                                                        MailMessage email = new MailMessage();
                                                        email.From = (rule.aliasRemitente != null ? new MailAddress(rule.correoRemitente, rule.aliasRemitente) : new MailAddress(rule.correoRemitente));
                                                        email.To.Add(p.tblLeads.tblLeadEmails.OrderByDescending(m => m.main).FirstOrDefault().email);
                                                        email.Subject = mailSubject;
                                                        email.Body = mailBody;
                                                        email.IsBodyHtml = true;
                                                        if (cco != null)
                                                            email.Bcc.Add(cco);
                                                        if (replyTo != null)
                                                            email.ReplyToList.Add(replyTo);
                                                        MailMessageResponse r = new MailMessageResponse
                                                        {
                                                            MailMessage = email
                                                        };
                                                        listInstances.Add(r);
                                                        #endregion
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        errorMessagesByRule += "<br />Rule ID: " + rule.idEnvioAutomatico;
                                                        errorMessagesByRule += "<br />Purchase ID: " + p.purchaseID.ToString();
                                                        errorMessagesByRule += "<br />Transaction ID: " + transactionID.ToString();
                                                        errorMessagesByRule += "<br />Detail: When trying to save changes.<br />Message: " + ex.Message + "<br />Inner Exception: " + (ex.InnerException != null ? ex.InnerException.Message ?? "" : "");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorMessagesByRule += "<br />Rule ID: " + rule.idEnvioAutomatico;
                                    errorMessagesByRule += "<br />Detail: When assigning values.<br />Message: " + ex.Message + "<br />Inner Exception: " + (ex.InnerException != null ? ex.InnerException.Message ?? "" : "");
                                }
                                break;
                            }
                        case "hotelConfReminder"://tbaReservaciones
                            {
                                try
                                {
                                    var firmas = new List<Guid>();
                                    var date = startDay.AddDays(rule.diasDespues);
                                    var reservationStatus = new int?[] { 2, 4, 8, 15, 16, 21, 23 };
                                    var _destinations = Array.ConvertAll(destinations.ToArray(), int.Parse).Cast<int?>().ToArray();
                                    var _resorts = resorts.Count() > 0 ? Array.ConvertAll(resorts.ToArray(), int.Parse).Cast<int?>().ToArray() : new int?[] { };

                                    var query = (from rsv in dba.tbaReservaciones
                                                 join sign in dba.tbaFirmas on rsv.idReservacion equals sign.idReservacion
                                                 where !reservationStatus.Contains(rsv.idEstatusReservacion)
                                                 && ruleTerminals.Contains(rsv.tbaProspectos.idTerminal)
                                                 && (_destinations.Count() == 0 || _destinations.Contains(rsv.idDestino))
                                                 && (_resorts.Contains(rsv.idLugar) || _resorts.Count() == 0)
                                                 && rsv.llegada > endDay
                                                 && rsv.cartaConfirmacionEnviadaPor != null && rsv.fechaEnvioCartaConfirmacion != null
                                                 && (sign.idTipodeDocumento == 1 && sign.fechaFirma == null && sign.firma == null && ((sign.fechaUltimoEnvio == null && sign.fechaCreacion <= date) || sign.fechaUltimoEnvio == date) && sign.urlDocumento.Contains("hotelAndPresentationConfirmation"))
                                                 select rsv).ToList();

                                    foreach (var r in query)
                                    {
                                        if (GeneralFunctions.IsEmailValid(r.tbaProspectos.email))
                                        {
                                            var signature = r.tbaFirmas.Where(m => m.idTipodeDocumento == 1 && m.urlDocumento.Contains("hotelAndPresentationConfirmation")).OrderByDescending(m => m.fechaCreacion).FirstOrDefault();
                                            var firma = signature.firma != null ? (Guid?)null : signature.idFirma;
                                            var mailBody = rule.cuerpoCorreo;
                                            var mailSubject = rule.asuntoCorreo;

                                            if (firma != null && !firmas.Contains((Guid)firma))
                                            {
                                                firmas.Add((Guid)firma);
                                                mailBody = mailBody.Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;firstName&quot; labeled=&quot;false&quot; /]", r.tbaProspectos.firstName)
                                                .Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;lastName&quot; labeled=&quot;false&quot; /]", r.tbaProspectos.lastName)
                                                .Replace("[vg:var table=&quot;tbaLugares&quot; field=&quot;lugar&quot; labeled=&quot;false&quot; /]", r.tbaLugares.lugar)
                                                .Replace("here [link]", "<a href=\"http://eplatform.villagroup.com/signit/?signature=" + firma + "\">here</a>");

                                                try
                                                {
                                                    dba.tbaFirmas.Single(m => m.idFirma == firma).fechaUltimoEnvio = startDay;
                                                    dba.SaveChanges();

                                                    #region "mail instance"
                                                    MailMessage email = new MailMessage();
                                                    email.From = (rule.aliasRemitente != null ? new MailAddress(rule.correoRemitente, rule.aliasRemitente) : new MailAddress(rule.correoRemitente));
                                                    email.To.Add(r.tbaProspectos.email);
                                                    email.Subject = mailSubject;
                                                    email.Body = mailBody;
                                                    email.IsBodyHtml = true;
                                                    if (cco != null)
                                                        email.Bcc.Add(cco);
                                                    if (replyTo != null)
                                                        email.ReplyToList.Add(replyTo);

                                                    MailMessageResponse x = new MailMessageResponse
                                                    {
                                                        MailMessage = email
                                                    };
                                                    listInstances.Add(x);
                                                    #endregion
                                                }
                                                catch (Exception ex)
                                                {
                                                    errorMessagesByRule += "<br />Rule ID: " + rule.idEnvioAutomatico;
                                                    errorMessagesByRule += "<br />Reservation ID: " + r.idReservacion.ToString();
                                                    errorMessagesByRule += "<br />Signature ID: " + firma.ToString();
                                                    errorMessagesByRule += "<br />Detail: When trying to save changes.<br />Message: " + ex.Message + "<br />Inner Exception: " + ex.InnerException != null ? ex.InnerException.Message != null ? ex.InnerException.Message : "" : "";
                                                }
                                            }
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    errorMessagesByRule += "<br />Rule ID: " + rule.idEnvioAutomatico;
                                    errorMessagesByRule += "<br />Detail: When assigning values.<br />Message: " + ex.Message + "<br />Inner Exception: " + ex.InnerException != null ? ex.InnerException.Message != null ? ex.InnerException.Message : "" : "";
                                }
                                break;
                            }
                        case "purchaseConfReminder"://tbaReservaciones
                            {
                                try
                                {
                                    var firmas = new List<Guid>();
                                    var date = startDay.AddDays(rule.diasDespues);
                                    var reservationStatus = new int?[] { 2, 4, 8, 15, 16, 21, 23 };
                                    var _destinations = Array.ConvertAll(destinations.ToArray(), int.Parse).Cast<int?>().ToArray();
                                    var _resorts = resorts.Count() > 0 ? Array.ConvertAll(resorts.ToArray(), int.Parse).Cast<int?>().ToArray() : new int?[] { };

                                    var query = (from r in dba.tbaReservaciones
                                                 join s in dba.tbaFirmas on r.idReservacion equals s.idReservacion
                                                 where ruleTerminals.Contains(r.tbaProspectos.idTerminal)
                                                 && !reservationStatus.Contains(r.idEstatusReservacion)
                                                 && (_destinations.Count() == 0 || _destinations.Contains(r.idDestino))
                                                 && (_resorts.Count() == 0 || _resorts.Contains(r.idLugar))
                                                 && r.llegada > endDay
                                                 && r.cartaConfirmacionEnviadaPor != null && r.fechaEnvioCartaConfirmacion != null
                                                 && (s.idTipodeDocumento == 4 && s.fechaFirma == null && s.firma == null && ((s.fechaUltimoEnvio == null && s.fechaCreacion <= date) || s.fechaUltimoEnvio == date) && s.urlDocumento.Contains("purchaseConfirmation"))
                                                 select r).ToList();

                                    foreach (var r in query)
                                    {
                                        if (GeneralFunctions.IsEmailValid(r.tbaProspectos.email))
                                        {
                                            var signature = r.tbaFirmas.Where(m => m.idTipodeDocumento == 4 && m.urlDocumento.Contains("purchaseConfirmation")).OrderByDescending(m => m.fechaCreacion).FirstOrDefault();
                                            var firma = signature.firma != null ? (Guid?)null : signature.idFirma;
                                            var mailBody = rule.cuerpoCorreo;
                                            var mailSubject = rule.asuntoCorreo;

                                            if (firma != null && !firmas.Contains((Guid)firma))
                                            {
                                                firmas.Add((Guid)firma);

                                                mailBody = mailBody.Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;firstName&quot; labeled=&quot;false&quot; /]", r.tbaProspectos.firstName)
                                                .Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;lastName&quot; labeled=&quot;false&quot; /]", r.tbaProspectos.lastName)
                                                .Replace("here [link]", "<a href=\"http://eplatform.villagroup.com/signit/?signature=" + firma + "\">here</a>");

                                                try
                                                {
                                                    dba.tbaFirmas.Single(m => m.idFirma == firma).fechaUltimoEnvio = startDay;
                                                    dba.SaveChanges();

                                                    #region "mail instance"
                                                    MailMessage email = new MailMessage();
                                                    email.From = (rule.aliasRemitente != null ? new MailAddress(rule.correoRemitente, rule.aliasRemitente) : new MailAddress(rule.correoRemitente));
                                                    email.To.Add(r.tbaProspectos.email);
                                                    email.Subject = mailSubject;
                                                    email.Body = mailBody;
                                                    email.IsBodyHtml = true;
                                                    if (cco != null)
                                                        email.Bcc.Add(cco);
                                                    if (replyTo != null)
                                                        email.ReplyToList.Add(replyTo);

                                                    MailMessageResponse x = new MailMessageResponse
                                                    {
                                                        MailMessage = email
                                                    };
                                                    listInstances.Add(x);
                                                    #endregion
                                                }
                                                catch (Exception ex)
                                                {
                                                    errorMessagesByRule += "<br />Rule ID: " + rule.idEnvioAutomatico;
                                                    errorMessagesByRule += "<br />Reservation ID: " + r.idReservacion.ToString();
                                                    errorMessagesByRule += "<br />Signature ID: " + firma.ToString();
                                                    errorMessagesByRule += "<br />Detail: When trying to save changes.<br />Message: " + ex.Message + "<br />Inner Exception: " + ex.InnerException != null ? ex.InnerException.Message != null ? ex.InnerException.Message : "" : "";
                                                }
                                            }

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorMessagesByRule += "<br />Rule ID: " + rule.idEnvioAutomatico;
                                    errorMessagesByRule += "<br />Detail: When assigning values.<br />Message: " + ex.Message + "<br />Inner Exception: " + ex.InnerException != null ? ex.InnerException.Message != null ? ex.InnerException.Message : "" : "";
                                }
                                break;
                            }
                        case "preArrivalTafer"://tblReservations
                            {
                                try
                                {
                                    var pdm = new PreArrivalDataModel();
                                    var reservationStatus = new int?[] { 1, 2, 4 };
                                    var _destinations = Array.ConvertAll(destinations.ToArray(), long.Parse).Cast<long?>().ToArray();
                                    var _resorts = resorts.Count() > 0 ? Array.ConvertAll(resorts.ToArray(), long.Parse).Cast<long?>().ToArray() : new long?[] { };
                                    var arrivalDeparture = rule.llegada;
                                    var bookingStatusArray = new int?[] { 16, 5, 6 };//sold, not interested, contacted
                                    var bookingStatusExcluded = new int?[] { 5, 16 };//not interested, sold
                                    var marketCodesExcluded = new string[] { };
                                    var counter = 0;
                                    var counter2 = 0;
                                    var counter1 = 0;
                                    var query = from r in db.tblReservations
                                                join l in db.tblLeads on r.leadID equals l.leadID
                                                where reservationStatus.Contains(r.reservationStatusID)
                                                && !bookingStatusExcluded.Contains(l.bookingStatusID)
                                                && ruleTerminals.Contains(l.terminalID)
                                                && _destinations.Contains(r.destinationID)
                                                && (_resorts.Contains(r.placeID) || _resorts.Count() == 0)
                                                && (rule.llegada == false || (iDate <= r.arrivalDate && r.arrivalDate <= fDate))
                                                && (rule.llegada == true || (iDate <= r.departureDate && r.departureDate <= fDate))
                                                && !r.frontOfficeMarketCode.Contains("bodas extranjeras")
                                                && r.totalNights > 2
                                                select r;

                                    foreach (var rsv in query.GroupBy(m => m.hotelConfirmationNumber))
                                    {
                                        var r = rsv.FirstOrDefault();
                                        if (r.tblLeads.tblLeadEmails.Count(m => m.main && m.dnc != true) > 0 && GeneralFunctions.IsEmailValid(r.tblLeads.tblLeadEmails.FirstOrDefault(m => m.main && m.dnc != true).email))
                                        {
                                            var letters = pdm.GetAvailableLetters(r.reservationID, (int)rule.sysEventID, ruleTerminals.Select(m => (long?)m).ToList());//reminder
                                            if (letters.Count() > 0)
                                            {
                                                var id = r.reservationID.ToString();
                                                var _exists = db.tblFieldsValues.Where(mbox => ruleTerminals.Contains((long)mbox.terminalID));
                                                if (_exists.Count(m => m.value == id && startDay <= m.dateSaved && m.dateSaved <= endDay) == 0)
                                                {
                                                    try
                                                    {
                                                        var letter = letters.FirstOrDefault();
                                                        var preview = pdm.PreviewEmail(r.reservationID, letter.ID);
                                                        preview.ReservationID = r.reservationID.ToString();
                                                        preview.LeadID = r.leadID.ToString();
                                                        counter++;
                                                        var sent = pdm.SendEmail(new JavaScriptSerializer().Serialize(preview));
                                                        if (sent.Type == Attempt_ResponseTypes.Ok)
                                                        {
                                                            try
                                                            {
                                                                var bookingStatus = bookingStatusArray.Contains(r.tblLeads.bookingStatusID) ? r.tblLeads.bookingStatusID : 18;
                                                                var interaction = new PreArrivalInteractionsInfoModel();
                                                                interaction.InteractionsInfo_LeadID = r.leadID;
                                                                interaction.InteractionsInfo_BookingStatus = bookingStatus;
                                                                interaction.InteractionsInfo_InteractionType = 2;
                                                                interaction.InteractionsInfo_InteractionComments = "Email Automatically Sent";
                                                                interaction.InteractionsInfo_SavedByUser = r.tblLeads.assignedToUserID;
                                                                interaction.InteractionsInfo_InteractedWithUser = r.tblLeads.assignedToUserID;
                                                                interaction.InteractionsInfo_ParentInteraction = (long?)null;
                                                                interaction.InteractionsInfo_TotalSold = null;
                                                                var saveInteraction = new InteractionDataModel().SaveInteraction(interaction);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                var m = EmailNotifications.GetSystemEmail(ex.Message + " Inner Exception: " + ex.InnerException ?? "", "efalcon@villagroup.com");
                                                                var l = new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = m } };
                                                                EmailNotifications.Send(l, true);
                                                            }
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        counter--;
                                                    }
                                                }
                                            }
                                            else
                                                counter1++;
                                        }
                                        else
                                            counter2++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorMessagesByRule += "<br />Detail: When assigning values.<br />Message: " + ex.Message + "<br />Inner Exception: " + ex.InnerException != null ? ex.InnerException.Message != null ? ex.InnerException.Message : "" : "";
                                }
                                break;
                            }
                        case "preArrivalVG"://tblReservations terminal 80
                            {
                                try
                                {
                                    var pdm = new PreArrivalDataModel();
                                    var reservationStatus = new int?[] { 1, 2, 4 };
                                    var _destinations = Array.ConvertAll(destinations.ToArray(), long.Parse).Cast<long?>().ToArray();
                                    var _resorts = resorts.Count() > 0 ? Array.ConvertAll(resorts.ToArray(), long.Parse).Cast<long?>().ToArray() : new long?[] { };
                                    var arrivalDeparture = rule.llegada;
                                    var bookingStatusArray = new int?[] { 16, 5, 6 };//sold, not interested, contacted
                                    var bookingStatusExcluded = new int?[] { 5, 16, 15 };//not interested, sold, not contactable
                                    var leadSourcesExcluded = new long?[] { 4 };
                                    var marketCodesExcluded = new string[] { };
                                    var counter = 0;
                                    var counter2 = 0;
                                    var counter1 = 0;
                                    var query = from r in db.tblReservations
                                                join l in db.tblLeads on r.leadID equals l.leadID
                                                where reservationStatus.Contains(r.reservationStatusID)
                                                && !leadSourcesExcluded.Contains(l.leadSourceID)
                                                && !bookingStatusExcluded.Contains(l.bookingStatusID)
                                                && ruleTerminals.Contains(l.terminalID)
                                                && _destinations.Contains(r.destinationID)
                                                && (_resorts.Contains(r.placeID) || _resorts.Count() == 0)
                                                && (rule.llegada == false || (iDate <= r.arrivalDate && r.arrivalDate <= fDate))
                                                && (rule.llegada == true || (iDate <= r.departureDate && r.departureDate <= fDate))
                                                select r;

                                    foreach (var rsv in query.GroupBy(m => m.hotelConfirmationNumber))
                                    {
                                        var r = rsv.FirstOrDefault();
                                        if (r.tblLeads.tblLeadEmails.Count(m => m.main && m.dnc != true) > 0 && GeneralFunctions.IsEmailValid(r.tblLeads.tblLeadEmails.FirstOrDefault(m => m.main && m.dnc != true).email))
                                        {
                                            var letters = pdm.GetAvailableLetters(r.reservationID, (int)rule.sysEventID, ruleTerminals.Select(m => (long?)m).ToList());//reminder
                                            if (letters.Count() > 0)
                                            {
                                                var id = r.reservationID.ToString();

                                                var _exists = db.tblFieldsValues.Where(m => ruleTerminals.Contains((long)m.terminalID));
                                                if (_exists == null || _exists.Count(m => m.value == id && startDay <= m.dateSaved && m.dateSaved <= endDay) == 0)
                                                {
                                                    try
                                                    {
                                                        var letter = letters.FirstOrDefault();
                                                        var preview = pdm.PreviewEmail(r.reservationID, letter.ID);
                                                        preview.ReservationID = r.reservationID.ToString();
                                                        preview.LeadID = r.leadID.ToString();
                                                        counter++;
                                                        var sent = pdm.SendEmail(new JavaScriptSerializer().Serialize(preview));
                                                        if (sent.Type == Attempt_ResponseTypes.Ok)
                                                        {
                                                            try
                                                            {
                                                                var bookingStatus = bookingStatusArray.Contains(r.tblLeads.bookingStatusID) ? r.tblLeads.bookingStatusID : 18;
                                                                var interaction = new PreArrivalInteractionsInfoModel();
                                                                interaction.InteractionsInfo_LeadID = r.leadID;
                                                                interaction.InteractionsInfo_BookingStatus = bookingStatus;
                                                                interaction.InteractionsInfo_InteractionType = 2;
                                                                interaction.InteractionsInfo_InteractionComments = "Email Automatically Sent";
                                                                interaction.InteractionsInfo_SavedByUser = r.tblLeads.assignedToUserID;
                                                                interaction.InteractionsInfo_InteractedWithUser = r.tblLeads.assignedToUserID;
                                                                interaction.InteractionsInfo_ParentInteraction = (long?)null;
                                                                interaction.InteractionsInfo_TotalSold = null;
                                                                var saveInteraction = new InteractionDataModel().SaveInteraction(interaction);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                var m = EmailNotifications.GetSystemEmail(ex.Message + " Inner Exception: " + ex.InnerException ?? "", "efalcon@villagroup.com");
                                                                var l = new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = m } };
                                                                EmailNotifications.Send(l, true);
                                                            }
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        counter--;
                                                    }
                                                }
                                            }
                                            else
                                                counter1++;
                                        }
                                        else
                                            counter2++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorMessagesByRule += "<br />Detail: When assigning values.<br />Message: " + ex.Message + "<br />Inner Exception: " + ex.InnerException != null ? ex.InnerException.Message != null ? ex.InnerException.Message : "" : "";
                                }
                                break;
                            }
                        case "preArrivalRC"://tblReservations terminal 10
                            {
                                try
                                {
                                    var pdm = new PreArrivalDataModel();
                                    var reservationStatus = new int?[] { 1, 2, 4 };
                                    var _destinations = Array.ConvertAll(destinations.ToArray(), long.Parse).Cast<long?>().ToArray();
                                    var _resorts = resorts.Count() > 0 ? Array.ConvertAll(resorts.ToArray(), long.Parse).Cast<long?>().ToArray() : new long?[] { };
                                    var arrivalDeparture = rule.llegada;
                                    var bookingStatusArray = new int?[] { 16, 5, 6 };//sold, not interested, contacted
                                    //var bookingStatusExcluded = new int?[] { 5, 16 };//not interested, sold
                                    //var secondaryBookingStatusExcluded = new int?[] { 1 };//prebooked
                                    var leadStatusExcluded = new int?[] { 4 };
                                    var leadSourcesIncluded = new long?[] { 1, 54, 22, 24, 26, 28 };//clac member, member, trial, premier, mac, elite
                                    var marketCodesExcluded = new string[] { };
                                    var counter = 0;
                                    var counter2 = 0;
                                    var counter1 = 0;
                                    var query = from r in db.tblReservations
                                                join l in db.tblLeads on r.leadID equals l.leadID
                                                where reservationStatus.Contains(r.reservationStatusID)
                                                && !leadStatusExcluded.Contains(l.leadStatusID)
                                                && leadSourcesIncluded.Contains(l.leadSourceID)
                                                && ruleTerminals.Contains(l.terminalID)
                                                && _destinations.Contains(r.destinationID)
                                                && (_resorts.Contains(r.placeID) || _resorts.Count() == 0)
                                                && (rule.llegada == false || (iDate <= r.arrivalDate && r.arrivalDate <= fDate))
                                                && (rule.llegada == true || (iDate <= r.departureDate && r.departureDate <= fDate))
                                                select r;

                                    foreach (var rsv in query.GroupBy(m => m.hotelConfirmationNumber))
                                    {
                                        var r = rsv.FirstOrDefault();
                                        if (r.tblLeads.tblLeadEmails.Count(m => m.main && m.dnc != true) > 0 && GeneralFunctions.IsEmailValid(r.tblLeads.tblLeadEmails.FirstOrDefault(m => m.main && m.dnc != true).email))
                                        {
                                            var letters = pdm.GetAvailableLetters(r.reservationID, (int)rule.sysEventID, ruleTerminals.Select(m => (long?)m).ToList());//reminder
                                            if (letters.Count() > 0)
                                            {
                                                var id = r.reservationID.ToString();

                                                foreach (var letter in letters)
                                                {
                                                    var sent = db.tblEmailNotificationLogs.Where(m => m.emailNotificationID == letter.ID && m.reservationID == r.reservationID);

                                                    if (sent == null || sent.Count() == 0)
                                                    {
                                                        try
                                                        {
                                                            var preview = pdm.PreviewEmail(id, letter.ID);
                                                            //filled in PreViewEmail
                                                            //preview.LeadID = r.leadID.ToString();
                                                            //preview.ReservationID = id;
                                                            counter++;
                                                            var send = pdm.SendEmail(serializer.Serialize(preview));

                                                            if (send.Type == Attempt_ResponseTypes.Ok)
                                                            {
                                                                //email sent, create transaction
                                                                try
                                                                {
                                                                    var bookingStatus = bookingStatusArray.Contains(r.tblLeads.bookingStatusID) ? r.tblLeads.bookingStatusID : 18;
                                                                    var interaction = new PreArrivalInteractionsInfoModel();

                                                                    interaction.InteractionsInfo_LeadID = r.leadID;
                                                                    interaction.InteractionsInfo_BookingStatus = bookingStatus;
                                                                    interaction.InteractionsInfo_InteractionType = 2;
                                                                    interaction.InteractionsInfo_InteractionComments = "Email Automatically Sent";
                                                                    interaction.InteractionsInfo_SavedByUser = r.tblLeads.assignedToUserID;
                                                                    interaction.InteractionsInfo_InteractedWithUser = r.tblLeads.assignedToUserID;
                                                                    interaction.InteractionsInfo_ParentInteraction = (long?)null;
                                                                    interaction.InteractionsInfo_TotalSold = null;
                                                                    var saveInteraction = new InteractionDataModel().SaveInteraction(interaction);
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    var m = EmailNotifications.GetSystemEmail(ex.Message + " Inner Exception: " + ex.InnerException ?? "", "efalcon@villagroup.com");
                                                                    var l = new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = m } };
                                                                    EmailNotifications.Send(l, true);
                                                                }
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            counter--;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                                counter1++;
                                        }
                                        else
                                            counter2++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorMessagesByRule += "<br />Detail: When assigning values.<br />Message: " + (ex.Message ?? "") + "<br />Inner Exception: " + ex.InnerException != null ? ex.InnerException.Message != null ? ex.InnerException.Message : "" : "";
                                }

                                break;
                            }
                    }

                    EmailNotifications.Send(listInstances, false);
                }
            }
        }

        public void _SendEmailReminders(HttpContextBase context = null)
        {
            ePlatEntities db = new ePlatEntities();

            var startOfDay = DateTime.Today;
            var endOfDay = DateTime.Today.AddDays(1).AddSeconds(-1);
            var emailNotifications = db.tblEmailNotifications.Where(m => m.active == true);//Reminder
            var byTerminal = emailNotifications.GroupBy(m => m.terminalID);
            foreach (var notifications in byTerminal)
            {
                var byEvent = notifications.GroupBy(m => m.eventID);

                foreach (var notification in byEvent)
                {
                    switch (notification.Key)
                    {
                        case 45://30 days reminder => se llamará Arrival Reminder
                            {
                                foreach (var n in notification)
                                {
                                    var destinations = n.tblEmailNotifications_Destinations.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_Destinations.Where(m => m.active).Select(m => (long?)m.destinationID).ToArray() : new long?[] { };
                                    var resorts = n.tblEmailNotifications_Places.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_Places.Where(m => m.active).Select(m => (long?)m.placeID).ToArray() : new long?[] { };
                                    var leadSources = n.tblEmailNotifications_LeadSources.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_LeadSources.Where(m => m.active).Select(m => (long?)m.leadSourceID).ToArray() : new long?[] { };
                                    var leadStatuses = n.tblEmailNotifications_LeadStatus.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_LeadStatus.Where(m => m.active).Select(m => (int?)m.leadStatusID).ToArray() : new int?[] { };
                                    var bookingStatuses = n.tblEmailNotifications_BookingStatus.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_BookingStatus.Where(m => m.active).Select(m => (int?)m.bookingStatusID).ToArray() : new int?[] { };
                                    var secondaryBookingStatuses = n.tblEmailNotifications_SecondaryBookingStatus.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_SecondaryBookingStatus.Where(m => m.active).Select(m => (int?)m.secondaryBookingStatusID).ToArray() : new int?[] { };
                                    var copyLead = n.copyLead;
                                    var copySender = n.copySender;
                                    var intervalDays = 30;//n.interval;
                                    var intervalDirectionID = 1;//n.intervalDirectionID;//1:prior;2:after
                                    var intervalTypeID = 1;//n.intervalTypeID;//1:arrivalDate;2:purchaseDate;3:tourDate

                                    var date = intervalDirectionID == 1 ? DateTime.Today.AddDays(intervalDays).Date : DateTime.Today.AddDays((intervalDays * -1)).Date;

                                    var targets = from l in db.tblLeads
                                                  join r in db.tblReservations on l.leadID equals r.leadID
                                                  join e in db.tblLeadEmails on l.leadID equals e.leadID
                                                  join u in db.tblUserProfiles on l.assignedToUserID equals u.userID
                                                  where l.terminalID == notifications.Key
                                                  && (destinations.Contains(r.destinationID) || destinations.Count() == 0)
                                                  && (resorts.Contains(r.placeID) || resorts.Count() == 0)
                                                  && (leadSources.Contains(l.leadSourceID) || leadSources.Count() == 0)
                                                  && (leadStatuses.Contains(l.leadStatusID) || leadStatuses.Count() == 0)
                                                  && (bookingStatuses.Contains(l.bookingStatusID) || bookingStatuses.Count() == 0)
                                                  && (secondaryBookingStatuses.Contains(l.secondaryBookingStatusID) || secondaryBookingStatuses.Count() == 0)
                                                  && ((intervalTypeID == 1 && r.arrivalDate.Value.Date == date))//agregar más casos segun intervalType
                                                  select r;
                                }
                                break;
                            }
                        case 34://Reminder
                            {
                                foreach (var n in notification)
                                {
                                    var destinations = n.tblEmailNotifications_Destinations.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_Destinations.Where(m => m.active).Select(m => (long?)m.destinationID).ToArray() : new long?[] { };
                                    var resorts = n.tblEmailNotifications_Places.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_Places.Where(m => m.active).Select(m => (long?)m.placeID).ToArray() : new long?[] { };
                                    var leadSources = n.tblEmailNotifications_LeadSources.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_LeadSources.Where(m => m.active).Select(m => (long?)m.leadSourceID).ToArray() : new long?[] { };
                                    var leadStatuses = n.tblEmailNotifications_LeadStatus.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_LeadStatus.Where(m => m.active).Select(m => (int?)m.leadStatusID).ToArray() : new int?[] { };
                                    var bookingStatuses = n.tblEmailNotifications_BookingStatus.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_BookingStatus.Where(m => m.active).Select(m => (int?)m.bookingStatusID).ToArray() : new int?[] { };
                                    var secondaryBookingStatuses = n.tblEmailNotifications_SecondaryBookingStatus.Where(m => m.active).Count() > 0 ? n.tblEmailNotifications_SecondaryBookingStatus.Where(m => m.active).Select(m => (int?)m.secondaryBookingStatusID).ToArray() : new int?[] { };
                                    var copyLead = n.copyLead;
                                    var copySender = n.copySender;
                                    var intervalDays = (int)n.days;//n.interval;
                                    var intervalDirectionID = 1;//n.intervalDirectionID;//1:prior;2:after
                                    var intervalTypeID = 1;//n.intervalTypeID;//1:arrivalDate;2:purchaseDate;3:tourDate
                                    //var template = EmailNotifications.GetEmailByNotification(n.emailNotificationID);
                                    var date = intervalDirectionID == 1 ? DateTime.Today.AddDays(intervalDays).Date : DateTime.Today.AddDays((intervalDays * -1)).Date;

                                    var targets = from l in db.tblLeads
                                                  join r in db.tblReservations on l.leadID equals r.leadID
                                                  join e in db.tblLeadEmails on l.leadID equals e.leadID
                                                  join u in db.tblUserProfiles on l.assignedToUserID equals u.userID
                                                  where l.terminalID == notifications.Key
                                                  && (destinations.Contains(r.destinationID) || destinations.Count() == 0)
                                                  && (resorts.Contains(r.placeID) || resorts.Count() == 0)
                                                  && (leadSources.Contains(l.leadSourceID) || leadSources.Count() == 0)
                                                  && (leadStatuses.Contains(l.leadStatusID) || leadStatuses.Count() == 0)
                                                  && (bookingStatuses.Contains(l.bookingStatusID) || bookingStatuses.Count() == 0)
                                                  && (secondaryBookingStatuses.Contains(l.secondaryBookingStatusID) || secondaryBookingStatuses.Count() == 0)
                                                  && ((intervalTypeID == 1 && r.arrivalDate.Value.Date == date))//agregar más casos segun intervalType
                                                  //validar campos requeridos
                                                  //validar puntos de venta
                                                  select r;

                                    var groupedByConfirmation = targets.GroupBy(m => m.hotelConfirmationNumber);
                                    foreach (var group in groupedByConfirmation)
                                    {
                                        var item = group.FirstOrDefault();
                                        var targetAddress = item.tblLeads.tblLeadEmails.Count(m => m.main && m.dnc != true) > 0 ? item.tblLeads.tblLeadEmails.FirstOrDefault(m => m.main && m.dnc != true) : null;

                                        //pendiente preguntar si haremos logsYYYYMM para esta tabla
                                        var alreadySent = db.tblEmailNotificationLogs.Count(m => m.emailNotificationID == n.emailNotificationID && m.reservationID == item.reservationID && startOfDay <= m.dateSent && m.dateSent <= endOfDay) > 0;
                                        //pendiente revisar en tblFieldsValues alreadySent
                                        if (!alreadySent)
                                        {
                                            if (targetAddress != null && GeneralFunctions.IsEmailValid(targetAddress.email))
                                            {
                                                var instance = PreviewEmail(item.reservationID, n.emailNotificationID);

                                            }
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }

            }
        }

        public EmailPreViewModel PreviewEmail(Guid id, int notificationID)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            return pdm.PreviewEmail(id, notificationID);
        }

        public AttemptResponse SendEmail(string model, int notificationID, HttpContextBase context = null)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            EmailPreViewModel instance = serializer.Deserialize<EmailPreViewModel>(model);
            System.Net.Mail.MailMessage email = EmailNotifications.GetEmailByNotification(notificationID);

            Guid transactionID = Guid.NewGuid();

            if (instance.FromAddress != null && instance.FromAddress != "")
            {
                email.From = new System.Net.Mail.MailAddress(instance.FromAddress, instance.FromAlias);
            }
            email.ReplyToList.Add(instance.ReplyTo);
            email.Subject = instance.Subject;
            if (instance.CC != null && instance.CC != "")
            {
                var cc = (email.CC.Count() > 0 ? string.Join(",", email.CC) : "") + (instance.CC != "" ? "," + instance.CC.Replace(" ", "") : "");
                var distinct = string.Join(",", cc.Split(',').Distinct());
                email.CC.Clear();
                email.CC.Add(distinct);
            }
            if (instance.BCC != null && instance.BCC != "")
            {
                var bcc = (email.Bcc.Count() > 0 ? string.Join(",", email.Bcc) : "") + (instance.BCC != "" ? "," + instance.BCC.Replace(" ", "") : "");
                var distinct = string.Join(",", bcc.Split(',').Distinct());
                email.Bcc.Clear();
                email.Bcc.Add(distinct);
            }
            //pendiente averiguar como agregar link de paymentConfirmation que no sea aquí, para que este metodo sea de uso general

            email.Body = InsertTracker(email.Body, transactionID.ToString());

            #region "tblEmailNotificationLogs save"
            var log = new tblEmailNotificationLogs();
            log.emailNotificationID = notificationID;
            log.dateSent = DateTime.Now;
            log.sentByUserID = HttpContext.Current.Request.IsAuthenticated ? new UserSession().UserID : db.aspnet_Users.Count(m => m.UserName == instance.FromAddress) > 0 ? db.aspnet_Users.Single(m => m.UserName == instance.FromAddress).UserId : Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");
            log.reservationID = instance.ReservationID != null ? Guid.Parse(instance.ReservationID) : (Guid?)null;
            log.leadID = instance.LeadID != null ? Guid.Parse(instance.LeadID) : (Guid?)null;
            log.trackingID = transactionID;
            log.emailPreviewJson = serializer.Serialize(instance);
            log.subject = instance.Subject;
            //tourID
            //leadslist_emailnotificacionID
            db.tblEmailNotificationLogs.AddObject(log);
            db.SaveChanges();
            #endregion

            #region "tblFieldsValues save"
            if (instance.ListFieldsValues.Count() > 0)
            {
                var fieldGroupID = int.Parse(instance.FieldGroup);
                var fieldGroup = db.tblFieldGroups.Single(m => m.fieldGroupID == fieldGroupID);
                var fields = fieldGroup.tblFields;
                var list = new List<SurveyViewModel.FieldValue>();
                SurveyViewModel.SurveyValuesModel survey = new SurveyViewModel.SurveyValuesModel();
                foreach (var field in instance.ListFieldsValues)
                {
                    list.Add(new SurveyViewModel.FieldValue()
                    {
                        FieldID = (int)fields.FirstOrDefault(m => m.field == field.Key).fieldID,
                        Value = field.Value
                    });
                }
                survey.SurveyID = (Guid)fieldGroup.fieldGroupGUID;
                survey.TransactionID = transactionID;
                survey.Fields = list;

                response = new NotificationsDataModel().SaveFieldValues(survey, context, email);
            }
            #endregion

            if (response.Type != Attempt_ResponseTypes.Error)
            {
                var sent = EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                if (!sent.FirstOrDefault().Sent.Value)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Email NOT Sent. Please report to System Administrator.<br />Key: " + transactionID;
                }
            }

            return response;
        }

        public static string InsertTracker(string body, string transactionID)
        {
            var anchors = Regex.Matches(body, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

            foreach (Match match in anchors)
            {
                var original = match.Groups[1].Value;
                var copy = match.Groups[1].Value;
                Match i = Regex.Match(copy, @"href=\""(.*?)\""", RegexOptions.Singleline);
                if (i.Success)
                {
                    if (i.Groups[1].Value.IndexOf("mailto") == -1 && i.Groups[1].Value.IndexOf("phone") == -1)
                    {
                        copy = copy.Replace("href=\"", "href=\"https://eplat.villagroup.com/Public/OpenLink/" + transactionID + "?url=");
                        body = body.Replace(original, copy);
                    }
                }
            }

            body += "<img src=\"https://eplat.villagroup.com/Public/GetImage/" + transactionID + "\" style=\"position:absolute;z-index:-10;opacity:0;\" />";

            return body;
        }

        public AttemptResponse CheckForBounces(string address = null, string pass = null, string server = null, int days = 0)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            var listIds = "";
            var today = DateTime.Today;
            var listEmails = new List<System.Net.Mail.MailMessage>();
            IQueryable<tblMailingSettings> query;

            if (address != null)
            {
                query = new tblMailingSettings()
                {
                    smtpServer = "smtp.villagroup.com",
                    smtpPort = "2525",
                    smtpUsername = address,
                    smtpPassword = pass,
                    smtpSsl = false,
                    popServer = server,
                    incomingPort = "110",
                    popUsername = address,
                    popPassword = pass,
                    popSsl = false
                } as IQueryable<tblMailingSettings>;
            }
            else
            {
                //query = db.tblMailingSettings.Where(m => m.active == true && m.senderName.IndexOf("reply") != -1);
                query = db.tblMailingSettings.Where(m => m.active == true && m.checkBounces == true);
            }

            //var query = db.tblMailingSettings.Where(m => m.active == true && m.senderName.IndexOf("reply") != -1);



            foreach (var i in query)
            {
                using (OpenPop.Pop3.Pop3Client client = new OpenPop.Pop3.Pop3Client())
                {
                    //connection
                    client.Connect(i.popServer, int.Parse(i.incomingPort), i.popSsl);
                    //authentication
                    client.Authenticate(i.popUsername, i.popPassword);
                    //client.Authenticate("efalcon@villagroup.com", "Miguel3423");

                    //messagesCount
                    var counter = client.GetMessageCount();
                    var daysBefore = days != 0 ? (days * -1) : -3;
                    while (counter > (counter - 10) && client.GetMessageHeaders(counter).DateSent > today.AddDays(daysBefore))
                    {
                        var headers = client.GetMessageHeaders(counter);
                        if (headers.Subject.Contains("Undeliver") || headers.Subject.Contains("failure"))
                        {
                            var msg = client.GetMessage(counter);
                            var cosa = msg.ToMailMessage();
                            listEmails.Add(cosa);
                            //                 var exp = @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                            //+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                            //  + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                            //+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";

                            //                 Regex regex = new Regex(exp, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            //                 MatchCollection matches = regex.Matches(cosa.Body);
                            //                 foreach (Match x in matches)
                            //                 {
                            //                     var aa = x.Success;
                            //                     var _address = x.Groups[0].Value;
                            //                     var leads = db.tblLeadEmails.Where(m => m.email == _address);

                            //                     foreach (var t in leads)
                            //                     {
                            //                         listIds += (listIds == "" ? "" : ",") + t.emailID;//t.email;
                            //                         t.bounced = true;
                            //                     }
                            //                 }
                            //db.SaveChanges();
                        }
                        counter--;
                    }
                }

                foreach (var email in listEmails)
                {
                    var exp = @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
               + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                 + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
               + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";

                    Regex regex = new Regex(exp, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    MatchCollection matches = regex.Matches(email.Body);
                    foreach (Match x in matches)
                    {
                        var aa = x.Success;
                        var _address = x.Groups[0].Value;

                        if (db.tblLeadEmails.Count(m => m.email == _address) > 0)
                        {
                            db.tblLeadEmails.First(m => m.email == _address).bounced = true;
                        }
                    }
                }
                db.SaveChanges();
            }

            return response;
        }

        public AttemptResponse ResetEmailsCounters()
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblMailingSettings;

                foreach (var i in query)
                {
                    i.dailyCounter = 0;
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Counters Succesfully Reset";
                return response;
            }
            catch
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Counters Not Reset";
                return response;
            }
        }

        public void OpenUrl(string id)
        {

        }

        public static List<ResortConnectOptionals> GetResortConnectOptionals(tblReservations reservation)
        {
            resortConnectEntities dba = new resortConnectEntities();
            string hc = null;
            string cn = null;
            var options = new List<ResortConnectOptionals>();

            if (reservation.hotelConfirmationNumber != null)
            {
                hc = reservation.hotelConfirmationNumber.Trim();
            }
            if (reservation.frontOfficeCertificateNumber != null)
            {
                cn = reservation.frontOfficeCertificateNumber.Trim();
            }
            if (cn != null && cn != "")
            {
                options = (from o in dba.ReservationOption
                           join r in dba.Reservation on o.ReservationId equals r.ReservationId
                           where r.ConfirmationNumber == cn
                           && o.ProductName.StartsWith("PA")
                           select new ResortConnectOptionals
                           {
                               ProductName = o.ProductName,
                               Quantity = o.Quantity,
                               BaseCurrencyAmount = o.BaseCurrencyAmount,
                               Note = o.Note,
                               Date = o.AppointmentDate,
                               CarrierName = o.CarrierName,
                               FlightNumber = o.FlightNumber
                           }).ToList();
            }
            if (options.Count() == 0 && hc != null && hc != "")
            {
                options = (from o in dba.ReservationOption
                           join r in dba.Reservation on o.ReservationId equals r.ReservationId
                           where r.ConfirmationNumber == hc
                           && o.ProductName.StartsWith("PA")
                           select new ResortConnectOptionals
                           {
                               ProductName = o.ProductName,
                               Quantity = o.Quantity,
                               BaseCurrencyAmount = o.BaseCurrencyAmount,
                               Note = o.Note,
                               Date = o.AppointmentDate,
                               CarrierName = o.CarrierName,
                               FlightNumber = o.FlightNumber
                           }).ToList();
            }

            return options;
        }

        public static List<ResortConnectOptionals> GetResortConnectOptionals(List<tblReservations> reservations)
        {
            resortConnectEntities dba = new resortConnectEntities();
            List<string> hc = new List<string>();
            List<string> cn = new List<string>();
            foreach (var i in reservations)
            {
                if (i.hotelConfirmationNumber != null)
                {
                    hc.Add(i.hotelConfirmationNumber.Trim());
                }
                if (i.frontOfficeCertificateNumber != null)
                {
                    cn.Add(i.frontOfficeCertificateNumber.Trim());
                }
            }
            List<ResortConnectOptionals> options = new List<ResortConnectOptionals>();

            options = (from r in dba.Reservation
                       join o in dba.ReservationOption on r.ReservationId equals o.ReservationId
                       where (hc.Contains(r.ConfirmationNumber) || cn.Contains(r.ConfirmationNumber))
                       && o.ProductName.StartsWith("PA")
                       select new ResortConnectOptionals
                       {
                           ConfirmationNumber = r.ConfirmationNumber != null ? r.ConfirmationNumber.ToLower() : null,
                           ProductName = o.ProductName,
                           Quantity = o.Quantity,
                           BaseCurrencyAmount = o.BaseCurrencyAmount,
                           Note = o.Note,
                           Date = o.AppointmentDate,
                           CarrierName = o.CarrierName,
                           FlightNumber = o.FlightNumber,
                           ResortID = r.ResortNumber,
                           LastUpdated = o.LastUpdated
                       }).ToList();

            return options.Where(m => m.ConfirmationNumber != null).ToList();

        }

        public static List<ResortConnectOptionals> GetResortConnectOptionals(List<int?> resorts, DateTime purchaseIDate, DateTime purchaseFDate)
        {
            resortConnectEntities db = new resortConnectEntities();

            var options = (from r in db.Reservation
                           join o in db.ReservationOption on r.ReservationId equals o.ReservationId
                           where resorts.Contains(r.ResortNumber)
                           && purchaseIDate <= o.LastUpdated && o.LastUpdated <= purchaseFDate
                           && o.ProductName.StartsWith("PA")
                           select new ResortConnectOptionals
                           {
                               ConfirmationNumber = r.ConfirmationNumber != null ? r.ConfirmationNumber.ToLower() : null,
                               ProductName = o.ProductName,
                               Quantity = o.Quantity,
                               BaseCurrencyAmount = o.BaseCurrencyAmount,
                               Note = o.Note,
                               Date = o.AppointmentDate,
                               CarrierName = o.CarrierName,
                               FlightNumber = o.FlightNumber,
                               ResortID = r.ResortNumber,
                               LastUpdated = o.LastUpdated
                           }).ToList();

            return options.Where(m => m.ConfirmationNumber != null).ToList();
        }

        /// <summary>
        /// import clac leads from file to eplat
        /// </summary>
        public AttemptResponse Import(string tag)
        {
            AttemptResponse attempt = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            long[] terminals = new UserSession().Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            string path = HttpContext.Current.Server.MapPath(@"~/Content/files/data/") + Path.GetFileName("clac.csv");
            var pTypes = db.tblPhoneTypes;
            var places = db.tblPlaces.Where(m => m.resortConnectResortID != null).Select(m => new { m.resortConnectResortID, m.frontOfficeResortID });
            var counter = 0;
            //try
            {
                List<ImportModel> arrivalsToImport = new List<ImportModel>();
                PreArrivalDataModel pdm = new PreArrivalDataModel();
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string[] lines = File.ReadAllLines(path);
                lines = lines[0].Split(';')[0].Any(char.IsDigit) ? lines : lines.Skip(1).ToArray();

                foreach (string line in lines)
                {
                    //if (counter > 9) break;
                    if (!string.IsNullOrEmpty(line))
                    {
                        var cols = line.Split(';');
                        string memberNumber = cols[0];
                        string pType = cols[1];
                        string phone = cols[2];
                        string email = cols[3];
                        string aDate = cols[4];
                        int place = cols[5] != null ? int.Parse(cols[5]) : 0;
                        var count = db.tblMemberContactInfo.Count(m => m.resortConnectResortID == place && m.memberNumber == memberNumber);

                        #region "save contact info"
                        var cInfo = new tblMemberContactInfo();
                        if (count == 0)
                        {
                            //var cInfo = new tblMemberContactInfo();
                            cInfo.memberNumber = memberNumber;
                            cInfo.phoneType = pType;
                            cInfo.phone = phone != null && phone != "" ? mexHash.mexHash.EncryptString(phone) : "";
                            cInfo.email = email != null && email != "" ? mexHash.mexHash.EncryptString(email) : "";
                            cInfo.checkInDate = DateTime.Parse(aDate.Substring(0, 4) + "-" + aDate.Substring(4, 2) + "-" + aDate.Substring(6, 2));
                            cInfo.resortConnectResortID = place;
                        }
                        #endregion

                        IQueryable<tblLeads> query = from m in db.tblMemberInfo
                                                     join l in db.tblLeads on m.leadID equals l.leadID
                                                     where terminals.Contains(l.terminalID)
                                                     && m.memberNumber.Contains(memberNumber)
                                                     select l;

                        if (query.Count() == 0)
                        {
                            SearchToImportModel model = new SearchToImportModel();
                            var placeID = place;
                            model.Search_I_ImportArrivalDate = aDate.Substring(0, 4) + "-" + aDate.Substring(4, 2) + "-" + aDate.Substring(6, 2);
                            model.Search_F_ImportArrivalDate = aDate.Substring(0, 4) + "-" + aDate.Substring(4, 2) + "-" + aDate.Substring(6, 2);
                            model.SearchToImport_ImportResort = new int[] { (place != null && place != 0 ? (int)places.FirstOrDefault(m => m.resortConnectResortID == placeID).frontOfficeResortID : 0) };

                            var arrivals = pdm.GetArrivalsToImport(model);
                            if (arrivals.Count() > 0)
                            {
                                arrivals = arrivals.Where(m => m.Contrato == memberNumber && m.Status != true).ToList();
                                foreach (var i in arrivals)
                                {
                                    i.Tags = tag;
                                }
                                arrivalsToImport.AddRange(arrivals);
                            }
                        }
                        else
                        {
                            cInfo.leadID = query.FirstOrDefault().leadID;
                            query.FirstOrDefault().tags = tag;
                        }

                        if (count == 0)
                        {
                            db.tblMemberContactInfo.AddObject(cInfo);
                            db.SaveChanges();
                        }
                        counter++;
                    }
                }

                #region "import"
                var response = pdm.ImportArrivals(serializer.Serialize(arrivalsToImport));
                var imported = serializer.Deserialize<List<ImportModel>>(response.ObjectID.GetType().GetProperty("arrivals").GetValue(response.ObjectID, null).ToString());

                foreach (var i in imported)
                {
                    var placeID = places.FirstOrDefault(m => m.frontOfficeResortID == i.idresort).resortConnectResortID;
                    string contract = i.Contrato.Trim();
                    var cInfo = db.tblMemberContactInfo.FirstOrDefault(m => m.resortConnectResortID == placeID && m.memberNumber == i.Contrato);
                    cInfo.leadID = i.LeadID;
                    db.SaveChanges();
                }
                #endregion
                attempt.Message = "Imported";
            }
            //catch (Exception ex)
            {
                // attempt.Message = "Not Imported<br />" + (ex.Message ?? "") + ex.InnerException != null ? ex.InnerException.Message ?? "" : "";
            }
            return attempt;
        }

        public static object UpdateOptions(int fromm, int to)
        {
            ePlatEntities db = new ePlatEntities();

            var query = from o in db.tblOptions
                        join ot in db.tblOptionTypes on o.optionTypeID equals ot.optionTypeID
                        join op in db.tblOptions_Places on o.optionID equals op.optionID
                        where o.optionTypeID == fromm && ot.terminalID == 10
                        orderby o.optionID
                        select new { o.optionID, o.optionName, op.placeID };
            var ccounter = 0;
            foreach (var i in query)
            {
                var counter = 0;
                var qquery = from o in db.tblOptions
                             join ot in db.tblOptionTypes on o.optionTypeID equals ot.optionTypeID
                             where ot.optionTypeID == to && o.optionName == i.optionName && ot.terminalID == 80
                             select o.optionID;

                foreach (var a in qquery)
                {
                    counter++;
                    var q = new tblOptions_Places();
                    q.optionID = a;
                    q.placeID = i.placeID;
                    db.tblOptions_Places.AddObject(q);
                }
                ccounter++;
            }
            db.SaveChanges();
            return new { ccounter };
        }

        public static bool UpdatePhones()
        {
            ePlatEntities db = new ePlatEntities();
            try
            {
                var query = from p in db.tblPhones
                            where p.phone.IndexOf("(") != -1
                            || p.phone.IndexOf(")") != -1
                            || p.phone.IndexOf("-") != -1
                            select p;
                var c = query.Count();
                foreach (var p in query)
                {
                    var phone = Regex.Replace(p.phone, @"[^\d]", "");
                    if (phone.Length >= 10)
                    {
                        p.phone = phone.Substring(phone.Length - 10);
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public static bool NormalizeReservations(int? resort, string date, string confirmation)
        {
            try
            {
                ePlatEntities db = new ePlatEntities();

                var fromDate = DateTime.Parse("2020-05-31 23:59:59");
                var _fromDate = date != null ? DateTime.Parse(date) : (DateTime?)null;
                var _toDate = date != null ? DateTime.Parse(date).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                var resorts = db.tblPlaces_Terminals.Where(m => m.terminalID == 10 && m.tblPlaces.frontOfficeResortID != null).Select(m => m.tblPlaces.frontOfficeResortID).ToArray();

                var query = from l in db.tblLeads
                            join r in db.tblReservations on l.leadID equals r.leadID
                            where l.terminalID == 10
                            //&& (resort == (int?)null || l.frontOfficeResortID == resort)
                            && ((resort == (int?)null && resorts.Contains(l.frontOfficeResortID)) || l.frontOfficeResortID == resort)
                            && ((_fromDate == null && r.arrivalDate > fromDate) || (_fromDate <= r.arrivalDate && r.arrivalDate <= _toDate))
                            && (confirmation == null || r.hotelConfirmationNumber == confirmation)
                            && r.tempConfirmationNumber != null && r.frontOfficeReservationID == null
                            select l;

                var a = query.Count();
                var aCounter = 0;
                var bCounter = 0;
                var cCounter = 0;
                foreach (var i in query)
                {
                    try
                    {
                        var r = i.tblReservations.OrderByDescending(m => m.arrivalDate).FirstOrDefault();
                        var front = FrontOfficeDataModel.GetArrivals(i.frontOfficeResortID.Value, r.arrivalDate.Value, r.arrivalDate.Value);
                        if (front != null && front.Count() > 0)
                        {
                            var coincidence = front.Where(m => r.frontOfficeCertificateNumber == m.CRS).FirstOrDefault();
                            if (coincidence != null)
                            {
                                //i.frontOfficeGuestID = (int?)coincidence.idhuesped;
                                foreach (var rsv in i.tblReservations.Where(m => m.arrivalDate == r.arrivalDate))
                                {
                                    //rsv.hotelConfirmationNumber = coincidence.numconfirmacion;
                                    if (rsv.frontOfficeReservationID == null)
                                    {
                                        rsv.frontOfficeReservationID = (long?)coincidence.idReservacion;
                                    }
                                }
                                aCounter++;
                            }
                            else { cCounter++; }
                        }
                    }
                    catch (Exception ex) { bCounter++; }
                }
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool test(string _fromDate, string _toDate)
        {
            ePlatEntities db = new ePlatEntities();
            IQueryable<tblReservations> query;
            var fromDate = DateTime.Parse(_fromDate, CultureInfo.InvariantCulture);
            var toDate = _toDate != "null" ? DateTime.Parse(_toDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : DateTime.MaxValue;
            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                query = from l in db.tblLeads
                        join r in db.tblReservations on l.leadID equals r.leadID
                        join o in db.tblOptionsSold on r.reservationID equals o.reservationID into r_o
                        from o in r_o.DefaultIfEmpty()
                        where l.terminalID == 10
                        && l.isTest != true && r.isTest != true
                        && r.arrivalDate >= fromDate
                        && r.arrivalDate <= toDate
                        && r.frontOfficeRoomListID != null
                        select r;
                scope.Complete();
            }
            var cosa = query.Count();
            var cosaa = query.GroupBy(m => m.hotelConfirmationNumber).Count();
            List<tblReservations> reservations = new List<tblReservations>();
            foreach (var i in query.GroupBy(m => m.hotelConfirmationNumber))
            {

            }

            return true;
        }

        public static bool _test()
        {
            ePlatEntities db = new ePlatEntities();
            IQueryable<tblLeads> query;
            var fromDate = DateTime.Parse("2019-01-01 00:00:00", CultureInfo.InvariantCulture);
            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                query = from l in db.tblLeads
                        join rsv in db.tblReservations on l.leadID equals rsv.leadID
                        where l.terminalID == 10 && l.bookingStatusID == 1
                        && rsv.arrivalDate > fromDate
                        select l;
                scope.Complete();
            }

            foreach (var i in query)
            {
                if (i.secondaryBookingStatusID == null)
                {
                    i.secondaryBookingStatusID = i.bookingStatusID;
                }
            }

            db.SaveChanges();
            return true;
        }

        public static AttemptResponse RedeemCredits(RedeemCredits model)
        {
            AttemptResponse response = new AttemptResponse();
            string UserName = HttpContext.Current.User.Identity.Name;

            foreach (var redeem in model.ShoppingCart)
            {
                try
                {
                    var token = Utils.Token.GetToken();

                    RestClient Client = new RestClient($"{GlobalVars.Token.ServerURL}/api/ReferralsRewardsRedemption/")
                    {
                        Timeout = -1
                    };

                    RestRequest PostRedemption = new RestRequest(Method.POST);
                    PostRedemption.AddHeader("Authorization", $"{token.Token_type} {token.Access_token}")
                        .AddParameter("MemberAccount", model.Account)
                        .AddParameter("TransactionTypeID", redeem.TransactionTypeID)
                        .AddParameter("Concept", redeem.Title)
                        .AddParameter("Amount", redeem.CreditsApplied)
                        //.AddParameter("Reference", redeem.CreditsApplied)
                        .AddParameter("ResortID", model.ResortID);
                    //.AddParameter("RegionID", 1);
                    IRestResponse ResponsePoints = Client.Execute(PostRedemption);
                    RedemptionResponse redemptionResponse = JsonConvert.DeserializeObject<RedemptionResponse>(ResponsePoints.Content.ToString());

                    switch (redemptionResponse.Status)
                    {
                        case (ViewModels.RedemptionStatus)RedemptionStatus.Ok:
                            {
                                response.Type = Attempt_ResponseTypes.Ok;
                                response.Message = "Credits Redeemed!";
                                response.ObjectID = redemptionResponse.AuthorizationCode;
                                break;
                            }
                        case (ViewModels.RedemptionStatus)RedemptionStatus.NotEnoughCredits:
                            {
                                response.Message = "Not Enough Credits";
                                response.Type = Attempt_ResponseTypes.Error;
                                break;
                            }
                        default:
                            {
                                response.Message = "Unexpected Error";
                                response.Type = Attempt_ResponseTypes.Error;
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Exception = ex;
                    return response;
                }
            }
            return response;
        }

        public static RedemptionResponse GetAvailableAccountCredits(string memberAccount)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var token = Token.GetToken();
            var client = new RestClient("https://developers.eplat.com/");
            var request = new RestRequest("api/ReferralsRewardsRedemption", Method.GET);
            request.AddParameter("memberAccount", memberAccount);
            request.AddHeader("Authorization", $"{token.Token_type} {token.Access_token}");

            var response = client.Execute(request);
            var responseText = serializer.Deserialize<RedemptionResponse>(response.Content);
            return responseText;
        }

        public static List<BillingResultsModel> DecryptCard(string lead)
        {
            ePlatEntities db = new ePlatEntities();
            var leadID = Guid.Parse(lead);
            var query = db.tblBillingInfo.Where(m => m.leadID == leadID).ToList();
            var list = new List<BillingResultsModel>();

            foreach (var i in query)
            {
                list.Add(new BillingResultsModel()
                {
                    BillingInfo_BillingInfoID = i.billingInfoID.ToString(),
                    BillingInfo_CardHolderName = i.cardHolderName,
                    BillingInfo_CardNumber = mexHash.mexHash.DecryptString(i.cardNumber).ToString(),
                    BillingInfo_CardCVV = i.cardCVV,
                    BillingInfo_CardType = i.cardTypeID.ToString() + "-" + i.tblCardTypes.cardType,
                    BillingInfo_CardExpiry = i.cardExpiry
                });
            }

            return list;
        }
    }
}
