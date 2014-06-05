using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    [Serializable]
    public class RaceDetail
    {
        #region Private Member Variables

        //The RaceDetail race id
        private long races_race_id;
        //The RaceDetail greyhound id
        private long greyhounds_grey_id;
        //The RaceDetail greyhound name
        private string greyhound_name;
        //The RaceDetail greyhound trainer
        private string trainer;
        //The RaceDetail greyhound birth date
        private string birth_date;
        //The RaceDetail greyhound score
        private string score;
        //The RaceDetail track number of the greyhound
        private int track_number;
        //The RaceDetail race completed flag
        private bool race_completed;
        //The RaceDetail position of the greyhound
        private string position;
        //The RaceDetail odd for the greyhound
        private string odd;
        //The RaceDetail predition for the greyhound
        private string prediction;
        //The RaceDetail race time
        private Decimal time;
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #endregion

        #region Public Properties

        //The RaceDetail id
        public long Races_Race_Id
        {
            get
            {
                return this.races_race_id;
            }
            set
            {
                this.races_race_id = value;
            }
        }

        //The RaceDetail stadium id
        public long Greyhounds_Grey_Id
        {
            get
            {
                return this.greyhounds_grey_id;
            }
            set
            {
                this.greyhounds_grey_id = value;
            }
        }

        //The RaceDetail greyhound name
        public string Greyhound_Name
        {
            get
            {
                return this.greyhound_name;
            }
            set
            {
                this.greyhound_name = value;
            }
        }

        //The RaceDetail greyhound trainer
        public string Trainer
        {
            get
            {
                return this.trainer;
            }
            set
            {
                this.trainer = value;
            }
        }

        //The RaceDetail greyhound birth date
        public string Birth_Date
        {
            get
            {
                return this.birth_date;
            }
            set
            {
                this.birth_date = value;
            }
        }

        //The RaceDetail greyhound score
        public string Score
        {
            get
            {
                return this.score;
            }
            set
            {
                this.score = value;
            }
        }

        //The RaceDetail name
        public int Track_Number
        {
            get
            {
                return this.track_number;
            }
            set
            {
                this.track_number = value;
            }
        }

        //The RaceDetail Track Length
        public bool Race_Completed
        {
            get
            {
                return this.race_completed;
            }
            set
            {
                this.race_completed = value;
            }
        }

        //The RaceDetail Grade
        public string Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        //The RaceDetail Odd
        public string Odd
        {
            get
            {
                return this.odd;
            }
            set
            {
                this.odd = value;
            }
        }

        //The RaceDetail Prediction
        public string Prediction
        {
            get
            {
                return this.prediction;
            }
            set
            {
                this.prediction = value;
            }
        }

        //The RaceDetail RaceDetail Type
        public Decimal Time
        {
            get
            {
                return this.time;
            }
            set
            {
                this.time = value;
            }
        }

        #endregion

        #region Public Methods
        /**
         * @brief Class constructor
         * @param long races_race_id The RaceDetail race id
         * @param long greyhounds_grey_id The RaceDetail greyhound id
         * @param string greyhound_name The RaceDetail greyhound name
         * @param string trainer The RaceDetail greyhound trainer
         * @param string birth_date The RaceDetail greyhound birth date
         * @param string score The RaceDetail greyhound score
         * @param int track_number The RaceDetail greyhound track number
         * @param bool race_completed The RaceDetail greyhound race completed flag
         * @param string position The RaceDetail greyhound position
         * @param string odd The RaceDetail odd for the greyhound
         * @param string prediction The RaceDetail prediction for the greyhound
         * @param Decimal time The RaceDetail greyhound time in race
         **/
        public RaceDetail(
            long races_race_id,
            long greyhounds_grey_id,
            string greyhound_name,
            string trainer,
            string birth_date,
            string score,
            int track_number,
            bool race_completed,
            string position,
            string odd,
            string prediction,
            Decimal time)
        {
            this.races_race_id = races_race_id;
            this.greyhounds_grey_id = greyhounds_grey_id;
            this.greyhound_name = greyhound_name;
            this.trainer = trainer;
            this.birth_date = birth_date;
            this.score = score;
            this.track_number = track_number;
            this.race_completed = race_completed;
            this.position = position;
            this.odd = odd;
            this.prediction = prediction;
            this.time = time;
        }

        #endregion
    }
}
