using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BotAlert.Helpers
{
    public static class StringValidatorExtensionMethods
    {
        /*public static bool IsDateValid(this string str)
        {
            *//*var regex = new Regex(@"\d{4}-\d{2}-\d{2}-\d{2}-\d{2}-\d{2}");
            return regex.Match(str).Success && DateTime.TryParse(str, out _);*//*
            try
            {

            }
            catch
            {
                return DateTime.ParseExact(m.Value, "yyyy-MM-dd-hh-mm-ss", CultureInfo.InvariantCulture)
            }
        }*/
    }
}
