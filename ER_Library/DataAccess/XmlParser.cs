using ER_Library.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace ER_Library.DataAccess
{
    public static class XmlParser
    {
        public static List<RateModel> ImportRatesFromXml(string filePath, string currencyCode)
        {
            List<RateModel> rates = new List<RateModel>();
            string currencyTag = GlobalConfig.GetAppConfig("CurrencyXml_TagPrefix") + currencyCode;

            XDocument doc = XDocument.Load(filePath);
            XNamespace ns = doc.Root.Name.Namespace;

            if (doc.Descendants(ns + currencyTag).FirstOrDefault() != null)
            {
                var elements = doc.Descendants(ns + GlobalConfig.GetAppConfig("RateXml_RowTag"));

                foreach (XElement elem in elements)
                {
                    RateModel tempModel = new RateModel();

                    string date = elem.Element(ns + GlobalConfig.GetAppConfig("RateXml_DateTag")).Value;
                    string rate = elem.Element(ns + currencyTag).Value;
                    rate = rate.Replace(',', '.');

                    tempModel.rate_date = Convert.ToDateTime(date);
                    tempModel.rate = Convert.ToDecimal(rate);
                    rates.Add(tempModel);
                }
            }

            return rates;
        }

        public static List<RateModel> ImportRatesFromHtml(int currency_id, DateTime startDate, DateTime endDate)
        {
            List<RateModel> rates = new List<RateModel>();

            string xPathDate = "//td[@class='stat_table_cell_date']";
            string xPathRate = "//td[@class='stat_table_cell']";

            string startDateFormated = startDate.ToString("dd-MM-yyyy");
            string endDateFormated = endDate.ToString("dd-MM-yyyy");

            using (WebClient webClient = new WebClient())
            {
                webClient.BaseAddress = GlobalConfig.GetAppConfig("BaseAddress");

                NameValueCollection query = new NameValueCollection();
                query["icid"] = GlobalConfig.GetAppConfig("icid");
                query["table"] = GlobalConfig.GetAppConfig("table"); ;
                query["column"] = GlobalConfig.GetAppConfig(currency_id.ToString()); ;
                query["startDate"] = startDateFormated;
                query["stopDate"] = endDateFormated;
                webClient.QueryString.Add(query);

                string page = webClient.DownloadString(GlobalConfig.GetAppConfig("HtmlPrefix"));
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(page);

                List<string> datesString = doc.DocumentNode.SelectNodes(xPathDate).Select(x => x.InnerText).ToList();
                List<string> ratesString = doc.DocumentNode.SelectNodes(xPathRate).Select(x => x.InnerText).ToList();

                for (int i = 0; i < datesString.Count(); i++)
                {
                    RateModel rateModel = new RateModel();

                    rateModel.rate_date = Convert.ToDateTime(datesString[i]);
                    rateModel.rate = Convert.ToDecimal(ratesString[i]);

                    rates.Add(rateModel);
                }

                return rates;
            }
        }
    }
}
