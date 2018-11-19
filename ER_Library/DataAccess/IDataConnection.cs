using ER_Library.Models;
using System;
using System.Collections.Generic;

namespace ER_Library.DataAccess
{
    public interface IDataConnection
    {
        List<CurrencyModel> GetCurrency_All();
        List<KeyValuePair<DateTime, decimal>> GetRates(DateTime startDate, DateTime endDate, int currency_id, int skipValue);
    }
}
