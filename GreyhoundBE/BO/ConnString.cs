using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    class ConnString
    {
        #region Private Member Variables

        private int m_nId;
        // The connection string
        private String m_strConnectionString;
        // The user of the database
        private String m_strUser;
        // The password of the user
        private String m_strPass;
        // The catalog of the connection string
        private String m_strCatalog;
        // The data source of the connection string
        private String m_strDataSource;

        #endregion

        #region Private Properties
        #endregion

        #region Private Methods


        #endregion

        #region Constructors

        /**
         * @brief Default constructor
         **/
        public ConnString()
        {
        }

        /**
         * @brief The class constructor
         * @param int n_id the id of the connection string
         * @param string str_connection_string the whole connection string
         * @param string str_user the user of the connection string
         * @param string str_pass the password of the user from the connection string
         * @param string str_catalog the catalog of the connection string
         * @param string str_data_source the data source of the connection string
         **/
        public ConnString(int n_id,
                            String str_connection_string,
                            String str_user,
                            String str_pass,
                            String str_catalog,
                            String str_data_source)
        {
            m_nId = n_id;

            if (str_connection_string.CompareTo("") == 0)
            {
                this.m_strConnectionString = CreateConnectionString(str_data_source, str_catalog, str_user, str_pass);
            }
            else
            {
                this.m_strConnectionString = str_connection_string;
            }

            this.m_strUser = str_user;
            this.m_strPass = str_pass;
            this.m_strCatalog = str_catalog;
            this.m_strDataSource = str_data_source;
        }
        #endregion

        #region Public Properties

        // The Connection string id
        public int Id
        {
            get
            {
                return this.m_nId;
            }
            set
            {
                this.m_nId = value;
            }
        }

        // The Connection string
        public string ConnectionString
        {
            get
            {
                return this.m_strConnectionString;
            }
            set
            {
                this.m_strConnectionString = value;
            }
        }

        // The user of the connection string
        public string User
        {
            get
            {
                return this.m_strUser;
            }
            set
            {
                this.m_strUser = value;
            }
        }

        // The Password of the connection string
        public string Pass
        {
            get
            {
                return this.m_strPass;
            }
            set
            {
                this.m_strPass = value;
            }
        }

        // The catalog of the connection string
        public string Catalog
        {
            get
            {
                return this.m_strCatalog;
            }
            set
            {
                this.m_strCatalog = value;
            }
        }

        // The datasource of the connection string
        public string DataSource
        {
            get
            {
                return this.m_strDataSource;
            }
            set
            {
                this.m_strDataSource = value;
            }
        }

        #endregion

        #region Public Methods

        /**
         * @brief Creates a connection string
         * @param string str_host the host of the connection string
         * @param string str_service_name the service name of the connection string
         * @param string str_user the user of the connection string
         * @param string str_pwd the password of the user from the connection string
         **/
        public String CreateConnectionString(String str_host, String str_service_name, String str_user, String str_pwd)
        {
            string strDbConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=[_HOST_])(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=[_SERVICE_NAME_])));User Id=[_USER_ID_];Password=[_PASSWORD_];";
            strDbConnectionString = strDbConnectionString.Replace("[_HOST_]", str_host);
            strDbConnectionString = strDbConnectionString.Replace("[_SERVICE_NAME_]", str_service_name);
            strDbConnectionString = strDbConnectionString.Replace("[_USER_ID_]", str_user);
            strDbConnectionString = strDbConnectionString.Replace("[_PASSWORD_]", str_pwd);
            return strDbConnectionString;
        }

        /**
         * @brief Decrypts a connection string
         * @param string connectionString the connection string to decrypt
         **/
        public String DecryptConnectionString(String connectionString)
        {
            return Tools.DecryptData(connectionString);
        }
        #endregion
    }
}
