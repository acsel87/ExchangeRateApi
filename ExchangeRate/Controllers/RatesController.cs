using ER_Library.DataAccess;
using ER_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ExchangeRate.Controllers
{
    public class RatesController : ApiController
    {
        [HttpGet]
        public List<KeyValuePair<DateTime, decimal>> GetRates(string startDate, string endDate, int currency_id, int skipValue)
        {
            DateTime start = Convert.ToDateTime(startDate);
            DateTime end = Convert.ToDateTime(endDate);

            return GlobalConfig.Connection.GetRates(start, end, currency_id, skipValue);
        }

        [HttpGet]
        public List<CurrencyModel> GetCurrency()
        {
            return GlobalConfig.Connection.GetCurrency_All();
        }
    }
}
