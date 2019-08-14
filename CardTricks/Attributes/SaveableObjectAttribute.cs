using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SaveableObjectAttribute : System.Attribute
    {
        public string Name { get; set; }

        public SaveableObjectAttribute()
        {
            Name = null;
        }
    }
}
