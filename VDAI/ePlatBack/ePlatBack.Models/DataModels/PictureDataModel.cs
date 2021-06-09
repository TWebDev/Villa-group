
using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Web.Script.Serialization;
using ePlatBack.Models.ViewModels;
using Newtonsoft.Json;

namespace ePlatBack.Models.DataModels
{
    public class PictureDataModel
    {
        //private string builder = "[";
        private static string firstPath = HttpContext.Current.Server.MapPath("~/");

        private static string secondPath = (firstPath.LastIndexOf("ePlatBack") >= 0 ? firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack")) : "");
        private static string finalPath = secondPath + "ePlatFront\\Content\\themes\\base\\images\\";

        public string GetGalleryName(string itemType, string itemID)
        {
            ePlatEntities db = new ePlatEntities();
            var galleryName = "";
            try
            {
                var _itemID = int.Parse(itemID);
                switch (itemType)
                {
                    case "Packages":
                        {
                            galleryName = db.tblPackages.Single(m => m.packageID == _itemID).package;
                            break;
                        }
                    case "Activities":
                        {
                            galleryName = db.tblServices.Single(m => m.serviceID == _itemID).service;
                            break;
                        }
                    case "Room Type":
                        {
                            galleryName = db.tblRoomTypes.Single(m => m.roomTypeID == _itemID).roomType;
                            break;
                        }
                    case "Places":
                        {
                            galleryName = db.tblPlaces.Single(m => m.placeID == _itemID).place;
                            break;
                        }
                    default:
                        {
                            galleryName = itemType.Substring(0, itemType.Length - 1);
                            break;
                        }
                }
            }
            catch { }
            return galleryName;
        }

        //public List<NewTreeItem> GetTree(string sysItemType, string itemID, string currentProvider, string destination)
        public string _GetTree(string sysItemType, string itemID, string currentProvider, string destination)
        {
            var list = new List<NewTreeItem>();
            try
            {
                var _itemID = int.Parse(itemID);
                //var tempDirectories = Directory.GetDirectories(finalPath, ".", SearchOption.TopDirectoryOnly);
                var tempDirectories = Directory.GetDirectories(finalPath, "*.*", SearchOption.TopDirectoryOnly);
                var providerLevel = currentProvider != "" ? currentProvider : "";
                var _directory = "";
                foreach (var i in tempDirectories)
                {
                    if (i.Substring(i.LastIndexOf("\\") + 1, i.Length - (i.LastIndexOf("\\") + 1)) != "banners" && i.Substring(i.LastIndexOf("\\") + 1, i.Length - (i.LastIndexOf("\\") + 1)) != "pages")
                    {
                        var initial = sysItemType.Substring(0, 1).ToLower();
                        var tempPath = i + "\\" + sysItemType.ToLower() + "\\" + initial + _itemID;
                        var path_a = tempPath;
                        var path_b = path_a.Substring(0, path_a.LastIndexOf("\\"));
                        if (path_b.Substring(path_b.LastIndexOf("\\") + 1, path_b.Length - (path_b.LastIndexOf("\\") + 1)) == "activities")
                        {
                            tempPath = i + "\\" + sysItemType.ToLower() + "\\" + "p" + providerLevel + "\\" + initial + _itemID;
                        }
                        if (!Directory.Exists(tempPath) && tempPath.IndexOf(destination) > 0)
                        {
                            _directory = tempPath;
                            Directory.CreateDirectory(tempPath);
                        }
                    }
                }

                var directories = Directory.GetDirectories(finalPath, "*.*", SearchOption.TopDirectoryOnly);

                foreach (var directory in directories)
                {
                    var dirName = directory.Substring(directory.LastIndexOf("\\") + 1, directory.Length - (directory.LastIndexOf("\\") + 1));
                    list.Add(new NewTreeItem()
                    {
                        id = dirName + "_" + dirName,
                        text = dirName,
                        icon = "/",
                        state = new TreeItemState() { opened = false, disabled = false, selected = _directory == directory },
                        children = _FindChildrenNodes(directory, dirName)
                    });
                }
            }
            catch { }
            var a = new JavaScriptSerializer().Serialize(list);
            //return list;
            return a;
        }

        public List<NewTreeItem> _FindChildrenNodes(string directory, string destination)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<NewTreeItem>();
            //var children = Directory.GetDirectories(directory, ".", SearchOption.TopDirectoryOnly);
            var children = Directory.GetDirectories(directory, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var child in children)
            {
                try
                {
                    var currentFolder = child.ToString().Substring(child.ToString().LastIndexOf("\\") + 1, child.ToString().Length - (child.ToString().LastIndexOf("\\") + 1));
                    var _dir = currentFolder;
                    if (currentFolder.Any(char.IsDigit))
                    {
                        var item = currentFolder.Substring(0, 1);
                        var itemID = long.Parse(currentFolder.Substring(1, currentFolder.Length - 1));
                        #region "switch"
                        switch (item)
                        {
                            case "a":
                                {
                                    var dir_ = db.tblServices.Where(m => m.serviceID == itemID);
                                    _dir = dir_.Count() > 0 ? dir_.Single().service : _dir;
                                    break;
                                }
                            case "p":
                                {
                                    if (child.IndexOf("packages") > 0)
                                    {
                                        var dir_ = db.tblPackages.Where(m => m.packageID == itemID);
                                        _dir = dir_.Count() > 0 ? dir_.Single().package : _dir;
                                        break;
                                    }
                                    if (child.IndexOf("places") > 0)
                                    {
                                        var dir_ = db.tblPlaces.Where(m => m.placeID == itemID);
                                        _dir = dir_.Count() > 0 ? dir_.Single().place : _dir;
                                        break;
                                    }
                                    if (child.IndexOf("activities") > 0)
                                    {
                                        var dir_ = db.tblProviders.Where(m => m.providerID == itemID);
                                        _dir = dir_.Count() > 0 ? dir_.Single().comercialName : _dir;
                                        break;
                                    }
                                    break;
                                }
                            case "r":
                                {
                                    var dir_ = db.tblRoomTypes.Where(m => m.roomTypeID == itemID);
                                    _dir = dir_.Count() > 0 ? dir_.Single().roomType : _dir;
                                    break;
                                }
                        }
                        #endregion
                    }
                    list.Add(new NewTreeItem()
                    {
                        id = destination + "_" + currentFolder,
                        text = _dir,
                        icon = "/",
                        state = new TreeItemState(){opened = false, disabled = false, selected = currentFolder == child},
                        children = _FindChildrenNodes(child.ToString(), destination)
                    });
                }
                catch
                {
                }
            }
            return list;
        }

