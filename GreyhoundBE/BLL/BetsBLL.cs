using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GreyhoundBE.DAL;
using GreyhoundBE.BO;

namespace GreyhoundBE.BLL
{
    public class BetsBLL
    {
        /**
         * @brief Returns all user's bets from Greyhound database system
         * @return An array with all user's bets
         */
        public static Bet[] GetBetsByUserId(User user)
        {
            return BetsDAL.GetBetsByUserId(user);
        }

        /**
         * @brief Try to insert a bet into the database
         * @return the bet_id if sucessful, -1 otherwise
         */
        public static long InsertBet(User user, Bet bet)
        {
            return BetsDAL.InsertBet(user, bet);
        }
    }
}
