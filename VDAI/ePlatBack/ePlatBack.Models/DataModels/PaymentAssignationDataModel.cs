using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;

namespace ePlatBack.Models.DataModels
{
    public class PaymentAssignationDataModel
    {
        ePlatEntities db = new ePlatEntities();

        public PaymentAssignationViewModel GetPaymentsAssignation(Guid purchaseid)
        {
            return GetPaymentsAssignation(purchaseid, null);
        }

        public PaymentAssignationViewModel GetPaymentsAssignation(Guid purchaseid, string date)
        {
            PaymentAssignationViewModel assignation = new PaymentAssignationViewModel();

            List<ExchangeRatesModel.ExchangeRateForDate> ExchangeRates = new List<ExchangeRatesModel.ExchangeRateForDate>();

            DateTime currentDate = (date != null ? DateTime.Parse(date) : DateTime.Today);
            DateTime tomorrow = currentDate.AddDays(1);


            var Purchase = (from p in db.tblPurchases
                            where p.purchaseID == purchaseid
                            select p).FirstOrDefault();

            if (Purchase != null)
            {
                assignation.PurchaseID = purchaseid;
                assignation.TerminalID = Purchase.terminalID;
                assignation.Customer = Purchase.tblLeads.firstName + " " + Purchase.tblLeads.lastName;
                assignation.CloseOuts = new List<PaymentAssignationViewModel.CloseOutItem>();

                //obtener closeouts
                List<long> psids = Purchase.tblPurchases_Services.Select(x => x.purchase_ServiceID).ToList();

                var CloseOuts = (from c in db.tblCloseOuts_Purchases
                                 where psids.Contains(c.purchase_ServiceID)
                                 select c.tblCloseOuts).Distinct();

                foreach (var closeout in CloseOuts.OrderBy(x => x.closeOutDate))
                {
                    PaymentAssignationViewModel.CloseOutItem closeoutitem = new PaymentAssignationViewModel.CloseOutItem();
                    closeoutitem.CloseOutID = closeout.closeOutID;
                    closeoutitem.PointOfSale = closeout.tblPointsOfSale.shortName + " - " + closeout.tblPointsOfSale.pointOfSale;
                    closeoutitem.PointOfSaleID = closeout.pointOfSaleID;
                    closeoutitem.Date = closeout.closeOutDate;
                    closeoutitem.UserID = closeout.salesAgentUserID;
                    closeoutitem.Status = true;
                    //payments
                    decimal pendingRefund = 0;
                    closeoutitem.Payments = new List<PaymentAssignationViewModel.PaymentItem>();
                    var Payments = from y in db.tblCloseOuts_PaymentDetails
                                   where y.closeOutID == closeoutitem.CloseOutID
                                   && y.tblPaymentDetails.purchaseID == purchaseid
                                   select y.tblPaymentDetails;

                    closeoutitem.Payments = GetPaymentsList(Payments);

                    //buscar pagos que se nulifiquen
                    closeoutitem.ExtraPayments = GetNullablePayments(closeoutitem.Payments);

                    //closeoutitem.DiagnosisMessage += closeout.tblCloseOuts_Purchases.Count(x => x.tblPurchases_Services.serviceStatusID == 4) + " canceled coupons.";

                    //asignations
                    closeoutitem.Assignations = new List<PaymentAssignationViewModel.AssignationItem>();
                    decimal uncoveredAmount = 0;
                    decimal totalToRefund = 0;
                    decimal refundsByReplacement = 0;
                    tblExchangeRates rate = null;

                    // ver si ya está guardada
                    var pusdids = from u in db.tblPurchaseServiceDetails
                                  where psids.Contains(u.purchase_ServiceID)
                                  select u.purchaseServiceDetailID;

                    var SavedAssignations = from s in db.tblPaymentsAssignation
                                            where s.closeOutID == closeout.closeOutID
                                            && pusdids.Contains(s.purchaseServiceDetailID)
                                            select s;

                    int savedAssignations = SavedAssignations.Count();
                    //para efectos de pruebas de como guardar asignación, establecer valor en 0
                    //savedAssignations = 0;
                    if (savedAssignations > 0)
                    {
                        foreach (var sa in SavedAssignations)
                        {
                            PaymentAssignationViewModel.AssignationItem assignationitem = new PaymentAssignationViewModel.AssignationItem();
                            assignationitem.PaymentAssignationID = sa.paymentAssignationID;
                            assignationitem.Service = sa.tblPurchaseServiceDetails.tblPurchases_Services.tblServices.service;
                            assignationitem.PurchaseServiceID = sa.tblPurchaseServiceDetails.purchase_ServiceID;
                            assignationitem.PurchaseServiceDetailID = sa.purchaseServiceDetailID;
                            assignationitem.Quantity = sa.tblPurchaseServiceDetails.quantity;
                            assignationitem.Unit = PriceDataModel.GetUnit((long)sa.tblPurchaseServiceDetails.netPriceID, "en-US").unit;
                            assignationitem.PriceType = sa.tblPurchaseServiceDetails.tblPriceTypes.priceType;
                            assignationitem.Folio = sa.tblPurchaseServiceDetails.coupon;
                            assignationitem.Total = new Money()
                            {
                                Amount = sa.unitAmount,
                                Currency = sa.tblCurrencies.currencyCode
                            };
                            assignationitem.CurrencyID = sa.unitCurrencyID;
                            if (sa.exchangeRateID != null)
                            {
                                assignationitem.ExchangeRate = sa.tblExchangeRates.exchangeRate;
                            }
                            assignationitem.ExchangeRateID = sa.exchangeRateID;
                            assignationitem.PaymentDetailsID = sa.paymentDetailsID;
                            if (sa.paymentDetailsID != null)
                            {
                                assignationitem.PaymentType = Utils.GeneralFunctions.PaymentTypes[sa.tblPaymentDetails.paymentType.ToString()];
                            }
                            assignationitem.PaymentSubtotal = new Money()
                            {
                                Amount = sa.paymentAmount,
                                Currency = sa.tblCurrencies1.currencyCode
                            };
                            try
                            {
                                closeoutitem.Payments.FirstOrDefault(p => p.PaymentDetailsID == sa.paymentDetailsID).TotalRemaining -= sa.paymentAmount;
                            }
                            catch
                            {

                            }
                            if (sa.paymentDetailsID != null)
                            {
                                assignationitem.PaymentTotal = sa.tblPaymentDetails.amount;
                                assignationitem.PaymentCurrencyID = (int)sa.tblPaymentDetails.currencyID;
                            }                            
                            assignationitem.MoneyTransactionTypeID = sa.moneyTransactionTypeID;

                            if (assignationitem.PaymentSubtotal.Amount > decimal.Round(decimal.Parse("0.01"), 2, MidpointRounding.AwayFromZero))
                            {
                                closeoutitem.Assignations.Add(assignationitem);
                            }
                        }
                    }
                    else
                    {

                        //PR1 si no está guardada procesarla
                        var Coupons = from u in db.tblCloseOuts_Purchases
                                      where u.closeOutID == closeout.closeOutID
                                      && u.tblPurchases.purchaseID == purchaseid
                                      && u.tblPurchases_Services.serviceStatusID != 2
                                      select new
                                      {
                                          tblPurchases_Services = u.tblPurchases_Services,
                                          paid = u.paid,
                                          canceled = u.canceled
                                      };


                        //Where(c => ((c.paid && !c.canceled) || (c.canceled && c.tblPurchases_Services.serviceStatusID != 4)) && !(c.tblPurchases_Services.serviceStatusID != 5 && c.tblPurchases_Services.total == 0))
                        foreach (var coupon in Coupons.OrderBy(x => x.tblPurchases_Services.serviceStatusID).ThenBy(x => x.tblPurchases_Services.purchase_ServiceID))
                        {
                            int transactionType = 1;
                            bool continueProcess = true;
                            decimal couponRefundsByReplacement = 0;
                            if ((coupon.tblPurchases_Services.serviceStatusID == 5 || coupon.tblPurchases_Services.serviceStatusID == 4) && coupon.tblPurchases_Services.cancelationDateTime < tomorrow)
                            {
                                if (continueProcess 
                                    && coupon.tblPurchases_Services.confirmationDateTime != null 
                                    && coupon.tblPurchases_Services.cancelationDateTime != null 
                                    && coupon.tblPurchases_Services.confirmationDateTime.Value.Date == coupon.tblPurchases_Services.cancelationDateTime.Value.Date
                                    && coupon.tblPurchases_Services.tblCloseOuts_Purchases.Count(x => x.paid && x.closeOutID != closeout.closeOutID) == 0)
                                {
                                    continueProcess = false;
                                }
                                if (coupon.tblPurchases_Services.tblPurchases_Services1.Count() == 1 && coupon.tblPurchases_Services.cancelationDateTime < tomorrow)
                                {
                                    //considerar que solo se anule el proceso, si el total de ambos cupones coincide
                                    decimal currentTotal = 0;
                                    foreach (var detail in coupon.tblPurchases_Services.tblPurchaseServiceDetails)
                                    {
                                        long cPriceID = (detail.priceID != null ? (long)detail.priceID : (long)detail.netPriceID);
                                        //APPLYPROMO replacedTotal += (detail.promo ? 0 : detail.quantity * (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price));
                                        currentTotal += PromoDataModel.ApplyPromo(detail.quantity * (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price), detail.purchaseServiceDetailID);
                                    }
                                    if (currentTotal == coupon.tblPurchases_Services.tblPurchases_Services1.FirstOrDefault().total)
                                    {
                                        continueProcess = false;
                                    }
                                    else if (currentTotal > coupon.tblPurchases_Services.tblPurchases_Services1.FirstOrDefault().total)
                                    {
                                        couponRefundsByReplacement = currentTotal - coupon.tblPurchases_Services.tblPurchases_Services1.FirstOrDefault().total;
                                    }
                                }

                                if (coupon.canceled && coupon.tblPurchases_Services.cancelationDateTime < tomorrow)
                                {
                                    transactionType = 2;
                                }
                            }

                            if (continueProcess)
                            {
                                decimal replacedTotal = 0;
                                decimal replacedCouponTotal = 0;
                                string replacedCouponFolio = "";
                                if (coupon.tblPurchases_Services.replacementOf != null && coupon.tblPurchases_Services.serviceStatusID == 3)
                                {

                                    tblPurchases_Services replacedCoupon = ReportDataModel.GetOriginalReplacedCoupon((long)coupon.tblPurchases_Services.replacementOf);
                                    replacedCouponFolio = replacedCoupon.tblPurchaseServiceDetails.FirstOrDefault().coupon.Substring(0, replacedCoupon.tblPurchaseServiceDetails.FirstOrDefault().coupon.IndexOf("-")); ;
                                    //comprobar que ambos cupones tengan el mismo valor
                                    foreach (var detail in replacedCoupon.tblPurchaseServiceDetails)
                                    {
                                        long cPriceID = (detail.priceID != null ? (long)detail.priceID : (long)detail.netPriceID);
                                        //APPLYPROMO replacedTotal += (detail.promo ? 0 : detail.quantity * (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price));
                                        replacedTotal += PromoDataModel.ApplyPromo(detail.quantity * (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price), detail.purchaseServiceDetailID);
                                    }
                                    if (replacedCoupon.cancelationCharge != null && replacedCoupon.cancelationCharge > 0)
                                    {
                                        replacedTotal = replacedTotal - (decimal)replacedCoupon.cancelationCharge;
                                    }

                                    replacedCouponTotal = replacedTotal;
                                }
                                decimal cancelationCharge = 0;
                                if (coupon.tblPurchases_Services.cancelationCharge != null && coupon.tblPurchases_Services.cancelationDateTime < tomorrow)
                                {
                                    cancelationCharge = (decimal)coupon.tblPurchases_Services.cancelationCharge;
                                }
                                foreach (var unit in coupon.tblPurchases_Services.tblPurchaseServiceDetails)
                                {
                                    decimal coveredAmount = 0;
                                    //APPLYPROMO decimal unitTotal = !unit.promo ? (decimal)unit.customPrice * unit.quantity : 0;
                                    decimal unitTotal = PromoDataModel.ApplyPromo((decimal)unit.customPrice * unit.quantity, unit.purchaseServiceDetailID);

                                    if (replacedTotal > 0) // hay un saldo a favor
                                    {
                                        PaymentAssignationViewModel.AssignationItem aitem = new PaymentAssignationViewModel.AssignationItem();
                                        aitem.Service = coupon.tblPurchases_Services.tblServices.service;
                                        aitem.PurchaseServiceID = unit.purchase_ServiceID;
                                        aitem.PurchaseServiceDetailID = unit.purchaseServiceDetailID;
                                        aitem.Quantity = unit.quantity;
                                        aitem.Unit = PriceDataModel.GetUnit((long)unit.netPriceID, "en-US").unit;
                                        aitem.PriceType = unit.tblPriceTypes.priceType;
                                        aitem.Folio = unit.coupon;
                                        if (replacedCouponFolio != "")
                                        {
                                            aitem.Folio += " on replacement of " + replacedCouponFolio;
                                        }
                                        aitem.CurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;
                                        aitem.Total = new Money()
                                        {
                                            Amount = unitTotal,
                                            Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                        };
                                        if (replacedTotal >= unitTotal)
                                        { //si hay saldo aplicar a unidad
                                            aitem.PaymentSubtotal = new Money()
                                            {
                                                Amount = unitTotal,
                                                Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                            };
                                            coveredAmount += unitTotal;
                                            replacedTotal -= unitTotal;
                                        }
                                        else
                                        {
                                            aitem.PaymentSubtotal = new Money()
                                            {
                                                Amount = replacedTotal,
                                                Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                            };
                                            coveredAmount += replacedTotal;
                                            replacedTotal -= replacedTotal;
                                        }
                                        aitem.ExchangeRate = 1;
                                        aitem.ExchangeRateID = null;
                                        aitem.PaymentDetailsID = null;
                                        aitem.PaymentType = "Replaced Coupon";
                                        aitem.PaymentTotal = replacedCouponTotal;
                                        aitem.PaymentCurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;
                                        aitem.MoneyTransactionTypeID = 1;
                                        if (aitem.PaymentSubtotal.Amount > decimal.Round(decimal.Parse("0.01"), 2, MidpointRounding.AwayFromZero))
                                        {
                                            closeoutitem.Assignations.Add(aitem);
                                        }
                                    }
                                    for (int i = 1; i <= transactionType; i++)
                                    {
                                        if (!coupon.paid)
                                        {
                                            i = 2;
                                        }
                                        if (i == 2)
                                        {
                                            coveredAmount = 0;
                                            if (cancelationCharge >= unitTotal)
                                            {
                                                cancelationCharge -= unitTotal;
                                                unitTotal -= unitTotal;
                                            }
                                            else
                                            {
                                                unitTotal -= cancelationCharge;
                                                cancelationCharge -= cancelationCharge;
                                            }
                                        }

                                        if (coupon.tblPurchases_Services.tblPurchases_Services1.Count() == 1 && coupon.tblPurchases_Services.cancelationDateTime < tomorrow)
                                        {
                                            if (couponRefundsByReplacement > unitTotal)
                                            {
                                                couponRefundsByReplacement -= unitTotal;
                                                refundsByReplacement -= unitTotal;
                                            }
                                            else
                                            {
                                                unitTotal = couponRefundsByReplacement;
                                                refundsByReplacement -= couponRefundsByReplacement;
                                                couponRefundsByReplacement = 0;
                                            }
                                        }

                                        //while (coveredAmount < unitTotal && GetRemainingPayments(closeoutitem.Payments, i) > 0)
                                        while (coveredAmount < unitTotal && GetRemainingPayments(closeoutitem.Payments, i) > decimal.Parse("0.01"))
                                        {
                                            //buscar un pago en la misma moneda

                                            //PaymentAssignationViewModel.PaymentItem currentPayment = (closeoutitem.Payments.Where(c => c.CurrencyID == unit.tblPurchases_Services.tblPurchases.currencyID && c.MoneyTransactionTypeID == i && c.TotalRemaining > 0)).OrderByDescending(x => x.TotalRemaining).FirstOrDefault();
                                            //primero de igual o menor tamaño que el cupón
                                            PaymentAssignationViewModel.PaymentItem currentPayment = new PaymentAssignationViewModel.PaymentItem();

                                            int unitCurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;

                                            List<PaymentAssignationViewModel.PaymentItem> closeOutPayment = closeoutitem.Payments.Where(c =>
                                                c.CurrencyID == unitCurrencyID &&
                                                c.MoneyTransactionTypeID == i &&
                                                c.TotalRemaining > 0
                                                ).ToList();

                                            currentPayment = (closeOutPayment.Where(c =>
                                                c.TotalRemaining == coupon.tblPurchases_Services.total
                                                )).OrderByDescending(x => x.TotalRemaining).FirstOrDefault();

                                            if (currentPayment == null)
                                            {
                                                currentPayment = (closeOutPayment.Where(c => c.TotalRemaining < coupon.tblPurchases_Services.total)).OrderByDescending(x => x.TotalRemaining).FirstOrDefault();
                                            }

                                            if (currentPayment == null)
                                            {
                                                currentPayment = closeOutPayment.OrderByDescending(x => x.TotalRemaining).FirstOrDefault();
                                            }  


                                            if (currentPayment != null)
                                            {
                                                PaymentAssignationViewModel.AssignationItem assignationitem = new PaymentAssignationViewModel.AssignationItem();
                                                assignationitem.Service = coupon.tblPurchases_Services.tblServices.service;
                                                assignationitem.PurchaseServiceID = unit.purchase_ServiceID;
                                                assignationitem.PurchaseServiceDetailID = unit.purchaseServiceDetailID;
                                                assignationitem.Quantity = unit.quantity;
                                                assignationitem.Unit = PriceDataModel.GetUnit((long)unit.netPriceID, "en-US").unit;
                                                assignationitem.PriceType = unit.tblPriceTypes.priceType;
                                                assignationitem.Folio = unit.coupon;
                                                assignationitem.Total = new Money()
                                                {
                                                    Amount = unitTotal,
                                                    Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                                };
                                                totalToRefund = unitTotal;
                                                assignationitem.CurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;

                                                //existe un pago de la misma moneda para aplicar
                                                assignationitem.PaymentSubtotal = new Money()
                                                {
                                                    Amount = (assignationitem.Total.Amount - coveredAmount) > currentPayment.TotalRemaining ? currentPayment.TotalRemaining : (assignationitem.Total.Amount - coveredAmount),
                                                    Currency = assignationitem.Total.Currency
                                                };
                                                currentPayment.TotalRemaining -= assignationitem.PaymentSubtotal.Amount;
                                                coveredAmount += assignationitem.PaymentSubtotal.Amount;

                                                assignationitem.ExchangeRate = 1;
                                                assignationitem.ExchangeRateID = null;
                                                assignationitem.PaymentDetailsID = currentPayment.PaymentDetailsID;
                                                assignationitem.PaymentType = currentPayment.PaymentType;
                                                assignationitem.PaymentTotal = currentPayment.TotalPayment;
                                                assignationitem.PaymentCurrencyID = currentPayment.CurrencyID;
                                                assignationitem.MoneyTransactionTypeID = currentPayment.MoneyTransactionTypeID;

                                                if (assignationitem.PaymentSubtotal.Amount > decimal.Round(decimal.Parse("0.01"), 2, MidpointRounding.AwayFromZero))
                                                {
                                                    closeoutitem.Assignations.Add(assignationitem);
                                                }
                                            }
                                            else
                                            {
                                                //buscar un pago de otra moneda que pueda cubrir el pago.
                                                int exchangeCurrencyID = 0;
                                                ExchangeRatesModel.ExchangeRateForDate currentRate = new ExchangeRatesModel.ExchangeRateForDate();

                                                DateTime exchangeDate = (coupon.tblPurchases_Services.confirmationDateTime != null ? (DateTime)coupon.tblPurchases_Services.confirmationDateTime.Value : coupon.tblPurchases_Services.dateSaved);
                                                foreach (var payment in closeoutitem.Payments.Where(p => p.MoneyTransactionTypeID == i && p.CurrencyID != unit.tblPurchases_Services.tblPurchases.currencyID && p.Date < closeout.closeOutDate.AddDays(1)))
                                                {

                                                    PaymentAssignationViewModel.AssignationItem assignationitem = new PaymentAssignationViewModel.AssignationItem();
                                                    assignationitem.Service = coupon.tblPurchases_Services.tblServices.service;
                                                    assignationitem.PurchaseServiceID = unit.purchase_ServiceID;
                                                    assignationitem.PurchaseServiceDetailID = unit.purchaseServiceDetailID;
                                                    assignationitem.Quantity = unit.quantity;
                                                    assignationitem.Unit = PriceDataModel.GetUnit((long)unit.netPriceID, "en-US").unit;
                                                    assignationitem.PriceType = unit.tblPriceTypes.priceType;
                                                    assignationitem.Folio = unit.coupon;
                                                    assignationitem.Total = new Money()
                                                    {
                                                        Amount = unitTotal,
                                                        Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                                    };
                                                    totalToRefund = unitTotal;
                                                    assignationitem.CurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;

                                                    exchangeCurrencyID = payment.CurrencyID != 2 ? payment.CurrencyID : assignationitem.CurrencyID;

                                                    rate = db.tblExchangeRates.Where(m => 
                                                        m.fromCurrencyID == exchangeCurrencyID 
                                                        && m.fromDate <= payment.Date
                                                        && (m.permanent_ == true || m.toDate > payment.Date)
                                                        && m.providerID == null 
                                                        && m.toCurrencyID == 2 
                                                        && m.terminalID == coupon.tblPurchases_Services.tblPurchases.terminalID
                                                        && m.exchangeRateTypeID == 1
                                                        && m.tblExchangeRates_PointsOfSales.Count(
                                                            p => p.pointOfSaleID == coupon.tblPurchases_Services.tblPurchases.pointOfSaleID
                                                            && p.dateAdded <= payment.Date
                                                            && (p.dateDeleted == null || p.dateDeleted > payment.Date)
                                                        ) > 0
                                                        ).OrderByDescending(m => m.fromDate).ThenByDescending(x => x.dateSaved).FirstOrDefault();
                                                    if (rate == null)
                                                    {
                                                        rate = db.tblExchangeRates.Where(m => m.fromCurrencyID == exchangeCurrencyID && m.fromDate <= payment.Date && m.providerID == null && m.toCurrencyID == 2 && m.terminalID == coupon.tblPurchases_Services.tblPurchases.terminalID && m.exchangeRateTypeID == 1 && m.tblExchangeRates_PointsOfSales.Count() == 0).OrderByDescending(m => m.fromDate).ThenByDescending(x => x.dateSaved).FirstOrDefault();
                                                    }
                                                    
                                                    currentRate = new ExchangeRatesModel.ExchangeRateForDate()
                                                    {
                                                        ExchangeRateID = rate.exchangeRateID,
                                                        Date = exchangeDate,
                                                        CurrencyCode = payment.CurrencyCode,
                                                        CurrencyID = payment.CurrencyID,
                                                        ExchangeRate = rate.exchangeRate
                                                    };
                                                    //    ExchangeRates.Add(currentRate);
                                                    //}
                                                    if (assignationitem.CurrencyID == 2) //compra en pesos y pago en otra moneda
                                                    {

                                                        assignationitem.PaymentSubtotal = new Money()
                                                        {
                                                            Amount = (assignationitem.Total.Amount - coveredAmount) / currentRate.ExchangeRate > payment.TotalRemaining ? payment.TotalRemaining : (assignationitem.Total.Amount - coveredAmount) / currentRate.ExchangeRate,
                                                            Currency = payment.CurrencyCode
                                                        };
                                                        payment.TotalRemaining -= assignationitem.PaymentSubtotal.Amount;
                                                        coveredAmount += assignationitem.PaymentSubtotal.Amount * currentRate.ExchangeRate;

                                                        assignationitem.ExchangeRate = currentRate.ExchangeRate;
                                                        assignationitem.ExchangeRateID = currentRate.ExchangeRateID;
                                                        assignationitem.PaymentDetailsID = payment.PaymentDetailsID;
                                                        assignationitem.PaymentType = payment.PaymentType;
                                                        assignationitem.PaymentTotal = payment.TotalPayment;
                                                        assignationitem.PaymentCurrencyID = payment.CurrencyID;
                                                        assignationitem.MoneyTransactionTypeID = payment.MoneyTransactionTypeID;

                                                    }
                                                    else if (assignationitem.CurrencyID != 2 && payment.CurrencyID == 2) //compra en otra moneda y pago en pesos
                                                    {
                                                        assignationitem.PaymentSubtotal = new Money()
                                                        {
                                                            Amount = (assignationitem.Total.Amount - coveredAmount) * currentRate.ExchangeRate > payment.TotalRemaining ? payment.TotalRemaining : (assignationitem.Total.Amount - coveredAmount) * currentRate.ExchangeRate,
                                                            Currency = payment.CurrencyCode
                                                        };
                                                        payment.TotalRemaining -= assignationitem.PaymentSubtotal.Amount;
                                                        coveredAmount += assignationitem.PaymentSubtotal.Amount / currentRate.ExchangeRate;

                                                        assignationitem.ExchangeRate = currentRate.ExchangeRate;
                                                        assignationitem.ExchangeRateID = currentRate.ExchangeRateID;
                                                        assignationitem.PaymentDetailsID = payment.PaymentDetailsID;
                                                        assignationitem.PaymentType = payment.PaymentType;
                                                        assignationitem.PaymentTotal = payment.TotalPayment;
                                                        assignationitem.PaymentCurrencyID = payment.CurrencyID;
                                                        assignationitem.MoneyTransactionTypeID = payment.MoneyTransactionTypeID;
                                                    }
                                                    else if (assignationitem.CurrencyID == 1 && payment.CurrencyID == 3)
                                                    {
                                                        //pago en otra moneda y compra en dolares
                                                        //obtener el tipo de cambio de dolar americano para multiplicar por el tipo de cambio USD y dividir entre CAD

                                                        ExchangeRatesModel.ExchangeRateForDate currentUSDRate = new ExchangeRatesModel.ExchangeRateForDate();

                                                        rate = db.tblExchangeRates.Where(m => 
                                                            m.fromCurrencyID == 1 
                                                            && m.fromDate <= payment.Date
                                                            && (m.permanent_ == true || m.toDate > payment.Date)
                                                            && m.providerID == null 
                                                            && m.toCurrencyID == 2 
                                                            && m.terminalID == coupon.tblPurchases_Services.tblPurchases.terminalID
                                                            && m.exchangeRateTypeID == 1
                                                            && m.tblExchangeRates_PointsOfSales.Count(
                                                                p => p.pointOfSaleID == coupon.tblPurchases_Services.tblPurchases.pointOfSaleID
                                                                && p.dateAdded <= payment.Date
                                                                && (p.dateDeleted == null || p.dateDeleted > payment.Date)
                                                            ) > 0
                                                            ).OrderByDescending(m => m.fromDate).FirstOrDefault();
                                                        
                                                        if (rate == null)
                                                        {
                                                            rate = db.tblExchangeRates.Where(m => m.fromCurrencyID == 1 && m.fromDate <= payment.Date && m.providerID == null && m.toCurrencyID == 2 && m.terminalID == coupon.tblPurchases_Services.tblPurchases.terminalID && m.exchangeRateTypeID == 1 && m.tblExchangeRates_PointsOfSales.Count() == 0).OrderByDescending(m => m.fromDate).FirstOrDefault();
                                                        }
                                                        
                                                        currentUSDRate = new ExchangeRatesModel.ExchangeRateForDate()
                                                        {
                                                            ExchangeRateID = rate.exchangeRateID,
                                                            Date = exchangeDate,
                                                            CurrencyCode = payment.CurrencyCode,
                                                            CurrencyID = payment.CurrencyID,
                                                            ExchangeRate = rate.exchangeRate
                                                        };

                                                        assignationitem.PaymentSubtotal = new Money()
                                                        {
                                                            Amount = (assignationitem.Total.Amount - coveredAmount) * currentUSDRate.ExchangeRate / currentRate.ExchangeRate > payment.TotalRemaining ? payment.TotalRemaining : (assignationitem.Total.Amount - coveredAmount) * currentUSDRate.ExchangeRate / currentRate.ExchangeRate,
                                                            Currency = "CAD"
                                                        };
                                                        payment.TotalRemaining -= assignationitem.PaymentSubtotal.Amount;
                                                        coveredAmount += assignationitem.PaymentSubtotal.Amount * currentRate.ExchangeRate / currentUSDRate.ExchangeRate;

                                                        assignationitem.ExchangeRate = currentRate.ExchangeRate;
                                                        assignationitem.ExchangeRateID = currentRate.ExchangeRateID;
                                                        assignationitem.PaymentDetailsID = payment.PaymentDetailsID;
                                                        assignationitem.PaymentType = payment.PaymentType;
                                                        assignationitem.PaymentTotal = payment.TotalPayment;
                                                        assignationitem.PaymentCurrencyID = payment.CurrencyID;
                                                        assignationitem.MoneyTransactionTypeID = payment.MoneyTransactionTypeID;
                                                    }

                                                    if (assignationitem.PaymentSubtotal.Amount > decimal.Round(decimal.Parse("0.01"), 2, MidpointRounding.AwayFromZero))
                                                    {
                                                        closeoutitem.Assignations.Add(assignationitem);
                                                    }
                                                }
                                            }
                                        }
                                        if (i == 1)
                                        {
                                            uncoveredAmount += unitTotal - coveredAmount;
                                        }
                                        else if (i == 2)
                                        {
                                            if (coveredAmount == 0 && unitTotal > 0 && GetRemainingPayments(closeoutitem.Payments, i) == 0)
                                            {
                                                closeoutitem.Status = false;
                                                if (closeoutitem.DiagnosisMessage != null)
                                                {
                                                    closeoutitem.DiagnosisMessage += "\n";
                                                }
                                                closeoutitem.DiagnosisMessage += "There is a pending refund for $" + Decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero) + " " + Purchase.tblCurrencies.currencyCode;
                                            }
                                            else if (coveredAmount > 0 && coveredAmount < totalToRefund && GetRemainingPayments(closeoutitem.Payments, i) == 0)
                                            {
                                                pendingRefund = (totalToRefund - coveredAmount);
                                                //buscar un cancelation charge igual al pending refund
                                                if (coupon.tblPurchases_Services.tblPurchases_Services1.Count() > 0 && coupon.tblPurchases_Services.tblPurchases_Services1.FirstOrDefault().cancelationCharge == pendingRefund)
                                                {
                                                    pendingRefund = 0;
                                                }
                                                if (pendingRefund > 0)
                                                {
                                                    closeoutitem.Status = false;
                                                    if (closeoutitem.DiagnosisMessage != null)
                                                    {
                                                        closeoutitem.DiagnosisMessage += "\n";
                                                    }
                                                    closeoutitem.DiagnosisMessage += "There is a pending refund for $" + Decimal.Round((totalToRefund - coveredAmount), 2, MidpointRounding.AwayFromZero) + " " + Purchase.tblCurrencies.currencyCode;
                                                }                                                
                                            }
                                        }
                                    }
                                }
                                refundsByReplacement += replacedTotal;
                            }
                        }
                    }

                    //extrapayments
                    decimal remainingPayments = GetRemainingPayments(closeoutitem.Payments, 1) + refundsByReplacement;
                    if (remainingPayments > decimal.Parse("0.01"))
                    {
                        //si hay extrapayments, buscar reembolsos y compararlos
                        foreach (var p in closeoutitem.Payments.Where(x => x.TotalRemaining > decimal.Parse("0.01") && x.MoneyTransactionTypeID == 1))
                        {
                            var currentRefund = closeoutitem.Payments.FirstOrDefault(x => x.TotalRemaining >= p.TotalRemaining - decimal.Parse("0.01") && x.MoneyTransactionTypeID == 2 && x.CurrencyID == p.CurrencyID);

                            decimal paymentInOtherCurrency = p.TotalRemaining;
                            if (currentRefund == null)
                            {
                                if (p.CurrencyID == 2)
                                {
                                    if (rate != null)
                                    {
                                        paymentInOtherCurrency = decimal.Round(p.TotalRemaining / rate.exchangeRate, 2);
                                    }

                                }
                                else if (p.CurrencyID == 1)
                                {
                                    if (rate != null)
                                    {
                                        paymentInOtherCurrency = decimal.Round(p.TotalRemaining * rate.exchangeRate, 2);
                                    }
                                }
                                currentRefund = closeoutitem.Payments.FirstOrDefault(x => decimal.Round(x.TotalRemaining, 2, MidpointRounding.AwayFromZero) >= decimal.Round(paymentInOtherCurrency, 2, MidpointRounding.AwayFromZero) && x.MoneyTransactionTypeID == 2 && x.CurrencyID != p.CurrencyID);
                            }
                            if (currentRefund != null)
                            {
                                PaymentAssignationViewModel.ExtraPaymentItem ep = new PaymentAssignationViewModel.ExtraPaymentItem();
                                ep.ExtraAmount = new Money()
                                {
                                    Amount = p.TotalRemaining,
                                    Currency = p.CurrencyCode
                                };
                                ep.ExtraFromPaymentDetailsID = p.PaymentDetailsID;
                                ep.PaymentDetailDescription = p.TotalPayment + " " + p.CurrencyCode;
                                ep.RefundPaymentDetailsID = currentRefund.PaymentDetailsID;
                                ep.RefundDetailDescription = currentRefund.TotalPayment + " " + currentRefund.CurrencyCode;
                                closeoutitem.ExtraPayments.Add(ep);

                                decimal remainingToCover = paymentInOtherCurrency;
                                currentRefund.TotalRemaining -= remainingToCover;
                                p.TotalRemaining -= p.TotalRemaining;
                            }
                        }

                        //una vez aplicados los reembolsos, ahora si verificar y mostrar mensajes
                        remainingPayments = GetRemainingPayments(closeoutitem.Payments, 1, ExchangeRates) + refundsByReplacement;
                        if (remainingPayments > decimal.Parse("0.1"))
                        {
                            if (closeoutitem.DiagnosisMessage != null)
                            {
                                closeoutitem.DiagnosisMessage += "\n";
                            }
                            closeoutitem.DiagnosisMessage += "Remaining Payments to Refund: $" + remainingPayments.ToString();
                        }
                    }
                    if (remainingPayments <= decimal.Parse("0.1") && closeoutitem.Status == true)
                    {
                        closeoutitem.Status = true;
                    }
                    else
                    {
                        closeoutitem.Status = false;
                    }

                    if (uncoveredAmount > decimal.Parse("0.1"))
                    {
                        if (decimal.Round(uncoveredAmount, 2, MidpointRounding.AwayFromZero) == decimal.Round(pendingRefund, 2, MidpointRounding.AwayFromZero))
                        {
                            closeoutitem.Status = true;
                            closeoutitem.DiagnosisMessage = "Virtual Balance applied";
                        }
                        else
                        {
                            closeoutitem.Status = false;
                            if (closeoutitem.DiagnosisMessage != null)
                            {
                                closeoutitem.DiagnosisMessage += "\n";
                            }
                            closeoutitem.DiagnosisMessage += "Not enought payments.\nThere is a Debt of: $" + uncoveredAmount + " " + Purchase.tblCurrencies.currencyCode;
                        }
                    }

                    assignation.CloseOuts.Add(closeoutitem);
                }

                ////////////////obtener pagos y cupones sin corte
                //PR2
                var couponSales = from c in db.tblPurchases_Services
                                  where c.purchaseID == purchaseid
                                  && ((c.tblCloseOuts_Purchases.Count(p => p.paid == true) == 0 && c.confirmationDateTime != null && c.confirmationDateTime < tomorrow && c.serviceStatusID != 2)
                                  || ((c.serviceStatusID == 4 || c.serviceStatusID == 5) && c.tblCloseOuts_Purchases.Count(p => p.canceled) == 0 && c.cancelationDateTime < tomorrow))
                                  orderby c.confirmationDateTime
                                  select c;

                if (couponSales.Count() > 0)
                {
                    PaymentAssignationViewModel.CloseOutItem closeoutitem = new PaymentAssignationViewModel.CloseOutItem();
                    closeoutitem.CloseOutID = 0;
                    closeoutitem.PointOfSale = Purchase.tblPointsOfSale.shortName + " - " + Purchase.tblPointsOfSale.pointOfSale;
                    closeoutitem.PointOfSaleID = Purchase.pointOfSaleID;
                    closeoutitem.Date = currentDate;
                    closeoutitem.UserID = Purchase.userID;
                    closeoutitem.Status = true;
                    //payments
                    decimal pendingRefund = 0;
                    closeoutitem.Payments = new List<PaymentAssignationViewModel.PaymentItem>();
                    var Payments = from p in db.tblPaymentDetails
                                   where p.purchaseID == purchaseid
                                   && (p.deleted == null || p.deleted == false)
                                   && p.tblCloseOuts_PaymentDetails.Count() == 0
                                   && p.tblMoneyTransactions.errorCode == "0"
                                   && p.dateSaved < tomorrow
                                   select p;

                    closeoutitem.Payments = GetPaymentsList(Payments);

                    //buscar pagos que se nulifiquen
                    closeoutitem.ExtraPayments = GetNullablePayments(closeoutitem.Payments);
                    //if (couponSales.Count(x => x.serviceStatusID == 4) > 0)
                    //{
                    //    closeoutitem.DiagnosisMessage += couponSales.Count(x => x.serviceStatusID == 4) + " canceled coupons.";
                    //}

                    //asignations
                    closeoutitem.Assignations = new List<PaymentAssignationViewModel.AssignationItem>();
                    decimal uncoveredAmount = 0;
                    decimal totalToRefund = 0;

                    //decimal totalcoupons = 0;
                    ////Purchase.tblPurchases_Services
                    //foreach (var c in couponSales.Where(x => x.serviceStatusID != 4 && x.serviceStatusID != 2 && !(x.serviceStatusID != 5 && x.total == 0)))
                    //{
                    //    if ((c.serviceStatusID == 5 && c.tblCloseOuts_Purchases.Count(x => x.canceled) == 0) || (c.serviceStatusID != 5 && c.tblCloseOuts_Purchases.Count(x => x.paid) == 0))
                    //    {
                    //        totalcoupons += c.total;
                    //    }
                    //    else if ((c.serviceStatusID == 4 || c.serviceStatusID == 5) && c.tblCloseOuts_Purchases.Count(x => x.paid) == 0 && c.confirmationDateTime != null)
                    //    {
                    //        //si el cupon fue confirmado antes pero no está cortado
                    //        foreach (var d in c.tblPurchaseServiceDetails)
                    //        {
                    //            totalcoupons += (d.promo != null ? d.quantity * (decimal)d.customPrice : 0);
                    //        }
                    //    }
                    //}

                    //if (totalcoupons > 0 && closeoutitem.Payments.Count() == 0)
                    //{
                    //    closeoutitem.Status = false;
                    //    if (closeoutitem.DiagnosisMessage != null)
                    //    {
                    //        closeoutitem.DiagnosisMessage += "\n";
                    //    }
                    //    closeoutitem.DiagnosisMessage += "There are not payments or refunds registered.";
                    //}

                    //Purchase.tblPurchases_Services
                    decimal refundsByReplacement = 0;
                    tblExchangeRates rate = null;

                    IOrderedQueryable<tblPurchases_Services> cCouponSales = couponSales.Where(x => (x.serviceStatusID != 4 && x.serviceStatusID != 2 && !(x.serviceStatusID != 5 && x.total == 0)) || (x.serviceStatusID == 4 && x.confirmationDateTime != null && x.confirmationDateTime < tomorrow )).OrderBy(x => x.serviceStatusID);

                    foreach (var coupon in cCouponSales)
                    {
                        int transactionType = 1;
                        bool continueProcess = true;
                        decimal couponRefundsByReplacement = 0;
                        if ((coupon.serviceStatusID == 5 || coupon.serviceStatusID == 4) && coupon.cancelationDateTime < tomorrow)
                        {
                            if (coupon.tblCloseOuts_Purchases.Count(x => x.canceled) > 0 && coupon.tblCloseOuts_Purchases.Count(p => p.paid) > 0)
                            {
                                continueProcess = false;
                            }
                            if (continueProcess 
                                && coupon.confirmationDateTime != null 
                                && coupon.cancelationDateTime != null 
                                && coupon.confirmationDateTime.Value.Date == coupon.cancelationDateTime.Value.Date
                                && coupon.tblCloseOuts_Purchases.Count(x => x.paid) == 0
                                && (coupon.cancelationCharge == null || coupon.cancelationCharge == 0))
                            {
                                continueProcess = false;
                            }

                            if (continueProcess && coupon.tblPurchases_Services1.Count() == 1)
                            {
                                //considerar que solo se anule el proceso, si el total de ambos cupones coincide
                                decimal currentTotal = 0;
                                foreach (var detail in coupon.tblPurchaseServiceDetails)
                                {
                                    long cPriceID = (detail.priceID != null ? (long)detail.priceID : (long)detail.netPriceID);
                                    //APPLYPROMO replacedTotal += (detail.promo ? 0 : detail.quantity * (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price));
                                     currentTotal += PromoDataModel.ApplyPromo(detail.quantity * (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price), detail.purchaseServiceDetailID);
                                }
                                if (currentTotal == coupon.tblPurchases_Services1.FirstOrDefault().total)
                                {
                                    continueProcess = false;
                                }
                                else if(currentTotal > coupon.tblPurchases_Services1.FirstOrDefault().total)
                                {
                                    /*NO FUNCIONA CUANDO EL CUPÓN YA FUE CANCELADO Y EL TOTAL GUARDADO ES 0*/
                                    couponRefundsByReplacement = currentTotal - coupon.tblPurchases_Services1.FirstOrDefault().total;
                                }     
                            }

                            if (coupon.tblCloseOuts_Purchases.Count(p => p.paid) == 0)
                            {
                                transactionType = 1;
                            }
                            else
                            {
                                transactionType = 2;
                            }                            
                        }
                        else
                        {
                            if (coupon.tblCloseOuts_Purchases.Count(x => x.paid) > 0)
                            {
                                continueProcess = false;
                            }
                        }
                        if (continueProcess)
                        {
                            decimal replacedTotal = 0;
                            decimal replacedCouponTotal = 0;
                            string replacedCouponFolio = "";
                            if (coupon.replacementOf != null && coupon.serviceStatusID == 3)
                            {
                                    tblPurchases_Services replacedCoupon = ReportDataModel.GetOriginalReplacedCoupon((long)coupon.replacementOf);
                                    replacedCouponFolio = replacedCoupon.tblPurchaseServiceDetails.FirstOrDefault().coupon.Substring(0, replacedCoupon.tblPurchaseServiceDetails.FirstOrDefault().coupon.IndexOf("-")); ;
                                    //comprobar que ambos cupones tengan el mismo valor
                                    foreach (var detail in replacedCoupon.tblPurchaseServiceDetails)
                                    {
                                        long cPriceID = (detail.priceID != null ? (long)detail.priceID : (long)detail.netPriceID);
                                        //APPLYPROMO replacedTotal += (detail.promo ? 0 : detail.quantity * (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price));
                                        replacedTotal += PromoDataModel.ApplyPromo(detail.quantity * (detail.customPrice != null ? (decimal)detail.customPrice : detail.tblPrices.price), detail.purchaseServiceDetailID);
                                    }
                                    if (replacedCoupon.cancelationCharge != null && replacedCoupon.cancelationCharge > 0)
                                    {
                                        replacedTotal = replacedTotal - (decimal)replacedCoupon.cancelationCharge;
                                    }
                                    replacedCouponTotal = replacedTotal;
                            }
                            
                            decimal cancelationCharge = 0;
                            if (coupon.cancelationCharge != null)
                            {
                                cancelationCharge = (decimal)coupon.cancelationCharge;
                            }
                            foreach (var unit in coupon.tblPurchaseServiceDetails)
                            {
                                decimal coveredAmount = 0;
                                //APPLYPROMO decimal unitTotal = !unit.promo ? (decimal)unit.customPrice * unit.quantity : 0;
                                decimal unitTotal = PromoDataModel.ApplyPromo((decimal)unit.customPrice * unit.quantity, unit.purchaseServiceDetailID);

                                if (replacedTotal > 0) // hay un saldo a favor
                                {
                                    PaymentAssignationViewModel.AssignationItem aitem = new PaymentAssignationViewModel.AssignationItem();
                                    aitem.Service = coupon.tblServices.service;
                                    aitem.PurchaseServiceID = unit.purchase_ServiceID;
                                    aitem.PurchaseServiceDetailID = unit.purchaseServiceDetailID;
                                    aitem.Quantity = unit.quantity;
                                    aitem.Unit = PriceDataModel.GetUnit((long)unit.netPriceID, "en-US").unit;
                                    aitem.PriceType = unit.tblPriceTypes.priceType;
                                    aitem.Folio = unit.coupon;
                                    if (replacedCouponFolio != "")
                                    {
                                        aitem.Folio += " on replacement of " + replacedCouponFolio;
                                    }
                                    aitem.CurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;
                                    aitem.Total = new Money()
                                    {
                                        Amount = unitTotal,
                                        Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                    };
                                    if (replacedTotal >= unitTotal)
                                    { //si hay saldo aplicar a unidad
                                        aitem.PaymentSubtotal = new Money()
                                        {
                                            Amount = unitTotal,
                                            Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                        };
                                        coveredAmount += unitTotal;
                                        replacedTotal -= unitTotal;
                                    }
                                    else
                                    {
                                        aitem.PaymentSubtotal = new Money()
                                        {
                                            Amount = replacedTotal,
                                            Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                        };
                                        coveredAmount += replacedTotal;
                                        replacedTotal -= replacedTotal;
                                    }
                                    aitem.ExchangeRate = 1;
                                    aitem.ExchangeRateID = null;
                                    aitem.PaymentDetailsID = null;
                                    aitem.PaymentType = "Replaced Coupon";
                                    aitem.PaymentTotal = replacedCouponTotal;
                                    aitem.PaymentCurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;
                                    aitem.MoneyTransactionTypeID = 1;
                                    if (aitem.PaymentSubtotal.Amount > decimal.Round(decimal.Parse("0.01"), 2, MidpointRounding.AwayFromZero))
                                    {
                                        closeoutitem.Assignations.Add(aitem);
                                    }

                                }
                                for (int i = 1; i <= transactionType; i++)
                                {
                                    if (transactionType == 2 && coupon.tblCloseOuts_Purchases.Count(x => x.paid) > 0)
                                    {
                                        i++;
                                    }
                                    if (i == 2)
                                    {
                                        coveredAmount = 0;
                                        if (cancelationCharge >= unitTotal)
                                        {
                                            cancelationCharge -= unitTotal;
                                            unitTotal -= unitTotal;
                                        }
                                        else
                                        {
                                            unitTotal -= cancelationCharge;
                                            cancelationCharge -= cancelationCharge;
                                        }
                                    }

                                    if (coupon.tblPurchases_Services1.Count() == 1 && (coupon.cancelationDateTime == null || coupon.cancelationDateTime < tomorrow)) {
                                        if (couponRefundsByReplacement > unitTotal)
                                        {
                                            couponRefundsByReplacement -= unitTotal;
                                            refundsByReplacement -= unitTotal;
                                        }
                                        else
                                        {
                                            unitTotal = couponRefundsByReplacement;
                                            refundsByReplacement -= couponRefundsByReplacement;
                                            couponRefundsByReplacement = 0;
                                        }
                                    }

                                    //while (coveredAmount < unitTotal && GetRemainingPayments(closeoutitem.Payments, i) > 0)
                                    //while (coveredAmount < unitTotal && GetRemainingPayments(closeoutitem.Payments, i) > decimal.Parse("0.01"))
                                    while (coveredAmount < unitTotal && GetRemainingPayments(closeoutitem.Payments, i) > decimal.Parse("0.01") && (unitTotal-coveredAmount) > decimal.Parse("0.01"))
                                    {
                                        //buscar un pago en la misma moneda

                                        //primero de igual o menor tamaño que el cupón
                                        PaymentAssignationViewModel.PaymentItem currentPayment = new PaymentAssignationViewModel.PaymentItem();

                                        int unitCurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;

                                        List<PaymentAssignationViewModel.PaymentItem> closeOutPayment = closeoutitem.Payments.Where(c =>
                                            c.CurrencyID == unitCurrencyID &&
                                            c.MoneyTransactionTypeID == i &&
                                            c.TotalRemaining > 0
                                            ).ToList();

                                        currentPayment = (closeOutPayment.Where(c =>
                                            c.TotalRemaining == coupon.total
                                            )).OrderByDescending(x => x.TotalRemaining).FirstOrDefault();

                                        if (currentPayment == null)
                                        {
                                            currentPayment = (closeOutPayment.Where(c => c.TotalRemaining < coupon.total)).OrderByDescending(x => x.TotalRemaining).FirstOrDefault();
                                        }   

                                        if (currentPayment == null) {
                                            currentPayment = closeOutPayment.OrderByDescending(x => x.TotalRemaining).FirstOrDefault();
                                        }                                        

                                        if (currentPayment != null)
                                        {
                                            PaymentAssignationViewModel.AssignationItem assignationitem = new PaymentAssignationViewModel.AssignationItem();
                                            assignationitem.Service = coupon.tblServices.service;
                                            assignationitem.PurchaseServiceID = unit.purchase_ServiceID;
                                            assignationitem.PurchaseServiceDetailID = unit.purchaseServiceDetailID;
                                            assignationitem.Quantity = unit.quantity;
                                            assignationitem.Unit = PriceDataModel.GetUnit((long)unit.netPriceID, "en-US").unit;
                                            assignationitem.PriceType = unit.tblPriceTypes.priceType;
                                            assignationitem.Folio = unit.coupon;
                                            assignationitem.Total = new Money()
                                            {
                                                Amount = unitTotal,
                                                Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                            };
                                            totalToRefund = unitTotal;
                                            assignationitem.CurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;

                                            //existe un pago de la misma moneda para aplicar
                                            assignationitem.PaymentSubtotal = new Money()
                                            {
                                                Amount = (assignationitem.Total.Amount - coveredAmount) > currentPayment.TotalRemaining ? currentPayment.TotalRemaining : assignationitem.Total.Amount - coveredAmount,
                                                Currency = assignationitem.Total.Currency
                                            };
                                            currentPayment.TotalRemaining -= assignationitem.PaymentSubtotal.Amount;
                                            coveredAmount += assignationitem.PaymentSubtotal.Amount;

                                            assignationitem.ExchangeRate = 1;
                                            assignationitem.ExchangeRateID = null;
                                            assignationitem.PaymentDetailsID = currentPayment.PaymentDetailsID;
                                            assignationitem.PaymentType = currentPayment.PaymentType;
                                            assignationitem.PaymentTotal = currentPayment.TotalPayment;
                                            assignationitem.PaymentCurrencyID = currentPayment.CurrencyID;
                                            assignationitem.MoneyTransactionTypeID = currentPayment.MoneyTransactionTypeID;

                                            if (assignationitem.PaymentSubtotal != null && assignationitem.PaymentSubtotal.Amount > decimal.Round(decimal.Parse("0.01"), 2, MidpointRounding.AwayFromZero))
                                            {
                                                closeoutitem.Assignations.Add(assignationitem);
                                            }
                                        }
                                        else
                                        {
                                            //buscar un pago de otra moneda que pueda cubrir el pago.
                                            int exchangeCurrencyID = 0;
                                            ExchangeRatesModel.ExchangeRateForDate currentRate = new ExchangeRatesModel.ExchangeRateForDate();

                                            DateTime exchangeDate = (coupon.confirmationDateTime != null ? (DateTime)coupon.confirmationDateTime.Value.Date : coupon.dateSaved.Date);
                                            foreach (var payment in closeoutitem.Payments.Where(p => p.MoneyTransactionTypeID == i && p.CurrencyID != unit.tblPurchases_Services.tblPurchases.currencyID))
                                            {
                                                PaymentAssignationViewModel.AssignationItem assignationitem = new PaymentAssignationViewModel.AssignationItem();
                                                assignationitem.Service = coupon.tblServices.service;
                                                assignationitem.PurchaseServiceID = unit.purchase_ServiceID;
                                                assignationitem.PurchaseServiceDetailID = unit.purchaseServiceDetailID;
                                                assignationitem.Quantity = unit.quantity;
                                                assignationitem.Unit = PriceDataModel.GetUnit((long)unit.netPriceID, "en-US").unit;
                                                assignationitem.PriceType = unit.tblPriceTypes.priceType;
                                                assignationitem.Folio = unit.coupon;
                                                assignationitem.Total = new Money()
                                                {
                                                    Amount = unitTotal,
                                                    Currency = unit.tblPurchases_Services.tblPurchases.tblCurrencies.currencyCode
                                                };
                                                totalToRefund = unitTotal;
                                                assignationitem.CurrencyID = unit.tblPurchases_Services.tblPurchases.currencyID;

                                                exchangeCurrencyID = payment.CurrencyID != 2 ? payment.CurrencyID : assignationitem.CurrencyID;
                                                //if (ExchangeRates.FirstOrDefault(x => x.Date == exchangeDate && x.CurrencyID == exchangeCurrencyID) != null)
                                                //{
                                                //    currentRate = ExchangeRates.FirstOrDefault(x => x.Date == exchangeDate && x.CurrencyID == exchangeCurrencyID);
                                                //}
                                                //else
                                                //{
                                                

                                                rate = db.tblExchangeRates.Where(m => 
                                                    m.fromCurrencyID == exchangeCurrencyID 
                                                    && m.fromDate <= payment.Date 
                                                    && (m.permanent_ == true || m.toDate > payment.Date)
                                                    && m.providerID == null 
                                                    && m.toCurrencyID == 2 
                                                    && m.terminalID == coupon.tblPurchases.terminalID
                                                    && m.exchangeRateTypeID == 1
                                                    && m.tblExchangeRates_PointsOfSales.Count(
                                                            p => p.pointOfSaleID == unit.tblPurchases_Services.tblPurchases.pointOfSaleID
                                                            && p.dateAdded <= payment.Date
                                                            && (p.dateDeleted == null || p.dateDeleted > payment.Date)
                                                        ) > 0
                                                    ).OrderByDescending(m => m.fromDate).FirstOrDefault();

                                                if (rate == null)
                                                {
                                                    rate = db.tblExchangeRates.Where(m => m.fromCurrencyID == exchangeCurrencyID && m.fromDate <= payment.Date && m.providerID == null && m.toCurrencyID == 2 && m.terminalID == coupon.tblPurchases.terminalID && m.exchangeRateTypeID == 1 && m.tblExchangeRates_PointsOfSales.Count() == 0).OrderByDescending(m => m.fromDate).FirstOrDefault();
                                                }
                                                
                                                    currentRate = new ExchangeRatesModel.ExchangeRateForDate()
                                                    {
                                                        ExchangeRateID = rate.exchangeRateID,
                                                        Date = exchangeDate,
                                                        CurrencyCode = payment.CurrencyCode,
                                                        CurrencyID = payment.CurrencyID,
                                                        ExchangeRate = rate.exchangeRate
                                                    };
                                                //    ExchangeRates.Add(currentRate);
                                                //}
                                                if (assignationitem.CurrencyID == 2) //compra en pesos y pago en otra moneda
                                                {

                                                    assignationitem.PaymentSubtotal = new Money()
                                                    {
                                                        Amount = (assignationitem.Total.Amount - coveredAmount) / currentRate.ExchangeRate > payment.TotalRemaining ? payment.TotalRemaining : (assignationitem.Total.Amount - coveredAmount) / currentRate.ExchangeRate,
                                                        Currency = payment.CurrencyCode
                                                    };
                                                    payment.TotalRemaining -= assignationitem.PaymentSubtotal.Amount;
                                                    coveredAmount += assignationitem.PaymentSubtotal.Amount * currentRate.ExchangeRate;

                                                    assignationitem.ExchangeRate = currentRate.ExchangeRate;
                                                    assignationitem.ExchangeRateID = currentRate.ExchangeRateID;
                                                    assignationitem.PaymentDetailsID = payment.PaymentDetailsID;
                                                    assignationitem.PaymentType = payment.PaymentType;
                                                    assignationitem.PaymentTotal = payment.TotalPayment;
                                                    assignationitem.PaymentCurrencyID = payment.CurrencyID;
                                                    assignationitem.MoneyTransactionTypeID = payment.MoneyTransactionTypeID;

                                                }
                                                else if (assignationitem.CurrencyID != 2 && payment.CurrencyID == 2) //compra en dolares y pago en pesos
                                                {
                                                    assignationitem.PaymentSubtotal = new Money()
                                                    {
                                                        Amount = (assignationitem.Total.Amount - coveredAmount) * currentRate.ExchangeRate > payment.TotalRemaining ? payment.TotalRemaining : (assignationitem.Total.Amount - coveredAmount) * currentRate.ExchangeRate,
                                                        Currency = payment.CurrencyCode
                                                    };
                                                    payment.TotalRemaining -= assignationitem.PaymentSubtotal.Amount;
                                                    coveredAmount += assignationitem.PaymentSubtotal.Amount / currentRate.ExchangeRate;

                                                    assignationitem.ExchangeRate = currentRate.ExchangeRate;
                                                    assignationitem.ExchangeRateID = currentRate.ExchangeRateID;
                                                    assignationitem.PaymentDetailsID = payment.PaymentDetailsID;
                                                    assignationitem.PaymentType = payment.PaymentType;
                                                    assignationitem.PaymentTotal = payment.TotalPayment;
                                                    assignationitem.PaymentCurrencyID = payment.CurrencyID;
                                                    assignationitem.MoneyTransactionTypeID = payment.MoneyTransactionTypeID;
                                                }
                                                else if (assignationitem.CurrencyID == 1 && payment.CurrencyID == 3)
                                                {
                                                    //pago en otra moneda y compra en dolares
                                                    //obtener el tipo de cambio de dolar americano para multiplicar por el tipo de cambio USD y dividir entre CAD

                                                    ExchangeRatesModel.ExchangeRateForDate currentUSDRate = new ExchangeRatesModel.ExchangeRateForDate();

                                                    if (ExchangeRates.FirstOrDefault(x => x.Date == exchangeDate && x.CurrencyID == 1) != null)
                                                    {
                                                        currentUSDRate = ExchangeRates.FirstOrDefault(x => x.Date == exchangeDate && x.CurrencyID == 1);
                                                    }
                                                    else
                                                    {
                                                        rate = db.tblExchangeRates.Where(m => 
                                                            m.fromCurrencyID == 1 
                                                            && m.fromDate <= payment.Date 
                                                            && (m.permanent_ == true || m.toDate > payment.Date)
                                                            && m.providerID == null 
                                                            && m.toCurrencyID == 2 
                                                            && m.terminalID == coupon.tblPurchases.terminalID
                                                            && m.exchangeRateTypeID == 1
                                                            && m.tblExchangeRates_PointsOfSales.Count(
                                                                p => p.pointOfSaleID == coupon.tblPurchases.pointOfSaleID
                                                                && p.dateAdded <= payment.Date
                                                                && (p.dateDeleted == null || p.dateDeleted > payment.Date)
                                                            ) > 0
                                                            ).OrderByDescending(m => m.fromDate).FirstOrDefault();

                                                        if (rate == null)
                                                        {
                                                            rate = db.tblExchangeRates.Where(m => m.fromCurrencyID == 1 && m.fromDate <= payment.Date && m.providerID == null && m.toCurrencyID == 2 && m.terminalID == coupon.tblPurchases.terminalID && m.exchangeRateTypeID == 1 && m.tblExchangeRates_PointsOfSales.Count() == 0).OrderByDescending(m => m.fromDate).FirstOrDefault();
                                                        }
                                                        
                                                        currentUSDRate = new ExchangeRatesModel.ExchangeRateForDate()
                                                        {
                                                            ExchangeRateID = rate.exchangeRateID,
                                                            Date = exchangeDate,
                                                            CurrencyCode = payment.CurrencyCode,
                                                            CurrencyID = payment.CurrencyID,
                                                            ExchangeRate = rate.exchangeRate
                                                        };
                                                        ExchangeRates.Add(currentUSDRate);
                                                    }

                                                    assignationitem.PaymentSubtotal = new Money()
                                                    {
                                                        Amount = (assignationitem.Total.Amount - coveredAmount) * currentUSDRate.ExchangeRate / currentRate.ExchangeRate > payment.TotalRemaining ? payment.TotalRemaining : (assignationitem.Total.Amount - coveredAmount) * currentUSDRate.ExchangeRate / currentRate.ExchangeRate,
                                                        Currency = "CAD"
                                                    };
                                                    payment.TotalRemaining -= assignationitem.PaymentSubtotal.Amount;
                                                    coveredAmount += assignationitem.PaymentSubtotal.Amount * currentRate.ExchangeRate / currentUSDRate.ExchangeRate;

                                                    assignationitem.ExchangeRate = currentRate.ExchangeRate;
                                                    assignationitem.ExchangeRateID = currentRate.ExchangeRateID;
                                                    assignationitem.PaymentDetailsID = payment.PaymentDetailsID;
                                                    assignationitem.PaymentType = payment.PaymentType;
                                                    assignationitem.PaymentTotal = payment.TotalPayment;
                                                    assignationitem.PaymentCurrencyID = payment.CurrencyID;
                                                    assignationitem.MoneyTransactionTypeID = payment.MoneyTransactionTypeID;
                                                }

                                                if (assignationitem.PaymentSubtotal != null && assignationitem.PaymentSubtotal.Amount >= decimal.Round(decimal.Parse("0.01"), 2, MidpointRounding.AwayFromZero))
                                                {
                                                    closeoutitem.Assignations.Add(assignationitem);
                                                }
                                            }
                                        }                                        
                                    }
                                    if (i == 1)
                                    {
                                        uncoveredAmount += unitTotal - coveredAmount;
                                    }
                                    else if (i == 2)
                                    {
                                        if (coveredAmount == 0 && unitTotal > 0 && GetRemainingPayments(closeoutitem.Payments, i) == 0)
                                        {
                                            closeoutitem.Status = false;
                                            if (closeoutitem.DiagnosisMessage != null)
                                            {
                                                closeoutitem.DiagnosisMessage += "\n";
                                            }
                                            closeoutitem.DiagnosisMessage += "There is a pending refund for $" + Decimal.Round(unitTotal, 2, MidpointRounding.AwayFromZero)+ " " + Purchase.tblCurrencies.currencyCode;
                                            pendingRefund += unitTotal;
                                        }
                                        else if (coveredAmount > 0 && coveredAmount < totalToRefund - decimal.Parse("0.01") && GetRemainingPayments(closeoutitem.Payments, i) == 0)
                                        {
                                            pendingRefund += (totalToRefund - coveredAmount);
                                            //buscar un cancelation charge igual al pending refund
                                            if (coupon.tblPurchases_Services1.Count() > 0 && coupon.tblPurchases_Services1.FirstOrDefault().cancelationCharge == pendingRefund)
                                            {
                                                pendingRefund = 0;
                                            }
                                            if (pendingRefund > 0)
                                            {
                                                closeoutitem.Status = false;
                                                if (closeoutitem.DiagnosisMessage != null)
                                                {
                                                    closeoutitem.DiagnosisMessage += "\n";
                                                }
                                                closeoutitem.DiagnosisMessage += "There is a pending refund for $" + Decimal.Round(pendingRefund, 2, MidpointRounding.AwayFromZero) + " " + Purchase.tblCurrencies.currencyCode;
                                            }
                                        }
                                    }
                                }                               
                            }
                            refundsByReplacement += replacedTotal;
                        }
                    }
                    //extrapayments
                    decimal remainingPayments = GetRemainingPayments(closeoutitem.Payments, 1) + refundsByReplacement;
                    if (remainingPayments > decimal.Parse("0.01"))
                    {
                        //si hay extrapayments, buscar reembolsos y compararlos
                        foreach (var p in closeoutitem.Payments.Where(x => x.TotalRemaining > decimal.Parse("0.01") && x.MoneyTransactionTypeID == 1))
                        {
                            var currentRefund = closeoutitem.Payments.FirstOrDefault(x => decimal.Round(x.TotalRemaining, 2, MidpointRounding.AwayFromZero) >= decimal.Round(p.TotalRemaining, 2, MidpointRounding.AwayFromZero) && x.MoneyTransactionTypeID == 2 && x.CurrencyID == p.CurrencyID);


                            decimal paymentInOtherCurrency = p.TotalRemaining;
                            if (currentRefund == null)
                            {                                
                                if (p.CurrencyID == 2)
                                {
                                    if(rate!= null)
                                    {
                                        paymentInOtherCurrency = decimal.Round(p.TotalRemaining / rate.exchangeRate, 2);
                                    }
                                    
                                } else if(p.CurrencyID == 1)
                                {
                                    if (rate != null)
                                    {
                                        paymentInOtherCurrency = decimal.Round(p.TotalRemaining * rate.exchangeRate, 2);
                                    }
                                }
                                currentRefund = closeoutitem.Payments.FirstOrDefault(x => decimal.Round(x.TotalRemaining, 2, MidpointRounding.AwayFromZero) >= decimal.Round(paymentInOtherCurrency, 2, MidpointRounding.AwayFromZero) && x.MoneyTransactionTypeID == 2 && x.CurrencyID != p.CurrencyID);
                            }

                            if (currentRefund != null)
                            {
                                PaymentAssignationViewModel.ExtraPaymentItem ep = new PaymentAssignationViewModel.ExtraPaymentItem();
                                ep.ExtraAmount = new Money()
                                {
                                    Amount = p.TotalRemaining,
                                    Currency = p.CurrencyCode
                                };
                                ep.ExtraFromPaymentDetailsID = p.PaymentDetailsID;
                                ep.PaymentDetailDescription = p.TotalPayment + " " + p.CurrencyCode;
                                ep.RefundPaymentDetailsID = currentRefund.PaymentDetailsID;
                                ep.RefundDetailDescription = currentRefund.TotalPayment + " " + currentRefund.CurrencyCode;
                                closeoutitem.ExtraPayments.Add(ep);

                                decimal remainingToCover = paymentInOtherCurrency;
                                currentRefund.TotalRemaining -= remainingToCover;
                                p.TotalRemaining -= p.TotalRemaining;
                            }
                        }

                        //una vez aplicados los reembolsos, ahora si verificar y mostrar mensajes
                        remainingPayments = GetRemainingPayments(closeoutitem.Payments, 1, ExchangeRates) + refundsByReplacement;
                        if (remainingPayments > decimal.Parse("0.1"))
                        {
                            if (closeoutitem.DiagnosisMessage != null)
                            {
                                closeoutitem.DiagnosisMessage += "\n";
                            }
                            closeoutitem.DiagnosisMessage += "Remaining Payments to Refund: $" + remainingPayments.ToString();
                        }
                    }

                    if (remainingPayments <= decimal.Parse("0.1") && closeoutitem.Status == true)
                    {
                        closeoutitem.Status = true;
                    }
                    else
                    {
                        closeoutitem.Status = false;
                    }

                    if (uncoveredAmount > decimal.Parse("0.1"))
                    {
                        if (decimal.Round(uncoveredAmount, 2, MidpointRounding.AwayFromZero) == decimal.Round(pendingRefund, 2, MidpointRounding.AwayFromZero) || decimal.Round(uncoveredAmount, 2, MidpointRounding.AwayFromZero) - decimal.Round(pendingRefund, 2, MidpointRounding.AwayFromZero) < decimal.Parse("0.1"))
                        {
                            closeoutitem.Status = true;
                            closeoutitem.DiagnosisMessage = "Virtual Balance applied";
                        }
                        else
                        {
                            closeoutitem.Status = false;
                            if (closeoutitem.DiagnosisMessage != null)
                            {
                                closeoutitem.DiagnosisMessage += "\n";
                            }
                            closeoutitem.DiagnosisMessage += "Not enought payments.\nThere is a Debt of: $" + uncoveredAmount + " " + Purchase.tblCurrencies.currencyCode;
                        }
                    }

                    assignation.CloseOuts.Add(closeoutitem);
                }
            }

