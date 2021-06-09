using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using System.Web;

namespace ePlatBack.Models.DataModels
{
    public class PromoDataModel
    {
        ePlatEntities db = new ePlatEntities();

        public PromoViewModel GetPromoForItemID(long id, long itemTypeId)
        {
            PromoViewModel promo = new PromoViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();
            var PromoQ = (from p in db.tblPromos_RelatedItems
                          join qpromo in db.tblPromos on p.promoID equals qpromo.promoID
                          join description in db.tblPromoDescriptions on p.promoID equals description.promoID
                          where p.itemID == id
                          && p.sysItemTypeID == itemTypeId
                          && qpromo.fromDateBW <= DateTime.Today
                          && qpromo.toDateBW >= DateTime.Today
                          && description.culture == culture
                          select description).FirstOrDefault();

            if (PromoQ != null)
            {
                promo.MainTag = PromoQ.mainTag;
                promo.TitleTag = PromoQ.title;
                promo.Instructions = PromoQ.instructions;
                promo.Description = PromoQ.description;
                if (itemTypeId == 1 || itemTypeId == 3)
                {
                    promo.TagColor = db.tblServices.FirstOrDefault(x => x.serviceID == id).tblProviders.tagColor;
                }
                if (promo.TagColor == null)
                {
                    promo.TagColor = "rgba(255,0,0,.75)";
                }
                if (promo.TagColor.IndexOf("rgb") >= 0)
                {
                    string colorsStr = promo.TagColor.Substring(promo.TagColor.IndexOf("(") + 1);
                    colorsStr = colorsStr.Remove(colorsStr.IndexOf(")"));
                    string[] colors = colorsStr.Split(',');
                    var colorTotal = Convert.ToInt32(colors[0]) * 0.299 + Convert.ToInt32(colors[1]) * 0.587 + Convert.ToInt32(colors[2]) * 0.114;
                    if (colorTotal > 186)
                    {
                        promo.TextColor = "#000000";
                    }
                    else
                    {
                        promo.TextColor = "#ffffff";
                    }
                }
            }

            return promo;
        }

        public static AttemptResponse GetPromo(string serviceids, string traveldates, long? terminalid = null, string promocode = null)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse attempt = new AttemptResponse();
            List<ApplicablePromo> promos = new List<ApplicablePromo>();
            string culture = Utils.GeneralFunctions.GetCulture();
            int currency = (culture == "en-US" ? 1 : 2);
            if (terminalid == null)
            {
                terminalid = Utils.GeneralFunctions.GetTerminalID();
            }

            string[] arrayStr = serviceids.Split(',');
            string[] arrayTDStr = traveldates.Split(',');

            List<long> listids = new List<long>();
            foreach (string s in arrayStr)
            {
                listids.Add(Convert.ToInt64(s));
            }

            List<DateTime> listtds = new List<DateTime>();
            foreach (string d in arrayTDStr)
            {
                listtds.Add(Convert.ToDateTime(d));
            }

            var PromosQ = (from p in db.tblPromos_RelatedItems
                           join promo in db.tblPromos on p.promoID equals promo.promoID
                           where promo.terminalID == terminalid
                           && listids.Contains(p.itemID)
                           && p.sysItemTypeID == 1
                           && promo.fromDateBW <= DateTime.Today
                           && promo.toDateBW >= DateTime.Today
                           && promo.publish == true
                           && (promo.currencyID == null || promo.currencyID == currency)
                           orderby promo.amount descending
                           select promo).Distinct();

            string message = "";


                //no hay promos con relación
                var PromosNRQ = (from p in db.tblPromos
                           where p.terminalID == terminalid
                           && p.fromDateBW <= DateTime.Today
                           && p.toDateBW >= DateTime.Today
                           && p.publish == false
                           && (p.currencyID == null || p.currencyID == currency)
                           select p).Distinct();
                PromosQ = PromosQ.Concat(PromosNRQ);


            //filtro de promocode
            if (promocode != null && promocode != "")
            {
                PromosQ = PromosQ.Where(x => x.promoCode == promocode);
            }
            else
            {
                PromosQ = PromosQ.Where(x => x.promoCode == null || x.promoCode == "");
            }

            //check if domain is public
            string domain = System.Web.HttpContext.Current.Request.Url.Host;



