using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using ePlatBack.Models;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using ePlatBack.Models.Utils.Custom.Attributes;
using System.Threading.Tasks;
using System.Reflection;
using System.Web.Script.Serialization;


namespace ePlatBack.Controllers.CRM
{
    public class PreArrivalController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            //PreArrivalViewModel pvm = new PreArrivalViewModel();
            var privileges = AdminDataModel.GetViewPrivileges(11597);
            ViewData.Model = new PreArrivalViewModel
            {
                Privileges = privileges,
                PreArrivalSearchModel = new PreArrivalSearchModel(),
                //PreArrivalInfoModel = new PreArrivalInfoModel(),
                PreArrivalInfoModel = new PreArrivalInfoModel
                {
                    Privileges = privileges,
                    PreArrivalEmailsInfoModel = new PreArrivalEmailsInfoModel(),
                    PreArrivalPhonesInfoModel = new PreArrivalPhonesInfoModel(),
                    PreArrivalMemberInfoModel = new PreArrivalMemberInfoModel(),
                    PreArrivalBillingModel = new PreArrivalBillingModel { Privileges = privileges, },
                    PreArrivalInteractionsInfoModel = new PreArrivalInteractionsInfoModel { Privileges = privileges, },
                    PreArrivalReservationsModel = new PreArrivalReservationsModel
                    {
                        Privileges = privileges,
                        PreArrivalOptionsSoldModel = new PreArrivalOptionsSoldModel { Privileges = privileges, },
                        PreArrivalFlightsModel = new PreArrivalFlightsModel { Privileges = privileges, },
                        PreArrivalPresentationsModel = new PreArrivalPresentationsModel { Privileges = privileges, },
                        PreArrivalPaymentsModel = new PreArrivalPaymentsModel { Privileges = privileges, },
                    },
                    MassUpdateModel = new MassUpdate { Privileges = privileges, },
                    SearchToImportModel = new SearchToImportModel(),
                },
                //Privileges = privileges,
            };

