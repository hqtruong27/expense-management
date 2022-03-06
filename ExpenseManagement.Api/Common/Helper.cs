using System.Globalization;
using System;

namespace ExpenseManagement.Api.Common
{
    public static class Helper
    {
        public static string ToVietNamDong(this decimal amount, bool withFormat = true)
        {
            var culture = CultureInfo.GetCultureInfo("vi-VN");
            return withFormat
                ? amount == 0 ? "0 ₫" : amount.ToString("#,### ₫", culture.NumberFormat)
                : amount == 0 ? "0" : amount.ToString("#,###", culture.NumberFormat);
        }

        public static async Task<string> GetTextFromFileAsync(this string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public static string ToPhoneNumber(this string str)
        {
            return "0" + str.Remove(0, 2);
        }

        public static string? ToVNPhoneNumber(this string str, bool withFormat = false)
        {
            var phoneNumber = !string.IsNullOrEmpty(str) && str.Length > 9 && str[..1] == "0" ? "84" + str.Remove(0, 1) : str;
            return !string.IsNullOrWhiteSpace(phoneNumber) && withFormat ? $"+{phoneNumber}" : null;
        }

        public static string? HMACSHA256(string text, string key)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            System.Text.ASCIIEncoding encoding = new();

            byte[] textBytes = encoding.GetBytes(text);
            byte[] keyBytes = encoding.GetBytes(key);

            byte[] hashBytes;

            using (System.Security.Cryptography.HMACSHA256 hash = new(keyBytes))
            {
                hashBytes = hash.ComputeHash(textBytes);
            }

            var result = BitConverter.ToString(hashBytes);

            return result.Replace("-", "").ToLower();
        }

        public static string GetDisplayName<T>(this T enumValue) where T : struct
        {
            //First judge whether it is enum type data
            System.Type type = enumValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("must be of Enum type", nameof(enumValue));
            }

            var enumString = enumValue.ToString() ?? string.Empty;
            //Find the corresponding Display Name for the enum
            var member = type.GetMember(enumString);
            if (member != null && member.Length > 0)
            {
                var attributeData = member[0].GetCustomAttributesData().FirstOrDefault();
                if (attributeData != null)
                {
                    //Pull out the value
                    return attributeData.NamedArguments.FirstOrDefault().TypedValue.Value?.ToString() ?? string.Empty;
                }
            }

            //If you have no Display Name, just return the ToString of the enum
            return enumString;
        }

        public static string GetDescription<T>(this T enumValue) where T : struct
        {
            var type = enumValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumValue)} must be of Enum type", nameof(enumValue));
            }

            var enumString = enumValue.ToString() ?? String.Empty;
            var memberInfo = type.GetMember(enumString);
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
                }
            }

            return enumString;
        }

        public static string ConvertAmountToText(decimal number)
        {
            string s = number.ToString("#");
            string[] numberWords = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] layer = new string[] { "", "nghìn", "triệu", "tỷ" };
            int i, j, unit, dozen, hundred;
            string str = " ";
            bool booAm = false;
            decimal decS;
            try
            {
                decS = Convert.ToDecimal(s.ToString());
            }
            catch
            {
                decS = 0;
            }
            if (decS < 0)
            {
                decS = -decS;
                s = decS.ToString();
                booAm = true;
            }
            i = s.Length;
            if (i == 0)
                str = numberWords[0] + str;
            else
            {
                j = 0;
                while (i > 0)
                {
                    unit = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0)
                        dozen = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        dozen = -1;
                    i--;
                    if (i > 0)
                        hundred = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        hundred = -1;
                    i--;
                    if ((unit > 0) || (dozen > 0) || (hundred > 0) || (j == 3))
                        str = layer[j] + str;
                    j++;
                    if (j > 3) j = 1;
                    if ((unit == 1) && (dozen > 1))
                        str = "một " + str;
                    else
                    {
                        if ((unit == 5) && (dozen > 0))
                            str = "lăm " + str;
                        else if (unit > 0)
                            str = numberWords[unit] + " " + str;
                    }
                    if (dozen < 0)
                        break;
                    else
                    {
                        if ((dozen == 0) && (unit > 0)) str = "lẻ " + str;
                        if (dozen == 1) str = "mười " + str;
                        if (dozen > 1) str = numberWords[dozen] + " mươi " + str;
                    }
                    if (hundred < 0) break;
                    else
                    {
                        if ((hundred > 0) || (dozen > 0) || (unit > 0)) str = numberWords[hundred] + " trăm " + str;
                    }
                    str = " " + str;
                }
            }
            if (booAm) str = "Âm " + str;
            var result = System.Text.RegularExpressions.Regex.Replace(str + "đồng chẵn", @"\s+", " ").Trim();
            return string.Concat(result[..1].ToUpper(), result.AsSpan(1));
        }
    }
}