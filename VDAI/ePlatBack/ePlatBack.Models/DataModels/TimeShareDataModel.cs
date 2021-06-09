using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using ePlatBack.Models.Utils.Custom;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;


namespace ePlatBack.Models.DataModels
{
    public class TimeShareDataModel
    {
        public static UserSession session = new UserSession();
        public static string FundsBalance()
        {
            ePlatEntities db = new ePlatEntities();
            var funds = MasterChartDataModel.LeadsCatalogs.FillDrpFundsPerSelectedTerminals();
            var fundsString = "";

            var _funds = funds.GroupBy(m => m.Text);
            foreach (var _fund in _funds)
            {
                fundsString += (fundsString != "" ? "|" : "") + _fund.FirstOrDefault().Text + "_";
                foreach (var a in _fund)
                {
                    var fundID = int.Parse(a.Value);
                    var fund = db.tblFunds.Single(m => m.fundID == fundID);
                    fundsString += fund.amount + " " + fund.tblCurrencies.currencyCode + ",";
                }
                fundsString = fundsString.TrimEnd(',');
            }

            return fundsString;
        }

        public static tblFunds getFund(int pointOfSaleID, string currencyCode)
        {
            ePlatEntities db = new ePlatEntities();
            tblFunds fund;
            if (db.tblFunds_PointsOfSale.Where(m => m.pointOfSaleID == pointOfSaleID).Count() > 1)
            {
                fund = db.tblFunds.Single(m => m.fundID == (int)db.tblFunds_PointsOfSale.FirstOrDefault(x => x.pointOfSaleID == pointOfSaleID && x.tblFunds.tblCurrencies.currencyCode == currencyCode).fundID);
            }
            else
            {
                fund = db.tblFunds.Single(m => m.fundID == (int)db.tblFunds_PointsOfSale.FirstOrDefault(x => x.pointOfSaleID == pointOfSaleID).fundID);
            }
            return fund;
        }

        public List<OutcomeInfoModel> SearchEgresses(OutcomeSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<OutcomeInfoModel>();
            var iDate = model.OutcomeSearch_I_Date != null ? DateTime.Parse(model.OutcomeSearch_I_Date, CultureInfo.InvariantCulture) : (DateTime?)null;
            var fDate = model.OutcomeSearch_F_Date != null ? DateTime.Parse(model.OutcomeSearch_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
            var opcs = model.OutcomeSearch_Opc != null ? model.OutcomeSearch_Opc.Select(m => (long?)long.Parse(m)).ToArray() : new long?[] { };
            var agents = model.OutcomeSearch_Agent != null ? model.OutcomeSearch_Agent.Select(m => Guid.Parse(m)).ToArray() : new Guid[] { };
            var pointsOfSale = model.OutcomeSearch_PointOfSale != null ? model.OutcomeSearch_PointOfSale.Select(m => int.Parse(m)).ToArray() : MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(true).Select(m => int.Parse(m.Value)).ToArray();

            var query = from e in db.tblEgresses
                        where (e.egressID == model.OutcomeSearch_Folio || model.OutcomeSearch_Folio == null)
                        && (e.customer.Contains(model.OutcomeSearch_Customer) || model.OutcomeSearch_Customer == null)
                        && ((e.dateSaved >= iDate && e.dateSaved <= fDate) || iDate == null)
                        && (opcs.Contains(e.opcID) || opcs.Count() == 0)
                        && (agents.Contains(e.savedByUserID) || agents.Count() == 0)
                        && (e.invitationNumber.Contains(model.OutcomeSearch_InvitationNumber.Trim()) || model.OutcomeSearch_InvitationNumber == null)
                        && pointsOfSale.Contains(e.pointOfSaleID)
                        select new
                        {
                            e.egressID,
                            e.terminalID,
                            e.dateSaved,
                            opc = e.opcID != null ? e.tblOPCS.opc : e.opcOther,
                            company = e.chargedToCompanyID != null ? e.tblCompanies.company : null,
                            e.customer,
                            e.amount,
                            e.tblCurrencies.currencyCode,
                            e.invitationNumber,
                            e.aspnet_Users.tblUserProfiles.FirstOrDefault(m => m.userID == e.savedByUserID).firstName,
                            e.aspnet_Users.tblUserProfiles.FirstOrDefault(m => m.userID == e.savedByUserID).lastName,
                            e.tblEgressConcepts.egressConcept,
                            e.egressTypeID,
                            e.tblPointsOfSale.pointOfSale
                        };

            foreach (var i in query)
            {
                var egressType = i.egressTypeID.ToString();
                list.Add(new OutcomeInfoModel()
                {
                    OutcomeInfo_EgressID = i.egressID,
                    OutcomeInfo_Terminal = (int)i.terminalID,
                    OutcomeInfo_DateSaved = i.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    OutcomeInfo_OpcText = i.opc,
                    OutcomeInfo_ChargedToCompanyText = i.company != null ? i.company : "General Public",
                    OutcomeInfo_Customer = i.customer,
                    OutcomeInfo_Amount = i.amount,
                    OutcomeInfo_Currency = i.currencyCode,
                    OutcomeInfo_Invitation = i.invitationNumber,
                    OutcomeInfo_Agent = i.firstName + " " + i.lastName,
                    OutcomeInfo_EgressTypeConcept = GeneralFunctions.EgressTypes.Single(m => m.Key == egressType).Value + " > " + i.egressConcept,
                    OutcomeInfo_PointOfSaleText = i.pointOfSale,
                });
            }
            return list;
        }

        public OutcomeInfoModel GetEgressInfo(long OutcomeInfo_EgressID)
        {
            ePlatEntities db = new ePlatEntities();
            var model = new OutcomeInfoModel();
            var query = db.tblEgresses.Single(m => m.egressID == OutcomeInfo_EgressID);

            model.OutcomeInfo_EgressID = query.egressID;
            model.OutcomeInfo_Terminal = (int)query.terminalID;
            model.OutcomeInfo_EgressType = query.egressTypeID;
            model.OutcomeInfo_EgressConcept = (long)query.egressConceptID;
            model.OutcomeInfo_EgressConceptText = query.tblEgressConcepts.egressConcept;
            model.OutcomeInfo_Opc = query.opcID != null ? query.opcID.ToString() : query.opcOther != null ? "null" : "0";
            model.OutcomeInfo_OtherOpc = query.opcOther;
            model.OutcomeInfo_PromotionTeam = query.promotionTeamID != null ? query.promotionTeamID.ToString() : "null";
            model.OutcomeInfo_Budget = query.budgetID != null ? query.budgetID.ToString() + "|" + (query.tblBudgets.budgetExt ? "Extension" : query.tblBudgets.perClient ? "Client" : query.tblBudgets.resetDayOfWeek) : "0";
            model.OutcomeInfo_ChargedToCompany = query.chargedToCompanyID != null ? query.chargedToCompanyID.ToString() : "null";
            model.OutcomeInfo_Customer = query.customer;
            model.OutcomeInfo_Invitation = query.invitationNumber;
            model.OutcomeInfo_Amount = query.amount;
            model.OutcomeInfo_Currency = query.tblCurrencies.currencyCode;
            model.OutcomeInfo_Location = query.locationID != null ? query.locationID.ToString() : "null";
            model.OutcomeInfo_LocationText = query.locationID != null ? query.tblLocations.location.ToString() : "null";
            model.OutcomeInfo_AdminFee = query.adminFee != null ? (decimal)query.adminFee : 0;
            model.OutcomeInfo_AgentComments = query.agentComments;
            model.OutcomeInfo_PointOfSale = query.pointOfSaleID;
            model.OutcomeInfo_DateSaved = query.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.OutcomeInfo_SavedByUser = query.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + query.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName;
            model.OutcomeInfo_AmountOfSale = query.amountOfSale ?? (decimal?)null;
            model.OutcomeInfo_CurrencyOfSale = query.currencyOfSaleID != null ? query.tblCurrencies1.currencyCode : "";

            //manifest fields
            model.CustomerID = query.spiCustomerID != null ? (int)query.spiCustomerID : (int?)null;
            model.FrontOfficeGuestID = query.frontOfficeGuestID;
            model.FrontOfficeResortID = query.frontOfficeResortID;
            model.MarketingProgram = query.spiMarketingProgram;
            model.OPCID = query.spiOpcID != null ? (int)query.spiOpcID : (int?)null;
            model.Source = query.spiSource;
            model.Subdivision = query.spiSubdivision;
            model.TourID = query.spiTourID != null ? (int)query.spiTourID : (int?)null;
            var now = DateTime.Now;
            var date24 = query.dateSaved.AddHours(24);
            var date72 = query.dateSaved.AddHours(72);
            var isAbleToUpdate = false;

            if (now <= date24)
            {
                var privilege = AdminDataModel.GetViewPrivileges(11843);
                if (privilege.Count() > 0 && privilege.FirstOrDefault(m => m.ComponentID == 11843).Edit)
                {
                    isAbleToUpdate = true;
                }
            }
            else if (now > date24 && now <= date72)
            {
                var privilege = AdminDataModel.GetViewPrivileges(11846);
                if (privilege.Count() > 0 && privilege.FirstOrDefault(m => m.ComponentID == 11846).Edit)
                {
                    isAbleToUpdate = true;
                }
            }
            else if (now > date72)
            {
                var privilege = AdminDataModel.GetViewPrivileges(11847);
                if (privilege.Count() > 0 && privilege.FirstOrDefault(m => m.ComponentID == 11847).Edit)
                {
                    isAbleToUpdate = true;
                }
            }
            model.OutcomeInfo_UpdateEgress = isAbleToUpdate;
            //****************************
            //model.OutcomeInfo_UpdateEgress = true;
            //var date = query.dateSaved.AddHours(72);
            //if (GeneralFunctions.IsUserInRole("Onsite Reservations Agent", null, true))
            //{
            //    if (query.dateSaved.Date != DateTime.Today.Date)
            //    {
            //        model.OutcomeInfo_UpdateEgress = false;
            //        //throw new Exception("Your profile doesn't have permission to update on a distinct date");
            //    }
            //}
            //else
            //{
            //    if (DateTime.Now > date && !GeneralFunctions.IsUserInRole("Administrator", null, true) && !GeneralFunctions.IsUserInRole("Comptroller"))
            //    {
            //        model.OutcomeInfo_UpdateEgress = false;
            //        //throw new Exception("Your profile doesn't have permission to update after 72 hrs");
            //    }
            //}

            return model;
        }

        public AttemptResponse Current_SaveEgress(OutcomeInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var purchases = new MasterChartDataModel.Purchases();
            var _terminalID = model.OutcomeInfo_Terminal;
            var cashTransaction = db.tblEgressConcepts.Single(m => m.egressConceptID == model.OutcomeInfo_EgressConcept).cashTransaction;
            tblEgresses egress;

            tblFunds targetFund;
            #region "fund selection"
            System.Web.HttpContext.Current.Application.Lock();
            if (db.tblFunds_PointsOfSale.Where(m => m.pointOfSaleID == model.OutcomeInfo_PointOfSale).Count() > 1)
            {
                var currencyFunds = db.tblFunds_PointsOfSale.Where(x => x.pointOfSaleID == model.OutcomeInfo_PointOfSale && x.tblFunds.tblCurrencies.currencyCode == model.OutcomeInfo_Currency);
                if (currencyFunds.Count() == 1)
                {
                    targetFund = db.tblFunds.Single(m => m.fundID == (int)currencyFunds.FirstOrDefault().fundID);
                    if (Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) == 0)
                    {
                        System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = targetFund.amount;
                    }
                }
                else
                {
                    if (model.OutcomeInfo_Fund == 0)
                    {
                        //por default esta propiedad está en 0 dado que no está en uso
                        response.Type = Attempt_ResponseTypes.Warning;
                        if (model.OutcomeInfo_EgressID != 0)
                        {
                            egress = db.tblEgresses.Single(m => m.egressID == model.OutcomeInfo_EgressID);
                            response.Message = "There is no specific fund for " + model.OutcomeInfo_Currency + " but register was assigned to \"" + egress.tblFunds.fundName + " " + egress.tblFunds.tblCurrencies.currencyCode + "\"";
                        }
                        else
                        {
                            response.Message = "There is no specific fund for " + model.OutcomeInfo_Currency;
                        }
                        var _existingFunds = db.tblFunds_PointsOfSale.Where(m => m.pointOfSaleID == model.OutcomeInfo_PointOfSale).Select(m => new { fundID = m.fundID, currencyCode = m.tblFunds.tblCurrencies.currencyCode, fund = m.tblFunds.fundName });
                        var existingFunds = new List<string>();
                        foreach (var i in _existingFunds)
                        {
                            existingFunds.Add(i.fundID + "_" + i.fund + " - " + i.currencyCode);
                        }
                        response.ObjectID = new { existingFunds = string.Join("|", existingFunds.ToArray()) };
                        return response;
                    }
                    else
                    {
                        //el modelo traerá el valor seleccionado en el messageBox
                        targetFund = db.tblFunds.Single(m => m.fundID == model.OutcomeInfo_Fund);
                        if (Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) == 0)
                        {
                            System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = targetFund.amount;
                        }
                    }
                }
            }
            else
            {
                targetFund = db.tblFunds.Single(m => m.fundID == (int)db.tblFunds_PointsOfSale.FirstOrDefault(x => x.pointOfSaleID == model.OutcomeInfo_PointOfSale).fundID);
                if (Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) == 0)
                {
                    System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = targetFund.amount;
                }
            }
            #endregion