            var IsPublic = (from d in db.tblTerminalDomains
                            where d.domain == domain
                            select d.isPublic).FirstOrDefault();

            if (domain.IndexOf("ePlat") >= 0 || IsPublic == null)
            {
                PromosQ = PromosQ.Where(x => x.applyOnEplat);
            }
            else if(IsPublic == true)
            {
                PromosQ = PromosQ.Where(x => x.applyOnPublic);
            }
            else
            {
                PromosQ = PromosQ.Where(x => x.applyOnPrivate);
            }

            foreach (tblPromos promo in PromosQ)
            {
                foreach (tblPromos_PromoTypes type in promo.tblPromos_PromoTypes.OrderByDescending(x => x.promoTypeID))
                {
                    ApplicablePromo rPromo = new ApplicablePromo();
                    rPromo.PromoID = promo.promoID;
                    rPromo.Promo = (promo.tblPromoDescriptions.Count() > 0 ? promo.tblPromoDescriptions.FirstOrDefault(x => x.culture == culture).title : promo.promo);
                    rPromo.ApplyOnPerson = promo.applyOnPerson;
                    rPromo.PromoCode = promo.promoCode;
                    rPromo.ServiceIDs = "";
                    int listIndex = 0;
                    
                    List<long> idsPromoQ = new List<long>();
                    bool applicable = true;
                    foreach (long sid in listids)
                    {
                        DateTime cdate = listtds[listIndex];
                        var PromoItemQ = (from x in db.tblPromos_RelatedItems
                                          where x.itemID == sid
                                          && x.promoID == promo.promoID
                                          && x.tblPromos.fromDateTW <= cdate
                                          && (x.tblPromos.toDateTW >= cdate || x.tblPromos.toDateTW == null)
                                          select x.promo_RelatedItemID).Count();

                        if (PromoItemQ > 0)
                        {
                            if (rPromo.ServiceIDs != "")
                            {
                                rPromo.ServiceIDs += ",";
                            }
                            rPromo.ServiceIDs += sid.ToString();
                            idsPromoQ.Add(sid);
                            applicable = true;
                        }
                        else
                        {
                            if (promocode == "" && idsPromoQ.Count() == 0)
                            {
                                applicable = false;
                            }
                        }
                    }
                    //var idsPromoQ = from x in db.tblPromos_RelatedItems
                    //                where arrayids.Contains(x.itemID)
                    //                && x.promoID == promo.promoID
                    //                select x.itemID;


                    //foreach (long item in idsPromoQ)
                    //{
                    //    if (rPromo.ServiceIDs != "")
                    //    {
                    //        rPromo.ServiceIDs += ",";
                    //    }
                    //    rPromo.ServiceIDs += item.ToString();
                    //}
                    if (applicable)
                    {
                        bool valid = false;
                        if (type.promoTypeID == 4)
                        {
                            //descuento
                            rPromo.PromoTypeID = type.promoTypeID;
                            rPromo.PromoType = type.tblPromoTypes.promoType;
                            rPromo.Discount = (promo.amount != null ? (decimal)promo.amount : (decimal)promo.percentage);
                            rPromo.DiscountType = (promo.amount != null ? "$" : "%");
                            rPromo.Apply = true;
                            rPromo.Condition = "";
                            valid = true;
                        }
                        else
                        {
                            if (promo.applyOnPerson)
                            {
                                //por persona
                                rPromo.PromoTypeID = type.promoTypeID;
                                rPromo.PromoType = type.tblPromoTypes.promoType;
                                rPromo.Apply = true;
                                rPromo.Condition = "";
                                valid = true;
                            }
                            else
                            {
                                //por tour
                                if (type.promoTypeID == 3 && valid == false)
                                {
                                    if (idsPromoQ.Count() >= 4)
                                    {
                                        rPromo.PromoTypeID = type.promoTypeID;
                                        rPromo.PromoType = type.tblPromoTypes.promoType;
                                        rPromo.Apply = true;
                                        rPromo.Condition = "";
                                        valid = true;
                                    }
                                    else
                                    {
                                        rPromo.PromoTypeID = type.promoTypeID;
                                        rPromo.PromoType = type.tblPromoTypes.promoType;
                                        rPromo.Apply = false;
                                        rPromo.Condition = Resources.Models.Promo.PromoStrings.Condition_4x3;
                                        valid = false;
                                    }
                                }
                                else if (type.promoTypeID == 2 && valid == false)
                                {
                                    if (idsPromoQ.Count() >= 3)
                                    {
                                        rPromo.PromoTypeID = type.promoTypeID;
                                        rPromo.PromoType = type.tblPromoTypes.promoType;
                                        rPromo.Apply = true;
                                        rPromo.Condition = "";
                                        valid = true;
                                    }
                                    else
                                    {
                                        rPromo.PromoTypeID = type.promoTypeID;
                                        rPromo.PromoType = type.tblPromoTypes.promoType;
                                        rPromo.Apply = false;
                                        rPromo.Condition = Resources.Models.Promo.PromoStrings.Condition_3x2;
                                        valid = false;
                                    }
                                }
                                else if (type.promoTypeID == 1 && valid == false)
                                {
                                    if (idsPromoQ.Count() >= 2)
                                    {
                                        rPromo.PromoTypeID = type.promoTypeID;
                                        rPromo.PromoType = type.tblPromoTypes.promoType;
                                        rPromo.Apply = true;
                                        rPromo.Condition = "";
                                        valid = true;
                                    }
                                    else
                                    {
                                        rPromo.PromoTypeID = type.promoTypeID;
                                        rPromo.PromoType = type.tblPromoTypes.promoType;
                                        rPromo.Apply = false;
                                        rPromo.Condition = Resources.Models.Promo.PromoStrings.Condition_2x1;
                                        valid = false;
                                    }
                                }
                            }
                        }
                        promos.Add(rPromo);
                    }                    
                }
            }

