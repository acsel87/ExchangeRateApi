using System;
using System.Collections.Generic;
using System.Linq;

namespace ER_Library.Helpers
{
    public static class ChartRateRange
    {
        public static void GetMinMax(this List<decimal> rates, out double min, out double max)
        {
            decimal ratesMin = rates.Min();
            decimal ratesMax = rates.Max();

            int decimalsNumber = CountRoundingPoint(ratesMax - ratesMin) - 1;

            if (decimalsNumber < 0)
            {
                decimalsNumber = 0;
            }

            min = Math.Floor(Convert.ToDouble(ratesMin) * Math.Pow(10, decimalsNumber)) / Math.Pow(10, decimalsNumber);
            max = Math.Ceiling(Convert.ToDouble(ratesMax) * Math.Pow(10, decimalsNumber)) / Math.Pow(10, decimalsNumber);
        }

        public static int CountRoundingPoint(decimal d)
        {
            d -= Math.Floor(d);

            int decimalsNumber = 0;

            while (d < 1 && d != 0)
            {
                decimalsNumber++;
                d *= 10;
            }

            return decimalsNumber;
        }
    }
}
