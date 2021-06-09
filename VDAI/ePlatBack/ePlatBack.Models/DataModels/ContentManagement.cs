using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Globalization;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Web.Script.Serialization;
using System.IO;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ePlatBack.Models.DataModels
{
    public class ContentManagement
    {
        public static UserSession session = new UserSession();
        public static SessionDetails sessiond = new SessionDetails();


        ePlatEntities db = new ePlatEntities();

        /******************************************************************************************************/
        public object getPlaces(int type)
        {
            var hotels = (from place in db.tblPlaces
                          join destination in db.tblDestinations on place.destinationID equals destination.destinationID
                          join zone in db.tblZones on place.zoneID equals zone.zoneID
                          join terminal in db.tblPlaces_Terminals on place.placeID equals terminal.placeID
                          orderby place.place
                          where place.placeTypeID == type
                          where place.destinationID == 1 || place.destinationID == 4
                          where terminal.terminalID == 62
                          select new
                          {
                              id = place.placeID,
                              name = place.place,
                              namelabel = place.placeLabel,
                              address = place.address,
                              phone = place.phone,
                              lat = place.lat,
                              lng = place.lng,
                              destination = destination.destination,
                              zone = zone.zone,
                              zoneid = zone.zoneID,
                              checkin = place.checkInTime,
                              checkout = place.checkOutTime,
                              active = place.active,

                          }

                          ).ToList();

            var orientations = (from places in db.tblPlacesOrientations
                                join ori in db.tblOrientations on places.orientationID equals ori.orientationID
                                select new
                                {
                                    placeid = places.placeID,
                                    orientation = ori.orientation
                                }

                                ).ToList();

            var x = hotels.ToArray();
            object[] data = new[]
               {
                new object[] {x},
                new object[] {orientations},

                 };

            return data;
        }
        /******************************************************************************************************/

        public Array getDataPlaces()
        {
            var destinations = (from dest in db.tblDestinations
                                join terminal in db.tblTerminals_Destinations on dest.destinationID equals terminal.destinationID
                                where terminal.terminalID == 62
                                select new
                                {
                                    label = dest.destination,
                                    value = dest.destinationID
                                }
                          ).ToList();

            var zones = (from zone in db.tblZones
                         where zone.destinationID == 1 || zone.destinationID == 4
                         select new
                         {
                             label = zone.zone,
                             value = zone.zoneID
                         }
                          ).ToList();
            var orientations = (from ori in db.tblOrientations
                                select new
                                {
                                    value = ori.orientationID,
                                    text = ori.orientation
                                }
                          ).ToList();
            var activities = (from act in db.tblFeatures
                              where act.featureTypeID == 1
                              orderby act.feature ascending

                              select new
                              {
                                  value = act.featureID,
                                  text = act.feature
                              }
                          ).ToList();
            var services = (from act in db.tblFeatures
                            where act.featureTypeID == 5
                            orderby act.feature ascending

                            select new
                            {
                                value = act.featureID,
                                text = act.feature
                            }
                          ).ToList();
            var benefits = (from ben in db.tblFeatures
                            where ben.featureTypeID == 3
                            orderby ben.feature ascending

                            select new
                            {
                                value = ben.featureID,
                                text = ben.feature
                            }
                          ).ToList();
            var clasifications = (from cla in db.tblPlaceClasifications

                                  select new
                                  {
                                      value = cla.placeClasificationID,
                                      label = cla.placeClasification
                                  }
                          ).ToList();

            var strucutres = (from cla in db.tblPageStructures

                              select new
                              {
                                  value = cla.pageStructureID,
                                  label = cla.structureName
                              }
                          ).ToList();
            var autores = (from user in db.tblUserProfiles
                           join job in db.tblUsers_JobPositions on user.userID equals job.userID
                           join pos in db.tblJobPositions on job.jobPositionID equals pos.jobPositionID
                           where pos.jobPositionID == 1077

                           select new
                           {
                               value = user.userID,
                               label = user.firstName + " " + user.lastName
                           }
                         ).ToList();
            var pagetypes = (from page in db.tblPageTypes


                             select new
                             {
                                 value = page.pageTypeID,
                                 label = page.pageType
                             }
                        ).ToList();


            object[] data = new[]
                {
                new object[] {destinations},
                new object[] {zones},
                new object[] { orientations },
                new object[] { activities },
                new object[] { services },
                new object[] { benefits },
                new object[] { clasifications },
                new object[] { strucutres },
                new object[] { autores },
                 new object[] { pagetypes }

                 };


            return data;
        }
        /******************************************************************************************************/

        public object AddPlace(string data)
        {

            var place = System.Web.Helpers.Json.Decode(data);
            tblPlaces places = new tblPlaces();
            if (place.placetypeid == 1)
            {

                places.place = Convert.ToString(place.hotelname);
                places.placeLabel = Convert.ToString(place.slogan);
                places.address = Convert.ToString(place.address);
                places.phone = Convert.ToString(place.phone);
                places.lat = Convert.ToString(place.lat);
                places.lng = Convert.ToString(place.lng);
                places.destinationID = Convert.ToInt64(place.destinationid);
                places.zoneID = Convert.ToInt32(place.zoneid);
                places.placeTypeID = Convert.ToInt32(place.placetypeid);
                places.checkInTime = Convert.ToString(place.checkin);
                places.checkOutTime = Convert.ToString(place.checkout);
                places.placeClasificationID = Convert.ToInt32(place.clasification);
                places.active = Convert.ToBoolean(place.active);
                places.twitterAccount = Convert.ToString(place.twitter);


                db.tblPlaces.AddObject(places);
                db.SaveChanges();
            }
            if (place.placetypeid == 2)
            {

                places.place = Convert.ToString(place.restaurantname);
                places.address = Convert.ToString(place.address);
                places.phone = Convert.ToString(place.phone);
                places.lat = Convert.ToString(place.lat);
                places.lng = Convert.ToString(place.lng);
                places.destinationID = Convert.ToInt64(place.destinationid);
                places.zoneID = Convert.ToInt32(place.zoneid);
                places.placeTypeID = Convert.ToInt32(place.placetypeid);
                places.checkInTime = Convert.ToString(place.openat);
                places.checkOutTime = Convert.ToString(place.closedat);
                places.active = Convert.ToBoolean(place.active);
                places.twitterAccount = Convert.ToString(place.twitter);

                db.tblPlaces.AddObject(places);
                db.SaveChanges();

            }
            if (place.placetypeid == 16)
            {

                places.place = Convert.ToString(place.spaname);
                places.address = Convert.ToString(place.address);
                places.phone = Convert.ToString(place.phone);
                places.lat = Convert.ToString(place.lat);
                places.lng = Convert.ToString(place.lng);
                places.destinationID = Convert.ToInt64(place.destinationid);
                places.zoneID = Convert.ToInt32(place.zoneid);
                places.placeTypeID = Convert.ToInt32(place.placetypeid);
                places.checkInTime = Convert.ToString(place.openat);
                places.checkOutTime = Convert.ToString(place.closedat);
                places.twitterAccount = Convert.ToString(place.twitter);
                places.active = Convert.ToBoolean(place.active);

                db.tblPlaces.AddObject(places);
                db.SaveChanges();

            }
            if (place.placetypeid == 21)
            {

                places.place = Convert.ToString(place.artgalleryname);
                places.address = Convert.ToString(place.address);
                places.phone = Convert.ToString(place.phone);
                places.lat = Convert.ToString(place.lat);
                places.lng = Convert.ToString(place.lng);
                places.destinationID = Convert.ToInt64(place.destinationid);
                places.zoneID = Convert.ToInt32(place.zoneid);
                places.placeTypeID = Convert.ToInt32(place.placetypeid);
                places.checkInTime = Convert.ToString(place.openat);
                places.checkOutTime = Convert.ToString(place.closedat);
                places.twitterAccount = Convert.ToString(place.twitter);
                places.active = Convert.ToBoolean(place.active);

                db.tblPlaces.AddObject(places);
                db.SaveChanges();

            }



            tblPlaces_Terminals terminal = (new tblPlaces_Terminals
            {
                terminalID = 62,
                placeID = places.placeID,
            });
            db.tblPlaces_Terminals.AddObject(terminal);
            db.SaveChanges();
            foreach (var item in place.orientations)
            {
                tblPlacesOrientations orientations = (new tblPlacesOrientations
                {
                    placeID = places.placeID,
                    orientationID = Convert.ToInt32(item),
                });
                db.tblPlacesOrientations.AddObject(orientations);
                db.SaveChanges();
            }

            foreach (var item in place.activities)
            {
                tblPlacesFeatures activities = (new tblPlacesFeatures
                {
                    placeID = places.placeID,
                    featureID = item,

                });
                db.tblPlacesFeatures.AddObject(activities);
                db.SaveChanges();
            }
            foreach (var item in place.services)
            {
                tblPlacesFeatures services = (new tblPlacesFeatures
                {
                    placeID = places.placeID,
                    featureID = item,

                });
                db.tblPlacesFeatures.AddObject(services);
                db.SaveChanges();
            }
            foreach (var item in place.benefits)
            {
                tblPlacesFeatures benefits = (new tblPlacesFeatures
                {
                    placeID = places.placeID,
                    featureID = item,

                });
                db.tblPlacesFeatures.AddObject(benefits);
                db.SaveChanges();
            }

            tblPlaceDescriptions description = (new tblPlaceDescriptions
            {
                placeID = places.placeID,
                culture = "en-US",
                fullDescription = place.description,
                shortDescription = place.shortdescription,
            });
            db.tblPlaceDescriptions.AddObject(description);
            db.SaveChanges();

            tblSeoItems seo = new tblSeoItems();
            seo.title = Convert.ToString(place.title);
            seo.friendlyUrl = Convert.ToString(place.friendlyurl);
            seo.url = place.staticurl + 62 + "/" + places.placeID;
            seo.culture = "en-US";
            seo.index_ = false;
            seo.follow = false;
            seo.terminalID = 62;
            seo.sysItemTypeID = place.sysitem;
            seo.itemID = places.placeID;
            db.tblSeoItems.AddObject(seo);
            db.SaveChanges();
            return places.placeID;
        }
        /******************************************************************************************************/

        public Array getDataPlaceSelected(Int64 id, int sys)
        {


            var activities = (from act in db.tblPlacesFeatures
                              join actfeature in db.tblFeatures on act.featureID equals actfeature.featureID
                              where act.placeID == id
                              where actfeature.featureTypeID == 1
                              orderby actfeature.feature ascending

                              select new
                              {
                                  value = act.featureID

                              }
                          ).ToList();
            var orientations = (from ori in db.tblPlacesOrientations
                                where ori.placeID == id
                                select new
                                {
                                    value = ori.orientationID,

                                }
                          ).ToList();
            var services = (from act in db.tblPlacesFeatures
                            join actfeature in db.tblFeatures on act.featureID equals actfeature.featureID
                            where act.placeID == id
                            where actfeature.featureTypeID == 5
                            orderby actfeature.feature ascending

                            select new
                            {
                                value = act.featureID

                            }
                          ).ToList();
            var benefits = (from act in db.tblPlacesFeatures
                            join actfeature in db.tblFeatures on act.featureID equals actfeature.featureID
                            where act.placeID == id
                            where actfeature.featureTypeID == 3
                            orderby actfeature.feature ascending

                            select new
                            {
                                value = act.featureID

                            }
                          ).ToList();

            var generalinfo = (from gen in db.tblPlaces
                               join destinations in db.tblDestinations on gen.destinationID equals destinations.destinationID
                               join zones in db.tblZones on gen.zoneID equals zones.zoneID
                               where gen.placeID == id
                               select new
                               {
                                   hotelid = gen.placeID,
                                   hotelname = gen.place,
                                   slogan = gen.placeLabel,
                                   address = gen.address,
                                   phone = gen.phone,
                                   destinationname = destinations.destination,
                                   destinationid = destinations.destinationID,
                                   checkin = gen.checkInTime,
                                   checkout = gen.checkOutTime,
                                   zonename = zones.zone,
                                   zoneid = zones.zoneID,
                                   lat = gen.lat,
                                   lng = gen.lng,
                                   twitter = gen.twitterAccount,
                                   active = gen.active,

                               }
                              ).FirstOrDefault();


            var description = (from des in db.tblPlaceDescriptions
                               where des.placeID == id
                               select new
                               {
                                   value = des.fullDescription,
                                   shortdescription = des.shortDescription

                               }
                          ).FirstOrDefault();

            var images = (from img in db.tblPictures_SysItemTypes
                          join i in db.tblPictures on img.pictureID equals i.pictureID
                          where img.itemID == id && img.sysItemTypeID == sys
                          select new
                          {
                              id = i.pictureID,
                              main = img.main,
                              idsys = img.picture_SysItemTypeID,
                              path = i.path,
                              name = i.file_,
                              logo = img.logo
                          }
                          ).ToList();
            var clasification = (from pla in db.tblPlaces
                                 join cla in db.tblPlaceClasifications on pla.placeClasificationID equals cla.placeClasificationID
                                 where pla.placeID == id

                                 select new
                                 {
                                     value = cla.placeClasificationID,
                                     label = cla.placeClasification

                                 }
                          ).FirstOrDefault();

            var seos = (from seo in db.tblSeoItems

                        where seo.itemID == id && seo.terminalID == 62 && seo.sysItemTypeID == sys

                        select new
                        {
                            title = seo.title,
                            friendlyurl = seo.friendlyUrl,
                            seoid = seo.seoItemID
                        }).FirstOrDefault();



            var rooms = (from room in db.tblRoomTypes
                         where room.placeID == id
                         select room
                         ).ToList();

            List<object> roomsdetails = new List<object>();


            foreach (var item in rooms)
            {
                var details = (from room in db.tblRoomTypes
                               join type in db.tblRoomTypeDescriptions on room.roomTypeID equals type.roomTypeID
                               join prices in db.tblPrices on room.roomTypeID equals prices.itemID

                               where room.roomTypeID == item.roomTypeID && prices.sysItemTypeID == 7
                               select new
                               {
                                   roomid = room.roomTypeID,
                                   description = type.description,
                                   price = prices.price,
                                   name = room.roomType,


                               }

                         ).ToList();


                roomsdetails.Add(details);

            }

            var picsroom = (from pictures in db.tblPictures
                            join pic in db.tblPictures_SysItemTypes on pictures.pictureID equals pic.pictureID
                            where pic.sysItemTypeID == 7 && pic.terminalID == 62
                            select new
                            {

                                picture = pictures.file_,
                                pictureid = pictures.pictureID,
                                picsysid = pic.picture_SysItemTypeID,
                                picid = pictures.pictureID,
                                roomid = pic.itemID

                            }

                        ).ToList();

            object[] data = new[]
                {
                new object[] {activities},
                new object[] { orientations },
                new object[] { services },
                new object[] { benefits },
                new object[] { description },
                new object[] { generalinfo },
                new object[] { images },
                new object[] { clasification },
                new object[] { seos },
                new object[] { roomsdetails },
                new object[] { picsroom }



                 };


            return data;
        }
        /******************************************************************************************************/

        public object updatePlace(string data)
        {

            var place = System.Web.Helpers.Json.Decode(data);

            Int64 id = Convert.ToInt64(place.placeid);
            var placeupdate = (from p in db.tblPlaces
                               where p.placeID == id
                               select p

                               ).FirstOrDefault();

            if (place.placetypeid == 1)
            {
                placeupdate.place = Convert.ToString(place.hotelname);
                placeupdate.placeLabel = Convert.ToString(place.slogan);
                placeupdate.address = Convert.ToString(place.address);
                placeupdate.phone = Convert.ToString(place.phone);
                placeupdate.lat = Convert.ToString(place.lat);
                placeupdate.lng = Convert.ToString(place.lng);
                placeupdate.destinationID = Convert.ToInt64(place.destinationid);
                placeupdate.zoneID = Convert.ToInt32(place.zoneid);
                placeupdate.placeTypeID = Convert.ToInt32(place.placetypeid);
                placeupdate.checkInTime = Convert.ToString(place.checkin);
                placeupdate.checkOutTime = Convert.ToString(place.checkout);
                placeupdate.placeClasificationID = Convert.ToInt32(place.clasification);
                placeupdate.twitterAccount = Convert.ToString(place.twitter);
                placeupdate.active = Convert.ToBoolean(place.active);


            }
            if (place.placetypeid == 2 || place.placetypeid == 16 || place.placetypeid == 21)
            {
                placeupdate.place = Convert.ToString(place.placename);
                placeupdate.address = Convert.ToString(place.address);
                placeupdate.phone = Convert.ToString(place.phone);
                placeupdate.lat = Convert.ToString(place.lat);
                placeupdate.lng = Convert.ToString(place.lng);
                placeupdate.destinationID = Convert.ToInt64(place.destinationid);
                placeupdate.zoneID = Convert.ToInt32(place.zoneid);
                placeupdate.placeTypeID = Convert.ToInt32(place.placetypeid);
                placeupdate.checkInTime = Convert.ToString(place.openat);
                placeupdate.checkOutTime = Convert.ToString(place.closedat);
                placeupdate.twitterAccount = Convert.ToString(place.twitter);
                placeupdate.active = Convert.ToBoolean(place.active);

            }



            var description = (from d in db.tblPlaceDescriptions
                               where d.placeID == id
                               select d

                               ).FirstOrDefault();

            if (description == null)
            {
                tblPlaceDescriptions desc = (new tblPlaceDescriptions
                {
                    placeID = id,
                    fullDescription = Convert.ToString(place.description),
                    shortDescription = Convert.ToString(place.shortdescription),
                    culture = "en-US",

                });
                db.tblPlaceDescriptions.AddObject(desc);
                db.SaveChanges();
            }
            else
            {
                description.fullDescription = Convert.ToString(place.description);
                description.shortDescription = Convert.ToString(place.shortdescription);

            }

            var features = (from feature in db.tblPlacesFeatures
                            where feature.placeID == id
                            select feature

                               );
            foreach (var item in features)
            {
                db.tblPlacesFeatures.DeleteObject(item);
            }
            db.SaveChanges();

            var orientations = (from ori in db.tblPlacesOrientations
                                where ori.placeID == id
                                select ori

                               );
            foreach (var item in orientations)
            {
                db.tblPlacesOrientations.DeleteObject(item);
            }
            db.SaveChanges();



            foreach (var item in place.activities)
            {
                tblPlacesFeatures newactivities = (new tblPlacesFeatures
                {
                    placeID = id,
                    featureID = item,

                });
                db.tblPlacesFeatures.AddObject(newactivities);
                db.SaveChanges();
            }
            foreach (var item in place.services)
            {
                tblPlacesFeatures services = (new tblPlacesFeatures
                {
                    placeID = id,
                    featureID = item,

                });
                db.tblPlacesFeatures.AddObject(services);
                db.SaveChanges();
            }
            foreach (var item in place.benefits)
            {
                tblPlacesFeatures benefits = (new tblPlacesFeatures
                {
                    placeID = id,
                    featureID = item,

                });
                db.tblPlacesFeatures.AddObject(benefits);
                db.SaveChanges();
            }
            foreach (var item in place.orientations)
            {
                tblPlacesOrientations neworientations = (new tblPlacesOrientations
                {
                    placeID = id,
                    orientationID = Convert.ToInt32(item),
                });
                db.tblPlacesOrientations.AddObject(neworientations);
                db.SaveChanges();
            }

            db.SaveChanges();
            Int64 logoid = Convert.ToInt64(place.logoid);
            Int64 sys = Convert.ToInt64(place.sysitem);

            if (logoid != 0)
            {
                var image = (from img in db.tblPictures_SysItemTypes
                             where img.picture_SysItemTypeID == logoid
                             select img

                         ).FirstOrDefault();
                var images = (from img in db.tblPictures_SysItemTypes
                              where img.itemID == id && img.sysItemTypeID == sys

                              select img

                             ).ToList();
                foreach (var item in images)
                {
                    item.logo = false;
                }
                image.logo = true;
            }

            var seos = (from seo in db.tblSeoItems
                        where seo.itemID == id && seo.terminalID == 62 && seo.sysItemTypeID == sys
                        select seo
                        ).FirstOrDefault();

            seos.title = Convert.ToString(place.title);
            seos.friendlyUrl = Convert.ToString(place.friendlyurl);
            db.SaveChanges();



            return "ok";
        }
        /******************************************************************************************************/

        public object verifyPlace(string name, int type)
        {
            var place = (from places in db.tblPlaces
                         where places.place.Contains(name)
                         where places.placeTypeID == type
                         where places.destinationID == 1 || places.destinationID == 4

                         select new
                         {
                             id = places.placeID,
                             name = places.place,
                             address = places.address
                         }


                         ).ToList();



            return place;
        }
        /******************************************************************************************************/

        public object joinPlace(Int64 id, int sys)
        {
            var place = (from places in db.tblPlaces_Terminals
                         where places.placeID == id && places.terminalID == 62

                         select places
                         ).ToList();
            bool isEmpty = !place.Any();
            if (isEmpty)
            {
                tblPlaces_Terminals placeterminal = (new tblPlaces_Terminals
                {
                    placeID = id,
                    terminalID = 62,
                });
                db.tblPlaces_Terminals.AddObject(placeterminal);

                tblSeoItems seo = new tblSeoItems();


                seo.title = "new";
                seo.friendlyUrl = "new";
                seo.url = "/" + id;
                seo.culture = "en-US";
                seo.index_ = false;
                seo.follow = false;
                seo.terminalID = 62;
                seo.sysItemTypeID = sys;
                seo.itemID = id;
                db.tblSeoItems.AddObject(seo);
                db.SaveChanges();
                return "ok";
            }
            else
            {
                return "existe";
            }
        }
        /******************************************************************************************************/

        public object unlinkPlace(Int64 id, int sys)
        {
            var place = (from places in db.tblPlaces_Terminals
                         where places.placeID == id && places.terminalID == 62
                         select places

                               );
            foreach (var item in place)
            {
                db.tblPlaces_Terminals.DeleteObject(item);
            }

            var features = (from feature in db.tblPlacesFeatures
                            where feature.placeID == id
                            select feature

                               );
            foreach (var item in features)
            {
                db.tblPlacesFeatures.DeleteObject(item);
            }


            var orientations = (from ori in db.tblPlacesOrientations
                                where ori.placeID == id
                                select ori

                               );
            foreach (var item in orientations)
            {
                db.tblPlacesOrientations.DeleteObject(item);
            }

            var seos = (from seo in db.tblSeoItems
                        where seo.itemID == id && seo.terminalID == 62 && seo.sysItemTypeID == sys
                        select seo
                            );
            foreach (var item in seos)
            {
                db.tblSeoItems.DeleteObject(item);
            }
            db.SaveChanges();
            return "ok";
        }
        /******************************************************************************************************/

        public object getPlacesOrientationsUpdated()
        {

            var orientations = (from places in db.tblPlacesOrientations
                                join ori in db.tblOrientations on places.orientationID equals ori.orientationID
                                select new
                                {
                                    placeid = places.placeID,
                                    orientation = ori.orientation
                                }

                                ).ToList();


            return orientations;
        }
        /******************************************************************************************************/

        public object addFeature(int id, string feature)
        {

            tblFeatures features = (new tblFeatures
            {
                feature = feature,
                featureTypeID = id,
            });
            db.tblFeatures.AddObject(features);
            db.SaveChanges();
            Int64 newid = features.featureID;
            return newid;
        }
        /******************************************************************************************************/

        public object deleteImage(Int64 id, Int64 idsys)
        {

            var image = (from img in db.tblPictures_SysItemTypes
                         where img.picture_SysItemTypeID == idsys
                         select img

                         ).ToList();
            foreach (var item in image)
            {
                db.tblPictures_SysItemTypes.DeleteObject(item);
            }

            var imgs = (from img in db.tblPictures
                        where img.pictureID == id
                        select img

                         ).ToList();
            foreach (var item in imgs)
            {
                db.tblPictures.DeleteObject(item);
            }
            db.SaveChanges();

            return "ok";
        }
        /******************************************************************************************************/

        public object updateMain(Int64 id, Int64 idsys, int sys)
        {

            var image = (from img in db.tblPictures_SysItemTypes
                         where img.picture_SysItemTypeID == idsys
                         select img

                         ).FirstOrDefault();

            var images = (from img in db.tblPictures_SysItemTypes
                          where img.itemID == id && img.sysItemTypeID == sys

                          select img

                         ).ToList();
            foreach (var item in images)
            {
                item.main = false;
                db.SaveChanges();
            }
            image.main = true;

            db.SaveChanges();

            return "ok";
        }

        /******************************************************************************************************/

        public object addArticle(string data)
        {
            var dato = System.Web.Helpers.Json.Decode(data);
            var id = session.UserID;
            var stat = true;
            tblContentPages page = new tblContentPages();
            page.page = Convert.ToString(dato.title);
            page.terminalID = 62;
            page.savedByUserID = id;
            page.dateSaved = Convert.ToDateTime(dato.date);
            page.pageTypeID = Convert.ToInt32(dato.type);
            page.order_ = 0;
            page.showInMenu = Convert.ToBoolean(dato.showinmenu);
            page.deleted = false;
            page.clickable = false;
            page.publicationDate = DateTime.Now;

            page.authorUserID = Guid.Parse(dato.author);


            db.tblContentPages.AddObject(page);
            db.SaveChanges();

            tblContentPageDescriptions des = new tblContentPageDescriptions();

            des.pageID = page.pageID;
            des.culture = "en-US";
            des.content_ = dato.content;
            des.header = dato.shortcontent;
            des.content_header = dato.titleformat;
            des.pageStructureID = Convert.ToInt32(dato.structure);
            if (dato.status == 1)
            {
                stat = true;
            }
            else
            {
                stat = false;
            }
            des.active = stat;

            db.tblContentPageDescriptions.AddObject(des);

            db.SaveChanges();


            foreach (var item in dato.orientations)
            {
                tblSysItemsOrientations orientations = new tblSysItemsOrientations();
                orientations.itemID = Convert.ToInt64(page.pageID);
                orientations.sysItemID = 9;
                orientations.orientationsID = item;
                db.tblSysItemsOrientations.AddObject(orientations);
                db.SaveChanges();
            }

            tblSeoItems seo = new tblSeoItems();

            seo.title = Convert.ToString(dato.titleseo);
            seo.friendlyUrl = Convert.ToString(dato.friendlyurl);
            seo.url = dato.staticurl + 62 + "/" + page.pageID;
            seo.culture = "en-US";
            seo.index_ = false;
            seo.follow = false;
            seo.terminalID = 62;
            seo.sysItemTypeID = dato.sysitem;
            seo.itemID = page.pageID;
            db.tblSeoItems.AddObject(seo);
            db.SaveChanges();
            return page.pageID;
        }
        /******************************************************************************************************/

        public object getArticles()
        {
            var articles = (from pages in db.tblContentPages
                            join des in db.tblContentPageDescriptions on pages.pageID equals des.pageID
                            where pages.terminalID == 62 && pages.deleted != true
                            orderby pages.dateSaved descending
                            select new
                            {
                                title = pages.page,
                                pageid = pages.pageID,
                                status = des.active,
                                dateSaved = pages.dateSaved,
                                publicationDate = pages.publicationDate

                            }
                            ).ToList();
            return articles;

        }
        /******************************************************************************************************/

        public object getItemsOrientationsUpdated(int itemid)
        {
            var orientations = (from items in db.tblSysItemsOrientations
                                join ori in db.tblOrientations on items.orientationsID equals ori.orientationID
                                where items.sysItemID == itemid
                                select new
                                {
                                    itemid = items.itemID,
                                    orientation = ori.orientation
                                }

                                ).ToList();
            return orientations;

        }
        /******************************************************************************************************/

        public object getItemSelected(Int64 id, int sys)
        {
            var page = (from pages in db.tblContentPages
                        join des in db.tblContentPageDescriptions on pages.pageID equals des.pageID
                        join est in db.tblPageStructures on des.pageStructureID equals est.pageStructureID
                        join types in db.tblPageTypes on pages.pageTypeID equals types.pageTypeID
                        join user in db.tblUserProfiles on pages.authorUserID equals user.userID
                        where pages.pageID == id
                        select new
                        {
                            pageid = pages.pageID,
                            title = pages.page,
                            date = pages.dateSaved,
                            content = des.content_,
                            structureid = des.pageStructureID,
                            structure = est.structureName,
                            status = des.active,
                            type = pages.pageTypeID,
                            typepage = types.pageType,
                            shortcontent = des.header,
                            titleformat = des.content_header,
                            author = user.firstName + " " + user.lastName,
                            authorid = user.userID,
                            publicationdate = pages.publicationDate,
                            showinmenu = pages.showInMenu,

                        }
                ).ToList();
            var orientations = (from ori in db.tblSysItemsOrientations
                                join orient in db.tblOrientations on ori.orientationsID equals orient.orientationID
                                where ori.itemID == id
                                select new
                                {
                                    value = ori.orientationsID,
                                }
                                 ).ToList();

            var images = (from img in db.tblPictures_SysItemTypes
                          join i in db.tblPictures on img.pictureID equals i.pictureID
                          where img.itemID == id && img.sysItemTypeID == sys
                          select new
                          {
                              id = i.pictureID,
                              main = img.main,
                              idsys = img.picture_SysItemTypeID,
                              path = i.path,
                              name = i.file_
                          }
                         ).ToList();
            var seos = (from seo in db.tblSeoItems

                        where seo.itemID == id && seo.terminalID == 62 && seo.sysItemTypeID == sys

                        select new
                        {
                            title = seo.title,
                            friendlyurl = seo.friendlyUrl,
                            seoid = seo.seoItemID
                        }).FirstOrDefault();

            object[] data = new[]
               {
                new object[] {page},
                new object[] { orientations },
                new object[] { images },
                new object[] { seos },



                 };
            return data;

        }
        /******************************************************************************************************/

        public object updateArticle(string data)
        {
            var dato = System.Web.Helpers.Json.Decode(data);
            Int64 articleid = Convert.ToInt64(dato.articleid);
            var id = session.UserID;
            var stat = true;

            var page = (from cont in db.tblContentPages
                        where cont.pageID == articleid
                        select cont

                        ).FirstOrDefault();


            page.page = Convert.ToString(dato.title);
            page.pageTypeID = Convert.ToInt32(dato.type);
            page.publicationDate = Convert.ToDateTime(dato.publicationdate);
            page.showInMenu = Convert.ToBoolean(dato.showinmenu);

            page.authorUserID = Guid.Parse(dato.author);
            db.SaveChanges();

            var des = (from cont in db.tblContentPageDescriptions
                       where cont.pageID == articleid
                       select cont

                       ).FirstOrDefault();

            des.content_ = dato.content;
            des.content_header = dato.titleformat;
            des.header = dato.shortcontent;
            des.pageStructureID = Convert.ToInt32(dato.structure);
            if (dato.status == 1)
            {
                stat = true;
            }
            else
            {
                stat = false;
            }
            des.active = stat;

            db.SaveChanges();


            var ori = (from o in db.tblSysItemsOrientations
                       where o.itemID == articleid && o.sysItemID == 9
                       select o
                       ).ToList();
            foreach (var item in ori)
            {
                db.tblSysItemsOrientations.DeleteObject(item);
            }

            foreach (var item in dato.orientations)
            {
                tblSysItemsOrientations orientations = new tblSysItemsOrientations();
                orientations.itemID = Convert.ToInt32(page.pageID);
                orientations.sysItemID = 9;
                orientations.orientationsID = item;
                db.tblSysItemsOrientations.AddObject(orientations);
                db.SaveChanges();
            }

            Int64 sys = Convert.ToInt64(dato.sysitem);
            var seos = (from seo in db.tblSeoItems
                        where seo.itemID == articleid && seo.terminalID == 62 && seo.sysItemTypeID == sys
                        select seo
                        ).FirstOrDefault();

            seos.title = Convert.ToString(dato.titleseo);
            seos.friendlyUrl = Convert.ToString(dato.friendlyurl);
            db.SaveChanges();
            return dato;
        }
        public object deleteArticle(Int64 pageid, DateTime date, int sys)
        {
            var page = (from p in db.tblContentPages
                        where p.pageID == pageid
                        select p

                        ).FirstOrDefault();
            var user = session.UserID;

            page.deleted = true;
            page.dateDeleted = date;
            page.deletedByUserID = user;

            var seos = (from seo in db.tblSeoItems
                        where seo.itemID == pageid && seo.terminalID == 62 && seo.sysItemTypeID == sys
                        select seo
                            );
            foreach (var item in seos)
            {
                db.tblSeoItems.DeleteObject(item);
            }



            db.SaveChanges();
            return "ok";
        }



        public object searchArticles(string data)
        {
            string article = "%";
            var search = System.Web.Helpers.Json.Decode(data);
            if (search.article != null)
            {
                article = Convert.ToString(search.article);
            }

            if (search.articleid != null)
            {
                Int64 x = Convert.ToInt64(search.articleid);
                var articles = (from pages in db.tblContentPages
                                join des in db.tblContentPageDescriptions on pages.pageID equals des.pageID
                                where pages.pageID == x && pages.deleted != true
                                select new
                                {
                                    title = pages.page,
                                    pageid = pages.pageID,
                                    status = des.active,
                                    dateSaved = pages.dateSaved,
                                    publicationDate = pages.publicationDate
                                }
                             ).ToList();
                return articles;
            }

            if (search.status != null || search.orientation != null)
            {

                if (search.status != null && search.orientation != null)
                {
                    int orientation = Convert.ToInt32(search.orientation);
                    Boolean status = search.status;
                    var articles = (from pages in db.tblContentPages
                                    join des in db.tblContentPageDescriptions on pages.pageID equals des.pageID
                                    join ori in db.tblSysItemsOrientations on pages.pageID equals ori.itemID
                                    where ori.orientationsID == orientation && pages.deleted != true && pages.page.Contains(article) && des.active == status

                                    select new
                                    {
                                        title = pages.page,
                                        pageid = pages.pageID,
                                        status = des.active,
                                        dateSaved = pages.dateSaved,
                                        publicationDate = pages.publicationDate
                                    }
                               ).ToList();
                    return articles;
                }

                if (search.status != null)
                {
                    Boolean status = search.status;
                    var articles = (from pages in db.tblContentPages
                                    join des in db.tblContentPageDescriptions on pages.pageID equals des.pageID
                                    where pages.page.Contains(article) && des.active == status && pages.deleted != true

                                    select new
                                    {
                                        title = pages.page,
                                        pageid = pages.pageID,
                                        status = des.active,
                                        dateSaved = pages.dateSaved,
                                        publicationDate = pages.publicationDate
                                    }
                               ).ToList();
                    return articles;
                }

                if (search.orientation != null)
                {
                    int orientation = Convert.ToInt32(search.orientation);
                    var articles = (from pages in db.tblContentPages
                                    join des in db.tblContentPageDescriptions on pages.pageID equals des.pageID
                                    join ori in db.tblSysItemsOrientations on pages.pageID equals ori.itemID
                                    where ori.orientationsID == orientation && pages.deleted != true && pages.page.Contains(article)

                                    select new
                                    {
                                        title = pages.page,
                                        pageid = pages.pageID,
                                        status = des.active,
                                        dateSaved = pages.dateSaved,
                                        publicationDate = pages.publicationDate
                                    }
                               ).ToList();

                    return articles;
                }

            }

            if (search.article != null)
            {
                var articles = (from pages in db.tblContentPages
                                join des in db.tblContentPageDescriptions on pages.pageID equals des.pageID
                                where pages.page.Contains(article) && pages.deleted != true

                                select new
                                {
                                    title = pages.page,
                                    pageid = pages.pageID,
                                    status = des.active,
                                    dateSaved = pages.dateSaved,
                                    publicationDate = pages.publicationDate
                                }
                            ).ToList();
                return articles;
            }


            return false;
        }
        /******************************************************************************************************/

        public object AddRoom(Int64 placeid, int sys, string room, DateTime date, int price, string description)
        {

            tblRoomTypes rooms = new tblRoomTypes();
            rooms.roomType = room;
            rooms.placeID = placeid;
            rooms.dateSaved = Convert.ToDateTime(date);
            rooms.savedByUserID = session.UserID;
            rooms.quantity = 0;
            db.tblRoomTypes.AddObject(rooms);
            db.SaveChanges();

            tblPrices prices = new tblPrices();
            prices.priceClasificationID = 6;
            prices.priceTypeID = 1;
            prices.sysItemTypeID = sys;
            prices.itemID = rooms.roomTypeID;
            prices.price = Convert.ToDecimal(price);
            prices.currencyID = 1;
            prices.permanent_ = true;
            prices.twPermanent_ = true;
            prices.twFromDate = date;
            prices.terminalID = 62;
            prices.taxesIncluded = true;
            prices.useOnSite = true;
            prices.useOnLine = true;
            prices.highlight = true;

            db.tblPrices.AddObject(prices);

            tblRoomTypeDescriptions des = new tblRoomTypeDescriptions();
            des.roomTypeID = rooms.roomTypeID;
            des.roomType = room;
            des.description = description;
            des.culture = "en-US";
            db.tblRoomTypeDescriptions.AddObject(des);
            db.SaveChanges();

            return rooms.roomTypeID;
        }
        /******************************************************************************************************/
        public object EditRoom(Int64 placeid, int sys, string room, DateTime date, int price, string description)
        {

            var roomtoedit = (from r in db.tblRoomTypes
                              where r.roomTypeID == placeid
                              select r
                        ).FirstOrDefault();
            if (roomtoedit != null)
            {



                roomtoedit.roomType = room;
                db.SaveChanges();

                var roomtypetoedit = (from r in db.tblRoomTypeDescriptions
                                      where r.roomTypeID == placeid
                                      select r
                           ).FirstOrDefault();

                var prices = (from r in db.tblPrices
                              where r.itemID == placeid && r.sysItemTypeID == 7

                              select r
                           ).FirstOrDefault();
                prices.price = Convert.ToDecimal(price);
                roomtypetoedit.roomType = room;
                roomtypetoedit.description = description;

                db.SaveChanges();

                return roomtoedit.roomTypeID;
            }
            return "not";

        }
        /******************************************************************************************************/
        public object GetRooms(Int64 id)
        {

            var rooms = (from room in db.tblRoomTypes
                         where room.placeID == id
                         select room
                           ).ToList();

            List<object> roomsdetails = new List<object>();


            foreach (var item in rooms)
            {
                var details = (from room in db.tblRoomTypes
                               join type in db.tblRoomTypeDescriptions on room.roomTypeID equals type.roomTypeID
                               join prices in db.tblPrices on room.roomTypeID equals prices.itemID

                               where room.roomTypeID == item.roomTypeID && prices.sysItemTypeID == 7
                               select new
                               {
                                   roomid = room.roomTypeID,
                                   description = type.description,
                                   price = prices.price,
                                   name = room.roomType,


                               }

                         ).ToList();


                roomsdetails.Add(details);

            }

            var picsroom = (from pictures in db.tblPictures
                            join pic in db.tblPictures_SysItemTypes on pictures.pictureID equals pic.pictureID
                            where pic.sysItemTypeID == 7 && pic.terminalID == 62
                            select new
                            {

                                picture = pictures.file_,
                                pictureid = pictures.pictureID,
                                picsysid = pic.picture_SysItemTypeID,
                                picid = pictures.pictureID,
                                roomid = pic.itemID

                            }

                        ).ToList();
            object[] data = new[]
               {
                new object[] { roomsdetails },
                new object[] { picsroom }
                 };

            return data;
        }
        /******************************************************************************************************/
        public object DeleteRoom(Int64 id, int sysid, Int64 picid)
        {
            var room = (from r in db.tblRoomTypeDescriptions
                        where r.roomTypeID == id
                        select r
                        ).ToList();
            foreach (var item in room)
            {
                db.tblRoomTypeDescriptions.DeleteObject(item);
            }
            db.SaveChanges();
            var roomtype = (from r in db.tblRoomTypes
                            where r.roomTypeID == id
                            select r
                         ).ToList();
            foreach (var item in roomtype)
            {
                db.tblRoomTypes.DeleteObject(item);
            }


            var image = (from img in db.tblPictures_SysItemTypes
                         where img.picture_SysItemTypeID == sysid
                         select img

                        ).ToList();
            foreach (var item in image)
            {
                db.tblPictures_SysItemTypes.DeleteObject(item);
            }

            var imgs = (from img in db.tblPictures
                        where img.pictureID == picid
                        select img

                         ).ToList();
            foreach (var item in imgs)
            {
                db.tblPictures.DeleteObject(item);
            }
            db.SaveChanges();





            return "ok";
        }

        /******************************************************************************************************/
        public object SaveSignature(Guid leadID, string signature, int type)
        {

            tblSignatures sign = new tblSignatures();
            Guid id = Guid.NewGuid();
            sign.signatureID = id;
            sign.terminalID = 62;
            sign.documentTypeID = 2;
            sign.signature = signature;
            sign.dateSaved = DateTime.Now;
            sign.urlDocument = "senses";
            sign.emailsToNotify = "senses";

            if (type == 1)
            {
                sign.harvestingID = leadID;
            }
            if (type == 2)
            {
                sign.leadID = leadID;
            }

            db.tblSignatures.AddObject(sign);
            db.SaveChanges();
            return "ok";
        }
        /******************************************************************************************************/
        public object GetOrientations()
        {

            var orientations = (from ori in db.tblOrientations
                                select new
                                {
                                    value = ori.orientationID,
                                    text = ori.orientation
                                }
                         ).ToList();

            var orientations2 = (from ori in db.tblOrientations
                                 select new
                                 {
                                     value = ori.orientationID,
                                     label = ori.orientation
                                 }
                         ).ToList();
            object[] data = new[]
              {
                new object[] {orientations},
                new object[] {orientations2},

                 };

            return data;

        }

        /******************************************************************************************************/
        public object GetAllPlaces()
        {

            var places = (from place in db.tblPlaces
                          join destination in db.tblDestinations on place.destinationID equals destination.destinationID
                          join zone in db.tblZones on place.zoneID equals zone.zoneID

                          orderby place.place
                          where place.destinationID == 1 || place.destinationID == 4

                          select new
                          {
                              id = place.placeID,
                              value = place.placeID,
                              name = place.place,
                              label = place.place,
                              namelabel = place.placeLabel,
                              address = place.address,
                              phone = place.phone,
                              lat = place.lat,
                              lng = place.lng,
                              destination = destination.destination,
                              zone = zone.zone,
                              zoneid = zone.zoneID,
                              checkin = place.checkInTime,
                              checkout = place.checkOutTime,


                          }

                         ).ToList();

            var placeszones = (from zone in db.tblZones
                               where zone.destinationID == 1 || zone.destinationID == 4

                               select new
                               {
                                   label = zone.zone,
                                   value = zone.zoneID,
                               }

                         ).ToList();
            object[] data = new[]
                {
                new object[] {places},
                new object[] {placeszones},

                 };

            return data;
        }

        /******************************************************************************************************/
        public object SaveEvent(string data)
        {
            tblSeoItems seo = new tblSeoItems();

            var eve = System.Web.Helpers.Json.Decode(data);
            tblCalendarEvents calendar = new tblCalendarEvents();
            calendar.Event = eve.evento;
            calendar.Description = eve.description;
            calendar.FromDateTime = Convert.ToDateTime(eve.fromdate);
            calendar.ToDateTime = Convert.ToDateTime(eve.todate);
            calendar.AllDay = Convert.ToBoolean(eve.allday);
            calendar.RepeatingEvent = Convert.ToBoolean(eve.repeatingevent);
            calendar.Link = eve.link;
            calendar.Sunday = Convert.ToBoolean(eve.sunday);
            calendar.Monday = Convert.ToBoolean(eve.monday);
            calendar.Tuesday = Convert.ToBoolean(eve.tuesday);
            calendar.Wednesday = Convert.ToBoolean(eve.wednesday);
            calendar.Thursday = Convert.ToBoolean(eve.thursday);
            calendar.Friday = Convert.ToBoolean(eve.friday);
            calendar.Saturday = Convert.ToBoolean(eve.saturday);
            calendar.LocationResortId = Convert.ToInt32(eve.locationid);
            calendar.isAffiliate = Convert.ToBoolean(eve.isaffiliate);
            calendar.DateCreated = DateTime.Today;
            calendar.isActive = Convert.ToBoolean(eve.active);




            if (eve.repeatingevent == true)
            {
                calendar.RepeatType = Convert.ToInt32(eve.repeattype);
                calendar.GapBetweenRepeats = Convert.ToInt32(eve.gaprepeat);
                calendar.NumberOfRepeats = Convert.ToInt32(eve.numberrepeat);
                calendar.RepeatMode = Convert.ToInt32(eve.moderepeat);
            }
            else
            {
                calendar.RepeatType = null;
                calendar.GapBetweenRepeats = null;
                calendar.NumberOfRepeats = null;
                calendar.RepeatMode = null;
            }

            db.tblCalendarEvents.AddObject(calendar);
            db.SaveChanges();

            foreach (var item in eve.orientations)
            {
                tblEventsOrientations orientations = (new tblEventsOrientations
                {
                    eventID = calendar.EventId,
                    orientationID = Convert.ToInt32(item),
                });
                db.tblEventsOrientations.AddObject(orientations);
                db.SaveChanges();
            }

            if (eve.locationid != null || eve.locationid != "")
            {
                tblPlacesEvents place = new tblPlacesEvents();
                place.placeID = Convert.ToInt64(eve.locationid);
                place.eventID = calendar.EventId;
                db.tblPlacesEvents.AddObject(place);
            }

            seo.title = Convert.ToString(eve.titleseo);
            seo.friendlyUrl = Convert.ToString(eve.slug);
            seo.url = eve.slug + 62 + "/" + calendar.EventId;
            seo.culture = "en-US";
            seo.index_ = false;
            seo.follow = false;
            seo.terminalID = 62;
            seo.sysItemTypeID = 10;
            seo.itemID = calendar.EventId;
            db.tblSeoItems.AddObject(seo);
            db.SaveChanges();

            db.SaveChanges();


            return calendar.EventId;
        }


        /******************************************************************************************************/
        public object GetEvents()
        {
            var events = (from evento in db.tblCalendarEvents
                          join eventplace in db.tblPlacesEvents on evento.EventId equals eventplace.eventID
                          join place in db.tblPlaces on eventplace.placeID equals place.placeID
                          join zone in db.tblZones on place.zoneID equals zone.zoneID
                          select new
                          {
                              id = evento.EventId,
                              evento = evento.Event,
                              place = place.place,
                              zone = zone.zone,
                              datefrom = evento.FromDateTime,
                              dateto = evento.ToDateTime,
                              created = evento.DateCreated

                          }

                          ).ToList();
            var events2 = (from evento in db.tblCalendarEvents
                           where evento.LocationResortId == 1
                           select new
                           {
                               id = evento.EventId,
                               evento = evento.Event,
                               place = "",
                               zone = "",
                               datefrom = evento.FromDateTime,
                               dateto = evento.ToDateTime,
                               created = evento.DateCreated


                           }

                          ).ToList();

            var orientations = (from eventorientations in db.tblEventsOrientations
                                join ori in db.tblOrientations on eventorientations.orientationID equals ori.orientationID

                                select new
                                {
                                    eventoid = eventorientations.eventID,
                                    orientation = ori.orientation,
                                    orientationid = ori.orientationID
                                }

                                ).ToList();

            var a = events.Concat(events2).OrderByDescending(xs => xs.created);
            var x = a.ToArray();
            object[] data = new[]
               {
                new object[] {x},
                new object[] {orientations},

                 };

            return data;
        }
        /******************************************************************************************************/

        public object GetEventSelected(int id)
        {
            var eve = (from eventos in db.tblCalendarEvents
                       where eventos.EventId == id
                       select eventos

                     ).FirstOrDefault();

            if (eve.LocationResortId == 1)
            {
                var events = (from e in db.tblCalendarEvents
                              join seo in db.tblSeoItems on e.EventId equals seo.itemID

                              where e.EventId == id
                              where seo.sysItemTypeID == 10 && seo.itemID == id
                              select new
                              {
                                  id = e.EventId,
                                  evento = e.Event,
                                  picture = e.Picture,
                                  description = e.Description,
                                  allday = e.AllDay,
                                  repetingevent = e.RepeatingEvent,
                                  repeattype = e.RepeatType,
                                  gaprepeat = e.GapBetweenRepeats,
                                  numberrepeat = e.NumberOfRepeats,
                                  sunday = e.Sunday,
                                  monday = e.Monday,
                                  tuesday = e.Tuesday,
                                  wednesday = e.Wednesday,
                                  thursday = e.Thursday,
                                  friday = e.Friday,
                                  saturday = e.Saturday,
                                  link = e.Link,
                                  datefrom = e.FromDateTime,
                                  dateto = e.ToDateTime,
                                  titleseo = seo.title,
                                  slug = seo.friendlyUrl,
                                  moderepeat = e.RepeatMode,
                                  isaffiliate = e.isAffiliate,
                                  active = e.isActive,
                              }

                           ).FirstOrDefault();
                var orientations = (from eventorientations in db.tblEventsOrientations
                                    join ori in db.tblOrientations on eventorientations.orientationID equals ori.orientationID
                                    where eventorientations.eventID == id
                                    select new
                                    {
                                        eventoid = eventorientations.eventID,
                                        orientation = ori.orientation,
                                        orientationid = ori.orientationID
                                    }

                               ).ToList();


                object[] data = new[]
                   {
                new object[] {events},
                new object[] {orientations},

                 };

                return data;

            }
            else
            {
                var events = (from e in db.tblCalendarEvents
                              join eventplace in db.tblPlacesEvents on e.EventId equals eventplace.eventID
                              join place in db.tblPlaces on eventplace.placeID equals place.placeID
                              join seo in db.tblSeoItems on e.EventId equals seo.itemID
                              join zone in db.tblZones on place.zoneID equals zone.zoneID
                              join destination in db.tblDestinations on place.destinationID equals destination.destinationID

                              where e.EventId == id
                              where seo.sysItemTypeID == 10 && seo.itemID == id
                              select new
                              {
                                  id = e.EventId,
                                  address = place.address,
                                  active = e.isActive,
                                  evento = e.Event,
                                  place = place.place,
                                  zone = zone.zone,
                                  destination = destination.destination,
                                  picture = e.Picture,
                                  description = e.Description,
                                  allday = e.AllDay,
                                  repetingevent = e.RepeatingEvent,
                                  repeattype = e.RepeatType,
                                  gaprepeat = e.GapBetweenRepeats,
                                  numberrepeat = e.NumberOfRepeats,
                                  sunday = e.Sunday,
                                  monday = e.Monday,
                                  tuesday = e.Tuesday,
                                  wednesday = e.Wednesday,
                                  thursday = e.Thursday,
                                  friday = e.Friday,
                                  saturday = e.Saturday,
                                  link = e.Link,
                                  datefrom = e.FromDateTime,
                                  dateto = e.ToDateTime,
                                  titleseo = seo.title,
                                  slug = seo.friendlyUrl,
                                  placeid = place.placeID,
                                  moderepeat = e.RepeatMode,
                                  isaffiliate = e.isAffiliate,

                              }

                              ).FirstOrDefault();
                var orientations = (from eventorientations in db.tblEventsOrientations
                                    join ori in db.tblOrientations on eventorientations.orientationID equals ori.orientationID
                                    where eventorientations.eventID == id
                                    select new
                                    {
                                        eventoid = eventorientations.eventID,
                                        orientation = ori.orientation,
                                        orientationid = ori.orientationID
                                    }

                               ).ToList();


                object[] data = new[]
                   {
                new object[] {events},
                new object[] {orientations},

                 };

                return data;
            }


            return "";

        }
        /******************************************************************************************************/

        public object DeleteImageEvent(int id)
        {
            var evento = (from e in db.tblCalendarEvents
                          where e.EventId == id
                          select e
                          ).FirstOrDefault(); ;

            evento.Picture = null;
            db.SaveChanges();

            return "ok";

        }
        /******************************************************************************************************/
        public object UpdateEvent(string data)
        {
            var eve = System.Web.Helpers.Json.Decode(data);
            Int32 id = Convert.ToInt32(eve.eventid);


            var calendar = (from e in db.tblCalendarEvents
                            where e.EventId == id
                            select e
                            ).FirstOrDefault();


            calendar.Event = eve.evento;
            calendar.Description = eve.description;
            calendar.FromDateTime = Convert.ToDateTime(eve.fromdate);
            calendar.ToDateTime = Convert.ToDateTime(eve.todate);
            calendar.AllDay = Convert.ToBoolean(eve.allday);
            calendar.RepeatingEvent = Convert.ToBoolean(eve.repeatingevent);
            calendar.Link = eve.link;
            calendar.Sunday = Convert.ToBoolean(eve.sunday);
            calendar.Monday = Convert.ToBoolean(eve.monday);
            calendar.Tuesday = Convert.ToBoolean(eve.tuesday);
            calendar.Wednesday = Convert.ToBoolean(eve.wednesday);
            calendar.Thursday = Convert.ToBoolean(eve.thursday);
            calendar.Friday = Convert.ToBoolean(eve.friday);
            calendar.Saturday = Convert.ToBoolean(eve.saturday);
            calendar.isAffiliate = Convert.ToBoolean(eve.isaffiliate);
            calendar.isActive = Convert.ToBoolean(eve.active);



            if (eve.repeatingevent == true)
            {
                calendar.RepeatType = Convert.ToInt32(eve.repeattype);
                calendar.GapBetweenRepeats = Convert.ToInt32(eve.gaprepeat);
                calendar.NumberOfRepeats = Convert.ToInt32(eve.numberrepeat);
                calendar.RepeatMode = Convert.ToInt32(eve.moderepeat);
            }
            else
            {
                calendar.RepeatType = null;
                calendar.GapBetweenRepeats = null;
                calendar.NumberOfRepeats = null;
                calendar.RepeatMode = null;
            }


            db.SaveChanges();

            var o = (from ori in db.tblEventsOrientations
                     where ori.eventID == id
                     select ori
                     ).ToList();

            foreach (var item in o)
            {
                db.tblEventsOrientations.DeleteObject(item);
            }

            foreach (var item in eve.orientations)
            {
                tblEventsOrientations orientations = (new tblEventsOrientations
                {
                    eventID = id,
                    orientationID = Convert.ToInt32(item),
                });
                db.tblEventsOrientations.AddObject(orientations);
                db.SaveChanges();
            }


            var pla = (from p in db.tblPlacesEvents
                       where p.eventID == id
                       select p
                       ).ToList();
            foreach (var item in pla)
            {
                db.tblPlacesEvents.DeleteObject(item);
            }

            if (eve.locationid != null)
            {
                tblPlacesEvents place = new tblPlacesEvents();
                place.placeID = Convert.ToInt64(eve.locationid);
                place.eventID = id;
                db.tblPlacesEvents.AddObject(place);
                calendar.LocationResortId = null;
            }
            else
            {
                calendar.LocationResortId = 1;
            }





            var seo = (from s in db.tblSeoItems
                       where s.itemID == id && s.terminalID == 62 && s.sysItemTypeID == 10
                       select s
                       ).FirstOrDefault();
            seo.title = Convert.ToString(eve.titleseo);
            seo.friendlyUrl = Convert.ToString(eve.slug);
            db.SaveChanges();
            return "ok";
        }

        /******************************************************************************************************/

        public object SearchEvents(string data)
        {
            string eventos = "";
            var search = System.Web.Helpers.Json.Decode(data);




            string hasID = Convert.ToString(search.eventid);
            string hasPlaceID = Convert.ToString(search.location);
            string hasZone = Convert.ToString(search.zone);
            string hasOrientations = Convert.ToString(search.orientation);
            string hasEvent = Convert.ToString(search.evento);

            Int64 zoneid = Convert.ToInt64(search.zone);
            Int64 x = Convert.ToInt64(search.eventid);
            Int64 placeid = Convert.ToInt64(search.location);
            Int64 orientation = Convert.ToInt64(search.orientation);
            string namevent = Convert.ToString(search.evento);

            var events = (from evento in db.tblCalendarEvents
                          join eventplace in db.tblPlacesEvents on evento.EventId equals eventplace.eventID
                          join place in db.tblPlaces on eventplace.placeID equals place.placeID
                          join zone in db.tblZones on place.zoneID equals zone.zoneID
                          join ori in db.tblEventsOrientations on evento.EventId equals ori.eventID

                          where (hasID != null ? evento.EventId == x : evento.EventId > 0) &&
                          (hasPlaceID != null ? eventplace.placeID == placeid : eventplace.placeID > 0) &&
                          (hasZone != null ? place.zoneID == zoneid : place.zoneID > 0) &&
                          (hasOrientations != null ? ori.orientationID == orientation : ori.orientationID > 0) &&
                          (hasEvent != null ? evento.Event.Contains(namevent) : evento.Event.Contains(eventos))


                          select new
                          {
                              id = evento.EventId,
                              evento = evento.Event,
                              place = place.place,
                              zone = zone.zone,
                              datefrom = evento.FromDateTime,
                              dateto = evento.ToDateTime,
                              created = evento.DateCreated

                          }
                       ).ToList();
            var events2 = (from evento in db.tblCalendarEvents
                           join ori in db.tblEventsOrientations on evento.EventId equals ori.eventID
                           where evento.LocationResortId == 1
                           where (hasID != null ? evento.EventId == x : evento.EventId > 0) &&
                           (hasOrientations != null ? ori.orientationID == orientation : ori.orientationID > 0) &&
                           (hasEvent != null ? evento.Event.Contains(namevent) : evento.Event.Contains(eventos))


                           select new
                           {
                               id = evento.EventId,
                               evento = evento.Event,
                               place = "",
                               zone = "",
                               datefrom = evento.FromDateTime,
                               dateto = evento.ToDateTime,
                               created = evento.DateCreated

                           }
                       ).ToList();
            var a = events.Concat(events2).OrderByDescending(eve => eve.created);
            var xs = a.ToArray();
            return xs;

        }
        /******************************************************************************************************/

        public object GetReviews()
        {
            var terminals = session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();

            var reviews = (from r in db.tblReviews
                           join sys in db.tblSysItemTypes on r.sysItemTypeID equals sys.sysItemTypeID
                           join place in db.tblPlaces on r.itemID equals place.placeID

                           //      join cont in db.tblContentPages on r.itemID equals cont.pageID
                           join type in db.tblPlaceTypes on place.placeTypeID equals type.placeTypeID
                           where terminals.Contains(r.terminalID)
                           where r.sysItemTypeID == 6
                           orderby r.dateSaved descending
                           select new
                           {
                               placename = place.place,
                               typeplace = type.placeType,
                               id = r.reviewID,
                               name = r.author,
                               review = r.review,
                               stars = r.rating,
                               status = r.active,
                               terminal = r.terminalID,
                               date = r.dateSaved,
                           }
                           ).ToList();
            var reviewspages = (from r in db.tblReviews
                                join sys in db.tblSysItemTypes on r.sysItemTypeID equals sys.sysItemTypeID
                                // join place in db.tblPlaces on r.itemID equals place.placeID
                                join cont in db.tblContentPages on r.itemID equals cont.pageID
                                join type in db.tblPageTypes on cont.pageTypeID equals type.pageTypeID
                                where r.sysItemTypeID == 9
                                where terminals.Contains(r.terminalID)
                                orderby r.dateSaved descending
                                select new
                                {
                                    placename = cont.page,
                                    typeplace = type.pageType,
                                    id = r.reviewID,
                                    name = r.author,
                                    review = r.review,
                                    stars = r.rating,
                                    status = r.active,
                                    terminal = r.terminalID,
                                    date = r.dateSaved,


                                }
                           ).ToList();
            var activities = (from r in db.tblReviews
                              join sys in db.tblSysItemTypes on r.sysItemTypeID equals sys.sysItemTypeID
                              // join place in db.tblPlaces on r.itemID equals place.placeID
                              join cont in db.tblServices on r.itemID equals cont.serviceID
                              join p in db.tblProviders on cont.providerID equals p.providerID
                              where r.sysItemTypeID == 1
                              where terminals.Contains(r.terminalID)
                              orderby r.dateSaved descending
                              select new
                              {
                                  placename = p.comercialName + " - " + cont.service,
                                  typeplace = "Activity",
                                  id = r.reviewID,
                                  name = r.author,
                                  review = r.review,
                                  stars = r.rating,
                                  status = r.active,
                                  terminal = r.terminalID,
                                  date = r.dateSaved,


                              }
                           ).ToList();

            var total = reviews.Concat(reviewspages);
            total = total.Concat(activities);
            return total.OrderByDescending(x => x.date);

        }
        /******************************************************************************************************/

        public object StatusReview(Int64 id)
        {
            var reviews = (from r in db.tblReviews
                           where r.reviewID == id
                           select r
                           ).FirstOrDefault();

            reviews.active = !reviews.active;
            db.SaveChanges();
            return reviews.active;

        }
        /******************************************************************************************************/

        public object DeleteEvent(Int64 eventid)
        {
            var evento = (from e in db.tblCalendarEvents
                          where e.EventId == eventid
                          select e
                          ).FirstOrDefault();

            db.tblCalendarEvents.DeleteObject(evento);

            var ori = (from o in db.tblEventsOrientations
                       where o.eventID == eventid
                       select o
                       ).ToList();
            foreach (var item in ori)
            {
                db.tblEventsOrientations.DeleteObject(item);
            }

            var places = (from p in db.tblPlacesEvents
                          where p.eventID == eventid
                          select p
                          ).FirstOrDefault();
            if (places != null)
            {
                db.tblPlacesEvents.DeleteObject(places);
            }
            db.SaveChanges();

            return "ok";

        }
        /******************************************************************************************************/
        public object updateFeature(Int64 id, string text)
        {
            var features = (from f in db.tblFeatures
                            where f.featureID == id
                            select f
                            ).FirstOrDefault();
            features.feature = text;
            db.SaveChanges();
            return "ok";
        }

        /******************************************************************************************************/
        public object deleteFeature(Int64 id)
        {
            var features = (from f in db.tblFeatures
                            where f.featureID == id
                            select f
                            ).FirstOrDefault();
            db.tblFeatures.DeleteObject(features);
            db.SaveChanges();
            return "ok";
        }

        /******************************************************************************************************/
        public object getSurveysGroup()
        {
            var surveys = (from s in db.tblFieldGroups
                           join user in db.tblUserProfiles on s.savedByUserID equals user.userID
                           where s.terminalID == 62
                           select new
                           {
                               name = s.fieldGroup,
                               id = s.fieldGroupID,
                               date = s.dateSaved,
                               url = s.fieldGroupGUID,
                               username = user.firstName + " " + user.lastName,
                               description = s.description
                           }

                          ).ToList();
            var a = surveys.ToArray();

            return surveys;
        }
        public class surveyitems : List<surveyitems>
        {
            public int id { get; set; }
            public string surveyname { get; set; }
            public string url { get; set; }
            public string createdby { get; set; }
            public string createdon { get; set; }

        }
        /******************************************************************************************************/
        public object getTemplateSurvey(Int64 fieldGroudID)
        {
            var surveys = (from s in db.tblFields
                           join t in db.tblFieldSubTypes on s.fieldSubTypeID equals t.fieldSubTypeID
                           join sf in db.tblFieldTypes on s.fieldTypeID equals sf.fieldTypeID

                           where s.fieldGroupID == fieldGroudID
                           select new
                           {

                               field = s.field,
                               fieldid = s.fieldID,
                               subfieldid = s.fieldSubTypeID,
                               subfield = t.subType,
                               description = s.description,
                               parentfieldid = s.parentFieldID,
                               order = s.order_,
                               visibleif = s.visibleIfFieldGuid,
                               guidfield = s.fieldGuid,
                               fieldtypeid = sf.fieldTypeID,
                               fieldoptions = s.options,

                           }

                          ).ToList();

            return surveys;
        }
        /******************************************************************************************************/
        public object SaveSurvey(int fieldGroupID, string data, Guid ambassadorID, Boolean contactInfo, string culture, Int64 answerID, DateTime date,
            string membership, int location, int membershipToLink, int locationID, DateTime departureDate)
        {
            var survey = System.Web.Helpers.Json.Decode(data);

            tblFieldGroupsAnswers newsurvey = new tblFieldGroupsAnswers();

            var folio = "";
            var name = "";
            var lastname = "";
            var phone = "";
            var cellphone = "";
            var email = "";
            var tags = "";

            foreach (var item in survey)
            {
                if (item.fieldid == 1198 || item.fieldid == 1200)
                {
                    folio = item.answer;
                }
                if (item.fieldid == 1151 || item.fieldid == 1166)
                {
                    name = item.answer;
                }
                if (item.fieldid == 1194 || item.fieldid == 1167)
                {
                    lastname = item.answer;
                }
                if (item.fieldid == 1154 || item.fieldid == 1170)
                {
                    phone = item.answer;
                }
                if (item.fieldid == 1197 || item.fieldid == 1171)
                {
                    cellphone = item.answer;
                }
                if (item.fieldid == 1153 || item.fieldid == 1172)
                {
                    email = item.answer;
                }
                if (item.fieldid == 1160 || item.fieldid == 1160)
                {
                    if (item.answer != "" && item.answet != null)
                    {
                        tags = "#" + item.answer.Trim().Remove(item.answer.Trim().IndexOf(" "));
                    }
                }



            }


            Guid leadID = Guid.NewGuid();


            Guid leadIDExists = new Guid("00000000-0000-0000-0000-000000000000");
            string mobile = null;
            string filteredPhone = "";
            string filteredEmail = "";
            string filteredMobile = "";

            if (email != null && email != "")
            {
                filteredEmail = email.Trim();
            }
            if (phone != null && phone != "" && phone.Length >= 10)
            {
                filteredPhone = Regex.Replace(phone, @"[^\d]", "");
                if (filteredPhone.Length >= 10)
                {
                    filteredPhone = filteredPhone.Substring(filteredPhone.Length - 10);
                }
            }
            if (cellphone != null && cellphone != "")
            {
                filteredMobile = Regex.Replace(cellphone, @"[^\d]", "");
                if (filteredMobile.Length >= 10)
                {
                    filteredMobile = filteredMobile.Substring(filteredMobile.Length - 10);
                }
            }
            Guid emailLead = new Guid("00000000-0000-0000-0000-000000000000");
            Guid phoneLead = new Guid("00000000-0000-0000-0000-000000000000");
            Guid cellphoneLead = new Guid("00000000-0000-0000-0000-000000000000");
            if (email != null && email.Length >= 10 && filteredEmail != "")
            {
                emailLead = (from e in db.tblLeads
                             join c in db.tblLeadEmails on e.leadID equals c.leadID
                             where c.email == filteredEmail && e.terminalID == 62
                             select e.leadID
                            ).FirstOrDefault();

            }
            if (phone != null && phone.Length >= 10 && filteredPhone != "")
            {
                phoneLead = (from e in db.tblLeads
                             join p in db.tblPhones on e.leadID equals p.leadID
                             where p.phone == filteredPhone && e.terminalID == 62
                             select e.leadID
                            ).FirstOrDefault();
            }
            if (cellphone != null && cellphone.Length >= 10 && filteredMobile != "")
            {
                cellphoneLead = (from e in db.tblLeads
                                 join p in db.tblPhones on e.leadID equals p.leadID
                                 where p.phone == filteredMobile && e.terminalID == 62
                                 select e.leadID
                                            ).FirstOrDefault();
            }

            if (emailLead != new Guid("00000000-0000-0000-0000-000000000000"))
            {
                leadIDExists = emailLead;
                leadID = leadIDExists;
            }
            if (phoneLead != new Guid("00000000-0000-0000-0000-000000000000"))
            {
                leadIDExists = phoneLead;
                leadID = leadIDExists;

            }
            if (cellphoneLead != new Guid("00000000-0000-0000-0000-000000000000"))
            {
                leadIDExists = cellphoneLead;
                leadID = leadIDExists;

            }

            tblLeads newLead = new tblLeads();
            if (emailLead != new Guid("00000000-0000-0000-0000-000000000000") ||
                cellphoneLead != new Guid("00000000-0000-0000-0000-000000000000")
                || phoneLead != new Guid("00000000-0000-0000-0000-000000000000")
                )
            {
                newsurvey = (from f in db.tblFieldGroupsAnswers
                             where f.leadID == leadID
                             select f
                            ).FirstOrDefault();
                if (newsurvey != null)
                {
                    newsurvey.fieldGroupsID = fieldGroupID;
                    newsurvey.answers = data;
                    newsurvey.dateSaved = date;
                    newsurvey.folio = folio;
                    newsurvey.savedByUserID = ambassadorID;
                    newsurvey.terminalID = 62;
                    newsurvey.capturedByUserID = session.UserID;
                    newsurvey.culture = culture;
                    newsurvey.contactInfo = contactInfo;
                    newsurvey.locationID = locationID;
                    newsurvey.DepartureDate = departureDate;
                    //newsurvey.dateImported = date;
                    db.SaveChanges();
                }
                else
                {
                    newsurvey = new tblFieldGroupsAnswers();
                    newsurvey.fieldGroupsID = fieldGroupID;
                    newsurvey.answers = data;
                    newsurvey.dateSaved = date;
                    newsurvey.folio = folio;
                    newsurvey.savedByUserID = ambassadorID;
                    newsurvey.terminalID = 62;
                    newsurvey.capturedByUserID = session.UserID;
                    newsurvey.culture = culture;
                    newsurvey.contactInfo = contactInfo;
                    newsurvey.locationID = locationID;
                    newsurvey.DepartureDate = departureDate;
                    db.tblFieldGroupsAnswers.AddObject(newsurvey);
                    newsurvey.dateImported = date;

                    db.SaveChanges();
                }
            }
            else
            {
                newsurvey.fieldGroupsID = fieldGroupID;
                newsurvey.answers = data;
                newsurvey.dateSaved = date;
                newsurvey.folio = folio;
                newsurvey.savedByUserID = ambassadorID;
                newsurvey.terminalID = 62;
                newsurvey.capturedByUserID = session.UserID;
                newsurvey.culture = culture;
                newsurvey.contactInfo = contactInfo;
                newsurvey.locationID = locationID;
                newsurvey.DepartureDate = departureDate;
                db.tblFieldGroupsAnswers.AddObject(newsurvey);
                newsurvey.dateImported = date;

                db.SaveChanges();
            }

            var field = (from f in db.tblFieldGroupsAnswers
                         where f.FieldGroupsAnswersID == newsurvey.FieldGroupsAnswersID
                         select f
                         ).FirstOrDefault();



            if (leadIDExists == new Guid("00000000-0000-0000-0000-000000000000"))
            {
                newLead.leadID = leadID;
                newLead.firstName = name;
                newLead.lastName = lastname;
                newLead.referenceID = newsurvey.FieldGroupsAnswersID.ToString();
                newLead.initialTerminalID = 62;
                newLead.terminalID = 62;
                newLead.interestedInDestinationID = 1;
                newLead.inputDateTime = DateTime.Now;
                newLead.dateCollected = date;
                if (locationID == 347)
                {
                    newLead.leadTypeID = 61;
                }
                else
                {
                    newLead.leadTypeID = 57;
                }
                newLead.bookingStatusID = 19; //Not Contacted
                newLead.leadSourceID = 55;
                newLead.leadSourceChannelID = 11;
                newLead.inputMethodID = 2; //import
                newLead.submissionForm = false;
                newLead.inputByUserID = session.UserID;
                newLead.deleted = false;
                newLead.isTest = false;
                newLead.leadStatusID = 2;
                newLead.tags = tags;
            }
            else
            {
                newLead = (from l in db.tblLeads
                           where l.leadID == leadIDExists && l.terminalID == 62
                           select l
                           ).FirstOrDefault();
                if (newLead != null)
                {
                    //newLead.referenceID = newsurvey.FieldGroupsAnswersID.ToString();
                    newLead.interestedInDestinationID = 1;
                    //newLead.inputDateTime = date;
                    if (locationID == 347)
                    {
                        newLead.leadTypeID = 61;
                    }
                    else
                    {
                        newLead.leadTypeID = 57;
                    }
                    newLead.bookingStatusID = 19; //Not Contacted
                    newLead.leadSourceID = 55;
                    newLead.leadSourceChannelID = 11;
                    //newLead.inputMethodID = 2; //import
                    newLead.submissionForm = false;
                    //newLead.inputByUserID = session.UserID;
                    newLead.deleted = false;
                    newLead.isTest = false;
                    newLead.leadStatusID = 2;
                    newLead.tags = tags;
                }

            }


            if (phone.Trim() != "" && phone != null)
            {
                if (phoneLead == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    tblPhones newPhone = new tblPhones();
                    newPhone.phone = filteredPhone;
                    newPhone.leadID = leadID;
                    newPhone.main = true;
                    newPhone.phoneTypeID = 3;
                    db.tblPhones.AddObject(newPhone);
                }

            }
            if (cellphone.Trim() != "" && cellphone != null)
            {
                if (cellphoneLead == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    tblPhones newCellphone = new tblPhones();
                    newCellphone.phone = filteredMobile;
                    newCellphone.leadID = leadID;
                    newCellphone.main = true;
                    newCellphone.phoneTypeID = 1;
                    db.tblPhones.AddObject(newCellphone);
                }

            }
            if (email.Trim() != "" && email != null)
            {
                if (emailLead == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    tblLeadEmails newEmail = new tblLeadEmails();
                    newEmail.email = filteredEmail;
                    newEmail.main = true;
                    newEmail.leadID = leadID;
                    db.tblLeadEmails.AddObject(newEmail);
                }
            }


            if (membership != "no")
            {
                var dataMembership = System.Web.Helpers.Json.Decode(membership);
                var code = Convert.ToInt32(dataMembership.code);
                var membershipID = Convert.ToInt32(dataMembership.membershipID);
                var password = dataMembership.password;
                newLead.leadTypeID = 58;
                var activationid = activateMembership(name, lastname, phone, cellphone, email, membershipID, location, date);

                GetEmail(name, lastname, 229, email, password);
                newsurvey.membershipCardID = activationid;
                db.SaveChanges();
                return "ok";
            }
            else
            {
                if (membershipToLink != 0)
                {
                    newsurvey.membershipCardID = membershipToLink;
                    newLead.leadTypeID = 59;

                }
                Guid transactionID = Guid.NewGuid();
                if (locationID != 347 && email != null && email.Trim() != "" && Regex.IsMatch(email, ("([a-z]+)([_.a-z0-9]*)([a-z0-9]+)(@)([a-z]+)([.a-z]+)([a-z]+)"))/*&& !Regex.IsMatch(email.Trim(), "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)(\\.[A-Za-z]{2,})$")*/)
                {
                    var emalPreview = sendWelcomeEmailPoll(name, lastname, 247, email, transactionID);
                    tblEmailNotificationLogs query = new tblEmailNotificationLogs();
                    query.leadID = leadID;
                    query.trackingID = transactionID;
                    query.subject = "Welcome letter by Poll";
                    query.emailPreviewJson = JsonConvert.SerializeObject(emalPreview);
                    query.sentByUserID = session.UserID;
                    query.dateSent = DateTime.Now;
                    query.emailNotificationID = 247;
                    query.trackingID = transactionID;
                    db.tblEmailNotificationLogs.AddObject(query);
                    /*if (leadIDExists == new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        db.tblLeads.AddObject(newLead);
                    }*/
                }
                if (leadIDExists == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    db.tblLeads.AddObject(newLead);
                }
                db.SaveChanges();
                field.leadID = leadID;
                db.SaveChanges();
                return newsurvey.FieldGroupsAnswersID;
            }



        }

        /******************************************************************************************************/
        public object GetAmbassadors()
        {

            var ambassadors = (from userTerminals in db.tblUsers_Terminals
                               join users in db.tblUserProfiles on userTerminals.userID equals users.userID
                               join job in db.tblUsers_JobPositions on users.userID equals job.userID
                               join terminals in db.tblTerminals on userTerminals.terminalID equals terminals.terminalID
                               join pos in db.tblJobPositions on job.jobPositionID equals pos.jobPositionID
                               join m in db.aspnet_Membership on users.userID equals m.UserId
                               where userTerminals.terminalID == 62 && ((pos.jobPositionID == 1084 && job.toDate == null)
                               || (pos.jobPositionID == 1086 && job.toDate == null)) && m.IsLockedOut == false
                               select new
                               {
                                   value = users.userID,
                                   label = users.firstName + " " + users.lastName
                               }).ToList();


            return ambassadors;
        }
        /******************************************************************************************************/
        public object GetSurveysSaved(DateTime fromDate, DateTime toDate, string folio, string locationID)
        {
            Guid ambassador = Guid.Parse("469F240F-3410-4E62-B621-F3FB087633F9");
            Guid lead_ambassador = Guid.Parse("325E12C8-AA81-4DE6-95FB-5A9BB9607D9C");
            ;
            Int64 location = 0;
            if (locationID != null)
            {
                location = Convert.ToInt64(locationID);
            }

            if (session.RoleID == ambassador)
            {
                var surveys = (from fg in db.tblFieldGroupsAnswers
                               join users in db.tblUserProfiles on fg.savedByUserID equals users.userID
                               join fields in db.tblFieldGroups on fg.fieldGroupsID equals fields.fieldGroupID

                               where fg.terminalID == 62 && fg.dateSaved <= toDate
                                          && fg.dateSaved >= fromDate
                               where users.userID == session.UserID && (folio != null ? fg.folio == folio : fg.FieldGroupsAnswersID > 0)
                               where location != 0 ? fg.locationID == location : (fg.locationID == null || fg.locationID > 0)
                               orderby fg.dateSaved
                               select new
                               {
                                   fieldgroupid = fg.fieldGroupsID,
                                   folio = fg.folio,
                                   name = fields.fieldGroup,
                                   id = fg.FieldGroupsAnswersID,
                                   dateSaved = fg.dateSaved,
                                   savedBy = users.firstName + " " + users.lastName,
                                   culture = fg.culture,
                                   contactInfo = fg.contactInfo,
                                   membershipID = fg.membershipCardID,
                                   locationID = fg.locationID,

                               }
                           ).ToList();
                return surveys;
            }
            else if (session.RoleID == lead_ambassador)
            {
                var ambassadores = (from a in db.tblSupervisors_Agents
                                    where a.supervisorUserID == session.UserID
                                    select a
                                    ).ToList();
                var amb_list = ambassadores.Select(m => (Guid?)m.agentUserID).ToArray();

                var surveys = (from fg in db.tblFieldGroupsAnswers
                               join users in db.tblUserProfiles on fg.savedByUserID equals users.userID
                               join fields in db.tblFieldGroups on fg.fieldGroupsID equals fields.fieldGroupID

                               where fg.terminalID == 62 && fg.dateSaved <= toDate
                                          && fg.dateSaved >= fromDate
                               where amb_list.Contains(users.userID) || users.userID == session.UserID
                               where folio != null ? fg.folio == folio : fg.FieldGroupsAnswersID > 0
                               where location != 0 ? fg.locationID == location : (fg.locationID == null || fg.locationID > 0)
                               orderby fg.dateSaved
                               select new
                               {
                                   fieldgroupid = fg.fieldGroupsID,
                                   folio = fg.folio,
                                   name = fields.fieldGroup,
                                   id = fg.FieldGroupsAnswersID,
                                   dateSaved = fg.dateSaved,
                                   savedBy = users.firstName + " " + users.lastName,
                                   culture = fg.culture,
                                   contactInfo = fg.contactInfo,
                                   membershipID = fg.membershipCardID,
                                   locationID = fg.locationID,


                               }
                           ).ToList();
                return surveys;
            }
            else
            {

                var surveys = (from fg in db.tblFieldGroupsAnswers
                               join users in db.tblUserProfiles on fg.savedByUserID equals users.userID
                               join fields in db.tblFieldGroups on fg.fieldGroupsID equals fields.fieldGroupID
                               join l in db.tblLocations on fg.locationID equals l.locationID into loca
                               // join lead in db.tblLeads on fg.leadID equals lead.leadID
                               from lo in loca.DefaultIfEmpty()
                               where fg.terminalID == 62 && fg.dateSaved <= toDate
                                          && fg.dateSaved >= fromDate
                               where folio != null ? fg.folio == folio : fg.FieldGroupsAnswersID > 0
                               where location != 0 ? fg.locationID == location : (fg.locationID == null || fg.locationID > 0)
                               orderby fg.dateSaved
                               select new
                               {
                                   fieldgroupid = fg.fieldGroupsID,
                                   folio = fg.folio,
                                   name = fields.fieldGroup,
                                   id = fg.FieldGroupsAnswersID,
                                   dateSaved = fg.dateSaved,
                                   savedBy = users.firstName + " " + users.lastName,
                                   culture = fg.culture,
                                   contactInfo = fg.contactInfo,
                                   membershipID = fg.membershipCardID,
                                   locationID = (int?)lo.locationID,
                                   location = lo.location,
                                   //firstName = lead.firstName,
                                   //lastName = lead.lastName,
                                   //phone = phone.phone
                               }
                            ).ToList();
                return surveys;

                //polls por fecha
                /*  DateTime dFrom = Convert.ToDateTime("2020-01-01");
                  DateTime dTo = Convert.ToDateTime("2020-01-31");
                  List<leadspolls> newLead = new List<leadspolls>();
                  List<WrongLeads> badleads = new List<WrongLeads>();

          var polls = (from p in db.tblFieldGroupsAnswers
                               where p.dateSaved >= dFrom && p.dateSaved <= dTo
                               select p
                               ).ToList();

                  foreach (var item in polls)
                  {
                      var lead = (from l in db.tblLeads
                                      where l.leadID == item.leadID
                                      select l).FirstOrDefault();
                      WrongLeads badLead = new WrongLeads();
                      string leadName = "";
                      string leadLastName = "";
                      bool error = false;

                      if (lead != null)
                      {
                           leadName = lead.firstName;
                           leadLastName = lead.lastName;
                      }


                      var ans = System.Web.Helpers.Json.Decode(item.answers);
                      leadspolls newL = new leadspolls();
                      foreach (var item2 in ans)                      
                      {
                          var name = "";
                          var phone = "";
                          var lastname = "";

                          if (item2.fieldid == 1151 || item2.fieldid == 1166)
                          {
                              name = item2.answer;
                              newL.firstName = name;
                              if (name != leadName)
                              {
                                  error = true;
                              }
                              if (item2.answer == "Virgilio")
                              {
                                  var a = 1;
                              }
                          }
                          if (item2.fieldid == 1194 || item2.fieldid == 1167)
                          {
                              lastname = item2.answer;
                              if (lastname != leadLastName)
                              {
                                  error = true;
                              }
                              if (item2.answer == "Fuhrmann")
                              {
                                  var a = 1;
                              }
                              newL.lastName = lastname;
                          }

                          if (item2.fieldid == 1195 || item2.fieldid == 1168)
                          {
                              newL.occupation = item2.answer;
                          }
                          if (item2.fieldid == 1196 || item2.fieldid == 1169)
                          {
                              newL.age = item2.answer;
                          }
                          if (item2.fieldid == 1153 || item2.fieldid == 1172)
                          {
                              if (item2.answer == "mefdad@gmail.com")
                              {
                                  var a = 23;
                              }
                              newL.email = item2.answer;
                          }
                          if (item2.fieldid == 1152 || item2.fieldid == 1199)
                          {
                              newL.city = item2.answer;
                          }
                          if (item2.fieldid == 1154 || item2.fieldid == 1170)
                          {
                              if (item2.answer == "2818047704")
                              {
                                  var f = 12;
                              }
                              phone = item2.answer;
                              newL.Phone = phone;
                          }
                          if (item2.fieldid == 1197 || item2.fieldid == 1171)
                          {
                              if (item2.answer == "2818047704")
                              {
                                  var f = 12;
                              }
                              newL.cellphone = item2.answer;
                          }
                          if (item2.fieldid == 1159 || item2.fieldid == 1173)
                          {
                              newL.q1 = item2.answer;
                          }
                          if (item2.fieldid == 1160 || item2.fieldid == 1174)
                          {
                              newL.q2 = item2.answer;
                          }
                          if (item2.fieldid == 1161 || item2.fieldid == 1175)
                          {
                              newL.q3 = item2.answer;
                          }
                          if (item2.fieldid == 1162 || item2.fieldid == 1176)
                          {
                              newL.q4 = item2.answer;
                          }
                          if (item2.fieldid == 1163 || item2.fieldid == 1177)
                          {
                              newL.q5 = item2.answer;
                          }
                          if (item2.fieldid == 1164 || item2.fieldid == 1178)
                          {
                              newL.q6 = item2.answer;
                          }
                      }

                      newL.folio = item.folio;
                      newL.date = item.dateSaved;
                      newL.locationID = item.locationID;
                      newLead.Add(newL);


                  }

                  var all = (from a in newLead
                             join lo in db.tblLocations on a.locationID equals lo.locationID into locat
                             from locations in locat.DefaultIfEmpty()
                             orderby a.date ascending
                             select new
                             {
                                 firstName = a.firstName,
                                 lastName = a.lastName,
                                 guest = a.firstName + " " + a.lastName,
                                 occupation = a.occupation,
                                 cellphone = a.cellphone,
                                 age = a.age,
                                 city = a.city,
                                 email = a.email,
                                 dateSaved = a.date,
                                 folio = a.folio,
                                 phone = a.Phone,
                                 q1 = a.q1,
                                 q2 = a.q2,
                                 q3 = a.q3,
                                 q4 = a.q4,
                                 q5 = a.q5,
                                 q6 = a.q6,
                                 location =  (string) locations?.location
                             }
                             );
                  dynamic bads = all;
                  return bads;
                 */
            }

        }
        public class WrongLeads
        {

            public int? folio { get; set; }
            public string LeadFromPoll { get; set; }
            public string LeadFromTblleads { get; set; }

        }
        public class leadspolls : List<lead_survey>
        {
            public DateTime? date { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public int? locationID { get; set; }
            public string q1 { get; set; }
            public string q2 { get; set; }
            public string q3 { get; set; }
            public string q4 { get; set; }
            public string q5 { get; set; }
            public string q6 { get; set; }
            public string occupation { get; set; }
            public string age { get; set; }
            public string email { get; set; }
            public string city { get; set; }
            public string cellphone { get; set; }

            public string Phone { get; set; }

            public int? folio { get; set; }


        }
        public class lead_survey : List<lead_survey>
        {
            public DateTime date { get; set; }
            public string location { get; set; }
            public string guest { get; set; }
            public int folio { get; set; }
            public string homePhone { get; set; }
            public string mobile { get; set; }
            public string email { get; set; }
            public string occupation { get; set; }
            public string age { get; set; }
            public string state { get; set; }
            public string city { get; set; }
            public string q1 { get; set; }
            public string q2 { get; set; }
            public string q3 { get; set; }
            public string q4 { get; set; }
            public string q5 { get; set; }
            public string q6 { get; set; }
            public string q7 { get; set; }
        }


        public object GetSurveyToEdit(Int64 fieldGroupID, Int64 answerID, int membershipID)
        {
            var template = getTemplateSurvey(fieldGroupID);



            if (membershipID != 0)
            {
                var answer = (from ans in db.tblFieldGroupsAnswers
                              join fields in db.tblFieldGroups on ans.fieldGroupsID equals fields.fieldGroupID
                              join users in db.tblUserProfiles on ans.savedByUserID equals users.userID
                              join mem in db.tblMembershipSales on ans.membershipCardID equals mem.membershipSalesID
                              join card in db.tblMembershipCards on mem.membershipCardID equals card.MembershipCardID
                              join l in db.tblLocations on mem.locationID equals l.locationID
                              join ambassador in db.tblUserProfiles on card.userID equals ambassador.userID
                              join lo in db.tblLocations on ans.locationID equals lo.locationID

                              where ans.FieldGroupsAnswersID == answerID
                              select new
                              {
                                  id = ans.FieldGroupsAnswersID,
                                  answer = ans.answers,
                                  culture = ans.culture,
                                  folio = ans.folio,
                                  dateSaved = ans.dateSaved,
                                  contactInfo = ans.contactInfo,
                                  fieldgroupid = ans.fieldGroupsID,
                                  surveyname = fields.fieldGroup,
                                  description = fields.description,
                                  ambassadorid = ans.savedByUserID,
                                  ambassadorName = users.firstName + " " + users.lastName,
                                  cardCode = card.Code,
                                  cardID = card.MembershipCardID,
                                  ambassador = ambassador.firstName + " " + ambassador.lastName,
                                  location = l.location,
                                  locaitonID = l.locationID,
                                  isProgress = mem.inProgress,
                                  locationPoll = lo.location,
                                  locationIDPoll = lo.locationID,
                                  departureDate = ans.DepartureDate,
                                  picture = ans.Picture,

                              }
                            ).FirstOrDefault();
                if (answer == null)
                {
                    var answer2 = (from ans in db.tblFieldGroupsAnswers
                                   join fields in db.tblFieldGroups on ans.fieldGroupsID equals fields.fieldGroupID
                                   join users in db.tblUserProfiles on ans.savedByUserID equals users.userID
                                   join mem in db.tblMembershipSales on ans.membershipCardID equals mem.membershipSalesID
                                   join card in db.tblMembershipCards on mem.membershipCardID equals card.MembershipCardID
                                   join l in db.tblLocations on mem.locationID equals l.locationID
                                   join ambassador in db.tblUserProfiles on card.userID equals ambassador.userID

                                   where ans.FieldGroupsAnswersID == answerID
                                   select new
                                   {
                                       id = ans.FieldGroupsAnswersID,
                                       answer = ans.answers,
                                       culture = ans.culture,
                                       folio = ans.folio,
                                       dateSaved = ans.dateSaved,
                                       contactInfo = ans.contactInfo,
                                       fieldgroupid = ans.fieldGroupsID,
                                       surveyname = fields.fieldGroup,
                                       description = fields.description,
                                       ambassadorid = ans.savedByUserID,
                                       ambassadorName = users.firstName + " " + users.lastName,
                                       cardCode = card.Code,
                                       cardID = card.MembershipCardID,
                                       ambassador = ambassador.firstName + " " + ambassador.lastName,
                                       location = l.location,
                                       locaitonID = l.locationID,
                                       isProgress = mem.inProgress,
                                       departureDate = ans.DepartureDate,
                                       picture = ans.Picture
                                   }
                            ).FirstOrDefault();
                    object[] data2 = new[]
                    {
                new object[] {template},
                new object[] {answer2},
                 };

                    return data2;

                }
                object[] data = new[]
                {
                new object[] {template},
                new object[] {answer},
                 };

                return data;
            }
            else
            {
                var answer = (from ans in db.tblFieldGroupsAnswers
                              join fields in db.tblFieldGroups on ans.fieldGroupsID equals fields.fieldGroupID
                              join users in db.tblUserProfiles on ans.savedByUserID equals users.userID
                              join l in db.tblLocations on ans.locationID equals l.locationID

                              where ans.FieldGroupsAnswersID == answerID
                              select new
                              {
                                  id = ans.FieldGroupsAnswersID,
                                  answer = ans.answers,
                                  culture = ans.culture,
                                  folio = ans.folio,
                                  dateSaved = ans.dateSaved,
                                  contactInfo = ans.contactInfo,
                                  fieldgroupid = ans.fieldGroupsID,
                                  surveyname = fields.fieldGroup,
                                  description = fields.description,
                                  ambassadorid = ans.savedByUserID,
                                  ambassadorName = users.firstName + " " + users.lastName,
                                  cardCode = 0,
                                  locationPoll = l.location,
                                  locationIDPoll = l.locationID,
                                  departureDate = ans.DepartureDate,
                                  picture = ans.Picture


                              }
                            ).FirstOrDefault();
                if (answer == null)
                {
                    var answer2 = (from ans in db.tblFieldGroupsAnswers
                                   join fields in db.tblFieldGroups on ans.fieldGroupsID equals fields.fieldGroupID
                                   join users in db.tblUserProfiles on ans.savedByUserID equals users.userID

                                   where ans.FieldGroupsAnswersID == answerID
                                   select new
                                   {
                                       id = ans.FieldGroupsAnswersID,
                                       answer = ans.answers,
                                       culture = ans.culture,
                                       folio = ans.folio,
                                       dateSaved = ans.dateSaved,
                                       contactInfo = ans.contactInfo,
                                       fieldgroupid = ans.fieldGroupsID,
                                       surveyname = fields.fieldGroup,
                                       description = fields.description,
                                       ambassadorid = ans.savedByUserID,
                                       ambassadorName = users.firstName + " " + users.lastName,
                                       cardCode = 0,
                                       departureDate = ans.DepartureDate,
                                       picture = ans.Picture


                                   }
                                                ).FirstOrDefault();
                    object[] data2 = new[]
                {
                new object[] {template},
                new object[] {answer2},
                 };

                    return data2;
                }

                object[] data = new[]
                {
                new object[] {template},
                new object[] {answer},
                 };

                return data;
            }




        }

        public object UpdateSurvey(int fieldGroupID, string data, Guid ambassadorID, Boolean contactInfo, string culture,
            Int64 answerID, DateTime date, string membership, int location, int membershipToLink, int locationID, DateTime departureDate)
        {
            var survey = System.Web.Helpers.Json.Decode(data);

            tblFieldGroupsAnswers newsurvey = new tblFieldGroupsAnswers();

            var folio = "";
            var name = "";
            var lastname = "";
            var phone = "";
            var cellphone = "";

            var email = "";

            foreach (var item in survey)
            {
                if (item.fieldid == 1198 || item.fieldid == 1200)
                {
                    folio = item.answer;
                }
                if (item.fieldid == 1151 || item.fieldid == 1166)
                {
                    name = item.answer;
                }
                if (item.fieldid == 1194 || item.fieldid == 1167)
                {
                    lastname = item.answer;
                }
                if (item.fieldid == 1154 || item.fieldid == 1170)
                {
                    phone = item.answer;
                }
                if (item.fieldid == 1197 || item.fieldid == 1171)
                {
                    cellphone = item.answer;
                }
                if (item.fieldid == 1153 || item.fieldid == 1172)
                {
                    email = item.answer;
                }
            }

            var editsurvey = (from s in db.tblFieldGroupsAnswers
                              where s.FieldGroupsAnswersID == answerID
                              select s
                              ).FirstOrDefault();

            editsurvey.answers = data;
            editsurvey.folio = folio;
            editsurvey.dateSaved = date;
            editsurvey.savedByUserID = ambassadorID;
            editsurvey.terminalID = 62;
            editsurvey.capturedByUserID = session.UserID;
            editsurvey.culture = culture;
            editsurvey.contactInfo = contactInfo;
            editsurvey.locationID = locationID;
            editsurvey.DepartureDate = departureDate;




            if (membership != "no")
            {
                var dataMembership = System.Web.Helpers.Json.Decode(membership);
                var code = Convert.ToInt32(dataMembership.code);
                var membershipID = Convert.ToInt32(dataMembership.membershipID);
                var password = dataMembership.password;
                GetEmail(name, lastname, 229, email, password);
                var activationid = activateMembership(name, lastname, phone, cellphone, email, membershipID, location, date);

                editsurvey.membershipCardID = activationid;
                db.SaveChanges();
                return "ok";

            }
            else
            {
                if (membershipToLink != 0)
                {
                    editsurvey.membershipCardID = membershipToLink;
                    var lead = (from l in db.tblLeads
                                where l.leadID == newsurvey.leadID
                                select l
                                ).FirstOrDefault();
                    if (lead != null)
                    {
                        lead.leadTypeID = 59;
                    }

                }
                db.SaveChanges();
                return "ok";
            }

        }
        public object deletePoll(Int64 id)
        {
            var survey = (from s in db.tblFieldGroupsAnswers
                          where s.FieldGroupsAnswersID == id
                          select s
                              ).FirstOrDefault();

            db.DeleteObject(survey);
            db.SaveChanges();
            return "ok";
        }
        public object GetProviders(Int64 terminalid)
        {
            var providers = (from s in db.tblProviders
                             where s.terminalID == terminalid
                             join d in db.tblDestinations on s.destinationID equals d.destinationID
                             select new
                             {
                                 providerID = s.providerID,
                                 name = s.comercialName,
                                 dateSaved = s.dateSaved,
                                 destinationid = s.destinationID,
                                 destination = d.destination,

                             }
                              ).ToList();


            return providers;
        }
        public object getEmails()
        {
            var polls = (from p in db.tblFieldGroupsAnswers
                         select p.answers
                         ).ToList();

            var folio = "";
            foreach (var item in polls)
            {
                var survey = System.Web.Helpers.Json.Decode(item);

                foreach (var item2 in survey)
                {
                    if (item2.fieldid == 1153 || item2.fieldid == 1171)
                    {

                        if (item2.answer == "anagis24@gmail.com")
                        {
                            folio = "si";
                        }


                    }
                }
            }

            return folio;
        }
        public object GetInfoForProviders()
        {
            var destinations = (from s in db.tblDestinations
                                select new
                                {
                                    value = s.destinationID,
                                    label = s.destination
                                }
                              ).ToList();
            var providerType = (from s in db.tblProviderTypes
                                select new
                                {
                                    value = s.providerTypeID,
                                    label = s.providerType
                                }
                              ).ToList();
            var currencies = (from s in db.tblCurrencies
                              select new
                              {
                                  value = s.currencyID,
                                  label = s.currency
                              }
                              ).ToList();
            object[] data = new[]
                 {
                new object[] {destinations},
                new object[] {providerType},
                new object[] {currencies},

                 };

            return data;

        }
        public object activateMembership(string name, string lastname, string phone, string cellphone,
    string email, int code, int location, DateTime date)
        {
            Guid id = Guid.NewGuid();
            tblLeads newlead = new tblLeads();


            newlead.leadID = id;
            newlead.initialTerminalID = 62;
            newlead.terminalID = 62;
            newlead.firstName = name;
            newlead.lastName = lastname;
            newlead.inputByUserID = session.UserID;
            newlead.inputDateTime = DateTime.Today;
            newlead.leadTypeID = 59;
            newlead.leadSourceID = 55;
            newlead.inputMethodID = 8;
            newlead.leadStatusID = 2;
            newlead.isTest = false;
            newlead.deleted = false;
            newlead.leadSourceChannelID = 11;
            newlead.interestedInDestinationID = 1;
            db.tblLeads.AddObject(newlead);

            if (phone != null && phone != "")
            {
                tblPhones phoneToAdd = new tblPhones();
                phoneToAdd.phone = Convert.ToString(phone);
                phoneToAdd.phoneTypeID = 1;
                phoneToAdd.doNotCall = false;
                phoneToAdd.leadID = newlead.leadID;
                phoneToAdd.main = true;

                db.tblPhones.AddObject(phoneToAdd);

            }
            if (cellphone != null && cellphone != "")
            {
                tblPhones phoneToAdd = new tblPhones();
                phoneToAdd.phone = Convert.ToString(cellphone);
                phoneToAdd.phoneTypeID = 1;
                phoneToAdd.doNotCall = false;
                phoneToAdd.leadID = id;
                phoneToAdd.main = true;

                db.tblPhones.AddObject(phoneToAdd);
            }
            tblLeadEmails emails = (new tblLeadEmails
            {
                email = email,
                leadID = newlead.leadID,
                main = true,
            });
            db.tblLeadEmails.AddObject(emails);

            tblMembershipSales newcard = (new tblMembershipSales
            {
                membershipCardID = code,
                leadID = id,
                locationID = location,
                activationDate = DateTime.Today,
                dueDate = DateTime.Today.AddDays(4),
                dateSaved = date,
                savedUserBy = session.UserID,
                payment = "FREE TRIAL",
                price = 0,
                inProgress = true,
                activatedByPoll = true,
            });
            db.tblMembershipSales.AddObject(newcard);

            var update = (from cards in db.tblMembershipCards
                          where cards.MembershipCardID == code
                          select cards
                          ).FirstOrDefault();
            update.Status = "Active";
            int codigo = Convert.ToInt32(update.Code);
            var x = (from users in db.aspnet_Membership
                     where users.UserId == update.userID
                     select users
                                   ).FirstOrDefault();

            string newpass = string.Empty;
            string epass = Convert.ToString(codigo);
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(epass);
            newpass = Convert.ToBase64String(encryted);

            tblSimpleAuthentication user = new tblSimpleAuthentication();
            user.password = newpass;
            user.username = email;
            user.userid = id;
            user.terminalID = 62;
            user.userType = "member";
            db.tblSimpleAuthentication.AddObject(user);




            //  var emailtosend = email;
            db.SaveChanges();

            /*  var emailid = 188;
              GetEmail(emailid, codigo, name, lastname, activation, duedate, payment, price, email, emailtosend, password, paidmsg);
              emailid = 189;
              emailtosend = x.Email;
              GetEmail(emailid, codigo, name, lastname, activation, duedate, payment, price, email, emailtosend, password, paidmsg);
              */
            return newcard.membershipSalesID;
        }
        public static System.Net.Mail.MailMessage GetEmail(string name, string lastname, int emailid, string emailtosend,
            string password)
        {
            ePlatEntities db = new ePlatEntities();
            var emailnotifications = (from emails in db.tblEmailNotifications
                                      where emails.emailNotificationID == emailid
                                      select emails
                         ).FirstOrDefault();

            var mail = (from emails in db.tblEmails
                        where emails.emailID == emailnotifications.emailID
                        select emails
                         ).FirstOrDefault();


            System.Net.Mail.MailMessage emailObj = null;
            emailObj = new System.Net.Mail.MailMessage();
            emailObj.From = new System.Net.Mail.MailAddress(mail.sender, mail.alias);
            emailObj.Subject = mail.subject;
            emailObj.Body = mail.content_.Replace("$name", name).Replace("$email", emailtosend).Replace("$password", password);


            emailObj.IsBodyHtml = true;
            emailObj.Priority = System.Net.Mail.MailPriority.Normal;
            emailObj.Bcc.Add("arturorb91@gmail.com");
            emailObj.BodyEncoding = UTF8Encoding.UTF8;
            emailObj.Bcc.Add(emailtosend);

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.villagroup.com";
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("info@sensesofmexico.com", "In43fo19@");
            smtp.Credentials = credentials;
            smtp.Port = 2525;
            smtp.DeliveryFormat = System.Net.Mail.SmtpDeliveryFormat.International;
            smtp.Send(emailObj);
            return emailObj;
        }
        public static System.Net.Mail.MailMessage sendWelcomeEmailPoll(string name, string lastname,
            int emailid, string emailtosend, Guid transactionID)
        {
            ePlatEntities db = new ePlatEntities();
            var emailnotifications = (from emails in db.tblEmailNotifications
                                      where emails.emailNotificationID == emailid
                                      select emails
                         ).FirstOrDefault();

            var mail = (from emails in db.tblEmails
                        where emails.emailID == emailnotifications.emailID
                        select emails
                         ).FirstOrDefault();

            if (emailtosend.Trim() != "")
            {

                System.Net.Mail.MailMessage emailObj = null;
                emailObj = new System.Net.Mail.MailMessage();
                emailObj.From = new System.Net.Mail.MailAddress("concierge@sensesofmexico.com", "Concierge");
                emailObj.Subject = mail.subject;
                //emailObj.Body = mail.content_.Replace("$name", name);
                emailObj.Body = Utils.EmailNotifications.InsertTrackerEmailLog(mail.content_, transactionID.ToString()/*, idType, objectID*/);
                emailObj.IsBodyHtml = true;
                emailObj.Priority = System.Net.Mail.MailPriority.Normal;
                emailObj.Bcc.Add("arturo.renteria@villagroup.com");
                //emailObj.BodyEncoding = UTF8Encoding.UTF8;
                emailObj.Bcc.Add(emailtosend);
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = "smtp.villagroup.com";
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("concierge1@sensesofmexico.com", "@Conc1erge1");
                smtp.Credentials = credentials;
                smtp.Port = 2525;
                //smtp.DeliveryFormat = System.Net.Mail.SmtpDeliveryFormat.International;
                smtp.Send(emailObj);
                //email sin  tracking 
                System.Net.Mail.MailMessage emailObjWithOutTracking = null;
                emailObjWithOutTracking = new System.Net.Mail.MailMessage();
                emailObjWithOutTracking.From = new System.Net.Mail.MailAddress(mail.sender, mail.alias);
                emailObjWithOutTracking.Subject = mail.subject;
                //emailObj.Body = mail.content_.Replace("$name", name);
                emailObjWithOutTracking.Body = mail.content_;
                emailObjWithOutTracking.IsBodyHtml = true;
                emailObjWithOutTracking.Priority = System.Net.Mail.MailPriority.Normal;
                emailObjWithOutTracking.Bcc.Add("arturo.renteria@villagroup.com");
                //emailObj.BodyEncoding = UTF8Encoding.UTF8;
                emailObjWithOutTracking.Bcc.Add(emailtosend);
                return emailObjWithOutTracking;
            }
            else
            {
                System.Net.Mail.MailMessage emailObj = null;
                return emailObj;
            }

        }
        public object GetActivesCodes()
        {
            var codes = (from ambassadors in db.tblMembershipCards
                         join users in db.tblUserProfiles on ambassadors.userID equals users.userID
                         join mem in db.tblMembershipSales on ambassadors.MembershipCardID equals mem.membershipCardID
                         where ambassadors.Status == "Active"
                         orderby ambassadors.Code ascending
                         select new
                         {
                             name = users.firstName + " " + users.lastName,
                             userID = ambassadors.userID,
                             label = ambassadors.Code,
                             value = ambassadors.MembershipCardID,
                             membershipID = mem.membershipSalesID,

                         }).ToList();

            return codes;
        }
        public object getProvider(Int64 providerID)
        {
            var provider = (from p in db.tblProviders
                            join t in db.tblProviderTypes on p.providerTypeID equals t.providerTypeID
                            join c in db.tblCurrencies on p.contractCurrencyID equals c.currencyID
                            join d in db.tblDestinations on p.destinationID equals d.destinationID
                            where p.providerID == providerID
                            select new
                            {
                                name = p.comercialName,
                                shortName = p.shortName,
                                rfc = p.rfc,
                                phone1 = p.phone1,
                                phone2 = p.phone2,
                                ext1 = p.ext1,
                                ext2 = p.ext2,
                                contactName = p.contactName,
                                contactEmail = p.contactEmail,
                                providerTypeID = p.providerTypeID,
                                providerType = t.providerType,
                                contractCurrencyID = p.contractCurrencyID,
                                contractCurrency = c.currency,
                                isActive = p.isActive,
                                destinationID = p.destinationID,
                                destination = d.destination,
                                legalName = p.taxesName,
                                picture = p.image,
                                benefit = p.benefit,
                                logo = p.logo,

                            }
                            ).FirstOrDefault();
            var description = (from d in db.tblProvidersDescriptions
                               where d.providerID == providerID
                               select new
                               {
                                   fullDescription = d.fullDescription,
                                   shortDescription = d.shortDescription,
                               }
                               ).FirstOrDefault();

            object[] data = new[]
              {
                new object[] {provider},
                new object[] {description},

                 };

            return data;

        }
        public object updateProvider(Int64 providerID, string data)
        {
            var provider = System.Web.Helpers.Json.Decode(data);

            var getProvider = (from p in db.tblProviders
                               where p.providerID == providerID
                               select p
                               ).FirstOrDefault();
            var description = (from p in db.tblProvidersDescriptions
                               where p.providerID == providerID
                               select p
                               ).FirstOrDefault();
            var seoItem = (from p in db.tblSeoItems
                           where p.sysItemTypeID == 15 && p.itemID == providerID
                           select p
                               ).FirstOrDefault();
            if (getProvider != null)
            {
                getProvider.comercialName = provider.comercialName;
                getProvider.shortName = provider.shortName;
                getProvider.rfc = provider.rfc;
                getProvider.taxesName = provider.legalEntity;
                getProvider.phone1 = provider.phone1;
                getProvider.phone2 = provider.phone2;
                getProvider.ext1 = provider.ext1;
                getProvider.ext2 = provider.ext2;
                getProvider.modifiedByUserID = session.UserID;
                getProvider.lastDateModified = DateTime.Now;
                getProvider.contactName = provider.contactName;
                getProvider.contactEmail = provider.contactEmail;
                getProvider.destinationID = Convert.ToInt64(provider.destinationID);
                getProvider.providerTypeID = Convert.ToInt32(provider.typeID);
                getProvider.contractCurrencyID = Convert.ToInt32(provider.currencyID);
                getProvider.isActive = Convert.ToBoolean(provider.isActive);
                getProvider.benefit = provider.benefit;
            }

            if (description != null)
            {
                description.fullDescription = provider.fullDescription;
                description.shortDescription = provider.shortDescriptionFinal;
                description.culture = provider.culture;
                description.terminalID = Convert.ToInt64(provider.terminalID);
            }
            else
            {
                tblProvidersDescriptions newDescription = new tblProvidersDescriptions();
                newDescription.fullDescription = provider.fullDescription;
                newDescription.shortDescription = provider.shortDescriptionFinal;
                newDescription.culture = provider.culture;
                newDescription.providerID = providerID;
                newDescription.terminalID = Convert.ToInt64(provider.terminalID);
                db.tblProvidersDescriptions.AddObject(newDescription);
            }

            if (seoItem != null)
            {
                seoItem.title = provider.seo.title;
                seoItem.friendlyUrl = provider.seo.friendlyurl;
                seoItem.url = provider.seo.friendlyurl;
                seoItem.culture = provider.culture;
                seoItem.terminalID = Convert.ToInt64(provider.terminalID);
                seoItem.sysItemTypeID = Convert.ToInt64(provider.sysItemType);
                seoItem.itemID = providerID;
            }
            else
            {
                tblSeoItems newSeoItem = new tblSeoItems();
                newSeoItem.title = provider.seo.title;
                newSeoItem.friendlyUrl = provider.seo.friendlyurl;
                newSeoItem.url = provider.seo.friendlyurl;
                newSeoItem.culture = provider.culture;
                newSeoItem.index_ = false;
                newSeoItem.follow = false;
                newSeoItem.terminalID = Convert.ToInt64(provider.terminalID);
                newSeoItem.sysItemTypeID = Convert.ToInt64(provider.sysItemType);
                newSeoItem.itemID = providerID;
                db.tblSeoItems.AddObject(newSeoItem);
            }



            db.SaveChanges();
            return providerID;

        }
        public object saveProvider(string data)
        {
            var provider = System.Web.Helpers.Json.Decode(data);

            tblProviders newProvider = new tblProviders();

            newProvider.comercialName = provider.comercialName;
            newProvider.terminalID = Convert.ToInt64(provider.terminalID);
            newProvider.shortName = provider.shortName;
            newProvider.rfc = provider.rfc;
            newProvider.taxesName = provider.legalEntity;
            newProvider.phone1 = provider.phone1;
            newProvider.phone2 = provider.phone2;
            newProvider.ext1 = provider.ext1;
            newProvider.ext2 = provider.ext2;
            newProvider.contactName = provider.contactName;
            newProvider.contactEmail = provider.contactEmail;
            newProvider.destinationID = Convert.ToInt64(provider.destinationID);
            newProvider.providerTypeID = Convert.ToInt32(provider.typeID);
            newProvider.contractCurrencyID = Convert.ToInt32(provider.currencyID);
            newProvider.isActive = Convert.ToBoolean(provider.isActive);
            newProvider.dateSaved = DateTime.Now;
            newProvider.savedByUserID = session.UserID;
            newProvider.benefit = provider.benefit;
            db.tblProviders.AddObject(newProvider);
            db.SaveChanges();
            tblProvidersDescriptions newDescription = new tblProvidersDescriptions();
            newDescription.fullDescription = provider.fullDescription;
            newDescription.shortDescription = provider.shortDescriptionFinal;
            newDescription.culture = provider.culture;
            newDescription.providerID = newProvider.providerID;
            newDescription.terminalID = Convert.ToInt64(provider.terminalID);
            db.tblProvidersDescriptions.AddObject(newDescription);


            tblSeoItems newSeoItem = new tblSeoItems();
            newSeoItem.title = provider.seo.title;
            newSeoItem.friendlyUrl = provider.seo.friendlyurl;
            newSeoItem.url = provider.seo.friendlyurl;
            newSeoItem.culture = provider.culture;
            newSeoItem.index_ = false;
            newSeoItem.follow = false;
            newSeoItem.terminalID = Convert.ToInt64(provider.terminalID);
            newSeoItem.sysItemTypeID = Convert.ToInt64(provider.sysItemType);
            newSeoItem.itemID = newProvider.providerID;
            newSeoItem.itemID = provider.placeID;
            db.tblSeoItems.AddObject(newSeoItem);

            db.SaveChanges();
            return newProvider.providerID;

        }
        public object DeleteImageProvider(int id)
        {
            var provider = (from e in db.tblProviders
                            where e.providerID == id
                            select e
                          ).FirstOrDefault();
            provider.image = null;
            db.SaveChanges();

            return "ok";

        }
        public object DeleteLogoProvider(int id)
        {
            var provider = (from e in db.tblProviders
                            where e.providerID == id
                            select e
                          ).FirstOrDefault();
            provider.logo = null;
            db.SaveChanges();

            return "ok";

        }
        public object DeleteProvider(int id)
        {
            var provider = (from e in db.tblProviders
                            where e.providerID == id
                            select e
                          ).FirstOrDefault();
            if (provider != null)
            {
                db.tblProviders.DeleteObject(provider);
            }

            var description = (from e in db.tblProvidersDescriptions
                               where e.providerID == id
                               select e
                         ).FirstOrDefault();
            if (description != null)
            {
                db.tblProvidersDescriptions.DeleteObject(description);
            }
            db.SaveChanges();

            return "ok";

        }
        public object GetLocations()
        {
            var locations = (from l in db.tblLocations
                             where (l.destinationID == 1 || l.destinationID == 4) && l.terminalID == 62
                             select new
                             {
                                 label = l.location,
                                 value = l.locationID
                             }
                             ).ToList();

            return locations;

        }
        public object saveMembershipComment(int membershipSaleID, string comment, string bookingStatus, DateTime dueDate)
        {
            var membership = (from m in db.tblMembershipSales
                              where m.membershipSalesID == membershipSaleID
                              select m

                              ).FirstOrDefault();
            var booking = (from m in db.tblMembershipSales
                           join l in db.tblLeads on m.leadID equals l.leadID
                           where m.membershipSalesID == membershipSaleID
                           select l

                              ).FirstOrDefault();
            if (membership != null)
            {
                membership.membershipComment = comment;
                membership.dueDate = dueDate;
                db.SaveChanges();
            }
            if (booking != null && bookingStatus != null)
            {
                Int32 bookingStatusID = Convert.ToInt32(bookingStatus);
                booking.bookingStatusID = bookingStatusID;
                db.SaveChanges();

            }

            return "ok";
            
        }
        public object getBookingStatusOptions()
        {
            var booking = (from m in db.tblSysWorkGroups_BookingStatus
                           join b in db.tblBookingStatus on m.bookingStatusID equals b.bookingStatusID
                           where m.sysWorkGroupID == 34
                           select new
                           {
                               text = b.bookingStatus,
                               value = b.bookingStatusID
                           }
                              ).ToList();



            return booking;

        }
    }

}
