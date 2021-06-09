using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ePlatBack.Models.ViewModels
{
    public class TaskSchedulerViewModel
    {
        public SearchTasks SearchTask { get; set; }
        public SaveTaskModel SaveTask { get; set; }
        public GetTaskScheduler GetTaskModel { get; set; }
        public TasksResults TaskResult { get; set; }

        public class TasksResults
        {
            public List<GetTaskScheduler> TaskSummary { get; set; }
        }
        public class SearchTasks
        {
            [Display(Name = "From Date Saved")]
            public DateTime? SearchFromDate { get; set; }
          
            [Display(Name = "To Date Saved")]
            public DateTime? SearchToDate { get; set; }

          /*[Display(Name = "Permanent")]
            public bool SearchPermanent { get; set; }
            [Display(Name = "Active")]
            public bool SearchActive { get; set; }*/
        }
        public class SaveTaskModel
        {
            public int? TaskSchedulerID { get; set; }
            
            [Required]
            [Display(Name = "Description")]
            public string Description { get; set; }
            
            [Display(Name = "Permanent")]
            public bool Permanent { get; set; }
            [Required]
            [Display(Name = "Start Date")]
            public string FromDate { get; set; }
       
            [Display(Name = "End Date")]
            public string ToDate { get; set; }

            [Required]
            [Display(Name = "URL")]
            public string Url { get; set; }

            //Days           
            [Display(Name = "Monday")]
            public bool Monday { get; set; }         
            [Display(Name = "Thuesday")]
            public bool Tuesday { get; set; }           
            [Display(Name = "Wednesday")]
            public bool WednesDay { get; set; }       
            [Display(Name = "Thursday")]
            public bool Thursday { get; set; }         
            [Display(Name = "Friday")]
            public bool Friday { get; set; }           
            [Display(Name = "Saturday")]
            public bool Saturday { get; set; }            
            [Display(Name = "Sunday")]
            public bool Sunday { get; set; }
            [Display(Name = "January ")]
            public bool January { get; set; }
            [Display(Name = "February")]
            public bool February { get; set; }
            [Display(Name="March")]
            public bool March { get; set; }
            [Display(Name="April")]
            public bool April { get; set; }
            [Display(Name ="May")]
            public bool May { get; set; }
            [Display(Name="June")]
            public bool June { get; set; }
            [Display(Name="July")]
            public bool July { get; set; }
            [Display(Name="August")]
            public bool August { get; set; }
            [Display(Name="September")]
            public bool September { get; set; }
            [Display(Name="October")]
            public bool October { get; set; }
            [Display(Name="November")]
            public bool November { get; set; }
            [Display(Name="December")]
            public bool December { get; set; }
            //
            [Display(Name = "Recur")]
            public string Lapse { get; set; }
            [Display(Name = "Many times at day")]
            public bool atDay { get; set; }
            [Display(Name = "Each time at day")]
            public long minutes { get; set; }
            public List<SelectListItem>DropDown_Lapse
            {
                get
                {
                    List<SelectListItem> Items = new List<SelectListItem>();
                    Items.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "-Select One-"
                    });                   
                    Items.Add(new SelectListItem()
                    {
                        Value = "1",
                        Text = "Daily"
                    });                    
                    Items.Add(new SelectListItem()
                    {
                        Value = "2",
                        Text = "Some Days Week"
                    });
                    Items.Add(new SelectListItem()
                    {
                        Value = "3",
                        Text = "Some Days Month"
                    });
                    Items.Add(new SelectListItem()
                    {
                        Value = "4",
                        Text = "One Time"
                    });
                    Items.Add(new SelectListItem()
                    {
                        Value = "5",
                        Text = "Many Times at Day"
                    });
                    return Items;
                }
            }
            public string[] NumberDays {get;set;}
            [Display(Name = "Select Days")]
            public List<SelectListItem> DropDown_NumberDays
            {
                get
                {
                    List<SelectListItem> Days = new List<SelectListItem>();
                    for(int x=1; x<=31; x++)
                    {
                        Days.Add(new SelectListItem()
                        {
                            Value = x.ToString(),
                            Text = x.ToString()
                        });
                    }
                    return Days;
                }
            }
        }
        public class GetTaskScheduler
        {
            public int GetTaskSchedulerID { get; set; }
            public string GetDescription { get; set; }
            public bool GetPermanent { get; set; }
            public string GetFromDate { get; set; }
            public string GetToDate { get; set; }
            public bool GetActive { get; set; }
            public string GetUrl { get; set; }
            public string GetRecur { get; set; }
            public string GetSaveByUser { get; set; }
            public DateTime GetDateSaved { get; set; }

            public string GetNumberDays { get; set; }
            //recur = 2 semanal
            public bool GetMonday { get; set; }
            public bool GetTuesday { get; set; }
            public bool GetWednesday { get; set; }
            public bool GetThursday   { get; set; }
            public bool GetFriday { get; set; }
            public bool GetSaturday { get; set; }
            public bool  GetSunday { get; set; }
            //recur = 3 Mensual
            public bool GetJanuary { get; set; }
            public bool GetFebruary{ get; set; }
            public bool GetMarch { get; set; }
            public bool GetApril { get; set; }
            public bool GetMay { get; set; }
            public bool GetJune { get; set; }
            public bool GetJuly { get; set; }
            public bool GetAugust { get; set; }
            public bool GetSeptember { get; set; }
            public bool GetOctober { get; set; }
            public bool GetNovember { get; set; }
            public bool GetDecember { get; set; }           
        }
        public class TaskResponse
        {
            public string response { get; set; }

            public DateTime date { get; set; }
        }
        public class TaskReport
        {
            public List<CurrentTask> lists { get; set; }
        }

        public class CurrentTask
        {
            public int taskID { get; set; }
            public string description { get; set; }
            public string type { get; set; }
            public string interval { get; set; }
            public DateTime? lastExecution { get; set; }
            public DateTime? nextExecution { get; set; }
            public string url { get; set; }
            public bool active { get; set; }

            public List<TaskEvents> events { get; set; }
        }

        public class TaskEvents
        {
            public int taskEventID { get; set; }
            public int taskSchedulerID { get; set; }
            public int executionTime { get; set; }
            public string urlResponse { get; set; }
            public string taskResponse { get; set; }
            public string taskTypeID { get; set; }
            public string taskType { get; set; }
            public DateTime dateSaved { get; set; }
        }

    }
}
