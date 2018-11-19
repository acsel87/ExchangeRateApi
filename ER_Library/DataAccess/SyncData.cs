using ER_Library.Helpers;
using ER_Library.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ER_Library.DataAccess
{
    public static class SyncData
    {
        /// <summary>
        /// Skip: BNR doesn't provide all Currency Model members (name, icon, symbol)
        /// </summary>
        /// <param name="latestExchangeRateURL"></param>
        /// <returns></returns>        
        public static List<CurrencyModel> UpdateCurrency()
        {
            List<CurrencyModel> currencies = new List<CurrencyModel>();

            XDocument doc = XDocument.Load(GlobalConfig.GetAppConfig("LatestExchangeRate"));
            XNamespace ns = doc.Root.Name.Namespace;

            if (doc.Descendants(ns + GlobalConfig.GetAppConfig("CurrencyXml_RateTag")).FirstOrDefault() != null)
            {
                var elements = doc.Descendants(ns + GlobalConfig.GetAppConfig("CurrencyXml_RateTag"));

                foreach (XElement elem in elements)
                {
                    CurrencyModel tempModel = new CurrencyModel();

                    string currencyCode = elem.Attribute(GlobalConfig.GetAppConfig("CurrencyXml_CurrencyAttribute")).Value;
                    tempModel.currency_code = currencyCode;

                    currencies.Add(tempModel);
                }
            }
            return currencies;
        }

        /// <summary>
        /// Required: Number of valid days in the selected period to compare with the ones in the database
        /// 
        /// Option 1: Need list with correct free days (together with weekends) to calculate them.
        /// 
        /// Option 2 (this):
        /// - check if "today" date is higher than stored "lastDate" date
        /// - if so, get file with any one currency for whole existing period from source
        /// - if databse has fewer, update it
        /// </summary>
        public static void UpdateValidDaysHtml()
        {
            //DateTime lastDate = Convert.ToDateTime(GlobalConfig.GetAppConfig("LastDate"));
            DateTime lastDate = Convert.ToDateTime(GlobalConfig.LastDate); // temp: can't set web.config
            lastDate.AddDayHour(0, 14);

            DateTime today = DateTime.Now;

            if (today.Hour < 14)
            {
                today.AddDayHour(-1, 14);
            }
            else
            {
                today.AddDayHour(0, 14);
            }

            if (today > lastDate)
            {
                string _lastDate = today.ToShortDateString();
                //GlobalConfig.SetAppConfig("LastDate", _lastDate);
                GlobalConfig.LastDate = _lastDate; // temp: can't set web.config

                string link = GlobalConfig.GetAppConfig("ValidDaysHtml");

                HtmlWeb web = new HtmlWeb();

                HtmlDocument htmlDoc = web.Load(link);

                string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                string path = Path.Combine(dir, "Files");
                string file = "ValidDays.html";

                htmlDoc.Save("C:\\Files\\ValidDays.html");
                //htmlDoc.Save(Path.Combine(path, file));
            }
        }

        public static int GetNumberOfDaysBetween(DateTime startDate, DateTime endDate)
        {
            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string path = Path.Combine(dir, "Files");
            string file = "ValidDays.html";
            string xPathDate = "//td[@class='stat_table_cell_date']";

            HtmlDocument htmlDoc = new HtmlDocument();

            htmlDoc.Load("C:\\Files\\ValidDays.html");
            //htmlDoc.Load(Path.Combine(path, file));

            List<string> datesString = htmlDoc.DocumentNode.SelectNodes(xPathDate).Select(x => x.InnerText).ToList();
            List<string> daysBetween = datesString.Where(x => Convert.ToDateTime(x) >= startDate && Convert.ToDateTime(x) <= endDate).ToList();

            return daysBetween.Count();
        }
    }
}
