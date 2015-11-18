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
        void CreateNewUser(string Name, string Surname, string Email, string Password,DateTime BirthDate, double GenderID );
        bool UserAuthentication(string Email, string Passsword);
        void FacebookLogin(string Name, string Surname, string Email, string Password, DateTime BirthDate, double GenderID);
        void AnonymousLogin(string DeviceID);
        bool AnonymousAuthentication(string DeviceID);
        double GetAnonymousID(string DeviceID);
        void CreateNewAnonymous(string DeviceID);
        void InsertUserToCostumerTable(double UserID, double UserTypeID);

        DataTable GetCostumerDetails(double UserID);
        DataTable GetUserDetails(string Email);
    }
}
