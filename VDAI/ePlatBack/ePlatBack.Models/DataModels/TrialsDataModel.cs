using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Text.RegularExpressions;

namespace ePlatBack.Models.DataModels
{
    public class TrialsDataModel
    {
        public static UserSession session = new UserSession();
        public static List<TrialsViewModel.Trial> SearchTrials(TrialsViewModel.SearchTrial model)
        {
            List<TrialsViewModel.Trial> Trials = new List<TrialsViewModel.Trial>();
            
            int bookingStatusLength = 0;
            int?[] bookingStatusArr = new int?[] { };
            if (model.Search_BookingStatusID != null)
            {
                bookingStatusArr = model.Search_BookingStatusID;
                bookingStatusLength = model.Search_BookingStatusID.Length;
            }

            int assignedLength = 0;
            Guid?[] assignedToArr = new Guid?[] { };
            if (model.Search_AssignedToUserID != null)
            {
                assignedToArr = model.Search_AssignedToUserID;
                assignedLength = model.Search_AssignedToUserID.Length;
            }
            else {
                assignedLength = 1;
                assignedToArr[0] = session.UserID;
            }

            if (model.Search_ContractNumber != null && model.Search_ContractNumber.Trim() == "")
            {
                model.Search_ContractNumber = null;
            }

            ePlatEntities db = new ePlatEntities();
            Trials = (from t in db.tblTrials
                      join profile in db.tblUserProfiles on t.assignedToUserID equals profile.userID
                      into t_profile
                      from profile in t_profile.DefaultIfEmpty()
                      join bs in db.tblBookingStatus on t.bookingStatusID equals bs.bookingStatusID
                      into t_bs
                      from bs in t_bs.DefaultIfEmpty()
                      where (model.Search_ContractNumber == null || model.Search_ContractNumber == t.contractNumber)
                      && (bookingStatusLength == 0 || bookingStatusArr.Contains(t.bookingStatusID))
                      && (assignedLength == 0 || assignedToArr.Contains(t.assignedToUserID))
                      select new TrialsViewModel.Trial()
                      {
                          TrialID = t.trialID,
                          ContractNumber = t.contractNumber,
                          Reference = t.reference,
                          LastName = t.lastName,
                          SalesType = t.salesType,
                          ContractStatus = t.contractStatus,
                          SourceOfSale = t.sourceOfSale,
                          DateInput = t.dateInput,
                          DateSale = t.dateSale,
                          Volume = t.volume,
                          DateExpiration = t.dateExpiration,
                          AssignedToUserID = t.assignedToUserID,
                          AssignedTo = profile.firstName + " " + profile.lastName,
                          Phone1 = t.phone1,
                          Phone2 = t.phone2,
                          Email1 = t.email1,
                          Email2 = t.email2,
                          BookingStatusID = t.bookingStatusID,
                          BookingStatus = bs.bookingStatus,
                          _cellVariants = new {
                              DateExpiration = (t.dateExpiration < DateTime.Today ? "danger" : "")
                          }
                      }).ToList();

            return Trials;
        }

        public static AttemptResponse SaveTrial(TrialsViewModel.Trial model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            var TrialQ = (from t in db.tblTrials
                         where t.trialID == model.TrialID
                         select t).FirstOrDefault();

            if (TrialQ != null)
            {
                if (model.AssignedToUserID != null)
                {
                    TrialQ.assignedToUserID = model.AssignedToUserID;
                }
                TrialQ.bookingStatusID = model.BookingStatusID;
                TrialQ.phone1 = model.Phone1;
                TrialQ.phone2 = model.Phone2;
                TrialQ.email1 = model.Email1;
                TrialQ.email2 = model.Email2;
            }

            try
            {
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Trial Saved";
                response.ObjectID = 0;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Trial not Saved";
                response.ObjectID = 0;
                response.Exception = ex;
            }

            return response;
        }

