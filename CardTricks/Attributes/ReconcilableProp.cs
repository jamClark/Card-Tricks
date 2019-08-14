using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ReconcilableProp : System.Attribute
    {
        public bool Override { get; set; }

        public ReconcilableProp()
        {
            Override = false;
        }
    }
}