        public List<TreeItem> GetTree(string sysItemType, string itemID, string currentProvider, string destination)
        {
            var list = new List<TreeItem>();
            try
            {
                var _itemID = int.Parse(itemID);
                //var tempDirectories = Directory.GetDirectories(finalPath, ".", SearchOption.TopDirectoryOnly);
                var tempDirectories = Directory.GetDirectories(finalPath, "*.*", SearchOption.TopDirectoryOnly);
                var providerLevel = currentProvider != "" ? currentProvider : "";
                foreach (var i in tempDirectories)
                {
                    if (i.Substring(i.LastIndexOf("\\") + 1, i.Length - (i.LastIndexOf("\\") + 1)) != "banners" && i.Substring(i.LastIndexOf("\\") + 1, i.Length - (i.LastIndexOf("\\") + 1)) != "pages")
                    {
                        var initial = sysItemType.Substring(0, 1).ToLower();
                        var tempPath = i + "\\" + sysItemType.ToLower() + "\\" + initial + _itemID;
                        var path_a = tempPath;
                        var path_b = path_a.Substring(0, path_a.LastIndexOf("\\"));
                        if (path_b.Substring(path_b.LastIndexOf("\\") + 1, path_b.Length - (path_b.LastIndexOf("\\") + 1)) == "activities")
                        {
                            tempPath = i + "\\" + sysItemType.ToLower() + "\\" + "p" + providerLevel + "\\" + initial + _itemID;
                        }
                        if (!Directory.Exists(tempPath) && tempPath.IndexOf(destination) > 0)
                        {
                            Directory.CreateDirectory(tempPath);
                        }
                    }
                }

                var directories = Directory.GetDirectories(finalPath, "*.*", SearchOption.TopDirectoryOnly);

                foreach (var directory in directories)
                {
                    var dirName = directory.Substring(directory.LastIndexOf("\\") + 1, directory.Length - (directory.LastIndexOf("\\") + 1));
                    list.Add(new TreeItem()
                    {
                        attr = new TreeItemAttributes() { id = dirName + "_" + dirName },
                        data = dirName,
                        children = FindChildrenNodes(directory, dirName)
                    });
                }
            }
            catch { }
            return list;
        }