        public static AttemptResponse SaveImport(string trials, bool unassigned)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];

            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<TrialsViewModel.Trial> Trials = js.Deserialize<List<TrialsViewModel.Trial>>(trials);

            List<string> contractNumbers = Trials.Select(x => x.ContractNumber).ToList();
            var trialVerification = from t in db.tblTrials
                                    where contractNumbers.Contains(t.contractNumber)
                                    && t.terminalID == terminalID
                                    select t;

            foreach (var trial in Trials)
            {
                if (trial.AssignedToUserID != null || unassigned)
                {
                    //verificar que no exista en la base para importar
                    if (trialVerification.Count(t => t.contractNumber == trial.ContractNumber && t.dateInput == trial.DateInput) == 0)
                    {
                        tblTrials newTrial = new tblTrials();
                        newTrial.trialID = Guid.NewGuid();
                        newTrial.contractNumber = trial.ContractNumber;
                        newTrial.reference = trial.Reference;
                        newTrial.lastName = trial.LastName;
                        newTrial.salesType = trial.SalesType;
                        newTrial.contractStatus = trial.ContractStatus;
                        newTrial.sourceOfSale = trial.SourceOfSale;
                        newTrial.dateInput = trial.DateInput;
                        newTrial.dateSale = trial.DateSale;
                        newTrial.volume = trial.Volume;
                        newTrial.dateExpiration = trial.DateExpiration;
                        newTrial.assignedToUserID = trial.AssignedToUserID;
                        newTrial.dateSaved = DateTime.Now;
                        newTrial.savedByUserID = session.UserID;
                        newTrial.terminalID = terminalID;
                        newTrial.bookingStatusID = 10;
                        db.tblTrials.AddObject(newTrial);
                    }
                    else
                    {
                        trialVerification.FirstOrDefault(t => t.contractNumber == trial.ContractNumber && t.dateInput == trial.DateInput).contractStatus = trial.ContractStatus;
                    }
                }
            }

            try
            {
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Trials Imported";
                response.ObjectID = 0;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Trials not Imported";
                response.ObjectID = 0;
                response.Exception = ex;
            }

            return response;
        }

        public static List<SelectListItem> GetAgents()
        {
            List<SelectListItem> Agents = new List<SelectListItem>();
            ePlatEntities db = new ePlatEntities();

            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];

            if (!GeneralFunctions.IsUserInRole("agent", session.UserID, true, db))
            {
                //obtener usuarios de terminal con rol de agente, supervisor o manager
                var UsersQ = from user in db.tblUsers_Terminals
                             join profile in db.tblUserProfiles on user.userID equals profile.userID
                             join role in db.tblUsers_SysWorkGroups on user.userID equals role.userID
                             join roleInfo in db.aspnet_Roles on role.roleID equals roleInfo.RoleId
                             where user.terminalID == terminalID
                             && (roleInfo.RoleName.Contains("agent") || roleInfo.RoleName.Contains("supervisor") || roleInfo.RoleName.Contains("manager"))
                             orderby profile.firstName
                             select new
                             {
                                 user.userID,
                                 profile.firstName,
                                 profile.lastName
                             };

                foreach (var user in UsersQ)
                {
                    SelectListItem newAgent = new SelectListItem();
                    newAgent.Value = user.userID.ToString();
                    newAgent.Text = user.firstName + " " + user.lastName;
                    if (user.userID == session.UserID)
                    {
                        newAgent.Selected = true;
                    }
                    Agents.Add(newAgent);
                }
            }

            //revisar si en la lista está el usuario logueado, sino agregarlo
            if (Agents.Count(x => x.Value == session.UserID.ToString()) == 0)
            {
                tblUserProfiles us = db.tblUserProfiles.Single(x => x.userID == session.UserID);
                SelectListItem newAgent = new SelectListItem();
                newAgent.Value = session.UserID.ToString();
                newAgent.Text = us.firstName + " " + us.lastName;
                newAgent.Selected = true;
                Agents.Add(newAgent);
            }

            return Agents;
        }
    }
}
