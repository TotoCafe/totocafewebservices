﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
namespace totoCafeWebServices
{
    public partial class deneme : System.Web.UI.Page
    {
        SqlConnection dbConnection;
        ServiceAPI myService = new ServiceAPI();
        public deneme()
        {
            dbConnection = DBConnect.getConnection();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
               
               DataTable dtCategories = myService.getCategoriesOfCompany(13);
               GridViewCategory.DataSource = dtCategories;
                GridViewCategory.DataBind();
              
            }
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

        protected void Button1_Click(object sender, EventArgs e)
        {

            string BirthDate = "1993-10-07";
            DateTime birthDateTime = Convert.ToDateTime(BirthDate);


            tbStringToDate.Text = birthDateTime.ToString();
        }

        protected void btnGetProduct_Click(object sender, EventArgs e)
        {
            int catID = int.Parse(tbCategoryID.Text);
            //Category IDs : 26 , 27 , 28 , 29 , 30 
            DataTable dtProductsOfCategory = myService.getProductViaCategory(catID);
            GridViewProduct.DataSource = dtProductsOfCategory;
            GridViewProduct.DataBind();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            ServiceAPI servis = new ServiceAPI();

            bool result = servis.UserAuthentication(tbEmail.Text, tbPassword.Text);

            lblAuth.Text = result.ToString();
        }
    }
}