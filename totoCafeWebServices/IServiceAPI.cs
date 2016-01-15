using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace totoCafeWebServices
{
    public interface IServiceAPI
    {
        void CreateNewUser(string Name, string Surname, string Email, string Password,string BirthDate, double GenderID );
        bool UserAuthentication(string Email, string Passsword);
        void FacebookLogin(string Name, string Surname, string Email, string Password, DateTime BirthDate, double GenderID);
        void AnonymousLogin(string DeviceID);
        bool AnonymousAuthentication(string DeviceID);
        double GetAnonymousID(string DeviceID);
        void CreateNewAnonymous(string DeviceID);
        void InsertUserToCostumerTable(double UserID, double UserTypeID);

        DataTable getCategoriesOfCompany(double CompanyID);
        DataTable getProductViaCategory(double CategoryID);

        DataTable GetCostumerDetails(double UserID);
        DataTable GetUserDetails(string Email);


        /* ilk olarak qr code okutulacak. Okutulan qr kodun taslağı şu şekilde olmalıdır. --> TotoCafe-1-Masa1
        * İlki Uygulama ismi - 2.si CompanyID - 3.sü TableID
        * okunan string split yapılacak ve qr[0] == "TotoCafe" için if yazılır
        * Eğer doğruysa qr[1] ve qr[2] bi değer varmı diye kontrol edilir --> mesela qr sonucumuz eksik ya da Sadece TotoCafe ise Qr kodumuz yanlıştır.
        * Üstteki işlemde kontrol edildikten sonra 
        * ilk metotumuz QrAuthentication yapılır.  
        * Avaliable table Avaliablety 1 == AVALIABLE 2 = FROZEN 
        *  CheckAvailability() select AvailabilityID from Table where TableID = @TableID if == 1 Devam et, if == 2 ise Masa Frozen Bırak
        *  CheckTableController() select count from TableController where TableID = @TableID and FinishDateTime = NULL
        *      -Üstteki işlemde Eğer count = 1 ise MASA ŞUAN DOLU UYARISI VER! count = 0 ise evet masa boş yürü bro, devammmm.
        * 
        */

        bool CheckAvailabilityOfTable(double TableID);
        bool CheckTableControllerIsExist(double TableID);
        void InsertRequestTableViaQr(double UserID, double CompanyID, double TableID);
        bool CheckRequestTableFlag(double UserID, double CompanyID, double TableID);
    }
}
