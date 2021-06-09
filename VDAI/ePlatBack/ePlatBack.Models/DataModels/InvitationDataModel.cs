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
using System.Web.Security;
using ePlatBack.Models.SPIModels.TheVillaGroup;
using System.Data.SqlClient;
using System.Diagnostics;



namespace ePlatBack.Models.DataModels
{
    public class InvitationDataModel
    {
        public static UserSession session = new UserSession();
        public class InvitationCatalogs
        {
            public static DependantFields GetDependantFields()
            {
                ePlatEntities db = new ePlatEntities();
                DependantFields df = new DependantFields();
                df.Fields = new List<DependantFields.DependantField>();
                
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                long terminalID = terminals[0];
                int? companyGroupID = db.tblTerminals.FirstOrDefault(x => x.terminalID == terminalID).companiesGroupID;
                var destinationsQ = from d in db.tblTerminals_Destinations
                                    where terminals.Contains(d.terminalID)
                                    select d.destinationID;

                DependantFields.FieldValue valDefault = new DependantFields.FieldValue();
                valDefault.ParentValue = null;
                valDefault.Value = "";
                valDefault.Text = "--Select One--";

                //ProgramID > LocationID
                DependantFields.DependantField LocationID = new DependantFields.DependantField();
                LocationID.Field = "LocationID";
                LocationID.ParentField = "ProgramID";
                LocationID.Values = new List<DependantFields.FieldValue>();

                var LocationsQ = from l in db.tblLocations
                                 where l.companiesGroupID == companyGroupID
                                 && destinationsQ.Contains(l.destinationID)
                                 orderby l.location
                                 select new
                                 {
                                     l.locationID,
                                     l.location,
                                     l.programID
                                 };

                foreach (var location in LocationsQ)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = location.programID;
                    val.Value = location.locationID.ToString();
                    val.Text = location.location;
                    LocationID.Values.Add(val);
                }
                LocationID.Values.Insert(0, valDefault);

                df.Fields.Add(LocationID);

                //ProgramID > PromotionTeamID
                DependantFields.DependantField PromotionTeamID = new DependantFields.DependantField();
                PromotionTeamID.Field = "PromotionTeamID";
                PromotionTeamID.ParentField = "ProgramID";
                PromotionTeamID.Values = new List<DependantFields.FieldValue>();

                var PromotionTeams = from p in db.tblPromotionTeams
                                     where destinationsQ.Contains(p.destinationID)
                                     orderby p.promotionTeam
                                     select p;

                foreach (var team in PromotionTeams)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = team.programID.ToString();
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

                List<int> promoTeamsArr = PromotionTeams.Select(x => x.promotionTeamID).ToList();

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
                OPCID.Values.Add(new DependantFields.FieldValue()
                {
                    ParentValue = null,
                    Value = "-1",
                    Text = "Other"
                });
                df.Fields.Add(OPCID);

                return df;
            }