            return assignation;
        }

        private List<PaymentAssignationViewModel.PaymentItem> GetPaymentsList(IEnumerable<tblPaymentDetails> Payments)
        {
            List<tblBankCommissions> bankCommissions = new List<tblBankCommissions>();
            List<PaymentAssignationViewModel.PaymentItem> paymentsList = new List<PaymentAssignationViewModel.PaymentItem>();
            foreach (var payment in Payments)
            {
                PaymentAssignationViewModel.PaymentItem paymentitem = new PaymentAssignationViewModel.PaymentItem();
                paymentitem.PaymentDetailsID = payment.paymentDetailsID;
                paymentitem.Date = payment.dateSaved;
                paymentitem.PaymentType = Utils.GeneralFunctions.PaymentTypes[payment.paymentType.ToString()];
                if (payment.savedByUserID != null)
                {
                    tblUserProfiles user = payment.aspnet_Users.tblUserProfiles.FirstOrDefault();
                    paymentitem.SavedBy = user.firstName + " " + user.lastName;
                }
                else
                {
                    paymentitem.SavedBy = "Website";
                }
                if (payment.paymentType == 2)
                {
                    if (payment.applyCommission == true)
                    {
                        decimal bankCommission = 0;

                        if (bankCommissions.Count(b => b.terminalID == payment.tblPurchases.terminalID && b.initialDate <= payment.dateSaved && b.finalDate > payment.dateSaved && (b.cardTypeID == payment.ccType || b.cardTypeID == null)) == 0)
                        {
                            bankCommissions.Add(MasterChartDataModel.Purchases.GetBankCommissionObject(payment.tblPurchases.terminalID, payment.dateSaved, payment.ccType));
                        }

                        bankCommission = bankCommissions.First(b => b.terminalID == payment.tblPurchases.terminalID && b.initialDate <= payment.dateSaved && b.finalDate > payment.dateSaved && (b.cardTypeID == payment.ccType || b.cardTypeID == null)).commissionPercentage;

                        paymentitem.TotalPayment = payment.amount;
                        paymentitem.TotalRemaining = payment.amount / (bankCommission / 100 + 1);
                        paymentitem.PaymentType += " with commission included";
                    }
                    else
                    {
                        paymentitem.TotalPayment = payment.amount;
                        paymentitem.TotalRemaining = payment.amount;
                    }
                }
                else
                {
                    paymentitem.TotalPayment = payment.amount;
                    paymentitem.TotalRemaining = payment.amount;
                }
                paymentitem.CurrencyID = (int)payment.currencyID;
                paymentitem.CurrencyCode = payment.tblCurrencies.currencyCode;
                paymentitem.MoneyTransactionTypeID = payment.tblMoneyTransactions.transactionTypeID;
                paymentsList.Add(paymentitem);
            }
            return paymentsList;
        }

