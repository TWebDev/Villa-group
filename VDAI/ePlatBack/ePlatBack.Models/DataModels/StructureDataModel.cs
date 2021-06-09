using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;

namespace ePlatBack.Models.DataModels
{
    public class StructureDataModel
    {
        public StructureViewModel getUsersStructure()
        {
            StructureViewModel structure = new StructureViewModel();
            structure.Users = new List<UserItem>();
            ePlatEntities db = new ePlatEntities();

            var UsersQ = from u in db.tblSupervisors_Agents
                         where u.supervisor_AgentID == null
                         select new
                         {
                             u.aspnet_Users1.UserId,
                             u.aspnet_Users1.tblUserProfiles.FirstOrDefault().firstName,
                             u.aspnet_Users1.tblUserProfiles.FirstOrDefault().lastName,
                             u.aspnet_Users1.tblUserProfiles.FirstOrDefault().tblDestinations.destination,
                             u.aspnet_Users1.tblUsers_SysWorkGroups
                         };

            foreach (var user in UsersQ)
            {
                UserItem useritem = new UserItem();
                useritem.UserID = user.UserId;
                useritem.FirstName = user.firstName;
                useritem.LastName = user.lastName;
                useritem.Destination = user.destination;
                foreach (var wg in user.tblUsers_SysWorkGroups)
                {
                    if(useritem.WorkGroups != ""){
                        useritem.WorkGroups += ", ";
                    }
                    useritem.WorkGroups = wg.tblSysWorkGroups.sysWorkGroup;
                }
                structure.Users.Add(useritem);
            }

            return structure;
        }
    }
}
