using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GreyhoundBE.BO;
using GreyhoundBE.BLL;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;

namespace GreyhoundBE.DAL
{
    /**
     * @brief This class defines the data access layer (DAL) for the database users.
     **/
    class RaceDetailDAL : DatabaseSchema
    {
        #region Private Member Variables


        private const string GET_RACES_DETAILS_BY_RACE_ID_SQL =
            "SELECT * FROM " + RACES_DETAILS_VIEW +
            " WHERE " + RACES_DETAILS_VIEW + "." + V_RACES_DETAILS_RACES_ID_FIELD + "='@races_race_id'" +
            " ORDER BY @sortBy @sortType";

        private const string GET_RACES_DETAILS_BY_DATE_SQL =
            "SELECT * FROM " + RACES_DETAILS_VIEW +
            " WHERE TO_DATE(TO_CHAR(" + RACES_DETAILS_VIEW + "." + V_RACES_DETAILS_RACES_DATE_TIME_FIELD + ", 'yyyy/mm/dd'), 'yyyy/mm/dd') =" +
            " TO_DATE('@date_time', 'yyyy/mm/dd')" +
            " ORDER BY @sortBy @sortType";
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        private static List<RaceDetail> executeQuery(User user, String strCommand)
        {
            List<RaceDetail> resultList = new List<RaceDetail>();

            OracleConnection connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(user.Role_Id));
            OracleDataReader sqlDataReader = null;
            try
            {
                //Open the connection
                connection.Open();
                
                OracleCommand command = new OracleCommand(strCommand);
                command.Connection = connection;
                //Runs the command
                sqlDataReader = command.ExecuteReader();

                //Fetches all rows
                while (sqlDataReader.Read())
                {
                    int races_race_id = Convert.ToInt32(sqlDataReader[V_RACES_DETAILS_RACES_ID_FIELD].ToString());
                    int greyhounds_grey_id = Convert.ToInt32(sqlDataReader[GREYHOUNDS_GREYHOUND_ID_FIELD].ToString());
                    string greyhound_name = sqlDataReader[GREYHOUNDS_NAME_FIELD].ToString();
                    string trainer = sqlDataReader[GREYHOUNDS_TRAINER_FIELD].ToString();
                    string birth_date = sqlDataReader[GREYHOUNDS_BIRTH_DATE_FIELD].ToString();
                    string score = sqlDataReader[GREYHOUNDS_SCORE_FIELD].ToString();
                    score = score.Replace(",", ".");
                    int track_number = Convert.ToInt32(sqlDataReader[RACES_DETAILS_TRACK_NUMBER_FIELD].ToString());
                    bool race_completed = Tools.StringToBool(sqlDataReader[RACES_DETAILS_RACE_COMPLETED_FIELD].ToString());
                    string position = sqlDataReader[RACES_DETAILS_POSITION_FIELD].ToString();
                    string odd = sqlDataReader[ODDS_VALUE_FIELD].ToString();
                    string prediction = "";
                    if( user != null && user.Role_Id > Role.ROLES.ANONYMOUS_USER_ROLE ) {
                        prediction = sqlDataReader[PREDICTIONS_ADVANCED_VALUE_FIELD].ToString();
                    }
                    else {
                        prediction = sqlDataReader[PREDICTIONS_SIMPLE_VALUE_FIELD].ToString();
                    }
                    prediction = prediction.Replace(",", ".");
                    Decimal time = Tools.StringToDecimal(sqlDataReader[RACES_DETAILS_TIME_FIELD].ToString());

                    RaceDetail race_detail = new RaceDetail(
                        races_race_id,
                        greyhounds_grey_id,
                        greyhound_name,
                        trainer,
                        birth_date,
                        score,
                        track_number,
                        race_completed,
                        position,
                        odd,
                        prediction,
                        time);

                    resultList.Add(race_detail);
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

            return resultList;
        }
        #endregion

        #region Constructors
        #endregion

        #region Public Properties
        #endregion

        #region Public Methods
        /**
         * @brief Returns all race details from a race identified by an ID
         * @param string races_race_id The race id index to return the details from the database
         * @param string sortBy The sort field to apply to the query
         * @param string sortType The sort type to apply to the query
         * @return RaceDetail[] An array with all races.
         **/
        public static RaceDetail[] GetRaceDetailsByRaceID(User user, string races_race_id, string sortBy, string sortType)
        {
            //Setup the command to run                
            string strCommand = GET_RACES_DETAILS_BY_RACE_ID_SQL;
            strCommand = strCommand.Replace("@races_race_id", races_race_id);
            strCommand = strCommand.Replace("@sortBy", sortBy);
            strCommand = strCommand.Replace("@sortType", sortType);

            Tools.printDebug(null, strCommand, null);

            List<RaceDetail> resultList = executeQuery(user, strCommand);

            return resultList.ToArray<RaceDetail>();
        }

        /**
         * @brief Returns the details of all races that occur(ed) in a particular date
         * @param string date_time The date string that identifies the date to retrieve from the database
         * @param string sortBy The sort field to apply to the query
         * @param string sortType The sort type to apply to the query
         * @return RaceDetail[] An array with all races.
         **/
        public static RaceDetail[] GetRaceDetailsByDate(User user, string date_time, string sortBy, string sortType)
        {
            //Setup the command to run                
            string strCommand = GET_RACES_DETAILS_BY_DATE_SQL;
            strCommand = strCommand.Replace("@date_time", date_time);
            strCommand = strCommand.Replace("@sortBy", sortBy);
            strCommand = strCommand.Replace("@sortType", sortType);

            Tools.printDebug(null, strCommand, null);

            List<RaceDetail> resultList = executeQuery(user, strCommand);

            return resultList.ToArray<RaceDetail>();
        }       
        #endregion
    }
}
