using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace totoCafeWebServices
{
    public partial class deneme : System.Web.UI.Page
    {
        SqlConnection dbConnection;

        public deneme()
        {
            dbConnection = DBConnect.getConnection();

        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnResult_Click(object sender, EventArgs e)
        {
            InsertRequestTableViaQr(double.Parse(tbUserID.Text), double.Parse(tbCompanyID.Text),
                double.Parse(tbTableID.Text));
        }

        public void InsertRequestTableViaQr(double UserID, double CompanyID, double TableID)
        {
            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }
            int CostumerID = 0;
            int uid = (int)UserID;
            string query = "SELECT CostumerID FROM Costumer WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@UserID", uid);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                CostumerID = int.Parse(reader[0].ToString());
            }
            reader.Close();
            dbConnection.Close();

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }

            query = "INSERT INTO Request (CompanyID,CostumerID,TableID,Flag) VALUES (@CompanyID,@CostumerID,@TableID,@Flag)";
            int compid = (int)CompanyID;
            int tablid = (int)TableID;

            command = new SqlCommand(query, dbConnection);

            command.Parameters.AddWithValue("@CompanyID", compid);
            command.Parameters.AddWithValue("@CostumerID", CostumerID);
            command.Parameters.AddWithValue("@TableID", tablid);
            command.Parameters.AddWithValue("@Flag", 0);

            command.ExecuteNonQuery(); // insert data to "Request" Table
            lblStatus.Text = "BAŞARILI";
            dbConnection.Close();
        }
    }
}