            public static List<SelectListItem> FillDrpDepositOptions()
            {
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "--Select One--"
                });
                list.Add(new SelectListItem()
                {
                    Value = "20,USD",
                    Text = "$20 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "40,USD",
                    Text = "$40 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "60,USD",
                    Text = "$60 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "80,USD",
                    Text = "$80 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "100,USD",
                    Text = "$100 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "120,USD",
                    Text = "$120 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "140,USD",
                    Text = "$140 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "160,USD",
                    Text = "$160 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "180,USD",
                    Text = "$180 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "200,USD",
                    Text = "$200 USD"
                });
                list.Add(new SelectListItem()
                {
                    Value = "100,MXN",
                    Text = "$100 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "200,MXN",
                    Text = "$200 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "300,MXN",
                    Text = "$300 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "400,MXN",
                    Text = "$400 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "500,MXN",
                    Text = "$500 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "600,MXN",
                    Text = "$600 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "700,MXN",
                    Text = "$700 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "800,MXN",
                    Text = "$800 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "900,MXN",
                    Text = "$900 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "1000,MXN",
                    Text = "$1,000 MXN"
                });
                list.Add(new SelectListItem()
                {
                    Value = "-1",
                    Text = "Other"
                });
                return list;
            }

            public static List<SelectListItem> FillDrpHours()
            {
                List<SelectListItem> hours = new List<SelectListItem>();
                hours.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "..."
                });
                hours.Add(new SelectListItem()
                {
                    Value = "01",
                    Text = "01"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "02",
                    Text = "02"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "03",
                    Text = "03"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "04",
                    Text = "04"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "05",
                    Text = "05"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "06",
                    Text = "06"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "07",
                    Text = "07"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "08",
                    Text = "08"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "09",
                    Text = "09"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "10",
                    Text = "10"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "11",
                    Text = "11"
                });
                hours.Add(new SelectListItem()
                {
                    Value = "12",
                    Text = "12"
                });
                return hours;
            }

            public static List<SelectListItem> FillDrpMinutes()
            {
                List<SelectListItem> minutes = new List<SelectListItem>();
                minutes.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "..."
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "00",
                    Text = "00"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "05",
                    Text = "05"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "10",
                    Text = "10"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "15",
                    Text = "15"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "20",
                    Text = "20"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "25",
                    Text = "25"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "30",
                    Text = "30"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "35",
                    Text = "35"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "40",
                    Text = "40"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "45",
                    Text = "45"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "50",
                    Text = "50"
                });
                minutes.Add(new SelectListItem()
                {
                    Value = "55",
                    Text = "55"
                });
                return minutes;
            }

            public static List<SelectListItem> FillDrpMeridians()
            {
                List<SelectListItem> meridians = new List<SelectListItem>();
                meridians.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "..."
                });
                meridians.Add(new SelectListItem()
                {
                    Value = "a. m.",
                    Text = "a. m."
                });
                meridians.Add(new SelectListItem()
                {
                    Value = "p. m.",
                    Text = "p. m."
                });
                return meridians;
            }

            public static List<SelectListItem> FillDrpConfirmationStatuses()
            {
                List<SelectListItem> statuses = new List<SelectListItem>();
                statuses.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "--Select One--"
                });
                statuses.Add(new SelectListItem()
                {
                    Value = "true",
                    Text = "Confirmed"
                });
                statuses.Add(new SelectListItem()
                {
                    Value = "false",
                    Text = "Not Confirmed"
                });
                return statuses;
            }
        }

        public static InvitationViewModel.InvitationsResponse GetInvitations(DateTime? date)
        {
            InvitationViewModel.InvitationsResponse response = new InvitationViewModel.InvitationsResponse();

            response.Invitations = new List<InvitationViewModel.InvitationInfoModel>();

            ePlatEntities db = new ePlatEntities();
            
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            date = date ?? DateTime.Today;
            DateTime tomorrow = date.Value.AddDays(1);

            //obtener lista de subordinados
            List<Guid> teamUsers = new List<Guid>();
            var subordinatedUsers = db.sp_getSubordinatedUsers(session.UserID);

            foreach (var u in subordinatedUsers)
            {
                if (u.userID != null)
                {
                    teamUsers.Add((Guid)u.userID);
                }
            }
            teamUsers.Add(session.UserID);

            List<SelectListItem> list = new List<SelectListItem>();
            long terminalID = terminals[0];
            var Invitations = from i in db.tblInvitations
                              join country in db.tblCountries on i.countryID equals country.countryID
                              into i_country from country in i_country.DefaultIfEmpty()
                              join place in db.tblPlaces on i.placeID equals place.placeID
                              into i_place from place in i_place.DefaultIfEmpty()
                              join program in db.tblProspectationPrograms on i.programID equals program.programID
                              into i_program from program in i_program.DefaultIfEmpty()
                              join opc in db.tblOPCS on i.opcID equals opc.opcID
                              into i_opc from opc in i_opc.DefaultIfEmpty()
                              join profile in db.tblUserProfiles on i.savedByUserID equals profile.userID
                              into i_profile from profile in i_profile.DefaultIfEmpty()
                              where i.presentationDateTime >= date
                              && i.presentationDateTime < tomorrow
                              && i.terminalID == terminalID
                              && teamUsers.Contains(i.savedByUserID)
                              select new
                              {
                                  i.invitationID,
                                  i.presentationDateTime,
                                  i.guest,
                                  i.countryID,
                                  country.country,
                                  i.state,
                                  i.destinationID,
                                  i.placeID,
                                  i.resortOther,
                                  place.place,
                                  i.depositAmount,
                                  i.depositCurrencyCode,
                                  i.pickUpTime,
                                  i.guestPhone,
                                  i.guestEmail,
                                  i.invitationNumber,
                                  i.pickUpNotes,
                                  i.gifts,
                                  i.programID,
                                  program.program,
                                  i.culture,
                                  i.confirmed,
                                  i.locationID,
                                  i.promotionTeamID,
                                  i.opcID,
                                  i.opcOther,
                                  opc.opc,
                                  i.dateSaved,
                                  i.showNoShow,
                                  i.welcomeLetterDate,
                                  profile.firstName,
                                  profile.lastName,
                                  i.spiCustomerID
                              };

            foreach (var inv in Invitations)
            {
                InvitationViewModel.InvitationInfoModel invitation = new InvitationViewModel.InvitationInfoModel();
                invitation.InvitationID = inv.invitationID;
               // invitation.PresentationDate = inv.presentationDateTime.ToString("yyyy-MM-dd");
                invitation.Guest = inv.guest;
                invitation.CountryID = inv.countryID.Value;
                invitation.Country = inv.country;
                invitation.State = inv.state;
                invitation.DestinationID = inv.destinationID.Value;
                invitation.PlaceID = inv.placeID;
                invitation.ResortOther = inv.resortOther;
                invitation.Resort = inv.placeID != null ? inv.place : inv.resortOther;
                invitation.Deposit = inv.depositAmount + "," + inv.depositCurrencyCode;
               // invitation.PickUpTimeHour = inv.pickUpTime.ToString("hh");
               // invitation.PickUpTimeMinute = inv.pickUpTime.ToString("mm");
                invitation.PickUpTimeMeridian = (inv.pickUpTime > TimeSpan.Parse("12:00:00") ? "p. m." : "a. m.");
               // invitation.PresentationTimeHour = inv.presentationDateTime.ToString("hh");
               // invitation.PresentationTimeMinute = inv.presentationDateTime.ToString("mm");
               // invitation.PresentationTimeMeridian = inv.presentationDateTime.ToString("tt");
                invitation.GuestPhone = inv.guestPhone;
                invitation.GuestEmail = inv.guestEmail;
                invitation.InvitationNumber = inv.invitationNumber;
                invitation.PickUpNotes = inv.pickUpNotes;
                invitation.Gifts = inv.gifts;
                invitation.ProgramID = inv.programID.Value;
                invitation.Program = inv.program;
                invitation.Culture = inv.culture;
                invitation.Confirmed = inv.confirmed;
                invitation.LocationID = inv.locationID;
                invitation.PromotionTeamID = inv.promotionTeamID;
                invitation.OPCID = inv.opcID;
                invitation.OPCOther = inv.opcOther;
                invitation.OPC = (inv.opcID != null ? inv.opc : inv.opcOther);
                invitation.SavedOn = inv.dateSaved.ToString("yyyy-MM-dd hh:mm:dd tt");
                invitation.SavedBy = inv.firstName + " " + inv.lastName;
                invitation.ShowNoShow = inv.showNoShow;
                invitation.WelcomeLetterSentOn = (inv.welcomeLetterDate != null ? inv.welcomeLetterDate.Value.ToString("yyyy-MM-dd hh:mm:dd tt") : "");
                invitation.SPICustomerID = inv.spiCustomerID;
                response.Invitations.Add(invitation);
            }

            return response;
        }

        public static AttemptResponse SaveInvitationInfo(InvitationViewModel.InvitationInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse attempt = new AttemptResponse();
            
            long? terminalID = model.TerminalID;
            if (terminalID == null)
            {
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                terminalID = terminals[0];
            }            

            tblInvitations invitation = new tblInvitations();
            if (model.InvitationID != null)
            {
                invitation = (from i in db.tblInvitations
                              where i.invitationID == model.InvitationID
                              select i).FirstOrDefault();
            }

            invitation.presentationDateTime = DateTime.Parse(model.PresentationDate + " " + (model.PresentationTimeMeridian == "p.m." && int.Parse(model.PresentationTimeHour) < 12 ? (int.Parse(model.PresentationTimeHour) + 12).ToString() : model.PresentationTimeHour) + ":" + model.PresentationTimeMinute + ":00");
            invitation.guest = model.Guest;
            invitation.countryID = model.CountryID;
            invitation.state = model.State;
            invitation.destinationID = model.DestinationID;
            if (model.PlaceID != null)
            {
                if (model.PlaceID == -1)
                {
                    invitation.resortOther = model.ResortOther;
                }
                else
                {
                    invitation.placeID = model.PlaceID;
                }
            }
            if (model.Deposit != null)
            {
                if (model.Deposit == "-1")
                {
                    invitation.depositAmount = model.DepositOther;
                    invitation.depositCurrencyCode = (model.DepositCurrencyID == 1 ? "USD" : "MXN");
                }
                else
                {
                    string[] deposit = model.Deposit.Split(',');
                    invitation.depositAmount = int.Parse(deposit[0]);
                    invitation.depositCurrencyCode = deposit[1];
                }                
            }
            invitation.pickUpTime = TimeSpan.Parse((model.PickUpTimeMeridian == "p. m." && int.Parse(model.PickUpTimeHour) < 12 ? (int.Parse(model.PickUpTimeHour) + 12).ToString() : model.PickUpTimeHour) + ":" + model.PickUpTimeMinute + ":00");
            invitation.guestPhone = model.GuestPhone;
            invitation.guestEmail = model.GuestEmail;
            invitation.invitationNumber = model.InvitationNumber;
            invitation.pickUpNotes = model.PickUpNotes;
            invitation.gifts = model.Gifts;
            invitation.programID = model.ProgramID;
            invitation.culture = model.Culture;
            invitation.confirmed = model.Confirmed;
            invitation.locationID = model.LocationID;
            invitation.promotionTeamID = model.PromotionTeamID;
            invitation.opcID = model.OPCID;
            invitation.opcOther = model.OPCOther;
            invitation.terminalID = (long)terminalID;
            invitation.spiCustomerID = model.SPICustomerID;
            if (model.InvitationID != null)
            {
                invitation.dateModified = DateTime.Now;
                invitation.modifiedByUserID = session.UserID;
            }
            else
            {
                invitation.dateSaved = DateTime.Now;
                invitation.savedByUserID = session.UserID;
                invitation.invitationID = Guid.NewGuid();
                db.tblInvitations.AddObject(invitation);

                //envío de eventos
                int eventID = 0;
                if (invitation.programID == 1 || invitation.programID == 10)
                {
                    //inhouse
                    if (invitation.confirmed == true)
                    {
                        //confirmed
                        eventID = 19;
                    }
                    else
                    {
                        //not confirmed
                        eventID = 20;
                    }
                }
                else if (invitation.programID == 3 || invitation.programID == 11)
                {
                    //members
                    if (invitation.confirmed == true)
                    {
                        //confirmed
                        eventID = 25;
                    }
                    else
                    {
                        //not confirmed
                        eventID = 26;
                    }
                }
                else if (invitation.programID == 6 || invitation.programID == 15)
                {
                    //street
                    if (invitation.confirmed == true)
                    {
                        //confirmed
                        eventID = 23;
                    }
                    else
                    {
                        //not confirmed
                        eventID = 24;
                    }
                }
                else if (invitation.programID == 8 || invitation.programID == 16)
                {
                    //airport
                    if (invitation.confirmed == true)
                    {
                        //confirmed
                        eventID = 21;
                    }
                    else
                    {
                        //not confirmed
                        eventID = 22;
                    }
                }
                System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(eventID, terminalID, invitation.culture);
                if (email != null)
                {
                    email.To.Add(invitation.guestEmail);
                    email.Body = email.Body.Replace("$Guest", invitation.guest);
                    email.Body = email.Body.Replace("$Destination", invitation.tblDestinations.destination);
                    email.Body = email.Body.Replace("$Hotel", invitation.tblPlaces.place);
                   // email.Body = email.Body.Replace("$PresentationDate", Utils.GeneralFunctions.DateFormat.ToText(invitation.presentationDateTime, invitation.culture));
                    email.Body = email.Body.Replace("$PresentationTime", model.PresentationTimeHour + ":" + model.PresentationTimeMinute + " " + model.PresentationTimeMeridian);
                    if (invitation.pickUpNotes != null)
                    {
                        email.Body = email.Body.Replace("$PickUpNotes", invitation.pickUpNotes.Replace(System.Environment.NewLine, "<br />").Replace("\r", "<br />").Replace("\n", "<br />"));
                    }
                    email.Body = email.Body.Replace("$PickupTime", model.PickUpTimeHour + ":" + model.PickUpTimeMinute + " " + model.PickUpTimeMeridian);
                    email.Body = email.Body.Replace("$Deposit", "$" + invitation.depositAmount + " " + invitation.depositCurrencyCode);
                    email.Body = email.Body.Replace("$Gifts", invitation.gifts.Replace(System.Environment.NewLine, "<br />").Replace("\r", "<br />").Replace("\n", "<br />"));

                    //Utils.EmailNotifications.Send(email);
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                }                
            }

            db.SaveChanges();

            attempt.Type = Attempt_ResponseTypes.Ok;
            attempt.ObjectID = invitation.invitationID;
            attempt.Exception = null;
            return attempt;
        }

        public static string GetTerminal()
        {
            ePlatEntities db = new ePlatEntities();
            
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];
            return db.tblTerminals.Single(x => x.terminalID == terminalID).terminal;
        }
    }  
    //----------------------------------------------------------------------------------------------------------------
    public class spiInvitationDataModel
    {
        public List<spiInvitation.invitationModelTable> searchInvitations (spiInvitation.searchInvitation model)
        {
            ePlatEntities db = new ePlatEntities();
            UserSession session = new UserSession();
            List<spiInvitation.invitationModelTable> list = new List<spiInvitation.invitationModelTable>();
            var queryPayType = GeneralFunctions.PaymentTypes.Where(x => long.Parse(x.Key) < 3).ToList();
            var terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
            var role = db.aspnet_Roles.Where(x => x.RoleId == session.RoleID).Select(x => x.RoleName).ToList();
            var Subordinates = spiInvitationDataModel.spiInvitationCatalog.GetSubordinatedUsers();
            var SubordinatesList = Subordinates.Split(',').Select(x => Guid.Parse(x)).ToArray();

            List<spiInvitation.PaymentTypes> paymentTypes = new List<spiInvitation.PaymentTypes>();
            foreach(var n in queryPayType)
            {
                paymentTypes.Add(new spiInvitation.PaymentTypes() {
                    paymenTypeID = int.Parse(n.Key),
                    paymentType = n.Value
                });
            }

            var query = from x in db.tblInvitations
                        join su in db.tblUserProfiles on x.savedByUserID equals su.userID
                        join p in db.tblPlaces on x.presentationPlaceID equals p.placeID
                        where /* parametros busqueda*/
                              ((x.presentationDateTime == model.searchPresentationDate || model.searchPresentationDate.Value == null)
                              && (x.dateSaved == model.searchInvitationByDate || model.searchInvitationByDate.Value == null)
                              && (x.spiHotelID == model.searchInvitationByHotel || model.searchInvitationByHotel == 0)
                              && (x.spiGroupID == model.searchInvitationGroups || model.searchInvitationGroups == 0)
                              && (x.spiTeamID == model.searchInvitationByTeam || model.searchInvitationByTeam == null))
                              && (x.invitationNumber == model.searchInvitationNumber || model.searchInvitationNumber == null)
                              && /*roles*/
                              ((role.Contains("OPC") && session.UserID == x.savedByUserID)
                              || (role.Contains("Direction Asistant") && terminals.Contains(x.terminalID) && SubordinatesList.Contains(x.savedByUserID))
                              || (role.Contains("Administrator")))
                        select new
                        {
                            invitationID = x.invitationID,
                            presentationDateTime = x.presentationDateTime,
                            pickUpTime = x.pickUpTime,
                            firstName = x.firstName,
                            secondName = x.secondName,
                            lastName = x.lastName,
                            spouseFirstName = x.spouseFirstName,
                            spouseSecondName = x.spouseSecondName,
                            spouseLastName = x.spouseLastName,
                            guestEmail = x.guestEmail,
                            guestPhone = x.guestPhone,
                            invitationNumber = x.invitationNumber,
                            depositCurrencyCode = x.depositCurrencyCode,
                            depositAmount = x.depositAmount,
                            spiLanguageID = x.spiLanguageID,
                            pickUpNotes = x.pickUpNotes,
                            gifts = x.gifts,
                            confirmed = x.confirmed,
                            opcID = x.opcID,
                            spiOpcID = x.opcID,
                            state = x.state,
                            spiCountryID = x.spiCountryID,
                            spiTeamID = x.spiTeamID,
                            spiGroupID = x.spiGroupID,
                            spiHotelID = x.spiHotelID,
                            spiLocationID = x.spiLocationID,
                            spiSalesRoomID = x.spiSalesRoomID,
                            dateSaved = x.dateSaved,
                            savedByUserID = x.savedByUserID,
                            savedByUser = su.firstName + " " + su.lastName,
                            dateModified = x.dateModified,
                            modifiedByUserID = x.modifiedByUserID,
                            //  modifiedByUser = mu.firstName + " " + mu.lastName,
                            spouse = x.spouseFirstName + " " + x.spouseLastName,
                            guest = x.firstName + " " + x.lastName,
                            jaladorOpcID = x.jaladoresOpcID,
                            programID = x.programID,
                            presentationPlaceID = x.presentationPlaceID,
                            presentationPlace = p.place,
                            presentationPlaceOld = p.oldPlaceName,
                            pickUpTypeID = x.pickUpTypeID,
                            manifestFolio = x.manifestFolio,
                            shiftID = x.shiftID,
                            depositPickUpID = x.depositPickUpID,
                            adults = x.adult,
                            childs = x.child,
                            depositPickUp = x.depositPickUpID,
                            spiCategoryID = x.spiCategoryID
                        };

            List<spiInvitation.invitationDeposits> invitationDeposits = (from d in db.tblInvitationDeposits
                                                                         join c in db.tblCurrencies on d.currencyID equals c.currencyID
                                                                         where query.Count(x => x.invitationID == d.invitationID) > 0 && d.invitationID != null
                                                                         select new spiInvitation.invitationDeposits()
                                                                         {
                                                                             invitationDepositID = d.invitationDepositID,
                                                                             invitationID= d.invitationID,
                                                                             amount = d.amount,
                                                                             currencyID = d.currencyID,
                                                                             currency = c.currencyCode,
                                                                             paymentTypeID = d.paymentTypeID,
                                                                             ccReferenceNumber = d.ccReferenceNumber,
                                                                             received = d.received,
                                                                             dateSaved = d.dateSaved,
                                                                             savedByUserID = d.savedByUserID,
                                                                             dateLastModification = d.dateLastModification.Value,
                                                                             modifiedByUserID = d.modifiedByUserID.Value,
                                                                             deleted = d.deleted.Value,
                                                                             deletedByUserID = d.deletedByUserID.Value,
                                                                             deletedDateTime = d.deletedDate.Value
                                                                         }).OrderByDescending(x => x.dateSaved).ToList();
            foreach (var x in query)
            {
                spiInvitation.invitationModelTable item = new spiInvitation.invitationModelTable();
                bool manifest = false;
                if (x.depositPickUpID.HasValue)
                {
                    manifest = x.depositPickUpID==2 ? false : db.tblSPIPremanifest.Count(n => n.invitationID == x.invitationID) > 0 ? false : true;
                }
                else
                {
                    manifest = db.tblSPIPremanifest.Count(n => n.invitationID == x.invitationID) > 0 ? false : true;
                }
                //si no hay registro ó depositPickUp ==2 aun no esta premanifestado
                
                var modifiedUser = x.modifiedByUserID != null ? db.tblUserProfiles.Single(n => n.userID == x.modifiedByUserID) : null;
               // var datePickUP =  DateTime.Now.Add(x.pickUpTime.Value);
                          item.invitationID = x.invitationID;
                          item.presentationDateTime = x.presentationDateTime == null ? new DateTime() :x.presentationDateTime.Value;
                         //item.presentationDateTimeFormat = x.presentationDateTime.ToString("yyyy-MM-dd");
                          item.presentationDateTimeFormat = x.presentationDateTime == null ? "" : x.presentationDateTime.Value.ToString("yyyy-MM-dd");
                         //item.pickUpTime = DateTime.Today.Add(x.pickUpTime).ToString("hh:mm tt");
                          item.pickUpTime = x.pickUpTime == null ? "" : DateTime.Today.Add(x.pickUpTime.Value).ToString("hh:mm tt");
                         //item.pickUpTimeFormat = DateTime.Today.Add(x.pickUpTime).ToString("hh:mm tt");
                          item.pickUpTimeFormat = x.pickUpTime == null ? "":DateTime.Today.Add(x.pickUpTime.Value).ToString("hh:mm tt");
                          item.firstName = x.firstName;
                          item.secondName = x.secondName;
                          item.lastName = x.lastName;
                          item.spouseFirstName = x.spouseFirstName;
                          item.spouseSecondName = x.spouseSecondName;
                          item.spouseLastName = x.spouseLastName;
                          item.guestEmail = x.guestEmail;
                          item.guestPhone = x.guestPhone;
                          item.invitationNumber = x.invitationNumber;
                          item.depositCurrencyCode = x.depositCurrencyCode;
                          item.depositAmount = x.depositAmount;
                          item.spiLanguageID = x.spiLanguageID;
                          item.spiCountryID = x.spiCountryID;
                          item.state = x.state;
                          item.pickUpNotes = x.pickUpNotes;
                          item.gift = x.gifts;
                          item.confirmed = x.confirmed;
                          item.opcID = x.opcID;                         
                          item.spiTeamID = x.spiTeamID;
                          item.spiGroupID = x.spiGroupID;
                          item.spiHotelID = x.spiHotelID;
                          item.spiLocationID = x.spiLocationID;
                          item.spiSalesRoomID = x.spiSalesRoomID;
                          item.dateModified = x.dateModified;
                          item.modifiedByUser = modifiedUser != null ? "<br />" + modifiedUser.firstName + " " + modifiedUser.lastName : null;
                          item.spouse = x.spouse;
                          item.guest = x.guest;
                          item.jaladorOpcID = x.jaladorOpcID;
                          item.programID = x.programID;
                          item.presentationPlaceID = x.presentationPlaceID;
                          if(x.presentationPlaceOld == null)
                          {
                              item.presentationPlace = x.presentationPlace;
                          }
                          else
                          {
                              item.presentationPlace = x.presentationPlaceOld;
                          }
                          item.saved = x.dateSaved.ToString("yyyy-MM-dd") + " " + x.savedByUser;
                          item.modified = x.dateModified.HasValue ? x.dateModified.Value.ToString("yyyy-MM-dd") + System.Environment.NewLine + modifiedUser.firstName + " " + modifiedUser.lastName:"";
                          item.modifiedByUserID = x.modifiedByUserID;
                          item.premanifest = item.presentationDateTime != null ? manifest : false;
                            //premanifest
                            /*  if (item.presentationDateTime != null)
                              {
                                  item.premanifest = manifest;
                              }
                              else
                              {
                                  item.premanifest = false;
                              }*/
                            //
                          item.manifestFolio = x.manifestFolio.HasValue ? x.manifestFolio.Value : 0;
                          item.shiftID = x.shiftID.HasValue ? x.shiftID.Value:0;
                          item.depositPickUpID = x.depositPickUpID.HasValue ? x.depositPickUpID.Value :0;
                          item.pickUpTypeID = x.pickUpTypeID;
                          item.invitationDeposits = new List<spiInvitation.invitationDeposits>();
                          item.invitationDeposits.AddRange(invitationDeposits.Where(n => n.invitationID == x.invitationID));
                          item.adults = x.adults;
                          item.childs = x.childs;
                          item.spiCategoryID = x.spiCategoryID;
                          list.Add(item);
            }

            foreach(var deposit in invitationDeposits)
            {
                deposit.paymentType = paymentTypes.FirstOrDefault(x => x.paymenTypeID == deposit.paymentTypeID).paymentType;
            }

            return list;
        }      
        public List<spiInvitation.guestToMatch> searchGuests(spiInvitation.invitationModel model)
        {
            SpiTvgEntities spiDB = new SpiTvgEntities();
            List<spiInvitation.guestToMatch> list = new List<spiInvitation.guestToMatch>();
            foreach(var guest in spiDB.spCustomer_check_tours(model.lastNameModal,model.firstNameModal,model.guestPhoneModal,null,null,model.guestEmailModal,null))
            {
                var tourDateFormat = guest.tour_date.ToString();
                list.Add(new spiInvitation.guestToMatch()
                {
                   guestToMatchID = guest.id,
                   customerName1 = guest.customer_name,
                   customerName2 = guest.customer_name2,
                   city = guest.city,
                   st = guest.st,
                   country = guest.country,
                   tourDate = guest.tour_date,
                   tourDateFormat = tourDateFormat,
                   tour = guest.tour,
                   salesCenter = guest.sales_center,
                   source = guest.source,
                   volumen = guest.Volumen,
                   match = guest.match,
                   contractType = guest.contract_type,
                   contractStatus = guest.contract_status,
                   acountNo = guest.Account_no
                });             
            }
            return list;
        }
        public static AttemptResponse saveInvitation(spiInvitation.invitationModelTable model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();            
            UserSession session = new UserSession();
            var currentUserID = (Guid)Membership.GetUser().ProviderUserKey;
            var userProfile = db.tblUserProfiles.Single(x => x.userID == currentUserID);
            var currentTerminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
            try
                {
                    var pickUpFormat = "";
                    var pickUp = new DateTime?();
                    if (model.pickUpTime != null)
                    {
                         pickUpFormat = DateTime.Now.ToString("yyyy-MM-dd") + " " + model.pickUpTime != "" ? model.pickUpTime.ToString():"";
                         pickUp = DateTime.Parse(pickUpFormat);
                    }

                if (model.invitationID == Guid.Empty)
                //if(db.tblInvitations.Count(x => x.invitationID == model.invitationID)==0)
                {
                    tblInvitations invitation = new tblInvitations();
                    invitation.invitationID = Guid.NewGuid();
                    // invitation.invitationID = model.invitationID;
                    invitation.presentationDateTime = model.presentationDateTime;//
                    model.presentationDateTimeFormat = model.presentationDateTime.Value.ToString("yyyy-MM-dd");
                    invitation.firstName = model.firstName;
                    invitation.secondName = model.secondName;
                    invitation.lastName = model.lastName;
                    invitation.spouseFirstName = model.spouseFirstName;
                    invitation.spouseSecondName = model.spouseSecondName;
                    invitation.spouseLastName = model.spouseLastName;
                    invitation.state = model.state;//
                    invitation.depositCurrencyCode = "USD";
                    invitation.pickUpTime = model.pickUpTime != null ? pickUp.Value.TimeOfDay : (TimeSpan?)null;
                    invitation.guest = model.firstName + " " + model.lastName;//
                    invitation.guestPhone = model.guestPhone;
                    invitation.guestEmail = model.guestEmail;//
                    invitation.invitationNumber = model.invitationNumber;//
                    invitation.pickUpNotes = model.pickUpNotes;
                    invitation.gifts = model.gift;
                    invitation.terminalID = currentTerminals.FirstOrDefault();//                      
                    invitation.dateSaved = DateTime.Now;//
                    invitation.savedByUserID = currentUserID;//
                    invitation.opcID = model.opcID;
                    invitation.spiCustomerID = model.spiCustumerID;
                    invitation.spiCountryID = model.spiCountryID;
                    invitation.spiTeamID = model.spiTeamID;
                    invitation.spiGroupID = model.spiGroupID;
                    invitation.spiLanguageID = model.spiLanguageID;
                    invitation.spiHotelID = model.spiHotelID;
                    invitation.spiLocationID = model.spiLocationID;
                    invitation.spiSalesRoomID = model.spiSalesRoomID;
                    invitation.presentationPlaceID = model.presentationPlaceID;
                    invitation.programID = model.programID;
                    invitation.jaladoresOpcID = model.jaladorOpcID;
                    if (model.guestPhone != null)
                    {
                        var cont = 0;
                        var phoneList = model.guestPhone.Select(x => x).ToArray();
                        for (int x = 1; x <= 3; x++)
                        {
                            if (phoneList[x] == phoneList[0])
                            {
                                cont++;
                            }
                        }
                        if (cont == 3)
                        {
                            invitation.guestPhone = null;
                        }
                    }
                    invitation.pickUpTypeID = model.pickUpTypeID;
                    invitation.shiftID = model.shiftID;
                    invitation.manifestFolio = model.manifestFolio;
                    invitation.depositPickUpID = model.depositPickUpID;
                    invitation.confirmed = model.depositPickUpID == 3 ? true : false;
                    invitation.adult = model.adults;
                    invitation.child = model.childs.HasValue ? model.childs.Value : 0;
                    invitation.spiCategoryID = model.spiCategoryID;
                    //SavePayments(model.invitationDeposits,invitation.invitationID);
                    db.tblInvitations.AddObject(invitation);
                    db.SaveChanges();
                    /*if (invitation.guestEmail != null && invitation.confirmed)
                     {
                         model.guest = model.firstName + " " + model.lastName;
                         model.pickUpTime = model.pickUpTime != null ? pickUp.Value.ToString("HH:mm") : model.pickUpTime;
                         model.invitationID = invitation.invitationID;
                         model.confirmed = invitation.confirmed;
                         var resp = spiInvitationDataModel.sendEmail(true, model);//(sendEmail, model)
                     }*/
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = invitation.invitationID;
                    response.Message = "Invitation Saved Success";
                }
                else
                {
                    var invitation = db.tblInvitations.Single(x => x.invitationID == model.invitationID);
                    //  var depositDetails = db.tblInvitationDeposits.Where(x => x.invitationID == invitation.invitationID);
                    invitation.presentationDateTime = model.presentationDateTime;//
                    model.presentationDateTimeFormat = model.presentationDateTime.Value.ToString("yyyy-MM-dd");
                    invitation.firstName = model.firstName;
                    invitation.secondName = model.secondName;
                    invitation.lastName = model.lastName;
                    invitation.spouseFirstName = model.spouseFirstName;
                    invitation.spouseSecondName = model.spouseSecondName;
                    invitation.spouseLastName = model.spouseLastName;
                    //invitation.depositAmount = model.depositAmount;//
                    invitation.depositCurrencyCode = "USD";
                    invitation.pickUpTime = model.pickUpTime != null ? pickUp.Value.TimeOfDay : (TimeSpan?)null;
                    invitation.guest = model.firstName + " " + model.lastName;
                    invitation.guestPhone = model.guestPhone;
                    invitation.guestEmail = model.guestEmail;//
                    invitation.invitationNumber = model.invitationNumber;//
                    invitation.pickUpNotes = model.pickUpNotes;
                    invitation.gifts = model.gift;
                    //invitation.confirmed = model.confirmed;//                       
                    invitation.dateModified = DateTime.Now;//
                    invitation.modifiedByUserID = currentUserID;//
                    invitation.state = model.state;
                    invitation.opcID = model.opcID;
                    invitation.presentationPlaceID = model.presentationPlaceID;
                    //                  
                    invitation.spiCustomerID = model.spiCustumerID;
                    invitation.spiCountryID = model.spiCountryID;
                    invitation.spiTeamID = model.spiTeamID;
                    invitation.spiGroupID = model.spiGroupID;
                    invitation.spiLanguageID = model.spiLanguageID;
                    invitation.spiHotelID = model.spiHotelID;
                    invitation.spiLocationID = model.spiLocationID;
                    invitation.spiSalesRoomID = model.spiSalesRoomID;
                    invitation.programID = model.programID;
                    invitation.jaladoresOpcID = model.jaladorOpcID;
                    if (model.guestPhone != null)
                    {
                        var cont = 0;
                        var phoneList = model.guestPhone.Select(x => x).ToArray();
                        for (int x = 1; x <= 3; x++)
                        {
                            if (phoneList[x] == phoneList[0])
                            {
                                cont++;
                            }
                        }
                        if (cont == 3)
                        {
                            invitation.guestPhone = null;
                        }
                    }
                    invitation.pickUpTypeID = model.pickUpTypeID;
                    invitation.shiftID = model.shiftID;
                    invitation.manifestFolio = model.manifestFolio;
                    invitation.depositPickUpID = model.depositPickUpID;
                    invitation.confirmed = model.depositPickUpID == 3 ? true : false;
                    invitation.adult = model.adults;
                    invitation.child = model.childs.HasValue ? model.childs.Value : 0;
                    invitation.spiCategoryID = model.spiCategoryID;
                    //SavePayments(model.invitationDeposits,invitation.invitationID);
                    db.SaveChanges();
                        //sendEmail
                        //{
                        /*if (model.guestEmail != null && model.sendEmail == true && model.confirmed==true)
                        {
                            model.guest = model.firstName + " " + model.lastName;
                            model.pickUpTime = model.pickUpTime != null ? pickUp.Value.ToString("HH:mm") : model.pickUpTime;
                            model.invitationID = invitation.invitationID;
                            model.confirmed = invitation.confirmed;
                            spiInvitationDataModel.sendEmail(true, model);//(sendEmail, model)
                        }*/
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.ObjectID = invitation.invitationID;
                        response.Message = "Invitation Updated Success";
                    }                   
                }
                catch(Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Exception = ex;
                    response.ObjectID = 0;
                    response.Message = "Invitation Error";
                }
            return response;
        }     
        public static AttemptResponse preManifestedInvitation(string invitationID, int salesRoomID, int? customerID = null)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            SpiTvgEntities spiDB = new SpiTvgEntities();
            UserSession session = new UserSession();
       
            //verificar si ya existe la invitacion a premanifestar antes
            var newID = Guid.Parse(invitationID);
            try
            {
                 var terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
                 var userSPI = db.tblUserProfiles.FirstOrDefault(x => session.UserID == x.userID).SPIUserName;
                 var country = spiDB.spEP_Pais();
                 var spiInvitationID = Guid.Parse(invitationID);
                 var cusID = customerID != null ? customerID : null;   
                 var invitation = db.tblInvitations.Single(x => x.invitationID == spiInvitationID);
                 tblSPIPremanifest newInvitation = new tblSPIPremanifest();
                 newInvitation.invitationID = newID;
                 newInvitation.customerID = customerID;
                 newInvitation.firstName = invitation.firstName;
                 newInvitation.secondName = invitation.secondName;
                 newInvitation.lastName = invitation.lastName;
                 newInvitation.spouseFirstName = invitation.spouseFirstName;
                 newInvitation.spouseSecondName = invitation.spouseSecondName;
                 newInvitation.spouseLastName = invitation.spouseLastName;
                 newInvitation.guestEmail = invitation.guestEmail;
                 newInvitation.guestPhone = invitation.guestPhone;
                 newInvitation.invitationNumber = invitation.invitationNumber;
                 newInvitation.presentationDateTime = invitation.presentationDateTime.Value.Add(invitation.pickUpTime.Value);
                 newInvitation.spiHotelID = invitation.spiHotelID.Value;
                 newInvitation.spiLocationID = invitation.spiLocationID.Value;
                 newInvitation.spiTeamID = invitation.spiTeamID.Value;
                 newInvitation.spiGroupID = invitation.spiGroupID.Value;
                 newInvitation.countryID = invitation.spiCountryID.Value;
                 newInvitation.country = country.FirstOrDefault(x => x.pais_id == invitation.spiCountryID).pais;
                 newInvitation.spiCountryID = invitation.spiCountryID.Value;
                 newInvitation.spiLanguageID = invitation.spiLanguageID;
                 newInvitation.salesRoomID = salesRoomID;
                 newInvitation.spiOpcID = db.tblOPCS.FirstOrDefault(x => x.opcID == invitation.opcID.Value).legacyKey;
                 newInvitation.tourDate = invitation.presentationDateTime;
                 newInvitation.userSPI = userSPI;
                 newInvitation.preManifestedDate = DateTime.Now;
                 newInvitation.preManifested = false;
                 newInvitation.dateSaved = DateTime.Now;
                 newInvitation.savedByUserID = session.UserID;
                 newInvitation.dateModified = null;
                 newInvitation.modifiedByUserID = null;
                 newInvitation.terminalID = terminals.FirstOrDefault();
                 newInvitation.depositAmount = GetInvitationDepositsTotal(newID);
                 newInvitation.spiCategoryID = invitation.spiCategoryID;
                 //hacer pruebas y remover                 
                 db.tblSPIPremanifest.AddObject(newInvitation);
                 db.SaveChanges();
                 response.Type = Attempt_ResponseTypes.Ok;
                 response.Message = "Invitation Premanifested Success !";
                 response.ObjectID = newID;
            }
            catch(Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error traying to save guest information !!";
                response.Exception = ex;
                response.ObjectID = newID;
            }
            finally
            {
                try 
                {
                 /*  if (db.tblSPIPremanifest.Count(x => x.invitationID == newID && x.preManifested == false) > 0 )
                   {*/                 
                       if (response.Type == Attempt_ResponseTypes.Ok)
                       {                             
                           var pre = db.tblSPIPremanifest.Single(x => x.invitationID == newID);
                           var userData = db.tblUserProfiles.Single(x => x.userID == pre.savedByUserID);
                           string cs = @"Data source=187.174.136.137;initial catalog=SPISales;persist security info=True;user id=gguerrap;password=11rdo10rra;";
                           using (SqlConnection con = new SqlConnection(cs))
                           {
                               //--openConection--//
                               con.Open();
                               //-----StoreProcedure-----//
                               SqlCommand premanifiestoAlta = new SqlCommand("spEP_Premanifiesto_alta", con);
                               premanifiestoAlta.CommandType = System.Data.CommandType.StoredProcedure;
                               //-----inputParameters------//
                               //required
                               premanifiestoAlta.Parameters.AddWithValue("@usuario_SPI", pre.userSPI).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@cliente_id", pre.customerID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@equipo_id", pre.spiTeamID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@grupo_ID", pre.spiGroupID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@primer_nombre", pre.firstName).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@apellidos", pre.lastName).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@pais", pre.country).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@pais_id", pre.spiCountryID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@opc_id", pre.spiOpcID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@fecha_tour", pre.tourDate).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@sala_ventas_id", pre.salesRoomID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@hotel_id", pre.spiHotelID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@locacion_id", pre.spiLocationID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@idioma_id", pre.spiLanguageID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@categoria_id", pre.spiCategoryID).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@usuario_EPlat", pre.savedByUserID.ToString()).Direction = System.Data.ParameterDirection.Input;
                               premanifiestoAlta.Parameters.AddWithValue("@nombre_Eplat", userData.firstName+" "+userData.lastName).Direction = System.Data.ParameterDirection.Input;
                               //nullable
                               if(pre.secondName!= null)
                               {
                                    premanifiestoAlta.Parameters.AddWithValue("@segundo_nombre", pre.secondName).Direction = System.Data.ParameterDirection.Input;
                               }
                               else
                               {
                                    premanifiestoAlta.Parameters.AddWithValue("@segundo_nombre", DBNull.Value).Direction = System.Data.ParameterDirection.Input;
                               }
                               if(pre.spouseFirstName != null)
                               {
                                    premanifiestoAlta.Parameters.AddWithValue("@primer_nombre2", pre.spouseFirstName).Direction = System.Data.ParameterDirection.Input;
                               }
                               else
                               {
                                    premanifiestoAlta.Parameters.AddWithValue("@primer_nombre2", DBNull.Value).Direction = System.Data.ParameterDirection.Input;
                               }
                               if(pre.spouseSecondName != null)
                               {
                                     premanifiestoAlta.Parameters.AddWithValue("@segundo_nombre2",pre.spouseSecondName).Direction = System.Data.ParameterDirection.Input;
                               }
                               else
                               {
                                    premanifiestoAlta.Parameters.AddWithValue("@segundo_nombre2", DBNull.Value).Direction = System.Data.ParameterDirection.Input;
                               }
                               if(pre.spouseLastName != null)
                               {
                                     premanifiestoAlta.Parameters.AddWithValue("@apellidos2", pre.spouseLastName).Direction = System.Data.ParameterDirection.Input;
                               }
                               else
                               {
                                    premanifiestoAlta.Parameters.AddWithValue("@apellidos2", DBNull.Value).Direction = System.Data.ParameterDirection.Input;
                               }
                               if(pre.guestEmail != null)
                               {
                                     premanifiestoAlta.Parameters.AddWithValue("@email", pre.guestEmail).Direction = System.Data.ParameterDirection.Input;
                               }
                               else
                               {
                                    premanifiestoAlta.Parameters.AddWithValue("@email", DBNull.Value).Direction = System.Data.ParameterDirection.Input;
                               }
                               if(pre.invitationNumber != null)
                               {
                                     premanifiestoAlta.Parameters.AddWithValue("@invitacion", pre.invitationNumber).Direction = System.Data.ParameterDirection.Input;
                               }
                               else
                               {
                                    premanifiestoAlta.Parameters.AddWithValue("@invitacion", DBNull.Value).Direction = System.Data.ParameterDirection.Input;
                               }
                               //deposito
                               if(pre.depositAmount != null)
                               {
                                 premanifiestoAlta.Parameters.AddWithValue("@deposito", pre.depositAmount).Direction = System.Data.ParameterDirection.Input;
                               }
                               else
                               {
                                 premanifiestoAlta.Parameters.AddWithValue("@deposito", DBNull.Value).Direction = System.Data.ParameterDirection.Input;
                               }
                               // premanifiestoAlta.Parameters.AddWithValue("@invitacion", pre.invitationNumber).Direction = System.Data.ParameterDirection.Input;
                               //------OutPut------//
                               premanifiestoAlta.Parameters.Add("@tour_id", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                               //---returnValue----//
                               premanifiestoAlta.Parameters.Add("@cliente_id", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                               //---ExecuteStoredProcedure---//
                               premanifiestoAlta.ExecuteNonQuery();
                               //---getOutPutParameter And returnValue----/
                               pre.spiTourID = (int?)premanifiestoAlta.Parameters["@tour_id"].Value;
                               if (pre.customerID == null)//new guest
                                   pre.spiReferenceCustomerID = (int?)premanifiestoAlta.Parameters[24].Value;
                               pre.preManifested = true;
                               //--save and closeConection--//
                               db.SaveChanges();
                               con.Close();
                           }
                    }
                }
                catch(Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "There was a error trying premanifest the invitation";
                    response.Exception = ex;
                }
            }
            return response;
        }
        //Deposit
        public static decimal? GetInvitationDepositsTotal(Guid invitationID)
        {
            ePlatEntities db = new ePlatEntities();
            var query = from invitation in db.tblInvitations
                        join deposit in db.tblInvitationDeposits on invitation.invitationID equals deposit.invitationID
                        join terminal in db.tblTerminals on invitation.terminalID equals terminal.terminalID
                        where invitation.invitationID == invitationID && deposit.invitationID == invitationID && deposit.deleted != true
                        select new
                        {
                            amount = deposit.amount,
                            dateSaved = invitation.dateSaved,
                            terminalID = terminal.exchangeRateFromTerminalID.Value,
                            currencyID = deposit.currencyID
                        };
            decimal ? totalAmount = null;

            if(query.Count() > 0)
            {
                totalAmount = 0;
                foreach (var deposit in query)//get exchangerate from invitation saved
                {
                    DateTime dateSaved = DateTime.Parse(deposit.dateSaved.ToString("yyyy-MM-dd"));
                    switch (deposit.currencyID)
                    {
                        case 1://dolars USD
                            {
                                totalAmount += deposit.amount;
                                break;
                            }
                        case 2://Mexican peso MXN
                            {
                                totalAmount += deposit.amount/InvitationExchangeRate(deposit.terminalID, deposit.dateSaved.Date,deposit.currencyID);
                                break;
                            }
                        case 3://Canadian dollar USD
                            {
                                //CAD > MXN
                                var amountINMXN = deposit.amount * InvitationExchangeRate(deposit.terminalID, deposit.dateSaved.Date, deposit.currencyID);
                                var erUSD = InvitationExchangeRate(deposit.terminalID, deposit.dateSaved.Date, 2);//USD ER
                               // var toMXN = InvitationExchangeRate(deposit.terminalID, deposit.dateSaved.Date, deposit.currencyID);
                                totalAmount += (amountINMXN / erUSD);//CURRENCY 2 = MXN
                                break;
                            }
                    }    
                }
                totalAmount = totalAmount.HasValue ? System.Math.Round(totalAmount.Value, 2) : totalAmount;
            }
            return totalAmount;
        }
        public static decimal InvitationExchangeRate(long terminalID, DateTime dateSaved, int currencyID)
        {
            ePlatEntities db = new ePlatEntities();
            decimal exchangeRate = 0;
           // dateSaved = dateSaved.AddDays(1);

            switch (currencyID)
            {
                case 2://MNX > USD
                    {
                        var exchangeRates = from x in db.tblExchangeRates
                                            where // (x.fromDate >= dateSavedFrom && x.toDate <= dateSavedTo)
                                                  //(x.fromDate >= dateSaved && dateSaved <= x.toDate)
                                                      x.toDate >= dateSaved
                                                   && terminalID == x.terminalID.Value
                                                   && x.exchangeRateTypeID == 1
                                                   && x.fromCurrencyID == 1 //pesos 
                                                   && x.toCurrencyID == 2 // dolares
                                                   && x.tblExchangeRates_PointsOfSales.Count() == 0
                                            select x;
                        exchangeRate = exchangeRates.FirstOrDefault().exchangeRate;
                        break;
                    }
                case 3:// CAD > MXN
                    {
                        var exchangeRates = from x in db.tblExchangeRates
                                            where dateSaved >= x.fromDate
                                                  && (dateSaved <= x.toDate || x.toDate == null)
                                                  && terminalID == x.terminalID.Value
                                                  && x.exchangeRateTypeID == 1
                                                  && x.fromCurrencyID == currencyID
                                                  && x.toCurrencyID == 2
                                            select x;
                        exchangeRate = exchangeRates.FirstOrDefault().exchangeRate;
                        break;
                    }
            }
            
            return exchangeRate;
        }
        //EmailPreview
        public static string sendEmail(bool sendEmail = false, spiInvitation.invitationModelTable invitation = null, int? emailNotificationID = null)
        {
            if(invitation.guestEmail != null) {
                if (invitation.guestEmail != "")
                {
                    ePlatEntities db = new ePlatEntities();
                    SpiTvgEntities spi = new SpiTvgEntities();
                    UserSession session = new UserSession();
                    string response = "";
                    var currentTerminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
                    var spiCulture = spi.spEP_Idioma().FirstOrDefault(x => invitation.spiLanguageID == x.idioma_id).idioma == "Spanish" ? "es-MX" : "en-US";
                    var spiHotel = invitation.spiHotelID != 0 ? spi.spEP_Hotel().FirstOrDefault(x => x.hotel_id == invitation.spiHotelID).hotel : "";
                    var pickUpDate = new DateTime();
                    var pickUpTime = new DateTime();
                    var pickUpTypeTime = "";
                    var events = db.tblInvitationsSettings.FirstOrDefault(x => x.placeID == invitation.presentationPlaceID && x.programID == invitation.programID && x.confirmed == invitation.confirmed);
                    System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(events.eventID, currentTerminals.FirstOrDefault(), spiCulture);
                    if (email != null)
                    {
                        spiInvitation.pickUpHotelInfo info = spiInvitationCatalog.getHotelInfo(invitation.spiHotelID, spiCulture, invitation.pickUpTypeID);
                        email.To.Add(invitation.guestEmail);
                        email.Body = email.Body.Replace("$Guest", invitation.guest);
                        email.Body = email.Body.Replace("$Destination", spiInvitationCatalog.getDestination(invitation.presentationPlaceID.Value, spiCulture));
                        email.Body = email.Body.Replace("$Hotel", spiHotel);
                        if (invitation.confirmed && invitation.pickUpTime != null)
                        {
                            pickUpDate = DateTime.ParseExact(invitation.presentationDateTimeFormat, "yyyy-MM-dd", null);
                            pickUpTime = DateTime.Today.Add(TimeSpan.Parse(invitation.pickUpTime));
                            var amPM = pickUpTime.ToString("tt").ToUpper();
                            email.Body = email.Body.Replace("$PresentationDate", Utils.GeneralFunctions.DateFormat.ToText(pickUpDate, spiCulture));
                            email.Body = email.Body.Replace("$PresentationTime", pickUpTime.ToString("hh") + ":" + pickUpTime.ToString("mm") + " " + amPM.Replace(" ", ""));
                            email.Body = email.Body.Replace("$PickupTime", pickUpTime.ToString("hh") + ":" + pickUpTime.ToString("mm") + " " + amPM.Replace(" ", ""));
                            pickUpTypeTime = pickUpTime.ToString("hh") + ":" + pickUpTime.ToString("mm") + " " + amPM.Replace(" ", "");
                        }
                        var template = "";
                        var ubication = "";
                        var notes = "";
                        var des = "";
                        var mapLink = "";
                        //Track
                        // var track = InvitationTrackURL(Guid.Empty, "", spiCulture);
                        var track = InvitationTrackURL(invitation.invitationID, "", spiCulture);
                        template += "<table style=" + '"' + "width:100%;align-content:center" + '"' + "><tr>";
                        notes = invitation.pickUpNotes != null ? "<p>" + invitation.pickUpNotes + "</p>" : "";
                        des = info.description != null ? info.description : notes;

                        switch (invitation.pickUpTypeID)
                        {
                            case 1://PROMOTOR PICK UP.-opcion normal
                                {
                                    var uPickUpw = spiCulture == "es-MX" ? "Pasaremos por usted a:" : "Your pick up will be:";
                                    if (info != null)
                                    {
                                        if (info.lat != null && info.lng != null)
                                        {
                                            ubication = "https://maps.googleapis.com/maps/api/staticmap?center=" + info.lat + "," + info.lng + "&zoom=19&size=400x300&maptype=roadmap&markers=color:red%7Clabel:%7C" + info.lat + "," + info.lng + "&key=AIzaSyAXoJHEOgj8zop9tUl-mOrpneZKlfhHpBM";
                                            mapLink = "https://www.google.com/maps/search/?api=1%26query=" + info.lat + "," + info.lng;
                                        }
                                        if (info.picture == null)
                                        {
                                            if (ubication != "")
                                            {
                                                template += "<td width=" + '"' + "50%" + '"' + " style=" + '"' + "font-family: Verdana; font-size:12px;" + (ubication != "" ? " padding-left:0px; " : "padding-left:30px;") + " " + (ubication != "" ? "padding-right:0px;" : "padding-right:30px;") + '"' + ">";
                                                template += invitation.pickUpTypeID == 2 ? des.Replace("$pickUpTime", pickUpTypeTime) : (uPickUpw + "<br>" + spiHotel + "<br>" + des);
                                                //UBICACION
                                                template += ubication != "" ? "<td width=" + '"' + "50%" + '"' + ">" + "<a href=" + '"' + track + "&s=37&u=" + mapLink + '"' + "target=" + '"' + "_blank" + '"' + "><img style=" + '"' + "display:block;border:0;" + '"' + "src=" + '"' + ubication + '"' + " width=" + '"' + "300" + '"' + "height=" + '"' + "200" + '"' + "/></a></td><tr></table>" : "<tr></table>";
                                                //NOTAS
                                                template += info.description != null ? notes : "";
                                            }
                                            else
                                            {
                                                template += notes;
                                            }
                                        }
                                        else
                                        {
                                            //DESCRIPCION
                                            template += "<td width=" + '"' + (ubication != "" ? "33.3%" : "50%") + '"' + " style=" + '"' + "font-family: Verdana; font-size:12px;" + (ubication != "" ? " padding-left:0px; " : "padding-left:30px; ") + " " + (ubication != "" ? "padding-right:0px; " : "padding- right:30px; ") + '"' + ">" + uPickUpw + "<br>";
                                            template += spiHotel + "<br>" + des;
                                            //IMAGEN
                                            template += "<td width=" + '"' + (ubication != "" ? "33.3%" : "50%") + '"' + "><img style=" + '"' + "display:block;border:0;" + '"' + " src=" + '"' + "https://eplat.villagroup.com" + info.picture + "?w=400&h=300&mode=crop" + '"' + " width=" + '"' + (ubication != "" ? "200" : "300") + '"' + "height=" + '"' + (ubication != "" ? "150" : "200") + '"' + "/></td>";
                                            //UBICACION
                                            template += ubication != "" ? "<td width=" + '"' + "33.3%" + '"' + ">" + "<a href=" + '"' + track + "&s=37&u=" + mapLink + '"' + "target=" + '"' + "_blank" + '"' + "><img style=" + '"' + "display:block;border:0;" + '"' + "src=" + '"' + ubication + '"' + " width=" + '"' + "200" + '"' + "height=" + '"' + "150" + '"' + "/></a></td><tr></table>" : "<tr></table>";
                                            //NOTAS
                                            template += info.description != null ? notes : "";
                                        }
                                    }
                                    else
                                    {
                                        template += invitation.pickUpNotes != null ? "<p>" + invitation.pickUpNotes + "</p>" : "";
                                    }
                                    break;
                                }
                            case 2://Taxi voucher.-quitar imagen de hotel dejar la ubicacion 
                                {
                                    var voucherURL = invitation.presentationPlaceID == 27 ? "http://eplatfront.villagroup.com/images/invitations/vouchersVDP.jpg" : "http://eplatfront.villagroup.com/images/invitations/vouchersGB.jpg";
                                    var voucherDescription = spiCulture == "es-MX" ? "Por favor tome un taxi a las " + pickUpTypeTime + " usando su voucher de taxi." : "Please take the taxi at " + pickUpTypeTime + " using your taxi voucher.";
                                    template += "<td  style=" + '"' + "font-family: Verdana; font-size:12px;" + '"' + ">" + voucherDescription + "</td></tr>";
                                    template += "<tr><td></br><img width=" + '"' + "540" + '"' + "src=" + '"' + voucherURL + '"' + "/></td></tr></table>";
                                    template += invitation.pickUpNotes != null ? "<p>" + invitation.pickUpNotes + "</p>" : "";
                                    break;
                                }
                            case 3://
                                {
                                    var privateDescription = spiCulture == "es-MX" ? "<p>" + "El transporte privado llegará por usted al lobby del hotel a las " + pickUpTypeTime + "</p>" : "<p>The private transportation will pick you up at the hotel lobby at " + pickUpTypeTime + "</p>";
                                    template = privateDescription;
                                    template += invitation.pickUpNotes != null ? "<p>" + invitation.pickUpNotes + "</p>" : "";
                                    //change invitation.pickUpNotes for des
                                    break;
                                }
                        }
                        //
                        email.Body = email.Body.Replace("$PickUpInfo", template);
                        //email.Body = email.Body.Replace("$Deposit", "$" + invitation.depositAmount + " " + invitation.depositCurrencyCode);
                        email.Body = email.Body.Replace("$Gifts", invitation.gift == null ? "" : invitation.gift.Replace(System.Environment.NewLine, "<br />").Replace("\r", "<br />").Replace("\n", "<br />"));
                        email.Body = email.Body.Replace("$DepositDetails", InvitationDepositTable(spiCulture, invitation.invitationDeposits, invitation.invitationID));
                        email.Body = email.Body.Replace("$Deposit", "");
                        if (sendEmail == true)
                        {
                            Guid trackingID = Guid.NewGuid();
                            email.Body = email.Body.Replace("$Link", InvitationTrackingModel(trackingID, events.eventID, invitation.invitationID, spiCulture));
                            email.Body = email.Body.Replace("$About", InvitationTrackURL(trackingID, "about-us", spiCulture));
                            email.Body = email.Body.Replace("$Whatsapp", InvitationTrackURL(trackingID, "https://wa.me/", spiCulture, invitation));
                            //Utils.EmailNotifications.Send(email);
                            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                        }
                        else {
                            Guid trackingID = Guid.Empty;
                            email.Body = email.Body.Replace("$About", InvitationTrackURL(trackingID, "about-us", spiCulture));
                            email.Body = email.Body.Replace("$Whatsapp", InvitationTrackURL(trackingID, "https://wa.me/", spiCulture, invitation));
                            response = email.Body;
                        }
                    }
                    return response;
                }
                else
                {
                    return "<h3> Fill the form to get email preview </h3>";
                }
            }
            else
            {
                return "<h3> Fill the form to get email preview </h3>";
            }
        }
        public static void SaveEmailLog(int eventID, Guid invitationID, Guid trackingID, string culture)
        {
            ePlatEntities db = new ePlatEntities();
            tblEmailNotificationLogs log = new tblEmailNotificationLogs();
            var session = new UserSession();
            var currentTerminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToList();

            var emailNotificationID = (from n in db.tblEmailNotifications
                                       join e in db.tblEmails on n.emailID equals e.emailID
                                       where n.eventID == eventID && currentTerminals.Contains(n.terminalID.Value) && e.culture == culture
                                       select n.emailNotificationID).FirstOrDefault();

            var currentUserID = (Guid)Membership.GetUser().ProviderUserKey;
            log.dateSent = DateTime.Now;
            log.sentByUserID = currentUserID;
            log.invitationID = invitationID;
            log.emailNotificationID = emailNotificationID;
            log.trackingID = trackingID;
            db.tblEmailNotificationLogs.AddObject(log);
            db.SaveChanges();
        }
        static string InvitationTrackingModel(Guid trackingID, int eventID,Guid invitationID, string spiCulture)
        {
            var url = Debugger.IsAttached ? "http://localhost:45000" : "https://eplat.villagroup.com";
            var track = '"' + url + "/public/GetImage?i=" + trackingID.ToString() + '"';
            SaveEmailLog(eventID,invitationID, trackingID, spiCulture);
            return track;
        }
        static string InvitationTrackEmail(Guid trackingID)
        {
            var url = Debugger.IsAttached ? "http://localhost:45000" : "https://eplat.villagroup.com";
            var track = url + "/public/getinvitationinfo?i=" + trackingID;
            return track;
        }
        static string InvitationTrackURL(Guid trackingID, string url, string spiCulture, spiInvitation.invitationModelTable invitation = null)
        {
            var track = InvitationTrackEmail(trackingID);
            var response = "";
            if (url.Contains("https://wa.me/"))
            {
                var urlAppDetails = "";
                if(spiCulture == "es-MX")
                {
                    response += "También puedes enviarnos un mensaje a través de ";
                    urlAppDetails += "&s=37&u=https://wa.me/5219981856016?text=Hola%20mi%20nombre%20es%20"
                        + invitation.guest.Replace(" ", "%20") + "%20mi%20numero%20de%invitacion%20es%20" + invitation.invitationNumber + '"';
                }
                else
                {
                    response += "You can also send us a message through ";
                    urlAppDetails += "&s=37&u=https://wa.me/5219981856016?text=Hello%20my%20name%20is%20"
                        + invitation.guest.Replace(" ", "%20") + "%20my%20invitation%20number%20is%20" + invitation.invitationNumber + '"';
                }
               
                response += "<a href=" + '"' + track + urlAppDetails + "target=" + '"' + "_blank" + '"' + "><img " + "width=" + '"' + 18 + '"' + "height=" + '"' + 18 
                    + '"' + "src=" + '"' + "http://eplatfront.villagroup.com/images/invitations/whatsapp.png" + '"' + " />" + "Whatsapp" + "</a>";
            }
            else if(url.Contains("about-us"))
            {
               var aboutUS = "";
               if(spiCulture == "es-MX")
                {
                    aboutUS = "<a href=" + '"' + track + "&s=37&u=" + "https://www.taferresorts.com/about-us" + '"' + "target=" + '"' + "_blank" + '"' 
                            + "style=" + '"' + "font-size:8pt;" + '"' + "><strong>MÁS INFORMACIÓN SOBRE NOSOTROS</strong></a>";
                    response += aboutUS; 
                }
                else
                {
                    aboutUS = "<a href=" + '"' + track + "&s=37&u=" + "https://www.taferresorts.com/about-us" + '"' + "target=" + '"' + "_blank" + '"' + "style=" + '"' + "font-size:8pt;" + '"' + "><strong>READ MORE ABOUT US</strong></a>";
                    response += aboutUS;
                }
            }
            else
            {
                response = track;
            }

            return response;
        }
        static string InvitationDepositTable(string culture, List<spiInvitation.invitationDeposits> deposits = null, Guid? invitationID = null)
        {
            ePlatEntities db = new ePlatEntities();
            string template = "";
            if(deposits == null)
            {
                var paymentList = spiInvitationDataModel.spiInvitationCatalog.paymentTypeList(); ;
                var currencyList = spiInvitationDataModel.spiInvitationCatalog.currenciesList();
                deposits = (from item in db.tblInvitationDeposits
                            join c in db.tblCurrencies on item.currencyID equals c.currencyID
                            where item.invitationID == invitationID && item.deleted == false
                            select new spiInvitation.invitationDeposits()
                            {
                                amount = item.amount,
                                currencyID = item.currencyID,
                                currency = c.currencyCode,
                                paymentTypeID = item.paymentTypeID,
                                ccReferenceNumber = item.ccReferenceNumber,
                                received = item.received,
                                deleted = item.deleted
                            }).ToList();
               foreach(var item in deposits)
               {
                    item.paymentType = paymentList.FirstOrDefault(x => item.paymentTypeID == int.Parse(x.Value)).Text;
               }
            }

            if (deposits.Count() > 0)
            {
                template += culture == "en-US" ? "<p>Your payment details:</p>": "<p>Detalles de pagos:</p>";
                template += "<p><table" + " align=" + '"' + "center" + '"' + " bgcolor=" + '"' + "#FAFAFA" + '"' +
                            "style=" + '"' + "width:70%;align-content:center;font-family: Verdana; font-size:12px;" + '"' + ">";
                template += culture == "en-US" ? "<tr><th>Payment Type</th><th>Amount</th><th>Currency</th><th>Received</th></tr>"
                            : "<tr><th>Tipo de Pago</th><th>Cantidad</th><th>Moneda</th><th>Recibido</th></tr>";
                foreach (var deposit in deposits.Where(x => x.deleted == false).ToList())
                {
                    var received = culture == "en-US" ? deposit.received == true ? "yes" : "no" : deposit.received == true ? "si" : "no";

                    template += "<tr align=" + '"' + "center" + '"' + "><td>" +
                                (culture == "en-US" ?
                                (deposit.paymentType == "Cash" ? deposit.paymentType + "</td>" : deposit.paymentType + '(' + deposit.ccReferenceNumber + ')' + "</td>") :
                                (deposit.paymentType == "Cash" ? "Efectivo" + "</td>" : "Tarejeta de Credito" + '(' + deposit.ccReferenceNumber + ')' + "</td>"));
                    template += "<td>$" + deposit.amount + "</td>";
                    template += "<td>" + deposit.currency + "</td>";
                    template += "<td>" + received + "</td></tr>";
                }
                template += "</table></p>";
            }
            return template;
        }
        //log guardado
        public static void _SaveEmailLog(spiInvitation.invitationModelTable invitation)
        {
            ePlatEntities db = new ePlatEntities();
            tblEmailNotificationLogs log = new tblEmailNotificationLogs();
            var session = new UserSession();
            var currentTerminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToList();
            //get presentationPlace after that get terminal after that get event 

            //verificar esta informacion
            var culture = invitation.spiLanguageID == "es" ? "es-MX" : "en-US"; 
            var eventID = db.tblInvitationsSettings.Where(n => n.programID == invitation.programID && n.confirmed == invitation.confirmed && n.placeID == invitation.presentationPlaceID)
                                           .Select(x => x.eventID).ToList();

            var emailNotificationID = (from n in db.tblEmailNotifications
                                    join e in db.tblEmails on n.emailID equals e.emailID
                                    where eventID.Contains(n.eventID) && currentTerminals.Contains(n.terminalID.Value) && e.culture == culture
                                    select n.emailNotificationID).FirstOrDefault();

            var currentUserID = (Guid)Membership.GetUser().ProviderUserKey;
            log.dateSent = DateTime.Now;
            log.sentByUserID = currentUserID;
            log.invitationID = invitation.invitationID;
            log.emailNotificationID = emailNotificationID;
            db.tblEmailNotificationLogs.AddObject(log);
            db.SaveChanges();

        }
        public static DependantFields GetDependentFieldsFromspiInvitation()
        {
            ePlatEntities db = new ePlatEntities();
            SpiTvgEntities spiDB = new SpiTvgEntities();
            DependantFields df = new DependantFields();
            UserSession session = new UserSession();
            var currentTerminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
            var userSPI = db.tblUserProfiles.FirstOrDefault(x => x.userID == session.UserID);
            df.Fields = new List<DependantFields.DependantField>();

            List<spiInvitation.DestinationName> destinationNames = GetDestinationNamesByTerminals(currentTerminals);

            //programs
            var programs = MasterChartDataModel.LeadsCatalogs.FillDrpPrograms().Select(x => int.Parse(x.Value)).ToList();
            DependantFields.DependantField opc = new DependantFields.DependantField();
            opc.Field = "opcID";
            opc.ParentField = "programID";
            opc.Values = new List<DependantFields.FieldValue>();
            DependantFields.DependantField opcJalador = new DependantFields.DependantField();
            opcJalador.Field = "jaladorOpcID";
            opcJalador.ParentField = "programID";
            opcJalador.Values = new List<DependantFields.FieldValue>();
            foreach (var proID in programs)
            {
                var opcsList = spiInvitationCatalog.fillDrpOPCLegacyKey(proID);
                var jaladorList = spiInvitationCatalog.fillDrpJalador(proID);
                foreach(var opcs in opcsList.OrderBy(x => x.Text))
                {
                    DependantFields.FieldValue valOPC = new DependantFields.FieldValue();
                    valOPC.ParentValue = proID;
                    valOPC.Value = opcs.Value;
                    valOPC.Text = opcs.Text;
                    opc.Values.Add(valOPC);
                }
                foreach(var jalador in jaladorList)
                {
                    DependantFields.FieldValue valJalador = new DependantFields.FieldValue();
                    valJalador.ParentValue = proID;
                    valJalador.Value = jalador.Value;
                    valJalador.Text = jalador.Text;
                    opcJalador.Values.Add(valJalador);
                }
            }
            if (userSPI.SPIUserName != null)
            {                           
                DependantFields.FieldValue valDefault = new DependantFields.FieldValue();
                valDefault.ParentValue = null;
                valDefault.Value = "";
                valDefault.Text = "-Select One-";
                //team
                DependantFields.DependantField spiTeamID = new DependantFields.DependantField();
                spiTeamID.Field = "spiTeamID";
                spiTeamID.ParentField = "spiGroupID";
                //spiTeamID.GrandParentField = "Team";
                spiTeamID.Values = new List<DependantFields.FieldValue>();

                DependantFields.DependantField spiSearchTeamID = new DependantFields.DependantField();
                spiSearchTeamID.Field = "searchInvitationByTeam";
                spiSearchTeamID.ParentField = "searchInvitationGroups";
                spiSearchTeamID.Values = new List<DependantFields.FieldValue>();

                DependantFields.DependantField category = new DependantFields.DependantField();
                category.Field = "spiCategoryID";
                category.ParentField = "spiTeamID";
                category.Values = new List<DependantFields.FieldValue>();
                //  foreach (var team in spiDB.spEP_Equipo(userSPI.SPIUserName))
                var groups = spiInvitationCatalog.fillDrpGroupsSPI();
                foreach (var team in spiDB.spEP_Equipo(userSPI.SPIUserName))
                {
                    if (groups.Count(x => x.Value == team.grupo_id.ToString())>0)
                    {
                        DependantFields.FieldValue valTeam = new DependantFields.FieldValue();
                        valTeam.ParentValue = team.grupo_id;
                        valTeam.Value = team.equipo_id.ToString();
                        valTeam.Text = team.equipo;
                        spiTeamID.Values.Add(valTeam);
                        spiSearchTeamID.Values.Add(valTeam);

                        foreach (var item in spiDB.spEP_Categoria(team.equipo_id))
                        {
                            DependantFields.FieldValue categoria = new DependantFields.FieldValue();
                            categoria.ParentValue = team.equipo_id;
                            categoria.Value = item.categoria_id.ToString();
                            categoria.Text = item.categoria;
                            category.Values.Add(categoria);
                        }
                    }
                }
                spiSearchTeamID.Values.Insert(0, valDefault);
                spiTeamID.Values.Insert(0, valDefault);
                opc.Values.Insert(0, valDefault);
                opcJalador.Values.Insert(0, valDefault);
                df.Fields.Add(spiSearchTeamID);
                df.Fields.Add(spiTeamID);
                df.Fields.Add(opc);
                df.Fields.Add(opcJalador);
                df.Fields.Add(category);
            }
            return df;
        }

        public static List<spiInvitation.DestinationName> GetDestinationNamesByTerminals(long[] terminals)
        {
            ePlatEntities db = new ePlatEntities();

                        return (from ter in db.tblTerminals
                               join desTer in db.tblTerminals_Destinations on ter.terminalID equals desTer.terminalID
                               join des in db.tblDestinations on desTer.destinationID equals des.destinationID
                               where terminals.Contains(ter.terminalID)
                               select new spiInvitation.DestinationName()
                               { 
                                   terminalID = ter.terminalID,
                                   destinationID = des.destinationID,
                                   destination = des.destination.ToUpper()
                               }).ToList();
        }

        public class spiInvitationCatalog
        {
            public static spiInvitation.pickUpHotelInfo getHotelInfo(int? spiHotelID, string culture, int pickUpTypeID)
            {
                ePlatEntities db = new ePlatEntities();
                spiInvitation.pickUpHotelInfo info = new spiInvitation.pickUpHotelInfo();
                UserSession session = new UserSession();

                var currentTermianls = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
                var query = from item in db.tblSPIHotelsPickUps
                            where currentTermianls.Contains(item.terminalID) && spiHotelID == item.spiHotelID
                            select item;
                foreach (var item in query)
                {
                    info.hotelPickUpID = item.hotelPickUpID;
                    info.description = culture == "es-MX" ? item.descriptionEsMX : item.descriptionEnUS;
                    info.picture = item.picture;
                    info.lat = item.lat;
                    info.lng = item.lng;
                }
                /*   switch(pickUpTypeID)
                   {
                       case 1://PROMOTOR PICK UP 
                           {
                               foreach (var item in query)
                               {
                                   info.hotelPickUpID = item.hotelPickUpID;
                                   info.description = culture == "es-MX" ? item.descriptionEsMX : item.descriptionEnUS;
                                   info.picture = item.picture;
                                   info.lat = item.lat;
                                   info.lng = item.lng;
                               }
                               break;
                           };
                       case 2: // taxi
                           {
                               foreach (var item in query)
                               {
                                   info.hotelPickUpID = item.hotelPickUpID;
                                   info.description = culture == "es-MX" ? "Por favor tome un taxi a las $pickUpTime usando su voucher de taxi." : "Please take the taxi at $pickUpTime using your taxi voucher." ;
                                   info.lat = item.lat;
                                   info.lng = item.lng;
                               }
                               break;
                           }
                       case 3://LOBBY TRANSPORTATION PICK UP
                           {
                               foreach (var item in query)
                               {
                                   info.hotelPickUpID = item.hotelPickUpID;
                                   info.description = culture == "es-MX" ? "" : "The private transportation will pick you up at the hotel lobby at $pickUpTime";
                                   info.lat = item.lat;
                                   info.lng = item.lng;
                               }
                               break;
                           }

                  }*/
                return info;
            }

            public static string getDestination(long placeID,string culture, bool? description = null)
            {
                ePlatEntities db = new ePlatEntities();
                var query = from p in db.tblPlaces
                            join d in db.tblDestinations on p.destinationID equals d.destinationID
                            join dd in db.tblDestinationDescriptions on p.destinationID equals dd.destinationID
                            where p.placeID == placeID
                            select new
                            {
                                destination = d.destination,
                                description = dd.description,
                                culture = dd.culture
                            };
                if (query.Count() > 0)
                {
                    if (description != null && description.Value)
                    {
                        return query.FirstOrDefault(x => x.culture == culture).description;
                    }
                    else
                    {
                        return query.FirstOrDefault().destination;
                    }
                }
                else
                    return "";
               
            }           
            public static string GetSubordinatedUsers()
            {
                ePlatEntities db = new ePlatEntities();
                var currentUser = (Guid)Membership.GetUser().ProviderUserKey;

                var query = UserDataModel.GetUsersBySupervisor(currentUser, false, false, true);
                var list = string.Join(",", query.Select(m => m.Value).ToArray());
                return list;
            }
            public static List<SelectListItem> fillDrpPresentationPlaces()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                UserSession session = new UserSession();
                var currentTermianls = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
                var query = from pt in db.tblPlaces_Terminals
                            join p in db.tblPlaces on pt.placeID equals p.placeID
                            where currentTermianls.Contains(pt.terminalID) && p.placeTypeID == 1
                            select new
                            {
                                oldPlace = p.oldPlaceName,
                                place = p.place,
                                placeID = p.placeID
                            };
                foreach (var item in query)
                {
                    if (item.oldPlace == null)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = item.placeID.ToString(),
                            Text = item.place
                        });
                    }
                    else
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = item.placeID.ToString(),
                            Text = item.oldPlace
                        });
                    }
                }
                list.Insert(0, ListItems.Default());
                return list;
            }
            public static List<SelectListItem> fillDrpOPCLegacyKey(int programID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                UserSession session = new UserSession();
                var terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
                var query =  (from opc in db.tblOPCS
                             join promT in db.tblOPC_PromotionTeams on opc.opcID equals promT.opcID
                             join promo in db.tblPromotionTeams on promT.promotionTeamID equals promo.promotionTeamID
                             join com in db.tblCompanies on opc.companyID equals com.companyID
                             join tecom in db.tblTerminals_Companies on com.companyID equals tecom.companyID
                             where     promT.terminateDate == null  //vigente                                
                                  //  && com.companyTypeID == 2 //marketing Company
                                    && terminals.Contains(tecom.terminalID) //session terminals  
                                    && promo.programID != null //promo programID 
                                    && promo.programID == programID //&& promo.programID == 16//airport program
                                    && opc.legacyKey != null
                             select new 
                             {
                                opcID = opc.opcID,
                                opc = opc.opc,
                               // opcName = opc.firstName,
                                //opc.secondSurname,
                                //opcLastName = opc.lastName,
                                programID = promo.programID
                             }).Distinct(); 
               
                foreach(var opc in query)
                {
                        list.Add(new SelectListItem()
                        {
                            Text = opc.opc,
                            Value = opc.opcID.ToString() 
                        });
                }
                return list;
            }
            public static List<SelectListItem> fillDrpJalador(int programID) 
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                UserSession session = new UserSession();
                var terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
                var query = (from opc in db.tblOPCS
                             join promT in db.tblOPC_PromotionTeams on opc.opcID equals promT.opcID
                             join promo in db.tblPromotionTeams on promT.promotionTeamID equals promo.promotionTeamID
                             join com in db.tblCompanies on opc.companyID equals com.companyID
                             join tecom in db.tblTerminals_Companies on com.companyID equals tecom.companyID
                             where  promT.jobPositionID == 1039 && //Jalador
                                    terminals.Contains(tecom.terminalID) //session terminals
                                    && com.companyTypeID == 2// marketing Company
                                    && promT.terminateDate == null  //vigen
                                    && promo.programID == programID
                                    //&& promo.programID == 16 //airport
                             select opc).Distinct().OrderBy(x => x.firstName);

                foreach (var opc in query)
                {
                        list.Add(new SelectListItem()
                        {
                            Text = opc.firstName + " " + opc.secondSurname + " " + opc.lastName,
                            Value = opc.opcID.ToString()
                        });
                }
                list.Add(new SelectListItem()
                {
                    Text = "-Select One-",
                    Value = "0"
                });
                return list;
            }
            public static List<SelectListItem> fillDrpHotelSPI()
            {
                ePlatEntities db = new ePlatEntities();
                SpiTvgEntities spiDB = new SpiTvgEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                UserSession session = new UserSession();
                var terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();

                var query = from item in spiDB.spEP_Hotel()
                            join hotel in db.tblSPIHotelsPickUps on item.hotel_id equals hotel.spiHotelID
                            join td in db.tblTerminals_Destinations on hotel.destinationID equals td.destinationID
                            where terminals.Contains(td.terminalID)
                            select item;

                //foreach (var hotel in spiDB.spEP_Hotel().OrderBy(x => x.hotel))
                foreach (var hotel in query.OrderBy(x => x.hotel).ToList())
                {
                      list.Add(new SelectListItem()
                      {
                          Text = hotel.hotel,
                          Value = hotel.hotel_id.ToString()
                      });
                  }
                list.Insert(0, ListItems.Default());
                return list;
            }
            public static List<SelectListItem> fillDrpLocationSPI()
            {
                SpiTvgEntities spiDB = new SpiTvgEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var hotel in spiDB.spEP_Locacion().OrderBy(x => x.locacion))
                {
                    list.Add(new SelectListItem()
                    {
                        Text = hotel.locacion,
                        Value = hotel.locacion_id.ToString()
                    });
                }
                list.Insert(0, ListItems.Default(Selected: true));
                return list;
            }
            public static List<SelectListItem> fillDrpLanguageSPI()
            {
                SpiTvgEntities spiDB = new SpiTvgEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var lang in spiDB.spEP_Idioma().OrderBy(x => x.idioma))
                {
                    list.Add(new SelectListItem()
                    {
                        Text = lang.idioma,
                        Value = lang.idioma_id.ToString()
                    });
                }
                list.Insert(0, ListItems.Default());
                return list;
            }
            public static List<SelectListItem> fillDrpCountriesSPI()
            {
                SpiTvgEntities spiDB = new SpiTvgEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var cou in spiDB.spEP_Pais().OrderBy(x => x.pais))
                {
                    list.Add(new SelectListItem()
                    {
                        Text = cou.pais,
                        Value = cou.pais_id.ToString()
                    });
                }
                list.Insert(0, ListItems.Default());
                return list;
            }
            public static List<SelectListItem> fillDrpGroupsSPI()
            {
                SpiTvgEntities spiDB = new SpiTvgEntities();
                ePlatEntities db = new ePlatEntities();
                UserSession session = new UserSession();
                var currentUserID = (Guid)Membership.GetUser().ProviderUserKey;
                var userSPI = db.tblUserProfiles.FirstOrDefault(x => x.userID == currentUserID);  
                List<SelectListItem> list = new List<SelectListItem>();
                var groups = spiDB.spEP_Equipo(userSPI.SPIUserName).Distinct().ToList();
                var currentTerminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
                List<spiInvitation.DestinationName> destinationNames = GetDestinationNamesByTerminals(currentTerminals);
                foreach (var group in groups)
                {
                    
                    if (list.Count(x => x.Value == group.grupo_id.ToString()) == 0)
                    {
                        list.Add(new SelectListItem()
                        {
                            Text = group.grupo,
                            Value = group.grupo_id.ToString()
                        });
                    }
                    /*
                    if (list.Count(x => x.Value == group.grupo_id.ToString()) == 0)
                    {

                        if(group.grupo.IndexOf("-") != 1)
                        {
                           if(destinationNames.Count(x => group.grupo.IndexOf(x.destination)!=-1)==1)
                            {
                                list.Add(new SelectListItem()
                                {
                                    Text = group.grupo,
                                    Value = group.grupo_id.ToString()
                                });
                            }
                           if(group.grupo.Contains("Riviera") || group.grupo.Contains("riviera") || group.grupo.Contains("RIVIERA"))
                            {
                                list.Add(new SelectListItem()
                                {
                                    Text = group.grupo,
                                    Value = group.grupo_id.ToString()
                                });
                            }
                        }
                        else
                        {
                            list.Add(new SelectListItem()
                            {
                                Text = group.grupo,
                                Value = group.grupo_id.ToString()
                            });
                        }
                    }*/
                }
                list.Insert(0, ListItems.Default());
                return list;
            }
            public static List<SelectListItem> fillDrpOPCSPI()
            {
                SpiTvgEntities spiDB = new SpiTvgEntities();
                ePlatEntities db = new ePlatEntities();
                var currentUserID = (Guid)Membership.GetUser().ProviderUserKey;
                var userSPI = db.tblUserProfiles.FirstOrDefault(x => x.userID == currentUserID);               
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var hotel in spiDB.spEP_OPC(userSPI.SPIUserName).OrderBy(x => x.nombre_opc))
                {
                    list.Add(new SelectListItem()
                    {
                        Text = hotel.nombre_opc,
                        Value = hotel.opc_id.ToString()
                    });
                }
                list.Insert(0, ListItems.Default());
                return list;
            }
            public static List<SelectListItem> fillDrpSalesRoomSPI()
            {
                SpiTvgEntities spiDB = new SpiTvgEntities();
                ePlatEntities db = new ePlatEntities();
                var currentUserID = (Guid)Membership.GetUser().ProviderUserKey;
                var userSPI = db.tblUserProfiles.FirstOrDefault(x => x.userID == currentUserID);
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var hotel in spiDB.spEP_Sala_ventas(userSPI.SPIUserName).OrderBy(x => x.sala_ventas))
                {
                    list.Add(new SelectListItem()
                    {
                        Text = hotel.sala_ventas,
                        Value = hotel.sala_ventas_id.ToString()
                    });
                }
                list.Insert(0, ListItems.Default());
                return list;
            }
            public static List<SelectListItem> currenciesList()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var query = from c in db.tblCurrencies
                            select c;
                foreach (var c in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = c.currencyID.ToString(),
                        Text = c.currencyCode
                    });
                }
                return list;
            }
            public static List<SelectListItem> paymentTypeList()
            {
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var x in GeneralFunctions.PaymentTypes.Where(x => long.Parse(x.Key) < 3))//only cash and credit
                {
                    list.Add(new SelectListItem()
                    {
                        Value = x.Key,
                        Text = x.Value
                    });
                }
                return list;
            }
            public static List<SelectListItem> pickUpTypeList()
            {
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem()
                {
                    Value = "1",
                    Text = "Promotor Pick Up"
                });
                list.Add(new SelectListItem()
                {
                    Value = "2",
                    Text = "Taxi Voucher"
                });
                list.Add(new SelectListItem()
                {
                    Value = "3",
                    Text = "Private Transportation Pick Up"
                });
                return list;
            }
            public static List<SelectListItem> categoriesList(int spiTeamID)
            {
                SpiTvgEntities spiDB = new SpiTvgEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var category in spiDB.spEP_Categoria(spiTeamID))
                {
                   /* list.Add(new SelectListItem()
                    {
                        Text = category.categoria,
                        Value = category.categoria_id.ToString()
                    });
                    */
                    if(list.Count(x => x.Value == category.categoria_id.ToString())==0)
                    {
                        list.Add(new SelectListItem()
                        {
                            Text = category.categoria,
                            Value = category.categoria_id.ToString()
                        });
                    }

                }
                return list;
            }
        }
        public static AttemptResponse SavePayments(List<spiInvitation.invitationDeposits> model)
        {
            UserSession session = new UserSession();
            ePlatEntities db = new ePlatEntities();
            AttemptResponse attempt = new AttemptResponse();
            try
            {
                var currentUserID = session.UserID;
                foreach (var d in model)
                {
                    if (d.invitationDepositID == null)
                    {
                        tblInvitationDeposits addDeposits = new tblInvitationDeposits();
                        addDeposits.invitationID = d.invitationID.Value;
                        addDeposits.amount = d.amount;
                        addDeposits.currencyID = d.currencyID;
                        addDeposits.paymentTypeID = d.paymentTypeID;
                        addDeposits.ccReferenceNumber = d.ccReferenceNumber;
                        addDeposits.received = d.received;
                        addDeposits.dateSaved = d.dateSaved;
                        addDeposits.savedByUserID = currentUserID;
                        addDeposits.dateLastModification = d.dateLastModification;
                        if (d.dateLastModification.HasValue)
                        {
                            addDeposits.modifiedByUserID = currentUserID;
                            addDeposits.dateLastModification = d.dateLastModification;
                        }
                        if (d.deleted.Value)
                        {
                            addDeposits.deleted = d.deleted;
                            addDeposits.deletedByUserID = currentUserID;
                            addDeposits.deletedDate = d.deletedDateTime;
                        }
                        else
                        {
                            addDeposits.deleted = false;
                        }
                        db.tblInvitationDeposits.AddObject(addDeposits);
                    }
                    else
                    {
                        var deposit = db.tblInvitationDeposits.Single(x => x.invitationID == d.invitationID.Value && x.invitationDepositID == d.invitationDepositID.Value);
                        deposit.amount = d.amount;
                        deposit.currencyID = d.currencyID;
                        deposit.paymentTypeID = d.paymentTypeID;
                        deposit.ccReferenceNumber = d.ccReferenceNumber;
                        deposit.received = d.received;
                        deposit.dateSaved = d.dateSaved;
                        deposit.savedByUserID = d.savedByUserID.Value;
                        if (d.dateLastModification.HasValue)
                        {
                            if (deposit.dateLastModification != d.dateLastModification.Value)
                            {
                                deposit.modifiedByUserID = currentUserID;
                                deposit.dateLastModification = d.dateLastModification;
                            }
                        }
                        if (d.deleted.Value)
                        {
                            deposit.deleted = d.deleted;
                            deposit.deletedByUserID = currentUserID;
                            deposit.deletedDate = d.deletedDateTime;
                        }
                    }
                    db.SaveChanges();
                }
                attempt.Type = Attempt_ResponseTypes.Ok;
                attempt.Message = "Deposits Saved Success";
                attempt.ObjectID = "1";
            }catch(Exception ex)
            {
                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Error trying to save deposits";
                attempt.ObjectID = "0";
                attempt.Exception = ex;
            }
            return attempt;
        }
        public static AttemptResponse SendEmailData(Guid invitationID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            spiInvitation.invitationModelTable invitation = new spiInvitation.invitationModelTable();
            var model = db.tblInvitations.Single(x => x.invitationID == invitationID);
            if (model.guestEmail != null)
            {
                invitation.invitationID = model.invitationID;
                invitation.presentationDateTime = model.presentationDateTime;//
                invitation.presentationDateTimeFormat = model.presentationDateTime.Value.ToString("yyyy-MM-dd");
                invitation.firstName = model.firstName;
                invitation.secondName = model.secondName;
                invitation.lastName = model.lastName;
                invitation.spouseFirstName = model.spouseFirstName;
                invitation.spouseSecondName = model.spouseSecondName;
                invitation.spouseLastName = model.spouseLastName;
                invitation.pickUpTime = model.pickUpTime.ToString();
                invitation.guest = model.firstName + " " + model.lastName;//
                invitation.guestPhone = model.guestPhone;
                invitation.guestEmail = model.guestEmail;//
                invitation.invitationNumber = model.invitationNumber;//
                invitation.pickUpNotes = model.pickUpNotes;
                invitation.gift = model.gifts;     
                invitation.spiLanguageID = model.spiLanguageID;
                invitation.spiHotelID = model.spiHotelID;
                invitation.spiLocationID = model.spiLocationID;
                invitation.presentationPlaceID = model.presentationPlaceID;
                invitation.programID = model.programID;
                invitation.pickUpTypeID = model.pickUpTypeID;
                invitation.confirmed = model.depositPickUpID == 3 ? true : false;
                invitation.guest = model.firstName + " " + model.lastName;
                var resp = spiInvitationDataModel.sendEmail(true, invitation);//(sendEmail, model)

                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = invitation.invitationID;
                response.Message = "Send Email Success";
            }
            else
            {
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = invitation.invitationID;
                response.Message = "Information Saved Success";
            }
            return response;
        }

        public static SPIInvitationReport.invitationReportResult SearchInvitationsDeposits(SPIInvitationReport.searchInvitationModel model)
        {
            ePlatEntities db = new ePlatEntities();
            SpiTvgEntities spiDB = new SpiTvgEntities();

            db.CommandTimeout = int.MaxValue;
            UserSession session = new UserSession();
            var terminals = session.Terminals.Select(x => int.Parse(x.ToString())).ToList();
            string spiUser = db.tblUserProfiles.FirstOrDefault(x => x.userID == session.UserID).SPIUserName;
            SpiTvgEntities dbSPI = new SpiTvgEntities();
            SPIInvitationReport.invitationReportResult list = new SPIInvitationReport.invitationReportResult();
            List<SelectListItem> pickUpType = spiInvitationCatalog.pickUpTypeList();
            list.totalP = new List<SPIInvitationReport.TotalByPaymentType>();
            list.totalC = new List<SPIInvitationReport.TotalByCurrencies>();
            list.Result = new List<SPIInvitationReport.invitationsReportModel>();
            list.premanifestDate = DateTime.Now.ToString("dddd, dd # MMMM # yyyy").Replace("#","de");
            list.shift = model.shift.HasValue ? model.shift.Value == 0 ? "All":model.shiftList.FirstOrDefault(x => x.Value == model.shift.Value.ToString()).Text:"";
            list.premanifestUser = session.User;


            DateTime? FromDateSaved = model.fromDate.HasValue ? model.fromDate.Value.AddMinutes(-1) : (DateTime?)null;
            DateTime? ToDateSaved = model.toDate.HasValue ? model.toDate.Value.AddDays(1) : (DateTime?)null;

            var groups = spiDB.spEP_Equipo(spiUser).Distinct().OrderBy(x => x.grupo_id).ToList();


            list.Result = (from ho in dbSPI.spEP_Hotel()
                           join invitation in db.tblInvitations on ho.hotel_id equals invitation.spiHotelID
                           join opc in db.tblOPCS on invitation.opcID equals opc.opcID
                           join jal in db.tblOPCS on invitation.opcID equals jal.opcID
                           join pp in db.tblProspectationPrograms on invitation.programID equals pp.programID
                           join pl in db.tblPlaces on invitation.presentationPlaceID equals pl.placeID
                           join us in db.tblUserProfiles on invitation.savedByUserID equals us.userID
                           where
                                 //((model.fromDate == null && model.toDate == null) || (invitation.dateSaved >= model.fromDate && invitation.dateSaved <= model.toDate))
                                 (model.fromDate == null && model.toDate == null) || (invitation.dateSaved >= FromDateSaved && invitation.dateSaved <= ToDateSaved)
                               && (model.presentationPlaceID == 0 || model.presentationPlaceID == invitation.presentationPlaceID)
                               && (model.shift == 0 || model.shift == invitation.shiftID)
                               && (model.programID == 0 || model.programID == invitation.programID)

                           select new SPIInvitationReport.invitationsReportModel()
                           {
                               invitationID = invitation.invitationID,
                               presentationPlaceID = invitation.presentationPlaceID,
                               presentationPlace = pl.oldPlaceName != null ? pl.oldPlaceName : pl.place,
                               programID = invitation.programID,
                               groupID = invitation.spiGroupID,
                               teamID = invitation.spiTeamID,
                               program = pp.program,
                               invitationFolio = invitation.invitationNumber,
                               guest = invitation.guest,
                               spiHotelID = invitation.spiHotelID,
                               spiHotel = ho.hotel,
                               opcID = invitation.opcID,
                              // opc = opc.firstName + " " + opc.lastName,
                               opc = opc.opc,
                               pickUpTime = invitation.pickUpTime.HasValue ? (DateTime.Now + invitation.pickUpTime.Value).ToString("hh:mm tt") : "",
                               pax = invitation.adult + (invitation.child != 0 ? (invitation.child * .1) : 0),
                               pickUpTypeID = invitation.pickUpTypeID,
                               pickUpType = pickUpType.FirstOrDefault(x => x.Value == invitation.pickUpTypeID.ToString()).Text,
                               depositPickUpID = invitation.depositPickUpID.HasValue ? invitation.depositPickUpID.Value : 0,
                               comments = invitation.depositPickUpID == 1 ? "Open" : invitation.depositPickUpID == 2 ? "Dont Manifest" : invitation.presentationDateTime.Value.ToString("dddd. dd # MMMM # yyyy").Replace("#", "de"),
                               state = invitation.state,
                               jaladorID = invitation.jaladoresOpcID.HasValue ? invitation.jaladoresOpcID.Value : 0,
                               jalador = invitation.jaladoresOpcID.HasValue ? jal.firstName + " " + jal.lastName : "",
                               manifestFolio = invitation.manifestFolio.HasValue ? invitation.manifestFolio.Value : 0,
                               dateSaved = invitation.dateSaved.ToString("yyyy-MM-dd"),
                               dateModified = invitation.dateModified.HasValue ? invitation.dateModified.Value.ToString("yyyy-MM-dd") : "",
                               savedByUser = us.firstName + " " + us.lastName,
                               modifiedByUser = invitation.dateModified.HasValue ? db.tblUserProfiles.FirstOrDefault(x => x.userID == invitation.modifiedByUserID).firstName + " " + db.tblUserProfiles.FirstOrDefault(x => x.userID == invitation.modifiedByUserID).lastName : "",
                               //deposits = new List<spiInvitation.invitationDeposits>()
                               deposits =  (from dep in db.tblInvitationDeposits
                                           join cu in db.tblCurrencies on dep.currencyID equals cu.currencyID
                                           join su in db.tblUserProfiles on dep.savedByUserID equals su.userID
                                           where invitation.invitationID == dep.invitationID && dep.deleted == false 
                                           && db.tblInvitationDeposits.Count(x => invitation.invitationID == x.invitationID) > 0
                                           select new spiInvitation.invitationDeposits()
                                           {
                                               invitationDepositID = dep.invitationDepositID,
                                               invitationID = dep.invitationID,
                                               amount = dep.amount,
                                               currencyID = dep.currencyID,
                                               currency = cu.currencyCode,
                                               paymentTypeID = dep.paymentTypeID,
                                               paymentType = "",
                                               ccReferenceNumber = dep.ccReferenceNumber,
                                               received = dep.received,
                                               dateSaved = dep.dateSaved,
                                               savedByUserID = dep.savedByUserID,
                                               saveUser = su.firstName + " " + su.lastName,
                                               modifiedByUserID = dep.modifiedByUserID,
                                               dateLastModification = dep.dateLastModification.HasValue ? dep.dateLastModification.Value : (DateTime?)null,
                                               modifiedByUser = dep.modifiedByUserID.HasValue ?
                                                                db.tblUserProfiles.FirstOrDefault(x => x.userID == dep.modifiedByUserID).firstName + " " + db.tblUserProfiles.FirstOrDefault(x => x.userID == dep.modifiedByUserID).lastName
                                                                : "",
                                               /* deleted = dep.deleted
                                                deletedDateTime = dep.deletedDate,
                                                deletedByUserID = dep.deletedByUserID,
                                                deleteByUser = dep.deleted.HasValue ?
                                                               db.tblUserProfiles.FirstOrDefault(x => x.userID == dep.modifiedByUserID).firstName + " " + db.tblUserProfiles.FirstOrDefault(x => x.userID == dep.modifiedByUserID).lastName
                                                               : ""*/
                                           }).ToList(),
                           }).ToList();

            var invitations = from item in list.Result
                              where item.invitationID != null
                              select new
                              {
                                  deposits = item.deposits,
                                  
                              };


           foreach(var item in invitations)
           {
                foreach(var d in item.deposits)
                {
                    if (list.totalP.Count(x => d.paymentTypeID == x.paymentTypeID && d.currencyID == x.currencyID) == 0)
                    {
                        list.totalP.Add(new SPIInvitationReport.TotalByPaymentType()
                        {
                            paymentTypeID = d.paymentTypeID,
                            paymentType = d.paymentTypeID == 1 ? "Cash":"TDC",
                            amount = d.amount,
                            currencyID = d.currencyID,
                            currency = d.currency
                        });
                    }
                    else
                    {
                        list.totalP.FirstOrDefault(x => x.paymentTypeID == d.paymentTypeID && x.currencyID == d.currencyID).amount += d.amount;
                    }
                    if(list.totalC.Count(x => x.currencyID == d.currencyID)==0)
                    {
                        list.totalC.Add(new SPIInvitationReport.TotalByCurrencies() {
                            currencyID = d.currencyID,
                            currency = d.currency,
                            amount = d.amount
                        });
                    }
                    else
                    {
                        list.totalC.FirstOrDefault(x => d.currencyID == x.currencyID).amount += d.amount;
                    }
                }
            }

            foreach(var item in list.Result)
            {
                var newgroup = groups.FirstOrDefault(x => item.groupID == x.grupo_id);
                var newTeam = groups.FirstOrDefault(x => item.teamID == x.equipo_id);
                item.group = newgroup != null ? newgroup.grupo : "";
                item.team = newTeam != null ? newTeam.equipo : "";
            }

            return list;
        }
    }
}
