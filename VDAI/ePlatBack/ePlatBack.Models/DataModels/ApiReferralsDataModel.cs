using System;
using System.Collections.Generic;
using System.Linq;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using ePlatBack.Models.eplatformDataModel;
using System.Net.Mail;
using Newtonsoft.Json;

namespace ePlatBack.Models.DataModels
{
    public enum RedemptionStatus
    {
        UserNotAuthorized = -4,
        NotEnoughData = -3,
        NotEnoughCredits = -2,
        NotFound = -1,
        AcceptTermsConditions = 0,
        Ok = 1,
        Applied = 2
    }

    public class ApiReferralsDataModel
    {
        public class RedemptionResponse
        {
            public RedemptionStatus Status = RedemptionStatus.NotFound;
            public int Amount = 0;
            public Guid? AuthorizationCode = null;
            public DateTime? Date = null;
        }
        public static List<APIReferralsViewModel.ReferralReservation> GetReservations(string fromDate, string toDate, int type)
        {
            List<APIReferralsViewModel.ReferralReservation> reservations = new List<APIReferralsViewModel.ReferralReservation>();

            ecommerceEntities ecommerce = new ecommerceEntities();
            DateTime fDate = DateTime.Parse(fromDate);
            DateTime tDate = DateTime.Parse(toDate);
            var Reservations = ecommerce.sp_reservacionesReferidos(terminales: "51", fechaInicio: fDate, fechaFin: tDate, tipoBusqueda: type);

            foreach (var reservation in Reservations)
            {
                if (reservations.Count(x => x.ReservationID == reservation.ID_RESERVACION) == 0)
                {
                    APIReferralsViewModel.ReferralReservation newReservation = new APIReferralsViewModel.ReferralReservation();
                    newReservation.ReservationID = reservation.ID_RESERVACION;
                    newReservation.LastModificationDate = (reservation.LAST_MODIFICATION_DATE != null ? reservation.LAST_MODIFICATION_DATE.Value.ToString("yyyy-MM-dd hh:mm:ss") : null);
                    newReservation.Terminal = reservation.TERMINAL;
                    newReservation.SalesDate = reservation.SALES_DATE != null ? reservation.SALES_DATE.Value.ToString("yyyy-MM-dd") : "";
                    newReservation.CertNumber = reservation.CERT_NUMBER;
                    newReservation.SalesAgent = reservation.SALES_AGENT;
                    newReservation.ToCloser = reservation.TO_CLOSER == "" ? null : reservation.TO_CLOSER;
                    newReservation.PackagePrice = reservation.PACKAGE_PRICE;
                    newReservation.PackageNights = reservation.PACKAGE_NTS;
                    newReservation.PackageAdults = reservation.PACKAGE_ADULTS;
                    newReservation.Destination = reservation.DESTINATION;
                    newReservation.ArrivalDate = reservation.ARRIVAL_DATE != null ? reservation.ARRIVAL_DATE.Value.ToString("yyyy-MM-dd") : null;
                    newReservation.DepartureDate = reservation.DEPARTURE_DATE != null ? reservation.DEPARTURE_DATE.Value.ToString("yyyy-MM-dd") : null;
                    newReservation.GuestFirstName = reservation.GUEST_FIRST_NAME;
                    newReservation.GuestLastName = reservation.GUEST_LAST_NAME;
                    newReservation.MarketingSource = reservation.MARKETING_SOURCE;
                    newReservation.LeadType = reservation.LEAD_TYPE;
                    newReservation.CertificateComments = reservation.CERTIFICATE_COMMMENTS;
                    newReservation.ReservationStatus = reservation.RESERVATION_STATUS;
                    newReservation.ConfirmationDate = reservation.CONFIRMATION_DATE != null ? reservation.CONFIRMATION_DATE.Value.ToString("yyyy-MM-dd") : null;
                    newReservation.LeadGenerator = reservation.LEAD_GENERATOR == "" ? null : reservation.LEAD_GENERATOR;
                    newReservation.TimeshareOffer = reservation.TIMESHARE_OFFER;
                    newReservation.ReservationAgent = reservation.RESERVATION_AGENT;
                    newReservation.ReservationCallStatus = reservation.RESERVATION_CALL_STATUS;
                    newReservation.CancelationDate = reservation.CANCELATION_DATE != null ? reservation.CANCELATION_DATE.Value.ToString("yyyy-MM-dd") : null;
                    newReservation.CancelationReason = reservation.CANCELATION_REASON;
                    newReservation.UnitType = reservation.UNIT_TYPE;
                    newReservation.HotelConfirmation = reservation.HOTEL_CONFIRMATION != null ? reservation.HOTEL_CONFIRMATION.Replace("\t", "") : null;
                    newReservation.Resort = reservation.RESORT;
                    newReservation.Flights = new List<APIReferralsViewModel.ReferralFlightInfo>();
                    if (reservation.FLIGHT_DATE_TIME != null && reservation.FLIGHT_DATE_TIME != " " && reservation.FLIGHT_DATE_TIME != "")
                    {
                        string time = "00:00:00";
                        int pad = 8 - (19 - reservation.FLIGHT_DATE_TIME.Length);
                        newReservation.Flights.Add(new APIReferralsViewModel.ReferralFlightInfo()
                        {
                            FlightDateTime = reservation.FLIGHT_DATE_TIME + time.Substring(pad)
                        });
                    }
                    newReservation.SpecialRequestComments = reservation.SPECIAL_REQUEST_COMMENTS;
                    newReservation.PlanType = reservation.PLAN_TYPE;
                    newReservation.VerificationAgent = reservation.VERIFICATION_AGENT;
                    newReservation.NumberOfChildren = reservation.NUMBER_OF_CHILDREN;
                    newReservation.TotalNights = reservation.TOTAL_NIGHTS;

                    reservations.Add(newReservation);
                }
                else
                {
                    string time = "00:00:00";
                    int pad = 8 - (19 - reservation.FLIGHT_DATE_TIME.Length);
                    reservations.FirstOrDefault(x => x.ReservationID == reservation.ID_RESERVACION).Flights.Add(new APIReferralsViewModel.ReferralFlightInfo()
                    {
                        FlightDateTime = reservation.FLIGHT_DATE_TIME + time.Substring(pad)
                    });
                }
            }

            return reservations;
        }

