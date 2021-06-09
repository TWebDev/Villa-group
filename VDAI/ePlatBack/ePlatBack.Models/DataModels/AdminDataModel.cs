
using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Routing;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Web.Security;
using System.Threading;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace ePlatBack.Models.DataModels
{
    public class AdminDataModel
    {
        public static UserSession session = new UserSession();

        public class AdminCatalogs
        {
            public static List<SelectListItem> GetAllWorkGroups()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                foreach (var i in db.tblSysWorkGroups.OrderBy(m => m.sysWorkGroup))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.sysWorkGroupID.ToString(),
                        Text = i.sysWorkGroup
                    });
                }
                return list;
            }

            public static List<SelectListItem> GetAllRoles()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                foreach (var i in db.aspnet_Roles.OrderBy(m => m.RoleName))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.RoleId.ToString(),
                        Text = i.RoleName
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpWorkGroups()
            {
                List<SelectListItem> list = new List<SelectListItem>();
                ePlatEntities db = new ePlatEntities();
                //list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblSysWorkGroups.OrderBy(m => m.sysWorkGroup))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.sysWorkGroupID.ToString(),
                        Text = i.sysWorkGroup
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpActiveRoles(long terminalID)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                ePlatEntities db = new ePlatEntities();
                var usersInTerminal = from u in db.tblUsers_Terminals
                                      where u.terminalID == terminalID
                                      && u.aspnet_Users.aspnet_Membership.IsApproved
                                      select u.userID;

                var ActiveRoles = (from a in db.tblUsers_SysWorkGroups
                                   where usersInTerminal.Contains(a.userID)
                                   && a.manageServices == true
                                   orderby a.aspnet_Roles.RoleName
                                   select new
                                   {
                                       a.roleID,
                                       a.aspnet_Roles.RoleName
                                   }).Distinct();

                foreach (var i in ActiveRoles)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.roleID.ToString(),
                        Text = i.RoleName
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpRoles()
            {
                List<SelectListItem> list = new List<SelectListItem>();
                ePlatEntities db = new ePlatEntities();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.aspnet_Roles.OrderBy(m => m.RoleName))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.RoleId.ToString(),
                        Text = i.RoleName
                    });
                }
                return list;
            }

            public static string[] FillDrpTables()
            {
                string[] list = new string[] { };
                var tableList = new List<string>();
                var tables = from a in typeof(ePlatEntities).GetProperties() select a;
                foreach (var i in tables)
                {
                    if (i.Name.IndexOf("tbl") == 0)
                    {
                        tableList.Add(i.Name.ToString());
                    }
                }
                list = tableList.ToArray();
                return list;
            }

            //public static List<SelectListItem> FillDrpTables()
            //{
            //    List<SelectListItem> list = new List<SelectListItem>();
            //    list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
            //    var tables = from a in typeof(ePlatEntities).GetProperties() select a;
            //    foreach (var i in tables)
            //    {
            //        if (i.Name.IndexOf("tbl") == 0)
            //        {
            //            list.Add(new SelectListItem()
            //            {
            //                Value = i.Name,
            //                Text = i.Name
            //            });
            //        }
            //    }
            //    return list;
            //}

            public static string[] FillDrpFields(string tableName)
            {
                var fieldList = new List<string>();
                if (tableName != "0")
                {
                    Type type = Type.GetType("ePlatBack.Models." + tableName);
                    var tableFields = from p in type.GetProperties()
                                      where (from a in p.GetCustomAttributes(false)
                                             where a is System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
                                             select true).FirstOrDefault()
                                      select p;
                    foreach (var i in tableFields)
                        fieldList.Add(i.Name);
                }
                return fieldList.ToArray();
            }

            //public static List<SelectListItem> FillDrpFields(string tableName)
            //{
            //    List<SelectListItem> list = new List<SelectListItem>();
            //    if (tableName != "0")
            //    {
            //        list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
            //        Type type = Type.GetType("ePlatBack.Models." + tableName);
            //        var tableFields = from p in type.GetProperties()
            //                          where (from a in p.GetCustomAttributes(false)
            //                                 where a is System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            //                                 select true).FirstOrDefault()
            //                          select p;
            //        foreach (var i in tableFields)
            //        {
            //            list.Add(new SelectListItem()
            //            {
            //                Value = i.Name,
            //                Text = i.Name
            //            });
            //        }
            //    }
            //    else
            //        list.Add(new SelectListItem() { Value = "0", Text = "--Select Table--", Selected = false });
            //    return list;
            //}

            public static List<SelectListItem> FillDrpDestinationsPerCurrentTerminals()
            {
                return PlaceDataModel.GetDestinationsByCurrentTerminals();
            }
        }

        //profiles

        public List<SysComponentModel> GetComponentsTree()
        {
            ePlatEntities db = new ePlatEntities();
            List<SysComponentModel> list = new List<SysComponentModel>();
            var query = from component in db.tblSysComponents
                        orderby component.sysParentComponentID
                        select component;

            foreach (var i in query)
            {
                var parentComponent = 0;
                if (i.sysParentComponentID != null)
                    parentComponent = int.Parse(i.sysParentComponentID.ToString());
                list.Add(new SysComponentModel()
                {
                    ComponentID = int.Parse(i.sysComponentID.ToString()),
                    ComponentName = i.sysComponent,
                    ParentComponentID = parentComponent,
                    ComponentTypeID = i.sysComponentTypeID,
                    ComponentTypeName = i.tblSysComponentTypes.sysComponentType,
                    Url = i.url,
                    TableName = i.tableName ?? "",
                    TableField = i.fieldName ?? ""
                    //Alias = i.tblSysComponentAliases.FirstOrDefault().alias
                });
            }
            return list;
        }

        public List<SelectListItem> GetComponentsTypes()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var i in db.tblSysComponentTypes.OrderBy(m => m.sysComponentType))
                list.Add(new SelectListItem()
                {
                    Value = i.sysComponentTypeID.ToString(),
                    Text = i.sysComponentType
                });
            return list;
        }

        public static List<long> GetChildrenOfComponentID(List<long> sysComponentIDs)
        {
            ePlatEntities db = new ePlatEntities();
            List<long> children = new List<long>();
            children = (from c in db.tblSysComponents
                        where sysComponentIDs.Contains((long)c.sysParentComponentID)
                        select c.sysComponentID).ToList();

            if (children.Count() > 0)
            {
                children = children.Concat(GetChildrenOfComponentID(children)).ToList();
            }
            return children;
        }

        public static long? GetSysComponentIDByURL(string url)
        {
            ePlatEntities db = new ePlatEntities();
            var sysComponentQ = (from s in db.tblSysComponents
                                 where s.url == url
                                 select s.sysComponentID).FirstOrDefault();

            return sysComponentQ;
        }

        public static List<SysComponentsPrivilegesModel> GetViewPrivileges(string viewComponent)
        {
            ePlatEntities db = new ePlatEntities();
            Guid roleId = (Guid)session.RoleID;
            int workGroupId = (int)session.WorkGroupID;
            List<SysComponentsPrivilegesModel> listPrivileges = new List<SysComponentsPrivilegesModel>();

            long viewComponentID;
            if (!long.TryParse(viewComponent, out viewComponentID))
            {
                viewComponentID = db.tblSysComponents.SingleOrDefault(m => m.sysComponent == viewComponent).sysComponentID;
            }

            long[] childrenComponents = GetChildrenOfComponentID(new List<long>() { viewComponentID }).ToArray();

            var queryPrivileges = from component in db.tblSysProfiles
                                  where component.roleID == roleId && component.sysWorkGroupID == workGroupId
                                  && (childrenComponents.Contains(component.sysComponentID) || component.sysComponentID == viewComponentID)
                                  orderby component.sysComponentOrder
                                  select new
                                  {
                                      componentID = component.sysComponentID,
                                      componentName = component.tblSysComponents.sysComponent,
                                      create = component.create_,
                                      edit = component.edit_,
                                      view = component.view_,
                                      alias = component.tblSysComponents.tblSysComponentAliases.FirstOrDefault(m => m.sysWorkGroupID == workGroupId).alias,
                                      compOrder = component.sysComponentOrder,
                                      component.tblSysComponents.sysComponentTypeID
                                  };

            foreach (var i in queryPrivileges)
            {
                var view = i.view;
                if (i.sysComponentTypeID == 20)
                {
                    if (i.view == true)
                    {
                        string ip = GeneralFunctions.GetIPAddress();
                        if (db.tblIPsIndex.Count(m => m.ip == ip) > 0)
                            view = true;
                        else
                            view = false;
                    }
                    else
                        view = true;
                }
                listPrivileges.Add(new SysComponentsPrivilegesModel()
                {
                    ComponentID = int.Parse(i.componentID.ToString()),
                    Component = i.componentName,
                    Create = i.create,
                    Edit = i.edit,
                    //View = i.view,
                    //View = i.sysComponentTypeID == 20 ? i.view ? db.tblIPsIndex.Count(m => m.ip == GeneralFunctions.GetIPAddress()) > 0 ? true : false : true : i.view,
                    View = view,
                    ComponentOrder = int.Parse(i.compOrder.ToString()),
                    Alias = i.alias
                });
            }

            return listPrivileges;
        }

        public static List<SysComponentsPrivilegesModel> GetViewPrivileges(long viewComponentID)
        {
            ePlatEntities db = new ePlatEntities();
            Guid roleId = (Guid)session.RoleID;
            int workGroupId = (int)session.WorkGroupID;
            List<SysComponentsPrivilegesModel> listPrivileges = new List<SysComponentsPrivilegesModel>();

            long[] childrenComponents = GetChildrenOfComponentID(new List<long>() { viewComponentID }).ToArray();

            var queryPrivileges = from component in db.tblSysProfiles
                                   join alias in db.tblSysComponentAliases on component.sysComponentID equals alias.sysComponentID
                                   into component_alias
                                   from alias in component_alias.DefaultIfEmpty()
                                  where component.roleID == roleId && component.sysWorkGroupID == workGroupId
                                  && (childrenComponents.Contains(component.sysComponentID) || component.sysComponentID == viewComponentID)
                                   && (alias.sysWorkGroupID == workGroupId || alias.sysWorkGroupID == null)
                                  orderby component.sysComponentOrder
                                  select new
                                  {
                                      componentID = component.sysComponentID,
                                      componentName = component.tblSysComponents.sysComponent,
                                      create = component.create_,
                                      edit = component.edit_,
                                      view = component.view_,
                                      required = component.required,
                                       alias = alias.alias ?? "",
                                      compOrder = component.sysComponentOrder,
                                      component.tblSysComponents.sysComponentTypeID
                                  };

            foreach (var i in queryPrivileges)
            {
                var view = i.view;
                if(i.sysComponentTypeID == 20)
                {
                    if (i.view == true)
                    {
                        string ip = GeneralFunctions.GetIPAddress();
                        if (db.tblIPsIndex.Count(m => m.ip == ip) > 0)
                            view = true;
                        else
                            view = false;
                    }
                    else
                        view = true;
                }
                
                listPrivileges.Add(new SysComponentsPrivilegesModel()
                {
                    ComponentID = int.Parse(i.componentID.ToString()),
                    Component = i.componentName,
                    Create = i.create,
                    Edit = i.edit,
                    //View = i.view,
                    //View = i.sysComponentTypeID == 20 ? i.view ? db.tblIPsIndex.Count(m => m.ip == GeneralFunctions.GetIPAddress()) > 0 ? true : false : true : i.view,
                    View = view,
                    Required = i.required ?? false,
                    ComponentOrder = int.Parse(i.compOrder.ToString()),
                    Alias = i.alias
                });
            }

            return listPrivileges;
        }

        public SysProfilePrivileges GetPrivileges(string roleID, int workgroupID)
        {
            ePlatEntities db = new ePlatEntities();
            SysProfilePrivileges model = new SysProfilePrivileges();
            List<SysComponentsPrivilegesModel> listPrivileges = new List<SysComponentsPrivilegesModel>();
            var role = Guid.Parse(roleID);
            //--get components privileges
            var queryPrivileges = from component in db.tblSysProfiles
                                  where component.roleID == role && component.sysWorkGroupID == workgroupID
                                  orderby component.sysComponentOrder
                                  select new
                                  {
                                      componentID = component.sysComponentID,
                                      componentName = component.tblSysComponents.sysComponent,
                                      create = component.create_,
                                      edit = component.edit_,
                                      view = component.view_,
                                      component.tblSysComponents.tblSysComponentAliases.FirstOrDefault(m => m.sysWorkGroupID == workgroupID).alias,
                                      compOrder = component.sysComponentOrder,
                                      component.tblSysComponents.sysComponentTypeID
                                  };
            foreach (var i in queryPrivileges)
            {
                listPrivileges.Add(new SysComponentsPrivilegesModel()
                {
                    ComponentID = int.Parse(i.componentID.ToString()),
                    Component = i.componentName,
                    Create = i.create,
                    Edit = i.edit,
                    View = i.view,
                    ComponentOrder = int.Parse(i.compOrder.ToString()),
                    Alias = i.alias
                });
            }

            //--get profile resorts
            var query = from resort in db.tblSysWorkGroups_Places
                        where resort.roleID == role
                        && resort.sysWorkGroupID == workgroupID
                        select new
                        {
                            resort.placeID,
                            place = resort.tblPlaces.place + " " + resort.tblPlaces.tblDestinations.destination
                        };
            var list = new List<SelectListItem>();
            foreach (var i in query)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.placeID.ToString(),
                    Text = i.place
                });
            }
            model.ListProfileResorts = list;
            model.ListSysComponentsPrivileges = listPrivileges;
            return model;
        }

        //public AttemptResponse SaveProfiles(ListProfileInfoModel list)
        //{
        //    ePlatEntities db = new ePlatEntities();
        //    AttemptResponse response = new AttemptResponse();
        //    Guid roleID = Guid.Parse(list.ProfileInfo_Role);
        //    //Guid.Parse(list.listProfileInfoModel.FirstOrDefault().ProfileInfo_Role);
        //    int workgroupID = int.Parse(list.ProfileInfo_SysworkGroup);
        //    //list.listProfileInfoModel.FirstOrDefault().ProfileInfo_WorkGroup;
        //    //--delete resorts previously saved on current profile
        //    var resortsToDelete = from resort in db.tblSysWorkGroups_Places
        //                          where resort.sysWorkGroupID == workgroupID
        //                          && resort.roleID == roleID
        //                          select resort;
        //    foreach (var i in resortsToDelete)
        //    {
        //        db.DeleteObject(i);
        //    }
        //    //--save resorts on current profile
        //    if (list.listProfileResorts != null)
        //    {
        //        foreach (var i in list.listProfileResorts)
        //        {
        //            var resort = new tblSysWorkGroups_Places();
        //            resort.roleID = roleID;
        //            resort.sysWorkGroupID = workgroupID;
        //            resort.placeID = long.Parse(i);
        //            resort.dateSaved = DateTime.Now;
        //            resort.savedByUserID = (Guid)Membership.GetUser().ProviderUserKey;
        //            db.AddObject("tblSysWorkGroups_Places", resort);
        //        }
        //    }
        //    //--save components privileges on profile
        //    if (list.listProfileInfoModel != null)
        //    {
        //        var idsNotSaved = "";
        //        for (var i = 0; i < list.listProfileInfoModel.Count(); i++)
        //        {
        //            try
        //            {
        //                //Guid roleID = Guid.Parse(list.listProfileInfoModel[i].ProfileInfo_Role);
        //                //int workgroupID = list.listProfileInfoModel[i].ProfileInfo_WorkGroup;
        //                int componentID = list.listProfileInfoModel[i].ProfileInfo_ComponentID;
        //                try
        //                {
        //                    var profile = db.tblSysProfiles.Where(m => m.roleID == roleID).Where(m => m.sysWorkGroupID == workgroupID).Where(m => m.sysComponentID == componentID).Single();
        //                    profile.create_ = list.listProfileInfoModel[i].ProfileInfo_Create;
        //                    profile.edit_ = list.listProfileInfoModel[i].ProfileInfo_Edit;
        //                    profile.view_ = list.listProfileInfoModel[i].ProfileInfo_View;
        //                    profile.sysComponentOrder = list.listProfileInfoModel[i].ProfileInfo_ComponentOrder;
        //                }
        //                catch
        //                {
        //                    var profile = new tblSysProfiles();
        //                    profile.roleID = Guid.Parse(list.listProfileInfoModel[i].ProfileInfo_Role);
        //                    profile.sysWorkGroupID = list.listProfileInfoModel[i].ProfileInfo_WorkGroup;
        //                    profile.sysComponentID = list.listProfileInfoModel[i].ProfileInfo_ComponentID;
        //                    profile.create_ = list.listProfileInfoModel[i].ProfileInfo_Create;
        //                    profile.edit_ = list.listProfileInfoModel[i].ProfileInfo_Edit;
        //                    profile.view_ = list.listProfileInfoModel[i].ProfileInfo_View;
        //                    profile.sysComponentOrder = list.listProfileInfoModel[i].ProfileInfo_ComponentOrder;
        //                    db.AddObject("tblSysProfiles", profile);
        //                }
        //                if (list.listProfileInfoModel[i].ProfileInfo_Alias != null)
        //                {
        //                    try
        //                    {
        //                        var query = db.tblSysComponentAliases.Where(m => m.sysComponentID == componentID).Where(m => m.sysWorkGroupID == workgroupID).Single();
        //                        query.alias = list.listProfileInfoModel[i].ProfileInfo_Alias;
        //                    }
        //                    catch
        //                    {
        //                        var alias = new tblSysComponentAliases();
        //                        alias.sysWorkGroupID = list.listProfileInfoModel[i].ProfileInfo_WorkGroup;
        //                        alias.alias = list.listProfileInfoModel[i].ProfileInfo_Alias;
        //                        alias.sysComponentID = list.listProfileInfoModel[i].ProfileInfo_ComponentID;
        //                        db.AddObject("tblSysComponentAliases", alias);
        //                    }
        //                }
        //                var component = db.tblSysComponents.Single(m => m.sysComponentID == componentID);
        //                component.url = list.listProfileInfoModel[i].ProfileInfo_Url;
        //                db.SaveChanges();
        //                response.Type = Attempt_ResponseTypes.Ok;
        //                response.Message = "Components Saved";
        //                response.ObjectID = 0;
        //            }
        //            catch (Exception ex)
        //            {
        //                idsNotSaved += list.listProfileInfoModel[i].ProfileInfo_ComponentID + ",";
        //                response.Exception = ex;
        //            }

        //        }
        //        if (idsNotSaved == "")
        //        {
        //            response.Message = "Components Saved";
        //            response.ObjectID = null;
        //        }
        //        else
        //        {
        //            if (response.Type == Attempt_ResponseTypes.Ok)
        //            {
        //                response.Type = Attempt_ResponseTypes.Error;
        //                response.ObjectID = 0;
        //                response.Message = "Components NOT Saved";
        //            }
        //            else
        //            {
        //                response.Type = Attempt_ResponseTypes.Warning;
        //                response.Message = "Some Componentes NOT Saved";
        //                response.ObjectID = new { ids = idsNotSaved };
        //            }
        //        }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            db.SaveChanges();
        //            response.Type = Attempt_ResponseTypes.Ok;
        //            response.Message = "Resorts Saved";
        //            response.ObjectID = 0;
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Type = Attempt_ResponseTypes.Error;
        //            response.Message = "Resorts NOT Saved";
        //            response.ObjectID = 0;
        //            response.Exception = ex;
        //        }
        //    }
        //    return response;
        //}

        public AttemptResponse UpdateComponentName(int componentID, string componentName)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                tblSysComponents component = db.tblSysComponents.Single(m => m.sysComponentID == componentID);
                component.sysComponent = componentName;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Component Name Updated";
                response.ObjectID = componentID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Component Name NOT Updated";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse RemoveComponent(int componentID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var _profiles = db.tblSysProfiles.Where(m => m.sysComponentID == componentID);
                var _aliases = db.tblSysComponentAliases.Where(m => m.sysComponentID == componentID);
                foreach (var i in _profiles)
                {
                    db.DeleteObject(i);
                }
                foreach (var i in _aliases)
                {
                    db.DeleteObject(i);
                }
                db.DeleteObject(db.tblSysComponents.Single(m => m.sysComponentID == componentID));
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Component Deleted";
                response.ObjectID = componentID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Component NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveComponent(ProfileInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            //int componentID = 0;
            if (model.ProfileInfo_ComponentID == 0)
            {
                try
                {
                    tblSysComponents component = new tblSysComponents();
                    component.sysComponent = model.ProfileInfo_Component;
                    component.sysComponentTypeID = model.ProfileInfo_ComponentType;
                    component.sysParentComponentID = model.ProfileInfo_ParentComponent;
                    component.description = model.ProfileInfo_Description;
                    component.tableName = model.ProfileInfo_TableName;
                    component.fieldName = model.ProfileInfo_FieldName;
                    db.tblSysComponents.AddObject(component);
                    db.SaveChanges();
                    response.Message = "Component Saved";
                    model.ProfileInfo_ComponentID = int.Parse(component.sysComponentID.ToString());
                    if (model.ProfileInfo_ComponentID != 0 && model.ProfileInfo_Role != Guid.Empty && model.ProfileInfo_WorkGroup != 0)
                    {
                        tblSysProfiles profile = new tblSysProfiles();
                        profile.roleID = model.ProfileInfo_Role;
                        profile.sysWorkGroupID = model.ProfileInfo_WorkGroup;
                        profile.sysComponentID = model.ProfileInfo_ComponentID;
                        profile.sysComponentOrder = model.ProfileInfo_ComponentOrder;
                        db.tblSysProfiles.AddObject(profile);
                        db.SaveChanges();
                        response.Message += " and added to profile";
                    }
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = component.sysComponentID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    var component = db.tblSysComponents.Single(m => m.sysComponentID == model.ProfileInfo_ComponentID);
                    if (model.ProfileInfo_Component != null)
                    {
                        component.sysComponent = model.ProfileInfo_Component;
                    }
                    if (model.ProfileInfo_ComponentType != 0)
                    {
                        component.sysComponentTypeID = model.ProfileInfo_ComponentType;
                    }
                    if (model.ProfileInfo_ParentComponent != 0)
                    {
                        component.sysParentComponentID = model.ProfileInfo_ParentComponent;
                    }
                    component.description = model.ProfileInfo_Description;
                    component.tableName = model.ProfileInfo_TableName;
                    component.fieldName = model.ProfileInfo_FieldName;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Component Updated";
                    response.ObjectID = component.sysComponentID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Component NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse SaveSysProfile(ProfileModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            DateTime now = DateTime.Now;
            try
            {
                var roleID = Guid.Parse(model.Profile_Role);
                var sysWorkGroupID = int.Parse(model.Profile_SysWorkGroup);

                #region "delete existing resorts in profile"
                foreach (var i in db.tblSysWorkGroups_Places.Where(m => m.roleID == roleID && m.sysWorkGroupID == sysWorkGroupID))
                {
                    db.DeleteObject(i);
                }
                #endregion

                #region "add resorts to profile"
                if (model.Profile_Resorts != null)
                {
                    foreach (var i in model.Profile_Resorts)
                    {
                        var resort = new tblSysWorkGroups_Places();
                        resort.roleID = roleID;
                        resort.sysWorkGroupID = sysWorkGroupID;
                        resort.placeID = long.Parse(i);
                        resort.savedByUserID = session.UserID;
                        resort.dateSaved = DateTime.Now;
                        db.tblSysWorkGroups_Places.AddObject(resort);
                    }
                }
                //add message of this proccess
                #endregion

                #region "save components in profile"
                //update or create profile
                if (model.Profile_ListComponents != null)
                {
                    foreach (var i in model.Profile_ListComponents)
                    {
                        //sysComponent
                        var sysComponent = db.tblSysComponents.Single(m => m.sysComponentID == i.ProfileInfo_ComponentID);
                        if (i.ProfileInfo_Url != null)
                        {
                            sysComponent.url = i.ProfileInfo_Url;
                        }
                        if (i.ProfileInfo_TableName != null)
                        {
                            sysComponent.tableName = i.ProfileInfo_TableName;
                        }
                        if (i.ProfileInfo_FieldName != null)
                        {
                            sysComponent.fieldName = i.ProfileInfo_FieldName;
                        }
                        try //if component has profile
                        {
                            //sysProfile
                            var sysProfile = db.tblSysProfiles.Single(m => m.roleID == roleID && m.sysWorkGroupID == sysWorkGroupID && m.sysComponentID == i.ProfileInfo_ComponentID);
                            sysProfile.create_ = i.ProfileInfo_Create;
                            sysProfile.edit_ = i.ProfileInfo_Edit;
                            sysProfile.view_ = i.ProfileInfo_View;
                            sysProfile.sysComponentOrder = i.ProfileInfo_ComponentID <= 4 ? i.ProfileInfo_ComponentID : i.ProfileInfo_ComponentOrder;
                            sysProfile.dateModified = now;
                            sysProfile.modifiedByUserID = session.UserID;
                            //alias
                            try //if alias exists
                            {
                                //if (sysComponent.sysParentComponentID != null)//prevent assign null value to main components
                                if (i.ProfileInfo_Alias != null)
                                {
                                    var alias = db.tblSysComponentAliases.Single(m => m.sysWorkGroupID == sysWorkGroupID && m.sysComponentID == i.ProfileInfo_ComponentID);
                                    alias.alias = i.ProfileInfo_Alias;
                                }
                            }
                            catch //if alias does not exist
                            {
                                var alias = new tblSysComponentAliases();
                                alias.sysComponentID = i.ProfileInfo_ComponentID;
                                alias.sysWorkGroupID = sysWorkGroupID;
                                alias.alias = i.ProfileInfo_Alias;
                                db.tblSysComponentAliases.AddObject(alias);
                            }
                        }
                        catch //if component does not have profile
                        {
                            var sysProfile = new tblSysProfiles();
                            sysProfile.roleID = roleID;
                            sysProfile.sysWorkGroupID = sysWorkGroupID;
                            sysProfile.sysComponentID = i.ProfileInfo_ComponentID;
                            sysProfile.create_ = i.ProfileInfo_Create;
                            sysProfile.edit_ = i.ProfileInfo_Edit;
                            sysProfile.view_ = i.ProfileInfo_View;
                            sysProfile.sysComponentOrder = i.ProfileInfo_ComponentID <= 4 ? i.ProfileInfo_ComponentID : i.ProfileInfo_ComponentOrder;
                            db.tblSysProfiles.AddObject(sysProfile);
                        }
                    }
                }
                #endregion

                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Profile Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Profile NOT Saved";
                response.Exception = ex;
                response.ObjectID = 0;
                return response;
            }
        }
        //public AttemptResponse SaveComponentsOrder(string[] componentsOrdered)
        //{
        //    ePlatEntities db = new ePlatEntities();
        //    AttemptResponse response = new AttemptResponse();
        //    try
        //    {
        //        var components = componentsOrdered.Select((i, v) => new { value = i, index = v });
        //        foreach (var i in components)
        //        {
        //            var component = db.tblSysComponents.Single(m => m.
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Type = Attempt_ResponseTypes.Error;
        //        response.Message = "Order Saved";
        //        response.ObjectID = 0;
        //        response.Exception = ex;
        //        return response;
        //    }
        //}

        //public AttemptResponse SaveComponent(ProfileInfoModel model)
        //{
        //    ePlatEntities db = new ePlatEntities();
        //    AttemptResponse response = new AttemptResponse();
        //    int componentID = 0;
        //    if (model.ProfileInfo_ComponentID == 0)
        //    {
        //        try
        //        {
        //            tblSysComponents component = new tblSysComponents();
        //            component.sysComponent = model.ProfileInfo_Component;
        //            component.sysComponentTypeID = model.ProfileInfo_ComponentTypeID;
        //            component.sysParentComponentID = model.ProfileInfo_ParentComponentID;
        //            component.description = model.ProfileInfo_Description;
        //            component.tableName = model.ProfileInfo_TableName;
        //            component.fieldName = model.ProfileInfo_TableField;
        //            db.AddObject("tblSysComponents", component);
        //            db.SaveChanges();
        //            response.Message = "Component Saved";
        //            model.ProfileInfo_ComponentID = int.Parse(component.sysComponentID.ToString());
        //            componentID = model.ProfileInfo_ComponentID;
        //            if (model.ProfileInfo_ComponentID != 0 && model.ProfileInfo_Role != "0" && model.ProfileInfo_WorkGroup != 0)
        //            {
        //                tblSysProfiles profile = new tblSysProfiles();
        //                profile.roleID = Guid.Parse(model.ProfileInfo_Role);
        //                profile.sysWorkGroupID = model.ProfileInfo_WorkGroup;
        //                profile.sysComponentID = componentID;
        //                profile.sysComponentOrder = model.ProfileInfo_ComponentOrder;
        //                db.AddObject("tblSysProfiles", profile);
        //                db.SaveChanges();
        //                response.Message += " and added to Profile";
        //            }
        //            response.Type = Attempt_ResponseTypes.Ok;
        //            response.ObjectID = component.sysComponentID;
        //            return response;
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Type = Attempt_ResponseTypes.Error;
        //            response.Message = "Component NOT Saved";
        //            response.Exception = ex;
        //            response.ObjectID = 0;
        //            return response;
        //        }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            var component = db.tblSysComponents.Single(m => m.sysComponentID == model.ProfileInfo_ComponentID);
        //            if (model.ProfileInfo_Component != null)
        //                component.sysComponent = model.ProfileInfo_Component;
        //            if (model.ProfileInfo_ComponentTypeID != 0)
        //                component.sysComponentTypeID = model.ProfileInfo_ComponentTypeID;
        //            if (model.ProfileInfo_ParentComponentID != 0)
        //                component.sysParentComponentID = model.ProfileInfo_ParentComponentID;
        //            component.description = model.ProfileInfo_Description;
        //            component.tableName = model.ProfileInfo_TableName;
        //            component.fieldName = model.ProfileInfo_TableField;
        //            db.SaveChanges();
        //            response.Message = "Component Updated";
        //            response.Type = Attempt_ResponseTypes.Ok;
        //            response.ObjectID = component.sysComponentID;
        //            return response;
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Type = Attempt_ResponseTypes.Error;
        //            response.Message = "Component NOT Updated";
        //            response.Exception = ex;
        //            response.ObjectID = 0;
        //            return response;
        //        }
        //    }
        //}

        public List<SelectListItem> GetDDLData(string path, string id)
        {
            var list = new List<SelectListItem>();
            switch (path)
            {
                case "WorkGroups":
                    {
                        list = AdminCatalogs.FillDrpWorkGroups();
                        break;
                    }
                case "Destination":
                    {
                        list = PlaceDataModel.GetResortsByDestination(int.Parse(id));
                        break;
                    }
                case "Roles":
                    {
                        list = AdminCatalogs.FillDrpRoles();
                        break;
                    }
            }
            return list;
        }

        public AttemptResponse ChangeComponentParent(int componentID, int parentComponentID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                db.tblSysComponents.Single(m => m.sysComponentID == componentID).sysParentComponentID = parentComponentID;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Component Updated";
                response.ObjectID = componentID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Component NOT Updated";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public ProfileInfoModel GetComponentInfo(int componentID)
        {
            ePlatEntities db = new ePlatEntities();
            ProfileInfoModel model = new ProfileInfoModel();
            try
            {
                var query = db.tblSysComponents.Single(m => m.sysComponentID == componentID);
                model.ProfileInfo_ComponentID = int.Parse(query.sysComponentID.ToString());
                model.ProfileInfo_Component = query.sysComponent;
                model.ProfileInfo_ComponentType = query.sysComponentTypeID;
                model.ProfileInfo_Description = query.description;
                model.ProfileInfo_TableName = query.tableName;
                model.ProfileInfo_FieldName = query.fieldName;
            }
            catch
            {
            }
            return model;
        }

        //roles
        public List<GenericListModel> SearchRoles(string role)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();

            var query = from search in db.aspnet_Roles
                        where (role == null || search.RoleName.Contains(role))
                        select search;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    Item2 = i.RoleId.ToString(),
                    ItemName = i.RoleName
                });
            }
            return list;
        }

        public AttemptResponse SaveRole(RoleInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.RoleInfo_RoleID != null)
            {
                try
                {
                    var role = Guid.Parse(model.RoleInfo_RoleID);
                    var query = db.aspnet_Roles.Single(m => m.RoleId == role);
                    query.RoleName = model.RoleInfo_Role;
                    query.LoweredRoleName = model.RoleInfo_Role.ToLower();
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Role Updated";
                    response.ObjectID = query.RoleId;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Role NOT Updated";
                    response.ObjectID = model.RoleInfo_RoleID;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    Roles.CreateRole(model.RoleInfo_Role);
                    aspnet_Roles role = db.aspnet_Roles.Single(m => m.RoleName == model.RoleInfo_Role);
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Role Saved";
                    response.ObjectID = role.RoleId;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Role NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public RoleInfoModel GetRole(string roleID)
        {
            ePlatEntities db = new ePlatEntities();
            RoleInfoModel model = new RoleInfoModel();
            var role = Guid.Parse(roleID);
            var query = db.aspnet_Roles.Single(m => m.RoleId == role);
            model.RoleInfo_RoleID = query.RoleId.ToString();
            model.RoleInfo_Role = query.RoleName;
            return model;
        }

        public AttemptResponse DeleteRole(string roleID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var role = Guid.Parse(roleID);
                var query = db.aspnet_Roles.Single(m => m.RoleId == role);
                Roles.DeleteRole(query.RoleName);
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Role Deleted";
                response.ObjectID = roleID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Role NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        //workgroups
        public AttemptResponse AssignWorkGroupToUser(int workgroupID, string roleID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var workgroup = new tblUsers_SysWorkGroups();
                //workgroup.userID = (Guid)Membership.GetUser().ProviderUserKey;
                workgroup.userID = session.UserID;
                workgroup.sysWorkGroupID = Int32.Parse(workgroupID.ToString());
                workgroup.roleID = Guid.Parse(roleID.ToString());
                workgroup.dateSaved = DateTime.Now;
                workgroup.savedByUserID = session.UserID;
                db.tblUsers_SysWorkGroups.AddObject(workgroup);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = workgroupID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.ObjectID = 0;
                return response;
            }
        }

        public List<GenericListModel> SearchWorkGroups(string workgroup)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            var query = from search in db.tblSysWorkGroups
                        where (workgroup == null || search.sysWorkGroup.Contains(workgroup))
                        select search;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.sysWorkGroupID,
                    ItemName = i.sysWorkGroup
                });
            }
            return list;
        }

        public AttemptResponse SaveWorkGroup(WorkGroupInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.WorkGroupInfo_WorkGroupID != 0)
            {
                try
                {
                    var query = db.tblSysWorkGroups.Single(m => m.sysWorkGroupID == model.WorkGroupInfo_WorkGroupID);
                    query.sysWorkGroup = model.WorkGroupInfo_WorkGroup;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "WorkGroup Updated";
                    response.ObjectID = query.sysWorkGroupID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "WorkGroup NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblSysWorkGroups workgroup = new tblSysWorkGroups();
                    workgroup.sysWorkGroup = model.WorkGroupInfo_WorkGroup;
                    db.tblSysWorkGroups.AddObject(workgroup);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "WorkGroup Saved";
                    response.ObjectID = workgroup.sysWorkGroupID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "WorkGroup NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public WorkGroupInfoModel GetWorkGroup(int workgroupID)
        {
            ePlatEntities db = new ePlatEntities();
            WorkGroupInfoModel model = new WorkGroupInfoModel();
            var query = db.tblSysWorkGroups.Single(m => m.sysWorkGroupID == workgroupID);
            model.WorkGroupInfo_WorkGroupID = query.sysWorkGroupID;
            model.WorkGroupInfo_WorkGroup = query.sysWorkGroup;
            return model;
        }

        public AttemptResponse DeleteWorkGroup(int workgroupID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblSysWorkGroups.Single(m => m.sysWorkGroupID == workgroupID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "WorkGroup Deleted";
                response.ObjectID = workgroupID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "WorkGroup NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public List<GenericListModel> GetAllBookingStatus()
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            foreach (var i in db.tblBookingStatus)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.bookingStatusID,
                    ItemName = i.bookingStatus
                });
            }
            return list;
        }

        public List<GenericListModel> GetActiveBookingStatusPerWorkGroup(int sysWorkGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            var query = from workgroup in db.tblSysWorkGroups_BookingStatus
                        where workgroup.sysWorkGroupID == sysWorkGroupID
                        orderby workgroup.orderIndex
                        select workgroup;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.bookingStatusID,
                    Item2 = i.orderIndex.ToString()
                });
            }
            return list;
        }

        public AttemptResponse SaveBookingStatusPerWorkGroup(int workgroupID, int[] bookingStatus)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = from status in db.tblSysWorkGroups_BookingStatus
                            where status.sysWorkGroupID == workgroupID
                            select status;
                foreach (var i in query)
                {
                    db.tblSysWorkGroups_BookingStatus.DeleteObject(i);
                }
                dynamic bookingStatusIndexes;
                if (bookingStatus != null)
                {
                    bookingStatusIndexes = bookingStatus.Select((i, v) => new { value = i, index = v });
                    foreach (var i in bookingStatusIndexes)
                    {
                        tblSysWorkGroups_BookingStatus status = new tblSysWorkGroups_BookingStatus();
                        status.sysWorkGroupID = workgroupID;
                        status.bookingStatusID = i.value;
                        status.orderIndex = i.index + 1;
                        db.tblSysWorkGroups_BookingStatus.AddObject(status);
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Booking Status Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Booking Status NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveBookingStatus(BookingStatusInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.BookingStatusInfo_BookingStatusID != 0)
            {
                try
                {
                    var status = db.tblBookingStatus.Single(m => m.bookingStatusID == model.BookingStatusInfo_BookingStatusID);
                    status.bookingStatus = model.BookingStatusInfo_BookingStatus;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Booking Status Updated";
                    response.ObjectID = status.bookingStatusID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Booking Status NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblBookingStatus status = new tblBookingStatus();
                    status.bookingStatus = model.BookingStatusInfo_BookingStatus;
                    db.tblBookingStatus.AddObject(status);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Booking Status Saved";
                    response.ObjectID = status.bookingStatusID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Booking Status NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public List<GenericListModel> GetAllLeadTypes()
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            foreach (var i in db.tblLeadTypes)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.leadTypeID,
                    ItemName = i.leadType
                });
            }
            return list;
        }

        public List<GenericListModel> GetActiveLeadTypesPerWorkGroup(int sysWorkGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            var query = from workgroup in db.tblLeadTypes_SysWorkGroups
                        where workgroup.sysWorkGroupID == sysWorkGroupID
                        select workgroup;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.leadTypeID
                });
            }
            return list;
        }

        public AttemptResponse SaveLeadTypesPerWorkGroup(int workgroupID, int[] leadTypes)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = from leadType in db.tblLeadTypes_SysWorkGroups
                            where leadType.sysWorkGroupID == workgroupID
                            select leadType;
                foreach (var i in query)
                    db.tblLeadTypes_SysWorkGroups.DeleteObject(i);
                if (leadTypes != null)
                {
                    foreach (var i in leadTypes)
                    {
                        tblLeadTypes_SysWorkGroups leadType = new tblLeadTypes_SysWorkGroups();
                        leadType.sysWorkGroupID = workgroupID;
                        leadType.leadTypeID = i;
                        db.tblLeadTypes_SysWorkGroups.AddObject(leadType);
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Lead Types Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Lead Types NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveLeadType(LeadTypeInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.LeadTypeInfo_LeadTypeID != 0)
            {
                try
                {
                    var query = db.tblLeadTypes.Single(m => m.leadTypeID == model.LeadTypeInfo_LeadTypeID);
                    query.leadType = model.LeadTypeInfo_LeadType;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Lead Type Updated";
                    response.ObjectID = query.leadTypeID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Lead Type NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblLeadTypes query = new tblLeadTypes();
                    query.leadType = model.LeadTypeInfo_LeadType;
                    db.tblLeadTypes.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Lead Type Saved";
                    response.ObjectID = query.leadTypeID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Lead Type NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public List<GenericListModel> GetAllLeadSources()
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            foreach (var i in db.tblLeadSources)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = int.Parse(i.leadSourceID.ToString()),
                    ItemName = i.leadSource,
                    Item2 = i.leadSourceInitials
                });
            }
            return list;
        }

        public List<GenericListModel> GetActiveLeadSourcesPerWorkGroup(int sysWorkGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            var query = from workgroup in db.tblLeadSources_SysWorkGroups
                        where workgroup.sysWorkGroupID == sysWorkGroupID
                        select workgroup;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = int.Parse(i.leadSourceID.ToString())
                });
            }
            return list;
        }

        public AttemptResponse SaveLeadSourcesPerWorkGroup(int workgroupID, int[] leadSources)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = from leadSource in db.tblLeadSources_SysWorkGroups
                            where leadSource.sysWorkGroupID == workgroupID
                            select leadSource;
                foreach (var i in query)
                    db.tblLeadSources_SysWorkGroups.DeleteObject(i);
                if (leadSources != null)
                {
                    foreach (var i in leadSources)
                    {
                        tblLeadSources_SysWorkGroups leadSource = new tblLeadSources_SysWorkGroups();
                        leadSource.sysWorkGroupID = workgroupID;
                        leadSource.leadSourceID = i;
                        db.tblLeadSources_SysWorkGroups.AddObject(leadSource);
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Lead Sources Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Lead Sources NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveLeadSource(LeadSourceInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.LeadSourceInfo_LeadSourceID != 0)
            {
                try
                {
                    var query = db.tblLeadSources.Single(m => m.leadSourceID == model.LeadSourceInfo_LeadSourceID);
                    query.leadSource = model.LeadSourceInfo_LeadSource;
                    query.leadSourceInitials = model.LeadSourceInfo_LeadSourceInitials;
                    query.active = model.LeadSourceInfo_IsActive;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Lead Source Updated";
                    response.ObjectID = query.leadSourceID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Lead Source NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblLeadSources query = new tblLeadSources();
                    query.leadSource = model.LeadSourceInfo_LeadSource;
                    query.leadSourceInitials = model.LeadSourceInfo_LeadSourceInitials;
                    query.active = model.LeadSourceInfo_IsActive;
                    db.tblLeadSources.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Lead Source Saved";
                    response.ObjectID = query.leadSourceID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Lead Source NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public List<GenericListModel> GetAllQualificationRequirements()
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            foreach (var i in db.tblQualificationRequirements)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.qualificationRequirementID,
                    ItemName = i.qualificationRequirement
                });
            }
            return list;
        }

        public List<GenericListModel> GetActiveQualificationRequirementsPerWorkGroup(int sysWorkGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            var query = from workgroup in db.tblSysWorkGroups_QualificationRequirements
                        where workgroup.sysWorkGroupID == sysWorkGroupID
                        select workgroup;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.qualificationRequirementID
                });
            }
            return list;
        }

        public AttemptResponse SaveQualificationRequirementsPerWorkGroup(int workgroupID, int[] qualificationRequirements)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = from requirement in db.tblSysWorkGroups_QualificationRequirements
                            where requirement.sysWorkGroupID == workgroupID
                            select requirement;
                foreach (var i in query)
                    db.tblSysWorkGroups_QualificationRequirements.DeleteObject(i);
                if (qualificationRequirements != null)
                {
                    foreach (var i in qualificationRequirements)
                    {
                        tblSysWorkGroups_QualificationRequirements requirement = new tblSysWorkGroups_QualificationRequirements();
                        requirement.sysWorkGroupID = workgroupID;
                        requirement.qualificationRequirementID = i;
                        db.tblSysWorkGroups_QualificationRequirements.AddObject(requirement);
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Qualification Requirements Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Qualification Requirements NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveQualificationRequirement(QualificationRequirementInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.QualificationRequirementInfo_QualificationRequirementID != 0)
            {
                try
                {
                    var requirement = db.tblQualificationRequirements.Single(m => m.qualificationRequirementID == model.QualificationRequirementInfo_QualificationRequirementID);
                    requirement.qualificationRequirement = model.QualificationRequirementInfo_QualificationRequirement;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Qualification Requirement Updated";
                    response.ObjectID = requirement.qualificationRequirementID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Qualification Requirement NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblQualificationRequirements requirement = new tblQualificationRequirements();
                    requirement.qualificationRequirement = model.QualificationRequirementInfo_QualificationRequirement;
                    db.tblQualificationRequirements.AddObject(requirement);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Qualification Requirement Saved";
                    response.ObjectID = requirement.qualificationRequirementID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Qualification Requirement NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        /**/
        public List<GenericListModel> GetAllResortFeeTypes()
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            foreach (var i in db.tblResortFeeTypes)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.resortFeeTypeID,
                    ItemName = i.resortFeeType
                });
            }
            return list;
        }

        public List<GenericListModel> GetActiveResortFeeTypesPerWorkGroup(int sysWorkGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();

            var query = from workgroup in db.tblSysWorkGroups_ResortFeeTypes
                        where workgroup.sysWorkGroupID == sysWorkGroupID
                        select workgroup;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.resortFeeTypeID
                });
            }
            return list;
        }

        public AttemptResponse SaveResortFeeTypesPerWorkGroup(int workgroupID, int[] resortFeeTypes)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = from feeType in db.tblSysWorkGroups_ResortFeeTypes
                            where feeType.sysWorkGroupID == workgroupID
                            select feeType;
                foreach (var i in query)
                    db.tblSysWorkGroups_ResortFeeTypes.DeleteObject(i);
                if (resortFeeTypes != null)
                {
                    foreach (var i in resortFeeTypes)
                    {
                        tblSysWorkGroups_ResortFeeTypes feeType = new tblSysWorkGroups_ResortFeeTypes();
                        feeType.sysWorkGroupID = workgroupID;
                        feeType.resortFeeTypeID = i;
                        db.tblSysWorkGroups_ResortFeeTypes.AddObject(feeType);
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Resort Fee Types Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Resort Fee Types NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveResortFeeType(ResortFeeTypeInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.ResortFeeTypeInfo_ResortFeeTypeID != 0)
            {
                try
                {
                    var query = db.tblResortFeeTypes.Single(m => m.resortFeeTypeID == model.ResortFeeTypeInfo_ResortFeeTypeID);
                    query.resortFeeType = model.ResortFeeTypeInfo_ResortFeeType;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Resort Fee Type Updated";
                    response.ObjectID = query.resortFeeTypeID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Resort Fee Type NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblResortFeeTypes feeType = new tblResortFeeTypes();
                    feeType.resortFeeType = model.ResortFeeTypeInfo_ResortFeeType;
                    db.tblResortFeeTypes.AddObject(feeType);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Resort Fee Type Saved";
                    response.ObjectID = feeType.resortFeeTypeID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Resort Fee Type NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public List<GenericListModel> GetAllReservationStatus()
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            foreach (var i in db.tblReservationStatus)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.reservationStatusID,
                    ItemName = i.reservationStatus
                });
            }
            return list;
        }

        public List<GenericListModel> GetActiveReservationStatusPerWorkGroup(int sysWorkGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();

            var query = from workgroup in db.tblReservationStatus_SysWorkGroups
                        where workgroup.sysWorkGroupID == sysWorkGroupID
                        select workgroup;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.reservationStatusID
                });
            }
            return list;
        }

        public AttemptResponse SaveReservationStatusPerWorkGroup(int workgroupID, int[] reservationStatus)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = from status in db.tblReservationStatus_SysWorkGroups
                            where status.sysWorkGroupID == workgroupID
                            select status;
                foreach (var i in query)
                    db.tblReservationStatus_SysWorkGroups.DeleteObject(i);
                if (reservationStatus != null)
                {
                    foreach (var i in reservationStatus)
                    {
                        tblReservationStatus_SysWorkGroups status = new tblReservationStatus_SysWorkGroups();
                        status.sysWorkGroupID = workgroupID;
                        status.reservationStatusID = i;
                        db.tblReservationStatus_SysWorkGroups.AddObject(status);
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Reservation Status Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Reservation Status NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveReservationStatus(ReservationStatusInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.ReservationStatusInfo_ReservationStatusID != 0)
            {
                try
                {
                    var query = db.tblReservationStatus.Single(m => m.reservationStatusID == model.ReservationStatusInfo_ReservationStatusID);
                    query.reservationStatus = model.ReservationStatusInfo_ReservationStatus;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Reservation Status Updated";
                    response.ObjectID = query.reservationStatusID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Reservation Status NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblReservationStatus status = new tblReservationStatus();
                    status.reservationStatus = model.ReservationStatusInfo_ReservationStatus;
                    db.tblReservationStatus.AddObject(status);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Reservation Status Saved";
                    response.ObjectID = status.reservationStatusID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Reservation Status NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }
        /**/
        public List<GenericListModel> GetAllVerificationAgreements()
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            foreach (var i in db.tblVerificationAgreements)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.verificationAgreementID,
                    ItemName = i.verificationAgreement
                });
            }
            return list;
        }

        public List<GenericListModel> GetActiveVerificationAgreementsPerWorkGroup(int sysWorkGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();

            var query = from workgroup in db.tblSysWorkGroups_VerificationAgreements
                        where workgroup.sysWorkGroupID == sysWorkGroupID
                        select workgroup;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.verificationAgreementID
                });
            }
            return list;
        }

        public AttemptResponse SaveVerificationAgreementsPerWorkGroup(int workgroupID, int[] verificationAgreements)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = from status in db.tblSysWorkGroups_VerificationAgreements
                            where status.sysWorkGroupID == workgroupID
                            select status;
                foreach (var i in query)
                    db.tblSysWorkGroups_VerificationAgreements.DeleteObject(i);
                if (verificationAgreements != null)
                {
                    foreach (var i in verificationAgreements)
                    {
                        tblSysWorkGroups_VerificationAgreements status = new tblSysWorkGroups_VerificationAgreements();
                        status.sysWorkGroupID = workgroupID;
                        status.verificationAgreementID = i;
                        db.tblSysWorkGroups_VerificationAgreements.AddObject(status);
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Verification Agreement Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Verification Agreement NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveVerificationAgreement(VerificationAgreementInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.VerificationAgreementInfo_VerificationAgreementID != 0)
            {
                try
                {
                    var query = db.tblVerificationAgreements.Single(m => m.verificationAgreementID == model.VerificationAgreementInfo_VerificationAgreementID);
                    query.verificationAgreement = model.VerificationAgreementInfo_VerificationAgreement;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Verification Agreement Updated";
                    response.ObjectID = query.verificationAgreementID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Verification Agreement NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblVerificationAgreements status = new tblVerificationAgreements();
                    status.verificationAgreement = model.VerificationAgreementInfo_VerificationAgreement;
                    db.tblVerificationAgreements.AddObject(status);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Verification Agreement Saved";
                    response.ObjectID = status.verificationAgreementID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Verification Agreement NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public static bool VerifyAccess()
        {
            ePlatEntities db = new ePlatEntities();
            bool Access = false;
            string url = System.Web.HttpContext.Current.Request.Url.AbsolutePath;//url del usuario
            var _queryUserAccess = from sWG in db.tblUsers_SysWorkGroups
                                   join sP in db.tblSysProfiles on sWG.sysWorkGroupID equals sP.sysWorkGroupID
                                   where (sWG.userID == session.UserID && sP.roleID == sWG.roleID)// select workgroup and role from user
                                   && sP.tblSysComponents.tblSysComponentTypes.sysComponentTypeID == 2//component type url
                                   select new
                                   {
                                       UserID = sWG.userID,
                                       SysProfileID = sP.sysProfileID,
                                       Role = sWG.aspnet_Roles.RoleName,
                                       WorkG = sWG.sysWorkGroup_UserID,
                                       sysComID = sP.sysComponentID,
                                       Create = sP.create_,
                                       Edit = sP.edit_,
                                       View = sP.view_
                                   };

            if (url.Substring(url.Length - 1, 1) == "/")
            {
                url = url.Substring(0, url.Length - 1);
            }
            var _queryAccessPath = db.tblSysComponents.FirstOrDefault(item => item.url == url);//
            var Validate = _queryUserAccess.FirstOrDefault(item => item.UserID == session.UserID && item.sysComID == _queryAccessPath.sysComponentID && item.View == true);
            var ValidateAdmin = _queryUserAccess.FirstOrDefault(item => item.UserID == session.UserID && item.Role.Contains("dministrator"));//UserRole Administrator

            if (Validate != null || (ValidateAdmin != null && Validate.Role == "Administrator"))//si el usuario es administrador access = true
            {
                Access = true;
            }
            return Access;
        }
        public static void CreateTableUserLogActivity()
        {
            ePlatEntities db = new ePlatEntities();
            DateTime YearAndMoth = DateTime.Now;

            string tableName = "";
            tableName += "tblUserLogActivity" + YearAndMoth.ToString("yyyyMM");

            var Check = db.sp_tableExists(tableName);
            var Exist = Check.FirstOrDefault().Value;

            if (Exist == 0)
            {
                db.sp_createUserLogActivityTable(tableName);
            }
        }
        //public static void CreateTableUserLogActivity(string item, string type, string attempt)
        //public static void CreateTableUserLogActivity(dynamic _item, string type, string attempt, string url = "", string urlMethod = "", RouteValueDictionary controller = null, string method = "")
        
        public static void CreateTableUserLogActivity(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null)
        {
            ePlatEntities db = new ePlatEntities();
            DateTime Date = DateTime.Now;
            JavaScriptSerializer Json = new JavaScriptSerializer();
            
            try
            {
                var url = request.UrlReferrer.ToString();
                var controller = request.RequestContext.RouteData.Values;
                var method = request.HttpMethod.ToString();
                var UserID = session.UserID;
                var User = db.tblUserProfiles.Single(x => x.userID == UserID);
                var contactInfo = "'False'";

                var item = Json.Serialize(_item);
                var JsonItem = Json.Deserialize<dynamic>(item);
                var attempt = _attempt != null ? Json.Serialize(_attempt) : null;
                var JsonAttempt = attempt != null ? Json.Deserialize<dynamic>(attempt) : "";
                string TableName = "";
                TableName = "tblUserLogActivity" + Date.ToString("yyyyMM");
                AdminDataModel.CreateTableUserLogActivity();
                var data = JsonItem.GetType();
                if (type != "Delete" && data.IsGenericType)
                {
                    foreach (var x in item.GetType().GetProperties())
                    {
                        if ((x.Name.ToLower().Contains("email") && x.GetValue(x, null) != null) || (x.Name.ToLower().Contains("phone") && x.GetValue(x, null) != null))
                        {
                            contactInfo = "'true'";
                            break;
                        }
                    }
                }
                string Insert = "INSERT INTO "
                + TableName
                + " (UserID, DateSave, Controller, Method, JsonModel, Description, UrlMethod, Url, ContactInfo) VALUES";
                string InsertValuesRow = " ('" + UserID + "','" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "','" + controller["controller"] + "Controller'" + ",'" + method + "','" + item + " " + "','" + controller["action"] + " > " + User.firstName + " ";

                //CASOS
                #region "casos"
                switch (type)
                {
                    case "Delete":
                        #region
                        {
                            if (JsonAttempt["Type"] == 1)
                            {
                                InsertValuesRow += "borro el objeto con el ID: " + item + ", Mensaje: " + JsonAttempt["Message"];
                            }
                            else
                            {
                                InsertValuesRow += "intento borrar esto: " + item + ", Mensaje: " + JsonAttempt["Message"] + ", Exeption: " + JsonItem["Exception"];
                            }
                            break;
                        }
                        #endregion
                    case "Search":
                        #region
                        {
                            InsertValuesRow += "busco esto: \n"; /*item*/
                            //UserActivityLogs modelInfo = DefaultOption(item);
                            UserActivityLogs modelInfo = DefaultOption(JsonItem);
                            InsertValuesRow += structureValuesRow(modelInfo);
                            #region SearchCloseOut and SearchCloseOutHistory
                            //var TerminalID = 0;
                            //List<int> PointOfSaleID = new List<int>();
                            // Guid AgentID = Guid.Empty;
                            //foreach (var n in JsonItem)
                            //{
                            //    InsertValuesRow += n.Key.Contains("FromDate")? "Date Range: " + n.Value.ToString() : "";
                            //    InsertValuesRow += n.Key.Contains("ToDate")? " to " + n.Value.ToString() + ", \n":"";
                            //    InsertValuesRow += n.Key.Contains("SearchCloseOut_Date") ? "Date: " + n.Value.ToString()+", ": "";
                            //    //buscar coincidencias                           
                            //    if (n.Key.Contains("TerminalID"))//Valor TerminalID -select one- "Required"
                            //    {
                            //        TerminalID = n.Value;
                            //        InsertValuesRow += " Terminal: " + db.tblTerminals.FirstOrDefault(x => x.terminalID == TerminalID).terminal + ", \n";
                            //    }
                            //    if (n.Key.Contains("PointOfSaleID"))// si hay valor PointOfSaleID -multiselect-
                            //    {
                            //        if (n.Value == null)// sino hay valor  entonces "All"
                            //           InsertValuesRow += "todos los puntos de venta, \n";//

                            //        else if(n.Value.GetType() != typeof(int))//si el valor es diferente de entero entonces valor  ==  object
                            //        {
                            //            for (int z = 0; z < n.Value.Length; z++)
                            //            {
                            //                PointOfSaleID.Add(int.Parse(n.Value[z].ToString()));//
                            //            }
                            //            if (PointOfSaleID.Count(x => x != 0) > 0)
                            //            {
                            //                var PointOfSale = db.tblPointsOfSale.Where(x => PointOfSaleID.Contains(x.pointOfSaleID));//point of sale
                            //                InsertValuesRow += "en los puntos de venta: \n";
                            //                foreach (var x in PointOfSale)
                            //                {
                            //                    InsertValuesRow += x.shortName + " - " + x.pointOfSale + ", \n";
                            //                }
                            //            }
                            //        }
                            //        else//si el valor es entero entonces valor = ID
                            //        {   int POSID = n.Value; //pos= point of sale
                            //            InsertValuesRow += "en el punto de venta: \n" + db.tblPointsOfSale.FirstOrDefault(x => x.pointOfSaleID == POSID).pointOfSale+",";
                            //        }
                            //    }                            
                            //    if (n.Key.Contains("SalesAgentID"))//si hay SalesAgentID muestra: 
                            //    {
                            //       Guid AgentID = n.Value== null ? Guid.Empty : Guid.Parse(n.Value.ToString());//AgentID null = All-Agents
                            //       InsertValuesRow += AgentID != Guid.Empty ? " con el agente: " + db.tblUserProfiles.FirstOrDefault(x => x.userID == AgentID).firstName+",": " con todos los agentes";
                            //    }
                            //}
                            #endregion
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
                                    InsertValuesRow += "intento actualizar estos datos: ";//
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
                                    InsertValuesRow += "intento guardar estos datos: " + "Mensaje:" + JsonAttempt["Message"] + ", Exeption:" + JsonAttempt["Exception"] + "\n";
                                }
                                else
                                {
                                    InsertValuesRow += "guardo estos datos: " + " mensaje: " + JsonAttempt["Message"] + ",";
                                }
                            }
                            UserActivityLogs modelInfo = DefaultOption(item);
                            InsertValuesRow += structureValuesRow(modelInfo);
                            InsertValuesRow.Count();
                            break;
                        }
                        #endregion
                    case "Get":
                        #region
                        {
                            InsertValuesRow += "obtuvo estos datos: ";
                            //UserActivityLogs modelInfo = DefaultOption(item);
                            //InsertValuesRow += structureValuesRow(modelInfo);
                            //if (JsonItem.GetType().IsGenericType)
                            if (data.IsGenericType)
                            {
                                foreach (var x in JsonItem)
                                {
                                    if ((x.Key.Contains("CloseOutID") || x.Key.Contains("Company") || x.Key.Contains("PointOfSale") || x.Key.Contains("Promotion") || x.Key.Contains("Destination") || x.Key.Contains("Exchange") ||
                                         x.Key.Contains("Agent") || x.Key.Contains("Date") || x.Key.Contains("Invoice") || x.Key.Contains("ID") || x.Key.Contains("Terminal") || x.Key.Contains("Description") || x.Key.Contains("Budget") ||
                                         x.Key.Contains("email") || x.Key.Contains("Email") || x.Key.Contains("phone") || x.Key.Contains("Phone") || x.Key.Contains("Folio") || x.Key.Contains("folio") || x.Key.Contains("User") || x.Key.Contains("user")
                                         || x.Key.Contains("Status"))
                                         && x.Value is string)
                                    {
                                        if (x.Key != null && x.Value != null)
                                            InsertValuesRow += " \n" + x.Key.ToString() + ":" + x.Value + ", ";
                                    }
                                }
                            }
                            else
                                InsertValuesRow += JsonItem;
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
                                    InsertValuesRow += "estos datos fueron actualizados: ";
                                }
                                #region Old
                                foreach (var x in JsonItem)
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
                                    InsertValuesRow += "intento guardar estos datos: " + "Mensaje:" + JsonAttempt["Message"] + ", Exeption:" + JsonAttempt["Exception"] + "\n";
                                }
                                else
                                {
                                    InsertValuesRow += "guardo estos datos: " + " mensaje: " + JsonAttempt["Message"] + ",";
                                }
                                #region Old
                                foreach (var x in JsonItem)
                                {
                                    if (x.Key.Contains("JsonModel"))
                                    {
                                        var CloseOutDetails = Json.Deserialize<object>(x.Value);
                                        foreach (var z in CloseOutDetails)
                                        {
                                            if (z.Key.Contains("Company") || z.Key.Contains("PointOfSale") || z.Key.Contains("Agent") || z.Key.Contains("FromDate") || z.Key.Contains("ToDate") || z.Key.Contains("CloseOutDate"))
                                            {
                                                InsertValuesRow += " " + z.Key.ToString() + ":" + z.Value.ToString() + ", \n";
                                            }
                                        }
                                    }
                                    else if (x.Key.Contains("Company") || x.Key.Contains("PointOfSale") || x.Key.Contains("Agent") || x.Key.Contains("FromDate") || x.Key.Contains("ToDate") || x.Key.Contains("CloseOutDate"))
                                    {
                                        InsertValuesRow += " " + x.Key.ToString() + ":" + x.Value.ToString() + ", \n";
                                    }
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
                                InsertValuesRow += "Restauro la Actividad con el ID: " + item + ", Mensaje: " + JsonAttempt["Message"];
                            }
                            else
                            {
                                InsertValuesRow += "Intento Restaurar la actividad: " + item + ", Mensaje: " + JsonAttempt["Message"] + ", Exeption: " + JsonItem["Exception"];
                            }
                            break;
                        }
                        #endregion
                    case "UpdateStock":
                        #region "Updatestock"
                        {
                            if (JsonAttempt["Type"] == 1)
                            {
                                InsertValuesRow += "Actualizo el Stock " + item + ", Mensaje: " + JsonAttempt["Message"];
                            }
                            else
                            {
                                InsertValuesRow += "Intento Actualizar el Stock " + item + ", Mensaje: " + JsonAttempt["Message"] + ", Exeption: " + JsonItem["Exception"];
                            }
                            break;
                        }
                        #endregion
                    case "MasterChart":
                        #region MasterChart
                        {
                            InsertValuesRow += "obtuvo estos datos: ";
                            var cont = new List<object>();
                            foreach (var Item in JsonItem)
                                cont.Add(new { item });

                            foreach (var Item in JsonItem)
                            {
                                if (Item.Value != null && Item.Key != "CouponsPrivileges" && Item.Value.GetType() == typeof(object[]) && cont.Count() < 8)
                                {
                                    foreach (var ItemValue in Item.Value)
                                    {
                                        foreach (KeyValuePair<string, object> x in ItemValue)
                                        {
                                            if (x.Value != null && x.Value.GetType() != typeof(object[]))
                                                InsertValuesRow += x.Key + ":" + x.Value + ",\n";
                                        }
                                    }
                                }
                                else if (Item.Key.Contains("PurchaseID") && Item.Value != null || Item.Key.Contains("Lead") && Item.Value != null || Item.Key.Contains("_Culture") && Item.Value != null || Item.Key.Contains("DateTime") && Item.Value != null
                                         || Item.Key.Contains("PurchaseID") && Item.Value != null || Item.Key.Contains("TerminalString") && Item.Value != null || Item.Key.Contains("_Terminal") && Item.Value != null || Item.Key.Contains("Total")
                                         && Item.Value != null || Item.Key.Contains("Paid") && Item.Value != null || Item.Key.Contains("Currency") && Item.Value != null || Item.Key.Contains("_PurchaseStatus") && Item.Value != null || Item.Key.Contains("PurchaseStatusString")
                                         && Item.Value != null || Item.Key.Contains("_Agent") && Item.Value != null || Item.Key.Contains("_PointOfSale") && Item.Value != null || Item.Key.Contains("PointOfSaleString") && Item.Value != null && Item.Value.GetType() != typeof(object[]))
                                    InsertValuesRow += Item.Key + ":" + Item.Value + ",\n";
                            }
                            break;
                        }
                        #endregion
                    default:
                        {
                            InsertValuesRow += "Info: \n";
                            UserActivityLogs modelInfo = DefaultOption(item);
                            InsertValuesRow += structureValuesRow(modelInfo);
                            break;
                        }
                }
                #endregion
                InsertValuesRow += "','" + urlMethod + "','" + url + "'," + contactInfo + ");";
                var query = Insert + InsertValuesRow;
                //db.ExecuteStoreCommand(query);
                //return 1;
            }
            catch (Exception ex)
            {
               // UserLogsActivitySendEx(ex, urlMethod, request);
                //return 1;
            }
        }
        //public static UserActivityLogs DefaultOption(string JsonModel)
        public static UserActivityLogs DefaultOption(dynamic JsonModel)
        {
            UserActivityLogs Logs = new UserActivityLogs();
            JavaScriptSerializer Json = new JavaScriptSerializer();
            
            string Values = "";
            Logs.asignation = new List<UserActivityLogs.LogsAsignation>();
            var json = Json.Deserialize<dynamic>(JsonModel);
            foreach (var model in json)
            {
                if (model.Value == null)
                {
                    UserActivityLogs.LogsAsignation newLog = new UserActivityLogs.LogsAsignation();
                    newLog.logsName = model.Key;
                    newLog.logsValue = (string)null;
                    Logs.asignation.Add(newLog);
                    Values += " " + model.Key + ":";
                    Values += (string)null + "null,\n";
                }
                else if (model.Value.GetType() == typeof(object[]))
                {
                    UserActivityLogs.LogsAsignation newlog = new UserActivityLogs.LogsAsignation();
                    newlog.logsName = model.Key;
                    newlog.loglist = new List<UserActivityLogs.logAsignationlist>();
                    for (int x = 0; x < model.Value.Length; x++)
                    {
                        UserActivityLogs.logAsignationlist newLoglist = new UserActivityLogs.logAsignationlist();
                        newLoglist.listLogsName = model.Key;
                        Values += " " + model.Key + "{ \n";
                        newLoglist.ListLogValues = new List<UserActivityLogs.Items>();
                        //if value == null
                        var tipo = model.Value[x] != null ? model.Value[x].GetType() : (string)null;
                        var tipo1 = tipo == null ? false : tipo.IsGenericType;
                        if (tipo1)
                        {
                            foreach (KeyValuePair<string, object> n in model.Value[x])
                            {
                                var value = n.Value == null ? "" : n.Value.ToString();
                                UserActivityLogs.Items list = new UserActivityLogs.Items();
                                list.Name = n.Key;
                                list.Value = value;
                                newLoglist.ListLogValues.Add(list);
                                Values += " " + n.Key + ":" + value + ", \n";
                            }
                        }
                        else
                        {
                            UserActivityLogs.Items list = new UserActivityLogs.Items();
                            list.Name = model.Key;
                            list.Value = model.Value[x] == null ? "-1(null)" : model.Value[x].ToString();
                            newLoglist.ListLogValues.Add(list);
                        }
                        Values += "},\n";
                        newlog.loglist.Add(newLoglist);
                    }
                    Logs.asignation.Add(newlog);
                }
                else
                {
                    UserActivityLogs.LogsAsignation newLog = new UserActivityLogs.LogsAsignation();
                    newLog.logsName = model.Key;
                    newLog.logsValue = model.Value.ToString();
                    Logs.asignation.Add(newLog);
                    Values += " " + model.Key + ":";
                    Values += model.Value.ToString() + ",\n";
                }
            }
            //  }
            return Logs;
        }
        public static string structureValuesRow(UserActivityLogs modelInfo)
        {
            var InsertValuesRow = "";
            var listByOrder = new List<List<string>>();
            var ModelList = new List<KeyValuePair<string, KeyValuePair<string, string>>>();
            var ModelNoList = new List<KeyValuePair<string, string>>();
            //options selected
            foreach (var logs in modelInfo.asignation)
            {
                if (logs.loglist != null)
                {
                    listByOrder = logs.loglist
                       .Where(x => x.ListLogValues.Count() > 1)
                       .OrderBy(z => z.listLogsName)
                       .Select(x => x.ListLogValues.Where(n => n.Name != "Selected").Select(y => y.Value).ToList()).ToList();//crear una lista de los objetos en las listas agrupados por el nombre
                    foreach (var x in listByOrder)
                    {
                        ModelNoList.Add(new KeyValuePair<string, string>(x[1].ToString(), x[0].ToString()));
                    }
                }
                else
                    ModelNoList.Add(new KeyValuePair<string, string>(logs.logsName, logs.logsValue));
            }
            foreach (var logs in modelInfo.asignation)
            {
                if (logs.loglist == null)
                {
                    InsertValuesRow += logs.logsName + ":";
                    if (logs.logsValue != null && logs.logsValue.GetType() != typeof(object[]))
                    {
                        var KeyValue = ModelNoList.Where(x => x.Key == logs.logsValue.ToString());
                        if (KeyValue.Count() > 1)
                            InsertValuesRow += KeyValue.Select(x => x.Key).FirstOrDefault() == null ? logs.logsValue != null ? logs.logsValue + ", \n" : logs.logsName.IndexOf("ID") > -1 ? "ALL,\n" : ", \n" : logs.logsValue + ", \n";
                        else
                            InsertValuesRow += KeyValue.Select(x => x.Key).FirstOrDefault() == null ? logs.logsValue != null ? logs.logsValue + ", \n" : logs.logsName.IndexOf("ID") > -1 ? "ALL,\n" : ", \n" : KeyValue.Select(x => x.Value).FirstOrDefault() + ", \n";
                        //  var KeyValue = ModelNoList.FirstOrDefault(x => x.Key == logs.logsValue.ToString());
                        //  InsertValuesRow += KeyValue.Key == null ? logs.logsValue != null ? logs.logsValue + ", \n" : logs.logsName.IndexOf("ID") > -1 ? "ALL,\n" : ", \n" : KeyValue.Value +", \n";
                    }
                    else
                        InsertValuesRow += logs.logsName.IndexOf("ID") > -1 && logs.logsValue == null ? "ALL,\n" : logs.logsValue != null ? logs.logsValue : ", \n";   //"All\n";
                }
                else if (logs.loglist != null)
                {  //Listas
                    foreach (var list in logs.loglist.Where(x => x.ListLogValues.Count() < 2))
                    {
                        foreach (var items in list.ListLogValues)
                        {
                            var KeyName = ModelNoList.FirstOrDefault(x => x.Key == items.Value.ToString());
                            InsertValuesRow += KeyName.Key == null ? items.Name + ":" + items.Value + ", \n" : items.Name + ":" + KeyName.Value + ", \n";
                        }
                    }
                }
            }//only if want to show the lists;
            //   InsertValuesRow += "Lists: \n";
            //foreach (var logs in modelInfo.asignation)
            //{
            //    if (logs.loglist != null)
            //    {
            //             listByOrder = logs.loglist
            //                .Where(x => x.ListLogValues.Count() > 1)
            //                .OrderBy(z => z.listLogsName)
            //                .Select(x => x.ListLogValues.Where(n => n.Name != "Selected").Select(y => y.Value).ToList()).ToList();
            //      //      InsertValuesRow += logs.logsName+"\n";                                           
            //            foreach(var x in listByOrder)
            //            { //0=selected, 1=text, 2=value
            //     //   InsertValuesRow +=" "+x[0]+" - "+x[1]+" \n"; 
            //            }
            //    }
            //}   
            return InsertValuesRow;
        }

        public static void UserLogsActivitySendEx(Exception ex, string terminalID, string urlReferrer = "",string query = "", HttpRequestBase request = null)
        {
            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
            UserSession session = new UserSession();
            ePlatEntities db = new ePlatEntities();
            var mail = "rsalinas@villagroup.com";
            
            //var url = request.Url.ToString();
            var url = urlReferrer;
            var urlMethod = request.Url.ToString();

            var controller = request.RequestContext.RouteData.Values;

            email.From = new System.Net.Mail.MailAddress("rsalinas@villagroup.com");
            email.Subject = "Error from: " + url + " " + DateTime.Now.ToString();
            email.Body = ex.ToString() + "\nAction:" + controller["action"].ToString() + "\nURL: " + " " + urlMethod + "\nUser: " + session.User+" TerminalID " +terminalID + "\nEmail: " + session.Email + "\nQuery: " +query;
            email.To.Add(mail);

            //EmailNotifications.Send(email);
            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
        }
        //ActivityLogs
        public class ActivityLogs : ActionFilterAttribute
        {
            //Implement  [Models.DataModels.AdminDataModel.ActivityLogs] in controller for tracking
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var json = new System.Web.Script.Serialization.JavaScriptSerializer();
                var Attempt = filterContext.Controller.ViewData.Model;

                var JsonItem = json.Serialize(filterContext.ActionParameters.Values.Select(x => x));
                // var ActionModel = filterContext.ActionDescriptor.ActionName.Contains("Search") == true ? "search" : "";                            
                //try
                //{
                AdminDataModel.CreateTableUserLogActivity(JsonItem, " ", (string)null);
                //}
                //catch (Exception ex)
                //{
                //    AdminDataModel.UserLogsActivitySendEx(ex);
                //}              
                var JsonResult = (filterContext.Result);
                Debug.WriteLine(filterContext.Result);
                Debug.WriteLine(filterContext.RequestContext);
            }
            public override void OnResultExecuted(ResultExecutedContext filterContext)
            {
                // var JsonItem = filterContext.ActionParameters.Values;      
                var Attempt = filterContext.Result;

                base.OnResultExecuted(filterContext);
            }
        }
    }
}
