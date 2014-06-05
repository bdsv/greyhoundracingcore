using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Web.Configuration;
using GreyhoundBE.BO;

namespace GreyhoundBE.BLL
{
    public class ConfigurationBLL
    {
        public ConfigurationBLL()
        {
        }

        public static String GetAccessKey()
        {
            return Tools.DecryptData(System.Configuration.ConfigurationManager.AppSettings["accessKey"]);
        }

        public static String GetConnectionStringByRole(Role.ROLES roleid)
        {
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            ConnectionStringsSection section = (ConnectionStringsSection)config.GetSection("connectionStrings");

            //return Tools.DecryptData(section.ConnectionStrings["Greyhound.Properties.ConnectionString" + (int)roleid].ConnectionString);
            return Tools.DecryptData(section.ConnectionStrings["Greyhound.Properties.ConnectionString"].ConnectionString);
        }

        public static string DBConnectionString
        {
            get
            {
                //return Tools.DecryptData(System.Configuration.ConfigurationManager.ConnectionStrings["Greyhound.Properties.ConnectionString99"].ConnectionString);
                return Tools.DecryptData(System.Configuration.ConfigurationManager.ConnectionStrings["Greyhound.Properties.ConnectionString"].ConnectionString);
            }
            //set
            //{
            //    System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            //    ConnectionStringsSection section = (ConnectionStringsSection)config.GetSection("connectionStrings");
            //    section.ConnectionStrings["Greyhound.Properties.ConnectionString"].ConnectionString = Tools.EncryptData(value);
            //    config.Save();
            //}
        }
    }
}
