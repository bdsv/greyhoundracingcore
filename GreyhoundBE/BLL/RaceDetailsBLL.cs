using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GreyhoundBE.DAL;
using GreyhoundBE.BO;

namespace GreyhoundBE.BLL
{
    public class RaceDetailsBLL
    {
        /**
         * @brief Returns an array of races_details by races_id query from Greyhound database system
         * @return An array of races_details
         */
        public static RaceDetail[] GetRaceDetailsByRaceID(User user, string races_race_id, string sortBy, string sortType)
        {
            return RaceDetailDAL.GetRaceDetailsByRaceID(user, races_race_id, sortBy, sortType);
        }

        /**
         * @brief Returns an array of races_details by date query from Greyhound database system
         * @return An array of races_details
         */
        public static RaceDetail[] GetRaceDetailsByDate(User user, string date, string sortBy, string sortType)
        {
            return RaceDetailDAL.GetRaceDetailsByDate(user, date, sortBy, sortType);
        }

        ///**
        // * @brief Returns all races_details entries from Greyhound database system
        // * @return An array with all races_details.
        // */
        //public static RaceDetail[] GetAllRaceDetailsPaged(string filter, int startRec, int endRec, string sortBy, string sortType)
        //{
        //    return RaceDetailDAL.GetAllRaceDetailsPaged(filter, startRec, endRec, sortBy, sortType);
        //}
        //
        ///**
        // * @brief Returns an array of races_details by greyhound_id query from Greyhound database system
        // * @return An array of races_details
        // */
        //public static RaceDetail[] GetRaceDetailsByGreyhoundID(string greyhound_id, string sortBy, string sortType)
        //{
        //    return RaceDetailDAL.GetRaceDetailsByGreyhoundID(greyhound_id, sortBy, sortType);
        //}

        ///**
        // * @brief Returns an array of races_details by greyhound_id query from Greyhound database system
        // * @return An array of races_details
        // */
        //public static RaceDetail GetRaceDetailsByRaceAndGreyhoundID(string race_id, string greyhound_id, string sortBy, string sortType)
        //{
        //    return RaceDetailDAL.GetRaceDetailsByRaceAndGreyhoundID(race_id, greyhound_id, sortBy, sortType);
        //}
    }
}
