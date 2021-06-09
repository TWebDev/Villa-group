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
using System.Globalization;

namespace ePlatBack.Models.DataModels
{
    public class SPIDataModel
    {
        public static UserSession session = new UserSession();
        public static SPIViewModel.CustomerHistory GetCustomerHistory(int customerID)
        {
            SPIViewModel.CustomerHistory CustomerHistory = new SPIViewModel.CustomerHistory();

            SPIModels.TheVillaGroup.SpiTvgEntities spidb = new SPIModels.TheVillaGroup.SpiTvgEntities();

            //una vez obtenido el idcustomer solicitar el historial
            CustomerHistory.CustomerID = customerID;
            CustomerHistory.History = new List<SPIViewModel.CustomerTour>();

            List<SPIModels.TheVillaGroup.spCustomer_History_Result> History = spidb.spCustomer_History(customerID).ToList();
            foreach (var tour in History.OrderByDescending(x => x.TourDate))
            {
                SPIViewModel.CustomerTour newTour = new SPIViewModel.CustomerTour();
                if (tour.TourDate != null)
                {
                    newTour.TourDate = tour.TourDate.Value.ToString("yyyy-MM-dd");
                }
                newTour.SalesCenter = tour.SalesCenter;
                newTour.TourSource = tour.Source;
                newTour.SourceGroup = tour.SourceGroup;
                newTour.SourceItem = tour.SourceItem;
                newTour.Qualification = tour.Qualification;
                newTour.TourContractNumber = tour.ContractNumber != null ? tour.ContractNumber.Trim() : "";
                newTour.Volume = tour.Volume;
                newTour.LegalNames = new List<SPIViewModel.LegalName>();

                if (tour.LegalNames != null)
                {
                    string[] legalNames = tour.LegalNames.Split(',');
                    foreach (var n in legalNames)
                    {
                        string[] cLegalName = n.Split('|');
                        SPIViewModel.LegalName newLegal = new SPIViewModel.LegalName();
                        newLegal.Name = cLegalName[0];
                        newLegal.DateOfBirth = DateTime.Parse(cLegalName[1]).ToString("yyyy-MM-dd");
                        if (cLegalName[1] != null)
                        {
                            DateTime birthday = DateTime.Parse(cLegalName[1]);
                            DateTime today = DateTime.Today;
                            var age = today.Year - birthday.Year;
                            if (birthday > today.AddYears(-age)) age--;
                            newLegal.Age = age;
                        }
                        newTour.LegalNames.Add(newLegal);
                    }
                }

                CustomerHistory.History.Add(newTour);
            }

            return CustomerHistory;
        }

