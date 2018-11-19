using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;

namespace ER_Library.DataAccess
{
    public static class GlobalConfig
    {
        public static string LastDate { get; set; } = "18-Nov-18";

        public static IDataConnection Connection { get; private set; }

        public static string CnnString(string name)
        {            
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static string GetAppConfig(string name)
        {

            return ConfigurationManager.AppSettings[name];
        }

        // TODO - set web.config
        public static void SetAppConfig(string name, string newValue)
        {
            string configPath = HttpRuntime.AppDomainAppPath;

            Configuration configuration = WebConfigurationManager.OpenWebConfiguration(configPath);
            //Configuration configuration = WebConfigurationManager.GetWebApplicationSection("appSettings") as Configuration;

            configuration.AppSettings.Settings[name].Value = newValue;
            configuration.Save(ConfigurationSaveMode.Minimal);
        }

        public static void SetConnection(IDataConnection dataConnection)
        {
            Connection = dataConnection;
        }

        public static string GetHtmlLink(int currency_id, DateTime startDate, DateTime endDate)
        {
            string startDateFormated = startDate.ToString("dd-MM-yyyy");
            string endDateFormated = endDate.ToString("dd-MM-yyyy");

            return $"{GetAppConfig("HtmlPrefix")}{GetAppConfig(currency_id.ToString())}{GetAppConfig("HtmlStartDate")}{startDateFormated}{GetAppConfig("HtmlStopDate")}{endDateFormated}";
        }
    }
}
