using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;


namespace ePlatBack.Models.DataModels
{
    public class CatalogsDataModel
    {
        public static UserSession session = new UserSession();
        public static string Capitals(string text)
        {
            var tag = "";
            foreach (char c in text)
            {
                if (char.IsUpper(c))
                    tag += " " + c;
                else
                    tag += c;
            }
            tag = tag.Substring(0, 1).ToUpper() + tag.Substring(1, tag.Length - 1);
            return tag;
        }
        public class AccountingAccounts
        {
            public List<AccountingAccountsModel.AccountingAccountSummary> GetAccountinAccountsSummary(AccountingAccountsModel.SearchAccountingAccountsModel model)
            {
                List<AccountingAccountsModel.AccountingAccountSummary> list = new List<AccountingAccountsModel.AccountingAccountSummary>();
                ePlatEntities db = new ePlatEntities();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var companies = model.SearchAccountingAccount_Companies != null ? model.SearchAccountingAccount_Companies.Select(m => int.Parse(m)).ToArray() : db.tblTerminals_Companies.Where(m => terminals.Contains(m.terminalID)).Select(m => (int)m.companyID).ToArray();

                var AccountingAccounts = (from aa in db.tblAccountingAccounts
                                          where companies.Contains(aa.companyID)
                                          select new
                                          {
                                              AccountName = aa.accountName,
                                              Type = aa.accountType,
                                              PriceTypeID = aa.priceTypeID
                                          });

                var AccAccSummary = (from a in AccountingAccounts
                                     orderby a.AccountName
                                     select new
                                     {
                                         a.AccountName,
                                         Income = AccountingAccounts.Count(x => x.Type == true && x.AccountName == a.AccountName),
                                         Outcome = AccountingAccounts.Count(x => x.Type == false && x.AccountName == a.AccountName)
                                     }).Distinct();

                ReportDataModel rdm = new ReportDataModel();
                List<PriceType> priceTypes = rdm.GetListOfPriceTypes(terminals.FirstOrDefault(), false);
                int priceTypesCount = priceTypes.Count();
                foreach (var accacc in AccAccSummary)
                {
                    AccountingAccountsModel.AccountingAccountSummary newSummary = new AccountingAccountsModel.AccountingAccountSummary();
                    newSummary.AccountingAccountName = accacc.AccountName;
                    newSummary.IncomeRegisteredAccounts = accacc.Income;
                    newSummary.OutcomeRegisteredAccounts = accacc.Outcome;
                    newSummary.NumberOfPriceTypes = priceTypesCount;
                    list.Add(newSummary);
                }

                return list;
            }

            public List<AccountingAccountsModel.AccountingAccountInfoModel> SearchAccountingAccounts(AccountingAccountsModel.SearchAccountingAccountsModel model)
            {
                List<AccountingAccountsModel.AccountingAccountInfoModel> list = new List<AccountingAccountsModel.AccountingAccountInfoModel>();
                ePlatEntities db = new ePlatEntities();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var companies = model.SearchAccountingAccount_Companies != null ? model.SearchAccountingAccount_Companies.Select(m => int.Parse(m)).ToArray() : db.tblTerminals_Companies.Where(m => terminals.Contains(m.terminalID)).Select(m => (int)m.companyID).ToArray();
                bool[] types = model.SearchAccountingAccount_AccountType != null ? model.SearchAccountingAccount_AccountType : new bool[] { true, false };

                var query = from aa in db.tblAccountingAccounts
                            where (aa.account.Contains(model.SearchAccountingAccount_AccountingAccount) || model.SearchAccountingAccount_AccountingAccount == null)
                            && (aa.accountName.Contains(model.SearchAccountingAccount_AccountingAccountName) || model.SearchAccountingAccount_AccountingAccountName == null)
                            && companies.Contains(aa.companyID)
                            && types.Contains(aa.accountType)
                            select aa;

                foreach (var i in query.OrderBy(m => m.account))
                {
                    list.Add(new AccountingAccountsModel.AccountingAccountInfoModel()
                    {
                        AccountingAccountInfo_AccountingAccountID = i.accountingAccountID,
                        AccountingAccountInfo_Account = i.account,
                        AccountingAccountInfo_AccountName = i.accountName,
                        AccountingAccountInfo_AccountTypeText = (i.accountType ? "Income" : "Outcome"),
                        AccountingAccountInfo_Company = i.companyID,
                        AccountingAccountInfo_CompanyText = i.tblCompanies.company,
                        AccountingAccountInfo_PriceTypeText = i.priceTypeID != null ? i.tblPriceTypes.priceType : "All"
                    });
                }
                return list;
            }