        public List<TreeItem> FindChildrenNodes(string directory, string destination)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<TreeItem>();
            //var children = Directory.GetDirectories(directory, ".", SearchOption.TopDirectoryOnly);
            var children = Directory.GetDirectories(directory, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var child in children)
            {
                var currentFolder = child.ToString().Substring(child.ToString().LastIndexOf("\\") + 1, child.ToString().Length - (child.ToString().LastIndexOf("\\") + 1));
                try
                {
                    var _dir = currentFolder;
                    if (currentFolder.Any(char.IsDigit))
                    {
                        var item = currentFolder.Substring(0, 1);
                        var itemID = long.Parse(currentFolder.Substring(1, currentFolder.Length - 1));
                        #region "switch"
                        switch (item)
                        {
                            case "a":
                                {
                                    var dir_ = db.tblServices.Where(m => m.serviceID == itemID);
                                    //_dir = dir_.Count() > 0 ? dir_.Single().service : _dir;
                                    _dir = dir_.Count() > 0 ? dir_.Single().service : null;
                                    break;
                                }
                            case "p":
                                {
                                    if (child.IndexOf("packages") > 0)
                                    {
                                        var dir_ = db.tblPackages.Where(m => m.packageID == itemID);
                                        //_dir = dir_.Count() > 0 ? dir_.Single().package : _dir;
                                        _dir = dir_.Count() > 0 ? dir_.Single().package : null;
                                        break;
                                    }
                                    if (child.IndexOf("places") > 0)
                                    {
                                        var dir_ = db.tblPlaces.Where(m => m.placeID == itemID);
                                        //_dir = dir_.Count() > 0 ? dir_.Single().place : _dir;
                                        _dir = dir_.Count() > 0 ? dir_.Single().place : null;
                                        break;
                                    }
                                    if (child.IndexOf("activities") > 0)
                                    {
                                        var dir_ = db.tblProviders.Where(m => m.providerID == itemID);
                                        //_dir = dir_.Count() > 0 ? dir_.Single().comercialName : _dir;
                                        _dir = dir_.Count() > 0 ? dir_.Single().comercialName : null;
                                        break;
                                    }
                                    break;
                                }
                            case "r":
                                {
                                    var dir_ = db.tblRoomTypes.Where(m => m.roomTypeID == itemID);
                                    //_dir = dir_.Count() > 0 ? dir_.Single().roomType : _dir;
                                    _dir = dir_.Count() > 0 ? dir_.Single().roomType : null;
                                    break;
                                }
                        }
                        #endregion
                    }
                    if (_dir != null)
                    {
                        list.Add(new TreeItem()
                        {
                            attr = new TreeItemAttributes() { id = destination + "_" + currentFolder },
                            data = _dir,
                            children = FindChildrenNodes(child.ToString(), destination)
                        });
                    }
                }
                catch
                {
                    list.Add(new TreeItem()
                    {
                        attr = new TreeItemAttributes() { id = destination + "_" + currentFolder },
                        data = currentFolder,
                        children = FindChildrenNodes(child.ToString(), destination)
                    });
                }
            }
            return list;
        }

