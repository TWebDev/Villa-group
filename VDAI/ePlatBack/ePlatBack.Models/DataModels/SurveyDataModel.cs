using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Globalization;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Excel;
using System.IO;
using ClosedXML.Excel;

namespace ePlatBack.Models.DataModels
{
    public class SurveyDataModel
    {
        ePlatEntities db = new ePlatEntities();
        public static UserSession session = new UserSession();
        public class SurveysCatalogs
        {
            public static List<SelectListItem> FillDrpFieldSubTypes(int fieldTypeID)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                ePlatEntities db = new ePlatEntities();
                var subTypes = from s in db.tblFieldSubTypes
                               where s.fieldTypeID == fieldTypeID
                               orderby s.subType
                               select s;

                foreach (var s in subTypes)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = s.fieldSubTypeID.ToString(),
                        Text = s.subType
                    });
                }
                list.Insert(0, ListItems.Default("Select One", ""));

                return list;
            }

            public static List<SelectListItem> FillDrpVisibilityOptions(int fieldTypeID)
            {
                if (fieldTypeID == 1)
                {
                    return new List<SelectListItem>()
                        {
                            new SelectListItem(){
                                Text = "Visible",
                                Value = "1",
                                Selected = true
                            },
                            new SelectListItem(){
                                Text = "Visible If",
                                Value = "2"
                            },
                            new SelectListItem(){
                                Text = "Hidden",
                                Value = "0"
                            }
                       };
                }
                else
                {
                    return new List<SelectListItem>()
                    {
                        new SelectListItem(){
                            Text = "Visible",
                            Value = "1"
                        },
                        new SelectListItem(){
                            Text = "Hidden",
                            Value = "0"
                        }
                    };
                }
            }
        }

        public static AttemptResponse PublishField(Guid transactionID, Guid fieldGuid)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            var FieldQ = (from f in db.tblFieldsValues
                         where f.transactionID == transactionID
                         && f.tblFields.fieldGuid == fieldGuid
                         select f).FirstOrDefault();
            try
            {
                if (FieldQ != null)
                {
                    if (FieldQ.publish)
                    {
                        FieldQ.publish = false;
                    }
                    else
                    {
                        FieldQ.publish = true;
                    }
                    FieldQ.datePublished = DateTime.Now;
                    FieldQ.publishedByUserID = session.UserID;
                    db.SaveChanges();
                    response.ObjectID = FieldQ.publish;
                }
                else
                {
                    response.ObjectID = false;
                }

                response.Type = Attempt_ResponseTypes.Ok;                
            }
            catch (Exception e)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = e.ToString();
            }

            return response;
        }

        public static AttemptResponse SaveSurvey(string model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            SurveyViewModel.SurveyModel Survey = js.Deserialize<SurveyViewModel.SurveyModel>(model);

            tblFieldGroups newSurvey = new tblFieldGroups();

            if (Survey.FieldGroupID != 0)
            {
                //buscar para editar
                newSurvey = (from s in db.tblFieldGroups
                             where s.fieldGroupID == Survey.FieldGroupID
                             select s).FirstOrDefault();
            }

            newSurvey.fieldGroupID = Survey.FieldGroupID;
            newSurvey.fieldGroup = Survey.SurveyName;
            newSurvey.description = Survey.Instructions;
            newSurvey.logo = Survey.Logo;
            newSurvey.savedByUserID = session.UserID;
            newSurvey.dateSaved = DateTime.Now;

            if (Survey.FieldGroupID == 0)
            {
                //nuevo
                newSurvey.fieldGroupGUID = Guid.NewGuid();
                db.tblFieldGroups.AddObject(newSurvey);
            }
            db.SaveChanges();

            List<long> fieldids = new List<long>();

            foreach (var infoField in Survey.InfoFields)
            {
                tblFields newField = new tblFields();
                if (infoField.I_FieldID != 0)
                {
                    newField = (from f in db.tblFields
                                where f.fieldID == infoField.I_FieldID
                                select f).FirstOrDefault();
                }
                newField.field = infoField.I_FieldName;
                newField.fieldGuid = infoField.I_TemporalID;
                newField.fieldGroupID = newSurvey.fieldGroupID;
                newField.description = infoField.I_Question;
                newField.fieldTypeID = 2;
                newField.fieldSubTypeID = infoField.I_FieldSubTypeID;
                newField.visibility = infoField.I_Visibility;
                if (infoField.I_FieldID == 0)
                {
                    db.tblFields.AddObject(newField);
                }
                db.SaveChanges();
                fieldids.Add(newField.fieldID);
            }

            foreach (var formField in Survey.FormFields)
            {
                tblFields newField = new tblFields();
                if (formField.F_FieldID != 0)
                {
                    newField = (from f in db.tblFields
                                where f.fieldID == formField.F_FieldID
                                select f).FirstOrDefault();
                }
                newField.field = formField.F_FieldName;
                newField.fieldGuid = formField.F_TemporalID;
                newField.fieldGroupID = newSurvey.fieldGroupID;
                newField.description = formField.F_Question;
                newField.fieldTypeID = 1;
                newField.fieldSubTypeID = formField.F_FieldSubTypeID;
                newField.parentFieldID = null;
                newField.visibility = formField.F_Visibility;
                newField.visibleIfFieldGuid = formField.F_VisibleIf;
                newField.options = formField.F_Options;
                newField.order_ = formField.F_Order;
                if (formField.F_FieldID == 0)
                {
                    db.tblFields.AddObject(newField);
                }
                db.SaveChanges();
                fieldids.Add(newField.fieldID);

                foreach (var child in formField.F_Fields)
                {
                    tblFields newChild = new tblFields();
                    if (child.F_FieldID != 0)
                    {
                        newChild = (from c in db.tblFields
                                    where c.fieldID == child.F_FieldID
                                    select c).FirstOrDefault();
                    }
                    newChild.field = child.F_FieldName;
                    newChild.fieldGuid = child.F_TemporalID;
                    newChild.fieldGroupID = newSurvey.fieldGroupID;
                    newChild.description = child.F_Question;
                    newChild.fieldTypeID = 1;
                    newChild.fieldSubTypeID = child.F_FieldSubTypeID;
                    if (child.F_ParentFieldID != null && child.F_ParentFieldID != "")
                    {
                        newChild.parentFieldID = newField.fieldID;
                    };
                    newChild.visibility = child.F_Visibility;
                    newChild.visibleIfFieldGuid = child.F_VisibleIf;
                    newChild.options = child.F_Options;
                    newChild.order_ = child.F_Order;
                    if (child.F_FieldID == 0)
                    {
                        db.tblFields.AddObject(newChild);
                    }
                    db.SaveChanges();
                    fieldids.Add(newChild.fieldID);

                    foreach (var grandchild in child.F_Fields)
                    {
                        tblFields newGrandChild = new tblFields();
                        if (grandchild.F_FieldID != 0)
                        {
                            newGrandChild = (from c in db.tblFields
                                             where c.fieldID == grandchild.F_FieldID
                                             select c).FirstOrDefault();
                        }
                        newGrandChild.field = grandchild.F_FieldName;
                        newGrandChild.fieldGuid = grandchild.F_TemporalID;
                        newGrandChild.fieldGroupID = newSurvey.fieldGroupID;
                        newGrandChild.description = grandchild.F_Question;
                        newGrandChild.fieldTypeID = 1;
                        newGrandChild.fieldSubTypeID = grandchild.F_FieldSubTypeID;
                        if (grandchild.F_ParentFieldID != null && grandchild.F_ParentFieldID != "")
                        {
                            newGrandChild.parentFieldID = newChild.fieldID;
                        };
                        newGrandChild.visibility = grandchild.F_Visibility;
                        newGrandChild.visibleIfFieldGuid = grandchild.F_VisibleIf;
                        newGrandChild.options = grandchild.F_Options;
                        newGrandChild.order_ = grandchild.F_Order;
                        if (grandchild.F_FieldID == 0)
                        {
                            db.tblFields.AddObject(newGrandChild);
                        }
                        db.SaveChanges();
                        fieldids.Add(newGrandChild.fieldID);
                    }
                }

            }

            var DataFields = from t in db.tblFields
                             where t.fieldGroupID == newSurvey.fieldGroupID
                             && t.fieldTypeID == 3
                             select t.fieldID;

            if (DataFields.Count() == 0)
            {
                tblFields df1 = new tblFields();
                df1.fieldGuid = Guid.NewGuid();
                df1.field = "DB";
                df1.fieldGroupID = newSurvey.fieldGroupID;
                df1.description = "DB";
                df1.fieldTypeID = 3;
                df1.fieldSubTypeID = 14;
                df1.visibility = 0;
                df1.order_ = 0;
                db.tblFields.AddObject(df1);

                tblFields df3 = new tblFields();
                df3.fieldGuid = Guid.NewGuid();
                df3.field = "Sent";
                df3.fieldGroupID = newSurvey.fieldGroupID;
                df3.description = "Sent";
                df3.fieldTypeID = 3;
                df3.fieldSubTypeID = 15;
                df3.visibility = 0;
                df3.order_ = 1;
                db.tblFields.AddObject(df3);

                tblFields df4 = new tblFields();
                df4.fieldGuid = Guid.NewGuid();
                df4.field = "Open";
                df4.fieldGroupID = newSurvey.fieldGroupID;
                df4.description = "Open";
                df4.fieldTypeID = 3;
                df4.fieldSubTypeID = 15;
                df4.visibility = 0;
                df4.order_ = 2;
                db.tblFields.AddObject(df4);

                tblFields df5 = new tblFields();
                df5.fieldGuid = Guid.NewGuid();
                df5.field = "Submited";
                df5.fieldGroupID = newSurvey.fieldGroupID;
                df5.description = "Submited";
                df5.fieldTypeID = 3;
                df5.fieldSubTypeID = 15;
                df5.visibility = 0;
                df5.order_ = 3;
                db.tblFields.AddObject(df5);

                db.SaveChanges();
            }

            var DeleteFields = from d in db.tblFields
                               where d.fieldGroupID == newSurvey.fieldGroupID
                               && !fieldids.Contains(d.fieldID)
                               && d.fieldTypeID != 3
                               select d;

            foreach (var delete in DeleteFields)
            {
                db.tblFields.DeleteObject(delete);
            }
            db.SaveChanges();

            try
            {
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = newSurvey.fieldGroupID;
            }
            catch (Exception e)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = e.ToString();
            }

            return response;
        }

        public List<SurveyViewModel.SurveyItem> Search(SurveyViewModel.SearchSurveyModel model)
        {
            List<SurveyViewModel.SurveyItem> surveys = new List<SurveyViewModel.SurveyItem>();

            var SurveysQ = from s in db.tblFieldGroups
                           where (s.fieldGroup.Contains(model.Search_Name)
                           || model.Search_Name == null)
                           && s.fieldGroupGUID != null
                           select s;

            foreach (var survey in SurveysQ)
            {
                SurveyViewModel.SurveyItem item = new SurveyViewModel.SurveyItem();
                item.FieldGroupID = survey.fieldGroupID;
                item.FieldGroup = survey.fieldGroup;
                item.Url = "https://eplatfront.villagroup.com/survey/" + survey.fieldGroupGUID;
                var user = db.tblUserProfiles.Single(x => x.userID == survey.savedByUserID);
                item.CreatedBy = user.firstName + " " + user.lastName;
                item.CreatedOn = survey.dateSaved.Value.ToString("yyyy-MM-dd");
                surveys.Add(item);
            }

            return surveys;
        }

        public static List<SurveyViewModel.SurveyReferral> GetReferrals(string fromDate, string toDate, int fieldGroupID, string[] terminals = null)
        {
            ePlatEntities db = new ePlatEntities();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<SurveyViewModel.SurveyReferral> referrals = new List<SurveyViewModel.SurveyReferral>();

            DateTime fDate = DateTime.Parse(fromDate);
            DateTime tDate = DateTime.Parse(toDate).AddDays(1);

            if (fieldGroupID == 12)
            {
                var Referrals = from r in db.vw_NetCenterSurveyReferrals
                                where r.Date >= fDate
                                && r.Date < tDate
                                select r;

                if (terminals != null)
                {
                    Referrals = Referrals.Where(x => terminals.Contains(x.Terminal));
                }

                foreach (var guestReferrals in Referrals)
                {
                    List<SurveyViewModel.JSONReferral> items = js.Deserialize<List<SurveyViewModel.JSONReferral>>(guestReferrals.Referrals);

                    foreach (var item in items)
                    {
                        if (item.Name.Trim() != "")
                        {
                            SurveyViewModel.SurveyReferral r = new SurveyViewModel.SurveyReferral();
                            r.Referral = item.Name;
                            r.Email = item.Email;
                            r.Mobile = item.Mobile;
                            r.HomePhone = item.HomePhone;
                            r.ReferredBy = guestReferrals.Guest;
                            r.Resort = guestReferrals.Resort;
                            r.Stay = guestReferrals.Stay;
                            r.SavedOn = guestReferrals.Date.ToString("yyyy-MM-dd");
                            r.Terminal = guestReferrals.Terminal;
                            referrals.Add(r);
                        }
                    }
                }
            }            

            return referrals;
        }

        public static SurveyViewModel.SurveyModel GetSurvey(string id)
        {
            ePlatEntities db = new ePlatEntities();
            Guid guidid = new Guid(id);
            var intid = (from s in db.tblFieldGroups
                         where s.fieldGroupGUID == guidid
                         select s.fieldGroupID).FirstOrDefault();

            return GetSurvey(intid);
        }

        public static SurveyViewModel.SurveyModel GetSurvey(int id)
        {
            SurveyViewModel.SurveyModel m = new SurveyViewModel.SurveyModel();
            ePlatEntities db = new ePlatEntities();

            var Survey = (from s in db.tblFieldGroups
                          where s.fieldGroupID == id
                          select s).FirstOrDefault();

            if (Survey != null)
            {
                m.FieldGroupID = Survey.fieldGroupID;
                m.SurveyName = Survey.fieldGroup;
                m.SurveyGuid = (Guid)Survey.fieldGroupGUID;
                m.Instructions = Survey.description;
                m.Logo = Survey.logo;
                m.FormFields = new List<SurveyViewModel.SurveyFormField>();
                m.InfoFields = new List<SurveyViewModel.SurveyInfoField>();
                foreach (var field in Survey.tblFields.Where(x => x.fieldTypeID == 1 && x.parentFieldID == null).OrderBy(o => o.order_))
                {
                    SurveyViewModel.SurveyFormField ff = new SurveyViewModel.SurveyFormField();
                    ff.F_FieldID = field.fieldID;
                    ff.F_TemporalID = (Guid)field.fieldGuid;
                    ff.F_Question = field.description;
                    ff.F_FieldName = field.field;
                    ff.F_FieldSubTypeID = (int)field.fieldSubTypeID;
                    ff.F_Visibility = (int)field.visibility;
                    ff.F_VisibilityOptions = new List<SelectListItem>();
                    ff.F_VisibleIf = field.visibleIfFieldGuid;
                    ff.F_Options = field.options;
                    ff.F_Order = field.order_;
                    ff.F_Fields = new List<SurveyViewModel.SurveyFormField>();
                    //obtener hijos
                    foreach (var child in Survey.tblFields.Where(x => x.parentFieldID == field.fieldID).OrderBy(o => o.order_))
                    {
                        SurveyViewModel.SurveyFormField fc = new SurveyViewModel.SurveyFormField();
                        fc.F_FieldID = child.fieldID;
                        fc.F_TemporalID = (Guid)child.fieldGuid;
                        fc.F_Question = child.description;
                        fc.F_FieldName = child.field;
                        fc.F_FieldSubTypeID = (int)child.fieldSubTypeID;
                        fc.F_Visibility = (int)child.visibility;
                        fc.F_VisibleIf = child.visibleIfFieldGuid;
                        fc.F_Options = child.options;
                        fc.F_Order = child.order_;
                        fc.F_ParentFieldID = child.tblFields2.fieldGuid.ToString();
                        fc.F_Fields = new List<SurveyViewModel.SurveyFormField>();
                        //obtener nietos
                        foreach (var gchild in Survey.tblFields.Where(x => x.parentFieldID == child.fieldID).OrderBy(o => o.order_))
                        {
                            SurveyViewModel.SurveyFormField fg = new SurveyViewModel.SurveyFormField();
                            fg.F_FieldID = gchild.fieldID;
                            fg.F_TemporalID = (Guid)gchild.fieldGuid;
                            fg.F_Question = gchild.description;
                            fg.F_FieldName = gchild.field;
                            fg.F_FieldSubTypeID = (int)gchild.fieldSubTypeID;
                            fg.F_Visibility = (int)gchild.visibility;
                            fg.F_VisibleIf = gchild.visibleIfFieldGuid;
                            fg.F_Options = gchild.options;
                            fg.F_Order = gchild.order_;
                            fg.F_ParentFieldID = gchild.tblFields2.fieldGuid.ToString();
                            fg.F_Fields = new List<SurveyViewModel.SurveyFormField>();
                            fc.F_Fields.Add(fg);
                        }

                        ff.F_Fields.Add(fc);
                    }

                    m.FormFields.Add(ff);
                }
                foreach (var field in Survey.tblFields.Where(x => x.fieldTypeID == 2))
                {
                    SurveyViewModel.SurveyInfoField inf = new SurveyViewModel.SurveyInfoField();
                    inf.I_FieldID = field.fieldID;
                    inf.I_TemporalID = (Guid)field.fieldGuid;
                    inf.I_Visibility = (int)field.visibility;
                    inf.I_FieldName = field.field;
                    inf.I_FieldSubTypeID = (int)field.fieldSubTypeID;
                    inf.I_Question = field.description;
                    m.InfoFields.Add(inf);
                }
            }

            return m;
        }

        public static AttemptResponse SaveSurveyValues(string model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            SurveyViewModel.SurveyValuesModel Survey = js.Deserialize<SurveyViewModel.SurveyValuesModel>(model);

            //antes de guardar, revisar que haya registro del GUID que envía el cliente para validar que se trate de un survey genuino
            var SurveyForm = (from s in db.tblFieldGroups
                              where s.fieldGroupGUID == Survey.SurveyID
                              select s).FirstOrDefault();

            if (SurveyForm != null)
            {
                //si existe el formato de survey, buscar los infofields
                var SavedFields = from f in db.tblFieldsValues
                                  where f.transactionID == Survey.TransactionID
                                  select f;

                if (SavedFields.Count() > 0)
                {
                    //si existe registro
                    if (SavedFields.Count() <= SurveyForm.tblFields.Count(f => f.fieldTypeID == 2 || f.fieldTypeID == 3))
                    {
                        //aun no está guardado, guardar
                        foreach (var field in Survey.Fields)
                        {
                            tblFieldsValues newValue = new tblFieldsValues();
                            newValue.fieldID = field.FieldID;
                            newValue.value = field.Value;
                            newValue.terminalID = SavedFields.FirstOrDefault().terminalID;
                            newValue.dateSaved = DateTime.Now;
                            newValue.transactionID = Survey.TransactionID;
                            db.tblFieldsValues.AddObject(newValue);
                        }

                        long? SubmittedFieldID = getFieldIDByName(SurveyForm.fieldGroupID, "Submitted");
                        if (SubmittedFieldID != null)
                        {
                            tblFieldsValues newValue = new tblFieldsValues();
                            newValue.fieldID = (long)SubmittedFieldID;
                            newValue.value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                            newValue.terminalID = SavedFields.FirstOrDefault().terminalID;
                            newValue.dateSaved = DateTime.Now;
                            newValue.transactionID = Survey.TransactionID;
                            db.tblFieldsValues.AddObject(newValue);
                        }

                        try
                        {
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Thank you for your answers!";
                        }
                        catch (Exception e)
                        {
                            //error al guardar
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Message = e.ToString();
                        }
                    }
                    else
                    {
                        //ya está guardado
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "This Survey was already answered. Your answers can\'t be overwritten.";
                    }
                }
                else
                {
                    //no existe el registro
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Sorry. This is not a valid Survey. Your Survey ID does not exist.";
                }
            }
            else
            {
                //no existe el formato
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Survey Format does not exist";
            }

            return response;
        }

        public static long getFieldIDByName(int fieldgroupid, string fieldname)
        {
            ePlatEntities db = new ePlatEntities();
            var field = (from f in db.tblFields
                         where f.fieldGroupID == fieldgroupid
                         && f.field == fieldname
                         select f.fieldID).FirstOrDefault();

            return field;
        }

        public static SurveyViewModel.SurveyInfoModel GetTransactionID(string id, string rid)
        {
            SurveyViewModel.SurveyInfoModel model = new SurveyViewModel.SurveyInfoModel();
            model.InfoFields = new List<SurveyViewModel.SurveyInfoValue>();

            if (id.Length == 36)
            {
                Guid surveyid = new Guid(id);
                if (rid.Length == 36)
                {
                    ePlatEntities db = new ePlatEntities();
                    string reservationid = rid.ToString();
                    Guid transactionid = (from t in db.tblFieldsValues
                                          where t.value == reservationid
                                         select t.transactionID).FirstOrDefault();

                    if (transactionid != null)
                    {
                        model.TransactionID = transactionid;
                        //verificar si no ha sido contestado
                        var SurveyForm = (from s in db.tblFieldGroups
                                          where s.fieldGroupGUID == surveyid
                                          select new
                                          {
                                              s.tblFields,
                                              s.fieldGroupID
                                          }).FirstOrDefault();

                        var TransactionFields = from t in db.tblFieldsValues
                                                where t.transactionID == transactionid
                                                select t.terminalID;

                        if (TransactionFields.Count() > 0)
                        {
                            if (TransactionFields.Count() <= SurveyForm.tblFields.Count(f => f.fieldTypeID == 2 || f.fieldTypeID == 3))
                            {
                                //aun no ha sido contestado
                                model.Status = true;

                                //ha sido abierto?
                                long? OpenFieldID = getFieldIDByName(SurveyForm.fieldGroupID, "Open");
                                if (OpenFieldID != null)
                                {
                                    var OpenFieldQ = from o in db.tblFieldsValues
                                                     where o.fieldID == OpenFieldID
                                                     && o.transactionID == transactionid
                                                     select o.fieldValueID;

                                    if (OpenFieldQ.Count() == 0)
                                    {
                                        tblFieldsValues newValue = new tblFieldsValues();
                                        newValue.fieldID = (long)OpenFieldID;
                                        newValue.value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                                        newValue.terminalID = TransactionFields.FirstOrDefault();
                                        newValue.dateSaved = DateTime.Now;
                                        newValue.transactionID = transactionid;
                                        db.tblFieldsValues.AddObject(newValue);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                //ya fue contestado
                                model.Status = false;
                                model.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Survey_Answered;
                            }
                        }
                        else
                        {
                            //no fue creado por el sistema
                            model.Status = false;
                            model.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Survey_Not_Registered;
                        }
                    }
                    else
                    {
                        //invalid transactionid
                        model.Status = false;
                        model.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Survey_Not_Valid;
                    }
                }
                else
                {
                    //invalid transactionid
                    model.Status = false;
                    model.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Survey_Not_Valid;
                }
            }
            else
            {
                //invalid surveyid
                model.Status = false;
                model.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Survey_Not_Valid;
            }

            return model;
        }

        public static SurveyViewModel.SurveyInfoModel GetSurveyInfo(string id, string tid)
        {
            SurveyViewModel.SurveyInfoModel model = new SurveyViewModel.SurveyInfoModel();
            model.InfoFields = new List<SurveyViewModel.SurveyInfoValue>();

            if (id.Length == 36)
            {
                Guid surveyid = new Guid(id);
                if (tid.Length == 36)
                {
                    Guid transactionid = new Guid(tid);
                    ePlatEntities db = new ePlatEntities();
                    //verificar si no ha sido contestado
                    var SurveyForm = (from s in db.tblFieldGroups
                                      where s.fieldGroupGUID == surveyid
                                      select new
                                      {
                                          s.tblFields,
                                          s.fieldGroupID
                                      }).FirstOrDefault();

                    var TransactionFields = from t in db.tblFieldsValues
                                            where t.transactionID == transactionid
                                            select t.terminalID;

                    if (TransactionFields.Count() > 0)
                    {
                        if (TransactionFields.Count() <= SurveyForm.tblFields.Count(f => f.fieldTypeID == 2 || f.fieldTypeID == 3))
                        {
                            //aun no ha sido contestado
                            model.Status = true;

                            //ha sido abierto?
                            long? OpenFieldID = getFieldIDByName(SurveyForm.fieldGroupID, "Open");
                            if (OpenFieldID != null)
                            {
                                var OpenFieldQ = from o in db.tblFieldsValues
                                                 where o.fieldID == OpenFieldID
                                                 && o.transactionID == transactionid
                                                 select o.fieldValueID;

                                if (OpenFieldQ.Count() == 0)
                                {
                                    tblFieldsValues newValue = new tblFieldsValues();
                                    newValue.fieldID = (long)OpenFieldID;
                                    newValue.value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                                    newValue.terminalID = TransactionFields.FirstOrDefault();
                                    newValue.dateSaved = DateTime.Now;
                                    newValue.transactionID = transactionid;
                                    db.tblFieldsValues.AddObject(newValue);
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            //ya fue contestado
                            model.Status = false;
                            model.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Survey_Answered;
                        }
                    }
                    else
                    {
                        //no fue creado por el sistema
                        model.Status = false;
                        model.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Survey_Not_Registered;
                    }

                    //regresar info
                    var VisibleInfoFields = from v in db.tblFields
                                            where v.tblFieldGroups.fieldGroupGUID == surveyid
                                            && v.visibility == 1
                                            select v.fieldID;

                    var InfoFieldValues = from i in db.tblFieldsValues
                                          where i.transactionID == transactionid
                                          select i;

                    foreach (var inf in InfoFieldValues)
                    {
                        SurveyViewModel.SurveyInfoValue newInfoField = new SurveyViewModel.SurveyInfoValue();
                        newInfoField.FieldName = inf.tblFields.field;
                        newInfoField.Value = inf.value;
                        model.InfoFields.Add(newInfoField);
                    }
                }
                else
                {
                    //invalid transactionid
                    model.Status = false;
                    model.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Survey_Not_Valid;
                }
            }
            else
            {
                //invalid surveyid
                model.Status = false;
                model.Message = ePlatBack.Models.Resources.Models.Controls.SubmitReview.Survey_Not_Valid;
            }

            return model;
        }

        public static List<SurveyViewModel.StatsField> GetStatsParams(int id, string from, string to)
        {
            List<SurveyViewModel.StatsField> fields = new List<SurveyViewModel.StatsField>();

            try
            {
                DateTime fromDate = DateTime.Parse(from);
                DateTime toDate = DateTime.Parse(to).AddDays(1);
                ePlatEntities db = new ePlatEntities();

                var InfoFields = from f in db.tblFields
                                 where f.fieldGroupID == id
                                 && !f.field.Contains("ID")
                                 && f.visibility == 0
                                 && f.fieldSubTypeID == 11
                                 select f;

                foreach (var inf in InfoFields)
                {
                    SurveyViewModel.StatsField newField = new SurveyViewModel.StatsField();
                    newField.FieldName = inf.field.Replace("$","");
                    newField.Values = new List<SelectListItem>();
                    List<SelectListItem> valuesList = GetParamsValues(inf.fieldID, fromDate, toDate);

                    //filtro de terminales
                    if (newField.FieldName == "Terminal")
                    {
                        List<long> terminals = session.UserTerminals.Split(',').Select(m => long.Parse(m)).ToList();
                        var terminalNames = (from t in db.tblTerminals
                                            where terminals.Contains(t.terminalID)
                                            select t.terminal).ToList();
                        foreach (var item in valuesList)
                        {
                            if (terminalNames.Contains(item.Text))
                            {
                                newField.Values.Add(item);
                            }
                        }
                    }
                    else if (newField.FieldName == "Destination")
                    {
                        var destinationNames = (from t in db.tblUsers_Destinations
                                                where t.userID == session.UserID
                                                select t.tblDestinations.destination).ToList();

                        foreach (var item in valuesList)
                        {
                            if (destinationNames.Contains(item.Text))
                            {
                                newField.Values.Add(item);
                            }
                        }
                    }
                    else
                    {
                        newField.Values = valuesList;
                    }

                    //filtro de destinos


                    fields.Add(newField);                    
                }
            }
            catch (Exception e)
            {

            }

            return fields;
        }

        public static List<SelectListItem> GetParamsValues(long fieldid, DateTime fromDate, DateTime toDate)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> valuesList = new List<SelectListItem>();

            //valuesList.Add(new SelectListItem()
            //{
            //    Text = "Any",
            //    Value = ""
            //});

            var FieldValues = (from v in db.tblFieldsValues
                               where v.fieldID == fieldid
                               && v.dateSaved >= fromDate
                               && v.dateSaved < toDate
                               select v.value).Distinct();

            foreach (var value in FieldValues)
            {
                SelectListItem item = new SelectListItem();
                item.Text = value;
                item.Value = value;
                valuesList.Add(item);
            }

            return valuesList;
        }

        public static SurveyViewModel.SurveyStats GetStats(int id, string model)
        {
            ePlatEntities db = new ePlatEntities();

            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<SurveyViewModel.StatParamItem> modelParams = js.Deserialize<List<SurveyViewModel.StatParamItem>>(model);

            SurveyViewModel.SurveyStats stats = new SurveyViewModel.SurveyStats();
            List<Guid> TransactionIDS = new List<Guid>();
            DateTime fromDate = new DateTime();
            long fromFieldID = 0;
            DateTime toDate = new DateTime();
            long toFieldID = 0;
            int confirmer = 0;
            foreach (var f in modelParams)
            {
                if (f.FieldName == "FromDate")
                {
                    fromFieldID = getFieldIDByName(id, "Sent");
                    if (fromFieldID == 0)
                    {
                        fromFieldID = getFieldIDByName(id, "$Sent");
                    }
                     fromDate = DateTime.Parse(f.Value.ToString());
                     confirmer += 1;
                }
                else if (f.FieldName == "ToDate")
                {
                    toFieldID = getFieldIDByName(id, "Sent");
                    if (toFieldID == 0)
                    {
                        toFieldID = getFieldIDByName(id, "$Sent");
                    }
                     toDate = DateTime.Parse(f.Value.ToString()).AddDays(1);
                     confirmer += 1;
                }
                if (confirmer >= 2)
                {
                    break;
                }
            }

            TransactionIDS = (from s in db.tblFieldsValues
                                where s.tblFields.fieldGroupID == id
                                && s.fieldID == fromFieldID 
                                && s.dateSaved >= fromDate 
                                && s.dateSaved < toDate
                                select s.transactionID).Distinct().ToList();

            foreach (var f in modelParams)
            {
                if (f.FieldName != "FromDate" && f.FieldName != "ToDate")
                {
                    if (f.Value != null)
                    {
                        long fieldid = getFieldIDByName(id, f.FieldName);
                        if (fieldid == 0)
                        {
                            fieldid = getFieldIDByName(id, "$" + f.FieldName);
                        }
                        string[] vals = f.Value.ToString().Split(',');
                        TransactionIDS = (from s in db.tblFieldsValues
                                          where s.tblFields.fieldGroupID == id
                                          && TransactionIDS.Contains(s.transactionID)
                                          && s.fieldID == fieldid && vals.Contains(s.value)
                                          select s.transactionID).Distinct().ToList();
                    }
                }
            }

            stats.Surveys = new List<SurveyViewModel.SurveyListItem>();
            SurveyViewModel.SurveyModel objSurvey = GetSurvey(id);

            var SurveysCache = from s in db.tblSurveysCache
                               where TransactionIDS.Contains(s.transactionID)
                               select s;

            var AllFields = from t in db.tblFieldsValues
                            join field in db.tblFields on t.fieldID equals field.fieldID
                            into t_field
                            from field in t_field.DefaultIfEmpty()
                            where TransactionIDS.Contains(t.transactionID)
                            select new
                            {
                                t.transactionID,
                                field.fieldGuid,
                                field.fieldID,
                                field.fieldTypeID,
                                field.fieldSubTypeID,
                                field.field,
                                t.dateSaved,
                                t.publish,
                                t.value
                            };

            foreach (var tid in TransactionIDS)
            {
                SurveyViewModel.SurveyListItem sli = new SurveyViewModel.SurveyListItem();
                var TFields = AllFields.Where(x => x.transactionID == tid);

                if (SurveysCache.FirstOrDefault(x => x.transactionID == tid) != null && (SurveysCache.FirstOrDefault(x => x.transactionID == tid).submitted != null || (SurveysCache.FirstOrDefault(x => x.transactionID == tid).submitted == null && TFields.Count(x => x.field == "Submitted") == 0)))
                {
                    sli = js.Deserialize<SurveyViewModel.SurveyListItem>(SurveysCache.FirstOrDefault(x => x.transactionID == tid).json);
                }
                else
                {
                    sli.TransactionID = tid;
                    sli.Fields = new List<SurveyViewModel.SurveyFieldItem>();

                    foreach (var infoField in objSurvey.InfoFields)
                    {
                        SurveyViewModel.SurveyFieldItem newField = new SurveyViewModel.SurveyFieldItem();
                        var f = TFields.FirstOrDefault(x => x.fieldID == infoField.I_FieldID);
                        if (f != null)
                        {
                            newField.FieldGuid = (Guid)f.fieldGuid;
                            newField.TypeID = f.fieldTypeID;
                            newField.SubTypeID = f.fieldSubTypeID;
                            newField.FieldName = f.field;
                            newField.Value = f.value;
                            sli.Fields.Add(newField);
                        }
                    }

                    if (TFields.Count(x => x.field.IndexOf("Submitted") >= 0) > 0)
                    {
                        foreach (var f1 in objSurvey.FormFields)
                        {
                            SurveyViewModel.SurveyFieldItem nf1 = new SurveyViewModel.SurveyFieldItem();
                            var cf1 = TFields.FirstOrDefault(x => x.fieldID == f1.F_FieldID);
                            if (cf1 != null)
                            {
                                nf1.FieldGuid = (Guid)cf1.fieldGuid;
                                nf1.TypeID = cf1.fieldTypeID;
                                nf1.SubTypeID = cf1.fieldSubTypeID;
                                nf1.Publish = cf1.publish;
                                nf1.FieldName = cf1.field;
                                nf1.Value = cf1.value;
                                sli.Fields.Add(nf1);
                            }


                            foreach (var f2 in f1.F_Fields)
                            {
                                SurveyViewModel.SurveyFieldItem nf2 = new SurveyViewModel.SurveyFieldItem();
                                var cf2 = TFields.FirstOrDefault(x => x.fieldID == f2.F_FieldID);
                                if (cf2 != null)
                                {
                                    nf2.FieldGuid = (Guid)cf2.fieldGuid;
                                    nf2.TypeID = cf2.fieldTypeID;
                                    nf2.SubTypeID = cf2.fieldSubTypeID;
                                    nf2.Publish = cf2.publish;
                                    nf2.FieldName = cf2.field;
                                    nf2.Value = cf2.value;
                                    sli.Fields.Add(nf2);
                                }
                                foreach (var f3 in f2.F_Fields)
                                {
                                    SurveyViewModel.SurveyFieldItem nf3 = new SurveyViewModel.SurveyFieldItem();
                                    var cf3 = TFields.FirstOrDefault(x => x.fieldID == f3.F_FieldID);
                                    if (cf3 != null)
                                    {
                                        nf3.FieldGuid = (Guid)cf3.fieldGuid;
                                        nf3.TypeID = cf3.fieldTypeID;
                                        nf3.SubTypeID = cf3.fieldSubTypeID;
                                        nf3.Publish = cf3.publish;
                                        nf3.FieldName = cf3.field;
                                        nf3.Value = cf3.value;
                                        sli.Fields.Add(nf3);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var f in TFields.Where(x => x.fieldTypeID == 3))
                    {
                        SurveyViewModel.SurveyFieldItem newField = new SurveyViewModel.SurveyFieldItem();
                        newField.FieldGuid = (Guid)f.fieldGuid;
                        newField.TypeID = f.fieldTypeID;
                        newField.SubTypeID = f.fieldSubTypeID;
                        newField.FieldName = f.field;
                        newField.Value = f.value;
                        sli.Fields.Add(newField);
                    }

                    //guardar survey cache
                    tblSurveysCache cache = new tblSurveysCache();
                    if (SurveysCache.FirstOrDefault(x => x.transactionID == tid) != null)
                    {
                        //actualizar
                        cache = SurveysCache.FirstOrDefault(x => x.transactionID == tid);
                    }
                    cache.transactionID = tid;
                    cache.json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sli);
                    if (sli.Fields.Count(x => x.FieldName == "Submitted") > 0)
                    {
                        cache.submitted = sli.Fields.FirstOrDefault(x => x.FieldName == "Submitted").Value;
                    }
                    if (SurveysCache.FirstOrDefault(x => x.transactionID == tid) == null)
                    {
                        db.tblSurveysCache.AddObject(cache);
                    }                    
                }
                
                stats.Surveys.Add(sli);
            }
            db.SaveChanges();

            //stats summary

            stats.Sent = stats.Surveys.Count();
            stats.Open = stats.Surveys.Count(x => x.Fields.Count(y => y.FieldName == "Open") > 0);
            stats.Submitted = stats.Surveys.Count(x => x.Fields.Count(y => y.FieldName == "Submitted") > 0);
            stats.Surveys.Count(x => x.Fields.Count(y => y.FieldName == "Open") > 0);
            stats.ReferralsAmount = 0;

            return stats;
        }

        public static bool SendEmail(string to, string cc, string subject, string content, string survey)
        {
            string emailSender = "";
            try
            {
                string currentUser = session.Email;
                emailSender = currentUser;
                SmtpClient mail = new SmtpClient("smtp.villagroup.com");
                mail.Port = 25;
                mail.EnableSsl = false;
                mail.DeliveryMethod = SmtpDeliveryMethod.Network;
                mail.UseDefaultCredentials = false;
                 mail.Credentials = new NetworkCredential("ecommerce@villagroup.com", "Nv.56em11ec");
                //mail.Credentials = new NetworkCredential("eplat@villagroup.com", "Ep35#Nt29Ghk5");
                MailMessage message = new MailMessage();
                message.From = new MailAddress(currentUser);
                string pattern = @"([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})";
                string emailAddress = Regex.Matches(to, pattern)[0].ToString();
                message.To.Add(emailAddress);
                if (cc != "")
                {
                    message.CC.Add(cc);
                }
                message.Bcc.Add("gguerrap@villagroup.com");
                if (subject != "")
                {
                    message.Subject = subject;
                }

                message.Body = "<div style=\"font-family:verdana; font-size:12px;\">" + content + "</div>" + survey;
                message.IsBodyHtml = true;
                mail.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    ///send mail with exception
                    SmtpClient mail = new SmtpClient("smtp.villagroup.com");
                    mail.Port = 25;
                    mail.EnableSsl = false;
                    mail.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mail.UseDefaultCredentials = false;
                    mail.Credentials = new NetworkCredential("eplat@villagroup.com", "Ep35#Nt29Ghk5");
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("gguerrap@villagroup.com");
                    message.Subject = "Email Sending Failed On Surveys Report";
                    message.To.Add("gguerrap@villagroup.com");
                    string ex_message = (ex.Message != null ? ex.Message.ToString() : "");
                    string ex_exception = (ex.InnerException != null ? ex.InnerException.ToString() : "");
                    message.Body = "Message: " + ex_message + "<br /><br />Inner Exception: " + ex_exception
                        + "<br /><br /><br /><br /><br />Data Inserted in Send eMail Form:<br />"
                        + "<br />From: " + emailSender
                        + "<br />To: " + to
                        + "<br />CC: " + cc
                        + "<br />Subject: " + subject
                        + "<br />Content: " + content;
                    message.IsBodyHtml = true;
                    mail.Send(message);
                    return true;
                }catch(Exception exx)
                {
                    return false;
                    //la caja de pandora no abrir por favor
                }
            }
        }

        public static AttemptResponse SurveyFindArchive(string date)
        {
            ePlatEntities db = new ePlatEntities();          
            AttemptResponse response = new AttemptResponse();
            //string FolderPath = @"C:\Users\Richar\Source\Workspaces\Workspace\ePlat\ePlatBack\ePlatBack\Content\files\clarabridge";
            string FolderPath = HttpContext.Current.Server.MapPath("~/");
            FolderPath = FolderPath.Substring(0, FolderPath.LastIndexOf("ePlatBack"));
            FolderPath = FolderPath + "ePlatBack\\Content\\files\\clarabridge";
            var FilesArray = Directory.GetFiles(FolderPath);
            // var Error = 0;           

            var SendMailError = new SurveyViewModel.SurveyModelResponse();
            SendMailError.error = "";
            SendMailError.fileName = "";
            SendMailError.status = true;
            SendMailError.subject = "";
            SendMailError.list = new List<string>();

            string emailBody = "";

            var filesSaved =  from file in db.tblSurveySaved
                              where file.fileName.Contains(date)
                              select file;
            //try
            //{                    
                    var getFileNames = FilesArray.Select(x => x.Replace("\\", ",")).ToArray();
                    var OnlyNames = getFileNames.Select(x => x.Split(',').Last()).ToArray();      

                    Guid currentUser = Membership.GetUser() == null ? Guid.Parse("c53613b6-c8b8-400d-95c6-274e6e60a14a") : session.UserID;     
                    foreach (var fileName in OnlyNames.Where(x=> x.Contains(date)))
                    {                 
                            FileInfo fileInformation = new FileInfo(FolderPath + "\\" + fileName);
                            var fileSize = fileInformation.Length;
                            tblSurveySaved SurveySaved = new tblSurveySaved();
                            if (filesSaved.Count(x => x.fileName == fileName) == 0)
                            {
                                if (fileSize > 7000)
                                {
                                    //var saved = SaveSurveyExcel(fileName);
                                    var saved  = getDataSurvey(fileInformation);

                                    if (saved == "")
                                    {
                                        SurveySaved.fileName = fileName;
                                        SurveySaved.savedByUserID = currentUser; //Guid.Parse("7310ece1-bab5-4a69-a969-af6c44157c59");//currentUser != null ? currentUser.Value : TaskScheduler;
                                        SurveySaved.dateSaved = DateTime.Now;
                                        SurveySaved.status = true;
                                        db.tblSurveySaved.AddObject(SurveySaved);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        emailBody += fileName + "\n" + Environment.NewLine + saved;
                                    }
                                }
                                else
                                {
                                    SurveySaved.fileName = fileName;
                                    SurveySaved.savedByUserID = currentUser; //Guid.Parse("7310ece1-bab5-4a69-a969-af6c44157c59");//currentUser != null ? currentUser.Value : TaskScheduler;
                                    SurveySaved.dateSaved = DateTime.Now;
                                    SurveySaved.status = true;
                                    db.tblSurveySaved.AddObject(SurveySaved);
                                    db.SaveChanges();
                                }
                            }
                    }

                    if (emailBody != "")
                    {
                        System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                        email.From = new System.Net.Mail.MailAddress("rsalinas@villagroup.com");
                        email.Subject = "Surveys " + DateTime.Now.Date;
                        email.Body = emailBody;
                        email.To.Add("rsalinas@villagroup.com");
                        //EmailNotifications.Send(email);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                response.Message = "Surveys Saved ";
                        response.ObjectID = 1;
                        response.Type = Attempt_ResponseTypes.Ok;
                        return response;
                    }
                    else
                    {
                        response.Message = "Surveys Not Saved ";
                        response.ObjectID = 0;
                        response.Type = Attempt_ResponseTypes.Warning;
                        return response;
                    }

                  /*  if (SendMailError.status)
                    {
                        response.Message = "Surveys Saved ";
                        response.ObjectID = 1;
                        response.Type = Attempt_ResponseTypes.Ok;
                        return response;
                    }
                    else
                    {
                        response.Message = "Surveys Not Saved ";
                        response.ObjectID = 0;
                        response.Type = Attempt_ResponseTypes.Warning;
                       // SurveySendEx(SendMailError);
                        return response;
                    }*/
        
         }

        public static string getDataSurvey(FileInfo FileName)
        {
            ePlatEntities db = new ePlatEntities();
            Guid userID = Guid.Parse("7310ECE1-BAB5-4A69-A969-AF6C44157C59");

            /*SurveyViewModel.SurveyModelResponse response = new SurveyViewModel.SurveyModelResponse();
            response.error = "";
            response.fileName = "";
            response.status = true;
            response.subject = "";
            */

            string emailBody = "";
            var wb = new XLWorkbook(FileName.FullName);
            var ws = wb.Worksheet("Sheet1");
            var limiteFila = ws.LastRowUsed().RowNumber();
            var limiteColumnas = ws.LastColumnUsed().ColumnNumber();

            List<string> SurveyName = new List<string>();
            List<string> QuestionList = new List<string>();
            List<string> AppliedSurveyKey = new List<string>();   
            QuestionList.Add("");
            SurveyName.Add("");
            AppliedSurveyKey.Add("");
            int organizationIndex = 0;

               for (int col = 1; col <= 23; col++)
               {   
                  var ques = ws.Cell(2,col).Value.ToString().Trim();
                  if(ques == "Confirmation Number" || ques == "First Name" || ques == "Last Name"
                     || ques == "Loyalty ID" || ques == "Loyalty level" || ques == "Loyalty member Y/N" || ques == "Rate")
                     QuestionList.Add("");
                  else                            
                     QuestionList.Add(ques);// obtener los datos del guest

                   if(ques == "Organization")
                       organizationIndex = col;
               }                  
               for (int fila = 3; fila <= limiteFila; fila++)//obtener organization
               {
                  SurveyName.Add(ws.Cell(fila,organizationIndex).Value.ToString().Trim());//surveyName
                  AppliedSurveyKey.Add(ws.Cell(fila,1).Value.ToString().Trim()); //Key
               }

               var Appliedsurvey = from app in db.tblAppliedSurveys
                                   where/* app.submittedDate < currentDateBefore && app.submittedDate > currentDateAfter
                                       &&*/  AppliedSurveyKey.Contains(app.key_key)
                                   select new
                                   {
                                     Key = app.key_key,
                                     SurveyID = app.surveyID
                                   };                   
                   
                   //obtener las preguntas de las organizaciones guardadas
                var surTopicQues = (from sur in db.tblSurveys
                               join topic in db.tblTopicQuestions on sur.surveyID equals topic.surveyID
                               join Ques in db.tblQuestions on topic.topicQuestionID equals Ques.topicQuestionID                              
                               where SurveyName.Contains(sur.name)
                               select new SurveyViewModel.SurveysTopics()
                               {
                                   SurveyID = sur.surveyID,
                                   SurveyName = sur.name,
                                   TopicQuesID = topic.topicQuestionID,
                                   TopicQuesName = topic.name,
                                   QuestionID = Ques.questionID,
                                   Question = Ques.name
                               }).ToList();

                List<SurveyViewModel.SurveysTopics> newSurTopics = new List<SurveyViewModel.SurveysTopics>();   

             if (surTopicQues.Count() > 0) //si hay surveys reconocidos iterar
             {
                for (int fila = 3; fila <= limiteFila; fila++)//cada fila representa nuevo registro
                {
                    surTopicQues = newSurTopics.Count() > 0 ? surTopicQues.Concat(newSurTopics).ToList() : surTopicQues;
                    var surveyRow = ws.Cell(fila, organizationIndex).Value.ToString().Trim();
                    var surveyRowID = surTopicQues.FirstOrDefault(x => x.SurveyName == surveyRow);

                    if (surveyRowID != null)
                    {
                        var dbTopic = surTopicQues.Where(x => x.SurveyID == surveyRowID.SurveyID).Distinct().ToList();
                        //var dbQues = surTopicQues.Where(x => dbTopic.Count(y => y)).Distinct().ToList();
                        int keyIndex = int.Parse(QuestionList.IndexOf("Key").ToString());
                        string key = ws.Cell(fila, QuestionList.IndexOf("Key")).Value.ToString().Trim();
                        if (surTopicQues.Count(x => x.SurveyName == surveyRow) > 0) //ver si esta agregado el survey de lo contrario no iterar en las preguntas o registros
                        {
                             var appSurID = 0;
                             if (Appliedsurvey.Count(x => x.Key == key) == 0)// verificar si ya esta guardado el key
                             {   
                                 tblAppliedSurveys AppliedSurvey = new tblAppliedSurveys();
                                 AppliedSurvey.key_key = key;
                                 AppliedSurvey.dateIn = DateTime.Parse(ws.Cell(fila, QuestionList.IndexOf("Date In")).Value.ToString().Trim());
                                 AppliedSurvey.dateOut = DateTime.Parse(ws.Cell(fila, QuestionList.IndexOf("Date Out")).Value.ToString().Trim());
                                 AppliedSurvey.email = ws.Cell(fila, QuestionList.IndexOf("Email Address")).Value.ToString().Trim();
                                 AppliedSurvey.organizationID =ws.Cell(fila, QuestionList.IndexOf("Full Organization Name")).Value.ToString().Trim();
                                 AppliedSurvey.hadProblem = bool.Parse(ws.Cell(fila, QuestionList.IndexOf("Had Problem")).Value.ToString().Trim());
                                 AppliedSurvey.hadComment = bool.Parse(ws.Cell(fila, QuestionList.IndexOf("Had Comment")).Value.ToString().Trim());
                                 AppliedSurvey.marketCodeID = FileName.FullName.Contains("Tafer Resorts") ? ws.Cell(fila, QuestionList.IndexOf("Market segment")).Value.ToString().Trim() :
                                                              ws.Cell(fila, QuestionList.IndexOf("Marketing code")).Value.ToString().Trim();
                                 AppliedSurvey.rate = ws.Cell(fila, QuestionList.IndexOf("Survey Score")).Value != null ? ws.Cell(fila, QuestionList.IndexOf("Survey Score")).Value.ToString().Trim() != "" ? 
                                                      double.Parse(ws.Cell(fila, QuestionList.IndexOf("Survey Score")).Value.ToString().Trim()) : 0 : 0;
                                 AppliedSurvey.Room = ws.Cell(fila, QuestionList.IndexOf("Room")).Value.ToString().Trim();
                                 AppliedSurvey.sentDate = ws.Cell(fila, QuestionList.IndexOf("Sent Date")).Value.ToString() != "" ? ws.Cell(fila, QuestionList.IndexOf("Sent Date")).Value != null ?
                                               DateTime.Parse(ws.Cell(fila, QuestionList.IndexOf("Sent Date")).Value.ToString().Trim()) : (DateTime?)null : (DateTime?)null;
                                 AppliedSurvey.submittedDate = ws.Cell(fila, QuestionList.IndexOf("Submitted Date")).Value.ToString() != "" ? ws.Cell(fila, QuestionList.IndexOf("Submitted Date")).Value != null ?
                                               DateTime.Parse(ws.Cell(fila, QuestionList.IndexOf("Submitted Date")).Value.ToString().Trim()) : (DateTime?)null : (DateTime?)null;
                                 AppliedSurvey.surveyLenguage = ws.Cell(fila, QuestionList.IndexOf("Survey Language")).Value.ToString().Trim();
                                 AppliedSurvey.propertyCode = int.Parse(ws.Cell(fila, QuestionList.IndexOf("Property Code")).Value.ToString().Trim());
                                 AppliedSurvey.surveyID = surveyRowID.SurveyID;
                                 db.tblAppliedSurveys.AddObject(AppliedSurvey);
                                 db.SaveChanges();

                                 appSurID = AppliedSurvey.surveyAppliedID;

                                 for (int col = 1; col <= limiteColumnas; col++)//iterar en preguntas 
                                 {
                                     dbTopic = newSurTopics.Count() > 0 ? dbTopic.Where(x => x.SurveyID == surveyRowID.SurveyID).Concat(newSurTopics.Where(x => x.SurveyID == surveyRowID.SurveyID).Distinct()).ToList() : dbTopic;
                                     string answer = ws.Cell(fila, col).Value != null ? ws.Cell(fila, col).Value.ToString().Trim() != "" ? ws.Cell(fila, col).Value.ToString().Trim() : null : null;// respuesta
                                     string topicQuesExcel = ws.Cell(2, col).Value.ToString().Trim();
                                     if (col != QuestionList.IndexOf(topicQuesExcel) && answer != null)
                                     {
                                          string topicExcel = topicQuesExcel.Contains(':') ? topicQuesExcel.Split(':').First().Trim() : "Without Topic";
                                          var questions = dbTopic.Where(x => x.TopicQuesName == topicExcel).Distinct().ToList();
                                          if (questions.Count() > 0)
                                          {
                                              string quesExcel = topicQuesExcel.Split(':').Last().Trim();
                                              if (questions.Count(x => x.Question == quesExcel) > 0)
                                              {
                                                /*  if(questions.Count(x => x.Question == quesExcel) > 1)
                                                  {

                                                  }*/

                                                  var question = questions.Single(x => x.Question == quesExcel);
                                                  if (answer != null || answer != "") //si la celda de la respuesta contiene informacion
                                                  {
                                                      tblAnswers record = new tblAnswers();
                                                      record.answer = answer;
                                                      record.questionID = question.QuestionID;
                                                      record.surveyAppliedID = appSurID;
                                                      db.tblAnswers.AddObject(record);
                                                      db.SaveChanges();
                                                  }
                                              }
                                              else//no hay pregunta relacionada con el topic agregar pregunta al topic => agregar resp .- envíar correo 
                                              {
                                                  tblQuestions question = new tblQuestions();
                                                  question.name = quesExcel;
                                                  question.topicQuestionID = questions.FirstOrDefault().TopicQuesID;
                                                  question.userID = userID;
                                                  question.dateSaved = DateTime.Now.Date;
                                                  db.tblQuestions.AddObject(question);
                                                  db.SaveChanges();

                                                  tblAnswers record = new tblAnswers();
                                                  record.answer = answer;
                                                  record.questionID = question.questionID;
                                                  record.surveyAppliedID = appSurID;
                                                  db.tblAnswers.AddObject(record);
                                                  db.SaveChanges();

                                                  //agregar registro de la pregunta a la lista de topics del survey en curso
                                                  newSurTopics.Add(
                                                  new SurveyViewModel.SurveysTopics()
                                                  {
                                                      SurveyID = surveyRowID.SurveyID,
                                                      SurveyName = surveyRow,
                                                      TopicQuesID = question.topicQuestionID,
                                                      TopicQuesName = topicExcel,
                                                      QuestionID = question.questionID,
                                                      Question = question.name
                                                  });
                                              }
                                          }
                                          else //no existe topic en el survey => agregar topic => agregar pregunta al topic => agregar respuesta con el id de la pregunta
                                          {
                                              tblTopicQuestions topic = new tblTopicQuestions();
                                              topic.name = topicExcel;
                                              topic.userID = userID;
                                              topic.dateSaved = DateTime.Now.Date;
                                              topic.surveyID = surveyRowID.SurveyID;
                                              db.tblTopicQuestions.AddObject(topic);
                                              db.SaveChanges();

                                              tblQuestions question = new tblQuestions();
                                              question.name = topicQuesExcel.Split(':').Last().Trim();
                                              question.topicQuestionID = topic.topicQuestionID;
                                              question.userID = userID;
                                              question.dateSaved = DateTime.Now.Date;
                                              db.tblQuestions.AddObject(question);
                                              db.SaveChanges();

                                              tblAnswers record = new tblAnswers();
                                              record.answer = answer;
                                              record.questionID = question.questionID;
                                              record.surveyAppliedID = appSurID;
                                              db.tblAnswers.AddObject(record);
                                              db.SaveChanges();

                                              newSurTopics.Add(
                                              new SurveyViewModel.SurveysTopics()
                                              {
                                                  SurveyID = surveyRowID.SurveyID,
                                                  SurveyName = surveyRow,
                                                  TopicQuesID = topic.topicQuestionID,
                                                  TopicQuesName = topic.name,
                                                  QuestionID = question.questionID,
                                                  Question = question.name
                                              });
                                          }
                                     }
                                 }
                             }
                             else//else ya existe registro en la db
                             {
                                 //survey ya existe
                                 emailBody += "Survey ya esta regsitrado Key = " + key + " SurveyID = " + surveyRowID.SurveyID + " Survey = " + surveyRow + "\n" + Environment.NewLine;
                             }
                        }
                    }
                    else
                    {
                        emailBody += "El survey " + surveyRow + " no esta registrado, " + " " + FileName.FullName + "\n" + Environment.NewLine;
                    }
                }
             }

           /*  if (emailBody != "")
             {
                 System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                 email.From = new System.Net.Mail.MailAddress("rsalinas@villagroup.com");
                 email.Subject = "Surveys " + DateTime.Now.Date;
                 email.Body = emailBody;
                 email.To.Add("rsalinas@villagroup.com");
                 EmailNotifications.Send(email);
                // response.status = false;
             }*/

            return emailBody;
        }

        public static string SaveSurveyExcel(string FileName)
        {
            ePlatEntities db = new ePlatEntities();
            Guid userID = Guid.Parse("7310ECE1-BAB5-4A69-A969-AF6C44157C59");
            string emailBody = "";
            //get Excel file
            string FolderPath = HttpContext.Current.Server.MapPath("~/");
            FolderPath = FolderPath.Substring(0, FolderPath.LastIndexOf("ePlatBack"));
            FolderPath = FolderPath + "ePlatBack\\Content\\files\\clarabridge";                       
            var file = FolderPath +"\\"+ FileName;
            object[,] ModelInfo = new object[0,0];
            SurveyViewModel.SurveyModelResponse SendErrorByEmail = new SurveyViewModel.SurveyModelResponse();
            SendErrorByEmail.error = "";
            SendErrorByEmail.fileName = "";
            SendErrorByEmail.status = true;
            SendErrorByEmail.subject = "";
            SendErrorByEmail.surveyStatus = new List<bool>();
            SendErrorByEmail.list = new List<string>();
            //open archive
            Application Excel = new Application();
            Workbook wkb = Excel.Workbooks.Open(file,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing);
            ModelInfo = ExcelScanIntenal(wkb);
            wkb.Close(false, FileName, null);

                   var limiteFilas =  ModelInfo.GetLength(0);
                   var limiteColumnas = ModelInfo.GetLength(1);   
                   List<string> SurveyName = new List<string>();
                   List<string> QuestionList = new List<string>();
                   List<string> AppliedSurveyKey = new List<string>();               
                   //get all organizations
                   QuestionList.Add("");
                   SurveyName.Add("");
                   AppliedSurveyKey.Add("");
                   for (int col = 1; col <= 23; col++)
                   {   
                       var ques = ModelInfo[2, col].ToString().Trim();
                       if(ques == "Confirmation Number" || ques == "First Name" || ques == "Last Name"
                           || ques == "Loyalty ID" || ques == "Loyalty level" || ques == "Loyalty member Y/N" || ques == "Rate")
                           QuestionList.Add("");
                       else                            
                           QuestionList.Add(ques);// obtener los datos del guest
                   }   
                   var Organizationindex = QuestionList.IndexOf("Organization"); 
                   //crear lista de surveys                  
                   for (int fila = 3; fila <= limiteFilas; fila++)//obtener organization
                   {
                       SurveyName.Add(ModelInfo[fila, Organizationindex].ToString().TrimStart('"').TrimEnd('"').Trim());//surveyName
                       AppliedSurveyKey.Add(ModelInfo[fila, 1].ToString().TrimStart('"').TrimEnd('"').Trim()); //Key
                   }

                   var Appliedsurvey = from app in db.tblAppliedSurveys
                                       where/* app.submittedDate < currentDateBefore && app.submittedDate > currentDateAfter
                                            &&*/  AppliedSurveyKey.Contains(app.key_key)
                                       select new
                                       {
                                           Key = app.key_key,
                                           SurveyID = app.surveyID
                                       };                   
                   
                   //obtener las preguntas de las organizaciones guardadas
                   var surTopicQues = (from sur in db.tblSurveys
                               join topic in db.tblTopicQuestions on sur.surveyID equals topic.surveyID
                               join Ques in db.tblQuestions on topic.topicQuestionID equals Ques.topicQuestionID                              
                               where SurveyName.Contains(sur.name)
                                      select new SurveyViewModel.SurveysTopics()
                               {
                                   SurveyID = sur.surveyID,
                                   SurveyName = sur.name,
                                   TopicQuesID = topic.topicQuestionID,
                                   TopicQuesName = topic.name,
                                   QuestionID = Ques.questionID,
                                   Question = Ques.name
                               }).ToList();

                   List<SurveyViewModel.SurveysTopics> newSurTopics = new List<SurveyViewModel.SurveysTopics>();

                     try
                      {
                       if (surTopicQues.Count() > 0) //si hay surveys reconocidos iterar
                       {
                           for (int fila = 3; fila <= limiteFilas; fila++)//cada fila representa nuevo registro
                           {
                               surTopicQues = newSurTopics.Count() > 0 ? surTopicQues.Concat(newSurTopics).ToList() : surTopicQues;

                               var surveyRow = ModelInfo[fila, Organizationindex].ToString().Trim();
                               surveyRow = surveyRow.Contains(@"\") ? surveyRow.Replace(@"\",string.Empty).Trim() : surveyRow;
                               var surveyRowID = surTopicQues.FirstOrDefault(x => x.SurveyName == surveyRow);
                               if (surveyRowID != null)
                               {
                                   var dbTopic = surTopicQues.Where(x => x.SurveyID == surveyRowID.SurveyID).Distinct().ToList();
                                   //var dbQues = surTopicQues.Where(x => dbTopic.Count(y => y)).Distinct().ToList();
                                   string key = ModelInfo[fila, QuestionList.IndexOf("Key")].ToString().Replace('"',' ').Trim();
                                   if (surTopicQues.Count(x => x.SurveyName == surveyRow) > 0) //ver si esta agregado el survey de lo contrario no iterar en las preguntas o registros
                                   {
                                       var appSurID = 0;
                                       if (Appliedsurvey.Count(x => x.Key == key) == 0)// verificar si ya esta guardado el key
                                       {
                                           tblAppliedSurveys AppliedSurvey = new tblAppliedSurveys();
                                           AppliedSurvey.key_key = key;
                                           AppliedSurvey.dateIn = DateTime.Parse(ModelInfo[fila, QuestionList.IndexOf("Date In")].ToString().Trim());
                                           AppliedSurvey.dateOut = DateTime.Parse(ModelInfo[fila, QuestionList.IndexOf("Date Out")].ToString().Trim());
                                           AppliedSurvey.email = ModelInfo[fila, QuestionList.IndexOf("Email Address")].ToString().Trim();
                                           AppliedSurvey.organizationID = ModelInfo[fila, QuestionList.IndexOf("Full Organization Name")].ToString().Trim();
                                           AppliedSurvey.hadProblem = bool.Parse(ModelInfo[fila, QuestionList.IndexOf("Had Problem")].ToString().Trim());
                                           AppliedSurvey.hadComment = bool.Parse(ModelInfo[fila, QuestionList.IndexOf("Had Comment")].ToString().Trim());
                                           AppliedSurvey.marketCodeID = FileName.Contains("Tafer Resorts") ? ModelInfo[fila, QuestionList.IndexOf("Market segment")].ToString().Trim() :
                                                                        ModelInfo[fila, QuestionList.IndexOf("Marketing code")].ToString().Trim();
                                           AppliedSurvey.rate = ModelInfo[fila, QuestionList.IndexOf("Survey Score")] == null ? 0 :
                                                                double.Parse(ModelInfo[fila, QuestionList.IndexOf("Survey Score")].ToString().Trim());
                                           AppliedSurvey.Room = ModelInfo[fila, QuestionList.IndexOf("Room")].ToString().Trim();
                                           AppliedSurvey.sentDate = DateTime.Parse(ModelInfo[fila, QuestionList.IndexOf("Sent Date")].ToString().Trim());
                                           AppliedSurvey.submittedDate = DateTime.Parse(ModelInfo[fila, QuestionList.IndexOf("Submitted Date")].ToString().Trim());
                                           AppliedSurvey.surveyLenguage = ModelInfo[fila, QuestionList.IndexOf("Survey Language")].ToString().Trim();
                                           AppliedSurvey.propertyCode = int.Parse(ModelInfo[fila, QuestionList.IndexOf("Property Code")].ToString().Trim());
                                           AppliedSurvey.surveyID = surveyRowID.SurveyID;
                                           db.tblAppliedSurveys.AddObject(AppliedSurvey);
                                           db.SaveChanges();

                                           appSurID = AppliedSurvey.surveyAppliedID;

                                           for (int col = 1; col <= limiteColumnas; col++)//iterar en preguntas 
                                           {
                                               dbTopic = newSurTopics.Count() > 0 ? dbTopic.Where(x => x.SurveyID == surveyRowID.SurveyID).Concat(newSurTopics.Where(x => x.SurveyID == surveyRowID.SurveyID).Distinct()).ToList() : dbTopic;
                                               string answer = ModelInfo[fila, col] != null ? ModelInfo[fila, col].ToString().Trim() : null;// respuesta
                                               string topicQuesExcel = ModelInfo[2, col].ToString().Trim();

                                               if (col != QuestionList.IndexOf(topicQuesExcel) && answer != null)
                                               {
                                                   string topicExcel = topicQuesExcel.Contains(':') ? topicQuesExcel.Split(':').First().Trim() : "Without Topic";
                                                   var questions = dbTopic.Where(x => x.TopicQuesName == topicExcel).Distinct().ToList();
                                                   if (questions.Count() > 0)
                                                   {
                                                       string quesExcel = topicQuesExcel.Split(':').Last().Trim();
                                                       if (questions.Count(x => x.Question == quesExcel) > 0)
                                                       {
                                                           var question = questions.Single(x => x.Question == quesExcel);
                                                           if (answer != null) //si la celda de la respuesta contiene informacion
                                                           {
                                                               tblAnswers record = new tblAnswers();
                                                               record.answer = answer;
                                                               record.questionID = question.QuestionID;
                                                               record.surveyAppliedID = appSurID;
                                                               db.tblAnswers.AddObject(record);
                                                               db.SaveChanges();
                                                           }
                                                       }
                                                       else//no hay pregunta relacionada con el topic agregar pregunta al topic => agregar resp .- envíar correo 
                                                       {
                                                           tblQuestions question = new tblQuestions();
                                                           question.name = quesExcel;
                                                           question.topicQuestionID = questions.FirstOrDefault().TopicQuesID;
                                                           question.userID = userID;
                                                           question.dateSaved = DateTime.Now.Date;
                                                           db.tblQuestions.AddObject(question);
                                                           db.SaveChanges();

                                                           tblAnswers record = new tblAnswers();
                                                           record.answer = answer;
                                                           record.questionID = question.questionID;
                                                           record.surveyAppliedID = appSurID;
                                                           db.tblAnswers.AddObject(record);
                                                           db.SaveChanges();

                                                           //agregar registro de la pregunta a la lista de topics del survey en curso
                                                           newSurTopics.Add(
                                                           new SurveyViewModel.SurveysTopics()
                                                           {
                                                               SurveyID = surveyRowID.SurveyID,
                                                               SurveyName = surveyRow,
                                                               TopicQuesID = question.topicQuestionID,
                                                               TopicQuesName = topicExcel,
                                                               QuestionID = question.questionID,
                                                               Question = question.name
                                                           });
                                                       }
                                                   }
                                                   else //no existe topic en el survey => agregar topic => agregar pregunta al topic => agregar respuesta con el id de la pregunta
                                                   {
                                                       tblTopicQuestions topic = new tblTopicQuestions();
                                                       topic.name = topicExcel;
                                                       topic.userID = userID;
                                                       topic.dateSaved = DateTime.Now.Date;
                                                       topic.surveyID = surveyRowID.SurveyID;
                                                       db.tblTopicQuestions.AddObject(topic);
                                                       db.SaveChanges();

                                                       tblQuestions question = new tblQuestions();
                                                       question.name = topicQuesExcel.Split(':').Last().Trim();
                                                       question.topicQuestionID = topic.topicQuestionID;
                                                       question.userID = userID;
                                                       question.dateSaved = DateTime.Now.Date;
                                                       db.tblQuestions.AddObject(question);
                                                       db.SaveChanges();

                                                       tblAnswers record = new tblAnswers();
                                                       record.answer = answer;
                                                       record.questionID = question.questionID;
                                                       record.surveyAppliedID = appSurID;
                                                       db.tblAnswers.AddObject(record);
                                                       db.SaveChanges();

                                                       newSurTopics.Add(
                                                       new SurveyViewModel.SurveysTopics()
                                                       {
                                                           SurveyID = surveyRowID.SurveyID,
                                                           SurveyName = surveyRow,
                                                           TopicQuesID = topic.topicQuestionID,
                                                           TopicQuesName = topic.name,
                                                           QuestionID = question.questionID,
                                                           Question = question.name
                                                       });
                                                   }
                                               }
                                           }
                                       }
                                       else//else ya existe registro en la db
                                       {
                                           //survey ya existe
                                           emailBody += "Survey ya esta regsitrado Key = " + key + " SurveyID = " + surveyRowID + " Survey = " + surveyRow + "\n" + Environment.NewLine;
                                       }
                                   }
                                 /*  else //else survey no encontrado
                                   {
                                       //SEND Email survey no encontrado
                                       emailBody += "El survey " + surveyRow + " no esta registrado, " + key + " " + FileName + "\n" + Environment.NewLine;
                                   }*/
                               }//if survey
                               else
                               {
                                   emailBody += "El survey " + surveyRow + " no esta registrado, " + " " + FileName + "\n" + Environment.NewLine;
                               }
                           }//FOR
                       }//if
                   }//try
                   catch (Exception ex)
                   {
                       emailBody += emailBody += "Error trying save survey " + FileName+" \n" + ex;
                       System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                       email.From = new System.Net.Mail.MailAddress("rsalinas@villagroup.com");
                       email.Subject = "Surveys " + DateTime.Now.Date;
                       email.Body = emailBody;
                       email.To.Add("rsalinas@villagroup.com");
                       //EmailNotifications.Send(email);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
            }

          /*   if(emailBody != "")
             {
                 System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                 email.From = new System.Net.Mail.MailAddress("rsalinas@villagroup.com");
                 email.Subject = "Surveys " + DateTime.Now.Date;
                 email.Body = emailBody;
                 email.To.Add("rsalinas@villagroup.com");
                 EmailNotifications.Send(email);
             }*/
     
             return emailBody;
        }

        public static object[,] ExcelScanIntenal(Workbook workBookIn)
        {
            int numSheets = workBookIn.Sheets.Count;
            object[,] ExcelContent = new object[0,0];
            for (int sheetNum = 1; sheetNum < numSheets + 1; sheetNum++)
            {
                Worksheet sheet = (Worksheet)workBookIn.Sheets[sheetNum];
                Range excelRange = sheet.UsedRange;
                object[,] valueArray = (object[,])excelRange.get_Value(
                XlRangeValueDataType.xlRangeValueDefault);
                ExcelContent = valueArray;    
            }
            return ExcelContent;
        }
        public static void SurveySendEx(SurveyViewModel.SurveyModelResponse model)
        {
            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();          
            Guid currentUser = Membership.GetUser() == null ? Guid.Parse("c53613b6-c8b8-400d-95c6-274e6e60a14a") : session.UserID;
            ePlatEntities db = new ePlatEntities();
            var mail = "rsalinas@villagroup.com";
            email.From = new System.Net.Mail.MailAddress("rsalinas@villagroup.com");
            var date = DateTime.Now.ToString();
            email.Subject = " " + date;
            email.Body = model.subject +"\n";
            foreach(var obj in model.list)
            {
                email.Body += " " + obj + " \n";
            }
            email.Body += model.fileName+" status : "+ model.status +"\n"+ model.error;
            email.To.Add(mail);
            //EmailNotifications.Send(email);
            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
        }   
        public static AttemptResponse GetStastsTaskScheduler()
        {
            AttemptResponse response = new AttemptResponse();
            var currentDate = DateTime.Now;
            int[] SurveysIDs = new int[3] { 12, 15, 18 };
            try
            {
                for (var x = 0; x < 3; x++)
                {
                    switch (SurveysIDs[x])
                    {
                        case 18:
                        case 12:
                            {
                                var fields = "";
                                fields += "[{" + '"' + "FieldName" + '"' + ':' + '"' + "FromDate" + '"' + ',' + '"' + "Value" + '"' + ':' + '"' + currentDate.Date.ToString("yyyy-MM-dd") + '"' + "}" + ',';
                                fields += "{" + '"' + "FieldName" + '"' + ':' + '"' + "ToDate" + '"' + ',' + '"' + "Value" + '"' + ':' + '"' + currentDate.Date.AddDays(-8).ToString("yyyy-MM-dd") + '"' + "}" + ',';
                                fields += "{" + '"' + "FieldName" + '"' + ':' + '"' + "MarketingSource" + '"' + ',' + '"' + "Value" + '"' + ':' + '"' + '"' + "},";
                                fields += "{" + '"' + "FieldName" + '"' + ':' + '"' + "Resort" + '"' + ',' + '"' + "Value" + '"' + ':' + '"' + '"' + "},";
                                fields += "{" + '"' + "FieldName" + '"' + ':' + '"' + "Destination" + '"' + ',' + '"' + "Value" + '"' + ':' + '"' + '"' + "},";
                                fields += "{" + '"' + "FieldName" + '"' + ':' + '"' + "Terminal" + '"' + ',' + '"' + "Value" + '"' + ':' + '"' + '"' + "}]";                                
                                SurveyDataModel.GetStats(SurveysIDs[x], fields);
                                break;
                            }
                        case 15:
                            {
                                var fields = "";
                                fields += "[{" + '"' + "FieldName" + '"' + ':' + '"' + "FromDate" + '"' + ',' + '"' + "Value" + '"' + ':' + '"' + currentDate.Date.ToString("yyyy-MM-dd") + '"' + "}" + ',';
                                fields += "{" + '"' + "FieldName" + '"' + ':' + '"' + "ToDate" + '"' + ',' + '"' + "Value" + '"' + ':' + '"' + currentDate.Date.AddDays(-8).ToString("yyyy-MM-dd") + '"' + "}" + ',';
                                fields += "{" + '"' + "FieldName" + '"' + ':' + '"' + "Terminal" + '"' + ',' + '"' + "Value" + '"' + ':' + '"' + '"' + "}]";
                                SurveyDataModel.GetStats(SurveysIDs[x], fields);
                                break;
                            }
                      }
                   }
                    response.Message = "GetStats - Task Success";
                    response.ObjectID = 0;
                    response.Type = Attempt_ResponseTypes.Ok;
                    return response;

                }catch(Exception ex)
                {
                    response.Message = "Error!";
                    response.ObjectID = 0;
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Exception = ex;
                    return response;
                }
           }
    }
}