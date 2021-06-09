using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using Microsoft.Web.Helpers;

namespace ePlatFront.Controllers
{
    public class ControlsController : Controller
    {
        //
        // GET: /Controls/
        [HttpPost]
        public JsonResult FreeVacationSave(FreeVacationViewModel fvmodel)
        {
            ControlsDataModel.FreeVacationDataModel freeVacation = new ControlsDataModel.FreeVacationDataModel();

            AttemptResponse attempt = freeVacation.Save(fvmodel);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public JsonResult GetPurchase(string cart)
        {
            string ip = Request.ServerVariables["REMOTE_ADDR"];
            string browser = Request.Browser.Browser + " - " + Request.Browser.Version;
            AttemptResponse attempt = PurchaseDataModel.GetFromClient(cart, ip, browser);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [HttpPost]
        public JsonResult FreeGetawaySave(FreeGetawayViewModel model)
        {
            ControlsDataModel.FreeGetawayDataModel freeGetaway = new ControlsDataModel.FreeGetawayDataModel();
            AttemptResponse attempt = new AttemptResponse();
            //if (ReCaptcha.Validate(privateKey: "6LdX2O4SAAAAAALciL2nqmXbkjNsY8YyujDLxLhq"))
            //{
                //attempt = freeGetaway.Save(model);
            //}
            //else
            //{
            //    attempt.Type = Attempt_ResponseTypes.Error;
            //    attempt.Message = "Please solve the ReCaptcha Challenge and Try again.";
            //}

            if (model.FreeGetaway_ValidationString == model.FreeGetaway_ValidationStringResponse)
            {
                attempt = freeGetaway.Save(model);
            }
            else
            {
                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Please type the validation characters again.";
            }

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [HttpPost]
        public JsonResult RedeemMyPackageSave(RedeemMyPackageViewModel model)
        {
            AttemptResponse attempt = new AttemptResponse();
            if (Models.ReCaptchaV2.Validate(secret: "6LfsSlEUAAAAADNNIzm8-hjArJafGCZm-Ck7JT0k", token: Request.Params["g-recaptcha-response"]))
            {
                attempt = ControlsDataModel.RedeemMyPackageDataModel.Save(model);
            }
            else
            {
                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Please solve the ReCaptcha Challenge and Try again.";
            }


            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [HttpPost]
        public JsonResult QuoteRequestSave(QuoteRequestViewModel qrmodel)
        {
            ControlsDataModel.QuoteRequestDataModel quoteRequest = new ControlsDataModel.QuoteRequestDataModel();
            AttemptResponse attempt = new AttemptResponse();
            if (Models.ReCaptchaV2.Validate(secret: "6LfsSlEUAAAAADNNIzm8-hjArJafGCZm-Ck7JT0k", token: Request.Params["g-recaptcha-response"]))
            {
                attempt = quoteRequest.Save(qrmodel);
            }
            else
            {
                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Please solve the ReCaptcha Challenge and Try again.";
            }          

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [HttpPost]
        public JsonResult UploadReviewPicture()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    string fname = "";
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];

                        // Checking for Internet Explorer  
                        /*if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }*/
                        fname = Guid.NewGuid().ToString() + ".jpg";

                        // Get the complete folder path and store the file inside it.  
                        fname = System.IO.Path.Combine(Server.MapPath("~/Content/themes/base/images/reviews/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json( new { 
                        ResponseType = 1,
                        ResponseMessage = "File Uploaded Successfully!",
                        FileName = fname.Substring(fname.LastIndexOf("\\") + 1)
                    });
                }
                catch (Exception ex)
                {
                    return Json( new { 
                        ResponseType = -1,
                        ResponseMessage = ex.Message
                    });
                }
            }
            else
            {
                return Json( new { 
                        ResponseType = -1,
                        ResponseMessage = "No Files Selected"
                    });
            }  
        }

        [HttpPost]
        public JsonResult PurchaseReviewSave(ActivityReviewsViewModel.ServiceReviewItem model)
        {
            ControlsDataModel.ReviewDataModel review = new ControlsDataModel.ReviewDataModel();
            AttemptResponse attempt = review.SetPurchaseServiceReview(model);
            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [HttpPost]
        public JsonResult BookingExperienceSave(ActivityReviewsViewModel.BookingExperience model)
        {
            ControlsDataModel.ReviewDataModel review = new ControlsDataModel.ReviewDataModel();
            AttemptResponse attempt = review.SetBookingExperienceReview(model);
            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [HttpPost]
        public JsonResult ReviewSave(ReviewListItem model)
        {
            ControlsDataModel.ReviewDataModel review = new ControlsDataModel.ReviewDataModel();
            AttemptResponse attempt = review.Save(model);
            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public ActionResult ApproveReview(long id)
        {
            ControlsDataModel.ReviewDataModel rdm = new ControlsDataModel.ReviewDataModel();
            PageViewModel page = rdm.ApproveReview(id);
            return View(page);
        }

        [HttpPost]
        public JsonResult GroupsFormSave(GroupsFormViewModel model)
        {
            ControlsDataModel.GroupsForm groupsForm = new ControlsDataModel.GroupsForm();
            AttemptResponse attempt = new AttemptResponse();
            if (Models.ReCaptchaV2.Validate(secret: "6LcrngoUAAAAAB652yYAoftrq1PQAdyjLj0KW3_r", token: Request.Params["g-recaptcha-response"]))
            {
                attempt = groupsForm.Save(model);
            }
            else
            {

                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Please solve the ReCaptcha Challenge and Try again.";
            }

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [HttpPost]
        public JsonResult ContactFormSave(ContactFormViewModel model)
        {
            ControlsDataModel.ContactForm contactForm = new ControlsDataModel.ContactForm();
            AttemptResponse attempt = new AttemptResponse();
            if (Models.ReCaptchaV2.Validate(secret: "6LcrngoUAAAAAB652yYAoftrq1PQAdyjLj0KW3_r", token: Request.Params["g-recaptcha-response"]))
            {
                attempt = contactForm.Save(model);
            } else
            {

                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Please solve the ReCaptcha Challenge and Try again.";
            }

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public JsonResult GetResortsByDestination(string id)
        {
            return Json(new
            {
                Resorts = ControlsDataModel.QuoteRequestDataModel.GetResortsListTextValue(id)
            },JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetResortsByTerminal(string id)
        {
            return Json(new
            {
                Resorts = ControlsDataModel.TransportationQuotes.GetResorts(id)
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPromo()
        {
            return Json(ControlsDataModel.SpecialPromoForCityDataModel.GetPromo(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBanners(string id)
        {
            return Json(TerminalDataModel.GettingBannerGroups(id), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult BookingForm()
        {
            QuoteRequestViewModel form = new QuoteRequestViewModel();
            return PartialView("_BookingFormPartial", form);
        }

        public PartialViewResult TransportationQuotes()
        {
            return PartialView("_TransportationQuotesPartial");
        }

    }
}
