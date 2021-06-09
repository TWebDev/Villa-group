using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Routing;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Web.Mvc;
using System.Diagnostics;
using System.Net;
using System.Timers;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ePlatBack.Models.DataModels
{
    public class RemindersDataModel
    {
      //  public static RemindersViewModel.SearchRemindersModel GetRemindersUser()
        public static List<RemindersViewModel.Reminder> GetRemindersUser()
        {
            ePlatEntities db = new ePlatEntities();
            UserSession session = new UserSession();
            Guid UserID = session.UserID;
            return (from reminder in db.tblPersonalReminders
                    join task in db.tblTaskScheduler on reminder.taskSchedulerID equals task.taskShedulerID
                    where reminder.savedByUserID == UserID && task.active
                    select new RemindersViewModel.Reminder()
                    {
                        reminderID = reminder.reminderID,
                        name = reminder.name,
                        description = reminder.description,
                        url = reminder.url,
                        forUserID = reminder.forUserID,
                        dateAlarm = task.nextExecutionDate.HasValue ? task.nextExecutionDate.Value : DateTime.Now,
                        dateSaved = reminder.dateSaved,
                        savedByUserID = reminder.savedByUserID,
                        lastModificationDate = reminder.lastModificationDate,
                        modifiedByUserID = reminder.modifiedByUserID,
                        typeID = task.recurEvery,
                        numberDayString = task.numberDays,
                        repeat = new RemindersViewModel.daysAndMonths()
                        {
                            weekly = new RemindersViewModel.Weekly()
                            {
                                Monday = task.monday.Value,
                                Tuesday = task.tuesday.Value,
                                Wednesday = task.wednesday.Value,
                                Thursday = task.thursday.Value,
                                Friday = task.friday.Value,
                                Saturday = task.saturday.Value,
                                Sunday = task.sunday.Value
                            },
                            montly = new RemindersViewModel.Montly()
                            {
                                January = task.january.Value,
                                February = task.february.Value,
                                March = task.march.Value,
                                April = task.april.Value,
                                May = task.may.Value,
                                June = task.june.Value,
                                July = task.july.Value,
                                August = task.august.Value,
                                September = task.september.Value,
                                October = task.october.Value,
                                November = task.november.Value,
                                December = task.december.Value
                            }
                        }
                    }).ToList();
        }
        public static AttemptResponse SaveReminder(RemindersViewModel.Reminder model)
        {
            ePlatEntities db = new ePlatEntities();
            UserSession session = new UserSession();
            AttemptResponse response = new AttemptResponse();
            TaskSchedulerDataModel TaskDataModel = new TaskSchedulerDataModel();
            string host = HttpContext.Current.Request.Url.Host.ToString();
            try
            {
                if (model.reminderID == null)
                {
                    tblTaskScheduler task = new tblTaskScheduler();
                    task.description = "Title: " + model.name + System.Environment.NewLine + "Description: " + model.description;
                    task.fromDate = model.dateAlarm;
                    task.toDate = null;////////////////////////////////////
                    
                    task.active = true;
                    task.permanent = true;
                    task.recurEvery = model.typeID;
                    task.saveByUserID = session.UserID;
                    task.dateSaved = DateTime.Now;
                    task.numberDays = model.numberDayString;
                    if (model.typeID == "1")
                    {
                        task.permanent = true;
                    }
                    if (model.typeID == "2")
                    {
                        task.monday = model.repeat.weekly.Monday;
                        task.tuesday = model.repeat.weekly.Tuesday;
                        task.wednesday = model.repeat.weekly.Wednesday;
                        task.thursday = model.repeat.weekly.Thursday;
                        task.friday = model.repeat.weekly.Friday;
                        task.saturday = model.repeat.weekly.Saturday;
                        task.sunday = model.repeat.weekly.Sunday;
                    }
                    if (model.typeID == "3")
                    {
                        //months
                        task.january = model.repeat.montly.January;
                        task.february = model.repeat.montly.February;
                        task.march = model.repeat.montly.March;
                        task.april = model.repeat.montly.April;
                        task.may = model.repeat.montly.May;
                        task.june = model.repeat.montly.June;
                        task.july = model.repeat.montly.July;
                        task.august = model.repeat.montly.August;
                        task.september = model.repeat.montly.September;
                        task.october = model.repeat.montly.October;
                        task.november = model.repeat.montly.November;
                        task.december = model.repeat.montly.December;
                    }
                    if (model.typeID == "4")
                    {
                        task.permanent = false;
                    }
                    tblPersonalReminders reminder = new tblPersonalReminders();
                    reminder.name = model.name;
                    reminder.description = model.description;
                    reminder.dateAlarm = model.dateAlarm;
                    reminder.forUserID = model.forUserID;
                    reminder.url = model.url;
                    reminder.dateSaved = DateTime.Now;
                    reminder.savedByUserID = session.UserID;
                    reminder.taskSchedulerID = task.taskShedulerID;
                    db.tblPersonalReminders.AddObject(reminder);
                    db.tblTaskScheduler.AddObject(task);
                    task.url ="http://" + host + "/crm/notifications/SetNotificationReminder?reminderID="+reminder.reminderID;
                    db.SaveChanges();
                    TaskDataModel.GetNextExecution(reminder.tblTaskScheduler.taskShedulerID,null);
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Reminder Saved Success";
                    response.ObjectID = reminder.reminderID;
                }
                else
                {
                    var reminder = db.tblPersonalReminders.Single(x => x.reminderID == model.reminderID);
                    reminder.name = model.name;
                    reminder.description = model.description;
                    reminder.dateAlarm = model.dateAlarm;
                    reminder.forUserID = model.forUserID;
                    reminder.url = model.url;
                    reminder.lastModificationDate = DateTime.Now;
                    reminder.modifiedByUserID = session.UserID;
                    reminder.tblTaskScheduler.description = "Title: " + model.name + System.Environment.NewLine + "Description: " + model.description;
                    reminder.tblTaskScheduler.fromDate = model.dateAlarm;
                    reminder.tblTaskScheduler.toDate = null;////////////////////////////////////
                    reminder.tblTaskScheduler.url = "http://" + host + "/crm/notifications/SetNotificationReminder?reminderID=" + reminder.reminderID;
                    reminder.tblTaskScheduler.active = true;
                    reminder.tblTaskScheduler.recurEvery = model.typeID;
                    reminder.tblTaskScheduler.numberDays = model.numberDayString;

                    if (model.typeID == "1")
                    {
                        reminder.tblTaskScheduler.permanent = true;
                    }
                    if (model.typeID == "2")
                    {
                        reminder.tblTaskScheduler.permanent = true;
                        reminder.tblTaskScheduler.monday = model.repeat.weekly.Monday;
                        reminder.tblTaskScheduler.tuesday = model.repeat.weekly.Tuesday;
                        reminder.tblTaskScheduler.wednesday = model.repeat.weekly.Wednesday;
                        reminder.tblTaskScheduler.thursday = model.repeat.weekly.Thursday;
                        reminder.tblTaskScheduler.friday = model.repeat.weekly.Friday;
                        reminder.tblTaskScheduler.saturday = model.repeat.weekly.Saturday;
                        reminder.tblTaskScheduler.sunday = model.repeat.weekly.Sunday;
                    }
                    else
                    {
                        reminder.tblTaskScheduler.permanent = true;
                        reminder.tblTaskScheduler.monday = null;
                        reminder.tblTaskScheduler.tuesday = null;
                        reminder.tblTaskScheduler.wednesday = null;
                        reminder.tblTaskScheduler.thursday = null;
                        reminder.tblTaskScheduler.friday = null;
                        reminder.tblTaskScheduler.saturday = null;
                        reminder.tblTaskScheduler.sunday = null;
                    }
                    if (model.typeID == "3")
                    {
                        //months
                        reminder.tblTaskScheduler.permanent = true;
                        reminder.tblTaskScheduler.january = model.repeat.montly.January;
                        reminder.tblTaskScheduler.february = model.repeat.montly.February;
                        reminder.tblTaskScheduler.march = model.repeat.montly.March;
                        reminder.tblTaskScheduler.april = model.repeat.montly.April;
                        reminder.tblTaskScheduler.may = model.repeat.montly.May;
                        reminder.tblTaskScheduler.june = model.repeat.montly.June;
                        reminder.tblTaskScheduler.july = model.repeat.montly.July;
                        reminder.tblTaskScheduler.august = model.repeat.montly.August;
                        reminder.tblTaskScheduler.september = model.repeat.montly.September;
                        reminder.tblTaskScheduler.october = model.repeat.montly.October;
                        reminder.tblTaskScheduler.november = model.repeat.montly.November;
                        reminder.tblTaskScheduler.december = model.repeat.montly.December;
                    }
                    else
                    { 
                        reminder.tblTaskScheduler.january = null;
                        reminder.tblTaskScheduler.february = null;
                        reminder.tblTaskScheduler.march = null;
                        reminder.tblTaskScheduler.april = null;
                        reminder.tblTaskScheduler.may = null;
                        reminder.tblTaskScheduler.june = null;
                        reminder.tblTaskScheduler.july = null;
                        reminder.tblTaskScheduler.august = null;
                        reminder.tblTaskScheduler.september = null;
                        reminder.tblTaskScheduler.october = null;
                        reminder.tblTaskScheduler.november = null;
                        reminder.tblTaskScheduler.december = null;
                    }
                    db.SaveChanges();
                    TaskDataModel.GetNextExecution(reminder.tblTaskScheduler.taskShedulerID, TaskDataModel.GetSource(reminder.tblTaskScheduler.taskShedulerID));
                    //TaskSchedulerDataModel.GetNextExecution(reminder.tblTaskScheduler.taskShedulerID, null);
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Reminder Update Success";
                    response.ObjectID = reminder.reminderID;
                }
            //get method to execute taskScheduler
            }
            catch(Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error trying to save remainder";
                response.Exception = ex;
                response.ObjectID = 0;
            }
            return response;
        }

        public static AttemptResponse DeleteReminder (int reminderID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var reminder = db.tblPersonalReminders.Single(x => x.reminderID == reminderID);
                db.DeleteObject(reminder);
                db.SaveChanges();
                //detener ejecucion > eliminar task > eliminar Reminder
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Reminder deleted Success";
                response.ObjectID = reminderID;
            }
            catch(Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error trying to delete remainder";
                response.Exception = ex;
                response.ObjectID = 0;
            }
            return response; 
        }

        public void NotificationReminder(int reminderID)
        {
            ePlatEntities db = new ePlatEntities();
            tblNotifications notification = new tblNotifications();
            if (db.tblPersonalReminders.Count(x => x.reminderID == reminderID) > 0)
            {
                var reminder = db.tblPersonalReminders.Single(x => x.reminderID == x.reminderID);
                notification.sysItemTypeID = 14;//notification
                notification.notificationTypeID = 2;//PersonalReminders
                notification.description = reminder.name + " " + reminder.description;
                notification.forUserID = reminder.forUserID.HasValue ? reminder.forUserID.Value : reminder.savedByUserID;
                notification.read_ = false;
                notification.eventDateTime = DateTime.Now;
                notification.eventByUserID = reminder.savedByUserID;
                if (reminder.url != null)
                {
                    notification.action = true;
                    notification.url = reminder.url;
                    notification.functionString = "redirect";
                    notification.button = "";
                }
                notification.terminalID = 0;
            }
        }

        public static Guid SaveReminderNotification(int reminderID)
        {
            ePlatEntities db = new ePlatEntities();
            var reminder = db.tblPersonalReminders.Single(x => x.reminderID == reminderID);
            var task = db.tblTaskScheduler.Single(x => x.taskShedulerID == reminder.taskSchedulerID);
            tblNotifications notification = new tblNotifications();
            notification.notificationTypeID = 2; //reminder
            notification.sysItemTypeID = 14;//reminder
            notification.terminalID = 5;//terminalID
            notification.forUserID = reminder.forUserID.HasValue? reminder.forUserID.Value : reminder.savedByUserID; 
            notification.description = reminder.description;
            notification.read_ = false;
            notification.eventDateTime = reminder.dateAlarm;
            notification.eventByUserID = reminder.savedByUserID;
            notification.title = reminder.name;
            if(reminder.url != null)
            {
                notification.action = true;
                notification.url = reminder.url;
                notification.button = " ";
                notification.functionString = " ";
            }
            db.tblNotifications.AddObject(notification);
            db.DeleteObject(reminder);
            db.DeleteObject(task);
            db.SaveChanges();

            return reminder.forUserID.HasValue ? reminder.forUserID.Value : reminder.savedByUserID;
        }
    }
}