        private List<PaymentAssignationViewModel.ExtraPaymentItem> GetNullablePayments(List<PaymentAssignationViewModel.PaymentItem> Payments)
        {
            List<PaymentAssignationViewModel.ExtraPaymentItem> extrapayments = new List<PaymentAssignationViewModel.ExtraPaymentItem>();

            foreach (var p in Payments.Where(p => p.MoneyTransactionTypeID == 1))
            {
                foreach (var r in Payments.Where(r => r.MoneyTransactionTypeID == 2 && r.PaymentType == p.PaymentType && r.Date >= p.Date))
                {
                    if (p.TotalRemaining == r.TotalRemaining)
                    {
                        PaymentAssignationViewModel.ExtraPaymentItem ep = new PaymentAssignationViewModel.ExtraPaymentItem();
                        ep.ExtraAmount = new Money()
                        {
                            Amount = p.TotalRemaining,
                            Currency = p.CurrencyCode
                        };
                        ep.ExtraFromPaymentDetailsID = p.PaymentDetailsID;
                        ep.PaymentDetailDescription = p.TotalPayment + " " + p.CurrencyCode;
                        ep.RefundPaymentDetailsID = r.PaymentDetailsID;
                        ep.RefundDetailDescription = r.TotalPayment + " " + r.CurrencyCode;
                        extrapayments.Add(ep);
                        p.TotalRemaining -= r.TotalRemaining;
                        r.TotalRemaining -= r.TotalRemaining;
                    }
                }
            }

            return extrapayments;
        }

