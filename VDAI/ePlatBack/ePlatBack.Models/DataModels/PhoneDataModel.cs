
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using AsterNET;
using System.Diagnostics;

namespace ePlatBack.Models.DataModels
{
    class PhoneDataModel
    {
        ePlatEntities db = new ePlatEntities();

        /// <summary>
        /// Gets all phone types.
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetAllPhoneTypes()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();


                var phoneTypes = from t in db.tblPhoneTypes
                                select new { t.phoneTypeID, t.phoneType };

                foreach (var i in phoneTypes)
                {
                    list.Add(new SelectListItem() { Text = i.phoneType, Value = i.phoneTypeID.ToString() });
                }
            
            return list;
        }

        public static AttemptResponse DialLeadPhoneNumber(Guid leadID, int? phoneID)
        {
            UserSession session = new UserSession();
            AttemptResponse response = new AttemptResponse();
            var db = new ePlatEntities();
            AsterNET.Manager.ManagerConnection astMan = new AsterNET.Manager.ManagerConnection();
            try
            {
                var l = (from x in db.tblLeads
                         join p in db.tblPhones on x.leadID equals p.leadID
                         where x.leadID == leadID
                         && (phoneID == p.phoneID || phoneID == null)
                         select new
                         {
                             p.main,
                             p.phone,
                             p.phoneTypeID,
                             x.terminalID,
                             x.leadSourceID,
                             x.leadSourceChannelID,
                             x.countryID,
                             x.tblCountries.country,
                             x.tblCountries.countryCallingCode
                         }).FirstOrDefault(m => m.main == true);

//                var l = (from x in db.tblLeads
//                         where x.leadID == leadID
//                         select
//new { x.tblPhones.OrderByDescending(o => o.main).FirstOrDefault(m => (phoneID == null || m.phoneID == phoneID)).phone, x.tblPhones.OrderByDescending(o => o.main).FirstOrDefault(m => (phoneID == null || m.phoneID == phoneID)).phoneTypeID, x.terminalID, x.leadSourceID, x.leadSourceChannelID, x.tblCountries.country, x.countryID, x.tblCountries.countryCallingCode }).Single();


                if (l.phone == null)
                {
                    throw new Exception("Lead has no phone number.");
                }
                else if (l.countryID == null)
                {
                    throw new Exception("Lead has no country specified.");
                }
                else if (l.countryCallingCode == null)
                {
                    throw new Exception(l.country + " has no country calling code specified.");
                }

                var u = (from U in db.aspnet_Users
                         join U_SWG in db.tblUsers_SysWorkGroups.DefaultIfEmpty() on U.UserId equals U_SWG.userID
                         join SWG in db.tblSysWorkGroups.DefaultIfEmpty() on U_SWG.sysWorkGroupID equals SWG.sysWorkGroupID
                         join SWG_S in db.tblSysWorkGroups_Servers.DefaultIfEmpty() on SWG.sysWorkGroupID equals SWG_S.sysWorkGroupID
                         join S in db.tblServers.DefaultIfEmpty() on SWG_S.serverID equals S.serverID
                         join UP in db.tblUserProfiles.DefaultIfEmpty() on U.UserId equals UP.userID
                         join T in db.tblSysWorkGroupsTeams.DefaultIfEmpty() on U_SWG.sysWorkGroupTeamID equals T.sysWorkGroupTeamID
                         where U.UserId == session.UserID && S.serverType == "Asterisk"
                         select new { S.serverID, S.name, S.privateIP, S.amiPort, S.username, S.password, UP.phoneEXT, U_SWG.sysWorkGroupTeamID, T.team }).First();


                if (u.phoneEXT == null)
                {
                    throw new Exception("Current user has no extension specified.");
                }
                else if (u.password == null || u.privateIP == null || u.username == null || u.amiPort == null)
                {
                    throw new Exception("Missing server configuration.");
                }
                else if (u.sysWorkGroupTeamID == null)
                {
                    throw new Exception("Team for user is not specified.");
                }


                var d = (
                        (from x in db.tblDialOutPrefixes where x.countryID == l.countryID && x.sysWorkGroupTeamID == u.sysWorkGroupTeamID && x.terminalID == l.terminalID && x.leadSourceID == l.leadSourceID && x.leadSourceChannelID == l.leadSourceChannelID select new { prefix = x.dialOutPrefix, weight = 1, x.leadSourceChannelID, x.leadSourceID })
                        .Union((from x in db.tblDialOutPrefixes where x.countryID == l.countryID && x.sysWorkGroupTeamID == u.sysWorkGroupTeamID && x.terminalID == l.terminalID && x.leadSourceID == l.leadSourceID select new { prefix = x.dialOutPrefix, weight = 2, x.leadSourceChannelID, x.leadSourceID }))
                        .Union((from x in db.tblDialOutPrefixes where x.countryID == l.countryID && x.sysWorkGroupTeamID == u.sysWorkGroupTeamID && x.terminalID == l.terminalID select new { prefix = x.dialOutPrefix, weight = 3, x.leadSourceChannelID, x.leadSourceID }))
                        .Union((from x in db.tblDialOutPrefixes where x.countryID == l.countryID && x.sysWorkGroupTeamID == u.sysWorkGroupTeamID select new { prefix = x.dialOutPrefix, weight = 4, x.leadSourceChannelID, x.leadSourceID }))
                     )
                    .OrderBy(o => o.weight).ThenByDescending(o => o.leadSourceID).ThenByDescending(o => o.leadSourceChannelID);


                if (!d.Any() || d.FirstOrDefault().prefix == "")
                {
                    throw new Exception("No dialout prefix found.");
                }


                //AsterNET.Manager.ManagerConnection astMan = new AsterNET.Manager.ManagerConnection();
                astMan.Hostname = u.privateIP;
                astMan.Port = (int)u.amiPort;
                astMan.Username = u.username;
                astMan.Password = u.password;
                // If Login() fails it throws : "Authentication Failed", so maybe we don't really need this condition.
                try
                {
                    astMan.Login();
                }
                catch
                {
                    var waitTime = 1000;
                    System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();

                    if(!client.ConnectAsync(u.privateIP, (int)u.amiPort).Wait(waitTime))
                    {
                        EventLog winLog = new EventLog();
                        winLog.Source = "srcEplat";
                        winLog.WriteEntry("Iniciar reconexión de VPN para "+ u.name, EventLogEntryType.Information, u.serverID);
                    }
                    else
                    {
                        //connection open, host active
                    }
                    try
                    {
                        client.Close();
                    }
                    catch { }
                    throw new Exception("There was an issue with the caller service. Please wait 5 seconds and try again.<br /> if problem persists, communicate with the System Administrator.");
                }

                var oA = new AsterNET.Manager.Action.OriginateAction();
                oA.Priority = "1";

                string exten = "";
                //null or empty previously validated.
                exten += d.FirstOrDefault().prefix;
                //null previously validated.
                exten += l.countryCallingCode.ToString();
                exten += l.phoneTypeID == 1 && l.countryID == 3 ? "1" : "";//add 1 for national cell phones

                exten += l.phone;

                oA.Exten = exten; // extension To connect To
                                  //oA.CallerId = l.phone; // see if this works fine for the agent to see what number is calling to, and see what the clients gets //TODO: TEST
                                  //oA.CallerId = u.phoneEXT;
                oA.CallerId = "ePlat";
                oA.Context = "from-internal"; // In context //fixed
                oA.Channel = "SIP/" + u.phoneEXT; // this Is the channel And dialed first!
                oA.Timeout = 5 * 1000; //Huge timeout just for testing but sill getting some timeouts, trygin with Async True
                oA.Async = true;
                oA.SetVariable("CALLERID(dnid)", exten);
                AsterNET.Manager.Response.ManagerResponse ManagerResponse = new AsterNET.Manager.Response.ManagerResponse();


                ManagerResponse = astMan.SendAction(oA);
                if (astMan.IsConnected())
                {
                    astMan.Logoff();
                }
                if (ManagerResponse.IsSuccess())
                {
                    response.Type = Attempt_ResponseTypes.Ok;
                    //response.ObjectID = ManagerResponse.ToString();
                    response.Message = ManagerResponse.Response;
                    response.ObjectID = ManagerResponse.ToString();
                }
                else
                {
                    response.Type = Attempt_ResponseTypes.Warning;
                    //response.ObjectID = ManagerResponse.ToString();
                    response.Message = ManagerResponse.Message;
                    response.ObjectID = ManagerResponse.ToString();
                }
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = ex.Message;
                //response.ObjectID = ex.Message;
                if (astMan.IsConnected())
                {
                    astMan.Logoff();
                }
            }

            return response;
        }
    }
}
