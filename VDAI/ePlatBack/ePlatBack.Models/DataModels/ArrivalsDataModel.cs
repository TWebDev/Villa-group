using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Reflection;
using System.Data.Common;
using System.Data.Objects;
using System.Web.Routing;
using System.Web.Security;
using System.Linq.Dynamic;
using System.Globalization;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils.Custom;
using ePlatBack.Models.Utils.Custom.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.SqlClient;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.DataModels
{
    public class ArrivalsDataModel
    {
        public static UserSession session = new UserSession();
        public static ArrivalsViewModel.ArrivalsResponse GetArrivals(DateTime? date, string name, bool avoidRequestToFront = false)
        {
            ArrivalsViewModel.ArrivalsResponse response = new ArrivalsViewModel.ArrivalsResponse();

            response.Arrivals = new List<ArrivalsViewModel.ArrivalInfoModel>();
            response.Summary = new List<ArrivalsViewModel.ArrivalsSummary>();
            response.Prearrivals = new List<ArrivalsViewModel.PrearrivalInfo>();
            response.PowerLine = new List<ArrivalsViewModel.TeamPowerLine>();
            response.Surveys = new List<ArrivalsViewModel.SurveyInfo>();

            ePlatEntities db = new ePlatEntities();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalid = terminals[0];
            date = date ?? DateTime.Today;
            DateTime tomorrow = date.Value.AddDays(1);

            var MarketCodes = db.tblMarketCodes;
            //Prearrival Las Vegas Status
            List<int?> LeadStatuses = new List<int?>() { 1, 2, 6, 13, 15 };
            int frontOfficeResortID = 0;

            //Prearrivals
            //surveys && prearrivals
            //surveys
            var TerminalPlaceID = (from t in db.tblTerminals
                                   where t.terminalID == terminalid
                                   select t.placeID).FirstOrDefault();

            //obtención del front office ResortID
            var FrontOfficeResortIDQ = (from f in db.tblPlaces
                                        where f.placeID == TerminalPlaceID
                                        select f.frontOfficeResortID).FirstOrDefault();

            if (FrontOfficeResortIDQ != null)
            {
                frontOfficeResortID = (int)FrontOfficeResortIDQ;
            }

            //obtención del ID del survey correspondiente al hotel
            var SurveyIDs = (from s in db.tblSurveys
                             where s.placeID == TerminalPlaceID
                             select s.surveyID).ToList();

            var Questions = from q in db.tblQuestions_Terminals
                            where q.terminalID == terminalid
                            select q;

            List<int> QuestionIDs = Questions.Select(x => x.questionID).ToList();

            if (name == null)
            {
                response.Surveys = (from s in db.tblAppliedSurveys
                                    where SurveyIDs.Contains((int)s.surveyID)
                                    && s.dateIn == date
                                    select new ArrivalsViewModel.SurveyInfo()
                                    {
                                        Key = s.key_key,
                                        SentDate = s.sentDate,
                                        SubmittedDate = s.submittedDate,
                                        Rate = s.rate,
                                        ArrivalDate = s.dateIn,
                                        SurveyRoomNumber = s.Room,
                                        Email = s.email,
                                        RawAnswers = s.tblAnswers.Where(x => QuestionIDs.Contains(x.questionID)).Select(x => new ArrivalsViewModel.RawAnswer() { QuestionID = x.questionID, Answer = x.answer })
                                    }).ToList();

                foreach (var s in response.Surveys)
                {
                    s.Answers = new List<ArrivalsViewModel.SurveyAnswer>();
                    foreach (var q in Questions.OrderBy(x => x.orderIndex))
                    {
                        ArrivalsViewModel.SurveyAnswer answer = new ArrivalsViewModel.SurveyAnswer();
                        answer.Description = q.description;
                        answer.FieldSubTypeID = q.fieldSubTypeID;
                        if (s.RawAnswers.FirstOrDefault(x => x.QuestionID == q.questionID) != null)
                        {
                            answer.Answer = s.RawAnswers.FirstOrDefault(x => x.QuestionID == q.questionID).Answer;
                            s.Answers.Add(answer);
                        }
                    }
                }

                //prearrivals
                response.Prearrivals = (from r in db.tblReservations
                                        join lead in db.tblLeads on r.leadID equals lead.leadID
                                        join place in db.tblPlaces on r.placeID equals place.placeID
                                        join member in db.tblMemberInfo on lead.leadID equals member.leadID
                                        into leadMember
                                        from member in leadMember.DefaultIfEmpty()
                                        join arrival in db.tblArrivals on lead.leadID equals arrival.leadID
                                        into leadArrival
                                        from arrival in leadArrival.DefaultIfEmpty()
                                        join qualification in db.tblQualificationStatus on arrival.hostessQualificationStatusID equals qualification.qualificationStatusID
                                        into arrivalQualification
                                        from qualification in arrivalQualification.DefaultIfEmpty()
                                        join bookingStatus in db.tblBookingStatus on lead.bookingStatusID equals bookingStatus.bookingStatusID
                                        into lead_bs
                                        from bookingStatus in lead_bs.DefaultIfEmpty()
                                        where r.arrivalDate == date
                                        && lead.initialTerminalID == 10
                                        && place.frontOfficeResortID == frontOfficeResortID
                                        && LeadStatuses.Contains(lead.leadStatusID)
                                        orderby lead.firstName, lead.lastName
                                        select new ArrivalsViewModel.PrearrivalInfo()
                                        {
                                            ArrivalDate = r.arrivalDate,
                                            LeadID = r.leadID,
                                            ReservationID = r.reservationID,
                                            HotelConfirmationNumber = r.hotelConfirmationNumber,
                                            Guest = lead.firstName + " " + lead.lastName,
                                            ClubType = member.clubType,
                                            AccountNumber = member.memberNumber,
                                            ContractNumber = member.contractNumber,
                                            CoOwner = member.coOwner,
                                            PreArrivalOptionsTotal = r.totalPaid,
                                            PreCheckIn = (r.preCheckIn == true ? true : false),
                                            BookingStatusID = lead.bookingStatusID,
                                            BookingStatus = bookingStatus.bookingStatus,
                                            TotalPaid = r.totalPaid,
                                            ConciergeComments = r.conciergeComments,
                                            HostessQualificationStatus = qualification.qualificationStatus,
                                            Interactions = lead.tblInteractions.Count(),
                                            Flights = r.tblFlights.Count(),
                                            Email1 = lead.tblLeadEmails.OrderBy(x => x.emailID).FirstOrDefault().email,
                                            Email2 = lead.tblLeadEmails.OrderBy(x => x.emailID).Skip(1).FirstOrDefault().email,
                                            Phone1 = lead.tblPhones.OrderBy(x => x.phoneID).Take(1).FirstOrDefault().phone,
                                            Phone2 = lead.tblPhones.OrderBy(x => x.phoneID).Skip(1).Take(1).FirstOrDefault().phone,
                                            Address = lead.address,
                                            City = lead.city,
                                            State = lead.state,
                                            CountryID = lead.countryID,
                                            ZipCode = lead.zipcode
                                        }).ToList();

                foreach (var pa in response.Prearrivals)
                {
                    pa.ArrivalDateString = pa.ArrivalDate.Value.ToString("yyyy-MM-dd");
                }
            }
            else
            {
                //prearrivals
                response.Prearrivals = new List<ArrivalsViewModel.PrearrivalInfo>();
            }

            //obtener información del front?
            bool requestArrivalsUpdate = false;
            if (name == null)
            {
                if (date >= DateTime.Today)
                {
                    requestArrivalsUpdate = true;
                    response.IsPast = false;
                }
                else
                {
                    response.IsPast = true;
                    var ArrivalSample = (from a in db.tblArrivals
                                         where a.frontOfficeResortID == frontOfficeResortID
                                        && a.arrivalDate >= date
                                        && a.arrivalDate < tomorrow
                                        && a.reservationStatus != "CA"
                                         && (a.dateLastUpdate == null || a.dateLastUpdate < tomorrow)
                                         select a).FirstOrDefault();
                    if (ArrivalSample != null)
                    {
                        requestArrivalsUpdate = true;
                    }
                    else
                    {
                        //si no hay arrivals capturadas
                        var SavedArrivals = (from a in db.tblArrivals
                                             where a.frontOfficeResortID == frontOfficeResortID
                                         && a.arrivalDate >= date
                                         && a.arrivalDate < tomorrow
                                             select a).Count();
                        if (SavedArrivals == 0)
                        {
                            requestArrivalsUpdate = true;
                        }
                    }
                }
            }

            //si la fecha es >= hoy, consultar Front Office
            if (requestArrivalsUpdate && avoidRequestToFront == false)
            {
                List<FrontOfficeViewModel.LlegadasResult> results = FrontOfficeDataModel.GetArrivals(frontOfficeResortID, (DateTime)date, (DateTime)date);

                foreach (var reservation in results)
                {
                    bool exists = false;

                    var arrival = (from a in db.tblArrivals
                                   where a.frontOfficeReservationID == reservation.idReservacion
                                   && a.frontOfficeResortID == reservation.idresort
                                   select a).FirstOrDefault();

                    if (arrival == null)
                    {
                        arrival = new tblArrivals();
                        arrival.arrivalID = Guid.NewGuid();
                        arrival.dateSaved = DateTime.Now;
                    }
                    else
                    {
                        exists = true;
                    }
                    arrival.terminalID = terminals[0];
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
                    arrival.lastUpdateUserID = session.UserID;
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
                        foreach (var pa in response.Prearrivals)
                        {
                            if (pa.HotelConfirmationNumber != null && pa.HotelConfirmationNumber.Trim() == arrival.crs.Trim())
                            {
                                if (pa.BookingStatusID == 1)
                                {
                                    arrival.manifestedAsPA = true;
                                }
                                arrival.leadID = pa.LeadID;
                                break;
                            }
                        }
                    }
                    if (!exists)
                    {
                        db.tblArrivals.AddObject(arrival);
                        db.SaveChanges();
                    }
                }
            }

            //obtener información de eplat para enviar al cliente
            List<SelectListItem> list = new List<SelectListItem>();
            int? workgroup = session.WorkGroupID;
            var bookingStatusWorkGroup = from bs in db.tblSysWorkGroups_BookingStatus
                                         where bs.sysWorkGroupID == workgroup
                                         orderby bs.orderIndex
                                         select new
                                         {
                                             bs.bookingStatusID,
                                             bs.tblBookingStatus.bookingStatus
                                         };

            var bookingStatusCatalog = from bs in db.tblBookingStatus
                                       select bs;

            var flightTypes = from ft in db.tblFlightTypes
                              select ft;

            var airLines = from a in db.tblAirLines
                           select a;

            //arrivals
            List<tblArrivals> ArrivalsList = new List<tblArrivals>();
            if (name == null)
            {
                ArrivalsList = (from a in db.tblArrivals
                                where a.frontOfficeResortID == frontOfficeResortID
                               && a.arrivalDate >= date
                               && a.arrivalDate < tomorrow
                                //&& a.reservationStatus != "CA"
                                select a).ToList();
            }
            else
            {
                ArrivalsList = (from a in db.tblArrivals
                                where a.frontOfficeResortID == frontOfficeResortID
                            && a.guest.Contains(name)
                                //&& a.reservationStatus != "CA"
                                select a).ToList();
            }

            //iteracion en lista de arrivals
            foreach (var arr in ArrivalsList)
            {
                if (arr.showStatus == null && arr.spiCustomerID != null && arr.presentationDate != null && arr.presentationDate.Value < DateTime.Now)
                {
                    //obtener feedback de SPI
                    SPIModels.TheVillaGroup.spCustomer_tracking_Result saleInfo = new SPIModels.TheVillaGroup.spCustomer_tracking_Result();
                    SPIModels.TheVillaGroup.SpiTvgEntities spidb = new SPIModels.TheVillaGroup.SpiTvgEntities();
                    saleInfo = spidb.spCustomer_tracking("greveles", (int)arr.spiCustomerID).FirstOrDefault();
                    if (saleInfo != null)
                    {
                        if (saleInfo.calificacion == null)
                        {
                            arr.showStatus = false;
                        }
                        else
                        {
                            arr.showStatus = true;
                            arr.salesStatus = saleInfo.Estatus;
                            arr.salesVolume = saleInfo.volumen;
                            if (saleInfo.calificacion.IndexOf("NQ") >= 0)
                            {
                                arr.checkInQualificationStatusID = 2;
                            }
                            else if (saleInfo.calificacion.IndexOf("Q") >= 0)
                            {
                                arr.checkInQualificationStatusID = 1;
                            }
                        }
                    }
                }
                //

                ArrivalsViewModel.ArrivalInfoModel arrival = GetArrivalInfoModel(arr);

                //buscar reservaciones del mismo cliente
                if (arrival.ExtensionArrivalID == null && arr.contract != null && arr.contract != "")
                {
                    var CustomerReservations = (from cr in db.tblArrivals
                                                where cr.contract == arr.contract
                                                && cr.arrivalDate < arrival.ArrivalDateOnly
                                                && !cr.reservationStatus.Contains("CA")
                                                select new
                                                {
                                                    cr.arrivalID,
                                                    cr.arrivalDate,
                                                    cr.nights,
                                                    cr.confirmationNumber
                                                });

                    arrival.ExtensionReservation = "No";
                    foreach (var res in CustomerReservations)
                    {
                        DateTime resDeparture = res.arrivalDate.Date.AddDays(res.nights);
                        if (arrival.ArrivalDateOnly == resDeparture)
                        {
                            arr.extensionFromArrivalID = res.arrivalID;
                            arrival.ExtensionArrivalID = res.arrivalID;
                            arrival.ExtensionReservation = "Yes, from " + res.confirmationNumber;
                        }
                    }
                }
                else
                {
                    if (arrival.ExtensionArrivalID == null)
                    {
                        arrival.ExtensionReservation = "No";
                    }
                    else
                    {
                        arrival.ExtensionReservation = "Yes, from " + arr.tblArrivals2.confirmationNumber;
                    }
                }

                //si es una extensión, marcar como NQ
                if (arr.extensionFromArrivalID != null)
                {
                    arr.hostessQualificationStatusID = 2;
                    arrival.HostessQualificationStatusID = 2;
                }

                //datos clarabridge
                if (name != null)
                {
                    //surveys
                    response.Surveys = (from s in db.tblAppliedSurveys
                                        where SurveyIDs.Contains((int)s.surveyID)
                                        && s.dateIn == arrival.ArrivalDateOnly
                                        && s.Room.Trim() == arrival.RoomNumber
                                        select new ArrivalsViewModel.SurveyInfo()
                                        {
                                            Key = s.key_key,
                                            SentDate = s.sentDate,
                                            SubmittedDate = s.submittedDate,
                                            Rate = s.rate,
                                            ArrivalDate = s.dateIn,
                                            SurveyRoomNumber = s.Room,
                                            Email = s.email,
                                            RawAnswers = s.tblAnswers.Where(x => QuestionIDs.Contains(x.questionID)).Select(x => new ArrivalsViewModel.RawAnswer() { QuestionID = x.questionID, Answer = x.answer })
                                        }).ToList();

                    foreach (var s in response.Surveys)
                    {
                        s.Answers = new List<ArrivalsViewModel.SurveyAnswer>();
                        foreach (var q in Questions.OrderBy(x => x.orderIndex))
                        {
                            ArrivalsViewModel.SurveyAnswer answer = new ArrivalsViewModel.SurveyAnswer();
                            answer.Description = q.description;
                            answer.FieldSubTypeID = q.fieldSubTypeID;
                            if (s.RawAnswers.FirstOrDefault(x => x.QuestionID == q.questionID) != null)
                            {
                                answer.Answer = s.RawAnswers.FirstOrDefault(x => x.QuestionID == q.questionID).Answer;
                                s.Answers.Add(answer);
                            }
                        }
                    }
                }
                ArrivalsViewModel.SurveyInfo su = response.Surveys.FirstOrDefault(x => x.ArrivalDate == arrival.ArrivalDateOnly &&
                    x.SurveyRoomNumber.Trim() == arrival.RoomNumber);
                if (su != null)
                {
                    if (arrival.Email1 == null)
                    {
                        arrival.Email1 = su.Email.Trim();
                    }
                    else
                    {
                        arrival.Email2 = su.Email.Trim();
                    }
                    arrival.SentDate = su.SentDate.Value.ToString("yyyy-MM-dd");
                    arrival.SubmittedDate = su.SubmittedDate.Value.ToString("yyyy-MM-dd");
                    arrival.Rate = decimal.Round((decimal)su.Rate, 2).ToString();
                    arrival.SurveyArrivalDate = su.ArrivalDate;
                    arrival.SurveyRoomNumber = su.SurveyRoomNumber;
                    arrival.Answers = su.Answers;
                }

                //datos de prearrivals
                if (name != null)
                {
                    response.Prearrivals = (from r in db.tblReservations
                                            join lead in db.tblLeads on r.leadID equals lead.leadID
                                            join place in db.tblPlaces on r.placeID equals place.placeID
                                            join member in db.tblMemberInfo on lead.leadID equals member.leadID
                                            into leadMember
                                            from member in leadMember.DefaultIfEmpty()
                                            join tarrival in db.tblArrivals on lead.leadID equals tarrival.leadID
                                            into leadArrival
                                            from tarrival in leadArrival.DefaultIfEmpty()
                                            join qualification in db.tblQualificationStatus on tarrival.hostessQualificationStatusID equals qualification.qualificationStatusID
                                            into arrivalQualification
                                            from qualification in arrivalQualification.DefaultIfEmpty()
                                            join bookingStatus in db.tblBookingStatus on lead.bookingStatusID equals bookingStatus.bookingStatusID
                                        into lead_bs
                                            from bookingStatus in lead_bs.DefaultIfEmpty()
                                            where r.leadID == arrival.LeadID
                                            orderby lead.firstName, lead.lastName
                                            select new ArrivalsViewModel.PrearrivalInfo()
                                            {
                                                ArrivalDate = r.arrivalDate,
                                                LeadID = r.leadID,
                                                ReservationID = r.reservationID,
                                                HotelConfirmationNumber = r.hotelConfirmationNumber,
                                                Guest = lead.firstName + " " + lead.lastName,
                                                ClubType = member.clubType,
                                                AccountNumber = member.memberNumber,
                                                ContractNumber = member.contractNumber,
                                                CoOwner = member.coOwner,
                                                PreArrivalOptionsTotal = r.totalPaid,
                                                PreCheckIn = (r.preCheckIn == true ? true : false),
                                                BookingStatusID = lead.bookingStatusID,
                                                BookingStatus = bookingStatus.bookingStatus,
                                                TotalPaid = r.totalPaid,
                                                ConciergeComments = r.conciergeComments,
                                                HostessQualificationStatus = qualification.qualificationStatus,
                                                Interactions = lead.tblInteractions.Count(),
                                                Flights = r.tblFlights.Count(),
                                                Email1 = lead.tblLeadEmails.OrderBy(x => x.emailID).FirstOrDefault().email,
                                                Email2 = lead.tblLeadEmails.OrderBy(x => x.emailID).Skip(1).FirstOrDefault().email,
                                                Phone1 = lead.tblPhones.OrderBy(x => x.phoneID).Take(1).FirstOrDefault().phone,
                                                Phone2 = lead.tblPhones.OrderBy(x => x.phoneID).Skip(1).Take(1).FirstOrDefault().phone,
                                                Address = lead.address,
                                                City = lead.city,
                                                State = lead.state,
                                                CountryID = lead.countryID,
                                                ZipCode = lead.zipcode
                                            }).ToList();

                    foreach (var par in response.Prearrivals)
                    {
                        par.ArrivalDateString = par.ArrivalDate.Value.ToString("yyyy-MM-dd");
                    }
                }

                ArrivalsViewModel.PrearrivalInfo pa = null;
                if (arrival.LeadID != null)
                {
                    pa = response.Prearrivals.FirstOrDefault(x => x.LeadID == arrival.LeadID);
                }
                if (pa == null)
                {
                    pa = response.Prearrivals.FirstOrDefault(x => x.HotelConfirmationNumber != null && x.HotelConfirmationNumber.Trim() == arrival.Crs.Trim());
                    if (pa != null)
                    {
                        arrival.LeadID = pa.LeadID;
                        arr.leadID = pa.LeadID;
                        if (pa.BookingStatusID == 1)
                        {
                            arrival.ManifestedAsPA = true;
                            arr.manifestedAsPA = true;
                        }
                    }
                }

                if (pa != null)
                {
                    pa.Located = true;
                    pa.ArrivalID = arrival.ArrivalID;
                    //datos relacionados al guest
                    arrival.Email1 = pa.Email1;
                    if (arrival.Picture == null && arrival.Email1 != null && arrival.Email1.Trim() != "")
                    {
                        arrival.Picture = PictureDataModel.GetPicasaAvatar(arrival.Email1.Trim());
                    }
                    arrival.Email2 = pa.Email2;
                    arrival.Phone1 = pa.Phone1;
                    arrival.Phone2 = pa.Phone2;
                    arrival.Address = pa.Address;
                    arrival.City = pa.City;
                    arrival.State = pa.State;
                    if (pa.Phone1 != null && pa.Phone1.Length >= 10)
                    {
                        arrival.StateID = CallClasificationDataModel.GetStateIDByPhone(pa.Phone1);
                    }
                    arrival.CountryID = pa.CountryID;
                    arrival.ZipCode = pa.ZipCode;
                    //datos relacionados al member
                    arrival.ClubType = pa.ClubType;
                    arrival.AccountNumber = pa.AccountNumber;
                    arrival.ContractNumber = pa.ContractNumber;
                    arrival.CoOwner = pa.CoOwner;
                    //datos relacionados al equipo de prearrival
                    arrival.PreArrivalReservationID = pa.ReservationID;
                    arrival.PreArrivalStatus = bookingStatusCatalog.FirstOrDefault(x => x.bookingStatusID == pa.BookingStatusID).bookingStatus;
                    arrival.PreArrivalOptionsTotal = "$" + pa.TotalPaid.ToString();
                    arrival.PreArrivalFeedBack = pa.ConciergeComments;

                    arrival.Interactions = new List<ArrivalsViewModel.Interaction>();
                    if (pa.Interactions > 0)
                    {
                        var Interactions = from interaction in db.tblInteractions
                                           join user in db.tblUserProfiles on interaction.interactedWithUserID equals user.userID
                                           into userInteraction
                                           from user in userInteraction.DefaultIfEmpty()
                                           join type in db.tblInteractionTypes on interaction.interactionTypeID equals type.interactionTypeID
                                           into interactionType
                                           from type in interactionType.DefaultIfEmpty()
                                           where interaction.leadID == pa.LeadID
                                           orderby interaction.interactionID
                                           select new
                                           {
                                               type.interactionType,
                                               interaction.bookingStatusID,
                                               interaction.interactionComments,
                                               interaction.interactedWithUserID,
                                               user.firstName,
                                               user.lastName,
                                               interaction.dateSaved
                                           };
                        foreach (var interaction in Interactions)
                        {
                            ArrivalsViewModel.Interaction newInteraction = new ArrivalsViewModel.Interaction();
                            newInteraction.Type = interaction.interactionType;
                            newInteraction.Status = bookingStatusCatalog.FirstOrDefault(x => x.bookingStatusID == interaction.bookingStatusID).bookingStatus;
                            newInteraction.Comments = interaction.interactionComments;
                            if (interaction.bookingStatusID == 1 && interaction.interactedWithUserID != null)
                            {
                                newInteraction.User = interaction.firstName + " " + interaction.lastName;
                            }
                            newInteraction.Date = interaction.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt");

                            arrival.Interactions.Add(newInteraction);
                        }
                    }

                    arrival.FlightsInfo = new List<ArrivalsViewModel.FlightInfo>();
                    if (pa.Flights > 0)
                    {
                        var Flights = from f in db.tblFlights
                                      where f.reservationID == pa.ReservationID
                                      orderby f.flightDateTime
                                      select new
                                      {
                                          f.flightTypeID,
                                          f.flightDateTime,
                                          f.flightNumber,
                                          f.passengers,
                                          f.airLineID
                                      };

                        foreach (var flight in Flights)
                        {
                            ArrivalsViewModel.FlightInfo newFlight = new ArrivalsViewModel.FlightInfo();
                            newFlight.FlightType = flightTypes.FirstOrDefault(x => x.flightTypeID == flight.flightTypeID).flightType;
                            newFlight.DateTime = flight.flightDateTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                            newFlight.Airline = airLines.FirstOrDefault(x => x.airLineID == flight.airLineID).airLine;
                            newFlight.FlightNumber = flight.flightNumber;
                            newFlight.NumberOfPassengers = flight.passengers;

                            arrival.FlightsInfo.Add(newFlight);
                        }
                    }

                    arrival.ConfirmationLetters = new NotificationsDataModel().SentLetters(pa.ReservationID);

                    //datos a mostrar en pre-checkin
                    arrival.PCIPrearrivalStatus = arrival.PreArrivalStatus;
                    if (arrival.Interactions.Count() > 0)
                    {
                        arrival.PCILastInteractionAgent = arrival.Interactions.LastOrDefault().User;
                        arrival.PCILastInteractionDate = arrival.Interactions.LastOrDefault().Date;
                    }

                    arrival.PCIVipCardType = arrival.VipCardType;
                    arrival.PCIVipCardStatus = arrival.VipCardStatus;
                    arrival.PCIVipCardStatusAgent = arrival.VipCardStatusAgent;
                    arrival.PCICompleted = arr.preCheckIn;
                    arrival.PCIConciergeComments = arr.preCheckInComments;
                }

                if ((arrival.AccountNumber == null || arrival.AccountNumber.Trim() == "") && (arr.contract != null && arr.contract != ""))
                {
                    arrival.AccountNumber = arr.contract;
                }

                //guest hub info
                if (arrival.GuestHubID == null)
                {
                    arrival.GuestHubID = GuestDataModel.GetHubID(arr.frontOfficeResortID, (long)arr.frontOfficeGuestID, arr.firstName, arr.lastName, null, arr.countryCode, arr.source, ref db);
                }


                //guest hub
                var GuestHub = (from g in db.tblGuestsHub
                                where g.guestHubID == arrival.GuestHubID
                                select new
                                {
                                    g.guestHubID,
                                    g.countryID,
                                    g.stateID,
                                    g.city,
                                    g.zipCode,
                                    g.email1
                                }).FirstOrDefault();

                if (GuestHub != null)
                {
                    arrival.CountryID = GuestHub.countryID;
                    arrival.StateID = GuestHub.stateID;
                    arrival.City = GuestHub.city;
                    arrival.ZipCode = GuestHub.zipCode;

                    if (arrival.Email1 == null || arrival.Email1 == "")
                    {
                        arrival.Email1 = GuestHub.email1;
                    }
                    if (arrival.GuestEmail == null || arrival.GuestEmail == "")
                    {
                        arrival.GuestEmail = GuestHub.email1;
                    }
                }

                //guest preferences
                GuestViewModel.PreferencesResponse preferences = GuestDataModel.GetPreferences(arrival.GuestHubID);
                arrival.Preferences = String.Join(",", preferences.PreferencesIDs);

                //AGREGAR ARRIVAL
                response.Arrivals.Add(arrival);

                //summary
                if (response.Summary.Count(x => x.ProgramID == arrival.ProgramID) == 0 && arrival.Program != "Airport")
                {
                    ArrivalsViewModel.ArrivalsSummary summary = new ArrivalsViewModel.ArrivalsSummary();
                    summary.ProgramID = arrival.ProgramID;
                    summary.Program = arrival.Program;
                    summary.Active = 0;
                    summary.CheckedIn = 0;
                    summary.CheckedOut = 0;
                    summary.BookingStatuses = new List<ArrivalsViewModel.ArrivalBookingStatus>();

                    foreach (var bs in bookingStatusWorkGroup)
                    {
                        summary.BookingStatuses.Add(new ArrivalsViewModel.ArrivalBookingStatus()
                        {
                            BookingStatusID = bs.bookingStatusID,
                            BookingStatus = bs.bookingStatus,
                            Amount = 0,
                            Percentage = 0
                        });
                    }
                    response.Summary.Add(summary);
                }
                int? programID = arrival.ProgramID;
                if (arrival.Program == "Airport")
                {
                    programID = response.Summary.FirstOrDefault(x => x.Program == "Inhouse Line").ProgramID;
                }
                arrival.ReservationStatus = arrival.ReservationStatus.Trim();
                if (arrival.ReservationStatus == "A")
                {
                    response.Summary.FirstOrDefault(x => x.ProgramID == programID).Active += 1;
                }
                else
                {
                    if (arrival.ReservationStatus == "IN")
                    {
                        response.Summary.FirstOrDefault(x => x.ProgramID == programID).CheckedIn += 1;
                    }
                    else if (arrival.ReservationStatus == "OUT")
                    {
                        response.Summary.FirstOrDefault(x => x.ProgramID == programID).CheckedOut += 1;
                    }

                    if (arrival.ReservationStatus == "IN" || (arrival.ReservationStatus == "OUT" && response.IsPast))
                    {
                        response.Summary.FirstOrDefault(x => x.ProgramID == programID).BookingStatuses.FirstOrDefault(x => x.BookingStatusID == (arrival.BookingStatusID != null ? arrival.BookingStatusID : bookingStatusWorkGroup.FirstOrDefault().bookingStatusID)).Amount += 1;
                    }
                }
            }
            db.SaveChanges();

            //cálculo de porcentajes
            foreach (var program in response.Summary)
            {
                foreach (var status in program.BookingStatuses)
                {
                    status.Percentage = (program.CheckedIn > 0 ? decimal.Round(status.Amount * 100 / (response.IsPast ? program.CheckedIn + program.CheckedOut : program.CheckedIn), 2) : 0);
                }
            }
            response.Summary = response.Summary.OrderBy(z => z.Program).ToList();

            //power line
            response.PowerLine = GetPowerLine(response.Arrivals);

            //filtro de prearrivals (.Where(x => x.BookingStatusID == 1))
            response.Prearrivals = response.Prearrivals.OrderBy(x => x.BookingStatusID).ThenBy(x => x.Guest).ToList();

            return response;
        }

        public static List<ArrivalsViewModel.TeamPowerLine> GetPowerLine(List<ArrivalsViewModel.ArrivalInfoModel> arrivals)
        {

            List<ArrivalsViewModel.TeamPowerLine> powerline = new List<ArrivalsViewModel.TeamPowerLine>();

            foreach (var arrival in arrivals)
            {
                if (arrival.PromotionTeamID != null && arrival.OPCID != null && arrival.HostessQualificationStatusID != null)
                {
                    //adding team
                    if (powerline.Count(x => x.PromotionTeamID == arrival.PromotionTeamID) == 0)
                    {
                        ArrivalsViewModel.TeamPowerLine team = new ArrivalsViewModel.TeamPowerLine();
                        team.PromotionTeamID = (int)arrival.PromotionTeamID;
                        team.Program = arrival.Program;
                        team.Total = 0;
                        team.Rows = 0;
                        team.BSTotals = GetBookingStatusTotals();
                        team.Promotors = new List<ArrivalsViewModel.PromotorPowerLine>();
                        team.QualificationStatuses = new List<ArrivalsViewModel.QualificationStatusPowerLine>();

                        ArrivalsViewModel.QualificationStatusPowerLine qspl1 = new ArrivalsViewModel.QualificationStatusPowerLine();
                        qspl1.Qualification = "Q";
                        qspl1.Total = 0;
                        qspl1.Percentage = 0;
                        qspl1.Rows = 0;
                        qspl1.BSTotals = GetBookingStatusTotals();
                        team.QualificationStatuses.Add(qspl1);

                        ArrivalsViewModel.QualificationStatusPowerLine qspl2 = new ArrivalsViewModel.QualificationStatusPowerLine();
                        qspl2.Qualification = "NQ";
                        qspl2.Total = 0;
                        qspl2.Percentage = 0;
                        qspl2.Rows = 0;
                        qspl2.BSTotals = GetBookingStatusTotals();
                        team.QualificationStatuses.Add(qspl2);

                        ArrivalsViewModel.QualificationStatusPowerLine qspl3 = new ArrivalsViewModel.QualificationStatusPowerLine();
                        qspl3.Qualification = "PA";
                        qspl3.Total = 0;
                        qspl3.Percentage = 0;
                        qspl3.Rows = 0;
                        qspl3.BSTotals = GetBookingStatusTotals();
                        team.QualificationStatuses.Add(qspl3);

                        powerline.Add(team);
                    }
                    //adding opc
                    if (powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID).Promotors.Count(x => x.OPCID == arrival.OPCID) == 0)
                    {
                        ArrivalsViewModel.PromotorPowerLine promotor = new ArrivalsViewModel.PromotorPowerLine();
                        promotor.OPCID = (long)arrival.OPCID;
                        promotor.OPCName = arrival.OPCName;
                        promotor.Total = 0;
                        promotor.Percentage = 0;
                        promotor.Rows = 0;
                        promotor.BSTotals = GetBookingStatusTotals();
                        promotor.QualificationStatuses = new List<ArrivalsViewModel.QualificationStatusPowerLine>();

                        ArrivalsViewModel.QualificationStatusPowerLine qspl1 = new ArrivalsViewModel.QualificationStatusPowerLine();
                        qspl1.Qualification = "Q";
                        qspl1.Total = 0;
                        qspl1.Percentage = 0;
                        qspl1.Rows = 0;
                        qspl1.BSTotals = GetBookingStatusTotals();
                        qspl1.Guests = new List<ArrivalsViewModel.GuestBookingStatus>();
                        promotor.QualificationStatuses.Add(qspl1);

                        ArrivalsViewModel.QualificationStatusPowerLine qspl2 = new ArrivalsViewModel.QualificationStatusPowerLine();
                        qspl2.Qualification = "NQ";
                        qspl2.Total = 0;
                        qspl2.Percentage = 0;
                        qspl2.Rows = 0;
                        qspl2.BSTotals = GetBookingStatusTotals();
                        qspl2.Guests = new List<ArrivalsViewModel.GuestBookingStatus>();
                        promotor.QualificationStatuses.Add(qspl2);

                        ArrivalsViewModel.QualificationStatusPowerLine qspl3 = new ArrivalsViewModel.QualificationStatusPowerLine();
                        qspl3.Qualification = "PA";
                        qspl3.Total = 0;
                        qspl3.Percentage = 0;
                        qspl3.Rows = 0;
                        qspl3.BSTotals = GetBookingStatusTotals();
                        qspl3.Guests = new List<ArrivalsViewModel.GuestBookingStatus>();
                        promotor.QualificationStatuses.Add(qspl3);

                        powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID).Promotors.Add(promotor);
                    }

                    //adding guest and totals in qualification group
                    ArrivalsViewModel.QualificationStatusPowerLine qualification =
                    powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID)
                        .Promotors.FirstOrDefault(x => x.OPCID == arrival.OPCID)
                        .QualificationStatuses.FirstOrDefault(x => x.Qualification == (arrival.ManifestedAsPA == true ? "PA" : arrival.HostessQualificationStatus));
                    qualification.Total += 1;
                    if (arrival.BookingStatusID != null)
                    {
                        qualification.BSTotals.FirstOrDefault(x => x.BookingStatusID == arrival.BookingStatusID).Amount += 1;
                        if (arrival.BookingStatusID == 7 || arrival.BookingStatusID == 11)
                        {
                            qualification.Bookings += 1;
                        }
                    }

                    ArrivalsViewModel.GuestBookingStatus newGuest = new ArrivalsViewModel.GuestBookingStatus();
                    newGuest.ArrivalID = arrival.ArrivalID;
                    newGuest.Guest = arrival.Guest;
                    newGuest.BookingStatus = GetBookingStatusBits();
                    if (arrival.BookingStatusID != null)
                    {
                        newGuest.BookingStatus.FirstOrDefault(x => x.BookingStatusID == arrival.BookingStatusID).Selected = true;
                    }
                    qualification.Guests.Add(newGuest);

                    //adding totals in qualification group in team
                    ArrivalsViewModel.QualificationStatusPowerLine teamQualification =
                    powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID)
                        .QualificationStatuses.FirstOrDefault(x => x.Qualification == (arrival.ManifestedAsPA == true ? "PA" : arrival.HostessQualificationStatus));
                    teamQualification.Total += 1;
                    if (arrival.BookingStatusID != null)
                    {
                        teamQualification.BSTotals.FirstOrDefault(x => x.BookingStatusID == arrival.BookingStatusID).Amount += 1;
                        if (arrival.BookingStatusID == 7 || arrival.BookingStatusID == 11)
                        {
                            teamQualification.Bookings += 1;
                        }
                    }

                    //adding totals in promotor
                    powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID)
                        .Promotors.FirstOrDefault(x => x.OPCID == arrival.OPCID).Total += 1;
                    if (arrival.BookingStatusID != null)
                    {
                        powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID)
                            .Promotors.FirstOrDefault(x => x.OPCID == arrival.OPCID).BSTotals.FirstOrDefault(x => x.BookingStatusID == arrival.BookingStatusID).Amount += 1;

                        if (arrival.BookingStatusID == 7 || arrival.BookingStatusID == 11)
                        {
                            powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID)
                            .Promotors.FirstOrDefault(x => x.OPCID == arrival.OPCID).Bookings += 1;
                        }
                    }
                    //addding totals in team
                    powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID).Total += 1;
                    if (arrival.BookingStatusID != null)
                    {
                        powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID).BSTotals.FirstOrDefault(x => x.BookingStatusID == arrival.BookingStatusID).Amount += 1;
                        if (arrival.BookingStatusID == 7 || arrival.BookingStatusID == 11)
                        {
                            powerline.FirstOrDefault(x => x.PromotionTeamID == arrival.PromotionTeamID).Bookings += 1;
                        }
                    }
                }
            }

            //percentages bstotals
            foreach (var team in powerline)
            {
                team.BookingPercentage = decimal.Round(team.Bookings * 100 / team.Total, 2);
                foreach (var qs in team.QualificationStatuses)
                {
                    qs.Percentage = decimal.Round(qs.Total * 100 / team.Total, 2);
                    if (qs.Total > 0)
                    {
                        qs.BookingPercentage = decimal.Round(qs.Bookings * 100 / qs.Total, 2);
                        foreach (var bs in qs.BSTotals)
                        {
                            bs.Percentage = decimal.Round(bs.Amount * 100 / qs.Total, 2);
                        }
                    }
                }
                foreach (var bs in team.BSTotals)
                {
                    bs.Percentage = decimal.Round(bs.Amount * 100 / team.Total, 2);
                }
                foreach (var promotor in team.Promotors)
                {
                    promotor.Percentage = decimal.Round(promotor.Total * 100 / team.Total, 2);
                    promotor.BookingPercentage = decimal.Round(promotor.Bookings * 100 / promotor.Total, 2);
                    foreach (var bs in promotor.BSTotals)
                    {
                        bs.Percentage = decimal.Round(bs.Amount * 100 / promotor.Total, 2);
                    }
                    foreach (var qs in promotor.QualificationStatuses)
                    {
                        if (qs.Total > 0)
                        {
                            qs.BookingPercentage = decimal.Round(qs.Bookings * 100 / qs.Total, 2);
                        }
                        qs.Percentage = decimal.Round(qs.Total * 100 / promotor.Total, 2);
                        qs.Rows += (qs.Total > 0 ? qs.Total * 2 : 1);
                        promotor.Rows += (qs.Total > 0 ? qs.Total * 2 : 1);
                        team.Rows += (qs.Total > 0 ? qs.Total * 2 : 1);
                        foreach (var bs in qs.BSTotals)
                        {
                            if (qs.Total > 0)
                            {
                                bs.Percentage = decimal.Round(bs.Amount * 100 / qs.Total, 2);
                            }
                        }
                    }
                }
                //orden de promotores
                team.Promotors = team.Promotors.OrderByDescending(x => x.BookingPercentage).ThenByDescending(x => x.Bookings).ToList();
            }

            //orden de equipos
            powerline = powerline.OrderByDescending(x => x.BookingPercentage).ToList();

            return powerline;
        }

        public static List<ArrivalsViewModel.BookingStatusBit> GetBookingStatusBits()
        {
            ePlatEntities db = new ePlatEntities();
            List<ArrivalsViewModel.BookingStatusBit> list = new List<ArrivalsViewModel.BookingStatusBit>();

            int workGroupID = (int)session.WorkGroupID;

            var bookingStatus = from bs in db.tblSysWorkGroups_BookingStatus
                                where bs.sysWorkGroupID == workGroupID
                                orderby bs.orderIndex
                                select new
                                {
                                    bs.bookingStatusID,
                                    bs.tblBookingStatus.bookingStatus,
                                    bs.tblBookingStatus.bookingStatusCode
                                };

            foreach (var bs in bookingStatus)
            {
                ArrivalsViewModel.BookingStatusBit nbs = new ArrivalsViewModel.BookingStatusBit();
                nbs.BookingStatusID = bs.bookingStatusID;
                nbs.BookingStatus = bs.bookingStatus;
                nbs.BookingStatusCode = bs.bookingStatusCode;
                nbs.Selected = false;
                list.Add(nbs);
            }
            return list;
        }

        public static List<ArrivalsViewModel.BookingStatusTotals> GetBookingStatusTotals()
        {
            ePlatEntities db = new ePlatEntities();
            List<ArrivalsViewModel.BookingStatusTotals> list = new List<ArrivalsViewModel.BookingStatusTotals>();

            int workGroupID = (int)session.WorkGroupID;

            var bookingStatus = from bs in db.tblSysWorkGroups_BookingStatus
                                where bs.sysWorkGroupID == workGroupID
                                orderby bs.orderIndex
                                select new
                                {
                                    bs.bookingStatusID,
                                    bs.tblBookingStatus.bookingStatus,
                                    bs.tblBookingStatus.bookingStatusCode
                                };

            foreach (var bs in bookingStatus)
            {
                ArrivalsViewModel.BookingStatusTotals nbs = new ArrivalsViewModel.BookingStatusTotals();
                nbs.BookingStatusID = bs.bookingStatusID;
                nbs.BookingStatus = bs.bookingStatus;
                nbs.BookingStatusCode = bs.bookingStatusCode;
                nbs.Amount = 0;
                nbs.Percentage = 0;
                list.Add(nbs);
            }
            return list;
        }

        public static ArrivalsViewModel.ArrivalInfoModel GetArrivalInfoModel(tblArrivals arr)
        {
            ArrivalsViewModel.ArrivalInfoModel aiv = new ArrivalsViewModel.ArrivalInfoModel();
            aiv.ArrivalID = arr.arrivalID;
            aiv.LeadID = arr.leadID;
            aiv.ExtensionArrivalID = arr.extensionFromArrivalID;
            aiv.GuestHubID = arr.guestHubID;

            aiv.FrontOfficeReservationID = arr.frontOfficeReservationID;
            aiv.FrontOfficeResortID = arr.frontOfficeResortID;
            aiv.FrontOfficeGuestID = arr.frontOfficeGuestID;
            aiv.ArrivalDateOnly = arr.arrivalDate.Date;
            aiv.ArrivalDate = arr.arrivalDate.ToString("yyyy-MM-dd hh:mm:ss tt");
            aiv.RoomNumber = arr.roomNumber.Trim();
            aiv.Nights = arr.nights;
            aiv.Guest = arr.guest ?? arr.lastName + " " + arr.firstName;
            aiv.FirstName = arr.firstName;
            aiv.LastName = arr.lastName;
            aiv.Adults = arr.adults;
            aiv.Children = arr.children;
            aiv.Infants = arr.infants;
            aiv.Country = arr.country;
            aiv.CountryCode = arr.countryCode;
            aiv.CountryID = arr.countryID;
            aiv.RealCountry = (arr.countryID != null ? arr.tblCountries.country : "");
            aiv.AgencyName = arr.agencyName;
            aiv.AgencyCode = arr.agencyCode;
            aiv.Source = arr.source;
            aiv.MarketCode = arr.marketCode;
            aiv.ReservationStatus = arr.reservationStatus;
            aiv.RoomType = arr.roomType;
            aiv.ConfirmationNumber = arr.confirmationNumber;
            aiv.Crs = arr.crs;
            if (arr.dateLastUpdate != null)
            {
                aiv.DateLastUpdate = arr.dateLastUpdate.Value.ToString("yyyy-MM-dd hh:mm:ss tt");
            }
            aiv.GuestTypeID = arr.arrivalsGuestTypeID;
            aiv.TravelSourceID = arr.arrivalsTravelSourceID;
            aiv.HostessQualificationStatusID = arr.hostessQualificationStatusID;
            aiv.HostessQualificationStatus = (arr.hostessQualificationStatusID != null ? arr.tblQualificationStatus.qualificationStatus : "");
            aiv.Nationality = arr.nationality ?? "";
            aiv.NQReasonID = arr.nqReasonID;
            aiv.NQReason = (arr.nqReasonID != null ? arr.tblNqReasons.nqReason : "");
            aiv.ProgramID = arr.programID;
            aiv.Program = (arr.programID != null ? arr.tblProspectationPrograms.program : "");
            aiv.PromotionTeamID = arr.promotionTeamID;
            aiv.OPCID = arr.opcID;
            aiv.OPCName = (arr.opcID != null ? arr.tblOPCS.firstName + " " + arr.tblOPCS.lastName : "");
            aiv.BookingStatusID = arr.bookingStatusID;
            aiv.BookingStatus = (arr.bookingStatusID != null ? arr.tblBookingStatus.bookingStatus : "");
            if (arr.hostessUserID != null)
            {
                tblUserProfiles hostess = arr.aspnet_Users1.tblUserProfiles.FirstOrDefault();
                aiv.HostessName = hostess.firstName + " " + hostess.lastName;
            }
            aiv.HostessInputDateTime = (arr.hostessInputDateTime != null ? arr.hostessInputDateTime.Value.ToString("yyyy-MM-dd hh:mm:ss") : "");
            aiv.Comments = arr.hostessComments ?? "";
            aiv.PresentationDate = (arr.presentationDate != null ? arr.presentationDate.Value.ToString("yyyy-MM-dd") : "");
            aiv.SalesRoomPartyID = arr.salesRoomPartyID;
            aiv.SalesRoom = (arr.salesRoomPartyID != null ? arr.tblSalesRoomsParties.tblSalesRooms.salesRoom : "");
            aiv.Party = (arr.salesRoomPartyID != null ? arr.tblSalesRoomsParties.partyDateTime.ToString("hh:mm tt") : "");
            aiv.Gifting = arr.gifting ?? "";
            aiv.Deposit = arr.deposit ?? 0;
            aiv.DepositCurrencyID = arr.depositCurrencyID;
            aiv.DepositCurrency = (arr.depositCurrencyID != null ? arr.tblCurrencies.currencyCode : "");
            aiv.InvitationNumber = arr.invitationNumber ?? "";
            aiv.InvitationID = arr.invitationID;
            aiv.PresentationPax = arr.presentationPax ?? 0;
            aiv.ShowStatus = arr.showStatus;
            aiv.SalesStatus = arr.salesStatus;
            aiv.SalesVolume = arr.salesVolume;
            aiv.SPICustomerID = arr.spiCustomerID;
            aiv.CheckInQualificationStatusID = arr.checkInQualificationStatusID;
            aiv.CheckInQualificationStatus = (arr.checkInQualificationStatusID != null ? arr.tblQualificationStatus1.qualificationStatus : "");
            aiv.CheckInTourStatusID = arr.checkInTourStatusID;
            aiv.VipCardType = arr.vipCardType;
            aiv.VipCardStatus = arr.vipCardStatus;
            if (arr.vipCardStatusByUserID != null)
            {
                tblUserProfiles vipCardAgent = arr.aspnet_Users3.tblUserProfiles.FirstOrDefault();
                aiv.VipCardStatusAgent = vipCardAgent.firstName + " " + vipCardAgent.lastName;
            }
            aiv.ManifestedAsPA = arr.manifestedAsPA;

            aiv.GuestEmail = arr.guestEmail;
            aiv.Confirmed = arr.confirmed;
            if (arr.pickUpTime != null)
            {
                aiv.PickUpTimeHour = ((TimeSpan)arr.pickUpTime).ToString("hh");
                aiv.PickUpTimeMinute = ((TimeSpan)arr.pickUpTime).ToString("mm");
                aiv.PickUpTimeMeridian = (arr.pickUpTime > TimeSpan.Parse("12:00:00") ? "p. m." : "a. m.");
            }
            aiv.InvitationGenerated = (arr.invitationID != null ? "Yes" : "Not yet");
            aiv.PreCheckIn = (arr.preCheckIn == true ? "Yes" : "No");
            aiv.ExtensionReservation = (arr.extensionFromArrivalID != null ? "Yes" : "No");
            if (arr.checkinDateTime != null)
            {
                aiv.CheckinDateTime = arr.checkinDateTime.Value.ToString("yyyy-MM-dd hh:mm:ss tt");
            }
            aiv.Contract = arr.contract;
            aiv.PlanType = arr.planType;
            aiv.PrintedLetterOnHand = arr.printedLetterOnHand;

            aiv.Interactions = new List<ArrivalsViewModel.Interaction>();
            aiv.FlightsInfo = new List<ArrivalsViewModel.FlightInfo>();

            aiv.Answers = new List<ArrivalsViewModel.SurveyAnswer>();

            return aiv;
        }

        public static List<SelectListItem> GetParties(DateTime date, int programID)
        {
            ePlatEntities db = new ePlatEntities();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];
            DateTime tomorrow = date.AddDays(1);
            List<SelectListItem> parties = new List<SelectListItem>();
            var SalesRooms = from n in db.tblSalesRoomsParties
                             where n.terminalID == terminalID
                             && n.programID == programID
                             && n.partyDateTime >= date
                             && n.partyDateTime < tomorrow
                             && n.isTemplate == false
                             orderby n.partyDateTime
                             select new
                             {
                                 n.partyDateTime,
                                 n.salesRoomPartyID
                             };

            if (SalesRooms.Count() > 0)
            {
                foreach (var party in SalesRooms)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = party.salesRoomPartyID.ToString();
                    item.Text = party.partyDateTime.ToString("hh:mm tt");
                    parties.Add(item);
                }
            }
            else
            {
                //si no hay parties, crear el default
                var PartyTemplates = from t in db.tblSalesRoomsParties
                                     where t.terminalID == terminalID
                                     && t.isTemplate == true
                                     && t.programID == programID
                                     select t;

                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        PartyTemplates = PartyTemplates.Where(x => x.monday == true);
                        break;
                    case DayOfWeek.Tuesday:
                        PartyTemplates = PartyTemplates.Where(x => x.tuesday == true);
                        break;
                    case DayOfWeek.Wednesday:
                        PartyTemplates = PartyTemplates.Where(x => x.wednesday == true);
                        break;
                    case DayOfWeek.Thursday:
                        PartyTemplates = PartyTemplates.Where(x => x.thursday == true);
                        break;
                    case DayOfWeek.Friday:
                        PartyTemplates = PartyTemplates.Where(x => x.friday == true);
                        break;
                    case DayOfWeek.Saturday:
                        PartyTemplates = PartyTemplates.Where(x => x.saturday == true);
                        break;
                    case DayOfWeek.Sunday:
                        PartyTemplates = PartyTemplates.Where(x => x.sunday == true);
                        break;
                }

                foreach (var party in PartyTemplates)
                {
                    tblSalesRoomsParties newParty = new tblSalesRoomsParties();
                    newParty.placeID = party.placeID;
                    newParty.salesRoomID = party.salesRoomID;
                    newParty.partyDateTime = DateTime.Parse(date.ToString("yyyy-MM-dd ") + party.partyDateTime.ToString("hh:mm:ss"));
                    newParty.allotment = party.allotment;
                    newParty.programID = party.programID;
                    newParty.terminalID = party.terminalID;
                    newParty.dateSaved = DateTime.Now;
                    newParty.savedByUserID = session.UserID;
                    db.tblSalesRoomsParties.AddObject(newParty);
                }
                db.SaveChanges();

                var Parties = from n in db.tblSalesRoomsParties
                              where n.terminalID == terminalID
                              && n.programID == programID
                              && n.partyDateTime >= date
                              && n.partyDateTime < tomorrow
                              && n.isTemplate == false
                              orderby n.partyDateTime
                              select new
                              {
                                  n.partyDateTime,
                                  n.salesRoomPartyID
                              };
                foreach (var party in Parties)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = party.salesRoomPartyID.ToString();
                    item.Text = party.partyDateTime.ToString("hh:mm tt");
                    parties.Add(item);
                }
            }

            parties.Insert(0, ListItems.Default("--Select One--", ""));

            return parties;
        }

        public static List<SelectListItem> GetReservationStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "Any",
                Value = ""
            });
            list.Add(new SelectListItem()
            {
                Text = "Active",
                Value = "A"
            });
            list.Add(new SelectListItem()
            {
                Text = "Checked In",
                Value = "IN"
            });
            list.Add(new SelectListItem()
            {
                Text = "Checked Out",
                Value = "OUT"
            });
            return list;
        }

        public static List<SelectListItem> GetCardTypes()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> cardTypes = new List<SelectListItem>();
            cardTypes.Add(new SelectListItem()
            {
                Value = "",
                Text = "--Select One--"
            });

            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];

            int? companyGroupID = db.tblTerminals.FirstOrDefault(x => x.terminalID == terminalID).companiesGroupID;


            var destinationID = (from d in db.tblTerminals_Destinations
                                 where d.terminalID == terminalID
                                 select d.destinationID).FirstOrDefault();

            var CreditCardTypes = from c in db.tblDiscountCards
                                  where c.destinationID == destinationID
                                  && c.companiesGroupID == companyGroupID
                                  select new
                                  {
                                      c.discountCardID,
                                      c.discountCard,
                                      c.discount
                                  };

            foreach (var card in CreditCardTypes)
            {
                cardTypes.Add(new SelectListItem()
                {
                    Value = card.discountCard,
                    Text = card.discountCard + " " + card.discount + "%"
                });
            }

            return cardTypes;
        }

        public static DependantFields GetDependantFields()
        {
            ePlatEntities db = new ePlatEntities();
            DependantFields df = new DependantFields();
            df.Fields = new List<DependantFields.DependantField>();
            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];

            int? companyGroupID = db.tblTerminals.FirstOrDefault(x => x.terminalID == terminalID).companiesGroupID;

            //ProgramID > TravelSourceID
            DependantFields.DependantField TravelSourceID = new DependantFields.DependantField();
            TravelSourceID.Field = "TravelSourceID";
            TravelSourceID.ParentField = "ProgramID";
            TravelSourceID.Values = new List<DependantFields.FieldValue>();

            var TravelSourcesQ = from t in db.tblArrivalsTravelSources
                                 where t.tblProspectationPrograms.companiesGroupID == companyGroupID
                                 orderby t.arrivalsGuestTypeID, t.arrivalsTravelSource
                                 select new
                                 {
                                     t.arrivalsTravelSourceID,
                                     t.arrivalsTravelSource,
                                     t.programID
                                 };

            foreach (var ts in TravelSourcesQ)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = ts.programID;
                val.Value = ts.arrivalsTravelSourceID.ToString();
                val.Text = ts.arrivalsTravelSource;
                TravelSourceID.Values.Add(val);
            }

            DependantFields.FieldValue valDefault = new DependantFields.FieldValue();
            valDefault.ParentValue = null;
            valDefault.Value = "";
            valDefault.Text = "--Select One--";
            TravelSourceID.Values.Insert(0, valDefault);

            df.Fields.Add(TravelSourceID);

            //ProgramID > PromotionTeamID
            DependantFields.DependantField PromotionTeamID = new DependantFields.DependantField();
            PromotionTeamID.Field = "PromotionTeamID";
            PromotionTeamID.ParentField = "ProgramID";
            PromotionTeamID.Values = new List<DependantFields.FieldValue>();

            var promoTeams = from t in db.tblTerminals_PromotionTeams
                             where t.terminalID == terminalID
                             orderby t.tblPromotionTeams.promotionTeam
                             select new
                             {
                                 t.tblPromotionTeams.promotionTeam,
                                 t.promotionTeamID,
                                 t.tblPromotionTeams.programID
                             };

            foreach (var team in promoTeams)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = team.promotionTeamID.ToString();
                val.Text = team.promotionTeam;
                PromotionTeamID.Values.Add(val);
            }
            PromotionTeamID.Values.Insert(0, valDefault);
            df.Fields.Add(PromotionTeamID);

            //PromotionTeamID > OPCID
            DependantFields.DependantField OPCID = new DependantFields.DependantField();
            OPCID.Field = "OPCID";
            OPCID.ParentField = "PromotionTeamID";
            OPCID.Values = new List<DependantFields.FieldValue>();

            List<int> promoTeamsArr = promoTeams.Select(x => x.promotionTeamID).ToList();

            var OPCSQ = (from o in db.tblOPC_PromotionTeams
                         where promoTeamsArr.Contains(o.promotionTeamID)
                         && o.deleted != true
                         select new
                         {
                             o.opcID,
                             o.tblOPCS.opc,
                             o.promotionTeamID
                         }).Distinct();

            foreach (var opc in OPCSQ.OrderBy(x => x.opc))
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = opc.promotionTeamID;
                val.Value = opc.opcID.ToString();
                val.Text = opc.opc;
                OPCID.Values.Add(val);
            }
            OPCID.Values.Insert(0, valDefault);
            df.Fields.Add(OPCID);

            //CountryID > StateID
            DependantFields.DependantField StateID = new DependantFields.DependantField();
            StateID.Field = "StateID";
            StateID.ParentField = "CountryID";
            StateID.Values = new List<DependantFields.FieldValue>();

            var States = from s in db.tblStates
                         select s;

            foreach (var state in States)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = state.countryID;
                val.Value = state.stateID.ToString();
                val.Text = state.state;
                StateID.Values.Add(val);
            }
            StateID.Values.Insert(0, valDefault);
            df.Fields.Add(StateID);

            //ProgramID > NQReasonID
            DependantFields.DependantField NQReasonID = new DependantFields.DependantField();
            NQReasonID.Field = "NQReasonID";
            NQReasonID.ParentField = "ProgramID";
            NQReasonID.Values = new List<DependantFields.FieldValue>();

            var DestinationID = (from d in db.tblTerminals_Destinations
                                 where d.terminalID == terminalID
                                 select d.destinationID).FirstOrDefault();

            var Programs = from n in db.tblProspectationPrograms
                           where n.companiesGroupID == companyGroupID
                           orderby n.program
                           select new
                           {
                               n.programID,
                               n.program
                           };

            foreach (var program in Programs)
            {
                //revisar nqreasons para ese programa
                var NQReasons = from n in db.tblNqReasons
                                where n.programID == program.programID
                                    && n.destinationID == DestinationID
                                    && n.companiesGroupID == companyGroupID
                                    && (n.permanent_ == true && n.fromDate <= DateTime.Now || n.toDate > DateTime.Now)
                                orderby n.nqReason
                                select n;

                if (NQReasons.Count() > 0)
                {
                    foreach (var nq in NQReasons)
                    {
                        DependantFields.FieldValue val = new DependantFields.FieldValue();
                        val.ParentValue = program.programID;
                        val.Value = nq.nqReasonID.ToString();
                        val.Text = nq.nqReason;
                        NQReasonID.Values.Add(val);
                    }
                }
                else
                {
                    var NQDefaultReasons = from r in db.tblNqReasons
                                           where r.programID == null
                                            && r.destinationID == DestinationID
                                            && r.companiesGroupID == companyGroupID
                                           orderby r.nqReason
                                           select r;

                    foreach (var nq in NQDefaultReasons)
                    {
                        DependantFields.FieldValue val = new DependantFields.FieldValue();
                        val.ParentValue = program.programID;
                        val.Value = nq.nqReasonID.ToString();
                        val.Text = nq.nqReason;
                        NQReasonID.Values.Add(val);
                    }
                }
            }
            NQReasonID.Values.Insert(0, valDefault);
            df.Fields.Add(NQReasonID);

            return df;
        }

        public static AttemptResponse CleanDuplicates(DateTime date)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse attempt = new AttemptResponse();

            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];
            DateTime nextDate = date.AddDays(1);

            List<tblArrivals> Duplicates = new List<tblArrivals>();
            var Arrivals = from d in db.tblArrivals
                           where d.terminalID == terminalID
                           && d.arrivalDate >= date && d.arrivalDate < nextDate
                           select d;

            foreach (var arrival in Arrivals)
            {
                tblArrivals duplicate = null;
                duplicate = Arrivals.FirstOrDefault(x => x.frontOfficeReservationID == arrival.frontOfficeReservationID && x.roomNumber == arrival.roomNumber && x.arrivalID != arrival.arrivalID);
                if (duplicate != null)
                {
                    if (duplicate.hostessUserID == null && Duplicates.Count(x => x.frontOfficeReservationID == arrival.frontOfficeReservationID) == 0)
                    {
                        Duplicates.Add(duplicate);
                    }
                }
            }

            //eliminar de la base
            try
            {
                foreach (var toDelete in Duplicates)
                {
                    db.tblArrivals.DeleteObject(toDelete);
                }
                if (Duplicates.Count() > 0)
                {
                    db.SaveChanges();
                }
                attempt.Type = Attempt_ResponseTypes.Ok;
                attempt.Message = "Duplicates were successfully cleaned.";
            }
            catch (Exception e)
            {
                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Error trying to clean duplicates.";
            }

            return attempt;
        }

        public static AttemptResponse SaveArrivalInfo(ArrivalsViewModel.ArrivalInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse attempt = new AttemptResponse();

            var Arrival = (from a in db.tblArrivals
                           where a.arrivalID == model.ArrivalID
                           select a).FirstOrDefault();

            if (Arrival != null)
            {
                if (model.LeadID != null && Arrival.leadID == null)
                {
                    Arrival.leadID = model.LeadID;
                    Arrival.manifestedAsPA = true;
                }
                if (model.CountryID != 0)
                {
                    Arrival.countryID = model.CountryID;
                }
                if (model.GuestTypeID != 0)
                {
                    Arrival.arrivalsGuestTypeID = model.GuestTypeID;
                }
                if (model.TravelSourceID != 0)
                {
                    Arrival.arrivalsTravelSourceID = model.TravelSourceID;
                }
                if (model.HostessQualificationStatusID != 0)
                {
                    if (Arrival.hostessInputDateTime == null)
                    {
                        Arrival.hostessInputDateTime = DateTime.Now;
                        Arrival.hostessUserID = session.UserID;
                    }
                    Arrival.hostessQualificationStatusID = model.HostessQualificationStatusID;
                }
                Arrival.nationality = model.Nationality;
                if (model.NQReasonID != 0)
                {
                    Arrival.nqReasonID = model.NQReasonID;
                }
                if (model.ProgramID != 0)
                {
                    Arrival.programID = model.ProgramID;
                }
                if (model.PromotionTeamID != 0)
                {
                    Arrival.promotionTeamID = model.PromotionTeamID;
                }
                Arrival.opcID = model.OPCID;
                if (model.BookingStatusID != 0)
                {
                    Arrival.bookingStatusID = model.BookingStatusID;
                }
                Arrival.hostessComments = model.Comments;

                if (model.PresentationDate != null)
                {
                    if (Arrival.manifestInputDateTime == null)
                    {
                        Arrival.manifestInputDateTime = DateTime.Now;
                    }
                    Arrival.presentationDate = DateTime.Parse(model.PresentationDate);
                }
                if (model.VipCardType != null && model.VipCardType != "")
                {
                    Arrival.vipCardType = model.VipCardType;
                    if (Arrival.vipCardStatus != model.VipCardStatus)
                    {
                        Arrival.vipCardStatus = model.VipCardStatus;
                        Arrival.vipCardStatusByUserID = session.UserID;
                        Arrival.vipCardStatusDate = DateTime.Now;
                    }
                }

                if ((Arrival.presentationDate != null && Arrival.presentationDate.Value.ToString("yyyy-MM-dd") != model.PresentationDate)
                    || Arrival.salesRoomPartyID != model.SalesRoomPartyID
                    || Arrival.deposit != model.Deposit
                    || Arrival.depositCurrencyID != model.DepositCurrencyID
                    || Arrival.invitationNumber != model.InvitationNumber
                    || Arrival.presentationPax != model.PresentationPax
                    || Arrival.gifting != model.Gifting)
                {
                    Arrival.manifestUpdateDateTime = DateTime.Now;
                    Arrival.manifestUpdateUserID = session.UserID;
                }

                if (model.SalesRoomPartyID != 0)
                {
                    Arrival.salesRoomPartyID = model.SalesRoomPartyID;
                }
                Arrival.deposit = model.Deposit;
                Arrival.depositCurrencyID = model.DepositCurrencyID;
                Arrival.invitationNumber = model.InvitationNumber;
                Arrival.presentationPax = model.PresentationPax;
                Arrival.gifting = model.Gifting;

                if (Arrival.spiCustomerID == null)
                {
                    if (model.SPICustomerID != null)
                    {
                        Arrival.spiCustomerID = model.SPICustomerID;
                        if (Arrival.spiCustomerID != null)
                        {
                            Arrival.checkInFeedBackDateTime = DateTime.Now;
                            Arrival.checkInFeedBackUserID = session.UserID;
                        }
                    }
                    else if (model.SPIRelatedCustomerID != null)
                    {
                        Arrival.spiCustomerID = model.SPIRelatedCustomerID;
                    }
                }

                //nuevos campos de invitación
                if (model.PickUpTimeHour != null && model.PickUpTimeMinute != null && model.PickUpTimeMeridian != null)
                {
                    Arrival.pickUpTime = TimeSpan.Parse((model.PickUpTimeMeridian == "p. m." && int.Parse(model.PickUpTimeHour) < 12 ? (int.Parse(model.PickUpTimeHour) + 12).ToString() : model.PickUpTimeHour) + ":" + model.PickUpTimeMinute + ":00");
                }
                if (model.GuestEmail != null)
                {
                    Arrival.guestEmail = model.GuestEmail;
                }
                if (model.Confirmed != null)
                {
                    Arrival.confirmed = model.Confirmed;
                }
                if (model.PrintedLetterOnHand != null)
                {
                    Arrival.printedLetterOnHand = model.PrintedLetterOnHand;
                }

                //si no está enlazada a una invitación, comprobar datos y generarla
                if (Arrival.invitationID == null)
                {
                    if (
                        Arrival.presentationDate != null
                        && Arrival.salesRoomPartyID != null
                        && Arrival.invitationNumber != null
                        && Arrival.pickUpTime != null
                        && Arrival.guestEmail != null
                        && Arrival.confirmed != null)
                    {
                        //guardar invitación
                        var Terminal = (from t in db.tblTerminals
                                        where t.terminalID == Arrival.terminalID
                                        select new
                                        {
                                            t.tblTerminals_Destinations.FirstOrDefault().destinationID,
                                            t.placeID,
                                            t.invitationTerminalID
                                        }).FirstOrDefault();
                        if (Terminal.invitationTerminalID != null)
                        {
                            InvitationViewModel.InvitationInfoModel invitation = new InvitationViewModel.InvitationInfoModel();
                            invitation.InvitationID = null;
                            invitation.PresentationDate = model.PresentationDate;
                            invitation.Guest = Arrival.guest;
                            if (Arrival.countryCode == "US")
                            {
                                invitation.CountryID = 1;
                                invitation.Culture = "en-US";
                            }
                            else if (Arrival.countryCode == "CA")
                            {
                                invitation.CountryID = 2;
                                invitation.Culture = "en-US";
                            }
                            else if (Arrival.countryCode == "MX")
                            {
                                invitation.CountryID = 3;
                                invitation.Culture = "es-MX";
                            }
                            invitation.State = "Unknown";
                            if (Terminal.destinationID != null)
                            {
                                invitation.DestinationID = Terminal.destinationID;
                            }
                            if (Terminal.placeID != null)
                            {
                                invitation.PlaceID = Terminal.placeID;
                            }
                            if (Arrival.deposit != null)
                            {
                                invitation.Deposit = Arrival.deposit.ToString() + "," + (Arrival.depositCurrencyID == 1 ? "USD" : "MXN");
                            }
                            else
                            {
                                invitation.Deposit = "0,USD";
                            }
                            invitation.PickUpTimeHour = model.PickUpTimeHour;
                            invitation.PickUpTimeMinute = model.PickUpTimeMinute;
                            invitation.PickUpTimeMeridian = model.PickUpTimeMeridian;

                            var partyTime = (from p in db.tblSalesRoomsParties
                                             where p.salesRoomPartyID == Arrival.salesRoomPartyID
                                             select p.partyDateTime).FirstOrDefault();
                            invitation.PresentationTimeHour = partyTime.ToString("hh");
                            invitation.PresentationTimeMinute = partyTime.ToString("mm");
                            invitation.PresentationTimeMeridian = partyTime.ToString("tt");

                            invitation.GuestEmail = Arrival.guestEmail;
                            invitation.InvitationNumber = Arrival.invitationNumber;
                            invitation.Gifts = Arrival.gifting;
                            invitation.ProgramID = (int)Arrival.programID;
                            invitation.Confirmed = (bool)Arrival.confirmed;
                            invitation.PromotionTeamID = Arrival.promotionTeamID;
                            invitation.OPCID = Arrival.opcID;

                            invitation.TerminalID = Terminal.invitationTerminalID;

                            AttemptResponse response = InvitationDataModel.SaveInvitationInfo(invitation);
                            Arrival.invitationID = new Guid(response.ObjectID.ToString());
                        }
                    }
                }

                //prearrival feedback
                if (model.PreArrivalReservationID != new Guid("00000000-0000-0000-0000-000000000000") && model.PreArrivalFeedBack != null && model.PreArrivalFeedBack != "")
                {
                    var PAReservation = (from r in db.tblReservations
                                         where r.reservationID == model.PreArrivalReservationID
                                         select r).FirstOrDefault();

                    if (PAReservation != null)
                    {
                        PAReservation.conciergeComments = model.PreArrivalFeedBack;
                    }
                }

                //guest info tab
                if (model.PCICompleted == true && (Arrival.preCheckIn == null || Arrival.preCheckIn == false))
                {
                    Arrival.preCheckIn = true;
                }
                Arrival.preCheckInComments = model.PCIConciergeComments;

                if (model.PCIVipCardType != null && model.VipCardType == null && model.VipCardType != model.PCIVipCardType)
                {
                    Arrival.vipCardType = model.PCIVipCardType;
                    if (Arrival.vipCardStatus != model.PCIVipCardStatus)
                    {
                        Arrival.vipCardStatus = model.PCIVipCardStatus;
                        Arrival.vipCardStatusByUserID = session.UserID;
                        Arrival.vipCardStatusDate = DateTime.Now;
                    }
                }

                if (Arrival.guestHubID == null)
                {
                    Guid GuestHubID = Guid.Parse("00000000-0000-0000-0000-000000000000");
                    string email = "";
                    if (model.GuestEmail != null && model.GuestEmail != "")
                    {
                        email = model.GuestEmail;
                    }
                    if (email == "")
                    {
                        if (model.Email1 != null && model.Email1 != "")
                        {
                            email = model.Email1;
                        }
                    }
                    if (email == "")
                    {
                        FrontOfficeViewModel.ContactInfo contact = FrontOfficeDataModel.GetContactInfo(Arrival.frontOfficeResortID, Arrival.frontOfficeGuestID);
                        if (contact.Email != null && contact.Email != "")
                        {
                            email = contact.Email;
                        }
                    }

                    if (email != "")
                    {
                        GuestHubID = GuestDataModel.GetGuestHubID((long)Arrival.frontOfficeGuestID, Arrival.frontOfficeResortID, email);
                    }
                    else
                    {
                        GuestHubID = GuestDataModel.GetGuestHubID((long)Arrival.frontOfficeGuestID, Arrival.frontOfficeResortID);
                    }


                    if (GuestHubID == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                    {
                        GuestHubID = GuestDataModel.SaveNewGuest(
                            Arrival.frontOfficeResortID,
                            Arrival.frontOfficeGuestID,
                            Arrival.firstName,
                            Arrival.lastName,
                            null,
                            email,
                            Arrival.countryCode,
                            "ePlat - Concierge SaveArrivalInfo",
                            null
                            );
                    }

                    Arrival.guestHubID = GuestHubID;
                }

                var GuestHub = (from g in db.tblGuestsHub
                                where g.guestHubID == Arrival.guestHubID
                                select g).FirstOrDefault();

                if (model.GuestEmail != null && model.GuestEmail != "")
                {
                    if (model.GuestEmail != GuestHub.email1 && model.GuestEmail != GuestHub.email2 && model.GuestEmail != GuestHub.email3)
                    {
                        if (GuestHub.email1 == null)
                        {
                            GuestHub.email1 = model.GuestEmail;
                        }
                        else
                        {
                            if (GuestHub.email2 == null)
                            {
                                GuestHub.email2 = model.GuestEmail;
                            }
                            else
                            {
                                if (GuestHub.email3 == null)
                                {
                                    GuestHub.email3 = model.GuestEmail;
                                }
                            }
                        }
                    }

                }
                if (model.Email1 != null && model.Email1 != "")
                {
                    if (model.Email1 != GuestHub.email1 && model.Email1 != GuestHub.email2 && model.Email1 != GuestHub.email3)
                    {
                        if (GuestHub.email1 == null)
                        {
                            GuestHub.email1 = model.Email1;
                        }
                        else
                        {
                            if (GuestHub.email2 == null)
                            {
                                GuestHub.email2 = model.Email1;
                            }
                            else
                            {
                                if (GuestHub.email3 == null)
                                {
                                    GuestHub.email3 = model.Email1;
                                }
                            }
                        }
                    }
                }
                if (model.Email2 != null && model.Email2 != "")
                {
                    if (model.Email2 != GuestHub.email1 && model.Email2 != GuestHub.email2 && model.Email2 != GuestHub.email3)
                    {
                        if (GuestHub.email1 == null)
                        {
                            GuestHub.email1 = model.Email2;
                        }
                        else
                        {
                            if (GuestHub.email2 == null)
                            {
                                GuestHub.email2 = model.Email2;
                            }
                            else
                            {
                                if (GuestHub.email3 == null)
                                {
                                    GuestHub.email3 = model.Email2;
                                }
                            }
                        }
                    }
                }
                if (model.CountryID != null && model.CountryID != 0)
                {
                    GuestHub.countryID = model.CountryID;
                }
                if (model.StateID != null)
                {
                    GuestHub.stateID = model.StateID;
                }
                if (model.City != null && model.City != "")
                {
                    GuestHub.city = model.City;
                }
                if (model.ZipCode != null && model.ZipCode != "")
                {
                    GuestHub.zipCode = model.ZipCode;
                }

                //GuestViewModel.PreferencesModel preferencesModel = new GuestViewModel.PreferencesModel();
                //preferencesModel.PreferencesIDs = model.Preferences;
                //GuestDataModel.UpdatePreferences((Guid)Arrival.guestHubID, preferencesModel);

                db.SaveChanges();
                attempt.ObjectID = GetArrivalInfoModel(Arrival);
                attempt.Type = Attempt_ResponseTypes.Ok;
            }
            else
            {
                attempt.Type = Attempt_ResponseTypes.Error;
            }

            attempt.Exception = null;
            return attempt;
        }

        public ArrivalsViewModel.GlobalPenetrationReport SearchGlobalPenetrationReport(ArrivalsViewModel.SearchGlobalPenetrationReport model)
        {
            ePlatEntities db = new ePlatEntities();
            ArrivalsViewModel.GlobalPenetrationReport report = new ArrivalsViewModel.GlobalPenetrationReport();

            DateTime fromDate = DateTime.Parse(model.SearchGlobalPenetration_I_ArrivalDate);
            DateTime toDate = DateTime.Parse(model.SearchGlobalPenetration_F_ArrivalDate);
            DateTime toDateNextDay = toDate.AddDays(1);

            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];

            if (toDate.Month == fromDate.Month)
            {
                if (toDate.Day == fromDate.Day)
                {
                    report.DatesRange = Utils.GeneralFunctions.DateFormat.MonthName(fromDate.Month, "en-US") + " " + fromDate.Day;
                }
                else
                {
                    report.DatesRange = fromDate.Day + " to " + toDate.Day;
                }
            }
            else
            {
                report.DatesRange = Utils.GeneralFunctions.DateFormat.MonthName(fromDate.Month, "en-US") + " " + fromDate.Day + " to " + Utils.GeneralFunctions.DateFormat.MonthName(toDate.Month, "en-US") + " " + toDate.Day;
            }

            report.PenetrationPerDay = new List<ArrivalsViewModel.PenetrationPerDay>();
            report.Totals = new ArrivalsViewModel.PenetrationPerDay();
            report.Booked = new ArrivalsViewModel.PenetrationPerDay();
            report.PenetrationPercentages = new ArrivalsViewModel.PenetrationPercentages();
            report.InHouseNQReasons = new List<ArrivalsViewModel.NQReasonTotals>();
            report.MembersNQReasons = new List<ArrivalsViewModel.NQReasonTotals>();

            var arrivals = from a in db.tblArrivals
                           join program in db.tblProspectationPrograms on a.programID equals program.programID
                           into arrivalProgram
                           from program in arrivalProgram.DefaultIfEmpty()
                           join nqreason in db.tblNqReasons on a.nqReasonID equals nqreason.nqReasonID
                           into arrivalNqReason
                           from nqreason in arrivalNqReason.DefaultIfEmpty()
                           where a.terminalID == terminalID
                           && a.arrivalDate >= fromDate
                           && a.arrivalDate < toDateNextDay
                           && a.extensionFromArrivalID == null
                           && a.reservationStatus.Trim() != "NS"
                           && a.reservationStatus.Trim() != "CA"
                           select new
                           {
                               program.program,
                               a.arrivalDate,
                               a.hostessQualificationStatusID,
                               nqreason.nqReason,
                               a.bookingStatusID,
                               a.arrivalsTravelSourceID,
                               a.nqReasonID,
                               a.arrivalID,
                               a.guest
                           };

            foreach (var arrival in arrivals)
            {
                //dayweek row
                if (report.PenetrationPerDay.Count(x => x.Date == arrival.arrivalDate.ToString("yyyy-MM-dd")) == 0)
                {
                    ArrivalsViewModel.PenetrationPerDay ppd = new
ArrivalsViewModel.PenetrationPerDay();
                    ppd.Date = arrival.arrivalDate.ToString("yyyy-MM-dd");
                    ppd.DayOfWeek = arrival.arrivalDate.DayOfWeek.ToString();
                    report.PenetrationPerDay.Add(ppd);
                }

                var row = report.PenetrationPerDay.FirstOrDefault(x => x.Date == arrival.arrivalDate.ToString("yyyy-MM-dd"));

                switch (arrival.program)
                {
                    case "Members":
                        if (arrival.bookingStatusID == null || arrival.bookingStatusID == 10)
                        {
                            row.MembersNotContacted += 1;
                            row.MembersArrivalsTotal += 1;
                            row.GlobalSummary += 1;

                            report.Totals.MembersNotContacted += 1;
                            report.Totals.MembersArrivalsTotal += 1;
                            report.Totals.GlobalSummary += 1;
                        }
                        else
                        {
                            if (arrival.nqReason == "Double Room")
                            {
                                row.MembersDoubles += 1;
                                row.MembersArrivalsTotal += 1;
                                row.GlobalSummary += 1;

                                report.Totals.MembersDoubles += 1;
                                report.Totals.MembersArrivalsTotal += 1;
                                report.Totals.GlobalSummary += 1;
                                if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                {
                                    report.Booked.MembersDoubles += 1;
                                    report.Booked.MembersArrivalsTotal += 1;
                                    report.Booked.GlobalSummary += 1;
                                }
                            }
                            else
                            {
                                if (arrival.hostessQualificationStatusID == 1)
                                {
                                    row.MembersQ += 1;
                                    row.MembersArrivalsTotal += 1;
                                    row.GlobalSummary += 1;

                                    report.Totals.MembersQ += 1;
                                    report.Totals.MembersArrivalsTotal += 1;
                                    report.Totals.GlobalSummary += 1;
                                    if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                    {
                                        report.Booked.MembersQ += 1;
                                        report.Booked.MembersArrivalsTotal += 1;
                                        report.Booked.GlobalSummary += 1;
                                    }
                                }
                                else if (arrival.hostessQualificationStatusID == 2)
                                {
                                    row.MembersNQ += 1;
                                    row.MembersArrivalsTotal += 1;
                                    row.GlobalSummary += 1;

                                    report.Totals.MembersNQ += 1;
                                    report.Totals.MembersArrivalsTotal += 1;
                                    report.Totals.GlobalSummary += 1;
                                    if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                    {
                                        report.Booked.MembersNQ += 1;
                                        report.Booked.MembersArrivalsTotal += 1;
                                        report.Booked.GlobalSummary += 1;
                                    }
                                }
                            }
                        }

                        //nqreason

                        break;
                    case "Inhouse Line":
                        if (arrival.bookingStatusID == null || arrival.bookingStatusID == 10)
                        {
                            row.InHouseNotContacted += 1;
                            row.InHouseAirportArrivalsTotal += 1;
                            row.GlobalSummary += 1;

                            report.Totals.InHouseNotContacted += 1;
                            report.Totals.InHouseAirportArrivalsTotal += 1;
                            report.Totals.GlobalSummary += 1;
                        }
                        else
                        {
                            if (arrival.nqReason == "Double Room")
                            {
                                row.InHouseDoubles += 1;
                                row.InHouseAirportArrivalsTotal += 1;
                                row.GlobalSummary += 1;

                                report.Totals.InHouseDoubles += 1;
                                report.Totals.InHouseAirportArrivalsTotal += 1;
                                report.Totals.GlobalSummary += 1;
                                if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                {
                                    report.Booked.InHouseDoubles += 1;
                                    report.Booked.InHouseAirportArrivalsTotal += 1;
                                    report.Booked.GlobalSummary += 1;
                                }
                            }
                            else
                            {
                                if (arrival.hostessQualificationStatusID == 1)
                                {
                                    row.InHouseQ += 1;
                                    row.InHouseAirportArrivalsTotal += 1;
                                    row.GlobalSummary += 1;

                                    report.Totals.InHouseQ += 1;
                                    report.Totals.InHouseAirportArrivalsTotal += 1;
                                    report.Totals.GlobalSummary += 1;
                                    if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                    {
                                        report.Booked.InHouseQ += 1;
                                        report.Booked.InHouseAirportArrivalsTotal += 1;
                                        report.Booked.GlobalSummary += 1;
                                    }
                                }
                                else if (arrival.hostessQualificationStatusID == 2)
                                {
                                    row.InHouseNQ += 1;
                                    row.InHouseAirportArrivalsTotal += 1;
                                    row.GlobalSummary += 1;

                                    report.Totals.InHouseNQ += 1;
                                    report.Totals.InHouseAirportArrivalsTotal += 1;
                                    report.Totals.GlobalSummary += 1;
                                    if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                    {
                                        report.Booked.InHouseNQ += 1;
                                        report.Booked.InHouseAirportArrivalsTotal += 1;
                                        report.Booked.GlobalSummary += 1;
                                    }
                                }
                            }
                        }

                        //nqreason

                        break;
                    case "NetCenter":
                        if (arrival.bookingStatusID == null || arrival.bookingStatusID == 10)
                        {

                        }
                        else
                        {
                            if (arrival.nqReason == "Double Room")
                            {

                            }
                            else
                            {
                                if (arrival.hostessQualificationStatusID == 1)
                                {
                                    row.NetCenterQ += 1;
                                    row.GlobalSummary += 1;

                                    report.Totals.NetCenterQ += 1;
                                    report.Totals.GlobalSummary += 1;
                                    if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                    {
                                        report.Booked.NetCenterQ += 1;
                                    }
                                }
                                else if (arrival.hostessQualificationStatusID == 2)
                                {
                                    row.NetCenterNQ += 1;
                                    row.GlobalSummary += 1;

                                    report.Totals.NetCenterNQ += 1;
                                    report.Totals.GlobalSummary += 1;
                                    if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                    {
                                        report.Booked.NetCenterNQ += 1;
                                    }
                                }
                            }
                        }

                        break;
                    case "Referrals":
                        if (arrival.bookingStatusID == null || arrival.bookingStatusID == 10)
                        {

                        }
                        else
                        {
                            if (arrival.nqReason == "Double Room")
                            {

                            }
                            else
                            {
                                if (arrival.hostessQualificationStatusID == 1)
                                {
                                    row.ReferralQ += 1;
                                    row.GlobalSummary += 1;

                                    report.Totals.ReferralQ += 1;
                                    report.Totals.GlobalSummary += 1;
                                    if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                    {
                                        report.Booked.ReferralQ += 1;
                                    }
                                }
                                else if (arrival.hostessQualificationStatusID == 2)
                                {
                                    row.ReferralNQ += 1;
                                    row.GlobalSummary += 1;

                                    report.Totals.ReferralNQ += 1;
                                    report.Totals.GlobalSummary += 1;
                                    if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                                    {
                                        report.Booked.ReferralNQ += 1;
                                    }
                                }
                            }
                        }

                        break;
                    case "Airport":
                        if (arrival.hostessQualificationStatusID == 1)
                        {
                            row.AirportQ += 1;
                            row.InHouseAirportArrivalsTotal += 1;
                            row.GlobalSummary += 1;

                            report.Totals.AirportQ += 1;
                            report.Totals.InHouseAirportArrivalsTotal += 1;
                            report.Totals.GlobalSummary += 1;
                            if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                            {
                                report.Booked.AirportQ += 1;
                                report.Booked.InHouseAirportArrivalsTotal += 1;
                                report.Booked.GlobalSummary += 1;
                            }
                        }

                        break;
                }
                if (arrival.arrivalsTravelSourceID == 4)
                {
                    report.GuestOfMembers += 1;
                }
                else if (arrival.arrivalsTravelSourceID == 1)
                {
                    report.Exchange += 1;
                }

                if (arrival.nqReasonID != null)
                {
                    switch (arrival.program)
                    {
                        case "Members":
                            if (report.MembersNQReasons.Count(x => x.NQReasonID == arrival.nqReasonID) == 0)
                            {
                                ArrivalsViewModel.NQReasonTotals nqr = new ArrivalsViewModel.NQReasonTotals();
                                nqr.NQReasonID = (int)arrival.nqReasonID;
                                nqr.NQReason = arrival.nqReason;
                                nqr.NQReasonTotal = 0;
                                nqr.NQReasonBooked = 0;
                                report.MembersNQReasons.Add(nqr);
                            }

                            var currentNQRM = report.MembersNQReasons.FirstOrDefault(x => x.NQReasonID == arrival.nqReasonID);
                            currentNQRM.NQReasonTotal += 1;
                            report.MembersNQTotal += 1;
                            if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                            {
                                currentNQRM.NQReasonBooked += 1;
                                report.MembersNQBooked += 1;
                            }
                            break;
                        case "Inhouse Line":
                            if (report.InHouseNQReasons.Count(x => x.NQReasonID == arrival.nqReasonID) == 0)
                            {
                                ArrivalsViewModel.NQReasonTotals nqr = new ArrivalsViewModel.NQReasonTotals();
                                nqr.NQReasonID = (int)arrival.nqReasonID;
                                nqr.NQReason = arrival.nqReason;
                                nqr.NQReasonTotal = 0;
                                nqr.NQReasonBooked = 0;
                                report.InHouseNQReasons.Add(nqr);
                            }

                            var currentNQRI = report.InHouseNQReasons.FirstOrDefault(x => x.NQReasonID == arrival.nqReasonID);
                            currentNQRI.NQReasonTotal += 1;
                            report.InHouseNQTotal += 1;
                            if (arrival.bookingStatusID != null && (arrival.bookingStatusID == 7 || arrival.bookingStatusID == 11))
                            {
                                currentNQRI.NQReasonBooked += 1;
                                report.InHouseNQBooked += 1;
                            }
                            break;
                    }
                }
            }

            //penetration
            if (report.Totals.MembersQ > 0)
            {
                report.PenetrationPercentages.MembersQ = decimal.Round((decimal)report.Booked.MembersQ * 100 / (decimal)report.Totals.MembersQ, 2);
            }
            if (report.Totals.MembersNQ > 0)
            {
                report.PenetrationPercentages.MembersNQ = decimal.Round((decimal)report.Booked.MembersNQ * 100 / (decimal)report.Totals.MembersNQ, 2);
            }
            if (report.Totals.MembersDoubles > 0)
            {
                report.PenetrationPercentages.MembersDoubles = decimal.Round((decimal)report.Booked.MembersDoubles * 100 / (decimal)report.Totals.MembersDoubles, 2);
            }
            if (report.Totals.MembersArrivalsTotal > 0)
            {
                report.PenetrationPercentages.MembersArrivalsTotal = decimal.Round((decimal)report.Booked.MembersArrivalsTotal * 100 / (decimal)report.Totals.MembersArrivalsTotal, 2);
            }
            if (report.Totals.InHouseQ > 0)
            {
                report.PenetrationPercentages.InHouseQ = decimal.Round((decimal)report.Booked.InHouseQ * 100 / (decimal)report.Totals.InHouseQ, 2);
            }
            if (report.Totals.InHouseNQ > 0)
            {
                report.PenetrationPercentages.InHouseNQ = decimal.Round((decimal)report.Booked.InHouseNQ * 100 / (decimal)report.Totals.InHouseNQ, 2);
            }
            if (report.Totals.AirportQ > 0)
            {
                report.PenetrationPercentages.AirportQ = decimal.Round((decimal)report.Booked.AirportQ * 100 / (decimal)report.Totals.AirportQ, 2);
            }
            if (report.Totals.InHouseDoubles > 0)
            {
                report.PenetrationPercentages.InHouseDoubles = decimal.Round((decimal)report.Booked.InHouseDoubles * 100 / (decimal)report.Totals.InHouseDoubles, 2);
            }
            if (report.Totals.NetCenterQ > 0)
            {
                report.PenetrationPercentages.NetCenterQ = decimal.Round((decimal)report.Booked.NetCenterQ * 100 / (decimal)report.Totals.NetCenterQ, 2);
            }
            if (report.Totals.NetCenterNQ > 0)
            {
                report.PenetrationPercentages.NetCenterNQ = decimal.Round((decimal)report.Booked.NetCenterNQ * 100 / (decimal)report.Totals.NetCenterNQ, 2);
            }
            if (report.Totals.ReferralQ > 0)
            {
                report.PenetrationPercentages.ReferralQ = decimal.Round((decimal)report.Booked.ReferralQ * 100 / (decimal)report.Totals.ReferralQ, 2);
            }
            if (report.Totals.ReferralNQ > 0)
            {
                report.PenetrationPercentages.ReferralNQ = decimal.Round((decimal)report.Booked.ReferralNQ * 100 / (decimal)report.Totals.ReferralNQ, 2);
            }
            if (report.Totals.InHouseAirportArrivalsTotal > 0)
            {
                report.PenetrationPercentages.InHouseAirportArrivalsTotal = decimal.Round((decimal)report.Booked.InHouseAirportArrivalsTotal * 100 / (decimal)report.Totals.InHouseAirportArrivalsTotal, 2);
            }
            if (report.Totals.GlobalSummary > 0)
            {
                report.PenetrationPercentages.GlobalSummary = decimal.Round((decimal)report.Booked.GlobalSummary * 100 / (decimal)report.Totals.GlobalSummary, 2);
            }

            //totals
            report.TotalCheckInMembers = report.Totals.MembersArrivalsTotal;
            report.TotalCheckInInHouse = report.Totals.InHouseAirportArrivalsTotal;
            report.TotalGlobalCheckIn = report.TotalCheckInMembers + report.TotalCheckInInHouse;

            report.Members = report.Totals.MembersQ + report.Totals.MembersNQ;

            report.TotalQMembers = report.Booked.MembersQ;
            report.TotalQInHouse = report.Booked.InHouseQ + report.Booked.AirportQ;
            report.QGlobalSummary = report.TotalQMembers + report.TotalQInHouse;

            if (report.TotalCheckInMembers > 0)
            {
                report.MembersQPenetration = decimal.Round((decimal)report.TotalQMembers * 100 / (decimal)report.TotalCheckInMembers, 2);
            }
            if (report.TotalCheckInInHouse > 0)
            {
                report.InHouseQPenetration = decimal.Round((decimal)report.TotalQInHouse * 100 / (decimal)report.TotalCheckInInHouse, 2);
            }
            if (report.TotalGlobalCheckIn > 0)
            {
                report.GlobalQPenetration = decimal.Round((decimal)report.QGlobalSummary * 100 / (decimal)report.TotalGlobalCheckIn, 2);
            }

            return report;
        }

        public ArrivalsViewModel.PenetrationReport SearchPenetrationReport(ArrivalsViewModel.SearchPenetrationReport model)
        {
            ePlatEntities db = new ePlatEntities();
            ArrivalsViewModel.PenetrationReport report = new ArrivalsViewModel.PenetrationReport();

            DateTime fromDate = DateTime.Parse(model.SearchPenetration_I_ArrivalDate);
            DateTime toDate = DateTime.Parse(model.SearchPenetration_F_ArrivalDate);
            DateTime toDateNextDay = toDate.AddDays(1);
            int programsLength = 0;
            int?[] programs = new int?[] { };
            if (model.SearchPenetration_ProgramID != null)
            {
                programs = model.SearchPenetration_ProgramID.ToArray();
                programsLength = model.SearchPenetration_ProgramID.Length;
            }

            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];

            report.MonthName = Utils.GeneralFunctions.DateFormat.MonthName(fromDate.Month, "en-US");
            if (toDate.Month != fromDate.Month)
            {
                report.MonthName += " to " + Utils.GeneralFunctions.DateFormat.MonthName(toDate.Month, "en-US");
            }

            var PlaceQ = (from t in db.tblTerminals
                          join place in db.tblPlaces on t.placeID equals place.placeID
                          select place.place).FirstOrDefault();

            report.Resort = PlaceQ;
            if (toDate.Month == fromDate.Month)
            {
                if (toDate.Day == fromDate.Day)
                {
                    report.DatesRange = Utils.GeneralFunctions.DateFormat.MonthName(fromDate.Month, "en-US") + " " + fromDate.Day;
                }
                else
                {
                    report.DatesRange = fromDate.Day + " to " + toDate.Day;
                }
            }
            else
            {
                report.DatesRange = Utils.GeneralFunctions.DateFormat.MonthName(fromDate.Month, "en-US") + " " + fromDate.Day + " to " + Utils.GeneralFunctions.DateFormat.MonthName(toDate.Month, "en-US") + " " + toDate.Day;
            }
            report.PromotersPenetration = new List<ArrivalsViewModel.PromoterPenetration>();
            report.Totals = GetDimensionGroups();

            //consulta
            var arrivals = from a in db.tblArrivals
                           where a.terminalID == terminalID
                           && a.arrivalDate >= fromDate
                           && a.arrivalDate < toDateNextDay
                           && (programs.Contains(a.programID) || programsLength == 0)
                           && a.extensionFromArrivalID == null
                           select a;

            foreach (var arrival in arrivals)
            {
                if (report.PromotersPenetration.Count(x => x.OpcID == arrival.opcID) == 0)
                {
                    ArrivalsViewModel.PromoterPenetration promotor = new ArrivalsViewModel.PromoterPenetration();
                    if (arrival.tblOPCS != null)
                    {
                        promotor.OpcID = arrival.opcID;
                        promotor.Promoter = arrival.tblOPCS.firstName + " " + arrival.tblOPCS.lastName;
                    }
                    else
                    {
                        promotor.Promoter = "Unknown";
                    }
                    promotor.DimensionGroups = GetDimensionGroups();
                    report.PromotersPenetration.Add(promotor);
                }

                ArrivalsViewModel.PromoterPenetration promoter = report.PromotersPenetration.FirstOrDefault(x => x.OpcID == arrival.opcID);

                if (arrival.showStatus == null && arrival.spiCustomerID != null)
                {
                    //obtener feedback de SPI
                    SPIModels.TheVillaGroup.spCustomer_tracking_Result saleInfo = new SPIModels.TheVillaGroup.spCustomer_tracking_Result();
                    SPIModels.TheVillaGroup.SpiTvgEntities spidb = new SPIModels.TheVillaGroup.SpiTvgEntities();
                    saleInfo = spidb.spCustomer_tracking("greveles", (int)arrival.spiCustomerID).FirstOrDefault();
                    if (saleInfo != null)
                    {
                        if (saleInfo.calificacion == null)
                        {
                            arrival.showStatus = false;
                        }
                        else
                        {
                            arrival.showStatus = true;
                            arrival.salesStatus = saleInfo.Estatus;
                            arrival.salesVolume = saleInfo.volumen;
                            if (saleInfo.calificacion.IndexOf("NQ") >= 0)
                            {
                                arrival.checkInQualificationStatusID = 2;
                            }
                            else if (saleInfo.calificacion.IndexOf("Q") >= 0)
                            {
                                arrival.checkInQualificationStatusID = 1;
                            }
                        }
                    }
                }
                if (arrival.showStatus != null && arrival.showStatus == false)
                {
                    //no show
                    if (arrival.bookingStatusID != null && (arrival.tblBookingStatus.bookingStatus == "Booked" || arrival.tblBookingStatus.bookingStatus == "Rescheduled"))
                    {
                        promoter.DimensionGroups.Last().Booked += 1;
                        promoter.DimensionGroups.Last().Total += 1;
                        report.Totals.Last().Booked += 1;
                        report.Totals.Last().Total += 1;
                    }
                    else if (arrival.bookingStatusID != null && arrival.tblBookingStatus.bookingStatus == "Talked To")
                    {
                        promoter.DimensionGroups.Last().TalkedTo += 1;
                        promoter.DimensionGroups.Last().Total += 1;
                        report.Totals.Last().TalkedTo += 1;
                        report.Totals.Last().Total += 1;
                    }
                }
                else
                {
                    if (arrival.bookingStatusID != null && arrival.tblBookingStatus.bookingStatus == "Canceled")
                    {
                        //canceled
                        promoter.DimensionGroups.Last().Booked += 1;
                        promoter.DimensionGroups.Last().Total += 1;
                        report.Totals.Last().Booked += 1;
                        report.Totals.Last().Total += 1;
                    }
                    else
                    {
                        if (arrival.hostessQualificationStatusID != null)
                        {
                            //Q o NQ
                            if (arrival.bookingStatusID != null && (arrival.tblBookingStatus.bookingStatus == "Booked" || arrival.tblBookingStatus.bookingStatus == "Rescheduled"))
                            {
                                promoter.DimensionGroups.FirstOrDefault(x => x.Group == arrival.tblQualificationStatus.qualificationStatus).Booked += 1;
                                promoter.DimensionGroups.FirstOrDefault(x => x.Group == arrival.tblQualificationStatus.qualificationStatus).Total += 1;
                                promoter.DimensionGroups.FirstOrDefault(x => x.Group == "Totals").Booked += 1;
                                promoter.DimensionGroups.FirstOrDefault(x => x.Group == "Totals").Total += 1;

                                report.Totals.FirstOrDefault(x => x.Group == arrival.tblQualificationStatus.qualificationStatus).Booked += 1;
                                report.Totals.FirstOrDefault(x => x.Group == arrival.tblQualificationStatus.qualificationStatus).Total += 1;
                                report.Totals.FirstOrDefault(x => x.Group == "Totals").Booked += 1;
                                report.Totals.FirstOrDefault(x => x.Group == "Totals").Total += 1;
                            }
                            else if (arrival.bookingStatusID != null && (arrival.tblBookingStatus.bookingStatus == "Talked To" || arrival.tblBookingStatus.bookingStatus == "Refused"))
                            {
                                promoter.DimensionGroups.FirstOrDefault(x => x.Group == arrival.tblQualificationStatus.qualificationStatus).TalkedTo += 1;
                                promoter.DimensionGroups.FirstOrDefault(x => x.Group == arrival.tblQualificationStatus.qualificationStatus).Total += 1;
                                promoter.DimensionGroups.FirstOrDefault(x => x.Group == "Totals").TalkedTo += 1;
                                promoter.DimensionGroups.FirstOrDefault(x => x.Group == "Totals").Total += 1;

                                report.Totals.FirstOrDefault(x => x.Group == arrival.tblQualificationStatus.qualificationStatus).TalkedTo += 1;
                                report.Totals.FirstOrDefault(x => x.Group == arrival.tblQualificationStatus.qualificationStatus).Total += 1;
                                report.Totals.FirstOrDefault(x => x.Group == "Totals").TalkedTo += 1;
                                report.Totals.FirstOrDefault(x => x.Group == "Totals").Total += 1;
                            }
                        }
                    }
                }
            }
            db.SaveChanges();
            //cálculo de porcentajes
            foreach (var promoter in report.PromotersPenetration)
            {
                foreach (var group in promoter.DimensionGroups)
                {
                    if (group.Total > 0)
                    {
                        group.Percentage = decimal.Round(group.Booked * 100 / group.Total, 2);
                    }
                }
            }

            foreach (var group in report.Totals)
            {
                if (group.Total > 0)
                {
                    group.Percentage = decimal.Round(group.Booked * 100 / group.Total, 2);
                }
            }

            return report;
        }

        public List<ArrivalsViewModel.PenetrationDimension> GetDimensionGroups()
        {
            List<ArrivalsViewModel.PenetrationDimension> list = new List<ArrivalsViewModel.PenetrationDimension>();
            string[] groups = { "Q", "NQ", "Totals", "No Show / Canceled" };
            foreach (var group in groups)
            {
                ArrivalsViewModel.PenetrationDimension newGroup = new ArrivalsViewModel.PenetrationDimension();
                newGroup.Group = group;
                newGroup.Booked = 0;
                newGroup.TalkedTo = 0;
                newGroup.Total = 0;
                newGroup.Percentage = 0;
                list.Add(newGroup);
            }
            return list;
        }

        public ArrivalsViewModel.ArrivalsForecast SearchArrivalsForecast(ArrivalsViewModel.SearchArrivalsForecast model)
        {
            ePlatEntities db = new ePlatEntities();

            ArrivalsViewModel.ArrivalsForecast forecast = new ArrivalsViewModel.ArrivalsForecast();
            forecast.Programs = new List<ArrivalsViewModel.ProgramArrivalsInfo>();
            forecast.Arrivals = new List<ArrivalsViewModel.ArrivalFrontInfo>();
            forecast.ErrorMessages = new List<string>();

            DateTime fromDate = DateTime.Parse(model.SearchForecast_I_ArrivalDate);
            DateTime toDate = DateTime.Parse(model.SearchForecast_F_ArrivalDate);

            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];

            int frontOfficeResortID = 0;

            var TerminalPlaceID = (from t in db.tblTerminals
                                   where t.terminalID == terminalID
                                   select t.placeID).FirstOrDefault();

            //obtención del front office ResortID
            var FrontOfficeResortIDQ = (from f in db.tblPlaces
                                        where f.placeID == TerminalPlaceID
                                        select f.frontOfficeResortID).FirstOrDefault();

            if (FrontOfficeResortIDQ != null)
            {
                frontOfficeResortID = (int)FrontOfficeResortIDQ;
            }

            var companyGroupID = (from t in db.tblTerminals
                                  where t.terminalID == terminalID
                                  select t.companiesGroupID).FirstOrDefault();

            var Programs = from p in db.tblProspectationPrograms
                           where p.companiesGroupID == companyGroupID
                           select new
                           {
                               p.programID,
                               p.program
                           };

            var TravelSources = from t in db.tblArrivalsTravelSources
                                select new
                                {
                                    t.arrivalsTravelSourceID,
                                    t.arrivalsTravelSource
                                };

            var MarketCodes = from m in db.tblMarketCodes
                              where m.frontOfficeResortID == frontOfficeResortID
                              select m;

            var NacLastNames = from l in db.tblLastNames_Countries
                               where l.countryID == 3
                               select l.lastName;

            var NQReasons = from n in db.tblNqReasons
                            where n.companiesGroupID == companyGroupID
                            select n;

            List<FrontOfficeViewModel.LlegadasResult> results = new List<FrontOfficeViewModel.LlegadasResult>();
            results = FrontOfficeDataModel.GetArrivals(frontOfficeResortID, fromDate, toDate);

            foreach (var reservation in results.Where(x => x.codigostatusreservacion != "CA").OrderBy(x => x.llegada))
            {
                ArrivalsViewModel.ArrivalFrontInfo arrival = new ArrivalsViewModel.ArrivalFrontInfo();
                arrival.FrontOfficeReservationID = (long)reservation.idReservacion;
                arrival.FrontOfficeResortID = (int)reservation.idresort;
                arrival.ArrivalDate = reservation.llegada.Value.ToString("yyyy-MM-dd");
                arrival.RoomNumber = reservation.NumHab ?? "";
                arrival.Nights = (int)reservation.CuartosNoche;
                arrival.Guest = reservation.Huesped;
                arrival.FirstName = reservation.nombres;
                arrival.LastName = reservation.apellidopaterno;
                arrival.Adults = (int)reservation.Adultos;
                if (reservation.Ninos != null)
                {
                    arrival.Children = (int)reservation.Ninos;
                }
                else
                {
                    arrival.Children = 0;
                }
                if (reservation.Infantes != null)
                {
                    arrival.Infants = (int)reservation.Infantes;
                }
                else
                {
                    arrival.Infants = 0;
                }
                arrival.Country = reservation.namepais;
                if (reservation.nameagencia != null)
                {
                    arrival.AgencyName = reservation.nameagencia;
                }
                arrival.Source = reservation.Procedencia;
                arrival.ReservationStatus = reservation.codigostatusreservacion.Trim();
                arrival.RoomType = reservation.TipoHab;
                arrival.ConfirmationNumber = reservation.numconfirmacion;
                arrival.Crs = reservation.CRS;

                //nac o int
                if (NacLastNames.Contains(reservation.apellidopaterno.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")) && reservation.Procedencia != "GRUPOS & BODAS NACIONALES")
                {
                    arrival.Nationality = "NAC";
                }

                //nq 1 night and double clasification
                //1 night - 18
                if (arrival.Nights == 1)
                {
                    arrival.HostessQualificationStatusID = 2;
                    if (NQReasons.FirstOrDefault(x => x.nqReason == "1 Noche") != null)
                    {
                        arrival.NQReasonID = NQReasons.FirstOrDefault(x => x.nqReason == "1 Noche").nqReasonID;
                        arrival.NQReason = "1 Noche";
                    }
                    else if (NQReasons.FirstOrDefault(x => x.nqReason == "1 Night") != null)
                    {
                        arrival.NQReasonID = NQReasons.FirstOrDefault(x => x.nqReason == "1 Night").nqReasonID;
                        arrival.NQReason = "1 Night";
                    }
                }
                //double room - 7
                if (forecast.Arrivals.Count(x => x.LastName == reservation.apellidopaterno && (x.Nights == reservation.CuartosNoche || (DateTime.Parse(x.ArrivalDate) >= reservation.llegada && DateTime.Parse(x.ArrivalDate) < reservation.llegada.Value.AddDays(2)))) > 0 && arrival.TravelSource != "Courtesy")
                {
                    //x.Guest.Contains(reservation.apellidopaterno)
                    arrival.HostessQualificationStatusID = 2;
                    arrival.NQReasonID = NQReasons.FirstOrDefault(x => x.nqReason == "Double Room").nqReasonID; ;
                    arrival.NQReason = "Double Room";
                }

                //checkin type clasification
                var currentMarketCode = MarketCodes.FirstOrDefault(m => m.marketCode == reservation.Procedencia);
                if (currentMarketCode != null)
                {
                    arrival.ProgramID = currentMarketCode.programID;
                    arrival.Program = Programs.FirstOrDefault(x => x.programID == arrival.ProgramID).program;
                    arrival.TravelSourceID = currentMarketCode.arrivalsTravelSourceID;
                    arrival.TravelSource = TravelSources.FirstOrDefault(x => x.arrivalsTravelSourceID == arrival.TravelSourceID).arrivalsTravelSource;
                }

                forecast.Arrivals.Add(arrival);

                if (arrival.ProgramID != null)
                {
                    if (forecast.Programs.Count(x => x.ProgramID == arrival.ProgramID) == 0)
                    {
                        ArrivalsViewModel.ProgramArrivalsInfo program = new ArrivalsViewModel.ProgramArrivalsInfo();
                        program.ProgramID = (int)arrival.ProgramID;
                        program.Program = arrival.Program;
                        program.Amount = 0;
                        program.CheckInTypes = new List<ArrivalsViewModel.CheckInTypeInfo>();
                        forecast.Programs.Add(program);
                    }

                    forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).Amount += 1;

                    if (arrival.Program == "Inhouse Line" && arrival.Nationality == "NAC" && arrival.NQReason != "Double Room")
                    {
                        if (forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).CheckInTypes.Count(x => x.TravelSourceID == 9999) == 0)
                        {
                            ArrivalsViewModel.CheckInTypeInfo checkInType = new ArrivalsViewModel.CheckInTypeInfo();
                            checkInType.TravelSourceID = 9999;
                            checkInType.TravelSource = "Nationals";
                            checkInType.Amount = 0;
                            checkInType.Percentage = 0;
                            forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).CheckInTypes.Add(checkInType);
                        }

                        forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).CheckInTypes.FirstOrDefault(x => x.TravelSourceID == 9999).Amount += 1;
                    }
                    else
                    {
                        //revisar si es NQ, de ser así, no agregar a ckeckintype
                        if (arrival.HostessQualificationStatusID == 2)
                        {
                            if (forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).CheckInTypes.Count(x => x.TravelSourceID == arrival.NQReasonID + 9000) == 0)
                            {
                                ArrivalsViewModel.CheckInTypeInfo checkInType = new ArrivalsViewModel.CheckInTypeInfo();
                                checkInType.TravelSourceID = arrival.NQReasonID + 9000;
                                checkInType.TravelSource = (checkInType.TravelSourceID > 9000 ? NQReasons.FirstOrDefault(x => x.nqReasonID == arrival.NQReasonID).nqReason : "");
                                checkInType.Amount = 0;
                                checkInType.Percentage = 0;
                                forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).CheckInTypes.Add(checkInType);
                            }

                            forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).CheckInTypes.FirstOrDefault(x => x.TravelSourceID == arrival.NQReasonID + 9000).Amount += 1;
                        }
                        else
                        {
                            if (forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).CheckInTypes.Count(x => x.TravelSourceID == arrival.TravelSourceID) == 0)
                            {
                                ArrivalsViewModel.CheckInTypeInfo checkInType = new ArrivalsViewModel.CheckInTypeInfo();
                                checkInType.TravelSourceID = (int)arrival.TravelSourceID;
                                checkInType.TravelSource = arrival.TravelSource;
                                checkInType.Amount = 0;
                                checkInType.Percentage = 0;
                                forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).CheckInTypes.Add(checkInType);
                            }

                            forecast.Programs.FirstOrDefault(x => x.ProgramID == arrival.ProgramID).CheckInTypes.FirstOrDefault(x => x.TravelSourceID == arrival.TravelSourceID).Amount += 1;
                        }
                    }
                }
                else
                {
                    forecast.ErrorMessages.Add(arrival.Source + " is not registered. I don't know how to clasify it.");
                }
            }

            //get percentages
            foreach (var program in forecast.Programs)
            {
                foreach (var checkinType in program.CheckInTypes)
                {
                    checkinType.Percentage = checkinType.Amount * 100 / program.Amount;
                }
            }

            return forecast;
        }

        public List<PreArrivalFeedbackModel.FeedbackResults> SearchPrearrivalsFeedback(PreArrivalFeedbackModel.SearchFeedback model)
        {
            List<PreArrivalFeedbackModel.FeedbackResults> results = new List<PreArrivalFeedbackModel.FeedbackResults>();

            ePlatEntities db = new ePlatEntities();

            DateTime fromDate = Convert.ToDateTime(model.Search_I_FromDate);
            DateTime toDate = Convert.ToDateTime(model.Search_F_ToDate).AddDays(1);

            var ArrivalsQ = from a in db.tblArrivals
                            where a.manifestedAsPA == true
                            && a.arrivalDate >= fromDate
                            && a.arrivalDate < toDate
                            join rsv in db.tblReservations on a.leadID equals rsv.leadID
                            join lead in db.tblLeads on a.leadID equals lead.leadID
                            join resort in db.tblPlaces on rsv.placeID equals resort.placeID
                            into rsv_resort
                            from resort in rsv_resort.DefaultIfEmpty()
                            join source in db.tblLeadSources on lead.leadSourceID equals source.leadSourceID
                            into lead_source
                            from source in lead_source.DefaultIfEmpty()
                            join presentation in db.tblPresentations on rsv.reservationID equals presentation.reservationID
                            into rsv_presentation
                            from presentation in rsv_presentation.DefaultIfEmpty()
                            join pbs in db.tblBookingStatus on lead.bookingStatusID equals pbs.bookingStatusID
                            into lead_pbs
                            from pbs in lead_pbs.DefaultIfEmpty()
                            join assigned in db.tblUserProfiles on lead.assignedToUserID equals assigned.userID
                            into lead_assigned
                            from assigned in lead_assigned.DefaultIfEmpty()
                            join qualification in db.tblQualificationStatus on a.hostessQualificationStatusID equals qualification.qualificationStatusID
                            into arr_qualification
                            from qualification in arr_qualification.DefaultIfEmpty()
                            join nq in db.tblNqReasons on a.nqReasonID equals nq.nqReasonID
                            into arr_nq
                            from nq in arr_nq.DefaultIfEmpty()
                            join team in db.tblPromotionTeams on a.promotionTeamID equals team.promotionTeamID
                            into arr_team
                            from team in arr_team.DefaultIfEmpty()
                            join promotor in db.tblOPCS on a.opcID equals promotor.opcID
                            into arr_promotor
                            from promotor in arr_promotor.DefaultIfEmpty()
                            join bs in db.tblBookingStatus on a.bookingStatusID equals bs.bookingStatusID
                            into arr_bs
                            from bs in arr_bs.DefaultIfEmpty()
                            join party in db.tblSalesRoomsParties on a.salesRoomPartyID equals party.salesRoomPartyID
                            into arr_party
                            from party in arr_party.DefaultIfEmpty()
                            orderby a.arrivalDate
                            select new
                            {
                                a.arrivalDate,
                                a.reservationStatus,
                                resort.place,
                                source.leadSource,
                                a.firstName,
                                a.lastName,
                                pbs.bookingStatus,
                                presentation.datePresentation,
                                presentation.timePresentation,
                                assignedTo = assigned.firstName + " " + assigned.lastName,
                                qualification.qualificationStatus,
                                nq.nqReason,
                                team.promotionTeam,
                                promotor.opc,
                                onsiteBookingStatus = bs.bookingStatus,
                                party,
                                rsv.conciergeComments
                            };

            foreach (var prearrival in ArrivalsQ)
            {
                PreArrivalFeedbackModel.FeedbackResults feedback = new PreArrivalFeedbackModel.FeedbackResults();
                feedback.ArrivalDate = prearrival.arrivalDate.ToString("yyyy-MM-dd");
                feedback.Resort = prearrival.place;
                feedback.LeadSource = prearrival.leadSource;
                feedback.FirstName = prearrival.firstName;
                feedback.LastName = prearrival.lastName;
                feedback.PrearrivalBookingStatus = prearrival.bookingStatus;
                if (prearrival.datePresentation != null)
                {
                    feedback.PresentationDate = prearrival.datePresentation.Value.ToString("yyyy-MM-dd") + " " + prearrival.timePresentation;
                }
                feedback.AssignedTo = prearrival.assignedTo;
                feedback.ReservationStatus = prearrival.reservationStatus;
                feedback.OnsiteQualification = prearrival.qualificationStatus;
                feedback.NQReason = prearrival.nqReason;
                feedback.Team = prearrival.promotionTeam;
                feedback.Promotor = prearrival.opc;
                feedback.OnsiteBookingStatus = prearrival.onsiteBookingStatus;
                if (prearrival.party != null)
                {
                    feedback.OnsitePresentationDate = prearrival.party.partyDateTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                }
                feedback.PrearrivalFeedback = prearrival.conciergeComments;
                results.Add(feedback);
            }

            return results;
        }
    }
}
