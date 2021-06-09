using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ePlatBack.Models.DataModels;
using System.Web.Mvc;
using ePlatBack.Models;


namespace ePlatBack.Controllers.membership
{
    public class cardsManagementController : Controller
    {
        
        ePlatEntities db = new ePlatEntities();
        public static UserSession session = new UserSession();

        public ActionResult Index()
        {
            
            return View();
        }

        //Obtener todos los registros de la tabla tblmembershipcards 
        public JsonResult getAmbassadorsRanges()
        {
            var terminalid = Convert.ToInt64(62);
            var nombres = (from ambassadors in db.tblMembershipCards
                           join users in db.tblUserProfiles on ambassadors.userID equals users.userID
                           where ambassadors.MembershipCardID > 1
                           select new
                           {
                               name = users.firstName + " " + users.lastName,
                               userID = ambassadors.userID,
                               Code = ambassadors.Code,
                               id = ambassadors.MembershipCardID,
                               date = ambassadors.dateSaved,
                               savedby = ambassadors.savedUserBy
                           }).ToList();

            
            return Json(nombres, JsonRequestBehavior.AllowGet);
        }

        //Obtener a todos los usuarios que tienen la terminar de senses of mexico
        public JsonResult getAmbassadors()
        {
            var terminalid = Convert.ToInt64(62);
            var nombres = (from userTerminals in db.tblUsers_Terminals
                           join users in db.tblUserProfiles on userTerminals.userID equals users.userID
                           join job in db.tblUsers_JobPositions on users.userID equals job.userID
                           join terminals in db.tblTerminals on userTerminals.terminalID equals terminals.terminalID
                           join pos in db.tblJobPositions on job.jobPositionID equals pos.jobPositionID
                           join m in db.aspnet_Membership on users.userID equals m.UserId

                           where userTerminals.terminalID == terminalid  && m.IsLockedOut == false
                           select new
                           {
                               label = users.firstName+" " + users.lastName,
                               value = users.userID,
                               
                           }).ToList();
            return Json(nombres, JsonRequestBehavior.AllowGet);
        }

