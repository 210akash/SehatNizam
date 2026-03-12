using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Services.Helper
{
    public static class StringHelper
    {
        public static string ToReverseEquation(this string s)
        {
            StringBuilder sb = new StringBuilder(s);

            sb.Replace("+", "Minus");
            sb.Replace("-", "Plus");
            sb.Replace("/", "Multiply");
            sb.Replace("*", "Divide");
            sb.Replace("Minus", "-");
            sb.Replace("Plus", "+");
            sb.Replace("Multiply", "*");
            sb.Replace("Divide", "/");

            return sb.ToString();
        }
    }
}
