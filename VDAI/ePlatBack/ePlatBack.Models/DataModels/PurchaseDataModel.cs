using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net;

namespace ePlatBack.Models.DataModels
{
    public class PurchaseDataModel
    {
        public static bool ProcessResponse(ProsaResponseModel response)
        {
            bool valid = true;
            ePlatEntities db = new ePlatEntities();
            string newDigest = Utils.GeneralFunctions.GetSHA1(response.EM_Total + response.EM_OrderID + response.EM_Merchant + response.EM_Store + response.EM_Term + response.EM_RefNum + "-" + response.EM_Auth);


            //es válido, guardar la aprobación
            //Guid purchaseid = Guid.ParseExact(response.EM_OrderIDX, "N");
            Guid purchaseid = new Guid(response.EM_OrderIDX);
            var purchase = (from p in db.tblPurchases
                            where p.purchaseID == purchaseid
                            select p).FirstOrDefault();

            //guardar paymentDetails
            tblPaymentDetails newPaymentDetails = new tblPaymentDetails();
            newPaymentDetails.amount = decimal.Parse(response.EM_Total) / 100;
            newPaymentDetails.currencyID = 2;
            newPaymentDetails.chargeDescriptionID = 1;
            newPaymentDetails.chargeTypeID = 1;
            newPaymentDetails.dateSaved = DateTime.Now;
            newPaymentDetails.paymentType = 2;
            //guardar transacción y billing info
            tblMoneyTransactions newMoneyTransaction = new tblMoneyTransactions();
            newMoneyTransaction.terminalID = purchase.terminalID;
            newMoneyTransaction.authCode = response.EM_Auth;
            newMoneyTransaction.authAmount = int.Parse(response.EM_Total) / 100;
            newMoneyTransaction.authDate = DateTime.Now;
            newMoneyTransaction.errorCode = (response.EM_Response == "approved" ? "0" : "-1");
            newMoneyTransaction.transactionDate = DateTime.Now;
            newMoneyTransaction.transactionTypeID = 1;
            int merchantIDInt = int.Parse(response.EM_Merchant);
            newMoneyTransaction.merchantAccountID = db.tblMerchantAccounts.FirstOrDefault(x => x.merchantID == merchantIDInt).merchantAccountID;
            newMoneyTransaction.reference = response.EM_RefNum + " " + response.EM_Response;
            newMoneyTransaction.madeByEplat = true;

            newPaymentDetails.tblMoneyTransactions = newMoneyTransaction;

            purchase.tblPaymentDetails.Add(newPaymentDetails);
            db.SaveChanges();

            //notificar por correo
            if (response.EM_Response == "approved" && response.EM_Digest == newDigest)
            {
                //confirmar por correo
                System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(1);
                email.To.Add(purchase.tblLeads.tblLeadEmails.FirstOrDefault().email);
                email.Body = email.Body
                    .Replace("$Purchase_FirstName", purchase.tblLeads.firstName)
                    .Replace("$Purchase_AuthCode", (newPaymentDetails.moneyTransactionID != null ? newPaymentDetails.tblMoneyTransactions.authCode : ""))
                    .Replace("$Purchase_Total", Decimal.Round((decimal)purchase.total, 2, MidpointRounding.AwayFromZero).ToString())
                    .Replace("$Purchase_Terms", PurchaseDataModel.GetTerms());


                //get cart items and replace on email body
                string cartItems = "";
                foreach (tblPurchases_Services item in purchase.tblPurchases_Services.OrderBy(x => x.dateSaved))
                {
                    if (item.tblServices.itemTypeID == 2 || item.tblServices.itemTypeID == 3) //services & transportation
                    {
                        cartItems += "<p>";
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Date + ": <strong>" + item.serviceDateTime.ToString("yyyy-MM-dd") + "</strong><br>";
                        ActivityDataModel activity = new ActivityDataModel();
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Service_Time + ": <strong>" + item.serviceDateTime.ToString("hh:mm:ss tt") + "</strong><br>";
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Service + ": <strong>" + item.tblServices.tblServiceDescriptions.FirstOrDefault(x => x.culture == "es-MX").service + "</strong><br>";
                        string units = "";
                        foreach (tblPurchaseServiceDetails detail in item.tblPurchaseServiceDetails)
                        {
                            if (units != "")
                            {
                                units += ", ";
                            }
                            units += detail.quantity + " " + detail.tblPrices1.tblPriceUnits.FirstOrDefault(c => c.culture == "es-MX");
                        }
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Good_for + ": <strong>" + units + "</strong><br>";
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Meeting_Time + ": <strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.To_be_confirmed + ".</strong>";
                        cartItems += "</p>";
                        if (item.tblServices.itemTypeID == 3 && purchase.terminalID == 6)
                        {
                            cartItems += "<p><strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Important + "</strong><br />" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Transportation_Note + "</p>";
                        }
                    }
                    else if (item.tblServices.itemTypeID == 5) //car rental
                    {
                        cartItems += "<p>";
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Date + ": <strong>" + item.serviceDateTime.ToString("yyyy-MM-dd") + "</strong><br>";
                        ActivityDataModel activity = new ActivityDataModel();
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Delivery_Time + ": <strong>" + item.serviceDateTime.ToString("hh:mm:ss tt") + "</strong><br>";
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Solicited_Vehicle + ": <strong>" + item.tblServices.tblServiceDescriptions.FirstOrDefault(x => x.culture == "es-MX").service + "</strong><br>";
                        string units = "";
                        foreach (tblPurchaseServiceDetails detail in item.tblPurchaseServiceDetails)
                        {
                            if (units != "")
                            {
                                units += ", ";
                            }
                            units += detail.quantity + " " + detail.tblPrices1.tblPriceUnits.FirstOrDefault(c => c.culture == "es-MX");
                        }
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Good_for + ": <strong>" + units + "</strong><br>";
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Delivery_Location + ": <strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.To_be_confirmed + ".</strong>";
                        cartItems += "</p>";
                    }
                    else
                    {
                        cartItems += "<p>";
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Date + ": <strong>" + item.serviceDateTime.ToString("yyyy-MM-dd") + "</strong><br>";
                        ActivityDataModel activity = new ActivityDataModel();
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Activity_Time + ": <strong>" + item.serviceDateTime.ToString("hh:mm:ss tt") + "</strong><br>";
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Activity_Name + ": <strong>" + item.tblServices.tblServiceDescriptions.FirstOrDefault(x => x.culture == "es-MX").service + "</strong><br>";
                        string units = "";
                        foreach (tblPurchaseServiceDetails detail in item.tblPurchaseServiceDetails)
                        {
                            if (units != "")
                            {
                                units += ", ";
                            }
                            units += detail.quantity + " " + detail.tblPrices1.tblPriceUnits.FirstOrDefault(c => c.culture == "es-MX");
                        }
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Good_for + ": <strong>" + units + "</strong><br>";
                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Meeting_Time + ": <strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.To_be_confirmed + ".</strong>";
                        cartItems += "</p>";
                    }

                }
                email.Body = email.Body
                    .Replace("$Purchase_CartItems", cartItems);

                //Utils.EmailNotifications.Send(email);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
            }
            else
            {
                valid = false;
                //enviar correo informando el error
                //Console.WriteLine("{0} Exception caught.", e);
                System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                email = new System.Net.Mail.MailMessage();
                email.From = new System.Net.Mail.MailAddress("info@myvallartaexperience.com", "ePlatFront");
                email.Subject = "Error en Proceso de Pago";
                email.Body = "<span style=\"font-family: verdana; font-size: 11px;\">Website<br>" + purchase.tblTerminals.terminal + "<br><br>Client <br>" + purchase.tblLeads.firstName + " " + purchase.tblLeads.lastName + "<br><br>Error <br>" + response.EM_Response.ToString() + "<br><br>Inner Exception <br>" + response.EM_RefNum + "</span>";
                email.IsBodyHtml = true;
                email.Priority = System.Net.Mail.MailPriority.High;
                email.To.Add("gguerrap@villagroup.com");
                email.To.Add("info@myvallartaexperience.com");

                //Utils.EmailNotifications.Send(email);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
            }


            return valid;
        }

        public static AttemptResponse PartialSave(PurchaseProcessViewModel purchase)
        {
            AttemptResponse attempt = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            try
            {
                tblPurchases newPurchase = SavePurchase(db, purchase);
                //generar SHA1
                string comdigest = "";
                string comtotal = newPurchase.total.ToString().Replace(".", "");
                string comcurrency = "484";
                string comorder_id = purchase.PurchaseID.ToString().Replace("-", "").Substring(0, 26);
                string commerchant = purchase.PaymentsMerchantID.ToString();
                string comstore = "1234";
                string comterm = "001";
                comdigest = Utils.GeneralFunctions.GetSHA1(commerchant + comstore + comterm + comtotal + comcurrency + comorder_id);
                //respuesta
                attempt.Type = Attempt_ResponseTypes.Ok;
                attempt.ObjectID = newPurchase.purchaseID;
                attempt.Message = comdigest;
            }
            catch (Exception e)
            {
                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Error al intentar guardar los datos de la compra.";
            }

            return attempt;
        }

