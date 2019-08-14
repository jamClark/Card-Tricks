using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SaveableAttribute : System.Attribute
    {
        public string Name { get; set; }
        public Type Concrete { get; set; }

        public SaveableAttribute()
        {
            Name = null;
            Concrete = null;
        }
    }
}
