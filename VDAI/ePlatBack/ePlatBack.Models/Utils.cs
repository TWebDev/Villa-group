
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models.Utils.Custom.Attributes;
using ePlatBack.Models.Utils.Custom;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime;
using ePlatBack.Models.ViewModels;
using System.Globalization;
using System.Data.Objects.DataClasses;
using ePlatBack.Models.DataModels;
using System.Threading;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.CodeDom;
using RestSharp;
using Newtonsoft.Json;

namespace ePlatBack.Models.Utils
{
    namespace Custom
    {
        namespace Attributes
        {
            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class ListTypeAttribute : System.Attribute
            {
                public ListTypeAttribute(Type itemsType)
                { this.singleItemType = itemsType; }

                private Type singleItemType;
                public Type SingleItemType { get { return singleItemType; } }

                public Type Type
                {
                    get { return typeof(List<>).MakeGenericType(singleItemType); }
                }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class IsPrimaryKey : System.Attribute
            {
                public IsPrimaryKey()
                { }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class AutomaticValueAttribute : System.Attribute
            {
                private AutomaticValueTypes automaticValueType;
                private string valuePath;

                public string ValuePath
                {

                    set { valuePath = value; }
                    get { return valuePath; }
                }

                public AutomaticValueTypes AutomaticValueType
                {
                    set { automaticValueType = value; }
                    get { return automaticValueType; }
                }



            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class GetValueFromAttribute : System.Attribute
            {
                private string valuePath;

                public string ValuePath
                {

                    set { valuePath = value; }
                    get { return valuePath; }
                }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class DataBaseInfoAttribute : System.Attribute
            {


                private Type dbType;
                private string dbName;
                private bool isRelationShip;
                private string primaryKeyName;
                private string foreignKeyName;
                private string primaryKeyDatabaseName;
                private string primaryKeyFieldName;
                private string primaryKeyTableName;
                private string foreignKeyFieldName;
                private string foreignKeyTableName;
                private DataBaseRelationShipCardinality cardinality;


                public Type RelationShipType
                {
                    set { dbType = value; }
                    get { return dbType; }
                }

                public string PrimaryKeyFieldName
                {
                    get { return primaryKeyFieldName; }
                    set { primaryKeyFieldName = value; }
                }

                public string PrimaryKeyTableName
                {
                    get { return primaryKeyTableName; }
                    set { primaryKeyTableName = value; }
                }

                public string ForeignKeyFieldName
                {
                    get { return foreignKeyFieldName; }
                    set { foreignKeyFieldName = value; }
                }

                public string ForeignKeyTableName
                {
                    get { return foreignKeyTableName; }
                    set { foreignKeyTableName = value; }
                }

                public string PrimaryKeyModelName
                {
                    set { primaryKeyName = value; }
                    get { return primaryKeyName; }
                }

                public string ForeignKeyModelName
                {
                    set { foreignKeyName = value; }
                    get { return foreignKeyName; }
                }

                public string PrimaryKeyDatabaseName
                {
                    set { primaryKeyDatabaseName = value; }
                    get { return primaryKeyDatabaseName; }
                }

                public string Name
                {
                    set { dbName = value; }
                    get { return dbName; }
                }

                public bool IsRelationShip
                {
                    set { isRelationShip = value; }
                    get { return isRelationShip; }
                }

                public DataBaseRelationShipCardinality Cardinality
                {
                    set { cardinality = value; }
                    get { return cardinality; }
                }

                public DataBaseInfoAttribute() { }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class FieldInfoAttribute : System.Attribute
            {
                private Type dbType;
                private string dbName;

                public string Name
                {
                    set { dbName = value; }
                    get { return dbName; }
                }

                public FieldInfoAttribute() { }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class FieldToRequestAttribute : System.Attribute
            {
                private Type dbType;
                private string dbName;

                public string Name
                {
                    set { dbName = value; }
                    get { return dbName; }
                }

                public FieldToRequestAttribute() { }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class AllowUpdateAttribute : System.Attribute
            {
                private bool allowUpdate;
                public bool AllowUpdate { get { return allowUpdate; } }
                public AllowUpdateAttribute(bool allowUpdate)
                { this.allowUpdate = allowUpdate; }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class DoNotUpdateAttribute : System.Attribute
            {
                public DoNotUpdateAttribute()
                { }
            }

            /// <summary>
            /// Any field with this attribute will be used as reference in the Logs
            /// </summary>
            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class LogReferenceAttribute : System.Attribute
            {
                private string name;
                public string Name { get { return name; } set { name = value; } }
                public LogReferenceAttribute() { }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class DoNotTrackChangesAttribute : System.Attribute
            {
                public DoNotTrackChangesAttribute()
                { }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class ParseBackToObjectAttribute : System.Attribute
            {
                public ParseBackToObjectAttribute()
                { }
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class OnValueChangedAttribute : System.Attribute
            {
                private OnValueChangedActions _action;
                private object _value;
                private string _target;
                private AutomaticValueTypes _ValueType;

                public OnValueChangedActions Action
                {
                    set { _action = value; }
                    get { return _action; }
                }

                public object Value
                {
                    get { return _value; }
                    set { _value = value; }
                }

                public string Target
                {
                    get { return _target; }
                    set { _target = value; }
                }

                public AutomaticValueTypes ValueType
                {
                    get { return _ValueType; }
                    set { _ValueType = value; }
                }



                public OnValueChangedAttribute() { }
            }
        }

        public static class Reflection
        {
            public static PropertyInfo[] GetPropertiesByAtribute<T>(object obj)
            {
                object properties;


                properties = obj.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(T))).ToArray<System.Reflection.PropertyInfo>();


                return properties as PropertyInfo[];


            }
            public static PropertyInfo GetPropertyByAtribute<T>(object obj)
            {
                return (obj.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(T)))).First();

            }
            public static PropertyInfo[] GetPropertiesWithoutAtribute<T>(object obj)
            {
                var properties = obj.GetType().GetProperties().Where(prop => !Attribute.IsDefined(prop, typeof(T))).Select(x => x as PropertyInfo);
                PropertyInfo[] propertiesArr = properties.Cast<PropertyInfo>().ToArray(); ;
                return propertiesArr;

            }
            public static PropertyInfo GetPropertyByName(object obj, string propertyName)
            {
                return obj.GetType().GetProperty(propertyName);

            }
            public static T GetCustomAttribute<T>(PropertyInfo property, int index = 0)
            {

                return (T)property.GetCustomAttributes(typeof(T), true)[index];
            }

            public static T GetCustomAttribute<T>(object obj, int index = 0)
            {
                Type objType = obj.GetType();

                //para el siguiente c
                dynamic sourceObjectDbInfo = Attribute.GetCustomAttribute(objType, typeof(T));

                return (T)sourceObjectDbInfo;

            }

            /// <summary>
            /// If the PropertyInfo has DataBaseInfoAttribute then return the Name stored in the Attribute, 
            /// else returns the property Name.
            /// </summary>
            /// <param name="p">The PropertyInfo to check on.</param>
            /// <returns></returns>
            public static string GetDbNameOrDefault(PropertyInfo p)
            {
                //// in case there is no dbInfoAttr then the name = property name assuming the UI property has the same name as db property
                if (HasCustomAttribute<DataBaseInfoAttribute>(p))
                {
                    return GetCustomAttribute<DataBaseInfoAttribute>(p).Name;
                }
                else
                {
                    return p.Name;
                }
            }

            /// <summary>
            /// Indicates if the given value is a value set by the DataBase.
            /// </summary>
            /// <param name="value">The value to check on.</param>
            /// <returns></returns>
            public static bool HasDbIdentity(Guid value)
            {
                bool hasIdentity;
                if (value == Guid.Empty) { hasIdentity = false; }
                else { hasIdentity = true; }
                return hasIdentity;
            }
            /// <summary>
            /// Indicates if the given value is a value set by the DataBase.
            /// </summary>
            /// <param name="value">The value to check on.</param>
            /// <returns></returns>
            public static bool HasDbIdentity(int value)
            {
                bool hasIdentity;
                if (value <= 0) { hasIdentity = false; }
                else { hasIdentity = true; }
                return hasIdentity;
            }

            public static bool HasDbIdentity(long value)
            {
                bool hasIdentity;
                if (value <= 0) { hasIdentity = false; }
                else { hasIdentity = true; }
                return hasIdentity;
            }
            /// <summary>
            /// Indicates if the given PrimaryKey property has a value set by the DataBase.
            /// </summary>
            /// <param name="property">The primary key to check Identity on.</param>
            /// <returns></returns>
            public static bool HasDbIdentity(PropertyInfo property)
            {

                bool hasIdentity = true;
                Type propertyType = property.PropertyType;
                Type intType = typeof(int);
                Type intTypeNullable = typeof(int?);
                Type longType = typeof(long);
                Type longTypeNullable = typeof(long?);
                Type guidType = typeof(Guid);
                Type guidTypeNullable = typeof(Guid?);


                if (propertyType == guidTypeNullable)
                {
                    //Convert.ChangeType(val, guidTypeNullable);
                    Guid propertyValue = (Guid)property.GetValue(property, null);
                    // hasIdentity=(propertyValue == Guid.Empty);
                    hasIdentity = HasDbIdentity(propertyValue);

                }
                else if (propertyType == intTypeNullable || propertyType == longTypeNullable)
                {
                    int propertyValue = (int)property.GetValue(property, null);
                    //hasIdentity=(propertyValue <= 0);
                    hasIdentity = HasDbIdentity(propertyValue);
                }

                return hasIdentity;
            }

            public static bool HasCustomAttribute<T>(PropertyInfo p)
            {

                bool hasAttribute = false;
                if (Attribute.IsDefined(p, typeof(T)))
                { hasAttribute = true; }

                return hasAttribute;
            }

            public static void setUpListItem(object sourceObject, PropertyInfo p, ref object returnObject)
            {



            }

            public static object GetPropertyValueByPath(object obj, string path)
            {

                try
                {
                    Type currentType = obj.GetType();
                    char[] charsToTrim = { '*', ' ', '\'', '.' };
                    foreach (string propertyName in path.Trim(charsToTrim).Split('.'))
                    {
                        string[] propertyParts = propertyName.Trim(charsToTrim).Split('|');

                        if (propertyParts.Length > 1)
                        {
                            PropertyInfo property = currentType.GetProperty(propertyParts[0]);

                            /////
                            IEnumerable items = (IEnumerable)property.GetValue(obj, null);
                            object first = items.Cast<object>().FirstOrDefault();
                            //////



                            obj = first;
                            currentType = first.GetType();
                        }
                        else
                        {
                            PropertyInfo property = currentType.GetProperty(propertyName);
                            obj = property.GetValue(obj, null);
                            currentType = property.PropertyType;
                        }


                    }
                    return obj;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }

            public static void SetPropertyValue(object obj, string propertyName, object value)
            {
                PropertyInfo p = obj.GetType().GetProperty(propertyName);
                Type t = Nullable.GetUnderlyingType(p.PropertyType)
                 ?? p.PropertyType;
                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                p.SetValue(obj, safeValue, null);
            }

            public static string DateReverter(Match m)
            {
                var newValue = "";
                if (m.ToString().Contains("/Date("))
                {
                    string strTicks = m.ToString().Split(new char[] { '(', ')' })[1];
                    DateTime veriOldDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    long ticks = long.Parse(strTicks) * 10000 + veriOldDate.Ticks;
                    DateTime theDate = new DateTime(ticks);
                    newValue = theDate.ToString();
                    ///pi.SetValue(jsonPropertyValue, newValue);
                }

                return newValue;
            }

            static void ParseDateTypeBack(this PropertyInfo date, ref object parsingObject)
            {
                object serializedDate = date.GetValue(parsingObject, null);
                if (serializedDate != null && serializedDate.ToString().Contains("/Date"))
                {
                    string propertyName = date.Name;
                    PropertyInfo uiProperty = parsingObject.GetType().GetProperty(propertyName);
                    string[] uiList = (string[])GetPropertyValueByPath(parsingObject, propertyName);
                    //Make sure things like /Date(1370626189827)/  (including the slashes)  are replaced by its serverSide valid date
                    var value = Regex.Replace(serializedDate.ToString(), @"\/Date\(\d*\)/", new MatchEvaluator(DateReverter));
                    parsingObject.GetType().GetProperty(propertyName).SetValue(parsingObject, value, null);
                }
            }

            static void ParseListTypeBack(this PropertyInfo list, ref object parsingObject)
            {
                string[] listValue = (string[])list.GetValue(parsingObject, null);

                if (listValue == null)
                {
                    return;
                }

                listValue[0] = Regex.Replace(listValue[0], @"\/Date\(\d*\)/", new MatchEvaluator(DateReverter));
                parsingObject.GetType().GetProperty(list.Name).SetValue(parsingObject, listValue, null);


                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();

                //Type attribute = typeof(ListTypeAttribute);
                //object[] attributes = list.GetType().GetCustomAttributes(attribute, true);


                ListTypeAttribute da = Reflection.GetCustomAttribute<ListTypeAttribute>(list);
                //ListTypeAttribute da = attributes[0] as ListTypeAttribute;
                // instead of (ListTypeAttribute)attributes[0]
                PropertyInfo uiProperty = parsingObject.GetType().GetProperty(list.Name);
                //string[] uiList = (string[])uiProperty.GetValue(parsingObject);
                string[] uiList = (string[])GetPropertyValueByPath(parsingObject, list.Name);

                var value = jss.Deserialize(uiList[0], da.Type);
                //parsingObject.GetType().GetProperty(uiProperty.Name).SetValue(lgim, value);
                parsingObject.GetType().GetProperty(uiProperty.Name).SetValue(parsingObject, value, null);


                //return new object();
            }

            public static void ParsePropertiesBackToObjects(this object parsingObject)
            {
                //obtener las propiedades que tengan [ParseBackToObject] Attribute
                //Type attribute = typeof(ParseBackToObjectAttribute);

                //var props = parsingObject.GetType().GetProperties();
                //DataBaseInfoAttribute
                var props = Reflection.GetPropertiesByAtribute<DataBaseInfoAttribute>(parsingObject);
                //PropertyInfo[] parsingBackProperties = props.Where(prop => Attribute.IsDefined(prop, attribute)).ToArray();

                foreach (var p in props)
                {
                    DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);

                    if (dbInfo.IsRelationShip && Reflection.HasCustomAttribute<ListTypeAttribute>(p))
                    {

                        p.ParseListTypeBack(ref parsingObject);

                    }
                    else if (dbInfo.IsRelationShip)
                    {
                        //p.ParsePropertiesBackToObjects();                    
                        p.GetValue(parsingObject, null).ParsePropertiesBackToObjects();
                    }
                    else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))  // dates
                    {
                        p.ParseDateTypeBack(ref parsingObject);
                    }
                    //else : no need to parse back


                }

            }

            public static void FillWith(this object uiObject, object sourceObject)
            {
                foreach (var p in uiObject.GetType().GetProperties())
                {
                    bool hasDbInfo = false;
                    DataBaseInfoAttribute dbi = null;
                    if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(p))
                    {
                        dbi = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
                        hasDbInfo = true;
                    }
                    if (hasDbInfo && dbi.IsRelationShip == true && Reflection.HasCustomAttribute<ListTypeAttribute>(p)) //OneToMany
                    {
                        var dbName = dbi.RelationShipType.Name;
                        IEnumerable<object> dbValue = Reflection.GetPropertyValueByPath(sourceObject, dbName) as IEnumerable<object>;
                        ListTypeAttribute uiListType = Reflection.GetCustomAttribute<ListTypeAttribute>(p);
                        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(uiListType.SingleItemType));

                        foreach (var li in dbValue)
                        {
                            var newLi = Activator.CreateInstance(uiListType.SingleItemType);
                            foreach (var q in newLi.GetType().GetProperties())
                            {
                                string valuePath = "";
                                //en caso de que el campo tenga el Attribute AutomaticValueAttribute no mandar el ID si no el texto
                                //ejemplo si estamos hablando tblInteractions.savedByuserID, no regresar el GUID si no el username

                                if (Reflection.HasCustomAttribute<AutomaticValueAttribute>(q))
                                {
                                    AutomaticValueAttribute av = Reflection.GetCustomAttribute<AutomaticValueAttribute>(q);
                                    valuePath = av.ValuePath;
                                }

                                // en caso de que no sea utomatic value, pero sea de una tabla relacionada, obtener el valor segun la ruta especificada.
                                if (Reflection.HasCustomAttribute<GetValueFromAttribute>(q))
                                {
                                    GetValueFromAttribute gvf = Reflection.GetCustomAttribute<GetValueFromAttribute>(q);
                                    valuePath = gvf.ValuePath;
                                }


                                if (valuePath != null && valuePath != "")
                                {
                                    var _dbValue = Reflection.GetPropertyValueByPath(li, valuePath);
                                    q.SetValue(newLi, _dbValue, null);
                                }
                                else
                                {
                                    var _dbName = Reflection.GetDbNameOrDefault(q);
                                    var _dbValue = li.GetType().GetProperty(_dbName).GetValue(li, null);
                                    q.SetValue(newLi, _dbValue, null);
                                }


                            }
                            list.Add(newLi);
                        }
                        if (list.Count > 0)
                        {
                            p.SetValue(uiObject, list, null);
                        }
                    }
                    else if (hasDbInfo && dbi.IsRelationShip == true) //one to one
                    {
                        var dbName = dbi.RelationShipType.Name;
                        IEnumerable<object> dbValue = Reflection.GetPropertyValueByPath(sourceObject, dbName) as IEnumerable<object>;

                        if (dbValue.Count() > 0)
                        {
                            PropertyInfo[] uiProps = p.PropertyType.GetProperties();
                            var propertyContent = p.GetValue(uiObject, null);
                            foreach (var q in uiProps)
                            {
                                //**//*//**//**/**/
                                bool _hasDbInfo = false;
                                DataBaseInfoAttribute _dbi = null;
                                if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(q))
                                {
                                    _dbi = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(q);
                                    _hasDbInfo = true;
                                }
                                if (_hasDbInfo && _dbi.IsRelationShip == true && Reflection.HasCustomAttribute<ListTypeAttribute>(q)) //OneToMany
                                {
                                    var _dbName = dbName + "|0." + _dbi.RelationShipType.Name;
                                    IEnumerable<object> _dbValue = Reflection.GetPropertyValueByPath(sourceObject, _dbName) as IEnumerable<object>;
                                    ListTypeAttribute uiListType = Reflection.GetCustomAttribute<ListTypeAttribute>(q);
                                    IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(uiListType.SingleItemType));

                                    foreach (var li in _dbValue)
                                    {
                                        var newLi = Activator.CreateInstance(uiListType.SingleItemType);
                                        foreach (var _q in newLi.GetType().GetProperties())
                                        {

                                            //en caso de que el campo tenga el Attribute AutomaticValueAttribute no mandar el ID si no el texto
                                            //ejemplo si estamos hablando tblInteractions.savedByuserID, no regresar el GUID si no el username

                                            if (Reflection.HasCustomAttribute<AutomaticValueAttribute>(_q))
                                            {

                                                AutomaticValueAttribute av = Reflection.GetCustomAttribute<AutomaticValueAttribute>(_q);
                                                if (av.ValuePath != null)
                                                {
                                                    var __dbValue = Reflection.GetPropertyValueByPath(li, av.ValuePath);
                                                    _q.SetValue(newLi, _dbValue, null);
                                                }
                                                else
                                                {
                                                    var __dbName = Reflection.GetDbNameOrDefault(_q);
                                                    var __dbValue = li.GetType().GetProperty(__dbName).GetValue(li, null);
                                                    _q.SetValue(newLi, __dbValue, null);
                                                }

                                                //var _dbValue = li.GetType().GetProperty(av.RelatedTableName).GetType().GetProperty(av.RelatedFieldName).GetValue(li, null);


                                            }
                                            else
                                            {
                                                var __dbName = Reflection.GetDbNameOrDefault(_q);
                                                var __dbValue = li.GetType().GetProperty(__dbName).GetValue(li, null);
                                                _q.SetValue(newLi, __dbValue, null);
                                            }


                                        }
                                        list.Add(newLi);
                                    }
                                    if (list.Count > 0)
                                    {
                                        q.SetValue(propertyContent, list, null);
                                    }
                                }
                                else
                                {
                                    //**//**//**//*
                                    var _dbName = Reflection.GetDbNameOrDefault(q);
                                    var _dbValue = Reflection.GetPropertyValueByPath(dbValue.First(), _dbName);
                                    q.SetValue(propertyContent, _dbValue, null);
                                    //**//**//**/*
                                }
                            }
                        }
                    }
                    else
                    {
                        var dbName = Reflection.GetDbNameOrDefault(p);
                        //var dbValue = sourceObject.GetType().GetProperty(dbName).GetValue(sourceObject, null);
                        var dbValue = Reflection.GetPropertyValueByPath(sourceObject, dbName);
                        p.SetValue(uiObject, dbValue, null);
                    }
                }

            }

            public static void setUpRelationShipProperty<T>(object sourceObject, PropertyInfo p, ref T returnObject, ref List<ChangesTracking.ChangeItem> changesList, ref string path, string logReferencePath, ref List<ChangesTracking.OnValueChangedItem> onValueChangedList)
            {
                /*Changes Tracking:BEGIN*/
                string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p).RelationShipType.Name;
                path += "." + tableName;
                //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
                var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);
                /*Changes Tracking:END*/


                DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
                ListTypeAttribute ListAttributes = Reflection.GetCustomAttribute<ListTypeAttribute>(p);
                Type relationShipType = dbInfo.RelationShipType; // v.g. tblLeadEmails                        
                // the list from UI
                IEnumerable<object> listItems = p.GetValue(sourceObject, null) as IEnumerable<object>;
                //The list from db     
                PropertyInfo returnProperty = Reflection.GetPropertyByName(returnObject, relationShipType.Name);
                //find out the PK's name so we can match by that name.
                var relationShipPkName = dbInfo.PrimaryKeyModelName;

