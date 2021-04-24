using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUtility
{
    /*DIDN'T USE IN THIS PROJECT - 
     * Just to learn how to add session objects apart from 
     * adding integer to sessions- this provided by default in asp.net core
     * HttpsContext.Sessions.GetInt32() / SetInt32()
     */

    public static class SessionExtension 
    { //after this, configure session options in StartUp.cs
        //To configure list/object
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        { //convert session into an object an store it and to retreive, convert it back and display.
            var value = session.GetString(key);
            if (value == null)
            {
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
        }
    }
}
