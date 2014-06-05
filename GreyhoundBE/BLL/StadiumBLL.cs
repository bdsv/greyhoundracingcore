using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GreyhoundBE.DAL;
using GreyhoundBE.BO;

namespace GreyhoundBE.BLL
{
    public class StadiumBLL
    {
        /**
         * @brief Returns all users from Greyhound database system
         * @return An array with all users.
         */
        public static Stadium[] GetAllStadiums()
        {
            return StadiumDAL.GetStadiumsPaged("1=1", 1, 100, DatabaseSchema.STADIUMS_STADIUM_ID_FIELD, "ASC");
        }

        /**
         * @brief Returns all users from Greyhound database system
         * @return An array with all users.
         */
        public static Stadium[] GetStadiumsPaged(string filter, int startRec, int endRec, string sortBy, string sortType)
        {
            return StadiumDAL.GetStadiumsPaged(filter, startRec, endRec, sortBy, sortType);
        }
    }
}
