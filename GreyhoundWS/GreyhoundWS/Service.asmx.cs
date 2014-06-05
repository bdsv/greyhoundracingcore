using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

using GreyhoundBE.BO;
using GreyhoundBE.BLL;
using System.Web.Security;
using System.Web.Script.Services;
using System.Web.SessionState;

namespace GreyhoundWS
{
    /// <summary>
    /// Summary description for Service
    /// </summary>
    [WebService(Namespace = "greyhound.be.webservice")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service : System.Web.Services.WebService
    {
        #region Private Methods
        private bool AuthenticateUser(string user_email, string pwd)
        {
            if ((user_email.Equals("guest") || user_email.Equals("guest.greyhounds.racing@gmail.com")) && pwd.Equals("3AaR84L8pBlghaM3JZwORA=="))
                return true;

            return UserBLL.LoginUser(user_email, pwd);
        }

        private void SendResponse(String result)
        {
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.Write(result);
        }
        #endregion

        #region WebMethods
        [WebMethod]
        public void LoginUser(string user, string token)
        {
            if (!AuthenticateUser(user, token))
            {
                String result = "{result: 'error', errormsg: 'Login Failed!'}";
                SendResponse(result);
            }
            else
            {
                if(user.ToLower().Equals("guest"))
                {
                    user = "guest.greyhounds.racing@gmail.com";
                }

                User session_user = UserBLL.GetUserByEmail(user);

                SessionIDManager Manager = new System.Web.SessionState.SessionIDManager();
                string session_id = Manager.CreateSessionID(Context);                
                GreyhoundSession session = new GreyhoundSession(session_id, session_user);

                // TODO: Implement session keys:
                //    1) generate a session key and ad it to the session keys table with an expiration date and associated to the user id
                //    2) return the key to the mobile application
                //    3) in each method call check the session key validity for the user id:
                //        i) if the key is invalid ignore the request
                //       ii) if the key is valid respond to the method call
                //      iii) if the key is valid but has expired generate a new key, return the key to the mobile app and respond to the method call

                String result = "{result: 'success', " +
                    "sessionid: '" + session.Session_Id + "'," +
                    "user_id: '" + session_user.User_Id + "'," +
                    "role_id: '" + session_user.Role_Id + "'," +
                    "name: '" + session_user.Name + "'," +                    
                    "address: '" + session_user.Address + "'," +
                    "mobile: '" + session_user.Mobile + "'," +
                    "paypal_id: '" + session_user.Paypal_Id + "'," +
                    "betfair_id: '" + session_user.Betfair_Id + "'," +
                    "expire: '" + session.Validity + "'}";

                SendResponse(result);
            }
        }

        [WebMethod]
        public void GetBetsByUserId(string user, string token)
        {
            if (!AuthenticateUser(user, token))
            {
                String result = "{error:\"not authenticated\", items:[]}";
                SendResponse(result);
                return;
            }

            User session_user = UserBLL.GetUserByEmail(user);

            Bet[] bets = BetsBLL.GetBetsByUserId(session_user);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Bet));
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (Bet bet in bets)
                {
                    ser.WriteObject(stream, bet);
                }

                String result = "{result: \"success\", items:[" + String.Join("},", Encoding.Default.GetString(stream.ToArray()).Split('}')) + "]}";
                result = Tools.normalizeResult(result);

                SendResponse(result);
            }
        }

        [WebMethod]
        public void InsertBet(string user, string token, int bet_type_id, int race_id, int grey_id, int bet_value)
        {
            if (!AuthenticateUser(user, token))
            {
                String result = "{error:\"not authenticated\", items:[]}";
                SendResponse(result);
                return;
            }

            User session_user = UserBLL.GetUserByEmail(user);
            Bet bet = new Bet(session_user.User_Id, bet_type_id, race_id, grey_id, bet_value);
            bet.Date_Time = (DateTime.Now.ToString("yyyy\\/MM\\/dd HH:mm:ss"));

            long bet_id = BetsBLL.InsertBet(session_user, bet);
            Tools.printDebug("Bet Id: " + bet_id);
            if( bet_id > 0 ) {                
                String result = "{result: 'success', " +
                    "user_id: '" + session_user.User_Id + "'," +
                    "bet_id: '" + bet_id + "'}";

                SendResponse(result);
            }
            else {
                String result = "{error:\"failed to insert the bet\", items:[]}";
                SendResponse(result);
            }            
        }

        [WebMethod]
        public void GetRaceEventsAtDate(string user, string token, DateTime date)
        {
            if (!AuthenticateUser(user, token))
            {
                String result = "{error:\"not authenticated\", items:[]}";
                SendResponse(result);
                return;
            }

            User session_user = UserBLL.GetUserByEmail(user);

            RaceEvent[] race_events = RaceEventBLL.GetRaceEventsAtDate(session_user, date.ToString("yyyy/MM/dd"));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RaceEvent));
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (RaceEvent race_event in race_events)
                {
                    ser.WriteObject(stream, race_event);
                }

                String result = "{result: \"success\", items:[" + String.Join("},", Encoding.Default.GetString(stream.ToArray()).Split('}')) + "]}";
                result = Tools.normalizeResult(result);

                SendResponse(result);
            }
        }

        [WebMethod]
        public void GetRaceDetailsByDate(string user, string token, DateTime date)
        {
            if (!AuthenticateUser(user, token))
            {
                String result = "{error:\"not authenticated\", items:[]}";
                SendResponse(result);
                return;
            }

            User session_user = UserBLL.GetUserByEmail(user);

            RaceDetail[] race_details = RaceDetailsBLL.GetRaceDetailsByDate(session_user, date.ToString("yyyy/MM/dd"), "track_number", "ASC");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RaceDetail));
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (RaceDetail race_detail in race_details)
                {
                    ser.WriteObject(stream, race_detail);
                }

                String result = "{result: \"success\", items:[" + String.Join("},", Encoding.Default.GetString(stream.ToArray()).Split('}')) + "]}";
                result = Tools.normalizeResult(result);

                SendResponse(result);
            }
        }

        [WebMethod]
        public void GetRaceDetailsByRaceId(string user, string token, int raceid)
        {
            if (!AuthenticateUser(user, token))
            {
                String result = "{error:\"not authenticated\", items:[]}";
                SendResponse(result);
                return;
            }

            User session_user = UserBLL.GetUserByEmail(user);

            RaceDetail[] race_details = RaceDetailsBLL.GetRaceDetailsByRaceID(session_user, "" + raceid, "track_number", "ASC");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RaceDetail));
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (RaceDetail race_detail in race_details)
                {
                    ser.WriteObject(stream, race_detail);
                }

                String result = "{result: \"success\", items:[" + String.Join("},", Encoding.Default.GetString(stream.ToArray()).Split('}')) + "]}";
                result = Tools.normalizeResult(result);

                SendResponse(result);
            }
        }

        [WebMethod]
        public void GetGreyhoundsRunningAtDate(string user, string token, string date)
        {
            if (!AuthenticateUser(user, token))
            {
                String result = "{error:\"not authenticated\", items:[]}";
                SendResponse(result);
                return;
            }

            User session_user = UserBLL.GetUserByEmail(user);

            Greyhound[] greyhounds = GreyhoundBLL.GetGreyhoundsRunningAtDate(session_user, date);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Greyhound));
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (Greyhound grey in greyhounds)
                {
                    ser.WriteObject(stream, grey);
                }


                String result = "{result: \"success\", items:[" + String.Join("},", Encoding.Default.GetString(stream.ToArray()).Split('}')) + "]}";
                result = Tools.normalizeResult(result);

                SendResponse(result);
            }
        }

        [WebMethod]
        public void RegisterNewUser(string user, string token,
            string name, string email, string password, string mobile,
            string address, string paypal_id, string betfair_id )
        {
            if (!AuthenticateUser(user, token))
            {
                String result = "{error:\"not authenticated\", items:[]}";
                SendResponse(result);
                return;
            }

            User newuser = new User(-1, Role.ROLES.REGISTERED_USER_ROLE, password, name, email, mobile, address, paypal_id, betfair_id);

            int res = UserBLL.RegisterNewUser(newuser);
            bool email_sent = Tools.sendMail(email, "" + res);
            if (res > 0 && email_sent)
            {
                String result = "{result: \"success\"}";

                SendResponse(result);
            }
            else
            {
                String errormsg = "'Unknown error!'";
                if( res > 0 )
                    errormsg = "'Failed to add the user account to the system'";
                else if( !email_sent )
                    errormsg = "'Failed to send the email address'";

                String result = "{result: 'error', errormsg: " + errormsg + "}";

                SendResponse(result);
            }
        }

        [WebMethod]
        public void UpdateUserSettings(string user, string token,
            string name, string password, string mobile,
            string address, string paypal_id, string betfair_id)
        {
            if (!AuthenticateUser(user, token))
            {
                String result = "{error:\"not authenticated\", items:[]}";
                SendResponse(result);
                return;
            }

            User existentUser = UserBLL.GetUserByEmail(user);
            int res = -1;
            if (existentUser != null)
            {
                if( address.Length > 0 )
                    existentUser.Address = address;
                if (name.Length > 0)
                    existentUser.Name = name;
                if (mobile.Length > 0)
                    existentUser.Mobile = mobile;
                if (paypal_id.Length > 0)
                    existentUser.Paypal_Id = paypal_id;
                if (betfair_id.Length > 0)
                    existentUser.Betfair_Id = betfair_id;
                if (password.Length > 0)
                    existentUser.Password = password;

                res = UserBLL.UpdateUserSettings(existentUser);
            }
            
            if (res > 0)
            {
                String result = "{result: \"success\"}";

                SendResponse(result);
            }
            else
            {
                String errormsg = "'Failed to update the user account in the system'";
                String result = "{result: 'error', errormsg: " + errormsg + "}";

                SendResponse(result);
            }
        }
        #endregion
    }
}