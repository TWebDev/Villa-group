#region Assembly System.Web.Mvc.dll, v4.0.0.0

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{

    //[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    //public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    //{
    //    private const string defaultErrorMessage = "{0} is required";

    //    public string OtherProperty;

    //    public RequiredIfAttribute(string OtherProperty)
    //        : base(defaultErrorMessage)
    //    {
    //        if (string.IsNullOrEmpty(OtherProperty))
    //            throw new ArgumentNullException("OtherProperty");
    //        this.OtherProperty = OtherProperty;
    //    }
    //    //public RequiredIf(string OtherProperty)
    //    //    : base(defaultErrorMessage)
    //    //{
    //    //    if (string.IsNullOrEmpty(OtherProperty))
    //    //        throw new ArgumentNullException("otherProperty");
    //    //    this.OtherProperty = OtherProperty;
    //    //}

    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, name, OtherProperty);
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        PropertyInfo otherPropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);
    //        if (OtherProperty.Length > 0)
    //        {
    //            if (value != null && value.ToString().Length > 0)
    //                return ValidationResult.Success;
    //            else
    //                //return new ValidationResult(OtherProperty + " is Missing");
    //                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
    //        }
    //        else
    //            return ValidationResult.Success;
    //    }

    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        yield return new ModelClientValidationRule
    //        {
    //            ErrorMessage = FormatErrorMessage(metadata.DisplayName),
    //            ValidationType = "requiredif"
    //        };
    //    }
    //    public class ModelClientValidationSelectOneRule : ModelClientValidationRule
    //    {
    //        public ModelClientValidationSelectOneRule
    //    (string errorMessage, string[] strProperties)
    //        {
    //            ErrorMessage = errorMessage;
    //            ValidationType = "requiredif";
    //            for (int intIndex = 0; intIndex < strProperties.Length; intIndex++)
    //            {
    //                ValidationParameters.Add("otherproperty" +
    //                intIndex.ToString(), strProperties[intIndex]);
    //            }
    //        }
    //    }

    //}

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class ArrayMinLengthAttribute : ValidationAttribute, IClientValidatable
    {
        public int minLength = 1;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as IList<object>;
            if (list != null && list.Count > minLength)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName)); ;
        }

        public IEnumerable<ModelClientValidationRule>
        GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //    return new[] { new ModelClientValidationSelectOneRule
            //    (FormatErrorMessage(metadata.DisplayName), 
            //CommaSeperatedProperties.Split(new char[] { ',' })) };
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "arrayminlength"
            };
        }
        public class ModelClientValidationSelectOneRule : ModelClientValidationRule
        {
            public ModelClientValidationSelectOneRule
        (string errorMessage, string[] strProperties)
            {
                ErrorMessage = errorMessage;
                ValidationType = "arrayminlength";
                for (int intIndex = 0; intIndex < strProperties.Length; intIndex++)
                {
                    ValidationParameters.Add(intIndex.ToString(), strProperties[intIndex]);
                }
            }
        }
        //public override bool IsValid(object value)
        //{
        //    var list = value as IList<object>;
        //    if (list != null && list.Count > minLength)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }

    /// <summary>
    /// It defines the property as required, only if element's value satisfies the condition.
    /// string OtherProperty: Name of property to evaluate. 
    /// object EqualsTo: nullable value to compare.
    /// </summary>
    public class RequiredIf : ValidationAttribute, IClientValidatable
    {
        private const string defaultErrorMessage = "{0} is required";
        public string OtherProperty;
        public object EqualsTo = null;
        public string Property;
        //public object DistinctOf = null;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //var _value = Convert.ChangeType(EqualsTo, typeof(bool));
            if (OtherProperty != null)
            {
                var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);
                var otherValue = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty).GetValue(validationContext.ObjectInstance, null);
                var type = otherValue != null ? otherValue.GetType() : value != null ? value.GetType() : typeof(string);
                //-----
                //var targetType = EqualsTo.GetType().BaseType.Name.IndexOf("Array") != -1;
                var isArray = EqualsTo != null ? EqualsTo.ToString().IndexOf(",") > -1 : false;
                //var _value = !isArray ? Convert.ChangeType(EqualsTo, type) : ((Array)EqualsTo);
                object _value;
                if (isArray)
                {
                    _value = EqualsTo.ToString().Split(',');
                    
                    for(var x= 0; x < ((Array)_value).Length; x++)
                    {
                        var a = otherValue;
                        var c = ((Array)_value).GetValue(x);
                        var b = Convert.ChangeType(((Array)_value).GetValue(x), otherValue.GetType());
                        if (object.Equals(otherValue, Convert.ChangeType(((Array)_value).GetValue(x), otherValue.GetType())))
                        {
                            if (value == null)
                            {
                                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                            }
                        }
                    }
                }
                else
                {
                    _value = Convert.ChangeType(EqualsTo, type);
                    if (object.Equals(otherValue, _value))
                    {
                        if (value == null)
                        {
                            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
                //-----
                //var _value = Convert.ChangeType(EqualsTo, type);

                //if (object.Equals(otherValue, _value))
                //if (object.Equals(otherValue, _value))
                //{
                //    if (value == null)
                //    {
                //        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                //    }
                //}
            }
            else
            {
                var privileges = validationContext.ObjectInstance.GetType().GetProperty("_privileges").GetValue(validationContext.ObjectInstance, null) as List<ePlatBack.Models.ViewModels.SysComponentsPrivilegesModel>;
                var _privilege = privileges != null ? privileges.Exists(m => m.Component == Property) ? privileges.Find(m => m.Component == Property).Required : false : false;
                
                if (_privilege == true)
                {
                    if (value == null || value.Equals(0))
                    {
                        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationParameters.Add("otherproperty", OtherProperty);
            rule.ValidationParameters.Add("equalsto", EqualsTo);
            //rule.ValidationParameters.Add("equalsto", EqualsTo.GetType().IsArray? string.Join(",",EqualsTo) : EqualsTo);
            //rule.ValidationParameters.Add("distinctof", DistinctOf);
            rule.ValidationType = "requiredif";
            yield return rule;
        }
    }

    public class RequiredIfDistinct : ValidationAttribute, IClientValidatable
    {
        private const string defaultErrorMessage = "{0} is required";
        public string OtherProperty;
        //public object EqualsTo = null;
        public object DistinctOf = null;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);
            var otherValue = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty).GetValue(validationContext.ObjectInstance, null);
            var type = otherValue != null ? otherValue.GetType() : value != null ? value.GetType() : typeof(string);
            //var _value = Convert.ChangeType(EqualsTo, type);
            //var _value = DistinctOf == null ? Convert.ChangeType(EqualsTo, type) : Convert.ChangeType(DistinctOf, type);
            var _value = Convert.ChangeType(DistinctOf, type);

            //if (DistinctOf == null)
            //{
            //if (object.Equals(otherValue, _value))
            //{
            //    if (value == null)
            //    {
            //        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            //    }
            //}
            //}
            //if (DistinctOf != null && EqualsTo == null)
            //{
            if (!object.Equals(otherValue, _value))
            {
                if (value == null)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
            }
            //}
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationParameters.Add("otherproperty", OtherProperty);
            //rule.ValidationParameters.Add("equalsto", EqualsTo);
            rule.ValidationParameters.Add("distinctof", DistinctOf);
            rule.ValidationType = "requiredifdistinct";
            yield return rule;
        }
    }

    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            base.OnActionExecuting(filterContext);
        }
    }
}