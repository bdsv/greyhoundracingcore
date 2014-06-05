using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    [Serializable]
    public class Greyhound
    {
        #region Private Member Variables

        //The greyhound id
        private long grey_id;
        //The greyhound full name
        private string name;
        //The greyhound trainer
        private string trainer;
        //The greyhound birth date
        private string birth_date;
        //The greyhound score
        private string score;
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #endregion

        #region Public Properties

        //The greyhound id
        public long Grey_Id
        {
            get
            {
                return this.grey_id;
            }
            set
            {
                this.grey_id = value;
            }
        }

        //The greyhound name
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

        //The greyhound trainer name
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

        //The greyhound birth date
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

        //The greyhound score
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

        #endregion

        #region Public Methods
        /**
         * @brief Class constructor
         * @param long grey_id The greyhound id
         * @param string name The greyhound full name
         * @param string trainer The greyhound trainer
         * @param string birth_date The greyhound birth date
         **/
        public Greyhound(
            long grey_id,
            string name,
            string trainer,
            string birth_date,
            string score)
        {
            this.grey_id    = grey_id;
            this.name       = name;
            this.trainer    = trainer;
            this.birth_date = birth_date;
            this.score      = score;
        }

        #endregion
    }
}
