using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;

namespace ePlatBack.Models.DataModels
{
    public class WikiFieldDataModel
    {
        public static UserSession session = new UserSession();
        public static AttemptResponse SaveWiki(WikiFieldViewModel.Post model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            
            if (model.WikiFieldID != 0)
            {
                try
                {
                    tblWikiFields wiki = db.tblWikiFields.Single(m => m.wikifieldID == model.WikiFieldID);
                    wiki.content_ = model.Content;
                    wiki.dateSaved = DateTime.Now;
                    wiki.culture = model.Culture;
                    wiki.sysComponentID = model.SysComponentID;
                    wiki.wikiType = model.Type;

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Wiki Updated";
                    response.ObjectID = wiki.wikifieldID;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Wiki NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                }
            }
            else
            {
                try
                {
                    tblWikiFields wiki = new tblWikiFields();
                    wiki.content_ = model.Content;
                    wiki.dateSaved = DateTime.Now;
                    wiki.savedByUserID = session.UserID;
                    wiki.culture = model.Culture;
                    wiki.sysComponentID = model.SysComponentID;
                    wiki.wikiType = model.Type;

                    db.tblWikiFields.AddObject(wiki);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Wiki Saved";
                    response.ObjectID = wiki.wikifieldID;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Wiki NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                }
            }
            return response;
        }

        public static WikiFieldViewModel GetWikiField(string sysComponent)
        {
            ePlatEntities db = new ePlatEntities();
            WikiFieldViewModel wikiModel = new WikiFieldViewModel();
            var sysComponentObject = from s in db.tblSysComponents
                                     where s.sysComponent == sysComponent
                                     select s;

            if (sysComponentObject.Count() > 0)
            {
                wikiModel.SysComponentID = sysComponentObject.FirstOrDefault().sysComponentID;
                wikiModel.FieldName = sysComponentObject.FirstOrDefault().description;
                List<WikiFieldViewModel.WikiCulture> wikiList = new List<WikiFieldViewModel.WikiCulture>();
                foreach (var culture in wikiModel.Cultures)
                {
                    WikiFieldViewModel.WikiCulture wikiCulture = new WikiFieldViewModel.WikiCulture();
                    wikiCulture.Culture = culture.CultureCode;
                    wikiCulture.HowTo = new List<WikiFieldViewModel.Post>();
                    wikiCulture.Glosary = new List<WikiFieldViewModel.Post>();
                    wikiList.Add(wikiCulture);
                }

                var wikis = from w in db.tblWikiFields
                            where w.tblSysComponents.sysComponent == sysComponent
                            select w;

                foreach (var wiki in wikis)
                {
                    WikiFieldViewModel.Post newPost = new WikiFieldViewModel.Post();
                    newPost.WikiFieldID = wiki.wikifieldID;
                    newPost.Type = (int)wiki.wikiType;
                    newPost.Content = wiki.content_;
                    tblUserProfiles up = db.tblUserProfiles.Single(u => u.userID == wiki.savedByUserID);
                    newPost.User = up.firstName + " " + up.lastName;
                    newPost.DateTime = wiki.dateSaved;
                    newPost.Culture = wiki.culture;
                    newPost.SysComponentID = wiki.sysComponentID;
                    newPost.Own = (session.UserID == wiki.savedByUserID ? true : false);

                    if (wiki.wikiType == 1)
                    {
                        wikiList.FirstOrDefault(x => x.Culture == wiki.culture).Glosary.Add(newPost);
                    }
                    else if (wiki.wikiType == 2)
                    {
                        wikiList.FirstOrDefault(x => x.Culture == wiki.culture).HowTo.Add(newPost);
                    }
                }

                wikiModel.Wikis = wikiList;
            }

            return wikiModel;
        }

        public static string GetFieldsWithContent(string fields)
        {
            string fieldsWithContent = "";
            if (fields != null)
            {
                string[] queryFields = fields.Split(',');
                ePlatEntities db = new ePlatEntities();

                var FieldsQuery = (from f in db.tblWikiFields
                                  where queryFields.Contains(f.tblSysComponents.sysComponent)
                                  select f.tblSysComponents.sysComponent).Distinct();

                foreach (var s in FieldsQuery)
                {
                    if (fieldsWithContent != "")
                    {
                        fieldsWithContent += ",";
                    }
                    fieldsWithContent += s.ToString();
                }
            }


            return fieldsWithContent;
        }
    }
}
