using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ePlatBack.Models.DataModels
{
    public class CouponDataModel
    {
        ePlatEntities db = new ePlatEntities();
        public static UserSession session = new UserSession();
        public CouponViewModel GetCoupon(string coupon)
        {
            return GetPrintableCoupon(coupon);
        }

        public CouponViewModel GetPrintableCoupon(string coupon)
        {
            CouponViewModel couponModel = new CouponViewModel();
            if (coupon.Length > 38)
            {
                Guid purchaseID = new Guid(coupon.Substring(0, 36));
                long purchase_ServiceID = Convert.ToInt64(coupon.Substring(37));

                var serviceCoupon = (from s in db.tblPurchases_Services
                                     where s.purchase_ServiceID == purchase_ServiceID
                                     && s.purchaseID == purchaseID
                                     select s).FirstOrDefault();

                if (serviceCoupon != null)
                {
                    string culture = serviceCoupon.tblPurchases.culture;
                    couponModel.PrintCounter = serviceCoupon.printCounter;
                    serviceCoupon.printCounter += 1;
                    db.SaveChanges();
                    couponModel.TerminalID = serviceCoupon.tblPurchases.terminalID.ToString();
                    couponModel.PurchaseID = serviceCoupon.purchaseID;
                    couponModel.ItemID = serviceCoupon.purchase_ServiceID;
                    couponModel.Type = (serviceCoupon.tblServices.transportationService ? 3 : 1);
                    couponModel.Website = serviceCoupon.tblPurchases.tblTerminals.terminal;
                    if (couponModel.Operator == null)
                    {
                        couponModel.Operator = ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Is_operated_by + " " + serviceCoupon.tblPurchases.tblTerminals.tblCompanies.company;
                    }
                    if (serviceCoupon.tblPurchases.tblTerminals.tblCompanies.rfc != null)
                    {
                        couponModel.Operator += "</br>" + serviceCoupon.tblPurchases.tblTerminals.tblCompanies.rfc;
                    }
                    couponModel.Destination = serviceCoupon.tblServices.tblDestinations.destination;
                    couponModel.CouponReference = serviceCoupon.couponReference;
                    couponModel.CouponNumber = (serviceCoupon.tblPurchaseServiceDetails.First().coupon != null ? serviceCoupon.tblPurchaseServiceDetails.First().coupon.Substring(0, serviceCoupon.tblPurchaseServiceDetails.First().coupon.IndexOf("-")) : "");
                    couponModel.GuestName = serviceCoupon.tblPurchases.tblLeads.firstName + " " + serviceCoupon.tblPurchases.tblLeads.lastName;
                    couponModel.ReservedFor = serviceCoupon.reservedFor;
                    if (serviceCoupon.tblPurchases.tblPaymentDetails.Count(x => x.tblMoneyTransactions.errorCode == "0" && (x.deleted == null || x.deleted == false)) > 0)
                    {
                        couponModel.FirstAuthCode = serviceCoupon.tblPurchases.tblPaymentDetails.FirstOrDefault(x => x.tblMoneyTransactions.errorCode == "0" && (x.deleted == null || x.deleted == false)).tblMoneyTransactions.authCode;
                    }

                    /*Domain for policies*/
                    //buscar por punto de venta
                    var domainForPos = (from p in db.tblPointsOfSale
                                        where p.pointOfSaleID == serviceCoupon.tblPurchases.pointOfSaleID
                                        select p.domain).FirstOrDefault();

                    if (domainForPos != null)
                    {
                        couponModel.WebsiteUrl = domainForPos;
                        var LogoQ = (from d in db.tblTerminalDomains
                                     where d.terminalID == serviceCoupon.tblPurchases.terminalID
                                     && d.culture == serviceCoupon.tblPurchases.culture
                                     && d.domain.Contains(domainForPos)
                                     select new { d.logo, d.operatedBy }).FirstOrDefault();
                        if (LogoQ != null)
                        {
                            couponModel.Logo = LogoQ.logo;
                            if (LogoQ.operatedBy != null)
                            {
                                couponModel.Operator = ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Is_operated_by + " " + LogoQ.operatedBy;
                            }
                        }
                    }
                    else
                    {
                        //si no hay por punto, obtener el default
                        var DomainQ = (from d in db.tblTerminalDomains
                                       where d.terminalID == serviceCoupon.tblPurchases.terminalID
                                       && d.culture == serviceCoupon.tblPurchases.culture
                                       && d.main == true
                                       select new { d.domain, d.logo, d.operatedBy }).FirstOrDefault();

                        if (DomainQ != null)
                        {
                            couponModel.WebsiteUrl = DomainQ.domain;
                            couponModel.Logo = DomainQ.logo;
                            if (DomainQ.operatedBy != null)
                            {
                                couponModel.Operator = ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Is_operated_by + " " + DomainQ.operatedBy;
                            }
                        }
                        else
                        {
                            couponModel.WebsiteUrl = System.Web.HttpContext.Current.Request.Url.Host;
                            couponModel.Logo = serviceCoupon.tblPurchases.tblTerminals.tblTerminalDomains.FirstOrDefault(x => !x.domain.Contains("localhost")).logo;
                        }
                    }
                    if (couponModel.Logo != null)
                    {
                        couponModel.Logo = couponModel.Logo.Replace("_white", "");
                    }

                    //BEG PackageServices

                    //verificar si el cupón tiene promo de paquete
                    bool applyPackage = false;
                    couponModel.PackageServices = new List<CouponServiceInfo>();
                    if (serviceCoupon.promoID != null && serviceCoupon.tblPromos.isPackage && (serviceCoupon.serviceStatusID == 3 || serviceCoupon.serviceStatusID == 6))
                    {
                        long promoID = (long)serviceCoupon.promoID;
                        //buscar otros cupones en la compra con la misma promo
                        var packageServices = from p in serviceCoupon.tblPurchases.tblPurchases_Services
                                              where p.promoID == promoID
                                              && (p.serviceStatusID == 3 || p.serviceStatusID == 6)
                                              orderby p.serviceDateTime
                                              select p;

                        if (packageServices.Count() > 1)
                        {
                            applyPackage = true;
                            foreach (var packageCoupon in packageServices)
                            {
                                if (packageCoupon.tblPurchaseServiceDetails.FirstOrDefault().coupon.Contains(couponModel.CouponNumber))
                                {
                                    CouponServiceInfo serviceInfo = new CouponServiceInfo();
                                    //BEG Service Info
                                    if (packageCoupon.tblServices.tblServiceDescriptions.Count(x => x.culture == culture.ToString()) > 0)
                                    {
                                        serviceInfo.ActivityName = packageCoupon.tblServices.tblServiceDescriptions.FirstOrDefault(x => x.culture == culture.ToString()).service;
                                    }
                                    else
                                    {
                                        serviceInfo.ActivityName = packageCoupon.tblServices.service;
                                    }

                                    if (packageCoupon.openCouponMonths == 0)
                                    {
                                        serviceInfo.ActivityDateAndSchedule = packageCoupon.serviceDateTime.ToString("yyyy-MM-dd");
                                    }
                                    else
                                    {
                                        serviceInfo.ActivityDateAndSchedule = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Valid_until + " " + packageCoupon.serviceDateTime.AddMonths(packageCoupon.openCouponMonths).ToString("yyyy-MM-dd");
                                    }

                                    foreach (tblPurchaseServiceDetails detail in packageCoupon.tblPurchaseServiceDetails)
                                    {
                                        if (serviceInfo.Units != null)
                                        {
                                            serviceInfo.Units += "<br />";
                                        }
                                        var unit = PriceDataModel.GetUnit((detail.priceID != null ? (long)detail.priceID : (long)detail.netPriceID), culture.ToString());
                                        serviceInfo.Units += detail.quantity + " " + unit.unit + " " + unit.additionalInfo;
                                    };

                                    if (!packageCoupon.tblServices.transportationService)
                                    {
                                        //activity
                                        if (packageCoupon.openCouponMonths == 0)
                                        {
                                            if (packageCoupon.meetingPointID != null)
                                            {
                                                if (packageCoupon.tblMeetingPoints.atYourHotel)
                                                {
                                                    serviceInfo.MeetingPoint = "At Your Hotel";
                                                }
                                                else
                                                {
                                                    serviceInfo.MeetingPoint = packageCoupon.tblMeetingPoints.tblPlaces.place + "<br />" + packageCoupon.tblMeetingPoints.tblPlaces.address;
                                                }
                                            }
                                            else
                                            {
                                                serviceInfo.MeetingPoint = packageCoupon.customMeetingPlace;
                                            }

                                            string hour = "";
                                            if (packageCoupon.customMeetingTime != null)
                                            {
                                                hour = Convert.ToString((TimeSpan)packageCoupon.customMeetingTime);
                                            }
                                            else
                                            {
                                                if (packageCoupon.meetingPointID != null)
                                                {
                                                    hour = Convert.ToString((TimeSpan)packageCoupon.tblMeetingPoints.hour);
                                                }
                                            }
                                            serviceInfo.MeetingTime = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour);
                                        }
                                        else
                                        {
                                            serviceInfo.MeetingPoint = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Open;
                                            serviceInfo.MeetingTime = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Open;
                                        }
                                    }
                                    else
                                    {
                                        //transportation
                                        if (packageCoupon.tblServices.tblMeetingPoints.Count() > 0)
                                        {
                                            //mike
                                            if (!packageCoupon.tblServices.tblMeetingPoints.FirstOrDefault().atYourHotel)
                                            {
                                                serviceInfo.MeetingPoint = packageCoupon.tblServices.tblMeetingPoints.FirstOrDefault().tblPlaces.place;
                                            }
                                            else
                                            {
                                                serviceInfo.MeetingPoint = "At Your Hotel";
                                            }
                                        }
                                        else
                                        {
                                            serviceInfo.MeetingPoint = ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Airport;
                                        }
                                        string hour = "";
                                        if (packageCoupon.customMeetingTime != null)
                                        {
                                            hour = Convert.ToString((TimeSpan)packageCoupon.customMeetingTime);
                                            serviceInfo.MeetingTime = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour);
                                        }
                                        else
                                        {
                                            if (packageCoupon.tblMeetingPoints.hour != null)
                                            {
                                                hour = Convert.ToString((TimeSpan)packageCoupon.customMeetingTime);
                                                serviceInfo.MeetingTime = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour);
                                            }
                                            else
                                            {
                                                serviceInfo.MeetingTime = "UNKNOWN";
                                            }
                                        }
                                    }
                                    serviceInfo.CouponNotes = packageCoupon.note;

                                    couponModel.PackageServices.Add(serviceInfo);
                                }
                            }
                        }
                    }

                    //definir si se usa Package Services o no
                    if (applyPackage == false)
                    {
                        //BEG Service Info
                        if (serviceCoupon.tblServices.tblServiceDescriptions.Count(x => x.culture == culture.ToString()) > 0)
                        {
                            couponModel.ActivityName = serviceCoupon.tblServices.tblServiceDescriptions.FirstOrDefault(x => x.culture == culture.ToString()).service;
                        }
                        else
                        {
                            couponModel.ActivityName = serviceCoupon.tblServices.service;
                        }

                        if (serviceCoupon.promoID != null)
                        {
                            couponModel.ActivityName += " (PROMO)";
                        }

                        if (serviceCoupon.openCouponMonths == 0)
                        {
                            couponModel.ActivityDateAndSchedule = serviceCoupon.serviceDateTime.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            couponModel.ActivityDateAndSchedule = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Valid_until + " " + serviceCoupon.serviceDateTime.AddMonths(serviceCoupon.openCouponMonths).ToString("yyyy-MM-dd");
                        }

                        foreach (tblPurchaseServiceDetails detail in serviceCoupon.tblPurchaseServiceDetails)
                        {
                            if (couponModel.Units != null)
                            {
                                couponModel.Units += "<br />";
                            }
                            var unit = PriceDataModel.GetUnit((detail.priceID != null ? (long)detail.priceID : (long)detail.netPriceID), culture.ToString());
                            couponModel.Units += detail.quantity + " " + unit.unit + " " + unit.additionalInfo;
                            if (detail.promo)
                            {
                                couponModel.Units += " (PROMO)";
                            }
                        };

                        if (couponModel.Type == 1)
                        {
                            //activity

                            if (serviceCoupon.meetingPointID != null)
                            {
                                if (serviceCoupon.tblMeetingPoints.atYourHotel)
                                {
                                    couponModel.MeetingPoint = "At Your Hotel";
                                }
                                else
                                {
                                    couponModel.MeetingPoint = serviceCoupon.tblMeetingPoints.tblPlaces.place + "<br />" + serviceCoupon.tblMeetingPoints.tblPlaces.address;
                                }
                            }
                            else
                            {
                                couponModel.MeetingPoint = serviceCoupon.customMeetingPlace;
                            }

                            string hour = "";
                            if (serviceCoupon.customMeetingTime != null)
                            {
                                hour = Convert.ToString((TimeSpan)serviceCoupon.customMeetingTime);
                            }
                            else
                            {
                                if (serviceCoupon.meetingPointID != null)
                                {
                                    hour = Convert.ToString((TimeSpan)serviceCoupon.tblMeetingPoints.hour);
                                }
                            }
                            couponModel.MeetingTime = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour);
                            if (serviceCoupon.openCouponMonths != 0)
                            {
                                if (couponModel.MeetingPoint == null || couponModel.MeetingPoint == "")
                                {
                                    couponModel.MeetingPoint = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Open;
                                }
                                if (couponModel.MeetingTime == null || couponModel.MeetingTime == "")
                                {
                                    couponModel.MeetingTime = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Open;
                                }
                            }
                        }
                        else
                        {
                            //transportation
                            if (serviceCoupon.tblServices.tblMeetingPoints.Count() > 0)
                            {
                                //mike
                                if (!serviceCoupon.tblServices.tblMeetingPoints.FirstOrDefault().atYourHotel)
                                {
                                    couponModel.MeetingPoint = serviceCoupon.tblServices.tblMeetingPoints.FirstOrDefault().tblPlaces.place;
                                }
                                else
                                {
                                    couponModel.MeetingPoint = "At Your Hotel";
                                }
                            }
                            else
                            {
                                couponModel.MeetingPoint = ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Airport;
                            }
                            string hour = "";
                            if (serviceCoupon.customMeetingTime != null)
                            {
                                hour = Convert.ToString((TimeSpan)serviceCoupon.customMeetingTime);
                                couponModel.MeetingTime = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour);
                            }
                            else
                            {
                                if (serviceCoupon.meetingPointID != null && serviceCoupon.tblMeetingPoints.hour != null)
                                {
                                    hour = Convert.ToString((TimeSpan)serviceCoupon.tblMeetingPoints.hour);
                                    couponModel.MeetingTime = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour);
                                }
                                else
                                {
                                    couponModel.MeetingTime = "UNKNOWN";
                                }
                            }
                        }
                        couponModel.CouponNotes = serviceCoupon.note;
                        //END Service Info
                    }
                    //END PackageServices

                    couponModel.Provider = serviceCoupon.tblServices.tblProviders.comercialName;
                    couponModel.ProviderPhone = serviceCoupon.tblServices.tblProviders.phone1;
                    couponModel.ConfirmationNumber = serviceCoupon.confirmationNumber;

                    var TerminalQ = (from t in db.tblTerminals
                                     where t.terminalID == serviceCoupon.tblPurchases.terminalID
                                     select new
                                     {
                                         t.couponUseDomainPhone,
                                         t.couponUseProviderPhone,
                                         t.couponUseTerminalPhone,
                                         t.largeCouponPhone,
                                         t.shortCouponPhone
                                     }).FirstOrDefault();

                    if (TerminalQ != null)
                    {
                        if (TerminalQ.couponUseDomainPhone)
                        {
                            couponModel.PhoneNumbers = serviceCoupon.tblPurchases.tblTerminals.tblTerminalDomains.FirstOrDefault().phoneUS;
                        }
                        else if (TerminalQ.couponUseTerminalPhone)
                        {
                            couponModel.PhoneNumbers = TerminalQ.largeCouponPhone;
                            couponModel.PhoneNumbersShortCoupon = TerminalQ.shortCouponPhone;
                        }
                        else if (TerminalQ.couponUseProviderPhone)
                        {
                            couponModel.PhoneNumbers = serviceCoupon.tblServices.tblProviders.phone1;
                            couponModel.PhoneNumbersShortCoupon = serviceCoupon.tblServices.tblProviders.phone1;
                        }
                    }
                    else
                    {
                        couponModel.PhoneNumbers = serviceCoupon.tblPurchases.tblTerminals.tblTerminalDomains.FirstOrDefault().phoneUS;
                    }

                    if (serviceCoupon.confirmationDateTime != null)
                    {
                        couponModel.PurchaseDate = serviceCoupon.confirmationDateTime.Value.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("AM", "a.m.").Replace("PM", "p.m.");
                        //exchange rate
                        //couponModel.ExchangeRate = MasterChartDataModel.Purchases.GetSpecificRate(serviceCoupon.confirmationDateTime, serviceCoupon.tblCurrencies.currencyCode, serviceCoupon.tblPurchases.terminalID);
                        couponModel.ExchangeRate = MasterChartDataModel.Purchases.GetSpecificRate(serviceCoupon.confirmationDateTime, serviceCoupon.tblCurrencies.currencyCode, serviceCoupon.tblPurchases.terminalID, serviceCoupon.tblPurchases.pointOfSaleID);
                    }
                    else
                    {
                        couponModel.PurchaseDate = serviceCoupon.tblPurchases.purchaseDateTime.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("AM", "a.m.").Replace("PM", "p.m.");
                        //exchange rate
                        //couponModel.ExchangeRate = MasterChartDataModel.Purchases.GetSpecificRate(serviceCoupon.tblPurchases.purchaseDateTime, serviceCoupon.tblCurrencies.currencyCode, serviceCoupon.tblPurchases.terminalID);
                        couponModel.ExchangeRate = MasterChartDataModel.Purchases.GetSpecificRate(serviceCoupon.tblPurchases.purchaseDateTime, serviceCoupon.tblCurrencies.currencyCode, serviceCoupon.tblPurchases.terminalID, serviceCoupon.tblPurchases.pointOfSaleID);
                    }

                    if (couponModel.Type == 3)
                    {
                        couponModel.TravelInfo = ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Flight + " " + (serviceCoupon.flightNumber != null ? serviceCoupon.flightNumber : "")
                            + "<br>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Airline + " " + (serviceCoupon.airline != null ? serviceCoupon.airline : "");
                        if (serviceCoupon.tblPurchases.stayingAtPlaceID != null || serviceCoupon.tblPurchases.stayingAtPlaceID != null)
                        {
                            couponModel.TravelInfo += "<br>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Hotel + " " + (serviceCoupon.tblPurchases.stayingAtPlaceID == -1 ? serviceCoupon.tblPurchases.stayingAt : db.tblPlaces.FirstOrDefault(x => x.placeID == serviceCoupon.tblPurchases.stayingAtPlaceID).place);
                        }
                        couponModel.RoundTrip = serviceCoupon.round;
                        if (couponModel.RoundTrip == true)
                        {
                            if (serviceCoupon.roundDate != null)
                            {
                                couponModel.RoundDate = serviceCoupon.roundDate.Value.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                couponModel.RoundDate = "NOT SET";
                            }

                            string hour = "";
                            if (serviceCoupon.roundMeetingTime != null)
                            {
                                hour = Convert.ToString((TimeSpan)serviceCoupon.roundMeetingTime);
                                couponModel.RoundMeetingTime = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour);
                            }
                            else
                            {
                                couponModel.RoundMeetingTime = "UNKNOWN";
                            }

                            string hourFT = "";
                            if (serviceCoupon.roundFlightTime != null)
                            {
                                hourFT = Convert.ToString((TimeSpan)serviceCoupon.roundFlightTime);
                                couponModel.RoundFlightTime = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hourFT);
                            }
                            else
                            {
                                couponModel.RoundFlightTime = "UNKNOWN";
                            }

                            couponModel.RoundTravelInfo = ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Flight + " " + (serviceCoupon.roundFlightNumber != null ? serviceCoupon.roundFlightNumber : "")
                            + "<br>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Airline + " " + (serviceCoupon.roundAirline != null ? serviceCoupon.roundAirline : "")
                            + "<br>" +
                            ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Flight_Time + " " + couponModel.RoundFlightTime;
                            if (serviceCoupon.tblPurchases.stayingAtPlaceID != null || serviceCoupon.tblPurchases.stayingAtPlaceID != null)
                            {
                                couponModel.RoundHotel += (serviceCoupon.tblPurchases.stayingAtPlaceID == -1 ? serviceCoupon.tblPurchases.stayingAt : db.tblPlaces.FirstOrDefault(x => x.placeID == serviceCoupon.tblPurchases.stayingAtPlaceID).place);
                            }
                        }
                    }
                    tblServiceDescriptions description = serviceCoupon.tblServices.tblServiceDescriptions.FirstOrDefault(x => x.culture == culture.ToString());
                    if (description != null)
                    {
                        couponModel.Recommendations = description.recommendations;
                        if (couponModel.CouponNotes != null && couponModel.CouponNotes != "")
                        {
                            couponModel.CouponNotes += "<br />";
                        }
                        couponModel.CouponNotes += description.cancelationPolicies;
                        if (description.policies != null)
                        {
                            couponModel.Disclaimer = "<div class=\"policies-block\"><p>" + ePlatBack.Models.Resources.Models.Coupon.CouponStrings.Notes + "</p>" + description.policies + "</div>";
                        }
                    }

                    long? policiesBlockID = serviceCoupon.tblPurchases.tblPointsOfSale.policiesBlockID;
                    if (policiesBlockID != null)
                    {
                        couponModel.Disclaimer += BlockDataModel.GetBlockDescription((long)policiesBlockID);
                    }
                    if (serviceCoupon.tblPurchases.userID != null)
                    {
                        tblUserProfiles profile = new tblUserProfiles();
                        profile = db.tblUserProfiles.Single(p => p.userID == serviceCoupon.tblPurchases.userID);
                        couponModel.SalesAgent = profile.firstName + " " + profile.lastName;
                    }
                    else
                    {
                        couponModel.SalesAgent = "Self Generated";
                    }
                    couponModel.Status = serviceCoupon.tblPurchaseServiceStatus.purchaseServiceStatus;
                    couponModel.StatusID = serviceCoupon.serviceStatusID;
                    if (serviceCoupon.serviceStatusID == 4 || serviceCoupon.serviceStatusID == 5)
                    {
                        couponModel.Status += " (" + serviceCoupon.cancelationDateTime.Value.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("AM", "a.m.").Replace("PM", "p.m.") + (serviceCoupon.cancelationNumber != null ? " / " + serviceCoupon.cancelationNumber : "") + ")";
                    }
                }
            }
            return couponModel;
        }

        public CouponViewModel GetCouponFromCache(string coupon, string culture)
        {
            CouponViewModel couponModel = new CouponViewModel();
            if (coupon.Length > 38)
            {
                Guid purchaseID = new Guid(coupon.Substring(0, 36));
                long purchase_ServiceID = Convert.ToInt64(coupon.Substring(37));
                //DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();

                var serviceCoupon = (from s in db.tblCouponInfo
                                     where s.purchase_ServiceID == purchase_ServiceID
                                     && s.purchaseID == purchaseID
                                     select s).FirstOrDefault();

                if (serviceCoupon != null)
                {
                    if (culture == "")
                    {
                        culture = serviceCoupon.culture;
                    }

                    couponModel.TerminalID = serviceCoupon.terminalID.ToString();
                    couponModel.PurchaseID = serviceCoupon.purchaseID;
                    couponModel.ItemID = serviceCoupon.purchase_ServiceID;
                    couponModel.Website = serviceCoupon.terminal;
                    couponModel.GuestName = serviceCoupon.customerFirstName + " " + serviceCoupon.customerLastName;
                    couponModel.CouponNumber = serviceCoupon.folio;
                    couponModel.CouponReference = serviceCoupon.couponReference;
                    couponModel.ActivityName = serviceCoupon.item;
                    if (serviceCoupon.promo != null)
                    {
                        couponModel.ActivityName += " (PROMO)";
                    }

                    if (serviceCoupon.openCouponMonths == 0)
                    {
                        couponModel.ActivityDateAndSchedule = serviceCoupon.dateItem.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        couponModel.ActivityDateAndSchedule = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Valid_until + " " + serviceCoupon.dateItem.AddMonths(serviceCoupon.openCouponMonths).ToString("yyyy-MM-dd");
                    }
                    foreach (var detail in serviceCoupon.tblCouponInfoUnits)
                    {
                        if (couponModel.Units != null)
                        {
                            couponModel.Units += "<br />";
                        }
                        couponModel.Units += detail.quantity + " " + detail.unit;
                        if (detail.isPromo)
                        {
                            couponModel.Units += " (PROMO)";
                        }
                    };

                    couponModel.Provider = serviceCoupon.provider;

                    var providerCurrencyID = (from p in db.tblProviders
                                              where p.providerID == serviceCoupon.providerID
                                              select p.invoiceCurrencyID).FirstOrDefault();

                    if (providerCurrencyID == null)
                    {
                        couponModel.InvoiceCurrency = "Undefined";
                    }
                    else if (providerCurrencyID == 2)
                    {
                        couponModel.InvoiceCurrency = "Pesos";
                    }
                    else if (providerCurrencyID == 1)
                    {
                        couponModel.InvoiceCurrency = "Dollars";
                    }
                    couponModel.ConfirmationNumber = serviceCoupon.confirmationNumber;
                    couponModel.SalesAgent = serviceCoupon.purchaseBy;
                    couponModel.Status = serviceCoupon.status;
                    couponModel.StatusID = serviceCoupon.serviceStatusID;
                    if (serviceCoupon.serviceStatusID == 4 || serviceCoupon.serviceStatusID == 5)
                    {
                        couponModel.Status += " (" + serviceCoupon.dateCanceled.Value.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("AM", "a.m.").Replace("PM", "p.m.") + ")";
                    }

                    //closeout
                    if (serviceCoupon.paidCloseOutID != null)
                    {
                        couponModel.CloseOut += "<span class=\"block\">" + serviceCoupon.paidCloseOut + "</span>";
                    }

                    if (serviceCoupon.canceledCloseOutID != null)
                    {
                        couponModel.CloseOut += "<span class=\"block\">" + serviceCoupon.canceledCloseOut + "</span>";
                    }

                    //Purchase Date
                    if (serviceCoupon.dateConfirmed != null)
                    {
                        couponModel.PurchaseDate = serviceCoupon.dateConfirmed.Value.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("AM", "a.m.").Replace("PM", "p.m.");
                    }
                    else
                    {
                        couponModel.PurchaseDate = serviceCoupon.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("AM", "a.m.").Replace("PM", "p.m.");
                    }

                    //cost
                    if (serviceCoupon.serviceStatusID == 4 || serviceCoupon.serviceStatusID == 5)
                    {
                        couponModel.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount = 0;
                        couponModel.Cost.FirstOrDefault(x => x.Currency == "USD").Amount = 0;
                    }
                    else
                    {
                        couponModel.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount = serviceCoupon.costMXN; couponModel.Cost.FirstOrDefault(x => x.Currency == "USD").Amount = serviceCoupon.costUSD;
                    }

                    //audited
                    couponModel.Audited = serviceCoupon.audited;
                    couponModel.ProviderInvoiceID = serviceCoupon.providerInvoiceID;
                    if (couponModel.Audited)
                    {
                        couponModel.Audit = "Audited on " + serviceCoupon.auditedOnDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") + " by " + serviceCoupon.auditedBy + " Invoice " + serviceCoupon.auditedProviderInvoice;
                    }
                }
            }
            return couponModel;
        }

        public CouponViewModel GetCoupon(string coupon, string culture)
        {
            CouponViewModel couponModel = new CouponViewModel();
            if (coupon.Length > 38)
            {
                Guid purchaseID = new Guid(coupon.Substring(0, 36));
                long purchase_ServiceID = Convert.ToInt64(coupon.Substring(37));
                //DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();

                var serviceCoupon = (from s in db.tblPurchases_Services
                                     where s.purchase_ServiceID == purchase_ServiceID
                                     && s.purchaseID == purchaseID
                                     select new
                                     {
                                         s.tblPurchases.terminalID,
                                         s.tblPurchases.culture,
                                         s.purchaseID,
                                         s.purchase_ServiceID,
                                         s.serviceID,
                                         s.tblPurchases.pointOfSaleID,
                                         s.tblPurchases.tblTerminals.terminal,
                                         s.tblPurchaseServiceDetails.FirstOrDefault().coupon,
                                         s.reservedFor,
                                         s.tblServices.service,
                                         s.promoID,
                                         s.couponReference,
                                         s.openCouponMonths,
                                         s.serviceDateTime,
                                         s.total,
                                         guestFirstName = s.tblPurchases.tblLeads.firstName,
                                         guestLastName = s.tblPurchases.tblLeads.lastName,
                                         details = s.tblPurchaseServiceDetails.Select(p => new
                                         {
                                             p.purchaseServiceDetailID,
                                             p.priceID,
                                             p.netPriceID,
                                             p.quantity,
                                             p.promo,
                                             p.customCost,
                                             p.customCostAlt,
                                             p.customCostAltNoIVA,
                                             p.customCostNoIVA,
                                             p.tblPrices1.tblCurrencies.currencyCode
                                         }),
                                         provider = s.tblServices.tblProviders.comercialName,
                                         s.confirmationNumber,
                                         s.tblPurchases.userID,
                                         s.tblPurchaseServiceStatus.purchaseServiceStatus,
                                         s.serviceStatusID,
                                         s.cancelationDateTime,
                                         s.cancelationNumber,
                                         moneyTransactions = s.tblPurchases_Services_MoneyTransactions.Where(x => x.tblMoneyTransactions.errorCode == "0" && x.tblMoneyTransactions.tblPaymentDetails.Count(c => (c.deleted == null || c.deleted == false)) > 0).Select(m => new
                                         {
                                             m.tblMoneyTransactions.transactionDate,
                                             m.tblMoneyTransactions.tblMoneyTransactionTypes.moneyTransactionType,
                                             m.tblMoneyTransactions.tblPaymentDetails.FirstOrDefault(c => c.deleted == null || c.deleted == false).paymentType,
                                             m.tblMoneyTransactions.tblBillingInfo.tblCardTypes.cardType,
                                             m.tblMoneyTransactions.tblBillingInfo.cardNumber,
                                             cardType2 = m.tblMoneyTransactions.tblPaymentDetails.FirstOrDefault(c => c.deleted == null || c.deleted == false).tblCardTypes.cardType,
                                             m.tblMoneyTransactions.tblPaymentDetails.FirstOrDefault(c => c.deleted == null || c.deleted == false).ccReferenceNumber,
                                             m.tblMoneyTransactions.tblPaymentDetails.FirstOrDefault(c => c.deleted == null || c.deleted == false).amount,
                                             m.tblMoneyTransactions.tblPaymentDetails.FirstOrDefault(c => c.deleted == null || c.deleted == false).tblCurrencies.currencyCode,
                                             m.tblMoneyTransactions.authCode,
                                             m.tblMoneyTransactions.tblPaymentDetails.FirstOrDefault(c => c.deleted == null || c.deleted == false).paymentComments
                                         }),
                                         closeouts = s.tblCloseOuts_Purchases.Select(c => new
                                         {
                                             c.tblCloseOuts.tblPointsOfSale.shortName,
                                             c.tblCloseOuts.closeOutDate,
                                             c.paid
                                         }),
                                         s.confirmationDateTime,
                                         s.tblCurrencies.currencyCode,
                                         s.tblPurchases.purchaseDateTime,
                                         s.tblPurchases.currencyID,
                                         s.dateSaved,
                                         s.tblPurchases.tblPointsOfSale.online,
                                         s.tblServices.tblProviders.invoiceCurrencyID,
                                         s.cancelationCharge,
                                         s.audit,
                                         s.paidToProvider,
                                         paidByFirstname = s.aspnet_Users1.tblUserProfiles.FirstOrDefault().firstName,
                                         paidByLastname = s.aspnet_Users1.tblUserProfiles.FirstOrDefault().lastName,
                                         s.paidDate,
                                         s.providerInvoiceID,
                                         auditFirstName = s.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName,
                                         auditLastName = s.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName,
                                         s.auditDate,
                                         s.auditInvoice,
                                         s.tblPurchases.tblTerminals.tblTerminalDomains.FirstOrDefault(t => t.culture == culture && !t.domain.Contains("localhost") && !t.domain.Contains("beta")).domain,
                                         s.tblPromos
                                     }).FirstOrDefault();

                if (serviceCoupon != null)
                {
                    if (culture == "")
                    {
                        culture = serviceCoupon.culture;
                    }

                    couponModel.TerminalID = serviceCoupon.terminalID.ToString();
                    couponModel.PurchaseID = serviceCoupon.purchaseID;
                    couponModel.ItemID = serviceCoupon.purchase_ServiceID;
                    couponModel.Website = serviceCoupon.terminal;
                    couponModel.GuestName = serviceCoupon.guestFirstName + " " + serviceCoupon.guestLastName;
                    couponModel.CouponNumber = (serviceCoupon.coupon != null ? serviceCoupon.coupon.Substring(0, serviceCoupon.coupon.IndexOf("-")) : "");
                    couponModel.CouponReference = serviceCoupon.couponReference;
                    couponModel.ReservedFor = serviceCoupon.reservedFor;
                    couponModel.WebsiteUrl = (serviceCoupon.domain != null ? serviceCoupon.domain : System.Web.HttpContext.Current.Request.Url.Host);

                    couponModel.ActivityName = serviceCoupon.serviceID + " - " + serviceCoupon.service;

                    if (serviceCoupon.promoID != null)
                    {
                        couponModel.ActivityName += " (PROMO)";
                    }

                    if (serviceCoupon.openCouponMonths == 0)
                    {
                        couponModel.ActivityDateAndSchedule = serviceCoupon.serviceDateTime.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        couponModel.ActivityDateAndSchedule = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Valid_until + " " + serviceCoupon.serviceDateTime.AddMonths(serviceCoupon.openCouponMonths).ToString("yyyy-MM-dd");
                    }
                    foreach (var detail in serviceCoupon.details)
                    {
                        if (couponModel.Units != null)
                        {
                            couponModel.Units += "<br />";
                        }
                        var unit = PriceDataModel.GetUnit((detail.priceID != null ? (long)detail.priceID : (long)detail.netPriceID), culture.ToString());
                        couponModel.Units += detail.quantity + " " + unit.unit + " " + unit.additionalInfo;
                        if (detail.promo)
                        {
                            couponModel.Units += " (PROMO)";
                        }
                    };

                    couponModel.Provider = serviceCoupon.provider;
                    if (serviceCoupon.invoiceCurrencyID == null)
                    {
                        couponModel.InvoiceCurrency = "Undefined";
                    }
                    else if (serviceCoupon.invoiceCurrencyID == 2)
                    {
                        couponModel.InvoiceCurrency = "Pesos";
                    }
                    else if (serviceCoupon.invoiceCurrencyID == 1)
                    {
                        couponModel.InvoiceCurrency = "Dollars";
                    }
                    couponModel.ConfirmationNumber = serviceCoupon.confirmationNumber;

                    if (serviceCoupon.userID != null)
                    {
                        tblUserProfiles profile = new tblUserProfiles();
                        profile = db.tblUserProfiles.Single(p => p.userID == serviceCoupon.userID);
                        couponModel.SalesAgent = profile.firstName + " " + profile.lastName;
                    }
                    else
                    {
                        couponModel.SalesAgent = "Self Generated";
                    }
                    couponModel.Status = serviceCoupon.purchaseServiceStatus;
                    couponModel.StatusID = serviceCoupon.serviceStatusID;
                    if (serviceCoupon.serviceStatusID == 4 || serviceCoupon.serviceStatusID == 5)
                    {
                        couponModel.Status += " (" + serviceCoupon.cancelationDateTime.Value.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("AM", "a.m.").Replace("PM", "p.m.") + (serviceCoupon.cancelationNumber != null ? " / " + serviceCoupon.cancelationNumber : "") + ")";
                    }

                    if (serviceCoupon.cancelationCharge != null && serviceCoupon.cancelationCharge > 0)
                    {
                        couponModel.CancelationCharge = (decimal)serviceCoupon.cancelationCharge;
                    }

                    //payments [paymenttype $00 USD transactionid]
                    foreach (var transaction in serviceCoupon.moneyTransactions.OrderBy(o => o.transactionDate))
                    {
                        couponModel.Payment += "<span class=\"block\">"
                            + (transaction.moneyTransactionType) + " "
                            + Utils.GeneralFunctions.PaymentTypes[transaction.paymentType.ToString()]
                            + (transaction.paymentType == 2 ?
                            " (" + (transaction.cardType != null ?
                            transaction.cardType + " "
                            + mexHash.mexHash.DecryptString(transaction.cardNumber).Substring(12)
                            : (transaction.cardType2 != null ? transaction.cardType2 : "") + " "
                            + transaction.ccReferenceNumber)
                            + ")" : "")
                            + " $" + transaction.amount + " "
                            + transaction.currencyCode
                            + (transaction.authCode != "" && transaction.authCode != "0" ?
                            " " + transaction.authCode : "")
                            + (transaction.paymentComments != null ?
                            "<br>[NOTE: " + transaction.paymentComments + "]" : "")
                            + "</span>";
                    }

                    //closeout
                    foreach (var closeout in serviceCoupon.closeouts)
                    {
                        couponModel.CloseOut += "<span class=\"block\">" + closeout.shortName + " " + closeout.closeOutDate.ToString("yyyy-MM-dd") + (closeout.paid ? " paid" : " canceled") + "</span>";
                    }

                    //Purchase Date
                    if (serviceCoupon.confirmationDateTime != null)
                    {
                        couponModel.PurchaseDate = serviceCoupon.confirmationDateTime.Value.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("AM", "a.m.").Replace("PM", "p.m.");
                    }
                    else
                    {
                        couponModel.PurchaseDate = serviceCoupon.purchaseDateTime.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("AM", "a.m.").Replace("PM", "p.m.");
                    }

                    //cost
                    if (serviceCoupon.details.Count() > 0 && serviceCoupon.details.FirstOrDefault().priceID == null)
                    {
                        decimal computedTotalMXN = 0;
                        decimal computedTotalUSD = 0;

                        //List<ComputedPriceModel> ComputedPrices = ComputedPrices = PriceDataModel.GetComputedPrices(serviceCoupon.serviceID, serviceCoupon.serviceDateTime, serviceCoupon.terminalID, serviceCoupon.online, serviceCoupon.dateSaved, serviceCoupon.culture);
                        List<ComputedPriceModel> ComputedPrices = PriceDataModel.GetComputedPrices(serviceCoupon.serviceID, serviceCoupon.serviceDateTime, serviceCoupon.pointOfSaleID, serviceCoupon.terminalID, serviceCoupon.dateSaved, serviceCoupon.culture);

                        //couponModel.CostCurrencyCode = "MXN";
                        couponModel.Cost = ReportDataModel.GetDefaultListOfMoney(false);
                        decimal partialCostUSD = 0;
                        decimal partialCostMXN = 0;

                        foreach (var detail in serviceCoupon.details)
                        {
                            long cPriceID = (long)detail.netPriceID;
                            partialCostUSD = 0;
                            partialCostMXN = 0;

                            if (detail.customCost != null)
                            {
                                if (detail.currencyCode == "USD")
                                {
                                    computedTotalUSD += (decimal)detail.customCost;
                                    computedTotalMXN += (decimal)detail.customCostAlt;
                                }
                                else
                                {
                                    computedTotalMXN += (decimal)detail.customCost;
                                    computedTotalUSD += (decimal)detail.customCostAlt;
                                }
                            }
                            else
                            {
                                if (serviceCoupon.serviceStatusID == 3 || serviceCoupon.serviceStatusID == 6)
                                {
                                    //USD
                                    if (ComputedPrices.Count(p => p.CurrencyID == 1 && p.PriceID == cPriceID && p.IsCost == true && p.PromoID == serviceCoupon.promoID) > 0)
                                    {
                                        partialCostUSD = ComputedPrices.FirstOrDefault(p => p.CurrencyID == 1 && p.PriceID == cPriceID && p.IsCost == true && p.PromoID == serviceCoupon.promoID).Price;
                                    }
                                    else if (ComputedPrices.Count(p => p.CurrencyID == 1 && p.PriceID == cPriceID && p.IsCost == true && p.PromoID == null) > 0)
                                    {
                                        partialCostUSD = ComputedPrices.FirstOrDefault(p => p.CurrencyID == 1 && p.PriceID == cPriceID && p.IsCost == true && p.PromoID == null).Price;
                                    }

                                    //MXN
                                    if (ComputedPrices.Count(p => p.CurrencyID == 2 && p.PriceID == cPriceID && p.IsCost == true && p.PromoID == serviceCoupon.promoID) > 0)
                                    {
                                        partialCostMXN = ComputedPrices.FirstOrDefault(p => p.CurrencyID == 2 && p.PriceID == cPriceID && p.IsCost == true && p.PromoID == serviceCoupon.promoID).Price;
                                    }
                                    else if (ComputedPrices.Count(p => p.CurrencyID == 2 && p.PriceID == cPriceID && p.IsCost == true && p.PromoID == null) > 0)
                                    {
                                        partialCostMXN = ComputedPrices.FirstOrDefault(p => p.CurrencyID == 2 && p.PriceID == cPriceID && p.IsCost == true && p.PromoID == null).Price;
                                    }

                                    //USD
                                    computedTotalUSD += (serviceCoupon.tblPromos != null && serviceCoupon.tblPromos.applyToCost ? PromoDataModel.ApplyPromo(detail.quantity * partialCostUSD, detail.purchaseServiceDetailID) : detail.quantity * partialCostUSD);
                                    //MXN
                                    computedTotalMXN += (serviceCoupon.tblPromos != null && serviceCoupon.tblPromos.applyToCost ? PromoDataModel.ApplyPromo(detail.quantity * partialCostMXN, detail.purchaseServiceDetailID) : detail.quantity * partialCostMXN);
                                }
                            }
                        }

                        couponModel.CustomCost = (serviceCoupon.details.Count(x => x.customCost != null) > 0 ? true : false);

                        //si el cargo por cancelacion es mayor al costo, usar el costo         

                        //if (serviceCoupon.serviceStatusID > 3 && serviceCoupon.cancelationCharge != null && serviceCoupon.cancelationCharge > 0)
                        //{
                        //    if (serviceCoupon.serviceStatusID == 6 )
                        //    {
                        //        //no show
                        //        couponModel.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount = computedTotalMXN;
                        //        couponModel.Cost.FirstOrDefault(x => x.Currency == "USD").Amount = computedTotalUSD;
                        //    }
                        //    else
                        //    {
                        //        couponModel.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount = 0;
                        //        couponModel.Cost.FirstOrDefault(x => x.Currency == "USD").Amount = 0;
                        //    }
                        //}
                        //else
                        //{
                        couponModel.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount = computedTotalMXN;
                        couponModel.Cost.FirstOrDefault(x => x.Currency == "USD").Amount = computedTotalUSD;
                        //}


                        couponModel.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount = decimal.Round(couponModel.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount, 2, MidpointRounding.AwayFromZero);
                        couponModel.Cost.FirstOrDefault(x => x.Currency == "USD").Amount = decimal.Round(couponModel.Cost.FirstOrDefault(x => x.Currency == "USD").Amount, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        if (serviceCoupon.serviceStatusID == 4 || serviceCoupon.serviceStatusID == 5)
                        {
                            couponModel.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount = 0;
                            couponModel.Cost.FirstOrDefault(x => x.Currency == "USD").Amount = 0;
                        }
                        else
                        {
                            couponModel.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount = -1;
                            couponModel.Cost.FirstOrDefault(x => x.Currency == "USD").Amount = -1;
                        }
                    }
                    couponModel.CouponTotal = serviceCoupon.total.ToString();
                    couponModel.CouponCurrencyCode = serviceCoupon.currencyCode;
                    //audited
                    couponModel.Audited = (serviceCoupon.audit == true ? true : false);
                    couponModel.ProviderInvoiceID = serviceCoupon.providerInvoiceID;
                    if (couponModel.Audited)
                    {
                        couponModel.Audit = "Audited on " + serviceCoupon.auditDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") + " by " + serviceCoupon.auditFirstName + " " + serviceCoupon.auditLastName + " " + (serviceCoupon.auditInvoice != null && serviceCoupon.auditInvoice != "" ? " Invoice " + serviceCoupon.auditInvoice : "");
                    }

                    //paid to provider
                    couponModel.PaidToProvider = (serviceCoupon.paidToProvider == true ? true : false);
                    if (couponModel.PaidToProvider)
                    {
                        couponModel.PaidToProviderInfo = "Paid on " + serviceCoupon.paidDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") + " by " + serviceCoupon.paidByFirstname + " " + serviceCoupon.paidByLastname;
                    }

                }
            }
            return couponModel;
        }

        public CouponsList GetCouponsList(AuditCouponsModel.SearchCoupon searchModel)
        {
            CouponsList couponsList = new CouponsList();
            List<CouponViewModel> coupons = new List<CouponViewModel>();
            decimal totalCostMXN = 0;
            decimal totalCostUSD = 0;

            if (searchModel.Search_Folio != null)
            {
                string[] folios = (searchModel.Search_Folio.IndexOf(",") > 0 ? searchModel.Search_Folio.Split(',').Select(p => p.Trim()).ToArray() : new string[1] { searchModel.Search_Folio });
                long[] terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();

                //padding a los folios
                List<string> folios2 = new List<string>();
                List<string> references = new List<string>();
                string letter = string.Empty;
                foreach (string folio in folios)
                {
                    string trimmedFolio = folio.Trim();
                    letter = trimmedFolio.Substring(0, 1);
                    if (!Utils.GeneralFunctions.Number.IsNumeric(letter))
                    {
                        string numeric = trimmedFolio.Substring(1);
                        folios2.Add(letter + numeric.PadLeft(5, '0'));
                        folios2.Add(letter + numeric.PadLeft(7, '0'));
                    }
                    else
                    {
                        references.Add(trimmedFolio);
                    }
                }

                var PosQ = from p in db.tblPointsOfSale
                           where terminals.Contains(p.terminalID)
                           && p.shortName == letter
                           select p.pointOfSaleID;

                var CouponServices = (from c in db.tblPurchaseServiceDetails
                                      join s in db.tblPurchases_Services on c.purchase_ServiceID equals s.purchase_ServiceID
                                      join p in db.tblPurchases on s.purchaseID equals p.purchaseID
                                      join l in db.tblLeads on p.leadID equals l.leadID
                                      where terminals.Contains(p.terminalID)
                                      && ((PosQ.Contains(p.pointOfSaleID)
                                      //&& folios2.Contains(c.coupon.Substring(0, c.coupon.IndexOf("-"))))
                                      && folios2.Contains(c.coupon.Substring(0, (c.coupon.IndexOf("-") != -1 ? c.coupon.IndexOf("-") : c.coupon.Length))))
                                      //&& folios2.Contains(c.coupon.Substring(0, c.coupon.Length-2)))
                                      //&& ((c.coupon.IndexOf("-") != -1 && folios2.Contains(c.coupon.Substring(0, (c.coupon.Length - 2)))) 
                                      //|| folios2.Contains(c.coupon)))
                                      || references.Contains(s.couponReference) || references.Contains(l.lastName))
                                      orderby c.coupon ascending
                                      select new
                                      {
                                          c.purchase_ServiceID,
                                          c.tblPurchases_Services.purchaseID
                                      }).Distinct();

                foreach (var coupon in CouponServices)
                {
                    string couponString = coupon.purchaseID.ToString() + "-" + coupon.purchase_ServiceID.ToString();
                    CouponViewModel currentCoupon = GetCoupon(couponString, "en-US");
                    totalCostMXN += currentCoupon.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount;
                    totalCostUSD += currentCoupon.Cost.FirstOrDefault(x => x.Currency == "USD").Amount;

                    var Assignations = (from y in db.tblPurchaseServiceDetails
                                        join a in db.tblPaymentsAssignation
                                        on y.purchaseServiceDetailID equals a.purchaseServiceDetailID
                                        into y_a
                                        from a in y_a.DefaultIfEmpty()
                                        join p in db.tblPaymentDetails
                                        on a.paymentDetailsID equals p.paymentDetailsID
                                        into a_p
                                        from p in a_p.DefaultIfEmpty()
                                        where y.purchase_ServiceID == coupon.purchase_ServiceID
                                        select new
                                        {
                                            y.purchase_ServiceID,
                                            p.invitation
                                        }).Distinct().ToList();

                    foreach (var inv in Assignations)
                    {
                        if (inv.invitation != null)
                        {
                            currentCoupon.Invitation += (currentCoupon.Invitation != null && currentCoupon.Invitation != "" ? ", " : "") + inv.invitation;
                        }
                    }

                    coupons.Add(currentCoupon);
                }
            }

            couponsList.Coupons = coupons;
            couponsList.TotalCost = ReportDataModel.GetDefaultListOfMoney(false);
            couponsList.TotalCost.FirstOrDefault(x => x.Currency == "MXN").Amount = totalCostMXN;
            couponsList.TotalCost.FirstOrDefault(x => x.Currency == "USD").Amount = totalCostUSD;
            return couponsList;
        }

        public CouponsList GetCouponsListFromCache(AuditCouponsModel.SearchCoupon searchModel)
        {
            CouponsList couponsList = new CouponsList();
            List<CouponViewModel> coupons = new List<CouponViewModel>();
            decimal totalCostMXN = 0;
            decimal totalCostUSD = 0;

            if (searchModel.Search_Folio != null)
            {
                string[] folios = (searchModel.Search_Folio.IndexOf(",") > 0 ? searchModel.Search_Folio.Split(',').Select(p => p.Trim()).ToArray() : new string[1] { searchModel.Search_Folio });
                long[] terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();

                //padding a los folios
                List<string> folios2 = new List<string>();
                foreach (string folio in folios)
                {
                    string letter = folio.Substring(0, 1);
                    string numeric = folio.Substring(1);
                    folios2.Add(letter + numeric.PadLeft(5, '0'));
                    folios2.Add(letter + numeric.PadLeft(7, '0'));
                }

                var CouponServices = (from c in db.tblCouponInfo
                                      where folios2.Contains(c.folio)
                                      && terminals.Contains(c.terminalID)
                                      orderby c.folio ascending
                                      select new
                                      {
                                          c.purchase_ServiceID,
                                          c.purchaseID
                                      }).Distinct();

                foreach (var coupon in CouponServices)
                {
                    string couponString = coupon.purchaseID.ToString() + "-" + coupon.purchase_ServiceID.ToString();
                    CouponViewModel currentCoupon = GetCouponFromCache(couponString, "en-US");
                    totalCostMXN += currentCoupon.Cost.FirstOrDefault(x => x.Currency == "MXN").Amount;
                    totalCostUSD += currentCoupon.Cost.FirstOrDefault(x => x.Currency == "USD").Amount;
                    coupons.Add(currentCoupon);
                }
            }

            couponsList.Coupons = coupons;
            couponsList.TotalCost = ReportDataModel.GetDefaultListOfMoney(false);
            couponsList.TotalCost.FirstOrDefault(x => x.Currency == "MXN").Amount = totalCostMXN;
            couponsList.TotalCost.FirstOrDefault(x => x.Currency == "USD").Amount = totalCostUSD;
            return couponsList;
        }

        public AuditCouponsModel.InvoiceItem GetProviderInvoice(long id)
        {
            bool ableToUnaudit = true;
            UserSession session = new UserSession();
            var Permissions = (from x in db.tblSysProfiles
                               where x.sysWorkGroupID == session.WorkGroupID
                               && x.roleID == session.RoleID
                               && x.sysComponentID == 11934
                               select x).FirstOrDefault();

            if (Permissions != null)
            {
                ableToUnaudit = Permissions.edit_;
            }

            AuditCouponsModel.InvoiceItem item = new AuditCouponsModel.InvoiceItem();
            var Invoice = (from p in db.tblProvidersInvoices
                           join pr in db.tblProviders
                           on p.providerID equals pr.providerID
                           where p.providerInvoiceID == id
                           select new
                           {
                               p.providerInvoiceID,
                               p.invoiceNumber,
                               p.providerID,
                               pr.invoiceCurrencyID
                           }).Single();

            item.ProviderInvoiceID = Invoice.providerInvoiceID;
            item.Invoice = Invoice.invoiceNumber;
            item.ProviderID = Invoice.providerID;
            if (Invoice.invoiceCurrencyID == null)
            {
                item.InvoiceCurrency = "Undefined";
            }
            else if (Invoice.invoiceCurrencyID == 2)
            {
                item.InvoiceCurrency = "Pesos";
            }
            else if (Invoice.invoiceCurrencyID == 1)
            {
                item.InvoiceCurrency = "Dollars";
            }

            var Coupons = (from c in db.tblPurchases_Services
                           join u in db.tblUserProfiles
                           on c.auditedByUserID equals u.userID
                           into c_u
                           from u in c_u.DefaultIfEmpty()
                           where c.providerInvoiceID == item.ProviderInvoiceID
                           select new
                           {
                               c.purchase_ServiceID,
                               c.purchaseID,
                               c.auditDate,
                               c.auditedByUserID,
                               u.nickname,
                               u.firstName,
                               u.lastName,
                               c.audit,
                               c.paidDate
                           }).ToList();

            var purchaseServiceIDs = Coupons.Select(x => x.purchase_ServiceID).ToList();

            var CouponsCache = (from cc in db.tblCouponInfo
                                where purchaseServiceIDs.Contains(cc.purchase_ServiceID)
                                select new
                                {
                                    cc.folio,
                                    cc.terminal,
                                    cc.customerFirstName,
                                    cc.customerLastName,
                                    cc.dateConfirmed,
                                    cc.item,
                                    cc.dateItem,
                                    cc.confirmationNumber,
                                    cc.confirmedBy,
                                    cc.status,
                                    cc.paidCloseOut,
                                    cc.costUSD,
                                    cc.costMXN,
                                    cc.totalUSD,
                                    cc.totalMXN,
                                    cc.currencyID,
                                    cc.purchase_ServiceID,
                                    cc.serviceStatusID,
                                    cc.serviceID,
                                    cc.provider,
                                    cc.cancelationCharge
                                }).ToList();

            var UnitsInCache = from u in db.tblCouponInfoUnits
                               where purchaseServiceIDs.Contains(u.purchaseServiceID)
                               select new
                               {
                                   u.purchaseServiceID,
                                   u.quantity,
                                   u.unit,
                                   u.customCost
                               };

            var Assignations = (from y in db.tblPurchaseServiceDetails
                                join a in db.tblPaymentsAssignation
                                on y.purchaseServiceDetailID equals a.purchaseServiceDetailID
                                into y_a
                                from a in y_a.DefaultIfEmpty()
                                join p in db.tblPaymentDetails
                                on a.paymentDetailsID equals p.paymentDetailsID
                                into a_p
                                from p in a_p.DefaultIfEmpty()
                                where purchaseServiceIDs.Contains(y.purchase_ServiceID)
                                select new
                                {
                                    y.purchase_ServiceID,
                                    y.coupon,
                                    p.invitation
                                }).Distinct().ToList();

            List<CouponViewModel> couponsList = new List<CouponViewModel>();
            foreach (var coupon in Coupons)
            {
                CouponViewModel currentCoupon = new CouponViewModel();
                currentCoupon.Cost = new List<Money>();
                var couponReal = Coupons.FirstOrDefault(x => x.purchase_ServiceID == coupon.purchase_ServiceID);
                var couponInCache = CouponsCache.FirstOrDefault(x => x.purchase_ServiceID == coupon.purchase_ServiceID);
                if (couponInCache != null)
                {
                    currentCoupon.CouponNumber = couponInCache.folio;
                    currentCoupon.Website = couponInCache.terminal;
                    currentCoupon.GuestName = couponInCache.customerFirstName + " " + couponInCache.customerLastName;
                    if (couponInCache.dateConfirmed != null)
                    {
                        currentCoupon.PurchaseDate = couponInCache.dateConfirmed.Value.ToString("yyyy-MM-dd hh:mm:dd tt");
                    }
                    currentCoupon.ActivityName = couponInCache.serviceID + " - " + couponInCache.item;
                    currentCoupon.ActivityDateAndSchedule = couponInCache.dateItem.ToString("yyyy-MM-dd");
                    currentCoupon.ConfirmationNumber = couponInCache.confirmationNumber;
                    currentCoupon.SalesAgent = couponInCache.confirmedBy;
                    currentCoupon.Status = couponInCache.status;
                    currentCoupon.CloseOut = couponInCache.paidCloseOut;
                    currentCoupon.Cost.Add(new Money()
                    {
                        Amount = couponInCache.costUSD,
                        Currency = "USD"
                    });
                    currentCoupon.Cost.Add(new Money()
                    {
                        Amount = couponInCache.costMXN,
                        Currency = "MXN"
                    });
                    currentCoupon.StatusID = couponInCache.serviceStatusID;
                    currentCoupon.Audited = (couponReal.audit == true ? true : false);
                    if (couponReal.audit == true)
                    {
                        currentCoupon.Audit = "Audited on " + couponReal.auditDate.Value.ToString("yyyy-MM-dd") + " by " + couponReal.firstName + " " + couponReal.lastName;
                    }
                    currentCoupon.PaidToProvider = couponReal.paidDate != null ? true : false;
                    currentCoupon.ProviderInvoiceID = item.ProviderInvoiceID;
                    foreach (var unit in UnitsInCache.Where(x => x.purchaseServiceID == coupon.purchase_ServiceID))
                    {
                        currentCoupon.Units += unit.quantity + " " + unit.unit;
                        if (unit.customCost == true)
                        {
                            currentCoupon.CustomCost = true;
                        }
                    }
                    currentCoupon.ItemID = couponInCache.purchase_ServiceID;
                    currentCoupon.Provider = couponInCache.provider;
                    if (couponInCache.currencyID == 1)
                    {
                        currentCoupon.CouponTotal = couponInCache.totalUSD.ToString();
                        currentCoupon.CouponCurrencyCode = "USD";
                    }
                    else if (couponInCache.currencyID == 2)
                    {
                        currentCoupon.CouponTotal = couponInCache.totalMXN.ToString();
                        currentCoupon.CouponCurrencyCode = "MXN";
                    }
                    currentCoupon.CancelationCharge = couponInCache.cancelationCharge;
                    //currentCoupon.InvoiceCurrency = (Invoice.invoiceCurrencyID == 2 ? "Pesos" : "Dollars");

                    if (Invoice.invoiceCurrencyID == null)
                    {
                        currentCoupon.InvoiceCurrency = "Undefined";
                    }
                    else if (Invoice.invoiceCurrencyID == 2)
                    {
                        currentCoupon.InvoiceCurrency = "Pesos";
                    }
                    else if (Invoice.invoiceCurrencyID == 1)
                    {
                        currentCoupon.InvoiceCurrency = "Dollars";
                    }
                }
                else
                {
                    string couponString = coupon.purchaseID.ToString() + "-" + coupon.purchase_ServiceID.ToString();
                    currentCoupon = GetCoupon(couponString, "en-US");
                }

                currentCoupon.AbleToUnaudit = ableToUnaudit;

                var couponInvitation = (from c in Assignations
                                        where c.purchase_ServiceID == coupon.purchase_ServiceID
                                        select new
                                        {
                                            c.invitation
                                        }).ToList();

                foreach (var inv in couponInvitation)
                {
                    if (inv.invitation != null)
                    {
                        currentCoupon.Invitation += (currentCoupon.Invitation != null && currentCoupon.Invitation != "" ? ", " : "") + inv.invitation;
                    }
                }

                couponsList.Add(currentCoupon);
            }
            couponsList = couponsList.OrderBy(x => x.CouponNumber).ToList();
            item.Coupons = couponsList;

            return item;
        }

        public AttemptResponse SaveProviderInvoice(AuditCouponsModel.InvoiceItem model)
        {
            AttemptResponse attempt = new AttemptResponse();
            ReportDataModel rdm = new ReportDataModel();
            tblProvidersInvoices invoice = new tblProvidersInvoices();
            if (model.ProviderInvoiceID == 0)
            {
                //nueva
                invoice.invoiceNumber = model.Invoice.Trim();
                invoice.dateSaved = DateTime.Now;
                invoice.savedByUserID = session.UserID;
                if (model.ProviderID != null)
                {
                    invoice.providerID = (int)model.ProviderID;
                }
                db.tblProvidersInvoices.AddObject(invoice);
                db.SaveChanges();
            }
            else
            {
                //editar
                invoice = (from x in db.tblProvidersInvoices
                           where x.providerInvoiceID == model.ProviderInvoiceID
                           select x).FirstOrDefault();

                invoice.invoiceNumber = model.Invoice.Trim();
                invoice.savedByUserID = session.UserID;
                if (model.ProviderID != null)
                {
                    invoice.providerID = (int)model.ProviderID;
                }
                db.SaveChanges();
            }

            try
            {
                //actualizar cupones relacionados
                long[] couponsids = new long[] { };
                if (model.PurchaseServicesIDs != null)
                {
                    couponsids = model.PurchaseServicesIDs.Split(',').Select(m => long.Parse(m)).ToArray();
                }

                foreach (var couponID in couponsids)
                {
                    var coupon = (from c in db.tblPurchases_Services
                                  where c.purchase_ServiceID == couponID
                                  && c.providerInvoiceID == null
                                  select c).FirstOrDefault();

                    if (coupon != null)
                    {
                        coupon.providerInvoiceID = invoice.providerInvoiceID;
                        coupon.providerInvoiceDate = DateTime.Now;
                    }
                    db.SaveChanges();
                    //actualizar cache con cupones relacionados
                    var cache = (from c in db.tblCouponInfo
                                 where c.purchase_ServiceID == couponID
                                 && c.providerInvoiceID == null
                                 select c).FirstOrDefault();

                    if (cache != null)
                    {
                        cache.providerInvoiceID = invoice.providerInvoiceID;
                        cache.auditedProviderInvoice = invoice.invoiceNumber;
                    }
                    db.SaveChanges();
                }


                //actualizar cupones excluidos
                var excludeCoupons = from c in db.tblPurchases_Services
                                     where c.providerInvoiceID == invoice.providerInvoiceID
                                     && !couponsids.Contains(c.purchase_ServiceID)
                                     select c;

                foreach (var exc in excludeCoupons)
                {
                    exc.providerInvoiceID = null;
                    exc.providerInvoiceDate = null;
                }
                db.SaveChanges();

                //actualizar cache con cupones desrelacionados
                var excludeCache = from c in db.tblCouponInfo
                                   where c.providerInvoiceID == invoice.providerInvoiceID
                                   && !couponsids.Contains(c.purchase_ServiceID)
                                   select c;

                foreach (var exc in excludeCache)
                {
                    exc.providerInvoiceID = null;
                }
                db.SaveChanges();

                //PAGO DE CUPONES
                long[] paidcouponsids = new long[] { };
                if (model.PaidPurchaseServicesIDs != null)
                {
                    paidcouponsids = model.PaidPurchaseServicesIDs.Split(',').Select(m => long.Parse(m)).ToArray();
                }

                //marcar cupones como pagados
                var paidPurchaseServices = from s in db.tblPurchases_Services
                                           where paidcouponsids.Contains(s.purchase_ServiceID)
                                           select s;

                foreach (var purchaseService in paidPurchaseServices)
                {
                    if (purchaseService.paidToProvider != true)
                    {
                        purchaseService.paidToProvider = true;
                        purchaseService.paidDate = DateTime.Now;
                        purchaseService.paidByUserID = session.UserID;
                    }
                }
                db.SaveChanges();

                //marcar cupones como despagados
                var notPaidCoupons = from c in db.tblPurchases_Services
                                     where c.providerInvoiceID == invoice.providerInvoiceID
                                     && !paidcouponsids.Contains(c.purchase_ServiceID)
                                     select c;

                foreach (var exc in notPaidCoupons)
                {
                    exc.paidToProvider = false;
                    exc.paidDate = null;
                    exc.paidByUserID = null;
                }
                db.SaveChanges();

                //AUDITAR CUPONES
                long[] selectedcouponsids = new long[] { };
                if (model.PurchaseSelectedServicesIDs != null)
                {
                    selectedcouponsids = model.PurchaseSelectedServicesIDs.Split(',').Select(m => long.Parse(m)).ToArray();
                }

                //marcar cupones como auditados
                var purchaseServices = from s in db.tblPurchases_Services
                                       where selectedcouponsids.Contains(s.purchase_ServiceID)
                                       select s;
                foreach (var purchaseService in purchaseServices)
                {
                    if (purchaseService.audit != true)
                    {
                        purchaseService.audit = true;
                        purchaseService.auditDate = DateTime.Now;
                        purchaseService.auditedByUserID = session.UserID;
                        purchaseService.auditInvoice = model.Invoice;
                    }
                    else
                    {
                        purchaseService.auditInvoice = model.Invoice;
                    }
                }
                db.SaveChanges();

                //marcar cache como auditado
                bool acer = invoice.tblProviders.tblTerminals.useCurrentCostER;
                if (acer)
                {
                    var cacheAudit = from c in db.tblCouponInfo
                                     where selectedcouponsids.Contains(c.purchase_ServiceID)
                                     select c;
                    foreach (var purchaseService in cacheAudit)
                    {
                        if (purchaseService.audited != true)
                        {
                            purchaseService.audited = true;
                            purchaseService.auditedOnDate = DateTime.Now;
                            tblUserProfiles user = db.tblUserProfiles.FirstOrDefault(x => x.userID == session.UserID);
                            purchaseService.auditedBy = user.firstName + " " + user.lastName;
                            purchaseService.auditedProviderInvoice = model.Invoice;
                        }
                        else
                        {
                            purchaseService.auditedProviderInvoice = model.Invoice;
                        }
                    }
                    db.SaveChanges();
                }

                //marcar cupones como desauditados
                var notAuditedCoupons = from c in db.tblPurchases_Services
                                        where c.providerInvoiceID == invoice.providerInvoiceID
                                        && !selectedcouponsids.Contains(c.purchase_ServiceID)
                                        select c;

                foreach (var exc in notAuditedCoupons)
                {
                    exc.audit = false;
                    exc.auditDate = null;
                    exc.auditedByUserID = null;
                    exc.auditInvoice = null;
                    exc.removeAuditDate = DateTime.Now;
                    exc.removeAuditByUserID = session.UserID;
                }
                db.SaveChanges();

                //marcar cache como desauditado
                var notAuditedCache = from c in db.tblCouponInfo
                                      where c.providerInvoiceID == invoice.providerInvoiceID
                                      && !selectedcouponsids.Contains(c.purchase_ServiceID)
                                      select c;

                foreach (var exc in notAuditedCache)
                {
                    exc.audited = false;
                    exc.auditedOnDate = null;
                    exc.auditedBy = null;
                    exc.auditedProviderInvoice = null;
                }
                db.SaveChanges();

                attempt.Type = Attempt_ResponseTypes.Ok;
                attempt.Exception = null;
                attempt.ObjectID = invoice.providerInvoiceID;
            }
            catch (Exception e)
            {
                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Exception = e.InnerException;
                attempt.ObjectID = invoice.providerInvoiceID;

                var email = new System.Net.Mail.MailMessage();
                email.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                email.To.Add("gguerrap@villagroup.com");
                email.Subject = "SaveProviderInvoice Error";
                email.IsBodyHtml = true;
                email.Body = "El método SaveProviderInvoice no se completó.<br>ProviderInvoiceID: " + invoice.providerInvoiceID + "<br>Error: " + e.InnerException;
                EmailNotifications.SendSync(email);
            }
            return attempt;
        }

        public static string GenerateBarCode(string codeInfo)
        {
            string imgstring = "";

            try
            {
                Image myimg = GenCode128.Code128Rendering.MakeBarcodeImage(codeInfo, 2, true);
                ImageFormat format = ImageFormat.Png;
                using (MemoryStream ms = new MemoryStream())
                {
                    myimg.Save(ms, format);
                    byte[] imageBytes = ms.ToArray();

                    imgstring = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {

            }

            return imgstring;
        }
    }
}
