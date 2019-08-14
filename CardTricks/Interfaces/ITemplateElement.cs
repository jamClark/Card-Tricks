using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Interfaces
{
    public interface ITemplateElement<T> : IElement where T : class
    {
        T Content { get; set; }
        string ContentToString { get; }
    }
}
