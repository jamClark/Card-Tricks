using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CardTricks.Interfaces;
using CardTricks.Utils;
using System.ComponentModel;
using CardTricks.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using CardTricks.Windows;
using CardTricks.Attributes;
using System.Reflection;
using System.Runtime.Serialization;
using CardTricks.Models.Elements;

namespace CardTricks.Models
{
    /// <summary>
    /// A container for card elements that are to be associated with a class of card.
    /// </summary>
    [KnownType(typeof(TextRegionElement))]
    [KnownType(typeof(Template))]
    [DataContract]
    public class Template : BaseElement, ITemplate
    {
        #region Public Properties
        [EditProp("Name", typeof(TextBox), "TextProperty", Width = 200, LabelWidth = 80, ToolTip = "The name of this template.", GroupID = 0)]
        [ShallowElementCloneAttribute]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        [EditProp("X Position", typeof(DoubleUpDown), "ValueDependencyProperty", BindingMode = BindingMode.TwoWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged, Converter = typeof(DpiToInchConverter), Width = 100, LabelWidth = 80, GroupID = 0)]
        [ShallowElementCloneAttribute]
        public override double XPos
        {
            get { return base.XPos; }
            set { base.XPos = value;}
        }

        [EditProp("Y Position", typeof(DoubleUpDown), "ValueDependencyProperty", BindingMode = BindingMode.TwoWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged, Converter = typeof(DpiToInchConverter), Width = 100, LabelWidth = 80, GroupID = 0)]
        [ShallowElementCloneAttribute]
        public override double YPos
        {
            get { return base.YPos; }
            set { base.YPos = value; }
        }

        #endregion


        #region Public Methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public Template(string name, ITemplate parentTemplate = null) : base(name, null, parentTemplate)
        {
            //set default property values
            Name = name;
            BaseInitializer();
        }

        public Template() : this("", null)
        {
        }

        protected override void BaseInitializer()
        {
            base.BaseInitializer();
            Width = 2.5 * TemplateUserControl.DPI;
            Height = 3.5 * TemplateUserControl.DPI;

            BindEditProps();
        }
        

        /// <summary>
        /// Creates a control heirarchy that displays all elements
        /// of this template as a WPF control. This control is
        /// data bound to the appropriate properties of this template.
        /// </summary>
        /// <returns></returns>
        public override UIElement GenerateLayout()
        {
            return base.GenerateLayout();
        }

        /// <summary>
        /// This method performs reconciliation on all existing elements so that the old
        /// template matches this one.
        /// Then it adds new elements to the old template. Finally, it tries to reconcile
        /// elements that have been fundamentally changed in the new template. Failing that,
        /// elements that cannot be resolved are simply removed until the old template
        /// matches the new one.
        /// </summary>
        /// <param name="oldTemplate"></param>
        /// <returns></returns>
        public bool ReconcileElementProperties(BaseElement oldTemplate)
        {
            if (!this.Guid.Equals(oldTemplate.Guid)) return false;
            Type sourceType = this.GetType();
            Type oldType = this.GetType();

            //first, let's do the easy stuff and update existing properties for both templates
            foreach (var property in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var targetProperty = oldType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                if (targetProperty != null &&
                    targetProperty.CanWrite &&
                    targetProperty.GetCustomAttribute<ReconcilableProp>(true) != null &&
                    property.GetCustomAttribute<ShallowElementCloneAttribute>(true) != null)
                {
                    targetProperty.SetValue(oldTemplate, property.GetValue(this, null), null);
                }
            }

            List<IElement> newElements = new List<IElement>();
            //now we are going to loop through the list of elements within the template
            //and try to reconcile their properties.
            foreach (IElement element in _Children)
            {
                bool notCopied = true;

                //yucky inner-loop that loop that looks
                //for the matching element within the old template.
                foreach (BaseElement oldElement in oldTemplate.Children)
                {
                    //this automatically checks the Guids to see if
                    //they are the matching element. No worries here.
                    //It will simply return if there is not a match...
                    //or you know... they generated the same Guid somehow ;)
                    if (element.ReconcileElementProperties(oldElement))
                    {
                        notCopied = false;
                        break;
                    }
                }

                //if the element couldn't be found in the old template, we'll need to
                //add it to a list that will be appened later
                if (notCopied)
                {
                    newElements.Add(element);
                }
            }

            //now something tricky. We need to add elements to
            //the old template to match the new one.
            foreach (IElement append in newElements)
            {
                oldTemplate.AddElement( append.GetShallowClone() );
            }

            return false;
        }
        
        #endregion


    }
}
    