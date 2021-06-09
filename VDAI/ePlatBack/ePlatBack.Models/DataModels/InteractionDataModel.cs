using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using System.Globalization;
using ePlatBack.Models.Utils.Custom;

namespace ePlatBack.Models.DataModels
{
    public class InteractionDataModel
    {
        public static UserSession session = new UserSession();

        public static List<SelectListItem> GetAllInteractionTypes()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            var query = from it in db.tblTerminals_InteractionTypes
                        where terminals.Contains(it.terminalID)
                        select new { it.interactionTypeID, it.tblInteractionTypes.interactionType };

            if (query.Count() == 0)
            {
                var _query = from it in db.tblInteractionTypes
                            where it.interactionTypeID != 13
                            select new { it.interactionTypeID, it.interactionType };
                foreach (var i in _query)
                    list.Add(new SelectListItem() { Value = i.interactionTypeID.ToString(), Text = i.interactionType });
            }
            else
            {
                foreach (var i in query)
                    list.Add(new SelectListItem() { Value = i.interactionTypeID.ToString(), Text = i.interactionType });
            }

            return list;
        }

        public static List<SelectListItem> GetInterestLevels()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from il in db.tblInterestLevels
                        select new { il.interestLevelID, il.interestLevel };
            foreach (var i in query)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.interestLevelID.ToString(),
                    Text = i.interestLevel
                });
            }
            return list;
        }

        public List<InteractionResultsModel> GetInteractions(Guid leadID)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<InteractionResultsModel>();
            //var teamID = db.tblUsers_SysWorkGroups.FirstOrDefault(m => m.userID == session.UserID && m.sysWorkGroupID == session.WorkGroupID && m.roleID == session.RoleID).sysWorkGroupTeamID;
            var teamID = db.tblUsers_SysWorkGroups.FirstOrDefault(m => m.userID == session.UserID && m.sysWorkGroupTeamID != null).sysWorkGroupTeamID;
            var allowed = teamID != null && !GeneralFunctions.IsUserInRole("Administrator") ? db.tblTeamsRelationships.Where(m => m.sysWorkGroupTeamID == (int)teamID).Select(m => (int?)m.visibleSysWorkGroupTeamID).ToList() : new List<int?>();
            //allowed.Insert(0, teamID);

            //var query = db.tblInteractions.Where(m => m.leadID == leadID).OrderByDescending(m => m.dateSaved);
            var query = (from interaction in db.tblInteractions
                        join user in db.aspnet_Users on interaction.savedByUserID equals user.UserId
                        join profile in db.tblUserProfiles on user.UserId equals profile.userID
                        join sw in db.tblUsers_SysWorkGroups on user.UserId equals sw.userID
                        where interaction.leadID == leadID
                        //&& (teamID == null || (sw.sysWorkGroupTeamID == teamID | allowed.Contains(sw.sysWorkGroupTeamID)))
                        && (allowed.Count() == 0 || (sw.sysWorkGroupTeamID == teamID | allowed.Contains(sw.sysWorkGroupTeamID)))
                         select new
                        {
                            interaction.interactionID,
                            interaction.leadID,
                            interaction.bookingStatusID,
                            interaction.interactionTypeID,
                            interaction.interactionComments,
                            interaction.savedByUserID,
                            userName = profile.firstName + " " + profile.lastName,
                            interaction.dateSaved,
                            interaction.interactedWithUserID,
                            interaction.parentInteractionID,
                            interaction.totalSold
                        }).Distinct().ToList();

            foreach (var i in query.OrderByDescending(m => m.dateSaved))
            {

                list.Add(new InteractionResultsModel()
                {
                    InteractionsInfo_InteractionID = i.interactionID.ToString(),
                    InteractionsInfo_LeadID = i.leadID.ToString(),
                    InteractionsInfo_BookingStatus = i.bookingStatusID.ToString(),
                    InteractionsInfo_InteractionType = i.interactionTypeID.ToString(),
                    InteractionsInfo_InteractionComments = i.interactionComments,
                    InteractionsInfo_SavedByUser = i.savedByUserID.ToString(),
                    InteractionsInfo_SavedByUserName = i.userName,
                    InteractionsInfo_DateSaved = i.dateSaved.ToString("F", CultureInfo.InvariantCulture),
                    InteractionsInfo_InteractedWithUser = i.interactedWithUserID.ToString(),
                    InteractionsInfo_ParentInteraction = i.parentInteractionID.ToString(),
                    InteractionsInfo_TotalSold = i.totalSold != null ? i.totalSold.ToString() : ""
                });
            }
            
            return list;
        }

        public AttemptResponse SaveInteraction(PreArrivalInteractionsInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var isAllowed = GeneralFunctions.IsUserInRole("Administrator") || GeneralFunctions.IsUserInRole("Operation Manager") || GeneralFunctions.IsUserInRole("Department Administrator");
            #region "save"
            try
            {
                var query = db.tblLeads.Single(m => m.leadID == model.InteractionsInfo_LeadID);
                var status = GetOptionsStatus();
                if (int.Parse(status.FirstOrDefault(m => int.Parse(m.Value) == model.InteractionsInfo_BookingStatus).Text) < int.Parse(status.FirstOrDefault(m => int.Parse(m.Value) == query.bookingStatusID).Text))
                {
                    if (!isAllowed)
                    {
                        response.Type = Attempt_ResponseTypes.Warning;
                        response.Message = "Interaction Not Saved<br />Status forbidden";
                        response.ObjectID = 0;
                        return response;
                    }
                }
                var interaction = new tblInteractions();
                interaction.bookingStatusID = model.InteractionsInfo_BookingStatus;
                interaction.interactionTypeID = model.InteractionsInfo_InteractionType;
                interaction.interactionComments = model.InteractionsInfo_InteractionComments;
                interaction.savedByUserID = model.InteractionsInfo_SavedByUser ?? session.UserID;//new change
                interaction.dateSaved = DateTime.Now;
                interaction.interactedWithUserID = model.InteractionsInfo_InteractedWithUser;
                interaction.parentInteractionID = model.InteractionsInfo_ParentInteraction;
                interaction.totalSold = model.InteractionsInfo_TotalSold != null && model.InteractionsInfo_TotalSold != "" ? decimal.Parse(model.InteractionsInfo_TotalSold) : (decimal?)null;
                query.tblInteractions.Add(interaction);
                query.bookingStatusID = model.InteractionsInfo_ParentInteraction == null || model.InteractionsInfo_ParentInteraction == 0 ? model.InteractionsInfo_BookingStatus : query.bookingStatusID;
                
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Interaction Saved";
                response.ObjectID = new { itemID = interaction.interactionID, dateSaved = interaction.dateSaved.ToString("F", CultureInfo.InvariantCulture) };
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Interaction NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
            #endregion
        }

        public PreArrivalInteractionsInfoModel GetInteraction(long interactionID)
        {
            ePlatEntities db = new ePlatEntities();
            var model = new PreArrivalInteractionsInfoModel();

            var query = db.tblInteractions.Single(m => m.interactionID == interactionID);

            model.InteractionsInfo_LeadID = (Guid)query.leadID;
            model.InteractionsInfo_InteractionType = query.interactionTypeID;
            model.InteractionsInfo_BookingStatus = query.bookingStatusID;
            model.InteractionsInfo_ParentInteraction = interactionID;

            return model;
        }

        public static List<InteractionsViewModel.Interaction> GetInteractionsList(Guid? leadID = null, Guid? trialID = null)
        {
            ePlatEntities db = new ePlatEntities();
            List<InteractionsViewModel.Interaction> Interactions = new List<InteractionsViewModel.Interaction>();
            List<InteractionsViewModel.Interaction> InteractionsQuery = new List<InteractionsViewModel.Interaction>();
                
            if(leadID != null){
                InteractionsQuery = (from i in db.tblInteractions
                            join bs in db.tblBookingStatus on i.bookingStatusID equals bs.bookingStatusID
                            into i_bs
                            from bs in i_bs.DefaultIfEmpty()
                            join it in db.tblInteractionTypes on i.interactionTypeID equals it.interactionTypeID
                            into i_it
                            from it in i_it.DefaultIfEmpty()
                            join il in db.tblInterestLevels on i.interestLevelID equals il.interestLevelID
                            into i_il
                            from il in i_il.DefaultIfEmpty()
                            join iu in db.tblUserProfiles on i.interactedWithUserID equals iu.userID
                            into i_iu
                            from iu in i_iu.DefaultIfEmpty()
                            join su in db.tblUserProfiles on i.savedByUserID equals su.userID
                            into i_su
                            from su in i_su.DefaultIfEmpty()
                            where i.leadID == leadID
                            orderby i.dateSaved descending
                            select new InteractionsViewModel.Interaction()
                           {
                               InteractionID = i.interactionID,
                               InteractionLeadID = i.leadID,
                               InteractionTrialID = i.trialID,
                               InteractionBookingStatusID = i.bookingStatusID,
                               InteractionBookingStatus = bs.bookingStatus,
                               InteractionTypeID = i.interactionTypeID,
                               InteractionType = it.interactionType,
                               InteractionComments = i.interactionComments,
                               InteractionInterestLevelID = i.interestLevelID,
                               InteractionInterestLevel = il.interestLevel,
                               InteractedWithUserID = i.interactedWithUserID,
                               InteractedWithUser = iu.firstName  + " " + iu.lastName,
                               ParentInteractionID = i.parentInteractionID,
                               TotalSold = i.totalSold != null ? i.totalSold : null,
                               InteractionDate = i.dateSaved,
                               InteractionSavedByUser = su.firstName + " " + su.lastName,
                               Editing = false,
                               ShowNotes = false
                           }).ToList();
            } else if (trialID != null) {
                InteractionsQuery = (from i in db.tblInteractions
                            join bs in db.tblBookingStatus on i.bookingStatusID equals bs.bookingStatusID
                            into i_bs
                            from bs in i_bs.DefaultIfEmpty()
                            join it in db.tblInteractionTypes on i.interactionTypeID equals it.interactionTypeID
                            into i_it
                            from it in i_it.DefaultIfEmpty()
                            join il in db.tblInterestLevels on i.interestLevelID equals il.interestLevelID
                            into i_il
                            from il in i_il.DefaultIfEmpty()
                            join iu in db.tblUserProfiles on i.interactedWithUserID equals iu.userID
                            into i_iu
                            from iu in i_iu.DefaultIfEmpty()
                            where i.trialID == trialID
                            orderby i.dateSaved descending
                            select new InteractionsViewModel.Interaction()
                           {
                               InteractionID = i.interactionID,
                               InteractionBookingStatusID = i.bookingStatusID,
                               InteractionBookingStatus = bs.bookingStatus,
                               InteractionTypeID = i.interactionTypeID,
                               InteractionType = it.interactionType,
                               InteractionComments = i.interactionComments,
                               InteractionInterestLevelID = i.interestLevelID,
                               InteractionInterestLevel = il.interestLevel,
                               InteractedWithUserID = i.interactedWithUserID,
                               InteractedWithUser = iu.firstName  + " " + iu.lastName,
                               ParentInteractionID = i.parentInteractionID,
                               TotalSold = i.totalSold != null ? i.totalSold : null,
                               InteractionDate = i.dateSaved,
                               Editing = false,
                               ShowNotes = false
                           }).ToList();
            }

            foreach (var interaction in InteractionsQuery.Where(x => x.ParentInteractionID == null))
            {
                interaction.InteractionNotes = InteractionsQuery.Where(x => x.ParentInteractionID == interaction.InteractionID).OrderBy(x => x.InteractionDate).ToList();
                Interactions.Add(interaction);
            }

            return Interactions;
        }

        public static AttemptResponse SaveInteractionInfo(InteractionsViewModel.Interaction model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            //guardar nuevo o actualizar interacción
            tblInteractions interaction = new tblInteractions();
            if (model.InteractionID != null) {
                interaction = (from i in db.tblInteractions
                              where i.interactionID == model.InteractionID
                              select i).FirstOrDefault();
            }
            if (model.InteractionID == null)
            {
                //nuevo
                interaction.leadID = model.InteractionLeadID;
                interaction.trialID = model.InteractionTrialID;
                interaction.savedByUserID = session.UserID;
                interaction.dateSaved = DateTime.Now;
                interaction.parentInteractionID = model.ParentInteractionID;
            }
            //nuevo o actualizar
            interaction.bookingStatusID = model.InteractionBookingStatusID;
            interaction.interactionTypeID = model.InteractionTypeID;
            interaction.interactionComments = model.InteractionComments;
            interaction.interestLevelID = model.InteractionInterestLevelID;
            if (model.InteractedWithUserID != null)
            {
                interaction.interactedWithUserID = model.InteractedWithUserID;
            }
            else
            {
                interaction.interactedWithUserID = session.UserID;
            }            

            if (model.InteractionID == null)
            {
                db.tblInteractions.AddObject(interaction);
            }

            //actualizar lead o trial con el booking status
            if (interaction.leadID != null)
            {
                //actualizar lead
                db.tblLeads.Single(x => x.leadID == interaction.leadID).bookingStatusID = interaction.bookingStatusID;
            }
            if (interaction.trialID != null)
            {
                //actualizar trial
                db.tblTrials.Single(x => x.trialID == interaction.trialID).bookingStatusID = interaction.bookingStatusID;
            }

            try
            {
                db.SaveChanges();

                //obtener objeto de interacción
                InteractionsViewModel.Interaction newInteraction = (from i in db.tblInteractions
                                     join bs in db.tblBookingStatus on i.bookingStatusID equals bs.bookingStatusID
                                     into i_bs
                                     from bs in i_bs.DefaultIfEmpty()
                                     join it in db.tblInteractionTypes on i.interactionTypeID equals it.interactionTypeID
                                     into i_it
                                     from it in i_it.DefaultIfEmpty()
                                     join il in db.tblInterestLevels on i.interestLevelID equals il.interestLevelID
                                     into i_il
                                     from il in i_il.DefaultIfEmpty()
                                     join iu in db.tblUserProfiles on i.interactedWithUserID equals iu.userID
                                     into i_iu
                                     from iu in i_iu.DefaultIfEmpty()
                                     join su in db.tblUserProfiles on i.savedByUserID equals su.userID
                                     into i_su
                                     from su in i_su.DefaultIfEmpty()
                                     where i.interactionID == interaction.interactionID
                                     orderby i.dateSaved descending
                                     select new InteractionsViewModel.Interaction()
                                     {
                                         InteractionID = i.interactionID,
                                         InteractionLeadID = i.leadID,
                                         InteractionTrialID = i.trialID,
                                         InteractionBookingStatusID = i.bookingStatusID,
                                         InteractionBookingStatus = bs.bookingStatus,
                                         InteractionTypeID = i.interactionTypeID,
                                         InteractionType = it.interactionType,
                                         InteractionComments = i.interactionComments,
                                         InteractionInterestLevelID = i.interestLevelID,
                                         InteractionInterestLevel = il.interestLevel,
                                         InteractedWithUserID = i.interactedWithUserID,
                                         InteractedWithUser = iu.firstName + " " + iu.lastName,
                                         ParentInteractionID = i.parentInteractionID,
                                         InteractionDate = i.dateSaved,
                                         InteractionSavedByUser = su.firstName + " " + su.lastName,
                                         Editing = false,
                                         ShowNotes = false
                                     }).FirstOrDefault();

                if (newInteraction != null)
                {
                    var notes = (from i in db.tblInteractions
                                      join bs in db.tblBookingStatus on i.bookingStatusID equals bs.bookingStatusID
                                      into i_bs
                                      from bs in i_bs.DefaultIfEmpty()
                                      join it in db.tblInteractionTypes on i.interactionTypeID equals it.interactionTypeID
                                      into i_it
                                      from it in i_it.DefaultIfEmpty()
                                      join il in db.tblInterestLevels on i.interestLevelID equals il.interestLevelID
                                      into i_il
                                      from il in i_il.DefaultIfEmpty()
                                      join iu in db.tblUserProfiles on i.interactedWithUserID equals iu.userID
                                      into i_iu
                                      from iu in i_iu.DefaultIfEmpty()
                                      join su in db.tblUserProfiles on i.savedByUserID equals su.userID
                                      into i_su
                                      from su in i_su.DefaultIfEmpty()
                                      where i.parentInteractionID == interaction.interactionID
                                      orderby i.dateSaved descending
                                      select new InteractionsViewModel.Interaction()
                                      {
                                          InteractionID = i.interactionID,
                                          InteractionLeadID = i.leadID,
                                          InteractionTrialID = i.trialID,
                                          InteractionBookingStatusID = i.bookingStatusID,
                                          InteractionBookingStatus = bs.bookingStatus,
                                          InteractionTypeID = i.interactionTypeID,
                                          InteractionType = it.interactionType,
                                          InteractionComments = i.interactionComments,
                                          InteractionInterestLevelID = i.interestLevelID,
                                          InteractionInterestLevel = il.interestLevel,
                                          InteractedWithUserID = i.interactedWithUserID,
                                          InteractedWithUser = iu.firstName + " " + iu.lastName,
                                          ParentInteractionID = i.parentInteractionID,
                                          InteractionDate = i.dateSaved,
                                          InteractionSavedByUser = su.firstName + " " + su.lastName,
                                          Editing = false,
                                          ShowNotes = false
                                      }).ToList();
                    newInteraction.InteractionNotes = notes;
                }

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Interaction Saved";
                response.ObjectID = newInteraction;                
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Interaction NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
            }
            return response;
        }

        public static AttemptResponse DeleteInteraction(long interactionID)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            db.tblInteractions.DeleteObject(db.tblInteractions.Single(x => x.interactionID == interactionID));
            try
            {
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Interaction Deleted";
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Interaction NOT Deleted";
                response.Exception = ex;
            }
            return response;
        }

        public static List<SelectListItem> GetOptionsStatus()
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            var query = from bs in db.tblTerminals_BookingStatus
                        where terminals.Contains(bs.terminalID) && bs.pickingOrder != null
                        select bs;

            foreach(var i in query)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.bookingStatusID.ToString(),
                    Text = i.pickingOrder.ToString()
                }) ;
            }
            return list;
        }
    }
}
