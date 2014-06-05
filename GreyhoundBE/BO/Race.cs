using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    [Serializable]
    public class Race
    {
        #region Private Member Variables
        //The Race id
        private long race_id;
        //The Race stadium id
        private long stadiums_std_id;
        //The Race date and time
        private string date_time;
        //The Race number
        private int race_number;
        //The Race track_length
        private Decimal track_length;
        //The Race grade
        private string grade;
        //The Race race type
        private string race_type;
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #endregion

        #region Public Properties

        //The Race id
        public long Race_Id
        {
            get
            {
                return this.race_id;
            }
            set
            {
                this.race_id = value;
            }
        }

        //The Race stadium id
        public long Stadiums_Std_Id
        {
            get
            {
                return this.stadiums_std_id;
            }
            set
            {
                this.stadiums_std_id = value;
            }
        }

        //The Race name
        public string Date_Time
        {
            get
            {
                return this.date_time;
            }
            set
            {
                this.date_time = value;
            }
        }

        //The Race race number
        public int Race_Number
        {
            get
            {
                return this.race_number;
            }
            set
            {
                this.race_number = value;
            }
        }

        //The Race Track Length
        public Decimal Track_Length
        {
            get
            {
                return this.track_length;
            }
            set
            {
                this.track_length = value;
            }
        }

        //The Race Grade
        public string Grade
        {
            get
            {
                return this.grade;
            }
            set
            {
                this.grade = value;
            }
        }

        //The Race Race Type
        public string Race_Type
        {
            get
            {
                return this.race_type;
            }
            set
            {
                this.race_type = value;
            }
        }

        #endregion

        #region Public Methods
        /**
         * @brief Class constructor
         * @param long race_id The Race id
         * @param long stadiums_std_id The Race stadium id
         * @param string date_time The Race date and time
         * @param int race_number The Race number
         * @param float track_length The Race track length
         * @param string grade The Race grade
         * @param string race_type The Race type
         **/
        public Race(
            long race_id,
            long stadiums_std_id,
            string date_time,
            int race_number,
            Decimal track_length,
            string grade,
            string race_type)
        {
            this.race_id = race_id;
            this.stadiums_std_id = stadiums_std_id;
            this.date_time = date_time;
            this.race_number = race_number;
            this.track_length = track_length;
            this.grade = grade;
            this.race_type = race_type;
        }

        #endregion
    }
}
