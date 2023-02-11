using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OsdpSpy.Attributes
{
    public class IsValidBaudRateAttribute : ValidationAttribute
    {
        public IsValidBaudRateAttribute() : base(Msg) {}

        private const string Msg =
            "Specify a valid baud rate: auto | 9600 | 19200 | 38400 | 57600 | 115200 | 230400";

        public override bool IsValid(object value)
        {
            try
            {
                if (value is string rate)
                {
                    var r = Convert.ToInt32(rate);
                    var rates = new[] { 9600, 19200, 38400, 57600, 115200, 230400 };
                    return  rates.Contains(r);
                }
                return false;
            }
            catch (Exception)
            {
                var rate = value as string;
                return rate.ToLower() == "auto";
            }
        }
    }
}