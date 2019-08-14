using CardTricks.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CardTricks
{
    /// <summary>
    /// Used to signify that an element property is a target for automatic
    /// binding to the layout model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited= false)]
    public class ModelPropAttribute : Attribute
    {
        public BindingMode BindingMode { get; set; }
        public UpdateSourceTrigger UpdateTrigger { get; set; }
        //public string DependencyProperty { get; set; }
        //public Type ControlType { get; set; }
        public Type Converter { get; set; }
        public BindTarget BindTarget { get; set; }


        public ModelPropAttribute()
        {
            BindTarget = BindTarget.Template;
        }

    }
}