        public object GetFiles(string directory)
        {
            ePlatEntities db = new ePlatEntities();
            var firstPath = HttpContext.Current.Server.MapPath("~/");
            var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
            var finalPath = secondPath + "ePlatFront\\Content\\themes\\base\\images";
            //var finalPath = "eplatfront.villagroup.com\\Content\\themes\\base\\images";
            string[] files = Directory.GetFiles(@finalPath + directory);
            List<PictureModel> list = new List<PictureModel>();
            foreach (var file in files)
            {
                var imagePath = file.Substring(file.IndexOf("\\Content\\"), (file.Length - file.LastIndexOf("\\Content\\"))).Replace("\\", "/").ToLower();
                //var query = db.tblPictures.Single(m => (m.path + m.file_).Equals(imagePath));
                var query = db.tblPictures.Where(m => (m.path + m.file_).Equals(imagePath));
                foreach (var i in db.tblPictures.Where(m => (m.path + m.file_).Equals(imagePath)))
                {
                    list.Add(new PictureModel()
                    {
                        PictureID = int.Parse(i.pictureID.ToString()),
                        File = i.file_,
                        Format = i.format,
                        Path = i.path,
                        Descriptions = i.tblPictureDescriptions.Count()
                    });
                }
            }
            return list;
        }

        public List<KeyValuePair<int, string>> GetItemNames(string parameters, string itemType)
        {
            ePlatEntities db = new ePlatEntities();
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
            List<KeyValuePair<string, long>> listTerminals = new List<KeyValuePair<string, long>>();

            switch (itemType)
            {
                case "Activities":
                    {
                        //--this block will change depending on itemType
                        foreach (var i in parameters.Split(','))
                            listTerminals.Add(new KeyValuePair<string, long>("", Int64.Parse(i)));
                        var arrayTerminals = listTerminals.Select(m => m.Value);
                        //--
                        var query = from a in db.tblServices
                                    where arrayTerminals.Contains(a.originalTerminalID)
                                    && a.deleted == false
                                    select new
                                    {
                                        key = a.serviceID,
                                        value = a.service
                                    };
                        foreach (var item in query)
                        {
                            list.Add(new KeyValuePair<int, string>(Int32.Parse(item.key.ToString()), item.value));
                        }
                        break;
                    }
                case "Packages":
                    {
                        //--this block will change depending on itemType
                        foreach (var i in parameters.Split(','))
                            listTerminals.Add(new KeyValuePair<string, long>("", Int64.Parse(i)));
                        var arrayTerminals = listTerminals.Select(m => m.Value);
                        //--
                        var query = from i in db.tblPackages
                                    where arrayTerminals.Contains(i.terminalID)
                                    select new
                                    {
                                        key = i.packageID,
                                        value = i.package
                                    };
                        foreach (var item in query)
                            list.Add(new KeyValuePair<int, string>(Int32.Parse(item.key.ToString()), item.value));
                        break;
                    }
                case "Room Type":
                    {
                        var placeID = Int64.Parse(parameters);
                        var query = from i in db.tblRoomTypes
                                    where i.placeID == placeID
                                    select new
                                    {
                                        key = i.roomTypeID,
                                        value = i.roomType
                                    };
                        foreach (var item in query)
                            list.Add(new KeyValuePair<int, string>(Int32.Parse(item.key.ToString()), item.value));
                        break;
                    }
                case "Places":
                    {
                        var query = PlaceDataModel.GetResortsByTerminals(parameters, true);
                        foreach (var item in query)
                        {
                            list.Add(new KeyValuePair<int, string>(Int32.Parse(item.Value.ToString()), item.Text));
                        }
                        break;
                    }

            }
            return list;
        }

