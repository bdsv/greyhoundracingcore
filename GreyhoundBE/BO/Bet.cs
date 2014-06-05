using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    [Serializable]
    public class Bet
    {
        #region Private Member Variables
        //The Bet id
        private long bet_id;
        //The Bet user id
        private long user_id;
        //The Bet type id
        private long bet_type_id;
        //The Bet Race id
        private long race_id;
        //The Bet Greyhound id
        private long grey_id;
        //The Bet date time
        private string date_time;
        //The Bet value
        private Decimal bet_value;
        //The Bet result: lost, win
        private int bet_result;
        //The Bet return value
        private Decimal return_value;
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #endregion

        #region Public Properties

        public long Bet_Id
        {
            get
            {
                return this.bet_id;
            }
            set
            {
                this.bet_id = value;
            }
        }

        public long User_Id
        {
            get
            {
                return this.user_id;
            }
            set
            {
                this.user_id = value;
            }
        }

        public long Bet_Type_Id
        {
            get
            {
                return this.bet_type_id;
            }
            set
            {
                this.bet_type_id = value;
            }
        }

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

        public Decimal Bet_Value
        {
            get
            {
                return this.bet_value;
            }
            set
            {
                this.bet_value = value;
            }
        }

        public int Bet_Result
        {
            get
            {
                return this.bet_result;
            }
            set
            {
                this.bet_result = value;
            }
        }

        public Decimal Return_Value
        {
            get
            {
                return this.return_value;
            }
            set
            {
                this.return_value = value;
            }
        }

        #endregion

        #region Public Methods
        /**
         * @brief Class constructor
         * @param long bet_id The Bet id
         * @param long user_id The Bet user id
         * @param long bet_type_id The Bet type id
         * @param long race_id The Bet race id
         * @param long grey_id The Bet grey id
         * @param string date_time The Bet date and time
         * @param Decimal bet_value The amount of the Bet
         * @param int bet_result The Bet result: 0 - Loose | 1 - Win
         * @param Decimal return_value The Bet result amount (positive or negative)
         **/
        public Bet(
            long bet_id,
            long user_id,
            long bet_type_id,
            long race_id,
            long grey_id,
            string date_time,
            Decimal bet_value,
            int bet_result,
            Decimal return_value)
        {
            this.bet_id = bet_id;
            this.user_id = user_id;
            this.bet_type_id = bet_type_id;
            this.race_id = race_id;
            this.grey_id = grey_id;
            this.date_time = date_time;
            this.bet_value = bet_value;
            this.bet_result = bet_result;
            this.return_value = return_value;
        }

        /**
         * @brief Class constructor
         * @param long user_id The Bet user id
         * @param long bet_type_id The Bet type id
         * @param long race_id The Bet race id
         * @param long grey_id The Bet grey id
         * @param Decimal bet_value The amount of the Bet
         **/
        public Bet(
            long user_id,
            long bet_type_id,
            long race_id,
            long grey_id,
            Decimal bet_value)
        {
            this.bet_id = -1;
            this.user_id = user_id;
            this.bet_type_id = bet_type_id;
            this.race_id = race_id;
            this.grey_id = grey_id;
            this.date_time = DateTime.Now.ToString();
            this.bet_value = bet_value;
            this.bet_result = -1;
            this.return_value = 0;
        }

        #endregion
    }
}
