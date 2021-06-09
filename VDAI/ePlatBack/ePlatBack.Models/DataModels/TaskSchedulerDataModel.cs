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
using Newtonsoft.Json;

namespace ePlatBack.Models.DataModels
{
    public class TaskSchedulerDataModel
    {
        //static System.Timers.Timer timer;
        public static UserSession session = new UserSession();
        static List<tblTaskScheduler> TasksModelList = new List<tblTaskScheduler>();
        static List<System.Timers.Timer> Tasks = new List<System.Timers.Timer>();

        //Tareas una ejecucion
        static Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>> TasksOneTime = new Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>>();
        //Tareas por hora
        static Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>> TasksPerHour = new Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>>();
        //Tareas por dia
        static Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>> TasksPerDay = new Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>>();
        //Tareas por semana
        static Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>> TasksPerWeek = new Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>>();
        //tareas por mes
        static Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>> TasksPerMonth = new Dictionary<int, Dictionary<tblTaskScheduler, System.Timers.Timer>>();

        public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null, string terminalID = null)
        {
            int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request, terminalID);
            return result;
        }
        public TaskSchedulerViewModel.TasksResults SearchTaskScheduler(TaskSchedulerViewModel.SearchTasks model)
        {
            ePlatEntities db = new ePlatEntities();

            var query = from task in db.tblTaskScheduler
                        where (task.nextExecutionDate >= model.SearchFromDate
                              && task.nextExecutionDate <= model.SearchToDate
                              && task.active == true)
                              || (model.SearchFromDate == null && model.SearchToDate == null)
                        select task;

            TaskSchedulerViewModel.TasksResults Task = new TaskSchedulerViewModel.TasksResults();
            TaskSchedulerViewModel.SaveTaskModel RecurList = new TaskSchedulerViewModel.SaveTaskModel();
            Task.TaskSummary = new List<TaskSchedulerViewModel.GetTaskScheduler>();
            foreach (var item in query)
            {
                string fromDate = item.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt");
                string toDate = item.toDate != null ? item.toDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "";

                var user = db.tblUserProfiles.Single(x => x.userID == item.saveByUserID);
                Task.TaskSummary.Add(new TaskSchedulerViewModel.GetTaskScheduler()
                {
                    GetTaskSchedulerID = item.taskShedulerID,
                    GetDescription = item.description,
                    GetUrl = item.url,
                    GetFromDate = fromDate,
                    GetToDate = toDate,
                    GetRecur = RecurList.DropDown_Lapse.FirstOrDefault(x => x.Value == item.recurEvery).Text,
                    GetPermanent = item.permanent,
                    GetActive = item.active,
                    GetSaveByUser = user.firstName + " " + user.lastName,
                    GetDateSaved = item.dateSaved.Date,
                    GetNumberDays = item.numberDays == null ? "" : item.numberDays,
                    //week
                    GetMonday = item.monday == null ? false : item.monday.Value,
                    GetTuesday = item.tuesday == null ? false : item.tuesday.Value,
                    GetWednesday = item.wednesday == null ? false : item.wednesday.Value,
                    GetThursday = item.thursday == null ? false : item.thursday.Value,
                    GetFriday = item.friday == null ? false : item.friday.Value,
                    GetSaturday = item.saturday == null ? false : item.saturday.Value,
                    GetSunday = item.sunday == null ? false : item.sunday.Value,
                    //month
                    GetJanuary = item.january == null ? false : item.january.Value,
                    GetFebruary = item.february == null ? false : item.february.Value,
                    GetMarch = item.march == null ? false : item.march.Value,
                    GetApril = item.april == null ? false : item.april.Value,
                    GetMay = item.may == null ? false : item.may.Value,
                    GetJune = item.june == null ? false : item.june.Value,
                    GetJuly = item.july == null ? false : item.july.Value,
                    GetAugust = item.august == null ? false : item.august.Value,
                    GetSeptember = item.september == null ? false : item.september.Value,
                    GetOctober = item.october == null ? false : item.october.Value,
                    GetNovember = item.november == null ? false : item.november.Value,
                    GetDecember = item.december == null ? false : item.december.Value,
                });
            }
            return Task;
        }
        public AttemptResponse DeleteTaskScheduler(int TaskID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var task = db.tblTaskScheduler.Single(x => x.taskShedulerID == TaskID);

                switch (task.recurEvery)
                {
                    case "1":
                        {
                            DeleteTaskFromDictionary(TaskID, "Day");
                            break;
                        }
                    case "2":
                        {
                            DeleteTaskFromDictionary(TaskID, "Week");
                            break;
                        }
                    case "3":
                        {
                            DeleteTaskFromDictionary(TaskID, "Month");
                            break;
                        }
                    case "4":
                        {
                            DeleteTaskFromDictionary(TaskID, "OneTime");
                            break;
                        }
                    case "5":
                        {
                            DeleteTaskFromDictionary(TaskID, "Hour");
                            break;
                        }
                }


                db.DeleteObject(task);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Task was Deleted Success ";
                response.ObjectID = TaskID.ToString();
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error try to Delete the Task";
                response.ObjectID = -1;
                response.Exception = ex;
            }
            return response;
        }
        public TaskSchedulerViewModel.GetTaskScheduler GetTaskScheduler(int TaskID)
        {
            ePlatEntities db = new ePlatEntities();
            TaskSchedulerViewModel.GetTaskScheduler Task = new TaskSchedulerViewModel.GetTaskScheduler();
            var TaskSaved = db.tblTaskScheduler.Single(x => x.taskShedulerID == TaskID);
            var user = db.tblUserProfiles.Single(x => x.userID == TaskSaved.saveByUserID);
            Task.GetTaskSchedulerID = (int)TaskSaved.taskShedulerID;
            Task.GetDescription = TaskSaved.description;
            Task.GetUrl = TaskSaved.url;
            Task.GetFromDate = TaskSaved.fromDate.ToString("yyyy-MM-dd hh:mm:ss tt");
            Task.GetToDate = TaskSaved.toDate != null ? TaskSaved.toDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "";
            Task.GetRecur = TaskSaved.recurEvery;
            Task.GetPermanent = TaskSaved.permanent;
            Task.GetSaveByUser = user.firstName + " " + user.lastName;
            Task.GetDateSaved = TaskSaved.dateSaved.Date;
            switch (TaskSaved.recurEvery)
            {
                case "2"://week
                    {
                        Task.GetMonday = TaskSaved.monday.Value;
                        Task.GetTuesday = TaskSaved.tuesday.Value;
                        Task.GetWednesday = TaskSaved.wednesday.Value;
                        Task.GetThursday = TaskSaved.thursday.Value;
                        Task.GetFriday = TaskSaved.friday.Value;
                        Task.GetSaturday = TaskSaved.saturday.Value;
                        Task.GetSunday = TaskSaved.sunday.Value;
                        break;
                    }
                case "3"://month
                    {
                        Task.GetNumberDays = TaskSaved.numberDays;
                        Task.GetJanuary = TaskSaved.january.Value;
                        Task.GetFebruary = TaskSaved.february.Value;
                        Task.GetMarch = TaskSaved.march.Value;
                        Task.GetApril = TaskSaved.april.Value;
                        Task.GetMay = TaskSaved.may.Value;
                        Task.GetJune = TaskSaved.june.Value;
                        Task.GetJuly = TaskSaved.july.Value;
                        Task.GetAugust = TaskSaved.august.Value;
                        Task.GetSeptember = TaskSaved.september.Value;
                        Task.GetOctober = TaskSaved.october.Value;
                        Task.GetNovember = TaskSaved.november.Value;
                        Task.GetDecember = TaskSaved.december.Value;
                        break;
                    }
            }
            return Task;
        }

        public AttemptResponse SaveTask(TaskSchedulerViewModel.SaveTaskModel Model)//guardar accion, para proxima repeticion
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var valueID = 0;
            try
            {
                DateTime currentDate = DateTime.Now;
                if (Model.TaskSchedulerID != null)
                {
                    var TaskID = db.tblTaskScheduler.Single(x => x.taskShedulerID == Model.TaskSchedulerID);
                    TaskID.description = Model.Description;
                    TaskID.permanent = Model.Permanent;//usar proxima ejecucion, ejecutar termporalmente
                    TaskID.fromDate = DateTime.Parse(Model.FromDate);//proxima ejecucion
                    TaskID.toDate = Model.ToDate != null ? DateTime.Parse(Model.ToDate) : (DateTime?)null;  //ejecutar hasta
                                                                                                            // TaskID.nextExecutionDate =DateTime.Parse(Model.FromDate);

                    TaskID.url = Model.Url;
                    //days
                    if (Model.Lapse == "2")
                    {
                        TaskID.monday = Model.Monday;
                        TaskID.tuesday = Model.Tuesday;
                        TaskID.wednesday = Model.WednesDay;
                        TaskID.thursday = Model.Thursday;
                        TaskID.friday = Model.Friday;
                        TaskID.saturday = Model.Saturday;
                        TaskID.sunday = Model.Sunday;
                    }
                    if (Model.Lapse == "3")
                    {
                        //months
                        TaskID.january = Model.January;
                        TaskID.february = Model.February;
                        TaskID.march = Model.March;
                        TaskID.april = Model.April;
                        TaskID.may = Model.May;
                        TaskID.june = Model.June;
                        TaskID.july = Model.July;
                        TaskID.august = Model.August;
                        TaskID.september = Model.September;
                        TaskID.october = Model.October;
                        TaskID.november = Model.November;
                        TaskID.december = Model.December;
                        foreach (var day in Model.NumberDays)
                        {
                            TaskID.numberDays += day + ",";
                        }
                    }
                    if (Model.Lapse == "5")
                    {
                        TaskID.atDay = Model.atDay;
                        TaskID.minutes = Model.minutes;
                    }
                    TaskID.recurEvery = Model.Lapse;
                    TaskID.saveByUserID = session.UserID;
                    TaskID.dateSaved = DateTime.Now.Date;

                    db.SaveChanges();
                    valueID = TaskID.taskShedulerID;
                    // GetNextExecution(valueID, GetSource(valueID));//update
                    GetRecurr(valueID);
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Task Updated Success";
                    response.ObjectID = valueID;
                    //return response;
                }
                else
                {
                    tblTaskScheduler newTask = new tblTaskScheduler();
                    newTask.description = Model.Description;
                    newTask.permanent = Model.Permanent;
                    newTask.fromDate = DateTime.Parse(Model.FromDate);
                    newTask.toDate = Model.ToDate != null ? DateTime.Parse(Model.ToDate) : (DateTime?)null;
                    newTask.url = Model.Url;
                    //days
                    if (Model.Lapse == "2")
                    {
                        newTask.monday = Model.Monday;
                        newTask.tuesday = Model.Tuesday;
                        newTask.wednesday = Model.WednesDay;
                        newTask.thursday = Model.Thursday;
                        newTask.friday = Model.Friday;
                        newTask.saturday = Model.Saturday;
                        newTask.sunday = Model.Sunday;
                    }
                    //months
                    if (Model.Lapse == "3")
                    {
                        newTask.january = Model.January;
                        newTask.february = Model.February;
                        newTask.march = Model.March;
                        newTask.april = Model.April;
                        newTask.may = Model.May;
                        newTask.june = Model.June;
                        newTask.july = Model.July;
                        newTask.august = Model.August;
                        newTask.september = Model.September;
                        newTask.october = Model.October;
                        newTask.november = Model.November;
                        newTask.december = Model.December;
                        foreach (var day in Model.NumberDays)
                        {
                            newTask.numberDays += day + ",";
                        }
                    }
                    else
                    {
                        newTask.numberDays = (string)null;
                    }
                    if (Model.Lapse == "5")
                    {
                        newTask.atDay = Model.atDay;
                        newTask.minutes = Model.minutes;
                    }
                    newTask.recurEvery = Model.Lapse;
                    newTask.saveByUserID = session.UserID;
                    newTask.dateSaved = DateTime.Now.Date;
                    newTask.lastExecution = (DateTime?)null;
                    newTask.permanent = Model.Permanent;
                    newTask.active = true;
                    db.tblTaskScheduler.AddObject(newTask);
                    db.SaveChanges();
                    valueID = newTask.taskShedulerID;
                    GetRecurr(valueID);//nuevo
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Task Save Success";
                    response.ObjectID = valueID;
                }

                return response;
            }
            catch (Exception ex)
            {
                // DeleteTaskScheduler(valueID);
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error trying to save the task";
                response.ObjectID = valueID;
                response.Exception = ex;
                return response;
            }
        }
        //--
        public void GetNextExecution(int taskID, object source = null)
        {
            GetRecurr(taskID);


            /*  if (source != null) //atualizar
              {
                  GetRecurr(taskID);
             //     ProgramingTask(taskID, source);
              }
              else//NUEVO
              {
                  GetRecurr(taskID);
               //   ProgramingTask(taskID);
              }*/
        }

        public static void GetRecurr(int taskID)
        {
            ePlatEntities db = new ePlatEntities();
            var Task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID && x.active == true);
            var currentDate = DateTime.Now;
            if (Task.active)
            {
                switch (Task.recurEvery)
                {
                    case "1": //Daily
                        {
                            if (Task.nextExecutionDate == null)//RUN FIRST TIME
                            {
                                Task.nextExecutionDate = Task.fromDate;
                                Task.minutes = 1440;
                                db.SaveChanges();
                                ProgramingTaskDaily(Task.taskShedulerID);
                            }
                            else// matching the execution 
                            {
                                if (Task.nextExecutionDate > DateTime.Now) //programar la ejecucion en tiempo que resta
                                {
                                    ProgramingTaskDaily(Task.taskShedulerID);
                                }
                                else  //recalcular la siguiente ejecucion
                                {
                                    var nextExe = Task.nextExecutionDate.Value;
                                    do
                                    {
                                        nextExe = nextExe.AddDays(1);

                                    } while (DateTime.Now > nextExe);
                                    Task.nextExecutionDate = nextExe;
                                }
                                db.SaveChanges();
                                ProgramingTaskDaily(Task.taskShedulerID);
                            }
                            break;
                        }
                    case "2": //Weekly
                        {
                            var day = "";
                            if (Task.nextExecutionDate == null && Task.lastExecution == null)// primera asignacion de la tarea
                            {
                                currentDate = currentDate > Task.fromDate ? currentDate.AddDays(1) : currentDate;
                                while (Task.nextExecutionDate == null)//nextejectution
                                {
                                    day = currentDate.DayOfWeek.ToString().ToLower();
                                    var condition = Task.GetType().GetProperty(day) == null ? false : bool.Parse(Task.GetType().GetProperty(day).GetValue(Task, null).ToString());
                                    if (condition == true)
                                    {
                                        Task.nextExecutionDate = currentDate.Date + Task.fromDate.TimeOfDay;
                                    }
                                    currentDate = currentDate.AddDays(1);
                                }
                            }
                            else /*if(Task.nextExecutionDate != null)*///reasignacion de la tarea
                            {
                                //currentDate = currentDate > Task.nextExecutionDate ? currentDate : currentDate.AddDays(1); //solo se aumenta el dia en caso de que ya haya pasado la fecha de ejecucion
                                // while (Task.nextExecutionDate != currentDate + Task.fromDate.TimeOfDay) /*&& currentDate >= Task.fromDate*///nextejectution
                                while (currentDate > Task.nextExecutionDate)
                                {
                                    currentDate = currentDate.AddDays(1);
                                    day = currentDate.DayOfWeek.ToString().ToLower();
                                    var condition = Task.GetType().GetProperty(day) == null ? false : bool.Parse(Task.GetType().GetProperty(day).GetValue(Task, null).ToString());
                                    if (condition == true)
                                    {
                                        Task.nextExecutionDate = currentDate.Date + Task.fromDate.TimeOfDay;
                                        break;
                                    }
                                    currentDate = currentDate.AddDays(1);
                                }
                            }
                            db.SaveChanges();
                            ProgramingTaskWeekly(Task.taskShedulerID);
                            break;
                        }
                    case "3": //montly//done
                        {
                            var numberDays = Task.numberDays.Remove(Task.numberDays.Length - 1);
                            var days = numberDays.Split(',').Select(x => int.Parse(x)).ToArray(); //numero de dias a programar                          
                            var months = Task.GetType().GetProperties().Select(x => x.Name).ToArray(); //meses indices 20-31                             
                            List<DateTime> MonthDates = new List<DateTime>();
                            var year = currentDate.Year;
                            var NextDate = DateTime.MinValue;
                            var mes = 1;
                            for (int y = 20; y <= 31; y++)
                            {
                                if (bool.Parse(Task.GetType().GetProperty(months[y]).GetValue(Task, null).ToString())) //si el mes esta registrado
                                {
                                    for (int x = 0; x < days.Count(); x++)
                                    {
                                        if (currentDate.Month <= mes && days[x] >= currentDate.Day)
                                            NextDate = new DateTime(currentDate.Year, mes, days[x]);
                                        else
                                            NextDate = new DateTime(currentDate.Year + 1, mes, days[x]);
                                        MonthDates.Add(NextDate);
                                    }
                                }
                                mes++;
                            }
                            NextDate = MonthDates.OrderBy(x => x).ThenBy(x => x.Day).ThenBy(x => x.Month).ThenBy(x => x.Year).FirstOrDefault(x => x > currentDate);
                            Task.nextExecutionDate = NextDate.Date + Task.fromDate.TimeOfDay;
                            db.SaveChanges();
                            ProgramingTaskMontly(Task.taskShedulerID);
                            break;
                        }
                    case "4": //one time
                        {
                            if (Task.nextExecutionDate == null)
                            {
                                Task.nextExecutionDate = Task.fromDate;
                            }
                            else //reprogramar
                            {
                                if (Task.nextExecutionDate > DateTime.Now)
                                {
                                    ProgramingTaskOneTime(Task.taskShedulerID);
                                }
                                else
                                {

                                }
                            }
                            db.SaveChanges();
                            ProgramingTaskOneTime(taskID);
                            break;
                        }
                    case "5"://many times at day
                        {
                            if (Task.nextExecutionDate == null) //sino tiene siguiente fecha de ejecucion entonces asignar Start Date
                            {
                                Task.nextExecutionDate = Task.fromDate;
                            }
                            else
                            {
                                if (Task.nextExecutionDate > DateTime.Now)
                                {
                                    ProgramingTaskPerHour(Task.taskShedulerID);
                                }
                                else
                                {
                                    var nextExe = Task.nextExecutionDate.Value;
                                    //while
                                    do
                                    {
                                        nextExe = nextExe.AddMinutes(Task.minutes.Value);

                                    } while (DateTime.Now > nextExe);
                                    Task.nextExecutionDate = nextExe;
                                }
                            }
                            db.SaveChanges();
                            ProgramingTaskPerHour(taskID);
                            break;
                        }
                }
            }
        }

        public static void ProgramingTask(int taskID, object tasksource = null)
        {
            ePlatEntities db = new ePlatEntities();
            TaskSchedulerDataModel AssignTask = new TaskSchedulerDataModel();
            var Task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);

            DateTime currentDate = DateTime.Now;
            int interval = (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds > 0 ? (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds : 0;
            int response = 0;
            //
            //eliminar de las listas 
            if (Task.active == false)//Delete data from
            {
                System.Timers.Timer taskSource = (System.Timers.Timer)tasksource;
                var taskModel = TasksModelList.Single(x => x.taskShedulerID == taskID);
                var currentTask = Tasks.Single(x => x == taskSource);
                Tasks.Remove(currentTask);
                TasksModelList.Remove(taskModel);
            }

            if (Task.active && interval > 0 && tasksource != null)
            {
                System.Timers.Timer taskSource = (System.Timers.Timer)tasksource;
                var index = TasksModelList.FindIndex(x => x.taskShedulerID == taskID);
                var currentTask = Tasks[index];
                currentTask.Stop();
                currentTask.Interval = interval;
                currentTask.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTask(source, Task.url, Task.taskShedulerID));
                currentTask.Enabled = true;
                currentTask.AutoReset = false;//si es del tipo diarío entonces autoreset = true;
                currentTask.Start();
            }
            //
            if (Task.active && interval > 0 && tasksource == null)
            {
                System.Timers.Timer task = new System.Timers.Timer();
                task.Interval = interval;
                task.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTask(source, Task.url, Task.taskShedulerID));
                task.Enabled = true;
                task.AutoReset = false;//si es del tipo diarío entonces autoreset = true;
                task.Start();
                Tasks.Add(task);
                TasksModelList.Add(Task);
            }

        }

        //las tareas que son programadas por este metodo se repiten varías veces en el día

        public int RunTask(object source, string taskUrl, int taskID)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime currentDate = DateTime.Now;
            System.Timers.Timer currentTask = (System.Timers.Timer)source;//get current task;
            AttemptResponse response = new AttemptResponse();
            try
            {
                var exist = db.tblTaskScheduler.FirstOrDefault(x => x.taskShedulerID == taskID) != null ? true : false;
                currentTask.Enabled = false;
                currentTask.Stop();
                if (exist)
                {
                    var req = (HttpWebRequest)WebRequest.Create(taskUrl);
                    req.ContentType = "application/json";
                    req.Method = "POST";
                    req.Timeout = 600000; // =  10min 
                    req.ContentLength = 0;
                    var res = (HttpWebResponse)req.GetResponse();//obtener respuesta
                    Stream respuesta = res.GetResponseStream();// status = ok ? si se ejecuto : catch(ex) 
                    TaskSchedulerViewModel.TaskResponse respFromServer = new TaskSchedulerViewModel.TaskResponse();
                    using (StreamReader reader = new StreamReader(respuesta))
                    {
                        respFromServer.response = reader.ReadToEnd();
                        respFromServer.date = DateTime.Now;
                    }
                    var respList = respFromServer.response.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\\", string.Empty)
                                   .Replace("\"", string.Empty).Split(',').Select(x => x).ToList();

                    if (respList.Count() > 3)
                    {
                        response.Type = respList[0].Split(':').Last().ToString() == "1" ? Attempt_ResponseTypes.Ok : Attempt_ResponseTypes.Error;
                        response.Message = respList[1].Split(':').Last().ToString();
                        response.ObjectID = respList[4].Split(':').Last().ToString();
                        Task.Run(() => CreateTableUserLogActivityAsync("", "TaskScheduler", response, "", null, "5"));
                        return 1;
                    }
                    else
                    {
                        Task.Run(() => CreateTableUserLogActivityAsync("", "TaskScheduler", respList, "", null, "5"));
                        return 1;
                    }
                }
                else//delete from TASKS
                {
                    var task = Tasks.FirstOrDefault(x => x == currentTask) != null ? true : false;
                    if (task)
                    {
                        tblTaskScheduler taskInList = TasksModelList.Single(x => x.taskShedulerID == taskID);
                        TasksModelList.Remove(taskInList);
                        Tasks.Remove(currentTask);
                    }
                    return 0;
                }
                //guardar en tabla de tarea ejecutada
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
                var hourLapse = (DateTime.Now - currentDate).Hours;
                task.lastExecution = currentDate.AddHours(hourLapse * -1);
                //     db.SaveChanges();
                GetNextExecution(taskID, source);
                //  NextTask();
            }
        }

        public object GetSource(int taskID)
        {
            return Tasks[TasksModelList.FindIndex(x => x.taskShedulerID == taskID)];
        }

        public static void TurnOnTaskScheduler()
        {
           /* ePlatEntities db = new ePlatEntities();
            TaskSchedulerDataModel OnRunTask = new TaskSchedulerDataModel();
            tblNotifications notification = new tblNotifications();
            notification.sysItemTypeID = 14;
            notification.terminalID = 5;
            notification.forUserID = Guid.Parse("7310ece1-bab5-4a69-a969-af6c44157c59");
            notification.read_ = false;
            notification.eventDateTime = DateTime.Now;
            notification.eventByUserID = Guid.Parse("c53613b6-c8b8-400d-95c6-274e6e60a14a");//system
            notification.title = "Turn On TaskScheduler Module";
            var Object = "";
            try
            {
                notification.description = "Task was added success" + System.Environment.NewLine + "";
                List<tblTaskScheduler> getTask = (from task in db.tblTaskScheduler
                                                  where task.active
                                                  select task).ToList();
                if (Tasks.Count() == 0 && TasksModelList.Count() == 0)
                {
                    foreach (var task in getTask)
                    {
                        try
                        {
                            GetRecurr(task.taskShedulerID);
                        }
                        catch (Exception ex)
                        {
                            Object = Object +" - "+task.taskShedulerID.ToString();
                        }
                    }
                }

            } catch (Exception ex)
            {
                notification.notificationTypeID = 3; //system
                notification.description = "It was an error try to turn on the taskscheduler module  "+ ex.ToString() + " " + Object;
                db.tblNotifications.AddObject(notification);
                db.SaveChanges();
            }*/
        }
        //old function TaskScheduler
        public static void _GetNextExecution_(int TaskID, object taskObject = null)
        {
            ePlatEntities db = new ePlatEntities();
            TaskSchedulerDataModel runTask = new TaskSchedulerDataModel();
            var Task = db.tblTaskScheduler.Single(x => x.taskShedulerID == TaskID && x.active == true);
            System.Timers.Timer timer = (System.Timers.Timer)taskObject;
            var reminder = db.tblPersonalReminders.FirstOrDefault(x => x.taskSchedulerID == Task.taskShedulerID);
            var currentDate = DateTime.Now;
            var response = 0;
            if (reminder != null)
            {
                Task.url += reminder.reminderID.ToString();
            }
            if (Task.active)
            {
                //if(currentDate >= Task.toDate && Task.permanent == false)
                if (Task.permanent == false || (Task.toDate != null && currentDate >= Task.toDate))///JUST ONE TIME
                {
                    Task.active = false;
                }
                switch (Task.recurEvery)
                {
                    case "1"://Daily
                        {
                            if (Task.nextExecutionDate == null)// sino tiene siguiente fecha de ejecucion entonces asignar Start Date
                            {
                                //Task.nextExecutionDate = Task.fromDate;
                                Task.nextExecutionDate = Task.fromDate;
                            }
                            else// si ya tiene fecha asignar siguiente ejecucion segun las fechas de ejecucion
                            {
                                if (Task.permanent == true)//si es permanente
                                {
                                    if (Task.nextExecutionDate < currentDate) //si la tarea es menor a la fecha actual
                                    {
                                        Task.nextExecutionDate = Task.nextExecutionDate.Value.AddHours(24);//asignar ejecucion al siguiente día
                                    }
                                }
                            }
                            break;
                        }
                    case "2"://Weekly
                        {
                            var day = "";
                            if (Task.nextExecutionDate == null && Task.lastExecution == null)// primera asignacion de la tarea
                            {
                                currentDate = currentDate > Task.fromDate ? currentDate.AddDays(1) : currentDate;
                                while (Task.nextExecutionDate == null)//nextejectution
                                {
                                    day = currentDate.DayOfWeek.ToString().ToLower();
                                    var condition = Task.GetType().GetProperty(day) == null ? false : bool.Parse(Task.GetType().GetProperty(day).GetValue(Task, null).ToString());
                                    if (condition == true)
                                    {
                                        Task.nextExecutionDate = currentDate.Date + Task.fromDate.TimeOfDay;
                                    }
                                    currentDate = currentDate.AddDays(1);
                                }
                            }
                            else /*if(Task.nextExecutionDate != null)*///reasignacion de la tarea
                            {
                                //currentDate = currentDate > Task.nextExecutionDate ? currentDate : currentDate.AddDays(1); //solo se aumenta el dia en caso de que ya haya pasado la fecha de ejecucion
                                // while (Task.nextExecutionDate != currentDate + Task.fromDate.TimeOfDay) /*&& currentDate >= Task.fromDate*///nextejectution
                                while (currentDate > Task.nextExecutionDate)
                                {
                                    currentDate = currentDate.AddDays(1);
                                    day = currentDate.DayOfWeek.ToString().ToLower();
                                    var condition = Task.GetType().GetProperty(day) == null ? false : bool.Parse(Task.GetType().GetProperty(day).GetValue(Task, null).ToString());
                                    if (condition == true)
                                    {
                                        Task.nextExecutionDate = currentDate.Date + Task.fromDate.TimeOfDay;
                                        break;
                                    }
                                    currentDate = currentDate.AddDays(1);
                                }
                            }
                            break;
                        }
                    case "3"://montly//done
                        {
                            var numberDays = Task.numberDays.Remove(Task.numberDays.Length - 1);
                            var days = numberDays.Split(',').Select(x => int.Parse(x)).ToArray(); //numero de dias a programar                          
                            var months = Task.GetType().GetProperties().Select(x => x.Name).ToArray(); //meses indices 20-31                             
                            List<DateTime> MonthDates = new List<DateTime>();
                            var year = currentDate.Year;
                            var NextDate = DateTime.MinValue;
                            var mes = 1;
                            for (int y = 20; y <= 31; y++)
                            {
                                if (bool.Parse(Task.GetType().GetProperty(months[y]).GetValue(Task, null).ToString())) //si el mes esta registrado
                                {
                                    for (int x = 0; x < days.Count(); x++)
                                    {
                                        if (currentDate.Month <= mes && days[x] >= currentDate.Day)
                                            NextDate = new DateTime(currentDate.Year, mes, days[x]);
                                        else
                                            NextDate = new DateTime(currentDate.Year + 1, mes, days[x]);
                                        MonthDates.Add(NextDate);
                                    }
                                }
                                mes++;
                            }
                            NextDate = MonthDates.OrderBy(x => x).ThenBy(x => x.Day).ThenBy(x => x.Month).ThenBy(x => x.Year).FirstOrDefault(x => x > currentDate);
                            Task.nextExecutionDate = NextDate.Date + Task.fromDate.TimeOfDay;
                            break;
                        }
                    case "4"://one time
                        {
                            if (Task.toDate < currentDate)
                            {
                                Task.active = false;
                            }
                            else
                            {
                                Task.nextExecutionDate = Task.fromDate;
                                Task.toDate = Task.fromDate;
                            }
                            break;
                        }
                }
                db.SaveChanges();
                //GET MILISECONDS AND RUN
                // obtener cantidad de milisegundos para la siguiente ejecucion
                //ProgramingTask(run, url, taskID);// ejecutar tarea mas proxima
                //ADD TO LIST
                if (Task.active)
                {
                    int interval = (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds > 0 ? (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds : 0;
                    if (interval > 0)
                    {
                        if (timer != null)// si la tarea ya existe en la lista Actualizar
                        {
                            var currentTask = Tasks.FirstOrDefault(x => x == timer);
                            currentTask.Stop();
                            currentTask.Interval = interval;
                            currentTask.Elapsed += new ElapsedEventHandler((source, e) => response = runTask.RunTask(source, Task.url, Task.taskShedulerID));
                            currentTask.Enabled = true;
                            currentTask.AutoReset = false;//si es del tipo diarío entonces autoreset = true;
                            currentTask.Start();
                        }
                        else// agregar nueva tarea
                        {
                            System.Timers.Timer task = new System.Timers.Timer();
                            task.Interval = interval;
                            task.Elapsed += new ElapsedEventHandler((source, e) => response = runTask.RunTask(source, Task.url, Task.taskShedulerID));
                            task.Enabled = true;
                            task.AutoReset = false;//si es del tipo diarío entonces autoreset = true;
                            task.Start();
                            Tasks.Add(task);
                        }
                    }
                }
            }
        }

        public static void _NextTask()
        {
            ePlatEntities db = new ePlatEntities();
            int mili = 0;
            string url = "";
            int taskID = 0;
            DateTime currentDay = DateTime.Now; //hora para comparar siguiente tarea
            IQueryable<tblTaskScheduler> currentTaskList = Enumerable.Empty<tblTaskScheduler>().AsQueryable();

            var count = 0;
            while (currentTaskList.Count() == 0 && count < 8)// si no hay tareas asignadas aumentar 1 día 
            {
                currentTaskList = (from task in db.tblTaskScheduler
                                       // where (task.nextExecutionDate >= currentDay && currentDay <= task.nextExecutionDate) 
                                   where (task.lastExecution < currentDay || task.lastExecution == null) && task.nextExecutionDate > currentDay
                                          && task.active == true
                                   select task).OrderBy(x => x.nextExecutionDate);
                currentDay.AddDays(1);
                count++;
            }

            if (currentTaskList.Count() > 0)
            {
                var asig = currentTaskList.FirstOrDefault().nextExecutionDate.Value - currentDay;
                var taskTime = currentTaskList.FirstOrDefault().nextExecutionDate.Value;
                if (currentTaskList.FirstOrDefault().active == true)
                {
                    url = currentTaskList.FirstOrDefault().url;
                    taskID = currentTaskList.FirstOrDefault().taskShedulerID;
                    mili = (int)asig.TotalMilliseconds;// obtener cantidad de milisegundos para la siguiente ejecucion
                    //ProgramingTask(run, url, taskID);// ejecutar tarea mas proxima
                    //ADD TO LIST
                }
            }
            //TaskSchedulerDataModel runTask = new TaskSchedulerDataModel();
            //int response = 0;
            //if (mili > 0)// si la cantidad de milisegundos es menor o igual a 0 la tarea ya ha sido ejecutada
            //{
            //    System.Timers.Timer task = new System.Timers.Timer();
            //    task = new System.Timers.Timer();
            //    task.Interval = mili;
            //    task.Elapsed += new ElapsedEventHandler((source, e) => response = runTask.RunTask(source, taskUrl, taskID));
            //    task.Enabled = true;
            //    task.AutoReset = false;//si es del tipo diarío entonces autoreset = true;
            //    task.Start();
            //    Tasks.Add(task);
            //}
        }//NextTask

        //--
        public static List<TaskSchedulerViewModel.CurrentTask> TaskReport()
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<TaskSchedulerViewModel.CurrentTask>();

            List<tblTaskScheduler> TaskPool = new List<tblTaskScheduler>();
            //  TaskPool = TasksOneTime.Where(x => x.Value != null
            var ListPool = TasksPerHour.Select(x => x.Value.Keys.ToList()).ToList();
            ListPool.AddRange(TasksPerDay.Select(x => x.Value.Keys.ToList()).ToList());
            ListPool.AddRange(TasksPerWeek.Select(x => x.Value.Keys.ToList()).ToList());
            ListPool.AddRange(TasksPerMonth.Select(x => x.Value.Keys.ToList()).ToList());
            ListPool.AddRange(TasksOneTime.Select(x => x.Value.Keys.ToList()).ToList());

            foreach (var task in ListPool)
            {
                var d = task.Select(x => x).ToList();
                TaskPool.AddRange(d);
            }

            DateTime lastMonth = DateTime.Now.AddMonths(-1);
            var tasks = db.tblTaskScheduler.ToList();
            var events = db.tblTaskSchedulerEvents.Where(x => x.dateSaved >= lastMonth).ToList();

            return (from task in tasks
                    where task.taskShedulerID > 0 && task.active
                    select new TaskSchedulerViewModel.CurrentTask()
                    {
                        taskID = task.taskShedulerID,
                        description = task.description,
                        type = task.recurEvery == "1" ? "Daily" : task.recurEvery == "2" ? "Weekly" : task.recurEvery == "3" ? "Montly" : task.recurEvery == "4" ? "One Tme" : task.recurEvery == "5" ? "Per hour" : "",
                        interval = "",
                        lastExecution = task.lastExecution,
                        nextExecution = task.nextExecutionDate,
                        url = task.url,
                        active = TaskPool.Count(t => t.taskShedulerID == task.taskShedulerID) == 1 ? true : false,
                        events = (from ev in events
                                  where ev.taskShedulerID == task.taskShedulerID
                                  select new TaskSchedulerViewModel.TaskEvents()
                                  {
                                      taskEventID = ev.taskEventID,
                                      taskSchedulerID = ev.taskShedulerID,
                                      executionTime = ev.executionTime,
                                      urlResponse = ev.urlResponse,
                                      taskResponse = ev.taskResponse,
                                      taskTypeID = task.recurEvery,
                                      taskType = task.recurEvery == "1" ? "Daily" : task.recurEvery == "2" ? "Weekly" : task.recurEvery == "3" ? "Montly" : task.recurEvery == "4" ? "One Tme" : task.recurEvery == "5" ? "Per hour" : "",
                                      dateSaved = task.dateSaved,
                                  }).ToList()
                    }).ToList();
        }

            //List<TaskSchedulerViewModel.TaskEvents> events = (from ebent in TaskPool
            //                                                  join task in db.tblTaskSchedulerEvents on ebent.taskShedulerID equals task.taskShedulerID
            //                                                  where TasksModelList.Count(x => x.taskShedulerID == ebent.taskShedulerID) > 0
            //                                                  select new TaskSchedulerViewModel.TaskEvents() {
            //                                                      taskEventID = task.taskEventID,
            //                                                      taskSchedulerID = task.taskShedulerID,
            //                                                      executionTime = task.executionTime,
            //                                                      urlResponse = task.urlResponse,
            //                                                      taskResponse = task.taskResponse,
            //                                                      taskTypeID = task.taskTypeID,
            //                                                      taskType = task.taskTypeID == 1 ? "Daily" : task.taskTypeID == 2 ? "Weekly" : task.taskTypeID == 3 ? "Montly" : task.taskTypeID == 4 ? "One Tme" : task.taskTypeID == 5 ? "Per hour" : "",
            //                                                      dateSaved = task.dateSaved,
            //                                                  }).ToList();


            /* foreach (var item in TasksModelList)
             {
                 TaskSchedulerViewModel.CurrentTask currentTask = new TaskSchedulerViewModel.CurrentTask();
                 currentTask.taskID = item.taskShedulerID;
                 currentTask.description = item.description;
                 currentTask.type = item.recurEvery == "1" ? "Daily" : item.recurEvery == "2" ? "Weekly" : item.recurEvery == "3" ? "Montly" : item.recurEvery == "4" ? "One Tme" : item.recurEvery == "5" ? "Per hour" : "";
                 currentTask.interval = item.minutes.ToString();
                 currentTask.lastExecution = item.lastExecution.HasValue ? item.lastExecution.Value.ToString("yyyy-MM-dd hh:mm") : "";
                 currentTask.nextExecution = item.nextExecutionDate.HasValue ? item.nextExecutionDate.Value.ToString("yyyy-MM-dd hh:mm") : "";
                 currentTask.url = item.url;
                 currentTask.active = item.active;
                 currentTask.events = new List<TaskSchedulerViewModel.TaskEvents>();
                 currentTask.events.AddRange(events.Where(x => x.taskEventID == item.taskShedulerID));
                 list.lists.Add(currentTask);
             }*/

            //return TaskPool;
        

        //new model

        /*private static void DeleteCurrentTask(int TaskID)
        {
            TaskSchedulerDataModel tdm = new TaskSchedulerDataModel();
            TasksModelList.RemoveAll(x => x.taskShedulerID == TaskID);
            System.Timers.Timer currentTask = (System.Timers.Timer)tdm.GetSource(TaskID);
            Tasks.Remove(currentTask);
        } */

        private static bool DeleteTaskFromDictionary(int taskID, string dictionary)
        {
            switch (dictionary)
            {
                case "OneTime":
                    {
                        return TasksOneTime.ContainsKey(taskID) ? TasksOneTime.Remove(taskID) : true;
                    }
                case "Hour":
                    {
                        return TasksPerHour.ContainsKey(taskID) ? TasksPerHour.Remove(taskID) : true;
                    }
                case "Day":
                    {
                        return TasksPerDay.ContainsKey(taskID) ? TasksPerDay.Remove(taskID) : true;
                    }
                case "Week":
                    {
                        return TasksPerWeek.ContainsKey(taskID) ?  TasksPerWeek.Remove(taskID) : true;    
                    }
                case "Month":
                    {
                        return TasksPerMonth.ContainsKey(taskID) ? TasksPerMonth.Remove(taskID) : true;
                    }
            }
            return false;
        }
        
        public static void ProgramingTaskDaily(int taskID, object tasksource = null)//done
        {
            ePlatEntities db = new ePlatEntities();
            TaskSchedulerDataModel AssignTask = new TaskSchedulerDataModel();
            Dictionary<tblTaskScheduler, System.Timers.Timer> currentTask = new Dictionary<tblTaskScheduler, System.Timers.Timer>();
            var Task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);

            DateTime currentDate = DateTime.Now;
            int interval = (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds > 0 ? (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds : 0;
            int response = 0;

            if (Task.active == false)//Delete data from
            {
                if (TasksPerHour.Remove(Task.taskShedulerID))
                {
                    //TaskDeleted Success
                }
            }
            //First Time
            if (Task.lastExecution == null && Task.active)
            {
                System.Timers.Timer task = new System.Timers.Timer();
                task.Interval = interval;
               // task.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTaskDailyFirstTime(source, Task.url, Task.taskShedulerID));
                task.Enabled = true;
                task.AutoReset = false;
                task.Start();

                currentTask.Add(Task, task);
                TasksPerDay.Add(Task.taskShedulerID, currentTask);

               // Tasks.Add(task);
               // TasksModelList.Add(Task);
            }
            // Programing INFINTE LOOP
            if (Task.lastExecution != null && Task.nextExecutionDate != null && Task.active)
            {
                TasksPerDay.Remove(taskID);
                    Dictionary<tblTaskScheduler, System.Timers.Timer> currentTaskPerDay = new Dictionary<tblTaskScheduler, System.Timers.Timer>();
                    System.Timers.Timer task = new System.Timers.Timer();
                    task.Interval = interval;
                  //  task.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTaskDaily(source, Task.url, Task.taskShedulerID));
                    task.Enabled = true;
                    task.AutoReset = true;
                    task.Start();

                    currentTaskPerDay.Add(Task,task);
                    TasksPerDay.Add(Task.taskShedulerID, currentTaskPerDay);
                         
            }
        }

        public static void ProgramingTaskPerHour(int taskID, object tasksource = null)//done
        {
            ePlatEntities db = new ePlatEntities();
            Dictionary<tblTaskScheduler, System.Timers.Timer> currentTask = new Dictionary<tblTaskScheduler, System.Timers.Timer>();
            TaskSchedulerDataModel AssignTask = new TaskSchedulerDataModel();
            var Task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);

            DateTime currentDate = DateTime.Now;
            int interval = (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds > 0 ? (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds : 0;
            int response = 0;

            //eliminar de las listas 
            if (Task.active == false)//Delete data from
            {
                if(TasksPerHour.Remove(Task.taskShedulerID))
                {
                    //TaskDeleted Success
                }
            }
            //First Time
            if (Task.lastExecution == null && Task.active)
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = interval;
              //  timer.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTaskPerHourFirstTime(source, Task.url, Task.taskShedulerID));
                timer.Enabled = true;
                timer.AutoReset = false;
                timer.Start();

                currentTask.Add(Task, timer);
                TasksPerHour.Add(Task.taskShedulerID, currentTask);

               // Tasks.Add(task);
               // TasksModelList.Add(Task);
            }
            // Programing INFINTE LOOP -la tarea ya existe en el diccionario por lo que no es necesario volverla a crear
            if (Task.lastExecution != null && Task.active)
            {
                //delete old task;
                    TasksPerHour.Remove(taskID);
                    Dictionary<tblTaskScheduler, System.Timers.Timer> currentTaskPerHour = new Dictionary<tblTaskScheduler, System.Timers.Timer>();
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.Interval = TimeSpan.FromMinutes(Task.minutes.Value).TotalMilliseconds;
                //    timer.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTaskPerHour(source, Task.url, Task.taskShedulerID));
                    timer.Enabled = true;
                    timer.AutoReset = true;
                    timer.Start();
                    currentTaskPerHour.Add(Task,timer);
                    TasksPerHour.Add(Task.taskShedulerID, currentTaskPerHour); 
                
                //Tasks.Add(task);
                //TasksModelList.Add(Task);
            }
        }

        public static void ProgramingTaskOneTime(int taskID, object tasksource = null)//done
        {
            ePlatEntities db = new ePlatEntities();
            TaskSchedulerDataModel AssignTask = new TaskSchedulerDataModel();
            Dictionary<tblTaskScheduler, System.Timers.Timer> currentTask = new Dictionary<tblTaskScheduler, System.Timers.Timer>();
            var Task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);

            DateTime currentDate = DateTime.Now;
            int interval = (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds > 0 ? (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds : 0;
            int response = 0;

            //First Time
            if (Task.lastExecution == null && Task.active && interval > 0)
            {
                System.Timers.Timer task = new System.Timers.Timer();
                task.Interval = interval;
             //   task.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTaskOneTime(source, Task.url, Task.taskShedulerID));
                task.Enabled = true;
                task.AutoReset = false;//si es del tipo diarío entonces autoreset = true;
                task.Start();

                currentTask.Add(Task, task);
                TasksOneTime.Add(Task.taskShedulerID, currentTask);
               // Tasks.Add(task);
              //  TasksModelList.Add(Task);
            }
        }

        public static void ProgramingTaskMontly(int taskID, object tasksource = null)//done
        {
            ePlatEntities db = new ePlatEntities();
            TaskSchedulerDataModel AssignTask = new TaskSchedulerDataModel();
            Dictionary<tblTaskScheduler, System.Timers.Timer> currentTask = new Dictionary<tblTaskScheduler, System.Timers.Timer>();
            var Task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);

            DateTime currentDate = DateTime.Now;
            int interval = (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds > 0 ? (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds : 0;
            int response = 0;

            if (Task.active == false)//Delete data from
            {
                if (TasksPerMonth.Remove(Task.taskShedulerID))
                {
                    //TaskDeleted Success
                }
            }
            //First Time
            if (Task.lastExecution == null && Task.active)
            {
                System.Timers.Timer task = new System.Timers.Timer();
                task.Interval = interval;
            //    task.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTaskMontly(source, Task.url, Task.taskShedulerID));
                task.Enabled = true;
                task.AutoReset = false;
                task.Start();

                currentTask.Add(Task, task);
                TasksPerMonth.Add(Task.taskShedulerID, currentTask);

                // Tasks.Add(task);
                // TasksModelList.Add(Task);
            }
            // Programing INFINTE LOOP
            if (Task.lastExecution != null && Task.nextExecutionDate != null && Task.active)
            {
                TasksPerMonth.Remove(taskID);    

                    Dictionary<tblTaskScheduler, System.Timers.Timer> currentTaskPerDay = new Dictionary<tblTaskScheduler, System.Timers.Timer>();
                    System.Timers.Timer task = new System.Timers.Timer();
                    task.Interval = interval;
             //       task.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTaskMontly(source, Task.url, Task.taskShedulerID));
                    task.Enabled = true;
                    task.AutoReset = true;
                    task.Start();

                    currentTaskPerDay.Add(Task, task);
                    TasksPerMonth.Add(Task.taskShedulerID, currentTaskPerDay);
                
            }
        }

        public static void ProgramingTaskWeekly(int taskID, object tasksource = null)//done
        {
            ePlatEntities db = new ePlatEntities();
            TaskSchedulerDataModel AssignTask = new TaskSchedulerDataModel();
            Dictionary<tblTaskScheduler, System.Timers.Timer> currentTask = new Dictionary<tblTaskScheduler, System.Timers.Timer>();
            var Task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);

            DateTime currentDate = DateTime.Now;
            int interval = (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds > 0 ? (int)(Task.nextExecutionDate.Value - currentDate).TotalMilliseconds : 0;
            int response = 0;

            if (Task.active == false)//Delete data from
            {
                if (TasksPerWeek.Remove(Task.taskShedulerID))
                {
                    //TaskDeleted Success
                }
            }
            //First Time
            if (Task.lastExecution == null && Task.active)
            {
                System.Timers.Timer task = new System.Timers.Timer();
                task.Interval = interval;
              //  task.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTaskWeekly(source, Task.url, Task.taskShedulerID));
                task.Enabled = true;
                task.AutoReset = false;
                task.Start();

                currentTask.Add(Task, task);
                TasksPerWeek.Add(Task.taskShedulerID, currentTask);

                // Tasks.Add(task);
                // TasksModelList.Add(Task);
            }
            // Programing INFINTE LOOP
            if (Task.lastExecution != null && Task.nextExecutionDate != null && Task.active)
            {
                    TasksPerWeek.Remove(taskID);
                    Dictionary<tblTaskScheduler, System.Timers.Timer> currentTaskPerDay = new Dictionary<tblTaskScheduler, System.Timers.Timer>();
                    System.Timers.Timer task = new System.Timers.Timer();
                    task.Interval = interval;
             //       task.Elapsed += new ElapsedEventHandler((source, e) => response = AssignTask.RunTaskWeekly(source, Task.url, Task.taskShedulerID));
                    task.Enabled = true;
                    task.AutoReset = true;
                    task.Start();

                    currentTaskPerDay.Add(Task, task);
                    TasksPerWeek.Add(Task.taskShedulerID, currentTaskPerDay);
                
            }
        }

        //1.-daily
        //2.-week
        //3.-month
        //4.-one time
        //5.-many times at day

        //EXECUTE TASK FIST TIME
        /*
        public int RunTaskPerHourFirstTime(object source, string taskUrl, int taskID)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime startTarskTime = DateTime.Now;
            DateTime endTaskTime = new DateTime();
            System.Timers.Timer currentTask = (System.Timers.Timer)source;//get current task;
            AttemptResponse response = new AttemptResponse();
            tblTaskSchedulerEvents taskEvent = new tblTaskSchedulerEvents();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                var exist = db.tblTaskScheduler.FirstOrDefault(x => x.taskShedulerID == taskID) != null ? true : false;
                taskEvent.taskShedulerID = taskID;
                taskEvent.taskTypeID = 5; 
                //DETENER LOOP
                currentTask.Enabled = false;
                currentTask.Stop();
                //
                if (exist)
                {
                    var req = (HttpWebRequest)WebRequest.Create(taskUrl);
                    req.ContentType = "application/json";
                    req.Method = "POST";
                    req.Timeout = 600000; // =  10min 
                    req.ContentLength = 0;
                    var res = (HttpWebResponse)req.GetResponse();//obtener respuesta
                    Stream respuesta = res.GetResponseStream();// status = ok ? si se ejecuto : catch(ex)
                    TaskSchedulerViewModel.TaskResponse respFromServer = new TaskSchedulerViewModel.TaskResponse();
                    using (StreamReader reader = new StreamReader(respuesta))
                    {
                        respFromServer.response = reader.ReadToEnd();
                        respFromServer.date = DateTime.Now;
                    }
                    
                    response = js.Deserialize<AttemptResponse>(respFromServer.response);

                    endTaskTime = DateTime.Now;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = respFromServer.response;
                    taskEvent.taskResponse = "Task Was Executed Successfully";
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return 0;
                }
                else
                {                     
                    if (!TasksPerHour.ContainsKey(taskID))
                    {
                        DeleteTaskFromDictionary(taskID, "Hour");
                    }
                    endTaskTime = DateTime.Now;
                    taskEvent.taskShedulerID = taskID;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = "";
                    taskEvent.taskResponse = "Tarea no programada, esta tarea no existe en la base de datos al momento de ser ejecutada";
                    taskEvent.taskTypeID = 5;
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {

                taskEvent.taskShedulerID = taskID;
                taskEvent.executionTime = (DateTime.Now - startTarskTime).Milliseconds;
                taskEvent.urlResponse = "";
                taskEvent.taskResponse = "Excepcion al momento de recibir la respues o tiempo de ejecucion excedido - " + js.Serialize(ex);
                taskEvent.taskTypeID = 5;
                taskEvent.dateSaved = DateTime.Now;
                db.tblTaskSchedulerEvents.AddObject(taskEvent);
                db.SaveChanges();

                //QUITAR DE LISTA DE EJECUCION
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
                currentTask.Enabled = false;
                currentTask.Stop();
                DeleteTaskFromDictionary(taskID, "Hour");
                return 0;
            }
            finally
            {
                //remove from currents lists old task an programing loop
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
                task.nextExecutionDate = task.nextExecutionDate.Value.AddMinutes((double)task.minutes);
                task.lastExecution = startTarskTime;
                //STOP CURRENT TASK
                currentTask.Enabled = false;
                currentTask.Stop();
                //SAVE CHANGES
                db.SaveChanges();
                //PROGRAMING NEXT EXECUTION IN A INFINITE LOOP 
                ProgramingTaskPerHour(taskID);
            }
        }

        public int RunTaskDailyFirstTime(object source, string taskUrl, int taskID)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime startTarskTime = DateTime.Now;
            DateTime endTaskTime = new DateTime();
            System.Timers.Timer currentTask = (System.Timers.Timer)source;//get current task;
            AttemptResponse response = new AttemptResponse();
            tblTaskSchedulerEvents taskEvent = new tblTaskSchedulerEvents();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                var exist = db.tblTaskScheduler.FirstOrDefault(x => x.taskShedulerID == taskID) != null ? true : false;
                taskEvent.taskShedulerID = taskID;
                taskEvent.taskTypeID = 1; //Daily
                //DETENER LOOP
                currentTask.Enabled = false;
                currentTask.Stop();
                //
                var data = 0;
                if (exist)
                {
                    var req = (HttpWebRequest)WebRequest.Create(taskUrl);
                    req.ContentType = "application/json";
                    req.Method = "POST";
                    req.Timeout = 600000; // =  10min 
                    req.ContentLength = 0;
                    var res = (HttpWebResponse)req.GetResponse();//obtener respuesta
                    Stream respuesta = res.GetResponseStream();// status = ok ? si se ejecuto : catch(ex)
                    TaskSchedulerViewModel.TaskResponse respFromServer = new TaskSchedulerViewModel.TaskResponse();
                    using (StreamReader reader = new StreamReader(respuesta))
                    {
                        respFromServer.response = reader.ReadToEnd();
                        respFromServer.date = DateTime.Now;
                    }
                    
                    response = js.Deserialize<AttemptResponse>(respFromServer.response);

                    endTaskTime = DateTime.Now;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = respFromServer.response;
                    taskEvent.taskResponse = "Task Was Executed Success";
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return data;
                }
                else
                {
                    if (!TasksPerDay.ContainsKey(taskID))
                    {
                        DeleteTaskFromDictionary(taskID, "Day");
                    }
                    endTaskTime = DateTime.Now;
                    taskEvent.taskShedulerID = taskID;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = "";
                    taskEvent.taskResponse = "Tarea no programada, esta tarea no existe en la base de datos al momento de ser ejecutada";
                    taskEvent.taskTypeID = 1;
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                taskEvent.taskShedulerID = taskID;
                taskEvent.executionTime = (DateTime.Now - startTarskTime).Milliseconds;
                taskEvent.urlResponse = "";
                taskEvent.taskResponse = "Excepcion al momento de recibir la respues o tiempo de ejecucion excedido - " + js.Serialize(ex);
                taskEvent.taskTypeID = 1;
                taskEvent.dateSaved = DateTime.Now;
                db.tblTaskSchedulerEvents.AddObject(taskEvent);
                db.SaveChanges();

                //QUITAR DE LISTA DE EJECUCION
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
                currentTask.Enabled = false;
                currentTask.Stop();
                DeleteTaskFromDictionary(taskID, "Day");
                return 0;
            }
            finally
            {
                //remove from currents lists old task an programing loop
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
             //   task.nextExecutionDate = task.nextExecutionDate.Value.AddMinutes((double)task.minutes);
                task.lastExecution = startTarskTime;
                //STOP CURRENT TASK
                currentTask.Enabled = false;
                currentTask.Stop();
                //SAVE CHANGES
                db.SaveChanges();
                //PROGRAMING NEXT EXECUTION IN A INFINITE LOOP 
                ProgramingTaskDaily(taskID);
            }
        }

        //LOOP
        public int RunTaskPerHour(object source, string taskUrl, int taskID)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime startTarskTime = DateTime.Now;
            DateTime endTaskTime = new DateTime();
            System.Timers.Timer currentTask = (System.Timers.Timer)source;//get current task;
            AttemptResponse response = new AttemptResponse();
            tblTaskSchedulerEvents taskEvent = new tblTaskSchedulerEvents();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                var exist = db.tblTaskScheduler.FirstOrDefault(x => x.taskShedulerID == taskID) != null ? true : false;
                taskEvent.taskShedulerID = taskID;
                taskEvent.taskTypeID = 5; 
                var data = 0;
                if (exist)
                {
                    var req = (HttpWebRequest)WebRequest.Create(taskUrl);
                    req.ContentType = "application/json";
                    req.Method = "POST";
                    req.Timeout = 600000; // =  10min 
                    req.ContentLength = 0;
                    var res = (HttpWebResponse)req.GetResponse();//obtener respuesta
                    Stream respuesta = res.GetResponseStream();// status = ok ? si se ejecuto : catch(ex)
                    TaskSchedulerViewModel.TaskResponse respFromServer = new TaskSchedulerViewModel.TaskResponse();
                    using (StreamReader reader = new StreamReader(respuesta))
                    {
                        respFromServer.response = reader.ReadToEnd();
                        respFromServer.date = DateTime.Now;
                    }
                   
                    response = js.Deserialize<AttemptResponse>(respFromServer.response);

                   /* var respList = respFromServer.response.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\\", string.Empty)
                                   .Replace("\"", string.Empty).Split(',').Select(x => x).ToList();

                    endTaskTime = DateTime.Now;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = respFromServer.response;
                    taskEvent.taskResponse = "Task Was Executed Success";
                    taskEvent.dateSaved = respFromServer.date;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return data;
                }
                else 
                {
                    if (!TasksPerHour.ContainsKey(taskID))
                    {
                        DeleteTaskFromDictionary(taskID, "Hour");
                    }
                    endTaskTime = DateTime.Now;
                    taskEvent.taskShedulerID = taskID;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = "";
                    taskEvent.taskResponse = "Tarea no programada! Está tarea no existe en la base de datos al momento de ser ejecutada";
                    taskEvent.taskTypeID = 5;
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                endTaskTime = DateTime.Now;
                taskEvent.taskShedulerID = taskID;
                taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                taskEvent.urlResponse = "";
                taskEvent.taskResponse = "Excepcion al momento de recibir la respues o tiempo de ejecucion excedido - " + js.Serialize(ex);
                taskEvent.taskTypeID = 5;
                taskEvent.dateSaved = DateTime.Now;
                db.tblTaskSchedulerEvents.AddObject(taskEvent);
                db.SaveChanges();
                return 0;
            }
            finally
            {
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
                task.lastExecution = startTarskTime;
                task.nextExecutionDate = task.nextExecutionDate.Value.AddMinutes((double)task.minutes);
                db.SaveChanges();
            }
        }

        public int RunTaskDaily(object source, string taskUrl, int taskID)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime startTarskTime = DateTime.Now;
            DateTime endTaskTime = new DateTime();
            System.Timers.Timer currentTask = (System.Timers.Timer)source;//get current task;
            AttemptResponse response = new AttemptResponse();
            tblTaskSchedulerEvents taskEvent = new tblTaskSchedulerEvents();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var exist = db.tblTaskScheduler.FirstOrDefault(x => x.taskShedulerID == taskID) != null ? true : false;
                taskEvent.taskShedulerID = taskID;
                taskEvent.taskTypeID = 1; //daily
                var data = 0;
                if (exist)
                {
                    var req = (HttpWebRequest)WebRequest.Create(taskUrl);
                    req.ContentType = "application/json";
                    req.Method = "POST";
                    req.Timeout = 600000; // =  10min 
                    req.ContentLength = 0;
                    var res = (HttpWebResponse)req.GetResponse();//obtener respuesta
                    Stream respuesta = res.GetResponseStream();// status = ok ? si se ejecuto : catch(ex)
                    TaskSchedulerViewModel.TaskResponse respFromServer = new TaskSchedulerViewModel.TaskResponse();
                    using (StreamReader reader = new StreamReader(respuesta))
                    {
                        respFromServer.response = reader.ReadToEnd();
                        respFromServer.date = DateTime.Now;
                    }

                    
                    response = js.Deserialize<AttemptResponse>(respFromServer.response);
                    /*var respList = respFromServer.response.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\\", string.Empty)
                                   .Replace("\"", string.Empty).Split(',').Select(x => x).ToList();

                    endTaskTime = DateTime.Now;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = respFromServer.response;
                    taskEvent.taskResponse = "Task Was Executed Success";
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return data;
                }
                else
                {
                    if (!TasksPerHour.ContainsKey(taskID))
                    {
                        DeleteTaskFromDictionary(taskID, "Day");
                    }
                    endTaskTime = DateTime.Now;
                    taskEvent.taskShedulerID = taskID;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = "";
                    taskEvent.taskResponse = "Tarea no programada, esta tarea no existe en la base de datos al momento de ser ejecutada";
                    taskEvent.taskTypeID = 1;
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                endTaskTime = DateTime.Now;
                taskEvent.taskShedulerID = taskID;
                taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                taskEvent.urlResponse = "";
                taskEvent.taskResponse = "Excepcion al momento de recibir la respues o tiempo de ejecucion excedido - " + js.Serialize(ex);
                taskEvent.taskTypeID = 1;
                taskEvent.dateSaved = DateTime.Now;
                db.tblTaskSchedulerEvents.AddObject(taskEvent);
                db.SaveChanges();
                return 0;
            }
            finally
            {
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
                task.lastExecution = startTarskTime;
              //  task.nextExecutionDate = task.nextExecutionDate.Value.AddMinutes((double)task.minutes);
                db.SaveChanges();
            }

        }

        //ONE AT TIME
        public int RunTaskOneTime(object source, string taskUrl, int taskID)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime startTarskTime = DateTime.Now;
            DateTime endTaskTime = new DateTime();
            System.Timers.Timer currentTask = (System.Timers.Timer)source;//get current task;
            AttemptResponse response = new AttemptResponse();
            tblTaskSchedulerEvents taskEvent = new tblTaskSchedulerEvents();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var exist = db.tblTaskScheduler.FirstOrDefault(x => x.taskShedulerID == taskID) != null ? true : false;
                taskEvent.taskShedulerID = taskID;
                taskEvent.taskTypeID = 4; //one time
                var data = 0;
                if (exist)
                {
                    var req = (HttpWebRequest)WebRequest.Create(taskUrl);
                    req.ContentType = "application/json";
                    req.Method = "POST";
                    req.Timeout = 600000; // =  10min 
                    req.ContentLength = 0;
                    var res = (HttpWebResponse)req.GetResponse();//obtener respuesta
                    Stream respuesta = res.GetResponseStream();// status = ok ? si se ejecuto : catch(ex)
                    TaskSchedulerViewModel.TaskResponse respFromServer = new TaskSchedulerViewModel.TaskResponse();
                    using (StreamReader reader = new StreamReader(respuesta))
                    {
                        respFromServer.response = reader.ReadToEnd();
                        respFromServer.date = DateTime.Now;
                    }
                  
                    response = js.Deserialize<AttemptResponse>(respFromServer.response);

                    endTaskTime = DateTime.Now;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = respFromServer.response;
                    taskEvent.taskResponse = "Task Was Executed Success";
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return data;
                }
                else //delete from current task list
                {  //intervalo menor a 0 

                    endTaskTime = DateTime.Now;
                    taskEvent.taskShedulerID = taskID;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = "";
                    taskEvent.taskResponse = "Tarea no programada, esta tarea no existe en la base de datos al momento de ser ejecutada";
                    taskEvent.taskTypeID = 4;
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                if (!TasksOneTime.ContainsKey(taskID))
                {
                    DeleteTaskFromDictionary(taskID, "Day");
                }
                endTaskTime = DateTime.Now;
                taskEvent.taskShedulerID = taskID;
                taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                taskEvent.urlResponse = "";
                taskEvent.taskResponse = "Excepcion al momento de recibir la respues o tiempo de ejecucion excedido - " + ex.Message.ToString() + " Inner Ex: "+ ex.InnerException;
                taskEvent.taskTypeID = 4;
                taskEvent.dateSaved = DateTime.Now;
                db.tblTaskSchedulerEvents.AddObject(taskEvent);
                db.SaveChanges();
                return 0;
            }
            finally
            {
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
                task.active = false;
                task.lastExecution = startTarskTime;
                db.SaveChanges();
            }
        }

        public int RunTaskMontly(object source, string taskUrl, int taskID)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime startTarskTime = DateTime.Now;
            DateTime endTaskTime = new DateTime();
            System.Timers.Timer currentTask = (System.Timers.Timer)source;//get current task;
            AttemptResponse response = new AttemptResponse();
            tblTaskSchedulerEvents taskEvent = new tblTaskSchedulerEvents();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var exist = db.tblTaskScheduler.FirstOrDefault(x => x.taskShedulerID == taskID) != null ? true : false;
                taskEvent.taskShedulerID = taskID;
                taskEvent.taskTypeID = 3; 
                //DETENER LOOP
                currentTask.Enabled = false;
                currentTask.Stop();
                //
                var data = 0;
                if (exist)
                {
                    var req = (HttpWebRequest)WebRequest.Create(taskUrl);
                    req.ContentType = "application/json";
                    req.Method = "POST";
                    req.Timeout = 600000; // =  10min 
                    req.ContentLength = 0;
                    var res = (HttpWebResponse)req.GetResponse();//obtener respuesta
                    Stream respuesta = res.GetResponseStream();// status = ok ? si se ejecuto : catch(ex)
                    TaskSchedulerViewModel.TaskResponse respFromServer = new TaskSchedulerViewModel.TaskResponse();
                    using (StreamReader reader = new StreamReader(respuesta))
                    {
                        respFromServer.response = reader.ReadToEnd();
                        respFromServer.date = DateTime.Now;
                    }
                    response = js.Deserialize<AttemptResponse>(respFromServer.response);

                    endTaskTime = DateTime.Now;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = respFromServer.response;
                    taskEvent.taskResponse = "Task Was Executed Success";
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return data;
                }
                else
                {
                    if (!TasksPerDay.ContainsKey(taskID))
                    {
                        DeleteTaskFromDictionary(taskID, "Month");
                    }
                    endTaskTime = DateTime.Now;
                    taskEvent.taskShedulerID = taskID;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = "";
                    taskEvent.taskResponse = "Tarea no programada, esta tarea no existe en la base de datos al momento de ser ejecutada";
                    taskEvent.taskTypeID = 3;
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                taskEvent.taskShedulerID = taskID;
                taskEvent.executionTime = (DateTime.Now - startTarskTime).Milliseconds;
                taskEvent.urlResponse = "";
                taskEvent.taskResponse = "Excepcion al momento de recibir la respues o tiempo de ejecucion excedido - " + js.Serialize(ex);
                taskEvent.taskTypeID = 3;
                taskEvent.dateSaved = DateTime.Now;
                db.tblTaskSchedulerEvents.AddObject(taskEvent);
                db.SaveChanges();

                //QUITAR DE LISTA DE EJECUCION
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
                currentTask.Enabled = false;
                currentTask.Stop();
                DeleteTaskFromDictionary(taskID, "Month");
                return 0;
            }
            finally
            {
                //remove from currents lists old task an programing loop
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
             //   task.nextExecutionDate = task.nextExecutionDate.Value.AddMinutes((double)task.minutes);
                task.lastExecution = startTarskTime;
                //STOP CURRENT TASK
                currentTask.Enabled = false;
                currentTask.Stop();
                //SAVE CHANGES
                db.SaveChanges();
                GetRecurr(task.taskShedulerID);
                //ProgramingTaskMontly(taskID);
            }
        }

        public int RunTaskWeekly(object source, string taskUrl, int taskID)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime startTarskTime = DateTime.Now;
            DateTime endTaskTime = new DateTime();
            System.Timers.Timer currentTask = (System.Timers.Timer)source;//get current task;
            AttemptResponse response = new AttemptResponse();
            tblTaskSchedulerEvents taskEvent = new tblTaskSchedulerEvents();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var exist = db.tblTaskScheduler.FirstOrDefault(x => x.taskShedulerID == taskID) != null ? true : false;
                taskEvent.taskShedulerID = taskID;
                taskEvent.taskTypeID = 2; //Weekly
                //DETENER LOOP
                currentTask.Enabled = false;
                currentTask.Stop();
                //
                var data = 0;
                if (exist)
                {
                    var req = (HttpWebRequest)WebRequest.Create(taskUrl);
                    req.ContentType = "application/json";
                    req.Method = "POST";
                    req.Timeout = 600000; // =  10min 
                    req.ContentLength = 0;
                    var res = (HttpWebResponse)req.GetResponse();//obtener respuesta
                    Stream respuesta = res.GetResponseStream();// status = ok ? si se ejecuto : catch(ex)
                    TaskSchedulerViewModel.TaskResponse respFromServer = new TaskSchedulerViewModel.TaskResponse();
                    using (StreamReader reader = new StreamReader(respuesta))
                    {
                        respFromServer.response = reader.ReadToEnd();
                        respFromServer.date = DateTime.Now;
                    }
                    
                    response = js.Deserialize<AttemptResponse>(respFromServer.response);

                    endTaskTime = DateTime.Now;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = respFromServer.response;
                    taskEvent.taskResponse = "Task Was Executed Success";
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return data;
                }
                else
                {
                    if (!TasksPerDay.ContainsKey(taskID))
                    {
                        DeleteTaskFromDictionary(taskID, "Week");
                    }
                    endTaskTime = DateTime.Now;
                    taskEvent.taskShedulerID = taskID;
                    taskEvent.executionTime = (endTaskTime - startTarskTime).Milliseconds;
                    taskEvent.urlResponse = "";
                    taskEvent.taskResponse = "Tarea no programada, esta tarea no existe en la base de datos al momento de ser ejecutada";
                    taskEvent.taskTypeID = 2;
                    taskEvent.dateSaved = DateTime.Now;
                    db.tblTaskSchedulerEvents.AddObject(taskEvent);
                    db.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                taskEvent.taskShedulerID = taskID;
                taskEvent.executionTime = (DateTime.Now - startTarskTime).Milliseconds;
                taskEvent.urlResponse = "";
                taskEvent.taskResponse = "Excepcion al momento de recibir la respues o tiempo de ejecucion excedido - " + js.Serialize(ex);
                taskEvent.taskTypeID = 2;
                taskEvent.dateSaved = DateTime.Now;
                db.tblTaskSchedulerEvents.AddObject(taskEvent);
                db.SaveChanges();

                //QUITAR DE LISTA DE EJECUCION
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
                currentTask.Enabled = false;
                currentTask.Stop();
                DeleteTaskFromDictionary(taskID, "Week");
                return 0;
            }
            finally
            {
                //remove from currents lists old task an programing loop
                tblTaskScheduler task = db.tblTaskScheduler.Single(x => x.taskShedulerID == taskID);
             //   task.nextExecutionDate = task.nextExecutionDate.Value.AddMinutes((double)task.minutes);
                task.lastExecution = startTarskTime;
                //STOP CURRENT TASK
                currentTask.Enabled = false;
                currentTask.Stop();
                //SAVE CHANGES
                db.SaveChanges();
                GetRecurr(task.taskShedulerID);
                //ProgramingTaskMontly(taskID);
            }
        }
        */
    }
}
