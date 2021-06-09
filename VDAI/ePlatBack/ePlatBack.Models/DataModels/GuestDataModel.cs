using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web.UI;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;


namespace ePlatBack.Models.DataModels
{
    public class GuestDataModel
    {
        public static List<GuestViewModel.Preferences.PreferenceType> GetPreferencesResortList(int resortId, string culture)
        {
            ePlatEntities db = new ePlatEntities();
            List<GuestViewModel.Preferences.PreferenceType> list = new List<GuestViewModel.Preferences.PreferenceType>();

            if (culture == null)
            {
                culture = "en-US";
            }

            //obtener placeid
            var placeID = db.tblPlaces.Where(x => x.frontOfficeResortID == resortId).FirstOrDefault().placeID;

            //obtener tipos de preferencias para el resort en el lenguaje y orden solicitado
            var PreferenceTypes = from p in db.tblPreferenceTypes_Places
                                  join c in db.tblPreferenceTypesDescriptions on p.preferenceTypeID equals c.preferenceTypeID
                                  into p_c
                                  from c in p_c.DefaultIfEmpty()
                                  join t in db.tblPreferenceTypes on p.preferenceTypeID equals t.preferenceTypeID
                                  into p_t
                                  from t in p_t.DefaultIfEmpty()
                                  where p.placeID == placeID
                                  && p.fromDate < DateTime.Now && (p.toDate == null || p.toDate > DateTime.Now)
                                  && c.culture == culture
                                  orderby p.preferenceOrder
                                  select new
                                  {
                                      p.preferenceTypeID,
                                      c.preferenceType,
                                      t.name,
                                      c.description,
                                      c.observations
                                  };

            //obtener preferencias de los tipos ya obtenidos
            var Preferences = from p in db.tblPreferences_Places
                              join c in db.tblPreferencesDescriptions on p.preferenceID equals c.preferenceID
                              into p_c
                              from c in p_c.DefaultIfEmpty()
                              join r in db.tblPreferences on p.preferenceID equals r.preferenceID
                              into p_r
                              from r in p_r.DefaultIfEmpty()
                              where p.placeID == placeID
                              && p.fromDate < DateTime.Now && (p.toDate == null || p.toDate > DateTime.Now)
                              && c.culture == culture
                              orderby r.textbox, c.preference
                              select new
                              {
                                  p.preferenceID,
                                  r.preferenceTypeID,
                                  c.preference,
                                  c.description,
                                  r.checkbox,
                                  r.textbox,
                                  r.name
                              };


            foreach (var preferenceType in PreferenceTypes)
            {
                GuestViewModel.Preferences.PreferenceType type = new GuestViewModel.Preferences.PreferenceType();
                type.PreferenceTypeID = preferenceType.preferenceTypeID;
                type.PreferenceTypeName = preferenceType.preferenceType;
                type.PreferenceTypeShortName = preferenceType.name;
                type.Description = preferenceType.description;
                type.Observations = preferenceType.observations;
                type.Preferences = new List<GuestViewModel.Preferences.PreferenceItem>();

                foreach (var preference in Preferences.Where(x => x.preferenceTypeID == preferenceType.preferenceTypeID).OrderBy(x => x.textbox).ThenBy(x => x.preference))
                {
                    GuestViewModel.Preferences.PreferenceItem newPreference = new GuestViewModel.Preferences.PreferenceItem();
                    newPreference.PreferenceID = preference.preferenceID;
                    newPreference.Preference = preference.preference;
                    newPreference.PreferenceShortName = preference.name;
                    newPreference.Description = preference.description;
                    newPreference.ShowCheckbox = preference.checkbox;
                    newPreference.ShowTextbox = preference.textbox;
                    type.Preferences.Add(newPreference);
                }

                list.Add(type);
            }

            return list;
        }

