using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ePlatBack.Models;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using System.Web.Script.Serialization;
using System.IO;

namespace ePlatBack.Models.DataModels
{
    public class WorkflowDataModel
    {
        UserSession session = new UserSession();

        public class WorkflowsCatalogs
        {
            
        }

        public List<WorkflowViewModel.WorkflowInfoModel> SearchWorkflows(WorkflowViewModel.WorkflowInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<WorkflowViewModel.WorkflowInfoModel>();
            var terminals = model.TerminalID != 0 ? new long[] { model.TerminalID } : session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            var query = from wf in db.tblWorkflows
                        where terminals.Contains(wf.terminalID)
                        && (model.Name == null || wf.workflowName.Contains(model.Name))
                        select wf;

            foreach (var i in query)
            {
                
                list.Add(new WorkflowViewModel.WorkflowInfoModel()
                {
                    System = i.system,
                    WorkflowID = i.workflowID,
                    TerminalID = i.terminalID,
                    Terminal = i.tblTerminals.terminal,
                    Name = i.workflowName,
                    WorkflowJson = new JavaScriptSerializer().Deserialize<WorkflowViewModel.WorkflowModel[]>(i.workflowJson)
                });
            }

            return list;
        }

        public AttemptResponse SaveWorkflow(WorkflowViewModel.WorkflowInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var serializer = new JavaScriptSerializer();

            //var model = serializer.Deserialize<List<WorkflowViewModel.WorkflowModel>>(workflow);//not used

            try
            {
                if (model.WorkflowID == null || model.WorkflowID == 0)
                {
                    var query = new tblWorkflows();

                    query.terminalID = model.TerminalID;
                    query.workflowName = model.Name;
                    query.difussionWay = int.Parse(model.DifussionWay);
                    query.difussionWayDetails = model.DifussionWay != "3" ? model.DifussionWayDetails : null;
                    query.sender = model.SenderAddress;
                    query.password = model.SenderPassword;
                    query.replyTo = model.ReplyTo;
                    query.workflowJson = new JavaScriptSerializer().Serialize(model.WorkflowJson);
                    db.tblWorkflows.AddObject(query);
                    db.SaveChanges();
                    response.Message = "Workflow Succesfully Saved";
                    response.ObjectID = query.workflowID;
                }
                else
                {
                    var query = db.tblWorkflows.Single(m => m.workflowID == model.WorkflowID);

                    query.terminalID = model.TerminalID;
                    query.workflowName = model.Name;
                    query.difussionWay = int.Parse(model.DifussionWay);
                    query.difussionWayDetails = model.DifussionWay != "3" ? model.DifussionWayDetails : null;
                    query.sender = model.SenderAddress;
                    query.password = model.SenderPassword;
                    query.replyTo = model.ReplyTo;
                    query.workflowJson = new JavaScriptSerializer().Serialize(model.WorkflowJson);
                    db.SaveChanges();
                    response.Message = "Workflow Succesfully Updated";
                    response.ObjectID = query.workflowID;
                }
                response.Type = Attempt_ResponseTypes.Ok;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Workflow NOT Saved/Updated";
                response.Exception = ex;
            }
            return response;
        }

        public List<WorkflowViewModel.WorkflowModel> GetWorkflow(int workflowID)
        {
            ePlatEntities db = new ePlatEntities();
            var model = new List<WorkflowViewModel.WorkflowModel>();
            var query = db.tblWorkflows.Single(m => m.workflowID == workflowID);

            model = new JavaScriptSerializer().Deserialize<List<WorkflowViewModel.WorkflowModel>>(query.workflowJson);

            return model;
        }

