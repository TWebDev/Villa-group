using System;
using System.Linq;
using System.Data;
using System.Linq.Dynamic;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Threading.Tasks;
using System.Globalization;

namespace ePlatBack.Models.DataModels
{
    public class DashboardDataModel
    {
        ePlatEntities db = new ePlatEntities();
        public static UserSession session = new UserSession();

        public static DailyCommissions GetDailyCommissions()
        {
            ePlatEntities db = new ePlatEntities();
            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);
            //DateTime today = DateTime.Parse("2019-07-24");
            //DateTime tomorrow = DateTime.Parse("2019-07-25");
            DailyCommissions dc = new DailyCommissions();
            ReportDataModel rdm = new ReportDataModel();
            IQueryable<tblExchangeRates> exchangeRates = null;
            dc.AgentsCommissions = new List<DailyCommissions.AgentCommission>();
            dc.Production = new Money()
            {
                Amount = 0,
                Currency = "MXN"
            };
            dc.ProductionNoIVA = new Money()
            {
                Amount = 0,
                Currency = "MXN"
            };
            dc.Commission = new Money()
            {
                Amount = 0,
                Currency = "MXN"
            };

            Guid userID = session.UserID;
            //Guid userID = new Guid("0386b408-27a5-40f7-8c73-e871a8163b71");
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];
            var userProfile = (from u in db.tblUserProfiles
                               where u.userID == userID
                               select u).FirstOrDefault();

            //obtener job positions
            var JobPositions = from j in db.tblUsers_JobPositions
                               where j.userID == userID
                               && j.fromDate <= DateTime.Now
                               && (j.toDate == null || j.toDate > DateTime.Now)
                               select new
                               {
                                   j.jobPositionID,
                                   j.tblJobPositions.jobPosition
                               };

            //tipos de agente
            int usersOSALength = JobPositions.Count(x => x.jobPosition.IndexOf("Online Sales Agent") >= 0);
            List<Guid?> usersOSA = new List<Guid?>();
            int usersSALength = JobPositions.Count(x => x.jobPosition.IndexOf("Sales Agent") >= 0 && x.jobPosition.IndexOf("Online") < 0);
            List<Guid?> usersSA = new List<Guid?>();
            int usersRALength = JobPositions.Count(x => x.jobPosition.IndexOf("Reservations Agent") >= 0);
            List<Guid?> usersRA = new List<Guid?>();

            if (usersOSALength > 0)
            {
                usersOSA.Add(userID);
            }
            if (usersSALength > 0)
            {
                usersSA.Add(userID);
            }
            if (usersRALength > 0)
            {
                usersRA.Add(userID);
            }

            bool isAgent = (usersOSALength + usersSALength + usersRALength > 0 ? true : false);

            //coupons
            IQueryable<tblPurchases_Services> CouponsQuery;

            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                CouponsQuery = (from coupon in db.tblPurchases_Services
                                join purchase in db.tblPurchases on coupon.purchaseID equals purchase.purchaseID
                                join service in db.tblServices on coupon.serviceID equals service.serviceID
                                join pos in db.tblPointsOfSale on purchase.pointOfSaleID equals pos.pointOfSaleID
                                join detail in db.tblPurchaseServiceDetails on coupon.purchase_ServiceID equals detail.purchase_ServiceID
                                join currency in db.tblCurrencies on purchase.currencyID equals currency.currencyID
                                where (
                               (coupon.confirmationDateTime >= today
                               && coupon.confirmationDateTime < tomorrow
                               && coupon.serviceStatusID >= 3)
                               ||
                               (coupon.confirmationDateTime < today
                               && coupon.cancelationDateTime >= today
                               && coupon.cancelationDateTime < tomorrow
                               && (coupon.serviceStatusID == 4 || coupon.serviceStatusID == 5))
                               )
                               && purchase.terminalID == terminalID
                               && purchase.isTest != true
                               && (isAgent && (usersOSA.Contains(purchase.userID) || usersOSALength == 0) || (usersSA.Contains(coupon.confirmedByUserID) || usersSALength == 0) || (usersRA.Contains(purchase.agentID) || usersRALength == 0))
                                select coupon).Distinct();

