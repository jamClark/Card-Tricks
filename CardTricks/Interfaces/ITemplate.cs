using CardTricks.Controls;
using CardTricks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CardTricks.Interfaces
{
    /// <summary>
    /// Card template interface.
    /// </summary>
    public interface ITemplate: IElement
    {
        new bool ReconcileElementProperties(BaseElement oldTemplate);

        void MoveElementToFront(BaseElement element);
        void MoveElementToBack(BaseElement element);
        void MoveElementDown(BaseElement element);
        void MoveElementUp(BaseElement element);
    }
}