        public static SPIViewModel.CustomerHistoryResults SearchCustomerHistory(string firstname, string lastname)
        {
            SPIModels.TheVillaGroup.SpiTvgEntities spidb = new SPIModels.TheVillaGroup.SpiTvgEntities();
            SPIViewModel.CustomerHistoryResults customerHistory = new SPIViewModel.CustomerHistoryResults();
            customerHistory.History = new List<SPIViewModel.CustomerHistoryItem>();
            customerHistory.LegalNames = new List<SPIViewModel.LegalName>();

            //buscar clientes
            List<SPIViewModel.CustomerSearchResult> customerMatches = new List<SPIViewModel.CustomerSearchResult>();
            if (lastname.Trim().IndexOf(" ") >= 0)
            {
                lastname = lastname.Substring(0, lastname.IndexOf(" "));
            }

            if (firstname.Trim().IndexOf(" ") >= 0)
            {
                firstname = firstname.Substring(0, firstname.IndexOf(" "));
            }

            List<SPIModels.TheVillaGroup.spCustomer_check_tours_Result> SearchResults = spidb.spCustomer_check_tours(lastname, firstname, null, null, null, null, null).ToList();

            var SearchSales = (from s in SearchResults
                               select new
                               {
                                   s.id,
                                   s.customer_name,
                                   s.customer_name2,
                                   s.Account_no
                               }).Distinct();

            var totalMatch = from m in SearchSales
                             where m.customer_name.Contains(lastname + " ")
                             || m.customer_name2.Contains(lastname + " ")
                             select m;

            if (totalMatch.Count() == 1)
            {
                SPIViewModel.CustomerSearchResult cus = new SPIViewModel.CustomerSearchResult();
                cus.CustomerID = totalMatch.FirstOrDefault().id;
                cus.Customer = totalMatch.FirstOrDefault().customer_name + (totalMatch.FirstOrDefault().customer_name2.Trim() != "" ? " & " + totalMatch.FirstOrDefault().customer_name2 : "");
                cus.AccountNumber = totalMatch.FirstOrDefault().Account_no;
                customerMatches.Add(cus);
            }
            else if (totalMatch.Count() > 1)
            {
                foreach (var customer in totalMatch)
                {
                    if (customerMatches.Count(x => x.CustomerID == customer.id) == 0)
                    {
                        SPIViewModel.CustomerSearchResult cus = new SPIViewModel.CustomerSearchResult();
                        cus.CustomerID = customer.id;
                        cus.Customer = customer.customer_name + (customer.customer_name2.Trim() != "" ? " & " + customer.customer_name2 : "");
                        cus.AccountNumber = customer.Account_no;
                        customerMatches.Add(cus);
                    }
                    else
                    {
                        //ya existe ese customerid
                        if (customer.Account_no != null)
                        {
                            if (customerMatches.FirstOrDefault(x => x.CustomerID == customer.id).AccountNumber != null && customerMatches.FirstOrDefault(x => x.CustomerID == customer.id).AccountNumber != "")
                            {
                                customerMatches.FirstOrDefault(x => x.CustomerID == customer.id).AccountNumber += ", ";
                            }
                            customerMatches.FirstOrDefault(x => x.CustomerID == customer.id).AccountNumber += customer.Account_no;
                        }
                    }
                }
            }
            else
            {
                foreach (var customer in SearchSales)
                {
                    if (customerMatches.Count(x => x.CustomerID == customer.id) == 0)
                    {
                        SPIViewModel.CustomerSearchResult cus = new SPIViewModel.CustomerSearchResult();
                        cus.CustomerID = customer.id;
                        cus.Customer = customer.customer_name + (customer.customer_name2.Trim() != "" ? " & " + customer.customer_name2 : "");
                        cus.AccountNumber = customer.Account_no;
                        customerMatches.Add(cus);
                    }
                    else
                    {
                        //ya existe ese customerid
                        if (customer.Account_no != null)
                        {
                            if (customerMatches.FirstOrDefault(x => x.CustomerID == customer.id).AccountNumber != null && customerMatches.FirstOrDefault(x => x.CustomerID == customer.id).AccountNumber != "")
                            {
                                customerMatches.FirstOrDefault(x => x.CustomerID == customer.id).AccountNumber += ", ";
                            }
                            customerMatches.FirstOrDefault(x => x.CustomerID == customer.id).AccountNumber += customer.Account_no;
                        }
                    }

                }
            }

            //iterar los clientes y buscar su historial
            foreach (var customer in customerMatches)
            {
                List<SPIModels.TheVillaGroup.spCustomer_History_Result> History = spidb.spCustomer_History(customer.CustomerID).ToList();
                foreach (var tour in History.OrderByDescending(x => x.TourDate))
                {
                    //history
                    SPIViewModel.CustomerHistoryItem newTour = new SPIViewModel.CustomerHistoryItem();

                    if (tour.TourDate != null)
                    {
                        newTour.TourDateTime = tour.TourDate.Value;
                        newTour.TourDate = tour.TourDate.Value.ToString("yyyy-MM-dd");
                    }
                    newTour.SalesCenter = tour.SalesCenter;
                    newTour.TourSource = tour.Source;
                    newTour.SourceGroup = tour.SourceGroup;
                    newTour.SourceItem = tour.SourceItem;
                    newTour.Qualification = tour.Qualification;
                    newTour.TourContractNumber = tour.ContractNumber != null ? tour.ContractNumber.Trim() : "";
                    newTour.Volume = tour.Volume;

                    newTour.CustomerID = customer.CustomerID;
                    newTour.AccountNumber = customer.AccountNumber;
                    newTour.CustomerName = customer.Customer;
                    newTour.LastTour = false;
                    newTour.LastContract = false;
                    newTour.LegalNames = tour.LegalNames;

                    customerHistory.History.Add(newTour);
                }
            }


            //ordenar historial y obtener el ultimo tour y el último contrato
            customerHistory.History = customerHistory.History.OrderByDescending(x => x.TourDateTime).ToList();
            if (customerHistory.History.Count() > 0)
            {
                customerHistory.History.FirstOrDefault().LastTour = true;
                customerHistory.History.FirstOrDefault(x => x.Volume != null && x.Volume > 0).LastContract = true;

                //legal names
                var lastContract = customerHistory.History.FirstOrDefault(x => x.LastContract);
                if (lastContract != null && lastContract.LegalNames != null)
                {
                    string[] legalNames = lastContract.LegalNames.Split(',');
                    foreach (var n in legalNames)
                    {
                        string[] cLegalName = n.Split('|');

                        if (customerHistory.LegalNames.Count(x => x.Name == cLegalName[0]) == 0)
                        {
                            SPIViewModel.LegalName newLegal = new SPIViewModel.LegalName();
                            newLegal.Name = cLegalName[0];
                            newLegal.DateOfBirth = DateTime.Parse(cLegalName[1]).ToString("yyyy-MM-dd");
                            if (cLegalName[1] != null)
                            {
                                DateTime birthday = DateTime.Parse(cLegalName[1]);
                                DateTime today = DateTime.Today;
                                var age = today.Year - birthday.Year;
                                if (birthday > today.AddYears(-age)) age--;
                                newLegal.Age = age;
                            }
                            customerHistory.LegalNames.Add(newLegal);
                        }
                    }
                }
            }

            return customerHistory;
        }

