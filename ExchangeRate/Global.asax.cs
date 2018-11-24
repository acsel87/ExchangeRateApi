using ER_Library.DataAccess;
using System.Web.Http;

namespace ExchangeRate
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {   //TestBranch         
            GlobalConfig.SetConnection(new SqlConnector());
            SyncData.UpdateValidDaysHtml();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
