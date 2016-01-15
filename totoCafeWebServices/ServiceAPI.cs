using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace totoCafeWebServices
{
    public class ServiceAPI:IServiceAPI
    {
        SqlConnection dbConnection;

        public ServiceAPI()
        {
            dbConnection = DBConnect.getConnection();

        }

        #region Anonymous Part

        /// <summary>
        /// Anonim girişler için Anonim kullanıcı denetimi, veritabanı kayıt işlemi..
        /// </summary>
        /// <param name="DeviceID"></param>
        public void AnonymousLogin(string DeviceID)
        {
            //Authentication == TRUE, Kayıtlı bir Anonim var!
            if (AnonymousAuthentication(DeviceID))
            {
                double AnonymousID = GetAnonymousID(DeviceID);

                //Get Costumer Details
                //GetCostumerDetails(AnonymousID); Metotu burda mı çağırayım yoksa Android arayüzünde ayrı olarak mı çağırayım daha karar vermedim. Şimdilik Comment Kalsın.

            }
            //Authentication == FALSE, Anonim olarak ilk kez giriyor. Anonim ve Costumer tablolarına kayıt işlemi.
            else
            {
                CreateNewAnonymous(DeviceID);
            }
        }
        /// <summary>
        /// Getting AnonymousID by using DeviceID
        /// </summary>
        /// <param name="DeviceID"></param>
        /// <returns></returns>
        public double GetAnonymousID(string DeviceID)
        {
            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }
            int AnonymousID = 0;

            string query = "SELECT AnonymousID FROM Anonymous WHERE DeviceID=@DeviceID";

            SqlCommand command = new SqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@DeviceID", DeviceID);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    AnonymousID = int.Parse(reader["AnonymousID"].ToString());
                }
            }

            reader.Close();
            dbConnection.Close();

            return AnonymousID;
        }

        /// <summary>
        /// Create New Anonymous to Anonymous Table
        /// </summary>
        /// <param name="DeviceID"></param>
        public void CreateNewAnonymous(string DeviceID)
        {
            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }
            int AnonymousID = 0;

            string query = "INSERT INTO Anonymous (DeviceID) VALUES (@DeviceID) SET @ID = SCOPE_IDENTITY()";

            SqlCommand command = new SqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@DeviceID", DeviceID);

            command.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

            command.ExecuteNonQuery();

            AnonymousID = Convert.ToInt32(command.Parameters["@ID"].Value);

            //Sırada AnonymousID ve AnonymousTypeID yi kullanıp Anonymous'ı Costumer tablosuna eklemekte.
            //UserTypeID = 1  == ANONYMOUS  || UserTypeID=2 NOTANONYMOUS
            int UserTypeID = 1; //anonim

            //Costumer oluşturma..
            InsertUserToCostumerTable(AnonymousID, UserTypeID);

            dbConnection.Close();
        }

        /// <summary>
        /// Anonymous Tablosuna giderek DeviceID si kayıtlı olan bir Anonim giriş var mı yok mu kontrol eder
        /// Boolean sonuç döndürür.
        /// Metot AnonymousLogin metotunda çağırıl ve ona göre Insert ya da Select işlemi yapılır!
        /// </summary>
        /// <param name="DeviceID"></param>
        /// <returns></returns>
        public bool AnonymousAuthentication(string DeviceID)
        {
            bool auth = false;

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }

            string query = "SELECT AnonymousID FROM Anonymous WHERE DeviceID=@DeviceID";

            SqlCommand command = new SqlCommand(query, dbConnection);

            command.Parameters.AddWithValue("@DeviceID", DeviceID);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                auth = true;
            }

            reader.Close();
            dbConnection.Close();

            return auth;
        }
        #endregion

        #region Normal Register And Login

        /// <summary>
        /// Mobil uygulamamızın veritabanı kayıt işlemi, kullanıcı Register sayfasındaki butonu tıkladığında bu metot çalışacak
        /// </summary>
        public void CreateNewUser(string Name, string Surname, string Email, string Password, string BirthDate, double GenderID)
        {
            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }
            int UserID = 0;
            //SCOPE_IDENTITY Bize veritabanına eklenen satırın ID sini getirir :)   kullanımı sadece bu kadar -->  SET @ID = SCOPE_IDENTITY()
            string query = "INSERT INTO User (Name,Surname,Email,Password,BirthDate,GenderID,PlatformID) VALUES (@Name,@Surname,@Email,@Password,@BirthDate,@GenderID,@PlatformID) SET @ID = SCOPE_IDENTITY()";

            DateTime birthDateTime = Convert.ToDateTime(BirthDate);

            SqlCommand command = new SqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Surname", Surname);
            command.Parameters.AddWithValue("@Email", Email);
            command.Parameters.AddWithValue("@Password", Password);
            command.Parameters.AddWithValue("@BirthDate", birthDateTime);
            command.Parameters.AddWithValue("@GenderID", GenderID);
            command.Parameters.AddWithValue("@PlatformID", 1); // PlatformID = 1 == TOTOCAFE  || PlatformID = 2 == FACEBOOK

            //id ye ulaşmak için parametrenin direction unu OUTPUT olarak vermek zorundayız
            command.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

            //Verileri "User" Tablosuna ekleme..
            command.ExecuteNonQuery();

            //Output verdiğimiz için Parametredeki değere ulaşabiliyoruz. Artık UserID elimizde
            UserID = Convert.ToInt32(command.Parameters["@ID"].Value);

            //Sırada UserID ve UserTypeID yi kullanıp User'ı Costumer tablosuna eklemekte.
            //UserTypeID = 1  == ANONYMOUS  || UserTypeID=2 NOTANONYMOUS
            int UserTypeID = 2; //anonim olmadığı için tipini direkt belirttim.

            //Costumer oluşturma..
            InsertUserToCostumerTable(UserID, UserTypeID);

            dbConnection.Close();

        }

        /// <summary>
        /// Kullanıcı Doğrulama Normal Login yaparken.
        /// Boolean Sonuç döndürür. Kullanıcı Veritabanında kayıtlı ise true, değilse false 
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public bool UserAuthentication(string Email, string Password)
        {
            bool auth = false;

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }

            string query = "SELECT UserID FROM User WHERE Email=@Email AND Password=@Password";

            SqlCommand command = new SqlCommand(query, dbConnection);

            command.Parameters.AddWithValue("@Email", Email);
            command.Parameters.AddWithValue("@Password", Password);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                auth = true;
            }

            reader.Close();
            dbConnection.Close();

            return auth;
        }
        #endregion

        #region Facebook Part
        public void FacebookLogin(string Name, string Surname, string Email, string Password, DateTime BirthDate, double GenderID)
        {
            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }
            int UserID = 0;
            string FacebookInsertQuery = "INSERT INTO User (Name,Surname,Email,Password,BirthDate,GenderID,PlatformID) VALUES (@Name,@Surname,@Email,@Password,@BirthDate,@GenderID,@PlatformID) SET @ID = SCOPE_IDENTITY()";

            SqlCommand command = new SqlCommand(FacebookInsertQuery, dbConnection);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Surname", Surname);
            command.Parameters.AddWithValue("@Email", Email); //FacebookID buraya eklenecek
            command.Parameters.AddWithValue("@Password", Password); //Facebook Email Buraya Eklenecek
            command.Parameters.AddWithValue("@BirthDate", BirthDate);
            command.Parameters.AddWithValue("@GenderID", GenderID);
            command.Parameters.AddWithValue("@PlatformID", 2); // PlatformID = 1 == TOTOCAFE  || PlatformID = 2 == FACEBOOK
            command.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

            command.ExecuteNonQuery();

            UserID = Convert.ToInt32(command.Parameters["@ID"].Value);

            //Sırada UserID ve UserTypeID yi kullanıp User'ı Costumer tablosuna eklemekte.
            //UserTypeID = 1  == ANONYMOUS  || UserTypeID=2 NOTANONYMOUS
            int UserTypeID = 2; //anonim olmadığı için tipini direkt belirttim.

            //Costumer oluşturma..
            InsertUserToCostumerTable(UserID, UserTypeID);

            dbConnection.Close();
        }
        #endregion

        #region DataTables

        public DataTable getCategoriesOfCompany(double CompanyID)
        {
           
            int compID = (int)CompanyID;

            DataTable dtCategories = new DataTable();

            dtCategories.Columns.Add("CategoryID", typeof(int));
            dtCategories.Columns.Add("CategoryName", typeof(String));
            dtCategories.Columns.Add("AvailabilityID", typeof(int));
            dtCategories.Columns.Add("CompanyID", typeof(int));

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }

            string query = "SELECT CategoryID,CategoryName,AvailabilityID,CompanyID FROM Category WHERE CompanyID = @CompanyID";
            SqlCommand command = new SqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@CompanyID", compID);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dtCategories.Rows.Add(
                        reader["CategoryID"],
                        reader["CategoryName"],
                        reader["AvailabilityID"],
                        reader["CompanyID"]
                        );
                }
            }

            reader.Close();
            dbConnection.Close();

            return dtCategories;

        }

        public DataTable getProductViaCategory(double CategoryID)
        {
            int catID = (int)CategoryID;

            DataTable dtProducts = new DataTable();

            dtProducts.Columns.Add("ProductID", typeof(int));
            dtProducts.Columns.Add("ProductName", typeof(String));
            dtProducts.Columns.Add("Detail", typeof(String));
            dtProducts.Columns.Add("Price", typeof(int));
            dtProducts.Columns.Add("Credit", typeof(int));
            dtProducts.Columns.Add("CategoryID", typeof(int));
            dtProducts.Columns.Add("AvailabilityID", typeof(int));

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }

            string query = "SELECT ProductID,ProductName,Detail,Price,Credit,CategoryID,AvailabilityID FROM Product WHERE CategoryID = @CategoryID";
            SqlCommand command = new SqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@CategoryID", catID);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dtProducts.Rows.Add(
                        reader["ProductID"],
                        reader["ProductName"],
                        reader["Detail"],
                        reader["Price"],
                        reader["Credit"],
                        reader["CategoryID"],
                        reader["AvailabilityID"]
                        );
                }
            }

            reader.Close();
            dbConnection.Close();

            return dtProducts;
        }

        
        /// <summary>
        /// DataTable ile user objesi oluşurup User Tablosundaki tüm verileri DataTable a aktardım.
        /// Her kullanıcı için Email Unique'tir.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public DataTable GetUserDetails(string Email)
        {
            DataTable userTable = new DataTable();
            userTable.Columns.Add(new DataColumn("Name", typeof(String)));
            userTable.Columns.Add(new DataColumn("Surname", typeof(String)));
            userTable.Columns.Add(new DataColumn("Email", typeof(String)));
            userTable.Columns.Add(new DataColumn("BirthDate", typeof(DateTime)));

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }

            string query = "SELECT * FROM User WHERE Email=@Email";

            SqlCommand command = new SqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@Email", Email);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    userTable.Rows.Add(reader["Name"], reader["Surname"], reader["Email"], reader["BirthDate"]);
                }
            }

            reader.Close();
            dbConnection.Close();
            return userTable;
        }

        /// <summary>
        /// UserID yi belirterek Costumer Tablosunu DataTable a aktarıyorum.
        /// Ve bu metotu costumer objemi döndüyor. Böyle Costumer Tablosundaki tüm verilere erişebiliyorum.
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public DataTable GetCostumerDetails(double UserID)
        {
            int uID = (int)UserID;

            DataTable costumerTable = new DataTable();
            costumerTable.Columns.Add(new DataColumn("CostumerID", typeof(int)));
            costumerTable.Columns.Add(new DataColumn("UserID", typeof(int)));
            costumerTable.Columns.Add(new DataColumn("UserTypeID", typeof(String)));

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }

            string query = "SELECT * FROM Costumer WHERE UserID=@UserID";

            SqlCommand command = new SqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@UserID", uID);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    costumerTable.Rows.Add(
                        reader["CostumerID"], 
                        reader["UserID"],
                        reader["UserTypeID"]
                        );
                }
            }

            reader.Close();
            dbConnection.Close();
            return costumerTable;

        }

        #endregion
        
        #region Masa Oturma
        /// <summary>
        /// User tablosuna yeni kayıt eklediğinde arkasından bu metot çalışır
        /// Unique userID ve metot çağırılırken UserTypeID si belirtilmiştir.
        /// </summary>
        public void InsertUserToCostumerTable(double UserID, double UserTypeID)
        {
            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }

            int CostumerID = 0;

            string query = "INSERT INTO Costumer (UserID,UserTypeID) VALUES (@UserID,@UserTypeID) SET @ID = SCOPE_IDENTITY()";

            SqlCommand command = new SqlCommand(query, dbConnection);

            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@UserTypeID", UserTypeID);

            command.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output; //Creating Parameter Direction for SCOPE_IDENTITY()

            command.ExecuteNonQuery(); // insert data to "Costumer" Table

            CostumerID = Convert.ToInt32(command.Parameters["@ID"].Value); //get CostumerID from Inserting Row!

            dbConnection.Close();

        }
        public bool CheckAvailabilityOfTable(double TableID)
        {
            bool auth = false;

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }
            int tid = (int)TableID;

            string query = "SELECT AvailabilityID FROM [Table] WHERE TableID=@TableID";
            //CheckAvailability() select AvailabilityID from Table where TableID = @TableID | if == 1 Devam et, if == 2 ise Masa Frozen Bırak
            //Avaliable table Avaliablety 1 == AVALIABLE 2 = FROZEN 

            SqlCommand command = new SqlCommand(query, dbConnection);

            command.Parameters.AddWithValue("@TableID", tid);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (int.Parse(reader["AvailabilityID"].ToString()) == 1) // AVALIABLE 
                {
                    auth = true;
                }
                //else "false" işlemi FROZEN demektir.
            }

            reader.Close();
            dbConnection.Close();

            return auth;
        }
        public bool CheckTableControllerIsExist(double TableID)
        {
            bool auth = false;

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }
            int tid = (int)TableID;

            string query = "SELECT count(*) FROM TableController WHERE TableID=@TableID AND FinishDateTime IS NULL";
            //CheckTableController() select count from TableController where TableID = @TableID and FinishDateTime IS NULL
            //*      -Üstteki işlemde Eğer count = 0 ise MASA BOŞ DOLU UYARISI VER! count = 1 MASA DOLU

            SqlCommand command = new SqlCommand(query, dbConnection);

            command.Parameters.AddWithValue("@TableID", tid);

            int result = int.Parse(command.ExecuteScalar().ToString());

            if (result == 0)
            {
                auth = true;  // Yani demekki masa boş, true dan yürüü
            }

            // else --> false demektir yani masa dolu, Alert mesajı çıkart!
            dbConnection.Close();

            return auth;
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
          
            dbConnection.Close();
        }
        /// <summary>
        /// Bu metot sürekli kontrol işlemi yapar. Costumer tarafında Qr okutulduktan sonra Request işlemleri yapıldıktan sonra 
        /// Company tarafından isteğimiz onaylandığında "flag" imiz değişmiş olacak ve metot "true" döndürüp 
        /// Costumer'ı menu sayfasına yönlendireceğiz.
        ///reader ile flag değişkeni alınır ve kontrol edilir. Costumer tarafında default olarak 0 oluşturduğundan kontrol ederiz. Eğer 
        //Eğer 0 'dan farklı ise demektir ki Company miz mesajımızı aldı ve bize mesaj olayı verdi.

        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="TableID"></param>
        /// <returns></returns>
        public bool CheckRequestTableFlag(double UserID, double CompanyID, double TableID)
        {
            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }
            int CostumerID = 0;
            int uid = (int)UserID;
            string firstquery = "SELECT CostumerID WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(firstquery, dbConnection);
            command.Parameters.AddWithValue("@UserID", uid);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                CostumerID = int.Parse(reader["CostumerID"].ToString());
            }
            reader.Close();
            dbConnection.Close();
            /////////////////////////////////////
            bool auth = false;

            if (dbConnection.State.ToString() == "Closed")
            {
                dbConnection.Open();
            }

            string query = "SELECT flag FROM Request WHERE (CompanyID=@CompanyID) AND (CostumerID=@CostumerID) AND (TableID=@TableID)";
           
            command = new SqlCommand(query, dbConnection);
            int compid = (int)CompanyID;
            int tablid = (int)TableID;
            command.Parameters.AddWithValue("@CompanyID", compid);
            command.Parameters.AddWithValue("@CostumerID", CostumerID);
            command.Parameters.AddWithValue("@TableID", tablid);

            reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (int.Parse(reader["flag"].ToString()) != 0)
                {
                    auth = true;
                }
            }

            // else --> false demektir yani masa dolu, Alert mesajı çıkart!
            dbConnection.Close();

            return auth;
        }

        #endregion
    }
}