        public static PictureDataModel.FineUploaderResult UploadFile(PictureDataModel.FineUpload upload, WorkflowViewModel.WorkflowInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            //try
            //{
            if (upload.Exception == null)
            {
                response = new WorkflowDataModel().SaveWorkflow(model);

                if (response.Type == Attempt_ResponseTypes.Ok)
                {
                    var firstPath = HttpContext.Current.Server.MapPath("~/");
                    var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                    var finalPath = secondPath + "ePlatBack\\Content\\files\\workflows\\";
                    var fileName = model.WorkflowID + "-" + upload.Filename;

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
                    var _filePath = "/content/files/workflows/" + fileName;
                    upload.SaveAs(filePath, false);

                    var id = long.Parse(response.ObjectID.ToString());

                    var workflow = (from w in db.tblWorkflows
                                    where w.terminalID == model.TerminalID
                                    && w.workflowID == id
                                    select w).FirstOrDefault();

                    if (workflow.difussionWayDetails.IndexOf(finalPath) != -1)
                    {
                        var deletePath = HttpContext.Current.Server.MapPath("~" + workflow.difussionWayDetails);
                        var file = new FileInfo(deletePath);
                        file.Delete();
                    }

                    workflow.difussionWayDetails = _filePath;
                    db.SaveChanges();
                    return new PictureDataModel.FineUploaderResult(true, new { response = response }, new { path = _filePath });
                }
                throw new Exception();
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

        public static DependantFields GetWorkflowsDependentFields()
        {
            ePlatEntities db = new ePlatEntities();
            DependantFields df = new DependantFields();
            df.Fields = new List<DependantFields.DependantField>();

            //System
            DependantFields.DependantField System = new DependantFields.DependantField();
            System.Field = "System";
            System.ParentField = "Terminal";
            System.Values = new List<DependantFields.FieldValue>();

            DependantFields.FieldValue val1 = new DependantFields.FieldValue();
            val1.ParentValue = null;
            val1.Value = "ePlat";
            val1.Text = "ePlat";
            System.Values.Add(val1);

            DependantFields.FieldValue val2 = new DependantFields.FieldValue();
            val2.ParentValue = null;
            val2.Value = "ePlatform";
            val2.Text = "ePlatform";
            System.Values.Add(val2);

            df.Fields.Add(System);

            //User Terminals
            DependantFields.DependantField Terminals = new DependantFields.DependantField();
            Terminals.Field = "Terminals";
            Terminals.ParentField = "System";
            Terminals.Values = new List<DependantFields.FieldValue>();

            var terminals = TerminalDataModel.GetCurrentUserTerminals();
            var terminalsArr = terminals.Select(m => long.Parse(m.Value)).ToArray();
            foreach (var i in terminals)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = "ePlat";
                val.Value = i.Value;
                val.Text = i.Text;
                Terminals.Values.Add(val);
            }

            df.Fields.Add(Terminals);

            //Terminal Destinations
            DependantFields.DependantField Destinations = new DependantFields.DependantField();
            Destinations.Field = "Destinations";
            Destinations.ParentField = "Terminals";
            Destinations.Values = new List<DependantFields.FieldValue>();

            var destinations = from d in db.tblDestinations
                               join td in db.tblTerminals_Destinations on d.destinationID equals td.destinationID
                               where terminalsArr.Contains(td.terminalID)
                               select new
                               {
                                   td.terminalID,
                                   td.destinationID,
                                   d.destination
                               };

            foreach (var i in destinations)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = i.terminalID.ToString();
                val.Value = i.destinationID.ToString();
                val.Text = i.destination;
                Destinations.Values.Add(val);
            }

            df.Fields.Add(Destinations);

            //Hotels by terminal & destination
            DependantFields.DependantField Hotels = new DependantFields.DependantField();
            Hotels.Field = "Hotels";
            Hotels.GrandParentField = "Terminals";
            Hotels.ParentField = "Destinations";
            Hotels.Values = new List<DependantFields.FieldValue>();

            var hotels = from p in db.tblPlaces
                         join pt in db.tblPlaces_Terminals on p.placeID equals pt.placeID
                         where terminalsArr.Contains(pt.terminalID)
                         select new
                         {
                             pt.terminalID,
                             p.destinationID,
                             p.placeID,
                             p.place
                         };

            foreach (var i in hotels)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.GrandParentValue = i.terminalID.ToString();
                val.ParentValue = i.destinationID.ToString();
                val.Value = i.placeID.ToString();
                val.Text = i.place;
                Hotels.Values.Add(val);
            }

            df.Fields.Add(Hotels);

            return df;
        }

        public List<SelectListItem> GetDDL(SelectListItem model)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();

            switch (model.Text)
            {
                case "destinationsPerTerminal":
                    {
                        var id = long.Parse(model.Value);
                        var query = from d in db.tblDestinations
                                    join td in db.tblTerminals_Destinations on d.destinationID equals td.destinationID
                                    where td.terminalID == id
                                    select new
                                    {
                                        d.destinationID,
                                        d.destination
                                    };
                        break;
                    }
            }

            return list;
        }
    }
}