        public static tblPurchases SavePurchase(ePlatEntities db, PurchaseProcessViewModel purchase)
        {
            tblPurchases newPurchase = new tblPurchases();
            if (purchase.PurchaseID == new Guid("00000000-0000-0000-0000-000000000000"))
            {
                //new purchase
                newPurchase.purchaseID = Guid.NewGuid();

                tblLeads newLead = new tblLeads();
                newLead.leadID = Guid.NewGuid();
                newLead.initialTerminalID = purchase.TerminalID;
                newLead.terminalID = purchase.TerminalID;
                newLead.firstName = purchase.FirstName;
                newLead.lastName = purchase.LastName;
                newLead.city = purchase.City;
                newLead.state = purchase.State;
                newLead.countryID = purchase.Country;
                //newLead.spouseFirstName
                //newLead.spouseLastName
                newLead.address = purchase.Address;
                newLead.zipcode = purchase.ZipCode;
                newLead.leadTypeID = 4;
                newLead.bookingStatusID = 3;
                //newLead.qualificationStatusID 
                newLead.leadSourceID = 20; //other
                newLead.inputByUserID = new Guid("c53613b6-c8b8-400d-95c6-274e6e60a14a");
                //newLead.assignedToUserID
                //newLead.modifiedByUserID
                newLead.inputDateTime = DateTime.Now;
                //newLead.assignationDate
                //newLead.modificationDate
                //newLead.referredByID
                //newLead.offerID
                //newLead.leadGenerator
                //newLead.TOSatus
                //newLead.TOCloser
                //newLead.leadComments
                newLead.inputMethodID = 4;
                newLead.deleted = false;
                //newLead.deletedByUserID
                newLead.isTest = false;
                //newLead.dateDeleted
                //newLead.leadDescription
                //newLead.leadStatusDescription
                //newLead.memberID
                newLead.leadStatusID = 7;
                //newLead.timeZoneID
                newLead.confirmed = false;
                //newLead.submissionForm
                //newLead.lastInteractionTypeID
                //newLead.callClasificationID
                //newLead.brokerContractID
                tblLeadEmails newEmail = new tblLeadEmails();
                newEmail.email = purchase.Email;
                newEmail.main = true;
                newLead.tblLeadEmails.Add(newEmail);
                tblPhones newPhone = new tblPhones();
                newPhone.phone = purchase.Phone;
                newPhone.phoneTypeID = 4; //unknown
                newPhone.doNotCall = false;
                newPhone.ext = "";
                newPhone.main = true;
                newLead.tblPhones.Add(newPhone);
                if (purchase.Mobile != null && purchase.Mobile != "")
                {
                    tblPhones newMobile = new tblPhones();
                    newMobile.phone = purchase.Mobile;
                    newMobile.phoneTypeID = 1; //mobile
                    newMobile.doNotCall = false;
                    newMobile.ext = "";
                    newMobile.main = false;
                    newLead.tblPhones.Add(newMobile);
                }
                newLead.inputDateTime = DateTime.Now;

                newPurchase.tblLeads = newLead;

                newPurchase.culture = Utils.GeneralFunctions.GetCulture();
                newPurchase.purchaseDateTime = DateTime.Now;
                //newPurchase.originalLandingPage
                newPurchase.ipAddress = purchase.IP;
                newPurchase.browser = purchase.Browser;
                newPurchase.os = purchase.OS;
                if (purchase.PromoID != 0)
                {
                    newPurchase.promoID = purchase.PromoID;
                }
                newPurchase.terminalID = purchase.TerminalID;
                newPurchase.customerRequests = purchase.Comments;
                newPurchase.total = purchase.Total;
                newPurchase.currencyID = purchase.CurrencyID;
                newPurchase.purchaseStatusID = 1;
                newPurchase.deleted = false;
                newPurchase.pointOfSaleID = purchase.PointOfSaleID;
                if (purchase.StayingAtPlaceID > 0)
                {
                    newPurchase.stayingAtPlaceID = purchase.StayingAtPlaceID;

                }
                else if (purchase.StayingAtPlaceID == 0)
                {
                    newPurchase.stayingAt = purchase.StayingAt;
                }
                newPurchase.jsonCart = purchase.ItemsJSON;
                if (purchase.CampaignTag != null)
                {
                    var campaignID = (from c in db.tblCampaigns
                                      where c.tag == purchase.CampaignTag
                                      && c.terminalID == purchase.TerminalID
                                      select c.campaignID).FirstOrDefault();
                    if (campaignID != null)
                    {
                        newPurchase.campaignID = campaignID;
                    }
                }

                db.tblPurchases.AddObject(newPurchase);
                db.SaveChanges();
            }
            else
            {
                //editing
                newPurchase = (from p in db.tblPurchases
                               where p.purchaseID == purchase.PurchaseID
                               select p).Single();

                newPurchase.tblLeads.firstName = purchase.FirstName;
                newPurchase.tblLeads.lastName = purchase.LastName;
                newPurchase.tblLeads.tblLeadEmails.First().email = purchase.Email;
                if (newPurchase.tblLeads.tblPhones.FirstOrDefault(x => x.phoneTypeID == 4) != null)
                {
                    newPurchase.tblLeads.tblPhones.FirstOrDefault(x => x.phoneTypeID == 4).phone = purchase.Phone;
                }
                if (newPurchase.tblLeads.tblPhones.FirstOrDefault(x => x.phoneTypeID == 1) != null && purchase.Mobile != null && purchase.Mobile != "")
                {
                    newPurchase.tblLeads.tblPhones.FirstOrDefault(x => x.phoneTypeID == 1).phone = purchase.Mobile;
                }

                newPurchase.tblLeads.address = purchase.Address;
                newPurchase.tblLeads.city = purchase.City;
                newPurchase.tblLeads.state = purchase.State;
                newPurchase.tblLeads.countryID = purchase.Country;
                newPurchase.tblLeads.zipcode = purchase.ZipCode;

                newPurchase.customerRequests = purchase.Comments;
                newPurchase.total = purchase.Total;
                if (purchase.PromoID != 0)
                {
                    newPurchase.promoID = purchase.PromoID;
                }
                newPurchase.currencyID = purchase.CurrencyID;
                if (purchase.StayingAtPlaceID > 0)
                {
                    newPurchase.stayingAtPlaceID = purchase.StayingAtPlaceID;
                }
                else if (purchase.StayingAtPlaceID == 0)
                {
                    newPurchase.stayingAt = purchase.StayingAt;
                }
            }

            //items
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<PurchaseItem> items = js.Deserialize<List<PurchaseItem>>(purchase.ItemsJSON);
            foreach (PurchaseItem item in items)
            {
                if (item.DateSaved != null)
                {
                    //ya existe, actualizar
                    List<long> ItemsToDelete = new List<long>();
                    foreach (tblPurchases_Services cartitem in newPurchase.tblPurchases_Services)
                    {
                        if (cartitem.purchase_ServiceID == item.CartItemID)
                        {
                            if (item.Deleted)
                            {
                                ItemsToDelete.Add(item.CartItemID);
                            }
                            else
                            {
                                cartitem.serviceID = item.ServiceID;
                                cartitem.wholeStay = false;
                                if (item.WeeklyAvailabilityID != null)
                                {
                                    cartitem.weeklyAvailabilityID = (long)item.WeeklyAvailabilityID;
                                    cartitem.serviceDateTime = Convert.ToDateTime(item.ServiceDate + " " + Utils.GeneralFunctions.DateFormat.ToMilitarHour(item.Schedule));
                                }
                                else
                                {
                                    cartitem.serviceDateTime = Convert.ToDateTime(item.ServiceDate);
                                }
                                cartitem.serviceStatusID = 1;
                                cartitem.total = item.PromoTotal;
                                bool promoPerPerson = false;
                                tblPromos appliedPromo = null;
                                if (item.PromoID != null && item.PromoID != 0)
                                {
                                    cartitem.promoID = item.PromoID;
                                    appliedPromo = db.tblPromos.Single(x => x.promoID == item.PromoID);
                                    promoPerPerson = appliedPromo.applyOnPerson;
                                }
                                else
                                {
                                    cartitem.promoID = null;
                                }
                                cartitem.savings = item.PromoSavings;
                                cartitem.currencyID = purchase.CurrencyID;

                                if (item.ServiceType == 3)
                                {
                                    cartitem.airline = item.Airline;
                                    cartitem.flightNumber = item.Flight;
                                    if (item.HotelID > 0)
                                    {
                                        newPurchase.stayingAtPlaceID = item.HotelID;
                                    }
                                    if (item.TransportationZoneID == 0)
                                    {
                                        cartitem.transportationZoneID = PlaceDataModel.GetTransportationZoneForPlace((long)item.HotelID);
                                    }
                                    else
                                    {
                                        cartitem.transportationZoneID = item.TransportationZoneID;
                                    }
                                    cartitem.customMeetingTime = item.Schedule != null ? TimeSpan.Parse(Utils.GeneralFunctions.DateFormat.ToMilitarHour(item.Schedule), System.Globalization.CultureInfo.InvariantCulture) : (TimeSpan?)null;

                                    if (item.Round != null && item.Round == true)
                                    {
                                        cartitem.round = item.Round;
                                        cartitem.roundAirline = item.RoundAirline;
                                        cartitem.roundFlightNumber = item.RoundFlightNumber;
                                        cartitem.roundDate = Convert.ToDateTime(item.RoundDate);
                                        cartitem.roundFlightTime = item.RoundMeetingTime != null ? TimeSpan.Parse(Utils.GeneralFunctions.DateFormat.ToMilitarHour(item.RoundMeetingTime), System.Globalization.CultureInfo.InvariantCulture) : (TimeSpan?)null;
                                    }
                                }

                                var copyDetails = new tblPurchaseServiceDetails[cartitem.tblPurchaseServiceDetails.Count()];
                                cartitem.tblPurchaseServiceDetails.CopyTo(copyDetails, 0);
                                foreach (var itemDetail in copyDetails)
                                {
                                    db.tblPurchaseServiceDetails.DeleteObject(itemDetail);
                                }

                                long promoPriceID = 0;
                                decimal promoCheapestPrice = 0;
                                int promoPriceTypeID = 0;
                                long? promoExchangeRateID = null;

                                //ciclo para identificar el precio menor
                                foreach (PurchaseItemDetail detail in item.Details)
                                {
                                    if (promoCheapestPrice == 0 || detail.Price < promoCheapestPrice)
                                    {
                                        promoPriceID = detail.PriceID;
                                        promoCheapestPrice = detail.Price;
                                        promoPriceTypeID = detail.PriceTypeID;
                                        promoExchangeRateID = detail.ExchangeRateID;
                                    }
                                }

                                //ciclo para guardar y aplicar promo
                                decimal promoQuantity = 1;
                                foreach (PurchaseItemDetail detail in item.Details)
                                {
                                    //agregar
                                    tblPurchaseServiceDetails newDetail = new tblPurchaseServiceDetails();
                                    newDetail.netPriceID = detail.PriceID;
                                    newDetail.priceTypeID = detail.PriceTypeID;
                                    newDetail.exchangeRateID = detail.ExchangeRateID;
                                    newDetail.customPrice = detail.Price;
                                    newDetail.quantity = detail.Quantity;
                                    if (cartitem.promoID != null && !promoPerPerson)
                                    {
                                        newDetail.promo = true;
                                    }
                                    else if (cartitem.promoID != null && promoPerPerson && promoPriceID == detail.PriceID)
                                    {
                                        foreach (var p in appliedPromo.tblPromos_PromoTypes.OrderByDescending(x => x.promoTypeID))
                                        {
                                            if (p.promoTypeID == 3 && detail.Quantity % 4 == 0) //4x3
                                            {
                                                promoQuantity = Math.Floor(detail.Quantity / 4);
                                            }
                                            else if (p.promoTypeID == 2 && detail.Quantity % 3 == 0) //3x2
                                            {
                                                promoQuantity = Math.Floor(detail.Quantity / 3);
                                            }
                                            else if (p.promoTypeID == 1 && detail.Quantity % 2 == 0) //2x1
                                            {
                                                promoQuantity = Math.Floor(detail.Quantity / 2);
                                            }
                                            else if (p.promoTypeID == 3 && detail.Quantity > 4) //4x3
                                            {
                                                promoQuantity = Math.Floor(detail.Quantity / 4);
                                            }
                                            else if (p.promoTypeID == 2 && detail.Quantity > 3) //3x2
                                            {
                                                promoQuantity = Math.Floor(detail.Quantity / 3);
                                            }
                                            else if (p.promoTypeID == 1 && detail.Quantity > 2) //2x1
                                            {
                                                promoQuantity = Math.Floor(detail.Quantity / 2);
                                            }
                                        }
                                        newDetail.quantity = detail.Quantity - promoQuantity;

                                    }
                                    cartitem.tblPurchaseServiceDetails.Add(newDetail);
                                }
                                if (promoPerPerson)
                                {
                                    tblPurchaseServiceDetails newDetail = new tblPurchaseServiceDetails();

                                    newDetail.quantity = promoQuantity;

                                    newDetail.promo = true;
                                    newDetail.netPriceID = promoPriceID;
                                    newDetail.priceTypeID = promoPriceTypeID;
                                    newDetail.exchangeRateID = promoExchangeRateID;
                                    newDetail.customPrice = promoCheapestPrice;
                                    cartitem.tblPurchaseServiceDetails.Add(newDetail);
                                }
                            }
                        }
                    }
                    foreach (long psid in ItemsToDelete)
                    {
                        //newPurchase.tblPurchases_Services.Remove(newPurchase.tblPurchases_Services.Single(x => x.purchase_ServiceID == psid));
                        db.tblPurchases_Services.DeleteObject(newPurchase.tblPurchases_Services.Single(x => x.purchase_ServiceID == psid));
                    }
                }
                else
                {
                    if (!item.Deleted)
                    {
                        //nuevo, agregar
                        tblPurchases_Services newService = new tblPurchases_Services();
                        newService.serviceID = item.ServiceID;
                        newService.dateSaved = DateTime.Now;
                        newService.wholeStay = false;
                        //newService.serviceDateTime = Convert.ToDateTime(item.ServiceDate);
                        //newService.weeklyAvailabilityID = item.WeeklyAvailabilityID;
                        if (item.WeeklyAvailabilityID != null)
                        {
                            newService.weeklyAvailabilityID = (long)item.WeeklyAvailabilityID;
                            newService.serviceDateTime = Convert.ToDateTime(item.ServiceDate + " " + Utils.GeneralFunctions.DateFormat.ToMilitarHour(item.Schedule));
                        }
                        else
                        {
                            newService.serviceDateTime = Convert.ToDateTime(item.ServiceDate);
                        }
                        //newService.meetingPointID / ask to client
                        newService.serviceStatusID = 1;
                        //newService.confirmationNumber / set by agent
                        //newService.promoID / set by agent
                        newService.total = item.PromoTotal;
                        newService.savings = item.PromoSavings;
                        bool promoPerPerson = false;
                        tblPromos appliedPromo = null;
                        if (item.PromoID != null && item.PromoID != 0)
                        {
                            newService.promoID = item.PromoID;
                            appliedPromo = db.tblPromos.Single(x => x.promoID == item.PromoID);
                            promoPerPerson = appliedPromo.applyOnPerson;
                        }
                        else
                        {
                            newService.promoID = null;
                        }
                        newService.currencyID = purchase.CurrencyID;
                        newService.reservedFor = purchase.FirstName + " " + purchase.LastName;
                        //newService.childrenAges / ask to client
                        //newService.note / set by agent

                        //TRANSPORTATION
                        if (item.ServiceType == 3)
                        {
                            newService.airline = item.Airline;
                            newService.flightNumber = item.Flight;
                            if (item.HotelID > 0)
                            {
                                newPurchase.stayingAtPlaceID = item.HotelID;
                            }
                            if (item.TransportationZoneID == 0)
                            {
                                newService.transportationZoneID = PlaceDataModel.GetTransportationZoneForPlace((long)item.HotelID);
                            }
                            else
                            {
                                newService.transportationZoneID = item.TransportationZoneID;
                            }
                            newService.customMeetingTime = item.Schedule != null ? TimeSpan.Parse(Utils.GeneralFunctions.DateFormat.ToMilitarHour(item.Schedule), System.Globalization.CultureInfo.InvariantCulture) : (TimeSpan?)null;

                            if (item.Round != null && item.Round == true)
                            {
                                newService.round = item.Round;
                                newService.roundAirline = item.RoundAirline;
                                newService.roundFlightNumber = item.RoundFlightNumber;
                                newService.roundDate = Convert.ToDateTime(item.RoundDate);
                                newService.roundFlightTime = item.RoundMeetingTime != null ? TimeSpan.Parse(Utils.GeneralFunctions.DateFormat.ToMilitarHour(item.RoundMeetingTime), System.Globalization.CultureInfo.InvariantCulture) : (TimeSpan?)null;
                            }
                        }

                        //newService.specialRequest / set by agent
                        //newService.cancelationCharge / set by agent
                        //newService.cancelationDateTime / set by system

                        long promoPriceID = 0;
                        decimal promoCheapestPrice = 0;
                        int promoPriceTypeID = 0;
                        long? promoExchangeRateID = null;

                        //ciclo para identificar el precio menor
                        foreach (PurchaseItemDetail detail in item.Details)
                        {
                            if (promoCheapestPrice == 0 || detail.Price < promoCheapestPrice)
                            {
                                promoPriceID = detail.PriceID;
                                promoCheapestPrice = detail.Price;
                                promoPriceTypeID = detail.PriceTypeID;
                                promoExchangeRateID = detail.ExchangeRateID;
                            }
                        }

                        //ciclo para guardar y aplicar promo
                        decimal promoQuantity = 1;
                        foreach (PurchaseItemDetail detail in item.Details)
                        {
                            //agregar
                            tblPurchaseServiceDetails newDetail = new tblPurchaseServiceDetails();
                            newDetail.netPriceID = detail.PriceID;
                            newDetail.priceTypeID = detail.PriceTypeID;
                            newDetail.exchangeRateID = detail.ExchangeRateID;
                            newDetail.customPrice = detail.Price;
                            newDetail.quantity = detail.Quantity;
                            if (newService.promoID != null && !promoPerPerson)
                            {
                                newDetail.promo = true;
                            }
                            else if (newService.promoID != null && promoPerPerson && promoPriceID == detail.PriceID)
                            {
                                foreach (var p in appliedPromo.tblPromos_PromoTypes.OrderByDescending(x => x.promoTypeID))
                                {
                                    if (p.promoTypeID == 3 && detail.Quantity % 4 == 0) //4x3
                                    {
                                        promoQuantity = Math.Floor(detail.Quantity / 4);
                                    }
                                    else if (p.promoTypeID == 2 && detail.Quantity % 3 == 0) //3x2
                                    {
                                        promoQuantity = Math.Floor(detail.Quantity / 3);
                                    }
                                    else if (p.promoTypeID == 1 && detail.Quantity % 2 == 0) //2x1
                                    {
                                        promoQuantity = Math.Floor(detail.Quantity / 2);
                                    }
                                    else if (p.promoTypeID == 3 && detail.Quantity > 4) //4x3
                                    {
                                        promoQuantity = Math.Floor(detail.Quantity / 4);
                                    }
                                    else if (p.promoTypeID == 2 && detail.Quantity > 3) //3x2
                                    {
                                        promoQuantity = Math.Floor(detail.Quantity / 3);
                                    }
                                    else if (p.promoTypeID == 1 && detail.Quantity > 2) //2x1
                                    {
                                        promoQuantity = Math.Floor(detail.Quantity / 2);
                                    }
                                }
                                newDetail.quantity = detail.Quantity - promoQuantity;
                            }
                            if (newDetail.quantity > 0)
                            {
                                newService.tblPurchaseServiceDetails.Add(newDetail);
                            }
                        }
                        if (promoPerPerson)
                        {
                            tblPurchaseServiceDetails newDetail = new tblPurchaseServiceDetails();
                            newDetail.quantity = promoQuantity;
                            newDetail.promo = true;
                            newDetail.netPriceID = promoPriceID;
                            newDetail.priceTypeID = promoPriceTypeID;
                            newDetail.exchangeRateID = promoExchangeRateID;
                            newDetail.customPrice = promoCheapestPrice;
                            newService.tblPurchaseServiceDetails.Add(newDetail);
                        }

                        newPurchase.tblPurchases_Services.Add(newService);
                    }
                }
            }

            db.SaveChanges();

            return newPurchase;
        }


