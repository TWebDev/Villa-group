using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using System.ComponentModel.DataAnnotations;

namespace ePlatBack.Models.ViewModels
{
    public class SurveyViewModel
    {
        public class SurveyIndexModel
        {
            public SearchSurveyModel SearchSurveyModel { get; set; }

            public SurveyModel SurveyModel { get; set; }

            public bool AbleToModify { get; set; }
        }

        public class SurveyValuesModel
        {
            public Guid SurveyID { get; set; }
            public Guid TransactionID { get; set; }
            public List<FieldValue> Fields { get; set; }
        }

        public class FieldValue
        {
            public int FieldID { get; set; }
            public string Value { get; set; }
        }

        public class SearchSurveyModel
        {
            [Display(Name="Survey Name")]
            public string Search_Name { get; set; }
        }

        public class SurveyItem
        {
            public int FieldGroupID { get; set; }
            public string FieldGroup { get; set; }
            public string Url { get; set; }
            public string CreatedBy { get; set; }
            public string CreatedOn { get; set; }
        }

        public class SurveyModel
        {
            public int FieldGroupID { get; set; }
            public string SurveyName { get; set; }
            public Guid SurveyGuid { get; set; }
            public string Instructions { get; set; }
            public string Logo { get; set; }
            public List<SurveyFormField> FormFields { get; set; }
            public List<SurveyInfoField> InfoFields { get; set; }

            public SurveyInfoField SurveyInfoField { get; set; }
            public SurveyFormField SurveyFormField { get; set; }
        }

        public class SurveyFormField
        {
            public long F_FieldID { get; set; }
            public Guid F_TemporalID { get; set; }
            [Display(Name = "Question")]
            public string F_Question { get; set; }
            [Display(Name = "Field Name to be shown in Report")]
            public string F_FieldName { get; set; }
            [Display(Name = "Type")]
            public int F_FieldSubTypeID { get; set; }
            public List<SelectListItem> F_FieldSubTypes { get; set; }
            [Display(Name = "Visibility")]
            public int F_Visibility { get; set; }
            public List<SelectListItem> F_VisibilityOptions { get; set; }
            [Display(Name = "This field is Yes")]
            public Guid? F_VisibleIf { get; set; }
            public List<SelectListItem> F_VisibleIfFields
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [Display(Name = "Options")]
            public string F_Options { get; set; }
            public List<SurveyFormField> F_Fields { get; set; }
            [Display(Name = "Parent Field")]
            public string F_ParentFieldID { get; set; }
            public int F_Order { get; set; }
        }

        public class SurveyInfoField
        {
            public long I_FieldID { get; set; }
            public Guid I_TemporalID { get; set; }
            [Display(Name = "Visibility of Field")]
            public int I_Visibility { get; set; }
            public List<SelectListItem> I_VisibilityOptions
            { get; set; }
            [Display(Name = "Variable Name")]
            public string I_FieldName { get; set; }
            [Display(Name = "Field Type")]
            public int I_FieldSubTypeID { get; set; }
            public List<SelectListItem> I_FieldSubTypes { get; set; }
            [Display(Name = "Name to Display")]
            public string I_Question { get; set; }
        }

        public class SurveyInfoModel
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public Guid TransactionID { get; set; }
            public List<SurveyInfoValue> InfoFields { get; set; }
        }

        public class SurveyInfoValue
        {
            public string FieldName { get; set; }
            public string Value { get; set; }
        }

        public class StatsField
        {
            public string FieldName { get; set; }
            public List<SelectListItem> Values { get; set; }
        }

        public class StatParamItem
        {
            public string FieldName { get; set; }
            public string Value { get; set; }
        }

        public class SurveyStats
        {
            public int Sent { get; set; }
            public int Open { get; set; }
            public int Submitted { get; set; }
            public int ReferralsAmount { get; set; }
            public List<SurveyListItem> Surveys { get; set; }
        }

        public class SurveyListItem
        {
            public Guid TransactionID { get; set; }
            public List<SurveyFieldItem> Fields { get; set; }
        }

        public class SurveyFieldItem
        {
            public Guid FieldGuid { get; set; }
            public int? TypeID { get; set; }
            public int? SubTypeID { get; set; }
            public bool Publish { get; set; }
            public string FieldName { get; set; }
            public string Value { get; set; }
        }

        public class SurveyReferral
        {
            public string Referral { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string HomePhone { get; set; }
            public string ReferredBy { get; set; }
            public string Resort { get; set; }
            public string Stay { get; set; }
            public string SavedOn { get; set; }
            public string Terminal { get; set; }
        }

        public class JSONReferral
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string HomePhone { get; set; }
        }

        public class SurveyModelResponse
        {
            public string fileName { get; set; }
            public string subject { get; set; }
            public List<bool> surveyStatus { get; set; }
            public List<string> list { get; set; }
            public string error { get; set; }
            public bool status { get; set; }
        }

        public class SurveysTopics
        {
            public int SurveyID { get; set; }
            public string SurveyName { get; set; }
            public int TopicQuesID { get; set; }
            public string TopicQuesName { get; set; }
            public int QuestionID { get; set; }
            public string Question { get; set; }
        }
        
    }
}
