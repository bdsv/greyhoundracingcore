using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GreyhoundBE.DAL;
using GreyhoundBE.BO;

namespace GreyhoundBE.BLL
{
    public class UserBLL
    {
        /**
         * @brief Returns a user from Greyhound database system provided an email address
         * @return An array with all users.
         */
        public static User GetUserByEmail(string email)
        {
            if (email == "guest") email = "guest.greyhounds.racing@gmail.com";

            return UserDAL.GetUserByEmail(email);
        }

        /**
         * @brief Returns all users from Greyhound database system
         * @return An array with all users.
         */
        public static int ConfirmUserRegistration(string user_id, string email)
        {
            return UserDAL.ConfirmUserRegistration(user_id, email);
        }

        /**
         * @brief Registers a new user in Greyhound database system
         * @return The user_id of the registered user
         */
        public static int RegisterNewUser(User user)
        {
            return UserDAL.RegisterNewUser(user);
        }

        /**
         * @brief Updates an existent user account in Greyhound database system
         * @return The user_id of the registered user
         */
        public static int UpdateUserSettings(User user)
        {
            return UserDAL.UpdateUserSettings(user);
        }

        /**
         * @brief Try to login with the credentials provided
         * @return True if login was sucessful, False otherwise
         */
        public static bool LoginUser(String user_email, String password)
        {
            return UserDAL.LoginUser(user_email, password);
        }

        ///**
        // * @brief Returns all users from Greyhound database system
        // * @return An array with all users.
        // */
        //public static User[] GetUsersPaged(string filter, int startRec, int endRec, string sortBy, string sortType)
        //{
        //    return UserDAL.GetUsersPaged(filter, startRec, endRec, sortBy, sortType);
        //}
    }
}
