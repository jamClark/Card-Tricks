using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CardTricks.Attributes
{
     interface IEditProp
    {
         string Label { get; set; }
         string ToolTip { get; set; }
         double LabelWidth { get; set; }
         double Width { get; set; }
         double Height { get; set; }
         string SelectedItem { get; set; }
         object SelectedValue { get; set; }
         bool IsEnabled { get; set; }
         int GroupID { get; set; }
         bool VerticalStacking { get; set; }
         
         string ItemsSourcePath { get; set; }
         //keys and values are combined into a single Pair that is attched to the itemssource of a collection control
         string KeysSourcePath { get; set; }
         Type KeysType { get; set; }
         string ValuesSourcePath { get; set; }
         Type ValueType { get; set; }
         string DisplayMemberPath { get; set; }
         string SelectedValuePath { get; set; }
         string SelectedItemPath { get; set; }

         BindingMode BindingMode { get; set; }
         UpdateSourceTrigger UpdateTrigger { get; set; }

         Type ControlType { get; set; }
         string DependencyProperty { get; set; }
         Type Converter { get; set; }
    }
}
