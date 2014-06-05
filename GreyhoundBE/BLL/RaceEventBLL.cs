using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GreyhoundBE.DAL;
using GreyhoundBE.BO;

namespace GreyhoundBE.BLL
{
    public class RaceEventBLL
    {
        /**
         * @brief Returns all race events at a given date from Greyhound database system
         * @return An array with all race events at the given date.
         */
        public static RaceEvent[] GetRaceEventsAtDate(User user, string date_time)
        {
            return RaceEventDAL.GetRaceEventsAtDate(user, date_time);
        }
    }
}
