using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GreyhoundBE.DAL;
using GreyhoundBE.BO;

namespace GreyhoundBE.BLL
{
    public class GreyhoundBLL
    {
        /**
         * @brief Returns all users from Greyhound database system
         * @return An array with all users.
         */
        public static Greyhound[] GetGreyhoundsRunningAtDate(User user, string date_time)
        {
            return GreyhoundDAL.GetGreyhoundsRunningAtDate(user, date_time);
        }

        ///**
        // * @brief Returns all users from Greyhound database system
        // * @return An array with all users.
        // */
        //public static Greyhound[] GetGreyhoundsPaged(string filter, int startRec, int endRec, string sortBy, string sortType)
        //{
        //    return GreyhoundDAL.GetGreyhoundsPaged(filter, startRec, endRec, sortBy, sortType);
        //}
    }
}
