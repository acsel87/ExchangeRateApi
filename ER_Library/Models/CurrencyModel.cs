using System.Drawing;
using System.IO;

namespace ER_Library.Models
{
    public class CurrencyModel
    {
        public int currency_id { get; set; }
        public string currency_name { get; set; }
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public byte[] currency_image { get; set; }

        public Image Flag()
        {
            if (currency_image != null)
            {
                MemoryStream ms = new MemoryStream(currency_image);
                return Image.FromStream(ms);
            }

            return null;
        }

        public string Info
        {
            get
            {
                return $"{currency_name.PadRight(50)}{currency_symbol} {currency_code}";
            }
        }
    }
}