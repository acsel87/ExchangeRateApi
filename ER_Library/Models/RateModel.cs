using System;
using System.Collections.Generic;

namespace ER_Library.Models
{
    public class RateModel
    {
        public DateTime rate_date { get; set; }
        public decimal rate { get; set; }

        public KeyValuePair<DateTime, decimal> RateKeyValue()
        {
            KeyValuePair<DateTime, decimal> temp = new KeyValuePair<DateTime, decimal>();

            if (rate_date != null)
            {
                temp = new KeyValuePair<DateTime, decimal>(rate_date, rate);
            }

            return temp;
        }
    }
}