        public static List<GuestViewModel.Preferences.PreferenceType> GetPreferencesList()
        {
            ePlatEntities db = new ePlatEntities();
            List<GuestViewModel.Preferences.PreferenceType> list = new List<GuestViewModel.Preferences.PreferenceType>();

            var PreferenceTypes = from p in db.tblPreferenceTypes
                                  where p.preferenceTypeID != 8
                                  orderby p.name
                                  select p;

            foreach (var preferenceType in PreferenceTypes)
            {
                GuestViewModel.Preferences.PreferenceType type = new GuestViewModel.Preferences.PreferenceType();
                type.PreferenceTypeID = preferenceType.preferenceTypeID;
                type.PreferenceTypeName = preferenceType.name;
                type.Preferences = new List<GuestViewModel.Preferences.PreferenceItem>();

                foreach (var preference in preferenceType.tblPreferences.OrderBy(x => x.name))
                {
                    GuestViewModel.Preferences.PreferenceItem newPreference = new GuestViewModel.Preferences.PreferenceItem();
                    newPreference.PreferenceID = preference.preferenceID;
                    newPreference.Preference = preference.name;
                    type.Preferences.Add(newPreference);
                }

                list.Add(type);
            }

            return list;
        }

        public static GuestViewModel.PreferencesResponse GetPreferences(Guid? guestID = null, long? frontOfficeGuestID = null, int? frontOfficeResortID = null)
        {
            ePlatEntities db = new ePlatEntities();
            GuestViewModel.PreferencesResponse preferences = new GuestViewModel.PreferencesResponse();

            if (guestID == null)
            {
                guestID = GetGuestHubID((long)frontOfficeGuestID, (int)frontOfficeResortID);
            }

            var GuestQ = (from guest in db.tblGuestsHub
                          where guest.guestHubID == guestID
                          select new
                          {
                              guest.guestHubID,
                              guest.firstName,
                              guest.lastName,
                              guest.tblGuestPreferences
                          }).FirstOrDefault();

            if (GuestQ != null)
            {
                preferences.GuestHubID = GuestQ.guestHubID;
                preferences.FirstName = GuestQ.firstName;
                preferences.LastName = GuestQ.lastName;
                //preferencias
                List<int> PreferencesList = new List<int>();
                //obtener la fecha más reciente 
                if (GuestQ.tblGuestPreferences.Count() > 0)
                {
                    DateTime? mostRecent = GuestQ.tblGuestPreferences.OrderByDescending(x => x.dateSaved).FirstOrDefault().dateSaved;
                    //preferencias por tipo
                    preferences.Preferences = new List<GuestViewModel.Preferences.PreferenceTypeGuest>();
                    //preferencias sin texto
                    foreach (var pref in GuestQ.tblGuestPreferences.Where(x => x.preferenceText == null && x.dateSaved == mostRecent))
                    {
                        if (preferences.Preferences.Count(x => x.PreferenceTypeID == pref.tblPreferences.preferenceTypeID) == 0)
                        {
                            GuestViewModel.Preferences.PreferenceTypeGuest newPT = new GuestViewModel.Preferences.PreferenceTypeGuest();
                            newPT.PreferenceTypeID = pref.tblPreferences.preferenceTypeID;
                            newPT.PreferenceTypeName = pref.tblPreferences.tblPreferenceTypes.name;
                            newPT.Preferences = new List<GuestViewModel.Preferences.PreferenceItemValue>();
                            preferences.Preferences.Add(newPT);
                        }
                        preferences.Preferences.FirstOrDefault(x => x.PreferenceTypeID == pref.tblPreferences.preferenceTypeID).Preferences.Add(new GuestViewModel.Preferences.PreferenceItemValue()
                        {
                            PreferenceID = pref.preferenceID,
                            Preference = pref.tblPreferences.name
                        });
                        PreferencesList.Add(pref.preferenceID);
                    }
                    //preferencias con texto
                    foreach (var pref in GuestQ.tblGuestPreferences.Where(x => x.preferenceText != null && x.dateSaved == mostRecent))
                    {
                        if (preferences.Preferences.Count(x => x.PreferenceTypeID == pref.tblPreferences.preferenceTypeID) == 0)
                        {
                            GuestViewModel.Preferences.PreferenceTypeGuest newPT = new GuestViewModel.Preferences.PreferenceTypeGuest();
                            newPT.PreferenceTypeID = pref.tblPreferences.preferenceTypeID;
                            newPT.PreferenceTypeName = pref.tblPreferences.tblPreferenceTypes.name;
                            newPT.Preferences = new List<GuestViewModel.Preferences.PreferenceItemValue>();
                            preferences.Preferences.Add(newPT);
                        }
                        preferences.Preferences.FirstOrDefault(x => x.PreferenceTypeID == pref.tblPreferences.preferenceTypeID).Preferences.Add(new GuestViewModel.Preferences.PreferenceItemValue()
                        {
                            PreferenceID = pref.preferenceID,
                            Preference = pref.tblPreferences.name,
                            Value = pref.preferenceText
                        });
                        PreferencesList.Add(pref.preferenceID);
                    }

                    preferences.PreferencesIDs = PreferencesList.ToArray();
                    preferences.Preferences = preferences.Preferences.OrderBy(x => x.PreferenceTypeName).ToList();
                }
                else
                {
                    preferences.PreferencesIDs = new int[] { };
                    preferences.Preferences = new List<GuestViewModel.Preferences.PreferenceTypeGuest>();
                }

                //última reservación
                var LastArrivalQ = (from a in db.tblArrivals
                                    join resort in db.tblPlaces on a.frontOfficeResortID equals resort.frontOfficeResortID
                                    into a_resort
                                    from resort in a_resort.DefaultIfEmpty()
                                    where a.guestHubID == guestID
                                    orderby a.arrivalDate descending
                                    select new
                                    {
                                        a.crs,
                                        a.arrivalDate,
                                        a.nights,
                                        a.adults,
                                        a.children,
                                        a.infants,
                                        a.guestComments,
                                        a.frontOfficeReservationID,
                                        resort.place
                                    }).FirstOrDefault();
                if (LastArrivalQ != null)
                {
                    preferences.Reservation = new GuestViewModel.LastReservation()
                    {
                        ReservationID = LastArrivalQ.frontOfficeReservationID,
                        Resort = LastArrivalQ.place,
                        CRS = LastArrivalQ.crs,
                        ArrivalDate = LastArrivalQ.arrivalDate,
                        Nights = LastArrivalQ.nights,
                        Adults = LastArrivalQ.adults,
                        Children = LastArrivalQ.children,
                        Infants = LastArrivalQ.infants
                    };
                }
                else
                {
                    preferences.Reservation = null;
                }
            }

            return preferences;
        }

