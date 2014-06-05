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
    class StadiumDAL : DatabaseSchema
    {
        #region Private Member Variables
        private const string GET_ALL_STADIUMS_PAGED_SQL =
            "SELECT * FROM (SELECT ROWNUM AS rn" +
            "," + STADIUMS_STADIUM_ID_FIELD +
            "," + STADIUMS_NAME_FIELD +
            "," + STADIUMS_NUM_TRACKS_FIELD +
            "," + STADIUMS_CITY_FIELD +
            "," + STADIUMS_COUNTRY_FIELD +
            " FROM ( SELECT * FROM " + STADIUMS_TABLE + " WHERE (@filter)" +
            " ORDER BY @sortBy @sortType) )" +
            " WHERE rn BETWEEN @start_rec AND @end_rec";

        private const string GET_STADIUM_BY_STADIUM_ID_SQL =
            "SELECT " + STADIUMS_STADIUM_ID_FIELD +
            " ," + STADIUMS_NAME_FIELD +
            " ," + STADIUMS_NUM_TRACKS_FIELD +
            " ," + STADIUMS_CITY_FIELD +
            " ," + STADIUMS_COUNTRY_FIELD +
            " FROM " + STADIUMS_TABLE +
            " WHERE " + STADIUMS_STADIUM_ID_FIELD + " = '@std_id'";
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        private static List<Stadium> executeQuery(String strCommand)
        {
            List<Stadium> resultList = new List<Stadium>();
            OracleConnection connection = new OracleConnection(ConfigurationBLL.DBConnectionString);
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
                    int std_id = Convert.ToInt32(sqlDataReader[STADIUMS_STADIUM_ID_FIELD].ToString());
                    string name = sqlDataReader[STADIUMS_NAME_FIELD].ToString();
                    string num_tracks = sqlDataReader[STADIUMS_NUM_TRACKS_FIELD].ToString();
                    string city = sqlDataReader[STADIUMS_CITY_FIELD].ToString();
                    string country = sqlDataReader[STADIUMS_COUNTRY_FIELD].ToString();

                    Stadium stadium = new Stadium(
                        std_id,
                        name,
                        num_tracks,
                        city,
                        country
                        );

                    resultList.Add(stadium);
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
         * @brief Returns all stadiums from DB
         * @param string filter The filter to apply to the query
         * @param int startRec The start index to return from the database
         * @param int endRec The end index to return from the database
         * @param string sortBy The sort field to apply to the query
         * @param string sortType The sort type to apply to the query
         * @return Stadium[] An array with all stadiums.
         **/
        public static Stadium[] GetStadiumsPaged(string filter, int startRec, int endRec, string sortBy, string sortType)
        {
            string strCommand = GET_ALL_STADIUMS_PAGED_SQL;
            strCommand = strCommand.Replace("@start_rec", startRec.ToString());
            strCommand = strCommand.Replace("@end_rec", endRec.ToString());
            strCommand = strCommand.Replace("@filter", filter);
            strCommand = strCommand.Replace("@sortBy", sortBy);
            strCommand = strCommand.Replace("@sortType", sortType);

            Tools.printDebug(null, strCommand, null);

            List<Stadium> resultList = executeQuery(strCommand);
            return resultList.ToArray<Stadium>();
        }

        /**
         * @brief Returns the stadium with the identified id from DB
         * @param string std_id The std_id filter to apply to the query 
         * @return Stadium A Stadium object array with the id defined in the query
         **/
        public static Stadium GetStadiumById(string std_id)
        {
            string strCommand = GET_STADIUM_BY_STADIUM_ID_SQL;
            strCommand = strCommand.Replace("@std_id", std_id);

            Tools.printDebug(null, strCommand, null);

            List<Stadium> resultList = executeQuery(strCommand);

            return resultList.ElementAtOrDefault(0);
        }
        #endregion
    }
}
