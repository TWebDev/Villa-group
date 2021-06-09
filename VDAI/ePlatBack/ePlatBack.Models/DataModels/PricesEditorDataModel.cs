using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;

namespace ePlatBack.Models.DataModels
{
    public class PricesEditorDataModel
    {
        public static PricesEditorViewModel.PriceInfo GetPricesInfo(PricesEditorViewModel.ParamsPriceEditor model)
        {
            PricesEditorViewModel.PriceInfo priceInfo = new PricesEditorViewModel.PriceInfo();
            DateTime date = DateTime.Parse(model.ParamsPricesEditor_Date);

            ePlatEntities db = new ePlatEntities();

            var ServiceInfo = (from service in db.tblServices
                              join provider in db.tblProviders on service.providerID equals provider.providerID
                              where service.serviceID == model.ParamsPricesEditor_ServiceID
                              select new
                              {
                                  service.service,
                                  provider.comercialName,
                                  provider.contractCurrencyID,
                                  provider.providerID,
                                  contract = provider.tblContractsCurrencyHistory.OrderByDescending(x => x.dateSaved).FirstOrDefault()
                              }).FirstOrDefault();

            priceInfo.Provider = ServiceInfo.comercialName;
            priceInfo.Item = ServiceInfo.service;
            priceInfo.ProviderContractCurrency = (ServiceInfo.contractCurrencyID != null ? (ServiceInfo.contractCurrencyID == 1 ? "USD" : "MXN") : "USD & MXN");
            priceInfo.ProviderContractDate = "From "  + ServiceInfo.contract.dateSaved.ToString("yyyy-MM-dd");
            int exchangeRateTypeID = 1;
            if (model.ParamsPricesEditor_Nationality == "en-US")
            {
                exchangeRateTypeID = 2;
            }
            else if (model.ParamsPricesEditor_Nationality == "es-MX")
            {
                exchangeRateTypeID = 3;

            }
            PriceExchangeRate exchangeRate = PriceDataModel.GetExchangeRateToApply(model.ParamsPricesEditor_TerminalID, ServiceInfo.providerID, model.ParamsPricesEditor_PointOfSaleID, date, exchangeRateTypeID);

            priceInfo.ExchangeRate = exchangeRate.Rate;
            priceInfo.ExchangeRateType = exchangeRate.Type;
            
            priceInfo.Rules = PriceDataModel.GetRules(model.ParamsPricesEditor_ServiceID, model.ParamsPricesEditor_TerminalID, date);
            priceInfo.Prices = PriceDataModel.GetComputedPrices(model.ParamsPricesEditor_ServiceID, date, model.ParamsPricesEditor_PointOfSaleID,model.ParamsPricesEditor_TerminalID,date, (model.ParamsPricesEditor_Nationality != null ? model.ParamsPricesEditor_Nationality : ""), null);
            priceInfo.RoundingRules = new List<PricesEditorViewModel.RoundingRuleModel>();
            //contemplar avoidRounding y tblPriceTypesRounding
            var avoidRounding = (from s in db.tblServices
                                  where s.serviceID == model.ParamsPricesEditor_ServiceID
                                  select s.avoidRounding).FirstOrDefault();

            if (avoidRounding == null || avoidRounding == false)
            {
                //revisar tabla de rounding
                var roundingRules = from r in db.tblPriceTypesRounding
                                    where r.terminalID == model.ParamsPricesEditor_TerminalID
                                    && r.fromDate <= date
                                    && (r.toDate == null || r.toDate > date)
                                    select r;

                foreach (var rule in roundingRules)
                {
                    PricesEditorViewModel.RoundingRuleModel newRule = new PricesEditorViewModel.RoundingRuleModel();
                    newRule.PriceTypeID = rule.priceTypeID;
                    newRule.RoundUp = rule.roundUp;
                    newRule.RoundDown = rule.roundDown;
                    newRule.RoundToFifty = rule.roundToFifty;
                    priceInfo.RoundingRules.Add(newRule);
                }
            }

            return priceInfo;
        }
    }
}
