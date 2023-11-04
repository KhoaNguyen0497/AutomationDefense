using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationDefense.Helpers
{
    public static class ListHelper
    {
        public static List<T> ToList<T>(this T t)
        {
            if (t == null)
            {
                return new List<T>();
            }

            return new List<T> { t };
        }

        public static List<T> NullSafe<T>(this List<T> list)
        {
            if (list == null)
            {
                list = new List<T>();
            }

            return list;
        }
    }
}
