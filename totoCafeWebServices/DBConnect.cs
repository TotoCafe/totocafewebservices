using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace totoCafeWebServices
{
    public class DBConnect
    {
        private static SqlConnection NewCon;
        private static string conStr = ConfigurationManager.ConnectionStrings["TotoCafeDB"].ConnectionString;

        public static SqlConnection getConnection()
        {
            NewCon = new SqlConnection(conStr);
            return NewCon;

        }
        public DBConnect()
        {

        }
    }
}