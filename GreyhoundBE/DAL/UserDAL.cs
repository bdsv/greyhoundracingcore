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
    /**
     * @brief This class defines the data access layer (DAL) for the database users.
     **/
    class UserDAL : DatabaseSchema
    {
        #region Private Member Variables

        //Query to get all users from DB
        private const string GET_ALL_USERS_PAGED_SQL =
            "SELECT * FROM (SELECT ROWNUM AS rn" +
            " ," + USERS_USER_ID_FIELD +
            " ," + USERS_ROLE_ID_FIELD +
            " ," + USERS_NAME_FIELD +
            " ," + USERS_EMAIL_FIELD +
            " ," + USERS_MOBILE_FIELD +
            " ," + USERS_ADDRESS_FIELD +
            " ," + USERS_PAYPAL_ID_FIELD +
            " ," + USERS_BETFAIR_ID_FIELD +
            " FROM ( SELECT * FROM " + USERS_TABLE + " WHERE (@filter)" +
            " ORDER BY @sortBy @sortType) )" +
            " WHERE rn BETWEEN @start_rec AND @end_rec";

        private const string GET_USER_BY_USER_EMAIL_SQL =
            "SELECT " + USERS_USER_ID_FIELD +
            " ," + USERS_ROLE_ID_FIELD +
            " ," + USERS_NAME_FIELD +
            " ," + USERS_EMAIL_FIELD +
            " ," + USERS_MOBILE_FIELD +
            " ," + USERS_ADDRESS_FIELD +
            " ," + USERS_PAYPAL_ID_FIELD +
            " ," + USERS_BETFAIR_ID_FIELD +
            " FROM " + USERS_TABLE +
            " WHERE UPPER(" + USERS_EMAIL_FIELD + ") = '@user_email'";

        private const string GET_USER_BY_USER_ID_SQL =
            "SELECT " + USERS_USER_ID_FIELD +
            " ," + USERS_ROLE_ID_FIELD +
            " ," + USERS_NAME_FIELD +
            " ," + USERS_EMAIL_FIELD +
            " ," + USERS_MOBILE_FIELD +
            " ," + USERS_ADDRESS_FIELD +
            " ," + USERS_PAYPAL_ID_FIELD +
            " ," + USERS_BETFAIR_ID_FIELD +
            " FROM " + USERS_TABLE +
            " WHERE " + USERS_USER_ID_FIELD + " = @user_id";

        //Query to insert a user in the db
        private const string REGISTER_USER =
            "UPDATE " + USERS_TABLE +
            " SET " + USERS_REGISTERED_FIELD +
            " = '@registered' WHERE " + USERS_USER_ID_FIELD +
            " = '@user_id' AND " + USERS_EMAIL_FIELD +
            " = '@email'";

        //Query to insert a user in the db
        private const string INSERT_USER =
            "INSERT INTO " + USERS_TABLE +
            " (" + USERS_USER_ID_FIELD +
            " ," + USERS_PASSWORD_FIELD +
            " ," + USERS_ROLE_ID_FIELD +
            " ," + USERS_NAME_FIELD +
            " ," + USERS_EMAIL_FIELD +
            " ," + USERS_REGISTERED_FIELD +
            " ," + USERS_MOBILE_FIELD +
            " ," + USERS_ADDRESS_FIELD +
            " ," + USERS_PAYPAL_ID_FIELD +
            " ," + USERS_BETFAIR_ID_FIELD + ")" +
            " VALUES" +
            " (:user_id," +
            " :password," +
            " :role_id," +
            " :name," +
            " :email," +
            " :registered," +
            " :mobile," +
            " :address," +
            " :paypal_id," +
            " :betfair_id)";

        //Query to insert a user in the db
        private const string UPDATE_USER_PASSWORD =
            "UPDATE " + USERS_TABLE +
            " SET " + USERS_PASSWORD_FIELD + "=:password" +
            "," + USERS_NAME_FIELD + "=:name" +
            "," + USERS_MOBILE_FIELD + "=:mobile" +
            "," + USERS_ADDRESS_FIELD + "=:address" +
            "," + USERS_PAYPAL_ID_FIELD + "=:paypal_id" +
            "," + USERS_BETFAIR_ID_FIELD + "=:betfair_id" +
            " WHERE " + USERS_USER_ID_FIELD + "=:user_id";

        //Query to insert a user in the db
        private const string UPDATE_USER_NO_PASSWORD =
            "UPDATE " + USERS_TABLE +
            " SET " + USERS_NAME_FIELD + "=:name" +
            "," + USERS_MOBILE_FIELD + "=:mobile" +
            "," + USERS_ADDRESS_FIELD + "=:address" +
            "," + USERS_PAYPAL_ID_FIELD + "=:paypal_id" +
            "," + USERS_BETFAIR_ID_FIELD + "=:betfair_id" +
            " WHERE " + USERS_USER_ID_FIELD + "=:user_id";

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
         * @brief Returns all users from the DB
         * @param string filter The filter to apply to the query
         * @param int startRec The start index to return from the database
         * @param int endRec The end index to return from the database
         * @param string sortBy The sort field to apply to the query
         * @param string sortType The sort type to apply to the query
         * @return User[] An array with all users.
         **/
        public static User[] GetUsersPaged(string filter, int startRec, int endRec, string sortBy, string sortType)
        {
            List<User> resultList = new List<User>();
            OracleConnection connection = new OracleConnection(ConfigurationBLL.DBConnectionString);
            OracleDataReader sqlDataReader = null;
            try
            {
                //Open the connection
                connection.Open();
                //Setup the command to run
                //Setup the command to run                
                string strCommand = GET_ALL_USERS_PAGED_SQL;
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
                    int user_id = Convert.ToInt32(sqlDataReader[USERS_USER_ID_FIELD].ToString());
                    Role.ROLES role_id = (Role.ROLES) Convert.ToInt32(sqlDataReader[USERS_ROLE_ID_FIELD].ToString());
                    string name = sqlDataReader[USERS_NAME_FIELD].ToString();
                    string email = sqlDataReader[USERS_EMAIL_FIELD].ToString();
                    string mobile = sqlDataReader[USERS_MOBILE_FIELD].ToString();
                    string address = sqlDataReader[USERS_ADDRESS_FIELD].ToString();
                    string paypal_id = sqlDataReader[USERS_PAYPAL_ID_FIELD].ToString();
                    string betfair_id = sqlDataReader[USERS_BETFAIR_ID_FIELD].ToString();

                    User user = new User(
                        user_id,
                        role_id,
                        name,
                        email,
                        mobile,
                        address,
                        paypal_id,
                        betfair_id
                        );

                    resultList.Add(user);
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

        /**
         * @brief Returns the user with the identified id from DB
         * @param string user_id The user_id filter to apply to the query 
         * @return User A User object array with the id defined in the query
         **/
        public static User GetUserByEmail(string user_email)
        {
            User user = null;
            OracleConnection connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(Role.ROLES.CONNECTION_USER_ROLE));
            OracleDataReader sqlDataReader = null;
            try
            {
                //Open the connection
                connection.Open();
                //Setup the command to run                
                string strCommand = GET_USER_BY_USER_EMAIL_SQL;
                strCommand = strCommand.Replace("@user_email", user_email.ToUpper());

                Tools.printDebug(null, strCommand, null);

                OracleCommand command = new OracleCommand(strCommand);
                command.Connection = connection;
                //Runs the command
                sqlDataReader = command.ExecuteReader();

                //Fetches all rows
                while (sqlDataReader.Read())
                {
                    int userid = Convert.ToInt32(sqlDataReader[USERS_USER_ID_FIELD].ToString());
                    Role.ROLES role_id = (Role.ROLES)Convert.ToInt32(sqlDataReader[USERS_ROLE_ID_FIELD].ToString());
                    string name = sqlDataReader[USERS_NAME_FIELD].ToString();
                    string email = sqlDataReader[USERS_EMAIL_FIELD].ToString();
                    string mobile = sqlDataReader[USERS_MOBILE_FIELD].ToString();
                    string address = sqlDataReader[USERS_ADDRESS_FIELD].ToString();
                    string paypal_id = sqlDataReader[USERS_PAYPAL_ID_FIELD].ToString();
                    string betfair_id = sqlDataReader[USERS_BETFAIR_ID_FIELD].ToString();

                    user = new User(
                        userid,
                        role_id,
                        name,
                        email,
                        mobile,
                        address,
                        paypal_id,
                        betfair_id
                    );

                    break;
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

            return user;
        }

        /**
         * @brief Returns the user with the identified id from DB
         * @param string user_id The user_id filter to apply to the query 
         * @return User A User object array with the id defined in the query
         **/
        public static User GetUserById(string user_id)
        {
            User user = null;
            OracleConnection connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(Role.ROLES.CONNECTION_USER_ROLE));
            OracleDataReader sqlDataReader = null;
            try
            {
                //Open the connection
                connection.Open();
                //Setup the command to run                
                string strCommand = GET_USER_BY_USER_ID_SQL;
                strCommand = strCommand.Replace("@user_id", user_id);

                Tools.printDebug(null, strCommand, null);

                OracleCommand command = new OracleCommand(strCommand);
                command.Connection = connection;
                //Runs the command
                sqlDataReader = command.ExecuteReader();

                //Fetches all rows
                while (sqlDataReader.Read())
                {
                    int userid = Convert.ToInt32(sqlDataReader[USERS_USER_ID_FIELD].ToString());
                    Role.ROLES role_id = (Role.ROLES)Convert.ToInt32(sqlDataReader[USERS_ROLE_ID_FIELD].ToString());
                    string name = sqlDataReader[USERS_NAME_FIELD].ToString();
                    string email = sqlDataReader[USERS_EMAIL_FIELD].ToString();
                    string mobile = sqlDataReader[USERS_MOBILE_FIELD].ToString();
                    string address = sqlDataReader[USERS_ADDRESS_FIELD].ToString();
                    string paypal_id = sqlDataReader[USERS_PAYPAL_ID_FIELD].ToString();
                    string betfair_id = sqlDataReader[USERS_BETFAIR_ID_FIELD].ToString();

                    user = new User(
                        userid,
                        role_id,
                        name,
                        email,
                        mobile,
                        address,
                        paypal_id,
                        betfair_id
                    );

                    break;
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

            return user;
        }

        public static int ConfirmUserRegistration(String user_id, String email)
        {
            int result = -1;

            bool error = false;
            OracleConnection connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(Role.ROLES.CONNECTION_USER_ROLE));
            OracleTransaction transaction = null;
            try
            {
                //Open the connection
                connection.Open();
                //Start a local transaction
                transaction = connection.BeginTransaction();

                //First we add the user to the database
                //Setup the command to run
                string strCommand = REGISTER_USER;
                strCommand = strCommand.Replace("@user_id", user_id);
                strCommand = strCommand.Replace("@email", email);
                strCommand = strCommand.Replace("@registered", "1");

                Tools.printDebug(strCommand);

                OracleCommand command = new OracleCommand(strCommand);
                command.Connection = connection;
                command.Transaction = transaction;
                
                //Run the command
                result = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                error = true;
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new Exception(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            finally
            {
                if (transaction != null)
                {
                    if (error)
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
                //Closes the datareader and connection to the database
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;
        }

        /**
         * @brief Adds a user to the database
         * @param User user The user to add
         **/
        public static int RegisterNewUser(User user)
        {
            int lastInsertedId = -1;

            bool error = false;
            OracleConnection connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(Role.ROLES.CONNECTION_USER_ROLE));
            OracleTransaction transaction = null;
            try
            {
                //Open the connection
                connection.Open();
                //Start a local transaction
                transaction = connection.BeginTransaction();

                //First we add the user to the database
                //Setup the command to run
                OracleCommand command = new OracleCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                //Get the next id
                command.CommandText = DatabaseSchema.SELECT_USERS_NEXT_ID;
                int user_id = Convert.ToInt32(command.ExecuteScalar());

                if (user_id > 0)
                {
                    command.Parameters.Clear(); 
                    command.CommandText = INSERT_USER;                    
                    
                    //First we set the parameters
                    command.Parameters.Add("user_id", user_id);
                    command.Parameters.Add("password", user.Password);
                    command.Parameters.Add("role_id", (int)user.Role_Id);
                    command.Parameters.Add("name", user.Name);
                    command.Parameters.Add("email", user.Email);
                    command.Parameters.Add("registered", DatabaseSchema.USER_NOT_REGISTERED);
                    command.Parameters.Add("mobile", user.Mobile);
                    command.Parameters.Add("address", user.Address);
                    command.Parameters.Add("paypal_id", user.Paypal_Id);
                    command.Parameters.Add("betfair_id", user.Betfair_Id);

                    //Run the command
                    command.ExecuteNonQuery();
                    lastInsertedId = user_id;
                }
            }
            catch (Exception exception)
            {
                error = true;
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new Exception(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            finally
            {
                if (transaction != null)
                {
                    if (error)
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
                //Closes the datareader and connection to the database
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return lastInsertedId;
        }


        /**
         * @brief Adds a user to the database
         * @param User user The user to add
         **/
        public static int UpdateUserSettings(User user)
        {
            int result = -1;

            bool error = false;
            OracleConnection connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(Role.ROLES.CONNECTION_USER_ROLE));
            OracleTransaction transaction = null;
            try
            {
                //Open the connection
                connection.Open();
                //Start a local transaction
                transaction = connection.BeginTransaction();

                //First we add the user to the database
                //Setup the command to run
                OracleCommand command = new OracleCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                if (user.User_Id > 0)
                {
                    command.Parameters.Clear();
                    command.BindByName = true;
                    if (user.Password != null && user.Password.Length > 0)
                    {
                        command.CommandText = UPDATE_USER_PASSWORD;
                        command.Parameters.Add(":password", user.Password);
                    }
                    else
                    {
                        command.CommandText = UPDATE_USER_NO_PASSWORD;
                    }

                    //First we set the parameters
                    command.Parameters.Add(":user_id", user.User_Id);                    
                    command.Parameters.Add(":name", user.Name);
                    command.Parameters.Add(":mobile", user.Mobile);
                    command.Parameters.Add(":address", user.Address);
                    command.Parameters.Add(":paypal_id", user.Paypal_Id);
                    command.Parameters.Add(":betfair_id", user.Betfair_Id);

                    Tools.printDebug(command.CommandText.ToString());
                    Tools.printDebug(command.Parameters.ToString());

                    //Run the command
                    result = command.ExecuteNonQuery();
                    Tools.printDebug("UPDATE USER RESULT: " + result);
                }
            }
            catch (Exception exception)
            {
                error = true;
                Tools.printDebug(new System.Diagnostics.StackFrame(), "", exception);
                throw new Exception(Resources.CANNOT_ACCESS_DATABASE_EXCEPTION_MESSAGE, exception);
            }
            finally
            {
                if (transaction != null)
                {
                    if (error)
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
                //Closes the datareader and connection to the database
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;
        }

        /**
         * @brief Try to login with the credentials provided
         * @return True if login was sucessful, False otherwise
         */
        public static bool LoginUser(String user_email, String password)
        {
            bool result = false;
            try
            {
                using (var connection = new OracleConnection(ConfigurationBLL.GetConnectionStringByRole(Role.ROLES.ADMINISTRATOR_USER_ROLE)))
                {
                    using (var Command = new OracleCommand())
                    {
                        Command.Connection = connection;
                        Command.CommandText ="LOGINUSER";
                        Command.CommandType = CommandType.StoredProcedure;

                        using (var param = new OracleParameter())
                        {
                            param.OracleDbType = OracleDbType.Int32;
                            param.Direction = ParameterDirection.ReturnValue;
                            param.ParameterName = "return";
                            Command.Parameters.Add(param);
                        }
                        using (var email_param = new OracleParameter())
                        {
                            email_param.OracleDbType = OracleDbType.Varchar2;
                            email_param.Direction = ParameterDirection.Input;
                            email_param.ParameterName = "email";
                            email_param.Value = user_email;
                            Command.Parameters.Add(email_param);
                        }

                        using (var password_param = new OracleParameter())
                        {
                            password_param.OracleDbType = OracleDbType.Varchar2;
                            password_param.Direction = ParameterDirection.Input;
                            password_param.ParameterName = "password";
                            password_param.Value = password;
                            Command.Parameters.Add(password_param);
                        }

                        using (var key_param = new OracleParameter())
                        {
                            key_param.OracleDbType = OracleDbType.Varchar2;
                            key_param.Direction = ParameterDirection.Input;
                            key_param.ParameterName = "mKEY";
                            key_param.Value = ConfigurationBLL.GetAccessKey();
                            Command.Parameters.Add(key_param);
                        }

                        connection.Open();
                        Command.ExecuteNonQuery();

                        if ( Command.Parameters["return"].DbType == System.Data.DbType.Int32 )
                        {
                            int res = Int32.Parse((Command.Parameters["return"].Value).ToString());
                            Tools.printDebug("Login Result: " + res);
                            result = (res > 0 ? true : false);
                        }
                        else
                        {
                            Tools.printDebug("Invalid Login");
                            result = false;
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
