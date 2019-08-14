using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Utils
{
    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T key, U value)
        {
            this.Key = key;
            this.Value = value;
        }

        public T Key { get; set; }
        public U Value { get; set; }
    };


    public class Pair
    {
        public Pair()
        {
        }

        public Pair(object key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        public object Key { get; set; }
        public object Value { get; set; }
    }
}
