using JsonServices;
using JsonServices.Web;

namespace totoCafeWebServices
{
    /// <summary>
    /// Summary description for totoHandler
    /// </summary>
    public class totoHandler : JsonHandler
    {

        public totoHandler()
        {
            this.service.Name = "totoCafeWebServices";
            this.service.Description = "JSON API for totoCafe Android Appliation";
            InterfaceConfiguration IConfig = new InterfaceConfiguration("RestAPI", typeof(IServiceAPI), typeof(ServiceAPI));
            this.service.Interfaces.Add(IConfig);
        }
    }
}