        public static Guid GetGuestHubID(long frontOfficeGuestID, int frontOfficeResortID, string email = null)
        {
            Guid GuestHubID = new Guid();
            ePlatEntities db = new ePlatEntities();
            var FOGuestID = (from x in db.tblGuestHub_FrontOffice
                             where x.frontOfficeGuestID == frontOfficeGuestID
                             && x.frontOfficeResortID == frontOfficeResortID
                             select x.guestHubID).FirstOrDefault();

            if (FOGuestID != null && FOGuestID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                GuestHubID = FOGuestID;
            }
            else
            {
                //buscar por correo
                if (email == null)
                {
                    FrontOfficeViewModel.ContactInfo contact = FrontOfficeDataModel.GetContactInfo(frontOfficeResortID, frontOfficeGuestID);
                    if (contact.Email != null && contact.Email != "" && !contact.Email.Contains("tiene") && contact.Email != "@")
                    {
                        email = contact.Email;
                    }
                }
                if (email != null)
                {
                    var EmGuestID = (from x in db.tblGuestsHub
                                     where x.email1 == email
                                     || x.email2 == email
                                     || x.email3 == email
                                     select x.guestHubID).FirstOrDefault();

                    if (EmGuestID != null && EmGuestID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                    {
                        GuestHubID = EmGuestID;
                    }
                }
            }
            return GuestHubID;
        }

        public static Guid GetHubID(int resortid, long guestid, string firstName, string lastName, string surName, string countryCode, string source, ref ePlatEntities db)
        {
            Guid hubID = new Guid();

            //buscar registro de idhuesped
            var GuestHubQ = (from g in db.tblGuestHub_FrontOffice
                             where g.frontOfficeResortID == resortid
                             && g.frontOfficeGuestID == guestid
                             select g).FirstOrDefault();

            if (GuestHubQ != null)
            {
                //ya existe un GuestHubID para ese cliente
                hubID = GuestHubQ.guestHubID;
            }
            else
            {
                //no existe un GuestHubID para ese frontOfficeGuestID, pero tal vez haya ya un registro con ese correo.
                //obtener email
                FrontOfficeViewModel.ContactInfo contactInfo = FrontOfficeDataModel.GetContactInfo(resortid, guestid);
                string email = contactInfo.Email;

                string phone = null;
                if(contactInfo.Phone != null && contactInfo.Phone != "")
                {
                    phone = contactInfo.Phone;
                    if(phone.IndexOf(" ") >= 0)
                    {
                        phone = phone.Substring(0, phone.IndexOf(" "));
                    }
                    if(phone.Length > 10)
                    {
                        phone = phone.Substring(phone.Length - 10);
                    }
                }

                if (email != null && email.IndexOf("tiene") >= 0)
                {
                    email = null;
                }

                if (email != null && email != "")
                {
                    //si existe el correo, buscar registro GuestHub por correo
                    var GuestEmailQ = (from e in db.tblGuestsHub
                                       where e.email1 == email
                                       || e.email2 == email
                                       || e.email3 == email
                                       select e.guestHubID).FirstOrDefault();


                    if (GuestEmailQ != null && GuestEmailQ != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                    {
                        //si ya existe, regresar el GuestHubID
                        hubID = GuestEmailQ;
                    }
                    else
                    {
                        //si no existe, registrar en GuestHub
                        hubID = SaveNewGuest(
                            resortid,
                            guestid,
                            firstName,
                            lastName,
                            surName,
                            email,
                            countryCode,
                            source, phone);
                    }
                }
                else
                {
                    //si no tiene correo, crear el registro sin correo
                    hubID = SaveNewGuest(
                            resortid,
                            guestid,
                            firstName,
                            lastName,
                            surName,
                            email,
                            countryCode,
                            source, phone);
                }
            }
            return hubID;
        }

        public static GuestViewModel.GuestHubIDJSONResponse GetGuestHubID(int resortid, string rk, DateTime date, string source)
        {
            GuestViewModel.GuestHubIDJSONResponse GuestHubIDJSON = new GuestViewModel.GuestHubIDJSONResponse();
            GuestHubIDJSON.GuestHubID = Guid.Parse("00000000-0000-0000-0000-000000000000");
            //GuestHubIDJSON.GuestHubID = Guid.Parse("bc4d640b-73d4-4a4c-b4d6-091d804c21f4");

            ePlatEntities db = new ePlatEntities();

            //buscar si no existe ya la llegada guardada
            var Arrival = (from a in db.tblArrivals
                           where a.frontOfficeResortID == resortid
                           && a.confirmationNumber.Trim() == rk
                           select new
                           {
                               a.arrivalID,
                               a.guestHubID,
                               a.frontOfficeGuestID,
                               a.firstName,
                               a.lastName,
                               a.countryCode
                           }).FirstOrDefault();

            if (Arrival != null)
            {
                if (Arrival.guestHubID != null)
                {
                    //si ya está relacionada a guest hub
                    GuestHubIDJSON.GuestHubID = (Guid)Arrival.guestHubID;
                }
                else
                {
                    GuestHubIDJSON.GuestHubID = GetHubID(resortid, (long)Arrival.frontOfficeGuestID, Arrival.firstName, Arrival.lastName, null, Arrival.countryCode, source, ref db);
                }
            }
            else
            {
                //obtener idhuesped de llegada
                var Arrivals = FrontOfficeDataModel.GetArrivals(resortid, date, date);

                var frontOfficeGuestQ = (from r in Arrivals
                                         where r.numconfirmacion.Trim() == rk
                                         select r).FirstOrDefault();

                if (frontOfficeGuestQ != null)
                {
                    GuestHubIDJSON.GuestHubID = GetHubID(resortid, (long)frontOfficeGuestQ.idhuesped, frontOfficeGuestQ.nombres, frontOfficeGuestQ.apellidopaterno, frontOfficeGuestQ.apellidomaterno, frontOfficeGuestQ.codepais, source, ref db);
                    SaveArrival(frontOfficeGuestQ, GuestHubIDJSON.GuestHubID, ref db);
                }
            }

            if (GuestHubIDJSON.GuestHubID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                //obtener correos del cliente

                //buscar relaciones en otros hoteles a partir del correo

                //buscar leads en eplat a partir del correo

                //guardar relaciones
            }

            return GuestHubIDJSON;
        }

        public static bool SaveArrival(FrontOfficeViewModel.LlegadasResult arrival, Guid guestHubID, ref ePlatEntities db, bool? useMembership = false)
        {

            //registrar Arrival
            tblArrivals a = new tblArrivals();
            a.arrivalID = Guid.NewGuid();
            a.frontOfficeReservationID = (long)arrival.idReservacion;
            a.frontOfficeResortID = (int)arrival.idresort;
            a.frontOfficeGuestID = (long)arrival.idhuesped;
            a.arrivalDate = DateTime.Parse(arrival.llegada.Value.ToString("yyyy-MM-dd ") + arrival.HLlegada);
            a.roomNumber = arrival.NumHab ?? "";
            a.nights = (int)arrival.CuartosNoche;
            a.guest = arrival.Huesped;
            a.firstName = arrival.nombres;
            a.lastName = arrival.apellidopaterno;
            a.adults = (int)arrival.Adultos;
            if (arrival.Ninos != null)
            {
                a.children = (int)arrival.Ninos;
            }
            else
            {
                a.children = 0;
            }
            if (arrival.Infantes != null)
            {
                a.infants = (int)arrival.Infantes;
            }
            else
            {
                a.infants = 0;
            }
            a.country = arrival.namepais;
            a.countryCode = arrival.codepais;
            if (a.nationality == null && arrival.codigostatusreservacion == "IN")
            {
                a.nationality = (arrival.codepais == "MX" ? "NAC" : "INT");
            }
            if (arrival.nameagencia != null)
            {
                a.agencyName = arrival.nameagencia;
            }
            if (arrival.codeagencia != null)
            {
                a.agencyCode = arrival.codeagencia;
            }
            a.source = arrival.Procedencia;
            a.marketCode = arrival.CodigoMerc;
            a.reservationStatus = arrival.codigostatusreservacion;
            a.roomType = arrival.TipoHab;
            a.confirmationNumber = arrival.numconfirmacion.Trim();
            a.crs = arrival.CRS.Trim();
            a.dateSaved = DateTime.Now;
            a.guestHubID = guestHubID;
            if (arrival.Contrato != null && arrival.Contrato != "" && !arrival.Contrato.Contains("PENDIENTE"))
            {
                a.contract = arrival.Contrato.Trim();
            }
            else
            {
                a.contract = null;
            }
            if (arrival.TipoPlan != null)
            {
                a.planType = arrival.TipoPlan;
            }
            a.terminalID = 42;
            a.split = arrival.Split;

            if (useMembership == true && Membership.GetUser() != null)
            {
                UserSession session = new UserSession();
                a.lastUpdateUserID = session.UserID;
            }
            else
            {
                a.lastUpdateUserID = new Guid("C53613B6-C8B8-400D-95C6-274E6E60A14A");
            }

            db.tblArrivals.AddObject(a);
            db.SaveChanges();

            return true;
        }

        public static Guid SaveNewGuest(int frontOfficeResortID, decimal? frontOfficeGuestID, string firstName, string lastName, string surname, string email, string countryCode, string source, string phone)
        {
            Guid GuestHubID = new Guid();

            ePlatEntities db = new ePlatEntities();
            tblGuestsHub newGuest = new tblGuestsHub();

            newGuest.guestHubID = Guid.NewGuid();
            newGuest.firstName = firstName;
            newGuest.lastName = lastName;
            newGuest.secondSurname = surname;
            newGuest.email1 = (email != null && email != "" && email != "@" ? email.Trim() : null);
            switch (countryCode.Trim())
            {
                case "CA":
                    newGuest.countryID = 2;
                    break;
                case "US":
                    newGuest.countryID = 1;
                    break;
                case "MX":
                    newGuest.countryID = 3;
                    break;
            }
            newGuest.dateSaved = DateTime.Now;
            newGuest.source = source;
            newGuest.phone = phone;

            db.tblGuestsHub.AddObject(newGuest);

            tblGuestHub_FrontOffice nfo = new tblGuestHub_FrontOffice();
            nfo.guestHubID = newGuest.guestHubID;
            nfo.frontOfficeGuestID = (long)frontOfficeGuestID;
            nfo.frontOfficeResortID = frontOfficeResortID;

            db.tblGuestHub_FrontOffice.AddObject(nfo);

            db.SaveChanges();

            GuestHubID = newGuest.guestHubID;

            return GuestHubID;
        }

        public static void UpdatePreferences(Guid id, GuestViewModel.PreferencesModel model)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime currentDateTime = DateTime.Now;
            string source = "API";
            if (model.Source != null)
            {
                source = model.Source;
            }

            //new preferences
            if (model.PreferencesIDs != null && model.PreferencesIDs != "")
            {
                int[] preferencesIDs = model.PreferencesIDs.Split(',').Select(Int32.Parse).ToArray();

                //guardar nuevas preferencias
                foreach (var preferenceid in preferencesIDs)
                {
                    tblGuestPreferences newPreference = new tblGuestPreferences();
                    newPreference.guestPreferenceID = Guid.NewGuid();
                    newPreference.preferenceID = preferenceid;
                    newPreference.guestHubID = id;
                    newPreference.dateSaved = currentDateTime;
                    newPreference.source = source;
                    db.tblGuestPreferences.AddObject(newPreference);
                }
            }

            //open preferences
            if (model.OpenPreferences != null)
            {
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                List<GuestViewModel.OpenPreference> openPreferences = js.Deserialize<List<GuestViewModel.OpenPreference>>(model.OpenPreferences);
                foreach (var preference in openPreferences)
                {
                    tblGuestPreferences oPreference = new tblGuestPreferences();
                    oPreference.guestPreferenceID = Guid.NewGuid();
                    oPreference.preferenceID = preference.PreferenceID;
                    oPreference.guestHubID = id;
                    oPreference.preferenceText = preference.Value;
                    oPreference.dateSaved = currentDateTime;
                    oPreference.source = source;
                    db.tblGuestPreferences.AddObject(oPreference);
                }
            }

            //guardar
            db.SaveChanges();
        }

        public static GuestViewModel.GuestsByPreference GetGuestsByPreference(int resortID, string fromDate, string toDate, string preferences)
        {
            ePlatEntities db = new ePlatEntities();
            GuestViewModel.GuestsByPreference Guests = new GuestViewModel.GuestsByPreference();
            DateTime fDate = DateTime.Parse(fromDate);
            DateTime tDate = DateTime.Parse(toDate).AddDays(1);
            int[] arrPreferences = preferences.Split(',').Select(m => int.Parse(m)).ToArray();

            //llenar objeto
            Guests.FromDate = fromDate;
            Guests.ToDate = toDate;
            Guests.ResortID = resortID;
            Guests.PreferenceIDs = preferences;
            Guests.Preferences = new List<GuestViewModel.Preferences.PreferenceName>();
            Guests.Guests = new List<GuestViewModel.Guest.GuestInfo>();

            //lista de preferencias solicitadas
            var Preferences = from p in db.tblPreferences
                              where arrPreferences.Contains(p.preferenceID)
                              select new
                              {
                                  p.preferenceID,
                                  p.name
                              };

            foreach (var pref in Preferences)
            {
                GuestViewModel.Preferences.PreferenceName newPref = new GuestViewModel.Preferences.PreferenceName();
                newPref.PreferenceID = pref.preferenceID;
                newPref.Preference = pref.name;
                Guests.Preferences.Add(newPref);
            }

            //buscar llegadas en resort con ese rango de fechas y guesthubid diferente de null, cruzado con preferencias
            var Arrivals = from a in db.tblArrivals
                           //join p in db.tblGuestPreferences
                           //on a.guestHubID equals p.guestHubID
                           //into a_p
                           //from p in a_p.DefaultIfEmpty()
                           join g in db.tblGuestsHub
                           on a.guestHubID equals g.guestHubID
                           into a_g
                           from g in a_g.DefaultIfEmpty()
                           where a.frontOfficeResortID == resortID
                           && a.arrivalDate >= fDate
                           && a.arrivalDate < tDate
                           && a.guestHubID != null
                           && !arrPreferences.Except(g.tblGuestPreferences.Select(x => x.preferenceID)).Any()
                           select new
                           {
                               a.guestHubID,
                               a.frontOfficeGuestID,
                               a.firstName,
                               a.lastName,
                               g.email1,
                               a.confirmationNumber,
                               a.arrivalDate
                           };

            foreach (var arr in Arrivals)
            {
                GuestViewModel.Guest.GuestInfo guest = new GuestViewModel.Guest.GuestInfo();

                guest.GuestHubID = arr.guestHubID;
                guest.GuestID = arr.frontOfficeGuestID;
                guest.FirstName = arr.firstName;
                guest.LastName = arr.lastName;
                guest.Email = arr.email1;
                guest.RK = arr.confirmationNumber.Trim();
                guest.ArrivalDate = arr.arrivalDate.ToString("yyyy-MM-dd");

                Guests.Guests.Add(guest);
            }

            return Guests;
        }
    }
}