            var _currentRates = purchases.GetExchangeRates(DateTime.Now, _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();
            var _targetRates = purchases.GetExchangeRates((model.OutcomeInfo_DateSaved != null ? DateTime.Parse(model.OutcomeInfo_DateSaved) : DateTime.Now), _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();
            var _amountToSave = purchases.ConvertAmountToRate(_targetRates, model.OutcomeInfo_Amount, model.OutcomeInfo_Currency, targetFund.tblCurrencies.currencyCode).Key;//converted to target fund currency
            if (model.OutcomeInfo_EgressID != 0)
            {
                try
                {
                    #region "Update"
                    egress = db.tblEgresses.Single(m => m.egressID == model.OutcomeInfo_EgressID);
                    //esta línea asigna valor a la variable de aplicación en caso de que no tenga (en teoría no se afecta otras transacciones porque el acceso a la variable está bloqueado
                    System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID] = System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID] == null ? egress.tblFunds.amount : System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID];
                    #region "verify if update is permitted"
                    var now = DateTime.Now;
                    //var date = egress.dateSaved.AddHours(72);
                    var date72 = egress.dateSaved.AddHours(72);
                    var date24 = egress.dateSaved.AddHours(24);
                    var isAbleToUpdate = false;

                    if (model.OutcomeInfo_Amount < 0)
                    {
                        throw new Exception("You can't save an egress with negative amount");
                    }

                    if (now <= date24)
                    {
                        var privilege = AdminDataModel.GetViewPrivileges(11843);
                        if (privilege.Count() > 0 && privilege.FirstOrDefault(m => m.ComponentID == 11843).Edit)
                        {
                            isAbleToUpdate = true;
                        }
                    }
                    else if (now > date24 && now <= date72)
                    {
                        var privilege = AdminDataModel.GetViewPrivileges(11846);
                        if (privilege.Count() > 0 && privilege.FirstOrDefault(m => m.ComponentID == 11846).Edit)
                        {
                            isAbleToUpdate = true;
                        }
                    }
                    else if (now > date72)
                    {
                        var privilege = AdminDataModel.GetViewPrivileges(11847);
                        if (privilege.Count() > 0 && privilege.FirstOrDefault(m => m.ComponentID == 11847).Edit)
                        {
                            isAbleToUpdate = true;
                        }
                    }
                    //if (DateTime.Now > date && !GeneralFunctions.IsUserInRole("Administrator") && !GeneralFunctions.IsUserInRole("Comptroller") && !GeneralFunctions.IsUserInRole("Comptroller VEX") && !GeneralFunctions.IsUserInRole("Department Administrator"))
                    //{
                    //    throw new Exception("Your profile doesn't have permission to update after 72 hrs");
                    //}
                    //}
                    if (!isAbleToUpdate)
                    {
                        throw new Exception("Your profile doesn't have permission to update.");
                    }
                    #endregion

                    var paymentsAmount = new List<KeyValuePair<decimal, string>>();
                    var _egressRates = purchases.GetExchangeRates(egress.dateSaved, _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();//point of sale 0 because its general ER needed
                    var _amountSaved = purchases.ConvertAmountToRate(_egressRates, egress.amount, egress.tblCurrencies.currencyCode, egress.tblFunds.tblCurrencies.currencyCode).Key;//converted to saved fund currency
                    int budgetID = model.OutcomeInfo_Budget != null ? int.Parse(model.OutcomeInfo_Budget.Split('|')[0]) : 0;
                    var budget = new tblBudgets();
                    decimal _amountToSaveInBgtCurr = 0;

                    if (budgetID != 0)
                    {
                        budget = db.tblBudgets.Single(m => m.budgetID == budgetID);
                        var budgetTeams = db.tblBudgets_PromotionTeams.Where(m => m.budgetID == budgetID).Select(m => m.promotionTeamID).ToArray();
                        _amountToSaveInBgtCurr = purchases.ConvertAmountToRate(_currentRates, model.OutcomeInfo_Amount, model.OutcomeInfo_Currency, budget.tblCurrencies.currencyCode).Key;
                    }
                    decimal budgetUsed = 0;
                    decimal budgetLeft = 0;
                    paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                    model.OutcomeInfo_EgressConceptText = db.tblEgressConcepts.Single(m => m.egressConceptID == model.OutcomeInfo_EgressConcept).egressConcept;

                    #region "budget application cases"
                    if (egress.tblEgressConcepts.egressConcept.ToLower() == "cash gift")
                    {
                        if (model.OutcomeInfo_EgressConceptText.ToLower() == "cash gift")
                        {
                            if (egress.budgetID != null)
                            {
                                if (model.OutcomeInfo_Budget != "0")
                                {
                                    if (egress.budgetID == int.Parse(model.OutcomeInfo_Budget.Split('|')[0]))
                                    {
                                        if (_amountSaved != model.OutcomeInfo_Amount)
                                        {
                                            paymentsAmount.Clear();
                                            if (!budget.perWeek)
                                            {
                                                paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                                            }
                                            else
                                            {
                                                #region "budget per week"
                                                var opcID = int.Parse(model.OutcomeInfo_Opc);
                                                var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                                                budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), _terminalID, egress.pointOfSaleID);
                                                //_amountSaved is on model' s currency, convert to budget's currency for substraction.
                                                var _amountInBudgetCurrency = purchases.ConvertAmountToRate(_currentRates, _amountSaved, model.OutcomeInfo_Currency, budget.tblCurrencies.currencyCode).Key;
                                                budgetUsed -= _amountInBudgetCurrency;//line that restores in budget amount previously used
                                                budgetLeft = budget.budget - budgetUsed;
                                                if (budgetLeft >= _amountToSaveInBgtCurr)
                                                {
                                                    //budget covers amount
                                                    //paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_currentRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                                                    paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_targetRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                                                }
                                                else
                                                {
                                                    //budget is not enough to cover amount
                                                    //response.Type = Attempt_ResponseTypes.Error;
                                                    //response.Message = "Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")";
                                                    //response.ObjectID = 0;
                                                    //return response;
                                                    throw new Exception("Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")");
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                    else
                                    {
                                        paymentsAmount.Clear();
                                        //update with new budget verification
                                        if (!budget.perWeek)
                                        {
                                            paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                                        }
                                        else
                                        {
                                            #region "budget per week"
                                            var opcID = int.Parse(model.OutcomeInfo_Opc);
                                            var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                                            budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), _terminalID, egress.pointOfSaleID);
                                            budgetLeft = budget.budget - budgetUsed;
                                            if (budgetLeft >= _amountToSaveInBgtCurr)
                                            {
                                                //budget covers amount
                                                //paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_currentRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                                                paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_targetRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                                            }
                                            else
                                            {
                                                //budget is not enough to cover amount
                                                //response.Type = Attempt_ResponseTypes.Error;
                                                //response.Message = "Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")";
                                                //response.ObjectID = 0;
                                                //return response;
                                                throw new Exception("Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")");
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (model.OutcomeInfo_Budget != "0")
                                {
                                    paymentsAmount.Clear();
                                    //update with budget verification
                                    if (!budget.perWeek)
                                    {
                                        paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                                    }
                                    else
                                    {
                                        #region "budget per week"
                                        var opcID = int.Parse(model.OutcomeInfo_Opc);
                                        var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                                        budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), _terminalID, egress.pointOfSaleID);
                                        budgetLeft = budget.budget - budgetUsed;
                                        if (budgetLeft >= _amountToSaveInBgtCurr)
                                        {
                                            //budget covers amount
                                            //paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_currentRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                                            paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_targetRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                                        }
                                        else
                                        {
                                            //budget is not enough to cover amount
                                            //response.Type = Attempt_ResponseTypes.Error;
                                            //response.Message = "Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")";
                                            //response.ObjectID = 0;
                                            //return response;
                                            throw new Exception("Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")");
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (model.OutcomeInfo_EgressConceptText.ToLower() == "cash gift")
                        {
                            if (model.OutcomeInfo_Budget != null)
                            {
                                paymentsAmount.Clear();
                                //update with budget verification
                                if (!budget.perWeek)
                                {
                                    paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                                }
                                else
                                {
                                    #region "budget per week"
                                    var opcID = int.Parse(model.OutcomeInfo_Opc);
                                    var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                                    budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), _terminalID, egress.pointOfSaleID);
                                    budgetLeft = budget.budget - budgetUsed;
                                    if (budgetLeft >= _amountToSaveInBgtCurr)
                                    {
                                        //budget covers amount
                                        //paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_currentRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                                        paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_targetRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                                    }
                                    else
                                    {
                                        //budget is not enough to cover amount
                                        //response.Type = Attempt_ResponseTypes.Error;
                                        //response.Message = "Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")";
                                        //response.ObjectID = 0;
                                        //return response;
                                        throw new Exception("Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")");
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion

                    if (egress.dateSaved.Month != DateTime.Parse(model.OutcomeInfo_DateSaved).Month)//el egreso se intenta cambiar de mes
                    {
                        var statements = db.tblFundsStatements.Where(m => (m.fundID == egress.fundID || m.fundID == targetFund.fundID) && m.year >= egress.dateSaved.Year && m.month >= egress.dateSaved.Month);
                        if (statements.Count() > 0)
                        {
                            foreach (var statement in statements)
                            {
                                db.DeleteObject(statement);
                            }
                        }
                    }

                    if (egress.tblEgressConcepts.cashTransaction)//if true, affect previous fund
                    {
                        //egress.tblFunds.amount += _amountSaved;//restore amount in fund
                        var prevFund = System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID];
                        System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID] = Convert.ToDecimal(System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID]) + _amountSaved;
                    }
                    if (cashTransaction)//if true, affect target fund
                    {
                        //targetFund.amount -= _amountToSave;
                        System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) - _amountToSave;//esta linea no funciona cuando el fondo origen y el destino son el mismo.
                    }
                    //db.SaveChanges();

                    #region "update egress"
                    try
                    {
                        egress.tblFunds.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID]);
                        targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]);
                        foreach (var charge in paymentsAmount)
                        {
                            egress.terminalID = model.OutcomeInfo_Terminal;
                            egress.egressTypeID = model.OutcomeInfo_EgressType;
                            egress.egressConceptID = model.OutcomeInfo_EgressConcept;
                            egress.pointOfSaleID = model.OutcomeInfo_PointOfSale;
                            egress.opcID = model.OutcomeInfo_Opc != null ? model.OutcomeInfo_Opc != "null" ? long.Parse(model.OutcomeInfo_Opc) : (long?)null : (long?)null;
                            egress.opcOther = model.OutcomeInfo_Opc == "null" ? model.OutcomeInfo_OtherOpc : null;
                            egress.promotionTeamID = model.OutcomeInfo_PromotionTeam != null ? int.Parse(model.OutcomeInfo_PromotionTeam) : (int?)null;
                            egress.budgetID = charge.Value != null && charge.Value != "0" && model.OutcomeInfo_EgressConceptText.ToLower() == "cash gift" ? int.Parse(charge.Value.Split('|')[0]) : (int?)null;
                            egress.chargedToCompanyID = model.OutcomeInfo_ChargedToCompany != "null" ? int.Parse(model.OutcomeInfo_ChargedToCompany) : (int?)null;
                            egress.customer = model.OutcomeInfo_Customer;
                            egress.invitationNumber = model.OutcomeInfo_Invitation.Trim();
                            egress.amountOfSale = model.OutcomeInfo_EgressConcept == 24 ? model.OutcomeInfo_AmountOfSale : (decimal?)null;//burned invitation
                            egress.currencyOfSaleID = egress.amountOfSale != null ? db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_CurrencyOfSale).currencyID : (int?)null;
                            egress.fundID = targetFund.fundID;
                            egress.amount = charge.Key;//model.OutcomeInfo_Amount;
                            egress.adminFee = model.OutcomeInfo_AdminFee ?? 0;
                            egress.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_Currency).currencyID;
                            egress.locationID = model.OutcomeInfo_Location != "null" ? int.Parse(model.OutcomeInfo_Location) : (int?)null;
                            egress.agentComments = model.OutcomeInfo_AgentComments;
                            egress.dateSaved = model.OutcomeInfo_DateSaved != egress.dateSaved.Date.ToString("yyyy-MM-dd") && model.OutcomeInfo_DateSaved != null ? DateTime.Parse((model.OutcomeInfo_DateSaved + " 23:59:59"), CultureInfo.InvariantCulture) : egress.dateSaved;
                            egress.dateLastModification = DateTime.Now;
                            egress.modifiedByUserID = session.UserID;
                            egress.spiCustomerID = model.CustomerID;
                            egress.spiMarketingProgram = model.MarketingProgram;
                            egress.spiSubdivision = model.Subdivision;
                            egress.spiSource = model.Source;
                            egress.spiOpcID = model.OPCID;
                            egress.frontOfficeGuestID = egress.frontOfficeGuestID == null ? model.FrontOfficeGuestID : egress.frontOfficeGuestID;
                            egress.frontOfficeResortID = egress.frontOfficeResortID == null ? model.FrontOfficeResortID : egress.frontOfficeResortID;
                            egress.spiTourID = model.TourID;
                            db.SaveChanges();
                        }
                        System.Web.HttpContext.Current.Application.UnLock();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Egress Updated";
                        response.ObjectID = new { egressID = egress.egressID, date = egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), agent = egress.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + egress.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName, print = (model.OutcomeInfo_SaveAndPrint ? true : false) };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        //WITH AMOUNT ASIGNATIONS ON THE TRY BLOCK, NOT RESTORATION NEEDED
                        //restore previous balance
                        //egress.tblFunds.amount -= _amountSaved;
                        //targetFund.amount += _amountToSave;
                        //targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) + _amountToSave;
                        //db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "There was an error updating egress";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                    #endregion
                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Application.UnLock();
                    response.Type = Attempt_ResponseTypes.Error;
                    //response.Message = "Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                #region "save"
                var paymentsAmount = new List<KeyValuePair<decimal, string>>();
                paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                model.OutcomeInfo_EgressConceptText = db.tblEgressConcepts.Single(m => m.egressConceptID == model.OutcomeInfo_EgressConcept).egressConcept;

                try
                {
                    if (model.OutcomeInfo_Amount < 0)
                    {
                        throw new Exception("You can't save an egress with negative amount");
                    }
                    if (cashTransaction)
                    {
                        //targetFund.amount -= _amountToSave;
                        System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) - _amountToSave;
                        //db.SaveChanges();
                    }

                    #region "create egress"
                    //try
                    //{
                        targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]);
                        if (model.OutcomeInfo_EgressConceptText.ToLower() == "cash gift" && model.OutcomeInfo_Budget != null && model.OutcomeInfo_Budget != "0")
                        {
                            #region "Apply budget"
                            //chargeback with budget selected
                            paymentsAmount.Clear();
                            int budgetID = int.Parse(model.OutcomeInfo_Budget.Split('|')[0]);
                            var budget = db.tblBudgets.Single(m => m.budgetID == budgetID);
                            var budgetTeams = db.tblBudgets_PromotionTeams.Where(m => m.budgetID == budgetID).Select(m => m.promotionTeamID).ToArray();
                            decimal budgetUsed = 0;
                            decimal amountLeft = purchases.ConvertAmountToRate(_currentRates, model.OutcomeInfo_Amount, model.OutcomeInfo_Currency, budget.tblCurrencies.currencyCode).Key;
                            decimal budgetLeft = 0;

                            if (!budget.perWeek)
                            {
                                paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                            }
                            else
                            {
                                #region "budget per week"
                                var opcID = int.Parse(model.OutcomeInfo_Opc);
                                var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                                budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), _terminalID, model.OutcomeInfo_PointOfSale);
                                budgetLeft = budget.budget - budgetUsed;

                                if (budgetLeft >= amountLeft)
                                {
                                    //budget covers amount
                                    paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_currentRates, amountLeft, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                                }
                                else
                                {
                                    //budget is not enough to covers amount
                                    throw new Exception("charge amount exceeds available budget amount");
                                }
                                #endregion
                            }
                            #endregion
                        }
                        foreach (var charge in paymentsAmount)
                        {
                            var _egress = new tblEgresses();
                            _egress.egressTypeID = model.OutcomeInfo_EgressType;
                            _egress.terminalID = model.OutcomeInfo_Terminal;
                            _egress.egressConceptID = model.OutcomeInfo_EgressConcept;
                            _egress.fundID = targetFund.fundID;
                            _egress.pointOfSaleID = model.OutcomeInfo_PointOfSale;
                            _egress.opcID = model.OutcomeInfo_Opc != null ? model.OutcomeInfo_Opc != "null" ? long.Parse(model.OutcomeInfo_Opc) : (long?)null : (long?)null;
                            _egress.opcOther = model.OutcomeInfo_Opc == "null" ? model.OutcomeInfo_OtherOpc : null;
                            _egress.promotionTeamID = model.OutcomeInfo_PromotionTeam != null ? int.Parse(model.OutcomeInfo_PromotionTeam) : (int?)null;
                            _egress.budgetID = charge.Value != null && charge.Value != "0" && model.OutcomeInfo_EgressConceptText.ToLower() == "cash gift" ? int.Parse(charge.Value.Split('|')[0]) : (int?)null;
                            _egress.chargedToCompanyID = model.OutcomeInfo_ChargedToCompany != "null" ? int.Parse(model.OutcomeInfo_ChargedToCompany) : (int?)null;
                            _egress.customer = model.OutcomeInfo_Customer;
                            _egress.invitationNumber = model.OutcomeInfo_Invitation.Trim();
                            _egress.amount = charge.Key;
                            _egress.adminFee = model.OutcomeInfo_AdminFee ?? 0;
                            _egress.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_Currency).currencyID;
                            _egress.locationID = model.OutcomeInfo_Location != "null" ? int.Parse(model.OutcomeInfo_Location) : (int?)null;
                            _egress.dateSaved = DateTime.Now;
                            _egress.savedByUserID = session.UserID;
                            _egress.agentComments = model.OutcomeInfo_AgentComments;
                            _egress.pointOfSaleID = model.OutcomeInfo_PointOfSale;
                            _egress.amountOfSale = model.OutcomeInfo_EgressConcept == 24 ? model.OutcomeInfo_AmountOfSale : (decimal?)null;//burned invitation
                            _egress.currencyOfSaleID = _egress.amountOfSale != null ? db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_CurrencyOfSale).currencyID : (int?)null;
                            _egress.spiCustomerID = model.CustomerID;
                            _egress.spiMarketingProgram = model.MarketingProgram;
                            _egress.spiSubdivision = model.Subdivision;
                            _egress.spiSource = model.Source;
                            _egress.spiOpcID = model.OPCID;
                            _egress.frontOfficeGuestID = model.FrontOfficeGuestID;
                            _egress.frontOfficeResortID = model.FrontOfficeResortID;
                            _egress.spiTourID = model.TourID;
                            db.tblEgresses.AddObject(_egress);
                            db.SaveChanges();

                            response.ObjectID = new
                            {
                                egressID = _egress.egressID,
                                date = _egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                agent = _egress.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + _egress.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName,
                                print = (model.OutcomeInfo_SaveAndPrint ? true : false)
                            };
                        }
                        System.Web.HttpContext.Current.Application.UnLock();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Egress Saved";
                        return response;
                    //}
                    //catch
                    //{
                        //WITH ASSIGNATION ON TRY BLOCK, NOT RESTORATION NEEDED
                        //if (cashTransaction)//only if changes were made to the fund
                        //{
                        //    //targetFund.amount += _amountToSave;
                        //    targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) + _amountToSave;
                        //    db.SaveChanges();
                        //}
                        //throw new Exception("There was an error saving egress");
                    //}
                    #endregion
                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Application.UnLock();
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Egress NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public AttemptResponse SaveEgress(OutcomeInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var purchases = new MasterChartDataModel.Purchases();
            var _terminalID = model.OutcomeInfo_Terminal;
            var cashTransaction = db.tblEgressConcepts.Single(m => m.egressConceptID == model.OutcomeInfo_EgressConcept).cashTransaction;
            tblEgresses egress;

            tblFunds targetFund;
            #region "fund selection"
            System.Web.HttpContext.Current.Application.Lock();
            if (db.tblFunds_PointsOfSale.Where(m => m.pointOfSaleID == model.OutcomeInfo_PointOfSale).Count() > 1)
            {
                var currencyFunds = db.tblFunds_PointsOfSale.Where(x => x.pointOfSaleID == model.OutcomeInfo_PointOfSale && x.tblFunds.tblCurrencies.currencyCode == model.OutcomeInfo_Currency);
                if (currencyFunds.Count() == 1)
                {
                    targetFund = db.tblFunds.Single(m => m.fundID == (int)currencyFunds.FirstOrDefault().fundID);
                    if (Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) == 0)
                    {
                        System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = targetFund.amount;
                    }
                }
                else
                {
                    if (model.OutcomeInfo_Fund == 0)
                    {
                        //por default esta propiedad está en 0 dado que no está en uso
                        response.Type = Attempt_ResponseTypes.Warning;
                        if (model.OutcomeInfo_EgressID != 0)
                        {
                            egress = db.tblEgresses.Single(m => m.egressID == model.OutcomeInfo_EgressID);
                            response.Message = "There is no specific fund for " + model.OutcomeInfo_Currency + " but register was assigned to \"" + egress.tblFunds.fundName + " " + egress.tblFunds.tblCurrencies.currencyCode + "\"";
                        }
                        else
                        {
                            response.Message = "There is no specific fund for " + model.OutcomeInfo_Currency;
                        }
                        var _existingFunds = db.tblFunds_PointsOfSale.Where(m => m.pointOfSaleID == model.OutcomeInfo_PointOfSale).Select(m => new { fundID = m.fundID, currencyCode = m.tblFunds.tblCurrencies.currencyCode, fund = m.tblFunds.fundName });
                        var existingFunds = new List<string>();
                        foreach (var i in _existingFunds)
                        {
                            existingFunds.Add(i.fundID + "_" + i.fund + " - " + i.currencyCode);
                        }
                        response.ObjectID = new { existingFunds = string.Join("|", existingFunds.ToArray()) };
                        return response;
                    }
                    else
                    {
                        //el modelo traerá el valor seleccionado en el messageBox
                        targetFund = db.tblFunds.Single(m => m.fundID == model.OutcomeInfo_Fund);
                        if (Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) == 0)
                        {
                            System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = targetFund.amount;
                        }
                    }
                }
            }
            else
            {
                targetFund = db.tblFunds.Single(m => m.fundID == (int)db.tblFunds_PointsOfSale.FirstOrDefault(x => x.pointOfSaleID == model.OutcomeInfo_PointOfSale).fundID);
                if (Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) == 0)
                {
                    System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = targetFund.amount;
                }
            }
            #endregion

            var _currentRates = purchases.GetExchangeRates(DateTime.Now, _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();
            var _targetRates = purchases.GetExchangeRates((model.OutcomeInfo_DateSaved != null ? DateTime.Parse(model.OutcomeInfo_DateSaved) : DateTime.Now), _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();
            var _amountToSave = purchases.ConvertAmountToRate(_targetRates, model.OutcomeInfo_Amount, model.OutcomeInfo_Currency, targetFund.tblCurrencies.currencyCode).Key;//converted to target fund currency
            if (model.OutcomeInfo_EgressID != 0)
            {
                try
                {
                    #region "Update"
                    egress = db.tblEgresses.Single(m => m.egressID == model.OutcomeInfo_EgressID);
                    //esta línea asigna valor a la variable de aplicación en caso de que no tenga (en teoría no se afecta otras transacciones porque el acceso a la variable está bloqueado
                    System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID] = System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID] == null ? egress.tblFunds.amount : System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID];
                    #region "verify if update is permitted"
                    var now = DateTime.Now;
                    //var date = egress.dateSaved.AddHours(72);
                    var date72 = egress.dateSaved.AddHours(72);
                    var date24 = egress.dateSaved.AddHours(24);
                    var isAbleToUpdate = false;

                    if (model.OutcomeInfo_Amount < 0)
                    {
                        throw new Exception("You can't save an egress with negative amount");
                    }

                    if (now <= date24)
                    {
                        var privilege = AdminDataModel.GetViewPrivileges(11843);
                        if (privilege.Count() > 0 && privilege.FirstOrDefault(m => m.ComponentID == 11843).Edit)
                        {
                            isAbleToUpdate = true;
                        }
                    }
                    else if (now > date24 && now <= date72)
                    {
                        var privilege = AdminDataModel.GetViewPrivileges(11846);
                        if (privilege.Count() > 0 && privilege.FirstOrDefault(m => m.ComponentID == 11846).Edit)
                        {
                            isAbleToUpdate = true;
                        }
                    }
                    else if (now > date72)
                    {
                        var privilege = AdminDataModel.GetViewPrivileges(11847);
                        if (privilege.Count() > 0 && privilege.FirstOrDefault(m => m.ComponentID == 11847).Edit)
                        {
                            isAbleToUpdate = true;
                        }
                    }

                    if (!isAbleToUpdate)
                    {
                        throw new Exception("Your profile doesn't have permission to update.");
                    }
                    #endregion

                    var paymentsAmount = new List<KeyValuePair<decimal, string>>();
                    var _egressRates = purchases.GetExchangeRates(egress.dateSaved, _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();//point of sale 0 because its general ER needed
                    var _amountSaved = purchases.ConvertAmountToRate(_egressRates, egress.amount, egress.tblCurrencies.currencyCode, egress.tblFunds.tblCurrencies.currencyCode).Key;//converted to saved fund currency
                    int budgetID = model.OutcomeInfo_Budget != null && model.OutcomeInfo_Budget != "0" ? int.Parse(model.OutcomeInfo_Budget.Split('|')[0]) : 0;

                    //*************
                    paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, null));
                    if (budgetID != 0)
                    {
                        paymentsAmount.Clear();
                        if (db.tblEgressConcepts.Single(m => m.egressConceptID == model.OutcomeInfo_EgressConcept).allowBudget == true)
                        {
                            var budget = db.tblBudgets.Single(m => m.budgetID == budgetID);
                            var amountToSave = purchases.ConvertAmountToRate(_egressRates, model.OutcomeInfo_Amount, model.OutcomeInfo_Currency, budget.tblCurrencies.currencyCode).Key;
                            if (!budget.perWeek)
                            {
                                var savedEgresses = db.tblEgresses.Where(m => m.invitationNumber == model.OutcomeInfo_Invitation && m.pointOfSaleID == model.OutcomeInfo_PointOfSale && m.budgetID == budgetID && m.egressID != egress.egressID).Select(m => new { m.amount, m.tblCurrencies.currencyCode, budgetCurrency = m.tblBudgets.tblCurrencies.currencyCode, m.dateSaved });
                                var savedPayments = db.tblPaymentDetails.Where(m => m.tblPurchases.terminalID == _terminalID && m.tblPurchases.pointOfSaleID == model.OutcomeInfo_PointOfSale && m.paymentType == 3 && m.invitation == model.OutcomeInfo_Invitation && m.deletedByUserID == null && m.budgetID == budgetID).Select(m => new { m.amount, m.tblCurrencies.currencyCode, budgetCurrency = m.tblBudgets.tblCurrencies.currencyCode, m.dateSaved });
                                var savedTransactions = savedEgresses.Count() > 0 ? savedPayments.Count() > 0 ? savedEgresses.Concat(savedPayments) : savedEgresses : savedPayments;

                                decimal budgetAvailable = 0;

                                if (savedTransactions.Count() == 0)
                                {
                                    budgetAvailable = budget.budget;
                                }
                                else
                                {
                                    decimal _budgetUsed = 0;
                                    foreach (var i in savedTransactions)
                                    {
                                        var _rate = purchases.GetExchangeRates(i.dateSaved, _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();
                                        _budgetUsed += purchases.ConvertAmountToRate(_rate, i.amount, i.currencyCode, i.budgetCurrency).Key;
                                    }
                                    budgetAvailable = budget.budget - _budgetUsed;
                                }
                                var useBudget = budgetAvailable > 0 ? true : false;

                                if (useBudget)
                                {

                                    var toBudget = purchases.ConvertAmountToRate(_targetRates, (amountToSave <= budgetAvailable ? amountToSave : budgetAvailable), budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key;
                                    var toOther = purchases.ConvertAmountToRate(_targetRates, (amountToSave <= budgetAvailable ? 0 : amountToSave - budgetAvailable), budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key;
                                    paymentsAmount.Add(new KeyValuePair<decimal, string>(toBudget, model.OutcomeInfo_Budget));
                                    if (toOther != 0)
                                    {
                                        paymentsAmount.Add(new KeyValuePair<decimal, string>(toOther, null));
                                    }
                                }
                                else
                                {
                                    //paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                                    paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, null));
                                }
                            }
                            else
                            {
                                var opcID = int.Parse(model.OutcomeInfo_Opc);
                                var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                                var _budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, egress.dateSaved.ToString("yyyy-MM-dd"), egress.terminalID, egress.pointOfSaleID);
                                var budgetAvailable = budget.budget - _budgetUsed <= 0 ? 0 : budget.budget - _budgetUsed;

                                var useBudget = budgetAvailable > 0 ? true : false;
                                if (useBudget)
                                {

                                    var toBudget = purchases.ConvertAmountToRate(_targetRates, (amountToSave <= budgetAvailable ? amountToSave : budgetAvailable), budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key;
                                    var toOther = purchases.ConvertAmountToRate(_targetRates, (amountToSave <= budgetAvailable ? 0 : amountToSave - budgetAvailable), budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key;
                                    paymentsAmount.Add(new KeyValuePair<decimal, string>(toBudget, model.OutcomeInfo_Budget));
                                    if (toOther != 0)
                                    {
                                        paymentsAmount.Add(new KeyValuePair<decimal, string>(toOther, null));
                                    }
                                }
                                else
                                {
                                    response.Message = "<br />No Budget Available";
                                    paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, null));
                                }
                            }
                        }
                    }

                    //*************
                    model.OutcomeInfo_EgressConceptText = db.tblEgressConcepts.Single(m => m.egressConceptID == model.OutcomeInfo_EgressConcept).egressConcept;

                    #region "budget application cases"
                    //if (egress.tblEgressConcepts.egressConcept.ToLower() == "cash gift")
                    //{
                    //    if (model.OutcomeInfo_EgressConceptText.ToLower() == "cash gift")
                    //    {
                    //        if (egress.budgetID != null)
                    //        {
                    //            if (model.OutcomeInfo_Budget != "0")
                    //            {
                    //                if (egress.budgetID == int.Parse(model.OutcomeInfo_Budget.Split('|')[0]))
                    //                {
                    //                    if (_amountSaved != model.OutcomeInfo_Amount)
                    //                    {
                    //                        //update only in amount
                    //                        paymentsAmount.Clear();
                    //                        if (!budget.perWeek)
                    //                        {
                    //                            paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                    //                        }
                    //                        else
                    //                        {
                    //                            #region "budget per week"
                    //                            var opcID = int.Parse(model.OutcomeInfo_Opc);
                    //                            var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                    //                            budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), _terminalID, egress.pointOfSaleID);
                    //                            //_amountSaved is on model' s currency, convert to budget's currency for substraction.
                    //                            var _amountInBudgetCurrency = purchases.ConvertAmountToRate(_currentRates, _amountSaved, model.OutcomeInfo_Currency, budget.tblCurrencies.currencyCode).Key;
                    //                            budgetUsed -= _amountInBudgetCurrency;//line that restores in budget amount previously used
                    //                            budgetLeft = budget.budget - budgetUsed;
                    //                            if (budgetLeft >= _amountToSaveInBgtCurr)
                    //                            {
                    //                                //budget covers amount
                    //                                paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_targetRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                    //                            }
                    //                            else
                    //                            {
                    //                                //budget is not enough to cover amount
                    //                                throw new Exception("Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")");
                    //                            }
                    //                            #endregion
                    //                        }
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    paymentsAmount.Clear();
                    //                    //update with new budget verification
                    //                    if (!budget.perWeek)
                    //                    {
                    //                        paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                    //                    }
                    //                    else
                    //                    {
                    //                        #region "budget per week"
                    //                        var opcID = int.Parse(model.OutcomeInfo_Opc);
                    //                        var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                    //                        budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), _terminalID, egress.pointOfSaleID);
                    //                        budgetLeft = budget.budget - budgetUsed;
                    //                        if (budgetLeft >= _amountToSaveInBgtCurr)
                    //                        {
                    //                            //budget covers amount
                    //                            //paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_currentRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                    //                            paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_targetRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                    //                        }
                    //                        else
                    //                        {
                    //                            //budget is not enough to cover amount
                    //                            //response.Type = Attempt_ResponseTypes.Error;
                    //                            //response.Message = "Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")";
                    //                            //response.ObjectID = 0;
                    //                            //return response;
                    //                            throw new Exception("Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")");
                    //                        }
                    //                        #endregion
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (model.OutcomeInfo_Budget != "0")
                    //            {
                    //                paymentsAmount.Clear();
                    //                //update with budget verification
                    //                if (!budget.perWeek)
                    //                {
                    //                    paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                    //                }
                    //                else
                    //                {
                    //                    #region "budget per week"
                    //                    var opcID = int.Parse(model.OutcomeInfo_Opc);
                    //                    var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                    //                    budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), _terminalID, egress.pointOfSaleID);
                    //                    budgetLeft = budget.budget - budgetUsed;
                    //                    if (budgetLeft >= _amountToSaveInBgtCurr)
                    //                    {
                    //                        //budget covers amount
                    //                        //paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_currentRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                    //                        paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_targetRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                    //                    }
                    //                    else
                    //                    {
                    //                        //budget is not enough to cover amount
                    //                        //response.Type = Attempt_ResponseTypes.Error;
                    //                        //response.Message = "Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")";
                    //                        //response.ObjectID = 0;
                    //                        //return response;
                    //                        throw new Exception("Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")");
                    //                    }
                    //                    #endregion
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (model.OutcomeInfo_EgressConceptText.ToLower() == "cash gift")
                    //    {
                    //        if (model.OutcomeInfo_Budget != null)
                    //        {
                    //            paymentsAmount.Clear();
                    //            //update with budget verification
                    //            if (!budget.perWeek)
                    //            {
                    //                paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                    //            }
                    //            else
                    //            {
                    //                #region "budget per week"
                    //                var opcID = int.Parse(model.OutcomeInfo_Opc);
                    //                var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                    //                budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), _terminalID, egress.pointOfSaleID);
                    //                budgetLeft = budget.budget - budgetUsed;
                    //                if (budgetLeft >= _amountToSaveInBgtCurr)
                    //                {
                    //                    //budget covers amount
                    //                    //paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_currentRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                    //                    paymentsAmount.Add(new KeyValuePair<decimal, string>(purchases.ConvertAmountToRate(_targetRates, _amountToSaveInBgtCurr, budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key, model.OutcomeInfo_Budget));
                    //                }
                    //                else
                    //                {
                    //                    //budget is not enough to cover amount
                    //                    //response.Type = Attempt_ResponseTypes.Error;
                    //                    //response.Message = "Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")";
                    //                    //response.ObjectID = 0;
                    //                    //return response;
                    //                    throw new Exception("Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")");
                    //                }
                    //                #endregion
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    if (egress.dateSaved.Month != DateTime.Parse(model.OutcomeInfo_DateSaved).Month)//el egreso se intenta cambiar de mes.
                    {
                        var statements = db.tblFundsStatements.Where(m => (m.fundID == egress.fundID || m.fundID == targetFund.fundID) && m.year >= egress.dateSaved.Year && m.month >= egress.dateSaved.Month);
                        if (statements.Count() > 0)
                        {
                            foreach (var statement in statements)
                            {
                                db.DeleteObject(statement);
                            }
                        }
                    }
                    if (egress.tblEgressConcepts.cashTransaction)//if true, affect previous fund
                    {
                        //egress.tblFunds.amount += _amountSaved;//restore amount in fund
                        var prevFund = System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID];
                        System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID] = Convert.ToDecimal(System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID]) + _amountSaved;
                    }
                    if (cashTransaction)//if true, affect target fund
                    {
                        //targetFund.amount -= _amountToSave;
                        System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) - _amountToSave;//esta linea no funciona cuando el fondo origen y el destino son el mismo.
                    }
                    
                    #region "update egress"
                    try
                    {
                        egress.tblFunds.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[egress.tblFunds.fundName + "-" + egress.tblFunds.currencyID]);
                        targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]);
                        var counter = 0;
                        foreach (var charge in paymentsAmount)
                        {
                            if (counter != 0)
                            {
                                egress = new tblEgresses();
                            }
                            egress.terminalID = model.OutcomeInfo_Terminal;
                            egress.egressTypeID = model.OutcomeInfo_EgressType;
                            egress.egressConceptID = model.OutcomeInfo_EgressConcept;
                            egress.pointOfSaleID = model.OutcomeInfo_PointOfSale;
                            egress.opcID = model.OutcomeInfo_Opc != null ? model.OutcomeInfo_Opc != "null" ? long.Parse(model.OutcomeInfo_Opc) : (long?)null : (long?)null;
                            egress.opcOther = model.OutcomeInfo_Opc == "null" ? model.OutcomeInfo_OtherOpc : null;
                            egress.promotionTeamID = model.OutcomeInfo_PromotionTeam != null ? int.Parse(model.OutcomeInfo_PromotionTeam) : (int?)null;
                            egress.budgetID = charge.Value != null && charge.Value != "0" && model.OutcomeInfo_EgressConceptText.ToLower() == "cash gift" ? int.Parse(charge.Value.Split('|')[0]) : (int?)null;
                            egress.chargedToCompanyID = model.OutcomeInfo_ChargedToCompany != "null" ? int.Parse(model.OutcomeInfo_ChargedToCompany) : (int?)null;
                            if (counter > 0 && model.OutcomeInfo_Opc != null && model.OutcomeInfo_Opc != "null" && budgetID > 0)
                            {
                                var opcID = long.Parse(model.OutcomeInfo_Opc);
                                var payingCompanyID = db.tblOPCS.Single(m => m.opcID == opcID).payingCompanyIDX;
                                egress.chargedToCompanyID = payingCompanyID ?? egress.chargedToCompanyID;
                            }
                            egress.customer = model.OutcomeInfo_Customer;
                            egress.invitationNumber = model.OutcomeInfo_Invitation.Trim();
                            egress.amountOfSale = model.OutcomeInfo_EgressConcept == 24 ? model.OutcomeInfo_AmountOfSale : (decimal?)null;//burned invitation
                            egress.currencyOfSaleID = egress.amountOfSale != null ? db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_CurrencyOfSale).currencyID : (int?)null;
                            egress.fundID = targetFund.fundID;
                            egress.amount = charge.Key;//model.OutcomeInfo_Amount;
                            egress.adminFee = model.OutcomeInfo_AdminFee ?? 0;
                            egress.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_Currency).currencyID;
                            egress.locationID = model.OutcomeInfo_Location != "null" ? int.Parse(model.OutcomeInfo_Location) : (int?)null;
                            egress.agentComments = model.OutcomeInfo_AgentComments;
                            egress.dateSaved = model.OutcomeInfo_DateSaved != egress.dateSaved.Date.ToString("yyyy-MM-dd") && model.OutcomeInfo_DateSaved != null ? DateTime.Parse((model.OutcomeInfo_DateSaved + " 23:59:59"), CultureInfo.InvariantCulture) : egress.dateSaved;
                            egress.dateLastModification = DateTime.Now;
                            egress.modifiedByUserID = session.UserID;
                            egress.spiCustomerID = model.CustomerID;
                            egress.spiMarketingProgram = model.MarketingProgram;
                            egress.spiSubdivision = model.Subdivision;
                            egress.spiSource = model.Source;
                            egress.spiOpcID = model.OPCID;
                            egress.frontOfficeGuestID = egress.frontOfficeGuestID == null ? model.FrontOfficeGuestID : egress.frontOfficeGuestID;
                            egress.frontOfficeResortID = egress.frontOfficeResortID == null ? model.FrontOfficeResortID : egress.frontOfficeResortID;
                            egress.spiTourID = model.TourID;
                            if (counter != 0)
                            {
                                egress.savedByUserID = session.UserID;
                                egress.dateLastModification = (DateTime?)null;
                                egress.modifiedByUserID = (Guid?)null;
                                db.tblEgresses.AddObject(egress);
                            }
                            db.SaveChanges();
                            counter++;
                        }
                        System.Web.HttpContext.Current.Application.UnLock();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Egress Updated" + response.Message;
                        response.ObjectID = new { egressID = egress.egressID, date = egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), agent = egress.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + egress.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName, print = (model.OutcomeInfo_SaveAndPrint ? true : false) };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        //WITH AMOUNT ASIGNATIONS ON THE TRY BLOCK, NOT RESTORATION NEEDED
                        //restore previous balance
                        //egress.tblFunds.amount -= _amountSaved;
                        //targetFund.amount += _amountToSave;
                        //targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) + _amountToSave;
                        //db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "There was an error updating egress";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                    #endregion
                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Application.UnLock();
                    response.Type = Attempt_ResponseTypes.Error;
                    //response.Message = "Charge amount exceeds available budget amount($" + budgetLeft + " " + budget.tblCurrencies.currencyCode + ")";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                #region "save"
                var now = DateTime.Now;
                var paymentsAmount = new List<KeyValuePair<decimal, string>>();
                paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, model.OutcomeInfo_Budget));
                model.OutcomeInfo_EgressConceptText = db.tblEgressConcepts.Single(m => m.egressConceptID == model.OutcomeInfo_EgressConcept).egressConcept;

                try
                {
                    if (model.OutcomeInfo_Amount < 0)
                    {
                        throw new Exception("You can't save an egress with negative amount");
                    }
                    if (cashTransaction)
                    {
                        //targetFund.amount -= _amountToSave;
                        System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) - _amountToSave;
                        //db.SaveChanges();
                    }

                    #region "create egress"
                    try
                    {
                        targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]);
                        //************
                        var budgetID = model.OutcomeInfo_Budget != null && model.OutcomeInfo_Budget != "0" ? int.Parse(model.OutcomeInfo_Budget.Split('|')[0]) : 0;
                        if (budgetID != 0)
                        {
                            paymentsAmount.Clear();
                            if (db.tblEgressConcepts.Single(m => m.egressConceptID == model.OutcomeInfo_EgressConcept).allowBudget == true)
                            {
                                var budget = db.tblBudgets.Single(m => m.budgetID == budgetID);
                                var amountToSave = purchases.ConvertAmountToRate(_currentRates, model.OutcomeInfo_Amount, model.OutcomeInfo_Currency, budget.tblCurrencies.currencyCode).Key;
                                if (!budget.perWeek)
                                {
                                    var savedEgresses = db.tblEgresses.Where(m => m.invitationNumber == model.OutcomeInfo_Invitation && m.pointOfSaleID == model.OutcomeInfo_PointOfSale && m.budgetID == budgetID).Select(m => new { m.amount, m.tblCurrencies.currencyCode, budgetCurrency = m.tblBudgets.tblCurrencies.currencyCode, m.dateSaved });
                                    var savedPayments = db.tblPaymentDetails.Where(m => m.tblPurchases.terminalID == _terminalID && m.tblPurchases.pointOfSaleID == model.OutcomeInfo_PointOfSale && m.paymentType == 3 && m.invitation == model.OutcomeInfo_Invitation && m.deletedByUserID == null && m.budgetID == budgetID).Select(m => new { m.amount, m.tblCurrencies.currencyCode, budgetCurrency = m.tblBudgets.tblCurrencies.currencyCode, m.dateSaved });
                                    var savedTransactions = savedEgresses.Count() > 0 ? savedPayments.Count() > 0 ? savedEgresses.Concat(savedPayments) : savedEgresses : savedPayments;
                                    decimal budgetAvailable = 0;

                                    if(savedTransactions.Count() == 0)
                                    {
                                        budgetAvailable = budget.budget;
                                    }
                                    else
                                    {
                                        decimal _budgetUsed = 0;
                                        foreach(var i in savedTransactions)
                                        {
                                            var _rate = purchases.GetExchangeRates(i.dateSaved, _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();
                                            _budgetUsed += purchases.ConvertAmountToRate(_rate, i.amount, i.currencyCode, i.budgetCurrency).Key;
                                        }
                                        budgetAvailable = budget.budget - _budgetUsed;
                                    }
                                    var useBudget = budgetAvailable > 0;
                                    if (useBudget)
                                    {

                                        var toBudget = purchases.ConvertAmountToRate(_currentRates, (amountToSave <= budgetAvailable ? amountToSave : budgetAvailable), budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key;
                                        var toOther = purchases.ConvertAmountToRate(_currentRates, (amountToSave <= budgetAvailable ? 0 : amountToSave - budgetAvailable), budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key;
                                        paymentsAmount.Add(new KeyValuePair<decimal, string>(toBudget, model.OutcomeInfo_Budget));
                                        if (toOther != 0)
                                        {
                                            paymentsAmount.Add(new KeyValuePair<decimal, string>(toOther, null));
                                        }
                                    }
                                    else
                                    {
                                        response.Message = "<br />No Budget Available";
                                        paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, null));
                                    }
                                }
                                else
                                {
                                    var opcID = int.Parse(model.OutcomeInfo_Opc);
                                    var teamID = int.Parse(model.OutcomeInfo_PromotionTeam);
                                    var _budgetUsed = purchases.GetBudgetUsedInWeek(opcID, teamID, now.ToString("yyyy-MM-dd"), _terminalID, model.OutcomeInfo_PointOfSale);
                                    var budgetAvailable = budget.budget - _budgetUsed <= 0 ? 0 : budget.budget - _budgetUsed;

                                    var useBudget = budgetAvailable > 0 ? true : false;
                                    if (useBudget)
                                    {

                                        var toBudget = purchases.ConvertAmountToRate(_currentRates, (amountToSave <= budgetAvailable ? amountToSave : budgetAvailable), budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key;
                                        var toOther = purchases.ConvertAmountToRate(_currentRates, (amountToSave <= budgetAvailable ? 0 : amountToSave - budgetAvailable), budget.tblCurrencies.currencyCode, model.OutcomeInfo_Currency).Key;
                                        paymentsAmount.Add(new KeyValuePair<decimal, string>(toBudget, model.OutcomeInfo_Budget));
                                        if (toOther != 0)
                                        {
                                            paymentsAmount.Add(new KeyValuePair<decimal, string>(toOther, null));
                                        }
                                    }
                                    else
                                    {
                                        paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, null));
                                    }
                                }
                            }
                            else
                            {
                                paymentsAmount.Add(new KeyValuePair<decimal, string>(model.OutcomeInfo_Amount, null));
                            }
                        }
                        //************
                        var counter = 0;
                        foreach (var charge in paymentsAmount)
                        {
                            var _egress = new tblEgresses();
                            _egress.egressTypeID = model.OutcomeInfo_EgressType;
                            _egress.terminalID = model.OutcomeInfo_Terminal;
                            _egress.egressConceptID = model.OutcomeInfo_EgressConcept;
                            _egress.fundID = targetFund.fundID;
                            _egress.pointOfSaleID = model.OutcomeInfo_PointOfSale;
                            _egress.opcID = model.OutcomeInfo_Opc != null ? model.OutcomeInfo_Opc != "null" ? long.Parse(model.OutcomeInfo_Opc) : (long?)null : (long?)null;
                            _egress.opcOther = model.OutcomeInfo_Opc == "null" ? model.OutcomeInfo_OtherOpc : null;
                            _egress.promotionTeamID = model.OutcomeInfo_PromotionTeam != null ? int.Parse(model.OutcomeInfo_PromotionTeam) : (int?)null;
                            _egress.budgetID = charge.Value != null && charge.Value != "0" && model.OutcomeInfo_EgressConceptText.ToLower() == "cash gift" ? int.Parse(charge.Value.Split('|')[0]) : (int?)null;
                            _egress.chargedToCompanyID = model.OutcomeInfo_ChargedToCompany != "null" ? int.Parse(model.OutcomeInfo_ChargedToCompany) : (int?)null;
                            if(counter > 0 && model.OutcomeInfo_Opc != null && model.OutcomeInfo_Opc != "null" && budgetID > 0)
                            {
                                var opcID = long.Parse(model.OutcomeInfo_Opc);
                                var payingCompanyID = db.tblOPCS.Single(m => m.opcID == opcID).payingCompanyIDX;
                                _egress.chargedToCompanyID = payingCompanyID ?? _egress.chargedToCompanyID;
                            }
                            _egress.customer = model.OutcomeInfo_Customer;
                            _egress.invitationNumber = model.OutcomeInfo_Invitation.Trim();
                            _egress.amount = charge.Key;
                            _egress.adminFee = model.OutcomeInfo_AdminFee ?? 0;
                            _egress.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_Currency).currencyID;
                            _egress.locationID = model.OutcomeInfo_Location != "null" ? int.Parse(model.OutcomeInfo_Location) : (int?)null;
                            _egress.dateSaved = now;
                            _egress.savedByUserID = session.UserID;
                            _egress.agentComments = model.OutcomeInfo_AgentComments;
                            _egress.pointOfSaleID = model.OutcomeInfo_PointOfSale;
                            _egress.amountOfSale = model.OutcomeInfo_EgressConcept == 24 ? model.OutcomeInfo_AmountOfSale : (decimal?)null;//burned invitation
                            _egress.currencyOfSaleID = _egress.amountOfSale != null ? db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_CurrencyOfSale).currencyID : (int?)null;
                            _egress.spiCustomerID = model.CustomerID;
                            _egress.spiMarketingProgram = model.MarketingProgram;
                            _egress.spiSubdivision = model.Subdivision;
                            _egress.spiSource = model.Source;
                            _egress.spiOpcID = model.OPCID;
                            _egress.frontOfficeGuestID = model.FrontOfficeGuestID;
                            _egress.frontOfficeResortID = model.FrontOfficeResortID;
                            _egress.spiTourID = model.TourID;
                            db.tblEgresses.AddObject(_egress);
                            db.SaveChanges();

                            response.ObjectID = new
                            {
                                egressID = _egress.egressID,
                                date = _egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                agent = _egress.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + _egress.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName,
                                print = (model.OutcomeInfo_SaveAndPrint ? true : false)
                            };
                            counter++;
                        }
                        System.Web.HttpContext.Current.Application.UnLock();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Egress Saved" + response.Message;
                        return response;
                    }
                    catch
                    {
                        //WITH ASSIGNATION ON TRY BLOCK, NOT RESTORATION NEEDED
                        //if (cashTransaction)//only if changes were made to the fund
                        //{
                        //    //targetFund.amount += _amountToSave;
                        //    targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) + _amountToSave;
                        //    db.SaveChanges();
                        //}
                        throw new Exception("There was an error saving egress");
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Application.UnLock();
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Egress NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        private Object egressLock = new Object();
        private Object incomeLock = new Object();

        //public AttemptResponse SaveEgress(OutcomeInfoModel model)
        //{
        //    ePlatEntities db = new ePlatEntities();
        //    AttemptResponse response = new AttemptResponse();
        //    var cashTransaction = db.tblEgressConcepts.Single(m => m.egressConceptID == model.OutcomeInfo_EgressConcept).cashTransaction;

        //    lock (egressLock)
        //    {
        //        if (model.OutcomeInfo_EgressID != 0)
        //        {
        //            #region "Update Egress"
        //            try
        //            {
        //                var egress = db.tblEgresses.Single(m => m.egressID == model.OutcomeInfo_EgressID);
        //                var date = egress.dateSaved.AddHours(72);
        //                if (model.OutcomeInfo_Amount < 0)
        //                {
        //                    throw new Exception("You can't save an egress with negative amount");
        //                }
        //                if (GeneralFunctions.IsUserInRole("Onsite Reservations Agent", null, true))
        //                {
        //                    if (egress.dateSaved.Date != DateTime.Today.Date)
        //                    {
        //                        throw new Exception("You don't have persmission to update");
        //                    }
        //                }
        //                else
        //                {
        //                    if (DateTime.Now > date)
        //                    {
        //                        throw new Exception("You don't have persmission to update");
        //                    }
        //                }
        //                #region "update fund"
        //                tblFunds fund;
        //                if (db.tblFunds_PointsOfSale.Where(m => m.pointOfSaleID == model.OutcomeInfo_PointOfSale).Count() > 1)
        //                {
        //                    var currencyFunds = db.tblFunds_PointsOfSale.Where(x => x.pointOfSaleID == model.OutcomeInfo_PointOfSale && x.tblFunds.tblCurrencies.currencyCode == model.OutcomeInfo_Currency);
        //                    if (currencyFunds.Count() == 1)
        //                    {
        //                        fund = db.tblFunds.Single(m => m.fundID == (int)currencyFunds.FirstOrDefault().fundID);
        //                    }
        //                    else
        //                    {
        //                        if (model.OutcomeInfo_Fund == 0)
        //                        {
        //                            //por default esta propiedad está en 0 dado que no está en uso
        //                            response.Type = Attempt_ResponseTypes.Warning;
        //                            response.Message = "There is no specific fund for " + model.OutcomeInfo_Currency + " but register was assigned to \"" + egress.tblFunds.fundName + " " + egress.tblFunds.tblCurrencies.currencyCode + "\"";
        //                            var _existingFunds = db.tblFunds_PointsOfSale.Where(m => m.pointOfSaleID == model.OutcomeInfo_PointOfSale).Select(m => new { fundID = m.fundID, currencyCode = m.tblFunds.tblCurrencies.currencyCode, fund = m.tblFunds.fundName });
        //                            var existingFunds = new List<string>();
        //                            foreach (var i in _existingFunds)
        //                            {
        //                                existingFunds.Add(i.fundID + "_" + i.fund + " - " + i.currencyCode);
        //                            }
        //                            response.ObjectID = new { existingFunds = string.Join("|", existingFunds.ToArray()) };
        //                            return response;
        //                        }
        //                        else
        //                        {
        //                            //el modelo traerá el valor seleccionado en el messageBox
        //                            fund = db.tblFunds.Single(m => m.fundID == model.OutcomeInfo_Fund);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    fund = db.tblFunds.Single(m => m.fundID == (int)db.tblFunds_PointsOfSale.FirstOrDefault(x => x.pointOfSaleID == model.OutcomeInfo_PointOfSale).fundID);
        //                }
        //                var previousAmount = fund.amount;
        //                decimal _amount = 0;
        //                if (fund.fundID != egress.fundID)
        //                {
        //                    //restauro la cantidad del fondo inicialmente afectado
        //                    db.tblFunds.Single(m => m.fundID == egress.fundID).amount += ((egress.amount * MasterChartDataModel.Purchases.GetSpecificRate(egress.dateSaved, egress.currencyID.ToString(), egress.terminalID)) / MasterChartDataModel.Purchases.GetSpecificRate(egress.dateSaved, db.tblFunds.Single(m => m.fundID == egress.fundID).currencyID.ToString(), egress.terminalID));
        //                    //obtengo el nuevo monto de acuerdo a la fecha en que se guardó el registro y la nueva moneda
        //                    _amount = (model.OutcomeInfo_Amount * MasterChartDataModel.Purchases.GetSpecificRate(egress.dateSaved, model.OutcomeInfo_Currency, egress.terminalID)) * -1;
        //                }
        //                else
        //                {
        //                    _amount = (egress.amount * MasterChartDataModel.Purchases.GetSpecificRate(egress.dateSaved, egress.currencyID.ToString(), egress.terminalID)) - (model.OutcomeInfo_Amount * MasterChartDataModel.Purchases.GetSpecificRate(egress.dateSaved, model.OutcomeInfo_Currency, egress.terminalID));
        //                }
        //                fund.amount = Math.Round(((fund.amount * MasterChartDataModel.Purchases.GetSpecificRate(egress.dateSaved, fund.currencyID.ToString(), egress.terminalID) + _amount) / MasterChartDataModel.Purchases.GetSpecificRate(egress.dateSaved, fund.currencyID.ToString(), egress.terminalID)), 2, MidpointRounding.AwayFromZero);
        //                #endregion
        //                if (cashTransaction)
        //                {
        //                    db.SaveChanges();
        //                }
        //                #region "update egress"
        //                try
        //                {
        //                    egress.terminalID = model.OutcomeInfo_Terminal;
        //                    egress.egressTypeID = model.OutcomeInfo_EgressType;
        //                    egress.egressConceptID = model.OutcomeInfo_EgressConcept;
        //                    egress.pointOfSaleID = model.OutcomeInfo_PointOfSale;
        //                    egress.opcID = model.OutcomeInfo_Opc != null ? model.OutcomeInfo_Opc != "null" ? long.Parse(model.OutcomeInfo_Opc) : (long?)null : (long?)null;
        //                    egress.opcOther = model.OutcomeInfo_Opc == "null" ? model.OutcomeInfo_OtherOpc : null;
        //                    egress.promotionTeamID = model.OutcomeInfo_PromotionTeam != null ? int.Parse(model.OutcomeInfo_PromotionTeam) : (int?)null;
        //                    egress.chargedToCompanyID = model.OutcomeInfo_ChargedToCompany != "null" ? int.Parse(model.OutcomeInfo_ChargedToCompany) : (int?)null;
        //                    egress.customer = model.OutcomeInfo_Customer;
        //                    egress.invitationNumber = model.OutcomeInfo_Invitation;
        //                    egress.amountOfSale = model.OutcomeInfo_EgressConcept == 24 ? model.OutcomeInfo_AmountOfSale : (decimal?)null;//burned invitation
        //                    egress.currencyOfSaleID = egress.amountOfSale != null ? db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_CurrencyOfSale).currencyID : (int?)null;
        //                    egress.fundID = fund.fundID;
        //                    egress.amount = model.OutcomeInfo_Amount;
        //                    egress.adminFee = model.OutcomeInfo_AdminFee ?? 0;
        //                    egress.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_Currency).currencyID;
        //                    egress.locationID = model.OutcomeInfo_Location != "null" ? int.Parse(model.OutcomeInfo_Location) : (int?)null;
        //                    egress.agentComments = model.OutcomeInfo_AgentComments;
        //                    egress.dateLastModification = DateTime.Now;
        //                    egress.modifiedByUserID = session.UserID;
        //                    db.SaveChanges();
        //                }
        //                catch
        //                {
        //                    if (cashTransaction)
        //                    {
        //                        fund.amount = previousAmount;
        //                        db.SaveChanges();
        //                    }
        //                    throw new Exception("There was an error updating egress");
        //                }
        //                #endregion
        //                response.Type = Attempt_ResponseTypes.Ok;
        //                response.Message = "Egress Updated";
        //                response.ObjectID = new { egressID = egress.egressID, date = egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), agent = egress.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + egress.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName, print = (model.OutcomeInfo_SaveAndPrint ? true : false) };
        //                return response;
        //            }
        //            catch (Exception ex)
        //            {
        //                response.Type = Attempt_ResponseTypes.Error;
        //                response.Message = "Egress NOT Updated";
        //                response.ObjectID = 0;
        //                response.Exception = ex;
        //                return response;
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            #region "Save Egress"
        //            try
        //            {
        //                if (model.OutcomeInfo_Amount < 0)
        //                {
        //                    throw new Exception("You can't save an egress with negative amount");
        //                }
        //                tblFunds fund;
        //                decimal previousAmount;

        //                if (db.tblFunds_PointsOfSale.Where(m => m.pointOfSaleID == model.OutcomeInfo_PointOfSale).Count() > 1)
        //                {
        //                    var currencyFunds = db.tblFunds_PointsOfSale.Where(x => x.pointOfSaleID == model.OutcomeInfo_PointOfSale && x.tblFunds.tblCurrencies.currencyCode == model.OutcomeInfo_Currency);
        //                    if (currencyFunds.Count() == 1)
        //                    {
        //                        fund = db.tblFunds.Single(m => m.fundID == (int)currencyFunds.FirstOrDefault().fundID);
        //                    }
        //                    else
        //                    {
        //                        if (model.OutcomeInfo_Fund == 0)
        //                        {
        //                            //por default esta propiedad está en 0 dado que no está en uso
        //                            response.Type = Attempt_ResponseTypes.Warning;
        //                            response.Message = "There is no specific fund for " + model.OutcomeInfo_Currency;
        //                            var _existingFunds = db.tblFunds_PointsOfSale.Where(m => m.pointOfSaleID == model.OutcomeInfo_PointOfSale).Select(m => new { fundID = m.fundID, currencyCode = m.tblFunds.tblCurrencies.currencyCode, fund = m.tblFunds.fundName });
        //                            var existingFunds = new List<string>();
        //                            foreach (var i in _existingFunds)
        //                            {
        //                                existingFunds.Add(i.fundID + "_" + i.fund + " - " + i.currencyCode);
        //                            }
        //                            response.ObjectID = new { existingFunds = string.Join("|", existingFunds.ToArray()) };
        //                            return response;
        //                        }
        //                        else
        //                        {
        //                            //el modelo traerá el valor seleccionado en el messageBox
        //                            fund = db.tblFunds.Single(m => m.fundID == model.OutcomeInfo_Fund);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    fund = db.tblFunds.Single(m => m.fundID == (int)db.tblFunds_PointsOfSale.FirstOrDefault(x => x.pointOfSaleID == model.OutcomeInfo_PointOfSale).fundID);
        //                }
        //                previousAmount = fund.amount;
        //                fund.amount = Math.Round((((fund.amount * MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, fund.currencyID.ToString(), model.OutcomeInfo_Terminal)) - (model.OutcomeInfo_Amount * MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, model.OutcomeInfo_Currency, model.OutcomeInfo_Terminal))) / MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, fund.currencyID.ToString(), model.OutcomeInfo_Terminal)), 2, MidpointRounding.AwayFromZero);
        //                if (cashTransaction)
        //                {
        //                    db.SaveChanges();
        //                }
        //                #region "create egress"
        //                var egress = new tblEgresses();
        //                try
        //                {
        //                    egress.egressTypeID = model.OutcomeInfo_EgressType;
        //                    egress.terminalID = model.OutcomeInfo_Terminal;
        //                    egress.egressConceptID = model.OutcomeInfo_EgressConcept;
        //                    egress.fundID = fund.fundID;
        //                    egress.pointOfSaleID = model.OutcomeInfo_PointOfSale;
        //                    egress.opcID = model.OutcomeInfo_Opc != null ? model.OutcomeInfo_Opc != "null" ? long.Parse(model.OutcomeInfo_Opc) : (long?)null : (long?)null;
        //                    egress.opcOther = model.OutcomeInfo_Opc == "null" ? model.OutcomeInfo_OtherOpc : null;
        //                    egress.promotionTeamID = model.OutcomeInfo_PromotionTeam != null ? int.Parse(model.OutcomeInfo_PromotionTeam) : (int?)null;
        //                    egress.chargedToCompanyID = model.OutcomeInfo_ChargedToCompany != "null" ? int.Parse(model.OutcomeInfo_ChargedToCompany) : (int?)null;
        //                    egress.customer = model.OutcomeInfo_Customer;
        //                    egress.invitationNumber = model.OutcomeInfo_Invitation;
        //                    egress.amount = model.OutcomeInfo_Amount;
        //                    egress.adminFee = model.OutcomeInfo_AdminFee ?? 0;
        //                    egress.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_Currency).currencyID;
        //                    egress.locationID = model.OutcomeInfo_Location != "null" ? int.Parse(model.OutcomeInfo_Location) : (int?)null;
        //                    egress.dateSaved = DateTime.Now;
        //                    egress.savedByUserID = session.UserID;
        //                    egress.agentComments = model.OutcomeInfo_AgentComments;
        //                    egress.pointOfSaleID = model.OutcomeInfo_PointOfSale;
        //                    egress.amountOfSale = model.OutcomeInfo_EgressConcept == 24 ? model.OutcomeInfo_AmountOfSale : (decimal?)null;//burned invitation
        //                    egress.currencyOfSaleID = egress.amountOfSale != null ? db.tblCurrencies.Single(m => m.currencyCode == model.OutcomeInfo_CurrencyOfSale).currencyID : (int?)null;
        //                    db.tblEgresses.AddObject(egress);
        //                    db.SaveChanges();
        //                }
        //                catch
        //                {
        //                    if (cashTransaction)//only if changes were made to the fund
        //                    {
        //                        fund.amount = previousAmount;
        //                        db.SaveChanges();
        //                    }
        //                    throw new Exception("There was an error saving egress");
        //                }
        //                #endregion
        //                response.Type = Attempt_ResponseTypes.Ok;
        //                response.Message = "Egress Saved";
        //                response.ObjectID = new { egressID = egress.egressID, date = egress.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), agent = egress.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + egress.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName, print = (model.OutcomeInfo_SaveAndPrint ? true : false) };
        //                return response;
        //            }
        //            catch (Exception ex)
        //            {
        //                response.Type = Attempt_ResponseTypes.Error;
        //                response.Message = "Egress NOT Saved";
        //                response.ObjectID = 0;
        //                response.Exception = ex;
        //                return response;
        //            }
        //            #endregion
        //        }
        //    }
        //}

        public AttemptResponse DeleteEgress(long OutcomeInfo_EgressID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var _row = db.tblEgresses.Single(m => m.egressID == OutcomeInfo_EgressID);
                var fund = db.tblFunds.Single(m => m.fundID == _row.fundID);
                fund.amount = Math.Round((((fund.amount * MasterChartDataModel.Purchases.GetSpecificRate(_row.dateSaved, fund.currencyID.ToString(), _row.terminalID, _row.pointOfSaleID)) + (_row.amount * MasterChartDataModel.Purchases.GetSpecificRate(_row.dateSaved, _row.currencyID.ToString(), _row.terminalID, _row.pointOfSaleID))) / MasterChartDataModel.Purchases.GetSpecificRate(_row.dateSaved, fund.currencyID.ToString(), _row.terminalID, _row.pointOfSaleID)), 2, MidpointRounding.AwayFromZero);
                db.tblEgresses.DeleteObject(_row);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Egress Deleted";
                response.ObjectID = _row.egressID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Egress NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public List<IncomeInfoModel> SearchIncomes(IncomeSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<IncomeInfoModel>();

            var iDate = model.IncomeSearch_I_Date != null ? DateTime.Parse(model.IncomeSearch_I_Date, CultureInfo.InvariantCulture) : (DateTime?)null;
            var fDate = model.IncomeSearch_F_Date != null ? DateTime.Parse(model.IncomeSearch_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
            var companies = model.IncomeSearch_Company != null ? model.IncomeSearch_Company.Select(m => int.Parse(m)).ToArray() : MasterChartDataModel.LeadsCatalogs.FillDrpCompaniesPerSelectedTerminals().Select(m => int.Parse(m.Value)).ToArray();
            var users = model.IncomeSearch_Receiver != null ? model.IncomeSearch_Receiver.Select(m => Guid.Parse(m)).ToArray() : new Guid[] { };

            var query = from i in db.tblIncomes
                        where ((i.dateSaved >= iDate && i.dateSaved <= fDate) || iDate == null)
                        && (users.Contains(i.receiverUserID) || users.Count() == 0)
                        && companies.Contains(i.tblFunds.companyID)
                        //&& (companies.Contains(i.tblFunds.companyID) || companies.Count() == 0)
                        select new
                        {
                            i.incomeID,
                            i.incomeConceptID,
                            i.dateSaved,
                            i.tblFunds.tblCompanies.company,
                            i.amount,
                            i.tblCurrencies.currencyCode,
                            i.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName,
                            i.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName
                        };

            foreach (var i in query)
            {
                list.Add(new IncomeInfoModel()
                {
                    IncomeInfo_IncomeID = i.incomeID,
                    IncomeInfo_DateSaved = i.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    IncomeInfo_Company = i.company,
                    IncomeInfo_Amount = i.amount,
                    IncomeInfo_ReceiverUser = i.firstName + " " + i.lastName,
                    IncomeInfo_Currency = i.currencyCode,
                    IncomeInfo_IncomeConceptText = GeneralFunctions.IncomeTypes.Single(m => m.Key == i.incomeConceptID.ToString()).Value
                });
            }

            return list;
        }

        public IncomeInfoModel GetIncomeInfo(long IncomeInfo_IncomeID)
        {
            ePlatEntities db = new ePlatEntities();
            var model = new IncomeInfoModel();
            var query = db.tblIncomes.Single(m => m.incomeID == IncomeInfo_IncomeID);

            model.IncomeInfo_IncomeID = query.incomeID;
            model.IncomeInfo_IncomeConcept = query.incomeConceptID;
            model.IncomeInfo_Company = query.tblFunds.companyID.ToString();
            model.IncomeInfo_Amount = query.amount;
            model.IncomeInfo_Currency = query.tblCurrencies.currencyCode;
            model.IncomeInfo_Fund = query.fundID;

            return model;
        }

        public AttemptResponse SaveIncome(IncomeInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var targetFund = db.tblFunds.Single(m => m.fundID == model.IncomeInfo_Fund);
            var _terminalID = targetFund.tblCompanies.tblTerminals.FirstOrDefault().terminalID;
            var _targetRates = new MasterChartDataModel.Purchases().GetExchangeRates(DateTime.Now, _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();
            var _amountToSave = new MasterChartDataModel.Purchases().ConvertAmountToRate(_targetRates, model.IncomeInfo_Amount, model.IncomeInfo_Currency, targetFund.tblCurrencies.currencyCode).Key;
            //var targetRate = MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, targetFund.currencyID.ToString(), _terminalID);
            var modelFundID = db.tblFunds.Single(m => m.fundID == model.IncomeInfo_Fund).fundID;
            if (model.IncomeInfo_IncomeID != 0)
            {
                #region "update"
                try
                {
                    var income = db.tblIncomes.Single(m => m.incomeID == model.IncomeInfo_IncomeID);
                    var _incomeFund = income.tblFunds;
                    var _incomeRates = new MasterChartDataModel.Purchases().GetExchangeRates(income.dateSaved, _terminalID, 0, true).Replace(" ", string.Empty).Split(',').ToList();
                    var _amountSaved = new MasterChartDataModel.Purchases().ConvertAmountToRate(_incomeRates, income.amount, income.tblCurrencies.currencyCode, income.tblFunds.tblCurrencies.currencyCode).Key;
                    var date = income.dateSaved.AddHours(72);
                    #region "validate update is allowed"
                    if (GeneralFunctions.IsUserInRole("Onsite Reservations Agent", null, true))
                    {
                        if (income.dateSaved.Date != DateTime.Today.Date)
                        {
                            throw new Exception("You don't have permission to update");
                        }
                    }
                    else
                    {
                        if (DateTime.Now > date && !GeneralFunctions.IsUserInRole("Administrator") && !GeneralFunctions.IsUserInRole("Comptroller") && !GeneralFunctions.IsUserInRole("Comptroller VEX") && !GeneralFunctions.IsUserInRole("Department Administrator"))
                        {
                            throw new Exception("You don't have permission to update after 72 hrs");
                        }
                    }
                    if (model.IncomeInfo_Amount < 0)
                    {
                        throw new Exception("You can't save an income with negative amount");
                    }
                    #endregion
                    ////
                    System.Web.HttpContext.Current.Application.Lock();
                    //var savedRate = MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, income.currencyID.ToString(), _terminalID);

                    if (Convert.ToDecimal(System.Web.HttpContext.Current.Application[income.tblFunds.fundName + "-" + income.tblFunds.currencyID]) == 0)
                    {
                        System.Web.HttpContext.Current.Application[income.tblFunds.fundName + "-" + income.tblFunds.currencyID] = income.tblFunds.amount;
                    }
                    if (Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) == 0)
                    {
                        System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = targetFund.amount;
                    }
                    ////
                    System.Web.HttpContext.Current.Application[income.tblFunds.fundName + "-" + income.tblFunds.currencyID] = Convert.ToDecimal(System.Web.HttpContext.Current.Application[income.tblFunds.fundName + "-" + income.tblFunds.currencyID]) - _amountSaved;
                    System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) + _amountToSave;

                    income.tblFunds.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[income.tblFunds.fundName + "-" + income.tblFunds.currencyID]);
                    targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]);
                    income.incomeConceptID = model.IncomeInfo_IncomeConcept;
                    income.fundID = model.IncomeInfo_Fund;
                    income.amount = model.IncomeInfo_Amount;
                    income.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.IncomeInfo_Currency).currencyID;
                    income.dateLastModification = DateTime.Now;
                    income.modifiedByUserID = session.UserID;
                    db.SaveChanges();

                    System.Web.HttpContext.Current.Application.UnLock();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Income Updated";
                    response.ObjectID = new { incomeID = income.incomeID, date = income.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), receiver = (income.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + income.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName) };
                    return response;
                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Application.UnLock();
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Income NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "save"
                try
                {
                    System.Web.HttpContext.Current.Application.Lock();
                    if (Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) == 0)
                    {
                        System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = targetFund.amount;
                    }
                    var income = new tblIncomes();
                    income.incomeConceptID = model.IncomeInfo_IncomeConcept;
                    var company = int.Parse(model.IncomeInfo_Company);
                    income.fundID = model.IncomeInfo_Fund;
                    income.amount = model.IncomeInfo_Amount;
                    income.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.IncomeInfo_Currency).currencyID;
                    income.receiverUserID = session.UserID;
                    income.dateSaved = DateTime.Now;
                    income.savedByUserID = session.UserID;
                    db.tblIncomes.AddObject(income);
                    System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID] = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]) + _amountToSave;
                    targetFund.amount = Convert.ToDecimal(System.Web.HttpContext.Current.Application[targetFund.fundName + "-" + targetFund.currencyID]);
                    //var modelFund = db.tblFunds.Single(m => m.fundID == model.IncomeInfo_Fund);
                    //modelFund.amount = Math.Round((((modelFund.amount * MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, modelFund.currencyID.ToString(), modelFund.tblCompanies.tblTerminals.FirstOrDefault().terminalID)) + (model.IncomeInfo_Amount * MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, model.IncomeInfo_Currency, modelFund.tblCompanies.tblTerminals.FirstOrDefault().terminalID))) / MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, modelFund.currencyID.ToString(), modelFund.tblCompanies.tblTerminals.FirstOrDefault().terminalID)), 2, MidpointRounding.AwayFromZero);
                    db.SaveChanges();
                    System.Web.HttpContext.Current.Application.UnLock();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Income Saved";
                    response.ObjectID = new { incomeID = income.incomeID, date = income.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), receiver = (income.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + income.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName) };
                    return response;
                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Application.UnLock();
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Income NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public AttemptResponse ResetVarApp(int fundID, decimal amount)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            System.Web.HttpContext.Current.Application.Lock();
            var fund = db.tblFunds.Single(m => m.fundID == fundID);
            fund.amount = amount;
            System.Web.HttpContext.Current.Application[fund.fundName + "-" + fund.currencyID] = amount;
            db.SaveChanges();
            System.Web.HttpContext.Current.Application.UnLock();
            response.Type = Attempt_ResponseTypes.Ok;
            response.Message = "Variable Successfully Reseted";
            response.ObjectID = 0;
            return response;
        }

        public AttemptResponse _SaveIncome(IncomeInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            lock (incomeLock)
            {
                var modelFundID = db.tblFunds.Single(m => m.fundID == model.IncomeInfo_Fund).fundID;
                if (model.IncomeInfo_IncomeID != 0)
                {
                    #region "update"
                    try
                    {
                        var income = db.tblIncomes.Single(m => m.incomeID == model.IncomeInfo_IncomeID);
                        var date = income.dateSaved.AddHours(72);

                        if (GeneralFunctions.IsUserInRole("Onsite Reservations Agent", null, true))
                        {
                            if (income.dateSaved.Date != DateTime.Today.Date)
                            {
                                throw new Exception("You don't have permission to update");
                            }
                        }
                        else
                        {
                            if (DateTime.Now > date)
                            {
                                throw new Exception("You don't have permission to update");
                            }
                        }
                        income.incomeConceptID = model.IncomeInfo_IncomeConcept;
                        var company = int.Parse(model.IncomeInfo_Company);
                        decimal _amount = 0;
                        if (modelFundID != income.fundID)
                        {
                            //restauro el monto del fondo principalmente afectado
                            //db.tblFunds.Single(m => m.fundID == income.fundID).amount -= ((income.amount * MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, income.currencyID.ToString(), income.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID)) / MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, income.currencyID.ToString(), income.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID));
                            db.tblFunds.Single(m => m.fundID == income.fundID).amount -= ((income.amount * MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, income.currencyID.ToString(), income.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID)) / MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, db.tblFunds.Single(m => m.fundID == income.fundID).currencyID.ToString(), income.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID));

                            _amount = (model.IncomeInfo_Amount * MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, model.IncomeInfo_Currency, income.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID)) * -1;
                        }
                        else
                        {
                            _amount = (income.amount * MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, income.currencyID.ToString(), income.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID)) - (model.IncomeInfo_Amount * MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, model.IncomeInfo_Currency, income.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID));
                        }
                        var modelFund = db.tblFunds.Single(m => m.fundID == model.IncomeInfo_Fund);
                        modelFund.amount = Math.Round(((modelFund.amount * MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, modelFund.currencyID.ToString(), income.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID) - _amount) / MasterChartDataModel.Purchases.GetSpecificRate(income.dateSaved, modelFund.currencyID.ToString(), income.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID)), 2, MidpointRounding.AwayFromZero);

                        if (model.IncomeInfo_Amount < 0)
                        {
                            throw new Exception("You can't save an income with negative amount");
                        }
                        income.fundID = model.IncomeInfo_Fund;
                        income.amount = model.IncomeInfo_Amount;
                        income.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.IncomeInfo_Currency).currencyID;
                        income.dateLastModification = DateTime.Now;
                        income.modifiedByUserID = session.UserID;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Income Updated";
                        response.ObjectID = new { incomeID = income.incomeID, date = income.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), receiver = (income.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + income.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName) };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Income NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
                else
                {
                    #region "save"
                    try
                    {
                        System.Web.HttpContext.Current.Application.Lock();
                        var income = new tblIncomes();
                        income.incomeConceptID = model.IncomeInfo_IncomeConcept;
                        var company = int.Parse(model.IncomeInfo_Company);
                        income.fundID = model.IncomeInfo_Fund;
                        income.amount = model.IncomeInfo_Amount;
                        income.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.IncomeInfo_Currency).currencyID;
                        income.receiverUserID = session.UserID;
                        income.dateSaved = DateTime.Now;
                        income.savedByUserID = session.UserID;
                        db.tblIncomes.AddObject(income);

                        var modelFund = db.tblFunds.Single(m => m.fundID == model.IncomeInfo_Fund);
                        modelFund.amount = Math.Round((((modelFund.amount * MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, modelFund.currencyID.ToString(), modelFund.tblCompanies.tblTerminals.FirstOrDefault().terminalID)) + (model.IncomeInfo_Amount * MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, model.IncomeInfo_Currency, modelFund.tblCompanies.tblTerminals.FirstOrDefault().terminalID))) / MasterChartDataModel.Purchases.GetSpecificRate(DateTime.Now, modelFund.currencyID.ToString(), modelFund.tblCompanies.tblTerminals.FirstOrDefault().terminalID)), 2, MidpointRounding.AwayFromZero);
                        db.SaveChanges();
                        System.Web.HttpContext.Current.Application.UnLock();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Income Saved";
                        response.ObjectID = new { incomeID = income.incomeID, date = income.dateSaved.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), receiver = (income.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + income.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName) };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        System.Web.HttpContext.Current.Application.UnLock();
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Income NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
            }
        }

        public AttemptResponse DeleteIncome(long IncomeInfo_IncomeID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var _row = db.tblIncomes.Single(m => m.incomeID == IncomeInfo_IncomeID);
                var fund = db.tblFunds.Single(m => m.fundID == _row.fundID);
                fund.amount = Math.Round((((fund.amount * MasterChartDataModel.Purchases.GetSpecificRate(_row.dateSaved, fund.currencyID.ToString(), _row.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID)) - (_row.amount * MasterChartDataModel.Purchases.GetSpecificRate(_row.dateSaved, _row.currencyID.ToString(), _row.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID))) / MasterChartDataModel.Purchases.GetSpecificRate(_row.dateSaved, fund.currencyID.ToString(), _row.tblFunds.tblCompanies.tblTerminals.FirstOrDefault().terminalID)), 2, MidpointRounding.AwayFromZero);
                db.tblIncomes.DeleteObject(_row);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Income Deleted";
                response.ObjectID = IncomeInfo_IncomeID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Income NOT Deleted";
                response.Exception = ex;
                response.ObjectID = 0;
                return response;
            }
        }

        public static DependantFields GetDependantEgressConcepts()
        {
            ePlatEntities db = new ePlatEntities();
            DependantFields df = new DependantFields();

            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            df.Fields = new List<DependantFields.DependantField>();


            //Terminal > EgressTypes
            DependantFields.DependantField EgressTypes = new DependantFields.DependantField();
            EgressTypes.Field = "OutcomeInfo_EgressType";
            EgressTypes.ParentField = "OutcomeInfo_Terminal";
            EgressTypes.Values = new List<DependantFields.FieldValue>();

            var queryTypes = GeneralFunctions.EgressTypes.Where(m => m.Terminals.Intersect(terminals).Count() > 0).OrderBy(m => m.Key);

            foreach (var i in terminals)
            {
                foreach (var type in queryTypes.Where(m => m.Terminals.Contains(i)))
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = i;
                    val.Value = type.Key;
                    val.Text = type.Value;
                    EgressTypes.Values.Add(val);
                }
            }

            DependantFields.FieldValue typeDefault = new DependantFields.FieldValue();
            typeDefault.ParentValue = null;
            typeDefault.Value = "";
            typeDefault.Text = "--Select One--";
            EgressTypes.Values.Insert(0, typeDefault);

            df.Fields.Add(EgressTypes);

            //EgressType > EgressConcepts
            DependantFields.DependantField EgressConcepts = new DependantFields.DependantField();
            EgressConcepts.Field = "OutcomeInfo_EgressConcept";
            EgressConcepts.ParentField = "OutcomeInfo_EgressType";
            EgressConcepts.GrandParentField = "OutcomeInfo_Terminal";//mike
            EgressConcepts.Values = new List<DependantFields.FieldValue>();

            var queryConcepts = from concept in db.tblEgressConcepts
                                join terminal in db.tblTerminals on concept.companyID equals terminal.companyID
                                where terminals.Contains(terminal.terminalID)
                                select new
                                {
                                    terminal.terminalID,
                                    concept.egressConceptID,
                                    concept.egressConcept,
                                    concept.egressTypeID
                                };

            foreach (var i in queryConcepts)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.GrandParentValue = i.terminalID;//mike
                val.ParentValue = i.egressTypeID;
                val.Value = i.egressConceptID.ToString();
                val.Text = i.egressConcept;
                EgressConcepts.Values.Add(val);
            }

            DependantFields.FieldValue conceptDefault = new DependantFields.FieldValue();
            conceptDefault.ParentValue = null;
            conceptDefault.Value = "";
            conceptDefault.Text = "--Select One--";
            EgressConcepts.Values.Insert(0, conceptDefault);

            df.Fields.Add(EgressConcepts);

            return df;
        }

        public static DependantFields _GetDependantEgressConcepts()
        {
            ePlatEntities db = new ePlatEntities();
            DependantFields df = new DependantFields();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            df.Fields = new List<DependantFields.DependantField>();


            //Terminal > EgressTypes
            DependantFields.DependantField EgressTypes = new DependantFields.DependantField();
            EgressTypes.Field = "OutcomeInfo_EgressType";
            EgressTypes.ParentField = "OutcomeInfo_Terminal";
            EgressTypes.Values = new List<DependantFields.FieldValue>();

            var queryTypes = GeneralFunctions.EgressTypes.Where(m => m.Terminals.Intersect(terminals).Count() > 0).OrderBy(m => m.Key);

            foreach (var i in terminals)
            {
                foreach (var type in queryTypes.Where(m => m.Terminals.Contains(i)))
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = i;
                    val.Value = type.Key;
                    val.Text = type.Value;
                    EgressTypes.Values.Add(val);
                }
            }

            //foreach (var i in queryTypes)
            //{
            //    DependantFields.FieldValue val = new DependantFields.FieldValue();
            //    val.ParentValue = terminalID;
            //    val.Value = i.Key.ToString();
            //    val.Text = i.Value;
            //    EgressTypes.Values.Add(val);
            //}

            DependantFields.FieldValue typeDefault = new DependantFields.FieldValue();
            typeDefault.ParentValue = null;
            typeDefault.Value = "";
            typeDefault.Text = "--Select One--";
            EgressTypes.Values.Insert(0, typeDefault);

            df.Fields.Add(EgressTypes);

            //EgressType > EgressConcepts
            DependantFields.DependantField EgressConcepts = new DependantFields.DependantField();
            EgressConcepts.Field = "OutcomeInfo_EgressConcept";
            EgressConcepts.ParentField = "OutcomeInfo_EgressType";
            EgressConcepts.Values = new List<DependantFields.FieldValue>();

            var queryConcepts = from concept in db.tblEgressConcepts
                                join terminal in db.tblTerminals on concept.companyID equals terminal.companyID
                                where terminals.Contains(terminal.terminalID)
                                //where terminal.terminalID == terminalID
                                select new
                                {
                                    terminal.terminalID,
                                    concept.egressConceptID,
                                    concept.egressConcept,
                                    concept.egressTypeID
                                };

            foreach (var i in queryConcepts)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = i.egressTypeID;
                val.Value = i.egressConceptID.ToString();
                val.Text = i.egressConcept;
                EgressConcepts.Values.Add(val);
            }

            DependantFields.FieldValue conceptDefault = new DependantFields.FieldValue();
            conceptDefault.ParentValue = null;
            conceptDefault.Value = "";
            conceptDefault.Text = "--Select One--";
            EgressConcepts.Values.Insert(0, conceptDefault);

            df.Fields.Add(EgressConcepts);

            return df;
        }

        public static bool ConceptAllowsBudget(long egressConceptID)
        {
            ePlatEntities db = new ePlatEntities();
            var query = db.tblEgressConcepts.Single(m => m.egressConceptID == egressConceptID).allowBudget ?? false;
            return query;
        }

        //public List<SelectListItem> GetDDLData(string itemType, string itemID)
        public object GetDDLData(string itemType, string itemID)
        {
            var list = new List<SelectListItem>();
            switch (itemType)
            {
                case "selectedTerminals":
                    {
                        list = TerminalDataModel.GetActiveTerminalsList();
                        break;
                    }
                case "location":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpLocationsPerCurrentTerminals(long.Parse(itemID));
                        if (itemID != "0")
                        {
                            list.Insert(0, ListItems.NotSet("--Unknown--"));
                        }
                        else
                        {
                            list.Insert(0, ListItems.Default("--Select Terminal--"));
                        }
                        break;
                    }
                case "opcsPerSelectedTerminals":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpOPC();
                        //list.Insert(list.Count(), ListItems.NotSet("--Not Registered--"));
                        break;
                    }
                case "opcs":
                    {
                        //list = MasterChartDataModel.LeadsCatalogs.FillDrpOPC();
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpAllOPCs(null,null);
                        var privileges = AdminDataModel.GetViewPrivileges(10985);
                        //if (privileges.FirstOrDefault(m => m.Component == "PurchasePayment_OPC") == null || privileges.FirstOrDefault(m => m.Component == "PurchasePayment_OPC").Create == null || privileges.FirstOrDefault(m => m.Component == "PurchasePayment_OPC").Create)
                        if (privileges.FirstOrDefault(m => m.Component == "OutcomeInfo_Opc").Create)
                        {
                            list.Insert(list.Count(), ListItems.NotSet("--Not Registered--"));
                        }
                        list.Insert(0, ListItems.Default("--Select One--", ""));
                        break;
                    }
                case "opc":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpCompaniesPerOPC(itemID);
                        break;
                    }
                case "mktCompaniesPerTerminals":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpMarketingCompaniesPerTerminals(itemID != "0" ? itemID : null);
                        break;
                    }
                case "opcTeam":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpPromotionTeamsPerOPC(itemID);
                        break;
                    }
                case "companiesPerSelectedTerminals":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpCompaniesPerSelectedTerminals();
                        break;
                    }
                case "fundsPerSelectedTerminals":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpFundsPerSelectedTerminals(null, true);
                        break;
                    }
                case "pointOfSale":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(long.Parse(itemID), true);
                        break;
                    }
                case "pointsOfSale":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(true);
                        break;
                    }
                case "agents":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.ListWorkGroupUsers();
                        //list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrentWorkGroupAgents(true,true);
                        break;
                    }
                case "conceptsPerTerminal":
                    {
                        if (itemID.IndexOf('|') != -1)
                        {
                            list = MasterChartDataModel.LeadsCatalogs.FillDrpEgressConcepts(long.Parse(itemID.Split('|')[0]), int.Parse(itemID.Split('|')[1])).OrderByDescending(m => m.Text).ToList();
                            break;
                        }
                        else
                        {
                            return MasterChartDataModel.LeadsCatalogs.FillDrpEgressConcepts(long.Parse(itemID)).OrderByDescending(m => m.Text).ToList();
                        }
                    }
                case "paymentTypesPerTerminal":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpEgressTypes(long.Parse(itemID));
                        break;
                    }
            }
            return list;
        }
    }
}
