using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Utils
{
    public static class Casting
    {
        public static bool TryCast<T>(ref T t, object o)
        {
            if (o == null|| !typeof(T).IsAssignableFrom(o.GetType()) )  return false;
            t = (T)o;
            return true;
        }

        public static T SimpleCast<T>(object input)
        {
            return (T)input;
        }

        public static T DynCast<T>(Type type, object input)
        {
            return (T)Convert.ChangeType(input, type);
        }
    }
}