                scope.Complete();
            }


            foreach (var jp in JobPositions)
            {
                //obtener comisiones por ventas personales
                var Commissions = from c in db.tblCommissions
                                  where c.jobPositionID == jp.jobPositionID
                                  && c.terminalID == terminalID
                                  && c.isBonus == false
                                  && c.@override == false
                                  && ((c.permanent_ && c.fromDate <= tomorrow)
                                  || (c.permanent_ == false && (c.fromDate <= tomorrow && c.toDate >= today)))
                                  select new CommissionsReportClass()
                                  {
                                      jobPositionID = c.jobPositionID,
                                      terminalID = c.terminalID,
                                      priceTypeID = c.priceTypeID,
                                      PriceType = c.tblPriceTypes.priceType,
                                      commissionPercentage = c.commissionPercentage,
                                      @override = c.@override,
                                      pointOfSaleIDs = c.tblCommissions_PointsOfSale.Select(x => x.pointOfSaleID),
                                      minProfit = c.minProfit,
                                      minVolume = c.minVolume,
                                      maxProfit = c.maxProfit,
                                      maxVolume = c.maxVolume,
                                      volumeCurrencyCode = c.volumeCurrencyCode,
                                      permanent_ = c.permanent_,
                                      fromDate = c.fromDate,
                                      toDate = c.toDate
                                  };

                if (Commissions.Count() > 0)
                {
                    DailyCommissions.AgentCommission agent = new DailyCommissions.AgentCommission();
                    agent.UserID = userID;
                    agent.JobPostion = jp.jobPosition;

                    if (userProfile != null)
                    {
                        agent.Agent = userProfile.firstName + " " + userProfile.lastName;
                    }
                    agent.Production = new Money()
                    {
                        Amount = 0,
                        Currency = "MXN"
                    };
                    agent.ProductionNoIVA = new Money()
                    {
                        Amount = 0,
                        Currency = "MXN"
                    };
                    agent.Commission = new Money()
                    {
                        Amount = 0,
                        Currency = "MXN"
                    };
                    agent.Commissions = new List<DailyCommissions.CommissionsStructure>();
                    //reglas de comisiones
                    foreach (var comm in Commissions.Where(x => x.jobPositionID == jp.jobPositionID))
                    {
                        DailyCommissions.CommissionsStructure commissionType = new DailyCommissions.CommissionsStructure();
                        commissionType.PriceTypeID = comm.priceTypeID;
                        commissionType.PriceType = comm.PriceType;
                        commissionType.MinProfit = comm.minProfit;
                        commissionType.MaxProfit = comm.maxProfit;
                        commissionType.Percentage = comm.commissionPercentage;
                        commissionType.Production = new Money()
                        {
                            Amount = 0,
                            Currency = "MXN"
                        };
                        commissionType.ProductionNoIVA = new Money()
                        {
                            Amount = 0,
                            Currency = "MXN"
                        };
                        commissionType.Commission = new Money()
                        {
                            Amount = 0,
                            Currency = "MXN"
                        };
                        agent.Commissions.Add(commissionType);
                    }

                    //cupones de la posición
                    var Coupons = CouponsQuery;
                    if (jp.jobPosition.IndexOf("Online Sales Agent") >= 0)
                    {
                        Coupons = from c in Coupons
                                  where c.tblPurchases.userID == userID
                                  select c;
                    }
                    else if (jp.jobPosition.IndexOf("Sales Agent") >= 0)
                    {
                        Coupons = from c in Coupons
                                  where c.confirmedByUserID == userID
                                  select c;
                    }
                    else if (jp.jobPosition.IndexOf("Reservations Agent") >= 0)
                    {
                        Coupons = from c in Coupons
                                  where c.tblPurchases.agentID == userID
                                  select c;
                    }

                    foreach (var coupon in Coupons)
                    {
                        //coupons
                        int couponPriceTypes = coupon.tblPurchaseServiceDetails.Select(x => x.priceTypeID).Distinct().Count();

                        //comisión por unidad y tipo de precio
                        foreach (int priceTypeID in coupon.tblPurchaseServiceDetails.Select(x => x.priceTypeID).Distinct())
                        {
                            bool commissionable = true;
                            if (userID != coupon.confirmedByUserID && jp.jobPosition.IndexOf("Online Sales Agent") == -1 && jp.jobPosition.IndexOf("Reservations Agent") == -1)
                            {
                                commissionable = false;
                            }

                            if (commissionable)
                            {
                                CouponDetail couponInfo = new CouponDetail();
                                if (couponPriceTypes > 1)
                                {
                                    couponInfo = rdm.GetCouponDetail(coupon, today, tomorrow, true, priceTypeID);
                                }
                                else
                                {
                                    couponInfo = rdm.GetCouponDetail(coupon, today, tomorrow);
                                }

                                var processable = true;
                                if (couponInfo.Percentage == -1)
                                {
                                    processable = false;
                                }

                                if (processable)
                                {

                                    //Services.FirstOrDefault(c => c.ServiceID == coupon.serviceID).CommissionsPerPrice.FirstOrDefault(x => x.PriceTypeID == priceTypeID).Coupons.Add(couponInfo);

                                    tblExchangeRates exchangeRateToApply = null;
                                    if (coupon.currencyID != 2)
                                    {
                                        if (exchangeRates == null)
                                        {
                                            exchangeRates = from r in db.tblExchangeRates
                                                            where r.exchangeRateTypeID == 1
                                                            && r.terminalID == coupon.tblPurchases.terminalID
                                                            && r.providerID == null
                                                            && r.fromCurrencyID == coupon.currencyID
                                                            && r.toCurrencyID == 2
                                                            && ((r.permanent_ && r.fromDate < tomorrow)
                                                            || (r.permanent_ != true && r.fromDate <= tomorrow && r.toDate >= today))
                                                            select r;

                                        }

                                        DateTime currentDate = coupon.confirmationDateTime ?? coupon.dateSaved;
                                        exchangeRateToApply = (from x in exchangeRates
                                                               where ((x.permanent_ && x.fromDate < currentDate)
                                                               || (x.permanent_ != true && x.fromDate <= currentDate && x.toDate >= currentDate))
                                                               && x.tblExchangeRates_PointsOfSales.Count(
                                                                p => p.pointOfSaleID == coupon.tblPurchases.pointOfSaleID
                                                                && p.dateAdded <= currentDate
                                                                && (p.dateDeleted == null || p.dateDeleted > currentDate)
                                                                ) > 0
                                                               orderby x.fromDate descending
                                                               select x).FirstOrDefault();

                                        if (exchangeRateToApply == null)
                                        {
                                            exchangeRateToApply = (from x in exchangeRates
                                                                   where ((x.permanent_ && x.fromDate < currentDate)
                                                                   || (x.permanent_ != true && x.fromDate <= currentDate && x.toDate >= currentDate))
                                                                   && x.tblExchangeRates_PointsOfSales.Count() == 0
                                                                   orderby x.fromDate descending
                                                                   select x).FirstOrDefault();

                                            if (exchangeRateToApply == null)
                                            {
                                                exchangeRateToApply = (from r in db.tblExchangeRates
                                                                       where r.exchangeRateTypeID == 1
                                                                       && r.terminalID == coupon.tblPurchases.terminalID
                                                                       && r.providerID == null
                                                                       && r.fromCurrencyID == coupon.currencyID
                                                                       && r.toCurrencyID == 2
                                                                       && r.fromDate <= tomorrow
                                                                       && r.tblExchangeRates_PointsOfSales.Count() == 0
                                                                       orderby r.fromDate descending, r.dateSaved descending
                                                                       select r).FirstOrDefault();
                                            }
                                        }
                                    }
                                    if (agent.Commissions.Count(x =>
                                        x.PriceTypeID == priceTypeID &&
                                        x.MinProfit <= couponInfo.UtilityPercentage &&
                                        (x.MaxProfit == null || x.MaxProfit >= couponInfo.UtilityPercentage)
                                        ) > 0)
                                    {
                                        agent.Commissions.FirstOrDefault(x =>
                                            x.PriceTypeID == priceTypeID &&
                                            x.MinProfit <= couponInfo.UtilityPercentage &&
                                            (x.MaxProfit == null || x.MaxProfit >= couponInfo.UtilityPercentage)
                                            ).Production.Amount += decimal.Round(couponInfo.Total.Amount * (exchangeRateToApply != null ? exchangeRateToApply.exchangeRate : 1), 2, MidpointRounding.AwayFromZero);

                                        agent.Commissions.FirstOrDefault(x =>
                                            x.PriceTypeID == priceTypeID &&
                                            x.MinProfit <= couponInfo.UtilityPercentage &&
                                            (x.MaxProfit == null || x.MaxProfit >= couponInfo.UtilityPercentage)).ProductionNoIVA.Amount += decimal.Round(couponInfo.TotalNoIVA.Amount * (exchangeRateToApply != null ? exchangeRateToApply.exchangeRate : 1), 2, MidpointRounding.AwayFromZero);

                                        agent.Production.Amount += decimal.Round(couponInfo.Total.Amount * (exchangeRateToApply != null ? exchangeRateToApply.exchangeRate : 1), 2, MidpointRounding.AwayFromZero);
                                        agent.ProductionNoIVA.Amount += decimal.Round(couponInfo.TotalNoIVA.Amount * (exchangeRateToApply != null ? exchangeRateToApply.exchangeRate : 1), 2, MidpointRounding.AwayFromZero);

                                        dc.Production.Amount += decimal.Round(couponInfo.Total.Amount * (exchangeRateToApply != null ? exchangeRateToApply.exchangeRate : 1), 2, MidpointRounding.AwayFromZero);
                                        dc.ProductionNoIVA.Amount += decimal.Round(couponInfo.TotalNoIVA.Amount * (exchangeRateToApply != null ? exchangeRateToApply.exchangeRate : 1), 2, MidpointRounding.AwayFromZero);
                                    }


                                    //commissions
                                    if (couponInfo.Percentage >= 0)
                                    {
                                        var commissionPercentage = (from c in Commissions
                                                                    where c.priceTypeID == priceTypeID
                                                                    && c.pointOfSaleIDs.Where(p => p == coupon.tblPurchases.pointOfSaleID).Count() > 0
                                                                    && ((c.permanent_ && c.fromDate <= coupon.confirmationDateTime) || (!c.permanent_ && c.fromDate <= coupon.confirmationDateTime && c.toDate >= coupon.confirmationDateTime))
                                                                    && c.minProfit <= couponInfo.UtilityPercentage
                                                                    && (c.maxProfit == null || c.maxProfit >= couponInfo.UtilityPercentage)
                                                                    select c.commissionPercentage).FirstOrDefault();

                                        couponInfo.Percentage = commissionPercentage;
                                        couponInfo.Commission.Amount = decimal.Round((couponInfo.TotalNoIVA.Amount * commissionPercentage / 100), 2, MidpointRounding.AwayFromZero);
                                        couponInfo.Commission.Currency = coupon.tblCurrencies.currencyCode;

                                        //si la moneda de venta no es pesos, convertir a pesos con el tipo de cambio de la agencia vigente
                                        if (coupon.currencyID != 2)
                                        {
                                            couponInfo.SalesMXN.Amount = decimal.Round(couponInfo.TotalNoIVA.Amount * exchangeRateToApply.exchangeRate, 2, MidpointRounding.AwayFromZero);
                                            couponInfo.CommissionMXN.Amount = decimal.Round(couponInfo.Commission.Amount * exchangeRateToApply.exchangeRate, 2, MidpointRounding.AwayFromZero);
                                            couponInfo.ExchangeRate = exchangeRateToApply.exchangeRate;
                                            couponInfo.ExchangeRateID = exchangeRateToApply.exchangeRateID;
                                            couponInfo.ExchangeRateVigency = exchangeRateToApply.fromDate.ToString("yyyy-MM-dd") + "/" + (exchangeRateToApply.permanent_ ? "permanent" : exchangeRateToApply.toDate.Value.ToString("yyyy-MM-dd"));
                                        }
                                        else
                                        {
                                            couponInfo.SalesMXN.Amount = couponInfo.TotalNoIVA.Amount;
                                            couponInfo.CommissionMXN.Amount = couponInfo.Commission.Amount;
                                            couponInfo.ExchangeRate = 1;
                                        }

                                        //-total commissions
                                        if (agent.Commissions.Count(x =>
                                    x.PriceTypeID == priceTypeID &&
                                    x.MinProfit <= couponInfo.UtilityPercentage &&
                                    (x.MaxProfit == null || x.MaxProfit >= couponInfo.UtilityPercentage)
                                    ) > 0)
                                        {
                                            agent.Commissions.FirstOrDefault(x =>
                                        x.PriceTypeID == priceTypeID &&
                                        x.MinProfit <= couponInfo.UtilityPercentage &&
                                        (x.MaxProfit == null || x.MaxProfit >= couponInfo.UtilityPercentage)
                                        ).Commission.Amount += couponInfo.CommissionMXN.Amount;
                                        }
                                        agent.Commission.Amount += couponInfo.CommissionMXN.Amount;
                                        dc.Commission.Amount += couponInfo.CommissionMXN.Amount;
                                    }
                                }
                            }
                        }
                    }

                    dc.AgentsCommissions.Add(agent);
                }
            }

            return dc;
        }

        public static AgentsVolume GetAgentsVolume(string period = "d")
        {
            AgentsVolume av = new AgentsVolume();
            av.Agents = new List<AgentsVolume.AgentVolume>();

            ePlatEntities db = new ePlatEntities();

            long[] terminals = { };

            if (session.Terminals != "")
            {
                terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            }
            if (terminals.Length > 0)
            {
                long terminalID = terminals.FirstOrDefault();
                int preferredCurrencyID = 2;
                var WidgetCurrencyQ = (from w in db.tblTerminals
                                       where w.terminalID == terminalID
                                       select w.dashboardDailyVolumeCurrencyID).FirstOrDefault();
                if (WidgetCurrencyQ != null)
                {
                    preferredCurrencyID = (int)WidgetCurrencyQ;
                }
                decimal exchangeRate = MasterChartDataModel.Purchases.GetSpecificRate
(DateTime.Now, "USD", terminalID);
                av.ExchangeRate = exchangeRate;

                av.Total = new Money()
                {
                    Amount = 0,
                    Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                };

                //pricetypes


                //con iva, sin iva

                //agentes

                //establecer periodo del reporte

                //DateTime begin = DateTime.Today.Date;
                //DateTime end = begin.AddDays(1);
                //if (period == "w")
                //{
                //    begin = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));
                //}
                //else if (period == "m")
                //{
                //    begin = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                //}

                //var couponDetails = from detail in db.tblPurchaseServiceDetails
                //                    join coupon in db.tblPurchases_Services on detail.purchase_ServiceID equals coupon.purchase_ServiceID
                //                    join purchase in db.tblPurchases on coupon.purchaseID equals purchase.purchaseID
                //                    where purchase.terminalID == terminalID
                //                    && coupon.serviceStatusID > 2
                //                    && posIDs.Contains(purchase.pointOfSaleID)
                //                    && ((coupon.confirmationDateTime > begin && coupon.confirmationDateTime < end) || (coupon.cancelationDateTime > begin && coupon.cancelationDateTime < end))
                //                    select new
                //                    {
                //                        detail.quantity,
                //                        detail.customPrice,
                //                        detail.dealPrice,
                //                        detail.priceTypeID,
                //                        detail.purchaseServiceDetailID,
                //                        coupon.serviceStatusID,
                //                        coupon.currencyID,
                //                        purchase.pointOfSaleID
                //                    };
                //foreach (var detail in couponDetails)
                //{
                //    decimal unitTotal = 0;
                //    if (detail.dealPrice != null)
                //    {
                //        unitTotal = decimal.Round(PromoDataModel.ApplyPromo(detail.quantity * (decimal)detail.dealPrice, detail.purchaseServiceDetailID), 2, MidpointRounding.AwayFromZero);
                //    }
                //    else
                //    {
                //        unitTotal = decimal.Round(PromoDataModel.ApplyPromo(detail.quantity * (decimal)detail.customPrice, detail.purchaseServiceDetailID), 2, MidpointRounding.AwayFromZero);
                //    }
                //    if (detail.currencyID == preferredCurrencyID)
                //    {
                //        unitTotal = decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero);
                //    }
                //    else
                //    {
                //        if (preferredCurrencyID == 1)
                //        {
                //            unitTotal = decimal.Round(unitTotal / exchangeRate, 2, MidpointRounding.AwayFromZero);
                //        }
                //        else
                //        {
                //            unitTotal = decimal.Round(unitTotal * exchangeRate, 2, MidpointRounding.AwayFromZero);
                //        }
                //    }
                //    if (detail.serviceStatusID == 3 || detail.serviceStatusID == 6)
                //    {
                //        dv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).ConfirmedCoupons += 1;
                //        dv.Total.Amount += unitTotal;
                //        dv.PriceTypeTotals.FirstOrDefault(x => x.PriceTypeID == detail.priceTypeID).Total.Amount += unitTotal;
                //        dv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).Total.Amount += unitTotal;
                //        dv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == detail.priceTypeID).Total.Amount += unitTotal;
                //    }
                //    else if (detail.serviceStatusID == 5)
                //    {
                //        dv.Total.Amount -= unitTotal;
                //        dv.PriceTypeTotals.FirstOrDefault(x => x.PriceTypeID == detail.priceTypeID).Total.Amount -= unitTotal;
                //        dv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).Total.Amount -= unitTotal;
                //        dv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == detail.priceTypeID).Total.Amount -= unitTotal;
                //    }
                //}

                ////obtención de porcentajes
                //decimal maxVolume = 1;
                //foreach (var pos in dv.PoSVolume)
                //{
                //    foreach (var pt in pos.PriceTypeVolume)
                //    {
                //        if (pt.Total.Amount > maxVolume)
                //        {
                //            maxVolume = pt.Total.Amount;
                //        }
                //    }
                //}

                //foreach (var pos in dv.PoSVolume)
                //{
                //    foreach (var pt in pos.PriceTypeVolume)
                //    {
                //        pt.GlobalPercentage = (int)(pt.Total.Amount * 100 / maxVolume);
                //        if (pos.Total.Amount > 0)
                //        {
                //            pt.Percentage = decimal.Round(pt.Total.Amount * 100 / pos.Total.Amount, 2, MidpointRounding.AwayFromZero);
                //        }
                //    }
                //}

                //decimal maxPriceTypeTotal = 1;
                //foreach (var pt in dv.PriceTypeTotals)
                //{
                //    if (pt.Total.Amount > maxPriceTypeTotal)
                //    {
                //        maxPriceTypeTotal = pt.Total.Amount;
                //    }
                //}
                //foreach (var pt in dv.PriceTypeTotals)
                //{

                //    if (dv.Total.Amount > 0)
                //    {
                //        pt.Percentage = decimal.Round(pt.Total.Amount * 100 / dv.Total.Amount, 2, MidpointRounding.AwayFromZero);
                //        pt.GlobalPercentage = (int)(pt.Total.Amount * 100 / maxPriceTypeTotal);
                //    }
                //}

                ////order
                //dv.PoSVolume = dv.PoSVolume.OrderByDescending(x => x.Total.Amount).ToList();
            }

            return av;
        }

        public static DailyVolume GetDailyVolume(string period = "d", long? terminalID = null)
        {
            DailyVolume dv = new DailyVolume();
            //terminalID
            if (terminalID == null)
            {
                long[] terminals = { };

                if (session.Terminals != "")
                {
                    terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                }
                if (terminals.Length > 0)
                {
                    terminalID = terminals.FirstOrDefault();
                }
            }

            //establecer meta
            ePlatEntities db = new ePlatEntities();
            DateTime month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var MonthlyGoal = (from g in db.tblTerminalGoals
                               where g.terminalID == terminalID
                               && g.month == month
                               select g).FirstOrDefault();

            if (MonthlyGoal != null)
            {
                dv.GoalTotal = new Money()
                {
                    Amount = MonthlyGoal.goal,
                    Currency = (MonthlyGoal.currencyID == 1 ? "USD" : "MXN")
                };
            }
            else
            {
                dv.GoalTotal = new Money()
                {
                    Amount = 0,
                    Currency = "USD"
                };
            }

            //establecer periodo del reporte
            DateTime begin = DateTime.Today.Date;
            DateTime end = begin.AddDays(1);
            DateTime lyBegin = begin.AddYears(-1);
            DateTime lyEnd = end.AddYears(-1);
            if (period == "d")
            {
                dv.GoalTotal.Amount = dv.GoalTotal.Amount / DateTime.DaysInMonth(begin.Year, begin.Month);
            }
            else if (period == "y")
            {
                begin = DateTime.Today.AddDays(-1);
                end = begin.AddDays(1);
                lyBegin = begin.AddYears(-1);
                lyEnd = end.AddYears(-1);

                dv.GoalTotal.Amount = dv.GoalTotal.Amount / DateTime.DaysInMonth(begin.Year, begin.Month);
            }
            else if (period == "w")
            {
                begin = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));
                int weekNumber = Utils.GeneralFunctions.DateFormat.GetIso8601WeekOfYear(DateTime.Today);
                lyBegin = Utils.GeneralFunctions.DateFormat.FirstDateOfWeekISO8601(DateTime.Today.AddYears(-1).Year, weekNumber);
                lyEnd = lyBegin.AddDays((int)DateTime.Today.DayOfWeek + 1);

                dv.GoalTotal.Amount = dv.GoalTotal.Amount / DateTime.DaysInMonth(begin.Year, begin.Month) * (int)DateTime.Today.DayOfWeek;
            }
            else if (period == "m")
            {
                begin = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                lyBegin = new DateTime(DateTime.Today.AddYears(-1).Year, DateTime.Today.AddYears(-1).Month, 1);
                lyEnd = DateTime.Today.AddYears(-1).AddDays(1).Date;
            }

            //llamar función de cálculo
            DailyVolume.DailyVolumeSet currentVolume = GetDailyVolumeByDate((long)terminalID, begin, end);

            DailyVolume.DailyVolumeSet lastYearVolume = GetDailyVolumeByDate((long)terminalID, lyBegin, lyEnd);

            dv.PoSVolume = currentVolume.PoSVolume;
            dv.LYPoSVolume = lastYearVolume.PoSVolume;
            dv.AgentsVolume = currentVolume.AgentsVolume;
            dv.ProgramsVolume = currentVolume.ProgramsVolume;
            dv.ExchangeRate = currentVolume.ExchangeRate;
            dv.LYExchangeRate = lastYearVolume.ExchangeRate;
            dv.Total = currentVolume.Total;
            dv.LYTotal = lastYearVolume.Total;
            dv.PriceTypeTotals = currentVolume.PriceTypeTotals;
            dv.LYPriceTypeTotals = lastYearVolume.PriceTypeTotals;

            //obtención de porcentajes
            decimal maxVolume = 1;
            foreach (var pos in dv.PoSVolume)
            {
                foreach (var pt in pos.PriceTypeVolume)
                {
                    if (pt.Total.Amount > maxVolume)
                    {
                        maxVolume = pt.Total.Amount;
                    }
                }
            }
            foreach (var pos in dv.LYPoSVolume)
            {
                foreach (var pt in pos.PriceTypeVolume)
                {
                    if (pt.Total.Amount > maxVolume)
                    {
                        maxVolume = pt.Total.Amount;
                    }
                }
            }

            decimal maxAgentVolume = 1;
            foreach (var ag in dv.AgentsVolume)
            {
                foreach (var pt in ag.PriceTypeVolume)
                {
                    if (pt.Total.Amount > maxAgentVolume)
                    {
                        maxAgentVolume = pt.Total.Amount;
                    }
                }
            }


            decimal maxProgramVolume = 1;
            foreach (var pr in dv.ProgramsVolume)
            {
                foreach (var pt in pr.PriceTypeVolume)
                {
                    if (pt.Total.Amount > maxProgramVolume)
                    {
                        maxProgramVolume = pt.Total.Amount;
                    }
                }
            }

            foreach (var pos in dv.PoSVolume)
            {
                foreach (var pt in pos.PriceTypeVolume)
                {
                    pt.GlobalPercentage = (int)(pt.Total.Amount * 100 / maxVolume);
                    if (pos.Total.Amount > 0)
                    {
                        pt.Percentage = decimal.Round(pt.Total.Amount * 100 / pos.Total.Amount, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }
            foreach (var pos in dv.LYPoSVolume)
            {
                foreach (var pt in pos.PriceTypeVolume)
                {
                    pt.GlobalPercentage = (int)(pt.Total.Amount * 100 / maxVolume);
                    if (pos.Total.Amount > 0)
                    {
                        pt.Percentage = decimal.Round(pt.Total.Amount * 100 / pos.Total.Amount, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }
            foreach (var ag in dv.AgentsVolume)
            {
                foreach (var pt in ag.PriceTypeVolume)
                {
                    pt.GlobalPercentage = (int)(pt.Total.Amount * 100 / maxAgentVolume);
                    if (ag.Total.Amount > 0)
                    {
                        pt.Percentage = decimal.Round(pt.Total.Amount * 100 / ag.Total.Amount, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }
            foreach (var pr in dv.ProgramsVolume)
            {
                foreach (var pt in pr.PriceTypeVolume)
                {
                    pt.GlobalPercentage = (int)(pt.Total.Amount * 100 / maxProgramVolume);
                    if (pr.Total.Amount > 0)
                    {
                        pt.Percentage = decimal.Round(pt.Total.Amount * 100 / pr.Total.Amount, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }

            decimal maxPriceTypeTotal = 1;
            foreach (var pt in dv.PriceTypeTotals)
            {
                if (pt.Total.Amount > maxPriceTypeTotal)
                {
                    maxPriceTypeTotal = pt.Total.Amount;
                }
            }
            foreach (var pt in dv.LYPriceTypeTotals)
            {
                if (pt.Total.Amount > maxPriceTypeTotal)
                {
                    maxPriceTypeTotal = pt.Total.Amount;
                }
            }

            foreach (var pt in dv.PriceTypeTotals)
            {
                if (dv.Total.Amount > 0)
                {
                    pt.Percentage = decimal.Round(pt.Total.Amount * 100 / dv.Total.Amount, 2, MidpointRounding.AwayFromZero);
                    pt.GlobalPercentage = (int)(pt.Total.Amount * 100 / maxPriceTypeTotal);
                }
            }
            foreach (var pt in dv.LYPriceTypeTotals)
            {
                if (dv.LYTotal.Amount > 0)
                {
                    pt.Percentage = decimal.Round(pt.Total.Amount * 100 / dv.LYTotal.Amount, 2, MidpointRounding.AwayFromZero);
                    pt.GlobalPercentage = (int)(pt.Total.Amount * 100 / maxPriceTypeTotal);
                }
            }

            //porcentaje respecto a meta
            if (dv.GoalTotal.Amount > 0)
            {
                dv.GoalPercentage = decimal.Round(dv.Total.Amount * 100 / dv.GoalTotal.Amount, 2);
            }
            else
            {
                dv.GoalPercentage = 0;
            }


            //order
            dv.PoSVolume = dv.PoSVolume.OrderByDescending(x => x.Total.Amount).ToList();
            dv.AgentsVolume = dv.AgentsVolume.OrderByDescending(x => x.Total.Amount).ToList();
            dv.ProgramsVolume = dv.ProgramsVolume.OrderByDescending(x => x.Total.Amount).ToList();

            return dv;
        }

        public static DailyVolume.DailyVolumeSet GetDailyVolumeByDate(long terminalID, DateTime begin, DateTime end)
        {
            ePlatEntities db = new ePlatEntities();
            DailyVolume.DailyVolumeSet dv = new DailyVolume.DailyVolumeSet();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            dv.PoSVolume = new List<DailyVolume.PointsOfSaleVolume>();
            dv.AgentsVolume = new List<DailyVolume.AgentVolume>();
            dv.ProgramsVolume = new List<DailyVolume.ProgramVolume>();

            int preferredCurrencyID = 2;
            var WidgetCurrencyQ = (from w in db.tblTerminals
                                   where w.terminalID == terminalID
                                   select w.dashboardDailyVolumeCurrencyID).FirstOrDefault();
            if (WidgetCurrencyQ != null)
            {
                preferredCurrencyID = (int)WidgetCurrencyQ;
            }
            decimal exchangeRate = MasterChartDataModel.Purchases.GetSpecificRate
(DateTime.Now, "USD", terminalID);
            dv.ExchangeRate = exchangeRate;

            ReportDataModel rdm = new ReportDataModel();
            //List<PriceType> PriceTypes = rdm.GetListOfPriceTypes(terminalID, false);
            List<PriceType> PriceTypes = rdm.GetListOfMainPriceTypes(terminalID);

            if (PriceTypes.Count() == 0)
            {
                PriceTypes = rdm.GetListOfPriceTypes(terminalID, false);
            }

            var MainPriceTypes = from m in db.tblPriceTypesGroups
                                 where m.terminalID == terminalID
                                 select m;

            dv.Total = new Money()
            {
                Amount = 0,
                Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
            };

            dv.PriceTypeTotals = new List<DailyVolume.PriceTypeVolume>();
            foreach (var pt in PriceTypes)
            {
                DailyVolume.PriceTypeVolume ptv = new DailyVolume.PriceTypeVolume();
                ptv.PriceTypeID = pt.PriceTypeID;
                ptv.PriceType = pt.Type;
                ptv.Total = new Money()
                {
                    Amount = 0,
                    Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                };
                ptv.Percentage = 0;
                ptv.GlobalPercentage = 0;
                dv.PriceTypeTotals.Add(ptv);
            }

            var PointsOfSale = from p in db.tblPointsOfSale
                               where p.terminalID == terminalID
                               && !p.pointOfSale.Contains("test")
                               && !p.pointOfSale.Contains("prueba")
                               select new
                               {
                                   p.pointOfSaleID,
                                   p.shortName,
                                   p.pointOfSale
                               };

            int[] posIDs = PointsOfSale.Select(x => x.pointOfSaleID).ToArray();

            foreach (var pos in PointsOfSale)
            {
                DailyVolume.PointsOfSaleVolume posVolume = new DailyVolume.PointsOfSaleVolume();
                posVolume.PointOfSaleID = pos.pointOfSaleID;
                posVolume.PointOfSale = pos.pointOfSale;
                posVolume.ShortName = pos.shortName;
                posVolume.PriceTypeVolume = new List<DailyVolume.PriceTypeVolume>();
                foreach (var pt in PriceTypes)
                {
                    DailyVolume.PriceTypeVolume ptv = new DailyVolume.PriceTypeVolume();
                    ptv.PriceTypeID = pt.PriceTypeID;
                    ptv.PriceType = pt.Type;
                    ptv.Total = new Money()
                    {
                        Amount = 0,
                        Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                    };
                    ptv.Percentage = 0;
                    ptv.GlobalPercentage = 0;
                    posVolume.PriceTypeVolume.Add(ptv);
                }
                posVolume.Total = new Money()
                {
                    Amount = 0,
                    Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                };
                dv.PoSVolume.Add(posVolume);
            }


            //obtener todo el historial dentro del rango de fecha
            var Cache = from c in db.tblWidgetsCache
                        where c.widgetID == 1
                        && c.date >= begin
                        && c.date < end
                        && c.terminalID == terminalID
                        select c;

            while (begin < end)
            {
                DailyVolume.DailyVolumeSet cdv = new DailyVolume.DailyVolumeSet();

                if (Cache.FirstOrDefault(x => x.date == begin) == null)
                {
                    cdv.PoSVolume = new List<DailyVolume.PointsOfSaleVolume>();
                    cdv.AgentsVolume = new List<DailyVolume.AgentVolume>();
                    cdv.ProgramsVolume = new List<DailyVolume.ProgramVolume>();
                    cdv.ExchangeRate = MasterChartDataModel.Purchases.GetSpecificRate
    (begin, "USD", terminalID);
                    cdv.Total = new Money()
                    {
                        Amount = 0,
                        Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                    };

                    cdv.PriceTypeTotals = new List<DailyVolume.PriceTypeVolume>();
                    foreach (var pt in PriceTypes)
                    {
                        DailyVolume.PriceTypeVolume ptv = new DailyVolume.PriceTypeVolume();
                        ptv.PriceTypeID = pt.PriceTypeID;
                        ptv.PriceType = pt.Type;
                        ptv.Total = new Money()
                        {
                            Amount = 0,
                            Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                        };
                        ptv.Percentage = 0;
                        ptv.GlobalPercentage = 0;
                        cdv.PriceTypeTotals.Add(ptv);
                    }

                    foreach (var pos in PointsOfSale)
                    {
                        DailyVolume.PointsOfSaleVolume posVolume = new DailyVolume.PointsOfSaleVolume();
                        posVolume.PointOfSaleID = pos.pointOfSaleID;
                        posVolume.PointOfSale = pos.pointOfSale;
                        posVolume.ShortName = pos.shortName;
                        posVolume.PriceTypeVolume = new List<DailyVolume.PriceTypeVolume>();
                        foreach (var pt in PriceTypes)
                        {
                            DailyVolume.PriceTypeVolume ptv = new DailyVolume.PriceTypeVolume();
                            ptv.PriceTypeID = pt.PriceTypeID;
                            ptv.PriceType = pt.Type;
                            ptv.Total = new Money()
                            {
                                Amount = 0,
                                Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                            };
                            ptv.Percentage = 0;
                            ptv.GlobalPercentage = 0;
                            posVolume.PriceTypeVolume.Add(ptv);
                        }
                        posVolume.Total = new Money()
                        {
                            Amount = 0,
                            Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                        };
                        cdv.PoSVolume.Add(posVolume);
                    }

                    DateTime nextDate = begin.AddDays(1);

                    var couponDetails = from detail in db.tblPurchaseServiceDetails
                                        join coupon in db.tblPurchases_Services on detail.purchase_ServiceID equals coupon.purchase_ServiceID
                                        join purchase in db.tblPurchases on coupon.purchaseID equals purchase.purchaseID
                                        join profile in db.tblUserProfiles on coupon.confirmedByUserID equals profile.userID
                                        into coupon_profile
                                        from profile in coupon_profile.DefaultIfEmpty()
                                        where purchase.terminalID == terminalID
                                        && coupon.serviceStatusID > 2
                                        && posIDs.Contains(purchase.pointOfSaleID)
                                        && ((coupon.confirmationDateTime > begin && coupon.confirmationDateTime < nextDate) || (coupon.cancelationDateTime > begin && coupon.cancelationDateTime < nextDate))
                                        select new
                                        {
                                            detail.quantity,
                                            detail.customPrice,
                                            detail.dealPrice,
                                            detail.priceTypeID,
                                            detail.purchaseServiceDetailID,
                                            coupon.serviceStatusID,
                                            coupon.currencyID,
                                            coupon.confirmedByUserID,
                                            profile.firstName,
                                            profile.lastName,
                                            purchase.pointOfSaleID,
                                            purchase.spiMarketingProgram
                                        };

                    foreach (var detail in couponDetails)
                    {
                        decimal unitTotal = 0;
                        if (detail.dealPrice != null)
                        {
                            unitTotal = decimal.Round(PromoDataModel.ApplyPromo(detail.quantity * (decimal)detail.dealPrice, detail.purchaseServiceDetailID), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            unitTotal = decimal.Round(PromoDataModel.ApplyPromo(detail.quantity * (decimal)detail.customPrice, detail.purchaseServiceDetailID), 2, MidpointRounding.AwayFromZero);
                        }
                        if (detail.currencyID == preferredCurrencyID)
                        {
                            unitTotal = decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            if (preferredCurrencyID == 1)
                            {
                                unitTotal = decimal.Round(unitTotal / cdv.ExchangeRate, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                unitTotal = decimal.Round(unitTotal * cdv.ExchangeRate, 2, MidpointRounding.AwayFromZero);
                            }
                        }
                        int? priceTypeID = priceTypeID = detail.priceTypeID;
                        if (MainPriceTypes.FirstOrDefault(x => x.priceTypeID == detail.priceTypeID) != null)
                        {
                            priceTypeID = MainPriceTypes.FirstOrDefault(x => x.priceTypeID == detail.priceTypeID).mainPriceTypeID;
                        }

                        //si no existe agente en la lista, agregar
                        if (cdv.AgentsVolume.FirstOrDefault(x => x.AgentID == detail.confirmedByUserID) == null)
                        {
                            DailyVolume.AgentVolume agentVolume = new DailyVolume.AgentVolume();
                            agentVolume.AgentID = detail.confirmedByUserID;
                            agentVolume.Agent = detail.firstName + " " + detail.lastName;
                            agentVolume.PriceTypeVolume = new List<DailyVolume.PriceTypeVolume>();
                            foreach (var pt in PriceTypes)
                            {
                                DailyVolume.PriceTypeVolume ptv = new DailyVolume.PriceTypeVolume();
                                ptv.PriceTypeID = pt.PriceTypeID;
                                ptv.PriceType = pt.Type;
                                ptv.Total = new Money()
                                {
                                    Amount = 0,
                                    Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                                };
                                ptv.Percentage = 0;
                                ptv.GlobalPercentage = 0;
                                agentVolume.PriceTypeVolume.Add(ptv);
                            }
                            agentVolume.Total = new Money()
                            {
                                Amount = 0,
                                Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                            };
                            cdv.AgentsVolume.Add(agentVolume);
                        }

                        //si no existe programa en la lista, agregar
                        if (cdv.ProgramsVolume.FirstOrDefault(x => x.Program == detail.spiMarketingProgram) == null)
                        {
                            DailyVolume.ProgramVolume programVolume = new DailyVolume.ProgramVolume();
                            programVolume.Program = detail.spiMarketingProgram;
                            programVolume.PriceTypeVolume = new List<DailyVolume.PriceTypeVolume>();
                            foreach (var pt in PriceTypes)
                            {
                                DailyVolume.PriceTypeVolume ptv = new DailyVolume.PriceTypeVolume();
                                ptv.PriceTypeID = pt.PriceTypeID;
                                ptv.PriceType = pt.Type;
                                ptv.Total = new Money()
                                {
                                    Amount = 0,
                                    Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                                };
                                ptv.Percentage = 0;
                                ptv.GlobalPercentage = 0;
                                programVolume.PriceTypeVolume.Add(ptv);
                            }
                            programVolume.Total = new Money()
                            {
                                Amount = 0,
                                Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                            };
                            cdv.ProgramsVolume.Add(programVolume);
                        }

                        if (detail.serviceStatusID == 3 || detail.serviceStatusID == 6)
                        {
                            cdv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).ConfirmedCoupons += 1;
                            cdv.AgentsVolume.FirstOrDefault(x => x.AgentID == detail.confirmedByUserID).ConfirmedCoupons += 1;
                            cdv.ProgramsVolume.FirstOrDefault(x => x.Program == detail.spiMarketingProgram).ConfirmedCoupons += 1;
                            cdv.Total.Amount += unitTotal;
                            if (cdv.PriceTypeTotals.FirstOrDefault(x => x.PriceTypeID == priceTypeID) != null)
                            {
                                cdv.PriceTypeTotals.FirstOrDefault(x => x.PriceTypeID == priceTypeID).Total.Amount += unitTotal;
                                cdv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == priceTypeID).Total.Amount += unitTotal;
                                cdv.AgentsVolume.FirstOrDefault(x => x.AgentID == detail.confirmedByUserID).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == priceTypeID).Total.Amount += unitTotal;
                                cdv.ProgramsVolume.FirstOrDefault(x => x.Program == detail.spiMarketingProgram).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == priceTypeID).Total.Amount += unitTotal;
                            }
                            cdv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).Total.Amount += unitTotal;
                            cdv.AgentsVolume.FirstOrDefault(x => x.AgentID == detail.confirmedByUserID).Total.Amount += unitTotal;
                            cdv.ProgramsVolume.FirstOrDefault(x => x.Program == detail.spiMarketingProgram).Total.Amount += unitTotal;
                        }
                        else if (detail.serviceStatusID == 5)
                        {
                            cdv.Total.Amount -= unitTotal;
                            if (cdv.PriceTypeTotals.FirstOrDefault(x => x.PriceTypeID == priceTypeID) != null)
                            {
                                cdv.PriceTypeTotals.FirstOrDefault(x => x.PriceTypeID == priceTypeID).Total.Amount -= unitTotal;
                                cdv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == priceTypeID).Total.Amount -= unitTotal;
                                cdv.AgentsVolume.FirstOrDefault(x => x.AgentID == detail.confirmedByUserID).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == priceTypeID).Total.Amount -= unitTotal;
                                cdv.ProgramsVolume.FirstOrDefault(x => x.Program == detail.spiMarketingProgram).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == priceTypeID).Total.Amount -= unitTotal;
                            }
                            cdv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == detail.pointOfSaleID).Total.Amount -= unitTotal;
                            cdv.AgentsVolume.FirstOrDefault(x => x.AgentID == detail.confirmedByUserID).Total.Amount -= unitTotal;
                            cdv.ProgramsVolume.FirstOrDefault(x => x.Program == detail.spiMarketingProgram).Total.Amount -= unitTotal;
                        }
                    }

                    //guardar en cache
                    if (begin < DateTime.Today)
                    {
                        tblWidgetsCache cache = new tblWidgetsCache();
                        cache.widgetID = 1;
                        cache.terminalID = terminalID;
                        cache.widgetJSON = js.Serialize(cdv);
                        cache.date = begin;
                        cache.dateSaved = DateTime.Now;
                        db.tblWidgetsCache.AddObject(cache);
                    }

                }
                else
                {
                    cdv = js.Deserialize<DailyVolume.DailyVolumeSet>(Cache.FirstOrDefault(x => x.date == begin).widgetJSON);
                }

                //sumatoria de cdv a dv
                foreach (var posvolume in cdv.PoSVolume)
                {
                    dv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == posvolume.PointOfSaleID).ConfirmedCoupons += posvolume.ConfirmedCoupons;
                    dv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == posvolume.PointOfSaleID).Total.Amount += posvolume.Total.Amount;
                    foreach (var ptvolume in posvolume.PriceTypeVolume)
                    {
                        //dv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == posvolume.PointOfSaleID).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == ptvolume.PriceTypeID).Total.Amount += ptvolume.Total.Amount;
                        var priceTypeVolume = dv.PoSVolume.FirstOrDefault(x => x.PointOfSaleID == posvolume.PointOfSaleID);
                        if (priceTypeVolume.PriceTypeVolume.Count(m => m.PriceTypeID == ptvolume.PriceTypeID) > 0)
                            priceTypeVolume.PriceTypeVolume.FirstOrDefault(m => m.PriceTypeID == ptvolume.PriceTypeID).Total.Amount += ptvolume.Total.Amount;
                    }
                }
                foreach (var agentvolume in cdv.AgentsVolume)
                {
                    if (dv.AgentsVolume.FirstOrDefault(x => x.AgentID == agentvolume.AgentID) == null)
                    {
                        DailyVolume.AgentVolume agentVolume = new DailyVolume.AgentVolume();
                        agentVolume.AgentID = agentvolume.AgentID;
                        agentVolume.Agent = agentvolume.Agent;
                        agentVolume.PriceTypeVolume = new List<DailyVolume.PriceTypeVolume>();
                        foreach (var pt in PriceTypes)
                        {
                            DailyVolume.PriceTypeVolume ptv = new DailyVolume.PriceTypeVolume();
                            ptv.PriceTypeID = pt.PriceTypeID;
                            ptv.PriceType = pt.Type;
                            ptv.Total = new Money()
                            {
                                Amount = 0,
                                Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                            };
                            ptv.Percentage = 0;
                            ptv.GlobalPercentage = 0;
                            agentVolume.PriceTypeVolume.Add(ptv);
                        }
                        agentVolume.Total = new Money()
                        {
                            Amount = 0,
                            Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                        };
                        dv.AgentsVolume.Add(agentVolume);
                    }

                    dv.AgentsVolume.FirstOrDefault(x => x.AgentID == agentvolume.AgentID).ConfirmedCoupons += agentvolume.ConfirmedCoupons;
                    dv.AgentsVolume.FirstOrDefault(x => x.AgentID == agentvolume.AgentID).Total.Amount += agentvolume.Total.Amount;
                    foreach (var ptvolume in agentvolume.PriceTypeVolume)
                    {
                        //dv.AgentsVolume.FirstOrDefault(x => x.AgentID == agentvolume.AgentID).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == ptvolume.PriceTypeID).Total.Amount += ptvolume.Total.Amount;
                        var priceTypeVolume = dv.AgentsVolume.FirstOrDefault(x => x.AgentID == agentvolume.AgentID).PriceTypeVolume;
                        if (priceTypeVolume.Count(m => m.PriceTypeID == ptvolume.PriceTypeID) > 0)
                            priceTypeVolume.FirstOrDefault(m => m.PriceTypeID == ptvolume.PriceTypeID).Total.Amount += ptvolume.Total.Amount;
                    }
                }
                if (cdv.ProgramsVolume == null)
                {
                    cdv.ProgramsVolume = new List<DailyVolume.ProgramVolume>();
                }
                foreach (var programvolume in cdv.ProgramsVolume)
                {
                    if (dv.ProgramsVolume.FirstOrDefault(x => x.Program == programvolume.Program) == null)
                    {
                        DailyVolume.ProgramVolume programVolume = new DailyVolume.ProgramVolume();
                        programVolume.Program = programvolume.Program;
                        programVolume.PriceTypeVolume = new List<DailyVolume.PriceTypeVolume>();
                        foreach (var pt in PriceTypes)
                        {
                            DailyVolume.PriceTypeVolume ptv = new DailyVolume.PriceTypeVolume();
                            ptv.PriceTypeID = pt.PriceTypeID;
                            ptv.PriceType = pt.Type;
                            ptv.Total = new Money()
                            {
                                Amount = 0,
                                Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                            };
                            ptv.Percentage = 0;
                            ptv.GlobalPercentage = 0;
                            programVolume.PriceTypeVolume.Add(ptv);
                        }
                        programVolume.Total = new Money()
                        {
                            Amount = 0,
                            Currency = (preferredCurrencyID == 1 ? "USD" : "MXN")
                        };
                        dv.ProgramsVolume.Add(programVolume);
                    }
                    dv.ProgramsVolume.FirstOrDefault(x => x.Program == programvolume.Program).ConfirmedCoupons += programvolume.ConfirmedCoupons;
                    dv.ProgramsVolume.FirstOrDefault(x => x.Program == programvolume.Program).Total.Amount += programvolume.Total.Amount;
                    foreach (var ptvolume in programvolume.PriceTypeVolume)
                    {
                        //dv.ProgramsVolume.FirstOrDefault(x => x.Program == programvolume.Program).PriceTypeVolume.FirstOrDefault(x => x.PriceTypeID == ptvolume.PriceTypeID).Total.Amount += ptvolume.Total.Amount;
                        var priceTypeVolume = dv.ProgramsVolume.FirstOrDefault(x => x.Program == programvolume.Program).PriceTypeVolume;
                        if (priceTypeVolume.Count(m => m.PriceTypeID == ptvolume.PriceTypeID) > 0)
                            priceTypeVolume.FirstOrDefault(m => m.PriceTypeID == ptvolume.PriceTypeID).Total.Amount += ptvolume.Total.Amount;
                    }
                }

                dv.ExchangeRate = cdv.ExchangeRate;
                dv.Total.Amount += cdv.Total.Amount;
                foreach (var pricetypetotal in cdv.PriceTypeTotals)
                {
                    //dv.PriceTypeTotals.FirstOrDefault(x => x.PriceTypeID == pricetypetotal.PriceTypeID).Total.Amount += pricetypetotal.Total.Amount;
                    var ptTotal = dv.PriceTypeTotals.FirstOrDefault(x => x.PriceTypeID == pricetypetotal.PriceTypeID);
                    if (ptTotal != null)
                        ptTotal.Total.Amount += pricetypetotal.Total.Amount;
                }

                begin = begin.AddDays(1);
            }

            //guardar cache
            db.SaveChanges();

            return dv;
        }

        public static PendingCloseOuts GetPendingCloseOuts()
        {
            ePlatEntities db = new ePlatEntities();
            DateTime tomorrow = DateTime.Today.AddDays(1);
            PendingCloseOuts pco = new PendingCloseOuts();

            long[] terminals = { };

            if (session.Terminals != "")
            {
                terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            }
            if (terminals.Length > 0)
            {
                pco.PendingAgentCOs = new List<PendingCloseOuts.PendingAgentCloseOuts>();
                //cupones vendidos no cortados
                /*var sales = from c in db.tblPurchases_Services
                            join closeout in db.tblCloseOuts_Purchases on c.purchase_ServiceID equals closeout.purchase_ServiceID
                            join purchase in db.tblPurchases on c.purchaseID equals purchase.purchaseID
                            join pos in db.tblPointsOfSale on purchase.pointOfSaleID equals pos.pointOfSaleID
                            join lead in db.tblLeads on purchase.leadID equals lead.leadID
                            join service in db.tblServices on c.serviceID equals service.serviceID
                            where c.serviceStatusID == 3
                            && c.confirmedByUserID != null
                            && closeout == null
                            && c.confirmationDateTime < tomorrow
                            && pos.pointOfSale.Contains("Prueba")
                            && purchase.purchaseStatusID != 5
                            && purchase.isTest != true
                            && terminals.Contains(purchase.terminalID)
                            select new
                            {
                                c.confirmedByUserID,
                                pos.pointOfSaleID,
                                pos.pointOfSale,
                                pos.shortName,
                                lead.firstName,
                                lead.lastName,
                                service.service,
                                c.tblPurchaseServiceStatus.purchaseServiceStatus,
                                c.tblPurchaseServiceDetails.FirstOrDefault().coupon,
                                c.confirmationDateTime
                            };*/

                var sales = (from c in db.tblPurchases_Services
                             join p in db.tblPaymentDetails on c.purchaseID equals p.purchaseID into c_p
                             from p in c_p.DefaultIfEmpty()
                             where c.serviceStatusID == 3
                             && c.confirmedByUserID != null
                             && c.tblCloseOuts_Purchases.Count() == 0
                             && c.confirmationDateTime < tomorrow
                             && !c.tblPurchases.tblPointsOfSale.pointOfSale.Contains("Prueba")
                             && c.tblPurchases.purchaseStatusID != 5
                             && c.tblPurchases.isTest != true
                             && terminals.Contains(c.tblPurchases.terminalID)
                             && p.tblCloseOuts_PaymentDetails.Count() == 0
                             select new
                             {
                                 c.confirmedByUserID,
                                 c.tblPurchases.pointOfSaleID,
                                 c.tblPurchases.tblPointsOfSale.pointOfSale,
                                 c.tblPurchases.tblPointsOfSale.shortName,
                                 c.tblPurchases.tblLeads.firstName,
                                 c.tblPurchases.tblLeads.lastName,
                                 c.tblServices.service,
                                 c.tblPurchaseServiceStatus.purchaseServiceStatus,
                                 c.tblPurchaseServiceDetails.FirstOrDefault().coupon,
                                 paymentDetails = c.tblPurchases.tblPaymentDetails.Where(m => p.tblCloseOuts_PaymentDetails.Count(x => x.paymentDetailsID == m.paymentDetailsID) == 0),
                                 c.confirmationDateTime
                             }).ToList();

                foreach (var s in sales.OrderBy(s => s.coupon))
                {
                    if (pco.PendingAgentCOs.Count(p => p.AgentID == s.confirmedByUserID) == 0)
                    {
                        PendingCloseOuts.PendingAgentCloseOuts agent = new PendingCloseOuts.PendingAgentCloseOuts();
                        agent.AgentID = (Guid)s.confirmedByUserID;
                        tblUserProfiles profile = db.tblUserProfiles.Single(x => x.userID == s.confirmedByUserID);
                        agent.Agent = profile.firstName + " " + profile.lastName;
                        agent.PendingCloseOuts = new List<PendingCloseOuts.CloseOutItem>();
                        PendingCloseOuts.CloseOutItem newCloseOutItem = new PendingCloseOuts.CloseOutItem();
                        newCloseOutItem.PointOfSaleID = s.pointOfSaleID;
                        newCloseOutItem.PointOfSale = s.shortName + " - " + s.pointOfSale;
                        newCloseOutItem.CloseOutDate = s.confirmationDateTime.Value.ToString("yyyy-MM-dd");
                        newCloseOutItem.PendingCoupons = new List<PendingCloseOuts.CouponItem>();
                        newCloseOutItem.PendingRefunds = new List<PendingCloseOuts.RefundItem>();
                        newCloseOutItem.PendingPayments = new List<PendingCloseOuts.PaymentItem>();

                        PendingCloseOuts.CouponItem coupon = new PendingCloseOuts.CouponItem();
                        if (s.coupon != null)
                        {
                            coupon.Folio = s.coupon.Substring(0, s.coupon.IndexOf("-"));
                        }
                        coupon.Customer = s.firstName + " " + s.lastName;
                        coupon.Service = s.service;
                        coupon.Status = s.purchaseServiceStatus;
                        newCloseOutItem.PendingCoupons.Add(coupon);

                        //mike
                        foreach (var i in s.paymentDetails)
                        {
                            PendingCloseOuts.PaymentItem payment = new PendingCloseOuts.PaymentItem
                            {
                                Purchase = i.purchaseID.ToString(),
                                Amount = i.amount.ToString()
                            };
                            newCloseOutItem.PendingPayments.Add(payment);
                        }
                        //end mike
                        agent.PendingCloseOuts.Add(newCloseOutItem);
                        pco.PendingAgentCOs.Add(agent);
                    }
                    else
                    {
                        PendingCloseOuts.PendingAgentCloseOuts agent = pco.PendingAgentCOs.FirstOrDefault(p => p.AgentID == s.confirmedByUserID);
                        if (agent.PendingCloseOuts.Count(x => x.PointOfSaleID == s.pointOfSaleID && x.CloseOutDate == s.confirmationDateTime.Value.ToString("yyyy-MM-dd")) == 0)
                        {
                            //agregar punto de venta
                            PendingCloseOuts.CloseOutItem newCloseOutItem = new PendingCloseOuts.CloseOutItem();
                            newCloseOutItem.PointOfSaleID = s.pointOfSaleID;
                            newCloseOutItem.PointOfSale = s.shortName + " - " + s.pointOfSale;
                            newCloseOutItem.CloseOutDate = s.confirmationDateTime.Value.ToString("yyyy-MM-dd");
                            newCloseOutItem.PendingCoupons = new List<PendingCloseOuts.CouponItem>();
                            newCloseOutItem.PendingRefunds = new List<PendingCloseOuts.RefundItem>();
                            newCloseOutItem.PendingPayments = new List<PendingCloseOuts.PaymentItem>();

                            PendingCloseOuts.CouponItem coupon = new PendingCloseOuts.CouponItem();
                            if (s.coupon != null)
                            {
                                coupon.Folio = s.coupon.Substring(0, s.coupon.IndexOf("-"));
                            }
                            coupon.Customer = s.firstName + " " + s.lastName;
                            coupon.Service = s.service;
                            coupon.Status = s.purchaseServiceStatus;
                            newCloseOutItem.PendingCoupons.Add(coupon);
                            //mike
                            foreach (var i in s.paymentDetails)
                            {
                                PendingCloseOuts.PaymentItem payment = new PendingCloseOuts.PaymentItem
                                {
                                    Purchase = i.purchaseID.ToString(),
                                    Amount = i.amount.ToString()
                                };
                                newCloseOutItem.PendingPayments.Add(payment);
                            }
                            //end mike
                            agent.PendingCloseOuts.Add(newCloseOutItem);
                        }
                        else
                        {
                            //editar punto de venta
                            PendingCloseOuts.CloseOutItem closeOutItem = agent.PendingCloseOuts.FirstOrDefault(x => x.PointOfSaleID == s.pointOfSaleID && x.CloseOutDate == s.confirmationDateTime.Value.ToString("yyyy-MM-dd"));
                            PendingCloseOuts.CouponItem coupon = new PendingCloseOuts.CouponItem();
                            if (s.coupon != null)
                            {
                                coupon.Folio = s.coupon.Substring(0, s.coupon.IndexOf("-"));
                            }
                            coupon.Customer = s.firstName + " " + s.lastName;
                            coupon.Service = s.service;
                            coupon.Status = s.purchaseServiceStatus;
                            closeOutItem.PendingCoupons.Add(coupon);
                        }
                    }
                }

                //cupones cancelados no cortados
                /*var cancelations = from c in db.tblPurchases_Services
                                   join purchase in db.tblPurchases on c.purchaseID equals purchase.purchaseID
                                   join pos in db.tblPointsOfSale on purchase.pointOfSaleID equals pos.pointOfSaleID
                                   join lead in db.tblLeads on purchase.leadID equals lead.leadID
                                   join service in db.tblServices on c.serviceID equals service.serviceID
                                   where (c.serviceStatusID == 4 || c.serviceStatusID == 5)
                                   && c.canceledByUserID != null
                                   && c.tblCloseOuts_Purchases.Count(x => x.canceled == true) == 0
                                   && c.cancelationDateTime < tomorrow
                                   && !pos.pointOfSale.Contains("Prueba")
                                   && purchase.purchaseStatusID != 5
                                   && purchase.isTest != true
                                   && terminals.Contains(purchase.terminalID)
                                   select new
                                   {
                                       c.canceledByUserID,
                                       pos.pointOfSaleID,
                                       pos.pointOfSale,
                                       pos.shortName,
                                       lead.firstName,
                                       lead.lastName,
                                       service.service,
                                       c.tblPurchaseServiceStatus.purchaseServiceStatus,
                                       c.tblPurchaseServiceDetails.FirstOrDefault().coupon,
                                       c.cancelationDateTime
                                   };*/

                var cancelations = from c in db.tblPurchases_Services
                                   where (c.serviceStatusID == 4 || c.serviceStatusID == 5)
                                   && c.canceledByUserID != null
                                   && c.tblCloseOuts_Purchases.Count(x => x.canceled == true) == 0
                                   && c.cancelationDateTime < tomorrow
                                   && !c.tblPurchases.tblPointsOfSale.pointOfSale.Contains("Prueba")
                                   && c.tblPurchases.purchaseStatusID != 5
                                   && c.tblPurchases.isTest != true
                                   && terminals.Contains(c.tblPurchases.terminalID)
                                   select new
                                   {
                                       c.canceledByUserID,
                                       c.tblPurchases.pointOfSaleID,
                                       c.tblPurchases.tblPointsOfSale.pointOfSale,
                                       c.tblPurchases.tblPointsOfSale.shortName,
                                       c.tblPurchases.tblLeads.firstName,
                                       c.tblPurchases.tblLeads.lastName,
                                       c.tblServices.service,
                                       c.tblPurchaseServiceStatus.purchaseServiceStatus,
                                       c.tblPurchaseServiceDetails.FirstOrDefault().coupon,
                                       c.cancelationDateTime
                                   };


                foreach (var s in cancelations)
                {
                    if (pco.PendingAgentCOs.Count(p => p.AgentID == s.canceledByUserID) == 0)
                    {
                        PendingCloseOuts.PendingAgentCloseOuts agent = new PendingCloseOuts.PendingAgentCloseOuts();
                        agent.AgentID = (Guid)s.canceledByUserID;
                        tblUserProfiles profile = db.tblUserProfiles.Single(x => x.userID == s.canceledByUserID);
                        agent.Agent = profile.firstName + " " + profile.lastName;
                        agent.PendingCloseOuts = new List<PendingCloseOuts.CloseOutItem>();
                        PendingCloseOuts.CloseOutItem newCloseOutItem = new PendingCloseOuts.CloseOutItem();
                        newCloseOutItem.PointOfSaleID = s.pointOfSaleID;
                        newCloseOutItem.PointOfSale = s.shortName + " - " + s.pointOfSale;
                        newCloseOutItem.CloseOutDate = s.cancelationDateTime.Value.ToString("yyyy-MM-dd");
                        newCloseOutItem.PendingCoupons = new List<PendingCloseOuts.CouponItem>();
                        newCloseOutItem.PendingRefunds = new List<PendingCloseOuts.RefundItem>();
                        PendingCloseOuts.CouponItem coupon = new PendingCloseOuts.CouponItem();
                        if (s.coupon != null)
                        {
                            coupon.Folio = s.coupon.Substring(0, s.coupon.IndexOf("-"));
                        }
                        coupon.Customer = s.firstName + " " + s.lastName;
                        coupon.Service = s.service;
                        coupon.Status = s.purchaseServiceStatus;
                        newCloseOutItem.PendingCoupons.Add(coupon);
                        agent.PendingCloseOuts.Add(newCloseOutItem);
                        pco.PendingAgentCOs.Add(agent);
                    }
                    else
                    {
                        PendingCloseOuts.PendingAgentCloseOuts agent = pco.PendingAgentCOs.FirstOrDefault(p => p.AgentID == s.canceledByUserID);
                        if (agent.PendingCloseOuts.Count(x => x.PointOfSaleID == s.pointOfSaleID && x.CloseOutDate == s.cancelationDateTime.Value.ToString("yyyy-MM-dd")) == 0)
                        {
                            //agregar punto de venta
                            PendingCloseOuts.CloseOutItem newCloseOutItem = new PendingCloseOuts.CloseOutItem();
                            newCloseOutItem.PointOfSaleID = s.pointOfSaleID;
                            newCloseOutItem.PointOfSale = s.shortName + " - " + s.pointOfSale;
                            newCloseOutItem.CloseOutDate = s.cancelationDateTime.Value.ToString("yyyy-MM-dd");
                            newCloseOutItem.PendingCoupons = new List<PendingCloseOuts.CouponItem>();
                            newCloseOutItem.PendingRefunds = new List<PendingCloseOuts.RefundItem>();

                            PendingCloseOuts.CouponItem coupon = new PendingCloseOuts.CouponItem();
                            if (s.coupon != null)
                            {
                                coupon.Folio = s.coupon.Substring(0, s.coupon.IndexOf("-"));
                            }
                            coupon.Customer = s.firstName + " " + s.lastName;
                            coupon.Service = s.service;
                            coupon.Status = s.purchaseServiceStatus;
                            newCloseOutItem.PendingCoupons.Add(coupon);
                            agent.PendingCloseOuts.Add(newCloseOutItem);
                        }
                        else
                        {
                            //editar punto de venta
                            PendingCloseOuts.CloseOutItem closeOutItem = agent.PendingCloseOuts.FirstOrDefault(x => x.PointOfSaleID == s.pointOfSaleID && x.CloseOutDate == s.cancelationDateTime.Value.ToString("yyyy-MM-dd"));
                            PendingCloseOuts.CouponItem coupon = new PendingCloseOuts.CouponItem();
                            if (s.coupon != null)
                            {
                                coupon.Folio = s.coupon.Substring(0, s.coupon.IndexOf("-"));
                            }
                            coupon.Customer = s.firstName + " " + s.lastName;
                            coupon.Service = s.service;
                            coupon.Status = s.purchaseServiceStatus;
                            closeOutItem.PendingCoupons.Add(coupon);
                        }
                    }
                }

                //refunds realizados no cortados
                var refunds = (from r in db.tblPaymentDetails
                               join purchase in db.tblPurchases on r.purchaseID equals purchase.purchaseID
                               join pos in db.tblPointsOfSale on purchase.pointOfSaleID equals pos.pointOfSaleID
                               join lead in db.tblLeads on purchase.leadID equals lead.leadID
                               join transaction in db.tblMoneyTransactions on r.moneyTransactionID equals transaction.moneyTransactionID
                               join currency in db.tblCurrencies on r.currencyID equals currency.currencyID
                               join closeout in db.tblCloseOuts_PaymentDetails on r.paymentDetailsID equals closeout.paymentDetailsID
                               where terminals.Contains(purchase.terminalID)
                               && (r.deleted == null || r.deleted == false)
                               && r.dateSaved < tomorrow
                               && closeout == null
                               && !pos.pointOfSale.Contains("Prueba")
                               && purchase.purchaseStatusID != 5
                               && purchase.isTest != true
                               && transaction.transactionTypeID == 2
                               select new
                               {
                                   r.savedByUserID,
                                   pos.pointOfSaleID,
                                   pos.pointOfSale,
                                   pos.shortName,
                                   lead.firstName,
                                   lead.lastName,
                                   r.amount,
                                   currency.currencyCode,
                                   r.paymentType,
                                   r.dateSaved
                               }).Take(50);

                foreach (var s in refunds)
                {
                    if (pco.PendingAgentCOs.Count(p => p.AgentID == s.savedByUserID) == 0)
                    {
                        PendingCloseOuts.PendingAgentCloseOuts agent = new PendingCloseOuts.PendingAgentCloseOuts();
                        agent.AgentID = (Guid)s.savedByUserID;
                        tblUserProfiles profile = db.tblUserProfiles.Single(x => x.userID == s.savedByUserID);
                        agent.Agent = profile.firstName + " " + profile.lastName;
                        agent.PendingCloseOuts = new List<PendingCloseOuts.CloseOutItem>();
                        PendingCloseOuts.CloseOutItem newCloseOutItem = new PendingCloseOuts.CloseOutItem();
                        newCloseOutItem.PointOfSaleID = s.pointOfSaleID;
                        newCloseOutItem.PointOfSale = s.shortName + " - " + s.pointOfSale;
                        newCloseOutItem.CloseOutDate = s.dateSaved.ToString("yyyy-MM-dd");
                        newCloseOutItem.PendingCoupons = new List<PendingCloseOuts.CouponItem>();
                        newCloseOutItem.PendingRefunds = new List<PendingCloseOuts.RefundItem>();

                        PendingCloseOuts.RefundItem refund = new PendingCloseOuts.RefundItem();
                        Money totalRefund = new Money();
                        totalRefund.Amount = s.amount;
                        totalRefund.Currency = s.currencyCode;
                        refund.Refund = totalRefund;
                        refund.Customer = s.firstName + " " + s.lastName;
                        refund.Type = Utils.GeneralFunctions.PaymentTypes[s.paymentType.ToString()];
                        newCloseOutItem.PendingRefunds.Add(refund);

                        agent.PendingCloseOuts.Add(newCloseOutItem);
                        pco.PendingAgentCOs.Add(agent);
                    }
                    else
                    {
                        PendingCloseOuts.PendingAgentCloseOuts agent = pco.PendingAgentCOs.FirstOrDefault(p => p.AgentID == s.savedByUserID);
                        if (agent.PendingCloseOuts.Count(x => x.PointOfSaleID == s.pointOfSaleID && x.CloseOutDate == s.dateSaved.ToString("yyyy-MM-dd")) == 0)
                        {
                            //agregar punto de venta
                            PendingCloseOuts.CloseOutItem newCloseOutItem = new PendingCloseOuts.CloseOutItem();
                            newCloseOutItem.PointOfSaleID = s.pointOfSaleID;
                            newCloseOutItem.PointOfSale = s.shortName + " - " + s.pointOfSale;
                            newCloseOutItem.CloseOutDate = s.dateSaved.ToString("yyyy-MM-dd");
                            newCloseOutItem.PendingCoupons = new List<PendingCloseOuts.CouponItem>();
                            newCloseOutItem.PendingRefunds = new List<PendingCloseOuts.RefundItem>();

                            PendingCloseOuts.RefundItem refund = new PendingCloseOuts.RefundItem();
                            Money totalRefund = new Money();
                            totalRefund.Amount = s.amount;
                            totalRefund.Currency = s.currencyCode;
                            refund.Refund = totalRefund;
                            refund.Customer = s.firstName + " " + s.lastName;
                            refund.Type = Utils.GeneralFunctions.PaymentTypes[s.paymentType.ToString()];
                            newCloseOutItem.PendingRefunds.Add(refund);

                            agent.PendingCloseOuts.Add(newCloseOutItem);
                        }
                        else
                        {
                            //editar punto de venta
                            PendingCloseOuts.CloseOutItem closeOutItem = agent.PendingCloseOuts.FirstOrDefault(x => x.PointOfSaleID == s.pointOfSaleID && x.CloseOutDate == s.dateSaved.ToString("yyyy-MM-dd"));

                            PendingCloseOuts.RefundItem refund = new PendingCloseOuts.RefundItem();
                            Money totalRefund = new Money();
                            totalRefund.Amount = s.amount;
                            totalRefund.Currency = s.currencyCode;
                            refund.Refund = totalRefund;
                            refund.Customer = s.firstName + " " + s.lastName;
                            refund.Type = Utils.GeneralFunctions.PaymentTypes[s.paymentType.ToString()];
                            closeOutItem.PendingRefunds.Add(refund);
                        }
                    }
                }

                //procesar
                pco.PendingAgentCOs = pco.PendingAgentCOs.OrderBy(x => x.Agent).ToList();
            }

            return pco;
        }

        public static HostessStatus GetHostessStatus()
        {
            ePlatEntities db = new ePlatEntities();
            HostessStatus status = new HostessStatus();
            status.Programs = new List<HostessStatus.ProgramStatus>();
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];

            var Arrivals = from a in db.tblArrivals
                           join program in db.tblProspectationPrograms on a.programID equals program.programID
                           into arr_program
                           from program in arr_program.DefaultIfEmpty()
                           join bs in db.tblBookingStatus on a.bookingStatusID equals bs.bookingStatusID
                           into arr_bs
                           from bs in arr_bs.DefaultIfEmpty()
                           where a.terminalID == terminalID
                           && a.arrivalDate >= today
                           && a.arrivalDate < tomorrow
                           && !a.reservationStatus.Contains("CA")
                           select new HostessStatus.ArrivalStatus()
                           {
                               arrivalID = a.arrivalID,
                               programID = program.programID,
                               program = program.program,
                               reservationStatus = a.reservationStatus,
                               bookingStatusID = bs.bookingStatusID,
                               bookingStatus = bs.bookingStatus
                           };

            int? workgroup = session.WorkGroupID;
            var bookingStatusWorkGroup = from bs in db.tblSysWorkGroups_BookingStatus
                                         where bs.sysWorkGroupID == workgroup
                                         orderby bs.orderIndex
                                         select new
                                         {
                                             bs.bookingStatusID,
                                             bs.tblBookingStatus.bookingStatus
                                         };
            int notContactedID = bookingStatusWorkGroup.FirstOrDefault(x => x.bookingStatus == "Not Contacted").bookingStatusID;

            foreach (var arrival in Arrivals)
            {
                if (arrival.programID != null)
                {
                    if (status.Programs.Count(x => x.ProgramID == arrival.programID) == 0)
                    {
                        HostessStatus.ProgramStatus program = new HostessStatus.ProgramStatus();
                        if (arrival.programID == null)
                        {
                            program.ProgramID = 0;
                            program.Program = "Unknown";
                        }
                        else
                        {
                            program.ProgramID = arrival.programID;
                            program.Program = arrival.program;
                        }
                        program.Reservations = 0;
                        program.CheckedIn = 0;
                        program.CheckedInPercentage = 0;
                        program.Status = new List<HostessStatus.BookingStatusItem>();

                        foreach (var bs in bookingStatusWorkGroup)
                        {
                            HostessStatus.BookingStatusItem newStatus = new HostessStatus.BookingStatusItem();
                            newStatus.BookingStatusID = bs.bookingStatusID;
                            newStatus.BookingStatus = bs.bookingStatus;
                            newStatus.Amount = 0;
                            newStatus.Percentage = 0;
                            program.Status.Add(newStatus);
                        }

                        status.Programs.Add(program);
                    }
                    var currentProgram = status.Programs.FirstOrDefault(x => x.ProgramID == arrival.programID);
                    currentProgram.Reservations += 1;

                    if (arrival.reservationStatus != "A" && arrival.reservationStatus != "CA")
                    {
                        currentProgram.CheckedIn += 1;
                        currentProgram.CheckedInPercentage = decimal.Round(currentProgram.CheckedIn * 100 / currentProgram.Reservations, 2);

                        //sumar status
                        var currentStatus = currentProgram.Status.FirstOrDefault(x => x.BookingStatusID == (arrival.bookingStatusID == null ? notContactedID : arrival.bookingStatusID));
                        currentStatus.Amount += 1;
                    }
                }
            }

            status.Programs = status.Programs.OrderBy(z => z.Program).ToList();
            foreach (var prog in status.Programs)
            {
                foreach (var sta in prog.Status)
                {
                    if (prog.CheckedIn > 0)
                    {
                        sta.Percentage = sta.Amount * 100 / prog.CheckedIn;
                    }
                }

            }

            return status;
        }

        public static CloseOutsWithErrors GetCloseOutsWithErrors()
        {
            ePlatEntities db = new ePlatEntities();
            CloseOutsWithErrors errors = new CloseOutsWithErrors();


            return errors;
        }

        //public Task GetCloseOutPendingCoupons(List<long> CloseOutIDs)
        //{


        //    return Task.FromResult(Coupons);
        //}

        public static OnlineGoals GetOnlineGoals()
        {
            ePlatEntities db = new ePlatEntities();
            OnlineGoals onlineGoals = new OnlineGoals();
            onlineGoals.Terminals = new List<OnlineGoals.OnlineTerminalGoal>();

            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            DateTime begin = DateTime.Parse(DateTime.Today.Year + "-" + DateTime.Today.Month + "-01");
            DateTime end = DateTime.Parse(DateTime.Today.AddMonths(1).Year + "-" + DateTime.Today.AddMonths(1).Month + "-01");
            DateTime beginLY = begin.AddYears(-1);
            DateTime endLY = end.AddYears(-1);

            var Terminals = from t in db.tblTerminals
                            where terminals.Contains(t.terminalID)
                            select new
                            {
                                t.terminalID,
                                t.terminal
                            };

            foreach (var tid in terminals)
            {
                int pointOfSaleID = 0;
                switch (tid)
                {
                    case 5: //vallarta : F
                        pointOfSaleID = 43;
                        break;
                    case 6: //cabo : W
                        pointOfSaleID = 70;
                        break;
                    case 7: //cancun : E
                        pointOfSaleID = 51;
                        break;
                    case 8: //loreto : L
                        pointOfSaleID = 81;
                        break;
                }

                if (pointOfSaleID != 0)
                {
                    List<OnlineGoals.OnlineDailySalesAndMarketing> LYSales = GetSalesAndMarketing(db, tid, beginLY, endLY, pointOfSaleID);

                    List<OnlineGoals.OnlineDailySalesAndMarketing> CYSales = GetSalesAndMarketing(db, tid, begin, end, pointOfSaleID);

                    OnlineGoals.OnlineTerminalGoal terminal = new OnlineGoals.OnlineTerminalGoal();
                    terminal.TerminalID = tid;
                    terminal.Terminal = Terminals.FirstOrDefault(x => x.terminalID == tid).terminal;
                    terminal.CurrencyID = 1;

                    terminal.LastYear = beginLY.Year;
                    terminal.CurrentYear = begin.Year;
                    terminal.MonthName = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(begin.Month);

                    terminal.LYMonthlySales = LYSales.Sum(x => x.USDTotalSales);
                    terminal.CYMonthlyGoal = decimal.Round(terminal.LYMonthlySales * (decimal)1.10, 2);
                    terminal.CYCurrentTotal = CYSales.Sum(x => x.USDTotalSales);
                    terminal.GoalPercentage = terminal.CYMonthlyGoal > 0 ? decimal.Round(terminal.CYCurrentTotal * 100 / terminal.CYMonthlyGoal, 2) : 0;
                    terminal.AverageSale = CYSales.Sum(x => x.ConfirmedCoupons) > 0 ? terminal.CYCurrentTotal / CYSales.Sum(x => x.ConfirmedCoupons) : 0;
                    decimal goalProgress = terminal.CYMonthlyGoal / DateTime.DaysInMonth(begin.Year, begin.Month) * (DateTime.Today.Day - 1);
                    terminal.GoalProgress = goalProgress;
                    terminal.Difference = terminal.CYCurrentTotal - goalProgress;


                    terminal.USDCurrentSales = CYSales.Sum(x => x.USDSales);
                    terminal.MXNCurrentSales = CYSales.Sum(x => x.MXNSales);
                    terminal.MXNUSDCurrentSales = CYSales.Sum(x => x.USDTotalSales) - CYSales.Sum(x => x.USDSales);

                    terminal.LYUSDSales = LYSales.Sum(x => x.USDSales);
                    terminal.CYUSDSales = CYSales.Sum(x => x.USDSales);
                    terminal.LYMXNSales = LYSales.Sum(x => x.MXNSales);
                    terminal.CYMXNSales = CYSales.Sum(x => x.MXNSales);

                    onlineGoals.Terminals.Add(terminal);
                }
            }

            return onlineGoals;
        }

        public static List<OnlineGoals.OnlineDailySalesAndMarketing> GetSalesAndMarketing(ePlatEntities db, long terminalID, DateTime begin, DateTime end, int pointOfSaleID)
        {
            List<OnlineGoals.OnlineDailySalesAndMarketing> salesAndMarketing = new List<OnlineGoals.OnlineDailySalesAndMarketing>();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            if (end > DateTime.Now)
            {
                end = DateTime.Today;
            }

            var Cache = (from d in db.tblWidgetsCache
                         where d.widgetID == 2
                         && d.date >= begin
                         && d.date < end
                         && d.terminalID == terminalID
                         select d).ToList();

            //List<string> GAdsAccounts = new List<string>();
            //GoogleAdsViewModel.Reports.SearchAdsInvestment search = new GoogleAdsViewModel.Reports.SearchAdsInvestment();
            //search.Search_I_FromDate = begin.ToString("yyyy-MM-dd");
            //search.Search_F_ToDate = end.AddDays(-1).ToString("yyyy-MM-dd");
            //search.Search_Accounts = new string[] { "335-439-2170" };

            //GoogleAdsViewModel.Reports.AdsInvestmentReport adsInvestment = GoogleAdsDataModel.Reports.GetAdsInvestments(search);

            while (begin < end)
            {
                if (Cache.FirstOrDefault(x => x.date == begin) == null)
                {
                    var POS = (from p in db.tblPointsOfSale
                               where p.pointOfSaleID == pointOfSaleID
                               select new
                               {
                                   p.pointOfSaleID,
                                   p.pointOfSale
                               }).FirstOrDefault();

                    //generar
                    OnlineGoals.OnlineDailySalesAndMarketing dsm = new OnlineGoals.OnlineDailySalesAndMarketing();

                    dsm.Date = begin;
                    dsm.PointOfSaleID = pointOfSaleID;
                    dsm.PointOfSale = POS.pointOfSale;
                    dsm.USDSales = 0;
                    dsm.MXNSales = 0;
                    dsm.USDTotalSales = 0;
                    dsm.USDMarketingSpend = 0;
                    dsm.ExchangeRate = MasterChartDataModel.Purchases.GetSpecificRate
    (begin, "USD", terminalID);
                    dsm.ConfirmedCoupons = 0;
                    dsm.ConfirmedCouponsIDs = new List<long>();
                    dsm.Customers = 0;
                    dsm.CustomerIDs = new List<Guid>();

                    DateTime nextDate = begin.AddDays(1);
                    var couponDetails = from detail in db.tblPurchaseServiceDetails
                                        join coupon in db.tblPurchases_Services on detail.purchase_ServiceID equals coupon.purchase_ServiceID
                                        join purchase in db.tblPurchases on coupon.purchaseID equals purchase.purchaseID
                                        join profile in db.tblUserProfiles on coupon.confirmedByUserID equals profile.userID
                                        into coupon_profile
                                        from profile in coupon_profile.DefaultIfEmpty()
                                        where purchase.terminalID == terminalID
                                        && coupon.serviceStatusID > 2
                                        && purchase.pointOfSaleID == pointOfSaleID
                                        && ((coupon.confirmationDateTime > begin && coupon.confirmationDateTime < nextDate) || (coupon.cancelationDateTime > begin && coupon.cancelationDateTime < nextDate))
                                        select new
                                        {
                                            detail.quantity,
                                            detail.customPrice,
                                            detail.dealPrice,
                                            detail.priceTypeID,
                                            detail.purchaseServiceDetailID,
                                            coupon.serviceStatusID,
                                            coupon.currencyID,
                                            coupon.confirmedByUserID,
                                            coupon.purchase_ServiceID,
                                            profile.firstName,
                                            profile.lastName,
                                            purchase.pointOfSaleID,
                                            purchase.spiMarketingProgram,
                                            purchase.leadID
                                        };

                    foreach (var detail in couponDetails)
                    {
                        decimal unitTotal = 0;
                        if (detail.dealPrice != null)
                        {
                            unitTotal = decimal.Round(PromoDataModel.ApplyPromo(detail.quantity * (decimal)detail.dealPrice, detail.purchaseServiceDetailID), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            unitTotal = decimal.Round(PromoDataModel.ApplyPromo(detail.quantity * (decimal)detail.customPrice, detail.purchaseServiceDetailID), 2, MidpointRounding.AwayFromZero);
                        }

                        if (detail.serviceStatusID == 3 || detail.serviceStatusID == 6)
                        {
                            if (detail.currencyID == 1)
                            {
                                dsm.USDSales += decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero);
                                dsm.USDTotalSales += decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero);
                            }
                            else if (detail.currencyID == 2)
                            {
                                dsm.MXNSales += decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero);
                                dsm.USDTotalSales += decimal.Round(unitTotal / dsm.ExchangeRate, 2, MidpointRounding.AwayFromZero);
                            }
                        }
                        else if (detail.serviceStatusID == 5)
                        {
                            if (detail.currencyID == 1)
                            {
                                dsm.USDSales -= decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero);
                                dsm.USDTotalSales -= decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero);
                            }
                            else if (detail.currencyID == 2)
                            {
                                dsm.MXNSales -= decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero);
                                dsm.USDTotalSales -= decimal.Round(unitTotal / dsm.ExchangeRate, 2, MidpointRounding.AwayFromZero);
                            }
                        }

                        if (!dsm.ConfirmedCouponsIDs.Contains(detail.purchase_ServiceID))
                        {
                            dsm.ConfirmedCouponsIDs.Add(detail.purchase_ServiceID);
                            dsm.ConfirmedCoupons++;
                        }
                        if (!dsm.CustomerIDs.Contains(detail.leadID))
                        {
                            dsm.CustomerIDs.Add(detail.leadID);
                            dsm.Customers++;
                        }
                    }

                    //obtener gasto de marketing




                    tblWidgetsCache wcache = new tblWidgetsCache();
                    wcache.widgetID = 2;
                    wcache.terminalID = terminalID;
                    wcache.widgetJSON = js.Serialize(dsm);
                    wcache.date = begin;
                    wcache.dateSaved = DateTime.Now;
                    db.tblWidgetsCache.AddObject(wcache);

                    salesAndMarketing.Add(dsm);
                }
                else
                {
                    //deserializar
                    OnlineGoals.OnlineDailySalesAndMarketing dsm = js.Deserialize<OnlineGoals.OnlineDailySalesAndMarketing>(Cache.FirstOrDefault(x => x.date == begin).widgetJSON);
                    salesAndMarketing.Add(dsm);
                }

                begin = begin.AddDays(1);
            }

            db.SaveChanges();

            return salesAndMarketing;
        }

        public static PendingCache GetPendingCache()
        {
            ePlatEntities db = new ePlatEntities();
            PendingCache pending = new PendingCache();
            pending.PendingCacheTerminals = new List<PendingCache.PendingTerminalCache>();

            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            DateTime startDate = DateTime.Today.AddDays(-40);
            DateTime endDate = DateTime.Today;

            //DateTime startDate = DateTime.Parse("2018-07-28");
            //DateTime endDate = DateTime.Parse("2018-07-30");

            var CloseOuts = (from c in db.tblCloseOuts
                             join pos in db.tblPointsOfSale on c.pointOfSaleID equals pos.pointOfSaleID
                             join t in db.tblTerminals on c.terminalID equals t.terminalID
                             where terminals.Contains(c.terminalID)
                             && c.closeOutDate >= startDate
                             && c.closeOutDate < endDate
                             orderby t.terminal, pos.shortName
                             select new
                             {
                                 c.terminalID,
                                 t.terminal,
                                 c.closeOutID,
                                 c.closeOutDate,
                                 pos.shortName,
                                 pos.pointOfSale
                             }).ToList();

            var CloseOutIDs = CloseOuts.Select(x => x.closeOutID).ToList();

            db.CommandTimeout = 360;
            List<PendingCache.CouponsQuery> Coupons = (from p in db.tblCloseOuts_Purchases
                                                       join c in db.tblCouponInfo
                                                       on p.purchase_ServiceID equals c.purchase_ServiceID
                                                       into p_c
                                                       from c in p_c.DefaultIfEmpty()
                                                       join s in db.tblPurchases_Services
                                                       on p.purchase_ServiceID equals s.purchase_ServiceID
                                                       into p_s
                                                       from s in p_s.DefaultIfEmpty()
                                                       where CloseOutIDs.Contains(p.closeOutID)
                                                       select new PendingCache.CouponsQuery()
                                                       {
                                                           closeOutID = p.closeOutID,
                                                           purchase_ServiceID = p.purchase_ServiceID,
                                                           folio = c.folio,
                                                           cancelationDateTime = s.cancelationDateTime,
                                                           auditDate = s.auditDate,
                                                           dateSaved = p.dateSaved,
                                                           dateLastUpdate = c.dateLastUpdate,
                                                           dateGenerated = c.dateGenerated
                                                       }).ToList();

            db.CommandTimeout = null;

            Coupons = (from c in Coupons
                       where c.folio == null
                            || ((c.dateLastUpdate == null && c.dateGenerated < c.cancelationDateTime) || (c.dateLastUpdate != null && c.dateLastUpdate < c.cancelationDateTime))
                            || ((c.dateLastUpdate == null && c.dateGenerated < c.auditDate) || (c.dateLastUpdate != null && c.dateLastUpdate < c.auditDate))
                            || ((c.dateLastUpdate == null && c.dateGenerated < c.dateSaved) || (c.dateLastUpdate != null && c.dateLastUpdate < c.dateSaved))
                       select c).ToList();

            pending.Purchase_ServiceIDs = String.Join(",", Coupons.Select(x => x.purchase_ServiceID).ToList());
            var CloseOutIDsToUpdate = Coupons.Select(x => x.closeOutID).Distinct();

            foreach (var co in CloseOuts.Where(x => CloseOutIDsToUpdate.Contains(x.closeOutID)).OrderBy(x => x.terminalID).ThenBy(x => x.closeOutDate).ThenBy(x => x.pointOfSale))
            {
                if (pending.PendingCacheTerminals.Count(x => x.TerminalID == co.terminalID) == 0)
                {
                    pending.PendingCacheTerminals.Add(new PendingCache.PendingTerminalCache()
                    {
                        TerminalID = co.terminalID,
                        Terminal = co.terminal,
                        PendingCloseOuts = new List<PendingCache.PendingCloseOutCache>()
                    });
                }


                if (pending.PendingCacheTerminals.FirstOrDefault(x => x.TerminalID == co.terminalID).PendingCloseOuts.Count(x => x.CloseOutID == co.closeOutID) == 0)
                {
                    pending.PendingCacheTerminals.FirstOrDefault(x => x.TerminalID == co.terminalID).PendingCloseOuts.Add(new PendingCache.PendingCloseOutCache()
                    {
                        CloseOutID = co.closeOutID,
                        CloseOutDate = co.closeOutDate,
                        PointOfSale = co.shortName + " - " + co.pointOfSale,
                        NotCachedCoupons = 0,
                        OutOfDateCoupons = 0
                    });
                }

                //cupones
                pending.PendingCacheTerminals.FirstOrDefault(x => x.TerminalID == co.terminalID).PendingCloseOuts.FirstOrDefault(x => x.CloseOutID == co.closeOutID).NotCachedCoupons = Coupons.Count(x => x.closeOutID == co.closeOutID && x.folio == null);

                pending.PendingCacheTerminals.FirstOrDefault(x => x.TerminalID == co.terminalID).PendingCloseOuts.FirstOrDefault(x => x.CloseOutID == co.closeOutID).OutOfDateCoupons = Coupons.Count(x => x.closeOutID == co.closeOutID && x.folio != null);
            }

            return pending;
        }

        public static ExchangeRatesStatus GetExchangeRatesStatus()
        {
            ePlatEntities db = new ePlatEntities();
            ExchangeRatesStatus status = new ExchangeRatesStatus();
            status.TerminalsExchanges = new List<ExchangeRatesStatus.TerminalExchangeRates>();

            List<long> sTerminals = new List<long> { };
            sTerminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToList();

            var ExchangeRates = from x in db.tblExchangeRates
                                where sTerminals.Contains((long)x.terminalID)
                                && x.providerID == null
                                && (x.toDate > DateTime.Now || x.toDate == null)
                                select x;

            var Terminals = from t in db.tblTerminals
                            where sTerminals.Contains(t.terminalID)
                            select new
                            {
                                t.terminalID,
                                t.terminal
                            };

            foreach (var terminal in Terminals)
            {
                ExchangeRatesStatus.TerminalExchangeRates newTerminal = new ExchangeRatesStatus.TerminalExchangeRates();
                newTerminal.Terminal = terminal.terminal;
                newTerminal.ExchangeRates = new List<ExchangeRatesStatus.ExchangeRateItem>();

                //buscar exchange rate dólares default
                var erUsdDefault = ExchangeRates.FirstOrDefault(x => x.terminalID == terminal.terminalID && x.exchangeRateTypeID == 1 && x.fromCurrencyID == 1 && x.toCurrencyID == 2 && x.tblExchangeRates_PointsOfSales.Count() == 0);

                ExchangeRatesStatus.ExchangeRateItem erUsdDefaultItem = new ExchangeRatesStatus.ExchangeRateItem();
                erUsdDefaultItem.FromCurrencyCode = "USD";
                erUsdDefaultItem.ToCurrencyCode = "MXN";
                erUsdDefaultItem.ExchangeType = "General ER";
                erUsdDefaultItem.PoS = "Default";
                if (erUsdDefault != null)
                {
                    //datos de er
                    erUsdDefaultItem.ExchangeRate = erUsdDefault.exchangeRate;
                    erUsdDefaultItem.ExchangeRateID = erUsdDefault.exchangeRateID;
                    erUsdDefaultItem.Vigency = "From " + erUsdDefault.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt") + " " + (erUsdDefault.toDate != null ? "To " + erUsdDefault.toDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "Permanent");
                    if (erUsdDefault.toDate != null)
                    {
                        //tiene un vencimiento
                        erUsdDefaultItem.DaysToExpire = ((DateTime)erUsdDefault.toDate.Value - DateTime.Today).TotalDays;
                    }
                    else
                    {
                        //es permanente
                        erUsdDefaultItem.DaysToExpire = 1000;
                    }
                }
                else
                {
                    //expirado
                    erUsdDefaultItem.DaysToExpire = -1;
                }

                newTerminal.ExchangeRates.Add(erUsdDefaultItem);

                //buscar exchange rate dólares para ciertos puntos de venta
                var erUsdPos = ExchangeRates.FirstOrDefault(x => x.terminalID == terminal.terminalID && x.exchangeRateTypeID == 1 && x.fromCurrencyID == 1 && x.toCurrencyID == 2 && x.tblExchangeRates_PointsOfSales.Count() > 0);

                ExchangeRatesStatus.ExchangeRateItem erUsdPosItem = new ExchangeRatesStatus.ExchangeRateItem();
                erUsdPosItem.FromCurrencyCode = "USD";
                erUsdPosItem.ToCurrencyCode = "MXN";
                erUsdPosItem.ExchangeType = "General ER";
                erUsdPosItem.PoS = "Specific Points Of Sale";
                if (erUsdPos != null)
                {
                    //datos de er
                    erUsdPosItem.ExchangeRate = erUsdPos.exchangeRate;
                    erUsdPosItem.ExchangeRateID = erUsdPos.exchangeRateID;
                    erUsdPosItem.Vigency = "From " + erUsdPos.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt") + " " + (erUsdPos.toDate != null ? "To " + erUsdPos.toDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "Permanent");
                    if (erUsdPos.toDate != null)
                    {
                        //tiene un vencimiento
                        erUsdPosItem.DaysToExpire = ((DateTime)erUsdPos.toDate.Value - DateTime.Today).TotalDays;
                    }
                    else
                    {
                        //es permanente
                        erUsdPosItem.DaysToExpire = 1000;
                    }
                }
                else
                {
                    //expirado
                    erUsdPosItem.DaysToExpire = -1;
                }
                newTerminal.ExchangeRates.Add(erUsdPosItem);

                //buscar exchange rate canadiense default
                var erCanDefault = ExchangeRates.FirstOrDefault(x => x.terminalID == terminal.terminalID && x.exchangeRateTypeID == 1 && x.fromCurrencyID == 3 && x.toCurrencyID == 2 && x.tblExchangeRates_PointsOfSales.Count() == 0);

                ExchangeRatesStatus.ExchangeRateItem erCanDefaultItem = new ExchangeRatesStatus.ExchangeRateItem();
                erCanDefaultItem.FromCurrencyCode = "CAN";
                erCanDefaultItem.ToCurrencyCode = "MXN";
                erCanDefaultItem.ExchangeType = "General ER";
                erCanDefaultItem.PoS = "Default";
                if (erCanDefault != null)
                {
                    //datos de er
                    erCanDefaultItem.ExchangeRate = erCanDefault.exchangeRate;
                    erCanDefaultItem.ExchangeRateID = erCanDefault.exchangeRateID;
                    erCanDefaultItem.Vigency = "From " + erCanDefault.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt") + " " + (erCanDefault.toDate != null ? "To " + erCanDefault.toDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "Permanent");
                    if (erCanDefault.toDate != null)
                    {
                        //tiene un vencimiento
                        erCanDefaultItem.DaysToExpire = ((DateTime)erCanDefault.toDate.Value - DateTime.Today).TotalDays;
                    }
                    else
                    {
                        //es permanente
                        erCanDefaultItem.DaysToExpire = 1000;
                    }
                }
                else
                {
                    //expirado
                    erCanDefaultItem.DaysToExpire = -1;
                }
                newTerminal.ExchangeRates.Add(erCanDefaultItem);

                //exchange rates de proveedor que están próximos a vencer dentro de los siguientes 3 días - PENDING


                status.TerminalsExchanges.Add(newTerminal);
            }

            return status;
        }

        public static AvailabilityStatus GetAvailabilityAlerts()
        {
            ePlatEntities db = new ePlatEntities();
            AvailabilityStatus status = new AvailabilityStatus();

            status.TerminalsAvailability = new List<AvailabilityStatus.TerminalAvailability>();

            List<long> sTerminals = new List<long> { };
            sTerminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToList();

            var Terminals = from t in db.tblTerminals
                            where sTerminals.Contains(t.terminalID)
                            select new
                            {
                                t.terminalID,
                                t.terminal
                            };

            //actividades con horarios próximos a vencer en 15 o menos días
            DateTime fiveDays = DateTime.Now.AddDays(8);
            var AvailabilityQ = from s in db.tblWeeklyAvailability
                                join service in db.tblServices on s.serviceID equals service.serviceID
                                into s_service
                                from service in s_service.DefaultIfEmpty()
                                join provider in db.tblProviders on service.providerID equals provider.providerID
                                into service_provider
                                from provider in service_provider.DefaultIfEmpty()
                                where sTerminals.Contains(service.originalTerminalID) && s.toDate > DateTime.Now && s.toDate <= fiveDays && service.deleted == false
                                && provider.isActive
                                select new
                                {
                                    service.serviceID,
                                    service.service,
                                    service.originalTerminalID,
                                    s.weeklyAvailabilityID,
                                    s.hour,
                                    s.toHour,
                                    s.fromDate,
                                    s.toDate,
                                    provider.comercialName,
                                    descriptions = service.tblServiceDescriptions.Count()
                                };

            //actividades con descripciones activas pero sin horarios vigentes
            var OnlineAvailabilityQ = from o in db.tblServices
                                      join provider in db.tblProviders on o.providerID equals provider.providerID
                                      into o_provider
                                      from provider in o_provider.DefaultIfEmpty()
                                      where sTerminals.Contains(o.originalTerminalID) && o.tblWeeklyAvailability.Count(x => x.permanent_ == true || x.toDate > DateTime.Now) == 0
                                      && o.deleted == false
                                      && o.tblServiceDescriptions.Count(x => x.active == true) > 0
                                      && o.tblCategories_Services.Count(x => x.tblCategories.showOnWebsite == true) > 0
                                      && provider.isActive
                                      select new
                                      {
                                          o.serviceID,
                                          o.service,
                                          o.originalTerminalID,
                                          provider.comercialName,
                                          descriptions = o.tblServiceDescriptions.Count()
                                      };


            //actividades con precios próximos a vencer en 5 o menos días
            var PricesQ = from p in db.tblPrices
                          join service in db.tblServices on p.itemID equals service.serviceID
                          join provider in db.tblProviders on service.providerID equals provider.providerID
                          into service_provider
                          from provider in service_provider.DefaultIfEmpty()
                          where sTerminals.Contains(p.terminalID) && p.toDate > DateTime.Now && p.toDate <= fiveDays && service.deleted == false && p.sysItemTypeID == 1
                          && provider.isActive
                          select new
                          {
                              service.serviceID,
                              service.service,
                              service.originalTerminalID,
                              provider.comercialName,
                              descriptions = service.tblServiceDescriptions.Count(),
                              p.priceID,
                              p.price,
                              p.currencyID,
                              p.fromDate,
                              p.toDate
                          };

            //actividades activas sin precios
            var ActivePricesQ = (from p in db.tblPrices
                                 where sTerminals.Contains(p.terminalID)
                                 && (p.toDate > DateTime.Now || p.permanent_ == true)
                                 select p.itemID).Distinct();

            var ActiveServicesNoPriceQ = from p in db.tblServices
                                         join provider in db.tblProviders on p.providerID equals provider.providerID
                                         into p_provider
                                         from provider in p_provider.DefaultIfEmpty()
                                         where sTerminals.Contains(p.originalTerminalID)
                                         && p.deleted == false
                                         && !ActivePricesQ.Contains(p.serviceID)
                                         && provider.isActive
                                         select new
                                         {
                                             p.serviceID,
                                             p.service,
                                             p.originalTerminalID,
                                             provider.comercialName,
                                             descriptions = p.tblServiceDescriptions.Count()
                                         };

            foreach (var terminal in Terminals)
            {
                AvailabilityStatus.TerminalAvailability newTerminal = new AvailabilityStatus.TerminalAvailability();
                newTerminal.Terminal = terminal.terminal;
                newTerminal.Availability = new List<AvailabilityStatus.AvailabilityItem>();
                newTerminal.Prices = new List<AvailabilityStatus.PriceItem>();

                //iterar horarios
                foreach (var schedule in AvailabilityQ.Where(x => x.originalTerminalID == terminal.terminalID))
                {
                    AvailabilityStatus.AvailabilityItem availabilityItem = new AvailabilityStatus.AvailabilityItem();
                    availabilityItem.ServiceID = schedule.serviceID;
                    availabilityItem.Service = schedule.service;
                    availabilityItem.Provider = schedule.comercialName;
                    availabilityItem.Descriptions = schedule.descriptions;
                    availabilityItem.WeeklyAvailabilityID = schedule.weeklyAvailabilityID;
                    availabilityItem.Hour = (schedule.toHour != null ? "From " + schedule.hour.ToString() + " to " + schedule.toHour.ToString() : schedule.hour.ToString());
                    availabilityItem.Vigency = schedule.fromDate.Value.ToString("yyyy-MM-dd") + (schedule.toDate != null ? " to " + schedule.toDate.Value.ToString("yyyy-MM-dd") : " - Permanent");
                    if (schedule.toDate != null)
                    {
                        //tiene un vencimiento
                        availabilityItem.DaysToExpire = ((DateTime)schedule.toDate.Value.Date - DateTime.Today).TotalDays;
                    }
                    else
                    {
                        //es permanente
                        availabilityItem.DaysToExpire = 1000;
                    }
                    newTerminal.Availability.Add(availabilityItem);
                }

                //iterar servicios activos sin horarios
                foreach (var service in OnlineAvailabilityQ.Where(x => x.originalTerminalID == terminal.terminalID))
                {
                    AvailabilityStatus.AvailabilityItem availabilityItem = new AvailabilityStatus.AvailabilityItem();
                    availabilityItem.ServiceID = service.serviceID;
                    availabilityItem.Service = service.service;
                    availabilityItem.Provider = service.comercialName;
                    availabilityItem.Descriptions = service.descriptions;
                    availabilityItem.WeeklyAvailabilityID = 0;
                    availabilityItem.Hour = "It doesn't have active Schedules";
                    availabilityItem.Vigency = "No Vigency";
                    availabilityItem.DaysToExpire = -1;

                    newTerminal.Availability.Add(availabilityItem);
                }

                //iterar precios
                foreach (var price in PricesQ.Where(x => x.originalTerminalID == terminal.terminalID))
                {
                    AvailabilityStatus.PriceItem priceItem = new AvailabilityStatus.PriceItem();
                    priceItem.ServiceID = price.serviceID;
                    priceItem.Service = price.service;
                    priceItem.Provider = price.comercialName;
                    priceItem.Descriptions = price.descriptions;
                    priceItem.PriceID = price.priceID;
                    priceItem.Price = "$" + price.price + (price.currencyID == 1 ? "USD" : "MXN");
                    priceItem.Vigency = price.fromDate.Value.ToString("yyyy-MM-dd hh-mm-ss tt") + (price.toDate != null ? " to " + price.toDate.Value.ToString("yyyy-MM-dd hh-mm-ss tt") : " - Permanent");
                    if (price.toDate != null)
                    {
                        //tiene un vencimiento
                        priceItem.DaysToExpire = ((DateTime)price.toDate.Value.Date - DateTime.Today).TotalDays;
                    }
                    else
                    {
                        //es permanente
                        priceItem.DaysToExpire = 1000;
                    }
                    newTerminal.Prices.Add(priceItem);
                }

                //iterar sobre servicios activos sin precios
                foreach (var service in ActiveServicesNoPriceQ.Where(x => x.originalTerminalID == terminal.terminalID))
                {
                    AvailabilityStatus.PriceItem priceItem = new AvailabilityStatus.PriceItem();
                    priceItem.ServiceID = service.serviceID;
                    priceItem.Service = service.service;
                    priceItem.Provider = service.comercialName;
                    priceItem.Descriptions = service.descriptions;
                    priceItem.Price = "It doesn't have active Prices";
                    priceItem.Vigency = "No Vigency";
                    priceItem.DaysToExpire = -1;
                    newTerminal.Prices.Add(priceItem);
                }

                status.TerminalsAvailability.Add(newTerminal);
            }

            return status;
        }


    }
}
