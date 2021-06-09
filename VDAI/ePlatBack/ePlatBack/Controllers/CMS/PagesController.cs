using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;

namespace ePlatBack.Controllers.CMS
{
    public class PagesController : Controller
    {
        //
        // GET: /Pages/
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                PageInfoViewModel pim = new PageInfoViewModel();
                ViewData.Model = new PageInfoViewModel
                {
                    PageSearchModel = new PageSearchModel(),
                    PageInfoModel = new PageInfoModel(),
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Container(int id)
        {
            ViewData.Model = PageDataModel.GetPageContainer(id);
            return View();                   
        }

        public PartialViewResult RenderPages()
        {
            var pim = new PageInfoModel
            {
                PageDescriptionModel = new PageDescriptionModel(),
                SeoItemInfoModel = new SeoItemInfoModel(),
            };
            return PartialView("_PagesManagementPartial", pim);
        }

        public PartialViewResult ContentBlocks()
        {
            BlockDataModel block = new BlockDataModel();
            BlockInfoViewModel blockinfo = new BlockInfoViewModel();
            blockinfo.BlocksList = block.GetBlocksList();
            blockinfo.BlockItem = new BlockItem()
            {
                BlockItem_Terminals = PageDataModel.PageCatalogs.FillDrpTerminals(),
                BlockItem_Description = new BlockDescription()
                {
                    Cultures = PageDataModel.PageCatalogs.FillDrpCultures()
                }
            };
            return PartialView("_ContentBlocksPartial", blockinfo);
        }

        public JsonResult SearchPages(PageSearchModel model)
        {
            PageDataModel pdm = new PageDataModel();
            return Json(pdm.SearchPages(model));
        }

        public JsonResult GetTree(string terminals)
        {
            PageDataModel pdm = new PageDataModel();
            return Json(pdm.GetTree(terminals), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageInfo(int pageID)
        {
            PageDataModel pdm = new PageDataModel();
            return Json(pdm.GetPageInfo(pageID));
        }

        public JsonResult SavePage(PageInfoModel model)
        {
            PageDataModel pdm = new PageDataModel();
            AttemptResponse attempt = pdm.SavePageInfo(model);
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

        public JsonResult DeletePage(int pageID)
        {
            PageDataModel pdm = new PageDataModel();
            AttemptResponse attempt = pdm.DeletePage(pageID);
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

        public JsonResult GetPageDescriptions(int pageID)
        {
            PageDataModel pdm = new PageDataModel();
            return Json(pdm.GetPageDescriptions(pageID));
        }

        public JsonResult GetPageDescription(int pageDescriptionID)
        {
            PageDataModel pdm = new PageDataModel();
            return Json(pdm.GetPageDescription(pageDescriptionID));
        }

        public JsonResult SavePageDescription(PageDescriptionModel model)
        {
            PageDataModel pdm = new PageDataModel();
            AttemptResponse attempt = pdm.SavePageDescription(model);
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

        public JsonResult DeletePageDescription(int pageDescriptionID)
        {
            PageDataModel pdm = new PageDataModel();
            AttemptResponse attempt = pdm.DeletePageDescription(pageDescriptionID);
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

        public JsonResult GetDDLData(string itemType, string itemID)
        {
            PageDataModel pdm = new PageDataModel();
            return Json(pdm.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveBlock(BlockItem block)
        {
            BlockDataModel pdm = new BlockDataModel();
            AttemptResponse attempt = pdm.SaveBlockItem(block);
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

        public JsonResult DeleteBlock(long blockID)
        {
            BlockDataModel pdm = new BlockDataModel();
            AttemptResponse attempt = pdm.DeleteBlockItem(blockID);
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

        public JsonResult DeleteBlockDescription(long blockDescID)
        {
            BlockDataModel pdm = new BlockDataModel();
            AttemptResponse attempt = pdm.DeleteBlockDescription(blockDescID);
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

        public JsonResult GetBlock(long blockID)
        {
            BlockDataModel bdm = new BlockDataModel();
            return Json(bdm.GetBlockItem(blockID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveBlockDescription(BlockDescription desc)
        {
            BlockDataModel pdm = new BlockDataModel();
            AttemptResponse attempt = pdm.SaveBlockDescriptionItem(desc);
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

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            PageDataModel pdm = new PageDataModel();
            return Content(pdm.Upload(upload, CKEditorFuncNum, CKEditor, langCode));
        }

        public ActionResult Browse(string CKEditorFuncNum, string CKEditor, string langCode)
        {
            PageDataModel pdm = new PageDataModel();
            return View(pdm.Browse(CKEditorFuncNum, CKEditor, langCode));
        }
    }
}