        private decimal GetRemainingPayments(List<PaymentAssignationViewModel.PaymentItem> payments, int type, List<ExchangeRatesModel.ExchangeRateForDate> ExchangeRates = null)
        {
            decimal remainingTotal = 0;
            int currencyid = 0;

            foreach (PaymentAssignationViewModel.PaymentItem payment in payments.Where(p => p.MoneyTransactionTypeID == type))
            {
                if (payment.TotalRemaining > 0)
                {
                    remainingTotal += payment.TotalRemaining;
                    currencyid = payment.CurrencyID;
                }
            }

            if (ExchangeRates != null)
            {
                //buscar refund en dólares
                foreach (PaymentAssignationViewModel.PaymentItem payment in payments.Where(p => p.MoneyTransactionTypeID != type && p.CurrencyID == currencyid))
                {
                    remainingTotal -= payment.TotalRemaining;
                }

                //buscar refund en otra moneda
                foreach (PaymentAssignationViewModel.PaymentItem payment in payments.Where(p => p.MoneyTransactionTypeID != type && p.CurrencyID != currencyid))
                {
                    ExchangeRatesModel.ExchangeRateForDate exchangeRate1 = ExchangeRates.FirstOrDefault(x => x.Date == payment.Date.Date && x.CurrencyID == currencyid);
                    if (exchangeRate1 == null)
                    {
                        exchangeRate1 = new ExchangeRatesModel.ExchangeRateForDate()
                        {
                            ExchangeRate = 1
                        };
                    }
                    ExchangeRatesModel.ExchangeRateForDate exchangeRate2 = ExchangeRates.FirstOrDefault(x => x.Date == payment.Date && x.CurrencyID == payment.CurrencyID);
                    if (exchangeRate2 == null)
                    {
                        exchangeRate2 = new ExchangeRatesModel.ExchangeRateForDate()
                        {
                            ExchangeRate = 1
                        };
                    }
                    remainingTotal -= payment.TotalRemaining / exchangeRate1.ExchangeRate * exchangeRate2.ExchangeRate;
                }
            }

            return remainingTotal;
        }