        public static MarketPlaceViewModel.Cart GetMarketPlaceCart(Guid? purchaseID, string crs)
        {
            MarketPlaceViewModel.Cart cart = new MarketPlaceViewModel.Cart();
            ePlatEntities db = new ePlatEntities();

            if (purchaseID == null && crs != null && crs != "")
            {
                var PurchaseIDQ = (from p in db.tblPurchases
                                  where p.crs == crs
                                  select new { p.purchaseID }).FirstOrDefault();

                if(PurchaseIDQ != null)
                {
                    purchaseID = PurchaseIDQ.purchaseID;
                }
            }

            if (purchaseID != null)
            {
                var Purchase = (from p in db.tblPurchases
                                where p.purchaseID == purchaseID
                                select new { 
                                    p.crs,
                                    p.terminalID,
                                    p.stayingAtPlaceID,
                                    p.pointOfSaleID,
                                    p.total,
                                    p.currencyID,
                                    p.culture
                                }).FirstOrDefault();

                if (Purchase != null)
                {
                    cart.ConfirmationNumber = Purchase.crs;
                    cart.TerminalID = Purchase.terminalID;
                    cart.PlaceID = (long)Purchase.stayingAtPlaceID;
                    cart.PointOfSaleID = Purchase.pointOfSaleID;
                    cart.Total = (decimal)Purchase.total;
                    cart.CurrencyID = Purchase.currencyID;
                    cart.Language = Purchase.culture;
                    cart.Items = new List<MarketPlaceViewModel.CartItem>();

                    var Items = (from i in db.tblPurchases_Services
                                 join s in db.tblServiceDescriptions.Where(x => x.culture == Purchase.culture)
                                 on i.serviceID equals s.serviceID
                                 into i_s
                                 from s in i_s.DefaultIfEmpty()
                                 join x in db.tblServices
                                 on i.serviceID equals x.serviceID
                                 into i_x
                                 from x in i_x.DefaultIfEmpty()
                                 join p in db.tblProviders
                                 on x.providerID equals p.providerID
                                 into i_p
                                 from p in i_p.DefaultIfEmpty()
                                 where i.purchaseID == purchaseID
                                 select new
                                 {
                                     i.purchase_ServiceID,
                                     i.serviceID,
                                     s.service,
                                     x.length,
                                     i.serviceDateTime,
                                     i.weeklyAvailabilityID,
                                     i.total,
                                     i.dateSaved,
                                     i.wholeStay,
                                     i.reservedFor,
                                     provider = p.comercialName,
                                     x.itemTypeID
                                 }).ToList();

                    var ItemIDs = Items.Select(x => x.purchase_ServiceID).ToList();

                    var Details = (from d in db.tblPurchaseServiceDetails
                                   join u in db.tblPriceUnits.Where(x => x.culture == Purchase.culture)
                                   on d.netPriceID equals u.priceID
                                   into d_u
                                   from u in d_u.DefaultIfEmpty()
                                   where ItemIDs.Contains(d.purchase_ServiceID)
                                   select new { 
                                        d.purchase_ServiceID,
                                        d.purchaseServiceDetailID,
                                        d.netPriceID,
                                        d.priceTypeID,
                                        d.exchangeRateID,
                                        d.quantity,
                                        d.customPrice,
                                        u.unit
                                   }).ToList();

                    foreach(var item in Items)
                    {
                        MarketPlaceViewModel.CartItem newItem = new MarketPlaceViewModel.CartItem();
                        newItem.CartItemID = item.purchase_ServiceID;
                        newItem.ItemID = item.serviceID;
                        newItem.Item = item.service;
                        newItem.ItemTypeID = item.itemTypeID;
                        newItem.Length = (item.length / 60) + " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.hours + (item.length % 60 > 0 ? " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.and + " " + item.length % 60 + " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.minutes : "");
                        newItem.Picture = PictureDataModel.GetMainPictureItem(1, item.serviceID);
                        newItem.ReservedFor = item.reservedFor;
                        newItem.Provider = item.provider;
                        newItem.ItemDate = item.serviceDateTime.ToString("yyyy-MM-dd");
                        newItem.WeeklyAvailabilityID = item.weeklyAvailabilityID;
                        newItem.Schedule = item.serviceDateTime.ToString("hh:mm tt");
                        newItem.Total = item.total;
                        newItem.DateSaved = item.dateSaved;
                        newItem.ApplyForMultipleDates = item.wholeStay;
                        newItem.Details = new List<MarketPlaceViewModel.CartItemDetail>();

                        foreach(var detail in Details.Where(x => x.purchase_ServiceID == item.purchase_ServiceID))
                        {
                            MarketPlaceViewModel.CartItemDetail newDetail = new MarketPlaceViewModel.CartItemDetail();
                            newDetail.PriceID = (long)detail.netPriceID;
                            newDetail.PriceTypeID = (int)detail.priceTypeID;
                            newDetail.ExchangeRateID = detail.exchangeRateID;
                            newDetail.Quantity = detail.quantity;
                            newDetail.Price = (decimal)detail.customPrice;
                            newDetail.Unit = detail.unit;
                            newItem.Details.Add(newDetail);
                        }

                        cart.Items.Add(newItem);
                    }
                }
            }

            return cart;
        }

