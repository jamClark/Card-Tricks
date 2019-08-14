using CardTricks.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace CardTricks
{
    /// <summary>
    /// Used to flag properties of an element as
    /// editable within the edit panel. This will
    /// allow anything derived from BaseElement to
    /// generate a list of controls where each control
    /// is bound to the model and then the list is
    /// bound to a window for display and editing
    /// of properties for that Element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class InstanceEditPropAttribute : System.Attribute, IEditProp
    {
        public enum Use { Template, Instance, Both }

        public string Label { get; set; }
        public string ToolTip { get; set; }
        public double LabelWidth { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string SelectedItem { get; set; }
        public object SelectedValue { get; set; }
        public bool IsEnabled { get; set; }
        public int GroupID { get; set; }
        public Use UseOption { get; set; }
        public bool VerticalStacking { get; set; }

        public string ItemsSourcePath { get; set; }
        //keys and values are combined into a single Pair that is attched to the itemssource of a collection control
        public string KeysSourcePath { get; set; }
        public Type KeysType { get; set; }
        public string ValuesSourcePath { get; set; }
        public Type ValueType { get; set; }
        public string DisplayMemberPath { get; set; }
        public string SelectedValuePath { get; set; }
        public string SelectedItemPath { get; set; }

        public BindingMode BindingMode { get; set; }
        public UpdateSourceTrigger UpdateTrigger { get; set; }

        public Type ControlType { get; set; }
        public string DependencyProperty { get; set; }
        public Type Converter { get; set; }


        public InstanceEditPropAttribute(
            string label,
            Type controlType,
            string dependencyProperty = "TextProperty")
        {
            //default values
            Width = Double.NaN;
            Height = Double.NaN;
            LabelWidth = Double.NaN;
            IsEnabled = true;
            GroupID = 10;
            ToolTip = null;
            UseOption = Use.Template;
            VerticalStacking = false;
            //these are used when generating lists of 'Pair' objects for lists that should display something different from their data
            DisplayMemberPath = "Key";
            SelectedValuePath = "Value";

            //manditory parameters
            ControlType = controlType;
            Label = label;
            DependencyProperty = dependencyProperty;

            
        }


    }
}
