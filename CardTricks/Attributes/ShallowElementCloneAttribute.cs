using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Attributes
{
    /// <summary>
    /// Used to flag a property of an element
    /// as being a viable target for shallow copying
    /// during a call to GetShallowClone().
    /// </summary>
    public class ShallowElementCloneAttribute : Attribute
    {
    }
}
