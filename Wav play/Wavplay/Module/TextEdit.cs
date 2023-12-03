using System;
using System.Security.Cryptography;
using System.Text;

namespace Wavplay
{
    public static class StringExtension
    {
        public static string GetMD5Checksum(this string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(str)));
        }

        public static string FirstOf(this string str, char Of)
        {
            if (str == null) return str;
            if (str.Length == 0) return str;
            if (str.IndexOf(Of) == -1) return string.Empty;
            str = str.Substring(0, str.LastIndexOf(Of));
            return str;
        }

        public static string FirstOf(this string str, string Of)
        {
            if (str == null || Of == null) return str;
            if (str.Length == 0 || Of.Length == 0) return str;
            if (str.IndexOf(Of) == -1) return string.Empty;
            str = str.Substring(0, str.LastIndexOf(Of));
            return str;
        }

        public static string FirstOf(this string str, char Of, int index)
        {
            if (str == null) return str;
            if (str.Length == 0) return str;
            if (str.IndexOf(Of) == -1) return string.Empty;
            if (index == 0 || index == 1 || index == 88)
            {
                if (index == 88)
                    return str.FirstOf(Of, str.QuantityOf(Of));

                return str.Substring(0, str.IndexOf(Of));
            }

            int Quantity = 0;
            for (int i = 0; i < str.Length + 1; i++)
            {
                if (str[i] == Of)
                {
                    Quantity++;
                    if (Quantity == index)
                        return str.Substring(0, i + 1);
                }
            }

            return str;
        }

        public static string LastOf(this string str, string Of)
        {
            if (str == null || Of == null) return str;
            if (str.Length == 0 || Of.Length == 0) return str;
            if (str.IndexOf(Of) == -1) return string.Empty;
            return str.Substring(str.IndexOf(Of) + Of.Length);
        }

        public static string LastOf(this string str, char Of)
        {
            if (str == null) return str;
            if (str.Length == 0) return str;
            if (str.IndexOf(Of) == -1) return string.Empty;
            return str.Substring(str.LastIndexOf(Of) + 1);
        }

        public static int QuantityOf(this string str, char Elm)
        {
            int Quantity = 0;
            foreach (char chr in str)
                if (chr == Elm)
                    Quantity++;

            return Quantity;
        }

        public static int QuantityOf(this string str, string Elm)
        {
            int Quantity = 0;
            while (Elm.IndexOf(str) != -1)
            {
                Quantity++;
                Elm = Elm.Substring(Elm.IndexOf(str) + str.Length);
            }

            return Quantity;
        }

        public static string KnTextCorrect(this string str)
        {
            if (str != null && str.Length > 0)
            {
                string copy = str;
                str = str.Trim().ToLower();

                if (str.Length > 0)
                {
                    str = str.Replace("(", "");
                    str = str.Replace(")", "");
                    str = str.Replace("   ", "");
                    str = str.Replace("  ", "");
                    str = str.Replace(" ", "");
                    str = str.Replace("\t", "");
                }
            }
            return str;
        }

        public static string TextCorrect(this string str)
        {
            if (str != null && str.Length > 0)
            {
                str = str.Trim();
                str = str.ToLower();

                if (str.Length > 0)
                {
                    str = Char.ToUpper(str[0]) + str.Substring(1);
                    str = str.Replace("+", " ");
                    str = str.Replace("_", " ");
                    str = str.Replace("-", " ");
                    str = str.Replace(".", " ");
                    str = str.Replace(",", "");
                    str = str.Replace("'", "");
                    str = str.Replace("\\", "");
                    str = str.Replace("/", "");
                    str = str.Replace("   ", " ");
                    str = str.Replace("  ", " ");
                }
                for (int i = 0; i < str.Length; i++) if (str[i] == ' ' && i + 1 < str.Length) str = str.Substring(0, i + 1) + Char.ToUpper(str[i + 1]) + str.Substring(i + 2, str.Length - i - 2);
            }
            return str;
        }

        public static string HtmTextCorrect(this string str)
        {
            if (str != null && str.Length > 0)
            {
                //str = str.Replace("'", "&#39;");
                str = str.Replace("'", "");
                str = str.Replace("\t", "   ");
                //str = str.Replace("\"", "&#34;");
                str = str.Replace("\"", "");
                //str = str.Replace("\\", "&#92;");
                str = str.Replace("\\", "");
                str = str.Replace("[", "");
                str = str.Replace("]", "");
                str = str.Replace("\n", "<br>");
            }
            return str;
        }

        public static string InMidleOf(this string Str, string Fr, string To)
        {
            if (Str.Length != 0 && Fr.Length != 0 && To.Length != 0)
            {
                if (Str.IndexOf(Fr) != -1)
                {
                    Str = Str.Substring(Str.IndexOf(Fr) + Fr.Length);
                    if (Str.IndexOf(To) != -1) Str = Str.Substring(0, Str.IndexOf(To));
                }
            }
            return Str;
        }
    }

    class TextEdit
    {
       
    }
}