        public static List<SelectListItem> GetDDLSearchCustomer(string firstname, string lastname)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SPIModels.TheVillaGroup.SpiTvgEntities spidb = new SPIModels.TheVillaGroup.SpiTvgEntities();

            if (lastname.Trim().IndexOf(" ") >= 0)
            {
                lastname = lastname.Substring(0, lastname.IndexOf(" "));
            }

            if (firstname.Trim().IndexOf(" ") >= 0)
            {
                firstname = firstname.Substring(0, firstname.IndexOf(" "));
            }

            List<SPIModels.TheVillaGroup.spCustomer_check_tours_Result> SearchResults = spidb.spCustomer_check_tours(lastname, firstname, null, null, null, null, null).ToList();

            var SearchSales = (from s in SearchResults
                               select new
                               {
                                   s.id,
                                   s.customer_name,
                                   s.customer_name2
                               }).Distinct();

            var totalMatch = from m in SearchSales
                             where m.customer_name.Contains(lastname + " ")
                             || m.customer_name2.Contains(lastname + " ")
                             select m;

            if (totalMatch.Count() == 1)
            {
                SelectListItem item = new SelectListItem();
                item.Value = totalMatch.FirstOrDefault().id.ToString();
                item.Text = totalMatch.FirstOrDefault().id.ToString() + " - " + totalMatch.FirstOrDefault().customer_name + (totalMatch.FirstOrDefault().customer_name2.Trim() != "" ? " & " + totalMatch.FirstOrDefault().customer_name2 : "");
                list.Add(item);
            }
            else if (totalMatch.Count() > 1)
            {
                foreach (var customer in totalMatch)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = customer.id.ToString();
                    item.Text = customer.id.ToString() + " - " + customer.customer_name + (customer.customer_name2.Trim() != "" ? " & " + customer.customer_name2 : "");
                    list.Add(item);
                }
            }
            else
            {
                foreach (var customer in SearchSales)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = customer.id.ToString();
                    item.Text = customer.id.ToString() + " - " + customer.customer_name + (customer.customer_name2.Trim() != "" ? " & " + customer.customer_name2 : "");
                    list.Add(item);
                }
            }

            if (list.Count() > 1)
            {
                list.Insert(0, new SelectListItem()
                {
                    Value = "",
                    Text = "Choose the best match"
                });
            }

            return list;
        }

        public static List<SPIViewModel.AgencyCustomer> GetManifestForAgency(DateTime date, long terminalID)
        {
            ePlatEntities db = new ePlatEntities();
            int regionID = 0;
            var terminalRegion = (from t in db.tblTerminals
                                  where t.terminalID == terminalID
                                  select t.spiRegionID).FirstOrDefault();

            if (terminalRegion != null)
            {
                regionID = (int)terminalRegion;
            }

            List<SPIViewModel.AgencyCustomer> Manifest = new List<SPIViewModel.AgencyCustomer>();

            SPIModels.TheVillaGroup.SpiTvgEntities spidb = new SPIModels.TheVillaGroup.SpiTvgEntities();

            Manifest = (from m in spidb.spManifest_Info(null, date, regionID)
                        select new SPIViewModel.AgencyCustomer()
                        {
                            DT_RowId = m.CustomerID.ToString(),//property for dataTables plugin use
                            CustomerID = m.CustomerID,//spicustomerid
                            FirstName = m.FirstName,
                            LastName = m.LastName,
                            Country = m.Nationality,
                            //tblpurchases
                            MarketingProgram = m.MarketingProgram,
                            Subdivision = m.Subdivision,
                            Source = m.Source,
                            OPCID = m.OPCID,
                            OPC = m.OPC,
                            TourID = m.TourID,
                            TourDate = date.ToString("yyyy-MM-dd"),
                            //
                            //tblleads solo si no tienen contenido
                            FrontOfficeGuestID = m.FrontOfficeIdCliente,
                            FrontOfficeResortID = m.FrontOfficeIdResort
                        }).ToList();

            return Manifest;
        }

        public static List<SPIViewModel.VLOCustomer> GetManifestForVLO(DateTime date, long terminalID, bool showEmail = true, bool showCancel = false)
        {
            List<SPIViewModel.VLOCustomer> Manifest = new List<SPIViewModel.VLOCustomer>();

            //obtener región
            ePlatEntities db = new ePlatEntities();
            var currentUserID = session.UserID;
            int regionID = 0;
            var terminalRegion = (from t in db.tblTerminals
                                  where t.terminalID == terminalID
                                  select t.spiRegionID).FirstOrDefault();

            if (terminalRegion != null)
            {
                regionID = (int)terminalRegion;
            }

            //obtener usuarios de terminal

            var UsersQ = from u in db.tblUsers_Terminals
                         join p in db.tblUserProfiles
                         on u.userID equals p.userID
                         into u_p
                         from p in u_p.DefaultIfEmpty()
                         where u.terminalID == terminalID
                         select new
                         {
                             u.userID,
                             p.firstName,
                             p.lastName,
                             p.spiUserID
                         };

            SPIModels.TheVillaGroup.SpiTvgEntities spidb = new SPIModels.TheVillaGroup.SpiTvgEntities();

            var ManifestQuery = spidb.spManifest_Info(null, date, regionID).ToList();
            var cultureGroups = db.tblFieldGroups.Where(m => m.tblEmailNotifications.terminalID == terminalID).Select(m => new { m.fieldGroupID, m.tblEmailNotifications.tblEmails.culture, m.description }).ToList();
            var groups = cultureGroups.Select(m => m.fieldGroupID).ToArray();
            //var fDate = DateTime.Today.AddDays(-5);
            //var fDate = date.AddDays(-15);
            var tDate = date.AddDays(20);
            var fieldValues = from fv in db.tblFieldsValues
                              join f in db.tblFields on fv.fieldID equals f.fieldID
                              where fv.terminalID == terminalID
                              && groups.Contains(f.fieldGroupID) 
                              && (fv.dateSaved >= date && fv.dateSaved <= tDate)
                              && (f.field.Contains("$Email") | f.field.Contains("$SentByUser") | f.field.Contains("$VLO") | f.field.Contains("$Sent") | f.field.Contains("$PD") | f.field.Contains("$Title") | f.field.Contains("$VPANumber") | f.field.Contains("$CollectDate") | f.field.Contains("$ActivationDate"))
                              select new
                              {
                                  f.fieldGroupID,
                                  f.field,
                                  fv.value,
                                  fv.dateSaved,
                                  fv.transactionID
                              };

            List<SelectListItem> VLOS = GetVLOS(null,true);
            List<SelectListItem> Letters = GetLetters(terminalID);

            if (showCancel)
            {
                foreach (var customer in ManifestQuery.OrderBy(m => m.PurchaseDate))
                {
                    if (customer.ContractStatus == "Cancel")
                    {
                        SPIViewModel.VLOCustomer newCustomer = new SPIViewModel.VLOCustomer();
                        newCustomer.CustomerID = customer.CustomerID;
                        newCustomer.VPANumber = customer.LegacyKey;
                        newCustomer.ContractStatus = customer.ContractStatus;
                        newCustomer.TourID = customer.TourID;
                        Manifest.Add(newCustomer);
                    }
                }
            }
            else
            {
                foreach (var customer in ManifestQuery.OrderBy(m => m.PurchaseDate))
                {
                    if (customer.ContractStatus == "Full" || customer.ContractStatus == "Procesable" || customer.ContractStatus == "Pending" || customer.ContractStatus == "LQD")
                    {
                        SPIViewModel.VLOCustomer newCustomer = new SPIViewModel.VLOCustomer();
                        var emails = new List<string>();
                        var emailStrings = new List<string>();
                        showEmail = customer.Email != null ? !Regex.IsMatch(customer.Email, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$") : true;

                        newCustomer.VLOS = VLOS;
                        newCustomer.Letters = Letters;
                        newCustomer.VPANumber = customer.LegacyKey ?? "";
                        newCustomer.Title = customer.Title ?? "";
                        newCustomer.FirstName = customer.FirstName ?? "";
                        newCustomer.LastName = customer.LastName ?? "";
                        if (customer.Email != null)
                        {
                            emails.Add((showEmail ? customer.Email : customer.Email.Substring(0, 4) + "********" + (customer.Email.IndexOf('.') != -1 ? customer.Email.Substring(customer.Email.LastIndexOf('.'), (customer.Email.Length - customer.Email.LastIndexOf('.'))) : "")));
                            emailStrings.Add(customer.Email);
                        }
                        newCustomer.ContractStatus = customer.ContractStatus ?? "";
                        newCustomer.VLOID = customer.VLOID != null ? (int)customer.VLOID : (int?)null;
                        newCustomer.CollectDate = null;
                        newCustomer.CustomerID = customer.CustomerID;
                        newCustomer.TourID = customer.TourID;
                        newCustomer.SendStatusString = "";
                        var vloUser = UsersQ.FirstOrDefault(x => x.spiUserID == customer.VLOID);
                        if (vloUser != null)
                        {
                            newCustomer.VLOUserID = vloUser.userID;
                            newCustomer.VLO = vloUser.firstName + " " + vloUser.lastName;
                        }
                        newCustomer.SalesCenter = customer.SalesCenter;
                        newCustomer.Culture = (customer.Nationality == "Mexico" ? "es-MX" : "en-US");
                        int groupID;
                        if (newCustomer.ContractStatus == "Full")
                        {
                            if (customer.PurchaseDate != null)
                            {
                                newCustomer.ActivationDate = customer.PurchaseDate.Value.AddDays(7).ToString("yyyy-MM-dd");
                            }
                            groupID = cultureGroups.FirstOrDefault(m => m.culture == newCustomer.Culture && m.description.Contains("Full")).fieldGroupID;
                        }
                        else
                        {
                            if (customer.PurchaseDate != null)
                            {
                                newCustomer.CollectDate = customer.PurchaseDate.Value.AddMonths(1).ToString("yyyy-MM-dd");
                                newCustomer.ActivationDate = customer.PurchaseDate.Value.AddDays(7).ToString("yyyy-MM-dd");
                            }
                            groupID = cultureGroups.FirstOrDefault(m => m.culture == newCustomer.Culture && m.description.Contains("Pender")).fieldGroupID;
                        }
                        newCustomer.Letter = groupID;

                        var vpa = newCustomer.VPANumber.Trim();
                        var transaction = fieldValues.Count(m => groups.Contains(m.fieldGroupID) && m.field == "$VPANumber" && vpa == m.value.Trim()) > 0 ? fieldValues.Where(m => groups.Contains(m.fieldGroupID) && m.field == "$VPANumber" && vpa == m.value.Trim()) : null;
                        if (transaction != null)
                        {
                            var transactions = transaction.Select(m => m.transactionID);
                            var values = fieldValues.Where(m => transactions.Contains(m.transactionID));
                            //newCustomer.EmailString = string.Join(",", transaction.Where(m => m.field == "$Email").Select(m => m.value));

                            //foreach (var i in values.Where(m => m.field == "$Email" || m.field == "$EmailAddress"))
                            emails = new List<string>();
                            emailStrings = new List<string>();
                            foreach (var i in values.Where(m => m.field == "$Email" || m.field == "$EmailAddress" && transactions.Contains(m.transactionID)))
                            {
                                if (emailStrings.Count(m => m == i.value) == 0)
                                {
                                    emails.Add(i.value.Substring(0, 4) + "********" + (i.value.IndexOf('.') != -1 ? i.value.Substring(i.value.LastIndexOf('.'), (i.value.Length - i.value.LastIndexOf('.'))) : ""));
                                    emailStrings.Add(i.value);
                                }
                            }

                            newCustomer.Letter = values.FirstOrDefault().fieldGroupID;
                            newCustomer.Title = values.FirstOrDefault(m => m.field == "$Title") != null ? values.FirstOrDefault(m => m.field == "$Title").value : "";
                            newCustomer.SendStatusString = values.FirstOrDefault(m => m.field == "$Sent").value;
                            newCustomer.VLOUserID = values.FirstOrDefault(x => x.field == "$VLO").value != null ? (Guid?)Guid.Parse(VLOS.FirstOrDefault(m => m.Text == values.FirstOrDefault(x => x.field == "$VLO").value).Value) : null;
                            newCustomer.VLO = values.FirstOrDefault(x => x.field == "$VLO").value != null ? VLOS.FirstOrDefault(m => m.Text == values.FirstOrDefault(x => x.field == "$VLO").value).Text : null;
                            newCustomer.PD = values.FirstOrDefault(m => m.field == "$PD").value;
                            newCustomer.CollectDate = values.Count(m => m.field == "$CollectDate") > 0 ? values.FirstOrDefault(m => m.field == "$CollectDate").value != null ? values.FirstOrDefault(m => m.field == "$CollectDate").value : newCustomer.CollectDate : newCustomer.CollectDate;
                            newCustomer.ActivationDate = values.Count(m => m.field == "$ActivationDate") > 0 ? values.FirstOrDefault(m => m.field == "$ActivationDate").value != null ? values.FirstOrDefault(m => m.field == "$ActivationDate").value : newCustomer.ActivationDate : newCustomer.ActivationDate;
                        }
                        newCustomer.Email = emails;
                        newCustomer.EmailString = emailStrings;
                        newCustomer.SendStatus = newCustomer.SendStatusString == "" ? (bool?)null : true;
                        if (newCustomer.VLOUserID == currentUserID || newCustomer.VLOUserID == null || GeneralFunctions.IsUserInRole("Administrator", currentUserID) || GeneralFunctions.IsUserInRole("VLO Supervisor", currentUserID))//condición para mostrar solo registros sin asignar y asignados a usuario logueado.
                        {
                            Manifest.Add(newCustomer);
                        }
                    }
                }
            }
            return Manifest;
        }

        public static List<SelectListItem> GetVLOS(string terminals = null, bool onlyActive = false)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            var terminalIds = terminals != null ? terminals.Split(',').Select(m => long.Parse(m)).ToArray() : new long[]{long.Parse(session.Terminals.Split(',').FirstOrDefault())};

            var UsersQ = from u in db.tblUsers_Terminals
                         join p in db.tblUserProfiles
                         on u.userID equals p.userID
                         into u_p
                         from p in u_p.DefaultIfEmpty()
                         //where u.terminalID == terminalID
                         where terminalIds.Contains(u.terminalID)
                         && (!onlyActive || (onlyActive && !p.aspnet_Users.aspnet_Membership.IsLockedOut))
                         select new
                         {
                             u.userID,
                             p.firstName,
                             p.lastName,
                             p.spiUserID
                         };

            foreach (var i in UsersQ)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.userID.ToString(),
                    Text = i.firstName + " " + i.lastName
                });
            }
            return list;
        }

        public static List<SelectListItem> GetLetters(long terminalID)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();

            var letters = from fg in db.tblFieldGroups
                          join en in db.tblEmailNotifications on fg.emailNotificationID equals en.emailNotificationID
                          where en.terminalID == terminalID
                          select new
                          {
                              fg.fieldGroupID,
                              fg.description
                          };
            foreach (var i in letters)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.fieldGroupID.ToString(),
                    Text = i.description
                });
            }

            return list;
        }

        public static List<SPIModels.TheVillaGroup.spEP_Hotel_Result> GetHotels()
        {
            List<SPIModels.TheVillaGroup.spEP_Hotel_Result> Hotels = new List<SPIModels.TheVillaGroup.spEP_Hotel_Result>();
            SPIModels.TheVillaGroup.SpiTvgEntities spidb = new SPIModels.TheVillaGroup.SpiTvgEntities();

            Hotels = spidb.spEP_Hotel().ToList();

            return Hotels;
        }

        /// <summary>
        /// corrige informacion de front usando el manifiesto
        /// el rango se define en el cuerpo del método
        /// </summary>
        /// <param name="terminalID"></param>
        public static string CorrectSPIData(long terminalID)
        {
            ePlatEntities db = new ePlatEntities();

            var results = "";
            //var iDate = DateTime.Parse("2018-04-25", CultureInfo.InvariantCulture);
            //while (iDate < DateTime.Parse("2018-04-29", CultureInfo.InvariantCulture))
            //{
            //    try
            //    {
            //        var fDate = iDate.AddDays(1).AddSeconds(-1);

            //        var manifest = GetManifestForAgency(iDate.Date, terminalID);
            //        var purchases = from lead in db.tblLeads
            //                        join p in db.tblPurchases on lead.leadID equals p.leadID
            //                        where lead.terminalID == terminalID
            //                        && lead.inputDateTime >= iDate && lead.inputDateTime <= fDate
            //                        && lead.spiCustomerID != null
            //                        select p;

            //        foreach (var i in purchases)
            //        {
            //            var row = manifest.Where(m => m.CustomerID == i.tblLeads.spiCustomerID);
            //            if (row.Count() == 1)
            //            {
            //                i.spiMarketingProgram = row.FirstOrDefault().MarketingProgram;
            //                i.spiSubdivision = row.FirstOrDefault().Subdivision;
            //                i.spiSource = row.FirstOrDefault().Source;
            //                i.spiOpcID = row.FirstOrDefault().OPCID;
            //                i.spiTourID = row.FirstOrDefault().TourID;
            //                i.spiTourDate = iDate.ToString("yyyy-MM-dd");
            //                var key = row.FirstOrDefault().OPCID != null ? row.FirstOrDefault().OPCID.ToString() : null;
            //                i.opcID = key != null ? db.tblOPCS.Count(m => m.legacyKey == key) > 0 ? db.tblOPCS.FirstOrDefault(m => m.legacyKey == key).opcID : (long?)null : (long?)null;
            //            }
            //        }
            //        db.SaveChanges();
            //        results += iDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ": " + purchases.Count().ToString() + " purchases successfully modified <br /> \n";
            //        iDate = iDate.AddDays(1);
            //    }
            //    catch (Exception ex)
            //    {
            //        results += iDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ": " + ex.InnerException.Message + " <br />";
            //    }
            //}
            return results;
        }
    }
}