            public AttemptResponse SaveAccountingAccount(AccountingAccountsModel.AccountingAccountInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.AccountingAccountInfo_AccountingAccountID != 0)
                {
                    #region "update"
                    try
                    {
                        var query = db.tblAccountingAccounts.Single(m => m.accountingAccountID == model.AccountingAccountInfo_AccountingAccountID);
                        query.account = model.AccountingAccountInfo_Account;
                        query.accountName = model.AccountingAccountInfo_AccountName;
                        query.companyID = model.AccountingAccountInfo_Company;
                        //query.priceTypeID = model.AccountingAccountInfo_PriceType;
                        query.priceTypeID = model.AccountingAccountInfo_PriceType.FirstOrDefault();
                        query.articleMXN = model.AccountingAccountInfo_ArticleMXN;
                        query.articleUSD = model.AccountingAccountInfo_ArticleUSD;
                        query.accountType = model.AccountingAccountInfo_AccountType;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Accounting Account Updated";
                        response.ObjectID = query.accountingAccountID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Accounting Account NOT Updated";
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
                        model.AccountingAccountInfo_PriceType = model.AccountingAccountInfo_PriceType != null ? model.AccountingAccountInfo_PriceType : PriceDataModel.PricesCatalogs.FillDrpPriceTypes().Select(m => (int?)int.Parse(m.Value)).ToArray();
                        var accountingAccount = new List<KeyValuePair<int, string>>();
                        foreach (var priceType in model.AccountingAccountInfo_PriceType)
                        {
                            var query = new tblAccountingAccounts();
                            query.account = model.AccountingAccountInfo_Account;
                            query.accountName = model.AccountingAccountInfo_AccountName;
                            query.companyID = model.AccountingAccountInfo_Company;
                            //query.priceTypeID = model.AccountingAccountInfo_PriceType;
                            query.priceTypeID = priceType;
                            query.articleMXN = model.AccountingAccountInfo_ArticleMXN;
                            query.articleUSD = model.AccountingAccountInfo_ArticleUSD;
                            query.accountType = model.AccountingAccountInfo_AccountType;
                            query.dateSaved = DateTime.Now;
                            query.savedByUserID = session.UserID;
                            db.tblAccountingAccounts.AddObject(query);
                            db.SaveChanges();
                            accountingAccount.Add(new KeyValuePair<int, string>(query.accountingAccountID, query.tblPriceTypes.priceType));
                        }

                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Accounting Account Saved";
                        response.ObjectID = new { accountingAccountID = accountingAccount };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Accounting Account(s) NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
            }
            public AccountingAccountsModel.AccountingAccountInfoModel GetAccountingAccount(int AccountingAccountInfo_AccountingAccountID)
            {
                ePlatEntities db = new ePlatEntities();
                AccountingAccountsModel.AccountingAccountInfoModel model = new AccountingAccountsModel.AccountingAccountInfoModel();

                var query = db.tblAccountingAccounts.Single(m => m.accountingAccountID == AccountingAccountInfo_AccountingAccountID);
                model.AccountingAccountInfo_AccountingAccountID = query.accountingAccountID;
                model.AccountingAccountInfo_Account = query.account;
                model.AccountingAccountInfo_AccountName = query.accountName;
                model.AccountingAccountInfo_Company = query.companyID;
                model.AccountingAccountInfo_CompanyText = query.tblCompanies.company;
                model.AccountingAccountInfo_PriceType = new int?[] { query.priceTypeID };
                model.AccountingAccountInfo_ArticleMXN = query.articleMXN;
                model.AccountingAccountInfo_ArticleUSD = query.articleUSD;
                model.AccountingAccountInfo_AccountType = query.accountType;
                return model;
            }
            public AttemptResponse DeleteAccountingAccount(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblAccountingAccounts.Single(m => m.accountingAccountID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Accounting Account Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Accounting Account NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class Budgets
        {
            public class BudgetsCatalogs
            {
                public static List<SelectListItem> FillDrpBudgetsPerTeam(int? promotionTeam = null)
                {
                    ePlatEntities db = new ePlatEntities();
                    List<SelectListItem> list = new List<SelectListItem>();

                    var pTeam = promotionTeam != null ? (int)promotionTeam : 0;
                    var today = DateTime.Now;

                    var query = db.tblBudgets_PromotionTeams.Where(m => pTeam == m.promotionTeamID
                        && (m.tblBudgets.fromDate <= today && (m.tblBudgets.permanent_ || m.tblBudgets.toDate >= today)));

                    foreach (var i in query)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.tblBudgets.budgetID.ToString() + "|" + (i.tblBudgets.budgetExt ? "Extension" : i.tblBudgets.perClient ? "Client" : i.tblBudgets.resetDayOfWeek),
                            Text = i.tblBudgets.leadCode + " | " + i.tblBudgets.leadQualification + " | $" + i.tblBudgets.budget + " " + i.tblBudgets.tblCurrencies.currencyCode + " | " + (i.tblBudgets.budgetExt ? "Extension" : i.tblBudgets.perClient ? " Per Client" : "Resets on " + i.tblBudgets.resetDayOfWeek)
                        });
                    }
                    return list;
                }
            }

            public List<BudgetsModel.BudgetInfoModel> SearchBudgets(BudgetsModel.SearchBudgetsModel model)
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<BudgetsModel.BudgetInfoModel>();

                var teams = model.SearchBudget_PromotionTeam != null ? model.SearchBudget_PromotionTeam : PromotionTeams.PromotionTeamsCatalogs.FillDrpPromotionTeams().Select(m => int.Parse(m.Value)).Distinct().ToArray();
                var fromDate = model.SearchBudget_I_Date != null ? DateTime.Parse(model.SearchBudget_I_Date) : (DateTime?)null;
                var toDate = model.SearchBudget_F_Date != null ? DateTime.Parse(model.SearchBudget_F_Date).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                var _now = DateTime.Now;

                var query = db.tblBudgets_PromotionTeams.Where(m => teams.Contains(m.promotionTeamID)
                    && (m.tblBudgets.leadCode == model.SearchBudget_LeadCode || model.SearchBudget_LeadCode == null)
                    && (m.tblBudgets.leadQualification.Contains(model.SearchBudget_LeadQualification) || model.SearchBudget_LeadQualification == null)
                    && ((fromDate <= m.tblBudgets.fromDate || (fromDate == null && _now >= m.tblBudgets.fromDate))
                    && (toDate >= m.tblBudgets.toDate || (toDate == null && (m.tblBudgets.toDate >= _now || m.tblBudgets.permanent_))))
                    //&& (toDate == null ? m.tblBudgets.permanent_ : toDate >= m.tblBudgets.toDate))
                    );

                foreach (var i in query.Select(m => new { m.tblBudgets.budgetID, m.tblBudgets.budget, m.tblBudgets.leadCode, m.tblBudgets.leadQualification, m.tblBudgets.tblCurrencies.currencyCode, m.tblBudgets.fromDate, m.tblBudgets.toDate, m.tblBudgets.permanent_, m.tblBudgets.budgetExt, m.tblPromotionTeams.promotionTeam }).GroupBy(m => new { m.budgetID, m.budget, m.currencyCode, m.fromDate, m.toDate, m.permanent_, m.budgetExt }))
                {
                    var _budgetID = i.FirstOrDefault().budgetID;
                    var _teams = string.Join(", ", db.tblBudgets.Single(m => m.budgetID == _budgetID).tblBudgets_PromotionTeams.Select(m => m.tblPromotionTeams.promotionTeam).OrderBy(m => m).ToArray());
                    list.Add(new BudgetsModel.BudgetInfoModel()
                    {
                        BudgetInfo_BudgetID = i.FirstOrDefault().budgetID,
                        BudgetInfo_Budget = i.FirstOrDefault().budget,
                        BudgetInfo_LeadQualification = i.FirstOrDefault().leadCode + " - " + i.FirstOrDefault().leadQualification,
                        BudgetInfo_CurrencyText = i.FirstOrDefault().currencyCode,
                        BudgetInfo_FromDate = i.FirstOrDefault().fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        BudgetInfo_ToDate = (i.FirstOrDefault().permanent_ ? "Permanent" : ((DateTime)i.FirstOrDefault().toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                        BudgetInfo_PromotionTeams = _teams,
                        BudgetInfo_BudgetExt = i.FirstOrDefault().budgetExt,
                        BudgetInfo_InUse = (db.tblPaymentDetails.Where(m => m.budgetID == _budgetID && (m.deleted == null || m.deleted == false)).Count() > 0)
                    });
                }
                return list;
            }

            public AttemptResponse SaveBudget(BudgetsModel.BudgetInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.BudgetInfo_BudgetID != 0)
                {
                    #region "update"
                    try
                    {
                        var budget = db.tblBudgets.Single(m => m.budgetID == model.BudgetInfo_BudgetID);
                        var bTeams = budget.tblBudgets_PromotionTeams.Select(m => m.promotionTeamID).ToArray();
                        Array.Sort(model.BudgetInfo_PromotionTeam);
                        Array.Sort(bTeams);
                        var budgetInUse = false;

                        if (db.tblPaymentDetails.Where(m => m.budgetID == model.BudgetInfo_BudgetID && (m.deleted == null || m.deleted == false)).Count() > 0)
                        {
                            budgetInUse = true;
                            if (budget.budget != model.BudgetInfo_Budget || budget.budgetExt != model.BudgetInfo_BudgetExt || budget.fromDate != DateTime.Parse(model.BudgetInfo_FromDate)
                                || budget.perClient != model.BudgetInfo_PerClient || budget.perWeek != model.BudgetInfo_PerWeek || budget.resetDayOfWeek != (model.BudgetInfo_PerWeek ? model.BudgetInfo_ResetDayOfWeek : null)
                                //|| !bTeams.SequenceEqual(model.BudgetInfo_PromotionTeam)
                                || model.BudgetInfo_PromotionTeam.Where(m => bTeams.Contains(m)).Count() != bTeams.Count()
                                )
                            {
                                throw new Exception("It is already in use");
                            }
                        }

                        budget.leadCode = model.BudgetInfo_LeadCode;
                        budget.leadQualification = model.BudgetInfo_LeadQualification.ToLower();
                        budget.budget = model.BudgetInfo_Budget;
                        budget.budgetExt = model.BudgetInfo_BudgetExt;
                        budget.currencyID = model.BudgetInfo_Currency;
                        budget.fromDate = DateTime.Parse(model.BudgetInfo_FromDate);
                        budget.permanent_ = model.BudgetInfo_Permanent;
                        budget.toDate = model.BudgetInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.BudgetInfo_ToDate);
                        budget.perClient = model.BudgetInfo_PerClient;
                        budget.perWeek = model.BudgetInfo_PerWeek;
                        budget.resetDayOfWeek = model.BudgetInfo_PerWeek ? model.BudgetInfo_ResetDayOfWeek : null;
                        budget.dateLastModification = DateTime.Now;
                        budget.modifiedByUserID = session.UserID;

                        foreach (var i in model.BudgetInfo_PromotionTeam)
                        {
                            if (bTeams.Where(m => m == i).Count() > 0)
                            {
                                bTeams = bTeams.Where(m => m != i).ToArray();
                            }
                            else
                            {
                                var bTeam = new tblBudgets_PromotionTeams();
                                bTeam.budgetID = model.BudgetInfo_BudgetID;
                                bTeam.promotionTeamID = i;
                                db.AddObject("tblBudgets_PromotionTeams", bTeam);
                            }
                        }

                        if (bTeams.Count() > 0)
                        {
                            foreach (var i in bTeams)
                            {
                                db.DeleteObject(db.tblBudgets_PromotionTeams.Single(m => m.budgetID == model.BudgetInfo_BudgetID && m.promotionTeamID == i));
                            }
                        }

                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Budget Updated";
                        response.ObjectID = new { budgetID = budget.budgetID, budgetInUse = budgetInUse };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Budget NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
                else
                {
                    #region "create"
                    try
                    {
                        var budget = new tblBudgets();
                        budget.leadCode = model.BudgetInfo_LeadCode;
                        budget.leadQualification = model.BudgetInfo_LeadQualification.ToLower();
                        budget.budget = model.BudgetInfo_Budget;
                        budget.budgetExt = model.BudgetInfo_BudgetExt;
                        budget.currencyID = model.BudgetInfo_Currency;
                        budget.fromDate = DateTime.Parse(model.BudgetInfo_FromDate);
                        budget.permanent_ = model.BudgetInfo_Permanent;
                        budget.toDate = model.BudgetInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.BudgetInfo_ToDate);
                        budget.perClient = model.BudgetInfo_PerClient;
                        budget.perWeek = model.BudgetInfo_PerWeek;
                        budget.resetDayOfWeek = model.BudgetInfo_PerWeek ? model.BudgetInfo_ResetDayOfWeek : null;
                        budget.dateSaved = DateTime.Now;
                        db.tblBudgets.AddObject(budget);

                        var _promotionTeams = model.BudgetInfo_PromotionTeam != null ? model.BudgetInfo_PromotionTeam : PromotionTeams.PromotionTeamsCatalogs.FillDrpPromotionTeams().Select(m => int.Parse(m.Value)).ToArray();

                        foreach (var i in _promotionTeams)
                        {
                            var bTeam = new tblBudgets_PromotionTeams();
                            bTeam.budgetID = budget.budgetID;
                            bTeam.promotionTeamID = i;
                            db.tblBudgets_PromotionTeams.AddObject(bTeam);
                        }

                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Budget Saved";
                        response.ObjectID = budget.budgetID;
                        return response;

                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Budget NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
            }

            public BudgetsModel.BudgetInfoModel GetBudget(int BudgetInfo_BudgetID)
            {
                ePlatEntities db = new ePlatEntities();
                BudgetsModel.BudgetInfoModel model = new BudgetsModel.BudgetInfoModel();

                var query = db.tblBudgets.Single(m => m.budgetID == BudgetInfo_BudgetID);

                model.BudgetInfo_BudgetID = query.budgetID;
                model.BudgetInfo_LeadCode = query.leadCode;
                model.BudgetInfo_LeadQualification = query.leadQualification;
                model.BudgetInfo_Budget = query.budget;
                model.BudgetInfo_BudgetExt = query.budgetExt;
                model.BudgetInfo_Currency = query.currencyID;
                model.BudgetInfo_FromDate = query.fromDate.ToString("yyyy-MM-dd");
                model.BudgetInfo_ToDate = query.toDate != null ? ((DateTime)query.toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                model.BudgetInfo_Permanent = query.permanent_;
                model.BudgetInfo_PerClient = query.perClient;
                model.BudgetInfo_PerWeek = query.perWeek;
                model.BudgetInfo_ResetDayOfWeek = query.resetDayOfWeek;
                model.BudgetInfo_PromotionTeam = query.tblBudgets_PromotionTeams.Select(m => m.promotionTeamID).ToArray();

                return model;
            }

            public AttemptResponse DeleteBudget(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    if (db.tblPaymentDetails.Where(m => m.budgetID == targetID && (m.deleted == null || m.deleted == false)).Count() > 0)
                    {
                        throw new Exception("It is already in use");
                    }
                    var query = db.tblBudgets.Single(m => m.budgetID == targetID);
                    db.tblBudgets.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Budget Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Budget NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class Commissions
        {
            public List<CommissionsModel.CommissionsInfoModel> SearchCommissions(CommissionsModel.SearchCommissionsModel model)
            {
                List<CommissionsModel.CommissionsInfoModel> list = new List<CommissionsModel.CommissionsInfoModel>();
                ePlatEntities db = new ePlatEntities();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var priceTypes = model.CommissionsSearch_PriceTypes != null ? model.CommissionsSearch_PriceTypes : PriceDataModel.PricesCatalogs.FillDrpPriceTypes().Select(m => int.Parse(m.Value)).ToArray();
                var jobPositions = model.CommissionsSearch_JobPositions != null ? model.CommissionsSearch_JobPositions : CatalogsDataModel.General.FillDrpJobPositions().Select(m => int.Parse(m.Value)).ToArray();
                var percentage = model.CommissionsSearch_CommissionPercentage != null ? decimal.Parse(model.CommissionsSearch_CommissionPercentage) : 0;

                var query = from commission in db.tblCommissions
                            where (commission.commissionPercentage == percentage || model.CommissionsSearch_CommissionPercentage == null)
                            && priceTypes.Contains(commission.priceTypeID)
                            && jobPositions.Contains(commission.jobPositionID)
                            && terminals.Contains(commission.terminalID)
                            && commission.dateDeleted == null
                            && ((commission.permanent_)
                            || (commission.permanent_ != true && commission.toDate > DateTime.Now))
                            select commission;

                foreach (var i in query)
                {
                    var _pointsOfSale = db.tblCommissions_PointsOfSale.Where(m => m.commissionID == i.commissionID).Select(m => m.tblPointsOfSale.shortName).Distinct().ToArray();
                    list.Add(new CommissionsModel.CommissionsInfoModel()
                    {
                        CommissionsInfo_CommissionID = i.commissionID,
                        CommissionsInfo_CommissionType = (i.isBonus == true ? "Monthly Bonus Commission" : "Regular Commission"),
                        CommissionsInfo_MinVolume = i.minVolume.ToString(),
                        CommissionsInfo_MaxVolume = i.maxVolume != null ? i.maxVolume.ToString() : "",
                        CommissionsInfo_VolumeCurrencyCode = i.volumeCurrencyCode,
                        CommissionsInfo_MinProfit = i.minProfit.ToString(),
                        CommissionsInfo_MaxProfit = i.maxProfit.ToString(),
                        //CommissionsInfo_PriceType = i.priceTypeID,
                        CommissionsInfo_PriceTypeText = i.tblPriceTypes.priceType,
                        //CommissionsInfo_JobPosition = i.jobPositionID,
                        CommissionsInfo_JobPositionText = i.tblJobPositions.jobPosition,
                        CommissionsInfo_CommissionPercentage = i.commissionPercentage,
                        CommissionsInfo_CommissionAmount = i.commissionAmount != null ? (decimal)i.commissionAmount : 0,
                        CommissionsInfo_CommissionCurrency = i.commissionPercentage != 0 ? "" : i.tblCurrencies.currencyCode,
                        CommissionsInfo_TerminalText = i.tblTerminals.terminal,
                        CommissionsInfo_PointsOfSale = _pointsOfSale,
                        CommissionsInfo_IsOverride = i.@override ? "True" : "False",
                        CommissionsInfo_IsPermanent = i.permanent_,
                        CommissionsInfo_FromDate = (i.fromDate != null ? i.fromDate.Value.ToString("yyyy-MM-dd") : ""),
                        CommissionsInfo_ToDate = (i.toDate != null ? i.toDate.Value.ToString("yyyy-MM-dd") : "")
                    });
                }
                return list;
            }

            public AttemptResponse SaveCommission(CommissionsModel.CommissionsInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.CommissionsInfo_CommissionID != 0)
                {
                    #region "update"
                    try
                    {
                        var query = db.tblCommissions.Single(m => m.commissionID == model.CommissionsInfo_CommissionID);
                        query.isBonus = model.CommissionsInfo_IsBonus;
                        query.permanent_ = model.CommissionsInfo_IsPermanent;
                        query.fromDate = (model.CommissionsInfo_FromDate != null ? DateTime.Parse(model.CommissionsInfo_FromDate) : DateTime.Now);
                        if (!query.permanent_)
                        {
                            query.toDate = (model.CommissionsInfo_ToDate != null ? DateTime.Parse(model.CommissionsInfo_ToDate) : (DateTime?)null);
                        }
                        else
                        {
                            query.toDate = (DateTime?)null;
                        }
                        if (model.CommissionsInfo_MinProfit != null)
                        {
                            query.minProfit = decimal.Parse(model.CommissionsInfo_MinProfit);
                        }
                        else
                        {
                            query.minProfit = 0;
                        }
                        query.maxProfit = model.CommissionsInfo_MaxProfit != null ? decimal.Parse(model.CommissionsInfo_MaxProfit) : (decimal?)null;

                        if (model.CommissionsInfo_MinVolume != null)
                        {
                            query.minVolume = decimal.Parse(model.CommissionsInfo_MinVolume);
                        }
                        else
                        {
                            query.minVolume = 0;
                        }
                        query.maxVolume = model.CommissionsInfo_MaxVolume != null ? decimal.Parse(model.CommissionsInfo_MaxVolume) : (decimal?)null;
                        query.volumeCurrencyCode = model.CommissionsInfo_VolumeCurrencyCode;
                        query.priceTypeID = model.CommissionsInfo_PriceType.FirstOrDefault();
                        query.jobPositionID = model.CommissionsInfo_JobPosition;
                        query.sysWorkGroupID = model.CommissionsInfo_SysWorkGroup != 0 ? model.CommissionsInfo_SysWorkGroup : (int?)null;
                        query.terminalID = model.CommissionsInfo_Terminal;
                        query.commissionPercentage = model.CommissionsInfo_CommissionPercentage;
                        query.commissionAmount = model.CommissionsInfo_CommissionPercentage == 0 ? model.CommissionsInfo_CommissionAmount : (decimal?)null;
                        query.commissionCurrencyID = model.CommissionsInfo_CommissionPercentage == 0 ? GeneralFunctions.GetCurrencyID(model.CommissionsInfo_CommissionCurrency) : (int?)null;
                        query.@override = model.CommissionsInfo_Override;
                        query.applyOnSales = model.CommissionsInfo_ApplyOnSales;
                        query.applyOnDealPrice = model.CommissionsInfo_ApplyOnDealPrice;
                        query.applyOnAdultSales = model.CommissionsInfo_ApplyOnAdultSales;
                        query.onlyIfCharged = model.CommissionsInfo_OnlyIfCharged;
                        var dbLocations = db.tblCommissions_PointsOfSale.Where(m => m.commissionID == query.commissionID).Select(m => m.pointOfSaleID).ToArray();
                        foreach (var i in model.CommissionsInfo_PointsOfSale)
                        {
                            var _location = int.Parse(i);
                            if (dbLocations.Contains(_location))
                            {
                                dbLocations = dbLocations.Where(m => m != _location).ToArray();
                            }
                            else
                            {
                                var location = new tblCommissions_PointsOfSale();
                                location.pointOfSaleID = _location;
                                location.commissionID = query.commissionID;
                                db.tblCommissions_PointsOfSale.AddObject(location);
                            }
                        }
                        if (dbLocations.Count() > 0)
                        {
                            foreach (var i in dbLocations)
                            {
                                db.tblCommissions_PointsOfSale.DeleteObject(db.tblCommissions_PointsOfSale.Single(m => m.commissionID == query.commissionID && m.pointOfSaleID == i));
                            }
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Commission Updated";
                        var _pointsOfSale = Array.ConvertAll(model.CommissionsInfo_PointsOfSale, x => int.Parse(x));
                        response.ObjectID = new { commissionID = query.commissionID, pointsOfSale = string.Join(",", db.tblPointsOfSale.Where(m => _pointsOfSale.Contains(m.pointOfSaleID)).Select(m => m.shortName)) };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Commission NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
                else
                {
                    try
                    {
                        //model.CommissionsInfo_PriceType = model.CommissionsInfo_PriceType != null ? model.CommissionsInfo_PriceType : Array.ConvertAll(PriceDataModel.PricesCatalogs.FillDrpPriceTypes(model.CommissionsInfo_Terminal).Select(m => m.Value).ToArray(), int.Parse);
                        model.CommissionsInfo_PriceType = model.CommissionsInfo_PriceType != null ? model.CommissionsInfo_PriceType : PriceDataModel.PricesCatalogs.FillDrpPriceTypes(model.CommissionsInfo_Terminal).Select(m => int.Parse(m.Value)).ToArray();
                        var commission = new List<KeyValuePair<long, string>>();
                        foreach (var priceType in model.CommissionsInfo_PriceType)
                        {
                            var query = new tblCommissions();
                            query.isBonus = model.CommissionsInfo_IsBonus;
                            query.permanent_ = model.CommissionsInfo_IsPermanent;
                            query.fromDate = (model.CommissionsInfo_FromDate != null ? DateTime.Parse(model.CommissionsInfo_FromDate) : DateTime.Now);
                            if (!query.permanent_)
                            {
                                query.toDate = (model.CommissionsInfo_ToDate != null ? DateTime.Parse(model.CommissionsInfo_ToDate) : (DateTime?)null);
                            }
                            else
                            {
                                query.toDate = (DateTime?)null;
                            }
                            if (model.CommissionsInfo_MinProfit != null)
                            {
                                query.minProfit = decimal.Parse(model.CommissionsInfo_MinProfit);
                            }
                            else
                            {
                                query.minProfit = 0;
                            }
                            query.maxProfit = model.CommissionsInfo_MaxProfit != null ? decimal.Parse(model.CommissionsInfo_MaxProfit) : (decimal?)null;

                            if (model.CommissionsInfo_MinVolume != null)
                            {
                                query.minVolume = decimal.Parse(model.CommissionsInfo_MinVolume);
                            }
                            else
                            {
                                query.minVolume = 0;
                            }
                            query.maxVolume = model.CommissionsInfo_MaxVolume != null ? decimal.Parse(model.CommissionsInfo_MaxVolume) : (decimal?)null;
                            query.volumeCurrencyCode = model.CommissionsInfo_VolumeCurrencyCode;
                            query.priceTypeID = priceType;
                            query.jobPositionID = model.CommissionsInfo_JobPosition;
                            query.sysWorkGroupID = model.CommissionsInfo_SysWorkGroup != 0 ? model.CommissionsInfo_SysWorkGroup : (int?)null;
                            query.terminalID = model.CommissionsInfo_Terminal;
                            query.commissionPercentage = model.CommissionsInfo_CommissionPercentage;
                            query.commissionAmount = model.CommissionsInfo_CommissionPercentage == 0 ? model.CommissionsInfo_CommissionAmount : (decimal?)null;
                            query.commissionCurrencyID = model.CommissionsInfo_CommissionPercentage == 0 ? GeneralFunctions.GetCurrencyID(model.CommissionsInfo_CommissionCurrency) : (int?)null;
                            query.@override = model.CommissionsInfo_Override;
                            query.applyOnSales = model.CommissionsInfo_ApplyOnSales;
                            query.applyOnDealPrice = model.CommissionsInfo_ApplyOnDealPrice;
                            query.applyOnAdultSales = model.CommissionsInfo_ApplyOnAdultSales;
                            query.onlyIfCharged = model.CommissionsInfo_OnlyIfCharged;
                            query.dateSaved = DateTime.Now;
                            query.savedByUserID = session.UserID;
                            db.tblCommissions.AddObject(query);
                            model.CommissionsInfo_PointsOfSale = model.CommissionsInfo_PointsOfSale != null ? model.CommissionsInfo_PointsOfSale : MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(model.CommissionsInfo_Terminal).Select(m => m.Value).ToArray();
                            foreach (var i in model.CommissionsInfo_PointsOfSale)
                            {
                                var _location = new tblCommissions_PointsOfSale();
                                _location.commissionID = query.commissionID;
                                _location.pointOfSaleID = int.Parse(i);
                                db.tblCommissions_PointsOfSale.AddObject(_location);
                            }
                            db.SaveChanges();
                            commission.Add(new KeyValuePair<long, string>(query.commissionID, query.tblPriceTypes.priceType));
                        }
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Commission Saved";
                        var _pointsOfSale = Array.ConvertAll(model.CommissionsInfo_PointsOfSale, x => int.Parse(x));
                        response.ObjectID = new { commissionID = commission, pointsOfSale = string.Join(",", db.tblPointsOfSale.Where(m => _pointsOfSale.Contains(m.pointOfSaleID)).Select(m => m.shortName)) };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Commission NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public CommissionsModel.CommissionsInfoModel GetCommission(int CommissionsInfo_CommissionID)
            {
                ePlatEntities db = new ePlatEntities();
                CommissionsModel.CommissionsInfoModel model = new CommissionsModel.CommissionsInfoModel();

                var query = db.tblCommissions.Single(m => m.commissionID == CommissionsInfo_CommissionID);

                model.CommissionsInfo_CommissionID = query.commissionID;
                model.CommissionsInfo_IsBonus = query.isBonus;
                model.CommissionsInfo_MinProfit = query.minProfit.ToString();
                model.CommissionsInfo_MaxProfit = query.maxProfit.ToString();
                model.CommissionsInfo_FromDate = (query.fromDate != null ? query.fromDate.Value.ToString("yyyy-MM-dd") : "");
                model.CommissionsInfo_IsPermanent = query.permanent_;
                model.CommissionsInfo_ToDate = (query.toDate != null ? query.toDate.Value.ToString("yyyy-MM-dd") : "");
                model.CommissionsInfo_MinVolume = query.minVolume.ToString();
                model.CommissionsInfo_MaxVolume = query.maxVolume != null ? query.maxVolume.ToString() : "";
                model.CommissionsInfo_VolumeCurrencyCode = query.volumeCurrencyCode;
                model.CommissionsInfo_PriceType = new int[] { query.priceTypeID };
                //model.CommissionsInfo_PriceTypeText = query.tblPriceTypes.priceType;
                model.CommissionsInfo_JobPosition = query.jobPositionID;
                //model.CommissionsInfo_JobPositionText = query.tblJobPositions.jobPosition;
                model.CommissionsInfo_CommissionPercentage = query.commissionPercentage;
                model.CommissionsInfo_CommissionAmount = query.commissionAmount ?? 0;
                model.CommissionsInfo_CommissionCurrency = query.commissionCurrencyID != null ? query.tblCurrencies.currencyCode : "";
                model.CommissionsInfo_Override = query.@override;
                model.CommissionsInfo_ApplyOnSales = query.applyOnSales;
                model.CommissionsInfo_ApplyOnDealPrice = query.applyOnDealPrice;
                model.CommissionsInfo_ApplyOnAdultSales = query.applyOnAdultSales;
                model.CommissionsInfo_OnlyIfCharged = query.onlyIfCharged;
                model.CommissionsInfo_SysWorkGroup = query.sysWorkGroupID != null ? (int)query.sysWorkGroupID : 0;
                model.CommissionsInfo_Terminal = query.terminalID;
                //model.CommissionsInfo_PointsOfSale = db.tblCommissions_PointsOfSale.Where(m => m.commissionID == query.commissionID).Select(m => m.pointOfSaleID).ToList().Cast<string>().ToArray();
                model.CommissionsInfo_PointsOfSale = Array.ConvertAll(db.tblCommissions_PointsOfSale.Where(m => m.commissionID == query.commissionID).Select(m => m.pointOfSaleID).ToArray(), x => x.ToString());
                return model;
            }

            public AttemptResponse DeleteCommission(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var commission = db.tblCommissions.Single(m => m.commissionID == targetID);
                    commission.dateDeleted = DateTime.Today;
                    commission.deletedByUserID = session.UserID;
                    commission.permanent_ = false;
                    commission.toDate = DateTime.Now;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Commission Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Commission NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class Companies
        {
            public class CompaniesCatalogs
            {
                public static List<SelectListItem> FillDrpMarketingCompanies()
                {
                    return FillDrpMarketingCompanies(null);
                }

                public static List<SelectListItem> FillDrpMarketingCompanies(long? terminalID)
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();
                    var terminals = new List<long>();
                    if (terminalID == null)
                    {
                        terminals = session.Terminals != "" ?
                            session.Terminals.Split(',').Select(m => long.Parse(m)).ToList() :
                            session.UserTerminals.Split(',').Select(m => long.Parse(m)).ToList();
                    }
                    else
                    {
                        terminals.Add((long)terminalID);
                    }

                    var companies = from c in db.tblTerminals_Companies
                                    where terminals.Contains(c.terminalID)
                                    & c.tblCompanies.companyTypeID == 2
                                    orderby c.tblCompanies.company
                                    select c.tblCompanies;
                    foreach (var i in companies)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.companyID.ToString(),
                            Text = i.company
                        });
                    }
                    return list;
                }

                public static List<SelectListItem> FillDrpTACompanies()
                {
                    return FillDrpTACompanies(null);
                }

                public static List<SelectListItem> FillDrpTACompanies(long? terminalID)
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();
                    var terminals = new List<long>();
                    if (terminalID == null)
                    {
                        terminals = session.Terminals != "" ?
                            session.Terminals.Split(',').Select(m => long.Parse(m)).ToList() :
                            session.UserTerminals.Split(',').Select(m => long.Parse(m)).ToList();
                    }
                    else
                    {
                        terminals.Add((long)terminalID);
                    }

                    var companies = from c in db.tblTerminals_Companies
                                    where terminals.Contains(c.terminalID)
                                    & c.tblCompanies.companyTypeID == 1
                                    orderby c.tblCompanies.company
                                    select c.tblCompanies;
                    foreach (var i in companies)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.companyID.ToString(),
                            Text = i.company
                        });
                    }
                    return list;
                }

                public static List<SelectListItem> FillDrpCompanies()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();

                    var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                    var companies = (from c in db.tblTerminals_Companies
                                     join company in db.tblCompanies on c.companyID equals company.companyID
                                     where terminals.Contains(c.terminalID)
                                     orderby c.tblCompanies.company
                                     select new
                                     {
                                         c.companyID,
                                         company.company
                                     }).Distinct();


                    foreach (var i in companies)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.companyID.ToString(),
                            Text = i.company
                        });
                    }
                    list = list.OrderByDescending(m => m.Text).ToList();
                    return list;
                }
            }

            public List<CompaniesModel.CompanyInfoModel> SearchCompanies(CompaniesModel.SearchCompaniesModel model)
            {
                List<CompaniesModel.CompanyInfoModel> list = new List<CompaniesModel.CompanyInfoModel>();
                ePlatEntities db = new ePlatEntities();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var countries = model.SearchCompanies_Countries != null ? model.SearchCompanies_Countries.Split(',').Select(m => Convert.ToInt32(m)).ToArray() : new int[] { };
                var companies = db.tblTerminals_Companies.Where(m => terminals.Contains(m.terminalID)).Select(m => m.companyID).ToArray();

                var query = from company in db.tblCompanies
                            where (company.company.Contains(model.SearchCompanies_Company) || model.SearchCompanies_Company == null)
                            && (countries.Contains((int)company.countryID) || model.SearchCompanies_Countries == null)
                            && companies.Contains(company.companyID)
                            select company;

                // query = query.Where(m => m.tblTerminals.FirstOrDefault(x => terminals.Contains(x.terminalID)).companyID != null);

                foreach (var i in query)
                {
                    list.Add(new CompaniesModel.CompanyInfoModel()
                    {
                        CompanyInfo_CompanyID = i.companyID,
                        CompanyInfo_Company = i.company,
                        CompanyInfo_ShortName = i.shortName,
                        CompanyInfo_Address = i.address,
                        CompanyInfo_City = i.city,
                        CompanyInfo_State = i.state,
                        CompanyInfo_Country = i.countryID != null ? (int)i.countryID : 0,
                        CompanyInfo_CountryText = i.tblCountries.country,
                        CompanyInfo_ZipCode = i.zipCode,
                        CompanyInfo_RFC = i.rfc,
                        CompanyTypeID = i.companyTypeID != null ? (int)i.companyTypeID : 0,
                        CompanyInfo_TerminalID = i.tblTerminals_Companies.Select(x => x.terminalID).ToArray()
                    });
                }
                return list;
            }

            public AttemptResponse SaveCompany(CompaniesModel.CompanyInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.CompanyInfo_CompanyID != 0)
                {
                    try
                    {
                        var query = db.tblCompanies.Single(m => m.companyID == model.CompanyInfo_CompanyID);
                        query.company = model.CompanyInfo_Company;
                        query.shortName = model.CompanyInfo_ShortName;
                        query.address = model.CompanyInfo_Address;
                        query.city = model.CompanyInfo_City;
                        query.state = model.CompanyInfo_State;
                        if (model.CompanyInfo_Country != 0)
                        {
                            query.countryID = model.CompanyInfo_Country;
                        }
                        query.zipCode = model.CompanyInfo_ZipCode;
                        query.rfc = model.CompanyInfo_RFC;
                        if (model.CompanyTypeID != 0)
                        {
                            query.companyTypeID = model.CompanyTypeID;
                        }

                        if (model.CompanyInfo_TerminalID.Count() != 0)
                        {
                            var terminals = query.tblTerminals_Companies.Select(x => x.terminalID);
                            if (model.CompanyInfo_TerminalID.Count() > terminals.Count() || model.CompanyInfo_TerminalID.Count() == terminals.Count())
                            {
                                foreach (var newTerminal in model.CompanyInfo_TerminalID)
                                {
                                    if (terminals.Contains(newTerminal) != true)
                                    {
                                        var newTerminalCompany = new tblTerminals_Companies()
                                        {
                                            companyID = model.CompanyInfo_CompanyID,
                                            terminalID = newTerminal
                                        };
                                        db.tblTerminals_Companies.AddObject(newTerminalCompany);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            if (model.CompanyInfo_TerminalID.Count() < terminals.Count())
                            {
                                foreach (var terminalDelete in terminals.ToList())
                                {
                                    if (model.CompanyInfo_TerminalID.Contains(terminalDelete) != true)
                                    {
                                        var item = db.tblTerminals_Companies.Where(x => x.companyID == model.CompanyInfo_CompanyID && x.terminalID == terminalDelete);
                                        foreach (var remove in item.ToList())
                                        {
                                            db.DeleteObject(remove);
                                        }
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Company Updated";
                        response.ObjectID = query.companyID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Company NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblCompanies();
                        query.company = model.CompanyInfo_Company;
                        query.shortName = model.CompanyInfo_ShortName;
                        query.address = model.CompanyInfo_Address;
                        query.city = model.CompanyInfo_City;
                        query.state = model.CompanyInfo_State;
                        if (model.CompanyInfo_Country != 0)
                        {
                            query.countryID = model.CompanyInfo_Country;
                        }
                        query.zipCode = model.CompanyInfo_ZipCode;
                        query.rfc = model.CompanyInfo_RFC;
                        if (model.CompanyTypeID != 0)
                        {
                            query.companyTypeID = model.CompanyTypeID;
                        }
                        db.tblCompanies.AddObject(query);
                        db.SaveChanges();
                        if (db.tblCompanies.Select(x => x.companyID == model.CompanyInfo_CompanyID).Count() > 0)
                        {
                            foreach (var item in model.CompanyInfo_TerminalID)
                            {
                                var newTerminalCompany = new tblTerminals_Companies()
                                {
                                    companyID = query.companyID,
                                    terminalID = item
                                };
                                db.tblTerminals_Companies.AddObject(newTerminalCompany);
                            }
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Company Saved";
                        response.ObjectID = query.companyID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Company NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public CompaniesModel.CompanyInfoModel GetCompany(int CompanyInfo_CompanyID)
            {
                ePlatEntities db = new ePlatEntities();
                CompaniesModel.CompanyInfoModel model = new CompaniesModel.CompanyInfoModel();

                var query = db.tblCompanies.Single(m => m.companyID == CompanyInfo_CompanyID);
                model.CompanyInfo_CompanyID = query.companyID;
                model.CompanyInfo_Company = query.company;
                model.CompanyInfo_ShortName = query.shortName;
                model.CompanyInfo_Address = query.address;
                model.CompanyInfo_City = query.city;
                model.CompanyInfo_State = query.state;
                model.CompanyInfo_Country = query.countryID != null ? (int)query.countryID : 0;
                model.CompanyInfo_CountryText = query.tblCountries.country;
                model.CompanyInfo_ZipCode = query.zipCode;
                model.CompanyInfo_RFC = query.rfc;
                model.CompanyTypeID = query.companyTypeID != null ? (int)query.companyTypeID : 0;
                model.CompanyType = query.tblCompanyTypes.companyType;
                model.CompanyInfo_TerminalID = query.tblTerminals_Companies.Select(x => x.terminalID).ToArray();
                model.CompanyInfo_Terminal = query.tblTerminals_Companies.Select(x => x.tblTerminals.terminal).ToArray();
                return model;
            }

            public AttemptResponse DeleteCompany(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblCompanies.Single(m => m.companyID == targetID);
                    if (db.tblTerminals_Companies.Select(x => x.companyID == targetID).Count() > 0)
                    {
                        var list = (from x in db.tblTerminals_Companies
                                    where x.companyID == targetID
                                    select x).Distinct();
                        foreach (var y in list)
                        {
                            db.DeleteObject(y);
                        }
                    }
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Company Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Company NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class Destinations
        {
            public class DestinationsCatalogs
            {
                public static List<SelectListItem> FillDrpCultures()
                {
                    ePlatEntities db = new ePlatEntities();
                    List<SelectListItem> list = new List<SelectListItem>();

                    foreach (var i in db.tblLanguages)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.culture,
                            Text = i.language
                        });
                    }
                    return list;
                }

                public static List<SelectListItem> FillDrpTerminals()
                {
                    ePlatEntities db = new ePlatEntities();
                    List<SelectListItem> list = new List<SelectListItem>();

                    list = TerminalDataModel.GetCurrentUserTerminals();
                    return list;
                }

                public static List<SelectListItem> FillDrpDestinationsPerCurrentTerminals()
                {
                    ePlatEntities db = new ePlatEntities();
                    List<SelectListItem> list = new List<SelectListItem>();

                    long[] currentTerminals = (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray();

                    var query = (from d in db.tblDestinations
                                 join td in db.tblTerminals_Destinations on d.destinationID equals td.destinationID
                                 where currentTerminals.Contains(td.terminalID)
                                 orderby d.destination ascending
                                 select d).Distinct();

                    foreach (var i in query)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.destinationID.ToString(),
                            Text = i.destination
                        });
                    }
                    return list;
                }
            }

            public List<DestinationsModel.DestinationsInfoModel> SearchDestinations(DestinationsModel.SearchDestinationsModel model)
            {
                ePlatEntities db = new ePlatEntities();
                List<DestinationsModel.DestinationsInfoModel> list = new List<DestinationsModel.DestinationsInfoModel>();
                //long[] currentTerminals = (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray();


                var query = from d in db.tblDestinations
                                //join td in db.tblTerminals_Destinations on d.destinationID equals td.destinationID
                            where (d.destination.Contains(model.Search_Destinations) || model.Search_Destinations == null)
                            //&& currentTerminals.Contains(td.terminalID)
                            orderby d.destination ascending
                            select d;
                foreach (var i in query)
                {
                    list.Add(new DestinationsModel.DestinationsInfoModel()
                    {
                        DestinationInfo_DestinationID = int.Parse(i.destinationID.ToString()),
                        DestinationInfo_DestinationName = i.destination,
                        DestinationInfo_Latitude = i.lat,
                        DestinationInfo_Longitude = i.lng
                    });
                }
                return list;
            }

            public AttemptResponse SaveDestination(DestinationsModel.DestinationsInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.DestinationInfo_DestinationID != 0)
                {
                    try
                    {
                        var query = db.tblDestinations.Single(m => m.destinationID == model.DestinationInfo_DestinationID);
                        query.destination = model.DestinationInfo_Destination;
                        query.lat = model.DestinationInfo_Latitude;
                        query.lng = model.DestinationInfo_Longitude;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Destination Updated";
                        response.ObjectID = query.destinationID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Destination NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblDestinations();
                        query.destination = model.DestinationInfo_Destination;
                        query.lat = model.DestinationInfo_Latitude;
                        query.lng = model.DestinationInfo_Longitude;
                        db.tblDestinations.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Destination Saved";
                        response.ObjectID = query.destinationID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Destination NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public DestinationsModel.DestinationsInfoModel GetDestination(int DestinationInfo_DestinationID)
            {
                ePlatEntities db = new ePlatEntities();
                DestinationsModel.DestinationsInfoModel model = new DestinationsModel.DestinationsInfoModel();

                var query = db.tblDestinations.Single(m => m.destinationID == DestinationInfo_DestinationID);
                model.DestinationInfo_DestinationID = int.Parse(query.destinationID.ToString());
                model.DestinationInfo_Destination = query.destination;
                model.DestinationInfo_Latitude = query.lat;
                model.DestinationInfo_Longitude = query.lng;
                return model;
            }

            public AttemptResponse DeleteDestination(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                long target = (long)targetID;
                try
                {
                    //prevents orphan destination seoItems
                    if (db.tblSeoItems.Where(m => m.itemID == target && m.sysItemTypeID == 4).Count() > 0)
                    {
                        throw new Exception("It is in use by Seo Items");
                    }
                    else
                    {
                        if (db.tblDestinationDescriptions.Where(m => m.destinationID == target).Count() > 0)
                        {
                            throw new Exception("It has Existent Descriptions");
                        }
                        else
                        {
                            var query = db.tblDestinations.Single(m => m.destinationID == target);
                            db.DeleteObject(query);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Destination Deleted";
                            response.ObjectID = targetID;
                            return response;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Destination NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }

            public AttemptResponse SaveDestinationDescription(DestinationsModel.DestinationDescriptionsModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.DestinationDescription_DestinationDescriptionID != 0)
                {
                    try
                    {
                        var query = db.tblDestinationDescriptions.Single(m => m.destinationDescriptionID == model.DestinationDescription_DestinationDescriptionID);
                        query.destinationID = model.DestinationDescription_DestinationID;
                        query.culture = model.DestinationDescription_Culture;
                        query.description = model.DestinationDescription_Description;
                        query.videoTitle = model.DestinationDescription_VideoTitle;
                        query.videoURL = model.DestinationDescription_VideoURL;
                        query.terminalID = model.DestinationDescription_Terminal;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Destination Description Updated";
                        response.ObjectID = query.destinationDescriptionID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Destination Description NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblDestinationDescriptions();
                        query.destinationID = model.DestinationDescription_DestinationID;
                        query.culture = model.DestinationDescription_Culture;
                        query.description = model.DestinationDescription_Description;
                        query.terminalID = model.DestinationDescription_Terminal;
                        query.videoTitle = model.DestinationDescription_VideoTitle;
                        query.videoURL = model.DestinationDescription_VideoURL;
                        db.tblDestinationDescriptions.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Destination Description Saved";
                        response.ObjectID = query.destinationDescriptionID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Destination Description NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                return response;
            }

            public List<DestinationsModel.DestinationDescriptionsModel> GetDestinationDescriptions(int DestinationInfo_DestinationID)
            {
                ePlatEntities db = new ePlatEntities();
                List<DestinationsModel.DestinationDescriptionsModel> list = new List<DestinationsModel.DestinationDescriptionsModel>();
                long[] currentTerminals = (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray();
                try
                {
                    var query = from description in db.tblDestinationDescriptions
                                where description.destinationID == DestinationInfo_DestinationID
                                && currentTerminals.Contains(description.terminalID)
                                select description;

                    foreach (var i in query)
                    {
                        list.Add(new DestinationsModel.DestinationDescriptionsModel()
                        {
                            DestinationDescription_DestinationDescriptionID = int.Parse(i.destinationDescriptionID.ToString()),
                            DestinationDescription_DestinationID = int.Parse(i.destinationID.ToString()),
                            DestinationDescription_Culture = i.culture,
                            DestinationDescription_Description = i.description,
                            DestinationDescription_Terminal = int.Parse(i.terminalID.ToString()),
                            DestinationDescription_VideoTitle = i.videoTitle,
                            DestinationDescription_VideoURL = i.videoURL
                        });
                    }
                }
                catch (Exception ex)
                {

                }
                return list;
            }

            public DestinationsModel.DestinationDescriptionsModel GetDestinationDescription(int DestinationDescription_DestinationDescriptionID)
            {
                ePlatEntities db = new ePlatEntities();
                DestinationsModel.DestinationDescriptionsModel model = new DestinationsModel.DestinationDescriptionsModel();

                var query = db.tblDestinationDescriptions.Single(m => m.destinationDescriptionID == DestinationDescription_DestinationDescriptionID);

                model.DestinationDescription_DestinationDescriptionID = int.Parse(query.destinationDescriptionID.ToString());
                model.DestinationDescription_DestinationID = int.Parse(query.destinationID.ToString());
                model.DestinationDescription_Culture = query.culture;
                model.DestinationDescription_Description = query.description;
                model.DestinationDescription_Terminal = int.Parse(query.terminalID.ToString());
                model.DestinationDescription_VideoTitle = query.videoTitle;
                model.DestinationDescription_VideoURL = query.videoURL;
                return model;
            }

            public AttemptResponse DeleteDestinationDescription(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblDestinationDescriptions.Single(m => m.destinationDescriptionID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Destination Description Deleted";
                    response.ObjectID = targetID;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Destination Description NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                }
                return response;
            }
        }

        public class ExchangeRates
        {
            public class ExchangeRatesCatalogs
            {
                public static List<SelectListItem> FillDrpActiveExchangeRates()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();

                    var query = db.tblExchangeRates.Where(m => m.fromDate <= DateTime.Today && (m.toDate >= DateTime.Today || (m.toDate == null && m.permanent_)) && m.tblCurrencies1.currencyCode == "MXN");

                    foreach (var i in query)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.exchangeRateID.ToString(),
                            Text = i.fromDate.Month + "-" + i.fromDate.Day + ", " + i.exchangeRate + " " + i.tblCurrencies.currencyCode + "-" + i.tblCurrencies1.currencyCode
                        });
                    }
                    return list;
                }

                public static List<SelectListItem> FillDrpExchangeRateTypes()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();

                    foreach (var i in db.tblExchangeRateTypes)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.exchangeRateTypeID.ToString(),
                            Text = i.exchangeRateType
                        });
                    }
                    return list;
                }

                public static tblExchangeRates RateAlreadyExists(ExchangeRatesModel.ExchangeRatesInfoModel model, ref ePlatEntities db)
                {
                    //ePlatEntities db = new ePlatEntities();
                    var _fromDate = model.ExchangeRatesInfo_I_Date != null ? DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                    var _fromCurrency = int.Parse(model.ExchangeRatesInfo_FromCurrency);
                    var _toCurrency = int.Parse(model.ExchangeRatesInfo_ToCurrency);

                    var rate = db.tblExchangeRates.Where(m =>
                        m.terminalID == model.ExchangeRatesInfo_Terminal
                        && m.fromCurrencyID == _fromCurrency
                        && m.toCurrencyID == _toCurrency
                        && (m.fromDate <= _fromDate && (m.permanent_ || _fromDate <= m.toDate))
                        && m.exchangeRateTypeID == model.ExchangeRatesInfo_ExchangeRateType);

                    if (model.ExchangeRatesInfo_Provider != 0)
                    {
                        rate = rate.Where(m => m.providerID == model.ExchangeRatesInfo_Provider);
                    }
                    else
                    {
                        rate = rate.Where(m => m.providerID == null);
                    }
                    if (model.ExchangeRatesInfo_PointsOfSale != null)
                    {
                        model.ExchangeRatesInfo_PointsOfSale = model.ExchangeRatesInfo_PointsOfSale.OrderBy(m => m).ToArray();
                        foreach (var i in rate)
                        {
                            var pos = i.tblExchangeRates_PointsOfSales.Where(m => m.dateDeleted == null).Select(m => m.pointOfSaleID).OrderBy(m => m).ToArray();
                            //if (pos.SequenceEqual(model.ExchangeRatesInfo_PointsOfSale))
                            //{
                            //    rate = rate.Where(m => m.exchangeRateID != i.exchangeRateID);
                            //}
                            if (pos.SequenceEqual(model.ExchangeRatesInfo_PointsOfSale))
                            {
                                rate = rate.Where(m => m.exchangeRateID == i.exchangeRateID);
                                break;
                            }
                            else
                            {
                                rate = rate.Where(m => m.exchangeRateID != i.exchangeRateID);
                            }
                        }
                    }
                    else
                    {
                        rate = rate.Where(m => m.tblExchangeRates_PointsOfSales.Count() == 0);
                    }

                    if (rate.Count() > 0)
                    {
                        return rate.OrderByDescending(m => m.dateSaved).FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }

                public static string ValidateModificationOfRate(ExchangeRatesModel.ExchangeRatesInfoModel model)
                {
                    ePlatEntities db = new ePlatEntities();

                    var rate = db.tblExchangeRates.Single(m => m.exchangeRateID == model.ExchangeRatesInfo_ExchangeRateID);
                    //savedRate is not permanent and saved toDate is less than now(if savedRate is closed) or yesterday
                    if (rate.toDate != null && rate.toDate <= (rate.toDate.Value.Hour != 0 ? DateTime.Now : DateTime.Today.AddSeconds(-1)))
                    {
                        //rate is expired
                        return "expired";
                    }
                    else
                    {
                        //if rate, fromCurrency, toCurrency, fromDate, terminal or provider are distinct of saved data
                        if (rate.exchangeRate != decimal.Parse(model.ExchangeRatesInfo_ExchangeRate)
                       || rate.fromCurrencyID != int.Parse(model.ExchangeRatesInfo_FromCurrency)
                       || rate.toCurrencyID != int.Parse(model.ExchangeRatesInfo_ToCurrency)
                       || rate.fromDate.Date != DateTime.Parse(model.ExchangeRatesInfo_I_Date).Date
                       || rate.terminalID != model.ExchangeRatesInfo_Terminal
                       || rate.providerID != (model.ExchangeRatesInfo_Provider == 0 ? (int?)null : model.ExchangeRatesInfo_Provider))
                        {
                            //attempt to modification. check if it is not in use before authorize
                            //var isRateUsed = db.tblPurchaseServiceDetails.Where(m => m.exchangeRateID == rate.exchangeRateID).Count() != 0 ? true : false;
                            if (rate.providerID != null)
                            {
                                var isRateUsed = (from ps in db.tblPurchases_Services
                                                  join s in db.tblServices on ps.serviceID equals s.serviceID
                                                  where s.providerID == rate.providerID
                                                  && ps.dateSaved >= rate.fromDate && (rate.permanent_ || ps.dateSaved <= rate.toDate)
                                                  select ps).Count() > 0 ? true : false;


                                if (!isRateUsed)
                                {
                                    var _provider = db.tblProviders.Single(m => m.providerID == rate.providerID);
                                    var _coupons = db.tblPurchases_Services.Where(m => m.tblServices.providerID == rate.providerID && m.currencyID != _provider.contractCurrencyID);
                                    if (_coupons.Count() > 0)
                                    {
                                        //anexar hora del movimiento más reciente
                                        return "in use";
                                    }
                                    else
                                    {
                                        return "not in use";
                                    }
                                }
                                else
                                {
                                    return "in use";
                                }
                            }
                            else
                            {
                                var isRateUsed = db.tblPurchaseServiceDetails.Where(m => m.exchangeRateID == rate.exchangeRateID).Count() != 0 ? true : false;
                                if (!isRateUsed)
                                {
                                    var _coupons = db.tblPurchases_Services.Where(m => m.tblPurchases.terminalID == rate.terminalID && m.dateSaved >= rate.fromDate && (rate.permanent_ || m.dateSaved <= rate.toDate) && m.tblPurchases.currencyID == rate.fromCurrencyID);
                                    var _payments = db.tblPaymentDetails.Where(m => m.tblPurchases.terminalID == rate.terminalID && m.dateSaved >= rate.fromDate && (rate.permanent_ || m.dateSaved <= rate.toDate) && m.currencyID == rate.fromCurrencyID && (m.deleted == null || m.deleted == false));
                                    //if (_coupons.Count() > 0)
                                    if (_coupons.Count() > 0 || _payments.Count() > 0)
                                    {
                                        return "in use";
                                    }
                                    else
                                    {
                                        return "not in use";
                                    }
                                }
                                else
                                {
                                    return "in use";
                                }
                            }
                        }
                        else
                        {
                            if (model.ExchangeRatesInfo_Permanent)
                            {
                                return "not in use";// true;
                            }
                            else
                            {
                                if (DateTime.Parse(model.ExchangeRatesInfo_F_Date).Date >= DateTime.Today)
                                {
                                    return "not in use";//true;
                                }
                                else
                                {
                                    if (model.ExchangeRatesInfo_Provider != 0)
                                    {
                                        var _provider = db.tblProviders.Single(m => m.providerID == (int)rate.providerID);
                                        var _coupons = db.tblPurchases_Services.Where(m => m.currencyID != _provider.contractCurrencyID && m.dateSaved >= DateTime.Today && m.tblServices.providerID == _provider.providerID);
                                        if (_coupons.Count() > 0)
                                        {
                                            return "not in use";
                                            //return false;
                                        }
                                        else
                                        {
                                            return "not in use";//return true;
                                        }
                                    }
                                    else
                                    {
                                        return "in use";
                                        //return false;
                                    }
                                }
                            }
                        }
                    }
                }

                public static string _ValidateModificationOfRate(ExchangeRatesModel.ExchangeRatesInfoModel model)
                {
                    ePlatEntities db = new ePlatEntities();

                    var rate = db.tblExchangeRates.Single(m => m.exchangeRateID == model.ExchangeRatesInfo_ExchangeRateID);
                    //savedRate is not permanent and saved toDate is less than now(if savedRate is closed) or yesterday
                    if (rate.toDate != null && rate.toDate <= (rate.toDate.Value.Hour != 0 ? DateTime.Now : DateTime.Today.AddSeconds(-1)))
                    {
                        //rate is expired
                        return "expired";
                    }
                    else
                    {
                        //if rate, fromCurrency, toCurrency, fromDate, terminal or provider are distinct of saved data
                        if (rate.exchangeRate != decimal.Parse(model.ExchangeRatesInfo_ExchangeRate)
                       || rate.fromCurrencyID != int.Parse(model.ExchangeRatesInfo_FromCurrency)
                       || rate.toCurrencyID != int.Parse(model.ExchangeRatesInfo_ToCurrency)
                       || rate.fromDate.Date != DateTime.Parse(model.ExchangeRatesInfo_I_Date).Date
                       || rate.terminalID != model.ExchangeRatesInfo_Terminal
                       || rate.providerID != (model.ExchangeRatesInfo_Provider == 0 ? (int?)null : model.ExchangeRatesInfo_Provider))
                        {
                            //attempt to modification. check if it is not in use before authorize
                            var isRateUsed = db.tblPurchaseServiceDetails.Where(m => m.exchangeRateID == rate.exchangeRateID).Count() != 0 ? true : false;
                            if (rate.providerID != null)
                            {
                                if (!isRateUsed)
                                {
                                    var _provider = db.tblProviders.Single(m => m.providerID == rate.providerID);
                                    var _coupons = db.tblPurchases_Services.Where(m => m.tblServices.providerID == rate.providerID && m.currencyID != _provider.contractCurrencyID);
                                    if (_coupons.Count() > 0)
                                    {
                                        //anexar hora del movimiento más reciente
                                        return "in use";
                                    }
                                    else
                                    {
                                        return "not in use";
                                    }
                                }
                                else
                                {
                                    return "in use";
                                }
                            }
                            else
                            {
                                if (!isRateUsed)
                                {
                                    var _coupons = db.tblPurchases_Services.Where(m => m.tblPurchases.terminalID == rate.terminalID && m.dateSaved >= rate.fromDate && (rate.permanent_ || m.dateSaved <= rate.toDate) && m.tblPurchases.currencyID == rate.fromCurrencyID);
                                    var _payments = db.tblPaymentDetails.Where(m => m.tblPurchases.terminalID == rate.terminalID && m.dateSaved >= rate.fromDate && (rate.permanent_ || m.dateSaved <= rate.toDate) && m.currencyID == rate.fromCurrencyID && (m.deleted == null || m.deleted == false));
                                    //if (_coupons.Count() > 0)
                                    if (_coupons.Count() > 0 || _payments.Count() > 0)
                                    {
                                        return "in use";
                                    }
                                    else
                                    {
                                        return "not in use";
                                    }
                                }
                                else
                                {
                                    return "in use";
                                }
                            }
                        }
                        else
                        {
                            if (model.ExchangeRatesInfo_Permanent)
                            {
                                return "not in use";// true;
                            }
                            else
                            {
                                if (DateTime.Parse(model.ExchangeRatesInfo_F_Date).Date >= DateTime.Today)
                                {
                                    return "not in use";//true;
                                }
                                else
                                {
                                    if (model.ExchangeRatesInfo_Provider != 0)
                                    {
                                        var _provider = db.tblProviders.Single(m => m.providerID == (int)rate.providerID);
                                        var _coupons = db.tblPurchases_Services.Where(m => m.currencyID != _provider.contractCurrencyID && m.dateSaved >= DateTime.Today && m.tblServices.providerID == _provider.providerID);
                                        if (_coupons.Count() > 0)
                                        {
                                            return "not in use";
                                            //return false;
                                        }
                                        else
                                        {
                                            return "not in use";//return true;
                                        }
                                    }
                                    else
                                    {
                                        return "in use";
                                        //return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            public List<ExchangeRatesModel.ExchangeRatesInfoModel> SearchExchangeRates(ExchangeRatesModel.SearchExchangeRatesModel model)
            {
                List<ExchangeRatesModel.ExchangeRatesInfoModel> list = new List<ExchangeRatesModel.ExchangeRatesInfoModel>();
                ePlatEntities db = new ePlatEntities();

                var emptyModelFlag = true;
                var terminals = model.SearchExchangeRates_Terminal != null ? model.SearchExchangeRates_Terminal.Select(m => (long?)long.Parse(m)).ToArray() : session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();
                var fromDate = (DateTime?)null;
                var toDate = (DateTime?)null;
                var fromCurrency = new int[] { };
                var toCurrency = new int[] { };
                var providers = new int?[] { };
                var ExchangeRateTypeID = new int[] { };
                var ExchangeRateTypename = new string[] { };


                if (model.SearchExchangeRates_FromDate != null)
                {
                    fromDate = DateTime.Parse(model.SearchExchangeRates_FromDate).Date == DateTime.Today ? DateTime.Today : DateTime.Parse(model.SearchExchangeRates_FromDate);
                    emptyModelFlag = false;
                }
                if (model.SearchExchangeRates_FromCurrency != null)
                {
                    fromCurrency = db.tblCurrencies.Where(m => model.SearchExchangeRates_FromCurrency.Contains(m.currencyCode)).Select(m => m.currencyID).ToArray();
                    emptyModelFlag = false;
                }
                if (model.SearchExchangeRates_ToCurrency != null)
                {
                    toCurrency = db.tblCurrencies.Where(m => model.SearchExchangeRates_ToCurrency.Contains(m.currencyCode)).Select(m => m.currencyID).ToArray();
                    emptyModelFlag = false;
                }

                if (model.SearchExchangeRates_ExchangeRateType != null)
                {
                    ExchangeRateTypeID = model.SearchExchangeRates_ExchangeRateType;
                    ExchangeRateTypename = model.SearchExchangeRates_ExchangeRateTypeText;
                    emptyModelFlag = false;
                }
                if (model.SearchExchangeRates_Providers != null)
                {
                    providers = model.SearchExchangeRates_Providers.Select(m => (int?)int.Parse(m)).ToArray();
                    emptyModelFlag = false;
                }

                var query = from rate in db.tblExchangeRates
                            where
                            (fromDate == null || rate.fromDate <= fromDate || rate.fromDate >= fromDate)
                            && ((fromDate == null || fromDate <= rate.toDate) || rate.toDate == null)
                            && (fromCurrency.Count() == 0 || fromCurrency.Contains(rate.fromCurrencyID))
                            && (toCurrency.Count() == 0 || toCurrency.Contains(rate.toCurrencyID))
                        && (((providers.Count() == 0 || providers.Contains(rate.providerID)) && (model.SearchExchangeRates_OptionProvider == true)) || (model.SearchExchangeRates_OptionProvider == false && rate.providerID == null))
                            && terminals.Contains(rate.terminalID)
                            && ((ExchangeRateTypeID.Count() == 0) || (ExchangeRateTypeID.Contains(rate.exchangeRateTypeID)))
                            orderby rate.fromDate descending
                            select rate;

                //var Users = from user in db.tblUserProfiles
                //            select user;

                var Users = UserDataModel.UserCatalogs.FillDrpUsersInWorkGroup(true).Select(m => new { Value = Guid.Parse(m.Value), m.Text }).ToList();

                var _permanent = query.Where(m => m.permanent_);

                foreach (var i in _permanent)
                {
                    list.Add(new ExchangeRatesModel.ExchangeRatesInfoModel()
                    {
                        ExchangeRatesInfo_ExchangeRateID = i.exchangeRateID,
                        ExchangeRatesInfo_ExchangeRate = i.exchangeRate.ToString(),
                        ExchangeRatesInfo_I_Date = i.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                        ExchangeRatesInfo_F_Date = i.toDate != null ? DateTime.Parse(i.toDate.ToString()).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) : "Permanent",
                        ExchangeRatesInfo_FromCurrencyText = (i.tblCurrencies.currency + " - " + i.tblCurrencies1.currency),
                        ExchangeRatesInfo_ProviderTerminal = i.providerID != null ? i.tblProviders.comercialName : i.terminalID != null ? i.tblTerminals.terminal : "",
                        ExchangeRatesInfo_ExchangeRateTypeText = i.tblExchangeRateTypes.exchangeRateType,
                        ExchangeRatesInfo_ExchangeRateDateSaved = i.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt"),
                        //ExchangeRatesInfo_ExchangeRateUser = Users.FirstOrDefault(x => x.userID == i.savedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == i.savedByUserID).lastName,
                        ExchangeRatesInfo_ExchangeRateUser = Users.FirstOrDefault(x => x.Value == i.savedByUserID).Text,
                        ExchangeRatesInfo_ExchangeRateLastModifyDate = i.dateLastModification == null ? "" : i.dateLastModification.Value.ToString("yyyy-MM-dd hh:mm:ss tt"),
                        //ExchangeRatesInfo_ExchangeRateLastModifyUser = Users.FirstOrDefault(x => x.userID == i.modifiedByUserID) == null ? "" : Users.FirstOrDefault(x => x.userID == i.modifiedByUserID).firstName + " " +
                        //                                               Users.FirstOrDefault(x => x.userID == i.modifiedByUserID).lastName
                        ExchangeRatesInfo_ExchangeRateLastModifyUser = i.modifiedByUserID != null ? Users.FirstOrDefault(x => x.Value == i.modifiedByUserID).Text : ""
                    });
                }

                var _count = emptyModelFlag ? 100 : (query.Count() - _permanent.Count());

                foreach (var i in query.Where(m => !m.permanent_).OrderByDescending(m => m.fromDate))
                {
                    list.Add(new ExchangeRatesModel.ExchangeRatesInfoModel()
                    {
                        ExchangeRatesInfo_ExchangeRateID = i.exchangeRateID,
                        ExchangeRatesInfo_ExchangeRate = i.exchangeRate.ToString(),
                        ExchangeRatesInfo_I_Date = i.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                        ExchangeRatesInfo_F_Date = i.toDate != null ? DateTime.Parse(i.toDate.ToString()).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) : "Permanent",
                        ExchangeRatesInfo_FromCurrencyText = (i.tblCurrencies.currency + " - " + i.tblCurrencies1.currency),
                        ExchangeRatesInfo_ProviderTerminal = i.providerID != null ? i.tblProviders.comercialName : i.terminalID != null ? i.tblTerminals.terminal : "",
                        ExchangeRatesInfo_ExchangeRateTypeText = i.tblExchangeRateTypes.exchangeRateType,
                        ExchangeRatesInfo_ExchangeRateDateSaved = i.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt"),
                        //ExchangeRatesInfo_ExchangeRateUser = Users.FirstOrDefault(x => x.userID == i.savedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == i.savedByUserID).lastName,
                        ExchangeRatesInfo_ExchangeRateUser = Users.FirstOrDefault(x => x.Value == i.savedByUserID).Text,
                        ExchangeRatesInfo_ExchangeRateLastModifyDate = i.dateLastModification == null ? "" : i.dateLastModification.Value.ToString("yyyy-MM-dd hh:mm:ss tt"),
                        //ExchangeRatesInfo_ExchangeRateLastModifyUser = Users.FirstOrDefault(x => x.userID == i.modifiedByUserID) == null ? "" : Users.FirstOrDefault(x => x.userID == i.modifiedByUserID).firstName + " " +
                        //                                               Users.FirstOrDefault(x => x.userID == i.modifiedByUserID).lastName
                        ExchangeRatesInfo_ExchangeRateLastModifyUser = i.modifiedByUserID != null ? Users.FirstOrDefault(x => x.Value == i.modifiedByUserID).Text : ""
                    });
                    _count--;
                    if (_count == 0)
                    {
                        break;
                    }
                }
                return list;
            }

            public AttemptResponse SaveExchangeRate(ExchangeRatesModel.ExchangeRatesInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                var Users = from user in db.tblUserProfiles
                            select user;

                if (model.ExchangeRatesInfo_PointsOfSale != null && model.ExchangeRatesInfo_PointsOfSale.Count() == MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(model.ExchangeRatesInfo_Terminal).Count())
                {
                    model.ExchangeRatesInfo_PointsOfSale = null;
                }

                if (model.ExchangeRatesInfo_ExchangeRateID != 0)
                {
                    #region "update"
                    try
                    {
                        switch (ExchangeRatesCatalogs.ValidateModificationOfRate(model))
                        {
                            case "expired":
                                {
                                    //intento de actualizar un registro expirado. Si es de costo se crea un registro nuevo con información del modelo y el existente se cierra un segundo antes de la fecha de inicio del nuevo.
                                    //Si no es costo, no se permite realizar actualización. Se verifica que no se cambie el tipo de registro en el intento de actualización.
                                    #region
                                    if (model.ExchangeRatesInfo_ExchangeRateType >= 4)//cost
                                    {
                                        var _oldRate = db.tblExchangeRates.Single(m => m.exchangeRateID == model.ExchangeRatesInfo_ExchangeRateID);
                                        if (_oldRate.exchangeRateTypeID >= 4)
                                        {
                                            //user= ultimo usario que modifico
                                            //date = ultima fecha de modificacion
                                            tblExchangeRates query = new tblExchangeRates();
                                            query.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
                                            query.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
                                            query.permanent_ = model.ExchangeRatesInfo_Permanent;
                                            query.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                            query.fromCurrencyID = int.Parse(model.ExchangeRatesInfo_FromCurrency);
                                            query.toCurrencyID = int.Parse(model.ExchangeRatesInfo_ToCurrency);
                                            query.providerID = model.ExchangeRatesInfo_Provider != 0 ? model.ExchangeRatesInfo_Provider : (int?)null;
                                            query.terminalID = model.ExchangeRatesInfo_Terminal != 0 ? model.ExchangeRatesInfo_Terminal : (long?)null;
                                            query.dateLastModification = DateTime.Now;
                                            query.modifiedByUserID = session.UserID;
                                            query.exchangeRateTypeID = model.ExchangeRatesInfo_ExchangeRateType;
                                            db.tblExchangeRates.AddObject(query);
                                            _oldRate.toDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).AddSeconds(-1);
                                            _oldRate.permanent_ = false;
                                            db.SaveChanges();
                                            response.Type = Attempt_ResponseTypes.Ok;
                                            response.Message = "Exchange Rate Saved";
                                            response.ObjectID = new
                                            {
                                                exchangeRateID = query.exchangeRateID,
                                                fromDate = query.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                                toDate = (query.permanent_ ? "Permanent" : ((DateTime)query.toDate).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)),
                                                dateSaved = query.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt"),
                                                saveByUser = Users.FirstOrDefault(x => x.userID == query.savedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == query.savedByUserID).lastName,
                                                lastModifiedDate = ((DateTime)query.dateLastModification).ToString("yyyy-MM-dd hh:mm:ss tt"),
                                                lastModifiedUser = Users.FirstOrDefault(x => x.userID == query.modifiedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == query.modifiedByUserID).lastName
                                            };
                                            return response;
                                        }
                                        else//try to change type in an expired rate
                                        {
                                            throw new Exception("Expired Rates Cannot Be Modified");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Expired Rates Cannot Be Modified");
                                    }
                                    #endregion
                                }
                            case "in use":
                                {
                                    //intento de actualizar un registro que esté vigente y en uso. Se crea uno nuevo iniciando inmediatamente y el registro existente se cierra un segundo antes de la fecha de inicio del nuevo.
                                    //
                                    var _oldRate = db.tblExchangeRates.Single(m => m.exchangeRateID == model.ExchangeRatesInfo_ExchangeRateID);
                                    var _now = DateTime.Now;
                                    tblExchangeRates query = new tblExchangeRates();

                                    if (_oldRate.terminalID != model.ExchangeRatesInfo_Terminal)
                                    {
                                        throw new Exception("Cannot change terminal of an exchange rate in use.<br />Please create a new exchange rate by pressing the NEW button.");
                                    }

                                    query.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
                                    if (model.ExchangeRatesInfo_ExchangeRateType >= 4)//cost
                                    {
                                        query.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
                                        query.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                        query.permanent_ = model.ExchangeRatesInfo_Permanent;
                                        _oldRate.toDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).AddSeconds(-1);
                                        _oldRate.permanent_ = false;
                                    }
                                    else//general
                                    {
                                        if (!model.ExchangeRatesInfo_Permanent && DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).Date < DateTime.Today)
                                        {
                                            throw new Exception("Cannot create/update exchange rates in a date prior than today");
                                        }
                                        query.permanent_ = model.ExchangeRatesInfo_Permanent;
                                        query.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).Date <= DateTime.Today ? _now : DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
                                        query.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                        _oldRate.permanent_ = false;
                                        _oldRate.toDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).Date <= DateTime.Today ? _now.AddSeconds(-1) : DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).AddSeconds(-1);
                                    }
                                    query.fromCurrencyID = int.Parse(model.ExchangeRatesInfo_FromCurrency);
                                    query.toCurrencyID = int.Parse(model.ExchangeRatesInfo_ToCurrency);
                                    query.providerID = model.ExchangeRatesInfo_Provider != 0 ? model.ExchangeRatesInfo_Provider : (int?)null;
                                    query.terminalID = model.ExchangeRatesInfo_Terminal != 0 ? model.ExchangeRatesInfo_Terminal : (long?)null;

                                    query.dateSaved = _now;//datesaved
                                    query.savedByUserID = session.UserID;
                                    query.exchangeRateTypeID = model.ExchangeRatesInfo_ExchangeRateType;
                                    _oldRate.dateLastModification = _now;//datesaved
                                    _oldRate.modifiedByUserID = (Guid)Membership.GetUser("eplat@villagroup.com").ProviderUserKey;//user
                                    ////pos
                                    var currentPoS = _oldRate.tblExchangeRates_PointsOfSales.Where(m => m.dateDeleted == null);
                                    if (currentPoS.Count() > 0)
                                    {
                                        foreach (var i in currentPoS)//close of old rate pos
                                        {
                                            //i.dateDeleted = _now;
                                            i.dateDeleted = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).Date <= DateTime.Today ? _now.AddSeconds(-1) : DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).AddSeconds(-1);
                                            i.deletedByUserID = session.UserID;
                                        }
                                    }
                                    if (model.ExchangeRatesInfo_PointsOfSale != null)
                                    {
                                        foreach (var i in model.ExchangeRatesInfo_PointsOfSale)//creation of pos por new rate
                                        {
                                            var pos = new tblExchangeRates_PointsOfSales();
                                            pos.pointOfSaleID = i;
                                            pos.dateAdded = _now;
                                            pos.addedByuserID = session.UserID;
                                            query.tblExchangeRates_PointsOfSales.Add(pos);
                                        }
                                    }
                                    ////end pos
                                    db.tblExchangeRates.AddObject(query);
                                    db.SaveChanges();
                                    response.Type = Attempt_ResponseTypes.Ok;
                                    response.Message = "Exchange Rate Saved";//anexar información de nuevo registro en mensaje (fechas)
                                    response.ObjectID = new
                                    {
                                        exchangeRateID = query.exchangeRateID,
                                        fromDate = query.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                        toDate = (query.permanent_ ? "Permanent" : ((DateTime)query.toDate).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)),
                                        dateSaved = query.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt"),
                                        saveByUser = Users.FirstOrDefault(x => x.userID == query.savedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == query.savedByUserID).lastName,
                                        lastModifiedDate = ((DateTime)query.dateLastModification).ToString("yyyy-MM-dd hh:mm:ss tt"),
                                        lastModifiedUser = Users.FirstOrDefault(x => x.userID == query.modifiedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == query.modifiedByUserID).lastName
                                    };
                                    return response;
                                }
                            case "not in use":
                                {
                                    //intento de actualizar un registro que no está en uso y vigente. Se actualiza con la información del modelo el registro existente debido a que no genera conflicto alguno.  
                                    var _oldRate = db.tblExchangeRates.Single(m => m.exchangeRateID == model.ExchangeRatesInfo_ExchangeRateID);
                                    _oldRate.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
                                    _oldRate.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
                                    _oldRate.permanent_ = model.ExchangeRatesInfo_Permanent;
                                    _oldRate.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                    _oldRate.fromCurrencyID = int.Parse(model.ExchangeRatesInfo_FromCurrency);
                                    _oldRate.toCurrencyID = int.Parse(model.ExchangeRatesInfo_ToCurrency);
                                    _oldRate.providerID = model.ExchangeRatesInfo_Provider != 0 ? model.ExchangeRatesInfo_Provider : (int?)null;
                                    _oldRate.terminalID = model.ExchangeRatesInfo_Terminal != 0 ? model.ExchangeRatesInfo_Terminal : (long?)null;
                                    _oldRate.dateLastModification = DateTime.Now;
                                    _oldRate.modifiedByUserID = session.UserID;
                                    _oldRate.exchangeRateTypeID = model.ExchangeRatesInfo_ExchangeRateType;
                                    ////PoS
                                    var currentPoS = _oldRate.tblExchangeRates_PointsOfSales.Where(m => m.dateDeleted == null && m.deletedByUserID == null).Select(m => m.pointOfSaleID).ToArray();

                                    if (model.ExchangeRatesInfo_PointsOfSale != null)
                                    {
                                        foreach (var pos in model.ExchangeRatesInfo_PointsOfSale)
                                        {
                                            if (currentPoS.Contains(pos))
                                            {
                                                currentPoS = currentPoS.Where(m => m != pos).ToArray();
                                            }
                                            else
                                            {
                                                var _pos = new tblExchangeRates_PointsOfSales();
                                                _pos.pointOfSaleID = pos;
                                                //_pos.exchangeRateID = _oldRate.exchangeRateID;
                                                _pos.dateAdded = DateTime.Now;
                                                _pos.addedByuserID = session.UserID;
                                                _oldRate.tblExchangeRates_PointsOfSales.Add(_pos);
                                            }
                                        }
                                    }
                                    if (currentPoS.Count() > 0)
                                    {
                                        foreach (var pos in currentPoS)
                                        {
                                            var _pos = _oldRate.tblExchangeRates_PointsOfSales.Single(m => m.pointOfSaleID == pos && m.dateDeleted == null && m.deletedByUserID == null);
                                            _pos.dateDeleted = DateTime.Now;
                                            _pos.deletedByUserID = session.UserID;
                                        }
                                    }
                                    ////end PoS
                                    db.SaveChanges();
                                    response.Type = Attempt_ResponseTypes.Ok;
                                    response.Message = "Exchange Rate Updated";
                                    response.ObjectID = new
                                    {
                                        exchangeRateID = model.ExchangeRatesInfo_ExchangeRateID,
                                        fromDate = _oldRate.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                        toDate = (_oldRate.permanent_ ? "Permanent" : ((DateTime)_oldRate.toDate).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)),
                                        dateSaved = _oldRate.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt"),
                                        saveByUser = Users.FirstOrDefault(x => x.userID == _oldRate.savedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == _oldRate.savedByUserID).lastName,
                                        lastModifiedDate = ((DateTime)_oldRate.dateLastModification).ToString("yyyy-MM-dd hh:mm:ss tt"),
                                        lastModifiedUser = Users.FirstOrDefault(x => x.userID == _oldRate.modifiedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == _oldRate.modifiedByUserID).lastName
                                    };
                                    return response;
                                }
                            default:
                                {
                                    throw new Exception();
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Exchange Rate NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
                else
                {
                    //check if rate exists
                    var rateExists = ExchangeRatesCatalogs.RateAlreadyExists(model, ref db);
                    var _now = DateTime.Now;
                    if (rateExists == null)
                    {
                        #region "Save"
                        try
                        {
                            //intento de nuevo registro sin que la información exista en otro registro. Se crea el nuevo registro.
                            tblExchangeRates query = new tblExchangeRates();
                            query.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
                            //query.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
                            query.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).Date == DateTime.Today ? model.ExchangeRatesInfo_ExchangeRateType >= 4 ? DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture) : _now : DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
                            query.permanent_ = model.ExchangeRatesInfo_Permanent;
                            query.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            query.fromCurrencyID = int.Parse(model.ExchangeRatesInfo_FromCurrency);
                            query.toCurrencyID = int.Parse(model.ExchangeRatesInfo_ToCurrency);
                            query.providerID = model.ExchangeRatesInfo_Provider != 0 ? model.ExchangeRatesInfo_Provider : (int?)null;
                            query.terminalID = model.ExchangeRatesInfo_Terminal != 0 ? model.ExchangeRatesInfo_Terminal : (long?)null;
                            query.dateSaved = _now;
                            query.savedByUserID = session.UserID;
                            query.exchangeRateTypeID = model.ExchangeRatesInfo_ExchangeRateType;

                            if (model.ExchangeRatesInfo_PointsOfSale != null)
                            {
                                foreach (var i in model.ExchangeRatesInfo_PointsOfSale)
                                {
                                    tblExchangeRates_PointsOfSales pos = new tblExchangeRates_PointsOfSales();
                                    pos.pointOfSaleID = i;
                                    pos.dateAdded = _now;
                                    pos.addedByuserID = session.UserID;
                                    query.tblExchangeRates_PointsOfSales.Add(pos);
                                }
                            }
                            db.tblExchangeRates.AddObject(query);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Exchange Rate Saved";
                            response.ObjectID = new
                            {
                                exchangeRateID = query.exchangeRateID,
                                fromDate = query.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                toDate = (query.permanent_ ? "Permanent" : ((DateTime)query.toDate).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)),
                                dateSaved = query.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt"),
                                saveByUser = Users.FirstOrDefault(x => x.userID == query.savedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == query.savedByUserID).lastName,
                                lastModifiedDate = "",
                                lastModifiedUser = query.modifiedByUserID != null ? Users.FirstOrDefault(x => x.userID == query.modifiedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == query.modifiedByUserID).lastName : ""
                            };
                            return response;
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Message = "Exchange Rate NOT Saved";
                            response.ObjectID = 0;
                            response.Exception = ex;
                            return response;
                        }
                        #endregion
                    }
                    else
                    {
                        try
                        {
                            //intento de nuevo registro encontrando un registro existente con la misma información. Se crea el nuevo registro con la fecha de inicio del modelo y el registro existente se cierra con la fecha de inicio del nuevo menos un segundo si no es costo,
                            //caso contrario se cierra en la fecha de inicio del nuevo con 00:00hrs y el existente se cierra un segundo antes.
                            //var _now = DateTime.Now;
                            tblExchangeRates query = new tblExchangeRates();
                            query.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
                            query.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).Date == DateTime.Today ? model.ExchangeRatesInfo_ExchangeRateType >= 4 ? DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture) : _now : DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
                            query.permanent_ = model.ExchangeRatesInfo_Permanent;
                            query.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            query.fromCurrencyID = int.Parse(model.ExchangeRatesInfo_FromCurrency);
                            query.toCurrencyID = int.Parse(model.ExchangeRatesInfo_ToCurrency);
                            query.providerID = model.ExchangeRatesInfo_Provider != 0 ? model.ExchangeRatesInfo_Provider : (int?)null;
                            query.terminalID = model.ExchangeRatesInfo_Terminal != 0 ? model.ExchangeRatesInfo_Terminal : (long?)null;
                            query.dateSaved = _now;
                            query.savedByUserID = session.UserID;
                            query.exchangeRateTypeID = model.ExchangeRatesInfo_ExchangeRateType;
                            //db.tblExchangeRates.AddObject(query);
                            rateExists.permanent_ = false;
                            rateExists.toDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).Date == DateTime.Today ? model.ExchangeRatesInfo_ExchangeRateType >= 4 ? DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).AddSeconds(-1) : _now.AddSeconds(-1) : DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture).AddSeconds(-1);
                            ////PoS
                            var currentPos = rateExists.tblExchangeRates_PointsOfSales;
                            if (currentPos.Count() > 0)
                            {
                                foreach (var i in currentPos)//close pos of existing rate
                                {
                                    i.dateDeleted = _now;
                                    i.deletedByUserID = session.UserID;
                                }
                            }
                            if (model.ExchangeRatesInfo_PointsOfSale != null)
                            {
                                foreach (var i in model.ExchangeRatesInfo_PointsOfSale)//create pos of new rate
                                {
                                    tblExchangeRates_PointsOfSales pos = new tblExchangeRates_PointsOfSales();
                                    pos.pointOfSaleID = i;
                                    pos.dateAdded = _now;
                                    pos.addedByuserID = session.UserID;
                                    query.tblExchangeRates_PointsOfSales.Add(pos);
                                }
                            }
                            ////end PoS
                            db.tblExchangeRates.AddObject(query);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Exchange Rate Saved";
                            response.ObjectID = new
                            {
                                exchangeRateID = query.exchangeRateID,
                                fromDate = query.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                toDate = (query.permanent_ ? "Permanent" : ((DateTime)query.toDate).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)),
                                dateSaved = query.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt"),
                                saveByUser = Users.FirstOrDefault(x => x.userID == query.savedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == query.savedByUserID).lastName,
                                lastModifiedDate = "",
                                lastModifiedUser = query.modifiedByUserID != null ? Users.FirstOrDefault(x => x.userID == query.modifiedByUserID).firstName + " " + Users.FirstOrDefault(x => x.userID == query.modifiedByUserID).lastName : ""
                            };
                            return response;
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Message = "Exchange Rate NOT Saved";
                            response.ObjectID = 0;
                            response.Exception = ex;
                            return response;
                        }
                    }
                }
            }