        public static AttemptResponse SaveMarketPlaceCart(MarketPlaceViewModel.Cart cart)
        {
            AttemptResponse attempt = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();

            //guarcar compra
            tblPurchases purchase = new tblPurchases();
            bool isANewOne = false;
            purchase = (from p in db.tblPurchases
                        where p.crs == cart.ConfirmationNumber
                        select p).FirstOrDefault();

            if (purchase == null)
            {
                //nueva
                isANewOne = true;
                purchase = new tblPurchases();
                purchase.purchaseID = Guid.NewGuid();
                purchase.culture = cart.Language;
                purchase.purchaseDateTime = DateTime.Now;
                purchase.ipAddress = cart.IPAddress;
                purchase.terminalID = cart.TerminalID;
                purchase.deleted = false;
                purchase.purchaseStatusID = 1;
                purchase.crs = cart.ConfirmationNumber;
                purchase.pointOfSaleID = cart.PointOfSaleID;
            }

            purchase.total = cart.Total;
            purchase.currencyID = cart.CurrencyID;
            purchase.stayingAtPlaceID = cart.PlaceID;

            if (isANewOne)
            {
                db.tblPurchases.AddObject(purchase);
            }
            db.SaveChanges();

            //guardar cupones
            foreach (var item in cart.Items)
            {
                if (item.DateSaved != null)
                {
                    //ya existe, actualizar
                    List<long?> ItemsToDelete = new List<long?>();
                    foreach (tblPurchases_Services cartitem in purchase.tblPurchases_Services)
                    {
                        if (cartitem.purchase_ServiceID == item.CartItemID)
                        {
                            if (item.Deleted)
                            {
                                ItemsToDelete.Add(item.CartItemID);
                            }
                            else
                            {
                                cartitem.serviceID = item.ItemID;
                                cartitem.reservedFor = item.ReservedFor;
                                cartitem.wholeStay = false;
                                if (item.WeeklyAvailabilityID != null)
                                {
                                    cartitem.weeklyAvailabilityID = (long)item.WeeklyAvailabilityID;
                                    cartitem.serviceDateTime = Convert.ToDateTime(item.ItemDate + " " + Utils.GeneralFunctions.DateFormat.ToMilitarHour(item.Schedule));
                                }
                                else
                                {
                                    cartitem.serviceDateTime = Convert.ToDateTime(item.ItemDate);
                                }
                                cartitem.serviceStatusID = 1;
                                cartitem.total = item.Total;
                                cartitem.currencyID = purchase.currencyID;

                                var copyDetails = new tblPurchaseServiceDetails[cartitem.tblPurchaseServiceDetails.Count()];
                                cartitem.tblPurchaseServiceDetails.CopyTo(copyDetails, 0);
                                foreach (var itemDetail in copyDetails)
                                {
                                    db.tblPurchaseServiceDetails.DeleteObject(itemDetail);
                                }

                                foreach (MarketPlaceViewModel.CartItemDetail detail in item.Details)
                                {
                                    //agregar
                                    tblPurchaseServiceDetails newDetail = new tblPurchaseServiceDetails();
                                    newDetail.promo = false;
                                    newDetail.netPriceID = detail.PriceID;
                                    newDetail.priceTypeID = detail.PriceTypeID;
                                    newDetail.exchangeRateID = detail.ExchangeRateID;
                                    newDetail.customPrice = detail.Price;
                                    newDetail.quantity = detail.Quantity;

                                    cartitem.tblPurchaseServiceDetails.Add(newDetail);
                                }
                            }
                        }
                    }
                    foreach (long psid in ItemsToDelete)
                    {
                        db.tblPurchases_Services.DeleteObject(purchase.tblPurchases_Services.Single(x => x.purchase_ServiceID == psid));
                    }
                }
                else
                {
                    if (!item.Deleted)
                    {
                        //nuevo, agregar
                        tblPurchases_Services newService = new tblPurchases_Services();
                        newService.serviceID = item.ItemID;
                        newService.dateSaved = DateTime.Now;
                        newService.wholeStay = item.ApplyForMultipleDates;
                        newService.serviceDateTime = Convert.ToDateTime(item.ItemDate);
                        newService.weeklyAvailabilityID = item.WeeklyAvailabilityID;
                        if (item.WeeklyAvailabilityID != null)
                        {
                            newService.weeklyAvailabilityID = (long)item.WeeklyAvailabilityID;
                            newService.serviceDateTime = Convert.ToDateTime(item.ItemDate + " " + Utils.GeneralFunctions.DateFormat.ToMilitarHour(item.Schedule));
                        }
                        else
                        {
                            newService.serviceDateTime = Convert.ToDateTime(item.ItemDate);
                        }

                        newService.serviceStatusID = 1;
                        newService.reservedFor = item.ReservedFor;
                        newService.total = item.Total;
                        newService.currencyID = purchase.currencyID;


                        foreach (MarketPlaceViewModel.CartItemDetail detail in item.Details)
                        {
                            //agregar
                            tblPurchaseServiceDetails newDetail = new tblPurchaseServiceDetails();
                            newDetail.netPriceID = detail.PriceID;
                            newDetail.priceTypeID = detail.PriceTypeID;
                            newDetail.exchangeRateID = detail.ExchangeRateID;
                            newDetail.customPrice = detail.Price;
                            newDetail.quantity = detail.Quantity;
                            if (newDetail.quantity > 0)
                            {
                                newService.tblPurchaseServiceDetails.Add(newDetail);
                            }
                        }

                        purchase.tblPurchases_Services.Add(newService);
                    }
                }
            }

            db.SaveChanges();

            //disparar correo de notificación
            var NotificationQ = (from e in db.tblEmailNotifications
                                 where e.eventID == 49
                                 && e.terminalID == purchase.terminalID
                                 && e.tblEmails.culture == purchase.culture
                                 && e.tblEmailNotifications_PointsOfSale.Count() == 0
                                 select new { e.emailNotificationID }).FirstOrDefault();

            if (NotificationQ != null)
            {
                System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmailByNotification((int)NotificationQ.emailNotificationID);
                email.Body = email.Body.Replace("$ConfirmationNumber", purchase.crs);

                var reservation = ResortConnectDataModel.GetReservation(purchase.crs);
                double difference = ((DateTime)reservation.Arrival - DateTime.Today).TotalDays;
                if(difference > 90)
                {
                    email.To.Add("respromos@resortcom.com");
                } else 
                {
                    if(difference > 0)
                    {
                        email.To.Add("joshl@resortcom.com");
                    }
                }

                ApiReferralsDataModel.SendEmail(email);

                //Utils.EmailNotifications.Send(email);
                //EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
            }

            attempt.Type = Attempt_ResponseTypes.Ok;
            attempt.Object = GetMarketPlaceCart(purchase.purchaseID, null);

            return attempt;
        }

