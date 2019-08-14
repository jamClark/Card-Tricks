using CardTricks.Controls;
using CardTricks.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CardTricks.Interfaces
{
    public delegate void RefreshLayoutEvent();
    public delegate void NameChangedEvent();
    public delegate void BackgroundChangedEvent();

    /// <summary>
    /// Generic element interface used by
    /// all card templates and elements.
    /// </summary>
    [ServiceContract]
    public interface IElement
    {
        event RefreshLayoutEvent RefreshLayout;
        void TriggerRefreshLayout();

        bool AllowFocus { get; }
        bool Locked { get; set; }

        Guid Guid { get; }
        string Name { get; set; }
        double XPos { get; set; }
        double YPos { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double Angle { get; set; }
        string ContentToString { get; }
        UserControl LastLayoutInstance { get; }
        IElement Inheritance { get; set; }
        IElement Parent { get; set; }
        List<BaseElement> Children { get; set; }
        IList<UIElement> EditableProperties { get; set; }
        IList<UIElement> InstanceProperties { get; set; }
        

        void ChangeID(string newId);
        void AddElement(BaseElement element);
        bool RemoveElement(BaseElement element);
        bool RemoveElementByName(string name);
        UIElement GenerateLayout();
        BaseElement GetShallowClone();
        bool ReconcileElementProperties(BaseElement oldElement, bool reconcileOverrides = false);
        void TriggerManualUpdates();
        void HandleManualUpdates();
        IList<UIElement> GetHierachyOfInstanceProperties();
        void BindInstanceEditProps();

        event NameChangedEvent NameChanged;
    }
}