        public AttemptResponse SavePaymentAssignation(Guid purchaseid, long closeoutid, DateTime closeoutDate)
        {
            AttemptResponse attempt = new AttemptResponse();
            UserSession us = new UserSession();

            PaymentAssignationViewModel Assignations = GetPaymentsAssignation(purchaseid, closeoutDate.ToString("yyyy-MM-dd"));

            foreach (var closeout in Assignations.CloseOuts)
            {
                if (closeout.CloseOutID == closeoutid)
                {
                    foreach (var assignationsItem in closeout.Assignations)
                    {
                        var assignation = (from a in db.tblPaymentsAssignation
                                           where a.paymentDetailsID == assignationsItem.PaymentDetailsID
                                           && a.purchaseServiceDetailID == assignationsItem.PurchaseServiceDetailID
                                           select a).FirstOrDefault();

                        if (assignation == null)
                        {
                            tblPaymentsAssignation newAssignation = new tblPaymentsAssignation();
                            newAssignation.dateSaved = DateTime.Now;
                            newAssignation.savedByUserID = us.UserID;
                            newAssignation.purchaseServiceDetailID = assignationsItem.PurchaseServiceDetailID;
                            newAssignation.unitAmount = assignationsItem.Total.Amount;
                            newAssignation.unitCurrencyID = assignationsItem.CurrencyID;
                            newAssignation.paymentDetailsID = assignationsItem.PaymentDetailsID;
                            newAssignation.paymentAmount = assignationsItem.PaymentSubtotal.Amount;
                            newAssignation.paymentCurrencyID = assignationsItem.PaymentCurrencyID;
                            newAssignation.exchangeRateID = assignationsItem.ExchangeRateID;
                            newAssignation.moneyTransactionTypeID = assignationsItem.MoneyTransactionTypeID;
                            newAssignation.description = assignationsItem.PaymentType;
                            if (closeout.CloseOutID != 0)
                            {
                                newAssignation.closeOutID = closeout.CloseOutID;
                            }
                            else
                            {
                                newAssignation.closeOutID = closeoutid;
                            }

                            db.tblPaymentsAssignation.AddObject(newAssignation);
                        }

                        //agregar legacy de relacion de cupones con moneytransactions
                        if (assignationsItem.PaymentDetailsID != null)
                        {
                            var legacyRelationship = (from l in db.tblPaymentDetails
                                                      where l.paymentDetailsID == assignationsItem.PaymentDetailsID
                                                      && l.tblMoneyTransactions.tblPurchases_Services_MoneyTransactions.Count(x => x.purchase_ServiceID == assignationsItem.PurchaseServiceID) > 0
                                                      select l.moneyTransactionID).FirstOrDefault();
                            if (legacyRelationship == null && assignationsItem.PaymentDetailsID != null)
                            {
                                long? moneyTransactionID = db.tblPaymentDetails.FirstOrDefault(p => p.paymentDetailsID == assignationsItem.PaymentDetailsID).moneyTransactionID;

                                if (moneyTransactionID != null)
                                {
                                    tblPurchases_Services_MoneyTransactions psmt = new tblPurchases_Services_MoneyTransactions();
                                    psmt.purchase_ServiceID = assignationsItem.PurchaseServiceID;
                                    psmt.moneyTransactionID = (long)moneyTransactionID;
                                    db.tblPurchases_Services_MoneyTransactions.AddObject(psmt);
                                }
                            }
                        }
                    }
                }
            }

            db.SaveChanges();

            attempt.Type = Attempt_ResponseTypes.Ok;
            attempt.Exception = null;

            return attempt;
        }
    }
}
