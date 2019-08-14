using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Attributes
{
    public delegate void DeepClone(PropertyInfo source, PropertyInfo dest);

    /// <summary>
    /// Used to flag a property of an element
    /// as being a viable target for shallow copying
    /// during a call to GetShallowClone().
    /// </summary>
    public class ContentElementCloneAttribute : Attribute
    {
    }
}
