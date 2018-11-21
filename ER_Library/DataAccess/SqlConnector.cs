using Dapper;
using ER_Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ER_Library.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private string db = GlobalConfig.GetAppConfig("DB");

        public List<CurrencyModel> GetCurrency_All()
        {
            List<CurrencyModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<CurrencyModel>("dbo.spGetCurrency_All").ToList();
            }

            return output;
        }

        public List<KeyValuePair<DateTime, decimal>> GetRates(DateTime startDate, DateTime endDate, int currency_id, int skipValue)
        {
            int validDaysBetween = SyncData.GetNumberOfDaysBetween(startDate, endDate);
            bool isAllDays = false;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@StartDate", startDate);
                p.Add("@EndDate", endDate);
                p.Add("@CurrencyId", currency_id);
                p.Add("@ValidDaysBetween", validDaysBetween);

                isAllDays = connection.Query<bool>("dbo.spCheckIfValidDays", p, commandType: CommandType.StoredProcedure).First();
            }

            if (isAllDays)
            {
                return GetRatesSkipped(startDate, endDate, currency_id, skipValue);
            }
            else
            {
                List<KeyValuePair<DateTime, decimal>> existingRates = GetRatesAll(startDate, endDate, currency_id);
                List<RateModel> missingRates = new List<RateModel>();
                List<RateModel> importedRates = XmlParser.ImportRatesFromHtml(currency_id, startDate, endDate);
                
                foreach (RateModel rate in importedRates)
                {
                    bool isRate = false;

                    foreach (var pair in existingRates)
                    {
                        if (rate.rate_date == pair.Key)
                        {
                            isRate = true;
                            break;
                        }
                    }

                    if (!isRate)
                    {
                        missingRates.Add(rate);                        
                    }                   
                }

                InsertRates(missingRates, currency_id);
                return GetRatesSkipped(startDate, endDate, currency_id, skipValue);
            }
        }

        private List<KeyValuePair<DateTime, decimal>> GetRatesSkipped(DateTime startDate, DateTime endDate, int currency_id, int skipValue)
        {
            List<KeyValuePair<DateTime, decimal>> output = new List<KeyValuePair<DateTime, decimal>>();

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@StartDate", startDate);
                p.Add("@EndDate", endDate);
                p.Add("@CurrencyId", currency_id);
                p.Add("@SkipValue", skipValue);

                output = connection.Query<KeyValuePair<DateTime, decimal>>("dbo.spGetRatesSkipped", p, commandType: CommandType.StoredProcedure).ToList();
            }

            return output;
        }

        private List<KeyValuePair<DateTime, decimal>> GetRatesAll(DateTime startDate, DateTime endDate, int currency_id)
        {
            List<KeyValuePair<DateTime, decimal>> output = new List<KeyValuePair<DateTime, decimal>>();

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@StartDate", startDate);
                p.Add("@EndDate", endDate);
                p.Add("@CurrencyId", currency_id);

                output = connection.Query<KeyValuePair<DateTime, decimal>>("dbo.spGetRatesAll", p, commandType: CommandType.StoredProcedure).ToList();
            }

            return output;
        }

        private void InsertRates(List<RateModel> rates, int currency_id)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();

                foreach (RateModel r in rates)
                {
                    p.Add("@CurrencyId", currency_id);
                    p.Add("@Date", r.rate_date);
                    p.Add("@Rate", r.rate);
                    p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spInsertRates", p, commandType: CommandType.StoredProcedure);
                }
            }
        }
    }
}
