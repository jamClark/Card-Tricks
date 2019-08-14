using CardTricks.Attributes;
using CardTricks.Cmds;
using CardTricks.Controls;
using CardTricks.Converters;
using CardTricks.Interfaces;
using CardTricks.Models.Elements;
using CardTricks.Utils;
using CardTricks.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CardTricks.Models
{
    /// <summary>
    /// Base class for all elements and templates.
    /// </summary>
    [KnownType(typeof(TextRegionElement))]
    [KnownType(typeof(RichTextRegion))]
    [KnownType(typeof(StaticImageElement))]
    [DataContract]
    public abstract class BaseElement : ViewModel, IElement
    {
        [NonSerialized]
        public const string InstalledFontFamiliesKey = "$Installed Font Families";
        [NonSerialized]
        public const string TextJustificationsKey = "$Text Justification List";



        #region Private Members
        //basic data - easy to serialize
        protected string _Name = "";
        protected double _XPos = 0;
        protected double _YPos = 0;
        protected double _Width;
        protected double _Height;
        protected bool _Locked = false;
        protected Guid _Guid = System.Guid.NewGuid();

        //object references - look out for cycles
        protected IElement _Inheritance;
        protected IElement _Parent;
        [DataMember(Order = 9, Name="Children")]
        protected List<BaseElement> _Children;
        
        //non-serializable datatypes
        [NonSerialized]
        protected Color _BackgroundColor;
        [NonSerialized]
        protected RotateTransform _Rotation;
        [NonSerialized]
        protected IList<UIElement> _EditableProperties;
        [NonSerialized]
        protected IList<UIElement> _InstanceProperties;
        #endregion


        #region Public Properties
        public virtual event RefreshLayoutEvent RefreshLayout;
        [DataMember]
        public Guid Guid { get { return _Guid; } protected set { _Guid = value; } }
        [DataMember]
        public virtual bool AllowFocus { get; protected set; }
        [DataMember]
        public bool Locked { get { return _Locked; } set { _Locked = value; } }

        [EditProp("ID", typeof(TextBox), "TextProperty", Width = 250, LabelWidth = 80, ToolTip = "The id of this element.", IsEnabled = false, GroupID = 0)]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        [DataMember(Order=1)]
        public virtual string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == null || !_Name.Equals(value))
                {
                    _Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        [EditProp("X Position", typeof(DoubleUpDown), "ValueDependencyProperty", BindingMode = BindingMode.TwoWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged, Converter = typeof(DpiToInchConverter), Width = 100, LabelWidth = 80, GroupID = 0)]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        [DataMember(Order=2)]
        public virtual double XPos
        {
            get { return _XPos; }
            set
            {
                if (_XPos != value)
                {
                    _XPos = value;
                    NotifyPropertyChanged("XPos");
                }
            }
        }

        [EditProp("Y Position", typeof(DoubleUpDown), "ValueDependencyProperty", BindingMode = BindingMode.TwoWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged, Converter = typeof(DpiToInchConverter), Width = 100, LabelWidth = 80, GroupID = 0)]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        [DataMember(Order=3)]
        public virtual double YPos
        {
            get { return _YPos; }
            set
            {
                if (_YPos != value)
                {
                    _YPos = value;
                    NotifyPropertyChanged("YPos");
                }
            }
        }

        [EditProp("Width", typeof(DoubleUpDown), "ValueDependencyProperty", BindingMode = BindingMode.TwoWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged, Converter = typeof(DpiToInchConverter), Width = 100, LabelWidth = 80, GroupID = 0)]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        [DataMember(Order=4)]
        public virtual double Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    _Width = value;
                    NotifyPropertyChanged("Width");
                }
            }
        }

        [EditProp("Height", typeof(DoubleUpDown), "ValueDependencyProperty", BindingMode = BindingMode.TwoWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged, Converter = typeof(DpiToInchConverter), Width = 100, LabelWidth = 80, GroupID = 0)]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        [DataMember(Order=5)]
        public virtual double Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }

        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        [DataMember(Order=5)]
        public virtual double Angle
        {
            get { return Rotation.Angle; }
            set
            {
                if (Rotation == null || Rotation.Angle != value)
                {
                    if (Rotation == null) Rotation = new RotateTransform();
                    Rotation.Angle = value; //triggers 'Rotation' property to change
                    NotifyPropertyChanged("Angle");
                }
            }
        }

        [EditProp("Rotation", typeof(DoubleUpDown), "ValueDependencyProperty", BindingMode = BindingMode.TwoWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged, Converter = typeof(TransformToAngleConverter), Width = 100, LabelWidth = 80, GroupID = 0)]
        public virtual RotateTransform Rotation
        {
            get { return _Rotation; }
            set
            {
                if (_Rotation == null || _Rotation != value)
                {
                    _Rotation = value;
                    NotifyPropertyChanged("Rotation");
                }
            }
        }

        [EditProp("Background Color", typeof(ColorPicker), "SelectedColorAdvProperty", Width = 150, LabelWidth = 80, GroupID = 0)]
        [ModelProp(BindingMode = BindingMode.TwoWay, Converter = typeof(ColorToSolidColorBrushConverter))]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        [DataMember(Order=6)]
        public virtual Color BackgroundColor
        {
            get { return _BackgroundColor; }
            set
            {
                if ((_BackgroundColor == null && value != null) || _BackgroundColor != value)
                {
                    _BackgroundColor = value;
                    NotifyPropertyChanged("BackgroundColor");
                }
            }
        }

        [ShallowElementCloneAttribute]
        //[DataMember(Order=7)]
        public virtual IElement Inheritance
        {
            get { return _Inheritance; }
            set
            {
                if (_Inheritance != value)
                {
                    _Inheritance = value;
                    NotifyPropertyChanged("Inheritance");
                }
            }
        }

        [ShallowElementCloneAttribute]
        [DataMember(Order=8)]
        public virtual IElement Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent != value)
                {
                    _Parent = value;
                    NotifyPropertyChanged("Parent");
                }
            }
        }

        
        public virtual List<BaseElement> Children
        {
            get
            {
                InitElementList();
                return _Children;
            }
            set
            {
                if (_Children != value)
                {
                    _Children = value as List<BaseElement>;
                    NotifyPropertyChanged("Children");
                }
            }
        }

        [ShallowElementCloneAttribute]
        public virtual IList<UIElement> EditableProperties
        {
            get { return _EditableProperties; }
            set
            {
                if (_EditableProperties != value)
                {
                    _EditableProperties = value;
                    NotifyPropertyChanged("EditableProperties");
                }
            }
        }

        //TODO: We need a way of recovering these when reloading
        public virtual IList<UIElement> InstanceProperties
        {
            get { return _InstanceProperties; }
            set
            {
                if (_InstanceProperties != value)
                {
                    _InstanceProperties = value;
                    NotifyPropertyChanged("InstanceProperties");
                }
            }
        }
        
        public UserControl LastLayoutInstance { get; protected set; }

        public virtual string ContentToString
        {
            get { return Name; }
            protected set { Name = value; }
        }
        #endregion


        #region Events
        protected event NameChangedEvent NameChangeHandler;
        public virtual event NameChangedEvent NameChanged
        {
            add { NameChangeHandler += value; }
            remove { NameChangeHandler -= value; }
        }

        public void OnChangeName()
        {
            if(NameChangeHandler != null) NameChangeHandler();
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public BaseElement(string name, IElement parent, IElement inheritance = null)
        {
            BaseInitializer();

            if(string.IsNullOrEmpty(name)) Name = _Guid.ToString();
            else Name = name;
            Parent = parent;
            Inheritance = inheritance;
        }

        /// <summary>
        /// Default constructor. This is for used to create elements of unknowtypes using reflective
        /// instance creation as well as allow the DataContract serializer to initialize 'blank-state' instances.
        /// </summary>
        public BaseElement() : this("", null, null)
        {
            
        }

        /// <summary>
        /// Used to initialize all default values for this object. Primarily
        /// used so that DataContract deserialization can properly set up everything.
        /// </summary>
        protected virtual void BaseInitializer()
        {
            _Guid = System.Guid.NewGuid();
            InitElementList();
            EditableProperties = new List<UIElement>();
            InstanceProperties = new List<UIElement>();
            Rotation = new RotateTransform();
            BackgroundColor = Colors.LightGray;

            //set default size in inches
            Width = 2.5 * TemplateUserControl.DPI;
            Height = 3.5 * TemplateUserControl.DPI;
        }

        /// <summary>
        /// Callback used to initalize default data for this object as well as handle
        /// any specialized/shared data conversion.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            BaseInitializer();
        }

        /// <summary>
        /// Attempts to set the id of this element. It first checks to see if any siblings
        /// are using this id. If the id is already in use it throws an Exception instead.
        /// </summary>
        /// <param name="newID"></param>
        public void ChangeID(string newID)
        {
            foreach (BaseElement sib in this.Parent.Children)
            {
                if(sib.Name.Equals(newID)) throw new Exception("The id " + newID + " is already in use by another element.");
            }
            Name = newID;
        }

        /// <summary>
        /// Adds a new element to the template.
        /// </summary>
        /// <param name="element"></param>
        public virtual void AddElement(BaseElement element)
        {
            InitElementList();
            _Children.Add(ConvertElement(element));
            element.Parent = this;
        }

        /// <summary>
        /// Removes the given element from the template.
        /// </summary>
        /// <param name="element">The element to remove</param>
        /// <returns>Returns true if the element was successfully removed. False if it did not exist within this template.</returns>
        public bool RemoveElement(BaseElement element)
        {
            InitElementList();
            bool result = _Children.Remove(ConvertElement(element));
            element.Parent = null;
            return result;
        }

        /// <summary>
        /// Removes the first instance of an element that has the given name.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        /// <returns>Returns true if the element was successfully removed. False if it did not exist within this template.</returns>
        public bool RemoveElementByName(string name)
        {
            if (_Children == null || _Children.Count < 1 || name == null || name.Length < 1) return false;
            BaseElement elm = null;
            foreach (BaseElement element in _Children)
            {
                if (element.Name == name) elm = element;
            }

            if (elm != null)
            {
                bool result = RemoveElement(elm);
                elm.Parent = null;
                return result;
            }
            return false;
        }

        /// <summary>
        /// Creates a control heirarchy that displays all elements
        /// of this template as a WPF control.
        /// </summary>
        /// <returns></returns>
        public virtual UIElement GenerateLayout()
        {
            //setup this element's layout
            TemplateUserControl control = new TemplateUserControl(this);
            BindLayoutToEditProps(control);
            BindModelHelper(control.layoutsurfaceContent, "BackgroundColor", TemplateUserControl.BackgroundProperty);
            //TODO: add parent elements here

            //now attach child elements
            foreach(BaseElement elm in _Children)
            {
                if(elm != null)
                    control.LayoutSurface.Children.Add(elm.GenerateLayout());
            }

            LastLayoutInstance = control;
            return control;
        }

        /// <summary>
        /// Updates a give template to reflect this one. Elements that
        /// share a name and datatype are transferred to the best ability.
        /// Elements that are not present in this one are removed.
        /// </summary>
        /// <param name="template"></param>
        public bool ReconcileElementProperties(BaseElement oldElement, bool reconcileOverrides = false)
        {
            //This assumes we already have an an element that matches this element's Guid
            if (!this.Guid.Equals(oldElement.Guid)) return false;
            Type sourceType = this.GetType();
            Type oldType = oldElement.GetType();

            //first, let's do the easy stuff and update existing properties for both templates
            foreach (var property in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var targetProperty = oldType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                if (targetProperty != null &&
                    targetProperty.CanWrite &&
                    targetProperty.GetCustomAttribute<ReconcilableProp>(true) != null &&
                    property.GetCustomAttribute<ReconcilableProp>(true) != null)
                {
                    if (targetProperty.GetCustomAttribute<ReconcilableProp>(true).Override == reconcileOverrides)
                    {
                        //edit property to reflect template changes
                        targetProperty.SetValue(oldElement, property.GetValue(this, null), null);
                    }
                }
            }
            

            //TODO: We must have an 'intelligent' mechinism here that can attempt to compare
            //properties of different Guids but the same ID. If IDs match, it tries to convert
            //the data from the old element format to the new one. Otherwise it must delete that data.


            //TODO: We must remove elements from the card that were removed from the master template
            //This will be dangerous and requires mitigation from the above mechanism as well as plenty
            //of warnings to the user about what they are doing.


            //now we must compare each child in this element with each child
            //of the other element. If one of them matches, we need to reconcile them
            foreach (BaseElement child in _Children)
            {
                //update: this will automagically compare Guids within the call now
                foreach (BaseElement targetChild in oldElement.Children)
                {
                    child.ReconcileElementProperties(targetChild);
                }
            }

            return true;
        }

        /// <summary>
        /// Copies this element to another element using shallow copying
        /// of all basic properties. Child elements however, are still
        /// deeply copied using recursion. This will not handle indexed properties.
        /// </summary>
        /// <returns></returns>
        public BaseElement GetShallowClone()
        {
            BaseElement dest = Activator.CreateInstance(this.GetType()) as BaseElement;
            if(dest == null) return null;
            Type sourceType = this.GetType();
            Type targetType = dest.GetType();

            //copy all values of all elements marked with a clone attribute
            foreach (var property in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var targetProperty = targetType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                if (targetProperty != null &&
                    targetProperty.CanWrite &&
                    targetProperty.PropertyType.IsAssignableFrom(property.PropertyType) &&
                    targetProperty.GetCustomAttribute<ShallowElementCloneAttribute>(true) != null &&
                    property.GetCustomAttribute<ShallowElementCloneAttribute>(true) != null)
                {
                    targetProperty.SetValue(dest, property.GetValue(this, null), null);
                }
                else if (targetProperty != null &&
                        targetProperty.CanWrite &&
                        targetProperty.PropertyType.IsAssignableFrom(property.PropertyType) &&
                        targetProperty.GetCustomAttribute<DeepElementCloneAttribute>(true) != null &&
                        property.GetCustomAttribute<DeepElementCloneAttribute>(true) != null)
                {
                    try
                    {
                        targetProperty.SetValue(dest, Utils.ObjectHelper.Clone(property.GetValue(this, null)), null);
                    }
                    catch (ArgumentException e)
                    {
                        MessageBox.Show("Failed to deep copy the property '" + property.Name + "' of the element '" + this._Name + "'.\n" + e.Message);
                    }
                }
                else if(
                    targetProperty != null &&
                    targetProperty.CanWrite &&
                    targetProperty.PropertyType.IsAssignableFrom(property.PropertyType) &&
                    targetProperty.GetCustomAttribute<ContentElementCloneAttribute>(true) != null &&
                    property.GetCustomAttribute<ContentElementCloneAttribute>(true) != null)
                {
                    this.DeepClone(property, this, targetProperty, dest); 
                }
            }

            
            //manually copy InstanceProperties
            //We need to generate the InstanceEditProperty controls and bind them here.
            dest.BindInstanceEditProps();
            //TODO: set default values to binding property

            //now deep shallow-copy of the children
            foreach(BaseElement child in _Children)
            {
                dest.Children.Add(child.GetShallowClone());
            }

            //we use reflection to get the private Guid value.
            //We do this in an effort to avoid letting overriding
            //classes mess with the Guid
            FieldInfo sourceGuid = sourceType.GetField("_Guid", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo destGuid = targetType.GetField("_Guid", BindingFlags.NonPublic | BindingFlags.Instance);
            destGuid.SetValue(dest, sourceGuid.GetValue(this));
            return dest;
        }

        /// <summary>
        /// Used to trigger all attached refresh events for the layout.
        /// Often, the parent container will attach to this event so
        /// that it knows when to redraw the template and it's controls
        /// using GenerateLayout().
        /// </summary>
        public void TriggerRefreshLayout()
        {
            if (RefreshLayout != null) RefreshLayout();
        }

        public int GetLayerCount()
        {
            return this._Children.Count;
        }

        public BaseElement GetElementAtLayer(int layer)
        {
            if (layer >= _Children.Count) return null;
            if (layer < 0) return null;

            return _Children[layer];
        }

        public void MoveElementToFront(BaseElement element)
        {
            if(_Children.Count < 1) return;
            int elementLayer = 0;
            foreach (BaseElement elm in _Children)
            {
                if (elm == element) break;
                elementLayer++;
            }
            
            if(elementLayer >= _Children.Count-1) return;
            //we must shift all elements that are above this one
            //down by 1 and then stick this one at the end
            for (int i = elementLayer; i < _Children.Count-1; i++)
            {
                _Children[i] = _Children[i+1];
            }

            _Children[_Children.Count - 1] = ConvertElement(element);
        }

        public void MoveElementToBack(BaseElement element)
        {
            if (_Children.Count < 1) return;
            int elementLayer = 0;
            foreach (BaseElement elm in _Children)
            {
                if (elm == element) break;
                elementLayer++;
            }

            if (elementLayer <= 0) return;
            for (int i = elementLayer; i >= 1; i--)
            {
                _Children[i] = _Children[i - 1];
            }

            _Children[0] = ConvertElement(element);
        }

        public void MoveElementDown(BaseElement element)
        {
            if (_Children.Count < 1) return;
            int elementLayer = 0;
            foreach (BaseElement elm in _Children)
            {
                if (elm == element) break;
                elementLayer++;
            }

            if (elementLayer <= 0) return;
            //move the other guy up
            _Children[elementLayer] = _Children[elementLayer-1];
            //move this guy down
            _Children[elementLayer - 1] = ConvertElement(element);
        }

        public void MoveElementUp(BaseElement element)
        {
            if (_Children.Count < 1) return;
            int elementLayer = 0;
            foreach (BaseElement elm in _Children)
            {
                if (elm == element) break;
                elementLayer++;
            }

            if (elementLayer >= _Children.Count-1) return;
            //move the other guy down
            _Children[elementLayer] = _Children[elementLayer+1];
            //move this guy up
            _Children[elementLayer + 1] = ConvertElement(element);
        }

        public void TriggerManualUpdates()
        {
            HandleManualUpdates();
            foreach (BaseElement element in _Children) element.TriggerManualUpdates();
        }

        public virtual void HandleManualUpdates()
        {
        }

        /// <summary>
        /// Returns all instance properties of this element and all of it's children.
        /// </summary>
        /// <returns></returns>
        public IList<UIElement> GetHierachyOfInstanceProperties()
        {
            List<UIElement> list = new List<UIElement>();
            foreach (BaseElement child in _Children)
            {
                list.AddRange(child.GetHierachyOfInstanceProperties());
            }

            if(this.InstanceProperties != null) list.AddRange(this.InstanceProperties);
            return list;
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        private void InitElementList()
        {
            if (_Children == null) _Children = new List<BaseElement>();
        }

        /// <summary>
        /// Utility for converting interfaces. Mostly used for deserialization
        /// for lack of any proper duck-typing or macro mechinisms in C#.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private BaseElement ConvertElement(IElement element)
        {
            return element as BaseElement;
        }

        /// <summary>
        /// Utility methods that is used to bind the EditProps of the
        /// BaseElement model to the layout elements.
        /// </summary>
        protected virtual void BindLayoutToEditProps(Control control)
        {
            //Bind this model to the element
            Binding bindXPos = new Binding("XPos");
            bindXPos.Source = this;
            bindXPos.Mode = BindingMode.TwoWay;
            control.SetBinding(Canvas.LeftProperty, bindXPos);

            Binding bindYPos = new Binding("YPos");
            bindYPos.Source = this;
            bindYPos.Mode = BindingMode.TwoWay;
            control.SetBinding(Canvas.TopProperty, bindYPos);

            Binding bindWidth = new Binding("Width");
            bindWidth.Source = this;
            bindWidth.Mode = BindingMode.TwoWay;
            control.SetBinding(TemplateUserControl.WidthProperty, bindWidth);

            Binding bindHeight = new Binding("Height");
            bindHeight.Source = this;
            bindHeight.Mode = BindingMode.TwoWay;
            control.SetBinding(TemplateUserControl.HeightProperty, bindHeight);

            Binding bindRotation = new Binding("Rotation");
            bindRotation.Source = this;
            bindRotation.Mode = BindingMode.TwoWay;
            bindRotation.Converter = new RotateToTransformConverter();
            control.SetBinding(TemplateUserControl.RenderTransformProperty, bindRotation);

        }

        /// <summary>
        /// Useses reflection to search for all properties of 
        /// this class that have the EditProp attribute and
        /// create and bind the appropriate generated control
        /// for the Edit Properties panel.
        /// </summary>
        protected virtual void BindEditProps()
        {
            Type type = this.GetType();
            PropertyInfo[] propInfo = type.GetProperties();
            
            Dictionary<int, List<UIElement>> templateGroups = new Dictionary<int,List<UIElement>>();
            Dictionary<int, List<UIElement>> instanceGroups = new Dictionary<int, List<UIElement>>();
            foreach (PropertyInfo prop in propInfo)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    EditPropAttribute editPropAttr = attr as EditPropAttribute;
                    if (editPropAttr != null)
                    {
                        //makes the controls and bind them to the property in question
                        StackPanel stack = BindEditPropHelper(editPropAttr, prop);
                        
                        if (templateGroups.ContainsKey(editPropAttr.GroupID))
                        {
                            templateGroups[editPropAttr.GroupID].Add(stack);
                        }
                        else
                        {
                            List<UIElement> list = new List<UIElement>();
                            list.Add(stack);
                            templateGroups.Add(editPropAttr.GroupID, list);
                        }
                    }

                }

            }

            //sort by group id and add properties in-order
            List<int> sortedList = templateGroups.Keys.ToList();
            sortedList.Sort();
            foreach(int group in sortedList)
            {
                foreach(UIElement element in templateGroups[group])
                {
                    EditableProperties.Add(element);
                }
            }
        }

        /// <summary>
        /// Similar to BindEditProps(), but this is called from GetShallowClone
        /// to peform (of all things) a deep clone of the properties that
        /// have been flagged with the InstanceEditProps attribute.
        /// This properties will then display controls for instances
        /// of templates (cards) and will be bound to thoses instances
        /// rather than the master template.
        /// 
        /// NOTE: The current system is flawed in that a reconciliation of
        /// instances will revert per-instance changes made using these controls.
        /// This makes them significantly less useful in a lot of cases as
        /// they can never be 'future-proofed'.
        /// </summary>
        public virtual void BindInstanceEditProps()
        {
            Type type = this.GetType();
            PropertyInfo[] propInfo = type.GetProperties();

            Dictionary<int, List<UIElement>> templateGroups = new Dictionary<int, List<UIElement>>();
            Dictionary<int, List<UIElement>> instanceGroups = new Dictionary<int, List<UIElement>>();
            foreach (PropertyInfo prop in propInfo)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    InstanceEditPropAttribute instancePropAttr = attr as InstanceEditPropAttribute;
                    if (instancePropAttr != null)
                    {
                        //makes the controls and bind them to the property in question
                        StackPanel stack = BindEditPropHelper(instancePropAttr, prop);

                        if (instanceGroups.ContainsKey(instancePropAttr.GroupID))
                        {
                            instanceGroups[instancePropAttr.GroupID].Add(stack);
                        }
                        else
                        {
                            List<UIElement> list = new List<UIElement>();
                            list.Add(stack);
                            instanceGroups.Add(instancePropAttr.GroupID, list);
                        }
                    }

                }

            }

            //sort by group id and add properties in-order
            List<int> sortedList = instanceGroups.Keys.ToList();
            sortedList.Sort();
            foreach (int group in sortedList)
            {
                foreach (UIElement element in instanceGroups[group])
                {
                    InstanceProperties.Add(element);
                }
            }

        }

        /// <summary>
        /// Helper method for binding controls to a property that has the EditProp attribute.
        /// </summary>
        /// <param name="editPropAttr"></param>
        /// <param name="prop"></param>
        private StackPanel BindEditPropHelper(IEditProp editPropAttr, PropertyInfo prop)
        {
            //first, create the label and control associated with this property.
            TextBlock label = new TextBlock();
            label.TextTrimming = TextTrimming.CharacterEllipsis;
            label.Text = editPropAttr.Label;
            label.Width = editPropAttr.LabelWidth;
            label.Margin = new Thickness(2);
            if (editPropAttr.ToolTip != null && editPropAttr.ToolTip.Length > 0) label.ToolTip = editPropAttr.ToolTip;

            Control con = Activator.CreateInstance(editPropAttr.ControlType) as Control;
            con.Width = editPropAttr.Width;
            con.Height = editPropAttr.Height;
            con.Margin = new Thickness(2);
            con.IsEnabled = editPropAttr.IsEnabled;
            if (editPropAttr.ToolTip != null && editPropAttr.ToolTip.Length > 0) con.ToolTip = editPropAttr.ToolTip;

            //stack 'em
            StackPanel stack = new StackPanel();
            stack.FlowDirection = FlowDirection.LeftToRight;
            if (editPropAttr.VerticalStacking) stack.Orientation = Orientation.Vertical;
            else stack.Orientation = Orientation.Horizontal;
            stack.Children.Add(label);
            stack.Children.Add(con);

            //bind the control to the property
            //ALERT: We are NOT handling exceptions here because we want them to throw
            //during develoment so that we can find invalid property names.
            //TODO: Decide a proper development macro to use to enable/disable this
            DependencyProperty dependency = DependencyObjectHelper.GetDependencyProperty(
                                                                    editPropAttr.ControlType,
                                                                    editPropAttr.DependencyProperty);
            Binding bindVal = new Binding(prop.Name);
            bindVal.Source = this;
            bindVal.Mode = editPropAttr.BindingMode;
            bindVal.UpdateSourceTrigger = editPropAttr.UpdateTrigger;
            if (editPropAttr.Converter != null) bindVal.Converter = Activator.CreateInstance(editPropAttr.Converter) as IValueConverter;
            BindingOperations.SetBinding(con, dependency, bindVal);

            //check for additional binding info
            Type type = this.GetType();
            var keysProperty = (editPropAttr.KeysSourcePath == null) ? null : type.GetProperty(editPropAttr.KeysSourcePath, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
            var valuesProperty = (editPropAttr.ValuesSourcePath == null) ? null : type.GetProperty(editPropAttr.ValuesSourcePath, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
            System.Collections.IEnumerable keys = (keysProperty == null) ? null : keysProperty.GetValue(this) as System.Collections.IEnumerable;
            System.Collections.IEnumerable values = (valuesProperty == null) ? null : valuesProperty.GetValue(this) as System.Collections.IEnumerable;
            IList<Pair> pairs = null;

            //combine keys and values (if available) into a single paired ItemsSource list
            if (keys != null && values != null)
            {
                System.Collections.IEnumerator keysEnum = keys.GetEnumerator();
                System.Collections.IEnumerator valuesEnum = values.GetEnumerator();
                pairs = new List<Pair>();
                do
                {
                    if (!keysEnum.MoveNext()) break;
                    if (!valuesEnum.MoveNext()) break;
                    pairs.Add(new Pair(keysEnum.Current, valuesEnum.Current));
                } while (true);

            }

            //if the control exposes an items list then we will fill it out
            //either using the ItemsSource property of the attribute or a
            // a list of paired values (see above) if available
            ItemsControl itemsCon = con as ItemsControl;
            if (itemsCon != null)
            {
                //setting a paired source
                if (pairs != null)
                {
                    itemsCon.ItemsSource = pairs;
                    itemsCon.DisplayMemberPath = editPropAttr.DisplayMemberPath;
                }
                else
                {
                    var itemsProperty = (editPropAttr.ItemsSourcePath == null) ? null : type.GetProperty(editPropAttr.ItemsSourcePath, BindingFlags.Public | BindingFlags.Instance);
                    System.Collections.IEnumerable items = (itemsProperty == null) ? null : itemsProperty.GetValue(this) as System.Collections.IEnumerable;
                    itemsCon.ItemsSource = items;
                }

            }

            //if the control exposes a selector that will be set as well.
            Selector selector = con as Selector;
            if (selector != null)
            {
                if (pairs != null) selector.SelectedValuePath = editPropAttr.SelectedValuePath;
                if (editPropAttr.SelectedValuePath != null)
                {
                    PropertyInfo p = type.GetProperty(editPropAttr.SelectedValuePath);
                    if(p != null)    selector.SelectedValue = p.GetValue(this);
                    if (selector.SelectedValue == null) selector.SelectedValue = editPropAttr.SelectedValue;
                }
                else selector.SelectedValue = editPropAttr.SelectedValue;
            }

            return stack;
        }
        
        /// <summary>
        /// Helps bind a control's DependencyProperty to a local property of this object.
        /// It relies on the local property having been flagged with a ModelPropAttribute.
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="modelPropertyName"></param>
        /// <param name="depProp"></param>
        protected virtual void BindModelHelper(DependencyObject dependencyObject, string modelPropertyName, DependencyProperty depProp, IValueConverter converter = null)
        {
            Type type = this.GetType();
            PropertyInfo prop = type.GetProperty(modelPropertyName);
            if (dependencyObject == null || !(dependencyObject is DependencyObject))
            {
                MessageBox.Show("Invalid DependencyProperty for '" + modelPropertyName + "' supplied to the element '" + this.Name + "'.");
                return;
            }
            if (prop == null)
            {
                MessageBox.Show("Invalid Model Property for '" + modelPropertyName + "' supplied to the element '" + this.Name + "'.");
                return;
            }

            //check for binding attributes
            ModelPropAttribute attr = prop.GetCustomAttribute(typeof(ModelPropAttribute)) as ModelPropAttribute;
            if (attr != null)
            {
                Binding bind = new Binding(prop.Name);
                bind.Source = this;
                bind.Mode = attr.BindingMode;
                bind.UpdateSourceTrigger = attr.UpdateTrigger;
                if (converter == null)
                {
                    if (attr.Converter != null)
                        bind.Converter = Activator.CreateInstance(attr.Converter) as IValueConverter;
                }
                else
                {
                    bind.Converter = converter;
                }
                BindingOperations.SetBinding(dependencyObject as DependencyObject, depProp, bind);

                
            }

        }

        /// <summary>
        /// This default implementation in fact does not peform a deep copy. It is a stand-in
        /// shallow copy using reflection of the object's proeprties. For a proper deep-copy,
        /// override this method and implement according to the Element's _Content datatype.
        /// </summary>
        /// <param name="sourceProp"></param>
        /// <param name="sourceElm"></param>
        /// <param name="destProp"></param>
        /// <param name="destElm"></param>
        protected virtual void DeepClone(PropertyInfo sourceProp, BaseElement sourceElm, PropertyInfo destProp, BaseElement destElm)
        {
            destProp.SetValue(destElm, sourceProp.GetValue(sourceElm, null), null);
        }
        #endregion

    }


}


