using CardTricks.Attributes;
using CardTricks.Controls;
using CardTricks.Converters;
using CardTricks.Interfaces;
using CardTricks.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace CardTricks.Models.Elements
{
    /// <summary>
    /// An element that allows users to create a region of text that is
    /// clipped to the region of the control. Text is wrapped if it spans
    /// too far horizontally and is clipped if it spans to far vertically.
    /// </summary>
    //[KnownType(typeof(IElement))]
    [Serializable]
    [DataContract]
    public class TextRegionElement : BaseElement, ITemplateElement<string>
    {
        #region Private Members
        [DataMember(Name="Text", Order=1)]
        private string _Content;
        [NonSerialized]
        private FontFamily _FontFamily;
        [NonSerialized]
        [DataMember]
        private Color _FontColor;
        [DataMember]
        private double _FontMinSize;
        [DataMember]
        private double _FontMaxSize;
        [DataMember]
        private FontWeight _FontWeight;
        [DataMember]
        private Nullable<bool> _SpellChecking;
        [DataMember]
        private string _SelectedFont = "Arial";
        [DataMember]
        private TextAlignment _Alignment;
        #endregion


        #region Public Properties
        //This 'Content' property must exist for the interface to work.
        //It represent the core point of data for this element.
        [ModelProp(BindingMode = BindingMode.TwoWay, UpdateTrigger=UpdateSourceTrigger.PropertyChanged, BindTarget=BindTarget.Auto)]
        [ShallowElementCloneAttribute]
        public string Content
        {
            get
            {
                return _Content;
            }

            set
            {
                if ((_Content == null && value != null) || !_Content.Equals(value))
                {
                    _Content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }
        
        //BUG ALERT: We can't guarentee that this gets copied first during a clone. If it isn't copied first
        //the the selected item will be out of sync in card instances! For now, the best we can hope
        //for is that it will be copied first if we put it above the 'FontFamily' property.
        //We will likely need a parameter for the shallow copy attribute that specifies order.
        [EditProp(
            "Font Family",
            typeof(ComboBox),
            Width = 190,
            LabelWidth = 80,
            ItemsSourcePath = "InstalledFonts",
            SelectedValuePath = "SelectedFont",
            BindingMode = BindingMode.TwoWay)]
        [InstanceEditProp(
            "Font Family",
            typeof(ComboBox),
            Width = 190,
            LabelWidth = 80,
            ItemsSourcePath = "InstalledFonts",
            SelectedValuePath = "SelectedFont",
            BindingMode = BindingMode.TwoWay)]
        [ShallowElementCloneAttribute]
        public string SelectedFont
        {
            get { return _SelectedFont; }
            set
            {
                if (!_SelectedFont.Equals(value))
                {
                    _SelectedFont = value;
                    FontFamily = new FontFamily(value);
                    NotifyPropertyChanged("SelectedFont");
                }
            }
        }
        /*
        [EditProp(
            "Font Family", 
            typeof(ComboBox), 
            Width = 190, 
            LabelWidth = 80, 
            ItemsSourcePath = "InstalledFonts", 
            SelectedValuePath = "SelectedFont",
            BindingMode = BindingMode.OneWayToSource)]
        [InstanceEditProp(
            "Font Family",
            typeof(ComboBox),
            Width = 190,
            LabelWidth = 80,
            ItemsSourcePath = "InstalledFonts",
            SelectedValuePath = "SelectedFont",
            BindingMode = BindingMode.OneWayToSource)]
         * */
        [ModelProp(BindingMode = BindingMode.OneWay)]
        [ShallowElementCloneAttribute]
        [ReconcilableProp(Override=true)] //the 'Override' param allows us to avoid reverting card instance fonts when we reconcile changes
        public FontFamily FontFamily
        {
            get { return _FontFamily; }
            set
            {
                if ((_FontFamily == null && value != null) || _FontFamily != value)
                {
                    _FontFamily = value;
                    NotifyPropertyChanged("FontFamily");
                    //_SelectedFont = FontFamily.Source;
                }
            }
        }
        public string[] InstalledFonts { get { return Utils.FontHelper.InstalledFamilies; } }
        
        [EditProp("Max Size", typeof(DoubleUpDown), "ValueDependencyProperty", BindingMode = BindingMode.TwoWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged, Width = 100, LabelWidth = 80)]
        [ModelProp(BindingMode=BindingMode.TwoWay, UpdateTrigger=UpdateSourceTrigger.PropertyChanged)]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        public double FontMaxSize
        {
            get { return _FontMaxSize; }
            set
            {
                if (_FontMaxSize != value)
                {
                    _FontMaxSize = value;
                    NotifyPropertyChanged("FontMaxSize");
                }
            }
        }

        [EditProp("Font Color", typeof(ColorPicker), "SelectedColorAdvProperty", Width = 150, LabelWidth=80, GroupID = 1)] //use this GroupID to get it at the top of the list
        [ModelProp(BindingMode = BindingMode.TwoWay, Converter=typeof(ColorToSolidColorBrushConverter))]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        public Color FontColor
        {
            get { return _FontColor; }
            set
            {
                if ((_FontColor == null && value != null) || _FontColor != value)
                {
                    _FontColor = value;
                    NotifyPropertyChanged("FontColor");
                }
            }
        }

        [EditProp("Spell Check", typeof(CheckBox), "IsCheckedProperty", Width=150, LabelWidth=80, GroupID = 1, BindingMode=BindingMode.TwoWay, UpdateTrigger=UpdateSourceTrigger.PropertyChanged)]
        [ModelProp(UpdateTrigger = UpdateSourceTrigger.PropertyChanged)]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        public Nullable<bool> SpellChecking
        {
            get { return _SpellChecking; }
            set
            {
                if (_SpellChecking != value)
                {
                    _SpellChecking = value;
                    NotifyPropertyChanged("SpellChecking");
                }
            }
        }

        [ShallowElementCloneAttribute]
        //[ReconcilableProp]
        public FontWeight FontWeight
        {
            get { return _FontWeight; }
            set
            {
                if ((_FontWeight == null && value != null) || _FontWeight != value)
                {
                    _FontWeight = value;
                    NotifyPropertyChanged("FontWeight");
                }
            }
        }

        
        [EditProp(
            "Alignment",
            typeof(ComboBox),
            "SelectedItemProperty",
            Width = 150,
            LabelWidth = 80,
            GroupID = 1,
            UpdateTrigger = UpdateSourceTrigger.PropertyChanged,
            BindingMode = BindingMode.OneWayToSource,
            Converter = typeof(TextAlignmentToPairConverter),
            KeysSourcePath = "AlignmentNames",
            ValuesSourcePath = "Alignments",
            SelectedValue = TextAlignment.Center)]
        [ModelProp(BindingMode = BindingMode.OneWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged)]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        public TextAlignment Alignment
        {
            get { return _Alignment; }
            set
            {
                if (_Alignment != value)
                {
                    _Alignment = value;
                     NotifyPropertyChanged("Alignment");
                }
            }
        }
        private string[] AlignmentNames { get { return new string[] { "Left", "Right", "Center", "Justify" }; } }
        private TextAlignment[] Alignments
        {
            get
            {
                return new TextAlignment[] { TextAlignment.Left, TextAlignment.Right, TextAlignment.Center, TextAlignment.Justify };
            }
        }


        /// <summary>
        /// This allows us to get a user-friendly identifying string
        /// from our content. This is what shows up in the sets and
        /// decks list when this element is tagged with '$Name'.
        /// </summary>
        public override string ContentToString
        {
            get { return _Content.Trim(); }
        }
        #endregion


        #region Methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public TextRegionElement(string name, ITemplate parent, IElement inheritsFrom = null) : base(name, parent, inheritsFrom)
        {
            //Be sure to set the default property values here
            //or you might find your control misbehaving.
            //This should also be done before the binding of EditProps.
            //Name = "Text Region";
            InitTextRegion();

            //be *sure* to do this if you want your EditProps to
            //show up on the side panel. And do it *after*
            //you have set your default values.
            BindEditProps();
        }

        /// <summary>
        /// Default constructor is needed for GetShallowCopy() to work.
        /// Also used by DataContract (de)serialization. Under normal
        /// circumstances, it should never be called directly.
        /// </summary>
        public TextRegionElement() : this(null, null, null)
        {
        }

        /// <summary>
        /// Utility for setting default data for this object.
        /// </summary>
        protected virtual void InitTextRegion()
        {
            Width = 100;
            Height = 100;
            Content = "<Text Region Element>";
            _FontFamily = new FontFamily("Arial");
            _FontColor = Colors.Black;
            _FontMinSize = 10.0;
            _FontMaxSize = 10.0;
            _BackgroundColor = Colors.Transparent;
            _SpellChecking = false;
            _Alignment = TextAlignment.Right;
            _SelectedFont = "Arial";

            //This flag is very important!
            //We need to set this flag in order to allow the keyboard
            //to focus on the textbox when we are editing cards - not
            //the template itself, but the actual cards using the template.
            //For things that can't be edited like static images, this
            //should remain false so that we can click 'through' this
            //element and drag the whole card.
            AllowFocus = true;
        }

        /// <summary>
        /// This is called after the DataContract has been deserialized. We use
        /// it to initialize default data as well as convert our saved font-family
        /// string into a non-saved FontFamily object.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context) 
        {
            InitTextRegion();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            //we need to re-create the font object based on the saved font name string
            _FontFamily = new FontFamily(_SelectedFont);
            TextAlignment temp = Alignment;

            //Again! Like in the constructor, we *must* call this and only *after*
            //all of the data has been recovered/initialized for this element.
            BindEditProps();
            BindInstanceEditProps();

            //actually, I lied. In this case we need to temporaliy store the info (above) and then
            //recover it properly after the edit prop binding is done mangling the data.
            Alignment = temp;
        }

        /// <summary>
        /// Provides the control layout of our element as it will appear on the template.
        /// </summary>
        /// <returns></returns>
        public override UIElement GenerateLayout()
        {
            //This will be out physical representation
            //of the text on the layout. A simple textbox
            //with a reasonable style. Note that the border
            //and background are set to transparent so that
            //they don't render but both are also bound
            //as the EditProp 'Background' wich allows them
            //to be set using a property control on the side panel.
            TextBox textblock = new TextBox();
            textblock.Text = _Content;
            textblock.Width = Double.NaN;
            textblock.Height = Double.NaN;
            textblock.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblock.VerticalAlignment = VerticalAlignment.Stretch;
            textblock.Margin = new Thickness(8);
            textblock.TextWrapping = TextWrapping.Wrap;
            textblock.AcceptsReturn = true;
            textblock.Name = "TextContent";
            textblock.TextChanged += TextHandler;
            
            //We must always *always* *Always* wrap our content inside one of these.
            ElementUserControl control = new ElementUserControl(this);
            control.AddChild(textblock);

            //Here we are binding this element's data to the visual layout model using a helper method.
            BindLayoutToEditProps(control); //call this to make sure baseclass functionality works.
            BindModelHelper(textblock, "Content", TextBox.TextProperty);
            BindModelHelper(textblock, "FontFamily", TextBox.FontFamilyProperty);
            BindModelHelper(textblock, "FontColor", TextBox.ForegroundProperty);
            BindModelHelper(textblock, "FontMaxSize", TextBox.FontSizeProperty);
            BindModelHelper(textblock, "BackgroundColor", TextBox.BackgroundProperty);
            BindModelHelper(textblock, "BackgroundColor", TextBox.BorderBrushProperty);
            BindModelHelper(textblock, "SpellChecking", SpellCheck.IsEnabledProperty);
            BindModelHelper(textblock, "Alignment", TextBox.TextAlignmentProperty);
            
            //TO BIND: weight/style/Margin 

            //This lets the element know what layout was generated last and
            //is how the designer knows to hi-light newly created elements.
            LastLayoutInstance = control;
            control.CanManipulate = !Locked;

            //return the wrapping control
            return control;
        }

        /// <summary>
        /// Use this to let the system know that we have changed the name of our card
        /// and that it needs to be reflected in the list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextHandler(object sender, TextChangedEventArgs e)
        {
           if(Name.Equals(Card.ElementNameCardBinding)) OnChangeName();
        }
        
        #endregion

    }
}
