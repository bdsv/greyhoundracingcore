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
    class RaceEventDAL : DatabaseSchema
    {
        #region Private Member Variables
        private const string GET_ALL_RACE_EVENTS_AT_DATE_SQL =
            "SELECT " + STADIUMS_TABLE + ".*, " + RACES_TABLE + ".*" +
            " FROM " + STADIUMS_TABLE + ", " + RACES_TABLE +
            " WHERE TO_DATE(TO_CHAR(" + RACES_TABLE + "." + RACES_DATE_TIME_FIELD + ", 'yyyy/mm/dd'), 'yyyy/mm/dd') =" +
            " TO_DATE('@date_time', 'yyyy/mm/dd')" +
            " AND " + STADIUMS_TABLE + "." + STADIUMS_STADIUM_ID_FIELD + " = " + RACES_TABLE + "." + RACES_STADIUM_ID_FIELD +
            " ORDER BY " + STADIUMS_TABLE + "." + STADIUMS_STADIUM_ID_FIELD + ", " + RACES_TABLE + "." + RACES_DATE_TIME_FIELD;
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
        public static RaceEvent[] GetRaceEventsAtDate(User user, string m_date_time)
        {
            List<RaceEvent> resultList = new List<RaceEvent>();
            OracleConnection connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(user.Role_Id));
            OracleDataReader sqlDataReader = null;
            try
            {
                //Open the connection
                connection.Open();
                //Setup the command to run                
                string strCommand = GET_ALL_RACE_EVENTS_AT_DATE_SQL;
                strCommand = strCommand.Replace("@date_time", m_date_time.ToString());

                Tools.printDebug(null, strCommand, null);

                OracleCommand command = new OracleCommand(strCommand);
                command.Connection = connection;
                //Runs the command
                sqlDataReader = command.ExecuteReader();

                //Fetches all rows
                int previous_std_id = -1;
                int previous_race_id = -1;
                
                Stadium stadium = null;
                Race race = null;
                while (sqlDataReader.Read())
                {                   
                    if (stadium != null && race != null)
                    {
                        RaceEvent raceEvent = new RaceEvent(stadium, race);
                        resultList.Add(raceEvent);

                        previous_std_id = -1;
                        previous_race_id = -1;
                        stadium = null;
                        race = null;
                    }

                    int std_id = Convert.ToInt32(sqlDataReader[STADIUMS_STADIUM_ID_FIELD].ToString());                    
                    if( previous_std_id != std_id ) {
                        previous_std_id = std_id;

                        string stadium_name = sqlDataReader[STADIUMS_NAME_FIELD].ToString();
                        string num_tracks = sqlDataReader[STADIUMS_NUM_TRACKS_FIELD].ToString();
                        string city = sqlDataReader[STADIUMS_CITY_FIELD].ToString();
                        string country = sqlDataReader[STADIUMS_COUNTRY_FIELD].ToString();

                        stadium = new Stadium(
                            std_id,
                            stadium_name,
                            num_tracks,
                            city,
                            country
                            );
                    }

                    int race_id = Convert.ToInt32(sqlDataReader[RACES_ID_FIELD].ToString());
                    if( previous_race_id != race_id ) {
                        previous_race_id = race_id;

                        int stadium_std_id = Convert.ToInt32(sqlDataReader[RACES_STADIUM_ID_FIELD].ToString());
                        string date_time = sqlDataReader[RACES_DATE_TIME_FIELD].ToString();
                        int race_number = Convert.ToInt32(sqlDataReader[RACES_RACE_NUMBER_FIELD].ToString());
                        Decimal track_length = Tools.StringToDecimal(sqlDataReader[RACES_TRACK_LENGTH_FIELD].ToString());
                        string grade = sqlDataReader[RACES_GRADE_FIELD].ToString();
                        string race_type = sqlDataReader[RACES_RACE_TYPE_FIELD].ToString();

                        race = new Race(
                            race_id,
                            stadium_std_id,
                            date_time,
                            race_number,
                            track_length,
                            grade,
                            race_type);
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