            //public AttemptResponse _SaveExchangeRate(ExchangeRatesModel.ExchangeRatesInfoModel model)
            //{
            //    ePlatEntities db = new ePlatEntities();
            //    AttemptResponse response = new AttemptResponse();
            //    if (model.ExchangeRatesInfo_IsClonation)
            //    {
            //        response = CloneExchangeRate(model);
            //        return response;
            //    }
            //    else
            //    {
            //        if (model.ExchangeRatesInfo_ExchangeRateID != 0)
            //        {
            //            #region "update"
            //            try
            //            {
            //                switch (ExchangeRatesCatalogs.ValidateModificationOfRate(model))
            //                {
            //                    case "expired":
            //                        {
            //                            throw new Exception("Expired Rates Cannot Be Modified");
            //                        }
            //                    case "in use":
            //                        {
            //                            response.Type = Attempt_ResponseTypes.Warning;
            //                            response.Message = "Exchange Rate NOT Updated. It is in use";
            //                            response.Message += "<br >NOTE: New Rate will be created with form data";
            //                            response.ObjectID = new { exchangeRateID = model.ExchangeRatesInfo_ExchangeRateID };
            //                            return response;
            //                        }
            //                    case "not in use":
            //                        {
            //                            var rateExists = ExchangeRatesCatalogs.RateAlreadyExists(model, ref db);
            //                            if (rateExists == null)
            //                            {
            //                                var query = db.tblExchangeRates.Single(m => m.exchangeRateID == model.ExchangeRatesInfo_ExchangeRateID);
            //                                var _oldQuery = from exchange in db.tblExchangeRates
            //                                                where exchange.exchangeRateID == model.ExchangeRatesInfo_ExchangeRateID
            //                                                select exchange;

            //                                query.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
            //                                query.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
            //                                query.permanent_ = model.ExchangeRatesInfo_Permanent;
            //                                query.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
            //                                query.fromCurrencyID = int.Parse(model.ExchangeRatesInfo_FromCurrency);
            //                                query.toCurrencyID = int.Parse(model.ExchangeRatesInfo_ToCurrency);
            //                                query.providerID = model.ExchangeRatesInfo_Provider != 0 ? model.ExchangeRatesInfo_Provider : (int?)null;
            //                                query.terminalID = model.ExchangeRatesInfo_Terminal != 0 ? model.ExchangeRatesInfo_Terminal : (long?)null;
            //                                query.savedByUserID = _oldQuery.FirstOrDefault().savedByUserID;
            //                                query.dateSaved = _oldQuery.FirstOrDefault().dateSaved;
            //                                query.dateLastModification = DateTime.Now;
            //                                query.exchangeRateTypeID = model.ExchangeRatesInfo_ExchangeRateType;
            //                                db.SaveChanges();
            //                                response.Type = Attempt_ResponseTypes.Ok;
            //                                response.Message = "Exchange Rate Updated";
            //                                response.ObjectID = new { exchangeRateID = model.ExchangeRatesInfo_ExchangeRateID, fromDate = query.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture), toDate = (query.permanent_ ? "Permanent" : ((DateTime)query.toDate).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)) };
            //                                return response;
            //                            }
            //                            else
            //                            {
            //                                #region "return message of rate with same data exists"
            //                                response.Type = Attempt_ResponseTypes.Warning;//comentado para impedir la opción de clonación hasta estar seguro del proceso a seguir
            //                                response.Message = "Exchange Rate NOT Saved. It already exists:"
            //                                    + "<br />1 " + rateExists.tblCurrencies.currencyCode + " = " + rateExists.exchangeRate + " " + rateExists.tblCurrencies1.currencyCode
            //                                    + " applied to " + (rateExists.providerID != null ? rateExists.tblProviders.comercialName : rateExists.tblTerminals.terminal);
            //                                response.Message += "<br >NOTE: New Rate will be created with form data";
            //                                response.ObjectID = new { exchangeRateID = rateExists.exchangeRateID };
            //                                return response;
            //                                #endregion
            //                            }
            //                        }
            //                    default:
            //                        {
            //                            throw new Exception();
            //                        }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                response.Type = Attempt_ResponseTypes.Error;
            //                response.Message = "Exchange Rate NOT Updated";
            //                response.ObjectID = 0;
            //                response.Exception = ex;
            //                return response;
            //            }
            //            #endregion

            //            #region "Update commented"
            //            //if (db.tblPurchaseServiceDetails.Where(m => m.exchangeRateID == model.ExchangeRatesInfo_ExchangeRateID).Count() == 0)
            //            //if (ExchangeRatesCatalogs.AttemptToCloseRate(model))
            //            {
            //                #region "try to update exchange rate not being in use"
            //                //try
            //                //{
            //                //    var query = db.tblExchangeRates.Single(m => m.exchangeRateID == model.ExchangeRatesInfo_ExchangeRateID);
            //                //    query.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
            //                //    query.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
            //                //    query.permanent_ = model.ExchangeRatesInfo_Permanent;
            //                //    query.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
            //                //    query.fromCurrencyID = int.Parse(model.ExchangeRatesInfo_FromCurrency);
            //                //    query.toCurrencyID = int.Parse(model.ExchangeRatesInfo_ToCurrency);
            //                //    query.providerID = model.ExchangeRatesInfo_Provider != 0 ? model.ExchangeRatesInfo_Provider : (int?)null;
            //                //    query.terminalID = model.ExchangeRatesInfo_Terminal != 0 ? model.ExchangeRatesInfo_Terminal : (long?)null;
            //                //    db.SaveChanges();
            //                //    response.Type = Attempt_ResponseTypes.Ok;
            //                //    response.Message = "Exchange Rate Updated";
            //                //    response.ObjectID = new { exchangeRateID = model.ExchangeRatesInfo_ExchangeRateID };
            //                //    return response;
            //                //}
            //                //catch (Exception ex)
            //                //{
            //                //    response.Type = Attempt_ResponseTypes.Error;
            //                //    response.Message = "Exchange Rate NOT Updated";
            //                //    response.ObjectID = 0;
            //                //    response.Exception = ex;
            //                //    return response;
            //                //}
            //                #endregion
            //            }
            //            //else
            //            {
            //                #region "return message of rate being in use"
            //                //response.Type = Attempt_ResponseTypes.Warning;
            //                //response.Message = "Exchange Rate NOT Updated. It is in use";
            //                //response.ObjectID = new { exchangeRateID = model.ExchangeRatesInfo_ExchangeRateID };
            //                //return response;
            //                #endregion
            //            }
            //            #endregion
            //        }
            //        else
            //        {
            //            //check if rate exists
            //            var rateExists = ExchangeRatesCatalogs.RateAlreadyExists(model, ref db);
            //            if (rateExists == null)
            //            {
            //                #region "Save"
            //                try
            //                {
            //                    tblExchangeRates query = new tblExchangeRates();
            //                    query.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
            //                    query.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date, CultureInfo.InvariantCulture);
            //                    query.permanent_ = model.ExchangeRatesInfo_Permanent;
            //                    query.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
            //                    query.fromCurrencyID = int.Parse(model.ExchangeRatesInfo_FromCurrency);
            //                    query.toCurrencyID = int.Parse(model.ExchangeRatesInfo_ToCurrency);
            //                    query.providerID = model.ExchangeRatesInfo_Provider != 0 ? model.ExchangeRatesInfo_Provider : (int?)null;
            //                    query.terminalID = model.ExchangeRatesInfo_Terminal != 0 ? model.ExchangeRatesInfo_Terminal : (long?)null;
            //                    query.dateSaved = DateTime.Now;
            //                    query.exchangeRateTypeID = model.ExchangeRatesInfo_ExchangeRateType;
            //                    db.tblExchangeRates.AddObject(query);
            //                    db.SaveChanges();
            //                    response.Type = Attempt_ResponseTypes.Ok;
            //                    response.Message = "Exchange Rate Saved";
            //                    response.ObjectID = new { exchangeRateID = query.exchangeRateID };
            //                    return response;
            //                }
            //                catch (Exception ex)
            //                {
            //                    response.Type = Attempt_ResponseTypes.Error;
            //                    response.Message = "Exchange Rate NOT Saved";
            //                    response.ObjectID = 0;
            //                    response.Exception = ex;
            //                    return response;
            //                }
            //                #endregion
            //            }
            //            else
            //            {
            //                #region "return message of rate with same data exists"
            //                response.Type = Attempt_ResponseTypes.Warning;//comentado para impedir la opción de clonación hasta estar seguro del proceso a seguir
            //                response.Message = "Exchange Rate NOT Saved. It already exists:"
            //                    + "<br />1 " + rateExists.tblCurrencies.currencyCode + " = " + rateExists.exchangeRate + " " + rateExists.tblCurrencies1.currencyCode
            //                    + " applied to " + (rateExists.providerID != null ? rateExists.tblProviders.comercialName : rateExists.tblTerminals.terminal);
            //                response.Message += "<br >NOTE: New Rate will be created with form data";
            //                response.ObjectID = new { exchangeRateID = rateExists.exchangeRateID };
            //                return response;
            //                #endregion
            //            }
            //        }
            //    }
            //}

            //public AttemptResponse CloneExchangeRate(long exchangeRateID, decimal exchangeRate, bool isUpdate)