        public List<PictureDescriptionInfoModel> GetPictureDescriptions(int pictureID)
        {
            ePlatEntities db = new ePlatEntities();
            List<PictureDescriptionInfoModel> list = new List<PictureDescriptionInfoModel>();
            try
            {
                var query = from description in db.tblPictureDescriptions
                            where description.pictureID == pictureID
                            select description;
                foreach (var i in query)
                {
                    list.Add(new PictureDescriptionInfoModel()
                    {
                        PictureDescriptionInfo_PictureDescriptionID = int.Parse(i.pictureDescriptionID.ToString()),
                        //PictureDescriptionInfo_PictureID = int.Parse(i.pictureID.ToString()),
                        //PictureDescriptionInfo_Alt = i.alt,
                        PictureDescriptionInfo_Culture = i.culture
                        //PictureDescriptionInfo_Description = i.description
                    });
                }
            }
            catch
            {
            }
            return list;
        }

        public PictureDescriptionInfoModel GetPictureDescription(int pictureDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            PictureDescriptionInfoModel model = new PictureDescriptionInfoModel();
            try
            {
                var query = db.tblPictureDescriptions.Single(m => m.pictureDescriptionID == pictureDescriptionID);
                model.PictureDescriptionInfo_PictureDescriptionID = int.Parse(query.pictureDescriptionID.ToString());
                model.PictureDescriptionInfo_PictureID = int.Parse(query.pictureID.ToString());
                model.PictureDescriptionInfo_Alt = query.alt;
                model.PictureDescriptionInfo_Culture = query.culture;
                model.PictureDescriptionInfo_Description = query.description;
            }
            catch
            {
            }
            return model;
        }