            attempt.Type = Attempt_ResponseTypes.Ok;
            attempt.Exception = null;
            attempt.ObjectID = promos;
            return attempt;
        }

        public static decimal ApplyPromo(decimal amount, long purchaseServiceDetailID) {
            ePlatEntities db = new ePlatEntities();
            decimal total = amount;

            //revisar promo aplicada
            long promoID = 0;
            var purchaseServicePromo = (from s in db.tblPurchaseServiceDetails
                                       where s.purchaseServiceDetailID == purchaseServiceDetailID
                                       select new
                                       {
                                           servicePromoID = s.tblPurchases_Services.promoID,
                                           purchasePromoID = s.tblPurchases_Services.tblPurchases.promoID,
                                           currencyID = s.tblPurchases_Services.currencyID,
                                           promo = s.promo
                                       }).FirstOrDefault();

            if (purchaseServicePromo.servicePromoID != null)
            {
                promoID = (long)purchaseServicePromo.servicePromoID;
            }
            else if(purchaseServicePromo.purchasePromoID != null)
            {
                promoID = (long)purchaseServicePromo.purchasePromoID;
            }

            //revisar info de promo
            if (promoID != 0 && purchaseServicePromo.promo)
            {
                CalculatePromo(promoID, ref total, ref db);
            }

            //revisar si la compra tiene promo general
            var PurchasePromoID = (from p in db.tblPurchaseServiceDetails
                                where p.purchaseServiceDetailID == purchaseServiceDetailID
                                select p.tblPurchases_Services.tblPurchases.promoID).FirstOrDefault();

            if (PurchasePromoID != null)
            {
                CalculatePromo(PurchasePromoID, ref total, ref db);
            }

            return total;
        }

        public static void CalculatePromo(long? promoID, ref decimal total, ref ePlatEntities db)
        {
            var promoInfo = (from p in db.tblPromos
                             where p.promoID == promoID
                             select p).FirstOrDefault();

            if (promoInfo.tblPromos_PromoTypes.Count(p => p.promoTypeID == 4) > 0)
            {
                if (promoInfo.amount != null && promoInfo.amount > 0)
                {
                    //descuento de dinero

                }
                else if (promoInfo.percentage != null && promoInfo.percentage > 0)
                {
                    //descuento de porcentaje
                    total = total - (total * (decimal)promoInfo.percentage / 100);
                }
            }
            else if (promoInfo.tblPromos_PromoTypes.Count(p => p.promoTypeID == 1) > 0
                || promoInfo.tblPromos_PromoTypes.Count(p => p.promoTypeID == 2) > 0
                || promoInfo.tblPromos_PromoTypes.Count(p => p.promoTypeID == 3) > 0)
            {
                total = 0;
            }
        }
    }
}
