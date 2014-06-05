using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GreyhoundBE.BO;
using GreyhoundBE.BLL;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using System.Data;

namespace GreyhoundBE.DAL
{
    class BetsDAL : DatabaseSchema
    {
        #region Private Member Variables
        private const string GET_BETS_BY_USER_ID_SQL =
            "SELECT * FROM " + BETS_TABLE +
            " WHERE " + BETS_USER_ID + " = '@user_id'";
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #endregion

        #region Constructors
        #endregion

        #region Public Properties
        #endregion

        #region Public Methods
        /**
         * @brief Returns all races from DB
         * @param string date_time The date_time to apply to the query
         * @return RaceEvent[] An array with all stadiums, races and races details (RaceEvent array) at the date provided
         **/
        public static Bet[] GetBetsByUserId(User user)
        {
            List<Bet> betsList = new List<Bet>();
            OracleConnection connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(user.Role_Id));
            OracleDataReader sqlDataReader = null;
            try
            {
                //Open the connection
                connection.Open();
                //Setup the command to run                
                string strCommand = GET_BETS_BY_USER_ID_SQL;
                strCommand = strCommand.Replace("@user_id", user.User_Id.ToString());

                Tools.printDebug(null, strCommand, null);

                OracleCommand command = new OracleCommand(strCommand);
                command.Connection = connection;
                //Runs the command
                sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    long bet_id = Convert.ToInt64(sqlDataReader[BETS_BET_ID].ToString());
                    long user_id = Convert.ToInt64(sqlDataReader[BETS_USER_ID].ToString());
                    long bet_type_id = Convert.ToInt64(sqlDataReader[BETS_BET_TYPE_ID].ToString());
                    long race_id = Convert.ToInt64(sqlDataReader[BETS_RACE_ID].ToString());
                    long grey_id = Convert.ToInt64(sqlDataReader[BETS_GREY_ID].ToString());
                    string date_time = sqlDataReader[BETS_DATE_TIME].ToString();
                    Decimal bet_value = Tools.StringToDecimal(sqlDataReader[BETS_BET_VALUE].ToString());
                    int bet_result = (!sqlDataReader[BETS_BET_RESULT].ToString().Equals("")?
                        Convert.ToInt32(sqlDataReader[BETS_BET_RESULT].ToString()) : -1);
                    Decimal return_value = Tools.StringToDecimal(sqlDataReader[BETS_RETURN_VALUE].ToString());

                    Bet bet = new Bet(
                        bet_id,
                        user_id,
                        bet_type_id,
                        race_id,
                        grey_id,
                        date_time,
                        bet_value,
                        bet_result,
                        return_value
                    );
                    betsList.Add(bet);
                }
            }
            catch (InvalidOperationException exception)
            {
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new InvalidOperationException(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            catch (SqlException exception)
            {
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new InvalidOperationException(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            catch (FormatException exception)
            {
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new FormatException(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            catch (OverflowException exception)
            {
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new OverflowException(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            finally
            {
                //Closes the datareader and connection to the database
                if (sqlDataReader != null)
                {
                    sqlDataReader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }

            //Returns the list of fetched bets in the form of an array
            return betsList.ToArray();
        }

        /**
         * @brief Try to insert a bet into the database
         * @return the bet_id if sucessful, -1 otherwise
         */
        public static long InsertBet(User user, Bet bet)
        {
            long result = -1;
            try
            {
                using (var connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(user.Role_Id)))
                {
                    using (var Command = new OracleCommand())
                    {
                        Command.Connection = connection;
                        Command.CommandText = "INSERT_BET";
                        Command.CommandType = CommandType.StoredProcedure;

                        using (var param = new OracleParameter())
                        {
                            param.OracleDbType = OracleDbType.Long;
                            param.Direction = ParameterDirection.Input;
                            param.ParameterName = "userId";
                            param.Value = bet.User_Id;
                            Command.Parameters.Add(param);
                        }
                        using (var param = new OracleParameter())
                        {
                            param.OracleDbType = OracleDbType.Long;
                            param.Direction = ParameterDirection.Input;
                            param.ParameterName = "betTypeId";
                            param.Value = bet.Bet_Type_Id;
                            Command.Parameters.Add(param);
                        }
                        using (var param = new OracleParameter())
                        {
                            param.OracleDbType = OracleDbType.Long;
                            param.Direction = ParameterDirection.Input;
                            param.ParameterName = "raceDetailId";
                            param.Value = bet.Race_Id;
                            Command.Parameters.Add(param);
                        }
                        using (var param = new OracleParameter())
                        {
                            param.OracleDbType = OracleDbType.Long;
                            param.Direction = ParameterDirection.Input;
                            param.ParameterName = "raceGreyId";
                            param.Value = bet.Grey_Id;
                            Command.Parameters.Add(param);
                        }
                        using (var param = new OracleParameter())
                        {
                            param.OracleDbType = OracleDbType.Decimal;
                            param.Direction = ParameterDirection.Input;
                            param.ParameterName = "betValue";
                            param.Value = bet.Bet_Value;
                            Command.Parameters.Add(param);
                        }
                        using (var param = new OracleParameter())
                        {
                            param.OracleDbType = OracleDbType.Int32;
                            param.Direction = ParameterDirection.Output;
                            param.ParameterName = "out_betId";
                            Command.Parameters.Add(param);
                        }

                        connection.Open();
                        int res = Command.ExecuteNonQuery();

                        if (Command.Parameters["out_betId"].DbType == System.Data.DbType.Int32)
                        {
                            result = Int32.Parse((Command.Parameters["out_betId"].Value).ToString());
                            Tools.printDebug("Insert Bet Result: " + result);
                        }
                        else
                        {
                            Tools.printDebug("Failed to Insert the Bet");
                        }
                    }
                }
            }
            catch (InvalidOperationException exception)
            {
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new InvalidOperationException(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            catch (SqlException exception)
            {
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new InvalidOperationException(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            catch (FormatException exception)
            {
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new FormatException(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            catch (OverflowException exception)
            {
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new OverflowException(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            catch (Exception exception)
            {
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
            }

            return result;
        }
        #endregion
    }
}
