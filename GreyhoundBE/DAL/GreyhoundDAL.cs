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
    class GreyhoundDAL : DatabaseSchema
    {
        #region Private Member Variables

        //Query to get all users from DB
        //private const string GET_ALL_GREYHOUNDS_SQL =
        //    "SELECT " + GREYHOUNDS_GREYHOUND_ID_FIELD +
        //    " ," + GREYHOUNDS_NAME_FIELD +
        //    " ," + GREYHOUNDS_TRAINER_FIELD +
        //    " ," + GREYHOUNDS_BIRTH_DATE_FIELD +
        //    " FROM " + GREYHOUNDS_TABLE + //;
        //    " WHERE " + GREYHOUNDS_GREYHOUND_ID_FIELD + " < 73000";

        // SELECT * FROM ( SELECT ROWNUM as rn,grey_id,name,trainer,birth_date FROM
        //    ( SELECT * FROM GREYHOUNDS WHERE trainer = 'G Oswald' ORDER BY grey_id ASC) ) WHERE rn BETWEEN 1 AND 20;
        //private const string GET_ALL_GREYHOUNDS_PAGED_SQL =
        //    "SELECT * FROM (SELECT ROWNUM AS rn" +
        //    "," + GREYHOUNDS_GREYHOUND_ID_FIELD +
        //    "," + GREYHOUNDS_NAME_FIELD +
        //    "," + GREYHOUNDS_TRAINER_FIELD +
        //    "," + GREYHOUNDS_BIRTH_DATE_FIELD +
        //    " ," + GREYHOUNDS_SCORE_FIELD +
        //    " FROM ( SELECT * FROM " + GREYHOUNDS_TABLE + " WHERE (@filter)" +
        //    " ORDER BY @sortBy @sortType) )" +
        //    " WHERE rn BETWEEN @start_rec AND @end_rec";

        //private const string GET_GREYHOUND_BY_GREYHOUND_ID_SQL =
        //    "SELECT " + GREYHOUNDS_GREYHOUND_ID_FIELD +
        //    " ," + GREYHOUNDS_NAME_FIELD +
        //    " ," + GREYHOUNDS_TRAINER_FIELD +
        //    " ," + GREYHOUNDS_BIRTH_DATE_FIELD +
        //    " ," + GREYHOUNDS_SCORE_FIELD +
        //    " FROM " + GREYHOUNDS_TABLE +
        //    " WHERE " + GREYHOUNDS_GREYHOUND_ID_FIELD + " = '@grey_id'";

        /*
        SELECT DISTINCT(Races_Details.greyhounds_grey_id), greyhounds.* 
        FROM Races, Races_Details, Greyhounds
        WHERE TO_DATE(TO_CHAR(Races.date_time, 'yyyy/mm/dd'), 'yyyy/mm/dd') = TO_DATE('2013-10-04', 'yyyy/mm/dd')
        AND Races_Details.greyhounds_grey_id = greyhounds.grey_id AND races_details.races_race_id = Races.race_id
        ORDER BY greyhounds.grey_id ASC;
        */
        private const string GET_GREYHOUNDS_RACING_AT_DATE_SQL =
            "SELECT DISTINCT(" +RACES_DETAILS_GREYHOUNDS_GREY_ID_FIELD + "), " + GREYHOUNDS_TABLE + ".*" +
            " FROM " + RACES_TABLE + "," + RACES_DETAILS_TABLE + "," + GREYHOUNDS_TABLE +
            " WHERE TO_DATE(TO_CHAR(" + RACES_TABLE + "." + RACES_DATE_TIME_FIELD +
            ", 'yyyy/mm/dd'), 'yyyy/mm/dd') = TO_DATE('@date_time', 'yyyy/mm/dd')" +
            " AND " + RACES_DETAILS_TABLE + "." + RACES_DETAILS_GREYHOUNDS_GREY_ID_FIELD + "=" + GREYHOUNDS_TABLE + "." + GREYHOUNDS_GREYHOUND_ID_FIELD +
            " AND " + RACES_DETAILS_TABLE + "." + RACES_DETAILS_RACES_RACE_ID_FIELD + "=" + RACES_TABLE + "." + RACES_ID_FIELD +
            " ORDER BY " + GREYHOUNDS_TABLE + "." + GREYHOUNDS_GREYHOUND_ID_FIELD + " ASC";
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        private static List<Greyhound> executeQuery(User user, String strCommand)
        {
            List<Greyhound> resultList = new List<Greyhound>();

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
                    int grey_id = Convert.ToInt32(sqlDataReader[GREYHOUNDS_GREYHOUND_ID_FIELD].ToString());
                    string name = sqlDataReader[GREYHOUNDS_NAME_FIELD].ToString();
                    string trainer = sqlDataReader[GREYHOUNDS_TRAINER_FIELD].ToString();
                    string birth_date = sqlDataReader[GREYHOUNDS_BIRTH_DATE_FIELD].ToString();
                    string score = sqlDataReader[GREYHOUNDS_SCORE_FIELD].ToString();

                    Greyhound greyhound = new Greyhound(
                        grey_id,
                        name,
                        trainer,
                        birth_date,
                        score
                        );

                    resultList.Add(greyhound);
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
        ///**
        // * @brief Returns all greyhounds from DB as pages
        // * @return Greyhound[] An array with all greyhounds.
        // **/
        //public static Greyhound[] GetGreyhoundsPaged(string filter, int startRec, int endRec, string sortBy, string sortType)
        //{            
        //    //Setup the command to run                
        //    string strCommand = GET_ALL_GREYHOUNDS_PAGED_SQL;
        //    strCommand = strCommand.Replace("@start_rec", startRec.ToString());
        //    strCommand = strCommand.Replace("@end_rec", endRec.ToString());
        //    strCommand = strCommand.Replace("@filter", filter);
        //    strCommand = strCommand.Replace("@sortBy", sortBy);
        //    strCommand = strCommand.Replace("@sortType", sortType);

        //    List<Greyhound> resultList = executeQuery(strCommand);
        //    Tools.printDebug(null, strCommand, null);

        //    return resultList.ToArray();
        //}

        /**
        * @brief Returns all greyhounds from DB
        * @return Greyhound[] An array with all greyhounds.
        **/
        public static Greyhound[] GetGreyhoundsRunningAtDate(User user, string date_time)
        {
            //Setup the command to run                
            string strCommand = GET_GREYHOUNDS_RACING_AT_DATE_SQL;
            strCommand = strCommand.Replace("@date_time", date_time.ToString());

            List<Greyhound> resultList = executeQuery(user, strCommand);
            Tools.printDebug(null, strCommand, null);

            return resultList.ToArray();
        }
        #endregion
    }
}