            return View();
        }
        public UserSession session = new UserSession();

        public ActionResult ReassignOptions(string id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();

            return View("OptionsReassignment", pdm.ReassignOptions(id));
        }

        public ActionResult GroupLeads(string leads)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            GroupLeadsModel model = new GroupLeadsModel();
            model.Leads = leads;
            return View("GroupLeads", model);
        }

        public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null, string terminalID = null, string logReference = null)
        //public int CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null, string terminalID = null, string logReference = null)
        {
            int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request, terminalID, logReference);
            //int result = GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request, terminalID, logReference);
            return result;
        }

        //public PartialViewResult RenderPreArrival()
        //{
        //    var pvm = new PreArrivalInfoModel
        //    {
        //        //Privileges = AdminDataModel.GetViewPrivileges(11597),
        //        PreArrivalEmailsInfoModel = new PreArrivalEmailsInfoModel(),
        //        PreArrivalPhonesInfoModel = new PreArrivalPhonesInfoModel(),
        //        PreArrivalMemberInfoModel = new PreArrivalMemberInfoModel(),
        //        PreArrivalBillingModel = new PreArrivalBillingModel(),
        //        PreArrivalInteractionsInfoModel = new PreArrivalInteractionsInfoModel(),
        //        PreArrivalReservationsModel = new PreArrivalReservationsModel
        //        {
        //            PreArrivalOptionsSoldModel = new PreArrivalOptionsSoldModel(),
        //            PreArrivalFlightsModel = new PreArrivalFlightsModel(),
        //            PreArrivalPresentationsModel = new PreArrivalPresentationsModel(),
        //            PreArrivalPaymentsModel = new PreArrivalPaymentsModel(),
        //        },
        //        MassUpdateModel = new MassUpdate(),
        //        SearchToImportModel = new SearchToImportModel(),
        //    };

        //    return PartialView("_PreArrivalManagementPartial", pvm);
        //}

        public JsonResult GetDependantFields()
        {
            var result = new JsonResult
            {
                MaxJsonLength = Int32.MaxValue,
                Data = PreArrivalDataModel.PreArrivalCatalogs.GetDependantFields(),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
            //return Json(PreArrivalDataModel.PreArrivalCatalogs.GetDependantFields(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDependantResorts()
        {
            var result = new JsonResult
            {
                MaxJsonLength = Int32.MaxValue,
                Data = PreArrivalDataModel.PreArrivalCatalogs.GetDependantResorts(),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
            //return Json(PreArrivalDataModel.PreArrivalCatalogs.GetDependantFields(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRecentLeadGroups()
        {
            var result = new JsonResult
            {
                MaxJsonLength = Int32.MaxValue,
                Data = PreArrivalDataModel.GetRecentLeadGroups(),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }

        public JsonResult ReservationsToReceive(SearchToReassignModel model)
        {
            var result = new JsonResult
            {
                MaxJsonLength = Int32.MaxValue,
                Data = PreArrivalDataModel.ReservationsToReceive(model),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return result;

        }

        public JsonResult InvoicesToRefund(Guid reservationID)
        {

            return Json(PreArrivalDataModel.PreArrivalCatalogs.GetInvoicesToRefund(reservationID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistory(string referenceID)
        {
            return Json(PreArrivalDataModel.GetHistory(referenceID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ClickToCall(Guid leadID, int? phoneID)
        {
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = PreArrivalDataModel.ClickToCall(leadID, phoneID);
            var terminal = session.Terminals.Split(',').FirstOrDefault();

            //Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal));

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataFromRC(Guid leadID)
        {
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = PreArrivalDataModel.GetDataFromRC(leadID);
            var terminal = session.Terminals.Split(',').FirstOrDefault();

            //Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal));

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UnloadLead(Guid leadID)
        {
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = PreArrivalDataModel.UnloadLead(leadID);
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal, leadID.ToString()));
                }
                catch { }
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UnloadReservation(Guid reservationID)
        {
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = PreArrivalDataModel.UnloadReservation(reservationID);
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal, reservationID.ToString()));
                }
                catch { }
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //Interactions
        public JsonResult GetInteractions(Guid leadID)
        {
            InteractionDataModel idm = new InteractionDataModel();
            return Json(idm.GetInteractions(leadID));
        }

        public async Task<JsonResult> SaveInteraction(PreArrivalInteractionsInfoModel model)
        {
            InteractionDataModel idm = new InteractionDataModel();
            AttemptResponse attempt = idm.SaveInteraction(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminal, model.InteractionsInfo_LeadID.ToString()));
                }
                catch { }
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SaveInteractionReply(long interactionID, string reply)
        {
            InteractionDataModel idm = new InteractionDataModel();
            PreArrivalInteractionsInfoModel model = new PreArrivalInteractionsInfoModel();
            model = idm.GetInteraction(interactionID);
            model.InteractionsInfo_InteractionComments = reply;

            AttemptResponse attempt = idm.SaveInteraction(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetFields()
        {
            return Json(PreArrivalDataModel.PreArrivalCatalogs.GetFields(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillDrpReportLayoutsByUser()
        {
            return Json(ReportDataModel.ReportsCatalogs.FillDrpReportLayoutsByUser(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveLayout(PreArrivalSearchModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();

            AttemptResponse attempt = pdm.SaveLayout(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult CopyLayout(long reportLayoutID)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();

            AttemptResponse attempt = pdm.CopyLayout(reportLayoutID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult DeleteLayout(long reportLayoutID)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();

            AttemptResponse attempt = pdm.DeleteLayout(reportLayoutID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<ActionResult> SearchPreArrival(PreArrivalSearchModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            model.Search_Columns += ",reservationComments/Reservation Comments";
            //model.Search_Columns += ",interactionComments/Interaction Comments";
            ViewData["Columns"] = model.Search_Columns;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            //if (!request.IsLocal)
            //    Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminal));

            return PartialView("_PreArrivalSearchResultsPartial", pdm.SearchPreArrival(model));
        }

        public JsonResult GetTransactionsHistory(int id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            return Json(pdm.GetTransactionsHistory(id), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetLead(Guid id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            var response = pdm.GetLead(id);

            //Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal));

            return Json(response);
        }

        public async Task<JsonResult> GetEmail(Guid id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            var response = pdm.GetEmail(id);
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal, id.ToString()));
                }
                catch { }
            }

            return Json(response);
        }

        public async Task<JsonResult> GetPhone(Guid id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            var response = pdm.GetPhone(id);
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal, id.ToString()));
                }
                catch { }
            }

            return Json(response);
        }

        public async Task<JsonResult> GetBilling(long id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            var response = pdm.GetBilling(id);
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal, id.ToString()));
                }
                catch { }
            }

            return Json(response);
        }

        public async Task<JsonResult> SaveLead(PreArrivalInfoModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.SaveLead(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            //var properties = Models.Utils.Custom.Reflection.GetPropertiesWithoutAtribute<DoNotTrackChangesAttribute>(model);

            //var json = pdm.GetModelToTrack(properties, model, typeof(PreArrivalInfoModel));


            //var logRef = properties.Where(m => Attribute.IsDefined(m, typeof(LogReferenceAttribute))).FirstOrDefault();
            //var logRefValue = logRef != null ? logRef.GetValue(model, null).ToString() : null;

            //var list = new List<KeyValuePair<string,dynamic>>();
            //var properties = model.GetType().GetProperties().Where(m => !Models.Utils.Custom.Reflection.HasCustomAttribute<DoNotTrackChangesAttribute>(m) && m.GetValue(model, null) != null);
            //foreach(var i in properties)
            //{
            //    list.Add(new KeyValuePair<string, dynamic>(
            //        i.Name, 
            //        i.GetValue(model, null)
            //        ));
            //}
            if (!request.IsLocal)
            {
                try
                {
                    var lID = model.Info_LeadID != null ? model.Info_LeadID : (Guid?)null;
                    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminal, lID.ToString()));
                }
                catch { }
            }


            //Task.Run(() => CreateTableUserLogActivityAsync(json, "Prearrival", attempt, urlMethod, request, terminal, logRefValue));
            //CreateTableUserLogActivityAsync(json, "Prearrival", attempt, urlMethod, request, terminal, logRefValue);

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> SaveBilling(PreArrivalBillingModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.SaveBilling(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminal, model.BillingInfo_LeadID.ToString()));
                }
                catch { }
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetReservation(Guid id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = pdm.GetReservation(id);
            var terminal = session.Terminals.Split(',').FirstOrDefault();

            //Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal));

            return Json(response);
        }

        public async Task<JsonResult> GetOptionSold(long id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            var response = pdm.GetOptionSold(id);

            //Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal));

            return Json(response);
        }

        public async Task<JsonResult> DeleteOptionSold(long id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            var attempt = pdm.DeleteOptionSold(id);
            if (!request.IsLocal)
            {
                try
                {
                    var rID = attempt.ObjectID.ToString().IndexOf(",") != -1 ? attempt.Object.ToString().Split(',')[1] : attempt.ObjectID;
                    Task.Run(() => CreateTableUserLogActivityAsync(attempt, "Get", (string)null, urlMethod, request, terminal, rID.ToString()));
                }
                catch { }
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID.ToString().IndexOf(",") != -1 ? attempt.Object.ToString().Split(',')[0] : attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetFlight(long id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            var response = pdm.GetFlight(id);

            //Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal));

            return Json(response);
        }

        public async Task<JsonResult> DeleteFlight(long id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            var response = pdm.DeleteFlight(id);
            if (!request.IsLocal)
            {
                try
                {
                    var rID = response.ObjectID.ToString().IndexOf(",") != -1 ? response.ObjectID.ToString().Split(',')[1] : response.ObjectID;
                    Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal, rID.ToString()));
                }
                catch { }
            }
            response.ObjectID = response.ObjectID.ToString().IndexOf(",") != -1 ? response.ObjectID.ToString().Split(',')[0] : response.ObjectID;
            return Json(response);
        }

        public async Task<JsonResult> GetPayment(long id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            var response = pdm.GetPayment(id);

            //Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminal));

            return Json(response);
        }

        public async Task<JsonResult> SaveReservation(PreArrivalReservationsModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.SaveReservation(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminal, model.ReservationInfo_LeadID.ToString()));
                }
                catch { }
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> SavePresentation(PreArrivalPresentationsModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.SavePresentation(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminal, model.PresentationInfo_LeadID.ToString()));
                }
                catch { }
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> SaveOptionSold(PreArrivalOptionsSoldModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.SaveOptionSold(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminal, model.OptionInfo_ReservationID.ToString()));
                }
                catch { }
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> SaveFlight(PreArrivalFlightsModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.SaveFlight(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminal, model.FlightInfo_ReservationID.ToString()));
                }
                catch { }
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> SavePayment(PreArrivalPaymentsModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.SavePayment(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminal, model.PaymentInfo_ReservationID.ToString()));
                }
                catch { }
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> DeleteTransaction(int id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var attempt = pdm.DeleteTransaction(id);
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    var rID = attempt.ObjectID.ToString().IndexOf(",") != -1 ? attempt.ObjectID.ToString().Split(',')[1] : null;
                    Task.Run(() => CreateTableUserLogActivityAsync(attempt, "Get", (string)null, urlMethod, request, terminal, rID));
                }
                catch { }
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> SaveLeadGroup(GenericListItem model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var serializer = new JavaScriptSerializer();
            AttemptResponse attempt = pdm.SaveLeadGroup(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminal = session.Terminals.Split(',').FirstOrDefault();
            if (!request.IsLocal)
            {
                try
                {
                    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminal));
                }
                catch { }
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult ApplyCharge(long id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.ApplyCharge(id);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult ApplyPendingCharges(string id)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.ApplyPendingCharges(id);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        /// <summary>
        /// deprecated
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult _GetArrivals(SearchToImportModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var response = pdm.GetArrivals(model);

            return Json(response);
        }

        //public async Task<ActionResult> GetArrivalsToImport(SearchToImportModel model)

        public JsonResult GetArrivalsToImport(SearchToImportModel model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            model.ListResults = pdm.GetArrivalsToImport(model);

            try
            {
                var result = new JsonResult
                {
                    MaxJsonLength = Int32.MaxValue,
                    Data = model.ListResults,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                //var result = new ContentResult
                //{
                //    Content = serializer.Serialize(model.ListResults),
                //    ContentType = "application/json"
                //};
                return result;
                //return Json(model.ListResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("");
            }
            //return PartialView("_ArrivalsToImportResultsPartial", model);
        }

        public ActionResult RenderImport()
        {
            var model = new SearchToImportModel();
            //return PartialView("_ImportFromFrontPartial", model);
            return View("FrontOfficeImport", model);
        }

        public JsonResult ImportArrivals(string __data)
        {
            //AttemptResponse attempt = new PreArrivalDataModel().ImportArrivals(__data);
            AttemptResponse attempt = new PreArrivalDataModel().Import(__data);
            try
            {
                string errorLocation = "";
                if (attempt.Exception != null)
                {
                    errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
                }

                var result = new JsonResult
                {
                    MaxJsonLength = Int32.MaxValue,
                    Data = new
                    {
                        ResponseType = attempt.Type,
                        ItemID = attempt.ObjectID,
                        ResponseMessage = attempt.Message,
                        ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                        InnerException = Debugging.GetInnerException(attempt.Exception)
                    },
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                return result;
            }
            catch
            {
                return Json("");
            }
        }

        public JsonResult MassUpdate(MassUpdate model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.MassUpdate(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult MassInsert(MassUpdate model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            AttemptResponse attempt = pdm.MassInsert(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult MassSending(MassUpdate model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();

            AttemptResponse attempt = pdm.MassSending(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        [ValidateInput(false)]
        public JsonResult SendEmail(string model)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var context = HttpContext;
            AttemptResponse attempt = pdm.SendEmail(model, false, context);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public JsonResult PreviewEmail(Guid reservationID, int emailNotificationID, string transactionID)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            var context = HttpContext;
            //return Json(pdm.PreviewEmail("fieldGroup", reservationID, fgID), JsonRequestBehavior.AllowGet);
            //return Json(pdm.PreviewEmail("fieldGroup", reservationID, emailNotificationID), JsonRequestBehavior.AllowGet);
            return Json(pdm.PreviewEmail(reservationID, emailNotificationID, transactionID, context), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvailableLetters(Guid reservationID)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            return Json(pdm.GetAvailableLetters(reservationID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetManifestByDate(int place, string date)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            return Json(PreArrivalDataModel.PreArrivalCatalogs.GetManifestByDate(place, date), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTourInfo(int tourID)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            return Json(PreArrivalDataModel.PreArrivalCatalogs.GetTourInfo(tourID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPointsRedemptionRate(string id)
        {
            return Json(PreArrivalDataModel.PreArrivalCatalogs.GetPointsRedemptionRate(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResetAppVar()
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            return Json(pdm.ResetAppVar(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDDLData(string itemType)
        {
            PreArrivalDataModel pdm = new PreArrivalDataModel();
            return Json(pdm.GetDDLData(itemType), JsonRequestBehavior.AllowGet);
        }
    }
}