        //Agregar las cards desde el componente newcard.vue
        [System.Web.Http.HttpPost]
        public JsonResult addCards(string status,  Guid ambassadorid , int initial, int final, string date)
        {
            var ambassador = Convert.ToString(ambassadorid);
            var id = session.UserID;


            var initialcode = Convert.ToInt32(initial);
            var finalcode = Convert.ToInt32(final);
            var codigo = initialcode;
            var today = Convert.ToDateTime(date);
            var codeexists = db.tblMembershipCards.Where(b => b.Code >= initialcode).Where(b => b.Code <= finalcode).Select(b => b.Code).FirstOrDefault();
            
            if ( codeexists == null)
            {
                for (int i = initialcode; i <= finalcode; i++)
                {
                    db.tblMembershipCards.AddObject(new tblMembershipCards
                    {
                        Code = i,
                        userID = ambassadorid,
                        Status = status.Trim(),
                        savedUserBy = id,
                        dateSaved = today,
                        
                    });
                   
                }
                
                db.SaveChanges();
                return Json("ok");
            }else
            {
                return Json("existe");
            }
            
        }
        //regresa las cards del rango que fue seleccionado en la tabla
        [System.Web.Http.HttpPost]
        public JsonResult getRangeSearch(int initial, int final)
        {
            var inicio = Convert.ToInt32(initial);
            var fin = Convert.ToInt32(final);
            
            var resultado = (from ambassadors in db.tblMembershipCards
                            join users in db.tblUserProfiles on ambassadors.userID equals users.userID
                             join savedby in db.tblUserProfiles on ambassadors.savedUserBy equals savedby.userID
                             where ambassadors.Code >= inicio
                           where ambassadors.Code <= fin
                           orderby ambassadors.Code
                             select new
                           {
                               name = users.firstName + " " + users.lastName,
                               userID = ambassadors.userID,
                               Code = ambassadors.Code,
                               id = ambassadors.MembershipCardID,
                               status = ambassadors.Status,
                               date = ambassadors.dateSaved,
                               savedby = savedby.firstName + " " + savedby.lastName,
                           }).ToList();
            


            return Json( resultado, JsonRequestBehavior.AllowGet);
        }
        //actualiza una sola card desde el componente carddetails.vue
        [System.Web.Http.HttpPost]
        public JsonResult updateSingleCard(int code, Guid ambassadorid)
        {
            var codigo = Convert.ToInt32(code);
            var update = db.tblMembershipCards.Where(b =>b.Code == codigo).FirstOrDefault();

            update.userID = ambassadorid;
            db.SaveChanges();



            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        //actualiza el rango de cards seleccionadas
        [System.Web.Http.HttpPost]
        public JsonResult updateRangeCard(int initial, int final, Guid ambassadorid)
        {
            var inicio = Convert.ToInt32(initial);
            var fin = Convert.ToInt32(final);
            var update = (from cards in db.tblMembershipCards
                          where cards.Code >= inicio
                          where cards.Code <= fin

                          select cards
                       );
            foreach (var item in update)
            {
                item.userID = ambassadorid;
            }
            db.SaveChanges();
            
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        //elimina el rango de cards seleccionadas solo si estan inactivas
        [System.Web.Http.HttpPost]
        public JsonResult deleteRangeCard(int initial, int final)
        {
            var initialcode = Convert.ToInt32(initial);
            var finalcode = Convert.ToInt32(final);


            var del = (  from cards in db.tblMembershipCards
                       where cards.Code >= initialcode
                       where cards.Code <= finalcode
                       
                       select  cards
                       );

            foreach (var item in del)
            {
                if (item.Status == "Inactive")
                {
                    db.tblMembershipCards.DeleteObject(item);
                }
              
            }
            db.SaveChanges();
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        //elimina una sola card desde el componente carddetails.vue
        [System.Web.Http.HttpPost]
        public JsonResult deleteCard(int code)
        {
            var codigo = Convert.ToInt32(code);


            var del = (from cards in db.tblMembershipCards
                       where cards.Code == codigo

                       select cards
                       );

            foreach (var item in del)
            {
                if (item.Status == "Inactive")
                {
                    db.tblMembershipCards.DeleteObject(item);
                }

            }
            db.SaveChanges();
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        //regresa la busqueda de un embajador seleccionado
        [System.Web.Http.HttpPost]
        public JsonResult getAmbassadorsSearch(Guid ambassadorid)
        {
            var nombres = (from ambassadors in db.tblMembershipCards
                           join users in db.tblUserProfiles on ambassadors.userID equals users.userID
                           where ambassadors.userID == ambassadorid
                           select new
                           {
                               name = users.firstName + " " + users.lastName,
                               userID = ambassadors.userID,
                               Code = ambassadors.Code,
                               id = ambassadors.MembershipCardID,
                               date = ambassadors.dateSaved,
                               savedby = ambassadors.savedUserBy
                           }).ToList();



            return Json(nombres, JsonRequestBehavior.AllowGet);
        }
        //regresa la busqueda ya sea por code, status, o status + embajador
        [System.Web.Http.HttpPost]
        public JsonResult getAmbassadorsSearchBy(Guid? ambassadorid, int? code, string status, int? search)
        {
         
            if (code.HasValue)
            {
                var nombres = (from ambassadors in db.tblMembershipCards
                               join users in db.tblUserProfiles on ambassadors.userID equals users.userID
                               join savedby in db.tblUserProfiles on ambassadors.savedUserBy equals savedby.userID
                               where (ambassadors.Code == code)
                               orderby ambassadors.Code
                               select new
                               {
                                   name = users.firstName + " " + users.lastName,
                                   userID = ambassadors.userID,
                                   Code = ambassadors.Code,
                                   id = ambassadors.MembershipCardID,
                                   status = ambassadors.Status,
                                   date = ambassadors.dateSaved,
                                   savedby = savedby.firstName + " " + savedby.lastName,
                               }).ToList();
                return Json(nombres, JsonRequestBehavior.AllowGet);
            }else if (status != "" && ambassadorid.HasValue) {
                var nombres = (from ambassadors in db.tblMembershipCards
                               join users in db.tblUserProfiles on ambassadors.userID equals users.userID
                               join savedby in db.tblUserProfiles on ambassadors.savedUserBy equals savedby.userID
                               where (ambassadors.Status == status)
                               where (ambassadors.userID == ambassadorid)
                               orderby ambassadors.Code
                               select new
                               {
                                   name = users.firstName + " " + users.lastName,
                                   userID = ambassadors.userID,
                                   Code = ambassadors.Code,
                                   id = ambassadors.MembershipCardID,
                                   status = ambassadors.Status,
                                   date = ambassadors.dateSaved,
                                   savedby = savedby.firstName + " " + savedby.lastName,
                               }).ToList();
                return Json(nombres, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var nombres = (from ambassadors in db.tblMembershipCards
                               join users in db.tblUserProfiles on ambassadors.userID equals users.userID
                               join savedby in db.tblUserProfiles on ambassadors.savedUserBy equals savedby.userID
                               where (ambassadors.Status == status)
                               orderby ambassadors.Code
                               select new
                               {
                                   name = users.firstName + " " + users.lastName,
                                   userID = ambassadors.userID,
                                   Code = ambassadors.Code,
                                   id = ambassadors.MembershipCardID,
                                   status = ambassadors.Status,
                                   date = ambassadors.dateSaved,
                                   savedby = savedby.firstName + " " + savedby.lastName,
                               }).ToList();
                return Json(nombres, JsonRequestBehavior.AllowGet);

            }
        }


    }
    
}