        public static AttemptResponse SendInvitationsReferrals(APIReferralsViewModel.ReferralInvitation model, string memberAccount)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            MailMessage Mail = new MailMessage();
            Guid AdminID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A"),
                           TrackingID = Guid.NewGuid();
            long[] ParticipantTerminals = { 35, 73 };

            int LeadSourceChannelID = 0,
                LeadSourceID = 0;

            string ReplyTo = string.Empty;

            var ReferrerInfo = (from l in db.tblLeads
                                join m in db.tblMemberInfo on l.leadID equals m.leadID
                                where m.memberNumber == memberAccount & ParticipantTerminals.Contains(l.terminalID)
                                select new
                                {
                                    l.firstName,
                                    l.lastName,
                                    l.terminalID,
                                    l.leadID,
                                    l.isTest
                                }).SingleOrDefault();

            if (ReferrerInfo.isTest)
            {
                response.Type = Attempt_ResponseTypes.Ok;
            }

            switch (ReferrerInfo.terminalID)
            {
                case 35:
                    ReplyTo = "help@beachfrontrewards.com";
                    LeadSourceChannelID = 32;
                    LeadSourceID = 65;
                    break;
                case 73:
                    ReplyTo = "help@beachfrontrewards.com";
                    LeadSourceChannelID = 50;
                    LeadSourceID = 66;
                    break;
            }

            var MailTemplate = GetEmailTerminalTemplate(ReferrerInfo.terminalID, "Referral Invitation");

            if (MailTemplate == null)
            {
                response.Message = "Email Template not Found";
                return response;
            }

            if (ReferrerInfo == null)
            {
                response.Message = "Customer not found";
                return response;
            }
            model.FirstName = ToTitleCase(model.FirstName);
            model.LastName = ToTitleCase(model.LastName);

            var attempt = NewReferral(new APIReferralsViewModel.NewReferral
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                TerminalID = ReferrerInfo.terminalID,
                ReferredBy = ReferrerInfo.leadID.ToString(),
                SourceChannelID = LeadSourceChannelID,
                SourceID = LeadSourceID
            }, 92);

