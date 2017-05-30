using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.HerramientasGenerales
{
    public class StringTools
    {
        public static int[] jsonStringToArray(string json)
        {
            return json.Trim('[').Trim(']').Split(',').Select(int.Parse).ToArray();
        }
    }
}