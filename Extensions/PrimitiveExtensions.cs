using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterX.Core.Extensions
{
    public static class PrimitiveExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNull(this int? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNullOrZero(this int? value)
        {
            if (value == null || value == 0) return true;
            else return false;
        }

        public static bool IsNullOrSpace(this string value)
        {
            if(value != null)
            {
                value = value.Replace(" ", "");
            }
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNull(this float? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNull(this decimal? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNull(this double? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNull(this char? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNull(this byte? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNull(this short? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNull(this sbyte? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNull(this DateTime? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNull(this long? value)
        {
            if (value == null) return true;
            else return false;
        }

        public static bool IsNumeric(this string value)
        {
            return int.TryParse(value, out _ );
        }

        public static int ToInt(this string value)
        {
            int result = 0;
            if (Int32.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        // https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            if (str != null)
            {
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }

        public static string RemoveNoneNumeric(this string str)
        {
            StringBuilder sb = new StringBuilder();
            if (str != null)
            {
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9'))
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }

        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }

        public static string Right(this string sValue, int iMaxLength)
        {
            //Check if the value is valid
            if (string.IsNullOrEmpty(sValue))
            {
                //Set valid empty string as string could be null
                sValue = string.Empty;
            }
            else if (sValue.Length > iMaxLength)
            {
                //Make the string no longer than the max length
                sValue = sValue.Substring(sValue.Length - iMaxLength, iMaxLength);
            }

            //Return the string
            return sValue;
        }

        public static string ReplaceApostrophe(this string value)
        {
            if (value == null) return value;
            else
                return value.Replace("'", "''");
        }

        public static string ReplaceCommas(this string value)
        {
            if (value == null) return value;
            else
                return value.Replace(",", "");
        }

        public static DateTime Date(this DateTime? value)
        {
            DateTime date = value ?? DateTime.Now;
            return date.Date;
        }

        // https://stackoverflow.com/questions/1859248/how-to-change-time-in-datetime
        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static string Concatenate(this List<string> list, string separator)
        {
            string result = "";
            foreach (string item in list)
            {
                result += separator + item;
            }
            return result;
        }
    }
}