            if (attempt.Type == Attempt_ResponseTypes.Ok)
            {
                Guid PreLeadID = Guid.Parse(Convert.ToString(attempt.ObjectID));

                MailTemplate.Subject = MailTemplate.Subject.Replace("$referrerFirstName", ReferrerInfo.firstName).Replace("$referrerLastName", ReferrerInfo.lastName);

                MailTemplate.Content = MailTemplate.Content.Replace("$referrerFirstName", ReferrerInfo.firstName)
                                                           .Replace("$referrerLastName", ReferrerInfo.lastName)
                                                           .Replace("$firstName", model.FirstName)
                                                           .Replace("$lastName", model.LastName)
                                                           .Replace("$LeadID", ReferrerInfo.leadID.ToString())
                                                           .Replace("$trakingID", TrackingID.ToString())
                                                           .Replace("$referralUrl", $"?id%3D{ ReferrerInfo.leadID }%26pre%3D{ PreLeadID }%26channelid%3D{ LeadSourceChannelID }");

                Mail.From = new MailAddress(MailTemplate.Sender, MailTemplate.Alias);
                Mail.To.Add(model.Email);
                Mail.Subject = MailTemplate.Subject;
                Mail.IsBodyHtml = true;
                Mail.Body = MailTemplate.Content;
                Mail.ReplyToList.Add(ReplyTo);

                EmailNotifications.SaveEmailSendLog(MailTemplate.EmailNotificationID, "lead", PreLeadID.ToString(), TrackingID, JsonConvert.SerializeObject(Mail), MailTemplate.Subject);
                MailTemplate.Content = EmailNotifications.InsertTrackerEmailLog(MailTemplate.Content, TrackingID.ToString());
                var Mails = new List<MailingViewModel.MailMessageResponse>
                {
                    new MailingViewModel.MailMessageResponse()
                    {
                        MailMessage = Mail
                    }
                };
                //Utils.EmailNotifications.Send(Mails);
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = PreLeadID.ToString();
            }
            return response;
        }

        public static AttemptResponse NewReferral(APIReferralsViewModel.NewReferral model, int? LeadTypeID = null)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            Guid AdminID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A"),
                 LeadID;

            bool IsTest = false;

            long[] ValidTermimals = { 35, 73 };

            //Se intenta obtener un Guid en caso de ser enviado
            if (GetLeadID(model.ReferredBy, out LeadID))
            {
                //Se verica que el lead no se de prueba
                IsTest = db.tblLeads.Where(x => x.leadID == LeadID).Select(x => x.isTest).SingleOrDefault();
            }
            else
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Customer not found";
                return response;
            }
            
            //Si el lead es de prueba se regresará un ok
            if (!IsTest)
            {
                model.FirstName = ToTitleCase(model.FirstName);
                model.LastName = ToTitleCase(model.LastName);

                var Source = (from ls in db.tblLeadSources
                              join lsc in db.tblLeadSourcesChannels on ls.leadSourceID equals lsc.leadSourceID
                              where ls.leadSourceID == model.SourceID
                                && lsc.leadSourceChannelID == model.SourceChannelID
                              select new
                              {
                                  LeadSourceID = ls.leadSourceID,
                                  LeadSourceChannelID = lsc.leadSourceChannelID
                              }).FirstOrDefault();

                tblLeads lead;

                if (model.PreLeadID != Guid.Empty)
                {
                    lead = (from referrer in db.tblLeads
                            join referral in db.tblLeads on referrer.leadID equals referral.referredByID
                            where referral.leadID == model.PreLeadID
                                  && referral.leadStatusID == 29
                                  && referral.leadTypeID == 92
                            select referral).FirstOrDefault();

                    if (lead == null)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Please check your email and try again";

                        return response;
                    }
                }
                else
                {
                    lead = new tblLeads
                    {
                        leadID = Guid.NewGuid(),
                        leadStatusID = 29,
                        interestLevelID = 2,
                        firstName = model.FirstName,
                        lastName = model.LastName,
                        referredByID = LeadID,
                        initialTerminalID = model.TerminalID,
                        terminalID = model.TerminalID,
                        inputDateTime = DateTime.Now,
                        inputByUserID = AdminID,
                        inputMethodID = 1,
                        deleted = false,
                        isTest = (bool)IsTest,
                    };

                    db.tblLeads.AddObject(lead);

                    if (model.Email != null)
                    {
                        tblLeadEmails mail = new tblLeadEmails();
                        mail.email = model.Email;
                        mail.main = true;
                        mail.dateLastModification = DateTime.Now;
                        mail.modifiedByUserID = AdminID;
                        lead.tblLeadEmails.Add(mail);
                    }
                }

                lead.relationship = model.Relationship;

                if (Source != null)
                {
                    lead.leadSourceID = Source.LeadSourceID;
                    lead.leadSourceChannelID = Source.LeadSourceChannelID;
                }

                if (model.Phone != null)
                {
                    tblPhones phone = new tblPhones
                    {
                        phone = model.Phone,
                        doNotCall = false,
                        main = true,
                        phoneTypeID = 4,
                        modifiedByUserID = AdminID,
                        dateLastModification = DateTime.Now
                    };
                    lead.tblPhones.Add(phone);

                    string Code = model.Phone.Substring(0, 3);
                    var Location = (from c in db.tblCountries
                                    join s in db.tblStates on c.countryID equals s.countryID
                                    join a in db.tblAreas on s.stateID equals a.stateID
                                    join ac in db.tblAreaCodes on a.areaID equals ac.areaID
                                    where ac.code == Code
                                    select new
                                    {
                                        c.countryID,
                                        s.stateID,
                                        a.area
                                    }).FirstOrDefault();
                    if (Location != null)
                    {
                        lead.countryID = Location.countryID;
                        lead.stateID = Location.stateID;
                    }
                }

                if (LeadTypeID.HasValue)
                {
                    lead.leadTypeID = LeadTypeID;
                }
                else
                {
                    lead.leadTypeID = 68;
                    lead.leadStatusID = 29;
                    lead.assignationDate = DateTime.Now;
                    lead.assignedToUserID = NetCenterDataModel.GetUserForAssignation(lead.leadTypeID, lead.terminalID);

                    Guid TrackID = Guid.NewGuid();
                    string Content;
                    var MailTemplate = new APIReferralsViewModel.EmailTemplate();
                    int NotificationID;

                    if (model.ReferredBy == LeadID.ToString())
                    {
                        MailTemplate = GetEmailTerminalTemplate(model.TerminalID, "New Referral Added");

                        Content = MailTemplate.Content.Replace("$firstName", model.FirstName)
                                                      .Replace("$lastName", model.LastName)
                                                      .Replace("$date", DateTime.Now.ToString("MMMM dd, yyyy"));
                    }
                    else
                    {
                        MailTemplate = GetEmailTerminalTemplate(model.TerminalID, "New Referral Registered With Referrer Anonymous");

                        NotificationID = db.tblEmailNotifications.Where(x => x.terminalID == model.TerminalID
                                                                          && x.description == "New Referral Registered With Referrer Anonymous")
                                                                 .Select(x => x.emailNotificationID).FirstOrDefault();

                        Content = MailTemplate.Content.Replace("$firstName", model.FirstName)
                                                      .Replace("$lastName", model.LastName)
                                                      .Replace("$trackingID", TrackID.ToString())
                                                      .Replace("$referralUrl", $"?id%3D{ LeadID }%26channelid%3D{ model.SourceChannelID }%26form%3Dfalse");
                    }
                    MailMessage Mail = new MailMessage
                    {
                        From = new MailAddress(MailTemplate.Sender, MailTemplate.Alias),
                        Subject = MailTemplate.Subject,
                        IsBodyHtml = true,
                        Body = Content
                    };
                    Mail.To.Add(model.Email);
                    Mail.ReplyToList.Add(MailTemplate.Sender.Replace("no-reply", "contact"));

                    EmailNotifications.SaveEmailSendLog(MailTemplate.EmailNotificationID, "lead", LeadID.ToString(), TrackID, JsonConvert.SerializeObject(Mail), MailTemplate.Subject);
                    Mail.Body = EmailNotifications.InsertTracker(Mail.Body, TrackID.ToString());

                    var Mails = new List<MailingViewModel.MailMessageResponse>
                    {
                        new MailingViewModel.MailMessageResponse()
                        {
                            MailMessage = Mail
                        }
                    };
                    //Utils.EmailNotifications.Send(Mails);

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = lead.leadID.ToString();
                }

                try
                {
                    db.SaveChanges();
                    response.ObjectID = lead.leadID;
                    response.Message = "New Referral Info Received";
                    response.Type = Attempt_ResponseTypes.Ok;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Type = Attempt_ResponseTypes.Error;
                    return response;
                }
            }
            else
            {
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Referral Saved";
            }

            return response;
        }

        public static AttemptResponse UpdateReferral(APIReferralsViewModel.UpdateReferral model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            Guid AdminID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");
            int[] AllowEditableLeadSourceChannelIDs = { 32, 50, 77, 78 };

            var Lead = (from l in db.tblLeads
                        join m in db.tblMemberInfo on l.referredByID equals m.leadID
                        where m.memberNumber == model.ReferredByMemberAccount
                              && AllowEditableLeadSourceChannelIDs.Contains((int)l.leadSourceChannelID)
                              && l.leadStatusID == 29
                              && l.leadID == model.ID
                        select l).SingleOrDefault();

            if (Lead != null)
            {
                Lead.firstName = ToTitleCase(model.FirstName);
                Lead.lastName = ToTitleCase(model.LastName);
                Lead.relationship = model.Relationship;

                if (model.Phone != null && model.Phone != string.Empty)
                {
                    var phone = db.tblPhones.Where(x => x.leadID == model.ID && x.main).SingleOrDefault();

                    if (phone == null)
                    {
                        phone = new tblPhones
                        {
                            phone = model.Phone,
                            doNotCall = false,
                            main = true,
                            phoneTypeID = 4,
                            modifiedByUserID = AdminID,
                            dateLastModification = DateTime.Now
                        };
                        Lead.tblPhones.Add(phone);
                    }
                    phone.phone = model.Phone;

                    string Code = model.Phone.Substring(0, 3);
                    var Location = (from c in db.tblCountries
                                    join s in db.tblStates on c.countryID equals s.countryID
                                    join a in db.tblAreas on s.stateID equals a.stateID
                                    join ac in db.tblAreaCodes on a.areaID equals ac.areaID
                                    where ac.code == Code
                                    select new
                                    {
                                        c.countryID,
                                        s.stateID,
                                        a.area
                                    }).FirstOrDefault();
                    if (Location != null)
                    {
                        Lead.countryID = Location.countryID;
                        Lead.stateID = Location.stateID;
                    }
                }

                if (model.Email != null && model.Email != string.Empty)
                {
                    var mail = db.tblLeadEmails.Where(x => x.leadID == model.ID && x.main).SingleOrDefault();

                    if (mail == null)
                    {
                        mail = new tblLeadEmails
                        {
                            modifiedByUserID = AdminID,
                            main = true
                        };
                        Lead.tblLeadEmails.Add(mail);
                    }

                    mail.email = model.Email;
                    mail.dateLastModification = DateTime.Now;

                }
                db.SaveChanges();

                response.Message = "Referral Info Update";
                response.Type = Attempt_ResponseTypes.Ok;
            }
            else
            {
                response.Message = "ReferralID not Found";
                response.Type = Attempt_ResponseTypes.Error;
            }

            return response;
        }

        public static APIReferralsViewModel.ListTable GetLeadCredits(string memberAccount)
        {
            ePlatEntities db = new ePlatEntities();
            APIReferralsViewModel.ListTable PointList = new APIReferralsViewModel.ListTable();
            PointList.HistoryPoints = new List<APIReferralsViewModel.History>();
            List<APIReferralsViewModel.ExpiredCredits> ExpiredCreditsList = new List<APIReferralsViewModel.ExpiredCredits>();
            DateTime CurrentDate = DateTime.Now;
            Guid AdminID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A"),
                LeadID;
            int Balance = 0;

            if (GetLeadID(memberAccount, out LeadID))
            {
                var Points = db.tblReferralsPoints.Where(x => x.leadID == LeadID && x.dateDeleted == null).OrderBy(x => x.dateSaved).ThenBy(x => x.referralPointsTransactionTypeID).ToList();

                for (int i = 0; i < Points.Count(); i++)
                {
                    var points = Points[i];
                    
                    if(points.statusCode == 1)
                    {
                        Balance += points.points;
                        points.balance = Balance;
                    }
                    if (!points.expiratedPoints.HasValue && points.expirationDate <= CurrentDate && points.activePoints > 0)
                    {
                        var ExpiredCredits = ExpiredCreditsList.SingleOrDefault(x => x.ReferralID == points.generatedByLeadID);
                        if (ExpiredCredits == null)
                        {
                            ExpiredCredits = new APIReferralsViewModel.ExpiredCredits();
                        }
                        ExpiredCredits.ReferralID = points.generatedByLeadID;
                        ExpiredCredits.Points -= points.activePoints;
                        ExpiredCredits.PlaceID = points.placeID;
                        ExpiredCredits.Concept = "Expired Points";
                        ExpiredCredits.GeneratedDate = points.expirationDate;
                        ExpiredCredits.TransactionTypeID = 5;
                        points.expiratedPoints = points.activePoints;
                        points.activePoints = 0;
                        ExpiredCreditsList.Add(ExpiredCredits);
                    }
                }

                if (ExpiredCreditsList.Count > 0)
                {
                    var result = ExpiredCredits(LeadID, ExpiredCreditsList);
                    for (int i = 0; i < result.Count; i++)
                    {
                        var ExpiredPoints = result[i];
                        ExpiredPoints.balance = Balance + ExpiredPoints.points;
                        Balance += ExpiredPoints.points;
                        Points.Add(ExpiredPoints);
                    }
                }

                PointList.TotalActivePoints = Points.Sum(x => x.activePoints);
                PointList.HistoryPoints = Points.Select(x => new APIReferralsViewModel.History
                {
                    Concept = x.concept,
                    Points = x.points,
                    ResortID = (long)x.placeID,
                    TransactionTypeID = x.referralPointsTransactionTypeID,
                    RedemptionID = x.points < 0 || x.appliedByUserID != AdminID ? x.referralPointsID : Guid.Empty,
                    Resort = x.tblPlaces.place + " " + x.tblPlaces.tblDestinations.destination,
                    ActivePoints = x.activePoints,
                    Balance = x.balance,
                    StatusCode = x.statusCode,
                    ExpirationDate = x.expirationDate,
                    GeneratedDate = x.dateSaved,
                    ExpiratedPoints = x.expiratedPoints != null ? (int)x.expiratedPoints : 0
                }).OrderBy(x => x.GeneratedDate).ThenBy(x => x.Balance).ToList();

                db.SaveChanges();
            }
            return PointList;
        }

        public static List<tblReferralsPoints> ExpiredCredits(Guid id, List<APIReferralsViewModel.ExpiredCredits> listExpiredCredits)
        {
            ePlatEntities db = new ePlatEntities();
            UserSession session = new UserSession();
            List<tblReferralsPoints> ListRedeemPoints = new List<tblReferralsPoints>();
            tblReferralsPoints Redemption = new tblReferralsPoints();
            List<Guid?> ReferralsID = listExpiredCredits.Select(x => x.ReferralID).ToList();

            var PointsList = db.tblReferralsPoints.Where(x => ReferralsID.Contains(x.generatedByLeadID) && x.expirationDate <= DateTime.Now).OrderBy(x => x.expirationDate).ToList();
            int AvailablePoints = PointsList.Sum(x => x.activePoints);

            for (int i = 0; i < listExpiredCredits.Count; i++)
            {
                var expiredPoints = listExpiredCredits[i];
                Redemption = new tblReferralsPoints
                {
                    referralPointsID = Guid.NewGuid(),
                    concept = expiredPoints.Concept,
                    points = expiredPoints.Points,
                    leadID = id,
                    referralPointsTransactionTypeID = expiredPoints.TransactionTypeID,
                    placeID = expiredPoints.PlaceID,
                    dateSaved = (DateTime)expiredPoints.GeneratedDate,
                    modifiedDate = DateTime.Now,
                    statusCode = 1,
                    modifiedByUserID = null,
                };
                ListRedeemPoints.Add(Redemption);
                db.tblReferralsPoints.AddObject(Redemption);

                foreach (var points in PointsList.Where(x => x.generatedByLeadID == expiredPoints.ReferralID))
                {
                    if (expiredPoints.Points == 0) break;
                    tblReferralsPointsRedemption Redeem = new tblReferralsPointsRedemption
                    {
                        referralPointsRedemptionID = Guid.NewGuid(),
                        sourceReferralPointsID = points.referralPointsID,
                        redeemedReferralPointsID = Redemption.referralPointsID,
                        dateSaved = (DateTime)expiredPoints.GeneratedDate
                    };
                    expiredPoints.Points += points.activePoints;
                    if (expiredPoints.Points <= 0)
                    {
                        Redeem.points = points.activePoints;
                        points.activePoints = 0;
                    }
                    else if (expiredPoints.Points > 0)
                    {
                        Redeem.points = points.activePoints - expiredPoints.Points;
                        points.activePoints -= Redeem.points;
                        expiredPoints.Points = 0;
                    }
                    Redemption.tblReferralsPointsRedemption.Add(Redeem);
                }
            }
            db.SaveChanges();
            return ListRedeemPoints;
        }

        public static APIReferralsViewModel.RedemptionInfo GetRedemptionInfo(Guid id)
        {
            ePlatEntities db = new ePlatEntities();

            var RedemptionStatus = (from rp in db.tblReferralsPoints
                                    join m in db.tblMemberInfo on rp.leadID equals m.leadID
                                    where rp.referralPointsID == id && rp.dateDeleted == null
                                    select new APIReferralsViewModel.RedemptionInfo
                                    {
                                        AppliedDate = rp.appliedDate,
                                        Concept = rp.concept,
                                        Credits = -rp.points,
                                        ResortID = rp.placeID,
                                        MemberAccount = m.memberNumber
                                    }).FirstOrDefault();

            if (RedemptionStatus == null)
            {
                RedemptionStatus = new APIReferralsViewModel.RedemptionInfo();
            }

            return RedemptionStatus;
        }

        public static APIReferralsViewModel.Rewards GetRewards(string memberAccount)
        {
            ePlatEntities db = new ePlatEntities();
            APIReferralsViewModel.Rewards Rewards = new APIReferralsViewModel.Rewards();
            Rewards.History = new List<APIReferralsViewModel.History>();
            Rewards.Referrals = new List<APIReferralsViewModel.ReferralInfo>();
            Guid LeadID = Guid.Empty;
            DateTime FromDate = DateTime.Now.AddMonths(-12);
            long[] ParticipantTerminals = { 35, 73 };

            if (GetLeadID(memberAccount, out LeadID))
            {
                int resortPrefix = int.Parse(memberAccount.Substring(0, memberAccount.IndexOf('-')));

                Rewards.CustomerDetails = (from l in db.tblLeads
                                           join mi in db.tblMemberInfo on l.leadID equals mi.leadID
                                           join ct in db.tblClubTypes on mi.clubTypeID equals ct.clubTypeID
                                           join il in db.tblIdentity_Leads on l.leadID equals il.leadID                                           
                                           where l.leadID == LeadID && ParticipantTerminals.Contains(l.terminalID)
                                           select new APIReferralsViewModel.CustomerDetails
                                           {
                                               FirstName = l.firstName,
                                               LastName = l.lastName,
                                               ProfilePicturePath = l.leadProfilePicturePath,
                                               TermsAndConditions = il.acceptTermsConditions,
                                               ID = l.leadID,
                                               IsTest = l.isTest,
                                               Membership = ct.resortConnectClubType
                                           }).SingleOrDefault();

                Rewards.CustomerDetails.ResortID = db.tblContractPrefix.Where(x => x.prefixID == resortPrefix)
                                                                              .Select(x => x.placeID)
                                                                              .SingleOrDefault();

                var Result = GetLeadCredits(memberAccount);

                Rewards.Referrals = GetReferrals(memberAccount);
                Rewards.History = Result.HistoryPoints;
                Rewards.CurrentBalance = Result.TotalActivePoints;
                Rewards.AssignedCredits = Rewards.History.Where(x => x.Points > 0).Select(x => x.Points).Sum();
                Rewards.RedeemCredits = Rewards.History.Where(x => x.Points < 0 && x.StatusCode == 1).Select(x => x.Points).Sum();
            }

            return Rewards;
        }

        public static List<APIReferralsViewModel.ReferralInfo> GetReferrals(string memberAccount, long? terminalID = null)
        {
            List<APIReferralsViewModel.ReferralInfo> ReferralList = new List<APIReferralsViewModel.ReferralInfo>();
            ePlatEntities db = new ePlatEntities();
            long[] ParticipantTerminals = { 35, 73 };
            int[] NotificationIDs = { 383, 384, 356, 357 };

            var Referrals = (from mi in db.tblMemberInfo
                             join referrer in db.tblLeads on mi.leadID equals referrer.leadID
                             join referral in db.tblLeads on referrer.leadID equals referral.referredByID
                             from r in db.tblReservations.Where(x => x.leadID == referral.leadID).OrderBy(x => x.dateSaved).Take(1).DefaultIfEmpty()

                             let spiTourID = r != null ? r.spiTourID : 0

                             from spi in db.tblSPIManifest.Where(x => x.tourID == spiTourID).DefaultIfEmpty()
                             from enl in db.tblEmailNotificationLogs.Where(x => NotificationIDs.Contains(x.emailNotificationID) && x.leadID == referral.leadID).DefaultIfEmpty()

                             let emailNotificationLogID = enl != null ? enl.emailNotificationLogID : 0

                             from ene in db.tblEmailNotificationEvents.Where(x => x.emailNotificationLogID == emailNotificationLogID).Take(1).DefaultIfEmpty()
                             where mi.memberNumber == memberAccount && ParticipantTerminals.Contains(referrer.terminalID)

                             let certificateNumber = r != null ? r.certificateNumber : null
                             let arrivalDate = r != null ? r.arrivalDate : null
                             let tourDate = spi != null ? spi.tourID : 0
                             let tourStatus = spi != null ? spi.tourStatus : null
                             let emailOpen = enl != null
                             let emailSended = ene != null

                             select new
                             {
                                 SRCImg = referral.leadProfilePicturePath,
                                 ID = referral.leadID,
                                 FirstName = referral.firstName,
                                 LastName = referral.lastName,
                                 LeadSourceChannelID = referral.leadSourceChannelID,
                                 InputDate = referral.inputDateTime,
                                 Certificate = certificateNumber,
                                 ReservationDate = certificateNumber,
                                 PresentationDate = tourDate,
                                 TourStatus = tourStatus,
                                 EmailOpen = emailOpen,
                                 EmailSended = emailSended,
                                 LeadStatusID = referral.leadStatusID,
                                 InteresLevel = referral.interestLevelID,
                                 CreditsGenerated = referral.tblReferralsPoints1.Select(x => new APIReferralsViewModel.CreditsInfo
                                 {
                                     Credits = x.points,
                                     ExpirationDate = x.expirationDate,
                                     TransactionTypeID = x.referralPointsTransactionTypeID
                                 })
                             }).OrderByDescending(x => x.InputDate)
                               .ThenBy(x => x.FirstName)
                               .ThenBy(x => x.LastName)
                               .ToArray();

            for (int i = 0; i < Referrals.Count(); i++)
            {
                var referral = Referrals[i];
                APIReferralsViewModel.ReferralInfo referralInfo = new APIReferralsViewModel.ReferralInfo
                {
                    SRCImg = referral.SRCImg,
                    CreditsGenerated = referral.CreditsGenerated,
                    FirstName = referral.FirstName,
                    LastName = referral.LastName,
                    InputDate = referral.InputDate,
                };

                switch (referral.CreditsGenerated.Count())
                {
                    case 1:
                        referralInfo.Status = "Attended presentation and qualified";
                        break;
                    case 2:
                        referralInfo.Status = "Became a member";
                        break;
                    case 3:
                        referralInfo.Status = "Became a elite member";
                        break;
                    default:
                        int[] leadStatus = { 32, 50, 77, 78 };

                        if (referral.TourStatus != null)
                        {
                            referralInfo.Status = "Attended Presentation but did not Qualified";
                        }
                        else if (referral.ReservationDate != null)
                        {
                            referralInfo.Status = "Ready to Travel";
                        }
                        else if (referral.Certificate != null)
                        {
                            referralInfo.Status = "Bought Certificate";
                        }
                        else if (referral.InteresLevel == 3)
                        {
                            referralInfo.Status = "Not Interested";
                        }
                        else if (referral.LeadStatusID != 29)
                        {
                            referralInfo.Status = "Work in Progress";
                        }
                        else if (referral.EmailSended && !referral.EmailOpen)
                        {
                            referralInfo.Status = "Invitation Sent";
                        }
                        else
                        {
                            referralInfo.Status = "Assigned";
                            referralInfo.ID = leadStatus.Contains((int)referral.LeadSourceChannelID) && referral.LeadStatusID == 29 ? referral.ID.ToString() : "";
                        }
                        break;
                }

                ReferralList.Add(referralInfo);
            }

            return ReferralList;
        }

        public static RedemptionResponse AuthorizeCharge(APIReferralsViewModel.RedeemCredits model)
        {
            RedemptionResponse response = new RedemptionResponse();
            tblReferralsPoints RedeemPoints = new tblReferralsPoints();
            ePlatEntities db = new ePlatEntities();

            Guid AdminID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A"),
                 LeadID;

            if (GetLeadID(model.MemberAccount, out LeadID) && model.TransactionTypeID > 3)
            {
                model.Amount = -Math.Abs(model.Amount);

                int AvailablePoints = db.tblReferralsPoints.Where(x => x.leadID == LeadID
                                                                  && x.dateDeleted == null
                                                                  && x.expirationDate > DateTime.Now)
                                                           .Select(x => x.activePoints)
                                                           .DefaultIfEmpty()
                                                           .Sum(x => x);

                RedeemPoints.referralPointsID = Guid.NewGuid();
                RedeemPoints.concept = model.Concept;
                RedeemPoints.points = model.Amount;
                RedeemPoints.leadID = LeadID;
                RedeemPoints.referralPointsTransactionTypeID = model.TransactionTypeID;
                RedeemPoints.placeID = model.ResortID;
                RedeemPoints.dateSaved = DateTime.Now;
                RedeemPoints.modifiedDate = RedeemPoints.dateSaved;
                RedeemPoints.modifiedByUserID = AdminID;
                AvailablePoints += model.Amount;
                db.tblReferralsPoints.AddObject(RedeemPoints);

                if (AvailablePoints >= 0)
                {
                    int SumPoints = 0,
                        RedeemPointsTotal = model.Amount;
                    string PlacePointsDistribution = string.Empty;

                    var PointsList = db.tblReferralsPoints
                                       .Where(x => x.leadID == LeadID
                                            && x.dateDeleted == null
                                            && x.expirationDate > DateTime.Now
                                            && x.activePoints > 0)
                                       .OrderBy(x => x.expirationDate)
                                       .ThenBy(x => x.referralPointsTransactionTypeID)
                                       .AsEnumerable()
                                       .TakeWhile(x => (SumPoints += x.activePoints) <= -model.Amount + x.activePoints)
                                       .ToList();

                    string CreditsDistribution = "";
                    int RedemedCredits = model.Amount;

                    RedeemPoints.statusCode = model.TransactionTypeID == 4 ? Convert.ToInt32(RedemptionStatus.Ok) : Convert.ToInt32(RedemptionStatus.Applied);

                    foreach (var points in PointsList)
                    {
                        tblReferralsPointsRedemption Redeem = new tblReferralsPointsRedemption
                        {
                            referralPointsRedemptionID = Guid.NewGuid(),
                            sourceReferralPointsID = points.referralPointsID,
                            redeemedReferralPointsID = RedeemPoints.referralPointsID,
                            dateSaved = DateTime.Now
                        };
                        model.Amount += points.activePoints;
                        db.tblReferralsPointsRedemption.AddObject(Redeem);

                        if (model.Amount < 0)
                        {
                            Redeem.points = points.activePoints;
                            points.activePoints = 0;
                        }
                        else
                        {
                            Redeem.points = points.activePoints - model.Amount;
                            points.activePoints -= Redeem.points;
                            model.Amount = 0;
                        }

                        CreditsDistribution += $"{Redeem.points} credits " +
                                               $"{ points.tblPlaces.place } <br />";
                                               
                    }
                    SendRedemptionTicket(LeadID, model.TransactionTypeID, model.Concept, model.Reference, RedemedCredits, RedeemPoints.referralPointsID.ToString(), AdminID, CreditsDistribution);
                    db.SaveChanges();

                    response.AuthorizationCode = RedeemPoints.referralPointsID;
                    response.Date = RedeemPoints.dateSaved;
                    response.Status = RedemptionStatus.Ok;
                    response.Amount = AvailablePoints - RedeemPointsTotal;
                }
                else
                {
                    RedeemPoints.statusCode = Convert.ToInt32(RedemptionStatus.NotEnoughCredits);
                    response.Date = RedeemPoints.dateSaved;
                    response.Status = RedemptionStatus.NotEnoughCredits;
                    response.Amount = AvailablePoints;
                }
            }
            return response;
        }

        public static RedemptionResponse RefundCharge(Guid id)
        {
            RedemptionResponse response = new RedemptionResponse();
            ePlatEntities db = new ePlatEntities();
            Guid AdminID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");

            var Redemption = db.tblReferralsPoints.Where(x => x.referralPointsID == id && x.dateDeleted == null).FirstOrDefault();

            // Se verfica que exista el RedemptionID
            if (Redemption != null)
            {
                // Se Verifica que el usuario este autorizado
                if (Redemption.modifiedByUserID == AdminID)
                {
                    var RedemptionList = db.tblReferralsPointsRedemption.Where(x => x.redeemedReferralPointsID == id).ToList();
                    Guid[] CreditsIDs = RedemptionList.Select(x => x.sourceReferralPointsID).ToArray();
                    var CreditsList = db.tblReferralsPoints.Where(x => CreditsIDs.Contains(x.referralPointsID)).OrderByDescending(x => x.expirationDate).ToList();

                    Redemption.dateDeleted = DateTime.Now;
                    Redemption.deteledByUserID = AdminID;
                    Redemption.statusCode = Convert.ToInt32(RedemptionStatus.Ok);
                    Redemption.balance = null;

                    foreach (var creditsInvolved in CreditsList)
                    {
                        var redeem = RedemptionList.Where(x => x.sourceReferralPointsID == creditsInvolved.referralPointsID).FirstOrDefault();

                        if (creditsInvolved.expirationDate > DateTime.Now)
                        {
                            creditsInvolved.activePoints += redeem.points;
                        }
                        else
                        {
                            creditsInvolved.expiratedPoints += redeem.points;
                        }
                        db.tblReferralsPointsRedemption.DeleteObject(redeem);
                    }
                    response.AuthorizationCode = id;
                    response.Date = Redemption.dateDeleted;
                    response.Status = RedemptionStatus.Ok;
                }
                else response.Status = RedemptionStatus.UserNotAuthorized;


            }

            db.SaveChanges();
            return response;
        }

        public static AttemptResponse AcceptTermsAndConditions(string id)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            long[] ParticipantsTerminals = { 35, 73 };

            var Lead = (from mi in db.tblMemberInfo
                        join l in db.tblLeads on mi.leadID equals l.leadID
                        join il in db.tblIdentity_Leads on l.leadID equals il.leadID
                        where mi.memberNumber == id && ParticipantsTerminals.Contains(l.terminalID)
                        select il).SingleOrDefault();

            if (Lead != null)
            {
                Lead.acceptTermsConditions = true;
                Lead.lastDateAcceptTermsConditions = DateTime.Now;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = $"Terms and conditions Accepted at {Lead.lastDateAcceptTermsConditions}";
            }
            else
            {
                response.Type = Attempt_ResponseTypes.Error;
            }

            return response;
        }
        private static void SendRedemptionTicket(Guid id, int transactionType, string concept, string reference, int credits, string redemptionID, Guid userID, string creditsDistribution)
        {
            ePlatEntities db = new ePlatEntities();

            var OwnerDetails = GetOwnerDetails(id);
            Guid TrackingID = Guid.NewGuid();

            string TransactionType = db.tblReferralsPointsTransactionTypes
                                       .Where(x => x.referralPointsTransactionTypeID == transactionType)
                                       .Select(x => x.transactionType)
                                       .FirstOrDefault();
            if (transactionType == 4)
            {
                var EmailNotification = (from en in db.tblEmailNotifications
                                         join e in db.tblEmails on en.emailID equals e.emailID
                                         where en.eventID == 51 && en.active == true
                                         select new
                                         {
                                             ReplyTo = en.replyTo,
                                             Content = e.content_,
                                             Subject = e.subject,
                                             Sender = e.sender,
                                             Alias = e.alias,
                                         }).FirstOrDefault();

                MailMessage MailNotification = new MailMessage
                {
                    From = new MailAddress("no-reply@eplat.com", "ePlat Notifications"),
                    Subject = "Maintence Fee Payment Notification",
                    IsBodyHtml = true,
                    Body = EmailNotification.Content.Replace("$firstName", OwnerDetails.FirstName)
                                                    .Replace("$lastName", OwnerDetails.LastName)
                                                    .Replace("$amount", $"{ credits:N}")
                                                    .Replace("$memberAccount", OwnerDetails.MemberAccount)
                                                    .Replace("$placesList", creditsDistribution)
                                                    .Replace("$redemptionID", redemptionID)
                };
                if (!OwnerDetails.IsTest)
                {
                    foreach (var email in EmailNotification.ReplyTo.Split(',').ToArray())
                    {
                        MailNotification.To.Add(email);
                    }
                }
                else
                {
                    MailNotification.To.Add("alexis.avila@villagroup.com");
                }
                EmailNotifications.Send(MailNotification);
            }

            var EmailTemplate = GetEmailTerminalTemplate(OwnerDetails.TerminalID, $"{ TransactionType } Ticket");
            EmailTemplate.Content = EmailTemplate.Content.Replace("$firstName", OwnerDetails.FirstName)
                                                         .Replace("$lastName", OwnerDetails.LastName)
                                                         .Replace("$amount", $"{ credits:N}")
                                                         .Replace("$memberAccount", OwnerDetails.MemberAccount)
                                                         .Replace("$redemptionID", redemptionID)
                                                         .Replace("$trackingID", TrackingID.ToString())
                                                         .Replace("$date", DateTime.Now.ToString("MM/dd/yyyy"))
                                                         .Replace("reference", reference)
                                                         .Replace("concept", concept)
                                                         .Replace("$emailID", OwnerDetails.EmailID.ToString());

            var Mail = new MailMessage
            {
                From = new MailAddress(EmailTemplate.Sender, EmailTemplate.Alias),
                Subject = EmailTemplate.Subject,
                IsBodyHtml = true,
                Body = EmailTemplate.Content
            };
            Mail.To.Add(OwnerDetails.Email);

            EmailNotifications.SaveEmailSendLog(EmailTemplate.EmailNotificationID, "lead", id.ToString(), TrackingID, JsonConvert.SerializeObject(Mail), EmailTemplate.Subject);
            Mail.Body = EmailNotifications.InsertTracker(Mail.Body, TrackingID.ToString());
            //EmailNotifications.Send(new List<MailingViewModel.MailMessageResponse>
            //{
            //    new MailingViewModel.MailMessageResponse
            //    {
            //        MailMessage = Mail
            //    }    
            //});

            tblInteractions interaction = new tblInteractions
            {
                leadID = id,
                interactionTypeID = 2,
                interactionComments = $"{EmailTemplate.Subject} by value of {credits:N} credits",
                savedByUserID = userID,
                dateSaved = DateTime.Now,
                saveMethod = "RedeemPoints",
            };

            db.tblInteractions.AddObject(interaction);

            db.SaveChanges();
        }

        /* Utilerias ReferralData model */

        /// <summary>
        /// Metodo para el envio de correos
        /// </summary>
        /// <param name="mail"></param>
        /// <returns>Attempt Response con resultado 1 si fue valido</returns>
        /// 

        public static AttemptResponse SendEmail(System.Net.Mail.MailMessage mail)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();

            var lowestMail = db.tblMailingSettings
                               .Where(x => x.active == true
                                        && x.senderName == "no-reply")
                               .OrderByDescending(x => x.dailyCounter)
                               .Select(x => x)
                               .Take(1)
                               .SingleOrDefault(); ;
            try
            {
                if (lowestMail != null)
                {
                    SmtpClient SMTPServer = new SmtpClient();

                    SMTPServer.Host = lowestMail.smtpServer;
                    SMTPServer.Port = int.Parse(lowestMail.smtpPort);
                    SMTPServer.Credentials = new System.Net.NetworkCredential(lowestMail.smtpUsername, lowestMail.smtpPassword);
                    SMTPServer.EnableSsl = true;
                    SMTPServer.Send(mail);

                    lowestMail.dailyCounter++;
                    db.SaveChanges();

                }
                else
                {
                    throw new NullReferenceException();
                }

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Mail sended";
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }

        public static APIReferralsViewModel.EmailTemplate GetEmailTerminalTemplate(long teminal, string templateName, string culture = "en-US")
        {
            ePlatEntities db = new ePlatEntities();

            var EmailTemplate = (from e in db.tblEmails
                                 join et in db.tblEmails_Terminals on e.emailID equals et.emailID
                                 join en in db.tblEmailNotifications on e.emailID equals en.emailID
                                 into e_en
                                 from en in e_en.DefaultIfEmpty()

                                 let active = en == null ? true : (bool)en.active

                                 where et.terminalID == teminal
                                     && e.description == templateName
                                     && e.culture == culture
                                     && active

                                 let notificationID = en != null ? en.emailNotificationID : 0
                                 let eventID = en != null ? en.eventID : 0
                                 let replyTo = en != null ? en.replyTo : null

                                 select new APIReferralsViewModel.EmailTemplate()
                                 {
                                     Alias = e.alias,
                                     Content = e.content_,
                                     Sender = e.sender,
                                     Subject = e.subject,
                                     EmailNotificationID = notificationID,
                                     TerminalID = et.terminalID,
                                     EventID = eventID,
                                     ReplyTo = replyTo
                                 }).FirstOrDefault();

            return EmailTemplate;
        }

        public static APIReferralsViewModel.EmailTemplate GetEmailTemplate(string templateName, string culture = "en-US")
        {
            ePlatEntities db = new ePlatEntities();

            var EmailTemplate = (from e in db.tblEmails
                                 where e.description == templateName && e.culture == culture
                                 select new APIReferralsViewModel.EmailTemplate()
                                 {
                                     Alias = e.alias,
                                     Content = e.content_,
                                     Sender = e.sender,
                                     Subject = e.subject,
                                 }).FirstOrDefault();

            return EmailTemplate;
        }
        private static APIReferralsViewModel.OwnerDetails GetOwnerDetails(Guid id)
        {
            ePlatEntities db = new ePlatEntities();

            return (from mi in db.tblMemberInfo
                    join l in db.tblLeads on mi.leadID equals l.leadID
                    join e in db.tblLeadEmails on l.leadID equals e.leadID
                    into l_e
                    from e in l_e.DefaultIfEmpty()
                    where l.leadID == id && e.main
                    select new APIReferralsViewModel.OwnerDetails
                    {
                        LeadID = l.leadID,
                        MemberAccount = mi.memberNumber,
                        FirstName = l.firstName,
                        LastName = l.lastName,
                        Email = e.email,
                        EmailID = e.emailID,
                        TerminalID = l.terminalID,
                        IsTest = l.isTest
                    }).FirstOrDefault();
        }

        public static string ToTitleCase(string text)
        {
            text = text.ToLower();
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
        }
        public static bool GetLeadID(string id, out Guid leadID)
        {
            ePlatEntities db = new ePlatEntities();
            Guid LeadID;
            long[] ValidTermimals = { 35, 73 };


            //Se intenta obtener un Guid en caso de ser enviado
            if (!Guid.TryParse(id, out LeadID))
            {
                //Si no es un Guid se intentara recuperar uno, desde la db
                leadID = (from mi in db.tblMemberInfo
                          join l in db.tblLeads on mi.leadID equals l.leadID
                          where mi.memberNumber == id
                                && ValidTermimals.Contains(l.terminalID)
                          select l.leadID).SingleOrDefault();
            }
            else
            {
                //Si lo se, se verificara que exista dentro de la db
                leadID = (from mi in db.tblMemberInfo
                          join l in db.tblLeads on mi.leadID equals l.leadID
                          where l.leadID == LeadID
                                && ValidTermimals.Contains(l.terminalID)
                          select l.leadID).SingleOrDefault();
            }
            //Si el lead no existe, retornara falso
            return leadID != Guid.Empty;
        }
    }
}