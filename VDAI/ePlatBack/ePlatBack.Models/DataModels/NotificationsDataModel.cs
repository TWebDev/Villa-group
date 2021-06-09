using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models.ViewModels;
using System.Collections.Generic;
using ePlatBack.Models.Utils;
using System.Web.Script.Serialization;
using System.Globalization;

namespace ePlatBack.Models.DataModels
{
    public class NotificationsDataModel
    {
        public static UserSession session = new UserSession();
        public class NotificationsCatalogs
        {
            public static List<SelectListItem> ConvertStringToOptions(string options, bool defaultOption = true)
            {
                var opt = options != null && options != "" ? options.Split(',').ToArray() : new string[] { };
                var list = new List<SelectListItem>();
                if (opt.Count() > 0)
                {
                    foreach (var i in opt)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i,
                            Text = i
                        });
                    }
                }
                if (defaultOption)
                {
                    list.Insert(0, ListItems.Default());
                }
                return list;
            }

            public static List<SelectListItem> FillDrpFieldGroups(string terminals = null)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                var _terminals = terminals != null ? terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray() : session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();

                var query = db.tblFieldGroups.Where(m => _terminals.Contains(m.tblEmailNotifications.terminalID));

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.fieldGroupID.ToString(),
                        Text = i.fieldGroup
                    });
                }
                return list;
            }

            public static string GetSubordinatedUsers()
            {
                ePlatEntities db = new ePlatEntities();

                var query = UserDataModel.GetUsersBySupervisor(null, false, false, true);
                var list = string.Join(",", query.Select(m => m.Text).ToArray());

                return list;
            }

            public static List<SelectListItem> GetEmails()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                //var query = db.tblEmailNotifications.Where(m => terminals.Contains(m.terminalID) && m.active == true);
                var query = db.tblEmails_Terminals.Where(m => terminals.Contains(m.terminalID)).Select(m => new { emailID = m.emailID, email = m.tblEmails.description });

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.emailID.ToString(),
                        Text = i.email
                    });
                }

                return list;
            }
        }

        public string GetFormName(int fieldGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            return db.tblFieldGroups.Single(m => m.fieldGroupID == fieldGroupID).fieldGroup;
        }

        public List<FormsInfoModel> SearchForms(FormsSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            List<FormsInfoModel> list = new List<FormsInfoModel>();
            var _terminals = session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();

            var query = from form in db.tblFieldGroups
                        where _terminals.Contains(form.tblEmailNotifications.terminalID)
                        && (form.fieldGroup.Contains(model.FormSearch_Name) || model.FormSearch_Name == null)
                        && (form.description.Contains(model.FormSearch_Description) || model.FormSearch_Description == null)
                        select new
                        {
                            form.fieldGroupID,
                            form.fieldGroupGUID,
                            form.fieldGroup,
                            form.description,
                            emailNotification = form.tblEmailNotifications.description,
                            userProfile = form.aspnet_Users.tblUserProfiles.FirstOrDefault(),
                            form.dateSaved
                        };

            foreach (var i in query)
            {
                list.Add(new FormsInfoModel()
                {
                    FormInfo_FormID = i.fieldGroupID,
                    FormInfo_FormGUID = i.fieldGroupGUID.ToString(),
                    FormInfo_Name = i.fieldGroup,
                    FormInfo_Description = i.description,
                    FormInfo_EmailNotification = i.emailNotification,
                    FormInfo_SavedByUser = i.userProfile.firstName + " " + i.userProfile.lastName,
                    FormInfo_DateSaved = i.dateSaved.Value.ToString("yyyy-MM-dd")
                });
            }
            return list;
        }

        public List<FieldValueModel> RenderSingleSending(int fieldGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<FieldValueModel>();

            var query = db.tblFields.Where(m => m.fieldGroupID == fieldGroupID);

            foreach (var i in query.OrderBy(m => m.order_))
            {
                //i.options = i.fieldSubTypeID == 18 ? NotificationsCatalogs.GetSubordinatedUsers() : i.options;
                list.Add(new FieldValueModel()
                {
                    FieldValue_FieldID = i.fieldID,
                    FieldValue_Field = i.field,
                    FieldValue_Description = i.description,
                    FieldValue_FieldType = i.fieldTypeID ?? 0,
                    FieldValue_FieldSubType = i.fieldSubTypeID ?? 0,
                    FieldValue_FieldSubTypeText = i.fieldSubTypeID != null ? i.tblFieldSubTypes.subType : "",
                    FieldValue_Options = i.fieldSubTypeID == 18 ? NotificationsCatalogs.ConvertStringToOptions(NotificationsCatalogs.GetSubordinatedUsers(), false) : NotificationsCatalogs.ConvertStringToOptions(i.options)
                });
            }

            return list;
        }

        public AttemptResponse SaveFieldValues(SurveyViewModel.SurveyValuesModel instance, System.Web.HttpContextBase context = null, System.Net.Mail.MailMessage email = null)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            try
            {
                var now = DateTime.Now;
                var list = instance.Fields;
                var surveyID = instance.SurveyID;
                var transactionID = instance.TransactionID;
                var group = db.tblFieldGroups.FirstOrDefault(m => m.fieldGroupGUID == surveyID);
                var notification = group.tblEmailNotifications;
                var systemFields = group.tblFields.Where(m => m.fieldTypeID == 3).Select(m => m.field).ToList();
                var log = db.tblEmailNotificationLogs.Where(m => m.trackingID == transactionID);

                #region "save system fields values"
                foreach (var i in systemFields)
                {
                    var newValue = new tblFieldsValues();
                    var fieldID = group.tblFields.Where(m => m.field == i).Count() > 0 ? SurveyDataModel.getFieldIDByName(group.fieldGroupID, i) : 0;

                    switch (i)
                    {
                        case "$Sent":
                            {
                                newValue.value = now.ToString("yyyy-MM-dd hh:mm:ss tt");
                                break;
                            }
                        case "$SentByUserID":
                            {
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    newValue.value = session.UserID.ToString();
                                }
                                else
                                {
                                    newValue.value = log != null && log.Count() > 0 ? log.FirstOrDefault().sentByUserID.ToString() : "";
                                }
                                break;
                            }
                        case "$SentByUser":
                            {
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    newValue.value = session.User;
                                }
                                else
                                {
                                    newValue.value = log != null && log.Count() > 0 ? db.tblUserProfiles.Where(m => m.userID == log.FirstOrDefault().sentByUserID).Select(m => new { profile = m.firstName + " " + m.lastName }).FirstOrDefault().profile : "";
                                }
                                break;
                            }
                        case "$DepartmentPhone":
                            {
                                var phone = "";
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    phone = session.Phone;
                                }
                                newValue.value = phone;
                                break;
                            }
                        case "$Extension":
                            {
                                var ext = "";
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    ext = session.Extension;
                                }
                                newValue.value = ext;
                                break;
                            }
                        case "$UserFirstName":
                            {
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    newValue.value = session.User.Split(' ')[0];
                                }
                                else
                                {
                                    newValue.value = log != null && log.Count() > 0 ? db.tblUserProfiles.FirstOrDefault(m => m.userID == log.FirstOrDefault().sentByUserID).firstName : "";
                                }
                                break;
                            }
                        case "$UserLastName":
                            {
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    newValue.value = session.User.Split(' ')[1];
                                }
                                else
                                {
                                    newValue.value = log != null && log.Count() > 0 ? db.tblUserProfiles.FirstOrDefault(m => m.userID == log.FirstOrDefault().sentByUserID).lastName : "";
                                }
                                break;
                            }
                        case "$SentByEmail":
                            {
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    newValue.value = session.Email;
                                }
                                else
                                {
                                    newValue.value = log != null && log.Count() > 0 ? db.aspnet_Users.Single(m => m.UserId == log.FirstOrDefault().sentByUserID).UserName : "";
                                }
                                break;
                            }
                        case "$SentToAddress":
                            {
                                newValue.value = string.Join(",", email.To);
                                break;
                            }
                    }
                    //newValue.terminalID = group.terminalID;
                    newValue.terminalID = notification.terminalID;
                    newValue.dateSaved = now;
                    newValue.transactionID = transactionID;

                    if (fieldID != 0)
                    {
                        newValue.fieldID = fieldID;
                        db.tblFieldsValues.AddObject(newValue);
                    }
                    else
                    {
                        tblFields newField = new tblFields();
                        newField.fieldGuid = Guid.NewGuid();
                        newField.field = i;
                        newField.fieldGroupID = group.fieldGroupID;
                        newField.description = i;
                        newField.fieldTypeID = 3;
                        newField.fieldSubTypeID = 14;
                        newField.visibility = 0;
                        newField.order_ = 0;

                        newField.tblFieldsValues.Add(newValue);
                        db.tblFields.AddObject(newField);
                    }
                    //db.SaveChanges();
                }
                #endregion

                //foreach (var item in list)
                foreach (var item in list.Distinct())
                {
                    var fv = new tblFieldsValues
                    {
                        fieldID = item.FieldID,
                        value = item.Value,
                        //terminalID = group.terminalID,
                        terminalID = notification.terminalID,
                        dateSaved = now,
                        transactionID = transactionID
                    };
                    db.tblFieldsValues.AddObject(fv);
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Email Sent";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Email NOT Sent";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }


        /// <summary>
        /// Gets emailNotification and replace body and subject reserved words by personalized content.
        /// Define on what email object section the replacement will have effect
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AttemptResponse SaveFieldValues(FieldValueModel model, System.Net.Mail.MailMessage emailMessage = null, bool sendOnly = false, System.Web.HttpContextBase context = null)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            SurveyViewModel.SurveyValuesModel survey = new JavaScriptSerializer().Deserialize<SurveyViewModel.SurveyValuesModel>(model.FieldValue_FieldValues);
            DateTime _now = DateTime.Now;

            try
            {
                var group = db.tblFieldGroups.Where(m => m.fieldGroupGUID == survey.SurveyID).FirstOrDefault();
                var systemFields = group.tblFields.Where(m => m.fieldTypeID == 3).Select(m => m.field).ToList();
                var notification = group.tblEmailNotifications;
                var email = new System.Net.Mail.MailMessage();

                if (emailMessage != null)
                {
                    email = emailMessage;
                }
                else
                {
                    email = EmailNotifications.GetEmailByNotification(notification.emailNotificationID);
                    if (email.From.Address == "empty@empty.com")
                    {
                        email.From = new System.Net.Mail.MailAddress(session.Email, email.From.DisplayName);
                    }
                    if (notification.copySender != null && (bool)notification.copySender)
                    {
                        email.Bcc.Add(session.Email);
                    }
                    if (notification.replyTo != null && notification.replyTo != "")
                    {
                        email.ReplyToList.Add(notification.replyTo);
                    }
                    else
                    {
                        email.ReplyToList.Add((email.From.Address == "empty@empty.com" ? session.Email : email.From.Address));
                    }
                }

                #region "save system fields values"
                foreach (var i in systemFields)
                {
                    var newValue = new tblFieldsValues();
                    var fieldID = group.tblFields.Where(m => m.field == i).Count() > 0 ? SurveyDataModel.getFieldIDByName(group.fieldGroupID, i) : 0;

                    switch (i)
                    {
                        case "$Sent":
                            {
                                newValue.value = _now.ToString("yyyy-MM-dd hh:mm:ss tt");
                                break;
                            }
                        case "$SentByUserID":
                            {
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    newValue.value = session.UserID.ToString();
                                }
                                else
                                {
                                    var user = db.aspnet_Membership.Where(m => m.LoweredEmail == email.From.Address);
                                    newValue.value = user.Count() > 0 ? user.FirstOrDefault().UserId.ToString() : "";
                                }
                                break;
                            }
                        case "$SentByUser":
                            {
                                var user = "";
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    user = session.User;
                                }
                                else
                                {
                                    user = email.From.DisplayName;
                                }
                                newValue.value = user;
                                email.Body = email.Body.Replace(i, user);
                                email.Subject = email.Subject.Replace(i, user);
                                break;
                            }
                        case "$DepartmentPhone":
                            {
                                var phone = "";
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    phone = session.Phone;
                                }
                                newValue.value = phone;
                                email.Body = email.Body.Replace(i, phone);
                                break;
                            }
                        case "$Extension":
                            {
                                var ext = "";
                                if (context != null && context.User.Identity.IsAuthenticated)
                                {
                                    ext = session.Extension;
                                }
                                newValue.value = ext;
                                email.Body = email.Body.Replace(i, ext);
                                break;
                            }
                        case "$UserFirstName":
                            {
                                var user = context != null && context.User.Identity.IsAuthenticated ? session.User : email.From.DisplayName;
                                var firstName = user.Split(' ')[0];
                                newValue.value = firstName;
                                email.Body = email.Body.Replace(i, firstName);
                                email.Subject = email.Subject.Replace(i, firstName);

                                if (email.From.DisplayName.IndexOf("$") != -1)
                                {
                                    email.From = new System.Net.Mail.MailAddress(email.From.Address, email.From.DisplayName.Replace(i, firstName));
                                }
                                break;
                            }
                        case "$UserLastName":
                            {
                                var user = context != null && context.User.Identity.IsAuthenticated ? session.User : email.From.DisplayName;
                                var ln = user.Split(' ');
                                var lastName = ln.Skip(1);
                                newValue.value = string.Join(" ", lastName);
                                email.Body = email.Body.Replace(i, string.Join(" ", lastName));
                                break;
                            }
                        case "$SentByEmail":
                            {
                                var _email = context != null && context.User.Identity.IsAuthenticated ? session.Email : email.From.Address;
                                email.Body = email.Body.Replace(i, _email);
                                break;
                            }
                        case "$SentToAddress":
                            {
                                newValue.value = string.Join(",", email.To);
                                break;
                            }
                    }
                    newValue.terminalID = notification.terminalID;
                    newValue.dateSaved = _now;
                    newValue.transactionID = survey.TransactionID;

                    if (fieldID != 0)
                    {
                        newValue.fieldID = fieldID;
                        db.tblFieldsValues.AddObject(newValue);
                    }
                    else
                    {
                        tblFields newField = new tblFields();
                        newField.fieldGuid = Guid.NewGuid();
                        newField.field = i;
                        newField.fieldGroupID = group.fieldGroupID;
                        newField.description = i;
                        newField.fieldTypeID = 3;
                        newField.fieldSubTypeID = 14;
                        newField.visibility = 0;
                        newField.order_ = 0;

                        newField.tblFieldsValues.Add(newValue);
                        db.tblFields.AddObject(newField);
                    }
                }
                #endregion

                #region "save form/model fields values & transaction replace"
                foreach (var field in survey.Fields)
                {
                    var newValue = new tblFieldsValues();
                    var _field = db.tblFields.Single(m => m.fieldID == (long)field.FieldID);

                    var _value = field.Value;
                    //do the replace
                    if (_field.tblFieldSubTypes.subType.ToLower().Contains("universal date") && _value != null && _value != "")
                    {
                        //change date format
                        _value = Utils.GeneralFunctions.DateFormat.ToText(DateTime.Parse(_value), notification.tblEmails.culture);
                    }

                    email.Body = email.Body.Replace(_field.field, _value);

                    newValue.fieldID = field.FieldID;
                    newValue.value = field.Value;
                    newValue.terminalID = notification.terminalID;
                    newValue.dateSaved = _now;
                    newValue.transactionID = survey.TransactionID;
                    db.tblFieldsValues.AddObject(newValue);

                    if (_field.tblFieldSubTypes.subType == "Email Address")//target address in form
                    {
                        //email.To.Add("efalcon@villagroup.com");//testing
                        email.To.Add(field.Value);
                    }
                }
                #endregion

                email.Body = EmailNotifications.InsertTracker(email.Body, survey.TransactionID.ToString());
                //con el codigo actual, se puede evitar el guardado condicionando la siguiente línea pues la personalizacion de la plantilla
                //se mezcla con la asignacion de valores a la instancia.
                if (!sendOnly)
                {
                    db.SaveChanges();
                }

                //var _response = EmailNotifications.SendSync(email);
                var _response = EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } }, false);



                //if (_response.Key)
                if (_response.FirstOrDefault().Sent == true)
                {
                    var newValue = new tblFieldsValues();
                    //verify field exists or create it in group
                    var fieldID = group.tblFields.Where(m => m.field == "$Received").Count() > 0 ? SurveyDataModel.getFieldIDByName(group.fieldGroupID, "$Received") : 0;
                    if (fieldID != 0)
                    {
                        newValue.fieldID = SurveyDataModel.getFieldIDByName(group.fieldGroupID, "$Received");
                        newValue.value = _now.ToString("yyyy-MM-dd hh:mm:ss tt");
                        newValue.terminalID = notification.terminalID;
                        newValue.dateSaved = _now;
                        newValue.transactionID = survey.TransactionID;
                        db.tblFieldsValues.AddObject(newValue);
                    }
                    else
                    {
                        tblFields newField = new tblFields();
                        newField.fieldGuid = Guid.NewGuid();
                        newField.field = "$Received";
                        newField.fieldGroupID = group.fieldGroupID;
                        newField.description = "$Received";
                        newField.fieldTypeID = 3;
                        newField.fieldSubTypeID = 15;
                        newField.visibility = 0;
                        newField.order_ = 0;

                        newValue.value = _now.ToString("yyyy-MM-dd hh:mm:ss tt");
                        newValue.terminalID = notification.terminalID;
                        newValue.dateSaved = _now;
                        newValue.transactionID = survey.TransactionID;

                        newField.tblFieldsValues.Add(newValue);
                        db.tblFields.AddObject(newField);
                    }
                    if (!sendOnly)
                    {
                        db.SaveChanges();
                    }
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = 0;
                    //response.Message = "Email Sent";
                    response.Message = "Sent on " + _now.ToString("yyyy-MM-dd hh:mm tt");
                }
                else
                {
                    var newValue = new tblFieldsValues();
                    var fieldID = group.tblFields.Where(m => m.field == "$SendException").Count() > 0 ? SurveyDataModel.getFieldIDByName(group.fieldGroupID, "$SendException") : 0;
                    if (fieldID != 0)
                    {
                        newValue.fieldID = SurveyDataModel.getFieldIDByName(group.fieldGroupID, "$SendException");
                        //newValue.value = _response.Value;
                        newValue.value = _response.FirstOrDefault().Exception;
                        newValue.terminalID = notification.terminalID;
                        newValue.dateSaved = _now;
                        newValue.transactionID = survey.TransactionID;
                        db.tblFieldsValues.AddObject(newValue);
                    }
                    else
                    {
                        tblFields newField = new tblFields();
                        newField.fieldGuid = Guid.NewGuid();
                        newField.field = "$SendException";
                        newField.fieldGroupID = group.fieldGroupID;
                        newField.description = "$SendException";
                        newField.fieldTypeID = 3;
                        newField.fieldSubTypeID = 14;
                        newField.visibility = 0;
                        newField.order_ = 0;

                        //newValue.value = _response.Value;
                        newValue.value = _response.FirstOrDefault().Exception;
                        newValue.terminalID = notification.terminalID;
                        newValue.dateSaved = _now;
                        newValue.transactionID = survey.TransactionID;

                        newField.tblFieldsValues.Add(newValue);
                        db.tblFields.AddObject(newField);
                    }
                    if (!sendOnly)
                    {
                        db.SaveChanges();
                    }
                    response.Type = Attempt_ResponseTypes.Warning;
                    //response.ObjectID = email.To + ": " + _response.Value;
                    response.ObjectID = email.To + ": " + _response.FirstOrDefault().Recipient;
                    response.Message = "There was an error in the sending process. Please verify the address and try again.<br />" + _response.FirstOrDefault().Sent;
                }
                //response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Email NOT Sent";
                response.ObjectID = ex.Message;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteNotificationValues(string transactionID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var transaction = Guid.Parse(transactionID);

            try
            {
                var query = db.tblFieldsValues.Where(m => m.transactionID == transaction);
                foreach (var i in query)
                {
                    db.tblFieldsValues.DeleteObject(i);
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Transaction Deleted";
                response.ObjectID = transactionID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Transaction NOT Deleted";
                response.Exception = ex;
                response.ObjectID = 0;
                return response;
            }
        }

        public List<NotificationHistoryModel> GetNotificationHistory(int FormID, string Date)
        {
            ePlatEntities db = new ePlatEntities();
            List<NotificationHistoryModel> list = new List<NotificationHistoryModel>();
            var fromDate = DateTime.Parse(Date);
            var toDate = DateTime.Parse(Date).AddDays(1).AddSeconds(-1);
            //var fromDate = DateTime.Parse("2018-06-08");
            //var toDate = DateTime.Parse("2018-06-14").AddDays(1).AddSeconds(-1);
            var _fields = db.tblFields.Where(m => m.fieldGroupID == FormID);
            var fields = _fields.Select(m => m.fieldID).ToArray();
            var sentByUserID = _fields.Where(m => m.field == "$SentByUserID").FirstOrDefault().fieldID;
            var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
            var authUsers = isAdmin ? new List<string>() { } : UserDataModel.GetUsersBySupervisor().Select(m => m.Value).ToList();
            var transactionsPerUsers = db.tblFieldsValues.Where(m => m.fieldID == sentByUserID && (m.dateSaved >= fromDate && m.dateSaved <= toDate) && (isAdmin || authUsers.Contains(m.value))).Select(m => m.transactionID).ToList();
            var values = db.tblFieldsValues.Where(m => fields.Contains(m.fieldID) && transactionsPerUsers.Contains(m.transactionID)).GroupBy(m => m.transactionID);

            foreach (var transaction in values)
            {
                var clicks = new List<KeyValuePair<string, string>>();
                var systemFields = transaction.Where(m => m.tblFields.fieldTypeID == 3);
                var notification = new NotificationHistoryModel();
                var clientName = transaction.Count(m => m.tblFields.field.Contains("$ClientName")) > 0 ? transaction.Where(m => m.tblFields.field.Contains("$ClientName")).FirstOrDefault().value : (transaction.Count(m => m.tblFields.field.Contains("$FirstName")) > 0 ? transaction.Where(m => m.tblFields.field.Contains("$FirstName")).FirstOrDefault().value : "")
                + " " + (transaction.Count(m => m.tblFields.field.Contains("$LastName")) > 0 ? transaction.Where(m => m.tblFields.field.Contains("$LastName")).FirstOrDefault().value : "");
                clientName = clientName.TrimStart(' ');
                notification.NotificationHistory_FormID = FormID;
                notification.NotificationHistory_Transaction = transaction.Key.ToString();
                notification.NotificationHistory_Receiver = clientName;
                notification.NotificationHistory_Email = transaction.Count(m => m.tblFields.field == "$Email" || m.tblFields.field == "$EmailAddress") > 0 ? transaction.FirstOrDefault(m => m.tblFields.field == "$Email" || m.tblFields.field == "$EmailAddress").value : "";
                notification.NotificationHistory_DateSent = systemFields.FirstOrDefault(m => m.tblFields.field == "$Sent").value;
                notification.NotificationHistory_DateReceived = systemFields.Count(m => m.tblFields.field == "$Received") > 0 ? systemFields.FirstOrDefault(m => m.tblFields.field == "$Received").value : "";
                notification.NotificationHistory_DateOpened = systemFields.Count(m => m.tblFields.field == "$Open") > 0 ? systemFields.FirstOrDefault(m => m.tblFields.field == "$Open").value : "";
                notification.NotificationHistory_SentByUser = systemFields.Count(m => m.tblFields.field == "$SentByUser") > 0 ? systemFields.FirstOrDefault(m => m.tblFields.field == "$SentByUser").value : "";
                //notification.NotificationHistory_DateReceived = systemFields.FirstOrDefault(m => m.tblFields.field == "$Received").value;
                //notification.NotificationHistory_DateOpened = systemFields.Count(m => m.tblFields.field == "$Open") > 0 ? systemFields.FirstOrDefault(m => m.tblFields.field == "$Open").value : "";
                //notification.NotificationHistory_SentByUser = systemFields.FirstOrDefault(m => m.tblFields.field == "$SentByUser").value;
                foreach (var field in systemFields.Where(m => m.tblFields.field == "$Click"))
                {
                    var pair = new KeyValuePair<string, string>(field.value, clientName);
                    clicks.Add(pair);
                }
                notification.NotificationHistory_Clicks = clicks;
                list.Add(notification);
            }

            return list;
        }

        //public static dynamic TrackEmailOpen(string id)
        public static dynamic TrackEmailOpen(string id)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime _now = DateTime.Now;
            var transaction = Guid.Parse(id);

            var fieldValues = db.tblFieldsValues.Where(m => m.transactionID == transaction);//field values per transaction
            var fieldGroup = fieldValues.FirstOrDefault().tblFields.fieldGroupID;//field group id
            var fields = db.tblFields.Where(m => m.fieldGroupID == fieldGroup).Select(m => m.field).ToList();//fields of group
            var fieldID = fields.Where(m => m == "$Open").Count() > 0 ? SurveyDataModel.getFieldIDByName(fieldGroup, "$Open") : 0;//id of group field named $Open

            #region "save event in notificationEvents"
            if (db.tblEmailNotificationLogs.Count(m => m.trackingID == transaction) > 0)
            {
                tblEmailNotificationEvents n = new tblEmailNotificationEvents();
                n.emailNotificationLogID = db.tblEmailNotificationLogs.FirstOrDefault(m => m.trackingID == transaction).emailNotificationLogID;
                n.sysEventID = 36;//Open Email
                n.dateEvent = _now;
                n.url = null;
                db.tblEmailNotificationEvents.AddObject(n);
                db.SaveChanges();
            }
            #endregion

            #region "save open event"
            tblFieldsValues newValue = new tblFieldsValues();
            tblFields newField = new tblFields();

            if (fieldID != 0)//field already exists in group
            {
                newField.fieldID = fieldID;//for comparison only
                //verify its value
                if (fieldValues.Where(m => m.tblFields.field == "$Open").Count() == 0)
                {
                    newValue.fieldID = fieldID;
                    newValue.value = _now.ToString("yyyy-MM-dd hh:mm:ss tt");
                    newValue.terminalID = fieldValues.FirstOrDefault().terminalID;
                    newValue.dateSaved = _now;
                    newValue.transactionID = transaction;
                    db.tblFieldsValues.AddObject(newValue);
                }
                else if (fieldValues.Where(m => m.tblFields.field == "$Open" && m.value == null).Count() > 0)
                {
                    newValue = fieldValues.FirstOrDefault(m => m.tblFields.field == "$Open" && m.value == null);
                    newValue.value = _now.ToString("yyyy-MM-dd hh:mm:ss tt");
                    db.SaveChanges();
                }
            }
            else
            {
                //create "Open" field and fieldValue with open date(now)
                newField.fieldGuid = Guid.NewGuid();
                newField.field = "$Open";
                newField.fieldGroupID = fieldGroup;
                newField.description = "$Open";
                newField.fieldTypeID = 3;
                newField.fieldSubTypeID = 15;
                newField.visibility = 0;
                newField.order_ = 0;
                db.tblFields.AddObject(newField);

                db.SaveChanges();

                newValue.fieldID = newField.fieldID;
                newValue.value = _now.ToString("yyyy-MM-dd hh:mm:ss tt");
                newValue.terminalID = fieldValues.FirstOrDefault().terminalID;
                newValue.dateSaved = _now;
                newValue.transactionID = transaction;
                db.tblFieldsValues.AddObject(newValue);
            }

            if (db.tblFieldsValues.Where(m => m.transactionID == transaction && (newField.fieldID == m.fieldID)).Count() == 0)
            {
                db.SaveChanges();
            }
            #endregion

            return 1;
        }

        public bool TrackUrlClick(string id, string url)
        {
            if (url == null || url == "")
            {
                return true;
            }
            ePlatEntities db = new ePlatEntities();
            DateTime _now = DateTime.Now;
            var transaction = Guid.Parse(id);
            var fieldValues = db.tblFieldsValues.Where(m => m.transactionID == transaction).Select(m => new { m.fieldID, m.terminalID }).FirstOrDefault();
            var fieldGroup = db.tblFields.FirstOrDefault(m => m.fieldID == fieldValues.fieldID).fieldGroupID;
            var fields = db.tblFields.Where(m => m.fieldGroupID == fieldGroup).Select(m => m.field).ToList();
            var fieldID = fields.Where(m => m == "$Click").Count() > 0 ? SurveyDataModel.getFieldIDByName(fieldGroup, "$Click") : 0;

            #region "save click event"
            if (fieldID != 0)
            {
                //field click exists in the group, create click fieldValue
                tblFieldsValues newValue = new tblFieldsValues();
                newValue.fieldID = fieldID;
                newValue.value = url;
                newValue.terminalID = fieldValues.terminalID;
                newValue.dateSaved = _now;
                newValue.transactionID = transaction;
                db.tblFieldsValues.AddObject(newValue);
            }
            else
            {
                //create field in group before save fieldValue
                tblFields newField = new tblFields();
                newField.fieldGuid = Guid.NewGuid();
                newField.field = "$Click";
                newField.fieldGroupID = fieldGroup;
                newField.description = "$Click";
                newField.fieldTypeID = 3;
                newField.fieldSubTypeID = 14;
                newField.visibility = 0;
                newField.order_ = 0;

                tblFieldsValues newValue = new tblFieldsValues();
                newValue.value = url;
                newValue.terminalID = fieldValues.terminalID;
                newValue.dateSaved = _now;
                newValue.transactionID = transaction;

                newField.tblFieldsValues.Add(newValue);
                db.tblFields.AddObject(newField);
            }
            #endregion

            #region "save click event in notificationEvents only if log exists"
            if (db.tblEmailNotificationLogs.Count(m => m.trackingID == transaction) > 0)
            {
                tblEmailNotificationEvents n = new tblEmailNotificationEvents();
                n.emailNotificationLogID = db.tblEmailNotificationLogs.FirstOrDefault(m => m.trackingID == transaction).emailNotificationLogID;
                n.sysEventID = 37;//clicked Link
                n.dateEvent = _now;
                n.url = url;
                db.tblEmailNotificationEvents.AddObject(n);
            }
            #endregion

            db.SaveChanges();

            TrackEmailOpen(id);

            return true;
        }

        public bool Unsuscribe()
        {
            //call unsuscribe method for certain notifications
            return true;
        }

        public List<KeyValuePair<string, string>> SentLetters(Guid id)
        {
            ePlatEntities db = new ePlatEntities();
            var rsvID = id;
            var list = new List<KeyValuePair<string, string>>();

            var notifications = from logs in db.tblEmailNotificationLogs
                                join n in db.tblEmailNotifications on logs.emailNotificationID equals n.emailNotificationID
                                where logs.reservationID == rsvID
                                select new
                                {
                                    logs.emailNotificationLogID,
                                    //n.emailNotificationID,
                                    n.description
                                };

            if (notifications != null)
            {
                foreach (var i in notifications)
                {
                    var log = i.emailNotificationLogID.ToString();
                    //var log = i.emailNotificationID.ToString();
                    list.Add(new KeyValuePair<string, string>(log, i.description));
                }
            }

            return list;
        }

        public string ViewLetter(object reservationID, int emailNotificationLogID)
        {
            ePlatEntities db = new ePlatEntities();

            var letter = db.tblEmailNotificationLogs.Single(m => m.emailNotificationLogID == emailNotificationLogID);
            var emailNotificationID = letter.emailNotificationID;
            var fieldGroup = letter.tblEmailNotifications.tblFieldGroups.FirstOrDefault();
            var rsvID = Guid.Parse(reservationID.ToString());
            System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmailByNotification(emailNotificationID);


            var query = db.tblReservations.Single(m => m.reservationID == rsvID);
            var assignedTo = query.tblLeads.aspnet_Users1.tblUserProfiles.FirstOrDefault();
            var sentBy = letter.aspnet_Users.tblUserProfiles.FirstOrDefault();
            var presentation = query.tblPresentations.FirstOrDefault();
            //check if can use PreArrivalDataModel.PreviewEmail method
            #region "replace of target specific fields - diamante"
            email.Body = email.Body
                .Replace("$Destination", query.tblDestinations.destination)
                .Replace("$SentByUser", sentBy.firstName + " " + sentBy.lastName)
                .Replace("$AssignedToUser", assignedTo.firstName + " " + assignedTo.lastName)
                .Replace("$SentByPhoneExt", sentBy.phoneEXT)
                .Replace("$SentByEmail", sentBy.aspnet_Users.LoweredUserName);
            #endregion

            #region "replace of target specific fields - reservation"
            email.Body = email.Body
                           .Replace("$ClientName", query.tblLeads.firstName + " " + query.tblLeads.lastName)
                           .Replace("$Resort", query.tblPlaces.place)
                           .Replace("$ArrivalDate", ((DateTime)query.arrivalDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                           .Replace("$PresentationDateTime", ((DateTime)presentation.datePresentation).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + " " + ((TimeSpan)presentation.timePresentation).ToString(@"hh\:mm"))
                           .Replace("$HotelConfirmation", query.hotelConfirmationNumber);
            #endregion

            return email.Body;
        }

        public AttemptResponse SendLetter(string data, System.Web.HttpContextBase context)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var model = new FieldValueModel();
            //var transaction = Guid.NewGuid();
            try
            {
                SPIViewModel.VLOCustomer obj = new JavaScriptSerializer().Deserialize<SPIViewModel.VLOCustomer>(data);
                var fieldGroup = db.tblFieldGroups.Single(m => m.fieldGroupID == obj.Letter);
                var fields = fieldGroup.tblFields;

                foreach (var address in obj.EmailString.Where(m => m != null && m != ""))
                {
                    var transaction = Guid.NewGuid();
                    var listFieldValues = new List<SurveyViewModel.FieldValue>();
                    var instance = new SurveyViewModel.SurveyValuesModel();

                    if (fields.Count(m => m.field.Contains("VPANumber")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.field.Contains("VPANumber")).fieldID, Value = obj.VPANumber });
                    }
                    if (fields.Count(m => m.field.Contains("Title")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.field.Contains("Title")).fieldID, Value = obj.Title });
                    }
                    if (fields.Count(m => m.field.Contains("$FirstName")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.field.Contains("$FirstName")).fieldID, Value = obj.FirstName });
                    }
                    if (fields.Count(m => m.field.Contains("$LastName")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.field.Contains("$LastName")).fieldID, Value = obj.LastName });
                    }
                    if (fields.Count(m => m.field.Contains("CollectDate")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.FirstOrDefault(m => m.field.Contains("CollectDate")).fieldID, Value = (obj.CollectDate ?? "") });
                    }
                    if (fields.Count(m => m.field.Contains("ActivationDate")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.field.Contains("ActivationDate")).fieldID, Value = (obj.ActivationDate ?? "") });
                    }
                    if (fields.Count(m => m.field.Contains("PD")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.field.Contains("PD")).fieldID, Value = obj.PD });
                    }
                    if (fields.Count(m => m.field.Contains("VLO")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.field.Contains("VLO")).fieldID, Value = obj.VLO });
                    }
                    if (fields.Count(m => m.field.Contains("CustomerID")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.field.Contains("CustomerID")).fieldID, Value = (obj.CustomerID != null ? obj.CustomerID.ToString() : null) });
                    }
                    if (fields.Count(m => m.field.Contains("TourID")) > 0)
                    {
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.field.Contains("TourID")).fieldID, Value = (obj.TourID != null ? obj.TourID.ToString() : null) });
                    }
                    if (fields.Count(m => m.field.Contains("Email")) > 0)
                    {
                        //listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.tblFieldSubTypes.subType.Contains("Email Address")).fieldID, Value = obj.EmailString });
                        listFieldValues.Add(new SurveyViewModel.FieldValue() { FieldID = (int)fields.Single(m => m.tblFieldSubTypes.subType.Contains("Email Address")).fieldID, Value = address });
                    }

                    instance.SurveyID = (Guid)fieldGroup.fieldGroupGUID;
                    instance.TransactionID = transaction;
                    instance.Fields = listFieldValues;
                    model.FieldValue_FieldValues = new JavaScriptSerializer().Serialize(instance);
                    response = SaveFieldValues(model, null, false, context);
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = ex.Message;
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }


        public static List<ePlatNotificationsModel.Notification> GetNotifications()
        {
            ePlatEntities db = new ePlatEntities();
            Guid userID = session.UserID;
            var terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToList();
            List<ePlatNotificationsModel.Notification> Notifications = new List<ePlatNotificationsModel.Notification>();
            List<ePlatNotificationsModel.NotificationsReference> getSettings = GetNotificationsReference();

            Notifications = (from not in db.tblNotifications
                                 //  join preference in db.tblNotificationPreferences on not.eventByUserID equals preference.userID
                             join user in db.tblUserProfiles on not.eventByUserID equals user.userID
                             join item in db.tblSysItemTypes on not.sysItemTypeID equals item.sysItemTypeID
                             join eventUser in db.tblUserProfiles on not.eventByUserID equals eventUser.userID
                             join terminal in db.tblTerminals on not.terminalID equals terminal.terminalID
                             where/*getSettings.Count(x => x.userID == not.forUserID) > 0 &&
                                   getSettings.Count(x => x.terminalID == not.terminalID) > 0*/
                                   not.forUserID == userID && not.read_ == false
                             select new ePlatNotificationsModel.Notification()
                             {
                                 notificationID = not.notificationID,
                                 itemID = not.notificationTypeID,
                                 sysItemTypeID = not.sysItemTypeID,
                                 sysItemTypeName = item.sysItemType,
                                 terminalID = not.terminalID,
                                 terminal = terminal.terminal,
                                 forUserID = not.forUserID,
                                 forUserFirstName = user.firstName,
                                 forUserLastName = user.lastName,
                                 description = not.description,
                                 read = not.read_,
                                 eventDateTime = not.eventDateTime,
                                 eventByUserID = not.eventByUserID,
                                 eventUserFirstName = eventUser.firstName,
                                 eventUserLastName = eventUser.lastName,
                                 deliveredDateTime = (DateTime?)null,
                                 readDateTime = not.readDateTime,
                             }).OrderByDescending(x => x.eventDateTime).Take(10).ToList();// only add 10 records to list 
            return Notifications;
        }

        public static bool UpdateNotificationRead(ePlatNotificationsModel.UpdateNotificationDelivered model)
        {
            ePlatEntities db = new ePlatEntities();
            Guid userID = session.UserID;
            try
            {
                var currentNotification = db.tblNotifications.Single(x => x.notificationID == model.notificationID);
                currentNotification.deliveredDateTime = model.deliveredDate;
                currentNotification.readDateTime = model.deliveredDate;
                currentNotification.read_ = true;
                db.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //save notifications to send

        public AttemptResponse SaveNotificationsReference(List<ePlatNotificationsModel.NotificationsReference> model)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            var userID = session.UserID;
            var terminals = session.Terminals.Select(x => long.Parse(x.ToString())).ToArray();
            try
            {
                foreach (var setting in model)
                {
                    var reference = new tblNotificationPreferences();
                    reference.notificationTypeID = setting.notificationTypeID;
                    reference.userID = userID;
                    reference.color = setting.color;
                    reference.terminalID = setting.terminalID;
                    db.tblNotificationPreferences.AddObject(reference);
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = 1;
                response.Message = "Settings Saved Success";
                return response;
            }
            catch (Exception ex)
            {
                response.Exception = ex;
                response.ObjectID = 1;
                response.Message = "Error Settings Saved Success ";
                response.Type = Attempt_ResponseTypes.Error;
                return response;
            }
        }

        public AttemptResponse DeleteNotificationReference(long notificationReferenceID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var setting = db.tblNotificationPreferences.Single(x => x.notificationPreferenceID == notificationReferenceID);
                db.tblNotificationPreferences.DeleteObject(setting);
                response.ObjectID = setting.notificationPreferenceID;
                response.Message = "Setting was deleted success";
                response.Type = Attempt_ResponseTypes.Ok;
                return response;
            }
            catch (Exception ex)
            {
                response.Exception = ex;
                response.ObjectID = 0;
                response.Message = "";
                response.Type = Attempt_ResponseTypes.Error;
                return response;
            }
        }

        public static List<ePlatNotificationsModel.NotificationsReference> GetNotificationsReference()
        {
            ePlatEntities db = new ePlatEntities();
            List<ePlatNotificationsModel.NotificationsReference> list = new List<ePlatNotificationsModel.NotificationsReference>();
            var userID = session.UserID;
            var terminals = session.Terminals.Select(x => (long?)long.Parse(x.ToString())).ToArray();

            list = (from x in db.tblNotificationPreferences
                    join y in db.tblNotificationTypes on x.notificationTypeID equals y.notificationTypeID
                    join z in db.tblUserProfiles on x.userID equals z.userID
                    join w in db.tblTerminals on x.terminalID equals w.terminalID
                    where userID == x.userID && x.terminalID != null && terminals.Contains((long)x.terminalID)
                    select new ePlatNotificationsModel.NotificationsReference()
                    {
                        notificationReferenceID = x.notificationPreferenceID,
                        notificationTypeID = x.notificationTypeID,
                        notificationType = y.notificationType,
                        userID = x.userID,
                        user = z.firstName + " " + z.lastName,
                        color = x.color,
                        terminalID = x.terminalID,
                        terminal = w.terminal
                    }).ToList();

            //user  without settings
            if (list.Count() == 0)
            {
                Guid ePlatUserID = Guid.Parse("c53613b6-c8b8-400d-95c6-274e6e60a14a");
                list = (from x in db.tblNotificationPreferences
                        join y in db.tblNotificationTypes on x.notificationTypeID equals y.notificationTypeID
                        join z in db.tblUserProfiles on x.userID equals z.userID
                        join w in db.tblTerminals on x.terminalID equals w.terminalID
                        where userID == ePlatUserID
                        select new ePlatNotificationsModel.NotificationsReference()
                        {
                            notificationReferenceID = x.notificationPreferenceID,
                            notificationTypeID = x.notificationTypeID,
                            notificationType = y.notificationType,
                            userID = x.userID,
                            user = z.firstName + " " + z.lastName,
                            color = x.color,
                            terminalID = x.terminalID,
                            terminal = w.terminal
                        }).ToList();
            }

            return list;
        }

        public AttemptResponse SaveNotification(ePlatNotificationsModel.Notification model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            Guid userID = session.UserID;

            try
            {
                var notification = new tblNotifications();
                notification.notificationID = model.itemID;
                notification.sysItemTypeID = model.sysItemTypeID;
                notification.terminalID = model.terminalID;
                notification.forUserID = model.forUserID;
                notification.description = model.description;
                notification.read_ = model.read;
                notification.eventDateTime = model.eventDateTime;
                notification.eventByUserID = model.eventByUserID;
                db.tblNotifications.AddObject(notification);

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Notification saved success";
                response.ObjectID = notification.notificationID;
                //send notification to client;

                return response;

            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Exception = ex;
                response.Message = "Notification not saved";
                response.ObjectID = "0";
                return response;
            }
        }

        public static long SaveNotificationWithOutFormat(string _itemID, string _sysItemTypeID, string _terminalID, string _forUserID, string _description, string _eventUserID) //return NotificationID
        {
            ePlatEntities db = new ePlatEntities();
            bool read_ = false;
            DateTime eventDateTime = new DateTime();
            Guid eventByUserID = session.UserID;
            long itemID = long.Parse(_itemID);
            long sysItemTypeID = long.Parse(_sysItemTypeID);
            long terminalID = long.Parse(_terminalID);
            Guid forUserID = Guid.Parse(_forUserID);
            Guid eventUserID = Guid.Parse(_eventUserID);
            try
            {
                var notification = new tblNotifications();
                notification.notificationTypeID = itemID;
                notification.sysItemTypeID = sysItemTypeID;
                notification.terminalID = terminalID;
                notification.forUserID = forUserID;
                notification.description = _description;
                notification.read_ = read_;
                notification.eventDateTime = eventDateTime;
                notification.eventByUserID = eventByUserID;
                db.tblNotifications.AddObject(notification);
                //send notification to client;
                var UrlAction = "http://localhost:45000//crm/Notifications/UpdateNotificacions?userID=" + forUserID;
                return notification.notificationID;
            }
            catch (Exception ex) //not saved
            {
                return 0;
            }
        }

        public static DateTime? ReadAndDeliveredDateTime(string method, long itemID)
        {
            ePlatEntities db = new ePlatEntities();
            var notification = db.tblNotifications.Single(x => x.notificationID == itemID);
            DateTime? response = new DateTime();
            if (method == "read")
            {
                if (notification.read_ == false)
                {
                    notification.readDateTime = DateTime.Now;
                    notification.read_ = true;
                    response = notification.readDateTime.Value;
                }
                else
                {
                    response = (DateTime?)null;
                }
            }
            else
            {
                if (notification.deliveredDateTime == null)
                {
                    notification.deliveredDateTime = DateTime.Now;
                    response = notification.deliveredDateTime.Value;
                }
                else
                {
                    response = notification.deliveredDateTime;
                }
            }
            db.SaveChanges();
            return response;
        }
    }
}
