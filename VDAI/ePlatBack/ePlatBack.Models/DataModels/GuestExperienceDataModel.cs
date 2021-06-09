using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePlatBack.Models.ViewModels;

namespace ePlatBack.Models.DataModels
{
    public class GuestExperienceDataModel
    {
        public static GuestExperienceViewModel.ArrivalsResponse GetArrivals(GuestExperienceViewModel.SearchFilters model)
        {
            GuestExperienceViewModel.ArrivalsResponse response = new GuestExperienceViewModel.ArrivalsResponse();
            response.Arrivals = new List<GuestExperienceViewModel.ArrivalItem>();
            response.Overview = new GuestExperienceViewModel.Overview()
            {
                Preferences = new List<GuestExperienceViewModel.PreferenceCounter>()
            };
            List<int?> AvailableResorts = new List<int?>();
            AvailableResorts.Add(9);
            AvailableResorts.Add(11);
            AvailableResorts.Add(13);

            ePlatEntities db = new ePlatEntities();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today;

            if (model.Search_FromDate != null)
            {
                fromDate = DateTime.Parse(model.Search_FromDate);
                if (model.Search_ToDate == null)
                {
                    toDate = DateTime.Parse(model.Search_FromDate);
                }
                else
                {
                    toDate = DateTime.Parse(model.Search_ToDate);
                }
            }
            DateTime endDate = toDate.AddDays(1);

            UserSession session = new UserSession();
            //var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            //long terminalid = terminals[0];
            long terminalid = 42;

            List<tblArrivals> Arrivals = new List<tblArrivals>();
            if (model.Search_FromDate != null
                && model.Search_Preferences == null
                && model.Search_GuestName == null
                && model.Search_ConfirmationNumber == null
                && model.Search_Crs == null)
            {
                //obtener rango de reservaciones del front
                List<FrontOfficeViewModel.LlegadasResult> FOArrivals = FrontOfficeDataModel.GetArrivals(model.Search_FrontOfficeResortID, fromDate, toDate);

                //obtener rango de reservaciones de tblArrivals
                Arrivals = (from a in db.tblArrivals
                            where a.frontOfficeResortID == model.Search_FrontOfficeResortID
                            && (a.arrivalDate >= fromDate)
                            && (a.arrivalDate < endDate)
                            select a).ToList();

                foreach (var reservation in FOArrivals.Where(x=>x.idhuesped != null))
                {
                    tblArrivals arrival = (from x in Arrivals
                                           where x.frontOfficeReservationID == reservation.idReservacion
                                           select x).FirstOrDefault();

                    if (arrival == null)
                    {
                        //si no existe guardar
                        arrival = new tblArrivals();
                        arrival.arrivalID = Guid.NewGuid();
                        arrival.dateSaved = DateTime.Now;
                        arrival.terminalID = terminalid;
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

                        arrival.contract = reservation.Contrato != null && reservation.Contrato.Trim() != "" && !reservation.Contrato.Contains("PENDIENTE")
                            ? reservation.Contrato.Trim()
                            : null;

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
                        arrival.guestHubID = GuestDataModel.GetHubID(arrival.frontOfficeResortID, (long)arrival.frontOfficeGuestID, arrival.firstName, arrival.lastName, null, arrival.countryCode, "ePlat CXM", ref db);

                        db.tblArrivals.AddObject(arrival);
                        db.SaveChanges();
                    }
                }
            }

            int preferencesLength = 0;
            int[] preferences = new int[] { };
            if (model.Search_Preferences != null)
            {
                preferences = model.Search_Preferences.ToArray();
                preferencesLength = model.Search_Preferences.Length;
            }

            //iterar sobre los arrivals a regresar para obtener perfiles y contadores
            Arrivals = (from a in db.tblArrivals
                        join g in db.tblGuestsHub
                        on a.guestHubID equals g.guestHubID
                        into a_g
                        from g in a_g.DefaultIfEmpty()
                        where a.frontOfficeResortID == model.Search_FrontOfficeResortID
                       && ((a.arrivalDate >= fromDate
                       && a.arrivalDate < endDate
                       && (!preferences.Except(g.tblGuestPreferences.Select(x => x.preferenceID)).Any()
                       || preferencesLength == 0))
                       || (model.Search_FromDate == null
                       && (model.Search_GuestName != null
                       || model.Search_ConfirmationNumber != null
                       || model.Search_Crs != null)))
                       && (a.guest.Contains(model.Search_GuestName.Trim()) || model.Search_GuestName == null)
                       && (a.confirmationNumber == model.Search_ConfirmationNumber.Trim() || model.Search_ConfirmationNumber == null)
                       && (a.crs == model.Search_Crs.Trim() || model.Search_Crs == null)
                        select a).ToList();

            var ArrivalsGuestHubIDs = Arrivals.Select(x => x.guestHubID).ToList();

            var Guests = (from g in db.tblGuestsHub
                          where ArrivalsGuestHubIDs.Contains(g.guestHubID)
                          select g).ToList();

            var Places = (from p in db.tblPlaces
                          where AvailableResorts.Contains(p.frontOfficeResortID)
                          select new
                          {
                              p.frontOfficeResortID,
                              p.placeID,
                              p.place,
                              p.tblDestinations.destination
                          }).ToList();

            List<long> AvailablePlaceIDs = Places.Select(x => x.placeID).ToList();

            var SurveyIDs = (from s in db.tblSurveys
                             where AvailablePlaceIDs.Contains((long)s.placeID)
                             select s.surveyID).ToList();

            //prearrivals
            List<ArrivalsViewModel.PrearrivalInfo> Prearrivals = new List<ArrivalsViewModel.PrearrivalInfo>();
            if (model.Search_GuestName != null || model.Search_ConfirmationNumber != null || model.Search_Crs != null)
            {
                var LeadIDs = Arrivals.Select(x => x.leadID).ToList();
                Prearrivals = (from r in db.tblReservations
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
                               join user in db.tblUserProfiles on lead.assignedToUserID equals user.userID
                               into lead_user
                               from user in lead_user.DefaultIfEmpty()
                               where (lead.initialTerminalID == 10 || lead.initialTerminalID == 16)
                               && place.frontOfficeResortID == model.Search_FrontOfficeResortID
                               && LeadIDs.Contains(r.leadID)
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
                                   ZipCode = lead.zipcode,
                                   TerminalID = lead.initialTerminalID,
                                   AssignedToFirstName = user.firstName,
                                   AssignedToLastName = user.lastName,
                                   FrontOfficeReservationID = r.frontOfficeReservationID
                               }).ToList();
            }
            else
            {
                Prearrivals = (from r in db.tblReservations
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
                               join user in db.tblUserProfiles on lead.assignedToUserID equals user.userID
                               into lead_user
                               from user in lead_user.DefaultIfEmpty()
                               where r.arrivalDate >= fromDate
                               && r.arrivalDate < endDate
                               && (lead.initialTerminalID == 10 || lead.initialTerminalID == 16)
                               && place.frontOfficeResortID == model.Search_FrontOfficeResortID
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
                                   ZipCode = lead.zipcode,
                                   TerminalID = lead.initialTerminalID,
                                   AssignedToFirstName = user.firstName,
                                   AssignedToLastName = user.lastName,
                                   FrontOfficeReservationID = r.frontOfficeReservationID
                               }).ToList();
            }