        public static async Task<AttemptResponse> Save(PurchaseProcessViewModel purchase)
        {
            AttemptResponse attempt = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            tblPurchases newPurchase = SavePurchase(db, purchase);
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            if (purchase.CurrencyID == 1)
            {
                //payment USD
                tblPaymentDetails newPaymentDetails = new tblPaymentDetails();
                string ccNumberTrimed = purchase.CCNumber.Replace(" ", string.Empty);
                if (ccNumberTrimed != "4111111111111111")
                {
                    try
                    {
                        //INICIA el proceso de pago
                        newPurchase.isTest = false;
                        int merchantAccountID = 0;
                        RescomDataModel.ApplyPayment_Data paymentData = new RescomDataModel.ApplyPayment_Data();
                        tblMerchantAccounts UsersQ = null;
                        //revisar si existe merchant para el punto de venta
                        UsersQ = (from u in db.tblMerchantAccountSettings
                                  where u.terminalID == purchase.TerminalID
                                  && u.pointOfSaleID == newPurchase.pointOfSaleID
                                  && u.tblMerchantAccounts.currencyID == purchase.CurrencyID
                                  select u.tblMerchantAccounts).FirstOrDefault();

                        if (UsersQ != null)
                        {
                            merchantAccountID = UsersQ.merchantAccountID;
                            paymentData.UserName = UsersQ.username;
                            paymentData.Password = UsersQ.password;
                        }
                        else
                        {
                            //revisar sin punto de venta
                            UsersQ = (from u in db.tblMerchantAccountSettings
                                      where u.terminalID == purchase.TerminalID
                                      && u.tblMerchantAccounts.currencyID == purchase.CurrencyID
                                      select u.tblMerchantAccounts).FirstOrDefault();

                            if (UsersQ != null)
                            {
                                merchantAccountID = UsersQ.merchantAccountID;
                                paymentData.UserName = UsersQ.username;
                                paymentData.Password = UsersQ.password;
                            }
                        }

                        paymentData.Card_Holder = purchase.CCHolderName;
                        paymentData.Card_Holder_Address = purchase.Address;
                        paymentData.Card_Holder_Zip = purchase.ZipCode;
                        paymentData.Card_Number = ccNumberTrimed;
                        paymentData.Card_Security_Number = purchase.CCSecurityNumber;
                        paymentData.Expiration_Date = purchase.CCExpirationMonth + "/01/" + purchase.CCExpirationYear;
                        paymentData.Reference_Code = TerminalDataModel.GetPrefix(purchase.TerminalID) + "-" + newPurchase.purchaseID;
                        paymentData.Transaction_Amount = Convert.ToDouble(purchase.Total);
                        paymentData.CurrencyID = purchase.CurrencyID;

                        RescomDataModel.ApplyPayment_Response paymentResponse = RescomDataModel.ApplyPayment(paymentData);

                        if (paymentResponse.Error_Code == 0)
                        {
                            //pagado
                            attempt.Type = Attempt_ResponseTypes.Ok;

                        }
                        else
                        {
                            // no se efectuó el pago
                            attempt.Type = Attempt_ResponseTypes.Error;
                            attempt.Message = RescomDataModel.ApplyPayment_ErrorCodes[paymentResponse.Error_Code];
                        }

                        //TERMINA el proceso de pago

                        //guardar paymentDetails
                        newPaymentDetails.amount = purchase.Total;
                        newPaymentDetails.currencyID = purchase.CurrencyID;
                        newPaymentDetails.chargeDescriptionID = 1;
                        newPaymentDetails.chargeTypeID = 1;
                        newPaymentDetails.dateSaved = DateTime.Now;
                        newPaymentDetails.paymentType = 2;
                        //newPaymentDetails.netCenterCharge / sólo para Reservaciones de Hospedaje
                        //newPaymentDetails.netCenterCost / sólo para reservaciones de Hospedaje
                        //newPaymentDetails.paymentComments / set by agent

                        //guardar transacción y billing info
                        tblMoneyTransactions newMoneyTransaction = new tblMoneyTransactions();
                        newMoneyTransaction.terminalID = purchase.TerminalID;
                        newMoneyTransaction.authCode = paymentResponse.Auth_Code;
                        newMoneyTransaction.authAmount = (decimal)paymentResponse.Authorization_Amount;
                        newMoneyTransaction.authDate = paymentResponse.Authorization_Date;
                        newMoneyTransaction.errorCode = paymentResponse.Error_Code.ToString();
                        newMoneyTransaction.transactionDate = DateTime.Now;
                        newMoneyTransaction.transactionTypeID = 1;
                        newMoneyTransaction.merchantAccountID = merchantAccountID;
                        newMoneyTransaction.reference = paymentData.Reference_Code;
                        newMoneyTransaction.madeByEplat = true;

                        //agregar las actividades relacionadas al pago sólo si se hizo el pago
                        ////GG : comentado porque se eliminó este procedimiento para realizarse hasta el momento del corte
                        //if (paymentResponse.Error_Code == 0)
                        //{
                        //    foreach (tblPurchases_Services cartitem in newPurchase.tblPurchases_Services)
                        //    {
                        //        tblPurchases_Services_MoneyTransactions relation = new tblPurchases_Services_MoneyTransactions();
                        //        relation.purchase_ServiceID = cartitem.purchase_ServiceID;
                        //        newMoneyTransaction.tblPurchases_Services_MoneyTransactions.Add(relation);
                        //    }
                        //}

                        tblBillingInfo newBillingInfo = new tblBillingInfo();

                        newBillingInfo.address = purchase.Address;
                        newBillingInfo.billingComments = "";
                        if (Utils.GeneralFunctions.Number.IsNumeric(purchase.CCSecurityNumber))
                        {
                            newBillingInfo.cardCVV = purchase.CCSecurityNumber;
                        }
                        newBillingInfo.cardExpiry = purchase.CCExpirationMonth + "/" + purchase.CCExpirationYear;
                        newBillingInfo.cardHolderName = purchase.FirstName + " " + purchase.LastName;
                        newBillingInfo.cardNumber = mexHash.mexHash.EncryptString(ccNumberTrimed);
                        newBillingInfo.cardTypeID = purchase.CCType;
                        newBillingInfo.city = purchase.City;
                        newBillingInfo.countryID = purchase.Country;
                        newBillingInfo.dateSaved = DateTime.Now;
                        newBillingInfo.firstName = purchase.FirstName;
                        newBillingInfo.lastName = purchase.LastName;
                        newBillingInfo.leadID = newPurchase.leadID;
                        newBillingInfo.state = purchase.State;
                        newBillingInfo.zipcode = purchase.ZipCode;

                        newMoneyTransaction.tblBillingInfo = newBillingInfo;
                        newPaymentDetails.tblMoneyTransactions = newMoneyTransaction;

                        newPurchase.tblPaymentDetails.Add(newPaymentDetails);
                    }
                    catch (Exception e)
                    {
                        attempt.Type = Attempt_ResponseTypes.Error;
                        attempt.Message = Resources.Models.Shared.PurchaseProcess.Payment_Error;
                        //enviar correo informando el error
                        //Console.WriteLine("{0} Exception caught.", e);
                        System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                        email = new System.Net.Mail.MailMessage();
                        email.From = new System.Net.Mail.MailAddress("info@myvallartaexperience.com", "ePlatFront");
                        email.Subject = "Error on Payment Process";
                        email.Body = "<span style=\"font-family: verdana; font-size: 11px;\">Website<br>" + newPurchase.tblTerminals.terminal + "<br><br>Client <br>" + newPurchase.tblLeads.firstName + " " + newPurchase.tblLeads.lastName + "<br><br>Error <br>" + e.ToString() + "<br><br>Inner Exception <br>" + e.InnerException.Message + "</span>";
                        email.IsBodyHtml = true;
                        email.Priority = System.Net.Mail.MailPriority.High;
                        email.To.Add("gguerrap@villagroup.com");
                        email.To.Add("info@myvallartaexperience.com");

                        //Utils.EmailNotifications.Send(email);
                        EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                    }
                }
                else
                {
                    //prueba de compra
                    newPurchase.isTest = true;
                    attempt.Type = Attempt_ResponseTypes.Ok;
                }

                db.SaveChanges();

                //confirmar por correo
                if (attempt.Type == Attempt_ResponseTypes.Ok)
                {
                    //obtener el correo de acuerdo al punto de venta
                    string culture = Utils.GeneralFunctions.GetCulture();
                    int? NotificationQ = (from n in db.tblEmailNotifications_PointsOfSale
                                          where n.tblEmailNotifications.eventID == 1
                                          && n.tblEmailNotifications.terminalID == purchase.TerminalID
                                          && n.pointOfSaleID == purchase.PointOfSaleID
                                          && n.tblEmailNotifications.tblEmails.culture == culture
                                          select n.emailNotificationID).FirstOrDefault();

                    if (NotificationQ == 0)
                    {
                        NotificationQ = (from e in db.tblEmailNotifications
                                         where e.eventID == 1
                                         && e.terminalID == purchase.TerminalID
                                         && e.tblEmails.culture == culture
                                         && e.tblEmailNotifications_PointsOfSale.Count() == 0
                                         select e.emailNotificationID).FirstOrDefault();
                    }
                    if (NotificationQ != null)
                    {
                        System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmailByNotification((int)NotificationQ);
                        email.To.Add(purchase.Email);
                        email.Body = email.Body
                            .Replace("$Purchase_FirstName", purchase.FirstName)
                            .Replace("$Purchase_AuthCode", (newPaymentDetails.moneyTransactionID != null ? newPaymentDetails.tblMoneyTransactions.authCode : "TEST"))
                            .Replace("$Purchase_Total", Decimal.Round(purchase.Total, 2, MidpointRounding.AwayFromZero).ToString())
                            .Replace("$Purchase_Terms", PurchaseDataModel.GetTermsByPointOfSale(purchase.PointOfSaleID));

                        //get cart items and replace on email body
                        string cartItems = "";
                        List<PurchaseItem> items = js.Deserialize<List<PurchaseItem>>(purchase.ItemsJSON);
                        foreach (PurchaseItem item in items.OrderBy(x => x.ServiceDate))
                        {
                            var serviceItemTypeID = (from p in newPurchase.tblPurchases_Services
                                                     where p.serviceID == item.ServiceID
                                                     select p.tblServices.itemTypeID).FirstOrDefault();

                            if (serviceItemTypeID != null)
                            {
                                if (serviceItemTypeID == 2 || serviceItemTypeID == 3) //services & transportation
                                {
                                    cartItems += "<p>";
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Date + ": <strong>" + item.ServiceDate + "</strong><br>";
                                    ActivityDataModel activity = new ActivityDataModel();
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Service_Time + ": <strong>" + item.Schedule + "</strong><br>";
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Service + ": <strong>" + item.Service + "</strong><br>";
                                    string units = "";
                                    foreach (PurchaseItemDetail detail in item.Details)
                                    {
                                        if (units != "")
                                        {
                                            units += ", ";
                                        }
                                        units += detail.Quantity + " " + detail.Unit;
                                    }
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Good_for + ": <strong>" + units + "</strong><br>";
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Meeting_Time + ": <strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.To_be_confirmed + ".</strong>";
                                    cartItems += "</p>";

                                    if (serviceItemTypeID == 3 && purchase.TerminalID == 6)
                                    {
                                        cartItems += "<p><strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Important + "</strong><br />" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Transportation_Note + "</p>";
                                    }
                                }
                                else if (serviceItemTypeID == 5) //car rental
                                {
                                    cartItems += "<p>";
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Date + ": <strong>" + item.ServiceDate + "</strong><br>";
                                    ActivityDataModel activity = new ActivityDataModel();
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Delivery_Time + ": <strong>" + item.Schedule + "</strong><br>";
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Solicited_Vehicle + ": <strong>" + item.Service + "</strong><br>";
                                    string units = "";
                                    foreach (PurchaseItemDetail detail in item.Details)
                                    {
                                        if (units != "")
                                        {
                                            units += ", ";
                                        }
                                        units += detail.Quantity + " " + detail.Unit;
                                    }
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Good_for + ": <strong>" + units + "</strong><br>";
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Delivery_Location + ": <strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.To_be_confirmed + ".</strong>";
                                    cartItems += "</p>";
                                }
                                else
                                {
                                    cartItems += "<p>";
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Date + ": <strong>" + item.ServiceDate + "</strong><br>";
                                    ActivityDataModel activity = new ActivityDataModel();
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Activity_Time + ": <strong>" + item.Schedule + "</strong><br>";
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Activity_Name + ": <strong>" + item.Service + "</strong><br>";
                                    string units = "";
                                    foreach (PurchaseItemDetail detail in item.Details)
                                    {
                                        if (units != "")
                                        {
                                            units += ", ";
                                        }
                                        units += detail.Quantity + " " + detail.Unit;
                                    }
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Good_for + ": <strong>" + units + "</strong><br>";
                                    cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Meeting_Time + ": <strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.To_be_confirmed + ".</strong>";
                                    cartItems += "</p>";
                                }
                            }
                        }
                        email.Body = email.Body
                            .Replace("$Purchase_CartItems", cartItems);

                        //Utils.EmailNotifications.Send(email);
                        EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                    }
                }
            }
            else
            {
                //Pago MXN
                attempt.Type = Attempt_ResponseTypes.Ok;
                //solicitud de sesión
                var client = new HttpClient();

                // Create the HttpContent for the form to be posted.
                var requestContent = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("apiOperation", "CREATE_CHECKOUT_SESSION"),
                    new KeyValuePair<string, string>("apiPassword", "feb4447fc6b57cb70940174f3c7bc97a"),
                    new KeyValuePair<string, string>("apiUsername", "merchant.6264329"),
                    new KeyValuePair<string, string>("merchant", "6264329"),
                    new KeyValuePair<string, string>("interaction.returnUrl", "https://" + HttpContext.Current.Request.Url.Host + "/Purchase/PaymentGatewayResponse/" + newPurchase.purchaseID.ToString()),
                    //new KeyValuePair<string, string>("apiPassword", "d5f30d73030a363900d1002c8f0369a5"),
                    //new KeyValuePair<string, string>("apiUsername", "merchant.TEST6264329"),
                    //new KeyValuePair<string, string>("merchant", "TEST6264329"),
                    //new KeyValuePair<string, string>("interaction.returnUrl", "http://" + HttpContext.Current.Request.Url.Host + ":37532/Purchase/PaymentGatewayResponse/" + newPurchase.purchaseID.ToString()),
                    new KeyValuePair<string, string>("order.id", newPurchase.purchaseID.ToString()),
                    new KeyValuePair<string, string>("order.amount", decimal.Round((decimal)newPurchase.total,2).ToString()),
                    new KeyValuePair<string, string>("order.currency", "MXN")
                });

