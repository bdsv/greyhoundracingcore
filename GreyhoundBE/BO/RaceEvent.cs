using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    [Serializable]
    public class RaceEvent
    {
        #region Private Member Variables
        //The Stadium entry
        private Stadium stadium;
        //The Race entry
        private Race race;
        ////The Races_Details entries
        //zzprivate List<RaceDetail> race_details;
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #endregion

        #region Public Properties

        //The Stadium entry
        public Stadium Stadium
        {
            get
            {
                return this.stadium;
            }
            set
            {
                this.stadium = value;
            }
        }

        //The Race entry
        public Race Race
        {
            get
            {
                return this.race;
            }
            set
            {
                this.race = value;
            }
        }

        ////The Races_Details entries
        //public List<RaceDetail> Race_Details
        //{
        //    get
        //    {
        //        return this.race_details;
        //    }
        //    set
        //    {
        //        this.race_details = value;
        //    }
        //}
        #endregion

        #region Public Methods
        /**
         * @brief Class constructor
         * @param Stadium stadium The Race stadium object
         * @param Race race The Race race object
         * @param List<RaceDetails> race_details The Race race_details object list
         **/
        public RaceEvent(
            Stadium stadium,
            Race race)
//            ,List<RaceDetail> race_details)
        {
            this.stadium = stadium;
            this.race = race;
//            this.race_details = race_details;
        }
        #endregion
    }
}
