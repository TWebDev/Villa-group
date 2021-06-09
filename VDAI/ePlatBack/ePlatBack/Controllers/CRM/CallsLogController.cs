using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Net.Mime;
using System.Web.Security;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;

namespace ePlatBack.Controllers.CRM
{
    public class CallsLogController : Controller
    {
        [Authorize]
        public PictureDataModel.FineUploaderResult UploadCallsCSV(PictureDataModel.FineUpload upload)
        {
            return (NetCenterDataModel.CallsLog.UploadCSV(upload));
        }

        [HttpPost]
        public void CallStarted(NetCenterViewModel.CallLogEvent.CallStarted model)
        {
            NetCenterDataModel.CallsLog.SaveCallStarted(model);
        }

        [HttpPost]
        public void CallFinished(NetCenterViewModel.CallLogEvent.CallFinished model)
        {
            NetCenterDataModel.CallsLog.SaveCallFinished(model);
        }
    }
}