                #region "Updates"
                /////UPDATES/////////////////////////////////////////////////////////////////////////
                foreach (var iDbItem in returnProperty.GetValue(returnObject, null) as IEnumerable)
                {
                    //get list of Log Reference Names so we can know where to get the reference values from.
                    object dbPkValue = Reflection.GetPropertyValueByPath(iDbItem, relationShipPkName);
                    //if (dbPkValue.GetType() == typeof(long) || dbPkValue.GetType() == typeof(long?))
                    //{// if is long convert it to int so can compare objects of the same type otherwise  Long == int : false;  and won't be any element in the query
                    //    dbPkValue = Convert.ToInt32(dbPkValue);
                    //}
                    //por cada item de la db, buscar su correspondiene en los uiItems matching by primaryKey value
                    var iUiItem = listItems.Where(xy => Reflection.GetPropertyValueByPath(xy, relationShipPkName).Equals(dbPkValue)).First();
                    PropertyInfo[] liProperties = Reflection.GetPropertiesWithoutAtribute<DoNotUpdateAttribute>(iUiItem);


                    ChangesTracking.LogReferenceObject LogReference = ChangesTracking.GetLogReference(iUiItem);
                    string FullReferenceText = logReferencePath;

                    foreach (var uiItemProperty in liProperties)
                    {
                        var _returnObject = iDbItem;
                        var _sourceObject = iUiItem;
                        // setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref  changesList,LogReference);

                        if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(uiItemProperty))
                        {
                            DataBaseInfoAttribute uiItemProperty_dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(uiItemProperty);
                            if (uiItemProperty_dbInfo.IsRelationShip && Attribute.IsDefined(uiItemProperty, typeof(ListTypeAttribute)))
                            {
                                setUpRelationShipProperty(_sourceObject, uiItemProperty, ref _returnObject, ref changesList, ref path, logReferencePath, ref onValueChangedList);
                            }
                        }
                        else
                        {
                            setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref changesList, LogReference.Text, sysComponentsIds, ref path, logReferencePath, LogReference.FieldDbNames, ref onValueChangedList);
                        }
                    }

                }
                #endregion

                #region "Inserts"
                /////INSERTS////////////////////////////////////////////////////////////////////////////////
                //PropertyInfo[] inserts= p.GetValue(sourceObject, null).GetType().GetProperties().Where(x => !HasDbIdentity(x)) as PropertyInfo[];
                var inserts = listItems.Where(li => Reflection.GetPropertyValueByPath(li, relationShipPkName) == null);