            public AttemptResponse CloneExchangeRate(ExchangeRatesModel.ExchangeRatesInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                tblExchangeRates _newRate = new tblExchangeRates();

                try
                {
                    var query = db.tblExchangeRates.Single(m => m.exchangeRateID == model.ExchangeRatesInfo_RateToBeCloned);
                    var _now = DateTime.Now;
                    //if (model.ExchangeRatesInfo_ExchangeRateID != 0)
                    //{
                    //    //attempt to create clone of existing rate when it is selected and try to modify it
                    //    _rate.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
                    //    _rate.permanent_ = query.permanent_;
                    //    //_rate.fromDate = DateTime.Today;
                    //    _rate.fromDate = query.fromDate.Date == DateTime.Today ? _now : DateTime.Today;
                    //    _rate.toDate = query.toDate;
                    //    _rate.dateSaved = _now;
                    //    _rate.fromCurrencyID = query.fromCurrencyID;
                    //    _rate.toCurrencyID = query.toCurrencyID;
                    //    _rate.terminalID = query.terminalID;
                    //    _rate.providerID = query.providerID;
                    //    _rate.exchangeRateTypeID = query.exchangeRateTypeID;
                    //}
                    //else
                    //{
                    //attempt to save rate and system detects there exist a rate with same data
                    _newRate.exchangeRate = decimal.Parse(model.ExchangeRatesInfo_ExchangeRate);
                    _newRate.permanent_ = model.ExchangeRatesInfo_Permanent;
                    _newRate.fromDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date).Date == DateTime.Today ? _now : DateTime.Parse(model.ExchangeRatesInfo_I_Date).Date < DateTime.Today ? _now : DateTime.Parse(model.ExchangeRatesInfo_I_Date);
                    _newRate.toDate = model.ExchangeRatesInfo_Permanent ? (DateTime?)null : DateTime.Parse(model.ExchangeRatesInfo_F_Date).AddDays(1).AddSeconds(-1);
                    _newRate.dateSaved = _now;
                    _newRate.savedByUserID = session.UserID;
                    _newRate.fromCurrencyID = int.Parse(model.ExchangeRatesInfo_FromCurrency);
                    _newRate.toCurrencyID = int.Parse(model.ExchangeRatesInfo_ToCurrency);
                    _newRate.terminalID = model.ExchangeRatesInfo_Terminal != 0 ? model.ExchangeRatesInfo_Terminal : (long?)null;
                    _newRate.providerID = model.ExchangeRatesInfo_Provider != 0 ? model.ExchangeRatesInfo_Provider : (int?)null;
                    _newRate.exchangeRateTypeID = model.ExchangeRatesInfo_ExchangeRateType;
                    //}
                    db.tblExchangeRates.AddObject(_newRate);
                    query.permanent_ = false;
                    query.toDate = DateTime.Parse(model.ExchangeRatesInfo_I_Date).Date == DateTime.Today ? _now.AddSeconds(-1) : DateTime.Parse(model.ExchangeRatesInfo_I_Date).Date < DateTime.Today ? _now.AddSeconds(-1) : DateTime.Parse(model.ExchangeRatesInfo_I_Date).AddSeconds(-1);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Exchange Rate Saved";

                    response.ObjectID = new { exchangeRateID = _newRate.exchangeRateID, fromDate = _newRate.fromDate, oldRateID = model.ExchangeRatesInfo_RateToBeCloned, oldRateVigency = ((DateTime)query.toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Exchange Rate NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }

            public ExchangeRatesModel.ExchangeRatesInfoModel GetExchangeRate(int ExchangeRatesInfo_ExchangeRateID)
            {
                ePlatEntities db = new ePlatEntities();
                ExchangeRatesModel.ExchangeRatesInfoModel model = new ExchangeRatesModel.ExchangeRatesInfoModel();

                var query = db.tblExchangeRates.Single(m => m.exchangeRateID == ExchangeRatesInfo_ExchangeRateID);
                model.ExchangeRatesInfo_ExchangeRateID = query.exchangeRateID;
                model.ExchangeRatesInfo_ExchangeRate = query.exchangeRate.ToString();
                model.ExchangeRatesInfo_I_Date = query.fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                model.ExchangeRatesInfo_Permanent = query.permanent_;
                model.ExchangeRatesInfo_F_Date = query.toDate != null ? DateTime.Parse(query.toDate.ToString()).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                model.ExchangeRatesInfo_FromCurrency = query.fromCurrencyID.ToString();
                model.ExchangeRatesInfo_ToCurrency = query.toCurrencyID.ToString();
                model.ExchangeRatesInfo_Provider = query.providerID != null ? (int)query.providerID : 0;
                model.ExchangeRatesInfo_Terminal = query.terminalID != null ? (long)query.terminalID : 0;
                model.ExchangeRatesInfo_ExchangeRateType = query.exchangeRateTypeID;
                model.ExchangeRatesInfo_PointsOfSale = query.tblExchangeRates_PointsOfSales.Count() > 0 ? query.tblExchangeRates_PointsOfSales.Where(m => m.dateDeleted == null && m.deletedByUserID == null).Select(m => m.pointOfSaleID).ToArray() : new int[] { };
                return model;
            }

            public AttemptResponse DeleteExchangeRate(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblExchangeRates.Single(m => m.exchangeRateID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Exchange Rate Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Exchange Rate NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class Locations
        {
            public List<LocationsModel.LocationInfoModel> SearchLocations(LocationsModel.SearchLocationsModel model)
            {
                var list = new List<LocationsModel.LocationInfoModel>();
                ePlatEntities db = new ePlatEntities();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var query = from l in db.tblLocations
                            where terminals.Contains(l.terminalID)
                            && (l.location.Contains(model.SearchLocation_Location) || model.SearchLocation_Location == null)
                            select l;

                foreach (var i in query)
                {
                    list.Add(new LocationsModel.LocationInfoModel()
                    {
                        LocationInfo_LocationID = i.locationID,
                        LocationInfo_Location = i.location,
                        LocationInfo_LocationCode = i.locationCode,
                        LocationInfo_DestinationText = i.tblDestinations.destination,
                        LocationInfo_TerminalText = i.tblTerminals.terminal
                    });
                }
                return list;
            }

            public AttemptResponse SaveLocation(LocationsModel.LocationInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.LocationInfo_LocationID != 0)
                {
                    #region "update"
                    try
                    {
                        var query = db.tblLocations.Single(m => m.locationID == model.LocationInfo_LocationID);

                        query.location = model.LocationInfo_Location;
                        query.locationCode = model.LocationInfo_LocationCode;
                        query.destinationID = model.LocationInfo_Destination;
                        query.companiesGroupID = model.LocationInfo_Terminal != query.terminalID ? db.tblTerminals.Single(m => m.terminalID == model.LocationInfo_Terminal).companiesGroupID : query.tblTerminals.companiesGroupID;
                        query.terminalID = model.LocationInfo_Terminal;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Location Updated";
                        response.ObjectID = query.locationID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Location NOT Updated";
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
                        var query = new tblLocations();
                        query.location = model.LocationInfo_Location;
                        query.locationCode = model.LocationInfo_LocationCode;
                        query.destinationID = model.LocationInfo_Destination;
                        query.terminalID = model.LocationInfo_Terminal;
                        query.companiesGroupID = db.tblTerminals.Single(m => m.terminalID == model.LocationInfo_Terminal).companiesGroupID;
                        db.tblLocations.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Location Saved";
                        response.ObjectID = query.locationID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Location NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
            }

            public LocationsModel.LocationInfoModel GetLocation(int LocationInfo_LocationID)
            {
                ePlatEntities db = new ePlatEntities();
                LocationsModel.LocationInfoModel model = new LocationsModel.LocationInfoModel();

                var query = db.tblLocations.Single(m => m.locationID == LocationInfo_LocationID);

                model.LocationInfo_LocationID = query.locationID;
                model.LocationInfo_Location = query.location;
                model.LocationInfo_LocationCode = query.locationCode;
                model.LocationInfo_Destination = query.destinationID;
                model.LocationInfo_Terminal = query.terminalID;
                return model;
            }

            public AttemptResponse DeleteLocation(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblLocations.Single(m => m.locationID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Location Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Location NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class Zones
        {
            public List<ZonesModel.ZonesInfoModel> SearchZones(ZonesModel.SearchZonesModel model)
            {
                List<ZonesModel.ZonesInfoModel> list = new List<ZonesModel.ZonesInfoModel>();
                ePlatEntities db = new ePlatEntities();

                var query = from zone in db.tblZones
                            where (zone.zone.Contains(model.Search_Zones) || model.Search_Zones == null)
                            orderby zone.zone, zone.tblDestinations.destination ascending
                            select zone;

                foreach (var i in query)
                {
                    list.Add(new ZonesModel.ZonesInfoModel()
                    {
                        ZoneInfo_ZoneID = i.zoneID,
                        ZoneInfo_Zone = i.zone,
                        ZoneInfo_Destination = int.Parse(i.destinationID.ToString()),
                        ZoneInfo_DestinationName = i.tblDestinations.destination
                    });
                }
                return list;
            }

            public AttemptResponse SaveZone(ZonesModel.ZonesInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.ZoneInfo_ZoneID != 0)
                {
                    try
                    {
                        var query = db.tblZones.Single(m => m.zoneID == model.ZoneInfo_ZoneID);
                        query.zone = model.ZoneInfo_Zone;
                        query.destinationID = model.ZoneInfo_Destination;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Zone Updated";
                        response.ObjectID = query.zoneID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Zone NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblZones();
                        query.zone = model.ZoneInfo_Zone;
                        query.destinationID = model.ZoneInfo_Destination;
                        db.tblZones.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Zone Saved";
                        response.ObjectID = query.zoneID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Zone NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public ZonesModel.ZonesInfoModel GetZone(int ZoneInfo_ZoneID)
            {
                ePlatEntities db = new ePlatEntities();
                ZonesModel.ZonesInfoModel model = new ZonesModel.ZonesInfoModel();

                var query = db.tblZones.Single(m => m.zoneID == ZoneInfo_ZoneID);
                model.ZoneInfo_ZoneID = query.zoneID;
                model.ZoneInfo_Zone = query.zone;
                model.ZoneInfo_Destination = int.Parse(query.destinationID.ToString());
                return model;
            }

            public AttemptResponse DeleteZone(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblZones.Single(m => m.zoneID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Zone Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Zone NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class PlaceTypes
        {
            public class PlaceTypesCatalogs
            {
                public static List<SelectListItem> FillDrpActivePlaceTypes()
                {
                    ePlatEntities db = new ePlatEntities();
                    List<SelectListItem> list = new List<SelectListItem>();

                    foreach (var i in db.tblPlaceTypes.Where(m => m.active == true))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.placeTypeID.ToString(),
                            Text = i.placeType
                        });
                    }
                    return list;
                }
            }

            public List<PlaceTypesModel.PlaceTypesInfoModel> SearchPlaceTypes(PlaceTypesModel.SearchPlaceTypesModel model)
            {
                List<PlaceTypesModel.PlaceTypesInfoModel> list = new List<PlaceTypesModel.PlaceTypesInfoModel>();
                ePlatEntities db = new ePlatEntities();

                var query = from pt in db.tblPlaceTypes
                            where (pt.placeType.Contains(model.Search_PlaceTypes) || model.Search_PlaceTypes == null)
                            orderby pt.placeType, pt.active
                            select pt;

                foreach (var i in query)
                {
                    list.Add(new PlaceTypesModel.PlaceTypesInfoModel()
                    {
                        PlaceTypeInfo_PlaceTypeID = i.placeTypeID,
                        PlaceTypeInfo_PlaceType = i.placeType,
                        PlaceTypeInfo_IsActive = i.active
                    });
                }
                return list;
            }

            public AttemptResponse SavePlaceType(PlaceTypesModel.PlaceTypesInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.PlaceTypeInfo_PlaceTypeID != 0)
                {
                    try
                    {
                        var query = db.tblPlaceTypes.Single(m => m.placeTypeID == model.PlaceTypeInfo_PlaceTypeID);
                        query.placeType = model.PlaceTypeInfo_PlaceType;
                        query.active = model.PlaceTypeInfo_IsActive;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Place Type Updated";
                        response.ObjectID = query.placeTypeID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Place Type NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblPlaceTypes();
                        query.placeType = model.PlaceTypeInfo_PlaceType;
                        query.active = model.PlaceTypeInfo_IsActive;
                        db.tblPlaceTypes.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Place Type Saved";
                        response.ObjectID = query.placeTypeID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Place Type NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public PlaceTypesModel.PlaceTypesInfoModel GetPlaceType(int PlaceTypeInfo_PlaceTypeID)
            {
                ePlatEntities db = new ePlatEntities();
                PlaceTypesModel.PlaceTypesInfoModel model = new PlaceTypesModel.PlaceTypesInfoModel();

                var query = db.tblPlaceTypes.Single(m => m.placeTypeID == PlaceTypeInfo_PlaceTypeID);
                model.PlaceTypeInfo_PlaceTypeID = query.placeTypeID;
                model.PlaceTypeInfo_PlaceType = query.placeType;
                model.PlaceTypeInfo_IsActive = query.active;
                return model;
            }

            public AttemptResponse DeletePlaceType(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblPlaceTypes.Single(m => m.placeTypeID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Place Type Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Place Type NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class TransportationZones
        {
            public class TransportationZonesCatalogs
            {

            }

            public List<TransportationZonesModel.TransportationZonesInfoModel> SearchTransportationZones(TransportationZonesModel.SearchTransportationZonesModel model)
            {
                ePlatEntities db = new ePlatEntities();
                List<TransportationZonesModel.TransportationZonesInfoModel> list = new List<TransportationZonesModel.TransportationZonesInfoModel>();

                var query = from tz in db.tblTransportationZones
                            where (tz.transportationZone.Contains(model.Search_TransportationZones) || model.Search_TransportationZones == null)
                            orderby tz.transportationZone, tz.tblDestinations.destination ascending
                            select tz;
                foreach (var i in query)
                {
                    list.Add(new TransportationZonesModel.TransportationZonesInfoModel()
                    {
                        TransportationZoneInfo_TransportationZoneID = i.transportationZoneID,
                        TransportationZoneInfo_TransportationZone = i.transportationZone,
                        TransportationZoneInfo_Destination = int.Parse(i.destinationID.ToString()),
                        TransportationZoneInfo_DestinationName = i.tblDestinations.destination
                    });
                }
                return list;
            }

            public AttemptResponse SaveTransportationZone(TransportationZonesModel.TransportationZonesInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.TransportationZoneInfo_TransportationZoneID != 0)
                {
                    try
                    {
                        var query = db.tblTransportationZones.Single(m => m.transportationZoneID == model.TransportationZoneInfo_TransportationZoneID);
                        query.transportationZone = model.TransportationZoneInfo_TransportationZone;
                        query.destinationID = model.TransportationZoneInfo_Destination;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Transportation Zone Updated";
                        response.ObjectID = query.transportationZoneID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Transportation Zone NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblTransportationZones();
                        query.transportationZone = model.TransportationZoneInfo_TransportationZone;
                        query.destinationID = model.TransportationZoneInfo_Destination;
                        db.tblTransportationZones.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Transportation Zone Saved";
                        response.ObjectID = query.transportationZoneID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Transportation Zone NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public TransportationZonesModel.TransportationZonesInfoModel GetTransportationZone(int TransportationZoneInfo_TransportationZoneID)
            {
                ePlatEntities db = new ePlatEntities();
                TransportationZonesModel.TransportationZonesInfoModel model = new TransportationZonesModel.TransportationZonesInfoModel();

                var query = db.tblTransportationZones.Single(m => m.transportationZoneID == TransportationZoneInfo_TransportationZoneID);
                model.TransportationZoneInfo_TransportationZoneID = query.transportationZoneID;
                model.TransportationZoneInfo_TransportationZone = query.transportationZone;
                model.TransportationZoneInfo_Destination = int.Parse(query.destinationID.ToString());
                return model;
            }

            public AttemptResponse DeleteTransportationZone(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                try
                {
                    var query = db.tblTransportationZones.Single(m => m.transportationZoneID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Transportation Zone Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Transportatio Zone NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class UserLeadSources
        {
            public List<UsersLeadSources.UserLeadSources> SearchUsersLeadSources(UsersLeadSources.SearchUserLeadSources model)
            {
                ePlatEntities db = new ePlatEntities();
                List<UsersLeadSources.UserLeadSources> list = new List<UsersLeadSources.UserLeadSources>();

                var listResorts = PreArrivalDataModel.PreArrivalCatalogs.GetFrontResorts();
                var listSources = LeadSourceDataModel.GetLeadSourcesByTerminal();
                var users = model.Search_Users ?? UserDataModel.GetUsersBySupervisor().Select(m => (Guid?)Guid.Parse(m.Value)).ToArray();
                var resorts = model.Search_Resorts ?? listResorts.Select(m => (long?)long.Parse(m.Value)).ToArray();
                var leadSources = model.Search_LeadSources ?? listSources.Select(m => (long?)long.Parse(m.Value)).ToArray();

                var query = from l in db.tblUsers_LeadSources
                            join u in db.tblUserProfiles on l.userID equals u.userID
                            join up in db.tblUserProfiles on l.savedByUserID equals up.userID into l_up
                            from up in l_up.DefaultIfEmpty()
                            join upp in db.tblUserProfiles on l.modifiedByUserID equals upp.userID into l_upp
                            from upp in l_upp.DefaultIfEmpty()
                            where users.Contains(l.userID) && resorts.Contains(l.frontOfficeResortID) && leadSources.Contains(l.leadSourceID)
                            select new
                            {
                                l.user_leadSourceID,
                                username = u.firstName + " " + u.lastName,
                                l.frontOfficeResortID,
                                l.leadSourceID,
                                l.fromDate,
                                l.toDate,
                                savedBy = up.firstName + " " + up.lastName,
                                modifiedBy = upp.firstName + " " + upp.lastName
                            };

                foreach (var i in query)
                {
                    list.Add(new UsersLeadSources.UserLeadSources()
                    {
                        User_LeadSourceID = i.user_leadSourceID,
                        User = i.username,
                        LeadSource = listSources.Single(m => long.Parse(m.Value) == i.leadSourceID).Text,
                        Resort = listResorts.Single(m => long.Parse(m.Value) == i.frontOfficeResortID).Text,
                        FromDate = i.fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ToDate = i.toDate != null ? i.toDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                        SavedByUser = i.modifiedBy ?? i.savedBy
                    });
                }

                return list;
            }

            public AttemptResponse SaveUserLeadSource(UsersLeadSources.UserLeadSources model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                var terminal = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray().FirstOrDefault();
                var resorts = model.Resorts.Count() > 0 ? model.Resorts : model.ListResorts.Select(m => (int?)int.Parse(m.Value)).ToArray();
                var leadSources = model.LeadSources.Count() > 0 ? model.LeadSources : model.ListLeadSources.Select(m => (long?)long.Parse(m.Value)).ToArray();

                if (model.Users.Count() == 0)
                {
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.Message = "Please select at least one user";
                    response.ObjectID = null;
                    return response;
                }

                if (model.User_LeadSourceID != null && model.User_LeadSourceID != 0)
                {
                    var query = (from u in db.tblUsers_LeadSources
                                 where u.user_leadSourceID == model.User_LeadSourceID
                                 select u).FirstOrDefault();

                    query.userID = model.Users[0];
                    query.frontOfficeResortID = model.Resorts[0];
                    query.leadSourceID = (long)model.LeadSources[0];
                    query.fromDate = DateTime.Parse(model.FromDate);
                    query.toDate = model.ToDate != null && model.ToDate != "" ? DateTime.Parse(model.ToDate) : (DateTime?)null;
                    query.modifiedByUserID = session.UserID;
                    query.dateModified = DateTime.Now;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Rule Updated";
                    response.ObjectID = new { id = query.user_leadSourceID, user = session.User };
                    return response;
                }
                else
                {
                    var data = new List<UsersLeadSources.UserLeadSources>();
                    foreach (var user in model.Users)
                    {
                        var profile = model.ListUsers.Single(m => Guid.Parse(m.Value) == user).Text;
                        foreach (var resort in resorts)
                        {
                            var place = model.ListResorts.FirstOrDefault(m => long.Parse(m.Value) == resort).Text;
                            foreach (var source in leadSources)
                            {
                                var leadSource = model.ListLeadSources.FirstOrDefault(m => long.Parse(m.Value) == source).Text;
                                var query = new tblUsers_LeadSources();
                                query.terminalID = terminal;
                                query.userID = user;
                                query.frontOfficeResortID = resort;
                                query.leadSourceID = (long)source;
                                query.fromDate = DateTime.Parse(model.FromDate);
                                query.toDate = model.ToDate != null && model.ToDate != "" ? DateTime.Parse(model.ToDate) : (DateTime?)null;
                                query.savedByUserID = session.UserID;
                                db.tblUsers_LeadSources.AddObject(query);
                                db.SaveChanges();
                                data.Add(new UsersLeadSources.UserLeadSources()
                                {
                                    User_LeadSourceID = query.user_leadSourceID,
                                    User = profile,
                                    Resort = place,
                                    LeadSource = leadSource,
                                    FromDate = model.FromDate,
                                    ToDate = model.ToDate,
                                    SavedByUser = session.User
                                });
                            }
                        }
                    }

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Rule(s) Saved";
                    response.ObjectID = new JavaScriptSerializer().Serialize(data);
                    return response;
                }
            }

            public UsersLeadSources.UserLeadSources GetUserLeadSource(int id)
            {
                ePlatEntities db = new ePlatEntities();
                UsersLeadSources.UserLeadSources model = new UsersLeadSources.UserLeadSources();

                var query = db.tblUsers_LeadSources.Single(m => m.user_leadSourceID == id);

                model.User_LeadSourceID = id;
                model.Users = new Guid[] { query.userID };
                model.Resorts = new int?[] { query.frontOfficeResortID };
                model.LeadSources = new long?[] { query.leadSourceID };
                model.FromDate = query.fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                model.ToDate = query.toDate != null ? query.toDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                return model;
            }
        }

        public class PlaceClasifications
        {
            public class PlaceClasificationsCatalogs
            {

            }

            public List<PlaceClasificationsModel.PlaceClasificationsInfoModel> SearchPlaceClasifications(PlaceClasificationsModel.SearchPlaceClasificationsModel model)
            {
                ePlatEntities db = new ePlatEntities();
                List<PlaceClasificationsModel.PlaceClasificationsInfoModel> list = new List<PlaceClasificationsModel.PlaceClasificationsInfoModel>();

                var query = from pc in db.tblPlaceClasifications
                            where (pc.placeClasification.Contains(model.Search_PlaceClasifications) || model.Search_PlaceClasifications == null)
                            orderby pc.placeClasification, pc.hosting ascending
                            select pc;
                foreach (var i in query)
                {
                    list.Add(new PlaceClasificationsModel.PlaceClasificationsInfoModel()
                    {
                        PlaceClasificationInfo_PlaceClasificationID = i.placeClasificationID,
                        PlaceClasificationInfo_PlaceClasification = i.placeClasification,
                        PlaceClasificationInfo_Hosting = i.hosting,
                        PlaceClasificationInfo_PlaceType = i.placeTypeID,
                        PlaceClasificationInfo_PlaceTypeName = i.tblPlaceTypes.placeType
                    });
                }
                return list;
            }

            public AttemptResponse SavePlaceClasification(PlaceClasificationsModel.PlaceClasificationsInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.PlaceClasificationInfo_PlaceClasificationID != 0)
                {
                    try
                    {
                        var query = db.tblPlaceClasifications.Single(m => m.placeClasificationID == model.PlaceClasificationInfo_PlaceClasificationID);
                        query.placeClasification = model.PlaceClasificationInfo_PlaceClasification;
                        query.placeTypeID = model.PlaceClasificationInfo_PlaceType;
                        query.hosting = model.PlaceClasificationInfo_Hosting;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Place Clasification Updated";
                        response.ObjectID = query.placeClasificationID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Place Clasification NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblPlaceClasifications();
                        query.placeClasification = model.PlaceClasificationInfo_PlaceClasification;
                        query.placeTypeID = model.PlaceClasificationInfo_PlaceType;
                        query.hosting = model.PlaceClasificationInfo_Hosting;
                        db.tblPlaceClasifications.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Place Clasification Saved";
                        response.ObjectID = query.placeClasificationID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Place Clasification NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public PlaceClasificationsModel.PlaceClasificationsInfoModel GetPlaceClasification(int PlaceClasificationInfo_PlaceClasificationID)
            {
                ePlatEntities db = new ePlatEntities();
                PlaceClasificationsModel.PlaceClasificationsInfoModel model = new PlaceClasificationsModel.PlaceClasificationsInfoModel();

                var query = db.tblPlaceClasifications.Single(m => m.placeClasificationID == PlaceClasificationInfo_PlaceClasificationID);
                model.PlaceClasificationInfo_PlaceClasificationID = query.placeClasificationID;
                model.PlaceClasificationInfo_PlaceClasification = query.placeClasification;
                model.PlaceClasificationInfo_PlaceType = query.placeTypeID;
                model.PlaceClasificationInfo_Hosting = query.hosting;
                return model;
            }

            public AttemptResponse DeletePlaceClasification(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                try
                {
                    var query = db.tblPlaceClasifications.Single(m => m.placeClasificationID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Place Clasification Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Place Clasification NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class Providers
        {
            public class ProvidersCatalogs
            {
                public static List<SelectListItem> FillDrpProvidersPerDestinations(long? terminalID)
                {
                    ePlatEntities db = new ePlatEntities();
                    List<long?> terminals = new List<long?>();
                    if (terminalID != null)
                    {
                        terminals.Add(terminalID);
                    }
                    else
                    {
                        session.Terminals.Split(',').Select(m => long.Parse(m)).ToList();
                    }

                    //var destinations = db.tblTerminals_Destinations.Where(m => terminals.Contains(m.terminalID)).Select(m => m.destinationID).ToArray();

                    var list = new List<SelectListItem>();

                    var query = from t in db.tblProviders
                                where t.isActive
                                && terminals.Contains(t.terminalID)
                                select t;

                    foreach (var i in query.OrderBy(m => m.comercialName))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.providerID.ToString(),
                            Text = i.comercialName
                        });
                    }
                    return list;
                }

                public static List<SelectListItem> FillDrpProvidersPerTerminals(long? terminalID)
                {
                    ePlatEntities db = new ePlatEntities();
                    var terminals = terminalID != null ? new long?[] { terminalID } : session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();
                    var list = new List<SelectListItem>();

                    var query = db.tblProviders.Where(m => m.isActive && terminals.Contains(m.terminalID));

                    foreach (var i in query.OrderBy(m => m.comercialName))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.providerID.ToString(),
                            Text = i.comercialName
                        });
                    }
                    return list;
                }

                public static List<SelectListItem> FillDrpProviderTypes()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();

                    foreach (var i in db.tblProviderTypes)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.providerTypeID.ToString(),
                            Text = i.providerType
                        });
                    }
                    return list;
                }
            }

            public List<ProvidersModel.ProviderInfoModel> SearchProviders(ProvidersModel.SearchProvidersModel model)
            {
                ePlatEntities db = new ePlatEntities();
                List<ProvidersModel.ProviderInfoModel> list = new List<ProvidersModel.ProviderInfoModel>();

                var terminals = session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();
                var destinations = model.SearchProviders_Destinations != null ? model.SearchProviders_Destinations.Split(',').Select(m => int.Parse(m)).ToArray() : Destinations.DestinationsCatalogs.FillDrpDestinationsPerCurrentTerminals().Select(m => int.Parse(m.Value)).ToArray();

                var query = from provider in db.tblProviders
                            where ((provider.comercialName.Contains(model.SearchProviders_ComercialName) || provider.shortName.Contains(model.SearchProviders_ComercialName) || provider.taxesName.Contains(model.SearchProviders_ComercialName)) || model.SearchProviders_ComercialName == null)
                            && destinations.Contains((int)provider.destinationID)
                            && terminals.Contains(provider.terminalID)
                            //&& (destinations.Contains((int)provider.destinationID) || model.SearchProviders_Destinations == null)
                            select provider;

                foreach (var i in query)
                {
                    list.Add(new ProvidersModel.ProviderInfoModel()
                    {
                        ProviderInfo_ProviderID = i.providerID,
                        ProviderInfo_ComercialName = i.comercialName,
                        ProviderInfo_ContractCurrency = i.contractCurrencyID != null ? i.tblCurrencies.currency : "US Dollar & Mexican Peso",
                        ProviderInfo_TaxesName = i.taxesName,
                        ProviderInfo_RFC = i.rfc,
                        ProviderInfo_Terminal = i.tblTerminals.terminal,
                        ProviderInfo_IsActive = i.isActive,
                    });
                }
                return list;
            }

            public AttemptResponse SaveProvider(ProvidersModel.ProviderInfoModel model, ControllerContext context)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.ProviderInfo_ProviderID != 0)
                {
                    #region "update"
                    try
                    {
                        var query = db.tblProviders.Single(m => m.providerID == model.ProviderInfo_ProviderID);
                        query.destinationID = model.ProviderInfo_Destination;
                        query.comercialName = model.ProviderInfo_ComercialName;
                        query.shortName = model.ProviderInfo_ShortName;
                        query.rfc = model.ProviderInfo_RFC;
                        query.taxesName = model.ProviderInfo_TaxesName;
                        query.phone1 = model.ProviderInfo_Phone1;
                        query.ext1 = model.ProviderInfo_Ext1;
                        query.phone2 = model.ProviderInfo_Phone2;
                        query.ext2 = model.ProviderInfo_Ext2;
                        query.contactEmail = model.ProviderInfo_ContactEmail;
                        query.contactName = model.ProviderInfo_ContactName;
                        query.providerTypeID = model.ProviderInfo_ProviderType;
                        query.terminalID = long.Parse(model.ProviderInfo_Terminal);
                        query.isActive = model.ProviderInfo_IsActive;
                        query.forCompanyID = model.ProviderInfo_ForCompany != 0 ? model.ProviderInfo_ForCompany : (int?)null;
                        if (model.ProviderInfo_ContractCurrency != null)
                        {
                            var _contractCurrency = model.ProviderInfo_ContractCurrency != "null" ? int.Parse(model.ProviderInfo_ContractCurrency) : (int?)null;
                            if (query.contractCurrencyID != _contractCurrency)
                            {
                                var contractCurrencyHistory = new tblContractsCurrencyHistory();
                                contractCurrencyHistory.providerID = query.providerID;
                                contractCurrencyHistory.contractCurrencyID = _contractCurrency;
                                contractCurrencyHistory.dateSaved = DateTime.Now;
                                contractCurrencyHistory.savedByUserID = session.UserID;
                                db.tblContractsCurrencyHistory.AddObject(contractCurrencyHistory);
                            }
                            query.contractCurrencyID = _contractCurrency;
                        }
                        query.avanceProviderID = model.ProviderInfo_AvanceProvider;
                        query.mxnAvanceProviderID = model.ProviderInfo_MXNAvanceProvider;
                        query.invoiceCurrencyID = model.ProviderInfo_InvoiceCurrency != "0" ? int.Parse(model.ProviderInfo_InvoiceCurrency) : (int?)null;
                        query.modifiedByUserID = session.UserID;
                        query.lastDateModified = DateTime.Now;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Provider Updated";
                        response.ObjectID = query.providerID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Provider NOT Updated";
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
                        var query = new tblProviders();
                        query.destinationID = model.ProviderInfo_Destination;
                        query.comercialName = model.ProviderInfo_ComercialName;
                        query.shortName = model.ProviderInfo_ShortName;
                        query.rfc = model.ProviderInfo_RFC;
                        query.taxesName = model.ProviderInfo_TaxesName;
                        query.phone1 = model.ProviderInfo_Phone1;
                        query.ext1 = model.ProviderInfo_Ext1;
                        query.phone2 = model.ProviderInfo_Phone2;
                        query.ext2 = model.ProviderInfo_Ext2;
                        query.contactEmail = model.ProviderInfo_ContactEmail;
                        query.contactName = model.ProviderInfo_ContactName;
                        query.providerTypeID = model.ProviderInfo_ProviderType;
                        query.terminalID = long.Parse(model.ProviderInfo_Terminal);
                        query.isActive = model.ProviderInfo_IsActive;
                        query.forCompanyID = model.ProviderInfo_ForCompany != 0 ? model.ProviderInfo_ForCompany : (int?)null;
                        if (model.ProviderInfo_ContractCurrency != null)
                        {
                            query.contractCurrencyID = model.ProviderInfo_ContractCurrency != "null" ? int.Parse(model.ProviderInfo_ContractCurrency) : (int?)null;
                            var contractCurrencyHistory = new tblContractsCurrencyHistory();
                            contractCurrencyHistory.contractCurrencyID = model.ProviderInfo_ContractCurrency != "null" ? int.Parse(model.ProviderInfo_ContractCurrency) : (int?)null;
                            contractCurrencyHistory.dateSaved = DateTime.Now;
                            contractCurrencyHistory.savedByUserID = session.UserID;
                            query.tblContractsCurrencyHistory.Add(contractCurrencyHistory);
                        }

                        query.avanceProviderID = model.ProviderInfo_AvanceProvider;
                        query.mxnAvanceProviderID = model.ProviderInfo_MXNAvanceProvider;
                        query.invoiceCurrencyID = model.ProviderInfo_InvoiceCurrency != "0" ? int.Parse(model.ProviderInfo_InvoiceCurrency) : (int?)null;
                        query.savedByUserID = session.UserID;
                        query.dateSaved = DateTime.Now;
                        db.tblProviders.AddObject(query);
                        db.SaveChanges();

                        if (model.ProviderInfo_FileToUpload != null)
                        {
                            try
                            {
                                var upload = (PictureDataModel.FineUpload)new PictureDataModel.FineUpload.ModelBinder().BindModel(context, new ModelBindingContext());
                                upload.Filename = model.ProviderInfo_FileToUpload;
                                UploadFile(upload, "p_" + query.providerID, true);
                            }
                            catch (Exception ex)
                            {
                                db.DeleteObject(query);
                                db.SaveChanges();
                                response.Type = Attempt_ResponseTypes.Error;
                                response.Message = "Provider NOT Saved";
                                response.ObjectID = 0;
                                response.Exception = ex;
                                return response;
                            }
                        }
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Provider Saved";
                        response.ObjectID = query.providerID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Provider NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
            }

            public ProvidersModel.ProviderInfoModel GetProvider(int ProviderInfo_ProviderID)
            {
                ePlatEntities db = new ePlatEntities();
                ProvidersModel.ProviderInfoModel model = new ProvidersModel.ProviderInfoModel();

                var query = db.tblProviders.Single(m => m.providerID == ProviderInfo_ProviderID);
                model.ProviderInfo_ProviderID = query.providerID;
                model.ProviderInfo_Terminal = query.terminalID.ToString();
                model.ProviderInfo_Destination = (long)query.destinationID;
                model.ProviderInfo_DestinationText = query.tblDestinations.destination;
                model.ProviderInfo_ComercialName = query.comercialName;
                model.ProviderInfo_ShortName = query.shortName;
                model.ProviderInfo_RFC = query.rfc;
                model.ProviderInfo_TaxesName = query.taxesName;
                model.ProviderInfo_Phone1 = query.phone1;
                model.ProviderInfo_Phone2 = query.phone2;
                model.ProviderInfo_Ext1 = query.ext1;
                model.ProviderInfo_Ext2 = query.ext2;
                model.ProviderInfo_ContactEmail = query.contactEmail;
                model.ProviderInfo_ContactName = query.contactName;
                model.ProviderInfo_ProviderType = query.providerTypeID;
                model.ProviderInfo_ProviderTypeText = query.tblProviderTypes.providerType;
                model.ProviderInfo_IsActive = query.isActive;
                model.ProviderInfo_ForCompany = query.forCompanyID != null ? (int)query.forCompanyID : 0;
                model.ProviderInfo_ContractCurrency = query.contractCurrencyID != null ? query.contractCurrencyID.ToString() : "null";
                model.ProviderInfo_AvanceProvider = query.avanceProviderID;
                model.ProviderInfo_MXNAvanceProvider = query.mxnAvanceProviderID;
                model.ProviderInfo_InvoiceCurrency = query.invoiceCurrencyID != null ? query.invoiceCurrencyID.ToString() : "0";
                return model;
            }

            public AttemptResponse DeleteProvider(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblProviders.Single(m => m.providerID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Provider Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Provider NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }

            public List<string> GetFilesOfProvider(int providerID)
            {
                ePlatEntities db = new ePlatEntities();
                var firstPath = HttpContext.Current.Server.MapPath("~/");
                var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                var finalPath = secondPath + "ePlatBack\\Content\\files\\contracts\\";
                var _files = new List<string>();
                string[] files = Directory.Exists(@finalPath + "p_" + providerID) ? Directory.GetFiles(@finalPath + "p_" + providerID) : new string[] { };

                foreach (var i in files)
                {
                    _files.Add(i.Substring(i.LastIndexOf("ePlatBack") + 9));
                }
                return _files;
            }

            public PictureDataModel.FineUploaderResult UploadFile(PictureDataModel.FineUpload upload, string path, bool newProvider)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    if (upload.Exception == null)
                    {
                        var firstPath = HttpContext.Current.Server.MapPath("~/");
                        var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                        var finalPath = secondPath + "ePlatBack\\Content\\files\\contracts\\";
                        var fileName = upload.Filename;

                        var fileNameDecoded = HttpContext.Current.Server.UrlDecode(fileName);
                        fileName = HttpUtility.UrlEncode(fileNameDecoded, Encoding.GetEncoding("iso-8859-8"));
                        fileName = fileName.Replace("+", "");

                        for (var i = 0; i < Regex.Matches(fileName, "%").Count; i++)
                        {
                            var encoded = fileName.Substring(fileName.IndexOf("%"), ((fileName.IndexOf("%") + 3) - fileName.IndexOf("%")));
                            var newFileName = fileName.Replace(encoded, "_");
                            fileName = newFileName;
                        }

                        var filePath = finalPath + path + "\\" + fileName;
                        var _filePath = "/content/files/" + path + "/" + fileName;
                        upload.SaveAs(filePath, false);
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.ObjectID = path.Split('_')[1];
                        response.Message = "File Uploaded";
                        return new PictureDataModel.FineUploaderResult(true, new { response = response }, new { path = _filePath });
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = upload.Exception ?? ex;
                    response.Message = "File NOT Uploaded";
                    return new PictureDataModel.FineUploaderResult(false, new { response = response });
                }
            }

            public AttemptResponse DeleteFileOfProvider(string file)
            {
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var firstPath = HttpContext.Current.Server.MapPath("~/");
                    var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                    var finalPath = secondPath + "ePlatBack\\Content\\files\\contracts\\";

                    var providerID = int.Parse(file.Split('\\')[0]);
                    if (File.Exists(@finalPath + "p_" + file))
                    {
                        File.Delete(@finalPath + "p_" + file);
                        response.Message = "File Deleted";
                    }
                    else
                    {
                        response.Message = "File Not Found";
                    }
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = file;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "File NOT Deleted";
                    response.Exception = ex;
                    response.ObjectID = 0;
                    return response;
                }
            }
        }

        public class OPCS
        {
            public List<OPCSModel.OPCSInfoModel> GetOPCSList(string terminals)
            {
                List<OPCSModel.OPCSInfoModel> list = new List<OPCSModel.OPCSInfoModel>();
                ePlatEntities db = new ePlatEntities();
                long[] currentTerminals = (terminals ?? "").Split(',').Select<string, long>(long.Parse).ToArray();

                var companies = db.tblTerminals_Companies.Where(x => currentTerminals.Contains(x.terminalID)).Select(x => x.companyID).ToArray();
                var query = from opc in db.tblOPCS
                            where companies.Contains((int)opc.companyID)
                            select opc;

                foreach (var i in query)
                {
                    list.Add(new OPCSModel.OPCSInfoModel()
                    {
                        OPCSInfo_OpcID = i.opcID,
                        OPCSInfo_Opc = i.opc,
                        OPCSInfo_FirstName = i.firstName,
                        OPCSInfo_MiddleName = i.middleName,
                        OPCSInfo_LastName = i.lastName,
                        OPCSInfo_SecondSurname = i.secondSurname,
                        OPCSInfo_Credential = i.credential,
                        OPCSInfo_Phone1 = i.phone1,
                        OPCSInfo_Phone2 = i.phone2,
                        OPCSInfo_EnlistDate = i.enlistDate,
                        OPCSInfo_PayingCompany = i.payingCompanyIDX != null ? (int)i.payingCompanyIDX : 0,
                        OPCSInfo_PayingCompanyText = i.payingCompanyIDX != null ? i.tblCompanies1.company : "",
                        OPCSInfo_LegacyKey = i.legacyKey,
                        OPCSInfo_AvanceID = i.avanceID,
                        OPCSInfo_User = i.userID != null ? ((Guid)i.userID).ToString() : "",
                        OPCSInfo_Company = i.companyID != null ? (int)i.companyID : 0,
                        OPCSInfo_CompanyText = i.companyID != null ? i.tblCompanies.company : ""
                    });
                }
                return list;

            }

            public List<OPCSModel.OPCSInfoModel> SearchOPCS(OPCSModel.SearchOPCSModel model)
            {
                List<OPCSModel.OPCSInfoModel> list = new List<OPCSModel.OPCSInfoModel>();
                ePlatEntities db = new ePlatEntities();
                long[] currentTerminals;

                if (model.SearchOPCS_Terminals != null && model.SearchOPCS_Terminals.Length > 0)
                {
                    currentTerminals = model.SearchOPCS_Terminals;
                }
                else
                {
                    currentTerminals = (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray();
                }

                var companies = model.SearchOPCS_Companies != null ? model.SearchOPCS_Companies.Split(',').Select(m => int.Parse(m)).ToArray() : db.tblTerminals_Companies.Where(x => currentTerminals.Contains(x.terminalID)).Select(x => x.companyID).ToArray();

                var query = (from opc in db.tblOPC_PromotionTeams
                             where (opc.tblOPCS.opc.Contains(model.SearchOPCS_OPC)
                             || model.SearchOPCS_OPC == null)
                             && companies.Contains((int)opc.tblOPCS.companyID)
                             && (opc.tblOPCS.legacyKey == model.SearchOPCS_LegacyKey || model.SearchOPCS_LegacyKey == null)
                             && (opc.tblOPCS.avanceID == model.SearchOPCS_AvanceID || model.SearchOPCS_AvanceID == null)
                             select opc).ToList();

                //filtrar sólo opcs activos si no se busca por nombre
                if ((model.SearchOPCS_OPC == null || model.SearchOPCS_OPC == "") && (model.SearchOPCS_LegacyKey == null || model.SearchOPCS_LegacyKey == ""))
                {
                    query = query.Where(x => x.deleted != true).ToList();
                }
                else
                {
                    //se busca por nombre o por legacy key, ocultar equipos inactivos
                    List<tblOPC_PromotionTeams> newQuery = new List<tblOPC_PromotionTeams>();
                    foreach (var opc in query)
                    {
                        int actives = opc.tblOPCS.tblOPC_PromotionTeams.Count(x => x.deleted != true);
                        if (actives == 0)
                        {
                            newQuery.Add(opc);
                        }
                        else
                        {
                            if (opc.deleted != true)
                            {
                                newQuery.Add(opc);
                            }
                        }
                    }
                    query = newQuery;
                }

                //filtrar promotion team
                if (model.SearchOPCS_PromotionTeam != null)
                {
                    query = query.Where(x => model.SearchOPCS_PromotionTeam.Contains(x.promotionTeamID)).ToList();
                }

                //filtrar job position
                if (model.Search_OPCS_JobPosition != null)
                {
                    query = query.Where(x => model.Search_OPCS_JobPosition.Contains(x.jobPositionID)).ToList();
                }

                //filtrar paying company
                if (model.SearchOPCS_PayingCompany != null)
                {
                    query = query.Where(x => model.SearchOPCS_PayingCompany.Contains(x.tblOPCS.payingCompanyIDX)).ToList();
                }

                foreach (var i in query.OrderBy(x => x.promotionTeamID))
                {
                    int actives = i.tblOPCS.tblOPC_PromotionTeams.Count(x => x.deleted != true);
                    list.Add(new OPCSModel.OPCSInfoModel()
                    {
                        OPCSInfo_OpcID = i.opcID,
                        OPCSInfo_Opc = i.tblOPCS.opc,
                        OPCSInfo_FirstName = i.tblOPCS.firstName,
                        OPCSInfo_MiddleName = i.tblOPCS.middleName,
                        OPCSInfo_LastName = i.tblOPCS.lastName,
                        OPCSInfo_SecondSurname = i.tblOPCS.secondSurname,
                        OPCSInfo_Credential = i.tblOPCS.credential,
                        OPCSInfo_Phone1 = i.tblOPCS.phone1,
                        OPCSInfo_Phone2 = i.tblOPCS.phone2,
                        OPCSInfo_EnlistDate = i.enlistDate,
                        OPCSInfo_PayingCompany = i.tblOPCS.payingCompanyIDX != null ? (int)i.tblOPCS.payingCompanyIDX : 0,
                        OPCSInfo_PayingCompanyText = i.tblOPCS.payingCompanyIDX != null ? i.tblOPCS.tblCompanies1.company : "",
                        OPCSInfo_LegacyKey = i.tblOPCS.legacyKey,
                        OPCSInfo_AvanceID = i.tblOPCS.avanceID,
                        OPCSInfo_User = i.tblOPCS.userID != null ? ((Guid)i.tblOPCS.userID).ToString() : "",
                        OPCSInfo_Company = i.tblOPCS.companyID != null ? (int)i.tblOPCS.companyID : 0,
                        OPCSInfo_CompanyText = i.tblOPCS.companyID != null ? i.tblOPCS.tblCompanies.company : "",
                        OPCSInfo_PromotionTeamText = i.tblPromotionTeams.promotionTeam,
                        OPCSInfo_JobPositionText = i.tblJobPositions.jobPosition,
                        OPCSInfo_Destination = i.tblPromotionTeams.tblDestinations.destination,
                        OPCSInfo_Active = (actives > 0 ? "Yes" : "No")
                    });
                }
                return list;
            }

            public AttemptResponse SaveOPC(OPCSModel.OPCSInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.OPCSInfo_OpcID != 0)
                {
                    //try
                    //{
                    //editing OPC
                    List<ChangesTracking.ChangeItem> changes = new List<ChangesTracking.ChangeItem>();
                    var query = db.tblOPCS.Single(m => m.opcID == model.OPCSInfo_OpcID);
                    query.opc = (model.OPCSInfo_FirstName + " " + model.OPCSInfo_MiddleName + " " + model.OPCSInfo_LastName + " " + model.OPCSInfo_SecondSurname).Replace("  ", " ");

                    /*TEAMS*/
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<OPCSModel.OPCTeamInfoModel> Teams = js.Deserialize<List<OPCSModel.OPCTeamInfoModel>>(model.OPCSInfo_TeamsStr);
                    foreach (OPCSModel.OPCTeamInfoModel team in Teams)
                    {
                        if (team.OPCPromotionTeamID == 0)
                        {
                            //new
                            changes.Add(Utils.ChangesTracking.setChangeLog(10619, "OPC", "", model.OPCSInfo_FirstName + " " + model.OPCSInfo_MiddleName + " " + model.OPCSInfo_LastName + " " + model.OPCSInfo_SecondSurname, query.opc + " - se unió al equipo " + team.OPCTeamInfo_PromotionTeamText + " como " + team.OPCTeamInfo_JobPositionText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));

                            tblOPC_PromotionTeams newTeam = new tblOPC_PromotionTeams();
                            newTeam.promotionTeamID = team.OPCTeamInfo_PromotionTeam;
                            newTeam.jobPositionID = team.OPCTeamInfo_JobPosition;
                            if (team.OPCTeamInfo_ParentOpc != null && team.OPCTeamInfo_ParentOpc > 0)
                            {
                                newTeam.parentOpcID = team.OPCTeamInfo_ParentOpc;
                            }
                            newTeam.dateSaved = DateTime.Now;
                            if (team.OPCTeamInfo_EnlistDate != null && team.OPCTeamInfo_EnlistDate != "")
                            {
                                newTeam.enlistDate = DateTime.Parse(team.OPCTeamInfo_EnlistDate);
                            }
                            if (team.OPCTeamInfo_Deleted == true)
                            {
                                if (team.OPCTeamInfo_TerminateDate != null && team.OPCTeamInfo_TerminateDate != "")
                                {
                                    newTeam.terminateDate = DateTime.Parse(team.OPCTeamInfo_TerminateDate);
                                }
                                else
                                {
                                    newTeam.terminateDate = null;
                                }
                            }
                            else
                            {
                                newTeam.terminateDate = null;
                            }
                            newTeam.terminateReason = team.OPCTeamInfo_TerminateReason;
                            newTeam.deleted = team.OPCTeamInfo_Deleted;
                            if (team.OPCTeamInfo_AssignToParentOpc != null && team.OPCTeamInfo_AssignToParentOpc > 0)
                            {
                                newTeam.terminateAssignSubsToOpcID = team.OPCTeamInfo_AssignToParentOpc;
                            }

                            query.tblOPC_PromotionTeams.Add(newTeam);
                        }
                        else
                        {
                            //edit
                            foreach (tblOPC_PromotionTeams teamdb in query.tblOPC_PromotionTeams)
                            {
                                if (teamdb.opc_promotionTeamID == team.OPCPromotionTeamID)
                                {
                                    if (teamdb.promotionTeamID != team.OPCTeamInfo_PromotionTeam)
                                    {
                                        teamdb.promotionTeamID = team.OPCTeamInfo_PromotionTeam;
                                        changes.Add(Utils.ChangesTracking.setChangeLog(10654, "Team", teamdb.tblPromotionTeams.promotionTeam, team.OPCTeamInfo_PromotionTeamText, query.opc + " - actualización de Equipo " + team.OPCTeamInfo_PromotionTeamText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                    }
                                    if (team.OPCTeamInfo_ParentOpc == -1)
                                    {
                                        team.OPCTeamInfo_ParentOpc = null;
                                    }
                                    if (teamdb.parentOpcID != team.OPCTeamInfo_ParentOpc)
                                    {
                                        changes.Add(Utils.ChangesTracking.setChangeLog(10654, "Parent OPC", (teamdb.parentOpcID != null ? db.tblOPCS.SingleOrDefault(x => x.opcID == teamdb.parentOpcID).opc : ""), db.tblOPCS.SingleOrDefault(x => x.opcID == team.OPCTeamInfo_ParentOpc).opc, query.opc + " - actualización de Supervisor en el Equipo " + team.OPCTeamInfo_PromotionTeamText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                        teamdb.parentOpcID = team.OPCTeamInfo_ParentOpc;
                                    }


                                    if (teamdb.jobPositionID != team.OPCTeamInfo_JobPosition)
                                    {
                                        changes.Add(Utils.ChangesTracking.setChangeLog(10655, "Job Position", teamdb.tblJobPositions.jobPosition, db.tblJobPositions.SingleOrDefault(x => x.jobPositionID == team.OPCTeamInfo_JobPosition).jobPosition, query.opc + " - actualización de puesto en el Equipo " + team.OPCTeamInfo_PromotionTeamText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                        teamdb.jobPositionID = team.OPCTeamInfo_JobPosition;
                                    }

                                    if (team.OPCTeamInfo_EnlistDate != null && team.OPCTeamInfo_EnlistDate != "")
                                    {
                                        if (teamdb.enlistDate != DateTime.Parse(team.OPCTeamInfo_EnlistDate))
                                        {
                                            changes.Add(Utils.ChangesTracking.setChangeLog(10656, "Enlist Date", teamdb.enlistDate != null ? teamdb.enlistDate.Value.ToString("yyyy-MM-ss") : "", team.OPCTeamInfo_EnlistDate != null ? DateTime.Parse(team.OPCTeamInfo_EnlistDate).ToString("yyyy-MM-ss") : "", query.opc + " - actualización de Fecha de Alta en Equipo " + team.OPCTeamInfo_PromotionTeamText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                            teamdb.enlistDate = DateTime.Parse(team.OPCTeamInfo_EnlistDate);
                                        }

                                    }
                                    if (team.OPCTeamInfo_Deleted == true)
                                    {
                                        if (team.OPCTeamInfo_TerminateDate != null && team.OPCTeamInfo_TerminateDate != "")
                                        {
                                            if (teamdb.terminateDate != DateTime.Parse(team.OPCTeamInfo_TerminateDate))
                                            {
                                                changes.Add(Utils.ChangesTracking.setChangeLog(10658, "Terminate Date", teamdb.terminateDate != null ? teamdb.terminateDate.Value.ToString("yyyy-MM-ss") : "", team.OPCTeamInfo_TerminateDate != null ? DateTime.Parse(team.OPCTeamInfo_TerminateDate).ToString("yyyy-MM-ss") : "", query.opc + " - actualización de Fecha de Baja de Equipo " + team.OPCTeamInfo_PromotionTeamText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                                teamdb.terminateDate = DateTime.Parse(team.OPCTeamInfo_TerminateDate);
                                            }
                                        }
                                        else
                                        {
                                            if (teamdb.terminateDate != null)
                                            {
                                                changes.Add(Utils.ChangesTracking.setChangeLog(10658, "Terminate Date", teamdb.terminateDate != null ? teamdb.terminateDate.Value.ToString("yyyy-MM-ss") : "", "", query.opc + " - eliminación de Fecha de Baja de Equipo " + team.OPCTeamInfo_PromotionTeamText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                                teamdb.terminateDate = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        teamdb.terminateDate = null;
                                    }

                                    if (teamdb.deleted != team.OPCTeamInfo_Deleted)
                                    {
                                        if (team.OPCTeamInfo_Deleted == false)
                                        {
                                            changes.Add(Utils.ChangesTracking.setChangeLog(10657, "Terminate", "", "", query.opc + " fue reactivado en el Equipo " + team.OPCTeamInfo_PromotionTeamText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                        }
                                        else
                                        {
                                            changes.Add(Utils.ChangesTracking.setChangeLog(10657, "Terminate", "", "", query.opc + " fue dado de Baja del Equipo " + team.OPCTeamInfo_PromotionTeamText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                        }
                                        teamdb.deleted = team.OPCTeamInfo_Deleted;
                                    }

                                    if ((teamdb.terminateReason == null ? "" : teamdb.terminateReason) != (team.OPCTeamInfo_TerminateReason == null ? "" : team.OPCTeamInfo_TerminateReason))
                                    {
                                        changes.Add(Utils.ChangesTracking.setChangeLog(10659, "Terminate Reason", (teamdb.terminateReason == null ? "" : teamdb.terminateReason), (team.OPCTeamInfo_TerminateReason == null ? "" : team.OPCTeamInfo_TerminateReason), query.opc + " - razón de la baja", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                        teamdb.terminateReason = team.OPCTeamInfo_TerminateReason;
                                    }

                                    if (team.OPCTeamInfo_AssignToParentOpc == -1)
                                    {
                                        team.OPCTeamInfo_AssignToParentOpc = null;
                                    }
                                    if (teamdb.terminateAssignSubsToOpcID != team.OPCTeamInfo_AssignToParentOpc)
                                    {
                                        changes.Add(Utils.ChangesTracking.setChangeLog(10886, "Assign Subs to ", (teamdb.terminateAssignSubsToOpcID != null ? db.tblOPCS.SingleOrDefault(x => x.opcID == teamdb.terminateAssignSubsToOpcID).opc : ""), db.tblOPCS.SingleOrDefault(x => x.opcID == team.OPCTeamInfo_AssignToParentOpc).opc, query.opc + " - cambio de asignación de Supervisor en el Equipo " + team.OPCTeamInfo_PromotionTeamText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                                        teamdb.terminateAssignSubsToOpcID = team.OPCTeamInfo_AssignToParentOpc;
                                        //modificar parentopc en los subs
                                        var Subs = from s in db.tblOPC_PromotionTeams
                                                   where s.promotionTeamID == team.OPCTeamInfo_PromotionTeam
                                                   && s.parentOpcID == query.opcID
                                                   select s;

                                        foreach (var s in Subs)
                                        {
                                            s.parentOpcID = team.OPCTeamInfo_AssignToParentOpc;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //buscar equipos en db que no vengan en el modelo para eliminarlas
                    List<long> opc_PromotionTeamIDs = new List<long>();
                    foreach (var teamdb in query.tblOPC_PromotionTeams)
                    {
                        if (Teams.Count(t => t.OPCPromotionTeamID == teamdb.opc_promotionTeamID) == 0)
                        {
                            //revisar que no se haya utilizado
                            if (teamdb.tblPromotionTeams.tblPaymentDetails.Count(c => (c.deleted == null || c.deleted == false)) == 0)
                            {
                                //agregar a cola de eliminación
                                opc_PromotionTeamIDs.Add(teamdb.opc_promotionTeamID);
                            }
                            else
                            {
                                //utilizado

                            }
                        }
                    }
                    //eliminar equipos
                    foreach (long otid in opc_PromotionTeamIDs)
                    {
                        db.DeleteObject(db.tblOPC_PromotionTeams.FirstOrDefault(x => x.opc_promotionTeamID == otid));
                        db.SaveChanges();
                    }

                    //revisar si ya no está en ningún equipo para reportar la baja definitiva
                    int activeTeams = query.tblOPC_PromotionTeams.Where(x => x.deleted != true).Count();
                    if (activeTeams == 0)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10619, "OPC", "", "", query.opc + " fue dado de Baja de la Lista de Promotores", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                    }

                    if (query.firstName != model.OPCSInfo_FirstName)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10624, "First Name", query.firstName, model.OPCSInfo_FirstName, query.opc + " - actualización del Primer Nombre", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.firstName = model.OPCSInfo_FirstName;
                    }

                    if (query.middleName != model.OPCSInfo_MiddleName)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10625, "Middle Name", query.middleName != null ? query.middleName : "", model.OPCSInfo_MiddleName != null ? model.OPCSInfo_MiddleName : "", query.opc + " - actualización del Segundo Nombre", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.middleName = model.OPCSInfo_MiddleName;
                    }

                    if (query.lastName != model.OPCSInfo_LastName)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10626, "Last Name", query.lastName, model.OPCSInfo_LastName, query.opc + " - actualización del Apellido Paterno", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.lastName = model.OPCSInfo_LastName;
                    }

                    if (query.secondSurname != model.OPCSInfo_SecondSurname)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10627, "Second Surname", query.secondSurname != null ? query.secondSurname : "", model.OPCSInfo_SecondSurname != null ? model.OPCSInfo_SecondSurname : "", query.opc + " - actualización del Apellido Materno", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.secondSurname = model.OPCSInfo_SecondSurname;
                    }

                    if (query.credential != model.OPCSInfo_Credential)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10622, "Credential", query.credential, model.OPCSInfo_Credential, query.opc + " - actualización de Credencial", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.credential = model.OPCSInfo_Credential;
                    }

                    if (query.phone1 != model.OPCSInfo_Phone1)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10628, "Main Phone Number", query.phone1 != null ? query.phone1 : "", model.OPCSInfo_Phone1 != null ? model.OPCSInfo_Phone1 : "", query.opc + " - actualización de Teléfono Principal", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.phone1 = model.OPCSInfo_Phone1;
                    }

                    if (query.phone2 != model.OPCSInfo_Phone2)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10629, "Alternate Phone Number", query.phone2 != null ? query.phone2 : "", model.OPCSInfo_Phone2 != null ? model.OPCSInfo_Phone2 : "", query.opc + " - actualización de Teléfono Alternativo", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.phone2 = model.OPCSInfo_Phone2;
                    }

                    if (query.enlistDate != model.OPCSInfo_EnlistDate)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10635, "Enlist Date", query.enlistDate != null ? DateTime.Parse(query.enlistDate.ToString()).ToString("yyyy-MM-ss") : "", model.OPCSInfo_EnlistDate != null ? DateTime.Parse(model.OPCSInfo_EnlistDate.ToString()).ToString("yyyy-MM-ss") : "", query.opc + " - actualización de Fecha de Alta", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.enlistDate = model.OPCSInfo_EnlistDate;
                    }

                    //if (model.OPCSInfo_PayingCompany != 0)
                    //{
                    //    if (query.payingCompanyID != model.OPCSInfo_PayingCompany)
                    //    {
                    //        changes.Add(Utils.ChangesTracking.setChangeLog(10630, "Paying Company", query.tblPayingCompanies.payingCompany, db.tblPayingCompanies.SingleOrDefault(x => x.payingCompanyID == model.OPCSInfo_PayingCompany).payingCompany, query.opc + " - actualización de Pagadora", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                    //        query.payingCompanyID = model.OPCSInfo_PayingCompany;
                    //    }
                    //}
                    //else
                    //{
                    //    if (query.payingCompanyID != null)
                    //    {
                    //        changes.Add(Utils.ChangesTracking.setChangeLog(10630, "Paying Company", query.tblPayingCompanies.payingCompany, "", query.opc + " - actualización de Pagadora", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                    //        query.payingCompanyID = null;
                    //    }
                    //}
                    if (model.OPCSInfo_PayingCompany != 0)
                    {
                        if (query.payingCompanyIDX != model.OPCSInfo_PayingCompany)
                        {
                            changes.Add(Utils.ChangesTracking.setChangeLog(11147, "Paying Company", (query.tblCompanies1 != null ? query.tblCompanies1.company : ""), db.tblCompanies.SingleOrDefault(x => x.companyID == model.OPCSInfo_PayingCompany).company, query.opc + " - actualización de Pagadora", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                            query.payingCompanyIDX = model.OPCSInfo_PayingCompany;
                        }
                    }
                    else
                    {
                        if (query.payingCompanyIDX != null)
                        {
                            changes.Add(Utils.ChangesTracking.setChangeLog(11147, "Paying Company", (query.tblCompanies1 != null ? query.tblCompanies1.company : ""), "", query.opc + " - actualización de Pagadora", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                            query.payingCompanyIDX = null;
                        }
                    }

                    if (query.legacyKey != model.OPCSInfo_LegacyKey)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10633, "Legacy Key", query.legacyKey, model.OPCSInfo_LegacyKey, query.opc + " - actualización de Legacy Key", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.legacyKey = model.OPCSInfo_LegacyKey;
                    }

                    if (query.avanceID != model.OPCSInfo_AvanceID)
                    {
                        changes.Add(Utils.ChangesTracking.setChangeLog(10633, "Avance ID", (query.avanceID != null ? query.avanceID : ""), (model.OPCSInfo_AvanceID != null ? model.OPCSInfo_AvanceID : ""), query.opc + " - actualización de Avance ID", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                        query.avanceID = model.OPCSInfo_AvanceID;
                    }

                    //if (model.OPCSInfo_User != "0")
                    if (model.OPCSInfo_User != null && model.OPCSInfo_User != "")
                    {
                        query.userID = Guid.Parse(model.OPCSInfo_User);
                    }
                    else
                    {
                        query.userID = null;
                    }

                    if (model.OPCSInfo_Company != 0)
                    {
                        if (query.companyID != model.OPCSInfo_Company)
                        {
                            changes.Add(Utils.ChangesTracking.setChangeLog(10631, "Company", query.tblCompanies.company, db.tblCompanies.SingleOrDefault(x => x.companyID == model.OPCSInfo_Company).company, query.opc + " - cambio de compañía", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                            query.companyID = model.OPCSInfo_Company;
                        }
                    }
                    else
                    {
                        query.companyID = null;
                    }

                    db.SaveChanges();

                    ChangesTracking.LogChanges(changes);

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "OPC Updated";
                    response.ObjectID = query.opcID;
                    return response;
                    //}
                    //catch (Exception ex)
                    //{
                    //    response.Type = Attempt_ResponseTypes.Error;
                    //    response.Message = "OPC NOT Updated";
                    //    response.ObjectID = 0;
                    //    response.Exception = ex;
                    //    return response;
                    //}
                }
                else
                {
                    //try
                    //{
                    //saving new OPC
                    bool valid = true;
                    if (model.OPCSInfo_LegacyKey != "" && model.OPCSInfo_LegacyKey != null)
                    {
                        //buscar si existe alguien con el mismo legacy key
                        var legacyCounter = db.tblOPCS.Where(x => x.legacyKey.Contains(model.OPCSInfo_LegacyKey) && x.companyID == model.OPCSInfo_Company).Count();
                        if (legacyCounter > 0)
                        {
                            valid = false;
                            response.Message = "There is an OPC with the same Legacy Key in the Database";
                        }
                    }

                    if (model.OPCSInfo_AvanceID != "" && model.OPCSInfo_AvanceID != null && model.OPCSInfo_AvanceID != "0")
                    {
                        //buscar si existe alguien con el mismo legacy key
                        var avanceCounter = db.tblOPCS.Where(x => x.avanceID.Contains(model.OPCSInfo_AvanceID) && x.companyID == model.OPCSInfo_Company).Count();
                        if (avanceCounter > 0)
                        {
                            valid = false;
                            response.Message = "There is an OPC with the same AvanceID in the Database";
                        }
                    }

                    if (valid)
                    {
                        var query = new tblOPCS();
                        query.opc = (model.OPCSInfo_FirstName + " " + model.OPCSInfo_MiddleName + " " + model.OPCSInfo_LastName + " " + model.OPCSInfo_SecondSurname).Replace("  ", " ");
                        query.firstName = model.OPCSInfo_FirstName;
                        query.middleName = model.OPCSInfo_MiddleName;
                        query.lastName = model.OPCSInfo_LastName;
                        query.secondSurname = model.OPCSInfo_SecondSurname;
                        query.credential = model.OPCSInfo_Credential;
                        query.phone1 = model.OPCSInfo_Phone1;
                        query.phone2 = model.OPCSInfo_Phone2;
                        query.enlistDate = model.OPCSInfo_EnlistDate;
                        if (model.OPCSInfo_PayingCompany != 0)
                        {
                            query.payingCompanyIDX = model.OPCSInfo_PayingCompany;
                        }
                        query.legacyKey = model.OPCSInfo_LegacyKey;
                        query.avanceID = model.OPCSInfo_AvanceID;
                        //if (model.OPCSInfo_User != "0")
                        if (model.OPCSInfo_User != null && model.OPCSInfo_User != "")
                        {
                            query.userID = Guid.Parse(model.OPCSInfo_User);
                        }
                        query.savedByUserID = session.UserID;
                        query.dateSaved = DateTime.Now;
                        if (model.OPCSInfo_Company != 0)
                        {
                            query.companyID = model.OPCSInfo_Company;
                        }

                        //equipos
                        if (model.OPCSInfo_TeamsStr != null)
                        {
                            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                            List<OPCSModel.OPCTeamInfoModel> Teams = js.Deserialize<List<OPCSModel.OPCTeamInfoModel>>(model.OPCSInfo_TeamsStr);
                            foreach (OPCSModel.OPCTeamInfoModel team in Teams)
                            {
                                //nuevo
                                tblOPC_PromotionTeams newTeam = new tblOPC_PromotionTeams();
                                newTeam.promotionTeamID = team.OPCTeamInfo_PromotionTeam;
                                newTeam.jobPositionID = team.OPCTeamInfo_JobPosition;
                                if (team.OPCTeamInfo_ParentOpc != null && team.OPCTeamInfo_ParentOpc > 0)
                                {
                                    newTeam.parentOpcID = team.OPCTeamInfo_ParentOpc;
                                }
                                newTeam.dateSaved = DateTime.Now;
                                if (team.OPCTeamInfo_EnlistDate != null && team.OPCTeamInfo_EnlistDate != "")
                                {
                                    newTeam.enlistDate = DateTime.Parse(team.OPCTeamInfo_EnlistDate);
                                }
                                if (team.OPCTeamInfo_TerminateDate != null && team.OPCTeamInfo_TerminateDate != "")
                                {
                                    newTeam.terminateDate = DateTime.Parse(team.OPCTeamInfo_TerminateDate);
                                }
                                newTeam.terminateReason = team.OPCTeamInfo_TerminateReason;
                                newTeam.deleted = team.OPCTeamInfo_Deleted;
                                query.tblOPC_PromotionTeams.Add(newTeam);
                            }
                        }

                        db.tblOPCS.AddObject(query);
                        db.SaveChanges();

                        //guardar log de Alta
                        List<ChangesTracking.ChangeItem> changes = new List<ChangesTracking.ChangeItem>();

                        changes.Add(Utils.ChangesTracking.setChangeLog(10619, "OPC", "", query.opc, query.opc + " - se dió de Alta en la Lista de Promotores", query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));

                        if (model.OPCSInfo_TeamsStr != null)
                        {
                            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                            List<OPCSModel.OPCTeamInfoModel> Teams = js.Deserialize<List<OPCSModel.OPCTeamInfoModel>>(model.OPCSInfo_TeamsStr);
                            foreach (OPCSModel.OPCTeamInfoModel team in Teams)
                            {
                                changes.Add(Utils.ChangesTracking.setChangeLog(10619, "OPC", "", model.OPCSInfo_FirstName + " " + model.OPCSInfo_MiddleName + " " + model.OPCSInfo_LastName + " " + model.OPCSInfo_SecondSurname, query.opc + " - se unió al equipo " + team.OPCTeamInfo_PromotionTeamText + " como " + team.OPCTeamInfo_JobPositionText, query.opcID.ToString(), "http://eplat.villagroup.com/settings/catalogs"));
                            }
                        }

                        ChangesTracking.LogChanges(changes);

                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "OPC Saved";
                        response.ObjectID = query.opcID;
                        return response;
                    }
                    else
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "There is an OPC with the same Legacy Key/Avance ID in the Database";
                        response.ObjectID = 0;
                        return response;
                    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    response.Type = Attempt_ResponseTypes.Error;
                    //    response.Message = "OPC not Saved";
                    //    response.ObjectID = 0;
                    //    response.Exception = ex;
                    //    return response;
                    //}
                }
            }

            public OPCSModel.OPCSInfoModel GetOPC(int OPCSInfo_OpcID)
            {
                ePlatEntities db = new ePlatEntities();
                OPCSModel.OPCSInfoModel model = new OPCSModel.OPCSInfoModel();

                var query = db.tblOPCS.Single(m => m.opcID == OPCSInfo_OpcID);
                model.OPCSInfo_OpcID = query.opcID;
                model.OPCSInfo_Opc = query.opc;
                model.OPCSInfo_FirstName = query.firstName;
                model.OPCSInfo_MiddleName = query.middleName;
                model.OPCSInfo_LastName = query.lastName;
                model.OPCSInfo_SecondSurname = query.secondSurname;
                model.OPCSInfo_Credential = query.credential;
                model.OPCSInfo_Phone1 = query.phone1;
                model.OPCSInfo_Phone2 = query.phone2;
                model.OPCSInfo_EnlistDate = query.enlistDate;
                model.OPCSInfo_PayingCompany = query.payingCompanyIDX != null ? (int)query.payingCompanyIDX : 0;
                model.OPCSInfo_LegacyKey = query.legacyKey;
                model.OPCSInfo_AvanceID = query.avanceID;
                model.OPCSInfo_User = query.userID != null ? ((Guid)query.userID).ToString() : "";
                model.OPCSInfo_Company = query.companyID != null ? (int)query.companyID : 0;

                List<OPCSModel.OPCTeamInfoModel> teams = new List<OPCSModel.OPCTeamInfoModel>();
                foreach (tblOPC_PromotionTeams team in query.tblOPC_PromotionTeams.Where(x => x.terminateDate == null || x.enlistDate == null || (x.terminateDate != null && x.enlistDate != null && x.enlistDate.Value.Date != x.terminateDate.Value.Date)))
                {
                    OPCSModel.OPCTeamInfoModel newTeam = new OPCSModel.OPCTeamInfoModel();
                    newTeam.OPCPromotionTeamID = team.opc_promotionTeamID;
                    newTeam.OPCTeamInfo_PromotionTeam = team.promotionTeamID;
                    newTeam.OPCTeamInfo_PromotionTeamText = team.tblPromotionTeams.promotionTeam;
                    newTeam.OPCTeamInfo_JobPosition = team.jobPositionID;
                    newTeam.OPCTeamInfo_JobPositionText = team.tblJobPositions.jobPosition;
                    newTeam.OPCTeamInfo_ParentOpc = (team.parentOpcID != null ? team.parentOpcID : -1);
                    newTeam.OPCTeamInfo_ParentOpcText = (team.parentOpcID != null ? team.tblOPCS1.opc : "None");
                    newTeam.OPCTeamInfo_EnlistDate = (team.enlistDate != null ? team.enlistDate.Value.ToString("yyyy-MM-dd") : "");
                    newTeam.OPCTeamInfo_Deleted = team.deleted;
                    newTeam.OPCTeamInfo_TerminateDate = (team.terminateDate != null ? team.terminateDate.Value.ToString("yyyy-MM-dd") : "");
                    newTeam.OPCTeamInfo_TerminateReason = team.terminateReason;
                    newTeam.OPCTeamInfo_AssignToParentOpc = (team.terminateAssignSubsToOpcID != null ? team.terminateAssignSubsToOpcID : -1);
                    newTeam.OPCTeamInfo_AssignToParentOpcText = (team.terminateAssignSubsToOpcID != null ? team.tblOPCS2.opc : "None");
                    newTeam.OPCTeamInfo_Subs = db.tblOPC_PromotionTeams.Count(x => x.deleted != true && x.parentOpcID == query.opcID);
                    if (newTeam.OPCTeamInfo_Subs > 0)
                    {
                        newTeam.OPCTeamInfo_HasSubs = true;
                    }
                    else
                    {
                        newTeam.OPCTeamInfo_HasSubs = false;
                    }
                    newTeam.OPCTeamInfo_DateSaved = team.dateSaved.ToString("yyyy-MM-dd");
                    newTeam.OPCTeamInfo_TemporalID = Guid.NewGuid().ToString();
                    teams.Add(newTeam);
                }

                model.OPCSInfo_Teams = teams;

                return model;
            }

            public AttemptResponse SendChangesReport()
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                //get report
                string userId = session.UserID.ToString();
                var logTables = Utils.GeneralFunctions.GetLogTables(DateTime.Today, ",");

                var logs = db.sp_getLogsByUser(logTables, userId, DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"), ",");
                string logsTable = "<table border=\"0\" style=\"font-family: verdana; font-size: 12px;\" width=\"100%\" cellpadding=\"15\">";
                int index = 1;
                foreach (var log in logs)
                {
                    if (log.sysComponent == "OPC")
                    {
                        int referenceID = Int32.Parse(log.referenceID);
                        var opcQ = db.tblOPCS.SingleOrDefault(x => x.opcID == referenceID);
                        string opcData = "Credencial: " + opcQ.credential + "<br>Nombre: " + opcQ.opc + "<br>Fecha de Alta: " + (opcQ.enlistDate != null ? opcQ.enlistDate.Value.ToString("yyyy-MM-dd") : "") + "<br>Pagadora: " + opcQ.tblPayingCompanies.payingCompany + "<br>Companía: " + opcQ.tblCompanies.company + "<br>Legacy Key: " + opcQ.legacyKey;

                        logsTable += "<tr " + (index % 2 != 0 ? "style=\"background-color:#ddd;\"" : "") + "><td>" + log.logDateTime + "</td><td colspan=\"3\">" + log.referenceText + "<br>" + opcData + "</td></tr>";
                    }
                    else
                    {
                        logsTable += "<tr " + (index % 2 != 0 ? "style=\"background-color:#ddd;\"" : "") + "><td>" + log.logDateTime + "</td><td>" + log.referenceText + "</td><td>De: " + log.previousValue + "</td><td>A: " + log.currentValue + "</td></tr>";
                    }
                    index++;
                }
                logsTable += "</table>";
                logsTable += "<br /><a href=\"http://eplat.villagroup.com/Catalogs/GetOPCsList/" + session.Terminals + "\">Descargar Lista de Promotores</a>";


                //get mail
                long?[] currentTerminals = (from m in session.Terminals.Split(',').Select(m => (long?)long.Parse(m)) select m).ToArray();

                System.Net.Mail.MailMessage emailObj = null;
                string culture = "es-MX";
                var emailQ = (from e in db.tblEmailNotifications
                              where e.eventID == 8
                              && currentTerminals.Contains(e.terminalID)
                              && e.tblEmails.culture == culture
                              && e.tblEmails.tblEmailTemplates.culture == culture
                              select e).FirstOrDefault();

                if (emailQ != null)
                {
                    emailObj = new System.Net.Mail.MailMessage();
                    emailObj.From = new System.Net.Mail.MailAddress(emailQ.tblEmails.sender, emailQ.tblEmails.alias);
                    string[] Bccs = emailQ.emailAccounts.Trim(',').Split(',');
                    if (Bccs.Length > 0)
                    {
                        foreach (string bcc in Bccs)
                        {
                            emailObj.Bcc.Add(bcc);
                        }
                    }
                    emailObj.Subject = emailQ.tblEmails.subject;
                    emailObj.Body = emailQ.tblEmails.tblEmailTemplates.htmlTemplate.Replace("$Content", emailQ.tblEmails.content_);
                    emailObj.IsBodyHtml = true;
                    emailObj.Priority = System.Net.Mail.MailPriority.Normal;
                }

                System.Net.Mail.MailMessage email = emailObj;
                email.Body = email.Body.Replace("$Logs", logsTable);

                //Utils.EmailNotifications.Send(email);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Changes Report Successfully Sent";
                //response.ObjectID = query.opcID;
                return response;
            }

            public string GetOPCsHistoryTable(string terminals)
            {
                ePlatEntities db = new ePlatEntities();
                long[] currentTerminals = (terminals ?? "").Split(',').Select<string, long>(long.Parse).ToArray();
                List<OPCSModel.OPCHistoryModel> History = new List<OPCSModel.OPCHistoryModel>();
                string table = "<!doctype html><html lang=\"es\"><head><meta charset=\"utf-8\"></head><body><table>";
                table += "<tr style=\"background-color: #252525; color: white;\"><th>Credential</th><th>OPC</th><th>Movement</th><th>Date</th><th>Team</th><th>Paying Company</th><th>Company</th><th>Comments<th></tr>";

                var HiringQ = (from o in db.tblOPC_PromotionTeams
                               where currentTerminals.Contains(o.tblOPCS.tblCompanies.tblTerminals_Companies.FirstOrDefault().terminalID)
                               && o.enlistDate != null
                               orderby o.enlistDate
                               select new
                               {
                                   o.tblOPCS.credential,
                                   o.tblOPCS.opc,
                                   o.enlistDate,
                                   o.tblPromotionTeams.promotionTeam,
                                   payingCompany = o.tblOPCS.tblCompanies1.company,
                                   o.tblOPCS.tblCompanies.company
                               }).Distinct();

                foreach (var h in HiringQ)
                {
                    OPCSModel.OPCHistoryModel newItem = new OPCSModel.OPCHistoryModel();
                    newItem.Credential = h.credential;
                    newItem.OPC = h.opc;
                    newItem.Movement = "Hiring";
                    newItem.Date = (DateTime)h.enlistDate;
                    newItem.Team = h.promotionTeam;
                    newItem.PayingCompany = h.payingCompany;
                    newItem.Company = h.company;
                    History.Add(newItem);
                }

                var TerminationQ = from o in db.tblOPC_PromotionTeams
                                   where currentTerminals.Contains(o.tblOPCS.tblCompanies.tblTerminals_Companies.FirstOrDefault().terminalID)
                                   && o.terminateDate != null
                                   orderby o.enlistDate
                                   select new
                                   {
                                       o.tblOPCS.credential,
                                       o.tblOPCS.opc,
                                       o.terminateDate,
                                       o.tblPromotionTeams.promotionTeam,
                                       payingCompany = o.tblOPCS.tblCompanies1.company,
                                       o.tblOPCS.tblCompanies.company,
                                       o.terminateReason
                                   };

                foreach (var t in TerminationQ)
                {
                    OPCSModel.OPCHistoryModel newItem = new OPCSModel.OPCHistoryModel();
                    newItem.Credential = t.credential;
                    newItem.OPC = t.opc;
                    newItem.Movement = "Termination";
                    newItem.Date = (DateTime)t.terminateDate;
                    newItem.Team = t.promotionTeam;
                    newItem.PayingCompany = t.payingCompany;
                    newItem.Company = t.company;
                    newItem.Reason = t.terminateReason;
                    History.Add(newItem);
                }

                History = History.OrderBy(x => x.Date).ToList();

                foreach (var h in History)
                {
                    table += "<tr><td>" + h.Credential + "</td><td>" + h.OPC + "</td><td>" + h.Movement + "</td><td>" + h.Date.ToString("yyyy-MM-dd") + "</td><td>" + h.Team + "</td><td>" + h.PayingCompany + "</td><td>" + h.Company + "</td><td>" + h.Reason + "</td></tr>";
                }

                table += "</table></body></html>";
                return table;
            }

            public string GetOPCsListTable(string terminals)
            {
                string table = "<!doctype html><html lang=\"es\"><head><meta charset=\"utf-8\"></head><body><table>";
                table += "<tr style=\"background-color: #252525; color: white;\"><th>Promotion Team</th><th>Credential</th><th>OPC</th><th>Job Position</th><th>Phone</th><th>Enlist Date</th><th>Paying Company</th><th>Company</th><th>Legacy Key</th><th>Avance ID</th><th>Destination</th></tr>";

                OPCSModel.SearchOPCSModel model = new OPCSModel.SearchOPCSModel();
                long[] currentTerminals = (terminals ?? "").Split(',').Select<string, long>(long.Parse).ToArray();
                model.SearchOPCS_Terminals = currentTerminals;
                int index = 1;
                List<OPCSModel.OPCSInfoModel> opcsList = SearchOPCS(model);
                foreach (OPCSModel.OPCSInfoModel opc in opcsList)
                {
                    table += "<tr " + (index % 2 != 0 ? "style=\"background-color:#ddd;\"" : "") + "><td>" + opc.OPCSInfo_PromotionTeamText + "</td><td>" + opc.OPCSInfo_Credential + "</td><td>" + opc.OPCSInfo_Opc + "</td><td>" + opc.OPCSInfo_JobPositionText + "</td><td>" + opc.OPCSInfo_Phone1 + " / " + opc.OPCSInfo_Phone2 + "</td><td>" + opc.OPCSInfo_EnlistDate + "</td><td>" + opc.OPCSInfo_PayingCompanyText + "</td><td>" + opc.OPCSInfo_CompanyText + "</td><td>" + opc.OPCSInfo_LegacyKey + "</td><td>" + opc.OPCSInfo_AvanceID + "</td><td>" + opc.OPCSInfo_Destination + "</td></tr>";
                    index++;
                }

                table += "</table></body></html>";
                return table;
            }

            public AttemptResponse DeleteOPC(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblOPCS.Single(m => m.opcID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "OPC Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "OPC NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class PayingCompanies
        {
            public class PayingCompaniesCatalogs
            {
                public static List<SelectListItem> FillDrpPayingCompanies()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();

                    long[] currentTerminals = (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray();
                    //var companies = db.tblTerminals_Companies.Where(m => currentTerminals.Contains(m.terminalID)).Select(m => m.companyID).ToArray();

                    //db.tblCompanies_PayingCompanies.Where(p => companies.Contains(p.companyID)).OrderBy(x => x.tblPayingCompanies.payingCompany).Select(p => p.tblPayingCompanies).Distinct()

                    var companies = from c in db.tblTerminals_Companies
                                    where currentTerminals.Contains(c.terminalID)
                                    && c.tblCompanies.companyTypeID == 3
                                    select new
                                    {
                                        c.tblCompanies.companyID,
                                        c.tblCompanies.company
                                    };

                    foreach (var i in companies)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.companyID.ToString(),
                            Text = i.company
                        });
                    }

                    return list;
                }
            }
        }

        public class PromotionTeams
        {
            public class PromotionTeamsCatalogs
            {
                public static List<SelectListItem> FillDrpPromotionTeamsByTerminal()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();
                    var _terminals = session.Terminals != "" ? session.Terminals : session.UserTerminals;
                    var terminals = _terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                    long terminalID = terminals[0];
                    var promoTeams = from t in db.tblTerminals_PromotionTeams
                                     where t.terminalID == terminalID
                                     orderby t.tblPromotionTeams.promotionTeam
                                     select new
                                     {
                                         t.tblPromotionTeams.promotionTeam,
                                         t.promotionTeamID
                                     };

                    foreach (var i in promoTeams)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.promotionTeamID.ToString(),
                            Text = i.promotionTeam
                        });
                    }
                    list.Insert(0, ListItems.Default());
                    return list;
                }

                public static List<SelectListItem> FillDrpPromotionTeams()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();
                    var _terminals = session.Terminals != "" ? session.Terminals : session.UserTerminals;
                    var terminals = _terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                    var destinations = db.tblTerminals_Destinations.Where(d => terminals.Contains(d.terminalID)).Select(d => d.destinationID).ToArray();

                    var companies = db.tblTerminals_Companies.Where(c => terminals.Contains(c.terminalID)).Select(c => c.companyID).ToArray();

                    var promoTeams = from t in db.tblPromotionTeams
                                     where destinations.Contains(t.destinationID)
                                     && (companies.Contains((int)t.companyID) || t.companyID == null)
                                     orderby t.promotionTeam
                                     select t;

                    foreach (var i in promoTeams)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.promotionTeamID.ToString(),
                            Text = i.promotionTeam
                        });
                    }
                    return list;
                }
            }

            public List<PromotionTeamsModel.PromotionTeamsInfoModel> SearchPromotionTeams(PromotionTeamsModel.SearchPromotionTeamsModel model)
            {
                List<PromotionTeamsModel.PromotionTeamsInfoModel> list = new List<PromotionTeamsModel.PromotionTeamsInfoModel>();
                ePlatEntities db = new ePlatEntities();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                //var companies = model.SearchPromotionTeams_Companies != null ? model.SearchPromotionTeams_Companies.Split(',').Select(m => int.Parse(m)).ToArray() : new int[] { };
                var companies = model.SearchPromotionTeams_Companies != null ? model.SearchPromotionTeams_Companies.Select(m => (int?)int.Parse(m)).ToArray() : db.tblTerminals_Companies.Where(m => terminals.Contains(m.terminalID)).Select(m => (int?)m.companyID).ToArray();
                var destinations = model.SearchPromotionTeams_Destinations != null ? model.SearchPromotionTeams_Destinations.Select(m => (long)long.Parse(m)).ToArray() : PlaceDataModel.GetDestinationsByCurrentTerminals().Select(m => long.Parse(m.Value)).ToArray();
                var query = from team in db.tblPromotionTeams
                            where (team.promotionTeam.Contains(model.SearchPromotionTeams_PromotionTeam) || model.SearchPromotionTeams_PromotionTeam == null)
                            //&& (companies.Contains((int)team.companyID) || model.SearchPromotionTeams_Companies == null)
                            && companies.Contains(team.companyID)
                            && destinations.Contains(team.destinationID)
                            select team;

                foreach (var i in query)
                {
                    list.Add(new PromotionTeamsModel.PromotionTeamsInfoModel()
                    {
                        PromotionTeamsInfo_PromotionTeamID = i.promotionTeamID,
                        PromotionTeamsInfo_PromotionTeam = i.promotionTeam,
                        PromotionTeamsInfo_Company = i.companyID != null ? (int)i.companyID : 0,
                        PromotionTeamsInfo_CompanyText = i.companyID != null ? i.tblCompanies.company : "",
                        PromotionTeamsInfo_Destination = i.destinationID != null ? (int)i.destinationID : 0,
                        PromotionTeamsInfo_DestinationText = i.destinationID != null ? i.tblDestinations.destination : ""
                    });
                }
                return list;
            }

            public AttemptResponse SavePromotionTeam(PromotionTeamsModel.PromotionTeamsInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.PromotionTeamsInfo_PromotionTeamID != 0)
                {
                    try
                    {
                        var query = db.tblPromotionTeams.Single(m => m.promotionTeamID == model.PromotionTeamsInfo_PromotionTeamID);
                        query.promotionTeam = model.PromotionTeamsInfo_PromotionTeam;
                        query.companyID = model.PromotionTeamsInfo_Company;
                        query.destinationID = model.PromotionTeamsInfo_Destination;
                        query.giftingBudget = model.PromotionTeamsInfo_GiftingBudget != 0 ? model.PromotionTeamsInfo_GiftingBudget : (decimal?)null;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Promotion Team Updated";
                        response.ObjectID = query.promotionTeamID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Promotion Team NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblPromotionTeams();
                        query.promotionTeam = model.PromotionTeamsInfo_PromotionTeam;
                        query.companyID = model.PromotionTeamsInfo_Company;
                        query.destinationID = model.PromotionTeamsInfo_Destination;
                        query.giftingBudget = model.PromotionTeamsInfo_GiftingBudget != 0 ? model.PromotionTeamsInfo_GiftingBudget : (decimal?)null;
                        db.tblPromotionTeams.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Promotion Team Saved";
                        response.ObjectID = query.promotionTeamID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Promotion Team NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public PromotionTeamsModel.PromotionTeamsInfoModel GetPromotionTeam(int PromotionTeamsInfo_PromotionTeamID)
            {
                ePlatEntities db = new ePlatEntities();
                var model = new PromotionTeamsModel.PromotionTeamsInfoModel();

                var query = db.tblPromotionTeams.Single(m => m.promotionTeamID == PromotionTeamsInfo_PromotionTeamID);
                model.PromotionTeamsInfo_PromotionTeamID = query.promotionTeamID;
                model.PromotionTeamsInfo_PromotionTeam = query.promotionTeam;
                model.PromotionTeamsInfo_Destination = query.destinationID != null ? (int)query.destinationID : 0;
                model.PromotionTeamsInfo_DestinationText = query.destinationID != null ? query.tblDestinations.destination : "";
                model.PromotionTeamsInfo_Company = query.companyID != null ? (int)query.companyID : 0;
                model.PromotionTeamsInfo_CompanyText = query.companyID != null ? query.tblCompanies.company : "";
                return model;
            }

            public AttemptResponse DeletePromotionTeam(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblPromotionTeams.Single(m => m.promotionTeamID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Promotion Team Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Promotion Team NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class CouponFolios
        {
            public class CouponFoliosCatalogs
            {
                public static List<SelectListItem> FillDrpCouponFolios()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();

                    foreach (var i in db.tblCouponFolios)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.couponFolioID.ToString(),
                            Text = i.couponFolioID.ToString()
                        });
                    }
                    return list;
                }
            }

            public List<CouponFoliosModel.CouponFoliosInfoModel> SearchCouponFolios(CouponFoliosModel.SearchCouponFoliosModel model)
            {
                ePlatEntities db = new ePlatEntities();
                List<CouponFoliosModel.CouponFoliosInfoModel> list = new List<CouponFoliosModel.CouponFoliosInfoModel>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                //var companies = model.SearchCouponFolios_Companies != null ? model.SearchCouponFolios_Companies.Split(',').Select(m => int.Parse(m)).ToArray() : new int[] { };
                var companies = model.SearchCouponFolios_Companies != null ? model.SearchCouponFolios_Companies.Split(',').Select(m => (int?)int.Parse(m)).ToArray() : db.tblTerminals_Companies.Where(m => terminals.Contains(m.terminalID)).Select(m => (int?)m.companyID).ToArray();
                long? fromFolio = 0;
                long? toFolio = 0;
                if (model.SearchCouponFolios_FromFolio != null)
                {
                    fromFolio = long.Parse(model.SearchCouponFolios_FromFolio);
                }
                if (model.SearchCouponFolios_ToFolio != null)
                {
                    toFolio = long.Parse(model.SearchCouponFolios_ToFolio);
                }

                var query = from coupon in db.tblCouponFolios
                            where (model.SearchCouponFolios_FromFolio == null || coupon.fromFolio == fromFolio)
                            && (model.SearchCouponFolios_ToFolio == null || coupon.toFolio == toFolio)
                            && (coupon.serial == model.SearchCouponFolios_Serial || model.SearchCouponFolios_Serial == null)
                            && companies.Contains(coupon.companyID)
                            //&& (companies.Contains((int)coupon.companyID) || model.SearchCouponFolios_Companies == null)
                            select coupon;

                foreach (var i in query)
                {
                    list.Add(new CouponFoliosModel.CouponFoliosInfoModel()
                    {
                        CouponFoliosInfo_CouponFolioID = i.couponFolioID,
                        CouponFoliosInfo_FromFolio = i.fromFolio != null ? i.fromFolio.ToString() : "",
                        CouponFoliosInfo_ToFolio = i.toFolio != null ? i.toFolio.ToString() : "",
                        CouponFoliosInfo_Serial = i.serial != null ? i.serial : "",
                        CouponFoliosInfo_Delivered = i.delivered != null ? i.delivered.ToString() : "",
                        CouponFoliosInfo_Generated = i.generated != null ? i.generated.ToString() : "",
                        CouponFoliosInfo_Available = i.available != null ? i.available.ToString() : "",
                        CouponFoliosInfo_CompanyText = i.companyID != null ? i.tblCompanies.company : ""
                    });
                }
                return list;
            }

            public AttemptResponse SaveCouponFolio(CouponFoliosModel.CouponFoliosInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.CouponFoliosInfo_CouponFolioID != 0)
                {
                    try
                    {
                        var query = db.tblCouponFolios.Single(m => m.couponFolioID == model.CouponFoliosInfo_CouponFolioID);
                        query.fromFolio = long.Parse(model.CouponFoliosInfo_FromFolio);
                        query.toFolio = long.Parse(model.CouponFoliosInfo_ToFolio);
                        query.serial = model.CouponFoliosInfo_Serial;
                        query.delivered = (int)((long.Parse(model.CouponFoliosInfo_ToFolio) - long.Parse(model.CouponFoliosInfo_FromFolio)) + 1);
                        query.available = ((int)((long.Parse(model.CouponFoliosInfo_ToFolio) - long.Parse(model.CouponFoliosInfo_FromFolio)) + 1)) - query.generated;
                        //query.available = (int)((long.Parse(model.CouponFoliosInfo_ToFolio) - long.Parse(model.CouponFoliosInfo_FromFolio)) + 1);
                        if (model.CouponFoliosInfo_Company != "0")
                        {
                            query.companyID = int.Parse(model.CouponFoliosInfo_Company);
                        }
                        if (model.CouponFoliosInfo_PointOfSale != "0")
                        {
                            query.pointOfSaleID = int.Parse(model.CouponFoliosInfo_PointOfSale);
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Coupon Folio Updated";
                        response.ObjectID = query.couponFolioID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Coupon Folio NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblCouponFolios();
                        query.fromFolio = long.Parse(model.CouponFoliosInfo_FromFolio);
                        query.toFolio = long.Parse(model.CouponFoliosInfo_ToFolio);
                        query.serial = model.CouponFoliosInfo_Serial;
                        query.delivered = (int)((int.Parse(model.CouponFoliosInfo_ToFolio) - int.Parse(model.CouponFoliosInfo_FromFolio)) + 1);
                        query.generated = 0;
                        query.available = (int)((int.Parse(model.CouponFoliosInfo_ToFolio) - int.Parse(model.CouponFoliosInfo_FromFolio)) + 1);
                        if (model.CouponFoliosInfo_Company != "0")
                        {
                            query.companyID = int.Parse(model.CouponFoliosInfo_Company);
                        }
                        if (model.CouponFoliosInfo_PointOfSale != "0")
                        {
                            query.pointOfSaleID = int.Parse(model.CouponFoliosInfo_PointOfSale);
                        }
                        db.tblCouponFolios.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Coupon Folio Saved";
                        response.ObjectID = query.couponFolioID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Coupon Folio NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public CouponFoliosModel.CouponFoliosInfoModel GetCouponFolio(int CouponFoliosInfo_CouponFolioID)
            {
                ePlatEntities db = new ePlatEntities();
                CouponFoliosModel.CouponFoliosInfoModel model = new CouponFoliosModel.CouponFoliosInfoModel();

                var query = db.tblCouponFolios.Single(m => m.couponFolioID == CouponFoliosInfo_CouponFolioID);
                model.CouponFoliosInfo_CouponFolioID = query.couponFolioID;
                model.CouponFoliosInfo_FromFolio = query.fromFolio != null ? query.fromFolio.ToString() : "";
                model.CouponFoliosInfo_ToFolio = query.toFolio != null ? query.toFolio.ToString() : "";
                model.CouponFoliosInfo_Serial = query.serial;
                model.CouponFoliosInfo_Available = query.available != null ? query.available.ToString() : "";
                model.CouponFoliosInfo_Generated = query.generated != null ? query.generated.ToString() : "";
                model.CouponFoliosInfo_Delivered = query.delivered != null ? query.delivered.ToString() : "";
                model.CouponFoliosInfo_Company = query.companyID != null ? query.companyID.ToString() : "";
                model.CouponFoliosInfo_CompanyText = query.companyID != null ? query.tblCompanies.company : "";
                model.CouponFoliosInfo_PointOfSale = query.pointOfSaleID != null ? query.pointOfSaleID.ToString() : "";
                return model;
            }

            public AttemptResponse DeleteCouponFolio(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                try
                {
                    var query = db.tblCouponFolios.Single(m => m.couponFolioID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Coupon Folio Deleted";
                    response.ObjectID = query.couponFolioID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Coupon Folio NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class PointsOfSale
        {
            public class PoinsOfSaleCatalogs
            {
                public static List<SelectListItem> FillDrpPointsOfSale()
                {
                    ePlatEntities db = new ePlatEntities();

                    var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                    var list = new List<SelectListItem>();

                    foreach (var i in db.tblPointsOfSale.Where(m => terminals.Contains(m.terminalID)))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.pointOfSaleID.ToString(),
                            Text = i.shortName + " - " + i.pointOfSale
                        });
                    }
                    return list;
                }

                public static List<SelectListItem> FillDrpPoliciesBlock()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();
                    var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                    var query = db.tblBlocks.Where(m => terminals.Contains(m.terminalID));

                    foreach (var i in query)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.blockID.ToString(),
                            Text = i.block
                        });
                    }
                    return list;
                }
            }

            public List<PointsOfSaleModel.PointsOfSaleInfoModel> SearchPointsOfSale(PointsOfSaleModel.SearchPointsOfSaleModel model)
            {
                ePlatEntities db = new ePlatEntities();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var list = new List<PointsOfSaleModel.PointsOfSaleInfoModel>();
                int placesLength = model.SearchPointsOfSale_Places != null ? model.SearchPointsOfSale_Places.Count() : 0;
                var places = model.SearchPointsOfSale_Places != null ? model.SearchPointsOfSale_Places.Select(m => long.Parse(m)).ToArray() : new long[] { };

                var query = from point in db.tblPointsOfSale
                            where (point.pointOfSale.Contains(model.SearchPointsOfSale_PointOfSale) || model.SearchPointsOfSale_PointOfSale == null)
                            && (places.Contains(point.placeID) || placesLength == 0)
                            && terminals.Contains(point.terminalID)
                            select point;

                foreach (var i in query)
                {
                    list.Add(new PointsOfSaleModel.PointsOfSaleInfoModel()
                    {
                        PointsOfSaleInfo_PointOfSaleID = i.pointOfSaleID,
                        PointsOfSaleInfo_PointOfSale = i.pointOfSale,
                        PointsOfSaleInfo_Place = i.placeID,
                        PointsOfSaleInfo_PlaceText = i.tblPlaces.place + " " + i.tblPlaces.tblDestinations.destination,
                        PointsOfSaleInfo_ShortName = i.shortName,
                        PointsOfSaleInfo_TerminalName = i.tblTerminals.terminal
                    });
                }
                return list;
            }

            public AttemptResponse SavePointOfSale(PointsOfSaleModel.PointsOfSaleInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.PointsOfSaleInfo_PointOfSaleID != 0)
                {
                    try
                    {
                        var query = db.tblPointsOfSale.Single(m => m.pointOfSaleID == model.PointsOfSaleInfo_PointOfSaleID);
                        query.pointOfSale = model.PointsOfSaleInfo_PointOfSale;
                        query.shortName = model.PointsOfSaleInfo_ShortName;
                        query.placeID = model.PointsOfSaleInfo_Place;
                        if (model.PointsOfSaleInfo_PoliciesBlock != null && model.PointsOfSaleInfo_PoliciesBlock != 0)
                        {
                            query.policiesBlockID = model.PointsOfSaleInfo_PoliciesBlock != 0 ? model.PointsOfSaleInfo_PoliciesBlock : (long?)null;
                            query.acceptCharges = model.PointsOfSaleInfo_AcceptCharges;
                        }
                        query.terminalID = model.PointsOfSaleInfo_Terminal;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Point Of Sale Updated";
                        response.ObjectID = query.pointOfSaleID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Point Of Sale NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblPointsOfSale();
                        query.pointOfSale = model.PointsOfSaleInfo_PointOfSale;
                        query.shortName = model.PointsOfSaleInfo_ShortName;
                        query.placeID = model.PointsOfSaleInfo_Place;
                        if (model.PointsOfSaleInfo_PoliciesBlock != null && model.PointsOfSaleInfo_PoliciesBlock != 0)
                        {
                            query.policiesBlockID = model.PointsOfSaleInfo_PoliciesBlock != 0 ? model.PointsOfSaleInfo_PoliciesBlock : (long?)null;
                            query.acceptCharges = model.PointsOfSaleInfo_AcceptCharges;
                        }
                        else
                        {
                            query.policiesBlockID = (long?)null;
                            query.acceptCharges = false;
                        }
                        query.terminalID = model.PointsOfSaleInfo_Terminal;
                        db.tblPointsOfSale.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Point Of Sale Saved";
                        response.ObjectID = query.pointOfSaleID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Point Of Sale NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public PointsOfSaleModel.PointsOfSaleInfoModel GetPointOfSale(int PointsOfSaleInfo_PointOfSaleID)
            {
                ePlatEntities db = new ePlatEntities();
                var model = new PointsOfSaleModel.PointsOfSaleInfoModel();

                var query = db.tblPointsOfSale.Single(m => m.pointOfSaleID == PointsOfSaleInfo_PointOfSaleID);
                model.PointsOfSaleInfo_PointOfSaleID = query.pointOfSaleID;
                model.PointsOfSaleInfo_PointOfSale = query.pointOfSale;
                model.PointsOfSaleInfo_ShortName = query.shortName;
                model.PointsOfSaleInfo_Place = query.placeID;
                model.PointsOfSaleInfo_PlaceText = query.tblPlaces.place;
                model.PointsOfSaleInfo_Terminal = query.terminalID;
                return model;
            }

            public AttemptResponse DeletePointOfSale(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                try
                {
                    var query = db.tblPointsOfSale.Single(m => m.pointOfSaleID == targetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Point Of Sale Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Point Of Sale NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class Promos
        {
            public class PromosCatalogs
            {
                public static List<SelectListItem> FillDrpPromoTypes()
                {
                    ePlatEntities db = new ePlatEntities();
                    var list = new List<SelectListItem>();

                    foreach (var i in db.tblPromoTypes)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.promoTypeID.ToString(),
                            Text = i.promoType
                        });
                    }
                    return list;
                }
            }

            public List<PromosModel.PromoInfoModel> SearchPromos(PromosModel.PromoSearchModel model)
            {
                ePlatEntities db = new ePlatEntities();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var promoTypes = model.PromoSearch_PromoTypes == null ? new int[] { } : model.PromoSearch_PromoTypes.Select(m => int.Parse(m)).ToArray();
                var FromDateBW = model.PromoSearch_FromDateBW == null ? (DateTime?)null : DateTime.Parse(model.PromoSearch_FromDateBW);
                var ToDateBW = model.PromoSearch_ToDateBW == null ? (DateTime?)null : DateTime.Parse(model.PromoSearch_ToDateBW);
                var FromDateTW = model.PromoSearch_FromDateTW == null ? (DateTime?)null : DateTime.Parse(model.PromoSearch_FromDateTW);
                var ToDateTW = model.PromoSearch_ToDateTW == null ? (DateTime?)null : DateTime.Parse(model.PromoSearch_ToDateTW);

                //var _query =  from p in db.tblPromos
                //              where ( p.fromDateBW >= FromDateBW && p.toDateBW >= ToDateBW || model.PromoSearch_FromDateBW == null && model.PromoSearch_ToDateBW == null)
                //              && (p.fromDateTW >= FromDateTW &&  p.toDateTW ==  ToDateTW || model.PromoSearch_FromDateTW ==  null &&  model.PromoSearch_ToDateTW ==  null)
                //              && (promoTypes.Contains(p.promoID))

                var query = db.tblPromos.Where(m => //m.tblPromos_RelatedItems.FirstOrDefault(x => x.sysItemTypeID == 1).promo_RelatedItemID != null
                                                    //&& m.tblPromos_PromoTypes.FirstOrDefault(x => promoTypes.Contains(x.promoTypeID)).promo_promoTypeID != null
                                                    //&&
                    (m.promo.Contains(model.PromoSearch_Promo) || model.PromoSearch_Promo == null)
                    && (m.promoCode.Contains(model.PromoSearch_PromoCode) || model.PromoSearch_PromoCode == null)
                    && (FromDateBW >= m.fromDateBW && ToDateBW >= m.toDateBW || model.PromoSearch_FromDateBW == null)
                    && (FromDateTW >= m.fromDateTW && ToDateTW >= m.toDateTW || model.PromoSearch_FromDateTW == null)
                    && (terminals.Contains(m.terminalID))
                    );

                List<PromosModel.PromoInfoModel> list = new List<PromosModel.PromoInfoModel>();
                foreach (var i in query)
                {
                    list.Add(new PromosModel.PromoInfoModel()
                    {
                        PromoInfo_PromoID = i.promoID,
                        PromoInfo_PromoCode = i.promoCode,
                        PromoInfo_Promo = i.promo,
                        PromoInfo_FromDateBW = i.fromDateBW.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        PromoInfo_ToDateBW = i.toDateBW.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        PromoInfo_FromDateTW = i.fromDateTW.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        PromoInfo_ToDateTW = i.toDateTW.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        PromoInfo_Publish = i.publish
                    });
                }
                return list;
            }

            public AttemptResponse SavePromo(PromosModel.PromoInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var TerminalID = model.PromoInfo_Terminal;
                var DateFromBW = DateTime.Parse(model.PromoInfo_FromDateBW);
                var DateToBW = DateTime.Parse(model.PromoInfo_ToDateBW);
                var DateFromTW = DateTime.Parse(model.PromoInfo_FromDateTW);
                var DatetoTW = DateTime.Parse(model.PromoInfo_ToDateTW);

                //Update
                if (model.PromoInfo_PromoID != null)
                {
                    var Promo = db.tblPromos.Single(x => x.promoID == model.PromoInfo_PromoID);
                    try
                    {
                        //type value
                        try
                        {
                            var Description = db.tblPromoDescriptions.Where(x => x.promoID == model.PromoInfo_PromoID);
                            var Type = db.tblPromos_PromoTypes.Where(x => x.promoID == model.PromoInfo_PromoID);

                            if (model.PromoInfo_Publish != null)
                            {
                                foreach (var item in Description)
                                {
                                    db.DeleteObject(item);
                                }
                            }
                            foreach (var item in Type)
                            {
                                db.DeleteObject(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Error Ocurred", ex);
                        }

                        var _UpdateType = new tblPromos_PromoTypes();
                        var _UpdateDescription = new tblPromoDescriptions();
                        foreach (var type in model.PromoInfo_Type)
                        {
                            _UpdateType.promoID = model.PromoInfo_PromoID.Value;
                            _UpdateType.promoTypeID = type;
                            Promo.tblPromos_PromoTypes.Add(_UpdateType);
                        }
                        if (model.PromoInfo_Publish != null)
                        {
                            _UpdateDescription.promoID = model.PromoInfo_PromoID.Value;
                            _UpdateDescription.culture = db.tblLanguages.FirstOrDefault(x => x.languageID == model.PromoInfo_DescriptionCulture).languageID.ToString();
                            _UpdateDescription.description = model.PromoInfo_DescriptionDescription;
                        }

                        Promo.terminalID = model.PromoInfo_Terminal;
                        Promo.promo = model.PromoInfo_Promo;
                        Promo.fromDateBW = DateFromBW;
                        Promo.toDateBW = DateToBW;
                        Promo.fromDateTW = DateFromTW;
                        Promo.toDateTW = DatetoTW;
                        Promo.currencyID = model.PromoInfo_Currency;
                        //amount Porcent
                        if (model.PromoInfo_Amount != null)
                        {
                            Promo.amount = model.PromoInfo_Amount;
                            Promo.percentage = (int?)null;
                        }
                        else
                        {
                            Promo.percentage = model.PromoInfo_Percentage;
                            Promo.amount = (int?)null;
                        }
                        Promo.anyPrice = model.PromoInfo_AnyPrice;
                        Promo.wholeStay = model.PromoInfo_WholeStay;
                        Promo.perNight = model.PromoInfo_PerNight;
                        Promo.combinable = model.PromoInfo_Combinable;
                        //Days
                        Promo.monday = model.PromoInfo_Monday;
                        Promo.tuesday = model.PromoInfo_Tuesday;
                        Promo.wednesday = model.PromoInfo_Wednesday;
                        Promo.thursday = model.PromoInfo_Thursday;
                        Promo.friday = model.PromoInfo_Friday;
                        Promo.saturday = model.PromoInfo_Saturday;
                        Promo.sunday = model.PromoInfo_Sunday;
                        #region coment

                        //// verificar la cantidad de tipos que hay segun el id de la promo
                        //var rowsPromoType = db.tblPromos_PromoTypes.Where(y => y.promoID == model.PromoInfo_PromoID);                                       
                        //if( model.PromoInfo_Type.Count() > rowsPromoType.Count())//agregar
                        //{   //intancia para ingresar nuevo tipo
                        //    var _newType = new tblPromos_PromoTypes();           

                        //    foreach(var type in model.PromoInfo_Type)
                        //    {
                        //     if (rowsPromoType.FirstOrDefault(x => x.promoID == model.PromoInfo_PromoID && x.promoTypeID == type).promoID == null) //si el ID o el tipo no existen entonces agregar el tipo
                        //      {
                        //          _newType.promoID = model.PromoInfo_PromoID.Value;
                        //          _newType.promoTypeID = type;
                        //          Promo.tblPromos_PromoTypes.Add(_newType);
                        //      }
                        //      else//sino solo asignar
                        //      {
                        //          var changeType = db.tblPromos_PromoTypes.Single(m => m.promoID == model.PromoInfo_PromoID.Value && m.promoTypeID == type);
                        //          changeType.promoID = model.PromoInfo_PromoID.Value;
                        //          changeType.promoTypeID = type;
                        //          Promo.tblPromos_PromoTypes.Add(changeType);
                        //      }                
                        //    }
                        //}
                        //else if(rowsPromoType.Count() > model.PromoInfo_Type.Count())//eliminar
                        //{
                        //    var _query = from m in db.tblPromos_PromoTypes          // typos guardados
                        //                 where model.PromoInfo_PromoID == m.promoID
                        //                 select m;

                        //    foreach (var x in _query)
                        //    {
                        //        if (!model.PromoInfo_Type.Contains(x.promoTypeID))// si el tipo (BD) no contiene el tipo (model) entonces eliminar
                        //        {
                        //            var DeleteType = db.tblPromos_PromoTypes.Single(z => z.promo_promoTypeID == x.promo_promoTypeID);
                        //            db.DeleteObject(DeleteType);                                   
                        //        }
                        //    }                      
                        //}
                        //if(rowsPromoType.Count() == model.PromoInfo_Type.Count())// igual
                        //{
                        //    foreach(var x in rowsPromoType)
                        //    {
                        //     db.DeleteObject(x);  
                        //    }
                        //    var newType = new tblPromos_PromoTypes();
                        //    foreach(var x in model.PromoInfo_Type)
                        //    {
                        //        newType.promoID = model.PromoInfo_PromoID.Value;
                        //        newType.promoTypeID = x;
                        //        Promo.tblPromos_PromoTypes.Add(newType);
                        //    }
                        //}
                        //if (model.PromoInfo_Publish)
                        //{
                        //     foreach (var x in model.PromoInfo_Description)
                        //     {
                        //         var itemDescription = db.tblPromoDescriptions.Single(y => y.promoID == model.PromoInfo_PromoID);
                        //         itemDescription.promoID = model.PromoInfo_PromoID.Value;
                        //         itemDescription.mainTag = x.PromoDescription_PromoTag;
                        //         itemDescription.title = x.PromoDescription_Title;
                        //         itemDescription.description = x.PromoDescription_Description;
                        //         itemDescription.instructions = x.PromoDescription_Instructions;
                        //         itemDescription.culture = x.PromoDescription_Culture;
                        //       //  Promo.tblPromoDescriptions.Add(itemDescription);
                        //     }
                        //// }
                        #endregion
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Promotion Updated";
                        response.ObjectID = model.PromoInfo_PromoID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Promotion Not Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else//save
                {
                    try
                    {
                        var Promos = new tblPromos();
                        Promos.promo = model.PromoInfo_Promo;
                        Promos.dateSaved = DateTime.Now.Date;
                        Promos.fromDateBW = DateFromBW;
                        Promos.toDateBW = DateToBW;
                        Promos.fromDateTW = DateFromTW;
                        Promos.toDateTW = DatetoTW;
                        Promos.amount = model.PromoInfo_Amount;
                        Promos.currencyID = model.PromoInfo_Currency;
                        Promos.percentage = model.PromoInfo_Percentage;
                        Promos.terminalID = model.PromoInfo_Terminal;
                        Promos.anyPrice = model.PromoInfo_AnyPrice;
                        Promos.applyOnPerson = model.PromoInfo_applyOnPerson;
                        Promos.wholeStay = model.PromoInfo_WholeStay;
                        Promos.perNight = model.PromoInfo_PerNight;
                        Promos.combinable = model.PromoInfo_Combinable;
                        Promos.publish = model.PromoInfo_Publish;
                        Promos.promoCode = model.PromoInfo_PromoCode;
                        Promos.applyOnPerson = model.PromoInfo_applyOnPerson;
                        Promos.monday = model.PromoInfo_Monday;
                        Promos.tuesday = model.PromoInfo_Tuesday;
                        Promos.wednesday = model.PromoInfo_Wednesday;
                        Promos.thursday = model.PromoInfo_Thursday;
                        Promos.friday = model.PromoInfo_Friday;
                        Promos.saturday = model.PromoInfo_Saturday;
                        Promos.sunday = model.PromoInfo_Sunday;
                        foreach (var x in model.PromoInfo_Type)
                        {
                            var itemType = new tblPromos_PromoTypes();
                            itemType.promoTypeID = x;
                            //itemType.promoID = Promos.promoID;
                            Promos.tblPromos_PromoTypes.Add(itemType);
                        }
                        if (model.PromoInfo_Publish)
                        {
                            foreach (var x in model.PromoInfo_DescriptionID.Split(','))
                            {
                                var item = x.Split('|');
                                var itemDescription = new tblPromoDescriptions();
                                itemDescription.promoID = model.PromoInfo_PromoID.Value;
                                itemDescription.mainTag = item[1];    //x.PromoDescription_PromoTag;
                                                                      //itemDescription.title = x.PromoDescription_Title;
                                                                      //itemDescription.description = x.PromoDescription_Description;
                                                                      //itemDescription.instructions = x.PromoDescription_Instructions;
                                itemDescription.culture = item[2];   //x.PromoDescription_Culture;
                                Promos.tblPromoDescriptions.Add(itemDescription);
                            }
                        }
                        db.tblPromos.AddObject(Promos);
                        // db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Promotion Saved";
                        response.ObjectID = model.PromoInfo_PromoID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Promotion Not Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public PromosModel.GetPromoModel GetPromo(int PromoInfo_PromoID)
            {
                ePlatEntities db = new ePlatEntities();
                var PublishList = new List<string>();

                var queryPromo = from x in db.tblPromos
                                 where (x.promoID == PromoInfo_PromoID)
                                 select x;

                var queryDescription = db.tblPromoDescriptions.Where(x => x.promoID == PromoInfo_PromoID);
                var queryPromoType = db.tblPromos_PromoTypes.Where(x => x.promoID == PromoInfo_PromoID);

                PromosModel.GetPromoModel Promo = new PromosModel.GetPromoModel();
                Promo.GetPromoID = queryPromo.FirstOrDefault().promoID;
                Promo.GetPromoTerminal = queryPromo.FirstOrDefault().tblTerminals.terminalID;
                Promo.GetPromoCode = queryPromo.FirstOrDefault().promoCode;
                Promo.GetPromoName = queryPromo.FirstOrDefault().promo;
                Promo.GetPromoFromDateBW = queryPromo.FirstOrDefault().fromDateBW.ToString("yyyy-MM-dd");
                Promo.GetPromoToDateBW = queryPromo.FirstOrDefault().toDateBW.ToString("yyyy-MM-dd");
                Promo.GetPromoFromDateTW = queryPromo.FirstOrDefault().fromDateTW.ToString("yyyy-MM-dd");
                Promo.GetPromoToDateTW = queryPromo.FirstOrDefault().toDateTW.ToString("yyyy-MM-dd");
                Promo.GetPromoCurrency = queryPromo.FirstOrDefault().currencyID.GetValueOrDefault();
                Promo.GetPromoAmount = queryPromo.FirstOrDefault().amount;
                Promo.GetPromoPercentage = queryPromo.FirstOrDefault().percentage;
                Promo.GetPromoAnyPrice = queryPromo.FirstOrDefault().anyPrice;
                Promo.GetPromoWholeStay = queryPromo.FirstOrDefault().wholeStay;
                Promo.GetPromoPerNight = queryPromo.FirstOrDefault().perNight;
                Promo.GetPromoCombinable = queryPromo.FirstOrDefault().combinable;
                Promo.GetPromoMonday = queryPromo.FirstOrDefault().monday;
                Promo.GetPromoTuesday = queryPromo.FirstOrDefault().tuesday;
                Promo.GetPromoWednesday = queryPromo.FirstOrDefault().wednesday;
                Promo.GetPromoThursday = queryPromo.FirstOrDefault().thursday;
                Promo.GetPromoFriday = queryPromo.FirstOrDefault().friday;
                Promo.GetPromoSaturday = queryPromo.FirstOrDefault().saturday;
                Promo.GetPromoSunday = queryPromo.FirstOrDefault().sunday;
                Promo.GetPromoPublish = queryPromo.FirstOrDefault().publish;
                Promo.GetPromoApplyPerson = queryPromo.FirstOrDefault().applyOnPerson;
                Promo.GetPromoPackage = queryPromo.FirstOrDefault().isPackage;
                //type
                if (queryPromoType.Count() > 0)
                {
                    Promo.GetPromoType = new List<PromosModel.PromoTypesModel>();
                    foreach (var x in queryPromoType)
                    {
                        PromosModel.PromoTypesModel TypeList = new PromosModel.PromoTypesModel();
                        TypeList.PromoTypesModelID = x.promo_promoTypeID;
                        TypeList.PromoID = x.promoID;
                        TypeList.PromoTypeID = x.promoTypeID;
                        Promo.GetPromoType.Add(TypeList);
                    }
                }
                //description
                if (queryDescription.Count() > 0 && queryPromo.FirstOrDefault().publish == true)
                {
                    foreach (var x in queryDescription)
                    {
                        PublishList.Add(
                                x.promoID + "|" +
                                x.promoDescriptionID + "|" +
                                x.mainTag + "|" +
                                x.culture + "|" +
                                x.title + "|" +
                                x.instructions + "|" +
                                x.description
                            );
                    }
                    Promo.GetPromoDescription = PublishList;
                }
                return Promo;
            }

            public AttemptResponse DeletePromo(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var queryDeletePromo = db.tblPromos.Single(x => x.promoID == targetID);
                    var queryDeleteType = db.tblPromos_PromoTypes.Where(x => x.promoID == targetID);
                    foreach (var x in db.tblPromos_PromoTypes.Where(x => x.promoID == targetID))
                    {
                        db.DeleteObject(x);
                    }
                    foreach (var x in db.tblPromoDescriptions.Where(x => x.promoID == targetID))
                    {
                        db.DeleteObject(x);
                    }
                    //db.DeleteObject(queryDeletePromo);
                    //db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Promo Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Promotion Not Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }

            public List<SelectListItem> FillDrpPromoPackage()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                foreach (var x in db.tblPromos.Where(n => n.isPackage == true))
                {
                    list.Add(new SelectListItem
                    {
                        Value = x.promoID.ToString(),
                        Text = x.isPackage.ToString()
                    });
                }
                return list;
            }
            public static List<SelectListItem> FillDrpPromoTypes()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                foreach (var Type in db.tblPromoTypes.Where(x => x.promoTypeID != null))
                {

                    list.Add(new SelectListItem()
                    {
                        Value = Type.promoTypeID.ToString(),
                        Text = Type.promoType
                    });
                }
                return list;
            }
        }
        ////Parties 
        public class PartiesSalesDataModel
        {
            public List<PartiesSales.PartiesSearchItem> SearchParties(PartiesSales.SeachParties model)
            {
                ePlatEntities db = new ePlatEntities();
                List<PartiesSales.PartiesSearchItem> list = new List<PartiesSales.PartiesSearchItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                // var program = db.tblProspectationPrograms.Select(x => x.programID);
                var currentTerminals = model.SearchTerminal == null ? new long[] { } : model.SearchTerminal.Split(',').Select(m => long.Parse(m)).ToArray();
                var currentProgramID = model.SearchProgramID == null ? new long[] { } : model.SearchProgramID.Split(',').Select(m => long.Parse(m)).ToArray();
                var fromDate = (DateTime?)null;
                var NextDay = (DateTime?)null;
                if (model.SearchDate_FromDate != null)
                {
                    fromDate = DateTime.Parse(model.SearchDate_FromDate) == DateTime.Today ? DateTime.Now : DateTime.Parse(model.SearchDate_FromDate);
                    NextDay = fromDate.Value.AddDays(1);
                }

                var query = from p in db.tblSalesRoomsParties
                            where (fromDate == null || fromDate <= p.partyDateTime && NextDay >= p.partyDateTime)
                            && (currentProgramID.Count() == 0 || currentProgramID.Contains(p.programID))
                            && (terminals.Contains(p.terminalID))
                            select p;

                foreach (var item in query)
                {
                    list.Add(new PartiesSales.PartiesSearchItem()
                    {
                        SalesRoomPartiesID = item.salesRoomPartyID,
                        PartiesDate = item.partyDateTime.ToString("yyyy-MM-dd hh:mm tt"),
                        SalesRoomParties = item.salesRoomID == null ? "" : db.tblSalesRooms.FirstOrDefault(x => x.salesRoomID == item.salesRoomID).salesRoom,
                        Program = item.tblProspectationPrograms.program,
                        Place = item.tblPlaces.place,
                        Terminal = item.tblTerminals.terminal,
                        Allotment = item.allotment.ToString(),
                        DateSaved = item.dateSaved.ToString("yyyy-MM-dd hh:mm tt"),
                        SavedByUser = item.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + item.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName
                    });
                }
                return list;
            }

            public AttemptResponse SaveParties(PartiesSales.PartiesInfo model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                //update
                if (model.PartiesInfo_SalesPartyID != null)
                {
                    try
                    {
                        var PlaceID = int.Parse(model.PartiesInfo_PlaceID);
                        var SalesPartiesID = int.Parse(model.PartiesInfo_SalesPartyID);
                        var SalesRoomParties = db.tblSalesRoomsParties.Single(x => x.salesRoomPartyID == SalesPartiesID);
                        var Date = DateTime.Parse(model.PartiesInfo_FromDate);
                        SalesRoomParties.partyDateTime = Date;
                        SalesRoomParties.allotment = int.Parse(model.PartiesInfo_Allotment);
                        SalesRoomParties.placeID = PlaceID;
                        SalesRoomParties.programID = int.Parse(model.PartiesInfo_ProgramName);

                        if (model.PartiesInfo_Room != "0")
                        {
                            SalesRoomParties.salesRoomID = int.Parse(model.PartiesInfo_Room);
                        }
                        else
                        {
                            SalesRoomParties.salesRoomID = (int?)null;
                        }
                        SalesRoomParties.terminalID = int.Parse(model.PartiesInfo_Terminal);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Sales Room Updated";
                        response.ObjectID = model.PartiesInfo_SalesPartyID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Sales Room NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else//save
                {
                    try
                    {
                        var PlaceID = int.Parse(model.PartiesInfo_PlaceID);
                        var SalesRoomParties = new tblSalesRoomsParties();
                        var Date = DateTime.Parse(model.PartiesInfo_FromDate);
                        SalesRoomParties.partyDateTime = Date;
                        SalesRoomParties.allotment = int.Parse(model.PartiesInfo_Allotment);
                        SalesRoomParties.placeID = long.Parse(model.PartiesInfo_PlaceID);
                        SalesRoomParties.programID = int.Parse(model.PartiesInfo_ProgramName);
                        if (model.PartiesInfo_Room != "0")
                        {
                            SalesRoomParties.salesRoomID = int.Parse(model.PartiesInfo_Room);
                        }
                        else
                        {
                            SalesRoomParties.salesRoomID = (int?)null;
                        }
                        SalesRoomParties.terminalID = int.Parse(model.PartiesInfo_Terminal);
                        SalesRoomParties.dateSaved = DateTime.Now;
                        SalesRoomParties.savedByUserID = session.UserID;
                        db.tblSalesRoomsParties.AddObject(SalesRoomParties);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Sales Room Saved";
                        response.ObjectID = SalesRoomParties.salesRoomPartyID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Sales Room NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }

            public AttemptResponse DeleteParties(int TargetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                try
                {
                    var query = db.tblSalesRoomsParties.Single(m => m.salesRoomPartyID == TargetID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Sales Room Deleted";
                    response.ObjectID = TargetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Sales Room NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }

            public PartiesSales.GetSalesRoomsParties FindSalesRooms(int PartiesInfo_SalesPartyID)
            {
                ePlatEntities db = new ePlatEntities();
                var query = from sroom in db.tblSalesRoomsParties
                            where sroom.salesRoomPartyID == PartiesInfo_SalesPartyID
                            select new
                            {
                                SalesRoomPartiesID = sroom.salesRoomPartyID,
                                SalesRoomPlaceID = sroom.placeID,
                                SalesRoomID = sroom.salesRoomID,
                                SalesRoomDate = sroom.partyDateTime,
                                SalesRoomAllotment = sroom.allotment,
                                SalesRoomProgramID = sroom.programID,
                                SalesRoomTerminalID = sroom.terminalID,
                                SalesRoomDateSaved = sroom.dateSaved,
                                SalesRoomSavedByUserID = sroom.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + sroom.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName
                            };
                PartiesSales.GetSalesRoomsParties SalesRoom = new PartiesSales.GetSalesRoomsParties()
                {
                    SalesRoomsPartiesID = query.FirstOrDefault().SalesRoomPartiesID,
                    Place = query.FirstOrDefault().SalesRoomPlaceID.ToString(),
                    Allotment = query.FirstOrDefault().SalesRoomAllotment.ToString(),
                    SalesRoom = query.FirstOrDefault().SalesRoomID.ToString(),
                    Date = query.FirstOrDefault().SalesRoomDate.ToString("yyyy-MM-dd hh:mm tt"),
                    Program = query.FirstOrDefault().SalesRoomProgramID.ToString(),
                    DateSaved = query.FirstOrDefault().SalesRoomDateSaved.ToString("yyyy-MM-dd hh:mm tt"),
                    SavedByUser = query.FirstOrDefault().SalesRoomSavedByUserID.ToString(),
                    Terminal = query.FirstOrDefault().SalesRoomTerminalID.ToString()
                };
                return SalesRoom;
            }
            public AttemptResponse SalesRoomsDuplicate(string DuplicateFromDate, string DuplicateToDate, string TerminalID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                DateTime TakeFromDate = DateTime.Parse(DuplicateFromDate);
                DateTime CopyToDate = DateTime.Parse(DuplicateToDate);
                DateTime DayX = TakeFromDate.AddDays(1);
                var n = TakeFromDate += TakeFromDate.TimeOfDay;
                long terminal = long.Parse(TerminalID);
                var query = from x in db.tblSalesRoomsParties
                            where terminal == x.terminalID && (n <= x.partyDateTime && DayX >= x.partyDateTime)
                            select x;
                if (query.Count() > 0)
                {
                    try
                    {
                        foreach (var Save in query)
                        {
                            var SalesRooms = new tblSalesRoomsParties();
                            var NewDateTime = Save.partyDateTime.TimeOfDay;
                            SalesRooms.placeID = Save.placeID;
                            SalesRooms.salesRoomID = Save.salesRoomID;
                            SalesRooms.partyDateTime = CopyToDate + NewDateTime;
                            SalesRooms.allotment = Save.allotment;
                            SalesRooms.programID = Save.programID;
                            SalesRooms.terminalID = Save.terminalID;
                            SalesRooms.dateSaved = DateTime.Now;
                            SalesRooms.savedByUserID = session.UserID;
                            db.tblSalesRoomsParties.AddObject(SalesRooms);
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Sales Room Duplicate Success";
                        response.ObjectID = 1;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Sales Rooms Parties Not Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                response.Type = Attempt_ResponseTypes.Warning;
                response.Message = "The date or terminal request it doesn't contain Sales Room Parties";
                response.ObjectID = -1;
                return response;
            }//prueba guardado-falta, validacion y ventana flotante

            //Listas
            public static List<SelectListItem> PlacesList()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                //Places-terminals                
                var placesTerminals = (from tp in db.tblPlaces_Terminals
                                       where tp.tblPlaces.vg == true
                                        && tp.tblPlaces.placeTypeID == 1
                                        && terminals.Contains(tp.terminalID)
                                       select new
                                       {
                                           placeID = tp.tblPlaces.placeID,
                                           placeName = tp.tblPlaces.place
                                       }).OrderBy(x => x.placeName);

                foreach (var place in placesTerminals)
                {
                    list.Add(new SelectListItem
                    {
                        Text = place.placeName,
                        Value = place.placeID.ToString()
                    });
                }
                return list;
            }

            public static List<SelectListItem> RoomsList(string rooms)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var roomID = int.Parse(rooms);
                var query = (from room in db.tblSalesRooms
                             where room.placeID == roomID
                             select room).OrderBy(x => x.salesRoom);

                foreach (var room in query)
                {
                    list.Add(new SelectListItem
                    {
                        Text = room.salesRoom,
                        Value = room.salesRoomID.ToString()
                    });
                }
                return list;
            }

            public static List<SelectListItem> SearchProgramList()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var query = (from Program in db.tblProspectationPrograms
                             join terminal in db.tblTerminals on Program.companiesGroupID equals terminal.companiesGroupID
                             where terminals.Contains(terminal.terminalID)
                             select Program).OrderBy(x => x.program);

                foreach (var item in query)
                {
                    list.Add(new SelectListItem
                    {
                        Text = item.program,
                        Value = item.programID.ToString()
                    });
                }
                return list;
            }

            public static List<SelectListItem> ProgramList(string ItemID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                var TerminalID = int.Parse(ItemID);

                var ProgramByGroup = db.tblTerminals.Where(n => TerminalID == n.terminalID).Select(n => n.companiesGroupID);
                var query = (from x in db.tblProspectationPrograms
                             where ProgramByGroup.Contains(x.companiesGroupID)
                             select x).OrderBy(x => x.program);

                foreach (var item in query)
                {
                    list.Add(new SelectListItem
                    {
                        Text = item.program,
                        Value = item.programID.ToString()
                    });
                }
                return list;
            }
        }

        public class Options
        {
            public List<OptionsModel.OptionsInfomodel> SearchOptions(OptionsModel.OptionsSearchModel model)
            {
                ePlatEntities db = new ePlatEntities();
                List<OptionsModel.OptionsInfomodel> list = new List<OptionsModel.OptionsInfomodel>();
                
                var terminals = session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();
                var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
                var places = model.OptionsSearch_Place ?? PlaceDataModel.GetResortsByProfile().Select(m => long.Parse(m.Value)).ToArray();
                var types = model.OptionsSearch_OptionType ?? MasterChartDataModel.LeadsCatalogs.FillDrpOptionCategories().Select(m => (int?)int.Parse(m.Value)).ToArray();
                var query = from option in db.tblOptions
                            join p in db.tblOptions_Places on option.optionID equals p.optionID into option_p
                            from p in option_p.DefaultIfEmpty()
                            where (terminals.Contains(option.tblOptionTypes.terminalID) || isAdmin)
                            && places.Contains(p.placeID)
                            && (model.OptionsSearch_OptionName == null || option.optionName.Contains(model.OptionsSearch_OptionName))
                            && (types.Contains(option.optionTypeID) || isAdmin)
                            //&& option.terminateDate == null
                            select option;

                foreach (var i in query.Distinct())
                {
                    var _places = string.Join(", ", i.tblOptions_Places.Select(m => new { place = m.tblPlaces.place + " " + m.tblPlaces.tblDestinations.destination }).Select(m => m.place));
                    list.Add(new OptionsModel.OptionsInfomodel()
                    {
                        OptionsInfo_OptionID = i.optionID,
                        OptionsInfo_OptionTypeText = i.tblOptionTypes.optionType + (i.tblOptionTypes.terminalID != null ? " - " + i.tblOptionTypes.tblTerminals.prefix : ""),
                        OptionsInfo_OptionName = i.optionName,
                        OptionsInfo_PlaceText = _places,
                        OptionsInfo_GoldCardPrice = i.goldCardPrice ?? 0,
                        OptionsInfo_ResortCredit = i.resortCredit ?? 0,
                        OptionsInfo_Active = i.terminateDate == null ? true : i.terminateDate > DateTime.Now
                    });
                }

                return list;
            }

            public OptionsModel.OptionsInfomodel GetOption(int OptionsInfo_OptionID)
            {
                ePlatEntities db = new ePlatEntities();
                OptionsModel.OptionsInfomodel model = new OptionsModel.OptionsInfomodel();

                var query = db.tblOptions.Single(m => m.optionID == OptionsInfo_OptionID);
                model.OptionsInfo_OptionID = query.optionID;
                model.OptionsInfo_OptionType = query.optionTypeID;
                model.OptionsInfo_OptionName = query.optionName;
                model.OptionsInfo_OptionDescription = query.optionDescription;
                model.OptionsInfo_Place = query.tblOptions_Places.Select(m => m.placeID).ToArray();
                model.OptionsInfo_GoldCardPrice = query.goldCardPrice ?? 0;
                model.OptionsInfo_ResortCredit = query.resortCredit ?? 0;
                return model;
            }

            public AttemptResponse SaveOption(OptionsModel.OptionsInfomodel model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                if (model.OptionsInfo_OptionID != 0)
                {
                    try
                    {
                        var query = new tblOptions();
                        if (db.tblOptionsSold.Where(m => m.optionID == model.OptionsInfo_OptionID).Count() == 0)
                        {
                            query = db.tblOptions.Single(m => m.optionID == model.OptionsInfo_OptionID);
                            query.optionName = model.OptionsInfo_OptionName;
                            query.optionDescription = model.OptionsInfo_OptionDescription;
                            query.optionTypeID = model.OptionsInfo_OptionType;
                            query.goldCardPrice = model.OptionsInfo_GoldCardPrice;
                            query.resortCredit = model.OptionsInfo_ResortCredit;
                            var _savedPlaces = query.tblOptions_Places.ToArray();

                            model.OptionsInfo_Place = model.OptionsInfo_Place ?? PlaceDataModel.GetResortsByProfile().Select(m => long.Parse(m.Value)).ToArray();

                            foreach (var i in _savedPlaces)
                            {
                                if (model.OptionsInfo_Place.Where(m => m == i.placeID).Count() > 0)
                                {
                                    model.OptionsInfo_Place = model.OptionsInfo_Place.Where(m => m != i.placeID).ToArray();
                                }
                                else
                                {
                                    db.DeleteObject(i);
                                }
                            }

                            if (model.OptionsInfo_Place.Count() > 0)
                            {
                                foreach (var i in model.OptionsInfo_Place)
                                {
                                    var _place = new tblOptions_Places();
                                    _place.placeID = i;
                                    query.tblOptions_Places.Add(_place);
                                }
                            }
                            response.Message = "Option Updated";
                        }
                        else
                        {
                            var _query = db.tblOptions.Single(m => m.optionID == model.OptionsInfo_OptionID);
                            _query.terminateDate = DateTime.Now;
                            query.optionName = model.OptionsInfo_OptionName;
                            query.optionDescription = model.OptionsInfo_OptionDescription;
                            query.optionTypeID = model.OptionsInfo_OptionType;
                            query.goldCardPrice = model.OptionsInfo_GoldCardPrice;
                            query.resortCredit = model.OptionsInfo_ResortCredit;

                            if (model.OptionsInfo_Place != null)
                            {
                                foreach (var i in model.OptionsInfo_Place)
                                {
                                    var _place = new tblOptions_Places();
                                    _place.placeID = i;
                                    query.tblOptions_Places.Add(_place);
                                }
                            }
                            db.tblOptions.AddObject(query);
                            response.Message = "Option Closed and Saved as New because there are sales related.";
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.ObjectID = query.optionID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Option NOT Saved/Updated";
                        response.ObjectID = 0;
                        response.Message = ex.Message;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblOptions();
                        query.optionName = model.OptionsInfo_OptionName;
                        query.optionDescription = model.OptionsInfo_OptionDescription;
                        query.optionTypeID = model.OptionsInfo_OptionType;
                        query.goldCardPrice = model.OptionsInfo_GoldCardPrice;
                        query.resortCredit = model.OptionsInfo_ResortCredit;
                        model.OptionsInfo_Place = model.OptionsInfo_Place ?? PlaceDataModel.GetResortsByProfile().Select(m => long.Parse(m.Value)).ToArray();

                        foreach (var i in model.OptionsInfo_Place)
                        {
                            var _place = new tblOptions_Places();
                            _place.placeID = i;
                            query.tblOptions_Places.Add(_place);
                        }

                        db.tblOptions.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Option Saved";
                        response.ObjectID = query.optionID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Option NOT Saved";
                        response.ObjectID = 0;
                        response.Message = ex.Message;
                        return response;
                    }
                }
            }

            public AttemptResponse DeleteOption(int targetID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();

                try
                {
                    var query = db.tblOptions.Single(m => m.optionID == targetID);
                    query.terminateDate = DateTime.Now;

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Option Deleted";
                    response.ObjectID = targetID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Option NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public class MarketCodesModel
        {
            public static List<SelectListItem> FillFrontPlaces()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var query = db.tblPlaces_Terminals.Where(m => terminals.Contains(m.terminalID)).Select(m => new { m.tblPlaces.frontOfficeResortID, m.tblPlaces.place, m.terminalID, m.tblPlaces.tblDestinations.destination });

                foreach (var i in query.Where(m => m.frontOfficeResortID != null))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.frontOfficeResortID.ToString(),
                        Text = i.place + " " + i.destination
                    });
                }

                return list;
            }
            public static List<SelectListItem> FillDrpPrograms()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                var query = db.tblProspectationPrograms;

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.programID.ToString(),
                        Text = i.program
                    });
                }

                return list;
            }
            public static DependantFields GetProgramsAndSources()
            {
                ePlatEntities db = new ePlatEntities();
                DependantFields df = new DependantFields();
                df.Fields = new List<DependantFields.DependantField>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var companiesGroups = db.tblTerminals.Where(m => terminals.Contains(m.terminalID) && m.companiesGroupID != null).Select(m => m.companiesGroupID).ToArray();

                DependantFields.DependantField Program = new DependantFields.DependantField();
                Program.Field = "MarketCodeInfo_Program";
                Program.ParentField = "MarketCodeInfo_Terminal";
                Program.Values = new List<DependantFields.FieldValue>();

                var queryPrograms = db.tblProspectationPrograms.Where(m => companiesGroups.Contains(m.companiesGroupID));

                foreach (var i in queryPrograms)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = i.tblCompaniesGroups.tblTerminals.FirstOrDefault().terminalID;
                    val.Value = i.programID.ToString();
                    val.Text = i.program;
                    Program.Values.Add(val);
                }

                var defaultProgram = new DependantFields.FieldValue();
                defaultProgram.ParentValue = null;
                defaultProgram.Value = "";
                defaultProgram.Text = "--Select One--";
                Program.Values.Insert(0, defaultProgram);

                df.Fields.Add(Program);

                DependantFields.DependantField TravelSource = new DependantFields.DependantField();
                TravelSource.Field = "MarketCodeInfo_TravelSource";
                TravelSource.ParentField = "MarketCodeInfo_Program";
                TravelSource.Values = new List<DependantFields.FieldValue>();

                var querySources = db.tblArrivalsTravelSources;

                foreach (var i in querySources)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = i.programID;
                    val.Value = i.arrivalsTravelSourceID.ToString();
                    val.Text = i.arrivalsTravelSource;
                    TravelSource.Values.Add(val);
                }

                var defaultSource = new DependantFields.FieldValue();
                defaultSource.ParentValue = null;
                defaultSource.Value = "";
                defaultSource.Text = "--Select One--";
                TravelSource.Values.Insert(0, defaultSource);

                df.Fields.Add(TravelSource);

                return df;
            }

            public List<MarketCodes.MarketCodesResults> SearchMarketCodes(MarketCodes.SearchMarketCodes model)
            {
                ePlatEntities db = new ePlatEntities();
                List<MarketCodes.MarketCodesResults> list = new List<MarketCodes.MarketCodesResults>();

                var places = model.MarketCodeSearch_Places != null ? model.MarketCodeSearch_Places : model.MarketCodeSearch_DrpPlaces.Select(m => (int?)int.Parse(m.Value)).ToArray();
                var leadSources = model.MarketCodeSearch_LeadSources != null ? model.MarketCodeSearch_LeadSources.Select(m => (long?)long.Parse(m)).ToArray() : new long?[] { };
                //var leadSources = model.MarketCodeSearch_LeadSources != null ? model.MarketCodeSearch_LeadSources.Select(m => (long?)long.Parse(m)).ToArray() : model.MarketCodeSearch_DrpLeadSources.Select(m => (long?)long.Parse(m.Value)).ToArray().SetValue(null,0);

                var query = from mc in db.tblMarketCodes
                            join place in db.tblPlaces on mc.frontOfficeResortID equals place.frontOfficeResortID into mc_place
                            from place in mc_place.DefaultIfEmpty()
                            where (model.MarketCodeSearch_MarketCode == null || mc.marketCode.Contains(model.MarketCodeSearch_MarketCode))
                            && places.Contains(mc.frontOfficeResortID)
                            //&& 
                            //&& (leadSources.Count() == 0 || (leadSources.Count(m => m.Value == 0) > 0 && leadSources.Count() > 1 && (leadSources.Contains(mc.leadSourceID) || mc.leadSourceID == null)) || (leadSources.Count(m => m.Value == 0) > 0 && leadSources.Count() == 1 && mc.leadSourceID == null) || leadSources.Contains(mc.leadSourceID))
                            select mc;

                
                
                foreach (var i in query)
                {                    
                    if (list.Count(m => m.MarketCodeInfo_MarketCodeID == i.marketCodeID) == 0)
                    {
                        list.Add(new MarketCodes.MarketCodesResults()
                        {
                            MarketCodeInfo_MarketCodeID = i.marketCodeID,
                            MarketCodeInfo_MarketCode = i.marketCode,
                            MarketCodeInfo_Resort = model.MarketCodeSearch_DrpPlaces.Single(m => (int?)int.Parse(m.Value) == i.frontOfficeResortID).Text,
                            MarketCodeInfo_LeadSource = i.leadSourceID != null ? model.MarketCodeSearch_DrpLeadSources.Count(m => (long?)long.Parse(m.Value) == i.leadSourceID) > 0 ? model.MarketCodeSearch_DrpLeadSources.Single(m => (long?)long.Parse(m.Value) == i.leadSourceID).Text : db.tblLeadSources.FirstOrDefault(m => m.leadSourceID == i.leadSourceID).leadSource : "Not Set"
                        });
                    }
                }

                return list.Distinct().ToList();
            }

            public List<MarketCodes.MarketCodesResults> _SearchMarketCodes(MarketCodes.SearchMarketCodes model)
            {
                ePlatEntities db = new ePlatEntities();
                List<MarketCodes.MarketCodesResults> list = new List<MarketCodes.MarketCodesResults>();

                var places = model.MarketCodeSearch_Places != null ? model.MarketCodeSearch_Places : model.MarketCodeSearch_DrpPlaces.Select(m => (int?)int.Parse(m.Value)).ToArray();
                var users = model.MarketCodeSearch_Users != null ? model.MarketCodeSearch_Users : new Guid?[] { };
                var leadSources = model.MarketCodeSearch_LeadSources != null ? model.MarketCodeSearch_LeadSources.Select(m => (long?)long.Parse(m)).ToArray() : new long?[] { };

                var a = users.Count(m => m.Value == Guid.Empty) > 0;
                var b = users.Count(m => m.Value == Guid.Empty) == 0;
                var c = users.Count();

                var query = from mk in db.tblMarketCodes
                            join mku in db.tblMarketCodes_Users on mk.marketCodeID equals mku.marketCodeID into mk_mku
                            from mku in mk_mku.DefaultIfEmpty()
                            join place in db.tblPlaces on mk.frontOfficeResortID equals place.frontOfficeResortID into mk_place
                            from place in mk_place.DefaultIfEmpty()
                            where (model.MarketCodeSearch_MarketCode == null || mk.marketCode.Contains(model.MarketCodeSearch_MarketCode))
                            && (places.Count() == 0 || places.Contains(place.frontOfficeResortID))
                            && ((leadSources.Count() == 0 && mk.leadSourceID == null) || leadSources.Contains(mk.leadSourceID))
                            && ((users.Count(m => m.Value == Guid.Empty) > 0 && ((users.Count() > 1 && users.Contains(mku.userID)) | mku == null))
                            || (users.Count(m => m.Value == Guid.Empty) == 0 && ((users.Count() == 0 && mku == null) || users.Contains(mku.userID))))
                            select new
                            {
                                check = users.Count() != 0,
                                mk.marketCodeID,
                                mk.frontOfficeResortID,
                                mk.marketCode,
                                mk.tblMarketCodes_Users,
                                mk.tblLeadSources.leadSource
                            };

                foreach (var i in query)
                {
                    var ids = i.tblMarketCodes_Users.Count() > 0 ? i.tblMarketCodes_Users.Select(m => m.userID).ToArray() : null;
                    var names = ids != null ? db.tblUserProfiles.Where(m => ids.Contains(m.userID)).Select(m => new { name = m.firstName + " " + m.lastName }).Select(m => m.name) : null;
                    var usernames = names != null ? string.Join(",", names) : "";
                    var place = db.tblPlaces.FirstOrDefault(m => m.frontOfficeResortID == i.frontOfficeResortID).place;

                    if (list.Count(m => m.MarketCodeInfo_MarketCodeID == i.marketCodeID) == 0)
                    {
                        list.Add(new MarketCodes.MarketCodesResults()
                        {
                            MarketCodeInfo_Checked = i.check,
                            MarketCodeInfo_MarketCodeID = i.marketCodeID,
                            MarketCodeInfo_MarketCode = i.marketCode,
                            MarketCodeInfo_UsersText = usernames,
                            MarketCodeInfo_Resort = place,
                            MarketCodeInfo_LeadSource = i.leadSource
                        });
                    }
                }

                return list.Distinct().ToList();
            }

            public MarketCodes.MarketCodeInfo GetMarketCodeInfo(int marketCodeID)
            {
                ePlatEntities db = new ePlatEntities();
                var model = new MarketCodes.MarketCodeInfo();

                var query = db.tblMarketCodes.Single(m => m.marketCodeID == marketCodeID);

                model.MarketCodeInfo_MarketCodeID = query.marketCodeID;
                model.MarketCodeInfo_MarketCode = query.marketCode;
                model.MarketCodeInfo_LeadSource = query.leadSourceID ?? 0;
                model.MarketCodeInfo_Place = query.frontOfficeResortID;
                //model.MarketCodeInfo_Users = query.tblMarketCodes_Users.Count() > 0 ? query.tblMarketCodes_Users.Select(m => (Guid?)m.userID).ToArray() : null;
                //model.MarketCodeInfo_Terminal = query.tblMarketCodes_Users.Count() > 0 ? query.tblMarketCodes_Users.FirstOrDefault().terminalID : 0;
                return model;
            }

            public AttemptResponse AssignMarketCodes(MarketCodes.MarketCodeInfo model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var marketCodes = model.MarketCodeInfo_MarketCodes.Split(',').Select(m => int.Parse(m)).ToArray();

                    foreach(var i in marketCodes)
                    {
                        var mc = db.tblMarketCodes.Single(m => m.marketCodeID == i);
                        mc.leadSourceID = model.MarketCodeInfo_AssignTo;
                    }

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Market Codes Succesfully Assigned";
                    response.ObjectID = 0;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Market Codes NOT Assigned";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }

            }

            public AttemptResponse SaveMarketCode(MarketCodes.MarketCodeInfo model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                
                if (model.MarketCodeInfo_MarketCodeID != 0)
                {
                    #region "update"
                    try
                    {
                        var query = db.tblMarketCodes.Single(m => m.marketCodeID == model.MarketCodeInfo_MarketCodeID);
                        if (db.tblMarketCodes.Count(m => m.marketCode == model.MarketCodeInfo_MarketCode && m.frontOfficeResortID == model.MarketCodeInfo_Place && m.marketCodeID != model.MarketCodeInfo_MarketCodeID) == 0)
                        {
                            query.marketCode = model.MarketCodeInfo_MarketCode;
                            query.frontOfficeResortID = model.MarketCodeInfo_Place;
                            query.leadSourceID = model.MarketCodeInfo_LeadSource;

                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Market Code Updated";
                            response.ObjectID = query.marketCodeID;
                            return response;
                        }
                        else
                        {
                            response.Type = Attempt_ResponseTypes.Warning;
                            response.Message = "A Market Code With Same Name Already Exist";
                            response.ObjectID = query.marketCodeID;
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Market Code NOT Updated";
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
                        var query = new tblMarketCodes();
                        if (db.tblMarketCodes.Count(m => m.frontOfficeResortID == model.MarketCodeInfo_Place && m.marketCode.ToLower() == model.MarketCodeInfo_MarketCode.ToLower()) == 0)
                        {
                            query.marketCode = model.MarketCodeInfo_MarketCode;
                            query.frontOfficeResortID = model.MarketCodeInfo_Place;
                            query.leadSourceID = model.MarketCodeInfo_LeadSource;

                            db.tblMarketCodes.AddObject(query);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Market Code Saved";
                            response.ObjectID = query.marketCodeID;
                            return response;
                        }
                        else
                        {
                            response.Type = Attempt_ResponseTypes.Warning;
                            response.Message = "Market Code Already Exist";
                            response.ObjectID = 0;
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Market Code NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
            }
        }

        //Banks
        public class BanksModel
        {
            public List<Banks.BankItem> SearchBanks(Banks.SearchBanks model)
            {
                ePlatEntities db = new ePlatEntities();
                List<Banks.BankItem> SearchBanks = new List<Banks.BankItem>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var currentTerminals = model.SearchTerminal == null ? terminals : model.SearchTerminal.Select(m => long.Parse(m)).ToArray();

                SearchBanks = (from x in db.tblBanks
                               join t in db.tblTerminals on x.terminalID equals t.terminalID
                               where
                                  (
                                  (model.SearchFromDate == null && model.SearchToDate == null)
                                  || (model.SearchFromDate == x.fromDate && model.SearchToDate == x.fromDate)
                                  || (x.fromDate >= model.SearchFromDate && x.fromDate <= model.SearchToDate)
                                  || (x.fromDate >= model.SearchFromDate && model.SearchToDate == null)
                                  || (model.SearchFromDate == null && x.fromDate < model.SearchToDate)
                                  )
                                  && (model.SearchBank == x.bank || model.SearchBank == null)
                                  && (currentTerminals.Contains(x.terminalID))
                               select new Banks.BankItem()
                               {
                                   BankID = x.bankID,
                                   BankName = x.bank,
                                   CveSat = x.cveSat,
                                   FromDate = x.fromDate,
                                   ToDate = x.toDate,
                                   Terminal = t.terminal,
                                   TerminalID = t.terminalID
                               }).ToList();

                return SearchBanks;
            }
            public AttemptResponse SaveBank(Banks.BankItem model)
            {
                AttemptResponse response = new AttemptResponse();
                ePlatEntities db = new ePlatEntities();
                if (model.BankID == null)
                {
                    try
                    {
                        tblBanks NewBank = new tblBanks();
                        NewBank.bank = model.BankName;
                        NewBank.cveSat = model.CveSat;
                        NewBank.fromDate = model.FromDate;
                        NewBank.toDate = model.ToDate;
                        NewBank.terminalID = model.TerminalID;
                        db.tblBanks.AddObject(NewBank);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Bank successfully saved";
                        response.ObjectID = NewBank.bankID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error, Bank not saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                else
                {
                    try
                    {
                        var NewBank = db.tblBanks.Single(x => x.bankID == model.BankID);
                        NewBank.bank = model.BankName;
                        NewBank.cveSat = model.CveSat;
                        NewBank.fromDate = model.FromDate;
                        NewBank.toDate = model.ToDate;
                        NewBank.terminalID = model.TerminalID;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Bank Updated";
                        response.ObjectID = NewBank.bankID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error Bank not updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                return response;
            }
            public AttemptResponse DeleteBank(int BankID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblBanks.Single(x => x.bankID == BankID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Bank Deleted";
                    response.ObjectID = BankID;
                    return response;

                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Bank NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }

            //public Banks.GetBanks GetBanks(int BankID)
            //{
            //    ePlatEntities db = new ePlatEntities();
            //    Banks.GetBanks model = new Banks.GetBanks();
            //    var query = db.tblBanks.Single(x => x.bankID == BankID);
            //    model.BankID = query.bankID;
            //    model.Bank = query.bank;
            //    model.CveSat = query.cveSat;
            //    model.FromDate = query.fromDate == null? "" :query.fromDate.Value.ToString("yyyy-MM-dd");
            //    model.toDate = query.toDate == null? "": query.toDate.Value.ToString("yyyy-MM-dd");//yyyy-MM-dd HH:mm:ss 
            //    model.TerminalID = query.terminalID;
            //    return model;

            //}

        }
        //Boats
        public class BoatsModel
        {
            public List<Boats.BoatSearchItems> SearchBoats(Boats.SearchBoats model)
            {
                ePlatEntities db = new ePlatEntities();
                List<Boats.BoatSearchItems> SearchBoats = new List<Boats.BoatSearchItems>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var currentTerminals = model.Terminal == null ? terminals : model.Terminal.Split(',').Select(m => long.Parse(m)).ToArray();

                SearchBoats = (from x in db.tblBoats
                               join t in db.tblTerminals on x.terminalID equals t.terminalID
                               join p in db.tblProviders on x.providersID equals p.providerID
                               where (model.SearchBoat == null || model.SearchBoat == x.boat)
                               && (model.Alias == null || model.Alias == x.shortName)
                               && (currentTerminals.Contains(x.terminalID))

                               select new Boats.BoatSearchItems()
                               {
                                   BoatSearchItemID = x.boatID,
                                   BoatSearchItemName = x.boat,
                                   BoatSearchItemQuota = x.quota,
                                   BoatSearchItemAlias = x.shortName,
                                   BoatSearchItemTerminal = t.terminal,
                                   BoatSearchItemProviders = p.comercialName
                               }).ToList();
                return SearchBoats;
            }
            public AttemptResponse SaveBoat(Boats.NewBoat model)
            {
                AttemptResponse response = new AttemptResponse();
                ePlatEntities db = new ePlatEntities();

                if (model.BoatID < 1)
                {
                    try
                    {
                        tblBoats NewBoat = new tblBoats();
                        NewBoat.boat = model.Boat;
                        NewBoat.quota = model.Qouta;
                        NewBoat.shortName = model.Shortname;
                        NewBoat.dateSaved = DateTime.Now;
                        NewBoat.savedByUser = session.UserID;
                        NewBoat.providersID = model.ProvidersID;
                        NewBoat.terminalID = model.TerminalID;
                        db.tblBoats.AddObject(NewBoat);
                        //db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Boat save success";
                        response.ObjectID = NewBoat.boatID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error Boat not saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                else
                {
                    try
                    {
                        var NewBoat = db.tblBoats.Single(x => x.boatID == model.BoatID);
                        NewBoat.boat = model.Boat;
                        NewBoat.quota = model.Qouta;
                        NewBoat.shortName = model.Shortname;
                        NewBoat.providersID = model.ProvidersID;
                        NewBoat.terminalID = model.TerminalID;
                        //db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Boat Updated";
                        response.ObjectID = NewBoat.boatID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error Boat not Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                db.SaveChanges();
                return response;
            }

            public AttemptResponse DeleteBoat(int BoatID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblBoats.Single(x => x.boatID == BoatID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Boat Deleted";
                    response.ObjectID = BoatID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Bank NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            public Boats.GetBoats GetBoats(int BoatID)
            {
                ePlatEntities db = new ePlatEntities();
                Boats.GetBoats model = new Boats.GetBoats();
                var query = db.tblBoats.Single(x => x.boatID == BoatID);
                model.BoatID = query.boatID;
                model.Boat = query.boat;
                model.Qouta = query.quota;
                model.Alias = query.shortName;
                model.Terminals = query.terminalID;
                model.Providers = query.providersID;
                return model;
            }
        }
        //Sales Channels
        public class SalesChannelsModel
        {
            public List<SalesChanels.SaleChannelSearchItems> SearchSalesChannels(SalesChanels.SearchSaleChannel model)
            {
                ePlatEntities db = new ePlatEntities();
                List<SalesChanels.SaleChannelSearchItems> SearchSalesChannels = new List<SalesChanels.SaleChannelSearchItems>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var currentTerminals = model.SearchTerminal == null ? terminals : model.SearchTerminal.Split(',').Select(m => long.Parse(m)).ToArray();

                SearchSalesChannels = (from x in db.tblSalesChannels
                                       join t in db.tblTerminals on x.terminalID equals t.terminalID
                                       join u in db.aspnet_Users on x.savedByUserID equals u.UserId
                                       where (model.Search_SaleChannel == x.salesChannel || model.Search_SaleChannel == null)
                                       && (currentTerminals.Contains(x.terminalID))
                                       select new SalesChanels.SaleChannelSearchItems()
                                       {
                                           SaleChannelSearchItemID = x.salesChannelID,
                                           SaleChannelSearchItemName = x.salesChannel,
                                           SaleChannelSearchItemUser = u.UserName,
                                           SaleChannelSearchItemDate = x.dateSaved,
                                           SaleChannelSearchItemTerminal = t.terminal
                                       }).ToList();
                return SearchSalesChannels;
            }
            public AttemptResponse SaveSaleChannel(SalesChanels.NewSalesChannel model)
            {
                AttemptResponse response = new AttemptResponse();
                ePlatEntities db = new ePlatEntities();
                if (model.SalesChannnelID == 0)
                {
                    try
                    {
                        tblSalesChannels NewSaleChannel = new tblSalesChannels();
                        NewSaleChannel.salesChannel = model.SalesChannel;
                        NewSaleChannel.savedByUserID = session.UserID;
                        NewSaleChannel.dateSaved = DateTime.Now;
                        NewSaleChannel.terminalID = model.TerminalID;
                        db.tblSalesChannels.AddObject(NewSaleChannel);
                        //db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Sale Channel save success";
                        response.ObjectID = NewSaleChannel.salesChannelID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error Sale Channel not saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                else
                {
                    try
                    {
                        var NewSaleChannel = db.tblSalesChannels.Single(x => x.salesChannelID == model.SalesChannnelID);
                        NewSaleChannel.salesChannel = model.SalesChannel;
                        NewSaleChannel.terminalID = model.TerminalID;
                        //db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Sale Channel Updated";
                        response.ObjectID = NewSaleChannel.salesChannelID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error Sale Channel not Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                db.SaveChanges();
                return response;
            }
            public AttemptResponse DeleteSaleChannel(int? SaleChannelID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblSalesChannels.Single(x => x.salesChannelID == SaleChannelID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Sale Channel Deleted";
                    response.ObjectID = SaleChannelID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Sale Channel NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            public SalesChanels.GetSalesChannels GetSalesChannels(int SaleChannelID)
            {
                ePlatEntities db = new ePlatEntities();
                SalesChanels.GetSalesChannels model = new SalesChanels.GetSalesChannels();
                var query = db.tblSalesChannels.Single(x => x.salesChannelID == SaleChannelID);
                model.SalesChannelID = query.salesChannelID;
                model.SaleChannel = query.salesChannel;
                model.UserId = query.savedByUserID;
                model.DateSaved = query.dateSaved == null ? "" : query.dateSaved.ToString("yyyy-MM-dd HH:mm:ss tt");
                model.Terminal = query.terminalID;
                return model;
            }

            public static List<SelectListItem> FillDrpSalesChannelsPerTerminals(long? TerminalID)
            {
                ePlatEntities db = new ePlatEntities();
                var terminals = TerminalID != null ? new long?[] { TerminalID } : session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();
                var list = new List<SelectListItem>();
                var query = from t in db.tblSalesChannels
                            where terminals.Contains(t.terminalID)
                            select t;
                foreach (var i in query.OrderBy(m => m.salesChannel))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.salesChannelID.ToString(),
                        Text = i.salesChannel
                    });
                }
                return list;
            }

        }
        //Bracelets
        public class BraceletsModel
        {
            public List<Bracelets.BraceletSearchItems> SearchBracelets(Bracelets.SearchBracelet model)
            {
                ePlatEntities db = new ePlatEntities();
                List<Bracelets.BraceletSearchItems> SearchBracelets = new List<Bracelets.BraceletSearchItems>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var currentTerminals = model.Terminal == null ? terminals : model.Terminal.Split(',').Select(m => long.Parse(m)).ToArray();
                SearchBracelets = (from x in db.tblBracelets
                                   join t in db.tblTerminals on x.terminalID equals t.terminalID
                                   where (model.Bracelet == x.bracelet || model.Bracelet == null)
                                   && (currentTerminals.Contains(x.terminalID.Value))
                                   select new Bracelets.BraceletSearchItems()
                                   {
                                       BraceletID = x.braceletID,
                                       Bracelet = x.bracelet,
                                       Terminal = t.terminal
                                   }).ToList();
                return SearchBracelets;
            }
            public AttemptResponse SaveBracelet(Bracelets.NewBracelet model)
            {
                AttemptResponse response = new AttemptResponse();
                ePlatEntities db = new ePlatEntities();
                if (model.BraceletID == 0)
                {
                    try
                    {
                        tblBracelets NewBracelet = new tblBracelets();
                        NewBracelet.bracelet = model.Bracelet;
                        NewBracelet.terminalID = model.TerminalID;
                        db.tblBracelets.AddObject(NewBracelet);
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Bracelet Saved";
                        response.ObjectID = NewBracelet.braceletID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error Bracelet Not Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;

                    }
                }
                else
                {
                    try
                    {
                        var NewBracelet = db.tblBracelets.Single(x => x.braceletID == model.BraceletID);
                        NewBracelet.bracelet = model.Bracelet;
                        NewBracelet.terminalID = model.TerminalID;
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Bracelet Updated";
                        response.ObjectID = NewBracelet.braceletID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error Bracelet Not Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;

                    }

                }
                db.SaveChanges();
                return response;

            }
            public AttemptResponse DeleteBracelet(int BraceletID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblBracelets.Single(x => x.braceletID == BraceletID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Bracelet Deleted";
                    response.ObjectID = BraceletID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Bracelet NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            public Bracelets.GetBracelets GetBracelets(int BraceletID)
            {
                ePlatEntities db = new ePlatEntities();
                Bracelets.GetBracelets model = new Bracelets.GetBracelets();
                var query = db.tblBracelets.Single(x => x.braceletID == BraceletID);
                model.GetBraceletID = query.braceletID;
                model.GetBracelet = query.bracelet;
                model.GetTerminalID = query.terminalID;
                return model;
            }
        }
        //Reminders
        public class ReminderModel
        {
            public List<Reminders.ReminderSearchItems> SearchReminder(Reminders.SearchReminder model)
            {
                ePlatEntities db = new ePlatEntities();
                List<Reminders.ReminderSearchItems> SearchReminders = new List<Reminders.ReminderSearchItems>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var currentTerminals = model.Terminal == null ? terminals : model.Terminal.Split(',').Select(m => long.Parse(m)).ToArray();
                SearchReminders = (from x in db.tblReminders
                                   join t in db.tblTerminals on x.terminalID equals t.terminalID
                                   join s in db.tblServices on x.serviceID equals s.serviceID
                                   where
                                   (
                                        (model.FromDate == null || model.FromDate == x.fromDate)
                                        && (model.ToDate == null || model.ToDate == x.toDate)
                                        && ((model.FromDate > x.fromDate || model.FromDate == x.fromDate))
                                        && ((model.ToDate < x.toDate || model.ToDate == x.toDate) && model.FromDate <= x.toDate)
                                   )
                                   || (currentTerminals.Contains(x.terminalID.Value))
                                   select new Reminders.ReminderSearchItems
                                   {
                                       ReminderID = x.reminderID,
                                       FromDate = x.fromDate,
                                       ToDate = x.toDate,
                                       Service = s.service,
                                       Message = x.message,
                                       Terminal = t.terminal

                                   }).ToList();
                return SearchReminders;
            }
            public AttemptResponse SaveReminder(Reminders.NewReminder model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.ReminderID == 0)
                {
                    try
                    {
                        tblReminders reminder = new tblReminders();
                        reminder.serviceID = model.ServiceID;
                        reminder.fromDate = model.FromDate;
                        reminder.toDate = model.ToDate;
                        reminder.message = model.Message;
                        reminder.savedByUserID = session.UserID;
                        reminder.dateSaved = DateTime.Now;
                        reminder.terminalID = model.TerminalID;
                        db.tblReminders.AddObject(reminder);
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Reminder Saved";
                        response.ObjectID = model.ReminderID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Reminder not saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                else
                {
                    try
                    {
                        var reminder = db.tblReminders.Single(x => x.reminderID == model.ReminderID);
                        reminder.serviceID = model.ServiceID;
                        reminder.fromDate = model.FromDate;
                        reminder.toDate = model.ToDate;
                        reminder.message = model.Message;
                        reminder.dateSaved = DateTime.Now;
                        reminder.savedByUserID = session.UserID;
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Reminder Updated";
                        response.ObjectID = model.ReminderID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Reminder NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                db.SaveChanges();
                return response;
            }
            public AttemptResponse DeleteReminder(long ReminderID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblReminders.Single(x => x.reminderID == ReminderID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Reminder Deleted";
                    response.ObjectID = ReminderID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Reminder NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            public Reminders.GetReminder GetReminders(long ReminderID)
            {
                ePlatEntities db = new ePlatEntities();
                Reminders.GetReminder model = new Reminders.GetReminder();
                var query = db.tblReminders.Single(x => x.reminderID == ReminderID);
                model.GetReminderID = query.reminderID;
                model.GetFromDate = query.fromDate == null ? "" : query.fromDate.Value.ToString("yyyy-MM-dd");
                model.GetToDate = query.toDate == null ? "" : query.toDate.Value.ToString("yyyy-MM-dd");
                model.GetMessage = query.message;
                model.GetServiceID = query.serviceID;
                model.GetTerminalID = query.terminalID == null ? 0 : query.terminalID.Value;
                return model;
            }
        }
        //Tours
        public class TourModel
        {
            //Qouta
            public List<Tours.Tour_QoutaSearchItems> SearchQouta(Tours.Tour_SearchQouta model)
            {
                ePlatEntities db = new ePlatEntities();
                List<Tours.Tour_QoutaSearchItems> SearchQouta = new List<Tours.Tour_QoutaSearchItems>();

                SearchQouta = (from x in db.tblQuotas
                               join s in db.tblServices on x.serviceID equals s.serviceID
                               join u in db.aspnet_Users on x.savedByUserID equals u.UserId
                               where
                                   (
                                        (model.SearchDate_FromDate == null || model.SearchDate_FromDate == x.fromDate)
                                        && (model.SearchDate_ToDate == null || model.SearchDate_ToDate == x.toDate)
                                        && ((model.SearchDate_FromDate > x.fromDate || model.SearchDate_FromDate == x.fromDate))
                                        && ((model.SearchDate_ToDate < x.toDate || model.SearchDate_ToDate == x.toDate) && model.SearchDate_FromDate <= x.toDate)
                                   )
                               select new Tours.Tour_QoutaSearchItems
                               {
                                   QoutaID = x.quotaID,
                                   Qouta = x.quota == null ? 0 : x.quota.Value,
                                   Note = x.notes,
                                   Tour = s.service,
                                   FromDate = x.fromDate == null ? "" : x.fromDate.Value.ToString("yyyy-MM-dd"),
                                   ToDate = x.toDate == null ? "" : x.toDate.Value.ToString("yyyy-MM-dd"),
                                   SavedByUser = u.UserName,
                                   DateSaved = x.dateSaved == null ? "" : x.dateSaved.Value.ToString("yyyy-MM-dd")
                               }).ToList();
                return SearchQouta;
            }
            public AttemptResponse SaveQouta(Tours.Tour_NewQouta model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.NewQoutaID == 0)
                {
                    try
                    {
                        tblQuotas qoutas = new tblQuotas();
                        qoutas.quota = model.NewQouta;
                        qoutas.serviceID = model.NewServiceID;
                        qoutas.fromDate = model.NewFromDate;
                        qoutas.toDate = model.NewToDate;
                        qoutas.notes = model.NewNote;
                        qoutas.permanent_ = model.permanent;
                        qoutas.dateSaved = DateTime.Now;
                        qoutas.savedByUserID = session.UserID;
                        db.tblQuotas.AddObject(qoutas);
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Quota Saved";
                        response.ObjectID = model.NewQoutaID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Quota not saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                else
                {
                    try
                    {
                        var query = db.tblQuotas.Single(x => x.quotaID == model.NewQoutaID);
                        query.quota = model.NewQouta;
                        query.fromDate = model.NewFromDate;
                        query.toDate = model.NewToDate;
                        query.notes = model.NewNote;
                        query.permanent_ = model.permanent;
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Quota Updated";
                        response.ObjectID = model.NewQoutaID;

                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Qouta NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                db.SaveChanges();
                return response;
            }
            public AttemptResponse DeleteQouta(int QoutaID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblQuotas.Single(x => x.quotaID == QoutaID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Quota Deleted";
                    response.ObjectID = QoutaID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Quota NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            public Tours.Tour_GetQouta GetQouta(int QoutaID)
            {
                ePlatEntities db = new ePlatEntities();
                Tours.Tour_GetQouta model = new Tours.Tour_GetQouta();
                var query = db.tblQuotas.Single(x => x.quotaID == QoutaID);
                model.GetQoutaID = query.quotaID;
                model.GetQouta = query.quota == null ? 0 : query.quota.Value;
                model.GetFromDate = query.fromDate == null ? "" : query.fromDate.Value.ToString("yyyy-MM-dd");
                model.GetToDate = query.toDate == null ? "" : query.toDate.Value.ToString("yyyy-MM-dd");
                model.GetNotes = query.notes;
                model.Getpermanent = query.permanent_ == null ? false : query.permanent_.Value;

                return model;

            }
        }

        //Operators
        public class OperatorModel
        {
            public List<Operators.OperatorSearchItems> SearchOperator(Operators.SearchOperator model)
            {
                ePlatEntities db = new ePlatEntities();
                List<Operators.OperatorSearchItems> SearchOperator = new List<Operators.OperatorSearchItems>();
                SearchOperator = (from x in db.tblOperators
                                  join u in db.aspnet_Users on x.savedByExecutiveUserID equals u.UserId
                                  join s in db.tblSalesChannels on x.salesChannelID equals s.salesChannelID
                                  join c in db.tblCompanies on x.companyID equals c.companyID
                                  where model.Operator == x.@operator
                                  select new Operators.OperatorSearchItems
                                  {
                                      OperatorID = x.operatorID,
                                      Operator = x.@operator,
                                      SalesChannel = s.salesChannel,
                                      SavedByUser = u.UserName,
                                      DateSaved = x.dateSaved == null ? "" : x.dateSaved.Value.ToString("yyyy-MM-dd"),
                                      Company = c.company

                                  }).ToList();
                return SearchOperator;
            }
            public AttemptResponse SaveOperator(Operators.NewOperator model)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                if (model.NewOperatorID == null)
                {
                    try
                    {
                        tblOperators Operator = new tblOperators();
                        Operator.@operator = model.NewOperatorName;
                        Operator.contactEmail = model.NewContactEmail;
                        Operator.discount = model.NewDiscount;
                        Operator.authorization = model.NewAuthorization;
                        Operator.salesChannelID = model.SalesChannelID;
                        Operator.cancelationLimit = model.CancelationLimit;
                        Operator.aditionalInfo = model.AditionalInfo;
                        Operator.maxDiscount = model.MaxDiscount;
                        Operator.savedByExecutiveUserID = session.UserID;
                        Operator.dateSaved = DateTime.Now;
                        Operator.companyID = model.CompanyID;
                        db.tblOperators.AddObject(Operator);
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Operator Saved";
                        response.ObjectID = model.NewOperatorID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Operator not saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                else
                {
                    try
                    {
                        var Operator = db.tblOperators.Single(x => x.operatorID == model.NewOperatorID);
                        Operator.@operator = model.NewOperatorName;
                        Operator.contactEmail = model.NewContactEmail;
                        Operator.discount = model.NewDiscount;
                        Operator.authorization = model.NewAuthorization;
                        Operator.maxDiscount = model.MaxDiscount;
                        Operator.savedByExecutiveUserID = session.UserID;
                        Operator.dateSaved = DateTime.Now;
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Operator Updated";
                        response.ObjectID = model.NewOperatorID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Operator NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }

                }
                db.SaveChanges();
                return response;

            }
            public AttemptResponse DeleteOperator(Guid OperatorID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                try
                {
                    var query = db.tblOperators.Single(x => x.operatorID == OperatorID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Operator Deleted";
                    response.ObjectID = OperatorID;
                    return response;

                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Operator NOT Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            public Operators.GetOperator GetOperator(Guid OperatorID)
            {
                ePlatEntities db = new ePlatEntities();
                Operators.GetOperator model = new Operators.GetOperator();
                var query = db.tblOperators.Single(x => x.operatorID == OperatorID);
                model.OperatorID = query.operatorID;
                model.Operator = query.@operator;
                model.Contact = query.contactEmail;
                model.Discount = query.discount == null ? false : query.discount.Value;
                model.Authorization = query.authorization == null ? false : query.authorization.Value;
                model.SalesChannelID = query.salesChannelID == null ? 0 : query.salesChannelID.Value;
                model.CancelationLimit = query.cancelationLimit == null ? 0 : query.cancelationLimit.Value;
                model.AditionInfo = query.aditionalInfo;
                model.MaxDiscount = query.maxDiscount == null ? 0 : query.maxDiscount.Value;
                model.CompanyID = query.companyID == null ? 0 : query.companyID.Value;
                return model;
            }

        }

        public class General
        {
            public static List<SelectListItem> FillDrpDestinations()
            {
                var list = new List<SelectListItem>();

                return list;
            }

            public static List<SelectListItem> FillDrpJobPositions()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                foreach (var i in db.tblJobPositions.OrderBy(x => x.jobPosition))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.jobPositionID.ToString(),
                        Text = i.jobPosition
                    });
                }
                return list;
            }

            public List<SelectListItem> GetDDLData(string itemType)
            {
                return GetDDLData(itemType, "");
            }

            public List<SelectListItem> GetDDLData(string itemType, string itemID)
            {
                var list = new List<SelectListItem>();
                switch (itemType)
                {
                    case "destination":
                        {
                            list = Destinations.DestinationsCatalogs.FillDrpDestinationsPerCurrentTerminals();
                            break;
                        }
                    case "placeType":
                        {
                            list = PlaceTypes.PlaceTypesCatalogs.FillDrpActivePlaceTypes();
                            break;
                        }
                    case "taCompany":
                        {
                            list = Companies.CompaniesCatalogs.FillDrpTACompanies();
                            list.Insert(0, ListItems.Default());
                            break;
                        }
                    case "company":
                        {
                            list = Companies.CompaniesCatalogs.FillDrpCompanies();
                            list.Insert(0, ListItems.Default());
                            break;
                        }
                    case "mktCompanySearch":
                        {
                            list = Companies.CompaniesCatalogs.FillDrpMarketingCompanies();
                            list = list.Where(m => m.Value != "0").ToList();
                            break;
                        }
                    case "mktCompany":
                        {
                            list = Companies.CompaniesCatalogs.FillDrpMarketingCompanies();
                            list.Insert(0, ListItems.Default());
                            break;
                        }
                    case "promotionTeam":
                        {
                            list = PromotionTeams.PromotionTeamsCatalogs.FillDrpPromotionTeams();
                            //list.Insert(0, ListItems.Default());
                            break;
                        }
                    case "opc":
                        {
                            list = ePlatBack.Models.DataModels.OpcDataModel.GetActiveOPCs();
                            list.Insert(1, new SelectListItem { Text = "None", Value = "-1" });
                            break;
                        }
                    case "pointOfSale":
                        {
                            var _itemID = itemID != null && itemID != "0" ? long.Parse(itemID) : (long?)null;
                            list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(_itemID);
                            //list = PointsOfSale.PoinsOfSaleCatalogs.FillDrpPointsOfSale();
                            break;
                        }
                    case "providersPerTerminals":
                        {
                            var _itemID = itemID != null && itemID != "0" ? long.Parse(itemID) : (long?)null;
                            //list = Providers.ProvidersCatalogs.FillDrpProvidersPerDestinations(_itemID);
                            list = Providers.ProvidersCatalogs.FillDrpProvidersPerTerminals(_itemID);
                            if (_itemID != null)
                            {
                                list.Insert(0, ListItems.Default("--All--"));
                            }
                            break;
                        }
                    case "payingCompany":
                        {
                            list = PayingCompanies.PayingCompaniesCatalogs.FillDrpPayingCompanies();
                            list.Insert(0, ListItems.Default());
                            break;
                        }
                    case "place":
                        {
                            //list = PlaceDataModel.GetResortsByProfile();
                            list = PlaceDataModel.GetPlacesByDestinationsPerTerminals();
                            list.Insert(0, ListItems.Default());
                            break;
                        }
                    case "selectedTerminals":
                        {
                            list = TerminalDataModel.GetActiveTerminalsList();
                            list.Insert(0, ListItems.Default());
                            break;
                        }
                    case "priceType":
                        {
                            list = PriceDataModel.PricesCatalogs.FillDrpPriceTypes();
                            break;
                        }
                    //room parties
                    case "SalesRoomPlaces":
                        {
                            list = CatalogsDataModel.PartiesSalesDataModel.PlacesList();
                            list.Insert(0, ListItems.Default());
                            break;
                        }
                    case "RoomsList":
                        {

                            list = CatalogsDataModel.PartiesSalesDataModel.RoomsList(itemID);
                            list.Insert(0, ListItems.Default("--Any--"));
                            break;
                        }
                    case "SalesRoomsProgramByCompany":
                        {
                            list = CatalogsDataModel.PartiesSalesDataModel.ProgramList(itemID);
                            list.Insert(0, ListItems.Default());
                            break;
                        }
                    case "SearchProgramList":
                        {
                            list = CatalogsDataModel.PartiesSalesDataModel.SearchProgramList();
                            break;
                        }
                    case "selectedTerminalsMultiple":
                        {
                            list = TerminalDataModel.GetActiveTerminalsList();
                            break;
                        }
                    case "servicesPerTerminals":
                        {
                            list = ActivityDataModel.ActivitiesCatalogs.GetServicesPerTerminalsActives();
                            list.Insert(0, ListItems.Default());
                            break;
                        }

                        //case "SelectedTerminals":
                        //    {
                        //        list = Catalogs
                        //        break;
                        //    }
                }
                return list;
            }
        }

        public class HotelPickUpsModel
        {
            public List<HotelPickUps.HotelPickUp> SearchHotelPickUps(HotelPickUps.SearchHotels model)
            {
                ePlatEntities db = new ePlatEntities();
                List<HotelPickUps.HotelPickUp> HotelsList = new List<HotelPickUps.HotelPickUp>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                long? terminalID = model.Search_TerminalID == null ? terminals[0] : model.Search_TerminalID;

                //obtener lista de hoteles de SPI
                var Hotels = SPIDataModel.GetHotels().ToList();

                //obtener datos de pickups de la terminal
                var PickUps = db.tblSPIHotelsPickUps.ToList();

                if (model.Search_HotelName != null)
                {
                    Hotels = Hotels.Where(x => x.hotel.ToLower().Contains(model.Search_HotelName.ToLower())).ToList();
                }

                foreach (var hotel in Hotels)
                {
                    var currentPickUp = (from p in PickUps
                                         where p.spiHotelID == hotel.hotel_id
                                         && p.terminalID == terminalID
                                         select p).FirstOrDefault();

                    HotelPickUps.HotelPickUp pickUp = new HotelPickUps.HotelPickUp();
                    pickUp.SpiHotelID = hotel.hotel_id;
                    pickUp.Hotel = hotel.hotel;
                    pickUp.PendingChanges = false;
                    pickUp.Destinations = new List<DependantFields.FieldValue>();
                    if (currentPickUp != null)
                    {
                        pickUp.HotelPickUpID = currentPickUp.hotelPickUpID;
                        pickUp.DestinationID = currentPickUp.destinationID;
                        pickUp.ZoneID = currentPickUp.zoneID;
                        pickUp.Picture = currentPickUp.picture;
                        pickUp.DescriptionENUS = currentPickUp.descriptionEnUS;
                        pickUp.DescriptionESMX = currentPickUp.descriptionEsMX;
                        pickUp.Lat = currentPickUp.lat;
                        pickUp.Lng = currentPickUp.lng;
                        pickUp.TerminalID = currentPickUp.terminalID;
                    }
                    HotelsList.Add(pickUp);
                }

                //filtrar con criterios de búsqueda
                if (model.Search_TerminalID != null)
                {
                    HotelsList = HotelsList.Where(x => x.TerminalID == model.Search_TerminalID).ToList();
                }

                if (model.Search_DestinationID != null)
                {
                    HotelsList = HotelsList.Where(x => x.DestinationID == model.Search_DestinationID).ToList();
                }

                if (model.Search_ZoneID != null)
                {
                    HotelsList = HotelsList.Where(x => x.ZoneID == model.Search_ZoneID).ToList();
                }

                return HotelsList;
            }

            public static DependantFields GetDependentFields()
            {
                ePlatEntities db = new ePlatEntities();
                DependantFields df = new DependantFields();
                df.Fields = new List<DependantFields.DependantField>();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                DependantFields.FieldValue valDefault = new DependantFields.FieldValue();
                valDefault.ParentValue = null;
                valDefault.Value = "";
                valDefault.Text = "Select One";

                //TerminalID > DestinationID
                DependantFields.DependantField DestinationID = new DependantFields.DependantField();
                DestinationID.Field = "Search_DestinationID";
                DestinationID.ParentField = "Search_TerminalID";
                DestinationID.Values = new List<DependantFields.FieldValue>();

                var DestinationsQ = from d in db.tblTerminals_Destinations
                                    where terminals.Contains(d.terminalID)
                                    orderby d.terminalID, d.tblDestinations.destination
                                    select new
                                    {
                                        d.terminalID,
                                        d.destinationID,
                                        d.tblDestinations.destination
                                    };

                foreach (var destination in DestinationsQ)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = destination.terminalID;
                    val.Value = destination.destinationID.ToString();
                    val.Text = destination.destination;
                    DestinationID.Values.Add(val);
                }

                DestinationID.Values.Insert(0, valDefault);
                df.Fields.Add(DestinationID);

                //DestinationID > ZoneID
                DependantFields.DependantField ZoneID = new DependantFields.DependantField();
                ZoneID.Field = "Search_ZoneID";
                ZoneID.ParentField = "Search_DestinationID";
                ZoneID.Values = new List<DependantFields.FieldValue>();

                var destinationIDs = DestinationsQ.Select(x => x.destinationID).ToList();
                var ZonesQ = from z in db.tblZones_Terminals
                             where destinationIDs.Contains(z.tblZones.destinationID)
                             && terminals.Contains(z.terminalID)
                             orderby z.terminalID, z.tblZones.destinationID, z.tblZones.zone
                             select new
                             {
                                 z.tblZones.destinationID,
                                 z.zoneID,
                                 z.tblZones.zone
                             };

                foreach (var zone in ZonesQ)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = zone.destinationID;
                    val.Value = zone.zoneID.ToString();
                    val.Text = zone.zone;
                    ZoneID.Values.Add(val);
                }
                ZoneID.Values.Insert(0, valDefault);
                df.Fields.Add(ZoneID);

                return df;
            }

            public AttemptResponse SavePickUp(HotelPickUps.HotelPickUp model)
            {
                AttemptResponse response = new AttemptResponse();
                ePlatEntities db = new ePlatEntities();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                if (model.HotelPickUpID == null)
                {
                    try
                    {
                        tblSPIHotelsPickUps NewPickUp = new tblSPIHotelsPickUps();
                        NewPickUp.hotelPickUpID = Guid.NewGuid();
                        NewPickUp.spiHotelID = model.SpiHotelID;
                        NewPickUp.destinationID = model.DestinationID;
                        NewPickUp.zoneID = model.ZoneID;
                        NewPickUp.descriptionEnUS = model.DescriptionENUS;
                        NewPickUp.descriptionEsMX = model.DescriptionESMX;
                        NewPickUp.lat = model.Lat;
                        NewPickUp.lng = model.Lng;
                        NewPickUp.savedByUserID = session.UserID;
                        NewPickUp.dateSaved = DateTime.Now;
                        NewPickUp.terminalID = terminals[0];
                        db.tblSPIHotelsPickUps.AddObject(NewPickUp);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Pick Up successfully saved";
                        response.ObjectID = NewPickUp.hotelPickUpID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error, Pick Up not saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                else
                {
                    try
                    {
                        var NewPickUp = db.tblSPIHotelsPickUps.Single(x => x.hotelPickUpID == model.HotelPickUpID);
                        NewPickUp.destinationID = model.DestinationID;
                        NewPickUp.zoneID = model.ZoneID;
                        NewPickUp.descriptionEnUS = model.DescriptionENUS;
                        NewPickUp.descriptionEsMX = model.DescriptionESMX;
                        NewPickUp.lat = model.Lat;
                        NewPickUp.lng = model.Lng;
                        NewPickUp.dateLastModification = DateTime.Now;
                        NewPickUp.modifiedByUserID = session.UserID;
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Pick Up Updated";
                        response.ObjectID = NewPickUp.hotelPickUpID;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Error Bank not updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                    }
                }
                return response;
            }


            public static PictureDataModel.FineUploaderResult UploadPickUpPicture(PictureDataModel.FineUpload upload, int SpiHotelID)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                //try
                //{
                if (upload.Exception == null)
                {
                    var firstPath = HttpContext.Current.Server.MapPath("~/");
                    var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                    var finalPath = secondPath + "ePlatBack\\Content\\files\\pickups\\";
                    var fileName = SpiHotelID + "-" + upload.Filename;

                    var fileNameDecoded = HttpContext.Current.Server.UrlDecode(fileName);
                    fileName = HttpUtility.UrlEncode(fileNameDecoded, Encoding.GetEncoding("iso-8859-8"));
                    fileName = fileName.Replace("+", "");

                    for (var i = 0; i < Regex.Matches(fileName, "%").Count; i++)
                    {
                        var encoded = fileName.Substring(fileName.IndexOf("%"), ((fileName.IndexOf("%") + 3) - fileName.IndexOf("%")));
                        var newFileName = fileName.Replace(encoded, "_");
                        fileName = newFileName;
                    }

                    var filePath = finalPath + "\\" + fileName;
                    var _filePath = "/content/files/pickups/" + fileName;
                    upload.SaveAs(filePath, false);

                    //guardar la relación
                    var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                    tblSPIHotelsPickUps hotelPickUp = new tblSPIHotelsPickUps();

                    long terminalID = terminals[0];
                    hotelPickUp = (from p in db.tblSPIHotelsPickUps
                                   where p.spiHotelID == SpiHotelID
                                   && p.terminalID == terminalID
                                   select p).FirstOrDefault();

                    if (hotelPickUp != null)
                    {
                        //ya existe
                        //si tiene foto anteriormente
                        if (hotelPickUp.picture != null)
                        {
                            //eliminar la foto anterior y 
                            var deletePath = HttpContext.Current.Server.MapPath("~" + hotelPickUp.picture);
                            var file = new FileInfo(deletePath);
                            file.Delete();
                        }

                        //sustituir el valor por la nueva
                        hotelPickUp.picture = _filePath;
                    }
                    else
                    {
                        //no existe
                        hotelPickUp = new tblSPIHotelsPickUps();
                        hotelPickUp.hotelPickUpID = Guid.NewGuid();
                        hotelPickUp.spiHotelID = SpiHotelID;
                        hotelPickUp.picture = _filePath;
                        hotelPickUp.terminalID = terminalID;
                        hotelPickUp.savedByUserID = session.UserID;
                        hotelPickUp.dateSaved = DateTime.Now;
                        db.tblSPIHotelsPickUps.AddObject(hotelPickUp);
                    }
                    db.SaveChanges();

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = new { HotelPickUpID = hotelPickUp.hotelPickUpID, SpiHotelID = hotelPickUp.spiHotelID, Picture = _filePath };
                    response.Message = "File Uploaded";
                    return new PictureDataModel.FineUploaderResult(true, new { response = response }, new { path = _filePath });
                }
                else
                {
                    throw new Exception();
                }
                //}
                //catch (Exception ex)
                //{
                //    response.Type = Attempt_ResponseTypes.Error;
                //    response.ObjectID = 0;
                //    response.Exception = upload.Exception ?? ex;
                //    response.Message = "File NOT Uploaded";
                //    return new PictureDataModel.FineUploaderResult(false, new { response = response });
                //}
            }
        }
    }
}
