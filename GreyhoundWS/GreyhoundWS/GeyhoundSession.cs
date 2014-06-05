using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    public class GreyhoundSession
    {
        #region Private Member Variables
        //The Session identifier
        private string session_id;
        //The Session user identifier
        private long user_id;
        //The Session role identifier
        private int role_id;
        //The Session datetime validity
        private DateTime validity;
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #endregion

        #region Public Properties

        //The Session identifier
        public string Session_Id
        {
            get
            {
                return this.session_id;
            }
            set
            {
                this.session_id = value;
            }
        }

        //The Session user identifier
        public long User_Id
        {
            get
            {
                return this.user_id;
            }
            set
            {
                this.user_id = value;
            }
        }

        //The Session role identifier
        public int Role_Id
        {
            get
            {
                return this.role_id;
            }
            set
            {
                this.role_id = value;
            }
        }

        //The Session datetime validity
        public DateTime Validity
        {
            get
            {
                return this.validity;
            }
            set
            {
                this.validity = value;
            }
        }
        #endregion

        #region Public Methods
        /**
         * @brief Class constructor
         * @param string session_id The Session identifier
         * @param int user_id The Session user id
         * @param int role_id The Session role id
         * @param string validity The Session datetime validity
         **/
        public GreyhoundSession(
            string session_id,
            int user_id,
            int role_id,
            DateTime validity)
        {
            this.session_id = session_id;
            this.user_id    = user_id;
            this.role_id    = role_id;
            this.validity   = validity;
        }

        /**
         * @brief Class constructor
         * @param User user The user that is identified by the Session
         **/
        public GreyhoundSession(String session_id, User user)
        {
            this.session_id = session_id;
            this.user_id = user.User_Id;
            this.role_id = (int)user.Role_Id;
            this.validity = DateTime.Now.AddMinutes(30);
        }
        #endregion
    }
}
