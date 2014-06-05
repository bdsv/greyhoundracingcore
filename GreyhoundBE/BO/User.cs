using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    [Serializable]
    public class User
    {
        #region Private Member Variables

        //The user id
        private long user_id;
        //The user password
        private string password;
        //The user role id
        private Role.ROLES role_id;
        //The user full name
        private string name;
        //The user email
        private string email;
        //The user mobile number
        private string mobile;
        //The user address
        private string address;
        //The user Paypal ID
        private string paypal_id;
        //The user Betfair ID
        private string betfair_id;
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #endregion

        #region Public Properties

        //The user id
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

        //The user password
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        //The user role id
        public Role.ROLES Role_Id
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

        //The user full name
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        //The user e-mail address
        public string Email
        {
            get
            {
                return this.email;
            }
            set
            {
                this.email = value;
            }
        }

        //The user mobile number
        public string Mobile
        {
            get
            {
                return this.mobile;
            }
            set
            {
                this.mobile = value;
            }
        }

        //The user address
        public string Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
            }
        }

        //The user Paypal ID
        public string Paypal_Id
        {
            get
            {
                return this.paypal_id;
            }
            set
            {
                this.paypal_id = value;
            }
        }

        //The user Betfair ID
        public string Betfair_Id
        {
            get
            {
                return this.betfair_id;
            }
            set
            {
                this.betfair_id = value;
            }
        }
        #endregion

        #region Public Methods
        /**
         * @brief Class constructor
         * @param long user_id The user id
         * @param int role_id The user role id
         * @param string name The user full name
         * @param string email The user email
         * @param string mobile The user mobile number
         * @param string address The user address
         * @param string paypal_id The user Paypal ID
         * @param string betfair_id The user Betfair ID
         **/
        public User(
            long user_id,
            Role.ROLES role_id,
            string name,
            string email,
            string mobile,
            string address,
            string paypal_id,
            string betfair_id)
        {
            this.user_id    = user_id;
            this.role_id    = role_id;
            this.name       = name;
            this.email      = email;
            this.mobile     = mobile;
            this.address    = address;
            this.paypal_id  = paypal_id;
            this.betfair_id = betfair_id;
            this.password   = null;
        }

        /**
         * @brief Class constructor
         * @param long user_id The user id
         * @param enum Role.ROLES role_id The user role id
         * @param string name The user full name
         * @param string email The user email
         * @param string mobile The user mobile number
         * @param string address The user address
         * @param string paypal_id The user Paypal ID
         * @param string betfair_id The user Betfair ID
         **/
        public User(
            long user_id,
            Role.ROLES role_id,
            string password,
            string name,
            string email,
            string mobile,
            string address,
            string paypal_id,
            string betfair_id)
        {
            this.user_id = user_id;
            this.role_id = role_id;
            this.password = password;
            this.name = name;
            this.email = email;
            this.mobile = mobile;
            this.address = address;
            this.paypal_id = paypal_id;
            this.betfair_id = betfair_id;
        }

        public void print()
        {
            Tools.printDebug("USERID: " + user_id);
            Tools.printDebug("NAME: " + name);
            Tools.printDebug("EMAIL: " + email);
            Tools.printDebug("PASSWORD: " + password);
            Tools.printDebug("ADDRESS: " + address);
        }
        #endregion
    }
}
