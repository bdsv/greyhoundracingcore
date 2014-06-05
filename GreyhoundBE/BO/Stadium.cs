using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    [Serializable]
    public class Stadium
    {
        #region Private Member Variables

        //The Stadium id
        private long std_id;
        //The Stadium name
        private string name;
        //The Stadium number of tracks
        private string num_tracks;
        //The Stadium city
        private string city;
        //The Stadium country
        private string country;
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #endregion

        #region Public Properties

        //The Stadium id
        public long Std_Id
        {
            get
            {
                return this.std_id;
            }
            set
            {
                this.std_id = value;
            }
        }

        //The Stadium name
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

        //The Stadium number of tracks
        public string Num_Tracks
        {
            get
            {
                return this.num_tracks;
            }
            set
            {
                this.num_tracks = value;
            }
        }

        //The Stadium City
        public string City
        {
            get
            {
                return this.city;
            }
            set
            {
                this.city = value;
            }
        }

        //The Stadium Country
        public string Country
        {
            get
            {
                return this.country;
            }
            set
            {
                this.country = value;
            }
        }

        #endregion

        #region Public Methods
        /**
         * @brief Class constructor
         * @param long std_id The Stadium id
         * @param string name The Stadium name
         * @param string num_tracks The Stadium number of tracks
         * @param string city The Stadium city
         * @param string country The Stadium country
         **/
        public Stadium(
            long std_id,
            string name,
            string num_tracks,
            string city,
            string country)
        {
            this.std_id     = std_id;
            this.name       = name;
            this.num_tracks = num_tracks;
            this.city       = city;
            this.country    = country;
        }

        #endregion
    }
}