                if (inserts != null && inserts.Count() > 0)
                {
                    foreach (var ins in inserts)
                    {
                        ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(ins);

                        string LogReference = LogReferenceObj.Text;
                        var newItem = Activator.CreateInstance(dbInfo.RelationShipType);
                        //var newItemProperties = dbInfo.RelationShipType.GetProperties().Where(x => !Attribute.IsDefined(x, typeof(isPrimaryKey)));
                        var newItemProperties = ins.GetType().GetProperties().Where(x => !Attribute.IsDefined(x, typeof(IsPrimaryKey)));
                        //PropertyInfo[] referenceProperties = Reflection.GetPropertiesByAtribute<LogReferenceAttribute>(sourceObject);
                        // string LogReference = ChangesTracking.GetLogReference(ins);
                        foreach (var nip in newItemProperties)
                        {
                            var _returnObject = newItem;
                            var _sourceObject = ins;
                            // setUpSimpleProperty(_sourceObject, nip, ref _returnObject, ref  changesList,LogReference);

                            DataBaseInfoAttribute nip_dbInfo = null;
                            if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(nip))
                            {
                                nip_dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(nip);
                            }

                            if (nip_dbInfo != null && nip_dbInfo.IsRelationShip && Attribute.IsDefined(nip, typeof(ListTypeAttribute)))
                            {
                                setUpRelationShipProperty(_sourceObject, nip, ref _returnObject, ref changesList, ref path, logReferencePath, ref onValueChangedList);
                            }
                            else
                            {
                                //string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(_sourceObject).Name;
                                //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
                                //var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);

                                setUpSimpleProperty(_sourceObject, nip, ref _returnObject, ref changesList, LogReference, sysComponentsIds, ref path, logReferencePath, LogReferenceObj.FieldDbNames, ref onValueChangedList);
                            }

                            //var uiValue = Reflection.GetPropertyValueByPath(ins, nip.Name);
                            //newItem.GetType().GetProperty(nip.Name).SetValue(newItem, uiValue, null);                
                        }
                        //Make sure we get the overload that takes 1 parameter and the parameter has the same type as our newItem
                        MethodInfo Add = returnProperty.PropertyType.GetMethods().Where(x => x.Name == "Add" && x.GetParameters()[0].ParameterType == relationShipType).FirstOrDefault();
                        Add.Invoke(returnProperty.GetValue(returnObject, null), new object[] { newItem });
                    }
                }
                #endregion

                //shorten the path
                path = path.Replace("." + tableName, "");
            }
            //public static void setUpOneToManyRelationShipProperty<T>(object sourceObject, PropertyInfo p, ref T returnObject, ref List<ChangesTracking.ChangeItem> changesList, ref string path, string logReferencePath)
            //{
            //    /*Changes Tracking:BEGIN*/
            //    string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p).RelationShipType.Name;
            //    path += "." + tableName;
            //    //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
            //    var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);
            //    /*Changes Tracking:END*/


            //    DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
            //    ListTypeAttribute ListAttributes = Reflection.GetCustomAttribute<ListTypeAttribute>(p);
            //    Type relationShipType = dbInfo.RelationShipType; // v.g. tblLeadEmails                        
            //    // the list from UI
            //    IEnumerable<object> listItems = p.GetValue(sourceObject, null) as IEnumerable<object>;
            //    //The list from db     
            //    PropertyInfo returnProperty = Reflection.GetPropertyByName(returnObject, relationShipType.Name);
            //    //find out the PK's name so we can match by that name.
            //    var relationShipPkDatabaseName = dbInfo.PrimaryKeyDatabaseName;
            //    var relationShipPkModelName = dbInfo.PrimaryKeyModelName;

            //    #region "Updates"
            //    /////UPDATES/////////////////////////////////////////////////////////////////////////
            //    foreach (var iDbItem in returnProperty.GetValue(returnObject, null) as IEnumerable)
            //    {
            //        //get list of Log Reference Names so we can know where to get the reference values from.
            //        object dbPkValue = Reflection.GetPropertyValueByPath(iDbItem, relationShipPkDatabaseName);
            //        //if (dbPkValue.GetType() == typeof(long) || dbPkValue.GetType() == typeof(long?))
            //        //{// if is long convert it to int so can compare objects of the same type otherwise  Long == int : false;  and won't be any element in the query
            //        //    dbPkValue = Convert.ToInt32(dbPkValue);
            //        //}
            //        //por cada item de la db, buscar su correspondiene en los uiItems matching by primaryKey value
            //        var iUiItem = listItems.Where(xy => Reflection.GetPropertyValueByPath(xy, relationShipPkModelName) != null && Reflection.GetPropertyValueByPath(xy, relationShipPkModelName).Equals(dbPkValue)).First();


            //        PropertyInfo[] liProperties = Reflection.GetPropertiesWithoutAtribute<DoNotUpdateAttribute>(iUiItem);


            //        ChangesTracking.LogReferenceObject LogReference = ChangesTracking.GetLogReference(iUiItem);
            //        string FullReferenceText = logReferencePath;

            //        foreach (var uiItemProperty in liProperties)
            //        {
            //            var _returnObject = iDbItem;
            //            var _sourceObject = iUiItem;
            //            // setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref  changesList,LogReference);

            //            if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(uiItemProperty))
            //            {
            //                DataBaseInfoAttribute uiItemProperty_dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(uiItemProperty);
            //                if (uiItemProperty_dbInfo.IsRelationShip && Attribute.IsDefined(uiItemProperty, typeof(ListTypeAttribute)))
            //                {
            //                    setUpRelationShipProperty(_sourceObject, uiItemProperty, ref _returnObject, ref changesList, ref path, logReferencePath);
            //                }
            //                else {
            //                    setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref  changesList, LogReference.Text, sysComponentsIds, ref path, logReferencePath, LogReference.FieldDbNames);
            //                }
            //            }
            //            else
            //            {
            //                setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref  changesList, LogReference.Text, sysComponentsIds, ref path, logReferencePath, LogReference.FieldDbNames);
            //            }
            //        }

            //    }
            //    #endregion

            //    #region "Inserts"
            //    /////INSERTS////////////////////////////////////////////////////////////////////////////////

            //    //PropertyInfo[] inserts= p.GetValue(sourceObject, null).GetType().GetProperties().Where(x => !HasDbIdentity(x)) as PropertyInfo[];


            //    if(listItems.Count()>0){
            //        var inserts = listItems.Where(li => Reflection.GetPropertyValueByPath(li, relationShipPkModelName) == null);
            //        if (inserts != null && inserts.Count() > 0)
            //        {
            //            foreach (var ins in inserts)
            //            {
            //                ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(ins);

            //                string LogReference = LogReferenceObj.Text;
            //                var newItem = Activator.CreateInstance(dbInfo.RelationShipType);
            //                //var newItemProperties = dbInfo.RelationShipType.GetProperties().Where(x => !Attribute.IsDefined(x, typeof(isPrimaryKey)));
            //                var newItemProperties = ins.GetType().GetProperties().Where(x => !Attribute.IsDefined(x, typeof(IsPrimaryKey)));
            //                //PropertyInfo[] referenceProperties = Reflection.GetPropertiesByAtribute<LogReferenceAttribute>(sourceObject);
            //                // string LogReference = ChangesTracking.GetLogReference(ins);
            //                foreach (var nip in newItemProperties)
            //                {
            //                    var _returnObject = newItem;
            //                    var _sourceObject = ins;
            //                    // setUpSimpleProperty(_sourceObject, nip, ref _returnObject, ref  changesList,LogReference);

            //                    DataBaseInfoAttribute nip_dbInfo = null;
            //                    if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(nip))
            //                    {
            //                        nip_dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(nip);
            //                    }

            //                    if (nip_dbInfo != null && nip_dbInfo.IsRelationShip && Attribute.IsDefined(nip, typeof(ListTypeAttribute)))
            //                    {
            //                        setUpRelationShipProperty(_sourceObject, nip, ref _returnObject, ref changesList, ref path, logReferencePath);
            //                    }
            //                    else
            //                    {
            //                        //string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(_sourceObject).Name;
            //                        //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
            //                        //var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);

            //                        setUpSimpleProperty(_sourceObject, nip, ref _returnObject, ref  changesList, LogReference, sysComponentsIds, ref path, logReferencePath, LogReferenceObj.FieldDbNames);
            //                    }

            //                    //var uiValue = Reflection.GetPropertyValueByPath(ins, nip.Name);
            //                    //newItem.GetType().GetProperty(nip.Name).SetValue(newItem, uiValue, null);                
            //                }
            //                //Make sure we get the overload that takes 1 parameter and the parameter has the same type as our newItem
            //                MethodInfo Add = returnProperty.PropertyType.GetMethods().Where(x => x.Name == "Add" && x.GetParameters()[0].ParameterType == relationShipType).FirstOrDefault();
            //                Add.Invoke(returnProperty.GetValue(returnObject, null), new object[] { newItem });
            //            }
            //        }
            //    }


            //    #endregion

            //    //shorten the path
            //    path = path.Replace("." + tableName, "");

            //}
            public static void setUpOneToManyRelationShipProperty(object sourceObject, PropertyInfo p, ref object returnObject, ref List<ChangesTracking.ChangeItem> changesList, ref string path, string logReferencePath, ref List<ChangesTracking.OnValueChangedItem> onValueChangedList)
            {
                /*Changes Tracking:BEGIN*/
                string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p).RelationShipType.Name;
                path += "." + tableName;
                //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
                var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);
                /*Changes Tracking:END*/


                DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
                ListTypeAttribute ListAttributes = Reflection.GetCustomAttribute<ListTypeAttribute>(p);
                Type relationShipType = dbInfo.RelationShipType; // v.g. tblLeadEmails                        
                // the list from UI
                IEnumerable<object> listItems = p.GetValue(sourceObject, null) as IEnumerable<object>;
                //The list from db     
                PropertyInfo returnProperty = Reflection.GetPropertyByName(returnObject, relationShipType.Name);
                //find out the PK's name so we can match by that name.
                var relationShipPkDatabaseName = dbInfo.PrimaryKeyDatabaseName;
                var relationShipPkModelName = dbInfo.PrimaryKeyModelName;

                #region "Updates"
                /////UPDATES/////////////////////////////////////////////////////////////////////////
                foreach (var iDbItem in returnProperty.GetValue(returnObject, null) as IEnumerable)
                {
                    //get list of Log Reference Names so we can know where to get the reference values from.
                    object dbPkValue = Reflection.GetPropertyValueByPath(iDbItem, relationShipPkDatabaseName);
                    //if (dbPkValue.GetType() == typeof(long) || dbPkValue.GetType() == typeof(long?))
                    //{// if is long convert it to int so can compare objects of the same type otherwise  Long == int : false;  and won't be any element in the query
                    //    dbPkValue = Convert.ToInt32(dbPkValue);
                    //}
                    //por cada item de la db, buscar su correspondiene en los uiItems matching by primaryKey value
                    var iUiItem = listItems.Where(xy => Reflection.GetPropertyValueByPath(xy, relationShipPkModelName) != null && Reflection.GetPropertyValueByPath(xy, relationShipPkModelName).Equals(dbPkValue)).First();


                    PropertyInfo[] liProperties = Reflection.GetPropertiesWithoutAtribute<DoNotUpdateAttribute>(iUiItem);


                    ChangesTracking.LogReferenceObject LogReference = ChangesTracking.GetLogReference(iUiItem);
                    string FullReferenceText = logReferencePath;

                    foreach (var uiItemProperty in liProperties)
                    {
                        var _returnObject = iDbItem;
                        var _sourceObject = iUiItem;
                        // setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref  changesList,LogReference);

                        if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(uiItemProperty))
                        {
                            DataBaseInfoAttribute uiItemProperty_dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(uiItemProperty);
                            if (uiItemProperty_dbInfo.IsRelationShip && Attribute.IsDefined(uiItemProperty, typeof(ListTypeAttribute)))
                            {
                                setUpRelationShipProperty(_sourceObject, uiItemProperty, ref _returnObject, ref changesList, ref path, logReferencePath, ref onValueChangedList);
                            }
                            else
                            {
                                setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref changesList, LogReference.Text, sysComponentsIds, ref path, logReferencePath, LogReference.FieldDbNames, ref onValueChangedList);
                            }
                        }
                        else
                        {
                            setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref changesList, LogReference.Text, sysComponentsIds, ref path, logReferencePath, LogReference.FieldDbNames, ref onValueChangedList);
                        }
                    }

                }
                #endregion

                #region "Inserts"
                /////INSERTS////////////////////////////////////////////////////////////////////////////////

                //PropertyInfo[] inserts= p.GetValue(sourceObject, null).GetType().GetProperties().Where(x => !HasDbIdentity(x)) as PropertyInfo[];

                if (listItems != null)
                {
                    if (listItems.Count() > 0)
                    {
                        //var inserts = listItems.Where(li => Reflection.GetPropertyValueByPath(li, relationShipPkModelName) == null);
                        var inserts = listItems.Where(li => Reflection.GetPropertyValueByPath(li, relationShipPkModelName) == null || int.Parse(Reflection.GetPropertyValueByPath(li, relationShipPkModelName).ToString()) == 0);
                        if (inserts != null && inserts.Count() > 0)
                        {
                            foreach (var ins in inserts)
                            {
                                ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(ins);

                                string LogReference = LogReferenceObj.Text;
                                var newItem = Activator.CreateInstance(dbInfo.RelationShipType);
                                //var newItemProperties = dbInfo.RelationShipType.GetProperties().Where(x => !Attribute.IsDefined(x, typeof(isPrimaryKey)));
                                var newItemProperties = ins.GetType().GetProperties().Where(x => !Attribute.IsDefined(x, typeof(IsPrimaryKey)));
                                //PropertyInfo[] referenceProperties = Reflection.GetPropertiesByAtribute<LogReferenceAttribute>(sourceObject);
                                // string LogReference = ChangesTracking.GetLogReference(ins);
                                foreach (var nip in newItemProperties)
                                {
                                    var _returnObject = newItem;
                                    var _sourceObject = ins;
                                    // setUpSimpleProperty(_sourceObject, nip, ref _returnObject, ref  changesList,LogReference);

                                    DataBaseInfoAttribute nip_dbInfo = null;
                                    if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(nip))
                                    {
                                        nip_dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(nip);
                                    }

                                    if (nip_dbInfo != null && nip_dbInfo.IsRelationShip && Attribute.IsDefined(nip, typeof(ListTypeAttribute)))
                                    {
                                        setUpRelationShipProperty(_sourceObject, nip, ref _returnObject, ref changesList, ref path, logReferencePath, ref onValueChangedList);
                                    }
                                    else
                                    {
                                        //string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(_sourceObject).Name;
                                        //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
                                        //var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);

                                        setUpSimpleProperty(_sourceObject, nip, ref _returnObject, ref changesList, LogReference, sysComponentsIds, ref path, logReferencePath, LogReferenceObj.FieldDbNames, ref onValueChangedList);
                                    }

                                    //var uiValue = Reflection.GetPropertyValueByPath(ins, nip.Name);
                                    //newItem.GetType().GetProperty(nip.Name).SetValue(newItem, uiValue, null);                
                                }
                                //Make sure we get the overload that takes 1 parameter and the parameter has the same type as our newItem
                                MethodInfo Add = returnProperty.PropertyType.GetMethods().Where(x => x.Name == "Add" && x.GetParameters()[0].ParameterType == relationShipType).FirstOrDefault();
                                Add.Invoke(returnProperty.GetValue(returnObject, null), new object[] { newItem });
                            }
                        }
                    }
                }

                #endregion

                //shorten the path
                path = path.Replace("." + tableName, "");

            }

            public static void OneToManySetUp(this PropertyInfo p, ref object sourceObject, ref object returnObject, ref List<ChangesTracking.ChangeItem> changesList, ref string path, string logReferencePath, ref List<ChangesTracking.OnValueChangedItem> onValueChangedList)
            {
                /*Changes Tracking:BEGIN*/
                string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p).RelationShipType.Name;
                path += "." + tableName;
                //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
                var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);
                /*Changes Tracking:END*/


                DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
                ListTypeAttribute ListAttributes = Reflection.GetCustomAttribute<ListTypeAttribute>(p);
                Type relationShipType = dbInfo.RelationShipType; // v.g. tblLeadEmails                        
                // the list from UI
                IEnumerable<object> listItems = p.GetValue(sourceObject, null) as IEnumerable<object>;
                //The list from db     
                PropertyInfo returnProperty = Reflection.GetPropertyByName(returnObject, relationShipType.Name);
                //find out the PK's name so we can match by that name.
                var relationShipPkDatabaseName = dbInfo.PrimaryKeyDatabaseName;
                var relationShipPkModelName = dbInfo.PrimaryKeyModelName;

                #region "Updates"
                /////UPDATES/////////////////////////////////////////////////////////////////////////
                foreach (var iDbItem in returnProperty.GetValue(returnObject, null) as IEnumerable)
                {
                    //get list of Log Reference Names so we can know where to get the reference values from.
                    object dbPkValue = Reflection.GetPropertyValueByPath(iDbItem, relationShipPkDatabaseName);
                    //if (dbPkValue.GetType() == typeof(long) || dbPkValue.GetType() == typeof(long?))
                    //{// if is long convert it to int so can compare objects of the same type otherwise  Long == int : false;  and won't be any element in the query
                    //    dbPkValue = Convert.ToInt32(dbPkValue);
                    //}
                    //por cada item de la db, buscar su correspondiene en los uiItems matching by primaryKey value
                    var iUiItem = listItems.Where(xy => Reflection.GetPropertyValueByPath(xy, relationShipPkModelName) != null && Reflection.GetPropertyValueByPath(xy, relationShipPkModelName).Equals(dbPkValue)).First();


                    PropertyInfo[] liProperties = Reflection.GetPropertiesWithoutAtribute<DoNotUpdateAttribute>(iUiItem);


                    ChangesTracking.LogReferenceObject LogReference = ChangesTracking.GetLogReference(iUiItem);
                    string FullReferenceText = logReferencePath;

                    foreach (var uiItemProperty in liProperties)
                    {
                        var _returnObject = iDbItem;
                        var _sourceObject = iUiItem;
                        // setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref  changesList,LogReference);

                        if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(uiItemProperty))
                        {
                            DataBaseInfoAttribute uiItemProperty_dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(uiItemProperty);
                            if (uiItemProperty_dbInfo.IsRelationShip && Attribute.IsDefined(uiItemProperty, typeof(ListTypeAttribute)))
                            {
                                setUpRelationShipProperty(_sourceObject, uiItemProperty, ref _returnObject, ref changesList, ref path, logReferencePath, ref onValueChangedList);
                            }
                            else
                            {
                                setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref changesList, LogReference.Text, sysComponentsIds, ref path, logReferencePath, LogReference.FieldDbNames, ref onValueChangedList);
                            }
                        }
                        else
                        {
                            setUpSimpleProperty(_sourceObject, uiItemProperty, ref _returnObject, ref changesList, LogReference.Text, sysComponentsIds, ref path, logReferencePath, LogReference.FieldDbNames, ref onValueChangedList);
                        }
                    }

                }
                #endregion

                #region "Inserts"
                /////INSERTS////////////////////////////////////////////////////////////////////////////////

                //PropertyInfo[] inserts= p.GetValue(sourceObject, null).GetType().GetProperties().Where(x => !HasDbIdentity(x)) as PropertyInfo[];


                if (listItems.Count() > 0)
                {
                    var inserts = listItems.Where(li => Reflection.GetPropertyValueByPath(li, relationShipPkModelName) == null);
                    if (inserts != null && inserts.Count() > 0)
                    {
                        foreach (var ins in inserts)
                        {
                            ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(ins);

                            string LogReference = LogReferenceObj.Text;
                            var newItem = Activator.CreateInstance(dbInfo.RelationShipType);
                            //var newItemProperties = dbInfo.RelationShipType.GetProperties().Where(x => !Attribute.IsDefined(x, typeof(isPrimaryKey)));
                            var newItemProperties = ins.GetType().GetProperties().Where(x => !Attribute.IsDefined(x, typeof(IsPrimaryKey)));
                            //PropertyInfo[] referenceProperties = Reflection.GetPropertiesByAtribute<LogReferenceAttribute>(sourceObject);
                            // string LogReference = ChangesTracking.GetLogReference(ins);
                            foreach (var nip in newItemProperties)
                            {
                                var _returnObject = newItem;
                                var _sourceObject = ins;
                                // setUpSimpleProperty(_sourceObject, nip, ref _returnObject, ref  changesList,LogReference);

                                DataBaseInfoAttribute nip_dbInfo = null;
                                if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(nip))
                                {
                                    nip_dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(nip);
                                }

                                if (nip_dbInfo != null && nip_dbInfo.IsRelationShip && Attribute.IsDefined(nip, typeof(ListTypeAttribute)))
                                {
                                    setUpRelationShipProperty(_sourceObject, nip, ref _returnObject, ref changesList, ref path, logReferencePath, ref onValueChangedList);
                                }
                                else
                                {
                                    //string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(_sourceObject).Name;
                                    //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
                                    //var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);

                                    setUpSimpleProperty(_sourceObject, nip, ref _returnObject, ref changesList, LogReference, sysComponentsIds, ref path, logReferencePath, LogReferenceObj.FieldDbNames, ref onValueChangedList);
                                }

                                //var uiValue = Reflection.GetPropertyValueByPath(ins, nip.Name);
                                //newItem.GetType().GetProperty(nip.Name).SetValue(newItem, uiValue, null);                
                            }
                            //Make sure we get the overload that takes 1 parameter and the parameter has the same type as our newItem
                            MethodInfo Add = returnProperty.PropertyType.GetMethods().Where(x => x.Name == "Add" && x.GetParameters()[0].ParameterType == relationShipType).FirstOrDefault();
                            Add.Invoke(returnProperty.GetValue(returnObject, null), new object[] { newItem });
                        }
                    }
                }


                #endregion

                //shorten the path
                path = path.Replace("." + tableName, "");

            }

            public static void SetUpWith(this object destinationObject, ref object sourceObject, ref List<ChangesTracking.ChangeItem> changesList, ref List<ChangesTracking.OnValueChangedItem> onValueChangedList)
            {
                string path = "";

                ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(sourceObject);

                string LogReference = "";
                LogReference = LogReferenceObj.Text;

                Type dbInfoAttr = typeof(DataBaseInfoAttribute);
                //var props = sourceObject.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, dbInfoAttr));
                var props = Reflection.GetPropertiesByAtribute<DataBaseInfoAttribute>(sourceObject);
                //get the syscomponents names base on simple propertyNames

                //get table name for syscomponents
                Type sourceObjType = sourceObject.GetType();
                //para el siguiente c
                DataBaseInfoAttribute sourceObjectDbInfo = (DataBaseInfoAttribute)Attribute.GetCustomAttribute(sourceObjType, typeof(DataBaseInfoAttribute));

                //string tableName = sourceObjectDbInfo.Name;
                string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(sourceObject).Name;

                // path =  tableName;
                //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
                var sysComponentsId = ChangesTracking.GetSysComponentIDs(tableName);

                foreach (var p in props)
                {
                    DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
                    if (dbInfo.IsRelationShip)
                    {
                        if (dbInfo.Cardinality == DataBaseRelationShipCardinality.OneToMany && Attribute.IsDefined(p, typeof(ListTypeAttribute)))
                        {
                            setUpOneToManyRelationShipProperty(sourceObject, p, ref destinationObject, ref changesList, ref path, LogReference, ref onValueChangedList);

                        }
                        else if (dbInfo.Cardinality == DataBaseRelationShipCardinality.OneToOne)
                        {

                            p.OneToManySetUp(ref sourceObject, ref destinationObject, ref changesList, ref path, LogReference, ref onValueChangedList);
                            //setUpOneToOneRelationShipProperty(sourceObject, p, ref destinationObject, ref changesList, ref path, LogReference);
                        }
                    }
                    else
                    {
                        setUpSimpleProperty(sourceObject, p, ref destinationObject, ref changesList, LogReference, sysComponentsId, ref path, LogReference, LogReferenceObj.FieldDbNames, ref onValueChangedList);
                    }
                }

            }
            //public static void setUpOneToOneRelationShipProperty<T>(object sourceObject, PropertyInfo p, ref T returnObject, ref List<ChangesTracking.ChangeItem> changesList, ref string path, string logReferencePath)
            //{
            //    /*Changes Tracking:BEGIN*/
            //    string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p).RelationShipType.Name;
            //    path += "." + tableName;
            //    //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
            //    var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);
            //    /*Changes Tracking:END*/


            //    DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
            //    Type relationShipType = dbInfo.RelationShipType; // v.g. tblLeadEmails

            //    PropertyInfo returnProperty = Reflection.GetPropertyByName(returnObject, relationShipType.Name);
            //    IEnumerable returnPropertyValue = (IEnumerable)returnProperty.GetValue(returnObject, null);

            //    //find out the PK's name so we can match by that name.

            //    var relationShipPkName = dbInfo.PrimaryKeyModelName;

            //    object sourcePropertyValue = p.GetValue(sourceObject, null);
            //    object first = returnPropertyValue.Cast<object>().FirstOrDefault();

            //    if (returnPropertyValue.Cast<object>().Count() <= 0)
            //    {
            //        first = Activator.CreateInstance(dbInfo.RelationShipType);
            //    }
            //    ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(sourceObject);
            //    string LogReference = "";
            //    LogReference = LogReferenceObj.Text;

            //    foreach (var sp in Reflection.GetPropertiesWithoutAtribute<IsPrimaryKey>(sourcePropertyValue))
            //    {
            //        //setUpRelationShipProperty(sourcePropertyValue, sp, ref first, ref changesList, ref path, logReferencePath);

            //        //si el la propiedad es relationship averguar si es oneToOne o OneToMany
            //        //--//--//--//-/--/-

            //        DataBaseInfoAttribute spDbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(sp);
            //        if (spDbInfo.IsRelationShip)
            //        {
            //            if (spDbInfo.Cardinality == DataBaseRelationShipCardinality.OneToMany && Attribute.IsDefined(p, typeof(ListTypeAttribute)))
            //            {
            //                setUpOneToManyRelationShipProperty<T>(sourcePropertyValue, sp, ref first, ref changesList, ref path, LogReference);

            //            }
            //            else if (spDbInfo.Cardinality == DataBaseRelationShipCardinality.OneToOne)
            //            {
            //                setUpOneToOneRelationShipProperty<T>(sourcePropertyValue, sp, ref first, ref changesList, ref path, LogReference);
            //            }
            //        }
            //        else
            //        {
            //            setUpSimpleProperty(sourcePropertyValue, sp, ref first, ref changesList, LogReference, sysComponentsIds, ref path, LogReference, LogReferenceObj.FieldDbNames);
            //            setUpSimpleProperty(sourcePropertyValue, sp, ref first, ref  changesList, LogReference, sysComponentsIds, ref path, logReferencePath, LogReferenceObj.FieldDbNames);
            //        }
            //        //--//--//--//--/-

            //        //else tratar como propiedad simple
            //        setUpSimpleProperty(sourcePropertyValue, sp, ref first, ref  changesList, LogReference, sysComponentsIds, ref path, logReferencePath, LogReferenceObj.FieldDbNames);
            //    }

            //    if (returnPropertyValue.Cast<object>().Count() <= 0)
            //    {
            //        MethodInfo Add = returnProperty.PropertyType.GetMethods().Where(x => x.Name == "Add" && x.GetParameters()[0].ParameterType == relationShipType).FirstOrDefault();
            //        Add.Invoke(returnProperty.GetValue(returnObject, null), new object[] { first });

            //    }

            //    //shorten the path
            //    path = path.Replace("." + tableName, "");

            //}
            public static void setUpOneToOneRelationShipProperty(object sourceObject, PropertyInfo p, ref object returnObject, ref List<ChangesTracking.ChangeItem> changesList, ref string path, string logReferencePath, ref List<ChangesTracking.OnValueChangedItem> onValueChangedList)
            {
                /*Changes Tracking:BEGIN*/
                string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p).RelationShipType.Name;
                path += "." + tableName;
                //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
                var sysComponentsIds = ChangesTracking.GetSysComponentIDs(tableName);
                /*Changes Tracking:END*/


                DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
                Type relationShipType = dbInfo.RelationShipType; // v.g. tblLeadEmails

                PropertyInfo returnProperty = Reflection.GetPropertyByName(returnObject, relationShipType.Name);
                IEnumerable returnPropertyValue = (IEnumerable)returnProperty.GetValue(returnObject, null);

                //find out the PK's name so we can match by that name.

                var relationShipPkName = dbInfo.PrimaryKeyModelName;

                object sourcePropertyValue = p.GetValue(sourceObject, null);
                object first = returnPropertyValue.Cast<object>().FirstOrDefault();

                if (returnPropertyValue.Cast<object>().Count() <= 0)
                {
                    first = Activator.CreateInstance(dbInfo.RelationShipType);
                }
                //ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(sourceObject);
                ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(sourcePropertyValue);
                string LogReference = "";
                LogReference = logReferencePath + "." + LogReferenceObj.Text;

                foreach (var sp in Reflection.GetPropertiesWithoutAtribute<IsPrimaryKey>(sourcePropertyValue))
                {
                    //setUpRelationShipProperty(sourcePropertyValue, sp, ref first, ref changesList, ref path, logReferencePath);

                    //si el la propiedad es relationship averguar si es oneToOne o OneToMany
                    //--//--//--//-/--/-

                    DataBaseInfoAttribute spDbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(sp);
                    if (spDbInfo.IsRelationShip)
                    {
                        if (spDbInfo.Cardinality == DataBaseRelationShipCardinality.OneToMany && HasCustomAttribute<ListTypeAttribute>(sp))
                        {
                            setUpOneToManyRelationShipProperty(sourcePropertyValue, sp, ref first, ref changesList, ref path, LogReference, ref onValueChangedList);
                        }
                        else if (spDbInfo.Cardinality == DataBaseRelationShipCardinality.OneToOne)
                        {
                            setUpOneToOneRelationShipProperty(sourcePropertyValue, sp, ref first, ref changesList, ref path, LogReference, ref onValueChangedList);
                        }
                    }
                    else
                    {
                        setUpSimpleProperty(sourcePropertyValue, sp, ref first, ref changesList, LogReference, sysComponentsIds, ref path, LogReference, LogReferenceObj.FieldDbNames, ref onValueChangedList);
                    }
                    //--//--//--//--/-

                    //else tratar como propiedad simple
                    //setUpSimpleProperty(sourcePropertyValue, sp, ref first, ref  changesList, LogReference, sysComponentsIds, ref path, logReferencePath, LogReferenceObj.FieldDbNames);
                }

                if (returnPropertyValue.Cast<object>().Count() <= 0)
                {
                    MethodInfo Add = returnProperty.PropertyType.GetMethods().Where(x => x.Name == "Add" && x.GetParameters()[0].ParameterType == relationShipType).FirstOrDefault();
                    Add.Invoke(returnProperty.GetValue(returnObject, null), new object[] { first });

                }

                //shorten the path
                path = path.Replace("." + tableName, "");

            }

            public static void setUpSimpleProperty(object sourceObject, PropertyInfo p, ref object returnObject, ref List<Utils.ChangesTracking.ChangeItem> changesList, string LogReference, IDictionary<string, int> sysComponentIds, ref string path, string logReferencePath, List<string> LogReferenceDbFieldNames, ref List<ChangesTracking.OnValueChangedItem> onValueChangedList)
            {
                string dbFieldName = Reflection.GetDbNameOrDefault(p);
                if (returnObject.GetType().GetProperty(dbFieldName) != null)
                {
                    string uiFieldName = p.Name;

                    //object dbValue = returnObject.GetType().GetProperty(dbFieldName).GetValue(returnObject, null);
                    object dbValue = Reflection.GetPropertyValueByPath(returnObject, dbFieldName);
                    //object uiValue = sourceObject.GetType().GetProperty(uiFieldName).GetValue(sourceObject, null);
                    object uiValue = Reflection.GetPropertyValueByPath(sourceObject, uiFieldName);

                    //mike


                    var dbValueText = "";
                    var uiValueText = "";

                    //end mike
                    if (uiValue != null)
                    {
                        if (uiValue.GetType() == typeof(Guid) && !Reflection.HasDbIdentity((Guid)uiValue))
                        {
                            uiValue = null;
                        }
                        else if (uiValue.GetType() == typeof(long) && !Reflection.HasDbIdentity((long)uiValue))
                        {
                            uiValue = null;
                        }
                        else if (uiValue.GetType() == typeof(int) && !Reflection.HasDbIdentity((int)uiValue))
                        {
                            uiValue = null;
                        }
                    }

                    if (Reflection.HasCustomAttribute<AutomaticValueAttribute>(p) && (uiValue == null || uiValue == ""))
                    {

                        AutomaticValueAttribute attr = Reflection.GetCustomAttribute<AutomaticValueAttribute>(p);

                        if (attr.AutomaticValueType == Utils.AutomaticValueTypes.CurrentUser)
                        {
                            uiValue = (Guid)Membership.GetUser().ProviderUserKey;
                        }
                        else if (attr.AutomaticValueType == Utils.AutomaticValueTypes.DateTimeStamp)
                        {
                            uiValue = DateTime.Now;
                        }
                        else if (attr.AutomaticValueType == Utils.AutomaticValueTypes.DateStamp)
                        {
                            DateTime currentDate = new DateTime(DateTime.Now.Ticks);
                            uiValue = currentDate.Date;
                        }
                    }

                    bool isCreditCard = false;
                    if (dbFieldName.ToLower().Contains("cardnumber"))
                    {
                        isCreditCard = true;
                    }
                    ChangesTracking.ObjectValuesComparisonResult valuesComparison = ChangesTracking.CompareUiAndDbValues(dbValue, uiValue, isCreditCard: isCreditCard);

                    if (valuesComparison.ValuesAreDiferent)
                    {
                        PropertyInfo pk = Reflection.GetPropertyByAtribute<IsPrimaryKey>(sourceObject);
                        object dbPkValue = Reflection.GetPropertyValueByPath(sourceObject, pk.Name);

                        if (!Reflection.HasCustomAttribute<DoNotTrackChangesAttribute>(p))
                        {

                            ChangesTracking.ChangeItem change = new ChangesTracking.ChangeItem();

                            change.CurrentValue = valuesComparison.uiComparedValue;
                            //change.CurrentValueObj = (uiValue != null) ? uiValue : null;
                            change.PreviousValue = valuesComparison.dbComparedValue;
                            change.ReferenceID = (dbPkValue != null) ? dbPkValue.ToString() : "";
                            change.ReferenceText = LogReference;
                            change.SysComponent = dbFieldName;

                            int sysCompID;

                            if (sysComponentIds.TryGetValue(dbFieldName, out sysCompID))
                            {
                                change.SysComponentID = sysCompID;
                            }
                            else
                            {
                                string noKeyMessage = "The '" + dbFieldName + "' key doesn't exists in the dictionaty:";
                                foreach (var d in sysComponentIds)
                                {
                                    noKeyMessage += d.Key + ", ";
                                }
                            }

                            change.Path = path;
                            change.PkName = Reflection.GetDbNameOrDefault(pk);

                            if (logReferencePath != LogReference)
                            {
                                //change.FullReferenceText = logReferencePath + ">" + LogReference;
                                change.FullReferenceText = logReferencePath + "." + LogReference;
                            }
                            else { change.FullReferenceText = logReferencePath; }
                            //change.FullReferenceText = logReferencePath + ">" + LogReference;
                            change.LogReferenceDbFieldNames = LogReferenceDbFieldNames;
                            changesList.Add(change);
                        }

                        if (dbPkValue == null)
                        {
                            ////populate the onValueChangedList with proper items
                            if (Reflection.HasCustomAttribute<OnValueChangedAttribute>(p))
                            {
                                object[] attrs = p.GetCustomAttributes(typeof(OnValueChangedAttribute), true);

                                foreach (OnValueChangedAttribute x in attrs)
                                {
                                    ChangesTracking.OnValueChangedItem onValueChangedItem = new ChangesTracking.OnValueChangedItem();

                                    onValueChangedItem.Target = x.Target;
                                    if (x.ValueType == Utils.AutomaticValueTypes.CurrentValue)
                                    {
                                        onValueChangedItem.Value = valuesComparison.uiComparedValue;
                                    }
                                    else
                                    {
                                        onValueChangedItem.Value = x.Value;
                                    }

                                    onValueChangedList.Add(onValueChangedItem);
                                }
                            }

                            //returnObject.GetType().GetProperty(dbFieldName).SetValue(returnObject, uiValue, null);


                        }

                        if (isCreditCard)
                        {
                            uiValue = mexHash.mexHash.EncryptString(uiValue.ToString());
                        }


                        SetPropertyValue(returnObject, dbFieldName, uiValue);

                    }
                }
            }

            //public static void fillUiObject<T>(ref T uiObject, object sourceObject) {
            //    foreach (var p in uiObject.GetType().GetProperties())
            //    {
            //        if (Reflection.HasCustomAttribute<DataBaseInfoAttribute>(p))
            //        {
            //            DataBaseInfoAttribute dbi = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
            //            if (dbi.IsRelationShip == true)
            //            {
            //                var dbName = dbi.RelationShipType.Name;
            //                IEnumerable<object> dbValue = Reflection.GetPropertyValueByPath(sourceObject, dbName) as IEnumerable<object>;
            //                ListTypeAttribute uiListType = Reflection.GetCustomAttribute<ListTypeAttribute>(p);
            //                IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(uiListType.SingleItemType));

            //                foreach (var li in dbValue)
            //                {
            //                    var newLi = Activator.CreateInstance(uiListType.SingleItemType);
            //                    foreach (var q in newLi.GetType().GetProperties())
            //                    {
            //                        var _dbName = Reflection.GetDbNameOrDefault(q);
            //                        var _dbValue = li.GetType().GetProperty(_dbName).GetValue(li, null);
            //                        q.SetValue(newLi, _dbValue, null);
            //                    }
            //                    list.Add(newLi);
            //                }

            //                if (list.Count > 0)
            //                {
            //                    p.SetValue(uiObject, list, null);
            //                }
            //            }
            //            else
            //            {
            //                var dbName = dbi.Name;
            //                var dbValue = sourceObject.GetType().GetProperty(dbName).GetValue(sourceObject, null);
            //                p.SetValue(uiObject, dbValue, null);
            //            }
            //        }


            //    }

            //}



            //public static void dataSetUp<T>(object sourceObject, ref T destinationObject, ref List<ChangesTracking.ChangeItem> changesList)
            //{
            //    string path = "";

            //    ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(sourceObject);

            //    string LogReference = "";
            //    LogReference = LogReferenceObj.Text;

            //    Type dbInfoAttr = typeof(DataBaseInfoAttribute);
            //    //var props = sourceObject.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, dbInfoAttr));
            //    var props = Reflection.GetPropertiesByAtribute<DataBaseInfoAttribute>(sourceObject);
            //    //get the syscomponents names base on simple propertyNames

            //    //get table name for syscomponents
            //    Type sourceObjType = sourceObject.GetType();
            //    //para el siguiente c
            //    DataBaseInfoAttribute sourceObjectDbInfo = (DataBaseInfoAttribute)Attribute.GetCustomAttribute(sourceObjType, typeof(DataBaseInfoAttribute));

            //    //string tableName = sourceObjectDbInfo.Name;
            //    string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(sourceObject).Name;

            //    // path =  tableName;
            //    //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
            //    var sysComponentsId = ChangesTracking.GetSysComponentIDs(tableName);

            //    foreach (var p in props)
            //    {
            //        DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
            //        if (dbInfo.IsRelationShip)
            //        {
            //            if (dbInfo.Cardinality == DataBaseRelationShipCardinality.OneToMany && Attribute.IsDefined(p, typeof(ListTypeAttribute)))
            //            {
            //                setUpOneToManyRelationShipProperty<T>(sourceObject, p, ref destinationObject, ref changesList, ref path, LogReference);
            //            }
            //            else if (dbInfo.Cardinality == DataBaseRelationShipCardinality.OneToOne)
            //            {
            //                setUpOneToOneRelationShipProperty<T>(sourceObject, p, ref destinationObject, ref changesList, ref path, LogReference);
            //            }
            //        }
            //        else
            //        {
            //            setUpSimpleProperty(sourceObject, p, ref destinationObject, ref changesList, LogReference, sysComponentsId, ref path, LogReference, LogReferenceObj.FieldDbNames);
            //        }
            //    }
            //}
            public class fakeObject
            {
                public object entityObject { get; set; }
            }
            public static void dataSetUp(object sourceObject, ref object destinationObject, ref List<ChangesTracking.ChangeItem> changesList, ref List<ChangesTracking.OnValueChangedItem> onValueChangedList)
            {
                string path = "";

                ChangesTracking.LogReferenceObject LogReferenceObj = ChangesTracking.GetLogReference(sourceObject);

                string LogReference = "";
                LogReference = LogReferenceObj.Text;

                Type dbInfoAttr = typeof(DataBaseInfoAttribute);
                //var props = sourceObject.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, dbInfoAttr));
                var props = Reflection.GetPropertiesByAtribute<DataBaseInfoAttribute>(sourceObject);
                //get the syscomponents names base on simple propertyNames

                //get table name for syscomponents
                Type sourceObjType = sourceObject.GetType();
                //para el siguiente c
                DataBaseInfoAttribute sourceObjectDbInfo = (DataBaseInfoAttribute)Attribute.GetCustomAttribute(sourceObjType, typeof(DataBaseInfoAttribute));

                //string tableName = sourceObjectDbInfo.Name;
                string tableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(sourceObject).Name;

                // path =  tableName;
                //IDictionary<string, int> sysComponents = ChangesTracking.GetSysComponentIDs(tableName);
                var sysComponentsId = ChangesTracking.GetSysComponentIDs(tableName);

                foreach (var p in props)
                {
                    DataBaseInfoAttribute dbInfo = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(p);
                    if (dbInfo.IsRelationShip)
                    {
                        if (dbInfo.Cardinality == DataBaseRelationShipCardinality.OneToMany && Attribute.IsDefined(p, typeof(ListTypeAttribute)))
                        {
                            setUpOneToManyRelationShipProperty(sourceObject, p, ref destinationObject, ref changesList, ref path, LogReference, ref onValueChangedList);
                        }
                        else if (dbInfo.Cardinality == DataBaseRelationShipCardinality.OneToOne)
                        {
                            setUpOneToOneRelationShipProperty(sourceObject, p, ref destinationObject, ref changesList, ref path, LogReference, ref onValueChangedList);
                        }
                    }
                    else
                    {
                        setUpSimpleProperty(sourceObject, p, ref destinationObject, ref changesList, LogReference, sysComponentsId, ref path, LogReference, LogReferenceObj.FieldDbNames, ref onValueChangedList);
                    }
                }

                if (onValueChangedList.Count > 0)
                {
                    foreach (ChangesTracking.OnValueChangedItem changedValue in onValueChangedList)
                    {
                        Utils.Custom.Reflection.SetPropertyValue(destinationObject, changedValue.Target, changedValue.Value);
                    }

                }
            }

            //public static object CloneObject(object objSource)
            //{
            //    //step : 1 Get the type of source object and create a new instance of that type
            //    Type typeSource = objSource.GetType();
            //    object objTarget = Activator.CreateInstance(typeSource,true);

            //    //Step2 : Get all the properties of source object type
            //    PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //    //Step : 3 Assign all source property to taget object 's properties
            //    foreach (PropertyInfo property in propertyInfo)
            //    {
            //        var a = Attribute.GetCustomAttributes(property, typeof(EdmScalarPropertyAttribute)).Any(m => ((EdmScalarPropertyAttribute)m).EntityKeyProperty);

            //        //Check whether property can be written to
            //        //if (property.CanWrite && !a)
            //        if (!a)
            //        {
            //            //Step : 4 check whether property type is value type, enum or string type
            //            if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
            //            {
            //                property.SetValue(objTarget, property.GetValue(objSource, null), null);
            //            }
            //            //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
            //            else if(property.PropertyType.Name.IndexOf("EntityCollection") != -1)
            //            {
            //                object objPropertyValue = property.GetValue(objSource, null);
            //                if (objPropertyValue == null)
            //                {
            //                    property.SetValue(objTarget, null, null);
            //                }
            //                else
            //                {
            //                    property.SetValue(objTarget, CloneObject(objPropertyValue), null);
            //                    //property.SetValue(objTarget, objPropertyValue, null);
            //                }
            //            }
            //        }
            //    }
            //    return objTarget;
            //}

            public static string GetPrimaryKeyName(string tableName)
            {
                var primaryKeyName = "";
                Type edmxType = typeof(EdmScalarPropertyAttribute);
                PropertyInfo[] otraCosa = Type.GetType("ePlatBack.Models." + tableName).GetProperties();

                foreach (var pi in otraCosa)
                {
                    if (Reflection.HasCustomAttribute<EdmScalarPropertyAttribute>(pi))
                    {
                        EdmScalarPropertyAttribute cosaAtt = Reflection.GetCustomAttribute<EdmScalarPropertyAttribute>(pi);
                        if (cosaAtt.EntityKeyProperty)
                        {
                            primaryKeyName = pi.Name;
                            break;
                        }
                    }
                }
                return primaryKeyName;
            }
        }
    }

    public static class ChangesTracking
    {
        public static string ToSqlInsertValues(this List<ChangeItem> changesList, Guid userID, DateTime logDateTime, string url)
        {
            var sb = new StringBuilder();

            foreach (ChangeItem change in changesList)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }

                sb.Append("(");
                sb.Append("'" + HttpUtility.HtmlEncode(change.SysComponentID).Trim() + "',");
                sb.Append("'" + HttpUtility.HtmlEncode(change.PreviousValue).Trim() + "',");
                sb.Append("'" + HttpUtility.HtmlEncode(change.CurrentValue).Trim() + "',");
                sb.Append("'" + HttpUtility.HtmlEncode(userID).Trim() + "',");
                sb.Append("'" + HttpUtility.HtmlEncode(logDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff")).Trim() + "',");
                //sb.Append("'" + HttpUtility.HtmlEncode(change.ReferenceText).Trim() + "',");
                sb.Append("'" + HttpUtility.HtmlEncode(change.FullReferenceText).Trim() + "',");
                sb.Append("'" + HttpUtility.HtmlEncode(change.ReferenceID).Trim() + "',");
                sb.Append("'" + HttpUtility.HtmlEncode(url).Trim() + "'");
                sb.Append(")");
            }



            //return String.Concat("<Changes>", sb.ToString().Trim(charsToTrim), "</Changes>");
            return sb.ToString().Replace("\r\n", ",").Replace("\r", ",").Replace("\n", ",");
        }

        public static ChangesTracking.ChangeItem setChangeLog(int sysComponentID, string sysComponent, string previousValue, string currentValue, string referenceText, string referenceID, string url)
        {
            ChangesTracking.ChangeItem change = new ChangesTracking.ChangeItem();
            change.SysComponentID = sysComponentID;
            change.SysComponent = sysComponent;
            change.PreviousValue = previousValue;
            change.CurrentValue = currentValue;
            change.ReferenceText = referenceText;
            change.FullReferenceText = referenceText;
            change.ReferenceID = referenceID;
            change.Path = url;
            return change;
        }

        public static string ToXMLForSQL(this List<ChangeItem> changesList, Guid userID, DateTime logDateTime, string url)
        {

            var sb = new StringBuilder();
            foreach (ChangeItem change in changesList)
            {
                sb.AppendLine("<Change ");

                sb.AppendLine(" sysComponenyID=\"" + HttpUtility.HtmlEncode(change.SysComponentID) + "\" ");
                sb.AppendLine(" previousValue=\"" + HttpUtility.HtmlEncode(change.PreviousValue) + "\" ");
                sb.AppendLine("  currentValue=\"" + HttpUtility.HtmlEncode(change.CurrentValue) + "\" ");
                sb.AppendLine(" userID=\"" + HttpUtility.HtmlEncode(userID) + "\" ");
                sb.AppendLine(" logDateTime=\"" + HttpUtility.HtmlEncode(logDateTime) + "\" ");
                sb.AppendLine("  referenceText=\"" + HttpUtility.HtmlEncode(change.ReferenceText) + "\" ");
                sb.AppendLine(" referenceID=\"" + HttpUtility.HtmlEncode(change.ReferenceID) + "\" ");
                sb.AppendLine(" url=\"" + HttpUtility.HtmlEncode(url) + "\" ");

                sb.AppendLine("  />");
            }


            return String.Concat("<Changes>", sb.ToString(), "</Changes>");

        }

        public class OnValueChangedItem
        {
            public string Target;
            public object Value;
        }

        public class ChangeItem
        {
            public int SysComponentID;
            public string SysComponent;
            public string PreviousValue;
            public string CurrentValue;
            //public object CurrentValueObj;
            //public Guid UserID;
            //public DateTime LogDate;
            public string ReferenceText;
            public string FullReferenceText;
            public string ReferenceID;
            public string Path;
            public string PkName;
            public List<string> LogReferenceUiFieldNames = new List<string>();
            public List<string> LogReferenceDbFieldNames = new List<string>();



            //public string Url;
        }

        public class ObjectValuesComparisonResult
        {
            public bool ValuesAreDiferent = false;
            public string dbComparedValue = "";
            public string uiComparedValue = "";
        }

        public static ObjectValuesComparisonResult CompareUiAndDbValues(object dbValue, object uiValue, bool usesTime = false, bool isCreditCard = false)
        {
            string valor1 = "";
            string valor2 = "";

            if (dbValue == null)
            {
                valor1 = "";

                if (uiValue == null) { valor2 = ""; }
                else { valor2 = uiValue.ToString(); }

            }
            //else if(dbValue is Guid){
            // if((Guid)dbValue==Guid.Empty){
            //     valor1 = "";
            // }
            //}
            else if (dbValue is decimal)
            { //db stores values like 1.00 whereas ui uses values like 1, furthermore the comparision returns always different.
                valor1 = dbValue.ToString();
                valor2 = Convert.ToDecimal(uiValue).ToString();

                if (!valor2.Contains("."))
                {
                    valor2 += ".00";
                }

            }
            else if (dbValue is DateTime)
            {
                if (usesTime)
                {
                    valor1 = String.Format("yyyy-MM-dd  hh:mm:ss", Convert.ToDateTime(dbValue.ToString().Trim())).ToString();
                    valor1 = Convert.ToDateTime(dbValue.ToString().Trim()).ToString("yyyy-MM-dd hh:mm:ss");
                }
                else
                {
                    // valor1 = String.Format("dd/MMM/yyyy", Convert.ToDateTime(dbValue.ToString().Trim())).ToString();
                    valor1 = Convert.ToDateTime(dbValue.ToString().Trim()).ToString("yyyy-MM-dd");
                }

                if (uiValue != null && !string.IsNullOrEmpty(uiValue.ToString().Trim()))
                {
                    //valor2 = String.Format("dd/MMM/yyyy", Convert.ToDateTime(uiValue.ToString().Trim())).ToString();
                    valor2 = Convert.ToDateTime(uiValue.ToString().Trim()).ToString("yyyy-MM-dd");
                }
            }
            else if (isCreditCard)
            {

                if (dbValue == null) { valor1 = ""; } else { valor1 = dbValue.ToString(); }
                if (uiValue == null) { valor2 = ""; } else { valor2 = uiValue.ToString(); }

                if (valor2.Contains("*"))
                { // compare decrypted and masked values
                    valor1 = Utils.GeneralFunctions.MaskCreditCard(mexHash.mexHash.DecryptString(valor1));

                }
                else
                { // compare decrypted values 
                    if (dbValue != null) { valor1 = mexHash.mexHash.DecryptString(valor1); }
                    else { valor1 = ""; }
                }





            }
            else
            {
                valor1 = dbValue.ToString();
                valor2 = (uiValue != null) ? uiValue.ToString() : "";
            }

            bool valuesAreDifferent = false;
            if (string.Compare(valor1, valor2) != 0)
            {
                valuesAreDifferent = true;
            }


            if (isCreditCard && valuesAreDifferent)
            {
                //make sure we log the credit cards masked
                if (!valor1.Contains("*"))
                {
                    valor1 = Utils.GeneralFunctions.MaskCreditCard(valor1);
                    valor2 = Utils.GeneralFunctions.MaskCreditCard(valor2);
                }

            }

            return new ObjectValuesComparisonResult() { ValuesAreDiferent = valuesAreDifferent, dbComparedValue = valor1, uiComparedValue = valor2 };

        }

        public class LogReferenceObject
        {
            public string Text = "";
            public List<string> FieldUiNames = new List<string>();
            public List<string> FieldDbNames = new List<string>();
        }

        public static LogReferenceObject GetLogReference(object p, List<string> LogReferenceFields)
        {
            LogReferenceObject lro = new LogReferenceObject();
            string LogReference = "";
            foreach (var rp in LogReferenceFields)
            {
                LogReference += Reflection.GetPropertyValueByPath(p, rp) + " ";
                lro.FieldUiNames.Add(rp);
                lro.FieldDbNames.Add(rp);
            }
            lro.Text = LogReference;


            return lro;
        }

        public static LogReferenceObject GetLogReference(PropertyInfo p)
        {
            Type tipo = p.GetType();
            LogReferenceAttribute lra = (LogReferenceAttribute)Attribute.GetCustomAttribute(tipo, typeof(LogReferenceAttribute));

            string topBread = "";
            string bottomBread = "";
            if (lra != null)
            {
                topBread = lra.Name + "(";
                bottomBread = ")";
            }

            LogReferenceObject lro = new LogReferenceObject();
            ListTypeAttribute lta = Reflection.GetCustomAttribute<ListTypeAttribute>(p);



            PropertyInfo[] referenceProperties = Reflection.GetPropertiesByAtribute<LogReferenceAttribute>(lta.SingleItemType);
            string LogReference = "";

            foreach (var rp in referenceProperties)
            {
                LogReference += Reflection.GetPropertyValueByPath(p, rp.Name) + " ";
                lro.FieldUiNames.Add(rp.Name);
                lro.FieldDbNames.Add(Reflection.GetDbNameOrDefault(rp));
            }
            lro.Text = topBread + LogReference + bottomBread;
            return lro;
        }

        public static LogReferenceObject GetLogReference(object obj)
        {
            //add the object reference like this

            Type tipo = obj.GetType();
            //lead(juan perez).email( fjimenez@villagroup.com )
            LogReferenceAttribute lra = (LogReferenceAttribute)Attribute.GetCustomAttribute(tipo, typeof(LogReferenceAttribute));

            string topBread = "";
            string bottomBread = "";
            if (lra != null)
            {
                topBread = lra.Name + "(";
                bottomBread = ")";
            }



            PropertyInfo[] referenceProperties = Reflection.GetPropertiesByAtribute<LogReferenceAttribute>(obj);
            string LogReference = "";


            LogReferenceObject lro = new LogReferenceObject();
            foreach (var rp in referenceProperties)
            {
                LogReference += Reflection.GetPropertyValueByPath(obj, rp.Name) + " ";
                lro.FieldUiNames.Add(rp.Name);
                lro.FieldDbNames.Add(Reflection.GetDbNameOrDefault(rp));

            }



            lro.Text = topBread + LogReference + bottomBread;


            return lro;
        }

        //public static string GetLogReference(PropertyInfo obj)
        //{
        //    PropertyInfo[] referenceProperties = Reflection.GetPropertiesByAtribute<LogReferenceAttribute>(obj);
        //    string LogReference = "";

        //    foreach (var rp in referenceProperties)
        //    {
        //        LogReference += Reflection.GetPropertyValueByPath(obj, rp.Name) + " ";
        //    }
        //    return LogReference;
        //}

        public static IDictionary<string, int> GetSysComponentIDs(string tableName)
        {

            IDictionary<string, int> sysComponents = new Dictionary<string, int>();
            ePlatEntities db = new ePlatEntities();
            var q = from x in db.tblSysComponents where x.tableName == tableName select new { x.fieldName, x.sysComponentID };

            foreach (var c in q)
            {
                //try-catch added to prevent error on duplicated key insert
                try
                {
                    sysComponents.Add(c.fieldName, (int)c.sysComponentID);
                }
                catch { }
            }

            return sysComponents;
        }

        public static void GetMissingReferenceIDs(object returnObject, ref List<ChangeItem> changes)
        {
            foreach (var change in changes)
            {
                if (change.ReferenceID == "" || change.ReferenceID == "0")
                {//Get the property's Items
                    IEnumerable<object> value = Reflection.GetPropertyValueByPath(returnObject, change.Path) as IEnumerable<object>;
                    try
                    {

                        if (value.Count() > 1)
                        {

                            //query the items to get the one whos LOg REFERENCE matches its logReference
                            //var item = value.Where(x => Reflection.GetPropertyValueByPath(x, change.SysComponent).ToString() == change.CurrentValue).First();
                            var item = value.Where(x => ChangesTracking.GetLogReference(x, change.LogReferenceDbFieldNames).Text == change.ReferenceText).First();
                            var itemID = Reflection.GetPropertyValueByPath(item, change.PkName).ToString();
                            change.ReferenceID = itemID;
                        }
                        else
                        {
                            var item = value.First();
                            var itemID = Reflection.GetPropertyValueByPath(item, change.PkName).ToString();
                            change.ReferenceID = itemID;
                        }
                    }
                    catch (Exception excep)
                    {
                        var cosa = excep.InnerException;
                    }
                }
            }

        }

        public static void GetMissingReferenceIDs(object returnObject, List<ChangeItem> changes)
        {
            foreach (var change in changes)
            {
                if (change.ReferenceID == "" || change.ReferenceID == "0")
                {//Get the property's Items
                    IEnumerable<object> value = Reflection.GetPropertyValueByPath(returnObject, change.Path) as IEnumerable<object>;
                    try
                    {

                        if (value.Count() > 1)
                        {

                            //query the items to get the one whos LOg REFERENCE matches its logReference
                            //var item = value.Where(x => Reflection.GetPropertyValueByPath(x, change.SysComponent).ToString() == change.CurrentValue).First();
                            var item = value.Where(x => ChangesTracking.GetLogReference(x, change.LogReferenceDbFieldNames).Text == change.ReferenceText).First();
                            var itemID = Reflection.GetPropertyValueByPath(item, change.PkName).ToString();
                            change.ReferenceID = itemID;
                        }
                        else
                        {
                            var item = value.First();
                            var itemID = Reflection.GetPropertyValueByPath(item, change.PkName).ToString();
                            change.ReferenceID = itemID;
                        }
                    }
                    catch (Exception excep)
                    {
                        var cosa = excep.InnerException;
                    }
                }
            }

        }

        public static void LogChanges(List<ChangeItem> changesList)
        {

            if (changesList.Count > 0)
            {
                ePlatEntities db = new ePlatEntities();

                Guid UserID = (Guid)Membership.GetUser().ProviderUserKey;
                string Url = System.Web.HttpContext.Current.Request.Url.ToString();
                DateTime LogDate = DateTime.Now;

                string tableName = "";
                tableName += "tblUserLogs";
                tableName += LogDate.ToString("yyyyMM");

                string insertValues = changesList.ToSqlInsertValues(UserID, LogDate, Url);

                string INSERT = "INSERT INTO "
                    + tableName
                    + "(sysComponentID, previousValue, currentValue, userID, logDateTime, referenceText, referenceID, url) VALUES "
                    + insertValues;

                var tableExists = db.sp_tableExists(tableName);
                var valor = tableExists.FirstOrDefault().Value;
                if (valor == 0)
                {
                    db.sp_createUserLogsTable(tableName);
                }
                db.ExecuteStoreCommand(INSERT);
            }
        }
    }

    /// <summary>
    /// Enumerates the possible results of trying to save, edit or delete against the database.
    /// </summary>
    public enum Attempt_ResponseTypes
    {
        /// <summary>
        /// Returns 1, operation completed successfully.
        /// </summary>
        Ok = 1,
        /// <summary>
        /// Returns 0, operation completed with warnings.
        /// </summary>
        Warning = 0,
        /// <summary>
        /// Returns -1, the operation could not be completed.
        /// </summary>
        Error = -1
    }

    public enum DataBaseRelationShipCardinality
    {
        OneToOne = 1,
        OneToMany = 2,
        ManyToMany = 3
    }

    public enum AutomaticValueTypes
    {
        DateStamp = 1,
        DateTimeStamp = 2,
        CurrentUser = 3,
        CurrentValue = 4
    }

    public enum OnValueChangedActions
    {
        Update = 1
    }

    /// <summary>
    /// Tells whether the try succeeded or not, the identifier of the object in turn and the error message if thrown.
    /// </summary>
    public class AttemptResponse
    {
        public Attempt_ResponseTypes Type = Attempt_ResponseTypes.Warning; // Ok, Error or Warning
        //public string ErrorMessage = ""; // The exception message
        public Exception Exception = null;
        public object ObjectID = null; //  Declared as object so we can handle any type of value.
        public object Object = null;
        public string Message = ""; // Custom message
    }

    public class _Debugging
    {
        public string GetErrorLocation(Exception EX)
        {
            return Debugging.GetErrorLocation(EX);
            //try
            //{
            //    string errorlocation = "";

            //    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(EX, true);


            //    Assembly assembly;
            //    MethodBase method;
            //    string fileName = "";
            //    string actionName = "";
            //    string lineNumber = "";
            //    string column = "";
            //    string parentNamespace = "";
            //    string declaringType = "";
            //    string assemblyError = "";

            //    if (trace != null)
            //    {
            //        //---
            //        try
            //        {
            //            fileName = trace.GetFrame(trace.FrameCount - 1).GetFileName();
            //        }
            //        catch (Exception filenameEx)
            //        {
            //            fileName = "Error Getting filename: " + filenameEx.Message;
            //        }
            //        //---

            //        try
            //        {
            //            lineNumber = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber().ToString();
            //        }
            //        catch (Exception lineNumberEx)
            //        {
            //            lineNumber = "Error Getting linenumber: " + lineNumberEx.Message;
            //        }
            //        //---

            //        try
            //        {
            //            column = trace.GetFrame(trace.FrameCount - 1).GetFileColumnNumber().ToString();
            //        }
            //        catch (Exception columnEx)
            //        {
            //            column = "Error Getting column: " + columnEx.Message;
            //        }
            //        //---

            //        try
            //        {
            //            method = trace.GetFrame(trace.FrameCount - 1).GetMethod();
            //        }
            //        catch (Exception)
            //        {
            //            method = null;
            //        }
            //        //---

            //        if (method != null)
            //        {

            //            //try
            //            //{
            //            //    assembly = method.DeclaringType.Assembly;
            //            //}
            //            //catch (Exception assemblyEx)
            //            //{
            //            //    assemblyError = "Error Getting assembly: " + assemblyEx.Message;
            //            //}
            //            //---

            //            try
            //            {
            //                actionName = method.Name;
            //            }
            //            catch (Exception actionNameEx)
            //            {
            //                actionName = "Error Getting actionName: " + actionNameEx.Message;

            //            }
            //            //---
            //            try
            //            {
            //                parentNamespace = method.DeclaringType.Namespace;
            //            }
            //            catch (Exception parentNameSpaceEx)
            //            {
            //                parentNamespace = "Error Getting parentNamespace: " + parentNameSpaceEx.Message;
            //            }
            //            //---
            //            try
            //            {
            //                declaringType = method.DeclaringType.ToString();
            //            }
            //            catch (Exception declaringTypeEx)
            //            {
            //                declaringType = "Error Getting declaringType: " + declaringTypeEx.Message;
            //            }

            //        }

            //    }




            //    errorlocation += "<b>Filename:</b> '" + fileName.Replace(" ", "&nbsp;") + "'<br />";
            //    //errorlocation += "<b>Assembly:</b> '" + ((assemblyError != "") ? assemblyError : assembly.FullName.Replace(" ", "&nbsp;")) + "'<br />";
            //    errorlocation += "<b>Namespace:</b> '" + parentNamespace.Replace(" ", "&nbsp;") + "'<br />";
            //    errorlocation += "<b>Class:</b> '" + declaringType + "'<br />";
            //    errorlocation += "<b>Actionname:</b> '" + actionName + "'<br />";
            //    errorlocation += "<b>Linenumber:</b> '" + lineNumber + "'<br />";
            //    errorlocation += "<b>Column:</b> '" + column + "'<br />";

            //    return errorlocation + "<br />";
            //}
            //catch (Exception all)
            //{
            //    throw new Exception("GetErrorLocationInternalError => " + all.Message, null);
            //}
        }
    }

    public static class Debugging
    {
        public static string GetErrorLocation(Exception EX)
        {
            string errorlocation = "";
            try
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(EX, true);


                Assembly assembly;
                MethodBase method;
                string fileName = "";
                string actionName = "";
                string lineNumber = "";
                string column = "";
                string parentNamespace = "";
                string declaringType = "";
                string assemblyError = "";

                if (trace != null)
                {
                    //---
                    try
                    {
                        fileName = trace.GetFrame(trace.FrameCount - 1).GetFileName();
                    }
                    catch (Exception filenameEx)
                    {
                        fileName = "Error Getting filename: " + filenameEx.Message;
                    }
                    //---

                    try
                    {
                        lineNumber = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber().ToString();
                    }
                    catch (Exception lineNumberEx)
                    {
                        lineNumber = "Error Getting linenumber: " + lineNumberEx.Message;
                    }
                    //---

                    try
                    {
                        column = trace.GetFrame(trace.FrameCount - 1).GetFileColumnNumber().ToString();
                    }
                    catch (Exception columnEx)
                    {
                        column = "Error Getting column: " + columnEx.Message;
                    }
                    //---

                    try
                    {
                        method = trace.GetFrame(trace.FrameCount - 1).GetMethod();
                    }
                    catch (Exception)
                    {
                        method = null;
                    }
                    //---

                    if (method != null)
                    {

                        try
                        {
                            assembly = method.DeclaringType.Assembly;
                        }
                        catch (Exception assemblyEx)
                        {
                            assemblyError = "Error Getting assembly: " + assemblyEx.Message;
                        }
                        //---

                        try
                        {
                            actionName = method.Name;
                        }
                        catch (Exception actionNameEx)
                        {
                            actionName = "Error Getting actionName: " + actionNameEx.Message;

                        }
                        //---
                        try
                        {
                            parentNamespace = method.DeclaringType.Namespace;
                        }
                        catch (Exception parentNameSpaceEx)
                        {
                            parentNamespace = "Error Getting parentNamespace: " + parentNameSpaceEx.Message;
                        }
                        //---
                        try
                        {
                            declaringType = method.DeclaringType.ToString();
                        }
                        catch (Exception declaringTypeEx)
                        {
                            declaringType = "Error Getting declaringType: " + declaringTypeEx.Message;
                        }

                    }

                }




                errorlocation += "<b>Filename:</b> '" + fileName.Replace(" ", "&nbsp;") + "'<br />";
                //errorlocation += "<b>Assembly:</b> '" + ((assemblyError != "") ? assemblyError : assembly.FullName.Replace(" ", "&nbsp;")) + "'<br />";
                errorlocation += "<b>Namespace:</b> '" + parentNamespace.Replace(" ", "&nbsp;") + "'<br />";
                errorlocation += "<b>Class:</b> '" + declaringType + "'<br />";
                errorlocation += "<b>Actionname:</b> '" + actionName + "'<br />";
                errorlocation += "<b>Linenumber:</b> '" + lineNumber + "'<br />";
                errorlocation += "<b>Column:</b> '" + column + "'<br />";

                return errorlocation + "<br />";
            }
            catch (Exception all)
            {
                //throw new Exception("GetErrorLocationInternalError => " + all.Message, null);
                return errorlocation += "GetErrorLocationInternalError => " + all.Message;
            }
        }
        //public static string GetErrorLocation(Exception EX)     
        //{
        //    try
        //    {


        //    string errorlocation = "";

        //    System.Diagnostics.StackTrace trace=null;

        //    try{
        //        trace= new System.Diagnostics.StackTrace(EX, true);
        //    }
        //    catch (Exception) { ;}




        //    Assembly assembly = null;
        //    MethodBase method = null;                
        //    string fileName="";
        //    string actionName="";
        //    string lineNumber="";
        //    string column="";
        //    string parentNamespace="";
        //    string declaringType="";
        //    string assemblyError = "";

        //    if(trace != null){
        //        //---
        //        try{
        //            fileName = trace.GetFrame(trace.FrameCount - 1).GetFileName();
        //        }
        //        catch (Exception filenameEx){
        //            fileName = "Error Getting filename: " + filenameEx.Message;
        //        }
        //        //---

        //        try{
        //                lineNumber = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber().ToString();
        //        }
        //        catch (Exception lineNumberEx){
        //            lineNumber = "Error Getting linenumber: " + lineNumberEx.Message;
        //        }
        //        //---

        //        try{
        //            column = trace.GetFrame(trace.FrameCount - 1).GetFileColumnNumber().ToString();
        //        }
        //        catch (Exception columnEx){
        //            column = "Error Getting column: " + columnEx.Message;
        //        }
        //        //---

        //        try{
        //            method = trace.GetFrame(trace.FrameCount - 1).GetMethod();
        //        }
        //        catch (Exception){
        //            method = null;
        //        }
        //        //---

        //        if(method !=null){

        //            try{
        //                assembly = method.DeclaringType.Assembly;
        //            }
        //            catch (Exception assemblyEx){
        //                assemblyError = "Error Getting assembly: " + assemblyEx.Message; 
        //            }
        //            //---

        //            try{
        //                actionName = method.Name;
        //            }
        //            catch (Exception actionNameEx){
        //                actionName = "Error Getting actionName: " + actionNameEx.Message;

        //            }
        //            //---
        //            try{
        //                parentNamespace = method.DeclaringType.Namespace;
        //            }
        //            catch (Exception parentNameSpaceEx){
        //                parentNamespace = "Error Getting parentNamespace: " + parentNameSpaceEx.Message;                     
        //            }
        //            //---
        //            try{
        //                declaringType = method.DeclaringType.ToString();
        //            }
        //            catch (Exception declaringTypeEx){
        //                declaringType = "Error Getting declaringType: " + declaringTypeEx.Message;                        
        //            }

        //        }

        //    }




        //    errorlocation += "<b>Filename:</b> '" + fileName.Replace(" ","&nbsp;") + "'<br />";
        //    errorlocation += "<b>Assembly:</b> '" + ((assemblyError != "") ? assemblyError : assembly.FullName.Replace(" ", "&nbsp;")) + "'<br />";
        //    errorlocation += "<b>Namespace:</b> '" + parentNamespace.Replace(" ","&nbsp;") + "'<br />";
        //    errorlocation += "<b>Class:</b> '" + declaringType + "'<br />";
        //    errorlocation += "<b>Actionname:</b> '" + actionName + "'<br />";
        //    errorlocation += "<b>Linenumber:</b> '" + lineNumber + "'<br />";
        //    errorlocation += "<b>Column:</b> '" + column + "'<br />";

        //    return errorlocation + "<br />";
        //    }
        //    catch (Exception all){
        //        throw new Exception("GetErrorLocationInternalError => " + all.Message, null);
        //    }
        //}
        public static string GetMessage(Exception EX)
        {
            return EX != null && EX.Message != null ? EX.Message.ToString() : "";
        }
        public static string GetInnerException(Exception EX)
        {
            return EX != null && EX.InnerException != null ? EX.InnerException.ToString() : "";
        }
    }



    public class EmailNotifications
    {

        public static KeyValuePair<bool, string> SendEmail(System.Net.Mail.MailMessage m, long? terminalID = null, Guid? userID = null, Boolean async = false)
        {
            ePlatEntities db = new ePlatEntities();

            try
            {
                #region "replace special caracters"
                m.Body = m.Body.Replace("á", "&aacute;")
                    .Replace("é", "&eacute;")
                    .Replace("í", "&iacute;")
                    .Replace("ó", "&oacute;")
                    .Replace("ú", "&uacute;")
                    .Replace("ñ", "&ntilde;")
                    .Replace("Á", "&Aacute;")
                    .Replace("É", "&Eacute;")
                    .Replace("Í", "&Iacute;")
                    .Replace("Ó", "&Oacute;")
                    .Replace("Ú", "&Uacute;")
                    .Replace("Ñ", "&Ntilde;");
                #endregion

                var query = from ms in db.tblMailingSettings
                            where ((userID == null && ms.userID == null) || ms.userID == userID)
                            && ((terminalID == null && ms.terminalID == null) || ms.terminalID == terminalID)
                            && ms.active != false
                            select ms;
                long id = 0;
                var highest = query.OrderByDescending(x => x.mailingSettingsID).FirstOrDefault().mailingSettingsID;
                if (async)
                {
                    try
                    {
                        id = long.Parse(System.Web.HttpContext.Current.Application["senderAccount"].ToString());//settings id
                        tblMailingSettings settings = null;
                        while (settings == null && id <= highest)
                        {
                            settings = query.FirstOrDefault(x => x.mailingSettingsID == id);
                            id++;
                        }

                        var smtp = new System.Net.Mail.SmtpClient();
                        smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                        smtp.EnableSsl = settings.smtpSsl;
                        smtp.Host = settings.smtpServer;
                        smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;

                        SendEmailDelegate sd = new SendEmailDelegate(smtp.Send);
                        AsyncCallback cb = new AsyncCallback(SendEmailResponse);
                        sd.BeginInvoke(m, cb, sd);

                        if (id > query.OrderByDescending(x => x.mailingSettingsID).FirstOrDefault().mailingSettingsID)
                        {
                            id = 1;
                        }
                        System.Web.HttpContext.Current.Application["senderAccount"] = id;
                    }
                    catch
                    {
                        query.FirstOrDefault(x => x.mailingSettingsID == id - 1).active = false;
                        db.SaveChanges();
                    }
                }
                else
                {
                    try
                    {
                        System.Web.HttpContext.Current.Application.Lock();
                        id = long.Parse(System.Web.HttpContext.Current.Application["senderAccount"].ToString());//settings id
                        tblMailingSettings settings = null;
                        while (settings == null && id <= highest)
                        {
                            settings = query.FirstOrDefault(x => x.mailingSettingsID == id);
                            id++;
                        }

                        using (var email = m)
                        {
                            using (var smtp = new System.Net.Mail.SmtpClient())
                            {
                                smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                                smtp.EnableSsl = settings.smtpSsl;
                                smtp.Host = settings.smtpServer;
                                smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;
                                smtp.Send(m);
                            }
                        }

                        if (id > query.OrderByDescending(x => x.mailingSettingsID).FirstOrDefault().mailingSettingsID)
                        {
                            id = 1;
                        }
                        System.Web.HttpContext.Current.Application["senderAccount"] = id;
                        System.Web.HttpContext.Current.Application.UnLock();
                    }
                    catch
                    {
                        System.Web.HttpContext.Current.Application.UnLock();
                        query.FirstOrDefault(x => x.mailingSettingsID == id - 1).active = false;
                        db.SaveChanges();
                    }
                }

                //System.Web.HttpContext.Current.Application.Lock();
                //var account = System.Web.HttpContext.Current.Application["senderAccount"].ToString();

                //if (async)
                //{
                //    var smtp = new System.Net.Mail.SmtpClient();
                //    smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                //    smtp.EnableSsl = settings.smtpSsl;
                //    smtp.Host = settings.smtpServer;
                //    smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;

                //    SendEmailDelegate sd = new SendEmailDelegate(smtp.Send);
                //    AsyncCallback cb = new AsyncCallback(SendEmailResponse);
                //    sd.BeginInvoke(m, cb, sd);
                //}
                //else
                //{
                //    using (var email = m)
                //    {
                //        using (var smtp = new System.Net.Mail.SmtpClient())
                //        {
                //            smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                //            smtp.EnableSsl = settings.smtpSsl;
                //            smtp.Host = settings.smtpServer;
                //            smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;
                //            smtp.Send(m);
                //        }
                //    }
                //}
                //var nextAccount = int.Parse(account) + 1;

                //System.Web.HttpContext.Current.Application["senderAccount"] = nextAccount > 6 ? 3 : nextAccount;
                //System.Web.HttpContext.Current.Application.UnLock();
            }
            catch (System.Net.Mail.SmtpFailedRecipientException ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
            return new KeyValuePair<bool, string>(true, "");
        }

        public static KeyValuePair<bool, string> _SendEmail(System.Net.Mail.MailMessage m, long? terminalID = null, Guid? userID = null, Boolean async = false)
        {
            ePlatEntities db = new ePlatEntities();

            try
            {
                #region "replace special caracters"
                m.Body = m.Body.Replace("á", "&aacute;")
                    .Replace("é", "&eacute;")
                    .Replace("í", "&iacute;")
                    .Replace("ó", "&oacute;")
                    .Replace("ú", "&uacute;")
                    .Replace("ñ", "&ntilde;")
                    .Replace("Á", "&Aacute;")
                    .Replace("É", "&Eacute;")
                    .Replace("Í", "&Iacute;")
                    .Replace("Ó", "&Oacute;")
                    .Replace("Ú", "&Uacute;")
                    .Replace("Ñ", "&Ntilde;");
                #endregion

                var query = from ms in db.tblMailingSettings
                            where ((userID == null && ms.userID == null) || ms.userID == userID)
                            && ((terminalID == null && ms.terminalID == null) || ms.terminalID == terminalID)
                            select ms;

                //var settings = query?.FirstOrDefault();
                System.Web.HttpContext.Current.Application.Lock();
                var account = System.Web.HttpContext.Current.Application["senderAccount"].ToString();
                var settings = query.FirstOrDefault(x => x.smtpUsername.IndexOf(account) != -1);

                if (async)
                {
                    var smtp = new System.Net.Mail.SmtpClient();
                    smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                    smtp.EnableSsl = settings.smtpSsl;
                    smtp.Host = settings.smtpServer;
                    smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;

                    SendEmailDelegate sd = new SendEmailDelegate(smtp.Send);
                    AsyncCallback cb = new AsyncCallback(SendEmailResponse);
                    sd.BeginInvoke(m, cb, sd);
                }
                else
                {
                    using (var email = m)
                    {
                        using (var smtp = new System.Net.Mail.SmtpClient())
                        {
                            smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                            smtp.EnableSsl = settings.smtpSsl;
                            smtp.Host = settings.smtpServer;
                            smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;
                            smtp.Send(m);
                        }
                    }
                }
                var nextAccount = int.Parse(account) + 1;
                //System.Web.HttpContext.Current.Application["senderAccount"] = nextAccount > 6 ? 2 : nextAccount;
                System.Web.HttpContext.Current.Application["senderAccount"] = nextAccount > 6 ? 3 : nextAccount;
                System.Web.HttpContext.Current.Application.UnLock();
                //var trackID = Guid.NewGuid();
                //SaveEmailSendLog(emailNotificationID, sysItemType, sysItemID, trackID);
            }
            catch (System.Net.Mail.SmtpFailedRecipientException ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
            return new KeyValuePair<bool, string>(true, "");
        }

        public static KeyValuePair<bool, string> SendEmail(System.Net.Mail.MailMessage m, int eventID, List<long?> terminalID, string sysItemType = null, Guid? sysItemID = null, Guid? userID = null, string culture = "en-US", Boolean async = false)
        {
            ePlatEntities db = new ePlatEntities();

            var notification = (from n in db.tblEmailNotifications
                                where n.eventID == eventID && terminalID.Contains(n.terminalID)
                                select n).FirstOrDefault();

            return SendEmail(m, notification.emailNotificationID, terminalID, sysItemType, sysItemID, userID, culture, async);
        }

        public static void Send(System.Net.Mail.MailMessage m)
        {
            //new line
            var list = new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = m } };
            //Send(m, true);
            Send(list, true);
        }

        public static void Send(System.Net.Mail.MailMessage m, Boolean Async)
        {
            //new
            var list = new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = m } };
            //SendEmail(m, null, null, Async);
            Send(list, Async);
        }

        public static KeyValuePair<bool, string> SendSync(System.Net.Mail.MailMessage m)
        {
            //new
            var list = new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = m } };
            //return SendEmail(m);
            var response = Send(list);
            return new KeyValuePair<bool, string>();
        }

        public static List<MailMessageResponse> Send(List<MailMessageResponse> list, Boolean async = false, long? terminalID = null, Guid? userID = null)
        {
            ePlatEntities db = new ePlatEntities();
            List<tblMailingSettings> query = new List<tblMailingSettings>();

            var emails = list.Select(m => m.MailMessage).ToList();
            var senders = emails.Select(m => m.From.Address).ToList();
            var domains = senders.Select(m => m.Split('@').Last()).ToList();

            var mailingAccounts = (from mad in db.tblMailingAccountDomains
                             join ma in db.tblMailingAccount on mad.mailingAccountID equals ma.mailingAccountID
                             where domains.Contains(mad.domain)
                             select mad.mailingAccountID).ToList();

            query = (from ms in db.tblMailingSettings
                     where ((userID == null && ms.userID == null) || ms.userID == userID)
                     && ((terminalID == null && ms.terminalID == null) || ms.terminalID == terminalID)
                     && ms.active != false
                     && ms.blastSender == true
                     && mailingAccounts.Contains(ms.mailingAccountID.Value)
                     select ms).ToList();

            if(query.Count() == 0)
            {
                var defaultMailingAcounts = new List<int?>() { 4, 5 };
                query = db.tblMailingSettings.Where(m => defaultMailingAcounts.Contains(m.mailingAccountID) && m.blastSender == true && m.active == true).ToList();

            }

            long id = 0;
            var highest = query.OrderByDescending(x => x.mailingSettingsID).FirstOrDefault().mailingSettingsID;
            //async = list.Count() == 1 ? true : async;//force async sendin gif list has only one email

            foreach (var item in list)
            {
                System.Net.Mail.MailMessage m = item.MailMessage;
                tblMailingSettings settings = null;

                #region "replace special caracters"
                m.Body = m.Body.Replace("á", "&aacute;")
                    .Replace("é", "&eacute;")
                    .Replace("í", "&iacute;")
                    .Replace("ó", "&oacute;")
                    .Replace("ú", "&uacute;")
                    .Replace("ñ", "&ntilde;")
                    .Replace("Á", "&Aacute;")
                    .Replace("É", "&Eacute;")
                    .Replace("Í", "&Iacute;")
                    .Replace("Ó", "&Oacute;")
                    .Replace("Ú", "&Uacute;")
                    .Replace("Ñ", "&Ntilde;");
                #endregion

                var _addresses = new System.Net.Mail.MailAddressCollection();
                if (m.To.Count() > 0)
                {
                    _addresses.Add(string.Join(",", m.To.Select(x => x.Address)));
                }
                foreach (var i in _addresses)
                {
                    var address = i.Address.ToLower();
                    var leadReceiver = db.tblLeadEmails.Where(x => x.email.ToLower() == address && x.bounced == true);
                    var userReceiver = db.aspnet_Users.Where(x => x.UserName.ToLower() == address && x.aspnet_Membership.IsLockedOut == true);
                    if (leadReceiver.Count() > 0 || userReceiver.Count() > 0)
                    {
                        m.To.Remove(i);
                    }
                }

                if (async)
                {
                    try
                    {
                        System.Web.HttpContext.Current.Application.Lock();
                        //id = long.Parse(System.Web.HttpContext.Current.Application["senderAccount"].ToString());//settings id
                        //while (settings == null && id <= highest)
                        //{
                        //    var _settings = query.FirstOrDefault(x => x.mailingSettingsID == id);
                        //    if (_settings != null && _settings.dailyCounter <= 2000)
                        //    {
                        //        settings = _settings;
                        //    }
                        //    id++;
                        //}
                        settings = query.OrderBy(x => x.dailyCounter).FirstOrDefault();

                        var smtp = new System.Net.Mail.SmtpClient();
                        smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                        smtp.EnableSsl = settings.smtpSsl;
                        smtp.Host = settings.smtpServer;
                        smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;
                        SendEmailDelegate sd = new SendEmailDelegate(smtp.Send);
                        AsyncCallback cb = new AsyncCallback(SendEmailResponse);
                        sd.BeginInvoke(m, cb, sd);

                        item.Sent = true;
                        settings.dailyCounter++;
                        db.SaveChanges();

                        //if (id > query.OrderByDescending(x => x.mailingSettingsID).FirstOrDefault().mailingSettingsID)
                        //{
                        //    id = 1;
                        //}
                        //System.Web.HttpContext.Current.Application["senderAccount"] = id;
                        System.Web.HttpContext.Current.Application.UnLock();
                    }
                    catch (Exception ex)
                    {
                        var _id = id - 1;
                        query.FirstOrDefault(x => x.mailingSettingsID == _id).active = false;

                        if (settings != null)
                        {
                            settings.active = false;
                            db.SaveChanges();
                        }

                        item.Sent = false;
                        item.Exception = ex.Message;
                    }
                }
                else
                {
                    try
                    {
                        System.Web.HttpContext.Current.Application.Lock();
                        //id = System.Web.HttpContext.Current.Application["settings"];
                        //settings = query.OrderBy(x => x.dailyCounter).FirstOrDefault();
                        //var app = System.Web.HttpContext.Current.Application["senderAccount"] as List<GenericListItem>;
                        //var _app = app.FirstOrDefault(x => int.Parse(x.Property1) == query.FirstOrDefault().mailingAccountID);
                        
                        

                        //while(settings == null)
                        //{
                        //    var _settings = query.FirstOrDefault(x => x.mailingSettingsID == long.Parse(_app.Property2));
                        //    if(_settings != null && _settings.dailyCounter<= 400)
                        //    {
                        //        settings = _settings;
                        //    }
                        //}

                        
                        var counter = query.Count()-1;
                        query = query.OrderByDescending(x => x.dailyCounter).ToList();
                        while (item.Sent != true && counter >= 0)
                        {
                            settings = query.ElementAt<tblMailingSettings>(counter);
                            using (var email = m)
                            {
                                using (var smtp = new System.Net.Mail.SmtpClient())
                                {
                                    try
                                    {
                                        smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                                        smtp.EnableSsl = settings.smtpSsl;
                                        smtp.Host = settings.smtpServer;
                                        smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;
                                        smtp.Send(m);
                                        item.Sent = true;
                                        settings.dailyCounter++;
                                        db.SaveChanges();
                                    }
                                    catch(Exception ex)
                                    {

                                    }
                                }
                            }
                            counter--;
                        }
                        

                        //var localApp = System.Web.HttpContext.Current.Application["senderAccount"] as List<SelectListItem>;//mailingAccount, mailingSettings
                        //localApp = localApp != null ? localApp : new List<SelectListItem>();
                        //var currentApp = localApp.Count() > 0 ? localApp.FirstOrDefault(x => mailingAccounts.Contains(int.Parse(x.Value))) : new SelectListItem() { Value = "0", Text = "0"};

                        //id = long.Parse(System.Web.HttpContext.Current.Application["senderAccount"].ToString());//settings id
                        //while (settings == null && id <= highest)
                        //while (settings == null)
                        //{
                        //    var _settings = query.FirstOrDefault(x => x.mailingSettingsID == id);
                        //    if (_settings != null && _settings.dailyCounter <= 2000)
                        //    {
                        //        settings = _settings;
                        //    }
                        //    id++;
                        //}


                        //settings = query.OrderBy(x => x.dailyCounter).FirstOrDefault();
                        
                        
                        
                        //if (currentApp.Value == "0")
                        //{
                        //    settings = query.OrderByDescending(x => x.dailyCounter).FirstOrDefault();
                        //}
                        //else
                        //{
                        //    settings = query.FirstOrDefault(x => long.Parse(currentApp.Text) == x.mailingSettingsID);
                        //}

                        //using (var email = m)
                        //{
                        //    using (var smtp = new System.Net.Mail.SmtpClient())
                        //    {
                        //        smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                        //        //smtp.Credentials = new NetworkCredential("noreply@amazing-mexico.com", "EP06am02xi2020");
                        //        smtp.EnableSsl = settings.smtpSsl;
                        //        smtp.Host = settings.smtpServer;
                        //        smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;
                        //        //m.Sender = new System.Net.Mail.MailAddress(settings.smtpUsername);
                        //        //m.Sender = new System.Net.Mail.MailAddress("noreply@amazing-mexico.com");
                        //        smtp.Send(m);
                        //    }
                        //}

                        //item.Sent = true;
                        //settings.dailyCounter++;
                        //db.SaveChanges();

                        //if (id > highest)
                        //{
                        //    id = 1;
                        //}
                        //System.Web.HttpContext.Current.Application["senderAccount"] = id;
                        //if (localApp.Count(x => int.Parse(x.Value) == settings.mailingAccountID) > 0)
                        //{
                        //    localApp.FirstOrDefault(x => int.Parse(x.Value) == settings.mailingAccountID).Text = settings.mailingSettingsID.ToString();
                        //    //localApp[settings.mailingAccountID] = settings.mailingSettingsID;
                        //}
                        //else
                        //{
                        //    localApp.Add(new SelectListItem() { Value = settings.mailingAccountID.ToString(), Text = settings.mailingSettingsID.ToString() });
                        //}
                        //System.Web.HttpContext.Current.Application["senderAccount"] = localApp;



                        //new GenericListItem() { Property1 = settings.mailingAccountID.ToString(), Property2 = settings.mailingSettingsID.ToString(), Property3 = settings.dailyCounter.ToString() }
                        System.Web.HttpContext.Current.Application.UnLock();
                    }
                    catch (Exception ex)
                    {
                        System.Web.HttpContext.Current.Application.UnLock();
                        var _id = id - 1;
                        query.FirstOrDefault(x => x.mailingSettingsID == _id).active = false;
                        if (settings != null)
                        {
                            settings.active = false;
                            db.SaveChanges();
                        }

                        item.Sent = false;
                        item.Exception = ex.Message;
                    }
                }
            }
            return list;
        }

        private delegate void SendEmailDelegate(System.Net.Mail.MailMessage m);

        private static void SendEmailResponse(IAsyncResult ar)
        {
            try
            {
                SendEmailDelegate sd = (SendEmailDelegate)(ar.AsyncState);

                sd.EndInvoke(ar);
            }
            catch (Exception ex) { }
        }

        public static System.Net.Mail.MailMessage GetEmail(long eventID)
        {
            return GetEmail(eventID, null, null);
        }

        public static System.Net.Mail.MailMessage GetEmail(long eventID, long? terminal, string culture, int? pointOfSale = null)
        {
            System.Net.Mail.MailMessage emailObj = null;
            long terminalID = terminal != null ? (long)terminal : Utils.GeneralFunctions.GetTerminalID();
            string _culture = culture != null ? culture : Utils.GeneralFunctions.GetCulture();
            ePlatEntities db = new ePlatEntities();

            var _email = from e in db.tblEmailNotifications
                             //join ps in db.tblEmailNotifications_PointsOfSale on e.emailNotificationID equals ps.emailNotificationID into e_ps
                             //from ps in e_ps.DefaultIfEmpty()
                         where e.eventID == eventID
                         && e.terminalID == terminalID
                         && e.tblEmails.culture == _culture
                         && e.tblEmails.tblEmailTemplates.culture == _culture
                         //&& (pointOfSale == null || e.tblEmailNotifications_PointsOfSale.FirstOrDefault(m => m.active).pointOfSaleID == pointOfSale)
                         //&& (e.tblEmailNotifications_PointsOfSale.FirstOrDefault(m => m.active).pointOfSaleID == pointOfSale || e.tblEmailNotifications_PointsOfSale.Count(m => m.active && m.pointOfSaleID == pointOfSale) == 0)
                         select e;

            //var email = _email.Count(m => m.tblEmailNotifications_PointsOfSale.FirstOrDefault(x => x.active && x.pointOfSaleID == pointOfSale).emailNotificationID != null) > 0 ? _email.FirstOrDefault(m => m.tblEmailNotifications_PointsOfSale.FirstOrDefault(x => x.active).pointOfSaleID == pointOfSale) : _email.FirstOrDefault(m => m.tblEmailNotifications_PointsOfSale.Count() == 0);
            var email = _email.Count(m => m.tblEmailNotifications_PointsOfSale.FirstOrDefault(x => x.active && x.pointOfSaleID == pointOfSale).emailNotificationID != null) > 0 ? _email.FirstOrDefault(m => m.tblEmailNotifications_PointsOfSale.FirstOrDefault(x => x.active && x.pointOfSaleID == pointOfSale).emailNotification_pointOfSaleID != null) : _email.FirstOrDefault(m => m.tblEmailNotifications_PointsOfSale.Count() == 0);

            if (email != null)
            {
                emailObj = new System.Net.Mail.MailMessage();
                emailObj.From = new System.Net.Mail.MailAddress(email.tblEmails.sender, email.tblEmails.alias);
                if (email.replyTo != null && email.replyTo != "")
                {
                    emailObj.Headers.Add("Reply-To", email.replyTo);
                }
                string[] Bccs = email.emailAccounts.Trim(',').Split(',');
                if (Bccs.Length > 0)
                {
                    foreach (string bcc in Bccs)
                    {
                        if (bcc != "")
                        {
                            emailObj.Bcc.Add(bcc);
                        }
                    }
                }
                emailObj.Subject = email.tblEmails.subject;
                emailObj.Body = email.tblEmails.tblEmailTemplates.htmlTemplate.Replace("$Content", email.tblEmails.content_);
                emailObj.IsBodyHtml = true;
                emailObj.Priority = System.Net.Mail.MailPriority.Normal;
            }

            return emailObj;
        }

        public static System.Net.Mail.MailMessage GetEmailByNotification(int emailNotificationID)
        {
            System.Net.Mail.MailMessage emailObj = null;

            ePlatEntities db = new ePlatEntities();

            var email = db.tblEmailNotifications.Single(m => m.emailNotificationID == emailNotificationID);

            if (email != null)
            {
                emailObj = new System.Net.Mail.MailMessage();
                emailObj.From = new System.Net.Mail.MailAddress(email.tblEmails.sender, email.tblEmails.alias);
                string[] Bccs = email.emailAccounts != null ? email.emailAccounts.Trim(',').Split(',') : new string[] { };
                if (Bccs.Length > 0)
                {
                    foreach (string bcc in Bccs)
                    {
                        emailObj.Bcc.Add(bcc);
                    }
                }
                emailObj.Subject = email.tblEmails.subject;
                emailObj.Body = email.tblEmails.tblEmailTemplates.htmlTemplate.Replace("$Content", email.tblEmails.content_);
                emailObj.IsBodyHtml = true;
                emailObj.Priority = System.Net.Mail.MailPriority.Normal;
            }

            return emailObj;
        }

        public static System.Net.Mail.MailMessage GetSystemEmail(string bodyMail, string emailAddress = null)
        {
            ePlatEntities db = new ePlatEntities();
            System.Net.Mail.MailMessage emailObj = null;

            var email = (from e in db.tblEmailNotifications
                         where e.eventID == 18
                         && e.terminalID == null
                         && e.tblEmails.culture == "en-US"
                         && e.tblEmails.tblEmailTemplates.culture == "en-US"
                         select e).FirstOrDefault();

            if (email != null)
            {
                emailObj = new System.Net.Mail.MailMessage();
                emailObj.From = new System.Net.Mail.MailAddress(email.tblEmails.sender, email.tblEmails.alias);
                if (emailAddress == null)
                {
                    string[] Bccs = email.emailAccounts.Trim(',').Split(',');
                    if (Bccs.Length > 0)
                    {
                        foreach (string bcc in Bccs)
                        {
                            emailObj.Bcc.Add(bcc);
                        }
                        emailObj.To.Add(email.emailAccounts);
                    }
                }
                else
                {
                    emailObj.To.Add(emailAddress);
                }
                emailObj.Subject = email.tblEmails.subject;
                emailObj.Body = email.tblEmails.tblEmailTemplates.htmlTemplate.Replace("$Content", email.tblEmails.content_);
                emailObj.Body = emailObj.Body.Replace("$BodyMail", bodyMail);
                emailObj.IsBodyHtml = true;
                emailObj.Priority = System.Net.Mail.MailPriority.Normal;
            }

            return emailObj;
        }
        public static string InsertTrackerEmailLog(string bodyMail, string transactionID/*, string idType, string objecTID*/)
        {
            //identify anchors
            var anchors = Regex.Matches(bodyMail, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);
            string originalBody = bodyMail;
            foreach (Match match in anchors)
            {
                var itemOriginal = match.Groups[1].Value;
                var item = match.Groups[1].Value;
                Match i = Regex.Match(item, @"href=\""(.*?)\""", RegexOptions.Singleline);
                if (i.Success)
                {
                    if (i.Groups[1].Value.IndexOf("mailto") == -1)
                    {
                        item = item.Replace("href=\"", "href=\"https://eplat.villagroup.com/public/getinvitationinfo?i=" + transactionID + "&s=37&u=");
                        bodyMail = bodyMail.Replace(itemOriginal, item);
                    }
                }
            }
            //SAVE LOG EMAIL WITHOUT TRACKING DETAILS
            var track = "\"https://eplat.villagroup.com/public/GetImage?i=" + transactionID + '"';
            bodyMail += "<img src=" + track + "style=\"position:absolute;z-index:-10; opacity:0;\"/>";
            return bodyMail;
        }
        public static string InsertTracker(string bodyMail, string transactionID)
        {
            //identify anchors
            var anchors = Regex.Matches(bodyMail, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

            foreach (Match match in anchors)
            {
                var itemOriginal = match.Groups[1].Value;
                var item = match.Groups[1].Value;
                Match i = Regex.Match(item, @"href=\""(.*?)\""", RegexOptions.Singleline);
                if (i.Success)
                {
                    if (i.Groups[1].Value.IndexOf("mailto") == -1 && i.Groups[1].Value.IndexOf("tel:") == -1)
                    {
                        item = item.Replace("href=\"", "href=\"https://eplat.villagroup.com/Notifications/OpenLink/" + transactionID + "?url=");
                        //item = item.Replace("href=\"", "href=\"http://localhost:45000/Notifications/OpenLink/" + transactionID + "?url=");
                        bodyMail = bodyMail.Replace(itemOriginal, item);
                    }
                }
            }

            //insert tracker
            bodyMail += "<img src=\"https://eplat.villagroup.com/Notifications/GetImage/" + transactionID + "\" style=\"position:absolute;z-index:-10; opacity:0;\" />";
            //bodyMail += "<img src=\"http://localhost:45000/Notifications/GetImage/" + transactionID + "\" style=\"position:absolute;z-index:-10; opacity:0;\" />";
            return bodyMail;
        }
        public static string ReplaceTrackingID(string BodyMail, string trackingID)
        {
            return BodyMail.Replace("#trackingID", trackingID).Replace("$trackingID", trackingID);
        }
        public static void SaveEmailSendLog(int emailNotificationID, string sysItemType, Guid? sysItemID, Guid trackID, Guid sentByUserID, string subject)
        {
            ePlatEntities db = new ePlatEntities();
            var query = new tblEmailNotificationLogs();

            switch (sysItemType)
            {
                case "reservation":
                    {
                        query.reservationID = sysItemID;
                        break;
                    }
                case "purchase":
                    {
                        query.purchaseID = sysItemID;
                        break;
                    }
                case "invitation":
                    {
                        query.invitationID = sysItemID;
                        break;
                    }
            }
            query.dateSent = DateTime.Now;
            query.emailNotificationID = emailNotificationID;
            query.trackingID = trackID;
            query.subject = subject;
            query.sentByUserID = sentByUserID;
            db.tblEmailNotificationLogs.AddObject(query);
            db.SaveChanges();
        }

        public static void SaveEmailSendLog(int emailNotificationID, string sysItemType, Guid? sysItemID, Guid trackID, Guid sentByUserID, string subject, string preview, Guid leadID)
        {
            ePlatEntities db = new ePlatEntities();
            var query = new tblEmailNotificationLogs
            {
                emailPreviewJson = preview,
                subject = subject,
                sentByUserID = sentByUserID,
                dateSent = DateTime.Now,
                emailNotificationID = emailNotificationID,
                trackingID = trackID,
                leadID = leadID
            };

            switch (sysItemType)
            {
                case "reservation":
                    {
                        query.reservationID = sysItemID;
                        break;
                    }
                case "purchase":
                    {
                        query.purchaseID = sysItemID;
                        break;
                    }
                case "invitation":
                    {
                        query.invitationID = sysItemID;
                        break;
                    }
            }
            db.tblEmailNotificationLogs.AddObject(query);
            db.SaveChanges();
        }

        public static void SaveEmailSendLog(int emailNotificationID, string sysItemType, string sysItemID, Guid trackID, string preview, string subject)
        {
            ePlatEntities db = new ePlatEntities();
            var query = new tblEmailNotificationLogs();

            try
            {
                switch (sysItemType)
                {
                    case "reservation":
                        {

                            query.reservationID = Guid.Parse(sysItemID);
                            break;
                        }
                    case "purchase":
                        {
                            query.purchaseID = Guid.Parse(sysItemID);
                            break;
                        }
                    case "invitation":
                        {
                            query.invitationID = Guid.Parse(sysItemID);
                            break;
                        }
                    case "lead":
                        {
                            query.leadID = Guid.Parse(sysItemID);
                            break;
                        }
                    case "tour":
                        {
                            // query.tourID = int.Parse(sysItemID);
                            break;
                        }
                }
                query.subject = subject;
                query.emailPreviewJson = preview;
                query.sentByUserID = Guid.Parse("c53613b6-c8b8-400d-95c6-274e6e60a14a");
                query.dateSent = DateTime.Now;
                query.emailNotificationID = emailNotificationID;
                query.trackingID = trackID;
                db.tblEmailNotificationLogs.AddObject(query);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                tblNotifications notification = new tblNotifications();
                notification.sysItemTypeID = 14;
                notification.terminalID = 5;
                notification.forUserID = Guid.Parse("7310ece1-bab5-4a69-a969-af6c44157c59");
                notification.read_ = false;
                notification.eventDateTime = DateTime.Now;
                notification.eventByUserID = Guid.Parse("c53613b6-c8b8-400d-95c6-274e6e60a14a");//system
                notification.title = "Error trying to save email tracking log ";
                notification.notificationTypeID = 3; //system
                notification.description = "Reference: " + subject + " Item Type: " + sysItemType + " ItemID: " + sysItemID + " trackingID: " + trackID + ex.InnerException.ToString();
                db.tblNotifications.AddObject(notification);
                db.SaveChanges();
            }
        }

        public static List<MailingViewModel.MailMessageResponse> Send(List<MailingViewModel.MailMessageResponse> list, Boolean async = false, long? terminalID = null, Guid? userID = null, int? sysWorkGroupTeamID = null, long? mailingSettingsID = null)
        {
            ePlatEntities db = new ePlatEntities();
            var response = new List<MailingViewModel.MailMessageResponse>();

            List<tblMailingSettings> query = new List<tblMailingSettings>();

            bool isOnline = HttpContext.Current.User.Identity.IsAuthenticated;

            if (mailingSettingsID == null || !isOnline) //no replys
            {

                var emails = list.Select(x => x.MailMessage).ToList();
                var senders = emails.Select(x => x.From.Address).ToList();
                var domains = senders.Select(x => x.Split('@').Last()).ToList();

                var verifyDomainIDs = (from mad in db.tblMailingAccountDomains
                                       join ma in db.tblMailingAccount on mad.mailingAccountID equals ma.mailingAccountID
                                       where domains.Contains(mad.domain)
                                       select mad.mailingAccountID).ToList();

                query = (from ma in db.tblMailingAccount
                         join ms in db.tblMailingSettings on ma.mailingAccountID equals ms.mailingAccountID
                         where (((userID == null && ms.userID == null) || ms.userID == userID)
                         && ((terminalID == null && ms.terminalID == null) || ms.terminalID == terminalID)
                         && ((ms.sysWorkGroupTeamID == null && sysWorkGroupTeamID == null) || ms.sysWorkGroupTeamID == sysWorkGroupTeamID)
                         && ms.active != false)
                         && ms.blastSender == true
                         && verifyDomainIDs.Contains(ms.mailingAccountID.Value)
                         select ms).ToList();
            }
            else// email con informacion en mailing settings 
            {
                query = db.tblMailingSettings.Where(x => x.mailingSettingsID == mailingSettingsID && x.active != false).ToList();
            }


            if (query.Count() == 0)// DEFAULT ACCOUNT
            {
                List<int?> defaultMailingAccountIDs = new List<int?>() { 4, 5 };
                query = db.tblMailingSettings.Where(x => defaultMailingAccountIDs.Contains(x.mailingAccountID) && x.blastSender == true && x.active == true).ToList();
            }

            #region old query
            //if (mailingSettingsID == null) //no replys
            //{
            //    query = (from ms in db.tblMailingSettings
            //             where (((userID == null && ms.userID == null) || ms.userID == userID)
            //             && ((terminalID == null && ms.terminalID == null) || ms.terminalID == terminalID)
            //             && ((ms.sysWorkGroupTeamID == null && sysWorkGroupTeamID == null) || ms.sysWorkGroupTeamID == sysWorkGroupTeamID)
            //             && ms.active != false)

            //             select ms).ToList();
            //}
            //else// email con informacion en mailing settings 
            //{
            //    query = db.tblMailingSettings.Where(x => x.mailingSettingsID == mailingSettingsID && x.active != false).ToList();
            //}
            #endregion

            long id = 1;
            var highest = query.OrderByDescending(x => x.mailingSettingsID).FirstOrDefault().mailingSettingsID;
            //async = list.Count() == 1 ? true : async;//force async sendin gif list has only one email

            foreach (var item in list)
            {
                var itemResponse = new MailingViewModel.MailMessageResponse();
                System.Net.Mail.MailMessage m = item.MailMessage;
                tblMailingSettings settings = null;
                itemResponse.Recipient = string.Join(",", m.To);
                #region "replace special caracters"
                m.Body = m.Body.Replace("á", "&aacute;")
                    .Replace("é", "&eacute;")
                    .Replace("í", "&iacute;")
                    .Replace("ó", "&oacute;")
                    .Replace("ú", "&uacute;")
                    .Replace("ñ", "&ntilde;")
                    .Replace("Á", "&Aacute;")
                    .Replace("É", "&Eacute;")
                    .Replace("Í", "&Iacute;")
                    .Replace("Ó", "&Oacute;")
                    .Replace("Ú", "&Uacute;")
                    .Replace("Ñ", "&Ntilde;");
                #endregion

                if (async)
                {
                    try
                    {
                        if (isOnline)
                        {
                            id = long.Parse(System.Web.HttpContext.Current.Application["senderAccount"].ToString());
                        }

                        while (settings == null && id <= highest)
                        {
                            var _settings = query.FirstOrDefault(x => x.mailingSettingsID == id);
                            if (_settings != null && _settings.dailyCounter <= 2000)
                            {
                                settings = _settings;
                            }
                            id++;
                        }

                        var smtp = new System.Net.Mail.SmtpClient();
                        smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                        smtp.EnableSsl = settings.smtpSsl;
                        smtp.Host = settings.smtpServer;
                        smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;

                        SendEmailDelegate sd = new SendEmailDelegate(smtp.Send);
                        AsyncCallback cb = new AsyncCallback(SendEmailResponse);
                        sd.BeginInvoke(m, cb, sd);

                        itemResponse.Sent = true;
                        item.Sent = true;
                        settings.dailyCounter++;
                        db.SaveChanges();

                        if (id > query.OrderByDescending(x => x.mailingSettingsID).FirstOrDefault().mailingSettingsID)
                        {
                            id = 1;
                        }
                        if (isOnline)
                        {
                            System.Web.HttpContext.Current.Application["senderAccount"] = id;
                        }
                    }
                    catch (Exception ex)
                    {
                        var _id = id - 1;
                        query.FirstOrDefault(x => x.mailingSettingsID == _id).active = false;

                        if (settings != null)
                        {
                            settings.active = false;
                            db.SaveChanges();
                        }
                        itemResponse.Sent = false;
                        item.Sent = false;
                        itemResponse.Exception = ex.Message;
                        item.Exception = ex.Message;
                    }
                }
                else
                {
                    try
                    {
                        if (isOnline)
                        {
                            System.Web.HttpContext.Current.Application.Lock();
                            id = long.Parse(System.Web.HttpContext.Current.Application["senderAccount"].ToString());//settings id
                        }

                        while (settings == null && id <= highest)
                        {
                            var _settings = query.FirstOrDefault(x => x.mailingSettingsID == id);
                            if (_settings != null && _settings.dailyCounter <= 2000)
                            {
                                settings = _settings;
                            }
                            id++;

                        }

                        using (var email = m)
                        {
                            using (var smtp = new System.Net.Mail.SmtpClient())
                            {
                                smtp.Credentials = new NetworkCredential(settings.smtpUsername, settings.smtpPassword);
                                smtp.EnableSsl = settings.smtpSsl;
                                smtp.Host = settings.smtpServer;
                                smtp.Port = settings.smtpPort != null ? int.Parse(settings.smtpPort) : 2525;
                                smtp.Send(m);
                            }
                        }

                        itemResponse.Sent = true;
                        item.Sent = true;
                        settings.dailyCounter++;
                        db.SaveChanges();

                        if (id > highest)
                        {
                            id = 1;
                        }

                        if (isOnline)
                        {
                            System.Web.HttpContext.Current.Application["senderAccount"] = id;
                            System.Web.HttpContext.Current.Application.UnLock();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Web.HttpContext.Current.Application.UnLock();
                        var _id = id - 1;
                        query.FirstOrDefault(x => x.mailingSettingsID == _id).active = false;
                        if (settings != null)
                        {
                            settings.active = false;
                            db.SaveChanges();
                        }
                        itemResponse.Sent = false;
                        item.Sent = false;
                        itemResponse.Exception = ex.Message;
                        item.Exception = ex.Message;

                        //SAVELOG

                    }
                }
                response.Add(itemResponse);
            }
            return list;
            return response;
        }
    }

    public class Dictionaries
    {
        public static Dictionary<string, string> FrontCountries = new Dictionary<string, string> { { "US", "1" }, { "CA", "2" }, { "MX", "3" } };
        //public static Dictionary<string, string> FrontPlanTypes = new Dictionary<string, string> { { "", "1" } };
        //public static Dictionary<string, string> FrontRoomTypes = new Dictionary<string, string> { { "JR. SUITE", "1" } };
        public static Dictionary<string, string> FrontPersonalTitles = new Dictionary<string, string> { { "SR.", "6" }, { "SRA.", "7" }, { "SRITA.", "8" } };//existen nulos
        public static Dictionary<string, int> FrontReservationStatus = new Dictionary<string, int> { { "A", 1 }, { "CA", 3 }, { "IN", 5 }, { "OUT", 6 }, { "NS", 7 } };
        public static Dictionary<string, int> PlanTypes = new Dictionary<string, int> { { "all inclusive", 1 }, { "todo incluido", 1 }, { "meal plan", 1 }, { "mp", 1 }, { "ai", 1 }, { "european", 2 }, { "europeo", 2 }, { "ep", 2 } };
        public static Dictionary<string, int> FrontTourStatus = new Dictionary<string, int> { { "No Califica", 6 }, { "Full Tour", 3 }, { "Courtesy Tour", 7 }, { "No Tour", 1 } };
    }

    public class GeneralFunctions
    {
        public static string GetInitials(string text)
        {
            string initials = string.Empty;
            string[] words = text.Split(' ');
            foreach (var word in words)
            {
                if (word.Trim() != "")
                {
                    if (word.IndexOf("[") >= 0)
                    {
                        initials += word;
                    }
                    else
                    {
                        initials += word.Substring(0, 1);
                    }
                }
            }

            return initials;
        }

        public static void SaveMSISDN()
        {
            //obtención de MSISDN
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string headerVars = "MSISDN,HTTP_X_MSISDN,HTTP_X_FH_MSISDN,HTTP_MSISDN,User-Identity-Forward-msisdn,X-MSISDN,HTTP_X_UP_CALLING_LINE_ID,X_MSISDN,X-UP-CALLING-LINE-ID,X_UP_CALLING_LINE_ID,X_WAP_NETWORK_CLIENT_MSISDN";

            string headersStr = "[";
            bool available = false;
            foreach (string headerVar in headerVars.Split(','))
            {
                if (context.Request.ServerVariables.Get(headerVar) != null)
                {
                    available = true;
                    headersStr += "{ \"Var\" : \"" + headerVar + "\", \"Value\" : \"" + context.Request.ServerVariables.Get(headerVar) + "\" },";
                }
            }

            if (available)
            {
                headersStr = (headersStr.Length > 1 ? headersStr.Substring(0, headersStr.Length - 1) : headersStr);
                headersStr += "]";

                string ipStr = GetIPAddress();
                string browserStr = "";
                System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
                browserStr = "{"
                        + "\"Type\" : \"" + browser.Type + "\","
                        + "\"Name\" : \"" + browser.Browser + "\","
                        + "\"Version\" : \"" + browser.Version + "\","
                        + "\"Major Version\" : \"" + browser.MajorVersion + "\","
                        + "\"Minor Version\" : \"" + browser.MinorVersion + "\","
                        + "\"Platform\" : \"" + browser.Platform + "\""
                + "}";

                Guid transactionID = Guid.NewGuid();

                ePlatBack.Models.DataModels.ControlsDataModel.MultiFields.SaveValue(178, ipStr, transactionID);
                ePlatBack.Models.DataModels.ControlsDataModel.MultiFields.SaveValue(179, headersStr, transactionID);
                ePlatBack.Models.DataModels.ControlsDataModel.MultiFields.SaveValue(180, browserStr, transactionID);
            }
        }

        public static string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static string GetSHA1(string str)
        {
            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha1.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        public static long GetTerminalID()
        {
            ePlatEntities db = new ePlatEntities();
            string domain = HttpContext.Current.Request.Url.Host;
            var terminal = (from t in db.tblTerminalDomains
                            where t.domain == domain
                            select t.terminalID).FirstOrDefault();

            return terminal;
        }

        public static string GetCulture()
        {
            ePlatEntities db = new ePlatEntities();
            string domain = HttpContext.Current.Request.Url.Host;
            var culture = (from t in db.tblTerminalDomains
                           where t.domain == domain
                           select t.culture).FirstOrDefault();

            return culture;
        }

        public static string GetCanonicalDomain()
        {
            ePlatEntities db = new ePlatEntities();
            string canonical = string.Empty;
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            var canonicalQ = (from c in db.tblTerminalDomains
                              where c.terminalID == terminalID
                              && c.main == true
                              select c.domain).FirstOrDefault();

            if (canonicalQ != null)
            {
                canonical = canonicalQ;
            }
            return canonical;
        }

        public static string GetDomain(string purchaseID)
        {
            ePlatEntities db = new ePlatEntities();
            try
            {
                var id = Guid.Parse(purchaseID);
                var purchase = db.tblPurchases.Single(m => m.purchaseID == id);
                var domain = db.tblTerminalDomains.Where(m => m.terminalID == purchase.terminalID && m.culture == purchase.culture && m.domain.IndexOf("localhost") == -1 && m.domain.IndexOf("beta") == -1);

                if (domain.Count() > 0)
                {
                    //return "https://" + domain.FirstOrDefault().domain + "/coupons/" + purchaseID;
                    return "https://" + (domain.FirstOrDefault().domain.IndexOf("experience.com") >= 0 ? domain.FirstOrDefault().domain : (domain.FirstOrDefault().culture == "es-MX" ? "mx." : "") + "eplatfront.villagroup.com");// +"/coupons/" + purchaseID;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static bool IsUserInRole(string roleName, Guid? userID = null, bool contains = false, ePlatEntities dataContext = null)
        {
            Guid _userID = userID ?? (Guid)Membership.GetUser().ProviderUserKey;
            ePlatEntities db = dataContext ?? new ePlatEntities();

            if (contains == true)
            {
                var _isInRol = from x in db.tblUsers_SysWorkGroups
                               where x.userID == _userID && x.aspnet_Roles.RoleName.Contains(roleName)
                               select x;
                return (_isInRol.Count() > 0);
            }
            else
            {
                var _isInRol = from x in db.tblUsers_SysWorkGroups
                               where x.userID == _userID && x.aspnet_Roles.RoleName == roleName
                               select x;
                return (_isInRol.Count() > 0);
            }


        }

        public static string GetPrimaryKeyName(string tableName)
        {
            var primaryKeyName = "";
            Type edmxType = typeof(EdmScalarPropertyAttribute);
            PropertyInfo[] piArray = Type.GetType("ePlatBack.Models." + tableName).GetProperties();

            foreach (var pi in piArray)
            {
                if (Reflection.HasCustomAttribute<EdmScalarPropertyAttribute>(pi))
                {
                    EdmScalarPropertyAttribute pAtt = Reflection.GetCustomAttribute<EdmScalarPropertyAttribute>(pi);
                    if (pAtt.EntityKeyProperty)
                    {
                        primaryKeyName = pi.Name;
                    }
                }
            }
            return primaryKeyName;
        }

        public static List<LeadModel.Views.ItemLogsModel> GetItemLogs(DateTime? fromDate, string referenceID, string referenceText, string delimiter = ",")
        {
            ePlatEntities db = new ePlatEntities();
            var listLogs = new List<LeadModel.Views.ItemLogsModel>();
            var date = new DateTime();

            //in case fromDate comes null, referenceText will be used to define initialDate in range
            if (fromDate == null)
            {
                switch (referenceText)
                {
                    case "Lead":
                        {
                            var lead = Guid.Parse(referenceID);
                            date = db.tblLeads.Single(m => m.leadID == lead).inputDateTime;
                            break;
                        }
                }
            }
            else
            {
                date = DateTime.Parse(fromDate.ToString());
            }
            var logTables = GetLogTables(date, delimiter);


            foreach (var i in db.sp_getLogsByReference(logTables, "'" + referenceID + "'", referenceText, delimiter))
            {
                //var tableName = db.tblSysComponents.Single(m => m.sysComponent == i.sysComponent).tableName;
                //var primaryKey = GetPrimaryKeyName(tableName);
                ////var query = "SELECT " + primaryKey.Substring(0, primaryKey.Length - 2) + " FROM " + tableName + " WHERE " + primaryKey + " = " + i.previousValue;
                ////var value = db.ExecuteStoreQuery<string>(query);

                //listLogs.Add(new LeadModel.Views.ItemLogsModel()
                //{
                //    ItemLogs_Field = i.sysComponent,
                //    ItemLogs_PreviousValue = db.ExecuteStoreQuery<string>("SELECT " + primaryKey.Substring(0, primaryKey.Length - 2) + " FROM " + tableName + " WHERE " + primaryKey + " = " + i.previousValue).ToString(),
                //    ItemLogs_CurrentValue = db.ExecuteStoreQuery<string>("SELECT " + primaryKey.Substring(0, primaryKey.Length - 2) + " FROM " + tableName + " WHERE " + primaryKey + " = " + i.currentValue).ToString(),
                //    ItemLogs_UserName = i.userName,
                //    ItemLogs_LogDateTime = i.logDateTime.ToString("yyyy-MM-dd hh:mm:s tt", CultureInfo.InvariantCulture)
                //});

                listLogs.Add(new LeadModel.Views.ItemLogsModel()
                {
                    ItemLogs_Field = i.sysComponent,
                    ItemLogs_PreviousValue = i.previousValue,
                    ItemLogs_CurrentValue = i.currentValue,
                    ItemLogs_UserName = i.userName,
                    ItemLogs_LogDateTime = i.logDateTime.ToString("yyyy-MM-dd hh:mm:s tt", CultureInfo.InvariantCulture)
                });
            }
            return listLogs;
        }

        public static string GetLogTables(DateTime fromDate, string delimiter)
        {
            string tables = "";

            var endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
            for (var initialDate = fromDate; initialDate <= endDate; initialDate = initialDate.AddMonths(1))
            {
                tables += initialDate.Year.ToString();
                tables += initialDate.Month < 10 ? "0" + initialDate.Month.ToString() : initialDate.Month.ToString();
                tables += delimiter;
            }

            tables = tables.Substring(0, tables.Length - 1);//remove last comma
            return tables;
        }

        public static string MaskCreditCard(string CC)
        {
            int len = CC.Length;
            return (len > 4) ? CC.Substring(len - 4).PadLeft(len, '*') : CC;
        }

        public static string MaskPhone(string phone)
        {
            int len = phone.Length;
            if(len < 3)
            {
                return phone;
            }
            var init = phone.Substring(0, 3);
            
            return (len > 3) ? init + phone.Substring(len - 3).PadLeft((len - 3), '*') : phone;
        }

        public static string MaskEmail(string email)
        {
            if (IsEmailValid(email))
            {
                int len = email.Length;
                int at = email.IndexOf("@");
                if (at >= 2)
                {
                    return email.Substring(at - 2, (len - at) + 2).PadLeft(len, '*');
                }
                else
                {
                    return email.Substring(1, (len - at)).PadLeft(len, '*');
                }
                //return email.Substring(at-2, (len - at)).PadLeft(len, '*');
            }
            return email;
            //int pad = ((len - at) / 3)+1;
            //string domain = email.Substring((at - pad));

            //return pad > 1 ? domain.PadLeft(len, '*') : email;
        }

        public static SysComponentsPrivilegesModel GetPrivilegesOfComponent(List<SysComponentsPrivilegesModel> Privileges, string Component)
        {
            return Privileges.FirstOrDefault(m => m.Component == Component);
        }

        public static int GetCurrencyID(string currency)
        {
            ePlatEntities db = new ePlatEntities();
            int _currency;

            if (!int.TryParse(currency, out _currency))
            {
                _currency = db.tblCurrencies.Single(m => m.currencyCode == currency).currencyID;
            }
            return _currency;
        }

        public static bool IsEmailValid(string str)
        {
            if (str != null && Regex.IsMatch(str, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                return true;
            return false;
        }

        public class Number
        {
            public static bool IsNumeric(object Expression)
            {
                bool isNum;
                double retNum;

                isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
                return isNum;
            }

            public static string GetAmountInText(string num)
            {
                string res, dec = "";
                Int64 entero;
                int decimales;
                double nro;

                try
                {
                    nro = Convert.ToDouble(num);
                }
                catch
                {
                    return "";
                }

                entero = Convert.ToInt64(Math.Truncate(nro));
                decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));

                if (decimales > 0)
                {
                    dec = " CON " + decimales.ToString() + "/100";
                }

                res = toText(Convert.ToDouble(entero)) + dec;

                return res;
            }

            private static string toText(double value)
            {
                string Num2Text = "";
                value = Math.Truncate(value);
                if (value == 0) Num2Text = "CERO";
                else if (value == 1) Num2Text = "UNO";
                else if (value == 2) Num2Text = "DOS";
                else if (value == 3) Num2Text = "TRES";
                else if (value == 4) Num2Text = "CUATRO";
                else if (value == 5) Num2Text = "CINCO";
                else if (value == 6) Num2Text = "SEIS";
                else if (value == 7) Num2Text = "SIETE";
                else if (value == 8) Num2Text = "OCHO";
                else if (value == 9) Num2Text = "NUEVE";
                else if (value == 10) Num2Text = "DIEZ";
                else if (value == 11) Num2Text = "ONCE";
                else if (value == 12) Num2Text = "DOCE";
                else if (value == 13) Num2Text = "TRECE";
                else if (value == 14) Num2Text = "CATORCE";
                else if (value == 15) Num2Text = "QUINCE";
                else if (value < 20) Num2Text = "DIECI" + toText(value - 10);
                else if (value == 20) Num2Text = "VEINTE";
                else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);
                else if (value == 30) Num2Text = "TREINTA";
                else if (value == 40) Num2Text = "CUARENTA";
                else if (value == 50) Num2Text = "CINCUENTA";
                else if (value == 60) Num2Text = "SESENTA";
                else if (value == 70) Num2Text = "SETENTA";
                else if (value == 80) Num2Text = "OCHENTA";
                else if (value == 90) Num2Text = "NOVENTA";
                else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);
                else if (value == 100) Num2Text = "CIEN";
                else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);
                else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";
                else if (value == 500) Num2Text = "QUINIENTOS";
                else if (value == 700) Num2Text = "SETECIENTOS";
                else if (value == 900) Num2Text = "NOVECIENTOS";
                else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);
                else if (value == 1000) Num2Text = "MIL";
                else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);
                else if (value < 1000000)
                {
                    Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";
                    if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);
                }
                else if (value == 1000000) Num2Text = "UN MILLON";
                else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);
                else if (value < 1000000000000)
                {
                    Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";
                    if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);
                }
                else if (value == 1000000000000) Num2Text = "UN BILLON";
                else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
                else
                {
                    Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                    if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
                }

                return Num2Text;
            }
        }
        public class DateFormat
        {
            public static string ToMeridianHour(string hour)
            {
                hour = (hour.Length > 5 ? hour.Substring(0, 5) : hour);
                if (hour.Length > 2)
                {
                    hour = (Convert.ToInt32(hour.Substring(0, 2)) > 12 ? (Convert.ToInt32(hour.Substring(0, 2)) - 12).ToString().PadLeft(2, '0') : hour.Substring(0, 2).PadLeft(2, '0')) + hour.Substring(2, 3) + (Convert.ToInt32(hour.Substring(0, 2)) >= 12 ? " p.m." : " a.m.");
                }
                return hour;
            }

            public static string ToMilitarHour(string hour)
            {
                try
                {
                    if ((hour.IndexOf("p.m.") >= 0 || hour.IndexOf("pm") >= 0))
                    {
                        int partialHour = Convert.ToInt32(hour.Substring(0, 2));
                        if (partialHour != 12)
                        {
                            hour = (partialHour + 12).ToString() + ":" + hour.Substring(3, 2);
                        }
                        else
                        {
                            hour = partialHour.ToString() + ":" + hour.Substring(3, 2);
                        }
                    }
                    else if ((hour.IndexOf("a.m.") >= 0 || hour.IndexOf("am") >= 0))
                    {
                        int partialHour = Convert.ToInt32(hour.Substring(0, 2));
                        if (partialHour != 12)
                        {
                            hour = (Convert.ToInt32(hour.Substring(0, 2))).ToString() + ":" + hour.Substring(3, 2);
                        }
                        else
                        {
                            hour = "00:" + hour.Substring(3, 2);
                        }
                    }
                    //else
                    //{
                    //    int partialHour = Convert.ToInt32(hour.Substring(0, 2));
                    //    if (partialHour != 12)
                    //    {
                    //        hour = (Convert.ToInt32(hour.Substring(0, 2))).ToString() + ":" + hour.Substring(3, 2);
                    //    }
                    //    else
                    //    {
                    //        hour = "00:" + hour.Substring(3, 2);
                    //    }
                    //}
                    return hour;
                }
                catch
                {
                    return "";
                }
            }

            public static string _ToMilitarHour(string hour)
            {
                try
                {
                    if ((hour.IndexOf("p.m.") >= 0 || hour.IndexOf("pm") >= 0))
                    {
                        int partialHour = Convert.ToInt32(hour.Substring(0, 2));
                        if (partialHour != 12)
                        {
                            hour = (partialHour + 12).ToString() + ":" + hour.Substring(3, 2);
                        }
                        else
                        {
                            hour = partialHour.ToString() + ":" + hour.Substring(3, 2);
                        }
                    }
                    else
                    {
                        int partialHour = Convert.ToInt32(hour.Substring(0, 2));
                        if (partialHour != 12)
                        {
                            hour = (Convert.ToInt32(hour.Substring(0, 2))).ToString() + ":" + hour.Substring(3, 2);
                        }
                        else
                        {
                            hour = "00:" + hour.Substring(3, 2);
                        }
                    }
                    return hour;
                }
                catch
                {
                    return "";
                }
            }

            public static string ToText(DateTime date, string culture)
            {
                string dateText = string.Empty;
                if (culture == "es-MX")
                {
                    dateText = TranslateDay(date.DayOfWeek.ToString()) + " " + date.Day + " de " + MonthName(date.Month, culture) + " de " + date.Year;
                }
                else
                {
                    dateText = date.DayOfWeek + " " + MonthName(date.Month, culture) + " " + Ordinal(date.Day) + ", " + date.Year;
                }
                return dateText;
            }

            public static string TranslateDay(string dayOfWeek)
            {
                string strDay = "";
                switch (dayOfWeek)
                {
                    case "Monday":
                        strDay = "Lunes";
                        break;
                    case "Tuesday":
                        strDay = "Martes";
                        break;
                    case "Wednesday":
                        strDay = "Miércoles";
                        break;
                    case "Thursday":
                        strDay = "Jueves";
                        break;
                    case "Friday":
                        strDay = "Viernes";
                        break;
                    case "Saturday":
                        strDay = "Sábado";
                        break;
                    case "Sunday":
                        strDay = "Domingo";
                        break;
                }
                return strDay;
            }

            public static string MonthName(int month, string culture)
            {
                string strMonth = "";
                if (culture == "es-MX")
                {
                    switch (month)
                    {
                        case 1:
                            strMonth = "Enero";
                            break;
                        case 2:
                            strMonth = "Febrero";
                            break;
                        case 3:
                            strMonth = "Marzo";
                            break;
                        case 4:
                            strMonth = "Abril";
                            break;
                        case 5:
                            strMonth = "Mayo";
                            break;
                        case 6:
                            strMonth = "Junio";
                            break;
                        case 7:
                            strMonth = "Julio";
                            break;
                        case 8:
                            strMonth = "Agosto";
                            break;
                        case 9:
                            strMonth = "Septiembre";
                            break;
                        case 10:
                            strMonth = "Octubre";
                            break;
                        case 11:
                            strMonth = "Noviembre";
                            break;
                        case 12:
                            strMonth = "Diciembre";
                            break;
                    }
                }
                else
                {
                    switch (month)
                    {
                        case 1:
                            strMonth = "January";
                            break;
                        case 2:
                            strMonth = "February";
                            break;
                        case 3:
                            strMonth = "March";
                            break;
                        case 4:
                            strMonth = "April";
                            break;
                        case 5:
                            strMonth = "May";
                            break;
                        case 6:
                            strMonth = "June";
                            break;
                        case 7:
                            strMonth = "July";
                            break;
                        case 8:
                            strMonth = "August";
                            break;
                        case 9:
                            strMonth = "September";
                            break;
                        case 10:
                            strMonth = "October";
                            break;
                        case 11:
                            strMonth = "November";
                            break;
                        case 12:
                            strMonth = "December";
                            break;
                    }
                }
                return strMonth;
            }

            public static string Ordinal(int day)
            {
                string strOrdinal = "";
                if (day == 1)
                {
                    strOrdinal = "1<sup>st</sup>";
                }
                else if (day == 2)
                {
                    strOrdinal = "2<sup>nd</sup>";
                }
                else if (day == 3)
                {
                    strOrdinal = "3<sup>rd</sup>";
                }
                else
                {
                    strOrdinal = day + "<sup>th</sup>";
                }
                return strOrdinal;
            }

            public static string SecondsToTime(int seconds)
            {
                string time = string.Empty;
                TimeSpan t = TimeSpan.FromSeconds(seconds);
                if (t.Hours > 0)
                {
                    time = t.Hours + " hours";
                }
                if (t.Minutes > 0)
                {
                    if (time != "")
                    {
                        time += ", ";
                    }
                    time += t.Minutes + " minutes";
                }
                if (t.Seconds > 0)
                {
                    if (time != "")
                    {
                        time += ", ";
                    }
                    time += t.Seconds + " seconds";
                }

                return time;
            }

            public static int GetIso8601WeekOfYear(DateTime time)
            {
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
                if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                {
                    time = time.AddDays(3);
                }

                // Return the week of our adjusted day
                return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }

            public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
            {
                DateTime jan1 = new DateTime(year, 1, 1);
                int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

                DateTime firstThursday = jan1.AddDays(daysOffset);
                var cal = CultureInfo.CurrentCulture.Calendar;
                int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                var weekNum = weekOfYear;
                if (firstWeek <= 1)
                {
                    weekNum -= 1;
                }
                var result = firstThursday.AddDays(weekNum * 7);
                return result.AddDays(-4);
            }
        }

        public static Dictionary<string, string> WeekDays = new Dictionary<string, string> { { "sunday", "Sunday" }, { "monday", "Monday" }, { "tuesday", "Tuesday" }, { "wednesday", "Wednesday" }, { "thursday", "Thursday" }, { "friday", "Friday" }, { "saturday", "Saturday" } };

        public static Dictionary<string, string> PaymentTypes = new Dictionary<string, string> { { "1", "Cash" }, { "2", "Credit Card" }, { "3", "Charge Back" }, { "4", "Resort Credit" }, { "5", "Wire Transfer" }, { "6", "Certificate" }, { "7", "Points Redemption" },{ "8", "Referrals Credits"} };

        public static List<MasterChartViewModel.EgressTypeModel> EgressTypes = new List<MasterChartViewModel.EgressTypeModel> { new MasterChartViewModel.EgressTypeModel() { Key = "1", Value = "Customer's Deposit", Terminals = new long[] { 4, 5, 6, 7, 8, 25, 26, 32, 43, 61 } }, new MasterChartViewModel.EgressTypeModel() { Key = "2", Value = "Charge Back", Terminals = new long[] { 4, 5, 6, 7, 8, 25, 26, 32, 43, 61 } }, new MasterChartViewModel.EgressTypeModel() { Key = "3", Value = "Coupons CxC", Terminals = new long[] { } }, new MasterChartViewModel.EgressTypeModel() { Key = "4", Value = "Loan", Terminals = new long[] { 25, 32, 5, 43, 61 } } };

        public static Dictionary<string, string> IncomeTypes = new Dictionary<string, string> { { "1", "Increase" }, { "2", "Reimbursement" }, { "3", "Loan" }, { "4", "Loan Payment" }, { "5", "Vale Marketing" } };

        public static Dictionary<string, string> RequiredFields = new Dictionary<string, string> {
            {"1", "First Name"},
            {"2", "Last Name"},
            {"3", "Email"},
            {"4", "Hotel Confirmation Number"},
            {"5", "Arrival Date"},
            {"6", "Destination"},
            {"7", "Resort"},
            {"8", "Presentation Date"},
            {"9", "Presentation Time"},
            {"10", "Optionals"}
        };

        public static Dictionary<int, string> ApplyPayment_ErrorCodes = new Dictionary<int, string>() {
            {0, "Approved"},
            {-1, "Declined"},
            {-2, "Data Incomplete"},
            {-3, "Date not valid"},
            {-4, "Access Denied"},
            {-5, "Loan not found"},
            {-6, "funding not found"},
            {-7, "legal merchant not found"},
            {-8, "merchant not found"},
            {-9, "error with data access class"},
            {-10, "error with cc class"},
            {-11, "HTTPS required on client"},
        };

        public static int? ParseStringToNullableInt(string text)
        {
            int value;
            if (int.TryParse(text, out value))
            {
                return value;
            }
            else
            {
                return (int?)null;
            }
        }

        public static long? ParseToNullableLong(string text)
        {
            long value;
            string _text = text.ToString();
            if (long.TryParse(_text, out value))
            {
                return value;
            }
            else
            {
                return (long?)null;
            }
        }

        public static long? ParseToNullableLong(long text)
        {
            long value;
            string _text = text.ToString();
            if (long.TryParse(_text, out value))
            {
                return value;
            }
            else
            {
                return (long?)null;
            }
        }

        public static int TriggerServicesLog(dynamic _item, string type, dynamic _attempt, string urlReferrer = "", HttpRequestBase request = null, string terminalID = null, string logReference = null)
        {
            // 
            //if (bool.Parse(HttpContext.Current.Application["logs"].ToString()) == false)
            //{

            //}
            //if (HttpContext.Current != null && HttpContext.Current.Application != null)
            //{
            //   // if (logs.Value)
            //        return 1;
            //}

            //
            ePlatEntities db = new ePlatEntities();
            DateTime Date = DateTime.Now;
            JavaScriptSerializer Json = new JavaScriptSerializer();
            Json.MaxJsonLength = Int32.MaxValue;
            var terminal = terminalID == null ? (int?)null : int.Parse(terminalID);
            var query = "";
            //exclusion of log for localhost
            //if (request.Url.ToString().IndexOf("localhost") != -1)
            //{
            //    return 1;
            //}

            //if (Debugger.IsAttached)
            //{
            //    return 1;
            //}
            try
            {
                var url = urlReferrer;
                var urlMethod = request == null ? "" : request.Url.ToString();
                var method = request == null ? "" : request.HttpMethod.ToString();
                var UserID = type == "TaskScheduler" ? Guid.Parse("c53613b6-c8b8-400d-95c6-274e6e60a14a") : (Guid)Membership.GetUser().ProviderUserKey;
                var User = db.tblUserProfiles.Single(x => x.userID == UserID);
                var contactInfo = "'False'";

                var item = Json.Serialize(_item);
                var JsonItem = Json.Deserialize<dynamic>(item);
                var attempt = _attempt != null ? Json.Serialize(_attempt) : null;
                var JsonAttempt = attempt != null ? Json.Deserialize<dynamic>(attempt) : "";
                string TableName = "";
                TableName = "tblUserLogActivity" + Date.ToString("yyyyMM");
                AdminDataModel.CreateTableUserLogActivity();
                //var data = JsonItem.GetType();
                var dataType = JsonItem.GetType();
                if (type != "Delete" && dataType.IsGenericType)
                {
                    foreach (var x in _item.GetType().GetProperties())
                    {
                        if ((x.Name.ToLower().Contains("email") && x.GetValue(_item, null) != null) || (x.Name.ToLower().Contains("phone") && x.GetValue(_item, null) != null))
                        {
                            contactInfo = "'True'";
                            break;
                        }
                    }
                }

                /*string Insert = "INSERT INTO "
                + TableName
                + " (UserID, DateSaved, Controller, Method, JsonModel, Description, UrlMethod, Url, ContactInfo, terminalID) VALUES";
                string InsertValuesRow = " ('" + UserID + "','" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "','" + controller["controller"] + "Controller'" + ",'" + method + "','" + item + " " + "','" + controller["action"] + " > " + User.firstName + " ";
                */

                string InsertValuesRow;
                string Insert = "INSERT INTO "
                       //+ TableName + " (UserID, DateSaved, Controller, Method, JsonModel, Description, UrlMethod, Url, ContactInfo, terminalID) VALUES";
                       + TableName + " (UserID, DateSaved, Controller, Method, JsonModel, Description, UrlMethod, Url, ContactInfo, terminalID, referenceID) VALUES";
                if (type == "TaskScheduler")
                {
                    InsertValuesRow = " ('" + UserID + "','" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "','" + "TaskScheduler" + "Controller'" + ",'" + method + "','" + item + " " + "','" + "Task" + " > " + User.firstName + " ";
                }
                else
                {
                    var controller = request.RequestContext.RouteData.Values;
                    InsertValuesRow = " ('" + UserID + "','" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "','" + controller["controller"] + "Controller'" + ",'" + method + "','" + item + " " + "','" + controller["action"] + " > " + User.firstName + " ";
                }

                //CASOS
                #region "casos"
                switch (type)
                {
                    case "Delete":
                        #region
                        {
                            if (JsonAttempt["Type"] == 1)
                            {
                                InsertValuesRow += "borró el objeto con el ID: " + item + ", Mensaje: " + JsonAttempt["Message"];
                            }
                            else
                            {
                                InsertValuesRow += "intentó borrar esto: " + item + ", Mensaje: " + JsonAttempt["Message"] + ", Exeption: " + JsonAttempt["Exception"];
                            }
                            break;
                        }
                    #endregion
                    case "Search":
                        #region
                        {
                            InsertValuesRow += "buscó esto: \n"; /*item*/
                            UserActivityLogs modelInfo = AdminDataModel.DefaultOption(item);
                            InsertValuesRow += AdminDataModel.structureValuesRow(modelInfo);

                            break;
                        }
                    #endregion
                    case "Save":
                        #region
                        {
                            if (attempt.Contains("Updated")) //update values
                            {
                                if (JsonAttempt["Type"] == 0)
                                {
                                    InsertValuesRow += "intentó actualizar estos datos: ";//
                                    InsertValuesRow += "Mensaje: " + JsonAttempt["Message"] + ", Exeption: " + JsonAttempt["Exception"] + "\n";
                                }
                                else
                                {
                                    InsertValuesRow += "estos datos fueron actualizados: ";
                                }
                            }
                            else
                            {
                                if (JsonAttempt["Type"] == 0)//error!
                                {
                                    InsertValuesRow += "intentó guardar estos datos: " + "Mensaje:" + JsonAttempt["Message"] + ", Exeption:" + JsonAttempt["Exception"] + "\n";
                                }
                                else
                                {
                                    InsertValuesRow += "guardó estos datos: " + " mensaje: " + JsonAttempt["Message"] + ",";
                                }
                            }
                            UserActivityLogs modelInfo = AdminDataModel.DefaultOption(item);
                            InsertValuesRow += AdminDataModel.structureValuesRow(modelInfo);
                            //InsertValuesRow.Count();
                            break;
                        }
                    #endregion
                    case "Get":
                        #region
                        {
                            InsertValuesRow += "obtuvó estos datos: ";
                            if (dataType.IsGenericType && JsonItem != null)
                            {
                                foreach (var x in JsonItem)
                                {
                                    if (x.GetType() != typeof(Dictionary<string, object>))
                                    {
                                        //if ((x.Key.Contains("CloseOutID") || x.Key.Contains("Company") || x.Key.Contains("PointOfSale") || x.Key.Contains("Promotion") || x.Key.Contains("Destination") || x.Key.Contains("Exchange") ||
                                        //     x.Key.Contains("Agent") || x.Key.Contains("Date") || x.Key.Contains("Invoice") || x.Key.Contains("ID") || x.Key.Contains("Terminal") || x.Key.Contains("Description") || x.Key.Contains("Budget") ||
                                        //     x.Key.Contains("email") || x.Key.Contains("Email") || x.Key.Contains("phone") || x.Key.Contains("Phone") || x.Key.Contains("Folio") || x.Key.Contains("folio") || x.Key.Contains("User") || x.Key.Contains("user")
                                        //     || x.Key.Contains("Status"))
                                        //     && x.Value is string)
                                        //{
                                        if (x.Key != null && x.Value != null)
                                        {
                                            InsertValuesRow += " \n" + x.Key.ToString() + ":" + x.Value + ", ";
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        foreach (var a in x)
                                        {
                                            //if ((a.Key.Contains("CloseOutID") || a.Key.Contains("Company") || a.Key.Contains("PointOfSale") || a.Key.Contains("Promotion") || a.Key.Contains("Destination") || a.Key.Contains("Exchange") ||
                                            // a.Key.Contains("Agent") || a.Key.Contains("Date") || a.Key.Contains("Invoice") || a.Key.Contains("ID") || a.Key.Contains("Terminal") || a.Key.Contains("Description") || a.Key.Contains("Budget") ||
                                            // a.Key.Contains("email") || a.Key.Contains("Email") || a.Key.Contains("phone") || a.Key.Contains("Phone") || a.Key.Contains("Folio") || a.Key.Contains("folio") || a.Key.Contains("User") || a.Key.Contains("user")
                                            // || a.Key.Contains("Status"))
                                            // && a.Value is string)
                                            //{
                                            if (a.Key != null && a.Value != null)
                                            {
                                                InsertValuesRow += " \n" + a.Key.ToString() + ":" + a.Value + ", ";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                InsertValuesRow += item;
                            }
                            break;
                        }
                    #endregion
                    case "SaveCloseOut":
                        #region
                        {
                            if (attempt.Contains("Updated")) //update values
                            {
                                if (JsonAttempt["Type"] == 0)//
                                {
                                    InsertValuesRow += "intento guardar estos datos: ";//
                                    InsertValuesRow += "Mensaje: " + JsonAttempt["Message"] + ", Exeption: " + JsonAttempt["Exception"] + "\n";
                                }
                                else
                                {
                                    InsertValuesRow += "estós datos fueron actualizados: ";
                                }
                                #region Old
                                foreach (var x in _item)
                                {
                                    if (x.Key.Contains("Company") || x.Key.Contains("PointOfSale") || x.Key.Contains("Agent") || x.Key.Contains("FromDate") || x.Key.Contains("ToDate") || x.Key.Contains("CloseOutDate"))
                                    {
                                        InsertValuesRow += " " + x.Key.ToString() + " " + x.Value.ToString() + ",";
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                if (JsonAttempt["Type"] == 0)//error!
                                {
                                    InsertValuesRow += "intentó guardar estos datos: " + "Mensaje:" + JsonAttempt["Message"] + ", Exeption:" + JsonAttempt["Exception"] + "\n";
                                }
                                else
                                {
                                    InsertValuesRow += "guardó estos datos: " + " mensaje: " + JsonAttempt["Message"] + ",";
                                }
                                #region Old
                                var JsonCloseOutItem = Json.Deserialize<object>(item);
                                foreach (var CloseOutItem in JsonCloseOutItem)
                                {
                                    if (CloseOutItem.Key.Contains("JsonModel"))
                                    {
                                        var CloseOutDetails = Json.Deserialize<object>(CloseOutItem.Value);
                                        foreach (var z in CloseOutDetails)
                                        {
                                            if (z.Key.Contains("Company") || z.Key.Contains("PointOfSale") || z.Key.Contains("Agent") || z.Key.Contains("FromDate") || z.Key.Contains("ToDate") || z.Key.Contains("CloseOutDate"))
                                            {
                                                InsertValuesRow += " " + z.Key.ToString() + ":" + z.Value.ToString() + ", \n";
                                            }
                                        }
                                    }
                                    /*   else if (CloseOutItem.Contains("Company") || CloseOutItem.Contains("PointOfSale") || CloseOutItem.Contains("Agent") || CloseOutItem.Contains("FromDate") || CloseOutItem.Contains("ToDate") || CloseOutItem.Contains("CloseOutDate"))
                                       {
                                           InsertValuesRow += " " + CloseOutItem.Key.ToString() + ":" + CloseOutItem.Value.ToString() + ", \n";
                                       }*/
                                }
                                #endregion
                            }
                            break;
                        }
                    #endregion
                    case "RestoreActivity"://Activities-Controller
                        #region
                        {
                            if (JsonAttempt["Type"] == 1)
                            {
                                InsertValuesRow += "Restauró la Actividad con el ID: " + item + ", Mensaje: " + JsonAttempt["Message"];
                            }
                            else
                            {
                                InsertValuesRow += "Intentó Restaurar la actividad: " + item + ", Mensaje: " + JsonAttempt["Message"] + ", Exeption: " + JsonAttempt["Exception"];
                            }
                            break;
                        }
                    #endregion
                    case "UpdateStock":
                        #region "Updatestock"
                        {
                            if (JsonAttempt["Type"] == 1)
                            {
                                InsertValuesRow += "Actualizó el Stock " + item + ", Mensaje: " + JsonAttempt["Message"];
                            }
                            else
                            {
                                InsertValuesRow += "Intentó Actualizar el Stock " + item + ", Mensaje: " + JsonAttempt["Message"] + ", Exeption: " + JsonAttempt["Exception"];
                            }
                            break;
                        }
                    #endregion
                    case "TaskScheduler":
                        #region
                        {
                            if (JsonAttempt["Type"] == 1)
                            {
                                InsertValuesRow += "Tarea realizada, Mensaje: " + JsonAttempt["Message"];
                            }
                            else
                            {
                                InsertValuesRow += "Tarea no ejecutada, Mensaje: " + JsonAttempt["Message"] + "," + " Exeption: " + JsonAttempt["Exception"];
                            }
                            break;
                        }
                    #endregion
                    case "GetPurchaseServices":
                        #region
                        {
                            InsertValuesRow += "obtuvó estos datos: ";
                            var list = new List<string>();
                            foreach (KeyValuePair<string, object> x in JsonItem[0])
                            {
                                var existItem = list.Count(z => z == x.Key);
                                if (x.Value != null && existItem == 0)
                                {
                                    InsertValuesRow += x.Key + ":" + x.Value + "\n";
                                }
                                else
                                    list.Add(x.Key);
                            }
                            break;
                        }
                    #endregion
                    case "Prearrival":
                        {
                            if (attempt.Contains("Updated")) //update values
                            {
                                InsertValuesRow += "Actualización de datos.\n";//
                                InsertValuesRow += "Mensaje: " + JsonAttempt["Message"];
                                if (JsonAttempt["Type"] == 0)
                                {

                                    InsertValuesRow += ", Exeption: " + JsonAttempt["Exception"] + "\n";
                                }
                                //else
                                //{
                                //    InsertValuesRow += "estos datos fueron actualizados: ";
                                //}
                            }
                            else
                            {
                                InsertValuesRow += "Guardado de datos.\n";//
                                InsertValuesRow += "Mensaje: " + JsonAttempt["Message"];
                                if (JsonAttempt["Type"] == 0)//error!
                                {
                                    InsertValuesRow += ", Exeption:" + JsonAttempt["Exception"] + "\n";
                                }
                                //else
                                //{
                                //    InsertValuesRow += "guardó estos datos: " + " mensaje: " + JsonAttempt["Message"] + ",";
                                //}
                            }
                            InsertValuesRow = InsertValuesRow.Replace("\\", "");
                            break;
                        }
                    default:
                        {
                            InsertValuesRow += "Info: \n";
                            UserActivityLogs modelInfo = AdminDataModel.DefaultOption(attempt);
                            InsertValuesRow += AdminDataModel.structureValuesRow(modelInfo);
                            break;
                        }
                }
                #endregion
                //InsertValuesRow += "','" + urlMethod + "','" + url + "'," + contactInfo + ",'" + terminal + "')";
                InsertValuesRow += "','" + urlMethod + "','" + url + "'," + contactInfo + "," + terminal + "," + (logReference != null ? "'" + logReference + "'" : "null") + ")";
                query = Insert + InsertValuesRow;
                db.ExecuteStoreCommand(query);
                return 1;
            }
            catch (Exception ex)
            {
                // AdminDataModel.UserLogsActivitySendEx(ex, terminalID, urlReferrer, query, request);
                return 1;
            }
        }
    }

    static class Rot13
    {
        /// <summary>
        /// Performs the ROT13 character rotation.
        /// </summary>
        public static string Transform(string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }
    }

    public class ListItems
    {
        public static SelectListItem Default(string Text = "--Select One--", string Value = "0", bool Selected = false)
        {
            return new SelectListItem() { Text = Text, Value = Value, Selected = Selected };
        }

        public static SelectListItem NotSet(string Text = "[Not Set]", string Value = "null", bool Selected = false)
        {
            return new SelectListItem() { Text = Text, Value = Value, Selected = Selected };
        }

        public static List<SelectListItem> Booleans()
        {
            List<SelectListItem> BooleanItems = new List<SelectListItem>();
            BooleanItems.Add(new SelectListItem() { Text = "True", Selected = false, Value = "true" });
            BooleanItems.Add(new SelectListItem() { Text = "False", Selected = false, Value = "false" });

            return BooleanItems;
        }
    }

    public static class Dynamic
    {
        public static IQueryable Join(this IQueryable source1, string alias1, IQueryable source2, string alias2, string key1, string key2, string selector, params object[] args)
        {
            if (source1 == null) throw new ArgumentNullException("source1");
            if (alias1 == null) throw new ArgumentNullException("alias1");
            if (source2 == null) throw new ArgumentNullException("source1");
            if (alias2 == null) throw new ArgumentNullException("alias2");
            if (key1 == null) throw new ArgumentNullException("key1");
            if (key2 == null) throw new ArgumentNullException("key2");
            if (selector == null) throw new ArgumentNullException("selector");
            ParameterExpression p1 = Expression.Parameter(source1.ElementType, alias1);
            ParameterExpression p2 = Expression.Parameter(source2.ElementType, alias2);
            LambdaExpression keyLambda1 = System.Linq.Dynamic.DynamicExpression.ParseLambda(new ParameterExpression[] { p1 }, null, key1, null);
            LambdaExpression keyLambda2 = System.Linq.Dynamic.DynamicExpression.ParseLambda(new ParameterExpression[] { p2 }, null, key2, null);
            FixLambdaReturnTypes(ref keyLambda1, ref keyLambda2);
            LambdaExpression lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(new ParameterExpression[] { p1, p2 }, null, selector, args);
            return source1.Provider.CreateQuery(
              Expression.Call(
                typeof(Queryable), "Join",
                new Type[] { source1.ElementType, source2.ElementType, keyLambda1.Body.Type, lambda.Body.Type },
               source1.Expression, source2.Expression, Expression.Quote(keyLambda1), Expression.Quote(keyLambda2), Expression.Quote(lambda)
               ));
        }

        private static void FixLambdaReturnTypes(ref LambdaExpression lambda1, ref LambdaExpression lambda2)
        {
            Type type1 = lambda1.Body.Type;
            Type type2 = lambda2.Body.Type;
            if (type1 != type2)
            {
                if (type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(Nullable<>) && type1 == type2.GetGenericArguments()[0])
                {
                    lambda1 = Expression.Lambda(Expression.Convert(lambda1.Body, type2), lambda1.Parameters.ToArray());
                }
                else
                {
                    // this may fail because types are incompatible
                    lambda2 = Expression.Lambda(Expression.Convert(lambda2.Body, type1), lambda2.Parameters.ToArray());
                }
            }
        }
    }

    public class UtilsImport
    {
        public static int CompareInstancesContent(object instance, object mirrorInstance)
        {
            var counter = 0;
            foreach (var i in instance.GetType().GetProperties())
            {
                var firstValue = i.GetValue(instance, null) != null ? i.GetValue(instance, null).ToString() : null;
                var mirrorValue = mirrorInstance.GetType().GetProperty(i.Name).GetValue(mirrorInstance, null) != null ? mirrorInstance.GetType().GetProperty(i.Name).GetValue(mirrorInstance, null).ToString() : null;
                if (firstValue != mirrorValue)
                {
                    counter++;
                    break;
                }
            }
            return counter;
        }
    }

    public class Token
    {
        public static TokenResult GetToken()
        {
            try
            {
                GlobalVars.Token = new TokenResult();
                if (DateTime.UtcNow > GlobalVars.Token.Expires)
                {
                    RestClient Client = new RestClient($"{GlobalVars.Token.ServerURL}/token")
                    {
                        Timeout = -1
                    };
                    
                    RestRequest RequestToken = new RestRequest(Method.POST);
                    RequestToken.AddHeader("Content-Type", "application/x-www-form-rlencoded")
                        .AddParameter("grant_type", "password")
                        .AddParameter("username", "eplat@eplat.com")
                        .AddParameter("password", "eP1at2020#");
                    IRestResponse ResponseToken = Client.Execute(RequestToken);
                    GlobalVars.Token = JsonConvert.DeserializeObject<TokenResult>(ResponseToken.Content.ToString());

                    if (GlobalVars.Token == null)
                    {
                        throw new Exception("Can't get access token");
                    }
                }
            }
            catch(Exception ex)
            {
                if (GlobalVars.GetTokenfailureCount <= 5)
                {
                    GlobalVars.Token = new TokenResult();
                    GlobalVars.GetTokenfailureCount++;
                    GlobalVars.Token = Token.GetToken();
                }
                else
                {
                    GlobalVars.GetTokenfailureCount = 0;
                    GlobalVars.Token = new TokenResult
                    {
                        Error = ex.Message + Environment.NewLine + $"Reached Max retry Count { GlobalVars.GetTokenfailureCount }",
                    };
                }
            }
            return GlobalVars.Token;
        }
    }

    //public class TextFinder
    //{
    //    public struct LinkItem
    //    {
    //        public string href;
    //        public string text;

    //        public override string ToString()
    //        {
    //            return href + "\n\t" + text;
    //        }
    //    }

    //    static class LinkFinder
    //    {
    //        public static List<LinkItem> Find(string file)
    //        {
    //            List<LinkItem> list = new List<LinkItem>();

    //            //find matches in file
    //            MatchCollection mc = Regex.Matches(file, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

    //            //loop over matches

    //        }
    //    }
    //}
}
