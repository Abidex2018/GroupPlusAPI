using System;
using System.Collections.Generic;

namespace GroupPlus.Common
{
    public static class UtilExtensions
    {
        public static string ToUtilString(this string stringToGroup)
        {
            if (string.IsNullOrEmpty(stringToGroup)) { return ""; }
            try
            {
                return stringToGroup.Replace("_", " ");
            }
            catch (Exception)
            {
                return stringToGroup;
            }
        }
        public static string ToUtilString(this object obj)
        {
            if (obj == null) { return ""; }
            try
            {
                var thisItem = obj.ToString();
                if (string.IsNullOrEmpty(thisItem))
                {
                    return "";

                }
                return thisItem.Replace("_", " ");
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
   
}
