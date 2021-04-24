using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreUtility
{
    public static class StaticDetails
    {
        public const string Role_Customer = "Independent Customer";
        public const string Role_Admin = "Administrator";
       

        public const string Session_Cart = "Cart Session";

        public static string ConvertToRawHtml(string source)
        { //similar to html.Raw method 
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for(int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if(let == '<')
                {
                    inside = true;
                    continue;
                }
                if(let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}