        public AttemptResponse SavePictureDescription(PictureDescriptionInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.PictureDescriptionInfo_PictureDescriptionID != 0)
            {
                try
                {
                    tblPictureDescriptions description = db.tblPictureDescriptions.Single(m => m.pictureDescriptionID == model.PictureDescriptionInfo_PictureDescriptionID);
                    description.pictureID = model.PictureDescriptionInfo_PictureID;
                    description.alt = model.PictureDescriptionInfo_Alt;
                    description.culture = model.PictureDescriptionInfo_Culture;
                    description.description = model.PictureDescriptionInfo_Description;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = description.pictureDescriptionID;
                    response.Message = "Picture Description Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Message = "Picture Description NOT Updated";
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblPictureDescriptions description = new tblPictureDescriptions();
                    description.pictureID = model.PictureDescriptionInfo_PictureID;
                    description.alt = model.PictureDescriptionInfo_Alt;
                    description.culture = model.PictureDescriptionInfo_Culture;
                    description.description = model.PictureDescriptionInfo_Description;
                    db.tblPictureDescriptions.AddObject(description);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = description.pictureDescriptionID;
                    response.Message = "Picture Description Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Message = "Picture Description NOT Saved";
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse DeletePictureDescription(int pictureDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblPictureDescriptions.Single(m => m.pictureDescriptionID == pictureDescriptionID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Picture Description Deleted";
                response.ObjectID = pictureDescriptionID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Picture Description NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public object GetImagesPerItemType(string sysItemType, string itemID)
        {
            ePlatEntities db = new ePlatEntities();
            List<PictureModel> list = new List<PictureModel>();
            try
            {
                var firstPath = HttpContext.Current.Server.MapPath("~/");
                var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                var finalPath = secondPath + "ePlatFront\\Content\\themes\\base\\images";
                //string[] files = Directory.GetFiles(@finalPath + sysItemTypeID);
                var _itemID = int.Parse(itemID);
                var query = db.tblPictures_SysItemTypes.Where(m => m.itemID == _itemID).Where(m => m.tblSysItemTypes.sysItemType == sysItemType);

                foreach (var i in query)
                {
                    list.Add(new PictureModel()
                    {
                        PictureID = int.Parse(i.pictureID.ToString()),
                        File = i.tblPictures.file_,
                        Format = i.tblPictures.format,
                        Path = i.tblPictures.path,
                        Main = i.main,
                        Order = i.order_,
                    });
                }
            }
            catch { }
            return list;
        }

        public AttemptResponse RemovePicture(int pictureID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var existQuery = from a in db.tblPictures_SysItemTypes
                                 where a.pictureID == pictureID
                                 select new
                                 {
                                     sysItemType = a.tblSysItemTypes.sysItemType,
                                     item = a.itemID
                                 };
                if (existQuery.Count() > 0)
                {
                    var listItems = "<ul>";
                    foreach (var i in existQuery)
                    {
                        switch (i.sysItemType)
                        {
                            case "Packages":
                                {
                                    listItems += "<li>Packages: " + db.tblPackages.Single(m => m.packageID == i.item).package + "</li>";
                                    break;
                                }
                            case "Activities":
                                {
                                    listItems += "<li>Activities: " + db.tblServices.Single(m => m.serviceID == i.item).service + "</li>";
                                    break;
                                }
                            case "Room Type":
                                {
                                    listItems += "<li>Room Types: " + db.tblRoomTypes.Single(m => m.roomTypeID == i.item).roomType + "</li>";
                                    break;
                                }
                        }
                    }
                    listItems += "</ul>";
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.Message = "This picture cannot be deleted since it's being used on the next galleries: <br />" + listItems;
                    response.ObjectID = pictureID;
                    return response;
                }
                else
                {
                    var firstPath = HttpContext.Current.Server.MapPath("~/");
                    var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                    var finalPath = secondPath + "ePlatFront";// \\Content\\themes\\base\\images";
                    var query = (from picture in db.tblPictures
                                 where picture.pictureID == pictureID
                                 select picture).Single();
                    var path = finalPath + query.path + query.file_;
                    var file = new FileInfo(path.Replace("/", "\\"));
                    file.Delete();
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Picture Deleted";
                    response.ObjectID = pictureID;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Picture NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SavePicturesOnGallery(string sysItemType, int itemID, string[] picturesArray)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var arrayIndexes = picturesArray.Select((i, v) => new { value = i, index = v });
            try
            {
                var query = from picture in db.tblPictures_SysItemTypes
                            where picture.tblSysItemTypes.sysItemType == sysItemType
                            && picture.itemID == itemID
                            select picture;
                foreach (var i in query)
                {
                    db.tblPictures_SysItemTypes.DeleteObject(i);
                }
                db.SaveChanges();
                response.Message = "Pictures Removed From Gallery";
                if (arrayIndexes.First(m => m.index == 0).value != "liNoPictures")
                {
                    foreach (var picture in arrayIndexes)
                    {
                        tblPictures_SysItemTypes picture_itemType = new tblPictures_SysItemTypes();
                        picture_itemType.pictureID = Int64.Parse(picture.value.Substring(9));
                        picture_itemType.itemID = itemID;
                        picture_itemType.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == sysItemType).sysItemTypeID;
                        picture_itemType.order_ = picture.index + 1;
                        picture_itemType.main = picture.index == 0 ? true : false;
                        db.tblPictures_SysItemTypes.AddObject(picture_itemType);
                    }
                    db.SaveChanges();
                    response.Message = "Pictures Saved on Gallery";
                }
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.ObjectID = 0;
                response.Message = "Pictures NOT Saved on Gallery";
                response.Exception = ex;
                return response;
            }
        }

        public FineUploaderResult UploadPicture(FineUpload upload, string path, string sysItemType)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var firstPath = HttpContext.Current.Server.MapPath("~/");
            var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
            var finalPath = secondPath + "ePlatFront\\Content\\themes\\base\\images\\";
            var fileName = upload.Filename;

            //----------------------------------------------------------------------
            //var matches = Regex.Matches(fileName, "%").Count;
            //for (var i = 0; i < Regex.Matches(fileName, "%").Count; i++)
            //for (var i = 0; i < matches; i++)
            //{
            //    var encoded = fileName.Substring(fileName.IndexOf("%"), ((fileName.IndexOf("%") + 3) - fileName.IndexOf("%")));
            //    var urldecoded = HttpContext.Current.Server.UrlDecode(encoded);


            //    var newFileName = fileName.Replace(encoded, "-");
            //    fileName = newFileName;
            //}
            var fileNameDecoded = HttpContext.Current.Server.UrlDecode(fileName);
            fileName = HttpUtility.UrlEncode(fileNameDecoded, Encoding.GetEncoding("iso-8859-8"));
            fileName = fileName.Replace("+", "");
            for (var i = 0; i < Regex.Matches(fileName, "%").Count; i++)
            {
                var encoded = fileName.Substring(fileName.IndexOf("%"), ((fileName.IndexOf("%") + 3) - fileName.IndexOf("%")));
                var newFileName = fileName.Replace(encoded, "_");
                fileName = newFileName;
            }
            //----------------------------------------------------------------------

            var filePath = finalPath + path + fileName;
            var picturePath = "/content/themes/base/images" + path.Replace("\\", "/");
            try
            {
                //prevent duplicate filename
                var existingPicture = db.tblPictures.Where(m => m.file_ == fileName && m.path == picturePath).Count();
                if (existingPicture > 0)
                {
                    throw new Exception("Another picture has same name, change it before upload");
                }
                upload.SaveAs(filePath);
                tblPictures picture = new tblPictures();
                picture.file_ = fileName;
                picture.format = upload.Filename.Substring((upload.Filename.IndexOf(".") + 1), (upload.Filename.Length - (upload.Filename.IndexOf(".") + 1)));
                picture.path = picturePath;
                //picture.path = "/content/themes/base/images/" + sysItemType;
                db.tblPictures.AddObject(picture);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = picture.pictureID;
                response.Message = "Picture Saved";
                return new FineUploaderResult(true, new { response = response }, new { path = picturePath + fileName });
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.ObjectID = 0;
                response.Exception = upload.Exception ?? ex;
                response.Message = "Picture NOT Saved";
                return new FineUploaderResult(false, new { response = response });
            }
        }

        [ModelBinder(typeof(ModelBinder))]
        public class FineUpload
        {
            public string Filename { get; set; }
            public Stream InputStream { get; set; }

            public Exception Exception { get; set; }

            public void SaveAs(string destination, bool overwrite = true, bool autoCreateDirectory = true)
            {
                if (autoCreateDirectory)
                {
                    var directory = new FileInfo(destination).Directory;
                    if (directory != null) directory.Create();
                }
                using (var file = new FileStream(destination, overwrite ? FileMode.Create : FileMode.CreateNew))
                    InputStream.CopyTo(file);
            }

            public class ModelBinder : IModelBinder
            {
                public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
                {
                    var upload = new FineUpload();
                    try
                    {
                        var request = controllerContext.RequestContext.HttpContext.Request;
                        var formUpload = request.Files.Count > 0;

                        //find filename
                        var xFileName = request.Headers["X-File-Name"];
                        var qqFile = request["qqfile"];
                        var formFilename = formUpload ? request.Files[0].FileName : null;

                        upload.Filename = xFileName ?? qqFile ?? formFilename;
                        upload.InputStream = formUpload ? request.Files[0].InputStream : request.InputStream;
                        return upload;
                    }
                    catch (Exception ex)
                    {
                        upload.Exception = ex;
                        return upload;
                    }
                    //var upload = new FineUpload
                    //{
                    //    Filename = xFileName ?? qqFile ?? formFilename,
                    //    InputStream = formUpload ? request.Files[0].InputStream : request.InputStream
                    //};
                    //return upload;
                }
            }

        }

        public class FineUploaderResult : ActionResult
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            public const string ResponseContentType = "text/plain";

            private readonly bool _success;
            private readonly string _error;
            private readonly bool? _preventRetry;
            private JObject _otherData;
            private readonly JObject _otherData2;

            public FineUploaderResult(bool success, object otherData = null, object otherData2 = null, string error = null, bool? preventRetry = null)
            {
                _success = success;
                _error = error;
                _preventRetry = preventRetry;

                if (otherData != null)
                    _otherData = JObject.FromObject(otherData);

                if (otherData2 != null)
                    _otherData2 = JObject.FromObject(otherData2);
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.ContentType = ResponseContentType;

                response.Write(BuildResponse());
            }

            public Object BuildResponse()
            {
                var response = _otherData ?? new JObject();
                response["path"] = _otherData2 ?? new JObject();
                response["success"] = _success;

                if (!string.IsNullOrWhiteSpace(_error))
                    response["error"] = _error;

                if (_preventRetry.HasValue)
                    response["preventRetry"] = _preventRetry.Value;

                return response;//.ToString();
            }
        }

        public static string GetMainPicture(int itemTypeId, long id)
        {
            ePlatEntities db = new ePlatEntities();
            try
            {
                var PicturePath = (from p in db.tblPictures_SysItemTypes
                                   where p.itemID == id
                                   && p.sysItemTypeID == itemTypeId
                                   orderby p.main descending, p.order_ ascending
                                   select new
                                   {
                                       p.tblPictures.path,
                                       p.tblPictures.file_
                                   }).FirstOrDefault();

                if (PicturePath != null)
                {
                    return PicturePath.path + PicturePath.file_;
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

        public static List<PictureListItem> GetPictures(int itemTypeId, long id)
        {
            ePlatEntities db = new ePlatEntities();
            string culture = Utils.GeneralFunctions.GetCulture();
            List<PictureListItem> pictures = new List<PictureListItem>();
            var PicturesQ = from p in db.tblPictures_SysItemTypes
                            join picture in db.tblPictures on p.pictureID equals picture.pictureID
                            join description in db.tblPictureDescriptions on p.pictureID equals description.pictureID
                            into pictureDescription from description in pictureDescription.DefaultIfEmpty()
                            where p.itemID == id
                            && p.sysItemTypeID == itemTypeId
                            && (description.culture == culture  || description == null)
                            orderby p.main descending, p.order_ ascending
                            select new
                            {
                                p.pictureID,
                                picture.path,
                                picture.file_,
                                description.description
                            };

            foreach (var picture in PicturesQ)
            {
                PictureListItem newPicture = new PictureListItem();
                newPicture.PictureID = picture.pictureID;
                newPicture.Picture = picture.path + picture.file_;
                newPicture.Description = picture.description;
                pictures.Add(newPicture);
            }

            return pictures;
        }

        public static List<PictureListItem> GetMainPictureItem(int itemTypeId, long id)
        {
            ePlatEntities db = new ePlatEntities();
            string culture = Utils.GeneralFunctions.GetCulture();
            List<PictureListItem> pictures = new List<PictureListItem>();
            var PicturesQ = (from p in db.tblPictures_SysItemTypes
                            join picture in db.tblPictures on p.pictureID equals picture.pictureID
                            join description in db.tblPictureDescriptions on p.pictureID equals description.pictureID
                            into pictureDescription
                            from description in pictureDescription.DefaultIfEmpty()
                            where p.itemID == id
                            && p.sysItemTypeID == itemTypeId
                            && (description.culture == culture || description == null)
                            orderby p.main descending, p.order_ ascending
                            select new
                            {
                                p.pictureID,
                                picture.path,
                                picture.file_,
                                description.description
                            }).FirstOrDefault();

            if (PicturesQ != null)
            {
                PictureListItem newPicture = new PictureListItem();
                newPicture.PictureID = PicturesQ.pictureID;
                newPicture.Picture = PicturesQ.path + PicturesQ.file_;
                newPicture.Description = PicturesQ.description;
                pictures.Add(newPicture);
            }

            return pictures;
        }

        public static string GetPicasaAvatar(string email)
        {
            string picture = string.Empty;

            string url = "http://picasaweb.google.com/data/entry/api/user/" + email.Trim() + "?alt=json";
            string json = "";
            try
            {
                using (var client = new WebClient())
                {
                    json = client.DownloadString(url);
                }

                PicasaAvatar avatarJson = JsonConvert.DeserializeObject<PicasaAvatar>(json);
                picture = avatarJson.Entry.Picture.Value;
            } catch(Exception ex){

            }

            return picture;
        }
    }
}
