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
    class RaceDAL : DatabaseSchema
    {
        #region Private Member Variables
        private const string GET_ALL_RACES_PAGED_SQL =
            "SELECT * FROM (SELECT ROWNUM AS rn" +
            "," + RACES_ID_FIELD +
            "," + RACES_STADIUM_ID_FIELD +
            "," + RACES_DATE_TIME_FIELD +
            "," + RACES_RACE_NUMBER_FIELD +
            "," + RACES_TRACK_LENGTH_FIELD +
            "," + RACES_GRADE_FIELD +
            "," + RACES_RACE_TYPE_FIELD +
            " FROM ( SELECT * FROM " + RACES_TABLE + " WHERE (@filter)" +
            " ORDER BY @sortBy @sortType) )" +
            " WHERE rn BETWEEN @start_rec AND @end_rec";

        private const string GET_RACES_BY_RACE_ID_SQL =
            "SELECT " + RACES_ID_FIELD +
            "," + RACES_STADIUM_ID_FIELD +
            "," + RACES_DATE_TIME_FIELD +
            "," + RACES_RACE_NUMBER_FIELD +
            "," + RACES_TRACK_LENGTH_FIELD +
            "," + RACES_GRADE_FIELD +
            "," + RACES_RACE_TYPE_FIELD +
            " FROM " + RACES_TABLE +
            " WHERE " + RACES_ID_FIELD + " = '@race_id'";

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
         * @param string filter The filter to apply to the query
         * @param int startRec The start index to return from the database
         * @param int endRec The end index to return from the database
         * @param string sortBy The sort field to apply to the query
         * @param string sortType The sort type to apply to the query
         * @return Race[] An array with all races.
         **/
        public static Race[] GetRacesPaged(string filter, int startRec, int endRec, string sortBy, string sortType)
        {
            List<Race> resultList = new List<Race>();
            OracleConnection connection = new OracleConnection(ConfigurationBLL.DBConnectionString);
            OracleDataReader sqlDataReader = null;
            try
            {
                //Open the connection
                connection.Open();
                //Setup the command to run                
                string strCommand = GET_ALL_RACES_PAGED_SQL;
                strCommand = strCommand.Replace("@start_rec", startRec.ToString());
                strCommand = strCommand.Replace("@end_rec", endRec.ToString());
                strCommand = strCommand.Replace("@filter", filter);
                strCommand = strCommand.Replace("@sortBy", sortBy);
                strCommand = strCommand.Replace("@sortType", sortType);

                Tools.printDebug(null, strCommand, null);

                OracleCommand command = new OracleCommand(strCommand);
                command.Connection = connection;
                //Runs the command
                sqlDataReader = command.ExecuteReader();

                //Fetches all rows
                while (sqlDataReader.Read())
                {
                    int race_id = Convert.ToInt32(sqlDataReader[RACES_ID_FIELD].ToString());
                    int stadium_std_id = Convert.ToInt32(sqlDataReader[RACES_STADIUM_ID_FIELD].ToString());
                    string date_time = sqlDataReader[RACES_DATE_TIME_FIELD].ToString();
                    int race_number = Convert.ToInt32(sqlDataReader[RACES_RACE_NUMBER_FIELD].ToString());
                    Decimal track_length = Tools.StringToDecimal(sqlDataReader[RACES_TRACK_LENGTH_FIELD].ToString());
                    string grade = sqlDataReader[RACES_GRADE_FIELD].ToString();
                    string race_type = sqlDataReader[RACES_RACE_TYPE_FIELD].ToString();

                    Race race = new Race(
                        race_id,
                        stadium_std_id,
                        date_time,
                        race_number,
                        track_length,
                        grade,
                        race_type);

                    resultList.Add(race);
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

            //Returns the list of fetched users in the form of an array
            return resultList.ToArray();
        }
        #endregion
    }
}
