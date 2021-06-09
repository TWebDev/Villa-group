using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;


namespace ePlatFront.Models
{
    public class ReCaptchaV2
    {
        public class APIResponse
        {
            public bool success { get; set; }
            public string challenge_ts { get; set; }
            public string hostname { get; set; }
            public List<string> error_codes { get; set; }
        }

        public static bool Validate(string secret, string token)
        {
            bool valid = false;

            string json = "";
            using (var client = new WebClient())
            {
               NameValueCollection reqparm = new NameValueCollection();
               reqparm.Add("secret", secret);
               reqparm.Add("response", token);
               byte[] responsebytes = client.UploadValues("https://www.google.com/recaptcha/api/siteverify", "POST", reqparm);
    json = Encoding.UTF8.GetString(responsebytes);
            }

            var serializer = new JavaScriptSerializer();
            try
            {
                APIResponse response = serializer.Deserialize<APIResponse>(json);
                valid = response.success;
            }
            catch (Exception e)
            {
                string body= "ReCaptchaV2.Validate:  " + e.InnerException.Message;
                var email = ePlatBack.Models.Utils.EmailNotifications.GetSystemEmail(body);
                ePlatBack.Models.Utils.EmailNotifications.Send(email, false);
            }            

            return valid;
        }
    }
}