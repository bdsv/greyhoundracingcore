using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GreyhoundBE.DAL;
using GreyhoundBE.BO;

namespace GreyhoundBE.BLL
{
    public class RaceBLL
    {
        /**
         * @brief Returns all races from Greyhound database system
         * @return An array with all races.
         */
        public static Race[] GetRacesPaged(string filter, int startRec, int endRec, string sortBy, string sortType)
        {
            return RaceDAL.GetRacesPaged(filter, startRec, endRec, sortBy, sortType);
        }
    }
}