            foreach (var arrival in Arrivals)
            {
                //obtener datos de savedArrival
                GuestExperienceViewModel.ArrivalItem newArrival = new GuestExperienceViewModel.ArrivalItem();

                newArrival.FrontOfficeResortID = arrival.frontOfficeResortID;
                newArrival.FrontOfficeReservationID = arrival.frontOfficeReservationID;
                newArrival.ConfirmationNumber = arrival.confirmationNumber;
                newArrival.CRS = arrival.crs;
                newArrival.CheckIn = arrival.arrivalDate;
                newArrival.CheckOut = arrival.arrivalDate.AddDays(arrival.nights);
                newArrival.Nights = arrival.nights;
                newArrival.Adults = arrival.adults;
                newArrival.Children = arrival.children;
                newArrival.Resort = Places.FirstOrDefault(x => x.frontOfficeResortID == arrival.frontOfficeResortID).place;
                newArrival.RoomType = arrival.roomType;
                newArrival.Agency = arrival.agencyName;

                newArrival.ArrivalID = arrival.arrivalID;
                newArrival.GuestHubID = arrival.guestHubID;
                newArrival.FirstName = arrival.firstName;
                newArrival.LastName = arrival.lastName;
                newArrival.RoomNumber = arrival.roomNumber;
                newArrival.Market = arrival.source;
                newArrival.Rate = arrival.rate;
                newArrival.Notes = arrival.arrivalComments;

                //revisar si es necesario obtener perfil
                bool updateFlags = false;
                if (arrival.dateLastProfileCheck == null || (arrival.dateLastProfileCheck < DateTime.Today.AddDays(-2) && arrival.arrivalDate > DateTime.Today))
                {
                    //generar perfil y guardarlo
                    GuestExperienceViewModel.Profile GuestProfile = new GuestExperienceViewModel.Profile();

                    //obtención de Guest 
                    var Guest = Guests.FirstOrDefault(x => x.guestHubID == arrival.guestHubID);

                    //Customer Info
                    GuestExperienceViewModel.CustomerInfo CustomerInfo = new GuestExperienceViewModel.CustomerInfo();

                    if (Guest != null)
                    {
                        CustomerInfo.FirstName = Guest.firstName;
                        CustomerInfo.LastName = Guest.lastName;
                        if (Guest.countryID != null)
                        {
                            CustomerInfo.Country = Guest.tblCountries.country;
                        }
                        if (Guest.stateID != null)
                        {
                            CustomerInfo.State = Guest.tblStates.state;
                        }
                        CustomerInfo.Phone = Guest.phone;
                        CustomerInfo.Email = Guest.email1;
                        if (Guest.email1 != null)
                        {
                            CustomerInfo.Picture = PictureDataModel.GetPicasaAvatar(Guest.email1.Trim());
                            CustomerInfo.Picture = CustomerInfo.Picture.Replace("s64-c", "s256-c");
                        }
                        if (CustomerInfo.Picture == null || CustomerInfo.Picture == "")
                        {
                            CustomerInfo.Picture = "/images/avatar.png";
                        }
                        CustomerInfo.Stays = 1;
                        CustomerInfo.Nights = arrival.nights;
                    }

                    GuestProfile.CustomerInfo = CustomerInfo;

                    //Member Info
                    GuestExperienceViewModel.MemberInfo MemberInfo = new GuestExperienceViewModel.MemberInfo();
                    MemberInfo.Contract = arrival.contract;

                    GuestProfile.MemberInfo = MemberInfo;

                    //Clarabridge Surveys
                    GuestProfile.ClarabridgeSurveys = new List<GuestExperienceViewModel.ClarabridgeSurvey>();

                    //Reservations and Surveys
                    GuestProfile.Reservations = new List<GuestExperienceViewModel.PastReservation>();
                    if (Guest.email1 != null)
                    {
                        List<FrontOfficeViewModel.HistorialReservacionesResult> history = new List<FrontOfficeViewModel.HistorialReservacionesResult>();
                        foreach (var resortid in AvailableResorts)
                        {
                            if (resortid == arrival.frontOfficeResortID)
                            {
                                //buscar historial en el mismo hotel
                                history = FrontOfficeDataModel.GetReservationsHistory((int)resortid, (int)arrival.frontOfficeGuestID, null);
                            }
                            else
                            {
                                //buscar historial en otros hoteles
                                history = FrontOfficeDataModel.GetReservationsHistory((int)resortid, null, Guest.email1);
                            }
                            foreach (var pastReservation in history.Where(x => x.llegada < arrival.arrivalDate))
                            {
                                GuestExperienceViewModel.PastReservation reservation = new GuestExperienceViewModel.PastReservation();
                                reservation.FrontOfficeResortID = (int)resortid;
                                reservation.FrontOfficeReservationID = pastReservation.idreservacion;
                                reservation.ConfirmationNumber = pastReservation.numconfirmacion;
                                reservation.CRS = pastReservation.numdereservacioncrs;
                                reservation.CheckIn = pastReservation.llegada;
                                reservation.CheckOut = pastReservation.salida;
                                reservation.Nights = ((DateTime)reservation.CheckOut - (DateTime)reservation.CheckIn).TotalDays;
                                reservation.Adults = pastReservation.numadultos;
                                reservation.Children = pastReservation.numchilds;
                                reservation.RoomType = pastReservation.nametipodehabitacion;
                                reservation.Agency = pastReservation.nameagencia;
                                switch (resortid)
                                {
                                    case 9:
                                        reservation.Destination = "Puerto Vallarta";
                                        reservation.Resort = "Garza Blanca Resort";
                                        break;
                                    case 11:
                                        reservation.Destination = "Cancun";
                                        reservation.Resort = "Villa del Palmar";
                                        break;
                                    case 13:
                                        reservation.Destination = "Puerto Vallarta";
                                        reservation.Resort = "Hotel Mousai";
                                        break;
                                }
                                reservation.PlanType = pastReservation.TipoPlan;

                                reservation.Rent = pastReservation.Renta ?? 0;
                                reservation.Package = pastReservation.Paquete ?? 0;
                                reservation.Spa = pastReservation.SPA ?? 0;
                                reservation.Pos = pastReservation.ConsumoPOS ?? 0;
                                reservation.Others = pastReservation.Otros ?? 0;
                                reservation.Total = reservation.Rent + reservation.Package + reservation.Spa + reservation.Pos + reservation.Others;
                                reservation.Tickets = new List<GuestExperienceViewModel.Ticket>();

                                GuestProfile.CustomerInfo.TotalSpend += reservation.Total;
                                GuestProfile.CustomerInfo.Rent += pastReservation.Renta ?? 0;
                                GuestProfile.CustomerInfo.Package += pastReservation.Paquete ?? 0;
                                GuestProfile.CustomerInfo.Spa += pastReservation.SPA ?? 0;
                                GuestProfile.CustomerInfo.Pos += pastReservation.ConsumoPOS ?? 0;
                                GuestProfile.CustomerInfo.Others += pastReservation.Otros ?? 0;

                                //buscar Clarabridge
                                var surveyInfo = (from s in db.tblAppliedSurveys
                                                  where SurveyIDs.Contains((int)s.surveyID)
                                                  && s.dateIn == pastReservation.llegada
                                                  && s.email == Guest.email1
                                                  select s).FirstOrDefault();

                                if (surveyInfo != null)
                                {
                                    GuestExperienceViewModel.ClarabridgeSurvey newSurvey = new GuestExperienceViewModel.ClarabridgeSurvey();
                                    newSurvey.ClarabridgeSurveyID = surveyInfo.surveyAppliedID;
                                    newSurvey.SubmittedDate = surveyInfo.submittedDate;
                                    newSurvey.Resort = reservation.Resort;
                                    newSurvey.HadComments = surveyInfo.hadComment;
                                    newSurvey.HadProblems = surveyInfo.hadProblem;
                                    newSurvey.Rate = decimal.Round((decimal)surveyInfo.rate, 2);

                                    reservation.ClarabridgeSurveyID = newSurvey.ClarabridgeSurveyID;
                                    GuestProfile.ClarabridgeSurveys.Add(newSurvey);
                                }

                                GuestProfile.CustomerInfo.Stays++;
                                GuestProfile.CustomerInfo.Nights += ((DateTime)reservation.CheckOut - (DateTime)reservation.CheckIn).TotalDays;

                                //PENDING - buscar tickets en Alice

                                GuestProfile.Reservations.Add(reservation);
                            }
                        }
                    }

                    //PrearrivalInfo
                    GuestExperienceViewModel.PrearrivalInfo PrearrivalInfo = new GuestExperienceViewModel.PrearrivalInfo();
                    ArrivalsViewModel.PrearrivalInfo prearrivalInfo = new ArrivalsViewModel.PrearrivalInfo();
                    if (arrival.leadID != null)
                    {
                        //buscar por leadid
                        prearrivalInfo = Prearrivals.FirstOrDefault(x => x.LeadID == arrival.leadID);
                    }
                    else
                    {
                        //buscar por numero de confirmacion
                        prearrivalInfo = Prearrivals.FirstOrDefault(x => x.HotelConfirmationNumber != null && x.HotelConfirmationNumber.Trim() == arrival.crs.Trim());
                        if (prearrivalInfo == null)
                        {
                            prearrivalInfo = Prearrivals.FirstOrDefault(x => x.FrontOfficeReservationID != null && x.FrontOfficeReservationID == arrival.frontOfficeReservationID);
                        }
                    }
                    if (prearrivalInfo != null)
                    {
                        arrival.leadID = prearrivalInfo.LeadID;
                        if (prearrivalInfo.BookingStatusID == 1 || prearrivalInfo.BookingStatusID == 16)
                        {
                            arrival.manifestedAsPA = true;
                        }
                        PrearrivalInfo.PrearrivalTerminalID = prearrivalInfo.TerminalID;
                        PrearrivalInfo.LeadID = prearrivalInfo.LeadID;
                        arrival.prearrivalTerminalID = prearrivalInfo.TerminalID;
                        PrearrivalInfo.BookingStatus = prearrivalInfo.BookingStatus;
                        PrearrivalInfo.AssignedTo = prearrivalInfo.AssignedToFirstName + " " + prearrivalInfo.AssignedToLastName;
                        PrearrivalInfo.OptionsTotal = prearrivalInfo.PreArrivalOptionsTotal;
                        if (PrearrivalInfo.OptionsTotal == null)
                        {
                            PrearrivalInfo.OptionsTotal = 0;
                        }
                        if (prearrivalInfo.TerminalID == 10)
                        {
                            PrearrivalInfo.PrearrivalDepartment = "Prearrival ResortCom";
                        }
                        else if (prearrivalInfo.TerminalID == 16)
                        {
                            PrearrivalInfo.PrearrivalDepartment = "Prearrival Tafer";
                            //buscar options sold
                            var OptionsSold = from o in db.tblOptionsSold
                                              join option in db.tblOptions on o.optionID equals option.optionID
                                              into o_option
                                              from option in o_option.DefaultIfEmpty()
                                              where o.reservationID == prearrivalInfo.ReservationID
                                              select new
                                              {
                                                  o.quantity,
                                                  option.optionName,
                                                  o.totalPaid
                                              };
                            foreach (var option in OptionsSold)
                            {
                                GuestExperienceViewModel.AdditionalItem additional = new GuestExperienceViewModel.AdditionalItem();
                                additional.Additional = option.optionName;
                                additional.Amount = option.quantity;
                                additional.Total = new Money()
                                {
                                    Amount = decimal.Parse(option.totalPaid),
                                    Currency = "USD"
                                };
                                GuestProfile.CustomerInfo.TotalSpend += decimal.Parse(option.totalPaid);
                                if(PrearrivalInfo.Additionals == null)
                                {
                                    PrearrivalInfo.Additionals = new List<GuestExperienceViewModel.AdditionalItem>();
                                }
                                PrearrivalInfo.Additionals.Add(additional);
                            }
                        }
                    }
                    GuestProfile.PrearrivalInfo = PrearrivalInfo;

                    //Preferences
                    GuestProfile.Preferences = new List<GuestViewModel.Preferences.PreferenceTypeGuest>();
                    GuestViewModel.PreferencesResponse preferencesResponse = GuestDataModel.GetPreferences(newArrival.GuestHubID);
                    GuestProfile.Preferences = preferencesResponse.Preferences;

                    //CurrentTickets
                    GuestProfile.CurrentTickets = new List<GuestExperienceViewModel.Ticket>();

                    //Interactions -- Se carga desde el cliente

                    newArrival.GuestProfile = GuestProfile;
                    arrival.jsonGuestProfile = js.Serialize(GuestProfile);
                    arrival.dateLastProfileCheck = DateTime.Now;
                    updateFlags = true;
                }
                else
                {
                    newArrival.GuestProfile = js.Deserialize<GuestExperienceViewModel.Profile>(arrival.jsonGuestProfile);
                    updateFlags = false;
                }

                //Global.Preferences counter
                foreach (var preferenceType in newArrival.GuestProfile.Preferences)
                {
                    foreach (GuestViewModel.Preferences.PreferenceItemValue preference in preferenceType.Preferences)
                    {
                        if (response.Overview.Preferences.Count(x => x.PreferenceID == preference.PreferenceID) == 0)
                        {
                            GuestExperienceViewModel.PreferenceCounter prefCounter = new GuestExperienceViewModel.PreferenceCounter();
                            prefCounter.PreferenceID = preference.PreferenceID;
                            prefCounter.PreferenceTypeID = preferenceType.PreferenceTypeID;
                            prefCounter.Preference = preference.Preference;
                            prefCounter.Amount = 0;
                            response.Overview.Preferences.Add(prefCounter);
                        }

                        response.Overview.Preferences.FirstOrDefault(x => x.PreferenceID == preference.PreferenceID).Amount++;
                    }
                }

                //CONTADORES LOCALES
                //Local.Recurring
                if (newArrival.GuestProfile.Reservations.Count() > 0)
                {
                    newArrival.Recurring = true;
                    //Global.RecurringClients
                    response.Overview.RecurringClients++;
                }
                else
                {
                    newArrival.Recurring = false;
                }
                //Local.Member
                if (arrival.contract != null && arrival.contract != "")
                {
                    newArrival.Member = true;
                    //Global.Members
                    response.Overview.Members++;
                }
                else
                {
                    newArrival.Member = false;
                }
                //Local.Survey
                if (newArrival.GuestProfile.ClarabridgeSurveys.Count() > 0)
                {
                    newArrival.Survey = true;
                    //Local.SurveyRate
                    newArrival.SurveyRate = decimal.Round(newArrival.GuestProfile.ClarabridgeSurveys.Sum(x => x.Rate) / newArrival.GuestProfile.ClarabridgeSurveys.Count(), 2);
                    //Global.WithSurveys
                    response.Overview.WithSurveys++;
                    //Global.HadProblems
                    if (newArrival.GuestProfile.ClarabridgeSurveys.Count(x => x.HadProblems == true) > 0)
                    {
                        newArrival.SurveyProblem = true;
                        response.Overview.HadProblems++;
                    }
                }
                else
                {
                    newArrival.Survey = false;
                }
                //Local.Prearrival
                if (newArrival.GuestProfile.PrearrivalInfo.LeadID != null)
                {
                    newArrival.Prearrival = true;
                    //Global.PrearrivalResortCom 
                    //Global.PreArrivalTafer
                    if (newArrival.GuestProfile.PrearrivalInfo.PrearrivalTerminalID == 10)
                    {
                        response.Overview.PrearrivalResortCom++;
                    }
                    else if (newArrival.GuestProfile.PrearrivalInfo.PrearrivalTerminalID == 16)
                    {
                        response.Overview.PrearrivalTafer++;
                    }
                }
                else
                {
                    newArrival.Prearrival = false;
                }
                //Local.Preferences
                if (newArrival.GuestProfile.Preferences.Count() > 0)
                {
                    newArrival.Preferences = true;
                    //Global.WithPreferences
                    response.Overview.WithPreferences++;
                }
                else
                {
                    newArrival.Preferences = false;
                }
                //Local.Complaints
                int nComplaints = newArrival.GuestProfile.Reservations.Sum(x => x.Tickets.Count(t => t.Type == "Complaint"));
                if (nComplaints > 0)
                {
                    newArrival.Complaints = true;
                    //Global.WithComplaints
                    response.Overview.WithComplaints++;
                }
                //Local.CurrentTickets
                //PENDIENTE CONECTAR CON ALICE
                //Global.WithCurrentTicketsñ
                if (updateFlags)
                {
                    arrival.recurring = newArrival.Recurring;
                    arrival.member = newArrival.Member;
                    arrival.survey = newArrival.Survey;
                    arrival.surveyRate = newArrival.SurveyRate;
                    arrival.prearrival = newArrival.Prearrival;
                    arrival.prearrivalTerminalID = newArrival.GuestProfile.PrearrivalInfo.PrearrivalTerminalID;
                    arrival.preferences = newArrival.Preferences;
                    arrival.currentTickets = newArrival.CurrentTickets;
                    arrival.complaints = newArrival.Complaints;
                }

                response.Arrivals.Add(newArrival);
            }
            db.SaveChanges();

            return response;
        }
    }
}