                // Get the response.
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpResponseMessage response = await client.PostAsync(
                    "https://banamex.dialectpayments.com/api/nvp/version/49",
                    requestContent);

                // Get the response content.
                HttpContent responseContent = response.Content;

                // Get the stream of the content.
                using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    // Write the output.
                    var responseStr = await reader.ReadToEndAsync();
                    var responseArr = responseStr.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(part => part.Split('='))
                        .ToDictionary(split => split[0], split => split[1]);
                    string sessionID = responseArr["session.id"];
                    string successIndicator = responseArr["successIndicator"];
                    newPurchase.evoSessionID = sessionID;
                    newPurchase.evoSuccessIndicator = successIndicator;
                    db.SaveChanges();
                }
            }

            attempt.Exception = null;
            attempt.ObjectID = new SavePurchaseResponse()
            {
                PurchaseID = newPurchase.purchaseID,
                SessionID = newPurchase.evoSessionID
            };

            return attempt;
        }

        public static bool PaymentGatewayResponse(Guid purchaseID, string resultIndicator)
        {
            bool success = false;
            ePlatEntities db = new ePlatEntities();
            var purchase = (from p in db.tblPurchases
                            where p.purchaseID == purchaseID
                            select p).FirstOrDefault();

            if (purchase != null)
            {
                if (purchase.evoSuccessIndicator == resultIndicator)
                {
                    success = true;

                    tblPaymentDetails newPaymentDetails = new tblPaymentDetails();
                    newPaymentDetails.amount = (decimal)purchase.total;
                    newPaymentDetails.currencyID = purchase.currencyID;
                    newPaymentDetails.chargeDescriptionID = 1;
                    newPaymentDetails.chargeTypeID = 1;
                    newPaymentDetails.dateSaved = DateTime.Now;
                    newPaymentDetails.paymentType = 2;

                    //guardar transacción y billing info
                    tblMoneyTransactions newMoneyTransaction = new tblMoneyTransactions();
                    newMoneyTransaction.terminalID = purchase.terminalID;
                    newMoneyTransaction.authCode = resultIndicator.Substring(resultIndicator.Length - 8);
                    newMoneyTransaction.authAmount = (decimal)purchase.total;
                    newMoneyTransaction.authDate = DateTime.Now;
                    newMoneyTransaction.errorCode = "0";
                    newMoneyTransaction.transactionDate = DateTime.Now;
                    newMoneyTransaction.transactionTypeID = 1;
                    newMoneyTransaction.merchantAccountID = 12;
                    newMoneyTransaction.reference = "EVO-" + purchase.purchaseID;
                    newMoneyTransaction.madeByEplat = true;

                    newPaymentDetails.tblMoneyTransactions = newMoneyTransaction;
                    purchase.tblPaymentDetails.Add(newPaymentDetails);

                    db.SaveChanges();
                    //confirmar por correo
                    if (success)
                    {
                        //obtener el correo de acuerdo al punto de venta
                        string culture = Utils.GeneralFunctions.GetCulture();
                        int? NotificationQ = (from n in db.tblEmailNotifications_PointsOfSale
                                              where n.tblEmailNotifications.eventID == 1
                                              && n.tblEmailNotifications.terminalID == purchase.terminalID
                                              && n.pointOfSaleID == purchase.pointOfSaleID
                                              && n.tblEmailNotifications.tblEmails.culture == culture
                                              select n.emailNotificationID).FirstOrDefault();

                        if (NotificationQ == 0)
                        {
                            NotificationQ = (from e in db.tblEmailNotifications
                                             where e.eventID == 1
                                             && e.terminalID == purchase.terminalID
                                             && e.tblEmails.culture == culture
                                             && e.tblEmailNotifications_PointsOfSale.Count() == 0
                                             select e.emailNotificationID).FirstOrDefault();
                        }
                        if (NotificationQ != null)
                        {
                            System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmailByNotification((int)NotificationQ);
                            email.To.Add(purchase.tblLeads.tblLeadEmails.FirstOrDefault().email);
                            email.Body = email.Body
                                .Replace("$Purchase_FirstName", purchase.tblLeads.firstName)
                                .Replace("$Purchase_AuthCode", (newPaymentDetails.moneyTransactionID != null ? newMoneyTransaction.authCode : "TEST"))
                                .Replace("$Purchase_Total", Decimal.Round((decimal)purchase.total, 2, MidpointRounding.AwayFromZero).ToString())
                                .Replace("$Purchase_Terms", PurchaseDataModel.GetTermsByPointOfSale(purchase.pointOfSaleID));

                            //get cart items and replace on email body
                            string cartItems = "";
                            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                            List<PurchaseItem> items = js.Deserialize<List<PurchaseItem>>(purchase.jsonCart);
                            foreach (PurchaseItem item in items.OrderBy(x => x.ServiceDate))
                            {
                                var serviceItemTypeID = (from p in purchase.tblPurchases_Services
                                                         where p.serviceID == item.ServiceID
                                                         select p.tblServices.itemTypeID).FirstOrDefault();

                                if (serviceItemTypeID != null)
                                {
                                    if (serviceItemTypeID == 2 || serviceItemTypeID == 3) //services & transportation
                                    {
                                        cartItems += "<p>";
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Date + ": <strong>" + item.ServiceDate + "</strong><br>";
                                        ActivityDataModel activity = new ActivityDataModel();
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Service_Time + ": <strong>" + item.Schedule + "</strong><br>";
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Service + ": <strong>" + item.Service + "</strong><br>";
                                        string units = "";
                                        foreach (PurchaseItemDetail detail in item.Details)
                                        {
                                            if (units != "")
                                            {
                                                units += ", ";
                                            }
                                            units += detail.Quantity + " " + detail.Unit;
                                        }
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Good_for + ": <strong>" + units + "</strong><br>";
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Meeting_Time + ": <strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.To_be_confirmed + ".</strong>";
                                        cartItems += "</p>";

                                        if (serviceItemTypeID == 3 && purchase.terminalID == 6)
                                        {
                                            cartItems += "<p><strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Important + "</strong><br />" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Transportation_Note + "</p>";
                                        }
                                    }
                                    else if (serviceItemTypeID == 5) //car rental
                                    {
                                        cartItems += "<p>";
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Date + ": <strong>" + item.ServiceDate + "</strong><br>";
                                        ActivityDataModel activity = new ActivityDataModel();
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Delivery_Time + ": <strong>" + item.Schedule + "</strong><br>";
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Solicited_Vehicle + ": <strong>" + item.Service + "</strong><br>";
                                        string units = "";
                                        foreach (PurchaseItemDetail detail in item.Details)
                                        {
                                            if (units != "")
                                            {
                                                units += ", ";
                                            }
                                            units += detail.Quantity + " " + detail.Unit;
                                        }
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Good_for + ": <strong>" + units + "</strong><br>";
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Delivery_Location + ": <strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.To_be_confirmed + ".</strong>";
                                        cartItems += "</p>";
                                    }
                                    else
                                    {
                                        cartItems += "<p>";
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Date + ": <strong>" + item.ServiceDate + "</strong><br>";
                                        ActivityDataModel activity = new ActivityDataModel();
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Activity_Time + ": <strong>" + item.Schedule + "</strong><br>";
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Activity_Name + ": <strong>" + item.Service + "</strong><br>";
                                        string units = "";
                                        foreach (PurchaseItemDetail detail in item.Details)
                                        {
                                            if (units != "")
                                            {
                                                units += ", ";
                                            }
                                            units += detail.Quantity + " " + detail.Unit;
                                        }
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Good_for + ": <strong>" + units + "</strong><br>";
                                        cartItems += ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Meeting_Time + ": <strong>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.To_be_confirmed + ".</strong>";
                                        cartItems += "</p>";
                                    }
                                }
                            }
                            email.Body = email.Body
                                .Replace("$Purchase_CartItems", cartItems);

                            //Utils.EmailNotifications.Send(email);
                            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                        }
                    }
                }
            }

            return success;
        }

        public static AttemptResponse GetFromClient(string cart, string ip, string browser)
        {
            AttemptResponse attempt = new AttemptResponse();

            attempt.Type = Attempt_ResponseTypes.Ok;

            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
            email = new System.Net.Mail.MailMessage();
            email.From = new System.Net.Mail.MailAddress("info@myvallartaexperience.com", "ePlatFront");
            email.Subject = "Purchase Data Object";
            email.Body = "<span style=\"font-family: verdana; font-size: 11px;\">Cart <br>" + cart + "<br><br>IP <br>" + ip + "<br><br>Browser <br>" + browser + "</span>";
            email.IsBodyHtml = true;
            email.Priority = System.Net.Mail.MailPriority.Normal;
            email.To.Add("gguerrap@villagroup.com");
            email.To.Add("info@myvallartaexperience.com");

            //Utils.EmailNotifications.Send(email);
            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

            attempt.Exception = null;

            return attempt;
        }

        public static List<PurchaseItem> GetPurchaseItems(Guid purchaseID)
        {
            ePlatEntities db = new ePlatEntities();
            string _culture = Utils.GeneralFunctions.GetCulture();
            List<PurchaseItem> items = new List<PurchaseItem>();

            var itemsQ = from i in db.tblPurchases_Services
                         where i.purchaseID == purchaseID
                         select i;

            foreach (tblPurchases_Services service in itemsQ)
            {
                PurchaseItem item = new PurchaseItem();
                item.CartItemID = service.purchase_ServiceID;
                item.ServiceID = service.serviceID;
                item.Service = service.tblServices.tblServiceDescriptions.FirstOrDefault(x => x.culture == _culture).service;
                item.ServiceDate = string.Format("{0:yyyy-MM-dd}", service.serviceDateTime);
                if (service.weeklyAvailabilityID != null)
                {
                    item.WeeklyAvailabilityID = (long)service.weeklyAvailabilityID;
                }
                item.Schedule = (service.customMeetingTime != null ? Utils.GeneralFunctions.DateFormat.ToMeridianHour(service.customMeetingTime.ToString()) : Utils.GeneralFunctions.DateFormat.ToMeridianHour(service.serviceDateTime.ToString("HH:mm")));
                if (service.tblServices.transportationService)
                {
                    item.Airline = service.airline;
                    item.Flight = service.flightNumber;
                    item.HotelID = service.tblPurchases.stayingAtPlaceID;
                    if (service.transportationZoneID != null)
                    {
                        item.TransportationZoneID = service.transportationZoneID;
                    }
                    item.Round = service.round;
                    if (service.round == true)
                    {
                        item.RoundAirline = service.roundAirline;
                        item.RoundFlightNumber = service.roundFlightNumber;
                        item.RoundDate = string.Format("{0:yyyy-MM-dd}", service.roundDate);
                        item.RoundMeetingTime = (service.roundFlightTime != null ? Utils.GeneralFunctions.DateFormat.ToMeridianHour(service.roundFlightTime.ToString()) : null);
                    }
                }

                item.Total = service.total;
                item.PromoTotal = service.total;
                if (service.savings != null && Utils.GeneralFunctions.Number.IsNumeric(service.savings))
                {
                    item.Savings = Convert.ToDecimal(service.savings);
                    item.PromoSavings = Convert.ToDecimal(service.savings);
                }
                item.DateSaved = string.Format("{0:yyyy-MM-dd}", service.dateSaved);
                List<PurchaseItemDetail> details = new List<PurchaseItemDetail>();
                string unit = string.Empty;
                long promoPriceID = 0;
                foreach (tblPurchaseServiceDetails detail in service.tblPurchaseServiceDetails.OrderByDescending(s => s.promo))
                {
                    var currentUnit = PriceDataModel.GetUnit((detail.priceID != null ? (long)detail.priceID : (long)detail.netPriceID), _culture);
                    unit = currentUnit.unit + " " + currentUnit.additionalInfo;
                    if (service.promoID == null)
                    {
                        details.Add(new PurchaseItemDetail()
                        {
                            PriceID = (detail.netPriceID != null ? (long)detail.netPriceID : (long)detail.priceID),//PriceID = detail.priceID,
                            PriceTypeID = (int)detail.priceTypeID,
                            ExchangeRateID = detail.exchangeRateID,
                            Quantity = detail.quantity,
                            Price = (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price),

                            Unit = unit
                        });
                    }
                    else
                    {
                        if (service.tblPromos.applyOnPerson != true)
                        {
                            details.Add(new PurchaseItemDetail()
                            {
                                PriceID = (detail.netPriceID != null ? (long)detail.netPriceID : (long)detail.priceID),
                                PriceTypeID = (int)detail.priceTypeID,
                                ExchangeRateID = detail.exchangeRateID,
                                Quantity = detail.quantity,
                                Price = (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price),
                                Unit = unit
                            });
                        }
                        else
                        {
                            if (detail.promo)
                            {
                                promoPriceID = (detail.netPriceID != null ? (long)detail.netPriceID : (long)detail.priceID);
                            }
                            else
                            {
                                details.Add(new PurchaseItemDetail()
                                {
                                    PriceID = (detail.netPriceID != null ? (long)detail.netPriceID : (long)detail.priceID),
                                    PriceTypeID = (int)detail.priceTypeID,
                                    ExchangeRateID = detail.exchangeRateID,
                                    Quantity = (promoPriceID == detail.priceID ? detail.quantity + 1 : detail.quantity),
                                    Price = (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price),
                                    Unit = unit
                                });
                            }
                        }
                    }

                }
                item.Details = details;

                items.Add(item);
            }

            return items;
        }

        public static string GetAuthCode(Guid purchaseID)
        {
            string code = "";
            ePlatEntities db = new ePlatEntities();
            var codeQ = from c in db.tblPurchases
                        where c.purchaseID == purchaseID
                        select c.tblPaymentDetails.OrderByDescending(x => x.dateSaved).FirstOrDefault().tblMoneyTransactions.authCode;

            if (codeQ.Count() > 0)
            {
                code = codeQ.FirstOrDefault();
            }

            return code;
        }

        public static string GetTerms()
        {
            ePlatEntities db = new ePlatEntities();
            string terms = "";
            long terminalid = Utils.GeneralFunctions.GetTerminalID();
            string culture = Utils.GeneralFunctions.GetCulture();
            var TermsQ = from t in db.tblBlockDescriptions
                         where t.tblBlocks.block == "Terms and Conditions"
                         && t.tblBlocks.terminalID == terminalid
                         && t.culture == culture
                         select t;

            if (TermsQ.Count() > 0)
            {
                terms = TermsQ.First().content_;
            }

            return terms;
        }

        public static string GetTermsByPointOfSale(int pointOfSaleID)
        {
            ePlatEntities db = new ePlatEntities();
            string terms = "";
            long terminalid = Utils.GeneralFunctions.GetTerminalID();
            string culture = Utils.GeneralFunctions.GetCulture();
            var BlockID = (from p in db.tblPointsOfSale
                           where p.pointOfSaleID == pointOfSaleID
                           select p.policiesBlockID).FirstOrDefault();

            var TermsQ = from t in db.tblBlockDescriptions
                         where t.blockID == BlockID
                         && t.culture == culture
                         select t;

            if (TermsQ.Count() > 0)
            {
                terms = TermsQ.FirstOrDefault().content_;
            }

            return terms;
        }

        public static CartSettingsModel GetCartSettings()
        {
            CartSettingsModel model = new CartSettingsModel();
            ePlatEntities db = new ePlatEntities();

            //obtener terminal a partir del punto de venta
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            model.TerminalID = terminalID;

            var CatalogID = (from c in db.tblCatalogs_Terminals
                             where c.terminalID == terminalID
                             && c.tblCatalogs.catalog == "Activities"
                             select c.catalogID).FirstOrDefault();

            if (CatalogID != null)
            {
                model.CatalogID = CatalogID;
            }


            //obtener el punto de venta a partir del dominio
            model.PointOfSaleID = GetPointOfSaleID(terminalID);

            return model;
        }

        public static int GetPointOfSaleID(long? terminalID)
        {
            ePlatEntities db = new ePlatEntities();
            //obtener dominio
            string domain = System.Web.HttpContext.Current.Request.Url.Host;
            domain = domain.Replace("www.", "").Replace("mx.", "").Replace(".mx", "");

            int pointOfSaleID = 0;
            if (domain.IndexOf("localhost") >= 0)
            {
                var PointOfSaleQ = (from p in db.tblPointsOfSale
                                    where p.terminalID == terminalID
                                    && p.localTest == true
                                    select p.pointOfSaleID).FirstOrDefault();

                if (PointOfSaleQ != 0)
                {
                    pointOfSaleID = PointOfSaleQ;
                }
            }
            else
            {
                var PointOfSaleQ = (from p in db.tblPointsOfSale
                                    where p.terminalID == terminalID
                                    && p.online == true
                                    && p.domain.Contains(domain)
                                    select p.pointOfSaleID).FirstOrDefault();

                if (PointOfSaleQ != 0)
                {
                    pointOfSaleID = PointOfSaleQ;
                }
                else
                {
                    var DefaultPointOfSaleQ = (from p in db.tblPointsOfSale
                                               where p.terminalID == terminalID
                                               && p.online == true
                                               select p.pointOfSaleID).FirstOrDefault();

                    pointOfSaleID = DefaultPointOfSaleQ;
                }
            }
            return pointOfSaleID;
        }

        public static PaymentsProvider GetPaymentsProvider()
        {
            PaymentsProvider provider = new PaymentsProvider();
            long terminalid = Utils.GeneralFunctions.GetTerminalID();
            string cultura = Utils.GeneralFunctions.GetCulture();
            int currencyid = (cultura == "es-MX" ? 2 : 1);

            ePlatEntities db = new ePlatEntities();

            var ProviderQ = (from p in db.tblMerchantAccountSettings
                             where p.terminalID == terminalid
                             && p.tblMerchantAccounts.currencyID == currencyid
                             select new
                             {
                                 p.tblMerchantAccounts.merchantProvider,
                                 p.tblMerchantAccounts.merchantAccount,
                                 p.tblMerchantAccounts.merchantID
                             }).FirstOrDefault();

            if (ProviderQ != null)
            {
                provider.ProviderName = ProviderQ.merchantProvider;
                provider.ProviderAccount = ProviderQ.merchantAccount;
                provider.MerchantID = ProviderQ.merchantID;
            }

            return provider;
        }

        public static ConfirmationPageViewModel GetPurchaseConfirmation(Guid purchaseID)
        {
            ConfirmationPageViewModel confirmation = new ConfirmationPageViewModel();
            //Page Properties
            string culture = Utils.GeneralFunctions.GetCulture();
            confirmation.Culture = culture.Substring(0, 2).ToLower();
            DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
            if (objMaster != null)
            {
                confirmation.Scripts_Header = objMaster.Scripts_Header;
                confirmation.Scripts_AfterBody = objMaster.Scripts_AfterBody;
                confirmation.Scripts_Footer = objMaster.Scripts_Footer;
                confirmation.Template_Header = objMaster.Template_Header;
                confirmation.Template_Footer = objMaster.Template_Footer;
                confirmation.Template_Logo = objMaster.Template_Logo;
                confirmation.Template_Phone_Desktop = objMaster.Template_Phone_Desktop;
                confirmation.Template_Phone_Mobile = objMaster.Template_Phone_Mobile;
            }
            SubmenusViewModel objSubmenus = PageDataModel.GetSubmenus();
            if (objSubmenus != null)
            {
                confirmation.Submenu1 = objSubmenus.Submenu1;
                confirmation.Submenu2 = objSubmenus.Submenu2;
                confirmation.Submenu3 = objSubmenus.Submenu3;
            }
            confirmation.Seo_Title = "Confirmation Page - My Experience Tours";
            confirmation.Seo_Follow = "nofollow";
            confirmation.Seo_Index = "noindex";
            confirmation.Seo_FriendlyUrl = "/Purchase/Confirmation/" + purchaseID.ToString();
            confirmation.CanonicalDomain = ePlatBack.Models.Utils.GeneralFunctions.GetCanonicalDomain();

            //Purchase Properties
            confirmation.PurchaseID = purchaseID;

            ePlatEntities db = new ePlatEntities();
            var PurchaseQ = (from p in db.tblPurchases
                             join paymentDetail in db.tblPaymentDetails on p.purchaseID equals paymentDetail.purchaseID
                             into purchase_payments
                             from paymentDetail in purchase_payments.DefaultIfEmpty()
                             join transaction in db.tblMoneyTransactions on paymentDetail.moneyTransactionID equals transaction.moneyTransactionID
                             into payment_transaction
                             from transaction in payment_transaction.DefaultIfEmpty()
                             join merchant in db.tblMerchantAccounts on transaction.merchantAccountID equals merchant.merchantAccountID
                             into transaction_merchant
                             from merchant in transaction_merchant.DefaultIfEmpty()
                             join currency in db.tblCurrencies on p.currencyID equals currency.currencyID
                             into purchase_currency
                             from currency in purchase_currency.DefaultIfEmpty()
                             where p.purchaseID == purchaseID
                             select new
                             {
                                 transaction.authCode,
                                 p.total,
                                 currency.currencyCode,
                                 merchant.merchantAccountBillingName
                             }).FirstOrDefault();

            if (PurchaseQ != null)
            {
                confirmation.AuthCode = PurchaseQ.authCode;
                confirmation.Total = PurchaseQ.total;
                confirmation.CurrencyCode = PurchaseQ.currencyCode;
                confirmation.MerchantName = PurchaseQ.merchantAccountBillingName;
            }

            return confirmation;
        }
    }
}
